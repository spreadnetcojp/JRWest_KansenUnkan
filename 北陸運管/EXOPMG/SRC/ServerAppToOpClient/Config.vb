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

    'ローカルファイルシステムにおけるFTPサーバルートディレクトリ名
    Public Shared FtpServerRootDirPath As String

    'FTPサーバ内におけるアクセス許可ディレクトリ名
    Public Shared PermittedPathInFtp As String

    'スレッド別テンポラリディレクトリのベース（プロセスのテンポラリディレクトリ）
    Public Shared TemporaryBaseDirPath As String

    '監視盤プログラムのCAB内におけるバージョンリストファイルのパス
    Public Shared KsbProgramVersionListPathInCab As String

    '改札機プログラムのCAB内におけるバージョンリストファイルのパス
    Public Shared GateProgramVersionListPathInCab As String

    '窓処プログラムのCAB内におけるバージョンリストファイルのパス
    Public Shared MadoProgramVersionListPathInCab As String

    'NOTE: XxxProgramGroupNamesInCabやXxxProgramGroupTitlesの要素の
    '件数や順序は、プログラムバージョン情報内の領域の件数や順序と同一である。
    'NOTE: 各ファイルの表示名は、運管サーバ内にプログラムを登録した際に、
    'XxxProgramGroupTitlesの要素を参照して決定する。
    'XxxProgramGroupTitlesの当該要素が1文字以上の場合は、それに
    'ファイル名（拡張子除去済み）を連結したパス文字列を表示名とする。
    'XxxProgramGroupTitlesの当該要素が0文字の場合は、ファイルフッタ
    'に設定されている「表示用データ」を表示名とする。

    '改札機プログラムのCAB内における全グループディレクトリのベースパス
    Public Shared GateProgramGroupBasePathInCab As String

    '改札機プログラムのCAB内における各グループディレクトリの名前
    Public Shared GateProgramGroupNamesInCab As String()

    '改札機プログラムの各グループディレクトリの表示名
    Public Shared GateProgramGroupTitles As String()

    '電文通信用リッスンアドレス
    Public Shared IpAddrForTelegConnection As IPAddress

    '電文通信用ポート番号
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

    'ウォッチドッグシーケンスにおける応答電文受信期限
    Public Shared WatchdogReplyLimitTicks As Integer

    '対運管端末通信プロセスのファイル転送シーケンス排他増強モード設定
    Public Shared EnableXllStrongExclusion As Boolean

    '対運管端末通信プロセスの能動的シーケンス排他増強モード設定
    Public Shared EnableActiveSeqStrongExclusion As Boolean

    '対運管端末通信プロセスの能動的単発シーケンス順序強制モード設定
    Public Shared EnableActiveOneOrdering As Boolean

    '運管端末ファイルULLにおける最大転送時間（0や-1は無期限）
    Public Shared OpClientFileUllTransferLimitTicks As Integer

    'プロセス別キーに対するプレフィックス
    Private Const APP_ID As String = "ToOpClient"

    'INIファイル内における各設定項目のキー
    Private Const FTP_SERVER_ROOT_DIR_PATH_KEY As String = APP_ID & "FtpServerRootDirPath"
    Private Const PERMITTED_PATH_IN_FTP_KEY As String = APP_ID & "PermittedPathInFtp"
    Private Const TEMP_BASE_DIR_PATH_KEY As String = APP_ID & "TemporaryBaseDirPath"
    Private Const KSB_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "KsbProgramVersionListPathInCab"
    Private Const GATE_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "GateProgramVersionListPathInCab"
    Private Const MADO_PRG_VER_LIST_PATH_IN_CAB_KEY As String = "MadoProgramVersionListPathInCab"
    Private Const GATE_PRG_GROUP_BASE_PATH_IN_CAB_KEY As String = "GateProgramGroupBasePathInCab"
    Private Const GATE_PRG_GROUP_NAMES_IN_CAB_KEY As String = "GateProgramGroupNamesInCab"
    Private Const GATE_PRG_GROUP_TITLES_IN_CAB_KEY As String = "GateProgramGroupTitles"
    Private Const SELF_ADDR_KEY As String = "SelfAddr"
    Private Const TELEG_CON_PORT_KEY As String = APP_ID & "TelegConnectionPort"
    Private Const TELEGRAPHER_PENDING_LIMIT_KEY As String = APP_ID & "TelegrapherPendingLimitTicks"
    Private Const WATCHDOG_INTERVAL_KEY As String = APP_ID & "WatchdogIntervalTicks"
    Private Const TELEG_READING_LIMIT_BASE_KEY As String = APP_ID & "TelegReadingLimitBaseTicks"
    Private Const TELEG_READING_LIMIT_EXTRA_KEY As String = APP_ID & "TelegReadingLimitExtraTicksPerMiB"
    Private Const TELEG_WRITING_LIMIT_BASE_KEY As String = APP_ID & "TelegWritingLimitBaseTicks"
    Private Const TELEG_WRITING_LIMIT_EXTRA_KEY As String = APP_ID & "TelegWritingLimitExtraTicksPerMiB"
    Private Const TELEG_LOGGING_MAX_ON_READ_KEY As String = APP_ID & "TelegLoggingMaxLengthOnRead"
    Private Const TELEG_LOGGING_MAX_ON_WRITE_KEY As String = APP_ID & "TelegLoggingMaxLengthOnWrite"
    Private Const WATCHDOG_REPLY_LIMIT_KEY As String = APP_ID & "WatchdogReplyLimitTicks"
    Private Const ENABLE_XLL_STRONG_EXCLUSION_KEY As String = APP_ID & "EnableXllStrongExclusion"
    Private Const ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY As String = APP_ID & "EnableActiveSeqStrongExclusion"
    Private Const ENABLE_ACTIVE_ONE_ORDERING_KEY As String = APP_ID & "EnableActiveOneOrdering"

    Private Const OPC_FILE_ULL_TRANSFER_LIMIT_KEY As String = "OpClientFileUllTransferLimitTicks"

    ''' <summary>INIファイルから運管サーバの対運管端末通信プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID, True)

        Try
            ReadFileElem(PATH_SECTION, FTP_SERVER_ROOT_DIR_PATH_KEY)
            FtpServerRootDirPath = LastReadValue

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

            ReadFileElem(PATH_SECTION, GATE_PRG_GROUP_BASE_PATH_IN_CAB_KEY)
            GateProgramGroupBasePathInCab = LastReadValue

            ReadFileElem(PATH_SECTION, GATE_PRG_GROUP_NAMES_IN_CAB_KEY)
            GateProgramGroupNamesInCab = LastReadValue.Split(","c)

            ReadFileElem(PATH_SECTION, GATE_PRG_GROUP_TITLES_IN_CAB_KEY)
            GateProgramGroupTitles = LastReadValue.Split(","c)

            If GateProgramGroupNamesInCab.Length <> GateProgramGroupTitles.Length Then
                Throw New OPMGException("Number of the elements is invalid. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
            End If

            ReadFileElem(NETWORK_SECTION, SELF_ADDR_KEY)
            IpAddrForTelegConnection = IPAddress.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, TELEG_CON_PORT_KEY)
            IpPortForTelegConnection = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TELEGRAPHER_PENDING_LIMIT_KEY)
            TelegrapherPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, WATCHDOG_INTERVAL_KEY)
            WatchdogIntervalTicks = Integer.Parse(LastReadValue)

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

            ReadFileElem(TIME_INFO_SECTION, WATCHDOG_REPLY_LIMIT_KEY)
            WatchdogReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_XLL_STRONG_EXCLUSION_KEY)
            EnableXllStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_SEQ_STRONG_EXCLUSION_KEY)
            EnableActiveSeqStrongExclusion = Boolean.Parse(LastReadValue)

            ReadFileElem(TELEGRAPHER_MODE_SECTION, ENABLE_ACTIVE_ONE_ORDERING_KEY)
            EnableActiveOneOrdering = Boolean.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, OPC_FILE_ULL_TRANSFER_LIMIT_KEY)
            OpClientFileUllTransferLimitTicks = Integer.Parse(LastReadValue)
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
