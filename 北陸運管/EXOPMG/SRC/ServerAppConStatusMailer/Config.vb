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

Imports System.Net.Mime
Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '���D�@�ڑ���Ԃ�M����������i0��-1�͖������j
    Public Shared GateConStatusTrustLimitTicks As Integer

    '�����ڑ���Ԃ�M����������i0��-1�͖������j
    Public Shared MadoConStatusTrustLimitTicks As Integer

    '���[�����M��ON�ɂ��鎞���i���j
    Public Shared MailStartHour As Integer

    '���[�����M��ON�ɂ��鎞���i���j
    Public Shared MailStartMinute As Integer

    '���[�����M��OFF�ɂ��鎞���i���j
    Public Shared MailEndHour As Integer

    '���[�����M��OFF�ɂ��鎞���i���j
    Public Shared MailEndMinute As Integer

    '���[�����M�̎����i���j
    Public Shared MailSendCycle As Integer

    '���[�����M���s�̒x�����ԁi�����␳�΍��p�j
    Public Shared MailSendDelayTicks As Integer

    '���[�����M����SMTP�T�[�o��
    Public Shared MailSmtpServerName As String

    '���[�����M����SMTP�|�[�g�ԍ�
    Public Shared MailSmtpPort As Integer

    '���[�����M����SMTP���[�U��
    Public Shared MailSmtpUserName As String

    '���[�����M����SMTP�p�X���[�h
    Public Shared MailSmtpPassword As String

    '���[�����M���̎��s����
    Public Shared MailSendLimitTicks As Integer

    '���[���ɐݒ肷��From�A�h���X
    Public Shared MailFromAddr As String

    '���[���ɐݒ肷��To�A�h���X
    Public Shared MailToAddrs As String()

    '���[���ɐݒ肷��Cc�A�h���X
    Public Shared MailCcAddrs As String()

    '���[���ɐݒ肷��Bcc�A�h���X
    Public Shared MailBccAddrs As String()

    '���[���ɐݒ肷��Subject��Encoding
    Public Shared MailSubjectEncoding As String

    '���[���ɐݒ肷��{����Encoding
    Public Shared MailBodyEncoding As String

    '���[���ɐݒ肷��Content-Transfer-Encoding
    Public Shared MailTransferEncoding As TransferEncoding

    'INI�t�@�C�����̃Z�N�V������
    Protected Const MAIL_SECTION As String = "ConStatusMail"

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const APP_ID As String = "ConStatusMailer"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const GATE_CON_STATUS_TRUST_LIMIT_KEY As String = "GateConStatusTrustLimitTicks"
    Private Const MADO_CON_STATUS_TRUST_LIMIT_KEY As String = "MadoConStatusTrustLimitTicks"
    Private Const MAIL_START_TIME_KEY As String = "StartTime"
    Private Const MAIL_END_TIME_KEY As String = "EndTime"
    Private Const MAIL_SEND_CYCLE_KEY As String = "SendCycle"
    Private Const MAIL_SEND_DELAY_KEY As String = "SendDelayTicks"
    Private Const MAIL_SMTP_SERVER_NAME_KEY As String = "SmtpServerName"
    Private Const MAIL_SMTP_PORT_KEY As String = "SmtpPort"
    Private Const MAIL_SMTP_USER_NAME_KEY As String = "SmtpUserName"
    Private Const MAIL_SMTP_PASSWORD_KEY As String = "SmtpPassword"
    Private Const MAIL_SEND_LIMIT_KEY As String = "SendLimitTicks"
    Private Const MAIL_FROM_ADDR_KEY As String = "FromAddr"
    Private Const MAIL_TO_ADDRS_KEY As String = "ToAddrs"
    Private Const MAIL_CC_ADDRS_KEY As String = "CcAddrs"
    Private Const MAIL_BCC_ADDRS_KEY As String = "BccAddrs"
    Private Const MAIL_SUBJECT_ENCODING_KEY As String = "SubjectEncoding"
    Private Const MAIL_BODY_ENCODING_KEY As String = "BodyEncoding"
    Private Const MAIL_TRANSFER_ENCODING_KEY As String = "TransferEncoding"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̋@��ڑ���ԃ��[�������v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Dim aStrings As String()
        Try
            ReadFileElem(TIME_INFO_SECTION, GATE_CON_STATUS_TRUST_LIMIT_KEY)
            GateConStatusTrustLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_CON_STATUS_TRUST_LIMIT_KEY)
            MadoConStatusTrustLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(MAIL_SECTION, MAIL_START_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            MailStartHour = Integer.Parse(aStrings(0))
            If MailStartHour < 0 OrElse MailStartHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            MailStartMinute = Integer.Parse(aStrings(1))
            If MailStartMinute < 0 OrElse MailStartMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If

            ReadFileElem(MAIL_SECTION, MAIL_END_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            MailEndHour = Integer.Parse(aStrings(0))
            If MailEndHour < 0 OrElse MailEndHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            MailEndMinute = Integer.Parse(aStrings(1))
            If MailEndMinute < 0 OrElse MailEndMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If

            ReadFileElem(MAIL_SECTION, MAIL_SEND_CYCLE_KEY)
            MailSendCycle = Integer.Parse(LastReadValue)
            If MailSendCycle < 1 OrElse MailSendCycle > 24 * 60 Then
                Throw New OPMGException("The value must be within the range 1 and 1440. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If

            ReadFileElem(MAIL_SECTION, MAIL_SEND_DELAY_KEY)
            MailSendDelayTicks = Integer.Parse(LastReadValue)

            ReadFileElem(MAIL_SECTION, MAIL_SMTP_SERVER_NAME_KEY)
            MailSmtpServerName = LastReadValue

            ReadFileElem(MAIL_SECTION, MAIL_SMTP_PORT_KEY)
            MailSmtpPort = Integer.Parse(LastReadValue)

            ReadFileElem(MAIL_SECTION, MAIL_SMTP_USER_NAME_KEY)
            MailSmtpUserName = LastReadValue

            ReadFileElem(MAIL_SECTION, MAIL_SMTP_PASSWORD_KEY)
            MailSmtpPassword = LastReadValue

            ReadFileElem(MAIL_SECTION, MAIL_SEND_LIMIT_KEY)
            MailSendLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(MAIL_SECTION, MAIL_FROM_ADDR_KEY)
            MailFromAddr = LastReadValue

            ReadFileElem(MAIL_SECTION, MAIL_TO_ADDRS_KEY)
            MailToAddrs = LastReadValue.Split(","c)

            ReadFileElem(MAIL_SECTION, MAIL_CC_ADDRS_KEY)
            MailCcAddrs = LastReadValue.Split(","c)

            ReadFileElem(MAIL_SECTION, MAIL_BCC_ADDRS_KEY)
            MailBccAddrs = LastReadValue.Split(","c)

            ReadFileElem(MAIL_SECTION, MAIL_SUBJECT_ENCODING_KEY)
            MailSubjectEncoding = LastReadValue

            ReadFileElem(MAIL_SECTION, MAIL_BODY_ENCODING_KEY)
            MailBodyEncoding = LastReadValue

            ReadFileElem(MAIL_SECTION, MAIL_TRANSFER_ENCODING_KEY)
            MailTransferEncoding = DirectCast([Enum].Parse(GetType(TransferEncoding), LastReadValue), TransferEncoding)
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
