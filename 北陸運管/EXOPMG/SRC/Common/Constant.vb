' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  �ԉ��T�[�o�ł̉��P���t�B�[�h�o�b�N
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Runtime.InteropServices
Imports System.Text

''' <summary>
''' �ݒ�l���o�̓N���X
''' </summary>
Public Class Constant

#Region "�v���C�x�[�g�t�B�[���h"
    Private Const BUFFER_LEN As Integer = 256
#End Region

#Region "API�錾"
    <DllImport("KERNEL32.DLL", CharSet:=CharSet.Auto)> _
    Private Shared Function GetPrivateProfileString( _
       ByVal lpAppName As String, _
       ByVal lpKeyName As String, _
       ByVal lpDefault As String, _
       ByVal lpReturnedString As System.Text.StringBuilder, _
       ByVal nSize As Integer, _
       ByVal lpFileName As String) As Integer
    End Function
    <DllImport("KERNEL32.DLL")> _
    Private Shared Function WritePrivateProfileString( _
       ByVal lpAppName As String, _
       ByVal lpKeyName As String, _
       ByVal lpString As String, _
       ByVal lpFileName As String) As Integer
    End Function
#End Region

#Region "���ϐ��擾"
    ''' <summary>
    ''' [���ϐ��擾]
    ''' </summary>
    ''' <remarks>
    ''' ���ϐ�����`����Ă��Ȃ��ꍇ��Nothing��ԋp����B
    ''' ��`�l��0�o�C�g�̕�����̏ꍇ�A0������String��ԋp����B
    ''' �����̕s�������������ꍇ�͗�O�iOPMGException�ȊO�j�𐶐�����B
    ''' </remarks>
    ''' <param name="sName">���ϐ���</param>
    ''' <returns>�擾�l</returns>
    Public Shared Function GetEnv(ByVal sName As String) As String
        'NOTE: OS�̑Ή��L��������Ɉˑ����Ĕ��������O�͂��̂܂�Throw����B
        Dim sRtn As String
        sRtn = System.Environment.GetEnvironmentVariable(sName, EnvironmentVariableTarget.Machine)
        If sRtn = Nothing Then
            sRtn = System.Environment.GetEnvironmentVariable(sName, EnvironmentVariableTarget.Process)
        End If
        If sRtn = Nothing Then
            sRtn = System.Environment.GetEnvironmentVariable(sName, EnvironmentVariableTarget.Machine)
        End If
        Return sRtn
    End Function
#End Region

#Region "�ݒ���擾"
    ''' <summary>
    ''' [�ݒ���擾]
    ''' �w��INI�t�@�C������ݒ����ǂݏo���B
    ''' </summary>
    ''' <remarks>
    ''' �w��INI�t�@�C���A�w��Z�N�V�����A�w��L�[�̂����ꂩ�����݂��Ȃ��ꍇ��Nothing��ԋp����B
    ''' �ݒ�l����������ꍇ��Nothing��ԋp����B
    ''' �ݒ�l��0�o�C�g�̕�����̏ꍇ�A0������String��ԋp����B
    ''' �����̕s�������������ꍇ�͗�O�iOPMGException�ȊO�j�𐶐�����B
    ''' </remarks>
    ''' <param name="SectionName">�Z�N�V������</param>
    ''' <param name="KeyName">�L�[��</param>
    ''' <param name="FileFullName">INI�t�@�C����΃p�X��</param>
    ''' <returns>�擾�l</returns>
    Public Shared Function GetIni(ByVal SectionName As String, ByVal KeyName As String, ByVal FileFullName As String) As String
        Dim sb As StringBuilder = New StringBuilder(BUFFER_LEN)
        'NOTE: API������SEH��O�����o���ꂽ�ۂ́ACLR�����炩��Exception�𐶐�����z��B
        '���̃P�[�X�͐ݒ�t�@�C���Ɉˑ����Ȃ��v���O�����̃o�O�ł��邽�߁A
        '���̃��\�b�h�̌Ăь��ɂ��̂܂܃X���[����B
        GetPrivateProfileString(SectionName, KeyName, vbLf, sb, BUFFER_LEN, FileFullName)

        Dim s As String = sb.ToString()
        '�w��̃t�@�C���܂��͎w��̐ݒ荀�ڂ����݂��Ȃ��ꍇ��Nothing��ԋp�B
        If s.Equals(vbLf) Then Return Nothing
        'API�Ɏw�肵���o�b�t�@�ɓ��肫��Ȃ������\��������ꍇ��Nothing��ԋp�B
        If s.Length >= BUFFER_LEN Then Return Nothing
        Return s
    End Function
#End Region

#Region "�ݒ��񏑍�"
    ''' <summary>
    ''' [�ݒ��񏑍�]
    ''' �w��INI�t�@�C���ɐݒ�����������ށB
    ''' </summary>
    ''' <param name="SectionName">�Z�N�V������</param>
    ''' <param name="KeyName">�L�[��</param>
    ''' <param name="FileFullName">INI�t�@�C����΃p�X��</param>
    ''' <param name="Value">�ݒ�l</param>
    ''' <returns>True:����,False:���s</returns>
    Public Shared Function SetIni(ByVal SectionName As String, ByVal KeyName As String, ByVal FileFullName As String, ByVal Value As String) As Boolean
        Try
            Dim sDir As String = System.IO.Path.GetDirectoryName(FileFullName)
            If Not System.IO.Directory.Exists(sDir) Then    '�t�H���_���Ȃ��ꍇ�쐬����
                System.IO.Directory.CreateDirectory(sDir)
            End If
            If WritePrivateProfileString(SectionName, KeyName, Value, FileFullName) = 0 Then
                Throw New System.ArgumentException("WritePrivateProfileString(" & SectionName & ", " & KeyName & ", " & Value & ", " & FileFullName & ") failed.")
            End If
            Return True
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            Return False
        End Try
    End Function
#End Region

End Class
