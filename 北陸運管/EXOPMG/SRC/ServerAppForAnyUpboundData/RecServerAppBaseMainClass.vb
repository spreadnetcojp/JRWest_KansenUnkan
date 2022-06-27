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
Imports System.Messaging
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' 登録プロセス共通のメイン処理を実装するクラス。
''' </summary>
Public Class RecServerAppBaseMainClass
    Inherits ServerAppBaseMainClass

#Region "内部クラス等"
    Protected Delegate Function RecordToDatabaseDelegate(ByVal sFilePath As String) As RecordingResult

    Protected Enum RecordingResult As Integer
        Success
        IOError
        ParseError
    End Enum
#End Region

#Region "定数や変数"
    'メインウィンドウ
    Protected Shared oMainForm As ServerAppForm

    '未処理データが格納されているディレクトリのパス
    Protected Shared sInputDirPath As String

    '内部都合で登録できなかったデータを格納するディレクトリのパス
    Protected Shared sSuspenseDirPath As String

    '書式異常で登録できなかったデータを格納するディレクトリのパス
    Protected Shared sQuarantineDirPath As String

    '登録済みデータを格納するディレクトリのパス
    Protected Shared sTrashDirPath As String

    '枝番の最大値
    Private Shared maxBranchNumber As Integer

    'データ登録スレッドへの終了要求フラグ
    Private Shared quitListener As Integer

    'データ登録メソッドへのデリゲート
    Private Shared oRecordToDatabaseDelegate As RecordToDatabaseDelegate
#End Region

#Region "メソッド"
    ''' <summary>
    ''' 登録プロセスの共通メイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 各登録プロセスのメイン処理から呼び出す。
    ''' </remarks>
    Protected Shared Sub RecServerAppBaseMain(ByVal oArgRecordToDatabaseDelegate As RecordToDatabaseDelegate)
        Try
            oRecordToDatabaseDelegate = oArgRecordToDatabaseDelegate

            'メッセージループがアイドル状態になる前（かつ、定期的にそれを行う
            'スレッドを起動する前）に、生存証明ファイルを更新しておく。
            Directory.CreateDirectory(RecServerAppBaseConfig.ResidentAppPulseDirPath)
            ServerAppPulser.Pulse()

            oMainForm = New ServerAppForm()

            'データ登録スレッドを開始する。
            Dim oRecorderThread As New Thread(AddressOf RecServerAppBaseMainClass.RecordingLoop)
            Log.Info("Starting the recorder thread...")
            quitListener = 0
            oRecorderThread.Name = "Recorder"
            oRecorderThread.Start()

            'ウインドウプロシージャを実行する。
            'NOTE: このメソッドから例外がスローされることはない。
            ServerAppBaseMain(oMainForm)

            Try
                'データ登録スレッドに終了を要求する。
                Log.Info("Sending quit request to the recorder thread...")
                Thread.VolatileWrite(quitListener, 1)

                'NOTE: 以下でデータ登録スレッドが終了しない場合、
                'データ登録スレッドは生存証明を行わないはずであり、
                '状況への対処はプロセスマネージャで行われる想定である。

                'データ登録スレッドの終了を待つ。
                Log.Info("Waiting for the recorder thread to quit...")
                oRecorderThread.Join()
                Log.Info("The recorder thread has quit.")
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                oRecorderThread.Abort()
            End Try
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            If oMainForm IsNot Nothing Then
                oMainForm.Dispose()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' データ登録スレッドのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' データ登録を行う。
    ''' </remarks>
    Private Shared Sub RecordingLoop()
        Dim oMessageQueue As MessageQueue = Nothing
        Try
            Log.Info("The recorder thread started.")

            sInputDirPath = RecServerAppBaseConfig.InputDirPathForApps(RecServerAppBaseConfig.AppIdentifier)
            sSuspenseDirPath = RecServerAppBaseConfig.SuspenseDirPathForApps(RecServerAppBaseConfig.AppIdentifier)
            sQuarantineDirPath = RecServerAppBaseConfig.QuarantineDirPathForApps(RecServerAppBaseConfig.AppIdentifier)
            sTrashDirPath = RecServerAppBaseConfig.TrashDirPathForApps(RecServerAppBaseConfig.AppIdentifier)
            maxBranchNumber = RecServerAppBaseConfig.MaxBranchNumberForApps(RecServerAppBaseConfig.AppIdentifier)

            'アクセスする予定のディレクトリについて、無ければ作成しておく。
            'NOTE: 基底クラスが作成するものや、必ずサブディレクトリの作成から
            '行うことになるものについては、対象外とする。
            Directory.CreateDirectory(sInputDirPath)

            Dim oDiagnosisTimer As New TickTimer(RecServerAppBaseConfig.SelfDiagnosisIntervalTicks)
            Dim isInitial As Boolean = True
            Dim fewSpan As New TimeSpan(0, 0, 0, 0, RecServerAppBaseConfig.PollIntervalTicks)
            Dim oFilter As New MessagePropertyFilter()
            oFilter.ClearAll()
            'oFilter.AppSpecific = True
            'oFilter.Body = True

            'NOTE: RecServerAppBaseConfig.MyMqPathのメッセージキューは、
            'この時点で必ず存在している前提である。存在していなければ、
            'システムに異常がある故、このプロセスは起動直後に終了するべき
            'である。
            oMessageQueue = New MessageQueue(RecServerAppBaseConfig.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([String])})

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())
            While Thread.VolatileRead(quitListener) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()
                End If

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
                '行われることはないだろうし、ここでTimeSpan.Zeroを渡して、
                '別の方法でCPUの解放期間を作るするようにすれば、
                'メッセージ受信に対する反応が悪くなる（メッセージ処理
                '性能が低下する）はずであるため、以下のとおり、
                'MessageQueue.Receive()で待つことにしている。
                Try
                    oMessageQueue.Receive(fewSpan)
                Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                    'プロセスを起動してから一度でも登録処理を実施しているならば、
                    'メッセージ受信待ちに戻る。
                    If Not isInitial Then Continue While
                End Try

                isInitial = False
                While Thread.VolatileRead(quitListener) = 0
                    systemTick = TickTimer.GetSystemTick()
                    If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                        oDiagnosisTimer.Start(systemTick)
                        ServerAppPulser.Pulse()
                    End If

                    'キューイングされているメッセージを全て読み捨てる。
                    oMessageQueue.Purge()

                    '最も古いファイルを１件処理する。
                    'ファイルがない場合は、メッセージ受信待ちに戻る。
                    If DispatchEarliestFile() = False Then Exit While
                End While
            End While
            Log.Info("Quit requested by manager.")
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP発生（または収集データ誤記テーブルへの登録）は、
            'プロセスマネージャが行うので、ここでは不要である。

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        Finally
            If oMessageQueue IsNot Nothing Then
                oMessageQueue.Close()
            End If
        End Try
    End Sub

    Private Shared Function DispatchEarliestFile() As Boolean
        '未処理データの格納ディレクトリから所定パターンの名前を持つ
        '最古のファイルを検索する。
        'NOTE: 未処理データの格納ディレクトリは、この時点で（この
        'プロセスの権限でアクセス可能な状態で）必ず存在している
        'という前提である。存在していなければ、システムに異常が
        'ある故、このプロセスは起動直後に終了するべきである。
        Dim oEarliestFileInfo As FileInfo = UpboundDataPath.FindEarliest(sInputDirPath)
        If oEarliestFileInfo Is Nothing Then Return False

        'ファイルの内容をデータベースに反映する。
        Log.Info("ファイル[" & oEarliestFileInfo.Name & "]の登録を行います...")
        Dim result As RecordingResult = oRecordToDatabaseDelegate(oEarliestFileInfo.FullName)

        '反映の結果により、ファイルの新パスの結果別ディレクトリまでを決める。
        Dim sDestPath As String
        Select Case result
            Case RecordingResult.Success
                sDestPath = sTrashDirPath
            Case RecordingResult.IOError
                sDestPath = sSuspenseDirPath
            Case RecordingResult.ParseError
                sDestPath = sQuarantineDirPath
            Case Else
                Debug.Fail("This case is impermissible.")
                Return True
        End Select

        'ファイル名の「年月日」部分をもとに、ファイルの新パスの日付別ディレクトリまでを決める。
        sDestPath = Path.Combine(sDestPath, UpboundDataPath.GetDateString(oEarliestFileInfo.Name))

        'NOTE: sDestPathと衝突する名前のファイルは存在しないという前提である。
        'またディレクトリとして既に存在している場合は、書込可能という前提である。
        If Not Directory.Exists(sDestPath) Then
            'ディレクトリが存在していない場合である。
            'ディレクトリを作成した上で、旧パスのファイル名をそのまま結合して、
            '新パスを完成させる。
            Directory.CreateDirectory(sDestPath)
            sDestPath = Path.Combine(sDestPath, oEarliestFileInfo.Name)
        Else
            'ディレクトリが存在している場合である。
            sDestPath = UpboundDataPath.Gen(sDestPath, oEarliestFileInfo.Name)
        End If

        If UpboundDataPath.GetBranchNumber(sDestPath) <= maxBranchNumber Then
            'ファイルを新パスに移動する。
            'NOTE: ファイルは書込可能という前提である。
            File.Move(oEarliestFileInfo.FullName, sDestPath)
            Log.Info("ファイルを[" & sDestPath & "]に移動しました。")
        Else
            'ファイルを削除する。
            'NOTE: ファイルは書込可能という前提である。
            'NOTE: もし、ノードの削除が他のファイル操作と並行して行われ得るとしても、
            'ディレクトリからみえなくなれば、通信系プロセスは受信したファイルを
            '当該ディレクトリの同名エントリに問題なくMove可能とする。
            File.Delete(oEarliestFileInfo.FullName)
            Log.Warn("ファイルを削除しました。")
        End If

        Return True
    End Function
#End Region

End Class
