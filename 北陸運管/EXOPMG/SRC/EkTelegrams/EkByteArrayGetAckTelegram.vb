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
''' �C�Ӄo�C�g��擾ACK�d���B
''' </summary>
Public Class EkByteArrayGetAckTelegram
    Inherits EkTelegram

#Region "�萔"
    Private Const ByteArrayPos As Integer = 0
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ByteArray() As Byte()
        Get
            Dim len As Integer = GetObjDetailLen() - ByteArrayPos
            Dim aBytes As Byte() = New Byte(len - 1) {}
            Buffer.BlockCopy(RawBytes, GetRawPos(ByteArrayPos), aBytes, 0, len)
            Return aBytes
        End Get
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal aBytes As Byte())
        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Get, objCode, aBytes.Length)
        Buffer.BlockCopy(aBytes, 0, Me.RawBytes, GetRawPos(ByteArrayPos), aBytes.Length)
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "���\�b�h"
    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        'If GetObjDetailLen() < 1 Then
        '    Log.Error("ObjSize is invalid.")
        '    Return EkNakCauseCode.TelegramError
        'End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return EkNakCauseCode.None
    End Function
#End Region

End Class
