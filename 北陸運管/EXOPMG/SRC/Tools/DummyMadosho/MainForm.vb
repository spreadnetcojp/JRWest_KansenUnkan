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
Imports System.Messaging
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

Public Class MainForm
    Protected OptionalWriter As LogToOptionalDelegate
    Protected oLogDispStorage As DataTable
    Protected oLogDispBinder As BindingSource
    Protected oLogDispFilterEditDialog As LogDispFilterEditDialog = Nothing

    Protected Const MachineDirFormat As String = "%3R%3S_%4C_%2U"
    Protected Const MachineDirPattern As String = "??????_????_??"
    Protected Shared ReadOnly MachineDirRegx As New Regex("^[0-9]{6}_[0-9]{4}_[0-9]{2}$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Public UiState As UiStateClass
    Protected TelegGene As EkTelegramGene
    Protected Table1 As DataTable
    Protected Table2 As DataTable
    Protected Friend WithEvents InputQueue As MessageQueue = Nothing
    Protected Friend FaultDataFormDic As Dictionary(Of String, FaultDataForm)
    Protected Friend KadoDataFormDic As Dictionary(Of String, KadoDataForm)

    Protected Rand As Random

    Protected Shared Function GetMachineProfileTable(ByVal sPath As String) As DataTable
        Dim dt As New DataTable("MachineProfile")
        Dim n As Integer = Config.MachineProfileFieldNames.Length
        For i As Integer = 0 To n - 1
            dt.Columns.Add(Config.MachineProfileFieldNames(i), Config.FieldNamesTypes(Config.MachineProfileFieldNames(i)))
        Next i

        Using sr As StreamReader = New StreamReader(sPath, Encoding.Default)
            Dim line As String = sr.ReadLine()
            Dim lineCount As Integer = 1
            While line IsNot Nothing
                Dim columns As String() = line.Split(","c)
                If columns.Count <> n Then
                    Throw New OPMGException("�@��\����" & lineCount.ToString() & "�s�ڂ̃J���������s���ł��B")
                End If

                dt.Rows.Add(columns)

                line = sr.ReadLine()
                lineCount += 1
            End While
        End Using

        Return dt
    End Function

    Protected Shared Function GetMachineProfile(ByVal oProfileTableRow As DataRow) As Object()
        Dim oProfile As Object() = New Object(Config.MachineProfileFieldNames.Length - 1) {}
        For i As Integer = 0 To Config.MachineProfileFieldNames.Length - 1
            oProfile(i) = oProfileTableRow(i)
        Next i
        Return oProfile
    End Function

    Protected Shared Function GetMachineId(ByVal oProfileTableRow As DataRow) As String
        Dim sId As String = _
           oProfileTableRow.Field(Of String)("MODEL_CODE") & "_" & _
           oProfileTableRow.Field(Of String)("RAIL_SECTION_CODE") & _
           oProfileTableRow.Field(Of String)("STATION_ORDER_CODE") & "_" & _
           oProfileTableRow.Field(Of Integer)("CORNER_CODE").ToString("D4") & "_" & _
           oProfileTableRow.Field(Of Integer)("UNIT_NO").ToString("D2")
        Return sId
    End Function

    Public Shared Function GetMachineId(ByVal sModel As String, ByVal sStation As String, ByVal sCorner As String, ByVal sUnit As String) As String
        Return sModel & "_" & sStation & "_" & sCorner & "_" & sUnit
    End Function

    Public Shared Function GetMachineId(ByVal sMachineDirName As String) As String
        Return Config.ModelSym & "_" & sMachineDirName
    End Function

    Public Shared Function GetMachineDirNameOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(2)
    End Function

    Public Shared Function GetModelOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(0, 1)
    End Function

    Public Shared Function GetHypStationOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(2, 3) & "-" & sMachineId.Substring(5, 3)
    End Function

    Public Shared Function GetStationOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(2, 6)
    End Function

    Public Shared Function GetCornerOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(9, 4)
    End Function

    Public Shared Function GetUnitOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(14, 2)
    End Function

    Public Shared Function GetEkCodeOf(ByVal sMachineId As String) As EkCode
        Return EkCode.Parse(sMachineId.Substring(2), MachineDirFormat)
    End Function

    'NOTE: ���g�p
    Protected Shared Sub DeleteFiles(ByVal sMachineId As String, ByVal sDirPath As String, ByVal sFileNamePattern As String, Optional ByVal oFileNameRegx As Regex = Nothing)
        Try
            For Each sFilePath As String In Directory.GetFiles(sDirPath, sFileNamePattern)
                If oFileNameRegx IsNot Nothing Then
                    Dim sFileName As String = Path.GetFileName(sFilePath)
                    If oFileNameRegx.IsMatch(sFileName) Then
                        File.Delete(sFilePath)
                        Log.Info(sMachineId, "�t�@�C�� [" & sFilePath & "] ���폜���܂����B")
                    End If
                Else
                    File.Delete(sFilePath)
                    Log.Info(sMachineId, "�t�@�C�� [" & sFilePath & "] ���폜���܂����B")
                End If
            Next sFilePath
        Catch ex As Exception
            Log.Error(sMachineId, "Exception caught.", ex)
        End Try
    End Sub

    'NOTE: ���O�o�͖��ɌĂ΂��̂ŁA����̒��Ń��O���o�͂��Ă͂Ȃ�Ȃ��B
    Protected Sub BeginFetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)
        Try
            'OPT: ��L����������̓f�b�h���b�N���Ȃ��Ǝv����̂ŁA
            'BeginInvoke()�ł͂Ȃ��AInvoke()�ł��悢��������Ȃ��B
            'Invoke()�Ȃ�΁A���b�Z�[�W�L���[�����ӂ��S�z���Ȃ��B
            BeginInvoke( _
                OptionalWriter, _
                New Object() {number, sSecondName, sDateTime, sKind, sClassName, sMethodName, sText})
        Catch ex As Exception
            'NOTE: ����Control���j�����ꂽ��ɂ��̃��\�b�h���Ăяo����閜����̏ꍇ��z�肵�Ă���B
            '���̌�́i���̃f���Q�[�g�Ɉˑ����Ȃ��j������ʏ�ʂ�s���悤�A��O�͈���Ԃ��B
        End Try
    End Sub

    'NOTE: ���O�o�͖��ɌĂ΂��̂ŁA����̒��Ń��O���o�͂��Ă͂Ȃ�Ȃ��B
    Protected Sub FetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)

        If LogDispCheckBox.Checked Then
            If oLogDispStorage.Rows.Count > Config.LogDispMaxRowsCount Then
                oLogDispStorage.Rows.Remove(oLogDispStorage.Rows(0))
            End If

            Dim oRow As DataRow = oLogDispStorage.NewRow()
            oRow(0) = sDateTime
            oRow(1) = sSecondName
            oRow(2) = sKind
            oRow(3) = sText
            oLogDispStorage.Rows.Add(oRow)

            Dim nDispRows As Integer = LogDispGrid.Rows.Count
            If nDispRows <> 0 Then
                LogDispGrid.FirstDisplayedScrollingRowIndex = nDispRows - 1
            End If
        End If
    End Sub

    Private Sub LogDispGrid_CellFormatting(sender As System.Object, e As DataGridViewCellFormattingEventArgs) Handles LogDispGrid.CellFormatting
        If e.ColumnIndex = 3 Then
            Dim dgv As DataGridView = DirectCast(sender, DataGridView)
            Dim k As String = DirectCast(dgv.Rows(e.RowIndex).Cells(2).Value, String)
            Select Case k
                Case "[INFO]"
                    e.CellStyle.ForeColor = Color.RoyalBlue
                    e.CellStyle.SelectionForeColor = Color.RoyalBlue
                Case "[WARN]"
                    e.CellStyle.ForeColor = Color.Fuchsia
                    e.CellStyle.SelectionForeColor = Color.Fuchsia
                Case "[ERROR]"
                    e.CellStyle.ForeColor = Color.Red
                    e.CellStyle.SelectionForeColor = Color.Red
                Case "[FATAL]"
                    e.CellStyle.ForeColor = Color.DarkOrange
                    e.CellStyle.SelectionForeColor = Color.DarkOrange
                Case Else
                    e.CellStyle.ForeColor = Color.DarkGray
                    e.CellStyle.SelectionForeColor = Color.DarkGray
            End Select
        End If
    End Sub

    Private Sub LogDispClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogDispClearButton.Click
        oLogDispStorage.Clear()
    End Sub

    Private Sub LogDispFilterEditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogDispFilterEditButton.Click
        If oLogDispFilterEditDialog Is Nothing Then
            oLogDispFilterEditDialog = New LogDispFilterEditDialog(UiState.LogDispFilterHistory, oLogDispStorage)
        End If

        Dim onError As Boolean = False
        Do
            If oLogDispFilterEditDialog.ShowDialog(onError) <> DialogResult.OK Then Return
            Dim sNewFilter As String = oLogDispFilterEditDialog.FilterValue
            Try
                oLogDispBinder.Filter = sNewFilter
                Dim nDispRows As Integer = LogDispGrid.Rows.Count
                If nDispRows <> 0 Then
                    LogDispGrid.FirstDisplayedScrollingRowIndex = nDispRows - 1
                End If
            Catch ex As Exception
                AlertBox.Show(Lexis.LogDispFilterIsInvalid)
                onError = True
                Continue Do
            End Try

            UiState.LogDispFilterHistory.Remove(sNewFilter)
            UiState.LogDispFilterHistory.Insert(0, sNewFilter)
            While UiState.LogDispFilterHistory.Count > Config.LogDispFilterMaxHisCount
                UiState.LogDispFilterHistory.RemoveAt(UiState.LogDispFilterHistory.Count - 1)
            End While

            LogDispFilter.Text = sNewFilter
            Exit Do
        Loop
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)

        OptionalWriter = New LogToOptionalDelegate(AddressOf Me.FetchLog)

        oLogDispStorage = New DataTable()
        'oLogDispStorage.Columns.Add("Time", GetType(DateTime))
        oLogDispStorage.Columns.Add("Time", GetType(String))
        oLogDispStorage.Columns.Add("Source", GetType(String))
        oLogDispStorage.Columns.Add("Kind", GetType(String))
        oLogDispStorage.Columns.Add("Message", GetType(String))

        oLogDispBinder = New BindingSource()
        oLogDispBinder.DataSource = oLogDispStorage

        LogDispGrid.DefaultCellStyle.SelectionForeColor = LogDispGrid.DefaultCellStyle.ForeColor
        LogDispGrid.DefaultCellStyle.SelectionBackColor = Color.LightGray
        LogDispGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        LogDispGrid.AutoGenerateColumns = True
        LogDispGrid.DataSource = oLogDispBinder

        'LogDispGrid.Columns(0).DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss.fff"
        LogDispGrid.Columns(2).Visible = False
        LogDispGrid.Columns(3).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        LogDispGrid.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
        LogDispGrid.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
        LogDispGrid.Columns(3).SortMode = DataGridViewColumnSortMode.NotSortable
        LogDispGrid.Columns(0).Width = MyUtility.GetTextWidth("9999/99/99 99:99:99.999", LogDispGrid.Font)
        LogDispGrid.Columns(1).Width = MyUtility.GetTextWidth("W_999999_9999_99", LogDispGrid.Font)
        LogDispGrid.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
    End Sub

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)

        Log.SetOptionalWriter(New LogToOptionalDelegate(AddressOf Me.BeginFetchLog))

        Dim sWorkingDir As String = System.Environment.CurrentDirectory
        Dim sIniFilePath As String = Path.ChangeExtension(Application.ExecutablePath, ".ini")
        sIniFilePath = Path.Combine(sWorkingDir, Path.GetFileName(sIniFilePath))
        Try
            Lexis.Init(sIniFilePath)
            Config.Init(sIniFilePath)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
            Me.Close()
            Return
        End Try

        Log.SetKindsMask(Config.LogKindsMask)

        LocalConnectionProvider.Init()

        Dim oSerializer As New DataContractSerializer(GetType(UiStateClass))
        Dim sStateFileUri As String = Path.ChangeExtension(Path.GetFileName(Application.ExecutablePath), ".xml")
        sStateFileUri = sStateFileUri.Insert(sStateFileUri.Length - 4, "State")
        Try
            Using xr As XmlReader = XmlReader.Create(sStateFileUri)
                UiState = DirectCast(oSerializer.ReadObject(xr), UiStateClass)
            End Using
        Catch ex As FileNotFoundException
            Log.Info("Initializing UiState...")
            UiState = New UiStateClass()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.UiStateDeserializeFailed)
            Me.Close()
            Return
        End Try

        'Lexis���琶������������Config��UiState�̒l���e�R���g���[���ɔ��f����B

        If Config.ClearLogDispFilterHisOnBoot Then
            UiState.LogDispFilterHistory.Clear()
        End If

        For i As Integer = 0 To Config.LogDispFilterInitialHis.Rows.Count - 1
            Dim s As String = Config.LogDispFilterInitialHis.Rows(i).Field(Of String)("Value")
            UiState.LogDispFilterHistory.Remove(s)
            UiState.LogDispFilterHistory.Insert(i, s)
        Next i

        If UiState.LogDispFilterHistory.Count <> 0 Then
            oLogDispBinder.Filter = UiState.LogDispFilterHistory(0)
            LogDispFilter.Text = UiState.LogDispFilterHistory(0)
            Dim nDispRows As Integer = LogDispGrid.Rows.Count
            If nDispRows <> 0 Then
                LogDispGrid.FirstDisplayedScrollingRowIndex = nDispRows - 1
            End If
        End If

        LogDispGrid.Columns(0).HeaderText = Lexis.LogDispTimeColumnTitle.Gen()
        LogDispGrid.Columns(1).HeaderText = Lexis.LogDispSourceColumnTitle.Gen()
        LogDispGrid.Columns(3).HeaderText = Lexis.LogDispMessageColumnTitle.Gen()

        If Config.LogDispMessageColumnWidth > 0 Then
            LogDispGrid.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            LogDispGrid.Columns(3).Width = Config.LogDispMessageColumnWidth
        End If

        Me.Text = Lexis.FormTitle.Gen()

        TelegGene = New EkTelegramGeneForNativeModels("")

        InitTable1()
        InitTable2WithoutFilter()
        TuneTable2FilterToTable1Selection()
        TableSplitContainer.SplitterDistance _
           = DataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) _
            + SystemInformation.VerticalScrollBarWidth _
            + SystemInformation.BorderSize.Width * 2 _
            + TableSplitContainer.SplitterWidth - 1

        FaultDataFormDic = New Dictionary(Of String, FaultDataForm)
        KadoDataFormDic = New Dictionary(Of String, KadoDataForm)

        Rand = New Random(1)

        Try
            If Not MessageQueue.Exists(Config.SelfMqPath) Then
                InputQueue = MessageQueue.Create(Config.SelfMqPath)
            Else
                InputQueue = New MessageQueue(Config.SelfMqPath)
            End If
            InputQueue.MessageReadPropertyFilter.ClearAll()
            InputQueue.MessageReadPropertyFilter.Body = True
            InputQueue.MessageReadPropertyFilter.Id = True
            InputQueue.MessageReadPropertyFilter.ResponseQueue = True
            InputQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(ExtAppFuncMessageBody)})
            InputQueue.Purge()
            InputQueue.SynchronizingObject = Me
            InputQueue.BeginReceive()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.MessageQueueServiceNotAvailable)
            Me.Close()
            Return
        End Try

        SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
        If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
            Me.Close()
            Return
        End If

        Do
            Dim aDirectoryInfo As DirectoryInfo()
            Try
                Dim oDirInfo As New DirectoryInfo(Path.Combine(SimWorkingDirDialog.SelectedPath, Config.ModelPathInSimWorkingDir))
                aDirectoryInfo = oDirInfo.GetDirectories(MachineDirPattern)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                AlertBox.Show(Lexis.InvalidDirectorySpecified)
                Exit Do
            End Try

            For Each oDirectoryInfo As DirectoryInfo In aDirectoryInfo
                If Not MachineDirRegx.IsMatch(oDirectoryInfo.Name) Then Continue For
                Try
                    FetchMachineProfileFromFile(oDirectoryInfo.FullName)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                End Try
            Next oDirectoryInfo
        Loop While False
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        If oLogDispFilterEditDialog IsNot Nothing Then
            oLogDispFilterEditDialog.Dispose()
            oLogDispFilterEditDialog = Nothing
        End If

        If InputQueue IsNot Nothing Then
            'NOTE: �Ō�ɌĂяo����BeginReceive�ɑΉ�����ReceiveCompleted�C�x���g�́A
            '���L�ɂ��InputQueue.readHandle�̂悤�Ȃ��̂�Close�ɂ���āA
            '����ȍ~�A�������邱�Ƃ͖����Ȃ�z��ł���B
            'InputQueue.SynchronizingObject��Nothing�ɂ��Ă����΂悳�����ɂ�
            '�v���邪�A���̃v���p�e�B�̓X���b�h�Z�[�t�ł͂Ȃ������ł��邽�߁A
            '����BeginReceive���s���Ă��܂��Ă��邱�̎��_�ł͕ύX����ׂ��ł͂Ȃ��B
            InputQueue.Dispose()
        End If

        If Config.SelfMqPath IsNot Nothing Then
            Try
                'NOTE: SelfMqPath����������ꍇ�́AMessageQueue.Exists()��
                '���ۂɓ��Y�p�X�ɃL���[�����݂��Ă��Ă��AFalse��ԋp
                '����悤�ł���B����ASelfMqPath����������ꍇ���A
                '���b�Z�[�W�L���[�T�[�r�X���C���X�g�[������Ă������́A
                'MessageQueue.Create���������Ă��܂��B
                '����āASelfMqPath����������ꍇ�ɁA�N�����ɍ쐬���Ă��܂���
                '�L���[�̍폜�����݂�ɂ́A�����ł�MessageQueue.Exists�ɂ��
                '���f���ȗ����邵���Ȃ��B�Ȃ��A�폜�����݂��Ƃ���ŁA
                'SelfMqPath���������邱�Ƃ𗝗R��MessageQueue.Delete()����
                'MessageQueueException���X���[����A�폜�͎��s���邪�A
                '����MessageQueueErrorCode�v���p�e�B�ɂ���āA�L���[���c����
                '���܂����Ƃ������邽�߁A���̌x�����o�����Ƃ��ł���B
                'If MessageQueue.Exists(Config.SelfMqPath) Then
                '    MessageQueue.Delete(Config.SelfMqPath)
                'End If

                MessageQueue.Delete(Config.SelfMqPath)

            Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.FormatNameBufferTooSmall
                'NOTE: �{���́A�A�v���P�[�V������MessageQueueErrorCode��
                '�Q�Ƃ���ׂ��ł͂Ȃ����AMessageQueue�N���X�̋�����
                '�������邽�߂�ނ𓾂Ȃ��BMessageQueue�N���X�̋�����
                '���P���ꂽ��A�������邱�ƁB
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.MessageQueueDeleteFailed)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If

        If UiState IsNot Nothing Then
            'NOTE: ���̃P�[�X�ł́A�E�ӂ̊e�R���g���[���ɁA���Ȃ��Ƃ��N�����̃t�@�C������
            '���[�h�����l�̓Z�b�g�ς݂̑z��ł���B

            'TODO: �R���g���[���̏�Ԃ�ۑ����邱�Ƃɂ����ꍇ�́A
            '�����Ŋe�R���g���[���̒l��UiState�ɔ��f����B

            Dim oSerializer As New DataContractSerializer(GetType(UiStateClass))
            Dim sStateFileUri As String = Path.ChangeExtension(Path.GetFileName(Application.ExecutablePath), ".xml")
            sStateFileUri = sStateFileUri.Insert(sStateFileUri.Length - 4, "State")
            Try
                Using xw As XmlWriter = XmlWriter.Create(sStateFileUri)
                    oSerializer.WriteObject(xw, UiState)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.UiStateSerializeFailed)
            End Try
        End If

        LocalConnectionProvider.Dispose()

        Log.SetOptionalWriter(Nothing)

        MyBase.OnFormClosed(e)
    End Sub

    Protected Sub AddExtraColumnsToTable2()
        If UpboundProcStateRadioButton.Checked Then
            Table2.Columns.Add("LATCH_CONF", GetType(Byte))
            Table2.Columns.Add("FAULT_SEQ_NO", GetType(UInteger))
            Table2.Columns.Add("FAULT_DATE", GetType(DateTime))
            Table2.Columns.Add("KADO_SEQ_NO", GetType(UInteger))
            Table2.Columns.Add("KADO_DATE", GetType(DateTime))
        End If
    End Sub

    Protected Sub InitExtraComboColumnViewOfTable2(ByVal sName As String, ByVal sHeaderText As String, ByVal sSampleText As String, ByVal sMenuHeaderText As String, ByVal sMenuSampleText As String, ByVal oMenuTable As DataTable)
        Dim oColumns As DataGridViewColumnCollection = DataGridView2.Columns

        Dim oCodeColumn As DataGridViewColumn = oColumns(sName)
        oCodeColumn.HeaderText = sHeaderText
        oCodeColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        oCodeColumn.Width = MyUtility.GetTextWidth(sSampleText, DataGridView2.Font)

        Dim oMenuColumn As New DataGridViewComboBoxColumn()
        oMenuColumn.DataPropertyName = sName
        oMenuColumn.Name = sName & "_MENU"
        oMenuColumn.DataSource = oMenuTable
        oMenuColumn.ValueMember = "Key"
        oMenuColumn.DisplayMember = "Value"
        oMenuColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        oMenuColumn.FlatStyle = FlatStyle.Flat
        oMenuColumn.HeaderText = sMenuHeaderText
        oMenuColumn.Width = MyUtility.GetTextWidth(sMenuSampleText, DataGridView2.Font)
        oColumns.Insert(oCodeColumn.Index + 1, oMenuColumn)
    End Sub

    Protected Sub InitExtraColumnsViewOfTable2()
        If UpboundProcStateRadioButton.Checked Then
            InitExtraComboColumnViewOfTable2("LATCH_CONF", "���b�`�`��(X)", "FF..", "���b�`�`��", "���b�`�O�o�D...��", Config.MenuTableOfLatchConf)
            DataGridView2.Columns("LATCH_CONF").Frozen = True
            DataGridView2.Columns("LATCH_CONF_MENU").Frozen = True

            DataGridView2.Columns("FAULT_SEQ_NO").ReadOnly = True
            DataGridView2.Columns("FAULT_SEQ_NO").HeaderText = "�ŏI�ُ�SEQ.No"
            DataGridView2.Columns("FAULT_SEQ_NO").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("FAULT_SEQ_NO").Width = MyUtility.GetTextWidth("�ŏI�ُ�SEQ.No..", DataGridView2.Font)
            DataGridView2.Columns("FAULT_DATE").ReadOnly = True
            DataGridView2.Columns("FAULT_DATE").HeaderText = "�ŏI�ُ폈������"
            'DataGridView2.Columns("FAULT_DATE").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("FAULT_DATE").Width = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)

            DataGridView2.Columns("KADO_SEQ_NO").ReadOnly = True
            DataGridView2.Columns("KADO_SEQ_NO").HeaderText = "�ŏI�ғ�SEQ.No"
            DataGridView2.Columns("KADO_SEQ_NO").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("KADO_SEQ_NO").Width = MyUtility.GetTextWidth("�ŏI�ғ�SEQ.No..", DataGridView2.Font)
            DataGridView2.Columns("KADO_DATE").ReadOnly = True
            DataGridView2.Columns("KADO_DATE").HeaderText = "�ŏI�ғ���������"
            'DataGridView2.Columns("KADO_DATE").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("KADO_DATE").Width = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)
        End If
    End Sub

    Protected Function GetMonitorMachineRowCountForTable2(ByVal oMachine As Machine) As Integer
        If UpboundProcStateRadioButton.Checked Then
            Return 0
        End If

        Return 0
    End Function

    Protected Sub SetProfileValueToTable2MonitorMachineRow(ByVal oTargetRow As DataRow, ByVal sMachineId As String, ByVal oMachine As Machine)
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            If sFieldName = "MACHINE_ID" Then
                oTargetRow(sFieldName) = sMachineId
            ElseIf sFieldName = "TERM_MACHINE_ID" Then
                oTargetRow(sFieldName) = ""
            Else
                oTargetRow(sFieldName) = oMachine.Profile(Config.MachineProfileFieldNamesIndices(sFieldName))
            End If
        Next i
    End Sub

    Protected Sub SetExtraValueToTable2MonitorMachineRow(ByVal oTargetRow As DataRow, ByVal oMachine As Machine, ByVal index As Integer, ByVal count As Integer)
    End Sub

    Protected Function GetTermMachineRowCountForTable2(ByVal oMachine As TermMachine) As Integer
        If UpboundProcStateRadioButton.Checked Then
            Return 1
        End If

        Return 0
    End Function

    Protected Sub SetProfileValueToTable2TermMachineRow(ByVal oTargetRow As DataRow, ByVal sMonitorMachineId As String, ByVal sTermMachineId As String, ByVal oTermMachine As TermMachine)
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            If sFieldName = "MACHINE_ID" Then
                oTargetRow(sFieldName) = sMonitorMachineId
            ElseIf sFieldName = "TERM_MACHINE_ID" Then
                oTargetRow(sFieldName) = sTermMachineId
            Else
                oTargetRow(sFieldName) = oTermMachine.Profile(Config.MachineProfileFieldNamesIndices(sFieldName))
            End If
        Next i
    End Sub

    Protected Sub SetExtraValueToTable2TermMachineRow(ByVal oTargetRow As DataRow, ByVal oMachine As TermMachine, ByVal index As Integer, ByVal count As Integer)
        If UpboundProcStateRadioButton.Checked Then
            oTargetRow("LATCH_CONF") = oMachine.LatchConf
            oTargetRow("FAULT_SEQ_NO") = oMachine.FaultSeqNumber
            oTargetRow("FAULT_DATE") = oMachine.FaultDate
            oTargetRow("KADO_SEQ_NO") = oMachine.KadoSeqNumber
            oTargetRow("KADO_DATE") = oMachine.KadoDate
        End If
    End Sub

    Protected Sub FetchStateFromTable2Row(ByVal oRow As DataRow)
        Dim sMachineId As String = oRow.Field(Of String)("MACHINE_ID")
        Dim oMachine As Machine = UiState.Machines(sMachineId)

        If UpboundProcStateRadioButton.Checked Then
            Dim sTermMachineId As String = oRow.Field(Of String)("TERM_MACHINE_ID")
            Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
            oTerm.LatchConf = oRow.Field(Of Byte)("LATCH_CONF")
        End If
    End Sub

    Protected Sub InitTable1()
        '��̃f�[�^�e�[�u�����쐬���A�t�B�[���h����ݒ肷��B
        Table1 = New DataTable()
        For i As Integer = 0 To Config.Table1FieldNames.Length - 1
            Dim sFieldName As String = Config.Table1FieldNames(i)
            Table1.Columns.Add(sFieldName, Config.FieldNamesTypes(sFieldName))
        Next i

        'UiState.Machines�̊�{�����f�[�^�e�[�u���ɓW�J����B
        For Each oMachineEntry As KeyValuePair(Of String, Machine) In UiState.Machines
            Dim oRow As DataRow = Table1.NewRow()
            For i As Integer = 0 To Config.Table1FieldNames.Length - 1
                Dim sFieldName As String = Config.Table1FieldNames(i)
                If sFieldName = "MACHINE_ID" Then
                    oRow(sFieldName) = oMachineEntry.Key
                ElseIf sFieldName = "LAST_CONFIRMED" Then
                    oRow(sFieldName) = oMachineEntry.Value.LastConfirmed
                Else
                    oRow(sFieldName) = oMachineEntry.Value.Profile(Config.MachineProfileFieldNamesIndices(sFieldName))
                End If
            Next i
            Table1.Rows.Add(oRow)
        Next oMachineEntry

        Dim visibleFieldKind As Integer = If(SymbolizeCheckBox.Checked, 2, 1)

        Dim oDataView As DataView = New DataView(Table1)
        oDataView.Sort = "MACHINE_ID ASC"

        DataGridView1.SuspendLayout()
        DataGridView1.Columns.Clear()
        DataGridView1.AutoGenerateColumns = True
        DataGridView1.DataSource = oDataView
        For i As Integer = 0 To Config.Table1FieldNames.Length - 1
            Dim sFieldName As String = Config.Table1FieldNames(i)
            If Array.IndexOf(Config.Table1VisibleFieldNames, sFieldName) <> -1 Then
                DataGridView1.Columns(i).HeaderText = Config.Table1VisibleFieldNamesTitles(sFieldName)
                DataGridView1.Columns(i).Width = MyUtility.GetTextWidth(Config.Table1VisibleFieldNamesCanonicalValues(sFieldName), DataGridView1.Font)
                DataGridView1.Columns(i).ReadOnly = True
                DataGridView1.Columns(i).Visible = ((Config.Table1VisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
                If Config.FieldNamesTypes(sFieldName) Is GetType(DateTime) Then
                    DataGridView1.Columns(i).DefaultCellStyle.Format = Config.DateTimeFormatInGui
                ElseIf Config.FieldNamesTypes(sFieldName) IsNot GetType(String) Then
                    DataGridView1.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Else
                DataGridView1.Columns(i).Visible = False
            End If
        Next i
        DataGridView1.ResumeLayout()
    End Sub

    Protected Sub InitTable2WithoutFilter()
        '��̃f�[�^�e�[�u�����쐬���A�t�B�[���h����ݒ肷��B
        Table2 = New DataTable()
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            Table2.Columns.Add(sFieldName, Config.FieldNamesTypes(sFieldName))
        Next i
        AddExtraColumnsToTable2()

        'UiState.Machines�̊�{�����f�[�^�e�[�u���ɓW�J����B
        For Each oMachineEntry As KeyValuePair(Of String, Machine) In UiState.Machines
            With Nothing
                Dim n As Integer = GetMonitorMachineRowCountForTable2(oMachineEntry.Value)
                For index As Integer = 0 To n - 1
                    Dim oRow As DataRow = Table2.NewRow()
                    SetProfileValueToTable2MonitorMachineRow(oRow, oMachineEntry.Key, oMachineEntry.Value)
                    SetExtraValueToTable2MonitorMachineRow(oRow, oMachineEntry.Value, index, n)
                    Table2.Rows.Add(oRow)
                Next index
            End With

            For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMachineEntry.Value.TermMachines
                Dim n As Integer = GetTermMachineRowCountForTable2(oTermEntry.Value)
                For index As Integer = 0 To n - 1
                    Dim oRow As DataRow = Table2.NewRow()
                    SetProfileValueToTable2TermMachineRow(oRow, oMachineEntry.Key, oTermEntry.Key, oTermEntry.Value)
                    SetExtraValueToTable2TermMachineRow(oRow, oTermEntry.Value, index, n)
                    Table2.Rows.Add(oRow)
                Next index
            Next oTermEntry
        Next oMachineEntry

        Dim visibleFieldKind As Integer = If(SymbolizeCheckBox.Checked, 2, 1)

        Dim oDataView As DataView = New DataView(Table2)
        oDataView.Sort = "MACHINE_ID ASC, TERM_MACHINE_ID ASC"

        DataGridView2.SuspendLayout()
        DataGridView2.Columns.Clear()
        DataGridView2.AutoGenerateColumns = True
        DataGridView2.DataSource = oDataView
        DataGridView2.AutoGenerateColumns = False
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            If Array.IndexOf(Config.Table2VisibleFieldNames, sFieldName) <> -1 Then
                DataGridView2.Columns(i).HeaderText = Config.Table2VisibleFieldNamesTitles(sFieldName)
                DataGridView2.Columns(i).Width = MyUtility.GetTextWidth(Config.Table2VisibleFieldNamesCanonicalValues(sFieldName), DataGridView2.Font)
                DataGridView2.Columns(i).ReadOnly = True
                DataGridView2.Columns(i).Frozen = True
                DataGridView2.Columns(i).Visible = ((Config.Table2VisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
                If Config.FieldNamesTypes(sFieldName) Is GetType(DateTime) Then
                    'DataGridView2.Columns(i).DefaultCellStyle.Format = Config.DateTimeFormatInGui
                ElseIf Config.FieldNamesTypes(sFieldName) IsNot GetType(String) Then
                    DataGridView2.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Else
                DataGridView2.Columns(i).Visible = False
                DataGridView2.Columns(i).Frozen = True
            End If
        Next i
        InitExtraColumnsViewOfTable2()
        DataGridView2.ResumeLayout()
    End Sub

    Protected Sub TuneTable2FilterToTable1Selection()
        Dim oBuilder As New StringBuilder()
        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                If oBuilder.Length <> 0 Then
                    oBuilder.Append(" Or ")
                End If
                oBuilder.Append("MACHINE_ID = '" & DirectCast(gridRow.Cells(idxColumn).Value, String) & "'")
            End If
        Next gridRow
        DirectCast(DataGridView2.DataSource, DataView).RowFilter = oBuilder.ToString()
        DataGridView2.Columns("MACHINE_ID").Visible = False
    End Sub

    Protected Sub UpdateTable2OnMonitorStateChanged(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine)
        Dim nextRowCount As Integer = GetMonitorMachineRowCountForTable2(oMonitorMachine)
        Dim oCurRows As DataRow() = Table2.Select("MACHINE_ID = '" & sMonitorMachineId & "' AND TERM_MACHINE_ID = ''")

        DataGridView2.SuspendLayout()
        If oCurRows.Length >= nextRowCount Then
            For index As Integer = 0 To nextRowCount - 1
                SetExtraValueToTable2MonitorMachineRow(oCurRows(index), oMonitorMachine, index, nextRowCount)
            Next index
            For index As Integer = nextRowCount To oCurRows.Length - 1
                oCurRows(index).Delete()
            Next index
        Else
            For index As Integer = 0 To oCurRows.Length - 1
                SetExtraValueToTable2MonitorMachineRow(oCurRows(index), oMonitorMachine, index, nextRowCount)
            Next index
            For index As Integer = oCurRows.Length To nextRowCount - 1
                Dim oRow As DataRow = Table2.NewRow()
                SetProfileValueToTable2MonitorMachineRow(oRow, sMonitorMachineId, oMonitorMachine)
                SetExtraValueToTable2MonitorMachineRow(oRow, oMonitorMachine, index, nextRowCount)
                Table2.Rows.Add(oRow)
            Next index
        End If
        DataGridView2.ResumeLayout()
    End Sub

    Protected Sub UpdateTable2OnTermStateChanged(ByVal sMonitorMachineId As String, ByVal sTermMachineId As String, ByVal oTermMachine As TermMachine)
        Dim nextRowCount As Integer = GetTermMachineRowCountForTable2(oTermMachine)
        Dim oCurRows As DataRow() = Table2.Select("TERM_MACHINE_ID = '" & sTermMachineId & "'")

        DataGridView2.SuspendLayout()
        If oCurRows.Length >= nextRowCount Then
            For index As Integer = 0 To nextRowCount - 1
                SetExtraValueToTable2TermMachineRow(oCurRows(index), oTermMachine, index, nextRowCount)
            Next index
            For index As Integer = nextRowCount To oCurRows.Length - 1
                oCurRows(index).Delete()
            Next index
        Else
            For index As Integer = 0 To oCurRows.Length - 1
                SetExtraValueToTable2TermMachineRow(oCurRows(index), oTermMachine, index, nextRowCount)
            Next index
            For index As Integer = oCurRows.Length To nextRowCount - 1
                Dim oRow As DataRow = Table2.NewRow()
                SetProfileValueToTable2TermMachineRow(oRow, sMonitorMachineId, sTermMachineId, oTermMachine)
                SetExtraValueToTable2TermMachineRow(oRow, oTermMachine, index, nextRowCount)
                Table2.Rows.Add(oRow)
            Next index
        End If
        DataGridView2.ResumeLayout()
    End Sub

    Protected Sub FetchMachineProfileFromFile(ByVal sMachineDirPath As String)
        Dim sMachineDirName As String = Path.GetFileName(sMachineDirPath)
        Dim sMachineId As String = GetMachineId(sMachineDirName)
        Dim oMachine As Machine = Nothing
        Dim oTable1Row As DataRow
        Dim d As DateTime = DateTime.Now
        Dim newMachineDetected As Boolean = False

        If UiState.Machines.TryGetValue(sMachineId, oMachine) = True Then
            oTable1Row = Table1.Select("MACHINE_ID = '" & sMachineId & "'")(0)
        Else
            Log.Info("�V�����Ď��@�� [" & sMachineId & "] �����o���܂����B")
            oMachine = New Machine()
            oTable1Row = Table1.NewRow()
            oTable1Row("MACHINE_ID") = sMachineId
            newMachineDetected = True
        End If

        oMachine.LastConfirmed = d
        oTable1Row("LAST_CONFIRMED") = oMachine.LastConfirmed

        With Nothing
            Dim sFile As String = Path.Combine(sMachineDirPath, "#Machine.csv")
            Dim t As DateTime = File.GetLastWriteTime(sFile)
            If oMachine.Profile Is Nothing OrElse t <> oMachine.ProfileTimestamp Then
                Dim oProfileTable As DataTable = GetMachineProfileTable(sFile)
                If oProfileTable.Rows.Count <> 1 Then
                    Throw New OPMGException("�@��\���t�@�C���̍s�����ُ�ł��B")
                End If
                If GetMachineId(oProfileTable.Rows(0)) <> sMachineId Then
                    Throw New OPMGException("�@��\���t�@�C���̓��e�ƍ�ƃf�B���N�g�����ɕs����������܂��B")
                End If
                oMachine.Profile = GetMachineProfile(oProfileTable.Rows(0))
                oMachine.ProfileTimestamp = t
                For i As Integer = 0 To Config.Table1FieldNames.Length - 1
                    Dim sFieldName As String = Config.Table1FieldNames(i)
                    If sFieldName <> "MACHINE_ID" AndAlso _
                       sFieldName <> "LAST_CONFIRMED" Then
                        oTable1Row(sFieldName) = oMachine.Profile(Config.MachineProfileFieldNamesIndices(sFieldName))
                    End If
                Next i
            End If
        End With

        If newMachineDetected Then
            Try
                InitMonitorUpboundData(sMachineId, oMachine)
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        With Nothing
            'OPT: DummyKanshiban2�Ƃ̍��ق��ŏ����ɂ��邽�߂Ɏc���Ă��邪�A#Machine.csv���ēx�ǂޕK�v�͂Ȃ��B
            Dim sFile As String = Path.Combine(sMachineDirPath, "#Machine.csv")
            Dim t As DateTime = File.GetLastWriteTime(sFile)
            If oMachine.TermMachines Is Nothing OrElse t <> oMachine.TermMachinesProfileTimestamp Then
                Dim oProfileTable As DataTable = GetMachineProfileTable(sFile)
                Dim oTermMachines As New Dictionary(Of String, TermMachine)(oProfileTable.Rows.Count)

                Dim residueCount As Integer = 0
                For Each oProfileRow As DataRow In oProfileTable.Rows
                    Dim sTermId As String = GetMachineId(oProfileRow)
                    Dim oTerm As TermMachine = Nothing
                    If oMachine.TermMachines.TryGetValue(sTermId, oTerm) = True Then
                        oTerm.Profile = GetMachineProfile(oProfileRow)
                        oTermMachines.Add(sTermId, oTerm)
                        residueCount += 1
                        Dim oTable2Rows As DataRow() = Table2.Select("TERM_MACHINE_ID = '" & sTermId & "'")
                        For Each oRow As DataRow In oTable2Rows
                            SetProfileValueToTable2TermMachineRow(oRow, sMachineId, sTermId, oTerm)
                        Next oRow
                    Else
                        Log.Info("�V�����[���@�� [" & sTermId & "] �����o���܂����B")
                        oTerm = New TermMachine()
                        oTerm.Profile = GetMachineProfile(oProfileRow)
                        Dim sCornerName As String = oProfileRow.Field(Of String)("CORNER_NAME")
                        If sCornerName.Contains("�݂ǂ�") Then
                            If sCornerName.Contains("�抷") Then
                                oTerm.LatchConf = CByte(&H5)
                            Else
                                oTerm.LatchConf = CByte(&H4)
                            End If
                        ElseIf sCornerName.Contains("���Z") Then
                            oTerm.LatchConf = CByte(&H3)
                        ElseIf sCornerName.Contains("�抷") Then
                            oTerm.LatchConf = CByte(&H2)
                        ElseIf sCornerName.Contains("�ݗ�") OrElse sCornerName.Contains("����") Then
                            oTerm.LatchConf = CByte(&H4)  'TODO: �����������ł��邩�s���B
                        Else
                            oTerm.LatchConf = CByte(&H1)
                        End If
                        Try
                            InitTermUpboundData(sMachineId, sTermId, oTerm)
                        Catch ex As Exception
                            Log.Fatal("Unwelcome Exception caught.", ex)
                        End Try

                        oTermMachines.Add(sTermId, oTerm)
                        Dim n As Integer = GetTermMachineRowCountForTable2(oTerm)
                        For index As Integer = 0 To n - 1
                            Dim oRow As DataRow = Table2.NewRow()
                            SetProfileValueToTable2TermMachineRow(oRow, sMachineId, sTermId, oTerm)
                            SetExtraValueToTable2TermMachineRow(oRow, oTerm, index, n)
                            Table2.Rows.Add(oRow)
                        Next index
                    End If
                Next oProfileRow

                If residueCount < oMachine.TermMachines.Count Then
                    For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMachine.TermMachines
                        If Not oTermMachines.ContainsKey(oTermEntry.Key) Then
                            Log.Warn("�폜���ꂽ�[�� [" & oTermEntry.Key & "] �̏�ԏ����N���A���܂��B")
                            Dim oRows As DataRow() = Table2.Select("TERM_MACHINE_ID = '" & oTermEntry.Key & "'")
                            For Each oRow As DataRow In oRows
                                oRow.Delete()
                            Next oRow
                        End If
                    Next oTermEntry
                End If

                oMachine.TermMachines = oTermMachines
                oMachine.TermMachinesProfileTimestamp = t
            End If
        End With

        'NOTE: �[���@��̍\�����ω����Ă��ATable2�̊Ď��@��̍s�ɁA�֘A���鍀�ڂ͖����z��ł���B
        '����āATable2�̊Ď��@��̍s�ɂ��ẮA�Ď��@�펩�̂��ǉ����ꂽ�P�[�X�ł̂݁A�P�A����B
        If newMachineDetected Then
            UiState.Machines.Add(sMachineId, oMachine)
            Table1.Rows.Add(oTable1Row)
            Dim n As Integer = GetMonitorMachineRowCountForTable2(oMachine)
            For index As Integer = 0 To n - 1
                Dim oRow As DataRow = Table2.NewRow()
                SetProfileValueToTable2MonitorMachineRow(oRow, sMachineId, oMachine)
                SetExtraValueToTable2MonitorMachineRow(oRow, oMachine, index, n)
                Table2.Rows.Add(oRow)
            Next index
        End If
    End Sub

    Protected Sub InitMonitorUpboundData(ByVal sMonitorMachineId As String, ByVal oMonitor As Machine)
        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))

        With Nothing
            Dim sFileName As String = "#FaultDataForPassiveUll.dat"
            Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
            File.Delete(sFilePath)
        End With

        With Nothing
            Dim sFileName As String = "#KadoData.dat"
            Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
            File.Delete(sFilePath)
        End With

        For Each oTerm As TermMachine In oMonitor.TermMachines.Values
            oTerm.KadoSlot = 0
        Next oTerm
    End Sub

    Protected Sub InitTermUpboundData(ByVal sMonitorMachineId As String, ByVal sTermMachineId As String, ByVal oTerm As TermMachine)
        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))

        oTerm.FaultSeqNumber = 0UI
        oTerm.FaultDate = Config.EmptyTime

        oTerm.KadoSeqNumber = 0UI
        oTerm.KadoDate = Config.EmptyTime

        '�ғ��f�[�^�Ǘ��t�@�C���̓��Y���R�[�h������������B
        Dim now As DateTime = DateTime.Now
        Dim sFileName As String = "#KadoData.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Dim recLen As Integer = KadoDataUtil.RecordLengthInBytes
        Dim oBytes As Byte() = New Byte(recLen - 1) {}
        Dim termEkCode As EkCode = GetEkCodeOf(sTermMachineId)
        KadoDataUtil.InitBaseHeaderFields(termEkCode, now, 0UI, oBytes)
        KadoDataUtil.InitCommonPartFields(termEkCode, now, oBytes)

        Using oOutputStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
            Dim fileLen As Long = oOutputStream.Length
            Dim slotCount As Integer = If(fileLen < recLen, 1, CInt(fileLen \ recLen))

            If oTerm.KadoSlot = 0 Then
                oTerm.KadoSlot = slotCount
                slotCount += 1
            End If

            oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
            ExUpboundFileHeader.WriteToStream(&HB7, slotCount - 1, recLen, now, oOutputStream)

            oOutputStream.Seek(recLen * oTerm.KadoSlot, SeekOrigin.Begin)
            oOutputStream.Write(oBytes, 0, oBytes.Length)
        End Using
    End Sub

    Private Sub ViewModeRadioButtons_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpboundProcStateRadioButton.CheckedChanged
        If DirectCast(sender, RadioButton).Checked AndAlso Table2 IsNot Nothing Then
            Dim sFilter As String = DirectCast(DataGridView2.DataSource, DataView).RowFilter
            InitTable2WithoutFilter()
            DirectCast(DataGridView2.DataSource, DataView).RowFilter = sFilter
        End If
    End Sub

    Private Sub SymbolizeCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SymbolizeCheckBox.CheckedChanged
        Dim visibleFieldKind As Integer = If(SymbolizeCheckBox.Checked, 2, 1)

        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()

        DataGridView1.SuspendLayout()
        For i As Integer = 0 To Config.Table1FieldNames.Length - 1
            Dim sFieldName As String = Config.Table1FieldNames(i)
            If Array.IndexOf(Config.Table1VisibleFieldNames, sFieldName) <> -1 Then
                DataGridView1.Columns(i).Visible = ((Config.Table1VisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
            Else
                DataGridView1.Columns(i).Visible = False
            End If
        Next i
        DataGridView1.ResumeLayout()

        DataGridView2.SuspendLayout()
        Dim curRow As Integer = -1
        Dim curCol As Integer = -1
        If DataGridView2.CurrentCell IsNot Nothing Then
            curRow = DataGridView2.CurrentCell.RowIndex
            curCol = DataGridView2.CurrentCell.ColumnIndex
        End If
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            If Array.IndexOf(Config.Table2VisibleFieldNames, sFieldName) <> -1 Then
                DataGridView2.Columns(i).Visible = ((Config.Table2VisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
            Else
                DataGridView2.Columns(i).Visible = False
            End If
        Next i
        If curCol <> -1 AndAlso Not DataGridView2.Columns(curCol).Visible Then
            If visibleFieldKind = 1 Then
                Do
                    curCol -= 1
                    If DataGridView2.Columns(curCol).Visible Then
                        DataGridView2.CurrentCell = DataGridView2.Rows(curRow).Cells(curCol)
                        Exit Do
                    End If
                Loop While curCol > 0
            ElseIf visibleFieldKind = 2 Then
                Do
                    curCol += 1
                    If DataGridView2.Columns(curCol).Visible Then
                        DataGridView2.CurrentCell = DataGridView2.Rows(curRow).Cells(curCol)
                        Exit Do
                    End If
                Loop While curCol < Config.Table2FieldNames.Length
            End If
        End If
        DataGridView2.ResumeLayout()

        TableSplitContainer.SplitterDistance _
           = DataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) _
            + SystemInformation.VerticalScrollBarWidth _
            + SystemInformation.BorderSize.Width * 2 _
            + TableSplitContainer.SplitterWidth - 1

        SplitContainer1.Panel1.ResumeLayout()
        SplitContainer1.Panel2.ResumeLayout()
        SplitContainer1.ResumeLayout()
    End Sub

    Private Sub DataGridView1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGridView1.SelectionChanged
        If Table2 IsNot Nothing Then
            TuneTable2FilterToTable1Selection()
        End If
    End Sub

    Private Sub DataGridView1_UserDeletingRow(ByVal sender As System.Object, ByVal e As DataGridViewRowCancelEventArgs) Handles DataGridView1.UserDeletingRow
        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                UiState.Machines.Remove(sMachineId)
                Dim oRows As DataRow() = Table2.Select("MACHINE_ID = '" & sMachineId & "'")
                For Each oRow As DataRow In oRows
                    oRow.Delete()
                Next oRow
            End If
        Next gridRow
    End Sub

    Private Sub DataGridView1_UserDeletedRow(ByVal sender As System.Object, ByVal e As DataGridViewRowEventArgs) Handles DataGridView1.UserDeletedRow
        Table1.AcceptChanges()
    End Sub

    Private Sub DataGridView2_CellFormatting(ByVal sender As System.Object, ByVal e As DataGridViewCellFormattingEventArgs) Handles DataGridView2.CellFormatting
        If TypeOf DataGridView2.Columns(e.ColumnIndex) Is DataGridViewTextBoxColumn Then
            Dim oType As Type = e.Value.GetType()
            If oType Is GetType(Byte) Then
                e.Value = DirectCast(e.Value, Byte).ToString("X2")
                e.FormattingApplied = True
            ElseIf oType Is GetType(DateTime) Then
                Dim t As DateTime = DirectCast(e.Value, DateTime)
                If t.Equals(Config.EmptyTime) Then
                    e.Value = Lexis.EmptyTime.Gen()
                ElseIf t.Equals(Config.UnknownTime) Then
                    e.Value = Lexis.UnknownTime.Gen()
                Else
                    e.Value = t.ToString(Config.DateTimeFormatInGui)
                End If
                e.FormattingApplied = True
            End If
        End If
    End Sub

    Private Sub DataGridView2_CellParsing(ByVal sender As System.Object, ByVal e As DataGridViewCellParsingEventArgs) Handles DataGridView2.CellParsing
        If TypeOf DataGridView2.Columns(e.ColumnIndex) Is DataGridViewTextBoxColumn Then
            If e.DesiredType Is GetType(Byte) Then
                Try
                    e.Value = Byte.Parse(DirectCast(e.Value, String), NumberStyles.HexNumber)
                    e.ParsingApplied = True
                Catch ex As Exception
                    e.ParsingApplied = False
                End Try
            End If
        End If
    End Sub

    Private Sub DataGridView2_CellValueChanged(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles DataGridView2.CellValueChanged
        If e.RowIndex < 0 Then Return
        Dim oView As DataRowView = DirectCast(DataGridView2.Rows(e.RowIndex).DataBoundItem, DataRowView)
        oView.Row.AcceptChanges()
        FetchStateFromTable2Row(oView.Row)
    End Sub

    Private Sub DataGridView2_CellDoubleClick(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles DataGridView2.CellDoubleClick
        If e.RowIndex < 0 Then Return
        If Not DataGridView2.Columns(e.ColumnIndex).ReadOnly Then Return

        If UpboundProcStateRadioButton.Checked Then
            Dim oView As DataRowView = DirectCast(DataGridView2.Rows(e.RowIndex).DataBoundItem, DataRowView)
            Dim oRow As DataRow = oView.Row
            Dim sMonitorMachineId As String = oRow.Field(Of String)("MACHINE_ID")
            Dim sTermMachineId As String = oRow.Field(Of String)("TERM_MACHINE_ID")
            Dim sColName As String = DataGridView2.Columns(e.ColumnIndex).DataPropertyName
            Select Case sColName
                Case "FAULT_SEQ_NO", "FAULT_DATE"
                    Dim oForm As FaultDataForm = Nothing
                    If FaultDataFormDic.TryGetValue(sMonitorMachineId & sTermMachineId, oForm) = True Then
                        oForm.Activate()
                    Else
                        oForm = New FaultDataForm(sMonitorMachineId, sTermMachineId, Me)
                        FaultDataFormDic.Add(sMonitorMachineId & sTermMachineId, oForm)
                        oForm.Show()
                    End If
                Case "KADO_SEQ_NO", "KADO_DATE"
                    Dim oForm As KadoDataForm = Nothing
                    If KadoDataFormDic.TryGetValue(sMonitorMachineId & sTermMachineId, oForm) = True Then
                        oForm.Activate()
                    Else
                        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
                            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
                            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                                Return
                            End If
                        End If

                        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
                        Dim sMonitorMachineDir As String
                        Try
                            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
                            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                            If Not Directory.Exists(sMonitorMachineDir) Then
                                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                                Return
                            End If
                        Catch ex As Exception
                            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
                            Return
                        End Try

                        Dim sFileName As String = "#KadoData.dat"
                        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)

                        oForm = New KadoDataForm(sMonitorMachineId, sTermMachineId, sFilePath, Me)
                        KadoDataFormDic.Add(sMonitorMachineId & sTermMachineId, oForm)
                        oForm.Show()
                    End If
           End Select
        End If
    End Sub

    Private Sub MachineProfileFetchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MachineProfileFetchButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
        End If

        If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        Dim aDirectoryInfo As DirectoryInfo()
        Try
            Dim oDirInfo As New DirectoryInfo(Path.Combine(SimWorkingDirDialog.SelectedPath, Config.ModelPathInSimWorkingDir))
            aDirectoryInfo = oDirInfo.GetDirectories(MachineDirPattern)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            AlertBox.Show(Lexis.InvalidDirectorySpecified)
            Return
        End Try

        For Each oDirectoryInfo As DirectoryInfo In aDirectoryInfo
            If Not MachineDirRegx.IsMatch(oDirectoryInfo.Name) Then Continue For
            Try
                FetchMachineProfileFromFile(oDirectoryInfo.FullName)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        Next oDirectoryInfo

        AlertBox.Show(Lexis.MachineProfileFetchFinished)
    End Sub

    Private Sub UpboundDataClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpboundDataClearButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                ClearUpboundData(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub RandFaultDataStoreButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RandFaultDataStoreButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                StoreRandFaultData(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub RandFaultDataSendButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RandFaultDataSendButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                SendRandFaultData(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub KadoDataRandUpdateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KadoDataRandUpdateButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                UpdateKadoDataRandomly(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub KadoDataCommitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KadoDataCommitButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                CommitKadoData(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub InputQueue_ReceiveCompleted(ByVal sender As System.Object, ByVal e As System.Messaging.ReceiveCompletedEventArgs) Handles InputQueue.ReceiveCompleted
        Dim sTargetMachineId As String = Nothing
        Dim isProcCompleted As Boolean = False
        Dim sResult As String = Nothing
        Try
            Dim bd As ExtAppFuncMessageBody = DirectCast(e.Message.Body, ExtAppFuncMessageBody)
            Dim sContextDir As String = bd.WorkingDirectory

            If String.IsNullOrEmpty(sContextDir) OrElse String.IsNullOrEmpty(bd.Func) Then
                Log.Error("�s���ȗv������M���܂����B" & vbCrLf & "WorkingDirectory: " & If(sContextDir Is Nothing, "Nothing", "[" & sContextDir & "]") & vbCrLf & "Func: " & If(bd.Func Is Nothing, "Nothing",  "[" & bd.Func & "]"))
            End If

            Try
                Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
                sTargetMachineId = GetMachineId(Path.GetFileName(sMachineDir))
                FetchMachineProfileFromFile(sMachineDir)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                Log.Error("�s���ȗv������M���܂����B" & vbCrLf & "WorkingDirectory: [" & sContextDir & "]" & vbCrLf & "Func: [" & bd.Func & "]")
                Exit Try
            End Try

            Log.Info(sTargetMachineId, "[" & sContextDir & "] �ɑ΂���v�� [" & bd.Func & "] ���������܂�...")
            Select Case bd.Func.ToUpperInvariant()
                Case "ClearUpboundData".ToUpperInvariant()
                    isProcCompleted = ClearUpboundData(sTargetMachineId)
                Case "StoreRandFaultData".ToUpperInvariant()
                    isProcCompleted = StoreRandFaultData(sTargetMachineId)
                Case "SendRandFaultData".ToUpperInvariant()
                    isProcCompleted = SendRandFaultData(sTargetMachineId)
                Case "UpdateKadoDataRandomly".ToUpperInvariant()
                    isProcCompleted = UpdateKadoDataRandomly(sTargetMachineId)
                Case "CommitKadoData".ToUpperInvariant()
                    isProcCompleted = CommitKadoData(sTargetMachineId)
                Case Else
                    Log.Error(sTargetMachineId, "���m�̗v���ł��B")
                    isProcCompleted = False
            End Select
        Catch ex As Exception
            Log.Error(sTargetMachineId, "Exception caught.", ex)
        Finally
            SendResponseMessage(e.Message, isProcCompleted, sResult, sTargetMachineId)
        End Try

        Try
            InputQueue.BeginReceive()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    Protected Function CreateFileOfFaultDataPostReq( _
       ByVal oContents As Byte(), _
       ByVal sMachineDir As String, _
       ByVal sMonitorMachineId As String) As String
        Dim oTeleg As New EkByteArrayPostReqTelegram(TelegGene, EkByteArrayPostReqTelegram.FormalObjCodeAsMadoFaultData, oContents, 0)
        Dim sOddFileName As String = "#FaultDataPostReq_"
        Dim sOddFilePath As String = Path.Combine(sMachineDir, sOddFileName)

        Dim sFilePath As String
        Dim branchNum As Integer = -1
        Do
            branchNum += 1
            sFilePath = sOddFilePath & branchNum.ToString() & ".dat"
        Loop While File.Exists(sFilePath)

        Try
            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oTeleg.WriteToStream(oOutputStream)
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̍쐬�����s���܂����B", ex)
            Return Nothing
        End Try
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")
        Return sFilePath
    End Function

    Public Function ClearUpboundData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "�@��̕ێ�������f�[�^���N���A���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try
            Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)
            InitMonitorUpboundData(sMonitorMachineId, oMonitorMachine)
            For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
                InitTermUpboundData(sMonitorMachineId, oTermEntry.Key, oTermEntry.Value)
                UpdateTable2OnTermStateChanged(sMonitorMachineDir, oTermEntry.Key, oTermEntry.Value)
            Next oTermEntry
            Log.Info(sMonitorMachineId, "�N���A���������܂����B")
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True
    End Function

    Public Function StoreRandFaultData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "�����_���ُ�f�[�^�𐶐����Ď��W�p�ɒ~�ς��܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachines�ɐݒ肳��Ă�����Ɨ��������ƂɈُ�f�[�^�𐶐����A
        'sMonitorMachineDir��#FaultDataForPassiveUll.dat�ɒǋL����B

        Dim termCount As Integer = oMonitorMachine.TermMachines.Count
        If termCount = 0 Then
            Log.Warn(sMonitorMachineId, "�z���ɒ[�����Ȃ����ߒ��~���܂����B")
            Return True
        End If

        'NOTE: ���W�����i12���ԁj������ő�80�l�i����40�l�j�̗��p�҂��P�̑����Ŗ����N�����z��ł���B
        'TODO: ���b�V�����̓����w�Ȃǂ͂����Ƒ�����������Ȃ����A
        '�S�w���ςōl����΂����Ə��Ȃ��Ǝv���邽�߁A�������e�ɉ�����
        '�����\�ɂ��������悢�B�f�[�^�O���b�h�Ɂu�l�����x�v�I�ȍ��ځi�l��
        '�ҏW�\�j��p�ӂ���ȂǁB
        Dim recCount As Integer = Rand.Next(0, termCount * 80)

        Dim oTermEntries(termCount - 1) As KeyValuePair(Of String, TermMachine)
        CType(oMonitorMachine.TermMachines, ICollection(Of KeyValuePair(Of String, TermMachine))).CopyTo(oTermEntries, 0)

        'Dim oMoniEntries(UiState.Machines.Count - 1) As KeyValuePair(Of String, Machine)
        'CType(UiState.Machines, ICollection(Of KeyValuePair(Of String, Machine))).CopyTo(oMoniEntries, 0)

        Dim now As DateTime = DateTime.Now
        Dim prevTime As DateTime = DateTime.MinValue
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oTermEntries
            If oTermEntry.Value.FaultDate > prevTime Then
                prevTime = oTermEntry.Value.FaultDate
            End If
        Next oTermEntry
        Dim span As Double = (now - prevTime).TotalSeconds

        Dim sFileName As String = "#FaultDataForPassiveUll.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Try
            Using oOutputStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                Dim fileLen As Long = oOutputStream.Length
                Dim recLen As Integer = FaultDataUtil.RecordLengthInBytes
                If fileLen < recLen Then
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HB8, recCount, recLen, now, oOutputStream)
                Else
                    Dim totalRecCount As Integer = CInt((fileLen \ recLen) - 1) + recCount
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HB8, totalRecCount, recLen, now, oOutputStream)
                    oOutputStream.Seek(0, SeekOrigin.End)
                End If

                For i As Integer = 1 To recCount
                    Dim oBytes(recLen - 1) As Byte

                    Dim t As DateTime = prevTime.AddSeconds(span * i / recCount)
                    Dim termIndex As Integer = Rand.Next(0, termCount)
                    FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A6", oBytes)
                    FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �w�R�[�h", GetHypStationOf(oTermEntries(termIndex).Key), oBytes)
                    FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ��������", t.ToString("yyyyMMddHHmmss"), oBytes)
                    FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �R�[�i�[", GetCornerOf(oTermEntries(termIndex).Key), oBytes)
                    'FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ���@", GetUnitOf(oTermEntries(termIndex).Key), oBytes)
                    FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ���@", "0", oBytes)
                    'FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", MyUtility.GetNextSeqNumber(oTermEntries(termIndex).Value.FaultSeqNumber).ToString(), oBytes)
                    FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", "0", oBytes)
                    FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �o�[�W����", "01", oBytes)
                    FaultDataUtil.SetFieldValueToBytes("�f�[�^�����O�X", "780", oBytes)
                    FaultDataUtil.SetFieldValueToBytes("��������", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
                    FaultDataUtil.SetFieldValueToBytes("���@�ԍ�", GetUnitOf(oTermEntries(termIndex).Key), oBytes)
                    FaultDataUtil.SetFieldValueToBytes("�ʘH����", FaultDataUtil.CreatePassDirectionValue(oTermEntries(termIndex).Value.LatchConf), oBytes)

                    'Dim errorcdIndex As Integer = Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)
                    'FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
                    'FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("�ُ퍀�� �\���f�[�^"), Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

                    Dim sErrorCode As String = Config.FaultDataErrorCodeItems.Rows(Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
                    FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", sErrorCode, oBytes)

                    Dim sErrorText As String = Nothing
                    If Config.FaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                        FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", sErrorText, oBytes)
                    End If
                    If Config.FaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                        FaultDataUtil.SetFieldValueToBytes("�S�����\�� �\���f�[�^", sErrorText, oBytes)
                    End If
                    If Config.FaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                        FaultDataUtil.SetFieldValueToBytes("�ϕ\���� �\���f�[�^", sErrorText, oBytes)
                    End If
                    If Config.FaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                        FaultDataUtil.SetFieldValueToBytes("���u���e �\���f�[�^", sErrorText, oBytes)
                    End If

                    FaultDataUtil.AdjustByteCountField("�ُ퍀��", oBytes)
                    FaultDataUtil.AdjustByteCountField("�S�����\��", oBytes)
                    FaultDataUtil.AdjustByteCountField("�ϕ\����", oBytes)
                    FaultDataUtil.AdjustByteCountField("���u���e", oBytes)

                    oOutputStream.Write(oBytes, 0, oBytes.Length)

                    oTermEntries(termIndex).Value.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
                    oTermEntries(termIndex).Value.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                Next i
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �ւ̃��R�[�h�ǉ������s���܂����B", ex)
            Return False
        End Try
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �� [" & recCount.ToString() & "] ���R�[�h��ǉ����܂����B")

        'NOTE: �X�̒[���̍s�ɂ��āA���x���X�V���邱�ƂɂȂ�\�����������߁A
        '�����őS�[���̍s����x�����X�V���邱�Ƃɂ��Ă���B
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            UpdateTable2OnTermStateChanged(sMonitorMachineDir, oTermEntry.Key, oTermEntry.Value)
        Next oTermEntry

        Return True
    End Function

    Public Function SendRandFaultData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "�����_���ُ�f�[�^�𐶐����������M���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachines�ɐݒ肳��Ă�����Ɨ��������ƂɈُ�f�[�^�𐶐����A
        '�V�~�����[�^�{�̂ɑ��M������B

        Dim termCount As Integer = oMonitorMachine.TermMachines.Count
        If termCount = 0 Then
            Log.Warn(sMonitorMachineId, "�z���ɒ[�����Ȃ����ߒ��~���܂����B")
            Return True
        End If

        Dim oTermEntries(termCount - 1) As KeyValuePair(Of String, TermMachine)
        CType(oMonitorMachine.TermMachines, ICollection(Of KeyValuePair(Of String, TermMachine))).CopyTo(oTermEntries, 0)

        Dim oBytes(FaultDataUtil.RecordLengthInBytes - 1) As Byte

        Dim t As DateTime = DateTime.Now
        Dim termIndex As Integer = Rand.Next(0, termCount)
        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "C3", oBytes)
        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �w�R�[�h", GetHypStationOf(oTermEntries(termIndex).Key), oBytes)
        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ��������", t.ToString("yyyyMMddHHmmss"), oBytes)
        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �R�[�i�[", GetCornerOf(oTermEntries(termIndex).Key), oBytes)
        'FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ���@", GetUnitOf(oTermEntries(termIndex).Key), oBytes)
        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ���@", "0", oBytes)
        'FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", MyUtility.GetNextSeqNumber(oTermEntries(termIndex).Value.FaultSeqNumber).ToString(), oBytes)
        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", "0", oBytes)
        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �o�[�W����", "01", oBytes)
        FaultDataUtil.SetFieldValueToBytes("�f�[�^�����O�X", "780", oBytes)
        FaultDataUtil.SetFieldValueToBytes("��������", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
        FaultDataUtil.SetFieldValueToBytes("���@�ԍ�", GetUnitOf(oTermEntries(termIndex).Key), oBytes)
        FaultDataUtil.SetFieldValueToBytes("�ʘH����", FaultDataUtil.CreatePassDirectionValue(oTermEntries(termIndex).Value.LatchConf), oBytes)

        'Dim errorcdIndex As Integer = Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)
        'FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
        'FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("�ُ퍀�� �\���f�[�^"), Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

        Dim sErrorCode As String = Config.FaultDataErrorCodeItems.Rows(Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
        FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", sErrorCode, oBytes)

        Dim sErrorText As String = Nothing
        If Config.FaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
            FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", sErrorText, oBytes)
        End If
        If Config.FaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
            FaultDataUtil.SetFieldValueToBytes("�S�����\�� �\���f�[�^", sErrorText, oBytes)
        End If
        If Config.FaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
            FaultDataUtil.SetFieldValueToBytes("�ϕ\���� �\���f�[�^", sErrorText, oBytes)
        End If
        If Config.FaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
            FaultDataUtil.SetFieldValueToBytes("���u���e �\���f�[�^", sErrorText, oBytes)
        End If

        FaultDataUtil.AdjustByteCountField("�ُ퍀��", oBytes)
        FaultDataUtil.AdjustByteCountField("�S�����\��", oBytes)
        FaultDataUtil.AdjustByteCountField("�ϕ\����", oBytes)
        FaultDataUtil.AdjustByteCountField("���u���e", oBytes)

        oTermEntries(termIndex).Value.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
        oTermEntries(termIndex).Value.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        UpdateTable2OnTermStateChanged(sMonitorMachineDir, oTermEntries(termIndex).Key, oTermEntries(termIndex).Value)

        Dim sFilePath As String = CreateFileOfFaultDataPostReq(oBytes, sMonitorMachineDir, sMonitorMachineId)
        If sFilePath Is Nothing Then Return False

        Dim oFaultDataParams As Object() = { _
            sFilePath, _
            60000, _
            60000, _
            0, _
            3, _
            True}
        Return SendSimFuncMessage("ActiveOne", oFaultDataParams, sSimWorkingDir, sMonitorMachineId)
    End Function

    Public Function StoreFaultData(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oBytes As Byte()) As Boolean
        Log.Info(sMonitorMachineId, "�@�� [" & sSourceMachineId & "] �ُ̈�f�[�^���Ď��W�p�ɒ~�ς��܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim now As DateTime = DateTime.Now
        Dim sFileName As String = "#FaultDataForPassiveUll.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Try
            Using oOutputStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                Dim fileLen As Long = oOutputStream.Length
                Dim recLen As Integer = FaultDataUtil.RecordLengthInBytes
                If fileLen < recLen Then
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HB8, 1, recLen, now, oOutputStream)
                Else
                    Dim totalRecCount As Integer = CInt((fileLen \ recLen) - 1) + 1
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HB8, totalRecCount, recLen, now, oOutputStream)
                    oOutputStream.Seek(0, SeekOrigin.End)
                End If

                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �ւ̃��R�[�h�ǉ������s���܂����B", ex)
            Return False
        End Try
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �Ƀ��R�[�h��ǉ����܂����B")

        Dim oMachine As TermMachine = UiState.Machines(sMonitorMachineId).TermMachines(sSourceMachineId)
        oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
        oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        UpdateTable2OnTermStateChanged(sMonitorMachineId, sSourceMachineId, oMachine)

        Return True
    End Function

    Public Function SendFaultData(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oBytes As Byte()) As Boolean
        Log.Info(sMonitorMachineId, "�@�� [" & sSourceMachineId & "] �ُ̈�f�[�^�𑦎����M���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMachine As TermMachine = UiState.Machines(sMonitorMachineId).TermMachines(sSourceMachineId)
        oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
        oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        UpdateTable2OnTermStateChanged(sMonitorMachineId, sSourceMachineId, oMachine)

        Dim sFilePath As String = CreateFileOfFaultDataPostReq(oBytes, sMonitorMachineDir, sMonitorMachineId)
        If sFilePath Is Nothing Then Return False

        Dim oFaultDataParams As Object() = { _
            sFilePath, _
            60000, _
            60000, _
            0, _
            3, _
            True}
        Return SendSimFuncMessage("ActiveOne", oFaultDataParams, sSimWorkingDir, sMonitorMachineId)
    End Function

    Public Function UpdateKadoDataRandomly(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "�ғ��f�[�^�������_���ɍX�V���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        'sMonitorMachineDir��#KadoData.dat�̑S���R�[�h���X�V����B
        Dim now As DateTime = DateTime.Now
        Dim yesterday As DateTime = now.AddDays(-1).Date
        Dim sYesterday As String = yesterday.ToString("yyyyMMddHHmmss")
        Dim sFileName As String = "#KadoData.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Dim recLen As Integer = KadoDataUtil.RecordLengthInBytes
        Dim oBytes As Byte() = New Byte(recLen - 1) {}
        Try
            Using oStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
                Dim fileLen As Long = oStream.Length

                If fileLen < recLen * 2 OrElse fileLen Mod recLen <> 0 Then
                    Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̃T�C�Y���ُ�ł��B")
                    Return False
                End If

                Dim recCount As Integer = CInt((fileLen \ recLen) - 1)
                oStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                ExUpboundFileHeader.WriteToStream(&HB7, recCount, recLen, now, oStream)

                Dim oTerms(recCount - 1) As TermMachine
                For Each oTerm As TermMachine In UiState.Machines(sMonitorMachineId).TermMachines.Values
                    If oTerm.KadoSlot >= 1 AndAlso oTerm.KadoSlot <= recCount Then
                        oTerms(oTerm.KadoSlot - 1) = oTerm
                    End If
                Next oTerm

                For recIndex As Integer = 0 To recCount - 1
                    Dim pos As Integer = 0
                    Dim len As Integer = recLen
                    While pos < len
                        Dim readSize As Integer = oStream.Read(oBytes, pos, len - pos)
                        If readSize = 0 Then Exit While  'OPT: �O�̂��߂Ƀ`�F�b�N���Ă��邪�A�t�@�C�����r������Ă������A���蓾�Ȃ��͂��ł���A�s�v�B
                        pos += readSize
                    End While

                    KadoDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ��������", now.ToString("yyyyMMddHHmmss"), oBytes)
                    If oTerms(recIndex) IsNot Nothing Then
                        KadoDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", MyUtility.GetNextSeqNumber(oTerms(recIndex).KadoSeqNumber).ToString(), oBytes)
                    End If

                    KadoDataUtil.SetFieldValueToBytes("���ʕ� �W�v�I��(���W)����", now.ToString("yyyyMMddHHmmss"), oBytes)
                    If Rand.Next(0, 3) = 0 Then
                        Dim sOldDate As String = KadoDataUtil.GetFieldValueFromBytes("���ʕ� ���D���������_������", oBytes)
                        If sOldDate = "00000000000000" OrElse sOldDate < sYesterday Then
                            Dim sNewDate As String = yesterday.AddSeconds(Rand.Next(0, 24 * 60 * 60)).ToString("yyyyMMddHHmmss")
                            KadoDataUtil.SetFieldValueToBytes("���ʕ� ���D���������_������", sNewDate, oBytes)
                            KadoDataUtil.SetFieldValueToBytes("���ʕ� �W�D���������_������", sNewDate, oBytes)
                        End If
                    End If

                    For Each oField As XlsField In KadoDataUtil.Fields
                        If oField.MetaName.StartsWith("�W�v") AndAlso oField.MetaName.Substring(6) <> "�i�󂫁j" Then
                            Dim oldValue As Long = Long.Parse(KadoDataUtil.GetFieldValueFromBytes(oField.MetaName, oBytes))
                            Dim newValue As Long = oldValue + Rand.Next(0, 100)
                            If newValue > UInteger.MaxValue Then
                                newValue = UInteger.MaxValue
                            End If
                            KadoDataUtil.SetFieldValueToBytes(oField.MetaName, newValue.ToString(), oBytes)
                        End If
                    Next oField
                    KadoDataUtil.UpdateSummaryFields(oBytes)

                    oStream.Seek(-recLen, SeekOrigin.Current)
                    oStream.Write(oBytes, 0, oBytes.Length)

                    'NOTE: UiState�ƃO���b�h�̍X�V�͉ғ��f�[�^���W�������ɍs���B
                Next recIndex
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̍X�V�����s���܂����B", ex)
            Return False
        End Try

        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���X�V���܂����B")
        Return True
    End Function

    Public Function UpdateKadoData(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oBytes As Byte()) As Boolean
        Log.Info(sMonitorMachineId, "�@�� [" & sSourceMachineId & "] �̉ғ��f�[�^���X�V���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oTerm As TermMachine = UiState.Machines(sMonitorMachineId).TermMachines(sSourceMachineId)

        Dim now As DateTime = DateTime.Now
        Dim sFileName As String = "#KadoData.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Dim recLen As Integer = KadoDataUtil.RecordLengthInBytes

        Try
            Using oOutputStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                Dim fileLen As Long = oOutputStream.Length
                Dim slotCount As Integer = If(fileLen < recLen, 1, CInt(fileLen \ recLen))

                'OPT: ���L�̃P�[�X�͂��蓾�Ȃ��͂��ł���A�~���K�v���Ȃ��B
                If oTerm.KadoSlot = 0 Then
                    oTerm.KadoSlot = slotCount
                    slotCount += 1
                End If

                oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                ExUpboundFileHeader.WriteToStream(&HB7, slotCount - 1, recLen, now, oOutputStream)

                oOutputStream.Seek(recLen * oTerm.KadoSlot, SeekOrigin.Begin)
                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̍X�V�����s���܂����B", ex)
            Return False
        End Try

        'NOTE: UiState�ƃO���b�h�̍X�V�͉ғ��f�[�^���W�������ɍs���B
        'oTerm.KadoSeqNumber = UInteger.Parse(KadoDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
        'oTerm.KadoDate = DateTime.ParseExact(KadoDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        'UpdateTable2OnTermStateChanged(sMonitorMachineId, sSourceMachineId, oTerm)

        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���X�V���܂����B")
        Return True
    End Function

    Public Function CommitKadoData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "�ғ��f�[�^�̎��W�����𔽉f���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim now As DateTime = DateTime.Now
        Dim sFileName As String = "#KadoData.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Dim recLen As Integer = KadoDataUtil.RecordLengthInBytes
        Dim oBytes As Byte() = New Byte(recLen - 1) {}
        Try
            Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                Dim fileLen As Long = oInputStream.Length

                If fileLen < recLen * 2 OrElse fileLen Mod recLen <> 0 Then
                    Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̃T�C�Y���ُ�ł��B")
                    Return False
                End If

                Dim recCount As Integer = CInt((fileLen \ recLen) - 1)
                oInputStream.Seek(recLen, SeekOrigin.Begin)

                Dim oTerms(recCount - 1) As TermMachine
                Dim oTermKeys(recCount - 1) As String
                For Each oTermEntry As KeyValuePair(Of String, TermMachine) In UiState.Machines(sMonitorMachineId).TermMachines
                    Dim oTerm As TermMachine = oTermEntry.Value
                    If oTerm.KadoSlot >= 1 AndAlso oTerm.KadoSlot <= recCount Then
                        oTerms(oTerm.KadoSlot - 1) = oTerm
                        oTermKeys(oTerm.KadoSlot - 1) = oTermEntry.Key
                    End If
                Next oTermEntry

                For recIndex As Integer = 0 To recCount - 1
                    Dim pos As Integer = 0
                    Dim len As Integer = recLen
                    While pos < len
                        Dim readSize As Integer = oInputStream.Read(oBytes, pos, len - pos)
                        If readSize = 0 Then Exit While  'OPT: �O�̂��߂Ƀ`�F�b�N���Ă��邪�A�t�@�C�����r������Ă������A���蓾�Ȃ��͂��ł���A�s�v�B
                        pos += readSize
                    End While

                    Dim oTerm As TermMachine = oTerms(recIndex)
                    If oTerm IsNot Nothing Then
                        oTerm.KadoSeqNumber = UInteger.Parse(KadoDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
                        oTerm.KadoDate = DateTime.ParseExact(KadoDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                        UpdateTable2OnTermStateChanged(sMonitorMachineId, oTermKeys(recIndex), oTerm)
                    End If
                Next recIndex
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�ُ킪�������܂����B", ex)
            Return False
        End Try

        Log.Info(sMonitorMachineId, "���f���܂����B")
        Return True
    End Function

    Protected Function SendResponseMessage(ByVal oReceivedMessage As Message, ByVal isCompleted As Boolean, ByVal sResult As String, ByVal sMonitorMachineId As String) As Boolean
        Try
            Dim bd As ExtAppFuncMessageBody = DirectCast(oReceivedMessage.Body, ExtAppFuncMessageBody)
            bd.Func = ""
            bd.Args = Nothing
            bd.Completed = isCompleted
            bd.Result = sResult
            Dim oResponseMessage As New Message()
            oResponseMessage.CorrelationId = oReceivedMessage.Id
            oResponseMessage.Body = bd
            oReceivedMessage.ResponseQueue.Send(oResponseMessage)
            If sResult Is Nothing Then
                Log.Info(sMonitorMachineId, "�V�~�����[�^�{�̂։��� [" & isCompleted.ToString() & "] ��ԐM���܂����B")
            Else
                Log.Info(sMonitorMachineId, "�V�~�����[�^�{�̂։��� [" & isCompleted.ToString() & "][" & sResult & "] ��ԐM���܂����B")
            End If
            Return True
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try
    End Function

    Protected Function SendSimFuncMessage(ByVal sVerb As String, ByVal oParams As Object(), ByVal sWorkingDir As String, ByVal sMonitorMachineId As String) As Boolean
        Try
            Dim oOutMessage As New Message()
            Using oTargetQueue As New MessageQueue(Config.TargetMqPath & "@" & sWorkingDir.Replace("\", "/"))
                Dim bd As ExtSimFuncMessageBody
                bd.MachineId = GetMachineDirNameOf(sMonitorMachineId)
                bd.Verb = sVerb
                bd.Params = oParams
                oOutMessage.Body = bd
                oOutMessage.AppSpecific = 1
                oTargetQueue.Send(oOutMessage)
            End Using
            Log.Info(sMonitorMachineId, "�V�~�����[�^�{�̂֗v�� [" & sVerb & "] ���s���܂����B")
            Return True
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try
    End Function

End Class
