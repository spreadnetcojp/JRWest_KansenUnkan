' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/09/27  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text
Imports System.Windows.Forms

Imports JR.ExOpmg.Common

Public Class LogDispFilterEditDialog

    'NOTE: oSourceTableなどは、このFormに配置したDataGridViewのDataSourceに
    'セットするが、だからといって、このFormのDispose（= DataGridViewのDispose）
    'でDisposeされるわけではない（DataGridViewのソースコードから確認済み）。
    'ただし、現状の用途では、これらにIDisposableなオブジェクト（Componentなど）
    'を持たせるようなことはしていないため、これらのDispose呼び出しは省略する。

    Private oSourceTable As DataTable
    Private SourceSelectorGridBackColor As Color
    Private SourceSelectorGridForeColor As Color
    Private SourceSelectorGridSelectionBackColor As Color
    Private SourceSelectorGridSelectionForeColor As Color

    Private oKindTable As DataTable
    Private KindSelectorGridBackColor As Color
    Private KindSelectorGridForeColor As Color
    Private KindSelectorGridSelectionBackColor As Color
    Private KindSelectorGridSelectionForeColor As Color

    Private oFilterValueStorage As List(Of String)
    Private oLogStorage As DataTable
    Private isShownOnce As Boolean = False

    Public Sub New(ByVal oFilterValueStorage As List(Of String), ByVal oLogStorage As DataTable)
        InitializeComponent()

        Filter.DataSource = oFilterValueStorage
        Me.oFilterValueStorage = oFilterValueStorage
        Me.oLogStorage = oLogStorage
        oSourceTable = CreateSourceTable()
        oKindTable = CreateKindTable()
    End Sub

    Public Shadows Function ShowDialog(ByVal onError As Boolean) As DialogResult
        If Not onError Then
            Filter.DataSource = Nothing
            Filter.DataSource = oFilterValueStorage
            If oFilterValueStorage.Count <> 0 Then
                Filter.SelectedIndex = 0
            End If
        End If

        Return MyBase.ShowDialog()
    End Function

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)

        If isShownOnce Then Return

        SourceSelectorGrid.SuspendLayout()
        SourceSelectorGrid.AutoGenerateColumns = True
        SourceSelectorGrid.DataSource = oSourceTable
        SourceSelectorGrid.AutoGenerateColumns = False
        SourceSelectorGrid.AllowUserToAddRows = False
        SourceSelectorGrid.AllowUserToDeleteRows = False
        SourceSelectorGrid.Columns(0).ReadOnly = False
        SourceSelectorGrid.Columns(1).ReadOnly = True
        SourceSelectorGrid.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        SourceSelectorGrid.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        SourceSelectorGrid.AutoResizeColumn(0, DataGridViewAutoSizeColumnMode.AllCells)
        SourceSelectorGrid.ContextMenuStrip = GridMenuStrip
        SourceSelectorGrid.ResumeLayout()

        SourceSelectorGridBackColor = SourceSelectorGrid.DefaultCellStyle.BackColor
        SourceSelectorGridForeColor = SourceSelectorGrid.DefaultCellStyle.ForeColor
        SourceSelectorGridSelectionBackColor = SourceSelectorGrid.DefaultCellStyle.SelectionBackColor
        SourceSelectorGridSelectionForeColor = SourceSelectorGrid.DefaultCellStyle.SelectionForeColor

        KindSelectorGrid.SuspendLayout()
        KindSelectorGrid.AutoGenerateColumns = True
        KindSelectorGrid.DataSource = oKindTable
        KindSelectorGrid.AutoGenerateColumns = False
        KindSelectorGrid.AllowUserToAddRows = False
        KindSelectorGrid.AllowUserToDeleteRows = False
        KindSelectorGrid.Columns(0).ReadOnly = False
        KindSelectorGrid.Columns(1).ReadOnly = True
        KindSelectorGrid.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        KindSelectorGrid.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        KindSelectorGrid.AutoResizeColumn(0, DataGridViewAutoSizeColumnMode.AllCells)
        KindSelectorGrid.ContextMenuStrip = GridMenuStrip
        KindSelectorGrid.ResumeLayout()

        KindSelectorGridBackColor = KindSelectorGrid.DefaultCellStyle.BackColor
        KindSelectorGridForeColor = KindSelectorGrid.DefaultCellStyle.ForeColor
        KindSelectorGridSelectionBackColor = KindSelectorGrid.DefaultCellStyle.SelectionBackColor
        KindSelectorGridSelectionForeColor = KindSelectorGrid.DefaultCellStyle.SelectionForeColor

        EasyEditButton.Checked = True
        'EasyEditButton.Checked = False
        DirectEditButton.Checked = True

        isShownOnce = True
    End Sub

    Public ReadOnly Property FilterValue As String
        Get
            If DirectEditButton.Checked Then
                Return Filter.Text
            Else
                Dim oCond As New StringBuilder()
                AppendCondition(oSourceTable, "Source", oCond)
                AppendCondition(oKindTable, "Kind", oCond)
                Return oCond.ToString()
            End If
        End Get
    End Property

    Private Function CreateSourceTable As DataTable
        Dim oTempTable As New DataTable()
        oTempTable.Columns.Add("Checked", GetType(Boolean))
        oTempTable.Columns.Add("Key", GetType(String))

        Dim oTempHash As New HashSet(Of String)()
        For Each oRow As DataRow In oLogStorage.Rows
            oTempHash.Add(oRow.Field(Of String)("Source"))
        Next oRow
        For Each s As String In oTempHash
            Dim oRow As DataRow = oTempTable.NewRow()
            oRow("Checked") = True
            oRow("Key") = s
            oTempTable.Rows.Add(oRow)
        Next s

        Dim oTable As DataTable = oTempTable.Clone()
        Try
            Dim dv As DataView = New DataView(oTempTable)
            dv.Sort = "Key ASC"
            For Each drv As DataRowView In dv
                oTable.ImportRow(drv.Row)
            Next drv
        Catch ex As Exception
            Log.Fatal("Exception caught.", ex)
            oTable = New DataTable()
        End Try

        Return oTable
    End Function

    Private Function CreateKindTable As DataTable
        Dim oTable As New DataTable()
        oTable.Columns.Add("Checked", GetType(Boolean))
        oTable.Columns.Add("Key", GetType(String))

        Dim oTempList As New List(Of String)()
        oTempList.Add("[DEBUG]")
        oTempList.Add("[INFO]")
        oTempList.Add("[WARN]")
        oTempList.Add("[ERROR]")
        oTempList.Add("[FATAL]")
        For Each oRow As DataRow In oLogStorage.Rows
            Dim s As String = oRow.Field(Of String)("Kind")
            If Not oTempList.Contains(s) Then
                oTempList.Add(s)
            End If
        Next oRow
        For Each s As String In oTempList
            Dim oRow As DataRow = oTable.NewRow()
            oRow("Checked") = True
            oRow("Key") = s
            oTable.Rows.Add(oRow)
        Next s

        Return oTable
    End Function

    Private Sub AppendCondition(ByVal oTable As DataTable, ByVal sFieldName As String, ByVal oCond As StringBuilder)
        Dim nChecked As Integer = 0
        For Each oRow As DataRow In oTable.Rows
            If oRow.Field(Of Boolean)("Checked") Then
                nChecked += 1
            End If
        Next oRow

        If nChecked * 2 < oTable.Rows.Count Then
            If oCond.Length <> 0 Then
                oCond.Append(" And ")
            End If
            If nChecked <> 0 Then
                Dim oTargets As New StringBuilder()
                For Each oRow As DataRow In oTable.Rows
                    If oRow.Field(Of Boolean)("Checked") Then
                        If oTargets.Length <> 0 Then
                            oTargets.Append(", ")
                        End If
                        oTargets.Append("'" & oRow.Field(Of String)("Key") & "'")
                    End If
                Next oRow
                oCond.Append(sFieldName & " in (" & oTargets.ToString() & ")")
            Else
                oCond.Append(sFieldName & " = NULL")
            End If
        Else
            If nChecked <> oTable.Rows.Count Then
                Dim oTargets As New StringBuilder()
                For Each oRow As DataRow In oTable.Rows
                    If Not oRow.Field(Of Boolean)("Checked") Then
                        If oTargets.Length <> 0 Then
                            oTargets.Append(", ")
                        End If
                        oTargets.Append("'" & oRow.Field(Of String)("Key") & "'")
                    End If
                Next oRow
                If oCond.Length <> 0 Then
                    oCond.Append(" And ")
                End If
                oCond.Append(sFieldName & " not in (" & oTargets.ToString() & ")")
            End If
        End If
    End Sub

    Private Sub DirectEditButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DirectEditButton.CheckedChanged
        If DirectEditButton.Checked Then
            Filter.Enabled = True
        Else
            Filter.Enabled = False
        End If
    End Sub

    Private Sub EasyEditButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EasyEditButton.CheckedChanged
        If EasyEditButton.Checked Then
            EasyEditPanel.Enabled = True

            SourceSelectorGrid.DefaultCellStyle.BackColor = SourceSelectorGridBackColor
            SourceSelectorGrid.DefaultCellStyle.ForeColor = SourceSelectorGridForeColor
            SourceSelectorGrid.DefaultCellStyle.SelectionBackColor = SourceSelectorGridSelectionBackColor
            SourceSelectorGrid.DefaultCellStyle.SelectionForeColor = SourceSelectorGridSelectionForeColor

            KindSelectorGrid.DefaultCellStyle.BackColor = KindSelectorGridBackColor
            KindSelectorGrid.DefaultCellStyle.ForeColor = KindSelectorGridForeColor
            KindSelectorGrid.DefaultCellStyle.SelectionBackColor = KindSelectorGridSelectionBackColor
            KindSelectorGrid.DefaultCellStyle.SelectionForeColor = KindSelectorGridSelectionForeColor
        Else
            EasyEditPanel.Enabled = False

            SourceSelectorGrid.DefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke
            SourceSelectorGrid.DefaultCellStyle.ForeColor = System.Drawing.Color.DimGray
            SourceSelectorGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Gray
            SourceSelectorGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White

            KindSelectorGrid.DefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke
            KindSelectorGrid.DefaultCellStyle.ForeColor = System.Drawing.Color.DimGray
            KindSelectorGrid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Gray
            KindSelectorGrid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White
        End If
    End Sub

    Private Sub EasyEditRefreshButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EasyEditRefreshButton.Click
        oSourceTable = CreateSourceTable()
        SourceSelectorGrid.DataSource = oSourceTable

        oKindTable = CreateKindTable()
        KindSelectorGrid.DataSource = oKindTable
    End Sub

    Private Sub SelectorGrid_CellDoubleClick(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles SourceSelectorGrid.CellDoubleClick, KindSelectorGrid.CellDoubleClick
        If e.RowIndex < 0 Then Return
        Dim oGrid As DataGridView = DirectCast(sender, DataGridView)
        Dim oView As DataRowView = DirectCast(oGrid.Rows(e.RowIndex).DataBoundItem, DataRowView)
        Dim oRow As DataRow = oView.Row
        oRow("Checked") = Not oRow.Field(Of Boolean)("Checked")
    End Sub

    Private Sub SelectorGrid_CellContextMenuStripNeeded(ByVal sender As System.Object, ByVal e As DataGridViewCellContextMenuStripNeededEventArgs) Handles SourceSelectorGrid.CellContextMenuStripNeeded, KindSelectorGrid.CellContextMenuStripNeeded
        If e.RowIndex < 0 Then Return
        Dim oGrid As DataGridView = DirectCast(sender, DataGridView)
        Dim oCell As DataGridViewCell = oGrid(e.ColumnIndex, e.RowIndex)
        If Not oCell.Selected Then
            If (Control.ModifierKeys And Keys.Control) = Keys.Control Then
                oCell.Selected = True
            Else
                oGrid.CurrentCell = oCell
            End If
        End If
    End Sub

    Private Sub ToolStripMenuItemOfSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemOfSelect.Click
        Dim oGrid As DataGridView = TryCast(GridMenuStrip.SourceControl, DataGridView)
        If oGrid IsNot Nothing Then
            For Each gridRow As DataGridViewRow In oGrid.Rows
                If gridRow.Selected Then
                    Dim oView As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                    Dim oRow As DataRow = oView.Row
                    oRow("Checked") = True
                End If
            Next gridRow
        End If
    End Sub

    Private Sub ToolStripMenuItemOfDeselect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemOfDeselect.Click
        Dim oGrid As DataGridView = TryCast(GridMenuStrip.SourceControl, DataGridView)
        If oGrid IsNot Nothing Then
            For Each gridRow As DataGridViewRow In oGrid.Rows
                If gridRow.Selected Then
                    Dim oView As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                    Dim oRow As DataRow = oView.Row
                    oRow("Checked") = False
                End If
            Next gridRow
        End If
    End Sub

    Private Sub SelectorGrid_CellValueChanged(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles SourceSelectorGrid.CellValueChanged, KindSelectorGrid.CellValueChanged
        If e.RowIndex < 0 Then Return
        Dim oGrid As DataGridView = DirectCast(sender, DataGridView)
        Dim oView As DataRowView = DirectCast(oGrid.Rows(e.RowIndex).DataBoundItem, DataRowView)
        oView.Row.AcceptChanges()
    End Sub

    Private Sub AllSourcesSelectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllSourcesSelectButton.Click
        For Each oRow As DataRow In oSourceTable.Rows
            oRow("Checked") = True
        Next oRow
    End Sub

    Private Sub AllSourcesDeselectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllSourcesDeselectButton.Click
        For Each oRow As DataRow In oSourceTable.Rows
            oRow("Checked") = False
        Next oRow
    End Sub

    Private Sub AllKindsSelectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllKindsSelectButton.Click
        For Each oRow As DataRow In oKindTable.Rows
            oRow("Checked") = True
        Next oRow
    End Sub

    Private Sub AllKindsDeselectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllKindsDeselectButton.Click
        For Each oRow As DataRow In oKindTable.Rows
            oRow("Checked") = False
        Next oRow
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

End Class
