' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2013/12/10  (NES)小林  バージョン情報(TOICA,ICOCA)追加対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

''' <summary>
''' 窓処プログラムバージョン情報のレコード。
''' </summary>
Public Structure EkMadoProgramVersionInfoElement
    Public Name As String
    Public Value As String
    Public IsVersion As Boolean
End Structure

''' <summary>
''' 窓処のプログラムバージョン情報を読み出すクラス。
''' </summary>
Public Class EkMadoProgramVersionInfoReader

#Region "内部クラス等"
    Structure SourceFormat
        Public Length As Integer
        Public Encoding As String
        Public IsVersion As Boolean
        Public Name As String

        Public Sub New( _
           ByVal length As Integer, _
           ByVal sEncoding As String, _
           ByVal isVersion As Boolean, _
           ByVal sName As String)

            Me.Length = length
            Me.Encoding = sEncoding
            Me.IsVersion = isVersion
            Me.Name = sName
        End Sub
    End Structure
#End Region

#Region "定数"
    'NOTE: これを外部ファイルから読み込むようにすれば、窓処プログラム構成の
    '変更に対し、運管の実装変更なしで対応できるようになる。
    'ただし、運管の実装言語自体がスクリプト言語に近い（比較的簡易に記述でき、
    'コンパイルも手軽である）上、独自書式のファイルを記述するよりも誤りが
    '検出されやすいので、何とも言えない。
    'NOTE: 6バイトBCDの作成年月日は、本当にパックドBCDなのか疑問であるが、
    '想定と違っても変換で例外が発生することなどはないはずであり、
    '今のところ表示に用いることもないため、パックドBCDとみなして
    '変換するようにしている。
    'Ver0.1 MOD START TOICA,ICOCAの運賃データ対応
    Private Shared ReadOnly aSourceFormats() As SourceFormat = { _
        New SourceFormat(1,  "",      False, "対象ユーザコード"), _
        New SourceFormat(1,  "",      False, "適用エリアコード"), _
        New SourceFormat(1,  "",      False, "プログラム区分"), _
        New SourceFormat(4,  "BCD",   False, "プログラム動作許可日"), _
        New SourceFormat(4,  "BCD",   True,  "DLL全体バージョン"), _
        New SourceFormat(4,  "BCD",   False, "DLL適用バージョン"), _
        New SourceFormat(15, "",      False, "予備"), _
        New SourceFormat(2,  "BCD",   True,  "在来IC判定バージョン(Suica)"), _
        New SourceFormat(2,  "BCD",   True,  "在来IC判定バージョン(TOICA)"), _
        New SourceFormat(2,  "BCD",   True,  "在来IC判定バージョン(ICOCA)"), _
        New SourceFormat(2,  "BCD",   True,  "新幹線IC判定バージョン"), _
        New SourceFormat(2,  "BCD",   True,  "EXIC判定バージョン"), _
        New SourceFormat(2,  "ASCII", True,  "Suica運賃データ世代1バージョン"), _
        New SourceFormat(4,  "BCD",   False, "Suica運賃データ世代1適用年月日"), _
        New SourceFormat(2,  "ASCII", True,  "Suica運賃データ世代2バージョン"), _
        New SourceFormat(4,  "BCD",   False, "Suica運賃データ世代2適用年月日"), _
        New SourceFormat(15, "ASCII", False, "Suica運賃データ名"), _
        New SourceFormat(3,  "ASCII", False, "Suica運賃データ全体ソフト型式"), _
        New SourceFormat(2,  "ASCII", True,  "Suica運賃データバージョン"), _
        New SourceFormat(6,  "BCD",   False, "Suica運賃データ作成年月日"), _
        New SourceFormat(2,  "ASCII", True,  "TOICA運賃データ世代1バージョン"), _
        New SourceFormat(4,  "BCD",   False, "TOICA運賃データ世代1適用年月日"), _
        New SourceFormat(2,  "ASCII", True,  "TOICA運賃データ世代2バージョン"), _
        New SourceFormat(4,  "BCD",   False, "TOICA運賃データ世代2適用年月日"), _
        New SourceFormat(15, "ASCII", False, "TOICA運賃データ名"), _
        New SourceFormat(3,  "ASCII", False, "TOICA運賃データ全体ソフト型式"), _
        New SourceFormat(2,  "ASCII", True,  "TOICA運賃データバージョン"), _
        New SourceFormat(6,  "BCD",   False, "TOICA運賃データ作成年月日"), _
        New SourceFormat(2,  "ASCII", True,  "ICOCA運賃データ世代1バージョン"), _
        New SourceFormat(4,  "BCD",   False, "ICOCA運賃データ世代1適用年月日"), _
        New SourceFormat(2,  "ASCII", True,  "ICOCA運賃データ世代2バージョン"), _
        New SourceFormat(4,  "BCD",   False, "ICOCA運賃データ世代2適用年月日"), _
        New SourceFormat(15, "ASCII", False, "ICOCA運賃データ名"), _
        New SourceFormat(3,  "ASCII", False, "ICOCA運賃データ全体ソフト型式"), _
        New SourceFormat(2,  "ASCII", True,  "ICOCA運賃データバージョン"), _
        New SourceFormat(6,  "BCD",   False, "ICOCA運賃データ作成年月日"), _
        New SourceFormat(2,  "ASCII", True,  "特急料金データ世代1バージョン"), _
        New SourceFormat(4,  "BCD",   False, "特急料金データ世代1適用年月日"), _
        New SourceFormat(2,  "ASCII", True,  "特急料金データ世代2バージョン"), _
        New SourceFormat(4,  "BCD",   False, "特急料金データ世代2適用年月日"), _
        New SourceFormat(15, "ASCII", False, "特急料金データ名"), _
        New SourceFormat(3,  "ASCII", False, "特急料金データ全体ソフト型式"), _
        New SourceFormat(2,  "ASCII", True,  "特急料金データバージョン"), _
        New SourceFormat(6,  "BCD",   False, "特急料金データ作成年月日"), _
        New SourceFormat(2,  "BCD",   True,  "磁気ファームウェアバージョン"), _
        New SourceFormat(14, "",      False, "予備"), _
        New SourceFormat(1,  "",      False, "未締切時プログラム非適用チェックフラグ"), _
        New SourceFormat(1,  "",      False, "未送有時プログラム非適用チェックフラグ"), _
        New SourceFormat(46, "",      False, "備考")}

    Private Const _Length As Integer = 256
    'Ver0.1 MOD END TOICA,ICOCAの運賃データ対応
