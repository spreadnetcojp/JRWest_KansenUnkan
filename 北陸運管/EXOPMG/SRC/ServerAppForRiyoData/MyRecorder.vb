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

Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' 利用データ登録スレッド。
''' </summary>
Public Class MyRecorder

#Region "定数や変数"
    '利用データテーブル名の書式
    Protected Const StaFormat As String = "%3R%3S"

    'スレッド
    Private oThread As Thread

    '対象駅
    Private sTargetSta As String

    '未処理データが格納されているディレクトリのパス
    Private sInputDirPath As String

    '登録済みデータを格納するディレクトリのパス
    Private sOutputDirPath As String

    '日付別のディレクトリを作成する必要があるか
    Private needsDateDir As Boolean

    '所定時間よりも短い間隔でSystemTickを書き込む（0～0xFFFFFFFF）
    Private _LastPulseTick As Long

    '親スレッドからの終了要求
    Private _IsQuitRequest As Integer
#End Region

#Region "コンストラクタ"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal targetEkCode As EkCode, _
       ByVal needsDateDir As Boolean)

        Me.sTargetSta = targetEkCode.ToString(StaFormat)
        CreateTables()
        CreateProcs()

        Dim sBaseDirPath As String = Utility.CombinePathWithVirtualPath(Config.RiyoDataDirPath, targetEkCode.ToString(Config.RiyoDataStationBaseDirNameFormat))
        Me.sInputDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataInputDirPathInStationBase)
        Me.sOutputDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataOutputDirPathInStationBase)

        Me.needsDateDir = needsDateDir

        Me.oThread = New Thread(AddressOf Me.Task)
        Me.oThread.Name = sThreadName
        Me.LastPulseTick = 0
        Me.IsQuitRequest = False
    End Sub
#End Region

#Region "プロパティ"
    'NOTE: 子スレッドを開始して以降の_LastPulseTickは、カーネルを介した排他制御
    'なしに、子スレッドで書き込み、親スレッドで読み出すことにしている。
    'なお、_LastPulseTickは、実際的には、x86-64プロセッサにおける通常の
    '転送命令１つで（即ち、少なくとも割込による分断は無しに）全体を読む（書く）
    'ことが可能なサイズであり、複数コアによるバスオペレーションレベルでも
    '読み書きが分割されることのない位置に配置されていると思われる。また、
    '書き込みを行うスレッドが１つであるため、書き込みの競合についてのケアも
    '不要である。しかしながら、ThreadクラスのVolatileReadやVolatileWriteは
    '使用しない方針とする。これらのメソッドは不可分な動作を意図している
    'わけではない（たとえば、VolatileWriteは、VolatileReadを使用する別の
    'スレッドからの可視性を保証していても、不可分に見える書き換えを保証している
    'わけではない）と思われるのに対し、これらの変数に格納する値は、一応全バイト
    'で意味を成すものであるためである。_LastPulseTickは、死活監視に使うため
    'の重要な変数であるから、パフォーマンス上のよほどの必要性がない限り
    '（LOCK信号によるバスの性能低下すら問題となるような状況にならない限り）
    'VolatileReadやVolatileWriteに変更してはならない。
    Public Property LastPulseTick() As Long
        Get
            Return Interlocked.Read(_LastPulseTick)
        End Get

        Protected Set(ByVal tick As Long)
            Interlocked.Exchange(_LastPulseTick, tick)
        End Set
    End Property

    Private Property IsQuitRequest() As Boolean
        Get
            Return CBool(Thread.VolatileRead(_IsQuitRequest))
        End Get

        Set(ByVal val As Boolean)
            Thread.VolatileWrite(_IsQuitRequest, CInt(val))
        End Set
    End Property
#End Region

#Region "親スレッド用メソッド"
    Public Sub Start()
        LastPulseTick = TickTimer.GetSystemTick()
        oThread.Start()
    End Sub

    Public Sub Quit()
        IsQuitRequest = True
        oThread.Interrupt()
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
    Private Sub Task()
        Dim spanMax As New TimeSpan(0, 0, 0, 0, Config.RecordingIntervalTicks)
        Dim sLastOutputDir As String = ""
        Try
            Log.Info("The recorder thread started.")

            'アクセスする予定のディレクトリについて、無ければ作成しておく。
            'NOTE: 必ずサブディレクトリの作成から行うことになるものについては、対象外とする。
            Directory.CreateDirectory(sInputDirPath)
            Directory.CreateDirectory(sOutputDirPath)

            Dim nextRecordingTime As DateTime = DateTime.Now.AddMilliseconds(Config.RecordingIntervalTicks)
            While Not IsQuitRequest
                LastPulseTick = TickTimer.GetSystemTick()

                Dim span As TimeSpan = nextRecordingTime - DateTime.Now
                If span < TimeSpan.Zero Then
                    span = TimeSpan.Zero
                ElseIf span > spanMax Then
                    span = spanMax
                End If

                '周期の経過またはInterruptを待つ。
                Try
                    Thread.Sleep(span)
                Catch ex As ThreadInterruptedException
                    'ループ先頭に戻って、ループから抜ける。
                    Continue While
                End Try
                nextRecordingTime = DateTime.Now.AddMilliseconds(Config.RecordingIntervalTicks)

                '今回登録するファイルの一覧を作成する。
                Dim sFiles As String() = Directory.GetFiles(sInputDirPath)
                Dim validCount As Integer = 0
                For i As Integer = 0 To sFiles.Length - 1
                    If UpboundDataPath2.IsMatch(sFiles(i)) Then
                        validCount += 1
                        If Config.RecordingFileCountAtOnce > 0 AndAlso _
                           validCount >= Config.RecordingFileCountAtOnce Then Exit For
                    Else
                        sFiles(i) = Nothing
                    End If
                Next i
                If validCount = 0 Then Continue While

                'データベースへの登録を行う。
                Dim completed As Boolean = False
                Dim procCount As Integer = 0
                Dim dbCtl As New DatabaseTalker()
                Try
                    dbCtl.ConnectOpen()
                    dbCtl.TransactionBegin()

                    dbCtl.ExecuteSQLToWrite("EXEC uspPrepareToImportRiyoData" & sTargetSta)
                    For Each sFilePath As String In sFiles
                        If sFilePath Is Nothing Then Continue For
                        LastPulseTick = TickTimer.GetSystemTick()

                        Log.Info("ファイル[" & Path.GetFileName(sFilePath) & "]の登録を行います...")
                        dbCtl.ExecuteSQLToWrite("EXEC uspImportRiyoData" & sTargetSta & " '" & UpboundDataPath2.GetFormatCode(sFilePath) & "','" & sFilePath & "'")

                        procCount += 1
                        If procCount = validCount Then Exit For
                        If IsQuitRequest Then Exit For
                    Next sFilePath
                    dbCtl.ExecuteSQLToWrite("EXEC uspDispatchRiyoData" & sTargetSta)

                    dbCtl.TransactionCommit()
                    completed = True
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    dbCtl.TransactionRollBack()

                    'TODO: このプロセスは、このケースでも落ちないようにしている
                    'が、それゆえに、「プロセス異常終了のSNMP TRAP」が発生しない
                    'ため、問題が発生している（自動で解消する見込みがない）ことが
                    '外部に伝わらないという事態になりかねない。
                    'たとえば、SQL Server自体は動作しているが、ディスクフルなどで、
                    'Insertが失敗する場合などは、そのような事態になるかもしれない。
                    'よって、ここでSNMP TRAPを発生させるべきかもしれない。
                    '⇒そもそも、「プロセス異常終了のSNMP TRAP」は、プロセスマネージャ
                    'が落とされたときのために追加するものであり、その子プロセスが
                    '落ちた場合は、対応の対象外であるとのこと...。従来どおり、
                    'プロセスマネージャが子プロセスを必ず再起動させれば、それで
                    'よい（再起動が繰り返されるだけで、結局何も行えない...という
                    '状況は想定しない）。

                    'NOTE: リトライは次の周期まで待つ。
                Finally
                    dbCtl.ConnectClose()
                End Try

                If completed Then
                    Dim sDateDirPath As String
                    If needsDateDir Then
                        sDateDirPath = Path.Combine(sOutputDirPath, EkServiceDate.GenString(DateTime.Now))
                        If sLastOutputDir <> sDateDirPath Then
                            Directory.CreateDirectory(sDateDirPath)
                            sLastOutputDir = sDateDirPath
                        Else
                            sDateDirPath = sLastOutputDir
                        End If
                    Else
                        sDateDirPath = sOutputDirPath
                    End If

                    'NOTE: DB登録の途中でQuitされたとしても、ここにおいて、procCountは必ず1以上である。
                    For Each sFilePath As String In sFiles
                        If sFilePath Is Nothing Then Continue For
                        LastPulseTick = TickTimer.GetSystemTick()

                        'NOTE: 完全同一時刻に同一機器から連続的に（所定数超の）利用データがULLされる場合は、
                        '通信プロセスがBUSYのNAKを返信することで、送信機器側で少数の（大きな）ファイルに
                        '統合させるようになっている。このプロセスがInputDirPathから利用データを移動した
                        '直後、完全同一時刻に同一機器から利用データを受信すれば、移動済みのものが所定数に
                        '達している場合は、それらと同一時刻のファイル名を付与することになるが、その数も
                        '知れたものとなるはずである。また、利用データについては、運用上あり得ない頻度で
                        'ULLされたからといって、登録直後に捨てるというのは、さすがに怖い。よって、ここでは、
                        '移動先の名前における枝番が制限を超える場合も、捨てるといったことはしない。

                        'ファイルを新パスに移動する。
                        'NOTE: ファイルは書込可能という前提である。
                        Dim sDestPath As String = UpboundDataPath2.Gen(sDateDirPath, Path.GetFileName(sFilePath))
                        File.Move(sFilePath, sDestPath)
                        Log.Info("ファイルを[" & sDestPath & "]に移動しました。")

                        procCount -= 1
                        If procCount = 0 Then Exit For
                    Next sFilePath
                End If

            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP発生（または収集データ誤記テーブルへの登録）は、
            'プロセスマネージャが行うので、ここでは不要である。
        End Try
    End Sub


    Private Sub CreateTables()
        Dim sPath As String = Path.Combine(Config.RiyoDataImporterFilesBasePath, "RiyoDataTableCreator.sql")
        Dim sSQL As String
        Using oReader As StreamReader = New StreamReader(sPath, Encoding.GetEncoding(932))
            sSQL = oReader.ReadToEnd() _
                   .Replace("${Sta}", sTargetSta) _
                   .Replace("${RiyoDataDatabaseName}", Config.RiyoDataDatabaseName) _
                   .Replace("${ShiteiDataDatabaseName}", Config.ShiteiDataDatabaseName)
        End Using

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            For Each sBatch As String In sSQL.Split(New String() {"${GO}"}, StringSplitOptions.RemoveEmptyEntries)
                dbCtl.ExecuteSQLToWrite(sBatch)
            Next sBatch
            dbCtl.TransactionCommit()
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Sub CreateProcs()
        Dim sPath As String = Path.Combine(Config.RiyoDataImporterFilesBasePath, "RiyoDataProcCreator.sql")
        Dim sSQL As String
        Using oReader As StreamReader = New StreamReader(sPath, Encoding.GetEncoding(932))
            sSQL = oReader.ReadToEnd() _
                   .Replace("${Sta}", sTargetSta) _
                   .Replace("${BasePath}", Config.RiyoDataImporterFilesBasePath) _
                   .Replace("${RiyoDataDatabaseName}", Config.RiyoDataDatabaseName) _
                   .Replace("${ShiteiDataDatabaseName}", Config.ShiteiDataDatabaseName)
        End Using

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            For Each sBatch As String In sSQL.Split(New String() {"${GO}"}, StringSplitOptions.RemoveEmptyEntries)
                dbCtl.ExecuteSQLToWrite(sBatch)
            Next sBatch
            dbCtl.TransactionCommit()
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub
#End Region

End Class
