' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/07/14  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text

Imports JR.ExOpmg.Common

Public Class KsbProDataForm

    Private FormKey As String
    Private ManagerForm As MainForm

    '�X�^�C��
    Private CellStyleOfPlain As DataGridViewCellStyle
    Private CellStyleOfDisabled As DataGridViewCellStyle

    Public Sub New(ByVal sMachineId As String, ByVal sDataKind As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal dataAcceptDate As DateTime, ByVal sDataHashValue As String, ByVal sArchiveCatalog As String, ByVal oVersionListData As Byte(), ByVal sFormKey As String, ByVal oManagerForm As MainForm)
        InitializeComponent()

        Me.Text = "�Ď��Ճv���O����"
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

            VersionListDataGridView.Columns(0).HeaderText = "���ږ�"
            VersionListDataGridView.Columns(0).Width = maxTitleWidth
            VersionListDataGridView.Columns(0).ReadOnly = True
            VersionListDataGridView.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

            VersionListDataGridView.Columns(1).HeaderText = "����"
            VersionListDataGridView.Columns(1).Width = maxFormatWidth
            VersionListDataGridView.Columns(1).ReadOnly = True
            VersionListDataGridView.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
            VersionListDataGridView.Columns(1).Visible = False

            VersionListDataGridView.Columns(2).HeaderText = "�l"
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
            oComboColumn.HeaderText = "�l�̈Ӗ�"
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
        'NOTE: OnShown�ł͂Ȃ��A���̃^�C�~���O�ł�������{����̂́A������ł��邪�A���R������B
        'OnShown�̎��_�ł́AVersionListDataGridView�̍s�̋��L����������Ȃ����߂ł���B
        '�s�̋��L���������ꂽ��łȂ���΁ADataGridViewComboBoxCell��DataSource��Tag�ɉ�����
        '�Z�b�g���Ă����Ӗ��ł���i�񋤗L�̃C���X�^���X���������ꂽ�i�K��Nothing�ɖ߂��Ă��܂��j�B
        'TODO: �s�̋��L�����������^�C�~���O�𒲂ׂāA���̏��������̃^�C�~���O�ɍ��킹�Ď�������͔̂��ɉ����B
        '���̂悤�Ȑ���̂Ȃ��d�g�݂��l�������B
        '�܂��Ƃ��Ȃ̂́ADataGridViewRow�h����XlsDataGridViewRow�Ȃǂ��`���A���L�𔭐�����
        '�Ȃ��i�����̃^�C�~���O���R���g���[������j���@�ł��邪�A�����ʂ��������ł���B
        'XlsField���A�l���Tag�ɂł͂Ȃ��A��p��i�l�̍��ׂ̗�j��Value�ɕێ������邱�Ƃ��āA
        '�\������ꍇ�͏����Ƃ���FormatDescription��\������̂͂悢�l���ł��邪�A
        'DataSource�ɂ��ē����悤�ȁi��т����j����������ł���B
        'VersionListDataGridView.RowUnshared�̃^�C�~���O�ŃZ�b�g�����Ƃ��Ă��A
        'CellFormatting�̎��_�ŃZ�b�g����Ă��Ȃ����߁AOnDataError���������Ă��܂��͂��B
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
