' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Public Class EkConstants

    '利用データのレコード長（NOTE: 使用禁止）
    'TODO: 次世代車補対応により、対N間通信プロセスとともに除去する。
    Public Const RiyoDataRecordLen As Integer = 1460

    '駅務機器から収集するファイルのヘッダ長
    Public Const UpboundDataHeaderLen As Integer = 17

    'データ区分
    'NOTE: データベースのテーブル名の編集に使用可能である。
    Public Const DataPurposeMaster As String = "MST"    'マスタ
    Public Const DataPurposeProgram As String = "PRG"   'プログラム

    'ファイル区分
    Public Const FilePurposeData As String = "DAT"      'データ（CABやBIN本体）
    Public Const FilePurposeList As String = "LST"      '適用リスト

    '機種コード
    Public Const ModelCodeNone As String = ""           '未定義機種
    Public Const ModelCodeKanshiban As String = "W"     '監視盤
    Public Const ModelCodeGate As String = "G"          '改札機
    Public Const ModelCodeTokatsu As String = "X"       '統括
    Public Const ModelCodeMadosho As String = "Y"       '窓処

    '製品コード
    Public Const ProductCodeOfKanshiban As String = "75"
    Public Const ProductCodeOfGate As String = "70"
    Public Const ProductCodeOfMadosho As String = "86"

    '機種に対応する製品コード
    Public Shared ReadOnly ProductCodeOfModels As New Dictionary(Of String, String) From { _
       {ModelCodeKanshiban, ProductCodeOfKanshiban}, _
       {ModelCodeGate, ProductCodeOfGate}, _
       {ModelCodeMadosho, ProductCodeOfMadosho}}

    '仕様コード
    'NOTE: EkCommon内の他のクラスは、これらの２文字目が機種コードであること
    'および、これらが６文字であることを前提に実装しているので、これらを
    '変更する際は注意しなければならない。
    Public Const SpecificCodeOfKanshiban As String = "EW7200"
    Public Const SpecificCodeOfGate As String = "EG7000"
    Public Const SpecificCodeOfMadosho As String = "EY4100"

    '機種に対応する仕様コード
    Public Shared ReadOnly SpecificCodeOfModels As New Dictionary(Of String, String) From { _
       {ModelCodeKanshiban, SpecificCodeOfKanshiban}, _
       {ModelCodeGate, SpecificCodeOfGate}, _
       {ModelCodeMadosho, SpecificCodeOfMadosho}}

    'プログラム代表バージョン書式
    Public Const ProgramDataVersionFormatOfKanshiban As String = "D8"
    Public Const ProgramDataVersionFormatOfGate As String = "D4"
    Public Const ProgramDataVersionFormatOfMadosho As String = "D4"

    '機種に対応するプログラム代表バージョン書式
    Public Shared ReadOnly ProgramDataVersionFormatOfModels As New Dictionary(Of String, String) From { _
       {ModelCodeKanshiban, ProgramDataVersionFormatOfKanshiban}, _
       {ModelCodeGate, ProgramDataVersionFormatOfGate}, _
       {ModelCodeMadosho, ProgramDataVersionFormatOfMadosho}}

End Class
