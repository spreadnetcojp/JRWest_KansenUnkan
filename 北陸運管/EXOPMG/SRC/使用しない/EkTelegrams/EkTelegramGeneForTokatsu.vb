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

Imports System.Net.Sockets

Imports JR.ExOpmg.Common

''' <summary>
''' 運管サーバと統括サーバの間の旧電文書式。
''' </summary>
Public Class EkTelegramGeneForTokatsu
    Inherits EkTelegramGene

    Private Const ReservedAreaLen As Integer = 8
    Private Const CrcLen As Integer = 2

    Public Sub New(ByVal sXllBasePath As String)
        Me.CmdCodePos = 0
        Me.SubCmdCodePos = CmdCodePos + CmdCodeLen
        Me.ReqNumberPos = SubCmdCodePos + SubCmdCodeLen
        Me.ClientModelCodePos = ReqNumberPos + ReqNumberLen
        Me.ClientRailSectionCodePos = ClientModelCodePos + ClientModelCodeLen
        Me.ClientStationOrderCodePos = ClientRailSectionCodePos + ClientRailSectionCodeLen
        Me.ClientCornerCodePos = ClientStationOrderCodePos + ClientStationOrderCodeLen
        Me.ClientUnitCodePos = ClientCornerCodePos + ClientCornerCodeLen
        Me.SendTimePos = ClientUnitCodePos + ClientUnitCodeLen
        Me.ObjSizePos = SendTimePos + SendTimeLen
        Dim ReservedAreaPos As Integer = ObjSizePos + ObjSizeLen
        Me.ObjCodePos = ReservedAreaPos + ReservedAreaLen
        Me.ObjDetailPos = ObjCodePos + ObjCodeLen

        Me.MinAllocSize = 256
        Me.MaxReceiveSize = 512 * 1024

        Me.XllBasePath = sXllBasePath
    End Sub

    'ObjSizeから電文全体のバイト長を算出するメソッド
    Protected Friend Overrides Function GetRawLenByObjSize(ByVal objSize As UInteger) As Integer
        Return ObjCodePos + CInt(objSize)
    End Function

    '電文全体バイト長からObjSizeにセットするべき値を算出するメソッド
    Protected Friend Overrides Function GetObjSizeByRawLen(ByVal rawLen As Integer) As UInteger
        Return CUInt(rawLen - ObjCodePos)
    End Function

    'ObjSizeからObjDetail部のバイト長を算出するメソッド
    Protected Friend Overrides Function GetObjDetailLenByObjSize(ByVal objSize As UInteger) As Integer
        Return CInt(objSize) - (ObjCodeLen + CrcLen)
    End Function

    'ObjDetail部のバイト長からObjSizeにセットするべき値を算出するメソッド
    Protected Friend Overrides Function GetObjSizeByObjDetailLen(ByVal objDetailLen As Integer) As UInteger
        Return CUInt(ObjCodeLen + objDetailLen + CrcLen)
    End Function

    'CRC部に値をセットするメソッド
    Protected Friend Overrides Sub UpdateCrc(ByVal aRawBytes As Byte())
        Dim telegLen As Integer = GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(aRawBytes, ObjSizePos))
        Dim pos As Integer = telegLen - CrcLen
        Dim crc As UShort = Utility.CalculateCRC16(aRawBytes, 0, pos)
        Utility.CopyUInt16ToLeBytes2(crc, aRawBytes, pos)
    End Sub

    'CRC部の値とその他の部位の値の整合性をチェックするメソッド
    Protected Friend Overrides Function IsCrcIndicatingOkay(ByVal aRawBytes As Byte()) As Boolean
        Dim telegLen As Integer = GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(aRawBytes, ObjSizePos))
        Dim pos As Integer = telegLen - CrcLen
        Dim idealCrc As UShort = Utility.CalculateCRC16(aRawBytes, 0, pos)
        Dim crc As UShort = Utility.GetUInt16FromLeBytes2(aRawBytes, pos)
        Return idealCrc = crc
    End Function
End Class
