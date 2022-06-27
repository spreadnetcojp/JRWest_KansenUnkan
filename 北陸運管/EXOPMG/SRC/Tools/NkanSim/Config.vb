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

    '�Ώۉw�T�C�o�l�R�[�h
    Public Shared TargetEkCode As EkCode

    '�L�����O���
    Public Shared LogKindsMask As Integer

    '�^�ǃT�[�oIP�A�h���X
    Public Shared ServerIpAddr As String

    '�d���ʐM�p�|�[�g�ԍ�
    Public Shared IpPortForTelegConnection As Integer

    '�d������M�X���b�h��~���e����
    Public Shared TelegrapherPendingLimitTicks As Integer

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

    '�J�Ǘv���d���̉�����M����
    Public Shared ComStartReplyLimitTicks As Integer

    '�v���R�}���h�d���̉�����M����
    Public Shared InquiryReplyLimitTicks As Integer

    'INI�t�@�C�����̃Z�N�V������
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const NETWORK_SECTION As String = "Network"
    Protected Const TIME_INFO_SECTION As String = "TimeInfo"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const SELF_COMPANY_KEY As String = "SelfCompany"
    Private Const SELF_EKCODE_KEY As String = "SelfEkCode"
    Private Const TARGET_EKCODE_KEY As String = "TargetEkCode"
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const SERVER_IP_ADDR_KEY As String = "ServerIpAddr"
    Private Const TELEG_CON_PORT_KEY As String = "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = "TelegrapherPendingLimitTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = "TelegLoggingMaxLengthOnWrite"
    Private Const COM_START_REPLY_LIMIT_KEY As String = "ComStartReplyLimitTicks"
    Private Const INQUIRY_REPLY_LIMIT_KEY As String = "InquiryReplyLimitTicks"

    ''' <summary>INI�t�@�C������m�ԃV�~�����[�^�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Try
            ReadFileElem(CREDENTIAL_SECTION, SELF_COMPANY_KEY)
            SelfCompany = DirectCast([Enum].Parse(GetType(EkCompany), LastReadValue), EkCompany)

            ReadFileElem(CREDENTIAL_SECTION, SELF_EKCODE_KEY)
            SelfEkCode = EkCode.Parse(LastReadValue, "%R-%S")

            ReadFileElem(CREDENTIAL_SECTION, TARGET_EKCODE_KEY)
            TargetEkCode = EkCode.Parse(LastReadValue, "%R-%S")

            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, SERVER_IP_ADDR_KEY)
            ServerIpAddr = LastReadValue

            ReadFileElem(NETWORK_SECTION, TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

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

            ReadFileElem(TIME_INFO_SECTION, COM_START_REPLY_LIMIT_KEY)
            ComStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, INQUIRY_REPLY_LIMIT_KEY)
            InquiryReplyLimitTicks = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
