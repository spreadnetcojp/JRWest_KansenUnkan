' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Net

Imports JR.ExOpmg.Common

Public Class TelServerAppBaseConfig
    Inherits ServerAppBaseConfig

    '読み出し対象のメッセージキュー
    Public Shared MyMqPath As String

    'FTPサーバルートディレクトリ名
    Public Shared FtpServerRootDirPath As String

    'FTPサーバ内におけるアクセス許可ディレクトリ名
    Public Shared PermittedPathInFtp As String

    'スレッド別テンポラリディレクトリのベース（プロセスのテンポラリディレクトリ）
    Public Shared TemporaryBaseDirPath As String

    '電文通信用リッスンアドレス
    Public Shared IpAddrForTelegConnection As IPAddress

    '電文通信用リッスンポート番号
    Public Shared IpPortForTelegConnection As Integer

    '電文送受信スレッド停止許容時間
    Public Shared TelegrapherPendingLimitTicks As Integer

    'ウォッチドッグシーケンスの間隔
    Public Shared WatchdogIntervalTicks As Integer

    '１電文受信開始～完了の期限（基本時間、0や-1は指定禁止）
    Public Shared TelegReadingLimitBaseTicks As Integer

    '１電文受信開始～完了の期限（メビバイトあたりの追加時間）
    Public Shared TelegReadingLimitExtraTicksPerMiB As Integer

    '１電文書込開始～完了の期限（基本時間、0や-1は指定禁止）
    Public Shared TelegWritingLimitBaseTicks As Integer

    '１電文書込開始～完了の期限（メビバイトあたりの追加時間）
    Public Shared TelegWritingLimitExtraTicksPerMiB As Integer

    '１電文受信あたりのログ保存最大レングス
    Public Shared TelegLoggingMaxLengthOnRead As Integer

    '１電文書込あたりのログ保存最大レングス
    Public Shared TelegLoggingMaxLengthOnWrite As Integer

    'コネクション切断から通信状態変更までの遅延時間
    Public Shared PseudoConnectionProlongationTicks As Integer

    '収集データ誤記テーブルに対する通信異常登録をONにする時刻（00時00分からの経過分）
    Public Shared LineErrorRecordingStartMinutesInDay As Integer

    '収集データ誤記テーブルに対する通信異常登録をOFFにする時刻（00時00分からの経過分）（StartMinutesInDay以上に補正済み）
    Public Shared LineErrorRecordingEndMinutesInDay As Integer

    '収集データ誤記テーブルに対する通信異常の重複登録禁止期間（これを0以下にすれば、登録は行わない）
    Public Shared LineErrorRecordingIntervalTicks As Integer

    '-------Ver0.1 次世代車補対応 ADD START-----------
    '通信異常の警報メール生成をONにする時刻（00時00分からの経過分）
    Public Shared LineErrorAlertingStartMinutesInDay As Integer

    '通信異常の警報メール生成をOFFにする時刻（00時00分からの経過分）（StartMinutesInDay以上に補正済み）
    Public Shared LineErrorAlertingEndMinutesInDay As Integer

    '通信異常の警報メールの重複生成禁止期間（これを0以下にすれば、生成は行わない）
    Public Shared LineErrorAlertingIntervalTicks As Integer
    '-------Ver0.1 次世代車補対応 ADD END-------------

    '通信異常とみなさない運管側ポートオープンから接続初期化シーケンス完了までの期限
    Public Shared InitialConnectLimitTicksForLineError As Integer

    'ウォッチドッグシーケンスにおける応答電文受信期限
    Public Shared WatchdogReplyLimitTicks As Integer

    'ファイル転送シーケンス排他増強モード設定
    Public Shared EnableXllStrongExclusion As Boolean

    '能動的シーケンス排他増強モード設定
    Public Shared EnableActiveSeqStrongExclusion As Boolean

    '能動的単発シーケンス順序強制モード設定
    Public Shared EnableActiveOneOrdering As Boolean

    'マスタ/プログラムDLL同時実行最大クライアント数
    Public Shared ConcurrentMasProDllMaxCount As Integer

    '指定ファイルULL同時実行最大クライアント数
    Public Shared ConcurrentScheduledUllMaxCount As Integer

    '通信状態に関するSNMP通知用アプリ番号（0の場合は通知しない）
    Public Shared SnmpAppNumberForConnectionStatus As Integer

    'INIファイル内における各設定項目のキー
    Private Const FTP_SERVER_ROOT_DIR_PATH_KEY As String = "FtpServerRootDirPath"
    Private Const PERMITTED_PATH_IN_FTP_KEY As String = "PermittedPathInFtp"
    Private Const TEMP_BASE_DIR_PATH_KEY As String = "TemporaryBaseDirPath"
    Private Const SELF_ADDR_KEY As String = "SelfAddr"
    Private Const TELEG_CON_PORT_KEY As String = "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = "TelegrapherPendingLimitTicks"
    Private Const WATCHDOG_INTERVAL_KEY As String = "WatchdogIntervalTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = "TelegLoggingMaxLengthOnWrite"
    Private Const PSEUDO_CONNECTION_PROLONGATION_KEY As String = "PseudoConnectionProlongationTicks"
    Private Const LINE_ERROR_RECORDING_START_TIME_KEY As String = "LineErrorRecordingStartTime"
    Private Const LINE_ERROR_RECORDING_END_TIME_KEY As String = "LineErrorRecordingEndTime"
    Private Const LINE_ERROR_RECORDING_INTERVAL_KEY As String = "LineErrorRecordingIntervalTicks"
    '-------Ver0.1 次世代車補対応 ADD START-----------
    Private Const LINE_ERROR_ALERTING_START_TIME_KEY As String = "LineErrorAlertingStartTime"
    Private Const LINE_ERROR_ALERTING_END_TIME_KEY As String = "LineErrorAlertingEndTime"
    Private Const LINE_ERROR_ALERTING_INTERVAL_KEY As String = "LineErrorAlertingIntervalTicks"
    '-------Ver0.1 次世代車補対応 ADD END-------------
    Private Const INITIAL_CONNECT_LIMIT_FOR_LINE_ERROR_KEY As String = "InitialConnectLimitTicksForLineError"
    Private Const WATCHDOG_REPLY_LIMIT_KEY As String = "WatchdogReplyLimitTicks"
    Private Const ENABLE_XLL_STRONG_EXCLUSION_KEY As String = "EnableXllStrongExclusion"
    Private Const ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY As String = "EnableActiveSeqStrongExclusion"
    Private Const ENABLE_ACTIVE_ONE_ORDERING_KEY As String = "EnableActiveOneOrdering"
    Private Const CONCURRENT_MASPRO_DLL_MAX_COUNT_KEY As String = "ConcurrentMasProDllMaxCount"
    Private Const CONCURRENT_SCHEDULED_ULL_MAX_COUNT_KEY As String = "ConcurrentScheduledUllMaxCount"
    Private Const SNMP_APP_NUMBER_FOR_CONNECTION_STATUS_KEY As String = "ConnectionStatus"

    ''' <summary>INIファイルから運管サーバの通信系プロセスに必須の設定値を取り込む。</summary>
    Public Shared Sub TelServerAppBaseInit(ByVal sIniFilePath As String, ByVal sModelName As String, Optional ByVal needInfoOfOtherApps As Boolean = False)
        Dim sAppIdentifier As String = "To" & sModelName
        ServerAppBaseInit(sIniFilePath, sAppIdentifier, needInfoOfOtherApps)

        Dim aStrings As String()
        Try
            ReadFileElem(MQ_SECTION, sAppIdentifier & MQ_PATH_KEY)
            MyMqPath = LastReadValue

            ReadFileElem(PATH_SECTION, sAppIdentifier & FTP_SERVER_ROOT_DIR_PATH_KEY)
            FtpServerRootDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, sAppIdentifier & PERMITTED_PATH_IN_FTP_KEY)
            PermittedPathInFtp = LastReadValue

            ReadFileElem(PATH_SECTION, sAppIdentifier & TEMP_BASE_DIR_PATH_KEY)
            TemporaryBaseDirPath = LastReadValue

            ReadFileElem(NETWORK_SECTION, SELF_ADDR_KEY)
            IpAddrForTelegConnection = IPAddress.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, sAppIdentifier & TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & WATCHDOG_INTERVAL_KEY)
            WatchdogIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEG_READING_LIMIT_BASE_KEY)
            TelegReadingLimitBaseTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEG_READING_LIMIT_EXTRA_KEY)
            TelegReadingLimitExtraTicksPerMiB = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEG_WRITING_LIMIT_BASE_KEY)
            TelegWritingLimitBaseTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & TELEG_WRITING_LIMIT_EXTRA_KEY)
            TelegWritingLimitExtraTicksPerMiB = Integer.Parse(LastReadValue)

            ReadFileElem(LOGGING_SECTION, sAppIdentifier & TELEG_LOGGING_MAX_ON_READ_KEY)
            TelegLoggingMaxLengthOnRead = Integer.Parse(LastReadValue)

            ReadFileElem(LOGGING_SECTION, sAppIdentifier & TELEG_LOGGING_MAX_ON_WRITE_KEY)
            TelegLoggingMaxLengthOnWrite = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & PSEUDO_CONNECTION_PROLONGATION_KEY)
            PseudoConnectionProlongationTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_RECORDING_START_TIME_KEY)
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

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_RECORDING_END_TIME_KEY)
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

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_RECORDING_INTERVAL_KEY)
            LineErrorRecordingIntervalTicks = Integer.Parse(LastReadValue)

            '-------Ver0.1 次世代車補対応 ADD START-----------
            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_ALERTING_START_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorAlertingStartHour As Integer = Integer.Parse(aStrings(0))
            If lineErrorAlertingStartHour < 0 OrElse lineErrorAlertingStartHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorAlertingStartMinute As Integer = Integer.Parse(aStrings(1))
            If lineErrorAlertingStartMinute < 0 OrElse lineErrorAlertingStartMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            LineErrorAlertingStartMinutesInDay = lineErrorAlertingStartHour * 60 + lineErrorAlertingStartMinute

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_ALERTING_END_TIME_KEY)
            aStrings = LastReadValue.Split(":"c)
            If aStrings.Length <> 2 Then
                Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorAlertingEndHour As Integer = Integer.Parse(aStrings(0))
            If lineErrorAlertingEndHour < 0 OrElse lineErrorAlertingEndHour > 23 Then
                Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            Dim lineErrorAlertingEndMinute As Integer = Integer.Parse(aStrings(1))
            If lineErrorAlertingEndMinute < 0 OrElse lineErrorAlertingEndMinute > 59 Then
                Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If
            LineErrorAlertingEndMinutesInDay = lineErrorAlertingEndHour * 60 + lineErrorAlertingEndMinute

            'StartMinutesInDay <= EndMinutesInDayになるよう、
            '必要に応じてEndMinutesInDayには補正をかけておく。
            'NOTE: StartMinutesInDay == EndMinutesInDayは正当な設定
            'であり、有効時間帯がその１分間だけであることを意味する。
            If LineErrorAlertingStartMinutesInDay > LineErrorAlertingEndMinutesInDay Then
                LineErrorAlertingEndMinutesInDay += 24 * 60
            End If

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & LINE_ERROR_ALERTING_INTERVAL_KEY)
            LineErrorAlertingIntervalTicks = Integer.Parse(LastReadValue)
            '-------Ver0.1 次世代車補対応 ADD END-------------

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & INITIAL_CONNECT_LIMIT_FOR_LINE_ERROR_KEY)
            InitialConnectLimitTicksForLineError = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & WATCHDOG_REPLY_LIMIT_KEY)
            WatchdogReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & ENABLE_XLL_STRONG_EXCLUSION_KEY)
            EnableXllStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY)
            EnableActiveSeqStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & ENABLE_ACTIVE_ONE_ORDERING_KEY)
            EnableActiveOneOrdering = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & CONCURRENT_MASPRO_DLL_MAX_COUNT_KEY)
            ConcurrentMasProDllMaxCount = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, sAppIdentifier & CONCURRENT_SCHEDULED_ULL_MAX_COUNT_KEY)
            ConcurrentScheduledUllMaxCount = Integer.Parse(LastReadValue)

            ReadFileElem(SNMP_APP_NUMBER_SECTION, sAppIdentifier & SNMP_APP_NUMBER_FOR_CONNECTION_STATUS_KEY)
            SnmpAppNumberForConnectionStatus = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    Public Shared Sub TelServerAppBaseDispose()
        ServerAppBaseDispose()
    End Sub

End Class
