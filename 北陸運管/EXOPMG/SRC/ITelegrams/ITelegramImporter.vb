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

Imports System.IO
Imports System.Net.Sockets

''' <summary>
''' 外部から電文を取り込むオブジェクトのインタフェース。
''' </summary>
Public Interface ITelegramImporter
    'ソケットからの電文取得メソッド
    'NOTE: timeoutBaseTicksに0または-1を指定すると無期限待機となる。
    'NOTE: バイト列が電文として完全に不正である（所定箇所に記載されている
    'レングスが規定値に満たない、あるいは規定より大きい）ために処理できない
    '場合や、指定時間内にヘッダ部に相当するバイト数を読み取れないまたは、
    'ヘッダ部に記載された分のバイト数を読み取れない場合、電文の途中で
    '相手装置から終端を告げられた場合、外部要因の可能性がある
    'SocketExceptionが発生した場合など、コネクション終了に持ち込むべきで
    'ある（プログラムの異常と扱うべきでない）ケースでは、発生事象を内部で
    '記録し、Nothingを返却する。
    Function GetTelegramFromSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As ITelegram
End Interface
