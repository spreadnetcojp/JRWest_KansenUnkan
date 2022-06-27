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
''' NAK電文。
''' </summary>
Public Class EkNakTelegram
    Inherits EkTelegram
    Implements INakTelegram

#Region "定数"
    Private Const CauseNumberPos As Integer = 0
    Private Const CauseNumberLen As Integer = 3
    Private Const CauseTextPos As Integer = CauseNumberPos + CauseNumberLen
    Private Const CauseTextLen As Integer = 47
    Private Const ObjDetailLen As Integer = CauseTextPos + CauseTextLen
#End Region

#Region "プロパティ"
    Private ReadOnly Property __CauseCode() As NakCauseCode Implements INakTelegram.CauseCode
        Get
            Return CauseCode
        End Get
    End Property

    Public Property CauseCode() As NakCauseCode
        Get
            Return New EkNakCauseCode(CauseNumber, CauseText)
        End Get

        Set(ByVal causeCode As NakCauseCode)
            Dim rawCauseCode As Byte() = EkNakCauseCode.GetDefaultRawBytes(causeCode)
            Buffer.BlockCopy(rawCauseCode, 0, RawBytes, GetRawPos(CauseNumberPos), rawCauseCode.Length)

            If TypeOf causeCode Is EkNakCauseCode Then
                Dim realCauseCode As EkNakCauseCode = DirectCast(causeCode, EkNakCauseCode)

                If realCauseCode.RawNumber <> -1 Then
                    CauseNumber = realCauseCode.RawNumber
                End If

                If realCauseCode.RawText IsNot Nothing Then
                    CauseText = realCauseCode.RawText
                End If
            End If
        End Set
    End Property

    Public Property CauseNumber() As Integer
        Get
            Return Utility.GetIntFromDecimalAsciiBytes(RawBytes, GetRawPos(CauseNumberPos), CauseNumberLen)
        End Get

        Set(ByVal causeNumber As Integer)
            Utility.CopyIntToDecimalAsciiBytes(causeNumber, RawBytes, GetRawPos(CauseNumberPos), CauseNumberLen)
        End Set
    End Property

    Public Property CauseText() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(CauseTextPos), CauseTextLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal sCauseText As String)
            Array.Clear(RawBytes, GetRawPos(CauseTextPos), CauseTextLen)
            Encoding.UTF8.GetBytes(sCauseText, 0, sCauseText.Length, RawBytes, GetRawPos(CauseTextPos))
        End Set
    End Property
#End Region

