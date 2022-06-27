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
''' 開局レスポンス電文。
''' </summary>
Public Class NkComStartAckTelegram
    Inherits NkTelegram

#Region "定数"
    Private Const ObjLen As Integer = 0
#End Region

#Region "プロパティ"
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal seqCode As NkSeqCode)
        MyBase.New(seqCode, NkCmdCode.ComStartAck, ObjLen)
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
