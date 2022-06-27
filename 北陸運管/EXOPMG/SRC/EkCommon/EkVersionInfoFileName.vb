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
''' �o�[�W�������̃t�@�C��������舵�����߂̃N���X�B
''' </summary>
Public Class EkVersionInfoFileName

#Region "�萔"
    Private Shared ReadOnly oFileNameRegx As New Regex("^[MP]_[GWY]_[0-9]{12}VER.DAT$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �t�@�C�������o�[�W�������̂��̂ł��邩�ۂ��𔻒肷��B
    ''' </summary>
    ''' <remarks>
    ''' GetXxxx���\�b�h�́A���̃��\�b�h�̖߂�l��True�ɂȂ�t�@�C������
    ''' �����ɌĂяo�����Ƃ�O��Ƃ���B
    ''' </remarks>
    ''' <param name="sFileName">�t�@�C����</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsValid(ByVal sFileName As String) As Boolean
        If Not oFileNameRegx.IsMatch(sFileName) Then Return False

        Dim sDataApplicableModel As String = GetDataApplicableModel(sFileName)
        If sDataApplicableModel.Equals("W") AndAlso GetDataPurpose(sFileName).Equals("MST") Then Return False

        Return True
    End Function

    ''' <summary>
    ''' �o�[�W�������t�@�C��������o�[�W�����t�^�Ώۃf�[�^�̗p�r���擾����B
    ''' </summary>
    ''' <param name="sFileName">�o�[�W�������t�@�C����</param>
    ''' <returns>�f�[�^�p�r�i"MST"�܂���"PRG"�j</returns>
    Public Shared Function GetDataPurpose(ByVal sFileName As String) As String
        Select Case sFileName.Substring(0, 1).ToUpperInvariant()
            Case "M"
                Return "MST"
            Case "P"
                Return "PRG"
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' �o�[�W�������t�@�C��������f�[�^�̓K�p�Ώۋ@����擾����B
    ''' </summary>
    ''' <param name="sFileName">�o�[�W�������t�@�C����</param>
    ''' <returns>�f�[�^�̓K�p�Ώۋ@��i�p�r���}�X�^�̏ꍇ��"G"�܂���"Y"�A�p�r���v���O�����̏ꍇ��"W"�܂���"G"�܂���"Y"�j</returns>
    Public Shared Function GetDataApplicableModel(ByVal sFileName As String) As String
        Return sFileName.Substring(2, 1).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �o�[�W�������t�@�C��������Ώۍ��@�̎��ʃR�[�h���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �߂�l��Model�v���p�e�B�͏��0�ł���B
    ''' </remarks>
    ''' <param name="sFileName">�o�[�W�������t�@�C����</param>
    ''' <returns>�Ώۍ��@�̎��ʃR�[�h</returns>
    Public Shared Function GetDataApplicableUnit(ByVal sFileName As String) As EkCode
        Return EkCode.Parse(sFileName.Substring(4, 12), "%3R%3S%4C%2U")
    End Function

    ''' <summary>
    ''' �����ȃt�@�C�����𐶐�����B
    ''' </summary>
    ''' <param name="sFileName">�o�[�W�������t�@�C����</param>
    ''' <returns>�����ȃo�[�W�������t�@�C����</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetDataPurpose(sFileName), GetDataApplicableModel(sFileName), GetDataApplicableUnit(sFileName))
    End Function

    ''' <summary>
    ''' �o�[�W�������̃t�@�C�����𐶐�����B
    ''' </summary>
    ''' <remarks>
    ''' ��������t�@�C�����̌`���͖{�N���X�̓����ɃJ�v�Z�������Ă���B
    ''' </remarks>
    ''' <param name="sDataPurpose">�f�[�^�̗p�r�i"MST"�܂���"PRG"�j</param>
    ''' <param name="sDataApplicableModel">�f�[�^�̓K�p�Ώۋ@��i�p�r���}�X�^�̏ꍇ��"G"�܂���"Y"�A�p�r���v���O�����̏ꍇ��"W"�܂���"G"�܂���"Y"�j</param>
    ''' <param name="dataApplicableUnit">�f�[�^�̑Ώۍ��@</param>
    ''' <returns>�o�[�W�������t�@�C����</returns>
    Public Shared Function Gen( _
       ByVal sDataPurpose As String, _
       ByVal sDataApplicableModel As String, _
       ByVal dataApplicableUnit As EkCode) As String

        Select Case sDataPurpose
            Case "MST"
                Return "M_" & sDataApplicableModel & dataApplicableUnit.ToString("_%3R%3S%4C%2UVER.DAT")
            Case "PRG"
                Return "P_" & sDataApplicableModel & dataApplicableUnit.ToString("_%3R%3S%4C%2UVER.DAT")
            Case Else
                Return Nothing
        End Select
    End Function
#End Region

End Class
