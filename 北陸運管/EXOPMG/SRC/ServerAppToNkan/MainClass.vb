' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' 対Ｎ間通信プロセスのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "定数や変数"
    'メインウィンドウ
    Protected Friend Shared oMainForm As ServerAppForm
#End Region

#Region "メソッド"
    ''' <summary>
    ''' 対Ｎ間通信プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 対Ｎ間通信プロセスのエントリポイントである。
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

                LocalConnectionProvider.Init()

                'メッセージループがアイドル状態になる前（かつ、定期的にそれを行う
                'スレッドを起動する前）に、生存証明ファイルを更新しておく。
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '通信管理スレッドを開始する。
                Dim oListener As New MyListener()
                Log.Info("Starting the listener thread...")
                oListener.Start()

                'ウインドウプロシージャを実行する。
                'NOTE: このメソッドから例外がスローされることはない。
                ServerAppBaseMain(oMainForm)

                Try
                    '通信管理スレッドに終了を要求する。
                    Log.Info("Sending quit request to the listener thread...")
                    oListener.Quit()

                    'NOTE: 以下で通信管理スレッドが終了しない場合、
                    '通信管理スレッドは生存証明を行わないはずであり、
                    '状況への対処はプロセスマネージャで行われる想定である。

                    '通信管理スレッドの終了を待つ。
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
