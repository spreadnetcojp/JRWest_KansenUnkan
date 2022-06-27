' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Net

Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '読み出し対象のメッセージキュー
    Public Shared MyMqPath As String

    '電文通信用リッスンアドレス
    Public Shared IpAddrForTelegConnection As IPAddress

    '電文送受信スレッド停止許容時間
    Public Shared TelegrapherPendingLimitTicks As Integer

    '１電文受信開始～完了の期限（基本時間、0や-1は指定禁止）
    Public Shared TelegReadingLimitBaseTicks As Integer

    '１電文受信開始～完了の期限（メビバイトあたりの追加時間）
    Public Shared TelegReadingLimitExtraTicksPerMiB As Integer

    '１電文書込開始～完了の期限（0や-1は無期限）
    Public Shared TelegWritingLimitBaseTicks As Integer

    '１電文書込開始～完了の期限（メビバイトあたりの追加時間）
    Public Shared TelegWritingLimitExtraTicksPerMiB As Integer

    '１電文受信あたりのログ保存最大レングス
    Public Shared TelegLoggingMaxLengthOnRead As Integer

    '１電文書込あたりのログ保存最大レングス
    Public Shared TelegLoggingMaxLengthOnWrite As Integer

    '収集データ誤記テーブルに対する通信異常登録をONにする時刻（00時00分からの経過分）
    Public Shared LineErrorRecordingStartMinutesInDay As Integer

    '収集データ誤記テーブルに対する通信異常登録をOFFにする時刻（00時00分からの経過分）（StartMinutesInDay以上に補正済み）
    Public Shared LineErrorRecordingEndMinutesInDay As Integer

    '収集データ誤記テーブルに対する通信異常の重複登録禁止期間
    Public Shared LineErrorRecordingIntervalTicks As Integer

    '収集データ誤記テーブルに通信異常を登録することになるポートオープンから開局シーケンス完了までの期限
    Public Shared InitialConnectLimitTicksForLineError As Integer

    '利用データ応答電文受信期限
    Public Shared RiyoDataReplyLimitTicks As Integer

    '締切件数データ応答電文受信期限
    Public Shared SummaryDataReplyLimitTicks As Integer

    'プロセス別キーに対するプレフィックス
    Private Const APP_ID As String = "ToNkan"

    'INIファイル内における各設定項目のキー
    Private Const SELF_ADDR_KEY As String = "SelfAddr"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = APP_ID & "TelegrapherPendingLimitTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = APP_ID & "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = APP_ID & "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = APP_ID & "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = APP_ID & "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = APP_ID & "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = APP_ID & "TelegLoggingMaxLengthOnWrite"
    Private Const LINE_ERROR_RECORDING_START_TIME_KEY As String = APP_ID & "LineErrorRecordingStartTime"
    Private Const LINE_ERROR_RECORDING_END_TIME_KEY As String = APP_ID & "LineErrorRecordingEndTime"
    Private Const LINE_ERROR_RECORDING_INTERVAL_KEY As String = APP_ID & "LineErrorRecordingIntervalTicks"
    Private Const INITIAL_CONNECT_LIMIT_FOR_LINE_ERROR_KEY As String = APP_ID & "InitialConnectLimitTicksForLineError"
    Private Const RIYO_DATA_REPLY_LIMIT_KEY As String = APP_ID & "RiyoDataReplyLimitTicks"
    Private Const SUMMARY_DATA_REPLY_LIMIT_KEY As String = APP_ID & "SummaryDataReplyLimitTicks"

    ''' <summary>INIファイルから運管サーバの対Ｎ間通信プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Dim aStrings As String()
        Try
            ReadFileElem(MQ_SECTION, APP_ID & MQ_PATH_KEY)
            MyMqPath = LastReadValue

            ReadFileElem(NETWORK_SECTION, SELF_ADDR_KEY)
            IpAddrForTelegConnection = IPAddress.Parse(LastReadValue)

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

            ReadFileElem(TIME_INFO_SECTION, LINE_ERROR_RECORDING_START_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorRecordingStartHour As Integer = Integer.Parse(aStrings(0))
            If lineErrorRecordingStartHour < 0 OrElse lineErrorRecordingStartHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorRecordingStartMinute As Integer = Integer.Parse(aStrings(1))
            If lineErrorRecordingStartMinute < 0 OrElse lineErrorRecordingStartMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            LineErrorRecordingStartMinutesInDay = lineErrorRecordingStartHour * 60 + lineErrorRecordingStartMinute

            ReadFileElem(TIME_INFO_SECTION, LINE_ERROR_RECORDING_END_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorRecordingEndHour As Integer = Integer.Parse(aStrings(0))
            If lineErrorRecordingEndHour < 0 OrElse lineErrorRecordingEndHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorRecordingEndMinute As Integer = Integer.Parse(aStrings(1))
            If lineErrorRecordingEndMinute < 0 OrElse lineErrorRecordingEndMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            LineErrorRecordingEndMinutesInDay = lineErrorRecordingEndHour * 60 + lineErrorRecordingEndMinute

            'StartMinutesInDay <= EndMinutesInDayになるよう、
            '必要に応じてEndMinutesInDayには補正をかけておく。
            'NOTE: StartMinutesInDay == EndMinutesInDayは正当な設定
            'であり、有効時間帯がその１分間だけであることを意味する。
            If LineErrorRecordingStartMinutesInDay > LineErrorRecordingEndMinutesInDay Then
                LineErrorRecordingEndMinutesInDay += 24 * 60
            End If

            ReadFileElem(TIME_INFO_SECTION, LINE_ERROR_RECORDING_INTERVAL_KEY)
            LineErrorRecordingIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, INITIAL_CONNECT_LIMIT_FOR_LINE_ERROR_KEY)
            InitialConnectLimitTicksForLineError = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, RIYO_DATA_REPLY_LIMIT_KEY)
            RiyoDataReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, SUMMARY_DATA_REPLY_LIMIT_KEY)
            SummaryDataReplyLimitTicks = Integer.Parse(LastReadValue)
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
