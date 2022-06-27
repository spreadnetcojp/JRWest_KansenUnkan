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

Public Class Config
    Inherits TelServerAppBaseConfig

    '接続状態取得シーケンスの間隔
    Public Shared TktConStatusGetIntervalTicks As Integer

    '接続状態取得シーケンスにおける応答電文受信期限
    Public Shared TktConStatusGetReplyLimitTicks As Integer

    '接続状態取得シーケンスにおけるリトライのインターバル
    Public Shared TktConStatusGetRetryIntervalTicks As Integer

    '接続状態取得シーケンスにおけるリトライの最大回数（正常とみなすべきNAK受信時）
    Public Shared TktConStatusGetMaxRetryCountToForget As Integer

    '接続状態取得シーケンスにおけるリトライの最大回数（継続すべきでないNAK受信時）
    Public Shared TktConStatusGetMaxRetryCountToCare As Integer

    '窓処マスタ一式DLLにおける最大転送時間（0や-1は無期限）
    Public Shared MadoMasterSuiteDllTransferLimitTicks As Integer

    '窓処マスタ一式DLLにおける開始電文の応答受信期限
    Public Shared MadoMasterSuiteDllStartReplyLimitTicks As Integer

    '窓処マスタ一式DLLにおける開始リトライのインターバル
    Public Shared MadoMasterSuiteDllRetryIntervalTicks As Integer

    '窓処マスタ一式DLLにおける開始リトライの最大回数
    Public Shared MadoMasterSuiteDllMaxRetryCountToCare As Integer

    '窓処マスタ適用リストDLLにおける最大転送時間（0や-1は無期限）
    Public Shared MadoMasterListDllTransferLimitTicks As Integer

    '窓処マスタ適用リストDLLにおける開始電文の応答受信期限
    Public Shared MadoMasterListDllStartReplyLimitTicks As Integer

    '窓処マスタ適用リストDLLにおける開始リトライのインターバル
    Public Shared MadoMasterListDllRetryIntervalTicks As Integer

    '窓処マスタ適用リストDLLにおける開始リトライの最大回数
    Public Shared MadoMasterListDllMaxRetryCountToCare As Integer

    '窓処プログラム一式DLLにおける最大転送時間（0や-1は無期限）
    Public Shared MadoProgramSuiteDllTransferLimitTicks As Integer

    '窓処プログラム一式DLLにおける開始電文の応答受信期限
    Public Shared MadoProgramSuiteDllStartReplyLimitTicks As Integer

    '窓処プログラム一式DLLにおける開始リトライのインターバル
    Public Shared MadoProgramSuiteDllRetryIntervalTicks As Integer

    '窓処プログラム一式DLLにおける開始リトライの最大回数
    Public Shared MadoProgramSuiteDllMaxRetryCountToCare As Integer

    '窓処プログラム適用リストDLLにおける最大転送時間（0や-1は無期限）
    Public Shared MadoProgramListDllTransferLimitTicks As Integer

    '窓処プログラム適用リストDLLにおける開始電文の応答受信期限
    Public Shared MadoProgramListDllStartReplyLimitTicks As Integer

    '窓処プログラム適用リストDLLにおける開始リトライのインターバル
    Public Shared MadoProgramListDllRetryIntervalTicks As Integer

    '窓処プログラム適用リストDLLにおける開始リトライの最大回数
    Public Shared MadoProgramListDllMaxRetryCountToCare As Integer

    '窓処マスタバージョン情報ULLにおける最大転送時間（0や-1は無期限）
    Public Shared MadoMasterVersionInfoUllTransferLimitTicks As Integer

    '窓処プログラムバージョン情報ULLにおける最大転送時間（0や-1は無期限）
    Public Shared MadoProgramVersionInfoUllTransferLimitTicks As Integer

    'プロセス別キーに対するプレフィックス
    Private Const MODEL_NAME As String = "Tokatsu"

    'INIファイル内における各設定項目のキー
    Private Const TKT_CON_STATUS_GET_INTERVAL_TICKS_KEY As String = "TktConStatusGetIntervalTicks"
    Private Const TKT_CON_STATUS_GET_REPLY_LIMIT_TICKS_KEY As String = "TktConStatusGetReplyLimitTicks"
    Private Const TKT_CON_STATUS_GET_RETRY_INTERVAL_TICKS_KEY As String = "TktConStatusGetRetryIntervalTicks"
    Private Const TKT_CON_STATUS_GET_MAX_RETRY_COUNT_TO_FORGET_KEY As String = "TktConStatusGetMaxRetryCountToForget"
    Private Const TKT_CON_STATUS_GET_MAX_RETRY_COUNT_TO_CARE_KEY As String = "TktConStatusGetMaxRetryCountToCare"
    Private Const MADO_MASTER_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "MadoMasterSuiteDllTransferLimitTicks"
    Private Const MADO_MASTER_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "MadoMasterSuiteDllStartReplyLimitTicks"
    Private Const MADO_MASTER_SUITE_DLL_RETRY_INTERVAL_KEY As String = "MadoMasterSuiteDllRetryIntervalTicks"
    Private Const MADO_MASTER_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "MadoMasterSuiteDllMaxRetryCountToCare"
    Private Const MADO_MASTER_LIST_DLL_TRANSFER_LIMIT_KEY As String = "MadoMasterListDllTransferLimitTicks"
    Private Const MADO_MASTER_LIST_DLL_START_REPLY_LIMIT_KEY As String = "MadoMasterListDllStartReplyLimitTicks"
    Private Const MADO_MASTER_LIST_DLL_RETRY_INTERVAL_KEY As String = "MadoMasterListDllRetryIntervalTicks"
    Private Const MADO_MASTER_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "MadoMasterListDllMaxRetryCountToCare"
    Private Const MADO_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "MadoProgramSuiteDllTransferLimitTicks"
    Private Const MADO_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "MadoProgramSuiteDllStartReplyLimitTicks"
    Private Const MADO_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY As String = "MadoProgramSuiteDllRetryIntervalTicks"
    Private Const MADO_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "MadoProgramSuiteDllMaxRetryCountToCare"
    Private Const MADO_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY As String = "MadoProgramListDllTransferLimitTicks"
    Private Const MADO_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY As String = "MadoProgramListDllStartReplyLimitTicks"
    Private Const MADO_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY As String = "MadoProgramListDllRetryIntervalTicks"
    Private Const MADO_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "MadoProgramListDllMaxRetryCountToCare"
    Private Const MADO_MASTER_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "MadoMasterVersionInfoUllTransferLimitTicks"
    Private Const MADO_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "MadoProgramVersionInfoUllTransferLimitTicks"

    ''' <summary>INIファイルから運管サーバの対統括通信プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        TelServerAppBaseInit(sIniFilePath, MODEL_NAME, True)

        Dim sAppIdentifier As String = "To" & MODEL_NAME
        Try
            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_INTERVAL_TICKS_KEY)
            TktConStatusGetIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_REPLY_LIMIT_TICKS_KEY)
            TktConStatusGetReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_RETRY_INTERVAL_TICKS_KEY)
            TktConStatusGetRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_MAX_RETRY_COUNT_TO_FORGET_KEY)
            TktConStatusGetMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, TKT_CON_STATUS_GET_MAX_RETRY_COUNT_TO_CARE_KEY)
            TktConStatusGetMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_SUITE_DLL_TRANSFER_LIMIT_KEY)
            MadoMasterSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_SUITE_DLL_START_REPLY_LIMIT_KEY)
            MadoMasterSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_SUITE_DLL_RETRY_INTERVAL_KEY)
            MadoMasterSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            MadoMasterSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_LIST_DLL_TRANSFER_LIMIT_KEY)
            MadoMasterListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_LIST_DLL_START_REPLY_LIMIT_KEY)
            MadoMasterListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_LIST_DLL_RETRY_INTERVAL_KEY)
            MadoMasterListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            MadoMasterListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY)
            MadoProgramSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY)
            MadoProgramSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY)
            MadoProgramSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            MadoProgramSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY)
            MadoProgramListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY)
            MadoProgramListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY)
            MadoProgramListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            MadoProgramListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_MASTER_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            MadoMasterVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, MADO_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            MadoProgramVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)
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
