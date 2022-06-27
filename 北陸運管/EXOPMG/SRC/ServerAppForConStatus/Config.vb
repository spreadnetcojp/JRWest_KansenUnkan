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
    Inherits RecServerAppBaseConfig

    '登録対象データ書式ファイルのパス
    Public Shared FormatFilePath_G As String
    Public Shared FormatFilePath_Y As String
    Public Shared FormatFilePath_X As String

    '入力データ別（プロセス別）キーに対するプレフィックス
    Private Const DATA_NAME As String = "ConStatus"

    ''' <summary>INIファイルから運管サーバの機器接続状態登録プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        RecServerAppBaseInit(sIniFilePath, DATA_NAME)

        Try
            ReadFileElem(PATH_SECTION, "ConStatusFormatFilePath_G")
            FormatFilePath_G = LastReadValue

            ReadFileElem(PATH_SECTION, "ConStatusFormatFilePath_Y")
            FormatFilePath_Y = LastReadValue

            ReadFileElem(PATH_SECTION, "ConStatusFormatFilePath_X")
            FormatFilePath_X = LastReadValue

        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
