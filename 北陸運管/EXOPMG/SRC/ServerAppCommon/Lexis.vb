' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2013/12/10  (NES)小林  統括状態情報の追加対応
'   0.2      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'メッセージボックスに表示する文言
    Public Shared EnvVarNotFound As New Sentence("環境変数[{0}]が設定されていません。", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("設定ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnInitializingProcess As New Sentence("プロセスの初期化で異常が発生しました。", SentenceAttr.Error)

    '収集データ誤記テーブルに登録する異常内容文言の部品
    Public Shared CdtKanshiban As New Sentence("監視盤")
    Public Shared CdtGate As New Sentence("改札機")
    Public Shared CdtTokatsu As New Sentence("明収／ＥＸ統括")
    Public Shared CdtMadosho As New Sentence("窓口処理機")
    Public Shared CdtGeneralDataPort As New Sentence("通常データ用ポート")
    Public Shared CdtRiyoDataPort As New Sentence("利用データ用ポート")

    '収集データ誤記テーブルに登録する異常内容文言
    Public Shared CdtProcessAbended As New Sentence("プロセスが異常終了しました。({0})")
    Public Shared CdtThreadAbended As New Sentence("スレッドが異常終了しました。({0}-{1})")
    Public Shared CdtLineError As New Sentence("{0}と{2}で通信が行えません。(号機:{1})")
    Public Shared CdtNkanLineError As New Sentence("Ｎ間サーバと通信が行えません。")
    Public Shared CdtMachineMasterErratumDetected As New Sentence("機器構成マスタに異常を検出しました。")
    Public Shared CdtScheduledUllFailed As New Sentence("{0}からの収集に失敗しました。(号機:{1})")
    Public Shared CdtReadingTotallyFailed As New Sentence("全体の解析が失敗しました。({0}, {1})")
    Public Shared CdtReadingPartiallyFailed As New Sentence("一部の解析が失敗しました。({0}, {1})")
    Public Shared CdtRecordingFailed As New Sentence("データの登録に失敗しました。")
    Public Shared CdtTheUnitNotFound As New Sentence("機器が存在しません。(線区:{0} 駅順:{1} コーナ:{2} 号機:{3})")
    Public Shared CdtTheCornerNotFound As New Sentence("機器が存在しません。(線区:{0} 駅順:{1} コーナ:{2})")
    Public Shared CdtUnpairedKadoDataDetected As New Sentence("稼動・保守データの一部が欠落しています。(号機:{0})")

    '-------Ver0.2 次世代車補対応 MOD START-----------
    'NOTE; 次世代車補対応で、メール文言の号機番号はIntegerで入力する（差し替え時に書式を指定可能とする）ように統一。
    '異常データ通知メールの文言
    Public Shared FaultDataMailSubject As New Sentence("{0} {1} {2} {3:D}号機で異常データが発生しました")
    Public Shared FaultDataMailBody As New Sentence("{0} {1}\n{2}")
    Public Shared DateTimeFormatInFaultDataMailBody As New Sentence("yyyy/MM/dd HH:mm:ss")
    '-------Ver0.2 次世代車補対応 MOD END-------------

    '-------Ver0.2 次世代車補対応 ADD START-----------
    '通信異常の警報メールの文言
    Public Shared KanshibanLineErrorAlertMailSubject As New Sentence("新幹線運管サーバで {0} {1} 監視盤 {2:D}号機との通常データ用ポート通信異常が発生しています。")
    Public Shared Kanshiban2LineErrorAlertMailSubject As New Sentence("新幹線運管サーバで {0} {1} 監視盤 {2:D}号機との利用データ用ポート通信異常が発生しています。")
    Public Shared TokatsuLineErrorAlertMailSubject As New Sentence("新幹線運管サーバで {0} {1} 統括 {2:D}号機との通常データ用ポート通信異常が発生しています。")  'NOTE: コーナー名は差し替えで除去する。
    Public Shared MadoshoLineErrorAlertMailSubject As New Sentence("新幹線運管サーバで {0} {1} 窓処 {2:D}号機との通常データ用ポート通信異常が発生しています。")
    Public Shared Madosho2LineErrorAlertMailSubject As New Sentence("新幹線運管サーバで {0} {1} 窓処 {2:D}号機との利用データ用ポート通信異常が発生しています。")
    Public Shared KanshibanLineErrorAlertMailBody As New Sentence("{3} より\n新幹線運管サーバで {0} {1} 監視盤 {2:D}号機との通常データ用ポート通信異常が発生しています。")
    Public Shared Kanshiban2LineErrorAlertMailBody As New Sentence("{3} より\n新幹線運管サーバで {0} {1} 監視盤 {2:D}号機との利用データ用ポート通信異常が発生しています。")
    Public Shared TokatsuLineErrorAlertMailBody As New Sentence("{3} より\n新幹線運管サーバで {0} {1} 統括 {2:D}号機との通常データ用ポート通信異常が発生しています。")  'NOTE: コーナー名は差し替えで除去する。
    Public Shared MadoshoLineErrorAlertMailBody As New Sentence("{3} より\n新幹線運管サーバで {0} {1} 窓処 {2:D}号機との通常データ用ポート通信異常が発生しています。")
    Public Shared Madosho2LineErrorAlertMailBody As New Sentence("{3} より\n新幹線運管サーバで {0} {1} 窓処 {2:D}号機との利用データ用ポート通信異常が発生しています。")
    Public Shared DateTimeFormatInLineErrorAlertMailBody As New Sentence("yyyy/MM/dd HH:mm:ss")
    '-------Ver0.2 次世代車補対応 ADD END-------------

    '-------Ver0.2 次世代車補対応 MOD START-----------
    'NOTE; 次世代車補対応で、メール文言の号機番号はIntegerで入力する（差し替え時に書式を指定可能とする）ように統一。
    '機器接続状態メールの文言
    Public Shared ConStatusMailSubject As New Sentence("{0} 新幹線運管サーバ定時報告")
    Public Shared DateTimeFormatInConStatusMailSubject As New Sentence("yyyy/MM/dd HH:mm")
    Public Shared GatePartTitleInConStatusMailBody As New Sentence("【改札機状態情報】")
    Public Shared KsbLabelInConStatusMailBody As New Sentence("{0} {1} 監視盤 {2:D}号機: ")
    Public Shared KsbOpmgErrorInConStatusMailBody As New Sentence("運管×")
    Public Shared GateLabelInConStatusMailBody As New Sentence("{0} {1} 改札機 {2:D}号機: ")
    Public Shared GatePowerErrorInConStatusMailBody As New Sentence("電源×")
    Public Shared GateMainKsbErrorInConStatusMailBody As New Sentence("監視盤×")
    Public Shared GateMainIcuErrorInConStatusMailBody As New Sentence("主制御×")
    Public Shared GateMainDsvErrorInConStatusMailBody As New Sentence("配SV(主)×")
    Public Shared GateIcuDsvErrorInConStatusMailBody As New Sentence("配SV(IC)×")
    Public Shared GateIcuTktErrorInConStatusMailBody As New Sentence("統括×")
    'Ver0.1 ADD 統括状態情報の追加対応
    Public Shared TktPartTitleInConStatusMailBody As New Sentence("【統括状態情報】")
    Public Shared MadoPartTitleInConStatusMailBody As New Sentence("【窓処状態情報】")
    Public Shared TktLabelInConStatusMailBody As New Sentence("{0} {1} 統括 {2:D}号機: ")  'NOTE: コーナー名は差し替えで除去する。
    Public Shared TktOpmgErrorInConStatusMailBody As New Sentence("運管×")
    'Ver0.1 ADD 統括状態情報の追加対応
    Public Shared TktIdcErrorInConStatusMailBody As New Sentence("センター×")
    Public Shared MadoLabelInConStatusMailBody As New Sentence("{0} {1} 窓処 {2:D}号機: ")
    Public Shared MadoTktIdErrorInConStatusMailBody As New Sentence("統括(ID)×")
    Public Shared MadoTktDlErrorInConStatusMailBody As New Sentence("統括(DL)×")
    Public Shared MadoKsbErrorInConStatusMailBody As New Sentence("監視盤×")
    Public Shared MadoDsvErrorInConStatusMailBody As New Sentence("配SV×")
    Public Shared ErrorSeparatorInConStatusMailBody As New Sentence(" ")
    '-------Ver0.2 次世代車補対応 MOD END-------------

    ''' <summary>INIファイルの内容を取り込む。</summary>
    ''' <remarks>
    ''' INIファイルの内容を取り込む。
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
