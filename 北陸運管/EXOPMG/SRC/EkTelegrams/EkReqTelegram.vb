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
''' REQ�d���B
''' </summary>
''' <remarks>
''' �����܂ŁA�e��REQ�d���N���X�̎����̈ꕔ���s���邽�߂̃N���X�ł���B
''' </remarks>
Public MustInherit Class EkReqTelegram
    Inherits EkTelegram
    Implements IReqTelegram

#Region "�ϐ�"
    Private _ReplyLimitTicks As Integer
#End Region

#Region "�v���p�e�B"
    Private ReadOnly Property __ReplyLimitTicks() As Integer Implements IReqTelegram.ReplyLimitTicks
        Get
            Return _ReplyLimitTicks
        End Get
    End Property

    Public Property ReplyLimitTicks() As Integer
        Get
            Return _ReplyLimitTicks
        End Get

        Set(ByVal ticks As Integer)
            _ReplyLimitTicks = ticks
        End Set
    End Property
#End Region

#Region "�R���X�g���N�^�i�T�u�N���X�̃R���X�g���N�^�̎����p�j"
    Protected Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal cmdCode As EkCmdCode, _
       ByVal subCmdCode As EkSubCmdCode, _
       ByVal objCode As Integer, _
       ByVal objDetailLen As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, cmdCode, subCmdCode, objCode, objDetailLen)
        Me._ReplyLimitTicks = replyLimitTicks
    End Sub

    'iTeleg�̎��̂�EkTelegram�ł��邱�Ƃ�O��Ƃ��郁�\�b�h�ł��B
    '������g�����������ꍇ�́AInvalidCastException���X���[����܂��B
    Protected Sub New(ByVal oGene As EkTelegramGene, ByVal iTeleg As ITelegram)
        MyBase.New(oGene, iTeleg)
        If TypeOf iTeleg Is EkReqTelegram Then
            Me._ReplyLimitTicks = DirectCast(iTeleg, EkReqTelegram)._ReplyLimitTicks
        Else
            'NOTE: iTeleg����M�d���iEkDodgyTelegram�j�̏ꍇ�́A������̃P�[�X
            '�Ƃ��ď������s���邪�A����͈Ӑ}�ʂ�ł���B
            '���̃v���g�R���ł́A�d���ɉ�����M�����ɑ���������͊i�[����Ă��Ȃ�
            '�i�����炱���A���̂悤�Ȑ�p�����o�ɕʓr�ݒ肷�邱�ƂɂȂ��Ă���j�B
            '���Ȃ킿�AReplyLimitTicks�v���p�e�B�́AREQ�d���𑗐M���鑤�ł݈̂Ӗ������B
            Me._ReplyLimitTicks = 0
        End If
    End Sub

    'iTeleg�̎��̂�EkTelegram�ł��邱�Ƃ�O��Ƃ��郁�\�b�h�ł��B
    '������g�����������ꍇ�́AInvalidCastException���X���[����܂��B
    Protected Sub New(ByVal iTeleg As ITelegram)
        MyBase.New(iTeleg)
        If TypeOf iTeleg Is EkReqTelegram Then
            Me._ReplyLimitTicks = DirectCast(iTeleg, EkReqTelegram)._ReplyLimitTicks
        Else
            'NOTE: iTeleg����M�d���iEkDodgyTelegram�j�̏ꍇ�́A������̃P�[�X
            '�Ƃ��ď������s���邪�A����͈Ӑ}�ʂ�ł���B
            '���̃v���g�R���ł́A�d���ɉ�����M�����ɑ���������͊i�[����Ă��Ȃ�
            '�i�����炱���A���̂悤�Ȑ�p�����o�ɕʓr�ݒ肷�邱�ƂɂȂ��Ă���j�B
            '���Ȃ킿�AReplyLimitTicks�v���p�e�B�́AREQ�d���𑗐M���鑤�ł݈̂Ӗ������B
            Me._ReplyLimitTicks = 0
        End If
    End Sub
#End Region

#Region "���\�b�h"
    '�n���ꂽ�d����ACK�Ƃ��Đ����������邩���肷�郁�\�b�h
    Public Function IsValidAck(ByVal iReplyTeleg As ITelegram) As Boolean Implements IReqTelegram.IsValidAck
        Dim oReplyTeleg As EkTelegram = DirectCast(iReplyTeleg, EkTelegram)
        If oReplyTeleg.SubCmdCode <> SubCmdCode Then Return False
        If oReplyTeleg.ObjCode <> ObjCode Then Return False
        'NOTE: �K�v�Ȃ�A���̑��̍��ڂ̐������������Ń`�F�b�N�\�ł���B
        '�������A�N���X�̒S���͈͂̈�ѐ����l������Ȃ�AReqNumber��
        'ClientCode�̃`�F�b�N�́AServerTelegrapher�i�܂���ClientTelegrapher�j
        '�̃T�u�N���X�ōs���̂��Ó��ł���BProcOnAckTelegramReceive()��
        '�t�b�N���āA�N���X���ŊǗ�����lastSentReqNumber�Ɣ�r����΂悢�B
        Return True
    End Function

    '�n���ꂽ�d����NAK�Ƃ��Đ����������邩���肷�郁�\�b�h
    Public Function IsValidNak(ByVal iReplyTeleg As ITelegram) As Boolean Implements IReqTelegram.IsValidNak
        Dim oReplyTeleg As EkTelegram = DirectCast(iReplyTeleg, EkTelegram)
        If oReplyTeleg.SubCmdCode <> SubCmdCode Then Return False
        If oReplyTeleg.ObjCode <> ObjCode Then Return False
        'NOTE: �K�v�Ȃ�A���̑��̍��ڂ̐������������Ń`�F�b�N�\�ł���B
        '�������A�N���X�̒S���͈͂̈�ѐ����l������Ȃ�AReqNumber��
        'ClientCode�̃`�F�b�N�́AServerTelegrapher�i�܂���ClientTelegrapher�j
        '�̃T�u�N���X�ōs���̂��Ó��ł���BProcOnNakTelegramReceive()��
        '�t�b�N���āA�N���X���ŊǗ�����lastSentReqNumber�Ɣ�r����΂悢�B
        Return True
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected MustOverride Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram Implements IReqTelegram.ParseAsAck

    '�n���ꂽ�d���̌^��NAK�d���̌^�ɕϊ����郁�\�b�h
    Private Function ParseAsINak(ByVal oReplyTeleg As ITelegram) As INakTelegram Implements IReqTelegram.ParseAsNak
        Return New EkNakTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��NAK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsNak(ByVal oReplyTeleg As ITelegram) As EkNakTelegram
        Return New EkNakTelegram(oReplyTeleg)
    End Function
#End Region

End Class
