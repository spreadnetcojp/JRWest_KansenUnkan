' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2013/06/18  (NES)小林  BaseInfoにコンストラクタを追加
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' 各共通処理の構造体
''' 
''' </summary>
''' <remarks></remarks>
Public Class RecDataStructure

#Region "宣言領域（Public）"

    ''' <summary>
    ''' 定義情報
    ''' </summary>
    Public Structure DefineInfo
        Dim KOMOKU_NAME As String                   '項目名称
        Dim COMMENT As String                       'コマンド
        Dim BYTE_OFFSET As Integer                  'バイトオフセット
        Dim BYTE_LEN As Integer                     'バイトレングス
        Dim BIT_OFFSET As Integer                   'ビットオフセット
        Dim BIT_LEN As Integer                      'ビットレングス
        Dim DATA_FORMAT As String                   'データ形式
        Dim FIELD_NAME As String                    'フィールド名
        Dim FIELD_FORMAT As String                  'フィールド形式
        Dim PARA1 As Boolean                        '主キーか否か
        Dim PARA2 As Boolean                        'NULL許容か否か
        Dim PARA3 As String                         'パラメーター
        Dim PARA4 As String                         'パラメーター
        Dim PARA5 As String                         'パラメーター
        Dim PARA6 As String                         'パラメーター
    End Structure

    ''' <summary>
    ''' 基本ヘッダ部情報
    ''' </summary>
    Public Structure BaseInfo
        Dim DATA_KIND As String                 'データ種別（1～2桁の16進数字）
        Dim STATION_CODE As Station             '駅コード
        Dim PROCESSING_TIME As String           '処理日時（文脈によりyyyyMMddHHmmss形式またはDateTimeをカルチャ依存でToStringした形式）TODO: 要リファクタリング
        Dim CORNER_CODE As String               'コーナー（4桁の数字）
        Dim UNIT_NO As Integer                  '号機（1～2桁の数字）
        Dim MODEL_CODE As String                '機種

        Public Sub New(ByVal causeModel As String, ByVal causeUnit As EkCode, ByVal time As DateTime)
            Me.MODEL_CODE = causeModel
            Me.STATION_CODE.RAIL_SECTION_CODE = causeUnit.RailSection.ToString("D3")
            Me.STATION_CODE.STATION_ORDER_CODE = causeUnit.StationOrder.ToString("D3")
            Me.CORNER_CODE = causeUnit.Corner.ToString("D4")
            Me.UNIT_NO = causeUnit.Unit
            Me.PROCESSING_TIME = time.ToString("yyyyMMddHHmmss")
        End Sub

        Public Sub New(ByVal causeModel As String, ByVal causeUnit As EkCode)
            Me.New(causeModel, causeUnit, DateTime.Now)
        End Sub
    End Structure

    ''' <summary>
    ''' 駅コード
    ''' </summary>
    Public Structure Station
        Dim RAIL_SECTION_CODE As String        '線区（3桁の数字）
        Dim STATION_ORDER_CODE As String       '駅順（3桁の数字）
    End Structure

#End Region

End Class
