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

Imports System.Globalization
Imports System.IO
Imports System.Messaging
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 駅務機器と電文の送受信を行うクラス。
''' </summary>
Public Class TelServerAppTelegrapher
    Inherits ServerTelegrapher

#Region "ユーティリティメソッド"
    'NOTE: 次世代車補対応にて、各種シーケンスの仕様記述用のクラスは、
    'Publicに変更して、このクラスの外で定義するようにした。
    '-------Ver0.1 次世代車補対応 DEL START-----------
    '-------Ver0.1 次世代車補対応 DEL END-------------

    Protected Shared Function GenCplxObjCode(ByVal objCode As Integer, ByVal subObjCode As Integer) As UShort
        Return CUShort(objCode << 8 Or subObjCode)
    End Function
#End Region

#Region "定数や変数"
    '各種テーブル共通の項目にセットする値
    Protected Const UserId As String = "System"
    Protected Const MachineId As String = "Server"

    'スレッド別ディレクトリ名の書式
    Protected Const sDirNameFormat As String = "%3R%3S_%4C_%2U"

    '一時ファイルの名前
    Protected Const sTempFileName As String = "ReceivedData.bin"

    '一時作業用ディレクトリ名
    Protected sTempDirPath As String

    '電文書式
    Protected oTelegGene As EkTelegramGene

    '相手装置の装置コード
    'NOTE: ProcOnReqTelegramReceive()をフックして受信電文のClientCodeと比較してもよい。
    Protected clientCode As EkCode

    '通信相手の（DB仕様）機種コード
    Protected sClientModel As String

    '通信相手の（DB仕様）コネクション区分
    Protected sPortPurpose As String

    '-------Ver0.1 次世代車補対応 ADD START-----------
    '通信相手の駅名
    Protected sClientStationName As String

    '通信相手のコーナー名
    Protected sClientCornerName As String
    '-------Ver0.1 次世代車補対応 ADD END-------------

    'アクセスを許可するFTPサイト内仮想ファイルシステムのパス
    Protected sPermittedPathInFtp As String

    'アクセスを許可するローカルファイルシステムのパス
    Protected sPermittedPath As String

    '収集データ誤記テーブルに記録するための通信相手機種名称
    Protected sCdtClientModelName As String

    '収集データ誤記テーブルに記録するポート名称
    Protected sCdtPortName As String

    'ウォッチドッグの種別
    Protected formalObjCodeOfWatchdog As Integer

    '整時データ取得の種別
    Protected formalObjCodeOfTimeDataGet As Integer

    'マスタ/プログラム一式DLLの仕様
    Protected oMasProSuiteDllSpecOfDataKinds As Dictionary(Of String, TelServerAppMasProDllSpec)

    'マスタ/プログラム適用リストDLLの仕様
    Protected oMasProListDllSpecOfDataKinds As Dictionary(Of String, TelServerAppMasProDllSpec)

    '指定ファイルULLの仕様
    Protected oScheduledUllSpecOfDataKinds As Dictionary(Of String, TelServerAppScheduledUllSpec)

    'マスタ/プログラムDL完了通知の仕様
    'NOTE: KeyはObjCodeとSubObjCodeからGenCplxObjCodeメソッドで生成する。
    'なお、ObjCodeごとにSubObjCodeが0x00のレコードを必ず用意しなければならない。
    'プロトコル仕様にそのようなObjCodeとSubObjCode（0x00）の組み合わせが
    '存在しない場合、それがダミーレコードであることがわかるようにValueの
    'DataKindにはNothingを設定すること。
    Protected oMasProDlReflectSpecOfCplxObjCodes As Dictionary(Of UShort, TelServerAppMasProDlReflectSpec)

    'POST電文受信の仕様
    Protected oByteArrayPassivePostSpecOfObjCodes As Dictionary(Of Byte, TelServerAppByteArrayPassivePostSpec)

    'バージョン情報ULLの仕様
    Protected oVersionInfoUllSpecOfObjCodes As Dictionary(Of Byte, TelServerAppVersionInfoUllSpec)

    '利用データULLの仕様
    Protected oRiyoDataUllSpecOfObjCodes As Dictionary(Of Byte, TelServerAppRiyoDataUllSpec)

    '利用データ格納先ディレクトリ名
    Protected sRiyoDataInputDirPath As String
    Protected sRiyoDataRejectDirPath As String

    '現在実行中の利用データULLが正常終了すると仮定した場合の移動先フルパス名
    'NOTE: PassiveUllの多重受け入れを行わないことを前提にしているので注意。
    'もしServerTelegrapherが改造されて、多重受け入れを容認するようになったら、
    '任意数のファイル名をoPassiveXllQueueの各アイテムに紐づけて管理する必要がある。
    Protected sCurUllRiyoDataReservedInputPath As String

    '次に送信するREQ電文の通番
    Protected reqNumberForNextSnd As Integer

    '次に受信するREQ電文の通番
    'NOTE: ProcOnReqTelegramReceive()をフックして、受信したREQ電文の通番の
    '連続性等をチェックするなら用意する。
    'Protected reqNumberForNextRcv As Integer

    'NOTE: 「意図的な切断」と「異常による切断」を区別したいならば、
    'Protected needConnection As Booleanを用意し、
    'ProcOnConnectNoticeReceive()とProcOnDisconnectRequestReceive()をフックして
    'それをON/OFFするとよい。ProcOnConnectionDisappear()では、それをみて、
    '遷移先の回線状態を決めることができる。

    '疑似回線状態
    Protected pseudoLineStatus As LineStatus

    '疑似コネクション延長期間か否か（実質的な意味はコネクション観察期間か否か）
    Protected isPseudoConnectionProlongationPeriod As Boolean

    '疑似コネクション延長期間をつくるタイマ
    Protected oPseudoConnectionProlongationTimer As TickTimer

    '収集データ誤記テーブルに対する通信異常の重複登録禁止期間か否か
    Protected hidesLineErrorFromRecording As Boolean

    '収集データ誤記テーブルに対する通信異常の重複登録禁止期間をつくるタイマ
    Protected oLineErrorRecordingIntervalTimer As TickTimer

    '-------Ver0.1 次世代車補対応 ADD START-----------
    '通信異常の警報メールの重複生成禁止期間か否か
    Protected hidesLineErrorFromAlerting As Boolean

    '通信異常の警報メールの重複登録禁止期間をつくるタイマ
    Protected oLineErrorAlertingIntervalTimer As TickTimer
    '-------Ver0.1 次世代車補対応 ADD END-------------

    '初回接続タイマ
    'NOTE: 通信異常を検出するためのタイマである。
    '一度でも接続すれば、切断時に通信異常と認識できるため、
    '動作させる必要はなくなる。
    Protected oInitialConnectLimitTimerForLineError As TickTimer

    '-------Ver0.1 次世代車補対応 ADD START-----------
    '通信異常が始まった日時
    Protected lineErrorBeginingTime As DateTime

    '通信異常の警報メールの文言
    Protected lineErrorAlertMailSubject As Sentence
    Protected lineErrorAlertMailBody As Sentence
    '-------Ver0.1 次世代車補対応 ADD END-------------

    '回線状態
    Private _LineStatus As Integer
#End Region

#Region "コンストラクタ"
    '-------Ver0.1 次世代車補対応 MOD START-----------
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal oTelegImporter As EkTelegramImporter, _
       ByVal oTelegGene As EkTelegramGene, _
       ByVal clientCode As EkCode, _
       ByVal sClientModel As String, _
       ByVal sPortPurpose As String, _
       ByVal sCdtClientModelName As String, _
       ByVal sCdtPortName As String, _
       ByVal sClientStationName As String, _
       ByVal sClientCornerName As String, _
       ByVal lineErrorAlertMailSubject As Sentence, _
       ByVal lineErrorAlertMailBody As Sentence)

        MyBase.New(sThreadName, oParentMessageSock, oTelegImporter)
        Me.sTempDirPath = Path.Combine(TelServerAppBaseConfig.TemporaryBaseDirPath, clientCode.ToString(sDirNameFormat))
        Me.oTelegGene = oTelegGene
        Me.clientCode = clientCode
        Me.sClientModel = sClientModel
        Me.sPortPurpose = sPortPurpose
        Me.sClientStationName = sClientStationName
        Me.sClientCornerName = sClientCornerName
        Me.sPermittedPathInFtp = Path.Combine(TelServerAppBaseConfig.PermittedPathInFtp, clientCode.ToString(sDirNameFormat))
        Me.sPermittedPath = Utility.CombinePathWithVirtualPath(TelServerAppBaseConfig.FtpServerRootDirPath, sPermittedPathInFtp)
        Me.sCdtClientModelName = sCdtClientModelName
        Me.sCdtPortName = sCdtPortName
        Me.lineErrorAlertMailSubject = lineErrorAlertMailSubject
        Me.lineErrorAlertMailBody = lineErrorAlertMailBody

        'NOTE: MayOverride
        Me.formalObjCodeOfWatchdog = -1
        Me.formalObjCodeOfTimeDataGet = -1
        Me.oMasProSuiteDllSpecOfDataKinds = Nothing
        Me.oMasProListDllSpecOfDataKinds = Nothing
        Me.oScheduledUllSpecOfDataKinds = Nothing
        Me.oMasProDlReflectSpecOfCplxObjCodes = Nothing
        Me.oByteArrayPassivePostSpecOfObjCodes = Nothing
        Me.oVersionInfoUllSpecOfObjCodes = Nothing
        Me.oRiyoDataUllSpecOfObjCodes = Nothing

        Me.reqNumberForNextSnd = 0
        Me.pseudoLineStatus = LineStatus.Initial
        Me.isPseudoConnectionProlongationPeriod = False
        Me.oPseudoConnectionProlongationTimer = New TickTimer(TelServerAppBaseConfig.PseudoConnectionProlongationTicks)
        Me.hidesLineErrorFromRecording = If(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks <= 0, True, False)
        Me.oLineErrorRecordingIntervalTimer = If(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks <= 0, Nothing, New TickTimer(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks))
        Me.hidesLineErrorFromAlerting = If(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks <= 0, True, False)
        Me.oLineErrorAlertingIntervalTimer = If(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks <= 0, Nothing, New TickTimer(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks))
        Me.oInitialConnectLimitTimerForLineError = New TickTimer(TelServerAppBaseConfig.InitialConnectLimitTicksForLineError)
        Me.LineStatus = LineStatus.Initial

        Me.oWatchdogTimer.Renew(TelServerAppBaseConfig.WatchdogIntervalTicks)
        Me.telegReadingLimitBaseTicks = TelServerAppBaseConfig.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = TelServerAppBaseConfig.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = TelServerAppBaseConfig.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = TelServerAppBaseConfig.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = TelServerAppBaseConfig.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = TelServerAppBaseConfig.TelegLoggingMaxLengthOnWrite
        Me.enableXllStrongExclusion = TelServerAppBaseConfig.EnableXllStrongExclusion
        Me.enableActiveSeqStrongExclusion = TelServerAppBaseConfig.EnableActiveSeqStrongExclusion
        Me.enableActiveOneOrdering = TelServerAppBaseConfig.EnableActiveOneOrdering

        'NOTE: 利用データをRiyoDataTrashDirPathに移動する対Ｎ間通信プロセスがない（東海向けでない）
        '場合であっても、このプロセス自身でRiyoDataTrashDirPathに直接移動することはしない。
        '洗い替え処理が対Ｎ間通信プロセスの有無を意識して、削除対象ディレクトリを選択する。
        Dim sRiyoDataBaseDirPath As String = Utility.CombinePathWithVirtualPath(TelServerAppBaseConfig.RiyoDataDirPath, clientCode.ToString(TelServerAppBaseConfig.RiyoDataStationBaseDirNameFormat))
        Me.sRiyoDataInputDirPath = Utility.CombinePathWithVirtualPath(sRiyoDataBaseDirPath, TelServerAppBaseConfig.RiyoDataInputDirPathInStationBase)
        Me.sRiyoDataRejectDirPath = Utility.CombinePathWithVirtualPath(sRiyoDataBaseDirPath, TelServerAppBaseConfig.RiyoDataRejectDirPathInStationBase)

        'このTelegrapherが作業で使うディレクトリを初期化する。
        Log.Info("Initializing directory [" & sTempDirPath & "]...")
        Utility.DeleteTemporalDirectory(sTempDirPath)
        Directory.CreateDirectory(sTempDirPath)

        'FTPサーバ上の当該クライアント用ディレクトリについて、無ければ作成する。
        'NOTE: ディレクトリの作成自体は、通信開始時に行えるが、通信開始前でも
        '自分自身はディレクトリにアクセスすることがあるため、ここに必要である。
        Log.Info("Createing directory [" & sPermittedPath & "]...")
        Directory.CreateDirectory(sPermittedPath)

        '通信状態テーブルからコネクションを削除。
        Me.DeleteDirectConStatus()
    End Sub
    '-------Ver0.1 次世代車補対応 MOD END-------------
#End Region

#Region "プロパティ"
    'このプロパティは、親スレッドにおいても、ある瞬間（少し過去の瞬間）の
    '回線状態をユーザに表示する目的であれば、参照可能である。
    'このスレッドを特定の箇所で停止させた上で状態を取得する（その後、この
    'スレッドに対して任意の操作を行ってから、任意に再開させることができる）
    'わけではないため、呼び出しから戻った時点で、戻り値の回線状態が維持
    'されているとは限らない。そのかわり、高頻度で呼び出しても大した負荷が
    'かからない。また、一度「切断」になったら、親スレッドが新しいソケットを
    '渡さない限り他の状態に変化しないことを考慮すれば、親スレッドがこの
    'スレッドを制御する上でも利用できるケースはある。
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
    Protected Overrides Function CreateWatchdogReqTelegram() As IReqTelegram
        If formalObjCodeOfWatchdog = -1 Then Return Nothing
        Return New EkWatchdogReqTelegram(oTelegGene, formalObjCodeOfWatchdog, TelServerAppBaseConfig.WatchdogReplyLimitTicks)
    End Function

    Protected Overrides Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oPseudoConnectionProlongationTimer Then
            Return ProcOnPseudoDisconnectTime()
        End If

        If oTimer Is oLineErrorRecordingIntervalTimer Then
            Return ProcOnLineErrorRecordingTime()
        End If

        '-------Ver0.1 次世代車補対応 ADD START-----------
        If oTimer Is oLineErrorAlertingIntervalTimer Then
            Return ProcOnLineErrorAlertingTime()
        End If
        '-------Ver0.1 次世代車補対応 ADD END-------------

        If oTimer Is oInitialConnectLimitTimerForLineError Then
            Return ProcOnInitialConnectLimitTimeForLineError()
        End If

        Return MyBase.ProcOnTimeout(oTimer)
    End Function

    Protected Overridable Function ProcOnPseudoDisconnectTime() As Boolean
        Log.Info("Connection observation period ended.")

        isPseudoConnectionProlongationPeriod = False

        'pseudoLineStatusをLineStatusに一致させる。
        'NOTE: LineStatus = LineStatus.Steadyで
        'pseudoLineStatus <> LineStatus.Steadyというのはあり得ない。
        If LineStatus <> LineStatus.Steady Then
            'NOTE: このケースでpseudoLineStatusがLineStatus.Initialという
            'ことはあり得ない。タイマが開始されているということは、
            '実コネクションの切断があったということであり、
            'さらに前には実コネクションおよび疑似コネクションの
            '接続があったということになる。
            If pseudoLineStatus = LineStatus.Steady Then
                pseudoLineStatus = LineStatus.Disconnected
                Log.Error("Closing the pseudo connection...")
                ProcOnPseudoConnectionDisappear()
            End If
        End If
        Return True
    End Function

    Protected Overridable Function ProcOnLineErrorRecordingTime() As Boolean
        Log.Info("Line error recording time comes.")

        Debug.Assert(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks > 0)

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

    '-------Ver0.1 次世代車補対応 ADD START-----------
    Protected Overridable Function ProcOnLineErrorAlertingTime() As Boolean
        Log.Info("Line error alerting time comes.")

        Debug.Assert(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks > 0)

        If LineStatus = LineStatus.Steady Then
            hidesLineErrorFromAlerting = False
        Else
            '通信異常の警報メールを生成する。
            'NOTE: 警報メールでは、新たに生成されるメールが無いことを以て、
            '異常が復旧したとみなすことになるため、通信異常が発生している
            '限りは、定期的に新たなメールを生成しなければならない。
            EmitLineErrorMail()

            RegisterTimer(oLineErrorAlertingIntervalTimer, TickTimer.GetSystemTick())
            Debug.Assert(hidesLineErrorFromAlerting = True)
        End If
        Return True
    End Function
    '-------Ver0.1 次世代車補対応 ADD END-------------

    Protected Overridable Function ProcOnInitialConnectLimitTimeForLineError() As Boolean
        Log.Error("Initial connection limit time comes for line error.")
        '-------Ver0.1 次世代車補対応 ADD START-----------
        lineErrorBeginingTime = DateTime.Now
        '-------Ver0.1 次世代車補対応 ADD END-------------

        If Not hidesLineErrorFromRecording Then
            Debug.Assert(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks > 0)

            '収集データ誤記テーブルに通信異常を登録する。
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromRecording = True
        End If

        '-------Ver0.1 次世代車補対応 ADD START-----------
        If Not hidesLineErrorFromAlerting Then
            Debug.Assert(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks > 0)

            '通信異常の警報メールを生成する。
            EmitLineErrorMail()

            RegisterTimer(oLineErrorAlertingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromAlerting = True
        End If
        '-------Ver0.1 次世代車補対応 ADD END-------------

        Return True
    End Function

    Protected Overrides Function ProcOnParentMessageReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        '-------Ver0.1 次世代車補対応 MOD START-----------
        Select Case oRcvMsg.Kind
            Case ServerAppInternalMessageKind.NameChangeNotice
                Return ProcOnNameChangeNoticeReceive(oRcvMsg)
            Case ServerAppInternalMessageKind.MasProDllRequest
                Return ProcOnMasProDllRequestReceive(oRcvMsg)
            Case ServerAppInternalMessageKind.ScheduledUllRequest
                Return ProcOnScheduledUllRequestReceive(oRcvMsg)
            Case Else
                Return MyBase.ProcOnParentMessageReceive(oRcvMsg)
        End Select
        '-------Ver0.1 次世代車補対応 MOD END-------------
    End Function

    '-------Ver0.1 次世代車補対応 ADD START-----------
    Protected Overridable Function ProcOnNameChangeNoticeReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("NameChange notified by manager.")

        Dim oExt As NameChangeNoticeExtendPart = NameChangeNotice.Parse(oRcvMsg).ExtendPart
        sClientStationName = oExt.StationName
        sClientCornerName = oExt.CornerName
        Return True
    End Function
    '-------Ver0.1 次世代車補対応 ADD END-------------

    Protected Overridable Function ProcOnMasProDllRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("MasProDll requested by manager.")

        'NOTE: このTelegrapherは親スレッドと緊密な連携をとりながら、MasProDllRequestを
        '処理することにしている。具体的に、このTelegrapherは、あるMasProDllRequestに
        '起因するファイル転送が終わったと認識した（完了したまたは、諦めた）時点で、
        'それに対応するMasProDllResponseを親スレッドへ送信する。そうすることで、
        '親スレッドは、同時に行う能動的DLLの件数をコントロール可能になる。
        'なお、その結果として、このTelegrapherがMasProDllRequestに対して
        'MasProDllResponseを返信していない状況で、親スレッドが次の能動的DLLを
        '要求してくる（新たなMasProDllRequest等を送信してくる）ことは、
        'あり得なくなっている。

        'NOTE: oRcvMsgがプロセス内で生成されたものであることを考えると、
        'このチェックは冗長である。
        If oMasProSuiteDllSpecOfDataKinds Is Nothing OrElse _
           oMasProListDllSpecOfDataKinds Is Nothing Then
            Log.Fatal("I don't support MasProDll.")

            '親スレッドに応答を返信してメソッドを終了する。
            MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End If

        'NOTE: oRcvMsgがプロセス内で生成されたものであることを考えると、
        'このチェックは冗長である。
        Dim oExt As MasProDllRequestExtendPart = MasProDllRequest.Parse(oRcvMsg).ExtendPart
        If Not EkMasProListFileName.IsValid(oExt.ListFileName) Then
            Log.Fatal("The file name [" & oExt.ListFileName & "] is invalid as MasProListFileName.")

            '親スレッドに応答を返信してメソッドを終了する。
            MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End If

        Dim sDataKind As String = EkMasProListFileName.GetDataKind(oExt.ListFileName)
        Dim sDataFileName As String
        Dim sDataFileHashValue As String
        Dim oSpec As TelServerAppMasProDllSpec

        'NOTE: oRcvMsgがプロセス内で生成されたものであることを考えると、
        'これをTryブロックに入れるのは冗長である。
        Try
            If String.IsNullOrEmpty(oExt.DataFileName) Then
                sDataFileName = ""
                sDataFileHashValue = ""
                oSpec = oMasProListDllSpecOfDataKinds(sDataKind)
            Else
                sDataFileName = Path.Combine(sPermittedPathInFtp, oExt.DataFileName)
                sDataFileHashValue = oExt.DataFileHashValue
                oSpec = oMasProSuiteDllSpecOfDataKinds(sDataKind)
            End If
        Catch ex As KeyNotFoundException
            Log.Fatal("I don't support the DataKind [" & sDataKind & "].")

            '親スレッドに応答を返信してメソッドを終了する。
            MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End Try

        'MasProDllRequestで指定されたファイルをFTPサーバ上にコピーする。
        'NOTE: 同一種別・同一バージョンの配信指示がオーバーラップすることは
        'あり得ない（既に配信中になっていれば、対運管端末通信プロセスが
        '運管端末に「ビジー」を返すことになっている）。そのため、コピー先に
        '存在している別の（転送中の）ファイルと名前が衝突することはあり得ない。
        'なお、対運管端末通信プロセスによる配慮が無いと仮定しても、この
        'Telegrapherの親スレッドは、同一のTelegrapherに対し、先に依頼した
        '配信が終了しない限り、次の配信は依頼しないので、FTPサーバ上の
        'ファイル名が衝突することはあり得ない。

        If Not String.IsNullOrEmpty(oExt.DataFileName) Then
            Dim sDataSrcPath As String = Path.Combine(TelServerAppBaseConfig.MasProDirPath, oExt.DataFileName)
            Dim sDataDstPath As String = Path.Combine(sPermittedPath, oExt.DataFileName)
            File.Copy(sDataSrcPath, sDataDstPath, True)
        End If

        Dim sListSrcPath As String = Path.Combine(TelServerAppBaseConfig.MasProDirPath, oExt.ListFileName)
        Dim sListDstPath As String = Path.Combine(sPermittedPath, oExt.ListFileName)
        File.Copy(sListSrcPath, sListDstPath, True)

        If Not String.IsNullOrEmpty(oExt.DataFileName) Then
            'DLLバージョン情報テーブルの該当レコードの不明フラグにTrueを設定する。
            'NOTE: このタイミングで「不明」にしてしまうと、キューイングされている間に
            '通信異常が発生した場合に「不明」のままになってしまう（次回の同一バージョンの
            '配信指示でデータ本体の送信が必要になってしまう）ため、少しもったいない。
            'しかし、これの派生クラスにおいて、RegisterActiveDllしたものが即座に
            '実行されないのは稀であると思われるし、共通化のためには致し方ない。
            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()
                UpdateDllVersionUncertainFlag(dbCtl, oExt.ListFileName, "1")
                dbCtl.TransactionCommit()
            Catch ex As DatabaseException
                dbCtl.TransactionRollBack()
                Throw
            Catch ex As Exception
                dbCtl.TransactionRollBack()
                Throw New DatabaseException(ex)
            Finally
                dbCtl.ConnectClose()
            End Try
        End If

        Dim oXllReqTeleg As New EkMasProDllReqTelegram( _
           oTelegGene, _
           oSpec.ObjCode, _
           oSpec.SubObjCode, _
           ContinueCode.Start, _
           sDataFileName, _
           sDataFileHashValue, _
           Path.Combine(sPermittedPathInFtp, oExt.ListFileName), _
           oExt.ListFileHashValue, _
           0, 0, 0, _
           oSpec.TransferLimitTicks, _
           oSpec.StartReplyLimitTicks)

        RegisterActiveDll( _
           oXllReqTeleg, _
           oSpec.RetryIntervalTicks, _
           oSpec.MaxRetryCountToForget + 1, _
           oSpec.MaxRetryCountToCare + 1)
        Return True
    End Function

    '能動的DLLが成功した（ContinueCode.Finishの転送終了REQ電文を受信した）場合
    Protected Overrides Sub ProcOnActiveDllComplete(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Info("MasProDll completed.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            Dim appUnitTable As DataTable = SelectApplicableUnits(dbCtl, sListFileName)

            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL状態テーブルを更新する。
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusNormal)

                'DLLバージョンテーブルを更新する。
                UpdateOrInsertDllVersion(dbCtl, sListFileName)
            End If

            'DLL状態テーブルを更新する。
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusNormal)

            'DL状態テーブルの関連レコードを「配信中」に変更する。
            UpdateDlStatusToExecutingIfNeeded(dbCtl, appUnitTable, sListFileName)

            'バージョン情報期待値テーブルを更新する。
            'NOTE: DLL状態を配信中以外にしたことで、下記で参照するマスタや
            'プログラムの登録情報が他のスレッドで上書きされると、正しい
            '動作は期待できない。トランザクションをコミットするまでは
            '他のコネクションからは古い状態がみえるようにDBを設定しておく
            'ことが必須であることに注意。
            Dim sSureDataFileName As String = SelectMasProDataFileName(dbCtl, sListFileName)
            If EkMasProListFileName.GetDataPurpose(sListFileName).Equals(EkConstants.DataPurposeMaster) Then
                DeleteAndInsertMasterVersionInfoExpected(dbCtl, appUnitTable, sSureDataFileName)
            Else
                DeleteAndInsertProgramVersionInfoExpected(dbCtl, appUnitTable, sSureDataFileName)
            End If

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        If Not String.IsNullOrEmpty(sDataFileName) Then
            'FTPサーバ上に置いたファイルを削除する。
            File.Delete(Path.Combine(sPermittedPath, sDataFileName))
        End If

        'FTPサーバ上に置いたファイルを削除する。
        File.Delete(Path.Combine(sPermittedPath, sListFileName))

        '親スレッドに応答を返信する。
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的DLLが成功した（ContinueCode.FinishWithoutStoringの転送終了REQ電文を受信した）場合
    Protected Overrides Sub ProcOnActiveDllCompleteWithoutStoring(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("MasProDll failed by content error.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Log.Info("The client says [" _
           & oXllReqTeleg.ResultantVersionOfSlot1.ToString("D8") & ", " _
           & oXllReqTeleg.ResultantVersionOfSlot2.ToString("D8") & ", " _
           & oXllReqTeleg.ResultantFlagOfFull.ToString("X2") & "].")

        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL状態テーブルを更新する。
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusContentError)

                'DLLバージョンテーブルを更新する（不明フラグをFalseに戻すだけ）。
                UpdateDllVersionUncertainFlag(dbCtl, sListFileName, "0")
            End If

            'DLL状態テーブルを更新する。
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusContentError)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        If Not String.IsNullOrEmpty(sDataFileName) Then
            'FTPサーバ上に置いたファイルを削除する。
            File.Delete(Path.Combine(sPermittedPath, sDataFileName))
        End If

        'FTPサーバ上に置いたファイルを削除する。
        File.Delete(Path.Combine(sPermittedPath, sListFileName))

        '親スレッドに応答を返信する。
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的DLLにてクライアントから転送失敗または転送化けを通知された（ContinueCode.Abortの転送終了REQ電文を受信した）場合
    Protected Overrides Sub ProcOnActiveDllAbort(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("MasProDll failed by transfer error.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Log.Info("The client says [" _
           & oXllReqTeleg.ResultantVersionOfSlot1.ToString("D8") & ", " _
           & oXllReqTeleg.ResultantVersionOfSlot2.ToString("D8") & ", " _
           & oXllReqTeleg.ResultantFlagOfFull.ToString("X2") & "].")

        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL状態テーブルを更新する。
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

                'DLLバージョンテーブルを更新する（不明フラグをFalseに戻すだけ）。
                UpdateDllVersionUncertainFlag(dbCtl, sListFileName, "0")
            End If

            'DLL状態テーブルを更新する。
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        If Not String.IsNullOrEmpty(sDataFileName) Then
            'FTPサーバ上に置いたファイルを削除する。
            File.Delete(Path.Combine(sPermittedPath, sDataFileName))
        End If

        'FTPサーバ上に置いたファイルを削除する。
        File.Delete(Path.Combine(sPermittedPath, sListFileName))

        '親スレッドに応答を返信する。
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的DLLにて転送終了REQ電文待ちのタイムアウトが発生した場合
    Protected Overrides Sub ProcOnActiveDllTimeout(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("MasProDll failed by transfer timeout.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL状態テーブルを更新する。
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusTimeout)

                'NOTE: この状況では、バージョン情報（サーバ）テーブルの不明フラグは、
                'Trueのままにしておくべきである。
            End If

            'DLL状態テーブルを更新する。
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusTimeout)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        'NOTE: この状況では、FTPサーバ上に置いたファイルを削除するべきではない。

        '親スレッドに応答を返信する。
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的DLLの開始で異常とみなすべきでないリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveDllRetryOverToForget(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        'NOTE: あり得ないと思われるが、相手が返してくるNAK次第であるため、用意しておく。
        '本当にあり得ないものと扱うには、GetRequirement()にて、
        '能動的DLLに関するEkNakCauseCode.NoDataなNAKは切断扱いにするとよい。
        ProcOnActiveDllRetryOverToCare(iXllReqTeleg, iNakTeleg)
    End Sub

    '能動的DLLの開始で異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveDllRetryOverToCare(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Log.Error("MasProDll failed by retry over.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL状態テーブルを更新する。
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

                'DLLバージョンテーブルを更新する（不明フラグをFalseに戻すだけ）。
                UpdateDllVersionUncertainFlag(dbCtl, sListFileName, "0")
            End If

            'DLL状態テーブルを更新する。
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        If Not String.IsNullOrEmpty(sDataFileName) Then
            'FTPサーバ上に置いたファイルを削除する。
            File.Delete(Path.Combine(sPermittedPath, sDataFileName))
        End If

        'FTPサーバ上に置いたファイルを削除する。
        File.Delete(Path.Combine(sPermittedPath, sListFileName))

        '親スレッドに応答を返信する。
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的DLLの最中やキューイングされた能動的DLLの開始前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveDllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("MasProDll failed by telegramming error.")

        Dim oXllReqTeleg As EkMasProDllReqTelegram = DirectCast(iXllReqTeleg, EkMasProDllReqTelegram)
        Dim sDataFileName As String = Path.GetFileName(oXllReqTeleg.DataFileName)
        Dim sListFileName As String = Path.GetFileName(oXllReqTeleg.ListFileName)

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            If Not String.IsNullOrEmpty(sDataFileName) Then
                'DLL状態テーブルを更新する。
                UpdateDllStatusForData(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

                'NOTE: この状況では、バージョン情報（サーバ）テーブルの不明フラグは、
                'Trueのままにしておくべきである。転送終了REQ電文を受信した際に、
                'その電文書式でエラーを検出してしまったケースを想定してのことである。
                'その際、相手装置は受信したファイル自体は有効なものとして扱っている
                'はずである。
            End If

            'DLL状態テーブルを更新する。
            UpdateDllStatusForList(dbCtl, sListFileName, DbConstants.DllStatusAbnormal)

            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        'NOTE: この状況では、FTPサーバ上に置いたファイルを削除するべきではない。

        '親スレッドに応答を返信する。
        MasProDllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    Protected Overridable Function ProcOnScheduledUllRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("ScheduledUll requested by manager.")

        'NOTE: このTelegrapherは親スレッドと緊密な連携をとりながら、ScheduledUllRequestを
        '処理することにしている。具体的に、このTelegrapherは、あるScheduledUllRequestに
        '起因するファイル転送が終わったと認識した（完了したまたは、諦めた）時点で、
        'それに対応するScheduledUllResponseを親スレッドへ送信する。そうすることで、
        '親スレッドは、同時に行う能動的ULLの件数をコントロール可能になる。
        'なお、その結果として、このTelegrapherがScheduledUllRequestに対して
        'ScheduledUllResponseを返信していない状況で、親スレッドが次の能動的ULLを
        '要求してくる（新たなScheduledUllRequest等を送信してくる）ことは、
        'あり得なくなっている。

        'NOTE: oRcvMsgがプロセス内で生成されたものであることを考えると、
        'このチェックは冗長である。
        If oScheduledUllSpecOfDataKinds Is Nothing Then
            Log.Fatal("I don't support ScheduledUll.")

            '親スレッドに応答を返信してメソッドを終了する。
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End If

        'NOTE: oRcvMsgがプロセス内で生成されたものであることを考えると、
        'このチェックは冗長である。
        Dim oExt As ScheduledUllRequestExtendPart = ScheduledUllRequest.Parse(oRcvMsg).ExtendPart
        If Not EkScheduledDataFileName.IsValid(oExt.FileName) Then
            Log.Fatal("The file name [" & oExt.FileName & "] is invalid as ScheduledDataFileName.")

            '親スレッドに応答を返信してメソッドを終了する。
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End If

        Dim sDataKind As String = EkScheduledDataFileName.GetKind(oExt.FileName)
        Dim oSpec As TelServerAppScheduledUllSpec

        'NOTE: oRcvMsgがプロセス内で生成されたものであることを考えると、
        'これをTryブロックに入れるのは冗長である。
        Try
            oSpec = oScheduledUllSpecOfDataKinds(sDataKind)
        Catch ex As KeyNotFoundException
            Log.Fatal("I don't support the DataKind [" & sDataKind & "].")

            '親スレッドに応答を返信してメソッドを終了する。
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return True
        End Try

        Dim oXllReqTeleg As New EkServerDrivenUllReqTelegram( _
           oTelegGene, _
           oSpec.ObjCode, _
           ContinueCode.Start, _
           Path.Combine(sPermittedPathInFtp, oExt.FileName), _
           oSpec.TransferLimitTicks, _
           oSpec.StartReplyLimitTicks)

        RegisterActiveUll( _
           oXllReqTeleg, _
           oSpec.RetryIntervalTicks, _
           oSpec.MaxRetryCountToForget + 1, _
           oSpec.MaxRetryCountToCare + 1)
        Return True
    End Function

    '能動的ULLが成功した（受信済みのハッシュ値と受信完了したファイルの内容が整合していることを確認した）
    '（転送終了REQ電文に対しACK電文を返信することになる）場合
    Protected Overrides Function ProcOnActiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Log.Info("ScheduledUll completed.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)

        If File.Exists(sFilePath) Then
            Dim oSpec As TelServerAppScheduledUllSpec = oScheduledUllSpecOfDataKinds(EkScheduledDataFileName.GetKind(sFileName))
            Dim sDstPath As String = UpboundDataPath.Gen(TelServerAppBaseConfig.InputDirPathForApps(oSpec.RecAppIdentifier), clientCode, DateTime.Now)
            If UpboundDataPath.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.MaxBranchNumberForApps(oSpec.RecAppIdentifier) Then
                'FTPサーバ上のファイルを登録プロセスが読み取るパスに移動する。
                File.Move(sFilePath, sDstPath)

                '登録プロセスに通知する。
                TelServerAppBaseConfig.MessageQueueForApps(oSpec.RecAppIdentifier).Send(New ExtFileCreationNotice())
            Else
               'FTPサーバ上のファイルを削除する。
                File.Delete(sFilePath)
                Log.Warn("File deleted.")
            End If

            '親スレッドに応答を返信する。
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return EkNakCauseCode.None
        Else
            '転送されてきたはずのファイルが無い場合
            'NOTE: その場合、事前にハッシュ値のエラーとなっているはずである
            'ため、ここが実行されることは基本的にないはずであるが、念のため
            'それなりの実装をしておく。

            '収集データ誤記テーブルに収集失敗を登録する。
            InsertScheduledUllFailureToCdt(sFileName)

            '親スレッドに応答を返信する。
            ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
            Return EkNakCauseCode.HashValueError 'NOTE: 微妙
        End If
    End Function

    '能動的ULLにて転送化けを検出した（受信済みのハッシュ値と受信完了したファイルの内容に不整合を検出した）
    '（転送終了REQ電文に対しハッシュ値の不一致を示すNAK電文を返信することになる）場合
    Protected Overrides Function ProcOnActiveUllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Log.Error("ScheduledUll failed by hash value error.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)

        '収集データ誤記テーブルに収集失敗を登録する。
        InsertScheduledUllFailureToCdt(sFileName)

        If File.Exists(sFilePath) Then
            Dim oSpec As TelServerAppScheduledUllSpec = oScheduledUllSpecOfDataKinds(EkScheduledDataFileName.GetKind(sFileName))
            Dim sDstPath As String = UpboundDataPath.Gen(TelServerAppBaseConfig.RejectDirPathForApps(oSpec.RecAppIdentifier), clientCode, DateTime.Now)
            If UpboundDataPath.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.MaxBranchNumberForApps(oSpec.RecAppIdentifier) Then
                'FTPサーバ上のファイルを破損データ用パスに移動する。
                File.Move(sFilePath, sDstPath)
            Else
                'FTPサーバ上のファイルを削除する。
                File.Delete(sFilePath)
                Log.Warn("File deleted.")
            End If
        End If

        '親スレッドに応答を返信する。
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
        Return EkNakCauseCode.HashValueError
    End Function

    '能動的ULLにてクライアントから転送失敗を通知された（ContinueCode.Abortの転送終了REQ電文を受信した）場合
    Protected Overrides Sub ProcOnActiveUllAbort(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("ScheduledUll failed by transfer error.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

        '収集データ誤記テーブルに収集失敗を登録する。
        InsertScheduledUllFailureToCdt(sFileName)

        'FTPサーバ上に残ったファイルがあれば削除する。
        File.Delete(Path.Combine(sPermittedPath, sFileName))

        '親スレッドに応答を返信する。
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLの開始で異常とみなすべきでないリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveUllRetryOverToForget(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Log.Info("ScheduledUll skipped by retry over to forget.")

        'NOTE: この場合、収集データ誤記テーブルに収集失敗は登録しない。

        '親スレッドに応答を返信する。
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLの開始で異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveUllRetryOverToCare(ByVal iXllReqTeleg As IXllReqTelegram, ByVal iNakTeleg As INakTelegram)
        Log.Error("ScheduledUll failed by retry over to care.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

        '収集データ誤記テーブルに収集失敗を登録する。
        InsertScheduledUllFailureToCdt(sFileName)

        '親スレッドに応答を返信する。
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLにて転送終了REQ電文待ちのタイムアウトが発生した場合
    Protected Overrides Sub ProcOnActiveUllTimeout(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("ScheduledUll failed by transfer timeout.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

        '収集データ誤記テーブルに収集失敗を登録する。
        InsertScheduledUllFailureToCdt(sFileName)

        'NOTE: この状況では、FTPサーバ上にファイルが残っていても、削除するべきではない。

        '親スレッドに応答を返信する。
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    '能動的ULLの最中やキューイングされた能動的ULLの開始前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        Log.Error("ScheduledUll failed by telegramming error.")

        Dim oXllReqTeleg As EkServerDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkServerDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)

        '収集データ誤記テーブルに収集失敗を登録する。
        InsertScheduledUllFailureToCdt(sFileName)

        'NOTE: この状況では、FTPサーバ上にファイルが残っていても、削除するべきではない。

        '親スレッドに応答を返信する。
        ScheduledUllResponse.Gen().WriteToSocket(oParentMessageSock)
    End Sub

    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)
        Dim objCode As Integer = oRcvTeleg.ObjCode
        Select Case oRcvTeleg.SubCmdCode
            Case EkSubCmdCode.Get
                If objCode = formalObjCodeOfTimeDataGet Then
                    Return ProcOnTimeDataGetReqTelegramReceive(oRcvTeleg)
                End If

                If oMasProDlReflectSpecOfCplxObjCodes IsNot Nothing AndAlso _
                   oMasProDlReflectSpecOfCplxObjCodes.ContainsKey(GenCplxObjCode(objCode, 0)) Then
                    Return ProcOnMasProDlReflectReqTelegramReceive(oRcvTeleg)
                End If

            Case EkSubCmdCode.Post
                If oByteArrayPassivePostSpecOfObjCodes IsNot Nothing AndAlso _
                   oByteArrayPassivePostSpecOfObjCodes.ContainsKey(CByte(objCode)) Then
                    Return ProcOnByteArrayPostReqTelegramReceive(oRcvTeleg)
                End If
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(iRcvTeleg)
    End Function

    Protected Overridable Function ProcOnTimeDataGetReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkTimeDataGetReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("TimeDataGet REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("TimeDataGet REQ received.")

        Dim oReplyTeleg As EkTimeDataGetAckTelegram = oRcvTeleg.CreateAckTelegram(DateTime.Now)
        Log.Info("Sending TimeDataGet ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnMasProDlReflectReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkMasProDlReflectReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("MasProDlReflect REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Dim oSpec As TelServerAppMasProDlReflectSpec = oMasProDlReflectSpecOfCplxObjCodes(GenCplxObjCode(oRcvTeleg.ObjCode, oRcvTeleg.SubObjCode))
        If oSpec.DataKind Is Nothing Then
            Log.Error("MasProDlReflect REQ with invalid SubObjCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg)
            Return True
        End If

        Log.Info("MasProDlReflect REQ received.")

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            UpdateOrInsertDlStatus(dbCtl, oRcvTeleg)
            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        Dim oReplyTeleg As EkMasProDlReflectAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending MasProDlReflect ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnByteArrayPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkByteArrayPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("ByteArrayPost REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("ByteArrayPost REQ received.")

        Dim oSpec As TelServerAppByteArrayPassivePostSpec = oByteArrayPassivePostSpecOfObjCodes(CByte(oRcvTeleg.ObjCode))
        Dim sDstPath As String = UpboundDataPath.Gen(TelServerAppBaseConfig.InputDirPathForApps(oSpec.RecAppIdentifier), clientCode, DateTime.Now)
        If UpboundDataPath.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.MaxBranchNumberForApps(oSpec.RecAppIdentifier) Then
            '一時作業用ディレクトリでファイル化する。
            Dim sTmpPath As String = Path.Combine(sTempDirPath, sTempFileName)
            Try
                Using oStream As New FileStream(sTmpPath, FileMode.Create, FileAccess.Write)
                    Dim aBytes As Byte() = oRcvTeleg.ByteArray
                    oStream.Write(aBytes, 0, aBytes.Length)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                'NOTE: 一応、ランタイムな条件次第で発生する例外もあるので、
                'どうするのがベストかよく考えた方がよい。
                Abort()
            End Try

            '作成したファイルを登録プロセスが読み取るパスに移動する。
            File.Move(sTmpPath, sDstPath)

            '登録プロセスに通知する。
            TelServerAppBaseConfig.MessageQueueForApps(oSpec.RecAppIdentifier).Send(New ExtFileCreationNotice())
        Else
            Log.Warn("Ignored.")
        End If

        Dim oReplyTeleg As EkByteArrayPostAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending ByteArrayPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    'ヘッダ部の内容が受動的ULLのREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsPassiveUllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)

        If oTeleg.SubCmdCode <> EkSubCmdCode.Get Then Return False
        Dim objCode As Byte = CByte(oTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then Return True

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then Return True

        Return False
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsPassiveUllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        'NOTE: 派生クラスのIsPassiveUllReqが許可したObjCodeの電文については、
        '変換も派生クラスのParseAsPassiveUllReqで完了させている
        'という想定である。

        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        Dim objCode As Byte = CByte(oTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Dim oSpec As TelServerAppVersionInfoUllSpec = oVersionInfoUllSpecOfObjCodes(objCode)
            Return New EkClientDrivenUllReqTelegram(iTeleg, oSpec.TransferLimitTicks)
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Dim oSpec As TelServerAppRiyoDataUllSpec = oRiyoDataUllSpecOfObjCodes(objCode)
            Return New EkClientDrivenUllReqTelegram(iTeleg, oSpec.TransferLimitTicks)
        End If

        Debug.Fail("This case is impermissible.")
        Return Nothing
    End Function

    '受動的ULLの準備（予告されたファイルの受け入れ確認）を行うメソッド
    Protected Overrides Function PrepareToStartPassiveUll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        'NOTE: 派生クラスのIsPassiveUllReqが許可したObjCodeの電文については、
        '受け入れ確認も派生クラスのPrepareToStartPassiveUllで完了させている
        'という想定である。
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePathInFtp As String = oXllReqTeleg.FileName
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(TelServerAppBaseConfig.FtpServerRootDirPath, sFilePathInFtp)

        'NOTE: 「..\」等の混入を許すなら、sPermittedPathもsFilePathも正規化した方がよいかもしれない。
        If Not Utility.IsAncestPath(sPermittedPath, sFilePath) Then
            Log.Error("The telegram specifies illegal path [" & sFilePathInFtp & "].")
            Return EkNakCauseCode.NotPermit 'NOTE: 微妙
        End If

        Dim sFileName As String = Path.GetFileName(sFilePath)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Dim oSpec As TelServerAppVersionInfoUllSpec = oVersionInfoUllSpecOfObjCodes(objCode)
            If Not EkVersionInfoFileName.IsValid(sFileName) Then
                Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
                Return EkNakCauseCode.TelegramError 'NOTE: 微妙
            End If
            If Not EkVersionInfoFileName.GetDataApplicableModel(sFileName).Equals(oSpec.ApplicableModel) Then
                Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
                Return EkNakCauseCode.TelegramError 'NOTE: 微妙
            End If
            If Not EkVersionInfoFileName.GetDataPurpose(sFileName).Equals(oSpec.DataPurpose) Then
                Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
                Return EkNakCauseCode.TelegramError 'NOTE: 微妙
            End If
            Log.Info("Accepting the file [" & sFileName & "] as VersionInfoUll...")
            Return EkNakCauseCode.None
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Dim oSpec As TelServerAppRiyoDataUllSpec = oRiyoDataUllSpecOfObjCodes(objCode)
            If Not sFileName.Equals(oSpec.FileName, StringComparison.OrdinalIgnoreCase) Then
                Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
                Return EkNakCauseCode.TelegramError 'NOTE: 微妙
            End If

            '-------Ver0.1 次世代車補対応 MOD START-----------
            Dim sDstPath As String = UpboundDataPath2.Gen(sRiyoDataInputDirPath, clientCode, oSpec.FormatCode, DateTime.Now)
            If UpboundDataPath2.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.RiyoDataMaxBranchNumber Then
                Log.Info("Accepting the file [" & sFileName & "] as RiyoDataUll...")
                sCurUllRiyoDataReservedInputPath = sDstPath
                Return EkNakCauseCode.None
            Else
                Log.Warn("Branch number is now missing to accept the file [" & sFileName & "] as RiyoDataUll.")
                Return EkNakCauseCode.Busy
            End If
            '-------Ver0.1 次世代車補対応 MOD END-------------
        End If

        Debug.Fail("This case is impermissible.")
        Return EkNakCauseCode.TelegramError
    End Function

    '受動的ULLが成功した（受信済みのハッシュ値と受信完了したファイルの内容が整合していることを確認した）
    '（転送終了REQ電文に対しACK電文を返信することになる）場合
    Protected Overrides Function ProcOnPassiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        'NOTE: 派生クラスのIsPassiveUllReqが許可したObjCodeの電文については、
        '結果に応じた処置も派生クラスのメソッドで完了させているという想定である。
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Info("VersionInfoUll completed.")

            If File.Exists(sFilePath) Then
                Dim retValue As NakCauseCode = EkNakCauseCode.TelegramError
                Do
                    Dim dbCtl As New DatabaseTalker()
                    Try
                        dbCtl.ConnectOpen()
                        dbCtl.TransactionBegin()

                        Dim insertResult As NakCauseCode
                        If EkVersionInfoFileName.GetDataPurpose(sFileName).Equals(EkConstants.DataPurposeMaster) Then
                            insertResult = DeleteAndInsertMasterVersionInfo(dbCtl, sFilePath)
                        Else
                            insertResult = DeleteAndInsertProgramVersionInfo(dbCtl, sFilePath, oVersionInfoUllSpecOfObjCodes(objCode).GroupTitles)
                        End If
                        If insertResult <> EkNakCauseCode.None Then
                            retValue = insertResult
                            Exit Do
                        End If

                        dbCtl.TransactionCommit()
                        retValue = EkNakCauseCode.None

                    Catch ex As DatabaseException
                        'NOTE: 本当はファイルを削除したいし、NAKを返信する方が
                        '親切であるが、サーバプロセス内ではDatabaseExceptionも
                        '予期せぬ異常として統一的に扱うことにしている。
                        Throw
                    Catch ex As Exception
                        Throw New DatabaseException(ex)
                    Finally
                        If retValue <> EkNakCauseCode.None Then
                            dbCtl.TransactionRollBack()
                        End If
                        dbCtl.ConnectClose()
                    End Try
                Loop While False

                'NOTE: 本来、ここでFTPサーバ上のファイルを削除するのが自然であるが、
                'それは行わないこととする。テストなどの際、生の受信データを確認したい
                'ケースがあるためである。なお、実運用においては、このデータは最新の
                'ものだけに意味があり、より新しいものを駅務機器側でいくらでも
                '生成できることから、たとえ最新のものであっても、バックアップを
                'とるほどの価値はない。よって、ユニークな名前に改名して別の
                'ディレクトリに退避するといったことは行わない（テストなどの際は、
                'ユーザがFTPサーバ上からファイルをコピーすればよい）。
                Return retValue
            Else
                '転送されてきたはずのファイルが無い場合
                'NOTE: その場合、事前にハッシュ値のエラーとなっているはずである
                'ため、ここが実行されることは基本的にないはずであるが、念のため
                'それなりの実装をしておく。
                Log.Error("Where is the file?")

                'NOTE: 下記により通信異常が発生すると思われるため、
                '特別な異常の登録や通知は行わないことにしておく。
                Return EkNakCauseCode.HashValueError 'NOTE: 微妙
            End If
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Info("RiyoDataUll completed.")

            If File.Exists(sFilePath) Then
                Dim oFileInfo As New FileInfo(sFilePath)
                '-------Ver0.1 次世代車補対応 MOD START-----------
                Dim oSpec As TelServerAppRiyoDataUllSpec = oRiyoDataUllSpecOfObjCodes(objCode)
                If oFileInfo.Length Mod oSpec.RecordLen <> 0 Then
                    'サイズ不正の場合はNAKを返信する（Ｎ間に渡すときに困るため）。
                    'TODO: 相手装置が諦めないのであれば、同じことが繰り返される
                    'ことが危惧される。よって、本当にそうであれば、駅務機器側に
                    '責任があるとは言え、運管側でこのファイルを特別なディレクトリ
                    'に退避し、収集データ誤記テーブルに異常を登録してから、
                    'ACKを返す方がよい気がする。
                    Log.Error("The file size is invalid.")
                    Dim sDstPath As String = UpboundDataPath2.Gen(sRiyoDataRejectDirPath, clientCode, oSpec.FormatCode, DateTime.Now)
                    If UpboundDataPath2.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.RiyoDataMaxBranchNumber Then
                        'FTPサーバ上のファイルを破損データ用パスに移動する。
                        File.Move(sFilePath, sDstPath)
                    Else
                        'FTPサーバ上のファイルを削除する。
                        File.Delete(sFilePath)
                        Log.Warn("File deleted.")
                    End If
                    Return EkNakCauseCode.HashValueError 'TODO: 微妙すぎる（InvalidContent等を使いたい）
                Else
                    'FTPサーバ上のファイルを対Ｎ間通信プロセスが読み取るパスに移動する。
                    File.Move(sFilePath, sCurUllRiyoDataReservedInputPath)
                    Return EkNakCauseCode.None
                End If
                '-------Ver0.1 次世代車補対応 MOD END-------------
            Else
                '転送されてきたはずのファイルが無い場合
                'NOTE: その場合、事前にハッシュ値のエラーとなっているはずである
                'ため、ここが実行されることは基本的にないはずであるが、念のため
                'それなりの実装をしておく。
                Log.Error("Where is the file?")

                'NOTE: 下記により通信異常が発生すると思われるため、
                '特別な異常の登録や通知は行わないことにしておく。
                Return EkNakCauseCode.HashValueError 'NOTE: 微妙
            End If
        End If

        Debug.Fail("This case is impermissible.")
        Return EkNakCauseCode.TelegramError
    End Function

    '受動的ULLにて転送化けを検出した（受信済みのハッシュ値と受信完了したファイルの内容に不整合を検出した）
    '（転送終了REQ電文に対しハッシュ値の不一致を示すNAK電文を返信することになる）場合
    Protected Overrides Function ProcOnPassiveUllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        'NOTE: 派生クラスのIsPassiveUllReqが許可したObjCodeの電文については、
        '結果に応じた処置も派生クラスのメソッドで完了させているという想定である。
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("VersionInfoUll failed by hash value error.")

            'NOTE: この先も同じことが続くのではないかという心配は拭い去れないが、
            'こちらはNAKを返しているわけであるし、コネクションが切断されることで
            'ポート閉の収集データ登録は行われ、ユーザが気付くきっかけもあるため、
            '外部に対してそれ以上の配慮は行わないことにする。

            'NOTE: 本来、ここでFTPサーバ上のファイルを削除するのが自然であるが、
            'それは行わないこととする。テストなどの際、生の受信データを確認したい
            'ケースがあるためである。
            Return EkNakCauseCode.HashValueError
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("RiyoDataUll failed by hash value error.")

            'NOTE: この先も同じことが続くのではないかという心配は拭い去れないが、
            'こちらはNAKを返しているわけであるし、コネクションが切断されることで
            'ポート閉の収集データ登録は行われ、ユーザが気付くきっかけもあるため、
            '特に何も行わないことにする。

            'TODO: 相手装置が諦めないのであれば、下記処理は、単なる削除に
            '変更する方がよい気がする。
            If File.Exists(sFilePath) Then
                '-------Ver0.1 次世代車補対応 MOD START-----------
                Dim oSpec As TelServerAppRiyoDataUllSpec = oRiyoDataUllSpecOfObjCodes(objCode)
                Dim sDstPath As String = UpboundDataPath2.Gen(sRiyoDataRejectDirPath, clientCode, oSpec.FormatCode, DateTime.Now)
                If UpboundDataPath2.GetBranchNumber(sDstPath) <= TelServerAppBaseConfig.RiyoDataMaxBranchNumber Then
                    'FTPサーバ上のファイルを破損データ用パスに移動する。
                    File.Move(sFilePath, sDstPath)
                Else
                    'FTPサーバ上のファイルを削除する。
                    File.Delete(sFilePath)
                    Log.Warn("File deleted.")
                End If
                '-------Ver0.1 次世代車補対応 MOD END-------------
            End If

            Return EkNakCauseCode.HashValueError
        End If

        Debug.Fail("This case is impermissible.")
        Return EkNakCauseCode.TelegramError
    End Function

    '受動的ULLにてクライアントから転送失敗を通知された（ContinueCode.Abortの転送終了REQ電文を受信した）場合
    Protected Overrides Sub ProcOnPassiveUllAbort(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: 派生クラスのIsPassiveUllReqが許可したObjCodeの電文については、
        '結果に応じた処置も派生クラスのメソッドで完了させているという想定である。
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim sFilePath As String = Path.Combine(sPermittedPath, sFileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("VersionInfoUll failed by transfer error.")

            'NOTE: コネクションが切断されることでポート閉の収集データ登録は行われ、
            'ユーザが気付くきっかけはあるため、それ以上のことは特に行わないことにする。

            'NOTE: 本来、ここでFTPサーバ上のファイルを削除するのが自然であるが、
            'それは行わないこととする。正常系で移動や削除を行わないこととの
            '一貫性を保つためである。
            Return
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("RiyoDataUll failed by transfer error.")

            'NOTE: コネクションが切断されることでポート閉の収集データ登録は行われ、
            'ユーザが気付くきっかけはあるため、それ以上のことは特に行わないことにする。

            'FTPサーバ上に残ったファイルがあれば削除する。
            File.Delete(sFilePath)
            Return
        End If

        Debug.Fail("This case is impermissible.")
    End Sub

    '受動的ULLにて転送終了REQ電文待ちのタイムアウトが発生した場合
    Protected Overrides Sub ProcOnPassiveUllTimeout(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: 派生クラスのIsPassiveUllReqが許可したObjCodeの電文については、
        '結果に応じた処置も派生クラスのメソッドで完了させているという想定である。
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("VersionInfoUll failed by transfer timeout.")

            'NOTE: コネクションが切断されることでポート閉の収集データ登録は行われ、
            'ユーザが気付くきっかけはあるため、特に何も行わないことにする。

            'NOTE: この状況では、FTPサーバ上にある受信（受信中）ファイルを削除しない方がよい。
            Return
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("RiyoDataUll failed by transfer timeout.")

            'NOTE: コネクションが切断されることでポート閉の収集データ登録は行われ、
            'ユーザが気付くきっかけはあるため、特に何も行わないことにする。

            'NOTE: この状況では、FTPサーバ上にある受信（受信中）ファイルを削除しない方がよい。
            Return
        End If

        Debug.Fail("This case is impermissible.")
    End Sub

    '受動的ULLの最中やキューイングされた受動的ULLの開始前に通信異常を検出した場合
    Protected Overrides Sub ProcOnPassiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: 派生クラスのIsPassiveUllReqが許可したObjCodeの電文については、
        '結果に応じた処置も派生クラスのメソッドで完了させているという想定である。
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFileName As String = Path.GetFileName(oXllReqTeleg.FileName)
        Dim objCode As Byte = CByte(oXllReqTeleg.ObjCode)

        If oVersionInfoUllSpecOfObjCodes IsNot Nothing AndAlso _
           oVersionInfoUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("VersionInfoUll failed by telegramming error.")

            'NOTE: コネクションが切断されることでポート閉の収集データ登録は行われ、
            'ユーザが気付くきっかけはあるため、特に何も行わないことにする。

            'NOTE: この状況では、FTPサーバ上にある受信（受信中）ファイルを削除しない方がよい。
            Return
        End If

        If oRiyoDataUllSpecOfObjCodes IsNot Nothing AndAlso _
           oRiyoDataUllSpecOfObjCodes.ContainsKey(objCode) Then
            Log.Error("RiyoDataUll failed by telegramming error.")

            'NOTE: コネクションが切断されることでポート閉の収集データ登録は行われ、
            'ユーザが気付くきっかけはあるため、特に何も行わないことにする。

            'NOTE: この状況では、FTPサーバ上にある受信（受信中）ファイルを削除しない方がよい。
            Return
        End If

        Debug.Fail("This case is impermissible.")
    End Sub

    '親スレッドからコネクションを受け取った場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionAppear()
        '従来機の実装に合わせて、再接続時の送信REQ電文通番が0になるようにする。
        'NOTE: プロトコル仕様書には「起動時に初期化」とあるが、
        'プロトコル仕様書や相手装置からすれば、回線断は
        '装置（通信プロセス）再起動と同義と言えるため、
        'この実装はプロトコル仕様とも合致していると考えられる。
        reqNumberForNextSnd = 0
        LineStatus = LineStatus.Steady

        If pseudoLineStatus <> LineStatus.Steady Then
            Log.Info("Opening the pseudo connection...")
            pseudoLineStatus = LineStatus.Steady
            ProcOnPseudoConnectionAppear()
        End If

        UnregisterTimer(oInitialConnectLimitTimerForLineError)

        'FTPサーバ上の当該クライアント用ディレクトリを初期化する。
        'NOTE: 前回のコネクション終了時に残してしまった（シーケンスの途中で
        'こちらからタイムアウト等したために削除を保留にしていた）ファイルを
        '削除することが目的であるが、FTPサーバが転送の中止を認識できずに
        '握り続けているなら、それについては削除しない。
        '電文のポートに再接続してきたということは、クライアント自身は
        'ファイル転送を完了または中止しているはずであるが、転送の中止を
        'FTPサーバが認識していないというのは、十分にあり得ることである。
        '最悪、アプリ再起動時（おそらく24時間ごと）に、クリーンアップを行う
        'ため、ここでは何も削除しないという選択肢もあるが、使用リソースの
        '単調増加は防ぐに越したことはないため、可能な限りの内容物を削除する。
        'NOTE: FTPサーバは、握り続けているファイルをいずれ解放するはずで
        'あるが、その前に同名ファイルを再ULLしようとしたクライアント側で
        '致命的な問題が起きるようであるなら、削除できないファイルが
        'ある限り電文用のセッションを確立させない（Accept直後の
        'シーケンスでBUSYのNAKを返す）方がよいかもしれない。
        Log.Info("Cleaning up directory [" & sPermittedPath & "]...")
        Utility.CleanUpDirectory(sPermittedPath)
    End Sub

    'コネクションを切断した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionDisappear()
        '-------Ver0.1 次世代車補対応 ADD START-----------
        lineErrorBeginingTime = DateTime.Now
        '-------Ver0.1 次世代車補対応 ADD END-------------
        LineStatus = LineStatus.Disconnected

        If isPseudoConnectionProlongationPeriod Then
            'NOTE: pseudoLineStatusは正常側に転ぶようになっている。
            'よって、このケース（現実のConnectionがDisappearされ得るケース）では、
            'pseudoLineStatusも必ずSteadyである。
            Log.Error("Closing the pseudo connection because a connection closed during observation period...")
            pseudoLineStatus = LineStatus.Disconnected
            ProcOnPseudoConnectionDisappear()
        Else
            Log.Info("Starting connection observation period...")
            RegisterTimer(oPseudoConnectionProlongationTimer, TickTimer.GetSystemTick())
            isPseudoConnectionProlongationPeriod = True
        End If

        If Not hidesLineErrorFromRecording Then
            Debug.Assert(TelServerAppBaseConfig.LineErrorRecordingIntervalTicks > 0)

            '収集データ誤記テーブルに通信異常を登録する。
            InsertLineErrorToCdt()

            RegisterTimer(oLineErrorRecordingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromRecording = True
        End If

        '-------Ver0.1 次世代車補対応 ADD START-----------
        If Not hidesLineErrorFromAlerting Then
            Debug.Assert(TelServerAppBaseConfig.LineErrorAlertingIntervalTicks > 0)

            '通信異常の警報メールを生成する。
            EmitLineErrorMail()

            RegisterTimer(oLineErrorAlertingIntervalTimer, TickTimer.GetSystemTick())
            hidesLineErrorFromAlerting = True
        End If
        '-------Ver0.1 次世代車補対応 ADD END-------------
    End Sub

    '疑似コネクション開始
    Protected Overridable Sub ProcOnPseudoConnectionAppear()
        '接続のTRAP通知を行う。
        If TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus <> 0 Then
            SnmpTrap.Act( _
               TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus, _
               sClientModel, clientCode, _
               TelServerAppBaseConfig.IpPortForTelegConnection, _
               SnmpStatusCode.Connect)
        End If

        '通信状態テーブルにコネクションを登録。
        InsertDirectConStatus()
    End Sub

    '疑似コネクション終了
    Protected Overridable Sub ProcOnPseudoConnectionDisappear()
        '切断のTRAP通知を行う。
        If TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus <> 0 Then
            SnmpTrap.Act( _
               TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus, _
               sClientModel, clientCode, _
               TelServerAppBaseConfig.IpPortForTelegConnection, _
               SnmpStatusCode.Disconnect)
        End If

        '通信状態テーブルからコネクションを削除。
        DeleteDirectConStatus()
    End Sub

    Protected Overrides Sub ProcOnUnhandledException(ByVal exc As Exception)
        '切断のTRAP通知を行う。
        If TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus <> 0 Then
            SnmpTrap.Act( _
               TelServerAppBaseConfig.SnmpAppNumberForConnectionStatus, _
               sClientModel, clientCode, _
               TelServerAppBaseConfig.IpPortForTelegConnection, _
               SnmpStatusCode.Disconnect)
        End If

        Try
            '通信状態テーブルからコネクションを削除。
            DeleteDirectConStatus()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try

        MyBase.ProcOnUnhandledException(exc)
    End Sub
#End Region

#Region "イベント処理実装用メソッド"
    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        oReqTeleg.ReqNumber = reqNumberForNextSnd
        oReqTeleg.ClientCode = clientCode
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
        'NOTE: データ種別などで分岐することも可能。データ種別をみれば、
        '運管サーバがNAKを送信した場合なのか、クライアント機器がNAKを
        '送信した場合なのか判別することも可能。
        Select Case oNakTeleg.CauseCode
            '継続（リトライオーバー）しても異常とはみなせないNAK電文
            Case EkNakCauseCode.NoData, EkNakCauseCode.Unnecessary
                Return NakRequirement.ForgetOnRetryOver

            '継続（リトライオーバー）したら異常とみなすべきNAK電文
            Case EkNakCauseCode.Busy, EkNakCauseCode.NoTime, EkNakCauseCode.InvalidContent, EkNakCauseCode.UnknownLight
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

    Protected Overridable Sub DeleteDirectConStatus()
        Dim sSQL As String = _
           "DELETE FROM S_DIRECT_CON_STATUS" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
           & " AND UNIT_NO = " & clientCode.Unit.ToString() _
           & " AND PORT_KBN = '" & sPortPurpose & "'"
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Sub InsertDirectConStatus()
        Dim sSQL As String = _
           "INSERT INTO S_DIRECT_CON_STATUS" _
           & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
            & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, PORT_KBN, CONNECT_DATE)" _
           & " VALUES (GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " '" & sClientModel & "'," _
                   & " '" & clientCode.RailSection.ToString("D3") & "'," _
                   & " '" & clientCode.StationOrder.ToString("D3") & "'," _
                   & " " & clientCode.Corner.ToString() & "," _
                   & " " & clientCode.Unit.ToString() & "," _
                   & " '" & sPortPurpose & "'," _
                   & " GETDATE())"
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            Throw
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Function SelectApplicableUnits(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String) As DataTable
        Dim sDataAppModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataPurpose As String = EkMasProListFileName.GetDataPurpose(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim sDataSubKind As String = EkMasProListFileName.GetDataSubKind(sListFileName)
        Dim sDataVersion As string = EkMasProListFileName.GetDataVersion(sListFileName)
        Dim sListVersion As string = EkMasProListFileName.GetListVersion(sListFileName)
        Dim sClientRailSection As string = clientCode.RailSection.ToString("D3")
        Dim sClientStationOrder As string = clientCode.StationOrder.ToString("D3")
        Dim sClientCorner As string = clientCode.Corner.ToString()
        Dim sClientUnit As string = clientCode.Unit.ToString()

        '配信開始日時を取得。
        'NOTE: Clientが用いる日時は、現在日時の方が近いかもしれないが、
        'そもそも配信先のClientを決める際にこれを用いているため、
        'これを用いることにする。
        Dim sSQLToSelectDllStartTime As String = _
           "SELECT DELIVERY_START_TIME" _
           & " FROM S_" & sDataPurpose & "_DLL_STS" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
           & " AND DATA_KIND = '" & sDataKind & "'" _
           & " AND DATA_SUB_KIND = '" & sDataSubKind & "'" _
           & " AND DATA_VERSION = '" & sDataVersion & "'" _
           & " AND VERSION = '" & sListVersion & "'" _
           & " AND RAIL_SECTION_CODE = '" & sClientRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sClientStationOrder & "'" _
           & " AND CORNER_CODE = " & sClientCorner _
           & " AND UNIT_NO = " & sClientUnit
        Dim sDeliveryStartTime As String = CStr(dbCtl.ExecuteSQLToReadScalar(sSQLToSelectDllStartTime))
        Dim dllStartTime As DateTime = DateTime.ParseExact(sDeliveryStartTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)

        '共通部分テーブル（機器構成マスタの配信指示時点の有効要素）を定義するSQLを編集。
        Dim sSQLToDefineCTE As String = _
           "WITH M_SERVICE_MACHINE (MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, ADDRESS, MONITOR_ADDRESS)" _
           & " AS" _
           & " (SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, ADDRESS, MONITOR_ADDRESS" _
               & " FROM M_MACHINE" _
               & " WHERE SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                              & " FROM M_MACHINE" _
                                              & " WHERE SETTING_START_DATE <= '" & sDeliveryStartDate & "'" _
                                              & " AND INSERT_DATE <= CONVERT(DATETIME, '" & dllStartTime.ToString("yyyy/MM/dd HH:mm:ss") & "', 120))) "

        '配信指示の時点でのClientのIPアドレスを取得するSQLを編集。
        'NOTE: 機器構成マスタ登録時点のチェックにより全て同一であり、
        '配信を開始できたということで、少なくとも１つは存在している
        '（洗い替えバッチも無用な削除はしない）ものとする。
        Dim sSQLToSelectAddrOfClient As String = _
           "SELECT TOP 1 ADDRESS" _
           & " FROM M_SERVICE_MACHINE" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & sClientRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sClientStationOrder & "'" _
           & " AND CORNER_CODE = " & sClientCorner _
           & " AND UNIT_NO = " & sClientUnit _
           & " AND ADDRESS <> ''"

        '上記IPアドレスが接続先に指定されている配信指示の時点で有効な
        '機器構成マスタのレコードを取得するSQLを編集。
        Dim sSQLToSelectUnitsUnderClient As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM M_SERVICE_MACHINE" _
           & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
           & " AND MONITOR_ADDRESS = (" & sSQLToSelectAddrOfClient & ")"

        '適用先装置の線区～号機を取得するSQLを編集。
        Dim sSQLToSelectApplicableUnits As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM S_" & sDataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        'NOTE: プログラム適用リストの場合は、有効な行を抽出するにあたり、
        '適用日にもとづく追加の条件をもうけている。なお、ブランクは
        'どのような日付（数字列）よりも小さいとみなされる想定である。
        If sDataPurpose.Equals(EkConstants.DataPurposeProgram) Then
            sSQLToSelectApplicableUnits = sSQLToSelectApplicableUnits _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                    & " OR APPLICABLE_DATE = '19000101'" _
                    & " OR APPLICABLE_DATE = '99999999')"
        End If

        'Client配下の適用先装置の線区～号機を取得するSQLを実行。
        Dim sSQLToSelectApplicableUnitsUnderClient As String = _
           sSQLToSelectUnitsUnderClient & " INTERSECT " & sSQLToSelectApplicableUnits
        Return  dbCtl.ExecuteSQLToRead(sSQLToDefineCTE & sSQLToSelectApplicableUnitsUnderClient)
    End Function

    Protected Overridable Function SelectMasProDataFileName(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String) As String
        Dim sDataAppModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataPurpose As String = EkMasProListFileName.GetDataPurpose(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim sDataSubKind As String = EkMasProListFileName.GetDataSubKind(sListFileName)
        Dim sDataVersion As string = EkMasProListFileName.GetDataVersion(sListFileName)

        Dim sSQL As String = _
           "SELECT FILE_NAME" _
           & " FROM S_" & sDataPurpose & "_DATA_HEADLINE" _
           & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
           & " AND DATA_KIND = '" & sDataKind & "'" _
           & " AND DATA_SUB_KIND = '" & sDataSubKind & "'" _
           & " AND DATA_VERSION = '" & sDataVersion & "'"
        Return CStr(dbCtl.ExecuteSQLToReadScalar(sSQL))
    End Function

    'DLL状態テーブルのデータ本体用レコードを更新する。
    Protected Overridable Sub UpdateDllStatusForData(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String, ByVal status As Integer)
        Dim sDeliveryEndTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")

        Dim sSQL As String = _
           "UPDATE S_" & EkMasProListFileName.GetDataPurpose(sListFileName) & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_END_TIME = '" & sDeliveryEndTime & "'," _
               & " DELIVERY_STS = " & status.ToString() _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
           & " AND DATA_KIND = '" & EkMasProListFileName.GetDataKind(sListFileName) & "'" _
           & " AND DATA_SUB_KIND = '" & EkMasProListFileName.GetDataSubKind(sListFileName) & "'" _
           & " AND DATA_VERSION = '" & EkMasProListFileName.GetDataVersion(sListFileName) & "'" _
           & " AND VERSION = '" & EkMasProListFileName.GetDataVersion(sListFileName) & "'" _
           & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
           & " AND UNIT_NO = " & clientCode.Unit.ToString()
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    'DLL状態テーブルの適用リスト用レコードを更新する。
    Protected Overridable Sub UpdateDllStatusForList(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String, ByVal status As Integer)
        Dim sDeliveryEndTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")

        Dim sSQL As String = _
           "UPDATE S_" & EkMasProListFileName.GetDataPurpose(sListFileName) & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_END_TIME = '" & sDeliveryEndTime & "'," _
               & " DELIVERY_STS = " & status.ToString() _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
           & " AND DATA_KIND = '" & EkMasProListFileName.GetDataKind(sListFileName) & "'" _
           & " AND DATA_SUB_KIND = '" & EkMasProListFileName.GetDataSubKind(sListFileName) & "'" _
           & " AND DATA_VERSION = '" & EkMasProListFileName.GetDataVersion(sListFileName) & "'" _
           & " AND VERSION = '" & EkMasProListFileName.GetListVersion(sListFileName) & "'" _
           & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
           & " AND UNIT_NO = " & clientCode.Unit.ToString()
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    'DL状態テーブルを「配信中」に変更する。
    Protected Overridable Sub UpdateDlStatusToExecutingIfNeeded(ByVal dbCtl As DatabaseTalker, ByVal appUnitTable As DataTable, ByVal sListFileName As String)
        'sListFileNameが示す適用リストや機器構成に応じて、
        '配信先となる号機を「適用リスト」「データ本体」それぞれに
        'ついて導き出し、DL状態テーブルに当該レコードがまだ存在しない
        '場合のみ、レコードを追加する（状態は「配信中」とする）。

        Dim sDataAppModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataPurpose As String = EkMasProListFileName.GetDataPurpose(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim sDataSubKind As String = EkMasProListFileName.GetDataSubKind(sListFileName)
        Dim sDataVersion As string = EkMasProListFileName.GetDataVersion(sListFileName)
        Dim sListVersion As string = EkMasProListFileName.GetListVersion(sListFileName)
        Dim appUnits As DataRowCollection = appUnitTable.Rows

        'NOTE: 以下の点に注意。
        '(1) Clientは、マスタ適用リストについては、appUnitへ配信しない。
        '(2) Clientは、たとえパターン番号が違っていても、それ以外のキーが
        '    過去に送ったものと同じなら、マスタ本体についてもappUnitへ
        '    配信しない（appUnitが取りに来ない）。
        '(3) Clientは、プログラム適用リストについては、appUnitへ必ず配信
        '    する（配信しなければならない）。よって、新たな配信指示が
        '    あったからには、過去に配信が成功していることに意味はない。
        '    ただし、実際に配信が行われるまで、過去の結果が表示に残るのは
        '    致し方ない（配信完了日時の前後関係で、過去の結果とわかる）。
        '(4) プログラムの場合、マスタの場合と異なり、DATA_SUB_KINDは
        '    キーでない。

        'OPT: 基本的にここでINSERTを実行する必要はないはず。

        If sDataPurpose.Equals(EkConstants.DataPurposeMaster) Then
            For Each appUnit As DataRow In appUnits
                Dim sAppRailSection As String = appUnit.Field(Of String)("RAIL_SECTION_CODE")
                Dim sAppStationOrder As String = appUnit.Field(Of String)("STATION_ORDER_CODE")
                Dim sAppCorner As String = appUnit.Field(Of Integer)("CORNER_CODE").ToString()
                Dim sAppUnit As String = appUnit.Field(Of Integer)("UNIT_NO").ToString()

                Dim sSQLToSelectMstDlStsAboutAnySubKind As String = _
                   "SELECT *" _
                   & " FROM S_" & sDataPurpose & "_DL_STS" _
                   & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                   & " AND DATA_KIND = '" & sDataKind & "'" _
                   & " AND VERSION = '" & sDataVersion & "'" _
                   & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
                   & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
                   & " AND CORNER_CODE = " & sAppCorner _
                   & " AND UNIT_NO = " & sAppUnit _
                   & " AND DELIVERY_STS <> " & DbConstants.DlStatusPreExecuting.ToString()

                'マスタDL状態テーブルに新規の（配信結果が「配信中」の）レコードを
                '追加する（既存のレコードがあれば「配信中」変更する）SQLを実行。
                'NOTE: 当該号機に関して、たとえパターンNoが異なっても、種別と
                'バージョンが一致する配信結果が１件でも存在する場合は、対象外とする。
                Dim sSQLToUpdateMstDlStsAboutData As String = _
                   "MERGE INTO S_" & sDataPurpose & "_DL_STS AS Target" _
                   & " USING (SELECT '" & sDataAppModel & "' MODEL_CODE," _
                                 & " '" & EkConstants.FilePurposeData & "' FILE_KBN," _
                                 & " '" & sDataKind & "' DATA_KIND," _
                                 & " '" & sDataSubKind & "' DATA_SUB_KIND," _
                                 & " '" & sDataVersion & "' VERSION," _
                                 & " '" & sAppRailSection & "' RAIL_SECTION_CODE," _
                                 & " '" & sAppStationOrder & "' STATION_ORDER_CODE," _
                                 & " " & sAppCorner & " CORNER_CODE," _
                                 & " " & sAppUnit & " UNIT_NO) AS Source" _
                   & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                     & " AND Target.FILE_KBN = Source.FILE_KBN" _
                     & " AND Target.DATA_KIND = Source.DATA_KIND" _
                     & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
                     & " AND Target.VERSION = Source.VERSION" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED" _
                   & " AND NOT EXISTS (" & sSQLToSelectMstDlStsAboutAnySubKind & ") THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DELIVERY_STS = " & DbConstants.DlStatusExecuting.ToString() _
                   & " WHEN NOT MATCHED" _
                   & " AND NOT EXISTS (" & sSQLToSelectMstDlStsAboutAnySubKind & ") THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " DATA_SUB_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " Source.MODEL_CODE," _
                            & " Source.FILE_KBN," _
                            & " Source.DATA_KIND," _
                            & " Source.DATA_SUB_KIND," _
                            & " Source.VERSION," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateMstDlStsAboutData)
            Next appUnit
        Else
            For Each appUnit As DataRow In appUnits
                Dim sAppRailSection As String = appUnit.Field(Of String)("RAIL_SECTION_CODE")
                Dim sAppStationOrder As String = appUnit.Field(Of String)("STATION_ORDER_CODE")
                Dim sAppCorner As String = appUnit.Field(Of Integer)("CORNER_CODE").ToString()
                Dim sAppUnit As String = appUnit.Field(Of Integer)("UNIT_NO").ToString()

                Dim sSQLToSelectPrgDlSts As String = _
                   "SELECT *" _
                   & " FROM S_" & sDataPurpose & "_DL_STS" _
                   & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                   & " AND DATA_KIND = '" & sDataKind & "'" _
                   & " AND VERSION = '" & sDataVersion & "'" _
                   & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
                   & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
                   & " AND CORNER_CODE = " & sAppCorner _
                   & " AND UNIT_NO = " & sAppUnit _
                   & " AND DELIVERY_STS <> " & DbConstants.DlStatusPreExecuting.ToString()

                'プログラムDL状態テーブルに新規の（配信結果が「配信中」の）レコードを
                '追加する（既存のレコードがあれば「配信中」変更する）SQLを実行。

                'プログラムDL状態テーブルにプログラム本体に関する新規の
                '（配信結果が「配信中」の）レコードを追加する（既存の
                'レコードがあれば「配信中」変更する）SQLを実行。
                Dim sSQLToUpdatePrgDlStsAboutData As String = _
                   "MERGE INTO S_" & sDataPurpose & "_DL_STS AS Target" _
                   & " USING (SELECT '" & sDataAppModel & "' MODEL_CODE," _
                                 & " '" & EkConstants.FilePurposeData & "' FILE_KBN," _
                                 & " '" & sDataKind & "' DATA_KIND," _
                                 & " '" & sDataVersion & "' VERSION," _
                                 & " '" & sAppRailSection & "' RAIL_SECTION_CODE," _
                                 & " '" & sAppStationOrder & "' STATION_ORDER_CODE," _
                                 & " " & sAppCorner & " CORNER_CODE," _
                                 & " " & sAppUnit & " UNIT_NO) AS Source" _
                   & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                     & " AND Target.FILE_KBN = Source.FILE_KBN" _
                     & " AND Target.DATA_KIND = Source.DATA_KIND" _
                     & " AND Target.VERSION = Source.VERSION" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED" _
                   & " AND NOT EXISTS (" & sSQLToSelectPrgDlSts & ") THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DELIVERY_STS = " & DbConstants.DlStatusExecuting.ToString() _
                   & " WHEN NOT MATCHED" _
                   & " AND NOT EXISTS (" & sSQLToSelectPrgDlSts & ") THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " Source.MODEL_CODE," _
                            & " Source.FILE_KBN," _
                            & " Source.DATA_KIND," _
                            & " Source.VERSION," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdatePrgDlStsAboutData)

                'NOTE: 適用リストの場合、VERSIONには適用リストバージョンを入れる。
                'NOTE: 適用リストの場合、過去のものと同じVERSIONで内容の異なるリストを
                '配信することが、普通にあり得る。また、プログラム適用リストの場合、
                'appUnitはそれを必ずappUnitに配信する。よって、このケースでは、
                '同じVERSIONのレコードが既に存在している場合も、一度「配信中」に
                'した方がよいし、そうすることが可能である（最悪、配信完了日時の
                '前後関係で事象は解釈できるはずであるが、それだと分かり辛い）。
                'しかし、ここでは既にレコードが存在している場合は「配信中」にはしない。
                'DLLシーケンスが完了する（このメソッドが呼ばれる）前に、DL完了通知を
                '受信している（最終的な値でレコードが作成されている）可能性が皆無とは
                '言えないためである。そのかわり、運管端末からの配信指示を受け入れた
                '時点で（こちらのプロセスに配信指示メッセージを送信する前に）
                '当該レコードを削除しておけばよい。バージョンが１周した（しかも同じ
                'バージョンで新たな配信指示を行った）ということは、そのレコードは
                '保持対象外とみなすことができるので、仕様的な問題はない。
                'また、このプロセスにおける配信開始やDLLシーケンス終了時点で
                'レコードを削除するよりも自然な動作になる。それらの方法だと、運管端末
                'より配信指示を受け入れてからその時点までの間に配信を中止することに
                'なったら、DL状態列に過去の情報が表示されることになってしまう。

                'プログラムDL状態テーブルに適用リストに関する新規の
                '（配信結果が「配信中」の）レコードを追加する（既存の
                'レコードがあれば「配信中」変更する）SQLを実行。
                Dim sSQLToUpdatePrgDlStsAboutList As String = _
                   "MERGE INTO S_" & sDataPurpose & "_DL_STS AS Target" _
                   & " USING (SELECT '" & sDataAppModel & "' MODEL_CODE," _
                                 & " '" & EkConstants.FilePurposeList & "' FILE_KBN," _
                                 & " '" & sDataKind & "' DATA_KIND," _
                                 & " '" & sListVersion & "' VERSION," _
                                 & " '" & sAppRailSection & "' RAIL_SECTION_CODE," _
                                 & " '" & sAppStationOrder & "' STATION_ORDER_CODE," _
                                 & " " & sAppCorner & " CORNER_CODE," _
                                 & " " & sAppUnit & " UNIT_NO) AS Source" _
                   & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                     & " AND Target.FILE_KBN = Source.FILE_KBN" _
                     & " AND Target.DATA_KIND = Source.DATA_KIND" _
                     & " AND Target.VERSION = Source.VERSION" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DELIVERY_STS = " & DbConstants.DlStatusExecuting.ToString() _
                   & " WHEN NOT MATCHED THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " Source.MODEL_CODE," _
                            & " Source.FILE_KBN," _
                            & " Source.DATA_KIND," _
                            & " Source.VERSION," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdatePrgDlStsAboutList)
            Next appUnit
        End If
    End Sub

    'DL状態テーブルを更新する。
    'NOTE: DLLシーケンスの完了より先にDL完了通知を受信する可能性も皆無ではない
    'ため、レコードがない場合は新規に追加する（MERGEを使う）。
    Protected Overridable Sub UpdateOrInsertDlStatus(ByVal dbCtl As DatabaseTalker, ByVal oRcvTeleg As EkMasProDlReflectReqTelegram)
        Dim oSpec As TelServerAppMasProDlReflectSpec = oMasProDlReflectSpecOfCplxObjCodes(GenCplxObjCode(oRcvTeleg.ObjCode, oRcvTeleg.SubObjCode))

        '以下の情報をもとに一意となるレコードに対し、DELIVERY_STSにoRcvTeleg.EatResultをセットする。
        'oSpec.DataPurpose （テーブル名として使用）
        'oSpec.ApplicableModel
        'oSpec.FilePurpose
        'oSpec.DataKind
        'oRcvTeleg.PatternNumber （oSpec.DataPurposeがEkConstants.DataPurposeMasterの場合だけ使用）
        'oRcvTeleg.VersionNumber
        'oRcvTeleg.EatClientCode

        Dim sDeliveryEndTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")

        Dim sSQL As String
        If oSpec.DataPurpose.Equals(EkConstants.DataPurposeMaster) Then
            Dim sVerFormat As String
            If oSpec.FilePurpose.Equals(EkConstants.FilePurposeList) Then
                sVerFormat = "D2" 'NOTE: 今のところここが動作するプロトコルは無い。
            Else
                sVerFormat = "D3"
            End If

            '-------Ver0.1　フェーズ２　「適用済み」状態を追加　MOD START-----------
            sSQL = _
               "MERGE INTO S_" & oSpec.DataPurpose & "_DL_STS AS Target" _
               & " USING (SELECT '" & oSpec.ApplicableModel & "' MODEL_CODE," _
                             & " '" & oSpec.FilePurpose & "' FILE_KBN," _
                             & " '" & oSpec.DataKind & "' DATA_KIND," _
                             & " '" & oRcvTeleg.PatternNumber.ToString("D2") & "' DATA_SUB_KIND," _
                             & " '" & oRcvTeleg.VersionNumber.ToString(sVerFormat) & "' VERSION," _
                             & " '" & oRcvTeleg.EatClientCode.RailSection.ToString("D3") & "' RAIL_SECTION_CODE," _
                             & " '" & oRcvTeleg.EatClientCode.StationOrder.ToString("D3") & "' STATION_ORDER_CODE," _
                             & " " & oRcvTeleg.EatClientCode.Corner.ToString() & " CORNER_CODE," _
                             & " " & oRcvTeleg.EatClientCode.Unit.ToString() & " UNIT_NO) AS Source" _
               & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                 & " AND Target.FILE_KBN = Source.FILE_KBN" _
                 & " AND Target.DATA_KIND = Source.DATA_KIND" _
                 & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
                 & " AND Target.VERSION = Source.VERSION" _
                 & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                 & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                 & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                 & " AND Target.UNIT_NO = Source.UNIT_NO)" _
               & " WHEN MATCHED" _
               & " AND (" & oRcvTeleg.EatResult.ToString() & " <> " & DbConstants.DlStatusContinuingNormal.ToString() _
                 & " OR Target.DELIVERY_STS <> " & DbConstants.DlStatusNormal.ToString() & ") THEN" _
                & " UPDATE" _
                 & " SET Target.UPDATE_DATE = GETDATE()," _
                     & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                     & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                     & " Target.DELIVERY_END_TIME = '" & sDeliveryEndTime & "'," _
                     & " Target.DELIVERY_STS = " & oRcvTeleg.EatResult.ToString() _
               & " WHEN NOT MATCHED THEN" _
                & " INSERT (INSERT_DATE," _
                        & " INSERT_USER_ID," _
                        & " INSERT_MACHINE_ID," _
                        & " UPDATE_DATE," _
                        & " UPDATE_USER_ID," _
                        & " UPDATE_MACHINE_ID," _
                        & " MODEL_CODE," _
                        & " FILE_KBN," _
                        & " DATA_KIND," _
                        & " DATA_SUB_KIND," _
                        & " VERSION," _
                        & " RAIL_SECTION_CODE," _
                        & " STATION_ORDER_CODE," _
                        & " CORNER_CODE," _
                        & " UNIT_NO," _
                        & " DELIVERY_END_TIME," _
                        & " DELIVERY_STS)" _
                & " VALUES (GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " Source.MODEL_CODE," _
                        & " Source.FILE_KBN," _
                        & " Source.DATA_KIND," _
                        & " Source.DATA_SUB_KIND," _
                        & " Source.VERSION," _
                        & " Source.RAIL_SECTION_CODE," _
                        & " Source.STATION_ORDER_CODE," _
                        & " Source.CORNER_CODE," _
                        & " Source.UNIT_NO," _
                        & " '" & sDeliveryEndTime & "'," _
                        & " " & oRcvTeleg.EatResult.ToString() & ");"
            '-------Ver0.1　フェーズ２　「適用済み」状態を追加　MOD END-----------
        Else
            Dim sVerFormat As String
            If oSpec.FilePurpose.Equals(EkConstants.FilePurposeList) Then
                sVerFormat = "D2"
            Else
                sVerFormat = EkConstants.ProgramDataVersionFormatOfModels(oSpec.ApplicableModel)
            End If

            '-------Ver0.1　フェーズ２　「適用済み」状態を追加　MOD START-----------
            sSQL = _
               "MERGE INTO S_" & oSpec.DataPurpose & "_DL_STS AS Target" _
               & " USING (SELECT '" & oSpec.ApplicableModel & "' MODEL_CODE," _
                             & " '" & oSpec.FilePurpose & "' FILE_KBN," _
                             & " '" & oSpec.DataKind & "' DATA_KIND," _
                             & " '" & oRcvTeleg.VersionNumber.ToString(sVerFormat) & "' VERSION," _
                             & " '" & oRcvTeleg.EatClientCode.RailSection.ToString("D3") & "' RAIL_SECTION_CODE," _
                             & " '" & oRcvTeleg.EatClientCode.StationOrder.ToString("D3") & "' STATION_ORDER_CODE," _
                             & " " & oRcvTeleg.EatClientCode.Corner.ToString() & " CORNER_CODE," _
                             & " " & oRcvTeleg.EatClientCode.Unit.ToString() & " UNIT_NO) AS Source" _
               & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                 & " AND Target.FILE_KBN = Source.FILE_KBN" _
                 & " AND Target.DATA_KIND = Source.DATA_KIND" _
                 & " AND Target.VERSION = Source.VERSION" _
                 & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                 & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                 & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                 & " AND Target.UNIT_NO = Source.UNIT_NO)" _
               & " WHEN MATCHED" _
               & " AND (" & oRcvTeleg.EatResult.ToString() & " <> " & DbConstants.DlStatusContinuingNormal.ToString() _
                 & " OR Target.DELIVERY_STS <> " & DbConstants.DlStatusNormal.ToString() & ") THEN" _
                & " UPDATE" _
                 & " SET Target.UPDATE_DATE = GETDATE()," _
                     & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                     & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                     & " Target.DELIVERY_END_TIME = '" & sDeliveryEndTime & "'," _
                     & " Target.DELIVERY_STS = " & oRcvTeleg.EatResult.ToString() _
               & " WHEN NOT MATCHED THEN" _
                & " INSERT (INSERT_DATE," _
                        & " INSERT_USER_ID," _
                        & " INSERT_MACHINE_ID," _
                        & " UPDATE_DATE," _
                        & " UPDATE_USER_ID," _
                        & " UPDATE_MACHINE_ID," _
                        & " MODEL_CODE," _
                        & " FILE_KBN," _
                        & " DATA_KIND," _
                        & " VERSION," _
                        & " RAIL_SECTION_CODE," _
                        & " STATION_ORDER_CODE," _
                        & " CORNER_CODE," _
                        & " UNIT_NO," _
                        & " DELIVERY_END_TIME," _
                        & " DELIVERY_STS)" _
                & " VALUES (GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " Source.MODEL_CODE," _
                        & " Source.FILE_KBN," _
                        & " Source.DATA_KIND," _
                        & " Source.VERSION," _
                        & " Source.RAIL_SECTION_CODE," _
                        & " Source.STATION_ORDER_CODE," _
                        & " Source.CORNER_CODE," _
                        & " Source.UNIT_NO," _
                        & " '" & sDeliveryEndTime & "'," _
                        & " " & oRcvTeleg.EatResult.ToString() & ");"
            '-------Ver0.1　フェーズ２　「適用済み」状態を追加　MOD END-----------
        End If
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    Protected Overridable Sub UpdateDllVersionUncertainFlag(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String, ByVal sUncertainFlg As String)
        Dim sSQL As String = _
           "UPDATE S_" & EkMasProListFileName.GetDataPurpose(sListFileName) & "_DLL_VER" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " UNCERTAIN_FLG = '" & sUncertainFlg & "'" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND DATA_KIND = '" & EkMasProListFileName.GetDataKind(sListFileName) & "'" _
           & " AND DATA_SUB_KIND = '" & EkMasProListFileName.GetDataSubKind(sListFileName) & "'" _
           & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
           & " AND UNIT_NO = " & clientCode.Unit.ToString()
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    Protected Overridable Sub UpdateOrInsertDllVersion(ByVal dbCtl As DatabaseTalker, ByVal sListFileName As String)
        Dim sSQL As String = _
           "MERGE INTO S_" & EkMasProListFileName.GetDataPurpose(sListFileName) & "_DLL_VER AS Target" _
           & " USING (SELECT '" & sClientModel & "' MODEL_CODE," _
                         & " '" & EkMasProListFileName.GetDataKind(sListFileName) & "' DATA_KIND," _
                         & " '" & EkMasProListFileName.GetDataSubKind(sListFileName) & "' DATA_SUB_KIND," _
                         & " '" & clientCode.RailSection.ToString("D3") & "' RAIL_SECTION_CODE," _
                         & " '" & clientCode.StationOrder.ToString("D3") & "' STATION_ORDER_CODE," _
                         & " " & clientCode.Corner.ToString() & " CORNER_CODE," _
                         & " " & clientCode.Unit.ToString() & " UNIT_NO," _
                         & " '" & EkMasProListFileName.GetDataVersion(sListFileName) & "' DATA_VERSION) AS Source" _
           & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
             & " AND Target.DATA_KIND = Source.DATA_KIND" _
             & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
             & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
             & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
             & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
             & " AND Target.UNIT_NO = Source.UNIT_NO)" _
           & " WHEN MATCHED THEN" _
            & " UPDATE" _
             & " SET Target.UPDATE_DATE = GETDATE()," _
                 & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                 & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                 & " Target.DATA_VERSION = Source.DATA_VERSION," _
                 & " Target.UNCERTAIN_FLG = '0'" _
           & " WHEN NOT MATCHED THEN" _
            & " INSERT (INSERT_DATE," _
                    & " INSERT_USER_ID," _
                    & " INSERT_MACHINE_ID," _
                    & " UPDATE_DATE," _
                    & " UPDATE_USER_ID," _
                    & " UPDATE_MACHINE_ID," _
                    & " MODEL_CODE," _
                    & " DATA_KIND," _
                    & " DATA_SUB_KIND," _
                    & " RAIL_SECTION_CODE," _
                    & " STATION_ORDER_CODE," _
                    & " CORNER_CODE," _
                    & " UNIT_NO," _
                    & " DATA_VERSION," _
                    & " UNCERTAIN_FLG)" _
            & " VALUES (GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " Source.MODEL_CODE," _
                    & " Source.DATA_KIND," _
                    & " Source.DATA_SUB_KIND," _
                    & " Source.RAIL_SECTION_CODE," _
                    & " Source.STATION_ORDER_CODE," _
                    & " Source.CORNER_CODE," _
                    & " Source.UNIT_NO," _
                    & " Source.DATA_VERSION," _
                    & " '0');"
        dbCtl.ExecuteSQLToWrite(sSQL)
    End Sub

    Protected Overridable Sub DeleteAndInsertMasterVersionInfoExpected(ByVal dbCtl As DatabaseTalker, ByVal appUnitTable As DataTable, ByVal sDataFileName As String)
        '(1)sDataFileNameに含まれる機種・種別・パターンNo・マスタバージョンを取得する。
        '(2)マスタバージョン情報期待値テーブルに対し、(1)の「機種・種別」および
        'appUnitTableの号機と一致する全てのレコードを(1)で取得した「マスタバージョン」
        'および「パターンNo」によって、更新（なければ作成）する。

        Dim sDataAppModel As String = EkMasterDataFileName.GetApplicableModel(sDataFileName)
        Dim sDataKind As String = EkMasterDataFileName.GetKind(sDataFileName)
        Dim sDataSubKind As String = EkMasterDataFileName.GetSubKind(sDataFileName)
        Dim sDataVersion As string = EkMasterDataFileName.GetVersion(sDataFileName)
        Dim appUnits As DataRowCollection = appUnitTable.Rows

        For Each appUnit As DataRow In appUnits
            Dim sAppRailSection As String = appUnit.Field(Of String)("RAIL_SECTION_CODE")
            Dim sAppStationOrder As String = appUnit.Field(Of String)("STATION_ORDER_CODE")
            Dim sAppCorner As String = appUnit.Field(Of Integer)("CORNER_CODE").ToString()
            Dim sAppUnit As String = appUnit.Field(Of Integer)("UNIT_NO").ToString()

            Dim sSQL As String = _
               "MERGE INTO S_" & EkConstants.DataPurposeMaster & "_VER_INFO_EXPECTED AS Target" _
               & " USING (SELECT '" & sDataAppModel & "' MODEL_CODE," _
                             & " '" & sAppRailSection & "' RAIL_SECTION_CODE," _
                             & " '" & sAppStationOrder & "' STATION_ORDER_CODE," _
                             & " " & sAppCorner & " CORNER_CODE," _
                             & " " & sAppUnit & " UNIT_NO," _
                             & " '" & sDataKind & "' DATA_KIND," _
                             & " '" & sDataSubKind & "' DATA_SUB_KIND," _
                             & " '" & sDataVersion & "' DATA_VERSION) AS Source" _
               & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
                 & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                 & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                 & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                 & " AND Target.UNIT_NO = Source.UNIT_NO" _
                 & " AND Target.DATA_KIND = Source.DATA_KIND)" _
               & " WHEN MATCHED THEN" _
                & " UPDATE" _
                 & " SET Target.UPDATE_DATE = GETDATE()," _
                     & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                     & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                     & " Target.DATA_SUB_KIND = Source.DATA_SUB_KIND," _
                     & " Target.DATA_VERSION = Source.DATA_VERSION" _
               & " WHEN NOT MATCHED THEN" _
                & " INSERT (INSERT_DATE," _
                        & " INSERT_USER_ID," _
                        & " INSERT_MACHINE_ID," _
                        & " UPDATE_DATE," _
                        & " UPDATE_USER_ID," _
                        & " UPDATE_MACHINE_ID," _
                        & " MODEL_CODE," _
                        & " RAIL_SECTION_CODE," _
                        & " STATION_ORDER_CODE," _
                        & " CORNER_CODE," _
                        & " UNIT_NO," _
                        & " DATA_KIND," _
                        & " DATA_SUB_KIND," _
                        & " DATA_VERSION)" _
                & " VALUES (GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " GETDATE()," _
                        & " '" & UserId & "'," _
                        & " '" & MachineId & "'," _
                        & " Source.MODEL_CODE," _
                        & " Source.RAIL_SECTION_CODE," _
                        & " Source.STATION_ORDER_CODE," _
                        & " Source.CORNER_CODE," _
                        & " Source.UNIT_NO," _
                        & " Source.DATA_KIND," _
                        & " Source.DATA_SUB_KIND," _
                        & " Source.DATA_VERSION);"
            dbCtl.ExecuteSQLToWrite(sSQL)
        Next appUnit
    End Sub

    Protected Overridable Sub DeleteAndInsertProgramVersionInfoExpected(ByVal dbCtl As DatabaseTalker, ByVal appUnitTable As DataTable, ByVal sDataFileName As String)
        '(1)sDataFileNameに含まれる機種・種別を取得する。
        '(2)sDataFileNameをキーにプログラムデータ内容テーブルからCABに含まれるバージョンの一覧を取得する。
        '(3)プログラムバージョン情報期待値テーブルにおいて、(1)の「機種・種別」およびappUnitTableの号機を
        '  キーにする全てのレコードを削除する。
        '(4)プログラムバージョン情報期待値テーブルに対し、(1)の「機種・種別」、appUnitTableの号機
        '  および(2)で取得した一覧をもとに、新たなレコードを作成する。

        Dim sDataAppModel As String = EkProgramDataFileName.GetApplicableModel(sDataFileName)
        Dim appUnits As DataRowCollection = appUnitTable.Rows

        Dim sSQLToSelectRegInfos As String = _
           "SELECT ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME" _
           & " FROM S_" & EkConstants.DataPurposeProgram & "_DATA" _
           & " WHERE FILE_NAME = '" & sDataFileName & "'"
        Dim regInfos As DataRowCollection = dbCtl.ExecuteSQLToRead(sSQLToSelectRegInfos).Rows

        Dim oStringBuilder As New StringBuilder()
        For Each regInfo As DataRow In regInfos
            oStringBuilder.Append( _
               " (GETDATE()," _
               & " '" & UserId & "'," _
               & " '" & MachineId & "'," _
               & " GETDATE()," _
               & " '" & UserId & "'," _
               & " '" & MachineId & "'," _
               & " '" & sDataAppModel & "'," _
               & " '{0}'," _
               & " '{1}'," _
               & " {2}," _
               & " {3}," _
               & " '" & regInfo.Field(Of String)("ELEMENT_ID") & "'," _
               & " '" & regInfo.Field(Of String)("ELEMENT_VERSION") & "'," _
               & " '" & regInfo.Field(Of String)("ELEMENT_NAME") & "'),")
        Next regInfo

        Dim sValuesList As String = Nothing
        If oStringBuilder.Length <> 0 Then
            oStringBuilder.Remove(oStringBuilder.Length - 1, 1)
            sValuesList = oStringBuilder.ToString()
        End If

        For Each appUnit As DataRow In appUnits
            Dim sAppRailSection As String = appUnit.Field(Of String)("RAIL_SECTION_CODE")
            Dim sAppStationOrder As String = appUnit.Field(Of String)("STATION_ORDER_CODE")
            Dim sAppCorner As String = appUnit.Field(Of Integer)("CORNER_CODE").ToString()
            Dim sAppUnit As String = appUnit.Field(Of Integer)("UNIT_NO").ToString()

            Dim sSQLToDelete As String = _
               "DELETE FROM S_" & EkConstants.DataPurposeProgram & "_VER_INFO_EXPECTED" _
               & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
               & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
               & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
               & " AND CORNER_CODE = " & sAppCorner _
               & " AND UNIT_NO = " & sAppUnit
            dbCtl.ExecuteSQLToWrite(sSQLToDelete)

            If sValuesList IsNot Nothing Then
                Dim sSQLToInsert As String = _
                   "INSERT INTO S_" & EkConstants.DataPurposeProgram & "_VER_INFO_EXPECTED" _
                   & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID," _
                    & " UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
                    & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                    & " ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
                   & " VALUES" & String.Format(sValuesList, sAppRailSection, sAppStationOrder, sAppCorner, sAppUnit)
                dbCtl.ExecuteSQLToWrite(sSQLToInsert)
            End If
        Next appUnit
    End Sub

    Protected Overridable Function DeleteAndInsertMasterVersionInfo(ByVal dbCtl As DatabaseTalker, ByVal sFilePath As String) As NakCauseCode
        'TODO: ファイルの解析でエラーを検出した際の戻り値が微妙すぎる。
        'コネクションの異常でないのにコネクションを切ることになるし、
        '返信された側も意味がわからないと思われる。
        'ほんとうは、こういったケース用のNAK事由を
        'プロトコル仕様として定義すべきである。

        Dim sFileName As String = Path.GetFileName(sFilePath)
        Dim sDataAppModel As String = EkVersionInfoFileName.GetDataApplicableModel(sFileName)
        Dim dataAppUnit As EkCode = EkVersionInfoFileName.GetDataApplicableUnit(sFileName)

        'マスタバージョン情報ファイルから情報を読み出す。
        Dim aElements As EkMasterVersionInfoElement()
        Try
            Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                If Not sDataAppModel.Equals(EkConstants.ModelCodeMadosho) Then
                    'TODO: バージョン情報はもはやUpboundDataではないし、
                    '共通化のためにも、できればこの無駄なヘッダもなくしたい。
                    oInputStream.Seek(EkConstants.UpboundDataHeaderLen, SeekOrigin.Begin)
                End If
                aElements = EkMasterVersionInfoReader.GetElementsFromStream(oInputStream)
            End Using
        Catch ex As IOException
            Log.Error("Exception caught.", ex)
            Return EkNakCauseCode.TelegramError
        Catch ex As FormatException
            Log.Error("Exception caught.", ex)
            Return EkNakCauseCode.TelegramError
        End Try

        Dim oStringBuilder As New StringBuilder()

        '読み出したバージョン情報の各レコードを処理する。
        For i As Integer = 0 To aElements.Length - 1
            '表示対象レコードの場合
            If Not aElements(i).Kind.Equals("") AndAlso _
               Not aElements(i).Version.Equals("000") Then
                oStringBuilder.Append( _
                   " (GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " '" & sDataAppModel & "'," _
                   & " '{0}'," _
                   & " '{1}'," _
                   & " {2}," _
                   & " {3}," _
                   & " '" & aElements(i).Kind & "'," _
                   & " '" & aElements(i).SubKind & "'," _
                   & " '" & aElements(i).Version & "'),")
            End If
        Next

        Dim sValuesList As String = Nothing
        If oStringBuilder.Length <> 0 Then
            oStringBuilder.Remove(oStringBuilder.Length - 1, 1)
            sValuesList = oStringBuilder.ToString()
        End If

        Dim sAppRailSection As String = dataAppUnit.RailSection.ToString("D3")
        Dim sAppStationOrder As String = dataAppUnit.StationOrder.ToString("D3")
        Dim sAppCorner As String = dataAppUnit.Corner.ToString()
        Dim sAppUnit As String = dataAppUnit.Unit.ToString()

        Dim sSQLToDelete As String = _
           "DELETE FROM D_" & EkConstants.DataPurposeMaster & "_VER_INFO" _
           & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
           & " AND CORNER_CODE = " & sAppCorner _
           & " AND UNIT_NO = " & sAppUnit
        dbCtl.ExecuteSQLToWrite(sSQLToDelete)

        If sValuesList IsNot Nothing Then
            Dim sSQLToInsert As String = _
               "INSERT INTO D_" & EkConstants.DataPurposeMaster & "_VER_INFO" _
               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID," _
                & " UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
                & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                & " DATA_KIND, DATA_SUB_KIND, DATA_VERSION)" _
               & " VALUES" & String.Format(sValuesList, sAppRailSection, sAppStationOrder, sAppCorner, sAppUnit)
            dbCtl.ExecuteSQLToWrite(sSQLToInsert)
        End If

        Return EkNakCauseCode.None
    End Function

    Protected Overridable Function DeleteAndInsertProgramVersionInfo(ByVal dbCtl As DatabaseTalker, ByVal sFilePath As String, ByVal aGroupTitles As String()) As NakCauseCode
        'TODO: ファイルの解析でエラーを検出した際の戻り値が微妙すぎる。
        'コネクションの異常でないのにコネクションを切ることになるし、
        '返信された側も意味がわからないと思われる。
        'ほんとうは、こういったケース用のNAK事由を
        'プロトコル仕様として定義すべきである。

        Dim sFileName As String = Path.GetFileName(sFilePath)
        Dim sDataAppModel As String = EkVersionInfoFileName.GetDataApplicableModel(sFileName)
        Dim dataAppUnit As EkCode = EkVersionInfoFileName.GetDataApplicableUnit(sFileName)

        If sDataAppModel.Equals(EkConstants.ModelCodeMadosho) Then
            'TODO: 要確認。I/F仕様書や現行機の現地データをみる限りは、
            'ヘッダ部は無いようであるが、今回、改札機系と統一されている（その一方で
            'I/F仕様書の表現は現行と同じままになっている）可能性も考えられる。

            'プログラムバージョン情報ファイルから情報を読み出す。
            Dim aElementsForCur As EkMadoProgramVersionInfoElement()
            Dim aElementsForNew As EkMadoProgramVersionInfoElement()
            Try
                Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                    oInputStream.Seek(6, SeekOrigin.Begin)
                    aElementsForCur = EkMadoProgramVersionInfoReader.GetElementsFromStream(oInputStream)
                    oInputStream.Seek(6, SeekOrigin.Current)
                    aElementsForNew = EkMadoProgramVersionInfoReader.GetElementsFromStream(oInputStream)
                End Using
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                Return EkNakCauseCode.TelegramError
            Catch ex As FormatException
                Log.Error("Exception caught.", ex)
                Return EkNakCauseCode.TelegramError
            End Try

            DeleteAndInsertMadoProgramVersionInfo(dbCtl, dataAppUnit, aElementsForCur, "CUR")
            DeleteAndInsertMadoProgramVersionInfo(dbCtl, dataAppUnit, aElementsForNew, "NEW")
        Else
            'NOTE: 監視盤CABの解析方法を改札機CABと同様にした場合は、
            'groupCountの取得方法も改札機側と同様にするべきである。
            'なお、そうするか否かに関係なく、groupCountは派生クラスで
            '設定する方が、スッキリするかもしれない。
            Dim groupCount As Integer = aGroupTitles.Length
            Dim oReader As EkProgramVersionInfoReader
            If sDataAppModel.Equals(EkConstants.ModelCodeKanshiban) Then
                oReader = New EkProgramVersionInfoReaderForW()
            Else
                oReader = New EkProgramVersionInfoReaderForG()
            End If

            'プログラムバージョン情報ファイルから各グループの見出し情報を読み出す。
            Dim aGroupHeadersForCur(groupCount - 1) As EkProgramVersionInfoElementGroupHeader
            Dim aGroupsOfElementsForCur(groupCount - 1)() As EkProgramVersionInfoElement
            Dim aGroupHeadersForNew(groupCount - 1) As EkProgramVersionInfoElementGroupHeader
            Dim aGroupsOfElementsForNew(groupCount - 1)() As EkProgramVersionInfoElement
            Try
                Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                    'TODO: バージョン情報はもはやUpboundDataではないし、
                    'できればこの無駄なヘッダもなくしたい。
                    oInputStream.Seek(EkConstants.UpboundDataHeaderLen + 1, SeekOrigin.Begin)
                    For i As Integer = 0 To groupCount - 1
                        aGroupHeadersForCur(i) = oReader.GetOneGroupHeaderFromStream(oInputStream)
                    Next
                    For i As Integer = 0 To groupCount - 1
                        aGroupsOfElementsForCur(i) = oReader.GetOneGroupElementsFromStream(oInputStream, aGroupHeadersForCur(i))
                    Next
                    For i As Integer = 0 To groupCount - 1
                        aGroupHeadersForNew(i) = oReader.GetOneGroupHeaderFromStream(oInputStream)
                    Next
                    For i As Integer = 0 To groupCount - 1
                        aGroupsOfElementsForNew(i) = oReader.GetOneGroupElementsFromStream(oInputStream, aGroupHeadersForNew(i))
                    Next
                End Using
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                Return EkNakCauseCode.TelegramError
            Catch ex As FormatException
                Log.Error("Exception caught.", ex)
                Return EkNakCauseCode.TelegramError
            End Try

            DeleteAndInsertGateryProgramVersionInfo(dbCtl, sDataAppModel, dataAppUnit, aGroupsOfElementsForCur, "CUR", aGroupTitles)
            DeleteAndInsertGateryProgramVersionInfo(dbCtl, sDataAppModel, dataAppUnit, aGroupsOfElementsForNew, "NEW", aGroupTitles)
        End If

        Return EkNakCauseCode.None
    End Function

    Protected Overridable Sub DeleteAndInsertMadoProgramVersionInfo( _
       ByVal dbCtl As DatabaseTalker, _
       ByVal dataAppUnit As EkCode, _
       ByVal aElements As EkMadoProgramVersionInfoElement(), _
       ByVal sTableGeneration As String)

        Dim oStringBuilder As New StringBuilder()
        For i As Integer = 0 To aElements.Length - 1
            If aElements(i).IsVersion Then
                oStringBuilder.Append( _
                   " (GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " '" & EkConstants.ModelCodeMadosho & "'," _
                   & " '{0}'," _
                   & " '{1}'," _
                   & " {2}," _
                   & " {3}," _
                   & " '" & i.ToString("D2") & "'," _
                   & " '" & aElements(i).Value & "'," _
                   & " '" & aElements(i).Name.Replace("バージョン", "") & "'),")
            End If
        Next

        Dim sValuesList As String = Nothing
        If oStringBuilder.Length <> 0 Then
            oStringBuilder.Remove(oStringBuilder.Length - 1, 1)
            sValuesList = oStringBuilder.ToString()
        End If

        Dim sAppRailSection As String = dataAppUnit.RailSection.ToString("D3")
        Dim sAppStationOrder As String = dataAppUnit.StationOrder.ToString("D3")
        Dim sAppCorner As String = dataAppUnit.Corner.ToString()
        Dim sAppUnit As String = dataAppUnit.Unit.ToString()

        Dim sSQLToDelete As String = _
           "DELETE FROM D_" & EkConstants.DataPurposeProgram & "_VER_INFO_" & sTableGeneration _
           & " WHERE MODEL_CODE = '" & EkConstants.ModelCodeMadosho & "'" _
           & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
           & " AND CORNER_CODE = " & sAppCorner _
           & " AND UNIT_NO = " & sAppUnit
        dbCtl.ExecuteSQLToWrite(sSQLToDelete)

        If sValuesList IsNot Nothing Then
            Dim sSQLToInsert As String = _
               "INSERT INTO D_" & EkConstants.DataPurposeProgram & "_VER_INFO_" & sTableGeneration _
               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID," _
                & " UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
                & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                & " ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
               & " VALUES" & String.Format(sValuesList, sAppRailSection, sAppStationOrder, sAppCorner, sAppUnit)
            dbCtl.ExecuteSQLToWrite(sSQLToInsert)
        End If
    End Sub

    Protected Overridable Sub DeleteAndInsertGateryProgramVersionInfo( _
       ByVal dbCtl As DatabaseTalker, _
       ByVal sDataAppModel As String, _
       ByVal dataAppUnit As EkCode, _
       ByVal aGroupsOfElements As EkProgramVersionInfoElement()(), _
       ByVal sTableGeneration As String, _
       ByVal aGroupTitles As String())

        Dim oStringBuilder As New StringBuilder()
        For i As Integer = 0 To aGroupsOfElements.Length - 1
            For j As Integer = 0 To aGroupsOfElements(i).Length - 1
                Dim sElemName As String
                If aGroupTitles(i).Length <> 0 Then
                    sElemName = aGroupTitles(i) & "\" & Path.GetFileNameWithoutExtension(aGroupsOfElements(i)(j).FileName)
                Else
                    sElemName = aGroupsOfElements(i)(j).DispName
                End If

                oStringBuilder.Append( _
                   " (GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " GETDATE()," _
                   & " '" & UserId & "'," _
                   & " '" & MachineId & "'," _
                   & " '" & sDataAppModel & "'," _
                   & " '{0}'," _
                   & " '{1}'," _
                   & " {2}," _
                   & " {3}," _
                   & " '" & i.ToString("D2") & "\" & aGroupsOfElements(i)(j).FileName.ToUpperInvariant() & "'," _
                   & " '" & aGroupsOfElements(i)(j).Version & "'," _
                   & " '" & sElemName & "'),")
            Next
        Next

        Dim sValuesList As String = Nothing
        If oStringBuilder.Length <> 0 Then
            oStringBuilder.Remove(oStringBuilder.Length - 1, 1)
            sValuesList = oStringBuilder.ToString()
        End If

        Dim sAppRailSection As String = dataAppUnit.RailSection.ToString("D3")
        Dim sAppStationOrder As String = dataAppUnit.StationOrder.ToString("D3")
        Dim sAppCorner As String = dataAppUnit.Corner.ToString()
        Dim sAppUnit As String = dataAppUnit.Unit.ToString()

        Dim sSQLToDelete As String = _
           "DELETE FROM D_" & EkConstants.DataPurposeProgram & "_VER_INFO_" & sTableGeneration _
           & " WHERE MODEL_CODE = '" & sDataAppModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & sAppRailSection & "'" _
           & " AND STATION_ORDER_CODE = '" & sAppStationOrder & "'" _
           & " AND CORNER_CODE = " & sAppCorner _
           & " AND UNIT_NO = " & sAppUnit
        dbCtl.ExecuteSQLToWrite(sSQLToDelete)

        If sValuesList IsNot Nothing Then
            Dim sSQLToInsert As String = _
               "INSERT INTO D_" & EkConstants.DataPurposeProgram & "_VER_INFO_" & sTableGeneration _
               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID," _
                & " UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID," _
                & " MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                & " ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
               & " VALUES" & String.Format(sValuesList, sAppRailSection, sAppStationOrder, sAppCorner, sAppUnit)
            dbCtl.ExecuteSQLToWrite(sSQLToInsert)
        End If
    End Sub

    Protected Overridable Sub InsertScheduledUllFailureToCdt(ByVal sFileName As String)
        Dim recBaseInfo As New RecDataStructure.BaseInfo(sClientModel, clientCode)

        Dim aCdtKinds As String()
        Dim sDataKind As String = EkScheduledDataFileName.GetKind(sFileName)
        If DbConstants.CdtKindsOfDataKinds.ContainsKey(sDataKind) Then
            aCdtKinds = DbConstants.CdtKindsOfDataKinds(sDataKind)
        Else
            'NOTE: 不明な種別について「データの登録に失敗しました」の異常を
            '登録する場合と、フォールバックの方法が異なるが、単なる
            'フォールバックであり、Scheduleの設定に誤りがない限り、
            '動作することもないため、気にしないことにする。
            Log.Error("CollectedDataTypo code for [" & sDataKind & "] is not defined.")
            aCdtKinds = New String(0) {sDataKind}
        End If

        Dim sErrorInfo As String = Lexis.CdtScheduledUllFailed.Gen(sCdtClientModelName, clientCode.Unit.ToString())

        For i As Integer = 0 To aCdtKinds.Length - 1
            CollectedDataTypoRecorder.Record(recBaseInfo, aCdtKinds(i), sErrorInfo)
        Next
    End Sub

    Protected Overridable Sub InsertLineErrorToCdt()
        Dim now As DateTime = DateTime.Now
        'StartMinutesInDay以上になるように補正した
        '現在時刻を（0時0分からの経過分の形式で）求める。
        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
        If TelServerAppBaseConfig.LineErrorRecordingStartMinutesInDay > nowMinutesInDay Then
            nowMinutesInDay += 24 * 60
        End If

        '有効時間帯のみ登録を行う。
        If nowMinutesInDay <= TelServerAppBaseConfig.LineErrorRecordingEndMinutesInDay Then
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(sClientModel, clientCode), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtLineError.Gen(sCdtClientModelName, clientCode.Unit.ToString(), sCdtPortName))
        End If
    End Sub

    '-------Ver0.1 次世代車補対応 ADD START-----------
    Protected Overridable Sub EmitLineErrorMail()
        Dim now As DateTime = DateTime.Now
        'StartMinutesInDay以上になるように補正した
        '現在時刻を（0時0分からの経過分の形式で）求める。
        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
        If TelServerAppBaseConfig.LineErrorAlertingStartMinutesInDay > nowMinutesInDay Then
            nowMinutesInDay += 24 * 60
        End If

        '有効時間帯で、警報メール送信プロセスありで、通信相手が休止号機でない場合のみ生成を行う。
        Dim oTargetQueue As MessageQueue = Nothing
        If nowMinutesInDay <= TelServerAppBaseConfig.LineErrorAlertingEndMinutesInDay AndAlso _
           TelServerAppBaseConfig.MessageQueueForApps.TryGetValue("AlertMailer", oTargetQueue) = True Then
            Dim sSQL As String = _
               "SELECT COUNT(*)" _
               & " FROM M_RESTING_MACHINE" _
               & " WHERE MODEL_CODE = '" & sClientModel & "'" _
               & " AND RAIL_SECTION_CODE = '" & clientCode.RailSection.ToString("D3") & "'" _
               & " AND STATION_ORDER_CODE = '" & clientCode.StationOrder.ToString("D3") & "'" _
               & " AND CORNER_CODE = " & clientCode.Corner.ToString() _
               & " AND UNIT_NO = " & clientCode.Unit.ToString()
            Dim resting As Integer
            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                resting = CInt(dbCtl.ExecuteSQLToReadScalar(sSQL))
            Catch ex As DatabaseException
                Throw
            Catch ex As Exception
                Throw New DatabaseException(ex)
            Finally
                dbCtl.ConnectClose()
            End Try

            If resting = 0 Then
                '警報メールの文面を生成する。
                Dim sMailTitle As String = lineErrorAlertMailSubject.Gen(sClientStationName, sClientCornerName, clientCode.Unit)
                Dim sMailBody As String = lineErrorAlertMailBody.Gen(sClientStationName, sClientCornerName, clientCode.Unit, lineErrorBeginingTime)

                '警報メール送信プロセスに送信を要求する。
                oTargetQueue.Send(New ExtAlertMailSendRequest(sMailTitle, sMailBody))

                Log.Debug("Line error alert emitted.")
            Else
                Log.Debug("Line error alert suppressed because the client is resting.")
            End If
        End If
    End Sub
    '-------Ver0.1 次世代車補対応 ADD END-------------
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

'NOTE: 次世代車補対応にて、各種シーケンスの仕様記述用のクラスは、
'Publicに変更して、TelServerAppTelegrapherの中から定義箇所を移動した。
'それにより、これらのクラス名を記述している全ての箇所も機械的に変更している。
'内容の変更は、TelServerAppRiyoDataUllSpecへのメンバ追加のみである。
'-------Ver0.1 次世代車補対応 ADD START-----------
Public Class TelServerAppMasProDllSpec
    'NOTE: MaxRetryCountToForgetの値が有効に使われることはないはず
    'であるが、TelServerAppScheduledUllSpecとの一貫性維持のため、用意している。
    Public ObjCode As Byte
    Public SubObjCode As Byte
    Public TransferLimitTicks As Integer
    Public StartReplyLimitTicks As Integer
    Public RetryIntervalTicks As Integer
    Public MaxRetryCountToForget As Integer
    Public MaxRetryCountToCare As Integer

    Public Sub New( _
       ByVal objCode As Byte, _
       ByVal subObjCode As Byte, _
       ByVal transferLimitTicks As Integer, _
       ByVal startReplyLimitTicks As Integer, _
       ByVal retryIntervalTicks As Integer, _
       ByVal maxRetryCountToForget As Integer, _
       ByVal maxRetryCountToCare As Integer)

        Me.ObjCode = objCode
        Me.SubObjCode = subObjCode
        Me.TransferLimitTicks = transferLimitTicks
        Me.StartReplyLimitTicks = startReplyLimitTicks
        Me.RetryIntervalTicks = retryIntervalTicks
        Me.MaxRetryCountToForget = maxRetryCountToForget
        Me.MaxRetryCountToCare = maxRetryCountToCare
    End Sub
End Class

Public Class TelServerAppScheduledUllSpec
    Public ObjCode As Byte
    Public TransferLimitTicks As Integer
    Public StartReplyLimitTicks As Integer
    Public RetryIntervalTicks As Integer
    Public MaxRetryCountToForget As Integer
    Public MaxRetryCountToCare As Integer
    Public RecAppIdentifier As String

    Public Sub New( _
       ByVal objCode As Byte, _
       ByVal transferLimitTicks As Integer, _
       ByVal startReplyLimitTicks As Integer, _
       ByVal retryIntervalTicks As Integer, _
       ByVal maxRetryCountToForget As Integer, _
       ByVal maxRetryCountToCare As Integer, _
       ByVal recAppIdentifier As String)

        Me.ObjCode = objCode
        Me.TransferLimitTicks = transferLimitTicks
        Me.StartReplyLimitTicks = startReplyLimitTicks
        Me.RetryIntervalTicks = retryIntervalTicks
        Me.MaxRetryCountToForget = maxRetryCountToForget
        Me.MaxRetryCountToCare = maxRetryCountToCare
        Me.RecAppIdentifier = recAppIdentifier
    End Sub
End Class

Public Class TelServerAppMasProDlReflectSpec
    Public ApplicableModel As String
    Public FilePurpose As String
    Public DataPurpose As String
    Public DataKind As String

    Public Sub New( _
       ByVal sApplicableModel As String, _
       ByVal sFilePurpose As String, _
       ByVal sDataPurpose As String, _
       ByVal sDataKind As String)

        Me.ApplicableModel = sApplicableModel
        Me.FilePurpose = sFilePurpose
        Me.DataPurpose = sDataPurpose
        Me.DataKind = sDataKind
    End Sub
End Class

Public Class TelServerAppByteArrayPassivePostSpec
    Public RecAppIdentifier As String

    Public Sub New(ByVal recAppIdentifier As String)
        Me.RecAppIdentifier = recAppIdentifier
    End Sub
End Class

Public Class TelServerAppVersionInfoUllSpec
    Public ApplicableModel As String
    Public DataPurpose As String
    Public GroupTitles As String()
    Public TransferLimitTicks As Integer

    Public Sub New( _
       ByVal sApplicableModel As String, _
       ByVal sDataPurpose As String, _
       ByVal aGroupTitles As String(), _
       ByVal transferLimitTicks As Integer)

        Me.ApplicableModel = sApplicableModel
        Me.DataPurpose = sDataPurpose
        Me.GroupTitles = aGroupTitles
        Me.TransferLimitTicks = transferLimitTicks
    End Sub
End Class

Public Class TelServerAppRiyoDataUllSpec
    Public FileName As String
    Public FormatCode As String
    Public RecordLen As Integer
    Public TransferLimitTicks As Integer

    Public Sub New( _
       ByVal sFileName As String, _
       ByVal sFormatCode As String, _
       ByVal recordLen As Integer, _
       ByVal transferLimitTicks As Integer)

        Me.FileName = sFileName
        Me.FormatCode = sFormatCode
        Me.RecordLen = recordLen
        Me.TransferLimitTicks = transferLimitTicks
    End Sub
End Class
'-------Ver0.1 次世代車補対応 ADD END-------------
