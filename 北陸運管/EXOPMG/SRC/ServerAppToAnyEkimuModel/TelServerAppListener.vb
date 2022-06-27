' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
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
''' 通信プロセス共通のクライアント管理クラス。
''' </summary>
Public MustInherit Class TelServerAppListener
#Region "内部クラス等"
    Protected Enum ClientState
        Registered
        Started
        Aborted
        WaitingForRestart
        QuitRequested
        Discarded
    End Enum

    Protected Enum ClientActiveXllState
        None
        Waiting
        Running
    End Enum

    Protected Class Client
        Public State As ClientState
        Public Code As EkCode
        '-------Ver0.1 次世代車補対応 ADD START-----------
        Public StationName As String
        Public CornerName As String
        '-------Ver0.1 次世代車補対応 ADD END-------------
        Public Telegrapher As ServerTelegrapher
        Public ChildSteerSock As Socket

        Public MasProDllState As ClientActiveXllState
        Public SendSuiteOnMasProDll As Boolean
        Public ScheduledUllState As ClientActiveXllState
        Public Sub New()
            MasProDllState = ClientActiveXllState.None
            ScheduledUllState = ClientActiveXllState.None
        End Sub
    End Class

    Protected Class MasProDllInfo
        Public DataApplicableModel As String 'WまたはGまたはY
        Public DataPurpose As String 'MSTまたはPRG
        Public DataKind As String 'DSHやWPG
        Public DataSubKind As String 'パターン番号またはエリア番号
        Public DataVersion As String
        Public DataFileName As String
        Public DataFileHashValue As String
        Public ListVersion As String
        Public ListFileName As String
        Public ListFileHashValue As String
        Public RemainingCount As Integer 'WaitingとRunningの合計件数
        Public WaitingClients As List(Of Client)
    End Class

    Protected Class ScheduledUllInfo
        Public FileName As String
        Public RemainingCount As Integer 'WaitingとRunningの合計件数
        Public WaitingClients As List(Of Client)
    End Class
#End Region

