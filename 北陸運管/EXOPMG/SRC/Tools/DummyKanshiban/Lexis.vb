' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/02/16  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'ウィンドウタイトル
    Public Shared FormTitle As New Sentence("多重監視盤向け 運用・保守データサーバ")
    Public Shared FaultDataFormTitle As New Sentence("{1} {2} {0} {3:D}号機 異常データ編集")
    Public Shared KadoDataFormTitle As New Sentence("{1} {2} {0} {3:D}号機 稼動保守データ編集")

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
    Public Shared FaultDataFileForActiveOneRewriteReally As New Sentence("即時送信形式の既存ファイルを上書きします。", SentenceAttr.Information)
    Public Shared FaultDataFileFormatSelectorDescription As New Sentence("ファイルを新規作成します。形式を選択してください。")
    Public Shared FaultDataFileFormatSelectorFormat0Text As New Sentence("即時送信形式")
    Public Shared FaultDataFileFormatSelectorFormat1Text As New Sentence("再収集形式")
    Public Shared FaultDataFileForPassiveUllAppendReally As New Sentence("再収集形式のファイルに追記を行います。", SentenceAttr.Information)
    Public Shared FaultDataFileForActiveOneAppendError As New Sentence("即時送信形式のファイルに追記はできません。", SentenceAttr.Error)
    Public Shared FaultDataFileSizeError As New Sentence("ファイルサイズが異常です。", SentenceAttr.Error)
    Public Shared FaultDataFileReadError As New Sentence("ファイル読み込みで異常が発生しました。\n{0}", SentenceAttr.Error)
    Public Shared FaultDataFileWriteError As New Sentence("ファイル書き込みで異常が発生しました。\n{0}", SentenceAttr.Error)
    Public Shared FaultDataFileExclusionError As New Sentence("ファイルに対する変更を検出しました。\n書き込みは中止します。", SentenceAttr.Error)
    Public Shared FaultDataStoreFailed As New Sentence("蓄積できませんでした。", SentenceAttr.Error)
    Public Shared FaultDataStoreFinished As New Sentence("蓄積しました。", SentenceAttr.Information)
    Public Shared FaultDataSendFailed As New Sentence("シミュレータ本体へ要求できませんでした。", SentenceAttr.Error)
    Public Shared FaultDataSendFinished As New Sentence("シミュレータ本体へ要求しました。\nシミュレータ本体のログを確認してください。", SentenceAttr.Information)
    Public Shared FaultDataBaseHeaderSetReally As New Sentence("機器IDや現在日時をもとに基本ヘッダーを再設定します。\nよろしいですか？", SentenceAttr.Question)
    Public Shared FaultDataAllHeadersSetReally As New Sentence("機器IDや現在日時をもとに通路方向までの全項目を再設定します。\nよろしいですか？", SentenceAttr.Question)
    Public Shared FaultDataErrorTextsSetReally As New Sentence("エラーコードをもとに各項目の「表示データ」と「有効バイト数」を設定します。\nよろしいですか？", SentenceAttr.Question)
    Public Shared FaultDataByteCountsSetReally As New Sentence("各項目について「表示データ」をもとに「有効バイト数」を設定します。\nよろしいですか？", SentenceAttr.Question)
    Public Shared FaultDataErrorTextsSetFailed As New Sentence("「表示データ」の設定で異常が発生しました。\n{0}", SentenceAttr.Error)
    'Public Shared FaultDataErrorTextsNotFound As New Sentence("エラーコードに紐づく文言がみつかりませんでした。", SentenceAttr.Error)
    Public Shared KadoDataManagementFileIsBroken As New Sentence("機器の稼動保守データのファイルサイズが異常です。\n代替措置として、機器IDや現在日時をもとにした初期値を表示します。", SentenceAttr.Warning)
    Public Shared KadoDataManagementFileReadError As New Sentence("機器の稼動保守データのファイル読み込みで異常が発生しました。\n代替措置として、機器IDや現在日時をもとにした初期値を表示します。", SentenceAttr.Warning)
    Public Shared KadoDataManagementFileIsLocked As New Sentence("機器の稼動保守データのファイルが他のプロセスにより使用中です。\n読み込みを再試行しますか？", SentenceAttr.Question)
    Public Shared KadoDataFileRewriteReally As New Sentence("既存ファイルを上書きします。", SentenceAttr.Information)
    Public Shared KadoDataFileCreateReally As New Sentence("ファイルを新規作成します。", SentenceAttr.Information)
    Public Shared KadoDataFileAppendReally As New Sentence("ファイルに追記を行います。", SentenceAttr.Information)
    Public Shared KadoDataFileSizeError As New Sentence("ファイルサイズが異常です。", SentenceAttr.Error)
    Public Shared KadoDataFileReadError As New Sentence("ファイル読み込みで異常が発生しました。\n{0}", SentenceAttr.Error)
    Public Shared KadoDataFileWriteError As New Sentence("ファイル書き込みで異常が発生しました。\n{0}", SentenceAttr.Error)
    Public Shared KadoDataFileExclusionError As New Sentence("ファイルに対する変更を検出しました。\n書き込みは中止します。", SentenceAttr.Error)
    Public Shared KadoDataStoreFailed As New Sentence("反映できませんでした。", SentenceAttr.Error)
    Public Shared KadoDataStoreFinished As New Sentence("反映しました。", SentenceAttr.Information)
    Public Shared KadoDataBaseHeaderSetReally As New Sentence("機器IDや現在日時をもとに基本ヘッダーを再設定します。\nよろしいですか？", SentenceAttr.Question)
    Public Shared KadoDataAllHeadersSetReally As New Sentence("機器IDや現在日時をもとに基本ヘッダーと共通部を再設定します。\nよろしいですか？", SentenceAttr.Question)
    Public Shared KadoDataSummariesSetReally As New Sentence("各種合計項目を算出して設定します。\nよろしいですか？", SentenceAttr.Question)
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
