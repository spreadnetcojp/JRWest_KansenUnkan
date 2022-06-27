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

'枝番つき日付別ディレクトリ名を取り扱うためのクラス
Public Class TimestampedDirPath

#Region "定数"
    Private Const timestampPos As Integer = 0
    Private Const timestampLen As Integer = 8
    Private Const branchNumPos As Integer = 9
    Private Const sSeparator As String = "_"
    Private Const sTimestampFormat As String = "yyyyMMdd"
    Private Const sPattern As String = "????????_*"
    Private Shared ReadOnly sRegx As New Regex("^[0-9]{8}_[0-9]+$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
#End Region

#Region "メソッド"
    Public Shared Function FindLatest(ByVal sBaseDirPath As String) As DirectoryInfo
        Dim oDirInfo As New DirectoryInfo(sBaseDirPath)
        Dim aDirectoryInfo As DirectoryInfo() = oDirInfo.GetDirectories(sPattern)
        Dim oLatestDirectoryInfo As DirectoryInfo = Nothing
        For Each oDirectoryInfo As DirectoryInfo In aDirectoryInfo
            If Not sRegx.IsMatch(oDirectoryInfo.Name) Then Continue For

            If oLatestDirectoryInfo Is Nothing Then
                oLatestDirectoryInfo = oDirectoryInfo
            Else
                Dim result As Integer = String.CompareOrdinal(oDirectoryInfo.Name, 0, oLatestDirectoryInfo.Name, 0, timestampLen)
                If result > 0 Then
                    oLatestDirectoryInfo = oDirectoryInfo
                ElseIf result = 0
                    Dim branch As Integer = Integer.Parse(oDirectoryInfo.Name.Substring(branchNumPos))
                    Dim latestBranch As Integer = Integer.Parse(oLatestDirectoryInfo.Name.Substring(branchNumPos))
                    If branch > latestBranch Then
                        oLatestDirectoryInfo = oDirectoryInfo
                    End If
                End If
            End If
        Next oDirectoryInfo
        Return oLatestDirectoryInfo
    End Function

    Public Shared Function IsMatch(ByVal sPath As String) As Boolean
        Return sRegx.IsMatch(Path.GetFileName(sPath))
    End Function

    Public Shared Function GetTimestamp(ByVal sPath As String) As DateTime
        Return DateTime.ParseExact(GetTimestampString(sPath), sTimestampFormat, CultureInfo.InvariantCulture)
    End Function

    Public Shared Function GetTimestampString(ByVal sPath As String) As String
        Return Path.GetFileName(sPath).Substring(timestampPos, timestampLen)
    End Function

    Public Shared Function Gen(ByVal sBaseDirPath As String, ByVal timestamp As DateTime) As String
        Dim sTimestamp As String = timestamp.ToString(sTimestampFormat)
        Dim oDirInfo As New DirectoryInfo(sBaseDirPath)
        Dim aDirectoryInfo As DirectoryInfo() = oDirInfo.GetDirectories(sTimestamp & "_*")
        Dim latestBranch As Integer = 0
        For Each oDirectoryInfo As DirectoryInfo In aDirectoryInfo
            If Not sRegx.IsMatch(oDirectoryInfo.Name) Then Continue For

            Dim branch As Integer = Integer.Parse(oDirectoryInfo.Name.Substring(branchNumPos))
            If branch > latestBranch Then
                latestBranch = branch
            End If
        Next oDirectoryInfo

        latestBranch += 1
        Return Path.Combine(sBaseDirPath, sTimestamp & sSeparator & latestBranch.ToString())
    End Function
#End Region

End Class
