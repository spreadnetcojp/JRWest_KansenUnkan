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
''' �w���@��Ƃ��ĉ^�ǃT�[�o�Ɠd���̑���M���s���N���X�B
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

    '�d������
    Protected oTelegGene As EkTelegramGene

    'FTP�T�C�g�̃��[�g�Ƒ΂ɂȂ郍�[�J���f�B���N�g���i��Ɨp�j
    Protected sFtpBasePath As String

    'FTP�T�C�g���ɂ����鍆�@�ʃf�B���N�g���̃p�X
    Protected sPermittedPathInFtp As String

    'FTP�T�C�g�̍��@�ʃf�B���N�g���Ƒ΂ɂȂ郍�[�J���f�B���N�g���i��Ɨp�j
    Protected sPermittedPath As String

    '����M�����f�B���N�g��
    Protected sCapDirPath As String

    '�����u�̑��u�R�[�h
    'TODO: ProcOnReqTelegramReceive()���t�b�N���Ď�M�d����ClientCode�Ɣ�r���Ă��悢�B
    Protected selfEkCode As EkCode

    '���ɑ��M����REQ�d���̒ʔ�
    Protected reqNumberForNextSnd As Integer

    '���Ɏ�M����REQ�d���̒ʔ�
    'TODO: ProcOnReqTelegramReceive()���t�b�N���āA��M����REQ�d���̒ʔԂ�
    '�A���������`�F�b�N����Ȃ�p�ӂ���B
    'Protected reqNumberForNextRcv As Integer

    'ComStart�V�[�P���X�ɕt�^����ʔԁi���O�o�͗p�j
    Protected traceNumberForComStart As Integer

    'TimeDataGet�V�[�P���X�ɕt�^����ʔԁi���O�o�͗p�j
    Protected traceNumberForTimeDataGet As Integer

    '�C��ActiveOne�V�[�P���X�ɕt�^����ʔԁi���O�o�͗p�j
    Protected traceNumberForActiveOne As Integer

    '�d���̌������p�����ׂ����ۂ�
    Protected enableCommunication As Boolean

    'NOTE: �u�Ӑ}�I�Ȑؒf�v�Ɓu�ُ�ɂ��ؒf�v����ʂ������Ȃ�΁A
    'Protected needConnection As Boolean��p�ӂ��A
    'ProcOnConnectNoticeReceive()��ProcOnDisconnectRequestReceive()���t�b�N����
    '�����ON/OFF����Ƃ悢�BProcOnConnectionDisappear()�ł́A������݂āA
    '�J�ڐ�̉����Ԃ����߂邱�Ƃ��ł���B

    '������
    Private _LineStatus As Integer

    '�E�H�b�`�h�b�O�̃f�[�^���
    Private Shared ReadOnly ObjCodeForWatchdogIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkWatchdogReqTelegram.FormalObjCodeInTokatsu}, _
       {EkAplProtocol.Kanshiban, EkWatchdogReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkWatchdogReqTelegram.FormalObjCodeInMadosho}, _
       {EkAplProtocol.Kanshiban2, EkWatchdogReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkWatchdogReqTelegram.FormalObjCodeInMadosho}}

    '�ڑ��������̃f�[�^���
    Private Shared ReadOnly ObjCodeForComStartIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Kanshiban, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkComStartReqTelegram.FormalObjCodeInMadosho}, _
       {EkAplProtocol.Kanshiban2, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkComStartReqTelegram.FormalObjCodeInMadosho}}

    '�����f�[�^�擾�̃f�[�^���
    Private Shared ReadOnly ObjCodeForTimeDataGetIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkTimeDataGetReqTelegram.FormalObjCodeInTokatsu}, _
       {EkAplProtocol.Kanshiban, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Kanshiban2, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}}
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal oTelegGene As EkTelegramGene, ByVal sFtpBasePath As String, ByVal sCapDirPath As String, ByVal oForm As MainForm)
        MyBase.New(sThreadName, oParentMessageSock, New EkTelegramImporter(oTelegGene))
        Me.oTelegGene = oTelegGene
        Me.sFtpBasePath = sFtpBasePath
        Me.sCapDirPath = sCapDirPath
        Me.oForm = oForm
        Me.reqNumberForNextSnd = 0
        Me.traceNumberForTimeDataGet = 0
        Me.enableCommunication = False
        Me.LineStatus = LineStatus.Initial

        Me.selfEkCode = Config.SelfEkCode
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

        Dim oActiveChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(Me.oActiveXllWorkerMessageSock, oActiveChildSock)
        Me.oActiveXllWorker = New FtpWorker( _
           sThreadName & "-ActiveXll", _
           oActiveChildSock, _
           Config.FtpServerUri, _
           New NetworkCredential(Config.FtpUserName, Config.FtpPassword), _
           Config.ActiveFtpRequestLimitTicks, _
           Config.ActiveFtpLogoutLimitTicks, _
           Config.ActiveFtpTransferStallLimitTicks, _
           Config.ActiveFtpUsePassiveMode, _
           Config.ActiveFtpLogoutEachTime, _
           Config.ActiveFtpBufferLength)
        Me.activeXllWorkerPendingLimitTicks = Config.ActiveFtpWorkerPendingLimitTicks

        Dim oPassiveChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(Me.oPassiveXllWorkerMessageSock, oPassiveChildSock)
        Me.oPassiveXllWorker = New FtpWorker( _
           sThreadName & "-PassiveXll", _
           oPassiveChildSock, _
           Config.FtpServerUri, _
           New NetworkCredential(Config.FtpUserName, Config.FtpPassword), _
           Config.PassiveFtpRequestLimitTicks, _
           Config.PassiveFtpLogoutLimitTicks, _
           Config.PassiveFtpTransferStallLimitTicks, _
           Config.PassiveFtpUsePassiveMode, _
           Config.PassiveFtpLogoutEachTime, _
           Config.PassiveFtpBufferLength)
        Me.passiveXllWorkerPendingLimitTicks = Config.PassiveFtpWorkerPendingLimitTicks

        Me.sPermittedPathInFtp = Path.Combine(Config.ModelPathInFtp, selfEkCode.ToString(sDirNameFormat))
        Me.sPermittedPath = Utility.CombinePathWithVirtualPath(sFtpBasePath, sPermittedPathInFtp)
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
            Case MyInternalMessageKind.TimeDataGetExecRequest
                Return ProcOnTimeDataGetExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ActiveOneExecRequest
                Return ProcOnActiveOneExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ActiveUllExecRequest
                Return ProcOnActiveUllExecRequestReceive(oRcvMsg)
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
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    DirectCast(oRcvTeleg, EkTelegram).WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return MyBase.ProcOnTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnComStartExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ComStartExec requested by manager.")

        Dim sSeqName As String = "ComStart #" & traceNumberForComStart.ToString()
        UpdateTraceNumberForComStart()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New EkComStartReqTelegram( _
           oTelegGene, _
           ObjCodeForComStartIn(Config.AplProtocol),
           Config.ComStartReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        Return True
    End Function

    Protected Overridable Function ProcOnTimeDataGetExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("TimeDataGetExec requested by manager.")

        Dim sSeqName As String = "TimeDataGet #" & traceNumberForTimeDataGet.ToString()
        UpdateTraceNumberForTimeDataGet()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New EkTimeDataGetReqTelegram( _
           oTelegGene, _
           ObjCodeForTimeDataGetIn(Config.AplProtocol),
           Config.TimeDataGetReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ActiveOneExec requested by manager.")

        Dim oExt As ActiveOneExecRequestExtendPart _
           = ActiveOneExecRequest.Parse(oRcvMsg).ExtendPart

        Dim oTeleg As EkDodgyTelegram
        Try
            Dim oImporter As New EkTelegramImporter(oTelegGene)
            Using oInputStream As New FileStream(oExt.ApplyFilePath, FileMode.Open, FileAccess.Read)
                oTeleg = oImporter.GetTelegramFromStream(oInputStream)
            End Using
            If oTeleg Is Nothing Then Return True
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return True
        End Try

        Dim sSeqName As String = "ActiveOne #" & traceNumberForActiveOne.ToString()
        UpdateTraceNumberForActiveOne()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New EkAnonyReqTelegram(oTeleg, oExt.ReplyLimitTicks)
        RegisterActiveOne(oReqTeleg, oExt.RetryIntervalTicks, oExt.MaxRetryCountToForget + 1, oExt.MaxRetryCountToCare + 1, sSeqName)
        Return True
    End Function

    '�\���I�P���V�[�P���X�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Info("ComStart completed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case rtt Is GetType(EkTimeDataGetReqTelegram)
                Log.Info("TimeDataGet completed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case Else
                Log.Info("ActiveOne completed.")
        End Select
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    'NOTE: �{�N���X��GetRequirement()�̎�����A���̃��\�b�h���Ă΂�邱�Ƃ͂��蓾�Ȃ��B
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart skipped.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case rtt Is GetType(EkTimeDataGetReqTelegram)
                Log.Error("TimeDataGet skipped.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case Else
                Log.Error("ActiveOne skipped.")
        End Select
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart failed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering
                Dim automaticComStart As Boolean
                SyncLock oForm.UiState
                    automaticComStart = oForm.UiState.AutomaticComStart
                End SyncLock
                If automaticComStart AndAlso LineStatus <> LineStatus.Steady Then
                   '�J�n�̂��߂̐ڑ��������V�[�P���X�Ń��g���C�I�[�o�[�����������ꍇ�ł���B
                    enableCommunication = False
                End If

            Case rtt Is GetType(EkTimeDataGetReqTelegram)
                Log.Error("TimeDataGet failed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering
                Dim automaticComStart As Boolean
                SyncLock oForm.UiState
                    automaticComStart = oForm.UiState.AutomaticComStart
                End SyncLock
                If automaticComStart AndAlso LineStatus <> LineStatus.Steady Then
                   '�J�n�̂��߂̐����V�[�P���X�Ń��g���C�I�[�o�[�����������ꍇ�ł���B
                    enableCommunication = False
                End If

            Case Else
                Log.Error("ActiveOne retry over.")
                'NOTE: �V�~�����[�^�䂦�ɁA�ؒf�͎����I�ɍs���Ȃ�����
                '�悢���߁AenableCommunication��True�̂܂܂ɂ��Ă����B
                '��ŁuenableCommunication = False�v���s���Ă���̂́A
                '�ڑ��ƃZ�b�g�Ŏ����I�ɍs�����V�[�P���X�����s����
                '�P�[�X�̂��߂ł���B
        End Select
    End Sub

    '�\���I�P���V�[�P���X�̍Œ���L���[�C���O���ꂽ�\���I�P���V�[�P���X�̎��{�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart failed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case rtt Is GetType(EkTimeDataGetReqTelegram)
                Log.Error("TimeDataGet failed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case Else
                Log.Error("ActiveOne failed.")
        End Select
    End Sub

    Protected Overridable Function ProcOnActiveUllExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ActiveUllExec requested by manager.")

        Dim oExt As ActiveUllExecRequestExtendPart _
           = ActiveUllExecRequest.Parse(oRcvMsg).ExtendPart

        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram
        Try
            Dim sTransferFileName As String = Path.GetFileName(oExt.TransferFilePath)
            File.Copy(oExt.TransferFilePath, Path.Combine(sPermittedPath, sTransferFileName), True)

            oXllReqTeleg = New EkClientDrivenUllReqTelegram( _
               oTelegGene, _
               oExt.ObjCode, _
               ContinueCode.Start, _
               Path.Combine(sPermittedPathInFtp, sTransferFileName), _
               oExt.TransferFileHashValue, _
               oExt.TransferLimitTicks, _
               oExt.ReplyLimitTicks)

        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return True
        End Try

        RegisterActiveUll(oXllReqTeleg, oExt.RetryIntervalTicks, oExt.MaxRetryCountToForget + 1, oExt.MaxRetryCountToCare + 1)
        Return True
    End Function

    '�\���IULL�̓]���J�nREQ�d���ɑ����]���I��REQ�d���𐶐����郁�\�b�h
    Protected Overrides Function CreateActiveUllContinuousReqTelegram(ByVal oXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)
        'TODO: �Ō�̈����́A�ʂ̐ݒ�l���Q�Ƃ������B
        Return oRealUllReqTeleg.CreateContinuousTelegram(cc, 0, oRealUllReqTeleg.ReplyLimitTicks)
    End Function

    '�\���IULL�����������i�]���I��REQ�d���ɑ΂���ACK�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnActiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Log.Info("ActiveUll completed.")
    End Sub

    '�\���IULL�ɂē]���I��REQ�d���ɑ΂���NAK�d������M�����ꍇ
    Protected Overrides Sub ProcOnActiveUllFinalizeError(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Error("ActiveUll failed by finalize error.")
    End Sub

    '�\���IULL�ɂē]�������s�����iContinueCode.Abort�̓]���I��REQ�d���𑗐M���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Sub ProcOnActiveUllTransferError(ByVal oXllReqTeleg As IXllReqTelegram)
        Log.Error("ActiveUll failed by transfer error.")
    End Sub

    '�\���IULL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    'NOTE: �{�N���X��GetRequirement()�̎�����A���̃��\�b�h���Ă΂�邱�Ƃ͂��蓾�Ȃ��B
    Protected Overrides Sub ProcOnActiveUllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Fatal("ActiveUll failed by surprising retry over.")
    End Sub

    '�\���IULL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveUllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Error("ActiveUll failed by retry over.")
    End Sub

    '�\���IULL�̍Œ���L���[�C���O���ꂽ�\���IULL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
        Log.Error("ActiveUll failed.")
    End Sub

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)
        Select Case oRcvTeleg.SubCmdCode
            Case EkSubCmdCode.Get
                Dim isObjCodeRegistered As Boolean = False
                Dim sApplyFilePath As String = Nothing
                SyncLock oForm.UiState
                    If oForm.UiState.ApplyFileForPassiveGetObjCodes.ContainsKey(CByte(oRcvTeleg.ObjCode)) Then
                        isObjCodeRegistered = True
                        sApplyFilePath = oForm.UiState.ApplyFileForPassiveGetObjCodes(CByte(oRcvTeleg.ObjCode))
                    End If
                End SyncLock
                If isObjCodeRegistered Then
                    Return ProcOnRegisteredGetReqTelegramReceive(oRcvTeleg, sApplyFilePath)
                End If

            Case EkSubCmdCode.Post
                Dim isObjCodeRegistered As Boolean = False
                SyncLock oForm.UiState
                    isObjCodeRegistered = oForm.UiState.SomethingForPassivePostObjCodes.ContainsKey(CByte(oRcvTeleg.ObjCode))
                End SyncLock
                If isObjCodeRegistered Then
                    Return ProcOnRegisteredPostReqTelegramReceive(oRcvTeleg)
                End If
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnRegisteredGetReqTelegramReceive(ByVal iRcvTeleg As ITelegram, ByVal sApplyFilePath As String) As Boolean
        Dim oRcvTeleg As New EkByteArrayGetReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("ByteArrayGet REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("ByteArrayGet REQ received.")

        Dim forceNakCause As NakCauseCode = EkNakCauseCode.None
        SyncLock oForm.UiState
            If oForm.UiState.ForceReplyNakToPassiveGetReq Then
                forceNakCause = New EkNakCauseCode(oForm.UiState.NakCauseNumberToPassiveGetReq, oForm.UiState.NakCauseTextToPassiveGetReq)
            End If
        End SyncLock
        If forceNakCause <> EkNakCauseCode.None Then
            If SendNakTelegram(forceNakCause, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        If sApplyFilePath Is Nothing OrElse _
           Not File.Exists(sApplyFilePath) Then
            Log.Warn("No data exists to reply.")
            If SendNakTelegram(EkNakCauseCode.NoData, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        Log.Info("Reading reply data from [" & sApplyFilePath & "]...")
        Dim aReplyBytes As Byte()
        Try
            Using oInputStream As New FileStream(sApplyFilePath, FileMode.Open, FileAccess.Read)
                '�t�@�C���̃����O�X���擾����B
                Dim len As Integer = CInt(oInputStream.Length)
                '�t�@�C����ǂݍ��ށB
                aReplyBytes = New Byte(len - 1) {}
                Dim pos As Integer = 0
                Do
                    Dim readSize As Integer = oInputStream.Read(aReplyBytes, pos, len - pos)
                    If readSize = 0 Then Exit Do
                    pos += readSize
                Loop
            End Using
        Catch ex As Exception
            Log.Error("Unwelcome Exception caught.", ex)
            SendNakTelegramThenDisconnect(EkNakCauseCode.Busy, oRcvTeleg) 'TODO: ����
            Return True
        End Try

        Dim oReplyTeleg As EkByteArrayGetAckTelegram = oRcvTeleg.CreateAckTelegram(aReplyBytes)
        Log.Info("Sending ByteArrayGet ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnRegisteredPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkByteArrayPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("ByteArrayPost REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("ByteArrayPost REQ received.")

        Dim forceNakCause As NakCauseCode = EkNakCauseCode.None
        SyncLock oForm.UiState
            If oForm.UiState.ForceReplyNakToPassivePostReq Then
                forceNakCause = New EkNakCauseCode(oForm.UiState.NakCauseNumberToPassivePostReq, oForm.UiState.NakCauseTextToPassivePostReq)
            End If
        End SyncLock
        If forceNakCause <> EkNakCauseCode.None Then
            If SendNakTelegram(forceNakCause, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        Dim oReplyTeleg As EkByteArrayPostAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ByteArrayPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    '�w�b�_���̓��e���󓮓IDLL��REQ�d���̂��̂ł��邩���肷�郁�\�b�h
    Protected Overrides Function IsPassiveDllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get Then
            SyncLock oForm.UiState
                Return oForm.UiState.SomethingForPassiveDllObjCodes.ContainsKey(CByte(oTeleg.ObjCode))
            End SyncLock
        End If

        Return False
    End Function

    '�n���ꂽ�d���C���X�^���X��K�؂Ȍ^�̃C���X�^���X�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsPassiveDllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)

        Dim transferLimitTicks As Integer
        SyncLock oForm.UiState
            transferLimitTicks = oForm.UiState.PassiveDllTransferLimitTicks
        End SyncLock

        'TODO: ���݂̃v���g�R���ɂ�����󓮓IDLL�V�[�P���X��REQ�d����
        '�f�[�^��ʂɊ֌W�Ȃ�EkMasProDllReqTelegram�ł��邪�A�����łȂ��Ȃ���
        '�ꍇ�̂��Ƃ�z�肷��Ȃ�AoForm.UiState.SomethingForPassiveDllObjCode
        '�ɂ́A�d���̌^���i�[���Ă����������悢��������Ȃ��B
        Return New EkMasProDllReqTelegram(oTeleg, transferLimitTicks)
    End Function

    '�󓮓IDLL�̏����i�\�����ꂽ�t�@�C���̎󂯓���m�F�j���s�����\�b�h
    Protected Overrides Function PrepareToStartPassiveDll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        SyncLock oForm.UiState
            If oForm.UiState.ForceReplyNakToPassiveDllStartReq Then
                Return New EkNakCauseCode(oForm.UiState.NakCauseNumberToPassiveDllStartReq, oForm.UiState.NakCauseTextToPassiveDllStartReq)
            End If
        End SyncLock

        'NOTE: ���O�Ƀ`�F�b�N���Ă��邽�߁AiXllReqTeleg.DataFileName���̓p�X�Ƃ��Ė��Q�ł���B
        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Log.Info("Starting PassiveDll of the files [" & Path.GetFileName(oXllReqTeleg.DataFileName) & "] [" & Path.GetFileName(oXllReqTeleg.ListFileName) & "]...")
        Return EkNakCauseCode.None
    End Function

    '�󓮓IDLL�̓]���J�nREQ�d���ɑ����]���I��REQ�d���𐶐����郁�\�b�h
    Protected Overrides Function CreatePassiveDllContinuousReqTelegram(ByVal iXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
                Dim replyLimitTicks As Integer
                Dim resultantVersionOfSlot1 As Integer
                Dim resultantVersionOfSlot2 As Integer
                Dim resultantFlagOfFull As Integer
                SyncLock oForm.UiState
                    replyLimitTicks = oForm.UiState.PassiveDllFinishReplyLimitTicks
                    resultantVersionOfSlot1 = oForm.UiState.PassiveDllResultantVersionOfSlot1
                    resultantVersionOfSlot2 = oForm.UiState.PassiveDllResultantVersionOfSlot2
                    resultantFlagOfFull = oForm.UiState.PassiveDllResultantFlagOfFull
                End SyncLock
                Return oXllReqTeleg.CreateContinuousTelegram(cc, resultantVersionOfSlot1, resultantVersionOfSlot2, resultantFlagOfFull, 0, replyLimitTicks)

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select

        Return Nothing
    End Function

    '�󓮓IDLL�����������i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e���������Ă��邱�Ƃ��m�F�����j
    '�iContinueCode.Finish�̓]���I��REQ�d���𑗐M���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Function ProcOnPassiveDllComplete(ByVal iXllReqTeleg As IXllReqTelegram) As Boolean
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Log.Info("PassiveDll completed.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select

        SyncLock oForm.UiState
            Return oForm.UiState.SimulateStoringOnPassiveDllComplete
        End SyncLock
    End Function

    '�󓮓IDLL�ɂē]�����������o�����i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e�ɕs���������o�����j
    '�iContinueCode.Abort�̓]���I��REQ�d���𑗐M���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Sub ProcOnPassiveDllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Log.Error("PassiveDll failed by hash value error.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '�󓮓IDLL�ɂē]�������s�����iContinueCode.Abort�̓]���I��REQ�d���𑗐M���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Sub ProcOnPassiveDllTransferError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Log.Error("PassiveDll failed by transfer error.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '�󓮓IDLL�̍Œ���L���[�C���O���ꂽ�󓮓IDLL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnPassiveDllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Log.Error("PassiveDll failed.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '�w�b�_���̓��e���󓮓IULL��REQ�d���̂��̂ł��邩���肷�郁�\�b�h
    Protected Overrides Function IsPassiveUllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get Then
            SyncLock oForm.UiState
                Return oForm.UiState.ApplyFileForPassiveUllObjCodes.ContainsKey(CByte(oTeleg.ObjCode))
            End SyncLock
        End If

        Return False
    End Function

    '�n���ꂽ�d���C���X�^���X��K�؂Ȍ^�̃C���X�^���X�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsPassiveUllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)

        Dim transferLimitTicks As Integer
        SyncLock oForm.UiState
            transferLimitTicks = oForm.UiState.PassiveUllTransferLimitTicks
        End SyncLock

        'TODO: ���݂̃v���g�R���ɂ�����󓮓IULL�V�[�P���X��REQ�d����
        '�f�[�^��ʂɊ֌W�Ȃ�EkServerDrivenUllReqTelegram�ł��邪�A�����łȂ��Ȃ���
        '�ꍇ�̂��Ƃ�z�肷��Ȃ�AoForm.UiState.TypeForPassiveUllObjCode�̂悤��
        '�Ƃ���ɁA�d���̌^���i�[���Ă����������悢��������Ȃ��B
        Return New EkServerDrivenUllReqTelegram(oTeleg, transferLimitTicks)
    End Function

    '�󓮓IULL�̏����i�w�肳�ꂽ�t�@�C���̗p�Ӂj���s�����\�b�h
    Protected Overrides Function PrepareToStartPassiveUll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
                SyncLock oForm.UiState
                    If oForm.UiState.ForceReplyNakToPassiveUllStartReq Then
                        Return New EkNakCauseCode(oForm.UiState.NakCauseNumberToPassiveUllStartReq, oForm.UiState.NakCauseTextToPassiveUllStartReq)
                    End If
                End SyncLock

                Dim isObjCodeRegistered As Boolean = False
                Dim sApplyFilePath As String = Nothing
                SyncLock oForm.UiState
                    'NOTE: ���b�N���������Ă����ԂɕύX����Ă���\��������̂ŁA
                    'ApplyFileForPassiveUllObjCodes�ɓo�^����Ă��邩�ēx�`�F�b�N��
                    '�s�����Ƃɂ��Ă���B
                    If oForm.UiState.ApplyFileForPassiveUllObjCodes.ContainsKey(CByte(oXllReqTeleg.ObjCode)) Then
                        isObjCodeRegistered = True
                        sApplyFilePath = oForm.UiState.ApplyFileForPassiveUllObjCodes(CByte(oXllReqTeleg.ObjCode))
                    End If
                End SyncLock

                If Not isObjCodeRegistered Then
                    Log.Warn("Setting was changed during a sequence.")
                    Return EkNakCauseCode.NoData 'TODO: ����
                End If

                If sApplyFilePath Is Nothing OrElse _
                   Not File.Exists(sApplyFilePath) Then
                    Log.Warn("No data exists to reply.")
                    Return EkNakCauseCode.NoData
                End If

                'NOTE: ���葕�u�̕s����݂���₷���悤�AoXllReqTeleg.FileName��
                'ObjCode�Ɛ������Ă��Ȃ��ꍇ�ɁA�x�����炢�͏o���Ă��悢�Ǝv����B
                '�������A���̌x���𗊂�Ɏ���������ɂ́A���̃V�~�����[�^�̎�����
                '���O�ɍs���ׂ��ł���A�{���]�|�ł��邽�߁A��߂Ă����B
                'NOTE: ���O�Ƀ`�F�b�N���Ă��邽�߁AoXllReqTeleg.FileName�̓p�X�Ƃ��Ė��Q�ł���B
                Dim sTransferFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
                Try
                    File.Copy(sApplyFilePath, Path.Combine(sPermittedPath, sTransferFileName), True)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    'NOTE: ���File.Exists����File.Copy�܂ł̊Ԃ�
                    '�t�@�C�����ړ���폜���ꂽ�P�[�X�Ƃ݂Ȃ��B
                    Return EkNakCauseCode.NoData
                End Try

                Log.Info("Starting PassiveUll of the file [" & sTransferFileName & "]...")
                Return EkNakCauseCode.None

            Case Else
                Debug.Fail("This case is impermissible.")
                Return EkNakCauseCode.NotPermit
        End Select
    End Function

    '�󓮓IULL�̓]���J�nREQ�d���ɑ����]���I��REQ�d���𐶐����郁�\�b�h
    Protected Overrides Function CreatePassiveUllContinuousReqTelegram(ByVal iXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
                Dim replyLimitTicks As Integer
                SyncLock oForm.UiState
                    replyLimitTicks = oForm.UiState.PassiveUllFinishReplyLimitTicks
                End SyncLock
                Return oXllReqTeleg.CreateContinuousTelegram(cc, 0, replyLimitTicks)

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select

        Return Nothing
    End Function

    '�󓮓IULL�����������i�]���I��REQ�d���ɑ΂���ACK�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnPassiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iAckTeleg As IXllTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Log.Info("PassiveUll completed.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '�󓮓IULL�ɂē]���I��REQ�d���ɑ΂���NAK�d������M�����ꍇ
    Protected Overrides Sub ProcOnPassiveUllFinalizeError(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Log.Error("PassiveUll failed by finalize error.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '�󓮓IULL�ɂē]�������s�����iContinueCode.Abort�̓]���I��REQ�d���𑗐M���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Sub ProcOnPassiveUllTransferError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Log.Error("PassiveUll failed by transfer error.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '�󓮓IULL�̍Œ���L���[�C���O���ꂽ�󓮓IULL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnPassiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Log.Error("PassiveUll failed.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '�w�b�_���̓��e���E�H�b�`�h�b�OREQ�d���̂��̂ł��邩���肷�郁�\�b�h
    Protected Overrides Function IsWatchdogReq(ByVal iTeleg As ITelegram) As Boolean
        If Config.EnableWatchdog = False Then
            Return False
        End If

        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oTeleg.ObjCode = ObjCodeForWatchdogIn(Config.AplProtocol) Then
            Return True
        Else
            Return False
        End If
    End Function

    '�n���ꂽ�d���C���X�^���X��K�؂Ȍ^�̃C���X�^���X�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsWatchdogReq(ByVal iTeleg As ITelegram) As IWatchdogReqTelegram
        Return New EkWatchdogReqTelegram(iTeleg)
    End Function

    '�e�X���b�h����R�l�N�V�������󂯎�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionAppear()
        reqNumberForNextSnd = 0
        enableCommunication = True

        'FTP�Ŏg���ꎞ��Ɨp�f�B���N�g��������������B
        Log.Info("Initializing directory [" & sPermittedPath & "]...")
        Utility.DeleteTemporalDirectory(sPermittedPath)
        Directory.CreateDirectory(sPermittedPath)

        Dim automaticComStart As Boolean
        SyncLock oForm.UiState
            automaticComStart = oForm.UiState.AutomaticComStart
        End SyncLock

        If automaticComStart Then
            enableActiveOneOrdering = True

            If Config.AplProtocol = EkAplProtocol.Tokatsu Then
                Dim sSeqName As String = "TimeDataGet #" & traceNumberForTimeDataGet.ToString()
                UpdateTraceNumberForTimeDataGet()

                Log.Info("Register " & sSeqName & " as ActiveOne.")

                Dim oReqTeleg As New EkTimeDataGetReqTelegram( _
                   oTelegGene, _
                   ObjCodeForTimeDataGetIn(Config.AplProtocol),
                   Config.TimeDataGetReplyLimitTicks)

                RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
            Else
                Dim sSeqName As String = "ComStart #" & traceNumberForComStart.ToString()
                UpdateTraceNumberForComStart()

                Log.Info("Register " & sSeqName & " as ActiveOne.")

                Dim oReqTeleg As New EkComStartReqTelegram( _
                   oTelegGene, _
                   ObjCodeForComStartIn(Config.AplProtocol),
                   Config.ComStartReplyLimitTicks)

                RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
            End If
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

    'NOTE: �ȉ���4���\�b�h�ōs���Ă���FTP����M�t�@�C����sCapDirPath�ւ�
    '�ۑ��́AActiveXllWorker��PassiveXllWorker��FtpWorker����h��������
    'MyFtpWorker�̃C���X�^���X�Ƃ��ėp�ӂ��A����炪��܂���X���b�h�ɂāA
    'FTP�����̃^�C�~���O�ŁiTelegrapher��Response��ԐM����O�Ɂj
    '���{��������A�o�����ɂ�����@�B�I�L�^�Ƃ����ړI�ɍ��v����(*1)�B
    '�������A�����FtpWorker�́A�I�[�o���C�h�\�ȃ��\�b�h�ł���
    'ProcOnDownloadRequestReceive��ProcOnUploadRequestReceive��
    '�����āA�uFTP�̎��{�v�ƁuTelegrapher�ւ�Response�ԐM�v��
    '���ڎ������Ă��邽�߁AMyFtpWorker�ł������I�[�o���C�h���āA
    '�uFTP�̎��{�v�ƁuTelegrapher�ւ�Response�ԐM�v�̊Ԃ�
    '�J�X�^���ȏ�����ǉ�����ꍇ�A�uFTP�̎��{�v�Ȃǂ܂�
    '���O�Ŏ�������K�v�������Ă��܂��B����́A���܂�ɂ����ʂ�
    '����iIXllWorker��Implements����N���X��V�K�ɗp�ӂ���̂�
    '�卷���Ȃ��j���߁AFtpWorker�����t�@�N�^�����O�\�ȋ@�����
    '�܂ł́A�ȉ���4���\�b�h�ŕۑ����s�����Ƃɂ����B
    '�Ȃ��AActiveXllWorker��PassiveXllWorker�ŕۑ����s���悤�ɂ���
    '�ꍇ�́A������2�X���b�h�ɂ�����u���݂��Ȃ��t�@�C�����̌����`
    '�t�@�C���̍쐬�v�Ń��[�X�R���f�B�V�������������Ȃ��悤�A
    'ActiveXllWorker�Ő�������t�@�C����PassiveXllWorker�Ő�������
    '�t�@�C���ɂ́A�ʂ̖����K����K�p����ׂ��iCapDataPath�ɂ�����
    'TransKind������ʂ̕����ɂ���ׂ��j�ł���B
    '*1 ���ہATelegrapher�́AWorker�Ɉ˗������d���������Ԃ�Disconnect()��
    '�s�����ꍇ�AWorker��CancelTransfer()��������AWorker�����Response��
    '�҂��̂́A������ȉ���4���\�b�h�ɓn�����Ƃ͂��Ȃ��B����āA�ȉ���
    '4���\�b�h���I�[�o�[���C�h����������ƁA���Ƃ�Worker�ɂ�����
    'FTP���������Ă����Ƃ��Ă��A���Y�t�@�C���͕ۑ����Ȃ����ƂƂȂ�B

    Protected Overrides Function ProcOnActiveDownloadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If DownloadResponse.Parse(oRcvMsg).Result = DownloadResult.Finished Then
            Dim capRcvFiles As Boolean
            SyncLock oForm.UiState
                capRcvFiles = oForm.UiState.CapRcvFiles
            End SyncLock

            If capRcvFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oActiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnActiveDownloadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnActiveUploadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If UploadResponse.Parse(oRcvMsg).Result = UploadResult.Finished Then
            Dim capSndFiles As Boolean
            SyncLock oForm.UiState
                capSndFiles = oForm.UiState.CapSndFiles
            End SyncLock

            If capSndFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oActiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnActiveUploadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnPassiveDownloadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If DownloadResponse.Parse(oRcvMsg).Result = DownloadResult.Finished Then
            Dim capRcvFiles As Boolean
            SyncLock oForm.UiState
                capRcvFiles = oForm.UiState.CapRcvFiles
            End SyncLock

            If capRcvFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oPassiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnPassiveDownloadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnPassiveUploadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If UploadResponse.Parse(oRcvMsg).Result = UploadResult.Finished Then
            Dim capSndFiles As Boolean
            SyncLock oForm.UiState
                capSndFiles = oForm.UiState.CapSndFiles
            End SyncLock

            If capSndFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oPassiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnPassiveUploadResponseReceive(oRcvMsg)
    End Function
#End Region

#Region "�C�x���g���������p���\�b�h"
    Protected Function SendNakTelegram(ByVal cause As NakCauseCode, ByVal oSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As ITelegram = oSourceTeleg.CreateNakTelegram(cause)
        If oReplyTeleg IsNot Nothing Then
            Log.Info("Sending NAK (" & cause.ToString() & ") telegram...")
            Return SendReplyTelegram(oReplyTeleg, oSourceTeleg)
        Else
            Return False
        End If
    End Function

    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        oReqTeleg.ReqNumber = reqNumberForNextSnd
        oReqTeleg.ClientCode = selfEkCode
        Dim ret As Boolean = MyBase.SendReqTelegram(oReqTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReqTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

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
        Dim ret As Boolean = MyBase.SendReplyTelegram(oReplyTeleg, oSourceTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReplyTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return ret
    End Function

    'NAK�d���𑗐M����ꍇ���M�����ꍇ�̂��̌�̋��������߂邽�߂̃��\�b�h
    Protected Overrides Function GetRequirement(ByVal oNakTeleg As INakTelegram) As NakRequirement
        'TODO: �f�[�^��ʂȂǂł����򂵂Ă����΁A�قƂ�ǂ̃P�[�X��
        '�v���g�R���ᔽ�Ƃ݂Ȃ��āANakRequirement.DisconnectImmediately
        '��ԋp���邱�ƂɂȂ�͂��B
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

    Protected Sub UpdateTraceNumberForComStart()
        If traceNumberForComStart >= 999 Then
            traceNumberForComStart = 0
        Else
            traceNumberForComStart += 1
        End If
    End Sub

    Protected Sub UpdateTraceNumberForTimeDataGet()
        If traceNumberForTimeDataGet >= 999 Then
            traceNumberForTimeDataGet = 0
        Else
            traceNumberForTimeDataGet += 1
        End If
    End Sub

    Protected Sub UpdateTraceNumberForActiveOne()
        If traceNumberForActiveOne >= 999 Then
            traceNumberForActiveOne = 0
        Else
            traceNumberForActiveOne += 1
        End If
    End Sub
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
