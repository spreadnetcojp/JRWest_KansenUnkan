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

    '���[�J���t�@�C���V�X�e���ɂ�����FTP�T�[�o���[�g�f�B���N�g����
    Public Shared FtpServerRootDirPath As String

    'FTP�T�[�o���ɂ�����A�N�Z�X���f�B���N�g����
    Public Shared PermittedPathInFtp As String

    '�X���b�h�ʃe���|�����f�B���N�g���̃x�[�X�i�v���Z�X�̃e���|�����f�B���N�g���j
    Public Shared TemporaryBaseDirPath As String

    '�Ď��Ճv���O������CAB���ɂ�����o�[�W�������X�g�t�@�C���̃p�X
    Public Shared KsbProgramVersionListPathInCab As String

    '���D�@�v���O������CAB���ɂ�����o�[�W�������X�g�t�@�C���̃p�X
    Public Shared GateProgramVersionListPathInCab As String

    '�����v���O������CAB���ɂ�����o�[�W�������X�g�t�@�C���̃p�X
    Public Shared MadoProgramVersionListPathInCab As String

    'NOTE: XxxProgramGroupNamesInCab��XxxProgramGroupTitles�̗v�f��
    '�����⏇���́A�v���O�����o�[�W���������̗̈�̌����⏇���Ɠ���ł���B
    'NOTE: �e�t�@�C���̕\�����́A�^�ǃT�[�o���Ƀv���O������o�^�����ۂɁA
    'XxxProgramGroupTitles�̗v�f���Q�Ƃ��Č��肷��B
    'XxxProgramGroupTitles�̓��Y�v�f��1�����ȏ�̏ꍇ�́A�����
    '�t�@�C�����i�g���q�����ς݁j��A�������p�X�������\�����Ƃ���B
    'XxxProgramGroupTitles�̓��Y�v�f��0�����̏ꍇ�́A�t�@�C���t�b�^
    '�ɐݒ肳��Ă���u�\���p�f�[�^�v��\�����Ƃ���B

    '���D�@�v���O������CAB���ɂ�����S�O���[�v�f�B���N�g���̃x�[�X�p�X
    Public Shared GateProgramGroupBasePathInCab As String

    '���D�@�v���O������CAB���ɂ�����e�O���[�v�f�B���N�g���̖��O
    Public Shared GateProgramGroupNamesInCab As String()

    '���D�@�v���O�����̊e�O���[�v�f�B���N�g���̕\����
    Public Shared GateProgramGroupTitles As String()

    '�d���ʐM�p���b�X���A�h���X
    Public Shared IpAddrForTelegConnection As IPAddress

    '�d���ʐM�p�|�[�g�ԍ�
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

    '�E�H�b�`�h�b�O�V�[�P���X�ɂ����鉞���d����M����
    Public Shared WatchdogReplyLimitTicks As Integer

    '�Ή^�ǒ[���ʐM�v���Z�X�̃t�@�C���]���V�[�P���X�r���������[�h�ݒ�
    Public Shared EnableXllStrongExclusion As Boolean

    '�Ή^�ǒ[���ʐM�v���Z�X�̔\���I�V�[�P���X�r���������[�h�ݒ�
    Public Shared EnableActiveSeqStrongExclusion As Boolean

    '�Ή^�ǒ[���ʐM�v���Z�X�̔\���I�P���V�[�P���X�����������[�h�ݒ�
    Public Shared EnableActiveOneOrdering As Boolean

    '�^�ǒ[���t�@�C��ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared OpClientFileUllTransferLimitTicks As Integer

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const APP_ID As String = "ToOpClient"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const FTP_SERVER_ROOT_DIR_PATH_KEY As String = APP_ID & "FtpServerRootDirPath"
    Private Const PERMITTED_PATH_IN_FTP_KEY As String = APP_ID & "PermittedPathInFtp"
    Private Const TEMP_BASE_DIR_PATH_KEY As String = APP_ID & "TemporaryBaseDirPath"
    Private Const KSB_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "KsbProgramVersionListPathInCab"
    Private Const GATE_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "GateProgramVersionListPathInCab"
    Private Const MADO_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "MadoProgramVersionListPathInCab"
    Private Const GATE_PRG_GROUP_BASE_PATH_IN_CAB_KEY As String = "GateProgramGroupBasePathInCab"
    Private Const GATE_PRG_GROUP_NAMES_IN_CAB_KEY As String = "GateProgramGroupNamesInCab"
    Private Const GATE_PRG_GROUP_TITLES_IN_CAB_KEY As String = "GateProgramGroupTitles"
    Private Const SELF_ADDR_KEY As String = "SelfAddr"
    Private Const TELEG_CON_PORT_KEY As String = APP_ID & "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = APP_ID & "TelegrapherPendingLimitTicks"
    Private Const WATCHDOG_INTERVAL_KEY As String = APP_ID & "WatchdogIntervalTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = APP_ID & "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = APP_ID & "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = APP_ID & "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = APP_ID & "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = APP_ID & "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = APP_ID & "TelegLoggingMaxLengthOnWrite"
    Private Const WATCHDOG_REPLY_LIMIT_KEY As String = APP_ID & "WatchdogReplyLimitTicks"
    Private Const ENABLE_XLL_STRONG_EXCLUSION_KEY As String = APP_ID & "EnableXllStrongExclusion"
    Private Const ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY As String = APP_ID & "EnableActiveSeqStrongExclusion"
    Private Const ENABLE_ACTIVE_ONE_ORDERING_KEY As String = APP_ID & "EnableActiveOneOrdering"

    Private Const OPC_FILE_ULL_TRANSFER_LIMIT_KEY As String = "OpClientFileUllTransferLimitTicks"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̑Ή^�ǒ[���ʐM�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID, True)

        Try
            ReadFileElem(PATH_SECTION, FTP_SERVER_ROOT_DIR_PATH_KEY)
            FtpServerRootDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, PERMITTED_PATH_IN_FTP_KEY)
            PermittedPathInFtp = LastReadValue

            ReadFileElem(PATH_SECTION, TEMP_BASE_DIR_PATH_KEY)
            TemporaryBaseDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, KSB_PRG_VER_LIST_PATH_IN_CAB_KEY)
            KsbProgramVersionListPathInCab = LastReadValue

            ReadFileElem(PATH_SECTION, GATE_PRG_VER_LIST_PATH_IN_CAB_KEY)
            GateProgramVersionListPathInCab = LastReadValue

            ReadFileElem(PATH_SECTION, MADO_PRG_VER_LIST_PATH_IN_CAB_KEY)
            MadoProgramVersionListPathInCab = LastReadValue

            ReadFileElem(PATH_SECTION, GATE_PRG_GROUP_BASE_PATH_IN_CAB_KEY)
            GateProgramGroupBasePathInCab = LastReadValue

            ReadFileElem(PATH_SECTION, GATE_PRG_GROUP_NAMES_IN_CAB_KEY)
            GateProgramGroupNamesInCab = LastReadValue.Split(","c)

            ReadFileElem(PATH_SECTION, GATE_PRG_GROUP_TITLES_IN_CAB_KEY)
            GateProgramGroupTitles = LastReadValue.Split(","c)

            If GateProgramGroupNamesInCab.Length <> GateProgramGroupTitles.Length Then
                Throw New OPMGException("Number of the elements is invalid. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If

            ReadFileElem(NETWORK_SECTION, SELF_ADDR_KEY)
            IpAddrForTelegConnection = IPAddress.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, WATCHDOG_INTERVAL_KEY)
            WatchdogIntervalTicks = Integer.Parse(LastReadValue)

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

            ReadFileElem(TIME_INFO_SECTION, WATCHDOG_REPLY_LIMIT_KEY)
            WatchdogReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_XLL_STRONG_EXCLUSION_KEY)
            EnableXllStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY)
            EnableActiveSeqStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_ONE_ORDERING_KEY)
            EnableActiveOneOrdering = Boolean.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, OPC_FILE_ULL_TRANSFER_LIMIT_KEY)
            OpClientFileUllTransferLimitTicks = Integer.Parse(LastReadValue)
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
