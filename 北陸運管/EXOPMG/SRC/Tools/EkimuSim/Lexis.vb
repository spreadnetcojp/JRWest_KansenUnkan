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

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    '�E�B���h�E�^�C�g��
    Public Shared FormTitle As New Sentence("�w���@�� {0}")
    Public Shared FormTitleEkCodeFormat As New Sentence("%2M-%3R-%3S-%4C-%2U")

    '���b�Z�[�W�{�b�N�X����
    Public Shared DoNotExecInSameWorkingDir As New Sentence("����̍�ƃt�H���_�ŕ����N�����Ȃ��ł��������B", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("�ݒ�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UiStateDeserializeFailed As New Sentence("��ԃt�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UiStateSerializeFailed As New Sentence("��ԃt�@�C���̏������݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared ConnectFailed As New Sentence("�ڑ��ł��܂���ł����B", SentenceAttr.Error)
    Public Shared UnwelcomeExceptionCaught As New Sentence("�v���O�����Ɉُ�����o���܂����B", SentenceAttr.Error)
    Public Shared TheInputValueIsUnsuitableForObjCode As New Sentence("�f�[�^��ʂ�2����16�i������͂��Ă��������B", SentenceAttr.Warning)
    Public Shared TheInputValueIsDuplicative As New Sentence("�L�[������̍s�����݂��܂��B", SentenceAttr.Warning)
    Public Shared ScenarioFileIsIllegal As New Sentence("�V�i���I�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared ScenarioFileIsEmpty As New Sentence("�V�i���I�t�@�C������ł��B", SentenceAttr.Error)
    Public Shared DoNotRepeatScenarioThatContainsAbsoluteTiming As New Sentence("��Γ�����p�����V�i���I���J��Ԃ����s���邱�Ƃ͂ł��܂���B", SentenceAttr.Error)

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
