' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/05/13  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits BaseConfig

    '有効ログ種別
    Public Shared LogKindsMask As Integer

    '背景色
    Public Shared BackgroundColor As System.Drawing.Color

    'ボタン色
    Public Shared ButtonColor As System.Drawing.Color

    'FTPユーザ名
    Public Shared FtpUserName As String

    'FTPパスワード
    Public Shared FtpPassword As String

    'FTPワーキングディレクトリ名
    Public Shared FtpWorkingDirPath As String

    'FTPサーバ内におけるアクセス許可ディレクトリ名
    Public Shared PermittedPathInFtp As String

    'FTPサーバURI
    Public Shared FtpServerUri As String

    '運管サーバIPアドレス
    Public Shared ServerIpAddr As String

    '電文通信用ポート番号
    Public Shared IpPortForTelegConnection As Integer

    '電文送受信スレッド停止許容時間
    Public Shared TelegrapherPendingLimitTicks As Integer

    '電文送受信スレッドUll実行許容時間（0や-1は無期限）
    Public Shared TelegrapherUllLimitTicks As Integer

    '電文送受信スレッド配信指示実行許容時間（0や-1は無期限）
    Public Shared TelegrapherDllInvokeLimitTicks As Integer

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

    '配信指示電文の応答受信期限
    Public Shared MasProDllInvokeReplyLimitTicks As Integer

    'マスタ/プログラムULLにおける最大転送時間（0や-1は無期限）
    Public Shared MasProUllTransferLimitTicks As Integer

    'マスタ/プログラムULLにおける開始電文の応答受信期限
    Public Shared MasProUllStartReplyLimitTicks As Integer

    'マスタ/プログラムULLにおける終了電文の応答受信期限
    Public Shared MasProUllFinishReplyLimitTicks As Integer

    'ウォッチドッグによる回線状態監視の要否
    Public Shared EnableWatchdog As Boolean

    'ファイル転送シーケンス排他増強モード設定
    Public Shared EnableXllStrongExclusion As Boolean

    '能動的シーケンス排他増強モード設定
    Public Shared EnableActiveSeqStrongExclusion As Boolean

    '能動的単発シーケンス順序強制モード設定
    Public Shared EnableActiveOneOrdering As Boolean

    'FTPスレッドの停止許容時間
    Public Shared FtpWorkerPendingLimitTicks As Integer

    'FTPの各種リクエストに対する応答受信期限
    Public Shared FtpRequestLimitTicks As Integer

    'FTPのログアウトのリクエストに対する応答受信期限
    Public Shared FtpLogoutLimitTicks As Integer

    'FTPで異常と判定するデータ転送停止時間（-1は無期限）
    Public Shared FtpTransferStallLimitTicks As Integer

    'FTPでパッシブモードを使うか否か
    Public Shared FtpUsePassiveMode As Boolean

    'FTPで転送を行うごとにログアウトするか否か
    Public Shared FtpLogoutEachTime As Boolean

    'FTPで使用するバッファの容量
    Public Shared FtpBufferLength As Integer

    'INIファイル内のセクション名
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const COLOR_SECTION As String = "Color"
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const PATH_SECTION As String = "Path"
    Protected Const NETWORK_SECTION As String = "Network"
    Protected Const TIME_INFO_SECTION As String = "TimeInfo"
    Protected Const TELEGRAPHER_MODE_SECTION As String = "TelegrapherMode"

    'INIファイル内における各設定項目のキー
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const BACKGROUND_COLOR_KEY As String = "ScreenRGB"
    Private Const BUTTON_COLOR_KEY As String = "ButtonRGB"
    Private Const FTP_USER_NAME_KEY As String = "FtpUserName"
    Private Const FTP_PASSWORD_KEY As String = "FtpPassword"
    Private Const FTP_WORKING_DIR_PATH_KEY As String = "FtpWorkingDirPath"
    Private Const PERMITTED_PATH_IN_FTP_KEY As String = "PermittedPathInFtp"
    Private Const FTP_SERVER_URI_KEY As String = "FtpServerUri"
    Private Const SERVER_IP_ADDR_KEY As String = "ServerIpAddr"
    Private Const TELEG_CON_PORT_KEY As String = "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = "TelegrapherPendingLimitTicks"
    Private Const TELEGRAPHER_ULL_LIMIT_KEY As String = "TelegrapherUllLimitTicks"
    Private Const TELEGRAPHER_DLL_INVOKE_LIMIT_KEY As String = "TelegrapherDllInvokeLimitTicks"
    Private Const WATCHDOG_INTERVAL_LIMIT_KEY As String = "WatchdogIntervalLimitTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = "TelegLoggingMaxLengthOnWrite"
    Private Const COM_START_REPLY_LIMIT_KEY As String = "ComStartReplyLimitTicks"
    Private Const MASPRO_DLL_INVOKE_REPLY_LIMIT_KEY As String = "MasProDllInvokeReplyLimitTicks"
    Private Const MASPRO_ULL_TRANSFER_LIMIT_KEY As String = "MasProUllTransferLimitTicks"
    Private Const MASPRO_ULL_START_REPLY_LIMIT_KEY As String = "MasProUllStartReplyLimitTicks"
    Private Const MASPRO_ULL_FINISH_REPLY_LIMIT_KEY As String = "MasProUllFinishReplyLimitTicks"
    Private Const ENABLE_WATCHDOG_KEY As String = "EnableWatchdog"
    Private Const ENABLE_XLL_STRONG_EXCLUSION_KEY As String = "EnableXllStrongExclusion"
    Private Const ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY As String = "EnableActiveSeqStrongExclusion"
    Private Const ENABLE_ACTIVE_ONE_ORDERING_KEY As String = "EnableActiveOneOrdering"
    Private Const FTP_WORKER_PENDING_LIMIT_TICKS_KEY As String = "FtpWorkerPendingLimitTicks"
    Private Const FTP_REQUEST_LIMIT_TICKS_KEY As String = "FtpRequestLimitTicks"
    Private Const FTP_LOGOUT_LIMIT_TICKS_KEY As String = "FtpLogoutLimitTicks"
    Private Const FTP_TRANSFER_STALL_LIMIT_TICKS_KEY As String = "FtpTransferStallLimitTicks"
    Private Const FTP_USE_PASSIVE_MODE_KEY As String = "FtpUsePassiveMode"
    Private Const FTP_LOGOUT_EACH_TIME_KEY As String = "FtpLogoutEachTime"
    Private Const FTP_BUFFER_LENGTH_KEY As String = "FtpBufferLength"

    ''' <summary>INIファイルからデ集クライアント試供アプリに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Dim arrTemp As String()
        Try
            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(COLOR_SECTION, BACKGROUND_COLOR_KEY)
            arrTemp = LastReadValue.Split(","c)
            BackgroundColor = System.Drawing.Color.FromArgb(Integer.Parse(arrTemp(0)), Integer.Parse(arrTemp(1)), Integer.Parse(arrTemp(2)))

            ReadFileElem(COLOR_SECTION, BUTTON_COLOR_KEY)
            arrTemp = LastReadValue.Split(","c)
            ButtonColor = System.Drawing.Color.FromArgb(Integer.Parse(arrTemp(0)), Integer.Parse(arrTemp(1)), Integer.Parse(arrTemp(2)))

            ReadFileElem(CREDENTIAL_SECTION, FTP_USER_NAME_KEY)
            FtpUserName = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, FTP_PASSWORD_KEY)
            FtpPassword = LastReadValue

            ReadFileElem(PATH_SECTION, FTP_WORKING_DIR_PATH_KEY)
            FtpWorkingDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, PERMITTED_PATH_IN_FTP_KEY)
            PermittedPathInFtp = LastReadValue

            ReadFileElem(NETWORK_SECTION, FTP_SERVER_URI_KEY)
            FtpServerUri = LastReadValue

            ReadFileElem(NETWORK_SECTION, SERVER_IP_ADDR_KEY)
            ServerIpAddr = LastReadValue

            ReadFileElem(NETWORK_SECTION, TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_ULL_LIMIT_KEY)
            TelegrapherUllLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_DLL_INVOKE_LIMIT_KEY)
            TelegrapherDllInvokeLimitTicks = Integer.Parse(LastReadValue)

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

            ReadFileElem(TIME_INFO_SECTION, MASPRO_DLL_INVOKE_REPLY_LIMIT_KEY)
            MasProDllInvokeReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MASPRO_ULL_TRANSFER_LIMIT_KEY)
            MasProUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MASPRO_ULL_START_REPLY_LIMIT_KEY)
            MasProUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MASPRO_ULL_FINISH_REPLY_LIMIT_KEY)
            MasProUllFinishReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_WATCHDOG_KEY)
            EnableWatchdog = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_XLL_STRONG_EXCLUSION_KEY)
            EnableXllStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY)
            EnableActiveSeqStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_ONE_ORDERING_KEY)
            EnableActiveOneOrdering = Boolean.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, FTP_WORKER_PENDING_LIMIT_TICKS_KEY)
            FtpWorkerPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, FTP_REQUEST_LIMIT_TICKS_KEY)
            FtpRequestLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, FTP_LOGOUT_LIMIT_TICKS_KEY)
            FtpLogoutLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, FTP_TRANSFER_STALL_LIMIT_TICKS_KEY)
            FtpTransferStallLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, FTP_USE_PASSIVE_MODE_KEY)
            FtpUsePassiveMode = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, FTP_LOGOUT_EACH_TIME_KEY)
            FtpLogoutEachTime = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, FTP_BUFFER_LENGTH_KEY)
            FtpBufferLength = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
