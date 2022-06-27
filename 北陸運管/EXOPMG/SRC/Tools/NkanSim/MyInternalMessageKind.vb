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
''' Ｎ間共通シミュレータ用内部メッセージの種別。
''' </summary>
Public Class MyInternalMessageKind
    Inherits InternalMessageKind

    Public Const ActiveOneExecRequest As Integer = AppDefinitionBase
    Public Const ComStartExecRequest As Integer = AppDefinitionBase + 1
    Public Const InquiryExecRequest As Integer = AppDefinitionBase + 2
End Class
