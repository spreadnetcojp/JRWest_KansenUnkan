' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

''' <summary>
''' 監視盤プログラムバージョン情報と改札機プログラムバージョン情報のグループ見出し。
''' </summary>
Public Structure EkProgramVersionInfoElementGroupHeader
    Public ElementCount As Integer
End Structure

''' <summary>
''' 監視盤プログラムバージョン情報と改札機プログラムバージョン情報のレコード。
''' </summary>
Public Structure EkProgramVersionInfoElement
    Public FileName As String
    Public Version As String
    Public DispName As String
End Structure

''' <summary>
''' 監視盤プログラムバージョン情報と改札機プログラムバージョン情報を読み出す抽象クラス。
''' </summary>
Public MustInherit Class EkProgramVersionInfoReader

#Region "定数"
    Protected Const GroupHeaderLen As Integer = 10
    Protected Const ElementCountPos As Integer = 8
    Protected Const ElementLen As Integer = 108
    Protected Const FileNamePos As Integer = 0
    Protected Const FileNameLen As Integer = 12
    Protected VersionPos As Integer
    Protected VersionLen As Integer
    Protected DispNamePos As Integer
    Protected DispNameLen As Integer
#End Region

#Region "メソッド"
    'NOTE: ファイルの長さが短い場合などには、IOExceptionをスローします。
    'NOTE: 解釈が不可能な場合は、FormatExceptionをスローします。
    Public Overridable Function GetOneGroupHeaderFromStream(ByVal oInputStream As Stream) As EkProgramVersionInfoElementGroupHeader
        Dim pos As Integer = 0
        Dim RawBytes(GroupHeaderLen - 1) As Byte
        Do
            Dim readLimit As Integer = GroupHeaderLen - pos
            If readLimit = 0 Then Exit Do
            Dim readSize As Integer = oInputStream.Read(RawBytes, pos, readLimit)
            If readSize = 0 Then
                Throw New EndOfStreamException()
            End If
            pos += readSize
        Loop

        Dim ret As New EkProgramVersionInfoElementGroupHeader()
        ret.ElementCount = Utility.GetUInt16FromLeBytes2(RawBytes, ElementCountPos)
        Return ret
    End Function

    'NOTE: ファイルの長さが短い場合などには、IOExceptionをスローします。
    'NOTE: 解釈が不可能な場合は、FormatExceptionをスローします。
    Public Overridable Function GetOneGroupElementsFromStream(ByVal oInputStream As Stream, ByVal groupHeader As EkProgramVersionInfoElementGroupHeader) As EkProgramVersionInfoElement()
        Dim numElems As Integer = groupHeader.ElementCount
        Dim Length As Integer = numElems * ElementLen

        Dim pos As Integer = 0
        Dim RawBytes(Length - 1) As Byte
        Do
            Dim readLimit As Integer = Length - pos
            If readLimit = 0 Then Exit Do
            Dim readSize As Integer = oInputStream.Read(RawBytes, pos, readLimit)
            If readSize = 0 Then
                Throw New EndOfStreamException()
            End If
            pos += readSize
        Loop

        pos = 0
        Dim aInfoElements As EkProgramVersionInfoElement() = New EkProgramVersionInfoElement(numElems - 1) {}
        For i As Integer = 0 To numElems - 1
            'ファイル名を抜き出す。
            'NOTE: プログラムバージョン情報そのものにおける仕様は「JIS」となっているが、
            '比較対象となるFILELIST.TXTの仕様は「ASCII」であるため、ASCIIで定義されない
            '文字が入ってくることはあり得ない（駅務機器側の処理に異常がある）ものとみなす。
            If Not Utility.IsVisibleAsciiBytes(RawBytes, pos + FileNamePos, FileNameLen) Then
                Throw New FormatException("FileName of Element #" & i.ToString() & " is invalid (not visible ASCII bytes).")
            End If
            'TODO: 従来機が受信した改札機プログラムバージョン情報の実データから、
            'この領域の余りはヌル文字で埋められると想定して実装している。もし、
            '0x20で埋められるなら「.TrimEnd(Chr(&H20))」に変更すること。
            aInfoElements(i).FileName = Encoding.UTF8.GetString(RawBytes, pos + FileNamePos, FileNameLen).TrimEnd(Chr(0))

            'バージョンを抜き出す。
            If Not Utility.IsVisibleAsciiBytes(RawBytes, pos + VersionPos, VersionLen) Then
                Throw New FormatException("Version of Element #" & i.ToString() & " is invalid (not visible ASCII bytes).")
            End If
            aInfoElements(i).Version = Encoding.UTF8.GetString(RawBytes, pos + VersionPos, VersionLen).TrimEnd(Chr(0))

            '表示名を抜き出す。
            Try
                aInfoElements(i).DispName = Encoding.GetEncoding(932).GetString(RawBytes, pos + DispNamePos, DispNameLen).TrimEnd(Chr(&H20))
            Catch ex As DecoderFallbackException
                Throw New FormatException("DispName of Element #" & i.ToString() & " is invalid.")
            End Try

            pos += ElementLen
        Next
        Return aInfoElements
    End Function
#End Region

End Class
