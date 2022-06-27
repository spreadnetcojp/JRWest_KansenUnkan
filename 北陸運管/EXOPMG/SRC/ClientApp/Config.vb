' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2013/11/11  (NES)金沢  フェーズ２権限対応
'   0.2      2014/04/01  (NES)河脇  北陸対応
'                                       マスタ取込時、チェックパターン追加対応
'                                       グループ別監視盤設定情報の表示制御対応
'                                       グループ別稼動保守の出力制御対応
'                                       グループ別不正乗車券検出の出力制御対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits BaseConfig

    '端末種別
    Public Shared MachineKind As String

    '端末ID
    Public Shared MachineName As String

    '-------Ver0.1　フェーズ２権限対応　ADD START-----------

    'バージョン番号
    Public Shared VerNoSet As String

    'IDマスタ書式ファイルのパス
    Public Shared IdMasterFormatFilePath As String
    '-------Ver0.1　フェーズ２権限対応　ADD END-----------

    '事業者
    Public Shared SelfCompany As EkCompany

    'アカウントをロックアウトするログイン試行回数
    Public Shared MaxInvalidPasswordAttempts As Integer

    'FTPユーザ名
    Public Shared FtpUserName As String

    'FTPパスワード
    Public Shared FtpPassword As String

    '有効ログ種別
    Public Shared LogKindsMask As Integer

    'DBからの取得許容行数
    Public Shared MaxUpboundDataToGet As Integer

    '背景色
    Public Shared BackgroundColor As System.Drawing.Color

    'ボタン色
    Public Shared ButtonColor As System.Drawing.Color

    '状態保存ファイルのパス
    Public Shared CookieFilePath As String

    '機器構成マスタ書式ファイルのパス
    Public Shared MachineMasterFormatFilePath As String

    '帳票テンプレートディレクトリのパス
    Public Shared LedgerTemplateDirPath As String

    'FTPワーキングディレクトリ名
    Public Shared FtpWorkingDirPath As String

    'FTPサーバ内におけるアクセス許可ディレクトリ名
    Public Shared PermittedPathInFtp As String

    'FTPサーバURI
    Public Shared FtpServerUri As String

    '一時作業用ディレクトリ名
    Public Shared TemporaryBaseDirPath As String

    '監視盤プログラムのCAB内におけるバージョンリストファイルのパス
    Public Shared KsbProgramVersionListPathInCab As String

    '改札機プログラムのCAB内におけるバージョンリストファイルのパス
    Public Shared GateProgramVersionListPathInCab As String

    '窓処プログラムのCAB内におけるバージョンリストファイルのパス
    Public Shared MadoProgramVersionListPathInCab As String

    '運管サーバIPアドレス
    Public Shared ServerIpAddr As String

    '電文通信用ポート番号
    Public Shared IpPortForTelegConnection As Integer

    '機器接続状態確認画面の更新周期（分）
    Public Shared ConStatusDispRefreshRate As Integer

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

    '操作ログを保持する日数
    Public Shared OperationLogsKeepingDays As Integer

    'ログを保持する日数
    Public Shared LogsKeepingDays As Integer

    '-------Ver0.2　北陸対応　ADD START-----------
    'マスタ毎に許可するパターン番号（範囲）
    Public Shared MstLimitPattern As New ArrayList

    'グループ毎に許可するパターン番号（範囲）
    Public Shared MstLimitEkiCode As New ArrayList

    '監視盤設定情報のグループ別一覧列表示有無
    Public Shared KsbConfigOutListCol As New ArrayList

    '監視盤設定情報の特別指定
    Public Shared KsbConfigDirectEkCode As String

    '監視盤設定情報の特別コーナ別一覧列表示有無
    Public Shared KsbConfigOutListColDirect As String

    '監視盤設定情報の特別コーナ別帳票
    Public Shared KsbConfigPrintDirect As String

    '監視盤設定情報グループ別一覧列表示有無
    Public Shared KsbPrintList As New ArrayList

    '不正乗車券検出データグループ別帳票
    Public Shared FuseiJoshaPrintList As New ArrayList

    '稼動保守データ出力のグループ別帳票
    Public Shared KadoPrintListK As New ArrayList
    Public Shared KadoPrintListH As New ArrayList

    '稼動保守データ設定のグループ別帳票
    Public Shared KadoPrintSetList As New ArrayList

    '稼動保守データ設定の機種コンボアイテム
    Public Shared SysKadoDataModelCode As New ArrayList

    '-------Ver0.2　北陸対応　ADD END-----------

    'INIファイル内のセクション名
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const COLOR_SECTION As String = "Color"
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const PATH_SECTION As String = "Path"
    Protected Const NETWORK_SECTION As String = "Network"
    Protected Const TIME_INFO_SECTION As String = "TimeInfo"
    Protected Const TELEGRAPHER_MODE_SECTION As String = "TelegrapherMode"
    Protected Const STORAGE_LIFE_SECTION As String = "StorageLife"
    '-------Ver0.2　北陸対応　ADD START-----------
    Protected Const MSTINPUTCHECK_SECTION As String = "MstInputCheck"
    Protected Const KSBCONFIG_SECTION As String = "MntDispKsbConfig"
    Protected Const FUSEIJYOSYA_SECTION As String = "MntDispFuseiJoshaData"
    Protected Const SYSKADODATAMST_SECTION As String = "SysKadoDataMst"
    Protected Const MNTKADOPRINT_SECTION As String = "MntDispKadoData"
    '-------Ver0.2　北陸対応　ADD END-----------

    'INIファイル内における各設定項目のキー
    Private Const MACHINE_KIND_KEY As String = "MachineKind"
    Private Const MACHINE_NAME_KEY As String = "MachineName"
    Private Const SELF_COMPANY_KEY As String = "SelfCompany"
    Private Const MAX_INVALID_PASSWORD_ATTEMPTS_KEY As String = "MaxInvalidPasswordAttempts"
    Private Const FTP_USER_NAME_KEY As String = "FtpUserName"
    Private Const FTP_PASSWORD_KEY As String = "FtpPassword"
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const MAX_UPBOUNDS_TO_GET_KEY As String = "MaxUpboundDataToGet"
    Private Const BACKGROUND_COLOR_KEY As String = "ScreenRGB"
    Private Const BUTTON_COLOR_KEY As String = "ButtonRGB"
    Private Const COOKIE_FILE_PATH_KEY As String = "CookieFilePath"
    Private Const MACHINE_MASTER_FORMAT_FILE_PATH_KEY As String = "MachineMasterFormatFilePath"
    '-------Ver0.1　フェーズ２権限/バージョン表示対応　START-----------
    Private Const VER_NO_KEY As String = "VerNoSet"
    Private Const ID_MASTER_FORMAT_FILE_PATH_KEY As String = "IdMasterFormatFilePath"
    '-------Ver0.1　フェーズ２権限/バージョン表示対応　END-------------
    Private Const LEDGER_TEMPLATE_DIR_PATH_KEY As String = "LedgerTemplateDirPath"
    Private Const FTP_WORKING_DIR_PATH_KEY As String = "FtpWorkingDirPath"
    Private Const PERMITTED_PATH_IN_FTP_KEY As String = "PermittedPathInFtp"
    Private Const FTP_SERVER_URI_KEY As String = "FtpServerUri"
    Private Const TEMP_BASE_DIR_PATH_KEY As String = "TemporaryBaseDirPath"
    Private Const KSB_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "KsbProgramVersionListPathInCab"
    Private Const GATE_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "GateProgramVersionListPathInCab"
    Private Const MADO_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "MadoProgramVersionListPathInCab"
    Private Const SERVER_IP_ADDR_KEY As String = "ServerIpAddr"
    Private Const TELEG_CON_PORT_KEY As String = "TelegConnectionPort"
    Private Const CON_STATUS_DISP_REFRESH_RATE_KEY As String = "ConStatusDispRefreshRate"
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
    Private Const OPERATION_LOGS_KEEPING_DAYS_KEY As String = "OperationLogsKeepingDays"
    Private Const LOGS_KEEPING_DAYS_KEY As String = "LogsKeepingDays"
    '-------Ver0.2　北陸対応　ADD START-----------
    Private Const LIMIT_PATTERN_KEY As String = "LimitPattern"
    Private Const LIMIT_EKI_CODE_KEY As String = "LimitEkiCode"
    Private Const OUTLIST_COL_KEY As String = "OutListCol"
    Private Const KSB_PRINT_KEY As String = "LedgerTemplate"
    Private Const DIRECT_EKCODE_KEY As String = "DirectEkCode"
    Private Const OUTLISTCOL_DIRECT_KEY As String = "OutListCol_Direct"
    Private Const PRINT_DIRECT_KEY As String = "LedgerTemplate_Direct"
    Private Const FUSEIJYOSYA_PRINT_KEY As String = "LedgerTemplate"
    Private Const KADOUSET_PRINT_KEY As String = "LedgerTemplate"
    Private Const KADOU_K_PRINT_KEY As String = "LedgerTemplateK"
    Private Const KADOU_H_PRINT_KEY As String = "LedgerTemplateH"
    Private Const MODEL_CODE_KEY As String = "ModelCode"
    '-------Ver0.2　北陸対応　ADD END-----------

    ''' <summary>INIファイルから運管端末アプリに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Dim arrTemp As String()
        Try
            ReadFileElem(CREDENTIAL_SECTION, MACHINE_KIND_KEY)
            MachineKind = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, MACHINE_NAME_KEY)
            MachineName = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, SELF_COMPANY_KEY)
            SelfCompany = DirectCast([Enum].Parse(GetType(EkCompany), LastReadValue), EkCompany)

            ReadFileElem(CREDENTIAL_SECTION, MAX_INVALID_PASSWORD_ATTEMPTS_KEY)
            MaxInvalidPasswordAttempts = Integer.Parse(LastReadValue)

            ReadFileElem(CREDENTIAL_SECTION, FTP_USER_NAME_KEY)
            FtpUserName = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, FTP_PASSWORD_KEY)
            FtpPassword = LastReadValue

            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(DATABASE_SECTION, MAX_UPBOUNDS_TO_GET_KEY)
            MaxUpboundDataToGet = Integer.Parse(LastReadValue)

            ReadFileElem(COLOR_SECTION, BACKGROUND_COLOR_KEY)
            arrTemp = LastReadValue.Split(","c)
            BackgroundColor = System.Drawing.Color.FromArgb(Integer.Parse(arrTemp(0)), Integer.Parse(arrTemp(1)), Integer.Parse(arrTemp(2)))

            ReadFileElem(COLOR_SECTION, BUTTON_COLOR_KEY)
            arrTemp = LastReadValue.Split(","c)
            ButtonColor = System.Drawing.Color.FromArgb(Integer.Parse(arrTemp(0)), Integer.Parse(arrTemp(1)), Integer.Parse(arrTemp(2)))

            ReadFileElem(PATH_SECTION, COOKIE_FILE_PATH_KEY)
            CookieFilePath = LastReadValue

            ReadFileElem(PATH_SECTION, MACHINE_MASTER_FORMAT_FILE_PATH_KEY)
            MachineMasterFormatFilePath = LastReadValue

            '-------Ver0.1　フェーズ２権限/バージョン表示対応　ADD START-----------
            ReadFileElem(CREDENTIAL_SECTION, VER_NO_KEY)
            VerNoSet = LastReadValue

            ReadFileElem(PATH_SECTION, ID_MASTER_FORMAT_FILE_PATH_KEY)
            IdMasterFormatFilePath = LastReadValue
            '-------Ver0.1　フェーズ２権限/バージョン表示対応　ADD END-------------

            ReadFileElem(PATH_SECTION, LEDGER_TEMPLATE_DIR_PATH_KEY)
            LedgerTemplateDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, FTP_WORKING_DIR_PATH_KEY)
            FtpWorkingDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, PERMITTED_PATH_IN_FTP_KEY)
            PermittedPathInFtp = LastReadValue

            ReadFileElem(PATH_SECTION, TEMP_BASE_DIR_PATH_KEY)
            TemporaryBaseDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, KSB_PRG_VER_LIST_PATH_IN_CAB_KEY)
            KsbProgramVersionListPathInCab = LastReadValue

            ReadFileElem(PATH_SECTION, GATE_PRG_VER_LIST_PATH_IN_CAB_KEY)
            GateProgramVersionListPathInCab = LastReadValue

            ReadFileElem(PATH_SECTION, MADO_PRG_VER_LIST_PATH_IN_CAB_KEY)
            MadoProgramVersionListPathInCab = LastReadValue

            ReadFileElem(NETWORK_SECTION, FTP_SERVER_URI_KEY)
            FtpServerUri = LastReadValue

            ReadFileElem(NETWORK_SECTION, SERVER_IP_ADDR_KEY)
            ServerIpAddr = LastReadValue

            ReadFileElem(NETWORK_SECTION, TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, CON_STATUS_DISP_REFRESH_RATE_KEY)
            ConStatusDispRefreshRate = Integer.Parse(LastReadValue)

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

            ReadFileElem(STORAGE_LIFE_SECTION, OPERATION_LOGS_KEEPING_DAYS_KEY)
            OperationLogsKeepingDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, LOGS_KEEPING_DAYS_KEY)
            LogsKeepingDays = Integer.Parse(LastReadValue)

            '-------Ver0.2　北陸対応　ADD START-----------
            Dim i As Integer = 0
            'マスタ毎に許可するパターン番号（範囲）情報取得
            i = 0
            Do
                ReadFileElem(MSTINPUTCHECK_SECTION, LIMIT_PATTERN_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i > 0 Then
                        Exit Do
                    End If
                End If
                MstLimitPattern.Add(LastReadValue)
                i = i + 1
            Loop
            i = 0
            'グループ毎に許可するパターン番号（範囲）
            Do
                ReadFileElem(MSTINPUTCHECK_SECTION, LIMIT_EKI_CODE_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i > 0 Then
                        Exit Do
                    End If
                End If
                MstLimitEkiCode.Add(LastReadValue)
                i = i + 1
            Loop

            '監視盤設定情報の帳票リスト取得
            i = 0
            Do
                ReadFileElem(KSBCONFIG_SECTION, KSB_PRINT_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & KSBCONFIG_SECTION & ", Key: " & KSB_PRINT_KEY & ")")
                    Else
                        Exit Do
                    End If
                End If
                KsbPrintList.Add(LastReadValue)
                i = i + 1
            Loop

            '監視盤設定情報の一覧制御リスト取得
            i = 0
            Do
                ReadFileElem(KSBCONFIG_SECTION, OUTLIST_COL_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & KSBCONFIG_SECTION & ", Key: " & OUTLIST_COL_KEY & ")")
                    Else
                        Exit Do
                    End If
                End If
                KsbConfigOutListCol.Add(LastReadValue)
                i = i + 1
            Loop
            '監視盤設定情報の特別コーナー情報取得
            ReadFileElem(KSBCONFIG_SECTION, DIRECT_EKCODE_KEY, False)
            KsbConfigDirectEkCode = LastReadValue

            '監視盤設定情報の特別コーナーの画面一覧制御情報取得
            ReadFileElem(KSBCONFIG_SECTION, OUTLISTCOL_DIRECT_KEY, False)
            KsbConfigOutListColDirect = LastReadValue

            '監視盤設定情報の特別コーナーの帳票ファイル名取得
            ReadFileElem(KSBCONFIG_SECTION, PRINT_DIRECT_KEY, False)
            KsbConfigPrintDirect = LastReadValue

            '不正乗車の帳票リスト取得
            i = 0
            Do
                ReadFileElem(FUSEIJYOSYA_SECTION, FUSEIJYOSYA_PRINT_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & FUSEIJYOSYA_SECTION & ", Key: " & FUSEIJYOSYA_PRINT_KEY & ")")
                    Else
                        Exit Do
                    End If
                End If
                FuseiJoshaPrintList.Add(LastReadValue)
                i = i + 1
            Loop
            '稼動保守データ設定の機種リスト取得
            i = 0
            Do
                ReadFileElem(SYSKADODATAMST_SECTION, MODEL_CODE_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & SYSKADODATAMST_SECTION & ", Key: " & MODEL_CODE_KEY & ")")
                    Else
                        Exit Do
                    End If
                End If
                SysKadoDataModelCode.Add(LastReadValue)
                i = i + 1
            Loop

            '稼動保守データ設定の帳票リスト取得
            i = 0
            Do
                ReadFileElem(SYSKADODATAMST_SECTION, KADOUSET_PRINT_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & SYSKADODATAMST_SECTION & ", Key: " & KADOUSET_PRINT_KEY & ")")
                    Else
                        Exit Do
                    End If
                End If
                KadoPrintSetList.Add(LastReadValue)
                i = i + 1
            Loop

            '稼動データ出力用の帳票リスト取得
            i = 0
            Do
                ReadFileElem(MNTKADOPRINT_SECTION, KADOU_K_PRINT_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & MNTKADOPRINT_SECTION & ", Key: " & KADOU_K_PRINT_KEY & ")")
                    Else
                        Exit Do
                    End If
                End If
                KadoPrintListK.Add(LastReadValue)
                i = i + 1
            Loop

            '保守データ出力用の帳票リスト取得
            i = 0
            Do
                ReadFileElem(MNTKADOPRINT_SECTION, KADOU_H_PRINT_KEY + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & MNTKADOPRINT_SECTION & ", Key: " & KADOU_H_PRINT_KEY & ")")
                    Else
                        Exit Do
                    End If
                End If
                KadoPrintListH.Add(LastReadValue)
                i = i + 1
            Loop

            '-------Ver0.2　北陸対応　ADD END-----------

        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
