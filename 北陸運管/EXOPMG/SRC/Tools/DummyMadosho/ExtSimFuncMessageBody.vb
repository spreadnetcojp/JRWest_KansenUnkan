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

''' <summary>
''' MultiplexEkimuSimに対する外部からの操作メッセージのBodyの型。
''' </summary>
Public Structure ExtSimFuncMessageBody
    Public MachineId As String
    Public Verb As String
    Public Params As Object()
End Structure
