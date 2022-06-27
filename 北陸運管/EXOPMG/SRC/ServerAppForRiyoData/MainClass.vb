' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/04/10  (NES)����  ������ԕ�Ή��ɂĐV�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading
Imports System.Diagnostics

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp

''' <summary>
''' ���p�f�[�^�o�^�v���Z�X���ʂ̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "���\�b�h"
    ''' <summary>
    '''  ���p�f�[�^�o�^�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    '''  ���p�f�[�^�o�^�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForRiyoData")
        If m.WaitOne(0, False) Then
            Dim oMainForm As ServerAppForm = Nothing
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

                Log.Init(sLogBasePath, "ForRiyoData")
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

                '���b�Z�[�W���[�v���A�C�h����ԂɂȂ�O�i���A�����ؖ��t�@�C���̍X�V��
                '�������邱�ƂɂȂ�Ď��X���b�h���N������O�j�ɁA�����ؖ��t�@�C����
                '�X�V���Ă����B
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '�Ď��X���b�h���J�n����B
                Dim oWatcher As New MyWatcher(oMainForm)
                Log.Info("Starting the watcher thread...")
                oWatcher.Start()

                '�E�C���h�E�v���V�[�W�������s����B
                'NOTE: ���̃��\�b�h�����O���X���[����邱�Ƃ͂Ȃ��B
                ServerAppBaseMain(oMainForm)

                Try
                    '�Ď��X���b�h�ɏI����v������B
                    Log.Info("Sending quit request to the watcher thread...")
                    oWatcher.Quit()

                    'NOTE: �ȉ��ŊĎ��X���b�h���I�����Ȃ��ꍇ�A
                    '�Ď��X���b�h�͐����ؖ����s��Ȃ��͂��ł���A
                    '�󋵂ւ̑Ώ��̓v���Z�X�}�l�[�W���ōs����z��ł���B

                    '�Ď��X���b�h�̏I����҂B
                    Log.Info("Waiting for the watcher thread to quit...")
                    oWatcher.Join()
                    Log.Info("The watcher thread has quit.")
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oWatcher.Abort()
                End Try
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                If oMainForm IsNot Nothing Then
                    oMainForm.Dispose()
                End If

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
