' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/06/07  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '常駐プロセスの停止許容時間
    Public Shared ResidentAppPendingLimitTicks As Integer

    'プロセス別キーに対するプレフィックス
    Private Const APP_ID As String = "Manager"

    'INIファイル内における各設定項目のキー
    Private Const RESIDENT_APP_PENDING_LIMIT_KEY As String = "ResidentAppPendingLimitTicks"

    ''' <summary>INIファイルから運管サーバのプロセスマネージャに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Try
            ReadFileElem(TIME_INFO_SECTION, RESIDENT_APP_PENDING_LIMIT_KEY)
            ResidentAppPendingLimitTicks = Integer.Parse(LastReadValue)
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
