' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2018 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2018/01/18  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Messaging
Imports System.Net.Sockets
Imports System.Reflection
Imports System.Text

Imports JR.ExOpmg.Common

Public Class StringExpander

    Private Shared ReadOnly oFuncArgSeps As Char() = {";"c}
    Private Shared ReadOnly oSpaces As Char() = {" "c, ControlChars.Tab}
    Private Shared ReadOnly oBreaks As Char() = {ControlChars.Cr, ControlChars.Lf}
    Private Shared ReadOnly oSpacesAndBreaks As Char() = {" "c, ControlChars.Tab, ControlChars.Cr, ControlChars.Lf}
    Private Shared ReadOnly oDateTimeParseFormats As String() = {"yyyy/M/d H:m:s.fff", "yyyy/M/d H:m:s.ff", "yyyy/M/d H:m:s.f", "yyyy/M/d H:m:s", "yyyy/M/d H:m", "yyyy/M/d"}
    Private Const DateTimeStdFormat As String = "yyyy/MM/dd HH:mm:ss.fff"
    Private Const UIntNumberStyle As NumberStyles = NumberStyles.AllowLeadingWhite Or NumberStyles.AllowTrailingWhite

    'NOTE: sはSafeWordとRawCharで構成されている。
    'RawCharの";"を区切り文字、
    'RawCharの" "と水平タブを区切り後のトリム対象文字として解釈する。
    '戻り値の中のRawCharは基本的にSafeWordに変換する。
    Private Delegate Function FuncDelegate(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String

    Private Class FuncInfo
        Public Word As String
        Public Expand As FuncDelegate
        Sub New(ByVal sWord As String, ByVal oDelegate As FuncDelegate)
            Me.Word = sWord
            Me.Expand = oDelegate
        End Sub
    End Class

    Private Class SymInfo
        Public Word As String
        Public RawChar As Char
        Public SafeNumber As Char
        Sub New(ByVal sWord As String, ByVal rawChar As Char, ByVal safeNum As Integer)
            Debug.Assert(safeNum <= 9)
            Me.Word = sWord
            Me.RawChar = rawChar
            Me.SafeNumber = ChrW(AscW("0") + safeNum)
        End Sub
    End Class

    Private Shared oFuncInfoDic As Dictionary(Of String, FuncInfo)
    Private Shared oSymInfoDic As Dictionary(Of String, SymInfo)
    Private Shared oSymInfoForSafeNumbers As List(Of SymInfo)
    Private Shared oSafeNumberForRawChar As Dictionary(Of Char, Char)

    Private Shared Sub RegisterFunc(ByVal sWord As String, ByVal oDelegate As FuncDelegate)
        oFuncInfoDic.Add(sWord, New FuncInfo(sWord, oDelegate))
    End Sub

    Private Shared Sub RegisterSym(ByVal sWord As String, ByVal rawChar As Char)
        Dim n As Integer = oSymInfoForSafeNumbers.Count
        Dim oInfo As New SymInfo(sWord, rawChar, n)
        oSymInfoDic.Add(sWord, oInfo)
        oSymInfoForSafeNumbers.Add(oInfo)
        oSafeNumberForRawChar.Add(rawChar, ChrW(AscW("0"c) + n))
    End Sub

    Private Shared sSafeWordForFuncArgSep As String

    Shared Sub New()
        oFuncInfoDic = New Dictionary(Of String, FuncInfo)

        RegisterFunc("MidStr", AddressOf ExpandWordOfMidStr)
        RegisterFunc("LeftStr", AddressOf ExpandWordOfLeftStr)
        RegisterFunc("RightStr", AddressOf ExpandWordOfRightStr)
        RegisterFunc("StrElem", AddressOf ExpandWordOfStrElem)
        RegisterFunc("InStr", AddressOf ExpandWordOfInStr)
        RegisterFunc("InStrRev", AddressOf ExpandWordOfInStrRev)
        RegisterFunc("StrLen", AddressOf ExpandWordOfStrLen)
        RegisterFunc("Replace", AddressOf ExpandWordOfReplace)
        RegisterFunc("TrimSp", AddressOf ExpandWordOfTrimSp)
        RegisterFunc("TrimBr", AddressOf ExpandWordOfTrimBr)
        RegisterFunc("Trim", AddressOf ExpandWordOfTrim)
        RegisterFunc("ToUpper", AddressOf ExpandWordOfToUpper)
        RegisterFunc("ToLower", AddressOf ExpandWordOfToLower)
        RegisterFunc("Format", AddressOf ExpandWordOfFormat)
        RegisterFunc("BytesFrStr", AddressOf ExpandWordOfBytesFrStr)
        RegisterFunc("StrMin", AddressOf ExpandWordOfStrMin)
        RegisterFunc("StrMax", AddressOf ExpandWordOfStrMax)

        RegisterFunc("MidBytes", AddressOf ExpandWordOfMidBytes)
        RegisterFunc("LeftBytes", AddressOf ExpandWordOfLeftBytes)
        RegisterFunc("RightBytes", AddressOf ExpandWordOfRightBytes)
        RegisterFunc("BytesElem", AddressOf ExpandWordOfBytesElem)
        RegisterFunc("InBytes", AddressOf ExpandWordOfInBytes)
        RegisterFunc("BytesLen", AddressOf ExpandWordOfBytesLen)
        RegisterFunc("BitAndOfBytes", AddressOf ExpandWordOfBitAndOfBytes)
        RegisterFunc("BitOrOfBytes", AddressOf ExpandWordOfBitOrOfBytes)
        RegisterFunc("BitXorOfBytes", AddressOf ExpandWordOfBitXorOfBytes)
        RegisterFunc("BitNotOfBytes", AddressOf ExpandWordOfBitNotOfBytes)
        RegisterFunc("UIntFrLeBytes", AddressOf ExpandWordOfUIntFrLeBytes)
        RegisterFunc("UIntFrBeBytes", AddressOf ExpandWordOfUIntFrBeBytes)
        RegisterFunc("IntFrLeBytes", AddressOf ExpandWordOfIntFrLeBytes)
        RegisterFunc("IntFrBeBytes", AddressOf ExpandWordOfIntFrBeBytes)
        RegisterFunc("LeBytesFrInt", AddressOf ExpandWordOfLeBytesFrInt)
        RegisterFunc("BeBytesFrInt", AddressOf ExpandWordOfBeBytesFrInt)

        RegisterFunc("MidFields", AddressOf ExpandWordOfMidFields)
        RegisterFunc("LeftFields", AddressOf ExpandWordOfLeftFields)
        RegisterFunc("RightFields", AddressOf ExpandWordOfRightFields)
        RegisterFunc("FieldsElem", AddressOf ExpandWordOfFieldsElem)
        RegisterFunc("InFields", AddressOf ExpandWordOfInFields)
        RegisterFunc("FieldsLen", AddressOf ExpandWordOfFieldsLen)

        RegisterFunc("MidArray", AddressOf ExpandWordOfMidArray)
        RegisterFunc("LeftArray", AddressOf ExpandWordOfLeftArray)
        RegisterFunc("RightArray", AddressOf ExpandWordOfRightArray)
        RegisterFunc("ArrayElem", AddressOf ExpandWordOfArrayElem)
        RegisterFunc("InArray", AddressOf ExpandWordOfInArray)
        RegisterFunc("ArrayLen", AddressOf ExpandWordOfArrayLen)
        RegisterFunc("ValidateSep", AddressOf ExpandWordOfValidateSep)

        RegisterFunc("Add", AddressOf ExpandWordOfAdd)
        RegisterFunc("Sub", AddressOf ExpandWordOfSub)
        RegisterFunc("Mul", AddressOf ExpandWordOfMul)
        RegisterFunc("Div", AddressOf ExpandWordOfDiv)
        RegisterFunc("Quotient", AddressOf ExpandWordOfQuotient)
        RegisterFunc("Remainder", AddressOf ExpandWordOfRemainder)
        RegisterFunc("Neg", AddressOf ExpandWordOfNeg)
        RegisterFunc("Abs", AddressOf ExpandWordOfAbs)
        RegisterFunc("Int", AddressOf ExpandWordOfInt)
        RegisterFunc("Ceil", AddressOf ExpandWordOfCeil)
        RegisterFunc("Floor", AddressOf ExpandWordOfFloor)
        RegisterFunc("Min", AddressOf ExpandWordOfMin)
        RegisterFunc("Max", AddressOf ExpandWordOfMax)

        RegisterFunc("Now", AddressOf ExpandWordOfNow)
        RegisterFunc("TimeDiff", AddressOf ExpandWordOfTimeDiff)
        RegisterFunc("TimeAfter", AddressOf ExpandWordOfTimeAfter)
        RegisterFunc("TimeBefore", AddressOf ExpandWordOfTimeBefore)

        RegisterFunc("IsMatchBinFiles", AddressOf ExpandWordOfIsMatchBinFiles)
        RegisterFunc("IsMatchCsvFiles", AddressOf ExpandWordOfIsMatchCsvFiles)
        RegisterFunc("StrEq", AddressOf ExpandWordOfStrEq)
        RegisterFunc("StrNeq", AddressOf ExpandWordOfStrNeq)
        RegisterFunc("StrGeq", AddressOf ExpandWordOfStrGeq)
        RegisterFunc("StrGt", AddressOf ExpandWordOfStrGt)
        RegisterFunc("StrLeq", AddressOf ExpandWordOfStrLeq)
        RegisterFunc("StrLt", AddressOf ExpandWordOfStrLt)
        RegisterFunc("StrCmp", AddressOf ExpandWordOfStrCmp)
        RegisterFunc("IsZero", AddressOf ExpandWordOfIsZero)
        RegisterFunc("IsNega", AddressOf ExpandWordOfIsNega)
        RegisterFunc("IsPosi", AddressOf ExpandWordOfIsPosi)
        RegisterFunc("Eq", AddressOf ExpandWordOfEq)
        RegisterFunc("Neq", AddressOf ExpandWordOfNeq)
        RegisterFunc("Geq", AddressOf ExpandWordOfGeq)
        RegisterFunc("Gt", AddressOf ExpandWordOfGt)
        RegisterFunc("Leq", AddressOf ExpandWordOfLeq)
        RegisterFunc("Lt", AddressOf ExpandWordOfLt)
        RegisterFunc("And", AddressOf ExpandWordOfAnd)
        RegisterFunc("Or", AddressOf ExpandWordOfOr)
        RegisterFunc("Not", AddressOf ExpandWordOfNot)
        RegisterFunc("If", AddressOf ExpandWordOfIf)

        RegisterFunc("NewFileOfStr", AddressOf ExpandWordOfNewFileOfStr)
        RegisterFunc("NewFileOfBytes", AddressOf ExpandWordOfNewFileOfBytes)
        RegisterFunc("AppendStrToFile", AddressOf ExpandWordOfAppendStrToFile)
        RegisterFunc("AppendBytesToFile", AddressOf ExpandWordOfAppendBytesToFile)
        RegisterFunc("StrFrFile", AddressOf ExpandWordOfStrFrFile)
        RegisterFunc("BytesFrFile", AddressOf ExpandWordOfBytesFrFile)

        RegisterFunc("SetRef", AddressOf ExpandWordOfSetRef)
        RegisterFunc("SetVal", AddressOf ExpandWordOfSetVal)
        RegisterFunc("Val", AddressOf ExpandWordOfVal)
        RegisterFunc("ContextNum", AddressOf ExpandWordOfContextNum)
        RegisterFunc("ContextDir", AddressOf ExpandWordOfContextDir)
        RegisterFunc("MachineDir", AddressOf ExpandWordOfMachineDir)

        RegisterFunc("ExecDynFunc", AddressOf ExpandWordOfExecDynFunc)
        RegisterFunc("ExecCmdFunc", AddressOf ExpandWordOfExecCmdFunc)
        RegisterFunc("ExecAppFunc", AddressOf ExpandWordOfExecAppFunc)

        oSymInfoDic = New Dictionary(Of String, SymInfo)
        oSymInfoForSafeNumbers = New List(Of SymInfo)
        oSafeNumberForRawChar = New Dictionary(Of Char, Char)
        RegisterSym("$", "$"c)
        RegisterSym(",", ","c)
        RegisterSym(";", ";"c)
        RegisterSym(">", ">"c)
        RegisterSym(" ", " "c)
        RegisterSym("CR", ControlChars.Cr)
        RegisterSym("LF", ControlChars.Lf)
        RegisterSym("HT", ControlChars.Tab)
        RegisterSym("NUL", ControlChars.NullChar)

        sSafeWordForFuncArgSep = vbNullChar & oSafeNumberForRawChar(";"c)
    End Sub

    Private oExtAppTargetQueue As MessageQueue
    Private oParentMessageSock As Socket
    Private oParentMessageQueue As LinkedList(Of InternalMessage)
    Private oPostponeParentMessages As Action
    Private oAssemblies As Dictionary(Of String, Assembly)
    Private oGlobalVariables As Dictionary(Of String, VarHolder)
    Private sSourceDir As String
    Private sMachineDir As String
    Private sSafeWordMachineDir As String

    Public Sub New( _
      ByVal oExtAppTargetQueue As MessageQueue, _
      ByVal oParentMessageSock As Socket, _
      ByVal oParentMessageQueue As LinkedList(Of InternalMessage), _
      ByVal oPostponeParentMessages As Action, _
      ByVal sMachineDir As String)
        Me.oExtAppTargetQueue = oExtAppTargetQueue
        Me.oParentMessageSock = oParentMessageSock
        Me.oParentMessageQueue = oParentMessageQueue
        Me.oPostponeParentMessages = oPostponeParentMessages
        Me.oAssemblies = Nothing
        Me.oGlobalVariables = Nothing
        Me.sSourceDir = Environment.CurrentDirectory
        Me.sMachineDir = sMachineDir
        Me.sSafeWordMachineDir = ReplaceRawCharToSafeWord(sMachineDir)
    End Sub

    Public Property Assemblies() As Dictionary(Of String, Assembly)
        Get
            Return oAssemblies
        End Get

        Set(ByVal value As Dictionary(Of String, Assembly))
            oAssemblies = value
        End Set
    End Property

    Public Property GlobalVariables() As Dictionary(Of String, VarHolder)
        Get
            Return oGlobalVariables
        End Get

        Set(ByVal value As Dictionary(Of String, VarHolder))
            oGlobalVariables = value
        End Set
    End Property

    Public Property CurrentDirectory() As String
        Get
            Return sSourceDir
        End Get

        Set(ByVal value As String)
            sSourceDir = value
        End Set
    End Property

    Public Function Expand(ByVal sSrc As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If sSrc.IndexOf(ControlChars.NullChar) <> -1 Then Throw New FormatException("StringExpander cannot expand a string that contains null character. Use $[NUL] in place of null character.")

        'SymWordをSafeWordに置換する。なお、SymWord書式で記述されていない「$」や「>」や「;」や空白類はそのままとする。
        sSrc = ReplaceSymWordToSafeWord(sSrc)

        Dim i As Integer = sSrc.IndexOf("$"c)
        If i <> -1 Then
            Dim srcLen As Integer = sSrc.Length
            Dim srcExpandedLen As Integer = 0
            Dim sDst As String = ""
            Do
                If i + 2 >= srcLen Then Throw New FormatException("Halfway expression detected." & vbCrLf & ReplaceSafeWordToSymWord(sSrc.Substring(i)))
                Dim i2 As Integer = sSrc.IndexOf("<"c, i + 1)
                If i2 = -1 Then Throw New FormatException("Halfway expression detected." & vbCrLf & ReplaceSafeWordToSymWord(sSrc.Substring(i)))
                Dim sWord As String = sSrc.Substring((i + 1), i2 - (i + 1))
                Dim oFuncInfo As FuncInfo = Nothing
                If oFuncInfoDic.TryGetValue(sWord, oFuncInfo) = False Then Throw New FormatException("Unknown func $" & ReplaceSafeWordToSymWord(sWord) & "<> detected.")

                Dim i3 As Integer = i2 + 1
                Dim sArgs As String = ExpandFuncArgs(sSrc, i3, oLocalVariables, contextNum)
                sDst = sDst & sSrc.Substring(srcExpandedLen, i - srcExpandedLen) & oFuncInfo.Expand(Me, sArgs, oLocalVariables, contextNum)
                srcExpandedLen = i3

                If srcExpandedLen >= srcLen Then Exit Do
                i = sSrc.IndexOf("$"c, srcExpandedLen)

                If i = -1 Then
                    sDst = sDst & sSrc.Substring(srcExpandedLen)
                    Exit Do
                Else
                    sDst = sDst & sSrc.Substring(srcExpandedLen, i - srcExpandedLen)
                    srcExpandedLen = i
                End If
            Loop
            sSrc = sDst
        End If

        Return ReplaceSafeWordToRawChar(sSrc)
    End Function

    Private Function ExpandFuncArgs(ByVal sSrc As String, ByRef srcExpandedLen As Integer, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim srcLen As Integer = sSrc.Length
        Dim sDst As String = ""
        Do
            If srcExpandedLen >= srcLen Then Throw New FormatException("Unterminated args detected." & vbCrLf & ReplaceSafeWordToSymWord(sSrc.Substring(srcExpandedLen)))

            Dim i As Integer = sSrc.IndexOf("$"c, srcExpandedLen)
            Dim j As Integer = sSrc.IndexOf(">"c, srcExpandedLen)

            If j = -1 Then Throw New FormatException("Unterminated args detected." & vbCrLf & ReplaceSafeWordToSymWord(sSrc.Substring(srcExpandedLen)))

            If i = -1 OrElse j < i Then
                sDst = sDst & sSrc.Substring(srcExpandedLen, j - srcExpandedLen)
                srcExpandedLen = j + 1
                Exit Do
            End If

            Dim i2 As Integer = sSrc.IndexOf("<"c, i)
            If i2 = -1 Then Throw New FormatException("Halfway expression detected." & vbCrLf & ReplaceSafeWordToSymWord(sSrc.Substring(i)))
            Dim sWord As String = sSrc.Substring((i + 1), i2 - (i + 1))
            Dim oFuncInfo As FuncInfo = Nothing
            If oFuncInfoDic.TryGetValue(sWord, oFuncInfo) = False Then Throw New FormatException("Unknown func $" & ReplaceSafeWordToSymWord(sWord) & "<> detected.")

            Dim i3 As Integer = i2 + 1
            Dim sArgs As String = ExpandFuncArgs(sSrc, i3, oLocalVariables, contextNum)
            sDst = sDst & sSrc.Substring(srcExpandedLen, i - srcExpandedLen) & oFuncInfo.Expand(Me, sArgs, oLocalVariables, contextNum)
            srcExpandedLen = i3
        Loop
        Return sDst
    End Function

    Private Shared Function ReplaceSymWordToSafeWord(ByVal sSrc As String) As String
        Dim i As Integer = sSrc.IndexOf("$[", StringComparison.Ordinal)
        If i = -1 Then Return sSrc

        Dim srcLen As Integer = sSrc.Length
        Dim srcExpandedLen As Integer = 0
        Dim oDst As New StringBuilder(srcLen)
        Do
            If i + 2 >= srcLen Then Throw New FormatException("Unterminated sym detected." & vbCrLf & sSrc.Substring(i))
            Dim i2 As Integer = sSrc.IndexOf("]"c, i + 2)
            If i2 = -1 Then Throw New FormatException("Unterminated sym detected." & vbCrLf & sSrc.Substring(i))

            Dim sWord As String = sSrc.Substring(i + 2, i2 - (i + 2))
            Dim oSymInfo As SymInfo = Nothing
            If oSymInfoDic.TryGetValue(sWord, oSymInfo) = False Then Throw New FormatException("Unknown sym $[" & sWord & "] detected.")
            oDst.Append(sSrc.Substring(srcExpandedLen, i - srcExpandedLen))
            oDst.Append(ControlChars.NullChar)
            oDst.Append(oSymInfo.SafeNumber)
            srcExpandedLen = i2 + 1

            If srcExpandedLen >= srcLen Then Exit Do
            i = sSrc.IndexOf("$[", srcExpandedLen, StringComparison.Ordinal)
            If i = -1 Then
                oDst.Append(sSrc.Substring(srcExpandedLen))
                Exit Do
            End If
        Loop
        Return oDst.ToString()
    End Function

    Private Shared Function ReplaceSafeWordToSymWord(ByVal sSrc As String) As String
        Dim i As Integer = sSrc.IndexOf(ControlChars.NullChar)
        If i = -1 Then Return sSrc

        Dim srcLen As Integer = sSrc.Length
        Dim srcExpandedLen As Integer = 0
        Dim oDst As New StringBuilder(srcLen * 3)
        Do
            oDst.Append(sSrc.Substring(srcExpandedLen, i - srcExpandedLen))
            oDst.Append("$[")
            oDst.Append(oSymInfoForSafeNumbers(AscW(sSrc.Chars(i + 1)) - AscW("0"c)).Word)
            oDst.Append("]"c)
            srcExpandedLen = i + 2
            If srcExpandedLen >= srcLen Then Exit Do

            i = sSrc.IndexOf(ControlChars.NullChar, srcExpandedLen)
            If i = -1 Then
                oDst.Append(sSrc.Substring(srcExpandedLen))
                Exit Do
            End If
        Loop
        Return oDst.ToString()
    End Function

    Private Shared Function ReplaceSafeWordToRawChar(ByVal sSrc As String) As String
        Dim i As Integer = sSrc.IndexOf(ControlChars.NullChar)
        If i = -1 Then Return sSrc

        Dim srcLen As Integer = sSrc.Length
        Dim srcExpandedLen As Integer = 0
        Dim oDst As New StringBuilder(srcLen)
        Do
            oDst.Append(sSrc.Substring(srcExpandedLen, i - srcExpandedLen))
            oDst.Append(oSymInfoForSafeNumbers(AscW(sSrc.Chars(i + 1)) - AscW("0"c)).RawChar)
            srcExpandedLen = i + 2
            If srcExpandedLen >= srcLen Then Exit Do

            i = sSrc.IndexOf(ControlChars.NullChar, srcExpandedLen)
            If i = -1 Then
                oDst.Append(sSrc.Substring(srcExpandedLen))
                Exit Do
            End If
        Loop
        Return oDst.ToString()
    End Function

    Private Shared Function ReplaceRawCharToSafeWord(ByVal sSrc As String) As String
        Dim oDst As New StringBuilder(sSrc.Length * 2)
        For Each c As Char In sSrc
            Dim safeNum As Char
            If oSafeNumberForRawChar.TryGetValue(c, safeNum) = False Then
                oDst.Append(c)
            Else
                oDst.Append(ControlChars.NullChar)
                oDst.Append(safeNum)
            End If
        Next c
        Return oDst.ToString()
    End Function

    Private Shared Function ExpandWordOfMidStr(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$MidStr requires <String; StartIndex; OutputStringMaxLength>.")

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len OrElse _
           s.IndexOf(";"c, p2 + 1) <> -1 Then Throw New FormatException("$MidStr requires <String; StartIndex; OutputStringMaxLength>.")

        Try
            Dim sInput As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
            Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1))), NumberFormatInfo.InvariantInfo)
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1)), NumberFormatInfo.InvariantInfo)
            Return ReplaceRawCharToSafeWord(Mid(sInput, startInx + 1, outputLen))
        Catch ex As Exception
            Throw New FormatException("$MidStr requires <String; StartIndex; OutputStringMaxLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfLeftStr(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$LeftStr requires <String; OutputStringMaxLength>.")

        Try
            Dim sInput As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return ReplaceRawCharToSafeWord(Left(sInput, outputLen))
        Catch ex As Exception
            Throw New FormatException("$LeftStr requires <String; OutputStringMaxLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfRightStr(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$RightStr requires <String; OutputStringMaxLength>.")

        Try
            Dim sInput As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return ReplaceRawCharToSafeWord(Right(sInput, outputLen))
        Catch ex As Exception
            Throw New FormatException("$RightStr requires <String; OutputStringMaxLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfStrElem(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrElem requires <String; Index>.")

        Try
            Dim sInput As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
            Dim index As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return ReplaceRawCharToSafeWord(sInput.Chars(index))
        Catch ex As Exception
            Throw New FormatException("$StrElem requires <String; Index>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfInStr(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$InStr requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.")

        Dim sTarget As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 Then
            Dim sSearch As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
            Return sTarget.IndexOf(sSearch, StringComparison.Ordinal).ToString()
        Else
            If p2 + 1 >= len Then Throw New FormatException("$InStr requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.")

            Dim sSearch As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))
            Dim targetLen As Integer = sTarget.Length

            Dim p3 As Integer = s.IndexOf(";"c, p2 + 1)
            If p3 = -1 Then
                Try
                    Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1)), NumberFormatInfo.InvariantInfo)
                    If startInx >= targetLen Then Return "-1"
                    If startInx < 0 Then startInx = 0
                    Return sTarget.IndexOf(sSearch, startInx, StringComparison.Ordinal).ToString()
                Catch ex As Exception
                    Throw New FormatException("$InStr requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.", ex)
                End Try
            Else
                If p3 + 1 >= len OrElse _
                   s.IndexOf(";"c, p3 + 1) <> -1 Then Throw New FormatException("$InStr requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.")

                Try
                    Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1, p3 - (p2 + 1))), NumberFormatInfo.InvariantInfo)
                    If startInx >= targetLen Then Return "-1"
                    If startInx < 0 Then startInx = 0
                    Dim count As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p3 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
                    If count > targetLen - startInx Then count = targetLen - startInx
                    Return sTarget.IndexOf(sSearch, startInx, count, StringComparison.Ordinal).ToString()
                Catch ex As Exception
                    Throw New FormatException("$InStr requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.", ex)
                End Try
            End If
        End If
    End Function

    Private Shared Function ExpandWordOfInStrRev(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$InStrRev requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.")

        Dim sTarget As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 Then
            Dim sSearch As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
            Return sTarget.LastIndexOf(sSearch, StringComparison.Ordinal).ToString()
        Else
            If p2 + 1 >= len Then Throw New FormatException("$InStrRev requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.")

            Dim sSearch As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))
            Dim targetLen As Integer = sTarget.Length

            Dim p3 As Integer = s.IndexOf(";"c, p2 + 1)
            If p3 = -1 Then
                Try
                    Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1)), NumberFormatInfo.InvariantInfo)
                    If startInx < 0 Then Return "-1"
                    If startInx >= targetLen Then startInx = targetLen - 1
                    Return sTarget.LastIndexOf(sSearch, startInx, StringComparison.Ordinal).ToString()
                Catch ex As Exception
                    Throw New FormatException("$InStrRev requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.", ex)
                End Try
            Else
                If p3 + 1 >= len OrElse _
                   s.IndexOf(";"c, p3 + 1) <> -1 Then Throw New FormatException("$InStrRev requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.")

                Try
                    Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1, p3 - (p2 + 1))), NumberFormatInfo.InvariantInfo)
                    If startInx < 0 Then Return "-1"
                    If startInx >= targetLen Then startInx = targetLen - 1
                    Dim count As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p3 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
                    If count > startInx + 1 Then count = startInx + 1
                    Return sTarget.LastIndexOf(sSearch, startInx, count, StringComparison.Ordinal).ToString()
                Catch ex As Exception
                    Throw New FormatException("$InStrRev requires <String; SearchString> or <String; SearchString; StartIndex> or <String; SearchString; StartIndex; Length>.", ex)
                End Try
            End If
        End If
    End Function

    Private Shared Function ExpandWordOfStrLen(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$StrLen requires <String>.")

        Dim sInput As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))
        Return sInput.Length.ToString()
    End Function

    Private Shared Function ExpandWordOfReplace(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$Replace requires <String; OldString; NewString>.")

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len OrElse _
           s.IndexOf(";"c, p2 + 1) <> -1 Then Throw New FormatException("$Replace requires <String; OldString; NewString>.")

        Dim sInput As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim sOldStr As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))
        Dim sNewStr As String = ReplaceSafeWordToRawChar(s.Substring(p2 + 1).Trim(oSpaces))
        Return ReplaceRawCharToSafeWord(Replace(sInput, sOldStr, sNewStr))
    End Function

    Private Shared Function ExpandWordOfTrimSp(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$TrimSp requires <String>.")

        Dim sInput As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))
        Return ReplaceRawCharToSafeWord(sInput.Trim(oSpaces))
    End Function

    Private Shared Function ExpandWordOfTrimBr(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$TrimBr requires <String>.")

        Dim sInput As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))
        Return ReplaceRawCharToSafeWord(sInput.Trim(oBreaks))
    End Function

    Private Shared Function ExpandWordOfTrim(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$Trim requires <String>.")

        Dim sInput As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))
        Return ReplaceRawCharToSafeWord(sInput.Trim(oSpacesAndBreaks))
    End Function

    Private Shared Function ExpandWordOfToUpper(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$ToUpper requires <String>.")

        Dim sInput As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))
        Return ReplaceRawCharToSafeWord(sInput.ToUpperInvariant())
    End Function

    Private Shared Function ExpandWordOfToLower(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$ToLower requires <String>.")

        Dim sInput As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))
        Return ReplaceRawCharToSafeWord(sInput.ToLowerInvariant())
    End Function

    Private Shared Function ExpandWordOfFormat(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim oArgs As String() = s.Split(oFuncArgSeps)
        Try
            Dim oFmtArgs(oArgs.Length - 2) As Object
            For i As Integer = 1 To oArgs.Length - 1
                Dim sArg As String = ReplaceSafeWordToRawChar(oArgs(i).Trim(oSpaces))
                If sArg.IndexOf("/"c) <> -1 Then
                    oFmtArgs(i- 1) = DateTime.ParseExact(sArg, oDateTimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None)
                ElseIf sArg.IndexOf("."c) = -1 Then
                    oFmtArgs(i- 1) = Long.Parse(sArg, NumberFormatInfo.InvariantInfo)
                Else
                    'NOTE: 書式指定子"R"が指定されることは想定しない。
                    oFmtArgs(i- 1) = Decimal.Parse(sArg, NumberFormatInfo.InvariantInfo)
                End If
            Next i
            Return ReplaceRawCharToSafeWord(String.Format(oArgs(0), oFmtArgs))
        Catch ex As Exception
            Throw New FormatException("$Format requires <CompositFormatString; SemicolonDelimitedDecimalsOrTimesToFormat>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfBytesFrStr(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$BytesFrStr requires <String; Encoding> or <String; Encoding; OutputBytesLength> or <String; Encoding; OutputBytesLength; PadByte>.")

        Dim sSource As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 Then
            Dim sEnc As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
            Try
                Dim oEnc As Encoding
                Dim iEnc As Integer
                If Integer.TryParse(sEnc, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, iEnc) = True Then
                    oEnc = Encoding.GetEncoding(iEnc)
                Else
                    oEnc = Encoding.GetEncoding(sEnc)
                End If

                Return BitConverter.ToString(oEnc.GetBytes(sSource))
            Catch ex As Exception
                Throw New FormatException("$BytesFrStr requires <String; Encoding> or <String; Encoding; OutputBytesLength> or <String; Encoding; OutputBytesLength; PadByte>.", ex)
            End Try
        Else
            If p2 + 1 >= len Then Throw New FormatException("$BytesFrStr requires <String; Encoding> or <String; Encoding; OutputBytesLength> or <String; Encoding; OutputBytesLength; PadByte>.")

            Dim sEnc As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))
            Dim oEnc As Encoding
            Try
                Dim iEnc As Integer
                If Integer.TryParse(sEnc, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, iEnc) = True Then
                    oEnc = Encoding.GetEncoding(iEnc)
                Else
                    oEnc = Encoding.GetEncoding(sEnc)
                End If
            Catch ex As Exception
                Throw New FormatException("$BytesFrStr requires <String; Encoding> or <String; Encoding; OutputBytesLength> or <String; Encoding; OutputBytesLength; PadByte>.", ex)
            End Try

            Dim p3 As Integer = s.IndexOf(";"c, p2 + 1)
            If p3 = -1 Then
                Try
                    Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
                    Dim oOutput(outputLen - 1) As Byte
                    oEnc.GetBytes(sSource, 0, sSource.Length, oOutput, 0)
                    Return BitConverter.ToString(oOutput)
                Catch ex As Exception
                    Throw New FormatException("$BytesFrStr requires <String; Encoding> or <String; Encoding; OutputBytesLength> or <String; Encoding; OutputBytesLength; PadByte>.", ex)
                End Try
            Else
                If p3 + 1 >= len OrElse _
                   s.IndexOf(";"c, p3 + 1) <> -1 Then Throw New FormatException("$BytesFrStr requires <String; Encoding> or <String; Encoding; OutputBytesLength> or <String; Encoding; OutputBytesLength; PadByte>.")

                Try
                    Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1, p3 - (p2 + 1))), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
                    Dim pad As Byte = Byte.Parse(s.Substring(p3 + 1), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo)
                    Dim oOutput(outputLen - 1) As Byte
                    Dim i As Integer = oEnc.GetBytes(sSource, 0, sSource.Length, oOutput, 0)
                    While i <> outputLen
                        oOutput(i) = pad
                        i += 1
                    End While
                    Return BitConverter.ToString(oOutput)
                Catch ex As Exception
                    Throw New FormatException("$BytesFrStr requires <String; Encoding> or <String; Encoding; OutputBytesLength> or <String; Encoding; OutputBytesLength; PadByte>.", ex)
                End Try
            End If
        End If
    End Function

    Private Shared Function ExpandWordOfStrMin(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim sRet As String = ReplaceSafeWordToRawChar(oArgs(0).Trim(oSpaces))
            For i As Integer = 1 To oArgs.Length - 1
                Dim sArg As String = ReplaceSafeWordToRawChar(oArgs(i).Trim(oSpaces))
                If String.CompareOrdinal(sArg, sRet) < 0 Then
                    sRet = sArg
                End If
            Next i
            Return ReplaceRawCharToSafeWord(sRet)
        Catch ex As Exception
            Throw New FormatException("$StrMin requires <SemicolonDelimitedStrings>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfStrMax(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim sRet As String = ReplaceSafeWordToRawChar(oArgs(0).Trim(oSpaces))
            For i As Integer = 1 To oArgs.Length - 1
                Dim sArg As String = ReplaceSafeWordToRawChar(oArgs(i).Trim(oSpaces))
                If String.CompareOrdinal(sArg, sRet) > 0 Then
                    sRet = sArg
                End If
            Next i
            Return ReplaceRawCharToSafeWord(sRet)
        Catch ex As Exception
            Throw New FormatException("$StrMax requires <SemicolonDelimitedStrings>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfMidBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$MidBytes requires <Bytes; StartIndex; OutputBytesLength>.")

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len OrElse _
           s.IndexOf(";"c, p2 + 1) <> -1 Then Throw New FormatException("$MidBytes requires <Bytes; StartIndex; OutputBytesLength>.")

        Try
            'OPT: エラーチェックを目的とするにしても、MyUtility.GetBytesFromHyphenatedHexadecimalStringで
            'Byte()に変換するのは無駄が多すぎる。
            Dim oInput As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1))), NumberFormatInfo.InvariantInfo)
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            Return BitConverter.ToString(oInput, startInx, outputLen)
        Catch ex As Exception
            Throw New FormatException("$MidBytes requires <Bytes; StartIndex; OutputBytesLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfLeftBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$LeftBytes requires <Bytes; OutputBytesLength>.")

        Try
            'OPT: エラーチェックを目的とするにしても、MyUtility.GetBytesFromHyphenatedHexadecimalStringで
            'Byte()に変換するのは無駄が多すぎる。
            Dim oInput As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            Return BitConverter.ToString(oInput, outputLen)
        Catch ex As Exception
            Throw New FormatException("$LeftBytes requires <Bytes; OutputBytesLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfRightBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$RightBytes requires <Bytes; OutputBytesLength>.")

        Try
            'OPT: エラーチェックを目的とするにしても、MyUtility.GetBytesFromHyphenatedHexadecimalStringで
            'Byte()に変換するのは無駄が多すぎる。
            Dim oInput As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            Return BitConverter.ToString(oInput, oInput.Length - outputLen, outputLen)
        Catch ex As Exception
            Throw New FormatException("$RightBytes requires <Bytes; OutputBytesLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfBytesElem(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$BytesElem requires <Bytes; Index>.")

        Try
            'OPT: エラーチェックを目的とするにしても、MyUtility.GetBytesFromHyphenatedHexadecimalStringで
            'Byte()に変換するのは無駄が多すぎる。
            Dim oInput As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim index As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return oInput(index).ToString("X2")
        Catch ex As Exception
            Throw New FormatException("$BytesElem requires <Bytes; Index>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfInBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$InBytes requires <Bytes; SearchBytes> or <Bytes; SearchBytes; StartIndex> or <Bytes; SearchBytes; StartIndex; Length>.")

        Try
            'OPT: エラーチェックを目的とするにしても、MyUtility.GetBytesFromHyphenatedHexadecimalStringで
            'Byte()に変換するのは無駄が多すぎる。
            Dim sTarget As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
            Dim oTarget As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(sTarget)

            Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
            If p2 = -1 Then
                Dim sSearch As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
                Dim oSearch As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(sSearch)
                If oSearch.Length = 1 Then
                    Return Array.IndexOf(oTarget, oSearch(0)).ToString()
                Else
                    Dim i As Integer = sTarget.IndexOf(sSearch, StringComparison.OrdinalIgnoreCase)
                    Return If(i <= 0, i, i \ 3).ToString()
                End If
            Else
                If p2 + 1 >= len Then Throw New FormatException("$InBytes requires <Bytes; SearchBytes> or <Bytes; SearchBytes; StartIndex> or <Bytes; SearchBytes; StartIndex; Length>.")

                Dim sSearch As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))
                Dim oSearch As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(sSearch)
                Dim targetLen As Integer = oTarget.Length

                Dim p3 As Integer = s.IndexOf(";"c, p2 + 1)
                If p3 = -1 Then
                    Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1)), NumberFormatInfo.InvariantInfo)
                    If startInx >= targetLen Then Return "-1"
                    If startInx < 0 Then startInx = 0
                    If oSearch.Length = 1 Then
                        Return Array.IndexOf(oTarget, oSearch(0), startInx).ToString()
                    Else
                        Dim i As Integer = sTarget.IndexOf(sSearch, startInx * 3, StringComparison.OrdinalIgnoreCase)
                        Return If(i <= 0, i, i \ 3).ToString()
                    End If
                Else
                    If p3 + 1 >= len OrElse _
                       s.IndexOf(";"c, p3 + 1) <> -1 Then Throw New FormatException("$InBytes requires <Bytes; SearchBytes> or <Bytes; SearchBytes; StartIndex> or <Bytes; SearchBytes; StartIndex; Length>.")

                    Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1, p3 - (p2 + 1))), NumberFormatInfo.InvariantInfo)
                    If startInx >= targetLen Then Return "-1"
                    If startInx < 0 Then startInx = 0
                    Dim count As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p3 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
                    If count > targetLen - startInx Then count = targetLen - startInx
                    If oSearch.Length = 1 Then
                        Return Array.IndexOf(oTarget, oSearch(0), startInx, count).ToString()
                    Else
                        Dim i As Integer = sTarget.IndexOf(sSearch, startInx * 3, count * 3 - 1, StringComparison.OrdinalIgnoreCase)
                        Return If(i <= 0, i, i \ 3).ToString()
                    End If
                End If
            End If
        Catch ex As Exception
            Throw New FormatException("$InBytes requires <Bytes; SearchBytes> or <Bytes; SearchBytes; StartIndex> or <Bytes; SearchBytes; StartIndex; Length>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfBytesLen(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$BytesLen requires <Bytes>.")

        Try
            'OPT: エラーチェックを目的とするにしても、MyUtility.GetBytesFromHyphenatedHexadecimalStringで
            'Byte()に変換するのは無駄が多すぎる。
            Dim oOp As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Trim(oSpaces)))
            Return oOp.Length.ToString()
        Catch ex As Exception
            Throw New FormatException("$BytesLen requires <Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfBitAndOfBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$BitAndOfBytes requires <Bytes; Bytes>.")

        Try
            Dim oOp1 As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim oOp2 As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces)))
            Dim n1 As Integer = oOp1.Length
            Dim n2 As Integer = oOp2.Length
            If n1 = n2 Then
                For i As Integer = 0 To n1 - 1
                    oOp1(i) = oOp1(i) And oOp2(i)
                Next i
                Return BitConverter.ToString(oOp1)
            ElseIf n1 > n2 Then
                For i As Integer = 0 To n2 - 1
                    oOp1(i) = oOp1(i) And oOp2(i)
                Next i
                For i As Integer = n2 To n1 - 1
                    oOp1(i) = 0
                Next i
                Return BitConverter.ToString(oOp1)
            Else
                For i As Integer = 0 To n1 - 1
                    oOp2(i) = oOp2(i) And oOp1(i)
                Next i
                For i As Integer = n1 To n2 - 1
                    oOp2(i) = 0
                Next i
                Return BitConverter.ToString(oOp2)
            End If
        Catch ex As Exception
            Throw New FormatException("$BitAndOfBytes requires <Bytes; Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfBitOrOfBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$BitOrOfBytes requires <Bytes; Bytes>.")

        Try
            Dim oOp1 As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim oOp2 As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces)))
            Dim n1 As Integer = oOp1.Length
            Dim n2 As Integer = oOp2.Length
            If n1 = n2 Then
                For i As Integer = 0 To n1 - 1
                    oOp1(i) = oOp1(i) Or oOp2(i)
                Next i
                Return BitConverter.ToString(oOp1)
            ElseIf n1 > n2 Then
                For i As Integer = 0 To n2 - 1
                    oOp1(i) = oOp1(i) Or oOp2(i)
                Next i
                Return BitConverter.ToString(oOp1)
            Else
                For i As Integer = 0 To n1 - 1
                    oOp2(i) = oOp2(i) Or oOp1(i)
                Next i
                Return BitConverter.ToString(oOp2)
            End If
        Catch ex As Exception
            Throw New FormatException("$BitOrOfBytes requires <Bytes; Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfBitXorOfBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$BitXorOfBytes requires <Bytes; Bytes>.")

        Try
            Dim oOp1 As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim oOp2 As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces)))
            Dim n1 As Integer = oOp1.Length
            Dim n2 As Integer = oOp2.Length
            If n1 = n2 Then
                For i As Integer = 0 To n1 - 1
                    oOp1(i) = oOp1(i) Xor oOp2(i)
                Next i
                Return BitConverter.ToString(oOp1)
            ElseIf n1 > n2 Then
                For i As Integer = 0 To n2 - 1
                    oOp1(i) = oOp1(i) Xor oOp2(i)
                Next i
                Return BitConverter.ToString(oOp1)
            Else
                For i As Integer = 0 To n1 - 1
                    oOp2(i) = oOp2(i) Xor oOp1(i)
                Next i
                Return BitConverter.ToString(oOp2)
            End If
        Catch ex As Exception
            Throw New FormatException("$BitXorOfBytes requires <Bytes; Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfBitNotOfBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$BitNotOfBytes requires <Bytes>.")

        Try
            Dim oOp As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Trim(oSpaces)))
            For i As Integer = 0 To oOp.Length - 1
                oOp(i) = Not oOp(i)
            Next i
            Return BitConverter.ToString(oOp)
        Catch ex As Exception
            Throw New FormatException("$BitNotOfBytes requires <Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfUIntFrLeBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$UIntFrLeBytes requires <Bytes>.")

        Try
            Dim oOp As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Trim(oSpaces)))
            Dim opLen As Integer = oOp.Length
            If opLen > 8 Then Throw New OverflowException("Bytes length must be less than or equal to 8.")
            Dim ret As ULong = oOp(opLen - 1)
            For i As Integer = opLen - 2 To 0 Step -1
                ret = (ret << 8) Or oOp(i)
            Next i
            Return ret.ToString()
        Catch ex As Exception
            Throw New FormatException("$UIntFrLeBytes requires <Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfUIntFrBeBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$UIntFrBeBytes requires <Bytes>.")

        Try
            Dim oOp As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Trim(oSpaces)))
            Dim opLen As Integer = oOp.Length
            If opLen > 8 Then Throw New OverflowException("Bytes length must be less than or equal to 8.")
            Dim ret As ULong = oOp(0)
            For i As Integer = 1 To opLen - 1
                ret = (ret << 8) Or oOp(i)
            Next i
            Return ret.ToString()
        Catch ex As Exception
            Throw New FormatException("$UIntFrBeBytes requires <Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfIntFrLeBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$IntFrLeBytes requires <Bytes>.")

        Try
            Dim oOp As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Trim(oSpaces)))
            Dim opLen As Integer = oOp.Length
            If opLen > 8 Then Throw New OverflowException("Bytes length must be less than or equal to 8.")
            Dim msb As Byte = oOp(opLen - 1)
            If msb > 127 Then
                Dim ret As Long = msb
                For i As Integer = opLen - 2 To 0 Step -1
                    ret = (ret << 8) Or oOp(i)
                Next i
                ret = ret << (8 - opLen)
                ret = ret >> (8 - opLen)
                Return ret.ToString()
            Else
                Dim ret As ULong = msb
                For i As Integer = opLen - 2 To 0 Step -1
                    ret = (ret << 8) Or oOp(i)
                Next i
                Return ret.ToString()
            End If
        Catch ex As Exception
            Throw New FormatException("$IntFrLeBytes requires <Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfIntFrBeBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$IntFrBeBytes requires <Bytes>.")

        Try
            Dim oOp As Byte() = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Trim(oSpaces)))
            Dim opLen As Integer = oOp.Length
            If opLen > 8 Then Throw New OverflowException("Bytes length must be less than or equal to 8.")
            Dim msb As Byte = oOp(0)
            If msb > 127 Then
                Dim ret As Long = msb
                For i As Integer = 1 To opLen - 1
                    ret = (ret << 8) Or oOp(i)
                Next i
                ret = ret << (8 - opLen)
                ret = ret >> (8 - opLen)
                Return ret.ToString()
            Else
                Dim ret As ULong = msb
                For i As Integer = 1 To opLen - 1
                    ret = (ret << 8) Or oOp(i)
                Next i
                Return ret.ToString()
            End If
        Catch ex As Exception
            Throw New FormatException("$IntFrBeBytes requires <Bytes>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfLeBytesFrInt(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$LeBytesFrInt requires <DecimalInteger; OutputBytesLength>.")

        Try
            Dim dec As Long = Long.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim bytesLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            If bytesLen = 0 Then Throw New FormatException("OutputBytesLength must be greater than 0.")
            If dec < 0 Then
                If bytesLen < 8 Then
                    Dim minDec As Long = -1
                    minDec = minDec << (bytesLen * 8 - 1)
                    If dec < minDec Then Throw New OverflowException("It is impossible to represent " & dec.ToString() & " in " & bytesLen & " byte(s).")
                End If
            Else
                If bytesLen < 8 Then
                    Dim maxDec As ULong = ULong.MaxValue >> ((8 - bytesLen) * 8)
                    If dec > maxDec Then Throw New OverflowException("It is impossible to represent " & dec.ToString() & " in " & bytesLen & " byte(s).")
                End If
            End If

            Dim oRet As New StringBuilder(bytesLen * 3 - 1)
            For i As Integer = 1 To bytesLen
                If i <> 1 Then
                    oRet.Append("-"c)
                End If
                Dim b As Byte = CByte(dec And &HFF)
                oRet.Append(GetCharFromHalfByteX(b >> 4))
                oRet.Append(GetCharFromHalfByteX(b And CByte(&HF)))
                dec = dec >> 8
            Next i
            Return oRet.ToString()
        Catch ex As Exception
            Throw New FormatException("$LeBytesFrInt requires <DecimalInteger; OutputBytesLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfBeBytesFrInt(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$BeBytesFrInt requires <DecimalInteger; OutputBytesLength>.")

        Try
            Dim dec As Long = Long.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim bytesLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            If bytesLen = 0 Then Throw New FormatException("OutputBytesLength must be greater than 0.")
            If dec < 0 Then
                If bytesLen < 8 Then
                    Dim minDec As Long = -1
                    minDec = minDec << (bytesLen * 8 - 1)
                    If dec < minDec Then Throw New OverflowException("It is impossible to represent " & dec.ToString() & " in " & bytesLen & " byte(s).")
                End If
            Else
                If bytesLen < 8 Then
                    Dim maxDec As ULong = ULong.MaxValue >> ((8 - bytesLen) * 8)
                    If dec > maxDec Then Throw New OverflowException("It is impossible to represent " & dec.ToString() & " in " & bytesLen & " byte(s).")
                End If
            End If

            Dim oRet As New StringBuilder(bytesLen * 3 - 1)
            Dim start As Integer = bytesLen - 1
            For i As Integer = start To 0 Step -1
                If i <> start Then
                    oRet.Append("-"c)
                End If
                Dim b As Byte = CByte(dec >> (i * 8) And &HFF)
                oRet.Append(GetCharFromHalfByteX(b >> 4))
                oRet.Append(GetCharFromHalfByteX(b And CByte(&HF)))
            Next i
            Return oRet.ToString()
        Catch ex As Exception
            Throw New FormatException("$BeBytesFrInt requires <DecimalInteger; OutputBytesLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfMidFields(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$MidFields requires <Fields; StartIndex; OutputFieldsLength>.")

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len OrElse _
           s.IndexOf(";"c, p2 + 1) <> -1 Then Throw New FormatException("$MidFields requires <Fields; StartIndex; OutputFieldsLength>.")

        Try
            'NOTE: 個々のフィールドを編集するわけではないので、GetFieldsFromSpaceDelimitedStringによるデコードは省略する。
            '一方で、SafeWordのままだと区切り文字が複雑であるため、ReplaceSafeWordToRawCharは行うことにしている。
            'Dim oInput As String() = MyUtility.GetFieldsFromSpaceDelimitedString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim oInput As String() = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)).Split(" "c)
            Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1))), NumberFormatInfo.InvariantInfo)
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            Dim oBuilder As New StringBuilder(p1)
            For iElem As Integer = startInx To startInx + outputLen - 1
                If iElem <> startInx Then
                    oBuilder.Append(" "c)
                End If
                'oBuilder.Append(oInput(iElem).Replace("!", "!21").Replace(" ", "!20"))
                oBuilder.Append(oInput(iElem))
            Next iElem
            Return ReplaceRawCharToSafeWord(oBuilder.ToString())
        Catch ex As Exception
            Throw New FormatException("$MidFields requires <Fields; StartIndex; OutputFieldsLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfLeftFields(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$LeftFields requires <Fields; OutputFieldsLength>.")

        Try
            'NOTE: 個々のフィールドを編集するわけではないので、GetFieldsFromSpaceDelimitedStringによるデコードは省略する。
            '一方で、SafeWordのままだと区切り文字が複雑であるため、ReplaceSafeWordToRawCharは行うことにしている。
            'Dim oInput As String() = MyUtility.GetFieldsFromSpaceDelimitedString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim oInput As String() = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)).Split(" "c)
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            Dim oBuilder As New StringBuilder(p1)
            For iElem As Integer = 0 To outputLen - 1
                If iElem <> 0 Then
                    oBuilder.Append(" "c)
                End If
                'oBuilder.Append(oInput(iElem).Replace("!", "!21").Replace(" ", "!20"))
                oBuilder.Append(oInput(iElem))
            Next iElem
            Return ReplaceRawCharToSafeWord(oBuilder.ToString())
        Catch ex As Exception
            Throw New FormatException("$LeftFields requires <Fields; OutputFieldsLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfRightFields(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$RightFields requires <Fields; OutputFieldsLength>.")

        Try
            'NOTE: 個々のフィールドを編集するわけではないので、GetFieldsFromSpaceDelimitedStringによるデコードは省略する。
            '一方で、SafeWordのままだと区切り文字が複雑であるため、ReplaceSafeWordToRawCharは行うことにしている。
            'Dim oInput As String() = MyUtility.GetFieldsFromSpaceDelimitedString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim oInput As String() = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)).Split(" "c)
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            Dim oBuilder As New StringBuilder(p1)
            Dim inputLen As Integer = oInput.Length
            Dim startInx As Integer = inputLen - outputLen
            For iElem As Integer = startInx To inputLen - 1
                If iElem <> startInx Then
                    oBuilder.Append(" "c)
                End If
                'oBuilder.Append(oInput(iElem).Replace("!", "!21").Replace(" ", "!20"))
                oBuilder.Append(oInput(iElem))
            Next iElem
            Return ReplaceRawCharToSafeWord(oBuilder.ToString())
        Catch ex As Exception
            Throw New FormatException("$RightFields requires <Fields; OutputFieldsLength>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfFieldsElem(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$FieldsElem requires <Fields; Index>.")

        Try
            'NOTE: 個々のフィールドを編集するわけではないので、GetFieldsFromSpaceDelimitedStringによるデコードは省略する。
            '一方で、SafeWordのままだと区切り文字が複雑であるため、ReplaceSafeWordToRawCharは行うことにしている。
            'Dim oInput As String() = MyUtility.GetFieldsFromSpaceDelimitedString(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)))
            Dim oInput As String() = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)).Split(" "c)
            Dim index As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            'Return ReplaceRawCharToSafeWord(oInput(index).Replace("!", "!21").Replace(" ", "!20"))
            Return ReplaceRawCharToSafeWord(oInput(index))
        Catch ex As Exception
            Throw New FormatException("$FieldsElem requires <Fields; Index>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfInFields(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$InFields requires <Fields; SearchFields> or <Fields; SearchFields; StartIndex> or <Fields; SearchFields; StartIndex; Length>.")

        Try
            Dim sTarget As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
            Dim oTarget As String() = MyUtility.GetFieldsFromSpaceDelimitedString(sTarget)

            Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
            If p2 = -1 Then
                Dim sSearch As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
                Dim oSearch As String() = MyUtility.GetFieldsFromSpaceDelimitedString(sSearch)
                If oSearch.Length = 1 Then
                    Return Array.IndexOf(oTarget, oSearch(0)).ToString()
                Else
                    Return oTarget.IndexOf(oSearch).ToString()
                End If
            Else
                If p2 + 1 >= len Then Throw New FormatException("$InFields requires <Fields; SearchFields> or <Fields; SearchFields; StartIndex> or <Fields; SearchFields; StartIndex; Length>.")

                Dim sSearch As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))
                Dim oSearch As String() = MyUtility.GetFieldsFromSpaceDelimitedString(sSearch)
                Dim targetLen As Integer = oTarget.Length

                Dim p3 As Integer = s.IndexOf(";"c, p2 + 1)
                If p3 = -1 Then
                    Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1)), NumberFormatInfo.InvariantInfo)
                    If startInx >= targetLen Then Return "-1"
                    If startInx < 0 Then startInx = 0
                    If oSearch.Length = 1 Then
                        Return Array.IndexOf(oTarget, oSearch(0), startInx).ToString()
                    Else
                        'OPT: .Net Framework 4.5以降ならArraySegmentがIList(Of T)をインプリメントしているので、
                        '下記の実装が可能なはずである。
                        'Return oTarget.IndexOf(New ArraySegment(Of String)(oSearch, startInx, targetLen - startInx)).ToString()
                        Return oTarget.IndexOf(oSearch.Skip(startInx).ToArray()).ToString()
                    End If
                Else
                    If p3 + 1 >= len OrElse _
                       s.IndexOf(";"c, p3 + 1) <> -1 Then Throw New FormatException("$InFields requires <Fields; SearchFields> or <Fields; SearchFields; StartIndex> or <Fields; SearchFields; StartIndex; Length>.")

                    Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p2 + 1, p3 - (p2 + 1))), NumberFormatInfo.InvariantInfo)
                    If startInx >= targetLen Then Return "-1"
                    If startInx < 0 Then startInx = 0
                    Dim count As Integer = Integer.Parse(ReplaceSafeWordToRawChar(s.Substring(p3 + 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
                    If count > targetLen - startInx Then count = targetLen - startInx
                    If oSearch.Length = 1 Then
                        Return Array.IndexOf(oTarget, oSearch(0), startInx, count).ToString()
                    Else
                        'OPT: .Net Framework 4.5以降ならArraySegmentがIList(Of T)をインプリメントしているので、
                        '下記の実装が可能なはずである。
                        'Return oTarget.IndexOf(New ArraySegment(Of String)(oSearch, startInx, count)).ToString()
                        Return oTarget.IndexOf(oSearch.Skip(startInx).Take(count).ToArray()).ToString()
                    End If
                End If
            End If
        Catch ex As Exception
            Throw New FormatException("$InFields requires <Fields; SearchFields> or <Fields; SearchFields; StartIndex> or <Fields; SearchFields; StartIndex; Length>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfFieldsLen(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$FieldsLen requires <Fields>.")

        Try
            'NOTE: ReplaceSafeWordToRawChar(s.Trim(oSpaces)).Split(" "c).Length = 0 の場合は、
            'String.Emptyな1件のフィールドが存在するとみなす。

            'NOTE: 個々のフィールドを編集するわけではないので、GetFieldsFromSpaceDelimitedStringによるデコードは省略する。
            '一方で、SafeWordのままだと区切り文字が複雑であるため、ReplaceSafeWordToRawCharは行うことにしている。
            Dim sArgs As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))
            Dim n As Integer = 1
            Dim p As Integer = sArgs.IndexOf(" "c)
            While p <> -1
                n += 1
                p = sArgs.IndexOf(" "c, p + 1)
            End While
            Return n.ToString()
        Catch ex As Exception
            Throw New FormatException("$FieldsLen requires <Fields>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfMidArray(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim nArgs As Integer = oArgs.Length
            For i As Integer = 0 To nArgs - 3
                'NOTE: 編集の対象とするわけではないので、ReplaceSafeWordToRawCharは省略する。
                'よって、SafeWordの開始記号としてのヌル文字をSafeWordに誤変換することを避けるためにも、
                'Returnする際のReplaceRawCharToSafeWordによる変換も省略する。
                '引数のSemicolonDelimitedElemsが直接記述されたものである場合、oArgs(i)にはSafeWordに
                '変換可能な文字がRawな状態で含まれている可能性もあるが、
                'ここまで到達しているのであるから、少なくとも、Rawな";"や">"は
                '含まれていないはずであり、次に展開される際も正しく（１つの引数として）抽出された後、
                'ReplaceSafeWordToRawCharによって全文字がRawな状態にされるはずである。
                oArgs(i) = oArgs(i).Trim(oSpaces)
            Next i
            Dim startInx As Integer = Integer.Parse(ReplaceSafeWordToRawChar(oArgs(nArgs - 2)), NumberFormatInfo.InvariantInfo)
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(oArgs(nArgs - 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            If startInx + outputLen > nArgs - 2 Then Throw New FormatException("StartIndex + OutputElemsCount must be less than or equal to count of SemicolonDelimitedElems.")

            Dim oBuilder As New StringBuilder(s.Length)
            For iElem As Integer = startInx To startInx + outputLen - 1
                If iElem <> startInx Then
                    oBuilder.Append(";"c)
                End If
                oBuilder.Append(oArgs(iElem))
            Next iElem
            Return oBuilder.ToString()
        Catch ex As Exception
            Throw New FormatException("$MidArray requires <SemicolonDelimitedElems; StartIndex; OutputElemsCount>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfLeftArray(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim nArgs As Integer = oArgs.Length
            For i As Integer = 0 To nArgs - 2
                'NOTE: 編集の対象とするわけではないので、ReplaceSafeWordToRawCharは省略する。
                'よって、SafeWordの開始記号としてのヌル文字をSafeWordに誤変換することを避けるためにも、
                'Returnする際のReplaceRawCharToSafeWordによる変換も省略する。
                '引数のSemicolonDelimitedElemsが直接記述されたものである場合、oArgs(i)にはSafeWordに
                '変換可能な文字がRawな状態で含まれている可能性もあるが、
                'ここまで到達しているのであるから、少なくとも、Rawな";"や">"は
                '含まれていないはずであり、次に展開される際も正しく（１つの引数として）抽出された後、
                'ReplaceSafeWordToRawCharによって全文字がRawな状態にされるはずである。
                oArgs(i) = oArgs(i).Trim(oSpaces)
            Next i
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(oArgs(nArgs - 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)
            If outputLen > nArgs - 1 Then Throw New FormatException("OutputElemsCount must be less than or equal to count of SemicolonDelimitedElems.")

            Dim oBuilder As New StringBuilder(s.Length)
            For iElem As Integer = 0 To outputLen - 1
                If iElem <> 0 Then
                    oBuilder.Append(";"c)
                End If
                oBuilder.Append(oArgs(iElem))
            Next iElem
            Return oBuilder.ToString()
        Catch ex As Exception
            Throw New FormatException("$LeftArray requires <SemicolonDelimitedElems; OutputElemsCount>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfRightArray(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim nArgs As Integer = oArgs.Length
            For i As Integer = 0 To nArgs - 2
                'NOTE: 編集の対象とするわけではないので、ReplaceSafeWordToRawCharは省略する。
                'よって、SafeWordの開始記号としてのヌル文字をSafeWordに誤変換することを避けるためにも、
                'Returnする際のReplaceRawCharToSafeWordによる変換も省略する。
                '引数のSemicolonDelimitedElemsが直接記述されたものである場合、oArgs(i)にはSafeWordに
                '変換可能な文字がRawな状態で含まれている可能性もあるが、
                'ここまで到達しているのであるから、少なくとも、Rawな";"や">"は
                '含まれていないはずであり、次に展開される際も正しく（１つの引数として）抽出された後、
                'ReplaceSafeWordToRawCharによって全文字がRawな状態にされるはずである。
                oArgs(i) = oArgs(i).Trim(oSpaces)
            Next i
            Dim outputLen As Integer = Integer.Parse(ReplaceSafeWordToRawChar(oArgs(nArgs - 1)), UIntNumberStyle, NumberFormatInfo.InvariantInfo)

            Dim oBuilder As New StringBuilder(s.Length)
            Dim inputLen As Integer = nArgs - 1
            Dim startInx As Integer = inputLen - outputLen
            For iElem As Integer = startInx To inputLen - 1
                If iElem <> startInx Then
                    oBuilder.Append(";"c)
                End If
                oBuilder.Append(oArgs(iElem))
            Next iElem
            Return oBuilder.ToString()
        Catch ex As Exception
            Throw New FormatException("$RightArray requires <SemicolonDelimitedElems; OutputElemsCount>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfArrayElem(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim nArgs As Integer = oArgs.Length
            For i As Integer = 0 To nArgs - 2
                'NOTE: 編集の対象とするわけではないので、ReplaceSafeWordToRawCharは省略する。
                'よって、SafeWordの開始記号としてのヌル文字をSafeWordに誤変換することを避けるためにも、
                'Returnする際のReplaceRawCharToSafeWordによる変換も省略する。
                '引数のSemicolonDelimitedElemsが直接記述されたものである場合、oArgs(i)にはSafeWordに
                '変換可能な文字がRawな状態で含まれている可能性もあるが、
                'ここまで到達しているのであるから、少なくとも、Rawな";"や">"は
                '含まれていないはずであり、次に展開される際も正しく（１つの引数として）抽出された後、
                'ReplaceSafeWordToRawCharによって全文字がRawな状態にされるはずである。
                oArgs(i) = oArgs(i).Trim(oSpaces)
            Next i
            Dim index As Integer = Integer.Parse(ReplaceSafeWordToRawChar(oArgs(nArgs - 1)), NumberFormatInfo.InvariantInfo)
            If index >= nArgs - 1 Then Throw New FormatException("Index must be less than count of SemicolonDelimitedElems.")
            Return oArgs(index)
        Catch ex As Exception
            Throw New FormatException("$ArrayElem requires <SemicolonDelimitedElems; Index>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfInArray(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim nArgs As Integer = oArgs.Length
            Dim sSearch As String = ReplaceSafeWordToRawChar(oArgs(nArgs - 1).Trim(oSpaces))
            For i As Integer = 0 To nArgs - 2
                If ReplaceSafeWordToRawChar(oArgs(i).Trim(oSpaces)).Equals(sSearch) Then Return i.ToString()
            Next i
            Return "-1"
        Catch ex As Exception
            Throw New FormatException("$InArray requires <SemicolonDelimitedElems; SearchElem>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfArrayLen(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            'NOTE: s.Split(oFuncArgSeps).Length = 0 の場合は、String.Emptyな1件のフィールドが存在するとみなす。
            Dim n As Integer = 1
            Dim p As Integer = s.IndexOf(";"c)
            While p <> -1
                n += 1
                p = s.IndexOf(";"c, p + 1)
            End While
            Return n.ToString()
        Catch ex As Exception
            Throw New FormatException("$ArrayLen requires <SemicolonDelimitedElems>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfValidateSep(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Return s.Trim(oSpaces).Replace(sSafeWordForFuncArgSep, ";")
    End Function

    Private Shared Function ExpandWordOfAdd(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Add requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return Decimal.Add(op1, op2).ToString()
        Catch ex As Exception
            Throw New FormatException("$Add requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfSub(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Sub requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return Decimal.Subtract(op1, op2).ToString()
        Catch ex As Exception
            Throw New FormatException("$Sub requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfMul(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Mul requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return Decimal.Multiply(op1, op2).ToString()
        Catch ex As Exception
            Throw New FormatException("$Mul requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfDiv(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Div requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return Decimal.Divide(op1, op2).ToString()
        Catch ex As Exception
            Throw New FormatException("$Div requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfQuotient(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Quotient requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return Decimal.Truncate(Decimal.Remainder(op1, op2)).ToString()
        Catch ex As Exception
            Throw New FormatException("$Quotient requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfRemainder(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Remainder requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return Decimal.Remainder(op1, op2).ToString()
        Catch ex As Exception
            Throw New FormatException("$Remainder requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfNeg(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$Neg requires <Decimal>.")

        Try
            Dim op As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s), NumberFormatInfo.InvariantInfo)
            Return Decimal.Negate(op).ToString()
        Catch ex As Exception
            Throw New FormatException("$Neg requires <Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfAbs(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$Abs requires <Decimal>.")

        Try
            Dim op As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s), NumberFormatInfo.InvariantInfo)
            Return Math.Abs(op).ToString()
        Catch ex As Exception
            Throw New FormatException("$Abs requires <Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfInt(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$Int requires <Decimal>.")

        Try
            Dim op As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s), NumberFormatInfo.InvariantInfo)
            Return Decimal.Truncate(op).ToString()
        Catch ex As Exception
            Throw New FormatException("$Int requires <Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfCeil(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$Ceil requires <Decimal>.")

        Try
            Dim op As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s), NumberFormatInfo.InvariantInfo)
            Return Decimal.Ceiling(op).ToString()
        Catch ex As Exception
            Throw New FormatException("$Ceil requires <Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfFloor(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$Floor requires <Decimal>.")

        Try
            Dim op As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s), NumberFormatInfo.InvariantInfo)
            Return Decimal.Floor(op).ToString()
        Catch ex As Exception
            Throw New FormatException("$Floor requires <Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfMin(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim ret As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(oArgs(0)), NumberFormatInfo.InvariantInfo)
            For i As Integer = 1 To oArgs.Length - 1
                Dim d As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(oArgs(i)), NumberFormatInfo.InvariantInfo)
                If d < ret Then
                    ret = d
                End If
            Next i
            Return ret.ToString()
        Catch ex As Exception
            Throw New FormatException("$Min requires <SemicolonDelimitedDecimals>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfMax(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim ret As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(oArgs(0)), NumberFormatInfo.InvariantInfo)
            For i As Integer = 1 To oArgs.Length - 1
                Dim d As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(oArgs(i)), NumberFormatInfo.InvariantInfo)
                If d > ret Then
                    ret = d
                End If
            Next i
            Return ret.ToString()
        Catch ex As Exception
            Throw New FormatException("$Max requires <SemicolonDelimitedDecimals>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfNow(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If ReplaceSafeWordToRawChar(s.Trim(oSpaces)).Length <> 0 Then Throw New FormatException("$Now requires <>.")
        Return ReplaceRawCharToSafeWord(DateTime.Now.ToString(DateTimeStdFormat))
    End Function

    Private Shared Function ExpandWordOfTimeDiff(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$TimeDiff requires <BeginningTime; EndingTime>.")

        Try
            Dim op1 As DateTime = DateTime.ParseExact(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)), oDateTimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None)
            Dim op2 As DateTime = DateTime.ParseExact(ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces)), oDateTimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None)
            Return ReplaceRawCharToSafeWord(CInt(op2.Subtract(op1).TotalMilliseconds).ToString())
        Catch ex As Exception
            Throw New FormatException("$TimeDiff requires <BeginningTime; EndingTime>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfTimeAfter(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$TimeAfter requires <BeginningTime; TimeSpan>.")

        Try
            Dim op1 As DateTime = DateTime.ParseExact(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)), oDateTimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None)
            Dim op2 As Integer = CTypeTicks(ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces)))
            Return ReplaceRawCharToSafeWord(op1.AddMilliseconds(op2).ToString(DateTimeStdFormat))
        Catch ex As Exception
            Throw New FormatException("$TimeAfter requires <BeginningTime; TimeSpan>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfTimeBefore(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$TimeBefore requires <EndingTime; TimeSpan>.")

        Try
            Dim op1 As DateTime = DateTime.ParseExact(ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces)), oDateTimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None)
            Dim op2 As Integer = CTypeTicks(ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces)))
            Return ReplaceRawCharToSafeWord(op1.AddMilliseconds(-op2).ToString(DateTimeStdFormat))
        Catch ex As Exception
            Throw New FormatException("$TimeBefore requires <EndingTime; TimeSpan>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfIsMatchBinFiles(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then _
            Throw New FormatException("$IsMatchBinFiles requires <EvaluandFilePath; BaselineFilePath; MaskingFilePath> or <EvaluandFilePath; BaselineFilePath; MaskingFilePath; EvaluationLength>.")

        Dim sEvaluandFilePath As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len Then _
            Throw New FormatException("$IsMatchBinFiles requires <EvaluandFilePath; BaselineFilePath; MaskingFilePath> or <EvaluandFilePath; BaselineFilePath; MaskingFilePath; EvaluationLength>.")

        Dim sBaselineFilePath As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))

        Dim p3 As Integer = s.IndexOf(";"c, p2 + 1)
        If p3 = -1 Then
            Dim sMaskingFilePath As String = ReplaceSafeWordToRawChar(s.Substring(p2 + 1).Trim(oSpaces))
            Return MyUtility.IsMatchBin(sEvaluandFilePath, sBaselineFilePath, sMaskingFilePath, -1).ToString()
        Else
            If p3 + 1 >= len OrElse _
               s.IndexOf(";"c, p3 + 1) <> -1 Then _
                Throw New FormatException("$IsMatchBinFiles requires <EvaluandFilePath; BaselineFilePath; MaskingFilePath> or <EvaluandFilePath; BaselineFilePath; MaskingFilePath; EvaluationLength>.")

            Dim sMaskingFilePath As String = ReplaceSafeWordToRawChar(s.Substring(p2 + 1, p3 - (p2 + 1)).Trim(oSpaces))
            Dim compLen As Integer
            If Integer.TryParse(ReplaceSafeWordToRawChar(s.Substring(p3 + 1)), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, compLen) = False Then _
                Throw New FormatException("$IsMatchBinFiles requires <EvaluandFilePath; BaselineFilePath; MaskingFilePath> or <EvaluandFilePath; BaselineFilePath; MaskingFilePath; EvaluationLength>.")
            Return MyUtility.IsMatchBin(sEvaluandFilePath, sBaselineFilePath, sMaskingFilePath, compLen).ToString()
        End If
    End Function

    Private Shared Function ExpandWordOfIsMatchCsvFiles(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then _
            Throw New FormatException("$IsMatchCsvFiles requires <EvaluandFilePath; BaselineFilePath; MaskingFilePath> or <EvaluandFilePath; BaselineFilePath; MaskingFilePath; EvaluationFieldCount>.")

        Dim sEvaluandFilePath As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len Then _
            Throw New FormatException("$IsMatchCsvFiles requires <EvaluandFilePath; BaselineFilePath; MaskingFilePath> or <EvaluandFilePath; BaselineFilePath; MaskingFilePath; EvaluationFieldCount>.")

        Dim sBaselineFilePath As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))

        Dim p3 As Integer = s.IndexOf(";"c, p2 + 1)
        If p3 = -1 Then
            Dim sMaskingFilePath As String = ReplaceSafeWordToRawChar(s.Substring(p2 + 1).Trim(oSpaces))
            Return MyUtility.IsMatchCsv(sEvaluandFilePath, sBaselineFilePath, sMaskingFilePath, -1).ToString()
        Else
            If p3 + 1 >= len OrElse _
               s.IndexOf(";"c, p3 + 1) <> -1 Then _
                Throw New FormatException("$IsMatchCsvFiles requires <EvaluandFilePath; BaselineFilePath; MaskingFilePath> or <EvaluandFilePath; BaselineFilePath; MaskingFilePath; EvaluationFieldCount>.")

            Dim sMaskingFilePath As String = ReplaceSafeWordToRawChar(s.Substring(p2 + 1, p3 - (p2 + 1)).Trim(oSpaces))
            Dim compLen As Integer
            If Integer.TryParse(ReplaceSafeWordToRawChar(s.Substring(p3 + 1)), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, compLen) = False Then _
                Throw New FormatException("$IsMatchCsvFiles requires <EvaluandFilePath; BaselineFilePath; MaskingFilePath> or <EvaluandFilePath; BaselineFilePath; MaskingFilePath; EvaluationFieldCount>.")
            Return MyUtility.IsMatchCsv(sEvaluandFilePath, sBaselineFilePath, sMaskingFilePath, compLen).ToString()
        End If
    End Function

    Private Shared Function ExpandWordOfStrEq(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrEq requires <String; String>.")

        Dim op1 As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim op2 As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
        Return If(op1.Equals(op2), "True", "False")
    End Function

    Private Shared Function ExpandWordOfStrNeq(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrNeq requires <String; String>.")

        Dim op1 As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim op2 As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
        Return If(op1.Equals(op2), "False", "True")
    End Function

    Private Shared Function ExpandWordOfStrGeq(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrGeq requires <String; String>.")

        Dim op1 As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim op2 As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
        Return If(String.CompareOrdinal(op1, op2) >= 1, "True", "False")
    End Function

    Private Shared Function ExpandWordOfStrGt(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrGt requires <String; String>.")

        Dim op1 As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim op2 As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
        Return If(String.CompareOrdinal(op1, op2) > 1, "True", "False")
    End Function

    Private Shared Function ExpandWordOfStrLeq(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrLeq requires <String; String>.")

        Dim op1 As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim op2 As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
        Return If(String.CompareOrdinal(op1, op2) <= 1, "True", "False")
    End Function

    Private Shared Function ExpandWordOfStrLt(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrLt requires <String; String>.")

        Dim op1 As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim op2 As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
        Return If(String.CompareOrdinal(op1, op2) < 1, "True", "False")
    End Function

    Private Shared Function ExpandWordOfStrCmp(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrCmp requires <String; String>.")

        Dim op1 As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim op2 As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
        Return String.CompareOrdinal(op1, op2).ToString()
    End Function

    Private Shared Function ExpandWordOfIsZero(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$IsZero requires <Decimal>.")

        Try
            Dim op As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s), NumberFormatInfo.InvariantInfo)
            Return If(op = Decimal.Zero, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$IsZero requires <Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfIsNega(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$IsNega requires <Decimal>.")

        Try
            Dim op As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s), NumberFormatInfo.InvariantInfo)
            Return If(op < Decimal.Zero, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$IsNega requires <Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfIsPosi(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$IsPosi requires <Decimal>.")

        Try
            Dim op As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s), NumberFormatInfo.InvariantInfo)
            Return If(op > Decimal.Zero, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$IsPosi requires <Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfEq(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Eq requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return If(op1 = op2, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$Eq requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfNeq(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Neq requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return If(op1 <> op2, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$Neq requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfGeq(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Geq requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return If(op1 >= op2, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$Geq requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfGt(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Gt requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return If(op1 > op2, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$Gt requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfLeq(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Leq requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return If(op1 <= op2, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$Leq requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfLt(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$Lt requires <Decimal; Decimal>.")

        Try
            Dim op1 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)), NumberFormatInfo.InvariantInfo)
            Dim op2 As Decimal = Decimal.Parse(ReplaceSafeWordToRawChar(s.Substring(p1 + 1)), NumberFormatInfo.InvariantInfo)
            Return If(op1 < op2, "True", "False")
        Catch ex As Exception
            Throw New FormatException("$Lt requires <Decimal; Decimal>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfAnd(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim ret As Boolean = Boolean.Parse(ReplaceSafeWordToRawChar(oArgs(0)))
            For i As Integer = 1 To oArgs.Length - 1
                Dim b As Boolean = Boolean.Parse(ReplaceSafeWordToRawChar(oArgs(i)))
                ret = (ret And b)
            Next i
            Return ret.ToString()
        Catch ex As Exception
            Throw New FormatException("$And requires <SemicolonDelimitedBooleans>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfOr(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Try
            Dim oArgs As String() = s.Split(oFuncArgSeps)
            Dim ret As Boolean = Boolean.Parse(ReplaceSafeWordToRawChar(oArgs(0)))
            For i As Integer = 1 To oArgs.Length - 1
                Dim b As Boolean = Boolean.Parse(ReplaceSafeWordToRawChar(oArgs(i)))
                ret = (ret Or b)
            Next i
            Return ret.ToString()
        Catch ex As Exception
            Throw New FormatException("$Or requires <SemicolonDelimitedBooleans>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfNot(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$Not requires <Boolean>.")

        Try
            Dim op As Boolean = Boolean.Parse(ReplaceSafeWordToRawChar(s))
            Return (Not op).ToString()
        Catch ex As Exception
            Throw New FormatException("$Not requires <Boolean>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfIf(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$If requires <Boolean; OutputString; OutputString>.")

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len OrElse _
           s.IndexOf(";"c, p2 + 1) <> -1 Then Throw New FormatException("$If requires <Boolean; OutputString; OutputString>.")

        Try
            Dim cond As Boolean = Boolean.Parse(ReplaceSafeWordToRawChar(s.Substring(0, p1)))
            If cond Then
                'NOTE: この場でRAW文字列として編集するわけではないのでReplaceSafeWordToRawCharは省略する。
                'この関数に限り、直接記述されているセミコロンなどはそのままにしたいので、ReplaceRawCharToSafeWordも実施しない。
                Return s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces)
            Else
                'NOTE: この場でRAW文字列として編集するわけではないのでReplaceSafeWordToRawCharは省略する。
                'この関数に限り、直接記述されているセミコロンなどはそのままにしたいので、ReplaceRawCharToSafeWordも実施しない。
                Return s.Substring(p2 + 1).Trim(oSpaces)
            End If
        Catch ex As Exception
            Throw New FormatException("$If requires <Boolean; OutputString; OutputString>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfNewFileOfStr(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$NewFileOfStr requires <Path; Encoding; String>.")

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len OrElse _
           s.IndexOf(";"c, p2 + 1) <> -1 Then Throw New FormatException("$NewFileOfStr requires <Path; Encoding; String>.")

        Dim sPathOrg As String = s.Substring(0, p1).Trim(oSpaces)
        Dim sPath As String = ReplaceSafeWordToRawChar(sPathOrg)
        Dim sEnc As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))
        Dim sStr As String = ReplaceSafeWordToRawChar(s.Substring(p2 + 1).Trim(oSpaces))

        Dim oEnc As Encoding
        Try
            If Not Path.IsPathRooted(sPath) Then
                sPath = Path.Combine(Path.Combine(oPander.sMachineDir, "#" & contextNum.ToString()), sPath)
            End If

            Dim iEnc As Integer
            If Integer.TryParse(sEnc, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, iEnc) = True Then
                oEnc = Encoding.GetEncoding(iEnc)
            Else
                oEnc = Encoding.GetEncoding(sEnc)
            End If
        Catch ex As Exception
            Throw New FormatException("$NewFileOfStr requires <Path; Encoding; String>.", ex)
        End Try

        Using oWriter As New StreamWriter(sPath, False, oEnc)
            oWriter.Write(sStr)
        End Using

        'NOTE: sPathOrgにはSafeWordに変換可能な文字がRawな状態で含まれている可能性もあるが、
        'ReplaceRawCharToSafeWordによる変換は省略する。
        '少なくとも、このメソッドでsPathOrgとして正しく抽出できたのであるから、Rawな";"や">"は
        '含まれていないはずであり、次に展開される際も正しく（１つの引数として）抽出された後、
        'ReplaceSafeWordToRawCharによって全文字がRawな状態にされるはずである。
        Return sPathOrg
    End Function

    Private Shared Function ExpandWordOfNewFileOfBytes(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$NewFileOfBytes requires <Path; Bytes>.")

        Dim sPathOrg As String = s.Substring(0, p1).Trim(oSpaces)
        Dim sPath As String = ReplaceSafeWordToRawChar(sPathOrg)

        Dim oBytes As Byte()
        Try
            oBytes = MyUtility.GetBytesFromHyphenatedHexadecimalString(ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces)))

            If Not Path.IsPathRooted(sPath) Then
                sPath = Path.Combine(Path.Combine(oPander.sMachineDir, "#" & contextNum.ToString()), sPath)
            End If
        Catch ex As Exception
            Throw New FormatException("$NewFileOfBytes requires <Path; Bytes>.", ex)
        End Try

        Using oOutputStream As New FileStream(sPath, FileMode.Create, FileAccess.Write)
            oOutputStream.Write(oBytes, 0, oBytes.Length)
        End Using

        'NOTE: sPathOrgにはSafeWordに変換可能な文字がRawな状態で含まれている可能性もあるが、
        'ReplaceRawCharToSafeWordによる変換は省略する。
        '少なくとも、このメソッドでsPathOrgとして正しく抽出できたのであるから、Rawな";"や">"は
        '含まれていないはずであり、次に展開される際も正しく（１つの引数として）抽出された後、
        'ReplaceSafeWordToRawCharによって全文字がRawな状態にされるはずである。
        Return sPathOrg
    End Function

    Private Shared Function ExpandWordOfAppendStrToFile(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len Then Throw New FormatException("$AppendStrToFile requires <String; Path; Encoding>.")

        Dim p2 As Integer = s.IndexOf(";"c , p1 + 1)
        If p2 = -1 OrElse _
           p2 + 1 >= len OrElse _
           s.IndexOf(";"c, p2 + 1) <> -1 Then Throw New FormatException("$AppendStrToFile requires <String; Path; Encoding>.")

        Dim sStrOrg As String = s.Substring(0, p1).Trim(oSpaces)
        Dim sStr As String = ReplaceSafeWordToRawChar(sStrOrg)
        Dim sPath As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1, p2 - (p1 + 1)).Trim(oSpaces))
        Dim sEnc As String = ReplaceSafeWordToRawChar(s.Substring(p2 + 1).Trim(oSpaces))

        Dim oEnc As Encoding
        Try
            If Not Path.IsPathRooted(sPath) Then
                sPath = Path.Combine(Path.Combine(oPander.sMachineDir, "#" & contextNum.ToString()), sPath)
            End If

            Dim iEnc As Integer
            If Integer.TryParse(sEnc, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, iEnc) = True Then
                oEnc = Encoding.GetEncoding(iEnc)
            Else
                oEnc = Encoding.GetEncoding(sEnc)
            End If
        Catch ex As Exception
            Throw New FormatException("$AppendStrToFile requires <String; Path; Encoding>.", ex)
        End Try

        Using oWriter As New StreamWriter(sPath, True, oEnc)
            oWriter.Write(sStr)
        End Using

        'NOTE: sStrOrgにはSafeWordに変換可能な文字がRawな状態で含まれている可能性もあるが、
        'ReplaceRawCharToSafeWordによる変換は省略する。
        '少なくとも、このメソッドでsStrOrgとして正しく抽出できたのであるから、Rawな";"や">"は
        '含まれていないはずであり、次に展開される際も正しく（１つの引数として）抽出された後、
        'ReplaceSafeWordToRawCharによって全文字がRawな状態にされるはずである。
        Return sStrOrg
    End Function

    Private Shared Function ExpandWordOfAppendBytesToFile(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$AppendBytesToFile requires <Bytes; Path>.")

        Dim sPath As String
        Dim sBytes As String
        Dim oBytes As Byte()
        Try
            sBytes = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
            oBytes = MyUtility.GetBytesFromHyphenatedHexadecimalString(sBytes)
            sPath = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))
            If Not Path.IsPathRooted(sPath) Then
                sPath = Path.Combine(Path.Combine(oPander.sMachineDir, "#" & contextNum.ToString()), sPath)
            End If
        Catch ex As Exception
            Throw New FormatException("$AppendBytesToFile requires <Bytes; Path>.", ex)
        End Try

        Using oOutputStream As New FileStream(sPath, FileMode.Append, FileAccess.Write)
            oOutputStream.Write(oBytes, 0, oBytes.Length)
        End Using

        'NOTE: MyUtility.GetBytesFromHyphenatedHexadecimalString(sBytes)で例外がスローされていないことから、
        'sBytesに含まれる文字は数字とA～Zとハイフンのみである。
        Return sBytes
    End Function

    Private Shared Function ExpandWordOfStrFrFile(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$StrFrFile requires <Path; Encoding>.")

        Dim sPath As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        Dim sEnc As String = ReplaceSafeWordToRawChar(s.Substring(p1 + 1).Trim(oSpaces))

        Dim oEnc As Encoding
        Try
            If Not Path.IsPathRooted(sPath) Then
                sPath = Path.Combine(oPander.sSourceDir, sPath)
            End If

            Dim iEnc As Integer
            If Integer.TryParse(sEnc, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, iEnc) = True Then
                oEnc = Encoding.GetEncoding(iEnc)
            Else
                oEnc = Encoding.GetEncoding(sEnc)
            End If
        Catch ex As Exception
            Throw New FormatException("$StrFrFile requires <Path; Encoding>.", ex)
        End Try

        Dim sOutput As String
        Using oReader As New StreamReader(sPath, oEnc)
            sOutput = oReader.ReadToEnd()
        End Using

        Return ReplaceRawCharToSafeWord(sOutput)
    End Function

    Private Shared Function ExpandWordOfBytesFrFile(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$BytesFrFile requires <Path>.")

        Dim sPath As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))

        Try
            If Not Path.IsPathRooted(sPath) Then
                sPath = Path.Combine(oPander.sSourceDir, sPath)
            End If
        Catch ex As Exception
            Throw New FormatException("$BytesFrFile requires <Path>.", ex)
        End Try

        Dim oBytes As Byte()
        Using oInputStream As New FileStream(sPath, FileMode.Open, FileAccess.Read)
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

        Return BitConverter.ToString(oBytes)
    End Function

    Private Shared Function ExpandWordOfSetRef(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$SetRef requires <RefTypeVariableNameWithoutAsterisk; ValTypeVariableName>.")

        'NOTE: oLocalVariablesなどのKeyはValueと違い正しく比較できる必要があるためRawChar形式に正規化する。
        'sの全てが確実にSafeWordになっている保証があれば、SafeWordに統一する方が効率的だが、その保証はない。
        'OPT: そもそも変数名にSafeWord変換対象の文字を含ませることを禁止すれば、このような変換は省略可能である。
        Dim sName As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        If sName.Length = 0 Then Throw New FormatException("$SetRef requires <RefTypeVariableNameWithoutAsterisk; ValTypeVariableName>.")

        'NOTE: 現状は、それを$Valに渡されると戻り値の型が特殊故に面倒なガードが必要になるため、
        'リファレンス型変数のリファレンスを取得できないようにしてある。
        'そもそも、それを許すなら、このような使い道が限定された関数ではなく、$Ref関数を用意し、
        '$SetVal<リファレンス型変数; $Ref<参照したい変数>>のように記述させるべきである。
        Dim sValVarName As String = s.Substring(p1 + 1).Trim(oSpaces)
        If sValVarName.Length = 0 OrElse _
           sValVarName.Chars(0) = "*"c Then Throw New FormatException("$SetRef requires <RefTypeVariableNameWithoutAsterisk; ValTypeVariableName>.")

        sName = "*" & sName
        Dim oHolder As VarHolder = Nothing
        If oLocalVariables.TryGetValue(sName, oHolder) = False Then
            oHolder = New VarHolder()
            oLocalVariables.Add(sName, oHolder)
        End If

        Try
            oHolder.Value = If(sValVarName.Chars(0) = "@"c, oPander.oGlobalVariables(sValVarName), oLocalVariables(sValVarName))
        Catch ex As Exception
            Throw New FormatException("The variable """ & sValVarName & """ not found.")
        End Try

        Return ReplaceRawCharToSafeWord(sName)
    End Function

    Private Shared Function ExpandWordOfSetVal(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim len As Integer = s.Length

        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 OrElse _
           p1 + 1 >= len OrElse _
           s.IndexOf(";"c, p1 + 1) <> -1 Then Throw New FormatException("$SetVal requires <VariableName; String>.")

        'NOTE: oLocalVariablesなどのKeyはValueと違い正しく比較できる必要があるためRawChar形式に正規化する。
        'sの全てが確実にSafeWordになっている保証があれば、SafeWordに統一する方が効率的だが、その保証はない。
        'OPT: そもそも変数名にSafeWord変換対象の文字を含ませることを禁止すれば、このような変換は省略可能である。
        Dim sName As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))
        If sName.Length = 0 Then Throw New FormatException("$SetVal requires <VariableName; String>.")

        'NOTE: 効率化のため値はSafeWord形式で格納する。
        'この場でRAW文字列として編集するわけではないのでReplaceSafeWordToRawCharは省略する。
        'この関数に限り、直接記述されているセミコロンなどはそのままにしたいので、ReplaceRawCharToSafeWordも実施しない。
        Dim sValue As String = s.Substring(p1 + 1).Trim(oSpaces)

        Dim headChar As Char = sName.Chars(0)
        If headChar = "@"c Then
            Dim oHolder As VarHolder = Nothing
            If oPander.oGlobalVariables.TryGetValue(sName, oHolder) = False Then
                oHolder = New VarHolder()
                oPander.oGlobalVariables.Add(sName, oHolder)
            End If
            oHolder.Value = sValue
        ElseIf headChar = "*"c Then
            Try
                DirectCast(oLocalVariables(sName).Value, VarHolder).Value = sValue
            Catch ex As Exception
                Throw New FormatException("The variable """ & sName & """ not found.")
            End Try
        Else
            Dim oHolder As VarHolder = Nothing
            If oLocalVariables.TryGetValue(sName, oHolder) = False Then
                oHolder = New VarHolder()
                oLocalVariables.Add(sName, oHolder)
            End If
            oHolder.Value = sValue
        End If

        Return sValue
    End Function

    Private Shared Function ExpandWordOfVal(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If s.IndexOf(";"c) <> -1 Then Throw New FormatException("$Val requires <VariableName>.")

        'NOTE: oLocalVariablesなどのKeyはValueと違い正しく比較できる必要があるためRawChar形式に正規化する。
        'sの全てが確実にSafeWordになっている保証があれば、SafeWordに統一する方が効率的だが、その保証はない。
        'OPT: そもそも変数名にSafeWord変換対象の文字を含ませることを禁止すれば、このような変換は省略可能である。
        Dim sName As String = ReplaceSafeWordToRawChar(s.Trim(oSpaces))
        If sName.Length = 0 Then Throw New FormatException("$Val requires <VariableName>.")

        Dim headChar As Char = sName.Chars(0)
        Try
            'NOTE: 効率化のため値はSafeWord形式で格納されている。
            If headChar = "@"c Then
                Return DirectCast(oPander.oGlobalVariables(sName).Value, String)
            ElseIf headChar = "*"c Then
                Return DirectCast(DirectCast(oLocalVariables(sName).Value, VarHolder).Value, String)
            Else
                Return DirectCast(oLocalVariables(sName).Value, String)
            End If
        Catch ex As Exception
            Throw New FormatException("The variable """ & sName & """ not found.")
        End Try
    End Function

    Private Shared Function ExpandWordOfContextNum(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If ReplaceSafeWordToRawChar(s.Trim(oSpaces)).Length <> 0 Then Throw New FormatException("$ContextNum requires <>.")
        Return contextNum.ToString()
    End Function

    Private Shared Function ExpandWordOfContextDir(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If ReplaceSafeWordToRawChar(s.Trim(oSpaces)).Length <> 0 Then Throw New FormatException("$ContextDir requires <>.")
        'Return ReplaceRawCharToSafeWord(Path.Combine(oPander.sMachineDir, "#" & contextNum.ToString()))
        Return Path.Combine(oPander.sSafeWordMachineDir, "#" & contextNum.ToString())
    End Function

    Private Shared Function ExpandWordOfMachineDir(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        If ReplaceSafeWordToRawChar(s.Trim(oSpaces)).Length <> 0 Then Throw New FormatException("$MachineDir requires <>.")
        'Return ReplaceRawCharToSafeWord(oPander.sMachineDir)
        Return oPander.sSafeWordMachineDir
    End Function

    Private Shared Function ExpandWordOfExecDynFunc(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim p1 As Integer = s.IndexOf(";"c)
        Dim sFullFuncName As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))

        Dim p2 As Integer = sFullFuncName.IndexOf(".")
        If p2 = -1 OrElse p2 = 0 Then Throw New FormatException("$ExecDynFunc requires <CodeBlockName.ClassName.FuncName; SemicolonDelimitedArguments>.")
        Dim sCodeName As String = sFullFuncName.Substring(0, p2)

        Dim p3 As Integer = sFullFuncName.LastIndexOf("."c)
        If p3 = -1 OrElse p3 <= p2 + 1 OrElse p3 + 1 = sFullFuncName.Length Then Throw New FormatException("$ExecDynFunc requires <CodeBlockName.ClassName.FuncName; SemicolonDelimitedArguments>.")
        Dim sClassName As String = sFullFuncName.Substring(p2 + 1, p3 - (p2 + 1))
        Dim sMethodName As String = sFullFuncName.Substring(p3 + 1)

        Try
            Dim oAsm As Assembly = oPander.oAssemblies(sCodeName.ToUpperInvariant())
            Dim t As Type = oAsm.GetType(sClassName)
            If p1 <> -1 Then
                Dim oArgs As String() = s.Substring(p1 + 1).Split(oFuncArgSeps)
                For i As Integer = 0 To oArgs.Length - 1
                    Dim s2 As String = oArgs(i).Trim(oSpaces)
                    oArgs(i) = ReplaceSafeWordToRawChar(s2)
                Next i
                s = DirectCast(t.InvokeMember(sMethodName, BindingFlags.InvokeMethod, Nothing, Nothing, oArgs), String)
            Else
                s = DirectCast(t.InvokeMember(sMethodName, BindingFlags.InvokeMethod, Nothing, Nothing, Nothing), String)
            End If
            Return ReplaceRawCharToSafeWord(s)
        Catch ex As Exception
            Throw New FormatException("$ExecDynFunc requires <CodeBlockName.ClassName.FuncName; SemicolonDelimitedArguments>.", ex)
        End Try
    End Function

    Private Shared Function ExpandWordOfExecCmdFunc(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 Then Throw New FormatException("$ExecCmdFunc requires <FileName; SemicolonDelimitedArguments; Timeout>.")

        Dim p2 As Integer = s.LastIndexOf(";"c)
        If p2 = p1 Then Throw New FormatException("$ExecCmdFunc requires <FileName; SemicolonDelimitedArguments; Timeout>.")

        Dim sFileName As String = ReplaceSafeWordToRawChar(s.Substring(0, p1).Trim(oSpaces))

        '区切り文字で分解してから、個々の要素を RawChar に変換する。さらに、要素内の
        '空白類が区切り文字とみなされないように要素をダブルクォートで囲んでから、
        '全ての要素を空白で再接続して、Argumentsとする。
        'NOTE: たとえば「cmd.exe /c "echo aaa"」ではダブルクォートが echo に渡されない想定である。
        'なお、スイッチ部分（/c）がダブルクォートで囲まれると誤動作するので、
        '空白類を含む要素のみダブルクォートで囲む。
        Dim sArguments As String = s.Substring(p1 + 1, p2 - (p1 + 1))
        With Nothing
            Dim oArgElems As String() = sArguments.Split(oFuncArgSeps)
            Dim oArgsBuilder As New StringBuilder(sArguments.Length + oArgElems.Length * 2)
            For Each sArg As String In oArgElems
                If oArgsBuilder.Length <> 0 Then
                    oArgsBuilder.Append(" "c)
                End If

                sArg = ReplaceSafeWordToRawChar(sArg.Trim(oSpaces))
                If sArg.IndexOfAny(oSpaces) = -1 Then
                    oArgsBuilder.Append(sArg)
                Else
                    oArgsBuilder.Append(ControlChars.Quote)
                    oArgsBuilder.Append(sArg)
                    oArgsBuilder.Append(ControlChars.Quote)
                End If
            Next sArg
            sArguments = oArgsBuilder.ToString()
        End With

        Dim waitLimitTicks As Integer
        Try
            waitLimitTicks = CTypeTicks(ReplaceSafeWordToRawChar(s.Substring(p2 + 1).Trim(oSpaces)))
        Catch ex As Exception
            Throw New FormatException("$ExecCmdFunc requires <FileName; SemicolonDelimitedArguments; Timeout>.", ex)
        End Try

        Dim sWorkingDir As String = Path.Combine(oPander.sMachineDir, "#" & contextNum.ToString())
        Directory.CreateDirectory(sWorkingDir)

        Log.Debug("$ExecCmdFunc FileName<" & sFileName & "> Arguments<" & sArguments & "> start...")
        Using oProcess As New System.Diagnostics.Process()
            oProcess.StartInfo.WorkingDirectory = sWorkingDir
            oProcess.StartInfo.FileName = sFileName
            oProcess.StartInfo.Arguments = sArguments
            oProcess.StartInfo.UseShellExecute = False
            oProcess.StartInfo.RedirectStandardOutput = True
            oProcess.StartInfo.CreateNoWindow = True
            oProcess.Start()
            s = oProcess.StandardOutput.ReadToEnd()
            If waitLimitTicks = 0 Then
                oProcess.WaitForExit()
            Else
                'NOTE: この間、通信用ソケットや通信用タイマを参照できないが、
                '外部プロセスは短時間で終わる（waitLimitTicksは最大でも10秒程度である）想定である。
                If oProcess.WaitForExit(waitLimitTicks) = False Then
                    Try
                        oProcess.Kill()
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                    End Try
                    Throw New TimeoutException("$ExecCmdFunc timed out.")
                End If
            End If
        End Using
        If s.Length = 0 Then
            Log.Debug("$ExecCmdFunc completed with no output.")
        Else
            Log.Debug("$ExecCmdFunc completed with following output." & vbCrLf & s)
        End If
        Return ReplaceRawCharToSafeWord(s)
    End Function

    Private Shared Function ExpandWordOfExecAppFunc(ByVal oPander As StringExpander, ByVal s As String, ByVal oLocalVariables As Dictionary(Of String, VarHolder), ByVal contextNum As Integer) As String
        Dim p1 As Integer = s.IndexOf(";"c)
        If p1 = -1 Then Throw New FormatException("$ExecAppFunc requires <MessageQueueName; SemicolonDelimitedMessageContents; Timeout>.")

        Dim p2 As Integer = s.LastIndexOf(";"c)
        If p2 = p1 Then Throw New FormatException("$ExecAppFunc requires <MessageQueueName; SemicolonDelimitedMessageContents; Timeout>.")

        Dim oLoggingInfo As StringBuilder = Nothing
        If Log.LoggingDebug Then
            oLoggingInfo = New StringBuilder(s.Length * 3)
        End If

        Dim sMqName As String
        With Nothing
            Dim s2 As String = s.Substring(0, p1).Trim(oSpaces)
            sMqName = ReplaceSafeWordToRawChar(s2)
            If oLoggingInfo IsNot Nothing Then
                oLoggingInfo.Append("MQ<" & ReplaceSafeWordToSymWord(s2) & ">")
            End If
        End With

        Dim sFunc As String
        Dim oArgs As String() = Nothing
        With Nothing
            Dim sContent As String = s.Substring(p1 + 1, p2 - (p1 + 1))
            Dim p3 As Integer = sContent.IndexOf(";"c)
            If p3 = -1 Then
                Dim s2 As String = sContent.Trim(oSpaces)
                sFunc = ReplaceSafeWordToRawChar(s2)
                If oLoggingInfo IsNot Nothing Then
                    oLoggingInfo.Append(" FN<" & ReplaceSafeWordToSymWord(s2) & ">")
                End If
            Else
                sFunc = ReplaceSafeWordToRawChar(sContent.Substring(0, p3).Trim(oSpaces))
                oArgs = sContent.Substring(p3 + 1).Split(oFuncArgSeps)
                For i As Integer = 0 To oArgs.Length - 1
                    Dim s2 As String = oArgs(i).Trim(oSpaces)
                    oArgs(i) = ReplaceSafeWordToRawChar(s2)
                    If oLoggingInfo IsNot Nothing Then
                        oLoggingInfo.Append(" A" & i.ToString() & "<" & ReplaceSafeWordToSymWord(s2) & ">")
                    End If
                Next i
            End If
        End With

        Dim waitLimitTicks As Integer
        Try
            waitLimitTicks = CTypeTicks(ReplaceSafeWordToRawChar(s.Substring(p2 + 1).Trim(oSpaces)))
        Catch ex As Exception
            Throw New FormatException("$ExecAppFunc requires <MessageQueueName; SemicolonDelimitedMessageContents; Timeout>.", ex)
        End Try

        Dim sWorkingDir As String = Path.Combine(oPander.sMachineDir, "#" & contextNum.ToString())
        Directory.CreateDirectory(sWorkingDir)

        If oLoggingInfo IsNot Nothing Then
            Log.Debug("$ExecAppFunc " & oLoggingInfo.ToString() & " start...")
        End If

        Dim oOutMessage As New Message()
        Using oTargetQueue As New MessageQueue(".\private$\" & sMqName)
            Dim bd As ExtAppFuncMessageBody
            bd.WorkingDirectory = sWorkingDir
            bd.Func = sFunc
            bd.Args = oArgs
            bd.Result = Nothing
            oOutMessage.Body = bd
            oOutMessage.ResponseQueue = oPander.oExtAppTargetQueue
            oTargetQueue.Send(oOutMessage)
        End Using

        'NOTE: この間、通信用ソケットや通信用タイマを参照できないが、
        '外部プロセスは短時間で終わる（waitLimitTicksは
        '最大でも10秒程度である）想定である。
        Dim oJoinLimitTimer As New TickTimer(waitLimitTicks)
        oJoinLimitTimer.Start(TickTimer.GetSystemTick())
        Dim oCheckReadList As New ArrayList()
        Do
            Dim ticks As Integer = CInt(oJoinLimitTimer.GetTicksToTimeout(TickTimer.GetSystemTick()))
            If ticks <= 0 Then
                Throw New TimeoutException("$ExecAppFunc timed out.")
            End If

            'ソケットが読み出し可能になるか次のタイムアウトが発生するまで待機する。
            oCheckReadList.Add(oPander.oParentMessageSock)
            Socket.Select(oCheckReadList, Nothing, Nothing, ticks * 1000)

            If oCheckReadList.Count > 0 Then
                'TODO: 万が一、下記のGetInstanceFromSocket()で例外が発生した
                '場合は、oParentMessageSockから既に中途半端に読出しを行って
                'いたり、oParentMessageSock.BlockingがFalseに変更されている
                '可能性もあるため、シナリオの異常終了で済ませるべきではない。
                'TelegrapherのAbort()を行うべきである。
                Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oPander.oParentMessageSock)
                Select Case oRcvMsg.Kind
                    Case MyInternalMessageKind.AppFuncEndNotice
                        Dim oExt As AppFuncEndNoticeExtendPart = AppFuncEndNotice.Parse(oRcvMsg).ExtendPart
                        If oExt.CorrelationId = oOutMessage.Id Then
                            If oExt.Completed Then
                                s = oExt.Result
                                If s Is Nothing Then
                                    s = String.Empty
                                End If
                                Exit Do
                            Else
                                Throw New FormatException("$ExecAppFunc rejected.")
                            End If
                        Else
                            Log.Warn("Response of past AppFuncMessage received.")
                        End If

                    Case MyInternalMessageKind.QuitRequest, MyInternalMessageKind.ScenarioStartRequest, MyInternalMessageKind.ScenarioStopRequest
                        oPander.oParentMessageQueue.AddLast(oRcvMsg)
                        oPander.oPostponeParentMessages()
                        Log.Info("$ExecAppFunc interrupted.")
                        Throw New OperationCanceledException("$ExecAppFunc interrupted.")

                    Case Else
                        oPander.oParentMessageQueue.AddLast(oRcvMsg)
                        oPander.oPostponeParentMessages()
                End Select
            Else
                Throw New TimeoutException("$ExecAppFunc timed out.")
            End If
        Loop

        If oLoggingInfo IsNot Nothing Then
            If s.Length = 0 Then
                Log.Debug("$ExecAppFunc completed with no output.")
            Else
                Log.Debug("$ExecAppFunc completed with following output." & vbCrLf & s)
            End If
        End If
        Return ReplaceRawCharToSafeWord(s)
    End Function

    Private Shared Function GetCharFromHalfByteX(ByVal b As Byte) As Char
        If b <= 9 Then
            Return ChrW(AscW("0") + b)
        Else
            Return ChrW(b - 10 + AscW("A"c))
        End If
    End Function

    Public Shared Function CTypeTicks(ByVal s As String) As Integer
        Const styles As NumberStyles = NumberStyles.AllowDecimalPoint Or NumberStyles.AllowTrailingWhite
        Dim intTicks As Integer
        If s.EndsWith("ms", StringComparison.OrdinalIgnoreCase) Then
            intTicks = CInt(Double.Parse(s.Substring(0, s.Length - "ms".Length), styles))
        ElseIf s.EndsWith("s", StringComparison.OrdinalIgnoreCase) Then
            intTicks = CInt(Double.Parse(s.Substring(0, s.Length - "s".Length), styles) * 1000)
        ElseIf s.EndsWith("m", StringComparison.OrdinalIgnoreCase) Then
            intTicks = CInt(Double.Parse(s.Substring(0, s.Length - "m".Length), styles) * 1000 * 60)
        ElseIf s.EndsWith("h", StringComparison.OrdinalIgnoreCase) Then
            intTicks = CInt(Double.Parse(s.Substring(0, s.Length - "h".Length), styles) * 1000 * 60 * 60)
        ElseIf s.EndsWith("d", StringComparison.OrdinalIgnoreCase) Then
            intTicks = CInt(Double.Parse(s.Substring(0, s.Length - "d".Length), styles) * 1000 * 60 * 60 * 24)
        Else
            Throw New FormatException("The param ends with illegal unit.")
        End If
        If intTicks < 0 OrElse intTicks > Integer.MaxValue \ 4 Then
            Throw New FormatException("The param value is out of valid range.")
        End If
        Return intTicks
    End Function

End Class


Public Class VarHolder

    'NOTE: Varがリファレンス型変数なら他のVarHolderを、値型変数ならStringを指す。
    Public Value As Object

End Class
