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
Imports System.Net
Imports System.Net.Cache
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' ファイル転送の全手続きを裏で（生成元と非同期に）行うクラス（履歴保存機能つき）。
''' </summary>
''' <remarks>
''' ファイル転送の方法はFTPである。
''' </remarks>
Public Class MyFtpWorker
    Inherits FtpWorker

#Region "定数や変数"
    '送受信履歴ディレクトリ
    Protected sCapDirPath As String

    '送受信履歴の通信種別
    Protected sCapTransKind As String

    'メインフォームへの参照
    Protected oForm As MainForm
#End Region

#Region "コンストラクタ"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal sForeignBaseUri As String, _
       ByVal oCredential As NetworkCredential, _
       ByVal requestLimitTicks As Integer, _
       ByVal logoutLimitTicks As Integer, _
       ByVal transferStallLimitTicks As Integer, _
       ByVal usePassiveMode As Boolean, _
       ByVal logoutEachTime As Boolean, _
       ByVal bufferLength As Integer, _
       ByVal sCapDirPath As String, _
       ByVal sCapTransKind As String, _
       ByVal oForm As MainForm)

        MyBase.New( _
           sThreadName, _
           oParentMessageSock, _
           sForeignBaseUri, _
           oCredential, _
           requestLimitTicks, _
           logoutLimitTicks, _
           transferStallLimitTicks, _
           usePassiveMode, _
           logoutEachTime, _
           bufferLength)

        Me.sCapDirPath = sCapDirPath
        Me.sCapTransKind = sCapTransKind
        Me.oForm = oForm
    End Sub
#End Region

#Region "メソッド"
    Protected Overrides Function ProcOnDownloadRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Dim isOk As Boolean = MyBase.DoDownload(oRcvMsg)

        If isOk Then
            Dim capRcvFiles As Boolean
            SyncLock oForm.UiState
                capRcvFiles = oForm.UiState.CapRcvFiles
            End SyncLock

            If capRcvFiles Then
                Dim oExt As DownloadRequestExtendPart = DownloadRequest.Parse(oRcvMsg).ExtendPart
                Dim lastIndex As Integer = oExt.TransferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(oExt.TransferListBase, oExt.TransferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", sCapTransKind)
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        If isOk Then
            DownloadResponse.Gen(DownloadResult.Finished).WriteToSocket(oParentMessageSock)
        Else
            DownloadResponse.Gen(DownloadResult.Aborted).WriteToSocket(oParentMessageSock)
        End If

        Return True
    End Function

    Protected Overrides Function ProcOnUploadRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Dim capSndFiles As Boolean
        SyncLock oForm.UiState
            capSndFiles = oForm.UiState.CapSndFiles
        End SyncLock

        If capSndFiles Then
            Dim oExt As UploadRequestExtendPart = UploadRequest.Parse(oRcvMsg).ExtendPart
            Dim lastIndex As Integer = oExt.TransferList.Count - 1
            For i As Integer = 0 To lastIndex
                Try
                    Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(oExt.TransferListBase, oExt.TransferList(i))
                    Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", sCapTransKind)
                    File.Copy(sSrcPath, sDstPath)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                End Try
            Next
        End If

        Return MyBase.ProcOnUploadRequestReceive(oRcvMsg)
    End Function
#End Region

End Class
