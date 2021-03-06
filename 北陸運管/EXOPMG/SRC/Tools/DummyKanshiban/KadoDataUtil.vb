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

    Private Shared oFieldRefs(1) As Dictionary(Of String, FieldRef)
    Private Shared totalBitCount(1) As Integer

    Private Const AggregateFieldsOrigin As Integer = 15
    Private Shared ReadOnly oFields As XlsField()() = { _
        New XlsField() { _
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
            New XlsField(8*8, "X16", 1, " "c, "共通部 改札側搬送部番号"), _
            New XlsField(8*8, "X16", 1, " "c, "共通部 集札側搬送部番号"), _
            New XlsField(8*1, "D", 48, " "c, "共通部 改札側検知センサレベル"), _
            New XlsField(8*1, "D", 48, " "c, "共通部 集札側検知センサレベル"), _
            New XlsField(8*1, "X2", 48, " "c, "共通部 予備"), _
            New XlsField(8*4, "D", 1, " "c, "集計001 改(Ａ)総投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計002 改(Ａ)総投入枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計003 改(Ａ)１枚投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計004 改(Ａ)２枚投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計005 改(Ａ)３枚投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計006 改(Ａ)４枚投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計007 改(Ａ)５枚以上投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計008 改(Ａ)一括投入件数（２枚）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計009 改(Ａ)一括投入件数（３枚）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計010 改(Ａ)一括投入件数（４枚）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計011 改(Ａ)一括投入件数（５枚以上）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計012 改(Ａ)全枚数表投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計013 改(Ａ)全枚数裏投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計014 改(Ａ)裏表混合投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計015 改(Ａ)表投入枚数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計016 改(Ａ)表投入枚数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計017 改(Ａ)表投入枚数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計018 改(Ａ)表投入枚数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計019 改(Ａ)裏投入枚数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計020 改(Ａ)裏投入枚数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計021 改(Ａ)裏投入枚数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計022 改(Ａ)裏投入枚数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計023 改(Ａ)券判定ＯＫ件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計024 改(Ａ)券判定ＯＫ枚数（合計）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計025 改(Ａ)券判定ＯＫ枚数（１枚投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計026 改(Ａ)券判定ＯＫ枚数（２枚投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計027 改(Ａ)券判定ＯＫ枚数（３枚投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計028 改(Ａ)券判定ＯＫ枚数（４枚投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計029 改(Ａ)券判定ＯＫ枚数（NRZｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計030 改(Ａ)券判定ＯＫ枚数（FMｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計031 改(Ａ)券判定ＯＫ枚数（NRZ定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計032 改(Ａ)券判定ＯＫ枚数（FM定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計033 改(Ａ)券判定ＯＫ枚数（FM大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計034 改(Ａ)券判定ＯＫ枚数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計035 改(Ａ)判定対象外券投入枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計036 改(Ａ)追加投入待ち件数（乗車券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計037 改(Ａ)追加投入待ち件数（特急券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計038 改(Ａ)追加投入待ち件数（当駅迄券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計039 改(Ａ)追加投入待ち件数（乗車券+当駅迄券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計040 改(Ａ)追加投入待ち件数（特急券+当駅迄券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計041 改(Ａ)追加投入待ち件数（乗車券+特急券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計042 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計043 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計044 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計045 改(Ａ)その他ＩＣ処理受付枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計046 改(Ａ)ＩＣ１枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計047 改(Ａ)追加投入待ち件数（在来ＩＣ）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計048 改(Ａ)追加投入待ち件数（新幹線専用券当駅迄券なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計049 改(Ａ)ご利用票発券枚数（累計）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計050 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計051 改(Ａ)券判定ＮＧ件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計052 改(Ａ)異常券判定ＮＧ（表投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計053 改(Ａ)異常券判定ＮＧ（裏投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計054 改(Ａ)異常券判定ＮＧ（ﾊﾟﾘﾃｨｴﾗｰ：ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計055 改(Ａ)異常券判定ＮＧ（ﾊﾟﾘﾃｨｴﾗｰ：定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計056 改(Ａ)異常券判定ＮＧ（ﾌｫｰﾏｯﾄｴﾗｰ：ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計057 改(Ａ)異常券判定ＮＧ（ﾌｫｰﾏｯﾄｴﾗｰ：定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計058 改(Ａ)異常券判定ＮＧ（ﾌｫｰﾏｯﾄｴﾗｰ：大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計059 改(Ａ)異常券判定ＮＧ（ﾌｫｰﾏｯﾄｴﾗｰ：その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計060 改(Ａ)異常券判定ＮＧ（二重化ｴﾗｰ）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計061 改(Ａ)異常券判定ＮＧ（ｻﾑﾁｪｯｸｴﾗｰ：ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計062 改(Ａ)異常券判定ＮＧ（ｻﾑﾁｪｯｸｴﾗｰ：定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計063 改(Ａ)異常券判定ＮＧ（ｻﾑﾁｪｯｸｴﾗｰ：大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計064 改(Ａ)異常券判定ＮＧ（ｻﾑﾁｪｯｸｴﾗｰ：その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計065 改(Ａ)異常券判定ＮＧ（非磁気化券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計066 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計067 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計068 改(Ａ)無効券判定ＮＧ（券種判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計069 改(Ａ)無効券判定ＮＧ（大人券小児券混在判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計070 改(Ａ)無効券判定ＮＧ（期間判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計071 改(Ａ)無効券判定ＮＧ（区間判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計072 改(Ａ)無効券判定ＮＧ（入場券時間判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計073 改(Ａ)無効券判定ＮＧ（終列車判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計074 改(Ａ)無効券判定ＮＧ（使用済判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計075 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計076 改(Ａ)無効券判定ＮＧ（複乗判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計077 改(Ａ)無効券判定ＮＧ（複数枚有効判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計078 改(Ａ)無効券判定ＮＧ（使用開始後判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計079 改(Ａ)無効券判定ＮＧ（投入枚数判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計080 改(Ａ)有効組合せ判定ＮＧ（乗車券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計081 改(Ａ)有効組合せ判定ＮＧ（特急券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計082 改(Ａ)有効組合せ判定ＮＧ（当駅迄券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計083 改(Ａ)有効組合せ判定ＮＧ（乗車券・当駅迄券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計084 改(Ａ)有効組合せ判定ＮＧ（特急券・当駅迄券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計085 改(Ａ)有効組合せ判定ＮＧ（乗車券・特急券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計086 改(Ａ)有効組合せ判定ＮＧ（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計087 改(Ａ)組合せ判定ＮＧ（乗車券･特急券区間比較判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計088 改(Ａ)組合せ異常（新幹線専用券当駅迄券なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計089 改(Ａ)組合せ判定ＮＧ（接続判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計090 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計091 改(Ａ)組合せ判定ＮＧ（併用判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計092 改(Ａ)在来IC＋新幹線磁気３枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計093 改(Ａ)追加投入待ち件数（ＥＸＩＣ、(幹)定期券(IC)）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計094 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計095 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計096 改(Ａ)不正判定ＮＧ（複数回使用異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計097 改(Ａ)ＩＣ２枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計098 改(Ａ)その他ＮＧ（遅払い判定不可）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計099 改(Ａ)ＩＤチェック判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計100 改(Ａ)総ＩＣ磁気併用件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計101 改(Ａ)磁気書込件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計102 改(Ａ)磁気書込件数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計103 改(Ａ)磁気書込件数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計104 改(Ａ)磁気書込件数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計105 改(Ａ)磁気書込件数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計106 改(Ａ)磁気書込ﾘﾄﾗｲ件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計107 改(Ａ)磁気書込ﾘﾄﾗｲ回数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計108 改(Ａ)磁気書込ﾘﾄﾗｲ回数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計109 改(Ａ)磁気書込ﾘﾄﾗｲ回数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計110 改(Ａ)磁気書込ﾘﾄﾗｲ回数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計111 改(Ａ)磁気書込ﾘﾄﾗｲ→ＯＫ回数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計112 改(Ａ)磁気書込ﾘﾄﾗｲ→ＯＫ回数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計113 改(Ａ)磁気書込ﾘﾄﾗｲ→ＯＫ回数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計114 改(Ａ)磁気書込ﾘﾄﾗｲ→ＯＫ回数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計115 改(Ａ)磁気書込ﾘﾄﾗｲ→ＮＧ回数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計116 改(Ａ)磁気書込ﾘﾄﾗｲ→ＮＧ回数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計117 改(Ａ)磁気書込ﾘﾄﾗｲ→ＮＧ回数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計118 改(Ａ)磁気書込ﾘﾄﾗｲ→ＮＧ回数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計119 改(Ａ)パンチ回数（直接印刷部：改札ｴﾄﾞ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計120 改(Ａ)パンチ回数（直接印刷部：改札85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計121 改(Ａ)パンチ回数（直接印刷部：集札ｴﾄﾞ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計122 改(Ａ)パンチ回数（直接印刷部：集札85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計123 改(Ａ)パンチ回数（転写印刷部：85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計124 改(Ａ)印刷回数（直接印刷部：上側ｴﾄﾞ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計125 改(Ａ)印刷回数（直接印刷部：上側85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計126 改(Ａ)印刷回数（直接印刷部：下側ｴﾄﾞ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計127 改(Ａ)印刷回数（直接印刷部：下側85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計128 改(Ａ)印刷回数（転写印刷部：85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計129 改(Ｆ)ＳＮＤ−Ｍ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計130 改(Ｆ)ＳＮＤ−Ｍ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計131 改(Ｆ)ＳＮＤ−Ｍ５動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計132 改(Ｆ)ＳＮＤ−Ｍ６動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計133 改(Ｆ)ＳＮＤ−Ｍ７動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計134 改(Ｆ)ＳＮＤ−Ｐ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計135 改(Ｆ)ＳＮＤ−Ｐ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計136 改(Ｆ)ＳＮＤ−Ｐ５動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計137 改(Ｆ)ＭＴＲ−Ｅ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計138 改(Ｆ)ＭＴＲ−Ｅ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計139 改(Ｆ)ＭＴＲ−Ｈ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計140 改(Ｆ)ＭＴＲ−Ｈ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計141 改(Ｆ)ＭＴＲ−Ｈ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計142 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計143 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計144 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計145 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計146 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計147 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計148 改(Ｆ)分離部取込動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計149 改(Ｆ)分離部繰出し動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計150 改(Ｆ)整列部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計151 改(Ａ)総集札枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計152 改(Ａ)１枚集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計153 改(Ａ)２枚集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計154 改(Ａ)３枚集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計155 改(Ａ)４枚集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計156 改(Ａ)総別集札枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計157 改(Ａ)１枚別集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計158 改(Ａ)２枚別集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計159 改(Ａ)３枚別集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計160 改(Ａ)４枚別集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計161 改(Ａ)保留件数（処理異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計162 改(Ａ)保留件数（不正）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計163 改(Ａ)二重化による救済枚数（B,Gﾄﾗｯｸ）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計164 改(Ａ)二重化による救済枚数（B,Gﾄﾗｯｸ以外）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計165 改(Ａ)整列部動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計166 改(Ａ)券反転回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計167 改(Ａ)ＥＸＩＣ＋磁気１枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計168 改(Ａ)ＥＸＩＣ＋磁気２枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計169 改(Ａ)ＥＸＩＣ＋磁気３枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計170 改(Ａ)在来ＩＣ＋新幹線磁気１枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計171 改(Ａ)運休処理対象券投入枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計172 改(Ａ)全車自由席対象券投入枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計173 改(Ａ)遅払い対象券投入枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計174 改(Ａ)在来ＩＣ＋新幹線磁気２枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計175 改(Ｆ)ＳＮＤ−Ａ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計176 改(Ｆ)ＳＮＤ−Ａ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計177 改(Ｆ)ＳＮＤ−Ａ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計178 改(Ｆ)ＳＮＤ−Ａ５動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計179 改(Ｆ)ＳＮＤ−Ｍ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計180 改(Ｆ)ＳＮＤ−Ｐ６動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計181 改(Ｆ)ＳＮＤ−Ｐ７動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計182 改(Ｆ)ＳＮＤ−Ｐ８動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計183 改(Ｆ)ＳＮＤ−Ｐ９動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計184 改(Ｆ)ＳＮＤ−Ｅ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計185 改(Ｆ)ＳＮＤ−Ｅ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計186 改(Ｆ)ＳＮＤ−Ｅ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計187 改(Ｆ)ＳＮＤ−Ｅ５動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計188 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計189 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計190 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計191 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計192 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計193 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計194 改(Ｆ)ＭＴＲ−Ａ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計195 改(Ｆ)ＭＴＲ−Ａ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計196 改(Ｆ)ＭＴＲ−Ａ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計197 改(Ｆ)ＭＴＲ−Ｍ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計198 改(Ａ)総ＩＣ処理受付枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計199 改(Ａ)ＥＸＩＣ処理受付枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計200 改(Ａ)在来ＩＣ処理受付枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計201 集(Ａ)総投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計202 集(Ａ)総投入枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計203 集(Ａ)１枚投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計204 集(Ａ)２枚投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計205 集(Ａ)３枚投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計206 集(Ａ)４枚投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計207 集(Ａ)５枚以上投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計208 集(Ａ)一括投入件数（２枚）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計209 集(Ａ)一括投入件数（３枚）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計210 集(Ａ)一括投入件数（４枚）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計211 集(Ａ)一括投入件数（５枚以上）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計212 集(Ａ)全枚数表投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計213 集(Ａ)全枚数裏投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計214 集(Ａ)裏表混合投入件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計215 集(Ａ)表投入枚数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計216 集(Ａ)表投入枚数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計217 集(Ａ)表投入枚数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計218 集(Ａ)表投入枚数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計219 集(Ａ)裏投入枚数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計220 集(Ａ)裏投入枚数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計221 集(Ａ)裏投入枚数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計222 集(Ａ)裏投入枚数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計223 集(Ａ)券判定ＯＫ件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計224 集(Ａ)券判定ＯＫ枚数（合計）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計225 集(Ａ)券判定ＯＫ枚数（１枚投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計226 集(Ａ)券判定ＯＫ枚数（２枚投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計227 集(Ａ)券判定ＯＫ枚数（３枚投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計228 集(Ａ)券判定ＯＫ枚数（４枚投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計229 集(Ａ)券判定ＯＫ枚数（NRZｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計230 集(Ａ)券判定ＯＫ枚数（FMｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計231 集(Ａ)券判定ＯＫ枚数（NRZ定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計232 集(Ａ)券判定ＯＫ枚数（FM定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計233 集(Ａ)券判定ＯＫ枚数（FM大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計234 集(Ａ)券判定ＯＫ枚数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計235 集(Ａ)判定対象外券投入枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計236 集(Ａ)追加投入待ち件数（乗車券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計237 集(Ａ)追加投入待ち件数（特急券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計238 (Ａ)／入場券不正利用（通路を通過せず戻る行為）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計239 (Ａ)／入場券不正利用（２人組により連続投入する行為）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計240 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計241 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計242 集(Ａ)追加投入待ち件数（当駅から券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計243 集(Ａ)追加投入待ち件数（特急券＋当駅から券投入待ち）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計244 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計245 集(Ａ)その他ＩＣ処理枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計246 集(Ａ)ＩＣ処理件数（１枚処理）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計247 集(Ａ)追加投入待ち件数（在来ＩＣ）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計248 集(Ａ)追加投入待ち件数（新幹線専用券当駅から券なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計249 集(Ａ)ご利用票発券枚数（累計）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計250 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計251 集(Ａ)券判定ＮＧ件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計252 集(Ａ)異常券判定ＮＧ（表投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計253 集(Ａ)異常券判定ＮＧ（裏投入）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計254 集(Ａ)異常券判定ＮＧ（ﾊﾟﾘﾃｨｴﾗｰ：ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計255 集(Ａ)異常券判定ＮＧ（ﾊﾟﾘﾃｨｴﾗｰ：定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計256 集(Ａ)異常券判定ＮＧ（ﾌｫｰﾏｯﾄｴﾗｰ：ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計257 集(Ａ)異常券判定ＮＧ（ﾌｫｰﾏｯﾄｴﾗｰ：定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計258 集(Ａ)異常券判定ＮＧ（ﾌｫｰﾏｯﾄｴﾗｰ：大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計259 集(Ａ)異常券判定ＮＧ（ﾌｫｰﾏｯﾄｴﾗｰ：その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計260 集(Ａ)異常券判定ＮＧ（二重化ｴﾗｰ）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計261 集(Ａ)異常券判定ＮＧ（ｻﾑﾁｪｯｸｴﾗｰ：ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計262 集(Ａ)異常券判定ＮＧ（ｻﾑﾁｪｯｸｴﾗｰ：定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計263 集(Ａ)異常券判定ＮＧ（ｻﾑﾁｪｯｸｴﾗｰ：大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計264 集(Ａ)異常券判定ＮＧ（ｻﾑﾁｪｯｸｴﾗｰ：その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計265 集(Ａ)異常券判定ＮＧ（非磁気化券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計266 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計267 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計268 集(Ａ)無効券判定ＮＧ（券種判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計269 集(Ａ)無効券判定ＮＧ（大人券小児券混在判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計270 集(Ａ)無効券判定ＮＧ（期間判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計271 集(Ａ)無効券判定ＮＧ（区間判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計272 集(Ａ)無効券判定ＮＧ（入場券時間判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計273 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計274 集(Ａ)無効券判定ＮＧ（使用済判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計275 集(Ａ)無効券判定ＮＧ（自駅下車判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計276 集(Ａ)無効券判定ＮＧ（複乗判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計277 集(Ａ)無効券判定ＮＧ（複数枚有効判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計278 集(Ａ)無効券判定ＮＧ（使用開始後判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計279 集(Ａ)無効券判定ＮＧ（投入枚数判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計280 集(Ａ)有効組合せ判定ＮＧ（乗車券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計281 集(Ａ)有効組合せ判定ＮＧ（特急券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計282 集(Ａ)有効組合せ判定ＮＧ（当駅から乗車券なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計283 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計284 集(Ａ)有効組合せ判定ＮＧ（特急券＋当駅から乗車券なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計285 集(Ａ)有効組合せ判定ＮＧ（乗車券・特急券投入なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計286 集(Ａ)有効組合せ判定ＮＧ（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計287 集(Ａ)組合せ判定ＮＧ（乗車券･特急券区間比較判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計288 集(Ａ)有効組合せ判定ＮＧ（新幹線専用券当駅から乗車券なし）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計289 集(Ａ)組合せ判定ＮＧ（接続判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計290 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計291 集(Ａ)組合せ判定ＮＧ（併用判定）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計292 集(Ａ)在来ＩＣ＋新幹線磁気３枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計293 集(Ａ)追加投入待ち件数（ＥＸＩＣ、(幹)定期券(IC)）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計294 集(Ａ)不正判定ＮＧ（入出場サイクル異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計295 集(Ａ)不正判定ＮＧ（同一駅入出場異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計296 集(Ａ)不正判定ＮＧ（複数回使用異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計297 集(Ａ)ＩＣ処理件数（２枚処理）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計298 集(Ａ)その他ＮＧ（遅払い判定不可）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計299 集(Ａ)ＩＤチェック判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計300 集(Ａ)総ＩＣ磁気併用件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計301 集(Ａ)磁気書込件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計302 集(Ａ)磁気書込件数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計303 集(Ａ)磁気書込件数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計304 集(Ａ)磁気書込件数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計305 集(Ａ)磁気書込件数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計306 集(Ａ)磁気書込ﾘﾄﾗｲ件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計307 集(Ａ)磁気書込ﾘﾄﾗｲ回数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計308 集(Ａ)磁気書込ﾘﾄﾗｲ回数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計309 集(Ａ)磁気書込ﾘﾄﾗｲ回数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計310 集(Ａ)磁気書込ﾘﾄﾗｲ回数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計311 集(Ａ)磁気書込ﾘﾄﾗｲ→ＯＫ回数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計312 集(Ａ)磁気書込ﾘﾄﾗｲ→ＯＫ回数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計313 集(Ａ)磁気書込ﾘﾄﾗｲ→ＯＫ回数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計314 集(Ａ)磁気書込ﾘﾄﾗｲ→ＯＫ回数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計315 集(Ａ)磁気書込ﾘﾄﾗｲ→ＮＧ回数（ｴﾄﾞﾓﾝｿﾝ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計316 集(Ａ)磁気書込ﾘﾄﾗｲ→ＮＧ回数（定期券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計317 集(Ａ)磁気書込ﾘﾄﾗｲ→ＮＧ回数（大型券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計318 集(Ａ)磁気書込ﾘﾄﾗｲ→ＮＧ回数（その他）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計319 集(Ａ)パンチ回数（直接印刷部：改札ｴﾄﾞ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計320 集(Ａ)パンチ回数（直接印刷部：改札85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計321 集(Ａ)パンチ回数（直接印刷部：集札ｴﾄﾞ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計322 集(Ａ)パンチ回数（直接印刷部：集札85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計323 集(Ａ)パンチ回数（転写印刷部：85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計324 集(Ａ)印刷回数（直接印刷部：上側ｴﾄﾞ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計325 集(Ａ)印刷回数（直接印刷部：上側85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計326 集(Ａ)印刷回数（直接印刷部：下側ｴﾄﾞ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計327 集(Ａ)印刷回数（直接印刷部：下側85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計328 集(Ａ)印刷回数（転写直接印刷部：85mm券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計329 集(Ｆ)ＳＮＤ−Ｍ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計330 集(Ｆ)ＳＮＤ−Ｍ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計331 集(Ｆ)ＳＮＤ−Ｍ５動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計332 集(Ｆ)ＳＮＤ−Ｍ６動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計333 集(Ｆ)ＳＮＤ−Ｍ７動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計334 集(Ｆ)ＳＮＤ−Ｐ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計335 集(Ｆ)ＳＮＤ−Ｐ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計336 集(Ｆ)ＳＮＤ−Ｐ５動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計337 集(Ｆ)ＭＴＲ−Ｅ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計338 集(Ｆ)ＭＴＲ−Ｅ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計339 集(Ｆ)ＭＴＲ−Ｈ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計340 集(Ｆ)ＭＴＲ−Ｈ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計341 集(Ｆ)ＭＴＲ−Ｈ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計342 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計343 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計344 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計345 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計346 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計347 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計348 集(Ｆ)分離部取込動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計349 集(Ｆ)分離部繰出し動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計350 集(Ｆ)整列部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計351 集(Ａ)総集札枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計352 集(Ａ)１枚集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計353 集(Ａ)２枚集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計354 集(Ａ)３枚集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計355 集(Ａ)４枚集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計356 集(Ａ)総別集札枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計357 集(Ａ)１枚別集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計358 集(Ａ)２枚別集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計359 集(Ａ)３枚別集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計360 集(Ａ)４枚別集札件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計361 集(Ａ)保留件数（処理異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計362 集(Ａ)保留件数（不正）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計363 集(Ａ)二重化による救済枚数（B,Gﾄﾗｯｸ）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計364 集(Ａ)二重化による救済枚数（B,Gﾄﾗｯｸ以外）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計365 集(Ａ)整列部動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計366 集(Ａ)券反転回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計367 集(Ａ)ＥＸＩＣ＋磁気１枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計368 集(Ａ)ＥＸＩＣ＋磁気２枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計369 集(Ａ)ＥＸＩＣ＋磁気３枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計370 集(Ａ)在来ＩＣ＋新幹線磁気１枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計371 集(Ａ)運休 放出枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計372 集(Ａ)全車自由席 放出枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計373 集(Ａ)遅払い 印字枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計374 集(Ａ)在来ＩＣ＋新幹線磁気２枚処理件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計375 集(Ｆ)ＳＮＤ−Ａ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計376 集(Ｆ)ＳＮＤ−Ａ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計377 集(Ｆ)ＳＮＤ−Ａ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計378 集(Ｆ)ＳＮＤ−Ａ５動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計379 集(Ｆ)ＳＮＤ−Ｍ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計380 集(Ｆ)ＳＮＤ−Ｐ６動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計381 集(Ｆ)ＳＮＤ−Ｐ７動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計382 集(Ｆ)ＳＮＤ−Ｐ８動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計383 集(Ｆ)ＳＮＤ−Ｐ９動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計384 集(Ｆ)ＳＮＤ−Ｅ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計385 集(Ｆ)ＳＮＤ−Ｅ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計386 集(Ｆ)ＳＮＤ−Ｅ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計387 集(Ｆ)ＳＮＤ−Ｅ５動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計388 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計389 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計390 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計391 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計392 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計393 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計394 集(Ｆ)ＭＴＲ−Ａ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計395 集(Ｆ)ＭＴＲ−Ａ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計396 集(Ｆ)ＭＴＲ−Ａ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計397 集(Ｆ)ＭＴＲ−Ｍ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計398 集(Ａ)総ＩＣ処理受付枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計399 集(Ａ)ＥＸＩＣ処理受付枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計400 集(Ａ)在来ＩＣ処理受付枚数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計401 (Ｆ)主機集札一旦保留Ａ動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計402 (Ｆ)主機集札一旦保留Ｂ動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計403 (Ｆ)従機集札一旦保留Ａ動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計404 (Ａ)正券カウンタ満杯回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計405 (Ｆ)従機集札一旦保留Ｂ動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計406 (Ｆ)主機右ドア動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計407 (Ｆ)主機左ドア動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計408 (Ｆ)従機右ドア動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計409 (Ｆ)従機左ドア動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計410 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計411 改(Ｆ)ＳＮＤ−Ａ１動作回数  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計412 改(Ｆ)ＳＮＤ−Ｍ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計413 改(Ｆ)ＳＮＤ−Ｅ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計414 改(Ｆ)ＭＴＲ−Ｍ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計415 改(Ｆ)ＭＴＲ−Ｐ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計416 改(Ｆ)ＭＴＲ−Ｐ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計417 改(Ｆ)ＭＴＲ−Ｐ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計418 改(Ｆ)ＭＴＲ−Ｐ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計419 改(Ｆ)読取り部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計420 改(Ｆ)券反転部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計421 改(Ｆ)保留部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計422 改(Ｆ)直接パンチ部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計423 改(Ｆ)直接印刷部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計424 改(Ｆ)転写パンチ部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計425 改(Ｆ)転写印刷部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計426 改(Ｆ)放出部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計427 改(Ｆ)集札部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計428 改(Ｆ)別集札部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計429 改(Ｆ)発券動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計430 改(Ｆ)ＴＰＨ直接Ｌ印刷回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計431 改(Ｆ)ＴＰＨ直接Ｕ印刷回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計432 改(Ｆ)ＴＰＨ転写印刷回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計433 改(Ｆ)ＴＰＨ発券印刷回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計434 改(Ｆ)直接Φ３パンチ動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計435 改(Ｆ)転写Φ３パンチ動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計436 改(Ｆ)ＭＧ−ＲＵ大型券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計437 改(Ｆ)ＭＧ−ＲＵ普通券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計438 改(Ｆ)ＭＧ−ＲＬ大型券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計439 改(Ｆ)ＭＧ−ＲＬ普通券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計440 改(Ｆ)ＭＧ−Ｗ大型券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計441 改(Ｆ)ＭＧ−Ｗ普通券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計442 改(Ｆ)ＭＧ−Ｖ大型券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計443 改(Ｆ)ＭＧ−Ｖ普通券通過", Nothing, XlsByteOrder.LittleEndian), _
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
            New XlsField(8*4, "D", 1, " "c, "集計456 集(Ｆ)ＳＮＤ−Ａ１動作回数  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計457 集(Ｆ)ＳＮＤ−Ｍ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計458 集(Ｆ)ＳＮＤ−Ｅ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計459 集(Ｆ)ＭＴＲ−Ｍ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計460 集(Ｆ)ＭＴＲ−Ｐ１動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計461 集(Ｆ)ＭＴＲ−Ｐ２動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計462 集(Ｆ)ＭＴＲ−Ｐ３動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計463 集(Ｆ)ＭＴＲ−Ｐ４動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計464 集(Ｆ)読取り部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計465 集(Ｆ)券反転部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計466 集(Ｆ)保留部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計467 集(Ｆ)直接パンチ部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計468 集(Ｆ)直接印刷部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計469 集(Ｆ)転写パンチ部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計470 集(Ｆ)転写印刷部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計471 集(Ｆ)放出部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計472 集(Ｆ)集札部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計473 集(Ｆ)別集札部搬送回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計474 集(Ｆ)発券動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計475 集(Ｆ)ＴＰＨ直接Ｌ印刷回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計476 集(Ｆ)ＴＰＨ直接Ｕ印刷回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計477 集(Ｆ)ＴＰＨ転写印刷回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計478 集(Ｆ)ＴＰＨ発券印刷回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計479 集(Ｆ)直接Φ３パンチ動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計480 集(Ｆ)転写Φ３パンチ動作回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計481 集(Ｆ)ＭＧ−ＲＵ大型券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計482 集(Ｆ)ＭＧ−ＲＵ普通券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計483 集(Ｆ)ＭＧ−ＲＬ大型券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計484 集(Ｆ)ＭＧ−ＲＬ普通券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計485 集(Ｆ)ＭＧ−Ｗ大型券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計486 集(Ｆ)ＭＧ−Ｗ普通券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計487 集(Ｆ)ＭＧ−Ｖ大型券通過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計488 集(Ｆ)ＭＧ−Ｖ普通券通過", Nothing, XlsByteOrder.LittleEndian), _
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
            New XlsField(8*4, "D", 1, " "c, "集計500 （空き）", Nothing, XlsByteOrder.LittleEndian)}, _
        New XlsField() { _
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
            New XlsField(8*8, "X16", 1, " "c, "共通部 改札側搬送部番号"), _
            New XlsField(8*8, "X16", 1, " "c, "共通部 集札側搬送部番号"), _
            New XlsField(8*1, "D", 48, " "c, "共通部 改札側検知センサレベル"), _
            New XlsField(8*1, "D", 48, " "c, "共通部 集札側検知センサレベル"), _
            New XlsField(8*1, "X2", 48, " "c, "共通部 予備"), _
            New XlsField(8*4, "D", 1, " "c, "集計001 改(Ａ)読取異常−上ヘッド（エドモンソン券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計002 改(Ａ)読取異常−上ヘッド（８５ｍｍ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計003 改(Ａ)読取異常−下ヘッド（エドモンソン券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計004 改(Ａ)読取異常−下ヘッド（８５ｍｍ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計005 改(Ａ)読取異常−上ヘッド　１トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計006 改(Ａ)読取異常−上ヘッド　２トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計007 改(Ａ)読取異常−上ヘッド　３トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計008 改(Ａ)読取異常−上ヘッド　４トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計009 改(Ａ)読取異常−上ヘッド　５トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計010 改(Ａ)読取異常−上ヘッド　６トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計011 改(Ａ)読取異常−上ヘッド　７トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計012 改(Ａ)読取異常−上ヘッド　８トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計013 改(Ａ)読取異常−下ヘッド　１トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計014 改(Ａ)読取異常−下ヘッド　２トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計015 改(Ａ)読取異常−下ヘッド　３トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計016 改(Ａ)読取異常−下ヘッド　４トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計017 改(Ａ)読取異常−下ヘッド　５トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計018 改(Ａ)読取異常−下ヘッド　６トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計019 改(Ａ)読取異常−下ヘッド　７トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計020 改(Ａ)読取異常−下ヘッド　８トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計021 改(Ａ)書込異常回数−エドモンソン券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計022 改(Ａ)書込異常回数−定期券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計023 改(Ａ)書込異常回数−大型券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計024 改(Ａ)書込異常回数−その他（SFカード）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計025 改(Ａ)書込異常連続−エドモンソン券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計026 改(Ａ)書込異常連続−定期券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計027 改(Ａ)書込異常連続−大型券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計028 改(Ａ)書込異常連続−その他（SFカード）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計029 改(Ａ)書込異常連続−下ヘッド　１トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計030 改(Ａ)書込異常連続−下ヘッド　２トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計031 改(Ａ)書込異常連続−下ヘッド　３トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計032 改(Ａ)書込異常連続−下ヘッド　４トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計033 改(Ａ)書込異常連続−下ヘッド　５トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計034 改(Ａ)書込異常連続−下ヘッド　６トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計035 改(Ａ)書込異常連続−下ヘッド　７トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計036 改(Ａ)書込異常連続−下ヘッド　８トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計037 改(Ａ)ＩＣＲＷ異常検知回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計038 改(Ａ)ご利用票発券異常件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計039 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計040 改(Ａ)総ＩＣ未了件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計041 改(Ａ)ＩＣ読取り未了件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計042 改(Ａ)ＥＸＩＣ書込み未了件数（１枚処理時）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計043 改(Ａ)在来ＩＣ書込み未了件数（１枚処理時）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計044 改(Ａ)ＩＣ読取判定異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計045 改(Ａ)ＩＣ枚数超過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計046 改(Ａ)ＩＣＩＤｉ判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計047 改(Ａ)ＥＸＩＣ予約情報検索ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計048 改(Ａ)ＥＸＩＣバージョン判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計049 改(Ａ)ＥＸＩＣデータ項目判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計050 改(Ａ)ＥＸＩＣカード使用不可判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計051 改(Ａ)ＥＸＩＣ最終利用日付判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計052 改(Ａ)ＥＸＩＣネガチェック判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計053 改(Ａ)ＥＸＩＣ入出場シーケンス判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計054 改(Ａ)ＥＸＩＣ予約情報判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計055 改(Ａ)ＥＸＩＣ終列車判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計056 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計057 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計058 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計059 改(Ａ)ＥＸＩＣ在来線未出場ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計060 改(Ａ)EXIC当駅迄券なしＮＧ当駅から券なしＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計061 改(Ａ)在来ＩＣバージョン判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計062 改(Ａ)在来ＩＣＩＣ種別判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計063 改(Ａ)在来ＩＣデータ項目判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計064 改(Ａ)在来ＩＣカード正当性判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計065 改(Ａ)在来ＩＣマスタデータ判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計066 改(Ａ)在来ＩＣ活性化判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計067 改(Ａ)在来ＩＣカード使用不可判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計068 改(Ａ)在来ＩＣネガチェック判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計069 改(Ａ)在来ＩＣ定期券期間判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計070 改(Ａ)在来ＩＣ入出場シーケンス判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計071 改(Ａ)在来ＩＣ利用日付判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計072 改(Ａ)在来ＩＣ自駅下車判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計073 改(Ａ)在来ＩＣ区間判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計074 改(Ａ)在来ＩＣ入出場コード判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計075 改(Ａ)在来ＩＣ残額判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計076 改(Ａ)在来ＩＣ精算判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計077 改(Ａ)在来ＩＣ一点通過判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計078 改(Ａ)在来IC不正判定ＮＧ（入出場サイクル異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計079 改(Ａ)在来IC不正判定ＮＧ（入出場時間異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計080 改(Ａ)在来IC不正判定ＮＧ（同一駅入出場異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計081 改(Ａ)在来IC不正判定ＮＧ（連続入場・出場異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計082 改(Ａ)在来ＩＣ新幹線有効券なしＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計083 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計084 改(Ａ)磁気ＩＣ併用大小混在判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計085 改(Ａ)磁気ＩＣ併用新幹線区間重複ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計086 改(Ａ)磁気ＩＣ併用接続ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計087 改(Ａ)磁気ＩＣ併用当駅迄券複数枚ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計088 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計089 改(Ａ)磁気ＩＣ併用有効券複数枚ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計090 改(Ａ)磁気ＩＣ併用精算不可ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計091 改(Ａ)ＥＸＩＣ書込み異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計092 改(Ａ)在来ＩＣ書込み異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計093 改(Ａ)在来ＩＣテストカード判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計094 改(Ａ)在来ＩＣ定期区間エリアＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計095 改(Ａ)在来ＩＣ最終利用日付判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計096 改(Ａ)在来ＩＣ他社割引ＩＣカードＳＦ利用ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計097 改(Ａ)ＥＸＩＣ書込み未了件数（２枚処理時）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計098 改(Ａ)在来ＩＣ書込み未了件数（２枚処理時）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計099 改(Ａ)総ＩＣ判定ＮＧ件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計100 改(Ａ)在来ＩＣ会社間経路連続性判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計101 改(Ｆ)分離部ｾﾝｻ電源      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計102 改(Ｆ)分離部ｿﾚﾉｲﾄﾞ電源      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計103 改(Ｆ)分離部ｿﾚﾉｲﾄﾞPL検知    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計104 改(Ｆ)分離部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計105 改(Ｆ)分離部ﾓｰﾀ電源ｱﾗｰﾑ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計106 改(Ｆ)＋２４Ｖ電源          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計107 改(Ｆ)磁気部ｾﾝｻ電源         ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計108 改(Ｆ)磁気部ｿﾚﾉｲﾄﾞ電源      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計109 改(Ｆ)磁気部ｿﾚﾉｲﾄﾞPL検知    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計110 改(Ｆ)磁気ﾗｲﾄｱﾗｰﾑ(ON時間)   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計111 改(Ｆ)磁気部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計112 改(Ｆ)磁気ﾗｲﾄ電源電圧     ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計113 改(Ｆ)印刷〜放出部ｾﾝｻ電源   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計114 改(Ｆ)印刷〜放出部ｿﾚﾉｲﾄﾞPL  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計115 改(Ｆ)印刷部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計116 改(Ｆ)放出部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計117 改(Ｆ)集札部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計118 改(Ｆ)発券部H1ﾓｰﾀｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計119 改(Ｆ)発券部H2ﾓｰﾀｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計120 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計121 改(Ｆ)Ｅ２ＰＲＯＭ異常    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計122 改(Ｆ)直接パンチ異常      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計123 改(Ｆ)転写パンチ異常      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計124 改(Ｆ)直接上印刷動作異常  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計125 改(Ｆ)直接下印刷動作異常  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計126 改(Ｆ)転写印刷動作異常    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計127 改(Ｆ)転写リボン切れ      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計128 改(Ｆ)発券ﾛｰﾙ紙切れ       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計129 改(Ｆ)発券ﾛｰﾙ紙ｾｯﾄ不良    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計130 改(Ｆ)発券部ｶｯﾀ位置異常   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計131 改(Ｆ)分離部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計132 改(Ｆ)整列部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計133 改(Ｆ)反転部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計134 改(Ｆ)書込前券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計135 改(Ｆ)保留１券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計136 改(Ｆ)保留２券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計137 改(Ｆ)保留３券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計138 改(Ｆ)発券保留部券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計139 改(Ｆ)直接パンチ前券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計140 改(Ｆ)直接パンチ後券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計141 改(Ｆ)直接下印刷部券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計142 改(Ｆ)直接上印刷部券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計143 改(Ｆ)転写パンチ前券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計144 改(Ｆ)転写パンチ後券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計145 改(Ｆ)転写印刷部券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計146 改(Ｆ)直接印刷異常券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計147 改(Ｆ)転写印刷異常券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計148 改(Ｆ)集積部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計149 改(Ｆ)放出部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計150 改(Ｆ)集札部券詰り    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計151 改(Ｆ)放出部券詰り(取)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計152 改(Ｆ)集札部券詰り(取)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計153 改(Ｆ)印刷〜放出部券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計154 改(Ｆ)発券部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計155 改(Ｆ)発券部装填券詰り    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計156 改(Ｆ)保留１すり抜け      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計157 改(Ｆ)保留２すり抜け      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計158 改(Ｆ)保留３すり抜け      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計159 改(Ｆ)直接部ｽﾄｯﾊﾟすり抜け ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計160 改(Ｆ)転写部ｽﾄｯﾊﾟすり抜け ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計161 改(Ｆ)反転部すり抜け      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計162 改(Ｆ)反転部振り分け異常  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計163 改(Ｆ)保留分岐振り分け異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計164 改(Ｆ)印刷分岐振り分け異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計165 改(Ｆ)集札振り分け異常    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計166 改(Ｆ)放出振り分け異常    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計167 改(Ｆ)一旦集札振り分け異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計168 改(Ｆ)磁気CPU異常１       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計169 改(Ｆ)磁気CPU異常２       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計170 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計171 改(Ｆ)センサ異常          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計172 改(Ｆ)セット不良          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計173 改(Ｆ)コマンド異常  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計174 改(Ｆ)重送検知回数        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計175 改(Ｆ)Ｔ検故障回数        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計176 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計177 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計178 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計179 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計180 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計181 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計182 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計183 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計184 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計185 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計186 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計187 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計188 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計189 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計190 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計191 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計192 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計193 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計194 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計195 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計196 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計197 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計198 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計199 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計200 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計201 集(Ａ)読取異常−上ヘッド（エドモンソン券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計202 集(Ａ)読取異常−上ヘッド（８５ｍｍ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計203 集(Ａ)読取異常−下ヘッド（エドモンソン券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計204 集(Ａ)読取異常−下ヘッド（８５ｍｍ券）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計205 集(Ａ)読取異常−上ヘッド １トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計206 集(Ａ)読取異常−上ヘッド ２トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計207 集(Ａ)読取異常−上ヘッド ３トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計208 集(Ａ)読取異常−上ヘッド ４トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計209 集(Ａ)読取異常−上ヘッド ５トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計210 集(Ａ)読取異常−上ヘッド ６トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計211 集(Ａ)読取異常−上ヘッド ７トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計212 集(Ａ)読取異常−上ヘッド ８トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計213 集(Ａ)読取異常−下ヘッド １トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計214 集(Ａ)読取異常−下ヘッド ２トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計215 集(Ａ)読取異常−下ヘッド ３トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計216 集(Ａ)読取異常−下ヘッド ４トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計217 集(Ａ)読取異常−下ヘッド ５トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計218 集(Ａ)読取異常−下ヘッド ６トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計219 集(Ａ)読取異常−下ヘッド ７トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計220 集(Ａ)読取異常−下ヘッド ８トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計221 集(Ａ)書込異常回数−エドモンソン券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計222 集(Ａ)書込異常回数−定期券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計223 集(Ａ)書込異常回数−大型券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計224 集(Ａ)書込異常回数−その他（SFカード）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計225 集(Ａ)書込異常連続−エドモンソン券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計226 集(Ａ)書込異常連続−定期券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計227 集(Ａ)書込異常連続−大型券", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計228 集(Ａ)書込異常連続−その他（SFカード）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計229 集(Ａ)書込異常連続−下ヘッド １トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計230 集(Ａ)書込異常連続−下ヘッド ２トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計231 集(Ａ)書込異常連続−下ヘッド ３トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計232 集(Ａ)書込異常連続−下ヘッド ４トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計233 集(Ａ)書込異常連続−下ヘッド ５トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計234 集(Ａ)書込異常連続−下ヘッド ６トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計235 集(Ａ)書込異常連続−下ヘッド ７トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計236 集(Ａ)書込異常連続−下ヘッド ８トラック", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計237 集(Ａ)ＩＣＲＷ異常検知回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計238 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計239 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計240 集(Ａ)総ＩＣ未了件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計241 集(Ａ)ＩＣ読取り未了件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計242 集(Ａ)ＥＸＩＣ書込み未了件数（１枚処理時）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計243 集(Ａ)在来ＩＣ書込み未了件数（１枚処理時）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計244 集(Ａ)ＩＣ読取判定異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計245 集(Ａ)ＩＣ枚数超過", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計246 集(Ａ)ＩＣＩＤｉ判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計247 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計248 集(Ａ)ＥＸＩＣバージョン判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計249 集(Ａ)ＥＸＩＣデータ項目判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計250 集(Ａ)ＥＸＩＣカード使用不可判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計251 集(Ａ)ＥＸＩＣ最終利用日付判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計252 集(Ａ)ＥＸＩＣネガチェック判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計253 集(Ａ)ＥＸＩＣ入出場シーケンス判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計254 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計255 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計256 集(Ａ)ＥＸＩＣ利用日付判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計257 集(Ａ)ＥＸＩＣ自駅下車判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計258 集(Ａ)ＥＸＩＣ区間判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計259 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計260 集(Ａ)EXIC当駅迄券なしNG当駅から券なしNG", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計261 集(Ａ)在来ＩＣバージョン判定", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計262 集(Ａ)在来ＩＣＩＣ種別判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計263 集(Ａ)在来ＩＣデータ項目判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計264 集(Ａ)在来ＩＣカード正当性判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計265 集(Ａ)在来ＩＣマスタデータ判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計266 集(Ａ)在来ＩＣ活性化判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計267 集(Ａ)在来ＩＣカード使用不可判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計268 集(Ａ)在来ＩＣネガチェック判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計269 集(Ａ)在来ＩＣ定期券期間判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計270 集(Ａ)在来ＩＣ入出場シーケンス判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計271 集(Ａ)在来ＩＣ利用日付判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計272 集(Ａ)在来ＩＣ自駅下車判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計273 集(Ａ)在来ＩＣ区間判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計274 集(Ａ)在来ＩＣ入出場コード判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計275 集(Ａ)在来ＩＣ残額判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計276 集(Ａ)在来ＩＣ精算判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計277 集(Ａ)在来ＩＣ一点通過判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計278 集(Ａ)在来IC不正判定ＮＧ（入出場サイクル異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計279 集(Ａ)在来ＩＣ不正判定ＮＧ(入出場時間異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計280 集(Ａ)在来ＩＣ不正判定ＮＧ（同一駅入出場異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計281 集(Ａ)在来IC不正判定ＮＧ（連続入場・出場異常）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計282 集(Ａ)在来ＩＣ新幹線有効券なしＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計283 集(Ａ)在来ＩＣ未出場ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計284 集(Ａ)磁気ＩＣ併用大小混在判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計285 集(Ａ)磁気ＩＣ併用新幹線区間重複ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計286 集(Ａ)磁気ＩＣ併用接続ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計287 集(Ａ)磁気ＩＣ併用当駅迄券複数枚ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計288 集(Ａ)磁気IC併用当駅から券複数枚ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計289 集(Ａ)磁気IC併用有効券複数枚ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計290 集(Ａ)磁気ＩＣ併用精算不可ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計291 集(Ａ)ＥＸＩＣ書込み異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計292 集(Ａ)在来ＩＣ書込み異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計293 集(Ａ)在来ＩＣテストカード判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計294 集(Ａ)在来ＩＣ定期区間エリアＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計295 集(Ａ)在来ＩＣ最終利用日付判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計296 集(Ａ)在来ＩＣ他社割引ＩＣカードＳＦ利用ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計297 集(Ａ)ＥＸＩＣ書込み未了件数（２枚処理時）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計298 集(Ａ)在来ＩＣ書込み未了件数（２枚処理時）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計299 集(Ａ)総ＩＣ判定ＮＧ件数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計300 集(Ａ)在来ＩＣ会社間経路連続性判定ＮＧ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計301 集(Ｆ)分離部ｾﾝｻ電源      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計302 集(Ｆ)分離部ｿﾚﾉｲﾄﾞ電源      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計303 集(Ｆ)分離部ｿﾚﾉｲﾄﾞPL検知    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計304 集(Ｆ)分離部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計305 集(Ｆ)分離部ﾓｰﾀ電源ｱﾗｰﾑ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計306 集(Ｆ)＋２４Ｖ電源          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計307 集(Ｆ)磁気部ｾﾝｻ電源         ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計308 集(Ｆ)磁気部ｿﾚﾉｲﾄﾞ電源      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計309 集(Ｆ)磁気部ｿﾚﾉｲﾄﾞPL検知    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計310 集(Ｆ)磁気ﾗｲﾄｱﾗｰﾑ(ON時間)   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計311 集(Ｆ)磁気部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計312 集(Ｆ)磁気ﾗｲﾄ電源電圧     ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計313 集(Ｆ)印刷〜放出部ｾﾝｻ電源   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計314 集(Ｆ)印刷〜放出部ｿﾚﾉｲﾄﾞPL  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計315 集(Ｆ)印刷部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計316 集(Ｆ)放出部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計317 集(Ｆ)集札部ﾓｰﾀﾄﾞﾗｲﾊﾞｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計318 集(Ｆ)発券部H1ﾓｰﾀｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計319 集(Ｆ)発券部H2ﾓｰﾀｱﾗｰﾑ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計320 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計321 集(Ｆ)Ｅ２ＰＲＯＭ異常    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計322 集(Ｆ)直接パンチ異常      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計323 集(Ｆ)転写パンチ異常      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計324 集(Ｆ)直接上印刷動作異常  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計325 集(Ｆ)直接下印刷動作異常  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計326 集(Ｆ)転写印刷動作異常    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計327 集(Ｆ)転写リボン切れ      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計328 集(Ｆ)発券ﾛｰﾙ紙切れ       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計329 集(Ｆ)発券ﾛｰﾙ紙ｾｯﾄ不良    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計330 集(Ｆ)発券部ｶｯﾀ位置異常   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計331 集(Ｆ)分離部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計332 集(Ｆ)整列部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計333 集(Ｆ)反転部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計334 集(Ｆ)書込前券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計335 集(Ｆ)保留１券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計336 集(Ｆ)保留２券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計337 集(Ｆ)保留３券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計338 集(Ｆ)発券保留部券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計339 集(Ｆ)直接パンチ前券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計340 集(Ｆ)直接パンチ後券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計341 集(Ｆ)直接下印刷部券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計342 集(Ｆ)直接上印刷部券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計343 集(Ｆ)転写パンチ前券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計344 集(Ｆ)転写パンチ後券詰り", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計345 集(Ｆ)転写印刷部券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計346 集(Ｆ)直接印刷異常券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計347 集(Ｆ)転写印刷異常券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計348 集(Ｆ)集積部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計349 集(Ｆ)放出部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計350 集(Ｆ)集札部券詰り    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計351 集(Ｆ)放出部券詰り(取)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計352 集(Ｆ)集札部券詰り(取)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計353 集(Ｆ)印刷〜放出部券詰り  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計354 集(Ｆ)発券部券詰り      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計355 集(Ｆ)発券部装填券詰り    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計356 集(Ｆ)保留１すり抜け      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計357 集(Ｆ)保留２すり抜け      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計358 集(Ｆ)保留３すり抜け      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計359 集(Ｆ)直接部ｽﾄｯﾊﾟすり抜け ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計360 集(Ｆ)転写部ｽﾄｯﾊﾟすり抜け ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計361 集(Ｆ)反転部すり抜け      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計362 集(Ｆ)反転部振り分け異常  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計363 集(Ｆ)保留分岐振り分け異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計364 集(Ｆ)印刷分岐振り分け異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計365 集(Ｆ)集札振り分け異常    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計366 集(Ｆ)放出振り分け異常    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計367 集(Ｆ)一旦集札振り分け異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計368 集(Ｆ)磁気CPU異常１       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計369 集(Ｆ)磁気CPU異常２       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計370 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計371 集(Ｆ)センサ異常          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計372 集(Ｆ)セット不良          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計373 集(Ｆ)コマンド異常  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計374 集(Ｆ)重送検知回数        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計375 集(Ｆ)Ｔ検故障回数        ", Nothing, XlsByteOrder.LittleEndian), _
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
            New XlsField(8*4, "D", 1, " "c, "集計401 (Ｆ)人間検知故障（反射）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計402 (Ｆ)人間検知故障（透過）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計403 (Ｆ)ラインセンサ故障警告回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計404 (Ｆ)主機集札一旦保留Ａ異常回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計405 (Ｆ)主機集札一旦保留Ｂ異常回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計406 (Ｆ)従機集札一旦保留Ａ異常回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計407 (Ｆ)従機集札一旦保留Ｂ異常回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計408 （Ｆ）主機集札一旦保留Ａ満杯検知回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計409 （Ｆ）主機集札一旦保留Ｂ満杯検知回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計410 （Ｆ）従機集札一旦保留Ａ満杯検知回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計411 （Ｆ）従機集札一旦保留Ｂ満杯検知回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計412 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計413 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計414 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計415 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計416 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計417 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計418 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計419 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計420 （空き）", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計421 (Ａ)ドア故障−集札・構内側", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計422 (Ａ)ドア故障−集札・構外側", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計423 (Ａ)ドア故障−改札・構内側", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計424 (Ａ)ドア故障−改札・構外側", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計425 (Ａ)処理中断異常", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計426 (Ａ)機器異常自動復帰の再起動回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計427 (Ａ)省電力モード強制復帰回数", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "集計428 (Ａ)近接センサ故障回数", Nothing, XlsByteOrder.LittleEndian), _
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
            New XlsField(8*4, "D", 1, " "c, "集計500 （空き）", Nothing, XlsByteOrder.LittleEndian)}}

    Shared Sub New()
        For k As Integer = 0 To 1
            oFieldRefs(k) = New Dictionary(Of String, FieldRef)
            Dim bits As Integer = 0
            For i As Integer = 0 To oFields(k).Length - 1
                Dim oField As XlsField = oFields(k)(i)
                oFieldRefs(k).Add(oField.MetaName, New FieldRef(oField, bits, i))
                bits += oField.ElementBits * oField.ElementCount
            Next i
            totalBitCount(k) = bits
        Next k
    End Sub

    Public Shared ReadOnly Property RecordLengthInBits(ByVal k As Integer) As Integer
        Get
            Return totalBitCount(k)
        End Get
    End Property

    Public Shared ReadOnly Property RecordLengthInBytes(ByVal k As Integer) As Integer
        Get
            Return (totalBitCount(k) + 7) \ 8
        End Get
    End Property

    Public Shared ReadOnly Property Fields(ByVal k As Integer) As XlsField()
        Get
            Return oFields(k)
        End Get
    End Property

    Public Shared ReadOnly Property Field(ByVal k As Integer, ByVal sMetaName As String) As XlsField
        Get
            Return oFieldRefs(k)(sMetaName).Field
        End Get
    End Property

    Public Shared Function FieldIndexOf(ByVal k As Integer, ByVal sMetaName As String) As Integer
        Return oFieldRefs(k)(sMetaName).Index
    End Function

    Public Shared Function GetFieldValueFromBytes(ByVal k As Integer, ByVal sMetaName As String, ByVal oBytes As Byte()) As String
        Dim oRef As FieldRef = oFieldRefs(k)(sMetaName)
        Return oRef.Field.CreateValueFromBytes(oBytes, oRef.BitOffset)
    End Function

    Public Shared Sub SetFieldValueToBytes(ByVal k As Integer, ByVal sMetaName As String, ByVal sValue As String, ByVal oBytes As Byte())
        Dim oRef As FieldRef = oFieldRefs(k)(sMetaName)
        oRef.Field.CopyValueToBytes(sValue, oBytes, oRef.BitOffset)
    End Sub

    Public Shared Sub InitBaseHeaderFields(ByVal k As Integer, ByVal machine As EkCode, ByVal d As DateTime, ByVal seqNum As UInteger, ByVal oBytes As Byte())
        SetFieldValueToBytes(k, "基本ヘッダー データ種別", If(k = 0, "A7", "A8"), oBytes)
        SetFieldValueToBytes(k, "基本ヘッダー 駅コード", machine.ToString("%3R-%3S"), oBytes)
        SetFieldValueToBytes(k, "基本ヘッダー 処理日時", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes(k, "基本ヘッダー コーナー", machine.ToString("%C"), oBytes)
        SetFieldValueToBytes(k, "基本ヘッダー 号機", machine.ToString("%U"), oBytes)
        SetFieldValueToBytes(k, "基本ヘッダー シーケンスNo", seqNum.ToString(), oBytes)
        SetFieldValueToBytes(k, "基本ヘッダー バージョン", "01", oBytes)
    End Sub

    Public Shared Sub InitCommonPartFields(ByVal k As Integer, ByVal machine As EkCode, ByVal d As DateTime, ByVal oBytes As Byte())
        SetFieldValueToBytes(k, "共通部 集計開始日時", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes(k, "共通部 集計終了(収集)日時", "00000000000000", oBytes)
        SetFieldValueToBytes(k, "共通部 改札側搬送部点検日時", "00000000000000", oBytes)
        SetFieldValueToBytes(k, "共通部 集札側搬送部点検日時", "00000000000000", oBytes)
        'TODO: この２項目は窓処向けの実装になっており、改札機用につくりなおしたいが、もとになる情報がないので、このままでよい気も。
        SetFieldValueToBytes(k, "共通部 改札側搬送部番号", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes(k, "共通部 集札側搬送部番号", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes(k, "共通部 改札側検知センサレベル", Field(k, "共通部 改札側検知センサレベル").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes(k, "共通部 集札側検知センサレベル", Field(k, "共通部 集札側検知センサレベル").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes(k, "共通部 予備", Field(k, "共通部 予備").CreateDefaultValue(), oBytes)
    End Sub

    Public Shared Sub UpdateSummaryFields(ByVal oBytes As Byte()())
        'TODO: 改札機用につくりなおす。
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 293).MetaName, GetSummary(294, 344, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 273).MetaName, GetSummary(274, 282, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 203).MetaName, GetSummary(204, 262, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 184).MetaName, GetSummary(185, 192, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 149).MetaName, GetSummary(150, 173, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 128).MetaName, GetSummary(129, 138, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 112).MetaName, GetSummary(113, 117, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 9).MetaName, GetSummary(99, 101, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 8).MetaName, GetSummary(97, 98, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 7).MetaName, GetSummary(79, 86, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 6).MetaName, GetSummary(57, 68, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 5).MetaName, GetSummary(41, 46, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 4).MetaName, GetSummary(23, 30, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 3).MetaName, GetFieldValueFromBytes(Fields(AggregateFieldsOrigin + 7).MetaName, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 2).MetaName, GetSummary(New Integer() {6, 8}, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 1).MetaName, GetSummary(New Integer() {4, 5, 9}, oBytes), oBytes)
    End Sub

    Private Shared Function GetSummary(ByVal k As Integer, ByVal firstAggregateNumber As Integer, ByVal lastAggregateNumber As Integer, ByVal oBytes As Byte()) As String
        Dim sum As Long = 0
        For i As Integer = AggregateFieldsOrigin + firstAggregateNumber To AggregateFieldsOrigin + lastAggregateNumber
            sum += Long.Parse(GetFieldValueFromBytes(k, oFields(k)(i).MetaName, oBytes))
        Next i
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

    Private Shared Function GetSummary(ByVal aggregateIds As AggregateIdentifier(), ByVal oBytes As Byte()()) As String
        Dim sum As Long = 0
        For Each id As AggregateIdentifier In aggregateIds
            Dim k As Integer = id.Kind
            Dim i As Integer = AggregateFieldsOrigin + id.Number
            sum += Long.Parse(GetFieldValueFromBytes(k, oFields(k)(i).MetaName, oBytes(k)))
        Next id
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

    Private Structure AggregateIdentifier
        Public Kind As Integer
        Public Number As Integer
        Public Sub New(ByVal k As Integer, ByVal n As Integer)
            Kind = k
            Number = n
        End Sub
    End Structure

End Class
