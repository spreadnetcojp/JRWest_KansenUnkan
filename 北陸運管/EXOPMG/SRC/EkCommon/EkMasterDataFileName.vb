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
''' �w���@��̃}�X�^�t�@�C��������舵�����߂̃N���X�B
''' </summary>
Public Class EkMasterDataFileName

#Region "�萔"
    Private Shared ReadOnly oFileNameRegx As New Regex("^PR_[A-Z]{3}[0-9]{2}_[GY]_[0-9]{3}_[0-9]{8}\.bin$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �t�@�C�������}�X�^�f�[�^�̂��̂ł��邩�ۂ��𔻒肷��B
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
        'DB�ɓo�^����Ă��邩�ʓr�`�F�b�N���邱�Ƃ�O��Ƃ��Ă���B

        Dim version As Integer = GetVersionAsInt(sFileName)
        If version < 1 OrElse version > 255 Then Return False

        Return True
    End Function

    ''' <summary>
    ''' �}�X�^�t�@�C��������f�[�^�̎�ʂ��擾����B
    ''' </summary>
    ''' <param name="sFileName">�}�X�^�t�@�C����</param>
    ''' <returns>�f�[�^�̎�ʁi"DSH"���j</returns>
    Public Shared Function GetKind(ByVal sFileName As String) As String
        Return sFileName.Substring(3, 3).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �}�X�^�t�@�C��������f�[�^�̃T�u��ʁi�p�^�[��No�j���擾����B
    ''' </summary>
    ''' <param name="sFileName">�}�X�^�t�@�C����</param>
    ''' <returns>�f�[�^�̃T�u��ʁi�p�^�[��No�j</returns>
    Public Shared Function GetSubKindAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetSubKind(sFileName))
    End Function

    ''' <summary>
    ''' �}�X�^�t�@�C��������f�[�^�̃T�u��ʁi�p�^�[��No�j���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �Q���̕�����Ƃ��Ď擾����B
    ''' </remarks>
    ''' <param name="sFileName">�}�X�^�t�@�C����</param>
    ''' <returns>�f�[�^�̃T�u��ʁi�p�^�[��No�j</returns>
    Public Shared Function GetSubKind(ByVal sFileName As String) As String
        Return sFileName.Substring(6, 2)
    End Function

    ''' <summary>
    ''' �}�X�^�t�@�C��������f�[�^�̓K�p�Ώۋ@��i"G"�܂���"Y"�j���擾����B
    ''' </summary>
    ''' <param name="sFileName">�}�X�^�t�@�C����</param>
    ''' <returns>�f�[�^�̓K�p�Ώۋ@��i"G"�܂���"Y"�j</returns>
    Public Shared Function GetApplicableModel(ByVal sFileName As String) As String
        Return sFileName.Substring(9, 1).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �}�X�^�t�@�C��������f�[�^�̃o�[�W�����i�}�X�^�o�[�W�����j���擾����B
    ''' </summary>
    ''' <param name="sFileName">�}�X�^�t�@�C����</param>
    ''' <returns>�f�[�^�̃o�[�W�����i�}�X�^�o�[�W�����j</returns>
    Public Shared Function GetVersionAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetVersion(sFileName))
    End Function

    ''' <summary>
    ''' �}�X�^�t�@�C��������f�[�^�̃o�[�W�����i�}�X�^�o�[�W�����j���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �R���̕�����Ƃ��Ď擾����B
    ''' </remarks>
    ''' <param name="sFileName">�}�X�^�t�@�C����</param>
    ''' <returns>�f�[�^�̃o�[�W�����i�}�X�^�o�[�W�����j</returns>
    Public Shared Function GetVersion(ByVal sFileName As String) As String
        Return sFileName.Substring(11, 3)
    End Function

    ''' <summary>
    ''' �}�X�^�t�@�C�������烆�[�U��`�����l���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �W���̕�����Ƃ��Ď擾����B
    ''' </remarks>
    ''' <param name="sFileName">�}�X�^�t�@�C����</param>
    ''' <returns>���[�U��`�����l</returns>
    Public Shared Function GetUserMemo(ByVal sFileName As String) As String
        Return sFileName.Substring(15, 8)
    End Function

    ''' <summary>
    ''' �����ȃt�@�C�����𐶐�����B
    ''' </summary>
    ''' <param name="sFileName">�}�X�^�t�@�C����</param>
    ''' <returns>�����ȃ}�X�^�t�@�C����</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetKind(sFileName), GetSubKind(sFileName), GetApplicableModel(sFileName), GetVersion(sFileName), GetUserMemo(sFileName))
    End Function

    ''' <summary>
    ''' �}�X�^�̃t�@�C�����𐶐�����B
    ''' </summary>
    ''' <remarks>
    ''' ��������t�@�C�����̌`���͖{�N���X�̓����ɃJ�v�Z�������Ă���B
    ''' </remarks>
    ''' <param name="sKind">�f�[�^�̎�ʁi"DSH"���j</param>
    ''' <param name="sSubKind">�f�[�^�̃T�u��ʁi�p�^�[��No�j</param>
    ''' <param name="sApplicableModel">�f�[�^�̓K�p�Ώۋ@��i"G"�܂���"Y"�j</param>
    ''' <param name="sVersion">�f�[�^�̃o�[�W�����i�}�X�^�o�[�W�����j</param>
    ''' <param name="sUserMemo">���[�U��`�����l</param>
    ''' <returns>�}�X�^�t�@�C����</returns>
    Public Shared Function Gen( _
       ByVal sKind As String, _
       ByVal sSubKind As String, _
       ByVal sApplicableModel As String, _
       ByVal sVersion As String, _
       ByVal sUserMemo As String) As String

        Return "PR_" & sKind & sSubKind & "_" & sApplicableModel & "_" & sVersion & "_" & sUserMemo & ".bin"
    End Function
#End Region

End Class
