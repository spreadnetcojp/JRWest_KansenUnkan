' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴：
'   Ver      日付        担当       コメント
'   0.0      2006/08/01  溝上       新規作成
'   0.1      2006/08/07  溝上       他システムとの連携があるため、Write()とRead()メソッドでやり取りするデータに付けていたヘッダーブロックを除去
'   0.2      2006/09/18  宗行       Read()メソッドにてReceive処理をデータ長分繰り返すよう修正
'   0.3      2006/11/21  宗行       OP-003 ソケット接続時、IPアドレスの前0埋めを削除する
'   0.4      2006/11/22  宗行       ソケット接続時、IPアドレス指定の場合は、ホスト解決しないよう修正
'   0.5      2013/04/01  (NES)小林  クラス名をSocketControlから変更して不相応なメソッドを除去、
'                                   呼び元で対処できない例外発生時のリークを除去、
'                                   ローカル接続用メソッドを追加
' **********************************************************************
Option Strict On
Option Explicit On

Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

'NOTE: 例外処理の方針を改めた方がよい。
'現状、相手装置が関係している可能性のある例外も、
'このプログラム自身（このクラス自身や呼び元）の不具合とみなすべき例外も
'全てキャッチし、全てOPMGExceptionに置き換えているため、
'呼び元は、接続のやりなおしで済ますべきなのか、スレッドやプロセスの
'再起動に持ち込むべきなのか判断がつかない。
'相手装置が関係している可能性のある例外のみ
'キャッチする（OPMGExceptionに置き換える）か、
'全てキャッチし、相手装置が関係している可能性のある例外については
'（OPMGExceptionではなく）SocketExternalExceptionに置き換えるか、
'そのような場合は戻り値で異常を通知するか、方針を決めなければ
'まともに利用できない（呼び元で内部例外を解析するなどということに）。

''' <summary>
''' ソケット通信用ユーティリティ
''' </summary>
''' <remarks>
''' .NET FrameworkのSocketクラスを利用するアプリケーション向けのユーティリティ。
''' このクラスにはサーバー向け機能とクライアント向け機能の両方を持たせている。
''' </remarks>
Public Class SockUtil

    ' ****************************************
    ' このクラスの使い方:
    '   このクラスはサーバー向けのメソッドとクライアント向けのメソッドを実装している。
    '
    '   接続方法: TCP/IP、ストリーム接続 固定とする。
    '
    '   サーバー側の手順:
    '       (1) StartListener()メソッドでリスニングソケットを作成する。
    '           それにより、OSは、TCPのポートを作成し、クライアントからの接続を
    '           受け付ける（接続があればハンドシェークを行う）状態になる。
    '       (2) リスニングソケットに対してAccept()メソッドを実行する。
    '           このメソッドは同期処理されるため、クライアントからの接続がなければ、
    '           呼び元のスレッドは停止する。クライアントからの接続を受け付けると、
    '           アプリケーション任意データを読み書き可能なSocketを返す。
    '
    '   クライアント側の手順:
    '       (1) Connect()メソッドでサーバーに接続する。このメソッドは、接続が成功
    '           すると、アプリケーション任意データを読み書き可能なSocketを返す。
    ' ****************************************

    ' // //////////////////////////////////////// 内部関数
#Region " - CreateSocket()  Socket作成 "
    ''' <summary>
    ''' Socket作成
    ''' </summary>
    ''' <returns>新規Socket</returns>
    ''' <remarks>
    ''' 新規SocketをTCP/IP、ストリーム接続で生成する。
    ''' </remarks>
    Private Shared Function CreateSocket() As Socket
        ' Socket の初期化オプション
        '   INIファイル指定やコンストラクタの引数としても良いですが、
        '   通信処理でこのオプションに依存する部分があるため
        '   ここでは本物件に最適化して、オプションはローカル変数による指定とする。
        Dim eAddress As AddressFamily = AddressFamily.InterNetwork       ' IP (v4)
        Dim eSocket As SocketType = SocketType.Stream    ' ストリーム送受信
        Dim eProtocol As ProtocolType = ProtocolType.Tcp  ' TCP
        Return New Socket(eAddress, eSocket, eProtocol)
    End Function
#End Region

    ' // //////////////////////////////////////// メソッド
