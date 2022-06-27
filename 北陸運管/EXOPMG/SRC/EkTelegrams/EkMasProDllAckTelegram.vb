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
''' マスタファイルまたはプログラムファイルをDLLする際の専用ACK電文。
''' </summary>
Public Class EkMasProDllAckTelegram
    Inherits EkTelegram
    Implements IXllTelegram

#Region "定数"
    Private Const ContinueCodePos As Integer = 0
    Private Const ContinueCodeLen As Integer = 1
    Private Const ObjDetailLen As Integer = ContinueCodeLen
#End Region

#Region "プロパティ"
    Private ReadOnly Property __ContinueCode() As ContinueCode Implements IXllTelegram.ContinueCode
        Get
            Return ContinueCode
        End Get
    End Property

    Public Property ContinueCode() As ContinueCode
        Get
            Dim code As ContinueCode
            If EkMasProDllReqTelegram.oContinueCodeTable.TryGetValue(RawBytes(GetRawPos(ContinueCodePos)), code) = False Then
                code = ContinueCode.None
            End If
            Return code
        End Get

        Set(ByVal code As ContinueCode)
            RawBytes(GetRawPos(ContinueCodePos)) = EkMasProDllReqTelegram.oRawContinueCodeTable(code)
        End Set
    End Property

    Public ReadOnly Property RawContinueCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(ContinueCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, GetRawPos(ContinueCodePos), ret, 0, ContinueCodeLen)
            Return ret
        End Get
    End Property
#End Region

#Region "コンストラクタ"
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal continueCode As ContinueCode)

        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Get, objCode, ObjDetailLen)
        Me.ContinueCode = continueCode
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
