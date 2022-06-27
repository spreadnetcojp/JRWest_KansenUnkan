' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/03/01  (NES)�͘e  �V�K�쐬
'   0.2      2014/06/12  (NES)�c��  �k���Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    Public Shared EnvVarNotFound As New Sentence("���ϐ�{0}���ݒ肳��Ă��܂���B", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("�ݒ�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared SweepLogsFailed As New Sentence("�Â����O���폜�ł��܂���ł����B", SentenceAttr.Warning)

    Public Shared ERR_COMMON As New Sentence("{0}�̎擾�Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared ERR_FILE_READ As New Sentence("�t�@�C���̓ǂݍ��݂Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared ERR_FILE_WRITE As New Sentence("�t�@�C���̏������݂Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared ERR_UNKNOWN As New Sentence("���̑��ُ킪�������܂����B", SentenceAttr.Error)
    Public Shared ERR_FILE_CSV As New Sentence("�e�L�X�g�t�@�C�����w�肵�Ă��������B", SentenceAttr.Error)

    Public Shared Confirm As New Sentence("�������܂����B", SentenceAttr.Question)
    Public Shared Finished As New Sentence("�������܂����B", SentenceAttr.Question)

    Public Shared TheInputValueIsUnsuitableForMasterVersion As New Sentence("�o�[�W�����𐳂������͂��Ă��������B", SentenceAttr.Warning)

    'Ver0.2 ADD START  �k���Ή�
    Public Shared ThePatternNoDoesNotRelated As New Sentence("�}�X�^�Ɋ֘A����p�^�[��No�ł͂���܂���B", SentenceAttr.Warning)
    Public Shared FileTypeNG1 As New Sentence("�b�r�u�t�@�C�����w�肵�Ă��������B", SentenceAttr.Warning)
    Public Shared FileTypeNG2 As New Sentence("�b�r�u�t�@�C���͎w��ł��܂���B", SentenceAttr.Warning)
    Public Shared FileTypeNG3 As New Sentence("���ɕϊ��ς݂̃t�@�C���ł��B", SentenceAttr.Warning)
    'Ver0.2 ADD END    �k���Ή�

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
