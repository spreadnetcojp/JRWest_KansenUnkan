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

''' <summary>
''' ���D�@�v���O�����̗v�f�t�@�C���̃t�b�^�B
''' </summary>
Public Class EkProgramElementFooterForG
    Inherits EkProgramElementFooter

    'NOTE: sFooteredFilePath�Ƀt�@�C�����Ȃ��ꍇ��A�t�@�C���̒������Z���ꍇ�Ȃǂɂ́A
    'IOException���X���[���܂��B
    Public Sub New(ByVal sFooteredFilePath As String)
        MyBase.New(sFooteredFilePath)

        Me.VersionPos = 20
        Me.VersionLen = 2
        Me.DispNamePos = 28
        Me.DispNameLen = 60
    End Sub

End Class
