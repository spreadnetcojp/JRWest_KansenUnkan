' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/08/08  (NES)小林  新規作成
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
                    Throw New OPMGException("機器構成の" & lineCount.ToString() & "行目のカラム数が不正です。")
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
                        Log.Info(sMachineId, "ファイル [" & sFilePath & "] を削除しました。")
                    End If
                Else
                    File.Delete(sFilePath)
                    Log.Info(sMachineId, "ファイル [" & sFilePath & "] を削除しました。")
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
            'CABを展開する。
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

            'プログラムバージョンリストを解析する。
            Dim sVerListPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, ExConstants.MadoProgramVersionListPathInCab)
            Try
                Using oInputStream As New FileStream(sVerListPath, FileMode.Open, FileAccess.Read)
                    'ファイルのレングスを取得する。
                    Dim len As Integer = CInt(oInputStream.Length)
                    If len < ProgramVersionListUtil.RecordLengthInBytes Then
                        Throw New OPMGException("バージョンリストのサイズが異常です。")
                    End If
                    'ファイルを読み込む。
                    ret.VersionListData = New Byte(len - 1) {}
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(ret.VersionListData, pos, len - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Throw New OPMGException("バージョンリストの読み込みで異常が発生しました。", ex)
            End Try

            ret.RunnableDate = ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム動作許可日", ret.VersionListData)
            Dim oRunnableDate As DateTime
            If DateTime.TryParseExact(ret.RunnableDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, oRunnableDate) = False Then
                Throw New OPMGException("バージョンリストに記載された動作許可日が異常です。" & vbCrLf & "動作許可日: " & ret.RunnableDate)
            End If
        Finally
            Utility.DeleteTemporalDirectory(sTempDirPath)
        End Try

        'CAB内のファイル一覧を取得する。
        Using oProcess As New System.Diagnostics.Process()
            'NOTE: TsbCab -l は、コマンド引数に渡すCABファイルのパスに
            '多バイト文字が含まれているとクラッシュするようであるため、
            'WorkingDirectoryを当該ファイルのあるディレクトリにすることで、
            'コマンド引数にはファイル名のみを渡すことにする。
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

    'NOTE: ログ出力毎に呼ばれるので、これの中でログを出力してはならない。
    Protected Sub BeginFetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)
        Try
            'OPT: 上記が守られる限りはデッドロックもないと思われるので、
            'BeginInvoke()ではなく、Invoke()でもよいかもしれない。
            'Invoke()ならば、メッセージキューがあふれる心配もない。
            BeginInvoke( _
                OptionalWriter, _
                New Object() {number, sSecondName, sDateTime, sKind, sClassName, sMethodName, sText})
        Catch ex As Exception
            'NOTE: このControlが破棄された後にこのメソッドが呼び出される万が一の場合を想定している。
            'この後の（このデリゲートに依存しない）処理を通常通り行うよう、例外は握りつぶす。
        End Try
    End Sub

    'NOTE: ログ出力毎に呼ばれるので、これの中でログを出力してはならない。
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

        'Lexisから生成した文言やConfigやUiStateの値を各コントロールに反映する。

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
            'NOTE: 最後に呼び出したBeginReceiveに対応するReceiveCompletedイベントは、
            '下記によるInputQueue.readHandleのようなもののCloseによって、
            'それ以降、発生することは無くなる想定である。
            'InputQueue.SynchronizingObjectをNothingにしておけばよさそうにも
            '思えるが、このプロパティはスレッドセーフではなさそうであるため、
            '既にBeginReceiveを行ってしまっているこの時点では変更するべきではない。
            InputQueue.Dispose()
        End If

        If Config.SelfMqPath IsNot Nothing Then
            Try
                'NOTE: SelfMqPathが長すぎる場合は、MessageQueue.Exists()は
                '実際に当該パスにキューが存在していても、Falseを返却
                'するようである。一方、SelfMqPathが長すぎる場合も、
                'メッセージキューサービスがインストールされている限りは、
                'MessageQueue.Createが成功してしまう。
                'よって、SelfMqPathが長すぎる場合に、起動時に作成してしまった
                'キューの削除を試みるには、ここでのMessageQueue.Existsによる
                '判断を省略するしかない。なお、削除を試みたところで、
                'SelfMqPathが長すぎることを理由にMessageQueue.Delete()から
                'MessageQueueExceptionがスローされ、削除は失敗するが、
                'そのMessageQueueErrorCodeプロパティによって、キューが残って
                'しまうことが分かるため、その警告を出すことができる。
                'If MessageQueue.Exists(Config.SelfMqPath) Then
                '    MessageQueue.Delete(Config.SelfMqPath)
                'End If

                MessageQueue.Delete(Config.SelfMqPath)

            Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.FormatNameBufferTooSmall
                'NOTE: 本来は、アプリケーションがMessageQueueErrorCodeを
                '参照するべきではないが、MessageQueueクラスの挙動が
                '酷すぎるためやむを得ない。MessageQueueクラスの挙動が
                '改善されたら、除去すること。
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.MessageQueueDeleteFailed)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If

        If UiState IsNot Nothing Then
            'NOTE: このケースでは、右辺の各コントロールに、少なくとも起動時のファイルから
            'ロードした値はセット済みの想定である。

            'TODO: コントロールの状態を保存することにした場合は、
            'ここで各コントロールの値をUiStateに反映する。

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
            InitExtraComboColumnViewOfTable2("NEGA_STS", "ネガ状態 (X)", "FF..", "ネガ状態", "○○○○○状態...", Config.MenuTableOfTktNegaStatus)
            InitExtraComboColumnViewOfTable2("MEISAI_STS", "明細状態 (X)", "FF..", "明細状態", "○○○○○状態...", Config.MenuTableOfTktMeisaiStatus)
            InitExtraComboColumnViewOfTable2("ONLINE_STS", "オンライン状態 (X)", "FF..", "オンライン状態", "○○○○○状態...", Config.MenuTableOfTktOnlineStatus)
        End If

        If MadoConStatusRadioButton.Checked Then
            InitExtraComboColumnViewOfTable2("DLS_STS", "配信サーバ状態 (X)", "FF..", "配信サーバ状態", "○○○○○状態...", Config.MenuTableOfMadoDlsStatus)
            InitExtraComboColumnViewOfTable2("KSB_STS", "監視盤状態 (X)", "FF..", "監視盤状態", "○○○○○状態...", Config.MenuTableOfMadoKsbStatus)
            InitExtraComboColumnViewOfTable2("TK1_STS", "統括ID系状態 (X)", "FF..", "統括ID系状態", "○○○○○状態...", Config.MenuTableOfMadoTk1Status)
            InitExtraComboColumnViewOfTable2("TK2_STS", "統括DL系状態 (X)", "FF..", "統括DL系状態", "○○○○○状態...", Config.MenuTableOfMadoTk2Status)
        End If

        If MasStatusRadioButton.Checked Then
            DataGridView2.Columns("SLOT").ReadOnly = True
            DataGridView2.Columns("SLOT").Frozen = True
            DataGridView2.Columns("SLOT").HeaderText = "世代"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("配信待ち(9)",  DataGridView2.Font)
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
                DataGridView2.Columns(sKind & "_DataSubKind").HeaderText = "パターンNo (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataVersion").HeaderText = "マスタVer (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListVersion").HeaderText = "リストVer (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataAcceptDate").HeaderText = "データ統着 (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListAcceptDate").HeaderText = "リスト統着 (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataDeliverDate").HeaderText = "データ窓着 (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataHashValue").HeaderText = "データ概値 (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListHashValue").HeaderText = "リスト概値 (" & sKind &")"
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
            DataGridView2.Columns("SLOT").HeaderText = "世代"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("配信待ち(9)", DataGridView2.Font)
            Dim anWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim dvWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim lvWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim adWidth As Integer = MyUtility.GetTextWidth("○○○○○.", DataGridView2.Font)
            Dim rdWidth As Integer = MyUtility.GetTextWidth("○○○○○.", DataGridView2.Font)
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
            DataGridView2.Columns("YPG_DataSubKind").HeaderText = "エリアNo"
            DataGridView2.Columns("YPG_DataVersion").HeaderText = "代表Ver"
            DataGridView2.Columns("YPG_ListVersion").HeaderText = "リストVer"
            DataGridView2.Columns("YPG_DataAcceptDate").HeaderText = "データ統着日時"
            DataGridView2.Columns("YPG_ListAcceptDate").HeaderText = "リスト統着日時"
            DataGridView2.Columns("YPG_DataDeliverDate").HeaderText = "データ窓着日時"
            DataGridView2.Columns("YPG_ListDeliverDate").HeaderText = "リスト窓着日時"
            DataGridView2.Columns("YPG_RunnableDate").HeaderText = "動作許可日"
            DataGridView2.Columns("YPG_ApplicableDate").HeaderText = "適用日"
            DataGridView2.Columns("YPG_ApplyDate").HeaderText = "適用完了日時"
            DataGridView2.Columns("YPG_DataHashValue").HeaderText = "データハッシュ値"
            DataGridView2.Columns("YPG_ListHashValue").HeaderText = "リストハッシュ値"
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
            oTargetRow("SLOT") = "保持(" & (listIndex + 1).ToString() & ")"
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
            oTargetRow("SLOT") = "保持(" & (listIndex + 1).ToString() & ")"

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
                oTargetRow("SLOT") = "配信待ち(" & (listIndex + 1).ToString() & ")"
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
                oTargetRow("SLOT") = "適用中"
                For Each sKind As String In ExConstants.MadoMastersSubObjCodes.Keys
                    Dim oMas As HoldingMaster = Nothing
                    oMachine.HoldingMasters.TryGetValue(sKind, oMas)

                    'NOTE: 端末がマスタ本体を保持せずに適用リストを保持することは
                    'あり得ない。
                    '端末がsKindのマスタを保持していない場合、
                    '必ずoMas自体がNothingになるため、
                    'oMas.DataHashValueがNothingになることはない。
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
                oTargetRow("SLOT") = "配信待ち(" & (listIndex + 1).ToString() & ")"

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
                oTargetRow("SLOT") = If(listIndex = 1, "適用待ち", "適用中")

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
        '空のデータテーブルを作成し、フィールド名を設定する。
        Table1 = New DataTable()
        For i As Integer = 0 To Config.Table1FieldNames.Length - 1
            Dim sFieldName As String = Config.Table1FieldNames(i)
            Table1.Columns.Add(sFieldName, Config.FieldNamesTypes(sFieldName))
        Next i

        'UiState.Machinesの基本情報をデータテーブルに展開する。
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
        '空のデータテーブルを作成し、フィールド名を設定する。
        Table2 = New DataTable()
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            Table2.Columns.Add(sFieldName, Config.FieldNamesTypes(sFieldName))
        Next i
        AddExtraColumnsToTable2()

        'UiState.Machinesの基本情報をデータテーブルに展開する。
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
            Log.Info("新しい監視機器 [" & sMachineId & "] を検出しました。")
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
                    Throw New OPMGException("機器構成ファイルの行数が異常です。")
                End If
                If GetMachineId(oProfileTable.Rows(0)) <> sMachineId Then
                    Throw New OPMGException("機器構成ファイルの内容と作業ディレクトリ名に不整合があります。")
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
                        Log.Info("新しい端末機器 [" & sTermId & "] を検出しました。")
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
                            Log.Warn("削除された端末 [" & oTermEntry.Key & "] の状態情報をクリアします。")
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

        'NOTE: 端末機器の構成が変化しても、Table2の監視機器の行に、関連する項目は無い想定である。
        'よって、Table2の監視機器の行については、監視機器自体が追加されたケースでのみ、ケアする。
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
                            'NOTE: ダブルクリックされたのが監視機器の行であり、その行のsDataKindに関する列には値が存在している場合である。
                            '監視機器の行は、保持(n)のみであるため、上記の条件が成立しているなら、oMachine.HoldingMastersには
                            'sDataKindをキーとする要素が必ず存在している。
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
                            'NOTE: ダブルクリックされたのが端末機器の行であり、その行のsDataKindに関する列には値が存在している場合である。
                            '端末機器には、適用中の行と配信待ち(n)の行があるため、上記の条件が成立していても、（ダブルクリックされた
                            'のが配信待ち(n)の行なら）oTerm.HoldingMastersにsDataKindをキーとする要素が存在しているとは限らないし、
                            '（ダブルクリックされたのが適用中の行なら）oTerm.PendingMastersにsDataKindをキーとする要素が存在している
                            'とは限らない。
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
                        'NOTE: ダブルクリックされたのが監視機器の行であり、その行のsDataKindに関する列には値が存在している場合である。
                        '監視機器の行は、保持(n)のみであるため、上記の条件が成立しているなら、oMachine.HoldingMastersには
                        'sDataKindをキーとする要素が必ず存在している。
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
                        'NOTE: ダブルクリックされたのが端末機器の行であり、その行のsDataKindに関する列には値が存在している場合である。
                        '端末機器には、適用中の行と配信待ち(n)の行があるため、上記の条件が成立していても、（ダブルクリックされた
                        'のが配信待ち(n)の行なら）oTerm.HoldingMastersにsDataKindをキーとする要素が存在しているとは限らないし、
                        '（ダブルクリックされたのが適用中の行なら）oTerm.PendingMastersにsDataKindをキーとする要素が存在している
                        'とは限らない。
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

        'DataGridView2で選択中の窓処を抽出する。
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
            Log.Info(sMonitorMachineId, "選択中の端末機器 [" & sTermMachineId & "] から、マスタおよびマスタ適用リストを削除します...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
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

        'DataGridView2で選択中の窓処を抽出する。
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
            Log.Info(sMonitorMachineId, "選択中の端末機器 [" & sTermMachineId & "] に、統括で配信待ちの全マスタを配信します...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
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
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] のマスタ洗い替えを行います...")
                Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)
                SweepMonitorMachineHoldingMasters(sMonitorMachineId, oMonitorMachine)
                Log.Info(sMonitorMachineId, "マスタ洗い替えが終了しました。")
            End If
        Next gridRow
    End Sub

    Private Sub ProDirectInstallButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProDirectInstallButton.Click
        Dim t2TermIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "TERM_MACHINE_ID")
        Dim t2MonitorIdCol As Integer = Array.IndexOf(Config.Table2FieldNames, "MACHINE_ID")

        'DataGridView2で選択中の窓処を抽出する。
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
        oDialog.Filter = "CABファイル|*.cab"
        oDialog.FileName = ""
        oDialog.ReadOnlyChecked = True
        oDialog.Title = "投入するプログラムを選択してください。"
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

        'NOTE: 以下、少し汚いが、複数機器が選択されている場合の速度性能を優先して、
        'InstallMadoProgramDirectlyの中ではなく、呼び元でCABの解析を行うことにしている。

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(oDialog.FileName)
        Catch ex As Exception
            Log.Error("プログラム本体のハッシュ値算出で例外が発生しました。", ex)
            Return
        End Try

        Dim content As MadoProgramContent
        Try
            Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
            Dim sMonitorMachineId As String = DirectCast(DataGridView1.CurrentRow.Cells(idxColumn).Value, String)
            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error("代表機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            content = ExtractMadoProgramCab(oDialog.FileName, Path.Combine(sContextDir, "MadoPro"))
        Catch ex As Exception
            Log.Error("プログラム本体の解析で例外が発生しました。", ex)
            Return
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("バージョンリストからのエリアNoの抽出で例外が発生しました。", ex)
            Return
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("バージョンリストからの代表Verの抽出で例外が発生しました。", ex)
            Return
        End Try

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("プログラムの動作許可日が未来日に設定されています。" & content.RunnableDate & "まで適用できませんのでご注意ください。")
        End If

        For Each oIdSet As KeyValuePair(Of String, String) In oMachineIds
            Dim sTermMachineId As String = oIdSet.Key
            Dim sMonitorMachineId As String = oIdSet.Value
            Log.Info(sMonitorMachineId, "選択中の端末機器 [" & sTermMachineId & "] に、窓処プログラムを直接投入します...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
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

        'DataGridView2で選択中の窓処を抽出する。
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
            Log.Info(sMonitorMachineId, "選択中の端末機器 [" & sTermMachineId & "] に、統括で配信待ちの全窓処プログラムを配信します...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
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

        'DataGridView2で選択中の窓処を抽出する。
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
            Log.Info(sMonitorMachineId, "選択中の端末機器 [" & sTermMachineId & "] において、適用待ちのプログラムを適用します...")

            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
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
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] の窓処プログラム洗い替えを行います...")
                Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)
                SweepMonitorMachineHoldingPrograms(sMonitorMachineId, oMonitorMachine)
                Log.Info(sMonitorMachineId, "窓処プログラム洗い替えが終了しました。")
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
                Log.Error("不正な要求を受信しました。" & vbCrLf & "WorkingDirectory: " & If(sContextDir Is Nothing, "Nothing", "[" & sContextDir & "]") & vbCrLf & "Func: " & If(bd.Func Is Nothing, "Nothing",  "[" & bd.Func & "]"))
            End If

            Try
                Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
                sTargetMachineId = GetMachineId(Path.GetFileName(sMachineDir))
                FetchMachineProfileFromFile(sMachineDir)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                Log.Error("不正な要求を受信しました。" & vbCrLf & "WorkingDirectory: [" & sContextDir & "]" & vbCrLf & "Func: [" & bd.Func & "]")
                Exit Try
            End Try

            Log.Info(sTargetMachineId, "[" & sContextDir & "] に対する要求 [" & bd.Func & "] を処理します...")
            Select Case bd.Func.ToUpperInvariant()
                'TODO: bd.Argsの件数（0の場合はNothingであること）をチェックする。
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
                    Log.Error(sTargetMachineId, "未知の要求です。")
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
            'NOTE: 窓処のマスタバージョン情報には、改札機のマスタバージョン情報と異なり、
            '無駄な基本ヘッダ部は存在しない。
            'TODO: いつかはインタフェース仕様を共通化したい。
            'ExVersionInfoFileHeader.WriteToStream(&H8B, GetEkCodeOf(sTermId), DateTime.Now, 1, oOutputStream)
            ExMasterVersionInfo.WriteToStream(oTermMachine.HoldingMasters, oOutputStream)
        End Using
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を作成しました。")
    End Sub

    Protected Sub CreateFileOfMadoProVerInfo(ByVal sMonitorMachineId As String, ByVal sTermId As String, ByVal oTermMachine As TermMachine, ByVal sContextDir As String)
        Dim ar As Integer = DirectCast(oTermMachine.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
        Dim sFileName As String = _
           "MadoProVerInfo_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & _
           ".dat"
        Dim sFilePath As String = Path.Combine(sContextDir, sFileName)
        Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
            'NOTE: 窓処のプログラムバージョン情報には、改札機のプログラムバージョン情報と異なり、
            '無駄な基本ヘッダ部は存在しない。
            'TODO: いつかはインタフェース仕様を共通化したい。
            'ExVersionInfoFileHeader.WriteToStream(&H87, GetEkCodeOf(sTermId), DateTime.Now, 1, oOutputStream)
            ExProgramVersionInfoForY.WriteToStream(oTermMachine.HoldingPrograms(0), oOutputStream, ar)
            ExProgramVersionInfoForY.WriteToStream(oTermMachine.HoldingPrograms(1), oOutputStream, 0)
        End Using
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を作成しました。")
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
            Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を配信結果 [" & deliveryResult.ToString("X2") & "] で作成しました。")
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
            Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を配信結果 [" & deliveryResult.ToString("X2") & "] で作成しました。")
            Exit Do
        Loop
    End Sub

    Protected Function CreateConStatus(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachineに設定されている統括接続状態と
        'oMonitorMachine.TermMachinesに設定されている窓処接続状態をもとに、
        'sContextDirにExtOutput.datを作成する。

        Dim oReqTeleg As EkByteArrayGetReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sContextDir, "ConStatusGetReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkByteArrayGetReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream))
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "電文書式に不正があります。")
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
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を作成しました。")
        sResult = sFilePath

        Return True
    End Function

    Protected Function CreateMadoMasVerInfo(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        'oMonitorMachine.TermMachinesに設定されているマスタ保持状態をもとに、
        'sContextDirに号機別のMadoMasVerInfo_RRRSSSCCCCUU.datを作成する。
        'また、過去のものがあれば消す。

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

        'oMonitorMachine.TermMachinesに設定されている窓処向けプログラム保持状態をもとに、
        'sContextDirに号機別のMadoProVerInfo_RRRSSSCCCCUU.datを作成する。
        'また、過去のものがあれば消す。

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

        'sMachineDirの#PassiveDllReq.datが示すファイルをもとに
        '統括が保持する窓処向けマスタ（oMonitorMachine.HoldingMasters）と
        '窓処への配信待ちマスタ（oTermMachine.PendingMasters）を追加し、
        'sContextDirにExtOutput.datを作成する。
        'ただし、データに何らかの異常がある場合は、
        'これらを行わずに、ContinueCodeが
        'FinishWithoutStoringのExtOutput.datを作成する。

        'NOTE: ContinueCodeがFinishのExtOutput.datを作成した場合は、
        'DL完了通知も作成しなければならない。これについては、
        'マスタ適用リストに記載された窓処(t)のマスタ保持状態
        '（oMonitorMachine.TermMachines(t).HoldingMasters）をこの場で
        '更新することにした上で、DL完了通知もこの場でsContextDirに
        '作成するのが簡単であるが、窓処まで届いていない期間も
        '再現したいので、このアプリにDeliverMadoMas処理を用意し、
        'シミュレータ本体からその処理を要求された際に、窓処の
        'マスタ保持状態を更新する方式とする。

        'NOTE: 「最終受信物（データ本体）を配信完了していない場合に、マスタバージョン
        'やパターン番号がそれと異なる受信物（データ本体や適用リスト）を受け入れない」
        'ように統括部分を作ったとしても、キューには、マスタバージョンやパターン番号
        'が同一のものだけが入るとは限らない。具体的には、前半にマスタバージョンと
        'パターン番号が１世代前のものが（１つまたは複数）入り、後半にマスタバージョン
        'とパターン番号が最新のものが（１つまたは複数）入る場合があるはずである。
        '蛇足であるが、この場合、統括自身が後半のものを受け入れていることから、前半の
        'ものと同じマスタバージョンおよびパターン番号が付与されたデータ本体を全窓処に
        '配信済みであると言える。つまり、各キューにおける前半の情報は、当該窓処に
        '対してデータ本体を配信完了させた後に、同じマスタバージョン・同じパターン番号の
        'データ本体または適用リストを受信した際に、できたものであると言い切れる。

        Dim d As DateTime = DateTime.Now

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "電文書式に不正があります。")
            Return False
        End If

        Log.Info(sMonitorMachineId, "適用リストのファイル名は [" & oReqTeleg.ListFileName & "] です。")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "マスタ本体のファイル名は [" & oReqTeleg.DataFileName & "] です。")
        Else
            Log.Info(sMonitorMachineId, "マスタ本体のファイル名はありません。")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "適用リストのファイル名が不正です。")
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
                Log.Error(sMonitorMachineId, "マスタ本体のファイル名が不正です。")
                Return False
            End If
            If EkMasterDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "ファイル名（適用先機種）に不整合があります。")
                Return False
            End If
            If EkMasterDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "ファイル名（マスタ種別）に不整合があります。")
                Return False
            End If
            If EkMasterDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "ファイル名（パターンNo）に不整合があります。")
                Return False
            End If
            If EkMasterDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "ファイル名（マスタVer）に不整合があります。")
                Return False
            End If
        End If

        'NOTE: 基本的に、oReqTelegのSubObjCodeとListFileNameの整合性は、シナリオ側
        'で保証する想定である。また、DataFileNameが空でない場合のListFileNameとの
        '整合性についても、シナリオ側で保証する想定である。シナリオ側でなら、
        '整合性のないREQに対し、NAKを返信するように設定できるためである。
        'ただし、たとえ、シナリオ側でチェックしないとしても、そもそも、整合性のない
        '配信を行えてしまえたなら、それは運管システムの不具合のはずであるから、
        'シナリオが実施結果NGで終了する方がよい。以上のことから、ここでも
        '整合性のチェックを行い、整合性がない場合は、否定応答を返却する。
        If sApplicableModel <> Config.TermModelSym Then
            Log.Error(sMonitorMachineId, "ファイル名（適用先機種）が不正です。")
            Return False
        End If
        If Not ExConstants.MadoMastersSubObjCodes.ContainsKey(sDataKind) Then
            Log.Error(sMonitorMachineId, "ファイル名（マスタ種別）が不正です。")
            Return False
        End If
        If oReqTeleg.SubObjCode <> ExConstants.MadoMastersSubObjCodes(sDataKind) Then
            Log.Error(sMonitorMachineId, "電文のサブ種別がファイル名（マスタ種別）と整合していません。")
            Return False
        End If

        Dim oHoldingMasters As List(Of HoldingMaster) = Nothing
        oMonitorMachine.HoldingMasters.TryGetValue(sDataKind, oHoldingMasters)

        Dim sDataHashValue As String = Nothing
        Dim dataAcceptDate As DateTime
        Dim oDataFooter As Byte() = Nothing

        '統括が保持している中から、適用リストと組み合わせることができるマスタ本体を探す。
        'NOTE: 組み合わせることができるマスタ本体がないときは、sDataHashValueがNothingになる。
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
            '統括が保持していないマスタに関して、適用リストのみを送り付けられた場合は、
            'ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            'NOTE: そのケースで本物の統括がどのような反応を示すかは、分かっていない。
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "適用リストに紐づくマスタ本体がありません。")
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            'NOTE: 本物の統括は、このようにマスタ本体を受信した場合でも、その時点のシステム日時が
            '同名マスタを過去に受信した際のシステム日時よりも古い日時になっている場合は、
            '今回受信したマスタ本体を窓処へのDLL対象にはしない（今回受信したものも含めて、
            '受信日時が最も新しいものをDLL対象と認識し、窓処が既にそれを所持しているなら
            'DLLは行わないと思われる）。しかし、さすがにそれはイレギュラーなケースである
            'ため、シミュレータで無理に再現はしないことにする。

            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
            dataAcceptDate = d

            'マスタ本体のフッタ情報を読取る。
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
                Log.Error(sMonitorMachineId, "マスタ本体のフッタ情報の読取りで例外が発生しました。", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End Try

            '読取ったフッタ情報に問題がある場合は、
            'ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            Dim footerView As New ExMasterDataFooter(oDataFooter)
            Dim sViolation As String = footerView.GetFormatViolation()
            If sViolation IsNot nothing Then
                Log.Error(sMonitorMachineId, "マスタ本体のフッタ情報が異常です。" & vbCrLf & sViolation)
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End If
        End If

        Dim sListHashValue As String = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        '適用リストの内容を解析する。
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, Integer)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            'とりあえず、全て読み取る。
            'OPT: 「適用リストの内容を表示する機能」を追加した際に、手っ取り早く
            '実装するために、このように二度読みすることになっている。
            '非効率であり、改善の余地がある。
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '１行ずつ読み取る。
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '以下、適用リストの内容をチェックし、問題がある場合は、
                'ContinueCode.FinishWithoutStoringのREQ電文を作成する。

                'ヘッダ部の１行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストが空です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の１行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "適用リスト1行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '作成年月日をチェックする。
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された作成年月日が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'リストVerをチェックする。
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたリストVerがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストに2行目がありません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "適用リスト2行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'パターンNoをチェックする。
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたパターンNoがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'マスタVerをチェックする。
                If Not dataVersion.ToString("D3").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたマスタVerがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '機種コードをチェックする。
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された機種がファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '適用リストの３行目以降から、oMonitorMachineに関係する号機を抽出する。
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '読み込んだ行を列に分割する。
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 3 Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の項目数が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'サイバネ線区駅順コードの書式をチェックする。
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の駅コードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'コーナーコードの書式をチェックする。
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目のコーナーコードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '号機番号の書式をチェックする。
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の号機番号が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '行の重複をチェックする。
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目が既出の行と重複しています。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '行の内容を一時保存する。
                    oListedMachines.Add(sLineKey, 0)

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "適用リストの読取りで例外が発生しました。", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
            Return True
        End Try

        '統括の行や配信待ちの行は、DLL要求そのものを表す方がわかりやすいので、
        'DLL要求にマスタ本体が含まれていない場合は、ここで無効値に差し替えることにする。
        'NOTE: 実際の統括で、そのようなDLL要求を受けた場合に、窓処にDLLするマスタ本体は、
        '要求を受けた時点で決めるわけではなく、窓処へのDLLを行う時点で決める（その時点の
        '最新のものをDLLする）ようであるため、そのように表示するのは実態に合っている。
        'TODO: もしそのように表示されるのが嫌なら、ここをコメントアウトすればよい。
        If oReqTeleg.DataFileName.Length = 0 Then
            dataAcceptDate = Config.EmptyTime
            oDataFooter = Nothing
            sDataHashValue = Nothing
        End If

        '一時保存していた行が示す各機器に、配信のための情報をキューイングする。
        Dim targetTermCount As Integer = 0
        For Each sName As String In oListedMachines.Keys
            '行がoMonitorMachineに関係する場合
            Dim oTerm As TermMachine = Nothing
            If oMonitorMachine.TermMachines.TryGetValue(sName, oTerm) = True Then
                Log.Debug(sMonitorMachineId, "適用リストに記載された端末 [" & sName & "] の行をキューイングします。")
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
        Log.Debug(sMonitorMachineId, "適用リストに記載された" & oListedMachines.Count.ToString() & "台のうち、" & targetTermCount.ToString() & "台が当該機器の端末でした。")

        'NOTE: 下記のケースで、本物の統括がどのような反応を示すかは、よくわからない。
        If targetTermCount = 0 Then
            Log.Error(sMonitorMachineId, "配信を生み出さない適用リストを受信しました。")
            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
            Return True
        End If

        '統括のマスタ保持状態を更新する。
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
        Log.Info(sMonitorMachineId, "受け入れが完了しました。")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverMadoMas(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        '指定された窓処のキューから全てのマスタ適用リストを取り出し、
        'マスタ適用リストごとに、当該窓処(t)のマスタ保持状態
        '（oMonitorMachine.TermMachines(t).HoldingMasters）を
        '更新し、sMachineDirにマスタ適用リスト別・窓処別の
        '#MadoMasDlReflectReq_RRRSSSCCCCUU_N.dat（Nは0～）を作成する。
        'また、マスタ保持状態を更新した窓処については、
        'sContextDirにMadoMasVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、MadoMasDlReflectReq_RRRSSSCCCCUU_N.datと
        'MadoMasVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回の配信と無関係なもの、今回更新
        'していない窓処のもの）を削除する。

        'NOTE: これらのファイル名のRRRSSSCCCCUUは端末の機器コードで
        'あるが、これらのファイルが全端末分（シナリオ内で
        '「%T3R%T3S%T4C%T2U」と記述した場合に複製される全行分）
        '作成されるとは限らない。
        'よって、シナリオは、当該ファイルを送信する際、
        'ActiveOneではなく、TryActiveOneを使用する。

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
                    Log.Debug(sMonitorMachineId, "端末 [" & sTermMachineId & "] に対するマスタ配信はありません。")
                    Continue For
                ElseIf oTerm.Tk2Status <> &H2 Then
                    Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] については、統括DL系状態が接続以外に設定されているため、配信処理を保留します。")
                    Continue For
                End If

                Dim isHoldingMasUpdated As Boolean = False

                'マスタ種別ごとに処理を行う。
                For Each oKindEntry As KeyValuePair(Of String, LinkedList(Of PendingMaster)) In oTerm.PendingMasters
                    'TODO: 本物の統括は、最後にキューイングした要求に対応する配信しか行わないかもしれない。
                    '正確には、「ユーザの意思のみに応じて（運管からの要求のみをトリガーに）端末への配信を行い、
                    '一定時間内に結果（DL完了通知）をユーザに提示する（その場で端末に配信できないなら、当該
                    '端末への配信は諦めて、異常のDL完了通知を発生させる）」思想をやめて、監視盤と同じように、
                    '端末への配信を保留する（端末との回線状態によって、予期できないタイミングでDL完了通知を
                    '上げ得る）ようになってしまったが、現実的には保留にしたもの全てを配信するわけにもいかず、
                    '運管からの要求をマージして、端末ごとに何を保持している状態にするべきかを（わざわざ中継機
                    'の中に）管理し、実際の端末の保持状態との差から、必要なものだけを端末に配信する感じに
                    'なっているかもしれない。
                    '監視盤への歩み寄りという観点で、最悪そのこと自体は仕方ないとしても、端末に送信した
                    'ファイルに関連するDL完了通知しか発生させない可能性もある（受け付けた要求を実行しない
                    'のなら、その旨を表す正常以外のDL完了通知を発生させればよいはずであるが...）。
                    'もしそうだとしたら、運管に対する働きが実機とシミュレータで違う... ということになって
                    'しまうので、シミュレータでも、最後にキューイングされているもの以外を読み捨てる（※）
                    'などの動作にした方がよいかもしれない（※ 実際は、もっと複雑と思われる）。
                    'なお、たとえそうするにしても、TermMachineクラスのPendingMastersは必要である。
                    'シミュレータの機能として、窓処に未配信のものをユーザに示す必要があるためである。
                    'TODO: もし、上記したような事実があり、それを追認するしかないなら、運管側は、
                    'DL完了通知を受信した際、それと同じ種別の（バージョン等は異なる）マスタの
                    'それと同じ端末への配信状態で「送信中」のものがあれば「スキップ」等に変更する
                    'などの対応を入れると、幾分ましな方向に戻せるかもしれない。

                    '配信処理に使っていない全適用リストについて処理を行う。
                    For Each oPenMas As PendingMaster In oKindEntry.Value
                        'NOTE: 実体のない（ListHashValue Is Nothing の）適用リストで配信が行われる可能性は想定しない。
                        Log.Info(sMonitorMachineId, "適用リスト [" & oPenMas.ListVersion.ToString() & "] に基づき、端末 [" & sTermMachineId & "] に対する種別 [" & oKindEntry.Key & "] パターンNo [" & oPenMas.DataSubKind.ToString() & "] マスタVer [" & oPenMas.DataVersion.ToString() & "] のマスタ配信処理を行います...")

                        '配信結果（「正常」「適用済み」など）を決める。
                        Dim deliveryResult As Byte = &H0
                        Dim isOutOfArea As Boolean = False

                        Dim oLatestMas As HoldingMaster = FindLatestMasterDataInMonitorMachine(oMonitorMachine, oKindEntry.Key, oPenMas.DataSubKind, oPenMas.DataVersion)
                        If oLatestMas Is Nothing Then
                            'TODO: 基本的にあり得ないはずの状況であるが、本物の統括に合わせたい。
                            Log.Warn(sMonitorMachineId, "想定外の状況です。配信しなければならないマスタ本体が統括にありません。")
                            deliveryResult = &H5 'NOTE: 適当なコードがないので、とりあえず正常以外にしておく。
                        End If

                        If deliveryResult = &H0 Then
                            Dim ar As Integer = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
                            Dim oMasKinds As HashSet(Of String) = Nothing
                            If ExConstants.MadoAreasMasters.TryGetValue(ar, oMasKinds) = False OrElse _
                               Not oMasKinds.Contains(oKindEntry.Key) Then
                                'NOTE: 配信しないのに「適用済み」は不適切に思えるが、監視盤＆改札機は実際にこのように動作する。
                                'TODO: 統括＆窓処が同じであるとは限らないので確認する。
                                Log.Error(sMonitorMachineId, "この種別のマスタはエリア [" & ar.ToString() &"] の端末には配信できません。")
                                deliveryResult = &HF
                                isOutOfArea = True
                            End If
                        End If

                        If deliveryResult = &H0 Then
                            'TODO: 本物の統括に合わせたい。
                            '本物の統括は、「適用済み」対応の際に、受け入れ日時ではなく、ハッシュ値などを比較するようになったかもしれない。
                            '監視盤の場合はどうなのかも含め、理想形を確認するべきである。
                            Dim oMas As HoldingMaster = Nothing
                            If oTerm.HoldingMasters.TryGetValue(oKindEntry.Key, oMas) = True AndAlso _
                               oMas.DataSubKind = oPenMas.DataSubKind AndAlso _
                               oMas.DataVersion = oPenMas.DataVersion AndAlso _
                               oMas.DataAcceptDate = oLatestMas.DataAcceptDate Then
                                '窓処が保持しているものと同じものを配信することになる場合は、
                                '配信結果を「適用済み」とする。
                                Log.Warn(sMonitorMachineId, "当該端末に対しては当該マスタを適用済み（※配信済み）です。再配信は行いません。")
                                deliveryResult = &HF
                            End If
                        End If

                        '窓処のマスタ保持状態を更新する。
                        If deliveryResult = &H0 OrElse isOutOfArea Then
                            'NOTE: 窓処は適用リストを保持しないが、どの適用リストの指示によって
                            '当該窓処にマスタ本体の配信が行われたかが分かる方がよいので、
                            '適用リストバージョンもセットすることにする。
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
                                Log.Info(sMonitorMachineId, "当該端末に対して当該マスタの配信を行いました。")
                            Else
                                Log.Warn(sMonitorMachineId, "当該端末に対して当該マスタの配信を行いました。これは自動改札機システムの生成するバージョン情報を再現させるための特別措置ですので、ご注意ください。")
                            End If
                        End If

                        '#MadoMasDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
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

        '統括の窓処マスタ保持状態を更新する。
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
            Log.Error(sMonitorMachineId, "プログラム本体のハッシュ値算出で例外が発生しました。", ex)
            Return False
        End Try

        Dim content As MadoProgramContent
        Try
            content = ExtractMadoProgramCab(sFilePath, Path.Combine(sContextDir, "MadoPro"))
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "プログラム本体の解析で例外が発生しました。", ex)
            Return False
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "バージョンリストからのエリアNoの抽出で例外が発生しました。", ex)
            Return False
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "バージョンリストからの代表Verの抽出で例外が発生しました。", ex)
            Return False
        End Try

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn(sMonitorMachineId, "プログラムの動作許可日が未来日に設定されています。" & content.RunnableDate & "まで適用できませんのでご注意ください。")
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

        'sMachineDirの#PassiveDllReq.datが示すファイルをもとに
        '統括が保持する窓処向けプログラム（oMonitorMachine.HoldingPrograms）と、
        '窓処への配信待ちプログラム（oTermMachine.PendingMasters）を追加し、
        'sContextDirにExtOutput.datを作成する。
        'ただし、データに何らかの異常がある場合は、
        'これらを行わずに、ContinueCodeが
        'FinishWithoutStoringのExtOutput.datを作成する。

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "電文書式に不正があります。")
            Return False
        End If

        Log.Info(sMonitorMachineId, "適用リストのファイル名は [" & oReqTeleg.ListFileName & "] です。")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "プログラム本体のファイル名は [" & oReqTeleg.DataFileName & "] です。")
        Else
            Log.Info(sMonitorMachineId, "プログラム本体のファイル名はありません。")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "適用リストのファイル名が不正です。")
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
                Log.Error(sMonitorMachineId, "プログラム本体のファイル名が不正です。")
                Return False
            End If
            If EkProgramDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "ファイル名（適用先機種）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "ファイル名（プログラム種別）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "ファイル名（エリアNo）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "ファイル名（代表Ver）に不整合があります。")
                Return False
            End If
        End If

        'NOTE: 基本的に、oReqTelegのSubObjCodeとListFileNameの整合性は、シナリオ側
        'で保証する想定である。また、DataFileNameが空でない場合のListFileNameとの
        '整合性についても、シナリオ側で保証する想定である。シナリオ側でなら、
        '整合性のないREQに対し、NAKを返信するように設定できるためである。
        'ただし、たとえ、シナリオ側でチェックしないとしても、そもそも、整合性のない
        '配信を行えてしまえたなら、それは運管システムの不具合のはずであるから、
        'シナリオが実施結果NGで終了する方がよい。以上のことから、ここでも
        '整合性のチェックを行い、整合性がない場合は、否定応答を返却する。
        If sApplicableModel <> Config.TermModelSym Then
            Log.Error(sMonitorMachineId, "ファイル名（適用先機種）が不正です。")
            Return False
        End If
        If sDataKind <> "YPG" Then
            Log.Error(sMonitorMachineId, "ファイル名（プログラム種別）が不正です。")
            Return False
        End If
        If oReqTeleg.SubObjCode <> &H0 Then
            Log.Error(sMonitorMachineId, "電文のサブ種別が不正です。")
            Return False
        End If

        Dim sDataHashValue As String = Nothing
        Dim dataAcceptDate As DateTime
        Dim sRunnableDate As String = Nothing
        Dim sArchiveCatalog As String = Nothing
        Dim oVersionListData As Byte() = Nothing

        '統括が保持している中から、適用リストと組み合わせることができるプログラム本体を探す。
        'NOTE: 組み合わせることができるプログラム本体がないときは、sDataHashValueがNothingになる。
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
            '統括が保持していないバージョンの窓処向けプログラムに関して、適用リストのみを
            '送り付けられた場合は、ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            'NOTE: そのケースで本物の統括がどのような反応を示すかは、分かっていない。
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "適用リストに紐づくプログラム本体がありません。")
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            'NOTE: 本物の統括は、このようにプログラム本体を受信した場合でも、その時点のシステム日時が
            '同名プログラムを過去に受信した際のシステム日時よりも古い日時になっている場合は、
            '今回受信したプログラム本体を窓処へのDLL対象にはしない（今回受信したものも含めて、
            '受信日時が最も新しいものをDLL対象と認識し、窓処が既にそれを所持しているなら
            'DLLは行わないと思われる）。しかし、さすがにそれはイレギュラーなケースである
            'ため、シミュレータで無理に再現はしないことにする。

            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
            dataAcceptDate = d

            Dim content As MadoProgramContent
            Try
                Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
                content = ExtractMadoProgramCab(sDataFilePath, Path.Combine(sContextDir, "MadoPro"))
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "プログラム本体の解析で例外が発生しました。", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                Return True
            End Try

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData).Equals(dataSubKind.ToString("X2")) Then
                'NOTE: CABの中身を確認できる方が親切なので、このまま処理を強行する。
                'TODO: 本物の統括の動作に合わせた方がよい？
                Log.Warn(sMonitorMachineId, "バージョンリストに記載されたエリアNoがファイル名と整合していませんが、処理を強行します。")
            End If

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", "").Equals(dataVersion.ToString("D8")) Then
                'NOTE: CABの中身を確認できる方が親切なので、このまま処理を強行する。
                'TODO: 本物の統括の動作に合わせた方がよい？
                Log.Warn(sMonitorMachineId, "バージョンリストに記載された代表Verがファイル名と整合していませんが、処理を強行します。")
            End If

            sRunnableDate = content.RunnableDate
            sArchiveCatalog = content.ArchiveCatalog
            oVersionListData = content.VersionListData
        End If

        Dim sListHashValue As String = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        'TODO: 各窓処について、適用面のバージョンと待機面のバージョンを調べ、
        '待機面に書き込み可能とするバージョンを制限するべきかもしれない。

        '適用リストの内容を解析する。
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, String)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            'とりあえず、全て読み取る。
            'OPT: 「適用リストの内容を表示する機能」を追加した際に、手っ取り早く
            '実装するために、このように二度読みすることになっている。
            '非効率であり、改善の余地がある。
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '１行ずつ読み取る。
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '以下、適用リストの内容をチェックし、問題がある場合は、
                'ContinueCode.FinishWithoutStoringのREQ電文を作成する。

                'ヘッダ部の１行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストが空です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の１行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "適用リスト1行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '作成年月日をチェックする。
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された作成年月日が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'リストVerをチェックする。
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたリストVerがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストに2行目がありません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "適用リスト2行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                'エリアNoをチェックする。
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたエリアNoがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '代表Verをチェックする。
                If Not dataVersion.ToString("D8").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された代表Verがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '機種コードをチェックする。
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された機種がファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                    Return True
                End If

                '適用リストの３行目以降から、oMonitorMachineに関係する号機を抽出する。
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '読み込んだ行を列に分割する。
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 4 Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の項目数が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'サイバネ線区駅順コードの書式をチェックする。
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の駅コードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'コーナーコードの書式をチェックする。
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目のコーナーコードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '号機番号の書式をチェックする。
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の号機番号が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '適用日のレングスをチェックする。
                    If aColumns(3).Length <> 8 AndAlso aColumns(3).Length <> 0 Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '適用日がブランクでない場合、値をチェックする。
                    If aColumns(3).Length = 8 Then
                       If Not Utility.IsDecimalStringFixed(aColumns(3), 0, 8) Then
                            Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                            Return True
                        End If

                        Dim applicableDate As DateTime
                        If Not aColumns(3).Equals("99999999") AndAlso _
                           DateTime.TryParseExact(aColumns(3), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, applicableDate) = False Then
                            Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    '行の重複をチェックする。
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目が既出の行と重複しています。")
                        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '行の内容を一時保存する。
                    oListedMachines.Add(sLineKey, aColumns(3))

                    '行がoMonitorMachineに関係する場合
                    'TODO: ここでチェックしてDLL要求シーケンスを失敗させるのではなく、
                    '号機別にDL完了通知を出し分けるべきかもしれない。
                    Dim oTerm As TermMachine = Nothing
                    If oMonitorMachine.TermMachines.TryGetValue(sLineKey, oTerm) = True Then
                        'エリア番号をチェックする。
                        'NOTE: 監視盤と違い、統括で適用リストの全号機をまとめて扱う意味はないので、
                        'ここでチェックは行わず、DeliverMadoProにて、適用先号機ごとに
                        '適用エリア異常のDL完了通知を発生させることにしている（そうしないと
                        '適用エリア異常の使い道がない）。
                        'TODO: 本物の統括に合わせる。
                        'TODO: 窓処のプログラムにエリア番号0が指定されることはない（異常事態）かもしれない。
                        'If dataSubKind <> 0 AndAlso _
                        '   dataSubKind <> DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer) Then
                        '    Log.Error(sMonitorMachineId, "適用リストに記載された端末 [" & sLineKey & "] の所属エリアが、適用リストの対象エリアと異なります。")
                        '    sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
                        '    Return True
                        'End If
                    End If

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "適用リストの読取りで例外が発生しました。", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
            Return True
        End Try

        '統括の行や配信待ちの行は、DLL要求そのものを表す方がわかりやすいので、
        'DLL要求にプログラム本体が含まれていない場合は、ここで無効値に差し替えることにする。
        'NOTE: 実際の統括で、そのようなDLL要求を受けた場合に、窓処にDLLするプログラム本体は、
        '要求を受けた時点で決めるわけではなく、窓処へのDLLを行う時点で決める（その時点の
        '最新のものをDLLする）ようであるため、そのように表示するのは実態に合っている。
        'TODO: もしそのように表示されるのが嫌なら、ここをコメントアウトすればよい。
        If oReqTeleg.DataFileName.Length = 0 Then
            dataAcceptDate = Config.EmptyTime
            sRunnableDate = Nothing
            sArchiveCatalog = Nothing
            oVersionListData = Nothing
            sDataHashValue = Nothing
        End If

        '一時保存していた行が示す各機器に、配信のための情報をキューイングする。
        Dim targetTermCount As Integer = 0
        Dim targetTermFullCount As Integer = 0
        For Each oApplyEntry As KeyValuePair(Of String, String) In oListedMachines
            '行がoMonitorMachineに関係する場合
            Dim oTerm As TermMachine = Nothing
            If oMonitorMachine.TermMachines.TryGetValue(oApplyEntry.Key, oTerm) = True Then
                '適用日が現在の運用日付と同じか未来あるいは「19000101」か「99999999」の場合のみ、
                '配信が（このアプリの場合は、DL完了通知が）必要とみなす。
                'NOTE: 最新の監視盤（実機）は、この条件に該当していない行について、「適用済み」のDL完了通知を
                '送り付けるようになっていた気がする（そのために、運管側は、既に「正常」になっている場合は
                '「適用済み」のDL完了通知を無視しなければならなくなった）。統括（実機）も同じである可能性がある。
                'その思想からすると、 統括（実機）は、そもそも適用日が過去日の行であっても、当該行の窓処に
                '当該プログラムを未配信であれば、配信してしまうのかもしれない。
                'もしそうだとすると、かなり問題である。
                '運管は、適用日が過去日の行は、適用日がブランクの行と同じ扱いにすることになっている。
                'それゆえに、適用リストにそのような行しかなければ、統括に対して配信しない。
                'また、DLLシーケンスが完了した際（統括まで配信が完了した際）も、そのような適用日が
                '記載されている窓処については、配信状態を「配信中」にはしない。
                '運管の動作はI/F仕様（ツール仕様書の別紙6）に完全に合致している。
                'TODO: 本物の監視盤が「適用済み」を送り付けてくる件について、システム試験では、それを
                '無視するように運管側を改造し、監視盤チームの考える仕様通りということでOKとしたが、
                '監視盤や統括の実装がどうなっているのか、システムとして問題がないのか、システム試験で
                '実施していないケース（適用日が過去日の行の改札機に対して未配信だったケース等）に
                'ついても、運管の動作も含めて、検証するべきである。
                If oApplyEntry.Value.Length = 8 AndAlso _
                  (oApplyEntry.Value.Equals("19000101") OrElse _
                   String.CompareOrdinal(oApplyEntry.Value, sServiceDate) >= 0) Then
                    Log.Debug(sMonitorMachineId, "適用リストに記載された端末 [" & oApplyEntry.Key & "] 適用日 [" & oApplyEntry.Value & "] の行をキューイングします。")
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
                    Log.Debug(sMonitorMachineId, "適用リストに記載された端末 [" & oApplyEntry.Key & "] 適用日 [" & oApplyEntry.Value & "] の行は除外します。")
                End If
                targetTermFullCount += 1
            End If
        Next oApplyEntry
        Log.Debug(sMonitorMachineId, "適用リストに記載された" & oListedMachines.Count.ToString() & "台のうち、" & targetTermFullCount.ToString() & "台が当該機器の端末でした。そのうち" & targetTermCount.ToString() & "台の適用日が有効でした。")

        'NOTE: 下記のケースで、本物の統括がどのような反応を示すかは、よくわからない。
        If targetTermCount = 0 Then
            Log.Error(sMonitorMachineId, "配信を生み出さない適用リストを受信しました。")
            sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "FinishWithoutStoring")
            Return True
        End If

        '統括の窓処向けプログラム保持状態を更新する。
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
        Log.Info(sMonitorMachineId, "受け入れが完了しました。")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverMadoPro(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        '指定された窓処のキューから全てのプログラム適用リストを取り出し、
        'プログラム適用リストごとに、当該窓処(t)のプログラム保持状態
        '（oMonitorMachine.TermMachines(t).HoldingPrograms）を
        '更新し、sMachineDirに適用リスト別・窓処別の
        '#MadoProDlReflectReq_RRRSSSCCCCUU_N.dat（Nは0～）を作成する。
        'また、プログラム保持状態を更新した窓処については、
        'sContextDirにMadoProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、MadoProDlReflectReq_RRRSSSCCCCUU_N.datと
        'MadoProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回の配信と無関係なもの、今回更新
        'していない窓処のもの）を削除する。

        'NOTE: これらのファイル名のRRRSSSCCCCUUは端末の機器コードで
        'あるが、これらのファイルが全端末分（シナリオ内で
        '「%T3R%T3S%T4C%T2U」と記述した場合に複製される全行分）
        '作成されるとは限らない。
        'よって、シナリオは、当該ファイルを送信する際、
        'ActiveOneではなく、TryActiveOneを使用する。

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
                    Log.Debug(sMonitorMachineId, "端末 [" & sTermMachineId & "] に対するプログラム配信はありません。")
                    Continue For
                ElseIf oTerm.Tk2Status <> &H2 Then
                    Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] については、統括DL系状態が接続以外に設定されているため、配信処理を保留します。")
                    Continue For
                End If

                Dim isHoldingProUpdated As Boolean = False

                'TODO: 本物の統括は、最後にキューイングした要求に対応する配信しか行わないかもしれない。
                '正確には、「ユーザの意思のみに応じて（運管からの要求のみをトリガーに）端末への配信を行い、
                '一定時間内に結果（DL完了通知）をユーザに提示する（その場で端末に配信できないなら、当該
                '端末への配信は諦めて、異常のDL完了通知を発生させる）」思想をやめて、監視盤と同じように、
                '端末への配信を保留する（端末との回線状態によって、予期できないタイミングでDL完了通知を
                '上げ得る）ようになってしまったが、現実的には保留にしたもの全てを配信するわけにもいかず、
                '運管からの要求をマージして、端末ごとに何を保持している状態にするべきかを（わざわざ中継機
                'の中に）管理し、実際の端末の保持状態との差から、必要なものだけを端末に配信する感じに
                'なっているかもしれない。
                '監視盤への歩み寄りという観点で、最悪そのこと自体は仕方ないとしても、端末に送信した
                'ファイルに関連するDL完了通知しか発生させない可能性もある（受け付けた要求を実行しない
                'のなら、その旨を表す正常以外のDL完了通知を発生させればよいはずであるが...）。
                'もしそうだとしたら、運管に対する働きが実機とシミュレータで違う... ということになって
                'しまうので、シミュレータでも、最後にキューイングされているもの以外を読み捨てる（※）
                'などの動作にした方がよいかもしれない（※ 実際は、もっと複雑と思われる）。
                'なお、たとえそうするにしても、TermMachineクラスのPendingProgramsは必要である。
                'シミュレータの機能として、窓処に未配信のものをユーザに示す必要があるためである。
                'TODO: もし、上記したような事実があり、それを追認するしかないなら、運管側は、
                'DL完了通知を受信した際、それと同じ種別の（バージョン等は異なる）プログラムの
                'それと同じ端末への配信状態で「送信中」のものがあれば「スキップ」等に変更する
                'などの対応を入れると、幾分ましな方向に戻せるかもしれない。

                '配信処理に使っていない全適用リストについて処理を行う。
                For Each oPenPro As PendingProgram In oTerm.PendingPrograms
                    'NOTE: 実体のない（ListHashValue Is Nothing の）適用リストで配信が行われる可能性は想定しない。
                    Log.Info(sMonitorMachineId, "適用リスト [" & oPenPro.ListVersion.ToString() & "] に基づき、端末 [" & sTermMachineId & "] に対するエリアNo [" & oPenPro.DataSubKind.ToString() & "] 代表Ver [" & oPenPro.DataVersion.ToString() & "] のプログラム配信処理を行います...")

                    If oPenPro.ApplicableDate.Equals("99999999") Then
                        Log.Info(sMonitorMachineId, "※当該端末に対する要求は改造中止要求です。")
                    End If

                    '当該端末に対して最後に配信した適用リストの情報を取得する。
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

                    '適用リストの配信結果（「正常」または「異常」「適用済み」）を決める。
                    Dim listDeliveryResult As Byte = &H0

                    'NOTE: 本物の統括は適用リストのハッシュ値ではなく、適用リストの統着日時を
                    '比較するかもしれないが、そうだとすると、適用リストに関する「適用済み」の
                    'DL完了通知は事実上発生しないことになるため、とりあえずハッシュ値を比較する
                    'ことにしている。
                    'TODO: 本物の統括に合わせる。
                    If latestListHashValue IsNot Nothing AndAlso _
                       oPenPro.DataSubKind = latestDataSubKind AndAlso _
                       oPenPro.DataVersion = latestDataVersion AndAlso _
                       oPenPro.ListVersion = latestListVersion AndAlso _
                       StringComparer.OrdinalIgnoreCase.Compare(oPenPro.ListHashValue, latestListHashValue) = 0 Then
                        '「窓処が適用待ちの部材と一緒に保持している適用リスト」と同じものを配信する
                        'ことになる場合は、配信結果を「適用済み」とする。
                        'NOTE: そのケースでは、改造中止要求の適用リストに対しても「適用済み（改造中止済み？）」
                        'で済ましてしまうが、本物の統括もそうであるかは不明。そもそも、前回の配信時に
                        '改造中止をしているとしたら、待機面から消えているはずなので、普通にはあり得ない
                        'ケースと思われる。
                        If oPenPro.ApplicableDate.Equals("99999999") Then
                            Log.Warn(sMonitorMachineId, "当該端末に対しては当該適用リストを配信済みです。適用リストの再配信を行いませんので、改造中止も行いません。")
                        Else
                            Log.Warn(sMonitorMachineId, "当該端末に対しては当該適用リストを配信済みです。適用リストの再配信は行いません。適用リストに基づくプログラム本体の配信も行いません。")
                        End If
                        listDeliveryResult = &HF
                    End If

                    If listDeliveryResult = &H0 Then
                        If oPenPro.ApplicableDate.Equals("99999999") Then
                            'NOTE: oPenProのバージョンのプログラムが既に適用中になっているケースも
                            '以下のケース（無効な改造中止要求）に当てはまるはずである。
                            'NOTE: 「oTerm.HoldingPrograms(1) Is Nothing」でない場合においては、
                            '万が一「oTerm.HoldingPrograms(1).DataVersion = 0」であるとしても、
                            'それは、端末がバージョン0のプログラムを保持しているということである。
                            'よって、oPenPro.DataVersionも0であり、エリア番号も一致するなら、
                            'oPenProは有効な改造中止であり、以下の条件が偽になってよい。
                            If oTerm.HoldingPrograms(1) Is Nothing OrElse _
                               oTerm.HoldingPrograms(1).DataSubKind <> oPenPro.DataSubKind OrElse _
                               oTerm.HoldingPrograms(1).DataVersion <> oPenPro.DataVersion Then
                                'NOTE: 本物の統括がこのように厳密な動作をするのかは、不明である。
                                Log.Error(sMonitorMachineId, "無効な改造中止要求です。当該端末において当該プログラムが適用待ちになっていません。")
                                listDeliveryResult = &H1
                            End If
                        End If
                    End If

                    'NOTE: 適用リストが適用済みの場合や、改造中止要求の適用リストが無効な場合は、
                    'プログラム本体のDL完了通知は発生させない。それらのケースでは、
                    'プログラム本体は配信対象でないはずであり、問題ないはず。
                    '改造中止の行を含む適用リストがプログラム本体とともにDLLされるケースは想定しない。
                    'TODO: 本物の統括＆窓処がどう動くかは分からない。
                    If listDeliveryResult = &H0 Then
                        If oPenPro.ApplicableDate.Equals("99999999") Then
                            'NOTE: 適用日が「99999999」の行の窓処については、プログラム本体のDL完了通知は
                            '（適用済みなども含めて）生成しないことにする。本物の統括がどうなのかは不明。
                            'TODO: 運管において、ある窓処に対するあるバージョンのプログラムの初回の配信指示で、
                            '適用リストに「99999999」を記載してしまったり、「99999999」が記載された適用リストで
                            '配信を行う際に「プログラム+プログラム適用リスト 強制配信」にチェックを入れて
                            'しまったりすると、プログラム本体に関する当該窓処の受信状態が「配信中」になり、
                            'それがそのまま残ってしまうと思われる。これについては、適用日「99999999」が指定
                            'された窓処について「配信中」のレコードを作成しないように、そして、できること
                            'なら「99999999」が記載された適用リストで「プログラム+プログラム適用リスト 強制配信」
                            'を指定できないように、運管の実装を改善するべきである。

                            '窓処のプログラム保持状態を更新する。
                            oTerm.HoldingPrograms(1) = Nothing
                            isHoldingProUpdated = True
                            Log.Info(sMonitorMachineId, "当該端末に対して改造中止を行いました。")
                        Else
                            'NOTE: たとえ適用リストに関するDL完了通知が「適用済み」であったとしても、
                            'プログラム本体のDL完了通知も生成する（適用リストに関する「適用済み」など
                            'という概念が持ち込まれたことで、違和感があるかもしれないが、運管からの
                            'DLL要求には、適用リストのバージョンなどに関係なく、個別に意味がある）。
                            'また、たとえ適用リストが適用済み（= 実際は、単なる送信済み）であったとしても、
                            'その適用リストにおいて、当該プログラム未適用の窓処に有意な適用日が記載されて
                            'いれば、プログラム本体については「適用済み」ではなく「正常」のDL完了通知を
                            '生成する。
                            'TODO: 本物の統括は、適用リストに関するDL完了通知が「適用済み」である場合に、
                            'プログラム本体のDL完了通知（おそらく「適用済み」）を生成しないかもしれない。
                            'その状況では、運管における当該窓処の当該プログラムの受信状態も「配信中」
                            'ではなく「正常」等になっていると思われるが、本当にその保証があるのか
                            '検証した方がよい。
                            'TODO: このアプリでは、適用済みか否かを判断する上で、バージョン等の他に統着日時
                            'を比較しているが、本物の統括に合わせたい。もしかすると、本物の統括（窓処）では
                            '統着日時ではなくハッシュ値などを比較するように思想が改められている可能性がある。

                            'プログラム本体の配信結果（「正常」または「適用済み」）を決める。
                            Dim dataDeliveryResult As Byte = &H0

                            '処理する適用リストと同エリア・同代表バージョンの統括保持の窓処プログラム本体の中で、
                            '最も新しいものを探す。
                            Dim oLatestPro As HoldingProgram = FindLatestProgramDataInMonitorMachine(oMonitorMachine, oPenPro.DataSubKind, oPenPro.DataVersion)
                            If oLatestPro Is Nothing Then
                                'TODO: 基本的にあり得ないはずの状況であるが、本物の統括に合わせたい。
                                Log.Warn(sMonitorMachineId, "想定外の状況です。配信しなければならないプログラム本体が統括にありません。")
                                dataDeliveryResult = &HB 'NOTE: 意味が違うかもしれないが、とりあえず正常以外にしておく。
                                listDeliveryResult = &H1 'TODO: 適用リストの配信結果は３種類しかない。本物は適用リストを配信するのかもしれない。
                            End If

                            If dataDeliveryResult = &H0 Then
                                Dim ar As Integer = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
                                'TODO: 本物の統括＆窓処は、ファイル名や適用リスト内に記載されているエリア番号をチェックしないかもしれない。
                                'TODO: 窓処のプログラムにエリア番号0が指定されることはない（異常事態）かもしれないので、
                                '前半の条件は余計かもしれない。。
                                If oPenPro.DataSubKind <> 0 AndAlso oPenPro.DataSubKind <> ar Then
                                    Log.Error(sMonitorMachineId, "当該エリアNoのプログラムはエリア [" & ar.ToString() &"] の端末には配信できません。")
                                    dataDeliveryResult = &H2
                                    listDeliveryResult = &H1 'TODO: 適用リストの配信結果は３種類しかない。本物は適用リストを配信するのかもしれない。
                                End If
                            End If

                            If dataDeliveryResult = &H0 Then
                                If oTerm.HoldingPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                                   oTerm.HoldingPrograms(0).DataVersion = oPenPro.DataVersion Then
                                    '窓処が適用中のものと同じバージョンのプログラムを窓処に配信すると、何らかの不都合がある
                                    'かもしれないので、異常扱いにする。
                                    'NOTE: 適用日前のものを窓処が適用しているはずはないし、適用日を過ぎたものを窓処に配信しようとする
                                    'はずもない。しかし、窓処が適用中のものと同バージョンのプログラムを、適用日当日に、
                                    '統括が受信したケースや、適用日前に統括が受信し、窓処に配信しないまま適用日が過ぎた
                                    'ケースなどは、あり得る。後者は本関数内で適用日と運用日を比較して、別の異常扱いにすることも
                                    '可能であるが、前者はそうはいかない。
                                    'TODO: とりあえず配信結果を「適用済み」とするが、本物の統括に合わせた方がよい。
                                    Log.Warn(sMonitorMachineId, "当該端末には同バージョンのプログラムを適用済みです。プログラム本体の再配信は行いません。")
                                    dataDeliveryResult = &HF
                                End If
                            End If

                            If dataDeliveryResult = &H0 Then
                                'TODO: 本物の統括に合わせたい。
                                '本物の統括は、「適用済み」対応の際に、受け入れ日時ではなく、ハッシュ値などを比較するようになったかもしれない。
                                'また、監視盤の場合はどうなのかも含め、理想形を確認するべきである。
                                If oTerm.HoldingPrograms(1) IsNot Nothing AndAlso _
                                   oTerm.HoldingPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                                   oTerm.HoldingPrograms(1).DataVersion = oPenPro.DataVersion AndAlso _
                                   oTerm.HoldingPrograms(1).DataAcceptDate = oLatestPro.DataAcceptDate Then
                                    '窓処が適用中のものと同じものを配信することになる場合は、
                                    '配信結果を「適用済み」とする。
                                    Log.Warn(sMonitorMachineId, "当該端末に対しては当該プログラムを適用済み（※配信済み）です。プログラム本体の再配信は行いません。")
                                    dataDeliveryResult = &HF
                                End If
                            End If

                            If dataDeliveryResult = &H0 Then
                                If Not oPenPro.ApplicableDate.Equals("19000101") AndAlso _
                                   String.CompareOrdinal(oPenPro.ApplicableDate, oPenPro.RunnableDate) < 0 Then
                                    Log.Error(sMonitorMachineId, "プログラムの動作許可日が適用日以降に設定されています。配信は行いません。")
                                    dataDeliveryResult = &HC
                                    listDeliveryResult = &H1 'TODO: 適用リストの配信結果は３種類しかない。本物は適用リストを配信するのかもしれない。
                                End If
                            End If

                            '窓処のプログラム保持状態を更新する。
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
                                Log.Info(sMonitorMachineId, "当該端末の待機面に対して当該プログラム本体の配信を行いました。")
                                Log.Info(sMonitorMachineId, "当該端末の待機面に対して当該適用リストの配信を行いました。")
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
                                    Log.Info(sMonitorMachineId, "当該端末の待機面に対して当該適用リストの配信を行いました。")
                                ElseIf oTerm.HoldingPrograms(0) IsNot Nothing AndAlso _
                                       oTerm.HoldingPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                                       oTerm.HoldingPrograms(0).DataVersion = oPenPro.DataVersion Then
                                    'NOTE: 実機の窓処＆統括がどのように動作するかは不明である。
                                    '端末は待機面に受信する仕様のはずであるが、待機面への受信と
                                    '受信したリストの適用を一度に済ませたと考えれば、説明はつく。
                                    'ただし、そのように強弁するには、oTerm.HoldingPrograms(1) Is Nothing
                                    'であることも条件に入れるべきである。
                                    'TODO: 実機も含めて、あるべき動作を確認する必要がある。
                                    oTerm.HoldingPrograms(0).ListVersion = oPenPro.ListVersion
                                    oTerm.HoldingPrograms(0).ListAcceptDate = oPenPro.ListAcceptDate
                                    oTerm.HoldingPrograms(0).ListDeliverDate = d
                                    oTerm.HoldingPrograms(0).ApplicableDate = oPenPro.ApplicableDate
                                    oTerm.HoldingPrograms(0).ListContent = oPenPro.ListContent
                                    oTerm.HoldingPrograms(0).ListHashValue = oPenPro.ListHashValue
                                    isHoldingProUpdated = True
                                    Log.Warn(sMonitorMachineId, "当該端末の適用面に対して当該適用リストの配信を行いました。この適用日は意味を持ちませんので注意してください。")
                                Else
                                    'NOTE: あり得ないはずである。
                                    Log.Error(sMonitorMachineId, "当該端末において、当該適用リストに紐づくプログラム本体がありません。適用リストの配信は行いません。")
                                    listDeliveryResult = &H1
                                End If
                            End If

                            'プログラム本体に関する#MadoProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
                            CreateFileOfMadoProDlReflectReq( _
                               &H91, _
                               oPenPro.DataVersion, _
                               dataDeliveryResult, _
                               sMonitorMachineId, _
                               sTermMachineId, _
                               sMachineDir)
                        End If
                    End If

                    'NOTE: この配信の前に直接投入を実施した場合など、改札機に適用リストが
                    '存在しない場合は、下記を行わない。
                    '実物の改札機システムの挙動を（良し悪しに関係なく）忠実に再現する。
                    If latestListHashValue IsNot Nothing Then
                        '適用リストに関する#MadoProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
                        CreateFileOfMadoProDlReflectReq( _
                           &H75, _
                           oPenPro.ListVersion, _
                           listDeliveryResult, _
                           sMonitorMachineId, _
                           sTermMachineId, _
                           sMachineDir)
                    Else
                        Log.Warn(sMonitorMachineId, "当該端末が適用リストを保持していなかったため、適用リストのDL完了通知は作成しませんでした。これは自動改札機システムに合わせた制限事項です。")
                    End If
                Next oPenPro
                oTerm.PendingPrograms.Clear()

                If isHoldingProUpdated Then
                    CreateFileOfMadoProVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
                End If

                UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
            End If
        Next oTermEntry

        '統括の窓処プログラム保持状態を更新する。
        TrimMonitorMachineHoldingPrograms(sMonitorMachineId, oMonitorMachine)

        Return True
    End Function

    Protected Function ApplyMadoPro(ByVal sContextDir As String, ByVal sTermIdRegx As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim oTermIdRegx As New Regex(sTermIdRegx, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

        '指定された全窓処について、待機面にプログラムを保持しているかチェックし、
        'その適用日が運用日以前であれば、適用面に移動する。
        'また、プログラム保持状態を更新した窓処については、
        'sContextDirにMadoProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、MadoProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回更新していない窓処のもの）を削除する。

        'NOTE: これらのファイル名のRRRSSSCCCCUUは端末の機器コードで
        'あるが、これらのファイルが全端末分（シナリオ内で
        '「%T3R%T3S%T4C%T2U」と記述した場合に複製される全行分）
        '作成されるとは限らない。
        'よって、シナリオは、当該ファイルを送信する際、
        'ActiveOneではなく、TryActiveOneを使用する。

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
                    Log.Debug(sMonitorMachineId, "端末 [" & sTermMachineId & "] には、適用待ちのプログラムがありません。")
                ElseIf oTerm.HoldingPrograms(1).ListHashValue IsNot Nothing AndAlso _
                       String.CompareOrdinal(oTerm.HoldingPrograms(1).ApplicableDate, sServiceDate) > 0 Then
                    Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] には、適用待ちのプログラムがありますが、適用日前であるため、適用しません。")
                ElseIf String.CompareOrdinal(oTerm.HoldingPrograms(1).RunnableDate, sServiceDate) > 0 Then
                    Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] には、適用待ちのプログラムがありますが、動作許可日前であるため、適用しません。")
                Else
                    oTerm.HoldingPrograms(0) = oTerm.HoldingPrograms(1)
                    oTerm.HoldingPrograms(0).ApplyDate = d
                    oTerm.HoldingPrograms(1) = Nothing
                    Log.Info(sMonitorMachineId, "端末 [" & sTermMachineId & "] において、適用待ちのプログラムを適用しました。")
                    CreateFileOfMadoProVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
                    UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
                End If
            End If
        Next oTermEntry

        '統括の窓処プログラム保持状態を更新する。
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
                Log.Info(sMonitorMachineId, "シミュレータ本体へ応答 [" & isCompleted.ToString() & "] を返信しました。")
            Else
                Log.Info(sMonitorMachineId, "シミュレータ本体へ応答 [" & isCompleted.ToString() & "][" & sResult & "] を返信しました。")
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
            Log.Info(sMonitorMachineId, "シミュレータ本体へ要求 [" & sVerb & "] を行いました。")
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
        '統括の窓処マスタ保持状態を更新する。
        '配下の窓処が保持している最古の部材よりも古い同一種別の部材を、削除する。

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
                            Log.Debug(sMonitorMachineId, "統括から、種別 [" & sDataKind & "] パターンNo [" & oMas.DataSubKind.ToString() & "] マスタVer [" & oMas.DataVersion.ToString() & "] リストVer [" & oMas.ListVersion.ToString() & "] 統着日時 [" & oMas.ListAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] のマスタ適用リストを削除します。")
                        End If
                        If oMas.DataHashValue IsNot Nothing Then
                            Log.Debug(sMonitorMachineId, "統括から、種別 [" & sDataKind & "] パターンNo [" & oMas.DataSubKind.ToString() & "] マスタVer [" & oMas.DataVersion.ToString() & "] 統着日時 [" & oMas.DataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] のマスタ本体を削除します。")
                        End If
                    End If
                Next oMas
                oMonitorMachine.HoldingMasters(sDataKind) = oNewMasters
            End If
        Next sDataKind
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

    Protected Sub TrimMonitorMachineHoldingPrograms(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine)
        '統括の窓処プログラム保持状態を更新する。
        '配下の窓処が保持している最古の部材よりも古い部材を、削除する。

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
                    Log.Debug(sMonitorMachineId, "統括から、エリアNo [" & oPro.DataSubKind.ToString() & "] 代表Ver [" & oPro.DataVersion.ToString() & "] リストVer [" & oPro.ListVersion.ToString() & "] 統着日時 [" & oPro.ListAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] の窓処プログラム適用リストを削除します。")
                End If
                If oPro.DataHashValue IsNot Nothing Then
                    Log.Debug(sMonitorMachineId, "統括から、エリアNo [" & oPro.DataSubKind.ToString() & "] 代表Ver [" & oPro.DataVersion.ToString() & "] 統着日時 [" & oPro.DataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] の窓処プログラム本体を削除します。")
                End If
            End If
        Next oPro
        oMonitorMachine.HoldingPrograms = oNewPrograms
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

    Protected Sub SweepMonitorMachineHoldingMasters(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine)
        '統括の窓処マスタ保持状態を更新する。
        '配下の窓処が保持しているものと配下の窓処への配信待ちにしているもの以外は、削除する。

        For Each sDataKind As String In ExConstants.MadoMastersSubObjCodes.Keys
            Dim oHoldingMasters As List(Of HoldingMaster) = Nothing
            If oMonitorMachine.HoldingMasters.TryGetValue(sDataKind, oHoldingMasters) = True Then
                Dim oNewMasters As New List(Of HoldingMaster)()
                For Each oMas As HoldingMaster In oHoldingMasters
                    Dim isMasNecessary As Boolean = False
                    For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                        'TODO: DataSubKindは比較しない方がよいかもしれない。本物の統括は、
                        '窓処が保持しているマスタのパターン番号がわからないかもしれない。
                        Dim oTermHoldingMas As HoldingMaster = Nothing
                        If oTerm.HoldingMasters.TryGetValue(sDataKind, oTermHoldingMas) = True AndAlso _
                           oTermHoldingMas.DataSubKind = oMas.DataSubKind AndAlso _
                           oTermHoldingMas.DataVersion = oMas.DataVersion Then

                            'oMasがマスタ本体も含んでいる配信要求の場合は、マスタ本体に関する全項目が
                            '一致しているだけでも、oMasが必要とみなす。
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
                                'TODO: DataSubKindは比較しない方がよいかもしれない。本物の統括は、
                                '窓処が保持しているマスタのパターン番号がわからないかもしれない。
                                If oPendingMas.DataSubKind = oMas.DataSubKind AndAlso _
                                   oPendingMas.DataVersion = oMas.DataVersion Then

                                    'oMasがマスタ本体も含んでいる配信要求の場合は、マスタ本体に関する全項目が
                                    '一致しているだけでも、oMasが必要とみなす。
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
                            Log.Debug(sMonitorMachineId, "統括から、種別 [" & sDataKind & "] パターンNo [" & oMas.DataSubKind.ToString() & "] マスタVer [" & oMas.DataVersion.ToString() & "] リストVer [" & oMas.ListVersion.ToString() & "] 統着日時 [" & oMas.ListAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] のマスタ適用リストを削除します。")
                        End If
                        If oMas.DataHashValue IsNot Nothing Then
                            Log.Debug(sMonitorMachineId, "統括から、種別 [" & sDataKind & "] パターンNo [" & oMas.DataSubKind.ToString() & "] マスタVer [" & oMas.DataVersion.ToString() & "] 統着日時 [" & oMas.DataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] のマスタ本体を削除します。")
                        End If
                    End If
                Next oMas
                oMonitorMachine.HoldingMasters(sDataKind) = oNewMasters
            End If
        Next sDataKind
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

    Protected Sub SweepMonitorMachineHoldingPrograms(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine)
        '統括の窓処プログラム保持状態を更新する。
        '配下の窓処が保持しているものと配下の窓処への配信待ちにしているもの以外は、削除する。

        Dim oNewPrograms As New List(Of HoldingProgram)()
        For Each oPro As HoldingProgram In oMonitorMachine.HoldingPrograms
            Dim isProNecessary As Boolean = False
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                'TODO: DataSubKindは比較しない方がよいかもしれない。
                '配信の仕組みを用いて投入したプログラムのエリア番号は比較するまでもなく
                '一致しているはずであるが、そうでない場合、本物の統括には、
                '窓処が実際に保持しているプログラムのエリア番号がわからないかもしれない。
                For Each oTermHoldingPro As HoldingProgram In oTerm.HoldingPrograms
                    If oTermHoldingPro IsNot Nothing AndAlso _
                       oTermHoldingPro.DataSubKind = oPro.DataSubKind AndAlso _
                       oTermHoldingPro.DataVersion = oPro.DataVersion Then

                        'oProがプログラム本体も含んでいる配信要求の場合は、プログラム本体に関する全項目が
                        '一致しているだけでも、oProが必要とみなす。
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
                    'TODO: DataSubKindは比較しない方がよいかもしれない。本物の統括は、
                    '窓処が保持しているプログラムのパターン番号がわからないかもしれない。
                    If oPendingPro.DataSubKind = oPro.DataSubKind AndAlso _
                       oPendingPro.DataVersion = oPro.DataVersion Then

                        'oProがプログラム本体も含んでいる配信要求の場合は、プログラム本体に関する全項目が
                        '一致しているだけでも、oProが必要とみなす。
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
                    Log.Debug(sMonitorMachineId, "統括から、エリアNo [" & oPro.DataSubKind.ToString() & "] 代表Ver [" & oPro.DataVersion.ToString() & "] リストVer [" & oPro.ListVersion.ToString() & "] 統着日時 [" & oPro.ListAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] の窓処プログラム適用リストを削除します。")
                End If
                If oPro.DataHashValue IsNot Nothing Then
                    Log.Debug(sMonitorMachineId, "統括から、エリアNo [" & oPro.DataSubKind.ToString() & "] 代表Ver [" & oPro.DataVersion.ToString() & "] 統着日時 [" & oPro.DataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff") & "] の窓処プログラム本体を削除します。")
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

        '統括における指定された窓処への窓処プログラム配信保留状況を初期化し、
        '指定された窓処プログラムを当該窓処に投入する（保持させる）。
        'また、その窓処の窓処プログラム保持状況も初期化し、
        '指定された窓処プログラムをそれらの待機面に投入し、
        'sContextDirにMadoProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、MadoProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回の配信と無関係なもの、今回更新
        'していない改札機のもの）を削除する。

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
        Log.Info(sMonitorMachineId, "端末 [" & sTermMachineId & "] の待機面に対して窓処プログラムを直接投入しました。")

        'TODO: 下記のような状況の場合、実物のシステムではどうなるのか？
        'おそらく、再接続時に送信が行われるはずであるから、ここはちゃんと保留にして、
        '異常を解除した際に（手動でもよいので）バージョン情報を送信できた方がよい。
        'If oTermMachine.Tk2Status <> &H2 Then
        '    Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] については、統括DL系状態が接続以外に設定されているため、バージョン情報の送信を保留します。")
        'End If

        'プログラム本体に関する#MadoProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
        'TODO: これは無いと推測しているが、記憶が定かでない。
        '実機が発生させるなら、運管的にも発生させて構わないので、実機に合わせるべき。
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
