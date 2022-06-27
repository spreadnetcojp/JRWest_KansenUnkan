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

Public Class ExtMasProDllRequest
    Inherits Message

    Public Const FormalKind As Integer = 1

    Public ReadOnly Property ListFileName() As String
        Get
            Dim sBody As String = Body.ToString()
            Dim pos As Integer = sBody.IndexOf(":"c)
            Return sBody.Substring(0, pos)
        End Get
    End Property

    Public ReadOnly Property ForcingFlag() As Boolean
        Get
            Dim sBody As String = Body.ToString()
            Dim pos As Integer = sBody.IndexOf(":"c) + 1
            Return Not sBody.Substring(pos, 1).Equals("0")
        End Get
    End Property

    Public Sub New(ByVal oMessage As Message)
        MyBase.New()
        Debug.Assert(oMessage.AppSpecific = FormalKind)
        Me.AppSpecific = FormalKind
        Me.Body = oMessage.Body
    End Sub

    Public Sub New(ByVal sListFileName As String, ByVal forcingFlag As Boolean)
        MyBase.New()
        Me.AppSpecific = FormalKind
        Me.Body = sListFileName & ":" & If(forcingFlag = False, "0", "1")
    End Sub
End Class
