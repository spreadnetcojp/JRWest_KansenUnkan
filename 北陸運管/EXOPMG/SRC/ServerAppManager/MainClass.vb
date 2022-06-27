' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/06/07  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Messaging
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' プロセスマネージャのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "定数や変数"
    'メインウィンドウ
    Private Shared oMainForm As ServerAppForm

    '実作業スレッドへの終了要求フラグ
    Private Shared quitWorker As Integer
#End Region

    ''' <summary>
    ''' プロセスマネージャのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' プロセスマネージャのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppManager")
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

                Log.Init(sLogBasePath, "Manager")
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

                    'NOTE: デバッグ時などでない限りは実作業スレッドが
                    '必ず終了することを前提にしている。
                    'TODO: 運用上、終了は夜間の無人状態で行われる故、
                    '上記の前提を設けるのはNGかもしれない。
                    'そうであるなら、Joinに期限を設けて、タイムアウト時に
                    'SNMP TRAPやメールでユーザに通知することを検討
                    'しなければならない。

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
    ''' 各種常駐プロセスの起動・監視・終了を行う。
    ''' </remarks>
    Private Shared Sub WorkingLoop()
        Dim aProcesses(Config.ResidentApps.Length - 1) As System.Diagnostics.Process

        Try
            Log.Info("The worker thread started.")

            '各プロセスのメッセージキューを作成する。
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                If Config.MqPathForApps.ContainsKey(Config.ResidentApps(i)) Then
                    Dim sMqPath As String = Config.MqPathForApps(Config.ResidentApps(i))
                    If Not MessageQueue.Exists(sMqPath) Then
                        Log.Info("Registering [" & Config.MqPathForApps(Config.ResidentApps(i)) & "]...")
                        MessageQueue.Create(sMqPath)
                    End If
                End If
            Next

            '各プロセスを起動させる。
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                aProcesses(i) = New System.Diagnostics.Process()
                aProcesses(i).StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, GetFileNameOfProcess(i))
                aProcesses(i).StartInfo.UseShellExecute = False
                Log.Info("Starting [" & aProcesses(i).StartInfo.FileName & "]...")
                aProcesses(i).Start()

                'NOTE: プロセスの起動直後に異常が発生して、メッセージボックスが
                '表示された（ボタン押下待ちになった）場合も、下記メソッドから
                '復帰してしまうはずである。そのケースでは、生存証明が行われて
                'いないはずであるため、すぐに死活チェックを行うと、ユーザが
                'メッセージボックスの内容を確認する前に、プロセスをKillする
                'ことになってしまう。その意味でも、死活チェックの周期を
                '短くしすぎることはNGである。
                aProcesses(i).WaitForInputIdle()
            Next

            Dim oStatusPollTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            oStatusPollTimer.Start(TickTimer.GetSystemTick())

            While Thread.VolatileRead(quitWorker) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oStatusPollTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oStatusPollTimer.Start(systemTick)

                    For i As Integer = 0 To Config.ResidentApps.Length - 1
                        '終了しているプロセスを再起動させる。
                        If aProcesses(i).HasExited Then
                            Log.Fatal("[" & GetFileNameOfProcess(i) & "] is aborted.")

                            '収集データ誤記テーブルに異常を登録する。
                            CollectedDataTypoRecorder.Record( _
                               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                               DbConstants.CdtKindServerError, _
                               Lexis.CdtProcessAbended.Gen(GetFileNameOfProcess(i)))

                            aProcesses(i).Close()
                            aProcesses(i) = New System.Diagnostics.Process()
                            aProcesses(i).StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, GetFileNameOfProcess(i))
                            aProcesses(i).StartInfo.UseShellExecute = False
                            Log.Info("Starting [" & aProcesses(i).StartInfo.FileName & "]...")
                            aProcesses(i).Start()
                            aProcesses(i).WaitForInputIdle()
                        End If
                    Next

                    '各プロセスが生存しているかチェックを行う。
                    'NOTE: これがないと、個々のプロセスに関して「想定しない
                    '例外が発生した際や、管理系スレッドで想定しない異常を
                    '認識した際は、必ずプロセス全体の終了に漕ぎ付けなければ
                    'ならない」「フォアグラウンドスレッドの終了は絶対に
                    '行われるように作り込む」などの前提が必要になる。
                    '個々のプロセスをそのように作り込むのは理想であるが、
                    '万が一の事態を考えると、ここで保険をかければ、
                    '運用的に安全になるし、個々のプロセスを作り込む上での
                    '不安要素も少なくなる。
                    For i As Integer = 0 To Config.ResidentApps.Length - 1
                        Dim sFilePath As String = Path.Combine(Config.ResidentAppPulseDirPath, Config.ResidentApps(i))
                        Dim lastWriteTime As DateTime = File.GetLastWriteTime(sFilePath)
                        If lastWriteTime + New TimeSpan(CLng(Config.ResidentAppPendingLimitTicks) * 10000) < DateTime.Now Then
                            Log.Fatal("[" & GetFileNameOfProcess(i) & "] seems broken.")

                            '収集データ誤記テーブルに異常を登録する。
                            CollectedDataTypoRecorder.Record( _
                               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                               DbConstants.CdtKindServerError, _
                               Lexis.CdtProcessAbended.Gen(GetFileNameOfProcess(i)))

                            Try
                                aProcesses(i).Kill()
                            Catch ex As Exception
                                Log.Error("Exception caught.", ex)
                            End Try

                            aProcesses(i).Close()
                            aProcesses(i) = New System.Diagnostics.Process()
                            aProcesses(i).StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, GetFileNameOfProcess(i))
                            aProcesses(i).StartInfo.UseShellExecute = False
                            Log.Info("Starting [" & aProcesses(i).StartInfo.FileName & "]...")
                            aProcesses(i).Start()
                            aProcesses(i).WaitForInputIdle()
                        End If
                    Next

                End If
                Thread.Sleep(Config.PollIntervalTicks)
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'TODO: TRAP発生やメール通知など、何とかして
            'ユーザに気付いてもらう必要がある。

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))

        Finally
            '各プロセスを終了させる。
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                If aProcesses(i) IsNot Nothing Then
                    Try
                        Log.Info("Sending quit request to [" & GetFileNameOfProcess(i) & "]...")
                        aProcesses(i).CloseMainWindow()
                    Catch ex As Exception
                        'NOTE: このケースの想定には、aProcesses(i)のStartで
                        '失敗した場合だけでなく、Start成功後にaProcesses(i)
                        '自らが終了した場合も含まれる。
                        '後者の場合は、Fatalなログが出力されていないはずで
                        'あるため、ここで出力するログはFatalとする。
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                End If
            Next

            '各プロセスの終了を待つ。
            Dim oJoinLimitTimer As New TickTimer(Config.ResidentAppPendingLimitTicks)
            oJoinLimitTimer.Start(TickTimer.GetSystemTick())
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                If aProcesses(i) IsNot Nothing Then
                    Try
                        Dim ticks As Long = oJoinLimitTimer.GetTicksToTimeout(TickTimer.GetSystemTick())
                        If ticks < 0 Then ticks = 0

                        Log.Info("Waiting for [" & GetFileNameOfProcess(i) & "] to quit...")
                        If aProcesses(i).WaitForExit(CInt(ticks)) = False Then
                            Log.Fatal("[" & GetFileNameOfProcess(i) & "] seems broken.")

                            '収集データ誤記テーブルに異常を登録する。
                            CollectedDataTypoRecorder.Record( _
                               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                               DbConstants.CdtKindServerError, _
                               Lexis.CdtProcessAbended.Gen(GetFileNameOfProcess(i)))

                            Try
                                aProcesses(i).Kill()
                            Catch ex As Exception
                                Log.Error("Exception caught.", ex)
                            End Try
                        Else
                            Log.Info("[" & GetFileNameOfProcess(i) & "] has quit.")
                        End If
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                End If
            Next

            '各プロセスのハンドルを解放する。
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                If aProcesses(i) IsNot Nothing Then
                    Try
                        aProcesses(i).Close()
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                End If
            Next
        End Try
    End Sub

    Private Shared Function GetFileNameOfProcess(ByVal i As Integer) As String
        Return "ExOpmgServerApp" & Config.ResidentApps(i) & ".exe"
    End Function

End Class
