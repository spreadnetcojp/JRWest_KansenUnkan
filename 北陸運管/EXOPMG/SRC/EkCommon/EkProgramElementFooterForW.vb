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

'NOTE: ���݂̂Ƃ���A�g�p���Ă��Ȃ��B

''' <summary>
''' �Ď��Ճv���O�����̗v�f�t�@�C���̃t�b�^�B
''' </summary>
Public Class EkProgramElementFooterForW
    Inherits EkProgramElementFooter

    'NOTE: sFooteredFilePath�Ƀt�@�C�����Ȃ��ꍇ��A�t�@�C���̒������Z���ꍇ�Ȃǂɂ́A
    'IOException���X���[���܂��B
    Public Sub New(ByVal sFooteredFilePath As String)
        MyBase.New(sFooteredFilePath)

        Me.VersionPos = 8
        Me.VersionLen = 8
        Me.DispNamePos = 24
        Me.DispNameLen = 64
    End Sub

End Class
