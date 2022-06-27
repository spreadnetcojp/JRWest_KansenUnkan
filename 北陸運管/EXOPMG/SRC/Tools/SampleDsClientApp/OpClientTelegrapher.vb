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

Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 運管端末として運管サーバと電文の送受信を行うクラス。
''' </summary>
Public Class OpClientTelegrapher
    Inherits ClientTelegrapher

#Region "定数や変数"
    '電文書式
    Protected oTelegGene As EkTelegramGene

    '自装置の装置コード
    'NOTE: ProcOnReqTelegramReceive()をフックして受信電文のClientCodeと比較してもよい。
    Protected selfEkCode As EkCode

    '次に送信するREQ電文の通番
    Protected reqNumberForNextSnd As Integer

    '次に受信するREQ電文の通番
    'NOTE: ProcOnReqTelegramReceive()をフックして、受信したREQ電文の通番の
    '連続性等をチェックするなら用意する。
    'Protected reqNumberForNextRcv As Integer

    '電文の交換を継続すべきか否か
    Protected enableCommunication As Boolean

    'NOTE: 「意図的な切断」と「異常による切断」を区別したいならば、
    'Protected needConnection As Booleanを用意し、
    'ProcOnConnectNoticeReceive()とProcOnDisconnectRequestReceive()をフックして
    'それをON/OFFするとよい。ProcOnConnectionDisappear()では、それをみて、
    '遷移先の回線状態を決めることができる。

    '回線状態
    Private _LineStatus As Integer
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal oTelegGene As EkTelegramGene)
        MyBase.New(sThreadName, oParentMessageSock, New EkTelegramImporter(oTelegGene))
        Me.oTelegGene = oTelegGene
        Me.reqNumberForNextSnd = 0
        Me.enableCommunication = False
        Me.LineStatus = LineStatus.Initial

        Me.selfEkCode.Unit = 2
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

        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(Me.oActiveXllWorkerMessageSock, oChildSock)
        Me.oActiveXllWorker = New FtpWorker( _
           sThreadName & "-ActiveXll", _
           oChildSock, _
           Config.FtpServerUri, _
           New NetworkCredential(Config.FtpUserName, Config.FtpPassword), _
           Config.FtpRequestLimitTicks, _
           Config.FtpLogoutLimitTicks, _
           Config.FtpTransferStallLimitTicks, _
           Config.FtpUsePassiveMode, _
           Config.FtpLogoutEachTime, _
           Config.FtpBufferLength)
        Me.activeXllWorkerPendingLimitTicks = Config.FtpWorkerPendingLimitTicks
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
    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case ClientAppInternalMessageKind.MasProUllRequest
                Return ProcOnMasProUllRequestReceive(oRcvMsg)
            Case ClientAppInternalMessageKind.MasProDllInvokeRequest
                Return ProcOnMasProDllInvokeRequestReceive(oRcvMsg)
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
    End Function

    'ヘッダ部の内容がウォッチドッグREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsWatchdogReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oTeleg.ObjCode = EkWatchdogReqTelegram.FormalObjCodeInOpClient Then
            Return True
        Else
            Return False
        End If
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsWatchdogReq(ByVal iTeleg As ITelegram) As IWatchdogReqTelegram
        Return New EkWatchdogReqTelegram(iTeleg)
    End Function

    Protected Overridable Function ProcOnMasProDllInvokeRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("MasProDllInvoke requested by manager.")

        Dim oExt As MasProDllInvokeRequestExtendPart _
           = MasProDllInvokeRequest.Parse(oRcvMsg).ExtendPart
        Dim oReqTeleg As New EkMasProDllInvokeReqTelegram( _
           oTelegGene, _
           EkMasProDllInvokeReqTelegram.FormalObjCode, _
           oExt.ListFileName, _
           oExt.ForcingFlag, _
           Config.MasProDllInvokeReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, "MasProDllInvoke")
        Return True
    End Function

    '能動的単発シーケンスが成功した場合
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal oReqTeleg As IReqTelegram, ByVal oAckTeleg As ITelegram)
        Dim rtt As Type = oReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Info("ComStart completed.")
                enableActiveOneOrdering = Config.EnableActiveOneOrdering

            Case rtt Is GetType(EkMasProDllInvokeReqTelegram)
                Log.Info("MasProDllInvoke completed.")
                MasProDllInvokeResponse.Gen(MasProDllInvokeResult.Completed).WriteToSocket(oParentMessageSock)
        End Select
    End Sub

    '能動的単発シーケンスで異常とみなすべきでないリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal oReqTeleg As IReqTelegram, ByVal oNakTeleg As INakTelegram)
        Dim rtt As Type = oReqTeleg.GetType()
        Select Case True
            'NOTE: あり得ない。
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Warn("ComStart skipped by illegal NAK.")
                Me.enableCommunication = False

            'NOTE: あり得ない。
            Case rtt Is GetType(EkMasProDllInvokeReqTelegram)
                Log.Warn("MasProDllInvoke skipped by illegal NAK.")
                MasProDllInvokeResponse.Gen(MasProDllInvokeResult.Failed).WriteToSocket(oParentMessageSock)
        End Select
    End Sub

    '能動的単発シーケンスで異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal oReqTeleg As IReqTelegram, ByVal oNakTeleg As INakTelegram)
        Dim rtt As Type = oReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart failed.")
                Me.enableCommunication = False

            Case rtt Is GetType(EkMasProDllInvokeReqTelegram)
                Log.Error("MasProDllInvoke failed.")

                Dim result As MasProDllInvokeResult = MasProDllInvokeResult.FailedByUnknownLight
                If oNakTeleg IsNot Nothing Then
                    Select Case oNakTeleg.CauseCode
                        Case EkNakCauseCode.Busy
                            result = MasProDllInvokeResult.FailedByBusy
                        Case EkNakCauseCode.NoData
                            result = MasProDllInvokeResult.FailedByNoData
                        Case EkNakCauseCode.Unnecessary
                            result = MasProDllInvokeResult.FailedByUnnecessary
                        Case EkNakCauseCode.InvalidContent
                            result = MasProDllInvokeResult.FailedByInvalidContent
                    End Select
                End If

                MasProDllInvokeResponse.Gen(result).WriteToSocket(oParentMessageSock)
        End Select
    End Sub

    '能動的単発シーケンスの最中やキューイングされた能動的単発シーケンスの実施前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal oReqTeleg As IReqTelegram)
        Dim rtt As Type = oReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(EkComStartReqTelegram)
                Log.Error("ComStart failed.")

            Case rtt Is GetType(EkMasProDllInvokeReqTelegram)
                Log.Error("MasProDllInvoke failed.")
                MasProDllInvokeResponse.Gen(MasProDllInvokeResult.Failed).WriteToSocket(oParentMessageSock)
        End Select
    End Sub

    Protected Overridable Function ProcOnMasProUllRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("MasProUll requested by manager.")

        Dim oXllReqTeleg As New EkClientDrivenUllReqTelegram( _
           oTelegGene, _
           EkClientDrivenUllReqTelegram.FormalObjCodeAsOpClientFile, _
           ContinueCode.Start, _
           MasProUllRequest.Parse(oRcvMsg).FileName, _
           Config.MasProUllTransferLimitTicks, _
           Config.MasProUllStartReplyLimitTicks)

        RegisterActiveUll(oXllReqTeleg, 0, 1, 1)
        Return True
    End Function

    '能動的ULLの転送開始REQ電文に続く転送終了REQ電文を生成するメソッド
    Protected Overrides Function CreateActiveUllContinuousReqTelegram(ByVal oXllReqTeleg As IXllReqTelegram, ByVal cc As ContinueCode) As IXllReqTelegram
        Dim oRealUllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)
        Return oRealUllReqTeleg.CreateContinuousTelegram(cc, 0, Config.MasProUllFinishReplyLimitTicks)
    End Function

    '能動的ULLが成功した（転送終了REQ電文に対してACK電文を受信した）場合
    Protected Overrides Sub ProcOnActiveUllComplete(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oAckTeleg As IXllTelegram)
        Log.Info("Ull file completed.")
        MasProUllResponse.Gen(MasProUllResult.Completed).WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLにて転送終了REQ電文に対してNAK電文を受信した場合
    Protected Overrides Sub ProcOnActiveUllFinalizeError(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Error("Ull file failed by FinalizeError.")

        Dim result As MasProUllResult = MasProUllResult.FailedByUnknownLight
        If oNakTeleg IsNot Nothing Then
            Select Case oNakTeleg.CauseCode
                Case EkNakCauseCode.InvalidContent
                    result = MasProUllResult.FailedByInvalidContent
            End Select
        End If

        MasProUllResponse.Gen(result).WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLにて転送が失敗した（ContinueCode.Abortの転送終了REQ電文を送信することになる）場合
    Protected Overrides Sub ProcOnActiveUllTransferError(ByVal oXllReqTeleg As IXllReqTelegram)
        Log.Error("Ull file failed by TransferError.")
        MasProUllResponse.Gen(MasProUllResult.Failed).WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLの開始で異常とみなすべきでないリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveUllRetryOverToForget(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        'NOTE: あり得ないと思われるが、相手が返してくるNAK次第であるため、実装しておく。
        '本当にあり得ないものと扱うには、GetRequirement()にて、
        'OpClientFileUllに関するEkNakCauseCode.NoDataなNAKを切断扱いにするとよい。
        Log.Warn("Ull file failed by surprising RetryOver.")
        MasProUllResponse.Gen(MasProUllResult.Failed).WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLの開始で異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveUllRetryOverToCare(ByVal oXllReqTeleg As IXllReqTelegram, ByVal oNakTeleg As INakTelegram)
        Log.Error("Ull file failed by RetryOver.")

        Dim result As MasProUllResult = MasProUllResult.FailedByUnknownLight
        If oNakTeleg IsNot Nothing Then
            Select Case oNakTeleg.CauseCode
                Case EkNakCauseCode.Busy
                    result = MasProUllResult.FailedByBusy
                Case EkNakCauseCode.InvalidContent
                    result = MasProUllResult.FailedByInvalidContent
            End Select
        End If

        MasProUllResponse.Gen(result).WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLの最中やキューイングされた能動的ULLの開始前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveUllAnonyError(ByVal oXllReqTeleg As IXllReqTelegram)
        Log.Error("Ull file failed.")
        MasProUllResponse.Gen(MasProUllResult.Failed).WriteToSocket(oParentMessageSock)
    End Sub

    '親スレッドからコネクションを受け取った場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionAppear()
        reqNumberForNextSnd = 0
        enableCommunication = True

        enableActiveOneOrdering = True
        Log.Info("Register ComStart as ActiveOne.")

        Dim oReqTeleg As New EkComStartReqTelegram( _
           oTelegGene, _
           EkComStartReqTelegram.FormalObjCodeInOpClient,
           Config.ComStartReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, 0, 1, 1, "ComStart")
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
#End Region

#Region "イベント処理実装用メソッド"
    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        oReqTeleg.ReqNumber = reqNumberForNextSnd
        oReqTeleg.ClientCode = selfEkCode
        Dim ret As Boolean = MyBase.SendReqTelegram(oReqTeleg)

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
        Return MyBase.SendReplyTelegram(oReplyTeleg, oSourceTeleg)
    End Function

    'NAK電文を送信する場合や受信した場合のその後の挙動を決めるためのメソッド
    Protected Overrides Function GetRequirement(ByVal oNakTeleg As INakTelegram) As NakRequirement
        'NOTE: 運管端末では、送信するあらゆるREQ電文のLimitNakCountが1であるため、
        '軽度のNAKは、全てForgetOnRetryOverにしてもいいし、
        '全てCareOnRetryOverにしてもいい。
        'NOTE: 相手装置に対して厳しく接してよいなら、データ種別などで分岐することも可能。
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
