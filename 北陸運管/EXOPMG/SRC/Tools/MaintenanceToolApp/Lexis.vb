' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2014/04/20  (NES)      新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    Public Shared DoNotExecMultipleInstance As New Sentence("二重起動は出来ません。", SentenceAttr.Error)
    Public Shared SheetProcAbnormalEnd As New Sentence("一覧表示処理に失敗しました。", SentenceAttr.Error)
    Public Shared FormProcAbnormalEnd As New Sentence("画面表示処理に失敗しました。", SentenceAttr.Error)

    Public Shared EnvVarNotFound As New Sentence("環境変数{0}が設定されていません。", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("設定ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared SweepLogsFailed As New Sentence("古いログを削除できませんでした。", SentenceAttr.Warning)

    Public Shared DatabaseOpenErrorOccurred As New Sentence("DB接続に失敗しました。", SentenceAttr.Error)
    Public Shared DatabaseSearchErrorOccurred As New Sentence("検索処理に失敗しました。", SentenceAttr.Error)

    Public Shared ERR_COMMON As New Sentence("{0}の取得に失敗しました。", SentenceAttr.Error)
    Public Shared ERR_FILE_READ As New Sentence("ファイルの読み込みに失敗しました。", SentenceAttr.Error)
    Public Shared ERR_FILE_WRITE As New Sentence("ファイルの書き込みに失敗しました。", SentenceAttr.Error)
    'Public Shared ERR_FILE_CSV As New Sentence("テキストファイルを指定してください。", SentenceAttr.Error)

    Public Shared ReallyUpdate As New Sentence("更新してもよろしいですか？", SentenceAttr.Question)
    Public Shared UpdateCompleted As New Sentence("更新処理が正常に終了しました。", SentenceAttr.Information)
    Public Shared UpdateFailed As New Sentence("更新処理に失敗しました。", SentenceAttr.Error)

    Public Shared ReallyImport As New Sentence("{0}ファイルの内容で更新してもよろしいですか？", SentenceAttr.Question)
    Public Shared ReallyExport As New Sentence("{0}ファイルに保存してもよろしいですか？", SentenceAttr.Question)
    Public Shared ExportCompleted As New Sentence("保存処理が正常に終了しました。", SentenceAttr.Information)
    Public Shared DataErr1DetectedOnImport As New Sentence("登録するデータに空の項目があります。\n線区={0},駅順={1},機種={2},エラーコード={3}\n処理を中止します。", SentenceAttr.Error)
    Public Shared DataErr2DetectedOnImport As New Sentence("登録するデータに桁オーバーの項目があります。\n線区={0},駅順={1},機種={2},エラーコード={3}\n処理を中止します。", SentenceAttr.Error)
    Public Shared DataErr3DetectedOnImport As New Sentence("登録するデータに全角文字があります。\n線区={0},駅順={1},機種={2},エラーコード={3}\n処理を中止します。", SentenceAttr.Error)
    Public Shared DataErr4DetectedOnImport As New Sentence("登録するデータの線区駅順に数字以外の文字があります。\n線区={0},駅順={1},機種={2},エラーコード={3}\n処理を中止します。", SentenceAttr.Error)
    Public Shared DataErr5DetectedOnImport As New Sentence("登録するデータのエラーコードに不正な文字があります。\n線区={0},駅順={1},機種={2},エラーコード={3}\n処理を中止します。", SentenceAttr.Error)
    Public Shared DataErr6DetectedOnImport As New Sentence("登録するデータの機種コードが不正です。\n線区={0},駅順={1},機種={2},エラーコード={3}\n処理を中止します。", SentenceAttr.Error)

    ''' <summary>INIファイルの内容を取り込む。</summary>
    ''' <remarks>
    ''' INIファイルの内容を取り込む。
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
