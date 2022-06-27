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

Public Class KadoDataUtil

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

    Private Const AggregateFieldsOrigin As Integer = 15
    Private Shared ReadOnly oFields As XlsField() = New XlsField() { _
        New XlsField(8*1, "X2", 1, " "c, "基本ヘッダー データ種別", "DataKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "基本ヘッダー 駅コード", "Station"), _
        New XlsField(8*7, "X14", 1, " "c, "基本ヘッダー 処理日時"), _
        New XlsField(8*1, "D", 1, " "c, "基本ヘッダー コーナー"), _
        New XlsField(8*1, "D", 1, " "c, "基本ヘッダー 号機"), _
        New XlsField(8*4, "D", 1, " "c, "基本ヘッダー シーケンスNo", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*1, "X2", 1, " "c, "基本ヘッダー バージョン"), _
        New XlsField(8*7, "X14", 1, " "c, "共通部 集計開始日時"), _
        New XlsField(8*7, "X14", 1, " "c, "共通部 集計終了(収集)日時"), _
        New XlsField(8*7, "X14", 1, " "c, "共通部 改札側搬送部点検日時"), _
        New XlsField(8*7, "X14", 1, " "c, "共通部 集札側搬送部点検日時"), _
        New XlsField(8*8, "X10", 1, " "c, "共通部 改札側搬送部番号"), _
        New XlsField(8*8, "X10", 1, " "c, "共通部 集札側搬送部番号"), _
        New XlsField(8*1, "D", 48, " "c, "共通部 改札側検知センサレベル"), _
        New XlsField(8*1, "D", 48, " "c, "共通部 集札側検知センサレベル"), _
        New XlsField(8*1, "X2", 48, " "c, "共通部 予備"), _
        New XlsField(8*4, "D", 1, " "c, "集計001 窓処(APL)/総EXIC処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計002 窓処(APL)/総在来IC処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計003 窓処(APL)/総磁気券処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計004 窓処(APL)/EXIC業務EXIC処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計005 窓処(APL)/EXIC情報照会業務EXIC処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計006 窓処(APL)/在来IC業務在来IC処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計007 窓処(APL)/磁気業務磁気券処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計008 窓処(APL)/補助 在来IC/一体IC業務処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計009 窓処(APL)/補助 EXIC業務処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計010 窓処(APL)/係員認証 係員認証処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計011 窓処(APL)/業務認証 業務認証処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計012 窓処(APL)/障害 遅払設定処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計013 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計014 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計015 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計016 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計017 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計018 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計019 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計020 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計021 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計022 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計023 窓処(APL)/EXIC 新幹線入場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計024 窓処(APL)/EXIC 新幹線出場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計025 窓処(APL)/EXIC 強制出場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計026 窓処(APL)/EXIC 入場取消処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計027 窓処(APL)/EXIC 遅払出場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計028 窓処(APL)/EXIC 使用停止処理(自動)件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計029 窓処(APL)/EXIC 新幹線入場判定処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計030 窓処(APL)/EXIC 新幹線出場判定処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計031 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計032 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計033 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計034 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計035 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計036 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計037 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計038 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計039 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計040 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計041 窓処(APL)/EXIC情報照会 お預かり情報・利用履歴照会件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計042 窓処(APL)/EXIC情報照会 取消/払い戻し処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計043 窓処(APL)/EXIC情報照会 窓処発券処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計044 窓処(APL)/EXIC情報照会 予約変更処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計045 窓処(APL)/EXIC情報照会 案内表再出力処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計046 窓処(APL)/EXIC情報照会 使用停止処理(自動)件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計047 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計048 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計049 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計050 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計051 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計052 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計053 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計054 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計055 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計056 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計057 窓処(APL)/在来IC 入場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計058 窓処(APL)/在来IC 出場･精算処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計059 窓処(APL)/在来IC 減額処理処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計060 窓処(APL)/在来IC 他駅出場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計061 窓処(APL)/在来IC 強制出場/発駅ｷｬﾝｾﾙ処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計062 窓処(APL)/在来IC 利用履歴印字処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計063 窓処(APL)/在来IC 設定変更処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計064 窓処(APL)/在来IC 還元ｻｰﾋﾞｽ印字処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計065 窓処(APL)/在来IC 新幹線強制出場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計066 窓処(APL)/在来IC 新幹線入場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計067 窓処(APL)/在来IC 新幹線他駅出場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計068 窓処(APL)/在来IC 使用停止処理(自動)件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計069 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計070 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計071 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計072 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計073 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計074 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計075 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計076 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計077 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計078 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計079 窓処(APL)/磁気券 幹線入場処理（直接改札）件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計080 窓処(APL)/磁気券 幹線入場処理（乗換改札）件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計081 窓処(APL)/磁気券 在来線入場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計082 窓処(APL)/磁気券 幹線出場処理（直接改札）件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計083 窓処(APL)/磁気券 幹線出場処理（乗換改札）件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計084 窓処(APL)/磁気券 在来線出場処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計085 窓処(APL)/磁気券 遅払出場処理（直接改札）件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計086 窓処(APL)/磁気券 遅払出場処理（乗換改札）件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計087 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計088 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計089 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計090 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計091 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計092 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計093 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計094 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計095 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計096 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計097 窓処(APL)/補助 在来IC/一体型使用停止処理(手動)件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計098 窓処(APL)/補助 在来IC/一体型使用停止処理(自動)件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計099 窓処(APL)/補助 EXIC使用停止処理(手動)件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計100 窓処(APL)/補助 EXIC利用停止回復処理件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計101 窓処(APL)/補助 EXIC使用停止処理(自動)件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計102 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計103 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計104 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計105 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計106 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計107 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計108 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計109 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計110 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計111 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計112 窓処(APL)/総未了件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計113 窓処(APL)/EXIC業務 書込み未了件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計114 窓処(APL)/EXIC情報照会業務 書込み未了件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計115 窓処(APL)/在来IC業務 書込み未了件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計116 窓処(APL)/補助業務 在来IC書込み未了件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計117 窓処(APL)/補助業務 EXIC書込み未了件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計118 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計119 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計120 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計121 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計122 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計123 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計124 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計125 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計126 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計127 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計128 窓処(APL)/係員認証 総判定NG件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計129 窓処(APL)/係員認証 読取判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計130 窓処(APL)/係員認証 処理枚数判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計131 窓処(APL)/係員認証 IDi判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計132 窓処(APL)/係員認証 バージョン判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計133 窓処(APL)/係員認証 IC種別判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計134 窓処(APL)/係員認証 データ項目判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計135 窓処(APL)/係員認証 マスタデータ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計136 窓処(APL)/係員認証 活性化判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計137 窓処(APL)/係員認証 カード使用不可判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計138 窓処(APL)/係員認証 パスワードロック判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計139 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計140 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計141 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計142 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計143 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計144 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計145 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計146 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計147 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計148 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計149 窓処(APL)/EXIC 総判定NG件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計150 窓処(APL)/EXIC 処理枚数・組合せ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計151 窓処(APL)/EXIC 読取判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計152 窓処(APL)/EXIC IDi判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計153 窓処(APL)/EXIC ﾊﾞｰｼﾞｮﾝ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計154 窓処(APL)/EXIC ﾃﾞｰﾀ項目判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計155 窓処(APL)/EXIC ｶｰﾄﾞ使用不可判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計156 窓処(APL)/EXIC 活性化判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計157 窓処(APL)/EXIC EXICネガチェック判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計158 窓処(APL)/EXIC 一体型ICでの在来入場チェック判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計159 窓処(APL)/EXIC 入出場ｼｰｹﾝｽ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計160 窓処(APL)/EXIC 予約検索判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計161 窓処(APL)/EXIC 予約情報判定NG(予約変更中)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計162 窓処(APL)/EXIC 予約情報判定NG(削除)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計163 窓処(APL)/EXIC 予約情報判定NG(入場済み)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計164 窓処(APL)/EXIC 予約情報判定NG(発券済み)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計165 窓処(APL)/EXIC 予約情報判定(IDmｾｷｭﾘﾃｨ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計166 窓処(APL)/EXIC 予約情報判定(その他)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計167 窓処(APL)/EXIC 予約情報経路判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計168 窓処(APL)/EXIC 終列車判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計169 窓処(APL)/EXIC 利用日付判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計170 窓処(APL)/EXIC 自駅下車判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計171 窓処(APL)/EXIC 区間判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計172 窓処(APL)/EXIC 折り返し判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計173 窓処(APL)/EXIC 運休・全車自由・遅払い処理判定", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計174 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計175 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計176 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計177 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計178 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計179 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計180 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計181 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計182 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計183 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計184 窓処(APL)/EXIC情報照会 総判定NG件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計185 窓処(APL)/EXIC情報照会 処理枚数・組合せ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計186 窓処(APL)/EXIC情報照会 読取判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計187 窓処(APL)/EXIC情報照会 IDi判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計188 窓処(APL)/EXIC情報照会 ﾊﾞｰｼﾞｮﾝ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計189 窓処(APL)/EXIC情報照会 ﾃﾞｰﾀ項目判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計190 窓処(APL)/EXIC情報照会 ｶｰﾄﾞ使用不可判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計191 窓処(APL)/EXIC情報照会 活性化判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計192 窓処(APL)/EXIC情報照会 EXICネガチェック判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計193 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計194 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計195 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計196 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計197 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計198 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計199 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計200 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計201 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計202 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計203 窓処(APL)/在来IC 総判定NG件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計204 窓処(APL)/在来IC 処理枚数判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計205 窓処(APL)/在来IC 読取判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計206 窓処(APL)/在来IC IDi判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計207 窓処(APL)/在来IC ﾊﾞｰｼﾞｮﾝ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計208 窓処(APL)/在来IC ICﾃｽﾄｶｰﾄﾞ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計209 窓処(APL)/在来IC IC種別判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計210 窓処(APL)/在来IC ﾃﾞｰﾀ項目判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計211 窓処(APL)/在来IC 自社取扱媒体判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計212 窓処(APL)/在来IC 表示対象ｶｰﾄﾞ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計213 窓処(APL)/在来IC ICｶｰﾄﾞ正当性判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計214 窓処(APL)/在来IC 活性化判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計215 窓処(APL)/在来IC 活性化判定NG（前回操作が未完了）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計216 窓処(APL)/在来IC 10年失効判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計217 窓処(APL)/在来IC ｶｰﾄﾞ使用不可判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計218 窓処(APL)/在来IC ｶｰﾄﾞ有効期限判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計219 窓処(APL)/在来IC ネガチェック判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計220 窓処(APL)/在来IC ﾏｽﾀﾃﾞｰﾀ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計221 窓処(APL)/在来IC ｱｸｾｽ鍵異常ｴﾗｰ", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計222 窓処(APL)/在来IC 暦日異常", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計223 窓処(APL)/在来IC その他判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計224 窓処(APL)/在来IC 定期期間判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計225 窓処(APL)/在来IC 入出場ｼｰｹﾝｽ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計226 窓処(APL)/在来IC 利用日付判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計227 窓処(APL)/在来IC 自駅下車判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計228 窓処(APL)/在来IC 自駅下車判定NG(ｺｰﾅ一致)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計229 窓処(APL)/在来IC 区間判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計230 窓処(APL)/在来IC 定期券地域ｺｰﾄﾞ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計231 窓処(APL)/在来IC IC定期乗車駅ｺｰﾄﾞ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計232 窓処(APL)/在来IC SF地域ｺｰﾄﾞ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計233 窓処(APL)/在来IC 会社間経路連続性判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計234 窓処(APL)/在来IC 乗車駅ｴﾘｱ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計235 窓処(APL)/在来IC 通過ｻｰﾋﾞｽ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計236 窓処(APL)/在来IC 期間用確認NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計237 窓処(APL)/在来IC ﾎﾟｲﾝﾄ期限NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計238 窓処(APL)/在来IC 一点通過判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計239 窓処(APL)/在来IC 不正判定(入出場時間判定)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計240 窓処(APL)/在来IC 不正判定(同一駅入出場判定)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計241 窓処(APL)/在来IC 不正判定(入出場ｻｲｸﾙ判定)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計242 窓処(APL)/在来IC 不正判定(再投入判定)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計243 窓処(APL)/在来IC 不正判定(経路外判定)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計244 窓処(APL)/在来IC 残額判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計245 窓処(APL)/在来IC 1ﾗｯﾁ誤ﾀｯﾁ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計246 窓処(APL)/在来IC ﾌｪｰﾙｾｰﾌ判定1NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計247 窓処(APL)/在来IC ﾌｪｰﾙｾｰﾌ判定2NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計248 窓処(APL)/在来IC ﾌｪｰﾙｾｰﾌ判定3NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計249 窓処(APL)/在来IC ﾌｪｰﾙｾｰﾌ判定4NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計250 窓処(APL)/在来IC 特定都区市内ｴﾘｱ入場NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計251 窓処(APL)/在来IC 券止め判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計252 窓処(APL)/在来IC 入出場時間超過判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計253 窓処(APL)/在来IC 在来IC処理その他運用判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計254 窓処(APL)/在来IC 新幹線IC処理入出場ｼｰｹﾝｽ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計255 窓処(APL)/在来IC 新幹線IC処理利用日付判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計256 窓処(APL)/在来IC 新幹線IC処理自駅下車NG(ｺｰﾅ-一致)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計257 窓処(APL)/在来IC 新幹線IC処理残額判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計258 窓処(APL)/在来IC 新幹線IC処理乗車駅ｺｰﾄﾞNG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計259 窓処(APL)/在来IC 新幹線IC処理入場駅区間外NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計260 窓処(APL)/在来IC 新幹線IC処理券種判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計261 窓処(APL)/在来IC 新幹線IC処理データ項目判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計262 窓処(APL)/在来IC 新幹線IC処理その他運用判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計263 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計264 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計265 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計266 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計267 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計268 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計269 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計270 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計271 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計272 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計273 窓処(APL)/磁気券 総判定NG件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計274 窓処(APL)/磁気券 異常券判定NG(非磁気化券)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計275 窓処(APL)/磁気券 異常券判定NG(ﾌｫｰﾏｯﾄｴﾗｰ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計276 窓処(APL)/磁気券 異常券判定NG(ﾊﾟﾘﾃｨｴﾗｰ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計277 窓処(APL)/磁気券 異常券判定NG(ｻﾑﾁｪｯｸｴﾗｰ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計278 窓処(APL)/磁気券 異常券判定NG(二重化ｴﾗｰ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計279 窓処(APL)/磁気券 異常券判定NG(,W1・W2ﾊﾟﾘﾃｨｴﾗｰ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計280 窓処(APL)/磁気券 異常券判定NG(,無賃乗車証)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計281 窓処(APL)/磁気券 異常券判定NG(ﾌｪｰﾙｾｰﾌ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計282 窓処(APL)/磁気券 利用期間制限判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計283 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計284 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計285 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計286 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計287 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計288 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計289 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計290 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計291 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計292 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計293 窓処(APL)/補助 総判定NG件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計294 窓処(APL)/補助 在来IC/一体型IC使用停止 処理枚数判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計295 窓処(APL)/補助 在来IC/一体型IC使用停止 読取判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計296 窓処(APL)/補助 在来IC/一体型IC使用停止 IDi判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計297 窓処(APL)/補助 在来IC/一体型IC使用停止 ﾊﾞｰｼﾞｮﾝ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計298 窓処(APL)/補助 在来IC/一体型IC使用停止 表示対象ｶｰﾄﾞ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計299 窓処(APL)/補助 在来IC/一体型IC使用停止 ICﾃｽﾄｶｰﾄﾞ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計300 窓処(APL)/補助 在来IC/一体型IC使用停止 IC種別判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計301 窓処(APL)/補助 在来IC/一体型IC使用停止 ﾃﾞｰﾀ項目判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計302 窓処(APL)/補助 在来IC/一体型IC使用停止 自社取扱媒体判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計303 窓処(APL)/補助 在来IC/一体型IC使用停止 ICｶｰﾄﾞ正当性判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計304 窓処(APL)/補助 在来IC/一体型IC使用停止 活性化判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計305 窓処(APL)/補助 在来IC/一体型IC使用停止 活性化判定NG（前回操作が未完了）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計306 窓処(APL)/補助 在来IC/一体型IC使用停止 ｶｰﾄﾞ使用不可判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計307 窓処(APL)/補助 在来IC/一体型IC使用停止 ネガチェック判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計308 窓処(APL)/補助 在来IC/一体型IC使用停止 ﾏｽﾀﾃﾞｰﾀ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計309 窓処(APL)/補助 在来IC/一体型IC使用停止 ｱｸｾｽ鍵異常ｴﾗｰ", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計310 窓処(APL)/補助 在来IC/一体型IC使用停止 その他判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計311 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計312 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計313 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計314 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計315 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計316 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計317 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計318 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計319 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計320 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計321 窓処(APL)/補助 EXIC使用停止 対象外媒体", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計322 窓処(APL)/補助 EXIC使用停止 処理枚数・組合せ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計323 窓処(APL)/補助 EXIC使用停止 読取判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計324 窓処(APL)/補助 EXIC使用停止 IDi判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計325 窓処(APL)/補助 EXIC使用停止 ﾊﾞｰｼﾞｮﾝ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計326 窓処(APL)/補助 EXIC使用停止 ﾃﾞｰﾀ項目判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計327 窓処(APL)/補助 EXIC使用停止 ｶｰﾄﾞ使用不可判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計328 窓処(APL)/補助 EXIC使用停止 活性化判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計329 窓処(APL)/補助 EXIC使用停止 EXICネガチェック判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計330 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計331 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計332 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計333 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計334 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計335 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計336 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計337 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計338 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計339 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計340 窓処(APL)/補助 EXIC利用停止回復 処理枚数・組合せ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計341 窓処(APL)/補助 EXIC利用停止回復 読取判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計342 窓処(APL)/補助 EXIC利用停止回復 IDi判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計343 窓処(APL)/補助 EXIC利用停止回復 ﾊﾞｰｼﾞｮﾝ判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計344 窓処(APL)/補助 EXIC利用停止回復 ﾃﾞｰﾀ項目判定NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計345 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計346 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計347 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計348 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計349 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計350 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計351 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計352 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計353 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計354 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計355 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計356 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計357 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計358 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計359 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計360 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計361 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計362 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計363 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計364 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計365 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計366 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計367 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計368 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計369 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計370 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計371 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計372 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計373 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計374 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計375 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計376 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計377 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計378 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計379 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計380 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計381 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計382 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計383 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計384 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計385 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計386 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計387 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計388 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計389 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計390 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計391 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計392 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計393 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計394 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計395 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計396 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計397 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計398 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計399 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計400 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計401 窓処(FW)/磁気メカＦＷカウント 挿入枚数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計402 窓処(FW)/磁気メカＦＷカウント 磁気ヘッド通過件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計403 窓処(FW)/磁気メカＦＷカウント リードリトライ件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計404 窓処(FW)/磁気メカＦＷカウント ライトリトライ件数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計405 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計406 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計407 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計408 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計409 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計410 窓処(FW)/磁気メカＦＷカウント 挿入口部搬送モータPM01動作回数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計411 窓処(FW)/磁気メカＦＷカウント 挿入口シャッタSOL1動作回数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計412 窓処(FW)/磁気メカＦＷカウント エド券整列ガイドSOL02動作回数", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計413 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計414 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計415 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計416 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計417 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計418 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計419 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計420 窓処(FW)予備", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計421 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計422 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計423 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計424 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計425 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計426 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計427 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計428 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計429 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計430 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計431 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計432 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計433 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計434 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計435 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計436 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計437 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計438 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計439 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計440 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計441 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計442 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計443 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計444 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計445 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計446 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計447 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計448 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計449 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計450 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計451 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計452 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計453 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計454 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計455 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計456 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計457 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計458 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計459 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計460 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計461 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計462 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計463 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計464 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計465 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計466 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計467 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計468 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計469 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計470 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計471 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計472 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計473 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計474 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計475 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計476 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計477 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計478 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計479 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計480 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計481 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計482 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計483 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計484 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計485 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計486 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計487 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計488 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計489 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計490 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計491 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計492 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計493 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計494 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計495 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計496 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計497 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計498 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計499 （空き）", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "集計500 （空き）", Nothing, XlsByteOrder.LittleEndian)}

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

    Public Shared Sub InitBaseHeaderFields(ByVal machine As EkCode, ByVal d As DateTime, ByVal seqNum As UInteger, ByVal oBytes As Byte())
        SetFieldValueToBytes("基本ヘッダー データ種別", "A7", oBytes)
        SetFieldValueToBytes("基本ヘッダー 駅コード", machine.ToString("%3R-%3S"), oBytes)
        SetFieldValueToBytes("基本ヘッダー 処理日時", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes("基本ヘッダー コーナー", machine.ToString("%C"), oBytes)
        SetFieldValueToBytes("基本ヘッダー 号機", machine.ToString("%U"), oBytes)
        SetFieldValueToBytes("基本ヘッダー シーケンスNo", seqNum.ToString(), oBytes)
        SetFieldValueToBytes("基本ヘッダー バージョン", "01", oBytes)
    End Sub

    Public Shared Sub InitCommonPartFields(ByVal machine As EkCode, ByVal d As DateTime, ByVal oBytes As Byte())
        SetFieldValueToBytes("共通部 集計開始日時", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes("共通部 集計終了(収集)日時", "00000000000000", oBytes)
        SetFieldValueToBytes("共通部 改札側搬送部点検日時", "00000000000000", oBytes)
        SetFieldValueToBytes("共通部 集札側搬送部点検日時", "00000000000000", oBytes)
        SetFieldValueToBytes("共通部 改札側搬送部番号", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes("共通部 集札側搬送部番号", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes("共通部 改札側検知センサレベル", Field("共通部 改札側検知センサレベル").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes("共通部 集札側検知センサレベル", Field("共通部 集札側検知センサレベル").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes("共通部 予備", Field("共通部 予備").CreateDefaultValue(), oBytes)
    End Sub

    Public Shared Sub UpdateSummaryFields(ByVal oBytes As Byte())
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 293).MetaName, GetSummary(294, 344, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 273).MetaName, GetSummary(274, 282, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 203).MetaName, GetSummary(204, 262, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 184).MetaName, GetSummary(185, 192, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 149).MetaName, GetSummary(150, 173, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 128).MetaName, GetSummary(129, 138, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 112).MetaName, GetSummary(113, 117, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 9).MetaName, GetSummary(99, 101, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 8).MetaName, GetSummary(97, 98, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 7).MetaName, GetSummary(79, 86, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 6).MetaName, GetSummary(57, 68, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 5).MetaName, GetSummary(41, 46, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 4).MetaName, GetSummary(23, 30, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 3).MetaName, GetFieldValueFromBytes(Fields(AggregateFieldsOrigin + 7).MetaName, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 2).MetaName, GetSummary(New Integer() {6, 8}, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 1).MetaName, GetSummary(New Integer() {4, 5, 9}, oBytes), oBytes)
    End Sub

    Private Shared Function GetSummary(ByVal firstAggregateNumber As Integer, ByVal lastAggregateNumber As Integer, ByVal oBytes As Byte()) As String
        Dim sum As Long = 0
        For i As Integer = AggregateFieldsOrigin + firstAggregateNumber To AggregateFieldsOrigin + lastAggregateNumber
            sum += Long.Parse(GetFieldValueFromBytes(oFields(i).MetaName, oBytes))
        Next i
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

    Private Shared Function GetSummary(ByVal aggregateNumbers As Integer(), ByVal oBytes As Byte()) As String
        Dim sum As Long = 0
        For Each n As Integer In aggregateNumbers
            Dim i As Integer = AggregateFieldsOrigin + n
            sum += Long.Parse(GetFieldValueFromBytes(oFields(i).MetaName, oBytes))
        Next n
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

End Class
