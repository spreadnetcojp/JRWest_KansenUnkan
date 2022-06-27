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
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' �ʐM�v���Z�X���ʂ̃��C����������������N���X�B
''' </summary>
Public Class TelServerAppBaseMainClass
    Inherits ServerAppBaseMainClass

#Region "�萔��ϐ�"
    '���C���E�B���h�E
    Protected Friend Shared oMainForm As ServerAppForm
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �ʐM�v���Z�X�̋��ʃ��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �e�ʐM�v���Z�X�̃��C����������Ăяo���B
    ''' </remarks>
    Protected Shared Sub TelServerAppBaseMain(ByVal oListener As TelServerAppListener)
        Try
            '���b�Z�[�W���[�v���A�C�h����ԂɂȂ�O�i���A����I�ɂ�����s��
            '�X���b�h���N������O�j�ɁA�����ؖ��t�@�C�����X�V���Ă����B
            Directory.CreateDirectory(TelServerAppBaseConfig.ResidentAppPulseDirPath)
            ServerAppPulser.Pulse()

            oMainForm = New ServerAppForm()

            '�ʐM�Ǘ��X���b�h���J�n����B
            Log.Info("Starting the listener thread...")
            oListener.Start()

            '�E�C���h�E�v���V�[�W�������s����B
            'NOTE: ���̃��\�b�h�����O���X���[����邱�Ƃ͂Ȃ��B
            ServerAppBaseMain(oMainForm)

            Try
                '�ʐM�Ǘ��X���b�h�ɏI����v������B
                Log.Info("Sending quit request to the listener thread...")
                oListener.Quit()

                'NOTE: �ȉ��ŒʐM�Ǘ��X���b�h���I�����Ȃ��ꍇ�A
                '�ʐM�Ǘ��X���b�h�͐����ؖ����s��Ȃ��͂��ł���A
                '�󋵂ւ̑Ώ��̓v���Z�X�}�l�[�W���ōs����z��ł���B

                '�ʐM�Ǘ��X���b�h�̏I����҂B
                Log.Info("Waiting for the listener thread to quit...")
                oListener.Join()
                Log.Info("The listener thread has quit.")
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                oListener.Abort()
            End Try
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            If oMainForm IsNot Nothing Then
                oMainForm.Dispose()
            End If
        End Try
    End Sub
#End Region

End Class
