' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/04/10  (NES)小林  次世代車補対応にて新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Linq
Imports System.Messaging
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

'NOTE: 将来、このプロセスと同じ方法で保守系データの登録を行うように改善を
'行うなら、このクラスはConfigやRecorderとともに汎用性のあるクラスにして、
'ServerAppForAnyUpboundData2プロジェクトに移動する。
'それらのクラスでは、利用データや座席データに依存した値を（派生クラスが
'セットする想定の）Immutableなメンバ変数から参照するようにし、また、
'ファイルから読み込むSQLを配列などで管理することにして、任意の数のテーブルに
'対してInsert等を行えるようにする。もしくはSQLファイル側で複文を記述する
'ルールにして、1ファイルに限定する方針でもよい。
'ServerAppForRiyoDataプロジェクトは、これらの派生クラスを用意することになり、
'派生クラスの主な実装は、コンストラクタにおける「利用データや指定券データに
'依存した値のメンバ変数へのセット」だけになるはずである。

''' <summary>
''' 駅別データ登録スレッドを対象とする監視スレッド。
''' </summary>
Public Class MyWatcher

#Region "内部クラス等"
    Protected Enum TargetState
        Registered
        Started
        Aborted
        WaitingForRestart
        QuitRequested
        Discarded
    End Enum

    Protected Class Target
        Public State As TargetState
        Public Code As EkCode
        Public Recorder As MyRecorder
    End Class
#End Region

#Region "定数や変数"
    'スレッド名
    Protected Const ThreadName As String = "Watcher"

    '登録スレッド名の書式
    Protected Const RecorderNameFormat As String = "%3R%3S"

    '登録スレッドのAbort応答期限
    Protected Const RecorderAbortLimitTicks As Integer = 5000  'TODO: 設定から取得する？

    'クライアントのリスト
    Protected oTargetList As LinkedList(Of Target) 'OPT: Dictionaryに変更？

    'スレッド
    Protected oThread As Thread

    'メインウィンドウ
    Protected oMainForm As Form

    '親スレッドからの終了要求
    Private _IsQuitRequest As Integer
#End Region

#Region "プロパティ"
    Protected Property IsQuitRequest() As Boolean
        Get
            Return CBool(Thread.VolatileRead(_IsQuitRequest))
        End Get

        Set(ByVal val As Boolean)
            Thread.VolatileWrite(_IsQuitRequest, CInt(val))
        End Set
    End Property
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal oMainForm As Form)
        Me.oThread = New Thread(AddressOf Me.Task)
        Me.oThread.Name = ThreadName
        Me.oMainForm = oMainForm
        Me.IsQuitRequest = False
    End Sub
#End Region

#Region "親スレッド用メソッド"
    Public Sub Start()
        oThread.Start()
    End Sub

    Public Sub Quit()
        IsQuitRequest = True
    End Sub

    Public Sub Join()
        oThread.Join()
    End Sub

    Public Function Join(ByVal millisecondsTimeout As Integer) As Boolean
        Return oThread.Join(millisecondsTimeout)
    End Function

    'NOTE: このクラスに問題がない限り、Quit()で済ませるべきである。
    Public Sub Abort()
        oThread.Abort()
    End Sub

    Public ReadOnly Property ThreadState() As ThreadState
        Get
            Return oThread.ThreadState
        End Get
    End Property
#End Region

#Region "メソッド"
    ''' <summary>
    ''' 監視スレッドのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' Recorderの管理を行う。
    ''' </remarks>
    Private Sub Task()
        Dim oMessageQueue As MessageQueue = Nothing
        Try
            Log.Info("The watcher thread started.")
            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            Dim fewSpan As New TimeSpan(0, 0, 0, 0, Config.PollIntervalTicks)
            Dim oFilter As New MessagePropertyFilter()
            oFilter.ClearAll()
            oFilter.AppSpecific = True
            oFilter.Body = True

            'NOTE: Config.MyMqPathのメッセージキューは、
            'この時点で必ず存在している前提である。存在していなければ、
            'システムに異常がある故、このプロセスは起動直後に終了するべき
            'である。
            oMessageQueue = New MessageQueue(Config.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([String])})

            oTargetList = New LinkedList(Of Target)

            ProcOnManagementReady()

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Not IsQuitRequest
                '他のプロセスからのメッセージをチェックする。
                Dim oMessage As Message = Nothing
                Try
                    '所定時間メッセージを待つ。
                    'NOTE: MessageQueue.Receive()のタイムアウトは、実際に
                    '引数の時間が経過した際ではなく、呼び出し時点の
                    'システム時刻に引数の時間を加えた時刻Tを求めた上で、
                    'システム時刻がT以上になった際に行われるようである。
                    'つまり、呼び出しの間にシステム時刻が1時間戻されれば、
                    '呼び出しから戻るのは、「引数の時間+1時間」経過後に
                    'なってしまい、その間は、子スレッドの生存監視や
                    '親プロセスへの生存証明を行うことができなくなる。
                    'メインスレッドからの終了要求にも反応できなくなる。
                    'しかし、そのことが問題になるような大きな時刻補正が
                    '行われることはないという前提で、ここで待つことに
                    'している。実際に大きな時刻補正があるなら注意。
                    oMessage = oMessageQueue.Receive(fewSpan)
                Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                    'タイムアウトの場合である。この例外については握りつぶして、
                    'oMessage Is Nothingのまま、以下を実行する。
                End Try

                If oMessage IsNot Nothing Then
                    ProcOnMessageReceive(oMessage)
                End If

                '以上の処理でAbortRecorderの対象になったTargetについて、
                'ProcOnRecorderAbortを呼び出す。
                'その中でAbortRecorderの対象になったTargetについては、
                '次回にProcOnRecorderAbortを呼び出す。
                PrepareToRestartRecorders()

                '前回チェックから所定時間経過している場合は、全ての
                '登録スレッドについて、異常終了またはフリーズ
                'していないかチェックする。
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()

                    Log.Info("Checking pulse of all Recorders...")
                    For Each oTarget As Target In oTargetList
                        If oTarget.State = TargetState.Started Then
                            If oTarget.Recorder.ThreadState = ThreadState.Stopped Then
                                '予期せぬ例外などで異常終了している場合である。
                                Log.Fatal("Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "] has stopped.")
                                AbortRecorder(oTarget)
                            ElseIf TickTimer.GetTickDifference(systemTick, oTarget.Recorder.LastPulseTick) > Config.RecorderPendingLimitTicks Then
                                'フリーズしている場合である。
                                Log.Fatal("Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "] seems broken.")
                                AbortRecorder(oTarget)
                            End If
                        End If
                    Next oTarget
                    PrepareToRestartRecorders()
                    RestartRecorders()
                End If
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP発生（または収集データ誤記テーブルへの登録）は、
            'プロセスマネージャが行うので、ここでは不要である。

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        Finally
            If oTargetList IsNot Nothing
                '全ての登録スレッドに終了を要求する。
                'NOTE: ここでは、登録スレッドを作成した後、登録スレッドを
                'スタートさせる前に例外が発生した場合や、
                'スタート後の登録スレッドがAbortしている場合など
                'を考慮した実装を行っている。
                For Each oTarget As Target In oTargetList
                    If oTarget.State = TargetState.Started OrElse _
                       oTarget.State = TargetState.Aborted OrElse _
                       oTarget.State = TargetState.WaitingForRestart Then
                        Try
                            QuitRecorder(oTarget)
                        Catch ex As Exception
                            Log.Fatal("Unwelcome Exception caught.", ex)
                        End Try
                    End If
                Next oTarget

                '終了を要求した登録スレッドの終了を待つ。
                'NOTE: 実際にJoinを行うのは、QuitRecorderの対象に
                'なったスレッド（つまり、スタート済みのスレッド）
                'のみとなるため、ThreadStateExceptionが発生する
                '可能性はないものとする。
                WaitForRecordersToQuit()

                '不要になったクライアントを登録解除する。
                UnregisterDiscardedTargets()
            End If

            If oMessageQueue IsNot Nothing Then
                oMessageQueue.Close()
            End If
        End Try
    End Sub

    Protected Function FindTarget(ByVal code As EkCode) As Target
        For Each oTarget As Target In oTargetList
            If oTarget.Code = code Then Return oTarget
        Next oTarget
        Return Nothing
    End Function

    Protected Sub RegisterTarget(ByVal code As EkCode)
        Log.Info("Registering Recorder [" & code.ToString(RecorderNameFormat) & "]...")
        Dim oRecorder As New MyRecorder( _
           code.ToString(RecorderNameFormat), _
           code, _
           Not Config.ResidentApps.Contains("ToNkan"))
        Dim oTarget As New Target()
        oTarget.State = TargetState.Registered
        oTarget.Code = code
        oTarget.Recorder = oRecorder
        oTargetList.AddLast(oTarget)
    End Sub

    Protected Sub StartRecorder(ByVal oTarget As Target)
        Debug.Assert(oTarget.State = TargetState.Registered)

        Log.Info("Starting Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "]...")
        oTarget.Recorder.Start()
        oTarget.State = TargetState.Started
    End Sub

    Protected Sub AbortRecorder(ByVal oTarget As Target)
        Debug.Assert(oTarget.State <> TargetState.Registered)
        Debug.Assert(oTarget.State <> TargetState.QuitRequested)
        Debug.Assert(oTarget.State <> TargetState.Discarded)

        If oTarget.State <> TargetState.Started AndAlso
           oTarget.State <> TargetState.WaitingForRestart Then
            Log.Warn("The Recorder is already marked as broken.")
            Return
        End If

        If oTarget.State = TargetState.Started Then
            If oTarget.Recorder.ThreadState <> ThreadState.Stopped Then
                oTarget.Recorder.Abort()

                'NOTE: Abort()の結果、oTarget.Recorderは例外をキャッチしてログを
                '出力する可能性がある。また、こちらがAbort()から戻ってきた時点で、
                '既に例外処理が開始されていることは最低限保証されていてほしいが、
                'msdnをみた感じだといまいち不明であるため、スレッドが終了状態に
                'ならない限りは、通信相手に関するその他のグローバルな情報もまだ更新
                'する可能性があると考えるべきである。よって、できる限り終了を待って
                'から、新たなRecorderをスタートさせる。
                If oTarget.Recorder.Join(RecorderAbortLimitTicks) = False Then
                    Log.Warn("The Recorder may refuse to abort.")
                End If
            End If
            oTarget.Recorder = Nothing
        End If

        'NOTE: 再帰呼び出しが発生しないよう、ここで
        'ProcOnRecorderAbort(oTarget)は行わない。
        oTarget.State = TargetState.Aborted
    End Sub

    Protected Sub PrepareToRestartRecorders()
        For Each oTarget As Target In oTargetList
            If oTarget.State = TargetState.Aborted Then
                ProcOnRecorderAbort(oTarget)
                oTarget.State = TargetState.WaitingForRestart
            End If
        Next oTarget
    End Sub

    'NOTE: 強制終了、再起動、強制終了、再起動が短い周期で繰り返される可能性を考慮し、
    'これは、自己診断の周期で呼ぶ方が無難である。
    Protected Sub RestartRecorders()
        For Each oTarget As Target In oTargetList
            If oTarget.State = TargetState.WaitingForRestart Then
                Log.Info("Renewing Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "]...")
                oTarget.Recorder = New MyRecorder( _
                   oTarget.Code.ToString(RecorderNameFormat), _
                   oTarget.Code, _
                   Not Config.ResidentApps.Contains("ToNkan"))

                Log.Info("Restarting Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "]...")
                oTarget.Recorder.Start()
                oTarget.State = TargetState.Started

                ProcOnRecorderRestart(oTarget)
            End If
        Next oTarget
    End Sub

    Protected Sub QuitRecorder(ByVal oTarget As Target)
        Debug.Assert(oTarget.State <> TargetState.Registered)
        Debug.Assert(oTarget.State <> TargetState.QuitRequested)
        Debug.Assert(oTarget.State <> TargetState.Discarded)

        If oTarget.State <> TargetState.Started Then
            Log.Warn("The Recorder is already marked as broken.")
            If oTarget.State = TargetState.Aborted Then
                ProcOnRecorderAbort(oTarget)
            End If
            oTarget.State = TargetState.Discarded
            Return
        End If

        Log.Info("Sending quit request to Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "]...")
        Try
            'OPT: Quitの実装上、例外が発生する可能性を考慮することは必須でない。
            'また、oTarget.Recorder.Quit()で例外が発生するケースでは、
            '結局、oTarget.Recorder.Abort()などでも例外が発生すると思われる。
            oTarget.Recorder.Quit()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            If oTarget.Recorder.ThreadState <> ThreadState.Stopped Then
                oTarget.Recorder.Abort()
                If oTarget.Recorder.Join(RecorderAbortLimitTicks) = False Then
                    Log.Warn("The Recorder may refuse to abort.")
                End If
            End If
            oTarget.State = TargetState.Discarded
            Return
        End Try
        oTarget.State = TargetState.QuitRequested
    End Sub

    Protected Sub WaitForRecordersToQuit()
        Dim oJoinLimitTimer As New TickTimer(Config.RecorderPendingLimitTicks)
        oJoinLimitTimer.Start(TickTimer.GetSystemTick())
        For Each oTarget As Target In oTargetList
            If oTarget.State = TargetState.QuitRequested Then
                Dim ticks As Long = oJoinLimitTimer.GetTicksToTimeout(TickTimer.GetSystemTick())
                If ticks < 0 Then ticks = 0

                If oTarget.Recorder.Join(CInt(ticks)) = False Then
                    Log.Fatal("Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "] seems broken.")
                    oTarget.Recorder.Abort()
                    If oTarget.Recorder.Join(RecorderAbortLimitTicks) = False Then
                        Log.Warn("The Recorder may refuse to abort.")
                    End If
                Else
                    Log.Info("Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "] has quit.")
                End If
                oTarget.State = TargetState.Discarded
            End If
        Next oTarget
    End Sub

    Protected Sub UnregisterDiscardedTargets()
        Dim oNode As LinkedListNode(Of Target) = oTargetList.First
        While oNode IsNot Nothing
            Dim oTarget As Target = oNode.Value
            If oTarget.State = TargetState.Discarded Then
                Dim oDiscardedNode As LinkedListNode(Of Target) = oNode
                oNode = oNode.Next
                oTargetList.Remove(oDiscardedNode)
                Log.Info("Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "] unregistered.")
            Else
                oNode = oNode.Next
            End If
        End While
    End Sub

    Protected Overridable Function SelectStationsInService(ByVal sServiceDate As String) As DataTable
        '機器構成マスタにある「機種が監視盤または窓処」かつ「開始日がsServiceDate以前」の
        'レコードの駅コードを取得する。

        'NOTE: 運管サーバに対する利用データの送信元となる（個体数が最小となる）機種という
        'ことで、WとYを対象にしているが、監視盤が複数の駅を担当する可能性があるなら、
        'WはGに変更するべきかもしれない。ただし、そうする必要がある仕様なら、同一の
        '監視盤から受信した利用データであっても、改札機の設置駅をみて、登録先の
        'テーブルを選ぶ必要があるわけで、ここ以外にも設計の変更が必要になる。
        Dim sSQL As String = _
           "SELECT DISTINCT RAIL_SECTION_CODE, STATION_ORDER_CODE" _
           & " FROM M_MACHINE" _
           & " WHERE (MODEL_CODE = 'W' OR MODEL_CODE = 'Y')" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return dbCtl.ExecuteSQLToRead(sSQL)

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function
#End Region

