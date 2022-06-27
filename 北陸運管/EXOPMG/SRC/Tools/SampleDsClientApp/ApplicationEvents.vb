' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/05/13  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO

Imports JR.ExOpmg.Common

Namespace My

    ' ���̃C�x���g�� MyApplication �ɑ΂��ė��p�ł��܂�:
    '
    ' Startup: �A�v���P�[�V�������J�n���ꂽ�Ƃ��A�X�^�[�g�A�b�v �t�H�[�����쐬�����O�ɔ������܂��B
    ' Shutdown: �A�v���P�[�V���� �t�H�[�������ׂĕ���ꂽ��ɔ������܂��B���̃C�x���g�́A�ʏ�̏I���ȊO�̕��@�ŃA�v���P�[�V�������I�����ꂽ�Ƃ��ɂ͔������܂���B
    ' UnhandledException: �n���h������Ă��Ȃ���O���A�v���P�[�V�����Ŕ��������Ƃ��ɔ�������C�x���g�ł��B
    ' StartupNextInstance: �P��C���X�^���X �A�v���P�[�V�������N������A���ꂪ���ɃA�N�e�B�u�ł���Ƃ��ɔ������܂��B
    ' NetworkAvailabilityChanged: �l�b�g���[�N�ڑ����ڑ����ꂽ�Ƃ��A�܂��͐ؒf���ꂽ�Ƃ��ɔ������܂��B
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            Dim sWorkingDir As String = System.Environment.CurrentDirectory
            Dim sLogBasePath As String = Path.Combine(sWorkingDir, "LOG")
            JR.ExOpmg.Common.Log.Init(sLogBasePath, "SampleDsClientApp")
            JR.ExOpmg.Common.Log.Info("�v���Z�X�J�n")

            Dim sIniFilePath As String = Path.ChangeExtension(System.Windows.Forms.Application.ExecutablePath, ".ini")
            sIniFilePath = Path.Combine(sWorkingDir, Path.GetFileName(sIniFilePath))

            Try
                Lexis.Init(sIniFilePath)
                Config.Init(sIniFilePath)
            Catch ex As Exception
                JR.ExOpmg.Common.Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                e.Cancel = True
                Return
            End Try

            JR.ExOpmg.Common.Log.SetKindsMask(Config.LogKindsMask)

            LocalConnectionProvider.Init()

            OpClientUtil.StartTelegrapher()
        End Sub

        Private Sub MyApplication_Shutdown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shutdown
            OpClientUtil.QuitTelegrapher()

            LocalConnectionProvider.Dispose()

            JR.ExOpmg.Common.Log.Info("�v���Z�X�I��")
        End Sub

    End Class

End Namespace