#Region "定数や変数"
    '各種テーブル共通の項目にセットする値
    Protected Const UserId As String = "System"
    Protected Const MachineId As String = "Server"

    'スレッド名
    Protected Const ThreadName As String = "Listener"

    'クライアント名出力書式
    Protected Const EkCodeOupFormat As String = "%3R%3S_%4C_%2U"

    '電文送受信スレッドのAbort応答期限
    'NOTE: いくつかの派生クラスの電文送受信スレッドは、Abortの際に
    'DLL状態テーブルを戻す必要があるため、長めにしておくのが無難である。
    Protected Const TelegrapherAbortLimitTicks As Integer = 10000  'TODO: 設定から取得する？

    'クライアントのリスト
    Protected oClientList As LinkedList(Of Client) 'OPT: Dictionaryに変更？

    'スレッド
    Private oThread As Thread

    '親スレッドからの終了要求
    Private _IsQuitRequest As Integer

    '通信相手の（プロトコル仕様）機種コード
    Protected clientModelInProtocol As Integer

    '通信相手の（DB仕様）機種コード
    Protected sClientModel As String

    '通信相手の（DB仕様）コネクション区分
    Protected sPortPurpose As String

    'マスタ/プログラムのDLLを担当するか否か
    Protected handlesMasProDll As Boolean

    '配信処理のキュー
    Protected oMasProDllQueue As Queue(Of ExtMasProDllRequest)

    '現在実行中の配信処理
    Protected oCurMasProDll As MasProDllInfo

    '収集処理のキュー
    Protected oScheduledUllQueue As Queue(Of ExtScheduledUllRequest)

    '現在実行中の収集処理
    Protected oCurScheduledUll As ScheduledUllInfo

    '収集データ誤記テーブルに記録するための通信相手機種名称（派生クラスで必ず設定する）
    Protected sCdtClientModelName As String

    '収集データ誤記テーブルに記録するポート名称（派生クラスで必ず設定する）
    Protected sCdtPortName As String
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
    Public Sub New(ByVal clientModelInProtocol As Integer, ByVal sClientModel As String, ByVal sPortPurpose As String, ByVal handlesMasProDll As Boolean)
        Me.clientModelInProtocol = clientModelInProtocol
        Me.sClientModel = sClientModel
        Me.sPortPurpose = sPortPurpose
        Me.handlesMasProDll = handlesMasProDll

        Me.oThread = New Thread(AddressOf Me.Task)
        Me.oThread.Name = ThreadName
        Me.IsQuitRequest = False

        Me.oMasProDllQueue = New Queue(Of ExtMasProDllRequest)
        Me.oCurMasProDll = Nothing

        Me.oScheduledUllQueue = New Queue(Of ExtScheduledUllRequest)
        Me.oCurScheduledUll = Nothing
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
        Dim oListenerSock As Socket = Nothing  'リスニングソケット
        Try
            Log.Info("The listener thread started.")
            Dim oDiagnosisTimer As New TickTimer(TelServerAppBaseConfig.SelfDiagnosisIntervalTicks)
            Dim oCheckReadList As New ArrayList()

            '各電文送受信スレッドの一時作業用ディレクトリを親ディレクトリごとまとめて削除する。
            Log.Info("Sweeping directory [" & TelServerAppBaseConfig.TemporaryBaseDirPath & "]...")
            Utility.DeleteTemporalDirectory(TelServerAppBaseConfig.TemporaryBaseDirPath)

            '各電文送受信スレッドのFTPサイト用ディレクトリやその内容物を削除する。
            'NOTE: このディレクトリは、このプロセスだけでなく、FTPサーバも参照・操作し得る。
            '既に存在しているものの削除に失敗する場合は、転送の終了を認識していないFTPサーバが
            '書き込みで握っているケースと考えられるが、該当するサブディレクトリやファイルのみを
            '残して処理を強行する。なお、処理を強行せずにこのプロセスを異常終了させるとしても、
            'プロセスマネージャがこのプロセスを起動する次の機会にFTPサーバがガードを解いて
            'いれば、そこから正常動作が始まるため、問題はないはずである。アプリ再起動の
            'この機会を逃さずに、全ての一時ファイルを削除するという意味では、その方が理想的で
            'あるかもしれないが、とりあえず可用性を優先して、このようにしている。
            Dim sFtpBase As String = Utility.CombinePathWithVirtualPath(TelServerAppBaseConfig.FtpServerRootDirPath, TelServerAppBaseConfig.PermittedPathInFtp)
            If Directory.Exists(sFtpBase) Then
                Log.Info("Cleaning up directory [" & sFtpBase & "]...")
                Utility.CleanUpDirectory(sFtpBase)
            End If

            Dim oFilter As New MessagePropertyFilter()
            oFilter.ClearAll()
            oFilter.AppSpecific = True
            oFilter.Body = True

            'NOTE: TelServerAppBaseConfig.MyMqPathのメッセージキューは、
            'この時点で必ず存在している前提である。存在していなければ、
            'システムに異常がある故、このプロセスは起動直後に終了するべき
            'である。
            oMessageQueue = New MessageQueue(TelServerAppBaseConfig.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([String])})

            '既にキューイングされているメッセージを全て読み捨てる。
            'NOTE: この後で、DLL状態テーブルの「配信中」になっている
            'レコード（メッセージで配信を要求されていた全てのレコード）
            'を「異常」にし、機器構成の読み込みも強制的に行うため、
            'ここで捨てることには全く問題ない。
            oMessageQueue.Purge()

            If handlesMasProDll Then
                'マスタDLL状態テーブルおよびプログラムDLL状態テーブルについて、
                '「配信中」になっている当該種別の全レコードを「異常」に変更。
                'NOTE: 変更できない場合は、システムに異常がある故、
                'このプロセスは起動直後に終了するべきである。
                TransitDllStatusToAbnormal(EkConstants.DataPurposeMaster)
                TransitDllStatusToAbnormal(EkConstants.DataPurposeProgram)
            End If

            oClientList = New LinkedList(Of Client)
            ProcOnManagementReady()

            'リッスンを開始する。
            Log.Info("Start listening for [" & TelServerAppBaseConfig.IpAddrForTelegConnection.ToString() & ":" & TelServerAppBaseConfig.IpPortForTelegConnection.ToString() & "].")
            oListenerSock = SockUtil.StartListener(TelServerAppBaseConfig.IpAddrForTelegConnection, TelServerAppBaseConfig.IpPortForTelegConnection)

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Not IsQuitRequest
                '電文送受信スレッドからのメッセージおよび、新たな機器からの
                '接続要求をチェックする。
                Dim oNewSocket As Socket = Nothing

                oCheckReadList.Clear()
                oCheckReadList.Add(oListenerSock)
                For Each oClient As Client In oClientList
                    If oClient.State = ClientState.Started AndAlso IsWaitingForChildMessage(oClient) Then
                        oCheckReadList.Add(oClient.ChildSteerSock)
                    End If
                Next oClient

                'ソケットが読み出し可能になるまで所定時間待機する。
                Socket.Select(oCheckReadList, Nothing, Nothing, TelServerAppBaseConfig.PollIntervalTicks * 1000)

                If oCheckReadList.Count > 0 Then
                    Dim oReadableSock As Socket = DirectCast(oCheckReadList(0), Socket)
                    If oReadableSock Is oListenerSock Then
                        'リスニングソケットが読み出し可能になった場合は、
                        '送受信用ソケットを取り出す。
                        Try
                            oNewSocket = SockUtil.Accept(oListenerSock)
                        Catch ex As OPMGException
                            'NOTE: 実際のところはともかく、リスニングソケットが読み出し可能
                            'になったからといって、そこからのAccept()が成功するとは限らない
                            '（linuxのソケットのように、Accept()を呼び出すまでの間に発生
                            'したコネクションの異常が、Accept()で通知される可能性もある）
                            'ものとみなす。
                            Log.Error("Exception caught.", ex)
                        End Try
                    Else
                        '対電文送受信スレッド用ソケットが読み出し可能に
                        'なった場合は、メッセージを読み出す。
                        Dim oClient As Client = FindClient(oReadableSock)
                        ProcOnChildSteerSockReadable(oClient)
                    End If
                End If

                '送受信用ソケットが生成された場合、電文送受信スレッドに渡す。
                If oNewSocket IsNot Nothing Then
                    Dim oRemoteEndPoint As IPEndPoint = DirectCast(oNewSocket.RemoteEndPoint, IPEndPoint)
                    Dim oRemoteIPAddr As IPAddress = oRemoteEndPoint.Address
                    Log.Info("Incoming from [" & oRemoteEndPoint.Address.ToString() & "].")
                    ProcOnAccept(oNewSocket)
                End If

                '他のプロセスからのメッセージをチェックする。
                Dim oMessage As Message = Nothing
                Try
                    '所定時間メッセージを待つ。
                    oMessage = oMessageQueue.Receive(TimeSpan.Zero)
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
                            ElseIf TickTimer.GetTickDifference(systemTick, oClient.Telegrapher.LastPulseTick) > TelServerAppBaseConfig.TelegrapherPendingLimitTicks Then
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

            'NOTE: わざわざStart時にMainFormの参照を受け取ってまで
            'これを行う必要性は低いが、万が一問題があったときは、
            '早めにウィンドウが消える方がわかりやすいので、
            '下記を行うことにしている。
            TelServerAppBaseMainClass.oMainForm.Invoke(New MethodInvoker(AddressOf TelServerAppBaseMainClass.oMainForm.Close))
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

            If oListenerSock IsNot Nothing Then
                oListenerSock.Close()
                Log.Info("End listening for [" & TelServerAppBaseConfig.IpAddrForTelegConnection.ToString() & ":" & TelServerAppBaseConfig.IpPortForTelegConnection.ToString() & "].")
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

    '-------Ver0.1 次世代車補対応 MOD START-----------
    Protected Sub RegisterClient(ByVal code As EkCode, ByVal sStationName As String, ByVal sCornerName As String)
        Log.Info("Registering telegrapher [" & code.ToString(EkCodeOupFormat) & "]...")
        Dim oParentSock As Socket = Nothing
        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oParentSock, oChildSock)
        Dim oTelegrapher As ServerTelegrapher = CreateTelegrapher( _
          code.ToString(EkCodeOupFormat), _
          oChildSock, _
          code, _
          sStationName, _
          sCornerName)
        Dim oClient As New Client()
        oClient.State = ClientState.Registered
        oClient.Code = code
        oClient.StationName = sStationName
        oClient.CornerName = sCornerName
        oClient.Telegrapher = oTelegrapher
        oClient.ChildSteerSock = oParentSock
        oClientList.AddLast(oClient)
    End Sub
    '-------Ver0.1 次世代車補対応 MOD END-------------

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

        If oMsg.WriteToSocket(oClient.ChildSteerSock, TelServerAppBaseConfig.TelegrapherPendingLimitTicks) = False Then
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
                '-------Ver0.1 次世代車補対応 MOD START-----------
                oClient.Telegrapher = CreateTelegrapher( _
                   oClient.Code.ToString(EkCodeOupFormat), _
                   oChildSock, _
                   oClient.Code, _
                   oClient.StationName, _
                   oClient.CornerName)
                '-------Ver0.1 次世代車補対応 MOD END-------------

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
        If QuitRequest.Gen().WriteToSocket(oClient.ChildSteerSock, TelServerAppBaseConfig.TelegrapherPendingLimitTicks) = False Then
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
        Dim oJoinLimitTimer As New TickTimer(TelServerAppBaseConfig.TelegrapherPendingLimitTicks)
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
            Else
                oNode = oNode.Next
            End If
        End While
    End Sub

    Protected Sub SendNakTelegramThenDisconnect(ByVal cause As NakCauseCode, ByVal oSourceTeleg As EkTelegram, ByVal oSocket As Socket)
        Dim oReplyTeleg As EkNakTelegram = oSourceTeleg.CreateNakTelegram(cause)
        If oReplyTeleg IsNot Nothing Then
            Log.Info("Sending NAK (" & cause.ToString() & ") telegram...")
            SendReplyTelegram(oSocket, oReplyTeleg, oSourceTeleg)
            '上記呼び出しの戻り値は無視する（その後の処理に差異がないため）。
        End If

        Log.Info("Closing the connection...")
        Try
            oSocket.Shutdown(SocketShutdown.Both)
        Catch ex As SocketException
            Log.Error("SocketException caught.", ex)
        End Try
        oSocket.Close()
    End Sub

    Protected Function SendReplyTelegram(ByVal oSocket As Socket, ByVal oReplyTeleg As EkTelegram, ByVal oSourceTeleg As EkTelegram) As Boolean
        oReplyTeleg.RawReqNumber = oSourceTeleg.RawReqNumber
        oReplyTeleg.RawClientCode = oSourceTeleg.RawClientCode
        Return oReplyTeleg.WriteToSocket(oSocket, TelServerAppBaseConfig.TelegWritingLimitBaseTicks, TelServerAppBaseConfig.TelegWritingLimitExtraTicksPerMiB, TelServerAppBaseConfig.TelegLoggingMaxLengthOnWrite)
    End Function

    Protected Overridable Function SelectUnitsInService(ByVal sServiceDate As String) As DataTable
        '-------Ver0.1 次世代車補対応 MOD START-----------
        Dim sSQL As String = _
           "SELECT STATION_NAME, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_NAME, CORNER_CODE, UNIT_NO" _
           & " FROM M_MACHINE" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND ADDRESS <> ''" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')"
        '-------Ver0.1 次世代車補対応 MOD END-------------

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

    Protected Overridable Sub TransitDllStatusToAbnormal(ByVal sDataPurpose As String)
        Dim sSQL As String = _
           "UPDATE S_" & sDataPurpose & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_STS = " & DbConstants.DllStatusAbnormal.ToString() _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Sub TransitDllStatusToAbnormal(ByVal oDll As MasProDllInfo, ByVal code As EkCode)
        'NOTE: 配信物が何であるか（データ本体か適用リストか）は、
        'DLLバージョンテーブルとoDll.DataVersionから導き出すまでもない。
        'MODEL_CODEとDATA_KIND～UNIT_NOが合致するものの中から
        'DELIVERY_STSが「配信中」のものを選べば済むはずである。
        'なお、VERSIONの合致をチェックする必要はない。
        'MODEL_CODE～DATA_VERSIONが同一でVERSIONが異なるものが
        '同時期に「配信中」になっていることはあり得ないためである。
        Dim sSQL As String = _
           "UPDATE S_" & oDll.DataPurpose & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_STS = " & DbConstants.DllStatusAbnormal.ToString() _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & oDll.DataVersion & "'" _
           & " AND RAIL_SECTION_CODE = '" & code.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & code.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & code.Corner.ToString() _
           & " AND UNIT_NO = " & code.Unit.ToString() _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Function SelectDllListFileHashValue(ByVal oDll As MasProDllInfo) As String
        Dim sSQL As String = _
           "SELECT HASH_VALUE" _
           & " FROM S_" & oDll.DataPurpose & "_LIST_HEADLINE" _
           & " WHERE MODEL_CODE = '" & oDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & oDll.DataVersion & "'" _
           & " AND LIST_VERSION = '" & oDll.ListVersion & "'"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CStr(dbCtl.ExecuteSQLToReadScalar(sSQL))

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function SelectDllDataFileHashValue(ByVal oDll As MasProDllInfo) As String
        Dim sSQL As String = _
           "SELECT HASH_VALUE" _
           & " FROM S_" & oDll.DataPurpose & "_DATA_HEADLINE" _
           & " WHERE MODEL_CODE = '" & oDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & oDll.DataVersion & "'"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CStr(dbCtl.ExecuteSQLToReadScalar(sSQL))

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function SelectDllDataFileName(ByVal oDll As MasProDllInfo) As String
        Dim sSQL As String = _
           "SELECT FILE_NAME" _
           & " FROM S_" & oDll.DataPurpose & "_DATA_HEADLINE" _
           & " WHERE MODEL_CODE = '" & oDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & oDll.DataVersion & "'"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CStr(dbCtl.ExecuteSQLToReadScalar(sSQL))

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function SelectDllAgentUnits(ByVal oDll As MasProDllInfo) As DataTable
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            '配信開始日時を取得。
            'NOTE: 条件を満たすレコードの配信開始日時は全て等しく、
            '条件を満たすレコードは必ず存在する想定である。
            Dim sSQLToSelectDllStartTime As String = _
               "SELECT TOP 1 DELIVERY_START_TIME" _
               & " FROM S_" & oDll.DataPurpose & "_DLL_STS" _
               & " WHERE MODEL_CODE = '" & sClientModel & "'" _
               & " AND DATA_KIND = '" & oDll.DataKind & "'" _
               & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
               & " AND DATA_VERSION = '" & oDll.DataVersion & "'" _
               & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()
            Dim sDeliveryStartTime As String = CStr(dbCtl.ExecuteSQLToReadScalar(sSQLToSelectDllStartTime))
            Dim dllStartTime As DateTime = DateTime.ParseExact(sDeliveryStartTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)

            '共通部分テーブル（機器構成マスタの配信指示時点の有効要素）を定義するSQLを編集。
            Dim sSQLToDefineCTE As String = _
               "WITH M_SERVICE_MACHINE (MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, ADDRESS, MONITOR_ADDRESS)" _
               & " AS" _
               & " (SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, ADDRESS, MONITOR_ADDRESS" _
                   & " FROM M_MACHINE" _
                   & " WHERE SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                  & " FROM M_MACHINE" _
                                                  & " WHERE SETTING_START_DATE <= '" & sDeliveryStartDate & "'" _
                                                  & " AND INSERT_DATE <= CONVERT(DATETIME, '" & dllStartTime.ToString("yyyy/MM/dd HH:mm:ss") & "', 120))) "

            '適用先装置の線区～号機を取得するSQLを編集。
            'Dim sSQLToSelectApplicableUnits As String = _
            '   "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
            '   & " FROM S_" & oDll.DataPurpose & "_LIST" _
            '   & " WHERE FILE_NAME = '" & oDll.ListFileName & "'"
            Dim sSQLToSelectApplicableUnits As String = _
               "SELECT RAIL_SECTION_CODE + STATION_ORDER_CODE + CAST(CORNER_CODE AS varchar) + '_' + CAST(UNIT_NO AS varchar)" _
               & " FROM S_" & oDll.DataPurpose & "_LIST" _
               & " WHERE FILE_NAME = '" & oDll.ListFileName & "'"
            'NOTE: プログラム適用リストの場合は、有効な行を抽出するにあたり、
            '適用日にもとづく追加の条件をもうけている。なお、ブランクは
            'どのような日付（数字列）よりも小さいとみなされる想定である。
            If oDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
                sSQLToSelectApplicableUnits = sSQLToSelectApplicableUnits _
                   & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                        & " OR APPLICABLE_DATE = '19000101'" _
                        & " OR APPLICABLE_DATE = '99999999')"
            End If

            '直接の送信先となる装置のIPアドレスを取得するSQLを編集。
            'NOTE: MONITOR_ADDRESSには、ブランクが入る可能性は想定していない。
            'たとえば、実際に当該コーナーに存在しない監視盤のレコードを
            '機器構成に記述する運用になったとしても、そのレコードの
            'MONITOR_ADDRESSにも、実体となる監視盤のIPアドレスが
            '設定される想定である。
            'Dim sSQLToSelectAddrOfAgents As String = _
            '   "SELECT DISTINCT MONITOR_ADDRESS" _
            '   & " FROM M_SERVICE_MACHINE" _
            '   & " WHERE (RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO) IN (" & sSQLToSelectApplicableUnits & ")" _
            '   & " AND MODEL_CODE = '" & oDll.DataApplicableModel & "'"
            Dim sSQLToSelectAddrOfAgents As String = _
               "SELECT DISTINCT MONITOR_ADDRESS" _
               & " FROM M_SERVICE_MACHINE" _
               & " WHERE MODEL_CODE = '" & oDll.DataApplicableModel & "'" _
               & " AND RAIL_SECTION_CODE + STATION_ORDER_CODE + CAST(CORNER_CODE AS varchar) + '_' + CAST(UNIT_NO AS varchar)" _
                   & " IN (" & sSQLToSelectApplicableUnits & ")"

            '直接の送信先となる装置の線区～号機を取得するSQLを編集。
            'NOTE: sSQLToSelectAddrOfAgentsで得られる全ての監視盤または統括が
            'sSQLToSelectAgentsで得られる（それぞれの出力件数が同じになる）
            '想定であるが、そのことはチェックしない。そのチェックは、
            '適用リストではなく、機器構成マスタのチェックになるため、
            '機器構成マスタの登録時に行われるべきものである。
            Dim sSQLToSelectAgents As String = _
               "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
               & " FROM M_SERVICE_MACHINE" _
               & " WHERE MODEL_CODE = '" & sClientModel & "'" _
               & " AND ADDRESS IN (" & sSQLToSelectAddrOfAgents & ")"

            '直接の送信先となる装置の線区～号機を取得する。
            Return dbCtl.ExecuteSQLToRead(sSQLToDefineCTE & sSQLToSelectAgents)

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Sub UpdateLastSendVerByUncertainFlag(ByVal oDll As MasProDllInfo, ByVal agentCode As EkCode)
        'TODO: 暫定
        Dim sSQL As String = _
           "UPDATE S_" & oDll.DataPurpose & "_DLL_VER" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DATA_VERSION = '0'" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND RAIL_SECTION_CODE = '" & agentCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & agentCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & agentCode.Corner.ToString() _
           & " AND UNIT_NO = " & agentCode.Unit.ToString() _
           & " AND UNCERTAIN_FLG <> '0'"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Function SelectLastSendVer(ByVal oDll As MasProDllInfo, ByVal agentCode As EkCode) As DataTable
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            '指定された線区～号機の装置に対する指定された種別のデータの前回送信バージョンを取得するSQLを編集。
            Dim sSQLToSelectLastSendVer As String = _
               "SELECT DATA_VERSION" _
               & " FROM S_" & oDll.DataPurpose & "_DLL_VER" _
               & " WHERE MODEL_CODE = '" & sClientModel & "'" _
               & " AND DATA_KIND = '" & oDll.DataKind & "'" _
               & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
               & " AND RAIL_SECTION_CODE = '" & agentCode.RailSection.ToString("D3") & "'" _
               & " AND STATION_ORDER_CODE = '" & agentCode.StationOrder.ToString("D3") & "'" _
               & " AND CORNER_CODE = " & agentCode.Corner.ToString() _
               & " AND UNIT_NO = " & agentCode.Unit.ToString()

            '指定された線区～号機の装置に対する指定された種別のデータの前回送信バージョンを取得
            Return dbCtl.ExecuteSQLToRead(sSQLToSelectLastSendVer)

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Sub InsertScheduledUllFailureToCdt(ByVal oUll As ScheduledUllInfo, ByVal agentCode As EkCode)
        Dim recBaseInfo As New RecDataStructure.BaseInfo(sClientModel, agentCode)

        Dim aCdtKinds As String()
        Dim sDataKind As String = EkScheduledDataFileName.GetKind(oUll.FileName)
        If DbConstants.CdtKindsOfDataKinds.ContainsKey(sDataKind) Then
            aCdtKinds = DbConstants.CdtKindsOfDataKinds(sDataKind)
        Else
            'NOTE: 不明な種別について「データの登録に失敗しました」の異常を
            '登録する場合と、フォールバックの方法が異なるが、単なる
            'フォールバックであり、Scheduleの設定に誤りがない限り、
            '動作することもないため、気にしないことにする。
            Log.Error("CollectedDataTypo code for [" & sDataKind & "] is not defined.")
            aCdtKinds = New String(0) {sDataKind}
        End If

        Dim sErrorInfo As String = Lexis.CdtScheduledUllFailed.Gen(sCdtClientModelName, agentCode.Unit.ToString())

        For i As Integer = 0 To aCdtKinds.Length - 1
            CollectedDataTypoRecorder.Record(recBaseInfo, aCdtKinds(i), sErrorInfo)
        Next
    End Sub
