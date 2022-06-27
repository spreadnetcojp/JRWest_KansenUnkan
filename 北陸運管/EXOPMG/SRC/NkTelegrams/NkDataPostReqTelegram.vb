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

Imports System.IO

Imports JR.ExOpmg.Common

''' <summary>
''' �v���f�[�^���M�d���B
''' </summary>
Public Class NkDataPostReqTelegram
    Inherits NkReqTelegram

#Region "�v���p�e�B"
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal seqCode As NkSeqCode, _
       ByVal aBytes As Byte(), _
       ByVal replyLimitTicks As Integer)

        MyBase.New(seqCode, NkCmdCode.DataPostReq, aBytes.Length, replyLimitTicks)
        Buffer.BlockCopy(aBytes, 0, Me.RawBytes, ObjPos, aBytes.Length)
    End Sub

    Public Sub New( _
       ByVal seqCode As NkSeqCode, _
       ByVal aObjHeaderBytes As Byte(), _
       ByVal oObjFilePathList As List(Of String), _
       ByVal objFilesCombinedLen As Long, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(seqCode, NkCmdCode.DataPostReq, aObjHeaderBytes.Length, oObjFilePathList, objFilesCombinedLen, replyLimitTicks)
        Buffer.BlockCopy(aObjHeaderBytes, 0, Me.RawBytes, ObjPos, aObjHeaderBytes.Length)
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "���\�b�h"
    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If ObjFilePathList IsNot Nothing Then
            If ObjSize < ObjFilesCombinedLen Then
                Log.Error("ObjSize is invalid.")
                Return NakCauseCode.TelegramError
            End If
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return NakCauseCode.None
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram(ByVal status As UShort) As NkDataPostAckTelegram
        Return New NkDataPostAckTelegram(SeqCode, status)
    End Function

    '�n���ꂽ�d����ACK�Ƃ��Đ����������邩���肷�郁�\�b�h
    Public Overrides Function IsValidAck(ByVal iReplyTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        If oReplyTeleg.SeqCode <> SeqCode Then Return False
        If oReplyTeleg.CmdCode <> NkCmdCode.DataPostAck Then Return False
        'NOTE: �K�v�Ȃ�A���̑��̍��ڂ̐������������Ń`�F�b�N�\�ł���B
        '�������A�N���X�̒S���͈͂̈�ѐ����l������Ȃ�AoReplyTeleg.SrcEkCode
        '��Me.DstEkCode�̐������`�F�b�N�Ȃǂ́AClientTelegrapher�̃T�u�N���X��
        '�s���̂��Ó��ł���BProcOnAckTelegramReceive()���t�b�N���āA
        '���M���ɕۑ����Ă�����lastSentDstEkCode�Ɣ�r����΂悢�B
        Return True
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New NkDataPostAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As NkDataPostAckTelegram
        Return New NkDataPostAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
