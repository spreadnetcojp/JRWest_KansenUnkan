' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2013/11/11  (NES)金沢  フェーズ２権限対応
'   0.2      2014/06/10  (NES)中原  北陸対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'NOTE: 操作ログ以外のログは、ここではなくLog.Xxxxメソッドの呼び出し箇所に、文字列リテラルを直接記述すること。

    'NOTE: オリジナル文言では、出力できる情報は全て出力するようにし、文言使用箇所もそれに合わせて引数を渡すようにする。
    '事業者ごとの仕様で不要な情報があれば、当該事業者用のINIファイルに、その情報を間引いた文言を定義する。

    'メッセージボックス文言
    Public Shared EnvVarNotFound As New Sentence("環境変数{0}が設定されていません。", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("設定ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared SweepOperationLogsFailed As New Sentence("古い操作ログを削除できませんでした。", SentenceAttr.Warning)
    Public Shared SweepLogsFailed As New Sentence("古いログを削除できませんでした。", SentenceAttr.Warning)

    Public Shared DatabaseOpenErrorOccurred As New Sentence("DB接続に失敗しました。", SentenceAttr.Error)
    Public Shared NoRecordsFound As New Sentence("検索条件に一致するデータは存在しません。", SentenceAttr.Information)
    Public Shared HugeRecordsFound As New Sentence("検索結果が{0}件を超えています。\n条件を絞り込んでください。", SentenceAttr.Warning)
    Public Shared DatabaseSearchErrorOccurred As New Sentence("検索処理に失敗しました。", SentenceAttr.Error)

    Public Shared SheetProcAbnormalEnd As New Sentence("一覧表示処理に失敗しました。", SentenceAttr.Error)
    Public Shared FormProcAbnormalEnd As New Sentence("画面表示処理に失敗しました。", SentenceAttr.Error)
    Public Shared ComboBoxSetupFailed As New Sentence("画面表示処理に失敗しました。\n【{0}設定失敗】", SentenceAttr.Error)

    Public Shared LedgerTemplateNotFound As New Sentence("予期せぬエラーが発生しました。\n環境設定エラー発生。", SentenceAttr.Error)
    Public Shared ReallyPrinting As New Sentence("データの出力に時間がかかりますが\nよろしいですか？", SentenceAttr.Question)
    Public Shared PrintingErrorOccurred As New Sentence("出力処理に失敗しました。", SentenceAttr.Error)

    Public Shared LoginFailed As New Sentence("ログイン処理に失敗しました。", SentenceAttr.Error)
    Public Shared LoginFailedBecauseTheIdCodeHasBeenLockedOut As New Sentence("IDコードがロックアウトされています。\n解除はシステム管理者に連絡してください。", SentenceAttr.Error)
    Public Shared LoginFailedBecauseTheIdCodeIsIncorrect As New Sentence("ログインされたIDコードは登録されていません。", SentenceAttr.Warning)
    Public Shared LoginFailedBecauseThePasswordIsIncorrect As New Sentence("IDコードとパスワードが一致しません。\n入力し直してください。", SentenceAttr.Warning)

    Public Shared InputParameterIsIncomplete As New Sentence("{0}が入力されていません。", SentenceAttr.Warning)
    Public Shared CompetitiveOperationDetected As New Sentence("他のユーザーにより該当データが更新されましたので、\n再検索してください。", SentenceAttr.Warning)
    Public Shared ReallyInsert As New Sentence("登録してもよろしいですか？", SentenceAttr.Question)
    Public Shared InsertCompleted As New Sentence("登録処理が正常に終了しました。", SentenceAttr.Information)
    Public Shared InsertFailed As New Sentence("登録処理に失敗しました。", SentenceAttr.Error)
    Public Shared ReallyUpdate As New Sentence("更新してもよろしいですか？", SentenceAttr.Question)
    Public Shared UpdateCompleted As New Sentence("更新処理が正常に終了しました。", SentenceAttr.Information)
    Public Shared UpdateFailed As New Sentence("更新処理に失敗しました。", SentenceAttr.Error)
    Public Shared ReallyDelete As New Sentence("削除してもよろしいですか？", SentenceAttr.Question)
    Public Shared DeleteCompleted As New Sentence("削除処理が正常に終了しました。", SentenceAttr.Information)
    Public Shared DeleteFailed As New Sentence("削除処理に失敗しました。", SentenceAttr.Error)

    Public Shared NoIdCodeExists As New Sentence("IDマスタ情報が登録されていません。", SentenceAttr.Warning)
    Public Shared TheIdCodeAlreadyExists As New Sentence("IDコード{0}は既に登録されています。", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForIdCode As New Sentence("IDコードは8桁の英数字で入力してください。", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForPassword As New Sentence("パスワードは4桁～8桁の英数字で入力してください。", SentenceAttr.Warning)
    Public Shared ThePasswordsDifferFromOneAnother As New Sentence("パスワードとパスワード確認が不一致です。", SentenceAttr.Warning)
    Public Shared ReallyDeleteTheIdCode As New Sentence("削除してもよろしいですか？\nIDコード{0}", SentenceAttr.Question)

    Public Shared ThePatternNoAlreadyExists As New Sentence("パターンNo{0}は既に登録されています。", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForPatternNo As New Sentence("パターンNoは2桁の数字で入力してください。", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForPatternName As New Sentence("入力値が不正です。", SentenceAttr.Warning)
    Public Shared PatternNoIsFull As New Sentence("機種単位で登録できるパターン件数を超えています。\n99件以内で登録してください。", SentenceAttr.Warning)

    Public Shared TheAreaNoAlreadyExists As New Sentence("エリアNo{0}は既に登録されています。", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForAreaNo As New Sentence("エリアNoは2桁の数字で入力してください。", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForAreaName As New Sentence("入力値が不正です。", SentenceAttr.Warning)
    Public Shared AreaNoIsFull As New Sentence("機種単位で登録できるエリア件数を超えています。\n10件以内で登録してください。", SentenceAttr.Warning)

    Public Shared MachineMasterFormatFileNotFound As New Sentence("書式定義ファイルが存在しません。\n設定を確認してください。", SentenceAttr.Error)
    Public Shared TheFileNameIsUnsuitableForMachineMaster As New Sentence("読込対象ファイルが不正です。", SentenceAttr.Error)
    Public Shared MachineMasterFileNotFound As New Sentence("読込対象ファイルが存在しません。", SentenceAttr.Error)
    Public Shared MachineMasterFileReadFailed As New Sentence("読込処理に失敗しました。\nログを確認してください。", SentenceAttr.Error)
    Public Shared MachineMasterInsertFailed As New Sentence("登録処理に失敗しました。\n設定ファイルを確認してください。", SentenceAttr.Error)
    Public Shared MachineMasterInsertFailed2 As New Sentence("登録処理に失敗しました。\nログを確認してください。", SentenceAttr.Error)

    '-------Ver0.1　フェーズ２権限対応 ADD START-----------
    Public Shared IdMstFormatFileNotFound As New Sentence("書式定義ファイルが存在しません。\n設定を確認してください。", SentenceAttr.Error)
    Public Shared TheFileNameIsUnsuitableForIdMst As New Sentence("読込対象ファイルが不正です。", SentenceAttr.Error)
    Public Shared IdMstFileNotFound As New Sentence("読込対象ファイルが存在しません。", SentenceAttr.Error)
    Public Shared IdMstFileReadFailed As New Sentence("読込処理に失敗しました。\n別のプロセスで使用されているためアクセスできません。", SentenceAttr.Error)
    Public Shared IdMstInsertFailed As New Sentence("登録処理に失敗しました。", SentenceAttr.Error)
    Public Shared IdMstImport As New Sentence("データの取込みに失敗しました。", SentenceAttr.Error)
    Public Shared IdMstExport As New Sentence("データの保存に失敗しました。", SentenceAttr.Error)
    Public Shared IdMstImportlog As New Sentence("ログ出力に失敗しました。", SentenceAttr.Error)
    '-------Ver0.1　フェーズ２権限対応 ADD  END-------------
    Public Shared TheInputValueIsUnsuitableForFaultDataErrorCode As New Sentence("エラーコードの入力に誤りがあります。", SentenceAttr.Warning)

    Public Shared TheFileTypeIsInvalid As New Sentence("選択されたファイルは{0}ではありません。", SentenceAttr.Warning)
    Public Shared ThePatternNoDoesNotExist As New Sentence("パターンNoが登録されていません。", SentenceAttr.Warning)
    Public Shared TheAreaNoDoesNotExist As New Sentence("エリアNoが登録されていません。", SentenceAttr.Warning)
    Public Shared ConnectFailed As New Sentence("接続処理に失敗しました。", SentenceAttr.Error)

    '-------Ver0.2　北陸対応　ADD START-----------
    Public Shared ThePatternNoDoesNotRelated As New Sentence("マスタに関連するパターンNoではありません。", SentenceAttr.Warning)
    Public Shared ApplicationListExcludedStationIncluded As New Sentence("適用リストに対象外の駅が含まれています。", SentenceAttr.Warning)
    '-------Ver0.2　北陸対応　ADD END-----------

    Public Shared ReallyUllMasProFile As New Sentence("登録してもよろしいですか？", SentenceAttr.Question)
    Public Shared UllMasProFileCompleted As New Sentence("登録が完了しました。", SentenceAttr.Information)
    Public Shared UllMasProFileFailed As New Sentence("登録で異常が発生しました。", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByBusy As New Sentence("他の端末で操作中のため、登録できませんでした。", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByInvalidContent As New Sentence("異常なファイルのため、登録できませんでした。", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByUnknownLight As New Sentence("何らかの原因で、登録できませんでした。", SentenceAttr.Error)
    Public Shared ReallyExitWithoutUll As New Sentence("データが登録されていません。\n終了してもよろしいですか？", SentenceAttr.Question)

    Public Shared ReallyInvokeMasProDll As New Sentence("配信を開始してもよろしいですか？", SentenceAttr.Question)
    Public Shared InvokeMasProDllCompleted As New Sentence("配信を開始しました。", SentenceAttr.Information)
    Public Shared InvokeMasProDllFailed As New Sentence("配信の開始で異常が発生しました。", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByBusy As New Sentence("他の端末で操作中のため、配信を開始できませんでした。", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByNoData As New Sentence("配信対象データが登録されていないため、配信を開始できませんでした。", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByUnnecessary As New Sentence("適用リストから新たな配信先がみつかりませんでした。", SentenceAttr.Warning)
    Public Shared InvokeMasProDllFailedByInvalidContent As New Sentence("配信対象データが異常なため、配信を開始できませんでした。", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByUnknownLight As New Sentence("何らかの原因で、配信を開始できませんでした。", SentenceAttr.Error)

    Public Shared KensyuRangeIsInvalid As New Sentence("検修の指定が誤っています。", SentenceAttr.Warning)
    Public Shared DateRangeIsInvalid As New Sentence("日付の指定が誤っています。", SentenceAttr.Warning)
    Public Shared PrintEndItClearDate As New Sentence("出力が終了しました。\nクリア日付が跨った機体がありました。", SentenceAttr.Information)
    Public Shared PrintEndItMachineChange As New Sentence("出力が終了しました。\n機体の移設がありました。", SentenceAttr.Information)
    Public Shared PrintEndItDateReverse As New Sentence("出力が終了しました。\n開始条件と終了条件の日付の関係が逆の機体がありました。", SentenceAttr.Information)


    '帳票の文言
    Public Shared PassageInfo As New Sentence("通路方向：{0}")
    Public Shared TimeSpan As New Sentence("{0} {1}　から　{2} {3}　まで")

    '操作ログの文言
    Public Shared WindowSuffix As New Sentence("画面")
    Public Shared DialogSuffix As New Sentence("ダイアログ")
    Public Shared SheetCellDoubleClicked As New Sentence("{0}にて{1}の{2}行{3}列をダブルクリックしました。行内容:[{4}]")
    Public Shared DateTimePickerValueChanged As New Sentence("{0}にて{1}を{2}に変更しました。")
    Public Shared ComboBoxSelectionChanged As New Sentence("{0}にて{1}を{2}に変更しました。")
    Public Shared ComboBoxSelectionChangedToNothing As New Sentence("{0}にて{1}を未選択に変更しました。")
    Public Shared ButtonClicked As New Sentence("{0}にて{1}をクリックしました。")
    Public Shared SomeControlInvoked As New Sentence("{0}にて{1}({2}型)を操作しました。")
    Public Shared YesButtonClicked As New Sentence("はいボタン押下。")
    Public Shared NoButtonClicked As New Sentence("いいえボタン押下。")
    Public Shared OkButtonClicked As New Sentence("OKボタン押下。")

    ''' <summary>INIファイルの内容を取り込む。</summary>
    ''' <remarks>
    ''' INIファイルの内容を取り込む。
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
