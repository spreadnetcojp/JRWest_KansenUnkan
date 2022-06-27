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
''' �΂m�ԒʐM�v���Z�X�̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "�萔��ϐ�"
    '���C���E�B���h�E
    Protected Friend Shared oMainForm As ServerAppForm
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �΂m�ԒʐM�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �΂m�ԒʐM�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppToNkan")
        If m.WaitOne(0, False) Then
            Try
                Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
                If sLogBasePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
                    Return
                End If

                Dim sIniFilePath As String = Constant.GetEnv(REG_SERVER_INI)
                If sIniFilePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_SERVER_INI)
                    Return
                End If

                Log.Init(sLogBasePath, "ToNkan")
                Log.Info("�v���Z�X�J�n")

                Try
                    Lexis.Init(sIniFilePath)
                    Config.Init(sIniFilePath)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End Try

                Log.SetKindsMask(Config.LogKindsMask)

                LocalConnectionProvider.Init()

                '���b�Z�[�W���[�v���A�C�h����ԂɂȂ�O�i���A����I�ɂ�����s��
                '�X���b�h���N������O�j�ɁA�����ؖ��t�@�C�����X�V���Ă����B
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '�ʐM�Ǘ��X���b�h���J�n����B
                Dim oListener As New MyListener()
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

                LocalConnectionProvider.Dispose()
                Config.Dispose()
                Log.Info("�v���Z�X�I��")

                'NOTE: ������ʂ�Ȃ��Ă��A���̃X���b�h�̏��łƂƂ��ɉ�������
                '�悤�Ȃ̂ŁA�ň��̐S�z�͂Ȃ��B
                m.ReleaseMutex()

                Application.Exit()
            End Try
        End If
    End Sub
#End Region

End Class
