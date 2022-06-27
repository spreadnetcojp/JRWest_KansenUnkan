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
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 任意のREQ電文。
''' </summary>
Public Class EkAnonyReqTelegram
    Inherits EkReqTelegram

#Region "定数"
#End Region

#Region "変数"
    Private _OriginalFilePath As String
#End Region

#Region "プロパティ"
    Public Property OriginalFilePath() As String
        Get
            Return _OriginalFilePath
        End Get

        Set(ByVal filePath As String)
            _OriginalFilePath = filePath
        End Set
    End Property
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal oTeleg As ITelegram, ByVal replyLimitTicks As Integer, ByVal originalFilePath As String)
        MyBase.New(oTeleg)
        Me.ReplyLimitTicks = replyLimitTicks
        Me._OriginalFilePath = originalFilePath
    End Sub
#End Region

#Region "メソッド"
    '渡された電文の型をACK電文の型に変換するメソッド
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkAnonyAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
