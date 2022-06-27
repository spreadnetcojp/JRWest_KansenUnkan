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
''' �ێ�n�f�[�^�̃t�@�C���w�b�_�����������ރN���X�B
''' </summary>
Public Class ExUpboundFileHeader

    'NOTE: �������߂Ȃ��ꍇ�ȂǂɁAIOException���X���[�����܂��B
    Public Shared Sub WriteToStream(ByVal dataKind As Byte, ByVal recCount As Integer, ByVal recLength As Integer, ByVal creationDate As DateTime, ByVal oOutputStream As Stream)
        Dim RawBytes(recLength - 1) As Byte
        Dim pos As Integer = 0
        RawBytes(pos) = dataKind
        pos += 1
        Utility.CopyUInt16ToLeBytes2(CType(recCount, UShort), RawBytes, pos)
        pos += 2
        Utility.CHARtoBCD(creationDate.ToString("yyyyMMddHHmmss"), 7).CopyTo(RawBytes, pos)
        pos += 7
        oOutputStream.Write(RawBytes, 0, RawBytes.Length)
    End Sub

End Class
