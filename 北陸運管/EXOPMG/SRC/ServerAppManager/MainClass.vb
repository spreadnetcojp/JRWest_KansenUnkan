' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/06/07  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Messaging
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' �v���Z�X�}�l�[�W���̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "�萔��ϐ�"
    '���C���E�B���h�E
    Private Shared oMainForm As ServerAppForm

    '����ƃX���b�h�ւ̏I���v���t���O
    Private Shared quitWorker As Integer
#End Region

    ''' <summary>
    ''' �v���Z�X�}�l�[�W���̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �v���Z�X�}�l�[�W���̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppManager")
        If m.WaitOne(0, False) Then
            Try
                Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
                If sLogBasePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
                    Return
                End If

                Dim sIniFilePath As String = Constant.GetEnv(REG_SERVER_INI)
                If sIniFilePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_SERVER_INI)
                    Return
                End If

                Log.Init(sLogBasePath, "Manager")
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

                    'NOTE: �f�o�b�O���ȂǂłȂ�����͎���ƃX���b�h��
                    '�K���I�����邱�Ƃ�O��ɂ��Ă���B
                    'TODO: �^�p��A�I���͖�Ԃ̖��l��Ԃōs����́A
                    '��L�̑O���݂���̂�NG��������Ȃ��B
                    '�����ł���Ȃ�AJoin�Ɋ�����݂��āA�^�C���A�E�g����
                    'SNMP TRAP�⃁�[���Ń��[�U�ɒʒm���邱�Ƃ�����
                    '���Ȃ���΂Ȃ�Ȃ��B

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
    ''' <remarks>
    ''' �e��풓�v���Z�X�̋N���E�Ď��E�I�����s���B
    ''' </remarks>
    Private Shared Sub WorkingLoop()
        Dim aProcesses(Config.ResidentApps.Length - 1) As System.Diagnostics.Process

        Try
            Log.Info("The worker thread started.")

            '�e�v���Z�X�̃��b�Z�[�W�L���[���쐬����B
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                If Config.MqPathForApps.ContainsKey(Config.ResidentApps(i)) Then
                    Dim sMqPath As String = Config.MqPathForApps(Config.ResidentApps(i))
                    If Not MessageQueue.Exists(sMqPath) Then
                        Log.Info("Registering [" & Config.MqPathForApps(Config.ResidentApps(i)) & "]...")
                        MessageQueue.Create(sMqPath)
                    End If
                End If
            Next

            '�e�v���Z�X���N��������B
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                aProcesses(i) = New System.Diagnostics.Process()
                aProcesses(i).StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, GetFileNameOfProcess(i))
                aProcesses(i).StartInfo.UseShellExecute = False
                Log.Info("Starting [" & aProcesses(i).StartInfo.FileName & "]...")
                aProcesses(i).Start()

                'NOTE: �v���Z�X�̋N������Ɉُ킪�������āA���b�Z�[�W�{�b�N�X��
                '�\�����ꂽ�i�{�^�������҂��ɂȂ����j�ꍇ���A���L���\�b�h����
                '���A���Ă��܂��͂��ł���B���̃P�[�X�ł́A�����ؖ����s����
                '���Ȃ��͂��ł��邽�߁A�����Ɏ����`�F�b�N���s���ƁA���[�U��
                '���b�Z�[�W�{�b�N�X�̓��e���m�F����O�ɁA�v���Z�X��Kill����
                '���ƂɂȂ��Ă��܂��B���̈Ӗ��ł��A�����`�F�b�N�̎�����
                '�Z���������邱�Ƃ�NG�ł���B
                aProcesses(i).WaitForInputIdle()
            Next

            Dim oStatusPollTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            oStatusPollTimer.Start(TickTimer.GetSystemTick())

            While Thread.VolatileRead(quitWorker) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oStatusPollTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oStatusPollTimer.Start(systemTick)

                    For i As Integer = 0 To Config.ResidentApps.Length - 1
                        '�I�����Ă���v���Z�X���ċN��������B
                        If aProcesses(i).HasExited Then
                            Log.Fatal("[" & GetFileNameOfProcess(i) & "] is aborted.")

                            '���W�f�[�^��L�e�[�u���Ɉُ��o�^����B
                            CollectedDataTypoRecorder.Record( _
                               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                               DbConstants.CdtKindServerError, _
                               Lexis.CdtProcessAbended.Gen(GetFileNameOfProcess(i)))

                            aProcesses(i).Close()
                            aProcesses(i) = New System.Diagnostics.Process()
                            aProcesses(i).StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, GetFileNameOfProcess(i))
                            aProcesses(i).StartInfo.UseShellExecute = False
                            Log.Info("Starting [" & aProcesses(i).StartInfo.FileName & "]...")
                            aProcesses(i).Start()
                            aProcesses(i).WaitForInputIdle()
                        End If
                    Next

                    '�e�v���Z�X���������Ă��邩�`�F�b�N���s���B
                    'NOTE: ���ꂪ�Ȃ��ƁA�X�̃v���Z�X�Ɋւ��āu�z�肵�Ȃ�
                    '��O�����������ۂ�A�Ǘ��n�X���b�h�őz�肵�Ȃ��ُ��
                    '�F�������ۂ́A�K���v���Z�X�S�̂̏I���ɑ����t���Ȃ����
                    '�Ȃ�Ȃ��v�u�t�H�A�O���E���h�X���b�h�̏I���͐�΂�
                    '�s����悤�ɍ�荞�ށv�Ȃǂ̑O�񂪕K�v�ɂȂ�B
                    '�X�̃v���Z�X�����̂悤�ɍ�荞�ނ̂͗��z�ł��邪�A
                    '������̎��Ԃ��l����ƁA�����ŕی���������΁A
                    '�^�p�I�Ɉ��S�ɂȂ邵�A�X�̃v���Z�X����荞�ޏ�ł�
                    '�s���v�f�����Ȃ��Ȃ�B
                    For i As Integer = 0 To Config.ResidentApps.Length - 1
                        Dim sFilePath As String = Path.Combine(Config.ResidentAppPulseDirPath, Config.ResidentApps(i))
                        Dim lastWriteTime As DateTime = File.GetLastWriteTime(sFilePath)
                        If lastWriteTime + New TimeSpan(CLng(Config.ResidentAppPendingLimitTicks) * 10000) < DateTime.Now Then
                            Log.Fatal("[" & GetFileNameOfProcess(i) & "] seems broken.")

                            '���W�f�[�^��L�e�[�u���Ɉُ��o�^����B
                            CollectedDataTypoRecorder.Record( _
                               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                               DbConstants.CdtKindServerError, _
                               Lexis.CdtProcessAbended.Gen(GetFileNameOfProcess(i)))

                            Try
                                aProcesses(i).Kill()
                            Catch ex As Exception
                                Log.Error("Exception caught.", ex)
                            End Try

                            aProcesses(i).Close()
                            aProcesses(i) = New System.Diagnostics.Process()
                            aProcesses(i).StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, GetFileNameOfProcess(i))
                            aProcesses(i).StartInfo.UseShellExecute = False
                            Log.Info("Starting [" & aProcesses(i).StartInfo.FileName & "]...")
                            aProcesses(i).Start()
                            aProcesses(i).WaitForInputIdle()
                        End If
                    Next

                End If
                Thread.Sleep(Config.PollIntervalTicks)
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'TODO: TRAP�����⃁�[���ʒm�ȂǁA���Ƃ�����
            '���[�U�ɋC�t���Ă��炤�K�v������B

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))

        Finally
            '�e�v���Z�X���I��������B
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                If aProcesses(i) IsNot Nothing Then
                    Try
                        Log.Info("Sending quit request to [" & GetFileNameOfProcess(i) & "]...")
                        aProcesses(i).CloseMainWindow()
                    Catch ex As Exception
                        'NOTE: ���̃P�[�X�̑z��ɂ́AaProcesses(i)��Start��
                        '���s�����ꍇ�����łȂ��AStart�������aProcesses(i)
                        '���炪�I�������ꍇ���܂܂��B
                        '��҂̏ꍇ�́AFatal�ȃ��O���o�͂���Ă��Ȃ��͂���
                        '���邽�߁A�����ŏo�͂��郍�O��Fatal�Ƃ���B
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                End If
            Next

            '�e�v���Z�X�̏I����҂B
            Dim oJoinLimitTimer As New TickTimer(Config.ResidentAppPendingLimitTicks)
            oJoinLimitTimer.Start(TickTimer.GetSystemTick())
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                If aProcesses(i) IsNot Nothing Then
                    Try
                        Dim ticks As Long = oJoinLimitTimer.GetTicksToTimeout(TickTimer.GetSystemTick())
                        If ticks < 0 Then ticks = 0

                        Log.Info("Waiting for [" & GetFileNameOfProcess(i) & "] to quit...")
                        If aProcesses(i).WaitForExit(CInt(ticks)) = False Then
                            Log.Fatal("[" & GetFileNameOfProcess(i) & "] seems broken.")

                            '���W�f�[�^��L�e�[�u���Ɉُ��o�^����B
                            CollectedDataTypoRecorder.Record( _
                               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                               DbConstants.CdtKindServerError, _
                               Lexis.CdtProcessAbended.Gen(GetFileNameOfProcess(i)))

                            Try
                                aProcesses(i).Kill()
                            Catch ex As Exception
                                Log.Error("Exception caught.", ex)
                            End Try
                        Else
                            Log.Info("[" & GetFileNameOfProcess(i) & "] has quit.")
                        End If
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                End If
            Next

            '�e�v���Z�X�̃n���h�����������B
            For i As Integer = 0 To Config.ResidentApps.Length - 1
                If aProcesses(i) IsNot Nothing Then
                    Try
                        aProcesses(i).Close()
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                End If
            Next
        End Try
    End Sub

    Private Shared Function GetFileNameOfProcess(ByVal i As Integer) As String
        Return "ExOpmgServerApp" & Config.ResidentApps(i) & ".exe"
    End Function

End Class
