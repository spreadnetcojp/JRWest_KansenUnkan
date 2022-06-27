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
''' �^�ǃT�[�o�Ƒ����̊Ԃ̃��O�i���샍�O�E�Ɩ��O�F�؃��O�j���tACK�d���B
''' </summary>
Public Class EkMadoLogPostAckTelegram
    Inherits EkTelegram

#Region "�萔"
    Private Const LogDataDecisionCodePos As Integer = 0
    Private Const LogDataDecisionCodeLen As Integer = 1
    Private Const ObjDetailLen As Integer = LogDataDecisionCodePos + LogDataDecisionCodeLen
#End Region

#Region "�v���p�e�B"
    Public Property LogDataDecisionCode() As Integer
        Get
            Return RawBytes(GetRawPos(LogDataDecisionCodePos))
        End Get

        Set(ByVal decisionCode As Integer)
            RawBytes(GetRawPos(LogDataDecisionCodePos)) = CByte(decisionCode)
        End Set
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal oGene As EkTelegramGene, ByVal objCode As Integer, ByVal decisionCode As Integer)
        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Post, objCode, ObjDetailLen)

        Me.LogDataDecisionCode = decisionCode
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
#End Region

End Class
