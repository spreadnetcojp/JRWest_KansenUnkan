' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/01/14  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports Microsoft.VisualBasic.FileIO
Imports System.Globalization
Imports System.IO
Imports System.Messaging
Imports System.Net.Sockets
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Threading
Imports System.Xml

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

'TODO: 幾つもの種類の電文を周期的に送信できるようにするには、
'ActiveOneと同機能のタブを何枚か用意すればよい。

'TODO: 幾つもの種類のファイルを周期的に能動送信できるようにするには、
'ActiveUllと同機能のタブを何枚か用意すればよい。

'TODO: 受信電文によって、NAKの要否や種類を変えたりするには、
'PassiveGetやPassivePostと同機能のタブを何枚か用意すればよい。

'TODO: 指定されたファイルの種別によって、NAKの要否や種類を変えたりするには、
'PassiveUllやPassiveDllと同機能のタブを何枚か用意すればよい。

Public Class MainForm
    Protected OptionalWriter As LogToOptionalDelegate
    Protected oLogDispStorage As DataTable
    Protected oLogDispBinder As BindingSource
    Protected oLogDispFilterEditDialog As LogDispFilterEditDialog = Nothing

    Protected Enum ClientState
        Registered
        Started
        Aborted
        WaitingForRestart
        QuitRequested
        Discarded
    End Enum

    Protected Class Client
        Public State As ClientState
        Public Index As Integer
        Public Code As EkCode
        Public Addr As String
        Public Telegrapher As MyTelegrapher
        Public ChildSteerSock As Socket
    End Class

    Protected ClientDataTable As DataTable
    Protected MachineDataTable As DataTable

    Protected Friend WithEvents ExtAppTargetQueue As MessageQueue = Nothing

    '動的アセンブリ生成器
    Protected Friend AssemblyManager As DynAssemblyManager

    'NOTE: UiStateのメンバは電文送受信スレッドでも参照可能とする。
    'その際は、SyncLock UiStateした状態でディープコピーを行うこと。
    'また、SyncLock UiStateしている間、ログ出力などメインスレッドを
    '待つことになり得る処理は行ってはならない。
    'NOTE: メインスレッドは、該当するコントロールの状態が変化した際
    'などにおいて、SyncLock UiStateした状態でここに値を設定する。
    'その間、ChildSteerSockへの書き込みやChildSteerSockからの
    '受信待ちなど、電文送受信スレッドを待つことになり得る処理は
    '行ってはならない。
    Public UiState As UiStateClass

    '駅務機器電文書式
    Protected oTelegGene As EkTelegramGene

    'クライアント名出力書式
    Protected Const EkCodeOupFormat As String = "%3R%3S_%4C_%2U"

    '電文送受信スレッドのAbort応答期限
    Protected Const TelegrapherAbortLimitTicks As Integer = 10000  'TODO: 設定から取得する？

    Protected sFtpBasePath As String
    Protected sCapBasePath As String
    Protected sMqPath As String

    'クライアントのリスト
    Protected oClientList As List(Of Client)

    'メッセージボックスを表示中か
    Protected isAlertingTelegrapherAbort As Boolean = False

    'カラー
    Protected ClientDataGridViewBackColor As Color
    Protected ClientDataGridViewForeColor As Color
    Protected ClientDataGridViewSelectionBackColor As Color
    Protected ClientDataGridViewSelectionForeColor As Color

    '文字列展開言語のインタプリタ（ユーザ入力値チェック用）
    Protected oStringExpander As StringExpander

    Protected Shared Function GetMachineDataFromDatabase() As DataTable
        Dim dt As DataTable

        'NOTE: とりあえず運管端末と同じ順序で表示するよう、ORDER BY は指定していないが、
        '何らかの静的規則で順序を決めたいなら、ORDER BY を指定するべきである。
        '現状、たまたま主キーの順で取得できるようであるが、M_MACHINEが複数のエクステントに
        '分割された状況になれば、そうもいかないと思われる。
        '→ Config.MachineDataSortOrderでソート指定可能にした。
        Dim sSQL As String = _
           "SELECT " & [String].Join(", ", Config.MachineDataFieldNames) _
           & " FROM M_MACHINE" _
           & " WHERE (MODEL_CODE = '" & Config.ModelSym & "' OR MODEL_CODE = '" & Config.TermModelSym & "')" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & EkServiceDate.GenString() & "')"

        If Config.MachineDataSortOrder.Length <> 0 Then
            sSQL = sSQL & " ORDER BY " & Config.MachineDataSortOrder
        End If

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        Return dt
    End Function

    Protected Shared Function GetMachineDataFromFile(ByVal sPath As String) As DataTable
        Dim dt As New DataTable()
        For i As Integer = 0 To Config.MachineDataFieldNames.Length - 1
            dt.Columns.Add(Config.MachineDataFieldNames(i), Config.FieldNamesTypes(Config.MachineDataFieldNames(i)))
        Next i

        Using parser As New TextFieldParser(sPath, Encoding.Default)
            parser.TrimWhiteSpace = False
            parser.Delimiters = New String() {","}
            Dim lineCount As Integer = 1
            While Not parser.EndOfData
                Dim columns As String() = parser.ReadFields()
                If columns.Length < 22 Then
                    Throw New FormatException("機器構成の" & lineCount.ToString() & "行目のカラム数が不正です。")
                End If

                Try
                    Dim code As EkCode
                    code.RailSection = Integer.Parse(columns(7))
                    code.StationOrder = Integer.Parse(columns(8))
                    code.Corner = Integer.Parse(columns(10))
                    code.Unit = Integer.Parse(columns(13))
                Catch ex As Exception
                    Throw New FormatException("機器構成の" & lineCount.ToString() & "行目の機器コードが不正です。", ex)
                End Try

                If columns(12) = Config.ModelSym OrElse columns(12) = Config.TermModelSym Then
                    dt.Rows.Add(columns(6), _
                                columns(7), _
                                columns(8), _
                                columns(9), _
                                columns(10), _
                                columns(11), _
                                columns(12), _
                                columns(13), _
                                columns(14), _
                                columns(18), _
                                columns(19), _
                                columns(20), _
                                columns(21))
                End If

                lineCount += 1
            End While
        End Using

        If Config.MachineDataSortOrder.Length <> 0 Then
            Dim rows As DataRow() = dt.Select(Nothing, Config.MachineDataSortOrder)
            Dim dt2 As DataTable = dt.Clone()
            For Each row As DataRow In rows
                dt2.ImportRow(row)
            Next row
            Return dt2
        Else
            Return dt
        End If
    End Function

    'Protected Function FindClient(ByVal oSocket As Socket) As Client
    '    For Each oClient As Client In oClientList
    '        If oClient.ChildSteerSock Is oSocket Then Return oClient
    '    Next oClient
    '    Return Nothing 'NOTE: あり得ないと考えてよい。
    'End Function

    Protected Function FindClient(ByVal code As EkCode) As Client
        For Each oClient As Client In oClientList
            If oClient.Code = code Then Return oClient
        Next oClient
        Return Nothing
    End Function

    Protected Sub RegisterClient(ByVal code As EkCode, ByVal sAddr As String)
        Log.Info("Registering telegrapher [" & code.ToString(EkCodeOupFormat) & "]...")

        Dim oParentSock As Socket = Nothing
        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oParentSock, oChildSock)

        Dim index As Integer = oClientList.Count

        Dim oTelegrapher As MyTelegrapher = New MyTelegrapher( _
          code.ToString(EkCodeOupFormat), _
          oChildSock, _
          oTelegGene, _
          index, _
          code, _
          sFtpBasePath, _
          sCapBasePath, _
          sAddr, _
          MachineDataTable, _
          Me)

        Dim oClient As New Client()
        oClient.State = ClientState.Registered
        oClient.Index = index
        oClient.Code = code
        oClient.Addr = sAddr
        oClient.Telegrapher = oTelegrapher
        oClient.ChildSteerSock = oParentSock
        oClientList.Add(oClient)
    End Sub

    Protected Sub StartTelegrapher(ByVal oClient As Client)
        Debug.Assert(oClient.State = ClientState.Registered)

        Log.Info("Starting telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
        oClient.Telegrapher.Start()
        oClient.State = ClientState.Started
    End Sub

    'NOTE: 電文送受信スレッドを強制終了させた（電文送受信スレッドと通信を行う
    'ソケットが存在しない）Clientに関しても呼び出し可能である。
    'その場合、既にProcOnTelegrapherAbortが呼ばれていても、
    'このメッセージ送信に見合った処理が行えるように、
    '再度ProcOnTelegrapherAbortを呼ぶようになっている。
    Protected Function SendToTelegrapher(ByVal oClient As Client, ByVal oMsg As InternalMessage) As Boolean
        Debug.Assert(oClient.State <> ClientState.Registered)
        Debug.Assert(oClient.State <> ClientState.QuitRequested)
        Debug.Assert(oClient.State <> ClientState.Discarded)

        If oClient.State <> ClientState.Started Then
            If oClient.State <> ClientState.WaitingForRestart Then
                Log.Error("Internal messaging failed. Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] is already marked as broken.")
                Return False
            Else
                Log.Error("Internal messaging failed. Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] is waiting for restart.")
                AbortTelegrapher(oClient)
                Return False
            End If
        End If

        If oMsg.WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("Internal messaging failed. Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
            AbortTelegrapher(oClient)
            Return False
        Else
            Return True
        End If
    End Function

    Protected Sub AbortTelegrapher(ByVal oClient As Client)
        Debug.Assert(oClient.State <> ClientState.Registered)
        Debug.Assert(oClient.State <> ClientState.QuitRequested)
        Debug.Assert(oClient.State <> ClientState.Discarded)

        'NOTE: 「oClient.State = ClientState.Aborted」の場合は、そのまま
        'でもProcOnTelegrapherAbort(oClient)が呼び出されるはずであるため、
        '何も状態を変更せずに、本メソッドを終了する。
        'NOTE: ClientState.WaitingForRestartの場合は、
        'ProcOnTelegrapherAbort(oClient)は既に実行済みである。しかし、
        'それ以降に配信指示が行われ、oClientへMasProDllRequestを
        '送信しようとしてこのメソッドが呼ばれたのであれば、
        '再びProcOnTelegrapherAbort(oClient)を実行して、
        '配信先を別のClientへ切り替えたい。
        'よって、ここで本メソッドを終了させてはならない。
        If oClient.State <> ClientState.Started AndAlso
           oClient.State <> ClientState.WaitingForRestart Then
            Log.Warn("The telegrapher is already marked as broken.")
            Return
        End If

        If oClient.State = ClientState.Started Then
            oClient.ChildSteerSock.Close()
            oClient.ChildSteerSock = Nothing

            If oClient.Telegrapher.ThreadState <> ThreadState.Stopped Then
                oClient.Telegrapher.Abort()

                'NOTE: Abort()の結果、oClient.Telegrapherは例外をキャッチしてログを
                '出力する可能性がある。また、こちらがAbort()から戻ってきた時点で、
                '既に例外処理が開始されていることは最低限保証されていてほしいが、
                'msdnをみた感じだといまいち不明であるため、スレッドが終了状態に
                'ならない限りは、通信相手に関するその他のグローバルな情報もまだ更新
                'する可能性があると考えるべきである。よって、できる限り終了を待って
                'から、新たなTelegrapherをスタートさせる。
                If oClient.Telegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                    Log.Warn("The telegrapher may refuse to abort.")
                End If
            End If
            oClient.Telegrapher = Nothing
        End If

        'NOTE: 再帰呼び出しが発生しないよう、ここで
        'ProcOnTelegrapherAbort(oClient)は行わない。
        oClient.State = ClientState.Aborted
    End Sub

    Protected Sub PrepareToRestartTelegraphers()
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Aborted Then
                ProcOnTelegrapherAbort(oClient)
                oClient.State = ClientState.WaitingForRestart
            End If
        Next oClient
    End Sub

    'NOTE: 強制終了、再起動、強制終了、再起動が短い周期で繰り返される可能性を考慮し、
    'これは、自己診断の周期で呼ぶ方が無難である。
    Protected Sub RestartTelegraphers()
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.WaitingForRestart Then
                Log.Info("Renewing telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")

                Dim oChildSock As Socket = Nothing
                LocalConnectionProvider.CreateSockets(oClient.ChildSteerSock, oChildSock)

                oClient.Telegrapher = New MyTelegrapher( _
                  oClient.Code.ToString(EkCodeOupFormat), _
                  oChildSock, _
                  oTelegGene, _
                  oClient.Index, _
                  oClient.Code, _
                  sFtpBasePath, _
                  sCapBasePath, _
                  oClient.Addr, _
                  MachineDataTable, _
                  Me)

                Log.Info("Restarting telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
                oClient.Telegrapher.Start()
                oClient.State = ClientState.Started
                ProcOnTelegrapherRestart(oClient)
            End If
        Next oClient
    End Sub

    Protected Sub QuitTelegrapher(ByVal oClient As Client)
        Debug.Assert(oClient.State <> ClientState.Registered)
        Debug.Assert(oClient.State <> ClientState.QuitRequested)
        Debug.Assert(oClient.State <> ClientState.Discarded)

        If oClient.State <> ClientState.Started Then
            Log.Warn("The telegrapher is already marked as broken.")
            If oClient.State = ClientState.Aborted Then
                ProcOnTelegrapherAbort(oClient)
            End If
            oClient.State = ClientState.Discarded
            Return
        End If

        Log.Info("Sending quit request to telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
        If QuitRequest.Gen().WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            oClient.ChildSteerSock.Close()
            If oClient.Telegrapher.ThreadState <> ThreadState.Stopped Then
                oClient.Telegrapher.Abort()
                If oClient.Telegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                    Log.Warn("The telegrapher may refuse to abort.")
                End If
            End If
            oClient.State = ClientState.Discarded
        Else
            oClient.State = ClientState.QuitRequested
        End If
    End Sub

    Protected Sub WaitForTelegraphersToQuit()
        Dim oJoinLimitTimer As New TickTimer(Config.TelegrapherPendingLimitTicks)
        oJoinLimitTimer.Start(TickTimer.GetSystemTick())
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.QuitRequested Then
                Dim ticks As Long = oJoinLimitTimer.GetTicksToTimeout(TickTimer.GetSystemTick())
                If ticks < 0 Then ticks = 0

                If oClient.Telegrapher.Join(CInt(ticks)) = False Then
                    Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
                    oClient.ChildSteerSock.Close()
                    oClient.Telegrapher.Abort()
                    If oClient.Telegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                        Log.Warn("The telegrapher may refuse to abort.")
                    End If
                Else
                    Log.Info("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] has quit.")
                    oClient.ChildSteerSock.Close()
                End If
                oClient.State = ClientState.Discarded
            End If
        Next oClient
    End Sub

    Protected Sub UnregisterDiscardedClients()
        Dim i As Integer = 0
        While i < oClientList.Count
            Dim oClient As Client = oClientList(i)
            If oClient.State = ClientState.Discarded Then
                oClientList.RemoveAt(i)
                Log.Info("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] unregistered.")
            Else
                i += 1
            End If
        End While
    End Sub

    Protected Overridable Sub ProcOnTelegrapherAbort(ByVal oClient As Client)
    End Sub

    Protected Overridable Sub ProcOnTelegrapherRestart(ByVal oClient As Client)
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
        LogDispGrid.Columns(1).Width = MyUtility.GetTextWidth("999999_9999_99-Passive", LogDispGrid.Font)
        LogDispGrid.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        SplitContainer1.Panel2MinSize = 568
    End Sub

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)

        Log.SetOptionalWriter(New LogToOptionalDelegate(AddressOf Me.BeginFetchLog))

        Dim sWorkingDir As String = Environment.CurrentDirectory
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

        Me.SuspendLayout() '---------------------------------------------------

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
        UsageToolTip.SetToolTip(ActiveOneApplyFileLabel, Lexis.ActiveSeqApplyFileTipText.Gen())
        UsageToolTip.SetToolTip(ActiveOneApplyFileTextBox, Lexis.ActiveSeqApplyFileTipText.Gen())
        UsageToolTip.SetToolTip(ActiveUllTransferNameLabel, Lexis.ActiveSeqTransferNameTipText.Gen())
        UsageToolTip.SetToolTip(ActiveUllTransferNameTextBox, Lexis.ActiveSeqTransferNameTipText.Gen())
        UsageToolTip.SetToolTip(ActiveUllApplyFileLabel, Lexis.ActiveSeqApplyFileTipText.Gen())
        UsageToolTip.SetToolTip(ActiveUllApplyFileTextBox, Lexis.ActiveSeqApplyFileTipText.Gen())
        UsageToolTip.SetToolTip(ScenarioFileLabel, Lexis.ScenarioFileTipText.Gen())
        UsageToolTip.SetToolTip(ScenarioFileTextBox, Lexis.ScenarioFileTipText.Gen())
        PassiveGetApplyFileColumn.ToolTipText = Lexis.PassiveSeqApplyFileTipText.Gen()
        PassiveUllApplyFileColumn.ToolTipText = Lexis.PassiveSeqApplyFileTipText.Gen()
        UsageToolTip.SetToolTip(ActiveUllObjCodeLabel, Lexis.DataKindTipText.Gen())
        UsageToolTip.SetToolTip(ActiveUllObjCodeTextBox, Lexis.DataKindTipText.Gen())
        PassiveGetObjCodeColumn.ToolTipText = Lexis.DataKindTipText.Gen()
        PassiveUllObjCodeColumn.ToolTipText = Lexis.DataKindTipText.Gen()
        PassivePostObjCodeColumn.ToolTipText = Lexis.DataKindTipText.Gen()
        PassiveDllObjCodeColumn.ToolTipText = Lexis.DataKindTipText.Gen()

        AutomaticComStartCheckBox.Checked = UiState.AutomaticComStart

        ActiveOneApplyFileTextBox.Text = UiState.ActiveOneApplyFilePath
        ActiveOneReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveOneReplyLimitTicks)
        ActiveOneExecIntervalNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveOneExecIntervalTicks)

        ActiveUllObjCodeTextBox.Text = UiState.ActiveUllObjCode
        ActiveUllTransferNameTextBox.Text = UiState.ActiveUllTransferName
        ActiveUllApplyFileTextBox.Text = UiState.ActiveUllApplyFilePath
        ActiveUllTransferLimitNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveUllTransferLimitTicks)
        ActiveUllStartReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveUllStartReplyLimitTicks)
        ActiveUllFinishReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveUllFinishReplyLimitTicks)
        ActiveUllExecIntervalNumericUpDown.Value = Convert.ToDecimal(UiState.ActiveUllExecIntervalTicks)

        For Each oKeyValue As KeyValuePair(Of Byte, String) In UiState.PassiveGetObjCodesApplyFiles
            Dim oRow As New DataGridViewRow()
            oRow.CreateCells(PassiveGetDataGridView)
            With oRow
                .Cells(0).Value = oKeyValue.Key.ToString("X2")
                .Cells(1).Value = oKeyValue.Value
            End With
            PassiveGetDataGridView.Rows.Add(oRow)
        Next
        PassiveGetForceReplyNakCheckBox.Checked = UiState.PassiveGetForceReplyNak
        PassiveGetNakCauseNumberTextBox.Text = UiState.PassiveGetNakCauseNumber.ToString()
        PassiveGetNakCauseTextTextBox.Text = UiState.PassiveGetNakCauseText

        For Each oKeyValue As KeyValuePair(Of Byte, String) In UiState.PassiveUllObjCodesApplyFiles
            Dim oRow As New DataGridViewRow()
            oRow.CreateCells(PassiveUllDataGridView)
            With oRow
                .Cells(0).Value = oKeyValue.Key.ToString("X2")
                .Cells(1).Value = oKeyValue.Value
            End With
            PassiveUllDataGridView.Rows.Add(oRow)
        Next
        PassiveUllForceReplyNakCheckBox.Checked = UiState.PassiveUllForceReplyNak
        PassiveUllNakCauseNumberTextBox.Text = UiState.PassiveUllNakCauseNumber.ToString()
        PassiveUllNakCauseTextTextBox.Text = UiState.PassiveUllNakCauseText
        PassiveUllTransferLimitNumericUpDown.Value = Convert.ToDecimal(UiState.PassiveUllTransferLimitTicks)
        PassiveUllReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.PassiveUllReplyLimitTicks)

        For Each oKeyValue As KeyValuePair(Of Byte, String) In UiState.PassivePostObjCodes
            Dim oRow As New DataGridViewRow()
            oRow.CreateCells(PassivePostDataGridView)
            With oRow
                .Cells(0).Value = oKeyValue.Key.ToString("X2")
            End With
            PassivePostDataGridView.Rows.Add(oRow)
        Next
        PassivePostForceReplyNakCheckBox.Checked = UiState.PassivePostForceReplyNak
        PassivePostNakCauseNumberTextBox.Text = UiState.PassivePostNakCauseNumber.ToString()
        PassivePostNakCauseTextTextBox.Text = UiState.PassivePostNakCauseText

        For Each oKeyValue As KeyValuePair(Of Byte, String) In UiState.PassiveDllObjCodes
            Dim oRow As New DataGridViewRow()
            oRow.CreateCells(PassiveDllDataGridView)
            With oRow
                .Cells(0).Value = oKeyValue.Key.ToString("X2")
            End With
            PassiveDllDataGridView.Rows.Add(oRow)
        Next
        PassiveDllForceReplyNakCheckBox.Checked = UiState.PassiveDllForceReplyNak
        PassiveDllNakCauseNumberTextBox.Text = UiState.PassiveDllNakCauseNumber.ToString()
        PassiveDllNakCauseTextTextBox.Text = UiState.PassiveDllNakCauseText
        PassiveDllTransferLimitNumericUpDown.Value = Convert.ToDecimal(UiState.PassiveDllTransferLimitTicks)
        PassiveDllReplyLimitNumericUpDown.Value = Convert.ToDecimal(UiState.PassiveDllReplyLimitTicks)
        PassiveDllSimulateStoringCheckBox.Checked = UiState.PassiveDllSimulateStoring
        PassiveDllResultantVersionOfSlot1TextBox.Text = UiState.PassiveDllResultantVersionOfSlot1.ToString("D8")
        PassiveDllResultantVersionOfSlot2TextBox.Text = UiState.PassiveDllResultantVersionOfSlot2.ToString("D8")
        PassiveDllResultantFlagOfFullTextBox.Text = UiState.PassiveDllResultantFlagOfFull.ToString("X2")

        ScenarioFileTextBox.Text = UiState.ScenarioFilePath
        ScenarioStartDateTimeCheckBox.Checked = UiState.ScenarioStartTimeSpecified
        If Not UiState.ScenarioStartTimeSpecified Then
            UiState.ScenarioStartTime = DateTime.Now
        End If
        ScenarioStartDateTimePicker.Value = UiState.ScenarioStartTime

        CapSndTelegsCheckBox.Checked = UiState.CapSndTelegs
        CapRcvTelegsCheckBox.Checked = UiState.CapRcvTelegs
        CapSndFilesCheckBox.Checked = UiState.CapSndFiles
        CapRcvFilesCheckBox.Checked = UiState.CapRcvFiles

        Me.ResumeLayout() '----------------------------------------------------

        Dim oMachineDataFileSelDialog As New OpenFileDialog()
        oMachineDataFileSelDialog.Filter = "CSVファイル|*.csv"
        oMachineDataFileSelDialog.Title = "機器構成を選択してください（キャンセルするとサーバのDBから取得します）"
        oMachineDataFileSelDialog.FileName = ""
        oMachineDataFileSelDialog.ShowDialog()
        If oMachineDataFileSelDialog.FileName = "" Then
            '機器構成をデータベースから取得する。
            Try
                MachineDataTable = GetMachineDataFromDatabase()
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.UnwelcomeExceptionCaught, ex.Message)
                Me.Close()
                Return
            End Try
        Else
            '機器構成をファイルから取得する。
            Try
                MachineDataTable = GetMachineDataFromFile(oMachineDataFileSelDialog.FileName)
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.UnwelcomeExceptionCaught, ex.Message)
                Me.Close()
                Return
            End Try
        End If

        ClientDataTable = New DataTable()
        For i As Integer = 0 To Config.ClientDataFieldNames.Length - 1
            ClientDataTable.Columns.Add(Config.ClientDataFieldNames(i), Config.FieldNamesTypes(Config.ClientDataFieldNames(i)))
        Next i
        Dim oSelRows As DataRow() = MachineDataTable.Select("MODEL_CODE = '" & Config.ModelSym & "'")
        For idx As Integer = 0 To oSelRows.Length - 1
            Dim oRow As DataRow = ClientDataTable.NewRow()
            For i As Integer = 0 To Config.ClientDataFieldNames.Length - 1
                Dim sFieldName As String = Config.ClientDataFieldNames(i)
                Select Case sFieldName
                    Case "LINE_STATUS"
                        oRow(sFieldName) = Lexis.LineStatusInitial.Gen()
                    Case "SCENARIO_STATUS"
                        oRow(sFieldName) = Lexis.ScenarioStatusInitial.Gen()
                    Case "IDX"
                        oRow(sFieldName) = idx
                    Case Else
                        oRow(sFieldName) = oSelRows(idx)(sFieldName)
                End Select
            Next i
            ClientDataTable.Rows.Add(oRow)
        Next idx

        'データグリッドビューにクライアントを登録する。
        ClientDataGridView.AutoGenerateColumns = True
        ClientDataGridView.DataSource = ClientDataTable

        Dim visibleFieldKind As Integer = If(SymbolizeCheckBox.Checked, 2, 1)
        For i As Integer = 0 To Config.ClientDataFieldNames.Length - 1
            Dim sFieldName As String = Config.ClientDataFieldNames(i)
            If Array.IndexOf(Config.ClientDataVisibleFieldNames, sFieldName) <> -1 Then
                ClientDataGridView.Columns(i).HeaderText = Config.ClientDataVisibleFieldNamesTitles(sFieldName)
                ClientDataGridView.Columns(i).FillWeight = Config.ClientDataVisibleFieldNamesWeights(sFieldName)
                ClientDataGridView.Columns(i).Visible = ((Config.ClientDataVisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
            Else
                ClientDataGridView.Columns(i).Visible = False
            End If
        Next i

        '後々で参照することになるカラーを退避する。
        ClientDataGridViewBackColor = ClientDataGridView.DefaultCellStyle.BackColor
        ClientDataGridViewForeColor = ClientDataGridView.DefaultCellStyle.ForeColor
        ClientDataGridViewSelectionBackColor = ClientDataGridView.DefaultCellStyle.SelectionBackColor
        ClientDataGridViewSelectionForeColor = ClientDataGridView.DefaultCellStyle.SelectionForeColor

        sFtpBasePath = Path.Combine(sWorkingDir, "TMP")
        sCapBasePath = Path.Combine(sWorkingDir, "CAP")

        If Config.DeleteTmpDirOnAppStartup Then
            'FTPの一時作業用ディレクトリ（兼、外部プロセス連携用ディレクトリ）を空にする。
            If Directory.Exists(sFtpBasePath) Then
                Log.Info("Cleaning up directory [" & sFtpBasePath & "]...")
                Utility.CleanUpDirectory(sFtpBasePath)
            End If
        End If

        '外部アプリからのメッセージを受信するためのキューを作成する。
        Dim sMqPath As String = ".\private$\ExOpmgMultiplexEkimuSim@" & sWorkingDir.Replace("\", "/")
        Try
            If Not MessageQueue.Exists(sMqPath) Then
                ExtAppTargetQueue = MessageQueue.Create(sMqPath)
            Else
                ExtAppTargetQueue = New MessageQueue(sMqPath)
            End If
            ExtAppTargetQueue.MessageReadPropertyFilter.ClearAll()
            ExtAppTargetQueue.MessageReadPropertyFilter.AppSpecific = True
            ExtAppTargetQueue.MessageReadPropertyFilter.Body = True
            ExtAppTargetQueue.MessageReadPropertyFilter.CorrelationId = True
            ExtAppTargetQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(ExtAppFuncMessageBody), GetType(ExtSimFuncMessageBody)})
            ExtAppTargetQueue.Purge()
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            'NOTE: メッセージキューサービスのインストールを必須にするは避けたい
            'ので、このまま（外部常駐プロセス連携機能は使えない状態で）継続する。
            'NOTE: 外部常駐プロセス連携機能が使えないケースには、OSに
            'メッセージキューサービスがインストールされていないケース
            'の他、sMqPathが長くなりすぎるケースなどがある。
            '後者のケースでは、実際にキューが存在していても
            'MessageQueue.Exists()がFalseを返す。また、
            'MessageQueue.Create()は成功し、満足に使えない
            'キューが作られてしまう。
            AlertBox.Show(Lexis.MessageQueueServiceNotAvailable)
            If ExtAppTargetQueue IsNot Nothing Then
                ExtAppTargetQueue.Dispose()
                ExtAppTargetQueue = Nothing
            End If
        End Try

        '動的アセンブリ生成器を作成する。
        AssemblyManager = New DynAssemblyManager(Path.Combine(sWorkingDir, "cache"))

        '文字列展開言語のインタプリタ（ユーザ入力値チェック用）を作成する。
        With Nothing
            Dim code As EkCode
            Dim sPermittedPathInFtp As String = Path.Combine(Config.ModelPathInFtp, code.ToString(EkCodeOupFormat))
            Dim sPermittedPath As String = Utility.CombinePathWithVirtualPath(sFtpBasePath, sPermittedPathInFtp)
            oStringExpander = New StringExpander( _
               Nothing, _
               Nothing, _
               Nothing, _
               Nothing, _
               sPermittedPath)
        End With

        '電文書式オブジェクトを作成する。
        oTelegGene = New EkTelegramGeneForNativeModels(sFtpBasePath)

        '全クライアントの電文送受信スレッドを作成する。
        'NOTE: ExtAppTargetQueueのオブジェクトを参照するので、
        'それの作成後でなければならない。
        oClientList = New List(Of Client)
        For Each oRow As DataRow In ClientDataTable.Rows
            Dim code As EkCode
            code.Model = Config.SelfEkCode.Model
            code.RailSection = Integer.Parse(oRow.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(oRow.Field(Of String)("STATION_ORDER_CODE"))
            code.Corner = oRow.Field(Of Integer)("CORNER_CODE")
            code.Unit = oRow.Field(Of Integer)("UNIT_NO")
            Dim sAddr As String = oRow.Field(Of String)("ADDRESS")
            RegisterClient(code, sAddr)
        Next oRow

        '外部アプリからのメッセージ受信を開始する。
        If ExtAppTargetQueue IsNot Nothing Then
            Try
                ExtAppTargetQueue.SynchronizingObject = Me
                ExtAppTargetQueue.BeginReceive()
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                AlertBox.Show(Lexis.MessageQueueServiceNotAvailable)
            End Try
        End If

        '全クライアントの電文送受信スレッドを開始する。
        For Each oClient As Client In oClientList
            StartTelegrapher(oClient)
        Next oClient

        StatusPollTimer.Start()
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        StatusPollTimer.Stop()

        If oLogDispFilterEditDialog IsNot Nothing Then
            oLogDispFilterEditDialog.Dispose()
            oLogDispFilterEditDialog = Nothing
        End If

        If oClientList IsNot Nothing Then
            '全クライアントの電文送受信スレッドに終了を要求する。
            'NOTE: ここでは、対電文送受信スレッド通信用ソケットや
            '電文送受信スレッドを作成した後、電文送受信スレッドを
            'スタートさせる前に例外が発生した場合や、
            'スタート後の電文送受信スレッドがAbortしている場合など
            'を考慮した実装を行っている。
            For Each oClient As Client In oClientList
                If oClient.ChildSteerSock IsNot Nothing AndAlso _
                   (oClient.State = ClientState.Started OrElse _
                   oClient.State = ClientState.Aborted OrElse _
                   oClient.State = ClientState.WaitingForRestart) Then
                    QuitTelegrapher(oClient)
                End If
            Next oClient

            '終了を要求した電文送受信スレッドの終了を待つ。
            'NOTE: 実際にJoinを行うのは、QuitTelegrapherの対象に
            'なったスレッド（つまり、スタート済みのスレッド）
            'のみとなるため、ThreadStateExceptionが発生する
            '可能性はないものとする。
            WaitForTelegraphersToQuit()

            '不要になったクライアントを登録解除する。
            UnregisterDiscardedClients()
        End If

        If ExtAppTargetQueue IsNot Nothing Then
            'NOTE: 最後に呼び出したBeginReceiveに対応するReceiveCompletedイベントは、
            '下記によるExtAppTargetQueue.readHandleのようなもののCloseによって、
            'それ以降、発生することは無くなる想定である。
            'ExtAppTargetQueue.SynchronizingObjectをNothingにしておけばよさそうにも
            '思えるが、このプロパティはスレッドセーフではなさそうであるため、
            '既にBeginReceiveを行ってしまっているこの時点では変更するべきではない。
            ExtAppTargetQueue.Dispose()
        End If

        If sMqPath IsNot Nothing Then
            Try
                'NOTE: sMqPathが長すぎる場合は、MessageQueue.Exists()は
                '実際に当該パスにキューが存在していても、Falseを返却
                'するようである。一方、sMqPathが長すぎる場合も、
                'メッセージキューサービスがインストールされている限りは、
                'MessageQueue.Createが成功してしまう。
                'よって、sMqPathが長すぎる場合に、起動時に作成してしまった
                'キューの削除を試みるには、ここでのMessageQueue.Existsによる
                '判断を省略するしかない。なお、削除を試みたところで、
                'sMqPathが長すぎることを理由にMessageQueue.Delete()から
                'MessageQueueExceptionがスローされ、削除は失敗するが、
                'そのMessageQueueErrorCodeプロパティによって、キューが残って
                'しまうことが分かるため、その警告を出すことができる。
                'If MessageQueue.Exists(sMqPath) Then
                '    MessageQueue.Delete(sMqPath)
                'End If

                MessageQueue.Delete(sMqPath)

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

            UiState.ActiveOneApplyFilePath = ActiveOneApplyFileTextBox.Text
            UiState.ActiveOneReplyLimitTicks = Decimal.ToInt32(ActiveOneReplyLimitNumericUpDown.Value)
            UiState.ActiveOneExecIntervalTicks = Decimal.ToInt32(ActiveOneExecIntervalNumericUpDown.Value)

            UiState.ActiveUllObjCode = ActiveUllObjCodeTextBox.Text
            UiState.ActiveUllTransferName = ActiveUllTransferNameTextBox.Text
            UiState.ActiveUllApplyFilePath = ActiveUllApplyFileTextBox.Text
            UiState.ActiveUllTransferLimitTicks = Decimal.ToInt32(ActiveUllTransferLimitNumericUpDown.Value)
            UiState.ActiveUllStartReplyLimitTicks = Decimal.ToInt32(ActiveUllStartReplyLimitNumericUpDown.Value)
            UiState.ActiveUllFinishReplyLimitTicks = Decimal.ToInt32(ActiveUllFinishReplyLimitNumericUpDown.Value)
            UiState.ActiveUllExecIntervalTicks = Decimal.ToInt32(ActiveUllExecIntervalNumericUpDown.Value)

            UiState.ScenarioFilePath = ScenarioFileTextBox.Text
            UiState.ScenarioStartTimeSpecified = ScenarioStartDateTimeCheckBox.Checked
            UiState.ScenarioStartTime = ScenarioStartDateTimePicker.Value

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
        'NOTE: UiState.LogDispFilterHistoryは、参照先のオブジェクトだけでなく、
        '参照型変数自体もこのスレッドからしかアクセスしない（参照先オブジェクト
        'をファイルに保存するためだけにUiState内に用意しているに過ぎない）
        'ので、事前にSyncLock UiStateブロックの中で参照を取得しておくような
        'コーディングは不要である。
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

    Private Sub SymbolizeCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SymbolizeCheckBox.CheckedChanged
        Dim visibleFieldKind As Integer = If(SymbolizeCheckBox.Checked, 2, 1)

        ClientDataGridView.SuspendLayout()
        For i As Integer = 0 To Config.ClientDataFieldNames.Length - 1
            Dim sFieldName As String = Config.ClientDataFieldNames(i)
            If Array.IndexOf(Config.ClientDataVisibleFieldNames, sFieldName) <> -1 Then
                ClientDataGridView.Columns(i).Visible = ((Config.ClientDataVisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
            Else
                ClientDataGridView.Columns(i).Visible = False
            End If
        Next i
        ClientDataGridView.ResumeLayout()
    End Sub

    Private Sub SeqTabControl_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SeqTabControl.SelectedIndexChanged

        Select Case SeqTabControl.TabPages(SeqTabControl.SelectedIndex).Name
            Case "ConnectionTabPage", "ActiveOneTabPage", "ActiveUllTabPage", "ScenarioTabPage"
                ClientDataGridView.DefaultCellStyle.BackColor = ClientDataGridViewBackColor
                ClientDataGridView.DefaultCellStyle.ForeColor = ClientDataGridViewForeColor
                ClientDataGridView.DefaultCellStyle.SelectionBackColor = ClientDataGridViewSelectionBackColor
                ClientDataGridView.DefaultCellStyle.SelectionForeColor = ClientDataGridViewSelectionForeColor
            Case Else
                ClientDataGridView.DefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke
                ClientDataGridView.DefaultCellStyle.ForeColor = System.Drawing.Color.DimGray
                ClientDataGridView.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Gray
                ClientDataGridView.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White
        End Select
    End Sub

    Private Sub ConButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConButton.Click
        Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
        For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
            If gridRow.Selected Then
                Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                Dim oClient As Client = oClientList(DirectCast(drv.Row(idxColumn), Integer))
                SendToTelegrapher(oClient, ConnectRequest.Gen())
            End If
        Next gridRow
    End Sub

    Private Sub DisconButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DisconButton.Click
        Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
        For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
            If gridRow.Selected Then
                Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                Dim oClient As Client = oClientList(DirectCast(drv.Row(idxColumn), Integer))
                SendToTelegrapher(oClient, DisconnectRequest.Gen())
            End If
        Next gridRow
    End Sub

    Private Sub ScenarioStartButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScenarioStartButton.Click
        'ローカルファイルパスの書式をチェックしておく。
        Try
            Dim code As EkCode
            Dim sFilePath As String = ScenarioFileTextBox.Text
            sFilePath = sFilePath.Replace("%%", vbLf)
            sFilePath = MyUtility.ReplaceMachineIndex(sFilePath, 9999)
            sFilePath = code.ToString(sFilePath).Replace(ControlChars.Lf, "%"c)
            If sFilePath.Contains("$ContextDir<") OrElse _
               sFilePath.Contains("$ContextNum<") OrElse _
               sFilePath.Contains("$SetRef<") OrElse _
               sFilePath.Contains("$SetVal<") OrElse _
               sFilePath.Contains("$Val<") OrElse _
               sFilePath.Contains("$ExecDynFunc<") OrElse _
               sFilePath.Contains("$ExecCmdFunc<") OrElse _
               sFilePath.Contains("$ExecAppFunc<") Then Throw New FormatException()
            sFilePath = oStringExpander.Expand(sFilePath, Nothing, 0)
            Path.GetDirectoryName(sFilePath)
        Catch ex As Exception
            AlertBox.Show(Lexis.FilePathIsInvalid)
            Return
        End Try

        Dim oExt As New ScenarioStartRequestExtendPart()
        oExt.ScenarioFilePath = ScenarioFileTextBox.Text
        oExt.StartTimeSpecified = ScenarioStartDateTimeCheckBox.Checked
        oExt.StartTime = ScenarioStartDateTimePicker.Value

        Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
        For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
            If gridRow.Selected Then
                Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                Dim oClient As Client = oClientList(DirectCast(drv.Row(idxColumn), Integer))
                SendToTelegrapher(oClient, ScenarioStartRequest.Gen(oExt))
            End If
        Next gridRow
    End Sub

    Private Sub ScenarioStopButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScenarioStopButton.Click
        Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
        For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
            If gridRow.Selected Then
                Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                Dim oClient As Client = oClientList(DirectCast(drv.Row(idxColumn), Integer))
                SendToTelegrapher(oClient, ScenarioStopRequest.Gen())
            End If
        Next gridRow
    End Sub

    Private Sub StatusPollTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusPollTimer.Tick
        Dim oRows As DataRowCollection = ClientDataTable.Rows
        Dim idt As Integer = 0
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Started Then
                Dim lnSts As LineStatus = oClient.Telegrapher.LineStatus
                If lnSts = LineStatus.Initial Then
                    If Not oRows(idt).Field(Of String)("LINE_STATUS").Equals(Lexis.LineStatusInitial.Gen()) Then
                        oRows(idt).Item("LINE_STATUS") = Lexis.LineStatusInitial.Gen()
                    End If
                ElseIf lnSts = LineStatus.ConnectWaiting Then
                    If Not oRows(idt).Field(Of String)("LINE_STATUS").Equals(Lexis.LineStatusConnectWaiting.Gen()) Then
                        oRows(idt).Item("LINE_STATUS") = Lexis.LineStatusConnectWaiting.Gen()
                    End If
                ElseIf lnSts = LineStatus.ConnectFailed Then
                    If Not oRows(idt).Field(Of String)("LINE_STATUS").Equals(Lexis.LineStatusConnectFailed.Gen()) Then
                        oRows(idt).Item("LINE_STATUS") = Lexis.LineStatusConnectFailed.Gen()
                    End If
                ElseIf lnSts = LineStatus.Connected Then
                    If Not oRows(idt).Field(Of String)("LINE_STATUS").Equals(Lexis.LineStatusConnected.Gen()) Then
                        oRows(idt).Item("LINE_STATUS") = Lexis.LineStatusConnected.Gen()
                    End If
                ElseIf lnSts = LineStatus.ComStartWaiting Then
                    If Not oRows(idt).Field(Of String)("LINE_STATUS").Equals(Lexis.LineStatusComStartWaiting.Gen()) Then
                        oRows(idt).Item("LINE_STATUS") = Lexis.LineStatusComStartWaiting.Gen()
                    End If
                ElseIf lnSts = LineStatus.Steady Then
                    If Not oRows(idt).Field(Of String)("LINE_STATUS").Equals(Lexis.LineStatusSteady.Gen()) Then
                        oRows(idt).Item("LINE_STATUS") = Lexis.LineStatusSteady.Gen()
                    End If
                ElseIf lnSts = LineStatus.Disconnected Then
                    If Not oRows(idt).Field(Of String)("LINE_STATUS").Equals(Lexis.LineStatusDisconnected.Gen()) Then
                        oRows(idt).Item("LINE_STATUS") = Lexis.LineStatusDisconnected.Gen()
                    End If
                End If

                Dim snSts As ScenarioStatus = oClient.Telegrapher.ScenarioStatus
                If snSts = ScenarioStatus.Initial Then
                    If Not oRows(idt).Field(Of String)("SCENARIO_STATUS").Equals(Lexis.ScenarioStatusInitial.Gen()) Then
                        oRows(idt).Item("SCENARIO_STATUS") = Lexis.ScenarioStatusInitial.Gen()
                    End If
                ElseIf snSts = ScenarioStatus.Loaded Then
                    If Not oRows(idt).Field(Of String)("SCENARIO_STATUS").Equals(Lexis.ScenarioStatusLoaded.Gen()) Then
                        oRows(idt).Item("SCENARIO_STATUS") = Lexis.ScenarioStatusLoaded.Gen()
                    End If
                ElseIf snSts = ScenarioStatus.Running Then
                    If Not oRows(idt).Field(Of String)("SCENARIO_STATUS").Equals(Lexis.ScenarioStatusRunning.Gen()) Then
                        oRows(idt).Item("SCENARIO_STATUS") = Lexis.ScenarioStatusRunning.Gen()
                    End If
                ElseIf snSts = ScenarioStatus.Aborted Then
                    If Not oRows(idt).Field(Of String)("SCENARIO_STATUS").Equals(Lexis.ScenarioStatusAborted.Gen()) Then
                        oRows(idt).Item("SCENARIO_STATUS") = Lexis.ScenarioStatusAborted.Gen()
                    End If
                ElseIf snSts = ScenarioStatus.Finished Then
                    If Not oRows(idt).Field(Of String)("SCENARIO_STATUS").Equals(Lexis.ScenarioStatusFinished.Gen()) Then
                        oRows(idt).Item("SCENARIO_STATUS") = Lexis.ScenarioStatusFinished.Gen()
                    End If
                ElseIf snSts = ScenarioStatus.Stopped Then
                    If Not oRows(idt).Field(Of String)("SCENARIO_STATUS").Equals(Lexis.ScenarioStatusStopped.Gen()) Then
                        oRows(idt).Item("SCENARIO_STATUS") = Lexis.ScenarioStatusStopped.Gen()
                    End If
                End If
            End If

            If Not isAlertingTelegrapherAbort Then
                If oClient.State = ClientState.Started Then
                    If oClient.Telegrapher.ThreadState = ThreadState.Stopped Then
                        '予期せぬ例外などで異常終了している場合である。
                        Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] has stopped.")
                        AbortTelegrapher(oClient)
                    End If
                End If
                If oClient.State = ClientState.Aborted Then
                    oRows(idt).Item("LINE_STATUS") = Lexis.LineStatusInitial.Gen()
                    oRows(idt).Item("SCENARIO_STATUS") = Lexis.ScenarioStatusInitial.Gen()
                    isAlertingTelegrapherAbort = True
                    AlertBox.Show(Lexis.TheTelegrapherAborted, oClient.Code.ToString(EkCodeOupFormat))
                    isAlertingTelegrapherAbort = False
                End If
            End If

            idt += 1
        Next oClient

        If Not isAlertingTelegrapherAbort Then
            PrepareToRestartTelegraphers()
            RestartTelegraphers()
        End If
    End Sub

    Private Sub ComSartButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComSartButton.Click
        Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
        For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
            If gridRow.Selected Then
                Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                SendToTelegrapher(oClientList(DirectCast(drv.Row(idxColumn), Integer)), ComStartExecRequest.Gen())
            End If
        Next gridRow
    End Sub

    Private Sub TimeDataGetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimeDataGetButton.Click
        Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
        For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
            If gridRow.Selected Then
                Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                SendToTelegrapher(oClientList(DirectCast(drv.Row(idxColumn), Integer)), TimeDataGetExecRequest.Gen())
            End If
        Next gridRow
    End Sub

    Private Sub AutomaticComStartCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutomaticComStartCheckBox.CheckedChanged
        SyncLock UiState
            UiState.AutomaticComStart = AutomaticComStartCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub CapSndTelegsCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CapSndTelegsCheckBox.CheckedChanged
        SyncLock UiState
            UiState.CapSndTelegs = CapSndTelegsCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub CapRcvTelegsCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CapRcvTelegsCheckBox.CheckedChanged
        SyncLock UiState
            UiState.CapRcvTelegs = CapRcvTelegsCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub CapSndFilesCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CapSndFilesCheckBox.CheckedChanged
        SyncLock UiState
            UiState.CapSndFiles = CapSndFilesCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub CapRcvFilesCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CapRcvFilesCheckBox.CheckedChanged
        SyncLock UiState
            UiState.CapRcvFiles = CapRcvFilesCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub ActiveOneApplyFileSelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActiveOneApplyFileSelButton.Click
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return
        ActiveOneApplyFileTextBox.Text = FileSelDialog.FileName
    End Sub

    Private Sub ActiveOneExecButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActiveOneExecButton.Click
        If Not ActiveOneExecTimer.Enabled Then
            'ローカルファイルパスの書式をチェックしておく。
            Try
                Dim code As EkCode
                Dim sFilePath As String = ActiveOneApplyFileTextBox.Text
                sFilePath = sFilePath.Replace("%%", vbLf)
                sFilePath = MyUtility.ReplaceMachineIndex(sFilePath.Replace("%T", "%"), 9999)
                sFilePath = code.ToString(sFilePath).Replace(ControlChars.Lf, "%"c)
                If sFilePath.Contains("$ContextDir<") OrElse _
                   sFilePath.Contains("$ContextNum<") OrElse _
                   sFilePath.Contains("$SetRef<") OrElse _
                   sFilePath.Contains("$SetVal<") OrElse _
                   sFilePath.Contains("$Val<") OrElse _
                   sFilePath.Contains("$ExecDynFunc<") OrElse _
                   sFilePath.Contains("$ExecCmdFunc<") OrElse _
                   sFilePath.Contains("$ExecAppFunc<") Then Throw New FormatException()
                sFilePath = oStringExpander.Expand(sFilePath, Nothing, 0)
                Path.GetDirectoryName(sFilePath)
            Catch ex As Exception
                AlertBox.Show(Lexis.FilePathIsInvalid)
                Return
            End Try

            Dim rate As Integer = Decimal.ToInt32(ActiveOneExecIntervalNumericUpDown.Value)
            If rate = 0 Then
                'NOTE: シミュレータでは好きなタイミングで好きなだけ手動リトライが可能であるため、
                '電文送信の（自動）リトライ回数は０回に設定する。
                Dim oExt As New ActiveOneExecRequestExtendPart()
                oExt.ApplyFilePath = ActiveOneApplyFileTextBox.Text
                oExt.ReplyLimitTicks = Decimal.ToInt32(ActiveOneReplyLimitNumericUpDown.Value)
                oExt.RetryIntervalTicks = 60000
                oExt.MaxRetryCountToForget = 0
                oExt.MaxRetryCountToCare = 0
                oExt.DeleteApplyFileIfCompleted = False
                oExt.ApplyFileMustExists = True

                Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
                For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
                    If gridRow.Selected Then
                        Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                        Dim oClient As Client = oClientList(DirectCast(drv.Row(idxColumn), Integer))
                        SendToTelegrapher(oClient, ActiveOneExecRequest.Gen(oExt))
                    End If
                Next gridRow
            Else
                ActiveOneExecTimer.Interval = rate
                ActiveOneExecTimer.Enabled = True
                ActiveOneExecButton.Text = "中止"
                ActiveOneExecButton.BackColor = Color.Green
                ActiveOneExecIntervalNumericUpDown.Enabled = False
                ActiveOneApplyFileTextBox.Enabled = False
                ActiveOneReplyLimitNumericUpDown.Enabled = False
            End If
        Else
            ActiveOneExecTimer.Enabled = False
            ActiveOneExecButton.Text = "実行"
            ActiveOneExecButton.ResetBackColor()
            ActiveOneExecIntervalNumericUpDown.Enabled = True
            ActiveOneApplyFileTextBox.Enabled = True
            ActiveOneReplyLimitNumericUpDown.Enabled = True
        End If
    End Sub

    Private Sub ActiveOneExecTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActiveOneExecTimer.Tick
        Dim oExt As New ActiveOneExecRequestExtendPart()
        oExt.ApplyFilePath = ActiveOneApplyFileTextBox.Text
        oExt.ReplyLimitTicks = Decimal.ToInt32(ActiveOneReplyLimitNumericUpDown.Value)
        oExt.RetryIntervalTicks = 60000
        oExt.MaxRetryCountToForget = 0
        oExt.MaxRetryCountToCare = 0
        oExt.DeleteApplyFileIfCompleted = False
        oExt.ApplyFileMustExists = True

        Log.Info("Sending ActiveOneExecRequest to the telegraphers...")
        Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
        For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
            If gridRow.Selected Then
                Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                Dim oClient As Client = oClientList(DirectCast(drv.Row(idxColumn), Integer))
                SendToTelegrapher(oClient, ActiveOneExecRequest.Gen(oExt))
            End If
        Next gridRow
    End Sub

    Private Sub ActiveUllTransferNameSelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActiveUllTransferNameSelButton.Click
        Dim oForm As New ActiveUllTransferNameForm()
        Dim res As DialogResult = oForm.ShowDialog()
        Dim sSelValue As String = oForm.SelectedValue
        oForm.Dispose()
        If res = DialogResult.OK Then
            ActiveUllTransferNameTextBox.Text = sSelValue
        End If
    End Sub

    Private Sub ActiveUllApplyFileSelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActiveUllApplyFileSelButton.Click
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return
        ActiveUllApplyFileTextBox.Text = FileSelDialog.FileName
    End Sub

    Private Sub ActiveUllExecButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActiveUllExecButton.Click
        If Not ActiveUllExecTimer.Enabled Then
            'データ種別の書式をチェックし、整数に変換しておく。
            Dim objCode As Integer
            If Integer.TryParse(ActiveUllObjCodeTextBox.Text, NumberStyles.HexNumber, Nothing, objCode) = False Then
                AlertBox.Show(Lexis.TheInputValueIsUnsuitableForObjCode)
                Return
            End If

            '転送先ファイル名の書式をチェックしておく。
            Try
                Dim code As EkCode
                Dim sFilePath As String = ActiveUllTransferNameTextBox.Text
                sFilePath = sFilePath.Replace("%%", vbLf)
                sFilePath = MyUtility.ReplaceMachineIndex(sFilePath.Replace("%T", "%"), 9999)
                sFilePath = code.ToString(sFilePath).Replace(ControlChars.Lf, "%"c)
                If sFilePath.Contains("$ContextDir<") OrElse _
                   sFilePath.Contains("$ContextNum<") OrElse _
                   sFilePath.Contains("$SetRef<") OrElse _
                   sFilePath.Contains("$SetVal<") OrElse _
                   sFilePath.Contains("$Val<") OrElse _
                   sFilePath.Contains("$ExecDynFunc<") OrElse _
                   sFilePath.Contains("$ExecCmdFunc<") OrElse _
                   sFilePath.Contains("$ExecAppFunc<") Then Throw New FormatException()
                sFilePath = oStringExpander.Expand(sFilePath, Nothing, 0)
                Path.GetDirectoryName(sFilePath)
            Catch ex As Exception
                AlertBox.Show(Lexis.TransferNameIsInvalid)
                Return
            End Try

            'ローカルファイルパスの書式をチェックしておく。
            Try
                Dim code As EkCode
                Dim sFilePath As String = ActiveUllApplyFileTextBox.Text
                sFilePath = sFilePath.Replace("%%", vbLf)
                sFilePath = MyUtility.ReplaceMachineIndex(sFilePath.Replace("%T", "%"), 9999)
                sFilePath = code.ToString(sFilePath).Replace(ControlChars.Lf, "%"c)
                If sFilePath.Contains("$ContextDir<") OrElse _
                   sFilePath.Contains("$ContextNum<") OrElse _
                   sFilePath.Contains("$SetRef<") OrElse _
                   sFilePath.Contains("$SetVal<") OrElse _
                   sFilePath.Contains("$Val<") OrElse _
                   sFilePath.Contains("$ExecDynFunc<") OrElse _
                   sFilePath.Contains("$ExecCmdFunc<") OrElse _
                   sFilePath.Contains("$ExecAppFunc<") Then Throw New FormatException()
                sFilePath = oStringExpander.Expand(sFilePath, Nothing, 0)
                Path.GetDirectoryName(sFilePath)
            Catch ex As Exception
                AlertBox.Show(Lexis.FilePathIsInvalid)
                Return
            End Try

            Dim rate As Integer = Decimal.ToInt32(ActiveUllExecIntervalNumericUpDown.Value)
            If rate = 0 Then
                'NOTE: シミュレータでは好きなタイミングで好きなだけ手動リトライが可能であるため、
                '電文送信の（自動）リトライ回数は０回に設定する。
                Dim oExt As New ActiveUllExecRequestExtendPart()
                oExt.ObjCode = objCode
                oExt.TransferFileName = ActiveUllTransferNameTextBox.Text
                oExt.ApplyFilePath = ActiveUllApplyFileTextBox.Text
                oExt.ApplyFileHashValue = ""
                oExt.TransferLimitTicks = Decimal.ToInt32(ActiveUllTransferLimitNumericUpDown.Value)
                oExt.ReplyLimitTicksOnStart = Decimal.ToInt32(ActiveUllStartReplyLimitNumericUpDown.Value)
                oExt.ReplyLimitTicksOnFinish = Decimal.ToInt32(ActiveUllFinishReplyLimitNumericUpDown.Value)
                oExt.RetryIntervalTicks = 60000
                oExt.MaxRetryCountToForget = 0
                oExt.MaxRetryCountToCare = 0
                oExt.DeleteApplyFileIfCompleted = False
                oExt.ApplyFileMustExists = True

                Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
                For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
                    If gridRow.Selected Then
                        Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                        Dim oClient As Client = oClientList(DirectCast(drv.Row(idxColumn), Integer))
                        SendToTelegrapher(oClient, ActiveUllExecRequest.Gen(oExt))
                    End If
                Next gridRow
            Else
                ActiveUllExecTimer.Interval = rate
                ActiveUllExecTimer.Enabled = True
                ActiveUllExecButton.Text = "中止"
                ActiveUllExecButton.BackColor = Color.Green
                ActiveUllExecIntervalNumericUpDown.Enabled = False
                ActiveUllObjCodeTextBox.Enabled = False
                ActiveUllTransferNameTextBox.Enabled = False
                ActiveUllApplyFileTextBox.Enabled = False
                ActiveUllTransferLimitNumericUpDown.Enabled = False
                ActiveUllStartReplyLimitNumericUpDown.Enabled = False
                ActiveUllFinishReplyLimitNumericUpDown.Enabled = False
            End If
        Else
            ActiveUllExecTimer.Enabled = False
            ActiveUllExecButton.Text = "実行"
            ActiveUllExecButton.ResetBackColor()
            ActiveUllExecIntervalNumericUpDown.Enabled = True
            ActiveUllObjCodeTextBox.Enabled = True
            ActiveUllTransferNameTextBox.Enabled = True
            ActiveUllApplyFileTextBox.Enabled = True
            ActiveUllTransferLimitNumericUpDown.Enabled = True
            ActiveUllStartReplyLimitNumericUpDown.Enabled = True
            ActiveUllFinishReplyLimitNumericUpDown.Enabled = True
        End If
    End Sub

    Private Sub ActiveUllExecTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActiveUllExecTimer.Tick
        Dim oExt As New ActiveUllExecRequestExtendPart()
        oExt.ObjCode = Integer.Parse(ActiveUllObjCodeTextBox.Text, NumberStyles.HexNumber)
        oExt.TransferFileName = ActiveUllTransferNameTextBox.Text
        oExt.ApplyFilePath = ActiveUllApplyFileTextBox.Text
        oExt.ApplyFileHashValue = ""
        oExt.TransferLimitTicks = Decimal.ToInt32(ActiveUllTransferLimitNumericUpDown.Value)
        oExt.ReplyLimitTicksOnStart = Decimal.ToInt32(ActiveUllStartReplyLimitNumericUpDown.Value)
        oExt.ReplyLimitTicksOnFinish = Decimal.ToInt32(ActiveUllFinishReplyLimitNumericUpDown.Value)
        oExt.RetryIntervalTicks = 60000
        oExt.MaxRetryCountToForget = 0
        oExt.MaxRetryCountToCare = 0
        oExt.DeleteApplyFileIfCompleted = False
        oExt.ApplyFileMustExists = True

        Log.Info("Sending ActiveUllExecRequest to the telegraphers...")
        Dim idxColumn As Integer = Array.IndexOf(Config.ClientDataFieldNames, "IDX")
        For Each gridRow As DataGridViewRow In ClientDataGridView.Rows
            If gridRow.Selected Then
                Dim drv As DataRowView = DirectCast(gridRow.DataBoundItem, DataRowView)
                Dim oClient As Client = oClientList(DirectCast(drv.Row(idxColumn), Integer))
                SendToTelegrapher(oClient, ActiveUllExecRequest.Gen(oExt))
            End If
        Next gridRow
    End Sub

    'NOTE: lastEditRowは編集中の行番号。編集中でない場合は-1とする。
    'NOTE: sKeyAtBeginEditRowInDataGridViewは編集中の行の、編集開始時のキー値。
    'lastEditRowが-1以外の場合のみ有意である。新規の行を編集中はNothingとする。
    Private lastEditRow As Integer = -1
    Private sKeyAtBeginEditRowInDataGridView As String

    Private Sub PassiveGetDataGridView_CellMouseClick(ByVal sender As System.Object, ByVal e As DataGridViewCellMouseEventArgs) Handles PassiveGetDataGridView.CellMouseClick
        '左クリックの場合は、このメソッドでは処理しない。
        If e.Button <> MouseButtons.Right Then Return

        '列ヘッダを右クリックした場合は、このメソッドでは処理しない。
        If e.RowIndex = -1 Then Return

        '右クリックした場所に選択を移す。
        If e.ColumnIndex = -1 Then
            'NOTE: 行ヘッダが右クリックされた場合である。
            '当該行の１列目セルを選択しているが、これは、行ヘッダを選択しても
            '直前まで選択されていた行の妥当性チェックが実行されないことおよび、
            '直前まで選択されていた行の選択が解除されないことに対処するためである。
            PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveGetDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '右クリックした行とは別の編集中の行が存在する場合は、メニューは出さない。
        'ただし、編集中の行が真の編集中ではない場合は、メニューを出す。
        If lastEditRow <> -1 AndAlso lastEditRow <> e.RowIndex Then
            If Not (PassiveGetDataGridView.Rows(lastEditRow).IsNewRow AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveGetDataGridView.Rows(lastEditRow).Cells(0).Value)) AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveGetDataGridView.Rows(lastEditRow).Cells(1).Value))) Then
                Return
            End If
        End If

        If e.ColumnIndex = -1 Then
            PassiveGetDataGridView.Rows(e.RowIndex).Selected = True
            Application.DoEvents()
            PassiveGetRowHeaderMenu.Show(PassiveGetDataGridView, PassiveGetDataGridView.GetRowDisplayRectangle(e.RowIndex, False).Location + New Size(e.Location))
        ElseIf e.ColumnIndex = 1 Then
            PassiveGetApplyFileMenu.Show(PassiveGetDataGridView, PassiveGetDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False).Location + New Size(e.Location))
        End If
    End Sub

    Private Sub PassiveGetDelMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveGetDelMenuItem.Click
        RemovePassiveGetData()
    End Sub

    Private Sub PassiveGetSelMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveGetSelMenuItem.Click
        SelectPassiveGetDataApplyFile()
    End Sub

    Private Sub PassiveGetDataGridView_KeyDown(ByVal sender As System.Object, ByVal e As KeyEventArgs) Handles PassiveGetDataGridView.KeyDown
        Select Case e.KeyData
            Case Keys.Delete
                If PassiveGetDataGridView.SelectedRows.Count = 1 Then
                    RemovePassiveGetData()
                    e.Handled = True
                End If
            Case Keys.Apps
                If PassiveGetDataGridView.SelectedRows.Count = 0 AndAlso _
                   PassiveGetDataGridView.SelectedCells.Count = 1 AndAlso _
                   PassiveGetDataGridView.SelectedCells(0).ColumnIndex = 1 Then
                    Dim r As Rectangle = PassiveGetDataGridView.GetCellDisplayRectangle(1, PassiveGetDataGridView.SelectedCells(0).RowIndex, False)
                    PassiveGetApplyFileMenu.Show(PassiveGetDataGridView, r.Location + New Size((r.Size.Width - PassiveGetApplyFileMenu.Size.Width) \ 2, r.Size.Height))
                    e.Handled = True
                End If
        End Select
    End Sub

    Private Sub RemovePassiveGetData()
        Dim selectedRow As DataGridViewRow = PassiveGetDataGridView.SelectedRows(0)

        If selectedRow.IsNewRow Then
            selectedRow.Cells(0).Value = Nothing
            selectedRow.Cells(1).Value = Nothing
            selectedRow.ErrorText = Nothing
            Return
        End If

        If lastEditRow <> selectedRow.Index Then
            SyncLock UiState
                UiState.PassiveGetObjCodesApplyFiles.Remove(Byte.Parse(CStr(selectedRow.Cells(0).Value), NumberStyles.HexNumber))
            End SyncLock
        ElseIf sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView IsNot Nothing)
            SyncLock UiState
                UiState.PassiveGetObjCodesApplyFiles.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
            End SyncLock
        Else
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView Is Nothing)
            'NOTE: おそらく、selectedRow.IsNewRowに該当するケースであるため、
            'ここまで到達しないと思われる。たとえ到達したとしても、
            '新規の行を編集中にその行の削除を実施した場合である故、
            'Dictionaryには内容を登録していないので、Dictionaryからの削除は無用である。
        End If

        If lastEditRow = selectedRow.Index Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどで、余計なことが
            '行われないように、この時点で、編集中ではなかったことにしておく。
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどを処理する際、
            'lastEditRowが位置として参照されないことを前提に、
            '邪道ではあるが、この時点で補正を行っておく。
            'NOTE: デクリメント前の時点でlastEditRowは1以上であるため、
            'デクリメントの結果が-1やそれ以下になることはない。
            'NOTE: Rows.RemoveAt(...)によるRowValidatedイベントを処理する際、
            'lastEditRowは-1に変更される。さらに、Rows.RemoveAt(...)の結果として
            '既存の行が全て無くなれば、DefaultValuesNeededイベントが発生し、
            'lastEditRowは新規行の位置（おそらく0）に変更される。つまり、
            'この補正は、事実上不要である可能性が高い。
            lastEditRow -= 1
        End If

        PassiveGetDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub SelectPassiveGetDataApplyFile()
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return

        Dim selectedRow As DataGridViewRow = PassiveGetDataGridView.Rows(PassiveGetDataGridView.SelectedCells(0).RowIndex)

        'NOTE: 編集中の行や新規の行に対して、ファイル名の選択を実施した場合、
        'その場でのUiState.PassiveGetObjCodesApplyFilesへの反映は
        '無用である（編集を確定した時点で実施されるはずである）上、
        'sKeyがNothingの可能性もある。
        'このことから、UiState.PassiveGetObjCodesApplyFilesへの反映には
        '条件を設けている。
        Dim sKey As String = CStr(selectedRow.Cells(0).Value)
        If lastEditRow <> selectedRow.Index AndAlso _
           Not selectedRow.IsNewRow Then
            SyncLock UiState
                UiState.PassiveGetObjCodesApplyFiles(Byte.Parse(sKey, NumberStyles.HexNumber)) = FileSelDialog.FileName
            End SyncLock
        End If

        selectedRow.Cells(1).Selected = True
        selectedRow.Cells(1).Value = FileSelDialog.FileName
    End Sub

    Private Sub PassiveGetDataGridView_DefaultValuesNeeded(ByVal sender As System.Object, ByVal e As DataGridViewRowEventArgs) Handles PassiveGetDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing
        e.Row.Cells(1).Value = Nothing

        'NOTE: DataGridViewは、新規の行（アスタリスクの行）を選択後、
        'ダブルクリックやキー入力で編集を開始した際、編集中と同じ状態
        'になる一方で、CellBeginEditイベントは発生しないようなので、
        'まだ選択されただけの段階ではあるが、CellBeginEditイベント
        '発生時と同じ処理をここで実施することにしている。
        'この処置のせいで、真に編集中でない場合（キャレットが登場
        'していない場合）でもlastEditRowは-1以外になり得るので注意。
        'lastEditRow行のIsNewRowプロパティがTrueでかつ、その全セルが
        '空の場合は、真に編集中ではないとみなすことにする。
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveGetDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveGetDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: 仮に新規の行を編集開始して、ここが実行される場合は、
            'Nothingを代入することになる。
            sKeyAtBeginEditRowInDataGridView = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Value)
        End If
    End Sub

    Private Sub PassiveGetDataGridView_RowValidating(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveGetDataGridView.RowValidating
        If lastEditRow <> -1 Then
            Dim sNewKey As String = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Value)
            Dim sNewApplyFile As String = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(1).Value)

            If PassiveGetDataGridView.Rows(e.RowIndex).IsNewRow AndAlso _
               String.IsNullOrEmpty(sNewKey) AndAlso _
               String.IsNullOrEmpty(sNewApplyFile) Then Return

            Dim newKey As Byte
            If Byte.TryParse(sNewKey, NumberStyles.HexNumber, Nothing, newKey) = False Then
                PassiveGetDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsUnsuitableForObjCode.Gen()
                e.Cancel = True
                Return
            End If

            '新規の行を挿入した場合や、既に存在する行のキーを変更した場合は、
            '新しいキーが、他の行のキーと重複していないかチェックする。
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: このスレッド以外は、UiStateを参照するだけなので、このスレッドで
                'UiStateを参照するだけであれば、SyncLock UiStateは不要である。
                If UiState.PassiveGetObjCodesApplyFiles.ContainsKey(newKey) Then
                    PassiveGetDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsDuplicative.Gen()
                    e.Cancel = True
                    Return
                End If
            End If

            'ローカルファイルパスの書式をチェックしておく。
            Try
                Dim code As EkCode
                Dim sFilePath As String = sNewApplyFile
                sFilePath = sFilePath.Replace("%%", vbLf)
                sFilePath = MyUtility.ReplaceMachineIndex(sFilePath, 9999)
                sFilePath = code.ToString(sFilePath).Replace(ControlChars.Lf, "%"c)
                If sFilePath.Contains("$ContextDir<") OrElse _
                   sFilePath.Contains("$ContextNum<") OrElse _
                   sFilePath.Contains("$SetRef<") OrElse _
                   sFilePath.Contains("$SetVal<") OrElse _
                   sFilePath.Contains("$Val<") OrElse _
                   sFilePath.Contains("$ExecDynFunc<") OrElse _
                   sFilePath.Contains("$ExecCmdFunc<") OrElse _
                   sFilePath.Contains("$ExecAppFunc<") Then Throw New FormatException()
                sFilePath = oStringExpander.Expand(sFilePath, Nothing, 0)
                Path.GetDirectoryName(sFilePath)
            Catch ex As Exception
                PassiveGetDataGridView.Rows(e.RowIndex).ErrorText = Lexis.FilePathIsInvalid.Gen()
                e.Cancel = True
                Return
            End Try
        End If
    End Sub

    Private Sub PassiveGetDataGridView_RowValidated(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles PassiveGetDataGridView.RowValidated
        If lastEditRow <> -1 Then
            PassiveGetDataGridView.Rows(e.RowIndex).ErrorText = Nothing

            Dim sNewKey As String = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(0).Value)
            Dim sNewApplyFile As String = CStr(PassiveGetDataGridView.Rows(e.RowIndex).Cells(1).Value)
            If sNewApplyFile Is Nothing Then
                sNewApplyFile = ""
            End If

            SyncLock UiState
                If sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
                    UiState.PassiveGetObjCodesApplyFiles.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
                End If

                'NOTE: 以下の分岐に入らないケースは、RowValidatingで特別扱いしたケースである故、
                'sNewApplyFileも確実に空である。また、そのケースは、Rows(e.RowIndex).IsNewRow が
                'True である故、DataGridView上に行は追加されておらず、Rows.RemoveAt(e.RowIndex)
                'も無用である。
                If Not String.IsNullOrEmpty(sNewKey) Then
                    Dim newKey As Byte = Byte.Parse(sNewKey, NumberStyles.HexNumber)
                    UiState.PassiveGetObjCodesApplyFiles.Add(newKey, sNewApplyFile)
                End If
            End SyncLock

            lastEditRow = -1
        End If
    End Sub

    Private Sub PassiveGetForceReplyNakCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveGetForceReplyNakCheckBox.CheckedChanged
        SyncLock UiState
            UiState.PassiveGetForceReplyNak = PassiveGetForceReplyNakCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassiveGetNakCauseNumberTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveGetNakCauseNumberTextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveGetNakCauseNumberTextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveGetNakCauseNumberTextBox.Text)
        End If

        SyncLock UiState
            UiState.PassiveGetNakCauseNumber = number
        End SyncLock
    End Sub

    Private Sub PassiveGetNakCauseTextTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveGetNakCauseTextTextBox.TextChanged
        SyncLock UiState
            UiState.PassiveGetNakCauseText = PassiveGetNakCauseTextTextBox.Text
        End SyncLock
    End Sub

    Private Sub PassiveUllDataGridView_CellMouseClick(ByVal sender As System.Object, ByVal e As DataGridViewCellMouseEventArgs) Handles PassiveUllDataGridView.CellMouseClick
        '左クリックの場合は、このメソッドでは処理しない。
        If e.Button <> MouseButtons.Right Then Return

        '列ヘッダを右クリックした場合は、このメソッドでは処理しない。
        If e.RowIndex = -1 Then Return

        '右クリックした場所に選択を移す。
        If e.ColumnIndex = -1 Then
            'NOTE: 行ヘッダが右クリックされた場合である。
            '当該行の１列目セルを選択しているが、これは、行ヘッダを選択しても
            '直前まで選択されていた行の妥当性チェックが実行されないことおよび、
            '直前まで選択されていた行の選択が解除されないことに対処するためである。
            PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveUllDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '右クリックした行とは別の編集中の行が存在する場合は、メニューは出さない。
        'ただし、編集中の行が真の編集中ではない場合は、メニューを出す。
        If lastEditRow <> -1 AndAlso lastEditRow <> e.RowIndex Then
            If Not (PassiveUllDataGridView.Rows(lastEditRow).IsNewRow AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveUllDataGridView.Rows(lastEditRow).Cells(0).Value)) AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveUllDataGridView.Rows(lastEditRow).Cells(1).Value))) Then
                Return
            End If
        End If

        If e.ColumnIndex = -1 Then
            PassiveUllDataGridView.Rows(e.RowIndex).Selected = True
            Application.DoEvents()
            PassiveUllRowHeaderMenu.Show(PassiveUllDataGridView, PassiveUllDataGridView.GetRowDisplayRectangle(e.RowIndex, False).Location + New Size(e.Location))
        ElseIf e.ColumnIndex = 1 Then
            PassiveUllApplyFileMenu.Show(PassiveUllDataGridView, PassiveUllDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False).Location + New Size(e.Location))
        End If
    End Sub

    Private Sub PassiveUllDelMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveUllDelMenuItem.Click
        RemovePassiveUllData()
    End Sub

    Private Sub PassiveUllSelMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveUllSelMenuItem.Click
        SelectPassiveUllDataApplyFile()
    End Sub

    Private Sub PassiveUllDataGridView_KeyDown(ByVal sender As System.Object, ByVal e As KeyEventArgs) Handles PassiveUllDataGridView.KeyDown
        Select Case e.KeyData
            Case Keys.Delete
                If PassiveUllDataGridView.SelectedRows.Count = 1 Then
                    RemovePassiveUllData()
                    e.Handled = True
                End If
            Case Keys.Apps
                If PassiveUllDataGridView.SelectedRows.Count = 0 AndAlso _
                   PassiveUllDataGridView.SelectedCells.Count = 1 AndAlso _
                   PassiveUllDataGridView.SelectedCells(0).ColumnIndex = 1 Then
                    Dim r As Rectangle = PassiveUllDataGridView.GetCellDisplayRectangle(1, PassiveUllDataGridView.SelectedCells(0).RowIndex, False)
                    PassiveUllApplyFileMenu.Show(PassiveUllDataGridView, r.Location + New Size((r.Size.Width - PassiveUllApplyFileMenu.Size.Width) \ 2, r.Size.Height))
                    e.Handled = True
                End If
        End Select
    End Sub

    Private Sub RemovePassiveUllData()
        Dim selectedRow As DataGridViewRow = PassiveUllDataGridView.SelectedRows(0)

        If selectedRow.IsNewRow Then
            selectedRow.Cells(0).Value = Nothing
            selectedRow.Cells(1).Value = Nothing
            selectedRow.ErrorText = Nothing
            Return
        End If

        If lastEditRow <> selectedRow.Index Then
            SyncLock UiState
                UiState.PassiveUllObjCodesApplyFiles.Remove(Byte.Parse(CStr(selectedRow.Cells(0).Value), NumberStyles.HexNumber))
            End SyncLock
        ElseIf sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView IsNot Nothing)
            SyncLock UiState
                UiState.PassiveUllObjCodesApplyFiles.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
            End SyncLock
        Else
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView Is Nothing)
            'NOTE: おそらく、selectedRow.IsNewRowに該当するケースであるため、
            'ここまで到達しないと思われる。たとえ到達したとしても、
            '新規の行を編集中にその行の削除を実施した場合である故、
            'Dictionaryには内容を登録していないので、Dictionaryからの削除は無用である。
        End If

        If lastEditRow = selectedRow.Index Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどで、余計なことが
            '行われないように、この時点で、編集中ではなかったことにしておく。
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどを処理する際、
            'lastEditRowが位置として参照されないことを前提に、
            '邪道ではあるが、この時点で補正を行っておく。
            'NOTE: デクリメント前の時点でlastEditRowは1以上であるため、
            'デクリメントの結果が-1やそれ以下になることはない。
            'NOTE: Rows.RemoveAt(...)によるRowValidatedイベントを処理する際、
            'lastEditRowは-1に変更される。さらに、Rows.RemoveAt(...)の結果として
            '既存の行が全て無くなれば、DefaultValuesNeededイベントが発生し、
            'lastEditRowは新規行の位置（おそらく0）に変更される。つまり、
            'この補正は、事実上不要である可能性が高い。
            lastEditRow -= 1
        End If

        PassiveUllDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub SelectPassiveUllDataApplyFile()
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return

        Dim selectedRow As DataGridViewRow = PassiveUllDataGridView.Rows(PassiveUllDataGridView.SelectedCells(0).RowIndex)

        'NOTE: 編集中の行や新規の行に対して、ファイル名の選択を実施した場合、
        'その場でのUiState.PassiveUllObjCodesApplyFilesへの反映は
        '無用である（編集を確定した時点で実施されるはずである）上、
        'sKeyがNothingの可能性もある。
        'このことから、UiState.PassiveUllObjCodesApplyFilesへの反映には
        '条件を設けている。
        Dim sKey As String = CStr(selectedRow.Cells(0).Value)
        If lastEditRow <> selectedRow.Index AndAlso _
           Not selectedRow.IsNewRow Then
            SyncLock UiState
                UiState.PassiveUllObjCodesApplyFiles(Byte.Parse(sKey, NumberStyles.HexNumber)) = FileSelDialog.FileName
            End SyncLock
        End If

        selectedRow.Cells(1).Selected = True
        selectedRow.Cells(1).Value = FileSelDialog.FileName
    End Sub

    Private Sub PassiveUllDataGridView_DefaultValuesNeeded(ByVal sender As System.Object, ByVal e As DataGridViewRowEventArgs) Handles PassiveUllDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing
        e.Row.Cells(1).Value = Nothing

        'NOTE: DataGridViewは、新規の行（アスタリスクの行）を選択後、
        'ダブルクリックやキー入力で編集を開始した際、編集中と同じ状態
        'になる一方で、CellBeginEditイベントは発生しないようなので、
        'まだ選択されただけの段階ではあるが、CellBeginEditイベント
        '発生時と同じ処理をここで実施することにしている。
        'この処置のせいで、真に編集中でない場合（キャレットが登場
        'していない場合）でもlastEditRowは-1以外になり得るので注意。
        'lastEditRow行のIsNewRowプロパティがTrueでかつ、その全セルが
        '空の場合は、真に編集中ではないとみなすことにする。
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveUllDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveUllDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: 仮に新規の行を編集開始して、ここが実行される場合は、
            'Nothingを代入することになる。
            sKeyAtBeginEditRowInDataGridView = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Value)
        End If
    End Sub

    Private Sub PassiveUllDataGridView_RowValidating(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveUllDataGridView.RowValidating
        If lastEditRow <> -1 Then
            Dim sNewKey As String = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Value)
            Dim sNewApplyFile As String = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(1).Value)

            If PassiveUllDataGridView.Rows(e.RowIndex).IsNewRow AndAlso _
               String.IsNullOrEmpty(sNewKey) AndAlso _
               String.IsNullOrEmpty(sNewApplyFile) Then Return

            Dim newKey As Byte
            If Byte.TryParse(sNewKey, NumberStyles.HexNumber, Nothing, newKey) = False Then
                PassiveUllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsUnsuitableForObjCode.Gen()
                e.Cancel = True
                Return
            End If

            '新規の行を挿入した場合や、既に存在する行のキーを変更した場合は、
            '新しいキーが、他の行のキーと重複していないかチェックする。
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: このスレッド以外は、UiStateを参照するだけなので、このスレッドで
                'UiStateを参照するだけであれば、SyncLock UiStateは不要である。
                If UiState.PassiveUllObjCodesApplyFiles.ContainsKey(newKey) Then
                    PassiveUllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsDuplicative.Gen()
                    e.Cancel = True
                    Return
                End If
            End If

            'ローカルファイルパスの書式をチェックしておく。
            Try
                Dim code As EkCode
                Dim sFilePath As String = sNewApplyFile
                sFilePath = sFilePath.Replace("%%", vbLf)
                sFilePath = MyUtility.ReplaceMachineIndex(sFilePath, 9999)
                sFilePath = code.ToString(sFilePath).Replace(ControlChars.Lf, "%"c)
                If sFilePath.Contains("$ContextDir<") OrElse _
                   sFilePath.Contains("$ContextNum<") OrElse _
                   sFilePath.Contains("$SetRef<") OrElse _
                   sFilePath.Contains("$SetVal<") OrElse _
                   sFilePath.Contains("$Val<") OrElse _
                   sFilePath.Contains("$ExecDynFunc<") OrElse _
                   sFilePath.Contains("$ExecCmdFunc<") OrElse _
                   sFilePath.Contains("$ExecAppFunc<") Then Throw New FormatException()
                sFilePath = oStringExpander.Expand(sFilePath, Nothing, 0)
                Path.GetDirectoryName(sFilePath)
            Catch ex As Exception
                PassiveUllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.FilePathIsInvalid.Gen()
                e.Cancel = True
                Return
            End Try
        End If
    End Sub

    Private Sub PassiveUllDataGridView_RowValidated(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles PassiveUllDataGridView.RowValidated
        If lastEditRow <> -1 Then
            PassiveUllDataGridView.Rows(e.RowIndex).ErrorText = Nothing

            Dim sNewKey As String = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(0).Value)
            Dim sNewApplyFile As String = CStr(PassiveUllDataGridView.Rows(e.RowIndex).Cells(1).Value)
            If sNewApplyFile Is Nothing Then
                sNewApplyFile = ""
            End If

            SyncLock UiState
                If sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
                    UiState.PassiveUllObjCodesApplyFiles.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
                End If

                'NOTE: 以下の分岐に入らないケースは、RowValidatingで特別扱いしたケースである故、
                'sNewApplyFileも確実に空である。また、そのケースは、Rows(e.RowIndex).IsNewRow が
                'True である故、DataGridView上に行は追加されておらず、Rows.RemoveAt(e.RowIndex)
                'も無用である。
                If Not String.IsNullOrEmpty(sNewKey) Then
                    Dim newKey As Byte = Byte.Parse(sNewKey, NumberStyles.HexNumber)
                    UiState.PassiveUllObjCodesApplyFiles.Add(newKey, sNewApplyFile)
                End If
            End SyncLock

            lastEditRow = -1
        End If
    End Sub

    Private Sub PassiveUllForceReplyNakCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveUllForceReplyNakCheckBox.CheckedChanged
        SyncLock UiState
            UiState.PassiveUllForceReplyNak = PassiveUllForceReplyNakCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassiveUllNakCauseNumberTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveUllNakCauseNumberTextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveUllNakCauseNumberTextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveUllNakCauseNumberTextBox.Text)
        End If

        SyncLock UiState
            UiState.PassiveUllNakCauseNumber = number
        End SyncLock
    End Sub

    Private Sub PassiveUllNakCauseTextTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveUllNakCauseTextTextBox.TextChanged
        SyncLock UiState
            UiState.PassiveUllNakCauseText = PassiveUllNakCauseTextTextBox.Text
        End SyncLock
    End Sub

    Private Sub PassiveUllTransferLimitNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveUllTransferLimitNumericUpDown.ValueChanged
        SyncLock UiState
            UiState.PassiveUllTransferLimitTicks = Decimal.ToInt32(PassiveUllTransferLimitNumericUpDown.Value)
        End SyncLock
    End Sub

    Private Sub PassiveUllReplyLimitNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveUllReplyLimitNumericUpDown.ValueChanged
        SyncLock UiState
            UiState.PassiveUllReplyLimitTicks = Decimal.ToInt32(PassiveUllReplyLimitNumericUpDown.Value)
        End SyncLock
    End Sub

    Private Sub PassivePostDataGridView_CellMouseClick(ByVal sender As System.Object, ByVal e As DataGridViewCellMouseEventArgs) Handles PassivePostDataGridView.CellMouseClick
        '左クリックの場合は、このメソッドでは処理しない。
        If e.Button <> MouseButtons.Right Then Return

        '列ヘッダを右クリックした場合は、このメソッドでは処理しない。
        If e.RowIndex = -1 Then Return

        '右クリックした場所に選択を移す。
        If e.ColumnIndex = -1 Then
            'NOTE: 行ヘッダが右クリックされた場合である。
            '当該行の１列目セルを選択しているが、これは、行ヘッダを選択しても
            '直前まで選択されていた行の妥当性チェックが実行されないことおよび、
            '直前まで選択されていた行の選択が解除されないことに対処するためである。
            PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassivePostDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '右クリックした行とは別の編集中の行が存在する場合は、メニューは出さない。
        'ただし、編集中の行が真の編集中ではない場合は、メニューを出す。
        If lastEditRow <> -1 AndAlso lastEditRow <> e.RowIndex Then
            If Not (PassivePostDataGridView.Rows(lastEditRow).IsNewRow AndAlso _
                    String.IsNullOrEmpty(CStr(PassivePostDataGridView.Rows(lastEditRow).Cells(0).Value))) Then
                Return
            End If
        End If

        If e.ColumnIndex = -1 Then
            PassivePostDataGridView.Rows(e.RowIndex).Selected = True
            Application.DoEvents()
            PassivePostRowHeaderMenu.Show(PassivePostDataGridView, PassivePostDataGridView.GetRowDisplayRectangle(e.RowIndex, False).Location + New Size(e.Location))
        End If
    End Sub

    Private Sub PassivePostDelMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassivePostDelMenuItem.Click
        RemovePassivePostData()
    End Sub

    Private Sub PassivePostDataGridView_KeyDown(ByVal sender As System.Object, ByVal e As KeyEventArgs) Handles PassivePostDataGridView.KeyDown
        Select Case e.KeyData
            Case Keys.Delete
                If PassivePostDataGridView.SelectedRows.Count = 1 Then
                    RemovePassivePostData()
                    e.Handled = True
                End If
        End Select
    End Sub

    Private Sub RemovePassivePostData()
        Dim selectedRow As DataGridViewRow = PassivePostDataGridView.SelectedRows(0)

        If selectedRow.IsNewRow Then
            selectedRow.Cells(0).Value = Nothing
            selectedRow.ErrorText = Nothing
            Return
        End If

        If lastEditRow <> selectedRow.Index Then
            SyncLock UiState
                UiState.PassivePostObjCodes.Remove(Byte.Parse(CStr(selectedRow.Cells(0).Value), NumberStyles.HexNumber))
            End SyncLock
        ElseIf sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView IsNot Nothing)
            SyncLock UiState
                UiState.PassivePostObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
            End SyncLock
        Else
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView Is Nothing)
            'NOTE: おそらく、selectedRow.IsNewRowに該当するケースであるため、
            'ここまで到達しないと思われる。たとえ到達したとしても、
            '新規の行を編集中にその行の削除を実施した場合である故、
            'Dictionaryには内容を登録していないので、Dictionaryからの削除は無用である。
        End If

        If lastEditRow = selectedRow.Index Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどで、余計なことが
            '行われないように、この時点で、編集中ではなかったことにしておく。
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどを処理する際、
            'lastEditRowが位置として参照されないことを前提に、
            '邪道ではあるが、この時点で補正を行っておく。
            'NOTE: デクリメント前の時点でlastEditRowは1以上であるため、
            'デクリメントの結果が-1やそれ以下になることはない。
            'NOTE: Rows.RemoveAt(...)によるRowValidatedイベントを処理する際、
            'lastEditRowは-1に変更される。さらに、Rows.RemoveAt(...)の結果として
            '既存の行が全て無くなれば、DefaultValuesNeededイベントが発生し、
            'lastEditRowは新規行の位置（おそらく0）に変更される。つまり、
            'この補正は、事実上不要である可能性が高い。
            lastEditRow -= 1
        End If

        PassivePostDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub PassivePostDataGridView_DefaultValuesNeeded(ByVal sender As System.Object, ByVal e As DataGridViewRowEventArgs) Handles PassivePostDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing

        'NOTE: DataGridViewは、新規の行（アスタリスクの行）を選択後、
        'ダブルクリックやキー入力で編集を開始した際、編集中と同じ状態
        'になる一方で、CellBeginEditイベントは発生しないようなので、
        'まだ選択されただけの段階ではあるが、CellBeginEditイベント
        '発生時と同じ処理をここで実施することにしている。
        'この処置のせいで、真に編集中でない場合（キャレットが登場
        'していない場合）でもlastEditRowは-1以外になり得るので注意。
        'lastEditRow行のIsNewRowプロパティがTrueでかつ、その全セルが
        '空の場合は、真に編集中ではないとみなすことにする。
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassivePostDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassivePostDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: 仮に新規の行を編集開始して、ここが実行される場合は、
            'Nothingを代入することになる。
            sKeyAtBeginEditRowInDataGridView = CStr(PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Value)
        End If
    End Sub

    Private Sub PassivePostDataGridView_RowValidating(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassivePostDataGridView.RowValidating
        If lastEditRow <> -1 Then
            Dim sNewKey As String = CStr(PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Value)

            If PassivePostDataGridView.Rows(e.RowIndex).IsNewRow AndAlso _
               String.IsNullOrEmpty(sNewKey) Then Return

            Dim newKey As Byte
            If Byte.TryParse(sNewKey, NumberStyles.HexNumber, Nothing, newKey) = False Then
                PassivePostDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsUnsuitableForObjCode.Gen()
                e.Cancel = True
                Return
            End If

            '新規の行を挿入した場合や、既に存在する行のキーを変更した場合は、
            '新しいキーが、他の行のキーと重複していないかチェックする。
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: このスレッド以外は、UiStateを参照するだけなので、このスレッドで
                'UiStateを参照するだけであれば、SyncLock UiStateは不要である。
                If UiState.PassivePostObjCodes.ContainsKey(newKey) Then
                    PassivePostDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsDuplicative.Gen()
                    e.Cancel = True
                    Return
                End If
            End If
        End If
    End Sub

    Private Sub PassivePostDataGridView_RowValidated(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles PassivePostDataGridView.RowValidated
        If lastEditRow <> -1 Then
            PassivePostDataGridView.Rows(e.RowIndex).ErrorText = Nothing

            Dim sNewKey As String = CStr(PassivePostDataGridView.Rows(e.RowIndex).Cells(0).Value)

            SyncLock UiState
                If sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
                    UiState.PassivePostObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
                End If

                'NOTE: 以下の分岐に入らないケースは、Rows(e.RowIndex).IsNewRow が
                'True である故、DataGridView上に行は追加されておらず、Rows.RemoveAt(e.RowIndex)
                'も無用である。
                If Not String.IsNullOrEmpty(sNewKey) Then
                    Dim newKey As Byte = Byte.Parse(sNewKey, NumberStyles.HexNumber)
                    UiState.PassivePostObjCodes.Add(newKey, Nothing)
                End If
            End SyncLock

            lastEditRow = -1
        End If
    End Sub

    Private Sub PassivePostForceReplyNakCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassivePostForceReplyNakCheckBox.CheckedChanged
        SyncLock UiState
            UiState.PassivePostForceReplyNak = PassivePostForceReplyNakCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassivePostNakCauseNumberTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassivePostNakCauseNumberTextBox.TextChanged
        Dim number As Integer = 0
        If Not PassivePostNakCauseNumberTextBox.Text.Equals("") Then
            number = Integer.Parse(PassivePostNakCauseNumberTextBox.Text)
        End If

        SyncLock UiState
            UiState.PassivePostNakCauseNumber = number
        End SyncLock
    End Sub

    Private Sub PassivePostNakCauseTextTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassivePostNakCauseTextTextBox.TextChanged
        SyncLock UiState
            UiState.PassivePostNakCauseText = PassivePostNakCauseTextTextBox.Text
        End SyncLock
    End Sub

    Private Sub PassiveDllDataGridView_CellMouseClick(ByVal sender As System.Object, ByVal e As DataGridViewCellMouseEventArgs) Handles PassiveDllDataGridView.CellMouseClick
        '左クリックの場合は、このメソッドでは処理しない。
        If e.Button <> MouseButtons.Right Then Return

        '列ヘッダを右クリックした場合は、このメソッドでは処理しない。
        If e.RowIndex = -1 Then Return

        '右クリックした場所に選択を移す。
        If e.ColumnIndex = -1 Then
            'NOTE: 行ヘッダが右クリックされた場合である。
            '当該行の１列目セルを選択しているが、これは、行ヘッダを選択しても
            '直前まで選択されていた行の妥当性チェックが実行されないことおよび、
            '直前まで選択されていた行の選択が解除されないことに対処するためである。
            PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Selected = True
            Application.DoEvents()
        Else
            PassiveDllDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
            Application.DoEvents()
        End If

        '右クリックした行とは別の編集中の行が存在する場合は、メニューは出さない。
        'ただし、編集中の行が真の編集中ではない場合は、メニューを出す。
        If lastEditRow <> -1 AndAlso lastEditRow <> e.RowIndex Then
            If Not (PassiveDllDataGridView.Rows(lastEditRow).IsNewRow AndAlso _
                    String.IsNullOrEmpty(CStr(PassiveDllDataGridView.Rows(lastEditRow).Cells(0).Value))) Then
                Return
            End If
        End If

        If e.ColumnIndex = -1 Then
            PassiveDllDataGridView.Rows(e.RowIndex).Selected = True
            Application.DoEvents()
            PassiveDllRowHeaderMenu.Show(PassiveDllDataGridView, PassiveDllDataGridView.GetRowDisplayRectangle(e.RowIndex, False).Location + New Size(e.Location))
        End If
    End Sub

    Private Sub PassiveDllDelMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllDelMenuItem.Click
        RemovePassiveDllData()
    End Sub

    Private Sub PassiveDllDataGridView_KeyDown(ByVal sender As System.Object, ByVal e As KeyEventArgs) Handles PassiveDllDataGridView.KeyDown
        Select Case e.KeyData
            Case Keys.Delete
                If PassiveDllDataGridView.SelectedRows.Count = 1 Then
                    RemovePassiveDllData()
                    e.Handled = True
                End If
        End Select
    End Sub

    Private Sub RemovePassiveDllData()
        Dim selectedRow As DataGridViewRow = PassiveDllDataGridView.SelectedRows(0)

        If selectedRow.IsNewRow Then
            selectedRow.Cells(0).Value = Nothing
            selectedRow.ErrorText = Nothing
            Return
        End If

        If lastEditRow <> selectedRow.Index Then
            SyncLock UiState
                UiState.PassiveDllObjCodes.Remove(Byte.Parse(CStr(selectedRow.Cells(0).Value), NumberStyles.HexNumber))
            End SyncLock
        ElseIf sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView IsNot Nothing)
            SyncLock UiState
                UiState.PassiveDllObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
            End SyncLock
        Else
            '(lastEditRow = selectedRow.Index AndAlso sKeyAtBeginEditRowInDataGridView Is Nothing)
            'NOTE: おそらく、selectedRow.IsNewRowに該当するケースであるため、
            'ここまで到達しないと思われる。たとえ到達したとしても、
            '新規の行を編集中にその行の削除を実施した場合である故、
            'Dictionaryには内容を登録していないので、Dictionaryからの削除は無用である。
        End If

        If lastEditRow = selectedRow.Index Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどで、余計なことが
            '行われないように、この時点で、編集中ではなかったことにしておく。
            lastEditRow = -1
        ElseIf selectedRow.Index < lastEditRow Then
            'この後のRows.RemoveAt(...)によるRowValidatingイベントなどを処理する際、
            'lastEditRowが位置として参照されないことを前提に、
            '邪道ではあるが、この時点で補正を行っておく。
            'NOTE: デクリメント前の時点でlastEditRowは1以上であるため、
            'デクリメントの結果が-1やそれ以下になることはない。
            'NOTE: Rows.RemoveAt(...)によるRowValidatedイベントを処理する際、
            'lastEditRowは-1に変更される。さらに、Rows.RemoveAt(...)の結果として
            '既存の行が全て無くなれば、DefaultValuesNeededイベントが発生し、
            'lastEditRowは新規行の位置（おそらく0）に変更される。つまり、
            'この補正は、事実上不要である可能性が高い。
            lastEditRow -= 1
        End If

        PassiveDllDataGridView.Rows.RemoveAt(selectedRow.Index)
    End Sub

    Private Sub PassiveDllDataGridView_DefaultValuesNeeded(ByVal sender As System.Object, ByVal e As DataGridViewRowEventArgs) Handles PassiveDllDataGridView.DefaultValuesNeeded
        e.Row.Cells(0).Value = Nothing

        'NOTE: DataGridViewは、新規の行（アスタリスクの行）を選択後、
        'ダブルクリックやキー入力で編集を開始した際、編集中と同じ状態
        'になる一方で、CellBeginEditイベントは発生しないようなので、
        'まだ選択されただけの段階ではあるが、CellBeginEditイベント
        '発生時と同じ処理をここで実施することにしている。
        'この処置のせいで、真に編集中でない場合（キャレットが登場
        'していない場合）でもlastEditRowは-1以外になり得るので注意。
        'lastEditRow行のIsNewRowプロパティがTrueでかつ、その全セルが
        '空の場合は、真に編集中ではないとみなすことにする。
        Debug.Assert(lastEditRow = -1)
        lastEditRow = e.Row.Index
        sKeyAtBeginEditRowInDataGridView = Nothing
    End Sub

    Private Sub PassiveDllDataGridView_CellBeginEdit(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveDllDataGridView.CellBeginEdit
        If lastEditRow <> e.RowIndex Then
            lastEditRow = e.RowIndex

            'NOTE: 仮に新規の行を編集開始して、ここが実行される場合は、
            'Nothingを代入することになる。
            sKeyAtBeginEditRowInDataGridView = CStr(PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Value)
        End If
    End Sub

    Private Sub PassiveDllDataGridView_RowValidating(ByVal sender As System.Object, ByVal e As DataGridViewCellCancelEventArgs) Handles PassiveDllDataGridView.RowValidating
        If lastEditRow <> -1 Then
            Dim sNewKey As String = CStr(PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Value)

            If PassiveDllDataGridView.Rows(e.RowIndex).IsNewRow AndAlso _
               String.IsNullOrEmpty(sNewKey) Then Return

            Dim newKey As Byte
            If Byte.TryParse(sNewKey, NumberStyles.HexNumber, Nothing, newKey) = False Then
                PassiveDllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsUnsuitableForObjCode.Gen()
                e.Cancel = True
                Return
            End If

            '新規の行を挿入した場合や、既に存在する行のキーを変更した場合は、
            '新しいキーが、他の行のキーと重複していないかチェックする。
            If sKeyAtBeginEditRowInDataGridView Is Nothing OrElse _
               Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber) <> newKey Then

                'NOTE: このスレッド以外は、UiStateを参照するだけなので、このスレッドで
                'UiStateを参照するだけであれば、SyncLock UiStateは不要である。
                If UiState.PassiveDllObjCodes.ContainsKey(newKey) Then
                    PassiveDllDataGridView.Rows(e.RowIndex).ErrorText = Lexis.TheInputValueIsDuplicative.Gen()
                    e.Cancel = True
                    Return
                End If
            End If
        End If
    End Sub

    Private Sub PassiveDllDataGridView_RowValidated(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles PassiveDllDataGridView.RowValidated
        If lastEditRow <> -1 Then
            PassiveDllDataGridView.Rows(e.RowIndex).ErrorText = Nothing

            Dim sNewKey As String = CStr(PassiveDllDataGridView.Rows(e.RowIndex).Cells(0).Value)

            SyncLock UiState
                If sKeyAtBeginEditRowInDataGridView IsNot Nothing Then
                    UiState.PassiveDllObjCodes.Remove(Byte.Parse(sKeyAtBeginEditRowInDataGridView, NumberStyles.HexNumber))
                End If

                'NOTE: 以下の分岐に入らないケースは、Rows(e.RowIndex).IsNewRow が
                'True である故、DataGridView上に行は追加されておらず、Rows.RemoveAt(e.RowIndex)
                'も無用である。
                If Not String.IsNullOrEmpty(sNewKey) Then
                    Dim newKey As Byte = Byte.Parse(sNewKey, NumberStyles.HexNumber)
                    UiState.PassiveDllObjCodes.Add(newKey, Nothing)
                End If
            End SyncLock

            lastEditRow = -1
        End If
    End Sub

    Private Sub PassiveDllForceReplyNakCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllForceReplyNakCheckBox.CheckedChanged
        SyncLock UiState
            UiState.PassiveDllForceReplyNak = PassiveDllForceReplyNakCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassiveDllNakCauseNumberTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllNakCauseNumberTextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveDllNakCauseNumberTextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveDllNakCauseNumberTextBox.Text)
        End If

        SyncLock UiState
            UiState.PassiveDllNakCauseNumber = number
        End SyncLock
    End Sub

    Private Sub PassiveDllNakCauseTextTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllNakCauseTextTextBox.TextChanged
        SyncLock UiState
            UiState.PassiveDllNakCauseText = PassiveDllNakCauseTextTextBox.Text
        End SyncLock
    End Sub

    Private Sub PassiveDllTransferLimitNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllTransferLimitNumericUpDown.ValueChanged
        SyncLock UiState
            UiState.PassiveDllTransferLimitTicks = Decimal.ToInt32(PassiveDllTransferLimitNumericUpDown.Value)
        End SyncLock
    End Sub

    Private Sub PassiveDllReplyLimitNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllReplyLimitNumericUpDown.ValueChanged
        SyncLock UiState
            UiState.PassiveDllReplyLimitTicks = Decimal.ToInt32(PassiveDllReplyLimitNumericUpDown.Value)
        End SyncLock
    End Sub

    Private Sub PassiveDllSimulateStoringCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllSimulateStoringCheckBox.CheckedChanged
        SyncLock UiState
            UiState.PassiveDllSimulateStoring = PassiveDllSimulateStoringCheckBox.Checked
        End SyncLock
    End Sub

    Private Sub PassiveDllResultantVersionOfSlot1TextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllResultantVersionOfSlot1TextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveDllResultantVersionOfSlot1TextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveDllResultantVersionOfSlot1TextBox.Text)
        End If

        SyncLock UiState
            UiState.PassiveDllResultantVersionOfSlot1 = number
        End SyncLock
    End Sub

    Private Sub PassiveDllResultantVersionOfSlot2TextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllResultantVersionOfSlot2TextBox.TextChanged
        Dim number As Integer = 0
        If Not PassiveDllResultantVersionOfSlot2TextBox.Text.Equals("") Then
            number = Integer.Parse(PassiveDllResultantVersionOfSlot2TextBox.Text)
        End If

        SyncLock UiState
            UiState.PassiveDllResultantVersionOfSlot2 = number
        End SyncLock
    End Sub

    Private Sub PassiveDllResultantFlagOfFullTextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles PassiveDllResultantFlagOfFullTextBox.KeyPress
        If (e.KeyChar < "0"c OrElse "9"c < e.KeyChar) AndAlso _
           (e.KeyChar < "A"c OrElse "F"c < e.KeyChar) AndAlso _
           (e.KeyChar < "a"c OrElse "f"c < e.KeyChar) AndAlso _
           e.KeyChar <> ControlChars.Back Then
            e.Handled = True
        End If
    End Sub

    Private Sub PassiveDllResultantFlagOfFullTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PassiveDllResultantFlagOfFullTextBox.TextChanged
        Dim code As Integer
        If Integer.TryParse(PassiveDllResultantFlagOfFullTextBox.Text, NumberStyles.HexNumber, Nothing, code) = False Then
            code = &HFF
        End If

        SyncLock UiState
            UiState.PassiveDllResultantFlagOfFull = code
        End SyncLock
    End Sub

    Private Sub ScenarioFileSelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScenarioFileSelButton.Click
        FileSelDialog.FileName = ""
        FileSelDialog.ShowDialog()
        If FileSelDialog.FileName = "" Then Return
        ScenarioFileTextBox.Text = FileSelDialog.FileName
    End Sub

    Private Sub ExtAppTargetQueue_ReceiveCompleted(ByVal sender As System.Object, ByVal e As System.Messaging.ReceiveCompletedEventArgs) Handles ExtAppTargetQueue.ReceiveCompleted
        Try
            DispatchExtMessage(e.Message)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try

        Try
            ExtAppTargetQueue.BeginReceive()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    Private Sub DispatchExtMessage(ByVal oMessage As Message)
        If oMessage.AppSpecific = 1 Then
            Dim bd As ExtSimFuncMessageBody
            Try
                bd = DirectCast(oMessage.Body, ExtSimFuncMessageBody)
            Catch fooEx As Exception
                Log.Error("ExtSimFunc request with bad body received.")
                Return
            End Try

            Dim code As EkCode
            Try
                code = EkCode.Parse(bd.MachineId, EkCodeOupFormat)
            Catch fooEx As Exception
                Log.Error("ExtSimFunc request with bad machine id received.")
                Return
            End Try

            code.Model = Config.SelfEkCode.Model
            Dim oClient As Client = FindClient(code)
            If oClient Is Nothing Then
                Log.Error("ExtSimFunc request for unknown telegrapher [" & bd.MachineId & "] received.")
                Return
            End If

            If bd.Verb Is Nothing Then
                Log.Error("ExtSimFunc request without verb received.")
                Return
            End If

            Select Case bd.Verb.ToUpperInvariant()
                Case "Connect".ToUpperInvariant()
                    If bd.Params IsNot Nothing AndAlso bd.Params.Length <> 0 Then
                        Log.Error("ExtSimFunc (Connect) request with bad arity received.")
                        Return
                    End If

                    SendToTelegrapher(oClient, ConnectRequest.Gen())

                Case "Disconnect".ToUpperInvariant()
                    If bd.Params IsNot Nothing AndAlso bd.Params.Length <> 0 Then
                        Log.Error("ExtSimFunc (Disconnect) request with bad arity received.")
                        Return
                    End If

                    SendToTelegrapher(oClient, DisconnectRequest.Gen())

                Case "ActiveOne".ToUpperInvariant()
                    If bd.Params Is Nothing OrElse bd.Params.Length <> 6 Then
                        Log.Error("ExtSimFunc (ActiveOne) request with bad arity received.")
                        Return
                    End If

                    Dim oExt As New ActiveOneExecRequestExtendPart()
                    Try
                        oExt.ApplyFilePath = DirectCast(bd.Params(0), String)
                        oExt.ReplyLimitTicks = DirectCast(bd.Params(1), Integer)
                        oExt.RetryIntervalTicks = DirectCast(bd.Params(2), Integer)
                        oExt.MaxRetryCountToForget = DirectCast(bd.Params(3), Integer)
                        oExt.MaxRetryCountToCare = DirectCast(bd.Params(4), Integer)
                        oExt.DeleteApplyFileIfCompleted = DirectCast(bd.Params(5), Boolean)
                        oExt.ApplyFileMustExists = True
                    Catch fooEx As Exception
                        Log.Error("ExtSimFunc (ActiveOne) request with bad param received." & vbCrLf & fooEx.Message)
                        Return
                    End Try

                    SendToTelegrapher(oClient, ActiveOneExecRequest.Gen(oExt))

                Case "TryActiveOne".ToUpperInvariant()
                    If bd.Params Is Nothing OrElse bd.Params.Length <> 6 Then
                        Log.Error("ExtSimFunc (TryActiveOne) request with bad arity received.")
                        Return
                    End If

                    Dim oExt As New ActiveOneExecRequestExtendPart()
                    Try
                        oExt.ApplyFilePath = DirectCast(bd.Params(0), String)
                        oExt.ReplyLimitTicks = DirectCast(bd.Params(1), Integer)
                        oExt.RetryIntervalTicks = DirectCast(bd.Params(2), Integer)
                        oExt.MaxRetryCountToForget = DirectCast(bd.Params(3), Integer)
                        oExt.MaxRetryCountToCare = DirectCast(bd.Params(4), Integer)
                        oExt.DeleteApplyFileIfCompleted = DirectCast(bd.Params(5), Boolean)
                        oExt.ApplyFileMustExists = False
                    Catch fooEx As Exception
                        Log.Error("ExtSimFunc (TryActiveOne) request with bad param received." & vbCrLf & fooEx.Message)
                        Return
                    End Try

                    SendToTelegrapher(oClient, ActiveOneExecRequest.Gen(oExt))

                Case "ActiveUll".ToUpperInvariant()
                    If bd.Params Is Nothing OrElse bd.Params.Length <> 11 Then
                        Log.Error("ExtSimFunc (ActiveUll) request with bad arity received.")
                        Return
                    End If

                    Dim oExt As New ActiveUllExecRequestExtendPart()
                    Try
                        oExt.ObjCode = DirectCast(bd.Params(0), Integer)
                        oExt.TransferFileName = DirectCast(bd.Params(1), String)
                        oExt.ApplyFilePath = DirectCast(bd.Params(2), String)
                        oExt.ApplyFileHashValue = DirectCast(bd.Params(3), String)
                        oExt.TransferLimitTicks = DirectCast(bd.Params(4), Integer)
                        oExt.ReplyLimitTicksOnStart = DirectCast(bd.Params(5), Integer)
                        oExt.ReplyLimitTicksOnFinish = DirectCast(bd.Params(6), Integer)
                        oExt.RetryIntervalTicks = DirectCast(bd.Params(7), Integer)
                        oExt.MaxRetryCountToForget = DirectCast(bd.Params(8), Integer)
                        oExt.MaxRetryCountToCare = DirectCast(bd.Params(9), Integer)
                        oExt.DeleteApplyFileIfCompleted = DirectCast(bd.Params(10), Boolean)
                        oExt.ApplyFileMustExists = True
                    Catch fooEx As Exception
                        Log.Error("ExtSimFunc (ActiveUll) request with bad param received." & vbCrLf & fooEx.Message)
                        Return
                    End Try

                    SendToTelegrapher(oClient, ActiveUllExecRequest.Gen(oExt))

                Case "TryActiveUll".ToUpperInvariant()
                    If bd.Params Is Nothing OrElse bd.Params.Length <> 11 Then
                        Log.Error("ExtSimFunc (TryActiveUll) request with bad arity received.")
                        Return
                    End If

                    Dim oExt As New ActiveUllExecRequestExtendPart()
                    Try
                        oExt.ObjCode = DirectCast(bd.Params(0), Integer)
                        oExt.TransferFileName = DirectCast(bd.Params(1), String)
                        oExt.ApplyFilePath = DirectCast(bd.Params(2), String)
                        oExt.ApplyFileHashValue = DirectCast(bd.Params(3), String)
                        oExt.TransferLimitTicks = DirectCast(bd.Params(4), Integer)
                        oExt.ReplyLimitTicksOnStart = DirectCast(bd.Params(5), Integer)
                        oExt.ReplyLimitTicksOnFinish = DirectCast(bd.Params(6), Integer)
                        oExt.RetryIntervalTicks = DirectCast(bd.Params(7), Integer)
                        oExt.MaxRetryCountToForget = DirectCast(bd.Params(8), Integer)
                        oExt.MaxRetryCountToCare = DirectCast(bd.Params(9), Integer)
                        oExt.DeleteApplyFileIfCompleted = DirectCast(bd.Params(10), Boolean)
                        oExt.ApplyFileMustExists = False
                    Catch fooEx As Exception
                        Log.Error("ExtSimFunc (TryActiveUll) request with bad param received." & vbCrLf & fooEx.Message)
                        Return
                    End Try

                    SendToTelegrapher(oClient, ActiveUllExecRequest.Gen(oExt))

                Case Else
                    Log.Error("ExtSimFunc request with unknown verb (" & bd.Verb & ") received.")
            End Select
        Else
            Dim bd As ExtAppFuncMessageBody
            Try
                bd = DirectCast(oMessage.Body, ExtAppFuncMessageBody)
            Catch fooEx As Exception
                Log.Error("ExtAppFunc response with bad body received.")
                Return
            End Try

            Dim sMachineId As String
            Dim code As EkCode
            Try
                sMachineId = Path.GetFileName(Path.GetDirectoryName(bd.WorkingDirectory))
                code = EkCode.Parse(sMachineId, EkCodeOupFormat)
                code.Model = Config.SelfEkCode.Model
            Catch fooEx As Exception
                Log.Error("ExtAppFunc response with bad working path received.")
                Return
            End Try

            Dim oClient As Client = FindClient(code)
            If oClient Is Nothing Then
                Log.Error("ExtAppFunc response for unknown telegrapher [" & sMachineId & "] received.")
                Return
            End If

            Dim oExt As New AppFuncEndNoticeExtendPart()
            oExt.CorrelationId = oMessage.CorrelationId
            oExt.Completed = bd.Completed
            oExt.Result = bd.Result
            SendToTelegrapher(oClient, AppFuncEndNotice.Gen(oExt))
        End If
    End Sub

End Class
