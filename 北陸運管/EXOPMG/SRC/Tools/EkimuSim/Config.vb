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

Imports JR.ExOpmg.Common

'TODO: �A�v���̓��쒆�ɕύX���������̂́A�������珜�����AMainForm.vb��
'UiStateClass�ɐ錾����B

Public Class Config
    Inherits BaseConfig

    '���Ǝ�
    Public Shared SelfCompany As EkCompany

    '�����u�T�C�o�l�R�[�h
    Public Shared SelfEkCode As EkCode

    'FTP���[�U��
    Public Shared FtpUserName As String

    'FTP�p�X���[�h
    Public Shared FtpPassword As String

    '�L�����O���
    Public Shared LogKindsMask As Integer

    '�f�t�H���g���M�t�@�C���i�[�f�B���N�g��
    Public Shared DefaultApplyDataDirPath As String

    'FTP�T�[�o���ɂ�����@��ʃf�B���N�g����
    Public Shared ModelPathInFtp As String

    'FTP�T�[�oURI
    Public Shared FtpServerUri As String

    '�^�ǃT�[�oIP�A�h���X
    Public Shared ServerIpAddr As String

    '�d���ʐM�p�|�[�g�ԍ�
    Public Shared IpPortForTelegConnection As Integer

    '�d������M�X���b�h��~���e����
    Public Shared TelegrapherPendingLimitTicks As Integer

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

    '�����f�[�^�擾�v���d���̉�����M����
    Public Shared TimeDataGetReplyLimitTicks As Integer

    '�A�v���w�i�Z�b�V�����w�j�v���g�R��
    Public Shared AplProtocol As EkAplProtocol

    '�E�H�b�`�h�b�O�ɂ������ԊĎ��̗v��
    Public Shared EnableWatchdog As Boolean

    '�t�@�C���]���V�[�P���X�r���������[�h�ݒ�
    Public Shared EnableXllStrongExclusion As Boolean

    '�\���I�V�[�P���X�r���������[�h�ݒ�
    Public Shared EnableActiveSeqStrongExclusion As Boolean

    '�\���I�P���V�[�P���X�����������[�h�ݒ�
    Public Shared EnableActiveOneOrdering As Boolean

    '�\���V�[�P���X�pFTP�X���b�h�̒�~���e����
    Public Shared ActiveFtpWorkerPendingLimitTicks As Integer

    '�\���V�[�P���X�pFTP�̊e�탊�N�G�X�g�ɑ΂��鉞����M����
    Public Shared ActiveFtpRequestLimitTicks As Integer

    '�\���V�[�P���X�pFTP�̃��O�A�E�g�̃��N�G�X�g�ɑ΂��鉞����M����
    Public Shared ActiveFtpLogoutLimitTicks As Integer

    '�\���V�[�P���X�pFTP�ňُ�Ɣ��肷��f�[�^�]����~���ԁi-1�͖������j
    Public Shared ActiveFtpTransferStallLimitTicks As Integer

    '�\���V�[�P���X�pFTP�Ńp�b�V�u���[�h���g�����ۂ�
    Public Shared ActiveFtpUsePassiveMode As Boolean

    '�\���V�[�P���X�pFTP�œ]�����s�����ƂɃ��O�A�E�g���邩�ۂ�
    Public Shared ActiveFtpLogoutEachTime As Boolean

    '�\���V�[�P���X�pFTP�Ŏg�p����o�b�t�@�̗e��
    Public Shared ActiveFtpBufferLength As Integer

    '�󓮃V�[�P���X�pFTP�X���b�h�̒�~���e����
    Public Shared PassiveFtpWorkerPendingLimitTicks As Integer

    '�󓮃V�[�P���X�pFTP�̊e�탊�N�G�X�g�ɑ΂��鉞����M����
    Public Shared PassiveFtpRequestLimitTicks As Integer

    '�󓮃V�[�P���X�pFTP�̃��O�A�E�g�̃��N�G�X�g�ɑ΂��鉞����M����
    Public Shared PassiveFtpLogoutLimitTicks As Integer

    '�󓮃V�[�P���X�pFTP�ňُ�Ɣ��肷��f�[�^�]����~���ԁi-1�͖������j
    Public Shared PassiveFtpTransferStallLimitTicks As Integer

    '�󓮃V�[�P���X�pFTP�Ńp�b�V�u���[�h���g�����ۂ�
    Public Shared PassiveFtpUsePassiveMode As Boolean

    '�󓮃V�[�P���X�pFTP�œ]�����s�����ƂɃ��O�A�E�g���邩�ۂ�
    Public Shared PassiveFtpLogoutEachTime As Boolean

    '�󓮃V�[�P���X�pFTP�Ŏg�p����o�b�t�@�̗e��
    Public Shared PassiveFtpBufferLength As Integer

    'INI�t�@�C�����̃Z�N�V������
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const PATH_SECTION As String = "Path"
    Protected Const NETWORK_SECTION As String = "Network"
    Protected Const TIME_INFO_SECTION As String = "TimeInfo"
    Protected Const TELEGRAPHER_MODE_SECTION As String = "TelegrapherMode"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const SELF_COMPANY_KEY As String = "SelfCompany"
    Private Const SELF_EKCODE_KEY As String = "SelfEkCode"
    Private Const FTP_USER_NAME_KEY As String = "FtpUserName"
    Private Const FTP_PASSWORD_KEY As String = "FtpPassword"
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const DEFAULT_APPLY_DATA_DIR_PATH_KEY As String = "DefaultApplyDataDirPath"
    Private Const MODEL_PATH_IN_FTP_KEY As String = "ModelPathInFtp"
    Private Const FTP_SERVER_URI_KEY As String = "FtpServerUri"
    Private Const SERVER_IP_ADDR_KEY As String = "ServerIpAddr"
    Private Const TELEG_CON_PORT_KEY As String = "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = "TelegrapherPendingLimitTicks"
    Private Const WATCHDOG_INTERVAL_LIMIT_KEY As String = "WatchdogIntervalLimitTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = "TelegLoggingMaxLengthOnWrite"
    Private Const COM_START_REPLY_LIMIT_KEY As String = "ComStartReplyLimitTicks"
    Private Const TIME_DATA_GET_REPLY_LIMIT_KEY As String = "TimeDataGetReplyLimitTicks"
    Private Const APL_PROTOCOL_KEY As String = "AplProtocol"
    Private Const ENABLE_WATCHDOG_KEY As String = "EnableWatchdog"
    Private Const ENABLE_XLL_STRONG_EXCLUSION_KEY As String = "EnableXllStrongExclusion"
    Private Const ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY As String = "EnableActiveSeqStrongExclusion"
    Private Const ENABLE_ACTIVE_ONE_ORDERING_KEY As String = "EnableActiveOneOrdering"
    Private Const ACTIVE_FTP_WORKER_PENDING_LIMIT_TICKS_KEY As String = "ActiveFtpWorkerPendingLimitTicks"
    Private Const ACTIVE_FTP_REQUEST_LIMIT_TICKS_KEY As String = "ActiveFtpRequestLimitTicks"
    Private Const ACTIVE_FTP_LOGOUT_LIMIT_TICKS_KEY As String = "ActiveFtpLogoutLimitTicks"
    Private Const ACTIVE_FTP_TRANSFER_STALL_LIMIT_TICKS_KEY As String = "ActiveFtpTransferStallLimitTicks"
    Private Const ACTIVE_FTP_USE_PASSIVE_MODE_KEY As String = "ActiveFtpUsePassiveMode"
    Private Const ACTIVE_FTP_LOGOUT_EACH_TIME_KEY As String = "ActiveFtpLogoutEachTime"
    Private Const ACTIVE_FTP_BUFFER_LENGTH_KEY As String = "ActiveFtpBufferLength"
    Private Const PASSIVE_FTP_WORKER_PENDING_LIMIT_TICKS_KEY As String = "PassiveFtpWorkerPendingLimitTicks"
    Private Const PASSIVE_FTP_REQUEST_LIMIT_TICKS_KEY As String = "PassiveFtpRequestLimitTicks"
    Private Const PASSIVE_FTP_LOGOUT_LIMIT_TICKS_KEY As String = "PassiveFtpLogoutLimitTicks"
    Private Const PASSIVE_FTP_TRANSFER_STALL_LIMIT_TICKS_KEY As String = "PassiveFtpTransferStallLimitTicks"
    Private Const PASSIVE_FTP_USE_PASSIVE_MODE_KEY As String = "PassiveFtpUsePassiveMode"
    Private Const PASSIVE_FTP_LOGOUT_EACH_TIME_KEY As String = "PassiveFtpLogoutEachTime"
    Private Const PASSIVE_FTP_BUFFER_LENGTH_KEY As String = "PassiveFtpBufferLength"

    ''' <summary>INI�t�@�C������w���@��V�~�����[�^�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Try
            ReadFileElem(CREDENTIAL_SECTION, SELF_COMPANY_KEY)
            SelfCompany = DirectCast([Enum].Parse(GetType(EkCompany), LastReadValue), EkCompany)

            ReadFileElem(CREDENTIAL_SECTION, SELF_EKCODE_KEY)
            SelfEkCode = EkCode.Parse(LastReadValue, "%M-%R-%S-%C-%U")

            ReadFileElem(CREDENTIAL_SECTION, FTP_USER_NAME_KEY)
            FtpUserName = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, FTP_PASSWORD_KEY)
            FtpPassword = LastReadValue

            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(PATH_SECTION, DEFAULT_APPLY_DATA_DIR_PATH_KEY)
            DefaultApplyDataDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, MODEL_PATH_IN_FTP_KEY)
            ModelPathInFtp = LastReadValue

            ReadFileElem(NETWORK_SECTION, FTP_SERVER_URI_KEY)
            FtpServerUri = LastReadValue

            ReadFileElem(NETWORK_SECTION, SERVER_IP_ADDR_KEY)
            ServerIpAddr = LastReadValue

            ReadFileElem(NETWORK_SECTION, TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

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

            ReadFileElem(TIME_INFO_SECTION, TIME_DATA_GET_REPLY_LIMIT_KEY)
            TimeDataGetReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, APL_PROTOCOL_KEY)
            AplProtocol = DirectCast([Enum].Parse(GetType(EkAplProtocol), LastReadValue), EkAplProtocol)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_WATCHDOG_KEY)
            EnableWatchdog = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_XLL_STRONG_EXCLUSION_KEY)
            EnableXllStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY)
            EnableActiveSeqStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_ONE_ORDERING_KEY)
            EnableActiveOneOrdering = Boolean.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, ACTIVE_FTP_WORKER_PENDING_LIMIT_TICKS_KEY)
            ActiveFtpWorkerPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, ACTIVE_FTP_REQUEST_LIMIT_TICKS_KEY)
            ActiveFtpRequestLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, ACTIVE_FTP_LOGOUT_LIMIT_TICKS_KEY)
            ActiveFtpLogoutLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, ACTIVE_FTP_TRANSFER_STALL_LIMIT_TICKS_KEY)
            ActiveFtpTransferStallLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ACTIVE_FTP_USE_PASSIVE_MODE_KEY)
            ActiveFtpUsePassiveMode = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ACTIVE_FTP_LOGOUT_EACH_TIME_KEY)
            ActiveFtpLogoutEachTime = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ACTIVE_FTP_BUFFER_LENGTH_KEY)
            ActiveFtpBufferLength = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, PASSIVE_FTP_WORKER_PENDING_LIMIT_TICKS_KEY)
            PassiveFtpWorkerPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, PASSIVE_FTP_REQUEST_LIMIT_TICKS_KEY)
            PassiveFtpRequestLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, PASSIVE_FTP_LOGOUT_LIMIT_TICKS_KEY)
            PassiveFtpLogoutLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, PASSIVE_FTP_TRANSFER_STALL_LIMIT_TICKS_KEY)
            PassiveFtpTransferStallLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, PASSIVE_FTP_USE_PASSIVE_MODE_KEY)
            PassiveFtpUsePassiveMode = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, PASSIVE_FTP_LOGOUT_EACH_TIME_KEY)
            PassiveFtpLogoutEachTime = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, PASSIVE_FTP_BUFFER_LENGTH_KEY)
            PassiveFtpBufferLength = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
