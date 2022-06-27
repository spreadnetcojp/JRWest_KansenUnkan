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
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 対運管端末通信プロセスのメイン処理およびTelegrapher管理処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "内部クラス"
    Private Enum ClientState
        Registered
        QuitRequested
        Discarded
    End Enum

    Private Class Client
        Public State As ClientState
        Public Name As String
        Public Telegrapher As MyTelegrapher
        Public ChildSteerSock As Socket
    End Class
#End Region

#Region "定数や変数"
    '電文送受信スレッドのAbort応答期限
    Private Const TelegrapherAbortLimitTicks As Integer = 5000

    '電文書式
    Private Shared oTelegGene As EkTelegramGene

    '電文取り込み器
    Private Shared oTelegImporter As EkTelegramImporter

    '本プロセスがFTPで公開するディレクトリのローカルパス
    Private Shared sFtpBase As String

    'クライアントのリスト
    Private Shared oClientList As LinkedList(Of Client)

    'メインウィンドウ
    Private Shared oMainForm As ServerAppForm

    '通信管理スレッドへの終了要求フラグ
    Private Shared quitListener As Integer
#End Region

    ''' <summary>
    ''' 対運管端末通信プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 対運管端末通信プロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppToOpClient")
        If m.WaitOne(0, False) Then
            Try
                Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
                If sLogBasePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
                    Return
                End If

                Dim sIniFilePath As String = Constant.GetEnv(REG_SERVER_INI)
                If sIniFilePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_SERVER_INI)
                    Return
                End If

                Log.Init(sLogBasePath, "ToOpClient")
                Log.Info("プロセス開始")

                Try
                    Lexis.Init(sIniFilePath)
                    Config.Init(sIniFilePath)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End Try

                Log.SetKindsMask(Config.LogKindsMask)

                LocalConnectionProvider.Init()

                oTelegGene = New EkTelegramGeneForNativeModels(Config.FtpServerRootDirPath)
                oTelegImporter = New EkTelegramImporter(oTelegGene)
                oClientList = New LinkedList(Of Client)

                'メッセージループがアイドル状態になる前（かつ、定期的にそれを行う
                'スレッドを起動する前）に、生存証明ファイルを更新しておく。
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '通信管理スレッドを開始する。
                Dim oListenerThread As New Thread(AddressOf MainClass.ListeningLoop)
                Log.Info("Starting the listener thread...")
                quitListener = 0
                oListenerThread.Name = "Listener"
                oListenerThread.Start()

                'ウインドウプロシージャを実行する。
                'NOTE: このメソッドから例外がスローされることはない。
                ServerAppBaseMain(oMainForm)

                Try
                    '通信管理スレッドに終了を要求する。
                    Log.Info("Sending quit request to the listener thread...")
                    Thread.VolatileWrite(quitListener, 1)

                    'NOTE: 以下で通信管理スレッドが終了しない場合、
                    '通信管理スレッドは生存証明を行わないはずであり、
                    '状況への対処はプロセスマネージャで行われる想定である。

                    '通信管理スレッドの終了を待つ。
                    Log.Info("Waiting for the listener thread to quit...")
                    oListenerThread.Join()
                    Log.Info("The listener thread has quit.")
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oListenerThread.Abort()
                End Try
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                If oMainForm IsNot Nothing Then
                    oMainForm.Dispose()
                End If
                LocalConnectionProvider.Dispose()
                Config.Dispose()
                Log.Info("プロセス終了")

                'NOTE: ここを通らなくても、このスレッドの消滅とともに解放される
                'ようなので、最悪の心配はない。
                m.ReleaseMutex()

                Application.Exit()
            End Try
        End If
    End Sub

    ''' <summary>
    ''' 通信管理スレッドのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' リスニングソケットの制御およびTelegrapherの管理を行う。
    ''' </remarks>
    Private Shared Sub ListeningLoop()
        Dim oListenerSock As Socket = Nothing  'リスニングソケット

        Try
            Log.Info("The listener thread started.")
            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            Dim oCheckReadList As New ArrayList()

            '各電文送受信スレッドの一時作業用ディレクトリを親ディレクトリごとまとめて削除する。
            Log.Info("Sweeping directory [" & Config.TemporaryBaseDirPath & "]...")
            Utility.DeleteTemporalDirectory(Config.TemporaryBaseDirPath)

            '各電文送受信スレッドのFTPサイト用ディレクトリやその内容物を削除する。
            'NOTE: このディレクトリは、このプロセスだけでなく、FTPサーバも参照・操作し得る。
            '既に存在しているものの削除に失敗する場合は、転送の終了を認識していないFTPサーバが
            '書き込みで握っているケースと考えられるが、該当するサブディレクトリやファイルのみを
            '残して処理を強行する。なお、処理を強行せずにこのプロセスを異常終了させるとしても、
            'プロセスマネージャがこのプロセスを起動する次の機会にFTPサーバがガードを解いて
            'いれば、そこから正常動作が始まるため、問題はないはずである。アプリ再起動の
            'この機会を逃さずに、全ての一時ファイルを削除するという意味では、その方が理想的で
            'あるかもしれないが、とりあえず可用性を優先して、このようにしている。
            sFtpBase = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, Config.PermittedPathInFtp)
            If Directory.Exists(sFtpBase) Then
                Log.Info("Cleaning up directory [" & sFtpBase & "]...")
                Utility.CleanUpDirectory(sFtpBase)
            End If

            'マスタ/プログラムの管理ディレクトリがなければ、作成しておく。
            Directory.CreateDirectory(Config.MasProDirPath)

            'リッスンを開始する。
            Log.Info("Start listening for [" & Config.IpAddrForTelegConnection.ToString() & ":" & Config.IpPortForTelegConnection.ToString() & "].")
            oListenerSock = SockUtil.StartListener(Config.IpAddrForTelegConnection, Config.IpPortForTelegConnection)

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Thread.VolatileRead(quitListener) = 0
                Dim oSocket As Socket = Nothing

                'ソケット読み出し監視＆監視結果取得用のリストを作成する。
                oCheckReadList.Clear()
                oCheckReadList.Add(oListenerSock)

                'リスニングソケットが読み出し可能になるまで所定時間待機する。
                Socket.Select(oCheckReadList, Nothing, Nothing, Config.PollIntervalTicks * 1000)

                'リスニングソケットが読み出し可能になった場合は、送受信用ソケットを取り出す。
                If oCheckReadList.Count > 0 Then
                    Try
                        oSocket = SockUtil.Accept(oListenerSock)
                    Catch ex As OPMGException
                        'NOTE: 実際のところはともかく、リスニングソケットが読み出し可能
                        'になったからといって、そこからのAccept()が成功するとは限らない
                        '（linuxのソケットのように、Accept()を呼び出すまでの間に発生
                        'したコネクションの異常が、Accept()で通知される可能性もある）
                        'ものとみなす。
                        Log.Error("Exception caught.", ex)
                    End Try
                End If

                '送受信用ソケットを取り出した場合、電文送受信スレッドを作成して渡す。
                If oSocket IsNot Nothing Then
                    RegisterClient(oSocket)
                End If

                '前回チェックから所定時間経過している場合は、全ての
                '電文送受信スレッドについて、異常終了またはフリーズ
                'していないか、あるいは終了要求待ち（切断済み）に
                'なっていないかをチェックする。
                'NOTE: 新しいコネクションが作られた（oSocket IsNot Nothing
                'である）場合は、同一端末の過去のコネクションが切断済みに
                'なっている可能性も高いため、所定時間が経過していなくても、
                'チェックを行うことにしている。
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oSocket IsNot Nothing OrElse _
                   oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()

                    'Log.Info("Checking pulse of all telegraphers...")
                    For Each oClient As Client In oClientList
                        If oClient.Telegrapher.ThreadState = ThreadState.Stopped Then
                            '予期せぬ例外などで異常終了している場合である。
                            Log.Fatal("Sweeping telegrapher [" & oClient.Name & "] because it stopped...")
                            SweepBrokenTelegrapher(oClient)
                        ElseIf TickTimer.GetTickDifference(systemTick, oClient.Telegrapher.LastPulseTick) > Config.TelegrapherPendingLimitTicks Then
                            'フリーズしている場合である。
                            Log.Fatal("Sweeping telegrapher [" & oClient.Name & "] because it seems broken...")
                            SweepBrokenTelegrapher(oClient)
                        ElseIf oClient.Telegrapher.LineStatus = LineStatus.Disconnected Then
                            'Telegrapherが端末とのコネクションを切断して、
                            '親スレッドからの終了要求を待っている場合である。

                            'NOTE: Telegrapherの正しい終了シーケンスは、
                            'これである。Telegrapherが勝手に終了する仕様は
                            'あり得ない。Telegrapherが正常系で勝手に終了する
                            'となると、プロセス終了時にTelegrapherに終了要求を
                            '送信する際、正常系であるにも関わらず、ブロック
                            'されることを通常のケースとして想定しなければ
                            'ならなくなるためである。

                            '電文送受信スレッドに終了を要求する。
                            QuitTelegrapher(oClient)
                        End If
                    Next oClient

                    '終了を要求した電文送受信スレッドの終了を待つ。
                    WaitForTelegraphersToQuit()

                    '不要になったクライアントを登録解除する。
                    UnregisterDiscardedClients()
                End If
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP発生（または収集データ誤記テーブルへの登録）は、
            'プロセスマネージャが行うので、ここでは不要である。

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        Finally
            If oClientList IsNot Nothing
                '残っているクライアントの電文送受信スレッドに終了を要求する。
                'NOTE: ここでは、対電文送受信スレッド通信用ソケットや
                '電文送受信スレッドを作成した後、電文送受信スレッドを
                'スタートさせる前に例外が発生した場合や、
                'スタート後の電文送受信スレッドがAbortしている場合など
                'を考慮した実装を行っている。
                For Each oClient As Client In oClientList
                    Dim clientThreadState As ThreadState = oClient.Telegrapher.ThreadState
                    If oClient.ChildSteerSock IsNot Nothing AndAlso _
                       oClient.State = ClientState.Registered AndAlso _
                       oClient.Telegrapher.ThreadState <> ThreadState.Unstarted Then
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
                Log.Info("End listening for [" & Config.IpAddrForTelegConnection.ToString() & ":" & Config.IpPortForTelegConnection.ToString() & "].")
            End If
        End Try
    End Sub

    Private Shared Function FindClient(ByVal sName As String) As Client
        For Each oClient As Client In oClientList
            If oClient.Name = sName Then Return oClient
        Next oClient
        Return Nothing
    End Function

    Private Shared Sub RegisterClient(ByVal oSocket As Socket)
        Dim oClient As New Client()
        Dim oRemoteEndPoint As IPEndPoint = DirectCast(oSocket.RemoteEndPoint, IPEndPoint)
        oClient.Name = oRemoteEndPoint.Address.ToString() & "." & oRemoteEndPoint.Port.ToString()
        Log.Info("Incoming from [" & oClient.Name & "].")

        Dim oRcvTeleg As EkDodgyTelegram _
           = oTelegImporter.GetTelegramFromSocket(oSocket, Config.TelegReadingLimitBaseTicks, Config.TelegReadingLimitExtraTicksPerMiB, Config.TelegLoggingMaxLengthOnRead)
        If oRcvTeleg Is Nothing Then
            Log.Info("Closing the connection...")
            oSocket.Close()
            Return
        End If

        Dim headerViolation As NakCauseCode = oRcvTeleg.GetHeaderFormatViolation()
        If headerViolation <> EkNakCauseCode.None Then
            Log.Error("Telegram with invalid HeadPart received.")
            SendNakTelegramThenDisconnect(headerViolation, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.CmdCode <> EkCmdCode.Req Then
            Log.Error("Telegram with invalid CmdCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.SubCmdCode <> EkSubCmdCode.Get Then
            Log.Error("Telegram with invalid SubCmdCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.ObjCode <> EkComStartReqTelegram.FormalObjCodeInOpClient Then
            Log.Error("Telegram with invalid ObjCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        Dim oRcvComStartReqTeleg As New EkComStartReqTelegram(oRcvTeleg)
        Dim bodyViolation As NakCauseCode = oRcvComStartReqTeleg.GetBodyFormatViolation()
        If bodyViolation <> EkNakCauseCode.None Then
            Log.Error("ComStart REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(bodyViolation, oRcvComStartReqTeleg, oSocket)
            Return
        End If

        Dim clientCode As EkCode = oRcvComStartReqTeleg.ClientCode
        'NOTE: clientCodeが規定範囲内かチェックする方がよいかもしれない。

        Log.Info("ComStart REQ received.")

        Dim oldClient As Client = FindClient(oClient.Name)
        If oldClient IsNot Nothing Then
            Log.Warn("Telegrapher [" & oClient.Name & "] is running...")
            QuitTelegrapher(oldClient)
            WaitForTelegraphersToQuit()
            UnregisterDiscardedClients()
        End If

        '当該クライアント向け電文送受信スレッドの一時作業用ディレクトリのパスを生成。
        Dim sClientTempBase As String = Path.Combine(Config.TemporaryBaseDirPath , oClient.Name)

        '当該クライアント向けのFTPサイト用ディレクトリのパスを生成。
        Dim sClientFtpBase As String = Path.Combine(sFtpBase, oClient.Name)

        '同名のFTPサイト用ディレクトリが存在している場合は削除する。
        Dim isDirLocked As Boolean = False
        Log.Info("Initializing directory [" & sClientFtpBase & "]...")
        Try
            Directory.Delete(sClientFtpBase, True)
        Catch ex As DirectoryNotFoundException

        Catch ex As IOException
            Log.Error("Exception caught.", ex)
            isDirLocked = True
        Catch ex As UnauthorizedAccessException
            Log.Error("Exception caught.", ex)
            isDirLocked = True
        End Try

        '同名のFTPサイト用ディレクトリを削除できなかった（まだFTPサーバが握っている）
        '場合は、NAK（ビジー）電文を返信する。
        If isDirLocked Then
            SendNakTelegramThenDisconnect(EkNakCauseCode.Busy, oRcvComStartReqTeleg, oSocket)
            Return
        End If

        '当該クライアント向けのFTPサイト用ディレクトリを作成する。
        Directory.CreateDirectory(sClientFtpBase)

        'ACK電文を返信する。
        Dim oReplyTeleg As EkComStartAckTelegram = oRcvComStartReqTeleg.CreateAckTelegram()
        Log.Info("Sending ComStart ACK...")
        If SendReplyTelegram(oSocket, oReplyTeleg, oRcvComStartReqTeleg) = False Then
            Log.Info("Closing the connection...")
            oSocket.Close()
            Return
        End If

        oClient.ChildSteerSock = Nothing
        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oClient.ChildSteerSock, oChildSock)
        oClient.Telegrapher = New MyTelegrapher( _
           oClient.Name, _
           oChildSock, _
           oTelegImporter, _
           oTelegGene, _
           clientCode, _
           sClientTempBase, _
           sClientFtpBase)

        oClientList.AddLast(oClient)
        oClient.State = ClientState.Registered

        Log.Info("Starting telegrapher [" & oClient.Name & "]...")
        oClient.Telegrapher.Start()

        Log.Info("Sending new socket to telegrapher [" & oClient.Name & "]...")
        If ConnectNotice.Gen(oSocket).WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("Sweeping telegrapher [" & oClient.Name & "] because it seems broken...")
            SweepBrokenTelegrapher(oClient)
        End If
    End Sub

    Private Shared Sub SendNakTelegramThenDisconnect(ByVal cause As NakCauseCode, ByVal oSourceTeleg As EkTelegram, ByVal oSocket As Socket)
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

    Private Shared Function SendReplyTelegram(ByVal oSocket As Socket, ByVal oReplyTeleg As EkTelegram, ByVal oSourceTeleg As EkTelegram) As Boolean
        oReplyTeleg.RawReqNumber = oSourceTeleg.RawReqNumber
        oReplyTeleg.RawClientCode = oSourceTeleg.RawClientCode
        Return oReplyTeleg.WriteToSocket(oSocket, Config.TelegWritingLimitBaseTicks, Config.TelegWritingLimitExtraTicksPerMiB, Config.TelegLoggingMaxLengthOnWrite)
    End Function

    Private Shared Sub QuitTelegrapher(ByVal oClient As Client)
        Log.Info("Sending quit request to telegrapher [" & oClient.Name & "]...")
        If QuitRequest.Gen().WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("Sweeping the telegrapher because it seems broken...")
            SweepBrokenTelegrapher(oClient)
        Else
            oClient.State = ClientState.QuitRequested
        End If
    End Sub

    Private Shared Sub WaitForTelegraphersToQuit()
        'Log.Info("Waiting for telegraphers to quit...")
        Dim oJoinLimitTimer As New TickTimer(Config.TelegrapherPendingLimitTicks)
        oJoinLimitTimer.Start(TickTimer.GetSystemTick())
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.QuitRequested Then
                Dim ticks As Long = oJoinLimitTimer.GetTicksToTimeout(TickTimer.GetSystemTick())
                If ticks < 0 Then ticks = 0

                If oClient.Telegrapher.Join(CInt(ticks)) = False Then
                    Log.Fatal("Sweeping telegrapher [" & oClient.Name & "] because it seems broken...")
                    SweepBrokenTelegrapher(oClient)
                Else
                    Log.Info("Telegrapher [" & oClient.Name & "] has quit.")
                    oClient.ChildSteerSock.Close()
                    oClient.State = ClientState.Discarded
                End If
            End If
        Next oClient
    End Sub

    Private Shared Sub SweepBrokenTelegrapher(ByVal oClient As Client)
        oClient.ChildSteerSock.Close()
        If oClient.Telegrapher.ThreadState <> ThreadState.Stopped Then
            oClient.Telegrapher.Abort()
            If oClient.Telegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                Log.Warn("The telegrapher may refuse to abort.")
            End If
        End If
        oClient.State = ClientState.Discarded
    End Sub

    Private Shared Sub UnregisterDiscardedClients()
        Dim oNode As LinkedListNode(Of Client) = oClientList.First
        While oNode IsNot Nothing
            Dim oClient As Client = oNode.Value
            If oClient.State = ClientState.Discarded Then
                Dim oDiscardedNode As LinkedListNode(Of Client) = oNode
                oNode = oNode.Next
                oClientList.Remove(oDiscardedNode)
                Log.Info("Telegrapher [" & oClient.Name & "] unregistered.")

                'NOTE: FTPサーバは、ServerTelegrpherと無関係に動いている。たとえば、
                'ServerTelegrpherが転送終了電文の受信でタイムアウトした場合、
                'クライアント側が通信手順に違反していなくても、そのときに行っていた
                'ファイル転送は、まだ継続している可能性がある。ServerTelegrapherが
                '電文のコネクションを切断し、それをClientTelegrapherが検知して
                'FTPを中止することはあるかもしれないが、クライアント側の作り次第で
                'あるし、仮に中止するとしても、いつ検知して中止するかは全くわからない。
                '以上のとおりであるから、ここでFTPサーバの当該クライアント用
                'ディレクトリを削除することはしない。
                '基本的に、運管端末の起動は、ユーザの操作に応じて行われることで
                'あるから、作られるディレクトリの数も知れており、プロセスの再起動時に
                '削除すれば十分...という思想である。
            Else
                oNode = oNode.Next
            End If
        End While
    End Sub

End Class
