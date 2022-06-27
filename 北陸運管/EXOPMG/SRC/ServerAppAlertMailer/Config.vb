' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/04/10  (NES)小林  次世代車補対応にて新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Net.Mime
Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '読み出し対象メッセージキューの名前
    Public Shared MyMqPath As String

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

    'INIファイル内のセクション名
    Protected Const MAIL_SECTION As String = "AlertMail"

    'プロセス別キーに対するプレフィックス
    Private Const APP_ID As String = "AlertMailer"

    'INIファイル内における各設定項目のキー
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

    ''' <summary>INIファイルから運管サーバの機器接続状態メール生成プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Try
            ReadFileElem(MQ_SECTION, APP_ID & MQ_PATH_KEY)
            MyMqPath = LastReadValue

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

    Public Shared Sub Dispose()
        ServerAppBaseDispose()
    End Sub

End Class
