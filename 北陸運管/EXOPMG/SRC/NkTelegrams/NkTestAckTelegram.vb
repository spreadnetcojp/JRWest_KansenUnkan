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

Imports System.Globalization

Imports JR.ExOpmg.Common

''' <summary>
''' �܂�Ԃ��V�[�P���X�̉񓚃��X�|���X�d���B
''' </summary>
Public Class NkTestAckTelegram
    Inherits NkTelegram

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
    Public Sub New(ByVal aTestData As Byte())
        MyBase.New(NkSeqCode.Test, NkCmdCode.DataPostAck, If(aTestData Is Nothing, 0, aTestData.Length))
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
#End Region

End Class
