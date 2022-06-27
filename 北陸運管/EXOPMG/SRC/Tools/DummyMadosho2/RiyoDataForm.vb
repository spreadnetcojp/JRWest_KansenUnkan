' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/06/27  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class RiyoDataForm

    Private MonitorMachineId As String
    Private TermMachineId As String
    Private ManagerForm As MainForm
    Private RecordLength As Integer
    Private oTable As DataTable
    Private LastReadFilePath As String
    Private LastReadRecordIndex As Long
    Private LastWrittenFilePath As String
    Private LastWrittenRecordIndex As Long

    '�X�^�C��
    Private CellStyleOfPlain As DataGridViewCellStyle
    Private CellStyleOfDisabled As DataGridViewCellStyle
    Private CellStyleOfHighlighted As DataGridViewCellStyle

    Public Sub New(ByVal sMonitorMachineId As String, ByVal sTermMachineId As String, ByVal oManagerForm As MainForm)
        InitializeComponent()

        Me.MonitorMachineId = sMonitorMachineId
        Me.TermMachineId = sTermMachineId
        Me.ManagerForm = oManagerForm

        Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).Profile
        Me.Text = Lexis.RiyoDataFormTitle.Gen(oProf(Config.MachineProfileFieldNamesIndices("MODEL_NAME")), _
                                              oProf(Config.MachineProfileFieldNamesIndices("STATION_NAME")), _
                                              oProf(Config.MachineProfileFieldNamesIndices("CORNER_NAME")), _
                                              oProf(Config.MachineProfileFieldNamesIndices("UNIT_NO")))
        Me.MonitorMachineIdTextBox.Text = sMonitorMachineId
        Me.TermMachineIdTextBox.Text = sTermMachineId

        oTable = New DataTable()
        oTable.Columns.Add("TITLE", GetType(String))
        oTable.Columns.Add("FORMAT", GetType(String))
        oTable.Columns.Add("VALUE", GetType(String))
        Dim maxTitleWidth As Integer = 0
        Dim maxFormatWidth As Integer = 0
        For Each oField As XlsField In RiyoDataUtil.Fields
            Dim formatDesc As String = oField.CreateFormatDescription()
            Dim oRow As DataRow = oTable.NewRow()
            oRow("TITLE") = oField.MetaName
            oRow("FORMAT") = formatDesc
            oRow("VALUE") = CreateInitialValue(oField)
            oTable.Rows.Add(oRow)
            Dim titleWidth As Integer = MyUtility.GetTextWidth(oField.MetaName, RiyoDataGridView.Font)
            If titleWidth > maxTitleWidth Then
                maxTitleWidth = titleWidth
            End If
            Dim formatWidth As Integer = MyUtility.GetTextWidth(formatDesc & "...", RiyoDataGridView.Font)
            If formatWidth > maxFormatWidth Then
                maxFormatWidth = formatWidth
            End If
        Next oField
        RecordLength = RiyoDataUtil.RecordLengthInBytes

        RiyoDataGridView.SuspendLayout()

        RiyoDataGridView.AutoGenerateColumns = True
        RiyoDataGridView.DataSource = oTable
        RiyoDataGridView.AutoGenerateColumns = False

        RiyoDataGridView.Columns(0).HeaderText = "���ږ�"
        RiyoDataGridView.Columns(0).Width = maxTitleWidth
        RiyoDataGridView.Columns(0).ReadOnly = True
        RiyoDataGridView.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

        RiyoDataGridView.Columns(1).HeaderText = "����"
        RiyoDataGridView.Columns(1).Width = maxFormatWidth
        RiyoDataGridView.Columns(1).ReadOnly = True
        RiyoDataGridView.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable

        RiyoDataGridView.Columns(2).HeaderText = "�l"
        RiyoDataGridView.Columns(2).AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        RiyoDataGridView.Columns(2).FillWeight = 100.0!
        RiyoDataGridView.Columns(2).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        RiyoDataGridView.Columns(2).DefaultCellStyle.Font = New System.Drawing.Font("MS Gothic", 9.0!)
        RiyoDataGridView.Columns(2).ReadOnly = False
        RiyoDataGridView.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable

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
        oComboColumn.FillWeight = 160.0!
        oComboColumn.SortMode = DataGridViewColumnSortMode.NotSortable
        RiyoDataGridView.Columns.Insert(3, oComboColumn)

        RiyoDataGridView.ResumeLayout()

        LastReadFilePath = ""
        LastReadRecordIndex = 0
        LastWrittenFilePath = ""
        LastWrittenRecordIndex = 0
    End Sub

    Private Function CreateInitialValue(ByVal oField As XlsField) As String
        Select Case oField.MetaName
            Case "��{�w�b�_�[ �f�[�^���"
                Return "A0"
            Case "��{�w�b�_�[ �w�R�[�h"
                Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).Profile
                Return CStr(oProf(Config.MachineProfileFieldNamesIndices("RAIL_SECTION_CODE"))) & "-" & CStr(oProf(Config.MachineProfileFieldNamesIndices("STATION_ORDER_CODE")))
            Case "��{�w�b�_�[ ��������"
                Return DateTime.Now.ToString("yyyyMMddHHmmss")
            Case "��{�w�b�_�[ �R�[�i�["
                Return CStr(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).Profile(Config.MachineProfileFieldNamesIndices("CORNER_CODE")))
            Case "��{�w�b�_�[ ���@"
                Return CStr(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).Profile(Config.MachineProfileFieldNamesIndices("UNIT_NO")))
            Case "��{�w�b�_�[ �V�[�P���XNo"
                Return CStr(MyUtility.GetNextSeqNumber(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).SeqNumber))
            Case "��{�w�b�_�[ �o�[�W����"
                Return "02"  'TODO: ����ł悢�̂��m�F�B
            Case "�ʉߕ���"
                Return If(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).LatchConf >= &H3, "01", "02")
            Case "���b�`�`��"
                Return ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).LatchConf.ToString("X2")
            Case Else
                Return oField.CreateDefaultValue()
        End Select
    End Function

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)

        CellStyleOfPlain = New DataGridViewCellStyle()

        CellStyleOfDisabled = New DataGridViewCellStyle()
        CellStyleOfDisabled.BackColor = System.Drawing.Color.LightGray

        CellStyleOfHighlighted = New DataGridViewCellStyle()
        CellStyleOfHighlighted.BackColor = System.Drawing.Color.Yellow

        For i As Integer = 0 To RiyoDataUtil.Fields.Length - 1
            RiyoDataGridView.Rows(i).Cells(2).Tag = RiyoDataUtil.Fields(i)

            Dim oCell As DataGridViewCell = RiyoDataGridView.Rows(i).Cells(3)
            Dim oCombo As DataGridViewComboBoxCell = DirectCast(oCell, DataGridViewComboBoxCell)
            Select Case RiyoDataUtil.Fields(i).MetaType
                Case "Station"
                    oCombo.DataSource = Config.StationItems
                    oCell.ReadOnly = False
                Case "PassDirection"
                    oCombo.DataSource = Config.PassDirectionItems
                    oCell.ReadOnly = False
                Case "LatchConf"
                    oCombo.DataSource = Config.LatchConfItems
                    oCell.ReadOnly = False
                Case "AdultChild"
                    oCombo.DataSource = Config.AdultChildItems
                    oCell.ReadOnly = False
                Case "MaleFemale"
                    oCombo.DataSource = Config.MaleFemaleItems
                    oCell.ReadOnly = False
                Case "IcUseUnuse"
                    oCombo.DataSource = Config.IcUseUnuseItems
                    oCell.ReadOnly = False
                Case "AdultChildFlag"
                    oCombo.DataSource = Config.AdultChildFlagItems
                    oCell.ReadOnly = False
                Case "MaleFemaleFlag"
                    oCombo.DataSource = Config.MaleFemaleFlagItems
                    oCell.ReadOnly = False
                Case "CommutingFlag"
                    oCombo.DataSource = Config.CommutingFlagItems
                    oCell.ReadOnly = False
                Case "CombinedDiscountFlag"
                    oCombo.DataSource = Config.CombinedDiscountFlagItems
                    oCell.ReadOnly = False
                Case "DiscountFlag"
                    oCombo.DataSource = Config.DiscountFlagItems
                    oCell.ReadOnly = False
                Case "ReissueFlag"
                    oCombo.DataSource = Config.ReissueFlagItems
                    oCell.ReadOnly = False
                Case "TestFlag"
                    oCombo.DataSource = Config.TestFlagItems
                    oCell.ReadOnly = False
                Case "FreightRateAmendFlag"
                    oCombo.DataSource = Config.FreightRateAmendFlagItems
                    oCell.ReadOnly = False
                Case "ConnectionFlag"
                    oCombo.DataSource = Config.ConnectionFlagItems
                    oCell.ReadOnly = False
                Case "ContinuumFlag"
                    oCombo.DataSource = Config.ContinuumFlagItems
                    oCell.ReadOnly = False
                Case "TicketValidityFlag"
                    oCombo.DataSource = Config.TicketValidityFlagItems
                    oCell.ReadOnly = False
                Case "WithdrawFlag"
                    oCombo.DataSource = Config.WithdrawFlagItems
                    oCell.ReadOnly = False
                Case "CombineFlag"
                    oCombo.DataSource = Config.CombineFlagItems
                    oCell.ReadOnly = False
                Case "SeatKind"
                    oCombo.DataSource = Config.SeatKindItems
                    oCell.ReadOnly = False
                Case "TicketKind"
                    oCombo.DataSource = Config.TicketKindItems
                    oCell.ReadOnly = False
                Case "DiscountKind"
                    oCombo.DataSource = Config.DiscountKindItems
                    oCell.ReadOnly = False
                Case "AbsencePresence"
                    oCombo.DataSource = Config.AbsencePresenceItems
                    oCell.ReadOnly = False
                Case Else
                    oCell.Style = CellStyleOfDisabled
                    oCell.ReadOnly = True
            End Select
        Next i

        'RiyoDataGridView.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders)

        For i As Integer = 0 To Config.SearchBoxInitialHis.Rows.Count - 1
            SearchBox.Items.Insert(i, Config.SearchBoxInitialHis.Rows(i).Field(Of String)("Value"))
        Next i

        RepaintRiyoDataGridView(SearchBox.Text)
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        ManagerForm.RiyoDataFormDic.Remove(MonitorMachineId & TermMachineId)
        MyBase.OnFormClosed(e)
    End Sub

    <System.Security.Permissions.UIPermission( _
        System.Security.Permissions.SecurityAction.Demand, _
        Window:=System.Security.Permissions.UIPermissionWindow.AllWindows)> _
    Protected Overrides Function ProcessDialogKey(ByVal keyData As Keys) As Boolean
        If (keyData And Keys.KeyCode) = Keys.F3 Then
            If (keyData And Keys.Shift) <> Keys.None Then
                SearchPrevButton.PerformClick()
            Else
                SearchNextButton.PerformClick()
            End If
            Return True
        ElseIf (keyData And Keys.Control) <> Keys.None Then
            If (keyData And Keys.KeyCode) = Keys.F Then
                SearchBox.Select()
                Return True
            End If
        End If

        Return MyBase.ProcessDialogKey(keyData)
    End Function

    'OPT: oTable���t�@�C���ɏ����o���Ƃ��ɂ܂Ƃ߂čs�������悢��������Ȃ��B
    Private Sub RiyoDataGridView_CellValueChanged(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles RiyoDataGridView.CellValueChanged
        If e.RowIndex < 0 Then Return
        Dim oView As DataRowView = DirectCast(RiyoDataGridView.Rows(e.RowIndex).DataBoundItem, DataRowView)
        oView.Row.AcceptChanges()
    End Sub

    Private Sub SearchBox_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles SearchBox.KeyDown
        If (e.KeyCode = Keys.Enter) AndAlso Not e.Alt AndAlso Not e.Control Then
            UpdateSearchHistory(SearchBox.Text)
            RepaintRiyoDataGridView(SearchBox.Text)
            If (e.Modifiers And Keys.Shift) <> Keys.None Then
                If SearchPrevButton.Enabled Then
                    SearchPrevButton.Select()
                    SearchPrevButton.PerformClick()
                End If
            Else
                If SearchNextButton.Enabled Then
                    SearchNextButton.Select()
                    SearchNextButton.PerformClick()
                End If
            End If

            'e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub SearchBox_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchBox.Validated
        UpdateSearchHistory(SearchBox.Text)
        RepaintRiyoDataGridView(SearchBox.Text)
    End Sub

    'Private Sub SearchBox_DropDownClosed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchBox.DropDownClosed
    '    UpdateSearchHistory(SearchBox.SelectedItem.ToString())
    '    RepaintRiyoDataGridView(SearchBox.Text)
    'End Sub

    'Private Sub SearchBox_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchBox.SelectionChangeCommitted
    'End Sub

    Private Sub UpdateSearchHistory(ByVal sArraivalWord As String)
        SearchBox.Items.Remove(sArraivalWord)
        SearchBox.Items.Insert(0, sArraivalWord)
        While SearchBox.Items.Count > Config.SearchBoxMaxHisCount
            SearchBox.Items.RemoveAt(SearchBox.Items.Count - 1)
        End While
        SearchBox.Text = sArraivalWord
    End Sub

    Private Sub RepaintRiyoDataGridView(ByVal sBuzzWord As String)
        Dim oRows As DataGridViewRowCollection = RiyoDataGridView.Rows
        For Each oRow As DataGridViewRow In oRows
            For col As Integer = 0 To 3
                Dim oCell As DataGridViewCell = oRow.Cells(col)
                If sBuzzWord.Length <> 0 AndAlso DirectCast(oCell.FormattedValue, String).Contains(sBuzzWord) Then
                    oCell.Style = CellStyleOfHighlighted
                Else
                    oCell.Style = If(col = 3 AndAlso oCell.ReadOnly, CellStyleOfDisabled, CellStyleOfPlain)
                End If
            Next col
        Next oRow

        If sBuzzWord.Length = 0 Then
            SearchPrevButton.Enabled = False
            SearchNextButton.Enabled = False
        Else
            SearchPrevButton.Enabled = True
            SearchNextButton.Enabled = True
        End If
    End Sub

    Private Sub SearchPrevButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchPrevButton.Click
        Dim rowCount As Integer = RiyoDataGridView.Rows.Count
        Dim colCount As Integer = RiyoDataGridView.Columns.Count

        Dim startRow As Integer
        Dim startCol As Integer
        If RiyoDataGridView.CurrentCell IsNot Nothing Then
            startRow = RiyoDataGridView.CurrentCell.RowIndex
            startCol = RiyoDataGridView.CurrentCell.ColumnIndex
        Else
            startRow = 0
            startCol = 0
        End If

        Dim sBuzzWord As String = SearchBox.Text
        Dim oRows As DataGridViewRowCollection = RiyoDataGridView.Rows
        Dim row As Integer = startRow
        Dim col As Integer = startCol
        Do
            col -= 1
            If col < 0 Then
                col = colCount - 1
                row -= 1
                If row < 0 Then
                    row = rowCount - 1
                End If
            End If

            Dim oCell As DataGridViewCell = oRows(row).Cells(col)
            Dim sCellValue As String = DirectCast(oCell.FormattedValue, String)
            Dim i As Integer = sCellValue.IndexOf(sBuzzWord)

            If i <> -1 Then
                RiyoDataGridView.CurrentCell = oCell
                If Not oCell.ReadOnly AndAlso TypeOf oCell Is DataGridViewTextBoxCell Then
                    RiyoDataGridView.BeginEdit(True)
                    Dim oTextBox As DataGridViewTextBoxEditingControl = _
                       DirectCast(RiyoDataGridView.EditingControl, DataGridViewTextBoxEditingControl)
                    oTextBox.SelectionStart = i
                    oTextBox.SelectionLength = sBuzzWord.Length
                End If
                Exit Do
            End If

            If row = startRow AndAlso col = startCol Then
                AlertBox.Show(Lexis.SearchWordNotFound)
                Exit Do
            End If
        Loop
    End Sub

    Private Sub SearchNextButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchNextButton.Click
        Dim rowCount As Integer = RiyoDataGridView.Rows.Count
        Dim colCount As Integer = RiyoDataGridView.Columns.Count

        Dim startRow As Integer
        Dim startCol As Integer
        If RiyoDataGridView.CurrentCell IsNot Nothing Then
            startRow = RiyoDataGridView.CurrentCell.RowIndex
            startCol = RiyoDataGridView.CurrentCell.ColumnIndex
        Else
            startRow = rowCount - 1
            startCol = colCount - 1
        End If

        Dim sBuzzWord As String = SearchBox.Text
        Dim oRows As DataGridViewRowCollection = RiyoDataGridView.Rows
        Dim row As Integer = startRow
        Dim col As Integer = startCol
        Do
            col += 1
            If col >= colCount Then
                col = 0
                row += 1
                If row >= rowCount Then
                    row = 0
                End If
            End If

            Dim oCell As DataGridViewCell = oRows(row).Cells(col)
            Dim sCellValue As String = DirectCast(oCell.FormattedValue, String)
            Dim i As Integer = sCellValue.IndexOf(sBuzzWord)

            If i <> -1 Then
                RiyoDataGridView.CurrentCell = oCell
                If Not oCell.ReadOnly AndAlso TypeOf oCell Is DataGridViewTextBoxCell Then
                    RiyoDataGridView.BeginEdit(True)
                    Dim oTextBox As DataGridViewTextBoxEditingControl = _
                       DirectCast(RiyoDataGridView.EditingControl, DataGridViewTextBoxEditingControl)
                    oTextBox.SelectionStart = i
                    oTextBox.SelectionLength = sBuzzWord.Length
                End If
                Exit Do
            End If

            If row = startRow AndAlso col = startCol Then
                AlertBox.Show(Lexis.SearchWordNotFound)
                Exit Do
            End If
        Loop
    End Sub

    Private Sub FileReadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileReadButton.Click
        If LastReadFilePath.Length <> 0 Then
            Try
                RiyoDataOpenFileDialog.FileName = Path.GetFileName(LastReadFilePath)
                RiyoDataOpenFileDialog.InitialDirectory = Path.GetDirectoryName(LastReadFilePath)
            Catch ex As Exception
                RiyoDataOpenFileDialog.FileName = ""
            End Try
        Else
            RiyoDataOpenFileDialog.FileName = ""
        End If
        If RiyoDataOpenFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim recIndex As Long
        Dim oBytes As Byte()
        Try
            Dim oFileInfo As New FileInfo(RiyoDataOpenFileDialog.FileName)
            Dim recCount As Integer = CInt(oFileInfo.Length \ RecordLength)
            If recCount = 0 OrElse oFileInfo.Length <> RecordLength * recCount Then
                AlertBox.Show(Lexis.RiyoDataFileSizeError)
                Return
            End If

            Dim oRecSelector As New SelectRecordDialog()
            oRecSelector.Description = Lexis.SelectRecordToRead.Gen(recCount)
            oRecSelector.MaxIndex = recCount - 1
            If LastReadFilePath = RiyoDataOpenFileDialog.FileName AndAlso LastReadRecordIndex < recCount Then
                oRecSelector.Index = LastReadRecordIndex
            End If
            Using oRecSelector
                If oRecSelector.ShowDialog() <> DialogResult.OK Then Return
                recIndex = CLng(oRecSelector.Index)
            End Using

            oBytes = New Byte(RecordLength - 1) {}
            Using oInputStream As FileStream = oFileInfo.OpenRead()
                oInputStream.Position = RecordLength * recIndex
                Dim pos As Integer = 0
                Do
                    Dim readLimit As Integer = RecordLength - pos
                    If readLimit = 0 Then Exit Do
                    Dim readSize As Integer = oInputStream.Read(oBytes, pos, readLimit)
                    If readSize = 0 Then
                        Throw New EndOfStreamException()
                    End If
                    pos += readSize
                Loop
            End Using
        Catch ex As Exception
            AlertBox.Show(Lexis.RiyoDataFileReadError, ex.Message)
            Return
        End Try

        SetAllValues(oBytes)

        LastReadFilePath = RiyoDataOpenFileDialog.FileName
        LastReadRecordIndex = recIndex

        RepaintRiyoDataGridView(SearchBox.Text)
    End Sub

    Private Sub FileRewriteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileRewriteButton.Click
        If LastReadFilePath.Length <> 0 Then
            Try
                RiyoDataRewriteFileDialog.FileName = Path.GetFileName(LastReadFilePath)
                RiyoDataRewriteFileDialog.InitialDirectory = Path.GetDirectoryName(LastReadFilePath)
            Catch ex As Exception
                RiyoDataRewriteFileDialog.FileName = ""
            End Try
        Else
            RiyoDataRewriteFileDialog.FileName = ""
        End If
        If RiyoDataRewriteFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim oBytes As Byte() = GetAllValues()

        Dim fileInitialLen As Long
        Dim recIndex As Long
        Try
            Dim oFileInfo As New FileInfo(RiyoDataRewriteFileDialog.FileName)
            If oFileInfo.Exists Then
                fileInitialLen = oFileInfo.Length
                Dim recCount As Integer = CInt(fileInitialLen \ RecordLength)
                If recCount = 0 OrElse fileInitialLen <> RecordLength * recCount Then
                    AlertBox.Show(Lexis.RiyoDataFileSizeError)
                    Return
                End If
                Dim oRecSelector As New SelectRecordDialog()
                oRecSelector.Description = Lexis.SelectRecordToWrite.Gen(recCount)
                oRecSelector.MaxIndex = recCount - 1
                If LastReadFilePath = RiyoDataRewriteFileDialog.FileName AndAlso LastReadRecordIndex < recCount Then
                    oRecSelector.Index = LastReadRecordIndex
                End If
                Using oRecSelector
                    If oRecSelector.ShowDialog() <> DialogResult.OK Then Return
                    recIndex = CLng(oRecSelector.Index)
                End Using
            Else
                fileInitialLen = 0
                If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.RiyoDataFileCreateReally) = DialogResult.Cancel Then Return
                recIndex = 0
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.RiyoDataFileReadError, ex.Message)
            Return
        End Try

        Try
            Using oOutputStream As New FileStream(RiyoDataRewriteFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)
                Dim fileLen As Long = oOutputStream.Length
                If fileLen <> fileInitialLen Then Throw New OPMGException()
                oOutputStream.Seek(RecordLength * recIndex, SeekOrigin.Begin)
                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As OPMGException
            AlertBox.Show(Lexis.RiyoDataFileExclusionError)
            Return
        Catch ex As Exception
            AlertBox.Show(Lexis.RiyoDataFileWriteError, ex.Message)
            Return
        End Try

        LastReadFilePath = RiyoDataRewriteFileDialog.FileName
        LastReadRecordIndex = recIndex
    End Sub

    Private Sub FileAppendButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileAppendButton.Click
        If LastWrittenFilePath.Length <> 0 Then
            Try
                RiyoDataAppendFileDialog.FileName = Path.GetFileName(LastWrittenFilePath)
                RiyoDataAppendFileDialog.InitialDirectory = Path.GetDirectoryName(LastWrittenFilePath)
            Catch ex As Exception
                RiyoDataAppendFileDialog.FileName = ""
            End Try
        Else
            RiyoDataAppendFileDialog.FileName = ""
        End If
        If RiyoDataAppendFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim oBytes As Byte() = GetAllValues()

        Dim recIndex As Long
        Try
            Using oOutputStream As New FileStream(RiyoDataAppendFileDialog.FileName, FileMode.Append, FileAccess.Write)
                recIndex = oOutputStream.Position \ RecordLength
                If oOutputStream.Position <> recIndex * RecordLength OrElse recIndex = Long.MaxValue Then
                    AlertBox.Show(Lexis.RiyoDataFileSizeError)
                    Return
                End If
                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As Exception
            AlertBox.Show(Lexis.RiyoDataFileWriteError, ex.Message)
            Return
        End Try

        LastWrittenFilePath = RiyoDataAppendFileDialog.FileName
        LastWrittenRecordIndex = recIndex
    End Sub

    Private Sub StoreButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StoreButton.Click
        Dim oBytes As Byte() = GetAllValues()

        If ManagerForm.StoreRiyoData(MonitorMachineId, TermMachineId, oBytes) = True Then
            AlertBox.Show(Lexis.RiyoDataStoreFinished)
        Else
            AlertBox.Show(Lexis.RiyoDataStoreFailed)
        End If
    End Sub

    Private Sub SendButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendButton.Click
        If ManagerForm.SendRiyoData(MonitorMachineId) = True Then
            AlertBox.Show(Lexis.RiyoDataSendFinished)
        Else
            AlertBox.Show(Lexis.RiyoDataSendFailed)
        End If
    End Sub

    Private Sub BaseHeaderSetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BaseHeaderSetButton.Click
        If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.RiyoDataBaseHeaderSetReally) = DialogResult.No Then Return

        Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).Profile
        Dim now As DateTime = DateTime.Now
        SetValue("��{�w�b�_�[ �f�[�^���", "A0")
        SetValue("��{�w�b�_�[ �w�R�[�h", CStr(oProf(Config.MachineProfileFieldNamesIndices("RAIL_SECTION_CODE"))) & "-" & CStr(oProf(Config.MachineProfileFieldNamesIndices("STATION_ORDER_CODE"))))
        SetValue("��{�w�b�_�[ ��������", now.ToString("yyyyMMddHHmmss"))
        SetValue("��{�w�b�_�[ �R�[�i�[", CStr(oProf(Config.MachineProfileFieldNamesIndices("CORNER_CODE"))))
        SetValue("��{�w�b�_�[ ���@", CStr(oProf(Config.MachineProfileFieldNamesIndices("UNIT_NO"))))
        SetValue("��{�w�b�_�[ �V�[�P���XNo", CStr(MyUtility.GetNextSeqNumber(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).SeqNumber)))
        SetValue("��{�w�b�_�[ �o�[�W����", "02")  'TODO: ����ł悢�̂��m�F�B
    End Sub

    Private Sub MinDateReplaceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MinDateReplaceButton.Click
        Dim oDateSelector As New SelectDateTimeDialog()
        oDateSelector.Description = Lexis.RiyoDataMinDateReplaceReally.Gen()
        oDateSelector.DateTime = DateTime.Now

        Dim now As DateTime
        Using oDateSelector
            If oDateSelector.ShowDialog() <> DialogResult.OK Then Return
            now = oDateSelector.DateTime
        End Using

        'ReplaceDateIfNeeded("�ʗp��� �P���ڏ�� �L���J�n��", now)
        'ReplaceDateIfNeeded("�ʗp��� �Q���ڏ�� �L���J�n��", now)
        'ReplaceDateIfNeeded("�ʗp��� �R���ڏ�� �L���J�n��", now)
        ReplaceDateIfNeeded("���ǎ��� �P���ڏ�� �L���J�n��", now)
        ReplaceDateIfNeeded("���ǎ��� �Q���ڏ�� �L���J�n��", now)
        ReplaceDateIfNeeded("���ǎ��� �R���ڏ�� �L���J�n��", now)
        ReplaceDateIfNeeded("���ǎ��� �S���ڏ�� �L���J�n��", now)

        If GetValue("���ǎ��� �P���ڏ�� ���s����") <> "0000" Then
            SetValue("���ǎ��� �P���ڏ�� ���s����", now.ToString("MMdd"))
        End If

        If GetValue("���ǎ��� �Q���ڏ�� ���s����") <> "0000" Then
            SetValue("���ǎ��� �Q���ڏ�� ���s����", now.ToString("MMdd"))
        End If

        If GetValue("���ǎ��� �R���ڏ�� ���s����") <> "0000" Then
            SetValue("���ǎ��� �R���ڏ�� ���s����", now.ToString("MMdd"))
        End If

        If GetValue("���ǎ��� �S���ڏ�� ���s����") <> "0000" Then
            SetValue("���ǎ��� �S���ڏ�� ���s����", now.ToString("MMdd"))
        End If
    End Sub

    Private Sub MaxDateReplaceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MaxDateReplaceButton.Click
        Dim oDateSelector As New SelectDateTimeDialog()
        oDateSelector.Description = Lexis.RiyoDataMaxDateReplaceReally.Gen()
        oDateSelector.DateTime = DateTime.Now

        Dim now As DateTime
        Using oDateSelector
            If oDateSelector.ShowDialog() <> DialogResult.OK Then Return
            now = oDateSelector.DateTime
        End Using

        'ReplaceDateIfNeeded("�ʗp��� �P���ڏ�� �L���I����", now)
        'ReplaceDateIfNeeded("�ʗp��� �Q���ڏ�� �L���I����", now)
        'ReplaceDateIfNeeded("�ʗp��� �R���ڏ�� �L���I����", now)
    End Sub

    Private Sub EntDateReplaceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EntDateReplaceButton.Click
        Dim oDateSelector As New SelectDateTimeDialog()
        oDateSelector.Description = Lexis.RiyoDataEntDateReplaceReally.Gen()
        oDateSelector.DateTime = DateTime.Now

        Dim now As DateTime
        Using oDateSelector
            If oDateSelector.ShowDialog() <> DialogResult.OK Then Return
            now = oDateSelector.DateTime
        End Using

        If GetValue("��{�w�b�_�[ ��������") <> "00000000000000" Then
            SetValue("��{�w�b�_�[ ��������", now.ToString("yyyyMMddHHmmss"))
        End If

        If GetValue("���������� ��Ԍ� ��������") <> "00000000" Then
            SetValue("���������� ��Ԍ� ��������", now.ToString("MMddHHmm"))
        End If

        If GetValue("���������� ���}�� ��������") <> "00000000" Then
            SetValue("���������� ���}�� ��������", now.ToString("MMddHHmm"))
        End If
    End Sub

    Private Sub OrgStaReplaceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrgStaReplaceButton.Click
        Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).Profile
        Dim oStaSelector As New SelectStationDialog(Config.StationItems)
        oStaSelector.Description = Lexis.RiyoDataOrgStaReplaceReally.Gen()
        oStaSelector.Station = CStr(oProf(Config.MachineProfileFieldNamesIndices("RAIL_SECTION_CODE"))) & "-" & CStr(oProf(Config.MachineProfileFieldNamesIndices("STATION_ORDER_CODE")))

        Dim sStation As String
        Using oStaSelector
            If oStaSelector.ShowDialog() <> DialogResult.OK Then Return
            sStation = oStaSelector.Station
        End Using

        ReplaceStationIfNeeded("������� ��Ԍ� ���w", sStation)
        ReplaceStationIfNeeded("������� ���}�� ���w", sStation)
        ReplaceStationIfNeeded("������� �̂��݋�� ���w", sStation)
        ReplaceStationIfNeeded("������� �O���[����� ���w", sStation)
        ReplaceStationIfNeeded("������� IC��� ���w", sStation)
        ReplaceStationIfNeeded("������� FREX��� ���w", sStation)
        ReplaceStationIfNeeded("�w�茔��� �w��P �w���� ���w", sStation)
        ReplaceStationIfNeeded("�w�茔��� �w��Q �w���� ���w", sStation)
        ReplaceStationIfNeeded("�w�茔��� �w��R �w���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �P���ڏ�� ��Ԍ���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �P���ڏ�� ���}����� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �Q���ڏ�� ��Ԍ���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �Q���ڏ�� ���}����� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �R���ڏ�� ��Ԍ���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �R���ڏ�� ���}����� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �S���ڏ�� ��Ԍ���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �S���ڏ�� ���}����� ���w", sStation)

        'NOTE: �����Ԃ̒��w���r�e���p��Ԃ̗��p�w�P�ɂȂ��Ă���\���̕��������̂ŁA
        '�ȉ��́A���w�u���̑ΏۂƂ�������A���������I�ł���B
        '�����āA���������A�ʂɕύX��������Ó��ł���B
        'ReplaceStationIfNeeded("�r�e���p��ԂP ���p�w�P", sStation)
        'ReplaceStationIfNeeded("�r�e���p��ԂQ ���p�w�P", sStation)
    End Sub

    Private Sub DstStaReplaceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DstStaReplaceButton.Click
        Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).Profile
        Dim oStaSelector As New SelectStationDialog(Config.StationItems)
        oStaSelector.Description = Lexis.RiyoDataDstStaReplaceReally.Gen()
        oStaSelector.Station = CStr(oProf(Config.MachineProfileFieldNamesIndices("RAIL_SECTION_CODE"))) & "-" & CStr(oProf(Config.MachineProfileFieldNamesIndices("STATION_ORDER_CODE")))

        Dim sStation As String
        Using oStaSelector
            If oStaSelector.ShowDialog() <> DialogResult.OK Then Return
            sStation = oStaSelector.Station
        End Using

        ReplaceStationIfNeeded("������� ��Ԍ� ���w", sStation)
        ReplaceStationIfNeeded("������� ���}�� ���w", sStation)
        ReplaceStationIfNeeded("������� �̂��݋�� ���w", sStation)
        ReplaceStationIfNeeded("������� �O���[����� ���w", sStation)
        ReplaceStationIfNeeded("������� IC��� ���w", sStation)
        ReplaceStationIfNeeded("������� FREX��� ���w", sStation)
        ReplaceStationIfNeeded("�w�茔��� �w��P �w���� ���w", sStation)
        ReplaceStationIfNeeded("�w�茔��� �w��Q �w���� ���w", sStation)
        ReplaceStationIfNeeded("�w�茔��� �w��R �w���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �P���ڏ�� ��Ԍ���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �P���ڏ�� ���}����� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �Q���ڏ�� ��Ԍ���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �Q���ڏ�� ���}����� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �R���ڏ�� ��Ԍ���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �R���ڏ�� ���}����� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �S���ڏ�� ��Ԍ���� ���w", sStation)
        ReplaceStationIfNeeded("���ǎ��� �S���ڏ�� ���}����� ���w", sStation)

        'NOTE: ����̐��x�ł́A���C���R�z�V�����̉��D�@�ɓ��ꂷ��ہA���L���ڂɓ��C���R�z�V������
        '�w���ݒ肳�ꂽ���p�f�[�^����������\���͒Ⴂ�i�Ȃ��H�j�Ǝv����B
        'ReplaceStationIfNeeded("�r�e���p��ԂP ���p�w�Q", sStation)
        'ReplaceStationIfNeeded("�r�e���p��ԂQ ���p�w�Q", sStation)
    End Sub

    Private Sub EntStaReplaceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EntStaReplaceButton.Click
        Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(TermMachineId).Profile
        Dim oStaSelector As New SelectStationDialog(Config.StationItems)
        oStaSelector.Description = Lexis.RiyoDataEntStaReplaceReally.Gen()
        oStaSelector.Station = CStr(oProf(Config.MachineProfileFieldNamesIndices("RAIL_SECTION_CODE"))) & "-" & CStr(oProf(Config.MachineProfileFieldNamesIndices("STATION_ORDER_CODE")))

        Dim sStation As String
        Using oStaSelector
            If oStaSelector.ShowDialog() <> DialogResult.OK Then Return
            sStation = oStaSelector.Station
        End Using

        ReplaceStationIfNeeded("��{�w�b�_�[ �w�R�[�h", sStation)
        ReplaceStationIfNeeded("����w��� ��Ԍ� ����w", sStation)  'TODO: �R�[�i�[�ƍ��@���ꏏ�Ɏw��E�ύX�������B
        ReplaceStationIfNeeded("����w��� ���}�� ����w", sStation)

        'NOTE: ����̐��x�ł́A���C���R�z�V�����̉��D�@�ɓ��ꂷ��ہA���L���ڂɓ��C���R�z�V������
        '�w���ݒ肳�ꂽ���p�f�[�^����������\���͒Ⴂ�i�Ȃ��H�j�Ǝv����B
        'ReplaceStationIfNeeded("��Ԏn�_�w", sStation)
    End Sub

    Private Sub ReplaceDateIfNeeded(ByVal sFieldName As String, ByVal newValue As DateTime)
        Dim sOldValue As String = GetValue(sFieldName)
        If sOldValue <> "00000000" Then
            'TODO: �N�̏����͌���Ȃǂ����ƂɌ��߂�����悢�B
            Dim sNewValue As String = newValue.ToString("yyyyMMdd")
            If sOldValue.StartsWith("0000") Then
                sNewValue = "0000" & sNewValue.Substring(4)
            ElseIf sOldValue.StartsWith("000") Then
                sNewValue = "000" & sNewValue.Substring(3)
            ElseIf sOldValue.StartsWith("00") Then
                sNewValue = "00" & sNewValue.Substring(2)
            ElseIf sOldValue.StartsWith("0") Then
                sNewValue = "0" & sNewValue.Substring(1)
            End If
            SetValue(sFieldName, sNewValue)
        End If
    End Sub

    Private Sub ReplaceStationIfNeeded(ByVal sFieldName As String, ByVal sNewValue As String)
        If Config.ReplaceableRailSections.ContainsKey(GetValue(sFieldName).Substring(0, 3)) Then
            SetValue(sFieldName, sNewValue)
        End If
    End Sub

    Private Function GetValue(ByVal sMetaName As String) As String
        Dim i As Integer = RiyoDataUtil.FieldIndexOf(sMetaName)
        Return oTable.Rows(i).Field(Of String)("VALUE")
    End Function

    Private Sub SetValue(ByVal sMetaName As String, ByVal sValue As String)
        Dim i As Integer = RiyoDataUtil.FieldIndexOf(sMetaName)
        'TODO: ������ErrorText�̉�������������݂̂͂��Ƃ��Ȃ��̂ŁA
        'XlsDataGridView�ɂ�����ErrorText�������s���C�x���g��
        '�����CellValidating����ύX����B
        '���[�U�ɂ��ҏW���ۂ��Ɋ֌W�Ȃ��X�V���ɕK����������C�x���g��T���B
        RiyoDataGridView.Rows(i).Cells(3).ErrorText = ""
        oTable.Rows(i)("VALUE") = sValue
    End Sub

    Private Function GetAllValues() As Byte()
        Dim oBytes As Byte() = New Byte(RecordLength - 1) {}
        Dim bitOffset As Integer = 0
        For i As Integer = 0 To RiyoDataUtil.Fields.Length - 1
            Dim oField As XlsField = RiyoDataUtil.Fields(i)
            oField.CopyValueToBytes(oTable.Rows(i).Field(Of String)("VALUE"), oBytes, bitOffset)
            bitOffset += oField.ElementBits * oField.ElementCount
        Next i
        Return oBytes
    End Function

    Private Sub SetAllValues(ByVal oBytes As Byte())
        Dim bitOffset As Integer = 0
        For i As Integer = 0 To RiyoDataUtil.Fields.Length - 1
            Dim oField As XlsField = RiyoDataUtil.Fields(i)
            'TODO: ������ErrorText�̉�������������݂̂͂��Ƃ��Ȃ��̂ŁA
            'XlsDataGridView�ɂ�����ErrorText�������s���C�x���g��
            '�����CellValidating����ύX����B
            '���[�U�ɂ��ҏW���ۂ��Ɋ֌W�Ȃ��X�V���ɕK����������C�x���g��T���B
            RiyoDataGridView.Rows(i).Cells(3).ErrorText = ""
            oTable.Rows(i)("VALUE") = oField.CreateValueFromBytes(oBytes, bitOffset)
            bitOffset += oField.ElementBits * oField.ElementCount
        Next i
    End Sub

End Class
