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
Imports System.IO

''' <summary>
''' 駅務機器のプログラムバージョンリストの機種共通部。
''' </summary>
Public Class EkProgramVersionListHeader

#Region "定数"
    Public Const Length As Integer = 30

    Private Const RunnableDatePos As Integer = 3
    Private Const RunnableDateLen As Integer = 4
    Private Const EntireVersionPos As Integer = RunnableDatePos + RunnableDateLen
    Private Const EntireVersionLen As Integer = 4
#End Region

#Region "変数"
    Private RawBytes(Length - 1) As Byte
#End Region

#Region "プロパティ"
    Public Property RunnableDate() As DateTime
        Get
            Dim yyyymmdd As Integer = Utility.GetIntFromBcdBytes(RawBytes, RunnableDatePos, RunnableDateLen)
            Dim sTime As String = yyyymmdd.ToString("D8")
            Return DateTime.ParseExact(sTime, "yyyyMMdd", CultureInfo.InvariantCulture)
        End Get

        Set(ByVal appDate As DateTime)
            Dim sDate As String = appDate.ToString("yyyyMMdd")
            Dim yyyymmdd As Integer = Integer.Parse(sDate)
            Utility.CopyIntToBcdBytes(yyyymmdd, RawBytes, RunnableDatePos, RunnableDateLen)
        End Set
    End Property

    Public ReadOnly Property RawRunnableDate() As Byte()
        Get
            Dim ret As Byte() = New Byte(RunnableDateLen - 1) {}
            Buffer.BlockCopy(RawBytes, RunnableDatePos, ret, 0, RunnableDateLen)
            Return ret
        End Get
    End Property

    Public Property EntireVersion() As Integer
        Get
            Return Utility.GetIntFromBcdBytes(RawBytes, EntireVersionPos, EntireVersionLen)
        End Get

        Set(ByVal ver As Integer)
            Utility.CopyIntToBcdBytes(ver, RawBytes, EntireVersionPos, EntireVersionLen)
        End Set
    End Property

    Public ReadOnly Property RawEntireVersion() As Byte()
        Get
            Dim ret As Byte() = New Byte(EntireVersionLen - 1) {}
            Buffer.BlockCopy(RawBytes, EntireVersionPos, ret, 0, EntireVersionLen)
            Return ret
        End Get
    End Property
#End Region

#Region "メソッド"
    'NOTE: sFilePathにファイルがない場合や、ファイルの長さが短い場合などには、
    'IOExceptionをスローします。
    Public Sub New(ByVal sFilePath As String)
        Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
            Dim pos As Integer = 0
            Do
                Dim readLimit As Integer = Length - pos
                If readLimit = 0 Then Exit Do
                Dim readSize As Integer = oInputStream.Read(RawBytes, pos, readLimit)
                If readSize = 0 Then
                    Throw New EndOfStreamException()
                End If
                pos += readSize
            Loop
        End Using
    End Sub

    Public Function GetFormatViolation() As String
        If Not Utility.IsBcdBytes(RawBytes, RunnableDatePos, RunnableDateLen) Then
            Return "RunnableDate is invalid (not BCD bytes)."
        End If

        Dim yyyymmdd As Integer = Utility.GetIntFromBcdBytes(RawBytes, RunnableDatePos, 4)
        Dim sRunnableDate As String = yyyymmdd.ToString("D8")
        Dim oRunnableDate As DateTime
        If DateTime.TryParseExact(sRunnableDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, oRunnableDate) = False Then
            Return "RunnableDate is invalid (not a time)."
        End If

        If Not Utility.IsBcdBytes(RawBytes, EntireVersionPos, EntireVersionLen) Then
            Return "EntireVersion is invalid (not BCD bytes)."
        End If

        Return Nothing
    End Function
#End Region

End Class
