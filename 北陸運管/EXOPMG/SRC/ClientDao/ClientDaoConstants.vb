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
''' ClientDao共通の定数等を定義するクラス。
''' </summary>
Public Class ClientDaoConstants

    'Select All
    Public Const TERMINAL_ALL As String = "TerminalAll"

    '全駅のグループNo定義
    Public Const TERMINAL_ALL_GrpNo As Integer = 0

End Class
