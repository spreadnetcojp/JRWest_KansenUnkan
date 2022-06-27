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
Imports System.Net.Sockets
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' 内部メッセージ。
''' </summary>
Public Structure InternalMessage
#Region "定数"
    Private Const MsgSizePos As Integer = 0
    Private Const MsgSizeLen As Integer = 4
    Private Const MsgKindPos As Integer = MsgSizePos + MsgSizeLen
    Private Const MsgKindLen As Integer = 4

    'NOTE: 実質はFriendである。
    Public Const ExtendPartPos As Integer = MsgKindPos + MsgKindLen

    Private Const MinMsgSize As Integer = 128
#End Region

#Region "互換構造体実装用変数"
    'NOTE: 実質はFriendである。
    Public RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public ReadOnly Property Size() As Integer
        Get
            Return BitConverter.ToInt32(RawBytes, MsgSizePos)
        End Get
    End Property

    Public ReadOnly Property Kind() As Integer
        Get
            Return BitConverter.ToInt32(RawBytes, MsgKindPos)
        End Get
    End Property

    Public ReadOnly Property HasValue() As Boolean
        Get
            Return RawBytes IsNot Nothing
        End Get
    End Property
#End Region

#Region "互換構造体実装用の共通メソッド"
    '互換構造体が自らのヘッダ部項目（Size, Kind）を読み取る際に利用するメソッド。
    'NOTE: 実質はFriendである。
    Public Shared Function Parse(ByVal rawBytes As Byte()) As InternalMessage
        Dim ret As InternalMessage
        ret.RawBytes = rawBytes
        Return ret
    End Function
#End Region

#Region "任意長の拡張項目をもつ互換構造体の実装用メソッド"
    '互換構造体のインスタンス作成メソッドを実装する際の、基本となるコンストラクタ。
    'NOTE: このコンストラクタでは、メッセージ種別ごとの任意項目については、領域の用意のみ行う。
    '当該領域に対する値の設定は、呼び出し側（互換構造体側）で行う。
    'NOTE: 実質はFriendである。
    Public Sub New(ByVal kind As Integer, ByVal extendPartSize As Integer)
        Dim size As Integer = ExtendPartPos + extendPartSize
        If size < MinMsgSize Then
            size = MinMsgSize
        End If
        Dim bytes As Byte() = New Byte(size - 1) {}

        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, bytes, MsgSizePos, MsgSizeLen)
        Buffer.BlockCopy(BitConverter.GetBytes(kind), 0, bytes, MsgKindPos, MsgKindLen)
        Me.RawBytes = bytes
    End Sub
#End Region

#Region "拡張項目不要な種別の互換構造体の実装用メソッド"
    'NOTE: 実質はFriendである。
    Public Sub New(ByVal kind As Integer)
        Dim size As Integer = MinMsgSize
        Dim bytes As Byte() = New Byte(size - 1) {}
        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, bytes, MsgSizePos, MsgSizeLen)
        Buffer.BlockCopy(BitConverter.GetBytes(kind), 0, bytes, MsgKindPos, MsgKindLen)
        Me.RawBytes = bytes
    End Sub
#End Region

#Region "SerializableなObjectを拡張項目とする種別の互換構造体の実装用メソッド"
    'NOTE: 実質はFriendである。
    Public Sub New(ByVal kind As Integer, ByVal obj As Object)
        Dim mem As New MemoryStream()
        Dim bf As New BinaryFormatter()
        bf.Serialize(mem, obj)

        Dim memLen As Integer = CInt(mem.Length)

        Dim size As Integer = ExtendPartPos + memLen
        If size < MinMsgSize Then
            size = MinMsgSize
        End If
        Dim bytes As Byte() = New Byte(size - 1) {}

        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, bytes, MsgSizePos, MsgSizeLen)
        Buffer.BlockCopy(BitConverter.GetBytes(kind), 0, bytes, MsgKindPos, MsgKindLen)

        mem.Position = 0
        mem.Read(bytes, ExtendPartPos, memLen)

        Me.RawBytes = bytes
    End Sub

    'NOTE: 実質はFriendである。
    Public Function GetExtendObject() As Object
        Dim mem As New MemoryStream(RawBytes, ExtendPartPos, RawBytes.Length - ExtendPartPos, False)
        Dim bf As New BinaryFormatter()
        Return bf.Deserialize(mem)
    End Function
#End Region

#Region "Integerを拡張項目とする種別の互換構造体の実装用メソッド"
    'NOTE: 実質はFriendである。
    Public Sub New(ByVal kind As Integer, ByVal extend1 As Integer, ByVal extend2 As Integer)
        Dim size As Integer = ExtendPartPos + 4 + 4
        If size < MinMsgSize Then
            size = MinMsgSize
        End If
        Dim bytes As Byte() = New Byte(size - 1) {}

        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, bytes, MsgSizePos, MsgSizeLen)
        Buffer.BlockCopy(BitConverter.GetBytes(kind), 0, bytes, MsgKindPos, MsgKindLen)
        Buffer.BlockCopy(BitConverter.GetBytes(extend1), 0, bytes, ExtendPartPos, 4)
        Buffer.BlockCopy(BitConverter.GetBytes(extend2), 0, bytes, ExtendPartPos + 4, 4)
        Me.RawBytes = bytes
    End Sub

    'NOTE: 実質はFriendである。
    Public Function GetExtendInteger1() As Integer
        Return BitConverter.ToInt32(RawBytes, ExtendPartPos)
    End Function

    'NOTE: 実質はFriendである。
    Public Function GetExtendInteger2() As Integer
        Return BitConverter.ToInt32(RawBytes, ExtendPartPos + 4)
    End Function
#End Region

#Region "ソケットへの書き込みメソッド"
    'NOTE: 同一のSocketに対し、複数のスレッドが（排他制御なしに）このメソッドを実行
    'することは想定しない。よって、内部メッセージ送受信用Socketについては、単一の
    'スレッドでアクセスするよう、所有者決めてアプリを設計するのがベストである。
    'たとえば、コネクションをキープするタイプのクライアント側アプリにおいて、
    'Telegrapherが外部とのコネクションを切断した場合の自動再接続制御は、
    '他の能動的制御と同一のスレッドを中枢として行うべきであり、専用の
    'スレッド（切断監視スレッド）は用意しない方がよい。一見すると、専用の
    'スレッドを用意することで、切断監視のための実装が（内部に切断監視スレッド
    'を持つConnectionKeeperのようなクラスに）局所化されて、単純になりそうで
    'あるが、そうした場合、Telegrapherに対するConnectNoticeメッセージの送信は
    '切断監視スレッドが行う一方で、ActiveUllExecRequestのようなメッセージの
    '送信はメイン的スレッドが行うことになり、Telegrapherと内部通信を行うための
    'Socketの排他制御が複雑になる。そもそも、切断監視をメイン的スレッドで行うことは、
    'さほど大変なことではない。接続完了状態では、メッセージループのタイマハンドラで
    '接続状態の監視を行い、切断を認識したらBeginConnectして接続試行中状態になり、
    '接続試行中状態では、BeginConnectの完了の監視を行うだけである。
    '仮に、同一のSocketに関して、複数のスレッドがこのメソッドを呼び出すなら、
    '呼び出し側が（その参照に関するSyncLockを行うなどして）排他制御を行う
    '必要があるが、WSAEINPROGRESSのことを考えると、このメソッドのみならず、
    '同一のSocketに関するGetInstanceFromSocket()とも排他するべきであるし、
    'それどころか、同一のSocketに関するSocket.Select()とも排他しなければ
    'ならないと思われる。即ち、Socket.Select()を呼ぶ箇所で、WSAEINPROGRESSの
    'ハンドリングを行わなければならなくなる。特定環境でテストした限りでは、
    'Socketクラスのメソッド呼び出しでWSAEINPROGRESSが発生することは
    'なさそうであったが、中身がWinSockである以上は、発生することを前提とする
    'べきである（たとえば.NET Compact Frameworkだと、どうなるかわからない）。
    'なお、複数のスレッドから実行し得る（実行できて然るべきである）のは、
    'WriteToSocket同士以外は、WriteToSocketと読み出しだけであり、読み出し同士は
    '複数のスレッドで実行することがない（実行できる必要はない）ため、アプリに
    '非ブロッキングモードなSocketを用意してもらった上で、以下のメソッドに
    'おける sock へのアクセスをSyncLock sock～End SyncLockで囲むことで、
    'これらのメソッドをスレッドセーフにするという方針も存在する。
    'そのようにすれば、待機が発生するのは、Select()による読み出し待ちを行う
    '箇所（つまり、ManagementLoopのスレッド）だけになるし、そもそも
    '非ブロッキングモードであるわけだから、Select()でWSAEINPROGRESSが発生する
    'のを前提にする必要は（検証するまでもなく）全く無くなる。ただし、
    'WriteToSocket()の内部でSend()を複数回繰り返すことになるなど、
    '微妙な点もある（そもそも非ブロッキングモードにしておかなければ
    'ならないこと自体が微妙である）ので、やめておく。
    Public Sub WriteToSocket(ByVal sock As Socket)
        Debug.Assert(sock.Blocking)
        sock.SendTimeout = 0
        sock.Send(RawBytes)
    End Sub

    'NOTE: 送信バッファに入りきらない大きなメッセージを書き込む際に、
    '送信先のスレッドが不具合等でソケットからの読み出しを
    '行わなくなっていること等も想定したアプリ向けのバージョン。
    '親スレッドが子スレッドにメッセージを送信する際に、
    '子スレッドの停止許容時間と同レベルの時間を引数にして
    '利用することを想定。
    'NOTE: timeoutTicksに0または-1を指定すると無期限待機となる。
    Public Function WriteToSocket(ByVal sock As Socket, ByVal timeoutTicks As Integer) As Boolean
        Try
            Debug.Assert(sock.Blocking)
            sock.SendTimeout = timeoutTicks
            sock.Send(RawBytes)
        Catch ex As SocketException
            Select Case ex.ErrorCode
                '指定した時間内に書き込みできなかった場合（WSAETIMEDOUT）
                'TODO: これじゃない気も（Socketクラスの実装次第？）
                Case 10060
                    Log.Error("I'm through waiting for all bytes of the msg to write.", ex)
                    Return False

                '発生したなら、アプリの不具合が要因である可能性が濃厚であるため、
                'アプリを終了させて早めに（テスト中に）気付かせた方がよいエラー
                'NOTE: 10036（WSAEINPROGRESS）が発生しない条件で使われるように
                'する予定であるため、10036（WSAEINPROGRESS）もここにある。
                Case 10009, 10013, 10014, 10022, 10035, 10036, 10037, 10038, _
                     10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                     10047, 10048, 10049, 10056, 10092, 10093
                    Throw

                'Send()において、対端ソケット操作や装置内の状況で発生しそうであるため、
                'アプリを終了させるわけにはいかないと思われるエラー
                Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                     10058, 10061, 10064, 10065, 10101
                    Log.Error("SocketException caught.", ex)
                    Return False

                '発生し得ないはずであるが、将来どうなるかわからないため、
                'アプリを終了させない方が無難と思われるエラー
                Case Else
                    Log.Error("Surprising SocketException caught.", ex)
                    Return False
            End Select
        End Try
        Return True
    End Function
#End Region

#Region "ソケットからの読み出しメソッド"
    Public Shared Function GetInstanceFromSocket(ByVal sock As Socket) As InternalMessage
        Dim msgLen As Integer = MinMsgSize
        Dim bytes As Byte() = New Byte(msgLen - 1) {}
        Dim offset As Integer = 0
        Dim isReceivedMinSize As Boolean = False

        Debug.Assert(sock.Blocking)
        sock.ReceiveTimeout = 0
        Do
            Dim rcvlen As Integer = sock.Receive(bytes, offset, msgLen - offset, SocketFlags.None)
            offset = offset + rcvlen
            If offset = msgLen Then
                If isReceivedMinSize Then Exit Do

                msgLen = BitConverter.ToInt32(bytes, MsgSizePos)
                If msgLen = MinMsgSize Then Exit Do

                Debug.Assert(msgLen > MinMsgSize)
                Array.Resize(bytes, msgLen)
                isReceivedMinSize = True
            End If
        Loop

        Dim ret As InternalMessage
        ret.RawBytes = bytes
        Return ret
    End Function

    'NOTE: 送信元のスレッドが不具合等でソケットへの書き込みの途中で停止したり、
    'Sizeに設定した分のバイトを書き込まなかったり、（メッセージを
    '書き込むべき状況であるにもかかわらず）そもそも何も書き込まなかったり
    'すること等も想定するアプリ向けのバージョン。
    'NOTE: 親スレッドが子スレッドからのメッセージを受信する際に、
    '子スレッドの停止許容時間と同レベルの時間を引数にして
    '利用することを想定。
    'NOTE: このメソッドのみHasValueがFalseなインスタンスを返すことが
    'あり得る（メッセージの取り出しで上記のような異常を検出した場合）。
    'NOTE: timeoutTicksに0または-1を指定すると無期限待機となる。
    Public Shared Function GetInstanceFromSocket(ByVal sock As Socket, ByVal timeoutTicks As Integer) As InternalMessage
        Dim msgLen As Integer = MinMsgSize
        Dim bytes As Byte() = New Byte(msgLen - 1) {}
        Dim offset As Integer = 0
        Dim isReceivedMinSize As Boolean = False

        Debug.Assert(sock.Blocking)
        Dim timer As TickTimer = Nothing
        Dim systemTick As Long
        If timeoutTicks > 0 Then
            timer = New TickTimer(timeoutTicks)
            systemTick = TickTimer.GetSystemTick()
            timer.Start(systemTick)
        Else
            sock.ReceiveTimeout = 0
        End If
        Try
            Do
                If timeoutTicks > 0 Then
                    Dim ticks As Long = timer.GetTicksToTimeout(systemTick)
                    If ticks < 1 Then
                        Log.Error("I'm through waiting for all bytes of the msg to read.")
                        Return Nothing
                    End If
                    sock.ReceiveTimeout = CInt(ticks)
                End If

                Dim rcvlen As Integer = sock.Receive(bytes, offset, msgLen - offset, SocketFlags.None)
                If rcvlen = 0 Then
                    Log.Error("Connection closed by peer.")
                    Return Nothing
                End If

                offset = offset + rcvlen
                If offset = msgLen Then
                    If isReceivedMinSize Then Exit Do

                    msgLen = BitConverter.ToInt32(bytes, MsgSizePos)
                    If msgLen = MinMsgSize Then Exit Do

                    Debug.Assert(msgLen > MinMsgSize)
                    Array.Resize(bytes, msgLen)
                    isReceivedMinSize = True
                End If
            Loop
        Catch ex As SocketException
            Select Case ex.ErrorCode
                '指定した時間内に書き込みできなかった場合（WSAETIMEDOUT）
                'TODO: これじゃない気も（Socketクラスの実装次第？）
                Case 10060
                    Log.Error("I'm through waiting for all bytes of the telegram to read.", ex)
                    Return Nothing

                '発生したなら、アプリの不具合が要因である可能性が濃厚であるため、
                'アプリを終了させて早めに（テスト中に）気付かせた方がよいエラー
                'NOTE: 10036（WSAEINPROGRESS）が発生しない条件で使われるように
                'する予定であるため、10036（WSAEINPROGRESS）もここにある。
                Case 10009, 10013, 10014, 10022, 10035, 10036, 10037, 10038, _
                     10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                     10047, 10048, 10049, 10056, 10092, 10093
                    Throw

                'Receive()において、対端ソケット操作や装置内の状況で発生しそうであるため、
                'アプリを終了させるわけにはいかないと思われるエラー
                Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                     10058, 10061, 10064, 10065, 10101
                    Log.Error("SocketException caught.", ex)
                    Return Nothing

                '発生し得ないはずであるが、将来どうなるかわからないため、
                'アプリを終了させない方が無難と思われるエラー
                Case Else
                    Log.Error("Surprising SocketException caught.", ex)
                    Return Nothing
            End Select
        End Try

        Dim ret As InternalMessage
        ret.RawBytes = bytes
        return ret
    End Function
#End Region
End Structure
