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

Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Messaging
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 運管端末と電文の送受信を行うクラス。
''' </summary>
Public Class MyTelegrapher
    Inherits ServerTelegrapher

#Region "内部クラス等"
    Protected Structure DllIdentifier
        Public DataApplicableModel As String
        Public DataPurpose As String
        Public DataKind As String
        Public DataSubKind As String
        Public DataVersion As String
        Public ListVersion As String
    End Structure
#End Region

#Region "定数や変数"
    '各種テーブル共通の項目にセットする値
    Protected Const UserId As String = "System"
    Protected Const MachineId As String = "Server"

    '監視盤や改札機のCABに含まれるバージョン管理対象目録のファイル名
    Protected Const CatalogNameInCabDir As String = "FILELIST.TXT"

    'DLL状態テーブルをTASする際のSyncLock用オブジェクト
    'NOTE: 各端末からの登録や配信指示の受け入れは、当該データに関するDLL状態が
    '「配信中」以外の場合のみ可能とするが、DLL状態テーブルのあらゆるレコードに
    '関する（そのような目的の）チェックやチェック～セットは、このオブジェクトを
    'SyncLockした状態で行う。DLL状態テーブルの配信結果列を「配信中」に変更する
    'のは、このプロセスのみの役割であり、スレッドが落ちた際に、このオブジェクト
    '自体がSyncLockされたままになるようなこともない（.NET Frameworkが保証
    'しているはずである）ため、DBのレコードロック機能は用いるまでもない。
    'なお、DLL状態を「配信中」から他の状態に変更する責務を負ったスレッドが
    '落ちた際、どうやって責務を全うするかは、別問題として考えなければならない。
    'そのスレッドが停止後、正しく動作を継続しているリスナースレッドが責務を
    '引き継ぐのも１つの理想形であるが、それはなかなか大変である（責務を負って
    'いたスレッドを判別するための項目をDLL状態テーブル上に用意し、電文送受信
    'スレッドが落ちた際は、落ちたスレッドをキーにDLL状態テーブルの行をサーチ
    'しなければならない）。よって、そのスレッド自身のProcOnUnhandledException
    'で、元に戻すことにする。
    Protected Shared ReadOnly oDllStateTableTasLockObject As New Object()

    '上記を実現するため、DLL状態テーブルの何らかのレコードを「配信中」に
    '変更する場合は、事前にcurDllとisLockingDllStateRecordにその旨を
    '記録してから実施する（ジャーナリング相当の処理）。
    '処理の途中でAbortさせられる場合のことを想定し、実際の実行順序が
    '変わらないようにメモリバリアを挟んで処理を行う。
    '具体的には、下記の順序で処理を行う。
    '(1) 変更するレコードを識別するため値をcurDllにセットする。
    '(2) WriteBarrier
    '(3) isLockingDllStateRecordにTrueをセットする。
    '(4) WriteBarrier
    '(5) 実際にレコードを「配信中」に変更する。
    'ProcOnUnhandledExceptionでisLockingDllStateRecordがTrueの場合は、
    '記録しておいた情報からレコードを特定し、「異常」に変更する。
    '(3)完了から(5)完了までの間に（「配信中」に変更する前に）Abort
    'させられた場合、実際にこのスレッドは当該レコードの変更権を獲得した
    'わけでないにもかかわらず、ProcOnUnhandledExceptionで「異常」に
    '変更してしまうことになる。しかし、(3)が完了してから
    'ProcOnUnhandledExceptionで「異常」に変更するまでの間に、他の
    'スレッドが当該レコードを変更している可能性は極めて低い上、
    'そもそもスレッドがAbortさせられるということ自体、あっては
    'ならない異常事態であるから、それは許容する。
    Protected curDll As DllIdentifier
    Protected isLockingDllStateRecord As Boolean = False

    '一時作業用ディレクトリ名
    Protected sTempDirPath As String

    '電文書式
    Protected oTelegGene As EkTelegramGene

    '相手装置の装置コード
    'NOTE: ProcOnReqTelegramReceive()をフックして受信電文のClientCodeと比較してもよい。
    Protected clientCode As EkCode

    'アクセスを許可するパス
    Protected sPermittedPath As String

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

    '回線状態
    Private _LineStatus As Integer
#End Region

#Region "コンストラクタ"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal oTelegImporter As EkTelegramImporter, _
       ByVal oTelegGene As EkTelegramGene, _
       ByVal clientCode As EkCode, _
       ByVal sTempPath As String, _
       ByVal sPermittedPath As String)

        MyBase.New(sThreadName, oParentMessageSock, oTelegImporter)
        Me.sTempDirPath = sTempPath
        Me.oTelegGene = oTelegGene
        Me.clientCode = clientCode
        Me.sPermittedPath = sPermittedPath
        Me.reqNumberForNextSnd = 0
        Me.LineStatus = LineStatus.Initial

        Me.oWatchdogTimer.Renew(Config.WatchdogIntervalTicks)
        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite
        Me.enableXllStrongExclusion = Config.EnableXllStrongExclusion
        Me.enableActiveSeqStrongExclusion = Config.EnableActiveSeqStrongExclusion
        Me.enableActiveOneOrdering = Config.EnableActiveOneOrdering
    End Sub
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

#Region "イベント処理メソッド"
    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)
        Select Case oRcvTeleg.SubCmdCode
            Case EkSubCmdCode.Get
                Select Case oRcvTeleg.ObjCode
                    Case EkMasProDllInvokeReqTelegram.FormalObjCode
                        Return ProcOnMasProDllInvokeReqTelegramReceive(oRcvTeleg)
                End Select
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnMasProDllInvokeReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkMasProDllInvokeReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("MasProDllInvoke REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Dim sListFileName As String = Path.GetFileName(oRcvTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error("MasProDllInvoke REQ with invalid ListFileName received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.NoData, oRcvTeleg)
            Return True
        End If

        sListFileName = EkMasProListFileName.Normalize(sListFileName)
        Log.Info("MasProDllInvoke REQ about [" & sListFileName & "] received.")

        curDll.DataApplicableModel = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        curDll.DataPurpose = EkMasProListFileName.GetDataPurpose(sListFileName)
        curDll.DataKind = EkMasProListFileName.GetDataKind(sListFileName)
        curDll.DataSubKind = EkMasProListFileName.GetDataSubKind(sListFileName)
        curDll.DataVersion = EkMasProListFileName.GetDataVersion(sListFileName)
        curDll.ListVersion = EkMasProListFileName.GetListVersion(sListFileName)
        Thread.MemoryBarrier()

        Dim nakCause As NakCauseCode = EkNakCauseCode.None
        Do
            SyncLock oDllStateTableTasLockObject
                Try
                    '当該種別・当該バージョンのリストやデータが登録されていなければ、
                    'NAK（NO DATA）を返信する。
                    If Not IsCurDllObjectRegistered() Then
                        Log.Error("DLL objects are not registered.")
                        nakCause = EkNakCauseCode.NoData
                        Exit Do
                    End If

                    '動作許可日よりも過去日となる適用日が１つでも含まれる
                    '場合は、NAK（INVALID CONTENT）を返信する。
                    If curDll.DataPurpose = EkConstants.DataPurposeProgram AndAlso _
                       Not IsCurDllApplyingRunnableProgram(sListFileName) Then
                        Log.Error("The list contains adaptible date that is earlier than runnable date.")
                        nakCause = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    Dim dllStartTime As DateTime = DateTime.Now

                    '新たな配信が不要ならばNAK（UNNECESSARY）を返信する。
                    If Not IsCurDllNecessary(sListFileName, dllStartTime) Then
                        Log.Info("There is no client in the list file.")
                        nakCause = EkNakCauseCode.Unnecessary
                        Exit Do
                    End If

                    '機器構成マスタ上有効でない号機が適用対象になっている
                    '場合は、NAK（INVALID CONTENT）を返信する。
                    'NOTE: １つの適用リストを使いまわしている最中に、既に
                    '配信を済ませてある号機を機器構成から外すことができるよう、
                    '新たな配信先になる号機のみをチェック対象にしている。
                    '適用リスト内の全適用日（9999/99/99等は除く）を比較対象
                    'にする動作許可日チェックとはポリシーが異なるが、
                    '機器構成マスタと動作許可日（プログラム本体）では、
                    'プログラム適用リストとの結びつきの強さが異なるのは
                    '当然であり、とりあえず問題ではないものとする。
                    If Not IsCurDllConsistentWithMachineMaster(sListFileName, dllStartTime) Then
                        Log.Error("There is invalid client in the list file.")
                        nakCause = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '当該種別・当該バージョンのデータについて「配信中」や「登録中」の
                    '号機があるなら、NAK（BUSY）を返信する。
                    'NOTE: 「登録中」の号機がある場合とは、DLL状態テーブルに
                    '配信結果が「配信中」のダミー号機が存在する場合のことである。
                    If IsCurDllStatusBusyToStart() Then
                        Log.Info("DLL status is busy.")
                        nakCause = EkNakCauseCode.Busy
                        Exit Do
                    End If

                    '当該種別・当該バージョン・当該号機の配信結果を「配信中」に変更して、
                    '当該の通信プロセスに配信指示メッセージを送信する。
                    StartCurDll(sListFileName, dllStartTime, oRcvTeleg.ForcingFlag)

                Catch ex As DatabaseException
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    nakCause = EkNakCauseCode.TelegramError 'NOTE: 微妙
                    If isLockingDllStateRecord Then
                        'NOTE: ConnectCloseで例外が発生した場合であり、Abort
                        'したところで、「配信中」から変更することができない
                        '可能性は高いが、できる可能性もあるのでやっておく。
                        'NOTE: 本来なら、ユーザへの異常通知に関して、
                        '収集データ誤記テーブルへの登録だけでなく、間違いなく
                        '機能する（駄目なものに依存しない）ローレベルな
                        '仕組みを用意するべきである。
                        'たとえば、収集データ誤記登録機能が依存する全スレッドが
                        '「最終的にユーザに異常を表示する単純な機器」に対して、
                        '周期的に生存を通知するルールを設けるなどである。
                        'なお、SNMP TRAPは、収集データ誤記テーブルをユーザが
                        '能動的にポーリングする間を埋める（通知が成功すれば
                        '即座に異常を知ることができて、ちょっと嬉しい）という
                        '意義はあるものの、そのような要件は満たせない。
                        '「可能なら」異常を登録する（通知する）という点で、
                        '収集データ誤記登録と大同小異の機能である。
                        Abort()
                    Else
                        Exit Do
                    End If
                End Try
            End SyncLock
        Loop While False

        If nakCause <> EkNakCauseCode.None Then
            SendNakTelegramThenDisconnect(nakCause, oRcvTeleg)
            Return True
        End If

        Dim oReplyTeleg As EkMasProDllInvokeAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending MasProDllInvoke ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overrides Function CreateWatchdogReqTelegram() As IReqTelegram
        Return New EkWatchdogReqTelegram(oTelegGene, EkWatchdogReqTelegram.FormalObjCodeInOpClient, Config.WatchdogReplyLimitTicks)
    End Function

    'ヘッダ部の内容が受動的ULLのREQ電文のものであるか判定するメソッド
    Protected Overrides Function IsPassiveUllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        Return oTeleg.SubCmdCode = EkSubCmdCode.Get AndAlso _
               oTeleg.ObjCode = EkClientDrivenUllReqTelegram.FormalObjCodeAsOpClientFile
    End Function

    '渡された電文インスタンスを適切な型のインスタンスに変換するメソッド
    Protected Overrides Function ParseAsPassiveUllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        Return New EkClientDrivenUllReqTelegram(iTeleg, Config.OpClientFileUllTransferLimitTicks)
    End Function

    '受動的ULLの準備（予告されたファイルの受け入れ確認）を行うメソッド
    Protected Overrides Function PrepareToStartPassiveUll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePathInFtp As String = oXllReqTeleg.FileName
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, sFilePathInFtp)

        'NOTE: 「..\」等の混入を許すなら、sPermittedPathもsFilePathも正規化した方がよいかもしれない。
        If Not Utility.IsAncestPath(sPermittedPath, sFilePath) Then
            Log.Error("The telegram specifies illegal path [" & sFilePathInFtp & "].")
            Return EkNakCauseCode.NotPermit 'NOTE: 微妙
        End If

        Dim sFileName As String = Path.GetFileName(sFilePath)
        If EkMasProListFileName.IsValid(sFileName) Then
            curDll.DataApplicableModel = EkMasProListFileName.GetDataApplicableModel(sFileName)
            curDll.DataPurpose = EkMasProListFileName.GetDataPurpose(sFileName)
            curDll.DataKind = EkMasProListFileName.GetDataKind(sFileName)
            curDll.DataSubKind = EkMasProListFileName.GetDataSubKind(sFileName)
            curDll.DataVersion = EkMasProListFileName.GetDataVersion(sFileName)
            curDll.ListVersion = Nothing
        ElseIf EkMasterDataFileName.IsValid(sFileName) Then
            curDll.DataApplicableModel = EkMasterDataFileName.GetApplicableModel(sFileName)
            curDll.DataPurpose = EkConstants.DataPurposeMaster
            curDll.DataKind = EkMasterDataFileName.GetKind(sFileName)
            curDll.DataSubKind = EkMasterDataFileName.GetSubKind(sFileName)
            curDll.DataVersion = EkMasterDataFileName.GetVersion(sFileName)
            curDll.ListVersion = Nothing
        ElseIf EkProgramDataFileName.IsValid(sFileName) Then
            curDll.DataApplicableModel = EkProgramDataFileName.GetApplicableModel(sFileName)
            curDll.DataPurpose = EkConstants.DataPurposeProgram
            curDll.DataKind = EkProgramDataFileName.GetKind(sFileName)
            curDll.DataSubKind = EkProgramDataFileName.GetSubKind(sFileName)
            curDll.DataVersion = EkProgramDataFileName.GetVersion(sFileName)
            curDll.ListVersion = Nothing
        Else
            Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
            Return EkNakCauseCode.InvalidContent
        End If
        Thread.MemoryBarrier()

        SyncLock oDllStateTableTasLockObject
            Try
                '当該種別・当該バージョンのデータについて「配信中」や「登録中」の
                '号機があるなら、NAK（BUSY）を返信する。
                'NOTE: 「登録中」の号機がある場合とは、DLL状態テーブルに
                '配信結果が「配信中」のダミー号機が存在する場合のことである。
                If IsCurDllStatusBusyToRegister() Then
                    Log.Info("I am busy to accept a file [" & sFileName & "].")
                    Return EkNakCauseCode.Busy
                End If

                Log.Info("Accepting a file [" & sFileName & "]...")

                '当該種別・当該バージョンのダミー号機の配信結果を「配信中」に変更。
                PrepareToRegisterCurDllObject()

            Catch ex As DatabaseException
                Log.Fatal("Unwelcome Exception caught.", ex)
                If isLockingDllStateRecord Then
                    'NOTE: ConnectCloseで例外が発生した場合であり、Abort
                    'したところで、「配信中」から変更することができない
                    '可能性は高いが、できる可能性もあるのでやっておく。
                    'NOTE: 本来なら、ユーザへの異常通知に関して、
                    '収集データ誤記テーブルへの登録だけでなく、間違いなく
                    '機能する（駄目なものに依存しない）ローレベルな
                    '仕組みを用意するべきである。
                    'たとえば、収集データ誤記登録機能が依存する全スレッドが
                    '「最終的にユーザに異常を表示する単純な機器」に対して、
                    '周期的に生存を通知するルールを設けるなどである。
                    'なお、SNMP TRAPは、収集データ誤記テーブルをユーザが
                    '能動的にポーリングする間を埋める（通知が成功すれば
                    '即座に異常を知ることができて、ちょっと嬉しい）という
                    '意義はあるものの、そのような要件は満たせない。
                    '「可能なら」異常を登録する（通知する）という点で、
                    '収集データ誤記登録と大同小異の機能である。
                    Abort()
                Else
                    Return EkNakCauseCode.TelegramError 'NOTE: 微妙
                End If
            End Try
        End SyncLock

        Return EkNakCauseCode.None
    End Function

    '受動的ULLが成功した（受信済みのハッシュ値と受信完了したファイルの内容が整合していることを確認した）
    '（転送終了REQ電文に対しACK電文を返信することになる）場合
    Protected Overrides Function ProcOnPassiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, oXllReqTeleg.FileName)
        Dim sFileName As String = Path.GetFileName(sFilePath)
        Dim violation As NakCauseCode

        '受信したファイルの内容をDBに登録。
        If EkMasProListFileName.IsValid(sFileName) Then
            sFileName = EkMasProListFileName.Normalize(sFileName)
            violation = RegisterCurDllObjectAsList(sFilePath, sFileName, oXllReqTeleg.FileHashValue)
        ElseIf EkMasterDataFileName.IsValid(sFileName) Then
            sFileName = EkMasterDataFileName.Normalize(sFileName)
            violation = RegisterCurDllObjectAsMasterData(sFilePath, sFileName, oXllReqTeleg.FileHashValue)
        ElseIf EkProgramDataFileName.IsValid(sFileName) Then
            sFileName = EkProgramDataFileName.Normalize(sFileName)
            violation = RegisterCurDllObjectAsProgramData(sFilePath, sFileName, oXllReqTeleg.FileHashValue)
        Else
            Debug.Fail("This case is impermissible.")
            violation = EkNakCauseCode.TelegramError
        End If

        '内容等の問題により、DBに登録できない場合
        If violation <> EkNakCauseCode.None Then
            '受信したファイルを削除。
            File.Delete(sFilePath)

            '当該種別・当該バージョンのダミー号機の配信結果を「－」に変更。
            'NOTE: 例外が発生した場合、「配信中」のまま処理を継続するのは
            '避けたいので、そのままスレッドの終了に持ち込む。
            'NOTE: 本来なら（以下同文）
            FinishToRegisterCurDllObject()

            '呼び元に問題を通知して終了。
            Return violation
        End If

        '受信したファイルをマスタ/プログラムの管理ディレクトリに移動。
        'NOTE: 本当は、DBにコミットを行う前（ロールバックができる時点）で
        '実施するのが理想であるが、コミットしてからファイル移動が
        '行われるまでの間に例外の発生等がある可能性は低いため、
        '共通化を優先してここで実行する。
        Dim sDstPath As String = Path.Combine(Config.MasProDirPath, sFileName)
        File.Delete(sDstPath)
        File.Move(sFilePath, sDstPath)

        '当該種別・当該バージョンのダミー号機の配信結果を「－」に変更。
        'NOTE: 例外が発生した場合、「配信中」のまま処理を継続するのは
        '避けたいので、そのままスレッドの終了に持ち込む。
        'NOTE: 本来なら（以下同文）
        FinishToRegisterCurDllObject()

        Return EkNakCauseCode.None
    End Function

    '受動的ULLにて転送化けを検出した（受信済みのハッシュ値と受信完了したファイルの内容に不整合を検出した）
    '（転送終了REQ電文に対しハッシュ値の不一致を示すNAK電文を返信することになる）場合
    Protected Overrides Function ProcOnPassiveUllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, oXllReqTeleg.FileName)

        '受信したファイルがあれば捨てる。
        File.Delete(sFilePath)

        '当該種別・当該バージョンのダミー号機の配信結果を「－」に変更。
        'NOTE: 例外が発生した場合、「配信中」のまま処理を継続するのは
        '避けたいので、そのままスレッドの終了に持ち込む。
        'NOTE: 本来なら（以下同文）
        FinishToRegisterCurDllObject()

        Return EkNakCauseCode.HashValueError
    End Function

    '受動的ULLにてクライアントから転送失敗を通知された（ContinueCode.Abortの転送終了REQ電文を受信した）場合
    Protected Overrides Sub ProcOnPassiveUllAbort(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, oXllReqTeleg.FileName)

        '受信したファイルがあれば捨てる。
        File.Delete(sFilePath)

        '当該種別・当該バージョンのダミー号機の配信結果を「－」に変更。
        'NOTE: 例外が発生した場合、「配信中」のまま処理を継続するのは
        '避けたいので、そのままスレッドの終了に持ち込む。
        'NOTE: 本来なら（以下同文）
        FinishToRegisterCurDllObject()
    End Sub

    '受動的ULLにて転送終了REQ電文待ちのタイムアウトが発生した場合
    Protected Overrides Sub ProcOnPassiveUllTimeout(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: この状況では、FTPサーバ上にある受信（受信中）ファイルを削除しない方がよい。

        '当該種別・当該バージョンのダミー号機の配信結果を「－」に変更。
        'NOTE: 例外が発生した場合、「配信中」のまま処理を継続するのは
        '避けたいので、そのままスレッドの終了に持ち込む。
        'NOTE: 本来なら（以下同文）
        FinishToRegisterCurDllObject()
    End Sub

    '受動的ULLの最中やキューイングされた受動的ULLの開始前に通信異常を検出した場合
    Protected Overrides Sub ProcOnPassiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: この状況では、FTPサーバ上にある受信（受信中）ファイルを削除しない方がよい。

        If isLockingDllStateRecord Then
            '当該種別・当該バージョンのダミー号機の配信結果を「－」に変更。
            'NOTE: 例外が発生した場合、「配信中」のまま処理を継続するのは
            '避けたいので、そのままスレッドの終了に持ち込む。
            'NOTE: 本来なら（以下同文）
            FinishToRegisterCurDllObject()
        End If
    End Sub

    '親スレッドからコネクションを受け取った場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionAppear()
        LineStatus = LineStatus.Steady
    End Sub

    'コネクションを切断した場合（通信状態の変化をフックするためのメソッド）
    Protected Overrides Sub ProcOnConnectionDisappear()
        LineStatus = LineStatus.Disconnected
    End Sub

    Protected Overrides Sub ProcOnUnhandledException(ByVal unhandledEx As Exception)
        If isLockingDllStateRecord Then
            Try
                '当該種別・当該バージョンの「配信中」になっている
                '全てのレコードを「異常」に変更。
                TransitCurDllStatusToAbnormal()
            Catch ex As Exception
                'NOTE: 現状の製品仕様での対処方法としては、
                '運管サーバアプリの全プロセスを終了する（すくなくとも
                'DLL状態テーブルの「配信中」レコードを起動時に「異常」にする
                'ことができる通信系プロセスを終了する）くらいしかない。
                'NOTE: 本来なら（以下同文）
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        MyBase.ProcOnUnhandledException(unhandledEx)
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
        'TODO: データ種別などで分岐しておけば、ほとんどのケースを
        'プロトコル違反とみなして、NakRequirement.DisconnectImmediately
        'を返却することになるはず。
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

    Protected Overridable Function IsCurDllObjectRegistered() As Boolean
        'NOTE: テーブル内の最大レコード数も限られているし、インデックスも
        '効くので、件数そのものを取得して対象データの有無を判断する。

        Dim sSQLToCountList As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_LIST_HEADLINE" _
           & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
           & " AND LIST_VERSION = '" & curDll.ListVersion & "'"

        Dim sSQLToCountData As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
           & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            If CInt(dbCtl.ExecuteSQLToReadScalar(sSQLToCountList)) = 0 Then Return False
            If CInt(dbCtl.ExecuteSQLToReadScalar(sSQLToCountData)) = 0 Then Return False
            Return True
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function IsCurDllApplyingRunnableProgram(ByVal sListFileName As String) As Boolean
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            Dim sSQLToGetRunnableDate As String = _
               "SELECT RUNNABLE_DATE" _
               & " FROM S_" & EkConstants.DataPurposeProgram & "_DATA_HEADLINE" _
               & " WHERE DATA_KIND = '" & curDll.DataKind & "'" _
               & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
               & " AND DATA_VERSION = '" & curDll.DataVersion & "'"
            Dim sRunnableDate As String = CStr(dbCtl.ExecuteSQLToReadScalar(sSQLToGetRunnableDate))

            'NOTE: テーブル内の最大レコード数も限られているし、インデックスも
            '効くし、通常時の動作からして１件もヒットしない（FILE_NAMEの一致した
            'レコードをどのみち全てみることになる）ので、件数そのものを取得して
            '対象データの有無を判断する。
            Dim sSQL As String = _
               "SELECT COUNT(*)" _
               & " FROM S_" & EkConstants.DataPurposeProgram & "_LIST" _
               & " WHERE FILE_NAME = '" & sListFileName & "'" _
               & " AND APPLICABLE_DATE < '" & sRunnableDate & "'" _
               & " AND APPLICABLE_DATE <> '19000101'" _
               & " AND APPLICABLE_DATE <> ''"
            Return CInt(dbCtl.ExecuteSQLToReadScalar(sSQL)) = 0
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function IsCurDllNecessary(ByVal sListFileName As String, ByVal dllStartTime As DateTime) As Boolean
        Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)

        'NOTE: テーブル内の最大レコード数も限られているし、インデックスも
        '効くので、件数そのものを取得して対象データの有無を判断する。
        Dim sSQL As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        'NOTE: プログラム適用リストの場合は、有効な行を抽出するにあたり、
        '適用日にもとづく追加の条件をもうけている。なお、ブランクは
        'どのような日付（数字列）よりも小さいとみなされる想定である。
        If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
            sSQL = sSQL _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                   & " OR APPLICABLE_DATE = '19000101'" _
                   & " OR APPLICABLE_DATE = '99999999')"
        End If

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CInt(dbCtl.ExecuteSQLToReadScalar(sSQL)) <> 0
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function IsCurDllConsistentWithMachineMaster(ByVal sListFileName As String, ByVal dllStartTime As DateTime) As Boolean
        Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)

        '適用対象機器を取得するためのSQLを編集。
        Dim sSQLToSelectDataApplicableUnits As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM S_" & curDll.DataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        'NOTE: プログラム適用リストの場合は、有効な行を抽出するにあたり、
        '適用日にもとづく追加の条件をもうけている。なお、ブランクは
        'どのような日付（数字列）よりも小さいとみなされる想定である。
        If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
            sSQLToSelectDataApplicableUnits = sSQLToSelectDataApplicableUnits _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                   & " OR APPLICABLE_DATE = '19000101'" _
                   & " OR APPLICABLE_DATE = '99999999')"
        End If

        '配信指示の時点で有効な機器を取得するためのSQLを編集。
        Dim sSQLToSelectUnitsInService As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM M_MACHINE" _
           & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
           & " AND ADDRESS <> ''" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & sDeliveryStartDate & "'" _
                                        & " AND INSERT_DATE <= CONVERT(DATETIME, '" & dllStartTime.ToString("yyyy/MM/dd HH:mm:ss") & "', 120))"

        Dim applicableUnits As DataRowCollection
        Dim serviceUnits As EnumerableRowCollection(Of DataRow)
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            applicableUnits = dbCtl.ExecuteSQLToRead(sSQLToSelectDataApplicableUnits).Rows
            serviceUnits = dbCtl.ExecuteSQLToRead(sSQLToSelectUnitsInService).AsEnumerable()
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        For Each applicableUnit As DataRow In applicableUnits
            Dim sRailSection As String = applicableUnit.Field(Of String)("RAIL_SECTION_CODE")
            Dim sStationOrder As String = applicableUnit.Field(Of String)("STATION_ORDER_CODE")
            Dim corner As Integer = applicableUnit.Field(Of Integer)("CORNER_CODE")
            Dim unit As Integer = applicableUnit.Field(Of Integer)("UNIT_NO")
            Dim num As Integer = ( _
               From serviceUnit In serviceUnits _
               Where serviceUnit.Field(Of String)("RAIL_SECTION_CODE") = sRailSection And _
                     serviceUnit.Field(Of String)("STATION_ORDER_CODE") = sStationOrder And _
                     serviceUnit.Field(Of Integer)("CORNER_CODE") = corner And _
                     serviceUnit.Field(Of Integer)("UNIT_NO") = unit _
               Select serviceUnit _
            ).Count

            If num = 0 Then
                Log.Info("[" & sRailSection & sStationOrder & "_" & corner.ToString("D4") & "_" & unit.ToString("D2") & "] of [" & curDll.DataApplicableModel & "] is not in service.")
                Return False
            End If
        Next applicableUnit
        Return True
    End Function

    Protected Overridable Function IsCurDllStatusBusyToStart() As Boolean
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        'NOTE: テーブル内の最大レコード数も限られているし、インデックスも
        '効くので、件数そのものを取得して対象データの有無を判断する。
        'NOTE: 一見すると適用リストのレコードだけみて排他をかければよい
        'ように思えるかもしれないが、登録処理との排他のために、FILE_KBNが
        'FilePurposeDataのレコードもみる必要がある。
        'NOTE: 配信中は、リストバージョンが同じ適用リストの配信開始は禁止にする。
        '配信開始時、画面表示の都合により、適用リストに関するレコードは、
        '配信に使う適用リストとリストバージョンが等しいもの全てをDLL_STSテーブル
        'から消去するが、配信中は、当該配信物の各送信先に関するレコードを
        'DLL_STSテーブルから参照するためである。
        '逆に、適用リストの登録中は、たとえ適用リストバージョンが同じであっても
        '代表バージョンが異なる適用リストであれば、配信開始を禁止しない。
        '登録中は、適用リストに関するレコードをDLL_STSテーブルから参照しないため
        'である。なお、下記のSQLにおいて、代表バージョンが等しい適用リストを
        '登録中であるか否かは、FILE_KBNがFilePurposeDataのケースで判定している
        'ことに注意（登録中を示すダミーレコードは、たとえ適用リストの登録中を
        '示すものであっても、FILE_KBNはFilePurposeDataである）。
        Dim sSQL As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_DLL_STS" _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND ((FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
             & " AND DATA_KIND = '" & curDll.DataKind & "'" _
             & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
             & " AND DATA_VERSION = '" & curDll.DataVersion & "')" _
             & " OR (FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
             & " AND DATA_KIND = '" & curDll.DataKind & "'" _
             & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
             & " AND (DATA_VERSION = '" & curDll.DataVersion & "'" _
               & " OR VERSION = '" & curDll.ListVersion & "')))" _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CInt(dbCtl.ExecuteSQLToReadScalar(sSQL)) <> 0
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function IsCurDllStatusBusyToRegister() As Boolean
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        'NOTE: テーブル内の最大レコード数も限られているし、インデックスも
        '効くので、件数そのものを取得して対象データの有無を判断する。
        Dim sSQL As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_DLL_STS" _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CInt(dbCtl.ExecuteSQLToReadScalar(sSQL)) <> 0
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Sub StartCurDll(ByVal sListFileName As String, ByVal dllStartTime As DateTime, ByVal forcingFlag As Boolean)
        Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)
        Dim sDeliveryStartTime As String = dllStartTime.ToString("yyyyMMddHHmmss")

        '配信指示を送信するべきメッセージキュー等を求めておく。
        Dim oTargetQueue As MessageQueue
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            oTargetQueue = Config.MessageQueueForApps("ToTokatsu")
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            oTargetQueue = Config.MessageQueueForApps("ToKanshiban")
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        '配信指示を作成しておく。
        Dim oDllRequest As New ExtMasProDllRequest(sListFileName, forcingFlag)

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

        '適用先装置の線区～号機を取得するSQLを編集。
        Dim sSQLToSelectApplicableUnits As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM S_" & curDll.DataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        Dim sSQLToSelectApplicableUnitsCompoundStyled As String = _
           "SELECT RAIL_SECTION_CODE + STATION_ORDER_CODE + CAST(CORNER_CODE AS varchar) + '_' + CAST(UNIT_NO AS varchar)" _
           & " FROM S_" & curDll.DataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        'NOTE: プログラム適用リストの場合は、有効な行を抽出するにあたり、
        '適用日にもとづく追加の条件をもうけている。なお、ブランクは
        'どのような日付（数字列）よりも小さいとみなされる想定である。
        If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
            sSQLToSelectApplicableUnits = _
               sSQLToSelectApplicableUnits _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                    & " OR APPLICABLE_DATE = '19000101'" _
                    & " OR APPLICABLE_DATE = '99999999')"
            sSQLToSelectApplicableUnitsCompoundStyled = _
               sSQLToSelectApplicableUnitsCompoundStyled _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                    & " OR APPLICABLE_DATE = '19000101'" _
                    & " OR APPLICABLE_DATE = '99999999')"
        End If

        '直接の送信先となる装置のIPアドレスを取得するSQLを編集。
        'NOTE: MONITOR_ADDRESSには、ブランクが入る可能性は想定していない。
        'たとえば、実際に当該コーナーに存在しない監視盤のレコードを
        '機器構成に記述する運用になったとしても、そのレコードの
        'MONITOR_ADDRESSにも、実体となる監視盤のIPアドレスが
        '設定される想定である。
        'Dim sSQLToSelectAddrOfAgents As String = _
        '   "SELECT DISTINCT MONITOR_ADDRESS" _
        '   & " FROM M_SERVICE_MACHINE" _
        '   & " WHERE (RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO) IN (" & sSQLToSelectApplicableUnits & ")" _
        '   & " AND MODEL_CODE = '" & curDll.DataApplicableModel & "'"
        Dim sSQLToSelectAddrOfAgents As String = _
           "SELECT DISTINCT MONITOR_ADDRESS" _
           & " FROM M_SERVICE_MACHINE" _
           & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
           & " AND RAIL_SECTION_CODE + STATION_ORDER_CODE + CAST(CORNER_CODE AS varchar) + '_' + CAST(UNIT_NO AS varchar)" _
               & " IN (" & sSQLToSelectApplicableUnitsCompoundStyled & ")"

        '直接の送信先となる装置の線区～号機を取得するSQLを編集。
        'NOTE: sSQLToSelectAddrOfAgentsで得られる全ての監視盤または統括が
        'sSQLToSelectAgentsでも得られる（それぞれの出力件数が同じになる）
        '想定であるが、そのことはチェックしない。そのチェックは、
        '適用リストではなく、機器構成マスタのチェックになるため、
        '機器構成マスタの登録時に行われるべきものである。
        Dim sSQLToSelectAgents As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM M_SERVICE_MACHINE" _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND ADDRESS IN (" & sSQLToSelectAddrOfAgents & ")"

        'DLL状態テーブルのレコードを更新または追加するためのSQLの後半を編集。
        Dim sSQLWithoutUsingToUpdateOrInsertDllSts As String = _
             " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
             & " AND Target.FILE_KBN = Source.FILE_KBN" _
             & " AND Target.DATA_KIND = Source.DATA_KIND" _
             & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
             & " AND Target.DATA_VERSION = Source.DATA_VERSION" _
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
                 & " Target.DELIVERY_STS = Source.DELIVERY_STS," _
                 & " Target.DELIVERY_START_TIME = Source.DELIVERY_START_TIME," _
                 & " Target.DELIVERY_END_TIME = Source.DELIVERY_END_TIME" _
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
                    & " DATA_VERSION," _
                    & " VERSION," _
                    & " RAIL_SECTION_CODE," _
                    & " STATION_ORDER_CODE," _
                    & " CORNER_CODE," _
                    & " UNIT_NO," _
                    & " DELIVERY_STS," _
                    & " DELIVERY_START_TIME," _
                    & " DELIVERY_END_TIME)" _
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
                    & " Source.DATA_VERSION," _
                    & " Source.VERSION," _
                    & " Source.RAIL_SECTION_CODE," _
                    & " Source.STATION_ORDER_CODE," _
                    & " Source.CORNER_CODE," _
                    & " Source.UNIT_NO," _
                    & " Source.DELIVERY_STS," _
                    & " Source.DELIVERY_START_TIME," _
                    & " Source.DELIVERY_END_TIME);"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            'NOTE: 現状の設計で、配信の排他をエリア別に行うのは、プログラムやプログラム
            '適用リストのファイル名がエリア別に用意されているためである（あるユーザが
            '配信指示で指名したファイルが、別のユーザによって上書きされてしまうのを防ぐ
            '上で、インターロックの最小化を目指している）。しかし、実際に適用リストの
            '中に記載されている号機が、ファイル名のエリアに属しているかは、運管サーバ
            '自身はチェックしていない。もし、エリアに属さない駅が記載されていれば、
            '実際に配信で使うファイルと無関係のものまでロックすることになってしまい、
            '勿体ない。また、使用する適用リストには、最低でも１件の有効な号機が
            '記載されているとはいえ、それがファイル名のエリアに属していなければ、
            'そのファイルをロックできない。それを防ぐために、S_PRG_DLL_STSにも
            'DATA_SUB_KIND列を用意し、各行のキーに駅コードが含まれているにもかかわらず、
            '各行の属するエリアNoは、DATA_SUB_KINDで識別することにしている。
            'これは、マスタとの実装共通化というメリットももたらす。

            'DLL状態テーブルから今回の配信と種別・エリアNo・適用リストバージョンが
            '等しい適用リストに関する全レコードを削除する。
            Dim sSQLToDeleteFromDllStsAboutList As String = _
               "DELETE FROM S_" & curDll.DataPurpose & "_DLL_STS" _
               & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
               & " AND FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
               & " AND DATA_KIND = '" & curDll.DataKind & "'" _
               & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
               & " AND VERSION = '" & curDll.ListVersion & "'"
            dbCtl.ExecuteSQLToWrite(sSQLToDeleteFromDllStsAboutList)

            If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
                'プログラムDL状態テーブルから今回の配信と種別・適用リストバージョン・
                'エリアNoが等しい適用リストに関する全レコードを削除する。
                Dim sSQLToDeleteFromPrgDlStsAboutList As String = _
                   "DELETE FROM S_" & curDll.DataPurpose & "_DL_STS" _
                   & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
                   & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                   & " AND VERSION = '" & curDll.ListVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteFromPrgDlStsAboutList)
            End If

            '直接の送信先となる装置の線区～号機を取得する。
            Dim agents As DataRowCollection = dbCtl.ExecuteSQLToRead(sSQLToDefineCTE & sSQLToSelectAgents).Rows

            For Each agent As DataRow In agents
                Dim sAgentRailSection As String = agent.Field(Of String)("RAIL_SECTION_CODE")
                Dim sAgentStationOrder As String = agent.Field(Of String)("STATION_ORDER_CODE")
                Dim sAgentCorner As String = agent.Field(Of Integer)("CORNER_CODE").ToString()
                Dim sAgentUnit As String = agent.Field(Of Integer)("UNIT_NO").ToString()

                'NOTE: UNCERTAIN_FLGがTrueなレコードのDATA_VERSIONを0にするのは、
                'この後に通信プロセスで行うよりもここで行う方が効率的ではある。
                'しかし、ここを実行中は、運管端末で応答受信タイマがかけられているため、
                '時間的制約としては厳しい。そもそも、このタイミングでは、
                '当該号機への当該種別に関する配信が別バージョンについて実行されている
                '可能性もあり、当該種別の最終送信バージョンテーブルを変更することが
                'こちらのプロセスの役割で無いことは明らかである。以上のことから、
                '上記した処理は、駅務機器との通信プロセス側で行うことにしている。

                'agentの線区～号機が設定されている装置に対し、適用リストだけでなく
                'データ本体も送付するのか否かを決定する。
                Dim sendSuite As Boolean = False
                If forcingFlag = True Then
                    sendSuite = True
                Else
                    Dim sSQLToGetVer As String = _
                       "SELECT DATA_VERSION, UNCERTAIN_FLG" _
                       & " FROM S_" & curDll.DataPurpose & "_DLL_VER" _
                       & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
                       & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                       & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                       & " AND RAIL_SECTION_CODE = '" & sAgentRailSection & "'" _
                       & " AND STATION_ORDER_CODE = '" & sAgentStationOrder & "'" _
                       & " AND CORNER_CODE = " & sAgentCorner _
                       & " AND UNIT_NO = " & sAgentUnit
                    Dim verRows As DataRowCollection = dbCtl.ExecuteSQLToRead(sSQLToGetVer).Rows
                    If verRows.Count = 0 OrElse _
                       Not verRows(0).Field(Of String)("UNCERTAIN_FLG").Equals("0") OrElse _
                       Not verRows(0).Field(Of String)("DATA_VERSION").Equals(curDll.DataVersion) Then
                        sendSuite = True
                    End If
                End If

                'DLL状態テーブルにて適用リストのレコードを更新または追加する。
                'OPT: 該当するものは事前の削除処理の対象になっているので、
                '単なるINSERTでもよい。
                Dim sSQLToUpdateOrInsertDllStsAboutList As String = _
                   "MERGE INTO S_" & curDll.DataPurpose & "_DLL_STS AS Target" _
                   & " USING (SELECT '" & sDllAgentModel & "' MODEL_CODE," _
                                 & " '" & EkConstants.FilePurposeList & "' FILE_KBN," _
                                 & " '" & curDll.DataKind & "' DATA_KIND," _
                                 & " '" & curDll.DataSubKind & "' DATA_SUB_KIND," _
                                 & " '" & curDll.DataVersion & "' DATA_VERSION," _
                                 & " '" & curDll.ListVersion & "' VERSION," _
                                 & " '" & sAgentRailSection & "' RAIL_SECTION_CODE," _
                                 & " '" & sAgentStationOrder & "' STATION_ORDER_CODE," _
                                 & " " & sAgentCorner & " CORNER_CODE," _
                                 & " " & sAgentUnit & " UNIT_NO," _
                                 & " " & DbConstants.DllStatusExecuting.ToString() & " DELIVERY_STS," _
                                 & " '" & sDeliveryStartTime & "' DELIVERY_START_TIME," _
                                 & " '' DELIVERY_END_TIME) AS Source" _
                   & sSQLWithoutUsingToUpdateOrInsertDllSts
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDllStsAboutList)

                If sendSuite Then
                    'DLL状態テーブルにてデータ本体のレコードを更新または追加する。
                    Dim sSQLToUpdateOrInsertDllStsAboutData As String = _
                       "MERGE INTO S_" & curDll.DataPurpose & "_DLL_STS AS Target" _
                       & " USING (SELECT '" & sDllAgentModel & "' MODEL_CODE," _
                                     & " '" & EkConstants.FilePurposeData & "' FILE_KBN," _
                                     & " '" & curDll.DataKind & "' DATA_KIND," _
                                     & " '" & curDll.DataSubKind & "' DATA_SUB_KIND," _
                                     & " '" & curDll.DataVersion & "' DATA_VERSION," _
                                     & " '" & curDll.DataVersion & "' VERSION," _
                                     & " '" & sAgentRailSection & "' RAIL_SECTION_CODE," _
                                     & " '" & sAgentStationOrder & "' STATION_ORDER_CODE," _
                                     & " " & sAgentCorner & " CORNER_CODE," _
                                     & " " & sAgentUnit & " UNIT_NO," _
                                     & " " & DbConstants.DllStatusExecuting.ToString() & " DELIVERY_STS," _
                                     & " '" & sDeliveryStartTime & "' DELIVERY_START_TIME," _
                                     & " '' DELIVERY_END_TIME) AS Source" _
                       & sSQLWithoutUsingToUpdateOrInsertDllSts
                    dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDllStsAboutData)
                End If
            Next agent

            If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
                '適用リストに記載されている全ての有効号機について、
                'DL状態テーブルに当該レコードが無い場合は追加する。
                'レコードが既にある場合も（過去に違うエリアで
                '登録されたものであっても正しい条件で消されるように
                'しておくために）エリアNoを上書きする。
                'NOTE: プログラムのDL状態において、エリアNoは
                'あらたに配信を始める際に削除するレコードを
                '選択するためだけのものである。運管端末の配信状況
                '画面でエリアを絞る際は、DLL状態テーブルのエリアで
                '絞ることになっており、本質的にDL状態テーブルの
                'レコードを（画面の）エリアにひもづけるのは
                '駅コードである。よって、エリアNoが一致しなくても、
                '（適用リストに記述されたものと線区・駅順・
                'コーナー・号機が一致すれば）当該レコードとみなす。
                Dim sSQLToUpdateOrInsertDlStsAboutData As String = _
                   "MERGE INTO S_" & curDll.DataPurpose & "_DL_STS AS Target" _
                   & " USING (" & sSQLToSelectApplicableUnits & ") AS Source" _
                   & " ON (Target.MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                     & " AND Target.FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                     & " AND Target.DATA_KIND = '" & curDll.DataKind & "'" _
                     & " AND Target.VERSION = '" & curDll.DataVersion & "'" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
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
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " '" & curDll.DataApplicableModel & "'," _
                            & " '" & EkConstants.FilePurposeData & "'," _
                            & " '" & curDll.DataKind & "'," _
                            & " '" & curDll.DataSubKind & "'," _
                            & " '" & curDll.DataVersion & "'," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusPreExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDlStsAboutData)

                'プログラムの場合、Dataだけでなく、Listについてについても、
                'DL状態テーブルのレコードを追加する。
                'NOTE: Listの場合は、DL状態テーブルの関連レコードを
                '事前に削除しているため、基本的にINSERTしか実行されないはず。
                Dim sSQLToUpdateOrInsertDlStsAboutList As String = _
                   "MERGE INTO S_" & curDll.DataPurpose & "_DL_STS AS Target" _
                   & " USING (" & sSQLToSelectApplicableUnits & ") AS Source" _
                   & " ON (Target.MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                     & " AND Target.FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
                     & " AND Target.DATA_KIND = '" & curDll.DataKind & "'" _
                     & " AND Target.VERSION = '" & curDll.ListVersion & "'" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
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
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " '" & curDll.DataApplicableModel & "'," _
                            & " '" & EkConstants.FilePurposeList & "'," _
                            & " '" & curDll.DataKind & "'," _
                            & " '" & curDll.DataSubKind & "'," _
                            & " '" & curDll.ListVersion & "'," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusPreExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDlStsAboutList)
            Else
                '適用リストに記載されている全ての有効号機について、
                'DL状態テーブルに当該レコードが無い場合は追加する。
                'NOTE: マスタのDL状態は、パターン別に管理するので、
                'パターンNoまで一致しなければ、当該レコードとみなさない。
                Dim sSQLToUpdateOrInsertDlStsAboutData As String = _
                   "MERGE INTO S_" & curDll.DataPurpose & "_DL_STS AS Target" _
                   & " USING (" & sSQLToSelectApplicableUnits & ") AS Source" _
                   & " ON (Target.MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                     & " AND Target.FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                     & " AND Target.DATA_KIND = '" & curDll.DataKind & "'" _
                     & " AND Target.DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                     & " AND Target.VERSION = '" & curDll.DataVersion & "'" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
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
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " '" & curDll.DataApplicableModel & "'," _
                            & " '" & EkConstants.FilePurposeData & "'," _
                            & " '" & curDll.DataKind & "'," _
                            & " '" & curDll.DataSubKind & "'," _
                            & " '" & curDll.DataVersion & "'," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusPreExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDlStsAboutData)
            End If

            '当該データの当該号機への配信結果を実際に「配信中」にした後、
            'メッセージの送信等で失敗することは考えにくいが、
            '親スレッドからAbortが行われるケースがあるため、
            'それに備えて、配信結果を「配信中」にする旨を記録してから
            'トランザクションを完了させる。
            isLockingDllStateRecord = True
            Thread.MemoryBarrier()
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            isLockingDllStateRecord = False
            Thread.MemoryBarrier()
            Throw

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            isLockingDllStateRecord = False
            Thread.MemoryBarrier()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try

        oTargetQueue.Send(oDllRequest)

        'NOTE: この時点では、何かあったときにDLL状態テーブルの「配信中」を解除
        'する（「異常」に変更する）のは、既に、oDllRequestの送信先プロセスの
        '責務になっているものとする。
        isLockingDllStateRecord = False
        Thread.MemoryBarrier()
    End Sub

    Protected Overridable Sub PrepareToRegisterCurDllObject()
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        Dim sSQL As String = _
           "MERGE INTO S_" & curDll.DataPurpose & "_DLL_STS AS Target" _
           & " USING (SELECT '" & sDllAgentModel & "' MODEL_CODE," _
                         & " '" & EkConstants.FilePurposeData & "' FILE_KBN," _
                         & " '" & curDll.DataKind & "' DATA_KIND," _
                         & " '" & curDll.DataSubKind & "' DATA_SUB_KIND," _
                         & " '" & curDll.DataVersion & "' DATA_VERSION," _
                         & " '" & curDll.DataVersion & "' VERSION," _
                         & " '000' RAIL_SECTION_CODE," _
                         & " '000' STATION_ORDER_CODE," _
                         & " 0 CORNER_CODE," _
                         & " 0 UNIT_NO," _
                         & " " & DbConstants.DllStatusExecuting.ToString() & " DELIVERY_STS," _
                         & " '' DELIVERY_START_TIME," _
                         & " '' DELIVERY_END_TIME) AS Source" _
           & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
            & " AND Target.FILE_KBN = Source.FILE_KBN" _
            & " AND Target.DATA_KIND = Source.DATA_KIND" _
            & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
            & " AND Target.DATA_VERSION = Source.DATA_VERSION" _
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
                 & " Target.DELIVERY_STS = Source.DELIVERY_STS," _
                 & " Target.DELIVERY_START_TIME = Source.DELIVERY_START_TIME," _
                 & " Target.DELIVERY_END_TIME = Source.DELIVERY_END_TIME" _
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
                    & " DATA_VERSION," _
                    & " VERSION," _
                    & " RAIL_SECTION_CODE," _
                    & " STATION_ORDER_CODE," _
                    & " CORNER_CODE," _
                    & " UNIT_NO," _
                    & " DELIVERY_STS," _
                    & " DELIVERY_START_TIME," _
                    & " DELIVERY_END_TIME)" _
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
                    & " Source.DATA_VERSION," _
                    & " Source.VERSION," _
                    & " Source.RAIL_SECTION_CODE," _
                    & " Source.STATION_ORDER_CODE," _
                    & " Source.CORNER_CODE," _
                    & " Source.UNIT_NO," _
                    & " Source.DELIVERY_STS," _
                    & " Source.DELIVERY_START_TIME," _
                    & " Source.DELIVERY_END_TIME);"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            '当該データのダミー号機への配信結果を実際に「配信中」にした後、
            '登録を終えて「－」に戻すまでの間に親スレッドからAbortが行われる
            '場合などに備えて、配信結果を「配信中」にする旨を記録してから
            'トランザクションを完了させる。
            isLockingDllStateRecord = True
            Thread.MemoryBarrier()
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            isLockingDllStateRecord = False
            Thread.MemoryBarrier()
            Throw

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            isLockingDllStateRecord = False
            Thread.MemoryBarrier()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Sub FinishToRegisterCurDllObject()
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        Dim sSQL As String = _
           "DELETE FROM S_" & curDll.DataPurpose & "_DLL_STS" _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
           & " AND VERSION = '" & curDll.DataVersion & "'" _
           & " AND RAIL_SECTION_CODE = '000'" _
           & " AND STATION_ORDER_CODE = '000'" _
           & " AND CORNER_CODE = 0" _
           & " AND UNIT_NO = 0"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()
            'NOTE: フレームワーク上でトランザクションをきちんと完了させれば、
            'メモリバリアが発生する（下記のメモリ書き換えが、その手前に入り込む
            'ことはない）と思われるため、ここでのThread.MemoryBarrier()は
            '省略する。

            isLockingDllStateRecord = False
            Thread.MemoryBarrier()

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

    Protected Overridable Sub TransitCurDllStatusToAbnormal()
        'NOTE: このメソッドは、配信物の登録や配信開始で例外が発生した場合に
        '実行する。配信状態を「配信中」にしたレコードのせいで、スレッド
        '再起動後の登録や配信が不可能にならないよう、当該レコードの配信状態
        'を「異常」に変更するためのメソッドである。
        '当該レコードを指定する上で、LIST_VERSIONの指定は不要である。
        'MODEL_CODE～DATA_VERSIONが指定どおりのレコードの中で、
        'DELIVERY_STSがDllStatusExecutingになっているのは、
        '排他制御により、当該レコードのみに制限されているからである。
        '蛇足であるが、同様の理由で、DELIVERY_STS～DATA_SUB_KINDと
        'LIST_VERSIONを指定すれば、DATA_VERSIONを指定せずとも、DELIVERY_STSが
        'DllStatusExecutingになっている適用リストのレコードは、
        '現在実行中の配信のためのレコードだけに絞り込める。

        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        Dim sSQL As String = _
           "UPDATE S_" & curDll.DataPurpose & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_STS = " & DbConstants.DllStatusAbnormal.ToString() _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()
            'NOTE: フレームワーク上でトランザクションをきちんと完了させれば、
            'メモリバリアが発生する（下記のメモリ書き換えが、その手前に入り込む
            'ことはない）と思われるため、ここでのThread.MemoryBarrier()は
            '省略する。

            isLockingDllStateRecord = False
            Thread.MemoryBarrier()

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

    'NOTE: ユーザが操作を行ったその場で、ユーザに結果を返すことになる状況で実行される
    '（ユーザがその場で問題を認識して修正可能である）ため、このメソッドによるチェックは
    '十分すぎるくらい厳しくて構わないものとする。
    '上記のような条件で実行される上、適用リストは入力されたものをそのまま出力しなければ
    'ならないことから、このメソッドには「受信するものには寛容に、送信するものは厳密に」
    'の精神は当てはまらない。
    'むしろ、ここ以降で行われる処理では、発生した問題をユーザがその場で認識することは
    '期待できないし、制約のある機器で行われる故、入力データの書式を限定している可能性も
    '高いため、ここでガードしなければならない。
    '以上の理由から、列の値がダブルクォーテーションで囲まれているファイルなどは
    'CSVとしては正しくても、適用リストの仕様として定義されている書式とは異なるため、
    'ここでNGになるように処理を行う。
    '適用日に関しては、適用リストの仕様書に「日付でも99999999でもブランクでもない」場合の
    '動作が記載されているようにみえるが、これは駅務機器側が想定すべきケースとして
    '記載されているだけであり、運管サーバが許容すべきケースではないものとする。
    Protected Overridable Function RegisterCurDllObjectAsList(ByVal sFilePath As String, ByVal sFileName As String, ByVal sFileHashValue As String) As NakCauseCode
        Log.Info("Registering a list file [" & sFilePath & "]...")

        Dim ret As NakCauseCode = EkNakCauseCode.InvalidContent
        Do
            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()

                Dim sLine As String
                Dim aColumns As String()
                Using oReader As StreamReader _
                   = New StreamReader(sFilePath, Encoding.GetEncoding(932))

                    'ヘッダ部の１行目を読み込む。
                    sLine = oReader.ReadLine()
                    If sLine Is Nothing Then
                        Log.Error("The file is empty.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    'ヘッダ部の１行目を列に分割する。
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 2 Then
                        Log.Error("The first line of the file contains too many or too few columns.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '作成年月日を抽出する。
                    Dim sCreatedDate As String = aColumns(0)
                    Dim createdDate As DateTime
                    If DateTime.TryParseExact(sCreatedDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                        Log.Error("The first line of the file contains illegal created date.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '適用リストバージョンを抽出する。
                    Dim sListVersion As String = aColumns(1)
                    If Not EkMasProListFileName.GetListVersion(sFileName).Equals(sListVersion) Then
                        Log.Error("The first line of the file contains illegal list version.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    'ヘッダ部の２行目を読み込む。
                    sLine = oReader.ReadLine()
                    If sLine Is Nothing Then
                        Log.Error("The file does not have the second line.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    'ヘッダ部の２行目を列に分割する。
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 3 Then
                        Log.Error("The second line of the file contains too many or too few columns.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    'NOTE: 全エリア対応のプログラムを配信する際も適用リストはエリア別に用意できる
                    '仕様にするのであれば、DataPurposeがProgramでかつaColumns(0)が"00"のケースも
                    '許容しなければならない。また、適用リスト内部に記載された（CABに紐づく）
                    'エリアが00であることをDBに記憶し、配信指示でそのレコードが指定された際は、
                    'エリアNoが00のCABを読み出さなければならない。

                    'パターンNo.またはエリアNo.を抽出する。
                    If Not curDll.DataSubKind.Equals(aColumns(0)) Then
                        Log.Error("The second line of the file contains illegal sub kind.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    'マスタバージョンまたは代表バージョンを抽出する。
                    'NOTE: プログラム適用リストの仕様に合わせて、比較相手
                    '（ファイル名から取得したバージョン）の桁数を調整している。
                    'プログラム適用リストの仕様がそうでない（改札機プログラムや
                    '窓処プログラムの代表バージョンを４桁で記述する）ならば、
                    '下記If文の条件は「Not curDll.DataVersion.Equals(aColumns(1))」
                    'にするべきである。
                    Dim sVerFormat As String = If(curDll.DataPurpose.Equals(EkConstants.DataPurposeMaster), "D3", "D8")
                    If Not Integer.Parse(curDll.DataVersion).ToString(sVerFormat).Equals(aColumns(1)) Then
                        Log.Error("The second line of the file contains illegal version.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '機種コードを抽出する。
                    If Not curDll.DataApplicableModel.Equals(aColumns(2)) Then
                        Log.Error("The second line of the file contains illegal model code.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '適用リスト見出しテーブルに同一のファイル名に紐づく既存のレコードがあれば削除する。
                    Dim sSQLToDeleteHeadline As String = _
                       "DELETE FROM S_" & curDll.DataPurpose & "_LIST_HEADLINE" _
                       & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                       & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                       & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                       & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
                       & " AND LIST_VERSION = '" & sListVersion & "'"
                    dbCtl.ExecuteSQLToWrite(sSQLToDeleteHeadline)

                    '適用リスト見出しテーブルに情報を登録する。
                    Dim sSQLToInsertHeadline As String = _
                       "INSERT INTO S_" & curDll.DataPurpose & "_LIST_HEADLINE" _
                       & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, LIST_VERSION, FILE_NAME, HASH_VALUE, FILE_CREATE_DATE)" _
                       & " VALUES (GETDATE()," _
                               & " '" & UserId & "'," _
                               & " '" & MachineId & "'," _
                               & " GETDATE()," _
                               & " '" & UserId & "'," _
                               & " '" & MachineId & "'," _
                               & " '" & curDll.DataApplicableModel & "'," _
                               & " '" & curDll.DataKind & "'," _
                               & " '" & curDll.DataSubKind & "'," _
                               & " '" & curDll.DataVersion & "'," _
                               & " '" & sListVersion & "'," _
                               & " '" & sFileName & "'," _
                               & " '" & sFileHashValue & "'," _
                               & " '" & sCreatedDate & "')"
                    dbCtl.ExecuteSQLToWrite(sSQLToInsertHeadline)

                    '適用リスト内容テーブルに同一のファイル名に紐づく既存のレコードがあれば削除する。
                    Dim sSQLToDelete As String = _
                       "DELETE FROM S_" & curDll.DataPurpose & "_LIST" _
                       & " WHERE FILE_NAME = '" & sFileName & "'"
                    dbCtl.ExecuteSQLToWrite(sSQLToDelete)

                    'データ部を解析する。
                    Dim idealColumnCount As Integer = If(curDll.DataPurpose.Equals(EkConstants.DataPurposeMaster), 3, 4)
                    Dim oAboveLines As New LinkedList(Of String)
                    Dim lineNumber As Integer = 3
                    sLine = oReader.ReadLine()
                    While sLine IsNot Nothing
                        '読み込んだ行を列に分割する。
                        aColumns = sLine.Split(","c)
                        If aColumns.Length <> idealColumnCount Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains too many or too few columns.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If

                        'サイバネ線区駅順コードの書式をチェックする。
                        If aColumns(0).Length <> 6 OrElse _
                           Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal station code.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If

                        'コーナーコードの書式をチェックする。
                        If aColumns(1).Length <> 4 OrElse _
                           Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal corner code.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If

                        '号機番号の書式をチェックする。
                        If aColumns(2).Length <> 2 OrElse _
                           Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                           aColumns(2).Equals("00") Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal unit number.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If

                        If idealColumnCount = 4 Then
                            '適用日のレングスをチェックする。
                            If aColumns(3).Length <> 8 AndAlso aColumns(3).Length <> 0 Then
                                Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal applicable date.")
                                ret = EkNakCauseCode.InvalidContent
                                Exit Do
                            End If

                            '適用日がブランクでない場合、値をチェックする。
                            If aColumns(3).Length = 8 Then
                               If Not Utility.IsDecimalStringFixed(aColumns(3), 0, 8) Then
                                    Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal applicable date.")
                                    ret = EkNakCauseCode.InvalidContent
                                    Exit Do
                                End If

                                If Not aColumns(3).Equals("99999999") AndAlso _
                                   DateTime.TryParseExact(aColumns(3), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                                    Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal applicable date.")
                                    ret = EkNakCauseCode.InvalidContent
                                    Exit Do
                                End If
                            End If
                        End If

                        Dim sLineKey As String = aColumns(0) & aColumns(1) & aColumns(2)
                        If oAboveLines.Contains(sLineKey) Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file is duplicative.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If
                        oAboveLines.AddLast(sLineKey)

                        '行の情報を適用リスト内容テーブルに登録する。
                        Dim sSQLToInsert As String
                        If idealColumnCount = 3 Then
                            sSQLToInsert = _
                               "INSERT INTO S_" & curDll.DataPurpose & "_LIST" _
                               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO)" _
                               & " VALUES (GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " '" & sFileName & "'," _
                                       & " '" & aColumns(0).Substring(0, 3) & "'," _
                                       & " '" & aColumns(0).Substring(3, 3) & "'," _
                                       & " " & aColumns(1) & "," _
                                       & " " & aColumns(2) & ")"
                        Else
                            sSQLToInsert = _
                               "INSERT INTO S_" & curDll.DataPurpose & "_LIST" _
                               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, APPLICABLE_DATE)" _
                               & " VALUES (GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " '" & sFileName & "'," _
                                       & " '" & aColumns(0).Substring(0, 3) & "'," _
                                       & " '" & aColumns(0).Substring(3, 3) & "'," _
                                       & " " & aColumns(1) & "," _
                                       & " " & aColumns(2) & "," _
                                       & " '" & aColumns(3) & "')"
                        End If
                        dbCtl.ExecuteSQLToWrite(sSQLToInsert)

                        sLine = oReader.ReadLine()
                        lineNumber += 1
                    End While
                End Using
                dbCtl.TransactionCommit()
                ret = EkNakCauseCode.None

            Catch ex As DatabaseException
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.TelegramError  'NOTE: 微妙

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent  'NOTE: 微妙

            Finally
                If ret <> EkNakCauseCode.None Then
                    dbCtl.TransactionRollBack()
                End If
                dbCtl.ConnectClose()
            End Try
        Loop While False
        Return ret
    End Function

    Protected Overridable Function RegisterCurDllObjectAsMasterData(ByVal sFilePath As String, ByVal sFileName As String, ByVal sFileHashValue As String) As NakCauseCode
        Log.Info("Registering a master data file [" & sFilePath & "]...")

        Dim ret As NakCauseCode = EkNakCauseCode.InvalidContent
        Do
            Dim oFooter As EkMasterDataFileFooter
            Try
                oFooter = New EkMasterDataFileFooter(sFilePath)
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End Try

            Dim sFooterViolation As String = oFooter.GetFormatViolation()
            If sFooterViolation IsNot Nothing Then
                Log.Error("Footer format error detected." & vbCrLf & sFooterViolation)
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            If Not curDll.DataApplicableModel.Equals(oFooter.ApplicableModel) Then
                Log.Error("ApplicableModel values differ in file name and file footer.")
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            If Not curDll.DataKind.Equals(oFooter.Kind) Then
                Log.Error("Kind values differ in file name and file footer.")
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            If Not curDll.DataVersion.Equals(oFooter.Version) Then
                Log.Error("Version values differ in file name and file footer.")
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()

                'マスタデータ見出しテーブルに同一のファイル名に紐づく既存のレコードがあれば削除する。
                Dim sSQLToDeleteHeadline As String = _
                   "DELETE FROM S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
                   & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                   & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                   & " AND DATA_VERSION = '" & curDll.DataVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteHeadline)

                'マスタデータ見出しテーブルに情報を登録する。
                Dim sSQLToInsertHeadline As String = _
                   "INSERT INTO S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
                   & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, FILE_NAME, HASH_VALUE)" _
                   & " VALUES (GETDATE()," _
                           & " '" & UserId & "'," _
                           & " '" & MachineId & "'," _
                           & " GETDATE()," _
                           & " '" & UserId & "'," _
                           & " '" & MachineId & "'," _
                           & " '" & curDll.DataApplicableModel & "'," _
                           & " '" & curDll.DataKind & "'," _
                           & " '" & curDll.DataSubKind & "'," _
                           & " '" & curDll.DataVersion & "'," _
                           & " '" & sFileName & "'," _
                           & " '" & sFileHashValue & "')"
                dbCtl.ExecuteSQLToWrite(sSQLToInsertHeadline)

                dbCtl.TransactionCommit()
                ret = EkNakCauseCode.None

            Catch ex As DatabaseException
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.TelegramError  'NOTE: 微妙

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent  'NOTE: 微妙

            Finally
                If ret <> EkNakCauseCode.None Then
                    dbCtl.TransactionRollBack()
                End If
                dbCtl.ConnectClose()
            End Try
        Loop While False
        Return ret
    End Function

    Protected Overridable Function RegisterCurDllObjectAsProgramData(ByVal sFilePath As String, ByVal sFileName As String, ByVal sFileHashValue As String) As NakCauseCode
        Log.Info("Registering a program data file [" & sFilePath & "]...")

        Dim ret As NakCauseCode = EkNakCauseCode.InvalidContent
        Do
            '一時作業用ディレクトリを初期化する。
            Log.Info("Initializing directory [" & sTempDirPath & "]...")
            Utility.DeleteTemporalDirectory(sTempDirPath)
            Directory.CreateDirectory(sTempDirPath)

            'CABを展開する。
            Using oProcess As New System.Diagnostics.Process()
                oProcess.StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, "TsbCab.exe")
                oProcess.StartInfo.Arguments = "-x """ & sFilePath & """ """ & sTempDirPath & "\"""
                oProcess.StartInfo.UseShellExecute = False
                oProcess.StartInfo.RedirectStandardInput = True
                oProcess.StartInfo.CreateNoWindow = True
                oProcess.Start()
                Dim oStreamWriter As StreamWriter = oProcess.StandardInput
                oStreamWriter.WriteLine("")
                oStreamWriter.Close()
                oProcess.WaitForExit()
            End Using

            'プログラムバージョンリストのパスを取得する。
            Dim sVerListPath As String
            If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeKanshiban) Then
                sVerListPath = Config.KsbProgramVersionListPathInCab
            ElseIf curDll.DataApplicableModel.Equals(EkConstants.ModelCodeGate) Then
                sVerListPath = Config.GateProgramVersionListPathInCab
            Else
                sVerListPath = Config.MadoProgramVersionListPathInCab
            End If
            sVerListPath = Utility.CombinePathWithVirtualPath(sTempDirPath, sVerListPath)

            'プログラムバージョンリストから機種共通部を読み出す。
            Dim oVerList As EkProgramVersionListHeader
            Try
                oVerList = New EkProgramVersionListHeader(sVerListPath)
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End Try

            '読み出した機種共通部の書式をチェックする。
            Dim sVerListViolation As String = oVerList.GetFormatViolation()
            If sVerListViolation IsNot Nothing Then
                Log.Error("Format error detected in ProgramVersionList file." & vbCrLf & sVerListViolation)
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            '読み出した機種共通部から動作許可日を取得する。
            Dim runnableDate As DateTime = oVerList.RunnableDate

            '全てのプログラムグループのベースパスと、
            '各グループのディレクトリ名の配列および、
            '各グループの表示名の配列を取得する。
            'TODO: 監視盤CABを改札機CABと同じ方法で処理する場合は
            'Configに監視盤のProgramGroupに関するフィールドを用意し、
            'その参照をここで下記変数にセットすること。
            Dim sGroupBasePath As String = Nothing
            Dim aGroupNames As String() = Nothing
            Dim aGroupTitles As String() = Nothing
            If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeGate) Then
                sGroupBasePath = Config.GateProgramGroupBasePathInCab
                aGroupNames = Config.GateProgramGroupNamesInCab
                aGroupTitles = Config.GateProgramGroupTitles
            End If

            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()

                'プログラムデータ見出しテーブルに同一のファイル名に紐づく既存のレコードがあれば削除する。
                Dim sSQLToDeleteHeadline As String = _
                   "DELETE FROM S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
                   & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                   & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                   & " AND DATA_VERSION = '" & curDll.DataVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteHeadline)

                'プログラムデータ内容テーブルに同一のファイル名に紐づく既存のレコードがあれば削除する。
                'NOTE: たとえプログラムバージョンリストのプログラム区分が「差分DLL」であっても、
                '全ファイルの情報を消去した上で、CABで示されるもののみを登録する。
                'おそらく、改札機のCABには差分だけを入れたものは存在せず、監視盤のCABも
                'ファイルが１つである故に差分DLLと全体DLLに違いがなく、窓処については、
                'たとえ差分であっても、プログラムバージョンリストに全ファイルの情報が
                '格納されているため、それでよいものと考えられる。
                Dim sSQLToDelete As String = _
                   "DELETE FROM S_" & curDll.DataPurpose & "_DATA" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDelete)

                'プログラムデータ見出しテーブルに情報を登録する。
                Dim sSQLToInsertHeadline As String = _
                   "INSERT INTO S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
                   & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, FILE_NAME, HASH_VALUE, RUNNABLE_DATE)" _
                   & " VALUES (GETDATE()," _
                           & " '" & UserId & "'," _
                           & " '" & MachineId & "'," _
                           & " GETDATE()," _
                           & " '" & UserId & "'," _
                           & " '" & MachineId & "'," _
                           & " '" & curDll.DataApplicableModel & "'," _
                           & " '" & curDll.DataKind & "'," _
                           & " '" & curDll.DataSubKind & "'," _
                           & " '" & curDll.DataVersion & "'," _
                           & " '" & sFileName & "'," _
                           & " '" & sFileHashValue & "'," _
                           & " '" & runnableDate.ToString("yyyyMMdd") & "')"
                dbCtl.ExecuteSQLToWrite(sSQLToInsertHeadline)

                'プログラムデータ内容テーブルに情報を登録する。
                'TODO: 監視盤CABが改札機CABと同じように、所定ディレクトリにFILELIST.TXTを用意し、
                'そこから参照されるファイルのフッタにバージョンを格納するのであれば、
                '以下の監視盤専用の処理は除去し、次の「ElseIf ...」を
                '「If sGroupBasePath IsNot Nothing」にするべきである。
                If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeKanshiban) Then
                    '登録する情報を成型する。
                    'NOTE: 監視盤から受信するプログラムバージョン情報の仕様に
                    '合わせる。特にsElementIdのファイル名部分には注意。
                    Dim sElementId As String = "00\            "
                    Dim sVersion As String = oVerList.EntireVersion.ToString(EkConstants.ProgramDataVersionFormatOfKanshiban)
                    Dim sDispName As String = "監視盤アプリケーション"

                    'プログラムデータ内容テーブルに登録する。
                    Dim sSQLToInsert As String
                    sSQLToInsert = _
                       "INSERT INTO S_" & curDll.DataPurpose & "_DATA" _
                       & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
                       & " VALUES (GETDATE()," _
                               & " '" & UserId & "'," _
                               & " '" & MachineId & "'," _
                               & " GETDATE()," _
                               & " '" & UserId & "'," _
                               & " '" & MachineId & "'," _
                               & " '" & sFileName & "'," _
                               & " '" & sElementId & "'," _
                               & " '" & sVersion & "'," _
                               & " '" & sDispName & "')"
                    dbCtl.ExecuteSQLToWrite(sSQLToInsert)

                ElseIf curDll.DataApplicableModel.Equals(EkConstants.ModelCodeGate) Then
                    Dim sBaseDirPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, sGroupBasePath)

                    'CAB内の所定ディレクトリを順に処理する。
                    For i As Integer = 0 To aGroupNames.Length - 1
                        'NOTE: aGroupNames(i)の長さが0の場合も想定する（sBaseDirPath直下からファイルを読む）
                        '仕様であるが、実装的にはPath.Combine()の配慮に委ねることにしている。
                        Dim sDirPath As String = Path.Combine(sBaseDirPath, aGroupNames(i))
                        Dim sLine As String

                        'ディレクトリ内にある見出しファイルを解析する。
                        Using oReader As StreamReader _
                           = New StreamReader(Path.Combine(sDirPath, CatalogNameInCabDir), Encoding.GetEncoding(932))

                            '見出しファイルの各行を処理する。
                            Dim lineNumber As Integer = 1
                            sLine = oReader.ReadLine()
                            While sLine IsNot Nothing
                                If Not sLine.StartsWith("/", StringComparison.Ordinal) Then
                                    '見出しファイルの非コメント行からバージョン管理対象となるファイルの名前を取得する。
                                    Dim sElementFileName As String = sLine.Substring(2, 16).TrimEnd(Chr(&H20))
                                    If Not Path.GetFileName(sElementFileName).Equals(sElementFileName, StringComparison.OrdinalIgnoreCase) Then
                                        Log.Error("The line #" & lineNumber.ToString() & " of [" &  Path.Combine(aGroupNames(i), CatalogNameInCabDir) & "] contains illegal file name [" & sElementFileName  & "].")
                                        ret = EkNakCauseCode.InvalidContent
                                        Exit Do
                                    End If

                                    'ファイルのフッタを読み出す。
                                    Dim sElementFilePath As String = Path.Combine(sDirPath, sElementFileName)
                                    Dim oFooter As EkProgramElementFooter
                                    Try
                                        'TODO: 監視盤CABもここで処理することになった場合は、
                                        'curDll.DataApplicableModel.Equals(EkConstants.ModelCodeKanshiban)
                                        'の場合に「EkProgramElementFooterForW」インスタンスを生成するよう
                                        '処理を分岐させること。
                                        oFooter = New EkProgramElementFooterForG(sElementFilePath)
                                    Catch ex As IOException
                                        Log.Error("Exception caught in parsing [" & Path.Combine(aGroupNames(i), sElementFileName) & "].", ex)
                                        ret = EkNakCauseCode.InvalidContent
                                        Exit Do
                                    End Try

                                    '読み出したフッタの書式をチェックする。
                                    Dim sFooterViolation As String = oFooter.GetFormatViolation()
                                    If sFooterViolation IsNot Nothing Then
                                        Log.Error("Footer format error detected in [" & Path.Combine(aGroupNames(i), sElementFileName) & "]." & vbCrLf & sFooterViolation)
                                        ret = EkNakCauseCode.InvalidContent
                                        Exit Do
                                    End If

                                    'NOTE: 同じディレクトリに拡張子のみが異なるファイルが格納されている場合、
                                    '同一表示名でそれらの行が登録されるはずであるが、そもそも、今後の
                                    '改札機用には、そのようなCABは用意しないことになっている。

                                    'フッタの情報を成型する。
                                    Dim sElementId As String = i.ToString("D2") & "\" & sElementFileName.ToUpperInvariant()
                                    Dim sVersion As String = oFooter.Version
                                    Dim sDispName As String = oFooter.DispName
                                    If aGroupTitles(i).Length <> 0 Then
                                        sDispName = aGroupTitles(i) & "\" & Path.GetFileNameWithoutExtension(sElementFileName)
                                    End If

                                    '成型した情報をプログラムデータ内容テーブルに登録する。
                                    Dim sSQLToInsert As String
                                    sSQLToInsert = _
                                       "INSERT INTO S_" & curDll.DataPurpose & "_DATA" _
                                       & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
                                       & " VALUES (GETDATE()," _
                                               & " '" & UserId & "'," _
                                               & " '" & MachineId & "'," _
                                               & " GETDATE()," _
                                               & " '" & UserId & "'," _
                                               & " '" & MachineId & "'," _
                                               & " '" & sFileName & "'," _
                                               & " '" & sElementId & "'," _
                                               & " '" & sVersion & "'," _
                                               & " '" & sDispName & "')"
                                    dbCtl.ExecuteSQLToWrite(sSQLToInsert)
                                End If

                                sLine = oReader.ReadLine()
                                lineNumber += 1
                            End While
                        End Using
                    Next

                Else
                    'NOTE: 適用対象機種が窓処の場合である。
                    Dim aElements As EkMadoProgramVersionInfoElement()

                    'プログラムバージョンリストから窓処のプログラムバージョン情報を読み出す。
                    Using oInputStream As New FileStream(sVerListPath, FileMode.Open, FileAccess.Read)
                        aElements = EkMadoProgramVersionInfoReader.GetElementsFromStream(oInputStream)
                    End Using

                    '読み出したバージョン情報の各レコードを処理する。
                    For i As Integer = 0 To aElements.Length - 1
                        If aElements(i).IsVersion Then
                            '表示対象レコードの場合は、情報を成型する。
                            Dim sElementId As String = i.ToString("D2")
                            Dim sVersion As String = aElements(i).Value
                            Dim sDispName As String = aElements(i).Name.Replace("バージョン", "")

                            '成型した情報をプログラムデータ内容テーブルに登録する。
                            Dim sSQLToInsert As String
                            sSQLToInsert = _
                               "INSERT INTO S_" & curDll.DataPurpose & "_DATA" _
                               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
                               & " VALUES (GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " '" & sFileName & "'," _
                                       & " '" & sElementId & "'," _
                                       & " '" & sVersion & "'," _
                                       & " '" & sDispName & "')"
                            dbCtl.ExecuteSQLToWrite(sSQLToInsert)
                        End If
                    Next
                End If

                dbCtl.TransactionCommit()
                ret = EkNakCauseCode.None

            Catch ex As DatabaseException
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.TelegramError  'NOTE: 微妙

            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent

            Catch ex As FormatException
                Log.Error("Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent  'NOTE: 微妙

            Finally
                If ret <> EkNakCauseCode.None Then
                    dbCtl.TransactionRollBack()
                End If
                dbCtl.ConnectClose()
            End Try
        Loop While False

        '一時作業用ディレクトリを削除する。
        Log.Info("Sweeping directory [" & sTempDirPath & "]...")
        Utility.DeleteTemporalDirectory(sTempDirPath)

        Return ret
    End Function
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
