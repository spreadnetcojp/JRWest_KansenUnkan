' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/07/18  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions

Imports JR.ExOpmg.Common

'TODO: EkMasterDataFileFooterをリファクタリングして活用する。

''' <summary>
''' 駅務機器のマスタファイルのフッタ。
''' </summary>
Public Structure ExMasterDataFooter

#Region "定数"
    Public Const Length As Integer = 96

    Private Const KindPrefix As String = "PR_"
    Private Shared ReadOnly PrefixedKindRegx As New Regex("^PR_[A-Z]{3}$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Private Const ApplicableSpecificModelPos As Integer = 0
    Private Const ApplicableSpecificModelLen As Integer = 8
    Private Const PrefixedKindPos As Integer = ApplicableSpecificModelPos + ApplicableSpecificModelLen
    Private Const PrefixedKindLen As Integer = 8
    Private Const CreatedTimePos As Integer = PrefixedKindPos + PrefixedKindLen
    Private Const CreatedTimeLen As Integer = 6
    Private Const VersionPos As Integer = CreatedTimePos + CreatedTimeLen
    Private Const VersionLen As Integer = 2
    Private Const DispNamePos As Integer = VersionPos + VersionLen
    Private Const DispNameLen As Integer = 20
    Private Const DispTimePos As Integer = DispNamePos + DispNameLen
    Private Const DispTimeLen As Integer = 20
    Private Const DispReservedAreaPos As Integer = DispTimePos + DispTimeLen
    Private Const DispReservedAreaLen As Integer = 24
    Private Const DispDataPos As Integer = DispNamePos
    Private Const DispDataLen As Integer = 64
    Private Const SumCheckLengthPos As Integer = DispReservedAreaPos + DispReservedAreaLen
    Private Const SumCheckLengthLen As Integer = 4
    Private Const SumValuePos As Integer = SumCheckLengthPos + SumCheckLengthLen
    Private Const SumValueLen As Integer = 4
#End Region

#Region "変数"
    Private RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public Property ApplicableModel() As String
        Get
            Dim sRaw As String = ApplicableSpecificModel
            If sRaw.Equals(EkConstants.SpecificCodeOfGate) Then
                Return "G"
            ElseIf sRaw.Equals(EkConstants.SpecificCodeOfMadosho) Then
                Return "Y"
            Else
                Return Nothing
            End If
        End Get

        Set(ByVal sModel As String)
            Dim sRaw As String
            If sModel.Equals("G") Then
                sRaw = EkConstants.SpecificCodeOfGate
            ElseIf sModel.Equals("Y")
                sRaw = EkConstants.SpecificCodeOfMadosho
            Else
                sRaw = ""
            End If
            Utility.FillBytes(&H20, RawBytes, ApplicableSpecificModelPos, ApplicableSpecificModelLen)
            Encoding.UTF8.GetBytes(sRaw, 0, sRaw.Length, RawBytes, ApplicableSpecificModelPos)
        End Set
    End Property

    Public ReadOnly Property ApplicableSpecificModel() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, ApplicableSpecificModelPos, ApplicableSpecificModelLen).TrimEnd(Chr(&H20))
        End Get
    End Property

    Public Property Kind() As String
        Get
            Dim sRaw As String = PrefixedKind
            If PrefixedKindRegx.IsMatch(sRaw) Then
                Return sRaw.Substring(KindPrefix.Length)
            Else
                Return Nothing
            End If
        End Get

        Set(ByVal sKind As String)
            Dim sRaw As String = KindPrefix & sKind
            Utility.FillBytes(&H20, RawBytes, PrefixedKindPos, PrefixedKindLen)
            Encoding.UTF8.GetBytes(sRaw, 0, sRaw.Length, RawBytes, PrefixedKindPos)
        End Set
    End Property

    Public ReadOnly Property PrefixedKind() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, PrefixedKindPos, PrefixedKindLen).TrimEnd(Chr(&H20))
        End Get
    End Property

    Public Property CreatedTime() As DateTime
        Get
            Dim yyyymmdd As Integer = Utility.GetIntFromBcdBytes(RawBytes, CreatedTimePos, 4)
            Dim hhmm As Integer = Utility.GetIntFromBcdBytes(RawBytes, CreatedTimePos + 4, 2)
            Dim sTime As String = yyyymmdd.ToString("D8") & hhmm.ToString("D4")
            Return DateTime.ParseExact(sTime, "yyyyMMddHHmm", CultureInfo.InvariantCulture)
        End Get

        Set(ByVal time As DateTime)
            Dim sTime As String = time.ToString("yyyyMMddHHmm")
            Dim yyyymmdd As Integer = Integer.Parse(sTime.SubString(0, 8))
            Dim hhmm As Integer = Integer.Parse(sTime.SubString(8, 4))
            Utility.CopyIntToBcdBytes(yyyymmdd, RawBytes, CreatedTimePos, 4)
            Utility.CopyIntToBcdBytes(hhmm, RawBytes, CreatedTimePos + 4, 2)
        End Set
    End Property

    Public ReadOnly Property RawCreatedTime() As Byte()
        Get
            Dim ret As Byte() = New Byte(CreatedTimeLen - 1) {}
            Buffer.BlockCopy(RawBytes, CreatedTimePos, ret, 0, CreatedTimeLen)
            Return ret
        End Get
    End Property

    Public Property Version() As String
        Get
            Return Utility.GetIntFromBcdBytes(RawBytes, VersionPos, VersionLen).ToString("D3")
        End Get

        Set(ByVal sVersion As String)
            Dim intValue As Integer = Integer.Parse(sVersion)
            Utility.CopyIntToBcdBytes(intValue, RawBytes, VersionPos, VersionLen)
        End Set
    End Property

    Public Property DispName() As String
        Get
            Return Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetString(RawBytes, DispNamePos, DispNameLen).TrimEnd(Chr(&H20))
        End Get

        Set(ByVal sDispName As String)
            Utility.FillBytes(&H20, RawBytes, DispNamePos, DispNameLen)
            Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetBytes(sDispName, 0, sDispName.Length, RawBytes, DispNamePos)
        End Set
    End Property

    Public Property DispTime() As String
        Get
            Return Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetString(RawBytes, DispTimePos, DispTimeLen).TrimEnd(Chr(&H20))
        End Get

        Set(ByVal sDispTime As String)
            Utility.FillBytes(&H20, RawBytes, DispTimePos, DispTimeLen)
            Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetBytes(sDispTime, 0, sDispTime.Length, RawBytes, DispTimePos)
        End Set
    End Property

    Public Property DispData() As String
        Get
            Return Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetString(RawBytes, DispDataPos, DispDataLen).TrimEnd(Chr(&H20))
        End Get

        Set(ByVal sDispData As String)
            Utility.FillBytes(&H20, RawBytes, DispDataPos, DispDataLen)
            Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetBytes(sDispData, 0, sDispData.Length, RawBytes, DispDataPos)
        End Set
    End Property

    Public Property SumCheckLength() As UInteger
        Get
            Return Utility.GetUInt32FromLeBytes4(RawBytes, SumCheckLengthPos)
        End Get

        Set(ByVal checkLength As UInteger)
            Utility.CopyUInt32ToLeBytes4(checkLength, RawBytes, SumCheckLengthPos)
        End Set
    End Property

    Public Property SumValue() As UInteger
        Get
            Return Utility.GetUInt32FromLeBytes4(RawBytes, SumValuePos)
        End Get

        Set(ByVal value As UInteger)
            Utility.CopyUInt32ToLeBytes4(value, RawBytes, SumValuePos)
        End Set
    End Property
#End Region

#Region "メソッド"
    Public Sub New(ByVal oRawBytes As Byte())
        RawBytes = oRawBytes
    End Sub

    Public Function GetFormatViolation() As String
        If Not Utility.IsVisibleAsciiBytesFixed(RawBytes, ApplicableSpecificModelPos, ApplicableSpecificModelLen) Then
            Return "ApplicableSpecificModel is invalid (not ASCII bytes)."
        End If

        If ApplicableModel Is Nothing Then
            Return "ApplicableSpecificModel is invalid (unidentified model)."
        End If

        If Not Utility.IsVisibleAsciiBytesFixed(RawBytes, PrefixedKindPos, PrefixedKindLen) Then
            Return "PrefixedKind is invalid (not ASCII bytes)."
        End If

        If Kind Is Nothing Then
            Return "PrefixedKind is invalid (illegal prefix)."
        End If

        If Not Utility.IsBcdBytes(RawBytes, CreatedTimePos, CreatedTimeLen) Then
            Return "CreatedTime is invalid (not BCD bytes)."
        End If

        Dim yyyymmdd As Integer = Utility.GetIntFromBcdBytes(RawBytes, CreatedTimePos, 4)
        Dim hhmm As Integer = Utility.GetIntFromBcdBytes(RawBytes, CreatedTimePos + 4, 2)
        Dim sCreatedTime As String = yyyymmdd.ToString("D8") & hhmm.ToString("D4")
        Dim oCreatedTime As DateTime
        If DateTime.TryParseExact(sCreatedTime, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, oCreatedTime) = False Then
            Return "CreatedTime is invalid (not a time)."
        End If

        'NOTE: これの範囲チェックは、呼び元が行う。
        If Not Utility.IsBcdBytes(RawBytes, VersionPos, VersionLen) Then
            Return "Version is invalid (not BCD bytes)."
        End If

        Try
            'NOTE: プロパティのゲッタに副作用があってはならない（コンパイラは
            'そのように想定してよい）などの規定があるなら、オミットされる
            '可能性があるが、さすがにそのような規定はないものと想定している。
            Dim sDispData As String = DispData
        Catch ex As DecoderFallbackException
            Return "DispData is invalid."
        End Try

        Return Nothing
    End Function
#End Region

End Structure
