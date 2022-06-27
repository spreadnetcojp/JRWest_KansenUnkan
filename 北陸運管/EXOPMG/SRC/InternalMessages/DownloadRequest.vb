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

''' <summary>
''' ClientTelegrapherからIXllWorker実装スレッドへのダウンロード要求メッセージ。
''' </summary>
Public Structure DownloadRequest
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public ReadOnly Property ExtendPart() As DownloadRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, DownloadRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen(ByVal extend As DownloadRequestExtendPart) As InternalMessage
        Return New InternalMessage(InternalMessageKind.DownloadRequest, extend)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As DownloadRequest
        Debug.Assert(msg.Kind = InternalMessageKind.DownloadRequest)

        Dim ret As DownloadRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class DownloadRequestExtendPart
    '転送対象ファイル名のベースとするローカルパス
    Public TransferListBase As String

    '転送対象ファイル名の一覧
    Public TransferList As List(Of String)
End Class
