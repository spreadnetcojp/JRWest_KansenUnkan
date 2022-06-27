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

Imports System.IO

Imports JR.ExOpmg.Common

''' <summary>
''' 要求データ送信電文。
''' </summary>
Public Class NkDataPostReqTelegram
    Inherits NkReqTelegram

#Region "プロパティ"
#End Region

#Region "コンストラクタ"
    Public Sub New( _
       ByVal seqCode As NkSeqCode, _
       ByVal aBytes As Byte(), _
       ByVal replyLimitTicks As Integer)

        MyBase.New(seqCode, NkCmdCode.DataPostReq, aBytes.Length, replyLimitTicks)
        Buffer.BlockCopy(aBytes, 0, Me.RawBytes, ObjPos, aBytes.Length)
    End Sub

    Public Sub New( _
       ByVal seqCode As NkSeqCode, _
       ByVal aObjHeaderBytes As Byte(), _
       ByVal oObjFilePathList As List(Of String), _
       ByVal objFilesCombinedLen As Long, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(seqCode, NkCmdCode.DataPostReq, aObjHeaderBytes.Length, oObjFilePathList, objFilesCombinedLen, replyLimitTicks)
        Buffer.BlockCopy(aObjHeaderBytes, 0, Me.RawBytes, ObjPos, aObjHeaderBytes.Length)
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "メソッド"
    'ボディ部の書式違反をチェックするメソッド
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If ObjFilePathList IsNot Nothing Then
            If ObjSize < ObjFilesCombinedLen Then
                Log.Error("ObjSize is invalid.")
                Return NakCauseCode.TelegramError
            End If
        End If

        'ここ以降、プロパティにアクセス可能。

        Return NakCauseCode.None
    End Function

    'ACK電文を生成するメソッド
    Public Function CreateAckTelegram(ByVal status As UShort) As NkDataPostAckTelegram
        Return New NkDataPostAckTelegram(SeqCode, status)
    End Function

    '渡された電文がACKとして整合性があるか判定するメソッド
    Public Overrides Function IsValidAck(ByVal iReplyTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        If oReplyTeleg.SeqCode <> SeqCode Then Return False
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
        Return New NkDataPostAckTelegram(oReplyTeleg)
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As NkDataPostAckTelegram
        Return New NkDataPostAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
