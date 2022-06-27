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
''' 任意バイト列取得ACK電文。
''' </summary>
Public Class EkByteArrayGetAckTelegram
    Inherits EkTelegram

#Region "定数"
    Private Const ByteArrayPos As Integer = 0
#End Region

#Region "プロパティ"
    Public ReadOnly Property ByteArray() As Byte()
        Get
            Dim len As Integer = GetObjDetailLen() - ByteArrayPos
            Dim aBytes As Byte() = New Byte(len - 1) {}
            Buffer.BlockCopy(RawBytes, GetRawPos(ByteArrayPos), aBytes, 0, len)
            Return aBytes
        End Get
    End Property
#End Region

#Region "コンストラクタ"
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal aBytes As Byte())
        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Get, objCode, aBytes.Length)
        Buffer.BlockCopy(aBytes, 0, Me.RawBytes, GetRawPos(ByteArrayPos), aBytes.Length)
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "メソッド"
    'ボディ部の書式違反をチェックするメソッド
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        'If GetObjDetailLen() < 1 Then
        '    Log.Error("ObjSize is invalid.")
        '    Return EkNakCauseCode.TelegramError
        'End If

        'ここ以降、プロパティにアクセス可能。

        Return EkNakCauseCode.None
    End Function
#End Region

End Class
