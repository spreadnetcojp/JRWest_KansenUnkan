' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/06/10  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class SelectStationDialog

    Private oComboItems As DataTable
    Private oTable As DataTable

    Public Sub New(ByVal oStationItemTable As DataTable)
        InitializeComponent()

        oComboItems = oStationItemTable

        oTable = New DataTable()
        oTable.Columns.Add("VALUE", GetType(String))
        Dim oRow As DataRow = oTable.NewRow()
        oRow("VALUE") = "000-000"
        oTable.Rows.Add(oRow)

        StationGridView.SuspendLayout()

        StationGridView.AutoGenerateColumns = True
        StationGridView.DataSource = oTable
        StationGridView.AutoGenerateColumns = False

        StationGridView.Columns(0).AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        StationGridView.Columns(0).FillWeight = 100.0!
        StationGridView.Columns(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        StationGridView.Columns(0).DefaultCellStyle.Font = New System.Drawing.Font("MS Gothic", 9.0!)
        StationGridView.Columns(0).ReadOnly = False
        StationGridView.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

        Dim oDummyItems As New DataTable()
        oDummyItems.Columns.Add("Key", GetType(String))
        oDummyItems.Columns.Add("Value", GetType(String))
        Dim oComboColumn As New DataGridViewComboBoxColumn()
        oComboColumn.DataPropertyName = "VALUE"
        oComboColumn.Name = "VALUE_MENU"
        oComboColumn.DataSource = oDummyItems
        oComboColumn.ValueMember = "Key"
        oComboColumn.DisplayMember = "Value"
        oComboColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        oComboColumn.FlatStyle = FlatStyle.Flat
        oComboColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        oComboColumn.FillWeight = 160.0!
        oComboColumn.SortMode = DataGridViewColumnSortMode.NotSortable
        StationGridView.Columns.Insert(1, oComboColumn)

        StationGridView.ResumeLayout()
    End Sub

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)

        StationGridView.Rows(0).Cells(0).Tag = New XlsField(8*1, "D3", 2, "-"c, "�w�R�[�h")

        Dim oCell As DataGridViewCell = StationGridView.Rows(0).Cells(1)
        Dim oCombo As DataGridViewComboBoxCell = DirectCast(oCell, DataGridViewComboBoxCell)
        oCombo.DataSource = oComboItems
        oCell.ReadOnly = False
    End Sub

    Public Property Station As String
        Get
            Return oTable.Rows(0).Field(Of String)("VALUE")
        End Get
        Set(ByVal value As String)
            oTable.Rows(0)("VALUE") = value
        End Set
    End Property

    Public Property Description As String
        Get
            Return DescriptionLabel.Text
        End Get
        Set(ByVal value As String)
            DescriptionLabel.Text = value
        End Set
    End Property

    Private Sub StationGridView_CellValueChanged(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles StationGridView.CellValueChanged
        If e.RowIndex < 0 Then Return
        Dim oView As DataRowView = DirectCast(StationGridView.Rows(e.RowIndex).DataBoundItem, DataRowView)
        oView.Row.AcceptChanges()
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
