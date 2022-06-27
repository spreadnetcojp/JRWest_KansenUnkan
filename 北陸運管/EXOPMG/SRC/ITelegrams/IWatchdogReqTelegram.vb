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
''' ServerTelegrapherやClientTelegrapherが想定する仮想ウォッチドッグREQ電文。
''' </summary>
Public Interface IWatchdogReqTelegram
    Inherits IReqTelegram

    'ACK電文を生成するメソッド
    Function CreateAckTelegram() As ITelegram
End Interface
