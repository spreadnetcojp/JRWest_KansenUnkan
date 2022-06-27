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
'   0.2      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Net.Mime
Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '窓処操作ログ管理ディレクトリのパス
    Public Shared MadoLogDirPath As String

    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
    '窓処業務前認証ログ管理ディレクトリのパス
    Public Shared MadoCertLogDirPath As String
    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------

    '配信用マスタの保持世代数
    Public Shared MasterDataKeepingGenerations As Integer

    '配信用プログラムの保持世代数
    Public Shared ProgramDataKeepingGenerations As Integer

    '別集札データをデータベース上で保持する日数
    Public Shared BesshuDataVisibleDays As Integer

    '不正乗車券検出データをデータベース上で保持する日数
    Public Shared FuseiJoshaDataVisibleDays As Integer

    '強行突破検出データをデータベース上で保持する日数
    Public Shared KyokoToppaDataVisibleDays As Integer

    '紛失券検出データをデータベース上で保持する日数
    Public Shared FunshitsuDataVisibleDays As Integer

    '異常データをデータベース上で保持する日数
    Public Shared FaultDataVisibleDays As Integer

    '稼動データをデータベース上で保持する日数
    Public Shared KadoDataVisibleDays As Integer

    '保守データをデータベース上で保持する日数
    Public Shared HosyuDataVisibleDays As Integer

    '時間帯別乗降データをデータベース上で保持する日数
    Public Shared TrafficDataVisibleDays As Integer

    '収集データ誤記をデータベース上で保持する日数
    Public Shared CollectedDataTypoVisibleDays As Integer

    '-------Ver0.2 次世代車補対応 ADD START-----------
    '利用データをデータベース上で保持する日数
    Public Shared RiyoDataVisibleDays As Integer

    '新幹線指定券入場データをデータベース上で保持する日数
    Public Shared ShiteiDataVisibleDays As Integer
    '-------Ver0.2 次世代車補対応 ADD END-------------

    '機器接続状態をディレクトリ上で保持する日数
    Public Shared ConStatusKeepingDaysInRejectDir As Integer
    Public Shared ConStatusKeepingDaysInTrashDir As Integer
    Public Shared ConStatusKeepingDaysInQuarantineDir As Integer

    '監視盤設定情報をディレクトリ上で保持する日数
    Public Shared KsbConfigKeepingDaysInRejectDir As Integer
    Public Shared KsbConfigKeepingDaysInTrashDir As Integer
    Public Shared KsbConfigKeepingDaysInQuarantineDir As Integer

    '別集札データをディレクトリ上で保持する日数
    Public Shared BesshuDataKeepingDaysInRejectDir As Integer
    Public Shared BesshuDataKeepingDaysInTrashDir As Integer
    Public Shared BesshuDataKeepingDaysInQuarantineDir As Integer

    '明細系データをディレクトリ上で保持する日数
    Public Shared MeisaiDataKeepingDaysInRejectDir As Integer
    Public Shared MeisaiDataKeepingDaysInTrashDir As Integer
    Public Shared MeisaiDataKeepingDaysInQuarantineDir As Integer

    '異常データをディレクトリ上で保持する日数
    Public Shared FaultDataKeepingDaysInRejectDir As Integer
    Public Shared FaultDataKeepingDaysInTrashDir As Integer
    Public Shared FaultDataKeepingDaysInQuarantineDir As Integer

    '稼動・保守データをディレクトリ上で保持する日数
    Public Shared KadoDataKeepingDaysInRejectDir As Integer
    Public Shared KadoDataKeepingDaysInTrashDir As Integer
    Public Shared KadoDataKeepingDaysInQuarantineDir As Integer

    '時間帯別乗降データをディレクトリ上で保持する日数
    Public Shared TrafficDataKeepingDaysInRejectDir As Integer
    Public Shared TrafficDataKeepingDaysInTrashDir As Integer
    Public Shared TrafficDataKeepingDaysInQuarantineDir As Integer

    '利用データをディレクトリ上で保持する日数
    Public Shared RiyoDataKeepingDaysInRejectDir As Integer
    Public Shared RiyoDataKeepingDaysInTrashDir As Integer

    '窓処操作ログをディレクトリ上で保持する日数
    Public Shared MadoLogsKeepingDays As Integer

    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
    '窓処業務前認証ログをディレクトリ上で保持する日数
    Public Shared MadoCertLogsKeepingDays As Integer
    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------

    '運管サーバ自身のログをディレクトリ上で保持する日数
    Public Shared LogsKeepingDays As Integer

    'INIファイル内のセクション名
    Protected Const STORAGE_LIFE_SECTION As String = "StorageLife"

    'プロセス別キーに対するプレフィックス
    Private Const APP_ID As String = "Sweeper"

    ''' <summary>INIファイルから運管サーバの洗い替えプロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Try
            ReadFileElem(PATH_SECTION, "MadoLogDirPath")
            MadoLogDirPath = LastReadValue

            '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
            ReadFileElem(PATH_SECTION, "MadoCertLogDirPath")
            MadoCertLogDirPath = LastReadValue
            '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------

            ReadFileElem(STORAGE_LIFE_SECTION, "MasterDataKeepingGenerations")
            MasterDataKeepingGenerations = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "ProgramDataKeepingGenerations")
            ProgramDataKeepingGenerations = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "BesshuDataVisibleDays")
            BesshuDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "FuseiJoshaDataVisibleDays")
            FuseiJoshaDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "KyokoToppaDataVisibleDays")
            KyokoToppaDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "FunshitsuDataVisibleDays")
            FunshitsuDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "FaultDataVisibleDays")
            FaultDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "KadoDataVisibleDays")
            KadoDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "HosyuDataVisibleDays")
            HosyuDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "TrafficDataVisibleDays")
            TrafficDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "CollectedDataTypoVisibleDays")
            CollectedDataTypoVisibleDays = Integer.Parse(LastReadValue)

            '-------Ver0.2 次世代車補対応 ADD START-----------
            ReadFileElem(STORAGE_LIFE_SECTION, "RiyoDataVisibleDays")
            RiyoDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "ShiteiDataVisibleDays")
            ShiteiDataVisibleDays = Integer.Parse(LastReadValue)
            '-------Ver0.2 次世代車補対応 ADD END-------------

            ReadFileElem(STORAGE_LIFE_SECTION, "ConStatusKeepingDaysInRejectDir")
            ConStatusKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "ConStatusKeepingDaysInTrashDir")
            ConStatusKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "ConStatusKeepingDaysInQuarantineDir")
            ConStatusKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "KsbConfigKeepingDaysInRejectDir")
            KsbConfigKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "KsbConfigKeepingDaysInTrashDir")
            KsbConfigKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "KsbConfigKeepingDaysInQuarantineDir")
            KsbConfigKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "BesshuDataKeepingDaysInRejectDir")
            BesshuDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "BesshuDataKeepingDaysInTrashDir")
            BesshuDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "BesshuDataKeepingDaysInQuarantineDir")
            BesshuDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "MeisaiDataKeepingDaysInRejectDir")
            MeisaiDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "MeisaiDataKeepingDaysInTrashDir")
            MeisaiDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "MeisaiDataKeepingDaysInQuarantineDir")
            MeisaiDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "FaultDataKeepingDaysInRejectDir")
            FaultDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "FaultDataKeepingDaysInTrashDir")
            FaultDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "FaultDataKeepingDaysInQuarantineDir")
            FaultDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "KadoDataKeepingDaysInRejectDir")
            KadoDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "KadoDataKeepingDaysInTrashDir")
            KadoDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "KadoDataKeepingDaysInQuarantineDir")
            KadoDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "TrafficDataKeepingDaysInRejectDir")
            TrafficDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "TrafficDataKeepingDaysInTrashDir")
            TrafficDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "TrafficDataKeepingDaysInQuarantineDir")
            TrafficDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "RiyoDataKeepingDaysInRejectDir")
            RiyoDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "RiyoDataKeepingDaysInTrashDir")
            RiyoDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "MadoLogsKeepingDays")
            MadoLogsKeepingDays = Integer.Parse(LastReadValue)

            '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
            ReadFileElem(STORAGE_LIFE_SECTION, "MadoCertLogsKeepingDays")
            MadoCertLogsKeepingDays = Integer.Parse(LastReadValue)
            '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------

            ReadFileElem(STORAGE_LIFE_SECTION, "LogsKeepingDays")
            LogsKeepingDays = Integer.Parse(LastReadValue)
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
