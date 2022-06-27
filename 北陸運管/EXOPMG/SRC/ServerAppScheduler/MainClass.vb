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
Imports System.Messaging
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' スケジューラプロセスのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "内部クラス等"
    Private Class ScheduledEvent
        'タイトル
        Public Title As String

        '設定情報
        Public Config As ScheduledEventConfig

        '最終実施日時（正規化済み）
        Public LastExecTime As DateTime
    End Class
#End Region

#Region "定数や変数"
    '各イベントの情報
    Private Shared oScheduledEvents As List(Of ScheduledEvent)

    'メインウィンドウ
    Private Shared oMainForm As ServerAppForm

    '実作業スレッドへの終了要求フラグ
    Private Shared quitWorker As Integer
#End Region

    ''' <summary>
    ''' スケジューラプロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' スケジューラプロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppScheduler")
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

                Log.Init(sLogBasePath, "Scheduler")
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

                Try
                    oScheduledEvents = New List(Of ScheduledEvent)
                    Dim now As DateTime = DateTime.Now
                    For Each oEventConfig As KeyValuePair(Of String, ScheduledEventConfig) In Config.ScheduledEvents
                        Dim oEvent As New ScheduledEvent()
                        oEvent.Title = oEventConfig.Key
                        oEvent.Config = oEventConfig.Value
                        oEvent.LastExecTime = oEventConfig.Value.Normalize(now)
                        oScheduledEvents.Add(oEvent)
                    Next oEventConfig
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnInitializingProcess)
                    Return
                End Try

                'メッセージループがアイドル状態になる前（かつ、定期的にそれを行う
                'スレッドを起動する前）に、生存証明ファイルを更新しておく。
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '実作業スレッドを開始する。
                Dim oWorkerThread As New Thread(AddressOf MainClass.WorkingLoop)
                Log.Info("Starting the worker thread...")
                quitWorker = 0
                oWorkerThread.Name = "Worker"
                oWorkerThread.Start()

                'ウインドウプロシージャを実行する。
                'NOTE: このメソッドから例外がスローされることはない。
                ServerAppBaseMain(oMainForm)

                Try
                    '実作業スレッドに終了を要求する。
                    Log.Info("Sending quit request to the worker thread...")
                    Thread.VolatileWrite(quitWorker, 1)

                    'NOTE: 以下で実作業スレッドが終了しない場合、
                    '実作業スレッドは生存証明を行わないはずであり、
                    '状況への対処はプロセスマネージャで行われる想定である。

                    '実作業スレッドの終了を待つ。
                    Log.Info("Waiting for the worker thread to quit...")
                    oWorkerThread.Join()
                    Log.Info("The worker thread has quit.")
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oWorkerThread.Abort()
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

    ''' <summary>
    ''' 実作業スレッドのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 時刻の監視とメッセージの送信を行う。
    ''' </remarks>
    Private Shared Sub WorkingLoop()
        Try
            Log.Info("The worker thread started.")

            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Thread.VolatileRead(quitWorker) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()
                End If

                Dim now As DateTime = DateTime.Now
                For Each oEvent As ScheduledEvent In oScheduledEvents
                    Dim normNow As DateTime = oEvent.Config.Normalize(now)
                    If normNow > oEvent.LastExecTime Then
                        'StartMinutesInDay以上になるように補正した
                        '現在時刻を（0時0分からの経過分の形式で）求める。
                        Dim nowMinutesInDay As Integer = normNow.Hour * 60 + normNow.Minute
                        If oEvent.Config.StartMinutesInDay > nowMinutesInDay Then
                            nowMinutesInDay += 24 * 60
                        End If

                        '有効時間帯のみ送信を行う。
                        If nowMinutesInDay <= oEvent.Config.EndMinutesInDay Then
                            Log.Info("It's now time to " & oEvent.Title & ".")

                            Dim oMessage As New Message()
                            oMessage.AppSpecific = oEvent.Config.MessageKind
                            oMessage.Body = oEvent.Config.MessageBody
                            For Each oTargetApp As String In oEvent.Config.TargetApps
                                Config.MessageQueueForApps(oTargetApp).Send(oMessage)
                            Next oTargetApp
                        End If

                        oEvent.LastExecTime = normNow
                    ElseIf normNow < oEvent.LastExecTime Then
                        'システム日時が2周期以上戻された場合は、
                        '最終実施日時を正規化したシステム日時に合わせる。
                        Dim span As TimeSpan = oEvent.LastExecTime - normNow
                        Dim cycles As Integer = span.Minutes \ oEvent.Config.Cycle
                        If cycles > 1 Then
                            Log.Warn("The system time goes back into the past.")
                            oEvent.LastExecTime = normNow
                        End If
                    End If
                Next oEvent

                Thread.Sleep(Config.PollIntervalTicks)
            End While
            Log.Info("Quit requested by manager.")
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP発生（または収集データ誤記テーブルへの登録）は、
            'プロセスマネージャが行うので、ここでは不要である。

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        End Try
    End Sub


End Class
