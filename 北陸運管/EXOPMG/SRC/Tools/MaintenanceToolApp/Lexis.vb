' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2014/04/20  (NES)      �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    Public Shared DoNotExecMultipleInstance As New Sentence("��d�N���͏o���܂���B", SentenceAttr.Error)
    Public Shared SheetProcAbnormalEnd As New Sentence("�ꗗ�\�������Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared FormProcAbnormalEnd As New Sentence("��ʕ\�������Ɏ��s���܂����B", SentenceAttr.Error)

    Public Shared EnvVarNotFound As New Sentence("���ϐ�{0}���ݒ肳��Ă��܂���B", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("�ݒ�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared SweepLogsFailed As New Sentence("�Â����O���폜�ł��܂���ł����B", SentenceAttr.Warning)

    Public Shared DatabaseOpenErrorOccurred As New Sentence("DB�ڑ��Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared DatabaseSearchErrorOccurred As New Sentence("���������Ɏ��s���܂����B", SentenceAttr.Error)

    Public Shared ERR_COMMON As New Sentence("{0}�̎擾�Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared ERR_FILE_READ As New Sentence("�t�@�C���̓ǂݍ��݂Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared ERR_FILE_WRITE As New Sentence("�t�@�C���̏������݂Ɏ��s���܂����B", SentenceAttr.Error)
    'Public Shared ERR_FILE_CSV As New Sentence("�e�L�X�g�t�@�C�����w�肵�Ă��������B", SentenceAttr.Error)

    Public Shared ReallyUpdate As New Sentence("�X�V���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared UpdateCompleted As New Sentence("�X�V����������ɏI�����܂����B", SentenceAttr.Information)
    Public Shared UpdateFailed As New Sentence("�X�V�����Ɏ��s���܂����B", SentenceAttr.Error)

    Public Shared ReallyImport As New Sentence("{0}�t�@�C���̓��e�ōX�V���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared ReallyExport As New Sentence("{0}�t�@�C���ɕۑ����Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared ExportCompleted As New Sentence("�ۑ�����������ɏI�����܂����B", SentenceAttr.Information)
    Public Shared DataErr1DetectedOnImport As New Sentence("�o�^����f�[�^�ɋ�̍��ڂ�����܂��B\n����={0},�w��={1},�@��={2},�G���[�R�[�h={3}\n�����𒆎~���܂��B", SentenceAttr.Error)
    Public Shared DataErr2DetectedOnImport As New Sentence("�o�^����f�[�^�Ɍ��I�[�o�[�̍��ڂ�����܂��B\n����={0},�w��={1},�@��={2},�G���[�R�[�h={3}\n�����𒆎~���܂��B", SentenceAttr.Error)
    Public Shared DataErr3DetectedOnImport As New Sentence("�o�^����f�[�^�ɑS�p����������܂��B\n����={0},�w��={1},�@��={2},�G���[�R�[�h={3}\n�����𒆎~���܂��B", SentenceAttr.Error)
    Public Shared DataErr4DetectedOnImport As New Sentence("�o�^����f�[�^�̐���w���ɐ����ȊO�̕���������܂��B\n����={0},�w��={1},�@��={2},�G���[�R�[�h={3}\n�����𒆎~���܂��B", SentenceAttr.Error)
    Public Shared DataErr5DetectedOnImport As New Sentence("�o�^����f�[�^�̃G���[�R�[�h�ɕs���ȕ���������܂��B\n����={0},�w��={1},�@��={2},�G���[�R�[�h={3}\n�����𒆎~���܂��B", SentenceAttr.Error)
    Public Shared DataErr6DetectedOnImport As New Sentence("�o�^����f�[�^�̋@��R�[�h���s���ł��B\n����={0},�w��={1},�@��={2},�G���[�R�[�h={3}\n�����𒆎~���܂��B", SentenceAttr.Error)

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
