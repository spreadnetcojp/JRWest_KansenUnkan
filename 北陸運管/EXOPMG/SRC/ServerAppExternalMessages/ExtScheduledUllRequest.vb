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

Public Class ExtScheduledUllRequest
    Inherits Message

    Public Const FormalKind As Integer = 2

    Public ReadOnly Property FileName() As String
        Get
            Return Body.ToString()
        End Get
    End Property

    Public Sub New(ByVal oMessage As Message)
        MyBase.New()
        Debug.Assert(oMessage.AppSpecific = FormalKind)
        Me.AppSpecific = FormalKind
        Me.Body = oMessage.Body
    End Sub

    Public Sub New(ByVal sFileName As String)
        MyBase.New()
        Me.AppSpecific = FormalKind
        Me.Body = sFileName
    End Sub
End Class
