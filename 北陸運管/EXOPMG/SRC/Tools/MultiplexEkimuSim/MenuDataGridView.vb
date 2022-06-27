' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/01/14  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System
Imports System.Windows.Forms

Public Class MenuDataGridView
    Inherits DataGridView

    Public Sub New()
        MyBase.New()

        Me.AllowUserToAddRows = False
        Me.AllowUserToDeleteRows = False
        Me.AllowUserToResizeColumns = False
        Me.AllowUserToResizeRows = False
        Me.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.ColumnHeadersVisible = False
        Me.ImeMode = ImeMode.Disable
        Me.MultiSelect = False
        Me.ReadOnly = True
        Me.RowHeadersVisible = False
        Me.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.StandardTab = True
    End Sub

    <System.Security.Permissions.UIPermission( _
        System.Security.Permissions.SecurityAction.Demand, _
        Window:=System.Security.Permissions.UIPermissionWindow.AllWindows)> _
    Protected Overrides Function ProcessDialogKey(ByVal keyData As Keys) As Boolean
        If (keyData And Keys.KeyCode) = Keys.Enter Then
            Return False
        End If

        If (keyData And Keys.KeyCode) = Keys.Escape Then
            Return False
        End If

        Return MyBase.ProcessDialogKey(keyData)
    End Function

    <System.Security.Permissions.SecurityPermission( _
        System.Security.Permissions.SecurityAction.Demand, _
        Flags:=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)> _
    Protected Overrides Function ProcessDataGridViewKey(ByVal e As KeyEventArgs) As Boolean
        If e.KeyCode = Keys.Enter Then
            Return False
        End If

        If e.KeyCode = Keys.Escape Then
            Return False
        End If

        Return MyBase.ProcessDataGridViewKey(e)
    End Function

End Class
