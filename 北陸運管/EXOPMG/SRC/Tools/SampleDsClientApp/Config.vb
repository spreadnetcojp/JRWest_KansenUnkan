' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/05/13  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits BaseConfig

    '�L�����O���
    Public Shared LogKindsMask As Integer

    '�w�i�F
    Public Shared BackgroundColor As System.Drawing.Color

    '�{�^���F
    Public Shared ButtonColor As System.Drawing.Color

    'FTP���[�U��
    Public Shared FtpUserName As String

    'FTP�p�X���[�h
    Public Shared FtpPassword As String

    'FTP���[�L���O�f�B���N�g����
    Public Shared FtpWorkingDirPath As String

    'FTP�T�[�o���ɂ�����A�N�Z�X���f�B���N�g����
    Public Shared PermittedPathInFtp As String

    'FTP�T�[�oURI
    Public Shared FtpServerUri As String

    '�^�ǃT�[�oIP�A�h���X
    Public Shared ServerIpAddr As String

    '�d���ʐM�p�|�[�g�ԍ�
    Public Shared IpPortForTelegConnection As Integer

    '�d������M�X���b�h��~���e����
    Public Shared TelegrapherPendingLimitTicks As Integer

    '�d������M�X���b�hUll���s���e���ԁi0��-1�͖������j
    Public Shared TelegrapherUllLimitTicks As Integer

    '�d������M�X���b�h�z�M�w�����s���e���ԁi0��-1�͖������j
    Public Shared TelegrapherDllInvokeLimitTicks As Integer

    '�E�H�b�`�h�b�O�V�[�P���X�̍ő勖�e�Ԋu
    Public Shared WatchdogIntervalLimitTicks As Integer

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

    '�ڑ��������v���d���̉�����M����
    Public Shared ComStartReplyLimitTicks As Integer

    '�z�M�w���d���̉�����M����
    Public Shared MasProDllInvokeReplyLimitTicks As Integer

    '�}�X�^/�v���O����ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MasProUllTransferLimitTicks As Integer

    '�}�X�^/�v���O����ULL�ɂ�����J�n�d���̉�����M����
    Public Shared MasProUllStartReplyLimitTicks As Integer

    '�}�X�^/�v���O����ULL�ɂ�����I���d���̉�����M����
    Public Shared MasProUllFinishReplyLimitTicks As Integer

    '�E�H�b�`�h�b�O�ɂ������ԊĎ��̗v��
    Public Shared EnableWatchdog As Boolean

    '�t�@�C���]���V�[�P���X�r���������[�h�ݒ�
    Public Shared EnableXllStrongExclusion As Boolean

    '�\���I�V�[�P���X�r���������[�h�ݒ�
    Public Shared EnableActiveSeqStrongExclusion As Boolean

    '�\���I�P���V�[�P���X�����������[�h�ݒ�
    Public Shared EnableActiveOneOrdering As Boolean

    'FTP�X���b�h�̒�~���e����
    Public Shared FtpWorkerPendingLimitTicks As Integer

    'FTP�̊e�탊�N�G�X�g�ɑ΂��鉞����M����
    Public Shared FtpRequestLimitTicks As Integer

    'FTP�̃��O�A�E�g�̃��N�G�X�g�ɑ΂��鉞����M����
    Public Shared FtpLogoutLimitTicks As Integer

    'FTP�ňُ�Ɣ��肷��f�[�^�]����~���ԁi-1�͖������j
    Public Shared FtpTransferStallLimitTicks As Integer

    'FTP�Ńp�b�V�u���[�h���g�����ۂ�
    Public Shared FtpUsePassiveMode As Boolean

    'FTP�œ]�����s�����ƂɃ��O�A�E�g���邩�ۂ�
    Public Shared FtpLogoutEachTime As Boolean

    'FTP�Ŏg�p����o�b�t�@�̗e��
    Public Shared FtpBufferLength As Integer

    'INI�t�@�C�����̃Z�N�V������
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const COLOR_SECTION As String = "Color"
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const PATH_SECTION As String = "Path"
    Protected Const NETWORK_SECTION As String = "Network"
    Protected Const TIME_INFO_SECTION As String = "TimeInfo"
    Protected Const TELEGRAPHER_MODE_SECTION As String = "TelegrapherMode"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const BACKGROUND_COLOR_KEY As String = "ScreenRGB"
    Private Const BUTTON_COLOR_KEY As String = "ButtonRGB"
    Private Const FTP_USER_NAME_KEY As String = "FtpUserName"
    Private Const FTP_PASSWORD_KEY As String = "FtpPassword"
    Private Const FTP_WORKING_DIR_PATH_KEY As String = "FtpWorkingDirPath"
    Private Const PERMITTED_PATH_IN_FTP_KEY As String = "PermittedPathInFtp"
    Private Const FTP_SERVER_URI_KEY As String = "FtpServerUri"
    Private Const SERVER_IP_ADDR_KEY As String = "ServerIpAddr"
    Private Const TELEG_CON_PORT_KEY As String = "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = "TelegrapherPendingLimitTicks"
    Private Const TELEGRAPHER_ULL_LIMIT_KEY As String = "TelegrapherUllLimitTicks"
    Private Const TELEGRAPHER_DLL_INVOKE_LIMIT_KEY As String = "TelegrapherDllInvokeLimitTicks"
    Private Const WATCHDOG_INTERVAL_LIMIT_KEY As String = "WatchdogIntervalLimitTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = "TelegLoggingMaxLengthOnWrite"
    Private Const COM_START_REPLY_LIMIT_KEY As String = "ComStartReplyLimitTicks"
    Private Const MASPRO_DLL_INVOKE_REPLY_LIMIT_KEY As String = "MasProDllInvokeReplyLimitTicks"
    Private Const MASPRO_ULL_TRANSFER_LIMIT_KEY As String = "MasProUllTransferLimitTicks"
    Private Const MASPRO_ULL_START_REPLY_LIMIT_KEY As String = "MasProUllStartReplyLimitTicks"
    Private Const MASPRO_ULL_FINISH_REPLY_LIMIT_KEY As String = "MasProUllFinishReplyLimitTicks"
    Private Const ENABLE_WATCHDOG_KEY As String = "EnableWatchdog"
    Private Const ENABLE_XLL_STRONG_EXCLUSION_KEY As String = "EnableXllStrongExclusion"
    Private Const ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY As String = "EnableActiveSeqStrongExclusion"
    Private Const ENABLE_ACTIVE_ONE_ORDERING_KEY As String = "EnableActiveOneOrdering"
    Private Const FTP_WORKER_PENDING_LIMIT_TICKS_KEY As String = "FtpWorkerPendingLimitTicks"
    Private Const FTP_REQUEST_LIMIT_TICKS_KEY As String = "FtpRequestLimitTicks"
    Private Const FTP_LOGOUT_LIMIT_TICKS_KEY As String = "FtpLogoutLimitTicks"
    Private Const FTP_TRANSFER_STALL_LIMIT_TICKS_KEY As String = "FtpTransferStallLimitTicks"
    Private Const FTP_USE_PASSIVE_MODE_KEY As String = "FtpUsePassiveMode"
    Private Const FTP_LOGOUT_EACH_TIME_KEY As String = "FtpLogoutEachTime"
    Private Const FTP_BUFFER_LENGTH_KEY As String = "FtpBufferLength"

    ''' <summary>INI�t�@�C������f�W�N���C�A���g�����A�v���ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Dim arrTemp As String()
        Try
            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(COLOR_SECTION, BACKGROUND_COLOR_KEY)
            arrTemp = LastReadValue.Split(","c)
            BackgroundColor = System.Drawing.Color.FromArgb(Integer.Parse(arrTemp(0)), Integer.Parse(arrTemp(1)), Integer.Parse(arrTemp(2)))

            ReadFileElem(COLOR_SECTION, BUTTON_COLOR_KEY)
            arrTemp = LastReadValue.Split(","c)
            ButtonColor = System.Drawing.Color.FromArgb(Integer.Parse(arrTemp(0)), Integer.Parse(arrTemp(1)), Integer.Parse(arrTemp(2)))

            ReadFileElem(CREDENTIAL_SECTION, FTP_USER_NAME_KEY)
            FtpUserName = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, FTP_PASSWORD_KEY)
            FtpPassword = LastReadValue

            ReadFileElem(PATH_SECTION, FTP_WORKING_DIR_PATH_KEY)
            FtpWorkingDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, PERMITTED_PATH_IN_FTP_KEY)
            PermittedPathInFtp = LastReadValue

            ReadFileElem(NETWORK_SECTION, FTP_SERVER_URI_KEY)
            FtpServerUri = LastReadValue

            ReadFileElem(NETWORK_SECTION, SERVER_IP_ADDR_KEY)
            ServerIpAddr = LastReadValue

            ReadFileElem(NETWORK_SECTION, TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_ULL_LIMIT_KEY)
            TelegrapherUllLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_DLL_INVOKE_LIMIT_KEY)
            TelegrapherDllInvokeLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, WATCHDOG_INTERVAL_LIMIT_KEY)
            WatchdogIntervalLimitTicks = Integer.Parse(LastReadValue)

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

            ReadFileElem(TIME_INFO_SECTION, COM_START_REPLY_LIMIT_KEY)
            ComStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MASPRO_DLL_INVOKE_REPLY_LIMIT_KEY)
            MasProDllInvokeReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MASPRO_ULL_TRANSFER_LIMIT_KEY)
            MasProUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MASPRO_ULL_START_REPLY_LIMIT_KEY)
            MasProUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MASPRO_ULL_FINISH_REPLY_LIMIT_KEY)
            MasProUllFinishReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_WATCHDOG_KEY)
            EnableWatchdog = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_XLL_STRONG_EXCLUSION_KEY)
            EnableXllStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY)
            EnableActiveSeqStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_ONE_ORDERING_KEY)
            EnableActiveOneOrdering = Boolean.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, FTP_WORKER_PENDING_LIMIT_TICKS_KEY)
            FtpWorkerPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, FTP_REQUEST_LIMIT_TICKS_KEY)
            FtpRequestLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, FTP_LOGOUT_LIMIT_TICKS_KEY)
            FtpLogoutLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, FTP_TRANSFER_STALL_LIMIT_TICKS_KEY)
            FtpTransferStallLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, FTP_USE_PASSIVE_MODE_KEY)
            FtpUsePassiveMode = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, FTP_LOGOUT_EACH_TIME_KEY)
            FtpLogoutEachTime = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, FTP_BUFFER_LENGTH_KEY)
            FtpBufferLength = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
