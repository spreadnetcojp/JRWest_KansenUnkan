' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/11/21  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class FaultDataForm

    Private MonitorMachineId As String
    Private SourceMachineId As String
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

    Public Sub New(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oManagerForm As MainForm)
        InitializeComponent()

        Me.MonitorMachineId = sMonitorMachineId
        Me.SourceMachineId = sSourceMachineId
        Me.ManagerForm = oManagerForm

        Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).Profile
        Me.Text = Lexis.FaultDataFormTitle.Gen(oProf(Config.MachineProfileFieldNamesIndices("MODEL_NAME")), _
                                               oProf(Config.MachineProfileFieldNamesIndices("STATION_NAME")), _
                                               oProf(Config.MachineProfileFieldNamesIndices("CORNER_NAME")), _
                                               oProf(Config.MachineProfileFieldNamesIndices("UNIT_NO")))
        Me.MonitorMachineIdTextBox.Text = sMonitorMachineId
        Me.SourceMachineIdTextBox.Text = sSourceMachineId

        oTable = New DataTable()
        oTable.Columns.Add("TITLE", GetType(String))
        oTable.Columns.Add("FORMAT", GetType(String))
        oTable.Columns.Add("VALUE", GetType(String))
        Dim maxTitleWidth As Integer = 0
        Dim maxFormatWidth As Integer = 0
        For Each oField As XlsField In FaultDataUtil.Fields
            Dim formatDesc As String = oField.CreateFormatDescription()
            Dim oRow As DataRow = oTable.NewRow()
            oRow("TITLE") = oField.MetaName
            oRow("FORMAT") = formatDesc
            oRow("VALUE") = CreateInitialValue(oField)
            oTable.Rows.Add(oRow)
            Dim titleWidth As Integer = MyUtility.GetTextWidth(oField.MetaName, FaultDataGridView.Font)
            If titleWidth > maxTitleWidth Then
                maxTitleWidth = titleWidth
            End If
            Dim formatWidth As Integer = MyUtility.GetTextWidth(formatDesc & "...", FaultDataGridView.Font)
            If formatWidth > maxFormatWidth Then
                maxFormatWidth = formatWidth
            End If
        Next oField
        RecordLength = FaultDataUtil.RecordLengthInBytes

        FaultDataGridView.SuspendLayout()

        FaultDataGridView.AutoGenerateColumns = True
        FaultDataGridView.DataSource = oTable
        FaultDataGridView.AutoGenerateColumns = False

        FaultDataGridView.Columns(0).HeaderText = "���ږ�"
        FaultDataGridView.Columns(0).Width = maxTitleWidth
        FaultDataGridView.Columns(0).ReadOnly = True
        FaultDataGridView.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

        FaultDataGridView.Columns(1).HeaderText = "����"
        FaultDataGridView.Columns(1).Width = maxFormatWidth
        FaultDataGridView.Columns(1).ReadOnly = True
        FaultDataGridView.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable

        FaultDataGridView.Columns(2).HeaderText = "�l"
        FaultDataGridView.Columns(2).AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        FaultDataGridView.Columns(2).FillWeight = 100.0!
        FaultDataGridView.Columns(2).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        FaultDataGridView.Columns(2).DefaultCellStyle.Font = New System.Drawing.Font("MS Gothic", 9.0!)
        FaultDataGridView.Columns(2).ReadOnly = False
        FaultDataGridView.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable

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
        FaultDataGridView.Columns.Insert(3, oComboColumn)

        FaultDataGridView.ResumeLayout()

        LastReadFilePath = ""
        LastReadRecordIndex = 0
        LastWrittenFilePath = ""
        LastWrittenRecordIndex = 0
    End Sub

    Private Function CreateInitialValue(ByVal oField As XlsField) As String
        Select Case oField.MetaName
            Case "��{�w�b�_�[ �f�[�^���"
                Return "C3"
            Case "��{�w�b�_�[ �w�R�[�h"
                Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).Profile
                Return CStr(oProf(Config.MachineProfileFieldNamesIndices("RAIL_SECTION_CODE"))) & "-" & CStr(oProf(Config.MachineProfileFieldNamesIndices("STATION_ORDER_CODE")))
            Case "��{�w�b�_�[ ��������"
                Return DateTime.Now.ToString("yyyyMMddHHmmss")
            Case "��{�w�b�_�[ �R�[�i�["
                Return CStr(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).Profile(Config.MachineProfileFieldNamesIndices("CORNER_CODE")))
            Case "��{�w�b�_�[ ���@"
                'Return CStr(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).Profile(Config.MachineProfileFieldNamesIndices("UNIT_NO")))
                Return "0"
            Case "��{�w�b�_�[ �V�[�P���XNo"
                'Return CStr(MyUtility.GetNextSeqNumber(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).FaultSeqNumber))
                Return "0"
            Case "��{�w�b�_�[ �o�[�W����"
                Return "01"
            Case "�f�[�^�����O�X"
                Return "780"
            Case "��������"
                Return DateTime.Now.ToString("yyyyMMddHHmmss") & "00"
            Case "���@�ԍ�"
                Return CInt(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).Profile(Config.MachineProfileFieldNamesIndices("UNIT_NO"))).ToString("D2")
            Case "�ʘH����"
                Return FaultDataUtil.CreatePassDirectionValue(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).LatchConf)
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

        For i As Integer = 0 To FaultDataUtil.Fields.Length - 1
            FaultDataGridView.Rows(i).Cells(2).Tag = FaultDataUtil.Fields(i)

            Dim oCell As DataGridViewCell = FaultDataGridView.Rows(i).Cells(3)
            Dim oCombo As DataGridViewComboBoxCell = DirectCast(oCell, DataGridViewComboBoxCell)
            Select Case FaultDataUtil.Fields(i).MetaType
                Case "DataKind"
                    oCombo.DataSource = Config.DataKindItems
                    oCell.ReadOnly = False
                Case "Station"
                    oCombo.DataSource = Config.StationItems
                    oCell.ReadOnly = False
                Case "PassDirection"
                    oCombo.DataSource = Config.PassDirectionItems
                    oCell.ReadOnly = False
                Case "FaultDataErrorCode"
                    oCombo.DataSource = Config.FaultDataErrorCodeItems
                    oCell.ReadOnly = False
                Case Else
                    oCell.Style = CellStyleOfDisabled
                    oCell.ReadOnly = True
            End Select
        Next i

        'FaultDataGridView.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders)

        For i As Integer = 0 To Config.SearchBoxInitialHis.Rows.Count - 1
            SearchBox.Items.Insert(i, Config.SearchBoxInitialHis.Rows(i).Field(Of String)("Value"))
        Next i

        RepaintFaultDataGridView(SearchBox.Text)
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        ManagerForm.FaultDataFormDic.Remove(MonitorMachineId & SourceMachineId)
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
    Private Sub FaultDataGridView_CellValueChanged(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles FaultDataGridView.CellValueChanged
        If e.RowIndex < 0 Then Return
        Dim oView As DataRowView = DirectCast(FaultDataGridView.Rows(e.RowIndex).DataBoundItem, DataRowView)
        oView.Row.AcceptChanges()
    End Sub

    Private Sub SearchBox_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles SearchBox.KeyDown
        If (e.KeyCode = Keys.Enter) AndAlso Not e.Alt AndAlso Not e.Control Then
            UpdateSearchHistory(SearchBox.Text)
            RepaintFaultDataGridView(SearchBox.Text)
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
        RepaintFaultDataGridView(SearchBox.Text)
    End Sub

    'Private Sub SearchBox_DropDownClosed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchBox.DropDownClosed
    '    UpdateSearchHistory(SearchBox.SelectedItem.ToString())
    '    RepaintFaultDataGridView(SearchBox.Text)
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

    Private Sub RepaintFaultDataGridView(ByVal sBuzzWord As String)
        Dim oRows As DataGridViewRowCollection = FaultDataGridView.Rows
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
        Dim rowCount As Integer = FaultDataGridView.Rows.Count
        Dim colCount As Integer = FaultDataGridView.Columns.Count

        Dim startRow As Integer
        Dim startCol As Integer
        If FaultDataGridView.CurrentCell IsNot Nothing Then
            startRow = FaultDataGridView.CurrentCell.RowIndex
            startCol = FaultDataGridView.CurrentCell.ColumnIndex
        Else
            startRow = 0
            startCol = 0
        End If

        Dim sBuzzWord As String = SearchBox.Text
        Dim oRows As DataGridViewRowCollection = FaultDataGridView.Rows
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
                FaultDataGridView.CurrentCell = oCell
                If Not oCell.ReadOnly AndAlso TypeOf oCell Is DataGridViewTextBoxCell Then
                    FaultDataGridView.BeginEdit(True)
                    Dim oTextBox As DataGridViewTextBoxEditingControl = _
                       DirectCast(FaultDataGridView.EditingControl, DataGridViewTextBoxEditingControl)
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
        Dim rowCount As Integer = FaultDataGridView.Rows.Count
        Dim colCount As Integer = FaultDataGridView.Columns.Count

        Dim startRow As Integer
        Dim startCol As Integer
        If FaultDataGridView.CurrentCell IsNot Nothing Then
            startRow = FaultDataGridView.CurrentCell.RowIndex
            startCol = FaultDataGridView.CurrentCell.ColumnIndex
        Else
            startRow = rowCount - 1
            startCol = colCount - 1
        End If

        Dim sBuzzWord As String = SearchBox.Text
        Dim oRows As DataGridViewRowCollection = FaultDataGridView.Rows
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
                FaultDataGridView.CurrentCell = oCell
                If Not oCell.ReadOnly AndAlso TypeOf oCell Is DataGridViewTextBoxCell Then
                    FaultDataGridView.BeginEdit(True)
                    Dim oTextBox As DataGridViewTextBoxEditingControl = _
                       DirectCast(FaultDataGridView.EditingControl, DataGridViewTextBoxEditingControl)
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
                FaultDataOpenFileDialog.FileName = Path.GetFileName(LastReadFilePath)
                FaultDataOpenFileDialog.InitialDirectory = Path.GetDirectoryName(LastReadFilePath)
            Catch ex As Exception
                FaultDataOpenFileDialog.FileName = ""
            End Try
        Else
            FaultDataOpenFileDialog.FileName = ""
        End If
        If FaultDataOpenFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim recIndex As Long
        Dim oBytes As Byte()
        Try
            Dim oFileInfo As New FileInfo(FaultDataOpenFileDialog.FileName)
            Dim recCount As Integer = CInt(oFileInfo.Length \ RecordLength)
            If recCount = 0 OrElse oFileInfo.Length <> RecordLength * recCount Then
                AlertBox.Show(Lexis.FaultDataFileSizeError)
                Return
            End If
            If recCount = 1 Then
                recIndex = 0
            Else
                recCount -= 1
                Dim oRecSelector As New SelectRecordDialog()
                oRecSelector.Description = Lexis.SelectRecordToRead.Gen(recCount)
                oRecSelector.MaxIndex = recCount - 1
                If LastReadFilePath = FaultDataOpenFileDialog.FileName AndAlso LastReadRecordIndex < recCount Then
                    oRecSelector.Index = LastReadRecordIndex
                End If
                Using oRecSelector
                    If oRecSelector.ShowDialog() <> DialogResult.OK Then Return
                    recIndex = CLng(oRecSelector.Index) + 1
                End Using
            End If

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
            AlertBox.Show(Lexis.FaultDataFileReadError, ex.Message)
            Return
        End Try

        SetAllValues(oBytes)

        LastReadFilePath = FaultDataOpenFileDialog.FileName
        LastReadRecordIndex = If(recIndex = 0, 0, recIndex - 1)

        RepaintFaultDataGridView(SearchBox.Text)
    End Sub

    Private Sub FileRewriteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileRewriteButton.Click
        If LastReadFilePath.Length <> 0 Then
            Try
                FaultDataRewriteFileDialog.FileName = Path.GetFileName(LastReadFilePath)
                FaultDataRewriteFileDialog.InitialDirectory = Path.GetDirectoryName(LastReadFilePath)
            Catch ex As Exception
                FaultDataRewriteFileDialog.FileName = ""
            End Try
        Else
            FaultDataRewriteFileDialog.FileName = ""
        End If
        If FaultDataRewriteFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim oBytes As Byte() = GetAllValues()

        Dim fileInitialLen As Long
        Dim recIndex As Long
        Dim mode As FileMode
        Try
            Dim oFileInfo As New FileInfo(FaultDataRewriteFileDialog.FileName)
            If oFileInfo.Exists Then
                fileInitialLen = oFileInfo.Length
                Dim recCount As Integer = CInt(fileInitialLen \ RecordLength)
                If recCount = 0 OrElse fileInitialLen <> RecordLength * recCount Then
                    AlertBox.Show(Lexis.FaultDataFileSizeError)
                    Return
                End If
                If recCount = 1 Then
                    If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.FaultDataFileForActiveOneRewriteReally) = DialogResult.Cancel Then Return
                    recIndex = 0
                    mode = FileMode.Create  'NOTE: FileStream���쐬����܂łɑ��̃v���Z�X���ǋL�����s�����ꍇ�A����������Ă��܂����A��ނ𓾂Ȃ��B
                Else
                    recCount -= 1
                    Dim oRecSelector As New SelectRecordDialog()
                    oRecSelector.Description = Lexis.SelectRecordToWrite.Gen(recCount)
                    oRecSelector.MaxIndex = recCount - 1
                    If LastReadFilePath = FaultDataRewriteFileDialog.FileName AndAlso LastReadRecordIndex < recCount Then
                        oRecSelector.Index = LastReadRecordIndex
                    End If
                    Using oRecSelector
                        If oRecSelector.ShowDialog() <> DialogResult.OK Then Return
                        recIndex = CLng(oRecSelector.Index) + 1
                    End Using
                    mode = FileMode.OpenOrCreate
                End If
            Else
                fileInitialLen = 0
                Dim sDataKind As String = FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �f�[�^���", oBytes)
                Dim format As Integer = 0
                If sDataKind = "A6" Then
                    format = 1
                End If

                Dim oFormatSelector As New SelectFileFormatDialog()
                oFormatSelector.Description = Lexis.FaultDataFileFormatSelectorDescription.Gen()
                oFormatSelector.Format0Text = Lexis.FaultDataFileFormatSelectorFormat0Text.Gen()
                oFormatSelector.Format1Text = Lexis.FaultDataFileFormatSelectorFormat1Text.Gen()
                oFormatSelector.Format = format

                Using oFormatSelector
                    If oFormatSelector.ShowDialog() <> DialogResult.OK Then Return
                    format = oFormatSelector.Format
                End Using

                If format = 0 Then
                    recIndex = 0
                    mode = FileMode.Append
                Else
                    recIndex = 1
                    mode = FileMode.OpenOrCreate
                End If
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.FaultDataFileReadError, ex.Message)
            Return
        End Try

        If DataKindAutoAdjustCheckBox.Checked Then
            If mode = FileMode.OpenOrCreate Then
                FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A6", oBytes)
            Else
                FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "C3", oBytes)
            End If
        End If

        Dim now As DateTime = DateTime.Now
        Try
            Using oOutputStream As New FileStream(FaultDataRewriteFileDialog.FileName, mode, FileAccess.Write, FileShare.Read)
                If mode = FileMode.OpenOrCreate Then
                    Dim fileLen As Long = oOutputStream.Length
                    If fileLen <> fileInitialLen Then Throw New OPMGException()

                    If fileLen < RecordLength Then
                        ExUpboundFileHeader.WriteToStream(&HB8, 1, RecordLength, now, oOutputStream)
                    Else
                        oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                        Dim recCount As Integer = CInt((fileLen \ RecordLength) - 1)
                        ExUpboundFileHeader.WriteToStream(&HB8, recCount, RecordLength, now, oOutputStream)
                        oOutputStream.Seek(RecordLength * recIndex, SeekOrigin.Begin)
                    End If
                End If

                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As OPMGException
            AlertBox.Show(Lexis.FaultDataFileExclusionError)
            Return
        Catch ex As Exception
            AlertBox.Show(Lexis.FaultDataFileWriteError, ex.Message)
            Return
        End Try

        LastReadFilePath = FaultDataRewriteFileDialog.FileName
        LastReadRecordIndex = If(recIndex = 0, 0, recIndex - 1)
    End Sub

    'OPT: FileRewriteButton_Click�ŁA���R�[�h�̃C���f�b�N�X��recCount���w��ł���悤�ɂ���Α�p�\�B
    Private Sub FileAppendButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileAppendButton.Click
        If LastWrittenFilePath.Length <> 0 Then
            Try
                FaultDataAppendFileDialog.FileName = Path.GetFileName(LastWrittenFilePath)
                FaultDataAppendFileDialog.InitialDirectory = Path.GetDirectoryName(LastWrittenFilePath)
            Catch ex As Exception
                FaultDataAppendFileDialog.FileName = ""
            End Try
        Else
            FaultDataAppendFileDialog.FileName = ""
        End If
        If FaultDataAppendFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim oBytes As Byte() = GetAllValues()

        Dim fileInitialLen As Long
        Dim recIndex As Long
        Dim mode As FileMode
        Try
            Dim oFileInfo As New FileInfo(FaultDataAppendFileDialog.FileName)
            If oFileInfo.Exists Then
                fileInitialLen = oFileInfo.Length
                Dim recCount As Integer = CInt(fileInitialLen \ RecordLength)
                If recCount = 0 OrElse fileInitialLen <> RecordLength * recCount Then
                    AlertBox.Show(Lexis.FaultDataFileSizeError)
                    Return
                End If
                If recCount = 1 Then
                    AlertBox.Show(Lexis.FaultDataFileForActiveOneAppendError)
                    Return
                Else
                    If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.FaultDataFileForPassiveUllAppendReally) = DialogResult.Cancel Then Return
                    recIndex = recCount
                    mode = FileMode.OpenOrCreate
                End If
            Else
                fileInitialLen = 0
                Dim sDataKind As String = FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �f�[�^���", oBytes)
                Dim format As Integer = 0
                If sDataKind = "A6" Then
                    format = 1
                End If

                Dim oFormatSelector As New SelectFileFormatDialog()
                oFormatSelector.Description = Lexis.FaultDataFileFormatSelectorDescription.Gen()
                oFormatSelector.Format0Text = Lexis.FaultDataFileFormatSelectorFormat0Text.Gen()
                oFormatSelector.Format1Text = Lexis.FaultDataFileFormatSelectorFormat1Text.Gen()
                oFormatSelector.Format = format

                Using oFormatSelector
                    If oFormatSelector.ShowDialog() <> DialogResult.OK Then Return
                    format = oFormatSelector.Format
                End Using

                If format = 0 Then
                    recIndex = 0
                    mode = FileMode.Append
                Else
                    recIndex = 1
                    mode = FileMode.OpenOrCreate
                End If
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.FaultDataFileReadError, ex.Message)
            Return
        End Try

        If DataKindAutoAdjustCheckBox.Checked Then
            If mode = FileMode.OpenOrCreate Then
                FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A6", oBytes)
            Else
                FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "C3", oBytes)
            End If
        End If

        Dim now As DateTime = DateTime.Now
        Try
            Using oOutputStream As New FileStream(FaultDataAppendFileDialog.FileName, mode, FileAccess.Write, FileShare.Read)
                Dim fileLen As Long = oOutputStream.Length
                If fileLen <> fileInitialLen Then Throw New OPMGException()

                If mode = FileMode.OpenOrCreate Then
                    If fileLen < RecordLength Then
                        ExUpboundFileHeader.WriteToStream(&HB8, 1, RecordLength, now, oOutputStream)
                    Else
                        oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                        Dim totalRecCount As Integer = CInt((fileLen \ RecordLength) - 1) + 1
                        ExUpboundFileHeader.WriteToStream(&HB8, totalRecCount, RecordLength, now, oOutputStream)
                        oOutputStream.Seek(0, SeekOrigin.End)
                    End If
                End If

                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As OPMGException
            AlertBox.Show(Lexis.FaultDataFileExclusionError)
            Return
        Catch ex As Exception
            AlertBox.Show(Lexis.FaultDataFileWriteError, ex.Message)
            Return
        End Try

        LastWrittenFilePath = FaultDataAppendFileDialog.FileName
        LastWrittenRecordIndex = If(recIndex = 0, 0, recIndex - 1)
    End Sub

    Private Sub StoreButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StoreButton.Click
        Dim oBytes As Byte() = GetAllValues()

        If DataKindAutoAdjustCheckBox.Checked Then
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A6", oBytes)
        End If

        If ManagerForm.StoreFaultData(MonitorMachineId, SourceMachineId, oBytes) = True Then
            AlertBox.Show(Lexis.FaultDataStoreFinished)
        Else
            AlertBox.Show(Lexis.FaultDataStoreFailed)
        End If
    End Sub

    Private Sub SendButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendButton.Click
        Dim oBytes As Byte() = GetAllValues()

        If DataKindAutoAdjustCheckBox.Checked Then
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "C3", oBytes)
        End If

        If ManagerForm.SendFaultData(MonitorMachineId, SourceMachineId, oBytes) = True Then
            AlertBox.Show(Lexis.FaultDataSendFinished)
        Else
            AlertBox.Show(Lexis.FaultDataSendFailed)
        End If
    End Sub

    Private Sub BaseHeaderSetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BaseHeaderSetButton.Click
        If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.FaultDataBaseHeaderSetReally) = DialogResult.No Then Return

        Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).Profile
        Dim now As DateTime = DateTime.Now
        SetValue("��{�w�b�_�[ �f�[�^���", "C3")
        SetValue("��{�w�b�_�[ �w�R�[�h", CStr(oProf(Config.MachineProfileFieldNamesIndices("RAIL_SECTION_CODE"))) & "-" & CStr(oProf(Config.MachineProfileFieldNamesIndices("STATION_ORDER_CODE"))))
        SetValue("��{�w�b�_�[ ��������", now.ToString("yyyyMMddHHmmss"))
        SetValue("��{�w�b�_�[ �R�[�i�[", CStr(oProf(Config.MachineProfileFieldNamesIndices("CORNER_CODE"))))
        'SetValue("��{�w�b�_�[ ���@", CStr(oProf(Config.MachineProfileFieldNamesIndices("UNIT_NO"))))
        SetValue("��{�w�b�_�[ ���@", "0")
        'SetValue("��{�w�b�_�[ �V�[�P���XNo", CStr(MyUtility.GetNextSeqNumber(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).FaultSeqNumber)))
        SetValue("��{�w�b�_�[ �V�[�P���XNo", "0")
        SetValue("��{�w�b�_�[ �o�[�W����", "01")
    End Sub

    Private Sub AllHeadersSetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllHeadersSetButton.Click
        If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.FaultDataAllHeadersSetReally) = DialogResult.No Then Return

        Dim oProf As Object() = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).Profile
        Dim now As DateTime = DateTime.Now
        SetValue("��{�w�b�_�[ �f�[�^���", "C3")
        SetValue("��{�w�b�_�[ �w�R�[�h", CStr(oProf(Config.MachineProfileFieldNamesIndices("RAIL_SECTION_CODE"))) & "-" & CStr(oProf(Config.MachineProfileFieldNamesIndices("STATION_ORDER_CODE"))))
        SetValue("��{�w�b�_�[ ��������", now.ToString("yyyyMMddHHmmss"))
        SetValue("��{�w�b�_�[ �R�[�i�[", CStr(oProf(Config.MachineProfileFieldNamesIndices("CORNER_CODE"))))
        'SetValue("��{�w�b�_�[ ���@", CStr(oProf(Config.MachineProfileFieldNamesIndices("UNIT_NO"))))
        SetValue("��{�w�b�_�[ ���@", "0")
        'SetValue("��{�w�b�_�[ �V�[�P���XNo", CStr(MyUtility.GetNextSeqNumber(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).FaultSeqNumber)))
        SetValue("��{�w�b�_�[ �V�[�P���XNo", "0")
        SetValue("��{�w�b�_�[ �o�[�W����", "01")
        SetValue("�f�[�^�����O�X", "780")
        SetValue("��������", now.ToString("yyyyMMddHHmmss") & "00")
        SetValue("���@�ԍ�", CInt(oProf(Config.MachineProfileFieldNamesIndices("UNIT_NO"))).ToString("D2"))
        SetValue("�ʘH����", FaultDataUtil.CreatePassDirectionValue(ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId).LatchConf))
    End Sub

    Private Sub ErrorTextsSetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ErrorTextsSetButton.Click
        If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.FaultDataErrorTextsSetReally) = DialogResult.No Then Return

        'Dim oRows As DataRow() = Config.FaultDataErrorCodeItems.Select("Key = '" & GetValue("�G���[�R�[�h") & "'")
        'If oRows.Count = 0 Then
        '    AlertBox.Show(Lexis.FaultDataErrorTextsNotFound)
        '    Return
        'End If
        'SetValue("�ُ퍀�� �\���f�[�^", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("�ُ퍀�� �\���f�[�^"), oRows(0).Field(Of String)("Value").Substring(9), &H20))
        'SetByteCountValue("�ُ퍀��")

        Try
            Dim sCode As String = GetValue("�G���[�R�[�h")
            Dim sText As String = Nothing

            If Config.FaultDataErrorOutlines.TryGetValue(sCode, sText) = False Then
                sText = "\0"
            End If
            SetValue("�ُ퍀�� �\���f�[�^", sText)
            SetByteCountValue("�ُ퍀��")

            If Config.FaultDataErrorLabels.TryGetValue(sCode, sText) = False Then
                sText = "\0"
            End If
            SetValue("�S�����\�� �\���f�[�^", sText)
            SetByteCountValue("�S�����\��")

            If Config.FaultDataErrorDetails.TryGetValue(sCode, sText) = False Then
                sText = "\0"
            End If
            SetValue("�ϕ\���� �\���f�[�^", sText)
            SetByteCountValue("�ϕ\����")

            If Config.FaultDataErrorGuidances.TryGetValue(sCode, sText) = False Then
                sText = "\0"
            End If
            SetValue("���u���e �\���f�[�^", sText)
            SetByteCountValue("���u���e")
        Catch ex As Exception
            AlertBox.Show(Lexis.FaultDataErrorTextsSetFailed, ex.Message)
        End Try
    End Sub

    Private Sub ByteCountsSetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ByteCountsSetButton.Click
        If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.FaultDataByteCountsSetReally) = DialogResult.No Then Return

        SetByteCountValue("�ُ퍀��")
        SetByteCountValue("�S�����\��")
        SetByteCountValue("�ϕ\����")
        SetByteCountValue("���u���e")
    End Sub

    Private Sub SetByteCountValue(ByVal sSuperName As String)
        Dim sDataFieldName As String = sSuperName & " �\���f�[�^"
        Dim sLenFieldName As String = sSuperName & " �L���o�C�g��"
        SetValue(sLenFieldName, MyUtility.GetValidByteCount(FaultDataUtil.Field(sDataFieldName), GetValue(sDataFieldName)).ToString())
    End Sub

    Private Function GetValue(ByVal sMetaName As String) As String
        Dim i As Integer = FaultDataUtil.FieldIndexOf(sMetaName)
        Return oTable.Rows(i).Field(Of String)("VALUE")
    End Function

    Private Sub SetValue(ByVal sMetaName As String, ByVal sValue As String)
        Dim i As Integer = FaultDataUtil.FieldIndexOf(sMetaName)
        'TODO: ������ErrorText�̉�������������݂̂͂��Ƃ��Ȃ��̂ŁA
        'XlsDataGridView�ɂ�����ErrorText�������s���C�x���g��
        '�����CellValidating����ύX����B
        '���[�U�ɂ��ҏW���ۂ��Ɋ֌W�Ȃ��X�V���ɕK����������C�x���g��T���B
        FaultDataGridView.Rows(i).Cells(3).ErrorText = ""
        oTable.Rows(i)("VALUE") = sValue
    End Sub

    Private Function GetAllValues() As Byte()
        Dim oBytes As Byte() = New Byte(RecordLength - 1) {}
        Dim bitOffset As Integer = 0
        For i As Integer = 0 To FaultDataUtil.Fields.Length - 1
            Dim oField As XlsField = FaultDataUtil.Fields(i)
            oField.CopyValueToBytes(oTable.Rows(i).Field(Of String)("VALUE"), oBytes, bitOffset)
            bitOffset += oField.ElementBits * oField.ElementCount
        Next i
        Return oBytes
    End Function

    Private Sub SetAllValues(ByVal oBytes As Byte())
        Dim bitOffset As Integer = 0
        For i As Integer = 0 To FaultDataUtil.Fields.Length - 1
            Dim oField As XlsField = FaultDataUtil.Fields(i)
            'TODO: ������ErrorText�̉�������������݂̂͂��Ƃ��Ȃ��̂ŁA
            'XlsDataGridView�ɂ�����ErrorText�������s���C�x���g��
            '�����CellValidating����ύX����B
            '���[�U�ɂ��ҏW���ۂ��Ɋ֌W�Ȃ��X�V���ɕK����������C�x���g��T���B
            FaultDataGridView.Rows(i).Cells(3).ErrorText = ""
            oTable.Rows(i)("VALUE") = oField.CreateValueFromBytes(oBytes, bitOffset)
            bitOffset += oField.ElementBits * oField.ElementCount
        Next i
    End Sub

End Class
