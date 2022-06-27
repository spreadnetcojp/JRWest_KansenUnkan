' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'ウィンドウタイトル
    Public Shared FormTitle As New Sentence("駅務機器 {0}")
    Public Shared FormTitleEkCodeFormat As New Sentence("%2M-%3R-%3S-%4C-%2U")

    'メッセージボックス文言
    Public Shared DoNotExecInSameWorkingDir As New Sentence("同一の作業フォルダで複数起動しないでください。", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("設定ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared UiStateDeserializeFailed As New Sentence("状態ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared UiStateSerializeFailed As New Sentence("状態ファイルの書き込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared ConnectFailed As New Sentence("接続できませんでした。", SentenceAttr.Error)
    Public Shared UnwelcomeExceptionCaught As New Sentence("プログラムに異常を検出しました。", SentenceAttr.Error)
    Public Shared TheInputValueIsUnsuitableForObjCode As New Sentence("データ種別に2桁の16進数を入力してください。", SentenceAttr.Warning)
    Public Shared TheInputValueIsDuplicative As New Sentence("キーが同一の行が存在します。", SentenceAttr.Warning)
    Public Shared ScenarioFileIsIllegal As New Sentence("シナリオファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared ScenarioFileIsEmpty As New Sentence("シナリオファイルが空です。", SentenceAttr.Error)
    Public Shared DoNotRepeatScenarioThatContainsAbsoluteTiming As New Sentence("絶対日時を用いたシナリオを繰り返し実行することはできません。", SentenceAttr.Error)

    ''' <summary>INIファイルの内容を取り込む。</summary>
    ''' <remarks>
    ''' INIファイルの内容を取り込む。
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
