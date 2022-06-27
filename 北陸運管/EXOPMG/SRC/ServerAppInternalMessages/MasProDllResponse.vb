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
''' Server系Telegrapherから親スレッドへのマスタ/プログラムDLL応答メッセージ。
''' </summary>
''' <remarks>
''' 運管サーバのDLL処理において、DLL結果のDBへの書き込みは
''' 親スレッドではなくTelegrapher側スレッドの責務である。
''' よって、本メッセージの役割は、DLLの結果を親スレッドに伝えること
''' ではなく、要求されたDLLシーケンスに起因するファイル転送が
''' （恐らく）これ以上発生しないことや、次の要応答な依頼を
''' 処理可能になったことを親スレッドに伝えることである。
''' </remarks>
Public Structure MasProDllResponse
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen() As InternalMessage
        Return New InternalMessage(ServerAppInternalMessageKind.MasProDllResponse)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As MasProDllResponse
        Debug.Assert(msg.Kind = ServerAppInternalMessageKind.MasProDllResponse)

        Dim ret As MasProDllResponse
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure
