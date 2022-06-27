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

Imports System.Net.Sockets
Imports System.Threading

''' <summary>
''' 親スレッドからの要求や他装置からの電文を待つスレッドの基本クラス。
''' </summary>
Public Class Looper

#Region "定数や変数"
    '親スレッドメッセージ受信（及び応答返信）用ソケット
    Protected oParentMessageSock As Socket

    '動作中タイマ管理リスト
    Private oTimerList As LinkedList(Of TickTimer)

    '受信監視ソケット管理リスト
    Private oSockList As ArrayList

    'スレッド
    Private oThread As Thread
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket)
        Me.oParentMessageSock = oParentMessageSock
        Me.oTimerList = New LinkedList(Of TickTimer)
        Me.oSockList = New ArrayList()
        Me.oThread = New Thread(AddressOf Me.TaskLooper)
        Me.oThread.Name = sThreadName

        'NOTE: 現況は、Looperの各インスタンスをスレッドとするスレッドモデルを
        '採用している。もし、プロセスモデルとする場合は、多少の変更が必要。
        'まず、引数からoParentMessageSockを取得することはできないため、ここで自ら
        'ソケットを作成して、ローカルホストの所定の（引数で通知された？）
        'ポートにConnectすることになるはずである。加えて、親プロセスのみが
        'このプロセスを作る（1つのLooperを作るごとに必ずAcceptの完了まで待つ）
        'ような設計にしない（ユーザが直接exeを起動することも可能にする）ので
        'あれば、親プロセスが配下の（ローカル接続した）各Looperを区別できる
        'ようにするために、Looperは、Connect完了後、プロセス名等を記述した
        '開始通知を親プロセスに送信する必要もあると思われる。

        Me.RegisterSocket(Me.oParentMessageSock)
    End Sub
#End Region

#Region "親スレッド用メソッド"
    'メモリバリアになります。
    Public Overridable Sub Start()
        oThread.Start()
    End Sub

    Public Overridable Sub Join()
        oThread.Join()
    End Sub

    Public Overridable Function Join(ByVal millisecondsTimeout As Integer) As Boolean
        Return oThread.Join(millisecondsTimeout)
    End Function

    Public Overridable Sub Abort()
        oThread.Abort()
    End Sub

    Public ReadOnly Property ThreadState() As ThreadState
        Get
            Return oThread.ThreadState
        End Get
    End Property
#End Region

#Region "サブクラス実装用メソッド"
    Protected Sub RegisterTimer(ByVal oTimer As TickTimer, ByVal systemTick As Long)
        oTimer.Start(systemTick)
        If Not oTimerList.Contains(oTimer) Then
            oTimerList.AddLast(oTimer)
        End If
    End Sub

    Protected Sub UnregisterTimer(ByVal oTimer As TickTimer)
        oTimer.Terminate()
        oTimerList.Remove(oTimer)
    End Sub

    Protected Sub RegisterSocket(ByVal oSock As Socket)
        If Not oSockList.Contains(oSock) Then
            oSockList.Add(oSock)
        End If
    End Sub

    Protected Sub UnregisterSocket(ByVal oSock As Socket)
        oSockList.Remove(oSock)
    End Sub
#End Region

#Region "イベント処理メソッド（仮想）"
    Protected Overridable Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        Return True
    End Function

    Protected Overridable Function ProcOnSockReadable(ByVal oSock As Socket) As Boolean
        Return True
    End Function

    Protected Overridable Sub ProcOnUnhandledException(ByVal ex As Exception)
        If ex.GetType() IsNot GetType(ThreadAbortException) Then
            'NOTE: このメソッドをオーバライドしなければ、以下のとおり、exを
            'Catchしない場合と同等となります（プロセス全体が終了します）。
            'よって、当該Looperのスレッドのみを終了させたい場合は、
            'サブクラスでオーバライドすることを推奨します。そうする場合の
            '妥当な実装は、例外の発生し得ない必要最小限のことを行い、そのまま
            'メソッドを終了するなどです。このメソッドから戻れば、当該
            'LooperのスレッドのみがThreadState.Stoppedに遷移するはずです。
            '親スレッドは任意周期で各LooperのThreadStateプロパティを監視する
            '等により、そのことを検知可能です。
            'なお、各Looperにおける処理継続対象外の例外発生だけでなく、
            '各Looperのフリーズ等も監視したいのであれば、各Looperで
            '所定より短い周期で所定のプロパティを更新するようにし、親スレッド
            'は、ThreadStateプロパティではなく、それを監視するのが妥当です。
            Throw ex
        End If
    End Sub
#End Region

#Region "中核処理"
    Private Function FindTimeoutTimer(ByVal systemTick As Long) As TickTimer
        'NOTE: minTicksの初期値は理論上は0にするべきだが、
        'Select等がタイムアウトするタイミングに誤差がある場合の
        '性能を考慮し、1ms未来にタイムアウトするべきタイマで
        'あってもタイムアウトと判定する。
        Dim minTicks As Long = 1
        Dim oFoundTimer As TickTimer = Nothing
        For Each oTimer As TickTimer In oTimerList
            Dim ticks As Long = oTimer.GetTicksToTimeout(systemTick)
            If ticks < minTicks Then
                minTicks = ticks
                oFoundTimer = oTimer
            End If
        Next oTimer
        Return oFoundTimer
    End Function

    'NOTE: 動作しているタイマが無い場合は、InfiniteTicksを返却する。
    Private Function GetTicksToNextTimeout(ByVal systemTick As Long) As Long
        Dim minTicks As Long = TickTimer.InfiniteTicks
        For Each oTimer As TickTimer In oTimerList
            Dim ticks As Long = oTimer.GetTicksToTimeout(systemTick)
            If ticks < minTicks Then
                minTicks = ticks
            End If
        Next oTimer
        Return minTicks
    End Function

    Private Sub TaskLooper()
        Log.Info("The thread started.")
        Try
            Do
                Dim systemTick As Long = TickTimer.GetSystemTick()

                'タイムアウトしているタイマがあれば、
                '該当するメソッドを実行して、イベントの検索に戻る。
                Dim oTimer As TickTimer = FindTimeoutTimer(systemTick)
                If oTimer IsNot Nothing Then
                    UnregisterTimer(oTimer)
                    '該当するメソッドを実行する。
                    Dim toBeContinued As Boolean = ProcOnTimeout(oTimer)
                    'メソッドがスレッドを終了すべきと判断した場合は、スレッドを終了する。
                    If Not toBeContinued Then
                        Return
                    End If
                    'イベントの検索に戻る。
                    Continue Do
                End If

                '次のタイムアウトまでの時間を取得する。
                'タイムアウトしているタイマがないケースであるため、
                'ここで得られる時間は必ず1以上である。
                Dim ticks As Long = GetTicksToNextTimeout(systemTick)
                Debug.Assert(ticks > 0)

                If oSockList.Count <> 0 Then
                    '時間の単位を変換する。
                    'NOTE: 本当は、ticksがTickTimer.InfiniteTicksのケースでは、
                    'Socket.Selectに「-1」を渡すようにして、「無期限待機」を
                    'したい。しかし、.NET Framework 3.5のSocket.Selectには
                    'バグがあり、「-1」を指定した場合に即時復帰するようである
                    'ため、できるだけ長い時間（Integer.MaxValue）を指定した
                    '期限付きの待機にしておく。
                    Dim microSeconds As Integer = Integer.MaxValue
                    If ticks <= Integer.MaxValue \ 1000 Then
                        microSeconds = CInt(ticks * 1000)
                    End If

                    'ソケット読み出し監視＆監視結果取得用のリストを作成する。
                    'OPT: 毎回作り直すより、インスタンスをフィールドに保持しておき、
                    'それをClear()してから要素を追加する方が効率的と思われる。
                    Dim oCheckReadList As ArrayList = DirectCast(oSockList.Clone(), ArrayList)

                    'ソケットが読み出し可能になるか次のタイムアウトが発生するまで待機する。
                    Socket.Select(oCheckReadList, Nothing, Nothing, microSeconds)

                    '読み出し可能になったソケットがあれば、
                    '該当するメソッドを実行して、イベントの検索に戻る。
                    If oCheckReadList.Count > 0 Then
                        Dim oSock As Socket = DirectCast(oCheckReadList(0), Socket)
                        '該当するメソッドを実行する。
                        Dim toBeContinued As Boolean = ProcOnSockReadable(oSock)
                        'メソッドがスレッドを終了すべきと判断した場合は、スレッドを終了する。
                        If Not toBeContinued Then
                            Return
                        End If
                        'イベントの検索に戻る。
                        Continue Do
                    End If
                Else
                    If ticks = TickTimer.InfiniteTicks Then
                        Thread.Sleep(Timeout.Infinite)
                    Else
                        Thread.Sleep(CInt(ticks))
                    End If
                End If
            Loop
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            ProcOnUnhandledException(ex)
        End Try
    End Sub
#End Region

End Class
