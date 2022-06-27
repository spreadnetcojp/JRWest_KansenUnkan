' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2013/06/23  (NES)小林  メール送信機能追加
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Net.Mime
Imports JR.ExOpmg.Common

Public Class Config
    Inherits RecServerAppBaseConfig

    '登録対象データ書式ファイルのパス
    Public Shared FormatFilePath As String

    '注意域の異常データに関するSNMP通知用アプリ番号
    Public Shared SnmpAppNumberForWarningFaultOfKanshiban As Integer
    Public Shared SnmpAppNumberForWarningFaultOfGate As Integer
    Public Shared SnmpAppNumberForWarningFaultOfMadosho As Integer
    Public Shared SnmpAppNumberForWarningFaultOfModels As Dictionary(Of String, Integer)

    '危険域の異常データに関するSNMP通知用アプリ番号
    Public Shared SnmpAppNumberForCriticalFaultOfKanshiban As Integer
    Public Shared SnmpAppNumberForCriticalFaultOfGate As Integer
    Public Shared SnmpAppNumberForCriticalFaultOfMadosho As Integer
    Public Shared SnmpAppNumberForCriticalFaultOfModels As Dictionary(Of String, Integer)

    'メール送信をONにする時刻（時）
    Public Shared MailStartHour As Integer

    'メール送信をONにする時刻（分）
    Public Shared MailStartMinute As Integer

    'メール送信をOFFにする時刻（時）
    Public Shared MailEndHour As Integer

    'メール送信をOFFにする時刻（分）
    Public Shared MailEndMinute As Integer

    'メール送信時のSMTPサーバ名
    Public Shared MailSmtpServerName As String

    'メール送信時のSMTPポート番号
    Public Shared MailSmtpPort As Integer

    'メール送信時のSMTPユーザ名
    Public Shared MailSmtpUserName As String

    'メール送信時のSMTPパスワード
    Public Shared MailSmtpPassword As String

    'メール送信時の試行期限
    Public Shared MailSendLimitTicks As Integer

    'メール送信失敗時に既にキューイングされているメールの送信も失敗とするか
    Public Shared MailSendFailureSpreads As Boolean

    'メールに設定するFromアドレス
    Public Shared MailFromAddr As String

    'メールに設定するToアドレス
    Public Shared MailToAddrs As String()

    'メールに設定するCcアドレス
    Public Shared MailCcAddrs As String()

    'メールに設定するBccアドレス
    Public Shared MailBccAddrs As String()

    'メールに設定するSubjectのEncoding
    Public Shared MailSubjectEncoding As String

    'メールに設定する本文のEncoding
    Public Shared MailBodyEncoding As String

    'メールに設定するContent-Transfer-Encoding
    Public Shared MailTransferEncoding As TransferEncoding

    '入力データ別（プロセス別）キーに対するプレフィックス
    Private Const DATA_NAME As String = "FaultData"

    'INIファイル内のセクション名
    Protected Const MAIL_SECTION As String = DATA_NAME & "Mail"

    'INIファイル内における各設定項目のキー
    Private Const SNMP_APP_NUMBER_FOR_KSB_WANING As String = "WarningFaultOfKanshiban"
    Private Const SNMP_APP_NUMBER_FOR_GATE_WANING As String = "WarningFaultOfGate"
    Private Const SNMP_APP_NUMBER_FOR_MADO_WANING As String = "WarningFaultOfMadosho"
    Private Const SNMP_APP_NUMBER_FOR_KSB_CRITICAL As String = "CriticalFaultOfKanshiban"
    Private Const SNMP_APP_NUMBER_FOR_GATE_CRITICAL As String = "CriticalFaultOfGate"
    Private Const SNMP_APP_NUMBER_FOR_MADO_CRITICAL As String = "CriticalFaultOfMadosho"
    Private Const MAIL_START_TIME_KEY As String = "StartTime"
    Private Const MAIL_END_TIME_KEY As String = "EndTime"
    Private Const MAIL_SMTP_SERVER_NAME_KEY As String = "SmtpServerName"
    Private Const MAIL_SMTP_PORT_KEY As String = "SmtpPort"
    Private Const MAIL_SMTP_USER_NAME_KEY As String = "SmtpUserName"
    Private Const MAIL_SMTP_PASSWORD_KEY As String = "SmtpPassword"
    Private Const MAIL_SEND_LIMIT_KEY As String = "SendLimitTicks"
    Private Const MAIL_SEND_FAILURE_SPREADS_KEY As String = "SendFailureSpreads"
    Private Const MAIL_FROM_ADDR_KEY As String = "FromAddr"
    Private Const MAIL_TO_ADDRS_KEY As String = "ToAddrs"
    Private Const MAIL_CC_ADDRS_KEY As String = "CcAddrs"
    Private Const MAIL_BCC_ADDRS_KEY As String = "BccAddrs"
    Private Const MAIL_SUBJECT_ENCODING_KEY As String = "SubjectEncoding"
    Private Const MAIL_BODY_ENCODING_KEY As String = "BodyEncoding"
    Private Const MAIL_TRANSFER_ENCODING_KEY As String = "TransferEncoding"

    ''' <summary>INIファイルから運管サーバの異常データ登録プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        RecServerAppBaseInit(sIniFilePath, DATA_NAME)

        Dim aStrings As String()
        Try
            ReadFileElem(PATH_SECTION, "FaultDataFormatFilePath")
            FormatFilePath = LastReadValue

            ReadFileElem(SNMP_APP_NUMBER_SECTION, SNMP_APP_NUMBER_FOR_KSB_WANING)
            SnmpAppNumberForWarningFaultOfKanshiban = Integer.Parse(LastReadValue)

            ReadFileElem(SNMP_APP_NUMBER_SECTION, SNMP_APP_NUMBER_FOR_GATE_WANING)
            SnmpAppNumberForWarningFaultOfGate = Integer.Parse(LastReadValue)

            ReadFileElem(SNMP_APP_NUMBER_SECTION, SNMP_APP_NUMBER_FOR_MADO_WANING)
            SnmpAppNumberForWarningFaultOfMadosho = Integer.Parse(LastReadValue)

            SnmpAppNumberForWarningFaultOfModels = New Dictionary(Of String, Integer)
            SnmpAppNumberForWarningFaultOfModels.Add(EkConstants.ModelCodeKanshiban, SnmpAppNumberForWarningFaultOfKanshiban)
            SnmpAppNumberForWarningFaultOfModels.Add(EkConstants.ModelCodeGate, SnmpAppNumberForWarningFaultOfGate)
            SnmpAppNumberForWarningFaultOfModels.Add(EkConstants.ModelCodeMadosho, SnmpAppNumberForWarningFaultOfMadosho)

            ReadFileElem(SNMP_APP_NUMBER_SECTION, SNMP_APP_NUMBER_FOR_KSB_CRITICAL)
            SnmpAppNumberForCriticalFaultOfKanshiban = Integer.Parse(LastReadValue)

            ReadFileElem(SNMP_APP_NUMBER_SECTION, SNMP_APP_NUMBER_FOR_GATE_CRITICAL)
            SnmpAppNumberForCriticalFaultOfGate = Integer.Parse(LastReadValue)

            ReadFileElem(SNMP_APP_NUMBER_SECTION, SNMP_APP_NUMBER_FOR_MADO_CRITICAL)
            SnmpAppNumberForCriticalFaultOfMadosho = Integer.Parse(LastReadValue)

            SnmpAppNumberForCriticalFaultOfModels = New Dictionary(Of String, Integer)
            SnmpAppNumberForCriticalFaultOfModels.Add(EkConstants.ModelCodeKanshiban, SnmpAppNumberForCriticalFaultOfKanshiban)
            SnmpAppNumberForCriticalFaultOfModels.Add(EkConstants.ModelCodeGate, SnmpAppNumberForCriticalFaultOfGate)
            SnmpAppNumberForCriticalFaultOfModels.Add(EkConstants.ModelCodeMadosho, SnmpAppNumberForCriticalFaultOfMadosho)

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

            ReadFileElem(MAIL_SECTION, MAIL_SEND_FAILURE_SPREADS_KEY)
            MailSendFailureSpreads = Boolean.Parse(LastReadValue)

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

End Class
