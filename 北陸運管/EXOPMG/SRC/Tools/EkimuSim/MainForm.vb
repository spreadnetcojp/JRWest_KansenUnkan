' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2014/10/22  (NES)����  DataGridView�S�ʂ̋���������
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

'TODO: ����̎�ނ̓d���������I�ɑ��M�ł���悤�ɂ���ɂ́A
'ActiveOne�Ɠ��@�\�̃^�u���������p�ӂ���΂悢�B

'TODO: ����̎�ނ̃t�@�C���������I�ɔ\�����M�ł���悤�ɂ���ɂ́A
'ActiveUll�Ɠ��@�\�̃^�u���������p�ӂ���΂悢�B

'TODO: ��M�d���ɂ���āANAK�̗v�ۂ��ނ�ς����肷��ɂ́A
'PassiveGet��PassivePost�Ɠ��@�\�̃^�u���������p�ӂ���΂悢�B

'TODO: �w�肳�ꂽ�t�@�C���̎�ʂɂ���āANAK�̗v�ۂ��ނ�ς����肷��ɂ́A
'PassiveUll��PassiveDll�Ɠ��@�\�̃^�u���������p�ӂ���΂悢�B

Public Class MainForm
    Protected OptionalWriter As LogToOptionalDelegate

    'NOTE: UiState�̃����o�͓d������M�X���b�h�ł��Q�Ɖ\�Ƃ���B
    '���̍ۂ́ASyncLock UiState������ԂŃf�B�[�v�R�s�[���s�����ƁB
    '�܂��ASyncLock UiState���Ă���ԁA���O�o�͂Ȃǃ��C���X���b�h��
    '�҂��ƂɂȂ蓾�鏈���͍s���Ă͂Ȃ�Ȃ��B
    'NOTE: ���C���X���b�h�́A�Y������R���g���[���̏�Ԃ��ω�������
    '�Ȃǂɂ����āASyncLock UiState������Ԃł����ɒl��ݒ肷��B
    '���̊ԁAoChildSteerSock�ւ̏������݂�oChildSteerSock�����
    '��M�҂��ȂǁA�d������M�X���b�h��҂��ƂɂȂ蓾�鏈����
    '�s���Ă͂Ȃ�Ȃ��B
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

        'Lexis ���琶�������������e�R���g���[���ɔ��f����B

        Me.Text = Lexis.FormTitle.Gen(Config.SelfEkCode.ToString(Lexis.FormTitleEkCodeFormat.Gen()))

        'UiState�̒l���e�R���g���[���ɔ��f����B

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

        'FTP�̈ꎞ��Ɨp�f�B���N�g�����폜����B
        Log.Info("Sweeping directory [" & sFtpBasePath & "]...")
        Utility.DeleteTemporalDirectory(sFtpBasePath)

        '����M�����f�B���N�g���ɂ��āA������΍쐬���Ă����B
        Directory.CreateDirectory(sCapDirPath)

        '�d�������I�u�W�F�N�g���쐬����B
        oTelegGene = New EkTelegramGeneForNativeModels(sFtpBasePath)

        '�d������M�X���b�h���쐬����B
        Dim oMessageSockForTelegrapher As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oChildSteerSock, oMessageSockForTelegrapher)
        oTelegrapher = New MyTelegrapher("ToOpmg", oMessageSockForTelegrapher, oTelegGene, sFtpBasePath, sCapDirPath, Me)

        '�d������M�X���b�h���J�n����B
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
                '�d������M�X���b�h�ɏI���v���𑗐M����B
                Log.Info("Sending quit request to the telegrapher...")
                If QuitRequest.Gen().WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
                    Log.Fatal("The telegrapher seems broken.")
                End If

                '�d������M�X���b�h�̏I����҂B
                Log.Info("Waiting for the telegrapher to quit...")
                If oTelegrapher.Join(Config.TelegrapherPendingLimitTicks) = False Then
                    Log.Fatal("The telegrapher seems broken.")
                    oTelegrapher.Abort()
                End If
            End If
            oChildSteerSock.Close()
        End If

        If UiState IsNot Nothing Then
            'NOTE: ���̃P�[�X�ł́A�E�ӂ̊e�R���g���[���ɁA���Ȃ��Ƃ��N�����̃t�@�C������
            '���[�h�����l�̓Z�b�g�ς݂̑z��ł���B

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
        'TODO: ����ɂ�蔭������C�x���g�̂��߂̃n���h����p�ӂ���B
        Application.OnThreadException(ex)
    End Sub

    'NOTE: ���O�o�͖��ɌĂ΂��̂ŁA����̒��Ń��O���o�͂��Ă͂Ȃ�Ȃ��B
    Protected Sub BeginFetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)
        Try
            'OPT: ��L����������̓f�b�h���b�N���Ȃ��Ǝv����̂ŁA
            'BeginInvoke()�ł͂Ȃ��AInvoke()�ł��悢��������Ȃ��B
            'Invoke()�Ȃ�΁A���b�Z�[�W�L���[�����ӂ��S�z���Ȃ��B
            BeginInvoke( _
                OptionalWriter, _
                New Object() {number, sSecondName, sDateTime, sKind, sClassName, sMethodName, sText})
        Catch ex As Exception
            'NOTE: ����Control���j�����ꂽ��ɂ��̃��\�b�h���Ăяo����閜����̏ꍇ��z�肵�Ă���B
            '���̌�́i���̃f���Q�[�g�Ɉˑ����Ȃ��j������ʏ�ʂ�s���悤�A��O�͈���Ԃ��B
        End Try
    End Sub

    'NOTE: ���O�o�͖��ɌĂ΂��̂ŁA����̒��Ń��O���o�͂��Ă͂Ȃ�Ȃ��B
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
            'NOTE: ���b�Z�[�W���̂������I�ɑ��M�ł��邽�߁A�d�����M�̃��g���C�w���
            '�s�v�Ƃ��AReplyLimitTicks�ɂ��Ă̂݁A�w��\�ɂ��Ă���B
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
                ActiveOneExecButton.Text = "���s"
                ActiveOneExecButton.ResetBackColor()
                ActiveOneExecIntervalNumericUpDown.Enabled = True
            Else
                ActiveOneExecTimer.Interval = rate
                ActiveOneExecTimer.Enabled = True
                ActiveOneExecButton.Text = "���~"
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
            'NOTE: ���b�Z�[�W���̂������I�ɑ��M�ł��邽�߁A�d�����M�̃��g���C�w���
            '�s�v�Ƃ��AReplyLimitTicks�ɂ��Ă̂݁A�w��\�ɂ��Ă���B
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
                ActiveUllExecButton.Text = "���s"
                ActiveUllExecButton.ResetBackColor()
                ActiveUllExecIntervalNumericUpDown.Enabled = True
                ActiveUllObjCodeTextBox.Enabled = True
            Else
                ActiveUllExecTimer.Interval = rate
                ActiveUllExecTimer.Enabled = True
                ActiveUllExecButton.Text = "���~"
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

    'NOTE: lastEditRow�͕ҏW���̍s�ԍ��B�ҏW���łȂ��ꍇ��-1�Ƃ���B
    'NOTE: sKeyAtBeginEditRowInDataGridView�͕ҏW���̍s�́A�ҏW�J�n���̃L�[�l�B
    'lastEditRow��-1�ȊO�̏ꍇ�̂ݗL�ӂł���B�V�K�̍s��ҏW����Nothing�Ƃ���B
    Private lastEditRow As Integer = -1
    Private sKeyAtBeginEditRowInDataGridView As String

    Private Sub PassiveGetDataGridView_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles PassiveGetDataGridView.CellMouseClick
        '���N���b�N�̏ꍇ�́A���̃��\�b�h�ł͏������Ȃ��B
        If e.Button <> MouseButtons.Right Then Return

        '��w�b�_���E�N���b�N�����ꍇ�́A���̃��\�b�h�ł͏������Ȃ��B
        If e.RowIndex = -1 Then Return

        '�E�N���b�N�����ꏊ�ɑI�����ڂ��B
        If e.ColumnIndex = -1 Then
            'NOTE: �s�w�b�_���E�N���b�N���ꂽ�ꍇ�ł���B
            '���Y�s�̂P��ڃZ����I�����Ă��邪�A����́A�s�w�b�_��I�����Ă�
            '���O�܂őI������Ă����s�̑Ó����`�F�b�N�����s����Ȃ����Ƃ���сA
            '���O�܂őI������Ă����s�̑I������������Ȃ����ƂɑΏ����邽�߂ł���B
            PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveGetDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '�E�N���b�N�����s�Ƃ͕ʂ̕ҏW���̍s�����݂���ꍇ�́A���j���[�͏o���Ȃ��B
        '�������A�ҏW���̍s���^�̕ҏW���ł͂Ȃ��ꍇ�́A���j���[���o���B
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
            'NOTE: �����炭�AselectedRow.IsNewRow�ɊY������P�[�X�ł��邽�߁A
            '�����܂œ��B���Ȃ��Ǝv����B���Ƃ����B�����Ƃ��Ă��A
            '�V�K�̍s��ҏW���ɂ��̍s�̍폜�����{�����ꍇ�ł���́A
            'Dictionary�ɂ͓��e��o�^���Ă��Ȃ��̂ŁADictionary����̍폜�͖��p�ł���B
        End If

        If lastEditRow = selectedRow.Index Then
            '���̌��Rows.RemoveAt(...)�ɂ��RowValidating�C�x���g�ȂǂŁA�]�v�Ȃ��Ƃ�
            '�s���Ȃ��悤�ɁA���̎��_�ŁA�ҏW���ł͂Ȃ��������Ƃɂ��Ă����B
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            '���̌��Rows.RemoveAt(...)�ɂ��RowValidating�C�x���g�Ȃǂ���������ہA
            'lastEditRow���ʒu�Ƃ��ĎQ�Ƃ���Ȃ����Ƃ�O��ɁA
            '�ד��ł͂��邪�A���̎��_�ŕ␳���s���Ă����B
            'NOTE: �f�N�������g�O�̎��_��lastEditRow��1�ȏ�ł��邽�߁A
            '�f�N�������g�̌��ʂ�-1�₻��ȉ��ɂȂ邱�Ƃ͂Ȃ��B
            'NOTE: Rows.RemoveAt(...)�ɂ��RowValidated�C�x���g����������ہA
            'lastEditRow��-1�ɕύX�����B����ɁARows.RemoveAt(...)�̌��ʂƂ���
            '�����̍s���S�Ė����Ȃ�΁ADefaultValuesNeeded�C�x���g���������A
            'lastEditRow�͐V�K�s�̈ʒu�i�����炭0�j�ɕύX�����B�܂�A
            '���̕␳�́A������s�v�ł���\���������B
            lastEditRow -= 1
        End If

        PassiveGetDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub SelectPassiveGetDataApplyFile()
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return

        Dim selectedRow As DataGridViewRow = PassiveGetDataGridView.Rows(PassiveGetDataGridView.SelectedCells(0).RowIndex)

        'NOTE: �ҏW���̍s��V�K�̍s�ɑ΂��āA�t�@�C�����̑I�������{�����ꍇ�A
        '���̏�ł�UiState.ApplyFileForPassiveGetObjCodes�ւ̔��f��
        '���p�ł���i�ҏW���m�肵�����_�Ŏ��{�����͂��ł���j��A
        'sKey��Nothing�̉\��������B
        '���̂��Ƃ���AUiState.ApplyFileForPassiveGetObjCodes�ւ̔��f�ɂ�
        '������݂��Ă���B
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

        'NOTE: DataGridView�́A�V�K�̍s�i�A�X�^���X�N�̍s�j��I����A
        '�_�u���N���b�N��L�[���͂ŕҏW���J�n�����ہA�ҏW���Ɠ������
        '�ɂȂ����ŁACellBeginEdit�C�x���g�͔������Ȃ��悤�Ȃ̂ŁA
        '�܂��I�����ꂽ�����̒i�K�ł͂��邪�ACellBeginEdit�C�x���g
        '�������Ɠ��������������Ŏ��{���邱�Ƃɂ��Ă���B
        '���̏��u�̂����ŁA�^�ɕҏW���łȂ��ꍇ�i�L�����b�g���o��
        '���Ă��Ȃ��ꍇ�j�ł�lastEditRow��-1�ȊO�ɂȂ蓾��̂Œ��ӁB
        'lastEditRow�s��IsNewRow�v���p�e�B��True�ł��A���̑S�Z����
        '��̏ꍇ�́A�^�ɕҏW���ł͂Ȃ��Ƃ݂Ȃ����Ƃɂ���B
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveGetDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveGetDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: ���ɐV�K�̍s��ҏW�J�n���āA���������s�����ꍇ�́A
            'Nothing�������邱�ƂɂȂ�B
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

            '�V�K�̍s��}�������ꍇ��A���ɑ��݂���s�̃L�[��ύX�����ꍇ�́A
            '�V�����L�[���A���̍s�̃L�[�Əd�����Ă��Ȃ����`�F�b�N����B
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: ���̃X���b�h�ȊO�́AUiState���Q�Ƃ��邾���Ȃ̂ŁA���̃X���b�h��
                'UiState���Q�Ƃ��邾���ł���΁ASyncLock UiState�͕s�v�ł���B
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

                'NOTE: �ȉ��̕���ɓ���Ȃ��P�[�X�́ARowValidating�œ��ʈ��������P�[�X�ł���́A
                'sNewApplyFile���m���ɋ�ł���B�܂��A���̃P�[�X�́ARows(e.RowIndex).IsNewRow ��
                'True �ł���́ADataGridView��ɍs�͒ǉ�����Ă��炸�ARows.RemoveAt(e.RowIndex)
                '�����p�ł���B
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
        '���N���b�N�̏ꍇ�́A���̃��\�b�h�ł͏������Ȃ��B
        If e.Button <> MouseButtons.Right Then Return

        '��w�b�_���E�N���b�N�����ꍇ�́A���̃��\�b�h�ł͏������Ȃ��B
        If e.RowIndex = -1 Then Return

        '�E�N���b�N�����ꏊ�ɑI�����ڂ��B
        If e.ColumnIndex = -1 Then
            'NOTE: �s�w�b�_���E�N���b�N���ꂽ�ꍇ�ł���B
            '���Y�s�̂P��ڃZ����I�����Ă��邪�A����́A�s�w�b�_��I�����Ă�
            '���O�܂őI������Ă����s�̑Ó����`�F�b�N�����s����Ȃ����Ƃ���сA
            '���O�܂őI������Ă����s�̑I������������Ȃ����ƂɑΏ����邽�߂ł���B
            PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveUllDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '�E�N���b�N�����s�Ƃ͕ʂ̕ҏW���̍s�����݂���ꍇ�́A���j���[�͏o���Ȃ��B
        '�������A�ҏW���̍s���^�̕ҏW���ł͂Ȃ��ꍇ�́A���j���[���o���B
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
            'NOTE: �����炭�AselectedRow.IsNewRow�ɊY������P�[�X�ł��邽�߁A
            '�����܂œ��B���Ȃ��Ǝv����B���Ƃ����B�����Ƃ��Ă��A
            '�V�K�̍s��ҏW���ɂ��̍s�̍폜�����{�����ꍇ�ł���́A
            'Dictionary�ɂ͓��e��o�^���Ă��Ȃ��̂ŁADictionary����̍폜�͖��p�ł���B
        End If

        If lastEditRow = selectedRow.Index Then
            '���̌��Rows.RemoveAt(...)�ɂ��RowValidating�C�x���g�ȂǂŁA�]�v�Ȃ��Ƃ�
            '�s���Ȃ��悤�ɁA���̎��_�ŁA�ҏW���ł͂Ȃ��������Ƃɂ��Ă����B
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            '���̌��Rows.RemoveAt(...)�ɂ��RowValidating�C�x���g�Ȃǂ���������ہA
            'lastEditRow���ʒu�Ƃ��ĎQ�Ƃ���Ȃ����Ƃ�O��ɁA
            '�ד��ł͂��邪�A���̎��_�ŕ␳���s���Ă����B
            'NOTE: �f�N�������g�O�̎��_��lastEditRow��1�ȏ�ł��邽�߁A
            '�f�N�������g�̌��ʂ�-1�₻��ȉ��ɂȂ邱�Ƃ͂Ȃ��B
            'NOTE: Rows.RemoveAt(...)�ɂ��RowValidated�C�x���g����������ہA
            'lastEditRow��-1�ɕύX�����B����ɁARows.RemoveAt(...)�̌��ʂƂ���
            '�����̍s���S�Ė����Ȃ�΁ADefaultValuesNeeded�C�x���g���������A
            'lastEditRow�͐V�K�s�̈ʒu�i�����炭0�j�ɕύX�����B�܂�A
            '���̕␳�́A������s�v�ł���\���������B
            lastEditRow -= 1
        End If

        PassiveUllDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub SelectPassiveUllDataApplyFile()
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return

        Dim selectedRow As DataGridViewRow = PassiveUllDataGridView.Rows(PassiveUllDataGridView.SelectedCells(0).RowIndex)

        'NOTE: �ҏW���̍s��V�K�̍s�ɑ΂��āA�t�@�C�����̑I�������{�����ꍇ�A
        '���̏�ł�UiState.ApplyFileForPassiveUllObjCodes�ւ̔��f��
        '���p�ł���i�ҏW���m�肵�����_�Ŏ��{�����͂��ł���j��A
        'sKey��Nothing�̉\��������B
        '���̂��Ƃ���AUiState.ApplyFileForPassiveUllObjCodes�ւ̔��f�ɂ�
        '������݂��Ă���B
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

        'NOTE: DataGridView�́A�V�K�̍s�i�A�X�^���X�N�̍s�j��I����A
        '�_�u���N���b�N��L�[���͂ŕҏW���J�n�����ہA�ҏW���Ɠ������
        '�ɂȂ����ŁACellBeginEdit�C�x���g�͔������Ȃ��悤�Ȃ̂ŁA
        '�܂��I�����ꂽ�����̒i�K�ł͂��邪�ACellBeginEdit�C�x���g
        '�������Ɠ��������������Ŏ��{���邱�Ƃɂ��Ă���B
        '���̏��u�̂����ŁA�^�ɕҏW���łȂ��ꍇ�i�L�����b�g���o��
        '���Ă��Ȃ��ꍇ�j�ł�lastEditRow��-1�ȊO�ɂȂ蓾��̂Œ��ӁB
        'lastEditRow�s��IsNewRow�v���p�e�B��True�ł��A���̑S�Z����
        '��̏ꍇ�́A�^�ɕҏW���ł͂Ȃ��Ƃ݂Ȃ����Ƃɂ���B
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveUllDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveUllDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: ���ɐV�K�̍s��ҏW�J�n���āA���������s�����ꍇ�́A
            'Nothing�������邱�ƂɂȂ�B
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

            '�V�K�̍s��}�������ꍇ��A���ɑ��݂���s�̃L�[��ύX�����ꍇ�́A
            '�V�����L�[���A���̍s�̃L�[�Əd�����Ă��Ȃ����`�F�b�N����B
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: ���̃X���b�h�ȊO�́AUiState���Q�Ƃ��邾���Ȃ̂ŁA���̃X���b�h��
                'UiState���Q�Ƃ��邾���ł���΁ASyncLock UiState�͕s�v�ł���B
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

                'NOTE: �ȉ��̕���ɓ���Ȃ��P�[�X�́ARowValidating�œ��ʈ��������P�[�X�ł���́A
                'sNewApplyFile���m���ɋ�ł���B�܂��A���̃P�[�X�́ARows(e.RowIndex).IsNewRow ��
                'True �ł���́ADataGridView��ɍs�͒ǉ�����Ă��炸�ARows.RemoveAt(e.RowIndex)
                '�����p�ł���B
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
        '���N���b�N�̏ꍇ�́A���̃��\�b�h�ł͏������Ȃ��B
        If e.Button <> MouseButtons.Right Then Return

        '��w�b�_���E�N���b�N�����ꍇ�́A���̃��\�b�h�ł͏������Ȃ��B
        If e.RowIndex = -1 Then Return

        '�E�N���b�N�����ꏊ�ɑI�����ڂ��B
        If e.ColumnIndex = -1 Then
            'NOTE: �s�w�b�_���E�N���b�N���ꂽ�ꍇ�ł���B
            '���Y�s�̂P��ڃZ����I�����Ă��邪�A����́A�s�w�b�_��I�����Ă�
            '���O�܂őI������Ă����s�̑Ó����`�F�b�N�����s����Ȃ����Ƃ���сA
            '���O�܂őI������Ă����s�̑I������������Ȃ����ƂɑΏ����邽�߂ł���B
            PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassivePostDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '�E�N���b�N�����s�Ƃ͕ʂ̕ҏW���̍s�����݂���ꍇ�́A���j���[�͏o���Ȃ��B
        '�������A�ҏW���̍s���^�̕ҏW���ł͂Ȃ��ꍇ�́A���j���[���o���B
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
            'NOTE: �����炭�AselectedRow.IsNewRow�ɊY������P�[�X�ł��邽�߁A
            '�����܂œ��B���Ȃ��Ǝv����B���Ƃ����B�����Ƃ��Ă��A
            '�V�K�̍s��ҏW���ɂ��̍s�̍폜�����{�����ꍇ�ł���́A
            'Dictionary�ɂ͓��e��o�^���Ă��Ȃ��̂ŁADictionary����̍폜�͖��p�ł���B
        End If

        If lastEditRow = selectedRow.Index Then
            '���̌��Rows.RemoveAt(...)�ɂ��RowValidating�C�x���g�ȂǂŁA�]�v�Ȃ��Ƃ�
            '�s���Ȃ��悤�ɁA���̎��_�ŁA�ҏW���ł͂Ȃ��������Ƃɂ��Ă����B
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            '���̌��Rows.RemoveAt(...)�ɂ��RowValidating�C�x���g�Ȃǂ���������ہA
            'lastEditRow���ʒu�Ƃ��ĎQ�Ƃ���Ȃ����Ƃ�O��ɁA
            '�ד��ł͂��邪�A���̎��_�ŕ␳���s���Ă����B
            'NOTE: �f�N�������g�O�̎��_��lastEditRow��1�ȏ�ł��邽�߁A
            '�f�N�������g�̌��ʂ�-1�₻��ȉ��ɂȂ邱�Ƃ͂Ȃ��B
            'NOTE: Rows.RemoveAt(...)�ɂ��RowValidated�C�x���g����������ہA
            'lastEditRow��-1�ɕύX�����B����ɁARows.RemoveAt(...)�̌��ʂƂ���
            '�����̍s���S�Ė����Ȃ�΁ADefaultValuesNeeded�C�x���g���������A
            'lastEditRow�͐V�K�s�̈ʒu�i�����炭0�j�ɕύX�����B�܂�A
            '���̕␳�́A������s�v�ł���\���������B
            lastEditRow -= 1
        End If

        PassivePostDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub PassivePostDataGridView_DefaultValuesNeeded(ByVal sender As Object, ByVal e As DataGridViewRowEventArgs) Handles PassivePostDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing

        'NOTE: DataGridView�́A�V�K�̍s�i�A�X�^���X�N�̍s�j��I����A
        '�_�u���N���b�N��L�[���͂ŕҏW���J�n�����ہA�ҏW���Ɠ������
        '�ɂȂ����ŁACellBeginEdit�C�x���g�͔������Ȃ��悤�Ȃ̂ŁA
        '�܂��I�����ꂽ�����̒i�K�ł͂��邪�ACellBeginEdit�C�x���g
        '�������Ɠ��������������Ŏ��{���邱�Ƃɂ��Ă���B
        '���̏��u�̂����ŁA�^�ɕҏW���łȂ��ꍇ�i�L�����b�g���o��
        '���Ă��Ȃ��ꍇ�j�ł�lastEditRow��-1�ȊO�ɂȂ蓾��̂Œ��ӁB
        'lastEditRow�s��IsNewRow�v���p�e�B��True�ł��A���̑S�Z����
        '��̏ꍇ�́A�^�ɕҏW���ł͂Ȃ��Ƃ݂Ȃ����Ƃɂ���B
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassivePostDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassivePostDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: ���ɐV�K�̍s��ҏW�J�n���āA���������s�����ꍇ�́A
            'Nothing�������邱�ƂɂȂ�B
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

            '�V�K�̍s��}�������ꍇ��A���ɑ��݂���s�̃L�[��ύX�����ꍇ�́A
            '�V�����L�[���A���̍s�̃L�[�Əd�����Ă��Ȃ����`�F�b�N����B
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: ���̃X���b�h�ȊO�́AUiState���Q�Ƃ��邾���Ȃ̂ŁA���̃X���b�h��
                'UiState���Q�Ƃ��邾���ł���΁ASyncLock UiState�͕s�v�ł���B
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

                'NOTE: �ȉ��̕���ɓ���Ȃ��P�[�X�́ARows(e.RowIndex).IsNewRow ��
                'True �ł���́ADataGridView��ɍs�͒ǉ�����Ă��炸�ARows.RemoveAt(e.RowIndex)
                '�����p�ł���B
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
        '���N���b�N�̏ꍇ�́A���̃��\�b�h�ł͏������Ȃ��B
        If e.Button <> MouseButtons.Right Then Return

        '��w�b�_���E�N���b�N�����ꍇ�́A���̃��\�b�h�ł͏������Ȃ��B
        If e.RowIndex = -1 Then Return

        '�E�N���b�N�����ꏊ�ɑI�����ڂ��B
        If e.ColumnIndex = -1 Then
            'NOTE: �s�w�b�_���E�N���b�N���ꂽ�ꍇ�ł���B
            '���Y�s�̂P��ڃZ����I�����Ă��邪�A����́A�s�w�b�_��I�����Ă�
            '���O�܂őI������Ă����s�̑Ó����`�F�b�N�����s����Ȃ����Ƃ���сA
            '���O�܂őI������Ă����s�̑I������������Ȃ����ƂɑΏ����邽�߂ł���B
            PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveDllDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '�E�N���b�N�����s�Ƃ͕ʂ̕ҏW���̍s�����݂���ꍇ�́A���j���[�͏o���Ȃ��B
        '�������A�ҏW���̍s���^�̕ҏW���ł͂Ȃ��ꍇ�́A���j���[���o���B
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
            'NOTE: �����炭�AselectedRow.IsNewRow�ɊY������P�[�X�ł��邽�߁A
            '�����܂œ��B���Ȃ��Ǝv����B���Ƃ����B�����Ƃ��Ă��A
            '�V�K�̍s��ҏW���ɂ��̍s�̍폜�����{�����ꍇ�ł���́A
            'Dictionary�ɂ͓��e��o�^���Ă��Ȃ��̂ŁADictionary����̍폜�͖��p�ł���B
        End If

        If lastEditRow = selectedRow.Index Then
            '���̌��Rows.RemoveAt(...)�ɂ��RowValidating�C�x���g�ȂǂŁA�]�v�Ȃ��Ƃ�
            '�s���Ȃ��悤�ɁA���̎��_�ŁA�ҏW���ł͂Ȃ��������Ƃɂ��Ă����B
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            '���̌��Rows.RemoveAt(...)�ɂ��RowValidating�C�x���g�Ȃǂ���������ہA
            'lastEditRow���ʒu�Ƃ��ĎQ�Ƃ���Ȃ����Ƃ�O��ɁA
            '�ד��ł͂��邪�A���̎��_�ŕ␳���s���Ă����B
            'NOTE: �f�N�������g�O�̎��_��lastEditRow��1�ȏ�ł��邽�߁A
            '�f�N�������g�̌��ʂ�-1�₻��ȉ��ɂȂ邱�Ƃ͂Ȃ��B
            'NOTE: Rows.RemoveAt(...)�ɂ��RowValidated�C�x���g����������ہA
            'lastEditRow��-1�ɕύX�����B����ɁARows.RemoveAt(...)�̌��ʂƂ���
            '�����̍s���S�Ė����Ȃ�΁ADefaultValuesNeeded�C�x���g���������A
            'lastEditRow�͐V�K�s�̈ʒu�i�����炭0�j�ɕύX�����B�܂�A
            '���̕␳�́A������s�v�ł���\���������B
            lastEditRow -= 1
        End If

        PassiveDllDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub PassiveDllDataGridView_DefaultValuesNeeded(ByVal sender As Object, ByVal e As DataGridViewRowEventArgs) Handles PassiveDllDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing

        'NOTE: DataGridView�́A�V�K�̍s�i�A�X�^���X�N�̍s�j��I����A
        '�_�u���N���b�N��L�[���͂ŕҏW���J�n�����ہA�ҏW���Ɠ������
        '�ɂȂ����ŁACellBeginEdit�C�x���g�͔������Ȃ��悤�Ȃ̂ŁA
        '�܂��I�����ꂽ�����̒i�K�ł͂��邪�ACellBeginEdit�C�x���g
        '�������Ɠ��������������Ŏ��{���邱�Ƃɂ��Ă���B
        '���̏��u�̂����ŁA�^�ɕҏW���łȂ��ꍇ�i�L�����b�g���o��
        '���Ă��Ȃ��ꍇ�j�ł�lastEditRow��-1�ȊO�ɂȂ蓾��̂Œ��ӁB
        'lastEditRow�s��IsNewRow�v���p�e�B��True�ł��A���̑S�Z����
        '��̏ꍇ�́A�^�ɕҏW���ł͂Ȃ��Ƃ݂Ȃ����Ƃɂ���B
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveDllDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveDllDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: ���ɐV�K�̍s��ҏW�J�n���āA���������s�����ꍇ�́A
            'Nothing�������邱�ƂɂȂ�B
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

            '�V�K�̍s��}�������ꍇ��A���ɑ��݂���s�̃L�[��ύX�����ꍇ�́A
            '�V�����L�[���A���̍s�̃L�[�Əd�����Ă��Ȃ����`�F�b�N����B
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: ���̃X���b�h�ȊO�́AUiState���Q�Ƃ��邾���Ȃ̂ŁA���̃X���b�h��
                'UiState���Q�Ƃ��邾���ł���΁ASyncLock UiState�͕s�v�ł���B
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

                'NOTE: �ȉ��̕���ɓ���Ȃ��P�[�X�́ARows(e.RowIndex).IsNewRow ��
                'True �ł���́ADataGridView��ɍs�͒ǉ�����Ă��炸�ARows.RemoveAt(e.RowIndex)
                '�����p�ł���B
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
            ScenarioExecButton.Text = "���~"
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
        ScenarioExecButton.Text = "���s"
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
        'OPT: Connect()�̊Ԃ́A�ڑ��{�^���͉����s�ɂ���ׂ��ł��邪�A
        '�E�B���h�E�̈ړ���A�v���I���A���̑�UI�̍X�V�͂ł�������悢�B
        '�܂�ABeginConnect()����p���Ď�������ׂ��ł���B
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

            ConButton.Text = "�ؒf"
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
        ConButton.Text = "�ڑ�"
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
