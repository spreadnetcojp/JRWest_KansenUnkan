' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/02/16  (NES)小林  EkMasterVersion.vbをもとに作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' マスタバージョン情報を書き出す際に使用するクラス。
''' </summary>
Public Class ExMasterVersionInfo

    Public Const Length As Integer = (1 + 2) * 50

    Public Shared ReadOnly Kinds() As String = { _
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

    'NOTE: 書き出せない場合などに、IOExceptionをスローし得ます。
    Public Shared Sub WriteToStream(ByVal oMasters As Dictionary(Of String, HoldingMaster), ByVal oOutputStream As Stream)
        Dim RawBytes(Length - 1) As Byte
        Dim pos As Integer = 0
        For i As Integer = 0 To Kinds.Length - 1
            Dim oMaster As HoldingMaster = Nothing
            If Kinds(i).Length <> 0 AndAlso oMasters.TryGetValue(Kinds(i), oMaster) = True Then
                Utility.CopyIntToBcdBytes(oMaster.DataSubKind, RawBytes, pos, 1)
                pos += 1
                Utility.CopyIntToBcdBytes(oMaster.DataVersion, RawBytes, pos, 2)
                pos += 2
            Else
                Utility.CopyIntToBcdBytes(0, RawBytes, pos, 1)
                pos += 1
                Utility.CopyIntToBcdBytes(0, RawBytes, pos, 2)
                pos += 2
            End If
        Next i
        oOutputStream.Write(RawBytes, 0, RawBytes.Length)
    End Sub

End Class
