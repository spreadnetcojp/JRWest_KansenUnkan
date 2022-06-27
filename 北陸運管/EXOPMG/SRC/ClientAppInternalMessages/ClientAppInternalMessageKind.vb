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
''' 運管端末プロセス用内部メッセージの種別。
''' </summary>
Public Class ClientAppInternalMessageKind
    Inherits InternalMessageKind

    Public Const MasProUllRequest As Integer = AppDefinitionBase
    Public Const MasProUllResponse As Integer = AppDefinitionBase + 1
    Public Const MasProDllInvokeRequest As Integer = AppDefinitionBase + 2
    Public Const MasProDllInvokeResponse As Integer = AppDefinitionBase + 3
End Class
