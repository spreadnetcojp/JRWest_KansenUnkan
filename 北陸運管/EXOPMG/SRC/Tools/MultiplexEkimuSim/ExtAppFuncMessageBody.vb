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
''' MultiplexEkimuSimから外部常駐プロセスへの要求メッセージおよび
''' その応答メッセージのBodyの型。
''' </summary>
Public Structure ExtAppFuncMessageBody
    Public WorkingDirectory As String
    Public Func As String
    Public Args As String()
    Public Completed As Boolean
    Public Result As String
End Structure
