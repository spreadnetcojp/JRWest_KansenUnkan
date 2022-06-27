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

Imports System.Messaging

Public Class ExtFileCreationNotice
    Inherits Message

    Public Const FormalKind As Integer = 3

    Public Sub New(ByVal oMessage As Message)
        MyBase.New()
        Debug.Assert(oMessage.AppSpecific = FormalKind)
        Me.AppSpecific = FormalKind
        Me.Body = oMessage.Body
    End Sub

    Public Sub New()
        MyBase.New()
        Me.AppSpecific = FormalKind
    End Sub
End Class
