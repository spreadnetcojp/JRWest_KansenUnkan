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

Imports System.Globalization

Imports JR.ExOpmg.Common

''' <summary>
''' ����M�ۉ����d���B
''' </summary>
Public Class NkInquiryAckTelegram
    Inherits NkTelegram

#Region "�萔"
    Private Const ReturnStatusPos As Integer = ObjPos
    Private Const ReturnStatusLen As Integer = 2
    Private Const ReservedArea1Pos As Integer = ReturnStatusPos + ReturnStatusLen
    Private Const ReservedArea1Len As Integer = 2
    Private Const ObjLen As Integer = ReservedArea1Pos + ReservedArea1Len - ObjPos
#End Region

#Region "�v���p�e�B"
    Public Property ReturnStatus() As UShort
        Get
            Return Utility.GetUInt16FromLeBytes2(RawBytes, ReturnStatusPos)
        End Get

        Set(ByVal status As UShort)
            Utility.CopyUInt16ToLeBytes2(status, RawBytes, ReturnStatusPos)
        End Set
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal seqCode As NkSeqCode, ByVal status As UShort)
        MyBase.New(seqCode, NkCmdCode.InquiryAck, ObjLen)
        Me.ReturnStatus = status
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "���\�b�h"
    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If ObjSize <> ObjLen Then
            Log.Error("ObjSize is invalid.")
            Return NakCauseCode.TelegramError
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return NakCauseCode.None
    End Function
#End Region

End Class
