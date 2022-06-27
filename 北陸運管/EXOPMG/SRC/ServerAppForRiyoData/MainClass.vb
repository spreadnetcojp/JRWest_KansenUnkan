' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/04/10  (NES)小林  次世代車補対応にて新規作成
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
''' 利用データ登録プロセス共通のメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "メソッド"
    ''' <summary>
    '''  利用データ登録プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    '''  利用データ登録プロセスのエントリポイントである。
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
                Log.Info("プロセス開始")

                Try
                    Lexis.Init(sIniFilePath)
                    Config.Init(sIniFilePath)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End Try

                Log.SetKindsMask(Config.LogKindsMask)

                'メッセージループがアイドル状態になる前（かつ、生存証明ファイルの更新で
                '競合することになる監視スレッドを起動する前）に、生存証明ファイルを
                '更新しておく。
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '監視スレッドを開始する。
                Dim oWatcher As New MyWatcher(oMainForm)
                Log.Info("Starting the watcher thread...")
                oWatcher.Start()

                'ウインドウプロシージャを実行する。
                'NOTE: このメソッドから例外がスローされることはない。
                ServerAppBaseMain(oMainForm)

                Try
                    '監視スレッドに終了を要求する。
                    Log.Info("Sending quit request to the watcher thread...")
                    oWatcher.Quit()

                    'NOTE: 以下で監視スレッドが終了しない場合、
                    '監視スレッドは生存証明を行わないはずであり、
                    '状況への対処はプロセスマネージャで行われる想定である。

                    '監視スレッドの終了を待つ。
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
                Log.Info("プロセス終了")

                'NOTE: ここを通らなくても、このスレッドの消滅とともに解放される
                'ようなので、最悪の心配はない。
                m.ReleaseMutex()

                Application.Exit()
            End Try
        End If
    End Sub
#End Region

End Class
