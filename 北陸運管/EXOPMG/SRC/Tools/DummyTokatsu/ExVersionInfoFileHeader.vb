'NOTE: ���g�p
' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/02/16  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �^�p�n���f�[�^�̊�{�w�b�_�����t�@�C���ɏ������ރN���X�B
''' </summary>
Public Class ExVersionInfoFileHeader

    Public Const Length As Integer = 17

    'NOTE: �����o���Ȃ��ꍇ�ȂǂɁAIOException���X���[�����܂��B
    Public Shared Sub WriteToStream(ByVal dataKind As Byte, ByVal machineCode As EkCode, ByVal procDate As DateTime, ByVal version As Byte, ByVal oOutputStream As Stream)
        Dim RawBytes(Length - 1) As Byte
        Dim pos As Integer = 0
        RawBytes(pos) = dataKind
        pos += 1
        RawBytes(pos) = CType(machineCode.RailSection, Byte)
        pos += 1
        RawBytes(pos) = CType(machineCode.StationOrder, Byte)
        pos += 1
        Utility.CHARtoBCD(procDate.ToString("yyyyMMddHHmmss"), 7).CopyTo(RawBytes, pos)
        pos += 7
        RawBytes(pos) = CType(machineCode.Corner, Byte)
        pos += 1
        RawBytes(pos) = CType(machineCode.Unit, Byte)
        pos += 1
        Utility.CopyUInt32ToLeBytes4(0, RawBytes, pos)
        pos += 4
        RawBytes(pos) = version
        pos += 1
        oOutputStream.Write(RawBytes, 0, RawBytes.Length)
    End Sub

End Class
