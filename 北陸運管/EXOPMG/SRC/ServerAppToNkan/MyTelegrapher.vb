' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' Ｎ間と電文の送受信を行うクラス。
''' </summary>
Public Class MyTelegrapher
    Inherits ServerTelegrapher

#Region "内部クラス等"
    '通信フェーズの定義
    Protected Enum ComPhase As Integer
        NoSession
        Idling
    End Enum
#End Region

#Region "定数や変数"
    '集計開始シーケンス番号ファイルの名前
    Protected Const sSeqNumberFileName As String = "InitialSeqNumber.txt"

    '集信系シーケンスで送信するデータのサブヘッダ部の長さ
    Protected Const subHeaderLen As Integer = 28

    'サブヘッダにおける作成日時の位置と書式
    Protected Const timestampPosInSubHeader As Integer = 0
    Protected Const timestampLenInSubHeader As Integer = 14
    Protected Const sTimestampFormatInSubHeader As String = "yyyyMMddHHmmss"

    'サブヘッダにおけるシーケンス番号の位置と書式
    Protected Const seqNumberPosInSubHeader As Integer = 14
    Protected Const seqNumberLenInSubHeader As Integer = 6

    'サブヘッダにおけるデータ件数の位置と書式
    Protected Const recCountPosInSubHeader As Integer = 20
    Protected Const recCountLenInSubHeader As Integer = 8

    '締切データの書式
    Protected Const summaryLen As Integer = 128
    Protected Const summaryKindPos As Integer = 0
    Protected Const summaryTimestampPos As Integer = 1
    Protected Const summaryFormatIdPos As Integer = 14
    Protected Const summarySentCountPos As Integer = 15
    Protected Const summaryKindValue As Byte = &HD1
    Protected Const summaryFormatIdValue As Byte = &H1

    'シーケンス番号
    'TODO: リセットするタイミング（リセットしない or 締切ごと or コネクションごと）や
    'インクリメントの条件（否定応答を受信したときはインクリメントしない or する）、
    '付与対象（電文ごと or 利用データのレコードごと or 締切データも含めたレコードごと）、
    '0はじまりか1はじまりか、否定応答を受けてインクリメントしない場合、別のデータを先頭に
    'したレコードの塊を同じ番号で送付してよいか否か...等不明。
    Protected Const seqNumberMax As Integer = 999999
    Protected seqNumber As Integer

    '通信フェーズ
    Protected curComPhase As ComPhase

    'Ｎ間のアドレスコード
    Protected nkanEkCode As EkCode

    '担当駅のアドレスコード
    Protected selfEkCode As EkCode

    'リスニングソケット
    Protected oListenerSock As Socket

    '登録済み利用データ格納ディレクトリのパス
    Protected sInputDirPath As String

    '集計中利用データ格納ディレクトリのパス
    Protected sTallyingDirPath As String

    '集計済み利用データディレクトリ移動先ディレクトリのパス
    Protected sTrashDirPath As String

    '締切データ送信要否
    Protected needToSendSummaryData As Boolean

    '送信中の利用データのパスとレコード数
    Protected oSendingRiyoFilePathList As List(Of String)
    Protected sendingRiyoRecCount As Integer

    'NOTE: 「意図的な切断」と「異常による切断」を区別したいならば、
    'Protected needConnection As Booleanを用意し、
    'ProcOnComStartReqTelegramReceive()とProcOnComStopReqTelegramReceive()にて
    'それをON/OFFするとよい。ProcOnConnectionDisappear()では、それをみて、
    '遷移先の回線状態を決めることができる。

    '収集データ誤記テーブルに対する通信異常の重複登録禁止期間か否か
    Protected hidesLineErrorFromRecording As Boolean

    '収集データ誤記テーブルに対する通信異常の重複登録禁止期間をつくるタイマ
    Protected oLineErrorRecordingIntervalTimer As TickTimer

    '初回接続タイマ
    'NOTE: 通信異常を検出するためのタイマである。
    '一度でも接続すれば、切断時に通信異常と認識できるため、
    '動作させる必要はなくなる。
    Protected oInitialConnectLimitTimerForLineError As TickTimer

    '回線状態
    Private _LineStatus As Integer
#End Region

#Region "コンストラクタ"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal oTelegImporter As NkTelegramImporter, _
       ByVal selfEkCode As EkCode, _
       ByVal oListenerSock As Socket)

        MyBase.New(sThreadName, oParentMessageSock, oTelegImporter)

        Me.curComPhase = ComPhase.NoSession
        Me.nkanEkCode.RailSection = &HFF
        Me.nkanEkCode.StationOrder = &HFF
        Me.selfEkCode = selfEkCode
        Me.oListenerSock = oListenerSock
        Me.RegisterSocket(oListenerSock)

        Dim sBaseDirPath As String = Utility.CombinePathWithVirtualPath(Config.RiyoDataDirPath, selfEkCode.ToString(Config.RiyoDataStationBaseDirNameFormat))
        '-------Ver0.1 次世代車補対応 MOD START-----------
        Me.sInputDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataOutputDirPathInStationBase)
        '-------Ver0.1 次世代車補対応 MOD END-------------
        Me.sTallyingDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataTallyingDirPathInStationBase)
        Me.sTrashDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataTrashDirPathInStationBase)
        Me.needToSendSummaryData = False
        Me.oSendingRiyoFilePathList = Nothing
        Me.sendingRiyoRecCount = 0
        Me.hidesLineErrorFromRecording = False
        Me.oLineErrorRecordingIntervalTimer = New TickTimer(Config.LineErrorRecordingIntervalTicks)
        Me.oInitialConnectLimitTimerForLineError = New TickTimer(Config.InitialConnectLimitTicksForLineError)
        Me.LineStatus = LineStatus.Initial

        Me.oWatchdogTimer.Renew(If(Config.TelegrapherPendingLimitTicks >= 8, Config.TelegrapherPendingLimitTicks \ 8, 1))
        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB

        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite

        '登録済み利用データ格納ディレクトリがなければ、作成しておく。
        Directory.CreateDirectory(sInputDirPath)

        '集計済み利用データ格納ディレクトリがなければ、作成しておく。
        Directory.CreateDirectory(sTrashDirPath)

        '集計中利用データ格納ディレクトリがなければ、作成しておく。
        If Not Directory.Exists(sTallyingDirPath) Then
            Directory.CreateDirectory(sTallyingDirPath)
            Me.seqNumber = -1
        Else
            Me.seqNumber = GetNextSeqNumber(sTallyingDirPath)
        End If

        '集計中利用データ格納ディレクトリに開始通番ファイルがなければ、作成しておく。
        If Me.seqNumber = -1 Then
            Dim oLatestBackDirInfo As DirectoryInfo = TimestampedDirPath.FindLatest(sTrashDirPath)
            If oLatestBackDirInfo IsNot Nothing Then
                Dim number As Integer = GetNextSeqNumber(oLatestBackDirInfo.FullName)
                If number >= 0 Then
                    'NOTE: 集計済みのディレクトリになっている以上、
                    '締切データの送信も行っているものとみなし、
                    'その分をインクリメントする。
                    number = GetNextSeqNumber(number, 1)
                    Log.Warn("Restoring seq number to [" & number.ToString() & "]...")
                    SetInitialSeqNumber(sTallyingDirPath, number)
                    Me.seqNumber = GetNextSeqNumber(sTallyingDirPath)
                End If
            Else
                SetInitialSeqNumber(sTallyingDirPath, 0)
                Me.seqNumber = GetNextSeqNumber(sTallyingDirPath)
            End If
        End If

        '集計中利用データ格納ディレクトリに正しく読めない開始通番ファイルがあった場合や、
        '開始通番ファイルが作成できなかった場合
        If Me.seqNumber < 0 Then
            'TODO: 運用的にどうするのが一番よいか確認する。
            SetInitialSeqNumber(sTallyingDirPath, 0)
            Me.seqNumber = GetNextSeqNumber(sTallyingDirPath)
            If Me.seqNumber < 0 Then
                Application.Exit()
            End If
        End If
    End Sub
#End Region

#Region "プロパティ"
    'このプロパティは、親スレッドにおいても、ある瞬間（少し過去の瞬間）の
    '回線状態をユーザに表示する目的であれば、参照可能である。
    'このスレッドを特定の箇所で停止させた上で状態を取得する（その後、この
    'スレッドに対して任意の操作を行ってから、任意に再開させることができる）
    'わけではないため、呼び出しから戻った時点で、戻り値の回線状態が維持
    'されているとは限らない。そのかわり、高頻度で呼び出しても大した負荷が
    'かからない。
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

        Protected Set(ByVal status As LineStatus)
            Interlocked.Exchange(_LineStatus, status)
        End Set
    End Property
#End Region

#Region "親スレッド用メソッド"
    Public Overrides Sub Start()
        Dim systemTick As Long = TickTimer.GetSystemTick()
        RegisterTimer(oInitialConnectLimitTimerForLineError, systemTick)

        MyBase.Start()
    End Sub
#End Region

#Region "イベント処理メソッド"
    Protected Overrides Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oLineErrorRecordingIntervalTimer Then
            Return ProcOnLineErrorRecordingTime()
        End If

        If oTimer Is oInitialConnectLimitTimerForLineError Then
            Return ProcOnInitialConnectLimitTimeForLineError()
        End If

        Return MyBase.ProcOnTimeout(oTimer)
    End Function

    Protected Overridable Function ProcOnLineErrorRecordingTime() As Boolean
        Log.Info("Line error recording time comes.")

        If LineStatus = LineStatus.Steady Then
            hidesLineErrorFromRecording = False
        Else
            '収集データ誤記テーブルに通信異常を登録する。
            'NOTE: 収集データ誤記テーブルでは、新たな異常の登録が無いことを
            '以て、異常が復旧したとみなすことになるため、通信異常が発生している
            '限りは、定期的に新たな異常を登録しなければならない。
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            Debug.Assert(hidesLineErrorFromRecording = True)
        End If
        Return True
    End Function

    Protected Overridable Function ProcOnInitialConnectLimitTimeForLineError() As Boolean
        Log.Error("Initial connection limit time comes for line error.")

        If Not hidesLineErrorFromRecording Then
            '収集データ誤記テーブルに通信異常を登録する。
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromRecording = True
        End If
        Return True
    End Function

    Protected Overrides Function ProcOnSockReadable(ByVal oSock As Socket) As Boolean
        If oSock Is oListenerSock Then
            If curComPhase <> ComPhase.NoSession Then
                Disconnect()
                Debug.Assert(curComPhase = ComPhase.NoSession)
            End If

            Dim oNewSocket As Socket = Nothing
            Try
                oNewSocket = SockUtil.Accept(oListenerSock)
            Catch ex As OPMGException
                'NOTE: 実際のところはともかく、リスニングソケットが読み出し可能
                'になったからといって、そこからのAccept()が成功するとは限らない
                '（linuxのソケットのように、Accept()を呼び出すまでの間に発生
                'したコネクションの異常が、Accept()で通知される可能性もある）
                'ものとみなす。
                Log.Error("Exception caught.", ex)
            End Try

            If oNewSocket IsNot Nothing Then
                Dim oRemoteEndPoint As IPEndPoint = DirectCast(oNewSocket.RemoteEndPoint, IPEndPoint)
                Dim oRemoteIPAddr As IPAddress = oRemoteEndPoint.Address
                Log.Info("Incoming from [" & oRemoteEndPoint.Address.ToString() & "].")
                Connect(oNewSocket)

                'NOTE: 以降、開局要求を無期限で待ち続ける。
                'oListenerSockが相手をしなければならない装置が１台である故、
                '現在の接続済みソケットが残り続けるにしても、次の接続済み
                'ソケットを作るタイミングで回収することになる（リソース
                '使用量の単調増加はない）ため問題ない。
                '運管システム側において、通信異常をユーザに知らせるべきで
                'あるにしても、開局要求待ちタイムアウトのような仕組みで
                '発生させる必要はない。このスレッドが起動直後であれば、
                'oInitialConnectLimitTimerForLineErrorが動作しているはずであり、
                '開局要求がなければ、そのタイマがタイムアウトして、
                'DBに異常を登録することになる。このスレッドが起動直後
                'でない場合は、oInitialConnectLimitTimerForLineErrorの
                'タイムアウト発生時またはDisconnect()実行時において、
                '既にDBに異常が登録されているはずであり、その後も、
                'LineStatusがInitialまたはDisconnectedであることにより、
                '定期的に異常の登録が繰り返されるはずである。
            End If

            Return True
        End If

        Return MyBase.ProcOnSockReadable(oSock)
    End Function

    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Select Case oRcvMsg.Kind
            Case ServerAppInternalMessageKind.TallyTimeNotice
                Return ProcOnTallyTimeNoticeReceive(oRcvMsg)
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
    End Function

    Protected Overridable Function ProcOnTallyTimeNoticeReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Tally time notified by manager.")
        needToSendSummaryData = True
        Return True
    End Function

    Protected Overrides Function ProcOnQuitRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Dim toBeContinued As Boolean = MyBase.ProcOnQuitRequestReceive(oRcvMsg)
        If Not toBeContinued Then
            UnregisterSocket(oListenerSock)
        End If
        Return toBeContinued
    End Function

    Protected Overrides Function ProcOnReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As NkTelegram = DirectCast(iRcvTeleg, NkTelegram)
        If oRcvTeleg.DstEkCode <> selfEkCode Then
            Log.Error("Telegram with invalid DstEkCode received.")
            Disconnect()
            Return True
        Else
            Return MyBase.ProcOnReqTelegramReceive(oRcvTeleg)
        End If
    End Function

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As NkTelegram = DirectCast(iRcvTeleg, NkTelegram)
        Select Case oRcvTeleg.SeqCode
            Case NkSeqCode.Collection, NkSeqCode.Delivery
                Select Case oRcvTeleg.CmdCode
                    Case NkCmdCode.ComStartReq
                        Return ProcOnComStartReqTelegramReceive(oRcvTeleg)
                    Case NkCmdCode.ComStopReq
                        Return ProcOnComStopReqTelegramReceive(oRcvTeleg)
                    Case NkCmdCode.InquiryReq
                        Return ProcOnInquiryReqTelegramReceive(oRcvTeleg)
                    Case Else
                        Log.Error("Telegram with invalid CmdCode received.")
                        Disconnect()
                        Return True
                End Select

            Case NkSeqCode.Test
                Select Case oRcvTeleg.CmdCode
                    Case NkCmdCode.InquiryReq
                        Return ProcOnTestReqTelegramReceive(oRcvTeleg)
                    Case Else
                        Log.Error("Test sequence telegram with invalid CmdCode received.")
                        Disconnect()
                        Return True
                End Select

            Case Else
                Log.Error("Telegram with invalid SeqCode received.")
                Disconnect()
                Return True
        End Select

        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnComStartReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkComStartReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("ComStart REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        If oRcvTeleg.SeqCode <> NkSeqCode.Collection Then
            Log.Error("ComStart REQ with invalid SeqCode received.")
            Disconnect()
            Return True
        End If

        If curComPhase <> ComPhase.NoSession Then
            Log.Error("ComStart REQ received in disproportionate phase.")
            Disconnect()
            Return True
        End If

        Log.Info("ComStart REQ received.")

        Dim oReplyTeleg As NkComStartAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ComStart ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        UnregisterTimer(oInitialConnectLimitTimerForLineError)
        curComPhase = ComPhase.Idling
        LineStatus = LineStatus.Steady
        Return True
    End Function

    Protected Overridable Function ProcOnComStopReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkComStopReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("ComStop REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        If curComPhase <> ComPhase.Idling Then
            Log.Warn("ComStop REQ received in disproportionate phase.")
        Else
            Log.Info("ComStop REQ received.")
        End If

        Dim oReplyTeleg As NkComStopAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ComStop ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Disconnect()
        Return True
    End Function

    Protected Overridable Function ProcOnInquiryReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkInquiryReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Inquiry REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        If oRcvTeleg.SeqCode = NkSeqCode.Collection Then
            If curComPhase <> ComPhase.Idling Then
                Log.Warn("CollectionInquiry REQ received in disproportionate phase.")
            Else
                Log.Info("CollectionInquiry REQ received.")
            End If

            If needToSendSummaryData Then
                Log.Info("It's now time to send the summary data.")

                '送受信可否応答（可）を返信する。
                Dim oReplyTeleg As NkInquiryAckTelegram = oRcvTeleg.CreateAckTelegram(0)
                Log.Info("Sending CollectionInquiry ACK with ReturnStatus OK...")
                If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
                    Disconnect()
                    Return True
                End If

                'NOTE: 以下の実施途中で配信系シーケンスや折り返しシーケンスの要求電文を
                '受信することは想定していないため、RegisterActiveOne()を使うことは必須ではない。
                'ただし、RegisterActiveOne()を使用した方が実装がシンプルになるため、
                'RegisterActiveOne()を使用する。

                Dim contentsLength As Long = 0
                If Directory.Exists(sTallyingDirPath) Then
                    contentsLength = UpboundDataPath.GetContentsLength(sTallyingDirPath)
                End If

                'NOTE: 受信時のチェックでriyoRecordLenの倍数のはずであるため、
                'ここで余りが発生するときにどうするべきか（どうなるか）は、
                '考えないことにする。
                Dim sentCount As Integer = CInt(contentsLength \ EkConstants.RiyoDataRecordLen)

                '締切データを作成する。
                Dim aSummaryHeader As Byte() = CreateSubHeader(1)
                Dim aSummaryRecord As Byte() = CreateSummaryRecord(sentCount)
                Dim aSummary(aSummaryHeader.Length + aSummaryRecord.Length - 1) As Byte
                Buffer.BlockCopy(aSummaryHeader, 0, aSummary, 0, aSummaryHeader.Length)
                Buffer.BlockCopy(aSummaryRecord, 0, aSummary, aSummaryHeader.Length, aSummaryRecord.Length)
                Dim oDataPostReqTeleg As New NkDataPostReqTelegram(NkSeqCode.Collection, aSummary, Config.SummaryDataReplyLimitTicks)

                Log.Info("Register SummaryDataPost REQ as ActiveOne.")
                RegisterActiveOne(oDataPostReqTeleg, 0, 1, 1, "SummaryDataPost")
            Else
                '登録済み利用データを検索して、送受信可否応答のリターンステータスを決定する。
                '送信するファイル名のリストは、メモリ上に保持しておく。
                Dim totalLen As Long = 0
                oSendingRiyoFilePathList = UpboundDataPath.FindFullNames(sInputDirPath, totalLen, 4294967295 - subHeaderLen)
                If oSendingRiyoFilePathList.Count = 0 Then
                    '送受信可否応答（否）を返信する。
                    Dim oNegativeReplyTeleg As NkInquiryAckTelegram = oRcvTeleg.CreateAckTelegram(1)
                    Log.Info("Sending CollectionInquiry ACK with ReturnStatus NoData...")
                    If SendReplyTelegram(oNegativeReplyTeleg, oRcvTeleg) = False Then
                        Disconnect()
                        Return True
                    End If

                    Return True
                End If

                '送受信可否応答（可）を返信する。
                Dim oReplyTeleg As NkInquiryAckTelegram = oRcvTeleg.CreateAckTelegram(0)
                Log.Info("Sending CollectionInquiry ACK with ReturnStatus OK...")
                If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
                    Disconnect()
                    Return True
                End If

                'NOTE: 以下の実施途中で配信系シーケンスや折り返しシーケンスの要求電文を
                '受信することは想定していないため、RegisterActiveOne()を使うことは必須ではない。
                '仮に、受信したところで、既に利用データを途中のバイトまで送信している以上、
                '期限内に応答を返信することが普通に不可能であるから、プロトコル上の
                '不正にあたると思われる。ただし、RegisterActiveOne()を使用した方が
                '実装がシンプルになるため、RegisterActiveOne()を使用する。

                'NOTE: 受信時のチェックでriyoRecordLenの倍数のはずであるため、
                'ここで余りが発生するときにどうするべきか（どうなるか）は、
                '考えないことにする。

                '利用データを作成する。
                sendingRiyoRecCount = CInt(totalLen \ EkConstants.RiyoDataRecordLen)
                Dim aSubHeader As Byte() = CreateSubHeader(sendingRiyoRecCount)
                Dim oDataPostReqTeleg As New NkDataPostReqTelegram(NkSeqCode.Collection, aSubHeader, oSendingRiyoFilePathList, totalLen, Config.RiyoDataReplyLimitTicks)

                Log.Info("Register RiyoDataPost REQ as ActiveOne.")
                RegisterActiveOne(oDataPostReqTeleg, 0, 1, 1, "RiyoDataPost")
            End If
        Else
            Log.Info("DeriveryInquiry REQ received.")

            '送受信可否応答（否）を返信する。
            Dim oReplyTeleg As NkInquiryAckTelegram = oRcvTeleg.CreateAckTelegram(1)
            Log.Info("Sending DeriveryInquiry ACK with ReturnStatus NG...")
            If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
                Disconnect()
                Return True
            End If
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnTestReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New NkTestReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> NakCauseCode.None Then
            Log.Error("Test REQ with invalid BodyPart received.")
            Disconnect()
            Return True
        End If

        Log.Info("Test REQ received.")

        '回答レスポンスを返信する。
        Dim oReplyTeleg As NkTestAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending Test ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    '能動的単発シーケンスが成功した場合
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        Dim oReqTeleg As NkDataPostReqTelegram = DirectCast(iReqTeleg, NkDataPostReqTelegram)
        Dim oAckTeleg As NkDataPostAckTelegram = DirectCast(iAckTeleg, NkDataPostAckTelegram)
        Dim returnStatus As UShort = oAckTeleg.ReturnStatus

        '送信したデータの種別を判別する。
        'NOTE: 本当は、データ部先頭付近のデータ種別をみて判別するのが自然であるが、
        '実装量の関係で、下記のようにしている。
        '本当は、GetNextSeqNumberに渡す送信レコード数もoReqTelegのサブヘッダ部から
        '取得した方がよい。
        If oReqTeleg.ObjSize = subHeaderLen + summaryLen Then
            '締切データを送信した場合である。

            '回答レスポンスのリターンステータスが「正常」なら、前日の集計を終了する。
            If returnStatus = 0 Then
                Log.Info("SummaryDataPost ACK with ReturnStatus OK received.")

                '集計中だったディレクトリを集計済みディレクトリの下に移動
                Dim sNewDirPath As String = TimestampedDirPath.Gen(sTrashDirPath, EkServiceDate.Gen(DateTime.Now.AddDays(-1)))
                Directory.Move(sTallyingDirPath, sNewDirPath)

                seqNumber = GetNextSeqNumber(seqNumber, 1)

                '今日の集計に備える。
                Directory.CreateDirectory(sTallyingDirPath)
                SetInitialSeqNumber(sTallyingDirPath, seqNumber)
            Else
                Log.Error("SummaryDataPost ACK with ReturnStatus NG(" & returnStatus.ToString() & ") received.")
            End If

            needToSendSummaryData = False
        Else
            '利用データを送信した場合である。

            '回答レスポンスのリターンステータスが「正常」なら、送信したファイルを
            '集計中ディレクトリに移動する。
            If returnStatus = 0 Then
                Log.Info("RiyoDataPost ACK with ReturnStatus OK received.")

                For Each sPath As String In oSendingRiyoFilePathList
                    File.Move(sPath, UpboundDataPath.Gen(sTallyingDirPath, Path.GetFileName(sPath)))
                Next sPath

                seqNumber = GetNextSeqNumber(seqNumber, sendingRiyoRecCount)
            Else
                Log.Error("RiyoDataPost ACK with ReturnStatus NG(" & returnStatus.ToString() & ") received.")
            End If

            oSendingRiyoFilePathList = Nothing
            sendingRiyoRecCount = 0
        End If
    End Sub

    '能動的単発シーケンスの最中やキューイングされた能動的単発シーケンスの実施前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        oSendingRiyoFilePathList = Nothing
        sendingRiyoRecCount = 0
    End Sub

    'コネクションを切断した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionDisappear()
        curComPhase = ComPhase.NoSession
        LineStatus = LineStatus.Disconnected

        If Not hidesLineErrorFromRecording Then
            '収集データ誤記テーブルに通信異常を登録する。
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromRecording = True
        End If
    End Sub

    Protected Overrides Sub ProcOnUnhandledException(ByVal ex As Exception)
        MyBase.ProcOnUnhandledException(ex)
        UnregisterSocket(oListenerSock)
        'このまま呼び元に戻って、スレッドは終了状態になる。
    End Sub
#End Region

#Region "イベント処理実装用メソッド"
    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        'NOTE: たまたま利用データ等を送信しているときにTallyTimeNoticeを
        '受信すれば、回線断が発生することになるはずである。
        'そのことを考慮するとQuitRequest受信専用ソケットを用意して、それと
        'oListenerSockのみをoInterruptSockListにAddするのが自然である。
        'ただ、締切処理の時刻は、利用データが発生しない時刻に設定することに
        'なっているはずであり、上記のようなことが起き得るのは、特殊なケース
        'しか考えられない。それだけのためにソケットを消費するのもどうか
        'という気がするので、要望があるまでは、このままにしておく。
        Dim oInterruptSockList As New ArrayList(2)
        oInterruptSockList.Add(oListenerSock)
        oInterruptSockList.Add(oParentMessageSock)
        Dim oReqTeleg As NkReqTelegram = DirectCast(iReqTeleg, NkReqTelegram)
        oReqTeleg.SrcEkCode = selfEkCode
        oReqTeleg.DstEkCode = nkanEkCode
        Return oReqTeleg.WriteToSocketInterruptible( _
           oTelegSock, _
           oInterruptSockList, _
           telegWritingLimitBaseTicks, _
           telegWritingLimitExtraTicksPerMiB, _
           telegLoggingMaxLengthOnWrite)
    End Function

    Protected Overrides Function SendReplyTelegram(ByVal iReplyTeleg As ITelegram, ByVal iSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As NkTelegram = DirectCast(iReplyTeleg, NkTelegram)
        oReplyTeleg.SrcEkCode = selfEkCode
        oReplyTeleg.DstEkCode = nkanEkCode
        Return MyBase.SendReplyTelegram(oReplyTeleg, iSourceTeleg)
    End Function

    Protected Function CreateSubHeader(ByVal recCount As Integer) As Byte()
        'TODO: インタフェース仕様が曖昧なので、後でみなおす。
        Dim aSubHeader As Byte() = New Byte(subHeaderLen - 1) {}
        Dim sNow As String = DateTime.Now.ToString(sTimestampFormatInSubHeader)
        Encoding.UTF8.GetBytes(sNow, 0, timestampLenInSubHeader, aSubHeader, timestampPosInSubHeader)
        Utility.CopyIntToDecimalAsciiBytes(seqNumber, aSubHeader, seqNumberPosInSubHeader, seqNumberLenInSubHeader)
        Utility.CopyIntToDecimalAsciiBytes(recCount, aSubHeader, recCountPosInSubHeader, recCountLenInSubHeader)
        Return aSubHeader
    End Function

    Protected Function CreateSummaryRecord(ByVal sentCount As Integer) As Byte()
        Dim aSummaryRecord As Byte() = New Byte(summaryLen - 1) {}
        aSummaryRecord(summaryKindPos) = summaryKindValue
        aSummaryRecord(summaryFormatIdPos) = summaryFormatIdValue
        Dim aTimeStamp As Byte() = Utility.CHARtoBCD(DateTime.Now.ToString("yyyyMMddHHmmss"), 7)
        Buffer.BlockCopy(aTimeStamp, 0, aSummaryRecord, summaryTimestampPos, 7)
        Utility.CopyUInt32ToLeBytes4(CUInt(sentCount), aSummaryRecord, summarySentCountPos)
        Return aSummaryRecord
    End Function

    Protected Function SetInitialSeqNumber(ByVal sDirPath As String, ByVal number As Integer) As Boolean
        Dim aSeqNumber(seqNumberLenInSubHeader - 1) As Byte
        Utility.CopyIntToDecimalAsciiBytes(number, aSeqNumber, 0, seqNumberLenInSubHeader)

        Dim oOutputStream As FileStream = Nothing
        Dim sPath As String = Path.Combine(sDirPath, sSeqNumberFileName)
        Try
            oOutputStream = New FileStream(sPath, FileMode.Create, FileAccess.Write)
            oOutputStream.Write(aSeqNumber, 0, seqNumberLenInSubHeader)
        Catch ex As Exception
            Log.Fatal("Create file [" & sPath & "] failed.")
            Return False
        Finally
            If oOutputStream IsNot Nothing Then
                oOutputStream.Close()
            End If
        End Try

        Return True
    End Function

    Protected Function GetInitialSeqNumber(ByVal sDirPath As String) As Integer
        Dim aSeqNumber(seqNumberLenInSubHeader - 1) As Byte

        Dim oInputStream As FileStream = Nothing
        Dim sPath As String = Path.Combine(sDirPath, sSeqNumberFileName)
        Try
            oInputStream = New FileStream(sPath, FileMode.Open, FileAccess.Read)
            oInputStream.Read(aSeqNumber, 0, seqNumberLenInSubHeader)
        Catch ex As FileNotFoundException
            Log.Warn("[" & sPath & "] not found.")
            Return -1
        Catch ex As Exception
            Log.Fatal("[" & sPath & "] is broken.")
            Return -2
        Finally
            If oInputStream IsNot Nothing Then
                oInputStream.Close()
            End If
        End Try

        If Not Utility.IsDecimalAsciiBytesFixed(aSeqNumber, 0, seqNumberLenInSubHeader) Then
            Log.Fatal("[" & sPath & "] is broken.")
            Return -2
        End If

        Return Utility.GetIntFromDecimalAsciiBytes(aSeqNumber, 0, seqNumberLenInSubHeader)
    End Function

    Protected Function GetNextSeqNumber(ByVal sDirPath As String) As Integer
        Dim number As Integer = GetInitialSeqNumber(sDirPath)
        If number < 0 Then Return number

        'NOTE: 受信時のチェックでriyoRecordLenの倍数のはずであるため、
        'ここで余りが発生するときにどうするべきか（どうなるか）は、
        '考えないことにする。
        Dim contentsLength As Long = UpboundDataPath.GetContentsLength(sDirPath)
        Dim recCount As Integer = CInt(contentsLength \ EkConstants.RiyoDataRecordLen)

        Return GetNextSeqNumber(number, recCount)
    End Function

    Protected Function GetNextSeqNumber(ByVal number As Integer, ByVal recCount As Integer) As Integer
        number += recCount
        If number > seqNumberMax Then
            number -= seqNumberMax
        End If
        Return number
    End Function

    Protected Sub InsertLineErrorToCdt()
        Dim now As DateTime = DateTime.Now
        'StartMinutesInDay以上になるように補正した
        '現在時刻を（0時0分からの経過分の形式で）求める。
        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
        If Config.LineErrorRecordingStartMinutesInDay > nowMinutesInDay Then
            nowMinutesInDay += 24 * 60
        End If

        '有効時間帯のみ登録を行う。
        If nowMinutesInDay <= Config.LineErrorRecordingEndMinutesInDay Then
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, selfEkCode), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtNkanLineError.Gen())
        End If
    End Sub
#End Region

End Class

''' <summary>
''' 回線状態。
''' </summary>
Public Enum LineStatus As Integer
    Initial
    Steady
    Disconnected
End Enum
