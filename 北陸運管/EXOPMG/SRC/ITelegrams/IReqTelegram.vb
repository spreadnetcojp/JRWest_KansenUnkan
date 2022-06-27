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

''' <summary>
''' ServerTelegrapherやClientTelegrapherが想定する仮想REQ電文。
''' </summary>
Public Interface IReqTelegram
    Inherits ITelegram

    '応答返信期限
    'NOTE: プロトコルによって、実際の電文内にこれに相当する項目は存在しないはずである。
    'その場合は、アプリ側においてインスタンス化する際の引数で設定する。
    'REQ電文を送信する側のみが参照するので、受信した内容不明電文からREQ電文の
    'インスタンスを作成する際は、適当な値（0など）が自動で設定されればよい。
    ReadOnly Property ReplyLimitTicks() As Integer

    '渡された電文がACKとして整合性があるか判定するメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    'NOTE: 引数で渡す電文はCmdKindがAckであることを確認済みとする。
    Function IsValidAck(ByVal oReplyTeleg As ITelegram) As Boolean

    '渡された電文がNAKとして整合性があるか判定するメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    'NOTE: 引数で渡す電文はCmdKindがNakであることを確認済みとする。
    Function IsValidNak(ByVal oReplyTeleg As ITelegram) As Boolean

    '渡された電文の型をACK電文の型に変換するメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    'NOTE: 変換後のオブジェクトに対するGetBodyFormatViolation()の実行も、呼び元の責務である。
    Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As ITelegram

    '渡された電文の型をNAK電文の型に変換するメソッド
    'NOTE: 電文書式が同一であることは、呼び元が保証する。
    'NOTE: 呼び元は、Meだけでなく、引数で渡す電文についても、
    'ヘッダ部に書式違反が無いことを確認済みでなければならない。
    'NOTE: 変換後のオブジェクトに対するGetBodyFormatViolation()の実行も、呼び元の責務である。
    Function ParseAsNak(ByVal oReplyTeleg As ITelegram) As INakTelegram
End Interface 
