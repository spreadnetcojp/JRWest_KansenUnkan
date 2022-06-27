' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/02/16  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    '�E�B���h�E�^�C�g��
    Public Shared FormTitle As New Sentence("���d�Ď��Ռ��� �^�p�E�ێ�f�[�^�T�[�o")
    Public Shared FaultDataFormTitle As New Sentence("{1} {2} {0} {3:D}���@ �ُ�f�[�^�ҏW")
    Public Shared KadoDataFormTitle As New Sentence("{1} {2} {0} {3:D}���@ �ғ��ێ�f�[�^�ҏW")

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
    Public Shared FaultDataFileForActiveOneRewriteReally As New Sentence("�������M�`���̊����t�@�C�����㏑�����܂��B", SentenceAttr.Information)
    Public Shared FaultDataFileFormatSelectorDescription As New Sentence("�t�@�C����V�K�쐬���܂��B�`����I�����Ă��������B")
    Public Shared FaultDataFileFormatSelectorFormat0Text As New Sentence("�������M�`��")
    Public Shared FaultDataFileFormatSelectorFormat1Text As New Sentence("�Ď��W�`��")
    Public Shared FaultDataFileForPassiveUllAppendReally As New Sentence("�Ď��W�`���̃t�@�C���ɒǋL���s���܂��B", SentenceAttr.Information)
    Public Shared FaultDataFileForActiveOneAppendError As New Sentence("�������M�`���̃t�@�C���ɒǋL�͂ł��܂���B", SentenceAttr.Error)
    Public Shared FaultDataFileSizeError As New Sentence("�t�@�C���T�C�Y���ُ�ł��B", SentenceAttr.Error)
    Public Shared FaultDataFileReadError As New Sentence("�t�@�C���ǂݍ��݂ňُ킪�������܂����B\n{0}", SentenceAttr.Error)
    Public Shared FaultDataFileWriteError As New Sentence("�t�@�C���������݂ňُ킪�������܂����B\n{0}", SentenceAttr.Error)
    Public Shared FaultDataFileExclusionError As New Sentence("�t�@�C���ɑ΂���ύX�����o���܂����B\n�������݂͒��~���܂��B", SentenceAttr.Error)
    Public Shared FaultDataStoreFailed As New Sentence("�~�ςł��܂���ł����B", SentenceAttr.Error)
    Public Shared FaultDataStoreFinished As New Sentence("�~�ς��܂����B", SentenceAttr.Information)
    Public Shared FaultDataSendFailed As New Sentence("�V�~�����[�^�{�̂֗v���ł��܂���ł����B", SentenceAttr.Error)
    Public Shared FaultDataSendFinished As New Sentence("�V�~�����[�^�{�̂֗v�����܂����B\n�V�~�����[�^�{�̂̃��O���m�F���Ă��������B", SentenceAttr.Information)
    Public Shared FaultDataBaseHeaderSetReally As New Sentence("�@��ID�⌻�ݓ��������ƂɊ�{�w�b�_�[���Đݒ肵�܂��B\n��낵���ł����H", SentenceAttr.Question)
    Public Shared FaultDataAllHeadersSetReally As New Sentence("�@��ID�⌻�ݓ��������ƂɒʘH�����܂ł̑S���ڂ��Đݒ肵�܂��B\n��낵���ł����H", SentenceAttr.Question)
    Public Shared FaultDataErrorTextsSetReally As New Sentence("�G���[�R�[�h�����ƂɊe���ڂ́u�\���f�[�^�v�Ɓu�L���o�C�g���v��ݒ肵�܂��B\n��낵���ł����H", SentenceAttr.Question)
    Public Shared FaultDataByteCountsSetReally As New Sentence("�e���ڂɂ��āu�\���f�[�^�v�����ƂɁu�L���o�C�g���v��ݒ肵�܂��B\n��낵���ł����H", SentenceAttr.Question)
    Public Shared FaultDataErrorTextsSetFailed As New Sentence("�u�\���f�[�^�v�̐ݒ�ňُ킪�������܂����B\n{0}", SentenceAttr.Error)
    'Public Shared FaultDataErrorTextsNotFound As New Sentence("�G���[�R�[�h�ɕR�Â��������݂���܂���ł����B", SentenceAttr.Error)
    Public Shared KadoDataManagementFileIsBroken As New Sentence("�@��̉ғ��ێ�f�[�^�̃t�@�C���T�C�Y���ُ�ł��B\n��֑[�u�Ƃ��āA�@��ID�⌻�ݓ��������Ƃɂ��������l��\�����܂��B", SentenceAttr.Warning)
    Public Shared KadoDataManagementFileReadError As New Sentence("�@��̉ғ��ێ�f�[�^�̃t�@�C���ǂݍ��݂ňُ킪�������܂����B\n��֑[�u�Ƃ��āA�@��ID�⌻�ݓ��������Ƃɂ��������l��\�����܂��B", SentenceAttr.Warning)
    Public Shared KadoDataManagementFileIsLocked As New Sentence("�@��̉ғ��ێ�f�[�^�̃t�@�C�������̃v���Z�X�ɂ��g�p���ł��B\n�ǂݍ��݂��Ď��s���܂����H", SentenceAttr.Question)
    Public Shared KadoDataFileRewriteReally As New Sentence("�����t�@�C�����㏑�����܂��B", SentenceAttr.Information)
    Public Shared KadoDataFileCreateReally As New Sentence("�t�@�C����V�K�쐬���܂��B", SentenceAttr.Information)
    Public Shared KadoDataFileAppendReally As New Sentence("�t�@�C���ɒǋL���s���܂��B", SentenceAttr.Information)
    Public Shared KadoDataFileSizeError As New Sentence("�t�@�C���T�C�Y���ُ�ł��B", SentenceAttr.Error)
    Public Shared KadoDataFileReadError As New Sentence("�t�@�C���ǂݍ��݂ňُ킪�������܂����B\n{0}", SentenceAttr.Error)
    Public Shared KadoDataFileWriteError As New Sentence("�t�@�C���������݂ňُ킪�������܂����B\n{0}", SentenceAttr.Error)
    Public Shared KadoDataFileExclusionError As New Sentence("�t�@�C���ɑ΂���ύX�����o���܂����B\n�������݂͒��~���܂��B", SentenceAttr.Error)
    Public Shared KadoDataStoreFailed As New Sentence("���f�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared KadoDataStoreFinished As New Sentence("���f���܂����B", SentenceAttr.Information)
    Public Shared KadoDataBaseHeaderSetReally As New Sentence("�@��ID�⌻�ݓ��������ƂɊ�{�w�b�_�[���Đݒ肵�܂��B\n��낵���ł����H", SentenceAttr.Question)
    Public Shared KadoDataAllHeadersSetReally As New Sentence("�@��ID�⌻�ݓ��������ƂɊ�{�w�b�_�[�Ƌ��ʕ����Đݒ肵�܂��B\n��낵���ł����H", SentenceAttr.Question)
    Public Shared KadoDataSummariesSetReally As New Sentence("�e�퍇�v���ڂ��Z�o���Đݒ肵�܂��B\n��낵���ł����H", SentenceAttr.Question)
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
