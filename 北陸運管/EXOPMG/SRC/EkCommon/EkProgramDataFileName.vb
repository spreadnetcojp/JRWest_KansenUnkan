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
''' �w���@��̃v���O�����t�@�C��������舵�����߂̃N���X�B
''' </summary>
Public Class EkProgramDataFileName

#Region "�萔"
    Private Shared ReadOnly oFileNameRegx As New Regex("^[0-9]{2}_(" & EkConstants.SpecificCodeOfKanshiban & "|" & EkConstants.SpecificCodeOfGate & "|" & EkConstants.SpecificCodeOfMadosho & ")_[0-9]{1,8}\.CAB$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const fileNameLenForGateAndMado As Integer = 18
    Private Const fileNameLenForKanshiban As Integer = 22
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �t�@�C�������v���O�����f�[�^�̂��̂ł��邩�ۂ��𔻒肷��B
    ''' </summary>
    ''' <remarks>
    ''' GetXxxx���\�b�h�́A���̃��\�b�h�̖߂�l��True�ɂȂ�t�@�C������
    ''' �����ɌĂяo�����Ƃ�O��Ƃ���B
    ''' </remarks>
    ''' <param name="sFileName">�t�@�C����</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsValid(ByVal sFileName As String) As Boolean
        If Not oFileNameRegx.IsMatch(sFileName) Then Return False

        Dim sApplicableModel As String = GetApplicableModel(sFileName)
        If sApplicableModel.Equals("W") Then
            If sFileName.Length <> fileNameLenForKanshiban Then Return False
        Else
            If sFileName.Length <> fileNameLenForGateAndMado Then Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' �v���O�����t�@�C��������f�[�^�̎�ʁi"WPG"�܂���"GPG"�܂���"YPG"�j���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �擾���镶����̌`���͖{�N���X�̓����ɃJ�v�Z�������Ă���B
    ''' </remarks>
    ''' <param name="sFileName">�v���O�����t�@�C����</param>
    ''' <returns>�f�[�^�̎�ʁi"WPG"���A�K�p���X�g�t�@�C�������Ɠ��l�̒��ۃf�[�^��ʖ��j</returns>
    Public Shared Function GetKind(ByVal sFileName As String) As String
        Dim sApplicableModel As String = GetApplicableModel(sFileName)
        Return sApplicableModel & "PG"
    End Function

    ''' <summary>
    ''' �v���O�����t�@�C��������f�[�^�̃T�u��ʁi�G���ANo�j���擾����B
    ''' </summary>
    ''' <param name="sFileName">�v���O�����t�@�C����</param>
    ''' <returns>�f�[�^�̃T�u��ʁi�G���ANo�j</returns>
    Public Shared Function GetSubKindAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetSubKind(sFileName))
    End Function

    ''' <summary>
    ''' �v���O�����t�@�C��������f�[�^�̃T�u��ʁi�G���ANo�j���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �Q���̕�����Ƃ��Ď擾����B
    ''' </remarks>
    ''' <param name="sFileName">�v���O�����t�@�C����</param>
    ''' <returns>�f�[�^�̃T�u��ʁi�G���ANo�j</returns>
    Public Shared Function GetSubKind(ByVal sFileName As String) As String
        Return sFileName.Substring(0, 2)
    End Function

    ''' <summary>
    ''' �v���O�����t�@�C��������f�[�^�̓K�p�Ώۋ@��i"W"�܂���"G"�܂���"Y"�j���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �擾���镶����̌`���͖{�N���X�̓����ɃJ�v�Z�������Ă���B
    ''' </remarks>
    ''' <param name="sFileName">�v���O�����t�@�C����</param>
    ''' <returns>�f�[�^�̓K�p�Ώۋ@��i"W"���A�K�p���X�g�t�@�C�������Ɠ��l�̒��ۃf�[�^�@�햼�j</returns>
    Public Shared Function GetApplicableModel(ByVal sFileName As String) As String
        Return sFileName.Substring(4, 1).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �v���O�����t�@�C��������f�[�^�̓K�p�Ώې��i�R�[�h���擾����B
    ''' </summary>
    ''' <param name="sFileName">�v���O�����t�@�C����</param>
    ''' <returns>�f�[�^�̓K�p�Ώې��i�R�[�h</returns>
    Public Shared Function GetApplicableSpecificModel(ByVal sFileName As String) As String
        Return sFileName.Substring(3, 6).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �v���O�����t�@�C��������f�[�^�̃o�[�W�����i��\�o�[�W�����j���擾����B
    ''' </summary>
    ''' <param name="sFileName">�v���O�����t�@�C����</param>
    ''' <returns>�f�[�^�̃o�[�W�����i��\�o�[�W�����j</returns>
    Public Shared Function GetVersionAsInt(ByVal sFileName As String) As Integer
        Dim nextSepPos As Integer = sFileName.IndexOf("."c, 11)
        Return Integer.Parse(sFileName.Substring(10, nextSepPos - 10))
    End Function

    ''' <summary>
    ''' �v���O�����t�@�C��������f�[�^�̃o�[�W�����i��\�o�[�W�����j���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �K�p�Ώۋ@��ɂ���ĂW���܂��͂S���̕�����Ƃ��Ď擾����B
    ''' </remarks>
    ''' <param name="sFileName">�v���O�����t�@�C����</param>
    ''' <returns>�f�[�^�̃o�[�W�����i��\�o�[�W�����j</returns>
    Public Shared Function GetVersion(ByVal sFileName As String) As String
        Dim intValue As Integer = GetVersionAsInt(sFileName)
        Dim sApplicableModel As String = GetApplicableModel(sFileName)
        Return intValue.ToString(EkConstants.ProgramDataVersionFormatOfModels(sApplicableModel))
    End Function

    ''' <summary>
    ''' �����ȃt�@�C�����𐶐�����B
    ''' </summary>
    ''' <param name="sFileName">�v���O�����t�@�C����</param>
    ''' <returns>�����ȃv���O�����t�@�C����</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetSubKind(sFileName), GetApplicableModel(sFileName), GetVersion(sFileName))
    End Function

    ''' <summary>
    ''' �v���O�����̃t�@�C�����𐶐�����B
    ''' </summary>
    ''' <remarks>
    ''' ��������t�@�C�����̌`���͖{�N���X�̓����ɃJ�v�Z�������Ă���B
    ''' </remarks>
    ''' <param name="sSubKind">�f�[�^�̃T�u��ʁi�G���ANo�j</param>
    ''' <param name="sApplicableModel">�f�[�^�̓K�p�Ώۋ@��i"W"�܂���"G"�܂���"Y"�j</param>
    ''' <param name="sVersion">�f�[�^�̃o�[�W�����i��\�o�[�W�����j</param>
    ''' <returns>�v���O�����t�@�C����</returns>
    Public Shared Function Gen( _
       ByVal sSubKind As String, _
       ByVal sApplicableModel As String, _
       ByVal sVersion As String) As String

        Return sSubKind & "_" & EkConstants.SpecificCodeOfModels(sApplicableModel) & "_" & sVersion & ".CAB"
    End Function
#End Region

End Class
