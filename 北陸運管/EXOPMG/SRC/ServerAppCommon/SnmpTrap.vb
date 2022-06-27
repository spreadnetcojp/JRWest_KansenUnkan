' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/07/17  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応にて号機番号の型を変更
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Runtime.InteropServices

Imports JR.ExOpmg.Common

Public Enum SnmpStatusCode As Integer
    Connect = 1
    Disconnect = 2
End Enum

Public Class SnmpTrap

    Private Declare Ansi Function DoTrap Lib "JRSNMP_CALLX64.DLL" _
       Alias "JRSNMP_CALL" ( _
        ByVal appNumber As Integer, _
        ByVal railSectionCode As Integer, _
        ByVal stationOrderCode As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal sCornerCode As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal sModelCode As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal sModelType As String, _
        ByVal unitNumber As Integer, _
        ByVal portNumber As Integer, _
        ByVal status As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal sErrorCode As String) As Integer

    Public Shared Sub Act( _
       ByVal appNumber As Integer, _
       ByVal sClientModel As String, _
       ByVal clientCode As EkCode, _
       ByVal portNumber As Integer, _
       ByVal status As SnmpStatusCode)

        Dim result As Integer = -1

        Try
            result = DoTrap( _
             appNumber, ServerAppBaseConfig.SelfArea * 1000 + clientCode.RailSection, _
             clientCode.StationOrder, clientCode.Corner.ToString("D4"), _
             sClientModel, EkConstants.ProductCodeOfModels(sClientModel).PadRight(12, Chr(0)), _
             clientCode.Unit, portNumber, status, Nothing)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try

        Dim sParam As String = appNumber.ToString() & ", " _
           & sClientModel & ", " _
           & clientCode.ToString("%3R%3S_%4C_%2U") & ", " _
           & portNumber.ToString() & ", " _
           & status.ToString()
        If result = 0 Then
            Log.Info("SNMP TRAP [" & sParam & "] succeeded.")
        Else
            Log.Error("SNMP TRAP [" & sParam & "] failed.")
        End If
    End Sub

    Public Shared Sub Act( _
       ByVal appNumber As Integer, _
       ByVal sModel As String, _
       ByVal sRailSectionCode As String, _
       ByVal sStationOrderCode As String, _
       ByVal sCornerCode As String, _
       ByVal unitNumber As Integer, _
       ByVal portNumber As Integer, _
       ByVal sErrorCode As String)

        Dim result As Integer = -1

        If sErrorCode IsNot Nothing Then
            sErrorCode = sErrorCode.PadLeft(8, "0"c)
        End If

        Try
            result = DoTrap( _
             appNumber, ServerAppBaseConfig.SelfArea * 1000 + Integer.Parse(sRailSectionCode), _
             Integer.Parse(sStationOrderCode), sCornerCode, _
             sModel, EkConstants.ProductCodeOfModels(sModel).PadRight(12, Chr(0)),
             unitNumber, portNumber, SnmpStatusCode.Disconnect, sErrorCode)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try

        Dim sParam As String = appNumber.ToString() & ", " _
           & sModel & ", " _
           & sRailSectionCode & ", " _
           & sStationOrderCode & ", " _
           & sCornerCode & ", " _
           & unitNumber.ToString() & ", " _
           & portNumber.ToString() & ", " _
           & Utility.CNull(sErrorCode, "Nothing")

        If result = 0 Then
            Log.Info("SNMP TRAP [" & sParam & "] succeeded.")
        Else
            Log.Error("SNMP TRAP [" & sParam & "] failed.")
        End If
    End Sub

End Class