#Region "イベント処理メソッド"
    Protected Overridable Sub ProcOnManagementReady()
        'クライアントを登録する。
        'NOTE: 起動時なので、万が一、ランタイムな例外が発生した場合は、
        'プロセス終了とする。
        Dim serviceStations As DataRowCollection = SelectStationsInService(EkServiceDate.GenString()).Rows
        For Each serviceStation As DataRow In serviceStations
            Dim code As EkCode
            code.RailSection = Integer.Parse(serviceStation.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(serviceStation.Field(Of String)("STATION_ORDER_CODE"))
            RegisterTarget(code)
        Next serviceStation

        '全クライアントの電文送受信スレッドを開始する。
        For Each oTarget As Target In oTargetList
            StartRecorder(oTarget)
        Next oTarget
    End Sub

    Protected Overridable Sub ProcOnRecorderAbort(ByVal oTarget As Target)
        'NOTE: 既に停止して再起動待ちの状態（TargetState.WaitingForRestart）の
        'oTargetについては、それに対するメッセージ送信を試みた際、
        '再度このメソッドが呼び出されるようになっている。
        'この仕様は、TelServerAppListenerの実装を流用していることに起因しており、
        '利用データ登録プロセスのWatcherにとっては、特に意味があるわけではない。

        '収集データ誤記テーブルに異常を登録する。
        'NOTE: 上記の仕様ゆえ、再起動待ちのoTargetに対するメッセージ送信が
        'あれば、何度でもここが実行されることになるが、何度登録しても、
        '特に問題ないはずであるため、状態の管理は行わず、無条件に
        '登録を行うことにしている。
        Using curProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess()
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtThreadAbended.Gen(curProcess.ProcessName, oTarget.Code.ToString(RecorderNameFormat)))
        End Using
    End Sub

    Protected Overridable Sub ProcOnRecorderRestart(ByVal oTarget As Target)
    End Sub

    Protected Overridable Sub ProcOnMessageReceive(ByVal oMessage As Message)
        Select Case oMessage.AppSpecific
            Case ExtServiceDateChangeNotice.FormalKind
                Log.Info("ExtServiceDateChangeNotice received.")
                ProcOnServiceDateChangeNoticeReceive(oMessage)
            Case Else
                Log.Error("Unwelcome ExtMessage received.")
        End Select
    End Sub

    Protected Overridable Sub ProcOnServiceDateChangeNoticeReceive(ByVal oMessage As Message)
        '機器構成マスタから、現在の運用日付で運用されるべき全ての駅を検索する。
        Dim oServiceStationTable As DataTable
        Try
            oServiceStationTable = SelectStationsInService(EkServiceDate.GenString())
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'ユーザが気付く場所に異常を記録する。
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtMachineMasterErratumDetected.Gen())
            Return
        End Try

        Dim serviceStations As EnumerableRowCollection(Of DataRow) = oServiceStationTable.AsEnumerable()

        '既に登録している駅に関して、検索の結果に含まれていない場合は、
        '当該駅用の登録スレッドに終了を要求する。
        For Each oTarget As Target In oTargetList
            Dim code As EkCode = oTarget.Code
            Dim num As Integer = ( _
               From serviceStation In serviceStations _
               Where serviceStation.Field(Of String)("RAIL_SECTION_CODE") = code.RailSection.ToString("D3") And _
                     serviceStation.Field(Of String)("STATION_ORDER_CODE") = code.StationOrder.ToString("D3") _
               Select serviceStation _
            ).Count

            If num = 0 Then
                'NOTE: 駅が廃止になった場合、当該運用日までの全ての利用データの登録が
                '運用日付の切り替え時刻までの間に済んでいる想定である。
                QuitRecorder(oTarget)
            End If
        Next oTarget

        '終了を待つ。
        WaitForRecordersToQuit()

        '登録解除する。
        UnregisterDiscardedTargets()

        '検索で得た駅に関して、登録されていないものは、登録する。
        For Each row As DataRow In oServiceStationTable.Rows
            Dim code As EkCode
            code.RailSection = Integer.Parse(row.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(row.Field(Of String)("STATION_ORDER_CODE"))
            Dim oTarget As Target = FindTarget(code)
            If oTarget Is Nothing Then
                RegisterTarget(code)
            End If
        Next row

        '登録した駅の登録スレッドを開始させる。
        For Each oTarget As Target In oTargetList
            If oTarget.State = TargetState.Registered Then
                StartRecorder(oTarget)
            End If
        Next oTarget
    End Sub
#End Region

End Class
