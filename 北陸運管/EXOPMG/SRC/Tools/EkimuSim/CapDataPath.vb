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

Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions

Imports JR.ExOpmg.Common

Public Class CapDataPath

#Region "�萔"
    Private Const sTimestampFormat As String = "yyyyMMdd_HHmmssfff"
    Private Shared ReadOnly oFileNameRegx As New Regex("^[0-9]{8}_[0-9]{9}_(S|R)_(T|F)_[0-9]+\.dat$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const sFileNamePattern As String = "????????_?????????_?_?_*.dat"
    Private Const sFileExtension As String = ".dat"
    Private Const fileExtensionLen As Integer = 4
    Private Const timestampPosInFileName As Integer = 0
    Private Const timestampLenInFileName As Integer = 18
    Private Const directionPosInFileName As Integer = 19
    Private Const directionLenInFileName As Integer = 1
    Private Const transKindPosInFileName As Integer = 21
    Private Const transKindLenInFileName As Integer = 1
    Private Const dateLenInFileName As Integer = 8  'YYYYMMDD�����݂̂̕������i�ߋ����f�[�^�ޔ��̎����p�j
    Private Const branchNumPosInFileName As Integer = 23
#End Region

#Region "���\�b�h"
    'NOTE: ���Ԃ�g�����Ƃ͂Ȃ��B
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

    'NOTE: ���Ԃ�g�����Ƃ͂Ȃ��B
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

    'NOTE: ���Ԃ�g�����Ƃ͂Ȃ��B
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

    'NOTE: ���Ԃ�g�����Ƃ͂Ȃ��B
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

    Public Shared Function GetTimestamp(ByVal sPath As String) As DateTime
        Return DateTime.ParseExact(GetTimestampString(sPath), sTimestampFormat, CultureInfo.InvariantCulture)
    End Function

    Public Shared Function GetTimestampString(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(timestampPosInFileName, timestampLenInFileName)
    End Function

    Public Shared Function GetDateString(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(timestampPosInFileName, dateLenInFileName)
    End Function

    Public Shared Function GetDirectionString(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(directionPosInFileName, directionLenInFileName)
    End Function

    Public Shared Function GetTransKindString(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(transKindPosInFileName, transKindLenInFileName)
    End Function

    'NOTE: ����̃f�B���N�g���ɓ����ʂ̃t�@�C�����쐬����͓̂���v���Z�X�̓���X���b�h�݂̂Ƃ���B
    Public Shared Function Gen(ByVal sDestDirPath As String, ByVal timestamp As DateTime, ByVal direction As String, ByVal transKind As String) As String
        Debug.Assert(direction.Equals("S") OrElse direction.Equals("R"))
        Debug.Assert(transKind.Equals("T") OrElse transKind.Equals("F"))
        Return GenCore(sDestDirPath, timestamp.ToString(sTimestampFormat) & "_" & direction & "_" & transKind & "_", 1)
    End Function

    '�}�ԕ������[���T�v���X�����t�@�C������sDestDirPath�Ɍ������āA�p�X������������B
    '�������A�����t�@�C���ƏՓ˂���ꍇ�́A�}�ԕ����̐��l���̂��ύX����B
    'NOTE: ����̃f�B���N�g���ɓ����ʂ̃t�@�C�����쐬����͓̂���v���Z�X�̓���X���b�h�݂̂Ƃ���B
    'NOTE: ���Ԃ�g�����Ƃ͂Ȃ��B
    Public Shared Function Gen(ByVal sDestDirPath As String, ByVal sIntendedFileName As String) As String
        Debug.Assert(oFileNameRegx.IsMatch(sIntendedFileName))
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
