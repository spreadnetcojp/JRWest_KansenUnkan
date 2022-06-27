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
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �C�ӂ�REQ�d���B
''' </summary>
Public Class EkAnonyReqTelegram
    Inherits EkReqTelegram

#Region "�萔"
#End Region

#Region "�v���p�e�B"
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal oTeleg As ITelegram, ByVal replyLimitTicks As Integer)
        MyBase.New(oTeleg)
        Me.ReplyLimitTicks = replyLimitTicks
    End Sub
#End Region

#Region "���\�b�h"
    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkAnonyAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
