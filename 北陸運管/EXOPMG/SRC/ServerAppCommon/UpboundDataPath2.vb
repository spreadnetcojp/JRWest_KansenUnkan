' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/04/10  (NES)����  ������ԕ�Ή��ɂĐV�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions

Imports JR.ExOpmg.Common

'���f�[�^�̃t�@�C��������舵�����߂̃N���X
Public Class UpboundDataPath2

#Region "�萔"
    Private Const sEkCodeFormat As String = "%2M_%3R%3S_%4C_%2U_"
    Private Const sTimestampFormat As String = "yyyyMMddHHmmss"
    Private Shared ReadOnly oFileNameRegx As New Regex("^[0-9]{2}_[0-9]{6}_[0-9]{4}_[0-9]{2}_[0-9A-Z]{2}_[0-9]{14}_[0-9]+\.dat$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const sFileNamePattern As String = "??_??????_????_??_??_??????????????_*.dat"
    Private Const sFileExtension As String = ".dat"
    Private Const fileExtensionLen As Integer = 4
    Private Const formatCodePosInFileName As Integer = 18
    Private Const formatCodeLenInFileName As Integer = 2
    Private Const timestampPosInFileName As Integer = 21
    Private Const timestampLenInFileName As Integer = 14
    Private Const datePosInFileName As Integer = 21
    Private Const dateLenInFileName As Integer = 8
    Private Const timePosInFileName As Integer = 29
    Private Const timeLenInFileName As Integer = 6
    Private Const branchNumPosInFileName As Integer = 36
#End Region

#Region "���\�b�h"
    Public Shared Function FindEarliest(ByVal sDirPath As String) As FileInfo
        Dim oDirInfo As New DirectoryInfo(sDirPath)
        Dim aFileInfo As FileInfo() = oDirInfo.GetFiles(sFileNamePattern)
        Dim oEarliestFileInfo As FileInfo = Nothing
        For Each oFileInfo As FileInfo In aFileInfo
            If oEarliestFileInfo Is Nothing OrElse _
               oFileInfo.CreationTime < oEarliestFileInfo.CreationTime Then
                If oFileNameRegx.IsMatch(oFileInfo.Name) Then
                    oEarliestFileInfo = oFileInfo
                End If
            End If
        Next oFileInfo
        Return oEarliestFileInfo
    End Function

    Public Shared Function FindNames(ByVal sDirPath As String, ByRef combinedContentLen As Long, Optional ByVal combinedContentMaxLen As Long = -1) As List(Of String)
        Dim oDirInfo As New DirectoryInfo(sDirPath)
        Dim aFileInfo As FileInfo() = oDirInfo.GetFiles(sFileNamePattern)
        Dim oOutList As New List(Of String)(aFileInfo.Length)
        Dim totalLen As Long = 0
        For Each oFileInfo As FileInfo In aFileInfo
            If oFileNameRegx.IsMatch(oFileInfo.Name) Then
                Dim nextTotalLen As Long
                nextTotalLen = totalLen + oFileInfo.Length
                If combinedContentMaxLen >= 0 AndAlso nextTotalLen > combinedContentMaxLen Then
                    Log.Warn("Too many or too large files detected.")
                    Exit For
                End If
                totalLen = nextTotalLen
                oOutList.Add(oFileInfo.Name)
            End If
        Next oFileInfo
        combinedContentLen = totalLen
        Return oOutList
    End Function

    Public Shared Function FindFullNames(ByVal sDirPath As String, ByRef combinedContentLen As Long, Optional ByVal combinedContentMaxLen As Long = -1) As List(Of String)
        Dim oDirInfo As New DirectoryInfo(sDirPath)
        Dim aFileInfo As FileInfo() = oDirInfo.GetFiles(sFileNamePattern)
        Dim oOutList As New List(Of String)(aFileInfo.Length)
        Dim totalLen As Long = 0
        For Each oFileInfo As FileInfo In aFileInfo
            If oFileNameRegx.IsMatch(oFileInfo.Name) Then
                Dim nextTotalLen As Long
                nextTotalLen = totalLen + oFileInfo.Length
                If combinedContentMaxLen >= 0 AndAlso nextTotalLen > combinedContentMaxLen Then
                    Log.Warn("Too many or too large files detected.")
                    Exit For
                End If
                totalLen = nextTotalLen
                oOutList.Add(oFileInfo.FullName)
            End If
        Next oFileInfo
        combinedContentLen = totalLen
        Return oOutList
    End Function

    Public Shared Function GetContentsLength(ByVal sDirPath As String) As Long
        Dim oDirInfo As New DirectoryInfo(sDirPath)
        Dim aFileInfo As FileInfo() = oDirInfo.GetFiles(sFileNamePattern)
        Dim totalLen As Long = 0
        For Each oFileInfo As FileInfo In aFileInfo
            If oFileNameRegx.IsMatch(oFileInfo.Name) Then
                totalLen += oFileInfo.Length
            End If
        Next oFileInfo
        Return totalLen
    End Function

    Public Shared Function IsMatch(ByVal sPath As String) As Boolean
        Return oFileNameRegx.IsMatch(Path.GetFileName(sPath))
    End Function

    Public Shared Function GetEkCode(ByVal sPath As String) As EkCode
        Return EkCode.Parse(Path.GetFileName(sPath).Substring(0, timestampPosInFileName), sEkCodeFormat)
    End Function

    Public Shared Function GetFormatCode(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(formatCodePosInFileName, formatCodeLenInFileName)
    End Function

    Public Shared Function GetTimestamp(ByVal sPath As String) As DateTime
        Return DateTime.ParseExact(GetTimestampString(sPath), sTimestampFormat, CultureInfo.InvariantCulture)
    End Function

    Public Shared Function GetTimestampString(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(timestampPosInFileName, timestampLenInFileName)
    End Function

    Public Shared Function GetDateString(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(datePosInFileName, dateLenInFileName)
    End Function

    Public Shared Function GetTimeString(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(timePosInFileName, timeLenInFileName)
    End Function

    Public Shared Function GetBranchNumber(ByVal sPath As String) As Integer
        Dim sFileName As String = Path.GetFileName(sPath)
        Dim branchNumLenInFileName As Integer = sFileName.Length - (branchNumPosInFileName + fileExtensionLen)
        Return Utility.GetIntFromDecimalString(sFileName, branchNumPosInFileName, branchNumLenInFileName)
    End Function

    'NOTE: sDestDirPath�������f�B���N�g���ɓ���EkCode�̃t�@�C�����쐬����͓̂���v���Z�X�̓���X���b�h�݂̂Ƃ���B
    Public Shared Function Gen(ByVal sDestDirPath As String, ByVal ekCode As EkCode, ByVal formatCode As String, ByVal timestamp As DateTime) As String
        Return GenCore(sDestDirPath, ekCode.ToString(sEkCodeFormat) & formatCode & "_" & timestamp.ToString(sTimestampFormat) & "_", 1)
    End Function

    '�}�ԕ������[���T�v���X�����t�@�C������sDestDirPath�Ɍ������āA�p�X������������B
    '�������A�����t�@�C���ƏՓ˂���ꍇ�́A�}�ԕ����̐��l���̂��ύX����B
    'NOTE: sDestDirPath�������f�B���N�g���ɓ���EkCode�̃t�@�C�����쐬����͓̂���v���Z�X�̓���X���b�h�݂̂Ƃ���B
    Public Shared Function Gen(ByVal sDestDirPath As String, ByVal sIntendedFileName As String) As String
        Dim branchNumLenInFileName As Integer = sIntendedFileName.Length - (branchNumPosInFileName + fileExtensionLen)
        Dim branchNum As Integer = Utility.GetIntFromDecimalString(sIntendedFileName, branchNumPosInFileName, branchNumLenInFileName)
        Return GenCore(sDestDirPath, sIntendedFileName.Substring(0, branchNumPosInFileName), branchNum)
    End Function

    Private Shared Function GenCore(ByVal sDestDirPath As String, ByVal sFileNameBeforeBranchNum As String, ByVal minBranchNum As Integer) As String
        Dim branchNum As Integer = minBranchNum
        Dim sDestPathBeforeBranchNum As String = Path.Combine(sDestDirPath, sFileNameBeforeBranchNum)
        Dim sDestPath As String = sDestPathBeforeBranchNum & branchNum.ToString() & sFileExtension
        'NOTE: �����̃f�B���N�g�������݂��邱�Ƃ͂Ȃ��Ƃ����O��ł���B
        While File.Exists(sDestPath)
            branchNum += 1
            sDestPath = sDestPathBeforeBranchNum & branchNum.ToString() & sFileExtension
        End While
        Return sDestPath
    End Function
#End Region

End Class
