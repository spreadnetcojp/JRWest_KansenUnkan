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

        ''' <summary>
        ''' [���O�t�@�C���o�͐�f�B���N�g���w��p���ϐ���]
        ''' </summary>
        Private Const REG_LOG As String = "EXOPMG_LOG_DIR"

        ''' <summary>
        ''' [�[���pINI�t�@�C���w��p���ϐ���]
        ''' </summary>
        Private Const REG_CLIENT_INI As String = "EXOPMG_INIFILE_CLIENT"

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
            If sLogBasePath Is Nothing Then
                AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
                e.Cancel = True
            End If

            Dim sIniFilePath As String = Constant.GetEnv(REG_CLIENT_INI)
            If sIniFilePath Is Nothing Then
                AlertBox.Show(Lexis.EnvVarNotFound, REG_CLIENT_INI)
                e.Cancel = True
            End If

            JR.ExOpmg.Common.Log.Init(sLogBasePath, "ClientApp")
            JR.ExOpmg.Common.Log.Info("�v���Z�X�J�n")

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

            SweepUtil.SweepOperationLogs(sLogBasePath)
            SweepUtil.SweepLogs(sLogBasePath)

            LocalConnectionProvider.Init()

            'NOTE: ���O�ɏo�͂���郆�[�UID�̂��Ƃ��l������ƁA
            '���[�U�����O�C�������Ƃ��ɍs���������R��������Ȃ��B
            OpClientUtil.StartTelegrapher()
        End Sub

        Private Sub MyApplication_Shutdown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shutdown
            'NOTE: ���O�ɏo�͂���郆�[�UID�̂��Ƃ��l������ƁA
            '���[�U�����O�A�E�g�����Ƃ��ɍs���������R��������Ȃ��B
            OpClientUtil.QuitTelegrapher()

            LocalConnectionProvider.Dispose()

            JR.ExOpmg.Common.Log.Info("�v���Z�X�I��")
        End Sub

    End Class

End Namespace
