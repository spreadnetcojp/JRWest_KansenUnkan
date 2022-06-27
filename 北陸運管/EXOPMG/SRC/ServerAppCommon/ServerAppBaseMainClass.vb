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

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �^�ǃT�[�o�S�v���Z�X���ʂ̃��C����������������N���X�B
''' </summary>
Public Class ServerAppBaseMainClass

#Region "�萔��ϐ�"
    '���O�t�@�C���o�͐�f�B���N�g���w��p���ϐ��̖��O
    Protected Const REG_LOG As String = "EXOPMG_LOG_DIR"

    '�T�[�o�pINI�t�@�C���w��p���ϐ��̖��O
    Protected Const REG_SERVER_INI As String = "EXOPMG_INIFILE_SERVER"
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �S�v���Z�X�̋��ʃ��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �e�v���Z�X�̃��C����������Ăяo���B
    ''' </remarks>
    Protected Shared Sub ServerAppBaseMain(ByVal oForm As ServerAppForm)
        Try
            '��ʂ�\������iUI�p���b�Z�[�W���[�v���s����j�B
            Log.Info("��ʕ\�������J�n")
            oForm.ShowDialog()
            Log.Info("��ʕ\�������I��")
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub
#End Region

End Class
