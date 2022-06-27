' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/01/14  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' 多重駅務機器共通シミュレータ用内部メッセージの種別。
''' </summary>
Public Class MyInternalMessageKind
    Inherits InternalMessageKind

    Public Const ConnectRequest As Integer = AppDefinitionBase
    Public Const ActiveOneExecRequest As Integer = AppDefinitionBase + 1
    Public Const ActiveUllExecRequest As Integer = AppDefinitionBase + 2
    Public Const ComStartExecRequest As Integer = AppDefinitionBase + 3
    Public Const TimeDataGetExecRequest As Integer = AppDefinitionBase + 4
    Public Const ScenarioStartRequest As Integer = AppDefinitionBase + 5
    Public Const ScenarioStopRequest As Integer = AppDefinitionBase + 6
    Public Const AppFuncEndNotice As Integer = AppDefinitionBase + 7
End Class
