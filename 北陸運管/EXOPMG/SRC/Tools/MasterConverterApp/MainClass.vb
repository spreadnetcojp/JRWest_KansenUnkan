' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/03/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text.RegularExpressions
Imports JR.ExOpmg.Common

''' <summary>
''' �w���@��}�X�^�ϊ��o�̓c�[���̃��C����������������N���X�B
''' </summary>
Public Class MainClass

    ''' <summary>
    ''' �A�v���P�[�V�����p�f�[�^�f�B���N�g��������Windows�W���̊��ϐ���
    ''' </summary>
    Private Const REG_LOCALAPPDATA As String = "LOCALAPPDATA"

    ''' <summary>
    ''' ���O�t�@�C����
    ''' </summary>
    Private Shared ReadOnly oLogFileNameRegx As New Regex("^[0-9]{8}-MasterConverterApp[0-9]+-[0-9A-Z_\-]+\.csv$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const sLogFileNamePattern As String = "????????-MasterConverterApp*.csv"

    ''' <summary>
    ''' �w���@��}�X�^�ϊ��o�̓c�[���̃��C�������B
    ''' </summary>
    <STAThread()> _
    Public Shared Sub Main()
        Try
            Dim sLocalAppDataPath As String = Constant.GetEnv(REG_LOCALAPPDATA)
            If sLocalAppDataPath Is Nothing Then
                AlertBox.Show(Lexis.EnvVarNotFound, REG_LOCALAPPDATA)
                Return
            End If
            Dim sLogBasePath As String = Path.Combine(sLocalAppDataPath, "ExOpmg\MasterConverterApp\Log")
            Using curProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess()
                Log.Init(sLogBasePath, "MasterConverterApp" & curProcess.Id.ToString())
            End Using
            Log.Info("�v���Z�X�J�n")

            Dim sWorkingDir As String = System.Environment.CurrentDirectory()
            Dim sIniFilePath As String = Path.ChangeExtension(Application.ExecutablePath, ".ini")
            sIniFilePath = Path.Combine(sWorkingDir, Path.GetFileName(sIniFilePath))
            Try
                Lexis.Init(sIniFilePath)
                Config.Init(sIniFilePath)
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                Return
            End Try

            Log.SetKindsMask(Config.LogKindsMask)
            SweepLogs(sLogBasePath)

            Dim oForm As New MainForm()

            '��ʕ\���iUI�p���b�Z�[�W���[�v���s�j
            Log.Info("��ʕ\�������J�n")
            oForm.ShowDialog()
            Log.Info("��ʕ\�������I��")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            Log.Info("�v���Z�X�I��")
            Application.Exit()
        End Try
    End Sub

    Private Shared Sub SweepLogs(ByVal sLogBasePath As String)
        Try
            'Config.LogsKeepingDays���o�߂������샍�O��
            'sLogBasePath�̃f�B���N�g������폜����B
            Log.Info("Sweeping logs...")

            Dim boundDate As Integer = Integer.Parse(DateTime.Now.AddDays(-Config.LogsKeepingDays).ToString("yyyyMMdd"))
            For Each sFile As String In Directory.GetFiles(sLogBasePath, sLogFileNamePattern)
                Dim sFileName As String = Path.GetFileName(sFile)
                If oLogFileNameRegx.IsMatch(sFileName) AndAlso _
                   Integer.Parse(sFileName.Substring(0, 8)) < boundDate Then
                    File.Delete(sFile)
                    Log.Info("The file [" & sFile & "] deleted.")
                End If
            Next sFile

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SweepLogsFailed)
        End Try
    End Sub

End Class
