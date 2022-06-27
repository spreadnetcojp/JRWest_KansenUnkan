' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/04/10  (NES)����  ������ԕ�Ή��ɂĐV�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Linq
Imports System.Messaging
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

'NOTE: �����A���̃v���Z�X�Ɠ������@�ŕێ�n�f�[�^�̓o�^���s���悤�ɉ��P��
'�s���Ȃ�A���̃N���X��Config��Recorder�ƂƂ��ɔėp���̂���N���X�ɂ��āA
'ServerAppForAnyUpboundData2�v���W�F�N�g�Ɉړ�����B
'�����̃N���X�ł́A���p�f�[�^����ȃf�[�^�Ɉˑ������l���i�h���N���X��
'�Z�b�g����z��́jImmutable�ȃ����o�ϐ�����Q�Ƃ���悤�ɂ��A�܂��A
'�t�@�C������ǂݍ���SQL��z��ȂǂŊǗ����邱�Ƃɂ��āA�C�ӂ̐��̃e�[�u����
'�΂���Insert�����s����悤�ɂ���B��������SQL�t�@�C�����ŕ������L�q����
'���[���ɂ��āA1�t�@�C���Ɍ��肷����j�ł��悢�B
'ServerAppForRiyoData�v���W�F�N�g�́A�����̔h���N���X��p�ӂ��邱�ƂɂȂ�A
'�h���N���X�̎�Ȏ����́A�R���X�g���N�^�ɂ�����u���p�f�[�^��w�茔�f�[�^��
'�ˑ������l�̃����o�ϐ��ւ̃Z�b�g�v�����ɂȂ�͂��ł���B

