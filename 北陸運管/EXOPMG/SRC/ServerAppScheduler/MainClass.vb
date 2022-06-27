' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Messaging
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' �X�P�W���[���v���Z�X�̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "�����N���X��"
    Private Class ScheduledEvent
        '�^�C�g��
        Public Title As String

        '�ݒ���
        Public Config As ScheduledEventConfig

        '�ŏI���{�����i���K���ς݁j
        Public LastExecTime As DateTime
    End Class
#End Region

#Region "�萔��ϐ�"
    '�e�C�x���g�̏��
    Private Shared oScheduledEvents As List(Of ScheduledEvent)

    '���C���E�B���h�E
    Private Shared oMainForm As ServerAppForm

    '����ƃX���b�h�ւ̏I���v���t���O
    Private Shared quitWorker As Integer
#End Region

    ''' <summary>
    ''' �X�P�W���[���v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �X�P�W���[���v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppScheduler")
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

                Log.Init(sLogBasePath, "Scheduler")
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

                Try
                    oScheduledEvents = New List(Of ScheduledEvent)
                    Dim now As DateTime = DateTime.Now
                    For Each oEventConfig As KeyValuePair(Of String, ScheduledEventConfig) In Config.ScheduledEvents
                        Dim oEvent As New ScheduledEvent()
                        oEvent.Title = oEventConfig.Key
                        oEvent.Config = oEventConfig.Value
                        oEvent.LastExecTime = oEventConfig.Value.Normalize(now)
                        oScheduledEvents.Add(oEvent)
                    Next oEventConfig
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnInitializingProcess)
                    Return
                End Try

                '���b�Z�[�W���[�v���A�C�h����ԂɂȂ�O�i���A����I�ɂ�����s��
                '�X���b�h���N������O�j�ɁA�����ؖ��t�@�C�����X�V���Ă����B
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

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

                    'NOTE: �ȉ��Ŏ���ƃX���b�h���I�����Ȃ��ꍇ�A
                    '����ƃX���b�h�͐����ؖ����s��Ȃ��͂��ł���A
                    '�󋵂ւ̑Ώ��̓v���Z�X�}�l�[�W���ōs����z��ł���B

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
    ''' �����̊Ď��ƃ��b�Z�[�W�̑��M���s���B
    ''' </remarks>
    Private Shared Sub WorkingLoop()
        Try
            Log.Info("The worker thread started.")

            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Thread.VolatileRead(quitWorker) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()
                End If

                Dim now As DateTime = DateTime.Now
                For Each oEvent As ScheduledEvent In oScheduledEvents
                    Dim normNow As DateTime = oEvent.Config.Normalize(now)
                    If normNow > oEvent.LastExecTime Then
                        'StartMinutesInDay�ȏ�ɂȂ�悤�ɕ␳����
                        '���ݎ������i0��0������̌o�ߕ��̌`���Łj���߂�B
                        Dim nowMinutesInDay As Integer = normNow.Hour * 60 + normNow.Minute
                        If oEvent.Config.StartMinutesInDay > nowMinutesInDay Then
                            nowMinutesInDay += 24 * 60
                        End If

                        '�L�����ԑт̂ݑ��M���s���B
                        If nowMinutesInDay <= oEvent.Config.EndMinutesInDay Then
                            Log.Info("It's now time to " & oEvent.Title & ".")

                            Dim oMessage As New Message()
                            oMessage.AppSpecific = oEvent.Config.MessageKind
                            oMessage.Body = oEvent.Config.MessageBody
                            For Each oTargetApp As String In oEvent.Config.TargetApps
                                Config.MessageQueueForApps(oTargetApp).Send(oMessage)
                            Next oTargetApp
                        End If

                        oEvent.LastExecTime = normNow
                    ElseIf normNow < oEvent.LastExecTime Then
                        '�V�X�e��������2�����ȏ�߂��ꂽ�ꍇ�́A
                        '�ŏI���{�����𐳋K�������V�X�e�������ɍ��킹��B
                        Dim span As TimeSpan = oEvent.LastExecTime - normNow
                        Dim cycles As Integer = span.Minutes \ oEvent.Config.Cycle
                        If cycles > 1 Then
                            Log.Warn("The system time goes back into the past.")
                            oEvent.LastExecTime = normNow
                        End If
                    End If
                Next oEvent

                Thread.Sleep(Config.PollIntervalTicks)
            End While
            Log.Info("Quit requested by manager.")
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j�́A
            '�v���Z�X�}�l�[�W�����s���̂ŁA�����ł͕s�v�ł���B

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        End Try
    End Sub


End Class
