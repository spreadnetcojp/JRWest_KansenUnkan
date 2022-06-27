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
Imports System.Text

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.ServerApp.RecDataStructure

''' <summary>
''' ファイル名にて送信元クライアント情報を解析し、一時保持する。
''' </summary>
Public Class DefineInfoShutoku

#Region "宣言領域（Private）"

    ''' <summary>
    ''' 値タイプのチェック
    ''' </summary>
    Public Shared DataTypeError As String = "{0}タイプエラー。"

    ''' <summary>
    ''' １行目のデータ項目数のチェック
    ''' </summary>
    Public Shared DataNumError As String = "定義情報{0}行目のデータ項目数が不正です。"

#End Region

#Region "メソッド（Public）"

    ''' <summary>
    ''' 定義情報の取得
    ''' </summary>
    ''' <param name="fileName">INIファイル名</param>
    ''' <param name="sectionName">セクション名</param> 
    ''' <param name="infoObj">取得した結果を保存用</param> 
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>INIファイル名にて電文フォーマット定義情報を取得し、一時保持する</remarks>
    Public Shared Function GetDefineInfo(ByVal fileName As String, _
                                         ByVal sectionName As String, _
                                         ByRef infoObj() As DefineInfo) As Boolean
        Dim bRtn As Boolean = False
        Dim i As Integer = 0
        Dim strDefInfo As String = ""
        Dim strData() As String
        Try

            'INIファイルの存在チェック
            If File.Exists(fileName) = False Then
                Log.Error(String.Format(RecAppConstants.ERR_INI_FILE_NOT_FOUND, fileName))
                Return False
            End If

            For i = 1 To 9999
                strDefInfo = Constant.GetIni(sectionName, Format(i, "0000"), fileName)
                If strDefInfo <> "" Then
                    strData = strDefInfo.Split(CChar(","))

                    If strData.Length < 15 Then
                        Log.Error(String.Format(DataNumError, i))
                        Return bRtn
                    End If

                    'TODO: OPT: AAAAAA そもそも、このメソッドが１ファイルを登録するたびに呼ばれる
                    'こと自体が根本的にNGであるが、これはあまりにも...
                    'とりあえず「() As RecDataStructure.DefineInfo」をgrepして、
                    '「 As List(Of RecDataStructure.DefineInfo)」に置き換え、
                    'このメソッドでは、１回だけ「infoObj = New List(Of DefineInfo)」して
                    'ここは「New DefineInfo」と、それのAddにするだけでも、フィールド数の
                    '階乗分のDefineInfoのコピーが発生しないだけ、なんぼかましになりそう？
                    'それとも、領域確保のコストも無くすべく、このループの前にsectionName内に
                    'ある全てのKeyを取得し、１回だけ配列のNewを行うことにするか？
                    'しかし、領域確保のコストを言い出したら、登録プロセス用のこのライブラリは
                    '各フィールドのちょっとした変換を行うだけでも、ヒープからの無駄な確保を
                    '何百回も行っているし...
                    ReDim Preserve infoObj(i - 1)

                    '項目名称：日本語名称を取得。エラーメッセージに使用。
                    infoObj(i - 1).KOMOKU_NAME = strData(0)

                    'コマンド：情報を取得するが、暫定使用しません。
                    infoObj(i - 1).COMMENT = strData(1)

                    'バイトオフセット: 該当項目のバイトオフセット
                    Dim sByteOffset As String = strData(2)
                    If IsNumeric(sByteOffset) Then
                        infoObj(i - 1).BYTE_OFFSET = Convert.ToInt32(sByteOffset)
                    Else
                        Log.Error(String.Format(DataTypeError, "バイトオフセット"))
                        Return bRtn
                    End If

                    'バイトレングス: 該当項目のバイトレングス
                    Dim sByteLen As String = strData(3)
                    If IsNumeric(sByteLen) Then
                        infoObj(i - 1).BYTE_LEN = Integer.Parse(sByteLen)
                    Else
                        Log.Error(String.Format(DataTypeError, "バイトレングス"))
                        Return bRtn
                    End If


                    'ビットオフセット: 該当項目のビットオフセット
                    Dim sBitOffset As String = strData(4)
                    If IsNumeric(sBitOffset) Then
                        infoObj(i - 1).BIT_OFFSET = Integer.Parse(sBitOffset)
                    Else
                        Log.Error(String.Format(DataTypeError, "ビットオフセット"))
                        Return bRtn
                    End If

                    'ビットレングス: 該当項目のビットレングス
                    Dim sBitLen As String = strData(5)
                    If IsNumeric(sBitLen) Then
                        infoObj(i - 1).BIT_LEN = Integer.Parse(sBitLen)
                    Else
                        Log.Error(String.Format(DataTypeError, "ビットレングス"))
                        Return bRtn
                    End If

                    'データ形式:  該当項目がBINまたはBCD
                    infoObj(i - 1).DATA_FORMAT = strData(6)

                    'フィールド名: 登録対象ＤＢフィールド
                    infoObj(i - 1).FIELD_NAME = strData(7)

                    'フィールド形式: 登録時の型
                    infoObj(i - 1).FIELD_FORMAT = strData(8)

                    '主キー
                    If UCase(strData(9)).Equals("TRUE") Then
                        infoObj(i - 1).PARA1 = True
                    Else
                        infoObj(i - 1).PARA1 = False
                    End If

                    'IS NULL
                    If UCase(strData(10)).Equals("TRUE") Then
                        infoObj(i - 1).PARA2 = True
                    Else
                        infoObj(i - 1).PARA2 = False
                    End If

                    'パラメーター
                    infoObj(i - 1).PARA3 = strData(11)

                    'パラメーター
                    infoObj(i - 1).PARA4 = strData(12)

                    'パラメーター
                    infoObj(i - 1).PARA5 = strData(13)

                    'パラメーター
                    infoObj(i - 1).PARA6 = strData(14)
                Else
                    Exit For
                End If
            Next

            bRtn = True
        Catch ex As Exception
            Log.Error(RecAppConstants.ERR_BAD_INI_FILE)
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        End Try

        '空チェック
        If bRtn AndAlso infoObj Is Nothing OrElse infoObj.Length <= 0 Then
            Log.Error(RecAppConstants.ERR_BAD_INI_FILE)
            bRtn = False
        End If

        Return bRtn

    End Function

#End Region

End Class