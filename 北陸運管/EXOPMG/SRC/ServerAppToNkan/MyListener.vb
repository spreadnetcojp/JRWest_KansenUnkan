' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Linq
Imports System.Messaging
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 対Ｎ間通信プロセスのクライアント管理クラス。
''' </summary>
Public Class MyListener

#Region "内部クラス等"
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
        Public Code As EkCode
        Public Telegrapher As MyTelegrapher
        Public ChildSteerSock As Socket
        Public ListenerSock As Socket
    End Class
#End Region

#Region "定数や変数"
    'スレッド名
    Protected Const ThreadName As String = "Listener"

    'クライアント名出力書式
    Protected Const EkCodeOupFormat As String = "%3R%3S"

    '電文送受信スレッドのAbort応答期限
    Protected Const TelegrapherAbortLimitTicks As Integer = 5000  'TODO: 設定から取得する？

    '電文取り込み器
    Protected oTelegImporter As NkTelegramImporter

    'クライアントのリスト
    Protected oClientList As LinkedList(Of Client) 'OPT: Dictionaryに変更？

    'スレッド
    Private oThread As Thread

    '親スレッドからの終了要求
    Private _IsQuitRequest As Integer
#End Region

#Region "プロパティ"
    Private Property IsQuitRequest() As Boolean
        Get
            Return CBool(Thread.VolatileRead(_IsQuitRequest))
        End Get

        Set(ByVal val As Boolean)
            Thread.VolatileWrite(_IsQuitRequest, CInt(val))
        End Set
    End Property
#End Region

#Region "コンストラクタ"
    Public Sub New()
        Me.oTelegImporter = New NkTelegramImporter()
        Me.oThread = New Thread(AddressOf Me.Task)
        Me.oThread.Name = ThreadName
        Me.IsQuitRequest = False
    End Sub
#End Region

#Region "親スレッド用メソッド"
    Public Overridable Sub Start()
        oThread.Start()
    End Sub

    Public Sub Quit()
        IsQuitRequest = True
    End Sub

    Public Sub Join()
        oThread.Join()
    End Sub

    Public Function Join(ByVal millisecondsTimeout As Integer) As Boolean
        Return oThread.Join(millisecondsTimeout)
    End Function

    'NOTE: このクラスに問題がない限り、Quit()で済ませるべきである。
    Public Sub Abort()
        oThread.Abort()
    End Sub

    Public ReadOnly Property ThreadState() As ThreadState
        Get
            Return oThread.ThreadState
        End Get
    End Property
#End Region

#Region "メソッド"
    ''' <summary>
    ''' 通信管理スレッドのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' リスニングソケットの制御およびTelegrapherの管理を行う。
    ''' </remarks>
    Private Sub Task()
        Dim oMessageQueue As MessageQueue = Nothing
        Try
            Log.Info("The listener thread started.")

            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            Dim fewSpan As New TimeSpan(0, 0, 0, 0, Config.PollIntervalTicks)
            Dim oCheckReadList As New ArrayList()
            Dim oFilter As New MessagePropertyFilter()
            oFilter.ClearAll()
            oFilter.AppSpecific = True
            oFilter.Body = True

            'NOTE: Config.MyMqPathのメッセージキューは、
            'この時点で必ず存在している前提である。存在していなければ、
            'システムに異常がある故、このプロセスは起動直後に終了するべき
            'である。
            oMessageQueue = New MessageQueue(Config.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([String])})

            oClientList = New LinkedList(Of Client)

            ProcOnManagementReady()

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Not IsQuitRequest
                '電文送受信スレッドからのメッセージをチェックする。
                'NOTE: 対Ｎ間の電文送受信スレッドは親スレッドにメッセージを送信
                'しないため、ほとんど無意味であるが、ソケットをクローズしたこと
                'が検出できるという点で意味があるため、残してある。
                oCheckReadList.Clear()
                For Each oClient As Client In oClientList
                    If oClient.State = ClientState.Started Then
                        oCheckReadList.Add(oClient.ChildSteerSock)
                    End If
                Next oClient
                If oCheckReadList.Count <> 0 Then
                    'ソケットが読み出し可能かチェックする。
                    Socket.Select(oCheckReadList, Nothing, Nothing, 0)

                    '読み出し可能である場合は、メッセージを読み出す。
                    If oCheckReadList.Count > 0 Then
                        Dim oReadableSock As Socket = DirectCast(oCheckReadList(0), Socket)
                        Dim oClient As Client = FindClient(oReadableSock)
                        ProcOnChildSteerSockReadable(oClient)
                    End If
                End If

                '他のプロセスからのメッセージをチェックする。
                Dim oMessage As Message = Nothing
                Try
                    '所定時間メッセージを待つ。
                    'NOTE: MessageQueue.Receive()のタイムアウトは、実際に
                    '引数の時間が経過した際ではなく、呼び出し時点の
                    'システム時刻に引数の時間を加えた時刻Tを求めた上で、
                    'システム時刻がT以上になった際に行われるようである。
                    'つまり、呼び出しの間にシステム時刻が1時間戻されれば、
                    '呼び出しから戻るのは、「引数の時間+1時間」経過後に
                    'なってしまい、その間は、子スレッドの生存監視や
                    '親プロセスへの生存証明を行うことができなくなる。
                    'メインスレッドからの終了要求にも反応できなくなる。
                    'しかし、そのことが問題になるような大きな時刻補正が
                    '行われることはないという前提で、ここで待つことに
                    'している。実際に大きな時刻補正があるなら注意。
                    oMessage = oMessageQueue.Receive(fewSpan)
                Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                    'タイムアウトの場合である。この例外については握りつぶして、
                    'oMessage Is Nothingのまま、以下を実行する。
                End Try

                If oMessage IsNot Nothing Then
                    ProcOnMessageReceive(oMessage)
                End If

                '以上の処理でAbortTelegrapherの対象になったClientについて、
                'ProcOnTelegrapherAbortを呼び出す。
                'その中でAbortTelegrapherの対象になったClientについては、
                '次回にProcOnTelegrapherAbortを呼び出す。
                PrepareToRestartTelegraphers()

                '前回チェックから所定時間経過している場合は、全ての
                '電文送受信スレッドについて、異常終了またはフリーズ
                'していないかチェックする。
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()

                    Log.Info("Checking pulse of all telegraphers...")
                    For Each oClient As Client In oClientList
                        If oClient.State = ClientState.Started Then
                            If oClient.Telegrapher.ThreadState = ThreadState.Stopped Then
                                '予期せぬ例外などで異常終了している場合である。
                                Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] has stopped.")
                                AbortTelegrapher(oClient)
                            ElseIf TickTimer.GetTickDifference(systemTick, oClient.Telegrapher.LastPulseTick) > Config.TelegrapherPendingLimitTicks Then
                                'フリーズしている場合である。
                                Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
                                AbortTelegrapher(oClient)
                            End If
                        End If
                    Next oClient
                    PrepareToRestartTelegraphers()
                    RestartTelegraphers()
                End If
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP発生（または収集データ誤記テーブルへの登録）は、
            'プロセスマネージャが行うので、ここでは不要である。

            MainClass.oMainForm.Invoke(New MethodInvoker(AddressOf MainClass.oMainForm.Close))
        Finally
            If oClientList IsNot Nothing
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

            If oMessageQueue IsNot Nothing Then
                oMessageQueue.Close()
            End If
        End Try
    End Sub

    Private Function FindClient(ByVal oSocket As Socket) As Client
        For Each oClient As Client In oClientList
            If oClient.ChildSteerSock Is oSocket Then Return oClient
        Next oClient
        Return Nothing 'NOTE: あり得ないと考えてよい。
    End Function

    Protected Function FindClient(ByVal code As EkCode) As Client
        For Each oClient As Client In oClientList
            If oClient.Code = code Then Return oClient
        Next oClient
        Return Nothing
    End Function

    Protected Sub RegisterClient(ByVal code As EkCode, ByVal port As Integer)
        'リッスンを開始する。
        Log.Info("Start listening for [" & Config.IpAddrForTelegConnection.ToString() & ":" & port.ToString() & "].")
        Dim oListenerSock As Socket = SockUtil.StartListener(Config.IpAddrForTelegConnection, port)

        Log.Info("Registering telegrapher [" & code.ToString(EkCodeOupFormat) & "]...")
        Dim oParentSock As Socket = Nothing
        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oParentSock, oChildSock)
        Dim oTelegrapher As New MyTelegrapher( _
          code.ToString(EkCodeOupFormat), _
          oChildSock, _
          oTelegImporter, _
          code, _
          oListenerSock)
        Dim oClient As New Client()
        oClient.State = ClientState.Registered
        oClient.Code = code
        oClient.Telegrapher = oTelegrapher
        oClient.ChildSteerSock = oParentSock
        oClient.ListenerSock = oListenerSock
        oClientList.AddLast(oClient)
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
                Log.Warn("The telegrapher is already marked as broken.")
                Return False
            Else
                Log.Warn("The telegrapher is waiting for restart.")
                AbortTelegrapher(oClient)
                Return False
            End If
        End If

        If oMsg.WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
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
                   oTelegImporter, _
                   oClient.Code, _
                   oClient.ListenerSock)

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
        Dim oNode As LinkedListNode(Of Client) = oClientList.First
        While oNode IsNot Nothing
            Dim oClient As Client = oNode.Value
            If oClient.State = ClientState.Discarded Then
                Dim oDiscardedNode As LinkedListNode(Of Client) = oNode
                oNode = oNode.Next
                oClientList.Remove(oDiscardedNode)
                Log.Info("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] unregistered.")

                Dim port As Integer = DirectCast(oClient.ListenerSock.LocalEndPoint, IPEndPoint).Port
                oClient.ListenerSock.Close()
                Log.Info("End listening for [" & Config.IpAddrForTelegConnection.ToString() & ":" & port.ToString() & "].")
            Else
                oNode = oNode.Next
            End If
        End While
    End Sub

    Protected Overridable Function SelectStationsInService(ByVal sServiceDate As String) As DataTable
        '機器構成マスタにある「機種が監視盤または窓処」かつ「開始日がsServiceDate以前」の
        'レコードの、「駅コード」と「Ｎ間サーバ用ポート番号」を取得する。
        'NOTE: 機器構成マスタに「駅コード」が同一で「Ｎ間サーバ用ポート番号」が異なったり、
        '「Ｎ間サーバ用ポート番号」が同一で「駅コード」が異なったりするレコードは
        '存在しないものとする。

        Dim sSQL As String = _
           "SELECT DISTINCT RAIL_SECTION_CODE, STATION_ORDER_CODE, NK_PORT_NO" _
           & " FROM M_MACHINE" _
           & " WHERE (MODEL_CODE = 'W' OR MODEL_CODE = 'Y')" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return dbCtl.ExecuteSQLToRead(sSQL)

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function
#End Region

