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
''' 運管サーバと監視盤系機器の間の電文書式。
''' </summary>
''' <remarks>
''' インスタンスを生成する際にのみ指定するクラス。インスタンスは
''' 生成時点で、EkTelegramGeneの参照型変数にセットし、それ以降、
''' 本ライブラリの内部からも外部からも、EkTelegramGeneとして
''' アクセスする。
''' </remarks>
Public Class EkTelegramGeneForNativeModels
    Inherits EkTelegramGene

    Private Const ReservedAreaLen As Integer = 8
    Private Const HeaderCrcLen As Integer = 2
    Private Const ObjCrcLen As Integer = 2

    Private HeaderCrcPos As Integer

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
        Me.HeaderCrcPos = ReservedAreaPos + ReservedAreaLen
        Me.ObjCodePos = HeaderCrcPos + HeaderCrcLen
        Me.ObjDetailPos = ObjCodePos + ObjCodeLen

        Me.MinAllocSize = 256
        Me.MaxReceiveSize = 2 * 1024 * 1024

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
        Return CInt(objSize) - (ObjCodeLen + ObjCrcLen)
    End Function

    'ObjDetail部のバイト長からObjSizeにセットするべき値を算出するメソッド
    Protected Friend Overrides Function GetObjSizeByObjDetailLen(ByVal objDetailLen As Integer) As UInteger
        Return CUInt(ObjCodeLen + objDetailLen + ObjCrcLen)
    End Function

    'CRC部に値をセットするメソッド
    Protected Friend Overrides Sub UpdateCrc(ByVal aRawBytes As Byte())
        Dim headerCrc As UShort = Utility.CalculateCRC16(aRawBytes, 0, HeaderCrcPos)
        Utility.CopyUInt16ToLeBytes2(headerCrc, aRawBytes, HeaderCrcPos)

        Dim telegLen As Integer = GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(aRawBytes, ObjSizePos))
        Dim objCrcPos As Integer = telegLen - ObjCrcLen
        Dim objCrc As UShort = Utility.CalculateCRC16(aRawBytes, ObjCodePos, objCrcPos - ObjCodePos)
        Utility.CopyUInt16ToLeBytes2(objCrc, aRawBytes, objCrcPos)
    End Sub

    'CRC部の値とその他の部位の値の整合性をチェックするメソッド
    Protected Friend Overrides Function IsCrcIndicatingOkay(ByVal aRawBytes As Byte()) As Boolean
        Dim headerCrcByBytes As UShort = Utility.CalculateCRC16(aRawBytes, 0, HeaderCrcPos)
        Dim headerCrc As UShort = Utility.GetUInt16FromLeBytes2(aRawBytes, HeaderCrcPos)
        If headerCrcByBytes <> headerCrc Then Return False

        Dim telegLen As Integer = GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(aRawBytes, ObjSizePos))
        Dim objCrcPos As Integer = telegLen - ObjCrcLen
        Dim objCrcByBytes As UShort = Utility.CalculateCRC16(aRawBytes, ObjCodePos, objCrcPos - ObjCodePos)
        Dim objCrc As UShort = Utility.GetUInt16FromLeBytes2(aRawBytes, objCrcPos)
        Return objCrcByBytes = objCrc
    End Function
End Class
