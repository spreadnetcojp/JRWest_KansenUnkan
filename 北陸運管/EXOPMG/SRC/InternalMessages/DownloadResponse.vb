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
''' IXllWorker実装スレッドからClientTelegrapherへのダウンロード応答メッセージ。
''' </summary>
Public Structure DownloadResponse
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public ReadOnly Property Result() As DownloadResult
        Get
            Return DirectCast(InternalMessage.Parse(RawBytes).GetExtendInteger1(), DownloadResult)
        End Get
    End Property
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen(ByVal result As DownloadResult) As InternalMessage
        Return New InternalMessage(InternalMessageKind.DownloadResponse, result, 0)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As DownloadResponse
        Debug.Assert(msg.Kind = InternalMessageKind.DownloadResponse)

        Dim ret As DownloadResponse
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

Public Enum DownloadResult As Integer
    Finished
    Aborted
End Enum
