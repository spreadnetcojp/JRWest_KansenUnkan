' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/01/14  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �V�i���I��Ǎ���Ŏ��s����N���X�B
''' </summary>
Public Class ScenarioEnv

    Public Delegate Function ConnectDelegate() As Integer

    Public Delegate Sub DisconnectDelegate()

    Public Delegate Function SendReplyTelegramDelegate( _
       ByVal iReplyTeleg As ITelegram, _
       ByVal iSourceTeleg As ITelegram) As Boolean

    Public Delegate Function SendNakTelegramDelegate( _
       ByVal cause As NakCauseCode, _
       ByVal oSourceTeleg As ITelegram) As Boolean

    Public Delegate Sub RegisterActiveOneDelegate( _
       ByVal oReqTeleg As IReqTelegram, _
       ByVal retryIntervalTicks As Integer, _
       ByVal limitNakCountToForget As Integer, _
       ByVal limitNakCountToCare As Integer, _
       ByVal sSeqName As String)

    Public Delegate Sub RegisterActiveUllDelegate( _
       ByVal oXllReqTeleg As IXllReqTelegram, _
       ByVal retryIntervalTicks As Integer, _
       ByVal limitNakCountToForget As Integer, _
       ByVal limitNakCountToCare As Integer)

    Public Delegate Sub RegisterTimerDelegate( _
       ByVal oTimer As TickTimer, _
       ByVal systemTick As Long)

    Public Delegate Sub UnregisterTimerDelegate( _
       ByVal oTimer As TickTimer)

    Private Enum StatementVerb As Integer
        [GoTo]
        [Call]
        ExitProc
        FinishContext
        Connect
        Disconnect
        ActiveOne
        TryActiveOne
        ActiveUll
        TryActiveUll
        WaitForPassiveOne
        WaitForPassiveOneToNak
        WaitForPassiveUll
        WaitForPassiveUllToNak
        WaitForPassiveDll
        WaitForPassiveDllToNak
        Wait
        WaitUntil
        RegPassiveOneProc
        RegPassiveOneProcToNak
        UnregPassiveOneProc
        RegPassiveUllProc
        RegPassiveUllProcToNak
        UnregPassiveUllProc
        RegPassiveDllProc
        RegPassiveDllProcToNak
        UnregPassiveDllProc
        RegDisconnectProc
        UnregDisconnectProc
        RegTimerProc
        UnregTimerProc
        FinishScenario
        AbortScenario
        Evaluate
        Print
        CheckBinFile
        CheckCsvFile
    End Enum

    Private Structure StatementParam
        Public IsExpanded As Boolean
        Public Value As Object
    End Structure

    Private Class DllResultInfo
        Public ContinueCode As ContinueCode
        Public ResultantVersionOfSlot1 As Integer
        Public ResultantVersionOfSlot2 As Integer
        Public ResultantFlagOfFull As Integer
        Public Sub New(ByVal s As String)
            Dim sInfoElems As String() = s.Split(";"c)

            If sInfoElems.Length <> 4 Then
                Throw New FormatException("Bad arity of DllResultInfo.")
            End If
            For i As Integer = 0 To 3
                sInfoElems(i) = sInfoElems(i).Trim()
            Next i

            Me.ContinueCode = DirectCast([Enum].Parse(GetType(ContinueCode), sInfoElems(0), True), ContinueCode)
            If Me.ContinueCode <> ContinueCode.Finish AndAlso _
               Me.ContinueCode <> ContinueCode.FinishWithoutStoring AndAlso _
               Me.ContinueCode <> ContinueCode.Abort Then
                Throw New FormatException("The value contains invalid ContinueCode.")
            End If

            Me.ResultantVersionOfSlot1 = Integer.Parse(sInfoElems(1), NumberFormatInfo.InvariantInfo)
            If Me.ResultantVersionOfSlot1 < 0 OrElse Me.ResultantVersionOfSlot1 > 99999999 Then
                Throw New FormatException("The value contains invalid ResultantVersionOfSlot1.")
            End If

            Me.ResultantVersionOfSlot2 = Integer.Parse(sInfoElems(2), NumberFormatInfo.InvariantInfo)
            If Me.ResultantVersionOfSlot2 < 0 OrElse Me.ResultantVersionOfSlot2 > 99999999 Then
                Throw New FormatException("The value contains invalid ResultantVersionOfSlot2.")
            End If

            Me.ResultantFlagOfFull = Integer.Parse(sInfoElems(3), NumberFormatInfo.InvariantInfo)
            If Me.ResultantFlagOfFull < 0 OrElse Me.ResultantFlagOfFull > 255 Then
                Throw New FormatException("The value contains invalid ResultantFlagOfFull.")
            End If
        End Sub
    End Class

    'NOTE: ProcStatement�͎��͌��\�傫���̂ŁAStructure�ł͂Ȃ�Class�Ƃ���B
    '�V�i���I�̍s���Ȃǌ����Ă��邵�A�쐬���������Ɍ������R�X�g�ł�Structure���L���Ƃ͌����Ȃ��B
    '�����āAStructure�Ƃ����ꍇ�́A�V�i���I���̃��[�v�����s���ɉ��x�����{���邱�ƂɂȂ�
    'Procedure.Statements�̗v�f�̎擾�ɂ����āA�l�R�s�[�̃R�X�g���������ƂɂȂ�͂��B
    'OPT: ����̂悤��EkCode�������o�Ƃ���䂦�ɈӊO�Ƒ傫��Structure�͂��邩������Ȃ��̂ŁA
    'EkCode�̊e�v���p�e�B�̒l��ێ����邽�߂̓��������o�ϐ��̌^��؂�l�߂Ă����Ƃ悢�B
    Private Class ProcStatement
        Public Subject As EkCode
        Public Verb As StatementVerb
        Public Params As StatementParam()
        Public LineNumber As Integer '�V�i���I�t�@�C�����̍s�ԍ��i���O�o�͂ł̂ݎg�p�j
        Public Function Clone() As ProcStatement
            Return DirectCast(MemberwiseClone(), ProcStatement)
        End Function
    End Class

    Private Class Procedure
        Public Name As String '�v���V�[�W�����i���[�h���ȊO�́A���O�o�͂ł̂ݎg�p�j
        Public ParamNames As String()
        Public Statements As List(Of ProcStatement)
        Public PosOfLabels As Dictionary(Of String, Integer)
        Public Sub New(ByVal sName As String, ByVal oParamNames As String())
            Me.Name = sName
            Me.ParamNames = oParamNames
            Me.Statements = New List(Of ProcStatement)
            Me.PosOfLabels = New Dictionary(Of String, Integer)
        End Sub
    End Class

    Private Class StackFrame
        Public CallerProcedure As Procedure
        Public CallerPos As Integer
        Public LocalVariables As Dictionary(Of String, VarHolder)
        Public Sub New(ByVal oCallerProc As Procedure, ByVal callerPos As Integer)
            Me.CallerProcedure = oCallerProc
            Me.CallerPos = callerPos
            Me.LocalVariables = New Dictionary(Of String, VarHolder)
        End Sub
    End Class

    Private Class Context
        'NOTE: �R���e�L�X�g���A�\���I�V�[�P���X�̍s�̊����҂��̊Ԃ́A
        '���Y�V�[�P���X�ő��M����REQ�d���̎Q�Ƃ�ExecSeq�ɕێ����邱�ƂɂȂ��Ă���B
        'NOTE: �R���e�L�X�g���A�󓮓I�V�[�P���X�҂��⎞�ԑ҂��ȂǁAWaitFoo�n�̍s��
        '�����҂��̊Ԃ́A���Y�s�����҂��̂��߂�TickTimer�̎Q�Ƃ�ExecTimer��
        '�ێ����邱�ƂɂȂ��Ă���B
        'NOTE: �R���e�L�X�g���A�󓮓I�V�[�P���X�҂��̍s�̊����҂��ł��A
        '���Y�R���e�L�X�g�̓��Y�s�Ɏ��s���̃V�[�P���X���R�Â��Ĉȍ~�́A
        '���Y�V�[�P���X��REQ�d���̎Q�Ƃ�ExecSeq�ɕێ����邱�ƂɂȂ��Ă���B
        Public Number As Integer
        Public StartTime As DateTime
        Public ExecProcedure As Procedure
        Public ExecPos As Integer
        Public ExecSeq As EkReqTelegram
        Public ExecTimer As TickTimer
        Public IterationTargets As String()
        Public IterationPos As Integer
        Public CallStack As Stack(Of StackFrame)

        'NOTE: �����͔�r���Ȃ���ҋ@����ꍇ�̔�r�p�W�J�ς݃p�����[�^�B
        Public TelegCompObj As Object
        Public TelegMaskObj As Object
        Public TelegEvaluationLen As Integer
        Public DataCompObj As Object
        Public DataMaskObj As Object
        Public DataEvaluationLen As Integer
        Public ListCompObj As Object
        Public ListMaskObj As Object
        Public ListEvaluationLen As Integer

        Public Sub New(ByVal num As Integer)
            Me.Number = num
            Me.StartTime = DateTime.Now
            'Me.ExecProcedure = Nothing
            'Me.ExecPos = 0
            'Me.ExecSeq = Nothing
            'Me.ExecTimer = Nothing
            'Me.IterationTargets = Nothing
            'Me.IterationPos = 0
            Me.CallStack = New Stack(Of StackFrame)()
            Me.CallStack.Push(New StackFrame(Nothing, -1))
            'Me.TelegCompObj = Nothing
            'Me.TelegMaskObj = Nothing
            'Me.TelegEvaluationLen = 0
            'Me.DataCompObj = Nothing
            'Me.DataMaskObj = Nothing
            'Me.DataEvaluationLen = 0
            'Me.ListCompObj = Nothing
            'Me.ListMaskObj = Nothing
            'Me.ListEvaluationLen = 0
        End Sub
    End Class

    Private Class PassiveOneHandler
        Public SourceStatement As ProcStatement
        Public TelegCompObj As Object
        Public TelegMaskObj As Object
        Public TelegEvaluationLen As Integer
        Public Sub New(ByVal oEnv As ScenarioEnv, ByVal oSrcStatement As ProcStatement, ByVal oContext As Context)
            Me.SourceStatement = oSrcStatement
            Me.TelegCompObj = oEnv.EvaluateParam(oSrcStatement, 1, oContext)
            Me.TelegMaskObj = oEnv.EvaluateParam(oSrcStatement, 2, oContext)
            Me.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSrcStatement, 3, oContext), Integer)
        End Sub
    End Class

    Private Class PassiveUllHandler
        'NOTE: �n���h���Ɏ��s���̃V�[�P���X���R�Â��Ĉȍ~�A
        '�V�[�P���X���I���܂ł̊Ԃ́A
        '���Y�V�[�P���X��REQ�d���̎Q�Ƃ�BindSeq�ɕێ����邱�ƂɂȂ��Ă���B
        '�܂��A���Y�n���h���ɐݒ肵�����ۂ������������Ƃ�Context�𐶐����A
        '�ʂ̃��\�b�h�Ŏ��s���J�n����ꍇ�A���s���J�n����܂ł̊��Ԃ́A
        '���̎Q�Ƃ�SpawnedContext�ɕێ����邱�ƂɂȂ��Ă���B
        'NOTE: BindSeq��SpawnedContext�ɉ�����ێ����Ă���Ƃ��́A
        '���Y�n���h���ɕʂ̃V�[�P���X�ɕR�Â����Ƃ͂Ȃ����A
        '�����PassiveUll���̂����������Ɏ��s����邱�Ƃ��Ȃ�����
        '�ł���A���̑O�񂪂Ȃ��Ȃ����ꍇ�͎������C������K�v������B
        Public SourceStatement As ProcStatement
        Public TelegCompObj As Object
        Public TelegMaskObj As Object
        Public TelegEvaluationLen As Integer
        Public BindSeq As EkServerDrivenUllReqTelegram
        Public SpawnedContext As Context
        Public Sub New(ByVal oEnv As ScenarioEnv, ByVal oSrcStatement As ProcStatement, ByVal oContext As Context)
            Me.SourceStatement = oSrcStatement
            Me.TelegCompObj = oEnv.EvaluateParam(oSrcStatement, 1, oContext)
            Me.TelegMaskObj = oEnv.EvaluateParam(oSrcStatement, 2, oContext)
            Me.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSrcStatement, 3, oContext), Integer)
            'Me.BindSeq = Nothing
            'Me.SpawnedContext = Nothing
        End Sub
    End Class

    Private Class PassiveDllHandler
        'NOTE: �n���h���Ɏ��s���̃V�[�P���X���R�Â��Ĉȍ~�A
        '�V�[�P���X���I���܂ł̊Ԃ́A
        '���Y�V�[�P���X��REQ�d���̎Q�Ƃ�BindSeq�ɕێ����邱�ƂɂȂ��Ă���B
        '�܂��A���Y�n���h���ɐݒ肵�����ۂ������������Ƃ�Context�𐶐����A
        '�ʂ̃��\�b�h�Ŏ��s���J�n����ꍇ�A���s���J�n����܂ł̊��Ԃ́A
        '���̎Q�Ƃ�SpawnedContext�ɕێ����邱�ƂɂȂ��Ă���B
        'NOTE: BindSeq��SpawnedContext�ɉ�����ێ����Ă���Ƃ��́A
        '���Y�n���h���ɕʂ̃V�[�P���X�ɕR�Â����Ƃ͂Ȃ����A
        '�����PassiveDll���̂����������Ɏ��s����邱�Ƃ��Ȃ�����
        '�ł���A���̑O�񂪂Ȃ��Ȃ����ꍇ�͎������C������K�v������B
        Public SourceStatement As ProcStatement
        Public TelegCompObj As Object
        Public TelegMaskObj As Object
        Public TelegEvaluationLen As Integer
        Public BindSeq As EkMasProDllReqTelegram
        Public SpawnedContext As Context
        Public Sub New(ByVal oEnv As ScenarioEnv, ByVal oSrcStatement As ProcStatement, ByVal oContext As Context)
            Me.SourceStatement = oSrcStatement
            Me.TelegCompObj = oEnv.EvaluateParam(oSrcStatement, 1, oContext)
            Me.TelegMaskObj = oEnv.EvaluateParam(oSrcStatement, 2, oContext)
            Me.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSrcStatement, 3, oContext), Integer)
            'Me.BindSeq = Nothing
            'Me.SpawnedContext = Nothing
        End Sub
    End Class

    Private Class DisconnectHandler
        Public SourceStatement As ProcStatement
        Public Sub New(ByVal oSrcStatement As ProcStatement)
            Me.SourceStatement = oSrcStatement
        End Sub
    End Class

    Private Class TimerHandler
        Public Timer As TickTimer
        Public Count As Integer
        Public SourceStatement As ProcStatement
        Public Sub New(ByVal oTimer As TickTimer, ByVal cnt As Integer, ByVal oSrcStatement As ProcStatement)
            Me.Timer = oTimer
            Me.Count = cnt
            Me.SourceStatement = oSrcStatement
        End Sub
    End Class

    Private Shared ReadOnly oProcBeginningRegx As New Regex("^Proc\s", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly oSubjBeginningRegx As New Regex("^[0-9]+-[0-9]+-[0-9]+-[0-9]+\s", RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly oVerbBeginningRegx As New Regex("^[0-9A-Za-z]+(?=(\s|$))", RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly oVbCodeBeginningRegx As New Regex("^VbCode\s", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly oCsCodeBeginningRegx As New Regex("^CsCode\s", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly DateTimeParseFormats As String() = {"yyyy/M/d H:m:s.fff", "yyyy/M/d H:m:s.ff", "yyyy/M/d H:m:s.f", "yyyy/M/d H:m:s"}
    Private Shared ReadOnly TimeParseFormats As String() = {"H:m:s.fff", "H:m:s.ff", "H:m:s.f", "H:m:s"}

    Private Shared ReadOnly sEntryProcName As String = "Main".ToUpperInvariant()

    Private Enum ParamType As Integer
        [String]
        [Byte]
        [Integer]
        [Boolean]
        Label
        OptLabel
        ProcName
        OptProcName
        ProcParams
        BinFilePath
        OptBinFilePath
        OutBinFilePath
        CsvFilePath
        OptCsvFilePath
        OutCsvFilePath
        Ticks
        DateTime
        NakCauseCode
        OptNakCauseCode
        XllFileHashValue
        DllResultInfo
    End Enum

    Private Delegate Function OneStepDelegate( _
       ByVal oEnv As ScenarioEnv, _
       ByVal oContext As Context, _
       ByVal oSt As ProcStatement) As Boolean

    Private Shared oEnumForVerbText As Dictionary(Of String, StatementVerb)
    Private Shared oParamTypesForVerb()() As ParamType
    Private Shared oDelegateForVerb() As OneStepDelegate

    Shared Sub New()
        oEnumForVerbText = New Dictionary(Of String, StatementVerb)
        Dim verbs As StatementVerb() = CType([Enum].GetValues(GetType(StatementVerb)), StatementVerb())
        For Each verb As StatementVerb In verbs
            oEnumForVerbText.Add(verb.ToString().ToUpperInvariant(), verb)
        Next verb

        oParamTypesForVerb = New ParamType(verbs.Length - 1)() {}
        oDelegateForVerb = New OneStepDelegate(verbs.Length - 1) {}
        Dim t As New List(Of ParamType)

        t.Clear()
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.GoTo) = t.ToArray()
        oDelegateForVerb(StatementVerb.GoTo) = New OneStepDelegate(AddressOf ExecStatementOfGoTo)

        t.Clear()
        t.Add(ParamType.ProcName)
        t.Add(ParamType.ProcParams)
        oParamTypesForVerb(StatementVerb.Call) = t.ToArray()
        oDelegateForVerb(StatementVerb.Call) = New OneStepDelegate(AddressOf ExecStatementOfCall)

        t.Clear()
        oParamTypesForVerb(StatementVerb.ExitProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.ExitProc) = New OneStepDelegate(AddressOf ExecStatementOfExitProc)

        t.Clear()
        oParamTypesForVerb(StatementVerb.FinishContext) = t.ToArray()
        oDelegateForVerb(StatementVerb.FinishContext) = New OneStepDelegate(AddressOf ExecStatementOfFinishContext)

        t.Clear()
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.Connect) = t.ToArray()
        oDelegateForVerb(StatementVerb.Connect) = New OneStepDelegate(AddressOf ExecStatementOfConnect)

        t.Clear()
        oParamTypesForVerb(StatementVerb.Disconnect) = t.ToArray()
        oDelegateForVerb(StatementVerb.Disconnect) = New OneStepDelegate(AddressOf ExecStatementOfDisconnect)

        t.Clear()
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.OutBinFilePath)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Boolean)
        oParamTypesForVerb(StatementVerb.ActiveOne) = t.ToArray()
        oDelegateForVerb(StatementVerb.ActiveOne) = New OneStepDelegate(AddressOf ExecStatementOfActiveOne)

        t.Clear()
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.OutBinFilePath)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Boolean)
        oParamTypesForVerb(StatementVerb.TryActiveOne) = t.ToArray()
        oDelegateForVerb(StatementVerb.TryActiveOne) = New OneStepDelegate(AddressOf ExecStatementOfTryActiveOne)

        t.Clear()
        t.Add(ParamType.Byte)
        t.Add(ParamType.String)
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.XllFileHashValue)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Boolean)
        oParamTypesForVerb(StatementVerb.ActiveUll) = t.ToArray()
        oDelegateForVerb(StatementVerb.ActiveUll) = New OneStepDelegate(AddressOf ExecStatementOfActiveUll)

        t.Clear()
        t.Add(ParamType.Byte)
        t.Add(ParamType.String)
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.XllFileHashValue)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Boolean)
        oParamTypesForVerb(StatementVerb.TryActiveUll) = t.ToArray()
        oDelegateForVerb(StatementVerb.TryActiveUll) = New OneStepDelegate(AddressOf ExecStatementOfTryActiveUll)

        t.Clear()
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.OutBinFilePath)
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.WaitForPassiveOne) = t.ToArray()
        oDelegateForVerb(StatementVerb.WaitForPassiveOne) = New OneStepDelegate(AddressOf ExecStatementOfWaitForPassiveOne)

        t.Clear()
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.OutBinFilePath)
        t.Add(ParamType.NakCauseCode)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.WaitForPassiveOneToNak) = t.ToArray()
        oDelegateForVerb(StatementVerb.WaitForPassiveOneToNak) = New OneStepDelegate(AddressOf ExecStatementOfWaitForPassiveOneToNak)

        t.Clear()
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptNakCauseCode)
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.XllFileHashValue)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.WaitForPassiveUll) = t.ToArray()
        oDelegateForVerb(StatementVerb.WaitForPassiveUll) = New OneStepDelegate(AddressOf ExecStatementOfWaitForPassiveUll)

        t.Clear()
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.NakCauseCode)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.WaitForPassiveUllToNak) = t.ToArray()
        oDelegateForVerb(StatementVerb.WaitForPassiveUllToNak) = New OneStepDelegate(AddressOf ExecStatementOfWaitForPassiveUllToNak)

        t.Clear()
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptNakCauseCode)
        t.Add(ParamType.DllResultInfo)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.WaitForPassiveDll) = t.ToArray()
        oDelegateForVerb(StatementVerb.WaitForPassiveDll) = New OneStepDelegate(AddressOf ExecStatementOfWaitForPassiveDll)

        t.Clear()
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.NakCauseCode)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.WaitForPassiveDllToNak) = t.ToArray()
        oDelegateForVerb(StatementVerb.WaitForPassiveDllToNak) = New OneStepDelegate(AddressOf ExecStatementOfWaitForPassiveDllToNak)

        t.Clear()
        t.Add(ParamType.Ticks)
        oParamTypesForVerb(StatementVerb.Wait) = t.ToArray()
        oDelegateForVerb(StatementVerb.Wait) = New OneStepDelegate(AddressOf ExecStatementOfWait)

        t.Clear()
        t.Add(ParamType.DateTime)
        oParamTypesForVerb(StatementVerb.WaitUntil) = t.ToArray()
        oDelegateForVerb(StatementVerb.WaitUntil) = New OneStepDelegate(AddressOf ExecStatementOfWaitUntil)

        t.Clear()
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.OutBinFilePath)
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.ProcName)
        oParamTypesForVerb(StatementVerb.RegPassiveOneProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.RegPassiveOneProc) = New OneStepDelegate(AddressOf ExecStatementOfRegPassiveOneProc)

        t.Clear()
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.OutBinFilePath)
        t.Add(ParamType.NakCauseCode)
        t.Add(ParamType.ProcName)
        oParamTypesForVerb(StatementVerb.RegPassiveOneProcToNak) = t.ToArray()
        oDelegateForVerb(StatementVerb.RegPassiveOneProcToNak) = New OneStepDelegate(AddressOf ExecStatementOfRegPassiveOneProcToNak)

        t.Clear()
        t.Add(ParamType.Integer)
        oParamTypesForVerb(StatementVerb.UnregPassiveOneProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.UnregPassiveOneProc) = New OneStepDelegate(AddressOf ExecStatementOfUnregPassiveOneProc)

        t.Clear()
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptNakCauseCode)
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.XllFileHashValue)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.ProcName)
        t.Add(ParamType.ProcName)
        t.Add(ParamType.ProcName)
        oParamTypesForVerb(StatementVerb.RegPassiveUllProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.RegPassiveUllProc) = New OneStepDelegate(AddressOf ExecStatementOfRegPassiveUllProc)

        t.Clear()
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.NakCauseCode)
        t.Add(ParamType.ProcName)
        oParamTypesForVerb(StatementVerb.RegPassiveUllProcToNak) = t.ToArray()
        oDelegateForVerb(StatementVerb.RegPassiveUllProcToNak) = New OneStepDelegate(AddressOf ExecStatementOfRegPassiveUllProcToNak)

        t.Clear()
        t.Add(ParamType.Integer)
        oParamTypesForVerb(StatementVerb.UnregPassiveUllProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.UnregPassiveUllProc) = New OneStepDelegate(AddressOf ExecStatementOfUnregPassiveUllProc)

        t.Clear()
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptNakCauseCode)
        t.Add(ParamType.DllResultInfo)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.ProcName)
        t.Add(ParamType.ProcName)
        t.Add(ParamType.ProcName)
        oParamTypesForVerb(StatementVerb.RegPassiveDllProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.RegPassiveDllProc) = New OneStepDelegate(AddressOf ExecStatementOfRegPassiveDllProc)

        t.Clear()
        t.Add(ParamType.Integer)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.NakCauseCode)
        t.Add(ParamType.ProcName)
        oParamTypesForVerb(StatementVerb.RegPassiveDllProcToNak) = t.ToArray()
        oDelegateForVerb(StatementVerb.RegPassiveDllProcToNak) = New OneStepDelegate(AddressOf ExecStatementOfRegPassiveDllProcToNak)

        t.Clear()
        t.Add(ParamType.Integer)
        oParamTypesForVerb(StatementVerb.UnregPassiveDllProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.UnregPassiveDllProc) = New OneStepDelegate(AddressOf ExecStatementOfUnregPassiveDllProc)

        t.Clear()
        t.Add(ParamType.Integer)
        t.Add(ParamType.ProcName)
        oParamTypesForVerb(StatementVerb.RegDisconnectProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.RegDisconnectProc) = New OneStepDelegate(AddressOf ExecStatementOfRegDisconnectProc)

        t.Clear()
        t.Add(ParamType.Integer)
        oParamTypesForVerb(StatementVerb.UnregDisconnectProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.UnregDisconnectProc) = New OneStepDelegate(AddressOf ExecStatementOfUnregDisconnectProc)

        t.Clear()
        t.Add(ParamType.Integer)
        t.Add(ParamType.Integer)
        t.Add(ParamType.Ticks)
        t.Add(ParamType.ProcName)
        oParamTypesForVerb(StatementVerb.RegTimerProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.RegTimerProc) = New OneStepDelegate(AddressOf ExecStatementOfRegTimerProc)

        t.Clear()
        t.Add(ParamType.Integer)
        oParamTypesForVerb(StatementVerb.UnregTimerProc) = t.ToArray()
        oDelegateForVerb(StatementVerb.UnregTimerProc) = New OneStepDelegate(AddressOf ExecStatementOfUnregTimerProc)

        t.Clear()
        oParamTypesForVerb(StatementVerb.FinishScenario) = t.ToArray()
        oDelegateForVerb(StatementVerb.FinishScenario) = New OneStepDelegate(AddressOf ExecStatementOfFinishScenario)

        t.Clear()
        oParamTypesForVerb(StatementVerb.AbortScenario) = t.ToArray()
        oDelegateForVerb(StatementVerb.AbortScenario) = New OneStepDelegate(AddressOf ExecStatementOfAbortScenario)

        t.Clear()
        t.Add(ParamType.String)
        oParamTypesForVerb(StatementVerb.Evaluate) = t.ToArray()
        oDelegateForVerb(StatementVerb.Evaluate) = New OneStepDelegate(AddressOf ExecStatementOfEvaluate)

        t.Clear()
        t.Add(ParamType.String)
        oParamTypesForVerb(StatementVerb.Print) = t.ToArray()
        oDelegateForVerb(StatementVerb.Print) = New OneStepDelegate(AddressOf ExecStatementOfPrint)

        t.Clear()
        t.Add(ParamType.BinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.OptBinFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.CheckBinFile) = t.ToArray()
        oDelegateForVerb(StatementVerb.CheckBinFile) = New OneStepDelegate(AddressOf ExecStatementOfCheckBinFile)

        t.Clear()
        t.Add(ParamType.CsvFilePath)
        t.Add(ParamType.OptCsvFilePath)
        t.Add(ParamType.OptCsvFilePath)
        t.Add(ParamType.Integer)
        t.Add(ParamType.Label)
        oParamTypesForVerb(StatementVerb.CheckCsvFile) = t.ToArray()
        oDelegateForVerb(StatementVerb.CheckCsvFile) = New OneStepDelegate(AddressOf ExecStatementOfCheckCsvFile)
    End Sub

    Private Function EvaluateParam(ByVal oStatement As ProcStatement, ByVal paramIndex As Integer, ByVal oContext As Context) As Object
        'NOTE: ���̃��\�b�h�� oStatement.Params(i)�̒l������������K�v�͂Ȃ��B
        '���m�Ɍ����ƁA���������Ă͂Ȃ�Ȃ��B
        '���̃��\�b�h�́u$�v���܂܂��p�����[�^�ɂ��Ă̂ݎg�p�����B
        '�u$�v���܂܂��p�����[�^�́A�]������Ƃ��̏󋵁i�ϐ��̒l������j�ɂ����
        '�]�����ʂ��ω�����\���������A�O���v���Z�X�ɍ�Ƃ����s�����邱�Ƃ�
        '�ړI�Ƃ��Ă��邽�߁A�o���̓s�x�A�]������K�v������B

        Dim param As StatementParam = oStatement.Params(paramIndex)
        If param.IsExpanded Then Return param.Value
        Try
            Dim s As String = oStringExpander.Expand(DirectCast(param.Value, String), oContext.CallStack.Peek().LocalVariables, oContext.Number)
            Dim t As ParamType = oParamTypesForVerb(oStatement.Verb)(paramIndex)
            If t = ParamType.Label OrElse t = ParamType.OptLabel Then
                If s.Length = 0 Then
                    If t = ParamType.Label Then
                        Throw New FormatException("Blank is not allowed here.")
                    End If
                    Return Nothing
                ElseIf s.Equals("Next", StringComparison.OrdinalIgnoreCase) Then
                    'NOTE: oStatement���炻�̎��s�𒲂ׂ�̂͏����R�X�g���|���邽�߁A
                    'oContext.ExecPos��oStatement���܂�Proc�ɂ�����oStatement�̈ʒu��
                    '���邱�Ƃ�O��ɁA��􂵂��������s���Ă���B
                    Return oContext.ExecPos + 1
                Else
                    'NOTE: ���X���܂�Ӗ����Ȃ����߁A���̃��\�b�h�ŕϊ����s���P�[�X�i�h���L�����܂�ł����P�[�X�j�ł́A
                    '���\�D��ŁA�W�J��̕�����ɂ��Ă�CTypeParamText�ɂ�鎖�O�̕�����̃`�F�b�N�͍s��Ȃ����Ƃɂ���B
                    '�s���ȕ������܂�ł����PosOfLabels����݂���Ȃ����ƂŁA�G���[�ɂȂ�͂��B
                    'NOTE: ���x�����p�����[�^�Ƃ���RegFooProc�n�X�e�[�g�����g�͑��݂��Ȃ����߁A
                    'oContext.ExecProcedure�́A�K��oStatement���܂�Proc�ł���B
                    Dim pos As Integer
                    If oContext.ExecProcedure.PosOfLabels.TryGetValue(s.ToUpperInvariant(), pos) = False Then
                        '�ǂłȂ����������O�o�͂���\���ɂ��Ă͔r������B
                        If Not IsValidToken(s) Then
                            Throw New FormatException("This param contains illegal char for label.")
                        Else
                            Throw New FormatException("Undefined label [" & s & "].")
                        End If
                    End If
                    Return pos
                End If
            ElseIf t = ParamType.ProcName OrElse t = ParamType.OptProcName Then
                If s.Length = 0 Then
                    If t = ParamType.ProcName Then
                        Throw New FormatException("Blank is not allowed here.")
                    End If
                    Return Nothing
                Else
                    'NOTE: ���X���܂�Ӗ����Ȃ����߁A���̃��\�b�h�ŕϊ����s���P�[�X�i�h���L�����܂�ł����P�[�X�j�ł́A
                    '���\�D��ŁA�W�J��̕�����ɂ��Ă�CTypeParamText�ɂ�鎖�O�̕�����̃`�F�b�N�͍s��Ȃ����Ƃɂ���B
                    '�s���ȕ������܂�ł����oProcedures����݂���Ȃ����ƂŁA�G���[�ɂȂ�͂��B
                    Dim oTargetProc As Procedure = Nothing
                    If oProcedures.TryGetValue(s.ToUpperInvariant(), oTargetProc) = False Then
                        '�ǂłȂ����������O�o�͂���\���ɂ��Ă͔r������B
                        If Not IsValidToken(s) Then
                            Throw New FormatException("This param contains illegal char for proc name.")
                        Else
                            Throw New FormatException("Undefined proc name [" & s & "].")
                        End If
                    End If
                    Return oTargetProc
                End If
            Else
                Return CTypeParamText(s, t)
            End If
        Catch ex As Exception
            Throw New OPMGException("L" & oStatement.LineNumber.ToString() & ": Cannot evaluate the param(" & paramIndex.ToString() & ").", ex)
        End Try
    End Function

    Private Function CTypeParamText(ByVal s As String, ByVal t As ParamType) As Object
        Select Case t
            Case ParamType.String
                Return s

            Case ParamType.Byte
                Return Byte.Parse(s, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo)

            Case ParamType.Integer
                Return Integer.Parse(s, NumberFormatInfo.InvariantInfo)

            Case ParamType.Boolean
                Return Boolean.Parse(s)

            Case ParamType.Label, ParamType.OptLabel
                'NOTE: ���̃��\�b�h�ł͕�����`�F�b�N�����s���AString��ԋp���Ă��邪�A
                '���Statements�z��̗v�f�ԍ��iInteger�j�ɍ����ւ���̂ŁA���ӁB
                If s.Length = 0 Then
                    If t = ParamType.Label Then
                        Throw New FormatException("Blank is not allowed here.")
                    End If
                    Return Nothing
                End If
                If Not IsValidToken(s) Then
                    Throw New FormatException("This param contains illegal char for label.")
                End If
                Return s

            Case ParamType.ProcName, ParamType.OptProcName
                'NOTE: ���̃��\�b�h�ł͕�����`�F�b�N�����s���AString��ԋp���Ă��邪�A
                '���Procedure�ւ̎Q�Ƃɍ����ւ���̂ŁA���ӁB
                If s.Length = 0 Then
                    If t = ParamType.ProcName Then
                        Throw New FormatException("Blank is not allowed here.")
                    End If
                    Return Nothing
                End If
                If Not IsValidToken(s) Then
                    Throw New FormatException("This param contains illegal char for proc name.")
                End If
                Return s

            Case ParamType.ProcParams
                'NOTE: ���̃��\�b�h���R���p�C�����ɌĂяo�����ꍇ�́As��"$"���܂܂�Ă���\���͂Ȃ��B
                '���s���Ɂi�W�J��̕�����ɑ΂��āj�Ăяo�����Ƃ�����i"$[$]"�̓W�J���ʂƂ��Ắj"$"���܂܂��\�������邪�A
                '���s���ɂ́i�������̂��߁jProcParams�ɑ΂��Ă��̃��\�b�h�͎g��Ȃ����Ƃɂ��Ă���B
                'NOTE: ���������s�����̂́As��"$"���܂܂�Ă��Ȃ��ꍇ�ł��邩��A�����A�Z�~�R�������܂܂�Ă����Ƃ��āA
                '�W�J�����s�����i�ȗ����Ȃ��j�Ƃ��Ă��A�֐��̈�����؂蕶���Ƃ݂Ȃ���邱�Ƃ͂Ȃ��A
                '���Q�ȃZ�~�R�����ł���B
                Return s.Replace(";", "$[;]").Replace(">", "$[>]")

            Case ParamType.BinFilePath, ParamType.OptBinFilePath, ParamType.OutBinFilePath, ParamType.CsvFilePath, ParamType.OptCsvFilePath, ParamType.OutCsvFilePath
                If s.Equals("*", StringComparison.Ordinal) Then
                    If t = ParamType.BinFilePath OrElse t = ParamType.OutBinFilePath OrElse t = ParamType.CsvFilePath OrElse t = ParamType.OutCsvFilePath Then
                        Throw New FormatException("Asterisk is not allowed here.")
                    End If
                    Return s

                ElseIf s.Length = 0 Then
                    If t = ParamType.BinFilePath OrElse t = ParamType.CsvFilePath Then
                        Throw New FormatException("Blank is not allowed here.")
                    End If
                    Return s

                ElseIf s.StartsWith("Bytes:", StringComparison.OrdinalIgnoreCase) Then
                    If t = ParamType.OutBinFilePath OrElse t = ParamType.CsvFilePath OrElse t = ParamType.OptCsvFilePath OrElse t = ParamType.OutCsvFilePath Then
                        Throw New FormatException("Bytes is not allowed here.")
                    End If

                    'NOTE: ���̃p�����[�^��MyUtility.IsMatchBin�ɓn�����Ȃ�A
                    '�uBytes:�`�v�Ƃ���������̂܂܂ł��������]���ł��邪�A���̃��\�b�h���V�i���I��
                    '���[�h���ɌĂ΂��ꍇ�Ȃǂ́A���̎��_��Byte�z�񉻂��Ă������������I�ł��邵�A
                    '�����̌������[�h���_�Ō��o���邱�Ƃ��ł���ȂǁA���_�������B
                    Dim preLen As Integer = "Bytes:".Length
                    Return MyUtility.GetBytesFromHyphenatedHexadecimalString(s, preLen, s.Length - preLen)

                ElseIf s.StartsWith("Fields:", StringComparison.OrdinalIgnoreCase) Then
                    If t = ParamType.OutCsvFilePath OrElse t = ParamType.BinFilePath OrElse t = ParamType.OptBinFilePath OrElse t = ParamType.OutBinFilePath Then
                        Throw New FormatException("Fields is not allowed here.")
                    End If

                    'NOTE: ���̃p�����[�^��MyUtility.IsMatchCsv�ɓn�����Ȃ�A
                    '�uFields:�`�v�Ƃ���������̂܂܂ł��������]���ł��邪�A���̃��\�b�h���V�i���I��
                    '���[�h���ɌĂ΂��ꍇ�Ȃǂ́A���̎��_��String�z�񉻂��Ă������������I�ł��邵�A
                    '�����̌������[�h���_�Ō��o���邱�Ƃ��ł���ȂǁA���_�������B
                    Dim preLen As Integer = "Fields:".Length
                    Return MyUtility.GetFieldsFromSpaceDelimitedString(s.Substring(preLen))

                Else
                    If t = ParamType.OutBinFilePath OrElse t = ParamType.OutCsvFilePath Then
                        If s.IndexOf("*"c) <> -1 OrElse s.IndexOf("?"c) <> -1 Then
                            Throw New FormatException("Wildcard is not allowed here.")
                        End If
                    End If

                    'NOTE: ���̃p�����[�^��MyUtility.IsMatchBin��MyUtility.IsMatchCsv�Ȃǂɓn�����Ȃ�A
                    '�ꏏ��sScenarioBasePath���n���悤�ɂ��邱�ƂŁA�i��΃p�X�łȂ��ꍇ�́j�A����
                    '�s���邪�A���̃��\�b�h���V�i���I�̃��[�h���ɌĂ΂��ꍇ�Ȃǂ́A���̎��_��
                    '��΃p�X�����Ă������������I�ł��邽�߁A���̂悤�ɂ��Ă���B
                    If Not Path.IsPathRooted(s) Then
                        s = Path.Combine(sScenarioBasePath, s)
                    End If
                    Return s

                End If

            Case ParamType.Ticks
                Return StringExpander.CTypeTicks(s)

            Case ParamType.DateTime
                If s.StartsWith("++", StringComparison.Ordinal) Then
                    s = s.Substring("++".Length).Trim()
                    Dim i As Integer = s.IndexOf(" "c)
                    Dim days As Integer = 0
                    If i <> -1 Then
                        days = Integer.Parse(s.Substring(0, i), NumberFormatInfo.InvariantInfo)
                        'NOTE: i�́uTrim�ς݂́vs����݂����󔒂̈ʒu�Ȃ̂ŁA
                        '�ui + 1�v���L���Ȉʒu�ł���i�󔒂̎��̕����͕K�����݂���j�B
                        s = s.Substring(i + 1).Trim()
                    End If
                    Dim d As DateTime
                    If DateTime.TryParseExact(s, TimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, d) = False Then
                        Throw New FormatException("The param is invalid as proc-relative Time.")
                    End If
                    Return CLng((((days * 24 + d.Hour) * 60 + d.Minute) * 60 + d.Second) * 1000 + d.Millisecond)
                ElseIf s.StartsWith("+", StringComparison.Ordinal) Then
                    s = s.Substring("+".Length).Trim()
                    Dim i As Integer = s.IndexOf(" "c)
                    Dim days As Integer = 0
                    If i <> -1 Then
                        days = Integer.Parse(s.Substring(0, i), NumberFormatInfo.InvariantInfo)
                        'NOTE: i�́uTrim�ς݂́vs����݂����󔒂̈ʒu�Ȃ̂ŁA
                        '�ui + 1�v���L���Ȉʒu�ł���i�󔒂̎��̕����͕K�����݂���j�B
                        s = s.Substring(i + 1).Trim()
                    End If
                    Dim d As DateTime
                    If DateTime.TryParseExact(s, TimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, d) = False Then
                        Throw New FormatException("The param is invalid as proc-relative Time.")
                    End If
                    Return (((days * 24 + d.Hour) * 60 + d.Minute) * 60 + d.Second) * 1000 + d.Millisecond
                ElseIf s.IndexOf("/"c) <> -1 Then
                    Dim d As DateTime
                    If DateTime.TryParseExact(s, DateTimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, d) = False Then
                        Throw New FormatException("The param is invalid as absolute DateTime.")
                    End If
                    Return d
                Else
                    Dim d As DateTime
                    If DateTime.TryParseExact(s, TimeParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, d) = False Then
                        Throw New FormatException("The param is invalid as absolute Time.")
                    End If
                    Return d.ToString("HH:mm:ss.fff")
                End If

            Case ParamType.NakCauseCode, ParamType.OptNakCauseCode
                If s.Length = 0 Then
                    If t = ParamType.NakCauseCode Then
                        Throw New FormatException("Blank is not allowed here.")
                    End If
                    Return Nothing
                End If

                If s.Length < 3 OrElse Not Utility.IsDecimalStringFixed(s, 0, 3) Then
                    Throw New FormatException("The param is invalid as NakCauseCode.")
                End If
                Dim causeNumber As Integer = Utility.GetIntFromDecimalString(s, 0, 3)

                s = s.Substring(3)
                If Not MyUtility.IsAsciiString(s) OrElse s.Length > 47 Then
                    Throw New FormatException("The param may be dangerous as NakCauseCode.")
                End If

                Return New EkNakCauseCode(causeNumber, s)

            Case ParamType.XllFileHashValue
                If Not MyUtility.IsAsciiString(s) OrElse s.Length > 32 Then
                    Throw New FormatException("The param may be dangerous as XllFileHashValue.")
                End If
                Return s

            Case ParamType.DllResultInfo
                Return New DllResultInfo(s)
        End Select
        Return Nothing
    End Function

    Private Shared Sub ResolveLabels(ByVal oProcedure As Procedure)
        For iSt As Integer = 0 To oProcedure.Statements.Count - 1
            Dim oSt As ProcStatement = oProcedure.Statements(iSt)
            Dim paraTypes As ParamType() = oParamTypesForVerb(oSt.Verb)
            For i As Integer = 0 To paraTypes.Length - 1
                If paraTypes(i) = ParamType.Label OrElse paraTypes(i) = ParamType.OptLabel Then
                    If oSt.Params(i).Value Is Nothing Then
                        oSt.Params(i).IsExpanded = True
                    Else
                        Debug.Assert(oSt.Params(i).Value.GetType() Is GetType(String))
                        Dim sLab As String = DirectCast(oSt.Params(i).Value, String)
                        If sLab.IndexOf("$"c) = -1 Then
                            If sLab.Equals("Next", StringComparison.OrdinalIgnoreCase) Then
                                oSt.Params(i).Value = iSt + 1
                                oSt.Params(i).IsExpanded = True
                            Else
                                Dim pos As Integer
                                If oProcedure.PosOfLabels.TryGetValue(sLab.ToUpperInvariant(), pos) = True Then
                                    oSt.Params(i).Value = pos
                                    oSt.Params(i).IsExpanded = True
                                Else
                                    Throw New FormatException("L" & oSt.LineNumber.ToString() & ": Undefined label [" & sLab & "].")
                                End If
                            End If
                        End If
                    End If
                End If
            Next i
        Next iSt
    End Sub

    Private Shared Sub ResolveProcNames(ByVal oProcedures As Dictionary(Of String, Procedure))
        For Each oProcedure As Procedure In oProcedures.Values
            For Each oSt As ProcStatement In oProcedure.Statements
                Dim paraTypes As ParamType() = oParamTypesForVerb(oSt.Verb)
                For i As Integer = 0 To paraTypes.Length - 1
                    If paraTypes(i) = ParamType.ProcName OrElse paraTypes(i) = ParamType.OptProcName Then
                        If oSt.Params(i).Value Is Nothing Then
                            oSt.Params(i).IsExpanded = True
                        Else
                            Debug.Assert(oSt.Params(i).Value.GetType() Is GetType(String))
                            Dim sName As String = DirectCast(oSt.Params(i).Value, String)
                            If sName.IndexOf("$"c) = -1 Then
                                Dim oTargetProc As Procedure = Nothing
                                If oProcedures.TryGetValue(sName.ToUpperInvariant(), oTargetProc) = True Then
                                    oSt.Params(i).Value = oTargetProc
                                    oSt.Params(i).IsExpanded = True
                                Else
                                    Throw New FormatException("L" & oSt.LineNumber.ToString() & ": Undefined proc name [" & sName & "].")
                                End If
                            End If
                        End If
                    End If
                Next i
            Next oSt
        Next oProcedure
    End Sub

    'NOTE: �����̎��s�́AoReadyContexts�ɃL���[�C���O����Ă�����̂��Ȃ��Ȃ�܂ŁA
    '�A���I�ɍs���܂��B��̓I�ɂ́A�܂��AoReadyContexts�̐擪�ɂ��镶���ɂ��āA
    '����A���I�Ɏ��s���܂��B�\���I�V�[�P���X�̊�����󓮓I�V�[�P���X�̌��m��
    '�P���Ȏ��Ԍo�ߓ���҂K�v����������AoReadyContexts����f�L���[���āA
    '���ɃL���[�C���O����Ă��镶���ɂ��ē��l�̏������s���܂��B
    '�ȏ�̏����́AoRootTimer�̃n���h���iProcOnTimeout�̒��j�Ŏ��s���܂��B
    '����A���Ƃ��Δ\���I�P���V�[�P���X�����������ۂ́AProcOnActiveOneComplete��
    '�Ăяo����܂����A���̍ۂ́A�����҂��Ă��������Ȃ������������A
    '����΁A���̕�����oReadyContexts�ɃG���L���[���܂��B�������A���̏�ł�
    '�����̎��s�͎��{�����AoRootTimer���i����0�Łj�X�^�[�g�����āA���̃n���h��
    '�ɂāA���������s�����܂��B����́ATelegrapher�̐݌v�ɍ��킹���݌v�ł��B
    'Telegrapher�̊e���\�b�h�ɂ͌Ăяo�����ƌĂяo����鑤�����߂��Ă���A
    '���Ƃ���ProcOnActiveDllXxxx�̒�����Disconnect���Ăяo���Ă͂Ȃ�Ȃ����Ƃ�
    '�Ȃ��Ă��邽�߂ł��iProcOnActiveDllXxxx�́A�ʐM�V�[�P���X�̐���̌��ʁA
    '�Ăяo�������̂ł�����A���ŒʐM������s�����߂̃��\�b�h�ł͂Ȃ��A
    '�Ɩ��������s�����߂̃��\�b�h�ł���ƌ����܂��j�B
    Private _Status As Integer
    Private oTelegGene As EkTelegramGene
    Private oTelegImporter As EkTelegramImporter
    Private clientIndex As Integer
    Private clientCode As EkCode
    Private oTermCodes As List (Of EkCode)
    Private sPermittedPathInFtp As String
    Private sPermittedPath As String
    Private sWorkingDirPath As String
    Private sExeBasePath As String
    Private sScenarioBasePath As String
    Private oGlobalVariables As Dictionary(Of String, VarHolder)
    Private oAssemblies As Dictionary(Of String, Assembly)
    Private oProcedures As Dictionary(Of String, Procedure)
    Private oContexts As LinkedList(Of Context)
    Private oReadyContexts As LinkedList(Of Context)
    Private oPassiveOneWaitingContexts As LinkedList(Of Context)
    Private oPassiveUllWaitingContexts As LinkedList(Of Context)
    Private oPassiveDllWaitingContexts As LinkedList(Of Context)
    Private oPassiveOneHandlers As SortedDictionary(Of Integer, PassiveOneHandler)
    Private oPassiveUllHandlers As SortedDictionary(Of Integer, PassiveUllHandler)
    Private oPassiveDllHandlers As SortedDictionary(Of Integer, PassiveDllHandler)
    Private oDisconnectHandlers As SortedDictionary(Of Integer, DisconnectHandler)
    Private oTimerHandlers As Dictionary(Of Integer, TimerHandler)
    Private oRootTimer As TickTimer
    Private oContextTable(255) As Context 'OPT: �{���I�ɕs�v�i���O�o�͂ƃR���e�L�X�g�������̂��߂ɂ����ɗp�Ӂj

    Private Connect As ConnectDelegate
    Private Disconnect As DisconnectDelegate
    Private SendReplyTelegram As SendReplyTelegramDelegate
    Private SendNakTelegram As SendNakTelegramDelegate
    Private RegisterActiveOne As RegisterActiveOneDelegate
    Private RegisterActiveUll As RegisterActiveUllDelegate
    Private RegisterTimer As RegisterTimerDelegate
    Private UnregisterTimer As UnregisterTimerDelegate

    Private oStringExpander As StringExpander
    Private oAssemblyManager As DynAssemblyManager

    'NOTE: ���̃v���p�e�B�́A�e�X���b�h�ɂ����ĎQ�Ƃ⏑���݂��s����B
    '�e�X���b�h�́A���̃v���p�e�B��Running�łȂ��ꍇ�ɂ̂ݏ����݂��s���A
    'Running�ɕύX����BTelegrapher�́A���̃v���p�e�B��Running�̏ꍇ��
    '�̂ݏ����݂��s���ARunning�ȊO�ɕύX����B
    Public Property Status() As ScenarioStatus
        'NOTE: MyTelegrapher.LineStatus�̎���NOTE���Q�ƁB
        Get
            Return DirectCast(Interlocked.Add(_Status, 0), ScenarioStatus)
        End Get

        Set(ByVal status As ScenarioStatus)
            Interlocked.Exchange(_Status, status)
        End Set
    End Property

    Public Sub New( _
       ByVal oTelegGene As EkTelegramGene, _
       ByVal oTelegImporter As EkTelegramImporter, _
       ByVal clientIndex As Integer, _
       ByVal clientCode As EkCode, _
       ByVal oTermCodes As List (Of EkCode), _
       ByVal sPermittedPathInFtp As String, _
       ByVal sPermittedPath As String, _
       ByVal oConnect As ConnectDelegate, _
       ByVal oDisconnect As DisconnectDelegate, _
       ByVal oSendReplyTelegram As SendReplyTelegramDelegate, _
       ByVal oSendNakTelegram As SendNakTelegramDelegate, _
       ByVal oRegisterActiveOne As RegisterActiveOneDelegate, _
       ByVal oRegisterActiveUll As RegisterActiveUllDelegate, _
       ByVal oRegisterTimer As RegisterTimerDelegate, _
       ByVal oUnregisterTimer As UnregisterTimerDelegate, _
       ByVal oStringExpander As StringExpander, _
       ByVal oAssemblyManager As DynAssemblyManager)
        Me.oTelegGene = oTelegGene
        Me.oTelegImporter = oTelegImporter
        Me.clientIndex = clientIndex
        Me.clientCode = clientCode
        Me.oTermCodes = oTermCodes
        Me.sPermittedPathInFtp = sPermittedPathInFtp
        Me.sPermittedPath = sPermittedPath
        Me.sWorkingDirPath = Environment.CurrentDirectory
        Me.sExeBasePath = Path.GetDirectoryName([Assembly].GetExecutingAssembly().Location)
        Me.Connect = oConnect
        Me.Disconnect = oDisconnect
        Me.SendReplyTelegram = oSendReplyTelegram
        Me.SendNakTelegram = oSendNakTelegram
        Me.RegisterActiveOne = oRegisterActiveOne
        Me.RegisterActiveUll = oRegisterActiveUll
        Me.RegisterTimer = oRegisterTimer
        Me.UnregisterTimer = oUnregisterTimer
        Me.oStringExpander = oStringExpander
        Me.oAssemblyManager = oAssemblyManager
        Me.oGlobalVariables = Nothing
        Me.oAssemblies = Nothing
        Me.oProcedures = Nothing
        Me.oContexts = New LinkedList(Of Context)
        Me.oReadyContexts = New LinkedList(Of Context)
        Me.oPassiveOneWaitingContexts = New LinkedList(Of Context)
        Me.oPassiveUllWaitingContexts = New LinkedList(Of Context)
        Me.oPassiveDllWaitingContexts = New LinkedList(Of Context)
        Me.oPassiveOneHandlers = New SortedDictionary(Of Integer, PassiveOneHandler)
        Me.oPassiveUllHandlers = New SortedDictionary(Of Integer, PassiveUllHandler)
        Me.oPassiveDllHandlers = New SortedDictionary(Of Integer, PassiveDllHandler)
        Me.oDisconnectHandlers = New SortedDictionary(Of Integer, DisconnectHandler)
        Me.oTimerHandlers = New Dictionary(Of Integer, TimerHandler)
        Me.oRootTimer = New TickTimer(0)
        Me.Status = ScenarioStatus.Initial
        Array.Clear(oContextTable, 0, oContextTable.Length)
    End Sub

    'NOTE: sScenarioFilePath�Ƀt�@�C�����Ȃ��ꍇ�Ȃǂɂ́AIOException���X���[���܂��B
    'NOTE: �����Ɉُ킪����ꍇ�Ȃǂɂ́AIOException�ȊO��Exception���X���[���܂��B
    Public Sub Load(ByVal sScenarioFilePath As String)
        Debug.Assert(Status <> ScenarioStatus.Running AndAlso Status <> ScenarioStatus.Loaded)
        sScenarioBasePath = Path.GetDirectoryName(sScenarioFilePath)
        oStringExpander.CurrentDirectory = sScenarioBasePath

        oGlobalVariables = New Dictionary(Of String, VarHolder)
        oStringExpander.GlobalVariables = oGlobalVariables

        oAssemblies = New Dictionary(Of String, Assembly)
        oStringExpander.Assemblies = oAssemblies

        oProcedures = New Dictionary(Of String, Procedure)

        Using oReader As StreamReader = New StreamReader(sScenarioFilePath, Encoding.Default)
            Dim oCurProcedure As Procedure = Nothing
            Dim isLabelDangling As Boolean = False  '�{���Ȃ��̃��x�������邩

            Dim sCurVbCodeName As String = Nothing
            Dim sCurCsCodeName As String = Nothing
            Dim curCodeLineNumOrigin As Integer
            Dim oCodeBuilder As LinkedList(Of String) = Nothing

            Dim lineNumber As Integer = 1
            Dim sRawLine As String = oReader.ReadLine()
            While sRawLine IsNot Nothing
                Dim sLine As String = sRawLine.Trim()

                If sCurVbCodeName IsNot Nothing
                    If sLine.Equals("EndVbCode", StringComparison.OrdinalIgnoreCase) Then
                        If oAssemblyManager IsNot Nothing Then
                            Dim oAsm As Assembly = _
                              oAssemblyManager.GetAssembly("VisualBasic", oCodeBuilder.ToArray(), sScenarioFilePath, curCodeLineNumOrigin, _
                               sWorkingDirPath, sScenarioBasePath, sExeBasePath)
                            oAssemblies.Add(sCurVbCodeName.ToUpperInvariant(), oAsm)

                            oCodeBuilder = Nothing
                        End If
                        sCurVbCodeName = Nothing
                    Else
                        If oAssemblyManager IsNot Nothing Then
                            oCodeBuilder.AddLast(sRawLine)
                        End If
                    End If

                ElseIf sCurCsCodeName IsNot Nothing Then
                    If sLine.Equals("EndCsCode", StringComparison.OrdinalIgnoreCase) Then
                        If oAssemblyManager IsNot Nothing Then
                            Dim oAsm As Assembly = _
                              oAssemblyManager.GetAssembly("CSharp", oCodeBuilder.ToArray(), sScenarioFilePath, curCodeLineNumOrigin, _
                               sWorkingDirPath, sScenarioBasePath, sExeBasePath)
                            oAssemblies.Add(sCurCsCodeName.ToUpperInvariant(), oAsm)

                            oCodeBuilder = Nothing
                        End If
                        sCurCsCodeName = Nothing
                    Else
                        If oAssemblyManager IsNot Nothing Then
                            oCodeBuilder.AddLast(sRawLine)
                        End If
                    End If

                ElseIf sLine.Length = 0 OrElse sLine.Chars(0) = "'"c Then

                        '��s�܂��̓R�����g�s�Ȃ̂ŉ������Ȃ��B

                ElseIf oVbCodeBeginningRegx.IsMatch(sLine) Then
                    If oCurProcedure IsNot Nothing OrElse sCurVbCodeName IsNot Nothing OrElse sCurCsCodeName IsNot Nothing Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": VbCode definition is not allowed in other definition blocks.")
                    End If

                    Dim sName As String = sLine.Substring("VbCode".Length + 1).Trim()

                    'OPT: oAssemblyManager Is Nothing �̏ꍇ�́AsName�̒����╶����`�F�b�N�s�v�ł���i���O�ɍs���Ă���͂��ł���j�B
                    If sName.Length = 0 Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": VbCode definition requires a name.")
                    End If

                    If Not IsValidToken(sName) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": This name contains illegal char.")
                    End If

                    If oAssemblies.ContainsKey(sName.ToUpperInvariant()) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": The name """ & sName & """ conflicts with other code.")
                    End If

                    sCurVbCodeName = sName
                    If oAssemblyManager IsNot Nothing Then
                        curCodeLineNumOrigin = lineNumber + 1
                        oCodeBuilder = New LinkedList(Of String)()
                    End If

                ElseIf oCsCodeBeginningRegx.IsMatch(sLine) Then
                    If oCurProcedure IsNot Nothing OrElse sCurVbCodeName IsNot Nothing OrElse sCurCsCodeName IsNot Nothing Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": CsCode definition is not allowed in other definition blocks.")
                    End If

                    Dim sName As String = sLine.Substring("CsCode".Length + 1).Trim()

                    'OPT: oAssemblyManager Is Nothing �̏ꍇ�́AsName�̒����╶����`�F�b�N�s�v�ł���i���O�ɍs���Ă���͂��ł���j�B
                    If sName.Length = 0 Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": CsCode definition requires a name.")
                    End If

                    If Not IsValidToken(sName) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": This name contains illegal char.")
                    End If

                    If oAssemblies.ContainsKey(sName.ToUpperInvariant()) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": The name """ & sName & """ conflicts with other code.")
                    End If

                    sCurCsCodeName = sName
                    If oAssemblyManager IsNot Nothing Then
                        curCodeLineNumOrigin = lineNumber + 1
                        oCodeBuilder = New LinkedList(Of String)()
                    End If

                ElseIf oProcBeginningRegx.IsMatch(sLine) Then
                    If oCurProcedure IsNot Nothing OrElse sCurVbCodeName IsNot Nothing OrElse sCurCsCodeName IsNot Nothing Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Proc definition is not allowed in other definition blocks.")
                    End If

                    Dim sNameAndParams As String = sLine.Substring("Proc".Length + 1).Trim()
                    Dim paramPos As Integer = sNameAndParams.IndexOf("("c)
                    Dim sName As String = Nothing
                    Dim sParams As String = Nothing
                    If paramPos = -1 Then
                        sName = sNameAndParams
                    Else
                        sName = sNameAndParams.Substring(0, paramPos).Trim()
                        If sNameAndParams.Chars(sNameAndParams.Length - 1) <> ")"c Then
                            Throw New FormatException("L" & lineNumber.ToString() & ": Proc params must be in (...).")
                        End If
                        sParams = sNameAndParams.Substring(paramPos + 1, sNameAndParams.Length - 1 - (paramPos + 1)).Trim()
                    End If

                    If sName.Length = 0 Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Proc definition requires a name.")
                    End If

                    If Not IsValidToken(sName) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": This name contains illegal char.")
                    End If

                    If oProcedures.ContainsKey(sName.ToUpperInvariant()) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": The name """ & sName & """ conflicts with other proc.")
                    End If

                    If sParams IsNot Nothing AndAlso sParams.Length <> 0 Then
                        Dim oParams As String() = sParams.Split(","c)
                        Dim oUsingNames As New HashSet(Of String)
                        For i As Integer = 0 To oParams.Length - 1
                            Dim s As String = oParams(i).Trim()
                            Dim len As Integer = s.Length
                            If len = 0 Then
                                Throw New FormatException("L" & lineNumber.ToString() & ": Name of param(" & i.ToString() & ") is invalid.")
                            End If

                            Dim s2 As String = s
                            If s.Chars(0) = "*"c Then
                                If len = 1 Then
                                    Throw New FormatException("L" & lineNumber.ToString() & ": Name of param(" & i.ToString() & ") is invalid.")
                                End If
                                s2 = s.Substring(1)
                            End If

                            If Not IsValidToken(s2) Then
                                'NOTE: s�̐擪��"@"�̏ꍇ�͂����ɕ��򂷂�z��ł���B
                                Throw New FormatException("L" & lineNumber.ToString() & ": Name of param(" & i.ToString() & ") contains illegal char.")
                            End If
                            oParams(i) = s

                            If oUsingNames.Add(s) = False Then
                                Throw New FormatException("L" & lineNumber.ToString() & ": Name of param(" & i.ToString() & ") conflicts with other param.")
                            End If
                        Next i
                        oCurProcedure = New Procedure(sName, oParams)
                    Else
                        oCurProcedure = New Procedure(sName, New String(-1) {})
                    End If

                ElseIf sLine.Equals("EndProc", StringComparison.OrdinalIgnoreCase) Then
                    If oCurProcedure Is Nothing Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Beginning of proc definition not found.")
                    End If

                    If isLabelDangling Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Label is dangling.")
                    End If

                    ResolveLabels(oCurProcedure)
                    oProcedures.Add(oCurProcedure.Name.ToUpperInvariant(), oCurProcedure)
                    oCurProcedure = Nothing

                ElseIf sLine.StartsWith("#", StringComparison.Ordinal) Then
                    If oCurProcedure Is Nothing Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Label must be in a proc.")
                    End If

                    Dim sName As String = sLine.Substring("#".Length).Trim()
                    If sName.Equals("Next", StringComparison.OrdinalIgnoreCase) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": You can not define a label ""Next"" because a param ""Next"" means next statement.")
                    End If
                    If sName.Length = 0 Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": You can not define an empty string label.")
                    End If
                    If Not IsValidToken(sName) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": This label contains illegal char.")
                    End If
                    If oCurProcedure.PosOfLabels.ContainsKey(sName.ToUpperInvariant()) Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": The label """ & sName & """ already defined.")
                    End If

                    oCurProcedure.PosOfLabels.Add(sName.ToUpperInvariant(), oCurProcedure.Statements.Count)
                    isLabelDangling = True

                Else
                    If oCurProcedure Is Nothing Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Statement must be in a proc.")
                    End If

                    Dim oSt As New ProcStatement()
                    Dim m As Match

                    m = oSubjBeginningRegx.Match(sLine)
                    If Not m.Success Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Subject not found.")
                    End If
                    Try
                        oSt.Subject = EkCode.Parse(m.Value.Substring(0, m.Length - 1), "%R-%S-%C-%U")
                    Catch ex As Exception
                        Throw New FormatException("L" & lineNumber.ToString() & ": Bad subject """ & m.Value.Substring(0, m.Length - 1) & """.", ex)
                    End Try
                    sLine = sLine.Substring(m.Length).Trim()

                    m = oVerbBeginningRegx.Match(sLine)
                    If Not m.Success Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Verb not found.")
                    End If

                    If oEnumForVerbText.TryGetValue(m.Value.Trim().ToUpperInvariant(), oSt.Verb) = False Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": Unknown verb """ & m.Value.Trim() & """.")
                    End If
                    sLine = sLine.Substring(m.Length).Trim()

                    Dim containsTermCode As Boolean = False
                    Dim paraTypes As ParamType() = oParamTypesForVerb(oSt.Verb)
                    Dim paraTypesLen As Integer = paraTypes.Length
                    Dim isVariadicPara As Boolean = False
                    If paraTypesLen >= 1 AndAlso paraTypes(paraTypesLen - 1) = ParamType.ProcParams Then
                        paraTypesLen -= 1
                        isVariadicPara = True
                    End If

                    If sLine.Length = 0 Then
                        'NOTE: �u�K�{�p�����[�^���P�ł��肩�A������u�����N�ɂ��Ă��悢�v�X�e�[�g�����g�͑z�肵�Ȃ��B
                        If paraTypesLen <> 0 Then
                            If isVariadicPara Then
                                Throw New FormatException("L" & lineNumber.ToString() & ": The verb """ & oSt.Verb.ToString() & """ requires at least " & paraTypesLen & " param(s).")
                            Else
                                Throw New FormatException("L" & lineNumber.ToString() & ": The verb """ & oSt.Verb.ToString() & """ requires " & paraTypesLen & " param(s).")
                            End If
                        End If
                        oSt.Params = New StatementParam(-1) {}
                    Else
                        Dim paramTexts As String() = sLine.Replace("$[,]", vbLf).Split(","c)
                        If isVariadicPara Then
                            If paramTexts.Length < paraTypesLen Then
                                Throw New FormatException("L" & lineNumber.ToString() & ": The verb """ & oSt.Verb.ToString() & """ requires at least " & paraTypesLen & " param(s).")
                            End If
                        Else
                            If paramTexts.Length <> paraTypesLen Then
                                Throw New FormatException("L" & lineNumber.ToString() & ": The verb """ & oSt.Verb.ToString() & """ requires " & paraTypesLen & " param(s).")
                            End If
                        End If
                        oSt.Params = New StatementParam(paramTexts.Length - 1) {}
                        For i As Integer = 0 To paramTexts.Length - 1
                            Try
                                Dim paramText As String = paramTexts(i).Replace(vbLf, "$[,]").Trim()
                                paramText = paramText.Replace("%%", vbLf & vbLf) _
                                                     .Replace("%T", vbCr)
                                paramText = MyUtility.ReplaceMachineIndex(paramText, clientIndex)
                                paramText = clientCode.ToString(paramText).Replace(ControlChars.Lf, "%"c)
                                If Not containsTermCode AndAlso paramText.IndexOf(ControlChars.Cr) <> -1 Then
                                    containsTermCode = True
                                End If
                                oSt.Params(i).Value = paramText
                            Catch ex As Exception
                                Throw New FormatException("L" & lineNumber.ToString() & ": The param(" & i.ToString() & ") contains illegal %.", ex)
                            End Try
                        Next i
                    End If
                    oSt.LineNumber = lineNumber

                    If containsTermCode Then
                        For index As Integer = 0 To oTermCodes.Count - 1
                            Dim code As EkCode = oTermCodes(index)
                            Dim oSt2 As ProcStatement = oSt.Clone()
                            oSt2.Params = DirectCast(oSt.Params.Clone(), StatementParam())
                            For i As Integer = 0 To oSt2.Params.Length - 1
                                Dim t As ParamType = If(i < paraTypesLen, paraTypes(i), ParamType.ProcParams)
                                Try
                                    Dim s As String = DirectCast(oSt2.Params(i).Value, String)
                                    If s.IndexOf(ControlChars.Cr) <> -1 Then
                                        s = s.Replace("%"c, ControlChars.Lf).Replace(ControlChars.Cr, "%"c)
                                        s = MyUtility.ReplaceMachineIndex(s, index)
                                        s = code.ToString(s).Replace(ControlChars.Lf, "%"c)
                                    End If

                                    If s.IndexOf("$"c) <> -1 Then
                                        oSt2.Params(i).Value = s
                                        Debug.Assert(oSt2.Params(i).IsExpanded = False)
                                    Else
                                        oSt2.Params(i).Value = CTypeParamText(s, t)
                                        oSt2.Params(i).IsExpanded = True
                                    End If
                                Catch ex As Exception
                                    Throw New FormatException("L" & lineNumber.ToString() & ": The param(" & i.ToString() & ") is illegal as " & t.ToString() & ".", ex)
                                End Try
                            Next i
                            oCurProcedure.Statements.Add(oSt2)
                            isLabelDangling = False
                        Next index
                    Else
                        For i As Integer = 0 To oSt.Params.Length - 1
                            Dim t As ParamType = If(i < paraTypesLen, paraTypes(i), ParamType.ProcParams)
                            Try
                                Dim s As String = DirectCast(oSt.Params(i).Value, String)
                                If s.IndexOf("$"c) <> -1 Then
                                    oSt.Params(i).Value = s
                                    Debug.Assert(oSt.Params(i).IsExpanded = False)
                                Else
                                    oSt.Params(i).Value = CTypeParamText(s, t)
                                    oSt.Params(i).IsExpanded = True
                                End If
                            Catch ex As Exception
                                Throw New FormatException("L" & lineNumber.ToString() & ": The param(" & i.ToString() & ") is illegal as " & t.ToString() & ".", ex)
                            End Try
                        Next i
                        oCurProcedure.Statements.Add(oSt)
                        isLabelDangling = False
                    End If
                End If

                sRawLine = oReader.ReadLine()
                lineNumber += 1
            End While

            If oCurProcedure IsNot Nothing Then
                Throw New FormatException("L" & lineNumber.ToString() & ": Proc definition not ended.")
            End If

            If sCurVbCodeName IsNot Nothing Then
                Throw New FormatException("L" & lineNumber.ToString() & ": VbCode definition not ended.")
            End If

            If sCurCsCodeName IsNot Nothing Then
                Throw New FormatException("L" & lineNumber.ToString() & ": CsCode definition not ended.")
            End If
        End Using

        ResolveProcNames(oProcedures)

        Dim oEntryProc As Procedure = Nothing
        If oProcedures.TryGetValue(sEntryProcName, oEntryProc) = False Then
            Throw New FormatException("Main proc not found.")
        End If

        If oEntryProc.ParamNames.Length <> 0 Then
            Throw New FormatException("Main must be a proc with no params.")
        End If

        Status = ScenarioStatus.Loaded
    End Sub

    Public Sub StartRunning()
        Debug.Assert(Status <> ScenarioStatus.Initial AndAlso Status <> ScenarioStatus.Running)

        Dim oContext As New Context(0)
        oContext.ExecProcedure = oProcedures(sEntryProcName)
        oContextTable(0) = oContext
        Log.Info("ScenarioContext(0) spawned.")

        oContexts.AddLast(oContext)
        oReadyContexts.AddLast(oContext)
        Status = ScenarioStatus.Running
        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
    End Sub

    Public Sub StopRunning()
        If Status = ScenarioStatus.Running OrElse Status = ScenarioStatus.Loaded Then
            Status = ScenarioStatus.Stopped
        End If
        Terminate()
    End Sub

    Public Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oRootTimer Then
            ExecuteReadyContexts()
            Return True
        End If

        For Each oKeyValue As KeyValuePair(Of Integer, TimerHandler) In oTimerHandlers
            Dim oHandler As TimerHandler = oKeyValue.Value
            If oTimer Is oHandler.Timer Then
                Dim regNumber As Integer = oKeyValue.Key
                Dim oSt As ProcStatement = oHandler.SourceStatement
                Log.Info("ScenarioTimerProc #" & regNumber.ToString() & " signaled.")

                Dim num As Integer = Array.IndexOf(oContextTable, Nothing)
                If num < 0 Then
                    Log.Error("Too many contexts exist.")
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End If
                Dim oContext As New Context(num)
                oContextTable(num) = oContext
                Log.Info("ScenarioContext(" & num.ToString() & ") spawned.")

                Try
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 3, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("Timer handler must be a proc with no params.")
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try

                oContexts.AddLast(oContext)
                oReadyContexts.AddLast(oContext)
                If oHandler.Count <> 1 Then
                    If oHandler.Count <> 0 Then
                        oHandler.Count -= 1
                    End If
                    RegisterTimer(oHandler.Timer, TickTimer.GetSystemTick())
                Else
                    oTimerHandlers.Remove(regNumber)
                End If

                ExecuteReadyContexts()
                Return True
            End If
        Next oKeyValue

        For Each oContext As Context In oContexts
            If oTimer Is oContext.ExecTimer Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                oContext.ExecTimer = Nothing
                Try
                    'NOTE: ���̏󋵂ł́AoContext.ExecSeq�͕K��Nothing�ł���B
                    'oContext.ExecSeq�ɓd�����Z�b�g���鎞�_�i�ҋ@��������
                    'oContext�ɃV�[�P���X��R�Â������_�j��oContext.ExecTimer��
                    '�������邽�߂ł���B
                    Select Case oSt.Verb
                        Case StatementVerb.Wait, StatementVerb.WaitUntil
                            Log.Info("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": The time comes.")
                            oContext.ExecPos += 1
                        Case StatementVerb.WaitForPassiveOne, StatementVerb.WaitForPassiveOneToNak
                            Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": PassiveOne not detected within the time period.")
                            oContext.ExecPos = DirectCast(EvaluateParam(oSt, 6, oContext), Integer)
                            oPassiveOneWaitingContexts.Remove(oContext)
                        Case StatementVerb.WaitForPassiveUll
                            Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": PassiveUll not detected within the time period.")
                            oContext.ExecPos = DirectCast(EvaluateParam(oSt, 11, oContext), Integer)
                            oPassiveUllWaitingContexts.Remove(oContext)
                        Case StatementVerb.WaitForPassiveUllToNak
                            Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": PassiveUll not detected within the time period.")
                            oContext.ExecPos = DirectCast(EvaluateParam(oSt, 5, oContext), Integer)
                            oPassiveUllWaitingContexts.Remove(oContext)
                        Case StatementVerb.WaitForPassiveDll
                            Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": PassiveDll not detected within the time period.")
                            oContext.ExecPos = DirectCast(EvaluateParam(oSt, 9, oContext), Integer)
                            oPassiveDllWaitingContexts.Remove(oContext)
                        Case StatementVerb.WaitForPassiveDllToNak
                            Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": PassiveDll not detected within the time period.")
                            oContext.ExecPos = DirectCast(EvaluateParam(oSt, 5, oContext), Integer)
                            oPassiveDllWaitingContexts.Remove(oContext)
                    End Select
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try
                oReadyContexts.AddLast(oContext)
                ExecuteReadyContexts()
                Return True
            End If
        Next oContext

        Return False
    End Function

    '�\���I�P���V�[�P���X�����������ꍇ
    Public Function ProcOnActiveOneComplete(ByVal oReqTeleg As EkReqTelegram, ByVal oAckTeleg As EkTelegram) As Boolean
        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Info("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": ActiveOne completed.")

                Dim sOriginalFilePath As String = DirectCast(oReqTeleg, EkAnonyReqTelegram).OriginalFilePath
                If sOriginalFilePath IsNot Nothing Then
                    Try
                        Log.Debug("Deleting the file [" & sOriginalFilePath & "]...")
                        File.Delete(sOriginalFilePath)
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        Log.Error("The scenario aborted.")
                        Status = ScenarioStatus.Aborted
                        Terminate()
                        Return True
                    End Try
                End If

                Try
                    Dim sOutFilePath As String = DirectCast(EvaluateParam(oSt, 1, oContext), String)
                    If sOutFilePath.Length <> 0 Then
                        Dim sContextPath As String = Path.Combine(sPermittedPath, "#" & oContext.Number.ToString())
                        Directory.CreateDirectory(sContextPath)
                        Using oOutputStream As New FileStream(sOutFilePath, FileMode.Create, FileAccess.Write)
                            oAckTeleg.WriteToStream(oOutputStream)
                        End Using
                    End If
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try

                If oContext.IterationTargets IsNot Nothing Then
                    oContext.IterationPos += 1
                    If oContext.IterationPos >= oContext.IterationTargets.Length Then
                        oContext.IterationTargets = Nothing
                        oContext.IterationPos = 0
                    End If
                End If

                oContext.ExecSeq = Nothing
                If oContext.IterationTargets Is Nothing Then
                    oContext.ExecPos += 1
                End If
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oContext
        Return False
    End Function

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    Public Function ProcOnActiveOneRetryOverToForget(ByVal oReqTeleg As EkReqTelegram, ByVal oNakTeleg As EkNakTelegram) As Boolean
        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": ActiveOne retry over.")

                Try
                    Dim sOutFilePath As String = DirectCast(EvaluateParam(oSt, 1, oContext), String)
                    If sOutFilePath.Length <> 0 Then
                        Dim sContextPath As String = Path.Combine(sPermittedPath, "#" & oContext.Number.ToString())
                        Directory.CreateDirectory(sContextPath)
                        Using oOutputStream As New FileStream(sOutFilePath, FileMode.Create, FileAccess.Write)
                            oNakTeleg.WriteToStream(oOutputStream)
                        End Using
                    End If

                    oContext.IterationTargets = Nothing
                    oContext.IterationPos = 0
                    oContext.ExecSeq = Nothing
                    oContext.ExecPos = DirectCast(EvaluateParam(oSt, 3, oContext), Integer)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oContext
        Return False
    End Function

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Public Function ProcOnActiveOneRetryOverToCare(ByVal oReqTeleg As EkReqTelegram, ByVal oNakTeleg As EkNakTelegram) As Boolean
        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": ActiveOne retry over.")

                Try
                    Dim sOutFilePath As String = DirectCast(EvaluateParam(oSt, 1, oContext), String)
                    If sOutFilePath.Length <> 0 Then
                        Dim sContextPath As String = Path.Combine(sPermittedPath, "#" & oContext.Number.ToString())
                        Directory.CreateDirectory(sContextPath)
                        Using oOutputStream As New FileStream(sOutFilePath, FileMode.Create, FileAccess.Write)
                            oNakTeleg.WriteToStream(oOutputStream)
                        End Using
                    End If

                    oContext.IterationTargets = Nothing
                    oContext.IterationPos = 0
                    oContext.ExecSeq = Nothing
                    oContext.ExecPos = DirectCast(EvaluateParam(oSt, 4, oContext), Integer)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oContext
        Return False
    End Function

    '�\���I�P���V�[�P���X�̍Œ���L���[�C���O���ꂽ�\���I�P���V�[�P���X�̎��{�O�ɒʐM�ُ�����o�����ꍇ
    Public Function ProcOnActiveOneAnonyError(ByVal oReqTeleg As EkReqTelegram) As Boolean
        'NOTE: ���̃��\�b�h�́AExecStatementOfDisconnect()����
        '�Ăяo����邱�Ƃ����蓾��͂��B
        '�������A���̃R���e�L�X�g��Disconnect�X�e�[�g�����g��
        '���s���ł���䂦�A�ȉ��� oReqTeleg Is oContext.ExecSeq
        '�ƂȂ�oContext�Ƃ͕ʂ̃R���e�L�X�g�ł���B
        'NOTE: ���̃��\�b�h�́AProcOnPassiveOneReqTelegramReceive()
        '�ɂ�����Disconnect()����Ăяo����邱�Ƃ����蓾��悤��
        '�݂��邩������Ȃ����A���͂Ȃ��B
        '�܂��A����PassiveOne�ƕR�Â��Ă���R���e�L�X�g��
        '���s�J�n���Ă��Ȃ��i�V�K�́j�R���e�L�X�g�ł��邩
        'WaitForPassiveOne�n�X�e�[�g�����g�����s���̃R���e�L�X�g
        '�ł���䂦�A�ȉ��� oReqTeleg Is oContext.ExecSeq�ƂȂ�
        'oContext�Ƃ͕ʂ̃R���e�L�X�g�ł���B

        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": ActiveOne failed.")

                'Log.Error("The scenario aborted.")
                'Status = ScenarioStatus.Aborted
                'Terminate()
                'Return True

                Try
                    Dim sOutFilePath As String = DirectCast(EvaluateParam(oSt, 1, oContext), String)
                    If sOutFilePath.Length <> 0 Then
                        Dim sContextPath As String = Path.Combine(sPermittedPath, "#" & oContext.Number.ToString())
                        Directory.CreateDirectory(sContextPath)
                        Using oOutputStream As New FileStream(sOutFilePath, FileMode.Create, FileAccess.Write)
                            oReqTeleg.WriteToStream(oOutputStream)
                        End Using
                    End If

                    oContext.IterationTargets = Nothing
                    oContext.IterationPos = 0
                    oContext.ExecSeq = Nothing
                    oContext.ExecPos = DirectCast(EvaluateParam(oSt, 5, oContext), Integer)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oContext

        Return False
    End Function

    '�\���IULL�̓]���J�nREQ�d���ɑ����]���I��REQ�d���𐶐����郁�\�b�h
    Public Function CreateActiveUllContinuousReqTelegram(ByVal oReqTeleg As EkClientDrivenUllReqTelegram, ByVal cc As ContinueCode) As EkClientDrivenUllReqTelegram
        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                'Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Dim oNewReqTeleg As EkClientDrivenUllReqTelegram _
                 = oReqTeleg.CreateContinuousTelegram(cc, 0, oReqTeleg.AltReplyLimitTicks, 0, oReqTeleg.OriginalFilePath)
                oContext.ExecSeq = oNewReqTeleg
                Return oNewReqTeleg
            End If
        Next oContext

        Return Nothing
    End Function

    Public Function ProcOnActiveUllComplete(ByVal oReqTeleg As EkClientDrivenUllReqTelegram) As Boolean
        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Info("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": ActiveUll completed.")
                If oReqTeleg.OriginalFilePath IsNot Nothing Then
                    Try
                        Log.Debug("Deleting the file [" & oReqTeleg.OriginalFilePath & "]...")
                        File.Delete(oReqTeleg.OriginalFilePath)
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        Log.Error("The scenario aborted.")
                        Status = ScenarioStatus.Aborted
                        Terminate()
                        Return True
                    End Try
                End If

                If oContext.IterationTargets IsNot Nothing Then
                    oContext.IterationPos += 1
                    If oContext.IterationPos >= oContext.IterationTargets.Length Then
                        oContext.IterationTargets = Nothing
                        oContext.IterationPos = 0
                    End If
                End If

                oContext.ExecSeq = Nothing
                If oContext.IterationTargets Is Nothing Then
                    oContext.ExecPos += 1
                End If
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oContext
        Return False
    End Function

    Public Function ProcOnActiveUllRetryOverToForget(ByVal oReqTeleg As EkClientDrivenUllReqTelegram) As Boolean
        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": ActiveUll retry over.")
                oContext.IterationTargets = Nothing
                oContext.IterationPos = 0
                oContext.ExecSeq = Nothing
                Try
                    oContext.ExecPos = DirectCast(EvaluateParam(oSt, 7, oContext), Integer)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oContext

        Return False
    End Function

    Public Function ProcOnActiveUllRetryOverToCare(ByVal oReqTeleg As EkClientDrivenUllReqTelegram) As Boolean
        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": ActiveUll retry over.")
                oContext.IterationTargets = Nothing
                oContext.IterationPos = 0
                oContext.ExecSeq = Nothing
                Try
                    oContext.ExecPos = DirectCast(EvaluateParam(oSt, 8, oContext), Integer)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oContext

        Return False
    End Function

    Public Function ProcOnActiveUllAnonyError(ByVal oReqTeleg As EkClientDrivenUllReqTelegram) As Boolean
        'NOTE: ���̃��\�b�h�́AExecStatementOfDisconnect()����
        '�Ăяo����邱�Ƃ����蓾��͂��B
        '�������A���̃R���e�L�X�g��Disconnect�X�e�[�g�����g��
        '���s���ł���䂦�A�ȉ��� oReqTeleg Is oContext.ExecSeq
        '�ƂȂ�oContext�Ƃ͕ʂ̃R���e�L�X�g�ł���B
        'NOTE: ���̃��\�b�h�́AProcOnPassiveOneReqTelegramReceive()
        '�ɂ�����Disconnect()����Ăяo����邱�Ƃ����蓾��悤��
        '�݂��邩������Ȃ����A���͂Ȃ��B
        '�܂��A����PassiveOne�ƕR�Â��Ă���R���e�L�X�g��
        '���s�J�n���Ă��Ȃ��i�V�K�́j�R���e�L�X�g�ł��邩
        'WaitForPassiveOne�n�X�e�[�g�����g�����s���̃R���e�L�X�g
        '�ł���䂦�A�ȉ��� oReqTeleg Is oContext.ExecSeq�ƂȂ�
        'oContext�Ƃ͕ʂ̃R���e�L�X�g�ł���B

        For Each oContext As Context In oContexts
            If oReqTeleg Is oContext.ExecSeq Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Error("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": ActiveUll failed.")

                'Log.Error("The scenario aborted.")
                'Status = ScenarioStatus.Aborted
                'Terminate()
                'Return True

                oContext.IterationTargets = Nothing
                oContext.IterationPos = 0
                oContext.ExecSeq = Nothing
                Try
                    oContext.ExecPos = DirectCast(EvaluateParam(oSt, 9, oContext), Integer)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    Return True
                End Try
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oContext

        Return False
    End Function

    Public Function ProcOnPassiveOneReqTelegramReceive(ByVal oRcvTeleg As EkTelegram) As Boolean
        Dim oRcvTelegBytes As Byte() = oRcvTeleg.GetBytes()

        Dim oContext As Context = Nothing
        Dim oSt As ProcStatement = Nothing
        Dim iOutFilePathParam As Integer = -1
        Dim iReplyTelegParam As Integer
        Dim isNakReply As Boolean

        Try
            Dim oNode As LinkedListNode(Of Context) = oPassiveOneWaitingContexts.First
            While oNode IsNot Nothing
                oContext = oNode.Value
                oSt = oContext.ExecProcedure.Statements(oContext.ExecPos)
                If MyUtility.IsMatchBin(oRcvTelegBytes, oContext.TelegCompObj, oContext.TelegMaskObj, oContext.TelegEvaluationLen) Then
                    Log.Info("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": PassiveOne detected.")
                    isNakReply = If(oSt.Verb = StatementVerb.WaitForPassiveOneToNak, True, False)
                    iOutFilePathParam = 3
                    iReplyTelegParam = 4
                    UnregisterTimer(oContext.ExecTimer)
                    oContext.ExecTimer = Nothing
                    oContext.TelegCompObj = Nothing
                    oContext.TelegMaskObj = Nothing
                    oContext.TelegEvaluationLen = 0
                    oContext.ExecPos += 1
                    oPassiveOneWaitingContexts.Remove(oNode)
                    oReadyContexts.AddLast(oNode)
                    Exit While
                End If
                oNode = oNode.Next
            End While

            If iOutFilePathParam = -1 Then
                For Each oKeyValue As KeyValuePair(Of Integer, PassiveOneHandler) In oPassiveOneHandlers
                    Dim oHandler As PassiveOneHandler = oKeyValue.Value
                    If MyUtility.IsMatchBin(oRcvTelegBytes, oHandler.TelegCompObj, oHandler.TelegMaskObj, oHandler.TelegEvaluationLen) Then
                        oSt = oHandler.SourceStatement
                        Dim regNumber As Integer = oKeyValue.Key
                        Log.Info("ScenarioPassiveOneProc #" & regNumber.ToString() & " signaled.")
                        isNakReply = If(oSt.Verb = StatementVerb.RegPassiveOneProcToNak, True, False)
                        iOutFilePathParam = 4
                        iReplyTelegParam = 5

                        Dim num As Integer = Array.IndexOf(oContextTable, Nothing)
                        If num < 0 Then
                            Log.Error("Too many contexts exist.")
                            Log.Error("The scenario aborted.")
                            Status = ScenarioStatus.Aborted
                            Terminate()
                            'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
                            '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
                            Return False
                        End If
                        oContext = New Context(num)
                        oContextTable(num) = oContext
                        Log.Info("ScenarioContext(" & num.ToString() & ") spawned.")

                        oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 6, oContext), Procedure)
                        If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveOne handler must be a proc with no params.")

                        oContexts.AddLast(oContext)
                        oReadyContexts.AddLast(oContext)
                        Exit For
                    End If
                Next oKeyValue
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
            '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
            Return False
        End Try

        If iOutFilePathParam = -1 Then Return False

        Dim oReplyTelegParam As Object
        Try
            Dim sOutFilePath As String = DirectCast(EvaluateParam(oSt, iOutFilePathParam, oContext), String)
            If sOutFilePath.Length <> 0 Then
                Dim sContextPath As String = Path.Combine(sPermittedPath, "#" & oContext.Number.ToString())
                Directory.CreateDirectory(sContextPath)
                Using oOutputStream As New FileStream(sOutFilePath, FileMode.Create, FileAccess.Write)
                    oRcvTeleg.WriteToStream(oOutputStream)
                End Using
            End If

            oReplyTelegParam = EvaluateParam(oSt, iReplyTelegParam, oContext)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
            '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
            Return False
        End Try

        If isNakReply Then
            Dim nakCause As NakCauseCode = DirectCast(oReplyTelegParam, EkNakCauseCode)
            Debug.Assert(nakCause <> EkNakCauseCode.None)
            If SendNakTelegram(nakCause, oRcvTeleg) = False Then
                'Log.Error("The scenario aborted.")
                'Status = ScenarioStatus.Aborted
                'Terminate()
                'Disconnect()
                'Return True

                Disconnect()
            End If
        Else
            Dim nakCause As NakCauseCode = EkNakCauseCode.None
            Dim oReplyTeleg As EkDodgyTelegram = Nothing
            If oReplyTelegParam.GetType() Is GetType(String) Then
                Dim sReplyTelegPath As String = DirectCast(oReplyTelegParam, String)
                If sReplyTelegPath.Length = 0 Then
                    nakCause = EkNakCauseCode.NoData
                ElseIf Not File.Exists(sReplyTelegPath) Then
                    Log.Warn("The file [" & sReplyTelegPath & "] not found.")
                    nakCause = EkNakCauseCode.NoData
                Else
                    Dim retryCount As Integer = 0
                    While True
                        Log.Debug("Loading telegram from [" & sReplyTelegPath & "]...")
                        Try
                            Using oInputStream As New FileStream(sReplyTelegPath, FileMode.Open, FileAccess.Read)
                                oReplyTeleg = oTelegImporter.GetTelegramFromStream(oInputStream)
                            End Using
                            Exit While
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                            If ex.GetType() Is GetType(IOException) Then
                                'NOTE: �ʂ̃v���Z�X���r���I�Ɂi�ǂݎ��֎~�ŁjsReplyTelegPath�̃t�@�C����
                                '�J���ł���ꍇ�Ƃ݂Ȃ��B
                                If retryCount >= 3 Then
                                    nakCause = EkNakCauseCode.Busy
                                    Exit While
                                End If
                                Thread.Sleep(1000)
                                retryCount += 1
                            Else
                                'ex��DirectoryNotFoundException��FileNotFoundException�̏ꍇ�ł���B
                                'NOTE: ���File.Exists����New FileStream�܂ł̊Ԃ�
                                '�t�@�C�����ړ���폜���ꂽ�P�[�X�Ƃ݂Ȃ��B
                                'TODO: �V�i���I�ُ�I���̕����悢��������Ȃ��B
                                nakCause = EkNakCauseCode.NoData
                                Exit While
                            End If
                        End Try
                    End While
                End If
            Else
                oReplyTeleg = oTelegImporter.GetTelegramFromBytes(DirectCast(oReplyTelegParam, Byte()))
            End If

            If oReplyTeleg Is Nothing Then
                'NOTE: oTelegImporter�ɓn�����o�C�g�񂪓d���Ƃ��Ă̍Œ���̏�����
                '�������Ă��Ȃ������ꍇ�ł���B���̃P�[�X�ł́AoTelegImporter��
                '���\�b�h�̒��ŃG���[���O���o�͍ς݂ł���B
                Log.Error("The scenario aborted.")
                Status = ScenarioStatus.Aborted
                Terminate()
                'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
                '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
                Return False
            End If

            If nakCause <> EkNakCauseCode.None Then
                If SendNakTelegram(nakCause, oRcvTeleg) = False Then
                    'Log.Error("The scenario aborted.")
                    'Status = ScenarioStatus.Aborted
                    'Terminate()
                    'Disconnect()
                    'Return True

                    Disconnect()
                End If
            Else
                Log.Info("Sending Reply telegram...")
                If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
                    'Log.Error("The scenario aborted.")
                    'Status = ScenarioStatus.Aborted
                    'Terminate()
                    'Disconnect()
                    'Return True

                    Disconnect()
                End If
            End If
        End If

        'OPT: ���̃��\�b�h�̒��łȂ�A�ȉ��̂�����
        'ExecuteReadyContexts()�ł����e�ł���B
        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
        Return True
    End Function

    '�󓮓IDLL�̏����i�\�����ꂽ�t�@�C���̎󂯓���m�F�j���s�����\�b�h
    Public Function PrepareToStartPassiveDll(ByVal oXllReqTeleg As EkMasProDllReqTelegram) As NakCauseCode
        'NOTE: WaitForPassiveDllToNak�̍s���������邱�ƂɂȂ����ꍇ�́A
        '�R���e�L�X�g��ExecPos��i�߂āAoRootTimer���J�n�����Ȃ���΂Ȃ�Ȃ��B
        '�܂��ARegPassiveDllProcToNak�œo�^�����n���h�����d����ߑ������ꍇ�́A
        '�V�����R���e�L�X�g������āAoRootTimer���J�n�����Ȃ���΂Ȃ�Ȃ��B
        '�����́ANAK�d���̃\�P�b�g�ւ̏����݂������������
        'ProcOnReqTelegramReceiveCompleteBySendNak()�ōs��
        '�iNAK�d���̃\�P�b�g�ւ̏����݂����s�����P�[�X�ł́A
        '�V�i���I�ɕR�Â��Ă���V�[�P���X�ŒʐM�ُ킪��������
        '�P�[�X�̈��ƍl����j�Ƃ����d�l�ł��悢��������Ȃ����A
        '���̏�ōs�����Ƃɂ���B���R�͈ȉ��̂Ƃ���ł���B
        '�ENAK��ԐM����ׂ��d�����͂����Ƃ������ƑS�ʂ�m�肽��
        '  �V�i���I������͂��B
        '�ENAK��ԐM����ׂ��d������M������ɁA�����V�[�P���X����
        '  �ʐM�ُ킪���������P�[�X�́A�e�X�g�������ŕ�����悤��
        '  ��������������Ȃ����ADisconnectProc��o�^���Ă�����
        '  ������͂��ł���B
        '�E�t�ɁANAK�d���̃\�P�b�g�ւ̏����݂�������������Ƃ����āA
        '  �^�ǃT�[�o�܂œ͂����Ƃ��ۏ؂����킯�ł͂Ȃ��̂ŁA
        '  ���̃P�[�X�ł̂݌㑱�̏�����o�^����Proc�̏��������s�����
        '  �Ƃ��Ă��A���ɂ��肪���݂��Ȃ��B
        '�E���̏��oRootTimer���X�^�[�g�����Ă��A����ɂ��㑱�s��
        '  ���s�́A���̃��\�b�h�̎w�肵��NAK���R�R�[�h��NAK�d����
        '  �\�P�b�g�ɏ������񂾌�ɂȂ��Ă����B

        Dim oRcvTelegBytes As Byte() = oXllReqTeleg.GetBytes()

        Dim oContext As Context = Nothing
        Dim oSt As ProcStatement = Nothing
        Dim oHandler As PassiveDllHandler = Nothing

        Try
            Dim oNode As LinkedListNode(Of Context) = oPassiveDllWaitingContexts.First
            While oNode IsNot Nothing
                oContext = oNode.Value
                If oContext.ExecSeq Is Nothing AndAlso _
                   MyUtility.IsMatchBin(oRcvTelegBytes, oContext.TelegCompObj, oContext.TelegMaskObj, oContext.TelegEvaluationLen) Then
                    oSt = oContext.ExecProcedure.Statements(oContext.ExecPos)
                    Log.Info("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": PassiveDll detected.")
                    UnregisterTimer(oContext.ExecTimer)
                    oContext.ExecTimer = Nothing
                    oContext.TelegCompObj = Nothing
                    oContext.TelegMaskObj = Nothing
                    oContext.TelegEvaluationLen = 0
                    Exit While
                End If
                oNode = oNode.Next
            End While

            If oNode Is Nothing Then
                For Each oKeyValue As KeyValuePair(Of Integer, PassiveDllHandler) In oPassiveDllHandlers
                    oHandler = oKeyValue.Value
                    If MyUtility.IsMatchBin(oRcvTelegBytes, oHandler.TelegCompObj, oHandler.TelegMaskObj, oHandler.TelegEvaluationLen) Then
                        oSt = oHandler.SourceStatement
                        Dim regNumber As Integer = oKeyValue.Key
                        Log.Info("ScenarioPassiveDllProc #" & regNumber.ToString() & " signaled.")

                        Dim num As Integer = Array.IndexOf(oContextTable, Nothing)
                        If num < 0 Then
                            Log.Error("Too many contexts exist.")
                            Log.Error("The scenario aborted.")
                            Status = ScenarioStatus.Aborted
                            Terminate()
                            'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
                            '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
                            Return Nothing
                        End If
                        oContext = New Context(num)
                        oContextTable(num) = oContext
                        Log.Info("ScenarioContext(" & num.ToString() & ") spawned.")

                        'NOTE: oContextTable��num���L���Ă���oContext�ɂ��āA���̎��_�ł�
                        'oContexts�ɓo�^���邱�ƂɂȂ�Ƃ͌���Ȃ����A������͕K���o�^����
                        '�i�������̓V�i���I��Terminate��oContextTable�̏��������s���j�B
                        '���̃��\�b�h���œo�^���V�i���I��Terminate�����Ȃ��Ȃ�A���̃��\�b�h����
                        'oHandler.SpawnedContext�ɃZ�b�g���Ă����A���̐�ŌĂяo�����
                        '�����ꂩ��PassiveDll�p���\�b�h�ŁA�o�^���s�����V�i���I��Terminate���s���B
                        Exit For
                    End If
                Next oKeyValue
            End If

            '�V�i���I�Ɋ֌W�̂Ȃ��d���̏ꍇ�́A���ꂪ������l��ԋp����i�Ăь����������s���j�B
            'NOTE: �ԈႢ�ɂ݂��邩������Ȃ����AEkNakCauseCode��Enum�ł͂Ȃ�Class�ł���A
            '����͐������iEkNakCauseCode.None�Ƃ͈Ⴄ�l�ł���j�B
            If oSt Is Nothing Then Return Nothing

            'OPT: sTelegFilePath�̃t�@�C���쐬�͕K�v�ŏ����̃P�[�X�ɂ����Ă̂ݍs�������B
            '�V�i���I�����K�v�ɉ�����OutBinFilePath�Ƃ��ċL�q����i�s�v�Ȃ�u�����N�Ƃ���j�ȂǁB
            Dim needsExpand As Boolean = False
            If oNode IsNot Nothing Then
                'NOTE: oSt.Verb��WaitForPassiveDll�̏ꍇ��WaitForPassiveDllToNak�̏ꍇ��
                '�V�[�P���X��ߑ����Ĉȍ~�ɓW�J����\���̂���p�����[�^��
                '�v�f3�ȍ~�̃p�����[�^�ł���B
                For iParam As Integer = 3 To oSt.Params.Length - 1
                    If Not oSt.Params(iParam).IsExpanded Then
                        needsExpand = True
                        Exit For
                    End If
                Next iParam
            Else
                'NOTE: oSt.Verb��RegPassiveDllProc�̏ꍇ��RegPassiveDllProcToNak�̏ꍇ��
                '�V�[�P���X��ߑ����Ĉȍ~�ɓW�J����\���̂���p�����[�^��
                '�v�f4�ȍ~�̃p�����[�^�ł���B
                For iParam As Integer = 4 To oSt.Params.Length - 1
                    If Not oSt.Params(iParam).IsExpanded Then
                        needsExpand = True
                        Exit For
                    End If
                Next iParam
            End If

            If needsExpand Then
                Dim sTelegFilePath As String = Path.Combine(sPermittedPath, "#PassiveDllReq.dat")
                Using oOutputStream As New FileStream(sTelegFilePath, FileMode.Create, FileAccess.Write)
                    oXllReqTeleg.WriteToStream(oOutputStream)
                End Using
            End If

            If oNode IsNot Nothing Then
                If oSt.Verb = StatementVerb.WaitForPassiveDll Then
                    Dim oNakCauseParam As Object = EvaluateParam(oSt, 3, oContext)
                    If oNakCauseParam Is Nothing Then
                        'NOTE: oXllReqTeleg�̎�M�ɂ��WaitForPassiveDll�̍s����������ꍇ�ł���B
                        '�����̎��_�Ŋ�������킯�ł͂Ȃ����A���̍s��oXllReqTeleg�̃V�[�P���X���R�Â��B
                        oContext.ExecSeq = oXllReqTeleg

                        'NOTE: ���O�Ƀ`�F�b�N���Ă��邽�߁AiXllReqTeleg.DataFileName���̓p�X�Ƃ��Ė��Q�ł���B
                        Log.Info("Starting PassiveDll of the files [" & Path.GetFileName(oXllReqTeleg.DataFileName) & "] [" & Path.GetFileName(oXllReqTeleg.ListFileName) & "]...")
                        Return NakCauseCode.None
                    Else
                        oContext.ExecPos = DirectCast(EvaluateParam(oSt, 7, oContext), Integer)
                        oPassiveDllWaitingContexts.Remove(oNode)
                        oReadyContexts.AddLast(oNode)
                        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                        'NOTE: �ȉ��ŕԋp����l��EkNakCauseCode.None�Ƃ������Ƃ͂��蓾�Ȃ��B
                        Return DirectCast(oNakCauseParam, EkNakCauseCode)
                    End If
                Else 'oSt.Verb = StatementVerb.WaitForPassiveDllToNak
                    oContext.ExecPos += 1
                    oPassiveDllWaitingContexts.Remove(oNode)
                    oReadyContexts.AddLast(oNode)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                    'NOTE: �ȉ��ŕԋp����l��EkNakCauseCode.None�Ƃ������Ƃ͂��蓾�Ȃ��B
                    Return DirectCast(EvaluateParam(oSt, 3, oContext), EkNakCauseCode)
                End If
            Else
                If oSt.Verb = StatementVerb.RegPassiveDllProc Then
                    Dim oNakCauseParam As Object = EvaluateParam(oSt, 4, oContext)
                    If oNakCauseParam Is Nothing Then
                        'NOTE: oXllReqTeleg�̎�M�ɂ��RegPassiveDllProc�œo�^���Ă����������J�n����ꍇ�ł���B
                        '�����̎��_�ŏ������J�n����킯�ł͂Ȃ����A���̂��߂̃R���e�L�X�g�͍쐬����B
                        'NOTE: ���̎��_�ł͂܂��AoContext.ExecProcedure�͌��܂�Ȃ��B
                        oHandler.BindSeq = oXllReqTeleg
                        oHandler.SpawnedContext = oContext

                        'NOTE: ���O�Ƀ`�F�b�N���Ă��邽�߁AiXllReqTeleg.DataFileName���̓p�X�Ƃ��Ė��Q�ł���B
                        Log.Info("Starting PassiveDll of the files [" & Path.GetFileName(oXllReqTeleg.DataFileName) & "] [" & Path.GetFileName(oXllReqTeleg.ListFileName) & "]...")
                        Return NakCauseCode.None
                    Else
                        oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 7, oContext), Procedure)
                        If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveDll handler must be a proc with no params.")

                        oContexts.AddLast(oContext)
                        oReadyContexts.AddLast(oContext)
                        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                        'NOTE: �ȉ��ŕԋp����l��EkNakCauseCode.None�Ƃ������Ƃ͂��蓾�Ȃ��B
                        Return DirectCast(oNakCauseParam, EkNakCauseCode)
                    End If
                Else 'oSt.Verb = StatementVerb.RegPassiveDllProcToNak
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 5, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveDll handler must be a proc with no params.")

                    oContexts.AddLast(oContext)
                    oReadyContexts.AddLast(oContext)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                    'NOTE: �ȉ��ŕԋp����l��EkNakCauseCode.None�Ƃ������Ƃ͂��蓾�Ȃ��B
                    Return DirectCast(EvaluateParam(oSt, 4, oContext), EkNakCauseCode)
                End If
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
            '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
            Return Nothing
        End Try
    End Function

    '�󓮓IDLL�̓]���J�nREQ�d���ɑ����]���I��REQ�d���𐶐����郁�\�b�h
    Public Function CreatePassiveDllContinuousReqTelegram(ByVal oXllReqTeleg As EkMasProDllReqTelegram, ByVal cc As ContinueCode) As EkMasProDllReqTelegram
        Dim oContext As Context = Nothing
        Dim oSt As ProcStatement = Nothing
        Dim oHandler As PassiveDllHandler = Nothing
        Dim iReqTelegParam As Integer
        Dim iReplyLimitParam As Integer

        Dim oNode As LinkedListNode(Of Context) = oPassiveDllWaitingContexts.First
        While oNode IsNot Nothing
            oContext = oNode.Value
            If oContext.ExecSeq Is oXllReqTeleg Then
                oSt = oContext.ExecProcedure.Statements(oContext.ExecPos)

                'NOTE: oSt.Verb = StatementVerb.WaitForPassiveDllToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                iReqTelegParam = 4
                iReplyLimitParam = 5
                Exit While
            End If
            oNode = oNode.Next
        End While

        If oNode Is Nothing Then
            For Each oKeyValue As KeyValuePair(Of Integer, PassiveDllHandler) In oPassiveDllHandlers
                oHandler = oKeyValue.Value
                If oHandler.BindSeq Is oXllReqTeleg Then
                    oContext = oHandler.SpawnedContext
                    oSt = oHandler.SourceStatement

                    'NOTE: oSt.Verb = StatementVerb.RegPassiveDllProcToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                    iReqTelegParam = 5
                    iReplyLimitParam = 6
                    Exit For
                End If
            Next oKeyValue
        End If

        If oSt Is Nothing Then Return Nothing

        Try
            Dim oRet As EkMasProDllReqTelegram
            If cc = ContinueCode.Finish Then
                Dim oDllResult As DllResultInfo = DirectCast(EvaluateParam(oSt, iReqTelegParam, oContext), DllResultInfo)
                Dim replyLimit As Integer = DirectCast(EvaluateParam(oSt, iReplyLimitParam, oContext), Integer)

                'TODO: ���̃��\�b�h�Ɋւ��āA��������Q�Ԗڂ̈����itransferLimitTicks�j�́A
                '��`���珜�����Ă��܂������B�ǂ̋@��̂ǂ̃v���Z�X�ł���΂ɕs�v�����A
                '�Ď��Օێ��o�[�W�����Ȃǂ̈����������������_�ŁA���̓d���̓������\�b�h
                '�Ƃ̈�ѐ����ێ�����Ӗ��Ȃǂ������Ȃ��Ă���B
                oRet = oXllReqTeleg.CreateContinuousTelegram( _
                        oDllResult.ContinueCode, _
                        oDllResult.ResultantVersionOfSlot1, _
                        oDllResult.ResultantVersionOfSlot2, _
                        oDllResult.ResultantFlagOfFull, _
                        0, _
                        replyLimit)
            Else
                Dim replyLimit As Integer = DirectCast(EvaluateParam(oSt, iReplyLimitParam, oContext), Integer)

                'TODO: ���̃��\�b�h�Ɋւ��āA��������Q�Ԗڂ̈����itransferLimitTicks�j�́A
                '��`���珜�����Ă��܂������B�ǂ̋@��̂ǂ̃v���Z�X�ł���΂ɕs�v�����A
                '�Ď��Օێ��o�[�W�����Ȃǂ̈����������������_�ŁA���̓d���̓������\�b�h
                '�Ƃ̈�ѐ����ێ�����Ӗ��Ȃǂ������Ȃ��Ă���B
                oRet = oXllReqTeleg.CreateContinuousTelegram( _
                        cc, _
                        0, _
                        0, _
                        0, _
                        0, _
                        replyLimit)
            End If

            If oNode IsNot Nothing Then
                oContext.ExecSeq = oRet
            Else
                oHandler.BindSeq = oRet
            End If
            Return oRet
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
            '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
            Return Nothing
        End Try
    End Function

    Public Function ProcOnPassiveDllComplete(ByVal oXllReqTeleg As EkMasProDllReqTelegram) As Boolean
        Dim oNode As LinkedListNode(Of Context) = oPassiveDllWaitingContexts.First
        While oNode IsNot Nothing
            Dim oContext As Context = oNode.Value
            If oContext.ExecSeq Is oXllReqTeleg Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Info("PassiveDll completed.")
                'NOTE: oSt.Verb = StatementVerb.WaitForPassiveDllToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                'NOTE: oContext.ExecTimer�̉����́A�V�[�P���X��ߑ��������_�Ŏ��{�ς݂ł���B
                oContext.ExecSeq = Nothing
                oContext.ExecPos += 1
                oPassiveDllWaitingContexts.Remove(oNode)
                oReadyContexts.AddLast(oNode)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
            oNode = oNode.Next
        End While

        For Each oKeyValue As KeyValuePair(Of Integer, PassiveDllHandler) In oPassiveDllHandlers
            Dim oHandler As PassiveDllHandler = oKeyValue.Value
            If oHandler.BindSeq Is oXllReqTeleg Then
                Log.Info("PassiveDll completed.")
                'NOTE: oHandler.SourceStatement.Verb = StatementVerb.RegPassiveDllProcToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                '����āAoHandler.SpawnedContext�ɂ́A�K��Context�̎Q�Ƃ��Z�b�g����Ă���B
                Dim oContext As Context = oHandler.SpawnedContext
                Try
                    Dim oSt As ProcStatement = oHandler.SourceStatement
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 9, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveDll handler must be a proc with no params.")
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
                    '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
                    Return False
                End Try

                oHandler.BindSeq = Nothing
                oHandler.SpawnedContext = Nothing

                oContexts.AddLast(oContext)
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oKeyValue

        Return False
    End Function

    Public Function ProcOnPassiveDllAnonyError(ByVal oXllReqTeleg As EkMasProDllReqTelegram) As Boolean
        'NOTE: ���̃��\�b�h�́AExecStatementOfDisconnect()����
        '�Ăяo����邱�Ƃ����蓾��͂��B
        '�������A���̃R���e�L�X�g��Disconnect�X�e�[�g�����g��
        '���s���ł���䂦�A�ȉ��� oReqTeleg Is oContext.ExecSeq
        '�ƂȂ�oContext�Ƃ͕ʂ̃R���e�L�X�g�ł���B
        'NOTE: ���̃��\�b�h�́AProcOnPassiveOneReqTelegramReceive()
        '�ɂ�����Disconnect()����Ăяo����邱�Ƃ����蓾��悤��
        '�݂��邩������Ȃ����A���͂Ȃ��B
        '�܂��A����PassiveOne�ƕR�Â��Ă���R���e�L�X�g��
        '���s�J�n���Ă��Ȃ��i�V�K�́j�R���e�L�X�g�ł��邩
        'WaitForPassiveOne�n�X�e�[�g�����g�����s���̃R���e�L�X�g
        '�ł���䂦�A�ȉ��� oReqTeleg Is oContext.ExecSeq�ƂȂ�
        'oContext�Ƃ͕ʂ̃R���e�L�X�g�ł���B

        Try
            Dim oNode As LinkedListNode(Of Context) = oPassiveDllWaitingContexts.First
            While oNode IsNot Nothing
                Dim oContext As Context = oNode.Value
                If oContext.ExecSeq Is oXllReqTeleg Then
                    Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                    Log.Error("PassiveDll failed.")

                    'Log.Error("The scenario aborted.")
                    'Status = ScenarioStatus.Aborted
                    'Terminate()

                    oContext.ExecSeq = Nothing
                    oContext.ExecPos = DirectCast(EvaluateParam(oSt, 8, oContext), Integer)
                    oPassiveDllWaitingContexts.Remove(oNode)
                    oReadyContexts.AddLast(oNode)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                    Return True
                End If
                oNode = oNode.Next
            End While

            For Each oKeyValue As KeyValuePair(Of Integer, PassiveDllHandler) In oPassiveDllHandlers
                Dim oHandler As PassiveDllHandler = oKeyValue.Value
                If oHandler.BindSeq Is oXllReqTeleg Then
                    Log.Error("PassiveDll failed.")

                    'Log.Error("The scenario aborted.")
                    'Status = ScenarioStatus.Aborted
                    'Terminate()

                    'NOTE: oHandler.BindSeq Is oXllReqTeleg �Ƃ����󋵂ł��邽�߁A
                    'oHandler.SpawnedContext�ɂ́A�K��Context�̎Q�Ƃ��Z�b�g����Ă���B
                    Dim oContext As Context = oHandler.SpawnedContext
                    Dim oSt As ProcStatement = oHandler.SourceStatement
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 8, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveDll handler must be a proc with no params.")

                    oHandler.BindSeq = Nothing
                    oHandler.SpawnedContext = Nothing

                    oContexts.AddLast(oContext)
                    oReadyContexts.AddLast(oContext)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                    Return True
                End If
            Next oKeyValue
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: �V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
            Return False
        End Try

        Return False
    End Function

    '�󓮓IULL�̏����i�w�肳�ꂽ�t�@�C���̗p�Ӂj���s�����\�b�h
    Public Function PrepareToStartPassiveUll(ByVal oXllReqTeleg As EkServerDrivenUllReqTelegram) As NakCauseCode
        'NOTE: WaitForPassiveUllToNak�̍s���������邱�ƂɂȂ����ꍇ�́A
        '�R���e�L�X�g��ExecPos��i�߂āAoRootTimer���J�n�����Ȃ���΂Ȃ�Ȃ��B
        '�܂��ARegPassiveUllProcToNak�œo�^�����n���h�����d����ߑ������ꍇ�́A
        '�V�����R���e�L�X�g������āAoRootTimer���J�n�����Ȃ���΂Ȃ�Ȃ��B
        '�����́ANAK�d���̃\�P�b�g�ւ̏����݂������������
        'ProcOnReqTelegramReceiveCompleteBySendNak()�ōs��
        '�iNAK�d���̃\�P�b�g�ւ̏����݂����s�����P�[�X�ł́A
        '�V�i���I�ɕR�Â��Ă���V�[�P���X�ŒʐM�ُ킪��������
        '�P�[�X�̈��ƍl����j�Ƃ����d�l�ł��悢��������Ȃ����A
        '���̏�ōs�����Ƃɂ���B���R�͈ȉ��̂Ƃ���ł���B
        '�ENAK��ԐM����ׂ��d�����͂����Ƃ������ƑS�ʂ�m�肽��
        '  �V�i���I������͂��B
        '�ENAK��ԐM����ׂ��d������M������ɁA�����V�[�P���X����
        '  �ʐM�ُ킪���������P�[�X�́A�e�X�g�������ŕ�����悤��
        '  ��������������Ȃ����ADisconnectProc��o�^���Ă�����
        '  ������͂��ł���B
        '�E�t�ɁANAK�d���̃\�P�b�g�ւ̏����݂�������������Ƃ����āA
        '  �^�ǃT�[�o�܂œ͂����Ƃ��ۏ؂����킯�ł͂Ȃ��̂ŁA
        '  ���̃P�[�X�ł̂݌㑱�̏�����o�^����Proc�̏��������s�����
        '  �Ƃ��Ă��A���ɂ��肪���݂��Ȃ��B
        '�E���̏��oRootTimer���X�^�[�g�����Ă��A����ɂ��㑱�s��
        '  ���s�́A���̃��\�b�h�̎w�肵��NAK���R�R�[�h��NAK�d����
        '  �\�P�b�g�ɏ������񂾌�ɂȂ��Ă����B

        Dim oRcvTelegBytes As Byte() = oXllReqTeleg.GetBytes()

        Dim oContext As Context = Nothing
        Dim oSt As ProcStatement = Nothing
        Dim oHandler As PassiveUllHandler = Nothing

        Try
            Dim oNode As LinkedListNode(Of Context) = oPassiveUllWaitingContexts.First
            While oNode IsNot Nothing
                oContext = oNode.Value
                If oContext.ExecSeq Is Nothing AndAlso _
                   MyUtility.IsMatchBin(oRcvTelegBytes, oContext.TelegCompObj, oContext.TelegMaskObj, oContext.TelegEvaluationLen) Then
                    oSt = oContext.ExecProcedure.Statements(oContext.ExecPos)
                    Log.Info("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": PassiveUll detected.")
                    UnregisterTimer(oContext.ExecTimer)
                    oContext.ExecTimer = Nothing
                    oContext.TelegCompObj = Nothing
                    oContext.TelegMaskObj = Nothing
                    oContext.TelegEvaluationLen = 0
                    Exit While
                End If
                oNode = oNode.Next
            End While

            If oNode Is Nothing Then
                For Each oKeyValue As KeyValuePair(Of Integer, PassiveUllHandler) In oPassiveUllHandlers
                    oHandler = oKeyValue.Value
                    If MyUtility.IsMatchBin(oRcvTelegBytes, oHandler.TelegCompObj, oHandler.TelegMaskObj, oHandler.TelegEvaluationLen) Then
                        oSt = oHandler.SourceStatement
                        Dim regNumber As Integer = oKeyValue.Key
                        Log.Info("ScenarioPassiveUllProc #" & regNumber.ToString() & " signaled.")

                        Dim num As Integer = Array.IndexOf(oContextTable, Nothing)
                        If num < 0 Then
                            Log.Error("Too many contexts exist.")
                            Log.Error("The scenario aborted.")
                            Status = ScenarioStatus.Aborted
                            Terminate()
                            'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
                            '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
                            Return Nothing
                        End If
                        oContext = New Context(num)
                        oContextTable(num) = oContext
                        Log.Info("ScenarioContext(" & num.ToString() & ") spawned.")

                        'NOTE: oContextTable��num���L���Ă���oContext�ɂ��āA���̎��_�ł�
                        'oContexts�ɓo�^���邱�ƂɂȂ�Ƃ͌���Ȃ����A������͕K���o�^����
                        '�i�������̓V�i���I��Terminate��oContextTable�̏��������s���j�B
                        '���̃��\�b�h���œo�^���V�i���I��Terminate�����Ȃ��Ȃ�A���̃��\�b�h����
                        'oHandler.SpawnedContext�ɃZ�b�g���Ă����A���̐�ŌĂяo�����
                        '�����ꂩ��PassiveUll�p���\�b�h�ŁA�o�^���s�����V�i���I��Terminate���s���B
                        Exit For
                    End If
                Next oKeyValue
            End If

            '�V�i���I�Ɋ֌W�̂Ȃ��d���̏ꍇ�́A���ꂪ������l��ԋp����i�Ăь����������s���j�B
            'NOTE: �ԈႢ�ɂ݂��邩������Ȃ����AEkNakCauseCode��Enum�ł͂Ȃ�Class�ł���A
            '����͐������iEkNakCauseCode.None�Ƃ͈Ⴄ�l�ł���j�B
            If oSt Is Nothing Then Return Nothing

            'OPT: sTelegFilePath�̃t�@�C���쐬�͕K�v�ŏ����̃P�[�X�ɂ����Ă̂ݍs�������B
            '�V�i���I�����K�v�ɉ�����OutBinFilePath�Ƃ��ċL�q����i�s�v�Ȃ�u�����N�Ƃ���j�ȂǁB
            Dim needsExpand As Boolean = False
            If oNode IsNot Nothing Then
                'NOTE: oSt.Verb��WaitForPassiveUll�̏ꍇ��WaitForPassiveUllToNak�̏ꍇ��
                '�V�[�P���X��ߑ����Ĉȍ~�ɓW�J����\���̂���p�����[�^��
                '�v�f3�ȍ~�̃p�����[�^�ł���B
                For iParam As Integer = 3 To oSt.Params.Length - 1
                    If Not oSt.Params(iParam).IsExpanded Then
                        needsExpand = True
                        Exit For
                    End If
                Next iParam
            Else
                'NOTE: oSt.Verb��RegPassiveUllProc�̏ꍇ��RegPassiveUllProcToNak�̏ꍇ��
                '�V�[�P���X��ߑ����Ĉȍ~�ɓW�J����\���̂���p�����[�^��
                '�v�f4�ȍ~�̃p�����[�^�ł���B
                For iParam As Integer = 4 To oSt.Params.Length - 1
                    If Not oSt.Params(iParam).IsExpanded Then
                        needsExpand = True
                        Exit For
                    End If
                Next iParam
            End If

            If needsExpand Then
                Dim sTelegFilePath As String = Path.Combine(sPermittedPath, "#PassiveUllReq.dat")
                Using oOutputStream As New FileStream(sTelegFilePath, FileMode.Create, FileAccess.Write)
                    oXllReqTeleg.WriteToStream(oOutputStream)
                End Using
            End If

            Dim oFilePathParam As Object = Nothing
            Dim oFileHashParam As Object = Nothing
            Dim oTransLimitParam As Object = Nothing
            If oNode IsNot Nothing Then
                If oSt.Verb = StatementVerb.WaitForPassiveUll Then
                    Dim oNakCauseParam As Object = EvaluateParam(oSt, 3, oContext)
                    If oNakCauseParam Is Nothing Then
                        oFilePathParam = EvaluateParam(oSt, 4, oContext)
                        oFileHashParam = EvaluateParam(oSt, 5, oContext)
                        oTransLimitParam = EvaluateParam(oSt, 6, oContext)
                    Else
                        oContext.ExecPos = DirectCast(EvaluateParam(oSt, 9, oContext), Integer)
                        oPassiveUllWaitingContexts.Remove(oNode)
                        oReadyContexts.AddLast(oNode)
                        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                        'NOTE: �ȉ��ŕԋp����l��EkNakCauseCode.None�Ƃ������Ƃ͂��蓾�Ȃ��B
                        Return DirectCast(oNakCauseParam, EkNakCauseCode)
                    End If
                Else 'oSt.Verb = StatementVerb.WaitForPassiveUllToNak
                    oContext.ExecPos += 1
                    oPassiveUllWaitingContexts.Remove(oNode)
                    oReadyContexts.AddLast(oNode)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                    'NOTE: �ȉ��ŕԋp����l��EkNakCauseCode.None�Ƃ������Ƃ͂��蓾�Ȃ��B
                    Return DirectCast(EvaluateParam(oSt, 3, oContext), EkNakCauseCode)
                End If
            Else
                If oSt.Verb = StatementVerb.RegPassiveUllProc Then
                    Dim oNakCauseParam As Object = EvaluateParam(oSt, 4, oContext)
                    If oNakCauseParam Is Nothing Then
                        oFilePathParam = EvaluateParam(oSt, 5, oContext)
                        oFileHashParam = EvaluateParam(oSt, 6, oContext)
                        oTransLimitParam = EvaluateParam(oSt, 7, oContext)
                    Else
                        oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 9, oContext), Procedure)
                        If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveUll handler must be a proc with no params.")

                        oContexts.AddLast(oContext)
                        oReadyContexts.AddLast(oContext)
                        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                        'NOTE: �ȉ��ŕԋp����l��EkNakCauseCode.None�Ƃ������Ƃ͂��蓾�Ȃ��B
                        Return DirectCast(oNakCauseParam, EkNakCauseCode)
                    End If
                Else 'oSt.Verb = StatementVerb.RegPassiveUllProcToNak
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 5, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveUll handler must be a proc with no params.")

                    oContexts.AddLast(oContext)
                    oReadyContexts.AddLast(oContext)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                    'NOTE: �ȉ��ŕԋp����l��EkNakCauseCode.None�Ƃ������Ƃ͂��蓾�Ȃ��B
                    Return DirectCast(EvaluateParam(oSt, 4, oContext), EkNakCauseCode)
                End If
            End If

            'NOTE: ���O�Ƀ`�F�b�N���Ă��邽�߁AoXllReqTeleg.FileName�̓p�X�Ƃ��Ė��Q�ł���B
            Dim sTransferFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

            Dim sTransferFilePath As String = Path.Combine(sPermittedPath, sTransferFileName)
            If oFilePathParam.GetType() Is GetType(String) Then
                Dim sSrcFilePath As String = DirectCast(oFilePathParam, String)
                Dim nakCause As NakCauseCode = EkNakCauseCode.None
                If sSrcFilePath.Length = 0 Then
                    nakCause = EkNakCauseCode.NoData
                ElseIf Not File.Exists(sSrcFilePath) Then
                    'NOTE: �V�i���I�܂��͎������̌��i�V�i���I�ɋL�q���ꂽ�t�@�C�������݂��Ȃ����j��
                    '�C�Â����₷������ɂ́AoSt.Params(iFilePathParam).Value��"$Ext"���܂܂�Ȃ��P�[�X�ł́A
                    '������AbortScenario�����������悢���A����̂܂܂ł�Log.Warn�ɂ���ċC�Â��\�����������A
                    '����̂܂܂̕����w���@�킲�Ƃ�ACK�ԐM��NAK(NO DATA)�ԐM��؂�ւ��邱�Ƃ��ȒP�ł��邵�A
                    '�����͂��Ȃ��ł����B
                    Log.Warn("The file [" & sSrcFilePath & "] not found.")
                    nakCause = EkNakCauseCode.NoData
                Else
                    Dim retryCount As Integer = 0
                    While True
                        Try
                            Log.Debug("Copying file from [" & sSrcFilePath & "] to [" & sTransferFilePath & "]...")
                            MyUtility.CopyFileIfNeeded(sSrcFilePath, sTransferFilePath, True)
                            Exit While
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                            If ex.GetType() Is GetType(IOException) Then
                                'NOTE: �ʂ̃v���Z�X���r���I�Ɂi�ǂݎ��֎~�ŁjsSrcFilePath�̃t�@�C����
                                '�J���ł���ꍇ�Ƃ݂Ȃ��B
                                If retryCount >= 3 Then
                                    nakCause = EkNakCauseCode.Busy
                                    Exit While
                                End If
                                Thread.Sleep(1000)
                                retryCount += 1
                            Else
                                'ex��DirectoryNotFoundException��FileNotFoundException�̏ꍇ�ł���B
                                'NOTE: ���File.Exists����CopyFileIfNeeded�܂ł̊Ԃ�
                                '�t�@�C�����ړ���폜���ꂽ�P�[�X�Ƃ݂Ȃ��B
                                nakCause = EkNakCauseCode.NoData
                                Exit While
                            End If
                        End Try
                    End While
                End If

                If nakCause <> EkNakCauseCode.None Then
                    If oNode IsNot Nothing Then
                        'NOTE: NAK�̕ԐM�ƂƂ���WaitForPassiveUll�̍s������������ꍇ�ł���B
                        '�����̎��_��oXllReqTeleg�̃V�[�P���X�ƂƂ��Ɋ�������B
                        oContext.ExecPos = DirectCast(EvaluateParam(oSt, 9, oContext), Integer)
                        oPassiveUllWaitingContexts.Remove(oNode)
                        oReadyContexts.AddLast(oNode)
                    Else
                        oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 9, oContext), Procedure)
                        If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveUll handler must be a proc with no params.")

                        'NOTE: NAK�̕ԐM�ƂƂ���RegPassiveUllProc�œo�^���Ă����������J�n������ꍇ�ł���B
                        '�����̎��_�ŐV�K�̃R���e�L�X�g�ɂď������J�n����B
                        oContexts.AddLast(oContext)
                        oReadyContexts.AddLast(oContext)
                    End If
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                    Return nakCause
                End If
            Else
                Dim oFileContent As Byte() = DirectCast(oFilePathParam, Byte())
                Using oOutputStream As New FileStream(sTransferFilePath, FileMode.Create, FileAccess.Write)
                    oOutputStream.Write(oFileContent, 0, oFileContent.Length)
                End Using
            End If

            If oNode IsNot Nothing Then
                'NOTE: oXllReqTeleg�̎�M�ɂ��WaitForPassiveUll�̍s����������ꍇ�ł���B
                '�����̎��_�Ŋ�������킯�ł͂Ȃ����A���̍s��oXllReqTeleg�̃V�[�P���X���R�Â��B
                oContext.ExecSeq = oXllReqTeleg
            Else
                'NOTE: oXllReqTeleg�̎�M�ɂ��RegPassiveUllProc�œo�^���Ă����������J�n����ꍇ�ł���B
                '�����̎��_�ŏ������J�n����킯�ł͂Ȃ����A���̂��߂̃R���e�L�X�g�͍쐬����B
                'NOTE: ���̎��_�ł͂܂��AoContext.ExecProcedure�͌��܂�Ȃ��B
                oHandler.BindSeq = oXllReqTeleg
                oHandler.SpawnedContext = oContext
            End If

            oXllReqTeleg.FileHashValue = DirectCast(oFileHashParam, String)
            oXllReqTeleg.TransferLimitTicks = DirectCast(oTransLimitParam, Integer)

            'NOTE: ���̌�A�Ăь������̔���𕢂���NAK��ԐM���邱�Ƃ͂Ȃ��B
            Log.Info("Starting PassiveUll of the file [" & sTransferFileName & "]...")
            Return EkNakCauseCode.None
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
            '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
            Return Nothing
        End Try
    End Function

    '�󓮓IULL�̓]���J�nREQ�d���ɑ����]���I��REQ�d���𐶐����郁�\�b�h
    Public Function CreatePassiveUllContinuousReqTelegram(ByVal oXllReqTeleg As EkServerDrivenUllReqTelegram, ByVal cc As ContinueCode) As EkServerDrivenUllReqTelegram
        Dim oNode As LinkedListNode(Of Context) = oPassiveUllWaitingContexts.First
        While oNode IsNot Nothing
            Dim oContext As Context = oNode.Value
            If oContext.ExecSeq Is oXllReqTeleg Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)

                Dim oTransLimitParam As Object
                Dim oReplyLimitParam As Object
                Try
                    'NOTE: oSt.Verb = StatementVerb.WaitForPassiveUllToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                    oTransLimitParam = EvaluateParam(oSt, 6, oContext)
                    oReplyLimitParam = EvaluateParam(oSt, 7, oContext)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
                    '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
                    Return Nothing
                End Try

                Dim oNewReqTeleg As EkServerDrivenUllReqTelegram _
                 = oXllReqTeleg.CreateContinuousTelegram(cc, DirectCast(oTransLimitParam, Integer), DirectCast(oReplyLimitParam, Integer))
                oContext.ExecSeq = oNewReqTeleg
                Return oNewReqTeleg
            End If
            oNode = oNode.Next
        End While

        For Each oKeyValue As KeyValuePair(Of Integer, PassiveUllHandler) In oPassiveUllHandlers
            Dim oHandler As PassiveUllHandler = oKeyValue.Value
            If oHandler.BindSeq Is oXllReqTeleg Then
                Dim oSt As ProcStatement = oHandler.SourceStatement

                'NOTE: oSt.Verb = StatementVerb.RegPassiveUllProcToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                '����āAoHandler.SpawnedContext�ɂ́A�K��Context�̎Q�Ƃ��Z�b�g����Ă���B
                Dim oContext As Context = oHandler.SpawnedContext

                Dim oTransLimitParam As Object
                Dim oReplyLimitParam As Object
                Try
                    'NOTE: oSt.Verb = StatementVerb.RegPassiveUllProcToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                    oTransLimitParam = EvaluateParam(oSt, 7, oContext)
                    oReplyLimitParam = EvaluateParam(oSt, 8, oContext)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
                    '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
                    Return Nothing
                End Try

                Dim oNewReqTeleg As EkServerDrivenUllReqTelegram _
                 = oXllReqTeleg.CreateContinuousTelegram(cc, DirectCast(oTransLimitParam, Integer), DirectCast(oReplyLimitParam, Integer))
                oHandler.BindSeq = oNewReqTeleg
                Return oNewReqTeleg
            End If
        Next oKeyValue

        Return Nothing
    End Function

    Public Function ProcOnPassiveUllComplete(ByVal oXllReqTeleg As EkServerDrivenUllReqTelegram) As Boolean
        Dim oNode As LinkedListNode(Of Context) = oPassiveUllWaitingContexts.First
        While oNode IsNot Nothing
            Dim oContext As Context = oNode.Value
            If oContext.ExecSeq Is oXllReqTeleg Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                Log.Info("PassiveUll completed.")
                'NOTE: oSt.Verb = StatementVerb.WaitForPassiveUllToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                'NOTE: oContext.ExecTimer�̉����́A�V�[�P���X��ߑ��������_�Ŏ��{�ς݂ł���B
                oContext.ExecSeq = Nothing
                oContext.ExecPos += 1
                oPassiveUllWaitingContexts.Remove(oNode)
                oReadyContexts.AddLast(oNode)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
            oNode = oNode.Next
        End While

        For Each oKeyValue As KeyValuePair(Of Integer, PassiveUllHandler) In oPassiveUllHandlers
            Dim oHandler As PassiveUllHandler = oKeyValue.Value
            If oHandler.BindSeq Is oXllReqTeleg Then
                Log.Info("PassiveUll completed.")
                'NOTE: oHandler.SourceStatement.Verb = StatementVerb.RegPassiveUllProcToNak �Ƃ������Ƃ͂��蓾�Ȃ��B
                '����āAoHandler.SpawnedContext�ɂ́A�K��Context�̎Q�Ƃ��Z�b�g����Ă���B
                Dim oContext As Context = oHandler.SpawnedContext
                Try
                    Dim oSt As ProcStatement = oHandler.SourceStatement
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 11, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveUll handler must be a proc with no params.")
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    'NOTE: ���ɕK�v���͂Ȃ����ADisconnect�͂����ɁA
                    '�V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
                    Return False
                End Try

                oHandler.BindSeq = Nothing
                oHandler.SpawnedContext = Nothing

                oContexts.AddLast(oContext)
                oReadyContexts.AddLast(oContext)
                RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                Return True
            End If
        Next oKeyValue

        Return False
    End Function

    Public Function ProcOnPassiveUllAnonyError(ByVal oXllReqTeleg As EkServerDrivenUllReqTelegram) As Boolean
        'NOTE: ���̃��\�b�h�́AExecStatementOfDisconnect()����
        '�Ăяo����邱�Ƃ����蓾��͂��B
        '�������A���̃R���e�L�X�g��Disconnect�X�e�[�g�����g��
        '���s���ł���䂦�A�ȉ��� oReqTeleg Is oContext.ExecSeq
        '�ƂȂ�oContext�Ƃ͕ʂ̃R���e�L�X�g�ł���B
        'NOTE: ���̃��\�b�h�́AProcOnPassiveOneReqTelegramReceive()
        '�ɂ�����Disconnect()����Ăяo����邱�Ƃ����蓾��悤��
        '�݂��邩������Ȃ����A���͂Ȃ��B
        '�܂��A����PassiveOne�ƕR�Â��Ă���R���e�L�X�g��
        '���s�J�n���Ă��Ȃ��i�V�K�́j�R���e�L�X�g�ł��邩
        'WaitForPassiveOne�n�X�e�[�g�����g�����s���̃R���e�L�X�g
        '�ł���䂦�A�ȉ��� oReqTeleg Is oContext.ExecSeq�ƂȂ�
        'oContext�Ƃ͕ʂ̃R���e�L�X�g�ł���B

        Try
            Dim oNode As LinkedListNode(Of Context) = oPassiveUllWaitingContexts.First
            While oNode IsNot Nothing
                Dim oContext As Context = oNode.Value
                If oContext.ExecSeq Is oXllReqTeleg Then
                    Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
                    Log.Error("PassiveUll failed.")

                    'Log.Error("The scenario aborted.")
                    'Status = ScenarioStatus.Aborted
                    'Terminate()

                    oContext.ExecSeq = Nothing
                    oContext.ExecPos = DirectCast(EvaluateParam(oSt, 10, oContext), Integer)
                    oPassiveUllWaitingContexts.Remove(oNode)
                    oReadyContexts.AddLast(oNode)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                    Return True
                End If
                oNode = oNode.Next
            End While

            For Each oKeyValue As KeyValuePair(Of Integer, PassiveUllHandler) In oPassiveUllHandlers
                Dim oHandler As PassiveUllHandler = oKeyValue.Value
                If oHandler.BindSeq Is oXllReqTeleg Then
                    Log.Error("PassiveUll failed.")

                    'Log.Error("The scenario aborted.")
                    'Status = ScenarioStatus.Aborted
                    'Terminate()

                    'NOTE: oHandler.BindSeq Is oXllReqTeleg �Ƃ����󋵂ł��邽�߁A
                    'oHandler.SpawnedContext�ɂ́A�K��Context�̎Q�Ƃ��Z�b�g����Ă���B
                    Dim oContext As Context = oHandler.SpawnedContext
                    Dim oSt As ProcStatement = oHandler.SourceStatement
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 10, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveUll handler must be a proc with no params.")

                    oHandler.BindSeq = Nothing
                    oHandler.SpawnedContext = Nothing

                    oContexts.AddLast(oContext)
                    oReadyContexts.AddLast(oContext)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
                    Return True
                End If
            Next oKeyValue
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: �V�i���I�ŕߑ����Ȃ������̂œ�����p��������B
            Return False
        End Try

        Return False
    End Function

    Public Sub ProcOnConnectionDisappear()
        'NOTE: ���̃��\�b�h�́AExecStatementOfDisconnect()����
        '�Ăяo����邱�Ƃ����蓾��͂��B
        '�������A���̃R���e�L�X�g��Disconnect�X�e�[�g�����g��
        '���s���ł���䂦�A�ȉ��ŏ����̑ΏۂƂ���R���e�L�X�g�Ƃ�
        '�ʂ̃R���e�L�X�g�ł���B
        'NOTE: ���̃��\�b�h�́AProcOnPassiveOneReqTelegramReceive()
        '�ɂ�����Disconnect()����Ăяo����邱�Ƃ����蓾��悤��
        '�݂��邩������Ȃ����A���͂Ȃ��B
        '�܂��A����PassiveOne�ƕR�Â��Ă���R���e�L�X�g��
        '���s�J�n���Ă��Ȃ��i�V�K�́j�R���e�L�X�g�ł��邩
        'WaitForPassiveOne�n�X�e�[�g�����g�����s���̃R���e�L�X�g
        '�ł���䂦�A�ȉ��ŏ����̑ΏۂƂ���R���e�L�X�g�Ƃ�
        '�ʂ̃R���e�L�X�g�ł���B�Ȃ��ADisconnect()�̑O��
        'Terminate()�����s���Ă���Ȃ�A���̃��\�b�h��
        '�Ă΂�鎞�_�ł́AoDisconnectHandlers�͋�ł���B

        Dim needTerminate As Boolean = False
        Dim needExecute As Boolean = False

        For Each oKeyValue As KeyValuePair(Of Integer, DisconnectHandler) In oDisconnectHandlers
            Dim oHandler As DisconnectHandler = oKeyValue.Value
            Dim oSt As ProcStatement = oHandler.SourceStatement

            Dim regNumber As Integer = oKeyValue.Key
            Log.Info("ScenarioDisconnectProc #" & regNumber.ToString() & " signaled.")

            Dim num As Integer = Array.IndexOf(oContextTable, Nothing)
            If num < 0 Then
                Log.Error("Too many contexts exist.")
                needTerminate = True
                Exit For
            End If
            Dim oContext As New Context(num)
            oContextTable(num) = oContext
            Log.Info("ScenarioContext(" & num.ToString() & ") spawned.")

            Try
                oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 1, oContext), Procedure)
                If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("Disconnect handler must be a proc with no params.")
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                Log.Error("The scenario aborted.")
                Status = ScenarioStatus.Aborted
                Terminate()
                Return
            End Try

            oContexts.AddLast(oContext)
            oReadyContexts.AddLast(oContext)
            needExecute = True
        Next oKeyValue

        If needTerminate Then
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
        ElseIf needExecute Then
            RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
        End If
    End Sub

    Private Sub ExecuteReadyContexts()
        While oReadyContexts.Count <> 0
            Dim oContext As Context = oReadyContexts.First.Value
            Dim isContextCountinue As Boolean = ExecuteReadyContext(oContext)
            If Status <> ScenarioStatus.Running Then Return
            oReadyContexts.RemoveFirst()
            If Not isContextCountinue Then
                oContexts.Remove(oContext)
                oContextTable(oContext.Number) = Nothing
                If oContext.Number = 0 Then
                    Log.Info("The scenario finished because the main context goes away.")
                    Status = ScenarioStatus.Finished
                    Terminate()
                    Return
                End If
            End If
        End While
    End Sub

    Private Function ExecuteReadyContext(ByVal oContext As Context) As Boolean
        'NOTE: GoTo��Connect�̂悤�ȃX�e�[�g�����g�́A���̃��\�b�h�̒���
        '�����S�̂��I���A���̂܂܌㑱�̃X�e�[�g�����g���J�n���邱�ƂɂȂ�B
        '����āA���̃��\�b�h�ł́A�Ăяo���ꂽ�ۂɁA�ŏ��Ɏ��s�����s�ԍ���
        '�L�^���Ă����A�����s���ēx���s���邱�ƂɂȂ�ꍇ�́A�s���ȃ��[�v��
        '���m�������̂Ƃ��āA�V�i���I���ُ�I�������邱�Ƃɂ��Ă���B
        '���̎d�l�ɂ��A��������܂�Connect���J��Ԃ��悤�ȃV�i���I�́A
        '���s���Ă��A������Connect�̍s�ɖ߂�̂ł͂Ȃ��AWait�Ȃǂ̍s��
        '���s��ɁAConnect�̍s�ɖ߂�Ȃ���΂Ȃ�Ȃ��B
        Dim oStartProc As Procedure = oContext.ExecProcedure
        Dim startPos As Integer = oContext.ExecPos
        Do
            While oContext.ExecPos = oContext.ExecProcedure.Statements.Count
                Dim oOldFrame As StackFrame = oContext.CallStack.Pop()
                If oOldFrame.CallerProcedure Is Nothing Then
                    Log.Info("This context goes away because its entry proc ended.")
                    If Config.DeleteScenarioContextDirOnContextEnd Then
                        Try
                            Utility.DeleteTemporalDirectory(Path.Combine(sPermittedPath, "#" & oContext.Number.ToString()))
                        Catch ex As Exception
                            Log.Error("Exception caught.", ex)
                        End Try
                    End If
                    Return False
                End If
                Log.Debug("Exited from the proc """ & oContext.ExecProcedure.Name & """.")
                oContext.ExecProcedure = oOldFrame.CallerProcedure
                oContext.ExecPos = oOldFrame.CallerPos + 1
            End While

            Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)
            If (oSt.Subject.Model = 0 OrElse oSt.Subject.Model = clientCode.Model) AndAlso _
               (oSt.Subject.RailSection = 0 OrElse oSt.Subject.RailSection = clientCode.RailSection) AndAlso _
               (oSt.Subject.StationOrder = 0 OrElse oSt.Subject.StationOrder = clientCode.StationOrder) AndAlso _
               (oSt.Subject.Corner = 0 OrElse oSt.Subject.Corner = clientCode.Corner) AndAlso _
               (oSt.Subject.Unit = 0 OrElse oSt.Subject.Unit = clientCode.Unit) Then

                Log.Info("ScenarioContext(" & oContext.Number.ToString() & ") L" & oSt.LineNumber.ToString() & ": " & oSt.Verb.ToString() & "...")
                If oDelegateForVerb(oSt.Verb)(Me, oContext, oSt) = False Then
                    Return False
                End If

                'Wait�n�X�e�[�g�����g���i�r���܂Łj���s�����ꍇ�́A�{���\�b�h���甲����B
                'NOTE: ProcOnFooBar���\�b�h�ɂ����đ��������s���邱�ƂɂȂ�B
                If oContext.ExecSeq IsNot Nothing OrElse oContext.ExecTimer IsNot Nothing Then
                    Exit Do
                End If
            Else
                oContext.ExecPos += 1
            End If

            If oContext.ExecProcedure Is oStartProc AndAlso oContext.ExecPos = startPos Then
                Log.Error("Evil loop detected.")
                Log.Error("The scenario aborted.")
                Status = ScenarioStatus.Aborted
                Terminate()
                Return False
            End If
        Loop
        Return True
    End Function

    Private Shared Function ExecStatementOfGoTo(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfCall(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim oNextProc As Procedure = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Procedure)
            Log.Debug("Entering to the proc """ & oNextProc.Name & """...")
            Dim nextProcParamCount As Integer = oNextProc.ParamNames.Length
            If oSt.Params.Length - 1 <> nextProcParamCount Then Throw New FormatException("The proc """ & oNextProc.Name & """ requires " & nextProcParamCount.ToString() & " param(s).")
            Dim oOldFrame As StackFrame = oContext.CallStack.Peek()
            Dim oNewFrame As New StackFrame(oContext.ExecProcedure, oContext.ExecPos)
            For i As Integer = 0 To nextProcParamCount - 1
                'NOTE: �X�̃p�����[�^��oOldFrame�̃��[�J���ϐ����Q�Ƃ��ĕ]�����Ȃ���΂Ȃ�Ȃ�����A
                '�����p���Đ�������ϐ��i�Ăяo��Proc�̉������j��oNewFrame�ɐ������Ȃ���΂Ȃ�Ȃ��B
                '����̂ɁA�܂Ƃ߂�Expand���邱�Ƃ͕s�\�ł���B
                '�܂��A�X�̃p�����[�^�̓W�J���ʂɊ܂܂��Z�~�R�����Ȃǂ́A�����̋�؂蕶����
                '�݂Ȃ��ׂ��ł͂Ȃ����߁A�u���̕K�v������B
                Dim sArg As String = DirectCast(oSt.Params(i + 1).Value, String)
                If Not oSt.Params(i + 1).IsExpanded Then
                    sArg = oEnv.oStringExpander.Expand(sArg, oOldFrame.LocalVariables, oContext.Number).Replace(";", "$[;]").Replace(">", "$[>]").Replace("$", "$[$]")
                End If

                Dim oHolder As New VarHolder()
                Dim sVarName As String = oNextProc.ParamNames(i)
                If sVarName.Chars(0) = "*"c Then
                    'NOTE: �������������t�@�����X�^�ϐ��ł���ꍇ�ɓ���ȏ����i�������Ɠ����ϐ����Q�Ƃ�����j��
                    '�s���K�v�����邽�߁AoStringExpander.Expander.Expand�͎g�p���Ȃ��B
                    Try
                        Dim sArgPrefix As Char = sArg.Chars(0)
                        If sArgPrefix = "*"c Then
                            oHolder.Value = oOldFrame.LocalVariables(sArg).Value
                        ElseIf sArgPrefix = "@"c Then
                            oHolder.Value = oEnv.oGlobalVariables(sArg)
                        Else
                            oHolder.Value = oOldFrame.LocalVariables(sArg)
                        End If
                    Catch ex As Exception
                        'NOTE: sArg.Length = 0 �̏ꍇ��A������sArg���o�^����Ă��Ȃ��ꍇ�ł���B
                        Throw New FormatException("The param(" & (i + 1).ToString() & ") is not consistent with the param(" & i.ToString() & ") of the proc """ & oNextProc.Name & """." & vbCrLf & _
                                                  "It must be a variable name.")
                    End Try

                    oNewFrame.LocalVariables.Add(sVarName, oHolder)
                Else
                    'NOTE: oEnv.oStringExpander.Expand("$SetVal<" & sVarName & ";" & sParam & ">", oNewFrame.LocalVariables, oContext.Number)
                    '��L�̂悤�ɁAoStringExpander.Expand��p���ĉ��������쐬���Ă��悢���A
                    '���ʂ������Ȃ邽�߁A���ڎ�������B
                    oHolder.Value = sArg
                    If sVarName.Chars(0) = "@"c Then
                        oEnv.oGlobalVariables.Add(sVarName, oHolder)
                    Else
                        oNewFrame.LocalVariables.Add(sVarName, oHolder)
                    End If
                End If
            Next i
            oContext.CallStack.Push(oNewFrame)
            oContext.ExecProcedure = oNextProc
            oContext.ExecPos = 0
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfExitProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Dim oOldFrame As StackFrame = oContext.CallStack.Pop()
        If oOldFrame.CallerProcedure Is Nothing Then
            Log.Info("This context goes away because its entry proc ended.")
            If Config.DeleteScenarioContextDirOnContextEnd Then
                Try
                    Utility.DeleteTemporalDirectory(Path.Combine(oEnv.sPermittedPath, "#" & oContext.Number.ToString()))
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                End Try
            End If
            Return False
        End If
        Log.Debug("Exited from the proc """ & oContext.ExecProcedure.Name & """.")
        oContext.ExecProcedure = oOldFrame.CallerProcedure
        oContext.ExecPos = oOldFrame.CallerPos + 1
        Return True
    End Function

    Private Shared Function ExecStatementOfFinishContext(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Log.Info("This context goes away.")
        If Config.DeleteScenarioContextDirOnContextEnd Then
            Try
                Utility.DeleteTemporalDirectory(Path.Combine(oEnv.sPermittedPath, "#" & oContext.Number.ToString()))
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If
        Return False
    End Function

    Private Shared Function ExecStatementOfConnect(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim r As Integer = oEnv.Connect()
            If r > 0 Then
                oContext.ExecPos += 1
            ElseIf r = 0 Then
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)
            Else
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 1, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfDisconnect(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        oEnv.Disconnect()
        'NOTE: ��LDisconnect�Ăяo���̒�����ProcOnConnectionDisappear���Ăяo����A
        '���̒���Terminate()�����s����邱�Ƃ����蓾��͂��B
        If oEnv.Status <> ScenarioStatus.Running Then Return False
        oContext.ExecPos += 1
        Return True
    End Function

    Private Shared Function ExecStatementOfActiveOne(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Dim sFilePath As String = Nothing
        Dim oTeleg As EkDodgyTelegram = Nothing
        Try
            Dim oFilePathParam As Object = Nothing
            If oContext.IterationTargets Is Nothing Then
                oFilePathParam = oEnv.EvaluateParam(oSt, 0, oContext)
                If oFilePathParam.GetType() Is GetType(String) Then
                    sFilePath = DirectCast(oFilePathParam, String)
                    Dim sFileName As String = Path.GetFileName(sFilePath)
                    If sFileName.IndexOf("?"c) <> -1 OrElse sFileName.IndexOf("*"c) <> -1 Then
                        Dim sPaths As String() = Directory.GetFiles(Path.GetDirectoryName(sFilePath), sFileName)
                        If sPaths.Length = 0 Then
                            Log.Warn("There is no file matched with [" & sFilePath & "].")
                            oContext.ExecPos += 1
                            Return True
                        End If
                        oContext.IterationTargets = sPaths
                        oContext.IterationPos = 0
                        sFilePath = sPaths(0)
                    End If
                End If
            Else
                Do
                    sFilePath = oContext.IterationTargets(oContext.IterationPos)
                    If File.Exists(sFilePath) Then Exit Do

                    'NOTE: �����́AoContext.IterationTargets�̍쐬��A
                    '����Context�ioContext�Ɠ����X���b�h�Ŏ��s����邪�A
                    'oContext���O���C�x���g�҂���Ԃ̊ԂȂǂɎ��s���꓾��j
                    '�ɂ���āAoContext.IterationTargets�̎����t�@�C����
                    '�폜���ꂽ�ꍇ�Ɏ��s�����z��ł���B
                    Log.Warn("The file [" & sFilePath & "] vanished.")

                    oContext.IterationPos += 1
                    If oContext.IterationPos >= oContext.IterationTargets.Length Then
                        oContext.IterationTargets = Nothing
                        oContext.IterationPos = 0
                        oContext.ExecPos += 1
                        Return True
                    End If
                Loop
            End If

            If sFilePath IsNot Nothing Then
                Log.Debug("Loading telegram from [" & sFilePath & "]...")
                Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                    oTeleg = oEnv.oTelegImporter.GetTelegramFromStream(oInputStream)
                End Using
            Else
                Dim oBytes As Byte() = DirectCast(oFilePathParam, Byte())
                oTeleg = oEnv.oTelegImporter.GetTelegramFromBytes(oBytes)
            End If

            If oTeleg Is Nothing Then
                'NOTE: oTelegImporter�ɓn�����o�C�g�񂪓d���Ƃ��Ă̍Œ���̏�����
                '�������Ă��Ȃ������ꍇ�ł���B���̃P�[�X�ł́AoTelegImporter��
                '���\�b�h�̒��ŃG���[���O���o�͍ς݂ł���B
                Log.Error("The scenario aborted.")
                oEnv.Status = ScenarioStatus.Aborted
                oEnv.Terminate()
                Return False
            End If

            If DirectCast(oEnv.EvaluateParam(oSt, 6, oContext), Boolean) = False Then
                sFilePath = Nothing
            End If

            oContext.ExecSeq = New EkAnonyReqTelegram(oTeleg, DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer), sFilePath)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try

        Dim sSeqName As String = "ActiveOneOfScenarioContext(" & oContext.Number.ToString() & ")"
        Log.Info("Register " & sSeqName & " as ActiveOne.")
        oEnv.RegisterActiveOne(oContext.ExecSeq, 0, 1, 1, sSeqName)
        Return True
    End Function

    Private Shared Function ExecStatementOfTryActiveOne(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Dim sFilePath As String = Nothing
        Dim oTeleg As EkDodgyTelegram = Nothing
        Try
            Dim oFilePathParam As Object = Nothing
            If oContext.IterationTargets Is Nothing Then
                oFilePathParam = oEnv.EvaluateParam(oSt, 0, oContext)
                If oFilePathParam.GetType() Is GetType(String) Then
                    sFilePath = DirectCast(oFilePathParam, String)
                    If sFilePath.Length = 0 Then
                        oContext.ExecPos += 1
                        Return True
                    End If

                    Dim sFileName As String = Path.GetFileName(sFilePath)
                    If sFileName.IndexOf("?"c) <> -1 OrElse sFileName.IndexOf("*"c) <> -1 Then
                        Dim sPaths As String() = Directory.GetFiles(Path.GetDirectoryName(sFilePath), sFileName)
                        If sPaths.Length = 0 Then
                            Log.Debug("There is no file matched with [" & sFilePath & "].")
                            oContext.ExecPos += 1
                            Return True
                        End If
                        oContext.IterationTargets = sPaths
                        oContext.IterationPos = 0
                        sFilePath = sPaths(0)
                    ElseIf Not File.Exists(sFilePath) Then
                        Log.Debug("The file [" & sFilePath & "] not found.")
                        oContext.ExecPos += 1
                        Return True
                    End If
                End If
            Else
                Do
                    sFilePath = oContext.IterationTargets(oContext.IterationPos)
                    If File.Exists(sFilePath) Then Exit Do

                    'NOTE: �����́AoContext.IterationTargets�̍쐬��A
                    '����Context�ioContext�Ɠ����X���b�h�Ŏ��s����邪�A
                    'oContext���O���C�x���g�҂���Ԃ̊ԂȂǂɎ��s���꓾��j
                    '�ɂ���āAoContext.IterationTargets�̎����t�@�C����
                    '�폜���ꂽ�ꍇ�Ɏ��s�����z��ł���B
                    Log.Warn("The file [" & sFilePath & "] vanished.")

                    oContext.IterationPos += 1
                    If oContext.IterationPos >= oContext.IterationTargets.Length Then
                        oContext.IterationTargets = Nothing
                        oContext.IterationPos = 0
                        oContext.ExecPos += 1
                        Return True
                    End If
                Loop
            End If

            If sFilePath IsNot Nothing Then
                Log.Debug("Loading telegram from [" & sFilePath & "]...")
                Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                    oTeleg = oEnv.oTelegImporter.GetTelegramFromStream(oInputStream)
                End Using
            Else
                Dim oBytes As Byte() = DirectCast(oFilePathParam, Byte())
                oTeleg = oEnv.oTelegImporter.GetTelegramFromBytes(oBytes)
            End If

            If oTeleg Is Nothing Then
                'NOTE: oTelegImporter�ɓn�����o�C�g�񂪓d���Ƃ��Ă̍Œ���̏�����
                '�������Ă��Ȃ������ꍇ�ł���B���̃P�[�X�ł́AoTelegImporter��
                '���\�b�h�̒��ŃG���[���O���o�͍ς݂ł���B
                Log.Error("The scenario aborted.")
                oEnv.Status = ScenarioStatus.Aborted
                oEnv.Terminate()
                Return False
            End If

            If DirectCast(oEnv.EvaluateParam(oSt, 6, oContext), Boolean) = False Then
                sFilePath = Nothing
            End If

            oContext.ExecSeq = New EkAnonyReqTelegram(oTeleg, DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer), sFilePath)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try

        Dim sSeqName As String = "ActiveOneOfScenarioContext(" & oContext.Number.ToString() & ")"
        Log.Info("Register " & sSeqName & " as ActiveOne.")
        oEnv.RegisterActiveOne(oContext.ExecSeq, 0, 1, 1, sSeqName)
        Return True
    End Function

    Private Shared Function ExecStatementOfActiveUll(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram
        Try
            Dim sSrcFilePath As String = Nothing
            Dim sTransferFileName As String = DirectCast(oEnv.EvaluateParam(oSt, 1, oContext), String)
            Dim sTransferFilePath As String = Path.Combine(oEnv.sPermittedPath, sTransferFileName)
            If oContext.IterationTargets Is Nothing Then
                Dim oSrcFilePathParam As Object = oEnv.EvaluateParam(oSt, 2, oContext)
                If oSrcFilePathParam.GetType() Is GetType(String) Then
                    sSrcFilePath = DirectCast(oSrcFilePathParam, String)
                    Dim sFileName As String = Path.GetFileName(sSrcFilePath)
                    If sFileName.IndexOf("?"c) <> -1 OrElse sFileName.IndexOf("*"c) <> -1 Then
                        Dim sPaths As String() = Directory.GetFiles(Path.GetDirectoryName(sSrcFilePath), sFileName)
                        If sPaths.Length = 0 Then
                            Log.Warn("There is no file matched with [" & sSrcFilePath & "].")
                            oContext.ExecPos += 1
                            Return True
                        End If
                        oContext.IterationTargets = sPaths
                        oContext.IterationPos = 0
                        sSrcFilePath = sPaths(0)
                    End If
                    Log.Debug("Copying file from [" & sSrcFilePath & "] to [" & sTransferFilePath & "]...")
                    MyUtility.CopyFileIfNeeded(sSrcFilePath, sTransferFilePath, True)
                Else
                    Dim oBytes As Byte() = DirectCast(oSrcFilePathParam, Byte())
                    Using oOutputStream As New FileStream(sTransferFilePath, FileMode.Create, FileAccess.Write)
                        oOutputStream.Write(oBytes, 0, oBytes.Length)
                    End Using
                End If
            Else
                Do
                    sSrcFilePath = oContext.IterationTargets(oContext.IterationPos)
                    If File.Exists(sSrcFilePath) Then Exit Do

                    'NOTE: �����́AoContext.IterationTargets�̍쐬��A
                    '����Context�ioContext�Ɠ����X���b�h�Ŏ��s����邪�A
                    'oContext���O���C�x���g�҂���Ԃ̊ԂȂǂɎ��s���꓾��j
                    '�ɂ���āAoContext.IterationTargets�̎����t�@�C����
                    '�폜���ꂽ�ꍇ�Ɏ��s�����z��ł���B
                    Log.Warn("The file [" & sSrcFilePath & "] vanished.")

                    oContext.IterationPos += 1
                    If oContext.IterationPos >= oContext.IterationTargets.Length Then
                        oContext.IterationTargets = Nothing
                        oContext.IterationPos = 0
                        oContext.ExecPos += 1
                        Return True
                    End If
                Loop
                Log.Debug("Copying file from [" & sSrcFilePath & "] to [" & sTransferFilePath & "]...")
                MyUtility.CopyFileIfNeeded(sSrcFilePath, sTransferFilePath, True)
            End If

            Dim sTransferFilePathInFtp As String = Path.Combine(oEnv.sPermittedPathInFtp, sTransferFileName)
            If Not MyUtility.IsAsciiString(sTransferFilePathInFtp) OrElse sTransferFilePathInFtp.Length > 80 Then
                Throw New FormatException("The file name may be dangerous to EkClientDrivenUllReqTelegram.")
            End If

            'NOTE: �p�����[�^�̕]��������0,3,4,5,6,8�ł���B
            oXllReqTeleg = New EkClientDrivenUllReqTelegram( _
               oEnv.oTelegGene, _
               DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Byte), _
               ContinueCode.Start, _
               sTransferFilePathInFtp, _
               DirectCast(oEnv.EvaluateParam(oSt, 3, oContext), String), _
               DirectCast(oEnv.EvaluateParam(oSt, 4, oContext), Integer), _
               DirectCast(oEnv.EvaluateParam(oSt, 5, oContext), Integer), _
               DirectCast(oEnv.EvaluateParam(oSt, 6, oContext), Integer), _
               If(DirectCast(oEnv.EvaluateParam(oSt, 10, oContext), Boolean) = True, sSrcFilePath, Nothing))
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try

        oContext.ExecSeq = oXllReqTeleg
        oEnv.RegisterActiveUll(oXllReqTeleg, 0, 1, 1)
        Return True
    End Function

    Private Shared Function ExecStatementOfTryActiveUll(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram
        Try
            Dim sSrcFilePath As String = Nothing
            Dim sTransferFileName As String = DirectCast(oEnv.EvaluateParam(oSt, 1, oContext), String)
            Dim sTransferFilePath As String = Path.Combine(oEnv.sPermittedPath, sTransferFileName)
            If oContext.IterationTargets Is Nothing Then
                Dim oSrcFilePathParam As Object = oEnv.EvaluateParam(oSt, 2, oContext)
                If oSrcFilePathParam.GetType() Is GetType(String) Then
                    sSrcFilePath = DirectCast(oSrcFilePathParam, String)
                    If sSrcFilePath.Length = 0 Then
                        oContext.ExecPos += 1
                        Return True
                    End If

                    Dim sFileName As String = Path.GetFileName(sSrcFilePath)
                    If sFileName.IndexOf("?"c) <> -1 OrElse sFileName.IndexOf("*"c) <> -1 Then
                        Dim sPaths As String() = Directory.GetFiles(Path.GetDirectoryName(sSrcFilePath), sFileName)
                        If sPaths.Length = 0 Then
                            Log.Debug("There is no file matched with [" & sSrcFilePath & "].")
                            oContext.ExecPos += 1
                            Return True
                        End If
                        oContext.IterationTargets = sPaths
                        oContext.IterationPos = 0
                        sSrcFilePath = sPaths(0)
                    ElseIf Not File.Exists(sSrcFilePath) Then
                        Log.Debug("The file [" & sSrcFilePath & "] not found.")
                        oContext.ExecPos += 1
                        Return True
                    End If
                    Log.Debug("Copying file from [" & sSrcFilePath & "] to [" & sTransferFilePath & "]...")
                    MyUtility.CopyFileIfNeeded(sSrcFilePath, sTransferFilePath, True)
                Else
                    Dim oBytes As Byte() = DirectCast(oSrcFilePathParam, Byte())
                    Using oOutputStream As New FileStream(sTransferFilePath, FileMode.Create, FileAccess.Write)
                        oOutputStream.Write(oBytes, 0, oBytes.Length)
                    End Using
                End If
            Else
                Do
                    sSrcFilePath = oContext.IterationTargets(oContext.IterationPos)
                    If File.Exists(sSrcFilePath) Then Exit Do

                    'NOTE: �����́AoContext.IterationTargets�̍쐬��A
                    '����Context�ioContext�Ɠ����X���b�h�Ŏ��s����邪�A
                    'oContext���O���C�x���g�҂���Ԃ̊ԂȂǂɎ��s���꓾��j
                    '�ɂ���āAoContext.IterationTargets�̎����t�@�C����
                    '�폜���ꂽ�ꍇ�Ɏ��s�����z��ł���B
                    Log.Warn("The file [" & sSrcFilePath & "] vanished.")

                    oContext.IterationPos += 1
                    If oContext.IterationPos >= oContext.IterationTargets.Length Then
                        oContext.IterationTargets = Nothing
                        oContext.IterationPos = 0
                        oContext.ExecPos += 1
                        Return True
                    End If
                Loop
                Log.Debug("Copying file from [" & sSrcFilePath & "] to [" & sTransferFilePath & "]...")
                MyUtility.CopyFileIfNeeded(sSrcFilePath, sTransferFilePath, True)
            End If

            Dim sTransferFilePathInFtp As String = Path.Combine(oEnv.sPermittedPathInFtp, sTransferFileName)
            If Not MyUtility.IsAsciiString(sTransferFilePathInFtp) OrElse sTransferFilePathInFtp.Length > 80 Then
                Throw New FormatException("The file name may be dangerous to EkClientDrivenUllReqTelegram.")
            End If

            'NOTE: �p�����[�^�̕]��������0,3,4,5,6,8�ł���B
            oXllReqTeleg = New EkClientDrivenUllReqTelegram( _
               oEnv.oTelegGene, _
               DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Byte), _
               ContinueCode.Start, _
               sTransferFilePathInFtp, _
               DirectCast(oEnv.EvaluateParam(oSt, 3, oContext), String), _
               DirectCast(oEnv.EvaluateParam(oSt, 4, oContext), Integer), _
               DirectCast(oEnv.EvaluateParam(oSt, 5, oContext), Integer), _
               DirectCast(oEnv.EvaluateParam(oSt, 6, oContext), Integer), _
               If(DirectCast(oEnv.EvaluateParam(oSt, 10, oContext), Boolean) = True, sSrcFilePath, Nothing))
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try

        oContext.ExecSeq = oXllReqTeleg
        oEnv.RegisterActiveUll(oXllReqTeleg, 0, 1, 1)
        Return True
    End Function

    Private Shared Function ExecStatementOfWaitForPassiveOne(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim intTicks As Integer = DirectCast(oEnv.EvaluateParam(oSt, 5, oContext), Integer)
            If intTicks > 0 Then
                oContext.ExecTimer = New TickTimer(intTicks)
                oContext.TelegCompObj = oEnv.EvaluateParam(oSt, 0, oContext)
                oContext.TelegMaskObj = oEnv.EvaluateParam(oSt, 1, oContext)
                oContext.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer)
                oEnv.RegisterTimer(oContext.ExecTimer, TickTimer.GetSystemTick())
                oEnv.oPassiveOneWaitingContexts.AddLast(oContext)
            Else
                Log.Warn("No ticks to wait.")
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 6, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfWaitForPassiveOneToNak(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim intTicks As Integer = DirectCast(oEnv.EvaluateParam(oSt, 5, oContext), Integer)
            If intTicks > 0 Then
                oContext.ExecTimer = New TickTimer(intTicks)
                oContext.TelegCompObj = oEnv.EvaluateParam(oSt, 0, oContext)
                oContext.TelegMaskObj = oEnv.EvaluateParam(oSt, 1, oContext)
                oContext.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer)
                oEnv.RegisterTimer(oContext.ExecTimer, TickTimer.GetSystemTick())
                oEnv.oPassiveOneWaitingContexts.AddLast(oContext)
            Else
                Log.Warn("No ticks to wait.")
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 6, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfWaitForPassiveUll(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim intTicks As Integer = DirectCast(oEnv.EvaluateParam(oSt, 8, oContext), Integer)
            If intTicks > 0 Then
                oContext.ExecTimer = New TickTimer(intTicks)
                oContext.TelegCompObj = oEnv.EvaluateParam(oSt, 0, oContext)
                oContext.TelegMaskObj = oEnv.EvaluateParam(oSt, 1, oContext)
                oContext.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer)
                oEnv.RegisterTimer(oContext.ExecTimer, TickTimer.GetSystemTick())
                oEnv.oPassiveUllWaitingContexts.AddLast(oContext)
            Else
                Log.Warn("No ticks to wait.")
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 11, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfWaitForPassiveUllToNak(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim intTicks As Integer = DirectCast(oEnv.EvaluateParam(oSt, 4, oContext), Integer)
            If intTicks > 0 Then
                oContext.ExecTimer = New TickTimer(intTicks)
                oContext.TelegCompObj = oEnv.EvaluateParam(oSt, 0, oContext)
                oContext.TelegMaskObj = oEnv.EvaluateParam(oSt, 1, oContext)
                oContext.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer)
                oEnv.RegisterTimer(oContext.ExecTimer, TickTimer.GetSystemTick())
                oEnv.oPassiveUllWaitingContexts.AddLast(oContext)
            Else
                Log.Warn("No ticks to wait.")
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 5, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfWaitForPassiveDll(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim intTicks As Integer = DirectCast(oEnv.EvaluateParam(oSt, 11, oContext), Integer)
            If intTicks > 0 Then
                oContext.ExecTimer = New TickTimer(intTicks)
                oContext.TelegCompObj = oEnv.EvaluateParam(oSt, 0, oContext)
                oContext.TelegMaskObj = oEnv.EvaluateParam(oSt, 1, oContext)
                oContext.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer)
                oContext.DataCompObj = oEnv.EvaluateParam(oSt, 3, oContext)
                oContext.DataMaskObj = oEnv.EvaluateParam(oSt, 4, oContext)
                oContext.DataEvaluationLen = DirectCast(oEnv.EvaluateParam(oSt, 5, oContext), Integer)
                oContext.ListCompObj = oEnv.EvaluateParam(oSt, 6, oContext)
                oContext.ListMaskObj = oEnv.EvaluateParam(oSt, 7, oContext)
                oContext.ListEvaluationLen = DirectCast(oEnv.EvaluateParam(oSt, 8, oContext), Integer)
                oEnv.RegisterTimer(oContext.ExecTimer, TickTimer.GetSystemTick())
                oEnv.oPassiveDllWaitingContexts.AddLast(oContext)
            Else
                Log.Warn("No ticks to wait.")
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 12, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfWaitForPassiveDllToNak(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim intTicks As Integer = DirectCast(oEnv.EvaluateParam(oSt, 4, oContext), Integer)
            If intTicks > 0 Then
                oContext.ExecTimer = New TickTimer(intTicks)
                oContext.TelegCompObj = oEnv.EvaluateParam(oSt, 0, oContext)
                oContext.TelegMaskObj = oEnv.EvaluateParam(oSt, 1, oContext)
                oContext.TelegEvaluationLen = DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer)
                oEnv.RegisterTimer(oContext.ExecTimer, TickTimer.GetSystemTick())
                oEnv.oPassiveDllWaitingContexts.AddLast(oContext)
            Else
                Log.Warn("No ticks to wait.")
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 5, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfWait(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim intTicks As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)
            If intTicks > 0 Then
                oContext.ExecTimer = New TickTimer(intTicks)
                oEnv.RegisterTimer(oContext.ExecTimer, TickTimer.GetSystemTick())
            Else
                Log.Warn("No ticks to wait.")
                oContext.ExecPos += 1
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfWaitUntil(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Dim now As DateTime = DateTime.Now
        Dim d As DateTime
        Try
            Dim oTimeParam As Object = oEnv.EvaluateParam(oSt, 0, oContext)
            If oTimeParam.GetType Is GetType(DateTime) Then
                d = DirectCast(oTimeParam, DateTime)
            ElseIf oTimeParam.GetType Is GetType(Long) Then
                d = oEnv.oContextTable(0).StartTime.AddMilliseconds(CDbl(oTimeParam))
            ElseIf oTimeParam.GetType Is GetType(Integer) Then
                d = oContext.StartTime.AddMilliseconds(CDbl(oTimeParam))
            Else
                d = DateTime.ParseExact(DirectCast(oTimeParam, String), "HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None)
                If now.Subtract(d).TotalSeconds >= 60 Then
                    d = d.AddDays(1)
                End If
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try

        If d > now Then
            oContext.ExecTimer = New TickTimer(CLng(d.Subtract(now).TotalMilliseconds))
            oEnv.RegisterTimer(oContext.ExecTimer, TickTimer.GetSystemTick())
        Else
            Log.Warn("No ticks to wait.")
            oContext.ExecPos += 1
        End If
        Return True
    End Function

    Private Shared Function ExecStatementOfRegPassiveOneProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)

            Dim oHandler As PassiveOneHandler = Nothing
            If oEnv.oPassiveOneHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Warn("ScenarioPassiveOneProc #" & regNumber.ToString() & " exists. It will be overwritten.")
                oEnv.oPassiveOneHandlers.Remove(regNumber)
            End If

            oHandler = New PassiveOneHandler(oEnv, oSt, oContext)
            oEnv.oPassiveOneHandlers.Add(regNumber, oHandler)
            Log.Info("ScenarioPassiveOneProc #" & regNumber.ToString() & " registered.")
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfRegPassiveOneProcToNak(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)

            Dim oHandler As PassiveOneHandler = Nothing
            If oEnv.oPassiveOneHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Warn("ScenarioPassiveOneProc #" & regNumber.ToString() & " exists. It will be overwritten.")
                oEnv.oPassiveOneHandlers.Remove(regNumber)
            End If

            oHandler = New PassiveOneHandler(oEnv, oSt, oContext)
            oEnv.oPassiveOneHandlers.Add(regNumber, oHandler)
            Log.Info("ScenarioPassiveOneProc #" & regNumber.ToString() & " registered.")
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfUnregPassiveOneProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)
            Dim oHandler As PassiveOneHandler = Nothing
            If oEnv.oPassiveOneHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Info("ScenarioPassiveOneProc #" & regNumber.ToString() & " unregistered.")
                oEnv.oPassiveOneHandlers.Remove(regNumber)
            Else
                Log.Warn("ScenarioPassiveOneProc #" & regNumber.ToString() & " does not exist.")
            End If
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfRegPassiveUllProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)

            Dim oHandler As PassiveUllHandler = Nothing
            If oEnv.oPassiveUllHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Warn("ScenarioPassiveUllProc #" & regNumber.ToString() & " exists. It will be overwritten.")
                oEnv.oPassiveUllHandlers.Remove(regNumber)
            End If

            oHandler = New PassiveUllHandler(oEnv, oSt, oContext)
            oEnv.oPassiveUllHandlers.Add(regNumber, oHandler)
            Log.Info("ScenarioPassiveUllProc #" & regNumber.ToString() & " registered.")
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfRegPassiveUllProcToNak(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)

            Dim oHandler As PassiveUllHandler = Nothing
            If oEnv.oPassiveUllHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Warn("ScenarioPassiveUllProc #" & regNumber.ToString() & " exists. It will be overwritten.")
                oEnv.oPassiveUllHandlers.Remove(regNumber)
            End If

            oHandler = New PassiveUllHandler(oEnv, oSt, oContext)
            oEnv.oPassiveUllHandlers.Add(regNumber, oHandler)
            Log.Info("ScenarioPassiveUllProc #" & regNumber.ToString() & " registered.")
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfUnregPassiveUllProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)
            Dim oHandler As PassiveUllHandler = Nothing
            If oEnv.oPassiveUllHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Info("ScenarioPassiveUllProc #" & regNumber.ToString() & " unregistered.")
                oEnv.oPassiveUllHandlers.Remove(regNumber)
            Else
                Log.Warn("ScenarioPassiveUllProc #" & regNumber.ToString() & " does not exist.")
            End If
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfRegPassiveDllProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)

            Dim oHandler As PassiveDllHandler = Nothing
            If oEnv.oPassiveDllHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Warn("ScenarioPassiveDllProc #" & regNumber.ToString() & " exists. It will be overwritten.")
                oEnv.oPassiveDllHandlers.Remove(regNumber)
            End If

            oHandler = New PassiveDllHandler(oEnv, oSt, oContext)
            oEnv.oPassiveDllHandlers.Add(regNumber, oHandler)
            Log.Info("ScenarioPassiveDllProc #" & regNumber.ToString() & " registered.")
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfRegPassiveDllProcToNak(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)

            Dim oHandler As PassiveDllHandler = Nothing
            If oEnv.oPassiveDllHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Warn("ScenarioPassiveDllProc #" & regNumber.ToString() & " exists. It will be overwritten.")
                oEnv.oPassiveDllHandlers.Remove(regNumber)
            End If

            oHandler = New PassiveDllHandler(oEnv, oSt, oContext)
            oEnv.oPassiveDllHandlers.Add(regNumber, oHandler)
            Log.Info("ScenarioPassiveDllProc #" & regNumber.ToString() & " registered.")
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfUnregPassiveDllProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)
            Dim oHandler As PassiveDllHandler = Nothing
            If oEnv.oPassiveDllHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Info("ScenarioPassiveDllProc #" & regNumber.ToString() & " unregistered.")
                oEnv.oPassiveDllHandlers.Remove(regNumber)
            Else
                Log.Warn("ScenarioPassiveDllProc #" & regNumber.ToString() & " does not exist.")
            End If
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfRegDisconnectProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)

            Dim oHandler As DisconnectHandler = Nothing
            If oEnv.oDisconnectHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Warn("ScenarioDisconnectProc #" & regNumber.ToString() & " exists. It will be overwritten.")
                oEnv.oDisconnectHandlers.Remove(regNumber)
            End If

            oHandler = New DisconnectHandler(oSt)
            oEnv.oDisconnectHandlers.Add(regNumber, oHandler)
            Log.Info("ScenarioDisconnectProc #" & regNumber.ToString() & " registered.")
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfUnregDisconnectProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)
            Dim oHandler As DisconnectHandler = Nothing
            If oEnv.oDisconnectHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Info("ScenarioDisconnectProc #" & regNumber.ToString() & " unregistered.")
                oEnv.oDisconnectHandlers.Remove(regNumber)
            Else
                Log.Warn("ScenarioDisconnectProc #" & regNumber.ToString() & " does not exist.")
            End If
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfRegTimerProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)

            Dim oHandler As TimerHandler = Nothing
            If oEnv.oTimerHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Warn("ScenarioTimerProc #" & regNumber.ToString() & " exists. It will be overwritten.")
                oEnv.UnregisterTimer(oHandler.Timer)
                oEnv.oTimerHandlers.Remove(regNumber)
            End If

            oHandler = New TimerHandler(New TickTimer(DirectCast(oEnv.EvaluateParam(oSt, 2, oContext), Integer)), _
                                        DirectCast(oEnv.EvaluateParam(oSt, 1, oContext), Integer), _
                                        oSt)
            oEnv.RegisterTimer(oHandler.Timer, TickTimer.GetSystemTick())
            oEnv.oTimerHandlers.Add(regNumber, oHandler)
            Log.Info("ScenarioTimerProc #" & regNumber.ToString() & " registered.")
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfUnregTimerProc(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim regNumber As Integer = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), Integer)
            Dim oHandler As TimerHandler = Nothing
            If oEnv.oTimerHandlers.TryGetValue(regNumber, oHandler) Then
                Log.Info("ScenarioTimerProc #" & regNumber.ToString() & " unregistered.")
                oEnv.UnregisterTimer(oHandler.Timer)
                oEnv.oTimerHandlers.Remove(regNumber)
            Else
                Log.Warn("ScenarioTimerProc #" & regNumber.ToString() & " does not exist.")
            End If
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfFinishScenario(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Log.Info("The scenario finished.")
        oEnv.Status = ScenarioStatus.Finished
        oEnv.Terminate()
        Return False
    End Function

    Private Shared Function ExecStatementOfAbortScenario(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Log.Error("The scenario aborted.")
        oEnv.Status = ScenarioStatus.Aborted
        oEnv.Terminate()
        Return False
    End Function

    Private Shared Function ExecStatementOfEvaluate(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            oEnv.EvaluateParam(oSt, 0, oContext)
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfPrint(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            Dim s As String = DirectCast(oEnv.EvaluateParam(oSt, 0, oContext), String)
            Log.Info(s)
            oContext.ExecPos += 1
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfCheckBinFile(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            If MyUtility.IsMatchBin(oEnv.EvaluateParam(oSt, 0, oContext), oEnv.EvaluateParam(oSt, 1, oContext), oEnv.EvaluateParam(oSt, 2, oContext), DirectCast(oEnv.EvaluateParam(oSt, 3, oContext), Integer)) Then
                Log.Debug("Matched.")
                oContext.ExecPos += 1
            Else
                Log.Debug("Unmatched.")
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 4, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function ExecStatementOfCheckCsvFile(ByVal oEnv As ScenarioEnv, ByVal oContext As Context, ByVal oSt As ProcStatement) As Boolean
        Try
            If MyUtility.IsMatchCsv(oEnv.EvaluateParam(oSt, 0, oContext), oEnv.EvaluateParam(oSt, 1, oContext), oEnv.EvaluateParam(oSt, 2, oContext), DirectCast(oEnv.EvaluateParam(oSt, 3, oContext), Integer)) Then
                Log.Debug("Matched.")
                oContext.ExecPos += 1
            Else
                Log.Debug("Unmatched.")
                oContext.ExecPos = DirectCast(oEnv.EvaluateParam(oSt, 4, oContext), Integer)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            oEnv.Status = ScenarioStatus.Aborted
            oEnv.Terminate()
            Return False
        End Try
        Return True
    End Function

    Private Shared Function IsValidToken(ByVal sText As String) As Boolean
        'If sText.Length = 0 Then Return False  'NOTE: ����͌Ăь��Ń`�F�b�N����B
        For i As Integer = 0 To sText.Length - 1
            If Not Char.IsLetterOrDigit(sText, i) Then Return False
        Next i
        Return True
    End Function

    Private Sub Terminate()
        'NOTE: �V�i���I�̃f�o�b�O���K�v�Ȃ�A�ȉ����s���O�Ɏ��{����B

        oPassiveOneHandlers.Clear()
        oPassiveUllHandlers.Clear()
        oPassiveDllHandlers.Clear()
        oDisconnectHandlers.Clear()

        For Each oHandler As TimerHandler In oTimerHandlers.Values
            UnregisterTimer(oHandler.Timer)
        Next oHandler
        oTimerHandlers.Clear()

        For Each oContext As Context In oContexts
            If oContext.ExecTimer IsNot Nothing Then
                UnregisterTimer(oContext.ExecTimer)
            End If
            If Config.DeleteScenarioContextDirOnContextEnd Then
                Try
                    Utility.DeleteTemporalDirectory(Path.Combine(sPermittedPath, "#" & oContext.Number.ToString()))
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                End Try
            End If
        Next oContext
        oContexts.Clear()
        oReadyContexts.Clear()
        oPassiveOneWaitingContexts.Clear()
        oPassiveUllWaitingContexts.Clear()
        oPassiveDllWaitingContexts.Clear()
        Array.Clear(oContextTable, 0, oContextTable.Length)

        oGlobalVariables = Nothing
        oStringExpander.GlobalVariables = Nothing

        oAssemblies = Nothing
        oStringExpander.Assemblies = Nothing

        oProcedures = Nothing
    End Sub

End Class

''' <summary>
''' �V�i���I��ԁB
''' </summary>
Public Enum ScenarioStatus As Integer
    Initial
    Loaded
    Running
    Aborted
    Finished
    Stopped
End Enum
