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

Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 駅務機器として運管サーバと電文の送受信を行うクラス。
''' </summary>
Public Class MyTelegrapher
    Inherits JR.ExOpmg.MultiplexEkimuSim.ClientTelegrapher

#Region "定数や変数"
    'スレッド別ディレクトリ名の書式
    Protected Const sDirNameFormat As String = "%3R%3S_%4C_%2U"

    'メインフォームへの参照
    'NOTE: 依頼された通信の結果をUIに反映する際は、
    'このフォームのBeginInvokeメソッドにより、
    'メッセージループで任意のメソッドを実行させる。
    Protected oForm As MainForm

    '電文書式
    Protected oTelegGene As EkTelegramGene

    'FTPサイトのルートと対になるローカルディレクトリ（作業用）
    Protected sFtpBasePath As String

    'FTPサイト内における号機別ディレクトリのパス
    Protected sPermittedPathInFtp As String

    'FTPサイトの号機別ディレクトリと対になるローカルディレクトリ（作業用）
    Protected sPermittedPath As String

    '送受信履歴ディレクトリ
    Protected sCapDirPath As String

    'シミュレータに存在するClient全体の中での項番
    Protected selfIndex As Integer

    '自装置の装置コード
    'TODO: ProcOnReqTelegramReceive()をフックして受信電文のClientCodeと比較してもよい。
    Protected selfEkCode As EkCode

    '次に送信するREQ電文の通番
    Protected reqNumberForNextSnd As Integer

    '次に受信するREQ電文の通番
    'TODO: ProcOnReqTelegramReceive()をフックして、受信したREQ電文の通番の
    '連続性等をチェックするなら用意する。
    'Protected reqNumberForNextRcv As Integer

    'ComStartシーケンスに付与する通番（ログ出力用）
    Protected traceNumberForComStart As Integer

    'TimeDataGetシーケンスに付与する通番（ログ出力用）
    Protected traceNumberForTimeDataGet As Integer

    '任意ActiveOneシーケンスに付与する通番（ログ出力用）
    Protected traceNumberForActiveOne As Integer

    'NOTE: 「意図的な切断」と「異常による切断」を区別したいならば、
    'Protected needConnection As Booleanを用意し、
    'ProcOnConnectNoticeReceive()とProcOnDisconnectRequestReceive()をフックして
    'それをON/OFFするとよい。ProcOnConnectionDisappear()では、それをみて、
    '遷移先の回線状態を決めることができる。

    '端末機器の装置コード
    Protected oTermCodes As List(Of EkCode)

    '親スレッドから受信したメッセージを保留するためのキュー
    Protected oParentMessageQueue As LinkedList(Of InternalMessage)

    '保留したメッセージの処理開始遅延タイマ
    'NOTE: 0 tick で開始するので実際の意味での遅延が発生するわけではない。
    '単に、ProcOnParentmessageReceiveメソッドの中で処理を保留した場合、
    'その続きを同じメソッドの中で行うとProcOnParentmessageReceiveメソッドの
    '再帰呼び出しが発生するはずであるため、スタック使用量を一定範囲に収めるべく、
    'タイマで再開することにしているだけである。
    Protected oParentMessageProcTimer As TickTimer

    '文字列展開言語のインタプリタ
    Protected oStringExpander As StringExpander

    'シナリオ実行の開始遅延タイマ
    Protected oScenarioStartTimer As TickTimer

    'シナリオによる接続であるか
    Protected connectedByScenario As Boolean

    'シナリオ実行環境
    Protected oScenarioEnv As ScenarioEnv

    '回線状態
    Private _LineStatus As Integer

    'ウォッチドッグのデータ種別
    Private Shared ReadOnly ObjCodeForWatchdogIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkWatchdogReqTelegram.FormalObjCodeInTokatsu}, _
       {EkAplProtocol.Kanshiban, EkWatchdogReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkWatchdogReqTelegram.FormalObjCodeInMadosho}, _
       {EkAplProtocol.Kanshiban2, EkWatchdogReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkWatchdogReqTelegram.FormalObjCodeInMadosho}}

    '接続初期化のデータ種別
    Private Shared ReadOnly ObjCodeForComStartIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Kanshiban, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkComStartReqTelegram.FormalObjCodeInMadosho}, _
       {EkAplProtocol.Kanshiban2, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkComStartReqTelegram.FormalObjCodeInMadosho}}

    '整時データ取得のデータ種別
    Private Shared ReadOnly ObjCodeForTimeDataGetIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkTimeDataGetReqTelegram.FormalObjCodeInTokatsu}, _
       {EkAplProtocol.Kanshiban, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Kanshiban2, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}}
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal oTelegGene As EkTelegramGene, ByVal selfIndex As Integer, ByVal selfEkCode As EkCode, ByVal sFtpBasePath As String, ByVal sCapBasePath As String, ByVal sAddr As String, ByVal oMachineDataTable As DataTable, ByVal oForm As MainForm)
        MyBase.New(sThreadName, oParentMessageSock, New EkTelegramImporter(oTelegGene))
        Me.oTelegGene = oTelegGene
        Me.sFtpBasePath = sFtpBasePath
        Me.sCapDirPath = Path.Combine(sCapBasePath, selfEkCode.ToString(sDirNameFormat))
        Me.oForm = oForm
        Me.reqNumberForNextSnd = 0
        Me.traceNumberForTimeDataGet = 0
        Me.LineStatus = LineStatus.Initial

        Me.selfIndex = selfIndex
        Me.selfEkCode = selfEkCode
        Me.oWatchdogTimer.Renew(Config.WatchdogIntervalLimitTicks)
        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite
        Me.enableWatchdog = Config.EnableWatchdog
        Me.enableXllStrongExclusion = Config.EnableXllStrongExclusion
        Me.enableActiveSeqStrongExclusion = Config.EnableActiveSeqStrongExclusion
        Me.enableActiveOneOrdering = Config.EnableActiveOneOrdering

        '送受信履歴ディレクトリについて、無ければ作成しておく。
        Directory.CreateDirectory(sCapDirPath)

        Dim oActiveChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(Me.oActiveXllWorkerMessageSock, oActiveChildSock)
        Me.oActiveXllWorker = New FtpWorker( _
           sThreadName & "-ActiveXll", _
           oActiveChildSock, _
           Config.FtpServerUri, _
           New NetworkCredential(Config.FtpUserName, Config.FtpPassword), _
           Config.ActiveFtpRequestLimitTicks, _
           Config.ActiveFtpLogoutLimitTicks, _
           Config.ActiveFtpTransferStallLimitTicks, _
           Config.ActiveFtpUsePassiveMode, _
           Config.ActiveFtpLogoutEachTime, _
           Config.ActiveFtpBufferLength)
        Me.activeXllWorkerPendingLimitTicks = Config.ActiveFtpWorkerPendingLimitTicks

        Dim oPassiveChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(Me.oPassiveXllWorkerMessageSock, oPassiveChildSock)
        Me.oPassiveXllWorker = New FtpWorker( _
           sThreadName & "-PassiveXll", _
           oPassiveChildSock, _
           Config.FtpServerUri, _
           New NetworkCredential(Config.FtpUserName, Config.FtpPassword), _
           Config.PassiveFtpRequestLimitTicks, _
           Config.PassiveFtpLogoutLimitTicks, _
           Config.PassiveFtpTransferStallLimitTicks, _
           Config.PassiveFtpUsePassiveMode, _
           Config.PassiveFtpLogoutEachTime, _
           Config.PassiveFtpBufferLength)
        Me.passiveXllWorkerPendingLimitTicks = Config.PassiveFtpWorkerPendingLimitTicks

        Me.sPermittedPathInFtp = Path.Combine(Config.ModelPathInFtp, selfEkCode.ToString(sDirNameFormat))
        Me.sPermittedPath = Utility.CombinePathWithVirtualPath(sFtpBasePath, sPermittedPathInFtp)

        Directory.CreateDirectory(sPermittedPath)

        If True Then
            Dim oBuilder As New StringBuilder()
            Dim oSelRows As DataRow() = oMachineDataTable.Select("MODEL_CODE = '" & Config.ModelSym & "' AND ADDRESS = '" & sAddr & "'")
            Dim sAreaCodeFieldKey As String = Config.ModelSym & "_AREA_CODE"
            Dim areaCodeExists As Boolean = Config.FieldNamesTypes.ContainsKey(sAreaCodeFieldKey)
            For idx As Integer = 0 To oSelRows.Length - 1
                For i As Integer = 0 To Config.MachineFileFieldNames.Length - 1
                    Dim sFieldName As String = Config.MachineFileFieldNames(i)
                    oBuilder.Append(oSelRows(idx)(sFieldName).ToString() & ",")
                Next i
                If areaCodeExists Then
                    oBuilder.AppendLine(oSelRows(idx)(sAreaCodeFieldKey).ToString())
                Else
                    oBuilder.AppendLine("0")
                End If
            Next idx
            Using sw As New StreamWriter(Path.Combine(sPermittedPath, "#Machine.csv"), False, Encoding.Default)
                sw.Write(oBuilder.ToString())
            End Using
        End If

        oTermCodes = New List(Of EkCode)
        If Config.TermModelSym <> "" Then
            Dim oBuilder As New StringBuilder()
            Dim oSelRows As DataRow() = oMachineDataTable.Select("MODEL_CODE = '" & Config.TermModelSym & "' AND MONITOR_ADDRESS = '" & sAddr & "'")
            Dim sAreaCodeFieldKey As String = Config.TermModelSym & "_AREA_CODE"
            For idx As Integer = 0 To oSelRows.Length - 1
                For i As Integer = 0 To Config.MachineFileFieldNames.Length - 1
                    Dim sFieldName As String = Config.MachineFileFieldNames(i)
                    oBuilder.Append(oSelRows(idx)(sFieldName).ToString() & ",")
                Next i
                oBuilder.AppendLine(oSelRows(idx)(sAreaCodeFieldKey).ToString())

                Dim code As EkCode
                code.RailSection = Integer.Parse(oSelRows(idx).Field(Of String)("RAIL_SECTION_CODE"))
                code.StationOrder = Integer.Parse(oSelRows(idx).Field(Of String)("STATION_ORDER_CODE"))
                code.Corner = oSelRows(idx).Field(Of Integer)("CORNER_CODE")
                code.Unit = oSelRows(idx).Field(Of Integer)("UNIT_NO")
                oTermCodes.Add(code)
            Next idx
            Using sw As New StreamWriter(Path.Combine(sPermittedPath, "#TermMachine.csv"), False, Encoding.Default)
                sw.Write(oBuilder.ToString())
            End Using
        End If

        Me.oParentMessageQueue = New LinkedList(Of InternalMessage)()
        Me.oParentMessageProcTimer = New TickTimer(0)

        Me.oStringExpander = New StringExpander( _
           oForm.ExtAppTargetQueue, _
           oParentMessageSock, _
           oParentMessageQueue, _
           AddressOf Me.PostponeParentMessages, _
           sPermittedPath)

        Me.oScenarioStartTimer = New TickTimer(0)
        Me.connectedByScenario = False
        Me.oScenarioEnv = New ScenarioEnv( _
           oTelegGene, _
           DirectCast(oTelegImporter, EkTelegramImporter), _
           selfIndex, _
           selfEkCode, _
           oTermCodes, _
           sPermittedPathInFtp, _
           sPermittedPath, _
           AddressOf Me.ConnectForScenario, _
           AddressOf Me.DisconnectForScenario, _
           AddressOf Me.SendReplyTelegram, _
           AddressOf Me.SendNakTelegram, _
           AddressOf Me.RegisterActiveOne, _
           AddressOf Me.RegisterActiveUll, _
           AddressOf Me.RegisterTimer, _
           AddressOf Me.UnregisterTimer, _
           oStringExpander, _
           oForm.AssemblyManager)
    End Sub
#End Region

#Region "プロパティ"
    'NOTE: このプロパティは、親スレッドにおいて参照が行われる。
    Public Property LineStatus() As LineStatus
        'NOTE: InterlockedクラスのReadメソッドに関するmsdnの解説を読むと、
        '32ビット変数からの値の読み取りはInterlockedクラスのメソッドを使う
        'までもなく不可分である（全体を読み取るためのバスオペレーションが、
        '他のコアによるバスオペレーションに分断されることがない）ことが
        '保証されているようにも見え、実際にIntegerを引数とするReadメソッドは
        '用意されていない。ここでは、とりあえずInterlocked.Add（LOCK: XADD?）
        'を代用しているが、一般的に考えて、Interlockedクラスに
        '「Readメモリバリア+単独の32bitロード命令」で実装された（実質的な
        'VolatileRead相当の）Readメソッドが用意されるべきであり、もし、
        'それが用意されたら、それに変更した方がよい。なお、VolatileReadを
        '使用しないのは、ServerTelegrapherで決めた方針である。方針の詳細は
        'ServerTelegrapher.LastPulseTickのコメントを参照。
        Get
            Return DirectCast(Interlocked.Add(_LineStatus, 0), LineStatus)
        End Get

        Protected Set(ByVal status As LineStatus)
            Interlocked.Exchange(_LineStatus, status)
        End Set
    End Property

    'NOTE: このプロパティは、親スレッドにおいて参照が行われる。
    Public ReadOnly Property ScenarioStatus() As ScenarioStatus
        'NOTE: LineStatusの実装NOTEを参照。
        Get
            Return oScenarioEnv.Status
        End Get
    End Property
#End Region

#Region "イベント処理メソッド"
    Protected Overrides Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oParentMessageProcTimer Then
            Return ProcParentMessagesInQueue()
        End If

        If oTimer Is oScenarioStartTimer Then
            Return ProcOnScenarioStartTime()
        End If

        If oScenarioEnv.ProcOnTimeout(oTimer) = True Then
            Return True
        End If

        Return MyBase.ProcOnTimeout(oTimer)
    End Function

    Protected Overridable Function ProcOnScenarioStartTime() As Boolean
        Log.Info("Scenario start time comes.")

        oScenarioEnv.StartRunning()
        Return True
    End Function

    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If oParentMessageQueue.Count <> 0 Then
            oParentMessageQueue.AddLast(oRcvMsg)
            Return ProcParentMessagesInQueue()
        Else
            Return ProcParentMessage(oRcvMsg)
        End If
    End Function

    Protected Function ProcParentMessagesInQueue() As Boolean
        While oParentMessageQueue.Count <> 0
            Dim oParentMessage As InternalMessage = oParentMessageQueue.First.Value
            oParentMessageQueue.RemoveFirst()
            If ProcParentMessage(oParentMessage) = False Then
                Return False
            End If
        End While
        Return True
    End Function

    Protected Overridable Function ProcParentMessage(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case MyInternalMessageKind.ConnectRequest
                Return ProcOnConnectRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ScenarioStartRequest
                Return ProcOnScenarioStartRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ScenarioStopRequest
                Return ProcOnScenarioStopRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ComStartExecRequest
                Return ProcOnComStartExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.TimeDataGetExecRequest
                Return ProcOnTimeDataGetExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ActiveOneExecRequest
                Return ProcOnActiveOneExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ActiveUllExecRequest
                Return ProcOnActiveUllExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.AppFuncEndNotice
                Log.Warn("Response of past AppFuncMessage received.")
                Return True
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
    End Function

    Protected Overrides Function ProcOnTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim capRcvTelegs As Boolean
        SyncLock oForm.UiState
            capRcvTelegs = oForm.UiState.CapRcvTelegs
        End SyncLock

        If capRcvTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    DirectCast(oRcvTeleg, EkTelegram).WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return MyBase.ProcOnTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnConnectRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Connect requested by manager.")

        If curState <> State.NoConnection Then
            Log.Info("I have already connected.")
            Return True
        End If

        Dim sServerName As String = Config.ServerIpAddr & "." & Config.IpPortForTelegConnection.ToString()
        Log.Info("Connecting to [" & sServerName & "]...")
        LineStatus = LineStatus.ConnectWaiting
        Dim oNewSock As Socket
        Try
            oNewSock = SockUtil.Connect(Config.ServerIpAddr, Config.IpPortForTelegConnection)
        Catch ex As OPMGException
            Log.Error("Exception caught.", ex)
            LineStatus = LineStatus.ConnectFailed
            Return True
        End Try
        Dim oLocalEndPoint As IPEndPoint = DirectCast(oNewSock.LocalEndPoint, IPEndPoint)
        Dim sClientName As String = oLocalEndPoint.Address.ToString() & "." & oLocalEndPoint.Port.ToString()
        Log.Info("Connection established by [" & sClientName & "] to [" & sServerName & "].")
        LineStatus = LineStatus.Connected
        Connect(oNewSock)

        Dim systemTick As Long = TickTimer.GetSystemTick()
        LastPulseTick = systemTick
        RegisterTimer(oWatchdogTimer, systemTick)
        Return True
    End Function

    Protected Overridable Function ProcOnScenarioStartRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ScenarioStart requested by manager.")

        If oScenarioEnv.Status = ScenarioStatus.Running OrElse oScenarioEnv.Status = ScenarioStatus.Loaded Then
            Log.Warn("Current scenario will now terminate.")
            If oScenarioEnv.Status = ScenarioStatus.Loaded Then
                UnregisterTimer(oScenarioStartTimer)
            End If
            oScenarioEnv.StopRunning()
        End If

        Dim oExt As ScenarioStartRequestExtendPart _
           = ScenarioStartRequest.Parse(oRcvMsg).ExtendPart

        Dim sFilePath As String = oExt.ScenarioFilePath
        Try
            sFilePath = sFilePath.Replace("%%", vbLf)
            sFilePath = MyUtility.ReplaceMachineIndex(sFilePath, selfIndex)
            sFilePath = selfEkCode.ToString(sFilePath).Replace(ControlChars.Lf, "%"c)
            sFilePath = oStringExpander.Expand(sFilePath, Nothing, 0)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return True
        End Try

        Log.Info("Loading a scenario from [" & sFilePath & "]...")
        Try
            oScenarioEnv.Load(sFilePath)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return True
        End Try

        Dim now As DateTime = DateTime.Now
        If oExt.StartTimeSpecified AndAlso oExt.StartTime > now Then
            Log.Info("I'll start it at " &  oExt.StartTime.ToString("yyyy/MM/dd HH:mm:ss") & "...")
            oScenarioStartTimer.Renew(CLng(oExt.StartTime.Subtract(now).TotalMilliseconds))
            RegisterTimer(oScenarioStartTimer, TickTimer.GetSystemTick())
        Else
            Log.Info("Starting the scenario...")
            oScenarioEnv.StartRunning()
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnScenarioStopRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ScenarioStop requested by manager.")

        If oScenarioEnv.Status <> ScenarioStatus.Running AndAlso oScenarioEnv.Status <> ScenarioStatus.Loaded Then
            Log.Info("I am not running any scenario right now.")
        Else
            If oScenarioEnv.Status = ScenarioStatus.Loaded Then
                UnregisterTimer(oScenarioStartTimer)
            End If
            oScenarioEnv.StopRunning()
            Log.Info("The scenario stopped.")
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnComStartExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ComStartExec requested by manager.")

        Dim sSeqName As String = "ComStart #" & traceNumberForComStart.ToString()
        UpdateTraceNumberForComStart()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New EkComStartReqTelegram( _
           oTelegGene, _
           ObjCodeForComStartIn(Config.AplProtocol),
           Config.ComStartReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        If LineStatus = LineStatus.Connected Then
            LineStatus = LineStatus.ComStartWaiting
        End If
        Return True
    End Function

    Protected Overridable Function ProcOnTimeDataGetExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("TimeDataGetExec requested by manager.")

        Dim sSeqName As String = "TimeDataGet #" & traceNumberForTimeDataGet.ToString()
        UpdateTraceNumberForTimeDataGet()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New EkTimeDataGetReqTelegram( _
           oTelegGene, _
           ObjCodeForTimeDataGetIn(Config.AplProtocol),
           Config.TimeDataGetReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        If LineStatus = LineStatus.Connected Then
            LineStatus = LineStatus.ComStartWaiting
        End If
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ActiveOneExec requested by manager.")

        Dim oExt As ActiveOneExecRequestExtendPart _
           = ActiveOneExecRequest.Parse(oRcvMsg).ExtendPart

        Dim s As String = oExt.ApplyFilePath
        Try
            If s.Contains("%T") Then
                s = s.Replace("%%", vbLf) _
                     .Replace("%T", vbCr)
                s = MyUtility.ReplaceMachineIndex(s, selfIndex)
                s = selfEkCode.ToString(s).Replace(ControlChars.Cr, "%"c)
                For i As Integer = 0 To oTermCodes.Count - 1
                    Dim sWildPath As String = oTermCodes(i).ToString(MyUtility.ReplaceMachineIndex(s, i)).Replace(ControlChars.Lf, "%"c)
                    sWildPath = oStringExpander.Expand(sWildPath, Nothing, 0)
                    Dim sWildName As String = Path.GetFileName(sWildPath)
                    If sWildName.IndexOf("?"c) <> -1 OrElse sWildName.IndexOf("*"c) <> -1 Then
                        Dim sPaths As String() = Directory.GetFiles(Path.GetDirectoryName(sWildPath), sWildName)
                        If sPaths.Length <> 0 Then
                            For Each sPath As String In sPaths
                                oExt.ApplyFilePath = sPath
                                ProcOnActiveOneExecRequestReceive(oExt)
                            Next sPath
                        Else
                            If oExt.ApplyFileMustExists Then
                                Log.Warn("There is no file matched with [" & sWildPath & "].")
                            Else
                                Log.Debug("There is no file matched with [" & sWildPath & "].")
                            End If
                        End If
                    Else
                        If File.Exists(sWildPath) Then
                            oExt.ApplyFilePath = sWildPath
                            ProcOnActiveOneExecRequestReceive(oExt)
                        Else
                            If oExt.ApplyFileMustExists Then
                                Log.Error("The file [" & sWildPath & "] not found.")
                            Else
                                Log.Debug("The file [" & sWildPath & "] not found.")
                            End If
                        End If
                    End If
                Next i
            Else
                s = s.Replace("%%", vbLf)
                s = MyUtility.ReplaceMachineIndex(s, selfIndex)
                Dim sWildPath As String = selfEkCode.ToString(s).Replace(ControlChars.Lf, "%"c)
                sWildPath = oStringExpander.Expand(sWildPath, Nothing, 0)
                Dim sWildName As String = Path.GetFileName(sWildPath)
                If sWildName.IndexOf("?"c) <> -1 OrElse sWildName.IndexOf("*"c) <> -1 Then
                    Dim sPaths As String() = Directory.GetFiles(Path.GetDirectoryName(sWildPath), sWildName)
                    If sPaths.Length <> 0 Then
                        For Each sPath As String In sPaths
                            oExt.ApplyFilePath = sPath
                            ProcOnActiveOneExecRequestReceive(oExt)
                        Next sPath
                    Else
                        If oExt.ApplyFileMustExists Then
                            Log.Warn("There is no file matched with [" & sWildPath & "].")
                        Else
                            Log.Debug("There is no file matched with [" & sWildPath & "].")
                        End If
                    End If
                Else
                    If File.Exists(sWildPath) Then
                        oExt.ApplyFilePath = sWildPath
                        ProcOnActiveOneExecRequestReceive(oExt)
                    Else
                        If oExt.ApplyFileMustExists Then
                            Log.Error("The file [" & sWildPath & "] not found.")
                        Else
                            Log.Debug("The file [" & sWildPath & "] not found.")
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
        End Try

        Return True
    End Function

    Protected Sub ProcOnActiveOneExecRequestReceive(ByVal oExt As ActiveOneExecRequestExtendPart)
        Dim oTeleg As EkDodgyTelegram
        Try
            Log.Debug("Loading telegram from [" & oExt.ApplyFilePath & "]...")
            Using oInputStream As New FileStream(oExt.ApplyFilePath, FileMode.Open, FileAccess.Read)
                oTeleg = DirectCast(oTelegImporter, EkTelegramImporter).GetTelegramFromStream(oInputStream)
            End Using
            If oTeleg Is Nothing Then Return
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return
        End Try

        Dim sSeqName As String = "ActiveOne #" & traceNumberForActiveOne.ToString()
        UpdateTraceNumberForActiveOne()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim sOriginalFilePath As String = Nothing
        If oExt.DeleteApplyFileIfCompleted Then
            sOriginalFilePath = oExt.ApplyFilePath
        End If

        Dim oReqTeleg As New EkAnonyReqTelegram(oTeleg, oExt.ReplyLimitTicks, sOriginalFilePath)
        RegisterActiveOne(oReqTeleg, oExt.RetryIntervalTicks, oExt.MaxRetryCountToForget + 1, oExt.MaxRetryCountToCare + 1, sSeqName)
    End Sub

    '能動的単発シーケンスが成功した場合
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        If oScenarioEnv.ProcOnActiveOneComplete(DirectCast(iReqTeleg, EkReqTelegram), DirectCast(iAckTeleg, EkTelegram)) = False Then
            Dim rtt As Type = iReqTeleg.GetType()
            Select Case True
                Case rtt Is GetType(EkComStartReqTelegram)
                    Log.Info("ComStart completed.")
                    enableActiveOneOrdering = Config.EnableActiveOneOrdering

                Case rtt Is GetType(EkTimeDataGetReqTelegram)
                    Log.Info("TimeDataGet completed.")
                    enableActiveOneOrdering = Config.EnableActiveOneOrdering

                Case Else
                    Log.Info("ActiveOne completed.")
                    Dim sOriginalFilePath As String = DirectCast(iReqTeleg, EkAnonyReqTelegram).OriginalFilePath
                    If sOriginalFilePath IsNot Nothing Then
                        Try
                            Log.Debug("Deleting the file [" & sOriginalFilePath & "]...")
                            File.Delete(sOriginalFilePath)
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
            End Select
        End If
    End Sub

    '能動的単発シーケンスで異常とみなすべきでないリトライオーバーが発生した場合
    'NOTE: 本クラスのGetRequirement()の実装上、このメソッドが呼ばれることはあり得ない。
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        If oScenarioEnv.ProcOnActiveOneRetryOverToForget(DirectCast(iReqTeleg, EkReqTelegram), DirectCast(iNakTeleg, EkNakTelegram)) = False Then
            Dim rtt As Type = iReqTeleg.GetType()
            Select Case True
                Case rtt Is GetType(EkComStartReqTelegram)
                    Log.Error("ComStart skipped.")
                    enableActiveOneOrdering = Config.EnableActiveOneOrdering

                Case rtt Is GetType(EkTimeDataGetReqTelegram)
                    Log.Error("TimeDataGet skipped.")
                    enableActiveOneOrdering = Config.EnableActiveOneOrdering

                Case Else
                    Log.Error("ActiveOne skipped.")
            End Select
        End If
    End Sub

    '能動的単発シーケンスで異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        If oScenarioEnv.ProcOnActiveOneRetryOverToCare(DirectCast(iReqTeleg, EkReqTelegram), DirectCast(iNakTeleg, EkNakTelegram)) = False Then
            Dim rtt As Type = iReqTeleg.GetType()
            Select Case True
                Case rtt Is GetType(EkComStartReqTelegram)
                    Log.Error("ComStart failed.")
                    enableActiveOneOrdering = Config.EnableActiveOneOrdering

                Case rtt Is GetType(EkTimeDataGetReqTelegram)
                    Log.Error("TimeDataGet failed.")
                    enableActiveOneOrdering = Config.EnableActiveOneOrdering

                Case Else
                    Log.Error("ActiveOne retry over.")
            End Select
        End If
    End Sub

    '能動的単発シーケンスの最中やキューイングされた能動的単発シーケンスの実施前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        If oScenarioEnv.ProcOnActiveOneAnonyError(DirectCast(iReqTeleg, EkReqTelegram)) = False Then
            Dim rtt As Type = iReqTeleg.GetType()
            Select Case True
                Case rtt Is GetType(EkComStartReqTelegram)
                    Log.Error("ComStart failed.")
                    enableActiveOneOrdering = Config.EnableActiveOneOrdering

                Case rtt Is GetType(EkTimeDataGetReqTelegram)
                    Log.Error("TimeDataGet failed.")
                    enableActiveOneOrdering = Config.EnableActiveOneOrdering

                Case Else
                    Log.Error("ActiveOne failed.")
            End Select
        End If
    End Sub

    Protected Overridable Function ProcOnActiveUllExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ActiveUllExec requested by manager.")

        Dim oExt As ActiveUllExecRequestExtendPart _
           = ActiveUllExecRequest.Parse(oRcvMsg).ExtendPart

        Dim sTransFile As String = oExt.TransferFileName
        Dim sApplyFile As String = oExt.ApplyFilePath
        Try
            If sTransFile.Contains("%T") OrElse sApplyFile.Contains("%T") Then
                sTransFile = sTransFile.Replace("%%", vbLf) _
                                       .Replace("%T", vbCr)
                sTransFile = MyUtility.ReplaceMachineIndex(sTransFile, selfIndex)
                sTransFile = selfEkCode.ToString(sTransFile).Replace(ControlChars.Cr, "%"c)

                sApplyFile = sApplyFile.Replace("%%", vbLf) _
                                       .Replace("%T", vbCr)
                sApplyFile = MyUtility.ReplaceMachineIndex(sApplyFile, selfIndex)
                sApplyFile = selfEkCode.ToString(sApplyFile).Replace(ControlChars.Cr, "%"c)

                For i As Integer = 0 To oTermCodes.Count - 1
                    Dim sTransFileOfT As String = oTermCodes(i).ToString(MyUtility.ReplaceMachineIndex(sTransFile, i)).Replace(ControlChars.Lf, "%"c)
                    oExt.TransferFileName = oStringExpander.Expand(sTransFileOfT, Nothing, 0)

                    Dim sWildPath As String = oTermCodes(i).ToString(MyUtility.ReplaceMachineIndex(sApplyFile, i)).Replace(ControlChars.Lf, "%"c)
                    sWildPath = oStringExpander.Expand(sWildPath, Nothing, 0)
                    Dim sWildName As String = Path.GetFileName(sWildPath)
                    If sWildName.IndexOf("?"c) <> -1 OrElse sWildName.IndexOf("*"c) <> -1 Then
                        Dim sPaths As String() = Directory.GetFiles(Path.GetDirectoryName(sWildPath), sWildName)
                        If sPaths.Length <> 0 Then
                            For Each sPath As String In sPaths
                                oExt.ApplyFilePath = sPath
                                ProcOnActiveUllExecRequestReceive(oExt)
                            Next sPath
                        Else
                            If oExt.ApplyFileMustExists Then
                                Log.Warn("There is no file matched with [" & sWildPath & "].")
                            Else
                                Log.Debug("There is no file matched with [" & sWildPath & "].")
                            End If
                        End If
                    Else
                        If File.Exists(sWildPath) Then
                            oExt.ApplyFilePath = sWildPath
                            ProcOnActiveUllExecRequestReceive(oExt)
                        Else
                            If oExt.ApplyFileMustExists Then
                                Log.Error("The file [" & sWildPath & "] not found.")
                            Else
                                Log.Debug("The file [" & sWildPath & "] not found.")
                            End If
                        End If
                    End If
                Next i
            Else
                sTransFile = sTransFile.Replace("%%", vbLf)
                sTransFile = MyUtility.ReplaceMachineIndex(sTransFile, selfIndex)
                sTransFile = selfEkCode.ToString(sTransFile).Replace(ControlChars.Lf, "%"c)
                oExt.TransferFileName = oStringExpander.Expand(sTransFile, Nothing, 0)

                sApplyFile = sApplyFile.Replace("%%", vbLf)
                sApplyFile = MyUtility.ReplaceMachineIndex(sApplyFile, selfIndex)
                Dim sWildPath As String = selfEkCode.ToString(sApplyFile).Replace(ControlChars.Lf, "%"c)
                sWildPath = oStringExpander.Expand(sWildPath, Nothing, 0)
                Dim sWildName As String = Path.GetFileName(sWildPath)
                If sWildName.IndexOf("?"c) <> -1 OrElse sWildName.IndexOf("*"c) <> -1 Then
                    Dim sPaths As String() = Directory.GetFiles(Path.GetDirectoryName(sWildPath), sWildName)
                    If sPaths.Length <> 0 Then
                        For Each sPath As String In sPaths
                            oExt.ApplyFilePath = sPath
                            ProcOnActiveUllExecRequestReceive(oExt)
                        Next sPath
                    Else
                        If oExt.ApplyFileMustExists Then
                            Log.Warn("There is no file matched with [" & sWildPath & "].")
                        Else
                            Log.Debug("There is no file matched with [" & sWildPath & "].")
                        End If
                    End If
                Else
                    If File.Exists(sWildPath) Then
                        oExt.ApplyFilePath = sWildPath
                        ProcOnActiveUllExecRequestReceive(oExt)
                    Else
                        If oExt.ApplyFileMustExists Then
                            Log.Error("The file [" & sWildPath & "] not found.")
                        Else
                            Log.Debug("The file [" & sWildPath & "] not found.")
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
        End Try

        Return True
    End Function

    Protected Sub ProcOnActiveUllExecRequestReceive(ByVal oExt As ActiveUllExecRequestExtendPart)
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram
        Try
            Dim sTransferFilePath As String = Path.Combine(sPermittedPath, oExt.TransferFileName)
            Log.Debug("Copying file from [" & oExt.ApplyFilePath & "] to [" & sTransferFilePath & "]...")
            MyUtility.CopyFileIfNeeded(oExt.ApplyFilePath, sTransferFilePath, True)

            Dim sTransferFilePathInFtp As String = Path.Combine(sPermittedPathInFtp, oExt.TransferFileName)
            If Not MyUtility.IsAsciiString(sTransferFilePathInFtp) OrElse sTransferFilePathInFtp.Length > 80 Then
                Throw New FormatException("The file name may be dangerous to EkClientDrivenUllReqTelegram.")
            End If

            Dim sOriginalFilePath As String = Nothing
            If oExt.DeleteApplyFileIfCompleted Then
                sOriginalFilePath = oExt.ApplyFilePath
            End If

            oXllReqTeleg = New EkClientDrivenUllReqTelegram( _
               oTelegGene, _
               oExt.ObjCode, _
               ContinueCode.Start, _
               sTransferFilePathInFtp, _
               oExt.ApplyFileHashValue, _
               oExt.TransferLimitTicks, _
               oExt.ReplyLimitTicksOnStart, _
               oExt.ReplyLimitTicksOnFinish, _
               sOriginalFilePath)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return
        End Try

        RegisterActiveUll(oXllReqTeleg, oExt.RetryIntervalTicks, oExt.MaxRetryCountToForget + 1, oExt.MaxRetryCountToCare + 1)
    End Sub

    '能動的ULLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
    Protected Overrides Function CreateActiveUllContinuousReqTelegram(ByVal oXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)

        Dim oNewUllReqTeleg As EkClientDrivenUllReqTelegram = oScenarioEnv.CreateActiveUllContinuousReqTelegram(oRealUllReqTeleg, cc)
        If oNewUllReqTeleg IsNot Nothing Then
            Return oNewUllReqTeleg
        End If

        Return oRealUllReqTeleg.CreateContinuousTelegram(cc, 0, oRealUllReqTeleg.AltReplyLimitTicks, 0, oRealUllReqTeleg.OriginalFilePath)
    End Function

    Protected Overrides Sub ProcOnActiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)

        If oScenarioEnv.ProcOnActiveUllComplete(oRealUllReqTeleg) = False Then
            Log.Info("ActiveUll completed.")
            If oRealUllReqTeleg.OriginalFilePath IsNot Nothing Then
                Try
                    Log.Debug("Deleting the file [" & oRealUllReqTeleg.OriginalFilePath & "]...")
                    File.Delete(oRealUllReqTeleg.OriginalFilePath)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                End Try
            End If
        End If

        If Config.DeleteActiveUllTmpFileOnSeqEnd Then
            Try
                File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oRealUllReqTeleg.FileName)))
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If
    End Sub

    Protected Overrides Sub ProcOnActiveUllTransferError(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)

        If oScenarioEnv.ProcOnActiveUllAnonyError(oRealUllReqTeleg) = False Then
            Log.Error("ActiveUll failed by transfer error.")
        End If

        If Config.DeleteActiveUllTmpFileOnSeqEnd Then
            Try
                File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oRealUllReqTeleg.FileName)))
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If
    End Sub

    Protected Overrides Sub ProcOnActiveUllFinalizeError(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)

        If oScenarioEnv.ProcOnActiveUllAnonyError(oRealUllReqTeleg) = False Then
            Log.Error("ActiveUll failed by finalize error.")
        End If

        If Config.DeleteActiveUllTmpFileOnSeqEnd Then
            Try
                File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oRealUllReqTeleg.FileName)))
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If
    End Sub

    'NOTE: 本クラスのGetRequirement()の実装上、このメソッドが呼ばれることはあり得ない。
    Protected Overrides Sub ProcOnActiveUllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)

        If oScenarioEnv.ProcOnActiveUllRetryOverToForget(oRealUllReqTeleg) = False Then
            Log.Fatal("ActiveUll failed by surprising retry over.")
        End If

        If Config.DeleteActiveUllTmpFileOnSeqEnd Then
            Try
                File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oRealUllReqTeleg.FileName)))
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If
    End Sub

    Protected Overrides Sub ProcOnActiveUllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)

        If oScenarioEnv.ProcOnActiveUllRetryOverToCare(oRealUllReqTeleg) = False Then
            Log.Error("ActiveUll failed by retry over.")
        End If

        If Config.DeleteActiveUllTmpFileOnSeqEnd Then
            Try
                File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oRealUllReqTeleg.FileName)))
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If
    End Sub

    Protected Overrides Sub ProcOnActiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)

        If oScenarioEnv.ProcOnActiveUllAnonyError(oRealUllReqTeleg) = False Then
            Log.Error("ActiveUll failed.")
        End If

        If Config.DeleteActiveUllTmpFileOnSeqEnd Then
            Try
                File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oRealUllReqTeleg.FileName)))
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If
    End Sub

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)
        Select Case oRcvTeleg.SubCmdCode
            Case EkSubCmdCode.Get
                Dim isObjCodeRegistered As Boolean = False
                Dim sApplyFilePath As String = Nothing
                SyncLock oForm.UiState
                    If oForm.UiState.PassiveGetObjCodesApplyFiles.ContainsKey(CByte(oRcvTeleg.ObjCode)) Then
                        isObjCodeRegistered = True
                        sApplyFilePath = oForm.UiState.PassiveGetObjCodesApplyFiles(CByte(oRcvTeleg.ObjCode))
                    End If
                End SyncLock
                If isObjCodeRegistered Then
                    If sApplyFilePath IsNot Nothing Then
                        Try
                            sApplyFilePath = sApplyFilePath.Replace("%%", vbLf)
                            sApplyFilePath = MyUtility.ReplaceMachineIndex(sApplyFilePath, selfIndex)
                            sApplyFilePath = selfEkCode.ToString(sApplyFilePath).Replace(ControlChars.Lf, "%"c)
                            sApplyFilePath = oStringExpander.Expand(sApplyFilePath, Nothing, 0)
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                            Exit Select
                        End Try
                    End If
                    Return ProcOnPassiveGetReqTelegramReceive(oRcvTeleg, sApplyFilePath)
                End If

            Case EkSubCmdCode.Post
                Dim isObjCodeRegistered As Boolean = False
                SyncLock oForm.UiState
                    isObjCodeRegistered = oForm.UiState.PassivePostObjCodes.ContainsKey(CByte(oRcvTeleg.ObjCode))
                End SyncLock
                If isObjCodeRegistered Then
                    Return ProcOnPassivePostReqTelegramReceive(oRcvTeleg)
                End If
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnPassiveGetReqTelegramReceive(ByVal iRcvTeleg As ITelegram, ByVal sApplyFilePath As String) As Boolean
        Dim oRcvTeleg As New EkByteArrayGetReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("ByteArrayGet REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("ByteArrayGet REQ received.")

        If oScenarioEnv.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg) = True Then
            Return True
        End If

        Dim forceNakCause As NakCauseCode = EkNakCauseCode.None
        SyncLock oForm.UiState
            If oForm.UiState.PassiveGetForceReplyNak Then
                forceNakCause = New EkNakCauseCode(oForm.UiState.PassiveGetNakCauseNumber, oForm.UiState.PassiveGetNakCauseText)
            End If
        End SyncLock
        If forceNakCause <> EkNakCauseCode.None Then
            If SendNakTelegram(forceNakCause, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        If sApplyFilePath Is Nothing Then
            Log.Warn("Applied file name is invalid.")
            If SendNakTelegram(EkNakCauseCode.NoData, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        If Not File.Exists(sApplyFilePath) Then
            Log.Warn("The file [" & sApplyFilePath & "] not found.")
            If SendNakTelegram(EkNakCauseCode.NoData, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        Dim aReplyBytes As Byte()
        Dim retryCount As Integer = 0
        Do
            Log.Info("Loading reply data from [" & sApplyFilePath & "]...")
            Try
                Using oInputStream As New FileStream(sApplyFilePath, FileMode.Open, FileAccess.Read)
                    'ファイルのレングスを取得する。
                    Dim len As Integer = CInt(oInputStream.Length)
                    'ファイルを読み込む。
                    aReplyBytes = New Byte(len - 1) {}
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(aReplyBytes, pos, len - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
                Exit Do
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                If ex.GetType() Is GetType(IOException) Then
                    'NOTE: 別のプロセスが排他的に（読み取り禁止で）sApplyFilePathのファイルを
                    '開いでいる場合とみなす。
                    If retryCount >= 3 Then
                        If SendNakTelegram(EkNakCauseCode.Busy, oRcvTeleg) = False Then
                            Disconnect()
                        End If
                        Return True
                    End If
                    Thread.Sleep(1000)
                    retryCount += 1
                Else
                    'exがDirectoryNotFoundExceptionやFileNotFoundExceptionの場合である。
                    'NOTE: 先のFile.ExistsからNew FileStreamまでの間に
                    'ファイルが移動や削除されたケースとみなす。
                    If SendNakTelegram(EkNakCauseCode.NoData, oRcvTeleg) = False Then
                        Disconnect()
                    End If
                    Return True
                End If
            End Try
        Loop

        Dim oReplyTeleg As EkByteArrayGetAckTelegram = oRcvTeleg.CreateAckTelegram(aReplyBytes)
        Log.Info("Sending ByteArrayGet ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnPassivePostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkByteArrayPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("ByteArrayPost REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("ByteArrayPost REQ received.")

        If oScenarioEnv.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg) = True Then
            Return True
        End If

        Dim forceNakCause As NakCauseCode = EkNakCauseCode.None
        SyncLock oForm.UiState
            If oForm.UiState.PassivePostForceReplyNak Then
                forceNakCause = New EkNakCauseCode(oForm.UiState.PassivePostNakCauseNumber, oForm.UiState.PassivePostNakCauseText)
            End If
        End SyncLock
        If forceNakCause <> EkNakCauseCode.None Then
            If SendNakTelegram(forceNakCause, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        Dim oReplyTeleg As EkByteArrayPostAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ByteArrayPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    'ヘッダ部の内容が受動的DLLのREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsPassiveDllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get Then
            SyncLock oForm.UiState
                Return oForm.UiState.PassiveDllObjCodes.ContainsKey(CByte(oTeleg.ObjCode))
            End SyncLock
        End If

        Return False
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsPassiveDllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)

        Dim transferLimitTicks As Integer
        SyncLock oForm.UiState
            transferLimitTicks = oForm.UiState.PassiveDllTransferLimitTicks
        End SyncLock

        'TODO: 現在のプロトコルにおける受動的DLLシーケンスのREQ電文は
        'データ種別に関係なくEkMasProDllReqTelegramであるが、そうでなくなった
        '場合のことを想定するなら、oForm.UiState.SomethingForPassiveDllObjCode
        'には、電文の型を格納しておいた方がよいかもしれない。
        Return New EkMasProDllReqTelegram(oTeleg, transferLimitTicks)
    End Function

    '受動的DLLの準備（予告されたファイルの受け入れ確認）を行うメソッド
    Protected Overrides Function PrepareToStartPassiveDll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)

        Dim nakCause As NakCauseCode = oScenarioEnv.PrepareToStartPassiveDll(oXllReqTeleg)
        If nakCause IsNot Nothing Then
            Return nakCause
        End If

        SyncLock oForm.UiState
            If oForm.UiState.PassiveDllForceReplyNak Then
                Return New EkNakCauseCode(oForm.UiState.PassiveDllNakCauseNumber, oForm.UiState.PassiveDllNakCauseText)
            End If
        End SyncLock

        'NOTE: 事前にチェックしてあるため、iXllReqTeleg.DataFileName等はパスとして無害である。
        Log.Info("Starting PassiveDll of the files [" & Path.GetFileName(oXllReqTeleg.DataFileName) & "] [" & Path.GetFileName(oXllReqTeleg.ListFileName) & "]...")
        Return EkNakCauseCode.None
    End Function

    '受動的DLLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
    Protected Overrides Function CreatePassiveDllContinuousReqTelegram(ByVal iXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
                Dim oNewXllReqTeleg As EkMasProDllReqTelegram = oScenarioEnv.CreatePassiveDllContinuousReqTelegram(oXllReqTeleg, cc)
                If oNewXllReqTeleg Is Nothing Then
                    Dim replyLimitTicks As Integer
                    Dim resultantVersionOfSlot1 As Integer
                    Dim resultantVersionOfSlot2 As Integer
                    Dim resultantFlagOfFull As Integer
                    SyncLock oForm.UiState
                        If cc = ContinueCode.Finish AndAlso oForm.UiState.PassiveDllSimulateStoring Then
                            cc = ContinueCode.FinishWithoutStoring
                        End If
                        replyLimitTicks = oForm.UiState.PassiveDllReplyLimitTicks
                        resultantVersionOfSlot1 = oForm.UiState.PassiveDllResultantVersionOfSlot1
                        resultantVersionOfSlot2 = oForm.UiState.PassiveDllResultantVersionOfSlot2
                        resultantFlagOfFull = oForm.UiState.PassiveDllResultantFlagOfFull
                    End SyncLock
                    oNewXllReqTeleg = oXllReqTeleg.CreateContinuousTelegram(cc, resultantVersionOfSlot1, resultantVersionOfSlot2, resultantFlagOfFull, 0, replyLimitTicks)
                End If
                Return oNewXllReqTeleg

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select

        Return Nothing
    End Function

    Protected Overrides Sub ProcOnPassiveDllComplete(ByVal iXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)

                If oScenarioEnv.ProcOnPassiveDllComplete(oXllReqTeleg) = False Then
                    Log.Info("PassiveDll completed.")
                End If

                If Config.DeletePassiveDllTmpFileOnSeqEnd Then
                    If oXllReqTeleg.DataFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.DataFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                    If oXllReqTeleg.ListFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.ListFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    Protected Overrides Sub ProcOnPassiveDllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)

                If oScenarioEnv.ProcOnPassiveDllAnonyError(oXllReqTeleg) = False Then
                    Log.Error("PassiveDll failed by hash value error.")
                End If

                If Config.DeletePassiveDllTmpFileOnSeqEnd Then
                    If oXllReqTeleg.DataFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.DataFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                    If oXllReqTeleg.ListFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.ListFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    Protected Overrides Sub ProcOnPassiveDllTransferError(ByVal iXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)

                If oScenarioEnv.ProcOnPassiveDllAnonyError(oXllReqTeleg) = False Then
                    Log.Error("PassiveDll failed by transfer error.")
                End If

                If Config.DeletePassiveDllTmpFileOnSeqEnd Then
                    If oXllReqTeleg.DataFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.DataFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                    If oXllReqTeleg.ListFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.ListFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    Protected Overrides Sub ProcOnPassiveDllFinalizeError(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)

                If oScenarioEnv.ProcOnPassiveDllAnonyError(oXllReqTeleg) = False Then
                    Log.Error("PassiveDll failed by finalize error.")
                End If

                If Config.DeletePassiveDllTmpFileOnSeqEnd Then
                    If oXllReqTeleg.DataFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.DataFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                    If oXllReqTeleg.ListFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.ListFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    Protected Overrides Sub ProcOnPassiveDllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)

                If oScenarioEnv.ProcOnPassiveDllAnonyError(oXllReqTeleg) = False Then
                    Log.Error("PassiveDll failed.")
                End If

                If Config.DeletePassiveDllTmpFileOnSeqEnd Then
                    If oXllReqTeleg.DataFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.DataFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                    If oXllReqTeleg.ListFileName.Length <> 0 Then
                        Try
                            File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.ListFileName)))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    'ヘッダ部の内容が受動的ULLのREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsPassiveUllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get Then
            SyncLock oForm.UiState
                Return oForm.UiState.PassiveUllObjCodesApplyFiles.ContainsKey(CByte(oTeleg.ObjCode))
            End SyncLock
        End If

        Return False
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsPassiveUllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)

        Dim transferLimitTicks As Integer
        SyncLock oForm.UiState
            transferLimitTicks = oForm.UiState.PassiveUllTransferLimitTicks
        End SyncLock

        'TODO: 現在のプロトコルにおける受動的ULLシーケンスのREQ電文は
        'データ種別に関係なくEkServerDrivenUllReqTelegramであるが、そうでなくなった
        '場合のことを想定するなら、oForm.UiState.TypeForPassiveUllObjCodeのような
        'ところに、電文の型を格納しておいた方がよいかもしれない。
        Return New EkServerDrivenUllReqTelegram(oTeleg, transferLimitTicks)
    End Function

    '受動的ULLの準備（指定されたファイルの用意）を行うメソッド
    Protected Overrides Function PrepareToStartPassiveUll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)

                Dim nakCause As NakCauseCode = oScenarioEnv.PrepareToStartPassiveUll(oXllReqTeleg)
                If nakCause IsNot Nothing Then
                    Return nakCause
                End If

                SyncLock oForm.UiState
                    If oForm.UiState.PassiveUllForceReplyNak Then
                        Return New EkNakCauseCode(oForm.UiState.PassiveUllNakCauseNumber, oForm.UiState.PassiveUllNakCauseText)
                    End If
                End SyncLock

                Dim isObjCodeRegistered As Boolean = False
                Dim sApplyFilePath As String = Nothing
                SyncLock oForm.UiState
                    'NOTE: ロックを解除していた間に変更されている可能性もあるので、
                    'PassiveUllObjCodesApplyFilesに登録されているか再度チェックを
                    '行うことにしている。
                    If oForm.UiState.PassiveUllObjCodesApplyFiles.ContainsKey(CByte(oXllReqTeleg.ObjCode)) Then
                        isObjCodeRegistered = True
                        sApplyFilePath = oForm.UiState.PassiveUllObjCodesApplyFiles(CByte(oXllReqTeleg.ObjCode))
                    End If
                End SyncLock

                If Not isObjCodeRegistered Then
                    Log.Warn("Setting was changed during a sequence.")
                    Return EkNakCauseCode.NoData 'TODO: 微妙
                End If

                If sApplyFilePath Is Nothing Then
                    Log.Warn("Applied file name is invalid.")
                    Return EkNakCauseCode.NoData
                End If

                Try
                    sApplyFilePath = sApplyFilePath.Replace("%%", vbLf)
                    sApplyFilePath = MyUtility.ReplaceMachineIndex(sApplyFilePath, selfIndex)
                    sApplyFilePath = selfEkCode.ToString(sApplyFilePath).Replace(ControlChars.Lf, "%"c)
                    sApplyFilePath = oStringExpander.Expand(sApplyFilePath, Nothing, 0)

                    If Not File.Exists(sApplyFilePath) Then
                        Log.Warn("The file [" & sApplyFilePath & "] not found.")
                        Return EkNakCauseCode.NoData
                    End If
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Return EkNakCauseCode.NoData
                End Try

                'NOTE: 相手装置の不具合がみつかりやすいよう、oXllReqTeleg.FileNameが
                'ObjCodeと整合していない場合に、警告くらいは出してもよいと思われる。
                'しかし、その警告を頼りに試験をするには、このシミュレータの試験も
                '入念に行うべきであり、本末転倒であるため、やめておく。
                'NOTE: 事前にチェックしてあるため、oXllReqTeleg.FileNameはパスとして無害である。
                Dim sTransferFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
                Dim retryCount As Integer = 0
                Do
                    Try
                        Dim sTransferFilePath As String = Path.Combine(sPermittedPath, sTransferFileName)
                        Log.Debug("Copying file from [" & sApplyFilePath & "] to [" & sTransferFilePath & "]...")
                        MyUtility.CopyFileIfNeeded(sApplyFilePath, sTransferFilePath, True)
                        Exit Do
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        If ex.GetType() Is GetType(IOException) Then
                            'NOTE: 別のプロセスが排他的に（読み取り禁止で）sApplyFilePathのファイルを
                            '開いでいる場合とみなす。
                            If retryCount >= 3 Then Return EkNakCauseCode.Busy
                            Thread.Sleep(1000)
                            retryCount += 1
                        Else
                            'exがDirectoryNotFoundExceptionやFileNotFoundExceptionの場合である。
                            'NOTE: 先のFile.ExistsからCopyFileIfNeededまでの間に
                            'ファイルが移動や削除されたケースとみなす。
                            Return EkNakCauseCode.NoData
                        End If
                    End Try
                Loop

                Log.Info("Starting PassiveUll of the file [" & sTransferFileName & "]...")
                Return EkNakCauseCode.None

            Case Else
                Debug.Fail("This case is impermissible.")
                Return EkNakCauseCode.NotPermit
        End Select
    End Function

    '受動的ULLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
    Protected Overrides Function CreatePassiveUllContinuousReqTelegram(ByVal iXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)

                Dim oNewUllReqTeleg As EkServerDrivenUllReqTelegram = oScenarioEnv.CreatePassiveUllContinuousReqTelegram(oXllReqTeleg, cc)
                If oNewUllReqTeleg IsNot Nothing Then
                    Return oNewUllReqTeleg
                End If

                Dim replyLimitTicks As Integer
                SyncLock oForm.UiState
                    replyLimitTicks = oForm.UiState.PassiveUllReplyLimitTicks
                End SyncLock
                Return oXllReqTeleg.CreateContinuousTelegram(cc, 0, replyLimitTicks)

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select

        Return Nothing
    End Function

    Protected Overrides Sub ProcOnPassiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iAckTeleg As IXllTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)

                If oScenarioEnv.ProcOnPassiveUllComplete(oXllReqTeleg) = False Then
                    Log.Info("PassiveUll completed.")
                End If

                If Config.DeletePassiveUllTmpFileOnSeqEnd Then
                    Try
                        File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.FileName)))
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                    End Try
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    Protected Overrides Sub ProcOnPassiveUllTransferError(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iAckTeleg As IXllTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)

                If oScenarioEnv.ProcOnPassiveUllAnonyError(oXllReqTeleg) = False Then
                    Log.Error("PassiveUll failed by transfer error.")
                End If

                If Config.DeletePassiveUllTmpFileOnSeqEnd Then
                    Try
                        File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.FileName)))
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                    End Try
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    Protected Overrides Sub ProcOnPassiveUllFinalizeError(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)

                If oScenarioEnv.ProcOnPassiveUllAnonyError(oXllReqTeleg) = False Then
                    Log.Error("PassiveUll failed by finalize error.")
                End If

                If Config.DeletePassiveUllTmpFileOnSeqEnd Then
                    Try
                        File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.FileName)))
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                    End Try
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    Protected Overrides Sub ProcOnPassiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)

                If oScenarioEnv.ProcOnPassiveUllAnonyError(oXllReqTeleg) = False Then
                    Log.Error("PassiveUll failed.")
                End If

                If Config.DeletePassiveUllTmpFileOnSeqEnd Then
                    Try
                        File.Delete(Path.Combine(sPermittedPath, Path.GetFileName(oXllReqTeleg.FileName)))
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                    End Try
                End If

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    'ヘッダ部の内容がウォッチドッグREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsWatchdogReq(ByVal iTeleg As ITelegram) As Boolean
        If Config.EnableWatchdog = False Then
            Return False
        End If

        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oTeleg.ObjCode = ObjCodeForWatchdogIn(Config.AplProtocol) Then
            Return True
        Else
            Return False
        End If
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsWatchdogReq(ByVal iTeleg As ITelegram) As IWatchdogReqTelegram
        Return New EkWatchdogReqTelegram(iTeleg)
    End Function

    '新しいコネクションを得た場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionAppear()
        reqNumberForNextSnd = 0

        If connectedByScenario Then Return

        Dim automaticComStart As Boolean
        SyncLock oForm.UiState
            automaticComStart = oForm.UiState.AutomaticComStart
        End SyncLock

        If automaticComStart Then
            enableActiveOneOrdering = True

            If Config.AplProtocol = EkAplProtocol.Tokatsu Then
                Dim sSeqName As String = "TimeDataGet #" & traceNumberForTimeDataGet.ToString()
                UpdateTraceNumberForTimeDataGet()

                Log.Info("Register " & sSeqName & " as ActiveOne.")

                Dim oReqTeleg As New EkTimeDataGetReqTelegram( _
                   oTelegGene, _
                   ObjCodeForTimeDataGetIn(Config.AplProtocol),
                   Config.TimeDataGetReplyLimitTicks)

                RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
            Else
                Dim sSeqName As String = "ComStart #" & traceNumberForComStart.ToString()
                UpdateTraceNumberForComStart()

                Log.Info("Register " & sSeqName & " as ActiveOne.")

                Dim oReqTeleg As New EkComStartReqTelegram( _
                   oTelegGene, _
                   ObjCodeForComStartIn(Config.AplProtocol),
                   Config.ComStartReplyLimitTicks)

                RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
            End If
            LineStatus = LineStatus.ComStartWaiting
        End If
    End Sub

    'NOTE: このTelegrapherは、コネクションを作った際にLineStatusを
    'ComStartWaitingにするため、下記で定義している
    'ProcOnReqTelegramReceiveCompleteBySendAck～ProcOnConnectionDisappear
    'が呼ばれる際のLineStatusは、ComStartWaitingかSteadyのいずれかである。

    'REQ電文受信及びそれに対するACK電文送信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Function ProcOnReqTelegramReceiveCompleteBySendAck(ByVal iRcvTeleg As ITelegram, ByVal iSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ電文受信及びそれに対する軽度NAK電文（BUSY等）送信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Function ProcOnReqTelegramReceiveCompleteBySendNak(ByVal iRcvTeleg As ITelegram, ByVal iSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ電文送信及びそれに対するACK電文受信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Function ProcOnReqTelegramSendCompleteByReceiveAck(ByVal iSndTeleg As ITelegram, ByVal iRcvTeleg As ITelegram) As Boolean
        LineStatus = LineStatus.Steady
        Return True
    End Function

    'REQ電文送信及びそれに対する軽度NAK電文（BUSY等）受信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Function ProcOnReqTelegramSendCompleteByReceiveNak(ByVal iSndTeleg As ITelegram, ByVal iRcvTeleg As ITelegram) As Boolean
        'NOTE: たとえ開局時であっても、シミュレータなので、自動切断はしない。
        '切断は手動で（またはシナリオで）自由に実施することにしている。
        Return True
    End Function

    'コネクションを切断した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionDisappear()
        oScenarioEnv.ProcOnConnectionDisappear()
        LineStatus = LineStatus.Disconnected
    End Sub

    'NOTE: 以下の4メソッドで行っているFTP送受信ファイルのsCapDirPathへの
    '保存は、ActiveXllWorkerやPassiveXllWorkerをFtpWorkerから派生させた
    'MyFtpWorkerのインスタンスとして用意し、それらが包含するスレッドにて、
    'FTP完了のタイミングで（TelegrapherにResponseを返信する前に）
    '実施する方が、出入口における機械的記録という目的に合致する(*1)。
    'しかし、現状のFtpWorkerは、オーバライド可能なメソッドである
    'ProcOnDownloadRequestReceiveとProcOnUploadRequestReceiveに
    'おいて、「FTPの実施」と「TelegrapherへのResponse返信」を
    '直接実装しているため、MyFtpWorkerでそれらをオーバライドして、
    '「FTPの実施」と「TelegrapherへのResponse返信」の間に
    'カスタムな処理を追加する場合、「FTPの実施」などまで
    '自前で実装する必要が生じてしまう。それは、あまりにも無駄で
    'ある（IXllWorkerをImplementsするクラスを新規に用意するのと
    '大差がない）ため、FtpWorkerをリファクタリング可能な機会がある
    'までは、以下の4メソッドで保存を行うことにした。
    'なお、ActiveXllWorkerやPassiveXllWorkerで保存を行うようにする
    '場合は、それらの2スレッドにおける「存在しないファイル名の検索～
    'ファイルの作成」でレースコンディションが発生しないよう、
    'ActiveXllWorkerで生成するファイルとPassiveXllWorkerで生成する
    'ファイルには、別の命名規則を適用するべき（CapDataPathにおける
    'TransKind部分を別の文字にするべき）である。
    '*1 実際、Telegrapherは、Workerに依頼した仕事がある状態でDisconnect()を
    '行った場合、WorkerにCancelTransfer()させた後、WorkerからのResponseは
    '待つものの、それを以下の4メソッドに渡すことはしない。よって、以下の
    '4メソッドをオーバーライドする方式だと、たとえWorkerにおいて
    'FTPが成功していたとしても、当該ファイルは保存しないこととなる。

    Protected Overrides Function ProcOnActiveDownloadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If DownloadResponse.Parse(oRcvMsg).Result = DownloadResult.Finished Then
            Dim capRcvFiles As Boolean
            SyncLock oForm.UiState
                capRcvFiles = oForm.UiState.CapRcvFiles
            End SyncLock

            If capRcvFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oActiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnActiveDownloadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnActiveUploadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If UploadResponse.Parse(oRcvMsg).Result = UploadResult.Finished Then
            Dim capSndFiles As Boolean
            SyncLock oForm.UiState
                capSndFiles = oForm.UiState.CapSndFiles
            End SyncLock

            If capSndFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oActiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnActiveUploadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnPassiveDownloadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If DownloadResponse.Parse(oRcvMsg).Result = DownloadResult.Finished Then
            Dim capRcvFiles As Boolean
            SyncLock oForm.UiState
                capRcvFiles = oForm.UiState.CapRcvFiles
            End SyncLock

            If capRcvFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oPassiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnPassiveDownloadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnPassiveUploadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If UploadResponse.Parse(oRcvMsg).Result = UploadResult.Finished Then
            Dim capSndFiles As Boolean
            SyncLock oForm.UiState
                capSndFiles = oForm.UiState.CapSndFiles
            End SyncLock

            If capSndFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oPassiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnPassiveUploadResponseReceive(oRcvMsg)
    End Function
#End Region

#Region "イベント処理実装用メソッド"
    Protected Sub PostponeParentMessages()
        RegisterTimer(oParentMessageProcTimer, TickTimer.GetSystemTick())
    End Sub

    Protected Function SendNakTelegram(ByVal cause As NakCauseCode, ByVal oSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As ITelegram = oSourceTeleg.CreateNakTelegram(cause)
        If oReplyTeleg IsNot Nothing Then
            Log.Info("Sending NAK (" & cause.ToString() & ") telegram...")
            Return SendReplyTelegram(oReplyTeleg, oSourceTeleg)
        Else
            Return False
        End If
    End Function

    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        oReqTeleg.ReqNumber = reqNumberForNextSnd
        oReqTeleg.ClientCode = selfEkCode
        Dim ret As Boolean = MyBase.SendReqTelegram(oReqTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReqTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        If reqNumberForNextSnd >= 999999 Then
            reqNumberForNextSnd = 0
        Else
            reqNumberForNextSnd += 1
        End If

        Return ret
    End Function

    Protected Overrides Function SendReplyTelegram(ByVal iReplyTeleg As ITelegram, ByVal iSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As EkTelegram = DirectCast(iReplyTeleg, EkTelegram)
        Dim oSourceTeleg As EkTelegram = DirectCast(iSourceTeleg, EkTelegram)
        oReplyTeleg.RawReqNumber = oSourceTeleg.RawReqNumber
        oReplyTeleg.RawClientCode = oSourceTeleg.RawClientCode
        Dim ret As Boolean = MyBase.SendReplyTelegram(oReplyTeleg, oSourceTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReplyTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return ret
    End Function

    'NAK電文を送信する場合や受信した場合のその後の挙動を決めるためのメソッド
    Protected Overrides Function GetRequirement(ByVal oNakTeleg As INakTelegram) As NakRequirement
        Dim violation As NakCauseCode = oNakTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            'NOTE: ClientTelegrapher.SendReplyTelegramに「コマンド種別がNAKであり、
            'NAK事由が正確にParseできない」バイト配列を渡した場合にのみあり得る。
            'シナリオで任意バイト列の応答電文を返却可能なこのアプリ特有のケースである。
            'シミュレータとしての利便性を考慮し、そのようなケースでは、
            '自動で回線切断を行わないようにしておく。
            Return NakRequirement.ForgetOnRetryOver
        End If

        'TODO: データ種別ごとにあり得るNAK事由をプロトコルで規定し、
        'ここにおいてデータ種別とNAK事由の組み合わせで分岐するなら、
        '殆どのケースはプロトコル違反となって、
        'NakRequirement.DisconnectImmediatelyを返却することになるはず。
        Select Case oNakTeleg.CauseCode
            '継続（リトライオーバー）しても異常とはみなせないNAK電文
            'Case EkNakCauseCode.Xxxx
            '    Return NakRequirement.ForgetOnRetryOver

            '継続（リトライオーバー）したら異常とみなすべきNAK電文
            Case EkNakCauseCode.Busy, EkNakCauseCode.NoData, EkNakCauseCode.NoTime, EkNakCauseCode.Unnecessary, EkNakCauseCode.InvalidContent, EkNakCauseCode.UnknownLight
                Return NakRequirement.CareOnRetryOver

            '通信異常とみなすべきNAK電文
            Case EkNakCauseCode.TelegramError, EkNakCauseCode.NotPermit, EkNakCauseCode.HashValueError, EkNakCauseCode.UnknownFatal
                Return NakRequirement.DisconnectImmediately

            'NOTE: どのようなバイト列をParseしてもCauseCodeがNoneの
            'NAK電文にはならないはずであるため、CauseCodeがNoneの場合、
            '下記のケースとして処理する。
            Case Else
                Debug.Fail("This case is impermissible.")
                Return NakRequirement.CareOnRetryOver
        End Select
    End Function

    Protected Sub UpdateTraceNumberForComStart()
        If traceNumberForComStart >= 999 Then
            traceNumberForComStart = 0
        Else
            traceNumberForComStart += 1
        End If
    End Sub

    Protected Sub UpdateTraceNumberForTimeDataGet()
        If traceNumberForTimeDataGet >= 999 Then
            traceNumberForTimeDataGet = 0
        Else
            traceNumberForTimeDataGet += 1
        End If
    End Sub

    Protected Sub UpdateTraceNumberForActiveOne()
        If traceNumberForActiveOne >= 999 Then
            traceNumberForActiveOne = 0
        Else
            traceNumberForActiveOne += 1
        End If
    End Sub

    Protected Function ConnectForScenario() As Integer
        If curState <> State.NoConnection Then
            Log.Info("I have already connected.")
            Return 0
        End If

        Dim sServerName As String = Config.ServerIpAddr & "." & Config.IpPortForTelegConnection.ToString()
        Log.Info("Connecting to [" & sServerName & "]...")
        LineStatus = LineStatus.ConnectWaiting
        Dim oTelegSock As Socket
        Try
            oTelegSock = SockUtil.Connect(Config.ServerIpAddr, Config.IpPortForTelegConnection)
        Catch ex As OPMGException
            Log.Error("Exception caught.", ex)
            LineStatus = LineStatus.ConnectFailed
            Return -1
        End Try
        Dim oLocalEndPoint As IPEndPoint = DirectCast(oTelegSock.LocalEndPoint, IPEndPoint)
        Dim sClientName As String = oLocalEndPoint.Address.ToString() & "." & oLocalEndPoint.Port.ToString()
        Log.Info("Connection established by [" & sClientName & "] to [" & sServerName & "].")
        LineStatus = LineStatus.Connected

        connectedByScenario = True
        Connect(oTelegSock)
        connectedByScenario = False

        Dim systemTick As Long = TickTimer.GetSystemTick()
        LastPulseTick = systemTick
        RegisterTimer(oWatchdogTimer, systemTick)
        Return 1
    End Function

    Protected Sub DisconnectForScenario()
        If curState <> State.NoConnection Then
            Disconnect()
        End If
    End Sub
#End Region

End Class

''' <summary>
''' 回線状態。
''' </summary>
Public Enum LineStatus As Integer
    Initial
    ConnectWaiting
    ConnectFailed
    Connected
    ComStartWaiting
    Steady
    Disconnected
End Enum
