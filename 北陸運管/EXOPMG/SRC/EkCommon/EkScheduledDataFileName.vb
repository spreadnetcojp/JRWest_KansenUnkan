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

Imports System.Text.RegularExpressions

''' <summary>
''' �w���@�킩��T�[�o�v��ULL�Ŏ��W����f�[�^�̃t�@�C��������舵�����߂̃N���X�B
''' </summary>
Public Class EkScheduledDataFileName

#Region "�萔"
    Private Shared ReadOnly oFileNameRegx As New Regex("^SK_[A-Z]{3}.DAT$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �t�@�C�������T�[�o�v��ULL�Ŏ��W����f�[�^�̂��̂ł��邩�ۂ��𔻒肷��B
    ''' </summary>
    ''' <remarks>
    ''' GetXxxx���\�b�h�́A���̃��\�b�h�̖߂�l��True�ɂȂ�t�@�C������
    ''' �����ɌĂяo�����Ƃ�O��Ƃ���B
    ''' </remarks>
    ''' <param name="sFileName">�t�@�C����</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsValid(ByVal sFileName As String) As Boolean
        If Not oFileNameRegx.IsMatch(sFileName) Then Return False

        'NOTE: ���̃��\�b�h�ł�Kind�̒l�̓`�F�b�N���Ȃ��B
        '���̃��\�b�h���Ăяo������AGetKind�̖߂�l��
        '���������̂ł��邩�ʓr�`�F�b�N���邱�Ƃ�O��Ƃ��Ă���B
        Return True
    End Function

    ''' <summary>
    ''' �T�[�o�v��ULL�Ŏ��W����t�@�C��������f�[�^�̎�ʂ��擾����B
    ''' </summary>
    ''' <param name="sFileName">�T�[�o�v��ULL�Ŏ��W����t�@�C����</param>
    ''' <returns>�f�[�^�̎�ʁi"DSH"���j</returns>
    Public Shared Function GetKind(ByVal sFileName As String) As String
        Return sFileName.Substring(3, 3).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �����ȃt�@�C�����𐶐�����B
    ''' </summary>
    ''' <param name="sFileName">�T�[�o�v��ULL�Ŏ��W����t�@�C����</param>
    ''' <returns>�T�[�o�v��ULL�Ŏ��W���錵���ȃt�@�C����</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetKind(sFileName))
    End Function

    ''' <summary>
    ''' �T�[�o�v��ULL�Ŏ��W����f�[�^�̃t�@�C�����𐶐�����B
    ''' </summary>
    ''' <remarks>
    ''' ��������t�@�C�����̌`���͖{�N���X�̓����ɃJ�v�Z�������Ă���B
    ''' </remarks>
    ''' <param name="sKind">�f�[�^�̎�ʁi"KDO"���j</param>
    ''' <returns>�T�[�o�v��ULL�Ŏ��W����t�@�C����</returns>
    Public Shared Function Gen(ByVal sKind As String) As String
        Return "SK_" & sKind & ".DAT"
    End Function
#End Region

End Class
