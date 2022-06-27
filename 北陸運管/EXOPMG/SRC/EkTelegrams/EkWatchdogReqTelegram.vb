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
''' �E�H�b�`�h�b�OREQ�d���B
''' </summary>
Public Class EkWatchdogReqTelegram
    Inherits EkReqTelegram
    Implements IWatchdogReqTelegram

#Region "�萔"
    Public Const FormalObjCodeInKanshiban As Byte = &H0
    Public Const FormalObjCodeInOpClient As Byte = &H0
    Public Const FormalObjCodeInTokatsu As Byte = &H0
    Public Const FormalObjCodeInMadosho As Byte = &H0

    Private Const ObjDetailLen As Integer = 0
#End Region

#Region "�v���p�e�B"
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal oGene As EkTelegramGene, ByVal objCode As Integer, ByVal replyLimitTicks As Integer)
        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
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

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return EkNakCauseCode.None
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Private Function CreateIAckTelegram() As ITelegram Implements IWatchdogReqTelegram.CreateAckTelegram
        Return New EkWatchdogAckTelegram(Gene, ObjCode)
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As EkWatchdogAckTelegram
        Return New EkWatchdogAckTelegram(Gene, ObjCode)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkWatchdogAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkWatchdogAckTelegram
        Return New EkWatchdogAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
