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

''' <summary>
''' 登録系処理の定数を定義するクラス。
''' </summary>
Public Class RecAppConstants

    'ログ文言（定義ファイル関連）
    Public Const ERR_INI_FILE_NOT_FOUND As String = "書式定義ファイル[{0}]が存在しません。"
    Public Const ERR_BAD_INI_FILE As String = "書式定義ファイルの内容が不正です。"

    'ログ文言（1回目の解析）
    Public Const ERR_TOO_SHORT_FILE As String = "ファイルが1レコード未満の長さです。"
    Public Const ERR_INVALID_FIELD_AS_BCD As String = "{0}がBCDとみなせません。"
    Public Const ERR_INVALID_RECORD As String = "レコード[{0}]は不正なため登録しません。"
    Public Const ERR_FILE_ROUNDED_OFF As String = "ファイルの長さに端数があります。"

    'ログ文言（2回目の解析）
    'NOTE: 「n行目」の「n」は、１回目の解析で残ったレコードを1起点で計上した番号である。
    Public Const ERR_MSG_NOVALUE As String = "{0}行目の{1}がありません。"
    Public Const ERR_MSG_ERRVALUE As String = "{0}行目の{1}が不正です。"
    Public Const ERR_MACHINE_NOVALUE As String = "機器が存在しません。(線区:{0} 駅順:{1} コーナ:{2} 号機:{3})"

End Class