#End Region

#Region "イベント処理メソッド"
    '-------Ver0.1 次世代車補対応 MOD START-----------
    Protected MustOverride Function CreateTelegrapher( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal clientCode As EkCode, _
       ByVal sClientStationName As String, _
       ByVal sClientCornerName As String) As ServerTelegrapher
    '-------Ver0.1 次世代車補対応 MOD END-------------

    Protected Overridable Function IsWaitingForChildMessage(ByVal oClient As Client) As Boolean
        If oClient.MasProDllState = ClientActiveXllState.Running Then Return True
        If oClient.ScheduledUllState = ClientActiveXllState.Running Then Return True
        Return False
    End Function

    Protected Overridable Sub ProcOnManagementReady()
        'クライアントを登録する。
        'NOTE: 起動時なので、万が一、ランタイムな例外が発生した場合は、
        'プロセス終了とする。
        Dim serviceUnits As DataRowCollection = SelectUnitsInService(EkServiceDate.GenString()).Rows
        For Each serviceUnit As DataRow In serviceUnits
            Dim code As EkCode
            code.Model = clientModelInProtocol
            code.RailSection = Integer.Parse(serviceUnit.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(serviceUnit.Field(Of String)("STATION_ORDER_CODE"))
            code.Corner = serviceUnit.Field(Of Integer)("CORNER_CODE")
            code.Unit = serviceUnit.Field(Of Integer)("UNIT_NO")
            '-------Ver0.1 次世代車補対応 MOD START-----------
            RegisterClient(code, serviceUnit.Field(Of String)("STATION_NAME"), serviceUnit.Field(Of String)("CORNER_NAME"))
            '-------Ver0.1 次世代車補対応 MOD END-------------
        Next serviceUnit

        '全クライアントの電文送受信スレッドを開始する。
        For Each oClient As Client In oClientList
            StartTelegrapher(oClient)
        Next oClient
    End Sub

    Protected Overridable Sub ProcOnTelegrapherAbort(ByVal oClient As Client)
        'NOTE: 既に停止して再起動待ちの状態（ClientState.WaitingForRestart）の
        'oClientについては、それに対するメッセージ送信を試みた際、
        '再度このメソッドが呼び出されるようになっている。

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

        If oClient.MasProDllState = ClientActiveXllState.Running Then
            Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]への配信を中止しました。")

            'DLL状態テーブルにて、現在の配信に関するoClientが示す号機への配信結果を「異常」にする。
            'NOTE: DBが同一装置内にあるため、DB接続異常等の状態依存例外も予期せぬ異常とみなす。
            '万が一発生した場合、プロセスを再起動し、その際に配信結果を「異常」にできればよい。
            TransitDllStatusToAbnormal(oCurMasProDll, oClient.Code)

            oClient.MasProDllState = ClientActiveXllState.None
            oCurMasProDll.RemainingCount -= 1
            If oCurMasProDll.RemainingCount = 0 Then
                Log.Info("適用リスト[" & oCurMasProDll.ListFileName & "]による配信を終了します。")
                oCurMasProDll = Nothing
                oMasProDllQueue.Dequeue()
                DoNextMasProDll()
            Else
                RequestMasProDllToNextClient()
            End If
        End If

        If oClient.ScheduledUllState = ClientActiveXllState.Running Then
            Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]からの収集を中止しました。")

            'oClientが示す号機からの現在の収集について、DBに異常を登録する。
            InsertScheduledUllFailureToCdt(oCurScheduledUll, oClient.Code)

            oClient.ScheduledUllState = ClientActiveXllState.None
            oCurScheduledUll.RemainingCount -= 1
            If oCurScheduledUll.RemainingCount = 0 Then
                Log.Info("データ[" & oCurScheduledUll.FileName & "]の収集を終了します。")
                oCurScheduledUll = Nothing
                oScheduledUllQueue.Dequeue()
                DoNextScheduledUll()
            Else
                RequestScheduledUllToNextClient()
            End If
        End If
    End Sub

    Protected Overridable Sub ProcOnTelegrapherRestart(ByVal oClient As Client)
    End Sub

    Protected Overridable Sub ProcOnChildSteerSockReadable(ByVal oClient As Client)
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oClient.ChildSteerSock, TelServerAppBaseConfig.TelegrapherPendingLimitTicks)
        If Not oRcvMsg.HasValue Then
            Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
            AbortTelegrapher(oClient)
            Return
        End If

        ProcOnChildMessageReceive(oClient, oRcvMsg)
    End Sub

    Protected Overridable Sub ProcOnChildMessageReceive(ByVal oClient As Client, ByVal oRcvMsg As InternalMessage)
        Select Case oRcvMsg.Kind
            Case ServerAppInternalMessageKind.MasProDllResponse
                Log.Info("MasProDllResponse received.")
                ProcOnMasProDllResponseReceive(oClient, oRcvMsg)
            Case ServerAppInternalMessageKind.ScheduledUllResponse
                Log.Info("ScheduledUllResponse received.")
                ProcOnScheduledUllResponseReceive(oClient, oRcvMsg)
        End Select
    End Sub

    Protected Overridable Sub ProcOnMasProDllResponseReceive(ByVal oClient As Client, ByVal oRcvMsg As InternalMessage)
        Debug.Assert(oClient.MasProDllState = ClientActiveXllState.Running)
        oClient.MasProDllState = ClientActiveXllState.None
        oCurMasProDll.RemainingCount -= 1
        If oCurMasProDll.RemainingCount = 0 Then
            Log.Info("適用リスト[" & oCurMasProDll.ListFileName & "]による配信を終了します。")
            oCurMasProDll = Nothing
            oMasProDllQueue.Dequeue()
            DoNextMasProDll()
        Else
            RequestMasProDllToNextClient()
        End If
    End Sub

    Protected Overridable Sub ProcOnScheduledUllResponseReceive(ByVal oClient As Client, ByVal oRcvMsg As InternalMessage)
        Debug.Assert(oClient.ScheduledUllState = ClientActiveXllState.Running)
        oClient.ScheduledUllState = ClientActiveXllState.None
        oCurScheduledUll.RemainingCount -= 1
        If oCurScheduledUll.RemainingCount = 0 Then
            Log.Info("データ[" & oCurScheduledUll.FileName & "]の収集を終了します。")
            oCurScheduledUll = Nothing
            oScheduledUllQueue.Dequeue()
            DoNextScheduledUll()
        Else
            RequestScheduledUllToNextClient()
        End If
    End Sub

    Protected Overridable Sub ProcOnMessageReceive(ByVal oMessage As Message)
        Select Case oMessage.AppSpecific
            Case ExtMasProDllRequest.FormalKind
                Log.Info("ExtMasProDllRequest received.")
                ProcOnMasProDllRequestReceive(oMessage)
            Case ExtScheduledUllRequest.FormalKind
                Log.Info("ExtScheduledUllRequest received.")
                ProcOnScheduledUllRequestReceive(oMessage)
            Case ExtServiceDateChangeNotice.FormalKind
                Log.Info("ExtServiceDateChangeNotice received.")
                ProcOnServiceDateChangeNoticeReceive(oMessage)
            Case Else
                Log.Error("Unwelcome ExtMessage received.")
        End Select
    End Sub

    Protected Overridable Sub ProcOnMasProDllRequestReceive(ByVal oMessage As Message)
        Dim oMsg As New ExtMasProDllRequest(oMessage)
        Try
            Dim sFileName As String = oMsg.ListFileName
            If Not EkMasProListFileName.IsValid(sFileName) Then
                Log.Error("The message specifies invalid file [" & sFileName & "].")
                Return
            End If
        Catch ex As Exception
            Log.Error("Exception caught on parsing the message.", ex)
            Return
        End Try

        oMasProDllQueue.Enqueue(oMsg)
        If oMasProDllQueue.Count = 1 Then
            'NOTE: 先に実行中になっている配信が無い場合である。
            DoNextMasProDll()
        End If
    End Sub

    Protected Overridable Sub ProcOnScheduledUllRequestReceive(ByVal oMessage As Message)
        Dim oMsg As New ExtScheduledUllRequest(oMessage)
        Try
            Dim sFileName As String = oMsg.FileName
            If Not EkScheduledDataFileName.IsValid(sFileName) Then
                Log.Error("The message specifies invalid file [" & sFileName & "].")
                Return
            End If
        Catch ex As Exception
            Log.Error("Exception caught on parsing the message.", ex)
            Return
        End Try

        oScheduledUllQueue.Enqueue(oMsg)
        If oScheduledUllQueue.Count = 1 Then
            'NOTE: 先に実行中になっている収集が無い場合である。
            DoNextScheduledUll()
        End If
    End Sub

    Protected Overridable Sub ProcOnServiceDateChangeNoticeReceive(ByVal oMessage As Message)
        '機器構成マスタから、現在の運用日付で運用されるべき全ての号機を検索する。
        Dim oServiceUnitTable As DataTable
        Try
            oServiceUnitTable = SelectUnitsInService(EkServiceDate.GenString())
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'ユーザが気付く場所に異常を記録する。
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtMachineMasterErratumDetected.Gen())
            Return
        End Try

        Dim serviceUnits As EnumerableRowCollection(Of DataRow) = oServiceUnitTable.AsEnumerable()

        '既に登録している号機に関して、検索の結果に含まれていない場合は、
        '当該号機用の電文送受信スレッドに終了を要求する。
        'NOTE: 強制終了した直後のものや、再起動待ちのものも、
        '終了を要求することで、終了対象号機（登録解除待ち状態）になる。
        For Each oClient As Client In oClientList
            Dim code As EkCode = oClient.Code
            Dim num As Integer = ( _
               From serviceUnit In serviceUnits _
               Where serviceUnit.Field(Of String)("RAIL_SECTION_CODE") = code.RailSection.ToString("D3") And _
                     serviceUnit.Field(Of String)("STATION_ORDER_CODE") = code.StationOrder.ToString("D3") And _
                     serviceUnit.Field(Of Integer)("CORNER_CODE") = code.Corner And _
                     serviceUnit.Field(Of Integer)("UNIT_NO") = code.Unit _
               Select serviceUnit _
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

        '終了対象号機に関して、DLL状態テーブルの現在の配信の配信結果を「異常」にする。
        'NOTE: Telegrapherが正しく終了した場合、Telegrapher自身が行うはずのことであるが、
        'Telegrapherが正しく終了したとは限らないため、ここでも行う。
        Dim dllStoppedCount As Integer = 0
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Discarded AndAlso _
               oClient.MasProDllState <> ClientActiveXllState.None Then
                Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]への配信を中止しました。")

                '当該号機に関して、DLL状態テーブルの配信結果を「異常」にする。
                'NOTE: DBが同一装置内にあるため、DB接続異常等の状態依存例外も予期せぬ異常とみなす。
                '万が一発生した場合、プロセスを再起動し、その際に配信結果を「異常」にできればよい。
                TransitDllStatusToAbnormal(oCurMasProDll, oClient.Code)

                If oClient.MasProDllState = ClientActiveXllState.Running Then
                    dllStoppedCount += 1
                Else
                    Debug.Assert(oClient.MasProDllState = ClientActiveXllState.Waiting)
                    oCurMasProDll.WaitingClients.Remove(oClient)
                End If

                oClient.MasProDllState = ClientActiveXllState.None
                oCurMasProDll.RemainingCount -= 1
            End If
        Next oClient

        '現在の収集の終了対象号機に関して、収集データ誤記テーブルに異常を登録する。
        'NOTE: Telegrapherが正しく終了した場合、Telegrapher自身が行うはずのことであるが、
        'Telegrapherが正しく終了したとは限らないため、ここでも行う。
        Dim ullStoppedCount As Integer = 0
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Discarded AndAlso _
               oClient.ScheduledUllState <> ClientActiveXllState.None Then
                Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]からの収集を中止しました。")

                '当該号機からの収集について、DBに異常を登録する。
                InsertScheduledUllFailureToCdt(oCurScheduledUll, oClient.Code)

                If oClient.ScheduledUllState = ClientActiveXllState.Running Then
                    ullStoppedCount += 1
                Else
                    Debug.Assert(oClient.ScheduledUllState = ClientActiveXllState.Waiting)
                    oCurScheduledUll.WaitingClients.Remove(oClient)
                End If

                oClient.ScheduledUllState = ClientActiveXllState.None
                oCurScheduledUll.RemainingCount -= 1
            End If
        Next oClient

        '登録解除する。
        UnregisterDiscardedClients()

        '配信を失敗させた分に見合う新たな配信を開始する。
        If oCurMasProDll IsNot Nothing AndAlso oCurMasProDll.RemainingCount = 0 Then
            Log.Info("適用リスト[" & oCurMasProDll.ListFileName & "]による配信を終了します。")
            oCurMasProDll = Nothing
            oMasProDllQueue.Dequeue()
            DoNextMasProDll()
        Else
            While dllStoppedCount > 0
                RequestMasProDllToNextClient()
                dllStoppedCount -= 1
            End While
        End If

        '収集を失敗させた分に見合う新たな収集を開始する。
        If  oCurScheduledUll IsNot Nothing AndAlso oCurScheduledUll.RemainingCount = 0 Then
            Log.Info("データ[" & oCurScheduledUll.FileName & "]の収集を終了します。")
            oCurScheduledUll = Nothing
            oScheduledUllQueue.Dequeue()
            DoNextScheduledUll()
        Else
            While ullStoppedCount > 0
                RequestScheduledUllToNextClient()
                dllStoppedCount -= 1
            End While
        End If

        '検索で得た駅務機器識別コードに関して、登録されていないものは、登録する。
        '登録されているものは、駅名やコーナー名が更新されていないかチェックし、
        '更新されていたら、新しい駅名とコーナー名を通知する。
        For Each row As DataRow In oServiceUnitTable.Rows
            Dim code As EkCode
            code.Model = clientModelInProtocol
            code.RailSection = Integer.Parse(row.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(row.Field(Of String)("STATION_ORDER_CODE"))
            code.Corner = row.Field(Of Integer)("CORNER_CODE")
            code.Unit = row.Field(Of Integer)("UNIT_NO")
            '-------Ver0.1 次世代車補対応 MOD START-----------
            Dim oClient As Client = FindClient(code)
            Dim sStationName As String = row.Field(Of String)("STATION_NAME")
            Dim sCornerName As String = row.Field(Of String)("CORNER_NAME")
            If oClient Is Nothing Then
                RegisterClient(code, sStationName, sCornerName)
            Else
                If Not oClient.StationName.Equals(sStationName) OrElse _
                   Not oClient.CornerName.Equals(sCornerName) Then
                    'NOTE: SendToTelegrapherでTelegrapherをAbortさせることになった場合に
                    'リスタート時に新しい名前を渡せるよう、この時点でClientオブジェクトの
                    '内容を書き換えておく。
                    oClient.StationName = sStationName
                    oClient.CornerName = sCornerName

                    Dim oExt As New NameChangeNoticeExtendPart()
                    oExt.StationName = sStationName
                    oExt.CornerName = sCornerName

                    Log.Info("Sending NameChange notice to telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
                    SendToTelegrapher(oClient, NameChangeNotice.Gen(oExt))
                End If
            End If
            '-------Ver0.1 次世代車補対応 MOD END-------------
        Next row

        '登録した号機の電文送受信スレッドを開始させる。
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Registered Then
                StartTelegrapher(oClient)
            End If
        Next oClient
    End Sub

    'NOTE: MayOverride
    Protected Overridable Sub ProcOnAccept(ByVal oNewSocket As Socket)
    End Sub

    'NOTE: 先に実行している配信が無い場合のみ呼び出される。
    'ただし、次に行うべき配信があるとは限らないものとする。
    Protected Overridable Sub DoNextMasProDll()
        While oMasProDllQueue.Count <> 0
            Dim oMsg As ExtMasProDllRequest = oMasProDllQueue.Peek()
            Dim sListFileName As String = oMsg.ListFileName
            Log.Info("適用リスト[" & sListFileName & "]による配信を開始します。")

            'NOTE: DBが同一装置内にあるため、DB接続異常等の状態依存例外も予期せぬ異常とみなす。
            '万が一発生した場合、プロセスを再起動し、その際に配信結果を「異常」にできればよい。

            Dim oDll As New MasProDllInfo()
            oDll.DataApplicableModel = EkMasProListFileName.GetDataApplicableModel(sListFileName)
            oDll.DataPurpose = EkMasProListFileName.GetDataPurpose(sListFileName)
            oDll.DataKind = EkMasProListFileName.GetDataKind(sListFileName)
            oDll.DataSubKind = EkMasProListFileName.GetDataSubKind(sListFileName)
            oDll.DataVersion = EkMasProListFileName.GetDataVersion(sListFileName)
            oDll.ListVersion = EkMasProListFileName.GetListVersion(sListFileName)
            oDll.DataFileName = SelectDllDataFileName(oDll)
            oDll.ListFileName = sListFileName
            oDll.DataFileHashValue = SelectDllDataFileHashValue(oDll)
            oDll.ListFileHashValue = SelectDllListFileHashValue(oDll)

            '直接の配信先となる号機の駅務機器識別コードを検索する。
            'また、各号機に関するデータ本体の最終送信バージョンも取得する。
            Dim agents As DataRowCollection = SelectDllAgentUnits(oDll).Rows
            Dim oList As New List(Of Client)(agents.Count)
            For Each agent As DataRow In agents
                Dim code As EkCode
                code.Model = clientModelInProtocol
                code.RailSection = Integer.Parse(agent.Field(Of String)("RAIL_SECTION_CODE"))
                code.StationOrder = Integer.Parse(agent.Field(Of String)("STATION_ORDER_CODE"))
                code.Corner = agent.Field(Of Integer)("CORNER_CODE")
                code.Unit = agent.Field(Of Integer)("UNIT_NO")

                'NOTE: あるagentへ送信を行う際、どんなことがあっても（たとえば、
                'agentと既に不通であったとしても）、送信結果の確定によって
                'UNCERTAIN_FLGをクリアするよりも前に、ここを実行する（ここで
                'UNCERTAIN_FLGをDATA_VERSIONに反映させる）ものとする。

                'OPT: 最適化。DBに接続したまま、すべてを行うように。
                UpdateLastSendVerByUncertainFlag(oDll, code)

                Dim sendSuite As Boolean = False
                If oMsg.ForcingFlag = True Then
                    sendSuite = True
                Else
                    'OPT: ExecuteSQLToReadScalarを使う。
                    Dim lastSendVer As DataRowCollection = SelectLastSendVer(oDll, code).Rows
                    If lastSendVer.Count = 0 OrElse
                       Not lastSendVer(0).Field(Of String)("DATA_VERSION").Equals(oDll.DataVersion) Then
                        sendSuite = True
                    End If
                End If

                Dim oClient As Client = FindClient(code)
                If oClient Is Nothing Then
                    Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]は機器構成に登録されていません。")

                    'DLL状態テーブルにて、当該の配信に関する当該の号機への配信結果を「異常」にする。
                    TransitDllStatusToAbnormal(oDll, code)
                Else
                    If sendSuite Then
                        Log.Info("[" & oClient.Code.ToString(EkCodeOupFormat) & "]へは適用リストとデータ本体を配信します。")
                    Else
                        Log.Info("[" & oClient.Code.ToString(EkCodeOupFormat) & "]へは適用リストを配信します。")
                    End If
                    oClient.MasProDllState = ClientActiveXllState.Waiting
                    oClient.SendSuiteOnMasProDll = sendSuite
                    oList.Add(oClient)
                End If
            Next agent

            If oList.Count <> 0 Then
                oDll.RemainingCount = oList.Count
                oDll.WaitingClients = oList
                oCurMasProDll = oDll
                For count As Integer = 0 To TelServerAppBaseConfig.ConcurrentMasProDllMaxCount - 1
                    RequestMasProDllToNextClient()
                Next
                Exit While
            Else
                Log.Info("適用リスト[" & sListFileName & "]による配信を終了します。")
                oMasProDllQueue.Dequeue()
            End If
        End While
    End Sub

    'NOTE: 先に実行している収集が無い場合のみ呼び出される。
    'ただし、次に行うべき収集があるとは限らないものとする。
    Protected Overridable Sub DoNextScheduledUll()
        While oScheduledUllQueue.Count <> 0
            Dim oMsg As ExtScheduledUllRequest = oScheduledUllQueue.Peek()
            Dim sFileName As String = oMsg.FileName
            Log.Info("データ[" & sFileName & "]の収集を開始します。")

            Dim oUll As New ScheduledUllInfo()
            oUll.FileName = sFileName

            Dim oList As New List(Of Client)(oClientList.Count)
            For Each oClient As Client In oClientList
                oClient.ScheduledUllState = ClientActiveXllState.Waiting
                oList.Add(oClient)
            Next oClient

            If oList.Count <> 0 Then
                oUll.RemainingCount = oList.Count
                oUll.WaitingClients = oList
                oCurScheduledUll = oUll
                For count As Integer = 0 To TelServerAppBaseConfig.ConcurrentScheduledUllMaxCount - 1
                    RequestScheduledUllToNextClient()
                Next
                Exit While
            Else
                Log.Info("データ[" & sFileName & "]の収集を終了します。")
                oScheduledUllQueue.Dequeue()
            End If
        End While
    End Sub

    'NOTE: oCurMasProDllが続行される（RemainingCountが0でない）場合のみ呼び出される。
    'ただし、Waiting状態のClientが残っているとは限らないものとする。
    Protected Overridable Sub RequestMasProDllToNextClient()
        If oCurMasProDll.WaitingClients.Count = 0 Then Return

        Dim oClient As Client = oCurMasProDll.WaitingClients(0)
        Debug.Assert(oClient.MasProDllState = ClientActiveXllState.Waiting)

        'NOTE: oClientからの能動的収集を実行中の場合は、通信で待つことになって
        'しまうし、本来、この時点で内部メッセージを送信する権利があるのは
        '電文送受信スレッド側ということになるため、別のClientへの配信を
        '先に行う方がよいかもしれない。

        Log.Info("[" & oClient.Code.ToString(EkCodeOupFormat) & "]への配信を行います。")
        oCurMasProDll.WaitingClients.RemoveAt(0)
        oClient.MasProDllState = ClientActiveXllState.Running

        Dim oExt As New MasProDllRequestExtendPart()
        oExt.ListFileName = oCurMasProDll.ListFileName
        oExt.ListFileHashValue = oCurMasProDll.ListFileHashValue
        If oClient.SendSuiteOnMasProDll Then
            oExt.DataFileName = oCurMasProDll.DataFileName
            oExt.DataFileHashValue = oCurMasProDll.DataFileHashValue
        Else
            oExt.DataFileName = Nothing
            oExt.DataFileHashValue = Nothing
        End If

        Log.Info("Sending MasProDll request to telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
        SendToTelegrapher(oClient, MasProDllRequest.Gen(oExt))
    End Sub

    'NOTE: oScheduledUllQueueが続行される（RemainingCountが0でない）場合のみ呼び出される。
    'ただし、Waiting状態のClientが残っているとは限らないものとする。
    Protected Overridable Sub RequestScheduledUllToNextClient()
        If oCurScheduledUll.WaitingClients.Count = 0 Then Return

        Dim oClient As Client = oCurScheduledUll.WaitingClients(0)
        Debug.Assert(oClient.ScheduledUllState = ClientActiveXllState.Waiting)

        'NOTE: oClientへの能動的配信を実行中の場合は、通信で待つことになって
        'しまうし、この時点で内部メッセージを送信する権利があるのは
        '電文送受信スレッド側ということになってしまうため、別のClientからの
        '収集を先に行う方がよいかもしれない。

        Log.Info("[" & oClient.Code.ToString(EkCodeOupFormat) & "]からの収集を行います。")
        oCurScheduledUll.WaitingClients.RemoveAt(0)
        oClient.ScheduledUllState = ClientActiveXllState.Running

        Dim oExt As New ScheduledUllRequestExtendPart()
        oExt.FileName = oCurScheduledUll.FileName

        Log.Info("Sending ScheduledUll request to telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
        SendToTelegrapher(oClient, ScheduledUllRequest.Gen(oExt))
    End Sub
#End Region

End Class
