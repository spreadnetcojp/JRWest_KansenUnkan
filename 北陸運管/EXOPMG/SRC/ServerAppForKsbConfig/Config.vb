' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/06/01       金沢  北陸・項目拡張対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits RecServerAppBaseConfig

    '登録対象データ書式ファイルのパス
    Public Shared FormatFilePath As String
    '----------- 0.1  北陸・項目拡張対応   ADD  START------------------------
    Public Shared FormatOldFilePath As String
    '----------- 0.1  北陸・項目拡張対応   ADD    END------------------------

    'データ別キーに対するプレフィックス
    Private Const DATA_NAME As String = "KsbConfig"

    'INIファイル内における各設定項目のキー
    'Private Const FOO_BAR_KEY As String = DATA_NAME & "FooBar"

    ''' <summary>INIファイルから運管サーバの監視盤設定データ登録プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        RecServerAppBaseInit(sIniFilePath, DATA_NAME)

        Try
            ReadFileElem(PATH_SECTION, "KsbConfigFormatFilePath")
            FormatFilePath = LastReadValue
            '----------- 0.1  北陸・項目拡張対応   ADD  START------------------------
            ReadFileElem(PATH_SECTION, "KsbConfigOldFormatFilePath")
            FormatOldFilePath = LastReadValue
            '----------- 0.1  北陸・項目拡張対応   ADD    END------------------------
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class

