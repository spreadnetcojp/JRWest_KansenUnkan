' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp.RecDataStructure

''' <summary>
''' バイナリファイルの基本ヘッダ部を解析し、登録データとしてメモリに保持する。
''' </summary>
Public Class BinaryHeadInfoParse

#Region "メソッド（Public）"
    ''' <summary>
    ''' 基本ヘッダ部情報の解析
    ''' </summary>
    ''' <param name="baseInfo">バイナリファイルの基本ヘッダ部</param>
    ''' <param name="clientKind">クライアント種別</param>
    ''' <param name="infoObj">解析した結果を保存用</param> 
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Public Shared Function GetBaseInfo(ByVal baseInfo As Byte(), _
                                       ByVal clientKind As String, _
                                       ByRef infoObj As BaseInfo) As Boolean
        Dim bRtn As Boolean = False

        Try

            'データ種別
            infoObj.DATA_KIND = Hex(baseInfo(0))

            '駅コード
            infoObj.STATION_CODE.RAIL_SECTION_CODE = baseInfo(1).ToString("D3")
            infoObj.STATION_CODE.STATION_ORDER_CODE = baseInfo(2).ToString("D3")

            '処理日時
            infoObj.PROCESSING_TIME = FnHexDisp(baseInfo(3)) & FnHexDisp(baseInfo(4)) & _
                                      FnHexDisp(baseInfo(5)) & FnHexDisp(baseInfo(6)) & _
                                      FnHexDisp(baseInfo(7)) & FnHexDisp(baseInfo(8)) & _
                                      FnHexDisp(baseInfo(9))

            'コーナー
            infoObj.CORNER_CODE = baseInfo(10).ToString("D4")

            '号機
            infoObj.UNIT_NO = baseInfo(11)

            '機種
            Select Case clientKind
                Case "02"
                    Select Case infoObj.DATA_KIND
                        Case "A1", "A2", "A3", "A4", "A5", "A7", "A8", "55", "B1"
                            'G：（改札機）
                            infoObj.MODEL_CODE = EkConstants.ModelCodeGate
                        Case "54"
                            'W：（監視盤）
                            infoObj.MODEL_CODE = EkConstants.ModelCodeKanshiban
                        Case "A6", "C3"
                            If infoObj.UNIT_NO = 0 Then
                                'G：（改札機）
                                infoObj.MODEL_CODE = EkConstants.ModelCodeGate
                            Else
                                'W：（監視盤）
                                infoObj.MODEL_CODE = EkConstants.ModelCodeKanshiban
                            End If
                    End Select

                Case "06"
                    If infoObj.DATA_KIND = "89" Then
                        'Y：（窓口処理機）
                        infoObj.MODEL_CODE = EkConstants.ModelCodeMadosho
                    Else
                        'X：明収／EX統括
                        infoObj.MODEL_CODE = EkConstants.ModelCodeTokatsu
                    End If
                Case "08"
                    '０８＝Y：（窓口処理機）
                    infoObj.MODEL_CODE = EkConstants.ModelCodeMadosho
            End Select

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        End Try

        Return bRtn

    End Function

    ''' <summary>
    ''' １０進数を０ｻﾌﾟﾚｽしない１６進数の形に変換する
    ''' </summary>
    ''' <param name="bytDat10">１ﾊﾞｲﾄﾃﾞｰﾀ</param>
    ''' <returns>fnHexDisp       １６進数文字列</returns>
    Private Shared Function FnHexDisp(ByVal bytDat10 As Byte) As String

        '常時２桁で返す
        If Len(Hex(bytDat10)) <= 1 Then     '１桁ならば
            fnHexDisp = "0" & Hex(bytDat10)   '０ｻﾌﾟﾚｽしない
        Else                                '２桁ならば
            fnHexDisp = Hex(bytDat10)         'そのまま
        End If

    End Function

#End Region

End Class
