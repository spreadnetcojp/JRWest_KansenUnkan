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

Imports System.Globalization
Imports System.IO
Imports System.Messaging
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �w���@��Ɠd���̑���M���s���N���X�B
''' </summary>
Public Class TelServerAppTelegrapher
    Inherits ServerTelegrapher

#Region "���[�e�B���e�B���\�b�h"
    'NOTE: ������ԕ�Ή��ɂāA�e��V�[�P���X�̎d�l�L�q�p�̃N���X�́A
    'Public�ɕύX���āA���̃N���X�̊O�Œ�`����悤�ɂ����B
    '-------Ver0.1 ������ԕ�Ή� DEL START-----------
    '-------Ver0.1 ������ԕ�Ή� DEL END-------------

    Protected Shared Function GenCplxObjCode(ByVal objCode As Integer, ByVal subObjCode As Integer) As UShort
        Return CUShort(objCode << 8 Or subObjCode)
    End Function
#End Region

#Region "�萔��ϐ�"
    '�e��e�[�u�����ʂ̍��ڂɃZ�b�g����l
    Protected Const UserId As String = "System"
    Protected Const MachineId As String = "Server"

    '�X���b�h�ʃf�B���N�g�����̏���
    Protected Const sDirNameFormat As String = "%3R%3S_%4C_%2U"

    '�ꎞ�t�@�C���̖��O
    Protected Const sTempFileName As String = "ReceivedData.bin"

    '�ꎞ��Ɨp�f�B���N�g����
    Protected sTempDirPath As String

    '�d������
    Protected oTelegGene As EkTelegramGene

    '���葕�u�̑��u�R�[�h
    'NOTE: ProcOnReqTelegramReceive()���t�b�N���Ď�M�d����ClientCode�Ɣ�r���Ă��悢�B
    Protected clientCode As EkCode

    '�ʐM����́iDB�d�l�j�@��R�[�h
    Protected sClientModel As String

    '�ʐM����́iDB�d�l�j�R�l�N�V�����敪
    Protected sPortPurpose As String

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    '�ʐM����̉w��
    Protected sClientStationName As String

    '�ʐM����̃R�[�i�[��
    Protected sClientCornerName As String
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------

    '�A�N�Z�X��������FTP�T�C�g�����z�t�@�C���V�X�e���̃p�X
    Protected sPermittedPathInFtp As String

    '�A�N�Z�X�������郍�[�J���t�@�C���V�X�e���̃p�X
    Protected sPermittedPath As String

    '���W�f�[�^��L�e�[�u���ɋL�^���邽�߂̒ʐM����@�햼��
    Protected sCdtClientModelName As String

    '���W�f�[�^��L�e�[�u���ɋL�^����|�[�g����
    Protected sCdtPortName As String

    '�E�H�b�`�h�b�O�̎��
    Protected formalObjCodeOfWatchdog As Integer

    '�����f�[�^�擾�̎��
    Protected formalObjCodeOfTimeDataGet As Integer

    '�}�X�^/�v���O�����ꎮDLL�̎d�l
    Protected oMasProSuiteDllSpecOfDataKinds As Dictionary(Of String, TelServerAppMasProDllSpec)

    '�}�X�^/�v���O�����K�p���X�gDLL�̎d�l
    Protected oMasProListDllSpecOfDataKinds As Dictionary(Of String, TelServerAppMasProDllSpec)

    '�w��t�@�C��ULL�̎d�l
    Protected oScheduledUllSpecOfDataKinds As Dictionary(Of String, TelServerAppScheduledUllSpec)

    '�}�X�^/�v���O����DL�����ʒm�̎d�l
    'NOTE: Key��ObjCode��SubObjCode����GenCplxObjCode���\�b�h�Ő�������B
    '�Ȃ��AObjCode���Ƃ�SubObjCode��0x00�̃��R�[�h��K���p�ӂ��Ȃ���΂Ȃ�Ȃ��B
    '�v���g�R���d�l�ɂ��̂悤��ObjCode��SubObjCode�i0x00�j�̑g�ݍ��킹��
    '���݂��Ȃ��ꍇ�A���ꂪ�_�~�[���R�[�h�ł��邱�Ƃ��킩��悤��Value��
    'DataKind�ɂ�Nothing��ݒ肷�邱�ƁB
    Protected oMasProDlReflectSpecOfCplxObjCodes As Dictionary(Of UShort, TelServerAppMasProDlReflectSpec)

    'POST�d����M�̎d�l
    Protected oByteArrayPassivePostSpecOfObjCodes As Dictionary(Of Byte, TelServerAppByteArrayPassivePostSpec)

    '�o�[�W�������ULL�̎d�l
    Protected oVersionInfoUllSpecOfObjCodes As Dictionary(Of Byte, TelServerAppVersionInfoUllSpec)

    '���p�f�[�^ULL�̎d�l
    Protected oRiyoDataUllSpecOfObjCodes As Dictionary(Of Byte, TelServerAppRiyoDataUllSpec)

    '���p�f�[�^�i�[��f�B���N�g����
    Protected sRiyoDataInputDirPath As String
    Protected sRiyoDataRejectDirPath As String

    '���ݎ��s���̗��p�f�[�^ULL������I������Ɖ��肵���ꍇ�̈ړ���t���p�X��
    'NOTE: PassiveUll�̑��d�󂯓�����s��Ȃ����Ƃ�O��ɂ��Ă���̂Œ��ӁB
    '����ServerTelegrapher����������āA���d�󂯓����e�F����悤�ɂȂ�����A
    '�C�Ӑ��̃t�@�C������oPassiveXllQueue�̊e�A�C�e���ɕR�Â��ĊǗ�����K�v������B
    Protected sCurUllRiyoDataReservedInputPath As String

    '���ɑ��M����REQ�d���̒ʔ�
    Protected reqNumberForNextSnd As Integer

    '���Ɏ�M����REQ�d���̒ʔ�
    'NOTE: ProcOnReqTelegramReceive()���t�b�N���āA��M����REQ�d���̒ʔԂ�
    '�A���������`�F�b�N����Ȃ�p�ӂ���B
    'Protected reqNumberForNextRcv As Integer

    'NOTE: �u�Ӑ}�I�Ȑؒf�v�Ɓu�ُ�ɂ��ؒf�v����ʂ������Ȃ�΁A
    'Protected needConnection As Boolean��p�ӂ��A
    'ProcOnConnectNoticeReceive()��ProcOnDisconnectRequestReceive()���t�b�N����
    '�����ON/OFF����Ƃ悢�BProcOnConnectionDisappear()�ł́A������݂āA
    '�J�ڐ�̉����Ԃ����߂邱�Ƃ��ł���B

    '�^��������
    Protected pseudoLineStatus As LineStatus

    '�^���R�l�N�V�����������Ԃ��ۂ��i�����I�ȈӖ��̓R�l�N�V�����ώ@���Ԃ��ۂ��j
    Protected isPseudoConnectionProlongationPeriod As Boolean

    '�^���R�l�N�V�����������Ԃ�����^�C�}
    Protected oPseudoConnectionProlongationTimer As TickTimer

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�̏d���o�^�֎~���Ԃ��ۂ�
    Protected hidesLineErrorFromRecording As Boolean

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�̏d���o�^�֎~���Ԃ�����^�C�}
    Protected oLineErrorRecordingIntervalTimer As TickTimer

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    '�ʐM�ُ�̌x�񃁁[���̏d�������֎~���Ԃ��ۂ�
    Protected hidesLineErrorFromAlerting As Boolean

    '�ʐM�ُ�̌x�񃁁[���̏d���o�^�֎~���Ԃ�����^�C�}
    Protected oLineErrorAlertingIntervalTimer As TickTimer
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------

    '����ڑ��^�C�}
    'NOTE: �ʐM�ُ�����o���邽�߂̃^�C�}�ł���B
    '��x�ł��ڑ�����΁A�ؒf���ɒʐM�ُ�ƔF���ł��邽�߁A
    '���삳����K�v�͂Ȃ��Ȃ�B
    Protected oInitialConnectLimitTimerForLineError As TickTimer

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    '�ʐM�ُ킪�n�܂�������
    Protected lineErrorBeginingTime As DateTime

    '�ʐM�ُ�̌x�񃁁[���̕���
    Protected lineErrorAlertMailSubject As Sentence
    Protected lineErrorAlertMailBody As Sentence
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------

    '������
    Private _LineStatus As Integer
#End Region

#Region "�R���X�g���N�^"
    '-------Ver0.1 ������ԕ�Ή� MOD START-----------
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal oTelegImporter As EkTelegramImporter, _
       ByVal oTelegGene As EkTelegramGene, _
       ByVal clientCode As EkCode, _
       ByVal sClientModel As String, _
       ByVal sPortPurpose As String, _
       ByVal sCdtClientModelName As String, _
       ByVal sCdtPortName As String, _
       ByVal sClientStationName As String, _
       ByVal sClientCornerName As String, _
       ByVal lineErrorAlertMailSubject As Sentence, _
       ByVal lineErrorAlertMailBody As Sentence)

        MyBase.New(sThreadName, oParentMessageSock, oTelegImporter)
        Me.sTempDirPath = Path.Combine(TelServerAppBaseConfig.TemporaryBaseDirPath, clientCode.ToString(sDirNameFormat))
        Me.oTelegGene = oTelegGene
        Me.clientCode = clientCode
        Me.sClientModel = sClientModel
        Me.sPortPurpose = sPortPurpose
        Me.sClientStationName = sClientStationName
        Me.sClientCornerName = sClientCornerName
        Me.sPermittedPathInFtp = Path.Combine(TelServerAppBaseConfig.PermittedPathInFtp, clientCode.ToString(sDirNameFormat))
        Me.sPermittedPath = Utility.CombinePathWithVirtualPath(TelServerAppBaseConfig.FtpServerRootDirPath, sPermittedPathInFtp)
        Me.sCdtClientModelName = sCdtClientModelName
        Me.sCdtPortName = sCdtPortName
        Me.lineErrorAlertMailSubject = lineErrorAlertMailSubject
        Me.lineErrorAlertMailBody = lineErrorAlertMailBody

        'NOTE: MayOverride
        Me.formalObjCodeOfWatchdog = -1
        Me.formalObjCodeOfTimeDataGet = -1
        Me.oMasProSuiteDllSpecOfDataKinds = Nothing
        Me.oMasProListDllSpecOfDataKinds = Nothing
        Me.oScheduledUllSpecOfDataKinds = Nothing
        Me.oMasProDlReflectSpecOfCplxObjCodes = Nothing
        Me.oByteArrayPassivePostSpecOfObjCodes = Nothing
        Me.oVersionInfoUllSpecOfObjCodes = Nothing
        Me.oRiyoDataUllSpecOfObjCodes = Nothing

        Me.reqNumberForNextSnd = 0
        Me.pseudoLineStatus = LineStatus.Initial
        Me.isPseudoConnectionProlongationPeriod = False
        Me.oPseudoConnectionProlongationTimer = New TickTimer(TelServerAppBaseConfig.PseudoConnectionProlongationTicks)
        Me.hidesLineErrorFromRecording = If(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks <= 0, True, False)
        Me.oLineErrorRecordingIntervalTimer = If(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks <= 0, Nothing, New TickTimer(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks))
        Me.hidesLineErrorFromAlerting = If(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks <= 0, True, False)
        Me.oLineErrorAlertingIntervalTimer = If(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks <= 0, Nothing, New TickTimer(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks))
        Me.oInitialConnectLimitTimerForLineError = New TickTimer(TelServerAppBaseConfig.InitialConnectLimitTicksForLineError)
        Me.LineStatus = LineStatus.Initial

        Me.oWatchdogTimer.Renew(TelServerAppBaseConfig.WatchdogIntervalTicks)
        Me.telegReadingLimitBaseTicks = TelServerAppBaseConfig.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = TelServerAppBaseConfig.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = TelServerAppBaseConfig.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = TelServerAppBaseConfig.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = TelServerAppBaseConfig.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = TelServerAppBaseConfig.TelegLoggingMaxLengthOnWrite
        Me.enableXllStrongExclusion = TelServerAppBaseConfig.EnableXllStrongExclusion
        Me.enableActiveSeqStrongExclusion = TelServerAppBaseConfig.EnableActiveSeqStrongExclusion
        Me.enableActiveOneOrdering = TelServerAppBaseConfig.EnableActiveOneOrdering

        'NOTE: ���p�f�[�^��RiyoDataTrashDirPath�Ɉړ�����΂m�ԒʐM�v���Z�X���Ȃ��i���C�����łȂ��j
        '�ꍇ�ł����Ă��A���̃v���Z�X���g��RiyoDataTrashDirPath�ɒ��ڈړ����邱�Ƃ͂��Ȃ��B
        '�􂢑ւ��������΂m�ԒʐM�v���Z�X�̗L�����ӎ����āA�폜�Ώۃf�B���N�g����I������B
        Dim sRiyoDataBaseDirPath As String = Utility.CombinePathWithVirtualPath(TelServerAppBaseConfig.RiyoDataDirPath, clientCode.ToString(TelServerAppBaseConfig.RiyoDataStationBaseDirNameFormat))
        Me.sRiyoDataInputDirPath = Utility.CombinePathWithVirtualPath(sRiyoDataBaseDirPath, TelServerAppBaseConfig.RiyoDataInputDirPathInStationBase)
        Me.sRiyoDataRejectDirPath = Utility.CombinePathWithVirtualPath(sRiyoDataBaseDirPath, TelServerAppBaseConfig.RiyoDataRejectDirPathInStationBase)

        '����Telegrapher����ƂŎg���f�B���N�g��������������B
        Log.Info("Initializing directory [" & sTempDirPath & "]...")
        Utility.DeleteTemporalDirectory(sTempDirPath)
        Directory.CreateDirectory(sTempDirPath)

        'FTP�T�[�o��̓��Y�N���C�A���g�p�f�B���N�g���ɂ��āA������΍쐬����B
        'NOTE: �f�B���N�g���̍쐬���̂́A�ʐM�J�n���ɍs���邪�A�ʐM�J�n�O�ł�
        '�������g�̓f�B���N�g���ɃA�N�Z�X���邱�Ƃ����邽�߁A�����ɕK�v�ł���B
        Log.Info("Createing directory [" & sPermittedPath & "]...")
        Directory.CreateDirectory(sPermittedPath)

        '�ʐM��ԃe�[�u������R�l�N�V�������폜�B
        Me.DeleteDirectConStatus()
    End Sub
    '-------Ver0.1 ������ԕ�Ή� MOD END-------------
#End Region

#Region "�v���p�e�B"
    '���̃v���p�e�B�́A�e�X���b�h�ɂ����Ă��A����u�ԁi�����ߋ��̏u�ԁj��
    '�����Ԃ����[�U�ɕ\������ړI�ł���΁A�Q�Ɖ\�ł���B
    '���̃X���b�h�����̉ӏ��Œ�~��������ŏ�Ԃ��擾����i���̌�A����
    '�X���b�h�ɑ΂��ĔC�ӂ̑�����s���Ă���A�C�ӂɍĊJ�����邱�Ƃ��ł���j
    '�킯�ł͂Ȃ����߁A�Ăяo������߂������_�ŁA�߂�l�̉����Ԃ��ێ�
    '����Ă���Ƃ͌���Ȃ��B���̂����A���p�x�ŌĂяo���Ă��債�����ׂ�
    '������Ȃ��B�܂��A��x�u�ؒf�v�ɂȂ�����A�e�X���b�h���V�����\�P�b�g��
    '�n���Ȃ����葼�̏�Ԃɕω����Ȃ����Ƃ��l������΁A�e�X���b�h������
    '�X���b�h�𐧌䂷���ł����p�ł���P�[�X�͂���B
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
    Protected Overrides Function CreateWatchdogReqTelegram() As IReqTelegram
        If formalObjCodeOfWatchdog = -1 Then Return Nothing
        Return New EkWatchdogReqTelegram(oTelegGene, formalObjCodeOfWatchdog, TelServerAppBaseConfig.WatchdogReplyLimitTicks)
    End Function

    Protected Overrides Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oPseudoConnectionProlongationTimer Then
            Return ProcOnPseudoDisconnectTime()
        End If

        If oTimer Is oLineErrorRecordingIntervalTimer Then
            Return ProcOnLineErrorRecordingTime()
        End If

        '-------Ver0.1 ������ԕ�Ή� ADD START-----------
        If oTimer Is oLineErrorAlertingIntervalTimer Then
            Return ProcOnLineErrorAlertingTime()
        End If
        '-------Ver0.1 ������ԕ�Ή� ADD END-------------

        If oTimer Is oInitialConnectLimitTimerForLineError Then
            Return ProcOnInitialConnectLimitTimeForLineError()
        End If

        Return MyBase.ProcOnTimeout(oTimer)
    End Function

    Protected Overridable Function ProcOnPseudoDisconnectTime() As Boolean
        Log.Info("Connection observation period ended.")

        isPseudoConnectionProlongationPeriod = False

        'pseudoLineStatus��LineStatus�Ɉ�v������B
        'NOTE: LineStatus = LineStatus.Steady��
        'pseudoLineStatus <> LineStatus.Steady�Ƃ����̂͂��蓾�Ȃ��B
        If LineStatus <> LineStatus.Steady Then
            'NOTE: ���̃P�[�X��pseudoLineStatus��LineStatus.Initial�Ƃ���
            '���Ƃ͂��蓾�Ȃ��B�^�C�}���J�n����Ă���Ƃ������Ƃ́A
            '���R�l�N�V�����̐ؒf���������Ƃ������Ƃł���A
            '����ɑO�ɂ͎��R�l�N�V��������ы^���R�l�N�V������
            '�ڑ����������Ƃ������ƂɂȂ�B
            If pseudoLineStatus = LineStatus.Steady Then
                pseudoLineStatus = LineStatus.Disconnected
                Log.Error("Closing the pseudo connection...")
                ProcOnPseudoConnectionDisappear()
            End If
        End If
        Return True
    End Function

    Protected Overridable Function ProcOnLineErrorRecordingTime() As Boolean
        Log.Info("Line error recording time comes.")

        Debug.Assert(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks > 0)

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

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    Protected Overridable Function ProcOnLineErrorAlertingTime() As Boolean
        Log.Info("Line error alerting time comes.")

        Debug.Assert(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks > 0)

        If LineStatus = LineStatus.Steady Then
            hidesLineErrorFromAlerting = False
        Else
            '�ʐM�ُ�̌x�񃁁[���𐶐�����B
            'NOTE: �x�񃁁[���ł́A�V���ɐ�������郁�[�����������Ƃ��ȂāA
            '�ُ킪���������Ƃ݂Ȃ����ƂɂȂ邽�߁A�ʐM�ُ킪�������Ă���
            '����́A����I�ɐV���ȃ��[���𐶐����Ȃ���΂Ȃ�Ȃ��B
            EmitLineErrorMail()

            RegisterTimer(oLineErrorAlertingIntervalTimer, TickTimer.GetSystemTick())
            Debug.Assert(hidesLineErrorFromAlerting = True)
        End If
        Return True
    End Function
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------

    Protected Overridable Function ProcOnInitialConnectLimitTimeForLineError() As Boolean
        Log.Error("Initial connection limit time comes for line error.")
        '-------Ver0.1 ������ԕ�Ή� ADD START-----------
        lineErrorBeginingTime = DateTime.Now
        '-------Ver0.1 ������ԕ�Ή� ADD END-------------

        If Not hidesLineErrorFromRecording Then
            Debug.Assert(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks > 0)

            '���W�f�[�^��L�e�[�u���ɒʐM�ُ��o�^����B
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromRecording = True
        End If

        '-------Ver0.1 ������ԕ�Ή� ADD START-----------
        If Not hidesLineErrorFromAlerting Then
            Debug.Assert(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks > 0)

            '�ʐM�ُ�̌x�񃁁[���𐶐�����B
            EmitLineErrorMail()

            RegisterTimer(oLineErrorAlertingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromAlerting = True
        End If
        '-------Ver0.1 ������ԕ�Ή� ADD END-------------

        Return True
    End Function

    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        '-------Ver0.1 ������ԕ�Ή� MOD START-----------
        Select Case oRcvMsg.Kind
            Case ServerAppInternalMessageKind.NameChangeNotice
                Return ProcOnNameChangeNoticeReceive(oRcvMsg)
            Case ServerAppInternalMessageKind.MasProDllRequest
                Return ProcOnMasProDllRequestReceive(oRcvMsg)
            Case ServerAppInternalMessageKind.ScheduledUllRequest
                Return ProcOnScheduledUllRequestReceive(oRcvMsg)
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
        '-------Ver0.1 ������ԕ�Ή� MOD END-------------
    End Function

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    Protected Overridable Function ProcOnNameChangeNoticeReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("NameChange notified by manager.")

        Dim oExt As NameChangeNoticeExtendPart = NameChangeNotice.Parse(oRcvMsg).ExtendPart
        sClientStationName = oExt.StationName
        sClientCornerName = oExt.CornerName
        Return True
    End Function
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------

    Protected Overridable Function ProcOnMasProDllRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("MasProDll requested by manager.")

        'NOTE: ����Telegrapher�͐e�X���b�h�Ƌٖ��ȘA�g���Ƃ�Ȃ���AMasProDllRequest��
        '�������邱�Ƃɂ��Ă���B��̓I�ɁA����Telegrapher�́A����MasProDllRequest��
        '�N������t�@�C���]�����I������ƔF�������i���������܂��́A���߂��j���_�ŁA
        '����ɑΉ�����MasProDllResponse��e�X���b�h�֑��M����B�������邱�ƂŁA
        '�e�X���b�h�́A�����ɍs���\���IDLL�̌������R���g���[���\�ɂȂ�B
        '�Ȃ��A���̌��ʂƂ��āA����Telegrapher��MasProDllRequest�ɑ΂���
        'MasProDllResponse��ԐM���Ă��Ȃ��󋵂ŁA�e�X���b�h�����̔\���IDLL��
        '�v�����Ă���i�V����MasProDllRequest���𑗐M���Ă���j���Ƃ́A
        '���蓾�Ȃ��Ȃ��Ă���B

        'NOTE: oRcvMsg���v���Z�X���Ő������ꂽ���̂ł��邱�Ƃ��l����ƁA
        '���̃`�F�b�N�͏璷�ł���B
        If oMasProSuiteDllSpecOfDataKinds Is Nothing OrElse _
           oMasProListDllSpecOfDataKinds Is Nothing Then
            Log.Fatal("I don't support MasProDll.")

            '�e�X���b�h�ɉ�����ԐM���ă��\�b�h���I������B
            MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End If

        'NOTE: oRcvMsg���v���Z�X���Ő������ꂽ���̂ł��邱�Ƃ��l����ƁA
        '���̃`�F�b�N�͏璷�ł���B
        Dim oExt As MasProDllRequestExtendPart = MasProDllRequest.Parse(oRcvMsg).ExtendPart
        If Not EkMasProListFileName.IsValid(oExt.ListFileName) Then
            Log.Fatal("The file name [" & oExt.ListFileName & "] is invalid as MasProListFileName.")

            '�e�X���b�h�ɉ�����ԐM���ă��\�b�h���I������B
            MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End If

        Dim sDataKind As String = EkMasProListFileName.GetDataKind(oExt.ListFileName)
        Dim sDataFileName As String
        Dim sDataFileHashValue As String
        Dim oSpec As TelServerAppMasProDllSpec

        'NOTE: oRcvMsg���v���Z�X���Ő������ꂽ���̂ł��邱�Ƃ��l����ƁA
        '�����Try�u���b�N�ɓ����̂͏璷�ł���B
        Try
            If String.IsNullOrEmpty(oExt.DataFileName) Then
                sDataFileName = ""
                sDataFileHashValue = ""
                oSpec = oMasProListDllSpecOfDataKinds(sDataKind)
            Else
                sDataFileName = Path.Combine(sPermittedPathInFtp, oExt.DataFileName)
                sDataFileHashValue = oExt.DataFileHashValue
                oSpec = oMasProSuiteDllSpecOfDataKinds(sDataKind)
            End If
        Catch ex As KeyNotFoundException
            Log.Fatal("I don't support the DataKind [" & sDataKind & "].")

            '�e�X���b�h�ɉ�����ԐM���ă��\�b�h���I������B
            MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End Try

        'MasProDllRequest�Ŏw�肳�ꂽ�t�@�C����FTP�T�[�o��ɃR�s�[����B
        'NOTE: �����ʁE����o�[�W�����̔z�M�w�����I�[�o�[���b�v���邱�Ƃ�
        '���蓾�Ȃ��i���ɔz�M���ɂȂ��Ă���΁A�Ή^�ǒ[���ʐM�v���Z�X��
        '�^�ǒ[���Ɂu�r�W�[�v��Ԃ����ƂɂȂ��Ă���j�B���̂��߁A�R�s�[���
        '���݂��Ă���ʂ́i�]�����́j�t�@�C���Ɩ��O���Փ˂��邱�Ƃ͂��蓾�Ȃ��B
        '�Ȃ��A�Ή^�ǒ[���ʐM�v���Z�X�ɂ��z���������Ɖ��肵�Ă��A����
        'Telegrapher�̐e�X���b�h�́A�����Telegrapher�ɑ΂��A��Ɉ˗�����
        '�z�M���I�����Ȃ�����A���̔z�M�͈˗����Ȃ��̂ŁAFTP�T�[�o���
        '�t�@�C�������Փ˂��邱�Ƃ͂��蓾�Ȃ��B

        If Not String.IsNullOrEmpty(oExt.DataFileName) Then
            Dim sDataSrcPath As String = Path.Combine(TelServerAppBaseConfig.MasProDirPath, oExt.DataFileName)
            Dim sDataDstPath As String = Path.Combine(sPermittedPath, oExt.DataFileName)
            File.Copy(sDataSrcPath, sDataDstPath, True)
        End If

        Dim sListSrcPath As String = Path.Combine(TelServerAppBaseConfig.MasProDirPath, oExt.ListFileName)
        Dim sListDstPath As String = Path.Combine(sPermittedPath, oExt.ListFileName)
        File.Copy(sListSrcPath, sListDstPath, True)

        If Not String.IsNullOrEmpty(oExt.DataFileName) Then
            'DLL�o�[�W�������e�[�u���̊Y�����R�[�h�̕s���t���O��True��ݒ肷��B
            'NOTE: ���̃^�C�~���O�Łu�s���v�ɂ��Ă��܂��ƁA�L���[�C���O����Ă���Ԃ�
            '�ʐM�ُ킪���������ꍇ�Ɂu�s���v�̂܂܂ɂȂ��Ă��܂��i����̓���o�[�W������
            '�z�M�w���Ńf�[�^�{�̂̑��M���K�v�ɂȂ��Ă��܂��j���߁A�������������Ȃ��B
            '�������A����̔h���N���X�ɂ����āARegisterActiveDll�������̂�������
            '���s����Ȃ��̂͋H�ł���Ǝv���邵�A���ʉ��̂��߂ɂ͒v�����Ȃ��B
            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()
                UpdateDllVersionUncertainFlag(dbCtl, oExt.ListFileName, "1")
                dbCtl.TransactionCommit()
            Catch ex As DatabaseException
                dbCtl.TransactionRollBack()
                Throw
            Catch ex As Exception
                dbCtl.TransactionRollBack()
                Throw New DatabaseException(ex)
            Finally
                dbCtl.ConnectClose()
            End Try
        End If

        Dim oXllReqTeleg As New EkMasProDllReqTelegram( _
           oTelegGene, _
           oSpec.ObjCode, _
           oSpec.SubObjCode, _
           ContinueCode.Start, _
           sDataFileName, _
           sDataFileHashValue, _
           Path.Combine(sPermittedPathInFtp, oExt.ListFileName), _
           oExt.ListFileHashValue, _
           0, 0, 0, _
           oSpec.TransferLimitTicks, _
           oSpec.StartReplyLimitTicks)

        RegisterActiveDll( _
           oXllReqTeleg, _
           oSpec.RetryIntervalTicks, _
           oSpec.MaxRetryCountToForget + 1, _
           oSpec.MaxRetryCountToCare + 1)
        Return True
    End Function

    '�\���IDLL�����������iContinueCode.Finish�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnActiveDllComplete(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Info("MasProDll completed.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            Dim appUnitTable As DataTable = SelectApplicableUnits(dbCtl, sListFileName)

            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL��ԃe�[�u�����X�V����B
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusNormal)

                'DLL�o�[�W�����e�[�u�����X�V����B
                UpdateOrInsertDllVersion(dbCtl, sListFileName)
            End If

            'DLL��ԃe�[�u�����X�V����B
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusNormal)

            'DL��ԃe�[�u���̊֘A���R�[�h���u�z�M���v�ɕύX����B
            UpdateDlStatusToExecutingIfNeeded(dbCtl, appUnitTable, sListFileName)

            '�o�[�W���������Ғl�e�[�u�����X�V����B
            'NOTE: DLL��Ԃ�z�M���ȊO�ɂ������ƂŁA���L�ŎQ�Ƃ���}�X�^��
            '�v���O�����̓o�^��񂪑��̃X���b�h�ŏ㏑�������ƁA������
            '����͊��҂ł��Ȃ��B�g�����U�N�V�������R�~�b�g����܂ł�
            '���̃R�l�N�V��������͌Â���Ԃ��݂���悤��DB��ݒ肵�Ă���
            '���Ƃ��K�{�ł��邱�Ƃɒ��ӁB
            Dim sSureDataFileName As String = SelectMasProDataFileName(dbCtl, sListFileName)
            If EkMasProListFileName.GetDataPurpose(sListFileName).Equals(EkConstants.DataPurposeMaster) Then
                DeleteAndInsertMasterVersionInfoExpected(dbCtl, appUnitTable, sSureDataFileName)
            Else
                DeleteAndInsertProgramVersionInfoExpected(dbCtl, appUnitTable, sSureDataFileName)
            End If

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        If Not String.IsNullOrEmpty(sDataFileName) Then
            'FTP�T�[�o��ɒu�����t�@�C�����폜����B
            File.Delete(Path.Combine(sPermittedPath, sDataFileName))
        End If

        'FTP�T�[�o��ɒu�����t�@�C�����폜����B
        File.Delete(Path.Combine(sPermittedPath, sListFileName))

        '�e�X���b�h�ɉ�����ԐM����B
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IDLL�����������iContinueCode.FinishWithoutStoring�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnActiveDllCompleteWithoutStoring(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("MasProDll failed by content error.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Log.Info("The client says [" _
           & oXllReqTeleg.ResultantVersionOfSlot1.ToString("D8") & ", " _
           & oXllReqTeleg.ResultantVersionOfSlot2.ToString("D8") & ", " _
           & oXllReqTeleg.ResultantFlagOfFull.ToString("X2") & "].")

        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL��ԃe�[�u�����X�V����B
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusContentError)

                'DLL�o�[�W�����e�[�u�����X�V����i�s���t���O��False�ɖ߂������j�B
                UpdateDllVersionUncertainFlag(dbCtl, sListFileName, "0")
            End If

            'DLL��ԃe�[�u�����X�V����B
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusContentError)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        If Not String.IsNullOrEmpty(sDataFileName) Then
            'FTP�T�[�o��ɒu�����t�@�C�����폜����B
            File.Delete(Path.Combine(sPermittedPath, sDataFileName))
        End If

        'FTP�T�[�o��ɒu�����t�@�C�����폜����B
        File.Delete(Path.Combine(sPermittedPath, sListFileName))

        '�e�X���b�h�ɉ�����ԐM����B
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IDLL�ɂăN���C�A���g����]�����s�܂��͓]��������ʒm���ꂽ�iContinueCode.Abort�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnActiveDllAbort(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("MasProDll failed by transfer error.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Log.Info("The client says [" _
           & oXllReqTeleg.ResultantVersionOfSlot1.ToString("D8") & ", " _
           & oXllReqTeleg.ResultantVersionOfSlot2.ToString("D8") & ", " _
           & oXllReqTeleg.ResultantFlagOfFull.ToString("X2") & "].")

        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL��ԃe�[�u�����X�V����B
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

                'DLL�o�[�W�����e�[�u�����X�V����i�s���t���O��False�ɖ߂������j�B
                UpdateDllVersionUncertainFlag(dbCtl, sListFileName, "0")
            End If

            'DLL��ԃe�[�u�����X�V����B
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        If Not String.IsNullOrEmpty(sDataFileName) Then
            'FTP�T�[�o��ɒu�����t�@�C�����폜����B
            File.Delete(Path.Combine(sPermittedPath, sDataFileName))
        End If

        'FTP�T�[�o��ɒu�����t�@�C�����폜����B
        File.Delete(Path.Combine(sPermittedPath, sListFileName))

        '�e�X���b�h�ɉ�����ԐM����B
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IDLL�ɂē]���I��REQ�d���҂��̃^�C���A�E�g�����������ꍇ
    Protected Overrides Sub ProcOnActiveDllTimeout(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("MasProDll failed by transfer timeout.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL��ԃe�[�u�����X�V����B
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusTimeout)

                'NOTE: ���̏󋵂ł́A�o�[�W�������i�T�[�o�j�e�[�u���̕s���t���O�́A
                'True�̂܂܂ɂ��Ă����ׂ��ł���B
            End If

            'DLL��ԃe�[�u�����X�V����B
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusTimeout)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        'NOTE: ���̏󋵂ł́AFTP�T�[�o��ɒu�����t�@�C�����폜����ׂ��ł͂Ȃ��B

        '�e�X���b�h�ɉ�����ԐM����B
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IDLL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveDllRetryOverToForget(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        'NOTE: ���蓾�Ȃ��Ǝv���邪�A���肪�Ԃ��Ă���NAK����ł��邽�߁A�p�ӂ��Ă����B
        '�{���ɂ��蓾�Ȃ����̂ƈ����ɂ́AGetRequirement()�ɂāA
        '�\���IDLL�Ɋւ���EkNakCauseCode.NoData��NAK�͐ؒf�����ɂ���Ƃ悢�B
        ProcOnActiveDllRetryOverToCare(iXllReqTeleg, iNakTeleg)
    End Sub

    '�\���IDLL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveDllRetryOverToCare(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Log.Error("MasProDll failed by retry over.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL��ԃe�[�u�����X�V����B
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

                'DLL�o�[�W�����e�[�u�����X�V����i�s���t���O��False�ɖ߂������j�B
                UpdateDllVersionUncertainFlag(dbCtl, sListFileName, "0")
            End If

            'DLL��ԃe�[�u�����X�V����B
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        If Not String.IsNullOrEmpty(sDataFileName) Then
            'FTP�T�[�o��ɒu�����t�@�C�����폜����B
            File.Delete(Path.Combine(sPermittedPath, sDataFileName))
        End If

        'FTP�T�[�o��ɒu�����t�@�C�����폜����B
        File.Delete(Path.Combine(sPermittedPath, sListFileName))

        '�e�X���b�h�ɉ�����ԐM����B
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IDLL�̍Œ���L���[�C���O���ꂽ�\���IDLL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveDllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("MasProDll failed by telegramming error.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL��ԃe�[�u�����X�V����B
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

                'NOTE: ���̏󋵂ł́A�o�[�W�������i�T�[�o�j�e�[�u���̕s���t���O�́A
                'True�̂܂܂ɂ��Ă����ׂ��ł���B�]���I��REQ�d������M�����ۂɁA
                '���̓d�������ŃG���[�����o���Ă��܂����P�[�X��z�肵�Ă̂��Ƃł���B
                '���̍ہA���葕�u�͎�M�����t�@�C�����̂͗L���Ȃ��̂Ƃ��Ĉ����Ă���
                '�͂��ł���B
            End If

            'DLL��ԃe�[�u�����X�V����B
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        'NOTE: ���̏󋵂ł́AFTP�T�[�o��ɒu�����t�@�C�����폜����ׂ��ł͂Ȃ��B

        '�e�X���b�h�ɉ�����ԐM����B
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    Protected Overridable Function ProcOnScheduledUllRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ScheduledUll requested by manager.")

        'NOTE: ����Telegrapher�͐e�X���b�h�Ƌٖ��ȘA�g���Ƃ�Ȃ���AScheduledUllRequest��
        '�������邱�Ƃɂ��Ă���B��̓I�ɁA����Telegrapher�́A����ScheduledUllRequest��
        '�N������t�@�C���]�����I������ƔF�������i���������܂��́A���߂��j���_�ŁA
        '����ɑΉ�����ScheduledUllResponse��e�X���b�h�֑��M����B�������邱�ƂŁA
        '�e�X���b�h�́A�����ɍs���\���IULL�̌������R���g���[���\�ɂȂ�B
        '�Ȃ��A���̌��ʂƂ��āA����Telegrapher��ScheduledUllRequest�ɑ΂���
        'ScheduledUllResponse��ԐM���Ă��Ȃ��󋵂ŁA�e�X���b�h�����̔\���IULL��
        '�v�����Ă���i�V����ScheduledUllRequest���𑗐M���Ă���j���Ƃ́A
        '���蓾�Ȃ��Ȃ��Ă���B

        'NOTE: oRcvMsg���v���Z�X���Ő������ꂽ���̂ł��邱�Ƃ��l����ƁA
        '���̃`�F�b�N�͏璷�ł���B
        If oScheduledUllSpecOfDataKinds Is Nothing Then
            Log.Fatal("I don't support ScheduledUll.")

            '�e�X���b�h�ɉ�����ԐM���ă��\�b�h���I������B
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End If

        'NOTE: oRcvMsg���v���Z�X���Ő������ꂽ���̂ł��邱�Ƃ��l����ƁA
        '���̃`�F�b�N�͏璷�ł���B
        Dim oExt As ScheduledUllRequestExtendPart = ScheduledUllRequest.Parse(oRcvMsg).ExtendPart
        If Not EkScheduledDataFileName.IsValid(oExt.FileName) Then
            Log.Fatal("The file name [" & oExt.FileName & "] is invalid as ScheduledDataFileName.")

            '�e�X���b�h�ɉ�����ԐM���ă��\�b�h���I������B
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End If

        Dim sDataKind As String = EkScheduledDataFileName.GetKind(oExt.FileName)
        Dim oSpec As TelServerAppScheduledUllSpec

        'NOTE: oRcvMsg���v���Z�X���Ő������ꂽ���̂ł��邱�Ƃ��l����ƁA
        '�����Try�u���b�N�ɓ����̂͏璷�ł���B
        Try
            oSpec = oScheduledUllSpecOfDataKinds(sDataKind)
        Catch ex As KeyNotFoundException
            Log.Fatal("I don't support the DataKind [" & sDataKind & "].")

            '�e�X���b�h�ɉ�����ԐM���ă��\�b�h���I������B
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End Try

        Dim oXllReqTeleg As New EkServerDrivenUllReqTelegram( _
           oTelegGene, _
           oSpec.ObjCode, _
           ContinueCode.Start, _
           Path.Combine(sPermittedPathInFtp, oExt.FileName), _
           oSpec.TransferLimitTicks, _
           oSpec.StartReplyLimitTicks)

        RegisterActiveUll( _
           oXllReqTeleg, _
           oSpec.RetryIntervalTicks, _
           oSpec.MaxRetryCountToForget + 1, _
           oSpec.MaxRetryCountToCare + 1)
        Return True
    End Function

    '�\���IULL�����������i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e���������Ă��邱�Ƃ��m�F�����j
    '�i�]���I��REQ�d���ɑ΂�ACK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Function ProcOnActiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Log.Info("ScheduledUll completed.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)

        If File.Exists(sFilePath) Then
            Dim oSpec As TelServerAppScheduledUllSpec = oScheduledUllSpecOfDataKinds(EkScheduledDataFileName.GetKind(sFileName))
            Dim sDstPath As String = UpboundDataPath.Gen(TelServerAppBaseConfig.InputDirPathForApps(oSpec.RecAppIdentifier), clientCode, DateTime.Now)
            If UpboundDataPath.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.MaxBranchNumberForApps(oSpec.RecAppIdentifier) Then
                'FTP�T�[�o��̃t�@�C����o�^�v���Z�X���ǂݎ��p�X�Ɉړ�����B
                File.Move(sFilePath, sDstPath)

                '�o�^�v���Z�X�ɒʒm����B
                TelServerAppBaseConfig.MessageQueueForApps(oSpec.RecAppIdentifier).Send(New ExtFileCreationNotice())
            Else
               'FTP�T�[�o��̃t�@�C�����폜����B
                File.Delete(sFilePath)
                Log.Warn("File deleted.")
            End If

            '�e�X���b�h�ɉ�����ԐM����B
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return EkNakCauseCode.None
        Else
            '�]������Ă����͂��̃t�@�C���������ꍇ
            'NOTE: ���̏ꍇ�A���O�Ƀn�b�V���l�̃G���[�ƂȂ��Ă���͂��ł���
            '���߁A���������s����邱�Ƃ͊�{�I�ɂȂ��͂��ł��邪�A�O�̂���
            '����Ȃ�̎��������Ă����B

            '���W�f�[�^��L�e�[�u���Ɏ��W���s��o�^����B
            InsertScheduledUllFailureToCdt(sFileName)

            '�e�X���b�h�ɉ�����ԐM����B
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return EkNakCauseCode.HashValueError 'NOTE: ����
        End If
    End Function

    '�\���IULL�ɂē]�����������o�����i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e�ɕs���������o�����j
    '�i�]���I��REQ�d���ɑ΂��n�b�V���l�̕s��v������NAK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Function ProcOnActiveUllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Log.Error("ScheduledUll failed by hash value error.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)

        '���W�f�[�^��L�e�[�u���Ɏ��W���s��o�^����B
        InsertScheduledUllFailureToCdt(sFileName)

        If File.Exists(sFilePath) Then
            Dim oSpec As TelServerAppScheduledUllSpec = oScheduledUllSpecOfDataKinds(EkScheduledDataFileName.GetKind(sFileName))
            Dim sDstPath As String = UpboundDataPath.Gen(TelServerAppBaseConfig.RejectDirPathForApps(oSpec.RecAppIdentifier), clientCode, DateTime.Now)
            If UpboundDataPath.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.MaxBranchNumberForApps(oSpec.RecAppIdentifier) Then
                'FTP�T�[�o��̃t�@�C����j���f�[�^�p�p�X�Ɉړ�����B
                File.Move(sFilePath, sDstPath)
            Else
                'FTP�T�[�o��̃t�@�C�����폜����B
                File.Delete(sFilePath)
                Log.Warn("File deleted.")
            End If
        End If

        '�e�X���b�h�ɉ�����ԐM����B
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
        Return EkNakCauseCode.HashValueError
    End Function

    '�\���IULL�ɂăN���C�A���g����]�����s��ʒm���ꂽ�iContinueCode.Abort�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnActiveUllAbort(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("ScheduledUll failed by transfer error.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

        '���W�f�[�^��L�e�[�u���Ɏ��W���s��o�^����B
        InsertScheduledUllFailureToCdt(sFileName)

        'FTP�T�[�o��Ɏc�����t�@�C��������΍폜����B
        File.Delete(Path.Combine(sPermittedPath, sFileName))

        '�e�X���b�h�ɉ�����ԐM����B
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveUllRetryOverToForget(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Log.Info("ScheduledUll skipped by retry over to forget.")

        'NOTE: ���̏ꍇ�A���W�f�[�^��L�e�[�u���Ɏ��W���s�͓o�^���Ȃ��B

        '�e�X���b�h�ɉ�����ԐM����B
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�̊J�n�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveUllRetryOverToCare(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Log.Error("ScheduledUll failed by retry over to care.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

        '���W�f�[�^��L�e�[�u���Ɏ��W���s��o�^����B
        InsertScheduledUllFailureToCdt(sFileName)

        '�e�X���b�h�ɉ�����ԐM����B
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�ɂē]���I��REQ�d���҂��̃^�C���A�E�g�����������ꍇ
    Protected Overrides Sub ProcOnActiveUllTimeout(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("ScheduledUll failed by transfer timeout.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

        '���W�f�[�^��L�e�[�u���Ɏ��W���s��o�^����B
        InsertScheduledUllFailureToCdt(sFileName)

        'NOTE: ���̏󋵂ł́AFTP�T�[�o��Ƀt�@�C�����c���Ă��Ă��A�폜����ׂ��ł͂Ȃ��B

        '�e�X���b�h�ɉ�����ԐM����B
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '�\���IULL�̍Œ���L���[�C���O���ꂽ�\���IULL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("ScheduledUll failed by telegramming error.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

        '���W�f�[�^��L�e�[�u���Ɏ��W���s��o�^����B
        InsertScheduledUllFailureToCdt(sFileName)

        'NOTE: ���̏󋵂ł́AFTP�T�[�o��Ƀt�@�C�����c���Ă��Ă��A�폜����ׂ��ł͂Ȃ��B

        '�e�X���b�h�ɉ�����ԐM����B
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)
        Dim objCode As Integer = oRcvTeleg.ObjCode
        Select Case oRcvTeleg.SubCmdCode
            Case EkSubCmdCode.Get
                If objCode = formalObjCodeOfTimeDataGet Then
                    Return ProcOnTimeDataGetReqTelegramReceive(oRcvTeleg)
                End If

                If oMasProDlReflectSpecOfCplxObjCodes IsNot Nothing AndAlso _
                   oMasProDlReflectSpecOfCplxObjCodes.ContainsKey(GenCplxObjCode(objCode, 0)) Then
                    Return ProcOnMasProDlReflectReqTelegramReceive(oRcvTeleg)
                End If

            Case EkSubCmdCode.Post
                If oByteArrayPassivePostSpecOfObjCodes IsNot Nothing AndAlso _
                   oByteArrayPassivePostSpecOfObjCodes.ContainsKey(CByte(objCode)) Then
                    Return ProcOnByteArrayPostReqTelegramReceive(oRcvTeleg)
                End If
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(iRcvTeleg)
    End Function

    Protected Overridable Function ProcOnTimeDataGetReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkTimeDataGetReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("TimeDataGet REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("TimeDataGet REQ received.")

        Dim oReplyTeleg As EkTimeDataGetAckTelegram = oRcvTeleg.CreateAckTelegram(DateTime.Now)
        Log.Info("Sending TimeDataGet ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnMasProDlReflectReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkMasProDlReflectReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("MasProDlReflect REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Dim oSpec As TelServerAppMasProDlReflectSpec = oMasProDlReflectSpecOfCplxObjCodes(GenCplxObjCode(oRcvTeleg.ObjCode, oRcvTeleg.SubObjCode))
        If oSpec.DataKind Is Nothing Then
            Log.Error("MasProDlReflect REQ with invalid SubObjCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg)
            Return True
        End If

        Log.Info("MasProDlReflect REQ received.")

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            UpdateOrInsertDlStatus(dbCtl, oRcvTeleg)
            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        Dim oReplyTeleg As EkMasProDlReflectAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending MasProDlReflect ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnByteArrayPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkByteArrayPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("ByteArrayPost REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("ByteArrayPost REQ received.")

        Dim oSpec As TelServerAppByteArrayPassivePostSpec = oByteArrayPassivePostSpecOfObjCodes(CByte(oRcvTeleg.ObjCode))
        Dim sDstPath As String = UpboundDataPath.Gen(TelServerAppBaseConfig.InputDirPathForApps(oSpec.RecAppIdentifier), clientCode, DateTime.Now)
        If UpboundDataPath.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.MaxBranchNumberForApps(oSpec.RecAppIdentifier) Then
            '�ꎞ��Ɨp�f�B���N�g���Ńt�@�C��������B
            Dim sTmpPath As String = Path.Combine(sTempDirPath, sTempFileName)
            Try
                Using oStream As New FileStream(sTmpPath, FileMode.Create, FileAccess.Write)
                    Dim aBytes As Byte() = oRcvTeleg.ByteArray
                    oStream.Write(aBytes, 0, aBytes.Length)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                'NOTE: �ꉞ�A�����^�C���ȏ�������Ŕ��������O������̂ŁA
                '�ǂ�����̂��x�X�g���悭�l���������悢�B
                Abort()
            End Try

            '�쐬�����t�@�C����o�^�v���Z�X���ǂݎ��p�X�Ɉړ�����B
            File.Move(sTmpPath, sDstPath)

            '�o�^�v���Z�X�ɒʒm����B
            TelServerAppBaseConfig.MessageQueueForApps(oSpec.RecAppIdentifier).Send(New ExtFileCreationNotice())
        Else
            Log.Warn("Ignored.")
        End If

        Dim oReplyTeleg As EkByteArrayPostAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ByteArrayPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    '�w�b�_���̓��e���󓮓IULL��REQ�d���̂��̂ł��邩���肷�郁�\�b�h
    Protected Overrides Function IsPassiveUllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)

        If oTeleg.SubCmdCode <> EkSubCmdCode.Get Then Return False
        Dim objCode As Byte = CByte(oTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then Return True

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then Return True

        Return False
    End Function

    '�n���ꂽ�d���C���X�^���X��K�؂Ȍ^�̃C���X�^���X�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsPassiveUllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        'NOTE: �h���N���X��IsPassiveUllReq��������ObjCode�̓d���ɂ��ẮA
        '�ϊ����h���N���X��ParseAsPassiveUllReq�Ŋ��������Ă���
        '�Ƃ����z��ł���B

        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        Dim objCode As Byte = CByte(oTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Dim oSpec As TelServerAppVersionInfoUllSpec = oVersionInfoUllSpecOfObjCodes(objCode)
            Return New EkClientDrivenUllReqTelegram(iTeleg, oSpec.TransferLimitTicks)
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Dim oSpec As TelServerAppRiyoDataUllSpec = oRiyoDataUllSpecOfObjCodes(objCode)
            Return New EkClientDrivenUllReqTelegram(iTeleg, oSpec.TransferLimitTicks)
        End If

        Debug.Fail("This case is impermissible.")
        Return Nothing
    End Function

    '�󓮓IULL�̏����i�\�����ꂽ�t�@�C���̎󂯓���m�F�j���s�����\�b�h
    Protected Overrides Function PrepareToStartPassiveUll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        'NOTE: �h���N���X��IsPassiveUllReq��������ObjCode�̓d���ɂ��ẮA
        '�󂯓���m�F���h���N���X��PrepareToStartPassiveUll�Ŋ��������Ă���
        '�Ƃ����z��ł���B
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePathInFtp As String = oXllReqTeleg.FileName
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(TelServerAppBaseConfig.FtpServerRootDirPath, sFilePathInFtp)

        'NOTE: �u..\�v���̍����������Ȃ�AsPermittedPath��sFilePath�����K�����������悢��������Ȃ��B
        If Not Utility.IsAncestPath(sPermittedPath, sFilePath) Then
            Log.Error("The telegram specifies illegal path [" & sFilePathInFtp & "].")
            Return EkNakCauseCode.NotPermit 'NOTE: ����
        End If

        Dim sFileName As String = Path.GetFileName(sFilePath)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Dim oSpec As TelServerAppVersionInfoUllSpec = oVersionInfoUllSpecOfObjCodes(objCode)
            If Not EkVersionInfoFileName.IsValid(sFileName) Then
                Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
                Return EkNakCauseCode.TelegramError 'NOTE: ����
            End If
            If Not EkVersionInfoFileName.GetDataApplicableModel(sFileName).Equals(oSpec.ApplicableModel) Then
                Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
                Return EkNakCauseCode.TelegramError 'NOTE: ����
            End If
            If Not EkVersionInfoFileName.GetDataPurpose(sFileName).Equals(oSpec.DataPurpose) Then
                Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
                Return EkNakCauseCode.TelegramError 'NOTE: ����
            End If
            Log.Info("Accepting the file [" & sFileName & "] as VersionInfoUll...")
            Return EkNakCauseCode.None
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Dim oSpec As TelServerAppRiyoDataUllSpec = oRiyoDataUllSpecOfObjCodes(objCode)
            If Not sFileName.Equals(oSpec.FileName, StringComparison.OrdinalIgnoreCase) Then
                Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
                Return EkNakCauseCode.TelegramError 'NOTE: ����
            End If

            '-------Ver0.1 ������ԕ�Ή� MOD START-----------
            Dim sDstPath As String = UpboundDataPath2.Gen(sRiyoDataInputDirPath, clientCode, oSpec.FormatCode, DateTime.Now)
            If UpboundDataPath2.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.RiyoDataMaxBranchNumber Then
                Log.Info("Accepting the file [" & sFileName & "] as RiyoDataUll...")
                sCurUllRiyoDataReservedInputPath = sDstPath
                Return EkNakCauseCode.None
            Else
                Log.Warn("Branch number is now missing to accept the file [" & sFileName & "] as RiyoDataUll.")
                Return EkNakCauseCode.Busy
            End If
            '-------Ver0.1 ������ԕ�Ή� MOD END-------------
        End If

        Debug.Fail("This case is impermissible.")
        Return EkNakCauseCode.TelegramError
    End Function

    '�󓮓IULL�����������i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e���������Ă��邱�Ƃ��m�F�����j
    '�i�]���I��REQ�d���ɑ΂�ACK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Function ProcOnPassiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        'NOTE: �h���N���X��IsPassiveUllReq��������ObjCode�̓d���ɂ��ẮA
        '���ʂɉ��������u���h���N���X�̃��\�b�h�Ŋ��������Ă���Ƃ����z��ł���B
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Info("VersionInfoUll completed.")

            If File.Exists(sFilePath) Then
                Dim retValue As NakCauseCode = EkNakCauseCode.TelegramError
                Do
                    Dim dbCtl As New DatabaseTalker()
                    Try
                        dbCtl.ConnectOpen()
                        dbCtl.TransactionBegin()

                        Dim insertResult As NakCauseCode
                        If EkVersionInfoFileName.GetDataPurpose(sFileName).Equals(EkConstants.DataPurposeMaster) Then
                            insertResult = DeleteAndInsertMasterVersionInfo(dbCtl, sFilePath)
                        Else
                            insertResult = DeleteAndInsertProgramVersionInfo(dbCtl, sFilePath, oVersionInfoUllSpecOfObjCodes(objCode).GroupTitles)
                        End If
                        If insertResult <> EkNakCauseCode.None Then
                            retValue = insertResult
                            Exit Do
                        End If

                        dbCtl.TransactionCommit()
                        retValue = EkNakCauseCode.None

                    Catch ex As DatabaseException
                        'NOTE: �{���̓t�@�C�����폜���������ANAK��ԐM�������
                        '�e�؂ł��邪�A�T�[�o�v���Z�X���ł�DatabaseException��
                        '�\�����ʈُ�Ƃ��ē���I�Ɉ������Ƃɂ��Ă���B
                        Throw
                    Catch ex As Exception
                        Throw New DatabaseException(ex)
                    Finally
                        If retValue <> EkNakCauseCode.None Then
                            dbCtl.TransactionRollBack()
                        End If
                        dbCtl.ConnectClose()
                    End Try
                Loop While False

                'NOTE: �{���A������FTP�T�[�o��̃t�@�C�����폜����̂����R�ł��邪�A
                '����͍s��Ȃ����ƂƂ���B�e�X�g�Ȃǂ̍ہA���̎�M�f�[�^���m�F������
                '�P�[�X�����邽�߂ł���B�Ȃ��A���^�p�ɂ����ẮA���̃f�[�^�͍ŐV��
                '���̂����ɈӖ�������A���V�������̂��w���@�푤�ł�����ł�
                '�����ł��邱�Ƃ���A���Ƃ��ŐV�̂��̂ł����Ă��A�o�b�N�A�b�v��
                '�Ƃ�قǂ̉��l�͂Ȃ��B����āA���j�[�N�Ȗ��O�ɉ������ĕʂ�
                '�f�B���N�g���ɑޔ�����Ƃ��������Ƃ͍s��Ȃ��i�e�X�g�Ȃǂ̍ۂ́A
                '���[�U��FTP�T�[�o�ォ��t�@�C�����R�s�[����΂悢�j�B
                Return retValue
            Else
                '�]������Ă����͂��̃t�@�C���������ꍇ
                'NOTE: ���̏ꍇ�A���O�Ƀn�b�V���l�̃G���[�ƂȂ��Ă���͂��ł���
                '���߁A���������s����邱�Ƃ͊�{�I�ɂȂ��͂��ł��邪�A�O�̂���
                '����Ȃ�̎��������Ă����B
                Log.Error("Where is the file?")

                'NOTE: ���L�ɂ��ʐM�ُ킪��������Ǝv���邽�߁A
                '���ʂȈُ�̓o�^��ʒm�͍s��Ȃ����Ƃɂ��Ă����B
                Return EkNakCauseCode.HashValueError 'NOTE: ����
            End If
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Info("RiyoDataUll completed.")

            If File.Exists(sFilePath) Then
                Dim oFileInfo As New FileInfo(sFilePath)
                '-------Ver0.1 ������ԕ�Ή� MOD START-----------
                Dim oSpec As TelServerAppRiyoDataUllSpec = oRiyoDataUllSpecOfObjCodes(objCode)
                If oFileInfo.Length Mod oSpec.RecordLen <> 0 Then
                    '�T�C�Y�s���̏ꍇ��NAK��ԐM����i�m�Ԃɓn���Ƃ��ɍ��邽�߁j�B
                    'TODO: ���葕�u�����߂Ȃ��̂ł���΁A�������Ƃ��J��Ԃ����
                    '���Ƃ��뜜�����B����āA�{���ɂ����ł���΁A�w���@�푤��
                    '�ӔC������Ƃ͌����A�^�Ǒ��ł��̃t�@�C������ʂȃf�B���N�g��
                    '�ɑޔ����A���W�f�[�^��L�e�[�u���Ɉُ��o�^���Ă���A
                    'ACK��Ԃ������悢�C������B
                    Log.Error("The file size is invalid.")
                    Dim sDstPath As String = UpboundDataPath2.Gen(sRiyoDataRejectDirPath, clientCode, oSpec.FormatCode, DateTime.Now)
                    If UpboundDataPath2.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.RiyoDataMaxBranchNumber Then
                        'FTP�T�[�o��̃t�@�C����j���f�[�^�p�p�X�Ɉړ�����B
                        File.Move(sFilePath, sDstPath)
                    Else
                        'FTP�T�[�o��̃t�@�C�����폜����B
                        File.Delete(sFilePath)
                        Log.Warn("File deleted.")
                    End If
                    Return EkNakCauseCode.HashValueError 'TODO: ����������iInvalidContent�����g�������j
                Else
                    'FTP�T�[�o��̃t�@�C����΂m�ԒʐM�v���Z�X���ǂݎ��p�X�Ɉړ�����B
                    File.Move(sFilePath, sCurUllRiyoDataReservedInputPath)
                    Return EkNakCauseCode.None
                End If
                '-------Ver0.1 ������ԕ�Ή� MOD END-------------
            Else
                '�]������Ă����͂��̃t�@�C���������ꍇ
                'NOTE: ���̏ꍇ�A���O�Ƀn�b�V���l�̃G���[�ƂȂ��Ă���͂��ł���
                '���߁A���������s����邱�Ƃ͊�{�I�ɂȂ��͂��ł��邪�A�O�̂���
                '����Ȃ�̎��������Ă����B
                Log.Error("Where is the file?")

                'NOTE: ���L�ɂ��ʐM�ُ킪��������Ǝv���邽�߁A
                '���ʂȈُ�̓o�^��ʒm�͍s��Ȃ����Ƃɂ��Ă����B
                Return EkNakCauseCode.HashValueError 'NOTE: ����
            End If
        End If

        Debug.Fail("This case is impermissible.")
        Return EkNakCauseCode.TelegramError
    End Function

    '�󓮓IULL�ɂē]�����������o�����i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e�ɕs���������o�����j
    '�i�]���I��REQ�d���ɑ΂��n�b�V���l�̕s��v������NAK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Function ProcOnPassiveUllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        'NOTE: �h���N���X��IsPassiveUllReq��������ObjCode�̓d���ɂ��ẮA
        '���ʂɉ��������u���h���N���X�̃��\�b�h�Ŋ��������Ă���Ƃ����z��ł���B
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("VersionInfoUll failed by hash value error.")

            'NOTE: ���̐���������Ƃ������̂ł͂Ȃ����Ƃ����S�z�͐@������Ȃ����A
            '�������NAK��Ԃ��Ă���킯�ł��邵�A�R�l�N�V�������ؒf����邱�Ƃ�
            '�|�[�g�̎��W�f�[�^�o�^�͍s���A���[�U���C�t���������������邽�߁A
            '�O���ɑ΂��Ă���ȏ�̔z���͍s��Ȃ����Ƃɂ���B

            'NOTE: �{���A������FTP�T�[�o��̃t�@�C�����폜����̂����R�ł��邪�A
            '����͍s��Ȃ����ƂƂ���B�e�X�g�Ȃǂ̍ہA���̎�M�f�[�^���m�F������
            '�P�[�X�����邽�߂ł���B
            Return EkNakCauseCode.HashValueError
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("RiyoDataUll failed by hash value error.")

            'NOTE: ���̐���������Ƃ������̂ł͂Ȃ����Ƃ����S�z�͐@������Ȃ����A
            '�������NAK��Ԃ��Ă���킯�ł��邵�A�R�l�N�V�������ؒf����邱�Ƃ�
            '�|�[�g�̎��W�f�[�^�o�^�͍s���A���[�U���C�t���������������邽�߁A
            '���ɉ����s��Ȃ����Ƃɂ���B

            'TODO: ���葕�u�����߂Ȃ��̂ł���΁A���L�����́A�P�Ȃ�폜��
            '�ύX��������悢�C������B
            If File.Exists(sFilePath) Then
                '-------Ver0.1 ������ԕ�Ή� MOD START-----------
                Dim oSpec As TelServerAppRiyoDataUllSpec = oRiyoDataUllSpecOfObjCodes(objCode)
                Dim sDstPath As String = UpboundDataPath2.Gen(sRiyoDataRejectDirPath, clientCode, oSpec.FormatCode, DateTime.Now)
                If UpboundDataPath2.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.RiyoDataMaxBranchNumber Then
                    'FTP�T�[�o��̃t�@�C����j���f�[�^�p�p�X�Ɉړ�����B
                    File.Move(sFilePath, sDstPath)
                Else
                    'FTP�T�[�o��̃t�@�C�����폜����B
                    File.Delete(sFilePath)
                    Log.Warn("File deleted.")
                End If
                '-------Ver0.1 ������ԕ�Ή� MOD END-------------
            End If

            Return EkNakCauseCode.HashValueError
        End If

        Debug.Fail("This case is impermissible.")
        Return EkNakCauseCode.TelegramError
    End Function

    '�󓮓IULL�ɂăN���C�A���g����]�����s��ʒm���ꂽ�iContinueCode.Abort�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnPassiveUllAbort(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: �h���N���X��IsPassiveUllReq��������ObjCode�̓d���ɂ��ẮA
        '���ʂɉ��������u���h���N���X�̃��\�b�h�Ŋ��������Ă���Ƃ����z��ł���B
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("VersionInfoUll failed by transfer error.")

            'NOTE: �R�l�N�V�������ؒf����邱�ƂŃ|�[�g�̎��W�f�[�^�o�^�͍s���A
            '���[�U���C�t�����������͂��邽�߁A����ȏ�̂��Ƃ͓��ɍs��Ȃ����Ƃɂ���B

            'NOTE: �{���A������FTP�T�[�o��̃t�@�C�����폜����̂����R�ł��邪�A
            '����͍s��Ȃ����ƂƂ���B����n�ňړ���폜���s��Ȃ����ƂƂ�
            '��ѐ���ۂ��߂ł���B
            Return
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("RiyoDataUll failed by transfer error.")

            'NOTE: �R�l�N�V�������ؒf����邱�ƂŃ|�[�g�̎��W�f�[�^�o�^�͍s���A
            '���[�U���C�t�����������͂��邽�߁A����ȏ�̂��Ƃ͓��ɍs��Ȃ����Ƃɂ���B

            'FTP�T�[�o��Ɏc�����t�@�C��������΍폜����B
            File.Delete(sFilePath)
            Return
        End If

        Debug.Fail("This case is impermissible.")
    End Sub

    '�󓮓IULL�ɂē]���I��REQ�d���҂��̃^�C���A�E�g�����������ꍇ
    Protected Overrides Sub ProcOnPassiveUllTimeout(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: �h���N���X��IsPassiveUllReq��������ObjCode�̓d���ɂ��ẮA
        '���ʂɉ��������u���h���N���X�̃��\�b�h�Ŋ��������Ă���Ƃ����z��ł���B
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("VersionInfoUll failed by transfer timeout.")

            'NOTE: �R�l�N�V�������ؒf����邱�ƂŃ|�[�g�̎��W�f�[�^�o�^�͍s���A
            '���[�U���C�t�����������͂��邽�߁A���ɉ����s��Ȃ����Ƃɂ���B

            'NOTE: ���̏󋵂ł́AFTP�T�[�o��ɂ����M�i��M���j�t�@�C�����폜���Ȃ������悢�B
            Return
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("RiyoDataUll failed by transfer timeout.")

            'NOTE: �R�l�N�V�������ؒf����邱�ƂŃ|�[�g�̎��W�f�[�^�o�^�͍s���A
            '���[�U���C�t�����������͂��邽�߁A���ɉ����s��Ȃ����Ƃɂ���B

            'NOTE: ���̏󋵂ł́AFTP�T�[�o��ɂ����M�i��M���j�t�@�C�����폜���Ȃ������悢�B
            Return
        End If

        Debug.Fail("This case is impermissible.")
    End Sub

    '�󓮓IULL�̍Œ���L���[�C���O���ꂽ�󓮓IULL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnPassiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: �h���N���X��IsPassiveUllReq��������ObjCode�̓d���ɂ��ẮA
        '���ʂɉ��������u���h���N���X�̃��\�b�h�Ŋ��������Ă���Ƃ����z��ł���B
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("VersionInfoUll failed by telegramming error.")

            'NOTE: �R�l�N�V�������ؒf����邱�ƂŃ|�[�g�̎��W�f�[�^�o�^�͍s���A
            '���[�U���C�t�����������͂��邽�߁A���ɉ����s��Ȃ����Ƃɂ���B

            'NOTE: ���̏󋵂ł́AFTP�T�[�o��ɂ����M�i��M���j�t�@�C�����폜���Ȃ������悢�B
            Return
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("RiyoDataUll failed by telegramming error.")

            'NOTE: �R�l�N�V�������ؒf����邱�ƂŃ|�[�g�̎��W�f�[�^�o�^�͍s���A
            '���[�U���C�t�����������͂��邽�߁A���ɉ����s��Ȃ����Ƃɂ���B

            'NOTE: ���̏󋵂ł́AFTP�T�[�o��ɂ����M�i��M���j�t�@�C�����폜���Ȃ������悢�B
            Return
        End If

        Debug.Fail("This case is impermissible.")
    End Sub

    '�e�X���b�h����R�l�N�V�������󂯎�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionAppear()
        '�]���@�̎����ɍ��킹�āA�Đڑ����̑��MREQ�d���ʔԂ�0�ɂȂ�悤�ɂ���B
        'NOTE: �v���g�R���d�l���ɂ́u�N�����ɏ������v�Ƃ��邪�A
        '�v���g�R���d�l���⑊�葕�u���炷��΁A����f��
        '���u�i�ʐM�v���Z�X�j�ċN���Ɠ��`�ƌ����邽�߁A
        '���̎����̓v���g�R���d�l�Ƃ����v���Ă���ƍl������B
        reqNumberForNextSnd = 0
        LineStatus = LineStatus.Steady

        If pseudoLineStatus <> LineStatus.Steady Then
            Log.Info("Opening the pseudo connection...")
            pseudoLineStatus = LineStatus.Steady
            ProcOnPseudoConnectionAppear()
        End If

        UnregisterTimer(oInitialConnectLimitTimerForLineError)

        'FTP�T�[�o��̓��Y�N���C�A���g�p�f�B���N�g��������������B
        'NOTE: �O��̃R�l�N�V�����I�����Ɏc���Ă��܂����i�V�[�P���X�̓r����
        '�����炩��^�C���A�E�g���������߂ɍ폜��ۗ��ɂ��Ă����j�t�@�C����
        '�폜���邱�Ƃ��ړI�ł��邪�AFTP�T�[�o���]���̒��~��F���ł�����
        '���葱���Ă���Ȃ�A����ɂ��Ă͍폜���Ȃ��B
        '�d���̃|�[�g�ɍĐڑ����Ă����Ƃ������Ƃ́A�N���C�A���g���g��
        '�t�@�C���]���������܂��͒��~���Ă���͂��ł��邪�A�]���̒��~��
        'FTP�T�[�o���F�����Ă��Ȃ��Ƃ����̂́A�\���ɂ��蓾�邱�Ƃł���B
        '�ň��A�A�v���ċN�����i�����炭24���Ԃ��Ɓj�ɁA�N���[���A�b�v���s��
        '���߁A�����ł͉����폜���Ȃ��Ƃ����I���������邪�A�g�p���\�[�X��
        '�P�������͖h���ɉz�������Ƃ͂Ȃ����߁A�\�Ȍ���̓��e�����폜����B
        'NOTE: FTP�T�[�o�́A���葱���Ă���t�@�C����������������͂���
        '���邪�A���̑O�ɓ����t�@�C������ULL���悤�Ƃ����N���C�A���g����
        '�v���I�Ȗ�肪�N����悤�ł���Ȃ�A�폜�ł��Ȃ��t�@�C����
        '�������d���p�̃Z�b�V�������m�������Ȃ��iAccept�����
        '�V�[�P���X��BUSY��NAK��Ԃ��j�����悢��������Ȃ��B
        Log.Info("Cleaning up directory [" & sPermittedPath & "]...")
        Utility.CleanUpDirectory(sPermittedPath)
    End Sub

    '�R�l�N�V������ؒf�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionDisappear()
        '-------Ver0.1 ������ԕ�Ή� ADD START-----------
        lineErrorBeginingTime = DateTime.Now
        '-------Ver0.1 ������ԕ�Ή� ADD END-------------
        LineStatus = LineStatus.Disconnected

        If isPseudoConnectionProlongationPeriod Then
            'NOTE: pseudoLineStatus�͐��푤�ɓ]�Ԃ悤�ɂȂ��Ă���B
            '����āA���̃P�[�X�i������Connection��Disappear���꓾��P�[�X�j�ł́A
            'pseudoLineStatus���K��Steady�ł���B
            Log.Error("Closing the pseudo connection because a connection closed during observation period...")
            pseudoLineStatus = LineStatus.Disconnected
            ProcOnPseudoConnectionDisappear()
        Else
            Log.Info("Starting connection observation period...")
            RegisterTimer(oPseudoConnectionProlongationTimer, TickTimer.GetSystemTick())
            isPseudoConnectionProlongationPeriod = True
        End If

        If Not hidesLineErrorFromRecording Then
            Debug.Assert(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks > 0)

            '���W�f�[�^��L�e�[�u���ɒʐM�ُ��o�^����B
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromRecording = True
        End If

        '-------Ver0.1 ������ԕ�Ή� ADD START-----------
        If Not hidesLineErrorFromAlerting Then
            Debug.Assert(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks > 0)

            '�ʐM�ُ�̌x�񃁁[���𐶐�����B
            EmitLineErrorMail()

            RegisterTimer(oLineErrorAlertingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromAlerting = True
        End If
        '-------Ver0.1 ������ԕ�Ή� ADD END-------------
    End Sub

    '�^���R�l�N�V�����J�n
    Protected Overridable Sub ProcOnPseudoConnectionAppear()
        '�ڑ���TRAP�ʒm���s���B
        If TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus <> 0 Then
            SnmpTrap.Act( _
               TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus, _
               sClientModel, clientCode, _
               TelServerAppBaseConfig.IpPortForTelegConnection, _
               SnmpStatusCode.Connect)
        End If

        '�ʐM��ԃe�[�u���ɃR�l�N�V������o�^�B
        InsertDirectConStatus()
    End Sub

    '�^���R�l�N�V�����I��
    Protected Overridable Sub ProcOnPseudoConnectionDisappear()
        '�ؒf��TRAP�ʒm���s���B
        If TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus <> 0 Then
            SnmpTrap.Act( _
               TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus, _
               sClientModel, clientCode, _
               TelServerAppBaseConfig.IpPortForTelegConnection, _
               SnmpStatusCode.Disconnect)
        End If

        '�ʐM��ԃe�[�u������R�l�N�V�������폜�B
        DeleteDirectConStatus()
    End Sub

    Protected Overrides Sub ProcOnUnhandledException(ByVal exc As Exception)
        '�ؒf��TRAP�ʒm���s���B
        If TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus <> 0 Then
            SnmpTrap.Act( _
               TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus, _
               sClientModel, clientCode, _
               TelServerAppBaseConfig.IpPortForTelegConnection, _
               SnmpStatusCode.Disconnect)
        End If

        Try
            '�ʐM��ԃe�[�u������R�l�N�V�������폜�B
            DeleteDirectConStatus()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try

        MyBase.ProcOnUnhandledException(exc)
    End Sub
#End Region

#Region "�C�x���g���������p���\�b�h"
    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        oReqTeleg.ReqNumber = reqNumberForNextSnd
        oReqTeleg.ClientCode = clientCode
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
        'NOTE: �f�[�^��ʂȂǂŕ��򂷂邱�Ƃ��\�B�f�[�^��ʂ��݂�΁A
        '�^�ǃT�[�o��NAK�𑗐M�����ꍇ�Ȃ̂��A�N���C�A���g�@�킪NAK��
        '���M�����ꍇ�Ȃ̂����ʂ��邱�Ƃ��\�B
        Select Case oNakTeleg.CauseCode
            '�p���i���g���C�I�[�o�[�j���Ă��ُ�Ƃ݂͂Ȃ��Ȃ�NAK�d��
            Case EkNakCauseCode.NoData, EkNakCauseCode.Unnecessary
                Return NakRequirement.ForgetOnRetryOver

            '�p���i���g���C�I�[�o�[�j������ُ�Ƃ݂Ȃ��ׂ�NAK�d��
            Case EkNakCauseCode.Busy, EkNakCauseCode.NoTime, EkNakCauseCode.InvalidContent, EkNakCauseCode.UnknownLight
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

    Protected Overridable Sub DeleteDirectConStatus()
        Dim sSQL As String = _
           "DELETE FROM S_DIRECT_CON_STATUS" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
           & " AND UNIT_NO = " & clientCode.Unit.ToString() _
           & " AND PORT_KBN = '" & sPortPurpose & "'"
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Sub InsertDirectConStatus()
        Dim sSQL As String = _
           "INSERT INTO S_DIRECT_CON_STATUS" _
           & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
            & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, PORT_KBN, CONNECT_DATE)" _
           & " VALUES (GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " '" & sClientModel & "'," _
                   & " '" & clientCode.RailSection.ToString("D3") & "'," _
                   & " '" & clientCode.StationOrder.ToString("D3") & "'," _
                   & " " & clientCode.Corner.ToString() & "," _
                   & " " & clientCode.Unit.ToString() & "," _
                   & " '" & sPortPurpose & "'," _
                   & " GETDATE())"
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Function SelectApplicableUnits(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String) As DataTable
        Dim sDataAppModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataPurpose As String = EkMasProListFileName.GetDataPurpose(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim sDataSubKind As String = EkMasProListFileName.GetDataSubKind(sListFileName)
        Dim sDataVersion As string = EkMasProListFileName.GetDataVersion(sListFileName)
        Dim sListVersion As string = EkMasProListFileName.GetListVersion(sListFileName)
        Dim sClientRailSection As string = clientCode.RailSection.ToString("D3")
        Dim sClientStationOrder As string = clientCode.StationOrder.ToString("D3")
        Dim sClientCorner As string = clientCode.Corner.ToString()
        Dim sClientUnit As string = clientCode.Unit.ToString()

        '�z�M�J�n�������擾�B
        'NOTE: Client���p��������́A���ݓ����̕����߂���������Ȃ����A
        '���������z�M���Client�����߂�ۂɂ����p���Ă��邽�߁A
        '�����p���邱�Ƃɂ���B
        Dim sSQLToSelectDllStartTime As String = _
           "SELECT DELIVERY_START_TIME" _
           & " FROM S_" & sDataPurpose & "_DLL_STS" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
           & " AND DATA_KIND = '" & sDataKind & "'" _
           & " AND DATA_SUB_KIND = '" & sDataSubKind & "'" _
           & " AND DATA_VERSION = '" & sDataVersion & "'" _
           & " AND VERSION = '" & sListVersion & "'" _
           & " AND RAIL_SECTION_CODE = '" & sClientRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sClientStationOrder & "'" _
           & " AND CORNER_CODE = " & sClientCorner _
           & " AND UNIT_NO = " & sClientUnit
        Dim sDeliveryStartTime As String = CStr(dbCtl.ExecuteSQLToReadScalar(sSQLToSelectDllStartTime))
        Dim dllStartTime As DateTime = DateTime.ParseExact(sDeliveryStartTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)

        '���ʕ����e�[�u���i�@��\���}�X�^�̔z�M�w�����_�̗L���v�f�j���`����SQL��ҏW�B
        Dim sSQLToDefineCTE As String = _
           "WITH M_SERVICE_MACHINE (MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, ADDRESS, MONITOR_ADDRESS)" _
           & " AS" _
           & " (SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, ADDRESS, MONITOR_ADDRESS" _
               & " FROM M_MACHINE" _
               & " WHERE SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                              & " FROM M_MACHINE" _
                                              & " WHERE SETTING_START_DATE <= '" & sDeliveryStartDate & "'" _
                                              & " AND INSERT_DATE <= CONVERT(DATETIME, '" & dllStartTime.ToString("yyyy/MM/dd HH:mm:ss") & "', 120))) "

        '�z�M�w���̎��_�ł�Client��IP�A�h���X���擾����SQL��ҏW�B
        'NOTE: �@��\���}�X�^�o�^���_�̃`�F�b�N�ɂ��S�ē���ł���A
        '�z�M���J�n�ł����Ƃ������ƂŁA���Ȃ��Ƃ��P�͑��݂��Ă���
        '�i�􂢑ւ��o�b�`�����p�ȍ폜�͂��Ȃ��j���̂Ƃ���B
        Dim sSQLToSelectAddrOfClient As String = _
           "SELECT TOP 1 ADDRESS" _
           & " FROM M_SERVICE_MACHINE" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & sClientRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sClientStationOrder & "'" _
           & " AND CORNER_CODE = " & sClientCorner _
           & " AND UNIT_NO = " & sClientUnit _
           & " AND ADDRESS <> ''"

        '��LIP�A�h���X���ڑ���Ɏw�肳��Ă���z�M�w���̎��_�ŗL����
        '�@��\���}�X�^�̃��R�[�h���擾����SQL��ҏW�B
        Dim sSQLToSelectUnitsUnderClient As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM M_SERVICE_MACHINE" _
           & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
           & " AND MONITOR_ADDRESS = (" & sSQLToSelectAddrOfClient & ")"

        '�K�p�摕�u�̐���`���@���擾����SQL��ҏW�B
        Dim sSQLToSelectApplicableUnits As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM S_" & sDataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        'NOTE: �v���O�����K�p���X�g�̏ꍇ�́A�L���ȍs�𒊏o����ɂ�����A
        '�K�p���ɂ��ƂÂ��ǉ��̏������������Ă���B�Ȃ��A�u�����N��
        '�ǂ̂悤�ȓ��t�i������j�����������Ƃ݂Ȃ����z��ł���B
        If sDataPurpose.Equals(EkConstants.DataPurposeProgram) Then
            sSQLToSelectApplicableUnits = sSQLToSelectApplicableUnits _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                    & " OR APPLICABLE_DATE = '19000101'" _
                    & " OR APPLICABLE_DATE = '99999999')"
        End If

        'Client�z���̓K�p�摕�u�̐���`���@���擾����SQL�����s�B
        Dim sSQLToSelectApplicableUnitsUnderClient As String = _
           sSQLToSelectUnitsUnderClient & " INTERSECT " & sSQLToSelectApplicableUnits
        Return  dbCtl.ExecuteSQLToRead(sSQLToDefineCTE & sSQLToSelectApplicableUnitsUnderClient)
    End Function

    Protected Overridable Function SelectMasProDataFileName(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String) As String
        Dim sDataAppModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataPurpose As String = EkMasProListFileName.GetDataPurpose(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim sDataSubKind As String = EkMasProListFileName.GetDataSubKind(sListFileName)
        Dim sDataVersion As string = EkMasProListFileName.GetDataVersion(sListFileName)

        Dim sSQL As String = _
           "SELECT FILE_NAME" _
           & " FROM S_" & sDataPurpose & "_DATA_HEADLINE" _
           & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
           & " AND DATA_KIND = '" & sDataKind & "'" _
           & " AND DATA_SUB_KIND = '" & sDataSubKind & "'" _
           & " AND DATA_VERSION = '" & sDataVersion & "'"
        Return CStr(dbCtl.ExecuteSQLToReadScalar(sSQL))
    End Function

    'DLL��ԃe�[�u���̃f�[�^�{�̗p���R�[�h���X�V����B
    Protected Overridable Sub UpdateDllStatusForData(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String, ByVal status As Integer)
        Dim sDeliveryEndTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")

        Dim sSQL As String = _
           "UPDATE S_" & EkMasProListFileName.GetDataPurpose(sListFileName) & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_END_TIME = '" & sDeliveryEndTime & "'," _
               & " DELIVERY_STS = " & status.ToString() _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
           & " AND DATA_KIND = '" & EkMasProListFileName.GetDataKind(sListFileName) & "'" _
           & " AND DATA_SUB_KIND = '" & EkMasProListFileName.GetDataSubKind(sListFileName) & "'" _
           & " AND DATA_VERSION = '" & EkMasProListFileName.GetDataVersion(sListFileName) & "'" _
           & " AND VERSION = '" & EkMasProListFileName.GetDataVersion(sListFileName) & "'" _
           & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
           & " AND UNIT_NO = " & clientCode.Unit.ToString()
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    'DLL��ԃe�[�u���̓K�p���X�g�p���R�[�h���X�V����B
    Protected Overridable Sub UpdateDllStatusForList(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String, ByVal status As Integer)
        Dim sDeliveryEndTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")

        Dim sSQL As String = _
           "UPDATE S_" & EkMasProListFileName.GetDataPurpose(sListFileName) & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_END_TIME = '" & sDeliveryEndTime & "'," _
               & " DELIVERY_STS = " & status.ToString() _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
           & " AND DATA_KIND = '" & EkMasProListFileName.GetDataKind(sListFileName) & "'" _
           & " AND DATA_SUB_KIND = '" & EkMasProListFileName.GetDataSubKind(sListFileName) & "'" _
           & " AND DATA_VERSION = '" & EkMasProListFileName.GetDataVersion(sListFileName) & "'" _
           & " AND VERSION = '" & EkMasProListFileName.GetListVersion(sListFileName) & "'" _
           & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
           & " AND UNIT_NO = " & clientCode.Unit.ToString()
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    'DL��ԃe�[�u�����u�z�M���v�ɕύX����B
    Protected Overridable Sub UpdateDlStatusToExecutingIfNeeded(ByVal dbCtl As DatabaseTalker, ByVal appUnitTable As DataTable, ByVal sListFileName As String)
        'sListFileName�������K�p���X�g��@��\���ɉ����āA
        '�z�M��ƂȂ鍆�@���u�K�p���X�g�v�u�f�[�^�{�́v���ꂼ���
        '���ē����o���ADL��ԃe�[�u���ɓ��Y���R�[�h���܂����݂��Ȃ�
        '�ꍇ�̂݁A���R�[�h��ǉ�����i��Ԃ́u�z�M���v�Ƃ���j�B

        Dim sDataAppModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataPurpose As String = EkMasProListFileName.GetDataPurpose(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim sDataSubKind As String = EkMasProListFileName.GetDataSubKind(sListFileName)
        Dim sDataVersion As string = EkMasProListFileName.GetDataVersion(sListFileName)
        Dim sListVersion As string = EkMasProListFileName.GetListVersion(sListFileName)
        Dim appUnits As DataRowCollection = appUnitTable.Rows

        'NOTE: �ȉ��̓_�ɒ��ӁB
        '(1) Client�́A�}�X�^�K�p���X�g�ɂ��ẮAappUnit�֔z�M���Ȃ��B
        '(2) Client�́A���Ƃ��p�^�[���ԍ�������Ă��Ă��A����ȊO�̃L�[��
        '    �ߋ��ɑ��������̂Ɠ����Ȃ�A�}�X�^�{�̂ɂ��Ă�appUnit��
        '    �z�M���Ȃ��iappUnit�����ɗ��Ȃ��j�B
        '(3) Client�́A�v���O�����K�p���X�g�ɂ��ẮAappUnit�֕K���z�M
        '    ����i�z�M���Ȃ���΂Ȃ�Ȃ��j�B����āA�V���Ȕz�M�w����
        '    ����������ɂ́A�ߋ��ɔz�M���������Ă��邱�ƂɈӖ��͂Ȃ��B
        '    �������A���ۂɔz�M���s����܂ŁA�ߋ��̌��ʂ��\���Ɏc��̂�
        '    �v�����Ȃ��i�z�M���������̑O��֌W�ŁA�ߋ��̌��ʂƂ킩��j�B
        '(4) �v���O�����̏ꍇ�A�}�X�^�̏ꍇ�ƈقȂ�ADATA_SUB_KIND��
        '    �L�[�łȂ��B

        'OPT: ��{�I�ɂ�����INSERT�����s����K�v�͂Ȃ��͂��B

        If sDataPurpose.Equals(EkConstants.DataPurposeMaster) Then
            For Each appUnit As DataRow In appUnits
                Dim sAppRailSection As String = appUnit.Field(Of String)("RAIL_SECTION_CODE")
                Dim sAppStationOrder As String = appUnit.Field(Of String)("STATION_ORDER_CODE")
                Dim sAppCorner As String = appUnit.Field(Of Integer)("CORNER_CODE").ToString()
                Dim sAppUnit As String = appUnit.Field(Of Integer)("UNIT_NO").ToString()

                Dim sSQLToSelectMstDlStsAboutAnySubKind As String = _
                   "SELECT *" _
                   & " FROM S_" & sDataPurpose & "_DL_STS" _
                   & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                   & " AND DATA_KIND = '" & sDataKind & "'" _
                   & " AND VERSION = '" & sDataVersion & "'" _
                   & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
                   & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
                   & " AND CORNER_CODE = " & sAppCorner _
                   & " AND UNIT_NO = " & sAppUnit _
                   & " AND DELIVERY_STS <> " & DbConstants.DlStatusPreExecuting.ToString()

                '�}�X�^DL��ԃe�[�u���ɐV�K�́i�z�M���ʂ��u�z�M���v�́j���R�[�h��
                '�ǉ�����i�����̃��R�[�h������΁u�z�M���v�ύX����jSQL�����s�B
                'NOTE: ���Y���@�Ɋւ��āA���Ƃ��p�^�[��No���قȂ��Ă��A��ʂ�
                '�o�[�W��������v����z�M���ʂ��P���ł����݂���ꍇ�́A�ΏۊO�Ƃ���B
                Dim sSQLToUpdateMstDlStsAboutData As String = _
                   "MERGE INTO S_" & sDataPurpose & "_DL_STS AS Target" _
                   & " USING (SELECT '" & sDataAppModel & "' MODEL_CODE," _
                                 & " '" & EkConstants.FilePurposeData & "' FILE_KBN," _
                                 & " '" & sDataKind & "' DATA_KIND," _
                                 & " '" & sDataSubKind & "' DATA_SUB_KIND," _
                                 & " '" & sDataVersion & "' VERSION," _
                                 & " '" & sAppRailSection & "' RAIL_SECTION_CODE," _
                                 & " '" & sAppStationOrder & "' STATION_ORDER_CODE," _
                                 & " " & sAppCorner & " CORNER_CODE," _
                                 & " " & sAppUnit & " UNIT_NO) AS Source" _
                   & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                     & " AND Target.FILE_KBN = Source.FILE_KBN" _
                     & " AND Target.DATA_KIND = Source.DATA_KIND" _
                     & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
                     & " AND Target.VERSION = Source.VERSION" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED" _
                   & " AND NOT EXISTS (" & sSQLToSelectMstDlStsAboutAnySubKind & ") THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DELIVERY_STS = " & DbConstants.DlStatusExecuting.ToString() _
                   & " WHEN NOT MATCHED" _
                   & " AND NOT EXISTS (" & sSQLToSelectMstDlStsAboutAnySubKind & ") THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " DATA_SUB_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " Source.MODEL_CODE," _
                            & " Source.FILE_KBN," _
                            & " Source.DATA_KIND," _
                            & " Source.DATA_SUB_KIND," _
                            & " Source.VERSION," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateMstDlStsAboutData)
            Next appUnit
        Else
            For Each appUnit As DataRow In appUnits
                Dim sAppRailSection As String = appUnit.Field(Of String)("RAIL_SECTION_CODE")
                Dim sAppStationOrder As String = appUnit.Field(Of String)("STATION_ORDER_CODE")
                Dim sAppCorner As String = appUnit.Field(Of Integer)("CORNER_CODE").ToString()
                Dim sAppUnit As String = appUnit.Field(Of Integer)("UNIT_NO").ToString()

                Dim sSQLToSelectPrgDlSts As String = _
                   "SELECT *" _
                   & " FROM S_" & sDataPurpose & "_DL_STS" _
                   & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                   & " AND DATA_KIND = '" & sDataKind & "'" _
                   & " AND VERSION = '" & sDataVersion & "'" _
                   & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
                   & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
                   & " AND CORNER_CODE = " & sAppCorner _
                   & " AND UNIT_NO = " & sAppUnit _
                   & " AND DELIVERY_STS <> " & DbConstants.DlStatusPreExecuting.ToString()

                '�v���O����DL��ԃe�[�u���ɐV�K�́i�z�M���ʂ��u�z�M���v�́j���R�[�h��
                '�ǉ�����i�����̃��R�[�h������΁u�z�M���v�ύX����jSQL�����s�B

                '�v���O����DL��ԃe�[�u���Ƀv���O�����{�̂Ɋւ���V�K��
                '�i�z�M���ʂ��u�z�M���v�́j���R�[�h��ǉ�����i������
                '���R�[�h������΁u�z�M���v�ύX����jSQL�����s�B
                Dim sSQLToUpdatePrgDlStsAboutData As String = _
                   "MERGE INTO S_" & sDataPurpose & "_DL_STS AS Target" _
                   & " USING (SELECT '" & sDataAppModel & "' MODEL_CODE," _
                                 & " '" & EkConstants.FilePurposeData & "' FILE_KBN," _
                                 & " '" & sDataKind & "' DATA_KIND," _
                                 & " '" & sDataVersion & "' VERSION," _
                                 & " '" & sAppRailSection & "' RAIL_SECTION_CODE," _
                                 & " '" & sAppStationOrder & "' STATION_ORDER_CODE," _
                                 & " " & sAppCorner & " CORNER_CODE," _
                                 & " " & sAppUnit & " UNIT_NO) AS Source" _
                   & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                     & " AND Target.FILE_KBN = Source.FILE_KBN" _
                     & " AND Target.DATA_KIND = Source.DATA_KIND" _
                     & " AND Target.VERSION = Source.VERSION" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED" _
                   & " AND NOT EXISTS (" & sSQLToSelectPrgDlSts & ") THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DELIVERY_STS = " & DbConstants.DlStatusExecuting.ToString() _
                   & " WHEN NOT MATCHED" _
                   & " AND NOT EXISTS (" & sSQLToSelectPrgDlSts & ") THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " Source.MODEL_CODE," _
                            & " Source.FILE_KBN," _
                            & " Source.DATA_KIND," _
                            & " Source.VERSION," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdatePrgDlStsAboutData)

                'NOTE: �K�p���X�g�̏ꍇ�AVERSION�ɂ͓K�p���X�g�o�[�W����������B
                'NOTE: �K�p���X�g�̏ꍇ�A�ߋ��̂��̂Ɠ���VERSION�œ��e�̈قȂ郊�X�g��
                '�z�M���邱�Ƃ��A���ʂɂ��蓾��B�܂��A�v���O�����K�p���X�g�̏ꍇ�A
                'appUnit�͂����K��appUnit�ɔz�M����B����āA���̃P�[�X�ł́A
                '����VERSION�̃��R�[�h�����ɑ��݂��Ă���ꍇ���A��x�u�z�M���v��
                '���������悢���A�������邱�Ƃ��\�ł���i�ň��A�z�M����������
                '�O��֌W�Ŏ��ۂ͉��߂ł���͂��ł��邪�A���ꂾ�ƕ�����h���j�B
                '�������A�����ł͊��Ƀ��R�[�h�����݂��Ă���ꍇ�́u�z�M���v�ɂ͂��Ȃ��B
                'DLL�V�[�P���X����������i���̃��\�b�h���Ă΂��j�O�ɁADL�����ʒm��
                '��M���Ă���i�ŏI�I�Ȓl�Ń��R�[�h���쐬����Ă���j�\�����F���Ƃ�
                '�����Ȃ����߂ł���B���̂����A�^�ǒ[������̔z�M�w�����󂯓��ꂽ
                '���_�Łi������̃v���Z�X�ɔz�M�w�����b�Z�[�W�𑗐M����O�Ɂj
                '���Y���R�[�h���폜���Ă����΂悢�B�o�[�W�������P�������i����������
                '�o�[�W�����ŐV���Ȕz�M�w�����s�����j�Ƃ������Ƃ́A���̃��R�[�h��
                '�ێ��ΏۊO�Ƃ݂Ȃ����Ƃ��ł���̂ŁA�d�l�I�Ȗ��͂Ȃ��B
                '�܂��A���̃v���Z�X�ɂ�����z�M�J�n��DLL�V�[�P���X�I�����_��
                '���R�[�h���폜����������R�ȓ���ɂȂ�B�����̕��@���ƁA�^�ǒ[��
                '���z�M�w�����󂯓���Ă��炻�̎��_�܂ł̊Ԃɔz�M�𒆎~���邱�Ƃ�
                '�Ȃ�����ADL��ԗ�ɉߋ��̏�񂪕\������邱�ƂɂȂ��Ă��܂��B

                '�v���O����DL��ԃe�[�u���ɓK�p���X�g�Ɋւ���V�K��
                '�i�z�M���ʂ��u�z�M���v�́j���R�[�h��ǉ�����i������
                '���R�[�h������΁u�z�M���v�ύX����jSQL�����s�B
                Dim sSQLToUpdatePrgDlStsAboutList As String = _
                   "MERGE INTO S_" & sDataPurpose & "_DL_STS AS Target" _
                   & " USING (SELECT '" & sDataAppModel & "' MODEL_CODE," _
                                 & " '" & EkConstants.FilePurposeList & "' FILE_KBN," _
                                 & " '" & sDataKind & "' DATA_KIND," _
                                 & " '" & sListVersion & "' VERSION," _
                                 & " '" & sAppRailSection & "' RAIL_SECTION_CODE," _
                                 & " '" & sAppStationOrder & "' STATION_ORDER_CODE," _
                                 & " " & sAppCorner & " CORNER_CODE," _
                                 & " " & sAppUnit & " UNIT_NO) AS Source" _
                   & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                     & " AND Target.FILE_KBN = Source.FILE_KBN" _
                     & " AND Target.DATA_KIND = Source.DATA_KIND" _
                     & " AND Target.VERSION = Source.VERSION" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DELIVERY_STS = " & DbConstants.DlStatusExecuting.ToString() _
                   & " WHEN NOT MATCHED THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " Source.MODEL_CODE," _
                            & " Source.FILE_KBN," _
                            & " Source.DATA_KIND," _
                            & " Source.VERSION," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdatePrgDlStsAboutList)
            Next appUnit
        End If
    End Sub

    'DL��ԃe�[�u�����X�V����B
    'NOTE: DLL�V�[�P���X�̊��������DL�����ʒm����M����\�����F���ł͂Ȃ�
    '���߁A���R�[�h���Ȃ��ꍇ�͐V�K�ɒǉ�����iMERGE���g���j�B
    Protected Overridable Sub UpdateOrInsertDlStatus(ByVal dbCtl As DatabaseTalker, ByVal oRcvTeleg As EkMasProDlReflectReqTelegram)
        Dim oSpec As TelServerAppMasProDlReflectSpec = oMasProDlReflectSpecOfCplxObjCodes(GenCplxObjCode(oRcvTeleg.ObjCode, oRcvTeleg.SubObjCode))

        '�ȉ��̏������ƂɈ�ӂƂȂ郌�R�[�h�ɑ΂��ADELIVERY_STS��oRcvTeleg.EatResult���Z�b�g����B
        'oSpec.DataPurpose �i�e�[�u�����Ƃ��Ďg�p�j
        'oSpec.ApplicableModel
        'oSpec.FilePurpose
        'oSpec.DataKind
        'oRcvTeleg.PatternNumber �ioSpec.DataPurpose��EkConstants.DataPurposeMaster�̏ꍇ�����g�p�j
        'oRcvTeleg.VersionNumber
        'oRcvTeleg.EatClientCode

        Dim sDeliveryEndTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")

        Dim sSQL As String
        If oSpec.DataPurpose.Equals(EkConstants.DataPurposeMaster) Then
            Dim sVerFormat As String
            If oSpec.FilePurpose.Equals(EkConstants.FilePurposeList) Then
                sVerFormat = "D2" 'NOTE: ���̂Ƃ��낱�������삷��v���g�R���͖����B
            Else
                sVerFormat = "D3"
            End If

            '-------Ver0.1�@�t�F�[�Y�Q�@�u�K�p�ς݁v��Ԃ�ǉ��@MOD START-----------
            sSQL = _
               "MERGE INTO S_" & oSpec.DataPurpose & "_DL_STS AS Target" _
               & " USING (SELECT '" & oSpec.ApplicableModel & "' MODEL_CODE," _
                             & " '" & oSpec.FilePurpose & "' FILE_KBN," _
                             & " '" & oSpec.DataKind & "' DATA_KIND," _
                             & " '" & oRcvTeleg.PatternNumber.ToString("D2") & "' DATA_SUB_KIND," _
                             & " '" & oRcvTeleg.VersionNumber.ToString(sVerFormat) & "' VERSION," _
                             & " '" & oRcvTeleg.EatClientCode.RailSection.ToString("D3") & "' RAIL_SECTION_CODE," _
                             & " '" & oRcvTeleg.EatClientCode.StationOrder.ToString("D3") & "' STATION_ORDER_CODE," _
                             & " " & oRcvTeleg.EatClientCode.Corner.ToString() & " CORNER_CODE," _
                             & " " & oRcvTeleg.EatClientCode.Unit.ToString() & " UNIT_NO) AS Source" _
               & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                 & " AND Target.FILE_KBN = Source.FILE_KBN" _
                 & " AND Target.DATA_KIND = Source.DATA_KIND" _
                 & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
                 & " AND Target.VERSION = Source.VERSION" _
                 & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                 & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                 & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                 & " AND Target.UNIT_NO = Source.UNIT_NO)" _
               & " WHEN MATCHED" _
               & " AND (" & oRcvTeleg.EatResult.ToString() & " <> " & DbConstants.DlStatusContinuingNormal.ToString() _
                 & " OR Target.DELIVERY_STS <> " & DbConstants.DlStatusNormal.ToString() & ") THEN" _
                & " UPDATE" _
                 & " SET Target.UPDATE_DATE = GETDATE()," _
                     & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                     & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                     & " Target.DELIVERY_END_TIME = '" & sDeliveryEndTime & "'," _
                     & " Target.DELIVERY_STS = " & oRcvTeleg.EatResult.ToString() _
               & " WHEN NOT MATCHED THEN" _
                & " INSERT (INSERT_DATE," _
                        & " INSERT_USER_ID," _
                        & " INSERT_MACHINE_ID," _
                        & " UPDATE_DATE," _
                        & " UPDATE_USER_ID," _
                        & " UPDATE_MACHINE_ID," _
                        & " MODEL_CODE," _
                        & " FILE_KBN," _
                        & " DATA_KIND," _
                        & " DATA_SUB_KIND," _
                        & " VERSION," _
                        & " RAIL_SECTION_CODE," _
                        & " STATION_ORDER_CODE," _
                        & " CORNER_CODE," _
                        & " UNIT_NO," _
                        & " DELIVERY_END_TIME," _
                        & " DELIVERY_STS)" _
                & " VALUES (GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " Source.MODEL_CODE," _
                        & " Source.FILE_KBN," _
                        & " Source.DATA_KIND," _
                        & " Source.DATA_SUB_KIND," _
                        & " Source.VERSION," _
                        & " Source.RAIL_SECTION_CODE," _
                        & " Source.STATION_ORDER_CODE," _
                        & " Source.CORNER_CODE," _
                        & " Source.UNIT_NO," _
                        & " '" & sDeliveryEndTime & "'," _
                        & " " & oRcvTeleg.EatResult.ToString() & ");"
            '-------Ver0.1�@�t�F�[�Y�Q�@�u�K�p�ς݁v��Ԃ�ǉ��@MOD END-----------
        Else
            Dim sVerFormat As String
            If oSpec.FilePurpose.Equals(EkConstants.FilePurposeList) Then
                sVerFormat = "D2"
            Else
                sVerFormat = EkConstants.ProgramDataVersionFormatOfModels(oSpec.ApplicableModel)
            End If

            '-------Ver0.1�@�t�F�[�Y�Q�@�u�K�p�ς݁v��Ԃ�ǉ��@MOD START-----------
            sSQL = _
               "MERGE INTO S_" & oSpec.DataPurpose & "_DL_STS AS Target" _
               & " USING (SELECT '" & oSpec.ApplicableModel & "' MODEL_CODE," _
                             & " '" & oSpec.FilePurpose & "' FILE_KBN," _
                             & " '" & oSpec.DataKind & "' DATA_KIND," _
                             & " '" & oRcvTeleg.VersionNumber.ToString(sVerFormat) & "' VERSION," _
                             & " '" & oRcvTeleg.EatClientCode.RailSection.ToString("D3") & "' RAIL_SECTION_CODE," _
                             & " '" & oRcvTeleg.EatClientCode.StationOrder.ToString("D3") & "' STATION_ORDER_CODE," _
                             & " " & oRcvTeleg.EatClientCode.Corner.ToString() & " CORNER_CODE," _
                             & " " & oRcvTeleg.EatClientCode.Unit.ToString() & " UNIT_NO) AS Source" _
               & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                 & " AND Target.FILE_KBN = Source.FILE_KBN" _
                 & " AND Target.DATA_KIND = Source.DATA_KIND" _
                 & " AND Target.VERSION = Source.VERSION" _
                 & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                 & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                 & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                 & " AND Target.UNIT_NO = Source.UNIT_NO)" _
               & " WHEN MATCHED" _
               & " AND (" & oRcvTeleg.EatResult.ToString() & " <> " & DbConstants.DlStatusContinuingNormal.ToString() _
                 & " OR Target.DELIVERY_STS <> " & DbConstants.DlStatusNormal.ToString() & ") THEN" _
                & " UPDATE" _
                 & " SET Target.UPDATE_DATE = GETDATE()," _
                     & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                     & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                     & " Target.DELIVERY_END_TIME = '" & sDeliveryEndTime & "'," _
                     & " Target.DELIVERY_STS = " & oRcvTeleg.EatResult.ToString() _
               & " WHEN NOT MATCHED THEN" _
                & " INSERT (INSERT_DATE," _
                        & " INSERT_USER_ID," _
                        & " INSERT_MACHINE_ID," _
                        & " UPDATE_DATE," _
                        & " UPDATE_USER_ID," _
                        & " UPDATE_MACHINE_ID," _
                        & " MODEL_CODE," _
                        & " FILE_KBN," _
                        & " DATA_KIND," _
                        & " VERSION," _
                        & " RAIL_SECTION_CODE," _
                        & " STATION_ORDER_CODE," _
                        & " CORNER_CODE," _
                        & " UNIT_NO," _
                        & " DELIVERY_END_TIME," _
                        & " DELIVERY_STS)" _
                & " VALUES (GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " Source.MODEL_CODE," _
                        & " Source.FILE_KBN," _
                        & " Source.DATA_KIND," _
                        & " Source.VERSION," _
                        & " Source.RAIL_SECTION_CODE," _
                        & " Source.STATION_ORDER_CODE," _
                        & " Source.CORNER_CODE," _
                        & " Source.UNIT_NO," _
                        & " '" & sDeliveryEndTime & "'," _
                        & " " & oRcvTeleg.EatResult.ToString() & ");"
            '-------Ver0.1�@�t�F�[�Y�Q�@�u�K�p�ς݁v��Ԃ�ǉ��@MOD END-----------
        End If
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    Protected Overridable Sub UpdateDllVersionUncertainFlag(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String, ByVal sUncertainFlg As String)
        Dim sSQL As String = _
           "UPDATE S_" & EkMasProListFileName.GetDataPurpose(sListFileName) & "_DLL_VER" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " UNCERTAIN_FLG = '" & sUncertainFlg & "'" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND DATA_KIND = '" & EkMasProListFileName.GetDataKind(sListFileName) & "'" _
           & " AND DATA_SUB_KIND = '" & EkMasProListFileName.GetDataSubKind(sListFileName) & "'" _
           & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
           & " AND UNIT_NO = " & clientCode.Unit.ToString()
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    Protected Overridable Sub UpdateOrInsertDllVersion(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String)
        Dim sSQL As String = _
           "MERGE INTO S_" & EkMasProListFileName.GetDataPurpose(sListFileName) & "_DLL_VER AS Target" _
           & " USING (SELECT '" & sClientModel & "' MODEL_CODE," _
                         & " '" & EkMasProListFileName.GetDataKind(sListFileName) & "' DATA_KIND," _
                         & " '" & EkMasProListFileName.GetDataSubKind(sListFileName) & "' DATA_SUB_KIND," _
                         & " '" & clientCode.RailSection.ToString("D3") & "' RAIL_SECTION_CODE," _
                         & " '" & clientCode.StationOrder.ToString("D3") & "' STATION_ORDER_CODE," _
                         & " " & clientCode.Corner.ToString() & " CORNER_CODE," _
                         & " " & clientCode.Unit.ToString() & " UNIT_NO," _
                         & " '" & EkMasProListFileName.GetDataVersion(sListFileName) & "' DATA_VERSION) AS Source" _
           & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
             & " AND Target.DATA_KIND = Source.DATA_KIND" _
             & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
             & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
             & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
             & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
             & " AND Target.UNIT_NO = Source.UNIT_NO)" _
           & " WHEN MATCHED THEN" _
            & " UPDATE" _
             & " SET Target.UPDATE_DATE = GETDATE()," _
                 & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                 & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                 & " Target.DATA_VERSION = Source.DATA_VERSION," _
                 & " Target.UNCERTAIN_FLG = '0'" _
           & " WHEN NOT MATCHED THEN" _
            & " INSERT (INSERT_DATE," _
                    & " INSERT_USER_ID," _
                    & " INSERT_MACHINE_ID," _
                    & " UPDATE_DATE," _
                    & " UPDATE_USER_ID," _
                    & " UPDATE_MACHINE_ID," _
                    & " MODEL_CODE," _
                    & " DATA_KIND," _
                    & " DATA_SUB_KIND," _
                    & " RAIL_SECTION_CODE," _
                    & " STATION_ORDER_CODE," _
                    & " CORNER_CODE," _
                    & " UNIT_NO," _
                    & " DATA_VERSION," _
                    & " UNCERTAIN_FLG)" _
            & " VALUES (GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " Source.MODEL_CODE," _
                    & " Source.DATA_KIND," _
                    & " Source.DATA_SUB_KIND," _
                    & " Source.RAIL_SECTION_CODE," _
                    & " Source.STATION_ORDER_CODE," _
                    & " Source.CORNER_CODE," _
                    & " Source.UNIT_NO," _
                    & " Source.DATA_VERSION," _
                    & " '0');"
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    Protected Overridable Sub DeleteAndInsertMasterVersionInfoExpected(ByVal dbCtl As DatabaseTalker, ByVal appUnitTable As DataTable, ByVal sDataFileName As String)
        '(1)sDataFileName�Ɋ܂܂��@��E��ʁE�p�^�[��No�E�}�X�^�o�[�W�������擾����B
        '(2)�}�X�^�o�[�W���������Ғl�e�[�u���ɑ΂��A(1)�́u�@��E��ʁv�����
        'appUnitTable�̍��@�ƈ�v����S�Ẵ��R�[�h��(1)�Ŏ擾�����u�}�X�^�o�[�W�����v
        '����сu�p�^�[��No�v�ɂ���āA�X�V�i�Ȃ���΍쐬�j����B

        Dim sDataAppModel As String = EkMasterDataFileName.GetApplicableModel(sDataFileName)
        Dim sDataKind As String = EkMasterDataFileName.GetKind(sDataFileName)
        Dim sDataSubKind As String = EkMasterDataFileName.GetSubKind(sDataFileName)
        Dim sDataVersion As string = EkMasterDataFileName.GetVersion(sDataFileName)
        Dim appUnits As DataRowCollection = appUnitTable.Rows

        For Each appUnit As DataRow In appUnits
            Dim sAppRailSection As String = appUnit.Field(Of String)("RAIL_SECTION_CODE")
            Dim sAppStationOrder As String = appUnit.Field(Of String)("STATION_ORDER_CODE")
            Dim sAppCorner As String = appUnit.Field(Of Integer)("CORNER_CODE").ToString()
            Dim sAppUnit As String = appUnit.Field(Of Integer)("UNIT_NO").ToString()

            Dim sSQL As String = _
               "MERGE INTO S_" & EkConstants.DataPurposeMaster & "_VER_INFO_EXPECTED AS Target" _
               & " USING (SELECT '" & sDataAppModel & "' MODEL_CODE," _
                             & " '" & sAppRailSection & "' RAIL_SECTION_CODE," _
                             & " '" & sAppStationOrder & "' STATION_ORDER_CODE," _
                             & " " & sAppCorner & " CORNER_CODE," _
                             & " " & sAppUnit & " UNIT_NO," _
                             & " '" & sDataKind & "' DATA_KIND," _
                             & " '" & sDataSubKind & "' DATA_SUB_KIND," _
                             & " '" & sDataVersion & "' DATA_VERSION) AS Source" _
               & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                 & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                 & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                 & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                 & " AND Target.UNIT_NO = Source.UNIT_NO" _
                 & " AND Target.DATA_KIND = Source.DATA_KIND)" _
               & " WHEN MATCHED THEN" _
                & " UPDATE" _
                 & " SET Target.UPDATE_DATE = GETDATE()," _
                     & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                     & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                     & " Target.DATA_SUB_KIND = Source.DATA_SUB_KIND," _
                     & " Target.DATA_VERSION = Source.DATA_VERSION" _
               & " WHEN NOT MATCHED THEN" _
                & " INSERT (INSERT_DATE," _
                        & " INSERT_USER_ID," _
                        & " INSERT_MACHINE_ID," _
                        & " UPDATE_DATE," _
                        & " UPDATE_USER_ID," _
                        & " UPDATE_MACHINE_ID," _
                        & " MODEL_CODE," _
                        & " RAIL_SECTION_CODE," _
                        & " STATION_ORDER_CODE," _
                        & " CORNER_CODE," _
                        & " UNIT_NO," _
                        & " DATA_KIND," _
                        & " DATA_SUB_KIND," _
                        & " DATA_VERSION)" _
                & " VALUES (GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " Source.MODEL_CODE," _
                        & " Source.RAIL_SECTION_CODE," _
                        & " Source.STATION_ORDER_CODE," _
                        & " Source.CORNER_CODE," _
                        & " Source.UNIT_NO," _
                        & " Source.DATA_KIND," _
                        & " Source.DATA_SUB_KIND," _
                        & " Source.DATA_VERSION);"
            dbCtl.ExecuteSQLToWrite(sSQL)
        Next appUnit
    End Sub

    Protected Overridable Sub DeleteAndInsertProgramVersionInfoExpected(ByVal dbCtl As DatabaseTalker, ByVal appUnitTable As DataTable, ByVal sDataFileName As String)
        '(1)sDataFileName�Ɋ܂܂��@��E��ʂ��擾����B
        '(2)sDataFileName���L�[�Ƀv���O�����f�[�^���e�e�[�u������CAB�Ɋ܂܂��o�[�W�����̈ꗗ���擾����B
        '(3)�v���O�����o�[�W���������Ғl�e�[�u���ɂ����āA(1)�́u�@��E��ʁv�����appUnitTable�̍��@��
        '  �L�[�ɂ���S�Ẵ��R�[�h���폜����B
        '(4)�v���O�����o�[�W���������Ғl�e�[�u���ɑ΂��A(1)�́u�@��E��ʁv�AappUnitTable�̍��@
        '  �����(2)�Ŏ擾�����ꗗ�����ƂɁA�V���ȃ��R�[�h���쐬����B

        Dim sDataAppModel As String = EkProgramDataFileName.GetApplicableModel(sDataFileName)
        Dim appUnits As DataRowCollection = appUnitTable.Rows

        Dim sSQLToSelectRegInfos As String = _
           "SELECT ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME" _
           & " FROM S_" & EkConstants.DataPurposeProgram & "_DATA" _
           & " WHERE FILE_NAME = '" & sDataFileName & "'"
        Dim regInfos As DataRowCollection = dbCtl.ExecuteSQLToRead(sSQLToSelectRegInfos).Rows

        Dim oStringBuilder As New StringBuilder()
        For Each regInfo As DataRow In regInfos
            oStringBuilder.Append( _
               " (GETDATE()," _
               & " '" & UserId & "'," _
               & " '" & MachineId & "'," _
               & " GETDATE()," _
               & " '" & UserId & "'," _
               & " '" & MachineId & "'," _
               & " '" & sDataAppModel & "'," _
               & " '{0}'," _
               & " '{1}'," _
               & " {2}," _
               & " {3}," _
               & " '" & regInfo.Field(Of String)("ELEMENT_ID") & "'," _
               & " '" & regInfo.Field(Of String)("ELEMENT_VERSION") & "'," _
               & " '" & regInfo.Field(Of String)("ELEMENT_NAME") & "'),")
        Next regInfo

        Dim sValuesList As String = Nothing
        If oStringBuilder.Length <> 0 Then
            oStringBuilder.Remove(oStringBuilder.Length - 1, 1)
            sValuesList = oStringBuilder.ToString()
        End If

        For Each appUnit As DataRow In appUnits
            Dim sAppRailSection As String = appUnit.Field(Of String)("RAIL_SECTION_CODE")
            Dim sAppStationOrder As String = appUnit.Field(Of String)("STATION_ORDER_CODE")
            Dim sAppCorner As String = appUnit.Field(Of Integer)("CORNER_CODE").ToString()
            Dim sAppUnit As String = appUnit.Field(Of Integer)("UNIT_NO").ToString()

            Dim sSQLToDelete As String = _
               "DELETE FROM S_" & EkConstants.DataPurposeProgram & "_VER_INFO_EXPECTED" _
               & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
               & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
               & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
               & " AND CORNER_CODE = " & sAppCorner _
               & " AND UNIT_NO = " & sAppUnit
            dbCtl.ExecuteSQLToWrite(sSQLToDelete)

            If sValuesList IsNot Nothing Then
                Dim sSQLToInsert As String = _
                   "INSERT INTO S_" & EkConstants.DataPurposeProgram & "_VER_INFO_EXPECTED" _
                   & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID," _
                    & " UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
                    & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                    & " ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
                   & " VALUES" & String.Format(sValuesList, sAppRailSection, sAppStationOrder, sAppCorner, sAppUnit)
                dbCtl.ExecuteSQLToWrite(sSQLToInsert)
            End If
        Next appUnit
    End Sub

    Protected Overridable Function DeleteAndInsertMasterVersionInfo(ByVal dbCtl As DatabaseTalker, ByVal sFilePath As String) As NakCauseCode
        'TODO: �t�@�C���̉�͂ŃG���[�����o�����ۂ̖߂�l������������B
        '�R�l�N�V�����ُ̈�łȂ��̂ɃR�l�N�V������؂邱�ƂɂȂ邵�A
        '�ԐM���ꂽ�����Ӗ����킩��Ȃ��Ǝv����B
        '�ق�Ƃ��́A�����������P�[�X�p��NAK���R��
        '�v���g�R���d�l�Ƃ��Ē�`���ׂ��ł���B

        Dim sFileName As String = Path.GetFileName(sFilePath)
        Dim sDataAppModel As String = EkVersionInfoFileName.GetDataApplicableModel(sFileName)
        Dim dataAppUnit As EkCode = EkVersionInfoFileName.GetDataApplicableUnit(sFileName)

        '�}�X�^�o�[�W�������t�@�C���������ǂݏo���B
        Dim aElements As EkMasterVersionInfoElement()
        Try
            Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                If Not sDataAppModel.Equals(EkConstants.ModelCodeMadosho) Then
                    'TODO: �o�[�W�������͂��͂�UpboundData�ł͂Ȃ����A
                    '���ʉ��̂��߂ɂ��A�ł���΂��̖��ʂȃw�b�_���Ȃ��������B
                    oInputStream.Seek(EkConstants.UpboundDataHeaderLen, SeekOrigin.Begin)
                End If
                aElements = EkMasterVersionInfoReader.GetElementsFromStream(oInputStream)
            End Using
        Catch ex As IOException
            Log.Error("Exception caught.", ex)
            Return EkNakCauseCode.TelegramError
        Catch ex As FormatException
            Log.Error("Exception caught.", ex)
            Return EkNakCauseCode.TelegramError
        End Try

        Dim oStringBuilder As New StringBuilder()

        '�ǂݏo�����o�[�W�������̊e���R�[�h����������B
        For i As Integer = 0 To aElements.Length - 1
            '�\���Ώۃ��R�[�h�̏ꍇ
            If Not aElements(i).Kind.Equals("") AndAlso _
               Not aElements(i).Version.Equals("000") Then
                oStringBuilder.Append( _
                   " (GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " '" & sDataAppModel & "'," _
                   & " '{0}'," _
                   & " '{1}'," _
                   & " {2}," _
                   & " {3}," _
                   & " '" & aElements(i).Kind & "'," _
                   & " '" & aElements(i).SubKind & "'," _
                   & " '" & aElements(i).Version & "'),")
            End If
        Next

        Dim sValuesList As String = Nothing
        If oStringBuilder.Length <> 0 Then
            oStringBuilder.Remove(oStringBuilder.Length - 1, 1)
            sValuesList = oStringBuilder.ToString()
        End If

        Dim sAppRailSection As String = dataAppUnit.RailSection.ToString("D3")
        Dim sAppStationOrder As String = dataAppUnit.StationOrder.ToString("D3")
        Dim sAppCorner As String = dataAppUnit.Corner.ToString()
        Dim sAppUnit As String = dataAppUnit.Unit.ToString()

        Dim sSQLToDelete As String = _
           "DELETE FROM D_" & EkConstants.DataPurposeMaster & "_VER_INFO" _
           & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
           & " AND CORNER_CODE = " & sAppCorner _
           & " AND UNIT_NO = " & sAppUnit
        dbCtl.ExecuteSQLToWrite(sSQLToDelete)

        If sValuesList IsNot Nothing Then
            Dim sSQLToInsert As String = _
               "INSERT INTO D_" & EkConstants.DataPurposeMaster & "_VER_INFO" _
               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID," _
                & " UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
                & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                & " DATA_KIND, DATA_SUB_KIND, DATA_VERSION)" _
               & " VALUES" & String.Format(sValuesList, sAppRailSection, sAppStationOrder, sAppCorner, sAppUnit)
            dbCtl.ExecuteSQLToWrite(sSQLToInsert)
        End If

        Return EkNakCauseCode.None
    End Function

    Protected Overridable Function DeleteAndInsertProgramVersionInfo(ByVal dbCtl As DatabaseTalker, ByVal sFilePath As String, ByVal aGroupTitles As String()) As NakCauseCode
        'TODO: �t�@�C���̉�͂ŃG���[�����o�����ۂ̖߂�l������������B
        '�R�l�N�V�����ُ̈�łȂ��̂ɃR�l�N�V������؂邱�ƂɂȂ邵�A
        '�ԐM���ꂽ�����Ӗ����킩��Ȃ��Ǝv����B
        '�ق�Ƃ��́A�����������P�[�X�p��NAK���R��
        '�v���g�R���d�l�Ƃ��Ē�`���ׂ��ł���B

        Dim sFileName As String = Path.GetFileName(sFilePath)
        Dim sDataAppModel As String = EkVersionInfoFileName.GetDataApplicableModel(sFileName)
        Dim dataAppUnit As EkCode = EkVersionInfoFileName.GetDataApplicableUnit(sFileName)

        If sDataAppModel.Equals(EkConstants.ModelCodeMadosho) Then
            'TODO: �v�m�F�BI/F�d�l���⌻�s�@�̌��n�f�[�^���݂����́A
            '�w�b�_���͖����悤�ł��邪�A����A���D�@�n�Ɠ��ꂳ��Ă���i���̈����
            'I/F�d�l���̕\���͌��s�Ɠ����܂܂ɂȂ��Ă���j�\�����l������B

            '�v���O�����o�[�W�������t�@�C���������ǂݏo���B
            Dim aElementsForCur As EkMadoProgramVersionInfoElement()
            Dim aElementsForNew As EkMadoProgramVersionInfoElement()
            Try
                Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                    oInputStream.Seek(6, SeekOrigin.Begin)
                    aElementsForCur = EkMadoProgramVersionInfoReader.GetElementsFromStream(oInputStream)
                    oInputStream.Seek(6, SeekOrigin.Current)
                    aElementsForNew = EkMadoProgramVersionInfoReader.GetElementsFromStream(oInputStream)
                End Using
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                Return EkNakCauseCode.TelegramError
            Catch ex As FormatException
                Log.Error("Exception caught.", ex)
                Return EkNakCauseCode.TelegramError
            End Try

            DeleteAndInsertMadoProgramVersionInfo(dbCtl, dataAppUnit, aElementsForCur, "CUR")
            DeleteAndInsertMadoProgramVersionInfo(dbCtl, dataAppUnit, aElementsForNew, "NEW")
        Else
            'NOTE: �Ď���CAB�̉�͕��@�����D�@CAB�Ɠ��l�ɂ����ꍇ�́A
            'groupCount�̎擾���@�����D�@���Ɠ��l�ɂ���ׂ��ł���B
            '�Ȃ��A�������邩�ۂ��Ɋ֌W�Ȃ��AgroupCount�͔h���N���X��
            '�ݒ肷������A�X�b�L�����邩������Ȃ��B
            Dim groupCount As Integer = aGroupTitles.Length
            Dim oReader As EkProgramVersionInfoReader
            If sDataAppModel.Equals(EkConstants.ModelCodeKanshiban) Then
                oReader = New EkProgramVersionInfoReaderForW()
            Else
                oReader = New EkProgramVersionInfoReaderForG()
            End If

            '�v���O�����o�[�W�������t�@�C������e�O���[�v�̌��o������ǂݏo���B
            Dim aGroupHeadersForCur(groupCount - 1) As EkProgramVersionInfoElementGroupHeader
            Dim aGroupsOfElementsForCur(groupCount - 1)() As EkProgramVersionInfoElement
            Dim aGroupHeadersForNew(groupCount - 1) As EkProgramVersionInfoElementGroupHeader
            Dim aGroupsOfElementsForNew(groupCount - 1)() As EkProgramVersionInfoElement
            Try
                Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                    'TODO: �o�[�W�������͂��͂�UpboundData�ł͂Ȃ����A
                    '�ł���΂��̖��ʂȃw�b�_���Ȃ��������B
                    oInputStream.Seek(EkConstants.UpboundDataHeaderLen + 1, SeekOrigin.Begin)
                    For i As Integer = 0 To groupCount - 1
                        aGroupHeadersForCur(i) = oReader.GetOneGroupHeaderFromStream(oInputStream)
                    Next
                    For i As Integer = 0 To groupCount - 1
                        aGroupsOfElementsForCur(i) = oReader.GetOneGroupElementsFromStream(oInputStream, aGroupHeadersForCur(i))
                    Next
                    For i As Integer = 0 To groupCount - 1
                        aGroupHeadersForNew(i) = oReader.GetOneGroupHeaderFromStream(oInputStream)
                    Next
                    For i As Integer = 0 To groupCount - 1
                        aGroupsOfElementsForNew(i) = oReader.GetOneGroupElementsFromStream(oInputStream, aGroupHeadersForNew(i))
                    Next
                End Using
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                Return EkNakCauseCode.TelegramError
            Catch ex As FormatException
                Log.Error("Exception caught.", ex)
                Return EkNakCauseCode.TelegramError
            End Try

            DeleteAndInsertGateryProgramVersionInfo(dbCtl, sDataAppModel, dataAppUnit, aGroupsOfElementsForCur, "CUR", aGroupTitles)
            DeleteAndInsertGateryProgramVersionInfo(dbCtl, sDataAppModel, dataAppUnit, aGroupsOfElementsForNew, "NEW", aGroupTitles)
        End If

        Return EkNakCauseCode.None
    End Function

    Protected Overridable Sub DeleteAndInsertMadoProgramVersionInfo( _
       ByVal dbCtl As DatabaseTalker, _
       ByVal dataAppUnit As EkCode, _
       ByVal aElements As EkMadoProgramVersionInfoElement(), _
       ByVal sTableGeneration As String)

        Dim oStringBuilder As New StringBuilder()
        For i As Integer = 0 To aElements.Length - 1
            If aElements(i).IsVersion Then
                oStringBuilder.Append( _
                   " (GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " '" & EkConstants.ModelCodeMadosho & "'," _
                   & " '{0}'," _
                   & " '{1}'," _
                   & " {2}," _
                   & " {3}," _
                   & " '" & i.ToString("D2") & "'," _
                   & " '" & aElements(i).Value & "'," _
                   & " '" & aElements(i).Name.Replace("�o�[�W����", "") & "'),")
            End If
        Next

        Dim sValuesList As String = Nothing
        If oStringBuilder.Length <> 0 Then
            oStringBuilder.Remove(oStringBuilder.Length - 1, 1)
            sValuesList = oStringBuilder.ToString()
        End If

        Dim sAppRailSection As String = dataAppUnit.RailSection.ToString("D3")
        Dim sAppStationOrder As String = dataAppUnit.StationOrder.ToString("D3")
        Dim sAppCorner As String = dataAppUnit.Corner.ToString()
        Dim sAppUnit As String = dataAppUnit.Unit.ToString()

        Dim sSQLToDelete As String = _
           "DELETE FROM D_" & EkConstants.DataPurposeProgram & "_VER_INFO_" & sTableGeneration _
           & " WHERE MODEL_CODE = '" & EkConstants.ModelCodeMadosho & "'" _
           & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
           & " AND CORNER_CODE = " & sAppCorner _
           & " AND UNIT_NO = " & sAppUnit
        dbCtl.ExecuteSQLToWrite(sSQLToDelete)

        If sValuesList IsNot Nothing Then
            Dim sSQLToInsert As String = _
               "INSERT INTO D_" & EkConstants.DataPurposeProgram & "_VER_INFO_" & sTableGeneration _
               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID," _
                & " UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
                & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                & " ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
               & " VALUES" & String.Format(sValuesList, sAppRailSection, sAppStationOrder, sAppCorner, sAppUnit)
            dbCtl.ExecuteSQLToWrite(sSQLToInsert)
        End If
    End Sub

    Protected Overridable Sub DeleteAndInsertGateryProgramVersionInfo( _
       ByVal dbCtl As DatabaseTalker, _
       ByVal sDataAppModel As String, _
       ByVal dataAppUnit As EkCode, _
       ByVal aGroupsOfElements As EkProgramVersionInfoElement()(), _
       ByVal sTableGeneration As String, _
       ByVal aGroupTitles As String())

        Dim oStringBuilder As New StringBuilder()
        For i As Integer = 0 To aGroupsOfElements.Length - 1
            For j As Integer = 0 To aGroupsOfElements(i).Length - 1
                Dim sElemName As String
                If aGroupTitles(i).Length <> 0 Then
                    sElemName = aGroupTitles(i) & "\" & Path.GetFileNameWithoutExtension(aGroupsOfElements(i)(j).FileName)
                Else
                    sElemName = aGroupsOfElements(i)(j).DispName
                End If

                oStringBuilder.Append( _
                   " (GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " '" & sDataAppModel & "'," _
                   & " '{0}'," _
                   & " '{1}'," _
                   & " {2}," _
                   & " {3}," _
                   & " '" & i.ToString("D2") & "\" & aGroupsOfElements(i)(j).FileName.ToUpperInvariant() & "'," _
                   & " '" & aGroupsOfElements(i)(j).Version & "'," _
                   & " '" & sElemName & "'),")
            Next
        Next

        Dim sValuesList As String = Nothing
        If oStringBuilder.Length <> 0 Then
            oStringBuilder.Remove(oStringBuilder.Length - 1, 1)
            sValuesList = oStringBuilder.ToString()
        End If

        Dim sAppRailSection As String = dataAppUnit.RailSection.ToString("D3")
        Dim sAppStationOrder As String = dataAppUnit.StationOrder.ToString("D3")
        Dim sAppCorner As String = dataAppUnit.Corner.ToString()
        Dim sAppUnit As String = dataAppUnit.Unit.ToString()

        Dim sSQLToDelete As String = _
           "DELETE FROM D_" & EkConstants.DataPurposeProgram & "_VER_INFO_" & sTableGeneration _
           & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
           & " AND CORNER_CODE = " & sAppCorner _
           & " AND UNIT_NO = " & sAppUnit
        dbCtl.ExecuteSQLToWrite(sSQLToDelete)

        If sValuesList IsNot Nothing Then
            Dim sSQLToInsert As String = _
               "INSERT INTO D_" & EkConstants.DataPurposeProgram & "_VER_INFO_" & sTableGeneration _
               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID," _
                & " UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
                & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                & " ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
               & " VALUES" & String.Format(sValuesList, sAppRailSection, sAppStationOrder, sAppCorner, sAppUnit)
            dbCtl.ExecuteSQLToWrite(sSQLToInsert)
        End If
    End Sub

    Protected Overridable Sub InsertScheduledUllFailureToCdt(ByVal sFileName As String)
        Dim recBaseInfo As New RecDataStructure.BaseInfo(sClientModel, clientCode)

        Dim aCdtKinds As String()
        Dim sDataKind As String = EkScheduledDataFileName.GetKind(sFileName)
        If DbConstants.CdtKindsOfDataKinds.ContainsKey(sDataKind) Then
            aCdtKinds = DbConstants.CdtKindsOfDataKinds(sDataKind)
        Else
            'NOTE: �s���Ȏ�ʂɂ��āu�f�[�^�̓o�^�Ɏ��s���܂����v�ُ̈��
            '�o�^����ꍇ�ƁA�t�H�[���o�b�N�̕��@���قȂ邪�A�P�Ȃ�
            '�t�H�[���o�b�N�ł���ASchedule�̐ݒ�Ɍ�肪�Ȃ�����A
            '���삷�邱�Ƃ��Ȃ����߁A�C�ɂ��Ȃ����Ƃɂ���B
            Log.Error("CollectedDataTypo code for [" & sDataKind & "] is not defined.")
            aCdtKinds = New String(0) {sDataKind}
        End If

        Dim sErrorInfo As String = Lexis.CdtScheduledUllFailed.Gen(sCdtClientModelName, clientCode.Unit.ToString())

        For i As Integer = 0 To aCdtKinds.Length - 1
            CollectedDataTypoRecorder.Record(recBaseInfo, aCdtKinds(i), sErrorInfo)
        Next
    End Sub

    Protected Overridable Sub InsertLineErrorToCdt()
        Dim now As DateTime = DateTime.Now
        'StartMinutesInDay�ȏ�ɂȂ�悤�ɕ␳����
        '���ݎ������i0��0������̌o�ߕ��̌`���Łj���߂�B
        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
        If TelServerAppBaseConfig.LineErrorRecordingStartMinutesInDay > nowMinutesInDay Then
            nowMinutesInDay += 24 * 60
        End If

        '�L�����ԑт̂ݓo�^���s���B
        If nowMinutesInDay <= TelServerAppBaseConfig.LineErrorRecordingEndMinutesInDay Then
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(sClientModel, clientCode), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtLineError.Gen(sCdtClientModelName, clientCode.Unit.ToString(), sCdtPortName))
        End If
    End Sub

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    Protected Overridable Sub EmitLineErrorMail()
        Dim now As DateTime = DateTime.Now
        'StartMinutesInDay�ȏ�ɂȂ�悤�ɕ␳����
        '���ݎ������i0��0������̌o�ߕ��̌`���Łj���߂�B
        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
        If TelServerAppBaseConfig.LineErrorAlertingStartMinutesInDay > nowMinutesInDay Then
            nowMinutesInDay += 24 * 60
        End If

        '�L�����ԑтŁA�x�񃁁[�����M�v���Z�X����ŁA�ʐM���肪�x�~���@�łȂ��ꍇ�̂ݐ������s���B
        Dim oTargetQueue As MessageQueue = Nothing
        If nowMinutesInDay <= TelServerAppBaseConfig.LineErrorAlertingEndMinutesInDay AndAlso _
           TelServerAppBaseConfig.MessageQueueForApps.TryGetValue("AlertMailer", oTargetQueue) = True Then
            Dim sSQL As String = _
               "SELECT COUNT(*)" _
               & " FROM M_RESTING_MACHINE" _
               & " WHERE MODEL_CODE = '" & sClientModel & "'" _
               & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
               & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
               & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
               & " AND UNIT_NO = " & clientCode.Unit.ToString()
            Dim resting As Integer
            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                resting = CInt(dbCtl.ExecuteSQLToReadScalar(sSQL))
            Catch ex As DatabaseException
                Throw
            Catch ex As Exception
                Throw New DatabaseException(ex)
            Finally
                dbCtl.ConnectClose()
            End Try

            If resting = 0 Then
                '�x�񃁁[���̕��ʂ𐶐�����B
                Dim sMailTitle As String = lineErrorAlertMailSubject.Gen(sClientStationName, sClientCornerName, clientCode.Unit)
                Dim sMailBody As String = lineErrorAlertMailBody.Gen(sClientStationName, sClientCornerName, clientCode.Unit, lineErrorBeginingTime)

                '�x�񃁁[�����M�v���Z�X�ɑ��M��v������B
                oTargetQueue.Send(New ExtAlertMailSendRequest(sMailTitle, sMailBody))

                Log.Debug("Line error alert emitted.")
            Else
                Log.Debug("Line error alert suppressed because the client is resting.")
            End If
        End If
    End Sub
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------
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

'NOTE: ������ԕ�Ή��ɂāA�e��V�[�P���X�̎d�l�L�q�p�̃N���X�́A
'Public�ɕύX���āATelServerAppTelegrapher�̒������`�ӏ����ړ������B
'����ɂ��A�����̃N���X�����L�q���Ă���S�Ẳӏ����@�B�I�ɕύX���Ă���B
'���e�̕ύX�́ATelServerAppRiyoDataUllSpec�ւ̃����o�ǉ��݂̂ł���B
'-------Ver0.1 ������ԕ�Ή� ADD START-----------
Public Class TelServerAppMasProDllSpec
    'NOTE: MaxRetryCountToForget�̒l���L���Ɏg���邱�Ƃ͂Ȃ��͂�
    '�ł��邪�ATelServerAppScheduledUllSpec�Ƃ̈�ѐ��ێ��̂��߁A�p�ӂ��Ă���B
    Public ObjCode As Byte
    Public SubObjCode As Byte
    Public TransferLimitTicks As Integer
    Public StartReplyLimitTicks As Integer
    Public RetryIntervalTicks As Integer
    Public MaxRetryCountToForget As Integer
    Public MaxRetryCountToCare As Integer

    Public Sub New( _
       ByVal objCode As Byte, _
       ByVal subObjCode As Byte, _
       ByVal transferLimitTicks As Integer, _
       ByVal startReplyLimitTicks As Integer, _
       ByVal retryIntervalTicks As Integer, _
       ByVal maxRetryCountToForget As Integer, _
       ByVal maxRetryCountToCare As Integer)

        Me.ObjCode = objCode
        Me.SubObjCode = subObjCode
        Me.TransferLimitTicks = transferLimitTicks
        Me.StartReplyLimitTicks = startReplyLimitTicks
        Me.RetryIntervalTicks = retryIntervalTicks
        Me.MaxRetryCountToForget = maxRetryCountToForget
        Me.MaxRetryCountToCare = maxRetryCountToCare
    End Sub
End Class

Public Class TelServerAppScheduledUllSpec
    Public ObjCode As Byte
    Public TransferLimitTicks As Integer
    Public StartReplyLimitTicks As Integer
    Public RetryIntervalTicks As Integer
    Public MaxRetryCountToForget As Integer
    Public MaxRetryCountToCare As Integer
    Public RecAppIdentifier As String

    Public Sub New( _
       ByVal objCode As Byte, _
       ByVal transferLimitTicks As Integer, _
       ByVal startReplyLimitTicks As Integer, _
       ByVal retryIntervalTicks As Integer, _
       ByVal maxRetryCountToForget As Integer, _
       ByVal maxRetryCountToCare As Integer, _
       ByVal recAppIdentifier As String)

        Me.ObjCode = objCode
        Me.TransferLimitTicks = transferLimitTicks
        Me.StartReplyLimitTicks = startReplyLimitTicks
        Me.RetryIntervalTicks = retryIntervalTicks
        Me.MaxRetryCountToForget = maxRetryCountToForget
        Me.MaxRetryCountToCare = maxRetryCountToCare
        Me.RecAppIdentifier = recAppIdentifier
    End Sub
End Class

Public Class TelServerAppMasProDlReflectSpec
    Public ApplicableModel As String
    Public FilePurpose As String
    Public DataPurpose As String
    Public DataKind As String

    Public Sub New( _
       ByVal sApplicableModel As String, _
       ByVal sFilePurpose As String, _
       ByVal sDataPurpose As String, _
       ByVal sDataKind As String)

        Me.ApplicableModel = sApplicableModel
        Me.FilePurpose = sFilePurpose
        Me.DataPurpose = sDataPurpose
        Me.DataKind = sDataKind
    End Sub
End Class

Public Class TelServerAppByteArrayPassivePostSpec
    Public RecAppIdentifier As String

    Public Sub New(ByVal recAppIdentifier As String)
        Me.RecAppIdentifier = recAppIdentifier
    End Sub
End Class

Public Class TelServerAppVersionInfoUllSpec
    Public ApplicableModel As String
    Public DataPurpose As String
    Public GroupTitles As String()
    Public TransferLimitTicks As Integer

    Public Sub New( _
       ByVal sApplicableModel As String, _
       ByVal sDataPurpose As String, _
       ByVal aGroupTitles As String(), _
       ByVal transferLimitTicks As Integer)

        Me.ApplicableModel = sApplicableModel
        Me.DataPurpose = sDataPurpose
        Me.GroupTitles = aGroupTitles
        Me.TransferLimitTicks = transferLimitTicks
    End Sub
End Class

Public Class TelServerAppRiyoDataUllSpec
    Public FileName As String
    Public FormatCode As String
    Public RecordLen As Integer
    Public TransferLimitTicks As Integer

    Public Sub New( _
       ByVal sFileName As String, _
       ByVal sFormatCode As String, _
       ByVal recordLen As Integer, _
       ByVal transferLimitTicks As Integer)

        Me.FileName = sFileName
        Me.FormatCode = sFormatCode
        Me.RecordLen = recordLen
        Me.TransferLimitTicks = transferLimitTicks
    End Sub
End Class
'-------Ver0.1 ������ԕ�Ή� ADD END-------------
