' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2014/06/01       金沢  新規マスタ追加対応
'   0.2      2017/05/22  (NES)河脇  ポイントポストペイ対応
'                                     マスタ追加（昼特区間時間、ポストペイエリアマスタ）
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

''' <summary>
''' マスタバージョン情報のレコード。
''' </summary>
Public Structure EkMasterVersionInfoElement
    Public Kind As String
    Public SubKind As String
    Public Version As String
End Structure

''' <summary>
''' マスタバージョン情報を読み出すクラス。
''' </summary>
Public Class EkMasterVersionInfoReader

#Region "定数"
    '----------- 0.1  新規マスタ追加対応   ADD  START------------------------
    '----------- 0.2  ポイントポストペイ対応   MOD  START------------------------
    Private Shared ReadOnly aKinds() As String = { _
        "KEN",
        "DLY",
        "PAY",
        "",
        "ICD",
        "",
        "LOS",
        "DSC",
        "HLD",
        "EXP",
        "FRX",
        "ICH",
        "FJW",
        "IJW",
        "FJC",
        "IJC",
        "FJR",
        "DSH",
        "LST",
        "IJE",
        "CYC",
        "STP",
        "PNO",
        "FRC",
        "",
        "DUS",
        "NSI",
        "NTO",
        "NIC",
        "NJW",
        "",
        "FSK",
        "IUZ",
        "KSZ",
        "IUK",
        "SWK",
        "HIR",
        "PPA",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        ""}
    '----------- 0.2  ポイントポストペイ対応   MOD    END------------------------
    '----------- 0.1  新規マスタ追加対応   ADD    END------------------------
    Private Const _Length As Integer = (1 + 2) * 50
#End Region

#Region "メソッド"
    'NOTE: ファイルの長さが短い場合などには、IOExceptionをスローします。
    'NOTE: 解釈が不可能な場合は、FormatExceptionをスローします。
    Public Shared Function GetElementsFromStream(ByVal oInputStream As Stream) As EkMasterVersionInfoElement()
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
        Dim aInfoElements As EkMasterVersionInfoElement() = New EkMasterVersionInfoElement(aKinds.Length - 1) {}
        For i As Integer = 0 To aKinds.Length - 1
            aInfoElements(i).Kind = aKinds(i)

            If Not Utility.IsBcdBytes(RawBytes, pos, 1) Then
                Throw New FormatException("PatternNumber of Element #" & i.ToString() & " is invalid (not BCD bytes).")
            End If

            Dim intSubKind As Integer = Utility.GetIntFromBcdBytes(RawBytes, pos, 1)
            aInfoElements(i).SubKind = intSubKind.ToString("D2")
            pos += 1

            If Not Utility.IsBcdBytes(RawBytes, pos, 2) Then
                Throw New FormatException("VersionNumber of Element #" & i.ToString() & " is invalid (not BCD bytes).")
            End If

            Dim intVersion As Integer = Utility.GetIntFromBcdBytes(RawBytes, pos, 2)
            aInfoElements(i).Version = intVersion.ToString("D3")
            pos += 2
        Next
        Return aInfoElements
    End Function
#End Region

End Class
