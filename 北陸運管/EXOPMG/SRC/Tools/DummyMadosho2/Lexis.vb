' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/06/27  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'ウィンドウタイトル
    Public Shared FormTitle As New Sentence("多重窓口処理機向け 利用データサーバ")
    Public Shared RiyoDataFormTitle As New Sentence("{1} {2} {0} {3:D}号機 利用データ編集")

    'メッセージボックス文言
    Public Shared MultipleInstanceNotAllowed As New Sentence("複数起動しないでください。", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("設定ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared UiStateDeserializeFailed As New Sentence("状態ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared UiStateSerializeFailed As New Sentence("状態ファイルの書き込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared UnwelcomeExceptionCaught As New Sentence("プログラムに異常を検出しました。", SentenceAttr.Error)
    Public Shared MessageQueueServiceNotAvailable As New Sentence("メッセージキューサービスが使用できません。", SentenceAttr.Error)
    Public Shared MessageQueueDeleteFailed As New Sentence("メッセージキューの削除に失敗しました。\n不要であれば手動で削除してください。", SentenceAttr.Error)
    Public Shared InvalidDirectorySpecified As New Sentence("無効なディレクトリが指定されました。", SentenceAttr.Error)
    Public Shared MachineProfileFetchFinished As New Sentence("シミュレータ本体の全機器を走査しました。\n無くなった機器や移動した機器の情報はそのまま残していますので、不要であれば手動で削除してください。", SentenceAttr.Information)
    Public Shared SearchWordNotFound As New Sentence("マッチするセルは１つもありません。", SentenceAttr.Information)
    Public Shared RiyoDataFileCreateReally As New Sentence("ファイルを新規作成します。", SentenceAttr.Information)
    Public Shared RiyoDataFileSizeError As New Sentence("ファイルサイズが異常です。", SentenceAttr.Error)
    Public Shared RiyoDataFileReadError As New Sentence("ファイル読み込みで異常が発生しました。\n{0}", SentenceAttr.Error)
    Public Shared RiyoDataFileWriteError As New Sentence("ファイル書き込みで異常が発生しました。\n{0}", SentenceAttr.Error)
    Public Shared RiyoDataFileExclusionError As New Sentence("ファイルに対する変更を検出しました。\n書き込みは中止します。", SentenceAttr.Error)
    Public Shared RiyoDataStoreFailed As New Sentence("蓄積できませんでした。", SentenceAttr.Error)
    Public Shared RiyoDataStoreFinished As New Sentence("蓄積しました。", SentenceAttr.Information)
    Public Shared RiyoDataSendFailed As New Sentence("シミュレータ本体へ要求できませんでした。", SentenceAttr.Error)
    Public Shared RiyoDataSendFinished As New Sentence("シミュレータ本体へ要求しました。\nシミュレータ本体のログを確認してください。", SentenceAttr.Information)
    Public Shared RiyoDataBaseHeaderSetReally As New Sentence("機器IDや現在日時をもとに基本ヘッダーを再設定します。\nよろしいですか？", SentenceAttr.Question)
    Public Shared RiyoDataMinDateReplaceReally As New Sentence("ゼロ以外が設定されている開始日や発行日を\n下記で置き換えます。\nよろしいですか？")
    Public Shared RiyoDataMaxDateReplaceReally As New Sentence("ゼロ以外が設定されている終了日を下記で置き換えます。\nよろしいですか？")
    Public Shared RiyoDataEntDateReplaceReally As New Sentence("ゼロ以外が設定されている入場日時や基本ヘッダーの日時を\n下記で置き換えます。\nよろしいですか？")
    Public Shared RiyoDataOrgStaReplaceReally As New Sentence("幹線の駅コードが設定されている発駅を下記で置き換えます。\nよろしいですか？")
    Public Shared RiyoDataDstStaReplaceReally As New Sentence("幹線の駅コードが設定されている着駅を下記で置き換えます。\nよろしいですか？")
    Public Shared RiyoDataEntStaReplaceReally As New Sentence("幹線の駅コードが設定されている入場駅や基本ヘッダーの駅を\n下記で置き換えます。\nよろしいですか？")
    Public Shared SelectRecordToRead As New Sentence("{0:D}レコードが存在します。\n読み込むレコードを選択してください。")
    Public Shared SelectRecordToWrite As New Sentence("{0:D}レコードが存在します。\n上書きするレコードを選択してください。")
    Public Shared LogDispFilterIsInvalid As New Sentence("フィルタが不正です。再編集してください。", SentenceAttr.Error)

    'ログ表示グリッドの列ヘッダ文言
    Public Shared LogDispTimeColumnTitle As New Sentence("Time")
    Public Shared LogDispSourceColumnTitle As New Sentence("Source")
    Public Shared LogDispMessageColumnTitle As New Sentence("Message")

    'その他
    Public Shared EmptyTime As New Sentence("")
    Public Shared UnknownTime As New Sentence("(不明)")

    ''' <summary>INIファイルの内容を取り込む。</summary>
    ''' <remarks>
    ''' INIファイルの内容を取り込む。
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
