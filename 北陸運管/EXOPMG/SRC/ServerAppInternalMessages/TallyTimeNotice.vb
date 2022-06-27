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

Imports JR.ExOpmg.Common

''' <summary>
''' 対Ｎ間通信プロセスのListenerからTelegrapherへの集計時刻通知メッセージ。
''' </summary>
Public Structure TallyTimeNotice
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen() As InternalMessage
        Return New InternalMessage(ServerAppInternalMessageKind.TallyTimeNotice)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As TallyTimeNotice
        Debug.Assert(msg.Kind = ServerAppInternalMessageKind.TallyTimeNotice)

        Dim ret As TallyTimeNotice
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure
