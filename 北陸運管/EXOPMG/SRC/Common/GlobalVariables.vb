' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  OPMGMdlCmnPublicを改名して作成
' **********************************************************************
Option Strict On
Option Explicit On

''' <summary>
''' 共有可変情報格納用モジュール
''' </summary>
Public Class GlobalVariables

#Region "定数"
    Friend Shared ReadOnly LockObject As New Object()
#End Region

#Region "変数"
    'NOTE: この参照型変数を直接読み書きする際は、
    '自前でSyncLock LockObjectした上で行うこと。
    Friend Shared SysUserId As String = "SYS"
#End Region

#Region "公開プロパティ"
    Public Shared Property UserId() As String
        Get
            Dim sRetVal As String
            SyncLock GlobalVariables.LockObject
                sRetVal = SysUserId
            End SyncLock
            Return sRetVal
        End Get

        Set(ByVal sVal As String)
            If String.IsNullOrEmpty(sVal) Then
                sVal = "SYS"
            End If
            SyncLock GlobalVariables.LockObject
                SysUserId = sVal
            End SyncLock
        End Set
    End Property
#End Region

End Class
