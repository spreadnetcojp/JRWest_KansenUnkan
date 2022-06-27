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

Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �^�ǒ[���Ƃ��ĉ^�ǃT�[�o�Ɠd���̑���M���s���N���X�B
''' </summary>
Public Class OpClientTelegrapher
    Inherits ClientTelegrapher

#Region "�萔��ϐ�"
    '�d������
    Protected oTelegGene As EkTelegramGene

    '�����u�̑��u�R�[�h
    'NOTE: ProcOnReqTelegramReceive()���t�b�N���Ď�M�d����ClientCode�Ɣ�r���Ă��悢�B
    Protected selfEkCode As EkCode

    '���ɑ��M����REQ�d���̒ʔ�
    Protected reqNumberForNextSnd As Integer

    '���Ɏ�M����REQ�d���̒ʔ�
    'NOTE: ProcOnReqTelegramReceive()���t�b�N���āA��M����REQ�d���̒ʔԂ�
    '�A���������`�F�b�N����Ȃ�p�ӂ���B
    'Protected reqNumberForNextRcv As Integer

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
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal oTelegGene As EkTelegramGene)
        MyBase.New(sThreadName, oParentMessageSock, New EkTelegramImporter(oTelegGene))
        Me.oTelegGene = oTelegGene
        Me.reqNumberForNextSnd = 0
        Me.enableCommunication = False
        Me.LineStatus = LineStatus.Initial

        Me.selfEkCode.Unit = 2
        Me.oWatchdogTimer.Renew(Config.WatchdogIntervalLimitTicks)
        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite
        Me.enableWatchdog = Config.EnableWatchdog
        Me.enableXllStrongExclusion = Config.EnableXllStrongExclusion
        Me.enableActiveSeqStrongExclusion = Config.EnableActiveSeqStrongExclusion
        Me.enableActiveOneOrdering = Config.EnableActiveOneOrdering

        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(Me.oActiveXllWorkerMessageSock, oChildSock)
        Me.oActiveXllWorker = New FtpWorker( _
           sThreadName & "-ActiveXll", _
           oChildSock, _
           Config.FtpServerUri, _
           New NetworkCredential(Config.FtpUserName, Config.FtpPassword), _
           Config.FtpRequestLimitTicks, _
           Config.FtpLogoutLimitTicks, _
           Config.FtpTransferStallLimitTicks, _
           Config.FtpUsePassiveMode, _
           Config.FtpLogoutEachTime, _
           Config.FtpBufferLength)
        Me.activeXllWorkerPendingLimitTicks = Config.FtpWorkerPendingLimitTicks
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
    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case ClientAppInternalMessageKind.MasProUllRequest
                Return ProcOnMasProUllRequestReceive(oRcvMsg)
            Case ClientAppInternalMessageKind.MasProDllInvokeRequest
                Return ProcOnMasProDllInvokeRequestReceive(oRcvMsg)
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
    End Function

    '�w�b�_���̓��e���E�H�b�`�h�b�OREQ�d���̂��̂ł��邩���肷�郁�\�b�h
    Protected Overrides Function IsWatchdogReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oTeleg.ObjCode = EkWatchdogReqTelegram.FormalObjCodeInOpClient Then
            Return True
        Else
            Return False
        End If
    End Function

    '�n���ꂽ�d���C���X�^���X��K�؂Ȍ^�̃C���X�^���X�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsWatchdogReq(ByVal iTeleg As ITelegram) As IWatchdogReqTelegram
        Return New EkWatchdogReqTelegram(iTeleg)
    End Function

    Protected Overridable Function ProcOnMasProDllInvokeRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("MasProDllInvoke requested by manager.")

        Dim oExt As MasProDllInvokeRequestExtendPart _
           = MasProDllInvokeRequest.Parse(oRcvMsg).ExtendPart
        Dim oReqTeleg As New EkMasProDllInvokeReqTelegram( _
           oTelegGene, _
           EkMasProDllInvokeReqTelegram.FormalObjCode, _
           oExt.ListFileName, _
           oExt.ForcingFlag, _
           Config.MasProDllInvokeReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, "MasProDllInvoke")
        Return True
    End Function

    '�\���I�P���V�[�P���X�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal oReqTeleg As IReqTelegram, ByVal oAckTeleg As ITelegram)
        Dim rtt As Type = oReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Info("ComStart completed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case rtt Is GetType(EkMasProDllInvokeReqTelegram)
                Log.Info("MasProDllInvoke completed.")
                MasProDllInvokeResponse.Gen(MasProDllInvokeResult.Completed).WriteToSocket(oParentMessageSock)
        End Select
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal oReqTeleg As IReqTelegram, ByVal oNakTeleg As INakTelegram)
        Dim rtt As Type = oReqTeleg.GetType()
        Select Case True
            'NOTE: ���蓾�Ȃ��B
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Warn("ComStart skipped by illegal NAK.")
                Me.enableCommunication = False

            'NOTE: ���蓾�Ȃ��B
            Case rtt Is GetType(EkMasProDllInvokeReqTelegram)
                Log.Warn("MasProDllInvoke skipped by illegal NAK.")
                MasProDllInvokeResponse.Gen(MasProDllInvokeResult.Failed).WriteToSocket(oParentMessageSock)
        End Select
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal oReqTeleg As IReqTelegram, ByVal oNakTeleg As INakTelegram)
        Dim rtt As Type = oReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart failed.")
                Me.enableCommunication = False

            Case rtt Is GetType(EkMasProDllInvokeReqTelegram)
                Log.Error("MasProDllInvoke failed.")

                Dim result As MasProDllInvokeResult = MasProDllInvokeResult.FailedByUnknownLight
                If oNakTeleg IsNot Nothing Then
                    Select Case oNakTeleg.CauseCode
                        Case EkNakCauseCode.Busy
                            result = MasProDllInvokeResult.FailedByBusy
                        Case EkNakCauseCode.NoData
                            result = MasProDllInvokeResult.FailedByNoData
                        Case EkNakCauseCode.Unnecessary
                            result = MasProDllInvokeResult.FailedByUnnecessary
                        Case EkNakCauseCode.InvalidContent
                            result = MasProDllInvokeResult.FailedByInvalidContent
                    End Select
                End If

                MasProDllInvokeResponse.Gen(result).WriteToSocket(oParentMessageSock)
        End Select
    End Sub

    '�\���I�P���V�[�P���X�̍Œ���L���[�C���O���ꂽ�\���I�P���V�[�P���X�̎��{�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal oReqTeleg As IReqTelegram)
        Dim rtt As Type = oReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart failed.")

            Case rtt Is GetType(EkMasProDllInvokeReqTelegram)
                Log.Error("MasProDllInvoke failed.")
                MasProDllInvokeResponse.Gen(MasProDllInvokeResult.Failed).WriteToSocket(oParentMessageSock)
        End Select
    End Sub

    Protected Overridable Function ProcOnMasProUllRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("MasProUll requested by manager.")

        Dim oXllReqTeleg As New EkClientDrivenUllReqTelegram( _
           oTelegGene, _
           EkClientDrivenUllReqTelegram.FormalObjCodeAsOpClientFile, _
           ContinueCode.Start, _
           MasProUllRequest.Parse(oRcvMsg).FileName, _
           Config.MasProUllTransferLimitTicks, _
           Config.MasProUllStartReplyLimitTicks)

        RegisterActiveUll(oXllReqTeleg, 0, 1, 1)
        Return True
    End Function

    '�\���IULL�̓]���J�nREQ�d���ɑ����]���I��REQ�d���𐶐����郁�\�b�h
    Protected Overrides Function CreateActiveUllContinuousReqTelegram(ByVal oXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)
        Return oRealUllReqTeleg.CreateContinuousTelegram(cc, 0, Config.MasProUllFinishReplyLimitTicks)
    End Function

    '�\���IULL�����������i�]���I��REQ�d���ɑ΂���ACK�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnActiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Log.Info("Ull file completed.")
        MasProUllResponse.Gen(MasProUllResult.Completed).WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�ɂē]���I��REQ�d���ɑ΂���NAK�d������M�����ꍇ
    Protected Overrides Sub ProcOnActiveUllFinalizeError(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Error("Ull file failed by FinalizeError.")

        Dim result As MasProUllResult = MasProUllResult.FailedByUnknownLight
        If oNakTeleg IsNot Nothing Then
            Select Case oNakTeleg.CauseCode
                Case EkNakCauseCode.InvalidContent
                    result = MasProUllResult.FailedByInvalidContent
            End Select
        End If

        MasProUllResponse.Gen(result).WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�ɂē]�������s�����iContinueCode.Abort�̓]���I��REQ�d���𑗐M���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Sub ProcOnActiveUllTransferError(ByVal oXllReqTeleg As IXllReqTelegram)
        Log.Error("Ull file failed by TransferError.")
        MasProUllResponse.Gen(MasProUllResult.Failed).WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveUllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        'NOTE: ���蓾�Ȃ��Ǝv���邪�A���肪�Ԃ��Ă���NAK����ł��邽�߁A�������Ă����B
        '�{���ɂ��蓾�Ȃ����̂ƈ����ɂ́AGetRequirement()�ɂāA
        'OpClientFileUll�Ɋւ���EkNakCauseCode.NoData��NAK��ؒf�����ɂ���Ƃ悢�B
        Log.Warn("Ull file failed by surprising RetryOver.")
        MasProUllResponse.Gen(MasProUllResult.Failed).WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveUllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Error("Ull file failed by RetryOver.")

        Dim result As MasProUllResult = MasProUllResult.FailedByUnknownLight
        If oNakTeleg IsNot Nothing Then
            Select Case oNakTeleg.CauseCode
                Case EkNakCauseCode.Busy
                    result = MasProUllResult.FailedByBusy
                Case EkNakCauseCode.InvalidContent
                    result = MasProUllResult.FailedByInvalidContent
            End Select
        End If

        MasProUllResponse.Gen(result).WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�̍Œ���L���[�C���O���ꂽ�\���IULL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
        Log.Error("Ull file failed.")
        MasProUllResponse.Gen(MasProUllResult.Failed).WriteToSocket(oParentMessageSock)
    End Sub

    '�e�X���b�h����R�l�N�V�������󂯎�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionAppear()
        reqNumberForNextSnd = 0
        enableCommunication = True

        enableActiveOneOrdering = True
        Log.Info("Register ComStart as ActiveOne.")

        Dim oReqTeleg As New EkComStartReqTelegram( _
           oTelegGene, _
           EkComStartReqTelegram.FormalObjCodeInOpClient,
           Config.ComStartReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, "ComStart")
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
        Dim oReqTeleg As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        oReqTeleg.ReqNumber = reqNumberForNextSnd
        oReqTeleg.ClientCode = selfEkCode
        Dim ret As Boolean = MyBase.SendReqTelegram(oReqTeleg)

        If reqNumberForNextSnd >= 999999 Then
            reqNumberForNextSnd = 0
        Else
            reqNumberForNextSnd += 1
        End If

        Return ret
    End Function

    Protected Overrides Function SendReplyTelegram(ByVal iReplyTeleg As ITelegram, ByVal iSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As EkTelegram = DirectCast(iReplyTeleg, EkTelegram)
        Dim oSourceTeleg As EkTelegram = DirectCast(iSourceTeleg, EkTelegram)
        oReplyTeleg.RawReqNumber = oSourceTeleg.RawReqNumber
        oReplyTeleg.RawClientCode = oSourceTeleg.RawClientCode
        Return MyBase.SendReplyTelegram(oReplyTeleg, oSourceTeleg)
    End Function

    'NAK�d���𑗐M����ꍇ���M�����ꍇ�̂��̌�̋��������߂邽�߂̃��\�b�h
    Protected Overrides Function GetRequirement(ByVal oNakTeleg As INakTelegram) As NakRequirement
        'NOTE: �^�ǒ[���ł́A���M���邠����REQ�d����LimitNakCount��1�ł��邽�߁A
        '�y�x��NAK�́A�S��ForgetOnRetryOver�ɂ��Ă��������A
        '�S��CareOnRetryOver�ɂ��Ă������B
        'NOTE: ���葕�u�ɑ΂��Č������ڂ��Ă悢�Ȃ�A�f�[�^��ʂȂǂŕ��򂷂邱�Ƃ��\�B
        Select Case oNakTeleg.CauseCode
            '�p���i���g���C�I�[�o�[�j���Ă��ُ�Ƃ݂͂Ȃ��Ȃ�NAK�d��
            'Case EkNakCauseCode.Xxxx
            '    Return NakRequirement.ForgetOnRetryOver

            '�p���i���g���C�I�[�o�[�j������ُ�Ƃ݂Ȃ��ׂ�NAK�d��
            Case EkNakCauseCode.Busy, EkNakCauseCode.NoData, EkNakCauseCode.NoTime, EkNakCauseCode.Unnecessary, EkNakCauseCode.InvalidContent, EkNakCauseCode.UnknownLight
                Return NakRequirement.CareOnRetryOver

            '�ʐM�ُ�Ƃ݂Ȃ��ׂ�NAK�d��
            Case EkNakCauseCode.TelegramError, EkNakCauseCode.NotPermit, EkNakCauseCode.HashValueError, EkNakCauseCode.UnknownFatal
                Return NakRequirement.DisconnectImmediately

            'NOTE: �ǂ̂悤�ȃo�C�g���Parse���Ă�CauseCode��None��
            'NAK�d���ɂ͂Ȃ�Ȃ��͂��ł��邽�߁ACauseCode��None�̏ꍇ�A
            '���L�̃P�[�X�Ƃ��ď�������B
            Case Else
                Debug.Fail("This case is impermissible.")
                Return NakRequirement.CareOnRetryOver
        End Select
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
