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
''' �K�p���X�g�̃t�@�C��������舵�����߂̃N���X�B
''' </summary>
Public Class EkMasProListFileName

#Region "�萔"
    Private Shared ReadOnly oFileNameRegx As New Regex("^[A-Z]{3}_[A-Z]{3}[0-9]{2}_[A-Z]_[0-9]{1,8}_[0-9]{2}\.csv$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const fileNameLenForMasterPurpose As Integer = 22
    Private Const fileNameLenForProgramPurpose As Integer = 27
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �t�@�C�������K�p���X�g�̂��̂ł��邩�ۂ��𔻒肷��B
    ''' </summary>
    ''' <remarks>
    ''' GetXxxx���\�b�h�́A���̃��\�b�h�̖߂�l��True�ɂȂ�t�@�C������
    ''' �����ɌĂяo�����Ƃ�O��Ƃ���B
    ''' </remarks>
    ''' <param name="sFileName">�t�@�C����</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsValid(ByVal sFileName As String) As Boolean
        If Not oFileNameRegx.IsMatch(sFileName) Then Return False

        Dim sListKind As String = GetListKind(sFileName)
        Dim sDataApplicableModel As String = GetDataApplicableModel(sFileName)
        Dim dataVersion As Integer = GetDataVersionAsInt(sFileName)

        If sListKind.Equals("TGL") Then
            If sFileName.Length <> fileNameLenForMasterPurpose Then Return False
            If GetDataSubKind(sFileName).Equals("00") Then Return False

            If sDataApplicableModel.Equals("G") OrElse sDataApplicableModel.Equals("Y") Then
                'NOTE: ���̃��\�b�h�ł�DataKind�̒l�̓`�F�b�N���Ȃ��B
                '���̃��\�b�h���Ăяo������AGetDataKind�̖߂�l��
                '�uDataPurpose���Ƃɗp�ӂ��ꂽDB��̃e�[�u���v��
                '�o�^����Ă��邩�ʓr�`�F�b�N���邱�Ƃ�O��Ƃ��Ă���B
                '�܂��ADataKind���Ή�����G���A�ԍ���DB����擾���A
                '�K�p���X�g�ɋL�ڂ��ꂽ�w�̃G���A�Ɣ�r����̂��A
                '���̃��\�b�h�̖����ł͂Ȃ��B

                If dataVersion < 1 OrElse dataVersion > 255 Then Return False
            Else
                Return False
            End If
        ElseIf sListKind.Equals("TDL") Then
            If sFileName.Length <> fileNameLenForProgramPurpose Then Return False

            If sDataApplicableModel.Equals("G") Then
                If Not GetDataKind(sFileName).Equals("GPG") Then Return False
                If dataVersion > 9999 Then Return False
            ElseIf sDataApplicableModel.Equals("Y") Then
                If Not GetDataKind(sFileName).Equals("YPG") Then Return False
                If dataVersion > 9999 Then Return False
            ElseIf sDataApplicableModel.Equals("W") Then
                If Not GetDataKind(sFileName).Equals("WPG") Then Return False
            Else
                Return False
            End If
        Else
            Return False
        End If

        If GetListVersionAsInt(sFileName) = 0 Then Return False

        Return True
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������K�p���X�g���g�̎�ʂ��擾����B
    ''' </summary>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�K�p���X�g���g�̎�ʁi"TGL"�܂���"TDL"�j</returns>
    Public Shared Function GetListKind(ByVal sFileName As String) As String
        Return sFileName.Substring(0, 3).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������f�[�^�̗p�r���擾����B
    ''' </summary>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�f�[�^�p�r�i"MST"�܂���"PRG"�j</returns>
    Public Shared Function GetDataPurpose(ByVal sFileName As String) As String
        Select Case GetListKind(sFileName)
            Case "TGL"
                Return "MST"
            Case "TDL"
                Return "PRG"
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������f�[�^�̎�ʂ��擾����B
    ''' </summary>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�f�[�^�̎�ʁi�p�r���}�X�^�̏ꍇ��"DSH"���A�p�r���v���O�����̏ꍇ��"WPG"���j</returns>
    Public Shared Function GetDataKind(ByVal sFileName As String) As String
        Return sFileName.Substring(4, 3).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������f�[�^�̃T�u��ʁi�p�^�[��No�܂��̓G���ANo�j���擾����B
    ''' </summary>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�f�[�^�̃T�u��ʁi�p�r���}�X�^�̏ꍇ�̓p�^�[��No�A�p�r���v���O�����̏ꍇ�̓G���ANo�j</returns>
    Public Shared Function GetDataSubKindAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetDataSubKind(sFileName))
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������f�[�^�̃T�u��ʁi�p�^�[��No�܂��̓G���ANo�j���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �Q���̕�����Ƃ��Ď擾����B
    ''' </remarks>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�f�[�^�̃T�u��ʁi�p�r���}�X�^�̏ꍇ�̓p�^�[��No�A�p�r���v���O�����̏ꍇ�̓G���ANo�j</returns>
    Public Shared Function GetDataSubKind(ByVal sFileName As String) As String
        Return sFileName.Substring(7, 2)
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������f�[�^�̓K�p�Ώۋ@����擾����B
    ''' </summary>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�f�[�^�̓K�p�Ώۋ@��i�p�r���}�X�^�̏ꍇ��"G"�܂���"Y"�A�p�r���v���O�����̏ꍇ��"W"�܂���"G"�܂���"Y"�j</returns>
    Public Shared Function GetDataApplicableModel(ByVal sFileName As String) As String
        Return sFileName.Substring(10, 1).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������f�[�^�̃o�[�W�����i�}�X�^�o�[�W�����܂��͑�\�o�[�W�����j���擾����B
    ''' </summary>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�f�[�^�̃o�[�W�����i�p�r���}�X�^�̏ꍇ�̓}�X�^�o�[�W�����A�p�r���v���O�����̏ꍇ�͑�\�o�[�W�����j</returns>
    Public Shared Function GetDataVersionAsInt(ByVal sFileName As String) As Integer
        Dim nextSepPos As Integer = sFileName.IndexOf("_"c, 13)
        Return Integer.Parse(sFileName.Substring(12, nextSepPos - 12))
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������f�[�^�̃o�[�W�����i�}�X�^�o�[�W�����܂��͑�\�o�[�W�����j���擾����B
    ''' </summary>
    ''' <remarks>
    ''' �}�X�^�o�[�W�����Ȃ�΂R���A��\�o�[�W�����Ȃ�΂W���܂��͂S���i�K�p�Ώۋ@��ɂ��j�̕�����Ƃ��Ď擾����B
    ''' </remarks>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�f�[�^�̃o�[�W�����i�p�r���}�X�^�̏ꍇ�̓}�X�^�o�[�W�����A�p�r���v���O�����̏ꍇ�͑�\�o�[�W�����j</returns>
    Public Shared Function GetDataVersion(ByVal sFileName As String) As String
        Dim intValue As Integer = GetDataVersionAsInt(sFileName)
        If GetListKind(sFileName).Equals("TGL") Then
            Return intValue.ToString("D3")
        Else
            Return intValue.ToString(EkConstants.ProgramDataVersionFormatOfModels(GetDataApplicableModel(sFileName)))
        End If
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������K�p���X�g���g�̃o�[�W�������擾����B
    ''' </summary>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�K�p���X�g���g�̃o�[�W����</returns>
    Public Shared Function GetListVersionAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetListVersion(sFileName))
    End Function

    ''' <summary>
    ''' �K�p���X�g�t�@�C��������K�p���X�g���g�̃o�[�W�������擾����B
    ''' </summary>
    ''' <remarks>
    ''' �Q���̕�����Ƃ��Ď擾����B
    ''' </remarks>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�K�p���X�g���g�̃o�[�W����</returns>
    Public Shared Function GetListVersion(ByVal sFileName As String) As String
        Dim startPos As Integer = sFileName.Length - 6
        Return sFileName.Substring(startPos, 2)
    End Function

    ''' <summary>
    ''' �����ȃt�@�C�����𐶐�����B
    ''' </summary>
    ''' <param name="sFileName">�K�p���X�g�t�@�C����</param>
    ''' <returns>�����ȓK�p���X�g�t�@�C����</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetDataPurpose(sFileName), GetDataKind(sFileName), GetDataSubKind(sFileName), GetDataApplicableModel(sFileName), GetDataVersion(sFileName), GetListVersion(sFileName))
    End Function

    ''' <summary>
    ''' �K�p���X�g�̃t�@�C�����𐶐�����B
    ''' </summary>
    ''' <remarks>
    ''' ��������t�@�C�����̌`���͖{�N���X�̓����ɃJ�v�Z�������Ă���B
    ''' </remarks>
    ''' <param name="sDataPurpose">�f�[�^�̗p�r�i"MST"�܂���"PRG"�j</param>
    ''' <param name="sDataKind">�f�[�^�̎�ʁi�p�r���}�X�^�̏ꍇ��"DSH"���A�p�r���v���O�����̏ꍇ��"WPG"���j</param>
    ''' <param name="sDataSubKind">�f�[�^�̃T�u��ʁi�p�r���}�X�^�̏ꍇ�̓p�^�[��No�A�p�r���v���O�����̏ꍇ�̓G���ANo�j</param>
    ''' <param name="sDataApplicableModel">�f�[�^�̓K�p�Ώۋ@��i�p�r���}�X�^�̏ꍇ��"G"�܂���"Y"�A�p�r���v���O�����̏ꍇ��"W"�܂���"G"�܂���"Y"�j</param>
    ''' <param name="sDataVersion">�f�[�^�̃o�[�W�����i�p�r���}�X�^�̏ꍇ�̓}�X�^�o�[�W�����A�p�r���v���O�����̏ꍇ�͑�\�o�[�W�����j</param>
    ''' <param name="sListVersion">�K�p���X�g���g�̃o�[�W����</param>
    ''' <returns>�K�p���X�g�t�@�C����</returns>
    Public Shared Function Gen( _
       ByVal sDataPurpose As String, _
       ByVal sDataKind As String, _
       ByVal sDataSubKind As String, _
       ByVal sDataApplicableModel As String, _
       ByVal sDataVersion As String, _
       ByVal sListVersion As String) As String

        Select Case sDataPurpose
            Case "MST"
                Return "TGL_" & sDataKind & sDataSubKind & "_" & sDataApplicableModel & "_" & sDataVersion & "_" & sListVersion & ".csv"
            Case "PRG"
                Return "TDL_" & sDataKind & sDataSubKind & "_" & sDataApplicableModel & "_" & Integer.Parse(sDataVersion).ToString("D8") & "_" & sListVersion & ".csv"
            Case Else
                Return Nothing
        End Select
    End Function
#End Region

End Class
