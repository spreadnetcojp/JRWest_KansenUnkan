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
''' 要求コマンド電文。
''' </summary>
Public Class NkInquiryReqTelegram
    Inherits NkReqTelegram

#Region "定数"
    Private Const ObjLen As Integer = 0
#End Region

#Region "プロパティ"
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal seqCode As NkSeqCode, ByVal replyLimitTicks As Integer)
        MyBase.New(seqCode, NkCmdCode.InquiryReq, ObjLen, replyLimitTicks)
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

    'ACK電文を生成するメソッド
    Public Function CreateAckTelegram(ByVal status As UShort) As NkInquiryAckTelegram
        Return New NkInquiryAckTelegram(SeqCode, status)
    End Function

    '渡された電文がACKとして整合性があるか判定するメソッド
    Public Overrides Function IsValidAck(ByVal iReplyTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        If oReplyTeleg.SeqCode <> SeqCode Then Return False
        If oReplyTeleg.CmdCode <> NkCmdCode.InquiryAck Then Return False
        'NOTE: 必要なら、その他の項目の整合性もここでチェック可能である。
        'ただし、クラスの担当範囲の一貫性を考慮するなら、oReplyTeleg.SrcEkCode
        'とMe.DstEkCodeの整合性チェックなどは、ClientTelegrapherのサブクラスで
        '行うのが妥当である。ProcOnAckTelegramReceive()をフックして、
        '送信時に保存しておいたlastSentDstEkCodeと比較すればよい。
        Return True
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New NkInquiryAckTelegram(oReplyTeleg)
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As NkInquiryAckTelegram
        Return New NkInquiryAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
