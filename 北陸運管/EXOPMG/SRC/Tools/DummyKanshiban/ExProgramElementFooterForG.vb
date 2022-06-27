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

Imports JR.ExOpmg.Common

''' <summary>
''' ���D�@�v���O�����̗v�f�t�@�C���̃t�b�^�B
''' </summary>
Public Class ExProgramElementFooterForG
    Inherits EkProgramElementFooterForG

    Public Property Data() As Byte()
        Get
            Dim oBytes As Byte() = New Byte(Length - 1) {}
            Buffer.BlockCopy(RawBytes, 0, oBytes, 0, Length)
            Return oBytes
        End Get

        Set(ByVal oBytes As Byte())
            Buffer.BlockCopy(oBytes, 0, RawBytes, 0, Length)
        End Set
    End Property

    'NOTE: sFooteredFilePath�Ƀt�@�C�����Ȃ��ꍇ��A�t�@�C���̒������Z���ꍇ�Ȃǂɂ́A
    'IOException���X���[���܂��B
    Public Sub New(ByVal sFooteredFilePath As String)
        MyBase.New(sFooteredFilePath)
    End Sub

End Class
