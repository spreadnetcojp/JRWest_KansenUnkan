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

Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '�ǂݏo���Ώۂ̃��b�Z�[�W�L���[
    Public Shared MyMqPath As String

    '�d���ʐM�p���b�X���A�h���X
    Public Shared IpAddrForTelegConnection As IPAddress

    '�d������M�X���b�h��~���e����
    Public Shared TelegrapherPendingLimitTicks As Integer

    '�P�d����M�J�n�`�����̊����i��{���ԁA0��-1�͎w��֎~�j
    Public Shared TelegReadingLimitBaseTicks As Integer

    '�P�d����M�J�n�`�����̊����i���r�o�C�g������̒ǉ����ԁj
    Public Shared TelegReadingLimitExtraTicksPerMiB As Integer

    '�P�d�������J�n�`�����̊����i0��-1�͖������j
    Public Shared TelegWritingLimitBaseTicks As Integer

    '�P�d�������J�n�`�����̊����i���r�o�C�g������̒ǉ����ԁj
    Public Shared TelegWritingLimitExtraTicksPerMiB As Integer

    '�P�d����M������̃��O�ۑ��ő僌���O�X
    Public Shared TelegLoggingMaxLengthOnRead As Integer

    '�P�d������������̃��O�ۑ��ő僌���O�X
    Public Shared TelegLoggingMaxLengthOnWrite As Integer

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�o�^��ON�ɂ��鎞���i00��00������̌o�ߕ��j
    Public Shared LineErrorRecordingStartMinutesInDay As Integer

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�o�^��OFF�ɂ��鎞���i00��00������̌o�ߕ��j�iStartMinutesInDay�ȏ�ɕ␳�ς݁j
    Public Shared LineErrorRecordingEndMinutesInDay As Integer

    '���W�f�[�^��L�e�[�u���ɑ΂���ʐM�ُ�̏d���o�^�֎~����
    Public Shared LineErrorRecordingIntervalTicks As Integer

    '���W�f�[�^��L�e�[�u���ɒʐM�ُ��o�^���邱�ƂɂȂ�|�[�g�I�[�v������J�ǃV�[�P���X�����܂ł̊���
    Public Shared InitialConnectLimitTicksForLineError As Integer

    '���p�f�[�^�����d����M����
    Public Shared RiyoDataReplyLimitTicks As Integer

    '���،����f�[�^�����d����M����
    Public Shared SummaryDataReplyLimitTicks As Integer

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const APP_ID As String = "ToNkan"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const SELF_ADDR_KEY As String = "SelfAddr"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = APP_ID & "TelegrapherPendingLimitTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = APP_ID & "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = APP_ID & "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = APP_ID & "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = APP_ID & "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = APP_ID & "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = APP_ID & "TelegLoggingMaxLengthOnWrite"
    Private Const LINE_ERROR_RECORDING_START_TIME_KEY As String = APP_ID & "LineErrorRecordingStartTime"
    Private Const LINE_ERROR_RECORDING_END_TIME_KEY As String = APP_ID & "LineErrorRecordingEndTime"
    Private Const LINE_ERROR_RECORDING_INTERVAL_KEY As String = APP_ID & "LineErrorRecordingIntervalTicks"
    Private Const INITIAL_CONNECT_LIMIT_FOR_LINE_ERROR_KEY As String = APP_ID & "InitialConnectLimitTicksForLineError"
    Private Const RIYO_DATA_REPLY_LIMIT_KEY As String = APP_ID & "RiyoDataReplyLimitTicks"
    Private Const SUMMARY_DATA_REPLY_LIMIT_KEY As String = APP_ID & "SummaryDataReplyLimitTicks"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̑΂m�ԒʐM�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Dim aStrings As String()
        Try
            ReadFileElem(MQ_SECTION, APP_ID & MQ_PATH_KEY)
            MyMqPath = LastReadValue

            ReadFileElem(NETWORK_SECTION, SELF_ADDR_KEY)
            IpAddrForTelegConnection = IPAddress.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEG_READING_LIMIT_BASE_KEY)
            TelegReadingLimitBaseTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEG_READING_LIMIT_EXTRA_KEY)
            TelegReadingLimitExtraTicksPerMiB = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEG_WRITING_LIMIT_BASE_KEY)
            TelegWritingLimitBaseTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEG_WRITING_LIMIT_EXTRA_KEY)
            TelegWritingLimitExtraTicksPerMiB = Integer.Parse(LastReadValue)

            ReadFileElem(LOGGING_SECTION, TELEG_LOGGING_MAX_ON_READ_KEY)
            TelegLoggingMaxLengthOnRead = Integer.Parse(LastReadValue)

            ReadFileElem(LOGGING_SECTION, TELEG_LOGGING_MAX_ON_WRITE_KEY)
            TelegLoggingMaxLengthOnWrite = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, LINE_ERROR_RECORDING_START_TIME_KEY)
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

            ReadFileElem(TIME_INFO_SECTION, LINE_ERROR_RECORDING_END_TIME_KEY)
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

            ReadFileElem(TIME_INFO_SECTION, LINE_ERROR_RECORDING_INTERVAL_KEY)
            LineErrorRecordingIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, INITIAL_CONNECT_LIMIT_FOR_LINE_ERROR_KEY)
            InitialConnectLimitTicksForLineError = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, RIYO_DATA_REPLY_LIMIT_KEY)
            RiyoDataReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, SUMMARY_DATA_REPLY_LIMIT_KEY)
            SummaryDataReplyLimitTicks = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    Public Shared Sub Dispose()
        ServerAppBaseDispose()
    End Sub

End Class
