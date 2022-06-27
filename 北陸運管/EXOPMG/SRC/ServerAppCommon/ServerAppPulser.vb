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
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' 生存証明ファイルを更新するクラス。
''' </summary>
Public Class ServerAppPulser

#Region "メソッド"
    Public Shared Sub Pulse()
        Try
            Dim aBytes(14 - 1) As Byte
            Dim sTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")
            Encoding.UTF8.GetBytes(sTime, 0, 14, aBytes, 0)

            Dim sFilePath As String = Path.Combine(ServerAppBaseConfig.ResidentAppPulseDirPath, ServerAppBaseConfig.AppIdentifier)
            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oOutputStream.Write(aBytes, 0, 14)
            End Using
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub
#End Region

End Class
