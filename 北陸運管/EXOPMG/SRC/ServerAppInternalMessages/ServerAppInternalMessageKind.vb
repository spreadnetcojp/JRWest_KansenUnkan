' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応にてNameChangeNoticeを追加
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' 運管サーバプロセス用内部メッセージの種別。
''' </summary>
Public Class ServerAppInternalMessageKind
    Inherits InternalMessageKind

    Public Const MasProDllRequest As Integer = AppDefinitionBase
    Public Const MasProDllResponse As Integer = AppDefinitionBase + 1
    Public Const ScheduledUllRequest As Integer = AppDefinitionBase + 2
    Public Const ScheduledUllResponse As Integer = AppDefinitionBase + 3
    Public Const TallyTimeNotice As Integer = AppDefinitionBase + 4
    Public Const NameChangeNotice As Integer = AppDefinitionBase + 5
End Class
