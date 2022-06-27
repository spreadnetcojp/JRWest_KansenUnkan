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

Imports System.IO
Imports System.Text.RegularExpressions

Imports JR.ExOpmg.Common

Public Class SweepUtil

    Private Shared ReadOnly oOperationLogFileNameRegx As New Regex("^[0-9]{8}-ClientApp-Operation(-[0-9]+){0,1}\.csv$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const sOperationLogFileNamePattern As String = "????????-ClientApp-Operation*.csv"

    Private Shared ReadOnly oAnyLogFileNameRegx As New Regex("^[0-9]{8}-ClientApp-[0-9A-Z_\-]+\.csv$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const sAnyLogFileNamePattern As String = "????????-ClientApp-*.csv"

    Public Shared Sub SweepOperationLogs(ByVal sLogBasePath As String)
        Try
            'Config.OperationLogsKeepingDaysが経過した操作ログを
            'sLogBasePathのディレクトリから削除する。
            Log.Info("Sweeping operation logs...")

            Dim boundDate As Integer = Integer.Parse(DateTime.Now.AddDays(-Config.OperationLogsKeepingDays).ToString("yyyyMMdd"))
            For Each sFile As String In Directory.GetFiles(sLogBasePath, sOperationLogFileNamePattern)
                Dim sFileName As String = Path.GetFileName(sFile)
                If oOperationLogFileNameRegx.IsMatch(sFileName) AndAlso _
                   Integer.Parse(sFileName.Substring(0, 8)) < boundDate Then
                    File.Delete(sFile)
                    Log.Info("The file [" & sFile & "] deleted.")
                End If
            Next sFile

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SweepOperationLogsFailed)
        End Try
    End Sub

    Public Shared Sub SweepLogs(ByVal sLogBasePath As String)
        Try
            'Config.LogsKeepingDaysが経過した操作ログ以外のログを
            'sLogBasePathのディレクトリから削除する。
            Log.Info("Sweeping logs...")

            Dim boundDate As Integer = Integer.Parse(DateTime.Now.AddDays(-Config.LogsKeepingDays).ToString("yyyyMMdd"))
            For Each sFile As String In Directory.GetFiles(sLogBasePath, sAnyLogFileNamePattern)
                Dim sFileName As String = Path.GetFileName(sFile)
                If Not oOperationLogFileNameRegx.IsMatch(sFileName) AndAlso _
                   oAnyLogFileNameRegx.IsMatch(sFileName) AndAlso _
                   Integer.Parse(sFileName.Substring(0, 8)) < boundDate Then
                    File.Delete(sFile)
                    Log.Info("The file [" & sFile & "] deleted.")
                End If
            Next sFile

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SweepLogsFailed)
        End Try
    End Sub

End Class