''' <summary>
''' �w�ʃf�[�^�o�^�X���b�h��ΏۂƂ���Ď��X���b�h�B
''' </summary>
Public Class MyWatcher

#Region "�����N���X��"
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

#Region "�萔��ϐ�"
    '�X���b�h��
    Protected Const ThreadName As String = "Watcher"

    '�o�^�X���b�h���̏���
    Protected Const RecorderNameFormat As String = "%3R%3S"

    '�o�^�X���b�h��Abort��������
    Protected Const RecorderAbortLimitTicks As Integer = 5000  'TODO: �ݒ肩��擾����H

    '�N���C�A���g�̃��X�g
    Protected oTargetList As LinkedList(Of Target) 'OPT: Dictionary�ɕύX�H

    '�X���b�h
    Protected oThread As Thread

    '���C���E�B���h�E
    Protected oMainForm As Form

    '�e�X���b�h����̏I���v��
    Private _IsQuitRequest As Integer
#End Region

#Region "�v���p�e�B"
    Protected Property IsQuitRequest() As Boolean
        Get
            Return CBool(Thread.VolatileRead(_IsQuitRequest))
        End Get

        Set(ByVal val As Boolean)
            Thread.VolatileWrite(_IsQuitRequest, CInt(val))
        End Set
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal oMainForm As Form)
        Me.oThread = New Thread(AddressOf Me.Task)
        Me.oThread.Name = ThreadName
        Me.oMainForm = oMainForm
        Me.IsQuitRequest = False
    End Sub
#End Region

#Region "�e�X���b�h�p���\�b�h"
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

    'NOTE: ���̃N���X�ɖ�肪�Ȃ�����AQuit()�ōς܂���ׂ��ł���B
    Public Sub Abort()
        oThread.Abort()
    End Sub

    Public ReadOnly Property ThreadState() As ThreadState
        Get
            Return oThread.ThreadState
        End Get
    End Property
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �Ď��X���b�h�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' Recorder�̊Ǘ����s���B
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

            'NOTE: Config.MyMqPath�̃��b�Z�[�W�L���[�́A
            '���̎��_�ŕK�����݂��Ă���O��ł���B���݂��Ă��Ȃ���΁A
            '�V�X�e���Ɉُ킪����́A���̃v���Z�X�͋N������ɏI������ׂ�
            '�ł���B
            oMessageQueue = New MessageQueue(Config.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([String])})

            oTargetList = New LinkedList(Of Target)

            ProcOnManagementReady()

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Not IsQuitRequest
                '���̃v���Z�X����̃��b�Z�[�W���`�F�b�N����B
                Dim oMessage As Message = Nothing
                Try
                    '���莞�ԃ��b�Z�[�W��҂B
                    'NOTE: MessageQueue.Receive()�̃^�C���A�E�g�́A���ۂ�
                    '�����̎��Ԃ��o�߂����ۂł͂Ȃ��A�Ăяo�����_��
                    '�V�X�e�������Ɉ����̎��Ԃ�����������T�����߂���ŁA
                    '�V�X�e��������T�ȏ�ɂȂ����ۂɍs����悤�ł���B
                    '�܂�A�Ăяo���̊ԂɃV�X�e��������1���Ԗ߂����΁A
                    '�Ăяo������߂�̂́A�u�����̎���+1���ԁv�o�ߌ��
                    '�Ȃ��Ă��܂��A���̊Ԃ́A�q�X���b�h�̐����Ď���
                    '�e�v���Z�X�ւ̐����ؖ����s�����Ƃ��ł��Ȃ��Ȃ�B
                    '���C���X���b�h����̏I���v���ɂ������ł��Ȃ��Ȃ�B
                    '�������A���̂��Ƃ����ɂȂ�悤�ȑ傫�Ȏ����␳��
                    '�s���邱�Ƃ͂Ȃ��Ƃ����O��ŁA�����ő҂��Ƃ�
                    '���Ă���B���ۂɑ傫�Ȏ����␳������Ȃ璍�ӁB
                    oMessage = oMessageQueue.Receive(fewSpan)
                Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                    '�^�C���A�E�g�̏ꍇ�ł���B���̗�O�ɂ��Ă͈���Ԃ��āA
                    'oMessage Is Nothing�̂܂܁A�ȉ������s����B
                End Try

                If oMessage IsNot Nothing Then
                    ProcOnMessageReceive(oMessage)
                End If

                '�ȏ�̏�����AbortRecorder�̑ΏۂɂȂ���Target�ɂ��āA
                'ProcOnRecorderAbort���Ăяo���B
                '���̒���AbortRecorder�̑ΏۂɂȂ���Target�ɂ��ẮA
                '�����ProcOnRecorderAbort���Ăяo���B
                PrepareToRestartRecorders()

                '�O��`�F�b�N���珊�莞�Ԍo�߂��Ă���ꍇ�́A�S�Ă�
                '�o�^�X���b�h�ɂ��āA�ُ�I���܂��̓t���[�Y
                '���Ă��Ȃ����`�F�b�N����B
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()

                    Log.Info("Checking pulse of all Recorders...")
                    For Each oTarget As Target In oTargetList
                        If oTarget.State = TargetState.Started Then
                            If oTarget.Recorder.ThreadState = ThreadState.Stopped Then
                                '�\�����ʗ�O�Ȃǂňُ�I�����Ă���ꍇ�ł���B
                                Log.Fatal("Recorder [" & oTarget.Code.ToString(RecorderNameFormat) & "] has stopped.")
                                AbortRecorder(oTarget)
                            ElseIf TickTimer.GetTickDifference(systemTick, oTarget.Recorder.LastPulseTick) > Config.RecorderPendingLimitTicks Then
                                '�t���[�Y���Ă���ꍇ�ł���B
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
            'NOTE: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j�́A
            '�v���Z�X�}�l�[�W�����s���̂ŁA�����ł͕s�v�ł���B

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        Finally
            If oTargetList IsNot Nothing
                '�S�Ă̓o�^�X���b�h�ɏI����v������B
                'NOTE: �����ł́A�o�^�X���b�h���쐬������A�o�^�X���b�h��
                '�X�^�[�g������O�ɗ�O�����������ꍇ��A
                '�X�^�[�g��̓o�^�X���b�h��Abort���Ă���ꍇ�Ȃ�
                '���l�������������s���Ă���B
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

                '�I����v�������o�^�X���b�h�̏I����҂B
                'NOTE: ���ۂ�Join���s���̂́AQuitRecorder�̑Ώۂ�
                '�Ȃ����X���b�h�i�܂�A�X�^�[�g�ς݂̃X���b�h�j
                '�݂̂ƂȂ邽�߁AThreadStateException����������
                '�\���͂Ȃ����̂Ƃ���B
                WaitForRecordersToQuit()

                '�s�v�ɂȂ����N���C�A���g��o�^��������B
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

                'NOTE: Abort()�̌��ʁAoTarget.Recorder�͗�O���L���b�`���ă��O��
                '�o�͂���\��������B�܂��A�����炪Abort()����߂��Ă������_�ŁA
                '���ɗ�O�������J�n����Ă��邱�Ƃ͍Œ���ۏ؂���Ă��Ăق������A
                'msdn���݂��������Ƃ��܂����s���ł��邽�߁A�X���b�h���I����Ԃ�
                '�Ȃ�Ȃ�����́A�ʐM����Ɋւ��邻�̑��̃O���[�o���ȏ����܂��X�V
                '����\��������ƍl����ׂ��ł���B����āA�ł������I����҂���
                '����A�V����Recorder���X�^�[�g������B
                If oTarget.Recorder.Join(RecorderAbortLimitTicks) = False Then
                    Log.Warn("The Recorder may refuse to abort.")
                End If
            End If
            oTarget.Recorder = Nothing
        End If

        'NOTE: �ċA�Ăяo�����������Ȃ��悤�A������
        'ProcOnRecorderAbort(oTarget)�͍s��Ȃ��B
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

    'NOTE: �����I���A�ċN���A�����I���A�ċN�����Z�������ŌJ��Ԃ����\�����l�����A
    '����́A���Ȑf�f�̎����ŌĂԕ�������ł���B
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
            'OPT: Quit�̎�����A��O����������\�����l�����邱�Ƃ͕K�{�łȂ��B
            '�܂��AoTarget.Recorder.Quit()�ŗ�O����������P�[�X�ł́A
            '���ǁAoTarget.Recorder.Abort()�Ȃǂł���O����������Ǝv����B
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
        '�@��\���}�X�^�ɂ���u�@�킪�Ď��Ղ܂��͑����v���u�J�n����sServiceDate�ȑO�v��
        '���R�[�h�̉w�R�[�h���擾����B

        'NOTE: �^�ǃT�[�o�ɑ΂��闘�p�f�[�^�̑��M���ƂȂ�i�̐����ŏ��ƂȂ�j�@��Ƃ���
        '���ƂŁAW��Y��Ώۂɂ��Ă��邪�A�Ď��Ղ������̉w��S������\��������Ȃ�A
        'W��G�ɕύX����ׂ���������Ȃ��B�������A��������K�v������d�l�Ȃ�A�����
        '�Ď��Ղ����M�������p�f�[�^�ł����Ă��A���D�@�̐ݒu�w���݂āA�o�^���
        '�e�[�u����I�ԕK�v������킯�ŁA�����ȊO�ɂ��݌v�̕ύX���K�v�ɂȂ�B
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

#Region "�C�x���g�������\�b�h"
    Protected Overridable Sub ProcOnManagementReady()
        '�N���C�A���g��o�^����B
        'NOTE: �N�����Ȃ̂ŁA������A�����^�C���ȗ�O�����������ꍇ�́A
        '�v���Z�X�I���Ƃ���B
        Dim serviceStations As DataRowCollection = SelectStationsInService(EkServiceDate.GenString()).Rows
        For Each serviceStation As DataRow In serviceStations
            Dim code As EkCode
            code.RailSection = Integer.Parse(serviceStation.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(serviceStation.Field(Of String)("STATION_ORDER_CODE"))
            RegisterTarget(code)
        Next serviceStation

        '�S�N���C�A���g�̓d������M�X���b�h���J�n����B
        For Each oTarget As Target In oTargetList
            StartRecorder(oTarget)
        Next oTarget
    End Sub

    Protected Overridable Sub ProcOnRecorderAbort(ByVal oTarget As Target)
        'NOTE: ���ɒ�~���čċN���҂��̏�ԁiTargetState.WaitingForRestart�j��
        'oTarget�ɂ��ẮA����ɑ΂��郁�b�Z�[�W���M�����݂��ہA
        '�ēx���̃��\�b�h���Ăяo�����悤�ɂȂ��Ă���B
        '���̎d�l�́ATelServerAppListener�̎����𗬗p���Ă��邱�ƂɋN�����Ă���A
        '���p�f�[�^�o�^�v���Z�X��Watcher�ɂƂ��ẮA���ɈӖ�������킯�ł͂Ȃ��B

        '���W�f�[�^��L�e�[�u���Ɉُ��o�^����B
        'NOTE: ��L�̎d�l�䂦�A�ċN���҂���oTarget�ɑ΂��郁�b�Z�[�W���M��
        '����΁A���x�ł����������s����邱�ƂɂȂ邪�A���x�o�^���Ă��A
        '���ɖ��Ȃ��͂��ł��邽�߁A��Ԃ̊Ǘ��͍s�킸�A��������
        '�o�^���s�����Ƃɂ��Ă���B
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
        '�@��\���}�X�^����A���݂̉^�p���t�ŉ^�p�����ׂ��S�Ẳw����������B
        Dim oServiceStationTable As DataTable
        Try
            oServiceStationTable = SelectStationsInService(EkServiceDate.GenString())
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            '���[�U���C�t���ꏊ�Ɉُ���L�^����B
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtMachineMasterErratumDetected.Gen())
            Return
        End Try

        Dim serviceStations As EnumerableRowCollection(Of DataRow) = oServiceStationTable.AsEnumerable()

        '���ɓo�^���Ă���w�Ɋւ��āA�����̌��ʂɊ܂܂�Ă��Ȃ��ꍇ�́A
        '���Y�w�p�̓o�^�X���b�h�ɏI����v������B
        For Each oTarget As Target In oTargetList
            Dim code As EkCode = oTarget.Code
            Dim num As Integer = ( _
               From serviceStation In serviceStations _
               Where serviceStation.Field(Of String)("RAIL_SECTION_CODE") = code.RailSection.ToString("D3") And _
                     serviceStation.Field(Of String)("STATION_ORDER_CODE") = code.StationOrder.ToString("D3") _
               Select serviceStation _
            ).Count

            If num = 0 Then
                'NOTE: �w���p�~�ɂȂ����ꍇ�A���Y�^�p���܂ł̑S�Ă̗��p�f�[�^�̓o�^��
                '�^�p���t�̐؂�ւ������܂ł̊Ԃɍς�ł���z��ł���B
                QuitRecorder(oTarget)
            End If
        Next oTarget

        '�I����҂B
        WaitForRecordersToQuit()

        '�o�^��������B
        UnregisterDiscardedTargets()

        '�����œ����w�Ɋւ��āA�o�^����Ă��Ȃ����̂́A�o�^����B
        For Each row As DataRow In oServiceStationTable.Rows
            Dim code As EkCode
            code.RailSection = Integer.Parse(row.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(row.Field(Of String)("STATION_ORDER_CODE"))
            Dim oTarget As Target = FindTarget(code)
            If oTarget Is Nothing Then
                RegisterTarget(code)
            End If
        Next row

        '�o�^�����w�̓o�^�X���b�h���J�n������B
        For Each oTarget As Target In oTargetList
            If oTarget.State = TargetState.Registered Then
                StartRecorder(oTarget)
            End If
        Next oTarget
    End Sub
#End Region

End Class
