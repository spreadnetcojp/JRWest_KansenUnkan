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
''' 運管サーバと窓処の間のログ（操作ログ・業務前認証ログ）送付ACK電文。
''' </summary>
Public Class EkMadoLogPostAckTelegram
    Inherits EkTelegram

#Region "定数"
    Private Const LogDataDecisionCodePos As Integer = 0
    Private Const LogDataDecisionCodeLen As Integer = 1
    Private Const ObjDetailLen As Integer = LogDataDecisionCodePos + LogDataDecisionCodeLen
#End Region

#Region "プロパティ"
    Public Property LogDataDecisionCode() As Integer
        Get
            Return RawBytes(GetRawPos(LogDataDecisionCodePos))
        End Get

        Set(ByVal decisionCode As Integer)
            RawBytes(GetRawPos(LogDataDecisionCodePos)) = CByte(decisionCode)
        End Set
    End Property
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal oGene As EkTelegramGene, ByVal objCode As Integer, ByVal decisionCode As Integer)
        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Post, objCode, ObjDetailLen)

        Me.LogDataDecisionCode = decisionCode
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
#End Region

End Class
