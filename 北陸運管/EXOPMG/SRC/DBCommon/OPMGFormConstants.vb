' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

''' <summary>
''' 帳票常数を保存するクラス。
''' </summary>
''' <remarks></remarks>
Public Class OPMGFormConstants

    'TODO: Lexisに定義しなおしたものを使う。
    'このファイル自体を消して、コンパイルエラーが出たところを修正する。

    '現SFカード
    Public Const MST_SF1_CARD As String = "現SFカード"

    '新SFカード
    Public Const MST_SF2_CARD As String = "新SFカード"

    '新追加休日
    Public Const MST_ADD_HOLIDAY As String = "新追加休日"

    '新設定パラメータ
    Public Const MST_SET_PARA As String = "新設定パラメータ"

    '現IC発行
    Public Const MST_IC1_ISSUE As String = "現IC発行"

    '新IC発行
    Public Const MST_IC2_ISSUE As String = "新IC発行"

    'マスタ配信指示確認（配信データ）
    Public Const MST_ORDER_DATA As String = "マスタ配信指示確認（配信データ）"

    'マスタ配信指示確認（配信先）
    Public Const MST_ORDER_POINT As String = "マスタ配信指示確認（配信先）"

    'マスタ配信情報
    Public Const MST_ORDER_INFO As String = "マスタ配信情報"

    'マスタバージョン
    Public Const MST_VERSION As String = "マスタバージョン"

    'マスタ適用状況
    Public Const MST_APPLY_SITUATION As String = "マスタ適用状況"

    'プログラム配信指示確認（配信データ）
    Public Const PRO_ORDER_DATA As String = "プログラム配信指示確認（配信データ）"

    'プログラム配信指示確認（配信先）
    Public Const PRO_ORDER_POINT As String = "プログラム配信指示確認（配信先）"

    'プログラム配信情報
    Public Const PRO_ORDER_INFO As String = "プログラム配信情報"

    'プログラムバージョン
    Public Const PRO_VERSION As String = "プログラムバージョン"

    'プログラム適用状況
    Public Const PRO_APPLY_SITUATION As String = "プログラム適用状況"

    '-----Ver0.2 Start
    '出改札統括サーバ配信指示確認（配信データ
    Public Const UNI_ORDER_DATA As String = "出改札統括サーバ配信指示確認（配信データ）"

    '出改札統括サーバ配信指示確認（配信先）
    Public Const UNI_ORDER_POINT As String = "出改札統括サーバ配信指示確認（配信先）"

    'V0.1 Start
    '出改札統括サーババージョン詳細表示
    Public Const UNI_VERSION As String = "出改札統括サーババージョン詳細表示"
    'V0.1 End

    '出改札統括サーバ配信情報
    Public Const UNI_ORDER_INFO As String = "出改札統括サーバ配信情報"
    '-----Ver0.2 End

    '出力端末
    Public Const OUT_TERMINAL As String = "出力端末："

    '出力日時
    Public Const OUT_DATE_TIME As String = "出力日時："

    '適用日
    Public Const APPLY_DATE As String = "適用日："

    'マスタ名称
    Public Const MST_NAME As String = "マスタ名称："

    'グループ
    Public Const GROUP_STR As String = "グループ："

    '駅
    Public Const STATION_STR As String = "駅："

    '駅名
    Public Const STATION_NAME As String = "駅名："

    'コーナー
    Public Const CORNER_STR As String = "コーナー："

    '機種
    Public Const EQUIPMENT_TYPE As String = "機種："

    '号機
    Public Const NUM_EQUIPMENT As String = "号機："

    '状態
    Public Const STATUS_STR As String = "状態："

    'パターン名称
    Public Const PATTERN_NAME As String = "パターン名称："

    'データ名称
    Public Const DATA_NAME As String = "データ名称："

    'バージョン
    Public Const VERSION_STR As String = "バージョン："

    '最終登録日時
    Public Const LAST_LOGIN_DATE As String = "最終登録日時："

    '適用開始日
    Public Const APPLY_START_DATE As String = "適用開始日："

    'ＩＣ発行機関
    Public Const IC_ISSUE_ORGAN As String = "ＩＣ発行機関"

    'ＩＣカード機関
    Public Const IC_CARD_ORGAN As String = "ＩＣカード機関"

    'ＳＦカード機関
    Public Const SF_CARD_ORGAN As String = "ＳＦカード機関"

    'プログラム名称
    Public Const PRO_NAME As String = "プログラム名称："

    '機種名称
    Public Const EQUIPMENT_TYPE_NAME As String = "機種："

    '券種
    Public Const TICKET_KIND As String = "券種"

    '種別：
    Public Const CLASS_TYPE As String = "種別："

    'データ取得日時
    Public Const DATA_GET_TIME As String = "データ取得日時："

End Class
