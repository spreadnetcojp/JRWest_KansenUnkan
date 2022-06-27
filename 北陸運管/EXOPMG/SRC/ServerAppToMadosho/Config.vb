' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2015/01/09  (NES)金沢  窓処業務前認証ログ収集対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits TelServerAppBaseConfig

    '窓処操作ログ管理ディレクトリのパス
    Public Shared MadoLogDirPath As String

    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
    '窓処業務前認証ログ管理ディレクトリのパス
    Public Shared MadoCertLogDirPath As String
    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------

    '異常データULLにおける最大転送時間（0や-1は無期限）
    Public Shared MadoFaultDataUllTransferLimitTicks As Integer

    '異常データULLにおける開始電文の応答受信期限
    Public Shared MadoFaultDataUllStartReplyLimitTicks As Integer

    '異常データULLにおける開始リトライのインターバル
    Public Shared MadoFaultDataUllRetryIntervalTicks As Integer

    '異常データULLにおける開始リトライの最大回数（正常とみなすべきNAK受信時）
    Public Shared MadoFaultDataUllMaxRetryCountToForget As Integer

    '異常データULLにおける開始リトライの最大回数（継続すべきでないNAK受信時）
    Public Shared MadoFaultDataUllMaxRetryCountToCare As Integer

    '稼動データULLにおける最大転送時間（0や-1は無期限）
    Public Shared MadoKadoDataUllTransferLimitTicks As Integer

    '稼動データULLにおける開始電文の応答受信期限
    Public Shared MadoKadoDataUllStartReplyLimitTicks As Integer

    '稼動データULLにおける開始リトライのインターバル
    Public Shared MadoKadoDataUllRetryIntervalTicks As Integer

    '稼動データULLにおける開始リトライの最大回数（正常とみなすべきNAK受信時）
    Public Shared MadoKadoDataUllMaxRetryCountToForget As Integer

    '稼動データULLにおける開始リトライの最大回数（継続すべきでないNAK受信時）
    Public Shared MadoKadoDataUllMaxRetryCountToCare As Integer

    '窓処操作ログの枝番最大値
    Public Shared MadoLogMaxBranchNumber As Integer

    '窓処業務前認証ログ管理の枝番最大値
    Public Shared MadoCertLogMaxBranchNumber As Integer

    'プロセス別キーに対するプレフィックス
    Private Const MODEL_NAME As String = "Madosho"

    'INIファイル内における各設定項目のキー
    Private Const MADO_LOG_DIR_PATH_KEY As String = "MadoLogDirPath"
    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
    Private Const MADO_CERT_LOG_DIR_PATH_KEY As String = "MadoCertLogDirPath"
    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------
    Private Const MADO_FAULT_DATA_ULL_TRANSFER_LIMIT_KEY As String = "MadoFaultDataUllTransferLimitTicks"
    Private Const MADO_FAULT_DATA_ULL_START_REPLY_LIMIT_KEY As String = "MadoFaultDataUllStartReplyLimitTicks"
    Private Const MADO_FAULT_DATA_ULL_RETRY_INTERVAL_KEY As String = "MadoFaultDataUllRetryIntervalTicks"
    Private Const MADO_FAULT_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "MadoFaultDataUllMaxRetryCountToForget"
    Private Const MADO_FAULT_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "MadoFaultDataUllMaxRetryCountToCare"
    Private Const MADO_KADO_DATA_ULL_TRANSFER_LIMIT_KEY As String = "MadoKadoDataUllTransferLimitTicks"
    Private Const MADO_KADO_DATA_ULL_START_REPLY_LIMIT_KEY As String = "MadoKadoDataUllStartReplyLimitTicks"
    Private Const MADO_KADO_DATA_ULL_RETRY_INTERVAL_KEY As String = "MadoKadoDataUllRetryIntervalTicks"
    Private Const MADO_KADO_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "MadoKadoDataUllMaxRetryCountToForget"
    Private Const MADO_KADO_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "MadoKadoDataUllMaxRetryCountToCare"
    Private Const MADO_LOG_MAX_BRANCH_NUMBER_KEY As String = "MadoLogMaxBranchNumber"
    Private Const MADO_CERT_LOG_MAX_BRANCH_NUMBER_KEY As String = "MadoCertLogMaxBranchNumber"

    ''' <summary>INIファイルから運管サーバの対窓処通信プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        TelServerAppBaseInit(sIniFilePath, MODEL_NAME, True)

        'Dim sAppIdentifier As String = "To" & MODEL_NAME
        Try
            ReadFileElem(PATH_SECTION, MADO_LOG_DIR_PATH_KEY)
            MadoLogDirPath = LastReadValue

            '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
            ReadFileElem(PATH_SECTION, MADO_CERT_LOG_DIR_PATH_KEY)
            MadoCertLogDirPath = LastReadValue
            '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_TRANSFER_LIMIT_KEY)
            MadoFaultDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_START_REPLY_LIMIT_KEY)
            MadoFaultDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_RETRY_INTERVAL_KEY)
            MadoFaultDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            MadoFaultDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_FAULT_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            MadoFaultDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_TRANSFER_LIMIT_KEY)
            MadoKadoDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_START_REPLY_LIMIT_KEY)
            MadoKadoDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_RETRY_INTERVAL_KEY)
            MadoKadoDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            MadoKadoDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_KADO_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            MadoKadoDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(REGULATION_SECTION, MADO_LOG_MAX_BRANCH_NUMBER_KEY)
            MadoLogMaxBranchNumber = Integer.Parse(LastReadValue)

            ReadFileElem(REGULATION_SECTION, MADO_CERT_LOG_MAX_BRANCH_NUMBER_KEY)
            MadoCertLogMaxBranchNumber = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    Public Shared Sub Dispose()
        TelServerAppBaseDispose()
    End Sub

End Class
