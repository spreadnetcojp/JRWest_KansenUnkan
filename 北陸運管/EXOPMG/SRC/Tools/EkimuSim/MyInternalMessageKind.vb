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

''' <summary>
''' 駅務機器共通シミュレータ用内部メッセージの種別。
''' </summary>
Public Class MyInternalMessageKind
    Inherits InternalMessageKind

    Public Const ActiveOneExecRequest As Integer = AppDefinitionBase
    Public Const ActiveUllExecRequest As Integer = AppDefinitionBase + 1
    Public Const ComStartExecRequest As Integer = AppDefinitionBase + 2
    Public Const TimeDataGetExecRequest As Integer = AppDefinitionBase + 3
End Class
