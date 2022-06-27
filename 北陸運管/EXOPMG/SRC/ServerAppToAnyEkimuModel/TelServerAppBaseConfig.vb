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

Imports System.Net

Imports JR.ExOpmg.Common

Public Class TelServerAppBaseConfig
    Inherits ServerAppBaseConfig

    '�ǂݏo���Ώۂ̃��b�Z�[�W�L���[
    Public Shared MyMqPath As String

    'FTP�T�[�o���[�g�f�B���N�g����
    Public Shared FtpServerRootDirPath As String

    'FTP�T�[�o���ɂ�����A�N�Z�X���f�B���N�g����
    Public Shared PermittedPathInFtp As String

    '�X���b�h�ʃe���|�����f�B���N�g���̃x�[�X�i�v���Z�X�̃e���|�����f�B���N�g���j
    Public Shared TemporaryBaseDirPath As String

    '�d���ʐM�p���b�X���A�h���X
    Public Shared IpAddrForTelegConnection As IPAddress

    '�d���ʐM�p���b�X���|�[�g�ԍ�
    Public Shared IpPortForTelegConnection As Integer

    '�d������M�X���b�h��~���e����
    Public Shared TelegrapherPendingLimitTicks As Integer

    '�E�H�b�`�h�b�O�V�[�P���X�̊Ԋu
    Public Shared WatchdogIntervalTicks As Integer

    '�P�d����M�J�n�`�����̊����i��{���ԁA0��-1�͎w��֎~�j
    Public Shared TelegReadingLimitBaseTicks As Integer

    '�P�d����M�J�n�`�����̊����i���r�o�C�g������̒ǉ����ԁj
    Public Shared TelegReadingLimitExtraTicksPerMiB As Integer

    '�P�d�������J�n�`�����̊����i��{���ԁA0��-1�͎w��֎~�j
    Public Shared TelegWritingLimitBaseTicks As Integer

    '�P�d�������J�n�`�����̊����i���r�o�C�g������̒ǉ����ԁj
    Public Shared TelegWritingLimitExtraTicksPerMiB As Integer

    '�P�d����M������̃��O�ۑ��ő僌���O�X
    Public Shared TelegLoggingMaxLengthOnRead As Integer

    '�P�d������������̃��O�ۑ��ő僌���O�X
    Public Shared TelegLoggingMaxLengthOnWrite As Integer

    '�R�l�N�V�����ؒf����ʐM��ԕύX�܂ł̒x������
    Public Shared PseudoConnectionProlongationTicks As Integer

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�o�^��ON�ɂ��鎞���i00��00������̌o�ߕ��j
    Public Shared LineErrorRecordingStartMinutesInDay As Integer

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�o�^��OFF�ɂ��鎞���i00��00������̌o�ߕ��j�iStartMinutesInDay�ȏ�ɕ␳�ς݁j
    Public Shared LineErrorRecordingEndMinutesInDay As Integer

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�̏d���o�^�֎~���ԁi�����0�ȉ��ɂ���΁A�o�^�͍s��Ȃ��j
    Public Shared LineErrorRecordingIntervalTicks As Integer

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    '�ʐM�ُ�̌x�񃁁[��������ON�ɂ��鎞���i00��00������̌o�ߕ��j
    Public Shared LineErrorAlertingStartMinutesInDay As Integer

    '�ʐM�ُ�̌x�񃁁[��������OFF�ɂ��鎞���i00��00������̌o�ߕ��j�iStartMinutesInDay�ȏ�ɕ␳�ς݁j
    Public Shared LineErrorAlertingEndMinutesInDay As Integer

    '�ʐM�ُ�̌x�񃁁[���̏d�������֎~���ԁi�����0�ȉ��ɂ���΁A�����͍s��Ȃ��j
    Public Shared LineErrorAlertingIntervalTicks As Integer
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------

    '�ʐM�ُ�Ƃ݂Ȃ��Ȃ��^�Ǒ��|�[�g�I�[�v������ڑ��������V�[�P���X�����܂ł̊���
    Public Shared InitialConnectLimitTicksForLineError As Integer

    '�E�H�b�`�h�b�O�V�[�P���X�ɂ����鉞���d����M����
    Public Shared WatchdogReplyLimitTicks As Integer

    '�t�@�C���]���V�[�P���X�r���������[�h�ݒ�
    Public Shared EnableXllStrongExclusion As Boolean

    '�\���I�V�[�P���X�r���������[�h�ݒ�
    Public Shared EnableActiveSeqStrongExclusion As Boolean

    '�\���I�P���V�[�P���X�����������[�h�ݒ�
    Public Shared EnableActiveOneOrdering As Boolean

    '�}�X�^/�v���O����DLL�������s�ő�N���C�A���g��
    Public Shared ConcurrentMasProDllMaxCount As Integer

    '�w��t�@�C��ULL�������s�ő�N���C�A���g��
    Public Shared ConcurrentScheduledUllMaxCount As Integer

    '�ʐM��ԂɊւ���SNMP�ʒm�p�A�v���ԍ��i0�̏ꍇ�͒ʒm���Ȃ��j
    Public Shared SnmpAppNumberForConnectionStatus As Integer

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const FTP_SERVER_ROOT_DIR_PATH_KEY As String = "FtpServerRootDirPath"
    Private Const PERMITTED_PATH_IN_FTP_KEY As String = "PermittedPathInFtp"
    Private Const TEMP_BASE_DIR_PATH_KEY As String = "TemporaryBaseDirPath"
    Private Const SELF_ADDR_KEY As String = "SelfAddr"
    Private Const TELEG_CON_PORT_KEY As String = "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = "TelegrapherPendingLimitTicks"
    Private Const WATCHDOG_INTERVAL_KEY As String = "WatchdogIntervalTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = "TelegLoggingMaxLengthOnWrite"
    Private Const PSEUDO_CONNECTION_PROLONGATION_KEY As String = "PseudoConnectionProlongationTicks"
    Private Const LINE_ERROR_RECORDING_START_TIME_KEY As String = "LineErrorRecordingStartTime"
    Private Const LINE_ERROR_RECORDING_END_TIME_KEY As String = "LineErrorRecordingEndTime"
    Private Const LINE_ERROR_RECORDING_INTERVAL_KEY As String = "LineErrorRecordingIntervalTicks"
    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    Private Const LINE_ERROR_ALERTING_START_TIME_KEY As String = "LineErrorAlertingStartTime"
    Private Const LINE_ERROR_ALERTING_END_TIME_KEY As String = "LineErrorAlertingEndTime"
    Private Const LINE_ERROR_ALERTING_INTERVAL_KEY As String = "LineErrorAlertingIntervalTicks"
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------
    Private Const INITIAL_CONNECT_LIMIT_FOR_LINE_ERROR_KEY As String = "InitialConnectLimitTicksForLineError"
    Private Const WATCHDOG_REPLY_LIMIT_KEY As String = "WatchdogReplyLimitTicks"
    Private Const ENABLE_XLL_STRONG_EXCLUSION_KEY As String = "EnableXllStrongExclusion"
    Private Const ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY As String = "EnableActiveSeqStrongExclusion"
    Private Const ENABLE_ACTIVE_ONE_ORDERING_KEY As String = "EnableActiveOneOrdering"
    Private Const CONCURRENT_MASPRO_DLL_MAX_COUNT_KEY As String = "ConcurrentMasProDllMaxCount"
    Private Const CONCURRENT_SCHEDULED_ULL_MAX_COUNT_KEY As String = "ConcurrentScheduledUllMaxCount"
    Private Const SNMP_APP_NUMBER_FOR_CONNECTION_STATUS_KEY As String = "ConnectionStatus"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̒ʐM�n�v���Z�X�ɕK�{�̐ݒ�l����荞�ށB</summary>
    Public Shared Sub TelServerAppBaseInit(ByVal sIniFilePath As String, ByVal sModelName As String, Optional ByVal needInfoOfOtherApps As Boolean = False)
        Dim sAppIdentifier As String = "To" & sModelName
        ServerAppBaseInit(sIniFilePath, sAppIdentifier, needInfoOfOtherApps)

        Dim aStrings As String()
        Try
            ReadFileElem(MQ_SECTION, sAppIdentifier & MQ_PATH_KEY)
            MyMqPath = LastReadValue

            ReadFileElem(PATH_SECTION, sAppIdentifier & FTP_SERVER_ROOT_DIR_PATH_KEY)
            FtpServerRootDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, sAppIdentifier & PERMITTED_PATH_IN_FTP_KEY)
            PermittedPathInFtp = LastReadValue

            ReadFileElem(PATH_SECTION, sAppIdentifier & TEMP_BASE_DIR_PATH_KEY)
            TemporaryBaseDirPath = LastReadValue

            ReadFileElem(NETWORK_SECTION, SELF_ADDR_KEY)
            IpAddrForTelegConnection = IPAddress.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, sAppIdentifier & TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & WATCHDOG_INTERVAL_KEY)
            WatchdogIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEG_READING_LIMIT_BASE_KEY)
            TelegReadingLimitBaseTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEG_READING_LIMIT_EXTRA_KEY)
            TelegReadingLimitExtraTicksPerMiB = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEG_WRITING_LIMIT_BASE_KEY)
            TelegWritingLimitBaseTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEG_WRITING_LIMIT_EXTRA_KEY)
            TelegWritingLimitExtraTicksPerMiB = Integer.Parse(LastReadValue)

            ReadFileElem(LOGGING_SECTION, sAppIdentifier & TELEG_LOGGING_MAX_ON_READ_KEY)
            TelegLoggingMaxLengthOnRead = Integer.Parse(LastReadValue)

            ReadFileElem(LOGGING_SECTION, sAppIdentifier & TELEG_LOGGING_MAX_ON_WRITE_KEY)
            TelegLoggingMaxLengthOnWrite = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & PSEUDO_CONNECTION_PROLONGATION_KEY)
            PseudoConnectionProlongationTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_RECORDING_START_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorRecordingStartHour As Integer = Integer.Parse(aStrings(0))
            If lineErrorRecordingStartHour < 0 OrElse lineErrorRecordingStartHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorRecordingStartMinute As Integer = Integer.Parse(aStrings(1))
            If lineErrorRecordingStartMinute < 0 OrElse lineErrorRecordingStartMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            LineErrorRecordingStartMinutesInDay = lineErrorRecordingStartHour * 60 + lineErrorRecordingStartMinute

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_RECORDING_END_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorRecordingEndHour As Integer = Integer.Parse(aStrings(0))
            If lineErrorRecordingEndHour < 0 OrElse lineErrorRecordingEndHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorRecordingEndMinute As Integer = Integer.Parse(aStrings(1))
            If lineErrorRecordingEndMinute < 0 OrElse lineErrorRecordingEndMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            LineErrorRecordingEndMinutesInDay = lineErrorRecordingEndHour * 60 + lineErrorRecordingEndMinute

            'StartMinutesInDay <= EndMinutesInDay�ɂȂ�悤�A
            '�K�v�ɉ�����EndMinutesInDay�ɂ͕␳�������Ă����B
            'NOTE: StartMinutesInDay == EndMinutesInDay�͐����Ȑݒ�
            '�ł���A�L�����ԑт����̂P���Ԃ����ł��邱�Ƃ��Ӗ�����B
            If LineErrorRecordingStartMinutesInDay > LineErrorRecordingEndMinutesInDay Then
                LineErrorRecordingEndMinutesInDay += 24 * 60
            End If

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_RECORDING_INTERVAL_KEY)
            LineErrorRecordingIntervalTicks = Integer.Parse(LastReadValue)

            '-------Ver0.1 ������ԕ�Ή� ADD START-----------
            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_ALERTING_START_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorAlertingStartHour As Integer = Integer.Parse(aStrings(0))
            If lineErrorAlertingStartHour < 0 OrElse lineErrorAlertingStartHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorAlertingStartMinute As Integer = Integer.Parse(aStrings(1))
            If lineErrorAlertingStartMinute < 0 OrElse lineErrorAlertingStartMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            LineErrorAlertingStartMinutesInDay = lineErrorAlertingStartHour * 60 + lineErrorAlertingStartMinute

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_ALERTING_END_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorAlertingEndHour As Integer = Integer.Parse(aStrings(0))
            If lineErrorAlertingEndHour < 0 OrElse lineErrorAlertingEndHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorAlertingEndMinute As Integer = Integer.Parse(aStrings(1))
            If lineErrorAlertingEndMinute < 0 OrElse lineErrorAlertingEndMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            LineErrorAlertingEndMinutesInDay = lineErrorAlertingEndHour * 60 + lineErrorAlertingEndMinute

            'StartMinutesInDay <= EndMinutesInDay�ɂȂ�悤�A
            '�K�v�ɉ�����EndMinutesInDay�ɂ͕␳�������Ă����B
            'NOTE: StartMinutesInDay == EndMinutesInDay�͐����Ȑݒ�
            '�ł���A�L�����ԑт����̂P���Ԃ����ł��邱�Ƃ��Ӗ�����B
            If LineErrorAlertingStartMinutesInDay > LineErrorAlertingEndMinutesInDay Then
                LineErrorAlertingEndMinutesInDay += 24 * 60
            End If

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_ALERTING_INTERVAL_KEY)
            LineErrorAlertingIntervalTicks = Integer.Parse(LastReadValue)
            '-------Ver0.1 ������ԕ�Ή� ADD END-------------

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & INITIAL_CONNECT_LIMIT_FOR_LINE_ERROR_KEY)
            InitialConnectLimitTicksForLineError = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & WATCHDOG_REPLY_LIMIT_KEY)
            WatchdogReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & ENABLE_XLL_STRONG_EXCLUSION_KEY)
            EnableXllStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY)
            EnableActiveSeqStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & ENABLE_ACTIVE_ONE_ORDERING_KEY)
            EnableActiveOneOrdering = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & CONCURRENT_MASPRO_DLL_MAX_COUNT_KEY)
            ConcurrentMasProDllMaxCount = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & CONCURRENT_SCHEDULED_ULL_MAX_COUNT_KEY)
            ConcurrentScheduledUllMaxCount = Integer.Parse(LastReadValue)

            ReadFileElem(SNMP_APP_NUMBER_SECTION, sAppIdentifier & SNMP_APP_NUMBER_FOR_CONNECTION_STATUS_KEY)
            SnmpAppNumberForConnectionStatus = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    Public Shared Sub TelServerAppBaseDispose()
        ServerAppBaseDispose()
    End Sub

End Class
