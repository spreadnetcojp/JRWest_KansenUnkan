' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/03/01  (NES)河脇  新規作成
'   0.2      2014/06/12  (NES)田保  北陸対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    Public Shared EnvVarNotFound As New Sentence("環境変数{0}が設定されていません。", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("設定ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared SweepLogsFailed As New Sentence("古いログを削除できませんでした。", SentenceAttr.Warning)

    Public Shared ERR_COMMON As New Sentence("{0}の取得に失敗しました。", SentenceAttr.Error)
    Public Shared ERR_FILE_READ As New Sentence("ファイルの読み込みに失敗しました。", SentenceAttr.Error)
    Public Shared ERR_FILE_WRITE As New Sentence("ファイルの書き込みに失敗しました。", SentenceAttr.Error)
    Public Shared ERR_UNKNOWN As New Sentence("その他異常が発生しました。", SentenceAttr.Error)
    Public Shared ERR_FILE_CSV As New Sentence("テキストファイルを指定してください。", SentenceAttr.Error)

    Public Shared Confirm As New Sentence("処理しますか。", SentenceAttr.Question)
    Public Shared Finished As New Sentence("処理しました。", SentenceAttr.Question)

    Public Shared TheInputValueIsUnsuitableForMasterVersion As New Sentence("バージョンを正しく入力してください。", SentenceAttr.Warning)

    'Ver0.2 ADD START  北陸対応
    Public Shared ThePatternNoDoesNotRelated As New Sentence("マスタに関連するパターンNoではありません。", SentenceAttr.Warning)
    Public Shared FileTypeNG1 As New Sentence("ＣＳＶファイルを指定してください。", SentenceAttr.Warning)
    Public Shared FileTypeNG2 As New Sentence("ＣＳＶファイルは指定できません。", SentenceAttr.Warning)
    Public Shared FileTypeNG3 As New Sentence("既に変換済みのファイルです。", SentenceAttr.Warning)
    'Ver0.2 ADD END    北陸対応

    ''' <summary>INIファイルの内容を取り込む。</summary>
    ''' <remarks>
    ''' INIファイルの内容を取り込む。
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
