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

''' <summary>
''' ServerTelegrapher��ClientTelegrapher���z�肷�鉼�zNAK�d���B
''' </summary>
Public Interface INakTelegram
    Inherits ITelegram

    ReadOnly Property CauseCode() As NakCauseCode
End Interface 

'���z�u���R�v�l
Public Class NakCauseCode
    Public Overrides Function ToString() As String
        Return key
    End Function

    Public Const None As String = "None"
    Public Const Busy As String = "Busy"
    Public Const TelegramError As String = "TelegramError"

    Public key As String

    Public Shared Operator =(ByVal c1 As NakCauseCode, ByVal c2 As NakCauseCode) As Boolean
        Return c1.key.Equals(c2.key)
    End Operator

    Public Shared Operator <>(ByVal c1 As NakCauseCode, ByVal c2 As NakCauseCode) As Boolean
        Return Not c1.key.Equals(c2.key)
    End Operator

    Public Shared Operator =(ByVal c1 As NakCauseCode, ByVal c2 As String) As Boolean
        Return c1.key.Equals(c2)
    End Operator

    Public Shared Operator <>(ByVal c1 As NakCauseCode, ByVal c2 As String) As Boolean
        Return Not c1.key.Equals(c2)
    End Operator

    Public Shared Widening Operator CType(ByVal key As String) As NakCauseCode
        Return New NakCauseCode(key)
    End Operator

    Protected Sub New(ByVal key As String)
        Me.key = key
    End Sub

    Protected Sub New()
    End Sub
End Class
