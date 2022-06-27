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

''' <summary>
''' �\�P�b�g���쑕�u�O�v����O
''' </summary>
''' <remarks>
''' �\�P�b�g�ɑ΂��鏈�������s�����ہA�v�������u�O�ɂ���\����
''' ����ꍇ�ɐ��������O�B
''' </remarks>
Public Class SocketExternalException
    Inherits Exception
#Region " �R���X�g���N�^ "
    '���b�Z�[�W�v���p�e�B�̃f�t�H���g�l
    'NOTE: �ǂ�������Ƃ��Ă������B
    Private Const defaultMessage As String = "Socket operation failed by external cause."

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
End Class
