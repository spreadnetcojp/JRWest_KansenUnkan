' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2014/04/20  (NES)      新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits BaseConfig

    '有効ログ種別
    Public Shared LogKindsMask As Integer

    'ログを保持する日数
    Public Shared LogsKeepingDays As Integer

    '装置種別（ウィンドウタイトルに表示する名称）
    Public Shared MachineKind As String

    'ウィンドウタイトルに表示するバージョン番号
    Public Shared VerNoSet As String

    'INIファイル内のセクション名
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const STORAGE_LIFE_SECTION As String = "StorageLife"

    'INIファイル内における各設定項目のキー
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const LOGS_KEEPING_DAYS_KEY As String = "LogsKeepingDays"
    Private Const MACHINE_KIND_KEY As String = "MachineKind"
    Private Const VER_NO_SET_KEY As String = "VerNoSet"

    ''' <summary>INIファイルからマスタ変換ツールに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Try
            ReadFileElem(CREDENTIAL_SECTION, MACHINE_KIND_KEY)
            MachineKind = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, VER_NO_SET_KEY)
            VerNoSet = LastReadValue

            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, LOGS_KEEPING_DAYS_KEY)
            LogsKeepingDays = Integer.Parse(LastReadValue)

        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
