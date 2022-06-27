' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/01/14  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'ウィンドウタイトル
    Public Shared FormTitle As New Sentence("多重駅務機器")

    'メッセージボックス文言またはデータグリッドビューのエラー文言
    Public Shared DoNotExecInSameWorkingDir As New Sentence("同一の作業フォルダで複数起動しないでください。", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("設定ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared UiStateDeserializeFailed As New Sentence("状態ファイルの読み込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared UiStateSerializeFailed As New Sentence("状態ファイルの書き込みで異常が発生しました。", SentenceAttr.Error)
    Public Shared UnwelcomeExceptionCaught As New Sentence("継続不可能な異常を検出しました。\n{0}", SentenceAttr.Error)
    Public Shared MessageQueueServiceNotAvailable As New Sentence("メッセージキューサービスが無効です。\n外部常駐プロセスとの連携は行えません。", SentenceAttr.Warning)
    Public Shared MessageQueueDeleteFailed As New Sentence("メッセージキューの削除に失敗しました。\n不要であれば手動で削除してください。", SentenceAttr.Error)
    Public Shared TheTelegrapherAborted As New Sentence("電文送受信スレッドの停止を検出しました。\n電文送受信スレッドの再起動を行います\n機器名: {0}", SentenceAttr.Error)
    Public Shared TheInputValueIsUnsuitableForObjCode As New Sentence("データ種別に2桁の16進数を入力してください。", SentenceAttr.Warning)
    Public Shared TheInputValueIsDuplicative As New Sentence("キーが同一の行が存在します。", SentenceAttr.Warning)
    Public Shared TransferNameIsInvalid As New Sentence("転送名が不正です。", SentenceAttr.Warning)
    Public Shared FilePathIsInvalid As New Sentence("ファイル名が不正です。", SentenceAttr.Warning)
    Public Shared LogDispFilterIsInvalid As New Sentence("フィルタが不正です。再編集してください。", SentenceAttr.Error)

    'ログ表示グリッドの列ヘッダ文言
    Public Shared LogDispTimeColumnTitle As New Sentence("Time")
    Public Shared LogDispSourceColumnTitle As New Sentence("Source")
    Public Shared LogDispMessageColumnTitle As New Sentence("Message")

    'ツールチップ文言
    Public Shared DataKindTipText As New Sentence("2桁の16進数を指定してください。")
    Public Shared ActiveSeqTransferNameTipText As New Sentence( _
        "FTPサイト上でのファイル名を指定してください。\n" & _
        "ヒント1: ""%""で始まる文字列は、前処理で以下のように置換されます。\n" & _
        " %桁M  : シーケンスを実行する駅務機器の機種コード\n" & _
        " %桁R  : シーケンスを実行する駅務機器の線区コード\n" & _
        " %桁S  : シーケンスを実行する駅務機器の駅順コード\n" & _
        " %桁C  : シーケンスを実行する駅務機器のコーナーコード\n" & _
        " %桁U  : シーケンスを実行する駅務機器の号機番号\n" & _
        " %桁I  : シーケンスを実行する駅務機器の項番（シミュレータ内での通し番号）\n" & _
        " %T桁R : 端末機器の線区コード\n" & _
        " %T桁S : 端末機器の駅順コード\n" & _
        " %T桁C : 端末機器のコーナーコード\n" & _
        " %T桁U : 端末機器の号機番号\n" & _
        " %T桁I : 端末機器の項番（駅務機器内での通し番号）\n" & _
        " %%   : 1文字の""%""\n" & _
        " 桁には1～9の数字を記述してください。その桁数になるようゼロ埋めが行われます。\n" & _
        " 桁を記述しない場合は、ゼロサプレスが行われます。\n" & _
        " 端末機器とは、シーケンスを実行する駅務機器の配下にある機器のことです。\n" & _
        " 端末機器のコードに置換される記号（%T～）を１つでも記述すると、\n" & _
        " シーケンスの実行は端末機器別に行われます。\n" & _
        "ヒント2: ""$[シンボル名]""や""$関数名<引数リスト>""にマッチする部分は、下記例のようにシナリオと同じ方法で評価されます。\n" & _
        " $[$] : 1文字の""$""\n" & _
        " $Trim<文字列> : トリミングした文字列\n" & _
        " ただし、$ContextNum<>, $ContextDir<>, $SetRef<...>, $SetVal<...>, $Val<...>, $ExecDynFunc<...>, $ExecCmdFunc<...>, $ExecAppFunc<...> は不正な式とみなします。")
    Public Shared ActiveSeqApplyFileTipText As New Sentence( _
        "絶対パス形式または作業フォルダからの相対パス形式で、ファイルを指定してください。\n" & _
        "ヒント1: ""%""で始まる文字列は、前処理で以下のように置換されます。\n" & _
        " %桁M  : シーケンスを実行する駅務機器の機種コード\n" & _
        " %桁R  : シーケンスを実行する駅務機器の線区コード\n" & _
        " %桁S  : シーケンスを実行する駅務機器の駅順コード\n" & _
        " %桁C  : シーケンスを実行する駅務機器のコーナーコード\n" & _
        " %桁U  : シーケンスを実行する駅務機器の号機番号\n" & _
        " %桁I  : シーケンスを実行する駅務機器の項番（シミュレータ内での通し番号）\n" & _
        " %T桁R : 端末機器の線区コード\n" & _
        " %T桁S : 端末機器の駅順コード\n" & _
        " %T桁C : 端末機器のコーナーコード\n" & _
        " %T桁U : 端末機器の号機番号\n" & _
        " %T桁I : 端末機器の項番（駅務機器内での通し番号）\n" & _
        " %%   : 1文字の""%""\n" & _
        " 桁には1～9の数字を記述してください。その桁数になるようゼロ埋めが行われます。\n" & _
        " 桁を記述しない場合は、ゼロサプレスが行われます。\n" & _
        " 端末機器とは、シーケンスを実行する駅務機器の配下にある機器のことです。\n" & _
        " 端末機器のコードに置換される記号（%T～）を１つでも記述すると、\n" & _
        " シーケンスの実行は端末機器別に行われます。\n" & _
        "ヒント2: ""$[シンボル名]""や""$関数名<引数リスト>""にマッチする部分は、下記例のようにシナリオと同じ方法で評価されます。\n" & _
        " $[$] : 1文字の""$""\n" & _
        " $MachineDir<> : シーケンスを実行する駅務機器の作業ディレクトリ（絶対パス）\n" & _
        " ただし、$ContextNum<>, $ContextDir<>, $SetRef<...>, $SetVal<...>, $Val<...>, $ExecDynFunc<...>, $ExecCmdFunc<...>, $ExecAppFunc<...> は不正な式とみなします。")
    Public Shared PassiveSeqApplyFileTipText As New Sentence( _
        "絶対パス形式または作業フォルダからの相対パス形式で、ファイルを指定してください。\n" & _
        "ヒント1: ""%""で始まる文字列は、前処理で以下のように置換されます。\n" & _
        " %桁M  : シーケンスを実行する駅務機器の機種コード\n" & _
        " %桁R  : シーケンスを実行する駅務機器の線区コード\n" & _
        " %桁S  : シーケンスを実行する駅務機器の駅順コード\n" & _
        " %桁C  : シーケンスを実行する駅務機器のコーナーコード\n" & _
        " %桁U  : シーケンスを実行する駅務機器の号機番号\n" & _
        " %桁I  : シーケンスを実行する駅務機器の項番（シミュレータ内での通し番号）\n" & _
        " %%    : 1文字の""%""\n" & _
        " 桁には1～9の数字を記述してください。その桁数になるようゼロ埋めが行われます。\n" & _
        " 桁を記述しない場合は、ゼロサプレスが行われます。\n" & _
        "ヒント2: ""$[シンボル名]""や""$関数名<引数リスト>""にマッチする部分は、下記例のようにシナリオと同じ方法で評価されます。\n" & _
        " $[$] : 1文字の""$""\n" & _
        " $MachineDir<> : シーケンスを実行する駅務機器の作業ディレクトリ（絶対パス）\n" & _
        " ただし、$ContextNum<>, $ContextDir<>, $SetRef<...>, $SetVal<...>, $Val<...>, $ExecDynFunc<...>, $ExecCmdFunc<...>, $ExecAppFunc<...> は不正な式とみなします。")
    Public Shared ScenarioFileTipText As New Sentence( _
        "絶対パス形式または作業フォルダからの相対パス形式で、ファイルを指定してください。\n" & _
        "ヒント1: ""%""で始まる文字列は、前処理で以下のように置換されます。\n" & _
        " %桁M  : シナリオを実行する駅務機器の機種コード\n" & _
        " %桁R  : シナリオを実行する駅務機器の線区コード\n" & _
        " %桁S  : シナリオを実行する駅務機器の駅順コード\n" & _
        " %桁C  : シナリオを実行する駅務機器のコーナーコード\n" & _
        " %桁U  : シナリオを実行する駅務機器の号機番号\n" & _
        " %桁I  : シナリオを実行する駅務機器の項番（シミュレータ内での通し番号）\n" & _
        " %%    : 1文字の""%""\n" & _
        " 桁には1～9の数字を記述してください。その桁数になるようゼロ埋めが行われます。\n" & _
        " 桁を記述しない場合は、ゼロサプレスが行われます。\n" & _
        "ヒント2: ""$[シンボル名]""や""$関数名<引数リスト>""にマッチする部分は、下記例のようにシナリオと同じ方法で評価されます。\n" & _
        " $[$] : 1文字の""$""\n" & _
        " $MachineDir<> : シナリオを実行する駅務機器の作業ディレクトリ（絶対パス）\n" & _
        " ただし、$ContextNum<>, $ContextDir<>, $SetRef<...>, $SetVal<...>, $Val<...>, $ExecDynFunc<...>, $ExecCmdFunc<...>, $ExecAppFunc<...> は不正な式とみなします。")

    '回線状態文言
    Public Shared LineStatusInitial As New Sentence("")
    Public Shared LineStatusConnectWaiting As New Sentence("接続中...")
    Public Shared LineStatusConnectFailed As New Sentence("接続失敗")
    Public Shared LineStatusConnected As New Sentence("接続完(未開局)")
    Public Shared LineStatusComStartWaiting As New Sentence("開局中...")
    Public Shared LineStatusSteady As New Sentence("○")
    Public Shared LineStatusDisconnected As New Sentence("切断")

    'シナリオ状態文言
    Public Shared ScenarioStatusInitial As New Sentence("")
    Public Shared ScenarioStatusLoaded As New Sentence("待機中...")
    Public Shared ScenarioStatusRunning As New Sentence("実行中")
    Public Shared ScenarioStatusAborted As New Sentence("終了(×)")
    Public Shared ScenarioStatusFinished As New Sentence("終了(○)")
    Public Shared ScenarioStatusStopped As New Sentence("停止")

    ''' <summary>INIファイルの内容を取り込む。</summary>
    ''' <remarks>
    ''' INIファイルの内容を取り込む。
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
