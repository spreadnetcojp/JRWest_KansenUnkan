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
''' 親スレッドからMyTelegrapherへの能動的ULLシーケンス実施要求メッセージ。
''' </summary>
Public Structure ActiveUllExecRequest
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public ReadOnly Property ExtendPart() As ActiveUllExecRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, ActiveUllExecRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen(ByVal extend As ActiveUllExecRequestExtendPart) As InternalMessage
        Return New InternalMessage(MyInternalMessageKind.ActiveUllExecRequest, extend)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As ActiveUllExecRequest
        Debug.Assert(msg.Kind = MyInternalMessageKind.ActiveUllExecRequest)

        Dim ret As ActiveUllExecRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class ActiveUllExecRequestExtendPart
    Public ObjCode As Integer
    Public TransferFileName As String
    Public ApplyFilePath As String
    Public ApplyFileHashValue As String
    Public TransferLimitTicks As Integer
    Public ReplyLimitTicksOnStart As Integer
    Public ReplyLimitTicksOnFinish As Integer
    Public RetryIntervalTicks As Integer
    Public MaxRetryCountToForget As Integer
    Public MaxRetryCountToCare As Integer
    Public DeleteApplyFileIfCompleted As Boolean
    Public ApplyFileMustExists As Boolean
End Class
