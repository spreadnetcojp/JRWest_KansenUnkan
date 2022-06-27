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

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' 運管サーバ全プロセス共通のメイン処理を実装するクラス。
''' </summary>
Public Class ServerAppBaseMainClass

#Region "定数や変数"
    'ログファイル出力先ディレクトリ指定用環境変数の名前
    Protected Const REG_LOG As String = "EXOPMG_LOG_DIR"

    'サーバ用INIファイル指定用環境変数の名前
    Protected Const REG_SERVER_INI As String = "EXOPMG_INIFILE_SERVER"
#End Region

#Region "メソッド"
    ''' <summary>
    ''' 全プロセスの共通メイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 各プロセスのメイン処理から呼び出す。
    ''' </remarks>
    Protected Shared Sub ServerAppBaseMain(ByVal oForm As ServerAppForm)
        Try
            '画面を表示する（UI用メッセージループ実行する）。
            Log.Info("画面表示処理開始")
            oForm.ShowDialog()
            Log.Info("画面表示処理終了")
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub
#End Region

End Class
