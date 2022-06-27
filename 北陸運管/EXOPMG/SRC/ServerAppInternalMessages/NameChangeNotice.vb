' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/04/10  (NES)小林  次世代車補対応にて新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' 親スレッドからServer系Telegrapherへのクライアント名変更通知メッセージ。
''' </summary>
Public Structure NameChangeNotice
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public ReadOnly Property ExtendPart() As NameChangeNoticeExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, NameChangeNoticeExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen(ByVal extend As NameChangeNoticeExtendPart) As InternalMessage
        Return New InternalMessage(ServerAppInternalMessageKind.NameChangeNotice, extend)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As NameChangeNotice
        Debug.Assert(msg.Kind = ServerAppInternalMessageKind.NameChangeNotice)

        Dim ret As NameChangeNotice
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class NameChangeNoticeExtendPart
    Public StationName As String
    Public CornerName As String
End Class
