' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �ݗ��^�ǌ����̂��̂��x�[�X�ɍ쐬
' **********************************************************************
Option Strict On
Option Explicit On

Imports System.Text

''' <summary>
''' �y��O�N���X�z
''' </summary>
Public Class OPMGException
    Inherits Exception

    '���b�Z�[�W�v���p�e�B�̃f�t�H���g�l
    'NOTE: �ǂ�������Ƃ��Ă������B
    Private Const defaultMessage As String = "Some method fails in OPMG library."

#Region " �R���X�g���N�^ "
    ''' <summary>
    ''' �R���X�g���N�^
    ''' </summary>
    Public Sub New()
        MyBase.New(defaultMessage)
    End Sub

    ''' <summary>
    ''' �R���X�g���N�^
    ''' </summary>
    ''' <param name="message">�G���[���b�Z�[�W</param>
    ''' <remarks>
    ''' �C�ӂ̃G���[���b�Z�[�W���w�肷��ꍇ�̃R���X�g���N�^�B
    ''' </remarks>
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    ''' <summary>
    ''' �R���X�g���N�^
    ''' </summary>
    ''' <param name="innerException">���݂̗�O�̌����ł����O</param>
    Public Sub New(ByVal innerException As Exception)
        MyBase.New(defaultMessage, innerException)
    End Sub

    ''' <summary>
    ''' �R���X�g���N�^
    ''' </summary>
    ''' <param name="innerException">���݂̗�O�̌����ł����O</param>
    ''' <param name="message">�G���[���b�Z�[�W</param>
    ''' <remarks>
    ''' �C�ӂ̃G���[���b�Z�[�W���w�肷��ꍇ�̃R���X�g���N�^�B
    ''' </remarks>
    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
    End Sub
#End Region

#Region " +s DetailHeader()  �ڍ׃��O�̃w�b�_�s�쐬 "
    ''' <summary>
    ''' �ڍ׃��O�̃w�b�_�s�쐬
    ''' </summary>
    ''' <param name="placeName">�����ꏊ</param>
    ''' <returns>�ڍ׃��O�̃w�b�_�s</returns>
    ''' <remarks></remarks>
    Public Shared Function DetailHeader(ByVal placeName As String) As String
        Dim sb As New StringBuilder
        sb.AppendFormat("{0} fails.", placeName)
        Return sb.ToString()
    End Function
#End Region
#Region " +s DetailNull()  �ڍ׃��O��Nothing�o�� "
    ''' <summary>
    ''' �ڍ׃��O��Nothing�o��
    ''' </summary>
    ''' <param name="objName">�I�u�W�F�N�g��</param>
    ''' <returns>�ڍ׃��O��Nothing�o��</returns>
    ''' <remarks>
    ''' �I�u�W�F�N�g��Nothing�ł���ꍇ�̏o�͂��쐬����B
    ''' </remarks>
    Public Shared Function DetailNull(ByVal objName As String) As String
        Dim sb As New StringBuilder
        sb.AppendFormat("{0} is Nothing.", objName)
        Return sb.ToString()
    End Function
#End Region

#Region " +s DetailNotNull()  �ڍ׃��O��Not Nothing�o�� "
    ''' <summary>
    ''' �ڍ׃��O��Not Nothing�o��
    ''' </summary>
    ''' <param name="objName">�I�u�W�F�N�g��</param>
    ''' <returns>�ڍ׃��O��Not Nothing�o��</returns>
    ''' <remarks>
    ''' �I�u�W�F�N�g��Nothing�łȂ��ꍇ�̏o�͂��쐬����B
    ''' </remarks>
    Public Shared Function DetailNotNull(ByVal objName As String) As String
        Dim sb As New StringBuilder
        sb.AppendFormat("{0} is Something.", objName)
        Return sb.ToString()
    End Function
#End Region

#Region " +s DetailNullOrNotNull()  �ڍ׃��O��Nothing�܂���Not Nothing�o�� "
    ''' <summary>
    ''' �ڍ׃��O��Nothing�܂���Not Nothing�o��
    ''' </summary>
    ''' <param name="objName">�I�u�W�F�N�g��</param>
    ''' <param name="objValue">�I�u�W�F�N�g�ւ̎Q��</param>
    ''' <returns>�ڍ׃��O��Nothing�܂���Not Nothing�o��</returns>
    ''' <remarks>
    ''' �I�u�W�F�N�g�ւ̎Q�Ƃ�Nothing�̏ꍇ�ADetailNull()�̌��ʂ��A
    ''' �I�u�W�F�N�g�ւ̎Q�Ƃ�Not Nothing�̏ꍇ�ADetailNotNull()�̌��ʂ�Ԃ��B
    ''' </remarks>
    Public Shared Function DetailNullOrNotNull(ByVal objName As String, ByVal objValue As Object) As String
        Dim r$ = ""
        If IsNothing(objValue) Then
            r = DetailNull(objName)
        Else
            r = DetailNotNull(objName)
        End If
        Return r
    End Function
#End Region

#Region " +s DetailException()  �ڍ׃��O�̗�O�o�� "
    ''' <summary>
    ''' �ڍ׃��O�̗�O�o��
    ''' </summary>
    ''' <param name="actionName">���O�쐬���̑���</param>
    ''' <param name="exp">Catch������O</param>
    ''' <returns>�ڍ׃��O�̗�O�o��</returns>
    ''' <remarks>
    ''' �ڍ׃��O�o�͕�����쐬���ɔ���������O���o�͂���B
    ''' </remarks>
    Public Shared Function DetailException(ByVal actionName$, ByVal exp As Exception) As String
        Dim sb As New StringBuilder
        sb.AppendFormat("{0} is indeterminable. ({1})", actionName, exp.Message)
        Return sb.ToString()
    End Function
#End Region

End Class
