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

Imports Microsoft.VisualBasic.FileIO
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Text

Public Class MyUtility

    Public Shared ReadOnly MachineIndexRegx As New Regex("%[0-9]*I", RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Public Shared Function ReplaceMachineIndex(ByVal sText As String, ByVal index As Integer) As String
        Dim oBuilder As New StringBuilder()
        Dim startPos As Integer = 0

        Dim oMatch As Match = MachineIndexRegx.Match(sText)
        Do While oMatch.Success
            Dim s As String = oMatch.Value
            If s.Length = 2 Then
                s = index.ToString()
            Else
                Dim sNum As String = s.Substring(1, s.Length - 2)
                s = index.ToString("D" & sNum)
            End If

            Dim matchPos As Integer = oMatch.Index
            If matchPos <> startPos Then
                oBuilder.Append(sText.Substring(startPos, matchPos - startPos))
            End If

            oBuilder.Append(s)
            startPos = matchPos + oMatch.Length

            oMatch = oMatch.NextMatch()
        Loop

        If startPos <> sText.Length Then
            oBuilder.Append(sText.Substring(startPos, sText.Length - startPos))
        End If

        Return oBuilder.ToString()
    End Function

    'NOTE: oEvaluandObjには展開済みパス文字列（""は存在しないファイルを指定されたのと同等とみなす）またはバイト配列を渡せます。
    'NOTE: oCompObjには展開済みパス文字列（""はファイルなしの意、"*"はあらゆる内容のファイルの意）またはバイト配列を渡せます。
    'NOTE: oMaskObjには展開済みパス文字列（"*"は長さだけ比較の意）またはバイト配列を渡せます。
    'NOTE: oEvaluandObjのファイルが存在しないことを確認するには、oCompObjで""を渡します。
    Public Shared Function IsMatchBin(ByVal oEvaluandObj As Object, ByVal oCompObj As Object, ByVal oMaskObj As Object, ByVal evaluationLen As Integer, Optional ByVal sDefaultBasePath As String = Nothing) As Boolean
        Dim oCompBytes As Byte()
        If oCompObj.GetType() Is GetType(String) Then
            Dim sCompObj As String = DirectCast(oCompObj, String)
            If sCompObj.Equals("*", StringComparison.Ordinal) Then
                If evaluationLen < 0 Then
                    '常に一致とする。
                    Return True
                ElseIf evaluationLen = 0 Then
                    'バイト数が0以上なら（つまり、ファイルが存在してさえいれば）一致とする。
                    If oEvaluandObj.GetType() Is GetType(String) Then
                        Dim sEvaluandObj As String = DirectCast(oEvaluandObj, String)
                        Return (sEvaluandObj.Length <> 0 AndAlso File.Exists(sEvaluandObj))
                    Else
                        Return True
                    End If
                Else
                    'バイト数がevaluationLen以上なら一致とする。
                    If oEvaluandObj.GetType() Is GetType(String) Then
                        Dim sEvaluandObj As String = DirectCast(oEvaluandObj, String)
                        If sEvaluandObj.Length = 0 Then Return False
                        Dim oInfo As New FileInfo(sEvaluandObj)
                        If Not oInfo.Exists Then Return False
                        Return (oInfo.Length >= evaluationLen)
                    Else
                        Return (DirectCast(oEvaluandObj, Byte()).Length >= evaluationLen)
                    End If
                End If
            End If
            If sCompObj.Length = 0 Then
                '存在しないことを確認する。
                If oEvaluandObj.GetType() Is GetType(String) Then
                    Dim sEvaluandObj As String = DirectCast(oEvaluandObj, String)
                    Return (sEvaluandObj.Length = 0 OrElse Not File.Exists(sEvaluandObj))
                Else
                    Return False
                End If
            End If
            oCompBytes = GetBytesFromPathString(sCompObj, sDefaultBasePath)
        Else
            oCompBytes = DirectCast(oCompObj, Byte())
        End If

        Dim oEvaluandBytes As Byte()
        If oEvaluandObj.GetType() Is GetType(String) Then
            oEvaluandBytes = GetBytesFromPathString(DirectCast(oEvaluandObj, String), sDefaultBasePath)
        Else
            oEvaluandBytes = DirectCast(oEvaluandObj, Byte())
        End If

        Dim oMaskBytes As Byte()
        If oMaskObj.GetType() Is GetType(String) Then
            Dim sMaskObj As String = DirectCast(oMaskObj, String)
            If sMaskObj.Equals("*", StringComparison.Ordinal) Then
                If evaluationLen < 0  Then
                    Return (oEvaluandBytes.Length = oCompBytes.Length)
                Else
                    Return (oEvaluandBytes.Length >= evaluationLen)
                End If
            End If
            oMaskBytes = GetBytesFromPathString(sMaskObj, sDefaultBasePath)
        Else
            oMaskBytes = DirectCast(oMaskObj, Byte())
        End If

        If evaluationLen < 0 Then
            evaluationLen = oEvaluandBytes.Length
            If oCompBytes.Length <> evaluationLen Then
                Return False
            End If
        Else
            If oEvaluandBytes.Length < evaluationLen Then
                Return False
            End If
            If oCompBytes.Length < evaluationLen Then
                Return False
            End If
        End If

        If oMaskBytes.Length < evaluationLen Then
            evaluationLen = oMaskBytes.Length
        End If

        Dim lastIndex As Integer = evaluationLen - 1
        For i As Integer = 0 To lastIndex
            Dim b1 As Byte = oEvaluandBytes(i) And oMaskBytes(i)
            Dim b2 As Byte = oCompBytes(i) And oMaskBytes(i)
            If b1 <> b2 Then Return False
        Next i

        Return True
    End Function

    'NOTE: oEvaluandObjには展開済みパス文字列またはデータ文字列の配列を渡せます。
    'NOTE: oCompObjには展開済みパス文字列（""はファイルなしの意、"*"はあらゆる内容のファイルの意）またはデータ文字列の配列を渡せます。
    'NOTE: oMaskObjには展開済みパス文字列（"*"は長さだけ比較の意）またはデータ文字列の配列を渡せます。
    'NOTE: oEvaluandObjのファイルが存在しないことを確認するには、oCompObjで""を渡します。
    Public Shared Function IsMatchCsv(ByVal oEvaluandObj As Object, ByVal oCompObj As Object, ByVal oMaskObj As Object, ByVal evaluationLen As Integer, Optional ByVal sDefaultBasePath As String = Nothing) As Boolean
        Dim oCompFields As String()
        If oCompObj.GetType() Is GetType(String) Then
            Dim sCompObj As String = DirectCast(oCompObj, String)
            If sCompObj.Equals("*", StringComparison.Ordinal) Then
                If evaluationLen < 0 Then
                    '常に一致とする。
                    Return True
                ElseIf evaluationLen = 0 Then
                    '項目数が0以上なら（つまり、ファイルが存在してさえいれば）一致とする。
                    If oEvaluandObj.GetType() Is GetType(String) Then
                        Dim sEvaluandObj As String = DirectCast(oEvaluandObj, String)
                        Return (sEvaluandObj.Length <> 0 AndAlso File.Exists(sEvaluandObj))
                    Else
                        Return True
                    End If
                Else
                    '項目数がevaluationLen以上なら一致とする。
                    If oEvaluandObj.GetType() Is GetType(String) Then
                        Dim sEvaluandObj As String = DirectCast(oEvaluandObj, String)
                        If sEvaluandObj.Length <> 0 AndAlso File.Exists(sEvaluandObj) Then
                            Dim oFields As String() = GetFieldsFromPathString(sEvaluandObj, sDefaultBasePath)
                            Return (oFields.Length >= evaluationLen)
                        Else
                            Return False
                        End If
                    Else
                        Return (DirectCast(oEvaluandObj, String()).Length >= evaluationLen)
                    End If
                End If
            End If

            If sCompObj.Length = 0 Then
                '存在しないことを確認する。
                If oEvaluandObj.GetType() Is GetType(String) Then
                    Dim sEvaluandObj As String = DirectCast(oEvaluandObj, String)
                    Return (sEvaluandObj.Length = 0 OrElse Not File.Exists(sEvaluandObj))
                Else
                    Return False
                End If
            End If
            oCompFields = GetFieldsFromPathString(sCompObj, sDefaultBasePath)
        Else
            oCompFields = DirectCast(oCompObj, String())
        End If

        Dim oEvaluandFields As String()
        If oEvaluandObj.GetType() Is GetType(String) Then
            oEvaluandFields = GetFieldsFromPathString(DirectCast(oEvaluandObj, String), sDefaultBasePath)
        Else
            oEvaluandFields = DirectCast(oEvaluandObj, String())
        End If

        Dim oMaskFields As String()
        If oMaskObj.GetType() Is GetType(String) Then
            Dim sMaskObj As String = DirectCast(oMaskObj, String)
            If sMaskObj.Equals("*", StringComparison.Ordinal) Then
                If evaluationLen < 0  Then
                    Return (oEvaluandFields.Length = oCompFields.Length)
                Else
                    Return (oEvaluandFields.Length >= evaluationLen)
                End If
            End If
            oMaskFields = GetFieldsFromPathString(sMaskObj, sDefaultBasePath)
        Else
            oMaskFields = DirectCast(oMaskObj, String())
        End If

        If evaluationLen < 0 Then
            evaluationLen = oEvaluandFields.Length
            If oCompFields.Length <> evaluationLen Then
                Return False
            End If
        Else
            If oEvaluandFields.Length < evaluationLen Then
                Return False
            End If
            If oCompFields.Length < evaluationLen Then
                Return False
            End If
        End If

        If oMaskFields.Length < evaluationLen Then
            evaluationLen = oMaskFields.Length
        End If

        Dim lastIndex As Integer = evaluationLen - 1
        For i As Integer = 0 To lastIndex
            Dim n As Integer = Integer.Parse(oMaskFields(i))
            If n < 0 Then
                If String.CompareOrdinal(oEvaluandFields(i), oCompFields(i)) <> 0 Then Return False
            Else
                If oEvaluandFields(i).Length < n Then Return False
                If String.CompareOrdinal(oEvaluandFields(i), 0, oCompFields(i), 0, n) <> 0 Then Return False
            End If
        Next i

        Return True
    End Function

    Public Shared Function IsAsciiString(ByVal sText As String) As Boolean
        For i As Integer = 0 To sText.Length - 1
            Dim c As Integer = AscW(sText.Chars(i))
            If c > &H7F Then Return False
        Next i
        Return True
    End Function

    Public Shared Function GetBytesFromHyphenatedHexadecimalString(ByVal src As String) As Byte()
        Return GetBytesFromHyphenatedHexadecimalString(src, 0, src.Length)
    End Function

    Public Shared Function GetBytesFromHyphenatedHexadecimalString(ByVal src As String, ByVal pos As Integer, ByVal len As Integer) As Byte()
        len += 1
        If len Mod 3 <> 0 Then
            Throw New ArgumentException("The src length does not fit into byte array.")
        End If

        Dim max As Integer = len \ 3 - 1
        Dim bytes As Byte() = New Byte(max) {}
        For i As Integer = 0 To max
            Dim hi As Integer
            Dim c As Integer = AscW(src.Chars(pos))
            If c >= &H30 AndAlso c <= &H39 Then
                hi = c - &H30
            ElseIf c >= &H41 AndAlso c <= &H46 Then
                hi = c - &H37
            ElseIf c >= &H61 AndAlso c <= &H66 Then
                hi = c - &H57
            Else
                Throw New ArgumentException("A char of src(" & pos.ToString() & ") does not fit into byte array.")
            End If
            pos += 1

            Dim lo As Integer
            c = AscW(src.Chars(pos))
            If c >= &H30 AndAlso c <= &H39 Then
                lo = c - &H30
            ElseIf c >= &H41 AndAlso c <= &H46 Then
                lo = c - &H37
            ElseIf c >= &H61 AndAlso c <= &H66 Then
                lo = c - &H57
            Else
                Throw New ArgumentException("A char of src(" & pos.ToString() & ") does not fit into byte array.")
            End If
            pos += 1

            bytes(i) = CByte(hi << 4 Or lo)

            If i <> max Then
                c = AscW(src.Chars(pos))
                If c <> &H2D Then
                    Throw New ArgumentException("A char of src(" & pos.ToString() & ") does not fit into byte array.")
                End If
                pos += 1
            End If
        Next

        Return bytes
    End Function

    'NOTE: 「!」は特殊文字です。「!」に続く２文字は16進のASCIIコードとみなします。
    'よって、区切りではない空白は「!20」、!記号は「!21」で記述することができます。
    '「!」に続く２文字が16進数とみなせない場合はFormatExceptionをスローします。
    '「!」に続く２文字の16進数がASCIIコードとみなせない場合もFormatExceptionをスローします。
    Public Shared Function GetFieldsFromSpaceDelimitedString(ByVal s As String) As String()
        Dim oRet As String() = s.Split(" "c)
        Dim oBuilder As StringBuilder = Nothing
        For iField As Integer = 0 To oRet.Length - 1
            Dim sField As String = oRet(iField)
            Dim i As Integer = sField.IndexOf("!"c)
            If i = -1 Then Continue For

            If oBuilder Is Nothing Then
                oBuilder = New StringBuilder()
            Else
                oBuilder.Length = 0
            End If

            Dim len As Integer = sField.Length
            Dim startPos As Integer = 0
            Do
                oBuilder.Append(sField.Substring(startPos, i - startPos))
                If i + 2 >= len Then Throw New FormatException("Halfway escape sequence detected in index " & i & " of following field." & vbCrLf & sField)
                Dim hi As Integer = GetIntFromHexChar(sField.Chars(i + 1))
                Dim lo As Integer = GetIntFromHexChar(sField.Chars(i + 2))
                If hi = -1 OrElse lo = -1 Then Throw New FormatException("Illegal escape sequence detected in index " & i & " of following field." & vbCrLf & sField)
                Dim code As Integer = hi << 4 Or lo
                If code > 127 Then Throw New FormatException("Illegal escape sequence detected in index " & i & " of following field." & vbCrLf & sField)
                oBuilder.Append(ChrW(code))

                startPos = i + 3
                i = sField.IndexOf("!"c, startPos)
                If i = -1 Then
                    oBuilder.Append(sField.Substring(startPos))
                    Exit Do
                End If
            Loop
            oRet(iField) = oBuilder.ToString()
        Next iField
        Return oRet
    End Function

    Private Shared Function GetIntFromHexChar(ByVal c As Char) As Integer
        Dim i As Integer = AscW(c)
        If i >= AscW("0"c) AndAlso i <= AscW("9"c) Then Return i - AscW("0"c)
        If i >= AscW("A"c) AndAlso i <= AscW("F"c) Then Return i - (AscW("A"c) - 10)
        If i >= AscW("a"c) AndAlso i <= AscW("f"c) Then Return i - (AscW("a"c) - 10)
        Return -1
    End Function

    'NOTE: ファイルが読み取り排他でオープンされている場合は、IOExceptionをスローします。
    Public Shared Function GetBytesFromPathString(ByVal s As String, ByVal sDefaultBasePath As String) As Byte()
        If s.StartsWith("Bytes:", StringComparison.OrdinalIgnoreCase) Then
            Dim preLen As Integer = "Bytes:".Length
            Return MyUtility.GetBytesFromHyphenatedHexadecimalString(s, preLen, s.Length - preLen)
        Else
            If sDefaultBasePath IsNot Nothing AndAlso Not Path.IsPathRooted(s) Then
                s = Path.Combine(sDefaultBasePath, s)
            End If

            Dim oBytes As Byte()
            Using oInputStream As New FileStream(s, FileMode.Open, FileAccess.Read)
                'ファイルのレングスを取得する。
                Dim len As Integer = CInt(oInputStream.Length)
                'ファイルを読み込む。
                oBytes = New Byte(len - 1) {}
                Dim pos As Integer = 0
                Do
                    Dim readSize As Integer = oInputStream.Read(oBytes, pos, len - pos)
                    If readSize = 0 Then Exit Do
                    pos += readSize
                Loop
            End Using
            Return oBytes
        End If
    End Function

    'NOTE: ファイルが読み取り排他でオープンされている場合は、IOExceptionをスローします。TODO: 左記の動作を確認する。
    'NOTE: sに「Fields:」で始まる空白区切り文字列を渡す場合、それ以降の「!」は特殊文字です。
    '「!」に続く２文字は16進のASCIIコードとみなします。
    'よって、区切りではない空白は「!20」、!記号は「!21」で記述することができます。
    '「!」に続く２文字が16進数とみなせない場合はFormatExceptionをスローします。
    '「!」に続く２文字の16進数がASCIIコードとみなせない場合もFormatExceptionをスローします。
    Public Shared Function GetFieldsFromPathString(ByVal s As String, ByVal sDefaultBasePath As String) As String()
        If s.StartsWith("Fields:", StringComparison.OrdinalIgnoreCase) Then
            Dim preLen As Integer = "Fields:".Length
            Return GetFieldsFromSpaceDelimitedString(s.Substring(preLen))
        Else
            If sDefaultBasePath IsNot Nothing AndAlso Not Path.IsPathRooted(s) Then
                s = Path.Combine(sDefaultBasePath, s)
            End If

            Dim oCsvFields As String()
            Using parser As New TextFieldParser(s, Encoding.GetEncoding(932))
                parser.TrimWhiteSpace = False
                parser.Delimiters = New String() {","}
                parser.ReadLine()
                oCsvFields = parser.ReadFields()
            End Using
            Return oCsvFields
        End If
    End Function

    Public Shared Sub CopyFileIfNeeded(ByVal sSrcPath As String, ByVal sDstPath As String, ByVal overwrite As Boolean)
        If Not Path.GetFullPath(sSrcPath).Equals(Path.GetFullPath(sDstPath), StringComparison.OrdinalIgnoreCase) Then
            File.Copy(sSrcPath, sDstPath, overwrite)
        End If
    End Sub

    Public Shared Function GetTextWidth(ByVal s As String, ByVal fnt As Font) As Integer
        Dim canvas As New Bitmap(10, 10)
        Dim g As Graphics = Graphics.FromImage(canvas)
        Dim sf As New StringFormat()
        g.DrawString(s, fnt, Brushes.Black, 0, 0, sf)
        Dim stringSize As SizeF = g.MeasureString(s, fnt, 1000, sf)
        sf.Dispose()
        g.Dispose()
        Return CInt(Math.Ceiling(stringSize.Width))
    End Function

    'NOTE: 未使用
    Public Shared Function GetFocusedControl(ByVal parentControl As Control) As Control
        Dim c As Control
        For Each c In parentControl.Controls
            If c.Focused Then
                Return c
            End If
            If c.ContainsFocus Then
                Dim fc As Control = GetFocusedControl(c)
                If Not (fc Is Nothing) Then
                    Return fc
                End If
            End If
        Next
        Return Nothing
    End Function

End Class


Public Module IEnumerableExtensions

    <System.Runtime.CompilerServices.Extension()> _
    Public Function IndexOf(Of T)(ByVal source As IEnumerable(Of T), ByVal list As IList(Of T)) As Integer
        If list.Count = 0 Then Return 0

        Dim index As Integer = 0
        For Each item As T In source
            If item IsNot Nothing AndAlso item.Equals(list(0)) Then
                Dim part As IEnumerable(Of T) = source.Skip(index).Take(list.Count)
                If part.SequenceEqual(list) Then Return index
            End If
            index += 1
        Next item
        Return -1
    End Function

    '<System.Runtime.CompilerServices.Extension()> _
    'Public Function IndexOf(Of T)(ByVal source As IEnumerable(Of T), ByVal list As IList(Of T), ByVal startIndex As Integer) As Integer
    '    If list.Count = 0 Then Return startIndex

    '    For index As Integer = startIndex To source.Count - 1
    '        Dim item As T = source(index)
    '        If item IsNot Nothing AndAlso item.Equals(list(0)) Then
    '            Dim part As IEnumerable(Of T) = source.Skip(index).Take(list.Count)
    '            If part.SequenceEqual(list) Then Return index
    '        End If
    '    Next index
    '    Return -1
    'End Function

    '<System.Runtime.CompilerServices.Extension()> _
    'Public Function IndexOf(Of T)(ByVal source As IEnumerable(Of T), ByVal list As IList(Of T), ByVal startIndex As Integer, ByVal count As Integer) As Integer
    '    If list.Count = 0 Then Return startIndex

    '    For index As Integer = startIndex To startIndex + count - 1
    '        Dim item As T = source(index)
    '        If item IsNot Nothing AndAlso item.Equals(list(0)) Then
    '            Dim part As IEnumerable(Of T) = source.Skip(index).Take(list.Count)
    '            If part.SequenceEqual(list) Then Return index
    '        End If
    '    Next index
    '    Return -1
    'End Function

End Module