#Region "イベント処理メソッド"
    Protected Overridable Sub ProcOnManagementReady()
        'クライアントを登録する。
        'NOTE: 起動時なので、万が一、ランタイムな例外が発生した場合は、
        'プロセス終了とする。
        Dim serviceStations As DataRowCollection = SelectStationsInService(EkServiceDate.GenString()).Rows
        For Each serviceStation As DataRow In serviceStations
            Dim code As EkCode
            code.RailSection = Integer.Parse(serviceStation.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(serviceStation.Field(Of String)("STATION_ORDER_CODE"))
            Dim port As Integer = serviceStation.Field(Of Integer)("NK_PORT_NO")
            RegisterClient(code, port)
        Next serviceStation

        '全クライアントの電文送受信スレッドを開始する。
        For Each oClient As Client In oClientList
            StartTelegrapher(oClient)
        Next oClient
    End Sub

    Protected Overridable Sub ProcOnTelegrapherAbort(ByVal oClient As Client)
        'NOTE: 既に停止して再起動待ちの状態（ClientState.WaitingForRestart）の
        'oClientについては、それに対するメッセージ送信を試みた際、
        '再度このメソッドが呼び出されるようになっている。
        'この仕様は、TelServerAppListenerの実装を流用していることに起因しており、
        '対Ｎ間通信プロセス用のListenerにとっては、特に意味があるわけではない。

        '収集データ誤記テーブルに異常を登録する。
        'NOTE: 上記の仕様ゆえ、再起動待ちのoClientに対するメッセージ送信が
        'あれば、何度でもここが実行されることになるが、何度登録しても、
        '特に問題ないはずであるため、状態の管理は行わず、無条件に
        '登録を行うことにしている。
        Using curProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess()
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtThreadAbended.Gen(curProcess.ProcessName, oClient.Code.ToString(EkCodeOupFormat)))
        End Using
    End Sub

    Protected Overridable Sub ProcOnTelegrapherRestart(ByVal oClient As Client)
    End Sub

    Protected Overridable Sub ProcOnChildSteerSockReadable(ByVal oClient As Client)
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks)
        If Not oRcvMsg.HasValue Then
            Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
            AbortTelegrapher(oClient)
            Return
        End If

        ProcOnChildMessageReceive(oClient, oRcvMsg)
    End Sub

    Protected Overridable Sub ProcOnChildMessageReceive(ByVal oClient As Client, ByVal oRcvMsg As InternalMessage)
    End Sub

    Protected Overridable Sub ProcOnMessageReceive(ByVal oMessage As Message)
        Select Case oMessage.AppSpecific
            Case ExtServiceDateChangeNotice.FormalKind
                Log.Info("ExtServiceDateChangeNotice received.")
                ProcOnServiceDateChangeNoticeReceive(oMessage)
            Case ExtTallyTimeNotice.FormalKind
                Log.Info("ExtTallyTimeNotice received.")
                ProcOnTallyTimeNoticeReceive(oMessage)
            Case Else
                Log.Error("Unwelcome ExtMessage received.")
        End Select
    End Sub

    Protected Overridable Sub ProcOnServiceDateChangeNoticeReceive(ByVal oMessage As Message)
        '機器構成マスタから、現在の運用日付で運用されるべき全ての駅を検索する。
        Dim oServiceStationTable As DataTable
        Try
            oServiceStationTable = SelectStationsInService(EkServiceDate.GenString())
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'ユーザが気付く場所に異常を記録する。
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtMachineMasterErratumDetected.Gen())
            Return
        End Try

        Dim serviceStations As EnumerableRowCollection(Of DataRow) = oServiceStationTable.AsEnumerable()

        '既に登録している駅に関して、検索の結果に含まれていない場合は、
        '当該駅用の電文送受信スレッドに終了を要求する。
        For Each oClient As Client In oClientList
            Dim code As EkCode = oClient.Code
            Dim port As Integer = DirectCast(oClient.ListenerSock.LocalEndPoint, IPEndPoint).Port
            Dim num As Integer = ( _
               From serviceStation In serviceStations _
               Where serviceStation.Field(Of String)("RAIL_SECTION_CODE") = code.RailSection.ToString("D3") And _
                     serviceStation.Field(Of String)("STATION_ORDER_CODE") = code.StationOrder.ToString("D3") And _
                     serviceStation.Field(Of Integer)("NK_PORT_NO") = port _
               Select serviceStation _
            ).Count

            If num = 0 Then
                'NOTE: 配信中や収集中の場合、本来送信する権利を持つのは
                'Telegrapher側である。そのことを考慮するとTelegrapherに
                'QuitRequest受信専用ソケットを用意する方が自然である。
                QuitTelegrapher(oClient)
            End If
        Next oClient

        '終了を待つ。
        WaitForTelegraphersToQuit()

        '登録解除する。
        UnregisterDiscardedClients()

        '検索で得た駅に関して、登録されていないものは、登録する。
        For Each row As DataRow In oServiceStationTable.Rows
            Dim code As EkCode
            code.RailSection = Integer.Parse(row.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(row.Field(Of String)("STATION_ORDER_CODE"))
            Dim port As Integer = row.Field(Of Integer)("NK_PORT_NO")
            Dim oClient As Client = FindClient(code)
            If oClient Is Nothing Then
                RegisterClient(code, port)
            Else If DirectCast(oClient.ListenerSock.LocalEndPoint, IPEndPoint).Port <> port
                'Codeが同じでPort番号が異なるクライアントが既に登録されている場合である。
                Log.Error("同一の駅[" & code.ToString(EkCodeOupFormat) & "]が異なるポート番号で登録されています。")

                'ユーザが気付く場所に異常を記録する。
                CollectedDataTypoRecorder.Record( _
                   New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                   DbConstants.CdtKindServerError, _
                   Lexis.CdtMachineMasterErratumDetected.Gen())
            End If
        Next row

        '登録した駅の電文送受信スレッドを開始させる。
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Registered Then
                StartTelegrapher(oClient)
            End If
        Next oClient
    End Sub

    Protected Overridable Sub ProcOnTallyTimeNoticeReceive(ByVal oMessage As Message)
        For Each oClient As Client In oClientList
            SendToTelegrapher(oClient, TallyTimeNotice.Gen())
        Next oClient
    End Sub
#End Region

End Class
