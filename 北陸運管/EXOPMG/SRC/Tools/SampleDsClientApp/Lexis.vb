' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/05/13  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'NOTE: 操作ログ以外のログは、ここではなくLog.Xxxxメソッドの呼び出し箇所に、文字列リテラルを直接記述する方針。

    'NOTE: オリジナル文言では、出力できる情報は全て出力するようにし、文言使用箇所もそれに合わせて引数を渡すようにする。
    '事業者ごとの仕様で不要な情報があれば、当該事業者用のINIファイルに、その情報を間引いた文言を定義する。

    'ウィンドウタイトル
    Public Shared FormTitle As New Sentence("デ集クライアント")

    'メッセージボックス文言
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("設定ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)

    Public Shared TheFileTypeIsInvalid As New Sentence("選択されたファイルは{0}ではありません。", SentenceAttr.Warning)
    Public Shared ThePatternNoDoesNotExist As New Sentence("パターンNoが登録されていません。", SentenceAttr.Warning)
    Public Shared ConnectFailed As New Sentence("接続処理に失敗しました。", SentenceAttr.Error)

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

    '操作ログの文言
    Public Shared WindowSuffix As New Sentence("画面")
    Public Shared DialogSuffix As New Sentence("ダイアログ")
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
