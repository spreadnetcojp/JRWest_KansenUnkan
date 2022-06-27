' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/04/10  (NES)����  ������ԕ�Ή��ɂĐV�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Messaging

Public Class ExtAlertMailSendRequest
    Inherits Message

    Public Const FormalKind As Integer = 6

    Public ReadOnly Property MailTitle() As String
        Get
            Return CType(Body, ExtAlertMailSendRequestBody).Title
        End Get
    End Property

    Public ReadOnly Property MailBody() As String
        Get
            Return CType(Body, ExtAlertMailSendRequestBody).Body
        End Get
    End Property

    Public Sub New(ByVal oMessage As Message)
        MyBase.New()
        Debug.Assert(oMessage.AppSpecific = FormalKind)
        Me.AppSpecific = FormalKind
        Me.Body = oMessage.Body
    End Sub

    Public Sub New(ByVal sMailTitle As String, ByVal sMailBody As String)
        MyBase.New()
        Me.AppSpecific = FormalKind
        Me.Body = New ExtAlertMailSendRequestBody(sMailTitle, sMailBody)
    End Sub
End Class

<Serializable()> _
Public Structure ExtAlertMailSendRequestBody
    Public Title As String
    Public Body As String
    Public Sub New(ByVal sTitle As String, ByVal sBody As String)
        Title = sTitle
        Body = sBody
    End Sub
End Structure
