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
''' Ｎ間として運管サーバと電文の送受信を行うクラス。
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

    '送受信履歴ディレクトリ
    Protected sCapDirPath As String

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
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket, ByVal sCapDirPath As String, ByVal oForm As MainForm)
        MyBase.New(sThreadName, oParentMessageSock, New NkTelegramImporter())
        Me.sCapDirPath = sCapDirPath
        Me.oForm = oForm
        Me.enableCommunication = False
        Me.LineStatus = LineStatus.Initial

        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite
        Me.enableWatchdog = False
        Me.enableActiveOneOrdering = True
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
            Case MyInternalMessageKind.InquiryExecRequest
                Return ProcOnInquiryExecRequestReceive(oRcvMsg)
            Case MyInternalMessageKind.ActiveOneExecRequest
                Return ProcOnActiveOneExecRequestReceive(oRcvMsg)
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
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "R", "Telegram")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    DirectCast(oRcvTeleg, NkTelegram).WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return MyBase.ProcOnTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnComStartExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ComStartExec requested by manager.")

        'TODO: NkSeqCode.Collectionの部分はoRcvMsgから取得。

        Dim sSeqName As String = "ComStart"
        Log.Info("Register " & sSeqName & " as ActiveOne.")
        Dim oReqTeleg As New NkComStartReqTelegram(NkSeqCode.Collection, Config.ComStartReplyLimitTicks)
        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        Return True
    End Function

    Protected Overridable Function ProcOnInquiryExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("InquiryExec requested by manager.")

        'TODO: NkSeqCode.Collectionの部分はoRcvMsgから取得。

        Dim sSeqName As String = "Inquiry"
        Log.Info("Register " & sSeqName & " as ActiveOne.")
        Dim oReqTeleg As New NkInquiryReqTelegram(NkSeqCode.Collection, Config.InquiryReplyLimitTicks)
        RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
        Return True
    End Function

    Protected Overridable Function ProcOnActiveOneExecRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ActiveOneExec requested by manager.")

        Dim oExt As ActiveOneExecRequestExtendPart _
           = ActiveOneExecRequest.Parse(oRcvMsg).ExtendPart

        Dim oTeleg As NkDodgyTelegram
        Try
            Dim oImporter As New NkTelegramImporter()
            Using oInputStream As New FileStream(oExt.ApplyFilePath, FileMode.Open, FileAccess.Read)
                oTeleg = oImporter.GetTelegramFromStream(oInputStream)
            End Using
            If oTeleg Is Nothing Then Return True
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            Return True
        End Try

        Dim sSeqName As String = "ActiveOne"
        Log.Info("Register " & sSeqName & " as ActiveOne.")

        Dim oReqTeleg As New NkAnonyReqTelegram(oTeleg)
        oReqTeleg.ReplyLimitTicks = oExt.ReplyLimitTicks

        RegisterActiveOne(oReqTeleg, oExt.RetryIntervalTicks, oExt.MaxRetryCountToForget + 1, oExt.MaxRetryCountToCare + 1, sSeqName)
        Return True
    End Function

    '能動的単発シーケンスが成功した場合
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(NkComStartReqTelegram)
                Log.Info("ComStart completed.")

            Case rtt Is GetType(NkInquiryReqTelegram)
                Log.Info("Inquiry completed.")

            Case Else
                Log.Info("ActiveOne completed.")
        End Select
    End Sub

    '能動的単発シーケンスで異常とみなすべきでないリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        'NOTE: あり得ない。
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(NkComStartReqTelegram)
                Log.Error("ComStart skipped.")

            Case rtt Is GetType(NkInquiryReqTelegram)
                Log.Error("Inquiry skipped.")

            Case Else
                Log.Error("ActiveOne skipped.")
        End Select
    End Sub

    '能動的単発シーケンスで異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        'NOTE: あり得ない。
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(NkComStartReqTelegram)
                Log.Error("ComStart failed.")
                Dim automaticComStart As Boolean
                SyncLock oForm.UiState
                    automaticComStart = oForm.UiState.AutomaticComStart
                End SyncLock
                If automaticComStart AndAlso LineStatus <> LineStatus.Steady Then
                   '自動の開局シーケンスでリトライオーバーが発生した場合である。
                    enableCommunication = False
                End If

            Case rtt Is GetType(NkInquiryReqTelegram)
                Log.Error("Inquiry failed.")
                'NOTE: シミュレータなので、切断は手動で自由に実施することにしている。
                '上で「enableCommunication = False」を行っているケースは、
                '自動制御のための機能だからである。

            Case Else
                Log.Error("ActiveOne failed.")
                'NOTE: シミュレータなので、切断は手動で自由に実施することにしている。
                '上で「enableCommunication = False」を行っているケースは、
                '自動制御のための機能だからである。
        End Select
    End Sub

    '能動的単発シーケンスの最中やキューイングされた能動的単発シーケンスの実施前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        Dim rtt As Type = iReqTeleg.GetType()
        Select Case True
            Case rtt Is GetType(NkComStartReqTelegram)
                Log.Error("ComStart failed.")

            Case rtt Is GetType(NkInquiryReqTelegram)
                Log.Error("Inquiry failed.")

            Case Else
                Log.Error("ActiveOne failed.")
        End Select
    End Sub

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As NkTelegram = DirectCast(iRcvTeleg, NkTelegram)
        Select Case oRcvTeleg.CmdCode
            Case NkCmdCode.DataPostReq
                Dim isSeqCodeRegistered As Boolean = False
                SyncLock oForm.UiState
                    isSeqCodeRegistered = oForm.UiState.StatusCodeForPassivePostSeqCodes.ContainsKey(oRcvTeleg.SeqCode)
                End SyncLock
                If isSeqCodeRegistered Then
                    Return ProcOnRegisteredPostReqTelegramReceive(oRcvTeleg)
                End If
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnRegisteredPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkDataPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("DataPost REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Log.Info("DataPost REQ received.")

        Dim statusCode As UShort
        SyncLock oForm.UiState
            statusCode = oForm.UiState.StatusCodeForPassivePostSeqCodes(oRcvTeleg.SeqCode)
        End SyncLock

        Dim oReplyTeleg As NkDataPostAckTelegram = oRcvTeleg.CreateAckTelegram(statusCode)
        Log.Info("Sending DataPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    '親スレッドからコネクションを受け取った場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionAppear()
        enableCommunication = True

        Dim automaticComStart As Boolean
        SyncLock oForm.UiState
            automaticComStart = oForm.UiState.AutomaticComStart
        End SyncLock

        If automaticComStart Then
            Dim sSeqName As String = "ComStart"
            Log.Info("Register " & sSeqName & " as ActiveOne.")
            Dim oReqTeleg As New NkComStartReqTelegram(NkSeqCode.Collection, Config.ComStartReplyLimitTicks)
            RegisterActiveOne(oReqTeleg, 0, 1, 1, sSeqName)
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
#End Region

#Region "イベント処理実装用メソッド"
    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As NkReqTelegram = DirectCast(iReqTeleg, NkReqTelegram)
        oReqTeleg.SrcEkCode = Config.SelfEkCode
        oReqTeleg.DstEkCode = Config.TargetEkCode
        Dim ret As Boolean = MyBase.SendReqTelegram(oReqTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "Telegram")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReqTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return ret
    End Function

    Protected Overrides Function SendReplyTelegram(ByVal iReplyTeleg As ITelegram, ByVal iSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        oReplyTeleg.SrcEkCode = Config.SelfEkCode
        oReplyTeleg.DstEkCode = Config.TargetEkCode
        Dim ret As Boolean = MyBase.SendReplyTelegram(oReplyTeleg, iSourceTeleg)

        Dim capSndTelegs As Boolean
        SyncLock oForm.UiState
            capSndTelegs = oForm.UiState.CapSndTelegs
        End SyncLock

        If capSndTelegs Then
            Try
                Dim sFilePath As String = CapDataPath.Gen(sCapDirPath, DateTime.Now, "S", "Telegram")
                Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                    oReplyTeleg.WriteToStream(oOutputStream)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        Return ret
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
