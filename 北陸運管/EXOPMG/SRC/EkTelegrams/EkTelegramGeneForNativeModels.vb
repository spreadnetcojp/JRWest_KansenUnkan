' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Net.Sockets

Imports JR.ExOpmg.Common

''' <summary>
''' �^�ǃT�[�o�ƊĎ��Ռn�@��̊Ԃ̓d�������B
''' </summary>
''' <remarks>
''' �C���X�^���X�𐶐�����ۂɂ̂ݎw�肷��N���X�B�C���X�^���X��
''' �������_�ŁAEkTelegramGene�̎Q�ƌ^�ϐ��ɃZ�b�g���A����ȍ~�A
''' �{���C�u�����̓���������O��������AEkTelegramGene�Ƃ���
''' �A�N�Z�X����B
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

    'ObjSize����d���S�̂̃o�C�g�����Z�o���郁�\�b�h
    Protected Friend Overrides Function GetRawLenByObjSize(ByVal objSize As UInteger) As Integer
        Return ObjCodePos + CInt(objSize)
    End Function

    '�d���S�̃o�C�g������ObjSize�ɃZ�b�g����ׂ��l���Z�o���郁�\�b�h
    Protected Friend Overrides Function GetObjSizeByRawLen(ByVal rawLen As Integer) As UInteger
        Return CUInt(rawLen - ObjCodePos)
    End Function

    'ObjSize����ObjDetail���̃o�C�g�����Z�o���郁�\�b�h
    Protected Friend Overrides Function GetObjDetailLenByObjSize(ByVal objSize As UInteger) As Integer
        Return CInt(objSize) - (ObjCodeLen + ObjCrcLen)
    End Function

    'ObjDetail���̃o�C�g������ObjSize�ɃZ�b�g����ׂ��l���Z�o���郁�\�b�h
    Protected Friend Overrides Function GetObjSizeByObjDetailLen(ByVal objDetailLen As Integer) As UInteger
        Return CUInt(ObjCodeLen + objDetailLen + ObjCrcLen)
    End Function

    'CRC���ɒl���Z�b�g���郁�\�b�h
    Protected Friend Overrides Sub UpdateCrc(ByVal aRawBytes As Byte())
        Dim headerCrc As UShort = Utility.CalculateCRC16(aRawBytes, 0, HeaderCrcPos)
        Utility.CopyUInt16ToLeBytes2(headerCrc, aRawBytes, HeaderCrcPos)

        Dim telegLen As Integer = GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(aRawBytes, ObjSizePos))
        Dim objCrcPos As Integer = telegLen - ObjCrcLen
        Dim objCrc As UShort = Utility.CalculateCRC16(aRawBytes, ObjCodePos, objCrcPos - ObjCodePos)
        Utility.CopyUInt16ToLeBytes2(objCrc, aRawBytes, objCrcPos)
    End Sub

    'CRC���̒l�Ƃ��̑��̕��ʂ̒l�̐��������`�F�b�N���郁�\�b�h
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
