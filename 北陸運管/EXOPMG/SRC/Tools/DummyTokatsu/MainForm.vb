' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/08/08  (NES)����  �V�K�쐬
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

    Protected Structure MadoProgramContent
        Dim RunnableDate As String
        Dim ArchiveCatalog As String
        Dim VersionListData As Byte()
    End Structure

    Protected Const MachineDirFormat As String = "%3R%3S_%4C_%2U"
    Protected Const MachineDirPattern As String = "??????_????_??"
    Protected Shared ReadOnly MachineDirRegx As New Regex("^[0-9]{6}_[0-9]{4}_[0-9]{2}$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Protected UiState As UiStateClass
    Protected TelegGene As EkTelegramGene
    Protected TelegImporter As EkTelegramImporter
    Protected Table1 As DataTable
    Protected Table2 As DataTable
    Protected Friend WithEvents InputQueue As MessageQueue = Nothing
    Protected Friend MasProDataFormDic As Dictionary(Of String, Form)
    Protected Friend MasProListFormDic As Dictionary(Of String, Form)

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

    Protected Shared Function GetMachineId(ByVal sModel As String, ByVal sStation As String, ByVal sCorner As String, ByVal sUnit As String) As String
        Return sModel & "_" & sStation & "_" & sCorner & "_" & sUnit
    End Function

    Protected Shared Function GetMachineId(ByVal sMachineDirName As String) As String
        Return Config.ModelSym & "_" & sMachineDirName
    End Function

    Protected Shared Function GetMachineDirNameOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(2)
    End Function

    Protected Shared Function GetModelOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(0, 1)
    End Function

    Protected Shared Function GetStationOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(2, 6)
    End Function

    Protected Shared Function GetCornerOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(9, 4)
    End Function

    Protected Shared Function GetUnitOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(14, 2)
    End Function

    Protected Shared Function GetEkCodeOf(ByVal sMachineId As String) As EkCode
        Return EkCode.Parse(sMachineId.Substring(2), MachineDirFormat)
    End Function

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

    Protected Shared Function ExtractMadoProgramCab(ByVal sFilePath As String, ByVal sTempDirPath As String) As MadoProgramContent
        Dim ret As MadoProgramContent
        Utility.DeleteTemporalDirectory(sTempDirPath)
        Directory.CreateDirectory(sTempDirPath)
        Try
            'CAB��W�J����B
            Using oProcess As New System.Diagnostics.Process()
                oProcess.StartInfo.FileName = Path.Combine(Application.StartupPath, "TsbCab.exe")
                oProcess.StartInfo.Arguments = "-x """ & sFilePath & """ """ & sTempDirPath & "\"""
                oProcess.StartInfo.UseShellExecute = False
                oProcess.StartInfo.RedirectStandardInput = True
                oProcess.StartInfo.CreateNoWindow = True
                oProcess.Start()
                Dim oStreamWriter As StreamWriter = oProcess.StandardInput
                oStreamWriter.WriteLine("")
                oStreamWriter.Close()
                oProcess.WaitForExit()
            End Using

            '�v���O�����o�[�W�������X�g����͂���B
            Dim sVerListPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, ExConstants.MadoProgramVersionListPathInCab)
            Try
                Using oInputStream As New FileStream(sVerListPath, FileMode.Open, FileAccess.Read)
                    '�t�@�C���̃����O�X���擾����B
                    Dim len As Integer = CInt(oInputStream.Length)
                    If len < ProgramVersionListUtil.RecordLengthInBytes Then
                        Throw New OPMGException("�o�[�W�������X�g�̃T�C�Y���ُ�ł��B")
                    End If
                    '�t�@�C����ǂݍ��ށB
                    ret.VersionListData = New Byte(len - 1) {}
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(ret.VersionListData, pos, len - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Throw New OPMGException("�o�[�W�������X�g�̓ǂݍ��݂ňُ킪�������܂����B", ex)
            End Try

            ret.RunnableDate = ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�������싖��", ret.VersionListData)
            Dim oRunnableDate As DateTime
            If DateTime.TryParseExact(ret.RunnableDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, oRunnableDate) = False Then
                Throw New OPMGException("�o�[�W�������X�g�ɋL�ڂ��ꂽ���싖�����ُ�ł��B" & vbCrLf & "���싖��: " & ret.RunnableDate)
            End If
        Finally
            Utility.DeleteTemporalDirectory(sTempDirPath)
        End Try

        'CAB���̃t�@�C���ꗗ���擾����B
        Using oProcess As New System.Diagnostics.Process()
            'NOTE: TsbCab -l �́A�R�}���h�����ɓn��CAB�t�@�C���̃p�X��
            '���o�C�g�������܂܂�Ă���ƃN���b�V������悤�ł��邽�߁A
            'WorkingDirectory�𓖊Y�t�@�C���̂���f�B���N�g���ɂ��邱�ƂŁA
            '�R�}���h�����ɂ̓t�@�C�����݂̂�n�����Ƃɂ���B
            oProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(sFilePath)
            oProcess.StartInfo.FileName = Path.Combine(Application.StartupPath, "TsbCab.exe")
            oProcess.StartInfo.Arguments = "-l """ & Path.GetFileName(sFilePath) & """"
            oProcess.StartInfo.UseShellExecute = False
            oProcess.StartInfo.RedirectStandardInput = True
            oProcess.StartInfo.RedirectStandardOutput = True
            oProcess.StartInfo.CreateNoWindow = True
            oProcess.Start()
            Dim oStreamWriter As StreamWriter = oProcess.StandardInput
            oStreamWriter.WriteLine("")
            ret.ArchiveCatalog = oProcess.StandardOutput.ReadToEnd()
            oStreamWriter.Close()
            oProcess.WaitForExit()
        End Using

        Return ret
    End Function

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
        TelegImporter = New EkTelegramImporter(TelegGene)

        InitTable1()
        InitTable2WithoutFilter()
        TuneTable2FilterToTable1Selection()
        TableSplitContainer.SplitterDistance _
           = DataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) _
            + SystemInformation.VerticalScrollBarWidth _
            + SystemInformation.BorderSize.Width * 2 _
            + TableSplitContainer.SplitterWidth - 1

        MasProDataFormDic = New Dictionary(Of String, Form)
        MasProListFormDic = New Dictionary(Of String, Form)

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
                Dim ws As New XmlWriterSettings()
                ws.NewLineHandling = NewLineHandling.Entitize
                Using xw As XmlWriter = XmlWriter.Create(sStateFileUri, ws)
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
        If TktConStatusRadioButton.Checked Then
            Table2.Columns.Add("NEGA_STS", GetType(Byte))
            Table2.Columns.Add("MEISAI_STS", GetType(Byte))
            Table2.Columns.Add("ONLINE_STS", GetType(Byte))
        End If

        If MadoConStatusRadioButton.Checked Then
            Table2.Columns.Add("DLS_STS", GetType(Byte))
            Table2.Columns.Add("KSB_STS", GetType(Byte))
            Table2.Columns.Add("TK1_STS", GetType(Byte))
            Table2.Columns.Add("TK2_STS", GetType(Byte))
        End If

        If MasStatusRadioButton.Checked Then
            Table2.Columns.Add("SLOT", GetType(String))
            For Each sKind As String In ExConstants.MadoMastersSubObjCodes.Keys
                Table2.Columns.Add(sKind & "_DataSubKind", GetType(Integer))
                Table2.Columns.Add(sKind & "_DataVersion", GetType(Integer))
                Table2.Columns.Add(sKind & "_ListVersion", GetType(Integer))
                Table2.Columns.Add(sKind & "_DataAcceptDate", GetType(DateTime))
                Table2.Columns.Add(sKind & "_ListAcceptDate", GetType(DateTime))
                Table2.Columns.Add(sKind & "_DataDeliverDate", GetType(DateTime))
                Table2.Columns.Add(sKind & "_DataHashValue", GetType(String))
                Table2.Columns.Add(sKind & "_ListHashValue", GetType(String))
            Next sKind
        End If

        If ProStatusRadioButton.Checked Then
            Table2.Columns.Add("SLOT", GetType(String))
            Table2.Columns.Add("YPG_DataSubKind", GetType(Integer))
            Table2.Columns.Add("YPG_DataVersion", GetType(Integer))
            Table2.Columns.Add("YPG_ListVersion", GetType(Integer))
            Table2.Columns.Add("YPG_DataAcceptDate", GetType(DateTime))
            Table2.Columns.Add("YPG_ListAcceptDate", GetType(DateTime))
            Table2.Columns.Add("YPG_DataDeliverDate", GetType(DateTime))
            Table2.Columns.Add("YPG_ListDeliverDate", GetType(DateTime))
            Table2.Columns.Add("YPG_RunnableDate", GetType(String))
            Table2.Columns.Add("YPG_ApplicableDate", GetType(String))
            Table2.Columns.Add("YPG_ApplyDate", GetType(DateTime))
            Table2.Columns.Add("YPG_DataHashValue", GetType(String))
            Table2.Columns.Add("YPG_ListHashValue", GetType(String))
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
        If TktConStatusRadioButton.Checked Then
            InitExtraComboColumnViewOfTable2("NEGA_STS", "�l�K��� (X)", "FF..", "�l�K���", "�������������...", Config.MenuTableOfTktNegaStatus)
            InitExtraComboColumnViewOfTable2("MEISAI_STS", "���׏�� (X)", "FF..", "���׏��", "�������������...", Config.MenuTableOfTktMeisaiStatus)
            InitExtraComboColumnViewOfTable2("ONLINE_STS", "�I�����C����� (X)", "FF..", "�I�����C�����", "�������������...", Config.MenuTableOfTktOnlineStatus)
        End If

        If MadoConStatusRadioButton.Checked Then
            InitExtraComboColumnViewOfTable2("DLS_STS", "�z�M�T�[�o��� (X)", "FF..", "�z�M�T�[�o���", "�������������...", Config.MenuTableOfMadoDlsStatus)
            InitExtraComboColumnViewOfTable2("KSB_STS", "�Ď��Տ�� (X)", "FF..", "�Ď��Տ��", "�������������...", Config.MenuTableOfMadoKsbStatus)
            InitExtraComboColumnViewOfTable2("TK1_STS", "����ID�n��� (X)", "FF..", "����ID�n���", "�������������...", Config.MenuTableOfMadoTk1Status)
            InitExtraComboColumnViewOfTable2("TK2_STS", "����DL�n��� (X)", "FF..", "����DL�n���", "�������������...", Config.MenuTableOfMadoTk2Status)
        End If

        If MasStatusRadioButton.Checked Then
            DataGridView2.Columns("SLOT").ReadOnly = True
            DataGridView2.Columns("SLOT").Frozen = True
            DataGridView2.Columns("SLOT").HeaderText = "����"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("�z�M�҂�(9)",  DataGridView2.Font)
            Dim pnWidth As Integer = MyUtility.GetTextWidth("000.", DataGridView2.Font)
            Dim dvWidth As Integer = MyUtility.GetTextWidth("000.", DataGridView2.Font)
            Dim lvWidth As Integer = MyUtility.GetTextWidth("000.", DataGridView2.Font)
            Dim tmWidth As Integer = MyUtility.GetTextWidth("9999/99/99 99:99..", DataGridView2.Font)
            Dim hvWidth As Integer = MyUtility.GetTextWidth("AAAAAAAAAAAA...", DataGridView2.Font)
            For Each sKind As String In ExConstants.MadoMastersSubObjCodes.Keys
                DataGridView2.Columns(sKind & "_DataSubKind").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataVersion").ReadOnly = True
                DataGridView2.Columns(sKind & "_ListVersion").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataAcceptDate").ReadOnly = True
                DataGridView2.Columns(sKind & "_ListAcceptDate").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataDeliverDate").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataHashValue").ReadOnly = True
                DataGridView2.Columns(sKind & "_ListHashValue").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataSubKind").HeaderText = "�p�^�[��No (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataVersion").HeaderText = "�}�X�^Ver (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListVersion").HeaderText = "���X�gVer (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataAcceptDate").HeaderText = "�f�[�^���� (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListAcceptDate").HeaderText = "���X�g���� (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataDeliverDate").HeaderText = "�f�[�^���� (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataHashValue").HeaderText = "�f�[�^�T�l (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListHashValue").HeaderText = "���X�g�T�l (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataSubKind").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                DataGridView2.Columns(sKind & "_DataVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                DataGridView2.Columns(sKind & "_ListVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                'DataGridView2.Columns(sKind & "_DataAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
                'DataGridView2.Columns(sKind & "_ListAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
                'DataGridView2.Columns(sKind & "_DataDeliverDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
                DataGridView2.Columns(sKind & "_DataSubKind").Width = pnWidth
                DataGridView2.Columns(sKind & "_DataVersion").Width = dvWidth
                DataGridView2.Columns(sKind & "_ListVersion").Width = lvWidth
                DataGridView2.Columns(sKind & "_DataAcceptDate").Width = tmWidth
                DataGridView2.Columns(sKind & "_ListAcceptDate").Width = tmWidth
                DataGridView2.Columns(sKind & "_DataDeliverDate").Width = tmWidth
                DataGridView2.Columns(sKind & "_DataHashValue").Width = hvWidth
                DataGridView2.Columns(sKind & "_ListHashValue").Width = hvWidth
            Next sKind
        End If

        If ProStatusRadioButton.Checked Then
            DataGridView2.Columns("SLOT").ReadOnly = True
            DataGridView2.Columns("SLOT").Frozen = True
            DataGridView2.Columns("SLOT").HeaderText = "����"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("�z�M�҂�(9)", DataGridView2.Font)
            Dim anWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim dvWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim lvWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim adWidth As Integer = MyUtility.GetTextWidth("����������.", DataGridView2.Font)
            Dim rdWidth As Integer = MyUtility.GetTextWidth("����������.", DataGridView2.Font)
            Dim tmWidth As Integer = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)
            Dim hvWidth As Integer = MyUtility.GetTextWidth("AAAAAAAAAAAA...", DataGridView2.Font)
            DataGridView2.Columns("YPG_DataSubKind").ReadOnly = True
            DataGridView2.Columns("YPG_DataVersion").ReadOnly = True
            DataGridView2.Columns("YPG_ListVersion").ReadOnly = True
            DataGridView2.Columns("YPG_DataAcceptDate").ReadOnly = True
            DataGridView2.Columns("YPG_ListAcceptDate").ReadOnly = True
            DataGridView2.Columns("YPG_DataDeliverDate").ReadOnly = True
            DataGridView2.Columns("YPG_ListDeliverDate").ReadOnly = True
            DataGridView2.Columns("YPG_RunnableDate").ReadOnly = True
            DataGridView2.Columns("YPG_ApplicableDate").ReadOnly = True
            DataGridView2.Columns("YPG_ApplyDate").ReadOnly = True
            DataGridView2.Columns("YPG_DataHashValue").ReadOnly = True
            DataGridView2.Columns("YPG_ListHashValue").ReadOnly = True
            DataGridView2.Columns("YPG_DataSubKind").HeaderText = "�G���ANo"
            DataGridView2.Columns("YPG_DataVersion").HeaderText = "��\Ver"
            DataGridView2.Columns("YPG_ListVersion").HeaderText = "���X�gVer"
            DataGridView2.Columns("YPG_DataAcceptDate").HeaderText = "�f�[�^��������"
            DataGridView2.Columns("YPG_ListAcceptDate").HeaderText = "���X�g��������"
            DataGridView2.Columns("YPG_DataDeliverDate").HeaderText = "�f�[�^��������"
            DataGridView2.Columns("YPG_ListDeliverDate").HeaderText = "���X�g��������"
            DataGridView2.Columns("YPG_RunnableDate").HeaderText = "���싖��"
            DataGridView2.Columns("YPG_ApplicableDate").HeaderText = "�K�p��"
            DataGridView2.Columns("YPG_ApplyDate").HeaderText = "�K�p��������"
            DataGridView2.Columns("YPG_DataHashValue").HeaderText = "�f�[�^�n�b�V���l"
            DataGridView2.Columns("YPG_ListHashValue").HeaderText = "���X�g�n�b�V���l"
            DataGridView2.Columns("YPG_DataSubKind").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("YPG_DataVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("YPG_ListVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            'DataGridView2.Columns("YPG_DataAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("YPG_ListAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("YPG_DataDeliverDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("YPG_ListDeliverDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("YPG_ApplyDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("YPG_DataSubKind").Width = anWidth
            DataGridView2.Columns("YPG_DataVersion").Width = dvWidth
            DataGridView2.Columns("YPG_ListVersion").Width = lvWidth
            DataGridView2.Columns("YPG_DataAcceptDate").Width = tmWidth
            DataGridView2.Columns("YPG_ListAcceptDate").Width = tmWidth
            DataGridView2.Columns("YPG_DataDeliverDate").Width = tmWidth
            DataGridView2.Columns("YPG_ListDeliverDate").Width = tmWidth
            DataGridView2.Columns("YPG_RunnableDate").Width = rdWidth
            DataGridView2.Columns("YPG_ApplicableDate").Width = adWidth
            DataGridView2.Columns("YPG_ApplyDate").Width = tmWidth
            DataGridView2.Columns("YPG_DataHashValue").Width = hvWidth
            DataGridView2.Columns("YPG_ListHashValue").Width = hvWidth
        End If
    End Sub

    Protected Function GetMonitorMachineRowCountForTable2(ByVal oMachine As Machine) As Integer
        If TktConStatusRadioButton.Checked Then
            Return 1
        End If

        If MadoConStatusRadioButton.Checked Then
            Return 0
        End If

        If MasStatusRadioButton.Checked Then
            Dim nMax As Integer = 1
            For Each sKind As String In ExConstants.MadoMastersSubObjCodes.Keys
                Dim oHoldingMasters As List(Of HoldingMaster) = Nothing
                If oMachine.HoldingMasters.TryGetValue(sKind, oHoldingMasters) = True Then
                    Dim n As Integer = oHoldingMasters.Count
                    If n > nMax Then
                        nMax = n
                    End If
                End If
            Next sKind
            Return nMax
        End If

        If ProStatusRadioButton.Checked Then
            Dim nMax As Integer = 1
            Dim n As Integer = oMachine.HoldingPrograms.Count
            If n > nMax Then
                nMax = n
            End If
            Return nMax
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
        If TktConStatusRadioButton.Checked Then
            oTargetRow("NEGA_STS") = oMachine.NegaStatus
            oTargetRow("MEISAI_STS") = oMachine.MeisaiStatus
            oTargetRow("ONLINE_STS") = oMachine.OnlineStatus
        End If

        If MasStatusRadioButton.Checked Then
            Dim listIndex As Integer = count - index - 1
            oTargetRow("SLOT") = "�ێ�(" & (listIndex + 1).ToString() & ")"
            For Each sKind As String In ExConstants.MadoMastersSubObjCodes.Keys
                Dim oMas As HoldingMaster = Nothing
                Dim oHoldingMasters As List(Of HoldingMaster) = Nothing
                If oMachine.HoldingMasters.TryGetValue(sKind, oHoldingMasters) = True AndAlso _
                   listIndex < oHoldingMasters.Count Then
                    oMas = oHoldingMasters(listIndex)
                End If

                If oMas IsNot Nothing Then
                    oTargetRow(sKind & "_DataSubKind") = oMas.DataSubKind
                    oTargetRow(sKind & "_DataVersion") = oMas.DataVersion
                Else
                    oTargetRow(sKind & "_DataSubKind") = DbNull.Value
                    oTargetRow(sKind & "_DataVersion") = DbNull.Value
                End If

                If oMas IsNot Nothing AndAlso oMas.DataHashValue IsNot Nothing Then
                    oTargetRow(sKind & "_DataAcceptDate") = oMas.DataAcceptDate
                    oTargetRow(sKind & "_DataHashValue") = oMas.DataHashValue
                Else
                    oTargetRow(sKind & "_DataAcceptDate") = DbNull.Value
                    oTargetRow(sKind & "_DataHashValue") = DbNull.Value
                End If

                If oMas IsNot Nothing AndAlso oMas.ListHashValue IsNot Nothing Then
                    oTargetRow(sKind & "_ListVersion") = oMas.ListVersion
                    oTargetRow(sKind & "_ListAcceptDate") = oMas.ListAcceptDate
                    oTargetRow(sKind & "_ListHashValue") = oMas.ListHashValue
                Else
                    oTargetRow(sKind & "_ListVersion") = DbNull.Value
                    oTargetRow(sKind & "_ListAcceptDate") = DbNull.Value
                    oTargetRow(sKind & "_ListHashValue") = DbNull.Value
                End If

                oTargetRow(sKind & "_DataDeliverDate") = DbNull.Value
            Next sKind
        End If

        If ProStatusRadioButton.Checked Then
            Dim listIndex As Integer = count - index - 1
            Dim oPro As HoldingProgram = Nothing
            If listIndex < oMachine.HoldingPrograms.Count Then
                oPro = oMachine.HoldingPrograms(listIndex)
            End if
            oTargetRow("SLOT") = "�ێ�(" & (listIndex + 1).ToString() & ")"

            If oPro IsNot Nothing Then
                oTargetRow("YPG_DataSubKind") = oPro.DataSubKind
                oTargetRow("YPG_DataVersion") = oPro.DataVersion
            Else
                oTargetRow("YPG_DataSubKind") = DbNull.Value
                oTargetRow("YPG_DataVersion") = DbNull.Value
            End If

            If oPro IsNot Nothing AndAlso oPro.DataHashValue IsNot Nothing Then
                oTargetRow("YPG_DataAcceptDate") = oPro.DataAcceptDate
                oTargetRow("YPG_RunnableDate") = oPro.RunnableDate
                oTargetRow("YPG_DataHashValue") = oPro.DataHashValue
            Else
                oTargetRow("YPG_DataAcceptDate") = DbNull.Value
                oTargetRow("YPG_RunnableDate") = DbNull.Value
                oTargetRow("YPG_DataHashValue") = DbNull.Value
            End If

            If oPro IsNot Nothing AndAlso oPro.ListHashValue IsNot Nothing Then
                oTargetRow("YPG_ListVersion") = oPro.ListVersion
                oTargetRow("YPG_ListAcceptDate") = oPro.ListAcceptDate
                oTargetRow("YPG_ApplicableDate") = DbNull.Value
                oTargetRow("YPG_ListHashValue") = oPro.ListHashValue
            Else
                oTargetRow("YPG_ListVersion") = DbNull.Value
                oTargetRow("YPG_ListAcceptDate") = DbNull.Value
                oTargetRow("YPG_ApplicableDate") = DbNull.Value
                oTargetRow("YPG_ListHashValue") = DbNull.Value
            End If

            oTargetRow("YPG_DataDeliverDate") = DbNull.Value
            oTargetRow("YPG_ListDeliverDate") = DbNull.Value
            oTargetRow("YPG_ApplyDate") = DbNull.Value
        End If
    End Sub

    Protected Function GetTermMachineRowCountForTable2(ByVal oMachine As TermMachine) As Integer
        If TktConStatusRadioButton.Checked Then
            Return 0
        End If

        If MadoConStatusRadioButton.Checked Then
            Return 1
        End If

        If MasStatusRadioButton.Checked Then
            Dim n As Integer = 0
            For Each oPendingMasters As LinkedList(Of PendingMaster) In oMachine.PendingMasters.Values
                If n < oPendingMasters.Count Then
                    n = oPendingMasters.Count
                End If
            Next oPendingMasters
            Return n + 1
        End If

        If ProStatusRadioButton.Checked Then
            Return oMachine.PendingPrograms.Count + 2
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
        If MadoConStatusRadioButton.Checked Then
            oTargetRow("DLS_STS") = oMachine.DlsStatus
            oTargetRow("KSB_STS") = oMachine.KsbStatus
            oTargetRow("TK1_STS") = oMachine.Tk1Status
            oTargetRow("TK2_STS") = oMachine.Tk2Status
        End If

        If MasStatusRadioButton.Checked Then
            If index < count - 1 Then
                Dim listIndex As Integer = count - 1 - index - 1
                oTargetRow("SLOT") = "�z�M�҂�(" & (listIndex + 1).ToString() & ")"
                For Each sKind As String In ExConstants.MadoMastersSubObjCodes.Keys
                    Dim oMas As PendingMaster = Nothing
                    Dim oPendingMasters As LinkedList(Of PendingMaster) = Nothing
                    If oMachine.PendingMasters.TryGetValue(sKind, oPendingMasters) = True AndAlso _
                       listIndex < oPendingMasters.Count Then
                        oMas = oPendingMasters(listIndex)
                    End If

                    If oMas IsNot Nothing Then
                        oTargetRow(sKind & "_DataSubKind") = oMas.DataSubKind
                        oTargetRow(sKind & "_DataVersion") = oMas.DataVersion
                    Else
                        oTargetRow(sKind & "_DataSubKind") = DbNull.Value
                        oTargetRow(sKind & "_DataVersion") = DbNull.Value
                    End If

                    If oMas IsNot Nothing AndAlso oMas.DataHashValue IsNot Nothing Then
                        oTargetRow(sKind & "_DataAcceptDate") = oMas.DataAcceptDate
                        oTargetRow(sKind & "_DataHashValue") = oMas.DataHashValue
                    Else
                        oTargetRow(sKind & "_DataAcceptDate") = DbNull.Value
                        oTargetRow(sKind & "_DataHashValue") = DbNull.Value
                    End If

                    If oMas IsNot Nothing AndAlso oMas.ListHashValue IsNot Nothing Then
                        oTargetRow(sKind & "_ListVersion") = oMas.ListVersion
                        oTargetRow(sKind & "_ListAcceptDate") = oMas.ListAcceptDate
                        oTargetRow(sKind & "_ListHashValue") = oMas.ListHashValue
                    Else
                        oTargetRow(sKind & "_ListVersion") = DbNull.Value
                        oTargetRow(sKind & "_ListAcceptDate") = DbNull.Value
                        oTargetRow(sKind & "_ListHashValue") = DbNull.Value
                    End If

                    oTargetRow(sKind & "_DataDeliverDate") = DbNull.Value
                Next sKind
            Else
                oTargetRow("SLOT") = "�K�p��"
                For Each sKind As String In ExConstants.MadoMastersSubObjCodes.Keys
                    Dim oMas As HoldingMaster = Nothing
                    oMachine.HoldingMasters.TryGetValue(sKind, oMas)

                    'NOTE: �[�����}�X�^�{�̂�ێ������ɓK�p���X�g��ێ����邱�Ƃ�
                    '���蓾�Ȃ��B
                    '�[����sKind�̃}�X�^��ێ����Ă��Ȃ��ꍇ�A
                    '�K��oMas���̂�Nothing�ɂȂ邽�߁A
                    'oMas.DataHashValue��Nothing�ɂȂ邱�Ƃ͂Ȃ��B
                    If oMas IsNot Nothing Then
                        oTargetRow(sKind & "_DataSubKind") = oMas.DataSubKind
                        oTargetRow(sKind & "_DataVersion") = oMas.DataVersion
                        oTargetRow(sKind & "_DataAcceptDate") = oMas.DataAcceptDate
                        oTargetRow(sKind & "_DataDeliverDate") = oMas.DataDeliverDate
                        oTargetRow(sKind & "_DataHashValue") = oMas.DataHashValue
                    Else
                        oTargetRow(sKind & "_DataSubKind") = DbNull.Value
                        oTargetRow(sKind & "_DataVersion") = DbNull.Value
                        oTargetRow(sKind & "_DataAcceptDate") = DbNull.Value
                        oTargetRow(sKind & "_DataDeliverDate") = DbNull.Value
                        oTargetRow(sKind & "_DataHashValue") = DbNull.Value
                    End If

                    If oMas IsNot Nothing AndAlso oMas.ListHashValue IsNot Nothing Then
                        oTargetRow(sKind & "_ListVersion") = oMas.ListVersion
                        oTargetRow(sKind & "_ListAcceptDate") = oMas.ListAcceptDate
                        oTargetRow(sKind & "_ListHashValue") = oMas.ListHashValue
                    Else
                        oTargetRow(sKind & "_ListVersion") = DbNull.Value
                        oTargetRow(sKind & "_ListAcceptDate") = DbNull.Value
                        oTargetRow(sKind & "_ListHashValue") = DbNull.Value
                    End If
                Next sKind
            End If
        End If

        If ProStatusRadioButton.Checked Then
            If index < count - 2 Then
                Dim listIndex As Integer = count - 2 - index - 1
                Dim oPro As PendingProgram = oMachine.PendingPrograms(listIndex)
                oTargetRow("SLOT") = "�z�M�҂�(" & (listIndex + 1).ToString() & ")"

                oTargetRow("YPG_DataSubKind") = oPro.DataSubKind
                oTargetRow("YPG_DataVersion") = oPro.DataVersion

                If oPro.DataHashValue IsNot Nothing Then
                    oTargetRow("YPG_DataAcceptDate") = oPro.DataAcceptDate
                    oTargetRow("YPG_RunnableDate") = oPro.RunnableDate
                    oTargetRow("YPG_DataHashValue") = oPro.DataHashValue
                Else
                    oTargetRow("YPG_DataAcceptDate") = DbNull.Value
                    oTargetRow("YPG_RunnableDate") = DbNull.Value
                    oTargetRow("YPG_DataHashValue") = DbNull.Value
                End If

                If oPro.ListHashValue IsNot Nothing Then
                    oTargetRow("YPG_ListVersion") = oPro.ListVersion
                    oTargetRow("YPG_ListAcceptDate") = oPro.ListAcceptDate
                    oTargetRow("YPG_ApplicableDate") = oPro.ApplicableDate
                    oTargetRow("YPG_ListHashValue") = oPro.ListHashValue
                Else
                    oTargetRow("YPG_ListVersion") = DbNull.Value
                    oTargetRow("YPG_ListAcceptDate") = DbNull.Value
                    oTargetRow("YPG_ApplicableDate") = DbNull.Value
                    oTargetRow("YPG_ListHashValue") = DbNull.Value
                End If

                oTargetRow("YPG_DataDeliverDate") = DbNull.Value
                oTargetRow("YPG_ListDeliverDate") = DbNull.Value
                oTargetRow("YPG_ApplyDate") = DbNull.Value
            Else
                Dim listIndex As Integer = 1 - index + (count - 2)
                Dim oPro As HoldingProgram = oMachine.HoldingPrograms(listIndex)
                oTargetRow("SLOT") = If(listIndex = 1, "�K�p�҂�", "�K�p��")

                If oPro IsNot Nothing Then
                    oTargetRow("YPG_DataSubKind") = oPro.DataSubKind
                    oTargetRow("YPG_DataVersion") = oPro.DataVersion
                    oTargetRow("YPG_DataAcceptDate") = oPro.DataAcceptDate
                    oTargetRow("YPG_DataDeliverDate") = oPro.DataDeliverDate
                    oTargetRow("YPG_RunnableDate") = oPro.RunnableDate
                    oTargetRow("YPG_ApplyDate") = If(listIndex = 1, DbNull.Value, DirectCast(oPro.ApplyDate, Object))
                    oTargetRow("YPG_DataHashValue") = oPro.DataHashValue
                Else
                    oTargetRow("YPG_DataSubKind") = DbNull.Value
                    oTargetRow("YPG_DataVersion") = DbNull.Value
                    oTargetRow("YPG_DataAcceptDate") = DbNull.Value
                    oTargetRow("YPG_DataDeliverDate") = DbNull.Value
                    oTargetRow("YPG_RunnableDate") = DbNull.Value
                    oTargetRow("YPG_ApplyDate") = DbNull.Value
                    oTargetRow("YPG_DataHashValue") = DbNull.Value
                End If

                If oPro IsNot Nothing AndAlso oPro.ListHashValue IsNot Nothing Then
                    oTargetRow("YPG_ListVersion") = oPro.ListVersion
                    oTargetRow("YPG_ListAcceptDate") = oPro.ListAcceptDate
                    oTargetRow("YPG_ListDeliverDate") = oPro.ListDeliverDate
                    oTargetRow("YPG_ApplicableDate") = oPro.ApplicableDate
                    oTargetRow("YPG_ListHashValue") = oPro.ListHashValue
                Else
                    oTargetRow("YPG_ListVersion") = DbNull.Value
                    oTargetRow("YPG_ListAcceptDate") = DbNull.Value
                    oTargetRow("YPG_ListDeliverDate") = DbNull.Value
                    oTargetRow("YPG_ApplicableDate") = DbNull.Value
                    oTargetRow("YPG_ListHashValue") = DbNull.Value
                End If
            End If
        End If
    End Sub

    Protected Sub FetchStateFromTable2Row(ByVal oRow As DataRow)
        Dim sMachineId As String = oRow.Field(Of String)("MACHINE_ID")
        Dim oMachine As Machine = UiState.Machines(sMachineId)

        If TktConStatusRadioButton.Checked Then
            oMachine.NegaStatus = oRow.Field(Of Byte)("NEGA_STS")
            oMachine.MeisaiStatus = oRow.Field(Of Byte)("MEISAI_STS")
            oMachine.OnlineStatus = oRow.Field(Of Byte)("ONLINE_STS")
        End If

        If MadoConStatusRadioButton.Checked Then
            Dim sTermMachineId As String = oRow.Field(Of String)("TERM_MACHINE_ID")
            Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
            oTerm.DlsStatus = oRow.Field(Of Byte)("DLS_STS")
            oTerm.KsbStatus = oRow.Field(Of Byte)("KSB_STS")
            oTerm.Tk1Status = oRow.Field(Of Byte)("TK1_STS")
            oTerm.Tk2Status = oRow.Field(Of Byte)("TK2_STS")
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

        With Nothing
            Dim sFile As String = Path.Combine(sMachineDirPath, "#TermMachine.csv")
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

                        oTerm.HoldingPrograms(0) = New HoldingProgram()
                        oTerm.HoldingPrograms(0).DataAcceptDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).ListAcceptDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).DataDeliverDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).ListDeliverDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).ApplyDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).DataSubKind = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
                        oTerm.HoldingPrograms(0).DataVersion = 0
                        oTerm.HoldingPrograms(0).ListVersion = 0
                        oTerm.HoldingPrograms(0).RunnableDate = "00000000"
                        oTerm.HoldingPrograms(0).ApplicableDate = "00000000"
                        oTerm.HoldingPrograms(0).ArchiveCatalog = ""
                        oTerm.HoldingPrograms(0).VersionListData = New Byte(ProgramVersionListUtil.RecordLengthInBytes - 1) {}
                        oTerm.HoldingPrograms(0).ListContent = ""
                        oTerm.HoldingPrograms(0).DataHashValue = Config.UnknownHashValue
                        oTerm.HoldingPrograms(0).ListHashValue = Config.UnknownHashValue

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

    Private Sub ViewModeRadioButtons_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
       Handles TktConStatusRadioButton.CheckedChanged, MadoConStatusRadioButton.CheckedChanged, _
               MasStatusRadioButton.CheckedChanged, ProStatusRadioButton.CheckedChanged
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

        If MasStatusRadioButton.Checked OrElse ProStatusRadioButton.Checked Then
            Dim sColName As String = DataGridView2.Columns(e.ColumnIndex).DataPropertyName
            If Config.MachineProfileFieldNamesIndices.ContainsKey(sColName) Then Return
            If sColName = "SLOT" Then Return
            Dim sDataKind As String = sColName.Substring(0, 3)

            Dim oView As DataRowView = DirectCast(DataGridView2.Rows(e.RowIndex).DataBoundItem, DataRowView)
            If oView.Row.IsNull(sDataKind & "_DataSubKind") Then Return
            Dim sMachineId As String = oView.Row.Field(Of String)("MACHINE_ID")
            Dim dataSubKind As Integer = oView.Row.Field(Of Integer)(sDataKind & "_DataSubKind")
            Dim dataVersion As Integer = oView.Row.Field(Of Integer)(sDataKind & "_DataVersion")

            Dim sColType As String = sColName.Substring(4)
            If sColType = "ListVersion" OrElse sColType = "ListAcceptDate" OrElse sColType = "ListDeliverDate" OrElse _
               sColType = "ApplicableDate"  OrElse sColType = "ListHashValue" Then
                If oView.Row(sDataKind & "_ListHashValue").GetType() IsNot GetType(String) Then Return

                Dim listVersion As Integer = oView.Row.Field(Of Integer)(sDataKind & "_ListVersion")
                Dim listAcceptDate As DateTime = oView.Row.Field(Of DateTime)(sDataKind & "_ListAcceptDate")
                Dim sListHashValue As String = oView.Row.Field(Of String)(sDataKind & "_ListHashValue")
                Dim sKey As String = sMachineId & "." & sDataKind & "." & dataSubKind.ToString() & "." & dataVersion.ToString() & "."  & listVersion.ToString() & "." & listAcceptDate.ToString("yyyyMMddHHmmssfff") & "." & sListHashValue
                Dim oForm As Form = Nothing
                If MasProListFormDic.TryGetValue(sKey, oForm) = True Then
                    oForm.Activate()
                Else
                    Dim sListContent As String = Nothing
                    Dim oMachine As Machine = UiState.Machines(sMachineId)
                    If MasStatusRadioButton.Checked Then
                        Dim sTermMachineId As String = oView.Row.Field(Of String)("TERM_MACHINE_ID")
                        If sTermMachineId.Length = 0 Then
                            'NOTE: �_�u���N���b�N���ꂽ�̂��Ď��@��̍s�ł���A���̍s��sDataKind�Ɋւ����ɂ͒l�����݂��Ă���ꍇ�ł���B
                            '�Ď��@��̍s�́A�ێ�(n)�݂̂ł��邽�߁A��L�̏������������Ă���Ȃ�AoMachine.HoldingMasters�ɂ�
                            'sDataKind���L�[�Ƃ���v�f���K�����݂��Ă���B
                            For Each oMas As HoldingMaster In oMachine.HoldingMasters(sDataKind)
                                If oMas.DataSubKind = dataSubKind AndAlso _
                                   oMas.DataVersion = dataVersion AndAlso _
                                   oMas.ListVersion = listVersion AndAlso _
                                   oMas.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oMas.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oMas.ListContent
                                    Exit For
                                End If
                            Next oMas
                        Else
                            'NOTE: �_�u���N���b�N���ꂽ�̂��[���@��̍s�ł���A���̍s��sDataKind�Ɋւ����ɂ͒l�����݂��Ă���ꍇ�ł���B
                            '�[���@��ɂ́A�K�p���̍s�Ɣz�M�҂�(n)�̍s�����邽�߁A��L�̏������������Ă��Ă��A�i�_�u���N���b�N���ꂽ
                            '�̂��z�M�҂�(n)�̍s�Ȃ�joTerm.HoldingMasters��sDataKind���L�[�Ƃ���v�f�����݂��Ă���Ƃ͌���Ȃ����A
                            '�i�_�u���N���b�N���ꂽ�̂��K�p���̍s�Ȃ�joTerm.PendingMasters��sDataKind���L�[�Ƃ���v�f�����݂��Ă���
                            '�Ƃ͌���Ȃ��B
                            Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                            With Nothing
                                Dim oMas As HoldingMaster = Nothing
                                If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = True AndAlso _
                                   oMas.DataSubKind = dataSubKind AndAlso _
                                   oMas.DataVersion = dataVersion AndAlso _
                                   oMas.ListVersion = listVersion AndAlso _
                                   oMas.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oMas.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oMas.ListContent
                                End If
                            End With
                            If sListContent Is Nothing Then
                                Dim oPendingMasters As LinkedList(Of PendingMaster) = Nothing
                                If oTerm.PendingMasters.TryGetValue(sDataKind, oPendingMasters) = True Then
                                    For Each oMas As PendingMaster In oPendingMasters
                                        If oMas IsNot Nothing AndAlso _
                                           oMas.DataSubKind = dataSubKind AndAlso _
                                           oMas.DataVersion = dataVersion AndAlso _
                                           oMas.ListVersion = listVersion AndAlso _
                                           oMas.ListAcceptDate = listAcceptDate AndAlso _
                                           StringComparer.OrdinalIgnoreCase.Compare(oMas.ListHashValue, sListHashValue) = 0 Then
                                            sListContent = oMas.ListContent
                                            Exit For
                                        End If
                                    Next oMas
                                End If
                            End If
                        End If
                    ElseIf ProStatusRadioButton.Checked Then
                        Dim sTermMachineId As String = oView.Row.Field(Of String)("TERM_MACHINE_ID")
                        If sTermMachineId.Length = 0 Then
                            For Each oPro As HoldingProgram In oMachine.HoldingPrograms
                                If oPro.DataSubKind = dataSubKind AndAlso _
                                   oPro.DataVersion = dataVersion AndAlso _
                                   oPro.ListVersion = listVersion AndAlso _
                                   oPro.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oPro.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oPro.ListContent
                                    Exit For
                                End If
                            Next oPro
                        Else
                            Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                            For Each oPro As HoldingProgram In oTerm.HoldingPrograms
                                If oPro IsNot Nothing AndAlso _
                                   oPro.DataSubKind = dataSubKind AndAlso _
                                   oPro.DataVersion = dataVersion AndAlso _
                                   oPro.ListVersion = listVersion AndAlso _
                                   oPro.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oPro.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oPro.ListContent
                                    Exit For
                                End If
                            Next oPro
                            If sListContent Is Nothing Then
                                For Each oPro As PendingProgram In oTerm.PendingPrograms
                                    If oPro IsNot Nothing AndAlso _
                                       oPro.DataSubKind = dataSubKind AndAlso _
                                       oPro.DataVersion = dataVersion AndAlso _
                                       oPro.ListVersion = listVersion AndAlso _
                                       oPro.ListAcceptDate = listAcceptDate AndAlso _
                                       StringComparer.OrdinalIgnoreCase.Compare(oPro.ListHashValue, sListHashValue) = 0 Then
                                        sListContent = oPro.ListContent
                                        Exit For
                                    End If
                                Next oPro
                            End If
                        End If
                    End If
                    If sListContent IsNot Nothing Then
                        oForm = New ApplicableListForm(sMachineId, sDataKind, dataSubKind, dataVersion, listVersion, listAcceptDate, sListHashValue, sListContent, sKey, Me)
                        MasProListFormDic.Add(sKey, oForm)
                        oForm.Show()
                    End If
                End If
            Else
                If oView.Row(sDataKind & "_DataHashValue").GetType() IsNot GetType(String) Then Return

                Dim dataAcceptDate As DateTime = oView.Row.Field(Of DateTime)(sDataKind & "_DataAcceptDate")
                Dim sDataHashValue As String = oView.Row.Field(Of String)(sDataKind & "_DataHashValue")
                Dim sKey As String = sMachineId & "." & sDataKind & "." & dataSubKind.ToString() & "." & dataVersion.ToString() & "." & dataAcceptDate.ToString("yyyyMMddHHmmssfff") & "." & sDataHashValue

                Dim oForm As Form = Nothing
                If MasProDataFormDic.TryGetValue(sKey, oForm) = True Then
                    oForm.Activate()
                ElseIf MasStatusRadioButton.Checked Then
                    Dim oDataFooter As Byte() = Nothing
                    Dim oMachine As Machine = UiState.Machines(sMachineId)
                    Dim sTermMachineId As String = oView.Row.Field(Of String)("TERM_MACHINE_ID")
                    If sTermMachineId.Length = 0 Then
                        'NOTE: �_�u���N���b�N���ꂽ�̂��Ď��@��̍s�ł���A���̍s��sDataKind�Ɋւ����ɂ͒l�����݂��Ă���ꍇ�ł���B
                        '�Ď��@��̍s�́A�ێ�(n)�݂̂ł��邽�߁A��L�̏������������Ă���Ȃ�AoMachine.HoldingMasters�ɂ�
                        'sDataKind���L�[�Ƃ���v�f���K�����݂��Ă���B
                        For Each oMas As HoldingMaster In oMachine.HoldingMasters(sDataKind)
                            If oMas.DataSubKind = dataSubKind AndAlso _
                               oMas.DataVersion = dataVersion AndAlso _
                               oMas.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oMas.DataHashValue, sDataHashValue) = 0 Then
                                oDataFooter = oMas.DataFooter
                                Exit For
                            End If
                        Next oMas
                    Else
                        'NOTE: �_�u���N���b�N���ꂽ�̂��[���@��̍s�ł���A���̍s��sDataKind�Ɋւ����ɂ͒l�����݂��Ă���ꍇ�ł���B
                        '�[���@��ɂ́A�K�p���̍s�Ɣz�M�҂�(n)�̍s�����邽�߁A��L�̏������������Ă��Ă��A�i�_�u���N���b�N���ꂽ
                        '�̂��z�M�҂�(n)�̍s�Ȃ�joTerm.HoldingMasters��sDataKind���L�[�Ƃ���v�f�����݂��Ă���Ƃ͌���Ȃ����A
                        '�i�_�u���N���b�N���ꂽ�̂��K�p���̍s�Ȃ�joTerm.PendingMasters��sDataKind���L�[�Ƃ���v�f�����݂��Ă���
                        '�Ƃ͌���Ȃ��B
                        Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                        With Nothing
                            Dim oMas As HoldingMaster = Nothing
                            If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = True AndAlso _
                               oMas.DataSubKind = dataSubKind AndAlso _
                               oMas.DataVersion = dataVersion AndAlso _
                               oMas.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oMas.DataHashValue, sDataHashValue) = 0 Then
                                oDataFooter = oMas.DataFooter
                            End If
                        End With
                        If oDataFooter Is Nothing Then
                            Dim oPendingMasters As LinkedList(Of PendingMaster) = Nothing
                            If oTerm.PendingMasters.TryGetValue(sDataKind, oPendingMasters) = True Then
                                For Each oMas As PendingMaster In oPendingMasters
                                    If oMas IsNot Nothing AndAlso _
                                       oMas.DataSubKind = dataSubKind AndAlso _
                                       oMas.DataVersion = dataVersion AndAlso _
                                       oMas.DataAcceptDate = dataAcceptDate AndAlso _
                                       StringComparer.OrdinalIgnoreCase.Compare(oMas.DataHashValue, sDataHashValue) = 0 Then
                                        oDataFooter = oMas.DataFooter
                                        Exit For
                                    End If
                                Next oMas
                            End If
                        End If
                    End If
                    If oDataFooter IsNot Nothing Then
                        oForm = New MadoMasDataForm(sMachineId, sDataKind, dataSubKind, dataVersion, dataAcceptDate, sDataHashValue, oDataFooter, sKey, Me)
                        MasProDataFormDic.Add(sKey, oForm)
                        oForm.Show()
                    End If
                ElseIf ProStatusRadioButton.Checked Then
                    Dim sArchiveCatalog As String = Nothing
                    Dim oVersionListData As Byte() = Nothing
                    Dim oMachine As Machine = UiState.Machines(sMachineId)
                    Dim sTermMachineId As String = oView.Row.Field(Of String)("TERM_MACHINE_ID")
                    If sTermMachineId.Length = 0 Then
                        For Each oPro As HoldingProgram In oMachine.HoldingPrograms
                            If oPro.DataSubKind = dataSubKind AndAlso _
                               oPro.DataVersion = dataVersion AndAlso _
                               oPro.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oPro.DataHashValue, sDataHashValue) = 0 Then
                                sArchiveCatalog = oPro.ArchiveCatalog
                                oVersionListData = oPro.VersionListData
                                Exit For
                            End If
                        Next oPro
                    Else
                        Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                        For Each oPro As HoldingProgram In oTerm.HoldingPrograms
                            If oPro IsNot Nothing AndAlso _
                               oPro.DataSubKind = dataSubKind AndAlso _
                               oPro.DataVersion = dataVersion AndAlso _
                               oPro.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oPro.DataHashValue, sDataHashValue) = 0 Then
                                sArchiveCatalog = oPro.ArchiveCatalog
                                oVersionListData = oPro.VersionListData
                                Exit For
                            End If
                        Next oPro
                        If oVersionListData Is Nothing Then
                            For Each oPro As PendingProgram In oTerm.PendingPrograms
                                If oPro IsNot Nothing AndAlso _
                                   oPro.DataSubKind = dataSubKind AndAlso _
                                   oPro.DataVersion = dataVersion AndAlso _
                                   oPro.DataAcceptDate = dataAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oPro.DataHashValue, sDataHashValue) = 0 Then
                                    sArchiveCatalog = oPro.ArchiveCatalog
                                    oVersionListData = oPro.VersionListData
                                    Exit For
                                End If
                            Next oPro
                        End If
                    End If
                    If oVersionListData IsNot Nothing Then
                        oForm = New MadoProDataForm(sMachineId, sDataKind, dataSubKind, dataVersion, dataAcceptDate, sDataHashValue, sArchiveCatalog, oVersionListData, sKey, Me)
                        MasProDataFormDic.Add(sKey, oForm)
                        oForm.Show()
                    End If
                End If
            End If
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

    Private Sub MasClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MasClearButton.Click
        Dim t2TermIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "TERM_MACHINE_ID")
        Dim t2MonitorIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "MACHINE_ID")

        'DataGridView2�őI�𒆂̑����𒊏o����B
        Dim oMachineIds As New Dictionary(Of String, String)
        For Each oGridSelection As DataGridViewCell In DataGridView2.SelectedCells
            Dim sTermMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2TermIdCol).Value, String)
            If sTermMachineId.Length <> 0 AndAlso Not oMachineIds.ContainsKey(sTermMachineId) Then
                Dim sMonitorMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2MonitorIdCol).Value, String)
                oMachineIds.Add(sTermMachineId, sMonitorMachineId)
            End If
        Next oGridSelection

        If oMachineIds.Count = 0 Then
            AlertBox.Show(Lexis.TermMachineRowNotSelected)
            Return
        End If

        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        For Each oIdSet As KeyValuePair(Of String, String) In oMachineIds
            Dim sTermMachineId As String = oIdSet.Key
            Dim sMonitorMachineId As String = oIdSet.Value
            Log.Info(sMonitorMachineId, "�I�𒆂̒[���@�� [" & sTermMachineId & "] ����A�}�X�^����у}�X�^�K�p���X�g���폜���܂�...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Continue For
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            If ClearMadoMas(sContextDir, sTermMachineId) = True Then
                Dim sTermCodeInFileName As String = GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId)
                Dim oVerInfoParams As Object() = { _
                    &H8B, _
                    "M_Y_" & sTermCodeInFileName & "VER.DAT", _
                    Path.Combine(sContextDir, "MadoMasVerInfo_" & sTermCodeInFileName & ".dat"), _
                    "", _
                    0, _
                    60000, _
                    60000, _
                    60000, _
                    0, _
                    3, _
                    True}
                SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
            End If
        Next oIdSet
    End Sub

    Private Sub MasDeliverButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MasDeliverButton.Click
        Dim t2TermIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "TERM_MACHINE_ID")
        Dim t2MonitorIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "MACHINE_ID")

        'DataGridView2�őI�𒆂̑����𒊏o����B
        Dim oMachineIds As New Dictionary(Of String, String)
        For Each oGridSelection As DataGridViewCell In DataGridView2.SelectedCells
            Dim sTermMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2TermIdCol).Value, String)
            If sTermMachineId.Length <> 0 AndAlso Not oMachineIds.ContainsKey(sTermMachineId) Then
                Dim sMonitorMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2MonitorIdCol).Value, String)
                oMachineIds.Add(sTermMachineId, sMonitorMachineId)
            End If
        Next oGridSelection

        If oMachineIds.Count = 0 Then
            AlertBox.Show(Lexis.TermMachineRowNotSelected)
            Return
        End If

        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        For Each oIdSet As KeyValuePair(Of String, String) In oMachineIds
            Dim sTermMachineId As String = oIdSet.Key
            Dim sMonitorMachineId As String = oIdSet.Value
            Log.Info(sMonitorMachineId, "�I�𒆂̒[���@�� [" & sTermMachineId & "] �ɁA�����Ŕz�M�҂��̑S�}�X�^��z�M���܂�...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Continue For
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            If DeliverMadoMas(sContextDir, sTermMachineId) = True Then
                Dim sTermCodeInFileName As String = GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId)

                Dim oDlReflectParams As Object() = { _
                    Path.Combine(sMonitorMachineDir, "#MadoMasDlReflectReq_" & sTermCodeInFileName & "_*.dat"), _
                    60000, _
                    60000, _
                    0, _
                    3, _
                    True}
                SendSimFuncMessage("TryActiveOne", oDlReflectParams, sSimWorkingDir, sMonitorMachineId)

                Dim oVerInfoParams As Object() = { _
                    &H8B, _
                    "M_Y_" & sTermCodeInFileName & "VER.DAT", _
                    Path.Combine(sContextDir, "MadoMasVerInfo_" & sTermCodeInFileName & ".dat"), _
                    "", _
                    0, _
                    60000, _
                    60000, _
                    60000, _
                    0, _
                    3, _
                    True}
                SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
            End If
        Next oIdSet
    End Sub

    Private Sub MasSweepButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MasSweepButton.Click
        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] �̃}�X�^�􂢑ւ����s���܂�...")
                Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)
                SweepMonitorMachineHoldingMasters(sMonitorMachineId, oMonitorMachine)
                Log.Info(sMonitorMachineId, "�}�X�^�􂢑ւ����I�����܂����B")
            End If
        Next gridRow
    End Sub

    Private Sub ProDirectInstallButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProDirectInstallButton.Click
        Dim t2TermIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "TERM_MACHINE_ID")
        Dim t2MonitorIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "MACHINE_ID")

        'DataGridView2�őI�𒆂̑����𒊏o����B
        Dim oMachineIds As New Dictionary(Of String, String)
        For Each oGridSelection As DataGridViewCell In DataGridView2.SelectedCells
            Dim sTermMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2TermIdCol).Value, String)
            If sTermMachineId.Length <> 0 AndAlso Not oMachineIds.ContainsKey(sTermMachineId) Then
                Dim sMonitorMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2MonitorIdCol).Value, String)
                oMachineIds.Add(sTermMachineId, sMonitorMachineId)
            End If
        Next oGridSelection

        If oMachineIds.Count = 0 Then
            AlertBox.Show(Lexis.TermMachineRowNotSelected)
            Return
        End If

        Dim oDialog As New OpenFileDialog
        oDialog.Filter = "CAB�t�@�C��|*.cab"
        oDialog.FileName = ""
        oDialog.ReadOnlyChecked = True
        oDialog.Title = "��������v���O������I�����Ă��������B"
        If oDialog.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        'NOTE: �ȉ��A�����������A�����@�킪�I������Ă���ꍇ�̑��x���\��D�悵�āA
        'InstallMadoProgramDirectly�̒��ł͂Ȃ��A�Ăь���CAB�̉�͂��s�����Ƃɂ��Ă���B

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(oDialog.FileName)
        Catch ex As Exception
            Log.Error("�v���O�����{�̂̃n�b�V���l�Z�o�ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim content As MadoProgramContent
        Try
            Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
            Dim sMonitorMachineId As String = DirectCast(DataGridView1.CurrentRow.Cells(idxColumn).Value, String)
            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error("��\�@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            content = ExtractMadoProgramCab(oDialog.FileName, Path.Combine(sContextDir, "MadoPro"))
        Catch ex As Exception
            Log.Error("�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̃G���ANo�̒��o�ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̑�\Ver�̒��o�ŗ�O���������܂����B", ex)
            Return
        End Try

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("�v���O�����̓��싖�����������ɐݒ肳��Ă��܂��B" & content.RunnableDate & "�܂œK�p�ł��܂���̂ł����ӂ��������B")
        End If

        For Each oIdSet As KeyValuePair(Of String, String) In oMachineIds
            Dim sTermMachineId As String = oIdSet.Key
            Dim sMonitorMachineId As String = oIdSet.Value
            Log.Info(sMonitorMachineId, "�I�𒆂̒[���@�� [" & sTermMachineId & "] �ɁA�����v���O�����𒼐ړ������܂�...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Continue For
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            InstallMadoProgramDirectly(sContextDir, sTermMachineId, subKind, version, content, sHashValue)

            Dim sTermCodeInFileName As String = GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId)

            'Dim oDlReflectParams As Object() = { _
            '    Path.Combine(sMonitorMachineDir, "#MadoProDlReflectReq_" & sTermCodeInFileName & "_*.dat"), _
            '    60000, _
            '    60000, _
            '    0, _
            '    3, _
            '    True}
            'SendSimFuncMessage("TryActiveOne", oDlReflectParams, sSimWorkingDir, sMonitorMachineId)

            Dim oVerInfoParams As Object() = { _
                &H87, _
                "P_Y_" & sTermCodeInFileName & "VER.DAT", _
                Path.Combine(sContextDir, "MadoProVerInfo_" & sTermCodeInFileName & ".dat"), _
                "", _
                0, _
                60000, _
                60000, _
                60000, _
                0, _
                3, _
                True}
            SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
        Next oIdSet
    End Sub

    Private Sub ProDeliverButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProDeliverButton.Click
        Dim t2TermIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "TERM_MACHINE_ID")
        Dim t2MonitorIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "MACHINE_ID")

        'DataGridView2�őI�𒆂̑����𒊏o����B
        Dim oMachineIds As New Dictionary(Of String, String)
        For Each oGridSelection As DataGridViewCell In DataGridView2.SelectedCells
            Dim sTermMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2TermIdCol).Value, String)
            If sTermMachineId.Length <> 0 AndAlso Not oMachineIds.ContainsKey(sTermMachineId) Then
                Dim sMonitorMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2MonitorIdCol).Value, String)
                oMachineIds.Add(sTermMachineId, sMonitorMachineId)
            End If
        Next oGridSelection

        If oMachineIds.Count = 0 Then
            AlertBox.Show(Lexis.TermMachineRowNotSelected)
            Return
        End If

        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        For Each oIdSet As KeyValuePair(Of String, String) In oMachineIds
            Dim sTermMachineId As String = oIdSet.Key
            Dim sMonitorMachineId As String = oIdSet.Value
            Log.Info(sMonitorMachineId, "�I�𒆂̒[���@�� [" & sTermMachineId & "] �ɁA�����Ŕz�M�҂��̑S�����v���O������z�M���܂�...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Continue For
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            If DeliverMadoPro(sContextDir, sTermMachineId) = True Then
                Dim sTermCodeInFileName As String = GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId)

                Dim oDlReflectParams As Object() = { _
                    Path.Combine(sMonitorMachineDir, "#MadoProDlReflectReq_" & sTermCodeInFileName & "_*.dat"), _
                    60000, _
                    60000, _
                    0, _
                    3, _
                    True}
                SendSimFuncMessage("TryActiveOne", oDlReflectParams, sSimWorkingDir, sMonitorMachineId)

                Dim oVerInfoParams As Object() = { _
                    &H87, _
                    "P_Y_" & sTermCodeInFileName & "VER.DAT", _
                    Path.Combine(sContextDir, "MadoProVerInfo_" & sTermCodeInFileName & ".dat"), _
                    "", _
                    0, _
                    60000, _
                    60000, _
                    60000, _
                    0, _
                    3, _
                    True}
                SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
            End If
        Next oIdSet
    End Sub

    Private Sub ProApplyButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProApplyButton.Click
        Dim t2TermIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "TERM_MACHINE_ID")
        Dim t2MonitorIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "MACHINE_ID")

        'DataGridView2�őI�𒆂̑����𒊏o����B
        Dim oMachineIds As New Dictionary(Of String, String)
        For Each oGridSelection As DataGridViewCell In DataGridView2.SelectedCells
            Dim sTermMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2TermIdCol).Value, String)
            If sTermMachineId.Length <> 0 AndAlso Not oMachineIds.ContainsKey(sTermMachineId) Then
                Dim sMonitorMachineId As String = DirectCast(oGridSelection.OwningRow.Cells(t2MonitorIdCol).Value, String)
                oMachineIds.Add(sTermMachineId, sMonitorMachineId)
            End If
        Next oGridSelection

        If oMachineIds.Count = 0 Then
            AlertBox.Show(Lexis.TermMachineRowNotSelected)
            Return
        End If

        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        For Each oIdSet As KeyValuePair(Of String, String) In oMachineIds
            Dim sTermMachineId As String = oIdSet.Key
            Dim sMonitorMachineId As String = oIdSet.Value
            Log.Info(sMonitorMachineId, "�I�𒆂̒[���@�� [" & sTermMachineId & "] �ɂ����āA�K�p�҂��̃v���O������K�p���܂�...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Continue For
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            If ApplyMadoPro(sContextDir, sTermMachineId) = True Then
                Dim sTermCodeInFileName As String = GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId)
                Dim oVerInfoParams As Object() = { _
                    &H87, _
                    "P_Y_" & sTermCodeInFileName & "VER.DAT", _
                    Path.Combine(sContextDir, "MadoProVerInfo_" & sTermCodeInFileName & ".dat"), _
                    "", _
                    0, _
                    60000, _
                    60000, _
                    60000, _
                    0, _
                    3, _
                    True}
                SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
            End If
        Next oIdSet
    End Sub

    Private Sub ProSweepButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProSweepButton.Click
        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] �̑����v���O�����􂢑ւ����s���܂�...")
                Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)
                SweepMonitorMachineHoldingPrograms(sMonitorMachineId, oMonitorMachine)
                Log.Info(sMonitorMachineId, "�����v���O�����􂢑ւ����I�����܂����B")
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
                'TODO: bd.Args�̌����i0�̏ꍇ��Nothing�ł��邱�Ɓj���`�F�b�N����B
                Case "CreateConStatus".ToUpperInvariant()
                    isProcCompleted = CreateConStatus(sContextDir, sResult)
                Case "CreateMadoMasVerInfo".ToUpperInvariant()
                    isProcCompleted = CreateMadoMasVerInfo(sContextDir, bd.Args(0))
                Case "CreateMadoProVerInfo".ToUpperInvariant()
                    isProcCompleted = CreateMadoProVerInfo(sContextDir, bd.Args(0))
                Case "ClearMadoMas".ToUpperInvariant()
                    isProcCompleted = ClearMadoMas(sContextDir, bd.Args(0))
                Case "AcceptMadoMas".ToUpperInvariant()
                    isProcCompleted = AcceptMadoMas(sContextDir, sResult)
                Case "DeliverMadoMas".ToUpperInvariant()
                    isProcCompleted = DeliverMadoMas(sContextDir, bd.Args(0))
                Case "SweepMadoMas".ToUpperInvariant()
                    isProcCompleted = SweepMadoMas(sContextDir)
                Case "DirectInstallMadoPro".ToUpperInvariant()
                    isProcCompleted = DirectInstallMadoPro(sContextDir, bd.Args(0), bd.Args(1))
                Case "AcceptMadoPro".ToUpperInvariant()
                    isProcCompleted = AcceptMadoPro(sContextDir, sResult)
                Case "DeliverMadoPro".ToUpperInvariant()
                    isProcCompleted = DeliverMadoPro(sContextDir, bd.Args(0))
                Case "ApplyMadoPro".ToUpperInvariant()
                    isProcCompleted = ApplyMadoPro(sContextDir, bd.Args(0))
                Case "SweepMadoPro".ToUpperInvariant()
                    isProcCompleted = SweepMadoPro(sContextDir)
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

    Protected Function CreateStringOfContinuousPassiveDllReq(ByVal slot1Ver As Integer, ByVal slot2Ver As Integer, ByVal fullFlag As Integer, ByVal sContinueCode As String) As String
        Return sContinueCode & ";" & slot1Ver.ToString() & ";" & slot2Ver.ToString() & ";" & fullFlag.ToString()
    End Function

    Protected Sub CreateFileOfMadoMasVerInfo(ByVal sMonitorMachineId As String, ByVal sTermId As String, ByVal oTermMachine As TermMachine, ByVal sContextDir As String)
        Dim sFileName As String = _
           "MadoMasVerInfo_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & _
           ".dat"
        Dim sFilePath As String = Path.Combine(sContextDir, sFileName)
        Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
            'NOTE: �����̃}�X�^�o�[�W�������ɂ́A���D�@�̃}�X�^�o�[�W�������ƈقȂ�A
            '���ʂȊ�{�w�b�_���͑��݂��Ȃ��B
            'TODO: �����̓C���^�t�F�[�X�d�l�����ʉ��������B
            'ExVersionInfoFileHeader.WriteToStream(&H8B, GetEkCodeOf(sTermId), DateTime.Now, 1, oOutputStream)
            ExMasterVersionInfo.WriteToStream(oTermMachine.HoldingMasters, oOutputStream)
        End Using
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")
    End Sub

    Protected Sub CreateFileOfMadoProVerInfo(ByVal sMonitorMachineId As String, ByVal sTermId As String, ByVal oTermMachine As TermMachine, ByVal sContextDir As String)
        Dim ar As Integer = DirectCast(oTermMachine.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
        Dim sFileName As String = _
           "MadoProVerInfo_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & _
           ".dat"
        Dim sFilePath As String = Path.Combine(sContextDir, sFileName)
        Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
            'NOTE: �����̃v���O�����o�[�W�������ɂ́A���D�@�̃v���O�����o�[�W�������ƈقȂ�A
            '���ʂȊ�{�w�b�_���͑��݂��Ȃ��B
            'TODO: �����̓C���^�t�F�[�X�d�l�����ʉ��������B
            'ExVersionInfoFileHeader.WriteToStream(&H87, GetEkCodeOf(sTermId), DateTime.Now, 1, oOutputStream)
            ExProgramVersionInfoForY.WriteToStream(oTermMachine.HoldingPrograms(0), oOutputStream, ar)
            ExProgramVersionInfoForY.WriteToStream(oTermMachine.HoldingPrograms(1), oOutputStream, 0)
        End Using
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")
    End Sub

    Protected Sub CreateFileOfMadoMasDlReflectReq( _
       ByVal objCode As Byte, _
       ByVal subObjCode As Byte, _
       ByVal subKind As Integer, _
       ByVal version As Integer, _
       ByVal deliveryResult As Byte, _
       ByVal sMonitorMachineId As String, _
       ByVal sTermId As String, _
       ByVal sMachineDir As String)
        Dim oTeleg As New EkMasProDlReflectReqTelegram(TelegGene, objCode, subObjCode, subKind, version, GetEkCodeOf(sTermId), deliveryResult, 0)
        Dim sOddFileName As String = _
           "#MadoMasDlReflectReq_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & "_"
        Dim sOddFilePath As String = Path.Combine(sMachineDir, sOddFileName)

        Dim branchNum As Integer = -1
        Do
            branchNum += 1
            Dim sFilePath As String = sOddFilePath & branchNum.ToString() & ".dat"
            If File.Exists(sFilePath) Then Continue Do

            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oTeleg.WriteToStream(oOutputStream)
            End Using
            Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ��z�M���� [" & deliveryResult.ToString("X2") & "] �ō쐬���܂����B")
            Exit Do
        Loop
    End Sub

    Protected Sub CreateFileOfMadoProDlReflectReq( _
       ByVal objCode As Byte, _
       ByVal version As Integer, _
       ByVal deliveryResult As Byte, _
       ByVal sMonitorMachineId As String, _
       ByVal sTermId As String, _
       ByVal sMachineDir As String)
        Dim oTeleg As New EkMasProDlReflectReqTelegram(TelegGene, objCode, 0, 0, version, GetEkCodeOf(sTermId), deliveryResult, 0)
        Dim sOddFileName As String = _
           "#MadoProDlReflectReq_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & "_"
        Dim sOddFilePath As String = Path.Combine(sMachineDir, sOddFileName)

        Dim branchNum As Integer = -1
        Do
            branchNum += 1
            Dim sFilePath As String = sOddFilePath & branchNum.ToString() & ".dat"
            If File.Exists(sFilePath) Then Continue Do

            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oTeleg.WriteToStream(oOutputStream)
            End Using
            Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ��z�M���� [" & deliveryResult.ToString("X2") & "] �ō쐬���܂����B")
            Exit Do
        Loop
    End Sub

    Protected Function CreateConStatus(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine�ɐݒ肳��Ă��铝���ڑ���Ԃ�
        'oMonitorMachine.TermMachines�ɐݒ肳��Ă��鑋���ڑ���Ԃ����ƂɁA
        'sContextDir��ExtOutput.dat���쐬����B

        Dim oReqTeleg As EkByteArrayGetReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sContextDir, "ConStatusGetReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkByteArrayGetReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream))
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "�d�������ɕs��������܂��B")
            Return False
        End If

        Dim oBytes(4 + 15 * oMonitorMachine.TermMachines.Count - 1) As Byte
        Dim pos As Integer = 0

        oBytes(pos) = &H89
        pos += 1
        oBytes(pos) = oMonitorMachine.NegaStatus
        pos += 1
        oBytes(pos) = oMonitorMachine.MeisaiStatus
        pos += 1
        oBytes(pos) = oMonitorMachine.OnlineStatus
        pos += 1

        Dim oEnc As Encoding = Encoding.UTF8
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermId As String = oTermEntry.Key
            oEnc.GetBytes(GetStationOf(sTermId), 0, 6, oBytes, pos)
            pos += 6
            oEnc.GetBytes(GetCornerOf(sTermId), 0, 4, oBytes, pos)
            pos += 4
            oBytes(pos) = Byte.Parse(GetUnitOf(sTermId))
            pos += 1

            Dim oTerm As TermMachine = oTermEntry.Value
            oBytes(pos) = oTerm.DlsStatus
            pos += 1
            oBytes(pos) = oTerm.KsbStatus
            pos += 1
            oBytes(pos) = oTerm.Tk1Status
            pos += 1
            oBytes(pos) = oTerm.Tk2Status
            pos += 1
        Next oTermEntry

        Dim oAckTeleg As EkByteArrayGetAckTelegram = oReqTeleg.CreateAckTelegram(oBytes)

        Dim sFilePath As String = Path.Combine(sContextDir, "ExtOutput.dat")
        Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
            oAckTeleg.WriteToStream(oOutputStream)
        End Using
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")
        sResult = sFilePath

        Return True
    End Function

    Protected Function CreateMadoMasVerInfo(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        'oMonitorMachine.TermMachines�ɐݒ肳��Ă���}�X�^�ێ���Ԃ����ƂɁA
        'sContextDir�ɍ��@�ʂ�MadoMasVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�܂��A�ߋ��̂��̂�����Ώ����B

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            If oTermIdRegx.IsMatch(sTermMachineId) Then
                Dim sFileName As String = _
                   "MadoMasVerInfo_" & _
                   GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId) & _
                   ".dat"
                DeleteFiles(sMonitorMachineId, sContextDir, sFileName)

                CreateFileOfMadoMasVerInfo(sMonitorMachineId, sTermMachineId, oTermEntry.Value, sContextDir)
            End If
        Next oTermEntry

        Return True
    End Function

    Protected Function CreateMadoProVerInfo(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        'oMonitorMachine.TermMachines�ɐݒ肳��Ă��鑋�������v���O�����ێ���Ԃ����ƂɁA
        'sContextDir�ɍ��@�ʂ�MadoProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�܂��A�ߋ��̂��̂�����Ώ����B

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            If oTermIdRegx.IsMatch(sTermMachineId) Then
                Dim sFileName As String = _
                   "MadoProVerInfo_" & _
                   GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId) & _
                   ".dat"
                DeleteFiles(sMonitorMachineId, sContextDir, sFileName)

                CreateFileOfMadoProVerInfo(sMonitorMachineId, sTermMachineId, oTermEntry.Value, sContextDir)
            End If
        Next oTermEntry

        Return True
    End Function

    Protected Function ClearMadoMas(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            If oTermIdRegx.IsMatch(sTermMachineId) Then
                Dim sFileName As String = _
                   "MadoMasVerInfo_" & _
                   GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId) & _
                   ".dat"
                DeleteFiles(sMonitorMachineId, sContextDir, sFileName)

                Dim oTerm As TermMachine = oTermEntry.Value
                oTerm.HoldingMasters.Clear()
                oTerm.PendingMasters.Clear()
                CreateFileOfMadoMasVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
                UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
            End If
        Next oTermEntry

        Return True
    End Function

    Protected Function AcceptMadoMas(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'sMachineDir��#PassiveDllReq.dat�������t�@�C�������Ƃ�
        '�������ێ����鑋�������}�X�^�ioMonitorMachine.HoldingMasters�j��
        '�����ւ̔z�M�҂��}�X�^�ioTermMachine.PendingMasters�j��ǉ����A
        'sContextDir��ExtOutput.dat���쐬����B
        '�������A�f�[�^�ɉ��炩�ُ̈킪����ꍇ�́A
        '�������s�킸�ɁAContinueCode��
        'FinishWithoutStoring��ExtOutput.dat���쐬����B

        'NOTE: ContinueCode��Finish��ExtOutput.dat���쐬�����ꍇ�́A
        'DL�����ʒm���쐬���Ȃ���΂Ȃ�Ȃ��B����ɂ��ẮA
        '�}�X�^�K�p���X�g�ɋL�ڂ��ꂽ����(t)�̃}�X�^�ێ����
        '�ioMonitorMachine.TermMachines(t).HoldingMasters�j�����̏��
        '�X�V���邱�Ƃɂ�����ŁADL�����ʒm�����̏��sContextDir��
        '�쐬����̂��ȒP�ł��邪�A�����܂œ͂��Ă��Ȃ����Ԃ�
        '�Č��������̂ŁA���̃A�v����DeliverMadoMas������p�ӂ��A
        '�V�~�����[�^�{�̂��炻�̏�����v�����ꂽ�ۂɁA������
        '�}�X�^�ێ���Ԃ��X�V��������Ƃ���B

        'NOTE: �u�ŏI��M���i�f�[�^�{�́j��z�M�������Ă��Ȃ��ꍇ�ɁA�}�X�^�o�[�W����
        '��p�^�[���ԍ�������ƈقȂ��M���i�f�[�^�{�̂�K�p���X�g�j���󂯓���Ȃ��v
        '�悤�ɓ���������������Ƃ��Ă��A�L���[�ɂ́A�}�X�^�o�[�W������p�^�[���ԍ�
        '������̂��̂���������Ƃ͌���Ȃ��B��̓I�ɂ́A�O���Ƀ}�X�^�o�[�W������
        '�p�^�[���ԍ����P����O�̂��̂��i�P�܂��͕����j����A�㔼�Ƀ}�X�^�o�[�W����
        '�ƃp�^�[���ԍ����ŐV�̂��̂��i�P�܂��͕����j����ꍇ������͂��ł���B
        '�֑��ł��邪�A���̏ꍇ�A�������g���㔼�̂��̂��󂯓���Ă��邱�Ƃ���A�O����
        '���̂Ɠ����}�X�^�o�[�W��������уp�^�[���ԍ����t�^���ꂽ�f�[�^�{�̂�S������
        '�z�M�ς݂ł���ƌ�����B�܂�A�e�L���[�ɂ�����O���̏��́A���Y������
        '�΂��ăf�[�^�{�̂�z�M������������ɁA�����}�X�^�o�[�W�����E�����p�^�[���ԍ���
        '�f�[�^�{�̂܂��͓K�p���X�g����M�����ۂɁA�ł������̂ł���ƌ����؂��B

        Dim d As DateTime = DateTime.Now

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "�d�������ɕs��������܂��B")
            Return False
        End If

        Log.Info(sMonitorMachineId, "�K�p���X�g�̃t�@�C������ [" & oReqTeleg.ListFileName & "] �ł��B")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "�}�X�^�{�̂̃t�@�C������ [" & oReqTeleg.DataFileName & "] �ł��B")
        Else
            Log.Info(sMonitorMachineId, "�}�X�^�{�̂̃t�@�C�����͂���܂���B")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "�K�p���X�g�̃t�@�C�������s���ł��B")
            Return False
        End If

        Dim sApplicableModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim dataSubKind As Integer = EkMasProListFileName.GetDataSubKindAsInt(sListFileName)
        Dim dataVersion As Integer = EkMasProListFileName.GetDataVersionAsInt(sListFileName)
        Dim listVersion As Integer = EkMasProListFileName.GetListVersionAsInt(sListFileName)

        If oReqTeleg.DataFileName.Length <> 0 Then
            Dim sDataFileName As String = Path.GetFileName(oReqTeleg.DataFileName)
            If Not EkMasterDataFileName.IsValid(sDataFileName) Then
                Log.Error(sMonitorMachineId, "�}�X�^�{�̂̃t�@�C�������s���ł��B")
                Return False
            End If
            If EkMasterDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j�ɕs����������܂��B")
                Return False
            End If
            If EkMasterDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�}�X�^��ʁj�ɕs����������܂��B")
                Return False
            End If
            If EkMasterDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�p�^�[��No�j�ɕs����������܂��B")
                Return False
            End If
            If EkMasterDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�}�X�^Ver�j�ɕs����������܂��B")
                Return False
            End If
        End If

        'NOTE: ��{�I�ɁAoReqTeleg��SubObjCode��ListFileName�̐������́A�V�i���I��
        '�ŕۏ؂���z��ł���B�܂��ADataFileName����łȂ��ꍇ��ListFileName�Ƃ�
        '�������ɂ��Ă��A�V�i���I���ŕۏ؂���z��ł���B�V�i���I���łȂ�A
        '�������̂Ȃ�REQ�ɑ΂��ANAK��ԐM����悤�ɐݒ�ł��邽�߂ł���B
        '�������A���Ƃ��A�V�i���I���Ń`�F�b�N���Ȃ��Ƃ��Ă��A���������A�������̂Ȃ�
        '�z�M���s���Ă��܂����Ȃ�A����͉^�ǃV�X�e���̕s��̂͂��ł��邩��A
        '�V�i���I�����{����NG�ŏI����������悢�B�ȏ�̂��Ƃ���A�����ł�
        '�������̃`�F�b�N���s���A���������Ȃ��ꍇ�́A�ے艞����ԋp����B
        If sApplicableModel <> Config.TermModelSym Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j���s���ł��B")
            Return False
        End If
        If Not ExConstants.MadoMastersSubObjCodes.ContainsKey(sDataKind) Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�}�X�^��ʁj���s���ł��B")
            Return False
        End If
        If oReqTeleg.SubObjCode <> ExConstants.MadoMastersSubObjCodes(sDataKind) Then
            Log.Error(sMonitorMachineId, "�d���̃T�u��ʂ��t�@�C�����i�}�X�^��ʁj�Ɛ������Ă��܂���B")
            Return False
        End If

        Dim oHoldingMasters As List(Of HoldingMaster) = Nothing
        oMonitorMachine.HoldingMasters.TryGetValue(sDataKind, oHoldingMasters)

        Dim sDataHashValue As String = Nothing
        Dim dataAcceptDate As DateTime
        Dim oDataFooter As Byte() = Nothing

        '�������ێ����Ă��钆����A�K�p���X�g�Ƒg�ݍ��킹�邱�Ƃ��ł���}�X�^�{�̂�T���B
        'NOTE: �g�ݍ��킹�邱�Ƃ��ł���}�X�^�{�̂��Ȃ��Ƃ��́AsDataHashValue��Nothing�ɂȂ�B
        If oHoldingMasters IsNot Nothing Then
            For Each oMas As HoldingMaster In oHoldingMasters
                If oMas.DataSubKind = dataSubKind AndAlso _
                   oMas.DataVersion = dataVersion AndAlso _
                   oMas.DataHashValue IsNot Nothing Then
                    If sDataHashValue Is Nothing OrElse _
                       dataAcceptDate < oMas.DataAcceptDate Then
                        sDataHashValue = oMas.DataHashValue
                        dataAcceptDate = oMas.DataAcceptDate
                        oDataFooter = oMas.DataFooter
                    End If
                End If
            Next oMas
        End If

        If oReqTeleg.DataFileName.Length = 0 Then
            '�������ێ����Ă��Ȃ��}�X�^�Ɋւ��āA�K�p���X�g�݂̂𑗂�t����ꂽ�ꍇ�́A
            'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            'NOTE: ���̃P�[�X�Ŗ{���̓������ǂ̂悤�Ȕ������������́A�������Ă��Ȃ��B
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "�K�p���X�g�ɕR�Â��}�X�^�{�̂�����܂���B")
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            'NOTE: �{���̓����́A���̂悤�Ƀ}�X�^�{�̂���M�����ꍇ�ł��A���̎��_�̃V�X�e��������
            '�����}�X�^���ߋ��Ɏ�M�����ۂ̃V�X�e�����������Â������ɂȂ��Ă���ꍇ�́A
            '�����M�����}�X�^�{�̂𑋏��ւ�DLL�Ώۂɂ͂��Ȃ��i�����M�������̂��܂߂āA
            '��M�������ł��V�������̂�DLL�ΏۂƔF�����A���������ɂ�����������Ă���Ȃ�
            'DLL�͍s��Ȃ��Ǝv����j�B�������A�������ɂ���̓C���M�����[�ȃP�[�X�ł���
            '���߁A�V�~�����[�^�Ŗ����ɍČ��͂��Ȃ����Ƃɂ���B

            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
            dataAcceptDate = d

            '�}�X�^�{�̂̃t�b�^����ǎ��B
            oDataFooter = New Byte(ExMasterDataFooter.Length - 1) {}
            Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
            Try
                Using oInputStream As New FileStream(sDataFilePath, FileMode.Open, FileAccess.Read)
                    oInputStream.Seek(-ExMasterDataFooter.Length, SeekOrigin.End)
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(oDataFooter, pos, ExMasterDataFooter.Length - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "�}�X�^�{�̂̃t�b�^���̓ǎ��ŗ�O���������܂����B", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End Try

            '�ǎ�����t�b�^���ɖ�肪����ꍇ�́A
            'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            Dim footerView As New ExMasterDataFooter(oDataFooter)
            Dim sViolation As String = footerView.GetFormatViolation()
            If sViolation IsNot nothing Then
                Log.Error(sMonitorMachineId, "�}�X�^�{�̂̃t�b�^��񂪈ُ�ł��B" & vbCrLf & sViolation)
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End If
        End If

        Dim sListHashValue As String = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        '�K�p���X�g�̓��e����͂���B
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, Integer)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            '�Ƃ肠�����A�S�ēǂݎ��B
            'OPT: �u�K�p���X�g�̓��e��\������@�\�v��ǉ������ۂɁA�����葁��
            '�������邽�߂ɁA���̂悤�ɓ�x�ǂ݂��邱�ƂɂȂ��Ă���B
            '������ł���A���P�̗]�n������B
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '�P�s���ǂݎ��B
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '�ȉ��A�K�p���X�g�̓��e���`�F�b�N���A��肪����ꍇ�́A
                'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B

                '�w�b�_���̂P�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g����ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂P�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g1�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�쐬�N�������`�F�b�N����B
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�쐬�N�������s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '���X�gVer���`�F�b�N����B
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ���X�gVer���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g��2�s�ڂ�����܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g2�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�p�^�[��No���`�F�b�N����B
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�p�^�[��No���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�}�X�^Ver���`�F�b�N����B
                If Not dataVersion.ToString("D3").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�}�X�^Ver���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�@��R�[�h���`�F�b�N����B
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�@�킪�t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�K�p���X�g�̂R�s�ڈȍ~����AoMonitorMachine�Ɋ֌W���鍆�@�𒊏o����B
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '�ǂݍ��񂾍s���ɕ�������B
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 3 Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��ڐ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�T�C�o�l����w���R�[�h�̏������`�F�b�N����B
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̉w�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�R�[�i�[�R�[�h�̏������`�F�b�N����B
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̃R�[�i�[�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '���@�ԍ��̏������`�F�b�N����B
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��@�ԍ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�s�̏d�����`�F�b�N����B
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ����o�̍s�Əd�����Ă��܂��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�s�̓��e���ꎞ�ۑ�����B
                    oListedMachines.Add(sLineKey, 0)

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�K�p���X�g�̓ǎ��ŗ�O���������܂����B", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
            Return True
        End Try

        '�����̍s��z�M�҂��̍s�́ADLL�v�����̂��̂�\�������킩��₷���̂ŁA
        'DLL�v���Ƀ}�X�^�{�̂��܂܂�Ă��Ȃ��ꍇ�́A�����Ŗ����l�ɍ����ւ��邱�Ƃɂ���B
        'NOTE: ���ۂ̓����ŁA���̂悤��DLL�v�����󂯂��ꍇ�ɁA������DLL����}�X�^�{�̂́A
        '�v�����󂯂����_�Ō��߂�킯�ł͂Ȃ��A�����ւ�DLL���s�����_�Ō��߂�i���̎��_��
        '�ŐV�̂��̂�DLL����j�悤�ł��邽�߁A���̂悤�ɕ\������͎̂��Ԃɍ����Ă���B
        'TODO: �������̂悤�ɕ\�������̂����Ȃ�A�������R�����g�A�E�g����΂悢�B
        If oReqTeleg.DataFileName.Length = 0 Then
            dataAcceptDate = Config.EmptyTime
            oDataFooter = Nothing
            sDataHashValue = Nothing
        End If

        '�ꎞ�ۑ����Ă����s�������e�@��ɁA�z�M�̂��߂̏����L���[�C���O����B
        Dim targetTermCount As Integer = 0
        For Each sName As String In oListedMachines.Keys
            '�s��oMonitorMachine�Ɋ֌W����ꍇ
            Dim oTerm As TermMachine = Nothing
            If oMonitorMachine.TermMachines.TryGetValue(sName, oTerm) = True Then
                Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�[�� [" & sName & "] �̍s���L���[�C���O���܂��B")
                Dim oQueue As LinkedList(Of PendingMaster) = Nothing
                If oTerm.PendingMasters.TryGetValue(sDataKind, oQueue) = False Then
                    oQueue = New LinkedList(Of PendingMaster)()
                    oTerm.PendingMasters.Add(sDataKind, oQueue)
                End If
                Dim oPenMas As New PendingMaster()
                oPenMas.DataSubKind = dataSubKind
                oPenMas.DataVersion = dataVersion
                oPenMas.ListVersion = listVersion
                oPenMas.DataAcceptDate = dataAcceptDate
                oPenMas.DataFooter = oDataFooter
                oPenMas.DataHashValue = sDataHashValue
                oPenMas.ListAcceptDate = listAcceptDate
                oPenMas.ListContent = sListContent
                oPenMas.ListHashValue = sListHashValue
                oQueue.AddLast(oPenMas)
                UpdateTable2OnTermStateChanged(sMonitorMachineId, sName, oTerm)
                targetTermCount += 1
            End If
        Next sName
        Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ" & oListedMachines.Count.ToString() & "��̂����A" & targetTermCount.ToString() & "�䂪���Y�@��̒[���ł����B")

        'NOTE: ���L�̃P�[�X�ŁA�{���̓������ǂ̂悤�Ȕ������������́A�悭�킩��Ȃ��B
        If targetTermCount = 0 Then
            Log.Error(sMonitorMachineId, "�z�M�𐶂ݏo���Ȃ��K�p���X�g����M���܂����B")
            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
            Return True
        End If

        '�����̃}�X�^�ێ���Ԃ��X�V����B
        If oHoldingMasters Is Nothing Then
            oHoldingMasters = New List(Of HoldingMaster)()
            oMonitorMachine.HoldingMasters.Add(sDataKind, oHoldingMasters)
        End If
        Dim oNewMas As New HoldingMaster()
        oNewMas.DataSubKind = dataSubKind
        oNewMas.DataVersion = dataVersion
        oNewMas.ListVersion = listVersion
        oNewMas.DataAcceptDate = dataAcceptDate
        oNewMas.DataFooter = oDataFooter
        oNewMas.DataHashValue = sDataHashValue
        oNewMas.ListAcceptDate = listAcceptDate
        oNewMas.ListContent = sListContent
        oNewMas.ListHashValue = sListHashValue
        oHoldingMasters.Add(oNewMas)
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        Log.Info(sMonitorMachineId, "�󂯓��ꂪ�������܂����B")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverMadoMas(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        '�w�肳�ꂽ�����̃L���[����S�Ẵ}�X�^�K�p���X�g�����o���A
        '�}�X�^�K�p���X�g���ƂɁA���Y����(t)�̃}�X�^�ێ����
        '�ioMonitorMachine.TermMachines(t).HoldingMasters�j��
        '�X�V���AsMachineDir�Ƀ}�X�^�K�p���X�g�ʁE�����ʂ�
        '#MadoMasDlReflectReq_RRRSSSCCCCUU_N.dat�iN��0�`�j���쐬����B
        '�܂��A�}�X�^�ێ���Ԃ��X�V���������ɂ��ẮA
        'sContextDir��MadoMasVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AMadoMasDlReflectReq_RRRSSSCCCCUU_N.dat��
        'MadoMasVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����̔z�M�Ɩ��֌W�Ȃ��́A����X�V
        '���Ă��Ȃ������̂��́j���폜����B

        'NOTE: �����̃t�@�C������RRRSSSCCCCUU�͒[���̋@��R�[�h��
        '���邪�A�����̃t�@�C�����S�[�����i�V�i���I����
        '�u%T3R%T3S%T4C%T2U�v�ƋL�q�����ꍇ�ɕ��������S�s���j
        '�쐬�����Ƃ͌���Ȃ��B
        '����āA�V�i���I�́A���Y�t�@�C���𑗐M����ہA
        'ActiveOne�ł͂Ȃ��ATryActiveOne���g�p����B

        Dim d As DateTime = DateTime.Now

        Dim oUpdatedKinds As New HashSet(Of String)

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            If oTermIdRegx.IsMatch(sTermMachineId) Then
                Dim sFileName As String = _
                   "MadoMasVerInfo_" & _
                   GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId) & _
                   ".dat"
                DeleteFiles(sMonitorMachineId, sContextDir, sFileName)

                Dim oTerm As TermMachine = oTermEntry.Value

                Dim existsAnyProc As Boolean = False
                For Each oPendingList As LinkedList(Of PendingMaster) In oTerm.PendingMasters.Values
                    If oPendingList.Count <> 0 Then
                        existsAnyProc = True
                        Exit For
                    End If
                Next oPendingList
                If Not existsAnyProc Then
                    Log.Debug(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɑ΂���}�X�^�z�M�͂���܂���B")
                    Continue For
                ElseIf oTerm.Tk2Status <> &H2 Then
                    Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ��ẮA����DL�n��Ԃ��ڑ��ȊO�ɐݒ肳��Ă��邽�߁A�z�M������ۗ����܂��B")
                    Continue For
                End If

                Dim isHoldingMasUpdated As Boolean = False

                '�}�X�^��ʂ��Ƃɏ������s���B
                For Each oKindEntry As KeyValuePair(Of String, LinkedList(Of PendingMaster)) In oTerm.PendingMasters
                    'TODO: �{���̓����́A�Ō�ɃL���[�C���O�����v���ɑΉ�����z�M�����s��Ȃ���������Ȃ��B
                    '���m�ɂ́A�u���[�U�̈ӎv�݂̂ɉ����āi�^�ǂ���̗v���݂̂��g���K�[�Ɂj�[���ւ̔z�M���s���A
                    '��莞�ԓ��Ɍ��ʁiDL�����ʒm�j�����[�U�ɒ񎦂���i���̏�Œ[���ɔz�M�ł��Ȃ��Ȃ�A���Y
                    '�[���ւ̔z�M�͒��߂āA�ُ��DL�����ʒm�𔭐�������j�v�v�z����߂āA�Ď��ՂƓ����悤�ɁA
                    '�[���ւ̔z�M��ۗ�����i�[���Ƃ̉����Ԃɂ���āA�\���ł��Ȃ��^�C�~���O��DL�����ʒm��
                    '�グ����j�悤�ɂȂ��Ă��܂������A�����I�ɂ͕ۗ��ɂ������̑S�Ă�z�M����킯�ɂ��������A
                    '�^�ǂ���̗v�����}�[�W���āA�[�����Ƃɉ���ێ����Ă����Ԃɂ���ׂ������i�킴�킴���p�@
                    '�̒��Ɂj�Ǘ����A���ۂ̒[���̕ێ���ԂƂ̍�����A�K�v�Ȃ��̂�����[���ɔz�M���銴����
                    '�Ȃ��Ă��邩������Ȃ��B
                    '�Ď��Ղւ̕��݊��Ƃ����ϓ_�ŁA�ň����̂��Ǝ��͎̂d���Ȃ��Ƃ��Ă��A�[���ɑ��M����
                    '�t�@�C���Ɋ֘A����DL�����ʒm�������������Ȃ��\��������i�󂯕t�����v�������s���Ȃ�
                    '�̂Ȃ�A���̎|��\������ȊO��DL�����ʒm�𔭐�������΂悢�͂��ł��邪...�j�B
                    '�����������Ƃ�����A�^�ǂɑ΂��铭�������@�ƃV�~�����[�^�ňႤ... �Ƃ������ƂɂȂ���
                    '���܂��̂ŁA�V�~�����[�^�ł��A�Ō�ɃL���[�C���O����Ă�����̈ȊO��ǂݎ̂Ă�i���j
                    '�Ȃǂ̓���ɂ��������悢��������Ȃ��i�� ���ۂ́A�����ƕ��G�Ǝv����j�B
                    '�Ȃ��A���Ƃ���������ɂ��Ă��ATermMachine�N���X��PendingMasters�͕K�v�ł���B
                    '�V�~�����[�^�̋@�\�Ƃ��āA�����ɖ��z�M�̂��̂����[�U�Ɏ����K�v�����邽�߂ł���B
                    'TODO: �����A��L�����悤�Ȏ���������A�����ǔF���邵���Ȃ��Ȃ�A�^�Ǒ��́A
                    'DL�����ʒm����M�����ہA����Ɠ�����ʂ́i�o�[�W�������͈قȂ�j�}�X�^��
                    '����Ɠ����[���ւ̔z�M��ԂŁu���M���v�̂��̂�����΁u�X�L�b�v�v���ɕύX����
                    '�Ȃǂ̑Ή�������ƁA�����܂��ȕ����ɖ߂��邩������Ȃ��B

                    '�z�M�����Ɏg���Ă��Ȃ��S�K�p���X�g�ɂ��ď������s���B
                    For Each oPenMas As PendingMaster In oKindEntry.Value
                        'NOTE: ���̂̂Ȃ��iListHashValue Is Nothing �́j�K�p���X�g�Ŕz�M���s����\���͑z�肵�Ȃ��B
                        Log.Info(sMonitorMachineId, "�K�p���X�g [" & oPenMas.ListVersion.ToString() & "] �Ɋ�Â��A�[�� [" & sTermMachineId & "] �ɑ΂����� [" & oKindEntry.Key & "] �p�^�[��No [" & oPenMas.DataSubKind.ToString() & "] �}�X�^Ver [" & oPenMas.DataVersion.ToString() & "] �̃}�X�^�z�M�������s���܂�...")

                        '�z�M���ʁi�u����v�u�K�p�ς݁v�Ȃǁj�����߂�B
                        Dim deliveryResult As Byte = &H0
                        Dim isOutOfArea As Boolean = False

                        Dim oLatestMas As HoldingMaster = FindLatestMasterDataInMonitorMachine(oMonitorMachine, oKindEntry.Key, oPenMas.DataSubKind, oPenMas.DataVersion)
                        If oLatestMas Is Nothing Then
                            'TODO: ��{�I�ɂ��蓾�Ȃ��͂��̏󋵂ł��邪�A�{���̓����ɍ��킹�����B
                            Log.Warn(sMonitorMachineId, "�z��O�̏󋵂ł��B�z�M���Ȃ���΂Ȃ�Ȃ��}�X�^�{�̂������ɂ���܂���B")
                            deliveryResult = &H5 'NOTE: �K���ȃR�[�h���Ȃ��̂ŁA�Ƃ肠��������ȊO�ɂ��Ă����B
                        End If

                        If deliveryResult = &H0 Then
                            Dim ar As Integer = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
                            Dim oMasKinds As HashSet(Of String) = Nothing
                            If ExConstants.MadoAreasMasters.TryGetValue(ar, oMasKinds) = False OrElse _
                               Not oMasKinds.Contains(oKindEntry.Key) Then
                                'NOTE: �z�M���Ȃ��̂Ɂu�K�p�ς݁v�͕s�K�؂Ɏv���邪�A�Ď��Ձ����D�@�͎��ۂɂ��̂悤�ɓ��삷��B
                                'TODO: �����������������ł���Ƃ͌���Ȃ��̂Ŋm�F����B
                                Log.Error(sMonitorMachineId, "���̎�ʂ̃}�X�^�̓G���A [" & ar.ToString() &"] �̒[���ɂ͔z�M�ł��܂���B")
                                deliveryResult = &HF
                                isOutOfArea = True
                            End If
                        End If

                        If deliveryResult = &H0 Then
                            'TODO: �{���̓����ɍ��킹�����B
                            '�{���̓����́A�u�K�p�ς݁v�Ή��̍ۂɁA�󂯓�������ł͂Ȃ��A�n�b�V���l�Ȃǂ��r����悤�ɂȂ�����������Ȃ��B
                            '�Ď��Ղ̏ꍇ�͂ǂ��Ȃ̂����܂߁A���z�`���m�F����ׂ��ł���B
                            Dim oMas As HoldingMaster = Nothing
                            If oTerm.HoldingMasters.TryGetValue(oKindEntry.Key, oMas) = True AndAlso _
                               oMas.DataSubKind = oPenMas.DataSubKind AndAlso _
                               oMas.DataVersion = oPenMas.DataVersion AndAlso _
                               oMas.DataAcceptDate = oLatestMas.DataAcceptDate Then
                                '�������ێ����Ă�����̂Ɠ������̂�z�M���邱�ƂɂȂ�ꍇ�́A
                                '�z�M���ʂ��u�K�p�ς݁v�Ƃ���B
                                Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��Ă͓��Y�}�X�^��K�p�ς݁i���z�M�ς݁j�ł��B�Ĕz�M�͍s���܂���B")
                                deliveryResult = &HF
                            End If
                        End If

                        '�����̃}�X�^�ێ���Ԃ��X�V����B
                        If deliveryResult = &H0 OrElse isOutOfArea Then
                            'NOTE: �����͓K�p���X�g��ێ����Ȃ����A�ǂ̓K�p���X�g�̎w���ɂ����
                            '���Y�����Ƀ}�X�^�{�̂̔z�M���s��ꂽ��������������悢�̂ŁA
                            '�K�p���X�g�o�[�W�������Z�b�g���邱�Ƃɂ���B
                            Dim oNewMas As New HoldingMaster()
                            oNewMas.DataSubKind = oPenMas.DataSubKind
                            oNewMas.DataVersion = oPenMas.DataVersion
                            oNewMas.ListVersion = oPenMas.ListVersion
                            oNewMas.DataAcceptDate = oLatestMas.DataAcceptDate
                            oNewMas.DataDeliverDate = d
                            oNewMas.DataFooter = oLatestMas.DataFooter
                            oNewMas.DataHashValue = oLatestMas.DataHashValue
                            oNewMas.ListAcceptDate = oPenMas.ListAcceptDate
                            oNewMas.ListContent = oPenMas.ListContent
                            oNewMas.ListHashValue = oPenMas.ListHashValue
                            oTerm.HoldingMasters(oKindEntry.Key) = oNewMas
                            isHoldingMasUpdated = True
                            If deliveryResult = &H0 Then
                                Log.Info(sMonitorMachineId, "���Y�[���ɑ΂��ē��Y�}�X�^�̔z�M���s���܂����B")
                            Else
                                Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��ē��Y�}�X�^�̔z�M���s���܂����B����͎������D�@�V�X�e���̐�������o�[�W���������Č������邽�߂̓��ʑ[�u�ł��̂ŁA�����ӂ��������B")
                            End If
                        End If

                        '#MadoMasDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
                        CreateFileOfMadoMasDlReflectReq( _
                           &H74, _
                           ExConstants.MadoMastersSubObjCodes(oKindEntry.Key), _
                           oPenMas.DataSubKind, _
                           oPenMas.DataVersion, _
                           deliveryResult, _
                           sMonitorMachineId, _
                           sTermMachineId, _
                           sMachineDir)

                        oUpdatedKinds.Add(oKindEntry.Key)
                    Next oPenMas
                Next oKindEntry
                oTerm.PendingMasters.Clear()

                If isHoldingMasUpdated Then
                    CreateFileOfMadoMasVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
                End If

                UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
            End If
        Next oTermEntry

        '�����̑����}�X�^�ێ���Ԃ��X�V����B
        TrimMonitorMachineHoldingMasters(sMonitorMachineId, oMonitorMachine, oUpdatedKinds)

        Return True
    End Function

    Protected Function SweepMadoMas(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        SweepMonitorMachineHoldingMasters(sMonitorMachineId, oMonitorMachine)
        Return True
    End Function

    Protected Function DirectInstallMadoPro(ByVal sContextDir As String, ByVal sTermIdRegx As String, ByVal sFilePath As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(sFilePath)
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�v���O�����{�̂̃n�b�V���l�Z�o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim content As MadoProgramContent
        Try
            content = ExtractMadoProgramCab(sFilePath, Path.Combine(sContextDir, "MadoPro"))
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�o�[�W�������X�g����̃G���ANo�̒��o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�o�[�W�������X�g����̑�\Ver�̒��o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn(sMonitorMachineId, "�v���O�����̓��싖�����������ɐݒ肳��Ă��܂��B" & content.RunnableDate & "�܂œK�p�ł��܂���̂ł����ӂ��������B")
        End If

        For Each sTermId As String In oMonitorMachine.TermMachines.Keys
            If oTermIdRegx.IsMatch(sTermId) Then
                InstallMadoProgramDirectly(sContextDir, sTermId, subKind, version, content, sHashValue)
            End If
        Next sTermId

        Return True
    End Function

    Protected Function AcceptMadoPro(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'sMachineDir��#PassiveDllReq.dat�������t�@�C�������Ƃ�
        '�������ێ����鑋�������v���O�����ioMonitorMachine.HoldingPrograms�j�ƁA
        '�����ւ̔z�M�҂��v���O�����ioTermMachine.PendingMasters�j��ǉ����A
        'sContextDir��ExtOutput.dat���쐬����B
        '�������A�f�[�^�ɉ��炩�ُ̈킪����ꍇ�́A
        '�������s�킸�ɁAContinueCode��
        'FinishWithoutStoring��ExtOutput.dat���쐬����B

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "�d�������ɕs��������܂��B")
            Return False
        End If

        Log.Info(sMonitorMachineId, "�K�p���X�g�̃t�@�C������ [" & oReqTeleg.ListFileName & "] �ł��B")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "�v���O�����{�̂̃t�@�C������ [" & oReqTeleg.DataFileName & "] �ł��B")
        Else
            Log.Info(sMonitorMachineId, "�v���O�����{�̂̃t�@�C�����͂���܂���B")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "�K�p���X�g�̃t�@�C�������s���ł��B")
            Return False
        End If

        Dim sApplicableModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim dataSubKind As Integer = EkMasProListFileName.GetDataSubKindAsInt(sListFileName)
        Dim dataVersion As Integer = EkMasProListFileName.GetDataVersionAsInt(sListFileName)
        Dim listVersion As Integer = EkMasProListFileName.GetListVersionAsInt(sListFileName)

        If oReqTeleg.DataFileName.Length <> 0 Then
            Dim sDataFileName As String = Path.GetFileName(oReqTeleg.DataFileName)
            If Not EkProgramDataFileName.IsValid(sDataFileName) Then
                Log.Error(sMonitorMachineId, "�v���O�����{�̂̃t�@�C�������s���ł��B")
                Return False
            End If
            If EkProgramDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�v���O������ʁj�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�G���ANo�j�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i��\Ver�j�ɕs����������܂��B")
                Return False
            End If
        End If

        'NOTE: ��{�I�ɁAoReqTeleg��SubObjCode��ListFileName�̐������́A�V�i���I��
        '�ŕۏ؂���z��ł���B�܂��ADataFileName����łȂ��ꍇ��ListFileName�Ƃ�
        '�������ɂ��Ă��A�V�i���I���ŕۏ؂���z��ł���B�V�i���I���łȂ�A
        '�������̂Ȃ�REQ�ɑ΂��ANAK��ԐM����悤�ɐݒ�ł��邽�߂ł���B
        '�������A���Ƃ��A�V�i���I���Ń`�F�b�N���Ȃ��Ƃ��Ă��A���������A�������̂Ȃ�
        '�z�M���s���Ă��܂����Ȃ�A����͉^�ǃV�X�e���̕s��̂͂��ł��邩��A
        '�V�i���I�����{����NG�ŏI����������悢�B�ȏ�̂��Ƃ���A�����ł�
        '�������̃`�F�b�N���s���A���������Ȃ��ꍇ�́A�ے艞����ԋp����B
        If sApplicableModel <> Config.TermModelSym Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j���s���ł��B")
            Return False
        End If
        If sDataKind <> "YPG" Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�v���O������ʁj���s���ł��B")
            Return False
        End If
        If oReqTeleg.SubObjCode <> &H0 Then
            Log.Error(sMonitorMachineId, "�d���̃T�u��ʂ��s���ł��B")
            Return False
        End If

        Dim sDataHashValue As String = Nothing
        Dim dataAcceptDate As DateTime
        Dim sRunnableDate As String = Nothing
        Dim sArchiveCatalog As String = Nothing
        Dim oVersionListData As Byte() = Nothing

        '�������ێ����Ă��钆����A�K�p���X�g�Ƒg�ݍ��킹�邱�Ƃ��ł���v���O�����{�̂�T���B
        'NOTE: �g�ݍ��킹�邱�Ƃ��ł���v���O�����{�̂��Ȃ��Ƃ��́AsDataHashValue��Nothing�ɂȂ�B
        For Each oPro As HoldingProgram In oMonitorMachine.HoldingPrograms
            If oPro.DataSubKind = dataSubKind AndAlso _
               oPro.DataVersion = dataVersion AndAlso _
               oPro.DataHashValue IsNot Nothing Then
                If sDataHashValue Is Nothing OrElse _
                   dataAcceptDate < oPro.DataAcceptDate Then
                    sDataHashValue = oPro.DataHashValue
                    dataAcceptDate = oPro.DataAcceptDate
                    sRunnableDate = oPro.RunnableDate
                    sArchiveCatalog = oPro.ArchiveCatalog
                    oVersionListData = oPro.VersionListData
                End If
            End If
        Next oPro

        If oReqTeleg.DataFileName.Length = 0 Then
            '�������ێ����Ă��Ȃ��o�[�W�����̑��������v���O�����Ɋւ��āA�K�p���X�g�݂̂�
            '����t����ꂽ�ꍇ�́AContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            'NOTE: ���̃P�[�X�Ŗ{���̓������ǂ̂悤�Ȕ������������́A�������Ă��Ȃ��B
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "�K�p���X�g�ɕR�Â��v���O�����{�̂�����܂���B")
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            'NOTE: �{���̓����́A���̂悤�Ƀv���O�����{�̂���M�����ꍇ�ł��A���̎��_�̃V�X�e��������
            '�����v���O�������ߋ��Ɏ�M�����ۂ̃V�X�e�����������Â������ɂȂ��Ă���ꍇ�́A
            '�����M�����v���O�����{�̂𑋏��ւ�DLL�Ώۂɂ͂��Ȃ��i�����M�������̂��܂߂āA
            '��M�������ł��V�������̂�DLL�ΏۂƔF�����A���������ɂ�����������Ă���Ȃ�
            'DLL�͍s��Ȃ��Ǝv����j�B�������A�������ɂ���̓C���M�����[�ȃP�[�X�ł���
            '���߁A�V�~�����[�^�Ŗ����ɍČ��͂��Ȃ����Ƃɂ���B

            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
            dataAcceptDate = d

            Dim content As MadoProgramContent
            Try
                Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
                content = ExtractMadoProgramCab(sDataFilePath, Path.Combine(sContextDir, "MadoPro"))
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End Try

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData).Equals(dataSubKind.ToString("X2")) Then
                'NOTE: CAB�̒��g���m�F�ł�������e�؂Ȃ̂ŁA���̂܂܏��������s����B
                'TODO: �{���̓����̓���ɍ��킹�������悢�H
                Log.Warn(sMonitorMachineId, "�o�[�W�������X�g�ɋL�ڂ��ꂽ�G���ANo���t�@�C�����Ɛ������Ă��܂��񂪁A���������s���܂��B")
            End If

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", "").Equals(dataVersion.ToString("D8")) Then
                'NOTE: CAB�̒��g���m�F�ł�������e�؂Ȃ̂ŁA���̂܂܏��������s����B
                'TODO: �{���̓����̓���ɍ��킹�������悢�H
                Log.Warn(sMonitorMachineId, "�o�[�W�������X�g�ɋL�ڂ��ꂽ��\Ver���t�@�C�����Ɛ������Ă��܂��񂪁A���������s���܂��B")
            End If

            sRunnableDate = content.RunnableDate
            sArchiveCatalog = content.ArchiveCatalog
            oVersionListData = content.VersionListData
        End If

        Dim sListHashValue As String = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        'TODO: �e�����ɂ��āA�K�p�ʂ̃o�[�W�����Ƒҋ@�ʂ̃o�[�W�����𒲂ׁA
        '�ҋ@�ʂɏ������݉\�Ƃ���o�[�W�����𐧌�����ׂ���������Ȃ��B

        '�K�p���X�g�̓��e����͂���B
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, String)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            '�Ƃ肠�����A�S�ēǂݎ��B
            'OPT: �u�K�p���X�g�̓��e��\������@�\�v��ǉ������ۂɁA�����葁��
            '�������邽�߂ɁA���̂悤�ɓ�x�ǂ݂��邱�ƂɂȂ��Ă���B
            '������ł���A���P�̗]�n������B
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '�P�s���ǂݎ��B
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '�ȉ��A�K�p���X�g�̓��e���`�F�b�N���A��肪����ꍇ�́A
                'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B

                '�w�b�_���̂P�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g����ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂P�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g1�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�쐬�N�������`�F�b�N����B
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�쐬�N�������s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '���X�gVer���`�F�b�N����B
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ���X�gVer���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g��2�s�ڂ�����܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g2�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�G���ANo���`�F�b�N����B
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�G���ANo���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '��\Ver���`�F�b�N����B
                If Not dataVersion.ToString("D8").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ��\Ver���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�@��R�[�h���`�F�b�N����B
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�@�킪�t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�K�p���X�g�̂R�s�ڈȍ~����AoMonitorMachine�Ɋ֌W���鍆�@�𒊏o����B
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '�ǂݍ��񂾍s���ɕ�������B
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 4 Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��ڐ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�T�C�o�l����w���R�[�h�̏������`�F�b�N����B
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̉w�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�R�[�i�[�R�[�h�̏������`�F�b�N����B
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̃R�[�i�[�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '���@�ԍ��̏������`�F�b�N����B
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��@�ԍ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�K�p���̃����O�X���`�F�b�N����B
                    If aColumns(3).Length <> 8 AndAlso aColumns(3).Length <> 0 Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�K�p�����u�����N�łȂ��ꍇ�A�l���`�F�b�N����B
                    If aColumns(3).Length = 8 Then
                       If Not Utility.IsDecimalStringFixed(aColumns(3), 0, 8) Then
                            Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                            Return True
                        End If

                        Dim applicableDate As DateTime
                        If Not aColumns(3).Equals("99999999") AndAlso _
                           DateTime.TryParseExact(aColumns(3), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, applicableDate) = False Then
                            Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    '�s�̏d�����`�F�b�N����B
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ����o�̍s�Əd�����Ă��܂��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�s�̓��e���ꎞ�ۑ�����B
                    oListedMachines.Add(sLineKey, aColumns(3))

                    '�s��oMonitorMachine�Ɋ֌W����ꍇ
                    'TODO: �����Ń`�F�b�N����DLL�v���V�[�P���X�����s������̂ł͂Ȃ��A
                    '���@�ʂ�DL�����ʒm���o��������ׂ���������Ȃ��B
                    Dim oTerm As TermMachine = Nothing
                    If oMonitorMachine.TermMachines.TryGetValue(sLineKey, oTerm) = True Then
                        '�G���A�ԍ����`�F�b�N����B
                        'NOTE: �Ď��ՂƈႢ�A�����œK�p���X�g�̑S���@���܂Ƃ߂Ĉ����Ӗ��͂Ȃ��̂ŁA
                        '�����Ń`�F�b�N�͍s�킸�ADeliverMadoPro�ɂāA�K�p�捆�@���Ƃ�
                        '�K�p�G���A�ُ��DL�����ʒm�𔭐������邱�Ƃɂ��Ă���i�������Ȃ���
                        '�K�p�G���A�ُ�̎g�������Ȃ��j�B
                        'TODO: �{���̓����ɍ��킹��B
                        'TODO: �����̃v���O�����ɃG���A�ԍ�0���w�肳��邱�Ƃ͂Ȃ��i�ُ펖�ԁj��������Ȃ��B
                        'If dataSubKind <> 0 AndAlso _
                        '   dataSubKind <> DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer) Then
                        '    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�[�� [" & sLineKey & "] �̏����G���A���A�K�p���X�g�̑ΏۃG���A�ƈقȂ�܂��B")
                        '    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        '    Return True
                        'End If
                    End If

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�K�p���X�g�̓ǎ��ŗ�O���������܂����B", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
            Return True
        End Try

        '�����̍s��z�M�҂��̍s�́ADLL�v�����̂��̂�\�������킩��₷���̂ŁA
        'DLL�v���Ƀv���O�����{�̂��܂܂�Ă��Ȃ��ꍇ�́A�����Ŗ����l�ɍ����ւ��邱�Ƃɂ���B
        'NOTE: ���ۂ̓����ŁA���̂悤��DLL�v�����󂯂��ꍇ�ɁA������DLL����v���O�����{�̂́A
        '�v�����󂯂����_�Ō��߂�킯�ł͂Ȃ��A�����ւ�DLL���s�����_�Ō��߂�i���̎��_��
        '�ŐV�̂��̂�DLL����j�悤�ł��邽�߁A���̂悤�ɕ\������͎̂��Ԃɍ����Ă���B
        'TODO: �������̂悤�ɕ\�������̂����Ȃ�A�������R�����g�A�E�g����΂悢�B
        If oReqTeleg.DataFileName.Length = 0 Then
            dataAcceptDate = Config.EmptyTime
            sRunnableDate = Nothing
            sArchiveCatalog = Nothing
            oVersionListData = Nothing
            sDataHashValue = Nothing
        End If

        '�ꎞ�ۑ����Ă����s�������e�@��ɁA�z�M�̂��߂̏����L���[�C���O����B
        Dim targetTermCount As Integer = 0
        Dim targetTermFullCount As Integer = 0
        For Each oApplyEntry As KeyValuePair(Of String, String) In oListedMachines
            '�s��oMonitorMachine�Ɋ֌W����ꍇ
            Dim oTerm As TermMachine = Nothing
            If oMonitorMachine.TermMachines.TryGetValue(oApplyEntry.Key, oTerm) = True Then
                '�K�p�������݂̉^�p���t�Ɠ������������邢�́u19000101�v���u99999999�v�̏ꍇ�̂݁A
                '�z�M���i���̃A�v���̏ꍇ�́ADL�����ʒm���j�K�v�Ƃ݂Ȃ��B
                'NOTE: �ŐV�̊Ď��Ձi���@�j�́A���̏����ɊY�����Ă��Ȃ��s�ɂ��āA�u�K�p�ς݁v��DL�����ʒm��
                '����t����悤�ɂȂ��Ă����C������i���̂��߂ɁA�^�Ǒ��́A���Ɂu����v�ɂȂ��Ă���ꍇ��
                '�u�K�p�ς݁v��DL�����ʒm�𖳎����Ȃ���΂Ȃ�Ȃ��Ȃ����j�B�����i���@�j�������ł���\��������B
                '���̎v�z���炷��ƁA �����i���@�j�́A���������K�p�����ߋ����̍s�ł����Ă��A���Y�s�̑�����
                '���Y�v���O�����𖢔z�M�ł���΁A�z�M���Ă��܂��̂�������Ȃ��B
                '�����������Ƃ���ƁA���Ȃ���ł���B
                '�^�ǂ́A�K�p�����ߋ����̍s�́A�K�p�����u�����N�̍s�Ɠ��������ɂ��邱�ƂɂȂ��Ă���B
                '����䂦�ɁA�K�p���X�g�ɂ��̂悤�ȍs�����Ȃ���΁A�����ɑ΂��Ĕz�M���Ȃ��B
                '�܂��ADLL�V�[�P���X�����������ہi�����܂Ŕz�M�����������ہj���A���̂悤�ȓK�p����
                '�L�ڂ���Ă��鑋���ɂ��ẮA�z�M��Ԃ��u�z�M���v�ɂ͂��Ȃ��B
                '�^�ǂ̓����I/F�d�l�i�c�[���d�l���̕ʎ�6�j�Ɋ��S�ɍ��v���Ă���B
                'TODO: �{���̊Ď��Ղ��u�K�p�ς݁v�𑗂�t���Ă��錏�ɂ��āA�V�X�e�������ł́A�����
                '��������悤�ɉ^�Ǒ����������A�Ď��Ճ`�[���̍l����d�l�ʂ�Ƃ������Ƃ�OK�Ƃ������A
                '�Ď��Ղⓝ���̎������ǂ��Ȃ��Ă���̂��A�V�X�e���Ƃ��Ė�肪�Ȃ��̂��A�V�X�e��������
                '���{���Ă��Ȃ��P�[�X�i�K�p�����ߋ����̍s�̉��D�@�ɑ΂��Ė��z�M�������P�[�X���j��
                '���Ă��A�^�ǂ̓�����܂߂āA���؂���ׂ��ł���B
                If oApplyEntry.Value.Length = 8 AndAlso _
                  (oApplyEntry.Value.Equals("19000101") OrElse _
                   String.CompareOrdinal(oApplyEntry.Value, sServiceDate) >= 0) Then
                    Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�[�� [" & oApplyEntry.Key & "] �K�p�� [" & oApplyEntry.Value & "] �̍s���L���[�C���O���܂��B")
                    Dim oPenPro As New PendingProgram()
                    oPenPro.DataSubKind = dataSubKind
                    oPenPro.DataVersion = dataVersion
                    oPenPro.ListVersion = listVersion
                    oPenPro.DataAcceptDate = dataAcceptDate
                    oPenPro.RunnableDate = sRunnableDate
                    oPenPro.ArchiveCatalog = sArchiveCatalog
                    oPenPro.VersionListData = oVersionListData
                    oPenPro.DataHashValue = sDataHashValue
                    oPenPro.ListAcceptDate = listAcceptDate
                    oPenPro.ApplicableDate = oApplyEntry.Value
                    oPenPro.ListContent = sListContent
                    oPenPro.ListHashValue = sListHashValue
                    oTerm.PendingPrograms.AddLast(oPenPro)
                    UpdateTable2OnTermStateChanged(sMonitorMachineId, oApplyEntry.Key, oTerm)
                    targetTermCount += 1
                Else
                    Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�[�� [" & oApplyEntry.Key & "] �K�p�� [" & oApplyEntry.Value & "] �̍s�͏��O���܂��B")
                End If
                targetTermFullCount += 1
            End If
        Next oApplyEntry
        Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ" & oListedMachines.Count.ToString() & "��̂����A" & targetTermFullCount.ToString() & "�䂪���Y�@��̒[���ł����B���̂���" & targetTermCount.ToString() & "��̓K�p�����L���ł����B")

        'NOTE: ���L�̃P�[�X�ŁA�{���̓������ǂ̂悤�Ȕ������������́A�悭�킩��Ȃ��B
        If targetTermCount = 0 Then
            Log.Error(sMonitorMachineId, "�z�M�𐶂ݏo���Ȃ��K�p���X�g����M���܂����B")
            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
            Return True
        End If

        '�����̑��������v���O�����ێ���Ԃ��X�V����B
        Dim oNewPro As New HoldingProgram()
        oNewPro.DataSubKind = dataSubKind
        oNewPro.DataVersion = dataVersion
        oNewPro.ListVersion = listVersion
        oNewPro.DataAcceptDate = dataAcceptDate
        oNewPro.RunnableDate = sRunnableDate
        oNewPro.ArchiveCatalog = sArchiveCatalog
        oNewPro.VersionListData = oVersionListData
        oNewPro.DataHashValue = sDataHashValue
        oNewPro.ListAcceptDate = listAcceptDate
        oNewPro.ApplicableDate = Nothing
        oNewPro.ListContent = sListContent
        oNewPro.ListHashValue = sListHashValue
        oMonitorMachine.HoldingPrograms.Add(oNewPro)
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        Log.Info(sMonitorMachineId, "�󂯓��ꂪ�������܂����B")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverMadoPro(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        '�w�肳�ꂽ�����̃L���[����S�Ẵv���O�����K�p���X�g�����o���A
        '�v���O�����K�p���X�g���ƂɁA���Y����(t)�̃v���O�����ێ����
        '�ioMonitorMachine.TermMachines(t).HoldingPrograms�j��
        '�X�V���AsMachineDir�ɓK�p���X�g�ʁE�����ʂ�
        '#MadoProDlReflectReq_RRRSSSCCCCUU_N.dat�iN��0�`�j���쐬����B
        '�܂��A�v���O�����ێ���Ԃ��X�V���������ɂ��ẮA
        'sContextDir��MadoProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AMadoProDlReflectReq_RRRSSSCCCCUU_N.dat��
        'MadoProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����̔z�M�Ɩ��֌W�Ȃ��́A����X�V
        '���Ă��Ȃ������̂��́j���폜����B

        'NOTE: �����̃t�@�C������RRRSSSCCCCUU�͒[���̋@��R�[�h��
        '���邪�A�����̃t�@�C�����S�[�����i�V�i���I����
        '�u%T3R%T3S%T4C%T2U�v�ƋL�q�����ꍇ�ɕ��������S�s���j
        '�쐬�����Ƃ͌���Ȃ��B
        '����āA�V�i���I�́A���Y�t�@�C���𑗐M����ہA
        'ActiveOne�ł͂Ȃ��ATryActiveOne���g�p����B

        Dim d As DateTime = DateTime.Now

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            If oTermIdRegx.IsMatch(sTermMachineId) Then
                Dim sFileName As String = _
                   "MadoProVerInfo_" & _
                   GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId) & _
                   ".dat"
                DeleteFiles(sMonitorMachineId, sContextDir, sFileName)

                Dim oTerm As TermMachine = oTermEntry.Value

                If oTerm.PendingPrograms.Count = 0 Then
                    Log.Debug(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɑ΂���v���O�����z�M�͂���܂���B")
                    Continue For
                ElseIf oTerm.Tk2Status <> &H2 Then
                    Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ��ẮA����DL�n��Ԃ��ڑ��ȊO�ɐݒ肳��Ă��邽�߁A�z�M������ۗ����܂��B")
                    Continue For
                End If

                Dim isHoldingProUpdated As Boolean = False

                'TODO: �{���̓����́A�Ō�ɃL���[�C���O�����v���ɑΉ�����z�M�����s��Ȃ���������Ȃ��B
                '���m�ɂ́A�u���[�U�̈ӎv�݂̂ɉ����āi�^�ǂ���̗v���݂̂��g���K�[�Ɂj�[���ւ̔z�M���s���A
                '��莞�ԓ��Ɍ��ʁiDL�����ʒm�j�����[�U�ɒ񎦂���i���̏�Œ[���ɔz�M�ł��Ȃ��Ȃ�A���Y
                '�[���ւ̔z�M�͒��߂āA�ُ��DL�����ʒm�𔭐�������j�v�v�z����߂āA�Ď��ՂƓ����悤�ɁA
                '�[���ւ̔z�M��ۗ�����i�[���Ƃ̉����Ԃɂ���āA�\���ł��Ȃ��^�C�~���O��DL�����ʒm��
                '�グ����j�悤�ɂȂ��Ă��܂������A�����I�ɂ͕ۗ��ɂ������̑S�Ă�z�M����킯�ɂ��������A
                '�^�ǂ���̗v�����}�[�W���āA�[�����Ƃɉ���ێ����Ă����Ԃɂ���ׂ������i�킴�킴���p�@
                '�̒��Ɂj�Ǘ����A���ۂ̒[���̕ێ���ԂƂ̍�����A�K�v�Ȃ��̂�����[���ɔz�M���銴����
                '�Ȃ��Ă��邩������Ȃ��B
                '�Ď��Ղւ̕��݊��Ƃ����ϓ_�ŁA�ň����̂��Ǝ��͎̂d���Ȃ��Ƃ��Ă��A�[���ɑ��M����
                '�t�@�C���Ɋ֘A����DL�����ʒm�������������Ȃ��\��������i�󂯕t�����v�������s���Ȃ�
                '�̂Ȃ�A���̎|��\������ȊO��DL�����ʒm�𔭐�������΂悢�͂��ł��邪...�j�B
                '�����������Ƃ�����A�^�ǂɑ΂��铭�������@�ƃV�~�����[�^�ňႤ... �Ƃ������ƂɂȂ���
                '���܂��̂ŁA�V�~�����[�^�ł��A�Ō�ɃL���[�C���O����Ă�����̈ȊO��ǂݎ̂Ă�i���j
                '�Ȃǂ̓���ɂ��������悢��������Ȃ��i�� ���ۂ́A�����ƕ��G�Ǝv����j�B
                '�Ȃ��A���Ƃ���������ɂ��Ă��ATermMachine�N���X��PendingPrograms�͕K�v�ł���B
                '�V�~�����[�^�̋@�\�Ƃ��āA�����ɖ��z�M�̂��̂����[�U�Ɏ����K�v�����邽�߂ł���B
                'TODO: �����A��L�����悤�Ȏ���������A�����ǔF���邵���Ȃ��Ȃ�A�^�Ǒ��́A
                'DL�����ʒm����M�����ہA����Ɠ�����ʂ́i�o�[�W�������͈قȂ�j�v���O������
                '����Ɠ����[���ւ̔z�M��ԂŁu���M���v�̂��̂�����΁u�X�L�b�v�v���ɕύX����
                '�Ȃǂ̑Ή�������ƁA�����܂��ȕ����ɖ߂��邩������Ȃ��B

                '�z�M�����Ɏg���Ă��Ȃ��S�K�p���X�g�ɂ��ď������s���B
                For Each oPenPro As PendingProgram In oTerm.PendingPrograms
                    'NOTE: ���̂̂Ȃ��iListHashValue Is Nothing �́j�K�p���X�g�Ŕz�M���s����\���͑z�肵�Ȃ��B
                    Log.Info(sMonitorMachineId, "�K�p���X�g [" & oPenPro.ListVersion.ToString() & "] �Ɋ�Â��A�[�� [" & sTermMachineId & "] �ɑ΂���G���ANo [" & oPenPro.DataSubKind.ToString() & "] ��\Ver [" & oPenPro.DataVersion.ToString() & "] �̃v���O�����z�M�������s���܂�...")

                    If oPenPro.ApplicableDate.Equals("99999999") Then
                        Log.Info(sMonitorMachineId, "�����Y�[���ɑ΂���v���͉������~�v���ł��B")
                    End If

                    '���Y�[���ɑ΂��čŌ�ɔz�M�����K�p���X�g�̏����擾����B
                    Dim latestDataSubKind As Integer = oTerm.HoldingPrograms(0).DataSubKind
                    Dim latestDataVersion As Integer = oTerm.HoldingPrograms(0).DataVersion
                    Dim latestListVersion As Integer = oTerm.HoldingPrograms(0).ListVersion
                    'Dim latestListAcceptDate As Integer = oTerm.HoldingPrograms(0).ListAcceptDate
                    Dim latestListHashValue As String = oTerm.HoldingPrograms(0).ListHashValue
                    If oTerm.HoldingPrograms(1) IsNot Nothing Then
                        latestDataSubKind = oTerm.HoldingPrograms(1).DataSubKind
                        latestDataVersion = oTerm.HoldingPrograms(1).DataVersion
                        latestListVersion = oTerm.HoldingPrograms(1).ListVersion
                        'latestListAcceptDate = oTerm.HoldingPrograms(1).ListAcceptDate
                        latestListHashValue = oTerm.HoldingPrograms(1).ListHashValue
                    End If

                    '�K�p���X�g�̔z�M���ʁi�u����v�܂��́u�ُ�v�u�K�p�ς݁v�j�����߂�B
                    Dim listDeliveryResult As Byte = &H0

                    'NOTE: �{���̓����͓K�p���X�g�̃n�b�V���l�ł͂Ȃ��A�K�p���X�g�̓���������
                    '��r���邩������Ȃ����A�������Ƃ���ƁA�K�p���X�g�Ɋւ���u�K�p�ς݁v��
                    'DL�����ʒm�͎����㔭�����Ȃ����ƂɂȂ邽�߁A�Ƃ肠�����n�b�V���l���r����
                    '���Ƃɂ��Ă���B
                    'TODO: �{���̓����ɍ��킹��B
                    If latestListHashValue IsNot Nothing AndAlso _
                       oPenPro.DataSubKind = latestDataSubKind AndAlso _
                       oPenPro.DataVersion = latestDataVersion AndAlso _
                       oPenPro.ListVersion = latestListVersion AndAlso _
                       StringComparer.OrdinalIgnoreCase.Compare(oPenPro.ListHashValue, latestListHashValue) = 0 Then
                        '�u�������K�p�҂��̕��ނƈꏏ�ɕێ����Ă���K�p���X�g�v�Ɠ������̂�z�M����
                        '���ƂɂȂ�ꍇ�́A�z�M���ʂ��u�K�p�ς݁v�Ƃ���B
                        'NOTE: ���̃P�[�X�ł́A�������~�v���̓K�p���X�g�ɑ΂��Ă��u�K�p�ς݁i�������~�ς݁H�j�v
                        '�ōς܂��Ă��܂����A�{���̓����������ł��邩�͕s���B���������A�O��̔z�M����
                        '�������~�����Ă���Ƃ�����A�ҋ@�ʂ�������Ă���͂��Ȃ̂ŁA���ʂɂ͂��蓾�Ȃ�
                        '�P�[�X�Ǝv����B
                        If oPenPro.ApplicableDate.Equals("99999999") Then
                            Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��Ă͓��Y�K�p���X�g��z�M�ς݂ł��B�K�p���X�g�̍Ĕz�M���s���܂���̂ŁA�������~���s���܂���B")
                        Else
                            Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��Ă͓��Y�K�p���X�g��z�M�ς݂ł��B�K�p���X�g�̍Ĕz�M�͍s���܂���B�K�p���X�g�Ɋ�Â��v���O�����{�̂̔z�M���s���܂���B")
                        End If
                        listDeliveryResult = &HF
                    End If

                    If listDeliveryResult = &H0 Then
                        If oPenPro.ApplicableDate.Equals("99999999") Then
                            'NOTE: oPenPro�̃o�[�W�����̃v���O���������ɓK�p���ɂȂ��Ă���P�[�X��
                            '�ȉ��̃P�[�X�i�����ȉ������~�v���j�ɓ��Ă͂܂�͂��ł���B
                            'NOTE: �uoTerm.HoldingPrograms(1) Is Nothing�v�łȂ��ꍇ�ɂ����ẮA
                            '������uoTerm.HoldingPrograms(1).DataVersion = 0�v�ł���Ƃ��Ă��A
                            '����́A�[�����o�[�W����0�̃v���O������ێ����Ă���Ƃ������Ƃł���B
                            '����āAoPenPro.DataVersion��0�ł���A�G���A�ԍ�����v����Ȃ�A
                            'oPenPro�͗L���ȉ������~�ł���A�ȉ��̏������U�ɂȂ��Ă悢�B
                            If oTerm.HoldingPrograms(1) Is Nothing OrElse _
                               oTerm.HoldingPrograms(1).DataSubKind <> oPenPro.DataSubKind OrElse _
                               oTerm.HoldingPrograms(1).DataVersion <> oPenPro.DataVersion Then
                                'NOTE: �{���̓��������̂悤�Ɍ����ȓ��������̂��́A�s���ł���B
                                Log.Error(sMonitorMachineId, "�����ȉ������~�v���ł��B���Y�[���ɂ����ē��Y�v���O�������K�p�҂��ɂȂ��Ă��܂���B")
                                listDeliveryResult = &H1
                            End If
                        End If
                    End If

                    'NOTE: �K�p���X�g���K�p�ς݂̏ꍇ��A�������~�v���̓K�p���X�g�������ȏꍇ�́A
                    '�v���O�����{�̂�DL�����ʒm�͔��������Ȃ��B�����̃P�[�X�ł́A
                    '�v���O�����{�͔̂z�M�ΏۂłȂ��͂��ł���A���Ȃ��͂��B
                    '�������~�̍s���܂ޓK�p���X�g���v���O�����{�̂ƂƂ���DLL�����P�[�X�͑z�肵�Ȃ��B
                    'TODO: �{���̓������������ǂ��������͕�����Ȃ��B
                    If listDeliveryResult = &H0 Then
                        If oPenPro.ApplicableDate.Equals("99999999") Then
                            'NOTE: �K�p�����u99999999�v�̍s�̑����ɂ��ẮA�v���O�����{�̂�DL�����ʒm��
                            '�i�K�p�ς݂Ȃǂ��܂߂āj�������Ȃ����Ƃɂ���B�{���̓������ǂ��Ȃ̂��͕s���B
                            'TODO: �^�ǂɂ����āA���鑋���ɑ΂��邠��o�[�W�����̃v���O�����̏���̔z�M�w���ŁA
                            '�K�p���X�g�Ɂu99999999�v���L�ڂ��Ă��܂�����A�u99999999�v���L�ڂ��ꂽ�K�p���X�g��
                            '�z�M���s���ۂɁu�v���O����+�v���O�����K�p���X�g �����z�M�v�Ƀ`�F�b�N������
                            '���܂����肷��ƁA�v���O�����{�̂Ɋւ��铖�Y�����̎�M��Ԃ��u�z�M���v�ɂȂ�A
                            '���ꂪ���̂܂܎c���Ă��܂��Ǝv����B����ɂ��ẮA�K�p���u99999999�v���w��
                            '���ꂽ�����ɂ��āu�z�M���v�̃��R�[�h���쐬���Ȃ��悤�ɁA�����āA�ł��邱��
                            '�Ȃ�u99999999�v���L�ڂ��ꂽ�K�p���X�g�Łu�v���O����+�v���O�����K�p���X�g �����z�M�v
                            '���w��ł��Ȃ��悤�ɁA�^�ǂ̎��������P����ׂ��ł���B

                            '�����̃v���O�����ێ���Ԃ��X�V����B
                            oTerm.HoldingPrograms(1) = Nothing
                            isHoldingProUpdated = True
                            Log.Info(sMonitorMachineId, "���Y�[���ɑ΂��ĉ������~���s���܂����B")
                        Else
                            'NOTE: ���Ƃ��K�p���X�g�Ɋւ���DL�����ʒm���u�K�p�ς݁v�ł������Ƃ��Ă��A
                            '�v���O�����{�̂�DL�����ʒm����������i�K�p���X�g�Ɋւ���u�K�p�ς݁v�Ȃ�
                            '�Ƃ����T�O���������܂ꂽ���ƂŁA��a�������邩������Ȃ����A�^�ǂ����
                            'DLL�v���ɂ́A�K�p���X�g�̃o�[�W�����ȂǂɊ֌W�Ȃ��A�ʂɈӖ�������j�B
                            '�܂��A���Ƃ��K�p���X�g���K�p�ς݁i= ���ۂ́A�P�Ȃ鑗�M�ς݁j�ł������Ƃ��Ă��A
                            '���̓K�p���X�g�ɂ����āA���Y�v���O�������K�p�̑����ɗL�ӂȓK�p�����L�ڂ����
                            '����΁A�v���O�����{�̂ɂ��Ắu�K�p�ς݁v�ł͂Ȃ��u����v��DL�����ʒm��
                            '��������B
                            'TODO: �{���̓����́A�K�p���X�g�Ɋւ���DL�����ʒm���u�K�p�ς݁v�ł���ꍇ�ɁA
                            '�v���O�����{�̂�DL�����ʒm�i�����炭�u�K�p�ς݁v�j�𐶐����Ȃ���������Ȃ��B
                            '���̏󋵂ł́A�^�ǂɂ����铖�Y�����̓��Y�v���O�����̎�M��Ԃ��u�z�M���v
                            '�ł͂Ȃ��u����v���ɂȂ��Ă���Ǝv���邪�A�{���ɂ��̕ۏ؂�����̂�
                            '���؂��������悢�B
                            'TODO: ���̃A�v���ł́A�K�p�ς݂��ۂ��𔻒f�����ŁA�o�[�W�������̑��ɓ�������
                            '���r���Ă��邪�A�{���̓����ɍ��킹�����B����������ƁA�{���̓����i�����j�ł�
                            '���������ł͂Ȃ��n�b�V���l�Ȃǂ��r����悤�Ɏv�z�����߂��Ă���\��������B

                            '�v���O�����{�̂̔z�M���ʁi�u����v�܂��́u�K�p�ς݁v�j�����߂�B
                            Dim dataDeliveryResult As Byte = &H0

                            '��������K�p���X�g�Ɠ��G���A�E����\�o�[�W�����̓����ێ��̑����v���O�����{�̂̒��ŁA
                            '�ł��V�������̂�T���B
                            Dim oLatestPro As HoldingProgram = FindLatestProgramDataInMonitorMachine(oMonitorMachine, oPenPro.DataSubKind, oPenPro.DataVersion)
                            If oLatestPro Is Nothing Then
                                'TODO: ��{�I�ɂ��蓾�Ȃ��͂��̏󋵂ł��邪�A�{���̓����ɍ��킹�����B
                                Log.Warn(sMonitorMachineId, "�z��O�̏󋵂ł��B�z�M���Ȃ���΂Ȃ�Ȃ��v���O�����{�̂������ɂ���܂���B")
                                dataDeliveryResult = &HB 'NOTE: �Ӗ����Ⴄ��������Ȃ����A�Ƃ肠��������ȊO�ɂ��Ă����B
                                listDeliveryResult = &H1 'TODO: �K�p���X�g�̔z�M���ʂ͂R��ނ����Ȃ��B�{���͓K�p���X�g��z�M����̂�������Ȃ��B
                            End If

                            If dataDeliveryResult = &H0 Then
                                Dim ar As Integer = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
                                'TODO: �{���̓����������́A�t�@�C������K�p���X�g���ɋL�ڂ���Ă���G���A�ԍ����`�F�b�N���Ȃ���������Ȃ��B
                                'TODO: �����̃v���O�����ɃG���A�ԍ�0���w�肳��邱�Ƃ͂Ȃ��i�ُ펖�ԁj��������Ȃ��̂ŁA
                                '�O���̏����͗]�v��������Ȃ��B�B
                                If oPenPro.DataSubKind <> 0 AndAlso oPenPro.DataSubKind <> ar Then
                                    Log.Error(sMonitorMachineId, "���Y�G���ANo�̃v���O�����̓G���A [" & ar.ToString() &"] �̒[���ɂ͔z�M�ł��܂���B")
                                    dataDeliveryResult = &H2
                                    listDeliveryResult = &H1 'TODO: �K�p���X�g�̔z�M���ʂ͂R��ނ����Ȃ��B�{���͓K�p���X�g��z�M����̂�������Ȃ��B
                                End If
                            End If

                            If dataDeliveryResult = &H0 Then
                                If oTerm.HoldingPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                                   oTerm.HoldingPrograms(0).DataVersion = oPenPro.DataVersion Then
                                    '�������K�p���̂��̂Ɠ����o�[�W�����̃v���O�����𑋏��ɔz�M����ƁA���炩�̕s�s��������
                                    '��������Ȃ��̂ŁA�ُ툵���ɂ���B
                                    'NOTE: �K�p���O�̂��̂𑋏����K�p���Ă���͂��͂Ȃ����A�K�p�����߂������̂𑋏��ɔz�M���悤�Ƃ���
                                    '�͂����Ȃ��B�������A�������K�p���̂��̂Ɠ��o�[�W�����̃v���O�������A�K�p�������ɁA
                                    '��������M�����P�[�X��A�K�p���O�ɓ�������M���A�����ɔz�M���Ȃ��܂ܓK�p�����߂���
                                    '�P�[�X�Ȃǂ́A���蓾��B��҂͖{�֐����œK�p���Ɖ^�p�����r���āA�ʂُ̈툵���ɂ��邱�Ƃ�
                                    '�\�ł��邪�A�O�҂͂����͂����Ȃ��B
                                    'TODO: �Ƃ肠�����z�M���ʂ��u�K�p�ς݁v�Ƃ��邪�A�{���̓����ɍ��킹�������悢�B
                                    Log.Warn(sMonitorMachineId, "���Y�[���ɂ͓��o�[�W�����̃v���O������K�p�ς݂ł��B�v���O�����{�̂̍Ĕz�M�͍s���܂���B")
                                    dataDeliveryResult = &HF
                                End If
                            End If

                            If dataDeliveryResult = &H0 Then
                                'TODO: �{���̓����ɍ��킹�����B
                                '�{���̓����́A�u�K�p�ς݁v�Ή��̍ۂɁA�󂯓�������ł͂Ȃ��A�n�b�V���l�Ȃǂ��r����悤�ɂȂ�����������Ȃ��B
                                '�܂��A�Ď��Ղ̏ꍇ�͂ǂ��Ȃ̂����܂߁A���z�`���m�F����ׂ��ł���B
                                If oTerm.HoldingPrograms(1) IsNot Nothing AndAlso _
                                   oTerm.HoldingPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                                   oTerm.HoldingPrograms(1).DataVersion = oPenPro.DataVersion AndAlso _
                                   oTerm.HoldingPrograms(1).DataAcceptDate = oLatestPro.DataAcceptDate Then
                                    '�������K�p���̂��̂Ɠ������̂�z�M���邱�ƂɂȂ�ꍇ�́A
                                    '�z�M���ʂ��u�K�p�ς݁v�Ƃ���B
                                    Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��Ă͓��Y�v���O������K�p�ς݁i���z�M�ς݁j�ł��B�v���O�����{�̂̍Ĕz�M�͍s���܂���B")
                                    dataDeliveryResult = &HF
                                End If
                            End If

                            If dataDeliveryResult = &H0 Then
                                If Not oPenPro.ApplicableDate.Equals("19000101") AndAlso _
                                   String.CompareOrdinal(oPenPro.ApplicableDate, oPenPro.RunnableDate) < 0 Then
                                    Log.Error(sMonitorMachineId, "�v���O�����̓��싖�����K�p���ȍ~�ɐݒ肳��Ă��܂��B�z�M�͍s���܂���B")
                                    dataDeliveryResult = &HC
                                    listDeliveryResult = &H1 'TODO: �K�p���X�g�̔z�M���ʂ͂R��ނ����Ȃ��B�{���͓K�p���X�g��z�M����̂�������Ȃ��B
                                End If
                            End If

                            '�����̃v���O�����ێ���Ԃ��X�V����B
                            If dataDeliveryResult = &H0 Then
                                Debug.Assert(listDeliveryResult = &H0)
                                Dim oPro As New HoldingProgram()
                                oPro.DataSubKind = oPenPro.DataSubKind
                                oPro.DataVersion = oPenPro.DataVersion
                                oPro.ListVersion = oPenPro.ListVersion
                                oPro.DataAcceptDate = oLatestPro.DataAcceptDate
                                oPro.DataDeliverDate = d
                                oPro.RunnableDate = oLatestPro.RunnableDate
                                oPro.ArchiveCatalog = oLatestPro.ArchiveCatalog
                                oPro.VersionListData = oLatestPro.VersionListData
                                oPro.DataHashValue = oLatestPro.DataHashValue
                                oPro.ListAcceptDate = oPenPro.ListAcceptDate
                                oPro.ListDeliverDate = d
                                oPro.ApplicableDate = oPenPro.ApplicableDate
                                oPro.ListContent = oPenPro.ListContent
                                oPro.ListHashValue = oPenPro.ListHashValue
                                oTerm.HoldingPrograms(1) = oPro
                                isHoldingProUpdated = True
                                Log.Info(sMonitorMachineId, "���Y�[���̑ҋ@�ʂɑ΂��ē��Y�v���O�����{�̂̔z�M���s���܂����B")
                                Log.Info(sMonitorMachineId, "���Y�[���̑ҋ@�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B")
                            ElseIf listDeliveryResult = &H0 Then
                                If oTerm.HoldingPrograms(1) IsNot Nothing AndAlso _
                                   oTerm.HoldingPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                                   oTerm.HoldingPrograms(1).DataVersion = oPenPro.DataVersion Then
                                    oTerm.HoldingPrograms(1).ListVersion = oPenPro.ListVersion
                                    oTerm.HoldingPrograms(1).ListAcceptDate = oPenPro.ListAcceptDate
                                    oTerm.HoldingPrograms(1).ListDeliverDate = d
                                    oTerm.HoldingPrograms(1).ApplicableDate = oPenPro.ApplicableDate
                                    oTerm.HoldingPrograms(1).ListContent = oPenPro.ListContent
                                    oTerm.HoldingPrograms(1).ListHashValue = oPenPro.ListHashValue
                                    isHoldingProUpdated = True
                                    Log.Info(sMonitorMachineId, "���Y�[���̑ҋ@�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B")
                                ElseIf oTerm.HoldingPrograms(0) IsNot Nothing AndAlso _
                                       oTerm.HoldingPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                                       oTerm.HoldingPrograms(0).DataVersion = oPenPro.DataVersion Then
                                    'NOTE: ���@�̑������������ǂ̂悤�ɓ��삷�邩�͕s���ł���B
                                    '�[���͑ҋ@�ʂɎ�M����d�l�̂͂��ł��邪�A�ҋ@�ʂւ̎�M��
                                    '��M�������X�g�̓K�p����x�ɍς܂����ƍl����΁A�����͂��B
                                    '�������A���̂悤�ɋ��ق���ɂ́AoTerm.HoldingPrograms(1) Is Nothing
                                    '�ł��邱�Ƃ������ɓ����ׂ��ł���B
                                    'TODO: ���@���܂߂āA����ׂ�������m�F����K�v������B
                                    oTerm.HoldingPrograms(0).ListVersion = oPenPro.ListVersion
                                    oTerm.HoldingPrograms(0).ListAcceptDate = oPenPro.ListAcceptDate
                                    oTerm.HoldingPrograms(0).ListDeliverDate = d
                                    oTerm.HoldingPrograms(0).ApplicableDate = oPenPro.ApplicableDate
                                    oTerm.HoldingPrograms(0).ListContent = oPenPro.ListContent
                                    oTerm.HoldingPrograms(0).ListHashValue = oPenPro.ListHashValue
                                    isHoldingProUpdated = True
                                    Log.Warn(sMonitorMachineId, "���Y�[���̓K�p�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B���̓K�p���͈Ӗ��������܂���̂Œ��ӂ��Ă��������B")
                                Else
                                    'NOTE: ���蓾�Ȃ��͂��ł���B
                                    Log.Error(sMonitorMachineId, "���Y�[���ɂ����āA���Y�K�p���X�g�ɕR�Â��v���O�����{�̂�����܂���B�K�p���X�g�̔z�M�͍s���܂���B")
                                    listDeliveryResult = &H1
                                End If
                            End If

                            '�v���O�����{�̂Ɋւ���#MadoProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
                            CreateFileOfMadoProDlReflectReq( _
                               &H91, _
                               oPenPro.DataVersion, _
                               dataDeliveryResult, _
                               sMonitorMachineId, _
                               sTermMachineId, _
                               sMachineDir)
                        End If
                    End If

                    'NOTE: ���̔z�M�̑O�ɒ��ړ��������{�����ꍇ�ȂǁA���D�@�ɓK�p���X�g��
                    '���݂��Ȃ��ꍇ�́A���L���s��Ȃ��B
                    '�����̉��D�@�V�X�e���̋������i�ǂ������Ɋ֌W�Ȃ��j�����ɍČ�����B
                    If latestListHashValue IsNot Nothing Then
                        '�K�p���X�g�Ɋւ���#MadoProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
                        CreateFileOfMadoProDlReflectReq( _
                           &H75, _
                           oPenPro.ListVersion, _
                           listDeliveryResult, _
                           sMonitorMachineId, _
                           sTermMachineId, _
                           sMachineDir)
                    Else
                        Log.Warn(sMonitorMachineId, "���Y�[�����K�p���X�g��ێ����Ă��Ȃ��������߁A�K�p���X�g��DL�����ʒm�͍쐬���܂���ł����B����͎������D�@�V�X�e���ɍ��킹�����������ł��B")
                    End If
                Next oPenPro
                oTerm.PendingPrograms.Clear()

                If isHoldingProUpdated Then
                    CreateFileOfMadoProVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
                End If

                UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
            End If
        Next oTermEntry

        '�����̑����v���O�����ێ���Ԃ��X�V����B
        TrimMonitorMachineHoldingPrograms(sMonitorMachineId, oMonitorMachine)

        Return True
    End Function

    Protected Function ApplyMadoPro(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        '�w�肳�ꂽ�S�����ɂ��āA�ҋ@�ʂɃv���O������ێ����Ă��邩�`�F�b�N���A
        '���̓K�p�����^�p���ȑO�ł���΁A�K�p�ʂɈړ�����B
        '�܂��A�v���O�����ێ���Ԃ��X�V���������ɂ��ẮA
        'sContextDir��MadoProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AMadoProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����X�V���Ă��Ȃ������̂��́j���폜����B

        'NOTE: �����̃t�@�C������RRRSSSCCCCUU�͒[���̋@��R�[�h��
        '���邪�A�����̃t�@�C�����S�[�����i�V�i���I����
        '�u%T3R%T3S%T4C%T2U�v�ƋL�q�����ꍇ�ɕ��������S�s���j
        '�쐬�����Ƃ͌���Ȃ��B
        '����āA�V�i���I�́A���Y�t�@�C���𑗐M����ہA
        'ActiveOne�ł͂Ȃ��ATryActiveOne���g�p����B

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            If oTermIdRegx.IsMatch(sTermMachineId) Then
                Dim sFileName As String = _
                   "MadoProVerInfo_" & _
                   GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId) & _
                   ".dat"
                DeleteFiles(sMonitorMachineId, sContextDir, sFileName)

                Dim oTerm As TermMachine = oTermEntry.Value
                If oTerm.HoldingPrograms(1) Is Nothing Then
                    Log.Debug(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ́A�K�p�҂��̃v���O����������܂���B")
                ElseIf oTerm.HoldingPrograms(1).ListHashValue IsNot Nothing AndAlso _
                       String.CompareOrdinal(oTerm.HoldingPrograms(1).ApplicableDate, sServiceDate) > 0 Then
                    Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ́A�K�p�҂��̃v���O����������܂����A�K�p���O�ł��邽�߁A�K�p���܂���B")
                ElseIf String.CompareOrdinal(oTerm.HoldingPrograms(1).RunnableDate, sServiceDate) > 0 Then
                    Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ́A�K�p�҂��̃v���O����������܂����A���싖���O�ł��邽�߁A�K�p���܂���B")
                Else
                    oTerm.HoldingPrograms(0) = oTerm.HoldingPrograms(1)
                    oTerm.HoldingPrograms(0).ApplyDate = d
                    oTerm.HoldingPrograms(1) = Nothing
                    Log.Info(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ����āA�K�p�҂��̃v���O������K�p���܂����B")
                    CreateFileOfMadoProVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
                    UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
                End If
            End If
        Next oTermEntry

        '�����̑����v���O�����ێ���Ԃ��X�V����B
        TrimMonitorMachineHoldingPrograms(sMonitorMachineId, oMonitorMachine)

        Return True
    End Function

    Protected Function SweepMadoPro(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        SweepMonitorMachineHoldingPrograms(sMonitorMachineId, oMonitorMachine)
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

    Protected Function FindLatestMasterDataInMonitorMachine(ByVal oMonitorMachine As Machine, ByVal sDataKind As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer) As HoldingMaster
        Dim oLatestMas As HoldingMaster = Nothing
        Dim oHoldingMasters As List(Of HoldingMaster) = Nothing
        If oMonitorMachine.HoldingMasters.TryGetValue(sDataKind, oHoldingMasters) = True Then
            For Each oMas As HoldingMaster In oHoldingMasters
                If oMas.DataSubKind = dataSubKind AndAlso _
                   oMas.DataVersion = dataVersion AndAlso _
                   oMas.DataHashValue IsNot Nothing AndAlso _
                   (oLatestMas Is Nothing OrElse oMas.DataAcceptDate > oLatestMas.DataAcceptDate) Then
                    oLatestMas = oMas
                End If
            Next oMas
        End If
        Return oLatestMas
    End Function

    Protected Function FindLatestProgramDataInMonitorMachine(ByVal oMonitorMachine As Machine, ByVal dataSubKind As Integer, ByVal dataVersion As Integer) As HoldingProgram
        Dim oLatestPro As HoldingProgram = Nothing
        For Each oPro As HoldingProgram In oMonitorMachine.HoldingPrograms
            If oPro.DataSubKind = dataSubKind AndAlso _
               oPro.DataVersion = dataVersion AndAlso _
               oPro.DataHashValue IsNot Nothing AndAlso _
               (oLatestPro Is Nothing OrElse oPro.DataAcceptDate > oLatestPro.DataAcceptDate) Then
                oLatestPro = oPro
            End If
        Next oPro
        Return oLatestPro
    End Function

    Protected Sub TrimMonitorMachineHoldingMasters(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine, ByVal oTargetDataKinds As HashSet(Of String))
        '�����̑����}�X�^�ێ���Ԃ��X�V����B
        '�z���̑������ێ����Ă���ŌÂ̕��ނ����Â������ʂ̕��ނ��A�폜����B

        For Each sDataKind As String In oTargetDataKinds
            Dim oHoldingMasters As List(Of HoldingMaster) = Nothing
            If oMonitorMachine.HoldingMasters.TryGetValue(sDataKind, oHoldingMasters) = True Then
                Dim boundaryDate As DateTime = DateTime.MaxValue
                For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                    Dim oTermHoldingMas As HoldingMaster = Nothing
                    If oTerm.HoldingMasters.TryGetValue(sDataKind, oTermHoldingMas) = True Then
                        If oTermHoldingMas.ListHashValue IsNot Nothing AndAlso oTermHoldingMas.ListAcceptDate < boundaryDate Then
                            boundaryDate = oTermHoldingMas.ListAcceptDate
                        End If
                        If oTermHoldingMas.DataHashValue IsNot Nothing AndAlso oTermHoldingMas.DataAcceptDate < boundaryDate Then
                            boundaryDate = oTermHoldingMas.DataAcceptDate
                        End If
                    End If

                    Dim oPendingMasters As LinkedList(Of PendingMaster) = Nothing
                    If oTerm.PendingMasters.TryGetValue(sDataKind, oPendingMasters) = True Then
                        For Each oPendingMas As PendingMaster In oPendingMasters
                            If oPendingMas.ListHashValue IsNot Nothing AndAlso oPendingMas.ListAcceptDate < boundaryDate Then
                                boundaryDate = oPendingMas.ListAcceptDate
                            End If
                            If oPendingMas.DataHashValue IsNot Nothing AndAlso oPendingMas.DataAcceptDate < boundaryDate Then
                                boundaryDate = oPendingMas.DataAcceptDate
                            End If
                        Next oPendingMas
                    End If
                Next oTerm

                Dim oNewMasters As New List(Of HoldingMaster)()
                For Each oMas As HoldingMaster In oHoldingMasters
                    If (oMas.ListHashValue IsNot Nothing AndAlso oMas.ListAcceptDate >= boundaryDate) OrElse _
                       (oMas.DataHashValue IsNot Nothing AndAlso oMas.DataAcceptDate >= boundaryDate) Then
                        oNewMasters.Add(oMas)
                    Else
                        If oMas.ListHashValue IsNot Nothing Then
                            Log.Debug(sMonitorMachineId, "��������A��� [" & sDataKind & "] �p�^�[��No [" & oMas.DataSubKind.ToString() & "] �}�X�^Ver [" & oMas.DataVersion.ToString() & "] ���X�gVer [" & oMas.ListVersion.ToString() & "] �������� [" & oMas.ListAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] �̃}�X�^�K�p���X�g���폜���܂��B")
                        End If
                        If oMas.DataHashValue IsNot Nothing Then
                            Log.Debug(sMonitorMachineId, "��������A��� [" & sDataKind & "] �p�^�[��No [" & oMas.DataSubKind.ToString() & "] �}�X�^Ver [" & oMas.DataVersion.ToString() & "] �������� [" & oMas.DataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] �̃}�X�^�{�̂��폜���܂��B")
                        End If
                    End If
                Next oMas
                oMonitorMachine.HoldingMasters(sDataKind) = oNewMasters
            End If
        Next sDataKind
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

    Protected Sub TrimMonitorMachineHoldingPrograms(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine)
        '�����̑����v���O�����ێ���Ԃ��X�V����B
        '�z���̑������ێ����Ă���ŌÂ̕��ނ����Â����ނ��A�폜����B

        Dim boundaryDate As DateTime = DateTime.MaxValue
        For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
            For Each oTermHoldingPro As HoldingProgram In oTerm.HoldingPrograms
                If oTermHoldingPro IsNot Nothing Then
                    If oTermHoldingPro.ListHashValue IsNot Nothing AndAlso oTermHoldingPro.ListAcceptDate < boundaryDate Then
                        boundaryDate = oTermHoldingPro.ListAcceptDate
                    End If
                    If oTermHoldingPro.DataHashValue IsNot Nothing AndAlso oTermHoldingPro.DataAcceptDate < boundaryDate Then
                        boundaryDate = oTermHoldingPro.DataAcceptDate
                    End If
                End If
            Next oTermHoldingPro

            For Each oPendingPro As PendingProgram In oTerm.PendingPrograms
                If oPendingPro.ListHashValue IsNot Nothing AndAlso oPendingPro.ListAcceptDate < boundaryDate Then
                    boundaryDate = oPendingPro.ListAcceptDate
                End If
                If oPendingPro.DataHashValue IsNot Nothing AndAlso oPendingPro.DataAcceptDate < boundaryDate Then
                    boundaryDate = oPendingPro.DataAcceptDate
                End If
            Next oPendingPro
        Next oTerm

        Dim oNewPrograms As New List(Of HoldingProgram)()
        For Each oPro As HoldingProgram In oMonitorMachine.HoldingPrograms
            If (oPro.ListHashValue IsNot Nothing AndAlso oPro.ListAcceptDate >= boundaryDate) OrElse _
               (oPro.DataHashValue IsNot Nothing AndAlso oPro.DataAcceptDate >= boundaryDate) Then
                oNewPrograms.Add(oPro)
            Else
                If oPro.ListHashValue IsNot Nothing Then
                    Log.Debug(sMonitorMachineId, "��������A�G���ANo [" & oPro.DataSubKind.ToString() & "] ��\Ver [" & oPro.DataVersion.ToString() & "] ���X�gVer [" & oPro.ListVersion.ToString() & "] �������� [" & oPro.ListAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] �̑����v���O�����K�p���X�g���폜���܂��B")
                End If
                If oPro.DataHashValue IsNot Nothing Then
                    Log.Debug(sMonitorMachineId, "��������A�G���ANo [" & oPro.DataSubKind.ToString() & "] ��\Ver [" & oPro.DataVersion.ToString() & "] �������� [" & oPro.DataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] �̑����v���O�����{�̂��폜���܂��B")
                End If
            End If
        Next oPro
        oMonitorMachine.HoldingPrograms = oNewPrograms
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

    Protected Sub SweepMonitorMachineHoldingMasters(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine)
        '�����̑����}�X�^�ێ���Ԃ��X�V����B
        '�z���̑������ێ����Ă�����̂Ɣz���̑����ւ̔z�M�҂��ɂ��Ă�����̈ȊO�́A�폜����B

        For Each sDataKind As String In ExConstants.MadoMastersSubObjCodes.Keys
            Dim oHoldingMasters As List(Of HoldingMaster) = Nothing
            If oMonitorMachine.HoldingMasters.TryGetValue(sDataKind, oHoldingMasters) = True Then
                Dim oNewMasters As New List(Of HoldingMaster)()
                For Each oMas As HoldingMaster In oHoldingMasters
                    Dim isMasNecessary As Boolean = False
                    For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                        'TODO: DataSubKind�͔�r���Ȃ������悢��������Ȃ��B�{���̓����́A
                        '�������ێ����Ă���}�X�^�̃p�^�[���ԍ����킩��Ȃ���������Ȃ��B
                        Dim oTermHoldingMas As HoldingMaster = Nothing
                        If oTerm.HoldingMasters.TryGetValue(sDataKind, oTermHoldingMas) = True AndAlso _
                           oTermHoldingMas.DataSubKind = oMas.DataSubKind AndAlso _
                           oTermHoldingMas.DataVersion = oMas.DataVersion Then

                            'oMas���}�X�^�{�̂��܂�ł���z�M�v���̏ꍇ�́A�}�X�^�{�̂Ɋւ���S���ڂ�
                            '��v���Ă��邾���ł��AoMas���K�v�Ƃ݂Ȃ��B
                            If oMas.DataHashValue IsNot Nothing Then
                                If oTermHoldingMas.DataAcceptDate = oMas.DataAcceptDate Then
                                    isMasNecessary = True
                                    Exit For
                                End If
                            End If

                            If oTermHoldingMas.ListHashValue IsNot Nothing AndAlso _
                               oMas.ListHashValue IsNot Nothing AndAlso _
                               oTermHoldingMas.ListVersion = oMas.ListVersion AndAlso _
                               oTermHoldingMas.ListAcceptDate = oMas.ListAcceptDate Then
                                isMasNecessary = True
                                Exit For
                            End If
                        End If

                        Dim oPendingMasters As LinkedList(Of PendingMaster) = Nothing
                        If oTerm.PendingMasters.TryGetValue(sDataKind, oPendingMasters) = True Then
                            For Each oPendingMas As PendingMaster In oPendingMasters
                                'TODO: DataSubKind�͔�r���Ȃ������悢��������Ȃ��B�{���̓����́A
                                '�������ێ����Ă���}�X�^�̃p�^�[���ԍ����킩��Ȃ���������Ȃ��B
                                If oPendingMas.DataSubKind = oMas.DataSubKind AndAlso _
                                   oPendingMas.DataVersion = oMas.DataVersion Then

                                    'oMas���}�X�^�{�̂��܂�ł���z�M�v���̏ꍇ�́A�}�X�^�{�̂Ɋւ���S���ڂ�
                                    '��v���Ă��邾���ł��AoMas���K�v�Ƃ݂Ȃ��B
                                    If oMas.DataHashValue IsNot Nothing Then
                                        If oPendingMas.DataAcceptDate = oMas.DataAcceptDate Then
                                            isMasNecessary = True
                                            Exit For
                                        End If
                                    End If

                                    If oPendingMas.ListHashValue IsNot Nothing AndAlso _
                                       oMas.ListHashValue IsNot Nothing AndAlso _
                                       oPendingMas.ListVersion = oMas.ListVersion AndAlso _
                                       oPendingMas.ListAcceptDate = oMas.ListAcceptDate Then
                                        isMasNecessary = True
                                        Exit For
                                    End If
                                End If
                            Next oPendingMas
                            If isMasNecessary Then Exit For
                        End If
                    Next oTerm
                    If isMasNecessary Then
                        oNewMasters.Add(oMas)
                    Else
                        If oMas.ListHashValue IsNot Nothing Then
                            Log.Debug(sMonitorMachineId, "��������A��� [" & sDataKind & "] �p�^�[��No [" & oMas.DataSubKind.ToString() & "] �}�X�^Ver [" & oMas.DataVersion.ToString() & "] ���X�gVer [" & oMas.ListVersion.ToString() & "] �������� [" & oMas.ListAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] �̃}�X�^�K�p���X�g���폜���܂��B")
                        End If
                        If oMas.DataHashValue IsNot Nothing Then
                            Log.Debug(sMonitorMachineId, "��������A��� [" & sDataKind & "] �p�^�[��No [" & oMas.DataSubKind.ToString() & "] �}�X�^Ver [" & oMas.DataVersion.ToString() & "] �������� [" & oMas.DataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] �̃}�X�^�{�̂��폜���܂��B")
                        End If
                    End If
                Next oMas
                oMonitorMachine.HoldingMasters(sDataKind) = oNewMasters
            End If
        Next sDataKind
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

    Protected Sub SweepMonitorMachineHoldingPrograms(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine)
        '�����̑����v���O�����ێ���Ԃ��X�V����B
        '�z���̑������ێ����Ă�����̂Ɣz���̑����ւ̔z�M�҂��ɂ��Ă�����̈ȊO�́A�폜����B

        Dim oNewPrograms As New List(Of HoldingProgram)()
        For Each oPro As HoldingProgram In oMonitorMachine.HoldingPrograms
            Dim isProNecessary As Boolean = False
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                'TODO: DataSubKind�͔�r���Ȃ������悢��������Ȃ��B
                '�z�M�̎d�g�݂�p���ē��������v���O�����̃G���A�ԍ��͔�r����܂ł��Ȃ�
                '��v���Ă���͂��ł��邪�A�����łȂ��ꍇ�A�{���̓����ɂ́A
                '���������ۂɕێ����Ă���v���O�����̃G���A�ԍ����킩��Ȃ���������Ȃ��B
                For Each oTermHoldingPro As HoldingProgram In oTerm.HoldingPrograms
                    If oTermHoldingPro IsNot Nothing AndAlso _
                       oTermHoldingPro.DataSubKind = oPro.DataSubKind AndAlso _
                       oTermHoldingPro.DataVersion = oPro.DataVersion Then

                        'oPro���v���O�����{�̂��܂�ł���z�M�v���̏ꍇ�́A�v���O�����{�̂Ɋւ���S���ڂ�
                        '��v���Ă��邾���ł��AoPro���K�v�Ƃ݂Ȃ��B
                        If oPro.DataHashValue IsNot Nothing Then
                            If oTermHoldingPro.DataAcceptDate = oPro.DataAcceptDate Then
                                isProNecessary = True
                                Exit For
                            End If
                        End If

                        If oTermHoldingPro.ListHashValue IsNot Nothing AndAlso _
                           oPro.ListHashValue IsNot Nothing AndAlso _
                           oTermHoldingPro.ListVersion = oPro.ListVersion AndAlso _
                           oTermHoldingPro.ListAcceptDate = oPro.ListAcceptDate Then
                            isProNecessary = True
                            Exit For
                        End If
                    End If
                Next oTermHoldingPro
                If isProNecessary Then Exit For

                For Each oPendingPro As PendingProgram In oTerm.PendingPrograms
                    'TODO: DataSubKind�͔�r���Ȃ������悢��������Ȃ��B�{���̓����́A
                    '�������ێ����Ă���v���O�����̃p�^�[���ԍ����킩��Ȃ���������Ȃ��B
                    If oPendingPro.DataSubKind = oPro.DataSubKind AndAlso _
                       oPendingPro.DataVersion = oPro.DataVersion Then

                        'oPro���v���O�����{�̂��܂�ł���z�M�v���̏ꍇ�́A�v���O�����{�̂Ɋւ���S���ڂ�
                        '��v���Ă��邾���ł��AoPro���K�v�Ƃ݂Ȃ��B
                        If oPro.DataHashValue IsNot Nothing Then
                            If oPendingPro.DataAcceptDate = oPro.DataAcceptDate Then
                                isProNecessary = True
                                Exit For
                            End If
                        End If

                        If oPendingPro.ListHashValue IsNot Nothing AndAlso _
                           oPro.ListHashValue IsNot Nothing AndAlso _
                           oPendingPro.ListVersion = oPro.ListVersion AndAlso _
                           oPendingPro.ListAcceptDate = oPro.ListAcceptDate Then
                            isProNecessary = True
                            Exit For
                        End If
                    End If
                Next oPendingPro
                If isProNecessary Then Exit For
            Next oTerm

            If isProNecessary Then
                oNewPrograms.Add(oPro)
            Else
                If oPro.ListHashValue IsNot Nothing Then
                    Log.Debug(sMonitorMachineId, "��������A�G���ANo [" & oPro.DataSubKind.ToString() & "] ��\Ver [" & oPro.DataVersion.ToString() & "] ���X�gVer [" & oPro.ListVersion.ToString() & "] �������� [" & oPro.ListAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] �̑����v���O�����K�p���X�g���폜���܂��B")
                End If
                If oPro.DataHashValue IsNot Nothing Then
                    Log.Debug(sMonitorMachineId, "��������A�G���ANo [" & oPro.DataSubKind.ToString() & "] ��\Ver [" & oPro.DataVersion.ToString() & "] �������� [" & oPro.DataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] �̑����v���O�����{�̂��폜���܂��B")
                End If
            End If
        Next oPro
        oMonitorMachine.HoldingPrograms = oNewPrograms
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

    Protected Sub InstallMadoProgramDirectly(ByVal sContextDir As String, ByVal sTermMachineId As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal content As MadoProgramContent, ByVal sDataHashValue As String)
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)
        Dim oTermMachine As TermMachine = oMonitorMachine.TermMachines(sTermMachineId)

        '�����ɂ�����w�肳�ꂽ�����ւ̑����v���O�����z�M�ۗ��󋵂����������A
        '�w�肳�ꂽ�����v���O�����𓖊Y�����ɓ�������i�ێ�������j�B
        '�܂��A���̑����̑����v���O�����ێ��󋵂����������A
        '�w�肳�ꂽ�����v���O�����������̑ҋ@�ʂɓ������A
        'sContextDir��MadoProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AMadoProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����̔z�M�Ɩ��֌W�Ȃ��́A����X�V
        '���Ă��Ȃ����D�@�̂��́j���폜����B

        Dim sFileName As String = _
           "MadoProVerInfo_" & _
           GetStationOf(sTermMachineId) & GetCornerOf(sTermMachineId) & GetUnitOf(sTermMachineId) & _
           ".dat"
        DeleteFiles(sMonitorMachineId, sContextDir, sFileName)

        Dim d As DateTime = DateTime.Now

        oTermMachine.PendingPrograms.Clear()

        Dim oPro As New HoldingProgram()
        oPro.DataSubKind = dataSubKind
        oPro.DataVersion = dataVersion
        oPro.DataAcceptDate = d
        oPro.DataDeliverDate = d
        oPro.RunnableDate = content.RunnableDate
        oPro.ArchiveCatalog = content.ArchiveCatalog
        oPro.VersionListData = content.VersionListData
        oPro.DataHashValue = sDataHashValue
        oPro.ListVersion = 0
        oPro.ListAcceptDate = Config.EmptyTime
        oPro.ListDeliverDate = Config.EmptyTime
        oPro.ApplicableDate = Nothing
        oPro.ListContent = Nothing
        oPro.ListHashValue = Nothing
        oTermMachine.HoldingPrograms(1) = oPro
        Log.Info(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �̑ҋ@�ʂɑ΂��đ����v���O�����𒼐ړ������܂����B")

        'TODO: ���L�̂悤�ȏ󋵂̏ꍇ�A�����̃V�X�e���ł͂ǂ��Ȃ�̂��H
        '�����炭�A�Đڑ����ɑ��M���s����͂��ł��邩��A�����͂����ƕۗ��ɂ��āA
        '�ُ�����������ۂɁi�蓮�ł��悢�̂Łj�o�[�W�������𑗐M�ł��������悢�B
        'If oTermMachine.Tk2Status <> &H2 Then
        '    Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ��ẮA����DL�n��Ԃ��ڑ��ȊO�ɐݒ肳��Ă��邽�߁A�o�[�W�������̑��M��ۗ����܂��B")
        'End If

        '�v���O�����{�̂Ɋւ���#MadoProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
        'TODO: ����͖����Ɛ������Ă��邪�A�L�����肩�łȂ��B
        '���@������������Ȃ�A�^�ǓI�ɂ����������č\��Ȃ��̂ŁA���@�ɍ��킹��ׂ��B
        'CreateFileOfMadoProDlReflectReq( _
        '   &H91, _
        '   dataVersion, _
        '   &H0, _
        '   sMonitorMachineId, _
        '   sTermMachineId, _
        '   sMachineDir)

        CreateFileOfMadoProVerInfo(sMonitorMachineId, sTermMachineId, oTermMachine, sContextDir)
        UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTermMachine)
    End Sub

End Class
