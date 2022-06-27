' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/06/10  (NES)小林  新規作成
'   0.1      2017/08/08  (NES)小林  ElementFormatにA形式を追加
'   0.2      2017/11/21  (NES)小林  ElementFormatにS形式を追加
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.Text

Public Enum XlsByteOrder
    BigEndian
    LittleEndian
End Enum


Public Class XlsField

    'TODO: このクラスのインスタンスをスレッドセーフにしたい場合は、
    'oBuilderやoWorkBytesを各メソッドのローカル変数にして、
    'StringBuilderオブジェクトやByte配列は都度作成すること。
    Private elemBits As Integer
    Private elemByteOrder As XlsByteOrder
    Private elemFormat As String
    Private elemFormatRadix As Integer
    Private elemFormatOption As Integer
    Private elemCount As Integer
    Private sep As Char
    Private _metaName As String
    Private _metaType As String
    Private oBuilder As StringBuilder
    Private oWorkBytes As Byte()
    Private oEnc As Encoding

    Public ReadOnly Property ElementBits() As Integer
        Get
            Return elemBits
        End Get
    End Property

    Public ReadOnly Property ElementByteOrder() As XlsByteOrder
        Get
            Return elemByteOrder
        End Get
    End Property

    Public ReadOnly Property ElementFormat() As String
        Get
            Return elemFormat
        End Get
    End Property

    Public ReadOnly Property ElementCount() As Integer
        Get
            Return elemCount
        End Get
    End Property

    Public ReadOnly Property Separator() As Char
        Get
            Return sep
        End Get
    End Property

    Public ReadOnly Property MetaName() As String
        Get
            Return _metaName
        End Get
    End Property

    Public ReadOnly Property MetaType() As String
        Get
            Return _metaType
        End Get
    End Property

    Public Sub New(ByVal elemBits As Integer, ByVal elemFormat As String, ByVal elemCount As Integer, ByVal sep As Char, ByVal metaName As String, Optional ByVal metaType As String = Nothing, Optional ByVal elemByteOrder As XlsByteOrder = XlsByteOrder.BigEndian)
        If elemFormat Is Nothing Then
            Throw New ArgumentNullException("elemFormat")
        End If

        If elemBits <= 0 Then
            Throw New ArgumentException("ビット数は正数でなければなりません。", "elemBits")
        End If

        If elemFormat.Length = 0 Then
            Throw New ArgumentException("書式指定文字列には少なくとも底記号が必要です。", "elemFormat")
        End If

        If elemFormat.ToUpper().StartsWith("A") Then
            If elemBits Mod 8 <> 0 Then
                Throw New ArgumentException("A形式でのビット数は8の倍数でなければなりません。", "elemBits")
            End If
            If elemCount <> 1 Then
                Throw New ArgumentException("A形式での要素数は1でなければなりません。", "elemCount")
            End If
            elemFormatRadix = 0
        ElseIf elemFormat.ToUpper().StartsWith("S") Then
            If elemBits Mod 8 <> 0 Then
                Throw New ArgumentException("S形式でのビット数は8の倍数でなければなりません。", "elemBits")
            End If
            If elemCount <> 1 Then
                Throw New ArgumentException("S形式での要素数は1でなければなりません。", "elemCount")
            End If
            elemFormatRadix = -1
        ElseIf elemFormat.ToUpper().StartsWith("D") Then
            If elemBits > 64 Then
                Throw New ArgumentException("D形式でのビット数は64以下でなければなりません。", "elemBits")
            End If
            elemFormatRadix = 10
        ElseIf elemFormat.ToUpper().StartsWith("X") Then
            elemFormatRadix = 16
        Else
            Throw New ArgumentException("書式指定文字列の底記号が不正です。", "elemFormat")
        End If

        If elemCount < 1 Then
            Throw New ArgumentException("要素数は1以上でなければなりません。", "elemCount")
        ElseIf elemCount > 16777216 Then
            Throw New ArgumentException("要素数は16777216以下でなければなりません。", "elemCount")
        End If

        If CLng(elemCount) * elemBits > CLng(Integer.MaxValue) * 8 Then
            Throw New ArgumentException("要素の合計サイズが大きすぎます。")
        End If

        If elemFormatRadix = 0 Then
            If elemFormat.Length = 1 Then
                elemFormatOption = -1
            ElseIf elemFormat.Length = 3 Then
                If Integer.TryParse(elemFormat.Substring(1), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, elemFormatOption) = False Then
                    Throw New ArgumentException("A形式書式指定文字列のオプションは2文字の16進数でなければなりません。", "elemFormat")
                End If
            Else
                Throw New ArgumentException("A形式書式指定文字列のオプションは2文字の16進数でなければなりません。", "elemFormat")
            End If
        ElseIf elemFormatRadix = -1 Then
            If elemFormat.Length = 1 Then
                elemFormatOption = 0
            Else
                If Integer.TryParse(elemFormat.Substring(1), NumberStyles.None, CultureInfo.InvariantCulture, elemFormatOption) = False Then
                    Throw New ArgumentException("S形式書式指定文字列のオプションはコードページIDとして解釈可能でなければなりません。", "elemFormat")
                End If
            End If
            Try
                'TODO: 現状の実装は、ASCII上位互換のエンコーディングを想定している（UTF8やSJISやEUCが想定範囲であり、UTF16やJISは想定範囲外である）。
                oEnc = Encoding.GetEncoding(elemFormatOption, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
            Catch ex As Exception
                Throw New ArgumentException("S形式書式指定文字列のオプションはコードページIDとして解釈可能でなければなりません。", "elemFormat")
            End Try
        Else
            If elemFormat.Length = 1 Then
                elemFormatOption = 1
            Else
                If Integer.TryParse(elemFormat.Substring(1), NumberStyles.None, CultureInfo.InvariantCulture, elemFormatOption) = False Then
                    Throw New ArgumentException("書式指定文字列の桁数を解釈できません。", "elemFormat")
                End If
                If elemFormatOption = 0 Then
                    Throw New ArgumentException("書式指定文字列の桁数は1以上でなければなりません。", "elemFormat")
                End If
            End If
        End If

        Me.elemBits = elemBits
        Me.elemByteOrder = elemByteOrder
        Me.elemFormat = elemFormat
        Me.elemCount = elemCount
        Me.sep = sep
        _metaName = metaName
        _metaType = metaType
        oBuilder = New StringBuilder()
        If elemFormatRadix = -1 Then
            oWorkBytes = New Byte(elemBits \ 8 - 1) {}
        End If
    End Sub

    Public Function CreateFormatDescription() As String
        If elemCount = 1 Then
            Return elemFormat
        Else
            If sep = " "c Then
                Return elemFormat & " *" & elemCount.ToString()
            Else
                Return elemFormat & " *" & elemCount.ToString() & " with[" & sep & "]"
            End If
        End If
    End Function

    Public Function CreateDefaultValue() As String
        oBuilder.Length = 0
        If elemFormatRadix = 0 Then
            If elemFormatOption <= 0 Then
                For i As Integer = 1 To elemBits \ 8
                    oBuilder.Append("%00")
                Next i
            Else
                For i As Integer = 1 To elemBits \ 8
                    If elemFormatOption >= &H20 AndAlso elemFormatOption <= &H7E AndAlso elemFormatOption <> &H25 Then
                        oBuilder.Append(ChrW(elemFormatOption))
                    Else
                        oBuilder.Append("%"c)
                        oBuilder.Append(GetCharFromHalfByteA(CByte(elemFormatOption >> 4), elemFormat))
                        oBuilder.Append(GetCharFromHalfByteA(CByte(elemFormatOption And &HF), elemFormat))
                    End If
                Next i
            End If
        ElseIf elemFormatRadix = -1 Then
            '先頭から末尾までがヌル文字の文字列（表示上は１つの"\0"）をデフォルト値とする。
            oBuilder.Append("\0")
        Else
            For elem As Integer = 1 To elemCount
                If elem <> 1 Then
                    oBuilder.Append(sep)
                End If
                'OPT: ヒープを使わないようにする。
                oBuilder.Append(0.ToString(elemFormat))
            Next elem
        End If
        Return oBuilder.ToString()
    End Function

    Public Function CreateValueFromBytes(ByVal bytes As Byte(), Optional ByVal bitsOffset As Integer = 0) As String
        oBuilder.Length = 0

        'NOTE: bitPos の 0 は最上位ビット、7 は最下位ビットを指す。
        Dim bytePos As Integer = bitsOffset \ 8
        Dim bitPos As Integer = bitsOffset - bytePos * 8

        If elemFormatRadix = 0 Then
            For i As Integer = 1 To elemBits \ 8
                Dim b As Byte = bytes(bytePos) << bitPos
                bytePos += 1
                If bitPos <> 0 Then
                    b = b Or bytes(bytePos) >> (8 - bitPos)
                End If

                If b >= &H21 AndAlso b <= &H7E AndAlso b <> &H25 Then
                    oBuilder.Append(ChrW(b))
                ElseIf b = elemFormatOption AndAlso elemFormatOption = &H20 Then
                    oBuilder.Append(ChrW(b))
                Else
                    oBuilder.Append("%"c)
                    oBuilder.Append(GetCharFromHalfByteA(b >> 4, elemFormat))
                    oBuilder.Append(GetCharFromHalfByteA(CByte(b And &HF), elemFormat))
                End If
            Next i
        ElseIf elemFormatRadix = -1 Then
            Dim totalByteCount As Integer = elemBits \ 8

            For i As Integer = 0 To totalByteCount - 1
                Dim b As Byte = bytes(bytePos) << bitPos
                bytePos += 1
                If bitPos <> 0 Then
                    b = b Or bytes(bytePos) >> (8 - bitPos)
                End If

                oWorkBytes(i) = b
            Next i

            Dim startPos As Integer = 0
            Do
                Try
                    Dim s As String = oEnc.GetString(oWorkBytes, startPos, totalByteCount - startPos)

                    Dim lastIndex As Integer = s.Length - 1
                    While lastIndex >= 0
                        If Not s.Chars(lastIndex).Equals(Chr(0)) Then Exit While
                        lastIndex -= 1
                    End While

                    For j As Integer = 0 To lastIndex
                        Dim c As Char = s.Chars(j)
                        Select Case c
                            Case Chr(0)
                                oBuilder.Append("\0")
                            Case Chr(9)
                                oBuilder.Append("\t")
                            Case Chr(10)
                                oBuilder.Append("\n")
                            Case Chr(12)
                                oBuilder.Append("\f")
                            Case Chr(13)
                                oBuilder.Append("\r")
                            Case Chr(92)
                                oBuilder.Append("\\")
                            Case Else
                                oBuilder.Append(c)
                        End Select
                    Next j
                    Exit Do
                Catch ex As DecoderFallbackException
                    If ex.Index <> 0 Then
                        Dim s As String = oEnc.GetString(oWorkBytes, startPos, ex.Index)
                        Dim lastIndex As Integer = s.Length - 1
                        For j As Integer = 0 To lastIndex
                            Dim c As Char = s.Chars(j)
                            Select Case c
                                Case Chr(0)
                                    oBuilder.Append("\0")
                                Case Chr(9)
                                    oBuilder.Append("\t")
                                Case Chr(10)
                                    oBuilder.Append("\n")
                                Case Chr(12)
                                    oBuilder.Append("\f")
                                Case Chr(13)
                                    oBuilder.Append("\r")
                                Case Chr(92)
                                    oBuilder.Append("\\")
                                Case Else
                                    oBuilder.Append(c)
                            End Select
                        Next j
                    End If
                    For Each unknown As Byte In ex.BytesUnknown
                        oBuilder.AppendFormat("\x{0:x2}", unknown)
                    Next
                    startPos += ex.Index + ex.BytesUnknown.Length
                End Try
            Loop While startPos < totalByteCount
            oBuilder.Append("\0")
        Else
            For elem As Integer = 1 To elemCount
                If elem <> 1 Then
                    oBuilder.Append(sep)
                End If

                If elemFormatRadix = 16 Then
                    If elemByteOrder = XlsByteOrder.BigEndian Then
                        Dim extractiveChars As Integer = (elemBits + 3) \ 4
                        If extractiveChars > elemFormatOption Then
                            Dim needsChar As Boolean = False
                            Dim restBits As Integer = elemBits
                            Dim headBits As Integer = elemBits Mod 4
                            If headBits <> 0 Then
                                Dim b As Byte = bytes(bytePos) << bitPos
                                bitPos += headBits
                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        b = b Or bytes(bytePos) >> (headBits - bitPos)
                                    End If
                                End If
                                b = b >> (8 - headBits)

                                'OPT: この時点では必ず「needsChar = False」かつ「extractiveChars > elemFormatOption」
                                'であるため、下記の処理は簡略化可能。
                                If Not needsChar Then
                                    If b <> 0 OrElse extractiveChars <= elemFormatOption Then
                                        needsChar = True
                                    End If
                                End If
                                If needsChar Then
                                    oBuilder.Append(GetCharFromHalfByteX(b, elemFormat))
                                End If
                                extractiveChars -= 1
                                restBits -= headBits
                            End If
                            While restBits <> 0
                                Dim b As Byte = bytes(bytePos) << bitPos
                                bitPos += 4
                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        b = b Or bytes(bytePos) >> (4 - bitPos)
                                    End If
                                End If
                                b = b >> 4

                                If Not needsChar Then
                                    If b <> 0 OrElse extractiveChars <= elemFormatOption Then
                                        needsChar = True
                                    End If
                                End If
                                If needsChar Then
                                    oBuilder.Append(GetCharFromHalfByteX(b, elemFormat))
                                End If
                                extractiveChars -= 1
                                restBits -= 4
                            End While
                        Else
                            For i As Integer = extractiveChars + 1 To elemFormatOption
                                oBuilder.Append("0"c)
                            Next i

                            Dim restBits As Integer = elemBits
                            Dim headBits As Integer = elemBits Mod 4
                            If headBits <> 0 Then
                                Dim b As Byte = bytes(bytePos) << bitPos
                                bitPos += headBits
                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        b = b Or bytes(bytePos) >> (headBits - bitPos)
                                    End If
                                End If
                                oBuilder.Append(GetCharFromHalfByteX(b >> (8 - headBits), elemFormat))
                                restBits -= headBits
                            End If
                            While restBits <> 0
                                Dim b As Byte = bytes(bytePos) << bitPos
                                bitPos += 4
                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        b = b Or bytes(bytePos) >> (4 - bitPos)
                                    End If
                                End If
                                oBuilder.Append(GetCharFromHalfByteX(b >> 4, elemFormat))
                                restBits -= 4
                            End While
                        End If
                    Else
                        Dim bytePosNext As Integer
                        Dim bitPosNext As Integer
                        If elemBits >= 8 - bitPos Then
                            Dim elemSmallBits As Integer = elemBits - (8 - bitPos)
                            bitPosNext = elemSmallBits Mod 8
                            bytePosNext = bytePos + (elemSmallBits + 1 + 7) \ 8
                        Else
                            bitPosNext = bitPos + elemBits
                            bytePosNext = bytePos
                        End If

                        bytePos = bytePosNext
                        bitPos = bitPosNext - 1
                        If bitPos < 0 Then
                            bytePos -= 1
                            bitPos += 8
                        End If

                        Dim extractiveByteCount As Integer = (elemBits + 7) \ 8
                        If extractiveByteCount * 2 > elemFormatOption Then
                            Dim needsChar As Boolean = False
                            Dim extractiveChars As Integer = (elemBits + 3) \ 4
                            Dim restBits As Integer = elemBits
                            Dim headBits As Integer = elemBits Mod 8
                            If headBits <> 0 Then
                                Dim b As Byte
                                If bitPos + 1 <= headBits Then
                                    b = bytes(bytePos) >> (7 - bitPos) << (headBits - (bitPos + 1))
                                    bytePos -= 1
                                    If bitPos + 1 < headBits Then
                                        b = b Or CByte(bytes(bytePos) And (1 << (headBits - (bitPos + 1))) - 1)
                                    End If
                                    bitPos += 8 - headBits
                                Else
                                    b = bytes(bytePos) << (bitPos + 1 - headBits) >> (8 - headBits)
                                End If

                                Dim hb As Byte

                                'OPT: この時点では必ず「needsChar = False」であるため、簡略化可能。
                                hb = b >> 4
                                If Not needsChar Then
                                    If hb <> 0 OrElse extractiveChars <= elemFormatOption Then
                                        needsChar = True
                                    End If
                                End If
                                If needsChar Then
                                    oBuilder.Append(GetCharFromHalfByteX(hb, elemFormat))
                                End If
                                extractiveChars -= 1

                                hb = CByte(b And &HF)
                                If Not needsChar Then
                                    If hb <> 0 OrElse extractiveChars <= elemFormatOption Then
                                        needsChar = True
                                    End If
                                End If
                                If needsChar Then
                                    oBuilder.Append(GetCharFromHalfByteX(hb, elemFormat))
                                End If
                                extractiveChars -= 1

                                restBits -= headBits
                            End If
                            While restBits <> 0
                                Dim b As Byte
                                b = bytes(bytePos) >> (7 - bitPos) << (7 - bitPos)
                                bytePos -= 1
                                If bitPos <> 7 Then
                                    b = b Or CByte(bytes(bytePos) And (1 << (7 - bitPos)) - 1)
                                End If

                                Dim hb As Byte

                                hb = b >> 4
                                If Not needsChar Then
                                    If hb <> 0 OrElse extractiveChars <= elemFormatOption Then
                                        needsChar = True
                                    End If
                                End If
                                If needsChar Then
                                    oBuilder.Append(GetCharFromHalfByteX(hb, elemFormat))
                                End If
                                extractiveChars -= 1

                                hb = CByte(b And &HF)
                                If Not needsChar Then
                                    If hb <> 0 OrElse extractiveChars <= elemFormatOption Then
                                        needsChar = True
                                    End If
                                End If
                                If needsChar Then
                                    oBuilder.Append(GetCharFromHalfByteX(hb, elemFormat))
                                End If
                                extractiveChars -= 1

                                restBits -= 8
                            End While
                        Else
                            For i As Integer = extractiveByteCount * 2 + 1 To elemFormatOption
                                oBuilder.Append("0"c)
                            Next i

                            Dim restBits As Integer = elemBits
                            Dim headBits As Integer = elemBits Mod 8
                            If headBits <> 0 Then
                                Dim b As Byte
                                If bitPos + 1 <= headBits Then
                                    b = bytes(bytePos) >> (7 - bitPos) << (headBits - (bitPos + 1))
                                    bytePos -= 1
                                    If bitPos + 1 < headBits Then
                                        b = b Or CByte(bytes(bytePos) And (1 << (headBits - (bitPos + 1))) - 1)
                                    End If
                                    bitPos += 8 - headBits
                                Else
                                    b = bytes(bytePos) << (bitPos + 1 - headBits) >> (8 - headBits)
                                End If
                                oBuilder.Append(GetCharFromHalfByteX(b >> 4, elemFormat))
                                oBuilder.Append(GetCharFromHalfByteX(CByte(b And &HF), elemFormat))
                                restBits -= headBits
                            End If
                            While restBits <> 0
                                Dim b As Byte
                                b = bytes(bytePos) >> (7 - bitPos) << (7 - bitPos)
                                bytePos -= 1
                                If bitPos <> 7 Then
                                    b = b Or CByte(bytes(bytePos) And (1 << (7 - bitPos)) - 1)
                                End If
                                oBuilder.Append(GetCharFromHalfByteX(b >> 4, elemFormat))
                                oBuilder.Append(GetCharFromHalfByteX(CByte(b And &HF), elemFormat))
                                restBits -= 8
                            End While
                        End If

                        bytePos = bytePosNext
                        bitPos = bitPosNext
                    End If
                Else
                    Dim b64 As UInt64 = 0

                    Dim validBytes As Integer = (elemBits + 7) \ 8
                    Dim restBits As Integer = elemBits
                    Dim headBits As Integer = elemBits - (validBytes - 1) * 8
                    If elemByteOrder = XlsByteOrder.BigEndian Then
                        If headBits <> 0 Then
                            Dim b As Byte = bytes(bytePos) << bitPos
                            bitPos += headBits
                            If bitPos >= 8 Then
                                bytePos += 1
                                bitPos -= 8
                                If bitPos <> 0 Then
                                    b = b Or bytes(bytePos) >> (headBits - bitPos)
                                End If
                            End If
                            b64 = b >> (8 - headBits)
                            restBits -= headBits
                        End If
                        While restBits <> 0
                            Dim b As Byte = bytes(bytePos) << bitPos
                            bytePos += 1
                            If bitPos <> 0 Then
                                b = b Or bytes(bytePos) >> (8 - bitPos)
                            End If
                            b64 = b64 << 8 Or b
                            restBits -= 8
                        End While
                    Else
                        'OPT: この書式の場合、bytes の後方から各 Byte を読む必要はない。
                        'bytes の前方から読んだ各 Byte を b64 の LSB 側に Or してゆけばよい。

                        Dim bytePosNext As Integer
                        Dim bitPosNext As Integer
                        If elemBits >= 8 - bitPos Then
                            Dim elemSmallBits As Integer = elemBits - (8 - bitPos)
                            bitPosNext = elemSmallBits Mod 8
                            bytePosNext = bytePos + (elemSmallBits + 1 + 7) \ 8
                        Else
                            bitPosNext = bitPos + elemBits
                            bytePosNext = bytePos
                        End If

                        bytePos = bytePosNext
                        bitPos = bitPosNext - 1
                        If bitPos < 0 Then
                            bytePos -= 1
                            bitPos += 8
                        End If

                        If headBits <> 0 Then
                            Dim b As Byte
                            If bitPos + 1 <= headBits Then
                                b = bytes(bytePos) >> (7 - bitPos) << (headBits - (bitPos + 1))
                                bytePos -= 1
                                If bitPos + 1 < headBits Then
                                    b = b Or CByte(bytes(bytePos) And (1 << (headBits - (bitPos + 1))) - 1)
                                End If
                                bitPos += 8 - headBits
                            Else
                                b = bytes(bytePos) << (bitPos + 1 - headBits) >> (8 - headBits)
                            End If
                            b64 = b
                            restBits -= headBits
                        End If
                        While restBits <> 0
                            Dim b As Byte
                            b = bytes(bytePos) >> (7 - bitPos) << (7 - bitPos)
                            bytePos -= 1
                            If bitPos <> 7 Then
                                b = b Or CByte(bytes(bytePos) And (1 << (7 - bitPos)) - 1)
                            End If
                            b64 = b64 << 8 Or b
                            restBits -= 8
                        End While

                        bytePos = bytePosNext
                        bitPos = bitPosNext
                    End If

                    Dim validChars As Integer = 1
                    Dim pow As UInt64 = 1
                    Dim b64per10 As UInt64 = b64 \ 10UL
                    While pow <= b64per10
                        pow = pow * 10UL
                        validChars += 1
                    End While

                    For i As Integer = validChars + 1 To elemFormatOption
                        oBuilder.Append("0"c)
                    Next i

                    For i As Integer = 1 To validChars
                        Dim n As Integer = CInt(b64 \ pow)
                        oBuilder.Append(ChrW(AscW("0") + n))
                        'OPT: Decimalを介すことにならないなら「b64 = b64 - n * pow」がよい。
                        b64 = b64 Mod pow
                        pow = pow \ 10UL
                    Next i
                End If
            Next elem
        End If

        Return oBuilder.ToString()
    End Function

    Public Sub CopyValueToBytes(ByVal value As String, ByVal bytes As Byte(), Optional ByVal bitsOffset As Integer = 0)
        'NOTE: bitPos の 0 は最上位ビット、7 は最下位ビットを指す。
        Dim valueLen As Integer = value.Length
        Dim i As Integer = 0
        Dim bytePos As Integer = bitsOffset \ 8
        Dim bitPos As Integer = bitsOffset - bytePos * 8

        If elemFormatRadix = 0 Then
            For bi As Integer = 1 To elemBits \ 8
                Dim b As Integer
                If i >= valueLen Then
                    If elemFormatOption = -1 Then
                        Throw New ArgumentException("値を構成する文字が不足しています。")
                    Else
                        b = elemFormatOption
                    End If
                Else
                    Dim c As Char = value.Chars(i)
                    i += 1

                    If c = "%"c Then
                        If i + 1 >= valueLen Then
                            Throw New ArgumentException(i.ToString() & "文字目が不正です。""%""に続く2文字が16進数とみなせません。")
                        End If

                        c = value.Chars(i)
                        i += 1

                        Dim up4 As Integer = GetIntFromHexChar(c)
                        If up4 = -1 Then
                            Throw New ArgumentException(i.ToString() & "文字目が不正です。""%""に続く2文字が16進数とみなせません。")
                        End If

                        c = value.Chars(i)
                        i += 1

                        Dim lo4 As Integer = GetIntFromHexChar(c)
                        If lo4 = -1 Then
                            Throw New ArgumentException(i.ToString() & "文字目が不正です。""%""に続く2文字が16進数とみなせません。")
                        End If

                        b = up4 << 4 Or lo4
                    Else
                        b = AscW(c)
                        If b < 0 OrElse b > 255 Then
                            Throw New ArgumentException(i.ToString() & "文字目が不正です。")
                        End If
                    End If
                End If

                If bitPos = 0 Then
                    bytes(bytePos) = CByte(b)
                    bytePos += 1
                Else
                    bytes(bytePos) = bytes(bytePos) Or CByte(b >> bitPos)
                    bytePos += 1
                    bytes(bytePos) = CByte(b << (8 - bitPos) And &HFF)
                End If
            Next bi

            While i < valueLen
                If elemFormatOption = -1 OrElse AscW(value.Chars(i)) <> elemFormatOption Then
                    Throw New ArgumentException("値を構成する文字が多すぎます。")
                End If
                i += 1
            End While
        ElseIf elemFormatRadix = -1 Then
            Dim wbiMax As Integer = elemBits \ 8 - 1
            Dim wbi As Integer = 0
            While i < valueLen
                Dim b As Integer

                Dim c As Char = value.Chars(i)
                i += 1

                If c = "\"c Then
                    If i >= valueLen Then
                        Throw New ArgumentException(i.ToString() & "文字目が不正です。""\""に続く文字がありません。")
                    End If
                    c = value.Chars(i)
                    i += 1

                    Select Case c
                        Case "0"c
                            b = 0
                        Case "t"c
                            b = 9
                        Case "n"c
                            b = 10
                        Case "f"c
                            b = 12
                        Case "r"c
                            b = 13
                        Case "\"c
                            b = 92
                        Case "x"c
                            If i + 1 >= valueLen Then
                                Throw New ArgumentException(i.ToString() & "文字目が不正です。""\x""に続く2文字が16進数とみなせません。")
                            End If
                            c = value.Chars(i)
                            i += 1

                            Dim up4 As Integer = GetIntFromHexChar(c)
                            If up4 = -1 Then
                                Throw New ArgumentException(i.ToString() & "文字目が不正です。""\x""に続く2文字が16進数とみなせません。")
                            End If

                            c = value.Chars(i)
                            i += 1

                            Dim lo4 As Integer = GetIntFromHexChar(c)
                            If lo4 = -1 Then
                                Throw New ArgumentException(i.ToString() & "文字目が不正です。""\x""に続く2文字が16進数とみなせません。")
                            End If

                            b = up4 << 4 Or lo4
                        Case Else
                            Throw New ArgumentException(i.ToString() & "文字目が不正です。非対応のエスケープシーケンスです。")
                    End Select

                    If wbi > wbiMax Then
                        If wbi = wbiMax + 1 AndAlso b = 0 AndAlso i = valueLen Then Exit While
                        Throw New ArgumentException("値を構成する文字が多すぎます。")
                    End If

                    oWorkBytes(wbi) = CByte(b)
                    wbi += 1
                Else
                    Dim i2 As Integer = i
                    While i2 < valueLen
                        Dim c2 As Char = value.Chars(i2)
                        If c2 = "\"c Then Exit While
                        i2 += 1
                    End While

                    Try
                        wbi += oEnc.GetBytes(value, i - 1, i2 - (i - 1), oWorkBytes, wbi)
                        i = i2
                    Catch ex As EncoderFallbackException
                        Throw New ArgumentException((i + ex.Index).ToString() & "文字目が不正です。コードページに無い文字です。")
                    Catch ex As ArgumentException
                        Throw New ArgumentException("値を構成する文字が多すぎます。")
                    End Try
                End If
            End While

            For bi As Integer = 0 To wbiMax
                Dim b As Integer = 0
                If bi < wbi Then
                    b = oWorkBytes(bi)
                End If

                If bitPos = 0 Then
                    bytes(bytePos) = CByte(b)
                    bytePos += 1
                Else
                    bytes(bytePos) = bytes(bytePos) Or CByte(b >> bitPos)
                    bytePos += 1
                    bytes(bytePos) = CByte(b << (8 - bitPos) And &HFF)
                End If
            Next bi
        Else
            For elem As Integer = 1 To elemCount
                If i >= valueLen Then
                    Throw New ArgumentException("値を構成する要素が不足しています。")
                End If

                Dim k As Integer = value.IndexOf(sep, i)
                If k = -1 Then
                    k = valueLen
                End If

                Dim num As Integer = k - i
                If num = 0 Then
                    Throw New ArgumentException("値を構成する要素に空要素があります。")
                End If

                If elemFormatRadix = 16 Then
                    If elemByteOrder = XlsByteOrder.BigEndian Then
                        Dim restBits As Integer = elemBits
                        Dim idealNum As Integer = (elemBits + 3) \ 4
                        If num > idealNum Then
                            Do
                                If value.Chars(i) <> "0"c Then
                                    Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                                End If
                                i += 1
                                num -= 1
                            Loop While num > idealNum
                        ElseIf num < idealNum Then
                            Do
                                If bitPos = 0 Then
                                    bytes(bytePos) = 0
                                End If

                                If restBits + 4 > num * 4 Then
                                    bitPos += 4
                                    restBits -= 4
                                Else
                                    bitPos += restBits - num * 4
                                    restBits = num * 4
                                End If

                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        bytes(bytePos) = 0
                                    End If
                                End If

                                idealNum -= 1
                            Loop While num < idealNum
                        End If

                        Dim headBits As Integer = restBits Mod 4
                        If headBits <> 0 Then
                            Dim b As Integer = GetIntFromHexChar(value.Chars(i))
                            If b = -1 Then
                                Throw New ArgumentException(elem.ToString() & "番目の要素に不正な文字が含まれています。")
                            End If
                            i += 1

                            If b >= (1 << headBits) Then
                                Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                            End If

                            b = b << (8 - headBits)
                            If bitPos = 0 Then
                                bytes(bytePos) = CByte(b)
                                bitPos = headBits
                            Else
                                bytes(bytePos) = bytes(bytePos) Or CByte(b >> bitPos)
                                bitPos += headBits
                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        bytes(bytePos) = CByte(b << (headBits - bitPos) And &HFF)
                                    End If
                                End If
                            End If
                        End If

                        While i < k
                            Dim b As Integer = GetIntFromHexChar(value.Chars(i))
                            If b = -1 Then
                                Throw New ArgumentException(elem.ToString() & "番目の要素に不正な文字が含まれています。")
                            End If
                            i += 1

                            b = b << 4
                            If bitPos = 0 Then
                                bytes(bytePos) = CByte(b)
                                bitPos = 4
                            Else
                                bytes(bytePos) = bytes(bytePos) Or CByte(b >> bitPos)
                                bitPos += 4
                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        bytes(bytePos) = CByte(b << (4 - bitPos) And &HFF)
                                    End If
                                End If
                            End If
                        End While
                    Else
                        'If elemBits > 8 AndAlso (bitPos <> 0 OrElse elemBits Mod 8 <> 0)  Then
                        '    Throw New InvalidOperationException(elem.ToString() & "番目の要素はリトルエンディアンで複数バイトに格納しなければなりませんが、バイト境界に配置されません。")
                        'End If

                        Dim bytePosPrev As Integer = If(bitPos = 0, bytePos -  1, bytePos)

                        Dim bytePosNext As Integer
                        Dim bitPosNext As Integer
                        If elemBits >= 8 - bitPos Then
                            Dim elemSmallBits As Integer = elemBits - (8 - bitPos)
                            bitPosNext = elemSmallBits Mod 8
                            bytePosNext = bytePos + (elemSmallBits + 1 + 7) \ 8
                        Else
                            bitPosNext = bitPos + elemBits
                            bytePosNext = bytePos
                        End If

                        bytePos = bytePosNext
                        bitPos = bitPosNext - 1
                        If bitPos < 0 Then
                            bytePos -= 1
                            bitPos += 8
                        End If

                        Dim restBits As Integer = elemBits
                        Dim idealNum As Integer = (elemBits + 7) \ 8
                        num = (num + 1) \ 2
                        If num > idealNum Then
                            Do
                                If value.Chars(i) <> "0"c Then
                                    Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                                End If
                                i += 1

                                If (k - i) Mod 2 <> 0 Then
                                    If value.Chars(i) <> "0"c Then
                                        Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                                    End If
                                    i += 1
                                End If

                                num -= 1
                            Loop While num > idealNum
                        ElseIf num < idealNum Then
                            Do
                                If bytePos > bytePosPrev Then
                                    bytes(bytePos) = 0
                                End If
                                bytePos -= 1
                                idealNum -= 1
                            Loop While num < idealNum
                            restBits = num * 8
                        End If

                        Dim headBits As Integer = restBits Mod 8
                        If headBits <> 0 Then
                            Dim b As Integer = GetIntFromHexChar(value.Chars(i))
                            If b = -1 Then
                                Throw New ArgumentException(elem.ToString() & "番目の要素に不正な文字が含まれています。")
                            End If
                            i += 1

                            If (k - i) Mod 2 <> 0 Then
                                Dim b2 As Integer = GetIntFromHexChar(value.Chars(i))
                                If b2 = -1 Then
                                    Throw New ArgumentException(elem.ToString() & "番目の要素に不正な文字が含まれています。")
                                End If
                                i += 1

                                b = b << 4 Or b2
                            End If

                            If b >= (1 << headBits) Then
                                Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                            End If

                            b = b << (8 - headBits)
                            If bitPos = 7 Then
                                bytes(bytePos) = CByte(b)
                                bitPos -= headBits
                            ElseIf headBits <= bitPos + 1 Then
                                bytes(bytePos) = bytes(bytePos) Or CByte(b << (7 - bitPos))
                                bitPos -= headBits
                            Else
                                bytes(bytePos) = bytes(bytePos) Or CByte(b << (7 - bitPos))
                                bytePos -= 1
                                bytes(bytePos) = CByte(b >> (bitPos + 1))
                                bitPos += 8 - headBits
                            End If
                        End If

                        While i < k
                            Dim b As Integer = GetIntFromHexChar(value.Chars(i))
                            If b = -1 Then
                                Throw New ArgumentException(elem.ToString() & "番目の要素に不正な文字が含まれています。")
                            End If
                            i += 1

                            If (k - i) Mod 2 <> 0 Then
                                Dim b2 As Integer = GetIntFromHexChar(value.Chars(i))
                                If b2 = -1 Then
                                    Throw New ArgumentException(elem.ToString() & "番目の要素に不正な文字が含まれています。")
                                End If
                                i += 1

                                b = b << 4 Or b2
                            End If

                            If bitPos = 7 Then
                                bytes(bytePos) = CByte(b)
                                bytePos -= 1
                            Else
                                bytes(bytePos) = bytes(bytePos) Or CByte(b << (7 - bitPos))
                                bytePos -= 1
                                bytes(bytePos) = CByte(b >> (bitPos + 1))
                            End If
                        End While

                        bytePos = bytePosNext
                        bitPos = bitPosNext
                    End If
                Else
                    Dim b64 As UInt64 = 0
                    While i < k
                        Dim d As Integer = GetIntFromDecChar(value.Chars(i))
                        If d = -1 Then
                            Throw New ArgumentException(elem.ToString() & "番目の要素に不正な文字が含まれています。")
                        End If

                        Try
                            b64 = b64 * 10UL + CType(d, UInt64)
                        Catch ex As Exception
                            Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                        End Try

                        i += 1
                    End While

                    If b64 >= (1UL << elemBits) Then
                        Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                    End If

                    If elemByteOrder = XlsByteOrder.BigEndian Then
                        Dim restBits As Integer = (elemBits \ 8) * 8
                        Dim headBits As Integer = elemBits - restBits
                        If headBits <> 0 Then
                            If (b64 >> restBits) >= (1 << headBits) Then
                                Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                            End If
                            Dim b As Integer = CInt(b64 >> restBits)
                            b = b << (8 - headBits)
                            If bitPos = 0 Then
                                bytes(bytePos) = CByte(b)
                                bitPos = headBits
                            Else
                                bytes(bytePos) = bytes(bytePos) Or CByte(b >> bitPos)
                                bitPos += headBits
                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        bytes(bytePos) = CByte(b << (headBits - bitPos) And &HFF)
                                    End If
                                End If
                            End If
                        End If
                        While restBits <> 0
                            restBits -= 8
                            Dim b As Integer = CInt(b64 >> restBits And &HFFUL)
                            If bitPos = 0 Then
                                bytes(bytePos) = CByte(b)
                                bytePos += 1
                            Else
                                bytes(bytePos) = bytes(bytePos) Or CByte(b >> bitPos)
                                bytePos += 1
                                bytes(bytePos) = CByte(b << (8 - bitPos) And &HFF)
                            End If
                        End While
                    Else
                        'If elemBits > 8 AndAlso (bitPos <> 0 OrElse elemBits Mod 8 <> 0)  Then
                        '    Throw New InvalidOperationException(elem.ToString() & "番目の要素はリトルエンディアンで複数バイトに格納しなければなりませんが、バイト境界に配置されません。")
                        'End If

                        Dim restBits As Integer = (elemBits \ 8) * 8
                        Dim headBits As Integer = elemBits - restBits
                        For bits As Integer = 0 To restBits - 8 Step 8
                            Dim b As Integer = CInt(b64 >> bits And &HFFUL)
                            If bitPos = 0 Then
                                bytes(bytePos) = CByte(b)
                                bytePos += 1
                            Else
                                bytes(bytePos) = bytes(bytePos) Or CByte(b >> bitPos)
                                bytePos += 1
                                bytes(bytePos) = CByte(b << (8 - bitPos) And &HFF)
                            End If
                        Next
                        If headBits <> 0 Then
                            If (b64 >> restBits) >= (1 << headBits) Then
                                Throw New ArgumentException(elem.ToString() & "番目の要素が大きすぎます。")
                            End If
                            Dim b As Integer = CInt(b64 >> restBits)
                            b = b << (8 - headBits)
                            If bitPos = 0 Then
                                bytes(bytePos) = CByte(b)
                                bitPos = headBits
                            Else
                                bytes(bytePos) = bytes(bytePos) Or CByte(b >> bitPos)
                                bitPos += headBits
                                If bitPos >= 8 Then
                                    bytePos += 1
                                    bitPos -= 8
                                    If bitPos <> 0 Then
                                        bytes(bytePos) = CByte(b << (headBits - bitPos) And &HFF)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                Debug.Assert(i = k)
                i += 1
            Next elem

            While i < valueLen
                If value.Chars(i) <> " "c Then
                    Throw New ArgumentException("値を構成する要素が多すぎます。")
                End If
                i += 1
            End While
        End If
    End Sub

    Public Function NormalizeValue(ByVal value As String) As String
        'OPT: オブジェクトの再生成を避けるために、
        '下記bytesをtempBytesとして、インスタンスに常備してもよい。
        Dim bits As Integer = elemBits * elemCount
        Dim bytes((bits + 7) \ 8 - 1) As Byte
        CopyValueToBytes(value, bytes)
        Return CreateValueFromBytes(bytes)
    End Function

    Private Shared Function GetCharFromHalfByteA(ByVal b As Byte, ByVal format As String) As Char
        If b <= 9 Then
            Return ChrW(AscW("0") + b)
        Else
            Return ChrW(b - 10 + AscW(format))
        End If
    End Function

    Private Shared Function GetCharFromHalfByteX(ByVal b As Byte, ByVal format As String) As Char
        If b <= 9 Then
            Return ChrW(AscW("0") + b)
        Else
            Return ChrW(b - 10 + AscW(format) - (AscW("X"c) - AscW("A"c)))
        End If
    End Function

    Private Shared Function GetIntFromHexChar(ByVal c As Char) As Integer
        Dim i As Integer = AscW(c)
        If i >= AscW("0"c) AndAlso i <= AscW("9"c) Then Return i - AscW("0"c)
        If i >= AscW("A"c) AndAlso i <= AscW("F"c) Then Return i - (AscW("A"c) - 10)
        If i >= AscW("a"c) AndAlso i <= AscW("f"c) Then Return i - (AscW("a"c) - 10)
        Return -1
    End Function

    Private Shared Function GetIntFromDecChar(ByVal c As Char) As Integer
        Dim i As Integer = AscW(c)
        If i >= AscW("0"c) AndAlso i <= AscW("9"c) Then Return i - AscW("0"c)
        Return -1
    End Function

End Class
