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
''' 親スレッドからClient系Telegrapherへの配信指示要求メッセージ。
''' </summary>
Public Structure MasProDllInvokeRequest
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public ReadOnly Property ExtendPart() As MasProDllInvokeRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, MasProDllInvokeRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen(ByVal extend As MasProDllInvokeRequestExtendPart) As InternalMessage
        Return New InternalMessage(ClientAppInternalMessageKind.MasProDllInvokeRequest, extend)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As MasProDllInvokeRequest
        Debug.Assert(msg.Kind = ClientAppInternalMessageKind.MasProDllInvokeRequest)

        Dim ret As MasProDllInvokeRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class MasProDllInvokeRequestExtendPart
    Public ListFileName As String
    Public ForcingFlag As Boolean
End Class
