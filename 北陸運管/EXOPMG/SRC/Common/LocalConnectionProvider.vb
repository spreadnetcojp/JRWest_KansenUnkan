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
Option Strict On
Option Explicit On

Imports System.Net
Imports System.Net.Sockets

''' <summary>
''' ローカル接続生成クラス
''' </summary>
Public Class LocalConnectionProvider

#Region "定数や変数"
    '排他用オブジェクト
    'NOTE: 複数のスレッドによるoListenerSockの変更と参照や、複数のスレッドによる
    'コネクションの生成（Connectの実施）から、それ（Connectの戻り値）に対応
    'するサーバ側通信用ソケットの取り出し（Acceptの呼び出し完了）までの処理
    '同士を排他的に行わせるためのものである。
    'なお、本質的には、oListenerSockが参照され得る（CreateSocketsメソッドが
    '実行され得る）期間にoListenerSockの変更（InitやDisposeの実行）を行わないように
    'することは、呼び出し側の責務である。
    Private Shared ReadOnly oListenerLockObject As New Object()

    'リスニングソケット
    Private Shared oListenerSock As Socket
#End Region

#Region " +s Init()  初期化"
    ''' <summary>
    ''' クラスの初期化
    ''' </summary>
    ''' <remarks>
    ''' クラスを使用可能にする。
    ''' </remarks>
    Public Shared Sub Init()
        SyncLock oListenerLockObject
            If oListenerSock IsNot Nothing Then oListenerSock.Close()  '本当はエラーとしてよい。
            oListenerSock = SockUtil.StartLocalListener(0)
        End SyncLock
    End Sub
#End Region

#Region " +s Dispose()  破棄"
    ''' <summary>
    ''' クラスの破棄
    ''' </summary>
    ''' <remarks>
    ''' クラスを破棄にする。
    ''' </remarks>
    Public Shared Sub Dispose()
        SyncLock oListenerLockObject
            If oListenerSock IsNot Nothing Then
                oListenerSock.Close()
                oListenerSock = Nothing
            End If
        End SyncLock
    End Sub
#End Region

#Region " +s CreateSockets()  ソケット作成"
    ''' <summary>
    ''' ソケット作成
    ''' </summary>
    ''' <param name="oSock1">Socket</param>
    ''' <param name="oSock2">Socket</param>
    ''' <remarks>
    ''' ローカル接続を生成し、ローカル通信用ソケットを取得する。
    ''' </remarks>
    Public Shared Sub CreateSockets(ByRef oSock1 As Socket, ByRef oSock2 As Socket)
        Dim oSock1t As Socket = Nothing
        Dim oSock2t As Socket = Nothing

        SyncLock oListenerLockObject
            Try
                Dim portNo As Integer = DirectCast(oListenerSock.LocalEndPoint, IPEndPoint).Port
                oSock2t = SockUtil.ConnectToLocal(portNo)
                oSock1t = SockUtil.Accept(oListenerSock)
            Catch ex As Exception
                If oSock2t IsNot Nothing Then
                    oSock2t.Close()
                End If
                If oSock1t IsNot Nothing Then
                    oSock1t.Close()
                End If
                'OPT: 一時的にリソースが足りないだけである可能性も考えると、
                '戻り値で通知した方がよいかもしれない。
                Throw
            End Try
        End SyncLock

        oSock1 = oSock1t
        oSock2 = oSock2t
    End Sub
#End Region

End Class
