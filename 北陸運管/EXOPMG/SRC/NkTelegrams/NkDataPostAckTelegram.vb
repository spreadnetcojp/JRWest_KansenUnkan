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

Imports System.Globalization

Imports JR.ExOpmg.Common

''' <summary>
''' 回答レスポンス電文。
''' </summary>
Public Class NkDataPostAckTelegram
    Inherits NkTelegram

#Region "定数"
    Private Const ReturnStatusPos As Integer = ObjPos
    Private Const ReturnStatusLen As Integer = 2
    Private Const ReservedArea1Pos As Integer = ReturnStatusPos + ReturnStatusLen
    Private Const ReservedArea1Len As Integer = 2
    Private Const ObjLen As Integer = ReservedArea1Pos + ReservedArea1Len - ObjPos
#End Region

#Region "プロパティ"
    Public Property ReturnStatus() As UShort
        Get
            Return Utility.GetUInt16FromLeBytes2(RawBytes, ReturnStatusPos)
        End Get

        Set(ByVal status As UShort)
            Utility.CopyUInt16ToLeBytes2(status, RawBytes, ReturnStatusPos)
        End Set
    End Property
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal seqCode As NkSeqCode, ByVal status As UShort)
        MyBase.New(seqCode, NkCmdCode.DataPostAck, ObjLen)
        Me.ReturnStatus = status
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "メソッド"
    'ボディ部の書式違反をチェックするメソッド
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If ObjSize <> ObjLen Then
            Log.Error("ObjSize is invalid.")
            Return NakCauseCode.TelegramError
        End If

        'ここ以降、プロパティにアクセス可能。

        Return NakCauseCode.None
    End Function
#End Region

End Class
