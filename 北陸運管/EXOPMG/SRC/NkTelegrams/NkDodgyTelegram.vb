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

Imports System.Net.Sockets

Imports JR.ExOpmg.Common

''' <summary>
''' ��ʓ����s���̓d���B
''' </summary>
Public Class NkDodgyTelegram
    Inherits NkTelegram

    Friend Sub New(ByVal aRawBytes As Byte())
        MyBase.New(aRawBytes, Nothing, 0)
    End Sub

    'NOTE: ���̃N���X�̃C���X�^���X����GetBodyFormatViolation()���s�����Ƃ͖��Ӗ��ł���A
    '���炩�Ɍ��ł��邽�߁A����Ďg�p���ꂽ���Ƃ�����悤�A�����Ď������Ă��܂��B
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        Debug.Fail("The caller of ITelegram.GetBodyFormatViolation() may be wrong.")
        Return NakCauseCode.None
    End Function

End Class
