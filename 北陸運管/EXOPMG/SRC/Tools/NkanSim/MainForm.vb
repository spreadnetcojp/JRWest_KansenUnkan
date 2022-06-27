' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2014/10/22  (NES)����  EkimuSim�ɍ��킹�ă��C�A�E�g�������
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

        'Lexis ���琶�������������e�R���g���[���ɔ��f����B
        Me.Text = Lexis.FormTitle.Gen(Config.TargetEkCode.ToString(Lexis.FormTitleEkCodeFormat.Gen()))

        'UiState�̒l���e�R���g���[���ɔ��f����B
        AutomaticComStartCheckBox.Checked = UiState.AutomaticComStart
        CapSndTelegsCheckBox.Checked = UiState.CapSndTelegs
        CapRcvTelegsCheckBox.Checked = UiState.CapRcvTelegs
        CapSndFilesCheckBox.Checked = UiState.CapSndFiles
        CapRcvFilesCheckBox.Checked = UiState.CapRcvFiles

        'TODO: ���ɑ��݂���UiStateClass�̃����o�ɂ��āA
        '�R���g���[����p�ӂ�����A�������Y�ꂸ�Ɏ������邱�ƁB

        Me.ResumeLayout() '----------------------------------------------------

        Dim sCapDirPath As String = Path.Combine(sWorkingDir, "CAP")

        '����M�����f�B���N�g���ɂ��āA������΍쐬���Ă����B
        Directory.CreateDirectory(sCapDirPath)

        '�d������M�X���b�h���쐬����B
        Dim oMessageSockForTelegrapher As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oChildSteerSock, oMessageSockForTelegrapher)
        oTelegrapher = New MyTelegrapher("ToOpmg", oMessageSockForTelegrapher, sCapDirPath, Me)

        '�d������M�X���b�h���J�n����B
        Log.Info("Starting the telegrapher...")
        oTelegrapher.Start()

        LineStatusPollTimer.Start()
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        LineStatusPollTimer.Stop()

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

        LoggerTextBox.AppendText("[" & sDateTime & "] [" & sSecondName & "] " & sText & vbCrLf)
    End Sub

    Private Sub ConButton_Click(sender As System.Object, e As System.EventArgs) Handles ConButton.Click
        Dim lineStatus As LineStatus = oTelegrapher.LineStatus
        If lineStatus = LineStatus.Initial OrElse _
           lineStatus = LineStatus.Disconnected Then
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

                ConButton.Text = "�ؒf"
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
            ConButton.Text = "�ڑ�"
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
                ActiveOneExecRateNumericUpDown.Enabled = True
            Else
                ActiveOneExecTimer.Interval = rate
                ActiveOneExecTimer.Enabled = True
                ActiveOneExecButton.Text = "���~"
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
