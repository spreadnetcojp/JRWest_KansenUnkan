' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2018 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2018/02/22  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.CodeDom.Compiler
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions


'NOTE: ����f�B���N�g����sCacheBasePath�Ƃ��邱�̃N���X�̃C���X�^���X�͂P�����ł��邪�A
'���̃C���X�^���X�͕����̃X���b�h����r�����ӎ����邱�ƂȂ��g�p�\�ł���B
'�{���́A���̃N���X�̐Ӗ��̓I�u�W�F�N�g�t�@�C�����L���b�V�����邱�Ƃ̂ɂ݂ɂƂǂ߂āA
'�P��̃X���b�h����̂ݎg�p�\�Ƃ��A���̃X���b�h���g�p����Assembly���A���̃X���b�h��
'�ӔC�������ă��[�h���A�A�v���P�[�V�������x���ŗp�ӂ���Dictionary(Of String, Assembly)
'����ēn���i���� Dictionary �̓A�v���P�[�V�������r���I�ɃA�N�Z�X����j���j�ɂ��Ă��悢���A
'���������I�u�W�F�N�g�R�[�h�̃L�[�́A���̃N���X�̊Ǘ�����f�B���N�g���̃p�X�ł��邽�߁A
'�������������Ƃ����̃N���X�ɔC������j�ł���B�Ȃ��A�ň��A�A�v�����炱�̃N���X��
'�C���X�^���X���̂��̂�SyncLock���Ďg�p������j�����蓾�����A���\�I�ɍň��Ȃ͂��ł���
'���߁A��߂Ă���B
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

    'NOTE: sCacheBasePath�̃T�u�f�B���N�g���́A����\�[�X�R�[�h�̍ăR���p�C����
    '�s�v�ɂ��邽�߂́u�\�[�X�t�@�C�� vs �I�u�W�F�N�g�t�@�C���v�̃L���b�V���ł���B
    '��Ƀv���Z�X���ċN�������Ƃ��Ɋ�������邱�ƂɂȂ邪�A�\�[�X�R�[�h�ɑΉ�����
    '��L�����Assembly��T���ۂɂ��A�\�[�X�R�[�h�̎��ʎq�i�\�[�X�R�[�h��
    '�n�b�V���l�݂̂Ō��܂�킯�ł͂Ȃ��A�}�Ԃ����ׂȂ���΂Ȃ�Ȃ��j��
    '�����ŕK�{�ł���B
    'NOTE: oLoadedAssemblies�͎�L����ւ̓���I�u�W�F�N�g�t�@�C���̃��[�h��
    '���x���s��Ȃ��悤�ɂ��邽�߂́u�I�u�W�F�N�g�t�@�C�� vs Assembly�C���X�^���X�v
    '�̃L���b�V���ł���BDynAssemblyManager���g�p����v���O���}�̓I�u�W�F�N�g�t�@�C��
    '���ӎ������A�\�[�X�R�[�h�݂̂��炻��ɑΉ�����Assembly�𓾂����킯�ł��邩��A
    '.NET Framework��GAC�͎g�p���Ȃ��B
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

        'NOTE: sCacheBasePath�̉��ɂ̓n�b�V���l�ʂ̃T�u�f�B���N�g�������邪�A
        '���̂���ɉ��Ɂi�n�b�V���l�̏Փ˂�z�肵�āj�}�Ԃ̃f�B���N�g��������B
        '�ȉ��ł́A�n�b�V���l�ʂ̃I�u�W�F�N�g�����b�N�L�[�ɂ��āA���̃n�b�V���l��
        '�f�B���N�g���ւ̃A�N�Z�X��r�����䂷��i�n�b�V���l������̃\�[�X�R�[�h��
        '���Ă̈ȉ��̏����́A�����̃X���b�h���瓯���ɂ͍s��Ȃ��悤�ɂ���j�B
        Dim hash As Integer = sPreProCode.GetHashCode()
        Dim sHash As String = hash.ToString("X8")
        Dim sHashPath As String = Path.Combine(Path.Combine(sCacheBasePath, oSpec.ShortName), sHash)

        'NOTE: ���̎����ł́AoLockingObjectForHashCode�̗v�f�͗��_��̍ő��
        '�n�b�V���l�̎�ނƓ������i2^32�j�ɂȂ��A�v���Z�X�����s����
        '�v�f�����炷�@����Ȃ����A�Ó��Ȏ����ł���B���ɃV�i���I����.NET
        '�R�[�h�����x�����ς��āAsCacheBasePath�̔z���ɑ��݂���I�u�W�F�N�g
        '�t�@�C�������������A�S�Ẵn�b�V���l�ɑ�������f�B���N�g�����ł���
        '�Ƃ��Ă��A���ۂ�oLockingObjectForHashCode�ɒǉ������v�f�̌��́A
        '�ő�Łu�v���Z�X���N�����Ă��烍�[�h�����I�u�W�F�N�g�t�@�C���̌��v
        '�ɂȂ�킯�ł���A���̏󋵂Ŏ�L������������̂�
        'oLockingObjectForHashCode�₻�̗v�f�ɂȂ��Ă���Object�ł͂Ȃ��A
        '���[�h���ꂽAssembly�̂͂��ł���B�܂�A�V�~�����[�^�̉^�p���@�I��
        '���̎��������ɂȂ�̂ł���΁A�܂��́i�V�i���I���̃R�[�h��p��
        'AppDomain��p�ӂ��ājAssembly���A�����[�h�ł���悤�ɂ���ׂ��ł���A
        '��������Ȃ�A���̍ۂ́A�W���C�A���g���b�N���s�����ƂɂȂ�͂����邩��A
        '���̊Ԃ�oLockingObjectForHashCode.Clear()���s���܂łł���B
        'OPT: �ǂ����Ă�oLockingObjectForHashCode�̎g�p���郁���������ʂɎv����
        '�Ȃ�A�n�b�V���l�̃y�[�W���O���s���i�ł��P���Ȏ����Ƃ��ẮA
        'hash = sPreProCode.GetHashCode() \ 65536
        '�ȂǂƂ��āA�n�b�V���l���̂��͈̂̔͂����߂�j���ƂŁA��������
        '�����R�X�g�ƈ��������ɗe�ʂ��팸�ł���B
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

                        'OPT: �Œ蒷�̃o�b�t�@��p�ӂ��āA�t�@�C������o�b�t�@�T�C�Y����
                        '�ǂނ��Ƃɔ�r��������悢�B
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
                'TODO: .NET Framework 4.0�ȏ�Ȃ�A���L���悢�B
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
