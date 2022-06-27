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

Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' シナリオを読込んで実行するクラス。
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

    'NOTE: ProcStatementは実は結構大きいので、StructureではなくClassとする。
    'シナリオの行数など限られているし、作成時や解放時に限ったコストでもStructureが有利とは言えない。
    'そして、Structureとした場合は、シナリオ内のループを実行中に何度も実施することになる
    'Procedure.Statementsの要素の取得において、値コピーのコストが酷いことになるはず。
    'OPT: これのようにEkCodeをメンバとするゆえに意外と大きいStructureはあるかもしれないので、
    'EkCodeの各プロパティの値を保持するための内部メンバ変数の型を切り詰めておくとよい。
    Private Class ProcStatement
        Public Subject As EkCode
        Public Verb As StatementVerb
        Public Params As StatementParam()
        Public LineNumber As Integer 'シナリオファイル内の行番号（ログ出力でのみ使用）
        Public Function Clone() As ProcStatement
            Return DirectCast(MemberwiseClone(), ProcStatement)
        End Function
    End Class

    Private Class Procedure
        Public Name As String 'プロシージャ名（ロード時以外は、ログ出力でのみ使用）
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
        'NOTE: コンテキストが、能動的シーケンスの行の完了待ちの間は、
        '当該シーケンスで送信するREQ電文の参照をExecSeqに保持することになっている。
        'NOTE: コンテキストが、受動的シーケンス待ちや時間待ちなど、WaitFoo系の行の
        '完了待ちの間は、当該行完了待ちのためのTickTimerの参照をExecTimerに
        '保持することになっている。
        'NOTE: コンテキストが、受動的シーケンス待ちの行の完了待ちでかつ、
        '当該コンテキストの当該行に実行中のシーケンスが紐づいて以降は、
        '当該シーケンスのREQ電文の参照をExecSeqに保持することになっている。
        Public Number As Integer
        Public StartTime As DateTime
        Public ExecProcedure As Procedure
        Public ExecPos As Integer
        Public ExecSeq As EkReqTelegram
        Public ExecTimer As TickTimer
        Public IterationTargets As String()
        Public IterationPos As Integer
        Public CallStack As Stack(Of StackFrame)

        'NOTE: これらは比較しながら待機する場合の比較用展開済みパラメータ。
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
        'NOTE: ハンドラに実行中のシーケンスが紐づいて以降、
        'シーケンスが終わるまでの間は、
        '当該シーケンスのREQ電文の参照をBindSeqに保持することになっている。
        'また、当該ハンドラに設定した事象が発生したことでContextを生成し、
        '別のメソッドで実行を開始する場合、実行を開始するまでの期間は、
        'その参照をSpawnedContextに保持することになっている。
        'NOTE: BindSeqやSpawnedContextに何かを保持しているときは、
        '当該ハンドラに別のシーケンスに紐づくことはないが、
        'それはPassiveUll自体が複数同時に実行されることがないため
        'であり、その前提がなくなった場合は実装を修正する必要がある。
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
        'NOTE: ハンドラに実行中のシーケンスが紐づいて以降、
        'シーケンスが終わるまでの間は、
        '当該シーケンスのREQ電文の参照をBindSeqに保持することになっている。
        'また、当該ハンドラに設定した事象が発生したことでContextを生成し、
        '別のメソッドで実行を開始する場合、実行を開始するまでの期間は、
        'その参照をSpawnedContextに保持することになっている。
        'NOTE: BindSeqやSpawnedContextに何かを保持しているときは、
        '当該ハンドラに別のシーケンスに紐づくことはないが、
        'それはPassiveDll自体が複数同時に実行されることがないため
        'であり、その前提がなくなった場合は実装を修正する必要がある。
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
        'NOTE: このメソッドで oStatement.Params(i)の値を書き換える必要はない。
        '正確に言うと、書き換えてはならない。
        'このメソッドは「$」が含まれるパラメータについてのみ使用される。
        '「$」が含まれるパラメータは、評価するときの状況（変数の値や日時）によって
        '評価結果が変化する可能性がある上、外部プロセスに作業を実行させることも
        '目的としているため、出現の都度、評価する必要がある。

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
                    'NOTE: oStatementからその次行を調べるのは処理コストが掛かるため、
                    'oContext.ExecPosがoStatementを含むProcにおけるoStatementの位置で
                    'あることを前提に、飛躍した実装を行っている。
                    Return oContext.ExecPos + 1
                Else
                    'NOTE: 元々あまり意味がないため、このメソッドで変換を行うケース（ドル記号を含んでいたケース）では、
                    '性能優先で、展開後の文字列についてもCTypeParamTextによる事前の文字種のチェックは行わないことにする。
                    '不正な文字を含んでいればPosOfLabelsからみつからないことで、エラーになるはず。
                    'NOTE: ラベルをパラメータとするRegFooProc系ステートメントは存在しないため、
                    'oContext.ExecProcedureは、必ずoStatementを含むProcである。
                    Dim pos As Integer
                    If oContext.ExecProcedure.PosOfLabels.TryGetValue(s.ToUpperInvariant(), pos) = False Then
                        '可読でない文字をログ出力する可能性については排除する。
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
                    'NOTE: 元々あまり意味がないため、このメソッドで変換を行うケース（ドル記号を含んでいたケース）では、
                    '性能優先で、展開後の文字列についてもCTypeParamTextによる事前の文字種のチェックは行わないことにする。
                    '不正な文字を含んでいればoProceduresからみつからないことで、エラーになるはず。
                    Dim oTargetProc As Procedure = Nothing
                    If oProcedures.TryGetValue(s.ToUpperInvariant(), oTargetProc) = False Then
                        '可読でない文字をログ出力する可能性については排除する。
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
                'NOTE: このメソッドでは文字種チェックだけ行い、Stringを返却しているが、
                '後でStatements配列の要素番号（Integer）に差し替えるので、注意。
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
                'NOTE: このメソッドでは文字種チェックだけ行い、Stringを返却しているが、
                '後でProcedureへの参照に差し替えるので、注意。
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
                'NOTE: このメソッドがコンパイル時に呼び出される場合は、sに"$"が含まれている可能性はない。
                '実行時に（展開後の文字列に対して）呼び出されるとしたら（"$[$]"の展開結果としての）"$"が含まれる可能性があるが、
                '実行時には（効率化のため）ProcParamsに対してこのメソッドは使わないことにしている。
                'NOTE: ここが実行されるのは、sに"$"が含まれていない場合であるから、もし、セミコロンが含まれていたとして、
                '展開を実行した（省略しない）としても、関数の引数区切り文字とみなされることはない、
                '無害なセミコロンである。
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

                    'NOTE: このパラメータがMyUtility.IsMatchBinに渡されるなら、
                    '「Bytes:～」という文字列のままでも正しく評価できるが、このメソッドがシナリオの
                    'ロード時に呼ばれる場合などは、その時点でByte配列化しておく方が効率的であるし、
                    '書式の誤りをロード時点で検出することができるなど、利点も多い。
                    Dim preLen As Integer = "Bytes:".Length
                    Return MyUtility.GetBytesFromHyphenatedHexadecimalString(s, preLen, s.Length - preLen)

                ElseIf s.StartsWith("Fields:", StringComparison.OrdinalIgnoreCase) Then
                    If t = ParamType.OutCsvFilePath OrElse t = ParamType.BinFilePath OrElse t = ParamType.OptBinFilePath OrElse t = ParamType.OutBinFilePath Then
                        Throw New FormatException("Fields is not allowed here.")
                    End If

                    'NOTE: このパラメータがMyUtility.IsMatchCsvに渡されるなら、
                    '「Fields:～」という文字列のままでも正しく評価できるが、このメソッドがシナリオの
                    'ロード時に呼ばれる場合などは、その時点でString配列化しておく方が効率的であるし、
                    '書式の誤りをロード時点で検出することができるなど、利点も多い。
                    Dim preLen As Integer = "Fields:".Length
                    Return MyUtility.GetFieldsFromSpaceDelimitedString(s.Substring(preLen))

                Else
                    If t = ParamType.OutBinFilePath OrElse t = ParamType.OutCsvFilePath Then
                        If s.IndexOf("*"c) <> -1 OrElse s.IndexOf("?"c) <> -1 Then
                            Throw New FormatException("Wildcard is not allowed here.")
                        End If
                    End If

                    'NOTE: このパラメータがMyUtility.IsMatchBinやMyUtility.IsMatchCsvなどに渡されるなら、
                    '一緒にsScenarioBasePathも渡すようにすることで、（絶対パスでない場合の）連結も
                    '行われるが、このメソッドがシナリオのロード時に呼ばれる場合などは、その時点で
                    '絶対パス化しておく方が効率的であるため、このようにしている。
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
                        'NOTE: iは「Trim済みの」sからみつけた空白の位置なので、
                        '「i + 1」も有効な位置である（空白の次の文字は必ず存在する）。
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
                        'NOTE: iは「Trim済みの」sからみつけた空白の位置なので、
                        '「i + 1」も有効な位置である（空白の次の文字は必ず存在する）。
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

    'NOTE: 文脈の実行は、oReadyContextsにキューイングされているものがなくなるまで、
    '連続的に行います。具体的には、まず、oReadyContextsの先頭にある文脈について、
    '文を連続的に実行します。能動的シーケンスの完了や受動的シーケンスの検知や
    '単純な時間経過等を待つ必要が生じたら、oReadyContextsからデキューして、
    '次にキューイングされている文脈について同様の処理を行います。
    '以上の処理は、oRootTimerのハンドラ（ProcOnTimeoutの中）で実行します。
    '一方、たとえば能動的単発シーケンスが完了した際は、ProcOnActiveOneCompleteが
    '呼び出されますが、その際は、それを待ってた文脈がないかを検索し、
    'あれば、その文脈をoReadyContextsにエンキューします。ただし、その場では
    '文脈の実行は実施せず、oRootTimerを（時間0で）スタートさせて、そのハンドラ
    'にて、文脈を実行させます。これは、Telegrapherの設計に合わせた設計です。
    'Telegrapherの各メソッドには呼び出す側と呼び出される側が決められており、
    'たとえばProcOnActiveDllXxxxの中からDisconnectを呼び出してはならないことに
    'なっているためです（ProcOnActiveDllXxxxは、通信シーケンスの制御の結果、
    '呼び出されるものですから、中で通信制御を行うためのメソッドではなく、
    '業務処理を行うためのメソッドであると言えます）。
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
    Private oContextTable(255) As Context 'OPT: 本質的に不要（ログ出力とコンテキスト数制限のためにだけに用意）

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

    'NOTE: このプロパティは、親スレッドにおいて参照や書込みが行われる。
    '親スレッドは、このプロパティがRunningでない場合にのみ書込みを行い、
    'Runningに変更する。Telegrapherは、このプロパティがRunningの場合に
    'のみ書込みを行い、Running以外に変更する。
    Public Property Status() As ScenarioStatus
        'NOTE: MyTelegrapher.LineStatusの実装NOTEを参照。
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

    'NOTE: sScenarioFilePathにファイルがない場合などには、IOExceptionをスローします。
    'NOTE: 書式に異常がある場合などには、IOException以外のExceptionをスローします。
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
            Dim isLabelDangling As Boolean = False  '本文なしのラベルがあるか

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

                        '空行またはコメント行なので何もしない。

                ElseIf oVbCodeBeginningRegx.IsMatch(sLine) Then
                    If oCurProcedure IsNot Nothing OrElse sCurVbCodeName IsNot Nothing OrElse sCurCsCodeName IsNot Nothing Then
                        Throw New FormatException("L" & lineNumber.ToString() & ": VbCode definition is not allowed in other definition blocks.")
                    End If

                    Dim sName As String = sLine.Substring("VbCode".Length + 1).Trim()

                    'OPT: oAssemblyManager Is Nothing の場合は、sNameの長さや文字種チェック不要である（事前に行われているはずである）。
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

                    'OPT: oAssemblyManager Is Nothing の場合は、sNameの長さや文字種チェック不要である（事前に行われているはずである）。
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
                                'NOTE: sの先頭が"@"の場合はここに分岐する想定である。
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
                        'NOTE: 「必須パラメータが１つでありかつ、それをブランクにしてもよい」ステートメントは想定しない。
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
                    'NOTE: この状況では、oContext.ExecSeqは必ずNothingである。
                    'oContext.ExecSeqに電文をセットする時点（待機中だった
                    'oContextにシーケンスを紐づけた時点）でoContext.ExecTimerを
                    '解除するためである。
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

    '能動的単発シーケンスが成功した場合
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

    '能動的単発シーケンスで異常とみなすべきでないリトライオーバーが発生した場合
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

    '能動的単発シーケンスで異常とみなすべきリトライオーバーが発生した場合
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

    '能動的単発シーケンスの最中やキューイングされた能動的単発シーケンスの実施前に通信異常を検出した場合
    Public Function ProcOnActiveOneAnonyError(ByVal oReqTeleg As EkReqTelegram) As Boolean
        'NOTE: このメソッドは、ExecStatementOfDisconnect()から
        '呼び出されることがあり得るはず。
        'ただし、そのコンテキストはDisconnectステートメントを
        '実行中であるゆえ、以下で oReqTeleg Is oContext.ExecSeq
        'となるoContextとは別のコンテキストである。
        'NOTE: このメソッドは、ProcOnPassiveOneReqTelegramReceive()
        'におけるDisconnect()から呼び出されることもあり得るように
        'みえるかもしれないが、問題はない。
        'まず、そのPassiveOneと紐づいているコンテキストは
        '実行開始していない（新規の）コンテキストであるか
        'WaitForPassiveOne系ステートメントを実行中のコンテキスト
        'であるゆえ、以下で oReqTeleg Is oContext.ExecSeqとなる
        'oContextとは別のコンテキストである。

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

    '能動的ULLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
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
        'NOTE: このメソッドは、ExecStatementOfDisconnect()から
        '呼び出されることがあり得るはず。
        'ただし、そのコンテキストはDisconnectステートメントを
        '実行中であるゆえ、以下で oReqTeleg Is oContext.ExecSeq
        'となるoContextとは別のコンテキストである。
        'NOTE: このメソッドは、ProcOnPassiveOneReqTelegramReceive()
        'におけるDisconnect()から呼び出されることもあり得るように
        'みえるかもしれないが、問題はない。
        'まず、そのPassiveOneと紐づいているコンテキストは
        '実行開始していない（新規の）コンテキストであるか
        'WaitForPassiveOne系ステートメントを実行中のコンテキスト
        'であるゆえ、以下で oReqTeleg Is oContext.ExecSeqとなる
        'oContextとは別のコンテキストである。

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
                            'NOTE: 特に必要性はないが、Disconnectはせずに、
                            'シナリオで捕捉しなかった体で動作を継続させる。
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
            'NOTE: 特に必要性はないが、Disconnectはせずに、
            'シナリオで捕捉しなかった体で動作を継続させる。
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
            'NOTE: 特に必要性はないが、Disconnectはせずに、
            'シナリオで捕捉しなかった体で動作を継続させる。
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
                                'NOTE: 別のプロセスが排他的に（読み取り禁止で）sReplyTelegPathのファイルを
                                '開いでいる場合とみなす。
                                If retryCount >= 3 Then
                                    nakCause = EkNakCauseCode.Busy
                                    Exit While
                                End If
                                Thread.Sleep(1000)
                                retryCount += 1
                            Else
                                'exがDirectoryNotFoundExceptionやFileNotFoundExceptionの場合である。
                                'NOTE: 先のFile.ExistsからNew FileStreamまでの間に
                                'ファイルが移動や削除されたケースとみなす。
                                'TODO: シナリオ異常終了の方がよいかもしれない。
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
                'NOTE: oTelegImporterに渡したバイト列が電文としての最低限の条件を
                '満たしていなかった場合である。このケースでは、oTelegImporterの
                'メソッドの中でエラーログを出力済みである。
                Log.Error("The scenario aborted.")
                Status = ScenarioStatus.Aborted
                Terminate()
                'NOTE: 特に必要性はないが、Disconnectはせずに、
                'シナリオで捕捉しなかった体で動作を継続させる。
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

        'OPT: このメソッドの中でなら、以下のかわりに
        'ExecuteReadyContexts()でも許容できる。
        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())
        Return True
    End Function

    '受動的DLLの準備（予告されたファイルの受け入れ確認）を行うメソッド
    Public Function PrepareToStartPassiveDll(ByVal oXllReqTeleg As EkMasProDllReqTelegram) As NakCauseCode
        'NOTE: WaitForPassiveDllToNakの行を完了することになった場合は、
        'コンテキストのExecPosを進めて、oRootTimerを開始させなければならない。
        'また、RegPassiveDllProcToNakで登録したハンドラが電文を捕捉した場合は、
        '新しいコンテキストを作って、oRootTimerを開始させなければならない。
        'それらは、NAK電文のソケットへの書込みが成功した後に
        'ProcOnReqTelegramReceiveCompleteBySendNak()で行う
        '（NAK電文のソケットへの書込みが失敗したケースでは、
        'シナリオに紐づいているシーケンスで通信異常が発生した
        'ケースの一種と考える）という仕様でもよいかもしれないが、
        'この場で行うことにする。理由は以下のとおりである。
        '・NAKを返信するべき電文が届いたということ全般を知りたい
        '  シナリオがあるはず。
        '・NAKを返信するべき電文を受信した後に、同じシーケンス内で
        '  通信異常が発生したケースは、テストをする上で分かるように
        '  したいかもしれないが、DisconnectProcを登録しておけば
        '  分かるはずである。
        '・逆に、NAK電文のソケットへの書込みが成功したからといって、
        '  運管サーバまで届くことが保証されるわけではないので、
        '  そのケースでのみ後続の処理や登録したProcの処理が実行される
        '  としても、特にありがたみがない。
        '・この場でoRootTimerをスタートさせても、それによる後続行の
        '  実行は、このメソッドの指定したNAK事由コードのNAK電文を
        '  ソケットに書き込んだ後になってくれる。

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
                            'NOTE: 特に必要性はないが、Disconnectはせずに、
                            'シナリオで捕捉しなかった体で動作を継続させる。
                            Return Nothing
                        End If
                        oContext = New Context(num)
                        oContextTable(num) = oContext
                        Log.Info("ScenarioContext(" & num.ToString() & ") spawned.")

                        'NOTE: oContextTableのnumを占有しているoContextについて、この時点では
                        'oContextsに登録することになるとは限らないが、いずれは必ず登録する
                        '（もしくはシナリオのTerminateでoContextTableの初期化を行う）。
                        'このメソッド内で登録もシナリオのTerminateもしないなら、このメソッド内で
                        'oHandler.SpawnedContextにセットしておき、この先で呼び出される
                        'いずれかのPassiveDll用メソッドで、登録を行うかシナリオのTerminateを行う。
                        Exit For
                    End If
                Next oKeyValue
            End If

            'シナリオに関係のない電文の場合は、それが分かる値を返却する（呼び元が準備を行う）。
            'NOTE: 間違いにみえるかもしれないが、EkNakCauseCodeはEnumではなくClassであり、
            'これは正しい（EkNakCauseCode.Noneとは違う値である）。
            If oSt Is Nothing Then Return Nothing

            'OPT: sTelegFilePathのファイル作成は必要最小限のケースにおいてのみ行いたい。
            'シナリオ側が必要に応じてOutBinFilePathとして記述する（不要ならブランクとする）など。
            Dim needsExpand As Boolean = False
            If oNode IsNot Nothing Then
                'NOTE: oSt.VerbがWaitForPassiveDllの場合もWaitForPassiveDllToNakの場合も
                'シーケンスを捕捉して以降に展開する可能性のあるパラメータは
                '要素3以降のパラメータである。
                For iParam As Integer = 3 To oSt.Params.Length - 1
                    If Not oSt.Params(iParam).IsExpanded Then
                        needsExpand = True
                        Exit For
                    End If
                Next iParam
            Else
                'NOTE: oSt.VerbがRegPassiveDllProcの場合もRegPassiveDllProcToNakの場合も
                'シーケンスを捕捉して以降に展開する可能性のあるパラメータは
                '要素4以降のパラメータである。
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
                        'NOTE: oXllReqTelegの受信によりWaitForPassiveDllの行が完了する場合である。
                        '※この時点で完了するわけではないが、その行にoXllReqTelegのシーケンスが紐づく。
                        oContext.ExecSeq = oXllReqTeleg

                        'NOTE: 事前にチェックしてあるため、iXllReqTeleg.DataFileName等はパスとして無害である。
                        Log.Info("Starting PassiveDll of the files [" & Path.GetFileName(oXllReqTeleg.DataFileName) & "] [" & Path.GetFileName(oXllReqTeleg.ListFileName) & "]...")
                        Return NakCauseCode.None
                    Else
                        oContext.ExecPos = DirectCast(EvaluateParam(oSt, 7, oContext), Integer)
                        oPassiveDllWaitingContexts.Remove(oNode)
                        oReadyContexts.AddLast(oNode)
                        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                        'NOTE: 以下で返却する値がEkNakCauseCode.Noneということはあり得ない。
                        Return DirectCast(oNakCauseParam, EkNakCauseCode)
                    End If
                Else 'oSt.Verb = StatementVerb.WaitForPassiveDllToNak
                    oContext.ExecPos += 1
                    oPassiveDllWaitingContexts.Remove(oNode)
                    oReadyContexts.AddLast(oNode)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                    'NOTE: 以下で返却する値がEkNakCauseCode.Noneということはあり得ない。
                    Return DirectCast(EvaluateParam(oSt, 3, oContext), EkNakCauseCode)
                End If
            Else
                If oSt.Verb = StatementVerb.RegPassiveDllProc Then
                    Dim oNakCauseParam As Object = EvaluateParam(oSt, 4, oContext)
                    If oNakCauseParam Is Nothing Then
                        'NOTE: oXllReqTelegの受信によりRegPassiveDllProcで登録していた処理が開始する場合である。
                        '※この時点で処理が開始するわけではないが、そのためのコンテキストは作成する。
                        'NOTE: この時点ではまだ、oContext.ExecProcedureは決まらない。
                        oHandler.BindSeq = oXllReqTeleg
                        oHandler.SpawnedContext = oContext

                        'NOTE: 事前にチェックしてあるため、iXllReqTeleg.DataFileName等はパスとして無害である。
                        Log.Info("Starting PassiveDll of the files [" & Path.GetFileName(oXllReqTeleg.DataFileName) & "] [" & Path.GetFileName(oXllReqTeleg.ListFileName) & "]...")
                        Return NakCauseCode.None
                    Else
                        oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 7, oContext), Procedure)
                        If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveDll handler must be a proc with no params.")

                        oContexts.AddLast(oContext)
                        oReadyContexts.AddLast(oContext)
                        RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                        'NOTE: 以下で返却する値がEkNakCauseCode.Noneということはあり得ない。
                        Return DirectCast(oNakCauseParam, EkNakCauseCode)
                    End If
                Else 'oSt.Verb = StatementVerb.RegPassiveDllProcToNak
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 5, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveDll handler must be a proc with no params.")

                    oContexts.AddLast(oContext)
                    oReadyContexts.AddLast(oContext)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                    'NOTE: 以下で返却する値がEkNakCauseCode.Noneということはあり得ない。
                    Return DirectCast(EvaluateParam(oSt, 4, oContext), EkNakCauseCode)
                End If
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: 特に必要性はないが、Disconnectはせずに、
            'シナリオで捕捉しなかった体で動作を継続させる。
            Return Nothing
        End Try
    End Function

    '受動的DLLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
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

                'NOTE: oSt.Verb = StatementVerb.WaitForPassiveDllToNak ということはあり得ない。
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

                    'NOTE: oSt.Verb = StatementVerb.RegPassiveDllProcToNak ということはあり得ない。
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

                'TODO: このメソッドに関して、末尾から２番目の引数（transferLimitTicks）は、
                '定義から除去してしまいたい。どの機器のどのプロセスでも絶対に不要だし、
                '監視盤保持バージョンなどの引数が混入した時点で、他の電文の同名メソッド
                'との一貫性を維持する意味なども無くなっている。
                oRet = oXllReqTeleg.CreateContinuousTelegram( _
                        oDllResult.ContinueCode, _
                        oDllResult.ResultantVersionOfSlot1, _
                        oDllResult.ResultantVersionOfSlot2, _
                        oDllResult.ResultantFlagOfFull, _
                        0, _
                        replyLimit)
            Else
                Dim replyLimit As Integer = DirectCast(EvaluateParam(oSt, iReplyLimitParam, oContext), Integer)

                'TODO: このメソッドに関して、末尾から２番目の引数（transferLimitTicks）は、
                '定義から除去してしまいたい。どの機器のどのプロセスでも絶対に不要だし、
                '監視盤保持バージョンなどの引数が混入した時点で、他の電文の同名メソッド
                'との一貫性を維持する意味なども無くなっている。
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
            'NOTE: 特に必要性はないが、Disconnectはせずに、
            'シナリオで捕捉しなかった体で動作を継続させる。
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
                'NOTE: oSt.Verb = StatementVerb.WaitForPassiveDllToNak ということはあり得ない。
                'NOTE: oContext.ExecTimerの解除は、シーケンスを捕捉した時点で実施済みである。
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
                'NOTE: oHandler.SourceStatement.Verb = StatementVerb.RegPassiveDllProcToNak ということはあり得ない。
                'よって、oHandler.SpawnedContextには、必ずContextの参照がセットされている。
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
                    'NOTE: 特に必要性はないが、Disconnectはせずに、
                    'シナリオで捕捉しなかった体で動作を継続させる。
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
        'NOTE: このメソッドは、ExecStatementOfDisconnect()から
        '呼び出されることがあり得るはず。
        'ただし、そのコンテキストはDisconnectステートメントを
        '実行中であるゆえ、以下で oReqTeleg Is oContext.ExecSeq
        'となるoContextとは別のコンテキストである。
        'NOTE: このメソッドは、ProcOnPassiveOneReqTelegramReceive()
        'におけるDisconnect()から呼び出されることもあり得るように
        'みえるかもしれないが、問題はない。
        'まず、そのPassiveOneと紐づいているコンテキストは
        '実行開始していない（新規の）コンテキストであるか
        'WaitForPassiveOne系ステートメントを実行中のコンテキスト
        'であるゆえ、以下で oReqTeleg Is oContext.ExecSeqとなる
        'oContextとは別のコンテキストである。

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

                    'NOTE: oHandler.BindSeq Is oXllReqTeleg という状況であるため、
                    'oHandler.SpawnedContextには、必ずContextの参照がセットされている。
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
            'NOTE: シナリオで捕捉しなかった体で動作を継続させる。
            Return False
        End Try

        Return False
    End Function

    '受動的ULLの準備（指定されたファイルの用意）を行うメソッド
    Public Function PrepareToStartPassiveUll(ByVal oXllReqTeleg As EkServerDrivenUllReqTelegram) As NakCauseCode
        'NOTE: WaitForPassiveUllToNakの行を完了することになった場合は、
        'コンテキストのExecPosを進めて、oRootTimerを開始させなければならない。
        'また、RegPassiveUllProcToNakで登録したハンドラが電文を捕捉した場合は、
        '新しいコンテキストを作って、oRootTimerを開始させなければならない。
        'それらは、NAK電文のソケットへの書込みが成功した後に
        'ProcOnReqTelegramReceiveCompleteBySendNak()で行う
        '（NAK電文のソケットへの書込みが失敗したケースでは、
        'シナリオに紐づいているシーケンスで通信異常が発生した
        'ケースの一種と考える）という仕様でもよいかもしれないが、
        'この場で行うことにする。理由は以下のとおりである。
        '・NAKを返信するべき電文が届いたということ全般を知りたい
        '  シナリオがあるはず。
        '・NAKを返信するべき電文を受信した後に、同じシーケンス内で
        '  通信異常が発生したケースは、テストをする上で分かるように
        '  したいかもしれないが、DisconnectProcを登録しておけば
        '  分かるはずである。
        '・逆に、NAK電文のソケットへの書込みが成功したからといって、
        '  運管サーバまで届くことが保証されるわけではないので、
        '  そのケースでのみ後続の処理や登録したProcの処理が実行される
        '  としても、特にありがたみがない。
        '・この場でoRootTimerをスタートさせても、それによる後続行の
        '  実行は、このメソッドの指定したNAK事由コードのNAK電文を
        '  ソケットに書き込んだ後になってくれる。

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
                            'NOTE: 特に必要性はないが、Disconnectはせずに、
                            'シナリオで捕捉しなかった体で動作を継続させる。
                            Return Nothing
                        End If
                        oContext = New Context(num)
                        oContextTable(num) = oContext
                        Log.Info("ScenarioContext(" & num.ToString() & ") spawned.")

                        'NOTE: oContextTableのnumを占有しているoContextについて、この時点では
                        'oContextsに登録することになるとは限らないが、いずれは必ず登録する
                        '（もしくはシナリオのTerminateでoContextTableの初期化を行う）。
                        'このメソッド内で登録もシナリオのTerminateもしないなら、このメソッド内で
                        'oHandler.SpawnedContextにセットしておき、この先で呼び出される
                        'いずれかのPassiveUll用メソッドで、登録を行うかシナリオのTerminateを行う。
                        Exit For
                    End If
                Next oKeyValue
            End If

            'シナリオに関係のない電文の場合は、それが分かる値を返却する（呼び元が準備を行う）。
            'NOTE: 間違いにみえるかもしれないが、EkNakCauseCodeはEnumではなくClassであり、
            'これは正しい（EkNakCauseCode.Noneとは違う値である）。
            If oSt Is Nothing Then Return Nothing

            'OPT: sTelegFilePathのファイル作成は必要最小限のケースにおいてのみ行いたい。
            'シナリオ側が必要に応じてOutBinFilePathとして記述する（不要ならブランクとする）など。
            Dim needsExpand As Boolean = False
            If oNode IsNot Nothing Then
                'NOTE: oSt.VerbがWaitForPassiveUllの場合もWaitForPassiveUllToNakの場合も
                'シーケンスを捕捉して以降に展開する可能性のあるパラメータは
                '要素3以降のパラメータである。
                For iParam As Integer = 3 To oSt.Params.Length - 1
                    If Not oSt.Params(iParam).IsExpanded Then
                        needsExpand = True
                        Exit For
                    End If
                Next iParam
            Else
                'NOTE: oSt.VerbがRegPassiveUllProcの場合もRegPassiveUllProcToNakの場合も
                'シーケンスを捕捉して以降に展開する可能性のあるパラメータは
                '要素4以降のパラメータである。
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

                        'NOTE: 以下で返却する値がEkNakCauseCode.Noneということはあり得ない。
                        Return DirectCast(oNakCauseParam, EkNakCauseCode)
                    End If
                Else 'oSt.Verb = StatementVerb.WaitForPassiveUllToNak
                    oContext.ExecPos += 1
                    oPassiveUllWaitingContexts.Remove(oNode)
                    oReadyContexts.AddLast(oNode)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                    'NOTE: 以下で返却する値がEkNakCauseCode.Noneということはあり得ない。
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

                        'NOTE: 以下で返却する値がEkNakCauseCode.Noneということはあり得ない。
                        Return DirectCast(oNakCauseParam, EkNakCauseCode)
                    End If
                Else 'oSt.Verb = StatementVerb.RegPassiveUllProcToNak
                    oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 5, oContext), Procedure)
                    If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveUll handler must be a proc with no params.")

                    oContexts.AddLast(oContext)
                    oReadyContexts.AddLast(oContext)
                    RegisterTimer(oRootTimer, TickTimer.GetSystemTick())

                    'NOTE: 以下で返却する値がEkNakCauseCode.Noneということはあり得ない。
                    Return DirectCast(EvaluateParam(oSt, 4, oContext), EkNakCauseCode)
                End If
            End If

            'NOTE: 事前にチェックしてあるため、oXllReqTeleg.FileNameはパスとして無害である。
            Dim sTransferFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

            Dim sTransferFilePath As String = Path.Combine(sPermittedPath, sTransferFileName)
            If oFilePathParam.GetType() Is GetType(String) Then
                Dim sSrcFilePath As String = DirectCast(oFilePathParam, String)
                Dim nakCause As NakCauseCode = EkNakCauseCode.None
                If sSrcFilePath.Length = 0 Then
                    nakCause = EkNakCauseCode.NoData
                ElseIf Not File.Exists(sSrcFilePath) Then
                    'NOTE: シナリオまたは試験環境の誤り（シナリオに記述されたファイルが存在しない等）に
                    '気づかせやすくするには、oSt.Params(iFilePathParam).Valueに"$Ext"が含まれないケースでは、
                    'ここでAbortScenarioさせた方がよいが、現状のままでもLog.Warnによって気づく可能性が高いし、
                    '現状のままの方が駅務機器ごとにACK返信とNAK(NO DATA)返信を切り替えることが簡単であるし、
                    'そうはしないでおく。
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
                                'NOTE: 別のプロセスが排他的に（読み取り禁止で）sSrcFilePathのファイルを
                                '開いでいる場合とみなす。
                                If retryCount >= 3 Then
                                    nakCause = EkNakCauseCode.Busy
                                    Exit While
                                End If
                                Thread.Sleep(1000)
                                retryCount += 1
                            Else
                                'exがDirectoryNotFoundExceptionやFileNotFoundExceptionの場合である。
                                'NOTE: 先のFile.ExistsからCopyFileIfNeededまでの間に
                                'ファイルが移動や削除されたケースとみなす。
                                nakCause = EkNakCauseCode.NoData
                                Exit While
                            End If
                        End Try
                    End While
                End If

                If nakCause <> EkNakCauseCode.None Then
                    If oNode IsNot Nothing Then
                        'NOTE: NAKの返信とともにWaitForPassiveUllの行を完了させる場合である。
                        '※この時点でoXllReqTelegのシーケンスとともに完了する。
                        oContext.ExecPos = DirectCast(EvaluateParam(oSt, 9, oContext), Integer)
                        oPassiveUllWaitingContexts.Remove(oNode)
                        oReadyContexts.AddLast(oNode)
                    Else
                        oContext.ExecProcedure = DirectCast(EvaluateParam(oSt, 9, oContext), Procedure)
                        If oContext.ExecProcedure.ParamNames.Length <> 0 Then Throw New FormatException("PassiveUll handler must be a proc with no params.")

                        'NOTE: NAKの返信とともにRegPassiveUllProcで登録していた処理を開始させる場合である。
                        '※この時点で新規のコンテキストにて処理が開始する。
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
                'NOTE: oXllReqTelegの受信によりWaitForPassiveUllの行が完了する場合である。
                '※この時点で完了するわけではないが、その行にoXllReqTelegのシーケンスが紐づく。
                oContext.ExecSeq = oXllReqTeleg
            Else
                'NOTE: oXllReqTelegの受信によりRegPassiveUllProcで登録していた処理が開始する場合である。
                '※この時点で処理が開始するわけではないが、そのためのコンテキストは作成する。
                'NOTE: この時点ではまだ、oContext.ExecProcedureは決まらない。
                oHandler.BindSeq = oXllReqTeleg
                oHandler.SpawnedContext = oContext
            End If

            oXllReqTeleg.FileHashValue = DirectCast(oFileHashParam, String)
            oXllReqTeleg.TransferLimitTicks = DirectCast(oTransLimitParam, Integer)

            'NOTE: この後、呼び元がこの判定を覆してNAKを返信することはない。
            Log.Info("Starting PassiveUll of the file [" & sTransferFileName & "]...")
            Return EkNakCauseCode.None
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Log.Error("The scenario aborted.")
            Status = ScenarioStatus.Aborted
            Terminate()
            'NOTE: 特に必要性はないが、Disconnectはせずに、
            'シナリオで捕捉しなかった体で動作を継続させる。
            Return Nothing
        End Try
    End Function

    '受動的ULLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
    Public Function CreatePassiveUllContinuousReqTelegram(ByVal oXllReqTeleg As EkServerDrivenUllReqTelegram, ByVal cc As ContinueCode) As EkServerDrivenUllReqTelegram
        Dim oNode As LinkedListNode(Of Context) = oPassiveUllWaitingContexts.First
        While oNode IsNot Nothing
            Dim oContext As Context = oNode.Value
            If oContext.ExecSeq Is oXllReqTeleg Then
                Dim oSt As ProcStatement = oContext.ExecProcedure.Statements(oContext.ExecPos)

                Dim oTransLimitParam As Object
                Dim oReplyLimitParam As Object
                Try
                    'NOTE: oSt.Verb = StatementVerb.WaitForPassiveUllToNak ということはあり得ない。
                    oTransLimitParam = EvaluateParam(oSt, 6, oContext)
                    oReplyLimitParam = EvaluateParam(oSt, 7, oContext)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    'NOTE: 特に必要性はないが、Disconnectはせずに、
                    'シナリオで捕捉しなかった体で動作を継続させる。
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

                'NOTE: oSt.Verb = StatementVerb.RegPassiveUllProcToNak ということはあり得ない。
                'よって、oHandler.SpawnedContextには、必ずContextの参照がセットされている。
                Dim oContext As Context = oHandler.SpawnedContext

                Dim oTransLimitParam As Object
                Dim oReplyLimitParam As Object
                Try
                    'NOTE: oSt.Verb = StatementVerb.RegPassiveUllProcToNak ということはあり得ない。
                    oTransLimitParam = EvaluateParam(oSt, 7, oContext)
                    oReplyLimitParam = EvaluateParam(oSt, 8, oContext)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    Log.Error("The scenario aborted.")
                    Status = ScenarioStatus.Aborted
                    Terminate()
                    'NOTE: 特に必要性はないが、Disconnectはせずに、
                    'シナリオで捕捉しなかった体で動作を継続させる。
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
                'NOTE: oSt.Verb = StatementVerb.WaitForPassiveUllToNak ということはあり得ない。
                'NOTE: oContext.ExecTimerの解除は、シーケンスを捕捉した時点で実施済みである。
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
                'NOTE: oHandler.SourceStatement.Verb = StatementVerb.RegPassiveUllProcToNak ということはあり得ない。
                'よって、oHandler.SpawnedContextには、必ずContextの参照がセットされている。
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
                    'NOTE: 特に必要性はないが、Disconnectはせずに、
                    'シナリオで捕捉しなかった体で動作を継続させる。
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
        'NOTE: このメソッドは、ExecStatementOfDisconnect()から
        '呼び出されることがあり得るはず。
        'ただし、そのコンテキストはDisconnectステートメントを
        '実行中であるゆえ、以下で oReqTeleg Is oContext.ExecSeq
        'となるoContextとは別のコンテキストである。
        'NOTE: このメソッドは、ProcOnPassiveOneReqTelegramReceive()
        'におけるDisconnect()から呼び出されることもあり得るように
        'みえるかもしれないが、問題はない。
        'まず、そのPassiveOneと紐づいているコンテキストは
        '実行開始していない（新規の）コンテキストであるか
        'WaitForPassiveOne系ステートメントを実行中のコンテキスト
        'であるゆえ、以下で oReqTeleg Is oContext.ExecSeqとなる
        'oContextとは別のコンテキストである。

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

                    'NOTE: oHandler.BindSeq Is oXllReqTeleg という状況であるため、
                    'oHandler.SpawnedContextには、必ずContextの参照がセットされている。
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
            'NOTE: シナリオで捕捉しなかった体で動作を継続させる。
            Return False
        End Try

        Return False
    End Function

    Public Sub ProcOnConnectionDisappear()
        'NOTE: このメソッドは、ExecStatementOfDisconnect()から
        '呼び出されることがあり得るはず。
        'ただし、そのコンテキストはDisconnectステートメントを
        '実行中であるゆえ、以下で処理の対象とするコンテキストとは
        '別のコンテキストである。
        'NOTE: このメソッドは、ProcOnPassiveOneReqTelegramReceive()
        'におけるDisconnect()から呼び出されることもあり得るように
        'みえるかもしれないが、問題はない。
        'まず、そのPassiveOneと紐づいているコンテキストは
        '実行開始していない（新規の）コンテキストであるか
        'WaitForPassiveOne系ステートメントを実行中のコンテキスト
        'であるゆえ、以下で処理の対象とするコンテキストとは
        '別のコンテキストである。なお、Disconnect()の前に
        'Terminate()を実行しているなら、このメソッドが
        '呼ばれる時点では、oDisconnectHandlersは空である。

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
        'NOTE: GoToやConnectのようなステートメントは、このメソッドの中で
        '処理全体を終え、そのまま後続のステートメントを開始することになる。
        'よって、このメソッドでは、呼び出された際に、最初に実行した行番号を
        '記録しておき、同じ行を再度実行することになる場合は、不正なループを
        '検知したものとして、シナリオを異常終了させることにしている。
        'この仕様により、成功するまでConnectを繰り返すようなシナリオは、
        '失敗しても、即座にConnectの行に戻るのではなく、Waitなどの行を
        '実行後に、Connectの行に戻らなければならない。
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

                'Wait系ステートメントを（途中まで）実行した場合は、本メソッドから抜ける。
                'NOTE: ProcOnFooBarメソッドにおいて続きを実行することになる。
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
                'NOTE: 個々のパラメータはoOldFrameのローカル変数を参照して評価しなければならない一方、
                'それを用いて生成する変数（呼び出すProcの仮引数）はoNewFrameに生成しなければならない。
                'それ故に、まとめてExpandすることは不可能である。
                'また、個々のパラメータの展開結果に含まれるセミコロンなどは、引数の区切り文字と
                'みなすべきではないため、置換の必要がある。
                Dim sArg As String = DirectCast(oSt.Params(i + 1).Value, String)
                If Not oSt.Params(i + 1).IsExpanded Then
                    sArg = oEnv.oStringExpander.Expand(sArg, oOldFrame.LocalVariables, oContext.Number).Replace(";", "$[;]").Replace(">", "$[>]").Replace("$", "$[$]")
                End If

                Dim oHolder As New VarHolder()
                Dim sVarName As String = oNextProc.ParamNames(i)
                If sVarName.Chars(0) = "*"c Then
                    'NOTE: 実引数側もリファレンス型変数である場合に特殊な処理（実引数と同じ変数を参照させる）を
                    '行う必要があるため、oStringExpander.Expander.Expandは使用しない。
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
                        'NOTE: sArg.Length = 0 の場合や、辞書にsArgが登録されていない場合である。
                        Throw New FormatException("The param(" & (i + 1).ToString() & ") is not consistent with the param(" & i.ToString() & ") of the proc """ & oNextProc.Name & """." & vbCrLf & _
                                                  "It must be a variable name.")
                    End Try

                    oNewFrame.LocalVariables.Add(sVarName, oHolder)
                Else
                    'NOTE: oEnv.oStringExpander.Expand("$SetVal<" & sVarName & ";" & sParam & ">", oNewFrame.LocalVariables, oContext.Number)
                    '上記のように、oStringExpander.Expandを用いて仮引数を作成してもよいが、
                    '無駄が多くなるため、直接実装する。
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
        'NOTE: 上記Disconnect呼び出しの中からProcOnConnectionDisappearが呼び出され、
        'その中でTerminate()が実行されることがあり得るはず。
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

                    'NOTE: ここは、oContext.IterationTargetsの作成後、
                    '他のContext（oContextと同じスレッドで実行されるが、
                    'oContextが外部イベント待ち状態の間などに実行され得る）
                    'によって、oContext.IterationTargetsの示すファイルが
                    '削除された場合に実行される想定である。
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
                'NOTE: oTelegImporterに渡したバイト列が電文としての最低限の条件を
                '満たしていなかった場合である。このケースでは、oTelegImporterの
                'メソッドの中でエラーログを出力済みである。
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

                    'NOTE: ここは、oContext.IterationTargetsの作成後、
                    '他のContext（oContextと同じスレッドで実行されるが、
                    'oContextが外部イベント待ち状態の間などに実行され得る）
                    'によって、oContext.IterationTargetsの示すファイルが
                    '削除された場合に実行される想定である。
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
                'NOTE: oTelegImporterに渡したバイト列が電文としての最低限の条件を
                '満たしていなかった場合である。このケースでは、oTelegImporterの
                'メソッドの中でエラーログを出力済みである。
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

                    'NOTE: ここは、oContext.IterationTargetsの作成後、
                    '他のContext（oContextと同じスレッドで実行されるが、
                    'oContextが外部イベント待ち状態の間などに実行され得る）
                    'によって、oContext.IterationTargetsの示すファイルが
                    '削除された場合に実行される想定である。
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

            'NOTE: パラメータの評価順序は0,3,4,5,6,8である。
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

                    'NOTE: ここは、oContext.IterationTargetsの作成後、
                    '他のContext（oContextと同じスレッドで実行されるが、
                    'oContextが外部イベント待ち状態の間などに実行され得る）
                    'によって、oContext.IterationTargetsの示すファイルが
                    '削除された場合に実行される想定である。
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

            'NOTE: パラメータの評価順序は0,3,4,5,6,8である。
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
        'If sText.Length = 0 Then Return False  'NOTE: これは呼び元でチェックする。
        For i As Integer = 0 To sText.Length - 1
            If Not Char.IsLetterOrDigit(sText, i) Then Return False
        Next i
        Return True
    End Function

    Private Sub Terminate()
        'NOTE: シナリオのデバッグが必要なら、以下を行う前に実施する。

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
''' シナリオ状態。
''' </summary>
Public Enum ScenarioStatus As Integer
    Initial
    Loaded
    Running
    Aborted
    Finished
    Stopped
End Enum
