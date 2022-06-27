' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/06/27  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    '�E�B���h�E�^�C�g��
    Public Shared FormTitle As New Sentence("���d���������@���� ���p�f�[�^�T�[�o")
    Public Shared RiyoDataFormTitle As New Sentence("{1} {2} {0} {3:D}���@ ���p�f�[�^�ҏW")

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
    Public Shared SearchWordNotFound As New Sentence("�}�b�`����Z���͂P������܂���B", SentenceAttr.Information)
    Public Shared RiyoDataFileCreateReally As New Sentence("�t�@�C����V�K�쐬���܂��B", SentenceAttr.Information)
    Public Shared RiyoDataFileSizeError As New Sentence("�t�@�C���T�C�Y���ُ�ł��B", SentenceAttr.Error)
    Public Shared RiyoDataFileReadError As New Sentence("�t�@�C���ǂݍ��݂ňُ킪�������܂����B\n{0}", SentenceAttr.Error)
    Public Shared RiyoDataFileWriteError As New Sentence("�t�@�C���������݂ňُ킪�������܂����B\n{0}", SentenceAttr.Error)
    Public Shared RiyoDataFileExclusionError As New Sentence("�t�@�C���ɑ΂���ύX�����o���܂����B\n�������݂͒��~���܂��B", SentenceAttr.Error)
    Public Shared RiyoDataStoreFailed As New Sentence("�~�ςł��܂���ł����B", SentenceAttr.Error)
    Public Shared RiyoDataStoreFinished As New Sentence("�~�ς��܂����B", SentenceAttr.Information)
    Public Shared RiyoDataSendFailed As New Sentence("�V�~�����[�^�{�̂֗v���ł��܂���ł����B", SentenceAttr.Error)
    Public Shared RiyoDataSendFinished As New Sentence("�V�~�����[�^�{�̂֗v�����܂����B\n�V�~�����[�^�{�̂̃��O���m�F���Ă��������B", SentenceAttr.Information)
    Public Shared RiyoDataBaseHeaderSetReally As New Sentence("�@��ID�⌻�ݓ��������ƂɊ�{�w�b�_�[���Đݒ肵�܂��B\n��낵���ł����H", SentenceAttr.Question)
    Public Shared RiyoDataMinDateReplaceReally As New Sentence("�[���ȊO���ݒ肳��Ă���J�n���┭�s����\n���L�Œu�������܂��B\n��낵���ł����H")
    Public Shared RiyoDataMaxDateReplaceReally As New Sentence("�[���ȊO���ݒ肳��Ă���I���������L�Œu�������܂��B\n��낵���ł����H")
    Public Shared RiyoDataEntDateReplaceReally As New Sentence("�[���ȊO���ݒ肳��Ă������������{�w�b�_�[�̓�����\n���L�Œu�������܂��B\n��낵���ł����H")
    Public Shared RiyoDataOrgStaReplaceReally As New Sentence("�����̉w�R�[�h���ݒ肳��Ă��锭�w�����L�Œu�������܂��B\n��낵���ł����H")
    Public Shared RiyoDataDstStaReplaceReally As New Sentence("�����̉w�R�[�h���ݒ肳��Ă��钅�w�����L�Œu�������܂��B\n��낵���ł����H")
    Public Shared RiyoDataEntStaReplaceReally As New Sentence("�����̉w�R�[�h���ݒ肳��Ă������w���{�w�b�_�[�̉w��\n���L�Œu�������܂��B\n��낵���ł����H")
    Public Shared SelectRecordToRead As New Sentence("{0:D}���R�[�h�����݂��܂��B\n�ǂݍ��ރ��R�[�h��I�����Ă��������B")
    Public Shared SelectRecordToWrite As New Sentence("{0:D}���R�[�h�����݂��܂��B\n�㏑�����郌�R�[�h��I�����Ă��������B")
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