#End Region

#Region "メソッド"
    'NOTE: ファイルの長さが短い場合などには、IOExceptionをスローします。
    'NOTE: 解釈が不可能な場合は、FormatExceptionをスローします。
    Public Shared Function GetElementsFromStream(ByVal oInputStream As Stream) As EkMadoProgramVersionInfoElement()
        Dim RawBytes(_Length - 1) As Byte
        Dim pos As Integer = 0

        Do
            Dim readLimit As Integer = _Length - pos
            If readLimit = 0 Then Exit Do
            Dim readSize As Integer = oInputStream.Read(RawBytes, pos, readLimit)
            If readSize = 0 Then
                Throw New EndOfStreamException()
            End If
            pos += readSize
        Loop

        pos = 0
        Dim aInfoElements As EkMadoProgramVersionInfoElement() = New EkMadoProgramVersionInfoElement(aSourceFormats.Length - 1) {}
        For i As Integer = 0 To aSourceFormats.Length - 1
            aInfoElements(i).Name = aSourceFormats(i).Name
            aInfoElements(i).IsVersion = aSourceFormats(i).IsVersion

            Dim len As Integer = aSourceFormats(i).Length
            Select Case aSourceFormats(i).Encoding
                Case "BCD"
                    'NOTE: 以下は、試供された部材のバージョン情報（プログラムバージョンリスト）
                    'において、BCD項目に0～9以外がセットされているため、それを許容するために
                    'コメントアウトしてある。
                    'If Not Utility.IsBcdBytes(RawBytes, pos, len) Then
                    '    Throw New FormatException("Element #" & i.ToString() & " is invalid (not BCD bytes).")
                    'End If

                    aInfoElements(i).Value = BitConverter.ToString(RawBytes, pos, len).Replace("-", "")

                    '0x00で埋められている場合は特殊扱い（バージョン無し）とする。
                    If aSourceFormats(i).IsVersion Then
                        Dim bitSum As Byte = 0
                        For p As Integer = pos To pos + len - 1
                            bitSum = BitSum Or RawBytes(p)
                        Next
                        If bitSum = 0 Then
                            aInfoElements(i).Value = ""
                        End If
                    End If

                Case "ASCII"
                    If Not Utility.IsVisibleAsciiBytes(RawBytes, pos, len) Then
                        Throw New FormatException("Element #" & i.ToString() & " is invalid (not visible ASCII bytes).")
                    End If

                    aInfoElements(i).Value = Encoding.UTF8.GetString(RawBytes, pos, len).TrimEnd(Chr(0))

                Case Else
                    Debug.Assert(aSourceFormats(i).IsVersion = False)
            End Select

            pos += len
        Next
        Return aInfoElements
    End Function
#End Region

End Class
