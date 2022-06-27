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

Imports System.Threading

''' <summary>
''' ClientTelegrapherが仮想するファイル転送スレッド。
''' </summary>
Public Interface IXllWorker
    ReadOnly Property ThreadState() As ThreadState
    Sub Start()
    Sub Abort()
    Sub PrepareTransfer()
    Sub CancelTransfer()
End Interface 
