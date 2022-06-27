' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �m�ԂƂ��ĉ^�ǃT�[�o�Ɠd���̑���M���s���N���X�B
''' </summary>
Public Class MyTelegrapher
    Inherits ClientTelegrapher

#Region "�����N���X��"
    Delegate Sub RethrowExceptionDelegate(ByVal ex As Exception)
#End Region

#Region "�萔��ϐ�"
    '�X���b�h�ʃf�B���N�g�����̏���
    Protected Const sDirNameFormat As String = "%3R%3S_%4C_%2U"

    '���C���t�H�[���ւ̎Q��
    'NOTE: �˗����ꂽ�ʐM�̌��ʂ�UI�ɔ��f����ۂ́A
    '���̃t�H�[����BeginInvoke���\�b�h�ɂ��A
    '���b�Z�[�W���[�v�ŔC�ӂ̃��\�b�h�����s������B
    Protected oForm As MainForm

    '����M�����f�B���N�g��
    Protected sCapDirPath As String

    '�d���̌������p�����ׂ����ۂ�
    Protected enableCommunication As Boolean

    'NOTE: �u�Ӑ}�I�Ȑؒf�v�Ɓu�ُ�ɂ��ؒf�v����ʂ������Ȃ�΁A
    'Protected needConnection As Boolean��p�ӂ��A
    'ProcOnConnectNoticeReceive()��ProcOnDisconnectRequestReceive()���t�b�N����
    '�����ON/OFF����Ƃ悢�BProcOnConnectionDisappear()�ł́A������݂āA
    '�J�ڐ�̉����Ԃ����߂邱�Ƃ��ł���B

    '������
    Private _LineStatus As Integer
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal sCapDirPath As String, ByVal oForm As MainForm)
        MyBase.New(sThreadName, oParentMessageSock, New NkTelegramImporter())
        Me.sCapDirPath = sCapDirPath
        Me.oForm = oForm
        Me.enableCommunication = False
        Me.LineStatus = LineStatus.Initial

        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite
        Me.enableWatchdog = False
        Me.enableActiveOneOrdering = True
    End Sub
#End Region

#Region "�v���p�e�B"
    'NOTE: ���̃v���p�e�B�́A�e�X���b�h�ɂ����Ă��Q�Ƃ�ύX���s����B
    'Initial��Disconnected�̏ꍇ�́A�e�X���b�h�ɕύX�̌���������A
    '�e�X���b�h��Connected�ɕύX������B
    'Connected��Steady�̏ꍇ�́A�q�X���b�h�ɕύX�̌���������A
    '�q�X���b�h��Steady��Disconnected�ɕύX������B
    Public Property LineStatus() As LineStatus
        'NOTE: Interlocked�N���X��Read���\�b�h�Ɋւ���msdn�̉����ǂނƁA
        '32�r�b�g�ϐ�����̒l�̓ǂݎ���Interlocked�N���X�̃��\�b�h���g��
        '�܂ł��Ȃ��s���ł���i�S�̂�ǂݎ�邽�߂̃o�X�I�y���[�V�������A
        '���̃R�A�ɂ��o�X�I�y���[�V�����ɕ��f����邱�Ƃ��Ȃ��j���Ƃ�
        '�ۏ؂���Ă���悤�ɂ������A���ۂ�Integer�������Ƃ���Read���\�b�h��
        '�p�ӂ���Ă��Ȃ��B�����ł́A�Ƃ肠����Interlocked.Add�iLOCK: XADD?�j
        '���p���Ă��邪�A��ʓI�ɍl���āAInterlocked�N���X��
        '�uRead�������o���A+�P�Ƃ�32bit���[�h���߁v�Ŏ������ꂽ�i�����I��
        'VolatileRead�����́jRead���\�b�h���p�ӂ����ׂ��ł���A�����A
        '���ꂪ�p�ӂ��ꂽ��A����ɕύX���������悢�B�Ȃ��AVolatileRead��
        '�g�p���Ȃ��̂́AServerTelegrapher�Ō��߂����j�ł���B���j�̏ڍׂ�
        'ServerTelegrapher.LastPulseTick�̃R�����g���Q�ƁB
        Get
            Return DirectCast(Interlocked.Add(_LineStatus, 0), LineStatus)
        End Get

        Set(ByVal status As LineStatus)
            Interlocked.Exchange(_LineStatus, status)
        End Set
    End Property
#End Region

#Region "�C�x���g�������\�b�h"
    Protected Overrides Sub ProcOnUnhandledException(ByVal ex As Exception)
        MyBase.ProcOnUnhandledException(ex)

        oForm.BeginInvoke( _
           New RethrowExceptionDelegate(AddressOf oForm.RethrowException), _
           New Object() {ex})
    End Sub

    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case MyInternalMessageKind.ComStartExecRequest
                Return ProcOnComStartExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.InquiryExecRequest
                Return ProcOnInquiryExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ActiveOneExecRequest
                Return ProcOnActiveOneExecRequestReceive(oRcvMsg)
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
    End Function

    Protected Overrides Function ProcOnTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim capRcvTelegs As Boolean
        SyncLock oForm.UiState
            capRcvTelegs = oForm.UiState.CapRcvTelegs
        End SyncLock

        If capRcvTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "Telegram")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    DirectCast(oRcvTeleg, NkTelegram).WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return MyBase.ProcOnTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnComStartExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ComStartExec requested by manager.")

        'TODO: NkSeqCode.Collection�̕�����oRcvMsg����擾�B

        Dim sSeqName As String = "ComStart"
        Log.Info("Register " & sSeqName & " as ActiveOne.")
        Dim oReqTeleg As New NkComStartReqTelegram(NkSeqCode.Collection, Config.ComStartReplyLimitTicks)
        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        Return True
    End Function

    Protected Overridable Function ProcOnInquiryExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("InquiryExec requested by manager.")

        'TODO: NkSeqCode.Collection�̕�����oRcvMsg����擾�B

        Dim sSeqName As String = "Inquiry"
        Log.Info("Register " & sSeqName & " as ActiveOne.")
        Dim oReqTeleg As New NkInquiryReqTelegram(NkSeqCode.Collection, Config.InquiryReplyLimitTicks)
        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ActiveOneExec requested by manager.")

        Dim oExt As ActiveOneExecRequestExtendPart _
           = ActiveOneExecRequest.Parse(oRcvMsg).ExtendPart

        Dim oTeleg As NkDodgyTelegram
        Try
            Dim oImporter As New NkTelegramImporter()
            Using oInputStream As New FileStream(oExt.ApplyFilePath, FileMode.Open, FileAccess.Read)
                oTeleg = oImporter.GetTelegramFromStream(oInputStream)
            End Using
            If oTeleg Is Nothing Then Return True
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return True
        End Try

        Dim sSeqName As String = "ActiveOne"
        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New NkAnonyReqTelegram(oTeleg)
        oReqTeleg.ReplyLimitTicks = oExt.ReplyLimitTicks

        RegisterActiveOne(oReqTeleg, oExt.RetryIntervalTicks, oExt.MaxRetryCountToForget + 1, oExt.MaxRetryCountToCare + 1, sSeqName)
        Return True
    End Function

    '�\���I�P���V�[�P���X�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(NkComStartReqTelegram)
                Log.Info("ComStart completed.")

            Case rtt Is GetType(NkInquiryReqTelegram)
                Log.Info("Inquiry completed.")

            Case Else
                Log.Info("ActiveOne completed.")
        End Select
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        'NOTE: ���蓾�Ȃ��B
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(NkComStartReqTelegram)
                Log.Error("ComStart skipped.")

            Case rtt Is GetType(NkInquiryReqTelegram)
                Log.Error("Inquiry skipped.")

            Case Else
                Log.Error("ActiveOne skipped.")
        End Select
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        'NOTE: ���蓾�Ȃ��B
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(NkComStartReqTelegram)
                Log.Error("ComStart failed.")
                Dim automaticComStart As Boolean
                SyncLock oForm.UiState
                    automaticComStart = oForm.UiState.AutomaticComStart
                End SyncLock
                If automaticComStart AndAlso LineStatus <> LineStatus.Steady Then
                   '�����̊J�ǃV�[�P���X�Ń��g���C�I�[�o�[�����������ꍇ�ł���B
                    enableCommunication = False
                End If

            Case rtt Is GetType(NkInquiryReqTelegram)
                Log.Error("Inquiry failed.")
                'NOTE: �V�~�����[�^�Ȃ̂ŁA�ؒf�͎蓮�Ŏ��R�Ɏ��{���邱�Ƃɂ��Ă���B
                '��ŁuenableCommunication = False�v���s���Ă���P�[�X�́A
                '��������̂��߂̋@�\������ł���B

            Case Else
                Log.Error("ActiveOne failed.")
                'NOTE: �V�~�����[�^�Ȃ̂ŁA�ؒf�͎蓮�Ŏ��R�Ɏ��{���邱�Ƃɂ��Ă���B
                '��ŁuenableCommunication = False�v���s���Ă���P�[�X�́A
                '��������̂��߂̋@�\������ł���B
        End Select
    End Sub

    '�\���I�P���V�[�P���X�̍Œ���L���[�C���O���ꂽ�\���I�P���V�[�P���X�̎��{�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(NkComStartReqTelegram)
                Log.Error("ComStart failed.")

            Case rtt Is GetType(NkInquiryReqTelegram)
                Log.Error("Inquiry failed.")

            Case Else
                Log.Error("ActiveOne failed.")
        End Select
    End Sub

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As NkTelegram = DirectCast(iRcvTeleg, NkTelegram)
        Select Case oRcvTeleg.CmdCode
            Case NkCmdCode.DataPostReq
                Dim isSeqCodeRegistered As Boolean = False
                SyncLock oForm.UiState
                    isSeqCodeRegistered = oForm.UiState.StatusCodeForPassivePostSeqCodes.ContainsKey(oRcvTeleg.SeqCode)
                End SyncLock
                If isSeqCodeRegistered Then
                    Return ProcOnRegisteredPostReqTelegramReceive(oRcvTeleg)
                End If
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnRegisteredPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkDataPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("DataPost REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Log.Info("DataPost REQ received.")

        Dim statusCode As UShort
        SyncLock oForm.UiState
            statusCode = oForm.UiState.StatusCodeForPassivePostSeqCodes(oRcvTeleg.SeqCode)
        End SyncLock

        Dim oReplyTeleg As NkDataPostAckTelegram = oRcvTeleg.CreateAckTelegram(statusCode)
        Log.Info("Sending DataPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    '�e�X���b�h����R�l�N�V�������󂯎�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionAppear()
        enableCommunication = True

        Dim automaticComStart As Boolean
        SyncLock oForm.UiState
            automaticComStart = oForm.UiState.AutomaticComStart
        End SyncLock

        If automaticComStart Then
            Dim sSeqName As String = "ComStart"
            Log.Info("Register " & sSeqName & " as ActiveOne.")
            Dim oReqTeleg As New NkComStartReqTelegram(NkSeqCode.Collection, Config.ComStartReplyLimitTicks)
            RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        End If
    End Sub

    'NOTE: ����Telegrapher�̐e�X���b�h�́A�R�l�N�V������������ۂ�LineStatus��
    '�K��Connected�ɂ��Ă���Telegrapher�Ƀ\�P�b�g��n���悤�Ɏ������Ă��邽�߁A
    '���L�Œ�`���Ă���ProcOnReqTelegramReceiveCompleteBySendAck�`
    'ProcOnConnectionDisappear���Ă΂��ۂ�LineStatus�́AConnected��Steady��
    '�����ꂩ�ł���B

    'REQ�d����M�y�т���ɑ΂���ACK�d�����M���������ĒʐM�p�������肵���ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Function ProcOnReqTelegramReceiveCompleteBySendAck(ByVal iRcvTeleg As ITelegram, ByVal iSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ�d����M�y�т���ɑ΂���y�xNAK�d���iBUSY���j���M���������ĒʐM�p�������肵���ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Function ProcOnReqTelegramReceiveCompleteBySendNak(ByVal iRcvTeleg As ITelegram, ByVal iSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ�d�����M�y�т���ɑ΂���ACK�d����M���������ĒʐM�p�������肵���ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Function ProcOnReqTelegramSendCompleteByReceiveAck(ByVal iSndTeleg As ITelegram, ByVal iRcvTeleg As ITelegram) As Boolean
        LineStatus = LineStatus.Steady
        Return enableCommunication
    End Function

    'REQ�d�����M�y�т���ɑ΂���y�xNAK�d���iBUSY���j��M���������ĒʐM�p�������肵���ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Function ProcOnReqTelegramSendCompleteByReceiveNak(ByVal iSndTeleg As ITelegram, ByVal iRcvTeleg As ITelegram) As Boolean
        Return enableCommunication
    End Function

    '�R�l�N�V������ؒf�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionDisappear()
        LineStatus = LineStatus.Disconnected
        enableCommunication = False 'NOTE: ���W�b�N�I�ɖ��Ӗ������A�����ڂ̐�������ۂ��߁B
    End Sub
#End Region

#Region "�C�x���g���������p���\�b�h"
    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As NkReqTelegram = DirectCast(iReqTeleg, NkReqTelegram)
        oReqTeleg.SrcEkCode = Config.SelfEkCode
        oReqTeleg.DstEkCode = Config.TargetEkCode
        Dim ret As Boolean = MyBase.SendReqTelegram(oReqTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "Telegram")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReqTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return ret
    End Function

    Protected Overrides Function SendReplyTelegram(ByVal iReplyTeleg As ITelegram, ByVal iSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        oReplyTeleg.SrcEkCode = Config.SelfEkCode
        oReplyTeleg.DstEkCode = Config.TargetEkCode
        Dim ret As Boolean = MyBase.SendReplyTelegram(oReplyTeleg, iSourceTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "Telegram")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReplyTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return ret
    End Function
#End Region

End Class

''' <summary>
''' �����ԁB
''' </summary>
Public Enum LineStatus As Integer
    Initial
    Connected
    Steady
    Disconnected
End Enum
