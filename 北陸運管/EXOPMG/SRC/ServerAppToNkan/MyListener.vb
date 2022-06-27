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
Imports System.Linq
Imports System.Messaging
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �΂m�ԒʐM�v���Z�X�̃N���C�A���g�Ǘ��N���X�B
''' </summary>
Public Class MyListener

#Region "�����N���X��"
    Protected Enum ClientState
        Registered
        Started
        Aborted
        WaitingForRestart
        QuitRequested
        Discarded
    End Enum

    Protected Class Client
        Public State As ClientState
        Public Code As EkCode
        Public Telegrapher As MyTelegrapher
        Public ChildSteerSock As Socket
        Public ListenerSock As Socket
    End Class
#End Region

#Region "�萔��ϐ�"
    '�X���b�h��
    Protected Const ThreadName As String = "Listener"

    '�N���C�A���g���o�͏���
    Protected Const EkCodeOupFormat As String = "%3R%3S"

    '�d������M�X���b�h��Abort��������
    Protected Const TelegrapherAbortLimitTicks As Integer = 5000  'TODO: �ݒ肩��擾����H

    '�d����荞�݊�
    Protected oTelegImporter As NkTelegramImporter

    '�N���C�A���g�̃��X�g
    Protected oClientList As LinkedList(Of Client) 'OPT: Dictionary�ɕύX�H

    '�X���b�h
    Private oThread As Thread

    '�e�X���b�h����̏I���v��
    Private _IsQuitRequest As Integer
#End Region

#Region "�v���p�e�B"
    Private Property IsQuitRequest() As Boolean
        Get
            Return CBool(Thread.VolatileRead(_IsQuitRequest))
        End Get

        Set(ByVal val As Boolean)
            Thread.VolatileWrite(_IsQuitRequest, CInt(val))
        End Set
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New()
        Me.oTelegImporter = New NkTelegramImporter()
        Me.oThread = New Thread(AddressOf Me.Task)
        Me.oThread.Name = ThreadName
        Me.IsQuitRequest = False
    End Sub
#End Region

#Region "�e�X���b�h�p���\�b�h"
    Public Overridable Sub Start()
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
    ''' �ʐM�Ǘ��X���b�h�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' ���X�j���O�\�P�b�g�̐��䂨���Telegrapher�̊Ǘ����s���B
    ''' </remarks>
    Private Sub Task()
        Dim oMessageQueue As MessageQueue = Nothing
        Try
            Log.Info("The listener thread started.")

            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            Dim fewSpan As New TimeSpan(0, 0, 0, 0, Config.PollIntervalTicks)
            Dim oCheckReadList As New ArrayList()
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

            oClientList = New LinkedList(Of Client)

            ProcOnManagementReady()

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Not IsQuitRequest
                '�d������M�X���b�h����̃��b�Z�[�W���`�F�b�N����B
                'NOTE: �΂m�Ԃ̓d������M�X���b�h�͐e�X���b�h�Ƀ��b�Z�[�W�𑗐M
                '���Ȃ����߁A�قƂ�ǖ��Ӗ��ł��邪�A�\�P�b�g���N���[�Y��������
                '�����o�ł���Ƃ����_�ňӖ������邽�߁A�c���Ă���B
                oCheckReadList.Clear()
                For Each oClient As Client In oClientList
                    If oClient.State = ClientState.Started Then
                        oCheckReadList.Add(oClient.ChildSteerSock)
                    End If
                Next oClient
                If oCheckReadList.Count <> 0 Then
                    '�\�P�b�g���ǂݏo���\���`�F�b�N����B
                    Socket.Select(oCheckReadList, Nothing, Nothing, 0)

                    '�ǂݏo���\�ł���ꍇ�́A���b�Z�[�W��ǂݏo���B
                    If oCheckReadList.Count > 0 Then
                        Dim oReadableSock As Socket = DirectCast(oCheckReadList(0), Socket)
                        Dim oClient As Client = FindClient(oReadableSock)
                        ProcOnChildSteerSockReadable(oClient)
                    End If
                End If

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

                '�ȏ�̏�����AbortTelegrapher�̑ΏۂɂȂ���Client�ɂ��āA
                'ProcOnTelegrapherAbort���Ăяo���B
                '���̒���AbortTelegrapher�̑ΏۂɂȂ���Client�ɂ��ẮA
                '�����ProcOnTelegrapherAbort���Ăяo���B
                PrepareToRestartTelegraphers()

                '�O��`�F�b�N���珊�莞�Ԍo�߂��Ă���ꍇ�́A�S�Ă�
                '�d������M�X���b�h�ɂ��āA�ُ�I���܂��̓t���[�Y
                '���Ă��Ȃ����`�F�b�N����B
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()

                    Log.Info("Checking pulse of all telegraphers...")
                    For Each oClient As Client In oClientList
                        If oClient.State = ClientState.Started Then
                            If oClient.Telegrapher.ThreadState = ThreadState.Stopped Then
                                '�\�����ʗ�O�Ȃǂňُ�I�����Ă���ꍇ�ł���B
                                Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] has stopped.")
                                AbortTelegrapher(oClient)
                            ElseIf TickTimer.GetTickDifference(systemTick, oClient.Telegrapher.LastPulseTick) > Config.TelegrapherPendingLimitTicks Then
                                '�t���[�Y���Ă���ꍇ�ł���B
                                Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
                                AbortTelegrapher(oClient)
                            End If
                        End If
                    Next oClient
                    PrepareToRestartTelegraphers()
                    RestartTelegraphers()
                End If
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j�́A
            '�v���Z�X�}�l�[�W�����s���̂ŁA�����ł͕s�v�ł���B

            MainClass.oMainForm.Invoke(New MethodInvoker(AddressOf MainClass.oMainForm.Close))
        Finally
            If oClientList IsNot Nothing
                '�S�N���C�A���g�̓d������M�X���b�h�ɏI����v������B
                'NOTE: �����ł́A�Γd������M�X���b�h�ʐM�p�\�P�b�g��
                '�d������M�X���b�h���쐬������A�d������M�X���b�h��
                '�X�^�[�g������O�ɗ�O�����������ꍇ��A
                '�X�^�[�g��̓d������M�X���b�h��Abort���Ă���ꍇ�Ȃ�
                '���l�������������s���Ă���B
                For Each oClient As Client In oClientList
                    If oClient.ChildSteerSock IsNot Nothing AndAlso _
                       (oClient.State = ClientState.Started OrElse _
                       oClient.State = ClientState.Aborted OrElse _
                       oClient.State = ClientState.WaitingForRestart) Then
                        QuitTelegrapher(oClient)
                    End If
                Next oClient

                '�I����v�������d������M�X���b�h�̏I����҂B
                'NOTE: ���ۂ�Join���s���̂́AQuitTelegrapher�̑Ώۂ�
                '�Ȃ����X���b�h�i�܂�A�X�^�[�g�ς݂̃X���b�h�j
                '�݂̂ƂȂ邽�߁AThreadStateException����������
                '�\���͂Ȃ����̂Ƃ���B
                WaitForTelegraphersToQuit()

                '�s�v�ɂȂ����N���C�A���g��o�^��������B
                UnregisterDiscardedClients()
            End If

            If oMessageQueue IsNot Nothing Then
                oMessageQueue.Close()
            End If
        End Try
    End Sub

    Private Function FindClient(ByVal oSocket As Socket) As Client
        For Each oClient As Client In oClientList
            If oClient.ChildSteerSock Is oSocket Then Return oClient
        Next oClient
        Return Nothing 'NOTE: ���蓾�Ȃ��ƍl���Ă悢�B
    End Function

    Protected Function FindClient(ByVal code As EkCode) As Client
        For Each oClient As Client In oClientList
            If oClient.Code = code Then Return oClient
        Next oClient
        Return Nothing
    End Function

    Protected Sub RegisterClient(ByVal code As EkCode, ByVal port As Integer)
        '���b�X�����J�n����B
        Log.Info("Start listening for [" & Config.IpAddrForTelegConnection.ToString() & ":" & port.ToString() & "].")
        Dim oListenerSock As Socket = SockUtil.StartListener(Config.IpAddrForTelegConnection, port)

        Log.Info("Registering telegrapher [" & code.ToString(EkCodeOupFormat) & "]...")
        Dim oParentSock As Socket = Nothing
        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oParentSock, oChildSock)
        Dim oTelegrapher As New MyTelegrapher( _
          code.ToString(EkCodeOupFormat), _
          oChildSock, _
          oTelegImporter, _
          code, _
          oListenerSock)
        Dim oClient As New Client()
        oClient.State = ClientState.Registered
        oClient.Code = code
        oClient.Telegrapher = oTelegrapher
        oClient.ChildSteerSock = oParentSock
        oClient.ListenerSock = oListenerSock
        oClientList.AddLast(oClient)
    End Sub

    Protected Sub StartTelegrapher(ByVal oClient As Client)
        Debug.Assert(oClient.State = ClientState.Registered)

        Log.Info("Starting telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
        oClient.Telegrapher.Start()
        oClient.State = ClientState.Started
    End Sub

    'NOTE: �d������M�X���b�h�������I���������i�d������M�X���b�h�ƒʐM���s��
    '�\�P�b�g�����݂��Ȃ��jClient�Ɋւ��Ă��Ăяo���\�ł���B
    '���̏ꍇ�A����ProcOnTelegrapherAbort���Ă΂�Ă��Ă��A
    '���̃��b�Z�[�W���M�Ɍ��������������s����悤�ɁA
    '�ēxProcOnTelegrapherAbort���ĂԂ悤�ɂȂ��Ă���B
    Protected Function SendToTelegrapher(ByVal oClient As Client, ByVal oMsg As InternalMessage) As Boolean
        Debug.Assert(oClient.State <> ClientState.Registered)
        Debug.Assert(oClient.State <> ClientState.QuitRequested)
        Debug.Assert(oClient.State <> ClientState.Discarded)

        If oClient.State <> ClientState.Started Then
            If oClient.State <> ClientState.WaitingForRestart Then
                Log.Warn("The telegrapher is already marked as broken.")
                Return False
            Else
                Log.Warn("The telegrapher is waiting for restart.")
                AbortTelegrapher(oClient)
                Return False
            End If
        End If

        If oMsg.WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            AbortTelegrapher(oClient)
            Return False
        Else
            Return True
        End If
    End Function

    Protected Sub AbortTelegrapher(ByVal oClient As Client)
        Debug.Assert(oClient.State <> ClientState.Registered)
        Debug.Assert(oClient.State <> ClientState.QuitRequested)
        Debug.Assert(oClient.State <> ClientState.Discarded)

        If oClient.State <> ClientState.Started AndAlso
           oClient.State <> ClientState.WaitingForRestart Then
            Log.Warn("The telegrapher is already marked as broken.")
            Return
        End If

        If oClient.State = ClientState.Started Then
            oClient.ChildSteerSock.Close()
            oClient.ChildSteerSock = Nothing

            If oClient.Telegrapher.ThreadState <> ThreadState.Stopped Then
                oClient.Telegrapher.Abort()

                'NOTE: Abort()�̌��ʁAoClient.Telegrapher�͗�O���L���b�`���ă��O��
                '�o�͂���\��������B�܂��A�����炪Abort()����߂��Ă������_�ŁA
                '���ɗ�O�������J�n����Ă��邱�Ƃ͍Œ���ۏ؂���Ă��Ăق������A
                'msdn���݂��������Ƃ��܂����s���ł��邽�߁A�X���b�h���I����Ԃ�
                '�Ȃ�Ȃ�����́A�ʐM����Ɋւ��邻�̑��̃O���[�o���ȏ����܂��X�V
                '����\��������ƍl����ׂ��ł���B����āA�ł������I����҂���
                '����A�V����Telegrapher���X�^�[�g������B
                If oClient.Telegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                    Log.Warn("The telegrapher may refuse to abort.")
                End If
            End If
            oClient.Telegrapher = Nothing
        End If

        'NOTE: �ċA�Ăяo�����������Ȃ��悤�A������
        'ProcOnTelegrapherAbort(oClient)�͍s��Ȃ��B
        oClient.State = ClientState.Aborted
    End Sub

    Protected Sub PrepareToRestartTelegraphers()
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Aborted Then
                ProcOnTelegrapherAbort(oClient)
                oClient.State = ClientState.WaitingForRestart
            End If
        Next oClient
    End Sub

    'NOTE: �����I���A�ċN���A�����I���A�ċN�����Z�������ŌJ��Ԃ����\�����l�����A
    '����́A���Ȑf�f�̎����ŌĂԕ�������ł���B
    Protected Sub RestartTelegraphers()
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.WaitingForRestart Then
                Log.Info("Renewing telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
                Dim oChildSock As Socket = Nothing
                LocalConnectionProvider.CreateSockets(oClient.ChildSteerSock, oChildSock)
                oClient.Telegrapher = New MyTelegrapher( _
                   oClient.Code.ToString(EkCodeOupFormat), _
                   oChildSock, _
                   oTelegImporter, _
                   oClient.Code, _
                   oClient.ListenerSock)

                Log.Info("Restarting telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
                oClient.Telegrapher.Start()
                oClient.State = ClientState.Started

                ProcOnTelegrapherRestart(oClient)
            End If
        Next oClient
    End Sub

    Protected Sub QuitTelegrapher(ByVal oClient As Client)
        Debug.Assert(oClient.State <> ClientState.Registered)
        Debug.Assert(oClient.State <> ClientState.QuitRequested)
        Debug.Assert(oClient.State <> ClientState.Discarded)

        If oClient.State <> ClientState.Started Then
            Log.Warn("The telegrapher is already marked as broken.")
            If oClient.State = ClientState.Aborted Then
                ProcOnTelegrapherAbort(oClient)
            End If
            oClient.State = ClientState.Discarded
            Return
        End If

        Log.Info("Sending quit request to telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
        If QuitRequest.Gen().WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            oClient.ChildSteerSock.Close()
            If oClient.Telegrapher.ThreadState <> ThreadState.Stopped Then
                oClient.Telegrapher.Abort()
                If oClient.Telegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                    Log.Warn("The telegrapher may refuse to abort.")
                End If
            End If
            oClient.State = ClientState.Discarded
        Else
            oClient.State = ClientState.QuitRequested
        End If
    End Sub

    Protected Sub WaitForTelegraphersToQuit()
        Dim oJoinLimitTimer As New TickTimer(Config.TelegrapherPendingLimitTicks)
        oJoinLimitTimer.Start(TickTimer.GetSystemTick())
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.QuitRequested Then
                Dim ticks As Long = oJoinLimitTimer.GetTicksToTimeout(TickTimer.GetSystemTick())
                If ticks < 0 Then ticks = 0

                If oClient.Telegrapher.Join(CInt(ticks)) = False Then
                    Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
                    oClient.ChildSteerSock.Close()
                    oClient.Telegrapher.Abort()
                    If oClient.Telegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                        Log.Warn("The telegrapher may refuse to abort.")
                    End If
                Else
                    Log.Info("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] has quit.")
                    oClient.ChildSteerSock.Close()
                End If
                oClient.State = ClientState.Discarded
            End If
        Next oClient
    End Sub

    Protected Sub UnregisterDiscardedClients()
        Dim oNode As LinkedListNode(Of Client) = oClientList.First
        While oNode IsNot Nothing
            Dim oClient As Client = oNode.Value
            If oClient.State = ClientState.Discarded Then
                Dim oDiscardedNode As LinkedListNode(Of Client) = oNode
                oNode = oNode.Next
                oClientList.Remove(oDiscardedNode)
                Log.Info("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] unregistered.")

                Dim port As Integer = DirectCast(oClient.ListenerSock.LocalEndPoint, IPEndPoint).Port
                oClient.ListenerSock.Close()
                Log.Info("End listening for [" & Config.IpAddrForTelegConnection.ToString() & ":" & port.ToString() & "].")
            Else
                oNode = oNode.Next
            End If
        End While
    End Sub

    Protected Overridable Function SelectStationsInService(ByVal sServiceDate As String) As DataTable
        '�@��\���}�X�^�ɂ���u�@�킪�Ď��Ղ܂��͑����v���u�J�n����sServiceDate�ȑO�v��
        '���R�[�h�́A�u�w�R�[�h�v�Ɓu�m�ԃT�[�o�p�|�[�g�ԍ��v���擾����B
        'NOTE: �@��\���}�X�^�Ɂu�w�R�[�h�v������Łu�m�ԃT�[�o�p�|�[�g�ԍ��v���قȂ�����A
        '�u�m�ԃT�[�o�p�|�[�g�ԍ��v������Łu�w�R�[�h�v���قȂ����肷�郌�R�[�h��
        '���݂��Ȃ����̂Ƃ���B

        Dim sSQL As String = _
           "SELECT DISTINCT RAIL_SECTION_CODE, STATION_ORDER_CODE, NK_PORT_NO" _
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
            Dim port As Integer = serviceStation.Field(Of Integer)("NK_PORT_NO")
            RegisterClient(code, port)
        Next serviceStation

        '�S�N���C�A���g�̓d������M�X���b�h���J�n����B
        For Each oClient As Client In oClientList
            StartTelegrapher(oClient)
        Next oClient
    End Sub

    Protected Overridable Sub ProcOnTelegrapherAbort(ByVal oClient As Client)
        'NOTE: ���ɒ�~���čċN���҂��̏�ԁiClientState.WaitingForRestart�j��
        'oClient�ɂ��ẮA����ɑ΂��郁�b�Z�[�W���M�����݂��ہA
        '�ēx���̃��\�b�h���Ăяo�����悤�ɂȂ��Ă���B
        '���̎d�l�́ATelServerAppListener�̎����𗬗p���Ă��邱�ƂɋN�����Ă���A
        '�΂m�ԒʐM�v���Z�X�p��Listener�ɂƂ��ẮA���ɈӖ�������킯�ł͂Ȃ��B

        '���W�f�[�^��L�e�[�u���Ɉُ��o�^����B
        'NOTE: ��L�̎d�l�䂦�A�ċN���҂���oClient�ɑ΂��郁�b�Z�[�W���M��
        '����΁A���x�ł����������s����邱�ƂɂȂ邪�A���x�o�^���Ă��A
        '���ɖ��Ȃ��͂��ł��邽�߁A��Ԃ̊Ǘ��͍s�킸�A��������
        '�o�^���s�����Ƃɂ��Ă���B
        Using curProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess()
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtThreadAbended.Gen(curProcess.ProcessName, oClient.Code.ToString(EkCodeOupFormat)))
        End Using
    End Sub

    Protected Overridable Sub ProcOnTelegrapherRestart(ByVal oClient As Client)
    End Sub

    Protected Overridable Sub ProcOnChildSteerSockReadable(ByVal oClient As Client)
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks)
        If Not oRcvMsg.HasValue Then
            Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
            AbortTelegrapher(oClient)
            Return
        End If

        ProcOnChildMessageReceive(oClient, oRcvMsg)
    End Sub

    Protected Overridable Sub ProcOnChildMessageReceive(ByVal oClient As Client, ByVal oRcvMsg As InternalMessage)
    End Sub

    Protected Overridable Sub ProcOnMessageReceive(ByVal oMessage As Message)
        Select Case oMessage.AppSpecific
            Case ExtServiceDateChangeNotice.FormalKind
                Log.Info("ExtServiceDateChangeNotice received.")
                ProcOnServiceDateChangeNoticeReceive(oMessage)
            Case ExtTallyTimeNotice.FormalKind
                Log.Info("ExtTallyTimeNotice received.")
                ProcOnTallyTimeNoticeReceive(oMessage)
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
        '���Y�w�p�̓d������M�X���b�h�ɏI����v������B
        For Each oClient As Client In oClientList
            Dim code As EkCode = oClient.Code
            Dim port As Integer = DirectCast(oClient.ListenerSock.LocalEndPoint, IPEndPoint).Port
            Dim num As Integer = ( _
               From serviceStation In serviceStations _
               Where serviceStation.Field(Of String)("RAIL_SECTION_CODE") = code.RailSection.ToString("D3") And _
                     serviceStation.Field(Of String)("STATION_ORDER_CODE") = code.StationOrder.ToString("D3") And _
                     serviceStation.Field(Of Integer)("NK_PORT_NO") = port _
               Select serviceStation _
            ).Count

            If num = 0 Then
                'NOTE: �z�M������W���̏ꍇ�A�{�����M���錠�������̂�
                'Telegrapher���ł���B���̂��Ƃ��l�������Telegrapher��
                'QuitRequest��M��p�\�P�b�g��p�ӂ���������R�ł���B
                QuitTelegrapher(oClient)
            End If
        Next oClient

        '�I����҂B
        WaitForTelegraphersToQuit()

        '�o�^��������B
        UnregisterDiscardedClients()

        '�����œ����w�Ɋւ��āA�o�^����Ă��Ȃ����̂́A�o�^����B
        For Each row As DataRow In oServiceStationTable.Rows
            Dim code As EkCode
            code.RailSection = Integer.Parse(row.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(row.Field(Of String)("STATION_ORDER_CODE"))
            Dim port As Integer = row.Field(Of Integer)("NK_PORT_NO")
            Dim oClient As Client = FindClient(code)
            If oClient Is Nothing Then
                RegisterClient(code, port)
            Else If DirectCast(oClient.ListenerSock.LocalEndPoint, IPEndPoint).Port <> port
                'Code��������Port�ԍ����قȂ�N���C�A���g�����ɓo�^����Ă���ꍇ�ł���B
                Log.Error("����̉w[" & code.ToString(EkCodeOupFormat) & "]���قȂ�|�[�g�ԍ��œo�^����Ă��܂��B")

                '���[�U���C�t���ꏊ�Ɉُ���L�^����B
                CollectedDataTypoRecorder.Record( _
                   New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                   DbConstants.CdtKindServerError, _
                   Lexis.CdtMachineMasterErratumDetected.Gen())
            End If
        Next row

        '�o�^�����w�̓d������M�X���b�h���J�n������B
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Registered Then
                StartTelegrapher(oClient)
            End If
        Next oClient
    End Sub

    Protected Overridable Sub ProcOnTallyTimeNoticeReceive(ByVal oMessage As Message)
        For Each oClient As Client In oClientList
            SendToTelegrapher(oClient, TallyTimeNotice.Gen())
        Next oClient
    End Sub
#End Region

End Class
