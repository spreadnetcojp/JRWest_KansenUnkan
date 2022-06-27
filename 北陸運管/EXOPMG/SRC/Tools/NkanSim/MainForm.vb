' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2014/10/22  (NES)小林  EkimuSimに合わせてレイアウトを微調整
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

Public Class MainForm
    Protected OptionalWriter As LogToOptionalDelegate

    'NOTE: UiStateのメンバは電文送受信スレッドでも参照可能とする。
    'その際は、SyncLock UiStateした状態でディープコピーを行うこと。
    'また、SyncLock UiStateしている間、ログ出力などメインスレッドを
    '待つことになり得る処理は行ってはならない。
    'NOTE: メインスレッドは、該当するコントロールの状態が変化した際
    'などにおいて、SyncLock UiStateした状態でここに値を設定する。
    'その間、oChildSteerSockへの書き込みやoChildSteerSockからの
    '受信待ちなど、電文送受信スレッドを待つことになり得る処理は
    '行ってはならない。
    Public UiState As UiStateClass

    Protected oChildSteerSock As Socket
    Protected oTelegrapher As MyTelegrapher

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)

        OptionalWriter = New LogToOptionalDelegate(AddressOf Me.FetchLog)
    End Sub

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)

        Log.SetOptionalWriter(New LogToOptionalDelegate(AddressOf Me.BeginFetchLog))

        Dim sWorkingDir As String = System.Environment.CurrentDirectory
        Dim sIniFilePath As String = Path.ChangeExtension(Application.ExecutablePath, ".ini")
        sIniFilePath = Path.Combine(sWorkingDir, Path.GetFileName(sIniFilePath))
        Try
            Lexis.Init(sIniFilePath)
            Config.Init(sIniFilePath)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
            Me.Close()
            Return
        End Try

        Log.SetKindsMask(Config.LogKindsMask)

        LocalConnectionProvider.Init()

        UiState = New UiStateClass()

        Me.SuspendLayout() '---------------------------------------------------

        'Lexis から生成した文言を各コントロールに反映する。
        Me.Text = Lexis.FormTitle.Gen(Config.TargetEkCode.ToString(Lexis.FormTitleEkCodeFormat.Gen()))

        'UiStateの値を各コントロールに反映する。
        AutomaticComStartCheckBox.Checked = UiState.AutomaticComStart
        CapSndTelegsCheckBox.Checked = UiState.CapSndTelegs
        CapRcvTelegsCheckBox.Checked = UiState.CapRcvTelegs
        CapSndFilesCheckBox.Checked = UiState.CapSndFiles
        CapRcvFilesCheckBox.Checked = UiState.CapRcvFiles

        'TODO: 既に存在するUiStateClassのメンバについて、
        'コントロールを用意したら、ここも忘れずに実装すること。

        Me.ResumeLayout() '----------------------------------------------------

        Dim sCapDirPath As String = Path.Combine(sWorkingDir, "CAP")

        '送受信履歴ディレクトリについて、無ければ作成しておく。
        Directory.CreateDirectory(sCapDirPath)

        '電文送受信スレッドを作成する。
        Dim oMessageSockForTelegrapher As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oChildSteerSock, oMessageSockForTelegrapher)
        oTelegrapher = New MyTelegrapher("ToOpmg", oMessageSockForTelegrapher, sCapDirPath, Me)

        '電文送受信スレッドを開始する。
        Log.Info("Starting the telegrapher...")
        oTelegrapher.Start()

        LineStatusPollTimer.Start()
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        LineStatusPollTimer.Stop()

        If oChildSteerSock IsNot Nothing Then
            If oTelegrapher IsNot Nothing Then
                '電文送受信スレッドに終了要求を送信する。
                Log.Info("Sending quit request to the telegrapher...")
                If QuitRequest.Gen().WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
                    Log.Fatal("The telegrapher seems broken.")
                End If

                '電文送受信スレッドの終了を待つ。
                Log.Info("Waiting for the telegrapher to quit...")
                If oTelegrapher.Join(Config.TelegrapherPendingLimitTicks) = False Then
                    Log.Fatal("The telegrapher seems broken.")
                    oTelegrapher.Abort()
                End If
            End If
            oChildSteerSock.Close()
        End If

        LocalConnectionProvider.Dispose()

        Log.SetOptionalWriter(Nothing)

        MyBase.OnFormClosed(e)
    End Sub

    Protected Friend Sub RethrowException(ByVal ex As Exception)
        'TODO: これにより発生するイベントのためのハンドラを用意する。
        Application.OnThreadException(ex)
    End Sub

    'NOTE: ログ出力毎に呼ばれるので、これの中でログを出力してはならない。
    Protected Sub BeginFetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)
        Try
            'OPT: 上記が守られる限りはデッドロックもないと思われるので、
            'BeginInvoke()ではなく、Invoke()でもよいかもしれない。
            'Invoke()ならば、メッセージキューがあふれる心配もない。
            BeginInvoke( _
                OptionalWriter, _
                New Object() {number, sSecondName, sDateTime, sKind, sClassName, sMethodName, sText})
        Catch ex As Exception
            'NOTE: このControlが破棄された後にこのメソッドが呼び出される万が一の場合を想定している。
            'この後の（このデリゲートに依存しない）処理を通常通り行うよう、例外は握りつぶす。
        End Try
    End Sub

    'NOTE: ログ出力毎に呼ばれるので、これの中でログを出力してはならない。
    Protected Sub FetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)

        LoggerTextBox.AppendText("[" & sDateTime & "] [" & sSecondName & "] " & sText & vbCrLf)
    End Sub

    Private Sub ConButton_Click(sender As System.Object, e As System.EventArgs) Handles ConButton.Click
        Dim lineStatus As LineStatus = oTelegrapher.LineStatus
        If lineStatus = LineStatus.Initial OrElse _
           lineStatus = LineStatus.Disconnected Then
            'OPT: Connect()の間は、接続ボタンは押下不可にするべきであるが、
            'ウィンドウの移動やアプリ終了、その他UIの更新はできる方がよい。
            'つまり、BeginConnect()等を用いて実装するべきである。
            ConButton.Enabled = False
            Try
                Dim oTelegSock As Socket
                Try
                    oTelegSock = SockUtil.Connect(Config.ServerIpAddr, Config.IpPortForTelegConnection)
                Catch ex As OPMGException
                    Log.Error("Exception caught.", ex)
                    AlertBox.Show(Lexis.ConnectFailed)
                    Return
                End Try

                Log.Info("Sending new socket to the telegrapher...")
                oTelegrapher.LineStatus = LineStatus.Connected
                If ConnectNotice.Gen(oTelegSock).WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
                    Log.Fatal("The telegrapher seems broken.")
                    AlertBox.Show(Lexis.UnwelcomeExceptionCaught)
                    Close()
                End If

                ConButton.Text = "切断"
                ConButton.BackColor = Color.Green
            Finally
                ConButton.Enabled = True
            End Try
        Else
            ConButton.Enabled = False
            Log.Info("Sending disconnect request to the telegrapher...")
            If DisconnectRequest.Gen().WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
                Log.Fatal("The telegrapher seems broken.")
                AlertBox.Show(Lexis.UnwelcomeExceptionCaught)
                Close()
            End If
        End If
    End Sub

    Private Sub LineStatusPollTimer_Tick(sender As System.Object, e As System.EventArgs) Handles LineStatusPollTimer.Tick
        Dim lineStatus As LineStatus = oTelegrapher.LineStatus
        If lineStatus = lineStatus.Disconnected Then
            ConButton.Text = "接続"
            ConButton.ResetBackColor()
            ConButton.Enabled = True
        End If
    End Sub

    Private Sub LoggerClearButton_Click(sender As System.Object, e As System.EventArgs) Handles LoggerClearButton.Click
        LoggerTextBox.Clear()
    End Sub

    Private Sub ComSartButton_Click(sender As System.Object, e As System.EventArgs) Handles ComSartButton.Click
        ComStartExecRequest.Gen().WriteToSocket(oChildSteerSock)
    End Sub

    Private Sub InquiryButton_Click(sender As System.Object, e As System.EventArgs) Handles InquiryButton.Click
        InquiryExecRequest.Gen().WriteToSocket(oChildSteerSock)
    End Sub

    Private Sub AutomaticComStartCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles AutomaticComStartCheckBox.CheckedChanged
        SyncLock UiState
            UiState.AutomaticComStart = AutomaticComStartCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub CapSndTelegsCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CapSndTelegsCheckBox.CheckedChanged
        SyncLock UiState
            UiState.CapSndTelegs = CapSndTelegsCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub CapRcvTelegsCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CapRcvTelegsCheckBox.CheckedChanged
        SyncLock UiState
            UiState.CapRcvTelegs = CapRcvTelegsCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub CapSndFilesCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CapSndFilesCheckBox.CheckedChanged
        SyncLock UiState
            UiState.CapSndFiles = CapSndFilesCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub CapRcvFilesCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CapRcvFilesCheckBox.CheckedChanged
        SyncLock UiState
            UiState.CapRcvFiles = CapRcvFilesCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub ActiveOneApplyFileSelButton_Click(sender As System.Object, e As System.EventArgs) Handles ActiveOneApplyFileSelButton.Click
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return
        ActiveOneApplyFileTextBox.Text = FileSelDialog.FileName
    End Sub

    Private Sub ActiveOneExecButton_Click(sender As System.Object, e As System.EventArgs) Handles ActiveOneExecButton.Click
        Dim rate As Integer = Decimal.ToInt32(ActiveOneExecRateNumericUpDown.Value)
        If rate = 0 Then
            'NOTE: メッセージ自体を周期的に送信できるため、電文送信のリトライ指定は
            '不要とし、ReplyLimitTicksについてのみ、指定可能にしてある。
            Dim oExt As New ActiveOneExecRequestExtendPart()
            oExt.ApplyFilePath = ActiveOneApplyFileTextBox.Text
            oExt.ReplyLimitTicks = Decimal.ToInt32(ActiveOneReplyLimitNumericUpDown.Value)
            oExt.RetryIntervalTicks = 60000
            oExt.MaxRetryCountToForget = 0
            oExt.MaxRetryCountToCare = 0
            Log.Info("Sending ActiveOneExecRequest to the telegrapher...")
            ActiveOneExecRequest.Gen(oExt).WriteToSocket(oChildSteerSock)
        Else
            If ActiveOneExecTimer.Enabled Then
                ActiveOneExecTimer.Enabled = False
                ActiveOneExecButton.Text = "実行"
                ActiveOneExecButton.ResetBackColor()
                ActiveOneExecRateNumericUpDown.Enabled = True
            Else
                ActiveOneExecTimer.Interval = rate
                ActiveOneExecTimer.Enabled = True
                ActiveOneExecButton.Text = "中止"
                ActiveOneExecButton.BackColor = Color.Green
                ActiveOneExecRateNumericUpDown.Enabled = False
            End If
        End If
    End Sub

    Private Sub ActiveOneExecTimer_Tick(sender As System.Object, e As System.EventArgs) Handles ActiveOneExecTimer.Tick
        Dim oExt As New ActiveOneExecRequestExtendPart()
        oExt.ApplyFilePath = ActiveOneApplyFileTextBox.Text
        oExt.ReplyLimitTicks = Decimal.ToInt32(ActiveOneReplyLimitNumericUpDown.Value)
        oExt.RetryIntervalTicks = 60000
        oExt.MaxRetryCountToForget = 0
        oExt.MaxRetryCountToCare = 0
        Log.Info("Sending ActiveOneExecRequest to the telegrapher...")
        ActiveOneExecRequest.Gen(oExt).WriteToSocket(oChildSteerSock)
    End Sub
End Class
