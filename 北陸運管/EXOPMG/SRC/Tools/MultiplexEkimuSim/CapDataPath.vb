' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions

Imports JR.ExOpmg.Common

Public Class CapDataPath

#Region "定数"
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
    Private Const dateLenInFileName As Integer = 8  'YYYYMMDD部分のみの文字数（過去日データ退避の実装用）
    Private Const branchNumPosInFileName As Integer = 23
#End Region

#Region "メソッド"
    'NOTE: たぶん使うことはない。
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

    'NOTE: たぶん使うことはない。
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

    'NOTE: たぶん使うことはない。
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

    'NOTE: たぶん使うことはない。
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

    'NOTE: 特定のディレクトリに特定種別のファイルを作成するのは特定プロセスの特定スレッドのみとする。
    Public Shared Function Gen(ByVal sDestDirPath As String, ByVal timestamp As DateTime, ByVal direction As String, ByVal transKind As String) As String
        Debug.Assert(direction.Equals("S") OrElse direction.Equals("R"))
        Debug.Assert(transKind.Equals("T") OrElse transKind.Equals("F"))
        Return GenCore(sDestDirPath, timestamp.ToString(sTimestampFormat) & "_" & direction & "_" & transKind & "_", 1)
    End Function

    '枝番部分をゼロサプレスしたファイル名をsDestDirPathに結合して、パスを完成させる。
    'ただし、同名ファイルと衝突する場合は、枝番部分の数値自体も変更する。
    'NOTE: 特定のディレクトリに特定種別のファイルを作成するのは特定プロセスの特定スレッドのみとする。
    'NOTE: たぶん使うことはない。
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
        'NOTE: 同名のディレクトリが存在することはないという前提である。
        While File.Exists(sDestPath)
            branchNum += 1
            sDestPath = sDestPathBeforeBranchNum & branchNum.ToString() & sFileExtension
        End While
        Return sDestPath
    End Function
#End Region

End Class
