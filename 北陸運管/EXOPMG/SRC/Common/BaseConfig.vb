' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2014/04/01  (NES)�͘e  �k���Ή��FINI�t�@�C���̉σL�[���ڑΉ�
'   0.2      2017/04/10  (NES)����  ������ԕ�Ή��ɂāAGetFileSectionKeys
'                                   GetFileSectionAsDictionary�A
'                                   GetFileSectionAsDataTable��ǉ�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Data
Imports System.Runtime.InteropServices
Imports System.Text

''' <summary>
''' �萔�R���e�i�̊�{�N���X
''' </summary>
Public Class BaseConfig
    '�f�[�^�x�[�X�ڑ��p���
    Public Shared DatabaseServerName As String
    Public Shared DatabaseName As String
    Public Shared DatabaseUserName As String
    Public Shared DatabasePassword As String

    '�f�[�^�x�[�X�֘A�^�C�}�l
    Public Shared DatabaseReadLimitSeconds As Integer
    Public Shared DatabaseWriteLimitSeconds As Integer

    'INI�t�@�C�����̃Z�N�V������
    Protected Const DATABASE_SECTION As String = "Database"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const DATABASE_SERVER_NAME_KEY As String = "ServerName"
    Private Const DATABASE_NAME_KEY As String = "Name"
    Private Const DATABASE_USER_NAME_KEY As String = "UserName"
    Private Const DATABASE_PASSWORD_KEY As String = "Password"
    Private Const DATABASE_READ_LIMIT_KEY As String = "ReadLimitSeconds"
    Private Const DATABASE_WRITE_LIMIT_KEY As String = "WriteLimitSeconds"

    Protected Shared IniFileParh As String
    Protected Shared LastReadSection As String = ""
    Protected Shared LastReadKey As String = ""
    Protected Shared LastReadValue As String = ""

    Private Declare Ansi Function GetPrivateProfileStringToBytes Lib "KERNEL32.DLL" _
       Alias "GetPrivateProfileStringA" ( _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpApplicationName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String, _
        <MarshalAs(UnmanagedType.LPArray, ArraySubType:=UnmanagedType.U1)> ByVal lpReturnedString As Byte(), _
        ByVal nSize As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String _
      ) As Integer

    ''' <summary>
    ''' INI�t�@�C������w�荀�ڂ̐ݒ�l��ǂݎ��B
    ''' </summary>
    ''' <param name="sectionName">�Z�N�V�����̖���</param>
    ''' <param name="keyName">�L�[</param>
    ''' <param name="ValueCheck">Value�l�`�F�b�N�L�� True�F�L�AFalse�F��</param>
    ''' <remarks>Constant.GetIni���g�p����B</remarks>
    Protected Shared Sub ReadFileElem(ByVal sectionName As String, ByVal keyName As String, Optional ByVal ValueCheck As Boolean = True)
        ' --- Ver0.1 �k���Ή��FINI�t�@�C���̉σL�[���ڑΉ� MOD
        'Protected Shared Sub ReadFileElem(ByVal sectionName As String, ByVal keyName As String)
        If (String.IsNullOrEmpty(sectionName) OrElse String.IsNullOrEmpty(keyName)) Then
            Throw New OPMGException("Invalid parameter.")
        End If

        LastReadSection = sectionName
        LastReadKey = keyName

        LastReadValue = Constant.GetIni(sectionName, keyName, IniFileParh)
        ' --- Ver0.1 �k���Ή��FINI�t�@�C���̉σL�[���ڑΉ� MOD
        'If LastReadValue Is Nothing Then
        If (LastReadValue Is Nothing) And ValueCheck Then
            Throw New OPMGException("It's not defined or has too long value. (Section: " & sectionName & ", Key: " & keyName & ")")
        End If
    End Sub

    ''' <summary>
    ''' INI�t�@�C������w��Z�N�V�����Ɋ܂܂��S�ẴL�[��ǂݎ��AString�z��Ƃ��ĕԋp����B
    ''' </summary>
    ''' <param name="sectionName">�Z�N�V�����̖���</param>
    Protected Shared Function GetFileSectionKeys(ByVal sectionName As String) As String()
        Try
            '�Z�N�V�������̑S�L�[���k����؂�Ńo�C�g����Ɏ擾����B
            Dim bytes(65535) As Byte
            Dim validLengthOfBytes As Integer = _
               GetPrivateProfileStringToBytes(sectionName, Nothing, "[]_", bytes, bytes.Length, IniFileParh)
            If validLengthOfBytes = 0 Then
                'INI�t�@�C���⏊��Z�N�V�����͑��݂��A�L�[���P���Ȃ��ꍇ�ł���B
                Return New String(-1) {}
            End If
            If validLengthOfBytes = CUInt(bytes.Length - 2) Then
                '�S�L�[���o�b�t�@�ɓ��肫��Ȃ������\��������ꍇ�ł���B
                Throw New OPMGException("The [" & sectionName & "] section might contain too many keys.")
            End If

            '�o�C�g���String�ɕϊ�����B
            Dim sNullSeparatedKeys As String = Encoding.Default.GetString(bytes, 0, validLengthOfBytes - 1)
            If sNullSeparatedKeys.Equals("[]") Then
                'INI�t�@�C���܂��͏���Z�N�V���������݂��Ȃ��ꍇ�ł���B
                Throw New OPMGException("The [" & sectionName & "] section not found.")
            End If

            '�e�L�[��v�f�Ƃ���String�z����쐬���A�ԋp����B
            Return sNullSeparatedKeys.Split(Chr(0))

        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            'NOTE: GetPrivateProfileStringToBytes()��Encoding.Default.GetString()�ŗ�O���X���[���ꂽ�ꍇ��z��B
            Throw New OPMGException("Something may be wrong. (Section: " & sectionName & ")", ex)
        End Try
    End Function

    ''' <summary>
    ''' INI�t�@�C������w��Z�N�V�����Ɋ܂܂��S�ẴL�[�Ɛݒ�l��ǂݎ��ADictionary�Ƃ��ĕԋp����B
    ''' </summary>
    ''' <param name="sectionName">�Z�N�V�����̖���</param>
    ''' <remarks>Constant.GetIni���g�p����B</remarks>
    Protected Shared Function GetFileSectionAsDictionary(ByVal sectionName As String) As Dictionary(Of String, String)
        Dim keyNames As String() = GetFileSectionKeys(sectionName)
        Dim dic As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Try
            For Each keyName As String In keyNames
                ReadFileElem(sectionName, keyName)
                dic.Add(keyName, LastReadValue)
            Next keyName
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("Something may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try

        Return dic
    End Function

    ''' <summary>
    ''' INI�t�@�C������w��Z�N�V�����Ɋ܂܂��S�ẴL�[�Ɛݒ�l��ǂݎ��ADataTable�Ƃ��ĕԋp����B
    ''' </summary>
    ''' <param name="sectionName">�Z�N�V�����̖���</param>
    ''' <remarks>Constant.GetIni���g�p����B</remarks>
    Protected Shared Function GetFileSectionAsDataTable(ByVal sectionName As String, Optional ByVal addEmptyRow As Boolean = False) As DataTable
        Dim keyNames As String() = GetFileSectionKeys(sectionName)
        Dim dt As New DataTable()
        dt.Columns.Add("Key", GetType(String))
        dt.Columns.Add("Value", GetType(String))
        If addEmptyRow Then
            Dim row As DataRow = dt.NewRow()
            row("Key") = ""
            row("Value") = ""
            dt.Rows.Add(row)
        End If

        Try
            For Each keyName As String In keyNames
                ReadFileElem(sectionName, keyName)
                Dim row As DataRow = dt.NewRow()
                row("Key") = keyName
                row("Value") = LastReadValue
                dt.Rows.Add(row)
            Next keyName
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("Something may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try

        Return dt
    End Function

    ''' <summary>INI�t�@�C������Common���C�u�����ɕK�{�̐ݒ�l����荞�ށB</summary>
    Protected Shared Sub BaseInit(ByVal sIniFilePath As String)
        IniFileParh = sIniFilePath
        Try
            ReadFileElem(DATABASE_SECTION, DATABASE_SERVER_NAME_KEY)
            DatabaseServerName = LastReadValue

            ReadFileElem(DATABASE_SECTION, DATABASE_NAME_KEY)
            DatabaseName = LastReadValue

            ReadFileElem(DATABASE_SECTION, DATABASE_USER_NAME_KEY)
            DatabaseUserName = LastReadValue

            ReadFileElem(DATABASE_SECTION, DATABASE_PASSWORD_KEY)
            DatabasePassword = LastReadValue

            ReadFileElem(DATABASE_SECTION, DATABASE_READ_LIMIT_KEY)
            DatabaseReadLimitSeconds = Integer.Parse(LastReadValue)

            ReadFileElem(DATABASE_SECTION, DATABASE_WRITE_LIMIT_KEY)
            DatabaseWriteLimitSeconds = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
