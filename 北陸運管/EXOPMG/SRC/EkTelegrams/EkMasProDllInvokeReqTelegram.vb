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

Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �^�ǃT�[�o�Ɖ^�ǒ[���̊Ԃ̔z�M�w��REQ�d���B
''' </summary>
Public Class EkMasProDllInvokeReqTelegram
    Inherits EkReqTelegram

#Region "�萔"
    Public Const FormalObjCode As Byte = &H10

    Private Const ListFileNamePos As Integer = 0
    Private Const ListFileNameLen As Integer = 80
    Private Const ForcingFlagPos As Integer = ListFileNamePos + ListFileNameLen
    Private Const ForcingFlagLen As Integer = 1
    Private Const ObjDetailLen As Integer = ForcingFlagPos + ForcingFlagLen
#End Region

#Region "�v���p�e�B"
    Public Property ListFileName() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(ListFileNamePos), ListFileNameLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal listFileName As String)
            Array.Clear(RawBytes, GetRawPos(ListFileNamePos), ListFileNameLen)
            Encoding.UTF8.GetBytes(listFileName, 0, listFileName.Length, RawBytes, GetRawPos(ListFileNamePos))
        End Set
    End Property

    Public Property ForcingFlag() As Boolean
        Get
            Return RawBytes(GetRawPos(ForcingFlagPos)) <> 0
        End Get

        Set(ByVal flag As Boolean)
            RawBytes(GetRawPos(ForcingFlagPos)) = If(flag, CByte(1), CByte(0))
        End Set
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal listFileName As String, _
       ByVal forcingFlag As Boolean, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.ListFileName = listFileName
        Me.ForcingFlag = forcingFlag
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "���\�b�h"
    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If GetObjDetailLen() <> ObjDetailLen Then
            Log.Error("ObjSize is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If RawBytes(GetRawPos(ForcingFlagPos)) > 1 Then
            Log.Error("ForcingFlag is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(ListFileNamePos), ListFileNameLen) Then
            Log.Error("ListFileName is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return EkNakCauseCode.None
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As EkMasProDllInvokeAckTelegram
        Return New EkMasProDllInvokeAckTelegram(Gene, ObjCode)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkMasProDllInvokeAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Shadows Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkMasProDllInvokeAckTelegram
        Return New EkMasProDllInvokeAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
