' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2014/04/20  (NES)      新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading
Imports JR.ExOpmg.Common

''' <summary>
''' 保守ツールのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass

    ''' <summary>
    ''' アプリケーション用データディレクトリを示すWindows標準の環境変数名
    ''' </summary>
    Private Const REG_LOCALAPPDATA As String = "LOCALAPPDATA"

    ''' <summary>
    ''' ログファイル名
    ''' </summary>
    Private Shared ReadOnly oLogFileNameRegx As New Regex("^[0-9]{8}-MaintenanceToolApp[0-9]+-[0-9A-Z_\-]+\.csv$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const sLogFileNamePattern As String = "????????-MaintenanceToolApp*.csv"

    ''' <summary>
    ''' 駅務機器マスタ変換出力ツールのメイン処理。
    ''' </summary>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgMaintenanceToolApp")
        If m.WaitOne(0, False) Then
            Dim oToolMenu As FrmMaintenanceToolMenu = Nothing
            Try
                Dim sLocalAppDataPath As String = Constant.GetEnv(REG_LOCALAPPDATA)
                If sLocalAppDataPath Is Nothing Then

                    AlertBox.Show(Lexis.EnvVarNotFound, REG_LOCALAPPDATA)
                    Return
                End If
                Dim sLogBasePath As String = Path.Combine(sLocalAppDataPath, "ExOpmg\MaintenanceToolApp\Log")
                Using curProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess()
                    Log.Init(sLogBasePath, "MaintenanceToolApp" & curProcess.Id.ToString())
                End Using
                Log.Info("プロセス開始")

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

                oToolMenu = New FrmMaintenanceToolMenu()

                '画面表示（UI用メッセージループ実行）
                Log.Info("画面表示処理開始")
                oToolMenu.ShowDialog()
                Log.Info("画面表示処理終了")

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                If oToolMenu IsNot Nothing Then
                    oToolMenu.Dispose()
                End If
                Log.Info("プロセス終了")
                m.ReleaseMutex()
                Application.Exit()
            End Try
        Else
            AlertBox.Show(Lexis.DoNotExecMultipleInstance)
        End If
    End Sub

    Private Shared Sub SweepLogs(ByVal sLogBasePath As String)
        Try
            'Config.LogsKeepingDaysが経過した操作ログを
            'sLogBasePathのディレクトリから削除する。
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