#Region "コンストラクタ"
    'NOTE: oSourceTelegにフォーマット異常がある場合（GetHeaderFormatViolationの呼び出しで
    'NakCauseCode.None以外が返ってくる場合）においても、実行可能です。
    Public Sub New(ByVal oGene As EkTelegramGene, ByVal oSourceTeleg As EkTelegram, ByVal causeCode As NakCauseCode)
        MyBase.New(oGene, EkCmdCode.Nak, oSourceTeleg.RawSubCmdCode, oSourceTeleg.RawObjCode, ObjDetailLen)
        Me.CauseCode = causeCode
    End Sub

    'NOTE: oSourceTelegにフォーマット異常がある場合（GetHeaderFormatViolationの呼び出しで
    'NakCauseCode.None以外が返ってくる場合）においても、実行可能です。
    Public Sub New(ByVal oGene As EkTelegramGene, ByVal oSourceTeleg As EkTelegram, ByVal causeNumber As Integer, ByVal sCauseText As String)
        MyBase.New(oGene, EkCmdCode.Nak, oSourceTeleg.RawSubCmdCode, oSourceTeleg.RawObjCode, ObjDetailLen)
        Me.CauseNumber = causeNumber
        Me.CauseText = sCauseText
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

        If Not Utility.IsDecimalAsciiBytes(RawBytes, GetRawPos(CauseNumberPos), CauseNumberLen) Then
            Log.Error("CauseNumber is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(CauseTextPos), CauseTextLen) Then
            Log.Error("CauseText is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        Return EkNakCauseCode.None
    End Function
#End Region

End Class

'NOTE: 下記クラスは、インスタンスに実際のNAK事由番号やNAK事由文言が含まれない
'場合があるため、NumberやTextのようなプロパティは用意しない。受信NAK電文の
'事由番号や事由文言を参照したい場合は、受信NAK電文自体のCauseNumberプロパティや
'CauseTextプロパティを参照する方針である。
Public Class EkNakCauseCode
    Inherits NakCauseCode

    Public Const NoData As String = "NoData"
    Public Const NoTime As String = "NoTime"
    Public Const Unnecessary As String = "Unnecessary"
    Public Const InvalidContent As String = "InvalidContent"
    Public Const UnknownLight As String = "UnknownLight"

    Public Const NotPermit As String = "NotPermit"
    Public Const HashValueError As String = "HashValueError"
    Public Const UnknownFatal As String = "UnknownFatal"

    Protected Shared ReadOnly oCauseCodeTable As New Dictionary(Of Integer, String) From { _
       {101, EkNakCauseCode.Busy}, _
       {102, EkNakCauseCode.NoData}, _
       {103, EkNakCauseCode.NoTime}, _
       {104, EkNakCauseCode.Unnecessary}, _
       {105, EkNakCauseCode.InvalidContent}, _
       {901, EkNakCauseCode.TelegramError}, _
       {902, EkNakCauseCode.NotPermit}, _
       {903, EkNakCauseCode.HashValueError}}
    Protected Shared ReadOnly oRawCauseCodeTable As New Dictionary(Of String, Byte()) From { _
       {EkNakCauseCode.Busy, Encoding.UTF8.GetBytes("101BUSY")}, _
       {EkNakCauseCode.NoData, Encoding.UTF8.GetBytes("102NO DATA")}, _
       {EkNakCauseCode.NoTime, Encoding.UTF8.GetBytes("103NO TIME")}, _
       {EkNakCauseCode.Unnecessary, Encoding.UTF8.GetBytes("104UNNECESSARY")}, _
       {EkNakCauseCode.InvalidContent, Encoding.UTF8.GetBytes("105INVALID CONTENT")}, _
       {EkNakCauseCode.TelegramError, Encoding.UTF8.GetBytes("901ERROR")}, _
       {EkNakCauseCode.NotPermit, Encoding.UTF8.GetBytes("902NOT PERMIT")}, _
       {EkNakCauseCode.HashValueError, Encoding.UTF8.GetBytes("903MD5 ERROR")}}

    Protected Friend RawNumber As Integer
    Protected Friend RawText As String

    Public Overrides Function ToString() As String
        If RawText IsNot Nothing
            Return key & " - "  & RawText
        Else
            Return key
        End If
    End Function

    Public Overloads Shared Operator =(ByVal c1 As EkNakCauseCode, ByVal c2 As EkNakCauseCode) As Boolean
        If c1.RawNumber <> -1 AndAlso c2.RawNumber <> -1 Then
            Return c1.RawNumber = c2.RawNumber
        Else
            Return c1.key.Equals(c2.key)
        End If
    End Operator

    Public Overloads Shared Operator <>(ByVal c1 As EkNakCauseCode, ByVal c2 As EkNakCauseCode) As Boolean
        If c1.RawNumber <> -1 AndAlso c2.RawNumber <> -1 Then
            Return c1.RawNumber <> c2.RawNumber
        Else
            Return Not c1.key.Equals(c2.key)
        End If
    End Operator

    Public Overloads Shared Widening Operator CType(ByVal key As String) As EkNakCauseCode
        Return New EkNakCauseCode(key)
    End Operator

    Public Sub New(ByVal key As String)
        MyBase.New()
        Me.key = key
        Me.RawNumber = -1
        Me.RawText = Nothing
    End Sub

    Public Sub New(ByVal rawNumber As Integer, ByVal rawText As String)
        MyBase.New()

        If oCauseCodeTable.TryGetValue(rawNumber, Me.key) = False Then
            If rawNumber < 200 Then
                Me.key = EkNakCauseCode.UnknownLight
            Else
                Me.key = EkNakCauseCode.UnknownFatal
            End If
        End If

        Debug.Assert(rawNumber >= 0 And rawNumber <= 999)
        Me.RawNumber = rawNumber
        Me.RawText = rawText
    End Sub

    Friend Shared Function GetDefaultRawBytes(ByVal causeCode As NakCauseCode) As Byte()
        If oRawCauseCodeTable.ContainsKey(causeCode.key) Then
            Return oRawCauseCodeTable(causeCode.key)
        Else
            Return Encoding.UTF8.GetBytes("000")
        End If
    End Function
End Class
