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
Imports System.Messaging
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' 警報メール送信プロセスのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "定数や変数"
    'メインウィンドウ
    Private Shared oMainForm As ServerAppForm

    '実作業スレッドへの終了要求フラグ
    Private Shared quitWorker As Integer

    'メッセージキュー
    Private Shared oMessageQueue As MessageQueue

    'SMTPクライアント
    Private Shared oSmtpClient As SmtpClient
#End Region

    ''' <summary>
    ''' 警報メール送信プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 警報メール送信プロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppAlertMailer")
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

                Log.Init(sLogBasePath, "AlertMailer")
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

                If Not Config.MailSmtpServerName.Equals("") Then
                    oSmtpClient = New SmtpClient()
                    oSmtpClient.Host = Config.MailSmtpServerName
                    oSmtpClient.Port = Config.MailSmtpPort
                    oSmtpClient.Credentials = New NetworkCredential(Config.MailSmtpUserName, Config.MailSmtpPassword)
                    oSmtpClient.Timeout = Config.MailSendLimitTicks
                End If

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

                'NOTE: .NET Framework 4.0 以降では、以下を追加する。
                'なお、.NET Framework 4.5 以降では、Subject の
                'エンコード方法にも手を加えなければならないので、注意。
                'If oSmtpClient IsNot Nothing Then
                '    oSmtpClient.Dispose()
                'End If

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
    ''' 警報メール送信要求（MSMQメッセージ）の受信を待ち続ける。
    ''' 警報メール送信要求を受信した際は、SMTPでメールサーバに送信する。
    ''' </remarks>
    Private Shared Sub WorkingLoop()
        Try
            Log.Info("The worker thread started.")

            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            Dim fewSpan As New TimeSpan(0, 0, 0, 0, Config.PollIntervalTicks)
            Dim oFilter As New MessagePropertyFilter()
            oFilter.ClearAll()
            oFilter.AppSpecific = True
            oFilter.Body = True

            'NOTE: Config.MyMqPathのメッセージキューは、
            'この時点で必ず存在している前提である。存在していなければ、
            'システムに異常がある故、このプロセスは起動直後に終了するべき
            'である。
            oMessageQueue = New MessageQueue(Config.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([ExtAlertMailSendRequestBody])})

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())
            While Thread.VolatileRead(quitWorker) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()
                End If

                Dim oMessage As Message = Nothing
                Try
                    '所定時間メッセージを待つ。
                    'NOTE: MessageQueue.Receive()のタイムアウトは、実際に
                    '引数の時間が経過した際ではなく、呼び出し時点の
                    'システム時刻に引数の時間を加えた時刻Tを求めた上で、
                    'システム時刻がT以上になった際に行われるようである。
                    'つまり、呼び出しの間にシステム時刻が1時間戻されれば、
                    '呼び出しから戻るのは、「引数の時間+1時間」経過後に
                    'なってしまい、その間は、子スレッドの生存監視や
                    '親プロセスへの生存証明を行うことができなくなる。
                    'メインスレッドからの終了要求にも反応できなくなる。
                    'しかし、そのことが問題になるような大きな時刻補正が
                    '行われることはないだろうし、ここでTimeSpan.Zeroを渡して、
                    '別の方法でCPUの解放期間を作るするようにすれば、
                    'メッセージ受信に対する反応が悪くなる（メッセージ処理
                    '性能が低下する）はずであるため、以下のとおり、
                    'MessageQueue.Receive()で待つことにしている。
                    oMessage = oMessageQueue.Receive(fewSpan)
                Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                    'メッセージ受信待ちに戻る。
                    Continue While
                End Try

                If oMessage.AppSpecific <> ExtAlertMailSendRequest.FormalKind Then
                    Log.Error("Unwelcome ExtMessage received.")
                    'メッセージ受信待ちに戻る。
                    Continue While
                End If

                Dim sMailTitle As String = Nothing
                Dim sMailBody As String = Nothing
                Try
                    Dim oMailSendRequest As New ExtAlertMailSendRequest(oMessage)
                    sMailTitle = oMailSendRequest.MailTitle
                    sMailBody = oMailSendRequest.MailBody
                Catch ex As Exception
                    Log.Error("Exception caught on parsing ExtMessage.", ex)
                    'メッセージ受信待ちに戻る。
                    Continue While
                End Try

                Log.Info("ExtAlertMailSendRequest received." & vbCrLf & sMailTitle & vbCrLf & sMailBody)

                '送信を試みる。通信異常を検出した場合は、他の既着メッセージも全て読み捨てる。
                If oSmtpClient IsNot Nothing AndAlso TrySendAlertMail(sMailTitle, sMailBody) = -2 AndAlso Config.MailSendFailureSpreads Then
                    While Thread.VolatileRead(quitWorker) = 0
                        Try
                            oMessage = oMessageQueue.Receive(TimeSpan.Zero)
                        Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                            'メッセージ受信待ちに戻る。
                            Exit While
                        End Try

                        If oMessage.AppSpecific <> ExtAlertMailSendRequest.FormalKind Then
                            Log.Error("Unwelcome ExtMessage (AppSpecific: " & oMessage.AppSpecific.ToString() & ") received.")
                            '既着メッセージ取出しに戻る。
                            Continue While
                        End If

                        Try
                            Dim oMailSendRequest As New ExtAlertMailSendRequest(oMessage)
                            sMailTitle = oMailSendRequest.MailTitle
                            sMailBody = oMailSendRequest.MailBody
                        Catch ex As Exception
                            Log.Error("Exception caught on parsing ExtMessage.", ex)
                            '既着メッセージ取出しに戻る。
                            Continue While
                        End Try

                        Log.Info("ExtAlertMailSendRequest received." & vbCrLf & sMailTitle & vbCrLf & sMailBody)
                        Log.Warn("要求を捨てました。")
                    End While
                End If
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP発生（または収集データ誤記テーブルへの登録）は、
            'プロセスマネージャが行うので、ここでは不要である。

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        Finally
            If oMessageQueue IsNot Nothing Then
                oMessageQueue.Close()
            End If
        End Try
    End Sub

    Private Shared Function TrySendAlertMail(ByVal sSubject As String, ByVal sBody As String) As Integer
        Using oMail As New MailMessage()
            Try
                'メールヘッダのFROMや宛先を編集。
                oMail.From = New MailAddress(Config.MailFromAddr)
                For i As Integer = 0 To Config.MailToAddrs.Length - 1
                    If Not String.IsNullOrEmpty(Config.MailToAddrs(i)) Then
                        oMail.To.Add(Config.MailToAddrs(i))
                    End If
                Next
                For i As Integer = 0 To Config.MailCcAddrs.Length - 1
                    If Not String.IsNullOrEmpty(Config.MailCcAddrs(i)) Then
                        oMail.CC.Add(Config.MailCcAddrs(i))
                    End If
                Next
                For i As Integer = 0 To Config.MailBccAddrs.Length - 1
                    If Not String.IsNullOrEmpty(Config.MailBccAddrs(i)) Then
                        oMail.Bcc.Add(Config.MailBccAddrs(i))
                    End If
                Next

                'メールの件名を編集。
                Dim oSubjectEncoding As Encoding = Encoding.GetEncoding(Config.MailSubjectEncoding)
                oMail.Subject = String.Format( _
                   "=?{0}?B?{1}?=", _
                   oSubjectEncoding.BodyName, _
                   Convert.ToBase64String(oSubjectEncoding.GetBytes(sSubject), Base64FormattingOptions.None))

                'メール本文を編集。
                Dim oAltView As AlternateView = _
                   AlternateView.CreateAlternateViewFromString( _
                      sBody, _
                      Encoding.GetEncoding(Config.MailBodyEncoding), _
                      MediaTypeNames.Text.Plain)
                oAltView.TransferEncoding = Config.MailTransferEncoding
                oMail.AlternateViews.Add(oAltView)

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                Log.Fatal("件名または本文の異常で送信ができませんでした。")
                Return -4
            End Try

            Try
                'メールを送信。
                oSmtpClient.Send(oMail)

            Catch ex As SmtpFailedRecipientsException
                Log.Debug("Exception caught.", ex)
                Log.Error("一部の宛先(" & ex.FailedRecipient & ")への送信ができませんでした。")
                Return -1
            Catch ex As SmtpException
                Log.Error("Exception caught.", ex)
                Log.Error("通信異常により送信ができませんでした。")
                Return -2
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                Log.Fatal("予期せぬ異常により送信ができませんでした。")
                Return -3
            End Try
        End Using

        Log.Info("送信しました。")
        Return 0
    End Function

End Class
