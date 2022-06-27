' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2016/06/01  (NES)小林  TestData.Getのlen算出式を修正
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO

Imports JR.ExOpmg.Common

''' <summary>
''' 折り返しシーケンスの要求データ送信電文。
''' </summary>
Public Class NkTestReqTelegram
    Inherits NkReqTelegram

#Region "プロパティ"
    Public ReadOnly Property TestData() As Byte()
        Get
            Dim len As Integer = CInt(ObjSize)
            If len = 0 Then Return Nothing
            Dim aBytes As Byte() = New Byte(len - 1) {}
            Buffer.BlockCopy(RawBytes, ObjPos, aBytes, 0, len)
            Return aBytes
        End Get
    End Property
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal aTestData As Byte(), ByVal replyLimitTicks As Integer)
        MyBase.New(NkSeqCode.Test, NkCmdCode.InquiryReq, If(aTestData Is Nothing, 0, aTestData.Length), replyLimitTicks)
        If aTestData IsNot Nothing Then
            Buffer.BlockCopy(aTestData, 0, Me.RawBytes, ObjPos, aTestData.Length)
        End If
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "メソッド"
    'ボディ部の書式違反をチェックするメソッド
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        'ここ以降、プロパティにアクセス可能。

        Return NakCauseCode.None
    End Function

    'ACK電文を生成するメソッド
    Public Function CreateAckTelegram() As NkTestAckTelegram
        Return New NkTestAckTelegram(TestData)
    End Function

    '渡された電文がACKとして整合性があるか判定するメソッド
    Public Overrides Function IsValidAck(ByVal iReplyTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        If oReplyTeleg.SeqCode <> NkSeqCode.Test Then Return False
        If oReplyTeleg.CmdCode <> NkCmdCode.DataPostAck Then Return False
        'NOTE: 必要なら、その他の項目の整合性もここでチェック可能である。
        'ただし、クラスの担当範囲の一貫性を考慮するなら、oReplyTeleg.SrcEkCode
        'とMe.DstEkCodeの整合性チェックなどは、ClientTelegrapherのサブクラスで
        '行うのが妥当である。ProcOnAckTelegramReceive()をフックして、
        '送信時に保存しておいたlastSentDstEkCodeと比較すればよい。
        Return True
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New NkTestAckTelegram(oReplyTeleg)
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As NkTestAckTelegram
        Return New NkTestAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
