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
''' �\�P�b�g�����P�v�I�v����O
''' </summary>
''' <remarks>
''' �\�P�b�g�ɑ΂��鏈�������s�����ہA�v������P�v�I�i���̃\�P�b�g�ɂ��
''' �������\�[�X�̎g�p���j�ł���\��������ꍇ�ɐ��������O�B
''' ���[�U���݂Ă���󋵂ł̂ݏ������s����ꍇ��A�A�v���P�[�V������
''' �I�������邱�ƂŃA���[����������v�z�ł���ꍇ���̂����A
''' ���̗�O�ŃA�v���P�[�V�������I��������ׂ��ł͂Ȃ��i�������A�قƂ�ǂ�
''' �A�v���P�[�V�����ɂ����āA��������̎�i�Ń��[�U�ɏ󋵂�ʒm���邱�Ƃ�
''' �K�{�Ǝv����j�B
''' </remarks>
Public Class SocketImpermanentException
    Inherits Exception
#Region " �R���X�g���N�^ "
    '���b�Z�[�W�v���p�e�B�̃f�t�H���g�l
    'NOTE: �ǂ�������Ƃ��Ă������B
    Private Const defaultMessage As String = "Socket operation failed by impermanent cause."

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
