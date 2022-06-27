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
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' 整時データ取得ACK電文。
''' </summary>
Public Class EkTimeDataGetAckTelegram
    Inherits EkTelegram

#Region "定数"
    Private Const TimeDataFormat As String = "yyyyMMddHHmmssfff"
    Private Const TimeDataPos As Integer = 0
    Private Const TimeDataLen As Integer = 17
    Private Const ObjDetailLen As Integer = TimeDataLen
#End Region

#Region "プロパティ"
    Public Property TimeData() As DateTime
        Get
            Dim sTimeData As String = Encoding.UTF8.GetString(RawBytes, GetRawPos(TimeDataPos), TimeDataLen)
            Return DateTime.ParseExact(sTimeData, TimeDataFormat, CultureInfo.InvariantCulture)
        End Get

        Set(ByVal oTimeData As DateTime)
            Dim sTimeData As String = oTimeData.ToString(TimeDataFormat)
            Encoding.UTF8.GetBytes(sTimeData, 0, TimeDataLen, RawBytes, GetRawPos(TimeDataPos))
        End Set
    End Property
#End Region

#Region "コンストラクタ"
    'oTimeDataはTimeDataLen文字以下のASCIIキャラクタで構成される文字列であることが前提です。
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal oTimeData As DateTime)
        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Get, objCode, ObjDetailLen)
        Me.TimeData = oTimeData
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

        If Not Utility.IsDecimalAsciiBytesFixed(RawBytes, GetRawPos(TimeDataPos), TimeDataLen) Then
            Log.Error("TimeData is invalid (not decimal ASCII bytes).")
            Return EkNakCauseCode.TelegramError
        End If

        Dim sTimeData As String = Encoding.UTF8.GetString(RawBytes, GetRawPos(TimeDataPos), TimeDataLen)
        Dim oTimeData As DateTime
        If DateTime.TryParseExact(sTimeData, TimeDataFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, oTimeData) = False Then
            Log.Error("TimeData is invalid (not a time).")
            Return EkNakCauseCode.TelegramError
        End If

        'ここ以降、プロパティにアクセス可能。

        Return EkNakCauseCode.None
    End Function
#End Region

End Class
