' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/08/08  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'ウィンドウタイトル
    Public Shared FormTitle As New Sentence("多重統括向け 運用データサーバ")

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
    Public Shared TermMachineRowNotSelected As New Sentence("窓口処理機の行を１つ以上選択している状態で実行してください。", SentenceAttr.Error)
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
