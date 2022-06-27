' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/04/10  (NES)小林  次世代車補対応にて新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '登録スレッド停止許容時間
    Public Shared RecorderPendingLimitTicks As Integer

    '登録実行周期
    Public Shared RecordingIntervalTicks As Integer

    '１トランザクションで登録する最大ファイル数
    Public Shared RecordingFileCountAtOnce As Integer

    '読み出し対象メッセージキューの名前
    Public Shared MyMqPath As String

    '利用データのフォーマットファイルやSQLファイルの格納場所
    Public Shared RiyoDataImporterFilesBasePath As String

    'プロセス別キーに対するプレフィックス
    Private Const APP_ID As String = "ForRiyoData"

    ''' <summary>INIファイルから運管サーバの利用データ登録プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Try
            ReadFileElem(TIME_INFO_SECTION, APP_ID & "RecorderPendingLimitTicks")
            RecorderPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, APP_ID & "RecordingIntervalTicks")
            RecordingIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, APP_ID & "RecordingFileCountAtOnce")
            RecordingFileCountAtOnce = Integer.Parse(LastReadValue)

            ReadFileElem(MQ_SECTION, APP_ID & MQ_PATH_KEY)
            MyMqPath = LastReadValue

            ReadFileElem(PATH_SECTION, "RiyoDataImporterFilesBasePath")
            RiyoDataImporterFilesBasePath = LastReadValue

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
