' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/11/21  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class FaultDataUtil

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
        New XlsField(8*1, "X2", 1, " "c, "基本ヘッダー データ種別", "DataKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "基本ヘッダー 駅コード", "Station"), _
        New XlsField(8*7, "X14", 1, " "c, "基本ヘッダー 処理日時"), _
        New XlsField(8*1, "D", 1, " "c, "基本ヘッダー コーナー"), _
        New XlsField(8*1, "D", 1, " "c, "基本ヘッダー 号機"), _
        New XlsField(8*4, "D", 1, " "c, "基本ヘッダー シーケンスNo", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*1, "X2", 1, " "c, "基本ヘッダー バージョン"), _
        New XlsField(8*4, "D", 1, " "c, "データレングス", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*8, "X16", 1, " "c, "発生日時"), _
        New XlsField(8*1, "X2", 1, " "c, "号機番号"), _
        New XlsField(8*1, "X2", 1, " "c, "通路方向", "PassDirection"), _
        New XlsField(8*4, "X8", 1, " "c, "エラーコード", "FaultDataErrorCode"), _
        New XlsField(8*1, "X2", 1, " "c, "異常履歴指定"), _
        New XlsField(8*1, "X2", 1, " "c, "異常種別"), _
        New XlsField(8*1, "X2", 1, " "c, "リセットランプ情報"), _
        New XlsField(8*1, "X2", 1, " "c, "旅客案内釦情報"), _
        New XlsField(8*4, "D", 1, " "c, "異常項目 有効バイト数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*40, "S932", 1, " "c, "異常項目 表示データ"), _
        New XlsField(8*4, "D", 1, " "c, "４文字表示 有効バイト数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*12, "S932", 1, " "c, "４文字表示 表示データ"), _
        New XlsField(8*4, "D", 1, " "c, "可変表示部 有効バイト数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*540, "S932", 1, " "c, "可変表示部 表示データ"), _
        New XlsField(8*4, "D", 1, " "c, "処置内容 有効バイト数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*150, "S932", 1, " "c, "処置内容 表示データ")}

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

    Public Shared Sub AdjustByteCountField(ByVal sSuperName As String, ByVal oBytes As Byte())
        Dim sDataFieldName As String = sSuperName & " 表示データ"
        Dim sLenFieldName As String = sSuperName & " 有効バイト数"
        Dim sDataValue As String = GetFieldValueFromBytes(sDataFieldName, oBytes)
        Dim sLenValue As String = MyUtility.GetValidByteCount(Field(sDataFieldName), sDataValue).ToString()
        SetFieldValueToBytes(sLenFieldName, sLenValue, oBytes)
    End Sub

    Public Shared Function CreatePassDirectionValue(ByVal latchConfig As Byte) As String
        If latchConfig = &H0 Then
            Return "00"
        Else
            'OPT: そもそも、改札機のlatchConfigが&H3以上ということが、実運用ではあり得ないと思われる。
            Return If(latchConfig < &H3, "01", "02")
        End If
    End Function

End Class
