' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/05/13  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO

Imports JR.ExOpmg.Common

Namespace My

    ' 次のイベントは MyApplication に対して利用できます:
    '
    ' Startup: アプリケーションが開始されたとき、スタートアップ フォームが作成される前に発生します。
    ' Shutdown: アプリケーション フォームがすべて閉じられた後に発生します。このイベントは、通常の終了以外の方法でアプリケーションが終了されたときには発生しません。
    ' UnhandledException: ハンドルされていない例外がアプリケーションで発生したときに発生するイベントです。
    ' StartupNextInstance: 単一インスタンス アプリケーションが起動され、それが既にアクティブであるときに発生します。
    ' NetworkAvailabilityChanged: ネットワーク接続が接続されたとき、または切断されたときに発生します。
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            Dim sWorkingDir As String = System.Environment.CurrentDirectory
            Dim sLogBasePath As String = Path.Combine(sWorkingDir, "LOG")
            JR.ExOpmg.Common.Log.Init(sLogBasePath, "SampleDsClientApp")
            JR.ExOpmg.Common.Log.Info("プロセス開始")

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

            JR.ExOpmg.Common.Log.Info("プロセス終了")
        End Sub

    End Class

End Namespace
