' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/08/08  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class ProgramVersionListUtil

    Private Class FieldRef
        Public Field As XlsField
        Public BitOffset As Integer
        Public Index As Integer

        Public Sub New(ByVal oField As XlsField, ByVal bitOfs As Integer, ByVal i As Integer)
            Field = oField
            BitOffset = bitOfs
            Index = i
        End Sub
    End Class

    Private Shared oFieldRefs As Dictionary(Of String, FieldRef)
    Private Shared totalBitCount As Integer

    Private Shared ReadOnly oFields As XlsField() = New XlsField() { _
        New XlsField(8*1, "X2", 1, " "c, "共通部 ユーザコード", "CompanyCode"), _
        New XlsField(8*1, "X2", 1, " "c, "共通部 適用エリア", "IcArea"), _
        New XlsField(8*1, "X2", 1, " "c, "共通部 プログラム区分", "ProgramDistribution"), _
        New XlsField(8*4, "X8", 1, " "c, "共通部 プログラム動作許可日"), _
        New XlsField(8*1, "X2", 4, " "c, "共通部 プログラム全体Ver（新）"), _
        New XlsField(8*1, "X2", 4, " "c, "共通部 プログラム全体Ver（現）"), _
        New XlsField(8*1, "X2", 15, " "c, "共通部 予備"), _
        New XlsField(8*2, "X4", 1, " "c, "在来IC判定バージョン(Suica)"), _
        New XlsField(8*2, "X4", 1, " "c, "在来IC判定バージョン(TOICA)"), _
        New XlsField(8*2, "X4", 1, " "c, "在来IC判定バージョン(ICOCA)"), _
        New XlsField(8*2, "X4", 1, " "c, "新幹線IC判定バージョン"), _
        New XlsField(8*2, "X4", 1, " "c, "EXIC判定バージョン"), _
        New XlsField(8*2, "A", 1, " "c, "Suica運賃データ世代1バージョン"), _
        New XlsField(8*4, "X8", 1, " "c, "Suica運賃データ世代1適用年月日"), _
        New XlsField(8*2, "A", 1, " "c, "Suica運賃データ世代2バージョン"), _
        New XlsField(8*4, "X8", 1, " "c, "Suica運賃データ世代2適用年月日"), _
        New XlsField(8*15, "A", 1, " "c, "Suica運賃データ名"), _
        New XlsField(8*3, "A", 1, " "c, "Suica運賃データ全体ソフト型式"), _
        New XlsField(8*2, "A", 1, " "c, "Suica運賃データバージョン"), _
        New XlsField(8*6, "X12", 1, " "c, "Suica運賃データ作成年月日"), _
        New XlsField(8*2, "A", 1, " "c, "TOICA運賃データ世代1バージョン"), _
        New XlsField(8*4, "X8", 1, " "c, "TOICA運賃データ世代1適用年月日"), _
        New XlsField(8*2, "A", 1, " "c, "TOICA運賃データ世代2バージョン"), _
        New XlsField(8*4, "X8", 1, " "c, "TOICA運賃データ世代2適用年月日"), _
        New XlsField(8*15, "A", 1, " "c, "TOICA運賃データ名"), _
        New XlsField(8*3, "A", 1, " "c, "TOICA運賃データ全体ソフト型式"), _
        New XlsField(8*2, "A", 1, " "c, "TOICA運賃データバージョン"), _
        New XlsField(8*6, "X12", 1, " "c, "TOICA運賃データ作成年月日"), _
        New XlsField(8*2, "A", 1, " "c, "ICOCA運賃データ世代1バージョン"), _
        New XlsField(8*4, "X8", 1, " "c, "ICOCA運賃データ世代1適用年月日"), _
        New XlsField(8*2, "A", 1, " "c, "ICOCA運賃データ世代2バージョン"), _
        New XlsField(8*4, "X8", 1, " "c, "ICOCA運賃データ世代2適用年月日"), _
        New XlsField(8*15, "A", 1, " "c, "ICOCA運賃データ名"), _
        New XlsField(8*3, "A", 1, " "c, "ICOCA運賃データ全体ソフト型式"), _
        New XlsField(8*2, "A", 1, " "c, "ICOCA運賃データバージョン"), _
        New XlsField(8*6, "X12", 1, " "c, "ICOCA運賃データ作成年月日"), _
        New XlsField(8*2, "A", 1, " "c, "特急料金データ世代1バージョン"), _
        New XlsField(8*4, "X8", 1, " "c, "特急料金データ世代1適用年月日"), _
        New XlsField(8*2, "A", 1, " "c, "特急料金データ世代2バージョン"), _
        New XlsField(8*4, "X8", 1, " "c, "特急料金データ世代2適用年月日"), _
        New XlsField(8*15, "A", 1, " "c, "特急料金データ名"), _
        New XlsField(8*3, "A", 1, " "c, "特急料金データ全体ソフト型式"), _
        New XlsField(8*2, "A", 1, " "c, "特急料金データバージョン"), _
        New XlsField(8*6, "X12", 1, " "c, "特急料金データ作成年月日"), _
        New XlsField(8*2, "X4", 1, " "c, "磁気ファームウェアバージョン"), _
        New XlsField(8*1, "X2", 14, " "c, "予備"), _
        New XlsField(8*1, "X2", 1, " "c, "未締切時プログラム非適用チェックフラグ"), _
        New XlsField(8*1, "X2", 1, " "c, "未送有時プログラム非適用チェックフラグ"), _
        New XlsField(8*1, "X2", 46, " "c, "備考")}

    Shared Sub New()
        oFieldRefs = New Dictionary(Of String, FieldRef)
        Dim bits As Integer = 0
        For i As Integer = 0 To oFields.Length - 1
            Dim oField As XlsField = oFields(i)
            oFieldRefs.Add(oField.MetaName, New FieldRef(oField, bits, i))
            bits += oField.ElementBits * oField.ElementCount
        Next i
        totalBitCount = bits
    End Sub

    Public Shared ReadOnly Property RecordLengthInBits As Integer
        Get
            Return totalBitCount
        End Get
    End Property

    Public Shared ReadOnly Property RecordLengthInBytes As Integer
        Get
            Return (totalBitCount + 7) \ 8
        End Get
    End Property

    Public Shared ReadOnly Property Fields As XlsField()
        Get
            Return oFields
        End Get
    End Property

    Public Shared ReadOnly Property Field(ByVal sMetaName As String) As XlsField
        Get
            Return oFieldRefs(sMetaName).Field
        End Get
    End Property

    Public Shared Function FieldIndexOf(ByVal sMetaName As String) As Integer
        Return oFieldRefs(sMetaName).Index
    End Function

    Public Shared Function GetFieldValueFromBytes(ByVal sMetaName As String, ByVal oBytes As Byte()) As String
        Dim oRef As FieldRef = oFieldRefs(sMetaName)
        Return oRef.Field.CreateValueFromBytes(oBytes, oRef.BitOffset)
    End Function

    Public Shared Sub SetFieldValueToBytes(ByVal sMetaName As String, ByVal sValue As String, ByVal oBytes As Byte())
        Dim oRef As FieldRef = oFieldRefs(sMetaName)
        oRef.Field.CopyValueToBytes(sValue, oBytes, oRef.BitOffset)
    End Sub

End Class
