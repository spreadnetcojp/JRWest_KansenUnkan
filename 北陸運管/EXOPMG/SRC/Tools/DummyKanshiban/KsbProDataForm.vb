' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/07/14  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text

Imports JR.ExOpmg.Common

Public Class KsbProDataForm

    Private FormKey As String
    Private ManagerForm As MainForm

    'スタイル
    Private CellStyleOfPlain As DataGridViewCellStyle
    Private CellStyleOfDisabled As DataGridViewCellStyle

    Public Sub New(ByVal sMachineId As String, ByVal sDataKind As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal dataAcceptDate As DateTime, ByVal sDataHashValue As String, ByVal sArchiveCatalog As String, ByVal oVersionListData As Byte(), ByVal sFormKey As String, ByVal oManagerForm As MainForm)
        InitializeComponent()

        Me.Text = "監視盤プログラム"
        Me.FormKey = sFormKey
        Me.ManagerForm = oManagerForm
        Me.MachineIdTextBox.Text = sMachineId
        Me.DataKindTextBox.Text = sDataKind
        Me.DataSubKindTextBox.Text = dataSubKind.ToString()
        Me.DataVersionTextBox.Text = dataVersion.ToString()
        If dataAcceptDate = Config.EmptyTime Then
            Me.DataAcceptDateTextBox.Text = Lexis.EmptyTime.Gen()
        ElseIf dataAcceptDate = Config.UnknownTime Then
            Me.DataAcceptDateTextBox.Text = Lexis.UnknownTime.Gen()
        Else
            Me.DataAcceptDateTextBox.Text = dataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff")
        End If
        Me.DataHashValueTextBox.Text = sDataHashValue

        CellStyleOfPlain = New DataGridViewCellStyle()

        CellStyleOfDisabled = New DataGridViewCellStyle()
        CellStyleOfDisabled.BackColor = System.Drawing.Color.LightGray

        ArchiveCatalogTextBox.Text = sArchiveCatalog

        With Nothing
            Dim oTable As New DataTable()
            oTable.Columns.Add("TITLE", GetType(String))
            oTable.Columns.Add("FORMAT", GetType(String))
            oTable.Columns.Add("VALUE", GetType(String))

            Dim bitOffset As Integer = 0
            Dim maxTitleWidth As Integer = 0
            Dim maxFormatWidth As Integer = 0
            For Each oField As XlsField In ProgramVersionListUtil.Fields
                Dim formatDesc As String = oField.CreateFormatDescription()
                Dim oRow As DataRow = oTable.NewRow()
                oRow("TITLE") = oField.MetaName
                oRow("FORMAT") = formatDesc
                oRow("VALUE") = oField.CreateValueFromBytes(oVersionListData, bitOffset)
                oTable.Rows.Add(oRow)
                Dim titleWidth As Integer = MyUtility.GetTextWidth(oField.MetaName, VersionListDataGridView.Font)
                If titleWidth > maxTitleWidth Then
                    maxTitleWidth = titleWidth
                End If
                Dim formatWidth As Integer = MyUtility.GetTextWidth(formatDesc & "...", VersionListDataGridView.Font)
                If formatWidth > maxFormatWidth Then
                    maxFormatWidth = formatWidth
                End If
                bitOffset += oField.ElementBits * oField.ElementCount
            Next oField

            VersionListDataGridView.SuspendLayout()

            VersionListDataGridView.AutoGenerateColumns = True
            VersionListDataGridView.DataSource = oTable
            VersionListDataGridView.AutoGenerateColumns = False

            VersionListDataGridView.Columns(0).HeaderText = "項目名"
            VersionListDataGridView.Columns(0).Width = maxTitleWidth
            VersionListDataGridView.Columns(0).ReadOnly = True
            VersionListDataGridView.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

            VersionListDataGridView.Columns(1).HeaderText = "書式"
            VersionListDataGridView.Columns(1).Width = maxFormatWidth
            VersionListDataGridView.Columns(1).ReadOnly = True
            VersionListDataGridView.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
            VersionListDataGridView.Columns(1).Visible = False

            VersionListDataGridView.Columns(2).HeaderText = "値"
            VersionListDataGridView.Columns(2).AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            VersionListDataGridView.Columns(2).FillWeight = 100.0!
            VersionListDataGridView.Columns(2).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            VersionListDataGridView.Columns(2).DefaultCellStyle.Font = New System.Drawing.Font("MS Gothic", 9.0!)
            VersionListDataGridView.Columns(2).ReadOnly = False
            VersionListDataGridView.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable

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
            oComboColumn.HeaderText = "値の意味"
            oComboColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            oComboColumn.FillWeight = 50.0!
            oComboColumn.SortMode = DataGridViewColumnSortMode.NotSortable
            VersionListDataGridView.Columns.Insert(3, oComboColumn)

            VersionListDataGridView.ResumeLayout()
        End With
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        ManagerForm.MasProDataFormDic.Remove(FormKey)
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        'NOTE: OnShownではなく、このタイミングでこれを実施するのは、非効率であるが、理由がある。
        'OnShownの時点では、VersionListDataGridViewの行の共有が解除されないためである。
        '行の共有が解除された後でなければ、DataGridViewComboBoxCellのDataSourceやTagに何かを
        'セットしても無意味である（非共有のインスタンスが生成された段階でNothingに戻ってしまう）。
        'TODO: 行の共有が解除されるタイミングを調べて、この処理をそのタイミングに合わせて実装するのは非常に汚い。
        'そのような制約のない仕組みを考えたい。
        'まっとうなのは、DataGridViewRow派生のXlsDataGridViewRowなどを定義し、共有を発生させ
        'ない（解除のタイミングをコントロールする）方法であるが、実装量が多そうである。
        'XlsFieldを、値列のTagにではなく、専用列（値の左隣の列）のValueに保持させることして、
        '表示する場合は書式としてFormatDescriptionを表示するのはよい考えであるが、
        'DataSourceについて同じような（一貫した）解決が困難である。
        'VersionListDataGridView.RowUnsharedのタイミングでセットしたとしても、
        'CellFormattingの時点でセットされていないため、OnDataErrorが発生してしまうはず。
        If TabControl1.TabPages(TabControl1.SelectedIndex).Name.Equals("VersionListViewPage") Then
            For i As Integer = 0 To ProgramVersionListUtil.Fields.Length - 1
                Dim oField As XlsField = ProgramVersionListUtil.Fields(i)
                Dim oRow As DataGridViewRow = VersionListDataGridView.Rows(i)
                Dim oCell As DataGridViewCell = oRow.Cells(3)
                Dim oCombo As DataGridViewComboBoxCell = DirectCast(oCell, DataGridViewComboBoxCell)
                If oField.MetaType IsNot Nothing Then
                    oCombo.Style = CellStyleOfPlain
                    Select Case oField.MetaType
                        Case "CompanyCode"
                            oCombo.DataSource = Config.CompanyCodeItems
                        Case "IcArea"
                            oCombo.DataSource = Config.IcAreaItems
                        Case "ProgramDistribution"
                            oCombo.DataSource = Config.ProgramDistributionItems
                    End Select
                    oCombo.ReadOnly = False
                Else
                    oCombo.Style = CellStyleOfDisabled
                    oCombo.DataSource = Nothing
                    oCombo.ReadOnly = True
                End If
                VersionListDataGridView.Rows(i).Cells(2).Tag = oField
            Next i
        End If
    End Sub

End Class
