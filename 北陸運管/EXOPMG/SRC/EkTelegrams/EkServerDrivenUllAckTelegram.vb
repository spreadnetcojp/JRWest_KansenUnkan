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

Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' 最も基本的なサーバ主体ULLのACK電文。
''' </summary>
Public Class EkServerDrivenUllAckTelegram
    Inherits EkTelegram
    Implements IXllTelegram

#Region "定数"
    Private Const ContinueCodePos As Integer = 0
    Private Const ContinueCodeLen As Integer = 1
    Private Const FileHashValuePos As Integer = ContinueCodePos + ContinueCodeLen
    Private Const FileHashValueLen As Integer = 32
    Private Const ObjDetailLen As Integer = FileHashValuePos + FileHashValueLen
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
            If EkServerDrivenUllReqTelegram.oContinueCodeTable.TryGetValue(RawBytes(GetRawPos(ContinueCodePos)), code) = False Then
                code = ContinueCode.None
            End If
            Return code
        End Get

        Set(ByVal code As ContinueCode)
            RawBytes(GetRawPos(ContinueCodePos)) = EkServerDrivenUllReqTelegram.oRawContinueCodeTable(code)
        End Set
    End Property

    Public ReadOnly Property RawContinueCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(ContinueCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, GetRawPos(ContinueCodePos), ret, 0, ContinueCodeLen)
            Return ret
        End Get
    End Property

    Public Property FileHashValue() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal fileHashValue As String)
            Array.Clear(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen)
            Encoding.UTF8.GetBytes(fileHashValue, 0, fileHashValue.Length, RawBytes, GetRawPos(FileHashValuePos))
        End Set
    End Property
#End Region

#Region "コンストラクタ"
    'String型のxxxはXxxLen文字以下のASCIIキャラクタで構成される文字列であることが前提です。
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal fileHashValue As String)

        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Get, objCode, ObjDetailLen)
        Me.ContinueCode = continueCode
        Me.FileHashValue = fileHashValue
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

        'NOTE: ハッシュ値は、後の比較でチェックされるはずであるため、ここでの
        'チェックは緩めにする（文字列に変換可能でありさえすればよい）。
        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen) Then
            Log.Error("FileHashValue is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        'ここ以降、プロパティにアクセス可能。

        Return EkNakCauseCode.None
    End Function
#End Region

End Class
