' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/01/14  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' 親スレッドからMyTelegrapherへのシナリオ開始要求メッセージ。
''' </summary>
Public Structure ScenarioStartRequest
#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public ReadOnly Property ExtendPart() As ScenarioStartRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, ScenarioStartRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessageインスタンス生成メソッド"
    Public Shared Function Gen(ByVal extend As ScenarioStartRequestExtendPart) As InternalMessage
        Return New InternalMessage(MyInternalMessageKind.ScenarioStartRequest, extend)
    End Function
#End Region

#Region "InternalMessageからの変換メソッド"
    Public Shared Function Parse(ByVal msg As InternalMessage) As ScenarioStartRequest
        Debug.Assert(msg.Kind = MyInternalMessageKind.ScenarioStartRequest)

        Dim ret As ScenarioStartRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class ScenarioStartRequestExtendPart
    '開始日時指定有無
    Public StartTimeSpecified As Boolean

    '開始日時
    Public StartTime As DateTime

    'シナリオファイルのパス
    Public ScenarioFilePath As String
End Class
