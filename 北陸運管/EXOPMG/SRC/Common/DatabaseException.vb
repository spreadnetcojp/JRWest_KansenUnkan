Imports JR.ExOpmg.Common

Public Class DatabaseException
    Inherits Exception

    '���b�Z�[�W�v���p�e�B�̃f�t�H���g�l
    'NOTE: �ǂ�������Ƃ��Ă������B
    Private Const defaultMessage As String = "Some method fails in database access."

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

End Class
