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
''' Type5～8のシーケンスでServerTelegrapherやClientTelegrapherが
''' 想定する仮想電文。
''' </summary>
Public Interface IXllTelegram
    Inherits ITelegram

    'NOTE: Ull用電文のクラスにおいてContinueCodeプロパティで
    'ContinueCode.FinishWithoutStorinを返却するのは禁止とする。
    'そもそも、実際のUll用電文の仕様で、そのような値が
    '定義されていることはないはずである。
    '受信した電文のContinueCode相当項目に「Dll用電文の仕様で
    'ContinueCode.FinishWithoutStoring相当の値」が偶然格納されて
    'いる場合、ContinueCodeプロパティは、ContinueCode.Noneを
    '返却するべきである。
    ReadOnly Property ContinueCode() As ContinueCode
End Interface

'仮想「開始・終了」値
Public Enum ContinueCode As Integer
    None
    Start                   '転送開始
    Finish                  '転送正常終了
    FinishWithoutStoring    '転送正常終了も保存せず
    Abort                   '転送異常終了
End Enum
