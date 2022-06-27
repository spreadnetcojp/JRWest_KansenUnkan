' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/25  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

''' <summary>
''' DB仕様の定数を定義するクラス。
''' </summary>
Public Class DbConstants

    'NOTE: データベース仕様になっている定数でありながら、その実、
    '駅務機器とのI/F仕様で決まっている定数（またはEkCommonで使用する定数）
    'については、ここではなく、EkConstantsで定義する。
    '駅務機器とのI/F仕様は、運管システムの内部仕様であるデータベース仕様
    'よりも先に決められている（より基本的な）仕様である。
    '前者を担当するEkCommonモジュールは、後者を担当するDBCommonモジュール
    'よりも低位のモジュールであり、前者が後者に依存してはならない。

    '通信状態管理テーブルにセットするポート区分
    Public Const PortPurposeGeneralData As String = "1" '通常データ用
    Public Const PortPurposeRiyoData As String = "2"    '利用データ用

    'DLL状態テーブルの配信状態値
    'NOTE: 全て運管システム内の仕様である。
    Public Const DllStatusNormal As Integer = &H0
    Public Const DllStatusAbnormal As Integer = &H1
    Public Const DllStatusContentError As Integer = &H2
    Public Const DllStatusTimeout As Integer = &H3
    Public Const DllStatusExecuting As Integer = &HFFFF

    'DL状態テーブルの配信状態値
    '-------Ver0.1　フェーズ２　「適用済み」状態を追加　MOD START-----------
    'NOTE: 下記で定義した値のみ運管システム内部に入り込んだ
    '（あるいは運管システム内で完結した）仕様である。
    '下記以外は駅務機器の仕様次第で容易に追加される可能性があり、
    '運管システムの処理に影響する値でもないため、
    '駅務機器から受け取った値をそのままDBに登録する。
    '駅務機器が定義を追加した際は、DL状態名称テーブルに
    'その値と表示文言を追加するだけでよい。
    Public Const DlStatusNormal As Integer = &H0
    Public Const DlStatusContinuingNormal As Integer = &HF
    Public Const DlStatusPreExecuting As Integer = &HFFFE
    Public Const DlStatusExecuting As Integer = &HFFFF
    '-------Ver0.1　フェーズ２　「適用済み」状態を追加　MOD END-----------

    '収集データ誤記テーブルのレコード種別
    Public Const CdtKindAll As String = "全データ種別"
    Public Const CdtKindBesshuData As String = "別集札データ"
    Public Const CdtKindFuseiJoshaData As String = "不正乗車検出データ"
    Public Const CdtKindKyokoToppaData As String = "強行突破検出データ"
    Public Const CdtKindFunshitsuData As String = "紛失券検出データ"
    Public Const CdtKindFrexData As String = "FREX定期券ID検出データ"
    Public Const CdtKindFaultData As String = "異常データ（監視盤／改札機／窓口処理機）"
    Public Const CdtKindKadoData As String = "稼動・保守データ（改札機、窓口処理機）"
    Public Const CdtKindTrafficData As String = "時間帯別乗降データ"
    Public Const CdtKindKsbConfig As String = "監視盤設定情報"
    Public Const CdtKindConStatus As String = "機器接続状態"
    Public Const CdtKindServerError As String = "サーバ内異常"

    'データ種別に対応する収集データ誤記テーブルのレコード種別
    Public Shared ReadOnly CdtKindsOfDataKinds As New Dictionary(Of String, String()) From { _
       {"BSY", New String() {CdtKindBesshuData}}, _
       {"MEI", New String() {CdtKindFuseiJoshaData, CdtKindKyokoToppaData, CdtKindFunshitsuData, CdtKindFrexData}}, _
       {"ERR", New String() {CdtKindFaultData}}, _
       {"KDO", New String() {CdtKindKadoData}}, _
       {"TIM", New String() {CdtKindTrafficData}}}

    'SNMP通知の重大度
    Public Const SnmpSeverityWarning As String = "WARNING"   '注意域
    Public Const SnmpSeverityCritical As String = "CRITICAL" '危険域

End Class
