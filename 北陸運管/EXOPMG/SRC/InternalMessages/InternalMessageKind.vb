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

''' <summary>
''' 内部メッセージの種別。
''' </summary>
Public Class InternalMessageKind
    Public Const QuitRequest As Integer = 32

    Public Const ConnectNotice As Integer = 64
    Public Const DisconnectRequest As Integer = 65

    Public Const DownloadRequest As Integer = 96
    Public Const DownloadResponse As Integer = 97
    Public Const UploadRequest As Integer = 98
    Public Const UploadResponse As Integer = 99

    Public Const AppDefinitionBase As Integer = 1024
End Class
