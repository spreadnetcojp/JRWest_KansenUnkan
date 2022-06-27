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

Imports JR.ExOpmg.Common

''' <summary>
''' �ؒf�v���d���B
''' </summary>
Public Class NkComStopReqTelegram
    Inherits NkReqTelegram

#Region "�萔"
    Private Const ObjLen As Integer = 0
#End Region

#Region "�v���p�e�B"
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal seqCode As NkSeqCode, ByVal replyLimitTicks As Integer)
        MyBase.New(seqCode, NkCmdCode.ComStopReq, ObjLen, replyLimitTicks)
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

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As NkComStopAckTelegram
        Return New NkComStopAckTelegram(SeqCode)
    End Function

    '�n���ꂽ�d����ACK�Ƃ��Đ����������邩���肷�郁�\�b�h
    Public Overrides Function IsValidAck(ByVal iReplyTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        If oReplyTeleg.SeqCode <> SeqCode Then Return False
        If oReplyTeleg.CmdCode <> NkCmdCode.ComStopAck Then Return False
        'NOTE: �K�v�Ȃ�A���̑��̍��ڂ̐������������Ń`�F�b�N�\�ł���B
        '�������A�N���X�̒S���͈͂̈�ѐ����l������Ȃ�AoReplyTeleg.SrcEkCode
        '��Me.DstEkCode�̐������`�F�b�N�Ȃǂ́AClientTelegrapher�̃T�u�N���X��
        '�s���̂��Ó��ł���BProcOnAckTelegramReceive()���t�b�N���āA
        '���M���ɕۑ����Ă�����lastSentDstEkCode�Ɣ�r����΂悢�B
        Return True
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New NkComStopAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As NkComStopAckTelegram
        Return New NkComStopAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
