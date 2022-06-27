' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
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
