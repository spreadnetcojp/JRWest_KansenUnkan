' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2015/01/13  (NES)����  �����Ɩ��O�F�؃��O���W�Ή�
'   0.2      2015/04/24  (NES)����  �Ď��Տ��ݒ�DB�폜�̋@��C���iW��G�j
'   0.3      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' �􂢑ւ��v���Z�X�̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "�萔��ϐ�"
    '���C���E�B���h�E
    Private Shared oMainForm As ServerAppForm

    '����ƃX���b�h�ւ̏I���v���t���O
    Private Shared quitWorker As Integer

    '����ƃX���b�h�Ŏ��s���鏈��
    Private Shared job As System.Delegate

    '���O�i�[�p�f�B���N�g����
    Private Shared sLogBasePath As String

    '�폜�ΏۂƂ��郍�O�̏o�͌��v���Z�X
    '-------Ver0.3 ������ԕ�Ή� MOD START-----------
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
    '-------Ver0.3 ������ԕ�Ή� MOD END-------------

    '���f�[�^�i�[�p���t�ʃf�B���N�g�����̃t�H�[�}�b�g
    Private Shared ReadOnly sUpboundDataDirRegx As New Regex("^[0-9]{8}$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    '���O�폜�p�̒�`
    Private Const sAnyLogFileNamePattern As String = "????????-*.csv"
    Private Const sLogFileNameRegxBaseFormat As String = "^[0-9]{{8}}-({0})-[0-9A-Z_\.\-]+\.csv$"
    Private Const logFileNameRegxOptions As RegexOptions = RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant
#End Region

    ''' <summary>
    ''' �􂢑ւ��v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �􂢑ւ��v���Z�X�̃G���g���|�C���g�ł���B
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
                Log.Info("�v���Z�X�J�n")

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
                '-------Ver0.3 ������ԕ�Ή� ADD START-----------
                job = Action.Combine(job, New Action(AddressOf SweepRiyoDataFromDatabase))
                job = Action.Combine(job, New Action(AddressOf SweepShiteiDataFromDatabase))
                '-------Ver0.3 ������ԕ�Ή� ADD END-------------

                job = Action.Combine(job, New Action(AddressOf SweepConStatusFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepKsbConfigFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepBesshuDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepMeisaiDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepFaultDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepKadoDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepTrafficDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepRiyoDataFromFilesystem))
                job = Action.Combine(job, New Action(AddressOf SweepMadoLogsFromFilesystem))
                '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
                job = Action.Combine(job, New Action(AddressOf SweepMadoCertLogsFromFilesystem))
                '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------
                job = Action.Combine(job, New Action(AddressOf SweepLogsFromFilesystem))

                oMainForm = New ServerAppForm()

                '����ƃX���b�h���J�n����B
                Dim oWorkerThread As New Thread(AddressOf MainClass.WorkingLoop)
                Log.Info("Starting the worker thread...")
                quitWorker = 0
                oWorkerThread.Name = "Worker"
                oWorkerThread.Start()

                '�E�C���h�E�v���V�[�W�������s����B
                'NOTE: ���̃��\�b�h�����O���X���[����邱�Ƃ͂Ȃ��B
                ServerAppBaseMain(oMainForm)

                Try
                    '����ƃX���b�h�ɏI����v������B
                    Log.Info("Sending quit request to the worker thread...")
                    Thread.VolatileWrite(quitWorker, 1)

                    '����ƃX���b�h�̏I����҂B
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
                Log.Info("�v���Z�X�I��")

                'NOTE: ������ʂ�Ȃ��Ă��A���̃X���b�h�̏��łƂƂ��ɉ�������
                '�悤�Ȃ̂ŁA�ň��̐S�z�͂Ȃ��B
                m.ReleaseMutex()

                Application.Exit()
            End Try
        End If
    End Sub

    ''' <summary>
    ''' ����ƃX���b�h�̃��C�������B
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
                'TODO: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j
            End Try
        Next

        Log.Info("All jobs ended.")
        oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
    End Sub

    Private Shared Sub SweepMachineInfo()
        Log.Info("Called.")

        '��ԌÂ��z�M�J�n���̒��O�̐���܂ł��c���āA
        '�@��\���}�X�^�̃��R�[�h���폜����B

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

                'NOTE: �P��Agent�ɂ��A�z�M�˗�����}�X�^�̓K�p�Ώۋ@�킪
                '�P�����ł��邱�ƂɈˑ����Ă���B
                '�{���ł���΁ADLL��ԃe�[�u���ɂ́A���M��@�킾���łȂ��A
                '�}�X�^�̓K�p�Ώۋ@����i�[�ł�������悢�B
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

                'NOTE: S_MST_DLL_STS��S_MST_DL_STS����̓K�p���X�g�z�M��Ԃ̍폜�͍s��Ȃ��B
                '�����̃��R�[�h�͓��Y���X�g�o�[�W�����E���Y�p�^�[��No��
                '�K�p���X�g�ŐV���Ȕz�M�w�����s��ꂽ�ۂɁA�܂Ƃ߂č폜����B
                '�V���ɓo�^����錩���݂��Ȃ��p�^�[��No�̃��R�[�h�i�\�z�O��
                'DL�����ʒm���󂯂��ۂɓo�^�������R�[�h����сA���͔p�~���ꂽ
                '�p�^�[��No�Ŕz�M���s�����ۂ̌Â����R�[�h���j�̍폜�ɂ��Ă��A
                '����Ǘ��Ƃ͕ʌ��ł��邽�߁A�����ł͂Ȃ��ASweepDeadPatternRelatedData�ōs���B
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

                'NOTE: �v���O������ʂ��@��Ԃŏd�����Ă��Ȃ����ƂɈˑ����Ă���B
                '�{���ł���΁ADLL��ԃe�[�u���ɂ́A���M��@�킾���łȂ��A
                '�v���O�����̓K�p�Ώۋ@����i�[�ł�������悢�B
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

                'NOTE: S_PRG_DLL_STS��S_PRG_DL_STS����̓K�p���X�g�z�M��Ԃ̍폜�͍s��Ȃ��B
                '�����̃��R�[�h�͓��Y���X�g�o�[�W�����E���Y�G���ANo��
                '�K�p���X�g�ŐV���Ȕz�M�w�����s��ꂽ�ۂɁA�܂Ƃ߂č폜����B
                '�V���ɓo�^����錩���݂��Ȃ��G���ANo�̃��R�[�h�i�\�z�O��
                'DL�����ʒm���󂯂��ۂɓo�^�������R�[�h����сA���͔p�~���ꂽ
                '�G���ANo�Ŕz�M���s�����ۂ̌Â����R�[�h���j�̍폜�ɂ��Ă��A
                '����Ǘ��Ƃ͕ʌ��ł��邽�߁A�����ł͂Ȃ��ASweepDeadAreaRelatedData�ōs���B
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

        '���݂��Ȃ��p�^�[����`�Ɉˑ����郌�R�[�h��
        '�e��e�[�u������폜����B

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            'NOTE: �P��Agent�ɂ��A�z�M�˗�����}�X�^�̓K�p�Ώۋ@�킪
            '�P�����ł��邱�ƂɈˑ����Ă���B
            '�{���ł���΁ADLL��ԃe�[�u���ɂ́A���M��@�킾���łȂ��A
            '�}�X�^�̓K�p�Ώۋ@����i�[�ł�������悢�B
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

        '���݂��Ȃ��G���A��`�Ɉˑ����郌�R�[�h��
        '�e��e�[�u������폜����B

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            'NOTE: �G���A�̋@�킩��v���O������ʂ𓱂��ہA�Œ�l�uPG�v���g����
            '�����Ă��邱�Ƃɒ��ӁB�܂��A���̔���@���́A�v���O������ʂ��@��
            '���Ƃɕʂ̖��O�ōő�ł��P�����p�ӂ���Ă��邱�ƂɈˑ����Ă���B
            '�{���ł���΁ADLL��ԃe�[�u���ɂ́A���M��@�킾���łȂ��A
            '�v���O�����̓K�p�Ώۋ@����i�[�ł�������悢�B
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

        '�ŐV�̋@��\���ɑ��݂��Ȃ����@�̃��R�[�h��
        '�}�X�^DLL�o�[�W�����e�[�u������폜����B

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

        '�ŐV�̋@��\���ɑ��݂��Ȃ����@�̃��R�[�h��
        '�v���O����DLL�o�[�W�����e�[�u������폜����B

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

        '�ŐV�̋@��\���ɑ��݂��Ȃ����@�̃��R�[�h��
        '�}�X�^�o�[�W�������e�[�u������폜����B

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

        '�ŐV�̋@��\���ɑ��݂��Ȃ����@�̃��R�[�h��
        '�v���O�����o�[�W�������e�[�u������폜����B

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

        '�ŐV�̋@��\���ɑ��݂��Ȃ����@�̃��R�[�h��
        '�^�ǃT�[�o�@��ڑ���ԃe�[�u������폜����B

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

        '�ŐV�̋@��\���ɑ��݂��Ȃ����@�̃��R�[�h��
        '�@��ڑ���ԃe�[�u������폜����B

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

        '�ŐV�̋@��\���ɑ��݂��Ȃ����@�̃��R�[�h��
        '�Ď��Րݒ���e�[�u������폜����B

        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            '----Ver0.2 �Ď��Րݒ���폜�̋@��C���@MOD�@START------------------------
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
            '----Ver0.2 �Ď��Րݒ���폜�̋@��C���@MOD�@END--------------------------
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

        'Config.BesshuDataVisibleDays���o�߂������R�[�h��
        '�ʏW�D�f�[�^�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_BESSHU_DATA", Config.BesshuDataVisibleDays)
    End Sub

    Private Shared Sub SweepFuseiJoshaDataFromDatabase()
        Log.Info("Called.")

        'Config.FuseiJoshaDataVisibleDays���o�߂������R�[�h��
        '�s����Ԍ����o�f�[�^�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_FUSEI_JOSHA_DATA", Config.FuseiJoshaDataVisibleDays)
    End Sub

    Private Shared Sub SweepKyokoToppaDataFromDatabase()
        Log.Info("Called.")

        'Config.KyokoToppaDataVisibleDays���o�߂������R�[�h��
        '���s�˔j���o�f�[�^�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_KYOKO_TOPPA_DATA", Config.KyokoToppaDataVisibleDays)
    End Sub

    Private Shared Sub SweepFunshitsuDataFromDatabase()
        Log.Info("Called.")

        'Config.FunshitsuDataVisibleDays���o�߂������R�[�h��
        '���������o�f�[�^�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_FUNSHITSU_DATA", Config.FunshitsuDataVisibleDays)
    End Sub

    Private Shared Sub SweepFaultDataFromDatabase()
        Log.Info("Called.")

        'Config.FaultDataVisibleDays���o�߂������R�[�h��
        '�ُ�f�[�^�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_FAULT_DATA", Config.FaultDataVisibleDays)
    End Sub

    Private Shared Sub SweepKadoDataFromDatabase()
        Log.Info("Called.")

        'Config.KadoDataVisibleDays���o�߂������R�[�h��
        '�ғ��f�[�^�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_KADO_DATA", Config.KadoDataVisibleDays)
    End Sub

    Private Shared Sub SweepHosyuDataFromDatabase()
        Log.Info("Called.")

        'Config.HosyuDataVisibleDays���o�߂������R�[�h��
        '�ێ�f�[�^�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_HOSYU_DATA", Config.HosyuDataVisibleDays)
    End Sub

    Private Shared Sub SweepTrafficDataFromDatabase()
        Log.Info("Called.")

        'Config.TrafficDataVisibleDays���o�߂������R�[�h��
        '���ԑѕʏ�~�f�[�^�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_TRAFFIC_DATA", Config.TrafficDataVisibleDays)
    End Sub

    Private Shared Sub SweepCollectedDataTypoFromDatabase()
        Log.Info("Called.")

        'Config.CollectedDataTypoVisibleDays���o�߂������R�[�h��
        '���W�f�[�^��L�e�[�u������폜����B
        SweepOldRecordsFromDatabase("D_COLLECTED_DATA_TYPO", Config.CollectedDataTypoVisibleDays)
    End Sub

    '-------Ver0.3 ������ԕ�Ή� ADD START-----------
    Private Shared Sub SweepRiyoDataFromDatabase()
        Log.Info("Called.")

        'Config.RiyoDataVisibleDays���o�߂������R�[�h��
        '�S�w�̗��p�f�[�^�e�[�u������폜����B
        Dim oTable As DataTable = SelectTableNames("D_RIYO_DATA_[A-Z][0-9]_[0-9][0-9][0-9][0-9][0-9][0-9]".Replace("_", "?_"), Config.RiyoDataDatabaseName)
        If oTable IsNot Nothing Then
            For Each oRow As DataRow In oTable.Rows
                SweepOldRecordsFromDatabase(Config.RiyoDataDatabaseName & ".dbo." & oRow.Field(Of String)(0), Config.RiyoDataVisibleDays)
            Next oRow
        End If
    End Sub
    '-------Ver0.3 ������ԕ�Ή� ADD END-------------

    '-------Ver0.3 ������ԕ�Ή� ADD START-----------
    Private Shared Sub SweepShiteiDataFromDatabase()
        Log.Info("Called.")

        'Config.ShiteiDataVisibleDays���o�߂������R�[�h��
        '�S�w�̐V�����w�茔����f�[�^�e�[�u������폜����B
        Dim oTable As DataTable = SelectTableNames("D_SHITEI_DATA_[0-9][0-9][0-9][0-9][0-9][0-9]".Replace("_", "?_"), Config.ShiteiDataDatabaseName)
        If oTable IsNot Nothing Then
            For Each oRow As DataRow In oTable.Rows
                SweepOldRecordsFromDatabase(Config.ShiteiDataDatabaseName & ".dbo." & oRow.Field(Of String)(0), Config.ShiteiDataVisibleDays)
            Next oRow
        End If
    End Sub
    '-------Ver0.3 ������ԕ�Ή� ADD END-------------

    Private Shared Sub SweepConStatusFromFilesystem()
        Log.Info("Called.")

        'Config.ConStatusKeepingDaysInRejectDir���o�߂����t�@�C����
        'Config.RejectDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForConStatus"), Config.ConStatusKeepingDaysInRejectDir)

        'Config.ConStatusKeepingDaysInTrashDir���o�߂����f�B���N�g����
        'Config.TrashDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForConStatus"), Config.ConStatusKeepingDaysInTrashDir)

        'Config.ConStatusKeepingDaysInQuarantineDir���o�߂����f�B���N�g����
        'Config.QuarantineDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForConStatus"), Config.ConStatusKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBase�f�B���N�g�����̃t�@�C����
        'Config.InputDirPathInRecordingBase�f�B���N�g���Ɉړ�����H
        'TODO: ���Y�f�[�^����M����ʐM�v���Z�X�����삵�Ă��邱�Ƃ��l������ƁA
        'UpboundDataPath.Gen()���g�������ł͍ς܂Ȃ��i�r�����䂪�K�v�j�B
    End Sub

    Private Shared Sub SweepKsbConfigFromFilesystem()
        Log.Info("Called.")

        'Config.KsbConfigKeepingDaysInRejectDir���o�߂����t�@�C����
        'Config.RejectDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForKsbConfig"), Config.KsbConfigKeepingDaysInRejectDir)

        'Config.KsbConfigKeepingDaysInTrashDir���o�߂����f�B���N�g����
        'Config.TrashDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForKsbConfig"), Config.KsbConfigKeepingDaysInTrashDir)

        'Config.KsbConfigKeepingDaysInQuarantineDir���o�߂����f�B���N�g����
        'Config.QuarantineDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForKsbConfig"), Config.KsbConfigKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBase�f�B���N�g�����̃t�@�C����
        'Config.InputDirPathInRecordingBase�f�B���N�g���Ɉړ�����H
        'TODO: ���Y�f�[�^����M����ʐM�v���Z�X�����삵�Ă��邱�Ƃ��l������ƁA
        'UpboundDataPath.Gen()���g�������ł͍ς܂Ȃ��i�r�����䂪�K�v�j�B
    End Sub

    Private Shared Sub SweepBesshuDataFromFilesystem()
        Log.Info("Called.")

        'Config.BesshuDataKeepingDaysInRejectDir���o�߂����t�@�C����
        'Config.RejectDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForBesshuData"), Config.BesshuDataKeepingDaysInRejectDir)

        'Config.BesshuDataKeepingDaysInTrashDir���o�߂����f�B���N�g����
        'Config.TrashDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForBesshuData"), Config.BesshuDataKeepingDaysInTrashDir)

        'Config.BesshuDataKeepingDaysInQuarantineDir���o�߂����f�B���N�g����
        'Config.QuarantineDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForBesshuData"), Config.BesshuDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBase�f�B���N�g�����̃t�@�C����
        'Config.InputDirPathInRecordingBase�f�B���N�g���Ɉړ�����H
        'TODO: ���Y�f�[�^����M����ʐM�v���Z�X�����삵�Ă��邱�Ƃ��l������ƁA
        'UpboundDataPath.Gen()���g�������ł͍ς܂Ȃ��i�r�����䂪�K�v�j�B
    End Sub

    Private Shared Sub SweepMeisaiDataFromFilesystem()
        Log.Info("Called.")

        'Config.MeisaiDataKeepingDaysInRejectDir���o�߂����t�@�C����
        'Config.RejectDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForMeisaiData"), Config.MeisaiDataKeepingDaysInRejectDir)

        'Config.MeisaiDataKeepingDaysInTrashDir���o�߂����f�B���N�g����
        'Config.TrashDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForMeisaiData"), Config.MeisaiDataKeepingDaysInTrashDir)

        'Config.MeisaiDataKeepingDaysInQuarantineDir���o�߂����f�B���N�g����
        'Config.QuarantineDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForMeisaiData"), Config.MeisaiDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBase�f�B���N�g�����̃t�@�C����
        'Config.InputDirPathInRecordingBase�f�B���N�g���Ɉړ�����H
        'TODO: ���Y�f�[�^����M����ʐM�v���Z�X�����삵�Ă��邱�Ƃ��l������ƁA
        'UpboundDataPath.Gen()���g�������ł͍ς܂Ȃ��i�r�����䂪�K�v�j�B
    End Sub

    Private Shared Sub SweepFaultDataFromFilesystem()
        Log.Info("Called.")

        'Config.FaultDataKeepingDaysInRejectDir���o�߂����t�@�C����
        'Config.RejectDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForFaultData"), Config.FaultDataKeepingDaysInRejectDir)

        'Config.FaultDataKeepingDaysInTrashDir���o�߂����f�B���N�g����
        'Config.TrashDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForFaultData"), Config.FaultDataKeepingDaysInTrashDir)

        'Config.FaultDataKeepingDaysInQuarantineDir���o�߂����f�B���N�g����
        'Config.QuarantineDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForFaultData"), Config.FaultDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBase�f�B���N�g�����̃t�@�C����
        'Config.InputDirPathInRecordingBase�f�B���N�g���Ɉړ�����H
        'TODO: ���Y�f�[�^����M����ʐM�v���Z�X�����삵�Ă��邱�Ƃ��l������ƁA
        'UpboundDataPath.Gen()���g�������ł͍ς܂Ȃ��i�r�����䂪�K�v�j�B
    End Sub

    Private Shared Sub SweepKadoDataFromFilesystem()
        Log.Info("Called.")

        'Config.KadoDataKeepingDaysInRejectDir���o�߂����t�@�C����
        'Config.RejectDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForKadoData"), Config.KadoDataKeepingDaysInRejectDir)

        'Config.KadoDataKeepingDaysInTrashDir���o�߂����f�B���N�g����
        'Config.TrashDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForKadoData"), Config.KadoDataKeepingDaysInTrashDir)

        'Config.KadoDataKeepingDaysInQuarantineDir���o�߂����f�B���N�g����
        'Config.QuarantineDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForKadoData"), Config.KadoDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBase�f�B���N�g�����̃t�@�C����
        'Config.InputDirPathInRecordingBase�f�B���N�g���Ɉړ�����H
        'TODO: ���Y�f�[�^����M����ʐM�v���Z�X�����삵�Ă��邱�Ƃ��l������ƁA
        'UpboundDataPath.Gen()���g�������ł͍ς܂Ȃ��i�r�����䂪�K�v�j�B
    End Sub

    Private Shared Sub SweepTrafficDataFromFilesystem()
        Log.Info("Called.")

        'Config.TrafficDataKeepingDaysInRejectDir���o�߂����t�@�C����
        'Config.RejectDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.RejectDirPathForApps("ForTrafficData"), Config.TrafficDataKeepingDaysInRejectDir)

        'Config.TrafficDataKeepingDaysInTrashDir���o�߂����f�B���N�g����
        'Config.TrashDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.TrashDirPathForApps("ForTrafficData"), Config.TrafficDataKeepingDaysInTrashDir)

        'Config.TrafficDataKeepingDaysInQuarantineDir���o�߂����f�B���N�g����
        'Config.QuarantineDirPathInRecordingBase�f�B���N�g������폜����B
        SweepOldDirectoriesFromFilesystem(Config.QuarantineDirPathForApps("ForTrafficData"), Config.TrafficDataKeepingDaysInQuarantineDir)

        'Config.SuspenseDirPathInRecordingBase�f�B���N�g�����̃t�@�C����
        'Config.InputDirPathInRecordingBase�f�B���N�g���Ɉړ�����H
        'TODO: ���Y�f�[�^����M����ʐM�v���Z�X�����삵�Ă��邱�Ƃ��l������ƁA
        'UpboundDataPath.Gen()���g�������ł͍ς܂Ȃ��i�r�����䂪�K�v�j�B
    End Sub

    Private Shared Sub SweepRiyoDataFromFilesystem()
        Log.Info("Called.")
        'Config.RiyoDataKeepingDaysInRejectDir���o�߂����t�@�C����
        'Config.RiyoDataRejectDirPathInStationBase�f�B���N�g������폜����B
        If Directory.Exists(Config.RiyoDataDirPath) Then
            Dim aStationDirs As String() = Directory.GetDirectories(Config.RiyoDataDirPath)
            For Each sStationDir As String In aStationDirs
                Dim sPath As String = Utility.CombinePathWithVirtualPath(sStationDir, Config.RiyoDataRejectDirPathInStationBase)
                '-------Ver0.3 ������ԕ�Ή� MOD START-----------
                SweepOldFilesFromFilesystem2(sPath, Config.RiyoDataKeepingDaysInRejectDir)
                '-------Ver0.3 ������ԕ�Ή� MOD END-------------
            Next sStationDir
        End If

        'NOTE: �풓�v���Z�X�Ƃ��āuToNkan�v����сuForRiyoData�v���ݒ肳��Ă��邩�ۂ���
        '�폜����Ώۂ�؂�ւ���B
        If Directory.Exists(Config.RiyoDataDirPath) Then
            Dim aStationDirs As String() = Directory.GetDirectories(Config.RiyoDataDirPath)
            For Each sStationDir As String In aStationDirs
                '-------Ver0.3 ������ԕ�Ή� MOD START-----------
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
                '-------Ver0.3 ������ԕ�Ή� MOD END-------------
            Next sStationDir
        End If

        'TODO: �w���@��\������������ꍇ�́A�w�f�B���N�g�����Ə��������B
        '�������A���̓��ɏ����킯�ɂ͂����Ȃ��i�ێ����Ԃ����Ȃ����ƂɂȂ��A
        '�ݒ莟��ł͏W�v����Ȃ��̂ɁA�����Ă��܂����ƂɂȂ�j���߁A��L������
        '��ɉw�f�B���N�g���̒����S�T�u�f�B���N�g���̒��܂łɋ�ɂȂ��Ă���ꍇ
        '�ɂ̂ݏ����悤�ɂ��Ȃ���΂Ȃ�Ȃ��B
    End Sub

    Private Shared Sub SweepMadoLogsFromFilesystem()
        Log.Info("Called.")

        'Config.MadoLogsKeepingDays���o�߂����t�@�C����
        'Config.MadoLogDirPath�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.MadoLogDirPath, Config.MadoLogsKeepingDays)
    End Sub

    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
    Private Shared Sub SweepMadoCertLogsFromFilesystem()
        Log.Info("Called.")

        'Config.MadoCertLogsKeepingDays���o�߂����t�@�C����
        'Config.MadoCertLogDirPath�f�B���N�g������폜����B
        SweepOldFilesFromFilesystem(Config.MadoCertLogDirPath, Config.MadoCertLogsKeepingDays)
    End Sub
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------

    Private Shared Sub SweepLogsFromFilesystem()
        Log.Info("Called.")

        Dim oRegxElem As New StringBuilder()
        For Each sAppName As String In aAppNames
            oRegxElem.Append(sAppName & "|")
        Next sAppName
        oRegxElem.Remove(oRegxElem.Length - 1, 1)

        'Config.LogsKeepingDays���o�߂����t�@�C����
        'REG_LOG�f�B���N�g������폜����B
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

            'TODO: �ێ�Œ���I�Ƀ`�F�b�N����ۂ̃L�[���^�ǂł́u�ύX�����v�ł͂Ȃ�
            '�w���@��ł́u���������v�iDB�d�l���Łu���W�����v�ƌĂ΂�鍀�ځj�Ƃ���Ȃ�A
            '����������؂��āuSYUSYU_DATE�v���L�[�ɂ���ׂ��ł���B
            '�����łȂ��i�ύX�������L�[�ɂ���j�Ȃ�A�����̌��������łȂ��A
            '�^�ǒ[�����猟������ۂ̌�����A�C���f�b�N�X�̗򉻂��ɂ������l���Ă��A
            '�u���W�����v����L�[����O���āA����Ɂu�ύX�����v����L�[�ɂ���ׂ��ł���B
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

    '-------Ver0.3 ������ԕ�Ή� ADD START-----------
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
    '-------Ver0.3 ������ԕ�Ή� ADD END-------------

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

    '-------Ver0.3 ������ԕ�Ή� ADD START-----------
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
    '-------Ver0.3 ������ԕ�Ή� ADD END-------------

End Class
