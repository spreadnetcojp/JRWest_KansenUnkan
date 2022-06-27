' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/06/22  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class RiyoDataUtil

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
        New XlsField(8*1, "X2", 1, " "c, "基本ヘッダー データ種別"), _
        New XlsField(8*1, "D3", 2, "-"c, "基本ヘッダー 駅コード", "Station"), _
        New XlsField(8*7, "X14", 1, " "c, "基本ヘッダー 処理日時"), _
        New XlsField(8*1, "D", 1, " "c, "基本ヘッダー コーナー"), _
        New XlsField(8*1, "D", 1, " "c, "基本ヘッダー 号機"), _
        New XlsField(8*4, "D", 1, " "c, "基本ヘッダー シーケンスNo", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*1, "X2", 1, " "c, "基本ヘッダー バージョン"), _
        New XlsField(8*1, "X2", 1, " "c, "通路方向", "PassDirection"), _
        New XlsField(8*2, "X4", 1, " "c, "判定結果"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 乗車券 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 乗車券 着駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 特急券 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 特急券 着駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 のぞみ区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 のぞみ区間 着駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 グリーン区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 グリーン区間 着駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 FREX区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "発着情報 FREX区間 着駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "入場駅情報 乗車券 入場駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "入場駅情報 特急券 入場駅", "Station"), _
        New XlsField(8*4, "X8", 1, " "c, "入場日時情報 乗車券 月日時分"), _
        New XlsField(8*4, "X8", 1, " "c, "入場日時情報 特急券 月日時分"), _
        New XlsField(8*1, "D3", 2, "-"c, "当駅迄券情報 乗車券 乗車駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "当駅迄券情報 特急券 乗車駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "当駅から券情報 乗車券 着駅", "Station"), _
        New XlsField(8*1, "X2", 1, " "c, "大小区分 大人小児", "AdultChild"), _
        New XlsField(8*1, "X2", 1, " "c, "性別区分 男性女性", "MaleFemale"), _
        New XlsField(8*1, "X2", 1, " "c, "通用種別 １枚目情報 券種"), _
        New XlsField(8*4, "X8", 1, " "c, "通用種別 １枚目情報 有効開始日"), _
        New XlsField(8*4, "X8", 1, " "c, "通用種別 １枚目情報 有効終了日"), _
        New XlsField(8*1, "X2", 1, " "c, "通用種別 ２枚目情報 券種"), _
        New XlsField(8*4, "X8", 1, " "c, "通用種別 ２枚目情報 有効開始日"), _
        New XlsField(8*4, "X8", 1, " "c, "通用種別 ２枚目情報 有効終了日"), _
        New XlsField(8*1, "X2", 1, " "c, "通用種別 ３枚目情報 券種"), _
        New XlsField(8*4, "X8", 1, " "c, "通用種別 ３枚目情報 有効開始日"), _
        New XlsField(8*4, "X8", 1, " "c, "通用種別 ３枚目情報 有効終了日"), _
        New XlsField(8*1, "D3", 2, "-"c, "指定券情報 指定１ 指定区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "指定券情報 指定１ 指定区間 着駅", "Station"), _
        New XlsField(8*3, "X5", 1, " "c, "指定券情報 指定１ 列車番号"), _
        New XlsField(1*1, "D", 1, " "c, "指定券情報 指定１ 号車 Gビット"), _
        New XlsField(1*1, "D", 1, " "c, "指定券情報 指定１ 号車 増結ビット"), _
        New XlsField(1*6, "D", 1, " "c, "指定券情報 指定１ 号車番号"), _
        New XlsField(8*1, "X", 1, " "c, "指定券情報 指定１ 座席番号"), _
        New XlsField(8*1, "X2", 1, " "c, "指定券情報 指定１ 座席種別", "SeatKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "指定券情報 指定２ 指定区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "指定券情報 指定２ 指定区間 着駅", "Station"), _
        New XlsField(8*3, "X5", 1, " "c, "指定券情報 指定２ 列車番号"), _
        New XlsField(1*1, "D", 1, " "c, "指定券情報 指定２ 号車 Gビット"), _
        New XlsField(1*1, "D", 1, " "c, "指定券情報 指定２ 号車 増結ビット"), _
        New XlsField(1*6, "D", 1, " "c, "指定券情報 指定２ 号車番号"), _
        New XlsField(8*1, "X", 1, " "c, "指定券情報 指定２ 座席番号"), _
        New XlsField(8*1, "X2", 1, " "c, "指定券情報 指定２ 座席種別", "SeatKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "指定券情報 指定３ 指定区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "指定券情報 指定３ 指定区間 着駅", "Station"), _
        New XlsField(8*3, "X5", 1, " "c, "指定券情報 指定３ 列車番号"), _
        New XlsField(1*1, "D", 1, " "c, "指定券情報 指定３ 号車 Gビット"), _
        New XlsField(1*1, "D", 1, " "c, "指定券情報 指定３ 号車 増結ビット"), _
        New XlsField(1*6, "D", 1, " "c, "指定券情報 指定３ 号車番号"), _
        New XlsField(8*1, "X", 1, " "c, "指定券情報 指定３ 座席番号"), _
        New XlsField(8*1, "X2", 1, " "c, "指定券情報 指定３ 座席種別", "SeatKind"), _
        New XlsField(8*1, "X2", 2, " "c, "不正判定対象区分ビット"), _
        New XlsField(8*1, "D", 1, " "c, "投入枚数"), _
        New XlsField(8*1, "X2", 1, " "c, "併用パターン種別"), _
        New XlsField(8*1, "D3", 2, "-"c, "券読取情報 １枚目情報 区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "券読取情報 １枚目情報 区間 着駅", "Station"), _
        New XlsField(8*1, "D", 1, " "c, "券読取情報 １枚目情報 集計券種", "TicketKind"), _
        New XlsField(8*1, "D3", 1, " "c, "券読取情報 １枚目情報 割引", "DiscountKind"), _
        New XlsField(8*3, "X6", 1, " "c, "券読取情報 １枚目情報 商品番号"), _
        New XlsField(8*1, "X2", 2, " "c, "券読取情報 １枚目情報 発行会社"), _
        New XlsField(8*4, "X8", 1, " "c, "券読取情報 １枚目情報 有効開始日"), _
        New XlsField(8*2, "X4", 1, " "c, "券読取情報 １枚目情報 発行月日"), _
        New XlsField(8*1, "D3", 2, "-"c, "券読取情報 ２枚目情報 区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "券読取情報 ２枚目情報 区間 着駅", "Station"), _
        New XlsField(8*1, "D", 1, " "c, "券読取情報 ２枚目情報 集計券種", "TicketKind"), _
        New XlsField(8*1, "D3", 1, " "c, "券読取情報 ２枚目情報 割引", "DiscountKind"), _
        New XlsField(8*3, "X6", 1, " "c, "券読取情報 ２枚目情報 商品番号"), _
        New XlsField(8*1, "X2", 2, " "c, "券読取情報 ２枚目情報 発行会社"), _
        New XlsField(8*4, "X8", 1, " "c, "券読取情報 ２枚目情報 有効開始日"), _
        New XlsField(8*2, "X4", 1, " "c, "券読取情報 ２枚目情報 発行月日"), _
        New XlsField(8*1, "D3", 2, "-"c, "券読取情報 ３枚目情報 区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "券読取情報 ３枚目情報 区間 着駅", "Station"), _
        New XlsField(8*1, "D", 1, " "c, "券読取情報 ３枚目情報 集計券種", "TicketKind"), _
        New XlsField(8*1, "D3", 1, " "c, "券読取情報 ３枚目情報 割引", "DiscountKind"), _
        New XlsField(8*3, "X6", 1, " "c, "券読取情報 ３枚目情報 商品番号"), _
        New XlsField(8*1, "X2", 2, " "c, "券読取情報 ３枚目情報 発行会社"), _
        New XlsField(8*4, "X8", 1, " "c, "券読取情報 ３枚目情報 有効開始日"), _
        New XlsField(8*2, "X4", 1, " "c, "券読取情報 ３枚目情報 発行月日"), _
        New XlsField(8*1, "D3", 2, "-"c, "券読取情報 ４枚目情報 区間 発駅", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "券読取情報 ４枚目情報 区間 着駅", "Station"), _
        New XlsField(8*1, "D", 1, " "c, "券読取情報 ４枚目情報 集計券種", "TicketKind"), _
        New XlsField(8*1, "D3", 1, " "c, "券読取情報 ４枚目情報 割引", "DiscountKind"), _
        New XlsField(8*3, "X6", 1, " "c, "券読取情報 ４枚目情報 商品番号"), _
        New XlsField(8*1, "X2", 2, " "c, "券読取情報 ４枚目情報 発行会社"), _
        New XlsField(8*4, "X8", 1, " "c, "券読取情報 ４枚目情報 有効開始日"), _
        New XlsField(8*2, "X4", 1, " "c, "券読取情報 ４枚目情報 発行月日"), _
        New XlsField(8*1, "X2", 1, " "c, "券読取情報 １枚目情報 当駅有効", "TicketValidity"), _
        New XlsField(8*1, "X2", 1, " "c, "券読取情報 ２枚目情報 当駅有効", "TicketValidity"), _
        New XlsField(8*1, "X2", 1, " "c, "券読取情報 ３枚目情報 当駅有効", "TicketValidity"), _
        New XlsField(8*1, "X2", 1, " "c, "券読取情報 ４枚目情報 当駅有効", "TicketValidity"), _
        New XlsField(8*4, "X8", 1, " "c, "ＳＦ引去り金額"), _
        New XlsField(8*1, "D3", 2, "-"c, "ＳＦ利用区間１ 利用駅１", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "ＳＦ利用区間１ 利用駅２", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "ＳＦ利用区間２ 利用駅１", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "ＳＦ利用区間２ 利用駅２", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "乗車始点駅", "Station"), _
        New XlsField(8*1, "X2", 1, " "c, "券通しマスタ適用有無", "AbsencePresence"), _
        New XlsField(8*1, "X2", 46, " "c, "予備１"), _
        New XlsField(8*2, "X4", 1, " "c, "サム値", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*1, "X2", 52, " "c, "未使用"), _
        New XlsField(8*1, "X2", 288, " "c, "券エンコード情報 １枚目情報"), _
        New XlsField(8*1, "X2", 288, " "c, "券エンコード情報 ２枚目情報"), _
        New XlsField(8*1, "X2", 288, " "c, "券エンコード情報 ３枚目情報"), _
        New XlsField(8*1, "X2", 288, " "c, "券エンコード情報 ４枚目情報")}

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

    Public Shared Function CreatePassDirectionValue(ByVal latchConfig As Byte, ByVal direction As Integer) As String
        If direction = 0 Then
            Select Case latchConfig
                Case &H1
                    Return "01"
                Case &H2
                    Return "03"
                Case Else
                    Return "00"
            End Select
        ElseIf direction = 1 Then
            Select Case latchConfig
                Case &H1
                    Return "02"
                Case &H2
                    Return "04"
                Case Else
                    Return "00"
            End Select
        Else
            Return "00"
        End If
    End Function

End Class
