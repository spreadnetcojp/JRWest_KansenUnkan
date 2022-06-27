' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2015/01/13  (NES)小林  窓処業務前認証ログ収集対応
'   0.2      2015/04/24  (NES)金沢  監視盤情報設定DB削除の機種修正（W→G）
'   0.3      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' 洗い替えプロセスのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "定数や変数"
    'メインウィンドウ
    Private Shared oMainForm As ServerAppForm

    '実作業スレッドへの終了要求フラグ
    Private Shared quitWorker As Integer

    '実作業スレッドで実行する処理
    Private Shared job As System.Delegate

    'ログ格納用ディレクトリ名
    Private Shared sLogBasePath As String

    '削除対象とするログの出力元プロセス
    '-------Ver0.3 次世代車補対応 MOD START-----------
    Private Shared ReadOnly aAppNames As String() = {
       "Manager", _
       "Scheduler", _
       "ConStatusMailer", _
       "AlertMailer", _
       "ToOpClient", _
       "ToKanshiban", _
       "ToTokatsu", _
       "ToMadosho", _
       "ToKanshiban2", _
       "ToMadosho2", _
       "ToNkan", _
       "ForConStatus", _
       "ForKsbConfig", _
       "ForBesshuData", _
       "ForMeisaiData", _
       "ForFaultData", _
       "ForKadoData", _
       "ForTrafficData", _
       "ForRiyoData", _
       "Sweeper"}
    '-------Ver0.3 次世代車補対応 MOD END-------------

    '上りデータ格納用日付別ディレクトリ名のフォーマット
    Private Shared ReadOnly sUpboundDataDirRegx As New Regex("^[0-9]{8}$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    'ログ削除用の定義
    Private Const sAnyLogFileNamePattern As String = "????????-*.csv"
    Private Const sLogFileNameRegxBaseFormat As String = "^[0-9]{{8}}-({0})-[0-9A-Z_\.\-]+\.csv$"
    Private Const logFileNameRegxOptions As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant
#End Region

    ''' <summary>
    ''' 洗い替えプロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 洗い替えプロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppSweeper")
        If m.WaitOne(0, False) Then
            Try
                sLogBasePath = Constant.GetEnv(REG_LOG)
                If sLogBasePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
                    Return
                End If

                Dim sIniFilePath As String = Constant.GetEnv(REG_SERVER_INI)
                If sIniFilePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_SERVER_INI)
                    Return
                End If

                Log.Init(sLogBasePath, "Sweeper")
                Log.Info("プロセス開始")

                Try
                    Lexis.Init(sIniFilePath)
                    Config.Init(sIniFilePath)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End Try

                Log.SetKindsMask(Config.LogKindsMask)

                job = Action.Combine(job, New Action(AddressOf SweepMachineInfo))
                job = Action.Combine(job, New Action(AddressOf SweepMasters))
                job = Action.Combine(job, New Action(AddressOf SweepPrograms))
                job = Action.Combine(job, New Action(AddressOf SweepDeadPatternRelatedData))
                job = Action.Combine(job, New Action(AddressOf SweepDeadAreaRelatedData))

                job = Action.Combine(job, New Action(AddressOf SweepMasterDllVerFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepProgramDllVerFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepMasterVerInfoFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepProgramVerInfoFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepDirectConStatusFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepConStatusFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepKsbConfigFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepBesshuDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepFuseiJoshaDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepKyokoToppaDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepFunshitsuDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepFaultDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepKadoDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepHosyuDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepTrafficDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepCollectedDataTypoFromDatabase))
                '-------Ver0.3 次世代車補対応 ADD START-----------
                job = Action.Combine(job, New Action(AddressOf SweepRiyoDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepShiteiDataFromDatabase))
                '-------Ver0.3 次世代車補対応 ADD END-------------

                job = Action.Combine(job, New Action(AddressOf SweepConStatusFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepKsbConfigFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepBesshuDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepMeisaiDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepFaultDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepKadoDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepTrafficDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepRiyoDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepMadoLogsFromFilesystem))
                '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
                job = Action.Combine(job, New Action(AddressOf SweepMadoCertLogsFromFilesystem))
                '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------
                job = Action.Combine(job, New Action(AddressOf SweepLogsFromFilesystem))

                oMainForm = New ServerAppForm()

                '実作業スレッドを開始する。
                Dim oWorkerThread As New Thread(AddressOf MainClass.WorkingLoop)
                Log.Info("Starting the worker thread...")
                quitWorker = 0
                oWorkerThread.Name = "Worker"
                oWorkerThread.Start()

                'ウインドウプロシージャを実行する。
                'NOTE: このメソッドから例外がスローされることはない。
                ServerAppBaseMain(oMainForm)

                Try
                    '実作業スレッドに終了を要求する。
                    Log.Info("Sending quit request to the worker thread...")
                    Thread.VolatileWrite(quitWorker, 1)

                    '実作業スレッドの終了を待つ。
                    Log.Info("Waiting for the worker thread to quit...")
                    oWorkerThread.Join()
                    Log.Info("The worker thread has quit.")
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oWorkerThread.Abort()
                End Try
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                If oMainForm IsNot Nothing Then
                    oMainForm.Dispose()
                End If
                Config.Dispose()
                Log.Info("プロセス終了")

                'NOTE: ここを通らなくても、このスレッドの消滅とともに解放される
                'ようなので、最悪の心配はない。
                m.ReleaseMutex()

                Application.Exit()
            End Try
        End If
    End Sub

    ''' <summary>
    ''' 実作業スレッドのメイン処理。
    ''' </summary>
    Private Shared Sub WorkingLoop()
        Log.Info("The worker thread started.")

        Dim jobs As System.Delegate() = job.GetInvocationList()
        For i As Integer = 0 To jobs.Length - 1
            If Thread.VolatileRead(quitWorker) = 1 Then
                Log.Warn("Quit requested by manager.")
                Exit For
            End If

            Try
                DirectCast(jobs(i), Action).Invoke()
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                'TODO: TRAP発生（または収集データ誤記テーブルへの登録）
            End Try
        Next

        Log.Info("All jobs ended.")
        oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
    End Sub

    Private Shared Sub SweepMachineInfo()
        Log.Info("Called.")

        '一番古い配信開始日の直前の世代までを残して、
        '機器構成マスタのレコードを削除する。

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            Dim sSQL As String = _
               "DELETE FROM M_MACHINE" _
               & " WHERE" _
                 & " SETTING_START_DATE <" _
                    & " (SELECT MAX(SETTING_START_DATE)" _
                        & " FROM M_MACHINE" _
                        & " WHERE SETTING_START_DATE <= (SELECT SUBSTRING(MIN(DELIVERY_START_TIME), 1, 8) FROM S_MST_DLL_STS))" _
               & " AND" _
                 & " SETTING_START_DATE <" _
                    & " (SELECT MAX(SETTING_START_DATE)" _
                        & " FROM M_MACHINE" _
                        & " WHERE SETTING_START_DATE <= (SELECT SUBSTRING(MIN(DELIVERY_START_TIME), 1, 8) FROM S_PRG_DLL_STS))"
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepMasters()
        Log.Info("Called.")

        Dim targetDataFileNames As DataTable
        Dim targetListFileNames As DataTable

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            Dim sSQLToSelectData As String = _
               "SELECT MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, FILE_NAME" _
               & " FROM" _
               & " (SELECT MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, FILE_NAME, " _
                        & "RANK() OVER (PARTITION BY MODEL_CODE, DATA_KIND, DATA_SUB_KIND ORDER BY UPDATE_DATE DESC) AS RANKING" _
                   & " FROM S_MST_DATA_HEADLINE) AS DA" _
               & " WHERE DA.RANKING > " & Config.MasterDataKeepingGenerations.ToString()
            targetDataFileNames = dbCtl.ExecuteSQLToRead(sSQLToSelectData)

            Dim sSQLToSelectList As String = _
               "SELECT FILE_NAME" _
               & " FROM S_MST_LIST_HEADLINE" _
               & " WHERE MODEL_CODE + DATA_KIND + DATA_SUB_KIND + DATA_VERSION NOT IN" _
               & " (SELECT MODEL_CODE + DATA_KIND + DATA_SUB_KIND + DATA_VERSION" _
                   & " FROM" _
                   & " (SELECT MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION," _
                           & " RANK() OVER (PARTITION BY MODEL_CODE, DATA_KIND, DATA_SUB_KIND ORDER BY UPDATE_DATE DESC) AS RANKING" _
                       & " FROM" _
                       & " (SELECT A.MODEL_CODE, A.DATA_KIND, A.DATA_SUB_KIND, A.DATA_VERSION, A.UPDATE_DATE" _
                           & " FROM S_MST_LIST_HEADLINE AS A," _
                                & " (SELECT MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, MAX(UPDATE_DATE) AS MAX_UPD" _
                                    & " FROM S_MST_LIST_HEADLINE" _
                                    & " GROUP BY MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION" _
                                 & ") AS B" _
                           & " WHERE A.MODEL_CODE = B.MODEL_CODE" _
                           & " AND A.DATA_KIND = B.DATA_KIND" _
                           & " AND A.DATA_SUB_KIND = B.DATA_SUB_KIND" _
                           & " AND A.DATA_VERSION = B.DATA_VERSION" _
                           & " AND A.UPDATE_DATE = B.MAX_UPD" _
                        & ") AS D" _
                    & ") AS DA" _
                   & " WHERE DA.RANKING <= " & Config.MasterDataKeepingGenerations.ToString() & ")"
            targetListFileNames = dbCtl.ExecuteSQLToRead(sSQLToSelectList)

            dbCtl.TransactionBegin()

            For Each row As DataRow In targetDataFileNames.Rows
                Dim sAppModelCode As String = row.Field(Of String)("MODEL_CODE")
                Dim sDataKind As String = row.Field(Of String)("DATA_KIND")
                Dim sDataSubKind As String = row.Field(Of String)("DATA_SUB_KIND")
                Dim sDataVersion As String = row.Field(Of String)("DATA_VERSION")
                Dim sFileName As String = row.Field(Of String)("FILE_NAME")
                Dim sAgentModelCode As String = sAppModelCode
                If sAppModelCode.Equals(EkConstants.ModelCodeGate) Then
                    sAgentModelCode = EkConstants.ModelCodeKanshiban
                ElseIf sAppModelCode.Equals(EkConstants.ModelCodeMadosho) Then
                    sAgentModelCode = EkConstants.ModelCodeTokatsu
                End If

                'NOTE: １つのAgentにつき、配信依頼するマスタの適用対象機種が
                '１つだけであることに依存している。
                '本来であれば、DLL状態テーブルには、送信先機種だけでなく、
                'マスタの適用対象機種も格納できる方がよい。
                Dim sSQLToDeleteDataDllSts As String = _
                   "DELETE FROM S_MST_DLL_STS" _
                   & " WHERE MODEL_CODE = '" & sAgentModelCode & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                   & " AND DATA_KIND = '" & sDataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & sDataSubKind & "'" _
                   & " AND DATA_VERSION = '" & sDataVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteDataDllSts)

                Dim sSQLToDeleteDataDlSts As String = _
                   "DELETE FROM S_MST_DL_STS" _
                   & " WHERE MODEL_CODE = '" & sAppModelCode & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                   & " AND DATA_KIND = '" & sDataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & sDataSubKind & "'" _
                   & " AND VERSION = '" & sDataVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteDataDlSts)

                Dim sSQLToDeleteDataHeadline As String = _
                   "DELETE FROM S_MST_DATA_HEADLINE" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteDataHeadline)

                'Dim sSQLToDeleteData As String = _
                '   "DELETE FROM S_MST_DATA" _
                '   & " WHERE FILE_NAME = '" & sFileName & "'"
                'dbCtl.ExecuteSQLToWrite(sSQLToDeleteData)
            Next row

            For Each row As DataRow In targetListFileNames.Rows
                Dim sFileName As String = row.Field(Of String)("FILE_NAME")

                Dim sSQLToDeleteListHeadline As String = _
                   "DELETE FROM S_MST_LIST_HEADLINE" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteListHeadline)

                Dim sSQLToDeleteList As String = _
                   "DELETE FROM S_MST_LIST" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteList)

                'NOTE: S_MST_DLL_STSやS_MST_DL_STSからの適用リスト配信状態の削除は行わない。
                'これらのレコードは当該リストバージョン・当該パターンNoの
                '適用リストで新たな配信指示が行われた際に、まとめて削除する。
                '新たに登録される見込みがないパターンNoのレコード（予想外の
                'DL完了通知を受けた際に登録したレコードおよび、今は廃止された
                'パターンNoで配信を行った際の古いレコード等）の削除についても、
                '世代管理とは別件であるため、ここではなく、SweepDeadPatternRelatedDataで行う。
            Next row

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return
        Finally
            dbCtl.ConnectClose()
        End Try

        For Each row As DataRow In targetDataFileNames.Rows
            Dim sFileName As String = row.Field(Of String)("FILE_NAME")
            Try
                File.Delete(Path.Combine(Config.MasProDirPath, sFileName))
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        Next row

        For Each row As DataRow In targetListFileNames.Rows
            Dim sFileName As String = row.Field(Of String)("FILE_NAME")
            Try
                File.Delete(Path.Combine(Config.MasProDirPath, sFileName))
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        Next row
    End Sub

    Private Shared Sub SweepPrograms()
        Log.Info("Called.")

        Dim targetDataFileNames As DataTable
        Dim targetListFileNames As DataTable

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            Dim sSQLToSelectData As String = _
               "SELECT MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, FILE_NAME" _
               & " FROM" _
               & " (SELECT MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, FILE_NAME, " _
                        & "RANK() OVER (PARTITION BY MODEL_CODE, DATA_KIND, DATA_SUB_KIND ORDER BY UPDATE_DATE DESC) AS RANKING" _
                   & " FROM S_PRG_DATA_HEADLINE) AS DA" _
               & " WHERE DA.RANKING > " & Config.ProgramDataKeepingGenerations.ToString()
            targetDataFileNames = dbCtl.ExecuteSQLToRead(sSQLToSelectData)

            Dim sSQLToSelectList As String = _
               "SELECT FILE_NAME" _
               & " FROM S_PRG_LIST_HEADLINE" _
               & " WHERE MODEL_CODE + DATA_KIND + DATA_SUB_KIND + DATA_VERSION NOT IN" _
               & " (SELECT MODEL_CODE + DATA_KIND + DATA_SUB_KIND + DATA_VERSION" _
                   & " FROM" _
                   & " (SELECT MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION," _
                           & " RANK() OVER (PARTITION BY MODEL_CODE, DATA_KIND, DATA_SUB_KIND ORDER BY UPDATE_DATE DESC) AS RANKING" _
                       & " FROM" _
                       & " (SELECT A.MODEL_CODE, A.DATA_KIND, A.DATA_SUB_KIND, A.DATA_VERSION, A.UPDATE_DATE" _
                           & " FROM S_PRG_LIST_HEADLINE AS A," _
                                & " (SELECT MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, MAX(UPDATE_DATE) AS MAX_UPD" _
                                    & " FROM S_PRG_LIST_HEADLINE" _
                                    & " GROUP BY MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION" _
                                 & ") AS B" _
                           & " WHERE A.MODEL_CODE = B.MODEL_CODE" _
                           & " AND A.DATA_KIND = B.DATA_KIND" _
                           & " AND A.DATA_SUB_KIND = B.DATA_SUB_KIND" _
                           & " AND A.DATA_VERSION = B.DATA_VERSION" _
                           & " AND A.UPDATE_DATE = B.MAX_UPD" _
                        & ") AS D" _
                    & ") AS DA" _
                   & " WHERE DA.RANKING <= " & Config.ProgramDataKeepingGenerations.ToString() & ")"
            targetListFileNames = dbCtl.ExecuteSQLToRead(sSQLToSelectList)

            dbCtl.TransactionBegin()

            For Each row As DataRow In targetDataFileNames.Rows
                Dim sAppModelCode As String = row.Field(Of String)("MODEL_CODE")
                Dim sDataKind As String = row.Field(Of String)("DATA_KIND")
                Dim sDataSubKind As String = row.Field(Of String)("DATA_SUB_KIND")
                Dim sDataVersion As String = row.Field(Of String)("DATA_VERSION")
                Dim sFileName As String = row.Field(Of String)("FILE_NAME")
                Dim sAgentModelCode As String = sAppModelCode
                If sAppModelCode.Equals(EkConstants.ModelCodeGate) Then
                    sAgentModelCode = EkConstants.ModelCodeKanshiban
                ElseIf sAppModelCode.Equals(EkConstants.ModelCodeMadosho) Then
                    sAgentModelCode = EkConstants.ModelCodeTokatsu
                End If

                'NOTE: プログラム種別が機種間で重複していないことに依存している。
                '本来であれば、DLL状態テーブルには、送信先機種だけでなく、
                'プログラムの適用対象機種も格納できる方がよい。
                Dim sSQLToDeleteDataDllSts As String = _
                   "DELETE FROM S_PRG_DLL_STS" _
                   & " WHERE MODEL_CODE = '" & sAgentModelCode & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                   & " AND DATA_KIND = '" & sDataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & sDataSubKind & "'" _
                   & " AND DATA_VERSION = '" & sDataVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteDataDllSts)

                Dim sSQLToDeleteDataDlSts As String = _
                   "DELETE FROM S_PRG_DL_STS" _
                   & " WHERE MODEL_CODE = '" & sAppModelCode & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                   & " AND DATA_KIND = '" & sDataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & sDataSubKind & "'" _
                   & " AND VERSION = '" & sDataVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteDataDlSts)

                Dim sSQLToDeleteDataHeadline As String = _
                   "DELETE FROM S_PRG_DATA_HEADLINE" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteDataHeadline)

                Dim sSQLToDeleteData As String = _
                   "DELETE FROM S_PRG_DATA" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteData)
            Next row

            For Each row As DataRow In targetListFileNames.Rows
                Dim sFileName As String = row.Field(Of String)("FILE_NAME")

                Dim sSQLToDeleteListHeadline As String = _
                   "DELETE FROM S_PRG_LIST_HEADLINE" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteListHeadline)

                Dim sSQLToDeleteList As String = _
                   "DELETE FROM S_PRG_LIST" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteList)

                'NOTE: S_PRG_DLL_STSやS_PRG_DL_STSからの適用リスト配信状態の削除は行わない。
                'これらのレコードは当該リストバージョン・当該エリアNoの
                '適用リストで新たな配信指示が行われた際に、まとめて削除する。
                '新たに登録される見込みがないエリアNoのレコード（予想外の
                'DL完了通知を受けた際に登録したレコードおよび、今は廃止された
                'エリアNoで配信を行った際の古いレコード等）の削除についても、
                '世代管理とは別件であるため、ここではなく、SweepDeadAreaRelatedDataで行う。
            Next row

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return
        Finally
            dbCtl.ConnectClose()
        End Try

        For Each row As DataRow In targetDataFileNames.Rows
            Dim sFileName As String = row.Field(Of String)("FILE_NAME")
            Try
                File.Delete(Path.Combine(Config.MasProDirPath, sFileName))
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        Next row

        For Each row As DataRow In targetListFileNames.Rows
            Dim sFileName As String = row.Field(Of String)("FILE_NAME")
            Try
                File.Delete(Path.Combine(Config.MasProDirPath, sFileName))
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        Next row
    End Sub

    Private Shared Sub SweepDeadPatternRelatedData()
        Log.Info("Called.")

        '存在しないパターン定義に依存するレコードを
        '各種テーブルから削除する。

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            'NOTE: １つのAgentにつき、配信依頼するマスタの適用対象機種が
            '１つだけであることに依存している。
            '本来であれば、DLL状態テーブルには、送信先機種だけでなく、
            'マスタの適用対象機種も格納できる方がよい。
            Dim sSQL1 As String = _
               "DELETE FROM S_MST_DLL_STS" _
               & " WHERE FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
               & " AND NOT EXISTS (" _
                  & "SELECT 1 FROM M_PATTERN_DATA" _
                   & " WHERE REPLACE(" _
                      & "REPLACE(MODEL_CODE, '" & EkConstants.ModelCodeGate & "', '" & EkConstants.ModelCodeKanshiban & "'), " _
                      & "'" & EkConstants.ModelCodeMadosho & "', '" & EkConstants.ModelCodeTokatsu & "') = S_MST_DLL_STS.MODEL_CODE" _
                   & " AND MST_KIND = S_MST_DLL_STS.DATA_KIND" _
                   & " AND PATTERN_NO = S_MST_DLL_STS.DATA_SUB_KIND)"
            dbCtl.ExecuteSQLToWrite(sSQL1)

            Dim sSQL2 As String = _
               "DELETE FROM S_MST_DL_STS" _
               & " WHERE FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
               & " AND NOT EXISTS (" _
                  & "SELECT 1 FROM M_PATTERN_DATA" _
                   & " WHERE MODEL_CODE = S_MST_DL_STS.MODEL_CODE" _
                   & " AND MST_KIND = S_MST_DL_STS.DATA_KIND" _
                   & " AND PATTERN_NO = S_MST_DL_STS.DATA_SUB_KIND)"
            dbCtl.ExecuteSQLToWrite(sSQL2)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepDeadAreaRelatedData()
        Log.Info("Called.")

        '存在しないエリア定義に依存するレコードを
        '各種テーブルから削除する。

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            'NOTE: エリアの機種からプログラム種別を導く際、固定値「PG」を使って
            '導いていることに注意。また、この判定法自体、プログラム種別が機種
            'ごとに別の名前で最大でも１つだけ用意されていることに依存している。
            '本来であれば、DLL状態テーブルには、送信先機種だけでなく、
            'プログラムの適用対象機種も格納できる方がよい。
            Dim sSQL1 As String = _
               "DELETE FROM S_PRG_DLL_STS" _
               & " WHERE FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
               & " AND NOT EXISTS (" _
                  & "SELECT 1 FROM M_AREA_DATA" _
                   & " WHERE MODEL_CODE + 'PG' = S_PRG_DLL_STS.DATA_KIND" _
                   & " AND AREA_NO = S_PRG_DLL_STS.DATA_SUB_KIND)"
            dbCtl.ExecuteSQLToWrite(sSQL1)

            Dim sSQL2 As String = _
               "DELETE FROM S_PRG_DL_STS" _
               & " WHERE FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
               & " AND NOT EXISTS (" _
                  & "SELECT 1 FROM M_AREA_DATA" _
                   & " WHERE MODEL_CODE = S_PRG_DL_STS.MODEL_CODE" _
                   & " AND AREA_NO = S_PRG_DL_STS.DATA_SUB_KIND)"
            dbCtl.ExecuteSQLToWrite(sSQL2)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepMasterDllVerFromDatabase()
        Log.Info("Called.")

        '最新の機器構成に存在しない号機のレコードを
        'マスタDLLバージョンテーブルから削除する。

        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            Dim sSQL As String = _
               "DELETE FROM S_MST_DLL_VER" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = S_MST_DLL_VER.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = S_MST_DLL_VER.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = S_MST_DLL_VER.CORNER_CODE" _
                   & " AND UNIT_NO = S_MST_DLL_VER.UNIT_NO" _
                   & " AND MODEL_CODE = S_MST_DLL_VER.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepProgramDllVerFromDatabase()
        Log.Info("Called.")

        '最新の機器構成に存在しない号機のレコードを
        'プログラムDLLバージョンテーブルから削除する。

        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            Dim sSQL As String = _
               "DELETE FROM S_PRG_DLL_VER" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = S_PRG_DLL_VER.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = S_PRG_DLL_VER.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = S_PRG_DLL_VER.CORNER_CODE" _
                   & " AND UNIT_NO = S_PRG_DLL_VER.UNIT_NO" _
                   & " AND MODEL_CODE = S_PRG_DLL_VER.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepMasterVerInfoFromDatabase()
        Log.Info("Called.")

        '最新の機器構成に存在しない号機のレコードを
        'マスタバージョン情報テーブルから削除する。

        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            Dim sSQL1 As String = _
               "DELETE FROM S_MST_VER_INFO_EXPECTED" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = S_MST_VER_INFO_EXPECTED.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = S_MST_VER_INFO_EXPECTED.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = S_MST_VER_INFO_EXPECTED.CORNER_CODE" _
                   & " AND UNIT_NO = S_MST_VER_INFO_EXPECTED.UNIT_NO" _
                   & " AND MODEL_CODE = S_MST_VER_INFO_EXPECTED.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL1)

            Dim sSQL2 As String = _
               "DELETE FROM D_MST_VER_INFO" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = D_MST_VER_INFO.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = D_MST_VER_INFO.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = D_MST_VER_INFO.CORNER_CODE" _
                   & " AND UNIT_NO = D_MST_VER_INFO.UNIT_NO" _
                   & " AND MODEL_CODE = D_MST_VER_INFO.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL2)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepProgramVerInfoFromDatabase()
        Log.Info("Called.")

        '最新の機器構成に存在しない号機のレコードを
        'プログラムバージョン情報テーブルから削除する。

        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            Dim sSQL1 As String = _
               "DELETE FROM S_PRG_VER_INFO_EXPECTED" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = S_PRG_VER_INFO_EXPECTED.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = S_PRG_VER_INFO_EXPECTED.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = S_PRG_VER_INFO_EXPECTED.CORNER_CODE" _
                   & " AND UNIT_NO = S_PRG_VER_INFO_EXPECTED.UNIT_NO" _
                   & " AND MODEL_CODE = S_PRG_VER_INFO_EXPECTED.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL1)

            Dim sSQL2 As String = _
               "DELETE FROM D_PRG_VER_INFO_CUR" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = D_PRG_VER_INFO_CUR.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = D_PRG_VER_INFO_CUR.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = D_PRG_VER_INFO_CUR.CORNER_CODE" _
                   & " AND UNIT_NO = D_PRG_VER_INFO_CUR.UNIT_NO" _
                   & " AND MODEL_CODE = D_PRG_VER_INFO_CUR.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL2)

            Dim sSQL3 As String = _
               "DELETE FROM D_PRG_VER_INFO_NEW" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = D_PRG_VER_INFO_NEW.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = D_PRG_VER_INFO_NEW.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = D_PRG_VER_INFO_NEW.CORNER_CODE" _
                   & " AND UNIT_NO = D_PRG_VER_INFO_NEW.UNIT_NO" _
                   & " AND MODEL_CODE = D_PRG_VER_INFO_NEW.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL3)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepDirectConStatusFromDatabase()
        Log.Info("Called.")

        '最新の機器構成に存在しない号機のレコードを
        '運管サーバ機器接続状態テーブルから削除する。

        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            Dim sSQL As String = _
               "DELETE FROM S_DIRECT_CON_STATUS" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = S_DIRECT_CON_STATUS.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = S_DIRECT_CON_STATUS.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = S_DIRECT_CON_STATUS.CORNER_CODE" _
                   & " AND UNIT_NO = S_DIRECT_CON_STATUS.UNIT_NO" _
                   & " AND MODEL_CODE = S_DIRECT_CON_STATUS.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepConStatusFromDatabase()
        Log.Info("Called.")

        '最新の機器構成に存在しない号機のレコードを
        '機器接続状態テーブルから削除する。

        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            Dim sSQL As String = _
               "DELETE FROM D_CON_STATUS" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = D_CON_STATUS.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = D_CON_STATUS.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = D_CON_STATUS.CORNER_CODE" _
                   & " AND UNIT_NO = D_CON_STATUS.UNIT_NO" _
                   & " AND MODEL_CODE = D_CON_STATUS.MODEL_CODE" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepKsbConfigFromDatabase()
        Log.Info("Called.")

        '最新の機器構成に存在しない号機のレコードを
        '監視盤設定情報テーブルから削除する。

        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            '----Ver0.2 監視盤設定情報削除の機種修正　MOD　START------------------------
            Dim sSQL As String = _
               "DELETE FROM D_KSB_CONFIG" _
               & " WHERE NOT EXISTS (" _
                  & "SELECT 1 FROM M_MACHINE" _
                   & " WHERE RAIL_SECTION_CODE = D_KSB_CONFIG.RAIL_SECTION_CODE" _
                   & " AND STATION_ORDER_CODE = D_KSB_CONFIG.STATION_ORDER_CODE" _
                   & " AND CORNER_CODE = D_KSB_CONFIG.CORNER_CODE" _
                   & " AND UNIT_NO = D_KSB_CONFIG.UNIT_NO" _
                   & " AND MODEL_CODE = 'G'" _
                   & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                & " FROM M_MACHINE" _
                                                & " WHERE SETTING_START_DATE <= '" & sServiceDate & "'))"
            '----Ver0.2 監視盤設定情報削除の機種修正　MOD　END--------------------------
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepBesshuDataFromDatabase()
        Log.Info("Called.")

        'Config.BesshuDataVisibleDaysが経過したレコードを
        '別集札データテーブルから削除する。
        SweepOldRecordsFromDatabase("D_BESSHU_DATA", Config.BesshuDataVisibleDays)
    End Sub

    Private Shared Sub SweepFuseiJoshaDataFromDatabase()
        Log.Info("Called.")

        'Config.FuseiJoshaDataVisibleDaysが経過したレコードを
        '不正乗車券検出データテーブルから削除する。
        SweepOldRecordsFromDatabase("D_FUSEI_JOSHA_DATA", Config.FuseiJoshaDataVisibleDays)
    End Sub

    Private Shared Sub SweepKyokoToppaDataFromDatabase()
        Log.Info("Called.")

        'Config.KyokoToppaDataVisibleDaysが経過したレコードを
        '強行突破検出データテーブルから削除する。
        SweepOldRecordsFromDatabase("D_KYOKO_TOPPA_DATA", Config.KyokoToppaDataVisibleDays)
    End Sub

    Private Shared Sub SweepFunshitsuDataFromDatabase()
        Log.Info("Called.")

        'Config.FunshitsuDataVisibleDaysが経過したレコードを
        '紛失券検出データテーブルから削除する。
        SweepOldRecordsFromDatabase("D_FUNSHITSU_DATA", Config.FunshitsuDataVisibleDays)
    End Sub

    Private Shared Sub SweepFaultDataFromDatabase()
        Log.Info("Called.")

        'Config.FaultDataVisibleDaysが経過したレコードを
        '異常データテーブルから削除する。
        SweepOldRecordsFromDatabase("D_FAULT_DATA", Config.FaultDataVisibleDays)
    End Sub

    Private Shared Sub SweepKadoDataFromDatabase()
        Log.Info("Called.")

        'Config.KadoDataVisibleDaysが経過したレコードを
        '稼動データテーブルから削除する。
        SweepOldRecordsFromDatabase("D_KADO_DATA", Config.KadoDataVisibleDays)
    End Sub

    Private Shared Sub SweepHosyuDataFromDatabase()
        Log.Info("Called.")

        'Config.HosyuDataVisibleDaysが経過したレコードを
        '保守データテーブルから削除する。
        SweepOldRecordsFromDatabase("D_HOSYU_DATA", Config.HosyuDataVisibleDays)
    End Sub

    Private Shared Sub SweepTrafficDataFromDatabase()
        Log.Info("Called.")

        'Config.TrafficDataVisibleDaysが経過したレコードを
        '時間帯別乗降データテーブルから削除する。
        SweepOldRecordsFromDatabase("D_TRAFFIC_DATA", Config.TrafficDataVisibleDays)
    End Sub

    Private Shared Sub SweepCollectedDataTypoFromDatabase()
        Log.Info("Called.")

        'Config.CollectedDataTypoVisibleDaysが経過したレコードを
        '収集データ誤記テーブルから削除する。
        SweepOldRecordsFromDatabase("D_COLLECTED_DATA_TYPO", Config.CollectedDataTypoVisibleDays)
    End Sub

    '-------Ver0.3 次世代車補対応 ADD START-----------
    Private Shared Sub SweepRiyoDataFromDatabase()
        Log.Info("Called.")

        'Config.RiyoDataVisibleDaysが経過したレコードを
        '全駅の利用データテーブルから削除する。
        Dim oTable As DataTable = SelectTableNames("D_RIYO_DATA_[A-Z][0-9]_[0-9][0-9][0-9][0-9][0-9][0-9]".Replace("_", "?_"), Config.RiyoDataDatabaseName)
        If oTable IsNot Nothing Then
            For Each oRow As DataRow In oTable.Rows
                SweepOldRecordsFromDatabase(Config.RiyoDataDatabaseName & ".dbo." & oRow.Field(Of String)(0), Config.RiyoDataVisibleDays)
            Next oRow
        End If
    End Sub
    '-------Ver0.3 次世代車補対応 ADD END-------------

    '-------Ver0.3 次世代車補対応 ADD START-----------
    Private Shared Sub SweepShiteiDataFromDatabase()
        Log.Info("Called.")

        'Config.ShiteiDataVisibleDaysが経過したレコードを
        '全駅の新幹線指定券入場データテーブルから削除する。
        Dim oTable As DataTable = SelectTableNames("D_SHITEI_DATA_[0-9][0-9][0-9][0-9][0-9][0-9]".Replace("_", "?_"), Config.ShiteiDataDatabaseName)
        If oTable IsNot Nothing Then
            For Each oRow As DataRow In oTable.Rows
                SweepOldRecordsFromDatabase(Config.ShiteiDataDatabaseName & ".dbo." & oRow.Field(Of String)(0), Config.ShiteiDataVisibleDays)
            Next oRow
        End If
    End Sub
    '-------Ver0.3 次世代車補対応 ADD END-------------

    Private Shared Sub SweepConStatusFromFilesystem()
        Log.Info("Called.")

        'Config.ConStatusKeepingDaysInRejectDirが経過したファイルを
        'Config.RejectDirPathInRecordingBaseディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForConStatus"), Config.ConStatusKeepingDaysInRejectDir)

        'Config.ConStatusKeepingDaysInTrashDirが経過したディレクトリを
        'Config.TrashDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForConStatus"), Config.ConStatusKeepingDaysInTrashDir)

        'Config.ConStatusKeepingDaysInQuarantineDirが経過したディレクトリを
        'Config.QuarantineDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForConStatus"), Config.ConStatusKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBaseディレクトリ内のファイルを
        'Config.InputDirPathInRecordingBaseディレクトリに移動する？
        'TODO: 当該データを受信する通信プロセスが動作していることを考慮すると、
        'UpboundDataPath.Gen()を使うだけでは済まない（排他制御が必要）。
    End Sub

    Private Shared Sub SweepKsbConfigFromFilesystem()
        Log.Info("Called.")

        'Config.KsbConfigKeepingDaysInRejectDirが経過したファイルを
        'Config.RejectDirPathInRecordingBaseディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForKsbConfig"), Config.KsbConfigKeepingDaysInRejectDir)

        'Config.KsbConfigKeepingDaysInTrashDirが経過したディレクトリを
        'Config.TrashDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForKsbConfig"), Config.KsbConfigKeepingDaysInTrashDir)

        'Config.KsbConfigKeepingDaysInQuarantineDirが経過したディレクトリを
        'Config.QuarantineDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForKsbConfig"), Config.KsbConfigKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBaseディレクトリ内のファイルを
        'Config.InputDirPathInRecordingBaseディレクトリに移動する？
        'TODO: 当該データを受信する通信プロセスが動作していることを考慮すると、
        'UpboundDataPath.Gen()を使うだけでは済まない（排他制御が必要）。
    End Sub

    Private Shared Sub SweepBesshuDataFromFilesystem()
        Log.Info("Called.")

        'Config.BesshuDataKeepingDaysInRejectDirが経過したファイルを
        'Config.RejectDirPathInRecordingBaseディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForBesshuData"), Config.BesshuDataKeepingDaysInRejectDir)

        'Config.BesshuDataKeepingDaysInTrashDirが経過したディレクトリを
        'Config.TrashDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForBesshuData"), Config.BesshuDataKeepingDaysInTrashDir)

        'Config.BesshuDataKeepingDaysInQuarantineDirが経過したディレクトリを
        'Config.QuarantineDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForBesshuData"), Config.BesshuDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBaseディレクトリ内のファイルを
        'Config.InputDirPathInRecordingBaseディレクトリに移動する？
        'TODO: 当該データを受信する通信プロセスが動作していることを考慮すると、
        'UpboundDataPath.Gen()を使うだけでは済まない（排他制御が必要）。
    End Sub

    Private Shared Sub SweepMeisaiDataFromFilesystem()
        Log.Info("Called.")

        'Config.MeisaiDataKeepingDaysInRejectDirが経過したファイルを
        'Config.RejectDirPathInRecordingBaseディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForMeisaiData"), Config.MeisaiDataKeepingDaysInRejectDir)

        'Config.MeisaiDataKeepingDaysInTrashDirが経過したディレクトリを
        'Config.TrashDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForMeisaiData"), Config.MeisaiDataKeepingDaysInTrashDir)

        'Config.MeisaiDataKeepingDaysInQuarantineDirが経過したディレクトリを
        'Config.QuarantineDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForMeisaiData"), Config.MeisaiDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBaseディレクトリ内のファイルを
        'Config.InputDirPathInRecordingBaseディレクトリに移動する？
        'TODO: 当該データを受信する通信プロセスが動作していることを考慮すると、
        'UpboundDataPath.Gen()を使うだけでは済まない（排他制御が必要）。
    End Sub

    Private Shared Sub SweepFaultDataFromFilesystem()
        Log.Info("Called.")

        'Config.FaultDataKeepingDaysInRejectDirが経過したファイルを
        'Config.RejectDirPathInRecordingBaseディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForFaultData"), Config.FaultDataKeepingDaysInRejectDir)

        'Config.FaultDataKeepingDaysInTrashDirが経過したディレクトリを
        'Config.TrashDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForFaultData"), Config.FaultDataKeepingDaysInTrashDir)

        'Config.FaultDataKeepingDaysInQuarantineDirが経過したディレクトリを
        'Config.QuarantineDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForFaultData"), Config.FaultDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBaseディレクトリ内のファイルを
        'Config.InputDirPathInRecordingBaseディレクトリに移動する？
        'TODO: 当該データを受信する通信プロセスが動作していることを考慮すると、
        'UpboundDataPath.Gen()を使うだけでは済まない（排他制御が必要）。
    End Sub

    Private Shared Sub SweepKadoDataFromFilesystem()
        Log.Info("Called.")

        'Config.KadoDataKeepingDaysInRejectDirが経過したファイルを
        'Config.RejectDirPathInRecordingBaseディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForKadoData"), Config.KadoDataKeepingDaysInRejectDir)

        'Config.KadoDataKeepingDaysInTrashDirが経過したディレクトリを
        'Config.TrashDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForKadoData"), Config.KadoDataKeepingDaysInTrashDir)

        'Config.KadoDataKeepingDaysInQuarantineDirが経過したディレクトリを
        'Config.QuarantineDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForKadoData"), Config.KadoDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBaseディレクトリ内のファイルを
        'Config.InputDirPathInRecordingBaseディレクトリに移動する？
        'TODO: 当該データを受信する通信プロセスが動作していることを考慮すると、
        'UpboundDataPath.Gen()を使うだけでは済まない（排他制御が必要）。
    End Sub

    Private Shared Sub SweepTrafficDataFromFilesystem()
        Log.Info("Called.")

        'Config.TrafficDataKeepingDaysInRejectDirが経過したファイルを
        'Config.RejectDirPathInRecordingBaseディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForTrafficData"), Config.TrafficDataKeepingDaysInRejectDir)

        'Config.TrafficDataKeepingDaysInTrashDirが経過したディレクトリを
        'Config.TrashDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForTrafficData"), Config.TrafficDataKeepingDaysInTrashDir)

        'Config.TrafficDataKeepingDaysInQuarantineDirが経過したディレクトリを
        'Config.QuarantineDirPathInRecordingBaseディレクトリから削除する。
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForTrafficData"), Config.TrafficDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBaseディレクトリ内のファイルを
        'Config.InputDirPathInRecordingBaseディレクトリに移動する？
        'TODO: 当該データを受信する通信プロセスが動作していることを考慮すると、
        'UpboundDataPath.Gen()を使うだけでは済まない（排他制御が必要）。
    End Sub

    Private Shared Sub SweepRiyoDataFromFilesystem()
        Log.Info("Called.")
        'Config.RiyoDataKeepingDaysInRejectDirが経過したファイルを
        'Config.RiyoDataRejectDirPathInStationBaseディレクトリから削除する。
        If Directory.Exists(Config.RiyoDataDirPath) Then
            Dim aStationDirs As String() = Directory.GetDirectories(Config.RiyoDataDirPath)
            For Each sStationDir As String In aStationDirs
                Dim sPath As String = Utility.CombinePathWithVirtualPath(sStationDir, Config.RiyoDataRejectDirPathInStationBase)
                '-------Ver0.3 次世代車補対応 MOD START-----------
                SweepOldFilesFromFilesystem2(sPath, Config.RiyoDataKeepingDaysInRejectDir)
                '-------Ver0.3 次世代車補対応 MOD END-------------
            Next sStationDir
        End If

        'NOTE: 常駐プロセスとして「ToNkan」および「ForRiyoData」が設定されているか否かで
        '削除する対象を切り替える。
        If Directory.Exists(Config.RiyoDataDirPath) Then
            Dim aStationDirs As String() = Directory.GetDirectories(Config.RiyoDataDirPath)
            For Each sStationDir As String In aStationDirs
                '-------Ver0.3 次世代車補対応 MOD START-----------
                If Config.ResidentApps.Contains("ToNkan") Then
                    Dim sPath As String = Utility.CombinePathWithVirtualPath(sStationDir, Config.RiyoDataTrashDirPathInStationBase)
                    SweepOldBranchableDirectoriesFromFilesystem(sPath, Config.RiyoDataKeepingDaysInTrashDir)
                ElseIf Config.ResidentApps.Contains("ForRiyoData") Then
                    Dim sPath As String = Utility.CombinePathWithVirtualPath(sStationDir, Config.RiyoDataOutputDirPathInStationBase)
                    SweepOldDirectoriesFromFilesystem(sPath, Config.RiyoDataKeepingDaysInTrashDir)
                Else
                    Dim sPath As String = Utility.CombinePathWithVirtualPath(sStationDir, Config.RiyoDataInputDirPathInStationBase)
                    SweepOldFilesFromFilesystem2(sPath, Config.RiyoDataKeepingDaysInTrashDir)
                End If
                '-------Ver0.3 次世代車補対応 MOD END-------------
            Next sStationDir
        End If

        'TODO: 駅が機器構成から消えた場合は、駅ディレクトリごと消したい。
        'ただし、その日に消すわけにはいかない（保持期間を守らないことになる上、
        '設定次第では集計されないのに、消してしまうことになる）ため、上記処理の
        '後に駅ディレクトリの中が全サブディレクトリの中までに空になっている場合
        'にのみ消すようにしなければならない。
    End Sub

    Private Shared Sub SweepMadoLogsFromFilesystem()
        Log.Info("Called.")

        'Config.MadoLogsKeepingDaysが経過したファイルを
        'Config.MadoLogDirPathディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.MadoLogDirPath, Config.MadoLogsKeepingDays)
    End Sub

    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
    Private Shared Sub SweepMadoCertLogsFromFilesystem()
        Log.Info("Called.")

        'Config.MadoCertLogsKeepingDaysが経過したファイルを
        'Config.MadoCertLogDirPathディレクトリから削除する。
        SweepOldFilesFromFilesystem(Config.MadoCertLogDirPath, Config.MadoCertLogsKeepingDays)
    End Sub
    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------

    Private Shared Sub SweepLogsFromFilesystem()
        Log.Info("Called.")

        Dim oRegxElem As New StringBuilder()
        For Each sAppName As String In aAppNames
            oRegxElem.Append(sAppName & "|")
        Next sAppName
        oRegxElem.Remove(oRegxElem.Length - 1, 1)

        'Config.LogsKeepingDaysが経過したファイルを
        'REG_LOGディレクトリから削除する。
        Dim oLogFileNameRegx As New Regex(String.Format(sLogFileNameRegxBaseFormat, oRegxElem), logFileNameRegxOptions)
        Dim boundDate As Integer = Integer.Parse(DateTime.Now.AddDays(-Config.LogsKeepingDays).ToString("yyyyMMdd"))
        For Each sFile As String In Directory.GetFiles(sLogBasePath, sAnyLogFileNamePattern)
            Try
                Dim sFileName As String = Path.GetFileName(sFile)
                If oLogFileNameRegx.IsMatch(sFileName) AndAlso _
                   Integer.Parse(sFileName.Substring(0, 8)) < boundDate Then
                    File.Delete(sFile)
                    Log.Info("The file [" & sFile & "] deleted.")
                End If
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        Next sFile
    End Sub

    Private Shared Sub SweepOldRecordsFromDatabase(ByVal sTableName As String, ByVal days As Integer)
        Dim sBoundDate As String = EkServiceDate.Gen().AddDays(-days).ToString("yyyy/MM/dd HH:mm:ss.fff")
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            'TODO: 保守で定期的にチェックする際のキーを運管での「変更日時」ではなく
            '駅務機器での「発生日時」（DB仕様書で「収集日時」と呼ばれる項目）とするなら、
            'ここも割り切って「SYUSYU_DATE」をキーにするべきである。
            'そうでない（変更日時をキーにする）なら、ここの効率だけでなく、
            '運管端末から検索する際の効率や、インデックスの劣化しにくさを考えても、
            '「収集日時」を主キーから外して、代わりに「変更日時」を主キーにするべきである。
            Dim sSQL As String = _
               "DELETE FROM " & sTableName _
               & " WHERE UPDATE_DATE < '" & sBoundDate & "'"
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Shared Sub SweepOldFilesFromFilesystem(ByVal sBasePath As String, ByVal days As Integer)
        Try
            Log.Info("Sweeping old files in ["& sBasePath &"]...")
            If Directory.Exists(sBasePath) Then
                Dim boundDate As DateTime = EkServiceDate.Gen().AddDays(-days)
                Dim aFiles As String() = Directory.GetFiles(sBasePath)
                For Each sFile As String In aFiles
                    If UpboundDataPath.IsMatch(sFile) AndAlso _
                       UpboundDataPath.GetTimestamp(sFile) < boundDate Then
                        File.Delete(sFile)
                    End If
                Next sFile
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    '-------Ver0.3 次世代車補対応 ADD START-----------
    Private Shared Sub SweepOldFilesFromFilesystem2(ByVal sBasePath As String, ByVal days As Integer)
        Try
            Log.Info("Sweeping old files in ["& sBasePath &"]...")
            If Directory.Exists(sBasePath) Then
                Dim boundDate As DateTime = EkServiceDate.Gen().AddDays(-days)
                Dim aFiles As String() = Directory.GetFiles(sBasePath)
                For Each sFile As String In aFiles
                    If UpboundDataPath2.IsMatch(sFile) AndAlso _
                       UpboundDataPath2.GetTimestamp(sFile) < boundDate Then
                        File.Delete(sFile)
                    End If
                Next sFile
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub
    '-------Ver0.3 次世代車補対応 ADD END-------------

    Private Shared Sub SweepOldDirectoriesFromFilesystem(ByVal sBasePath As String, ByVal days As Integer)
        Try
            Log.Info("Sweeping old directories in ["& sBasePath &"]...")
            If Directory.Exists(sBasePath) Then
                Dim sBoundDate As String = EkServiceDate.Gen().AddDays(-days).ToString("yyyyMMdd")
                Dim aSubDirs As String() = Directory.GetDirectories(sBasePath)
                For Each sSubDir As String In aSubDirs
                    Dim sFileName as String = Path.GetFileName(sSubDir)
                    If sUpboundDataDirRegx.IsMatch(sFileName) AndAlso _
                       String.CompareOrdinal(sFileName, sBoundDate) < 0 Then
                        Directory.Delete(sSubDir, True)
                    End If
                Next sSubDir
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    Private Shared Sub SweepOldBranchableDirectoriesFromFilesystem(ByVal sBasePath As String, ByVal days As Integer)
        Try
            Log.Info("Sweeping old directories in ["& sBasePath &"]...")
            If Directory.Exists(sBasePath) Then
                Dim boundDate As DateTime = EkServiceDate.Gen().AddDays(-days)
                Dim aSubDirs As String() = Directory.GetDirectories(sBasePath)
                For Each sSubDir As String In aSubDirs
                    If TimestampedDirPath.IsMatch(sSubDir) AndAlso _
                       TimestampedDirPath.GetTimestamp(sSubDir) < boundDate Then
                        Directory.Delete(sSubDir, True)
                    End If
                Next sSubDir
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    '-------Ver0.3 次世代車補対応 ADD START-----------
    Private Shared Function SelectTableNames(ByVal sTableNamePat As String, ByVal sDatabaseName As String) As DataTable
        Dim sSQL As String = "SELECT name FROM " & sDatabaseName & ".dbo.sysobjects WHERE xtype = 'u' AND name LIKE '" & sTableNamePat & "' ESCAPE '?'"
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return Nothing
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function
    '-------Ver0.3 次世代車補対応 ADD END-------------

End Class
