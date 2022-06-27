' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 駅務機器として運管サーバと電文の送受信を行うクラス。
''' </summary>
Public Class MyTelegrapher
    Inherits ClientTelegrapher

#Region "内部クラス等"
    Delegate Sub RethrowExceptionDelegate(ByVal ex As Exception)
#End Region

#Region "定数や変数"
    'スレッド別ディレクトリ名の書式
    Protected Const sDirNameFormat As String = "%3R%3S_%4C_%2U"

    'メインフォームへの参照
    'NOTE: 依頼された通信の結果をUIに反映する際は、
    'このフォームのBeginInvokeメソッドにより、
    'メッセージループで任意のメソッドを実行させる。
    Protected oForm As MainForm

    '電文書式
    Protected oTelegGene As EkTelegramGene

    'FTPサイトのルートと対になるローカルディレクトリ（作業用）
    Protected sFtpBasePath As String

    'FTPサイト内における号機別ディレクトリのパス
    Protected sPermittedPathInFtp As String

    'FTPサイトの号機別ディレクトリと対になるローカルディレクトリ（作業用）
    Protected sPermittedPath As String

    '送受信履歴ディレクトリ
    Protected sCapDirPath As String

    '自装置の装置コード
    'TODO: ProcOnReqTelegramReceive()をフックして受信電文のClientCodeと比較してもよい。
    Protected selfEkCode As EkCode

    '次に送信するREQ電文の通番
    Protected reqNumberForNextSnd As Integer

    '次に受信するREQ電文の通番
    'TODO: ProcOnReqTelegramReceive()をフックして、受信したREQ電文の通番の
    '連続性等をチェックするなら用意する。
    'Protected reqNumberForNextRcv As Integer

    'ComStartシーケンスに付与する通番（ログ出力用）
    Protected traceNumberForComStart As Integer

    'TimeDataGetシーケンスに付与する通番（ログ出力用）
    Protected traceNumberForTimeDataGet As Integer

    '任意ActiveOneシーケンスに付与する通番（ログ出力用）
    Protected traceNumberForActiveOne As Integer

    '電文の交換を継続すべきか否か
    Protected enableCommunication As Boolean

    'NOTE: 「意図的な切断」と「異常による切断」を区別したいならば、
    'Protected needConnection As Booleanを用意し、
    'ProcOnConnectNoticeReceive()とProcOnDisconnectRequestReceive()をフックして
    'それをON/OFFするとよい。ProcOnConnectionDisappear()では、それをみて、
    '遷移先の回線状態を決めることができる。

    '回線状態
    Private _LineStatus As Integer

    'ウォッチドッグのデータ種別
    Private Shared ReadOnly ObjCodeForWatchdogIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkWatchdogReqTelegram.FormalObjCodeInTokatsu}, _
       {EkAplProtocol.Kanshiban, EkWatchdogReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkWatchdogReqTelegram.FormalObjCodeInMadosho}, _
       {EkAplProtocol.Kanshiban2, EkWatchdogReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkWatchdogReqTelegram.FormalObjCodeInMadosho}}

    '接続初期化のデータ種別
    Private Shared ReadOnly ObjCodeForComStartIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Kanshiban, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkComStartReqTelegram.FormalObjCodeInMadosho}, _
       {EkAplProtocol.Kanshiban2, EkComStartReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkComStartReqTelegram.FormalObjCodeInMadosho}}

    '整時データ取得のデータ種別
    Private Shared ReadOnly ObjCodeForTimeDataGetIn As New Dictionary(Of EkAplProtocol, Byte) From { _
       {EkAplProtocol.Tokatsu, EkTimeDataGetReqTelegram.FormalObjCodeInTokatsu}, _
       {EkAplProtocol.Kanshiban, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Kanshiban2, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}, _
       {EkAplProtocol.Madosho2, EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban}}
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal oTelegGene As EkTelegramGene, ByVal sFtpBasePath As String, ByVal sCapDirPath As String, ByVal oForm As MainForm)
        MyBase.New(sThreadName, oParentMessageSock, New EkTelegramImporter(oTelegGene))
        Me.oTelegGene = oTelegGene
        Me.sFtpBasePath = sFtpBasePath
        Me.sCapDirPath = sCapDirPath
        Me.oForm = oForm
        Me.reqNumberForNextSnd = 0
        Me.traceNumberForTimeDataGet = 0
        Me.enableCommunication = False
        Me.LineStatus = LineStatus.Initial

        Me.selfEkCode = Config.SelfEkCode
        Me.oWatchdogTimer.Renew(Config.WatchdogIntervalLimitTicks)
        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite
        Me.enableWatchdog = Config.EnableWatchdog
        Me.enableXllStrongExclusion = Config.EnableXllStrongExclusion
        Me.enableActiveSeqStrongExclusion = Config.EnableActiveSeqStrongExclusion
        Me.enableActiveOneOrdering = Config.EnableActiveOneOrdering

        Dim oActiveChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(Me.oActiveXllWorkerMessageSock, oActiveChildSock)
        Me.oActiveXllWorker = New FtpWorker( _
           sThreadName & "-ActiveXll", _
           oActiveChildSock, _
           Config.FtpServerUri, _
           New NetworkCredential(Config.FtpUserName, Config.FtpPassword), _
           Config.ActiveFtpRequestLimitTicks, _
           Config.ActiveFtpLogoutLimitTicks, _
           Config.ActiveFtpTransferStallLimitTicks, _
           Config.ActiveFtpUsePassiveMode, _
           Config.ActiveFtpLogoutEachTime, _
           Config.ActiveFtpBufferLength)
        Me.activeXllWorkerPendingLimitTicks = Config.ActiveFtpWorkerPendingLimitTicks

        Dim oPassiveChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(Me.oPassiveXllWorkerMessageSock, oPassiveChildSock)
        Me.oPassiveXllWorker = New FtpWorker( _
           sThreadName & "-PassiveXll", _
           oPassiveChildSock, _
           Config.FtpServerUri, _
           New NetworkCredential(Config.FtpUserName, Config.FtpPassword), _
           Config.PassiveFtpRequestLimitTicks, _
           Config.PassiveFtpLogoutLimitTicks, _
           Config.PassiveFtpTransferStallLimitTicks, _
           Config.PassiveFtpUsePassiveMode, _
           Config.PassiveFtpLogoutEachTime, _
           Config.PassiveFtpBufferLength)
        Me.passiveXllWorkerPendingLimitTicks = Config.PassiveFtpWorkerPendingLimitTicks

        Me.sPermittedPathInFtp = Path.Combine(Config.ModelPathInFtp, selfEkCode.ToString(sDirNameFormat))
        Me.sPermittedPath = Utility.CombinePathWithVirtualPath(sFtpBasePath, sPermittedPathInFtp)
    End Sub
#End Region

#Region "プロパティ"
    'NOTE: このプロパティは、親スレッドにおいても参照や変更が行われる。
    'InitialかDisconnectedの場合は、親スレッドに変更の権利があり、
    '親スレッドはConnectedに変更し得る。
    'ConnectedかSteadyの場合は、子スレッドに変更の権利があり、
    '子スレッドはSteadyかDisconnectedに変更し得る。
    Public Property LineStatus() As LineStatus
        'NOTE: InterlockedクラスのReadメソッドに関するmsdnの解説を読むと、
        '32ビット変数からの値の読み取りはInterlockedクラスのメソッドを使う
        'までもなく不可分である（全体を読み取るためのバスオペレーションが、
        '他のコアによるバスオペレーションに分断されることがない）ことが
        '保証されているようにも見え、実際にIntegerを引数とするReadメソッドは
        '用意されていない。ここでは、とりあえずInterlocked.Add（LOCK: XADD?）
        'を代用しているが、一般的に考えて、Interlockedクラスに
        '「Readメモリバリア+単独の32bitロード命令」で実装された（実質的な
        'VolatileRead相当の）Readメソッドが用意されるべきであり、もし、
        'それが用意されたら、それに変更した方がよい。なお、VolatileReadを
        '使用しないのは、ServerTelegrapherで決めた方針である。方針の詳細は
        'ServerTelegrapher.LastPulseTickのコメントを参照。
        Get
            Return DirectCast(Interlocked.Add(_LineStatus, 0), LineStatus)
        End Get

        Set(ByVal status As LineStatus)
            Interlocked.Exchange(_LineStatus, status)
        End Set
    End Property
#End Region

#Region "イベント処理メソッド"
    Protected Overrides Sub ProcOnUnhandledException(ByVal ex As Exception)
        MyBase.ProcOnUnhandledException(ex)

        oForm.BeginInvoke( _
           New RethrowExceptionDelegate(AddressOf oForm.RethrowException), _
           New Object() {ex})
    End Sub

    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case MyInternalMessageKind.ComStartExecRequest
                Return ProcOnComStartExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.TimeDataGetExecRequest
                Return ProcOnTimeDataGetExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ActiveOneExecRequest
                Return ProcOnActiveOneExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ActiveUllExecRequest
                Return ProcOnActiveUllExecRequestReceive(oRcvMsg)
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
    End Function

    Protected Overrides Function ProcOnTelegramReceive(ByVal oRcvTeleg As ITelegram) As Boolean
        Dim capRcvTelegs As Boolean
        SyncLock oForm.UiState
            capRcvTelegs = oForm.UiState.CapRcvTelegs
        End SyncLock

        If capRcvTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    DirectCast(oRcvTeleg, EkTelegram).WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return MyBase.ProcOnTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnComStartExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ComStartExec requested by manager.")

        Dim sSeqName As String = "ComStart #" & traceNumberForComStart.ToString()
        UpdateTraceNumberForComStart()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New EkComStartReqTelegram( _
           oTelegGene, _
           ObjCodeForComStartIn(Config.AplProtocol),
           Config.ComStartReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        Return True
    End Function

    Protected Overridable Function ProcOnTimeDataGetExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("TimeDataGetExec requested by manager.")

        Dim sSeqName As String = "TimeDataGet #" & traceNumberForTimeDataGet.ToString()
        UpdateTraceNumberForTimeDataGet()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New EkTimeDataGetReqTelegram( _
           oTelegGene, _
           ObjCodeForTimeDataGetIn(Config.AplProtocol),
           Config.TimeDataGetReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ActiveOneExec requested by manager.")

        Dim oExt As ActiveOneExecRequestExtendPart _
           = ActiveOneExecRequest.Parse(oRcvMsg).ExtendPart

        Dim oTeleg As EkDodgyTelegram
        Try
            Dim oImporter As New EkTelegramImporter(oTelegGene)
            Using oInputStream As New FileStream(oExt.ApplyFilePath, FileMode.Open, FileAccess.Read)
                oTeleg = oImporter.GetTelegramFromStream(oInputStream)
            End Using
            If oTeleg Is Nothing Then Return True
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return True
        End Try

        Dim sSeqName As String = "ActiveOne #" & traceNumberForActiveOne.ToString()
        UpdateTraceNumberForActiveOne()

        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New EkAnonyReqTelegram(oTeleg, oExt.ReplyLimitTicks)
        RegisterActiveOne(oReqTeleg, oExt.RetryIntervalTicks, oExt.MaxRetryCountToForget + 1, oExt.MaxRetryCountToCare + 1, sSeqName)
        Return True
    End Function

    '能動的単発シーケンスが成功した場合
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Info("ComStart completed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case rtt Is GetType(EkTimeDataGetReqTelegram)
                Log.Info("TimeDataGet completed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case Else
                Log.Info("ActiveOne completed.")
        End Select
    End Sub

    '能動的単発シーケンスで異常とみなすべきでないリトライオーバーが発生した場合
    'NOTE: 本クラスのGetRequirement()の実装上、このメソッドが呼ばれることはあり得ない。
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart skipped.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case rtt Is GetType(EkTimeDataGetReqTelegram)
                Log.Error("TimeDataGet skipped.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case Else
                Log.Error("ActiveOne skipped.")
        End Select
    End Sub

    '能動的単発シーケンスで異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart failed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering
                Dim automaticComStart As Boolean
                SyncLock oForm.UiState
                    automaticComStart = oForm.UiState.AutomaticComStart
                End SyncLock
                If automaticComStart AndAlso LineStatus <> LineStatus.Steady Then
                   '開始のための接続初期化シーケンスでリトライオーバーが発生した場合である。
                    enableCommunication = False
                End If

            Case rtt Is GetType(EkTimeDataGetReqTelegram)
                Log.Error("TimeDataGet failed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering
                Dim automaticComStart As Boolean
                SyncLock oForm.UiState
                    automaticComStart = oForm.UiState.AutomaticComStart
                End SyncLock
                If automaticComStart AndAlso LineStatus <> LineStatus.Steady Then
                   '開始のための整時シーケンスでリトライオーバーが発生した場合である。
                    enableCommunication = False
                End If

            Case Else
                Log.Error("ActiveOne retry over.")
                'NOTE: シミュレータゆえに、切断は自動的に行われない方が
                'よいため、enableCommunicationはTrueのままにしておく。
                '上で「enableCommunication = False」を行っているのは、
                '接続とセットで自動的に行ったシーケンスが失敗した
                'ケースのためである。
        End Select
    End Sub

    '能動的単発シーケンスの最中やキューイングされた能動的単発シーケンスの実施前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart failed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case rtt Is GetType(EkTimeDataGetReqTelegram)
                Log.Error("TimeDataGet failed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case Else
                Log.Error("ActiveOne failed.")
        End Select
    End Sub

    Protected Overridable Function ProcOnActiveUllExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ActiveUllExec requested by manager.")

        Dim oExt As ActiveUllExecRequestExtendPart _
           = ActiveUllExecRequest.Parse(oRcvMsg).ExtendPart

        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram
        Try
            Dim sTransferFileName As String = Path.GetFileName(oExt.TransferFilePath)
            File.Copy(oExt.TransferFilePath, Path.Combine(sPermittedPath, sTransferFileName), True)

            oXllReqTeleg = New EkClientDrivenUllReqTelegram( _
               oTelegGene, _
               oExt.ObjCode, _
               ContinueCode.Start, _
               Path.Combine(sPermittedPathInFtp, sTransferFileName), _
               oExt.TransferFileHashValue, _
               oExt.TransferLimitTicks, _
               oExt.ReplyLimitTicks)

        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return True
        End Try

        RegisterActiveUll(oXllReqTeleg, oExt.RetryIntervalTicks, oExt.MaxRetryCountToForget + 1, oExt.MaxRetryCountToCare + 1)
        Return True
    End Function

    '能動的ULLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
    Protected Overrides Function CreateActiveUllContinuousReqTelegram(ByVal oXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)
        'TODO: 最後の引数は、別の設定値を参照したい。
        Return oRealUllReqTeleg.CreateContinuousTelegram(cc, 0, oRealUllReqTeleg.ReplyLimitTicks)
    End Function

    '能動的ULLが成功した（転送終了REQ電文に対してACK電文を受信した）場合
    Protected Overrides Sub ProcOnActiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Log.Info("ActiveUll completed.")
    End Sub

    '能動的ULLにて転送終了REQ電文に対してNAK電文を受信した場合
    Protected Overrides Sub ProcOnActiveUllFinalizeError(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Error("ActiveUll failed by finalize error.")
    End Sub

    '能動的ULLにて転送が失敗した（ContinueCode.Abortの転送終了REQ電文を送信することになる）場合
    Protected Overrides Sub ProcOnActiveUllTransferError(ByVal oXllReqTeleg As IXllReqTelegram)
        Log.Error("ActiveUll failed by transfer error.")
    End Sub

    '能動的ULLの開始で異常とみなすべきでないリトライオーバーが発生した場合
    'NOTE: 本クラスのGetRequirement()の実装上、このメソッドが呼ばれることはあり得ない。
    Protected Overrides Sub ProcOnActiveUllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Fatal("ActiveUll failed by surprising retry over.")
    End Sub

    '能動的ULLの開始で異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveUllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Error("ActiveUll failed by retry over.")
    End Sub

    '能動的ULLの最中やキューイングされた能動的ULLの開始前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
        Log.Error("ActiveUll failed.")
    End Sub

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)
        Select Case oRcvTeleg.SubCmdCode
            Case EkSubCmdCode.Get
                Dim isObjCodeRegistered As Boolean = False
                Dim sApplyFilePath As String = Nothing
                SyncLock oForm.UiState
                    If oForm.UiState.ApplyFileForPassiveGetObjCodes.ContainsKey(CByte(oRcvTeleg.ObjCode)) Then
                        isObjCodeRegistered = True
                        sApplyFilePath = oForm.UiState.ApplyFileForPassiveGetObjCodes(CByte(oRcvTeleg.ObjCode))
                    End If
                End SyncLock
                If isObjCodeRegistered Then
                    Return ProcOnRegisteredGetReqTelegramReceive(oRcvTeleg, sApplyFilePath)
                End If

            Case EkSubCmdCode.Post
                Dim isObjCodeRegistered As Boolean = False
                SyncLock oForm.UiState
                    isObjCodeRegistered = oForm.UiState.SomethingForPassivePostObjCodes.ContainsKey(CByte(oRcvTeleg.ObjCode))
                End SyncLock
                If isObjCodeRegistered Then
                    Return ProcOnRegisteredPostReqTelegramReceive(oRcvTeleg)
                End If
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnRegisteredGetReqTelegramReceive(ByVal iRcvTeleg As ITelegram, ByVal sApplyFilePath As String) As Boolean
        Dim oRcvTeleg As New EkByteArrayGetReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("ByteArrayGet REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("ByteArrayGet REQ received.")

        Dim forceNakCause As NakCauseCode = EkNakCauseCode.None
        SyncLock oForm.UiState
            If oForm.UiState.ForceReplyNakToPassiveGetReq Then
                forceNakCause = New EkNakCauseCode(oForm.UiState.NakCauseNumberToPassiveGetReq, oForm.UiState.NakCauseTextToPassiveGetReq)
            End If
        End SyncLock
        If forceNakCause <> EkNakCauseCode.None Then
            If SendNakTelegram(forceNakCause, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        If sApplyFilePath Is Nothing OrElse _
           Not File.Exists(sApplyFilePath) Then
            Log.Warn("No data exists to reply.")
            If SendNakTelegram(EkNakCauseCode.NoData, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        Log.Info("Reading reply data from [" & sApplyFilePath & "]...")
        Dim aReplyBytes As Byte()
        Try
            Using oInputStream As New FileStream(sApplyFilePath, FileMode.Open, FileAccess.Read)
                'ファイルのレングスを取得する。
                Dim len As Integer = CInt(oInputStream.Length)
                'ファイルを読み込む。
                aReplyBytes = New Byte(len - 1) {}
                Dim pos As Integer = 0
                Do
                    Dim readSize As Integer = oInputStream.Read(aReplyBytes, pos, len - pos)
                    If readSize = 0 Then Exit Do
                    pos += readSize
                Loop
            End Using
        Catch ex As Exception
            Log.Error("Unwelcome Exception caught.", ex)
            SendNakTelegramThenDisconnect(EkNakCauseCode.Busy, oRcvTeleg) 'TODO: 微妙
            Return True
        End Try

        Dim oReplyTeleg As EkByteArrayGetAckTelegram = oRcvTeleg.CreateAckTelegram(aReplyBytes)
        Log.Info("Sending ByteArrayGet ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnRegisteredPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkByteArrayPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("ByteArrayPost REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("ByteArrayPost REQ received.")

        Dim forceNakCause As NakCauseCode = EkNakCauseCode.None
        SyncLock oForm.UiState
            If oForm.UiState.ForceReplyNakToPassivePostReq Then
                forceNakCause = New EkNakCauseCode(oForm.UiState.NakCauseNumberToPassivePostReq, oForm.UiState.NakCauseTextToPassivePostReq)
            End If
        End SyncLock
        If forceNakCause <> EkNakCauseCode.None Then
            If SendNakTelegram(forceNakCause, oRcvTeleg) = False Then
                Disconnect()
            End If
            Return True
        End If

        Dim oReplyTeleg As EkByteArrayPostAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ByteArrayPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    'ヘッダ部の内容が受動的DLLのREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsPassiveDllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get Then
            SyncLock oForm.UiState
                Return oForm.UiState.SomethingForPassiveDllObjCodes.ContainsKey(CByte(oTeleg.ObjCode))
            End SyncLock
        End If

        Return False
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsPassiveDllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)

        Dim transferLimitTicks As Integer
        SyncLock oForm.UiState
            transferLimitTicks = oForm.UiState.PassiveDllTransferLimitTicks
        End SyncLock

        'TODO: 現在のプロトコルにおける受動的DLLシーケンスのREQ電文は
        'データ種別に関係なくEkMasProDllReqTelegramであるが、そうでなくなった
        '場合のことを想定するなら、oForm.UiState.SomethingForPassiveDllObjCode
        'には、電文の型を格納しておいた方がよいかもしれない。
        Return New EkMasProDllReqTelegram(oTeleg, transferLimitTicks)
    End Function

    '受動的DLLの準備（予告されたファイルの受け入れ確認）を行うメソッド
    Protected Overrides Function PrepareToStartPassiveDll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        SyncLock oForm.UiState
            If oForm.UiState.ForceReplyNakToPassiveDllStartReq Then
                Return New EkNakCauseCode(oForm.UiState.NakCauseNumberToPassiveDllStartReq, oForm.UiState.NakCauseTextToPassiveDllStartReq)
            End If
        End SyncLock

        'NOTE: 事前にチェックしてあるため、iXllReqTeleg.DataFileName等はパスとして無害である。
        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Log.Info("Starting PassiveDll of the files [" & Path.GetFileName(oXllReqTeleg.DataFileName) & "] [" & Path.GetFileName(oXllReqTeleg.ListFileName) & "]...")
        Return EkNakCauseCode.None
    End Function

    '受動的DLLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
    Protected Overrides Function CreatePassiveDllContinuousReqTelegram(ByVal iXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
                Dim replyLimitTicks As Integer
                Dim resultantVersionOfSlot1 As Integer
                Dim resultantVersionOfSlot2 As Integer
                Dim resultantFlagOfFull As Integer
                SyncLock oForm.UiState
                    replyLimitTicks = oForm.UiState.PassiveDllFinishReplyLimitTicks
                    resultantVersionOfSlot1 = oForm.UiState.PassiveDllResultantVersionOfSlot1
                    resultantVersionOfSlot2 = oForm.UiState.PassiveDllResultantVersionOfSlot2
                    resultantFlagOfFull = oForm.UiState.PassiveDllResultantFlagOfFull
                End SyncLock
                Return oXllReqTeleg.CreateContinuousTelegram(cc, resultantVersionOfSlot1, resultantVersionOfSlot2, resultantFlagOfFull, 0, replyLimitTicks)

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select

        Return Nothing
    End Function

    '受動的DLLが成功した（受信済みのハッシュ値と受信完了したファイルの内容が整合していることを確認した）
    '（ContinueCode.Finishの転送終了REQ電文を送信することになる）場合
    Protected Overrides Function ProcOnPassiveDllComplete(ByVal iXllReqTeleg As IXllReqTelegram) As Boolean
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Log.Info("PassiveDll completed.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select

        SyncLock oForm.UiState
            Return oForm.UiState.SimulateStoringOnPassiveDllComplete
        End SyncLock
    End Function

    '受動的DLLにて転送化けを検出した（受信済みのハッシュ値と受信完了したファイルの内容に不整合を検出した）
    '（ContinueCode.Abortの転送終了REQ電文を送信することになる）場合
    Protected Overrides Sub ProcOnPassiveDllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Log.Error("PassiveDll failed by hash value error.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '受動的DLLにて転送が失敗した（ContinueCode.Abortの転送終了REQ電文を送信することになる）場合
    Protected Overrides Sub ProcOnPassiveDllTransferError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Log.Error("PassiveDll failed by transfer error.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '受動的DLLの最中やキューイングされた受動的DLLの開始前に通信異常を検出した場合
    Protected Overrides Sub ProcOnPassiveDllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkMasProDllReqTelegram)
                Log.Error("PassiveDll failed.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    'ヘッダ部の内容が受動的ULLのREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsPassiveUllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get Then
            SyncLock oForm.UiState
                Return oForm.UiState.ApplyFileForPassiveUllObjCodes.ContainsKey(CByte(oTeleg.ObjCode))
            End SyncLock
        End If

        Return False
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsPassiveUllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)

        Dim transferLimitTicks As Integer
        SyncLock oForm.UiState
            transferLimitTicks = oForm.UiState.PassiveUllTransferLimitTicks
        End SyncLock

        'TODO: 現在のプロトコルにおける受動的ULLシーケンスのREQ電文は
        'データ種別に関係なくEkServerDrivenUllReqTelegramであるが、そうでなくなった
        '場合のことを想定するなら、oForm.UiState.TypeForPassiveUllObjCodeのような
        'ところに、電文の型を格納しておいた方がよいかもしれない。
        Return New EkServerDrivenUllReqTelegram(oTeleg, transferLimitTicks)
    End Function

    '受動的ULLの準備（指定されたファイルの用意）を行うメソッド
    Protected Overrides Function PrepareToStartPassiveUll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
                SyncLock oForm.UiState
                    If oForm.UiState.ForceReplyNakToPassiveUllStartReq Then
                        Return New EkNakCauseCode(oForm.UiState.NakCauseNumberToPassiveUllStartReq, oForm.UiState.NakCauseTextToPassiveUllStartReq)
                    End If
                End SyncLock

                Dim isObjCodeRegistered As Boolean = False
                Dim sApplyFilePath As String = Nothing
                SyncLock oForm.UiState
                    'NOTE: ロックを解除していた間に変更されている可能性もあるので、
                    'ApplyFileForPassiveUllObjCodesに登録されているか再度チェックを
                    '行うことにしている。
                    If oForm.UiState.ApplyFileForPassiveUllObjCodes.ContainsKey(CByte(oXllReqTeleg.ObjCode)) Then
                        isObjCodeRegistered = True
                        sApplyFilePath = oForm.UiState.ApplyFileForPassiveUllObjCodes(CByte(oXllReqTeleg.ObjCode))
                    End If
                End SyncLock

                If Not isObjCodeRegistered Then
                    Log.Warn("Setting was changed during a sequence.")
                    Return EkNakCauseCode.NoData 'TODO: 微妙
                End If

                If sApplyFilePath Is Nothing OrElse _
                   Not File.Exists(sApplyFilePath) Then
                    Log.Warn("No data exists to reply.")
                    Return EkNakCauseCode.NoData
                End If

                'NOTE: 相手装置の不具合がみつかりやすいよう、oXllReqTeleg.FileNameが
                'ObjCodeと整合していない場合に、警告くらいは出してもよいと思われる。
                'しかし、その警告を頼りに試験をするには、このシミュレータの試験も
                '入念に行うべきであり、本末転倒であるため、やめておく。
                'NOTE: 事前にチェックしてあるため、oXllReqTeleg.FileNameはパスとして無害である。
                Dim sTransferFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
                Try
                    File.Copy(sApplyFilePath, Path.Combine(sPermittedPath, sTransferFileName), True)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                    'NOTE: 先のFile.ExistsからFile.Copyまでの間に
                    'ファイルが移動や削除されたケースとみなす。
                    Return EkNakCauseCode.NoData
                End Try

                Log.Info("Starting PassiveUll of the file [" & sTransferFileName & "]...")
                Return EkNakCauseCode.None

            Case Else
                Debug.Fail("This case is impermissible.")
                Return EkNakCauseCode.NotPermit
        End Select
    End Function

    '受動的ULLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
    Protected Overrides Function CreatePassiveUllContinuousReqTelegram(ByVal iXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
                Dim replyLimitTicks As Integer
                SyncLock oForm.UiState
                    replyLimitTicks = oForm.UiState.PassiveUllFinishReplyLimitTicks
                End SyncLock
                Return oXllReqTeleg.CreateContinuousTelegram(cc, 0, replyLimitTicks)

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select

        Return Nothing
    End Function

    '受動的ULLが成功した（転送終了REQ電文に対してACK電文を受信した）場合
    Protected Overrides Sub ProcOnPassiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iAckTeleg As IXllTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Log.Info("PassiveUll completed.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '受動的ULLにて転送終了REQ電文に対してNAK電文を受信した場合
    Protected Overrides Sub ProcOnPassiveUllFinalizeError(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Log.Error("PassiveUll failed by finalize error.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '受動的ULLにて転送が失敗した（ContinueCode.Abortの転送終了REQ電文を送信することになる）場合
    Protected Overrides Sub ProcOnPassiveUllTransferError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Log.Error("PassiveUll failed by transfer error.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    '受動的ULLの最中やキューイングされた受動的ULLの開始前に通信異常を検出した場合
    Protected Overrides Sub ProcOnPassiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim rtt As Type = iXllReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkServerDrivenUllReqTelegram)
                Log.Error("PassiveUll failed.")

            Case Else
                Debug.Fail("This case is impermissible.")
        End Select
    End Sub

    'ヘッダ部の内容がウォッチドッグREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsWatchdogReq(ByVal iTeleg As ITelegram) As Boolean
        If Config.EnableWatchdog = False Then
            Return False
        End If

        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oTeleg.ObjCode = ObjCodeForWatchdogIn(Config.AplProtocol) Then
            Return True
        Else
            Return False
        End If
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsWatchdogReq(ByVal iTeleg As ITelegram) As IWatchdogReqTelegram
        Return New EkWatchdogReqTelegram(iTeleg)
    End Function

    '親スレッドからコネクションを受け取った場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionAppear()
        reqNumberForNextSnd = 0
        enableCommunication = True

        'FTPで使う一時作業用ディレクトリを初期化する。
        Log.Info("Initializing directory [" & sPermittedPath & "]...")
        Utility.DeleteTemporalDirectory(sPermittedPath)
        Directory.CreateDirectory(sPermittedPath)

        Dim automaticComStart As Boolean
        SyncLock oForm.UiState
            automaticComStart = oForm.UiState.AutomaticComStart
        End SyncLock

        If automaticComStart Then
            enableActiveOneOrdering = True

            If Config.AplProtocol = EkAplProtocol.Tokatsu Then
                Dim sSeqName As String = "TimeDataGet #" & traceNumberForTimeDataGet.ToString()
                UpdateTraceNumberForTimeDataGet()

                Log.Info("Register " & sSeqName & " as ActiveOne.")

                Dim oReqTeleg As New EkTimeDataGetReqTelegram( _
                   oTelegGene, _
                   ObjCodeForTimeDataGetIn(Config.AplProtocol),
                   Config.TimeDataGetReplyLimitTicks)

                RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
            Else
                Dim sSeqName As String = "ComStart #" & traceNumberForComStart.ToString()
                UpdateTraceNumberForComStart()

                Log.Info("Register " & sSeqName & " as ActiveOne.")

                Dim oReqTeleg As New EkComStartReqTelegram( _
                   oTelegGene, _
                   ObjCodeForComStartIn(Config.AplProtocol),
                   Config.ComStartReplyLimitTicks)

                RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
            End If
        End If
    End Sub

    'NOTE: このTelegrapherの親スレッドは、コネクションを作った際にLineStatusを
    '必ずConnectedにしてからTelegrapherにソケットを渡すように実装しているため、
    '下記で定義しているProcOnReqTelegramReceiveCompleteBySendAck～
    'ProcOnConnectionDisappearが呼ばれる際のLineStatusは、ConnectedかSteadyの
    'いずれかである。

    'REQ電文受信及びそれに対するACK電文送信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Function ProcOnReqTelegramReceiveCompleteBySendAck(ByVal iRcvTeleg As ITelegram, ByVal iSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ電文受信及びそれに対する軽度NAK電文（BUSY等）送信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Function ProcOnReqTelegramReceiveCompleteBySendNak(ByVal iRcvTeleg As ITelegram, ByVal iSndTeleg As ITelegram) As Boolean
        Return True
    End Function

    'REQ電文送信及びそれに対するACK電文受信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Function ProcOnReqTelegramSendCompleteByReceiveAck(ByVal iSndTeleg As ITelegram, ByVal iRcvTeleg As ITelegram) As Boolean
        LineStatus = LineStatus.Steady
        Return enableCommunication
    End Function

    'REQ電文送信及びそれに対する軽度NAK電文（BUSY等）受信が完了して通信継続が決定した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Function ProcOnReqTelegramSendCompleteByReceiveNak(ByVal iSndTeleg As ITelegram, ByVal iRcvTeleg As ITelegram) As Boolean
        Return enableCommunication
    End Function

    'コネクションを切断した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionDisappear()
        LineStatus = LineStatus.Disconnected
        enableCommunication = False 'NOTE: ロジック的に無意味だが、見た目の整合性を保つため。
    End Sub

    'NOTE: 以下の4メソッドで行っているFTP送受信ファイルのsCapDirPathへの
    '保存は、ActiveXllWorkerやPassiveXllWorkerをFtpWorkerから派生させた
    'MyFtpWorkerのインスタンスとして用意し、それらが包含するスレッドにて、
    'FTP完了のタイミングで（TelegrapherにResponseを返信する前に）
    '実施する方が、出入口における機械的記録という目的に合致する(*1)。
    'しかし、現状のFtpWorkerは、オーバライド可能なメソッドである
    'ProcOnDownloadRequestReceiveとProcOnUploadRequestReceiveに
    'おいて、「FTPの実施」と「TelegrapherへのResponse返信」を
    '直接実装しているため、MyFtpWorkerでそれらをオーバライドして、
    '「FTPの実施」と「TelegrapherへのResponse返信」の間に
    'カスタムな処理を追加する場合、「FTPの実施」などまで
    '自前で実装する必要が生じてしまう。それは、あまりにも無駄で
    'ある（IXllWorkerをImplementsするクラスを新規に用意するのと
    '大差がない）ため、FtpWorkerをリファクタリング可能な機会がある
    'までは、以下の4メソッドで保存を行うことにした。
    'なお、ActiveXllWorkerやPassiveXllWorkerで保存を行うようにする
    '場合は、それらの2スレッドにおける「存在しないファイル名の検索～
    'ファイルの作成」でレースコンディションが発生しないよう、
    'ActiveXllWorkerで生成するファイルとPassiveXllWorkerで生成する
    'ファイルには、別の命名規則を適用するべき（CapDataPathにおける
    'TransKind部分を別の文字にするべき）である。
    '*1 実際、Telegrapherは、Workerに依頼した仕事がある状態でDisconnect()を
    '行った場合、WorkerにCancelTransfer()させた後、WorkerからのResponseは
    '待つものの、それを以下の4メソッドに渡すことはしない。よって、以下の
    '4メソッドをオーバーライドする方式だと、たとえWorkerにおいて
    'FTPが成功していたとしても、当該ファイルは保存しないこととなる。

    Protected Overrides Function ProcOnActiveDownloadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If DownloadResponse.Parse(oRcvMsg).Result = DownloadResult.Finished Then
            Dim capRcvFiles As Boolean
            SyncLock oForm.UiState
                capRcvFiles = oForm.UiState.CapRcvFiles
            End SyncLock

            If capRcvFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oActiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnActiveDownloadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnActiveUploadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If UploadResponse.Parse(oRcvMsg).Result = UploadResult.Finished Then
            Dim capSndFiles As Boolean
            SyncLock oForm.UiState
                capSndFiles = oForm.UiState.CapSndFiles
            End SyncLock

            If capSndFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oActiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnActiveUploadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnPassiveDownloadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If DownloadResponse.Parse(oRcvMsg).Result = DownloadResult.Finished Then
            Dim capRcvFiles As Boolean
            SyncLock oForm.UiState
                capRcvFiles = oForm.UiState.CapRcvFiles
            End SyncLock

            If capRcvFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oPassiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnPassiveDownloadResponseReceive(oRcvMsg)
    End Function

    Protected Overrides Function ProcOnPassiveUploadResponseReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        If UploadResponse.Parse(oRcvMsg).Result = UploadResult.Finished Then
            Dim capSndFiles As Boolean
            SyncLock oForm.UiState
                capSndFiles = oForm.UiState.CapSndFiles
            End SyncLock

            If capSndFiles Then
                Dim oXllReqTeleg As IXllReqTelegram = oPassiveXllQueue.First.Value.ReqTeleg
                Dim transferList As List(Of String) = oXllReqTeleg.TransferList
                Dim transferListBase As String = oXllReqTeleg.TransferListBase
                Dim lastIndex As Integer = transferList.Count - 1
                For i As Integer = 0 To lastIndex
                    Try
                        Dim sSrcPath As String = Utility.CombinePathWithVirtualPath(transferListBase, transferList(i))
                        Dim sDstPath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "F")
                        File.Copy(sSrcPath, sDstPath)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                Next
            End If
        End If

        Return MyBase.ProcOnPassiveUploadResponseReceive(oRcvMsg)
    End Function
#End Region

#Region "イベント処理実装用メソッド"
    Protected Function SendNakTelegram(ByVal cause As NakCauseCode, ByVal oSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As ITelegram = oSourceTeleg.CreateNakTelegram(cause)
        If oReplyTeleg IsNot Nothing Then
            Log.Info("Sending NAK (" & cause.ToString() & ") telegram...")
            Return SendReplyTelegram(oReplyTeleg, oSourceTeleg)
        Else
            Return False
        End If
    End Function

    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        oReqTeleg.ReqNumber = reqNumberForNextSnd
        oReqTeleg.ClientCode = selfEkCode
        Dim ret As Boolean = MyBase.SendReqTelegram(oReqTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReqTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        If reqNumberForNextSnd >= 999999 Then
            reqNumberForNextSnd = 0
        Else
            reqNumberForNextSnd += 1
        End If

        Return ret
    End Function

    Protected Overrides Function SendReplyTelegram(ByVal iReplyTeleg As ITelegram, ByVal iSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As EkTelegram = DirectCast(iReplyTeleg, EkTelegram)
        Dim oSourceTeleg As EkTelegram = DirectCast(iSourceTeleg, EkTelegram)
        oReplyTeleg.RawReqNumber = oSourceTeleg.RawReqNumber
        oReplyTeleg.RawClientCode = oSourceTeleg.RawClientCode
        Dim ret As Boolean = MyBase.SendReplyTelegram(oReplyTeleg, oSourceTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "T")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReplyTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return ret
    End Function

    'NAK電文を送信する場合や受信した場合のその後の挙動を決めるためのメソッド
    Protected Overrides Function GetRequirement(ByVal oNakTeleg As INakTelegram) As NakRequirement
        'TODO: データ種別などでも分岐しておけば、ほとんどのケースを
        'プロトコル違反とみなして、NakRequirement.DisconnectImmediately
        'を返却することになるはず。
        Select Case oNakTeleg.CauseCode
            '継続（リトライオーバー）しても異常とはみなせないNAK電文
            'Case EkNakCauseCode.Xxxx
            '    Return NakRequirement.ForgetOnRetryOver

            '継続（リトライオーバー）したら異常とみなすべきNAK電文
            Case EkNakCauseCode.Busy, EkNakCauseCode.NoData, EkNakCauseCode.NoTime, EkNakCauseCode.Unnecessary, EkNakCauseCode.InvalidContent, EkNakCauseCode.UnknownLight
                Return NakRequirement.CareOnRetryOver

            '通信異常とみなすべきNAK電文
            Case EkNakCauseCode.TelegramError, EkNakCauseCode.NotPermit, EkNakCauseCode.HashValueError, EkNakCauseCode.UnknownFatal
                Return NakRequirement.DisconnectImmediately

            'NOTE: どのようなバイト列をParseしてもCauseCodeがNoneの
            'NAK電文にはならないはずであるため、CauseCodeがNoneの場合、
            '下記のケースとして処理する。
            Case Else
                Debug.Fail("This case is impermissible.")
                Return NakRequirement.CareOnRetryOver
        End Select
    End Function

    Protected Sub UpdateTraceNumberForComStart()
        If traceNumberForComStart >= 999 Then
            traceNumberForComStart = 0
        Else
            traceNumberForComStart += 1
        End If
    End Sub

    Protected Sub UpdateTraceNumberForTimeDataGet()
        If traceNumberForTimeDataGet >= 999 Then
            traceNumberForTimeDataGet = 0
        Else
            traceNumberForTimeDataGet += 1
        End If
    End Sub

    Protected Sub UpdateTraceNumberForActiveOne()
        If traceNumberForActiveOne >= 999 Then
            traceNumberForActiveOne = 0
        Else
            traceNumberForActiveOne += 1
        End If
    End Sub
#End Region

End Class

''' <summary>
''' 回線状態。
''' </summary>
Public Enum LineStatus As Integer
    Initial
    Connected
    Steady
    Disconnected
End Enum