#Region " + StartListener()  リスナー開始 "
    ''' <summary>
    ''' リスナー開始
    ''' </summary>
    ''' <param name="Address">IPアドレス</param>
    ''' <param name="PortNo">ポート番号</param>
    ''' <returns>リスナー ソケット</returns>
    ''' <exception cref="OPMGException">このメソッドで発生した例外</exception>
    ''' <remarks>
    ''' 【サーバー側処理】
    ''' ソケット通信のサーバー側処理にて最初に実行する必要がある。
    ''' 指定したポート番号でリスナーを開始する。
    ''' リスナーが開始できた後にAccept()メソッドを呼ぶことにより、
    ''' クライアントからの接続を待つ状態になる。
    ''' </remarks>
    Public Shared Function StartListener(ByVal Address As IPAddress, ByVal PortNo As Integer) As Socket
        Dim r As Socket = Nothing
        Dim oEndPoint As IPEndPoint = Nothing
        Dim oListenerSock As Socket = Nothing
        Try
            ' リッスン ポートのエンド ポイントを生成
            oEndPoint = New IPEndPoint(Address, PortNo)
            ' リスナーの生成
            oListenerSock = CreateSocket()
            ' バインド
            oListenerSock.Bind(oEndPoint)
            oListenerSock.Listen(10)
            r = oListenerSock
        Catch ex As Exception
            ' 詳細ログ
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.StartListener()"))
            ' IPアドレス
            If IsNothing(Address) Then
                sb.AppendLine(OPMGException.DetailNull("Address"))
            Else
                sb.AppendFormat("Address is [{0}].", Address.ToString())
                sb.AppendLine()
            End If
            ' リッスン ポート
            sb.AppendFormat("PortNo is [{0}].", PortNo.ToString())
            sb.AppendLine()

            If oListenerSock IsNot Nothing Then
                oListenerSock.Close()
            End If
            Throw New OPMGException(sb.ToString(), ex)
        End Try
        Return r
    End Function
#End Region
#Region " + StartLocalListener()  ローカル接続用リスナー開始 "
    ''' <summary>
    ''' リスナー開始
    ''' </summary>
    ''' <param name="PortNo">ポート番号</param>
    ''' <returns>リスナー ソケット</returns>
    ''' <exception cref="OPMGException">このメソッドで発生した例外</exception>
    ''' <remarks>
    ''' 【サーバー側処理】
    ''' ソケット通信のサーバー側処理にて最初に実行する必要がある。
    ''' 指定したポート番号でリスナーを開始する。
    ''' リスナーが開始できた後にAccept()メソッドを呼ぶことにより、クライアントからの接続を待つ状態になる。
    ''' </remarks>
    Public Shared Function StartLocalListener(ByVal PortNo As Integer) As Socket
        Dim r As Socket = Nothing
        Dim oEndPoint As IPEndPoint = Nothing
        Dim oListenerSock As Socket = Nothing
        Try
            ' リッスン ポートのエンド ポイントを生成
            oEndPoint = New IPEndPoint(IPAddress.Parse("127.0.0.1"), PortNo)
            ' リスナーの生成
            oListenerSock = CreateSocket()
            ' バインド
            oListenerSock.Bind(oEndPoint)
            oListenerSock.Listen(1)
            r = oListenerSock
        Catch ex As Exception
            ' 詳細ログ
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.StartLocalListener()"))
            ' リッスン ポート
            sb.AppendFormat("PortNo is [{0}].", PortNo.ToString())
            sb.AppendLine()

            If oListenerSock IsNot Nothing Then
                oListenerSock.Close()
            End If
            Throw New OPMGException(sb.ToString(), ex)
        End Try
        Return r
    End Function
#End Region
#Region " + Accept()  接続待機 "
    ''' <summary>
    ''' 接続待機
    ''' </summary>
    ''' <param name="listenerSocket">リスナー ソケット</param>
    ''' <returns>送受信ソケット</returns>
    ''' <exception cref="OPMGException">このメソッドで発生した例外</exception>
    ''' <remarks>
    ''' 【サーバー側処理】
    ''' ソケット通信は同期実行されるため、このメソッドはクライアントからの受信があるまで終了しない。
    ''' クライアントからの受信があった場合、送受信Socketのインスタンスを生成して返す。
    ''' アプリケーションはRead()メソッドにSocketを使ってデータの受信が行える。
    ''' </remarks>
    Public Shared Function Accept(ByVal listenerSocket As Socket) As Socket
        Try
            ' 接続
            Return listenerSocket.Accept()
        Catch ex As Exception
            ' 詳細ログ
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.Accept()"))
            Throw New OPMGException(sb.ToString(), ex)
        End Try
    End Function
