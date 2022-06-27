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

''' <summary>
''' 親スレッドから各種Telegrapherへの接続通知メッセージ。
''' </summary>
Public Structure ConnectNotice
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "アクセサ"
    Public Function GetSocket() As Socket
        Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
        Dim inf As SocketInformation = DirectCast(obj, SocketInformation)
        Return New Socket(inf)
    End Function
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen(ByVal sock As Socket) As InternalMessage
        Dim curProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess()
        Dim inf As SocketInformation = sock.DuplicateAndClose(curProcess.Id)
        curProcess.Close()
        Return New InternalMessage(InternalMessageKind.ConnectNotice, inf)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As ConnectNotice
        Debug.Assert(msg.Kind = InternalMessageKind.ConnectNotice)

        Dim ret As ConnectNotice
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure
