' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

''' <summary>
''' �o�^�n�����̒萔���`����N���X�B
''' </summary>
Public Class RecAppConstants

    '���O�����i��`�t�@�C���֘A�j
    Public Const ERR_INI_FILE_NOT_FOUND As String = "������`�t�@�C��[{0}]�����݂��܂���B"
    Public Const ERR_BAD_INI_FILE As String = "������`�t�@�C���̓��e���s���ł��B"

    '���O�����i1��ڂ̉�́j
    Public Const ERR_TOO_SHORT_FILE As String = "�t�@�C����1���R�[�h�����̒����ł��B"
    Public Const ERR_INVALID_FIELD_AS_BCD As String = "{0}��BCD�Ƃ݂Ȃ��܂���B"
    Public Const ERR_INVALID_RECORD As String = "���R�[�h[{0}]�͕s���Ȃ��ߓo�^���܂���B"
    Public Const ERR_FILE_ROUNDED_OFF As String = "�t�@�C���̒����ɒ[��������܂��B"

    '���O�����i2��ڂ̉�́j
    'NOTE: �un�s�ځv�́un�v�́A�P��ڂ̉�͂Ŏc�������R�[�h��1�N�_�Ōv�サ���ԍ��ł���B
    Public Const ERR_MSG_NOVALUE As String = "{0}�s�ڂ�{1}������܂���B"
    Public Const ERR_MSG_ERRVALUE As String = "{0}�s�ڂ�{1}���s���ł��B"
    Public Const ERR_MACHINE_NOVALUE As String = "�@�킪���݂��܂���B(����:{0} �w��:{1} �R�[�i:{2} ���@:{3})"

End Class
