Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class ActiveUllTransferNameForm

    Public Property SelectedValue() As String
        Get
            Return _SelectedValue
        End Get

        Private Set(ByVal val As String)
            _SelectedValue = val
        End Set
    End Property

    Private _SelectedValue As String

    Public Sub New()
        InitializeComponent()
        InitMenuGrid()
        Me.KeyPreview = True
    End Sub

    Public Shadows Function ShowDialog() As DialogResult
        'NOTE: 最終選択行等の維持を目的に、同一インスタンスを繰り返し再利用されることを想定している。
        'そのような使用方法であっても、本メソッドの戻り値を参照するまでもなく、
        'ユーザの意志が判断できるよう、キャンセルや閉じるボタンで閉じられた場合は、
        'SelectedValueプロパティがNothingになるようにしておく。
        SelectedValue = Nothing
        Return MyBase.ShowDialog()
    End Function

    Public Sub InitMenuGrid()
        MenuGrid.Rows.Clear()
        For Each oItem As String() In Config.ActiveUllTransferNameList
            Dim newRow As New DataGridViewRow()
            newRow.CreateCells(MenuGrid)
            With newRow
                .Cells(0).Value = oItem(0)
                .Cells(1).Value = oItem(1)
            End With
            MenuGrid.Rows.Add(newRow)
        Next oItem
    End Sub

    Private Sub OkayButton_Click(sender As Object, e As EventArgs) Handles OkayButton.Click
        If MenuGrid.SelectedRows.Count <> 0 Then
            SelectedValue = DirectCast(MenuGrid.SelectedRows(0).Cells(1).Value, String)
            DialogResult = DialogResult.OK
        Else
            DialogResult = DialogResult.Cancel
        End If
    End Sub

    Private Sub CancButton_Click(sender As Object, e As EventArgs) Handles CancButton.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub ActiveUllTransferNameForm_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If e.KeyData = Keys.Enter Then
            OkayButton_Click(sender, e)
        End If

        If e.KeyData = Keys.Escape Then
            CancButton_Click(sender, e)
        End If
    End Sub

    Private Sub MenuGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MenuGrid.CellDoubleClick
        SelectedValue = DirectCast(MenuGrid.Rows(e.RowIndex).Cells(1).Value, String)
        DialogResult = DialogResult.OK
    End Sub

End Class
