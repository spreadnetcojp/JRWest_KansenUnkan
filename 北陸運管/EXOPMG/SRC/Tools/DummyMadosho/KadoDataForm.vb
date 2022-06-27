' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/11/21  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class KadoDataForm

    Private MonitorMachineId As String
    Private SourceMachineId As String
    Private ManagerForm As MainForm
    Private RecordLength As Integer
    Private oTable As DataTable
    Private LastReadFilePath As String
    Private LastReadRecordIndex As Long
    Private LastWrittenFilePath As String
    Private LastWrittenRecordIndex As Long

    'スタイル
    Private CellStyleOfPlain As DataGridViewCellStyle
    Private CellStyleOfDisabled As DataGridViewCellStyle
    Private CellStyleOfHighlighted As DataGridViewCellStyle

    Public Sub New(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal sManagementFilePath As String, ByVal oManagerForm As MainForm)
        InitializeComponent()

        Me.MonitorMachineId = sMonitorMachineId
        Me.SourceMachineId = sSourceMachineId
        Me.ManagerForm = oManagerForm

        Dim oTerm As TermMachine = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId)
        Dim oProf As Object() = oTerm.Profile
        Me.Text = Lexis.KadoDataFormTitle.Gen(oProf(Config.MachineProfileFieldNamesIndices("MODEL_NAME")), _
                                              oProf(Config.MachineProfileFieldNamesIndices("STATION_NAME")), _
                                              oProf(Config.MachineProfileFieldNamesIndices("CORNER_NAME")), _
                                              oProf(Config.MachineProfileFieldNamesIndices("UNIT_NO")))
        Me.MonitorMachineIdTextBox.Text = sMonitorMachineId
        Me.SourceMachineIdTextBox.Text = sSourceMachineId

        RecordLength = KadoDataUtil.RecordLengthInBytes

        Dim oBytes As Byte() = New Byte(RecordLength - 1) {}
        Do
            Try
                Using oInputStream As New FileStream(sManagementFilePath, FileMode.Open, FileAccess.Read)
                    Dim fileLen As Long = oInputStream.Length
                    If fileLen < RecordLength * (oTerm.KadoSlot + 1) Then Throw New OPMGException()
                    oInputStream.Seek(RecordLength * oTerm.KadoSlot, SeekOrigin.Begin)

                    Dim pos As Integer = 0
                    Dim len As Integer = RecordLength
                    While pos < len
                        Dim readSize As Integer = oInputStream.Read(oBytes, pos, len - pos)
                        If readSize = 0 Then Throw New IOException()  'TODO: ロックされたわけではなく、ロックせずにTruncateされたはずであるため、別の文言にしてもよい。
                        pos += readSize
                    End While
                End Using
                Exit Do
            Catch ex As Exception
                Dim exType As Type = ex.GetType()
                If exType Is GetType(IOException) Then
                    If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.KadoDataManagementFileIsLocked) = DialogResult.Yes Then Continue Do
                ElseIf exType Is GetType(OPMGException)
                     AlertBox.Show(Lexis.KadoDataManagementFileIsBroken)
                ElseIf exType IsNot GetType(FileNotFoundException) Then
                    AlertBox.Show(Lexis.KadoDataManagementFileReadError)
                End If

                Dim termEkCode As EkCode = MainForm.GetEkCodeOf(sSourceMachineId)
                Dim now As DateTime = DateTime.Now
                oBytes.Initialize()
                KadoDataUtil.InitBaseHeaderFields(termEkCode, now, MyUtility.GetNextSeqNumber(oTerm.KadoSeqNumber), oBytes)
                KadoDataUtil.InitCommonPartFields(termEkCode, now, oBytes)
                Exit Do
            End Try
        Loop

        oTable = New DataTable()
        oTable.Columns.Add("TITLE", GetType(String))
        oTable.Columns.Add("FORMAT", GetType(String))
        oTable.Columns.Add("VALUE", GetType(String))
        Dim maxTitleWidth As Integer = 0
        Dim maxFormatWidth As Integer = 0
        For Each oField As XlsField In KadoDataUtil.Fields
            Dim formatDesc As String = oField.CreateFormatDescription()
            Dim oRow As DataRow = oTable.NewRow()
            oRow("TITLE") = oField.MetaName
            oRow("FORMAT") = formatDesc
            oRow("VALUE") = KadoDataUtil.GetFieldValueFromBytes(oField.MetaName, oBytes)
            oTable.Rows.Add(oRow)
            Dim titleWidth As Integer = MyUtility.GetTextWidth(oField.MetaName, KadoDataGridView.Font)
            If titleWidth > maxTitleWidth Then
                maxTitleWidth = titleWidth
            End If
            Dim formatWidth As Integer = MyUtility.GetTextWidth(formatDesc & "...", KadoDataGridView.Font)
            If formatWidth > maxFormatWidth Then
                maxFormatWidth = formatWidth
            End If
        Next oField

        KadoDataGridView.SuspendLayout()

        KadoDataGridView.AutoGenerateColumns = True
        KadoDataGridView.DataSource = oTable
        KadoDataGridView.AutoGenerateColumns = False

        KadoDataGridView.Columns(0).HeaderText = "項目名"
        KadoDataGridView.Columns(0).Width = maxTitleWidth
        KadoDataGridView.Columns(0).ReadOnly = True
        KadoDataGridView.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

        KadoDataGridView.Columns(1).HeaderText = "書式"
        KadoDataGridView.Columns(1).Width = maxFormatWidth
        KadoDataGridView.Columns(1).ReadOnly = True
        KadoDataGridView.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable

        KadoDataGridView.Columns(2).HeaderText = "値"
        KadoDataGridView.Columns(2).AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        KadoDataGridView.Columns(2).FillWeight = 100.0!
        KadoDataGridView.Columns(2).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        KadoDataGridView.Columns(2).DefaultCellStyle.Font = New System.Drawing.Font("MS Gothic", 9.0!)
        KadoDataGridView.Columns(2).ReadOnly = False
        KadoDataGridView.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable

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
        oComboColumn.FillWeight = 160.0!
        oComboColumn.SortMode = DataGridViewColumnSortMode.NotSortable
        KadoDataGridView.Columns.Insert(3, oComboColumn)

        KadoDataGridView.ResumeLayout()

        LastReadFilePath = ""
        LastReadRecordIndex = 0
        LastWrittenFilePath = ""
        LastWrittenRecordIndex = 0
    End Sub

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)

        CellStyleOfPlain = New DataGridViewCellStyle()

        CellStyleOfDisabled = New DataGridViewCellStyle()
        CellStyleOfDisabled.BackColor = System.Drawing.Color.LightGray

        CellStyleOfHighlighted = New DataGridViewCellStyle()
        CellStyleOfHighlighted.BackColor = System.Drawing.Color.Yellow

        For i As Integer = 0 To KadoDataUtil.Fields.Length - 1
            KadoDataGridView.Rows(i).Cells(2).Tag = KadoDataUtil.Fields(i)

            Dim oCell As DataGridViewCell = KadoDataGridView.Rows(i).Cells(3)
            Dim oCombo As DataGridViewComboBoxCell = DirectCast(oCell, DataGridViewComboBoxCell)
            Select Case KadoDataUtil.Fields(i).MetaType
                Case "DataKind"
                    oCombo.DataSource = Config.DataKindItems
                    oCell.ReadOnly = False
                Case "Station"
                    oCombo.DataSource = Config.StationItems
                    oCell.ReadOnly = False
                Case Else
                    oCell.Style = CellStyleOfDisabled
                    oCell.ReadOnly = True
            End Select
        Next i

        'KadoDataGridView.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders)

        For i As Integer = 0 To Config.SearchBoxInitialHis.Rows.Count - 1
            SearchBox.Items.Insert(i, Config.SearchBoxInitialHis.Rows(i).Field(Of String)("Value"))
        Next i

        RepaintKadoDataGridView(SearchBox.Text)
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        ManagerForm.KadoDataFormDic.Remove(MonitorMachineId & SourceMachineId)
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

    'OPT: oTableをファイルに書き出すときにまとめて行う方がよいかもしれない。
    Private Sub KadoDataGridView_CellValueChanged(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles KadoDataGridView.CellValueChanged
        If e.RowIndex < 0 Then Return
        Dim oView As DataRowView = DirectCast(KadoDataGridView.Rows(e.RowIndex).DataBoundItem, DataRowView)
        oView.Row.AcceptChanges()
    End Sub

    Private Sub SearchBox_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles SearchBox.KeyDown
        If (e.KeyCode = Keys.Enter) AndAlso Not e.Alt AndAlso Not e.Control Then
            UpdateSearchHistory(SearchBox.Text)
            RepaintKadoDataGridView(SearchBox.Text)
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
        RepaintKadoDataGridView(SearchBox.Text)
    End Sub

    'Private Sub SearchBox_DropDownClosed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchBox.DropDownClosed
    '    UpdateSearchHistory(SearchBox.SelectedItem.ToString())
    '    RepaintKadoDataGridView(SearchBox.Text)
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

    Private Sub RepaintKadoDataGridView(ByVal sBuzzWord As String)
        Dim oRows As DataGridViewRowCollection = KadoDataGridView.Rows
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
        Dim rowCount As Integer = KadoDataGridView.Rows.Count
        Dim colCount As Integer = KadoDataGridView.Columns.Count

        Dim startRow As Integer
        Dim startCol As Integer
        If KadoDataGridView.CurrentCell IsNot Nothing Then
            startRow = KadoDataGridView.CurrentCell.RowIndex
            startCol = KadoDataGridView.CurrentCell.ColumnIndex
        Else
            startRow = 0
            startCol = 0
        End If

        Dim sBuzzWord As String = SearchBox.Text
        Dim oRows As DataGridViewRowCollection = KadoDataGridView.Rows
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
                KadoDataGridView.CurrentCell = oCell
                If Not oCell.ReadOnly AndAlso TypeOf oCell Is DataGridViewTextBoxCell Then
                    KadoDataGridView.BeginEdit(True)
                    Dim oTextBox As DataGridViewTextBoxEditingControl = _
                       DirectCast(KadoDataGridView.EditingControl, DataGridViewTextBoxEditingControl)
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
        Dim rowCount As Integer = KadoDataGridView.Rows.Count
        Dim colCount As Integer = KadoDataGridView.Columns.Count

        Dim startRow As Integer
        Dim startCol As Integer
        If KadoDataGridView.CurrentCell IsNot Nothing Then
            startRow = KadoDataGridView.CurrentCell.RowIndex
            startCol = KadoDataGridView.CurrentCell.ColumnIndex
        Else
            startRow = rowCount - 1
            startCol = colCount - 1
        End If

        Dim sBuzzWord As String = SearchBox.Text
        Dim oRows As DataGridViewRowCollection = KadoDataGridView.Rows
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
                KadoDataGridView.CurrentCell = oCell
                If Not oCell.ReadOnly AndAlso TypeOf oCell Is DataGridViewTextBoxCell Then
                    KadoDataGridView.BeginEdit(True)
                    Dim oTextBox As DataGridViewTextBoxEditingControl = _
                       DirectCast(KadoDataGridView.EditingControl, DataGridViewTextBoxEditingControl)
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
                KadoDataOpenFileDialog.FileName = Path.GetFileName(LastReadFilePath)
                KadoDataOpenFileDialog.InitialDirectory = Path.GetDirectoryName(LastReadFilePath)
            Catch ex As Exception
                KadoDataOpenFileDialog.FileName = ""
            End Try
        Else
            KadoDataOpenFileDialog.FileName = ""
        End If
        If KadoDataOpenFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim recIndex As Long
        Dim oBytes As Byte()
        Try
            Dim oFileInfo As New FileInfo(KadoDataOpenFileDialog.FileName)
            Dim recCount As Integer = CInt(oFileInfo.Length \ RecordLength)
            If recCount < 2 OrElse oFileInfo.Length <> RecordLength * recCount Then
                AlertBox.Show(Lexis.KadoDataFileSizeError)
                Return
            End If

            recCount -= 1
            Dim oRecSelector As New SelectRecordDialog()
            oRecSelector.Description = Lexis.SelectRecordToRead.Gen(recCount)
            oRecSelector.MaxIndex = recCount - 1
            If LastReadFilePath = KadoDataOpenFileDialog.FileName AndAlso LastReadRecordIndex < recCount Then
                oRecSelector.Index = LastReadRecordIndex
            End If
            Using oRecSelector
                If oRecSelector.ShowDialog() <> DialogResult.OK Then Return
                recIndex = CLng(oRecSelector.Index) + 1
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
            AlertBox.Show(Lexis.KadoDataFileReadError, ex.Message)
            Return
        End Try

        SetAllValues(oBytes)

        LastReadFilePath = KadoDataOpenFileDialog.FileName
        LastReadRecordIndex = recIndex - 1

        RepaintKadoDataGridView(SearchBox.Text)
    End Sub

    Private Sub FileRewriteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileRewriteButton.Click
        If LastReadFilePath.Length <> 0 Then
            Try
                KadoDataRewriteFileDialog.FileName = Path.GetFileName(LastReadFilePath)
                KadoDataRewriteFileDialog.InitialDirectory = Path.GetDirectoryName(LastReadFilePath)
            Catch ex As Exception
                KadoDataRewriteFileDialog.FileName = ""
            End Try
        Else
            KadoDataRewriteFileDialog.FileName = ""
        End If
        If KadoDataRewriteFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim oBytes As Byte() = GetAllValues()

        Dim fileInitialLen As Long
        Dim recIndex As Long
        Try
            Dim oFileInfo As New FileInfo(KadoDataRewriteFileDialog.FileName)
            If oFileInfo.Exists Then
                fileInitialLen = oFileInfo.Length
                Dim recCount As Integer = CInt(fileInitialLen \ RecordLength)
                If recCount < 2 OrElse fileInitialLen <> RecordLength * recCount Then
                    AlertBox.Show(Lexis.KadoDataFileSizeError)
                    Return
                End If

                recCount -= 1
                Dim oRecSelector As New SelectRecordDialog()
                oRecSelector.Description = Lexis.SelectRecordToWrite.Gen(recCount)
                oRecSelector.MaxIndex = recCount - 1
                If LastReadFilePath = KadoDataRewriteFileDialog.FileName AndAlso LastReadRecordIndex < recCount Then
                    oRecSelector.Index = LastReadRecordIndex
                End If
                Using oRecSelector
                    If oRecSelector.ShowDialog() <> DialogResult.OK Then Return
                    recIndex = CLng(oRecSelector.Index) + 1
                End Using
            Else
                fileInitialLen = 0
                If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.KadoDataFileCreateReally) = DialogResult.Cancel Then Return
                recIndex = 1
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.KadoDataFileReadError, ex.Message)
            Return
        End Try

        Dim now As DateTime = DateTime.Now
        Try
            Using oOutputStream As New FileStream(KadoDataRewriteFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)
                Dim fileLen As Long = oOutputStream.Length
                If fileLen <> fileInitialLen Then Throw New OPMGException()

                If fileLen < RecordLength Then
                    ExUpboundFileHeader.WriteToStream(&HB7, 1, RecordLength, now, oOutputStream)
                Else
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                    Dim recCount As Integer = CInt((fileLen \ RecordLength) - 1)
                    ExUpboundFileHeader.WriteToStream(&HB7, recCount, RecordLength, now, oOutputStream)
                    oOutputStream.Seek(RecordLength * recIndex, SeekOrigin.Begin)
                End If

                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As OPMGException
            AlertBox.Show(Lexis.KadoDataFileExclusionError)
            Return
        Catch ex As Exception
            AlertBox.Show(Lexis.KadoDataFileWriteError, ex.Message)
            Return
        End Try

        LastReadFilePath = KadoDataRewriteFileDialog.FileName
        LastReadRecordIndex = If(recIndex = 0, 0, recIndex - 1)
    End Sub

    'OPT: FileRewriteButton_Clickで、レコードのインデックスにrecCountを指定できるようにすれば代用可能。
    Private Sub FileAppendButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileAppendButton.Click
        If LastWrittenFilePath.Length <> 0 Then
            Try
                KadoDataAppendFileDialog.FileName = Path.GetFileName(LastWrittenFilePath)
                KadoDataAppendFileDialog.InitialDirectory = Path.GetDirectoryName(LastWrittenFilePath)
            Catch ex As Exception
                KadoDataAppendFileDialog.FileName = ""
            End Try
        Else
            KadoDataAppendFileDialog.FileName = ""
        End If
        If KadoDataAppendFileDialog.ShowDialog() <> DialogResult.OK Then Return

        Dim oBytes As Byte() = GetAllValues()

        Dim fileInitialLen As Long
        Dim recIndex As Long
        Try
            Dim oFileInfo As New FileInfo(KadoDataAppendFileDialog.FileName)
            If oFileInfo.Exists Then
                fileInitialLen = oFileInfo.Length
                Dim recCount As Integer = CInt(fileInitialLen \ RecordLength)
                If recCount < 2 OrElse fileInitialLen <> RecordLength * recCount Then
                    AlertBox.Show(Lexis.KadoDataFileSizeError)
                    Return
                End If

                If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.KadoDataFileAppendReally) = DialogResult.Cancel Then Return
                recIndex = recCount
            Else
                fileInitialLen = 0
                If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.KadoDataFileCreateReally) = DialogResult.Cancel Then Return
                recIndex = 1
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.KadoDataFileReadError, ex.Message)
            Return
        End Try

        Dim now As DateTime = DateTime.Now
        Try
            Using oOutputStream As New FileStream(KadoDataAppendFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)
                Dim fileLen As Long = oOutputStream.Length
                If fileLen <> fileInitialLen Then Throw New OPMGException()

                If fileLen < RecordLength Then
                    ExUpboundFileHeader.WriteToStream(&HB7, 1, RecordLength, now, oOutputStream)
                Else
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                    Dim totalRecCount As Integer = CInt((fileLen \ RecordLength) - 1) + 1
                    ExUpboundFileHeader.WriteToStream(&HB7, totalRecCount, RecordLength, now, oOutputStream)
                    oOutputStream.Seek(0, SeekOrigin.End)
                End If

                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As OPMGException
            AlertBox.Show(Lexis.KadoDataFileExclusionError)
            Return
        Catch ex As Exception
            AlertBox.Show(Lexis.KadoDataFileWriteError, ex.Message)
            Return
        End Try

        LastWrittenFilePath = KadoDataAppendFileDialog.FileName
        LastWrittenRecordIndex = recIndex - 1
    End Sub

    Private Sub ManFileUpdateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ManFileUpdateButton.Click
        Dim oBytes As Byte() = GetAllValues()
        If KeyFieldsAutoAdjustCheckBox.Checked Then
            Dim oTerm As TermMachine = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId)
            KadoDataUtil.SetFieldValueToBytes("基本ヘッダー 処理日時", DateTime.Now.ToString("yyyyMMddHHmmss"), oBytes)
            KadoDataUtil.SetFieldValueToBytes("基本ヘッダー シーケンスNo", MyUtility.GetNextSeqNumber(oTerm.KadoSeqNumber).ToString(), oBytes)
            SetAllValues(oBytes)
        End If

        If ManagerForm.UpdateKadoData(MonitorMachineId, SourceMachineId, oBytes) = True Then
            AlertBox.Show(Lexis.KadoDataStoreFinished)
        Else
            AlertBox.Show(Lexis.KadoDataStoreFailed)
        End If
    End Sub

    Private Sub BaseHeaderSetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BaseHeaderSetButton.Click
        If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.KadoDataBaseHeaderSetReally) = DialogResult.No Then Return

        Dim oBytes As Byte() = GetAllValues()
        Dim oTerm As TermMachine = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId)
        Dim termEkCode As EkCode = MainForm.GetEkCodeOf(SourceMachineId)
        Dim now As DateTime = DateTime.Now
        KadoDataUtil.InitBaseHeaderFields(termEkCode, now, MyUtility.GetNextSeqNumber(oTerm.KadoSeqNumber), oBytes)
        SetAllValues(oBytes)
    End Sub

    Private Sub AllHeadersSetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllHeadersSetButton.Click
        If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.KadoDataAllHeadersSetReally) = DialogResult.No Then Return

        Dim oBytes As Byte() = GetAllValues()
        Dim oTerm As TermMachine = ManagerForm.UiState.Machines(MonitorMachineId).TermMachines(SourceMachineId)
        Dim termEkCode As EkCode = MainForm.GetEkCodeOf(SourceMachineId)
        Dim now As DateTime = DateTime.Now
        KadoDataUtil.InitBaseHeaderFields(termEkCode, now, MyUtility.GetNextSeqNumber(oTerm.KadoSeqNumber), oBytes)
        KadoDataUtil.SetFieldValueToBytes("共通部 集計終了(収集)日時", now.ToString("yyyyMMddHHmmss"), oBytes)
        KadoDataUtil.SetFieldValueToBytes("共通部 改札側搬送部番号", termEkCode.ToString("%3R%3S%2C%2U"), oBytes)
        KadoDataUtil.SetFieldValueToBytes("共通部 集札側搬送部番号", termEkCode.ToString("%3R%3S%2C%2U"), oBytes)
        SetAllValues(oBytes)
    End Sub

    Private Sub SummariesSetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SummariesSetButton.Click
        If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.KadoDataSummariesSetReally) = DialogResult.No Then Return

        Dim oBytes As Byte() = GetAllValues()
        KadoDataUtil.UpdateSummaryFields(oBytes)
        SetAllValues(oBytes)
    End Sub

    Private Function GetValue(ByVal sMetaName As String) As String
        Dim i As Integer = KadoDataUtil.FieldIndexOf(sMetaName)
        Return oTable.Rows(i).Field(Of String)("VALUE")
    End Function

    Private Sub SetValue(ByVal sMetaName As String, ByVal sValue As String)
        Dim i As Integer = KadoDataUtil.FieldIndexOf(sMetaName)
        'TODO: ここにErrorTextの解除を実装するのはみっともないので、
        'XlsDataGridViewにおけるErrorText解除を行うイベントを
        '現状のCellValidatingから変更する。
        'ユーザによる編集か否かに関係なく更新時に必ず発生するイベントを探す。
        KadoDataGridView.Rows(i).Cells(3).ErrorText = ""
        oTable.Rows(i)("VALUE") = sValue
    End Sub

    Private Function GetAllValues() As Byte()
        Dim oBytes As Byte() = New Byte(RecordLength - 1) {}
        Dim bitOffset As Integer = 0
        For i As Integer = 0 To KadoDataUtil.Fields.Length - 1
            Dim oField As XlsField = KadoDataUtil.Fields(i)
            oField.CopyValueToBytes(oTable.Rows(i).Field(Of String)("VALUE"), oBytes, bitOffset)
            bitOffset += oField.ElementBits * oField.ElementCount
        Next i
        Return oBytes
    End Function

    Private Sub SetAllValues(ByVal oBytes As Byte())
        Dim bitOffset As Integer = 0
        For i As Integer = 0 To KadoDataUtil.Fields.Length - 1
            Dim oField As XlsField = KadoDataUtil.Fields(i)
            'TODO: ここにErrorTextの解除を実装するのはみっともないので、
            'XlsDataGridViewにおけるErrorText解除を行うイベントを
            '現状のCellValidatingから変更する。
            'ユーザによる編集か否かに関係なく更新時に必ず発生するイベントを探す。
            KadoDataGridView.Rows(i).Cells(3).ErrorText = ""
            oTable.Rows(i)("VALUE") = oField.CreateValueFromBytes(oBytes, bitOffset)
            bitOffset += oField.ElementBits * oField.ElementCount
        Next i
    End Sub

End Class
