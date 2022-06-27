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
''' ウォッチドッグREQ電文。
''' </summary>
Public Class EkWatchdogReqTelegram
    Inherits EkReqTelegram
    Implements IWatchdogReqTelegram

#Region "定数"
    Public Const FormalObjCodeInKanshiban As Byte = &H0
    Public Const FormalObjCodeInOpClient As Byte = &H0
    Public Const FormalObjCodeInTokatsu As Byte = &H0
    Public Const FormalObjCodeInMadosho As Byte = &H0

    Private Const ObjDetailLen As Integer = 0
#End Region

#Region "プロパティ"
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal oGene As EkTelegramGene, ByVal objCode As Integer, ByVal replyLimitTicks As Integer)
        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "メソッド"
    'ボディ部の書式違反をチェックするメソッド
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If GetObjDetailLen() <> ObjDetailLen Then
            Log.Error("ObjSize is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        'ここ以降、プロパティにアクセス可能。

        Return EkNakCauseCode.None
    End Function

    'ACK電文を生成するメソッド
    Private Function CreateIAckTelegram() As ITelegram Implements IWatchdogReqTelegram.CreateAckTelegram
        Return New EkWatchdogAckTelegram(Gene, ObjCode)
    End Function

    'ACK電文を生成するメソッド
    Public Function CreateAckTelegram() As EkWatchdogAckTelegram
        Return New EkWatchdogAckTelegram(Gene, ObjCode)
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkWatchdogAckTelegram(oReplyTeleg)
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkWatchdogAckTelegram
        Return New EkWatchdogAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
