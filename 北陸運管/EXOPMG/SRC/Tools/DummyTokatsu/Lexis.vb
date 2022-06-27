' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/08/08  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    '�E�B���h�E�^�C�g��
    Public Shared FormTitle As New Sentence("���d�������� �^�p�f�[�^�T�[�o")

    '���b�Z�[�W�{�b�N�X����
    Public Shared MultipleInstanceNotAllowed As New Sentence("�����N�����Ȃ��ł��������B", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("�ݒ�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UiStateDeserializeFailed As New Sentence("��ԃt�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UiStateSerializeFailed As New Sentence("��ԃt�@�C���̏������݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UnwelcomeExceptionCaught As New Sentence("�v���O�����Ɉُ�����o���܂����B", SentenceAttr.Error)
    Public Shared MessageQueueServiceNotAvailable As New Sentence("���b�Z�[�W�L���[�T�[�r�X���g�p�ł��܂���B", SentenceAttr.Error)
    Public Shared MessageQueueDeleteFailed As New Sentence("���b�Z�[�W�L���[�̍폜�Ɏ��s���܂����B\n�s�v�ł���Ύ蓮�ō폜���Ă��������B", SentenceAttr.Error)
    Public Shared InvalidDirectorySpecified As New Sentence("�����ȃf�B���N�g�����w�肳��܂����B", SentenceAttr.Error)
    Public Shared MachineProfileFetchFinished As New Sentence("�V�~�����[�^�{�̂̑S�@��𑖍����܂����B\n�����Ȃ����@���ړ������@��̏��͂��̂܂܎c���Ă��܂��̂ŁA�s�v�ł���Ύ蓮�ō폜���Ă��������B", SentenceAttr.Information)
    Public Shared TermMachineRowNotSelected As New Sentence("���������@�̍s���P�ȏ�I�����Ă����ԂŎ��s���Ă��������B", SentenceAttr.Error)
    Public Shared LogDispFilterIsInvalid As New Sentence("�t�B���^���s���ł��B�ĕҏW���Ă��������B", SentenceAttr.Error)

    '���O�\���O���b�h�̗�w�b�_����
    Public Shared LogDispTimeColumnTitle As New Sentence("Time")
    Public Shared LogDispSourceColumnTitle As New Sentence("Source")
    Public Shared LogDispMessageColumnTitle As New Sentence("Message")

    '���̑�
    Public Shared EmptyTime As New Sentence("")
    Public Shared UnknownTime As New Sentence("(�s��)")

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
