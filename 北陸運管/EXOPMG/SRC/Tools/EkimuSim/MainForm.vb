' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2014/10/22  (NES)小林  DataGridView全般の挙動を改良
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Net.Sockets
Imports System.Runtime.Serialization
Imports System.Threading
Imports System.Xml

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

'TODO: 幾つもの種類の電文を周期的に送信できるようにするには、
'ActiveOneと同機能のタブを何枚か用意すればよい。

'TODO: 幾つもの種類のファイルを周期的に能動送信できるようにするには、
'ActiveUllと同機能のタブを何枚か用意すればよい。

'TODO: 受信電文によって、NAKの要否や種類を変えたりするには、
'PassiveGetやPassivePostと同機能のタブを何枚か用意すればよい。

'TODO: 指定されたファイルの種別によって、NAKの要否や種類を変えたりするには、
'PassiveUllやPassiveDllと同機能のタブを何枚か用意すればよい。

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

    Protected oTelegGene As EkTelegramGene
    Protected oChildSteerSock As Socket
    Protected oTelegrapher As MyTelegrapher

    Protected oScenario As List(Of ScenarioElement)
    Protected nextExecIndexOfScenario As Integer
    Protected nextExecTimingOfScenario As DateTime

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

        Dim oSerializer As New DataContractSerializer(GetType(UiStateClass))
        Dim sStateFileUri As String = Path.ChangeExtension(Path.GetFileName(Application.ExecutablePath), ".xml")
        sStateFileUri = sStateFileUri.Insert(sStateFileUri.Length - 4, "State")
        Try
            Using xr As XmlReader = XmlReader.Create(sStateFileUri)
                UiState = DirectCast(oSerializer.ReadObject(xr), UiStateClass)
            End Using
        Catch ex As FileNotFoundException
            Log.Info("Initializing UiState...")
            UiState = New UiStateClass()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.UiStateDeserializeFailed)
            Me.Close()
            Return
        End Try

        Me.SuspendLayout() '---------------------------------------------------

        'Lexis から生成した文言を各コントロールに反映する。

        Me.Text = Lexis.FormTitle.Gen(Config.SelfEkCode.ToString(Lexis.FormTitleEkCodeFormat.Gen()))

        'UiStateの値を各コントロールに反映する。

        AutomaticComStartCheckBox.Checked = UiState.AutomaticComStart
        CapSndTelegsCheckBox.Checked = UiState.CapSndTelegs
        CapRcvTelegsCheckBox.Checked = UiState.CapRcvTelegs
        CapSndFilesCheckBox.Checked = UiState.CapSndFiles
        CapRcvFilesCheckBox.Checked = UiState.CapRcvFiles

        ActiveOneApplyFileTextBox.Text = UiState.ActiveOneApplyFilePath
        ActiveOneReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveOneReplyLimitTicks)
        ActiveOneExecIntervalNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveOneExecIntervalTicks)

        ActiveUllObjCodeTextBox.Text = UiState.ActiveUllObjCode
        ActiveUllTransferFileTextBox.Text = UiState.ActiveUllTransferFilePath
        ActiveUllTransferLimitNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveUllTransferLimitTicks)
        ActiveUllReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveUllReplyLimitTicks)
        ActiveUllExecIntervalNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveUllExecIntervalTicks)

        For Each oKeyValue As KeyValuePair(Of Byte, String) In UiState.ApplyFileForPassiveGetObjCodes
            Dim oRow As New DataGridViewRow()
            oRow.CreateCells(PassiveGetDataGridView)
            With oRow
                .Cells(0).Value = oKeyValue.Key.ToString("X2")
                .Cells(1).Value = oKeyValue.Value
            End With
            PassiveGetDataGridView.Rows.Add(oRow)
        Next
        PassiveGetForceReplyNakCheckBox.Checked = UiState.ForceReplyNakToPassiveGetReq
        PassiveGetNakCauseNumberTextBox.Text = UiState.NakCauseNumberToPassiveGetReq.ToString()
        PassiveGetNakCauseTextTextBox.Text = UiState.NakCauseTextToPassiveGetReq

        For Each oKeyValue As KeyValuePair(Of Byte, String) In UiState.ApplyFileForPassiveUllObjCodes
            Dim oRow As New DataGridViewRow()
            oRow.CreateCells(PassiveUllDataGridView)
            With oRow
                .Cells(0).Value = oKeyValue.Key.ToString("X2")
                .Cells(1).Value = oKeyValue.Value
            End With
            PassiveUllDataGridView.Rows.Add(oRow)
        Next
        PassiveUllForceReplyNakCheckBox.Checked = UiState.ForceReplyNakToPassiveUllStartReq
        PassiveUllNakCauseNumberTextBox.Text = UiState.NakCauseNumberToPassiveUllStartReq.ToString()
        PassiveUllNakCauseTextTextBox.Text = UiState.NakCauseTextToPassiveUllStartReq
        PassiveUllTransferLimitNumericUpDown.Value = Convert.ToDecimal(UiState.PassiveUllTransferLimitTicks)
        PassiveUllReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.PassiveUllFinishReplyLimitTicks)

        For Each oKeyValue As KeyValuePair(Of Byte, String) In UiState.SomethingForPassivePostObjCodes
            Dim oRow As New DataGridViewRow()
            oRow.CreateCells(PassivePostDataGridView)
            With oRow
                .Cells(0).Value = oKeyValue.Key.ToString("X2")
            End With
            PassivePostDataGridView.Rows.Add(oRow)
        Next
        PassivePostForceReplyNakCheckBox.Checked = UiState.ForceReplyNakToPassivePostReq
        PassivePostNakCauseNumberTextBox.Text = UiState.NakCauseNumberToPassivePostReq.ToString()
        PassivePostNakCauseTextTextBox.Text = UiState.NakCauseTextToPassivePostReq

        For Each oKeyValue As KeyValuePair(Of Byte, String) In UiState.SomethingForPassiveDllObjCodes
            Dim oRow As New DataGridViewRow()
            oRow.CreateCells(PassiveDllDataGridView)
            With oRow
                .Cells(0).Value = oKeyValue.Key.ToString("X2")
            End With
            PassiveDllDataGridView.Rows.Add(oRow)
        Next
        PassiveDllForceReplyNakCheckBox.Checked = UiState.ForceReplyNakToPassiveDllStartReq
        PassiveDllNakCauseNumberTextBox.Text = UiState.NakCauseNumberToPassiveDllStartReq.ToString()
        PassiveDllNakCauseTextTextBox.Text = UiState.NakCauseTextToPassiveDllStartReq
        PassiveDllTransferLimitNumericUpDown.Value = Convert.ToDecimal(UiState.PassiveDllTransferLimitTicks)
        PassiveDllReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.PassiveDllFinishReplyLimitTicks)
        PassiveDllSimulateStoringCheckBox.Checked = UiState.SimulateStoringOnPassiveDllComplete
        PassiveDllResultantVersionOfSlot1TextBox.Text = UiState.PassiveDllResultantVersionOfSlot1.ToString("D8")
        PassiveDllResultantVersionOfSlot2TextBox.Text = UiState.PassiveDllResultantVersionOfSlot2.ToString("D8")
        PassiveDllResultantFlagOfFullTextBox.Text = UiState.PassiveDllResultantFlagOfFull.ToString("X2")

        ScenarioFileTextBox.Text = UiState.ScenarioFilePath
        ScenarioExecIntervalNumericUpDown.Value = Convert.ToDecimal(UiState.ScenarioExecIntervalTicks)

        Me.ResumeLayout() '----------------------------------------------------

        Dim sFtpBasePath As String = Path.Combine(sWorkingDir, "TMP")
        Dim sCapDirPath As String = Path.Combine(sWorkingDir, "CAP")

        'FTPの一時作業用ディレクトリを削除する。
        Log.Info("Sweeping directory [" & sFtpBasePath & "]...")
        Utility.DeleteTemporalDirectory(sFtpBasePath)

        '送受信履歴ディレクトリについて、無ければ作成しておく。
        Directory.CreateDirectory(sCapDirPath)

        '電文書式オブジェクトを作成する。
        oTelegGene = New EkTelegramGeneForNativeModels(sFtpBasePath)

        '電文送受信スレッドを作成する。
        Dim oMessageSockForTelegrapher As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oChildSteerSock, oMessageSockForTelegrapher)
        oTelegrapher = New MyTelegrapher("ToOpmg", oMessageSockForTelegrapher, oTelegGene, sFtpBasePath, sCapDirPath, Me)

        '電文送受信スレッドを開始する。
        Log.Info("Starting the telegrapher...")
        oTelegrapher.Start()

        LineStatusPollTimer.Start()
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        LineStatusPollTimer.Stop()

        If ScenarioExecTimer.Enabled Then
            ScenarioExecTimer.Enabled = False
            TerminateScenario()
        End If

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

        If UiState IsNot Nothing Then
            'NOTE: このケースでは、右辺の各コントロールに、少なくとも起動時のファイルから
            'ロードした値はセット済みの想定である。

            UiState.ActiveOneApplyFilePath = ActiveOneApplyFileTextBox.Text
            UiState.ActiveOneReplyLimitTicks = Decimal.ToInt32(ActiveOneReplyLimitNumericUpDown.Value)
            UiState.ActiveOneExecIntervalTicks = Decimal.ToInt32(ActiveOneExecIntervalNumericUpDown.Value)

            UiState.ActiveUllObjCode = ActiveUllObjCodeTextBox.Text
            UiState.ActiveUllTransferFilePath = ActiveUllTransferFileTextBox.Text
            UiState.ActiveUllTransferLimitTicks = Decimal.ToInt32(ActiveUllTransferLimitNumericUpDown.Value)
            UiState.ActiveUllReplyLimitTicks = Decimal.ToInt32(ActiveUllReplyLimitNumericUpDown.Value)
            UiState.ActiveUllExecIntervalTicks = Decimal.ToInt32(ActiveUllExecIntervalNumericUpDown.Value)

            UiState.ScenarioFilePath = ScenarioFileTextBox.Text
            UiState.ScenarioExecIntervalTicks = Decimal.ToInt32(ScenarioExecIntervalNumericUpDown.Value)

            Dim oSerializer As New DataContractSerializer(GetType(UiStateClass))
            Dim sStateFileUri As String = Path.ChangeExtension(Path.GetFileName(Application.ExecutablePath), ".xml")
            sStateFileUri = sStateFileUri.Insert(sStateFileUri.Length - 4, "State")
            Try
                Using xw As XmlWriter = XmlWriter.Create(sStateFileUri)
                    oSerializer.WriteObject(xw, UiState)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.UiStateSerializeFailed)
            End Try
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

        If LoggerPreviewCheckBox.Checked Then
            LoggerTextBox.AppendText("[" & sDateTime & "] [" & sSecondName & "] " & sText & vbCrLf)
        End If
    End Sub

    Private Sub ConButton_Click(sender As System.Object, e As System.EventArgs) Handles ConButton.Click
        Dim lnSts As LineStatus = oTelegrapher.LineStatus
        If lnSts = LineStatus.Initial OrElse _
           lnSts = LineStatus.Disconnected Then
            If DoConnect() = False Then
                AlertBox.Show(Lexis.ConnectFailed)
            End If
        Else
            DoDisconnect()
        End If
    End Sub

    Private Sub LineStatusPollTimer_Tick(sender As System.Object, e As System.EventArgs) Handles LineStatusPollTimer.Tick
        Dim lnSts As LineStatus = oTelegrapher.LineStatus
        If lnSts = LineStatus.Disconnected Then
            Disconnected()
        End If
    End Sub

    Private Sub LoggerClearButton_Click(sender As System.Object, e As System.EventArgs) Handles LoggerClearButton.Click
        LoggerTextBox.Clear()
    End Sub

    Private Sub ComSartButton_Click(sender As System.Object, e As System.EventArgs) Handles ComSartButton.Click
        ComStartExecRequest.Gen().WriteToSocket(oChildSteerSock)
    End Sub

    Private Sub TimeDataGetButton_Click(sender As System.Object, e As System.EventArgs) Handles TimeDataGetButton.Click
        TimeDataGetExecRequest.Gen().WriteToSocket(oChildSteerSock)
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
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return
        ActiveOneApplyFileTextBox.Text = FileSelDialog.FileName
    End Sub

    Private Sub ActiveOneExecButton_Click(sender As System.Object, e As System.EventArgs) Handles ActiveOneExecButton.Click
        Dim rate As Integer = Decimal.ToInt32(ActiveOneExecIntervalNumericUpDown.Value)
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
                ActiveOneExecIntervalNumericUpDown.Enabled = True
            Else
                ActiveOneExecTimer.Interval = rate
                ActiveOneExecTimer.Enabled = True
                ActiveOneExecButton.Text = "中止"
                ActiveOneExecButton.BackColor = Color.Green
                ActiveOneExecIntervalNumericUpDown.Enabled = False
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

    Private Sub ActiveUllTransferFileSelButton_Click(sender As System.Object, e As System.EventArgs) Handles ActiveUllTransferFileSelButton.Click
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return
        ActiveUllTransferFileTextBox.Text = FileSelDialog.FileName
    End Sub

    Private Sub ActiveUllExecButton_Click(sender As System.Object, e As System.EventArgs) Handles ActiveUllExecButton.Click
        Dim objCode As Integer
        If Integer.TryParse(ActiveUllObjCodeTextBox.Text, NumberStyles.HexNumber, Nothing, objCode) = False Then
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForObjCode)
            Return
        End If

        Dim rate As Integer = Decimal.ToInt32(ActiveUllExecIntervalNumericUpDown.Value)
        If rate = 0 Then
            'NOTE: メッセージ自体を周期的に送信できるため、電文送信のリトライ指定は
            '不要とし、ReplyLimitTicksについてのみ、指定可能にしてある。
            Dim oExt As New ActiveUllExecRequestExtendPart()
            oExt.ObjCode = objCode
            oExt.TransferFilePath = ActiveUllTransferFileTextBox.Text
            oExt.TransferFileHashValue = ""
            oExt.TransferLimitTicks = Decimal.ToInt32(ActiveUllTransferLimitNumericUpDown.Value)
            oExt.ReplyLimitTicks = Decimal.ToInt32(ActiveUllReplyLimitNumericUpDown.Value)
            oExt.RetryIntervalTicks = 60000
            oExt.MaxRetryCountToForget = 0
            oExt.MaxRetryCountToCare = 0
            Log.Info("Sending ActiveUllExecRequest to the telegrapher...")
            ActiveUllExecRequest.Gen(oExt).WriteToSocket(oChildSteerSock)
        Else
            If ActiveUllExecTimer.Enabled Then
                ActiveUllExecTimer.Enabled = False
                ActiveUllExecButton.Text = "実行"
                ActiveUllExecButton.ResetBackColor()
                ActiveUllExecIntervalNumericUpDown.Enabled = True
                ActiveUllObjCodeTextBox.Enabled = True
            Else
                ActiveUllExecTimer.Interval = rate
                ActiveUllExecTimer.Enabled = True
                ActiveUllExecButton.Text = "中止"
                ActiveUllExecButton.BackColor = Color.Green
                ActiveUllExecIntervalNumericUpDown.Enabled = False
                ActiveUllObjCodeTextBox.Enabled = False
            End If
        End If
    End Sub

    Private Sub ActiveUllExecTimer_Tick(sender As System.Object, e As System.EventArgs) Handles ActiveUllExecTimer.Tick
        Dim oExt As New ActiveUllExecRequestExtendPart()
        oExt.ObjCode = Integer.Parse(ActiveUllObjCodeTextBox.Text, NumberStyles.HexNumber)
        oExt.TransferFilePath = ActiveUllTransferFileTextBox.Text
        oExt.TransferFileHashValue = ""
        oExt.TransferLimitTicks = Decimal.ToInt32(ActiveUllTransferLimitNumericUpDown.Value)
        oExt.ReplyLimitTicks = Decimal.ToInt32(ActiveUllReplyLimitNumericUpDown.Value)
        oExt.RetryIntervalTicks = 60000
        oExt.MaxRetryCountToForget = 0
        oExt.MaxRetryCountToCare = 0
        Log.Info("Sending ActiveUllExecRequest to the telegrapher...")
        ActiveUllExecRequest.Gen(oExt).WriteToSocket(oChildSteerSock)
    End Sub

    'NOTE: lastEditRowは編集中の行番号。編集中でない場合は-1とする。
    'NOTE: sKeyAtBeginEditRowInDataGridViewは編集中の行の、編集開始時のキー値。
    'lastEditRowが-1以外の場合のみ有意である。新規の行を編集中はNothingとする。
    Private lastEditRow As Integer = -1
    Private sKeyAtBeginEditRowInDataGridView As String

    Private Sub PassiveGetDataGridView_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles PassiveGetDataGridView.CellMouseClick
        '左クリックの場合は、このメソッドでは処理しない。
        If e.Button <> MouseButtons.Right Then Return

        '列ヘッダを右クリックした場合は、このメソッドでは処理しない。
        If e.RowIndex = -1 Then Return

        '右クリックした場所に選択を移す。
        If e.ColumnIndex = -1 Then
            'NOTE: 行ヘッダが右クリックされた場合である。
            '当該行の１列目セルを選択しているが、これは、行ヘッダを選択しても
            '直前まで選択されていた行の妥当性チェックが実行されないことおよび、
            '直前まで選択されていた行の選択が解除されないことに対処するためである。
            PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveGetDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '右クリックした行とは別の編集中の行が存在する場合は、メニューは出さない。
        'ただし、編集中の行が真の編集中ではない場合は、メニューを出す。
        If lastEditRow <> -1 AndAlso lastEditRow <> e.RowIndex Then
            If Not (PassiveGetDataGridView.Rows(lastEditRow).IsNewRow AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveGetDataGridView.Rows(lastEditRow).Cells(0).Value)) AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveGetDataGridView.Rows(lastEditRow).Cells(1).Value))) Then
                Return
            End If
        End If

        If e.ColumnIndex = -1 Then
            PassiveGetDataGridView.Rows(e.RowIndex).Selected = True
            Application.DoEvents()
            PassiveGetRowHeaderMenu.Show(PassiveGetDataGridView, PassiveGetDataGridView.GetRowDisplayRectangle(e.RowIndex, False).Location + New Size(e.Location))
        ElseIf e.ColumnIndex = 1 Then
            PassiveGetApplyFileMenu.Show(PassiveGetDataGridView, PassiveGetDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False).Location + New Size(e.Location))
        End If
    End Sub

    Private Sub PassiveGetDelMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PassiveGetDelMenuItem.Click
        RemovePassiveGetData()
    End Sub

    Private Sub PassiveGetSelMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PassiveGetSelMenuItem.Click
        SelectPassiveGetDataApplyFile()
    End Sub

    Private Sub PassiveGetDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles PassiveGetDataGridView.KeyDown
        Select Case e.KeyData
            Case Keys.Delete
                If PassiveGetDataGridView.SelectedRows.Count = 1 Then
                    RemovePassiveGetData()
                    e.Handled = True
                End If
            Case Keys.Apps
                If PassiveGetDataGridView.SelectedRows.Count = 0 AndAlso _
                   PassiveGetDataGridView.SelectedCells.Count = 1 AndAlso _
                   PassiveGetDataGridView.SelectedCells(0).ColumnIndex = 1 Then
                    Dim r As Rectangle = PassiveGetDataGridView.GetCellDisplayRectangle(1, PassiveGetDataGridView.SelectedCells(0).RowIndex, False)
                    PassiveGetApplyFileMenu.Show(PassiveGetDataGridView, r.Location + New Size((r.Size.Width - PassiveGetApplyFileMenu.Size.Width) \ 2, r.Size.Height))
                    e.Handled = True
                End If
        End Select
    End Sub

    Private Sub RemovePassiveGetData()
        Dim selectedRow As DataGridViewRow = PassiveGetDataGridView.SelectedRows(0)

        If selectedRow.IsNewRow Then
            selectedRow.Cells(0).Value = Nothing
            selectedRow.Cells(1).Value = Nothing
            selectedRow.ErrorText = Nothing
            Return
        End If

        If lastEditRow <> selectedRow.Index Then
            SyncLock UiState
                UiState.ApplyFileForPassiveGetObjCodes.Remove(Byte.Parse(CStr(selectedRow.Cells(0).Value), NumberStyles.HexNumber))
            End SyncLock
        ElseIf sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView IsNot Nothing)
            SyncLock UiState
                UiState.ApplyFileForPassiveGetObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
            End SyncLock
        Else
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView Is Nothing)
            'NOTE: おそらく、selectedRow.IsNewRowに該当するケースであるため、
            'ここまで到達しないと思われる。たとえ到達したとしても、
            '新規の行を編集中にその行の削除を実施した場合である故、
            'Dictionaryには内容を登録していないので、Dictionaryからの削除は無用である。
        End If

        If lastEditRow = selectedRow.Index Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどで、余計なことが
            '行われないように、この時点で、編集中ではなかったことにしておく。
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどを処理する際、
            'lastEditRowが位置として参照されないことを前提に、
            '邪道ではあるが、この時点で補正を行っておく。
            'NOTE: デクリメント前の時点でlastEditRowは1以上であるため、
            'デクリメントの結果が-1やそれ以下になることはない。
            'NOTE: Rows.RemoveAt(...)によるRowValidatedイベントを処理する際、
            'lastEditRowは-1に変更される。さらに、Rows.RemoveAt(...)の結果として
            '既存の行が全て無くなれば、DefaultValuesNeededイベントが発生し、
            'lastEditRowは新規行の位置（おそらく0）に変更される。つまり、
            'この補正は、事実上不要である可能性が高い。
            lastEditRow -= 1
        End If

        PassiveGetDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub SelectPassiveGetDataApplyFile()
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return

        Dim selectedRow As DataGridViewRow = PassiveGetDataGridView.Rows(PassiveGetDataGridView.SelectedCells(0).RowIndex)

        'NOTE: 編集中の行や新規の行に対して、ファイル名の選択を実施した場合、
        'その場でのUiState.ApplyFileForPassiveGetObjCodesへの反映は
        '無用である（編集を確定した時点で実施されるはずである）上、
        'sKeyがNothingの可能性もある。
        'このことから、UiState.ApplyFileForPassiveGetObjCodesへの反映には
        '条件を設けている。
        Dim sKey As String = CStr(selectedRow.Cells(0).Value)
        If lastEditRow <> selectedRow.Index AndAlso _
           Not selectedRow.IsNewRow Then
            SyncLock UiState
                UiState.ApplyFileForPassiveGetObjCodes(Byte.Parse(sKey, NumberStyles.HexNumber)) = FileSelDialog.FileName
            End SyncLock
        End If

        selectedRow.Cells(1).Selected = True
        selectedRow.Cells(1).Value = FileSelDialog.FileName
    End Sub

    Private Sub PassiveGetDataGridView_DefaultValuesNeeded(ByVal sender As Object, ByVal e As DataGridViewRowEventArgs) Handles PassiveGetDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing
        e.Row.Cells(1).Value = Nothing

        'NOTE: DataGridViewは、新規の行（アスタリスクの行）を選択後、
        'ダブルクリックやキー入力で編集を開始した際、編集中と同じ状態
        'になる一方で、CellBeginEditイベントは発生しないようなので、
        'まだ選択されただけの段階ではあるが、CellBeginEditイベント
        '発生時と同じ処理をここで実施することにしている。
        'この処置のせいで、真に編集中でない場合（キャレットが登場
        'していない場合）でもlastEditRowは-1以外になり得るので注意。
        'lastEditRow行のIsNewRowプロパティがTrueでかつ、その全セルが
        '空の場合は、真に編集中ではないとみなすことにする。
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveGetDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveGetDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: 仮に新規の行を編集開始して、ここが実行される場合は、
            'Nothingを代入することになる。
            sKeyAtBeginEditRowInDataGridView = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Value)
        End If
    End Sub

    Private Sub PassiveGetDataGridView_RowValidating(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveGetDataGridView.RowValidating
        If lastEditRow <> -1 Then
            Dim sNewKey As String = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Value)
            Dim sNewApplyFile As String = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(1).Value)

            If PassiveGetDataGridView.Rows(e.RowIndex).IsNewRow AndAlso _
               String.IsNullOrEmpty(sNewKey) AndAlso _
               String.IsNullOrEmpty(sNewApplyFile) Then Return

            Dim newKey As Byte
            If Byte.TryParse(sNewKey, NumberStyles.HexNumber, Nothing, newKey) = False Then
                PassiveGetDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsUnsuitableForObjCode.Gen()
                e.Cancel = True
                Return
            End If

            '新規の行を挿入した場合や、既に存在する行のキーを変更した場合は、
            '新しいキーが、他の行のキーと重複していないかチェックする。
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: このスレッド以外は、UiStateを参照するだけなので、このスレッドで
                'UiStateを参照するだけであれば、SyncLock UiStateは不要である。
                If UiState.ApplyFileForPassiveGetObjCodes.ContainsKey(newKey) Then
                    PassiveGetDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsDuplicative.Gen()
                    e.Cancel = True
                    Return
                End If
            End If
        End If
    End Sub

    Private Sub PassiveGetDataGridView_RowValidated(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles PassiveGetDataGridView.RowValidated
        If lastEditRow <> -1 Then
            PassiveGetDataGridView.Rows(e.RowIndex).ErrorText = Nothing

            Dim sNewKey As String = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Value)
            Dim sNewApplyFile As String = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(1).Value)
            If sNewApplyFile Is Nothing Then
                sNewApplyFile = ""
            End If

            SyncLock UiState
                If sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
                    UiState.ApplyFileForPassiveGetObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
                End If

                'NOTE: 以下の分岐に入らないケースは、RowValidatingで特別扱いしたケースである故、
                'sNewApplyFileも確実に空である。また、そのケースは、Rows(e.RowIndex).IsNewRow が
                'True である故、DataGridView上に行は追加されておらず、Rows.RemoveAt(e.RowIndex)
                'も無用である。
                If Not String.IsNullOrEmpty(sNewKey) Then
                    Dim newKey As Byte = Byte.Parse(sNewKey, NumberStyles.HexNumber)
                    UiState.ApplyFileForPassiveGetObjCodes.Add(newKey, sNewApplyFile)
                End If
            End SyncLock

            lastEditRow = -1
        End If
    End Sub

    Private Sub PassiveGetForceReplyNakCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles PassiveGetForceReplyNakCheckBox.CheckedChanged
        SyncLock UiState
            UiState.ForceReplyNakToPassiveGetReq = PassiveGetForceReplyNakCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassiveGetNakCauseNumberTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveGetNakCauseNumberTextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveGetNakCauseNumberTextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveGetNakCauseNumberTextBox.Text)
        End If

        SyncLock UiState
            UiState.NakCauseNumberToPassiveGetReq = number
        End SyncLock
    End Sub

    Private Sub PassiveGetNakCauseTextTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveGetNakCauseTextTextBox.TextChanged
        SyncLock UiState
            UiState.NakCauseTextToPassiveGetReq = PassiveGetNakCauseTextTextBox.Text
        End SyncLock
    End Sub

    Private Sub PassiveUllDataGridView_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles PassiveUllDataGridView.CellMouseClick
        '左クリックの場合は、このメソッドでは処理しない。
        If e.Button <> MouseButtons.Right Then Return

        '列ヘッダを右クリックした場合は、このメソッドでは処理しない。
        If e.RowIndex = -1 Then Return

        '右クリックした場所に選択を移す。
        If e.ColumnIndex = -1 Then
            'NOTE: 行ヘッダが右クリックされた場合である。
            '当該行の１列目セルを選択しているが、これは、行ヘッダを選択しても
            '直前まで選択されていた行の妥当性チェックが実行されないことおよび、
            '直前まで選択されていた行の選択が解除されないことに対処するためである。
            PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveUllDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '右クリックした行とは別の編集中の行が存在する場合は、メニューは出さない。
        'ただし、編集中の行が真の編集中ではない場合は、メニューを出す。
        If lastEditRow <> -1 AndAlso lastEditRow <> e.RowIndex Then
            If Not (PassiveUllDataGridView.Rows(lastEditRow).IsNewRow AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveUllDataGridView.Rows(lastEditRow).Cells(0).Value)) AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveUllDataGridView.Rows(lastEditRow).Cells(1).Value))) Then
                Return
            End If
        End If

        If e.ColumnIndex = -1 Then
            PassiveUllDataGridView.Rows(e.RowIndex).Selected = True
            Application.DoEvents()
            PassiveUllRowHeaderMenu.Show(PassiveUllDataGridView, PassiveUllDataGridView.GetRowDisplayRectangle(e.RowIndex, False).Location + New Size(e.Location))
        ElseIf e.ColumnIndex = 1 Then
            PassiveUllApplyFileMenu.Show(PassiveUllDataGridView, PassiveUllDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False).Location + New Size(e.Location))
        End If
    End Sub

    Private Sub PassiveUllDelMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PassiveUllDelMenuItem.Click
        RemovePassiveUllData()
    End Sub

    Private Sub PassiveUllSelMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PassiveUllSelMenuItem.Click
        SelectPassiveUllDataApplyFile()
    End Sub

    Private Sub PassiveUllDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles PassiveUllDataGridView.KeyDown
        Select Case e.KeyData
            Case Keys.Delete
                If PassiveUllDataGridView.SelectedRows.Count = 1 Then
                    RemovePassiveUllData()
                    e.Handled = True
                End If
            Case Keys.Apps
                If PassiveUllDataGridView.SelectedRows.Count = 0 AndAlso _
                   PassiveUllDataGridView.SelectedCells.Count = 1 AndAlso _
                   PassiveUllDataGridView.SelectedCells(0).ColumnIndex = 1 Then
                    Dim r As Rectangle = PassiveUllDataGridView.GetCellDisplayRectangle(1, PassiveUllDataGridView.SelectedCells(0).RowIndex, False)
                    PassiveUllApplyFileMenu.Show(PassiveUllDataGridView, r.Location + New Size((r.Size.Width - PassiveUllApplyFileMenu.Size.Width) \ 2, r.Size.Height))
                    e.Handled = True
                End If
        End Select
    End Sub

    Private Sub RemovePassiveUllData()
        Dim selectedRow As DataGridViewRow = PassiveUllDataGridView.SelectedRows(0)

        If selectedRow.IsNewRow Then
            selectedRow.Cells(0).Value = Nothing
            selectedRow.Cells(1).Value = Nothing
            selectedRow.ErrorText = Nothing
            Return
        End If

        If lastEditRow <> selectedRow.Index Then
            SyncLock UiState
                UiState.ApplyFileForPassiveUllObjCodes.Remove(Byte.Parse(CStr(selectedRow.Cells(0).Value), NumberStyles.HexNumber))
            End SyncLock
        ElseIf sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView IsNot Nothing)
            SyncLock UiState
                UiState.ApplyFileForPassiveUllObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
            End SyncLock
        Else
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView Is Nothing)
            'NOTE: おそらく、selectedRow.IsNewRowに該当するケースであるため、
            'ここまで到達しないと思われる。たとえ到達したとしても、
            '新規の行を編集中にその行の削除を実施した場合である故、
            'Dictionaryには内容を登録していないので、Dictionaryからの削除は無用である。
        End If

        If lastEditRow = selectedRow.Index Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどで、余計なことが
            '行われないように、この時点で、編集中ではなかったことにしておく。
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどを処理する際、
            'lastEditRowが位置として参照されないことを前提に、
            '邪道ではあるが、この時点で補正を行っておく。
            'NOTE: デクリメント前の時点でlastEditRowは1以上であるため、
            'デクリメントの結果が-1やそれ以下になることはない。
            'NOTE: Rows.RemoveAt(...)によるRowValidatedイベントを処理する際、
            'lastEditRowは-1に変更される。さらに、Rows.RemoveAt(...)の結果として
            '既存の行が全て無くなれば、DefaultValuesNeededイベントが発生し、
            'lastEditRowは新規行の位置（おそらく0）に変更される。つまり、
            'この補正は、事実上不要である可能性が高い。
            lastEditRow -= 1
        End If

        PassiveUllDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub SelectPassiveUllDataApplyFile()
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return

        Dim selectedRow As DataGridViewRow = PassiveUllDataGridView.Rows(PassiveUllDataGridView.SelectedCells(0).RowIndex)

        'NOTE: 編集中の行や新規の行に対して、ファイル名の選択を実施した場合、
        'その場でのUiState.ApplyFileForPassiveUllObjCodesへの反映は
        '無用である（編集を確定した時点で実施されるはずである）上、
        'sKeyがNothingの可能性もある。
        'このことから、UiState.ApplyFileForPassiveUllObjCodesへの反映には
        '条件を設けている。
        Dim sKey As String = CStr(selectedRow.Cells(0).Value)
        If lastEditRow <> selectedRow.Index AndAlso _
           Not selectedRow.IsNewRow Then
            SyncLock UiState
                UiState.ApplyFileForPassiveUllObjCodes(Byte.Parse(sKey, NumberStyles.HexNumber)) = FileSelDialog.FileName
            End SyncLock
        End If

        selectedRow.Cells(1).Selected = True
        selectedRow.Cells(1).Value = FileSelDialog.FileName
    End Sub

    Private Sub PassiveUllDataGridView_DefaultValuesNeeded(ByVal sender As Object, ByVal e As DataGridViewRowEventArgs) Handles PassiveUllDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing
        e.Row.Cells(1).Value = Nothing

        'NOTE: DataGridViewは、新規の行（アスタリスクの行）を選択後、
        'ダブルクリックやキー入力で編集を開始した際、編集中と同じ状態
        'になる一方で、CellBeginEditイベントは発生しないようなので、
        'まだ選択されただけの段階ではあるが、CellBeginEditイベント
        '発生時と同じ処理をここで実施することにしている。
        'この処置のせいで、真に編集中でない場合（キャレットが登場
        'していない場合）でもlastEditRowは-1以外になり得るので注意。
        'lastEditRow行のIsNewRowプロパティがTrueでかつ、その全セルが
        '空の場合は、真に編集中ではないとみなすことにする。
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveUllDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveUllDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: 仮に新規の行を編集開始して、ここが実行される場合は、
            'Nothingを代入することになる。
            sKeyAtBeginEditRowInDataGridView = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Value)
        End If
    End Sub

    Private Sub PassiveUllDataGridView_RowValidating(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveUllDataGridView.RowValidating
        If lastEditRow <> -1 Then
            Dim sNewKey As String = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Value)
            Dim sNewApplyFile As String = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(1).Value)

            If PassiveUllDataGridView.Rows(e.RowIndex).IsNewRow AndAlso _
               String.IsNullOrEmpty(sNewKey) AndAlso _
               String.IsNullOrEmpty(sNewApplyFile) Then Return

            Dim newKey As Byte
            If Byte.TryParse(sNewKey, NumberStyles.HexNumber, Nothing, newKey) = False Then
                PassiveUllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsUnsuitableForObjCode.Gen()
                e.Cancel = True
                Return
            End If

            '新規の行を挿入した場合や、既に存在する行のキーを変更した場合は、
            '新しいキーが、他の行のキーと重複していないかチェックする。
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: このスレッド以外は、UiStateを参照するだけなので、このスレッドで
                'UiStateを参照するだけであれば、SyncLock UiStateは不要である。
                If UiState.ApplyFileForPassiveUllObjCodes.ContainsKey(newKey) Then
                    PassiveUllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsDuplicative.Gen()
                    e.Cancel = True
                    Return
                End If
            End If
        End If
    End Sub

    Private Sub PassiveUllDataGridView_RowValidated(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles PassiveUllDataGridView.RowValidated
        If lastEditRow <> -1 Then
            PassiveUllDataGridView.Rows(e.RowIndex).ErrorText = Nothing

            Dim sNewKey As String = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Value)
            Dim sNewApplyFile As String = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(1).Value)
            If sNewApplyFile Is Nothing Then
                sNewApplyFile = ""
            End If

            SyncLock UiState
                If sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
                    UiState.ApplyFileForPassiveUllObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
                End If

                'NOTE: 以下の分岐に入らないケースは、RowValidatingで特別扱いしたケースである故、
                'sNewApplyFileも確実に空である。また、そのケースは、Rows(e.RowIndex).IsNewRow が
                'True である故、DataGridView上に行は追加されておらず、Rows.RemoveAt(e.RowIndex)
                'も無用である。
                If Not String.IsNullOrEmpty(sNewKey) Then
                    Dim newKey As Byte = Byte.Parse(sNewKey, NumberStyles.HexNumber)
                    UiState.ApplyFileForPassiveUllObjCodes.Add(newKey, sNewApplyFile)
                End If
            End SyncLock

            lastEditRow = -1
        End If
    End Sub

    Private Sub PassiveUllForceReplyNakCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles PassiveUllForceReplyNakCheckBox.CheckedChanged
        SyncLock UiState
            UiState.ForceReplyNakToPassiveUllStartReq = PassiveUllForceReplyNakCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassiveUllNakCauseNumberTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveUllNakCauseNumberTextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveUllNakCauseNumberTextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveUllNakCauseNumberTextBox.Text)
        End If

        SyncLock UiState
            UiState.NakCauseNumberToPassiveUllStartReq = number
        End SyncLock
    End Sub

    Private Sub PassiveUllNakCauseTextTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveUllNakCauseTextTextBox.TextChanged
        SyncLock UiState
            UiState.NakCauseTextToPassiveUllStartReq = PassiveUllNakCauseTextTextBox.Text
        End SyncLock
    End Sub

    Private Sub PassiveUllTransferLimitNumericUpDown_ValueChanged(sender As System.Object, e As System.EventArgs) Handles PassiveUllTransferLimitNumericUpDown.ValueChanged
        SyncLock UiState
            UiState.PassiveUllTransferLimitTicks = Decimal.ToInt32(PassiveUllTransferLimitNumericUpDown.Value)
        End SyncLock
    End Sub

    Private Sub PassiveUllReplyLimitNumericUpDown_ValueChanged(sender As System.Object, e As System.EventArgs) Handles PassiveUllReplyLimitNumericUpDown.ValueChanged
        SyncLock UiState
            UiState.PassiveUllFinishReplyLimitTicks = Decimal.ToInt32(PassiveUllReplyLimitNumericUpDown.Value)
        End SyncLock
    End Sub

    Private Sub PassivePostDataGridView_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles PassivePostDataGridView.CellMouseClick
        '左クリックの場合は、このメソッドでは処理しない。
        If e.Button <> MouseButtons.Right Then Return

        '列ヘッダを右クリックした場合は、このメソッドでは処理しない。
        If e.RowIndex = -1 Then Return

        '右クリックした場所に選択を移す。
        If e.ColumnIndex = -1 Then
            'NOTE: 行ヘッダが右クリックされた場合である。
            '当該行の１列目セルを選択しているが、これは、行ヘッダを選択しても
            '直前まで選択されていた行の妥当性チェックが実行されないことおよび、
            '直前まで選択されていた行の選択が解除されないことに対処するためである。
            PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassivePostDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '右クリックした行とは別の編集中の行が存在する場合は、メニューは出さない。
        'ただし、編集中の行が真の編集中ではない場合は、メニューを出す。
        If lastEditRow <> -1 AndAlso lastEditRow <> e.RowIndex Then
            If Not (PassivePostDataGridView.Rows(lastEditRow).IsNewRow AndAlso _
                    String.IsNullOrEmpty(CStr(PassivePostDataGridView.Rows(lastEditRow).Cells(0).Value))) Then
                Return
            End If
        End If

        If e.ColumnIndex = -1 Then
            PassivePostDataGridView.Rows(e.RowIndex).Selected = True
            Application.DoEvents()
            PassivePostRowHeaderMenu.Show(PassivePostDataGridView, PassivePostDataGridView.GetRowDisplayRectangle(e.RowIndex, False).Location + New Size(e.Location))
        End If
    End Sub

    Private Sub PassivePostDelMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PassivePostDelMenuItem.Click
        RemovePassivePostData()
    End Sub

    Private Sub PassivePostDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles PassivePostDataGridView.KeyDown
        Select Case e.KeyData
            Case Keys.Delete
                If PassivePostDataGridView.SelectedRows.Count = 1 Then
                    RemovePassivePostData()
                    e.Handled = True
                End If
        End Select
    End Sub

    Private Sub RemovePassivePostData()
        Dim selectedRow As DataGridViewRow = PassivePostDataGridView.SelectedRows(0)

        If selectedRow.IsNewRow Then
            selectedRow.Cells(0).Value = Nothing
            selectedRow.ErrorText = Nothing
            Return
        End If

        If lastEditRow <> selectedRow.Index Then
            SyncLock UiState
                UiState.SomethingForPassivePostObjCodes.Remove(Byte.Parse(CStr(selectedRow.Cells(0).Value), NumberStyles.HexNumber))
            End SyncLock
        ElseIf sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView IsNot Nothing)
            SyncLock UiState
                UiState.SomethingForPassivePostObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
            End SyncLock
        Else
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView Is Nothing)
            'NOTE: おそらく、selectedRow.IsNewRowに該当するケースであるため、
            'ここまで到達しないと思われる。たとえ到達したとしても、
            '新規の行を編集中にその行の削除を実施した場合である故、
            'Dictionaryには内容を登録していないので、Dictionaryからの削除は無用である。
        End If

        If lastEditRow = selectedRow.Index Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどで、余計なことが
            '行われないように、この時点で、編集中ではなかったことにしておく。
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどを処理する際、
            'lastEditRowが位置として参照されないことを前提に、
            '邪道ではあるが、この時点で補正を行っておく。
            'NOTE: デクリメント前の時点でlastEditRowは1以上であるため、
            'デクリメントの結果が-1やそれ以下になることはない。
            'NOTE: Rows.RemoveAt(...)によるRowValidatedイベントを処理する際、
            'lastEditRowは-1に変更される。さらに、Rows.RemoveAt(...)の結果として
            '既存の行が全て無くなれば、DefaultValuesNeededイベントが発生し、
            'lastEditRowは新規行の位置（おそらく0）に変更される。つまり、
            'この補正は、事実上不要である可能性が高い。
            lastEditRow -= 1
        End If

        PassivePostDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub PassivePostDataGridView_DefaultValuesNeeded(ByVal sender As Object, ByVal e As DataGridViewRowEventArgs) Handles PassivePostDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing

        'NOTE: DataGridViewは、新規の行（アスタリスクの行）を選択後、
        'ダブルクリックやキー入力で編集を開始した際、編集中と同じ状態
        'になる一方で、CellBeginEditイベントは発生しないようなので、
        'まだ選択されただけの段階ではあるが、CellBeginEditイベント
        '発生時と同じ処理をここで実施することにしている。
        'この処置のせいで、真に編集中でない場合（キャレットが登場
        'していない場合）でもlastEditRowは-1以外になり得るので注意。
        'lastEditRow行のIsNewRowプロパティがTrueでかつ、その全セルが
        '空の場合は、真に編集中ではないとみなすことにする。
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassivePostDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassivePostDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: 仮に新規の行を編集開始して、ここが実行される場合は、
            'Nothingを代入することになる。
            sKeyAtBeginEditRowInDataGridView = CStr(PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Value)
        End If
    End Sub

    Private Sub PassivePostDataGridView_RowValidating(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassivePostDataGridView.RowValidating
        If lastEditRow <> -1 Then
            Dim sNewKey As String = CStr(PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Value)

            If PassivePostDataGridView.Rows(e.RowIndex).IsNewRow AndAlso _
               String.IsNullOrEmpty(sNewKey) Then Return

            Dim newKey As Byte
            If Byte.TryParse(sNewKey, NumberStyles.HexNumber, Nothing, newKey) = False Then
                PassivePostDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsUnsuitableForObjCode.Gen()
                e.Cancel = True
                Return
            End If

            '新規の行を挿入した場合や、既に存在する行のキーを変更した場合は、
            '新しいキーが、他の行のキーと重複していないかチェックする。
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: このスレッド以外は、UiStateを参照するだけなので、このスレッドで
                'UiStateを参照するだけであれば、SyncLock UiStateは不要である。
                If UiState.SomethingForPassivePostObjCodes.ContainsKey(newKey) Then
                    PassivePostDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsDuplicative.Gen()
                    e.Cancel = True
                    Return
                End If
            End If
        End If
    End Sub

    Private Sub PassivePostDataGridView_RowValidated(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles PassivePostDataGridView.RowValidated
        If lastEditRow <> -1 Then
            PassivePostDataGridView.Rows(e.RowIndex).ErrorText = Nothing

            Dim sNewKey As String = CStr(PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Value)

            SyncLock UiState
                If sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
                    UiState.SomethingForPassivePostObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
                End If

                'NOTE: 以下の分岐に入らないケースは、Rows(e.RowIndex).IsNewRow が
                'True である故、DataGridView上に行は追加されておらず、Rows.RemoveAt(e.RowIndex)
                'も無用である。
                If Not String.IsNullOrEmpty(sNewKey) Then
                    Dim newKey As Byte = Byte.Parse(sNewKey, NumberStyles.HexNumber)
                    UiState.SomethingForPassivePostObjCodes.Add(newKey, Nothing)
                End If
            End SyncLock

            lastEditRow = -1
        End If
    End Sub

    Private Sub PassivePostForceReplyNakCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles PassivePostForceReplyNakCheckBox.CheckedChanged
        SyncLock UiState
            UiState.ForceReplyNakToPassivePostReq = PassivePostForceReplyNakCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassivePostNakCauseNumberTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassivePostNakCauseNumberTextBox.TextChanged
        Dim number As Integer = 0
        If Not PassivePostNakCauseNumberTextBox.Text.Equals("") Then
            number = Integer.Parse(PassivePostNakCauseNumberTextBox.Text)
        End If

        SyncLock UiState
            UiState.NakCauseNumberToPassivePostReq = number
        End SyncLock
    End Sub

    Private Sub PassivePostNakCauseTextTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassivePostNakCauseTextTextBox.TextChanged
        SyncLock UiState
            UiState.NakCauseTextToPassivePostReq = PassivePostNakCauseTextTextBox.Text
        End SyncLock
    End Sub

    Private Sub PassiveDllDataGridView_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles PassiveDllDataGridView.CellMouseClick
        '左クリックの場合は、このメソッドでは処理しない。
        If e.Button <> MouseButtons.Right Then Return

        '列ヘッダを右クリックした場合は、このメソッドでは処理しない。
        If e.RowIndex = -1 Then Return

        '右クリックした場所に選択を移す。
        If e.ColumnIndex = -1 Then
            'NOTE: 行ヘッダが右クリックされた場合である。
            '当該行の１列目セルを選択しているが、これは、行ヘッダを選択しても
            '直前まで選択されていた行の妥当性チェックが実行されないことおよび、
            '直前まで選択されていた行の選択が解除されないことに対処するためである。
            PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveDllDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '右クリックした行とは別の編集中の行が存在する場合は、メニューは出さない。
        'ただし、編集中の行が真の編集中ではない場合は、メニューを出す。
        If lastEditRow <> -1 AndAlso lastEditRow <> e.RowIndex Then
            If Not (PassiveDllDataGridView.Rows(lastEditRow).IsNewRow AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveDllDataGridView.Rows(lastEditRow).Cells(0).Value))) Then
                Return
            End If
        End If

        If e.ColumnIndex = -1 Then
            PassiveDllDataGridView.Rows(e.RowIndex).Selected = True
            Application.DoEvents()
            PassiveDllRowHeaderMenu.Show(PassiveDllDataGridView, PassiveDllDataGridView.GetRowDisplayRectangle(e.RowIndex, False).Location + New Size(e.Location))
        End If
    End Sub

    Private Sub PassiveDllDelMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PassiveDllDelMenuItem.Click
        RemovePassiveDllData()
    End Sub

    Private Sub PassiveDllDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles PassiveDllDataGridView.KeyDown
        Select Case e.KeyData
            Case Keys.Delete
                If PassiveDllDataGridView.SelectedRows.Count = 1 Then
                    RemovePassiveDllData()
                    e.Handled = True
                End If
        End Select
    End Sub

    Private Sub RemovePassiveDllData()
        Dim selectedRow As DataGridViewRow = PassiveDllDataGridView.SelectedRows(0)

        If selectedRow.IsNewRow Then
            selectedRow.Cells(0).Value = Nothing
            selectedRow.ErrorText = Nothing
            Return
        End If

        If lastEditRow <> selectedRow.Index Then
            SyncLock UiState
                UiState.SomethingForPassiveDllObjCodes.Remove(Byte.Parse(CStr(selectedRow.Cells(0).Value), NumberStyles.HexNumber))
            End SyncLock
        ElseIf sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView IsNot Nothing)
            SyncLock UiState
                UiState.SomethingForPassiveDllObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
            End SyncLock
        Else
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView Is Nothing)
            'NOTE: おそらく、selectedRow.IsNewRowに該当するケースであるため、
            'ここまで到達しないと思われる。たとえ到達したとしても、
            '新規の行を編集中にその行の削除を実施した場合である故、
            'Dictionaryには内容を登録していないので、Dictionaryからの削除は無用である。
        End If

        If lastEditRow = selectedRow.Index Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどで、余計なことが
            '行われないように、この時点で、編集中ではなかったことにしておく。
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどを処理する際、
            'lastEditRowが位置として参照されないことを前提に、
            '邪道ではあるが、この時点で補正を行っておく。
            'NOTE: デクリメント前の時点でlastEditRowは1以上であるため、
            'デクリメントの結果が-1やそれ以下になることはない。
            'NOTE: Rows.RemoveAt(...)によるRowValidatedイベントを処理する際、
            'lastEditRowは-1に変更される。さらに、Rows.RemoveAt(...)の結果として
            '既存の行が全て無くなれば、DefaultValuesNeededイベントが発生し、
            'lastEditRowは新規行の位置（おそらく0）に変更される。つまり、
            'この補正は、事実上不要である可能性が高い。
            lastEditRow -= 1
        End If

        PassiveDllDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub PassiveDllDataGridView_DefaultValuesNeeded(ByVal sender As Object, ByVal e As DataGridViewRowEventArgs) Handles PassiveDllDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing

        'NOTE: DataGridViewは、新規の行（アスタリスクの行）を選択後、
        'ダブルクリックやキー入力で編集を開始した際、編集中と同じ状態
        'になる一方で、CellBeginEditイベントは発生しないようなので、
        'まだ選択されただけの段階ではあるが、CellBeginEditイベント
        '発生時と同じ処理をここで実施することにしている。
        'この処置のせいで、真に編集中でない場合（キャレットが登場
        'していない場合）でもlastEditRowは-1以外になり得るので注意。
        'lastEditRow行のIsNewRowプロパティがTrueでかつ、その全セルが
        '空の場合は、真に編集中ではないとみなすことにする。
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveDllDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveDllDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: 仮に新規の行を編集開始して、ここが実行される場合は、
            'Nothingを代入することになる。
            sKeyAtBeginEditRowInDataGridView = CStr(PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Value)
        End If
    End Sub

    Private Sub PassiveDllDataGridView_RowValidating(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveDllDataGridView.RowValidating
        If lastEditRow <> -1 Then
            Dim sNewKey As String = CStr(PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Value)

            If PassiveDllDataGridView.Rows(e.RowIndex).IsNewRow AndAlso _
               String.IsNullOrEmpty(sNewKey) Then Return

            Dim newKey As Byte
            If Byte.TryParse(sNewKey, NumberStyles.HexNumber, Nothing, newKey) = False Then
                PassiveDllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsUnsuitableForObjCode.Gen()
                e.Cancel = True
                Return
            End If

            '新規の行を挿入した場合や、既に存在する行のキーを変更した場合は、
            '新しいキーが、他の行のキーと重複していないかチェックする。
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: このスレッド以外は、UiStateを参照するだけなので、このスレッドで
                'UiStateを参照するだけであれば、SyncLock UiStateは不要である。
                If UiState.SomethingForPassiveDllObjCodes.ContainsKey(newKey) Then
                    PassiveDllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsDuplicative.Gen()
                    e.Cancel = True
                    Return
                End If
            End If
        End If
    End Sub

    Private Sub PassiveDllDataGridView_RowValidated(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles PassiveDllDataGridView.RowValidated
        If lastEditRow <> -1 Then
            PassiveDllDataGridView.Rows(e.RowIndex).ErrorText = Nothing

            Dim sNewKey As String = CStr(PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Value)

            SyncLock UiState
                If sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
                    UiState.SomethingForPassiveDllObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
                End If

                'NOTE: 以下の分岐に入らないケースは、Rows(e.RowIndex).IsNewRow が
                'True である故、DataGridView上に行は追加されておらず、Rows.RemoveAt(e.RowIndex)
                'も無用である。
                If Not String.IsNullOrEmpty(sNewKey) Then
                    Dim newKey As Byte = Byte.Parse(sNewKey, NumberStyles.HexNumber)
                    UiState.SomethingForPassiveDllObjCodes.Add(newKey, Nothing)
                End If
            End SyncLock

            lastEditRow = -1
        End If
    End Sub

    Private Sub PassiveDllForceReplyNakCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllForceReplyNakCheckBox.CheckedChanged
        SyncLock UiState
            UiState.ForceReplyNakToPassiveDllStartReq = PassiveDllForceReplyNakCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassiveDllNakCauseNumberTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllNakCauseNumberTextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveDllNakCauseNumberTextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveDllNakCauseNumberTextBox.Text)
        End If

        SyncLock UiState
            UiState.NakCauseNumberToPassiveDllStartReq = number
        End SyncLock
    End Sub

    Private Sub PassiveDllNakCauseTextTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllNakCauseTextTextBox.TextChanged
        SyncLock UiState
            UiState.NakCauseTextToPassiveDllStartReq = PassiveDllNakCauseTextTextBox.Text
        End SyncLock
    End Sub

    Private Sub PassiveDllTransferLimitNumericUpDown_ValueChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllTransferLimitNumericUpDown.ValueChanged
        SyncLock UiState
            UiState.PassiveDllTransferLimitTicks = Decimal.ToInt32(PassiveDllTransferLimitNumericUpDown.Value)
        End SyncLock
    End Sub

    Private Sub PassiveDllReplyLimitNumericUpDown_ValueChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllReplyLimitNumericUpDown.ValueChanged
        SyncLock UiState
            UiState.PassiveDllFinishReplyLimitTicks = Decimal.ToInt32(PassiveDllReplyLimitNumericUpDown.Value)
        End SyncLock
    End Sub

    Private Sub PassiveDllSimulateStoringCheckBox_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllSimulateStoringCheckBox.CheckedChanged
        SyncLock UiState
            UiState.SimulateStoringOnPassiveDllComplete = PassiveDllSimulateStoringCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassiveDllResultantVersionOfSlot1TextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllResultantVersionOfSlot1TextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveDllResultantVersionOfSlot1TextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveDllResultantVersionOfSlot1TextBox.Text)
        End If

        SyncLock UiState
            UiState.PassiveDllResultantVersionOfSlot1 = number
        End SyncLock
    End Sub

    Private Sub PassiveDllResultantVersionOfSlot2TextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllResultantVersionOfSlot2TextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveDllResultantVersionOfSlot2TextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveDllResultantVersionOfSlot2TextBox.Text)
        End If

        SyncLock UiState
            UiState.PassiveDllResultantVersionOfSlot2 = number
        End SyncLock
    End Sub

    Private Sub PassiveDllResultantFlagOfFullTextBox_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles PassiveDllResultantFlagOfFullTextBox.KeyPress
        If (e.KeyChar < "0"c OrElse "9"c < e.KeyChar) AndAlso _
           (e.KeyChar < "A"c OrElse "F"c < e.KeyChar) AndAlso _
           (e.KeyChar < "a"c OrElse "f"c < e.KeyChar) AndAlso _
           e.KeyChar <> ControlChars.Back Then
            e.Handled = True
        End If
    End Sub

    Private Sub PassiveDllResultantFlagOfFullTextBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles PassiveDllResultantFlagOfFullTextBox.TextChanged
        Dim code As Integer
        If Integer.TryParse(PassiveDllResultantFlagOfFullTextBox.Text, NumberStyles.HexNumber, Nothing, code) = False Then
            code = &HFF
        End If

        SyncLock UiState
            UiState.PassiveDllResultantFlagOfFull = code
        End SyncLock
    End Sub

    Private Sub ScenarioFileSelButton_Click(sender As System.Object, e As System.EventArgs) Handles ScenarioFileSelButton.Click
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return
        ScenarioFileTextBox.Text = FileSelDialog.FileName
    End Sub

    Private Sub ScenarioExecButton_Click(sender As System.Object, e As System.EventArgs) Handles ScenarioExecButton.Click
        If ScenarioExecTimer.Enabled Then
            ScenarioExecTimer.Enabled = False
            TerminateScenario()
        Else
            Try
                oScenario = ScenarioReader.Read(ScenarioFileTextBox.Text)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                AlertBox.Show(Lexis.ScenarioFileIsIllegal)
                Return
            End Try

            If oScenario.Count = 0 Then
                AlertBox.Show(Lexis.ScenarioFileIsEmpty)
                Return
            End If

            If Decimal.ToInt32(ScenarioExecIntervalNumericUpDown.Value) <> 0 Then
                For i As Integer = 0 To oScenario.Count - 1
                    If Not oScenario(i).Timing.StartsWith("+") Then
                        AlertBox.Show(Lexis.DoNotRepeatScenarioThatContainsAbsoluteTiming)
                        oScenario = Nothing
                        Return
                    End If
                Next i
            End If

            nextExecIndexOfScenario = 0
            nextExecTimingOfScenario = GetAbsoluteTiming(DateTime.Now, oScenario(0).Timing)
            ScenarioExecIntervalNumericUpDown.ReadOnly = True
            ScenarioExecButton.Text = "中止"
            ScenarioExecButton.BackColor = Color.Green

            If DoScenario() = True Then
                ScenarioExecTimer.Enabled = True
            Else
                TerminateScenario()
            End If
        End If
    End Sub

    Private Sub ScenarioExecTimer_Tick(sender As System.Object, e As System.EventArgs) Handles ScenarioExecTimer.Tick
        If DoScenario() = False Then
            ScenarioExecTimer.Enabled = False
            TerminateScenario()
        End If
    End Sub

    Protected Sub TerminateScenario()
        oScenario = Nothing
        ScenarioExecIntervalNumericUpDown.ReadOnly = False
        ScenarioExecButton.Text = "実行"
        ScenarioExecButton.ResetBackColor()
    End Sub

    Protected Function DoScenario() As Boolean
        Dim execInterval As Integer = Decimal.ToInt32(ScenarioExecIntervalNumericUpDown.Value)
        While nextExecIndexOfScenario < oScenario.Count
            Dim now As DateTime = DateTime.Now
            If now.AddMilliseconds(60) >= nextExecTimingOfScenario Then
                Select Case oScenario(nextExecIndexOfScenario).Verb
                    Case ScenarioElementVerb.Connect
                        Dim lnSts As LineStatus = oTelegrapher.LineStatus
                        If lnSts = LineStatus.Initial OrElse _
                           lnSts = LineStatus.Disconnected Then
                            DoConnect()
                        End If
                    Case ScenarioElementVerb.Disconnect
                        Dim lnSts As LineStatus = oTelegrapher.LineStatus
                        If lnSts <> LineStatus.Initial AndAlso _
                           lnSts <> LineStatus.Disconnected Then
                            DoDisconnect()
                        End If
                    Case ScenarioElementVerb.ActiveOne
                        Dim oExt As New ActiveOneExecRequestExtendPart()
                        oExt.ApplyFilePath = oScenario(nextExecIndexOfScenario).Obj(0)
                        oExt.ReplyLimitTicks = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(1))
                        oExt.RetryIntervalTicks = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(2))
                        oExt.MaxRetryCountToForget = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(3))
                        oExt.MaxRetryCountToCare = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(4))
                        Log.Info("Sending ActiveOneExecRequest to the telegrapher...")
                        ActiveOneExecRequest.Gen(oExt).WriteToSocket(oChildSteerSock)
                    Case ScenarioElementVerb.ActiveUll
                        Dim oExt As New ActiveUllExecRequestExtendPart()
                        oExt.ObjCode = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(0), NumberStyles.HexNumber)
                        oExt.TransferFilePath = oScenario(nextExecIndexOfScenario).Obj(1)
                        oExt.TransferFileHashValue = oScenario(nextExecIndexOfScenario).Obj(2)
                        oExt.TransferLimitTicks = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(3))
                        oExt.ReplyLimitTicks = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(4))
                        oExt.RetryIntervalTicks = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(5))
                        oExt.MaxRetryCountToForget = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(6))
                        oExt.MaxRetryCountToCare = Integer.Parse(oScenario(nextExecIndexOfScenario).Obj(7))
                        Log.Info("Sending ActiveUllExecRequest to the telegrapher...")
                        ActiveUllExecRequest.Gen(oExt).WriteToSocket(oChildSteerSock)
                End Select

                nextExecIndexOfScenario += 1
                If nextExecIndexOfScenario < oScenario.Count Then
                    nextExecTimingOfScenario = GetAbsoluteTiming(nextExecTimingOfScenario, oScenario(nextExecIndexOfScenario).Timing)
                ElseIf execInterval <> 0 Then
                    nextExecIndexOfScenario = 0
                    nextExecTimingOfScenario = nextExecTimingOfScenario.AddMilliseconds(execInterval)
                    nextExecTimingOfScenario = GetAbsoluteTiming(nextExecTimingOfScenario, oScenario(nextExecIndexOfScenario).Timing)
                Else
                    Exit While
                End If
            Else
                Dim rate As Integer = Integer.MaxValue
                Dim span As Long = nextExecTimingOfScenario.Subtract(now).Ticks \ 10000
                If span <= Integer.MaxValue Then
                    ScenarioExecTimer.Interval = CInt(span)
                Else
                    ScenarioExecTimer.Interval = Integer.MaxValue
                End If
                Return True
            End If
        End While
        Return False
    End Function

    Protected Function DoConnect() As Boolean
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
                Return False
            End Try

            Log.Info("Sending new socket to the telegrapher...")
            oTelegrapher.LineStatus = LineStatus.Connected
            ConnectNotice.Gen(oTelegSock).WriteToSocket(oChildSteerSock)

            ConButton.Text = "切断"
            ConButton.BackColor = Color.Green
            Return True
        Finally
            ConButton.Enabled = True
        End Try
    End Function

    Protected Sub DoDisconnect()
        ConButton.Enabled = False
        Log.Info("Sending disconnect request to the telegrapher...")
        DisconnectRequest.Gen().WriteToSocket(oChildSteerSock)
    End Sub

    Protected Sub Disconnected()
        ConButton.Text = "接続"
        ConButton.ResetBackColor()
        ConButton.Enabled = True
    End Sub

    Protected Function GetAbsoluteTiming(ByVal prevTiming As DateTime, ByVal sTimingText As String) As DateTime
        If sTimingText.StartsWith("+") Then
            Return prevTiming.AddMilliseconds(Integer.Parse(sTimingText))
        Else
            Return DateTime.ParseExact(sTimingText, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None)
        End If
    End Function

End Class
