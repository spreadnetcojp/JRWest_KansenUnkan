' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits RecServerAppBaseConfig

    '登録対象データ書式ファイルのパス
    Public Shared FormatFilePath As String

    '入力データ別（プロセス別）キーに対するプレフィックス
    Private Const DATA_NAME As String = "TrafficData"

    ''' <summary>INIファイルから運管サーバの時間帯別乗降データ登録プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        RecServerAppBaseInit(sIniFilePath, DATA_NAME)

        Try
            ReadFileElem(PATH_SECTION, "TrafficDataFormatFilePath")
            FormatFilePath = LastReadValue

        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
