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

Public Class Config
    Inherits TelServerAppBaseConfig

    '���D�@�}�X�^�ꎮDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateMasterSuiteDllTransferLimitTicks As Integer

    '���D�@�}�X�^�ꎮDLL�ɂ�����J�n�d���̉�����M����
    Public Shared GateMasterSuiteDllStartReplyLimitTicks As Integer

    '���D�@�}�X�^�ꎮDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared GateMasterSuiteDllRetryIntervalTicks As Integer

    '���D�@�}�X�^�ꎮDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared GateMasterSuiteDllMaxRetryCountToCare As Integer

    '���D�@�}�X�^�K�p���X�gDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateMasterListDllTransferLimitTicks As Integer

    '���D�@�}�X�^�K�p���X�gDLL�ɂ�����J�n�d���̉�����M����
    Public Shared GateMasterListDllStartReplyLimitTicks As Integer

    '���D�@�}�X�^�K�p���X�gDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared GateMasterListDllRetryIntervalTicks As Integer

    '���D�@�}�X�^�K�p���X�gDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared GateMasterListDllMaxRetryCountToCare As Integer

    '���D�@�}�X�^�o�[�W�������ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateMasterVersionInfoUllTransferLimitTicks As Integer

    '���D�@�v���O�����ꎮDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateProgramSuiteDllTransferLimitTicks As Integer

    '���D�@�v���O�����ꎮDLL�ɂ�����J�n�d���̉�����M����
    Public Shared GateProgramSuiteDllStartReplyLimitTicks As Integer

    '���D�@�v���O�����ꎮDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared GateProgramSuiteDllRetryIntervalTicks As Integer

    '���D�@�v���O�����ꎮDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared GateProgramSuiteDllMaxRetryCountToCare As Integer

    '���D�@�v���O�����K�p���X�gDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateProgramListDllTransferLimitTicks As Integer

    '���D�@�v���O�����K�p���X�gDLL�ɂ�����J�n�d���̉�����M����
    Public Shared GateProgramListDllStartReplyLimitTicks As Integer

    '���D�@�v���O�����K�p���X�gDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared GateProgramListDllRetryIntervalTicks As Integer

    '���D�@�v���O�����K�p���X�gDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared GateProgramListDllMaxRetryCountToCare As Integer

    '���D�@�v���O�����o�[�W�������ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateProgramVersionInfoUllTransferLimitTicks As Integer

    '�Ď��Ճv���O�����ꎮDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared KsbProgramSuiteDllTransferLimitTicks As Integer

    '�Ď��Ճv���O�����ꎮDLL�ɂ�����J�n�d���̉�����M����
    Public Shared KsbProgramSuiteDllStartReplyLimitTicks As Integer

    '�Ď��Ճv���O�����ꎮDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared KsbProgramSuiteDllRetryIntervalTicks As Integer

    '�Ď��Ճv���O�����ꎮDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared KsbProgramSuiteDllMaxRetryCountToCare As Integer

    '�Ď��Ճv���O�����K�p���X�gDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared KsbProgramListDllTransferLimitTicks As Integer

    '�Ď��Ճv���O�����K�p���X�gDLL�ɂ�����J�n�d���̉�����M����
    Public Shared KsbProgramListDllStartReplyLimitTicks As Integer

    '�Ď��Ճv���O�����K�p���X�gDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared KsbProgramListDllRetryIntervalTicks As Integer

    '�Ď��Ճv���O�����K�p���X�gDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared KsbProgramListDllMaxRetryCountToCare As Integer

    '�Ď��Ճv���O�����o�[�W�������ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared KsbProgramVersionInfoUllTransferLimitTicks As Integer

    '�ʏW�D�f�[�^ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateBesshuDataUllTransferLimitTicks As Integer

    '�ʏW�D�f�[�^ULL�ɂ�����J�n�d���̉�����M����
    Public Shared GateBesshuDataUllStartReplyLimitTicks As Integer

    '�ʏW�D�f�[�^ULL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared GateBesshuDataUllRetryIntervalTicks As Integer

    '�ʏW�D�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i����Ƃ݂Ȃ��ׂ�NAK��M���j
    Public Shared GateBesshuDataUllMaxRetryCountToForget As Integer

    '�ʏW�D�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i�p�����ׂ��łȂ�NAK��M���j
    Public Shared GateBesshuDataUllMaxRetryCountToCare As Integer

    '���׃f�[�^ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateMeisaiDataUllTransferLimitTicks As Integer

    '���׃f�[�^ULL�ɂ�����J�n�d���̉�����M����
    Public Shared GateMeisaiDataUllStartReplyLimitTicks As Integer

    '���׃f�[�^ULL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared GateMeisaiDataUllRetryIntervalTicks As Integer

    '���׃f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i����Ƃ݂Ȃ��ׂ�NAK��M���j
    Public Shared GateMeisaiDataUllMaxRetryCountToForget As Integer

    '���׃f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i�p�����ׂ��łȂ�NAK��M���j
    Public Shared GateMeisaiDataUllMaxRetryCountToCare As Integer

    '�ُ�f�[�^ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared KsbGateFaultDataUllTransferLimitTicks As Integer

    '�ُ�f�[�^ULL�ɂ�����J�n�d���̉�����M����
    Public Shared KsbGateFaultDataUllStartReplyLimitTicks As Integer

    '�ُ�f�[�^ULL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared KsbGateFaultDataUllRetryIntervalTicks As Integer

    '�ُ�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i����Ƃ݂Ȃ��ׂ�NAK��M���j
    Public Shared KsbGateFaultDataUllMaxRetryCountToForget As Integer

    '�ُ�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i�p�����ׂ��łȂ�NAK��M���j
    Public Shared KsbGateFaultDataUllMaxRetryCountToCare As Integer

    '�ғ��E�ێ�f�[�^ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateKadoDataUllTransferLimitTicks As Integer

    '�ғ��E�ێ�f�[�^ULL�ɂ�����J�n�d���̉�����M����
    Public Shared GateKadoDataUllStartReplyLimitTicks As Integer

    '�ғ��E�ێ�f�[�^ULL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared GateKadoDataUllRetryIntervalTicks As Integer

    '�ғ��E�ێ�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i����Ƃ݂Ȃ��ׂ�NAK��M���j
    Public Shared GateKadoDataUllMaxRetryCountToForget As Integer

    '�ғ��E�ێ�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i�p�����ׂ��łȂ�NAK��M���j
    Public Shared GateKadoDataUllMaxRetryCountToCare As Integer

    '���ԑѕʏ�~�f�[�^ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared GateTrafficDataUllTransferLimitTicks As Integer

    '���ԑѕʏ�~�f�[�^ULL�ɂ�����J�n�d���̉�����M����
    Public Shared GateTrafficDataUllStartReplyLimitTicks As Integer

    '���ԑѕʏ�~�f�[�^ULL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared GateTrafficDataUllRetryIntervalTicks As Integer

    '���ԑѕʏ�~�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i����Ƃ݂Ȃ��ׂ�NAK��M���j
    Public Shared GateTrafficDataUllMaxRetryCountToForget As Integer

    '���ԑѕʏ�~�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i�p�����ׂ��łȂ�NAK��M���j
    Public Shared GateTrafficDataUllMaxRetryCountToCare As Integer

    '���D�@�v���O�����̊e�O���[�v�f�B���N�g���̕\����
    Public Shared GateProgramGroupTitles As String()

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const MODEL_NAME As String = "Kanshiban"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const GATE_MASTER_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "GateMasterSuiteDllTransferLimitTicks"
    Private Const GATE_MASTER_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "GateMasterSuiteDllStartReplyLimitTicks"
    Private Const GATE_MASTER_SUITE_DLL_RETRY_INTERVAL_KEY As String = "GateMasterSuiteDllRetryIntervalTicks"
    Private Const GATE_MASTER_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "GateMasterSuiteDllMaxRetryCountToCare"
    Private Const GATE_MASTER_LIST_DLL_TRANSFER_LIMIT_KEY As String = "GateMasterListDllTransferLimitTicks"
    Private Const GATE_MASTER_LIST_DLL_START_REPLY_LIMIT_KEY As String = "GateMasterListDllStartReplyLimitTicks"
    Private Const GATE_MASTER_LIST_DLL_RETRY_INTERVAL_KEY As String = "GateMasterListDllRetryIntervalTicks"
    Private Const GATE_MASTER_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "GateMasterListDllMaxRetryCountToCare"
    Private Const GATE_MASTER_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "GateMasterVersionInfoUllTransferLimitTicks"
    Private Const GATE_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "GateProgramSuiteDllTransferLimitTicks"
    Private Const GATE_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "GateProgramSuiteDllStartReplyLimitTicks"
    Private Const GATE_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY As String = "GateProgramSuiteDllRetryIntervalTicks"
    Private Const GATE_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "GateProgramSuiteDllMaxRetryCountToCare"
    Private Const GATE_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY As String = "GateProgramListDllTransferLimitTicks"
    Private Const GATE_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY As String = "GateProgramListDllStartReplyLimitTicks"
    Private Const GATE_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY As String = "GateProgramListDllRetryIntervalTicks"
    Private Const GATE_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "GateProgramListDllMaxRetryCountToCare"
    Private Const GATE_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "GateProgramVersionInfoUllTransferLimitTicks"
    Private Const KSB_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "KsbProgramSuiteDllTransferLimitTicks"
    Private Const KSB_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "KsbProgramSuiteDllStartReplyLimitTicks"
    Private Const KSB_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY As String = "KsbProgramSuiteDllRetryIntervalTicks"
    Private Const KSB_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "KsbProgramSuiteDllMaxRetryCountToCare"
    Private Const KSB_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY As String = "KsbProgramListDllTransferLimitTicks"
    Private Const KSB_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY As String = "KsbProgramListDllStartReplyLimitTicks"
    Private Const KSB_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY As String = "KsbProgramListDllRetryIntervalTicks"
    Private Const KSB_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "KsbProgramListDllMaxRetryCountToCare"
    Private Const KSB_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "KsbProgramVersionInfoUllTransferLimitTicks"

    Private Const GATE_BESSHU_DATA_ULL_TRANSFER_LIMIT_KEY As String = "GateBesshuDataUllTransferLimitTicks"
    Private Const GATE_BESSHU_DATA_ULL_START_REPLY_LIMIT_KEY As String = "GateBesshuDataUllStartReplyLimitTicks"
    Private Const GATE_BESSHU_DATA_ULL_RETRY_INTERVAL_KEY As String = "GateBesshuDataUllRetryIntervalTicks"
    Private Const GATE_BESSHU_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "GateBesshuDataUllMaxRetryCountToForget"
    Private Const GATE_BESSHU_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "GateBesshuDataUllMaxRetryCountToCare"
    Private Const GATE_MEISAI_DATA_ULL_TRANSFER_LIMIT_KEY As String = "GateMeisaiDataUllTransferLimitTicks"
    Private Const GATE_MEISAI_DATA_ULL_START_REPLY_LIMIT_KEY As String = "GateMeisaiDataUllStartReplyLimitTicks"
    Private Const GATE_MEISAI_DATA_ULL_RETRY_INTERVAL_KEY As String = "GateMeisaiDataUllRetryIntervalTicks"
    Private Const GATE_MEISAI_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "GateMeisaiDataUllMaxRetryCountToForget"
    Private Const GATE_MEISAI_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "GateMeisaiDataUllMaxRetryCountToCare"
    Private Const KSB_GATE_FAULT_DATA_ULL_TRANSFER_LIMIT_KEY As String = "KsbGateFaultDataUllTransferLimitTicks"
    Private Const KSB_GATE_FAULT_DATA_ULL_START_REPLY_LIMIT_KEY As String = "KsbGateFaultDataUllStartReplyLimitTicks"
    Private Const KSB_GATE_FAULT_DATA_ULL_RETRY_INTERVAL_KEY As String = "KsbGateFaultDataUllRetryIntervalTicks"
    Private Const KSB_GATE_FAULT_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "KsbGateFaultDataUllMaxRetryCountToForget"
    Private Const KSB_GATE_FAULT_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "KsbGateFaultDataUllMaxRetryCountToCare"
    Private Const GATE_KADO_DATA_ULL_TRANSFER_LIMIT_KEY As String = "GateKadoDataUllTransferLimitTicks"
    Private Const GATE_KADO_DATA_ULL_START_REPLY_LIMIT_KEY As String = "GateKadoDataUllStartReplyLimitTicks"
    Private Const GATE_KADO_DATA_ULL_RETRY_INTERVAL_KEY As String = "GateKadoDataUllRetryIntervalTicks"
    Private Const GATE_KADO_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "GateKadoDataUllMaxRetryCountToForget"
    Private Const GATE_KADO_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "GateKadoDataUllMaxRetryCountToCare"
    Private Const GATE_TRAFFIC_DATA_ULL_TRANSFER_LIMIT_KEY As String = "GateTrafficDataUllTransferLimitTicks"
    Private Const GATE_TRAFFIC_DATA_ULL_START_REPLY_LIMIT_KEY As String = "GateTrafficDataUllStartReplyLimitTicks"
    Private Const GATE_TRAFFIC_DATA_ULL_RETRY_INTERVAL_KEY As String = "GateTrafficDataUllRetryIntervalTicks"
    Private Const GATE_TRAFFIC_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "GateTrafficDataUllMaxRetryCountToForget"
    Private Const GATE_TRAFFIC_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "GateTrafficDataUllMaxRetryCountToCare"
    Private Const GATE_PRG_GROUP_TITLES_IN_CAB_KEY As String = "GateProgramGroupTitles"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̑ΊĎ��ՒʐM�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        TelServerAppBaseInit(sIniFilePath, MODEL_NAME, True)

        Dim sAppIdentifier As String = "To" & MODEL_NAME
        Try
            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_SUITE_DLL_TRANSFER_LIMIT_KEY)
            GateMasterSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_SUITE_DLL_START_REPLY_LIMIT_KEY)
            GateMasterSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_SUITE_DLL_RETRY_INTERVAL_KEY)
            GateMasterSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            GateMasterSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_LIST_DLL_TRANSFER_LIMIT_KEY)
            GateMasterListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_LIST_DLL_START_REPLY_LIMIT_KEY)
            GateMasterListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_LIST_DLL_RETRY_INTERVAL_KEY)
            GateMasterListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            GateMasterListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            GateMasterVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY)
            GateProgramSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY)
            GateProgramSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY)
            GateProgramSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            GateProgramSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY)
            GateProgramListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY)
            GateProgramListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY)
            GateProgramListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            GateProgramListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            GateProgramVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY)
            KsbProgramSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY)
            KsbProgramSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY)
            KsbProgramSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            KsbProgramSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY)
            KsbProgramListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY)
            KsbProgramListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY)
            KsbProgramListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            KsbProgramListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            KsbProgramVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_TRANSFER_LIMIT_KEY)
            GateBesshuDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_START_REPLY_LIMIT_KEY)
            GateBesshuDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_RETRY_INTERVAL_KEY)
            GateBesshuDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            GateBesshuDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            GateBesshuDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_TRANSFER_LIMIT_KEY)
            GateMeisaiDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_START_REPLY_LIMIT_KEY)
            GateMeisaiDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_RETRY_INTERVAL_KEY)
            GateMeisaiDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            GateMeisaiDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            GateMeisaiDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_TRANSFER_LIMIT_KEY)
            KsbGateFaultDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_START_REPLY_LIMIT_KEY)
            KsbGateFaultDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_RETRY_INTERVAL_KEY)
            KsbGateFaultDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            KsbGateFaultDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            KsbGateFaultDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_TRANSFER_LIMIT_KEY)
            GateKadoDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_START_REPLY_LIMIT_KEY)
            GateKadoDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_RETRY_INTERVAL_KEY)
            GateKadoDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            GateKadoDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            GateKadoDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_TRANSFER_LIMIT_KEY)
            GateTrafficDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_START_REPLY_LIMIT_KEY)
            GateTrafficDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_RETRY_INTERVAL_KEY)
            GateTrafficDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            GateTrafficDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            GateTrafficDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(PATH_SECTION, GATE_PRG_GROUP_TITLES_IN_CAB_KEY)
            GateProgramGroupTitles = LastReadValue.Split(","c)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    Public Shared Sub Dispose()
        TelServerAppBaseDispose()
    End Sub

End Class
