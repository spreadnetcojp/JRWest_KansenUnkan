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

Imports System.Globalization

Imports JR.ExOpmg.Common

''' <summary>
''' 折り返しシーケンスの回答レスポンス電文。
''' </summary>
Public Class NkTestAckTelegram
    Inherits NkTelegram

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
    Public Sub New(ByVal aTestData As Byte())
        MyBase.New(NkSeqCode.Test, NkCmdCode.DataPostAck, If(aTestData Is Nothing, 0, aTestData.Length))
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
#End Region

End Class