#End Region
#Region " + Connect()  接続 "
    ''' <summary>
    ''' 接続
    ''' </summary>
    ''' <param name="ServerName">サーバー名</param>
    ''' <param name="PortNo">ポート番号</param>
    ''' <returns>読み込み/書き込み可能なSocket</returns>
    ''' <exception cref="OPMGException">このメソッドで発生した例外</exception>
    ''' <remarks>
    ''' 【クライアント側処理】
    ''' サーバーに対してソケット接続を行う。
    ''' サーバーへの接続が成功したとき、送受信用Socketのインスタンスを返す。
    ''' アプリケーションはWrite()メソッドにSocketを使ってデータの送信が行える。
    ''' </remarks>
    Public Shared Function Connect(ByVal ServerName As String, ByVal PortNo As Integer) As Socket
        Dim oSocket As Socket = Nothing
        Dim oHost As IPHostEntry = Nothing
        Dim Address As IPAddress = Nothing
        Dim oEndPoint As IPEndPoint = Nothing
        Try

            Dim sIP As String()
            sIP = Split(ServerName, ".", -1, CompareMethod.Text)

            If sIP.Length = 4 Then
                'ⅰ)サーバー名がIPアドレスの場合

                'IP文字列を一旦数値化し文字列に戻すことで前0埋めを削除する。
                For i As Integer = 0 To 3
                    sIP(i) = CStr(CInt(sIP(i)))
                Next

                'IPアドレスをセット
                Address = IPAddress.Parse(sIP(0) & "." & sIP(1) & "." & sIP(2) & "." & sIP(3))
            Else
                'ⅱ)サーバー名がホスト名の場合

                ' ホスト エントリの取得
                oHost = Dns.GetHostEntry(ServerName)

                ' 最初のネットワーク カードのIPアドレスを取得
                Address = oHost.AddressList(0)
            End If

            ' サーバー側リッスン ポートのエンド ポイントを生成
            oEndPoint = New IPEndPoint(Address, PortNo)
            ' 接続の生成
            oSocket = CreateSocket()
            ' 接続
            oSocket.Connect(oEndPoint)
        Catch ex As Exception
            ' 詳細ログ
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.Connect()"))
            ' ホスト エントリ
            If IsNothing(oHost) Then
                sb.AppendLine(OPMGException.DetailNull("Host"))
            Else
                Try
                    sb.AppendFormat("HostName is [{0}].", oHost.HostName)
                Catch iex As Exception
                    sb.Append(OPMGException.DetailException("HostName", iex))
                End Try
                sb.AppendLine()
            End If
            ' IPアドレス
            If IsNothing(Address) Then
                sb.AppendLine(OPMGException.DetailNull("Address"))
            Else
                sb.AppendFormat("Address is [{0}].", Address.ToString())
                sb.AppendLine()
            End If
            sb.AppendFormat("PortNo is [{0}].", PortNo.ToString())
            sb.AppendLine()

            If oSocket IsNot Nothing Then
                oSocket.Close()
            End If
            Throw New OPMGException(sb.ToString(), ex)
        End Try
        Return oSocket
    End Function
#End Region
#Region " + ConnectToLocal()  ローカル接続 "
    ''' <summary>
    ''' 接続
    ''' </summary>
    ''' <param name="PortNo">ポート番号</param>
    ''' <returns>読み込み/書き込み可能なSocket</returns>
    ''' <exception cref="OPMGException">このメソッドで発生した例外</exception>
    ''' <remarks>
    ''' 【クライアント側処理】
    ''' サーバーに対してソケット接続を行う。
    ''' サーバーへの接続が成功したとき、送受信用Socketのインスタンスを返す。
    ''' アプリケーションはWrite()メソッドにSocketを使ってデータの送信が行える。
    ''' </remarks>
    Public Shared Function ConnectToLocal(ByVal PortNo As Integer) As Socket
        Dim oSocket As Socket = Nothing
        Dim oEndPoint As IPEndPoint = Nothing
        Try
            ' サーバー側リッスン ポートのエンド ポイントを生成
            oEndPoint = New IPEndPoint(IPAddress.Parse("127.0.0.1"), PortNo)
            ' 接続の生成
            oSocket = CreateSocket()
            ' 接続
            oSocket.Connect(oEndPoint)
        Catch ex As Exception
            ' 詳細ログ
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.ConnectToLocal()"))
            ' リッスン ポート
            sb.AppendFormat("PortNo is [{0}].", PortNo.ToString())
            sb.AppendLine()

            If oSocket IsNot Nothing Then
                oSocket.Close()
            End If
            Throw New OPMGException(sb.ToString(), ex)
        End Try
        Return oSocket
    End Function
#End Region

End Class
