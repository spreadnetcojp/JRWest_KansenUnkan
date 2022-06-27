' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2016/06/01  (NES)����  TestData.Get��len�Z�o�����C��
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO

Imports JR.ExOpmg.Common

''' <summary>
''' �܂�Ԃ��V�[�P���X�̗v���f�[�^���M�d���B
''' </summary>
Public Class NkTestReqTelegram
    Inherits NkReqTelegram

#Region "�v���p�e�B"
    Public ReadOnly Property TestData() As Byte()
        Get
            Dim len As Integer = CInt(ObjSize)
            If len = 0 Then Return Nothing
            Dim aBytes As Byte() = New Byte(len - 1) {}
            Buffer.BlockCopy(RawBytes, ObjPos, aBytes, 0, len)
            Return aBytes
        End Get
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal aTestData As Byte(), ByVal replyLimitTicks As Integer)
        MyBase.New(NkSeqCode.Test, NkCmdCode.InquiryReq, If(aTestData Is Nothing, 0, aTestData.Length), replyLimitTicks)
        If aTestData IsNot Nothing Then
            Buffer.BlockCopy(aTestData, 0, Me.RawBytes, ObjPos, aTestData.Length)
        End If
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "���\�b�h"
    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return NakCauseCode.None
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As NkTestAckTelegram
        Return New NkTestAckTelegram(TestData)
    End Function

    '�n���ꂽ�d����ACK�Ƃ��Đ����������邩���肷�郁�\�b�h
    Public Overrides Function IsValidAck(ByVal iReplyTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        If oReplyTeleg.SeqCode <> NkSeqCode.Test Then Return False
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
        Return New NkTestAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As NkTestAckTelegram
        Return New NkTestAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
