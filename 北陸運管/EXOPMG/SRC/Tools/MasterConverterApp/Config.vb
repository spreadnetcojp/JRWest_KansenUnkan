' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/03/01  (NES)小林  新規作成
'   0.2      2014/06/12  (NES)田保  北陸対応
'                                   ・ツールVerのタイトル表示対応
'                                   ・マスタ別パターン番号チェック対応
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

    'Ver0.2 ADD START  北陸対応
    Public Shared MachineKind As String
    Public Shared VerNoSet As String
    Public Shared LimitPatterns As ArrayList = New ArrayList()
    'Ver0.2 ADD END    北陸対応

    'INIファイル内のセクション名
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const STORAGE_LIFE_SECTION As String = "StorageLife"
    'Ver0.2 ADD START  北陸対応
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const MST_INPUT_CHECK_SECTION As String = "MstInputCheck"
    'Ver0.2 ADD END    北陸対応

    'INIファイル内における各設定項目のキー
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const LOGS_KEEPING_DAYS_KEY As String = "LogsKeepingDays"
    'Ver0.2 ADD START  北陸対応
    Private Const MACHINE_KIND_KEY As String = "MachineKind"
    Private Const VER_N_OSET As String = "VerNoSet"
    Private Const LIMI_TPATTERNS_KEY As String = "LimitPattern_"
    'Ver0.2 ADD END    北陸対応

    ''' <summary>INIファイルからマスタ変換ツールに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Dim i As Integer
        'Ver0.2 ADD START  北陸対応
        Dim workString() As String
        Dim subList As ArrayList
        'Ver0.2 ADD END    北陸対応

        Try
            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, LOGS_KEEPING_DAYS_KEY)
            LogsKeepingDays = Integer.Parse(LastReadValue)


            'Ver0.2 ADD START  北陸対応
            'ツールタイトル
            Try
                ReadFileElem(CREDENTIAL_SECTION, MACHINE_KIND_KEY)
                MachineKind = LastReadValue
                If MachineKind.Length = 0 Then
                    MachineKind = "駅務機器マスタ変換"
                End If
            Catch ex As Exception
                MachineKind = "駅務機器マスタ変換"
            End Try
            'バージョン
            Try
                ReadFileElem(CREDENTIAL_SECTION, VER_N_OSET)
                VerNoSet = LastReadValue
            Catch ex As Exception
                VerNoSet = ""   'バージョン指定が無ければバージョン非表示
            End Try


            'マスタ別のパターン番号チェック表読み込み
            For i = 0 To 99
                Try
                    ReadFileElem(MST_INPUT_CHECK_SECTION, LIMI_TPATTERNS_KEY & i)
                    workString = Split(LastReadValue, ",")
                    subList = New ArrayList()
                    subList.Add(workString(0))
                    subList.Add(Integer.Parse(workString(1)))
                    subList.Add(Integer.Parse(workString(2)))
                    LimitPatterns.Add(subList)
                Catch ex As Exception
                    Exit For
                End Try
            Next
            'Ver0.2 ADD END    北陸対応
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
