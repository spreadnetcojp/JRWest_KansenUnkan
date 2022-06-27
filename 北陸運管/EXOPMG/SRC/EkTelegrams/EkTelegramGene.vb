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
''' 電文書式。
''' </summary>
Public MustInherit Class EkTelegramGene

#Region "各項目の格納位置"
    'NOTE: 以下の変数は、サブクラスのコンストラクタで値を設定する。
    'サブクラス以外は、EkTelegramパッケージ内での参照のみ可とする。
    Protected Friend CmdCodePos As Integer
    Protected Friend SubCmdCodePos As Integer
    Protected Friend ReqNumberPos As Integer
    Protected Friend ClientModelCodePos As Integer
    Protected Friend ClientRailSectionCodePos As Integer
    Protected Friend ClientStationOrderCodePos As Integer
    Protected Friend ClientCornerCodePos As Integer
    Protected Friend ClientUnitCodePos As Integer
    Protected Friend SendTimePos As Integer
    Protected Friend ObjSizePos As Integer
    Protected Friend ObjCodePos As Integer
    Protected Friend ObjDetailPos As Integer
#End Region

#Region "各項目の格納長"
    'NOTE: 以下の変数は、サブクラスのコンストラクタでもとりあえず変更は不可とする。
    'サブクラス以外は、EkTelegramパッケージ内での参照のみ可とする。
    'NOTE: ハッシュマップをここに移動してくるなどにより、ほとんどの項目は
    'サブクラスで変更可能になるはず。
    Protected Friend CmdCodeLen As Integer = 3
    Protected Friend SubCmdCodeLen As Integer = 4
    Protected Friend ReqNumberLen As Integer = 6
    Protected Friend ClientModelCodeLen As Integer = 2
    Protected Friend ClientRailSectionCodeLen As Integer = 3
    Protected Friend ClientStationOrderCodeLen As Integer = 3
    Protected Friend ClientCornerCodeLen As Integer = 4
    Protected Friend ClientUnitCodeLen As Integer = 2
    Protected Friend SendTimeLen As Integer = 17
    Protected Friend ObjSizeLen As Integer = 4
    Protected Friend ObjCodeLen As Integer = 1
#End Region

#Region "送信開始日時の書式"
    'NOTE: 以下の変数は、サブクラスのコンストラクタで値を設定しなおしてもよい。
    'EkTelegramパッケージ外では変更も参照も不可とする。
    Protected Friend SendTimeFormat As String = "yyyyMMddHHmmssfff"
#End Region

#Region "ソケットやストリームから生成するEkTelegramの制限値"
    'NOTE: 以下の変数は、サブクラスのコンストラクタで値を設定する。
    'サブクラス以外は、EkTelegramパッケージ内での参照のみ可とする。
    'NOTE: 下記の条件を満たす値を設定すること。
    'MaxReceiveSize > MinAllocSize >= Gene.GetRawLenByObjSize(Gene.GetObjSizeByObjDetailLen(0))
    Protected Friend MinAllocSize As Integer
    Protected Friend MaxReceiveSize As Integer
#End Region

    'NOTE: 以降のメンバは、EkXxxxTelegramからはアクセスしない方針とする。
    '理由は、それらのクラスをいくつかに分別して、別のプロジェクトに
    '移すことを可能にしておくためである。
    'それらのクラスでは、以降のメンバを直接アクセスするかわりに、
    'EkTelegramに用意されたメソッドを用いて間接的にアクセスすることが可能である。

#Region "XllReqTelegramの背後に存在する情報"
    'NOTE: 以下の変数は、サブクラスのコンストラクタで値を設定する。
    'サブクラス以外は、EkTelegramパッケージ内での参照のみ可とする。

    'XllReqTelegram.TransferListのベースパス（ローカルパス）
    Protected Friend XllBasePath As String
#End Region

#Region "メソッド"
    'NOTE: 以下のメソッドは、サブクラスのコンストラクタで実装する。
    'サブクラス以外は、EkTelegramパッケージ内でのみ使用可とする。

    'ObjSizeから電文全体のバイト長を算出するメソッド
    Protected Friend MustOverride Function GetRawLenByObjSize(ByVal objSize As UInteger) As Integer

    '電文全体バイト長からObjSizeにセットするべき値を算出するメソッド
    Protected Friend MustOverride Function GetObjSizeByRawLen(ByVal rawLen As Integer) As UInteger

    'ObjSizeからObjDetail部のバイト長を算出するメソッド
    Protected Friend MustOverride Function GetObjDetailLenByObjSize(ByVal objSize As UInteger) As Integer

    'ObjDetail部のバイト長からObjSizeにセットするべき値を算出するメソッド
    Protected Friend MustOverride Function GetObjSizeByObjDetailLen(ByVal objDetailLen As Integer) As UInteger

    'CRC部に値をセットするメソッド
    Protected Friend MustOverride Sub UpdateCrc(ByVal aRawBytes As Byte())

    'CRC部の値とその他の部位の値の整合性をチェックするメソッド
    Protected Friend MustOverride Function IsCrcIndicatingOkay(ByVal aRawBytes As Byte()) As Boolean
#End Region

End Class
