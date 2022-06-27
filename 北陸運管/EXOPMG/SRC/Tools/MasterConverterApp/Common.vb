' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/03/01  (NES)河脇  新規作成
'   0.1      2017/06/15  (NES)趙　  ポイントポストペイ対応
'                                   ・昼特区間・時間マスタ追加対応
'                                   ・ポストペイエリアマスタ追加対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text
Imports JR.ExOpmg.Common

Public Class Common

    ''' <summary>
    ''' CSVファイルを読み込む
    ''' </summary>
    ''' <param name="filename">CSVファイル名</param>
    ''' <returns>読み込み結果用の配列</returns>
    Public Shared Function ReadCsv(ByVal filename As String) As ArrayList

        Dim ret As New ArrayList

        ''テキストファイルかどうか
        'Try
        '    Dim bytedata As Byte() = System.IO.File.ReadAllBytes(filename)

        '    For i As Integer = 0 To bytedata.Length - 1
        '        If bytedata(i) = 0 Then
        '            AlertBox.Show(Lexis.ERR_FILE_CSV)
        '            Return ret
        '        End If
        '    Next
        'Catch ex As Exception
        '    AlertBox.Show(Lexis.ERR_FILE_READ)
        '    Throw
        'End Try

        Try
            'Shift JISで読み込みます。
            Using swText As New FileIO.TextFieldParser(filename, System.Text.Encoding.GetEncoding(932))

                'フィールドが文字で区切られている設定を行います。
                swText.TextFieldType = FileIO.FieldType.Delimited

                '区切り文字を「,（カンマ）」に設定します。
                swText.Delimiters = New String() {","}

                'フィールドを"で囲み、改行文字、区切り文字を含めることが 'できるかを設定します。
                swText.HasFieldsEnclosedInQuotes = True

                'フィールドの前後からスペースを削除する設定を行います。
                swText.TrimWhiteSpace = False

                While Not swText.EndOfData
                    'CSVファイルのフィールドを読み込みます。
                    Dim fields As String() = swText.ReadFields()

                    '配列に追加します。コメントを除く
                    If Not fields(0).StartsWith("#") And Not fields(0).StartsWith("&") Then
                        ret.Add(fields)
                    End If
                End While

            End Using
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_FILE_READ)
            Throw
        End Try
        Return ret

    End Function

    ''' <summary>
    '''CSVファイルの指定行値を取得する
    ''' </summary>
    ''' <param name="CsvData">CSV内容を持つ配列</param>
    ''' <param name="row">行</param>
    Public Shared Function ReadStringFromCSV(ByVal CsvData As ArrayList, ByVal row As Integer) As String()

        Return CType(CsvData.Item(row), String())

    End Function

    ''' <summary>
    '''CSVファイルの指定行、列の値を取得する
    ''' </summary>
    ''' <param name="CsvData">CSV内容を持つ配列</param>
    ''' <param name="row">行</param>
    ''' <param name="col">列</param>
    ''' <returns>該当位置の値</returns>
    Public Shared Function ReadStringFromCSV(ByVal CsvData As ArrayList, ByVal row As Integer, ByVal col As Integer) As String

        Try
            Dim a As String() = CType(CsvData.Item(row), String())

            Return a(col)
        Catch ex As Exception
            Return ""
        End Try

    End Function

    Public Shared Function ReadBin(ByVal filename As String) As Byte()
        Try
            Return System.IO.File.ReadAllBytes(filename)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_FILE_READ)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 文字列がある範囲かどうかを判断する
    ''' </summary>
    ''' <param name="str">判断対象</param>
    ''' <param name="min">最少値</param>
    ''' <param name="max">最大値</param>
    ''' <returns>範囲以内の場合、true,その他 false</returns>
    Public Shared Function IsBetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String) As Boolean
        Try
            Dim r As New System.Text.RegularExpressions.Regex("^[0-9]+$")
            If r.IsMatch(str) = False Then
                Return False
            Else
                If CLng(str) > CLng(max) Or CLng(str) < CLng(min) Then
                    Return False
                End If
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 有効終了日のbyte配列を取得
    ''' </summary>
    Public Shared Function GetBCDDate(ByVal str As String, ByVal name As String) As Byte()

        Try
            '日付チェック
            Return Utility.CHARtoBCD(DateTime.Parse(Format(CInt(str), "0000/00/00")).ToString("yyyyMMdd"), 4)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 適用日付のbyte配列を取得
    ''' </summary>
    Public Shared Function GetApplyDate(ByVal str As String, ByVal name As String) As Byte()

        Try
            '日付チェック
            Return Utility.CHARtoBCD(DateTime.Parse(str).ToString("yyyyMMdd"), 4)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 適用日付のbyte配列を取得
    ''' </summary>
    Public Shared Function GetApplyDateDEC(ByVal str As String, ByVal name As String) As Byte()

        Try
            '日付チェック
            Return Utility.CHARtoDEC(DateTime.Parse(str).ToString("yyyyMMdd"), 8)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 適用日付のbyte配列を取得
    ''' </summary>
    Public Shared Function GetApplyDateTimeDEC(ByVal str As String, ByVal name As String) As Byte()

        Try
            '日付チェック
            Return Utility.CHARtoDEC(DateTime.Parse(str).ToString("yyyyMMddHHmm"), 12)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 適用日付のbyte配列を取得
    ''' </summary>
    Public Shared Function GetApplyDateTimeBCD(ByVal str As String, ByVal name As String) As Byte()

        Try
            '日付チェック
            Return Utility.CHARtoBCD(DateTime.Parse(str).ToString("yyyyMMddHHmm"), 6)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' マスタバージョンのbyte配列を取得
    ''' </summary>
    Public Shared Function GetVersion(ByVal str As String) As Byte()

        Try
            If IsBetweenAnd(str, "1", "255") Then
                Return New Byte() {Byte.Parse(str)}
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, "マスタバージョン")
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 指定文字列のbyte配列を取得
    ''' </summary>
    Public Shared Function GetBytesBetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return New Byte() {Byte.Parse(str)}
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 指定文字列のbyte配列を取得、配列に2byteがある
    ''' </summary>
    Public Shared Function GetBytes2BetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return BitConverter.GetBytes(Short.Parse(str))
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 指定文字列のbyte配列を取得、配列に3byteがある(モトローラ型（Big Endian）)
    ''' </summary>
    Public Shared Function GetBytes3BetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return Utility.CHARtoBINwithBigEndian(str, 3)
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 指定文字列のBCD byte配列を取得
    ''' </summary>
    Public Shared Function GetBCDBytesBetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String, ByVal len As Integer) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return Utility.CHARtoBCD(str, len)
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 指定文字列のBCD byte配列を取得
    ''' </summary>
    Public Shared Function GetDECBytesBetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String, ByVal len As Integer) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return Utility.CHARtoDEC(str, len)
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' 発行機関名のbyte配列を取得
    ''' </summary>
    Public Shared Function GetBytesKikan(ByVal str As String, ByVal name As String) As Byte()

        Dim ret As Byte() = New Byte(15) {}
        ret(0) = &H30

        If "0".CompareTo(str) = 0 Then
            Return ret
        End If

        Try
            If Encoding.GetEncoding(932).GetByteCount(str) / 2 = str.Length Then
                'SHIFT-JIS→JIS
                Dim temp As Byte() = Utility.SJtoJIS(str)
                Array.Copy(temp, ret, temp.Length)
            Else
                Throw New Exception
            End If

            Return ret
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    'Ver0.1 ADD START  ポイントポストペイ対応
    ''' <summary>
    ''' CSVファイルを読み込む
    ''' </summary>
    ''' <param name="filename">CSVファイル名</param>
    ''' <returns>読み込み結果用の配列</returns>
    Public Shared Function ReadCsvJRW(ByVal filename As String) As ArrayList

        Dim ret As New ArrayList
        Dim subList As New ArrayList

        Try
            'Shift JISで読み込みます。
            Using swText As New FileIO.TextFieldParser(filename, System.Text.Encoding.GetEncoding(932))

                'フィールドが文字で区切られている設定を行います。
                swText.TextFieldType = FileIO.FieldType.Delimited

                '区切り文字を「,（カンマ）」に設定します。
                swText.Delimiters = New String() {","}

                'フィールドを"で囲み、改行文字、区切り文字を含めることが 'できるかを設定します。
                swText.HasFieldsEnclosedInQuotes = True

                'フィールドの前後からスペースを削除する設定を行います。
                swText.TrimWhiteSpace = False

                While Not swText.EndOfData
                    'CSVファイルのフィールドを読み込みます。
                    Dim fields As String() = swText.ReadFields()

                    '配列に追加します。コメントを除く
                    If fields(0).StartsWith("#") Then
                        subList = New ArrayList
                        ret.Add(subList)
                    Else
                        subList.Add(fields)
                    End If
                End While

            End Using
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_FILE_READ)
            Throw
        End Try
        Return ret

    End Function

    ''' <summary>
    ''' 指定数値ののbyte配列を取得、配列に2byteがある(モトローラ型（Big Endian）)
    ''' </summary>
    Public Shared Function INTtoBINwithBigEndian(ByVal intVal As Integer,Optional ByVal len As Integer = 2) As Byte()

        Return Utility.CHARtoBINwithBigEndian(intVal.ToString, len)

    End Function
    'Ver0.1 ADD END  ポイントポストペイ対応
End Class
