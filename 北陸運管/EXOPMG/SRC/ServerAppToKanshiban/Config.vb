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

    '改札機マスタ一式DLLにおける最大転送時間（0や-1は無期限）
    Public Shared GateMasterSuiteDllTransferLimitTicks As Integer

    '改札機マスタ一式DLLにおける開始電文の応答受信期限
    Public Shared GateMasterSuiteDllStartReplyLimitTicks As Integer

    '改札機マスタ一式DLLにおける開始リトライのインターバル
    Public Shared GateMasterSuiteDllRetryIntervalTicks As Integer

    '改札機マスタ一式DLLにおける開始リトライの最大回数
    Public Shared GateMasterSuiteDllMaxRetryCountToCare As Integer

    '改札機マスタ適用リストDLLにおける最大転送時間（0や-1は無期限）
    Public Shared GateMasterListDllTransferLimitTicks As Integer

    '改札機マスタ適用リストDLLにおける開始電文の応答受信期限
    Public Shared GateMasterListDllStartReplyLimitTicks As Integer

    '改札機マスタ適用リストDLLにおける開始リトライのインターバル
    Public Shared GateMasterListDllRetryIntervalTicks As Integer

    '改札機マスタ適用リストDLLにおける開始リトライの最大回数
    Public Shared GateMasterListDllMaxRetryCountToCare As Integer

    '改札機マスタバージョン情報ULLにおける最大転送時間（0や-1は無期限）
    Public Shared GateMasterVersionInfoUllTransferLimitTicks As Integer

    '改札機プログラム一式DLLにおける最大転送時間（0や-1は無期限）
    Public Shared GateProgramSuiteDllTransferLimitTicks As Integer

    '改札機プログラム一式DLLにおける開始電文の応答受信期限
    Public Shared GateProgramSuiteDllStartReplyLimitTicks As Integer

    '改札機プログラム一式DLLにおける開始リトライのインターバル
    Public Shared GateProgramSuiteDllRetryIntervalTicks As Integer

    '改札機プログラム一式DLLにおける開始リトライの最大回数
    Public Shared GateProgramSuiteDllMaxRetryCountToCare As Integer

    '改札機プログラム適用リストDLLにおける最大転送時間（0や-1は無期限）
    Public Shared GateProgramListDllTransferLimitTicks As Integer

    '改札機プログラム適用リストDLLにおける開始電文の応答受信期限
    Public Shared GateProgramListDllStartReplyLimitTicks As Integer

    '改札機プログラム適用リストDLLにおける開始リトライのインターバル
    Public Shared GateProgramListDllRetryIntervalTicks As Integer

    '改札機プログラム適用リストDLLにおける開始リトライの最大回数
    Public Shared GateProgramListDllMaxRetryCountToCare As Integer

    '改札機プログラムバージョン情報ULLにおける最大転送時間（0や-1は無期限）
    Public Shared GateProgramVersionInfoUllTransferLimitTicks As Integer

    '監視盤プログラム一式DLLにおける最大転送時間（0や-1は無期限）
    Public Shared KsbProgramSuiteDllTransferLimitTicks As Integer

    '監視盤プログラム一式DLLにおける開始電文の応答受信期限
    Public Shared KsbProgramSuiteDllStartReplyLimitTicks As Integer

    '監視盤プログラム一式DLLにおける開始リトライのインターバル
    Public Shared KsbProgramSuiteDllRetryIntervalTicks As Integer

    '監視盤プログラム一式DLLにおける開始リトライの最大回数
    Public Shared KsbProgramSuiteDllMaxRetryCountToCare As Integer

    '監視盤プログラム適用リストDLLにおける最大転送時間（0や-1は無期限）
    Public Shared KsbProgramListDllTransferLimitTicks As Integer

    '監視盤プログラム適用リストDLLにおける開始電文の応答受信期限
    Public Shared KsbProgramListDllStartReplyLimitTicks As Integer

    '監視盤プログラム適用リストDLLにおける開始リトライのインターバル
    Public Shared KsbProgramListDllRetryIntervalTicks As Integer

    '監視盤プログラム適用リストDLLにおける開始リトライの最大回数
    Public Shared KsbProgramListDllMaxRetryCountToCare As Integer

    '監視盤プログラムバージョン情報ULLにおける最大転送時間（0や-1は無期限）
    Public Shared KsbProgramVersionInfoUllTransferLimitTicks As Integer

    '別集札データULLにおける最大転送時間（0や-1は無期限）
    Public Shared GateBesshuDataUllTransferLimitTicks As Integer

    '別集札データULLにおける開始電文の応答受信期限
    Public Shared GateBesshuDataUllStartReplyLimitTicks As Integer

    '別集札データULLにおける開始リトライのインターバル
    Public Shared GateBesshuDataUllRetryIntervalTicks As Integer

    '別集札データULLにおける開始リトライの最大回数（正常とみなすべきNAK受信時）
    Public Shared GateBesshuDataUllMaxRetryCountToForget As Integer

    '別集札データULLにおける開始リトライの最大回数（継続すべきでないNAK受信時）
    Public Shared GateBesshuDataUllMaxRetryCountToCare As Integer

    '明細データULLにおける最大転送時間（0や-1は無期限）
    Public Shared GateMeisaiDataUllTransferLimitTicks As Integer

    '明細データULLにおける開始電文の応答受信期限
    Public Shared GateMeisaiDataUllStartReplyLimitTicks As Integer

    '明細データULLにおける開始リトライのインターバル
    Public Shared GateMeisaiDataUllRetryIntervalTicks As Integer

    '明細データULLにおける開始リトライの最大回数（正常とみなすべきNAK受信時）
    Public Shared GateMeisaiDataUllMaxRetryCountToForget As Integer

    '明細データULLにおける開始リトライの最大回数（継続すべきでないNAK受信時）
    Public Shared GateMeisaiDataUllMaxRetryCountToCare As Integer

    '異常データULLにおける最大転送時間（0や-1は無期限）
    Public Shared KsbGateFaultDataUllTransferLimitTicks As Integer

    '異常データULLにおける開始電文の応答受信期限
    Public Shared KsbGateFaultDataUllStartReplyLimitTicks As Integer

    '異常データULLにおける開始リトライのインターバル
    Public Shared KsbGateFaultDataUllRetryIntervalTicks As Integer

    '異常データULLにおける開始リトライの最大回数（正常とみなすべきNAK受信時）
    Public Shared KsbGateFaultDataUllMaxRetryCountToForget As Integer

    '異常データULLにおける開始リトライの最大回数（継続すべきでないNAK受信時）
    Public Shared KsbGateFaultDataUllMaxRetryCountToCare As Integer

    '稼動・保守データULLにおける最大転送時間（0や-1は無期限）
    Public Shared GateKadoDataUllTransferLimitTicks As Integer

    '稼動・保守データULLにおける開始電文の応答受信期限
    Public Shared GateKadoDataUllStartReplyLimitTicks As Integer

    '稼動・保守データULLにおける開始リトライのインターバル
    Public Shared GateKadoDataUllRetryIntervalTicks As Integer

    '稼動・保守データULLにおける開始リトライの最大回数（正常とみなすべきNAK受信時）
    Public Shared GateKadoDataUllMaxRetryCountToForget As Integer

    '稼動・保守データULLにおける開始リトライの最大回数（継続すべきでないNAK受信時）
    Public Shared GateKadoDataUllMaxRetryCountToCare As Integer

    '時間帯別乗降データULLにおける最大転送時間（0や-1は無期限）
    Public Shared GateTrafficDataUllTransferLimitTicks As Integer

    '時間帯別乗降データULLにおける開始電文の応答受信期限
    Public Shared GateTrafficDataUllStartReplyLimitTicks As Integer

    '時間帯別乗降データULLにおける開始リトライのインターバル
    Public Shared GateTrafficDataUllRetryIntervalTicks As Integer

    '時間帯別乗降データULLにおける開始リトライの最大回数（正常とみなすべきNAK受信時）
    Public Shared GateTrafficDataUllMaxRetryCountToForget As Integer

    '時間帯別乗降データULLにおける開始リトライの最大回数（継続すべきでないNAK受信時）
    Public Shared GateTrafficDataUllMaxRetryCountToCare As Integer

    '改札機プログラムの各グループディレクトリの表示名
    Public Shared GateProgramGroupTitles As String()

    'プロセス別キーに対するプレフィックス
    Private Const MODEL_NAME As String = "Kanshiban"

    'INIファイル内における各設定項目のキー
    Private Const GATE_MASTER_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "GateMasterSuiteDllTransferLimitTicks"
    Private Const GATE_MASTER_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "GateMasterSuiteDllStartReplyLimitTicks"
    Private Const GATE_MASTER_SUITE_DLL_RETRY_INTERVAL_KEY As String = "GateMasterSuiteDllRetryIntervalTicks"
    Private Const GATE_MASTER_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "GateMasterSuiteDllMaxRetryCountToCare"
    Private Const GATE_MASTER_LIST_DLL_TRANSFER_LIMIT_KEY As String = "GateMasterListDllTransferLimitTicks"
    Private Const GATE_MASTER_LIST_DLL_START_REPLY_LIMIT_KEY As String = "GateMasterListDllStartReplyLimitTicks"
    Private Const GATE_MASTER_LIST_DLL_RETRY_INTERVAL_KEY As String = "GateMasterListDllRetryIntervalTicks"
    Private Const GATE_MASTER_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "GateMasterListDllMaxRetryCountToCare"
    Private Const GATE_MASTER_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "GateMasterVersionInfoUllTransferLimitTicks"
    Private Const GATE_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "GateProgramSuiteDllTransferLimitTicks"
    Private Const GATE_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "GateProgramSuiteDllStartReplyLimitTicks"
    Private Const GATE_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY As String = "GateProgramSuiteDllRetryIntervalTicks"
    Private Const GATE_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "GateProgramSuiteDllMaxRetryCountToCare"
    Private Const GATE_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY As String = "GateProgramListDllTransferLimitTicks"
    Private Const GATE_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY As String = "GateProgramListDllStartReplyLimitTicks"
    Private Const GATE_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY As String = "GateProgramListDllRetryIntervalTicks"
    Private Const GATE_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "GateProgramListDllMaxRetryCountToCare"
    Private Const GATE_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "GateProgramVersionInfoUllTransferLimitTicks"
    Private Const KSB_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY As String = "KsbProgramSuiteDllTransferLimitTicks"
    Private Const KSB_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY As String = "KsbProgramSuiteDllStartReplyLimitTicks"
    Private Const KSB_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY As String = "KsbProgramSuiteDllRetryIntervalTicks"
    Private Const KSB_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY As String = "KsbProgramSuiteDllMaxRetryCountToCare"
    Private Const KSB_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY As String = "KsbProgramListDllTransferLimitTicks"
    Private Const KSB_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY As String = "KsbProgramListDllStartReplyLimitTicks"
    Private Const KSB_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY As String = "KsbProgramListDllRetryIntervalTicks"
    Private Const KSB_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY As String = "KsbProgramListDllMaxRetryCountToCare"
    Private Const KSB_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY As String = "KsbProgramVersionInfoUllTransferLimitTicks"

    Private Const GATE_BESSHU_DATA_ULL_TRANSFER_LIMIT_KEY As String = "GateBesshuDataUllTransferLimitTicks"
    Private Const GATE_BESSHU_DATA_ULL_START_REPLY_LIMIT_KEY As String = "GateBesshuDataUllStartReplyLimitTicks"
    Private Const GATE_BESSHU_DATA_ULL_RETRY_INTERVAL_KEY As String = "GateBesshuDataUllRetryIntervalTicks"
    Private Const GATE_BESSHU_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "GateBesshuDataUllMaxRetryCountToForget"
    Private Const GATE_BESSHU_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "GateBesshuDataUllMaxRetryCountToCare"
    Private Const GATE_MEISAI_DATA_ULL_TRANSFER_LIMIT_KEY As String = "GateMeisaiDataUllTransferLimitTicks"
    Private Const GATE_MEISAI_DATA_ULL_START_REPLY_LIMIT_KEY As String = "GateMeisaiDataUllStartReplyLimitTicks"
    Private Const GATE_MEISAI_DATA_ULL_RETRY_INTERVAL_KEY As String = "GateMeisaiDataUllRetryIntervalTicks"
    Private Const GATE_MEISAI_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "GateMeisaiDataUllMaxRetryCountToForget"
    Private Const GATE_MEISAI_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "GateMeisaiDataUllMaxRetryCountToCare"
    Private Const KSB_GATE_FAULT_DATA_ULL_TRANSFER_LIMIT_KEY As String = "KsbGateFaultDataUllTransferLimitTicks"
    Private Const KSB_GATE_FAULT_DATA_ULL_START_REPLY_LIMIT_KEY As String = "KsbGateFaultDataUllStartReplyLimitTicks"
    Private Const KSB_GATE_FAULT_DATA_ULL_RETRY_INTERVAL_KEY As String = "KsbGateFaultDataUllRetryIntervalTicks"
    Private Const KSB_GATE_FAULT_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "KsbGateFaultDataUllMaxRetryCountToForget"
    Private Const KSB_GATE_FAULT_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "KsbGateFaultDataUllMaxRetryCountToCare"
    Private Const GATE_KADO_DATA_ULL_TRANSFER_LIMIT_KEY As String = "GateKadoDataUllTransferLimitTicks"
    Private Const GATE_KADO_DATA_ULL_START_REPLY_LIMIT_KEY As String = "GateKadoDataUllStartReplyLimitTicks"
    Private Const GATE_KADO_DATA_ULL_RETRY_INTERVAL_KEY As String = "GateKadoDataUllRetryIntervalTicks"
    Private Const GATE_KADO_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "GateKadoDataUllMaxRetryCountToForget"
    Private Const GATE_KADO_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "GateKadoDataUllMaxRetryCountToCare"
    Private Const GATE_TRAFFIC_DATA_ULL_TRANSFER_LIMIT_KEY As String = "GateTrafficDataUllTransferLimitTicks"
    Private Const GATE_TRAFFIC_DATA_ULL_START_REPLY_LIMIT_KEY As String = "GateTrafficDataUllStartReplyLimitTicks"
    Private Const GATE_TRAFFIC_DATA_ULL_RETRY_INTERVAL_KEY As String = "GateTrafficDataUllRetryIntervalTicks"
    Private Const GATE_TRAFFIC_DATA_ULL_MAX_RETRY_TO_FORGET_KEY As String = "GateTrafficDataUllMaxRetryCountToForget"
    Private Const GATE_TRAFFIC_DATA_ULL_MAX_RETRY_TO_CARE_KEY As String = "GateTrafficDataUllMaxRetryCountToCare"
    Private Const GATE_PRG_GROUP_TITLES_IN_CAB_KEY As String = "GateProgramGroupTitles"

    ''' <summary>INIファイルから運管サーバの対監視盤通信プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        TelServerAppBaseInit(sIniFilePath, MODEL_NAME, True)

        Dim sAppIdentifier As String = "To" & MODEL_NAME
        Try
            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_SUITE_DLL_TRANSFER_LIMIT_KEY)
            GateMasterSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_SUITE_DLL_START_REPLY_LIMIT_KEY)
            GateMasterSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_SUITE_DLL_RETRY_INTERVAL_KEY)
            GateMasterSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            GateMasterSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_LIST_DLL_TRANSFER_LIMIT_KEY)
            GateMasterListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_LIST_DLL_START_REPLY_LIMIT_KEY)
            GateMasterListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_LIST_DLL_RETRY_INTERVAL_KEY)
            GateMasterListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            GateMasterListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MASTER_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            GateMasterVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY)
            GateProgramSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY)
            GateProgramSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY)
            GateProgramSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            GateProgramSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY)
            GateProgramListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY)
            GateProgramListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY)
            GateProgramListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            GateProgramListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            GateProgramVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_SUITE_DLL_TRANSFER_LIMIT_KEY)
            KsbProgramSuiteDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_SUITE_DLL_START_REPLY_LIMIT_KEY)
            KsbProgramSuiteDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_SUITE_DLL_RETRY_INTERVAL_KEY)
            KsbProgramSuiteDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_SUITE_DLL_MAX_RETRY_TO_CARE_KEY)
            KsbProgramSuiteDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_LIST_DLL_TRANSFER_LIMIT_KEY)
            KsbProgramListDllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_LIST_DLL_START_REPLY_LIMIT_KEY)
            KsbProgramListDllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_LIST_DLL_RETRY_INTERVAL_KEY)
            KsbProgramListDllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_LIST_DLL_MAX_RETRY_TO_CARE_KEY)
            KsbProgramListDllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_PROGRAM_VERINFO_ULL_TRANSFER_LIMIT_KEY)
            KsbProgramVersionInfoUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_TRANSFER_LIMIT_KEY)
            GateBesshuDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_START_REPLY_LIMIT_KEY)
            GateBesshuDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_RETRY_INTERVAL_KEY)
            GateBesshuDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            GateBesshuDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_BESSHU_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            GateBesshuDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_TRANSFER_LIMIT_KEY)
            GateMeisaiDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_START_REPLY_LIMIT_KEY)
            GateMeisaiDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_RETRY_INTERVAL_KEY)
            GateMeisaiDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            GateMeisaiDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_MEISAI_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            GateMeisaiDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_TRANSFER_LIMIT_KEY)
            KsbGateFaultDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_START_REPLY_LIMIT_KEY)
            KsbGateFaultDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_RETRY_INTERVAL_KEY)
            KsbGateFaultDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            KsbGateFaultDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, KSB_GATE_FAULT_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            KsbGateFaultDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_TRANSFER_LIMIT_KEY)
            GateKadoDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_START_REPLY_LIMIT_KEY)
            GateKadoDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_RETRY_INTERVAL_KEY)
            GateKadoDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            GateKadoDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_KADO_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            GateKadoDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_TRANSFER_LIMIT_KEY)
            GateTrafficDataUllTransferLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_START_REPLY_LIMIT_KEY)
            GateTrafficDataUllStartReplyLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_RETRY_INTERVAL_KEY)
            GateTrafficDataUllRetryIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_MAX_RETRY_TO_FORGET_KEY)
            GateTrafficDataUllMaxRetryCountToForget = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, GATE_TRAFFIC_DATA_ULL_MAX_RETRY_TO_CARE_KEY)
            GateTrafficDataUllMaxRetryCountToCare = Integer.Parse(LastReadValue)

            ReadFileElem(PATH_SECTION, GATE_PRG_GROUP_TITLES_IN_CAB_KEY)
            GateProgramGroupTitles = LastReadValue.Split(","c)
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
