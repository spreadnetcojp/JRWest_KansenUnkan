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

Imports JR.ExOpmg.Common
Imports System.Text

''' <summary>
''' 業務仕様に基づくチェック処理等を提供するクラス。
''' </summary>
''' <remarks></remarks>
Public Class OPMGUtility

    ''' <summary>指定バイト位置から指定バイト数分のByte配列を取り出す</summary>
    ''' <remarks>
    ''' 指定バイト位置から指定バイト数分のByte配列を取り出す
    ''' </remarks>
    ''' <param name="fromBytes">Byte配列</param>
    ''' <param name="startIndex">指定バイト位置</param>
    ''' <param name="resultLen">指定バイト数</param>
    ''' <returns>変換後Byte配列</returns>
    Public Shared Function getBytesFromBytes(ByVal fromBytes As Byte(), ByVal startIndex As Integer, ByVal resultLen As Integer) As Byte()
        If startIndex + resultLen > fromBytes.Length Then
            Log.Error("引数が不正です。" & vbCrLf & _
                          "startIndex:" & startIndex & _
                          "; resultLen:" & resultLen & _
                          "; fromBytes.Length:" & fromBytes.Length) '引数不正
            Throw New DatabaseException()
        End If

        Dim bRtn(resultLen - 1) As Byte

        For i As Integer = 0 To resultLen - 1
            bRtn.SetValue((fromBytes.GetValue(startIndex + i)), i)
        Next

        Return bRtn
    End Function

    ''' <summary>指定バイト位置から指定バイト数分のByte配列を設定する</summary>
    ''' <remarks>
    '''  指定バイト位置から指定バイト数分のByte配列を設定する
    ''' </remarks>
    ''' <param name="value">Byte配列</param>
    ''' <param name="toBytes">Byte配列</param>
    ''' <param name="startIndex">指定バイト位置</param>
    ''' <returns>設定後バイト位置</returns>
    Public Shared Function setBytesToBytes(ByVal value As Byte(), ByRef toBytes As Byte(), ByVal startIndex As Integer) As Integer
        If startIndex + value.Length > toBytes.Length Then
            Log.Error("引数が不正です。" & vbCrLf & _
                          "startIndex:" & startIndex & _
                          "; value.Length:" & value.Length & _
                          "; toBytes.Length:" & toBytes.Length) '引数不正
            Throw New DatabaseException()
        End If

        For i As Integer = 0 To value.Length - 1
            toBytes.SetValue((value.GetValue(i)), startIndex + i)
        Next

        Return startIndex + value.Length
    End Function

    ''' <summary>
    ''' [コード変換（BIN→文字）]
    ''' 変換元の文字はAsciiのみ有効。
    ''' </summary>
    ''' <param name="fromBytes">Byte配列</param>
    ''' <param name="startIndex">指定バイト位置</param>
    ''' <param name="bytesLen">Byte数</param>
    ''' <returns>変換後文字</returns>
    Public Shared Function getAsciiStringFromBytes(ByVal fromBytes As Byte(), ByVal startIndex As Integer, ByVal bytesLen As Integer) As String
        If startIndex + bytesLen > fromBytes.Length Then
            Log.Error("引数が不正です。" & vbCrLf & _
                          "startIndex:" & startIndex & _
                          "; bytesLen:" & bytesLen & _
                          "; fromBytes.Length:" & fromBytes.Length) '引数不正
            Throw New DatabaseException()
        End If

        Dim bTemp As Byte()
        bTemp = getBytesFromBytes(fromBytes, startIndex, bytesLen)

        Return binToAsciiString(bTemp)
    End Function

    ''' <summary>
    ''' [コード変換（文字→BIN）]
    ''' 変換元の文字はAsciiのみ有効。
    ''' </summary>
    ''' <param name="value">文字</param>
    ''' <param name="toBytes">Byte配列</param>
    ''' <param name="startIndex">指定バイト位置</param>
    ''' <param name="len">Byte数</param>
    ''' <returns>設定後バイト位置</returns>
    Public Shared Function setAsciiStringToBytes(ByVal value As String, ByVal toBytes As Byte(), ByVal startIndex As Integer, ByVal len As Integer) As Integer
        If startIndex + len > toBytes.Length Then
            Log.Error("引数が不正です。" & vbCrLf & _
                          "startIndex:" & startIndex & _
                          "; len:" & len & _
                          "; toBytes.Length:" & toBytes.Length) '引数不正
            Throw New DatabaseException()
        End If

        If value.Length > len Then
            Log.Error("引数が不正です。" & vbCrLf & _
                          "value:" & value & _
                          "; len:" & len) '引数不正
            Throw New DatabaseException()
        End If

        Dim bTemp As Byte()
        bTemp = asciiStringToBin(value, len)

        Return setBytesToBytes(bTemp, toBytes, startIndex)
    End Function

    ''' <summary>
    ''' [コード変換（BIN→文字）]
    ''' 変換元の文字はShift_JISのみ有効。
    ''' </summary>
    ''' <param name="fromBytes">Byte配列</param>
    ''' <param name="startIndex">指定バイト位置</param>
    ''' <param name="bytesLen">Byte数</param>
    ''' <returns>変換後文字</returns>
    Public Shared Function getJisStringFromBytes(ByVal fromBytes As Byte(), ByVal startIndex As Integer, ByVal bytesLen As Integer) As String
        Dim bTemp As Byte()
        bTemp = getBytesFromBytes(fromBytes, startIndex, bytesLen)

        Return binToJisString(bTemp)
    End Function

    ''' <summary>
    ''' [コード変換（文字→BIN）]
    ''' 変換元の文字はShift_JISのみ有効。
    ''' </summary>
    ''' <param name="value">文字</param>
    ''' <param name="toBytes">Byte配列</param>
    ''' <param name="startIndex">指定バイト位置</param>
    ''' <param name="len">Byte数</param>
    ''' <returns>設定後バイト位置</returns>
    Public Shared Function setJisStringToBytes(ByVal value As String, ByRef toBytes As Byte(), ByVal startIndex As Integer, ByVal len As Integer) As Integer
        Dim bTemp As Byte()
        bTemp = jisStringToBin(value, len)

        Return setBytesToBytes(bTemp, toBytes, startIndex)
    End Function

    ''' <summary>
    ''' [コード変換（文字→BIN）]
    ''' 変換元の文字はasciiのみ有効。
    ''' </summary>
    ''' <param name="ASCIIpar">ascii文字</param>
    ''' <param name="len">Byte数</param>
    ''' <returns>Byte配列</returns>
    Public Shared Function asciiStringToBin(ByVal ASCIIpar As String, ByVal len As Integer) As Byte()
        If ASCIIpar.Length > len Then
            Log.Error("引数が不正です。" & vbCrLf & _
                          "ASCIIpar:" & ASCIIpar & _
                          "; len:" & len) '引数不正
            Throw New DatabaseException()
        End If

        Dim sTemp As String
        sTemp = ASCIIpar.PadRight(len)

        Return Encoding.ASCII.GetBytes(sTemp)
    End Function

    ''' <summary>
    ''' [コード変換（BIN→文字）]
    ''' 変換元の文字はasciiのみ有効。
    ''' </summary>
    ''' <param name="par">Byte配列</param>
    ''' <returns>ascii文字</returns>
    Public Shared Function binToAsciiString(ByVal par As Byte()) As String
        Return Encoding.ASCII.GetString(par).Trim
    End Function

    ''' <summary>
    ''' [コード変換（文字→BIN）]
    ''' 変換元の文字はShift_JISのみ有効。
    ''' </summary>
    ''' <param name="jisPar">Shift_JIS文字Byte配列</param>
    ''' <param name="len">Byte数</param>
    ''' <returns>Byte配列</returns>
    Public Shared Function jisStringToBin(ByVal JISpar As String, ByVal len As Integer) As Byte()
        Dim nLen As Integer = 0
        Dim bChar() As Char = {}
        Dim bRet() As Byte = {}

        bRet = Utility.SJtoJIS(JISpar)
        nLen = len - bRet.Length

        If nLen > 0 Then
            Array.Resize(bChar, nLen)
            nLen = bRet.Length
            Array.Resize(bRet, bRet.Length + bChar.Length)
            setBytesToBytes(System.Text.Encoding.Default.GetBytes(bChar), bRet, nLen)
        Else
            Array.Resize(bRet, len)
        End If
        Return bRet
    End Function

    ''' <summary>
    ''' [コード変換（BIN→文字）]
    ''' 変換元の文字はShift_JISのみ有効。
    ''' </summary>
    ''' <param name="jisPar">Byte配列</param>
    ''' <returns>Shift_JIS文字</returns>
    Public Shared Function binToJisString(ByVal jisPar As Byte()) As String
        Return System.Text.Encoding.GetEncoding("Shift_JIS").GetString(jisPar)
    End Function

    ''' <summary>
    ''' 文字列は英数字であるかをチェックする。
    ''' </summary>
    ''' <param name="sTxtContent">チェックする必要のある文字列</param>
    ''' <returns>文字列合法フラグ</returns>
    ''' <remarks>ＩＤコード、パスワードは0-9、a-z、A-Zによって組み合わせる文字列に限る</remarks>
    Public Shared Function checkCharacter(ByVal sTxtContent As String) As Boolean

        '当関数の戻り値。
        Dim bResult As Boolean = False
        Dim cTxt As Char
        For i As Integer = 0 To sTxtContent.Length() - 1
            cTxt = sTxtContent.Chars(i)
            If (Asc(cTxt) >= 48 And Asc(cTxt) <= 57) Or (Asc(cTxt) >= 65 And Asc(cTxt) <= 90) Or (Asc(cTxt) >= 97 And Asc(cTxt) <= 122) Then
                bResult = True
            Else
                bResult = False
                Exit For
            End If
        Next

        Return bResult

    End Function

    'TODO: 削除
    ''' <summary>
    ''' 文字列は数字であるかをチェックする。
    ''' </summary>
    ''' <param name="sTxtContent">チェックする必要のある文字列</param>
    ''' <returns>文字列合法フラグ</returns>
    ''' <remarks>文字列は0-9によって組み合わせる文字列に限る</remarks>
    Public Shared Function checkNumber(ByVal sTxtContent As String) As Boolean

        '当関数の戻り値。
        Dim bResult As Boolean = False
        Dim cTxt As Char
        For i As Integer = 0 To sTxtContent.Length() - 1
            cTxt = sTxtContent.Chars(i)
            If Asc(cTxt) >= 48 And Asc(cTxt) <= 57 Then
                bResult = True
            Else
                bResult = False
                Exit For
            End If
        Next

        Return bResult

    End Function

    ''''<summary>
    ''''文字列チェック
    ''''</summary>
    ''''<param name="CheckValue">チェック対象文字列</param>
    ''''<param name="MaxLength">最大文字数</param>
    ''''<param name="CharSize">全角半角制限　0:半角全角可　1:半角のみ可　2:全角のみ可</param>
    ''''<param name="NoInputFlag">未入力チェック　true:未入力禁止　false:未入力可</param>
    ''''<returns>int 0:正常 -1:文字数超過エラー -2:未入力エラー -3:全角混在エラー -4:半角混在エラー </returns>
    Public Shared Function CheckString(ByVal CheckValue As String, ByVal MaxLength As Integer _
                        , ByVal CharSize As Integer, ByVal NoInputFlag As Boolean) As Integer

        Dim Encode As Encoding
        Encode = Encoding.GetEncoding("Shift_JIS")
        If True = NoInputFlag Then
            If CheckValue.Trim.Length = 0 Then
                '未入力エラー
                Return -2
            End If
            '半角文字制限
            If CharSize = 1 Then
                If Not CheckValue.Length = Encode.GetByteCount(CheckValue) Then
                    '全角文字が含まれている
                    Return -3
                End If
            End If
            '全角文字制限
            If CharSize = 2 Then
                If Not CheckValue.Length = Encode.GetByteCount(CheckValue) / 2 Then
                    '半角文字が含まれている
                    Return -4
                End If
            End If
            '文字数チェック
            If MaxLength * 2 < Encode.GetByteCount(CheckValue) Then
                '文字数オーバー
                Return -1
            End If
        End If
        'チェック正常
        Return 0
    End Function
End Class
