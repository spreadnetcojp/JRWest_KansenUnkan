' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2018 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2018/02/22  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.CodeDom.Compiler
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions


'NOTE: あるディレクトリをsCacheBasePathとするこのクラスのインスタンスは１つだけであるが、
'そのインスタンスは複数のスレッドから排他を意識することなく使用可能である。
'本当は、このクラスの責務はオブジェクトファイルをキャッシュすることのにみにとどめて、
'単一のスレッドからのみ使用可能とし、他のスレッドが使用するAssemblyも、そのスレッドが
'責任をもってロードし、アプリケーションレベルで用意したDictionary(Of String, Assembly)
'を介して渡す（この Dictionary はアプリケーションが排他的にアクセスする）方針にしてもよいが、
'そもそもオブジェクトコードのキーは、このクラスの管理するディレクトリのパスであるため、
'そういったこともこのクラスに任せる方針である。なお、最悪、アプリからこのクラスの
'インスタンスそのものをSyncLockして使用する方針もあり得たが、性能的に最悪なはずである
'ため、やめている。
Public Class DynAssemblyManager

    Private Shared ReadOnly oProviderOptions As New Dictionary(Of String, String) From {{"CompilerVersion", "v3.5"}}
    Private Shared ReadOnly oSrcLineSeparators As String() = {vbCrLf}

    Private Class LangSpec
        Public IncludeDirectiveRegx As Regex
        Public ReferDirectiveRegx As Regex
        Public NamespaceUsingRegx As Regex
        Public CommentBeginning As String
        Public ShortName As String
        Public Sub New(ByVal oIncludeDirectiveRegx As Regex, ByVal oReferDirectiveRegx As Regex, ByVal oNamespaceUsingRegx As Regex, ByVal sCommentBeginning As String, ByVal sShortName As String)
            IncludeDirectiveRegx = oIncludeDirectiveRegx
            ReferDirectiveRegx = oReferDirectiveRegx
            NamespaceUsingRegx = oNamespaceUsingRegx
            CommentBeginning = sCommentBeginning
            ShortName = sShortName
        End Sub
    End Class

    Private Shared ReadOnly oIncludeDirectiveRegxVb As New Regex("^\s*#\s*Include\s*(<[^<>""]+>|""[^<>""]+"")\s*('.*)?$", RegexOptions.CultureInvariant Or RegexOptions.Compiled Or RegexOptions.IgnoreCase)
    Private Shared ReadOnly oIncludeDirectiveRegxCs As New Regex("^\s*#\s*include\s*(<[^<>""]+>|""[^<>""]+"")\s*(//.*)?$", RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly oReferDirectiveRegxVb As New Regex("^\s*#\s*Refer\s*(<[^<>""]+>|""[^<>""]+"")\s*('.*)?$", RegexOptions.CultureInvariant Or RegexOptions.Compiled Or RegexOptions.IgnoreCase)
    Private Shared ReadOnly oReferDirectiveRegxCs As New Regex("^\s*#\s*refer\s*(<[^<>""]+>|""[^<>""]+"")\s*(//.*)?$", RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly oNamespaceUsingRegxVb As New Regex("^\s*Imports\s+([^<>""\s]+)\s*('.*)?$", RegexOptions.CultureInvariant Or RegexOptions.Compiled Or RegexOptions.IgnoreCase)
    Private Shared ReadOnly oNamespaceUsingRegxCs As New Regex("^\s*using\s+([^<>""\s]+)\s*;\s*(//.*)?$", RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly oSpecOfLang As New Dictionary(Of String, LangSpec) From { _
        {"VisualBasic", New LangSpec(oIncludeDirectiveRegxVb, oReferDirectiveRegxVb, oNamespaceUsingRegxVb, "'", "vb")}, _
        {"CSharp", New LangSpec(oIncludeDirectiveRegxCs, oReferDirectiveRegxCs, oNamespaceUsingRegxCs, "//", "cs")}}

    Private Class CodeSegment
        Public SourceName As String
        Public LineNumberOrigin As Integer
        Public Codes As ArraySegment(Of String)
        Public Sub New(ByVal sSourceName As String, ByVal lineNumOrigin As Integer, ByVal oCodes As ArraySegment(Of String))
            SourceName = sSourceName
            LineNumberOrigin = lineNumOrigin
            Codes = oCodes
        End Sub
    End Class

    'NOTE: sCacheBasePathのサブディレクトリは、同一ソースコードの再コンパイルを
    '不要にするための「ソースファイル vs オブジェクトファイル」のキャッシュである。
    '主にプロセスを再起動したときに活かされることになるが、ソースコードに対応する
    '主記憶上のAssemblyを探す際にも、ソースコードの識別子（ソースコードの
    'ハッシュ値のみで決まるわけではなく、枝番も調べなければならない）を
    '得る上で必須である。
    'NOTE: oLoadedAssembliesは主記憶上への同一オブジェクトファイルのロードを
    '何度も行わないようにするための「オブジェクトファイル vs Assemblyインスタンス」
    'のキャッシュである。DynAssemblyManagerを使用するプログラマはオブジェクトファイル
    'を意識せず、ソースコードのみからそれに対応するAssemblyを得たいわけであるから、
    '.NET FrameworkのGACは使用しない。
    Private sCacheBasePath As String
    Private oLoadedAssemblies As New Dictionary(Of String, Assembly)()
    Private oLockingObjectForHashCode As New Dictionary(Of Integer, Object)()

    Public Sub New(ByVal sCacheBasePath As String)
        Me.sCacheBasePath = sCacheBasePath
    End Sub

    Public Function GetAssembly(ByVal sLangName As String, ByVal oSrcLines As String(), ByVal sSourceName As String, ByVal lineNumOrigin As Integer, ByVal sSysIncludeBasePath As String, ByVal sUsrIncludeBasePath As String, ByVal sUsrReferBasePath As String) As Assembly
        Dim oIncludedSrcs As New HashSet(Of String)()
        Dim oReferencedAsms As New HashSet(Of String)()
        Dim oUsingNamespaces As New HashSet(Of String)()
        Dim oPreProCodeSegs As New LinkedList(Of CodeSegment)
        Dim oSpec As LangSpec = oSpecOfLang(sLangName)
        AddSrcToPreProCode(oSpec, oSrcLines, sSourceName, lineNumOrigin, sSysIncludeBasePath, sUsrIncludeBasePath, sUsrReferBasePath, oIncludedSrcs, oReferencedAsms, oUsingNamespaces, oPreProCodeSegs)

        Dim sPreProCode As String
        Dim lineCount As Integer = 0
        With Nothing
            Dim oBuilder As New StringBuilder()
            For Each oSeg As CodeSegment In oPreProCodeSegs
                For i As Integer = oSeg.Codes.Offset To oSeg.Codes.Offset + oSeg.Codes.Count - 1
                    oBuilder.AppendLine(oSeg.Codes.Array(i))
                    lineCount += 1
                Next i
            Next oSeg
            sPreProCode = oBuilder.ToString()
        End With

        'NOTE: sCacheBasePathの下にはハッシュ値別のサブディレクトリがあるが、
        'そのさらに下に（ハッシュ値の衝突を想定して）枝番のディレクトリがある。
        '以下では、ハッシュ値別のオブジェクトをロックキーにして、このハッシュ値の
        'ディレクトリへのアクセスを排他制御する（ハッシュ値が同一のソースコードに
        'ついての以下の処理は、複数のスレッドから同時には行わないようにする）。
        Dim hash As Integer = sPreProCode.GetHashCode()
        Dim sHash As String = hash.ToString("X8")
        Dim sHashPath As String = Path.Combine(Path.Combine(sCacheBasePath, oSpec.ShortName), sHash)

        'NOTE: この実装では、oLockingObjectForHashCodeの要素は理論上の最大で
        'ハッシュ値の種類と同じ個数（2^32個）になる上、プロセスを実行中に
        '要素を減らす機会もないが、妥当な実装である。仮にシナリオ内の.NET
        'コードを何度も改変して、sCacheBasePathの配下に存在するオブジェクト
        'ファイルが増え続け、全てのハッシュ値に相当するディレクトリができた
        'としても、実際にoLockingObjectForHashCodeに追加される要素の個数は、
        '最大で「プロセスが起動してからロードしたオブジェクトファイルの個数」
        'になるわけであり、その状況で主記憶を圧迫するのは
        'oLockingObjectForHashCodeやその要素になっているObjectではなく、
        'ロードされたAssemblyのはずである。つまり、シミュレータの運用方法的に
        'この実装が問題になるのであれば、まずは（シナリオ内のコード専用の
        'AppDomainを用意して）Assemblyをアンロードできるようにするべきであり、
        'そうするなら、その際は、ジャイアントロックを行うことになるはずあるから、
        'その間にoLockingObjectForHashCode.Clear()を行うまでである。
        'OPT: どうしてもoLockingObjectForHashCodeの使用するメモリが無駄に思える
        'なら、ハッシュ値のページングを行う（最も単純な実装としては、
        'hash = sPreProCode.GetHashCode() \ 65536
        'などとして、ハッシュ値そのものの範囲を狭める）ことで、検索時の
        '処理コストと引き換えに容量を削減できる。
        Dim oLockingObj As Object = Nothing
        SyncLock oLockingObjectForHashCode
            If oLockingObjectForHashCode.TryGetValue(hash, oLockingObj) = False Then
                oLockingObj = New Object()
                oLockingObjectForHashCode.Add(hash, oLockingObj)
            End If
        End SyncLock

        Dim sObjPath As String
        Dim sErrorContent As String
        SyncLock oLockingObj
            Dim branchNum As Integer
            Dim sBranchPath As String
            If Directory.Exists(sHashPath) Then
                Do
                    For branchNum = 0 To 9999
                        sBranchPath = Path.Combine(sHashPath, branchNum.ToString())
                        If Not Directory.Exists(sBranchPath) Then Exit Do

                        'OPT: 固定長のバッファを用意して、ファイルからバッファサイズ分を
                        '読むごとに比較する方がよい。
                        Dim sSrcPath As String = Path.Combine(sBranchPath, "program." & oSpec.ShortName)
                        If File.Exists(sSrcPath) Then
                            Dim sArchiveCode As String
                            Using oReader As New StreamReader(sSrcPath)
                                sArchiveCode = oReader.ReadToEnd()
                            End Using

                            If sPreProCode.Equals(sArchiveCode) Then
                                sObjPath = Path.Combine(sBranchPath, "program.dll")
                                If File.Exists(sObjPath) Then
                                    SyncLock oLoadedAssemblies
                                        Dim sKey As String = sHash & branchNum.ToString()
                                        Dim oAsm As Assembly = Nothing
                                        If oLoadedAssemblies.TryGetValue(sKey, oAsm) = False Then
                                            oAsm = [Assembly].LoadFile(sObjPath)
                                            oLoadedAssemblies.Add(sKey, oAsm)
                                        End If
                                        Return oAsm
                                    End SyncLock
                                End If
                                Dim sErrPath As String = Path.Combine(sBranchPath, "errors.txt")
                                If File.Exists(sErrPath) Then
                                    Using oReader As New StreamReader(sErrPath)
                                        sErrorContent = oReader.ReadToEnd()
                                    End Using
                                    Throw New FormatException(sErrorContent)
                                End If
                                Exit Do
                            End If
                        End If
                    Next branchNum
                    Throw New DirectoryNotFoundException(sHashPath & "is full." & vbCrLf & "Please delete it.")
                Loop While False
            Else
                branchNum = 0
                sBranchPath = Path.Combine(sHashPath, "0")
            End If

            Directory.CreateDirectory(sBranchPath)
            sObjPath = Path.Combine(sBranchPath, "program.dll")
            Try
                Dim oParams As New CompilerParameters()
                For Each sAsm As String In oReferencedAsms
                    oParams.ReferencedAssemblies.Add(sAsm)
                Next sAsm
                'oParams.GenerateExecutable = False
                oParams.OutputAssembly = sObjPath
                oParams.GenerateInMemory = False

                Dim oResult As CompilerResults
                '------------------------------------------------------------------
                'TODO: .NET Framework 4.0以上なら、下記がよい。
                'Using oCodePro As CodeDomProvider = CodeDomProvider.CreateProvider(sLangName, oProviderOptions)
                '    oResult = oCodePro.CompileAssemblyFromSource(oParams, sPreProCode)
                'End Using
                '------------------------------------------------------------------
                If sLangName = "VisualBasic" Then
                    Using oCodePro As CodeDomProvider = New Microsoft.VisualBasic.VBCodeProvider(oProviderOptions)
                        oResult = oCodePro.CompileAssemblyFromSource(oParams, sPreProCode)
                    End Using
                ElseIf sLangName = "CSharp" Then
                    Using oCodePro As CodeDomProvider = New Microsoft.CSharp.CSharpCodeProvider(oProviderOptions)
                        oResult = oCodePro.CompileAssemblyFromSource(oParams, sPreProCode)
                    End Using
                Else
                    Using oCodePro As CodeDomProvider = CodeDomProvider.CreateProvider(sLangName)
                        oResult = oCodePro.CompileAssemblyFromSource(oParams, sPreProCode)
                    End Using
                End If

                With Nothing
                    Dim oBuilder As New StringBuilder()
                    oBuilder.Append(sLangName)
                    oBuilder.Append(" compiler returned with result code: ")
                    oBuilder.AppendLine(oResult.NativeCompilerReturnValue.ToString())
                    'For i As Integer = 0 To oResult.Output.Count - 1
                    '    oBuilder.AppendLine(oResult.Output(i))
                    'Next i
                    If oResult.Errors.Count > 0 Then
                        Dim oSrcNames(lineCount - 1) As String
                        Dim oSrcLineNumbers(lineCount - 1) As Integer
                        lineCount = 0
                        For Each oSeg As CodeSegment In oPreProCodeSegs
                            For i As Integer = oSeg.Codes.Offset To oSeg.Codes.Offset + oSeg.Codes.Count - 1
                                oSrcNames(lineCount) = oSeg.SourceName
                                oSrcLineNumbers(lineCount) = oSeg.LineNumberOrigin + i
                                lineCount += 1
                            Next i
                        Next oSeg
                        For iErrors As Integer = 0 To oResult.Errors.Count - 1
                            Dim oError As CompilerError = oResult.Errors(iErrors)
                            Dim i As Integer = oError.Line - 1
                            If i <> -1 Then
                                oBuilder.Append(oSrcNames(i))
                                oBuilder.Append("(")
                                oBuilder.Append(oSrcLineNumbers(i))
                                oBuilder.Append(") : ")
                            End If
                            oBuilder.Append(If(oError.IsWarning, "warning ", "error "))
                            oBuilder.Append(oError.ErrorNumber)
                            oBuilder.Append(": ")
                            oBuilder.AppendLine(oError.ErrorText)
                        Next iErrors
                    End If
                    sErrorContent = oBuilder.ToString()
                End With

                Using oWriter As New StreamWriter(Path.Combine(sBranchPath, "errors.txt"))
                    oWriter.Write(sErrorContent)
                End Using

                Using oWriter As New StreamWriter(Path.Combine(sBranchPath, "program." & oSpec.ShortName))
                    oWriter.Write(sPreProCode)
                End Using
            Catch ex As Exception
                Directory.Delete(sBranchPath)
                Throw
            End Try

            If Not File.Exists(sObjPath) Then
                Throw New FormatException(sErrorContent)
            End If

            SyncLock oLoadedAssemblies
                Dim sKey As String = sHash & branchNum.ToString()
                Dim oAsm As Assembly = Nothing
                If oLoadedAssemblies.TryGetValue(sKey, oAsm) = False Then
                    oAsm = [Assembly].LoadFile(sObjPath)
                    oLoadedAssemblies.Add(sKey, oAsm)
                End If
                Return oAsm
            End SyncLock
        End SyncLock
    End Function

    Private Sub AddSrcToPreProCode( _
      ByVal oSpec As LangSpec, _
      ByVal oSrcLines As String(), ByVal sSourceName As String, ByVal lineNumOrigin As Integer, _
      ByVal sSysIncludeBasePath As String, ByVal sUsrIncludeBasePath As String, ByVal sUsrReferBasePath As String, _
      ByVal oIncludedSrcs As HashSet(Of String), ByVal oReferencedAsms As HashSet(Of String), ByVal oUsingNamespaces As HashSet(Of String), _
      ByVal oPreProCode As LinkedList(Of CodeSegment))
        Dim srcSegStartPos As Integer = 0
        Dim srcPos As Integer
        For srcPos = 0 To oSrcLines.Length - 1
            Try
                Dim sSrcLine As String = oSrcLines(srcPos)
                Dim oMatch As Match

                oMatch = oSpec.IncludeDirectiveRegx.Match(sSrcLine)
                If oMatch.Success Then
                    If srcSegStartPos <> srcPos Then
                        oPreProCode.AddLast(New CodeSegment(sSourceName, lineNumOrigin, New ArraySegment(Of String)(oSrcLines, srcSegStartPos, srcPos - srcSegStartPos)))
                    End If
                    srcSegStartPos = srcPos + 1

                    Dim sMatchPart As String = oMatch.Groups(1).Value
                    Dim sFilePath As String = If(sMatchPart.Chars(0) = "<"c, sSysIncludeBasePath, sUsrIncludeBasePath)
                    sFilePath = Path.Combine(sFilePath, sMatchPart.Substring(1, sMatchPart.Length - 2))
                    Dim oFileInfo As New FileInfo(sFilePath)
                    sFilePath = oFileInfo.FullName

                    If oIncludedSrcs.Add(sFilePath.ToUpperInvariant()) = True Then
                        Dim sSubSourceCode As String
                        Using oReader As New StreamReader(sFilePath, Encoding.Default)
                            sSubSourceCode = oReader.ReadToEnd()
                        End Using
                        Dim oSubSrcLines As String() = sSubSourceCode.Split(oSrcLineSeparators, StringSplitOptions.None)
                        AddSrcToPreProCode(oSpec, oSubSrcLines, sFilePath, 1, sSysIncludeBasePath, sUsrIncludeBasePath, sUsrReferBasePath, oIncludedSrcs, oReferencedAsms, oUsingNamespaces, oPreProCode)
                    End If
                    Continue For
                End If

                oMatch = oSpec.ReferDirectiveRegx.Match(sSrcLine)
                If oMatch.Success Then
                    oSrcLines(srcPos) = oSpec.CommentBeginning & sSrcLine

                    Dim sMatchPart As String = oMatch.Groups(1).Value
                    Dim sFilePath As String = sMatchPart.Substring(1, sMatchPart.Length - 2)
                    If sMatchPart.Chars(0) = """"c Then
                        sFilePath = Path.Combine(sUsrReferBasePath, sFilePath)
                        Dim oFileInfo As New FileInfo(sFilePath)
                        sFilePath = oFileInfo.FullName
                    End If

                    oReferencedAsms.Add(sFilePath.ToUpperInvariant())
                    Continue For
                End If

                oMatch = oSpec.NamespaceUsingRegx.Match(sSrcLine)
                If oMatch.Success Then
                    Dim sNameSpace As String = oMatch.Groups(1).Value
                    If oUsingNamespaces.Add(sNameSpace.ToUpperInvariant()) = False Then
                        oSrcLines(srcPos) = oSpec.CommentBeginning & sSrcLine
                    End If
                    Continue For
                End If

            Catch ex As Exception
                Throw New FormatException("Preprocess error occurred at " & sSourceName & "(" & (lineNumOrigin + srcPos).ToString() & ")", ex)
            End Try
        Next srcPos

        Debug.Assert(srcPos = oSrcLines.Length)
        If srcSegStartPos <> srcPos Then
            oPreProCode.AddLast(New CodeSegment(sSourceName, lineNumOrigin, New ArraySegment(Of String)(oSrcLines, srcSegStartPos, srcPos - srcSegStartPos)))
        End If
    End Sub

End Class
