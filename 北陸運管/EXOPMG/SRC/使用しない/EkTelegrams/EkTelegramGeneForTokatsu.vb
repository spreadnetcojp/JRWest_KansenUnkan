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
''' �^�ǃT�[�o�Ɠ����T�[�o�̊Ԃ̋��d�������B
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
        Return CInt(objSize) - (ObjCodeLen + CrcLen)
    End Function

    'ObjDetail���̃o�C�g������ObjSize�ɃZ�b�g����ׂ��l���Z�o���郁�\�b�h
    Protected Friend Overrides Function GetObjSizeByObjDetailLen(ByVal objDetailLen As Integer) As UInteger
        Return CUInt(ObjCodeLen + objDetailLen + CrcLen)
    End Function

    'CRC���ɒl���Z�b�g���郁�\�b�h
    Protected Friend Overrides Sub UpdateCrc(ByVal aRawBytes As Byte())
        Dim telegLen As Integer = GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(aRawBytes, ObjSizePos))
        Dim pos As Integer = telegLen - CrcLen
        Dim crc As UShort = Utility.CalculateCRC16(aRawBytes, 0, pos)
        Utility.CopyUInt16ToLeBytes2(crc, aRawBytes, pos)
    End Sub

    'CRC���̒l�Ƃ��̑��̕��ʂ̒l�̐��������`�F�b�N���郁�\�b�h
    Protected Friend Overrides Function IsCrcIndicatingOkay(ByVal aRawBytes As Byte()) As Boolean
        Dim telegLen As Integer = GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(aRawBytes, ObjSizePos))
        Dim pos As Integer = telegLen - CrcLen
        Dim idealCrc As UShort = Utility.CalculateCRC16(aRawBytes, 0, pos)
        Dim crc As UShort = Utility.GetUInt16FromLeBytes2(aRawBytes, pos)
        Return idealCrc = crc
    End Function
End Class
