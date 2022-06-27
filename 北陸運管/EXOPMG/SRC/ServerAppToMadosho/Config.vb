' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2015/01/09  (NES)����  �����Ɩ��O�F�؃��O���W�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits TelServerAppBaseConfig

    '�������샍�O�Ǘ��f�B���N�g���̃p�X
    Public Shared MadoLogDirPath As String

    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
    '�����Ɩ��O�F�؃��O�Ǘ��f�B���N�g���̃p�X
    Public Shared MadoCertLogDirPath As String
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------

    '�ُ�f�[�^ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MadoFaultDataUllTransferLimitTicks As Integer

    '�ُ�f�[�^ULL�ɂ�����J�n�d���̉�����M����
    Public Shared MadoFaultDataUllStartReplyLimitTicks As Integer

    '�ُ�f�[�^ULL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared MadoFaultDataUllRetryIntervalTicks As Integer

    '�ُ�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i����Ƃ݂Ȃ��ׂ�NAK��M���j
    Public Shared MadoFaultDataUllMaxRetryCountToForget As Integer

    '�ُ�f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i�p�����ׂ��łȂ�NAK��M���j
    Public Shared MadoFaultDataUllMaxRetryCountToCare As Integer

    '�ғ��f�[�^ULL�ɂ�����ő�]�����ԁi0��-1�͖������j
    Public Shared MadoKadoDataUllTransferLimitTicks As Integer

    '�ғ��f�[�^ULL�ɂ�����J�n�d���̉�����M����
    Public Shared MadoKadoDataUllStartReplyLimitTicks As Integer

    '�ғ��f�[�^ULL�ɂ�����J�n���g���C�̃C���^�[�o��
    Public Shared MadoKadoDataUllRetryIntervalTicks As Integer

    '�ғ��f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i����Ƃ݂Ȃ��ׂ�NAK��M���j
    Public Shared MadoKadoDataUllMaxRetryCountToForget As Integer

    '�ғ��f�[�^ULL�ɂ�����J�n���g���C�̍ő�񐔁i�p�����ׂ��łȂ�NAK��M���j
    Public Shared MadoKadoDataUllMaxRetryCountToCare As Integer

    '�������샍�O�̎}�ԍő�l
    Public Shared MadoLogMaxBranchNumber As Integer

    '�����Ɩ��O�F�؃��O�Ǘ��̎}�ԍő�l
    Public Shared MadoCertLogMaxBranchNumber As Integer

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const MODEL_NAME As String = "Madosho"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const MADO_LOG_DIR_PATH_KEY As String = "MadoLogDirPath"
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
    Private Const MADO_CERT_LOG_DIR_PATH_KEY As String = "MadoCertLogDirPath"
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------
    Private Const MADO_FAULT_DATA_ULL_TRANSFER_LIMIT_KEY As String = "MadoFaultDataUllTransferLimitTicks"
    Private Const MADO_FAULT_DATA_ULL_START_REPLY_LIMIT_KEY As String = "MadoFaultDataUllStartReplyLimitTicks"
    Private Const MADO_FAULT_DATA_ULL_RETRY_INTERVAL_KEY As String = "MadoFaultDataUllRetryIntervalTicks"
    Private Const MADO_FAULT_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "MadoFaultDataUllMaxRetryCountToForget"
    Private Const MADO_FAULT_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "MadoFaultDataUllMaxRetryCountToCare"
    Private Const MADO_KADO_DATA_ULL_TRANSFER_LIMIT_KEY As String = "MadoKadoDataUllTransferLimitTicks"
    Private Const MADO_KADO_DATA_ULL_START_REPLY_LIMIT_KEY As String = "MadoKadoDataUllStartReplyLimitTicks"
    Private Const MADO_KADO_DATA_ULL_RETRY_INTERVAL_KEY As String = "MadoKadoDataUllRetryIntervalTicks"
    Private Const MADO_KADO_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "MadoKadoDataUllMaxRetryCountToForget"
    Private Const MADO_KADO_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "MadoKadoDataUllMaxRetryCountToCare"
    Private Const MADO_LOG_MAX_BRANCH_NUMBER_KEY As String = "MadoLogMaxBranchNumber"
    Private Const MADO_CERT_LOG_MAX_BRANCH_NUMBER_KEY As String = "MadoCertLogMaxBranchNumber"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̑Α����ʐM�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        TelServerAppBaseInit(sIniFilePath, MODEL_NAME, True)

        'Dim sAppIdentifier As String = "To" & MODEL_NAME
        Try
            ReadFileElem(PATH_SECTION, MADO_LOG_DIR_PATH_KEY)
            MadoLogDirPath = LastReadValue

            '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
            ReadFileElem(PATH_SECTION, MADO_CERT_LOG_DIR_PATH_KEY)
            MadoCertLogDirPath = LastReadValue
            '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_TRANSFER_LIMIT_KEY)
            MadoFaultDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_START_REPLY_LIMIT_KEY)
            MadoFaultDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_RETRY_INTERVAL_KEY)
            MadoFaultDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            MadoFaultDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            MadoFaultDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_TRANSFER_LIMIT_KEY)
            MadoKadoDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_START_REPLY_LIMIT_KEY)
            MadoKadoDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_RETRY_INTERVAL_KEY)
            MadoKadoDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            MadoKadoDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            MadoKadoDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(REGULATION_SECTION, MADO_LOG_MAX_BRANCH_NUMBER_KEY)
            MadoLogMaxBranchNumber = Integer.Parse(LastReadValue)

            ReadFileElem(REGULATION_SECTION, MADO_CERT_LOG_MAX_BRANCH_NUMBER_KEY)
            MadoCertLogMaxBranchNumber = Integer.Parse(LastReadValue)
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
