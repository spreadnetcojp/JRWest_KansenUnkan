' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �m�ԂƓd���̑���M���s���N���X�B
''' </summary>
Public Class MyTelegrapher
    Inherits ServerTelegrapher

#Region "�����N���X��"
    '�ʐM�t�F�[�Y�̒�`
    Protected Enum ComPhase As Integer
        NoSession
        Idling
    End Enum
#End Region

#Region "�萔��ϐ�"
    '�W�v�J�n�V�[�P���X�ԍ��t�@�C���̖��O
    Protected Const sSeqNumberFileName As String = "InitialSeqNumber.txt"

    '�W�M�n�V�[�P���X�ő��M����f�[�^�̃T�u�w�b�_���̒���
    Protected Const subHeaderLen As Integer = 28

    '�T�u�w�b�_�ɂ�����쐬�����̈ʒu�Ə���
    Protected Const timestampPosInSubHeader As Integer = 0
    Protected Const timestampLenInSubHeader As Integer = 14
    Protected Const sTimestampFormatInSubHeader As String = "yyyyMMddHHmmss"

    '�T�u�w�b�_�ɂ�����V�[�P���X�ԍ��̈ʒu�Ə���
    Protected Const seqNumberPosInSubHeader As Integer = 14
    Protected Const seqNumberLenInSubHeader As Integer = 6

    '�T�u�w�b�_�ɂ�����f�[�^�����̈ʒu�Ə���
    Protected Const recCountPosInSubHeader As Integer = 20
    Protected Const recCountLenInSubHeader As Integer = 8

    '���؃f�[�^�̏���
    Protected Const summaryLen As Integer = 128
    Protected Const summaryKindPos As Integer = 0
    Protected Const summaryTimestampPos As Integer = 1
    Protected Const summaryFormatIdPos As Integer = 14
    Protected Const summarySentCountPos As Integer = 15
    Protected Const summaryKindValue As Byte = &HD1
    Protected Const summaryFormatIdValue As Byte = &H1

    '�V�[�P���X�ԍ�
    'TODO: ���Z�b�g����^�C�~���O�i���Z�b�g���Ȃ� or ���؂��� or �R�l�N�V�������Ɓj��
    '�C���N�������g�̏����i�ے艞������M�����Ƃ��̓C���N�������g���Ȃ� or ����j�A
    '�t�^�Ώہi�d������ or ���p�f�[�^�̃��R�[�h���� or ���؃f�[�^���܂߂����R�[�h���Ɓj�A
    '0�͂��܂肩1�͂��܂肩�A�ے艞�����󂯂ăC���N�������g���Ȃ��ꍇ�A�ʂ̃f�[�^��擪��
    '�������R�[�h�̉�𓯂��ԍ��ő��t���Ă悢���ۂ�...���s���B
    Protected Const seqNumberMax As Integer = 999999
    Protected seqNumber As Integer

    '�ʐM�t�F�[�Y
    Protected curComPhase As ComPhase

    '�m�Ԃ̃A�h���X�R�[�h
    Protected nkanEkCode As EkCode

    '�S���w�̃A�h���X�R�[�h
    Protected selfEkCode As EkCode

    '���X�j���O�\�P�b�g
    Protected oListenerSock As Socket

    '�o�^�ςݗ��p�f�[�^�i�[�f�B���N�g���̃p�X
    Protected sInputDirPath As String

    '�W�v�����p�f�[�^�i�[�f�B���N�g���̃p�X
    Protected sTallyingDirPath As String

    '�W�v�ςݗ��p�f�[�^�f�B���N�g���ړ���f�B���N�g���̃p�X
    Protected sTrashDirPath As String

    '���؃f�[�^���M�v��
    Protected needToSendSummaryData As Boolean

    '���M���̗��p�f�[�^�̃p�X�ƃ��R�[�h��
    Protected oSendingRiyoFilePathList As List(Of String)
    Protected sendingRiyoRecCount As Integer

    'NOTE: �u�Ӑ}�I�Ȑؒf�v�Ɓu�ُ�ɂ��ؒf�v����ʂ������Ȃ�΁A
    'Protected needConnection As Boolean��p�ӂ��A
    'ProcOnComStartReqTelegramReceive()��ProcOnComStopReqTelegramReceive()�ɂ�
    '�����ON/OFF����Ƃ悢�BProcOnConnectionDisappear()�ł́A������݂āA
    '�J�ڐ�̉����Ԃ����߂邱�Ƃ��ł���B

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�̏d���o�^�֎~���Ԃ��ۂ�
    Protected hidesLineErrorFromRecording As Boolean

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�̏d���o�^�֎~���Ԃ�����^�C�}
    Protected oLineErrorRecordingIntervalTimer As TickTimer

    '����ڑ��^�C�}
    'NOTE: �ʐM�ُ�����o���邽�߂̃^�C�}�ł���B
    '��x�ł��ڑ�����΁A�ؒf���ɒʐM�ُ�ƔF���ł��邽�߁A
    '���삳����K�v�͂Ȃ��Ȃ�B
    Protected oInitialConnectLimitTimerForLineError As TickTimer

    '������
    Private _LineStatus As Integer
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal oTelegImporter As NkTelegramImporter, _
       ByVal selfEkCode As EkCode, _
       ByVal oListenerSock As Socket)

        MyBase.New(sThreadName, oParentMessageSock, oTelegImporter)

        Me.curComPhase = ComPhase.NoSession
        Me.nkanEkCode.RailSection = &HFF
        Me.nkanEkCode.StationOrder = &HFF
        Me.selfEkCode = selfEkCode
        Me.oListenerSock = oListenerSock
        Me.RegisterSocket(oListenerSock)

        Dim sBaseDirPath As String = Utility.CombinePathWithVirtualPath(Config.RiyoDataDirPath, selfEkCode.ToString(Config.RiyoDataStationBaseDirNameFormat))
        '-------Ver0.1 ������ԕ�Ή� MOD START-----------
        Me.sInputDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataOutputDirPathInStationBase)
        '-------Ver0.1 ������ԕ�Ή� MOD END-------------
        Me.sTallyingDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataTallyingDirPathInStationBase)
        Me.sTrashDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataTrashDirPathInStationBase)
        Me.needToSendSummaryData = False
        Me.oSendingRiyoFilePathList = Nothing
        Me.sendingRiyoRecCount = 0
        Me.hidesLineErrorFromRecording = False
        Me.oLineErrorRecordingIntervalTimer = New TickTimer(Config.LineErrorRecordingIntervalTicks)
        Me.oInitialConnectLimitTimerForLineError = New TickTimer(Config.InitialConnectLimitTicksForLineError)
        Me.LineStatus = LineStatus.Initial

        Me.oWatchdogTimer.Renew(If(Config.TelegrapherPendingLimitTicks >= 8, Config.TelegrapherPendingLimitTicks \ 8, 1))
        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB

        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite

        '�o�^�ςݗ��p�f�[�^�i�[�f�B���N�g�����Ȃ���΁A�쐬���Ă����B
        Directory.CreateDirectory(sInputDirPath)

        '�W�v�ςݗ��p�f�[�^�i�[�f�B���N�g�����Ȃ���΁A�쐬���Ă����B
        Directory.CreateDirectory(sTrashDirPath)

        '�W�v�����p�f�[�^�i�[�f�B���N�g�����Ȃ���΁A�쐬���Ă����B
        If Not Directory.Exists(sTallyingDirPath) Then
            Directory.CreateDirectory(sTallyingDirPath)
            Me.seqNumber = -1
        Else
            Me.seqNumber = GetNextSeqNumber(sTallyingDirPath)
        End If

        '�W�v�����p�f�[�^�i�[�f�B���N�g���ɊJ�n�ʔԃt�@�C�����Ȃ���΁A�쐬���Ă����B
        If Me.seqNumber = -1 Then
            Dim oLatestBackDirInfo As DirectoryInfo = TimestampedDirPath.FindLatest(sTrashDirPath)
            If oLatestBackDirInfo IsNot Nothing Then
                Dim number As Integer = GetNextSeqNumber(oLatestBackDirInfo.FullName)
                If number >= 0 Then
                    'NOTE: �W�v�ς݂̃f�B���N�g���ɂȂ��Ă���ȏ�A
                    '���؃f�[�^�̑��M���s���Ă�����̂Ƃ݂Ȃ��A
                    '���̕����C���N�������g����B
                    number = GetNextSeqNumber(number, 1)
                    Log.Warn("Restoring seq number to [" & number.ToString() & "]...")
                    SetInitialSeqNumber(sTallyingDirPath, number)
                    Me.seqNumber = GetNextSeqNumber(sTallyingDirPath)
                End If
            Else
                SetInitialSeqNumber(sTallyingDirPath, 0)
                Me.seqNumber = GetNextSeqNumber(sTallyingDirPath)
            End If
        End If

        '�W�v�����p�f�[�^�i�[�f�B���N�g���ɐ������ǂ߂Ȃ��J�n�ʔԃt�@�C�����������ꍇ��A
        '�J�n�ʔԃt�@�C�����쐬�ł��Ȃ������ꍇ
        If Me.seqNumber < 0 Then
            'TODO: �^�p�I�ɂǂ�����̂���Ԃ悢���m�F����B
            SetInitialSeqNumber(sTallyingDirPath, 0)
            Me.seqNumber = GetNextSeqNumber(sTallyingDirPath)
            If Me.seqNumber < 0 Then
                Application.Exit()
            End If
        End If
    End Sub
#End Region

#Region "�v���p�e�B"
    '���̃v���p�e�B�́A�e�X���b�h�ɂ����Ă��A����u�ԁi�����ߋ��̏u�ԁj��
    '�����Ԃ����[�U�ɕ\������ړI�ł���΁A�Q�Ɖ\�ł���B
    '���̃X���b�h�����̉ӏ��Œ�~��������ŏ�Ԃ��擾����i���̌�A����
    '�X���b�h�ɑ΂��ĔC�ӂ̑�����s���Ă���A�C�ӂɍĊJ�����邱�Ƃ��ł���j
    '�킯�ł͂Ȃ����߁A�Ăяo������߂������_�ŁA�߂�l�̉����Ԃ��ێ�
    '����Ă���Ƃ͌���Ȃ��B���̂����A���p�x�ŌĂяo���Ă��債�����ׂ�
    '������Ȃ��B
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

        Protected Set(ByVal status As LineStatus)
            Interlocked.Exchange(_LineStatus, status)
        End Set
    End Property
#End Region

#Region "�e�X���b�h�p���\�b�h"
    Public Overrides Sub Start()
        Dim systemTick As Long = TickTimer.GetSystemTick()
        RegisterTimer(oInitialConnectLimitTimerForLineError, systemTick)

        MyBase.Start()
    End Sub
#End Region

#Region "�C�x���g�������\�b�h"
    Protected Overrides Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oLineErrorRecordingIntervalTimer Then
            Return ProcOnLineErrorRecordingTime()
        End If

        If oTimer Is oInitialConnectLimitTimerForLineError Then
            Return ProcOnInitialConnectLimitTimeForLineError()
        End If

        Return MyBase.ProcOnTimeout(oTimer)
    End Function

    Protected Overridable Function ProcOnLineErrorRecordingTime() As Boolean
        Log.Info("Line error recording time comes.")

        If LineStatus = LineStatus.Steady Then
            hidesLineErrorFromRecording = False
        Else
            '���W�f�[�^��L�e�[�u���ɒʐM�ُ��o�^����B
            'NOTE: ���W�f�[�^��L�e�[�u���ł́A�V���Ȉُ�̓o�^���������Ƃ�
            '�ȂāA�ُ킪���������Ƃ݂Ȃ����ƂɂȂ邽�߁A�ʐM�ُ킪�������Ă���
            '����́A����I�ɐV���Ȉُ��o�^���Ȃ���΂Ȃ�Ȃ��B
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            Debug.Assert(hidesLineErrorFromRecording = True)
        End If
        Return True
    End Function

    Protected Overridable Function ProcOnInitialConnectLimitTimeForLineError() As Boolean
        Log.Error("Initial connection limit time comes for line error.")

        If Not hidesLineErrorFromRecording Then
            '���W�f�[�^��L�e�[�u���ɒʐM�ُ��o�^����B
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromRecording = True
        End If
        Return True
    End Function

    Protected Overrides Function ProcOnSockReadable(ByVal oSock As Socket) As Boolean
        If oSock Is oListenerSock Then
            If curComPhase <> ComPhase.NoSession Then
                Disconnect()
                Debug.Assert(curComPhase = ComPhase.NoSession)
            End If

            Dim oNewSocket As Socket = Nothing
            Try
                oNewSocket = SockUtil.Accept(oListenerSock)
            Catch ex As OPMGException
                'NOTE: ���ۂ̂Ƃ���͂Ƃ������A���X�j���O�\�P�b�g���ǂݏo���\
                '�ɂȂ�������Ƃ����āA���������Accept()����������Ƃ͌���Ȃ�
                '�ilinux�̃\�P�b�g�̂悤�ɁAAccept()���Ăяo���܂ł̊Ԃɔ���
                '�����R�l�N�V�����ُ̈킪�AAccept()�Œʒm�����\��������j
                '���̂Ƃ݂Ȃ��B
                Log.Error("Exception caught.", ex)
            End Try

            If oNewSocket IsNot Nothing Then
                Dim oRemoteEndPoint As IPEndPoint = DirectCast(oNewSocket.RemoteEndPoint, IPEndPoint)
                Dim oRemoteIPAddr As IPAddress = oRemoteEndPoint.Address
                Log.Info("Incoming from [" & oRemoteEndPoint.Address.ToString() & "].")
                Connect(oNewSocket)

                'NOTE: �ȍ~�A�J�Ǘv���𖳊����ő҂�������B
                'oListenerSock����������Ȃ���΂Ȃ�Ȃ����u���P��ł���́A
                '���݂̐ڑ��ς݃\�P�b�g���c�葱����ɂ��Ă��A���̐ڑ��ς�
                '�\�P�b�g�����^�C�~���O�ŉ�����邱�ƂɂȂ�i���\�[�X
                '�g�p�ʂ̒P�������͂Ȃ��j���ߖ��Ȃ��B
                '�^�ǃV�X�e�����ɂ����āA�ʐM�ُ�����[�U�ɒm�点��ׂ���
                '����ɂ��Ă��A�J�Ǘv���҂��^�C���A�E�g�̂悤�Ȏd�g�݂�
                '����������K�v�͂Ȃ��B���̃X���b�h���N������ł���΁A
                'oInitialConnectLimitTimerForLineError�����삵�Ă���͂��ł���A
                '�J�Ǘv�����Ȃ���΁A���̃^�C�}���^�C���A�E�g���āA
                'DB�Ɉُ��o�^���邱�ƂɂȂ�B���̃X���b�h���N������
                '�łȂ��ꍇ�́AoInitialConnectLimitTimerForLineError��
                '�^�C���A�E�g�������܂���Disconnect()���s���ɂ����āA
                '����DB�Ɉُ킪�o�^����Ă���͂��ł���A���̌���A
                'LineStatus��Initial�܂���Disconnected�ł��邱�Ƃɂ��A
                '����I�Ɉُ�̓o�^���J��Ԃ����͂��ł���B
            End If

            Return True
        End If

        Return MyBase.ProcOnSockReadable(oSock)
    End Function

    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case ServerAppInternalMessageKind.TallyTimeNotice
                Return ProcOnTallyTimeNoticeReceive(oRcvMsg)
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
    End Function

    Protected Overridable Function ProcOnTallyTimeNoticeReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Tally time notified by manager.")
        needToSendSummaryData = True
        Return True
    End Function

    Protected Overrides Function ProcOnQuitRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Dim toBeContinued As Boolean = MyBase.ProcOnQuitRequestReceive(oRcvMsg)
        If Not toBeContinued Then
            UnregisterSocket(oListenerSock)
        End If
        Return toBeContinued
    End Function

    Protected Overrides Function ProcOnReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As NkTelegram = DirectCast(iRcvTeleg, NkTelegram)
        If oRcvTeleg.DstEkCode <> selfEkCode Then
            Log.Error("Telegram with invalid DstEkCode received.")
            Disconnect()
            Return True
        Else
            Return MyBase.ProcOnReqTelegramReceive(oRcvTeleg)
        End If
    End Function

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As NkTelegram = DirectCast(iRcvTeleg, NkTelegram)
        Select Case oRcvTeleg.SeqCode
            Case NkSeqCode.Collection, NkSeqCode.Delivery
                Select Case oRcvTeleg.CmdCode
                    Case NkCmdCode.ComStartReq
                        Return ProcOnComStartReqTelegramReceive(oRcvTeleg)
                    Case NkCmdCode.ComStopReq
                        Return ProcOnComStopReqTelegramReceive(oRcvTeleg)
                    Case NkCmdCode.InquiryReq
                        Return ProcOnInquiryReqTelegramReceive(oRcvTeleg)
                    Case Else
                        Log.Error("Telegram with invalid CmdCode received.")
                        Disconnect()
                        Return True
                End Select

            Case NkSeqCode.Test
                Select Case oRcvTeleg.CmdCode
                    Case NkCmdCode.InquiryReq
                        Return ProcOnTestReqTelegramReceive(oRcvTeleg)
                    Case Else
                        Log.Error("Test sequence telegram with invalid CmdCode received.")
                        Disconnect()
                        Return True
                End Select

            Case Else
                Log.Error("Telegram with invalid SeqCode received.")
                Disconnect()
                Return True
        End Select

        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnComStartReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkComStartReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("ComStart REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        If oRcvTeleg.SeqCode <> NkSeqCode.Collection Then
            Log.Error("ComStart REQ with invalid SeqCode received.")
            Disconnect()
            Return True
        End If

        If curComPhase <> ComPhase.NoSession Then
            Log.Error("ComStart REQ received in disproportionate phase.")
            Disconnect()
            Return True
        End If

        Log.Info("ComStart REQ received.")

        Dim oReplyTeleg As NkComStartAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ComStart ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        UnregisterTimer(oInitialConnectLimitTimerForLineError)
        curComPhase = ComPhase.Idling
        LineStatus = LineStatus.Steady
        Return True
    End Function

    Protected Overridable Function ProcOnComStopReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkComStopReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("ComStop REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        If curComPhase <> ComPhase.Idling Then
            Log.Warn("ComStop REQ received in disproportionate phase.")
        Else
            Log.Info("ComStop REQ received.")
        End If

        Dim oReplyTeleg As NkComStopAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ComStop ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Disconnect()
        Return True
    End Function

    Protected Overridable Function ProcOnInquiryReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkInquiryReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Inquiry REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        If oRcvTeleg.SeqCode = NkSeqCode.Collection Then
            If curComPhase <> ComPhase.Idling Then
                Log.Warn("CollectionInquiry REQ received in disproportionate phase.")
            Else
                Log.Info("CollectionInquiry REQ received.")
            End If

            If needToSendSummaryData Then
                Log.Info("It's now time to send the summary data.")

                '����M�ۉ����i�j��ԐM����B
                Dim oReplyTeleg As NkInquiryAckTelegram = oRcvTeleg.CreateAckTelegram(0)
                Log.Info("Sending CollectionInquiry ACK with ReturnStatus OK...")
                If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
                    Disconnect()
                    Return True
                End If

                'NOTE: �ȉ��̎��{�r���Ŕz�M�n�V�[�P���X��܂�Ԃ��V�[�P���X�̗v���d����
                '��M���邱�Ƃ͑z�肵�Ă��Ȃ����߁ARegisterActiveOne()���g�����Ƃ͕K�{�ł͂Ȃ��B
                '�������ARegisterActiveOne()���g�p���������������V���v���ɂȂ邽�߁A
                'RegisterActiveOne()���g�p����B

                Dim contentsLength As Long = 0
                If Directory.Exists(sTallyingDirPath) Then
                    contentsLength = UpboundDataPath.GetContentsLength(sTallyingDirPath)
                End If

                'NOTE: ��M���̃`�F�b�N��riyoRecordLen�̔{���̂͂��ł��邽�߁A
                '�����ŗ]�肪��������Ƃ��ɂǂ�����ׂ����i�ǂ��Ȃ邩�j�́A
                '�l���Ȃ����Ƃɂ���B
                Dim sentCount As Integer = CInt(contentsLength \ EkConstants.RiyoDataRecordLen)

                '���؃f�[�^���쐬����B
                Dim aSummaryHeader As Byte() = CreateSubHeader(1)
                Dim aSummaryRecord As Byte() = CreateSummaryRecord(sentCount)
                Dim aSummary(aSummaryHeader.Length + aSummaryRecord.Length - 1) As Byte
                Buffer.BlockCopy(aSummaryHeader, 0, aSummary, 0, aSummaryHeader.Length)
                Buffer.BlockCopy(aSummaryRecord, 0, aSummary, aSummaryHeader.Length, aSummaryRecord.Length)
                Dim oDataPostReqTeleg As New NkDataPostReqTelegram(NkSeqCode.Collection, aSummary, Config.SummaryDataReplyLimitTicks)

                Log.Info("Register SummaryDataPost REQ as ActiveOne.")
                RegisterActiveOne(oDataPostReqTeleg, 0, 1, 1, "SummaryDataPost")
            Else
                '�o�^�ςݗ��p�f�[�^���������āA����M�ۉ����̃��^�[���X�e�[�^�X�����肷��B
                '���M����t�@�C�����̃��X�g�́A��������ɕێ����Ă����B
                Dim totalLen As Long = 0
                oSendingRiyoFilePathList = UpboundDataPath.FindFullNames(sInputDirPath, totalLen, 4294967295 - subHeaderLen)
                If oSendingRiyoFilePathList.Count = 0 Then
                    '����M�ۉ����i�ہj��ԐM����B
                    Dim oNegativeReplyTeleg As NkInquiryAckTelegram = oRcvTeleg.CreateAckTelegram(1)
                    Log.Info("Sending CollectionInquiry ACK with ReturnStatus NoData...")
                    If SendReplyTelegram(oNegativeReplyTeleg, oRcvTeleg) = False Then
                        Disconnect()
                        Return True
                    End If

                    Return True
                End If

                '����M�ۉ����i�j��ԐM����B
                Dim oReplyTeleg As NkInquiryAckTelegram = oRcvTeleg.CreateAckTelegram(0)
                Log.Info("Sending CollectionInquiry ACK with ReturnStatus OK...")
                If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
                    Disconnect()
                    Return True
                End If

                'NOTE: �ȉ��̎��{�r���Ŕz�M�n�V�[�P���X��܂�Ԃ��V�[�P���X�̗v���d����
                '��M���邱�Ƃ͑z�肵�Ă��Ȃ����߁ARegisterActiveOne()���g�����Ƃ͕K�{�ł͂Ȃ��B
                '���ɁA��M�����Ƃ���ŁA���ɗ��p�f�[�^��r���̃o�C�g�܂ő��M���Ă���ȏ�A
                '�������ɉ�����ԐM���邱�Ƃ����ʂɕs�\�ł��邩��A�v���g�R�����
                '�s���ɂ�����Ǝv����B�������ARegisterActiveOne()���g�p��������
                '�������V���v���ɂȂ邽�߁ARegisterActiveOne()���g�p����B

                'NOTE: ��M���̃`�F�b�N��riyoRecordLen�̔{���̂͂��ł��邽�߁A
                '�����ŗ]�肪��������Ƃ��ɂǂ�����ׂ����i�ǂ��Ȃ邩�j�́A
                '�l���Ȃ����Ƃɂ���B

                '���p�f�[�^���쐬����B
                sendingRiyoRecCount = CInt(totalLen \ EkConstants.RiyoDataRecordLen)
                Dim aSubHeader As Byte() = CreateSubHeader(sendingRiyoRecCount)
                Dim oDataPostReqTeleg As New NkDataPostReqTelegram(NkSeqCode.Collection, aSubHeader, oSendingRiyoFilePathList, totalLen, Config.RiyoDataReplyLimitTicks)

                Log.Info("Register RiyoDataPost REQ as ActiveOne.")
                RegisterActiveOne(oDataPostReqTeleg, 0, 1, 1, "RiyoDataPost")
            End If
        Else
            Log.Info("DeriveryInquiry REQ received.")

            '����M�ۉ����i�ہj��ԐM����B
            Dim oReplyTeleg As NkInquiryAckTelegram = oRcvTeleg.CreateAckTelegram(1)
            Log.Info("Sending DeriveryInquiry ACK with ReturnStatus NG...")
            If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
                Disconnect()
                Return True
            End If
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnTestReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkTestReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Test REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Log.Info("Test REQ received.")

        '�񓚃��X�|���X��ԐM����B
        Dim oReplyTeleg As NkTestAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending Test ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    '�\���I�P���V�[�P���X�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        Dim oReqTeleg As NkDataPostReqTelegram = DirectCast(iReqTeleg, NkDataPostReqTelegram)
        Dim oAckTeleg As NkDataPostAckTelegram = DirectCast(iAckTeleg, NkDataPostAckTelegram)
        Dim returnStatus As UShort = oAckTeleg.ReturnStatus

        '���M�����f�[�^�̎�ʂ𔻕ʂ���B
        'NOTE: �{���́A�f�[�^���擪�t�߂̃f�[�^��ʂ��݂Ĕ��ʂ���̂����R�ł��邪�A
        '�����ʂ̊֌W�ŁA���L�̂悤�ɂ��Ă���B
        '�{���́AGetNextSeqNumber�ɓn�����M���R�[�h����oReqTeleg�̃T�u�w�b�_������
        '�擾���������悢�B
        If oReqTeleg.ObjSize = subHeaderLen + summaryLen Then
            '���؃f�[�^�𑗐M�����ꍇ�ł���B

            '�񓚃��X�|���X�̃��^�[���X�e�[�^�X���u����v�Ȃ�A�O���̏W�v���I������B
            If returnStatus = 0 Then
                Log.Info("SummaryDataPost ACK with ReturnStatus OK received.")

                '�W�v���������f�B���N�g�����W�v�ς݃f�B���N�g���̉��Ɉړ�
                Dim sNewDirPath As String = TimestampedDirPath.Gen(sTrashDirPath, EkServiceDate.Gen(DateTime.Now.AddDays(-1)))
                Directory.Move(sTallyingDirPath, sNewDirPath)

                seqNumber = GetNextSeqNumber(seqNumber, 1)

                '�����̏W�v�ɔ�����B
                Directory.CreateDirectory(sTallyingDirPath)
                SetInitialSeqNumber(sTallyingDirPath, seqNumber)
            Else
                Log.Error("SummaryDataPost ACK with ReturnStatus NG(" & returnStatus.ToString() & ") received.")
            End If

            needToSendSummaryData = False
        Else
            '���p�f�[�^�𑗐M�����ꍇ�ł���B

            '�񓚃��X�|���X�̃��^�[���X�e�[�^�X���u����v�Ȃ�A���M�����t�@�C����
            '�W�v���f�B���N�g���Ɉړ�����B
            If returnStatus = 0 Then
                Log.Info("RiyoDataPost ACK with ReturnStatus OK received.")

                For Each sPath As String In oSendingRiyoFilePathList
                    File.Move(sPath, UpboundDataPath.Gen(sTallyingDirPath, Path.GetFileName(sPath)))
                Next sPath

                seqNumber = GetNextSeqNumber(seqNumber, sendingRiyoRecCount)
            Else
                Log.Error("RiyoDataPost ACK with ReturnStatus NG(" & returnStatus.ToString() & ") received.")
            End If

            oSendingRiyoFilePathList = Nothing
            sendingRiyoRecCount = 0
        End If
    End Sub

    '�\���I�P���V�[�P���X�̍Œ���L���[�C���O���ꂽ�\���I�P���V�[�P���X�̎��{�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        oSendingRiyoFilePathList = Nothing
        sendingRiyoRecCount = 0
    End Sub

    '�R�l�N�V������ؒf�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionDisappear()
        curComPhase = ComPhase.NoSession
        LineStatus = LineStatus.Disconnected

        If Not hidesLineErrorFromRecording Then
            '���W�f�[�^��L�e�[�u���ɒʐM�ُ��o�^����B
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromRecording = True
        End If
    End Sub

    Protected Overrides Sub ProcOnUnhandledException(ByVal ex As Exception)
        MyBase.ProcOnUnhandledException(ex)
        UnregisterSocket(oListenerSock)
        '���̂܂܌Ăь��ɖ߂��āA�X���b�h�͏I����ԂɂȂ�B
    End Sub
#End Region

#Region "�C�x���g���������p���\�b�h"
    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        'NOTE: ���܂��ܗ��p�f�[�^���𑗐M���Ă���Ƃ���TallyTimeNotice��
        '��M����΁A����f���������邱�ƂɂȂ�͂��ł���B
        '���̂��Ƃ��l�������QuitRequest��M��p�\�P�b�g��p�ӂ��āA�����
        'oListenerSock�݂̂�oInterruptSockList��Add����̂����R�ł���B
        '�����A���؏����̎����́A���p�f�[�^���������Ȃ������ɐݒ肷�邱�Ƃ�
        '�Ȃ��Ă���͂��ł���A��L�̂悤�Ȃ��Ƃ��N������̂́A����ȃP�[�X
        '�����l�����Ȃ��B���ꂾ���̂��߂Ƀ\�P�b�g�������̂��ǂ���
        '�Ƃ����C������̂ŁA�v�]������܂ł́A���̂܂܂ɂ��Ă����B
        Dim oInterruptSockList As New ArrayList(2)
        oInterruptSockList.Add(oListenerSock)
        oInterruptSockList.Add(oParentMessageSock)
        Dim oReqTeleg As NkReqTelegram = DirectCast(iReqTeleg, NkReqTelegram)
        oReqTeleg.SrcEkCode = selfEkCode
        oReqTeleg.DstEkCode = nkanEkCode
        Return oReqTeleg.WriteToSocketInterruptible( _
           oTelegSock, _
           oInterruptSockList, _
           telegWritingLimitBaseTicks, _
           telegWritingLimitExtraTicksPerMiB, _
           telegLoggingMaxLengthOnWrite)
    End Function

    Protected Overrides Function SendReplyTelegram(ByVal iReplyTeleg As ITelegram, ByVal iSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        oReplyTeleg.SrcEkCode = selfEkCode
        oReplyTeleg.DstEkCode = nkanEkCode
        Return MyBase.SendReplyTelegram(oReplyTeleg, iSourceTeleg)
    End Function

    Protected Function CreateSubHeader(ByVal recCount As Integer) As Byte()
        'TODO: �C���^�t�F�[�X�d�l���B���Ȃ̂ŁA��ł݂Ȃ����B
        Dim aSubHeader As Byte() = New Byte(subHeaderLen - 1) {}
        Dim sNow As String = DateTime.Now.ToString(sTimestampFormatInSubHeader)
        Encoding.UTF8.GetBytes(sNow, 0, timestampLenInSubHeader, aSubHeader, timestampPosInSubHeader)
        Utility.CopyIntToDecimalAsciiBytes(seqNumber, aSubHeader, seqNumberPosInSubHeader, seqNumberLenInSubHeader)
        Utility.CopyIntToDecimalAsciiBytes(recCount, aSubHeader, recCountPosInSubHeader, recCountLenInSubHeader)
        Return aSubHeader
    End Function

    Protected Function CreateSummaryRecord(ByVal sentCount As Integer) As Byte()
        Dim aSummaryRecord As Byte() = New Byte(summaryLen - 1) {}
        aSummaryRecord(summaryKindPos) = summaryKindValue
        aSummaryRecord(summaryFormatIdPos) = summaryFormatIdValue
        Dim aTimeStamp As Byte() = Utility.CHARtoBCD(DateTime.Now.ToString("yyyyMMddHHmmss"), 7)
        Buffer.BlockCopy(aTimeStamp, 0, aSummaryRecord, summaryTimestampPos, 7)
        Utility.CopyUInt32ToLeBytes4(CUInt(sentCount), aSummaryRecord, summarySentCountPos)
        Return aSummaryRecord
    End Function

    Protected Function SetInitialSeqNumber(ByVal sDirPath As String, ByVal number As Integer) As Boolean
        Dim aSeqNumber(seqNumberLenInSubHeader - 1) As Byte
        Utility.CopyIntToDecimalAsciiBytes(number, aSeqNumber, 0, seqNumberLenInSubHeader)

        Dim oOutputStream As FileStream = Nothing
        Dim sPath As String = Path.Combine(sDirPath, sSeqNumberFileName)
        Try
            oOutputStream = New FileStream(sPath, FileMode.Create, FileAccess.Write)
            oOutputStream.Write(aSeqNumber, 0, seqNumberLenInSubHeader)
        Catch ex As Exception
            Log.Fatal("Create file [" & sPath & "] failed.")
            Return False
        Finally
            If oOutputStream IsNot Nothing Then
                oOutputStream.Close()
            End If
        End Try

        Return True
    End Function

    Protected Function GetInitialSeqNumber(ByVal sDirPath As String) As Integer
        Dim aSeqNumber(seqNumberLenInSubHeader - 1) As Byte

        Dim oInputStream As FileStream = Nothing
        Dim sPath As String = Path.Combine(sDirPath, sSeqNumberFileName)
        Try
            oInputStream = New FileStream(sPath, FileMode.Open, FileAccess.Read)
            oInputStream.Read(aSeqNumber, 0, seqNumberLenInSubHeader)
        Catch ex As FileNotFoundException
            Log.Warn("[" & sPath & "] not found.")
            Return -1
        Catch ex As Exception
            Log.Fatal("[" & sPath & "] is broken.")
            Return -2
        Finally
            If oInputStream IsNot Nothing Then
                oInputStream.Close()
            End If
        End Try

        If Not Utility.IsDecimalAsciiBytesFixed(aSeqNumber, 0, seqNumberLenInSubHeader) Then
            Log.Fatal("[" & sPath & "] is broken.")
            Return -2
        End If

        Return Utility.GetIntFromDecimalAsciiBytes(aSeqNumber, 0, seqNumberLenInSubHeader)
    End Function

    Protected Function GetNextSeqNumber(ByVal sDirPath As String) As Integer
        Dim number As Integer = GetInitialSeqNumber(sDirPath)
        If number < 0 Then Return number

        'NOTE: ��M���̃`�F�b�N��riyoRecordLen�̔{���̂͂��ł��邽�߁A
        '�����ŗ]�肪��������Ƃ��ɂǂ�����ׂ����i�ǂ��Ȃ邩�j�́A
        '�l���Ȃ����Ƃɂ���B
        Dim contentsLength As Long = UpboundDataPath.GetContentsLength(sDirPath)
        Dim recCount As Integer = CInt(contentsLength \ EkConstants.RiyoDataRecordLen)

        Return GetNextSeqNumber(number, recCount)
    End Function

    Protected Function GetNextSeqNumber(ByVal number As Integer, ByVal recCount As Integer) As Integer
        number += recCount
        If number > seqNumberMax Then
            number -= seqNumberMax
        End If
        Return number
    End Function

    Protected Sub InsertLineErrorToCdt()
        Dim now As DateTime = DateTime.Now
        'StartMinutesInDay�ȏ�ɂȂ�悤�ɕ␳����
        '���ݎ������i0��0������̌o�ߕ��̌`���Łj���߂�B
        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
        If Config.LineErrorRecordingStartMinutesInDay > nowMinutesInDay Then
            nowMinutesInDay += 24 * 60
        End If

        '�L�����ԑт̂ݓo�^���s���B
        If nowMinutesInDay <= Config.LineErrorRecordingEndMinutesInDay Then
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, selfEkCode), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtNkanLineError.Gen())
        End If
    End Sub
#End Region

End Class

''' <summary>
''' �����ԁB
''' </summary>
Public Enum LineStatus As Integer
    Initial
    Steady
    Disconnected
End Enum
