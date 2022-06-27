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

    '�ڑ���Ԏ擾�V�[�P���X�̊Ԋu
    Public Shared TktConStatusGetIntervalTicks As Integer

    '�ڑ���Ԏ擾�V�[�P���X�ɂ����鉞���d����M����
    Public Shared TktConStatusGetReplyLimitTicks As Integer

    '�ڑ���Ԏ擾�V�[�P���X�ɂ����郊�g���C�̃C���^�[�o��
    Public Shared TktConStatusGetRetryIntervalTicks As Integer

    '�ڑ���Ԏ擾�V�[�P���X�ɂ����郊�g���C�̍ő�񐔁i����Ƃ݂Ȃ��ׂ�NAK��M���j
    Public Shared TktConStatusGetMaxRetryCountToForget As Integer

    '�ڑ���Ԏ擾�V�[�P���X�ɂ����郊�g���C�̍ő�񐔁i�p�����ׂ��łȂ�NAK��M���j
    Public Shared TktConStatusGetMaxRetryCountToCare As Integer

    '�����}�X�^�ꎮDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MadoMasterSuiteDllTransferLimitTicks As Integer

    '�����}�X�^�ꎮDLL�ɂ�����J�n�d���̉�����M����
    Public Shared MadoMasterSuiteDllStartReplyLimitTicks As Integer

    '�����}�X�^�ꎮDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared MadoMasterSuiteDllRetryIntervalTicks As Integer

    '�����}�X�^�ꎮDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared MadoMasterSuiteDllMaxRetryCountToCare As Integer

    '�����}�X�^�K�p���X�gDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MadoMasterListDllTransferLimitTicks As Integer

    '�����}�X�^�K�p���X�gDLL�ɂ�����J�n�d���̉�����M����
    Public Shared MadoMasterListDllStartReplyLimitTicks As Integer

    '�����}�X�^�K�p���X�gDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared MadoMasterListDllRetryIntervalTicks As Integer

    '�����}�X�^�K�p���X�gDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared MadoMasterListDllMaxRetryCountToCare As Integer

    '�����v���O�����ꎮDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MadoProgramSuiteDllTransferLimitTicks As Integer

    '�����v���O�����ꎮDLL�ɂ�����J�n�d���̉�����M����
    Public Shared MadoProgramSuiteDllStartReplyLimitTicks As Integer

    '�����v���O�����ꎮDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared MadoProgramSuiteDllRetryIntervalTicks As Integer

    '�����v���O�����ꎮDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared MadoProgramSuiteDllMaxRetryCountToCare As Integer

    '�����v���O�����K�p���X�gDLL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MadoProgramListDllTransferLimitTicks As Integer

    '�����v���O�����K�p���X�gDLL�ɂ�����J�n�d���̉�����M����
    Public Shared MadoProgramListDllStartReplyLimitTicks As Integer

    '�����v���O�����K�p���X�gDLL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared MadoProgramListDllRetryIntervalTicks As Integer

    '�����v���O�����K�p���X�gDLL�ɂ�����J�n���g���C�̍ő��
    Public Shared MadoProgramListDllMaxRetryCountToCare As Integer

    '�����}�X�^�o�[�W�������ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MadoMasterVersionInfoUllTransferLimitTicks As Integer

    '�����v���O�����o�[�W�������ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MadoProgramVersionInfoUllTransferLimitTicks As Integer

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const MODEL_NAME As String = "Tokatsu"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const TKT_CON_STATUS_GET_INTERVAL_TICKS_KEY As String = "TktConStatusGetIntervalTicks"
    Private Const TKT_CON_STATUS_GET_REPLY_LIMIT_TICKS_KEY As String = "TktConStatusGetReplyLimitTicks"
    Private Const TKT_CON_STATUS_GET_RETRY_INTERVAL_TICKS_KEY As String = "TktConStatusGetRetryIntervalTicks"
    Private Const TKT_CON_STATUS_GET_MAX_RETRY_COUNT_TO_FORGET_KEY As String = "TktConStatusGetMaxRetryCountToForget"
    Private Const TKT_CON_STATUS_GET_MAX_RETRY_COUNT_TO_CARE_KEY As String = "TktConStatusGetMaxRetryCountToCare"
    Private Const MADO_MASTER_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "MadoMasterSuiteDllTransferLimitTicks"
    Private Const MADO_MASTER_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "MadoMasterSuiteDllStartReplyLimitTicks"
    Private Const MADO_MASTER_SUITE_DLL_RETRY_INTERVAL_KEY As String = "MadoMasterSuiteDllRetryIntervalTicks"
    Private Const MADO_MASTER_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "MadoMasterSuiteDllMaxRetryCountToCare"
    Private Const MADO_MASTER_LIST_DLL_TRANSFER_LIMIT_KEY As String = "MadoMasterListDllTransferLimitTicks"
    Private Const MADO_MASTER_LIST_DLL_START_REPLY_LIMIT_KEY As String = "MadoMasterListDllStartReplyLimitTicks"
    Private Const MADO_MASTER_LIST_DLL_RETRY_INTERVAL_KEY As String = "MadoMasterListDllRetryIntervalTicks"
    Private Const MADO_MASTER_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "MadoMasterListDllMaxRetryCountToCare"
    Private Const MADO_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "MadoProgramSuiteDllTransferLimitTicks"
    Private Const MADO_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "MadoProgramSuiteDllStartReplyLimitTicks"
    Private Const MADO_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY As String = "MadoProgramSuiteDllRetryIntervalTicks"
    Private Const MADO_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "MadoProgramSuiteDllMaxRetryCountToCare"
    Private Const MADO_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY As String = "MadoProgramListDllTransferLimitTicks"
    Private Const MADO_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY As String = "MadoProgramListDllStartReplyLimitTicks"
    Private Const MADO_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY As String = "MadoProgramListDllRetryIntervalTicks"
    Private Const MADO_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "MadoProgramListDllMaxRetryCountToCare"
    Private Const MADO_MASTER_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "MadoMasterVersionInfoUllTransferLimitTicks"
    Private Const MADO_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "MadoProgramVersionInfoUllTransferLimitTicks"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̑Γ����ʐM�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        TelServerAppBaseInit(sIniFilePath, MODEL_NAME, True)

        Dim sAppIdentifier As String = "To" & MODEL_NAME
        Try
            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_INTERVAL_TICKS_KEY)
            TktConStatusGetIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_REPLY_LIMIT_TICKS_KEY)
            TktConStatusGetReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_RETRY_INTERVAL_TICKS_KEY)
            TktConStatusGetRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_MAX_RETRY_COUNT_TO_FORGET_KEY)
            TktConStatusGetMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_MAX_RETRY_COUNT_TO_CARE_KEY)
            TktConStatusGetMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_SUITE_DLL_TRANSFER_LIMIT_KEY)
            MadoMasterSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_SUITE_DLL_START_REPLY_LIMIT_KEY)
            MadoMasterSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_SUITE_DLL_RETRY_INTERVAL_KEY)
            MadoMasterSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            MadoMasterSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_LIST_DLL_TRANSFER_LIMIT_KEY)
            MadoMasterListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_LIST_DLL_START_REPLY_LIMIT_KEY)
            MadoMasterListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_LIST_DLL_RETRY_INTERVAL_KEY)
            MadoMasterListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            MadoMasterListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY)
            MadoProgramSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY)
            MadoProgramSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY)
            MadoProgramSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            MadoProgramSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY)
            MadoProgramListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY)
            MadoProgramListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY)
            MadoProgramListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            MadoProgramListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            MadoMasterVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            MadoProgramVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)
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
