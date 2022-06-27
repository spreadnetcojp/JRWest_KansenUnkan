' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/05/13  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'NOTE: ���샍�O�ȊO�̃��O�́A�����ł͂Ȃ�Log.Xxxx���\�b�h�̌Ăяo���ӏ��ɁA�����񃊃e�����𒼐ڋL�q������j�B

    'NOTE: �I���W�i�������ł́A�o�͂ł�����͑S�ďo�͂���悤�ɂ��A�����g�p�ӏ�������ɍ��킹�Ĉ�����n���悤�ɂ���B
    '���Ǝ҂��Ƃ̎d�l�ŕs�v�ȏ�񂪂���΁A���Y���Ǝҗp��INI�t�@�C���ɁA���̏����Ԉ������������`����B

    '�E�B���h�E�^�C�g��
    Public Shared FormTitle As New Sentence("�f�W�N���C�A���g")

    '���b�Z�[�W�{�b�N�X����
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("�ݒ�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)

    Public Shared TheFileTypeIsInvalid As New Sentence("�I�����ꂽ�t�@�C����{0}�ł͂���܂���B", SentenceAttr.Warning)
    Public Shared ThePatternNoDoesNotExist As New Sentence("�p�^�[��No���o�^����Ă��܂���B", SentenceAttr.Warning)
    Public Shared ConnectFailed As New Sentence("�ڑ������Ɏ��s���܂����B", SentenceAttr.Error)

    Public Shared ReallyUllMasProFile As New Sentence("�o�^���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared UllMasProFileCompleted As New Sentence("�o�^���������܂����B", SentenceAttr.Information)
    Public Shared UllMasProFileFailed As New Sentence("�o�^�ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByBusy As New Sentence("���̒[���ő��쒆�̂��߁A�o�^�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByInvalidContent As New Sentence("�ُ�ȃt�@�C���̂��߁A�o�^�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByUnknownLight As New Sentence("���炩�̌����ŁA�o�^�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared ReallyExitWithoutUll As New Sentence("�f�[�^���o�^����Ă��܂���B\n�I�����Ă���낵���ł����H", SentenceAttr.Question)

    Public Shared ReallyInvokeMasProDll As New Sentence("�z�M���J�n���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared InvokeMasProDllCompleted As New Sentence("�z�M���J�n���܂����B", SentenceAttr.Information)
    Public Shared InvokeMasProDllFailed As New Sentence("�z�M�̊J�n�ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByBusy As New Sentence("���̒[���ő��쒆�̂��߁A�z�M���J�n�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByNoData As New Sentence("�z�M�Ώۃf�[�^���o�^����Ă��Ȃ����߁A�z�M���J�n�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByUnnecessary As New Sentence("�K�p���X�g����V���Ȕz�M�悪�݂���܂���ł����B", SentenceAttr.Warning)
    Public Shared InvokeMasProDllFailedByInvalidContent As New Sentence("�z�M�Ώۃf�[�^���ُ�Ȃ��߁A�z�M���J�n�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByUnknownLight As New Sentence("���炩�̌����ŁA�z�M���J�n�ł��܂���ł����B", SentenceAttr.Error)

    '���샍�O�̕���
    Public Shared WindowSuffix As New Sentence("���")
    Public Shared DialogSuffix As New Sentence("�_�C�A���O")
    Public Shared DateTimePickerValueChanged As New Sentence("{0}�ɂ�{1}��{2}�ɕύX���܂����B")
    Public Shared ComboBoxSelectionChanged As New Sentence("{0}�ɂ�{1}��{2}�ɕύX���܂����B")
    Public Shared ComboBoxSelectionChangedToNothing As New Sentence("{0}�ɂ�{1}�𖢑I���ɕύX���܂����B")
    Public Shared ButtonClicked As New Sentence("{0}�ɂ�{1}���N���b�N���܂����B")
    Public Shared SomeControlInvoked As New Sentence("{0}�ɂ�{1}({2}�^)�𑀍삵�܂����B")
    Public Shared YesButtonClicked As New Sentence("�͂��{�^�������B")
    Public Shared NoButtonClicked As New Sentence("�������{�^�������B")
    Public Shared OkButtonClicked As New Sentence("OK�{�^�������B")

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
