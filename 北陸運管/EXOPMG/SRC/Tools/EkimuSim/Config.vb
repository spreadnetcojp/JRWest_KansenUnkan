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

Imports JR.ExOpmg.Common

'TODO: アプリの動作中に変更したいものは、ここから除去し、MainForm.vbの
'UiStateClassに宣言する。

Public Class Config
    Inherits BaseConfig

    '事業者
    Public Shared SelfCompany As EkCompany

    '自装置サイバネコード
    Public Shared SelfEkCode As EkCode

    'FTPユーザ名
    Public Shared FtpUserName As String

    'FTPパスワード
    Public Shared FtpPassword As String

    '有効ログ種別
    Public Shared LogKindsMask As Integer

    'デフォルト送信ファイル格納ディレクトリ
    Public Shared DefaultApplyDataDirPath As String

    'FTPサーバ内における機種別ディレクトリ名
    Public Shared ModelPathInFtp As String

    'FTPサーバURI
    Public Shared FtpServerUri As String

    '運管サーバIPアドレス
    Public Shared ServerIpAddr As String

    '電文通信用ポート番号
    Public Shared IpPortForTelegConnection As Integer

    '電文送受信スレッド停止許容時間
    Public Shared TelegrapherPendingLimitTicks As Integer

    'ウォッチドッグシーケンスの最大許容間隔
    Public Shared WatchdogIntervalLimitTicks As Integer

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

    '接続初期化要求電文の応答受信期限
    Public Shared ComStartReplyLimitTicks As Integer

    '整時データ取得要求電文の応答受信期限
    Public Shared TimeDataGetReplyLimitTicks As Integer

    'アプリ層（セッション層）プロトコル
    Public Shared AplProtocol As EkAplProtocol

    'ウォッチドッグによる回線状態監視の要否
    Public Shared EnableWatchdog As Boolean

    'ファイル転送シーケンス排他増強モード設定
    Public Shared EnableXllStrongExclusion As Boolean

    '能動的シーケンス排他増強モード設定
    Public Shared EnableActiveSeqStrongExclusion As Boolean

    '能動的単発シーケンス順序強制モード設定
    Public Shared EnableActiveOneOrdering As Boolean

    '能動シーケンス用FTPスレッドの停止許容時間
    Public Shared ActiveFtpWorkerPendingLimitTicks As Integer

    '能動シーケンス用FTPの各種リクエストに対する応答受信期限
    Public Shared ActiveFtpRequestLimitTicks As Integer

    '能動シーケンス用FTPのログアウトのリクエストに対する応答受信期限
    Public Shared ActiveFtpLogoutLimitTicks As Integer

    '能動シーケンス用FTPで異常と判定するデータ転送停止時間（-1は無期限）
    Public Shared ActiveFtpTransferStallLimitTicks As Integer

    '能動シーケンス用FTPでパッシブモードを使うか否か
    Public Shared ActiveFtpUsePassiveMode As Boolean

    '能動シーケンス用FTPで転送を行うごとにログアウトするか否か
    Public Shared ActiveFtpLogoutEachTime As Boolean

    '能動シーケンス用FTPで使用するバッファの容量
    Public Shared ActiveFtpBufferLength As Integer

    '受動シーケンス用FTPスレッドの停止許容時間
    Public Shared PassiveFtpWorkerPendingLimitTicks As Integer

    '受動シーケンス用FTPの各種リクエストに対する応答受信期限
    Public Shared PassiveFtpRequestLimitTicks As Integer

    '受動シーケンス用FTPのログアウトのリクエストに対する応答受信期限
    Public Shared PassiveFtpLogoutLimitTicks As Integer

    '受動シーケンス用FTPで異常と判定するデータ転送停止時間（-1は無期限）
    Public Shared PassiveFtpTransferStallLimitTicks As Integer

    '受動シーケンス用FTPでパッシブモードを使うか否か
    Public Shared PassiveFtpUsePassiveMode As Boolean

    '受動シーケンス用FTPで転送を行うごとにログアウトするか否か
    Public Shared PassiveFtpLogoutEachTime As Boolean

    '受動シーケンス用FTPで使用するバッファの容量
    Public Shared PassiveFtpBufferLength As Integer

    'INIファイル内のセクション名
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const PATH_SECTION As String = "Path"
    Protected Const NETWORK_SECTION As String = "Network"
    Protected Const TIME_INFO_SECTION As String = "TimeInfo"
    Protected Const TELEGRAPHER_MODE_SECTION As String = "TelegrapherMode"

    'INIファイル内における各設定項目のキー
    Private Const SELF_COMPANY_KEY As String = "SelfCompany"
    Private Const SELF_EKCODE_KEY As String = "SelfEkCode"
    Private Const FTP_USER_NAME_KEY As String = "FtpUserName"
    Private Const FTP_PASSWORD_KEY As String = "FtpPassword"
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const DEFAULT_APPLY_DATA_DIR_PATH_KEY As String = "DefaultApplyDataDirPath"
    Private Const MODEL_PATH_IN_FTP_KEY As String = "ModelPathInFtp"
    Private Const FTP_SERVER_URI_KEY As String = "FtpServerUri"
    Private Const SERVER_IP_ADDR_KEY As String = "ServerIpAddr"
    Private Const TELEG_CON_PORT_KEY As String = "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = "TelegrapherPendingLimitTicks"
    Private Const WATCHDOG_INTERVAL_LIMIT_KEY As String = "WatchdogIntervalLimitTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = "TelegLoggingMaxLengthOnWrite"
    Private Const COM_START_REPLY_LIMIT_KEY As String = "ComStartReplyLimitTicks"
    Private Const TIME_DATA_GET_REPLY_LIMIT_KEY As String = "TimeDataGetReplyLimitTicks"
    Private Const APL_PROTOCOL_KEY As String = "AplProtocol"
    Private Const ENABLE_WATCHDOG_KEY As String = "EnableWatchdog"
    Private Const ENABLE_XLL_STRONG_EXCLUSION_KEY As String = "EnableXllStrongExclusion"
    Private Const ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY As String = "EnableActiveSeqStrongExclusion"
    Private Const ENABLE_ACTIVE_ONE_ORDERING_KEY As String = "EnableActiveOneOrdering"
    Private Const ACTIVE_FTP_WORKER_PENDING_LIMIT_TICKS_KEY As String = "ActiveFtpWorkerPendingLimitTicks"
    Private Const ACTIVE_FTP_REQUEST_LIMIT_TICKS_KEY As String = "ActiveFtpRequestLimitTicks"
    Private Const ACTIVE_FTP_LOGOUT_LIMIT_TICKS_KEY As String = "ActiveFtpLogoutLimitTicks"
    Private Const ACTIVE_FTP_TRANSFER_STALL_LIMIT_TICKS_KEY As String = "ActiveFtpTransferStallLimitTicks"
    Private Const ACTIVE_FTP_USE_PASSIVE_MODE_KEY As String = "ActiveFtpUsePassiveMode"
    Private Const ACTIVE_FTP_LOGOUT_EACH_TIME_KEY As String = "ActiveFtpLogoutEachTime"
    Private Const ACTIVE_FTP_BUFFER_LENGTH_KEY As String = "ActiveFtpBufferLength"
    Private Const PASSIVE_FTP_WORKER_PENDING_LIMIT_TICKS_KEY As String = "PassiveFtpWorkerPendingLimitTicks"
    Private Const PASSIVE_FTP_REQUEST_LIMIT_TICKS_KEY As String = "PassiveFtpRequestLimitTicks"
    Private Const PASSIVE_FTP_LOGOUT_LIMIT_TICKS_KEY As String = "PassiveFtpLogoutLimitTicks"
    Private Const PASSIVE_FTP_TRANSFER_STALL_LIMIT_TICKS_KEY As String = "PassiveFtpTransferStallLimitTicks"
    Private Const PASSIVE_FTP_USE_PASSIVE_MODE_KEY As String = "PassiveFtpUsePassiveMode"
    Private Const PASSIVE_FTP_LOGOUT_EACH_TIME_KEY As String = "PassiveFtpLogoutEachTime"
    Private Const PASSIVE_FTP_BUFFER_LENGTH_KEY As String = "PassiveFtpBufferLength"

    ''' <summary>INIファイルから駅務機器シミュレータに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Try
            ReadFileElem(CREDENTIAL_SECTION, SELF_COMPANY_KEY)
            SelfCompany = DirectCast([Enum].Parse(GetType(EkCompany), LastReadValue), EkCompany)

            ReadFileElem(CREDENTIAL_SECTION, SELF_EKCODE_KEY)
            SelfEkCode = EkCode.Parse(LastReadValue, "%M-%R-%S-%C-%U")

            ReadFileElem(CREDENTIAL_SECTION, FTP_USER_NAME_KEY)
            FtpUserName = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, FTP_PASSWORD_KEY)
            FtpPassword = LastReadValue

            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(PATH_SECTION, DEFAULT_APPLY_DATA_DIR_PATH_KEY)
            DefaultApplyDataDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, MODEL_PATH_IN_FTP_KEY)
            ModelPathInFtp = LastReadValue

            ReadFileElem(NETWORK_SECTION, FTP_SERVER_URI_KEY)
            FtpServerUri = LastReadValue

            ReadFileElem(NETWORK_SECTION, SERVER_IP_ADDR_KEY)
            ServerIpAddr = LastReadValue

            ReadFileElem(NETWORK_SECTION, TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, WATCHDOG_INTERVAL_LIMIT_KEY)
            WatchdogIntervalLimitTicks = Integer.Parse(LastReadValue)

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

            ReadFileElem(TIME_INFO_SECTION, TIME_DATA_GET_REPLY_LIMIT_KEY)
            TimeDataGetReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, APL_PROTOCOL_KEY)
            AplProtocol = DirectCast([Enum].Parse(GetType(EkAplProtocol), LastReadValue), EkAplProtocol)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_WATCHDOG_KEY)
            EnableWatchdog = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_XLL_STRONG_EXCLUSION_KEY)
            EnableXllStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY)
            EnableActiveSeqStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_ONE_ORDERING_KEY)
            EnableActiveOneOrdering = Boolean.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, ACTIVE_FTP_WORKER_PENDING_LIMIT_TICKS_KEY)
            ActiveFtpWorkerPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, ACTIVE_FTP_REQUEST_LIMIT_TICKS_KEY)
            ActiveFtpRequestLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, ACTIVE_FTP_LOGOUT_LIMIT_TICKS_KEY)
            ActiveFtpLogoutLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, ACTIVE_FTP_TRANSFER_STALL_LIMIT_TICKS_KEY)
            ActiveFtpTransferStallLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ACTIVE_FTP_USE_PASSIVE_MODE_KEY)
            ActiveFtpUsePassiveMode = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ACTIVE_FTP_LOGOUT_EACH_TIME_KEY)
            ActiveFtpLogoutEachTime = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ACTIVE_FTP_BUFFER_LENGTH_KEY)
            ActiveFtpBufferLength = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, PASSIVE_FTP_WORKER_PENDING_LIMIT_TICKS_KEY)
            PassiveFtpWorkerPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, PASSIVE_FTP_REQUEST_LIMIT_TICKS_KEY)
            PassiveFtpRequestLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, PASSIVE_FTP_LOGOUT_LIMIT_TICKS_KEY)
            PassiveFtpLogoutLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, PASSIVE_FTP_TRANSFER_STALL_LIMIT_TICKS_KEY)
            PassiveFtpTransferStallLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, PASSIVE_FTP_USE_PASSIVE_MODE_KEY)
            PassiveFtpUsePassiveMode = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, PASSIVE_FTP_LOGOUT_EACH_TIME_KEY)
            PassiveFtpLogoutEachTime = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, PASSIVE_FTP_BUFFER_LENGTH_KEY)
            PassiveFtpBufferLength = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
