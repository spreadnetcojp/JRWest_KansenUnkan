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

Imports System.IO
Imports System.Text

''' <summary>
''' �Ď��Ճv���O�����Ɖ��D�@�v���O�����̗v�f�t�@�C���̃t�b�^�𒊏ۉ������N���X�B
''' </summary>
Public MustInherit Class EkProgramElementFooter

#Region "�萔"
    Public Const Length As Integer = 96
    Protected VersionPos As Integer
    Protected VersionLen As Integer
    Protected DispNamePos As Integer
    Protected DispNameLen As Integer
#End Region

#Region "�ϐ�"
    Protected RawBytes(Length - 1) As Byte
#End Region

#Region "�v���p�e�B"
    Public Overridable Property Version() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, VersionPos, VersionLen).TrimEnd(Chr(&H20))
        End Get

        Set(ByVal sVersion As String)
            Utility.FillBytes(&H20, RawBytes, VersionPos, VersionLen)
            Encoding.UTF8.GetBytes(sVersion, 0, sVersion.Length, RawBytes, VersionPos)
        End Set
    End Property

    Public Overridable Property DispName() As String
        Get
            Return Encoding.GetEncoding(932).GetString(RawBytes, DispNamePos, DispNameLen).TrimEnd(Chr(&H20))
        End Get

        Set(ByVal sDispName As String)
            Utility.FillBytes(&H20, RawBytes, DispNamePos, DispNameLen)
            Encoding.GetEncoding(932).GetBytes(sDispName, 0, sDispName.Length, RawBytes, DispNamePos)
        End Set
    End Property
#End Region

#Region "���\�b�h"
    'NOTE: sFooteredFilePath�Ƀt�@�C�����Ȃ��ꍇ��A�t�@�C���̒������Z���ꍇ�Ȃǂɂ́A
    'IOException���X���[���܂��B
    Protected Sub New(ByVal sFooteredFilePath As String)
        Using oInputStream As New FileStream(sFooteredFilePath, FileMode.Open, FileAccess.Read)
            oInputStream.Seek(-Length, SeekOrigin.End)
            Dim pos As Integer = 0
            Do
                Dim readSize As Integer = oInputStream.Read(RawBytes, pos, Length - pos)
                If readSize = 0 Then Exit Do
                pos += readSize
            Loop
        End Using
    End Sub

    Public Overridable Function GetFormatViolation() As String
        If Not Utility.IsVisibleAsciiBytesFixed(RawBytes, VersionPos, VersionLen) Then
            Return "Version is invalid (not visible ASCII bytes)."
        End If

        Try
            'NOTE: �v���p�e�B�̃Q�b�^�ɕ���p�������Ă͂Ȃ�Ȃ��i�R���p�C����
            '���̂悤�ɑz�肵�Ă悢�j�Ȃǂ̋K�肪����Ȃ�A�I�~�b�g�����
            '�\�������邪�A�������ɂ��̂悤�ȋK��͂Ȃ����̂Ƒz�肵�Ă���B
            Dim sDispName As String = DispName
        Catch ex As DecoderFallbackException
            Return "DispName is invalid."
        End Try

        Return Nothing
    End Function
#End Region

End Class
