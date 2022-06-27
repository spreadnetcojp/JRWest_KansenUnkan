' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
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
''' �ʐM�v���Z�X���ʂ̃N���C�A���g�Ǘ��N���X�B
''' </summary>
Public MustInherit Class TelServerAppListener
#Region "�����N���X��"
    Protected Enum ClientState
        Registered
        Started
        Aborted
        WaitingForRestart
        QuitRequested
        Discarded
    End Enum

    Protected Enum ClientActiveXllState
        None
        Waiting
        Running
    End Enum

    Protected Class Client
        Public State As ClientState
        Public Code As EkCode
        '-------Ver0.1 ������ԕ�Ή� ADD START-----------
        Public StationName As String
        Public CornerName As String
        '-------Ver0.1 ������ԕ�Ή� ADD END-------------
        Public Telegrapher As ServerTelegrapher
        Public ChildSteerSock As Socket

        Public MasProDllState As ClientActiveXllState
        Public SendSuiteOnMasProDll As Boolean
        Public ScheduledUllState As ClientActiveXllState
        Public Sub New()
            MasProDllState = ClientActiveXllState.None
            ScheduledUllState = ClientActiveXllState.None
        End Sub
    End Class

    Protected Class MasProDllInfo
        Public DataApplicableModel As String 'W�܂���G�܂���Y
        Public DataPurpose As String 'MST�܂���PRG
        Public DataKind As String 'DSH��WPG
        Public DataSubKind As String '�p�^�[���ԍ��܂��̓G���A�ԍ�
        Public DataVersion As String
        Public DataFileName As String
        Public DataFileHashValue As String
        Public ListVersion As String
        Public ListFileName As String
        Public ListFileHashValue As String
        Public RemainingCount As Integer 'Waiting��Running�̍��v����
        Public WaitingClients As List(Of Client)
    End Class

    Protected Class ScheduledUllInfo
        Public FileName As String
        Public RemainingCount As Integer 'Waiting��Running�̍��v����
        Public WaitingClients As List(Of Client)
    End Class
#End Region

#Region "�萔��ϐ�"
    '�e��e�[�u�����ʂ̍��ڂɃZ�b�g����l
    Protected Const UserId As String = "System"
    Protected Const MachineId As String = "Server"

    '�X���b�h��
    Protected Const ThreadName As String = "Listener"

    '�N���C�A���g���o�͏���
    Protected Const EkCodeOupFormat As String = "%3R%3S_%4C_%2U"

    '�d������M�X���b�h��Abort��������
    'NOTE: �������̔h���N���X�̓d������M�X���b�h�́AAbort�̍ۂ�
    'DLL��ԃe�[�u����߂��K�v�����邽�߁A���߂ɂ��Ă����̂�����ł���B
    Protected Const TelegrapherAbortLimitTicks As Integer = 10000  'TODO: �ݒ肩��擾����H

    '�N���C�A���g�̃��X�g
    Protected oClientList As LinkedList(Of Client) 'OPT: Dictionary�ɕύX�H

    '�X���b�h
    Private oThread As Thread

    '�e�X���b�h����̏I���v��
    Private _IsQuitRequest As Integer

    '�ʐM����́i�v���g�R���d�l�j�@��R�[�h
    Protected clientModelInProtocol As Integer

    '�ʐM����́iDB�d�l�j�@��R�[�h
    Protected sClientModel As String

    '�ʐM����́iDB�d�l�j�R�l�N�V�����敪
    Protected sPortPurpose As String

    '�}�X�^/�v���O������DLL��S�����邩�ۂ�
    Protected handlesMasProDll As Boolean

    '�z�M�����̃L���[
    Protected oMasProDllQueue As Queue(Of ExtMasProDllRequest)

    '���ݎ��s���̔z�M����
    Protected oCurMasProDll As MasProDllInfo

    '���W�����̃L���[
    Protected oScheduledUllQueue As Queue(Of ExtScheduledUllRequest)

    '���ݎ��s���̎��W����
    Protected oCurScheduledUll As ScheduledUllInfo

    '���W�f�[�^��L�e�[�u���ɋL�^���邽�߂̒ʐM����@�햼�́i�h���N���X�ŕK���ݒ肷��j
    Protected sCdtClientModelName As String

    '���W�f�[�^��L�e�[�u���ɋL�^����|�[�g���́i�h���N���X�ŕK���ݒ肷��j
    Protected sCdtPortName As String
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
    Public Sub New(ByVal clientModelInProtocol As Integer, ByVal sClientModel As String, ByVal sPortPurpose As String, ByVal handlesMasProDll As Boolean)
        Me.clientModelInProtocol = clientModelInProtocol
        Me.sClientModel = sClientModel
        Me.sPortPurpose = sPortPurpose
        Me.handlesMasProDll = handlesMasProDll

        Me.oThread = New Thread(AddressOf Me.Task)
        Me.oThread.Name = ThreadName
        Me.IsQuitRequest = False

        Me.oMasProDllQueue = New Queue(Of ExtMasProDllRequest)
        Me.oCurMasProDll = Nothing

        Me.oScheduledUllQueue = New Queue(Of ExtScheduledUllRequest)
        Me.oCurScheduledUll = Nothing
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
        Dim oListenerSock As Socket = Nothing  '���X�j���O�\�P�b�g
        Try
            Log.Info("The listener thread started.")
            Dim oDiagnosisTimer As New TickTimer(TelServerAppBaseConfig.SelfDiagnosisIntervalTicks)
            Dim oCheckReadList As New ArrayList()

            '�e�d������M�X���b�h�̈ꎞ��Ɨp�f�B���N�g����e�f�B���N�g�����Ƃ܂Ƃ߂č폜����B
            Log.Info("Sweeping directory [" & TelServerAppBaseConfig.TemporaryBaseDirPath & "]...")
            Utility.DeleteTemporalDirectory(TelServerAppBaseConfig.TemporaryBaseDirPath)

            '�e�d������M�X���b�h��FTP�T�C�g�p�f�B���N�g���₻�̓��e�����폜����B
            'NOTE: ���̃f�B���N�g���́A���̃v���Z�X�����łȂ��AFTP�T�[�o���Q�ƁE���삵����B
            '���ɑ��݂��Ă�����̂̍폜�Ɏ��s����ꍇ�́A�]���̏I����F�����Ă��Ȃ�FTP�T�[�o��
            '�������݂ň����Ă���P�[�X�ƍl�����邪�A�Y������T�u�f�B���N�g����t�@�C���݂̂�
            '�c���ď��������s����B�Ȃ��A���������s�����ɂ��̃v���Z�X���ُ�I��������Ƃ��Ă��A
            '�v���Z�X�}�l�[�W�������̃v���Z�X���N�����鎟�̋@���FTP�T�[�o���K�[�h��������
            '����΁A�������琳�퓮�삪�n�܂邽�߁A���͂Ȃ��͂��ł���B�A�v���ċN����
            '���̋@��𓦂����ɁA�S�Ă̈ꎞ�t�@�C�����폜����Ƃ����Ӗ��ł́A���̕������z�I��
            '���邩������Ȃ����A�Ƃ肠�����p����D�悵�āA���̂悤�ɂ��Ă���B
            Dim sFtpBase As String = Utility.CombinePathWithVirtualPath(TelServerAppBaseConfig.FtpServerRootDirPath, TelServerAppBaseConfig.PermittedPathInFtp)
            If Directory.Exists(sFtpBase) Then
                Log.Info("Cleaning up directory [" & sFtpBase & "]...")
                Utility.CleanUpDirectory(sFtpBase)
            End If

            Dim oFilter As New MessagePropertyFilter()
            oFilter.ClearAll()
            oFilter.AppSpecific = True
            oFilter.Body = True

            'NOTE: TelServerAppBaseConfig.MyMqPath�̃��b�Z�[�W�L���[�́A
            '���̎��_�ŕK�����݂��Ă���O��ł���B���݂��Ă��Ȃ���΁A
            '�V�X�e���Ɉُ킪����́A���̃v���Z�X�͋N������ɏI������ׂ�
            '�ł���B
            oMessageQueue = New MessageQueue(TelServerAppBaseConfig.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([String])})

            '���ɃL���[�C���O����Ă��郁�b�Z�[�W��S�ēǂݎ̂Ă�B
            'NOTE: ���̌�ŁADLL��ԃe�[�u���́u�z�M���v�ɂȂ��Ă���
            '���R�[�h�i���b�Z�[�W�Ŕz�M��v������Ă����S�Ẵ��R�[�h�j
            '���u�ُ�v�ɂ��A�@��\���̓ǂݍ��݂������I�ɍs�����߁A
            '�����Ŏ̂Ă邱�Ƃɂ͑S�����Ȃ��B
            oMessageQueue.Purge()

            If handlesMasProDll Then
                '�}�X�^DLL��ԃe�[�u������уv���O����DLL��ԃe�[�u���ɂ��āA
                '�u�z�M���v�ɂȂ��Ă��铖�Y��ʂ̑S���R�[�h���u�ُ�v�ɕύX�B
                'NOTE: �ύX�ł��Ȃ��ꍇ�́A�V�X�e���Ɉُ킪����́A
                '���̃v���Z�X�͋N������ɏI������ׂ��ł���B
                TransitDllStatusToAbnormal(EkConstants.DataPurposeMaster)
                TransitDllStatusToAbnormal(EkConstants.DataPurposeProgram)
            End If

            oClientList = New LinkedList(Of Client)
            ProcOnManagementReady()

            '���b�X�����J�n����B
            Log.Info("Start listening for [" & TelServerAppBaseConfig.IpAddrForTelegConnection.ToString() & ":" & TelServerAppBaseConfig.IpPortForTelegConnection.ToString() & "].")
            oListenerSock = SockUtil.StartListener(TelServerAppBaseConfig.IpAddrForTelegConnection, TelServerAppBaseConfig.IpPortForTelegConnection)

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Not IsQuitRequest
                '�d������M�X���b�h����̃��b�Z�[�W����сA�V���ȋ@�킩���
                '�ڑ��v�����`�F�b�N����B
                Dim oNewSocket As Socket = Nothing

                oCheckReadList.Clear()
                oCheckReadList.Add(oListenerSock)
                For Each oClient As Client In oClientList
                    If oClient.State = ClientState.Started AndAlso IsWaitingForChildMessage(oClient) Then
                        oCheckReadList.Add(oClient.ChildSteerSock)
                    End If
                Next oClient

                '�\�P�b�g���ǂݏo���\�ɂȂ�܂ŏ��莞�ԑҋ@����B
                Socket.Select(oCheckReadList, Nothing, Nothing, TelServerAppBaseConfig.PollIntervalTicks * 1000)

                If oCheckReadList.Count > 0 Then
                    Dim oReadableSock As Socket = DirectCast(oCheckReadList(0), Socket)
                    If oReadableSock Is oListenerSock Then
                        '���X�j���O�\�P�b�g���ǂݏo���\�ɂȂ����ꍇ�́A
                        '����M�p�\�P�b�g�����o���B
                        Try
                            oNewSocket = SockUtil.Accept(oListenerSock)
                        Catch ex As OPMGException
                            'NOTE: ���ۂ̂Ƃ���͂Ƃ������A���X�j���O�\�P�b�g���ǂݏo���\
                            '�ɂȂ�������Ƃ����āA���������Accept()����������Ƃ͌���Ȃ�
                            '�ilinux�̃\�P�b�g�̂悤�ɁAAccept()���Ăяo���܂ł̊Ԃɔ���
                            '�����R�l�N�V�����ُ̈킪�AAccept()�Œʒm�����\��������j
                            '���̂Ƃ݂Ȃ��B
                            Log.Error("Exception caught.", ex)
                        End Try
                    Else
                        '�Γd������M�X���b�h�p�\�P�b�g���ǂݏo���\��
                        '�Ȃ����ꍇ�́A���b�Z�[�W��ǂݏo���B
                        Dim oClient As Client = FindClient(oReadableSock)
                        ProcOnChildSteerSockReadable(oClient)
                    End If
                End If

                '����M�p�\�P�b�g���������ꂽ�ꍇ�A�d������M�X���b�h�ɓn���B
                If oNewSocket IsNot Nothing Then
                    Dim oRemoteEndPoint As IPEndPoint = DirectCast(oNewSocket.RemoteEndPoint, IPEndPoint)
                    Dim oRemoteIPAddr As IPAddress = oRemoteEndPoint.Address
                    Log.Info("Incoming from [" & oRemoteEndPoint.Address.ToString() & "].")
                    ProcOnAccept(oNewSocket)
                End If

                '���̃v���Z�X����̃��b�Z�[�W���`�F�b�N����B
                Dim oMessage As Message = Nothing
                Try
                    '���莞�ԃ��b�Z�[�W��҂B
                    oMessage = oMessageQueue.Receive(TimeSpan.Zero)
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
                            ElseIf TickTimer.GetTickDifference(systemTick, oClient.Telegrapher.LastPulseTick) > TelServerAppBaseConfig.TelegrapherPendingLimitTicks Then
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

            'NOTE: �킴�킴Start����MainForm�̎Q�Ƃ��󂯎���Ă܂�
            '������s���K�v���͒Ⴂ���A�������肪�������Ƃ��́A
            '���߂ɃE�B���h�E������������킩��₷���̂ŁA
            '���L���s�����Ƃɂ��Ă���B
            TelServerAppBaseMainClass.oMainForm.Invoke(New MethodInvoker(AddressOf TelServerAppBaseMainClass.oMainForm.Close))
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

            If oListenerSock IsNot Nothing Then
                oListenerSock.Close()
                Log.Info("End listening for [" & TelServerAppBaseConfig.IpAddrForTelegConnection.ToString() & ":" & TelServerAppBaseConfig.IpPortForTelegConnection.ToString() & "].")
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

    '-------Ver0.1 ������ԕ�Ή� MOD START-----------
    Protected Sub RegisterClient(ByVal code As EkCode, ByVal sStationName As String, ByVal sCornerName As String)
        Log.Info("Registering telegrapher [" & code.ToString(EkCodeOupFormat) & "]...")
        Dim oParentSock As Socket = Nothing
        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oParentSock, oChildSock)
        Dim oTelegrapher As ServerTelegrapher = CreateTelegrapher( _
          code.ToString(EkCodeOupFormat), _
          oChildSock, _
          code, _
          sStationName, _
          sCornerName)
        Dim oClient As New Client()
        oClient.State = ClientState.Registered
        oClient.Code = code
        oClient.StationName = sStationName
        oClient.CornerName = sCornerName
        oClient.Telegrapher = oTelegrapher
        oClient.ChildSteerSock = oParentSock
        oClientList.AddLast(oClient)
    End Sub
    '-------Ver0.1 ������ԕ�Ή� MOD END-------------

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

        If oMsg.WriteToSocket(oClient.ChildSteerSock, TelServerAppBaseConfig.TelegrapherPendingLimitTicks) = False Then
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

        'NOTE: �uoClient.State = ClientState.Aborted�v�̏ꍇ�́A���̂܂�
        '�ł�ProcOnTelegrapherAbort(oClient)���Ăяo�����͂��ł��邽�߁A
        '������Ԃ�ύX�����ɁA�{���\�b�h���I������B
        'NOTE: ClientState.WaitingForRestart�̏ꍇ�́A
        'ProcOnTelegrapherAbort(oClient)�͊��Ɏ��s�ς݂ł���B�������A
        '����ȍ~�ɔz�M�w�����s���AoClient��MasProDllRequest��
        '���M���悤�Ƃ��Ă��̃��\�b�h���Ă΂ꂽ�̂ł���΁A
        '�Ă�ProcOnTelegrapherAbort(oClient)�����s���āA
        '�z�M���ʂ�Client�֐؂�ւ������B
        '����āA�����Ŗ{���\�b�h���I�������Ă͂Ȃ�Ȃ��B
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
                '-------Ver0.1 ������ԕ�Ή� MOD START-----------
                oClient.Telegrapher = CreateTelegrapher( _
                   oClient.Code.ToString(EkCodeOupFormat), _
                   oChildSock, _
                   oClient.Code, _
                   oClient.StationName, _
                   oClient.CornerName)
                '-------Ver0.1 ������ԕ�Ή� MOD END-------------

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
        If QuitRequest.Gen().WriteToSocket(oClient.ChildSteerSock, TelServerAppBaseConfig.TelegrapherPendingLimitTicks) = False Then
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
        Dim oJoinLimitTimer As New TickTimer(TelServerAppBaseConfig.TelegrapherPendingLimitTicks)
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
            Else
                oNode = oNode.Next
            End If
        End While
    End Sub

    Protected Sub SendNakTelegramThenDisconnect(ByVal cause As NakCauseCode, ByVal oSourceTeleg As EkTelegram, ByVal oSocket As Socket)
        Dim oReplyTeleg As EkNakTelegram = oSourceTeleg.CreateNakTelegram(cause)
        If oReplyTeleg IsNot Nothing Then
            Log.Info("Sending NAK (" & cause.ToString() & ") telegram...")
            SendReplyTelegram(oSocket, oReplyTeleg, oSourceTeleg)
            '��L�Ăяo���̖߂�l�͖�������i���̌�̏����ɍ��ق��Ȃ����߁j�B
        End If

        Log.Info("Closing the connection...")
        Try
            oSocket.Shutdown(SocketShutdown.Both)
        Catch ex As SocketException
            Log.Error("SocketException caught.", ex)
        End Try
        oSocket.Close()
    End Sub

    Protected Function SendReplyTelegram(ByVal oSocket As Socket, ByVal oReplyTeleg As EkTelegram, ByVal oSourceTeleg As EkTelegram) As Boolean
        oReplyTeleg.RawReqNumber = oSourceTeleg.RawReqNumber
        oReplyTeleg.RawClientCode = oSourceTeleg.RawClientCode
        Return oReplyTeleg.WriteToSocket(oSocket, TelServerAppBaseConfig.TelegWritingLimitBaseTicks, TelServerAppBaseConfig.TelegWritingLimitExtraTicksPerMiB, TelServerAppBaseConfig.TelegLoggingMaxLengthOnWrite)
    End Function

    Protected Overridable Function SelectUnitsInService(ByVal sServiceDate As String) As DataTable
        '-------Ver0.1 ������ԕ�Ή� MOD START-----------
        Dim sSQL As String = _
           "SELECT STATION_NAME, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_NAME, CORNER_CODE, UNIT_NO" _
           & " FROM M_MACHINE" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND ADDRESS <> ''" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')"
        '-------Ver0.1 ������ԕ�Ή� MOD END-------------

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

    Protected Overridable Sub TransitDllStatusToAbnormal(ByVal sDataPurpose As String)
        Dim sSQL As String = _
           "UPDATE S_" & sDataPurpose & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_STS = " & DbConstants.DllStatusAbnormal.ToString() _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

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

    Protected Overridable Sub TransitDllStatusToAbnormal(ByVal oDll As MasProDllInfo, ByVal code As EkCode)
        'NOTE: �z�M�������ł��邩�i�f�[�^�{�̂��K�p���X�g���j�́A
        'DLL�o�[�W�����e�[�u����oDll.DataVersion���瓱���o���܂ł��Ȃ��B
        'MODEL_CODE��DATA_KIND�`UNIT_NO�����v������̂̒�����
        'DELIVERY_STS���u�z�M���v�̂��̂�I�ׂ΍ςނ͂��ł���B
        '�Ȃ��AVERSION�̍��v���`�F�b�N����K�v�͂Ȃ��B
        'MODEL_CODE�`DATA_VERSION�������VERSION���قȂ���̂�
        '�������Ɂu�z�M���v�ɂȂ��Ă��邱�Ƃ͂��蓾�Ȃ����߂ł���B
        Dim sSQL As String = _
           "UPDATE S_" & oDll.DataPurpose & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_STS = " & DbConstants.DllStatusAbnormal.ToString() _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & oDll.DataVersion & "'" _
           & " AND RAIL_SECTION_CODE = '" & code.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & code.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & code.Corner.ToString() _
           & " AND UNIT_NO = " & code.Unit.ToString() _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

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

    Protected Overridable Function SelectDllListFileHashValue(ByVal oDll As MasProDllInfo) As String
        Dim sSQL As String = _
           "SELECT HASH_VALUE" _
           & " FROM S_" & oDll.DataPurpose & "_LIST_HEADLINE" _
           & " WHERE MODEL_CODE = '" & oDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & oDll.DataVersion & "'" _
           & " AND LIST_VERSION = '" & oDll.ListVersion & "'"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CStr(dbCtl.ExecuteSQLToReadScalar(sSQL))

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function SelectDllDataFileHashValue(ByVal oDll As MasProDllInfo) As String
        Dim sSQL As String = _
           "SELECT HASH_VALUE" _
           & " FROM S_" & oDll.DataPurpose & "_DATA_HEADLINE" _
           & " WHERE MODEL_CODE = '" & oDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & oDll.DataVersion & "'"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CStr(dbCtl.ExecuteSQLToReadScalar(sSQL))

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function SelectDllDataFileName(ByVal oDll As MasProDllInfo) As String
        Dim sSQL As String = _
           "SELECT FILE_NAME" _
           & " FROM S_" & oDll.DataPurpose & "_DATA_HEADLINE" _
           & " WHERE MODEL_CODE = '" & oDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & oDll.DataVersion & "'"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CStr(dbCtl.ExecuteSQLToReadScalar(sSQL))

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function SelectDllAgentUnits(ByVal oDll As MasProDllInfo) As DataTable
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            '�z�M�J�n�������擾�B
            'NOTE: �����𖞂������R�[�h�̔z�M�J�n�����͑S�ē������A
            '�����𖞂������R�[�h�͕K�����݂���z��ł���B
            Dim sSQLToSelectDllStartTime As String = _
               "SELECT TOP 1 DELIVERY_START_TIME" _
               & " FROM S_" & oDll.DataPurpose & "_DLL_STS" _
               & " WHERE MODEL_CODE = '" & sClientModel & "'" _
               & " AND DATA_KIND = '" & oDll.DataKind & "'" _
               & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
               & " AND DATA_VERSION = '" & oDll.DataVersion & "'" _
               & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()
            Dim sDeliveryStartTime As String = CStr(dbCtl.ExecuteSQLToReadScalar(sSQLToSelectDllStartTime))
            Dim dllStartTime As DateTime = DateTime.ParseExact(sDeliveryStartTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)

            '���ʕ����e�[�u���i�@��\���}�X�^�̔z�M�w�����_�̗L���v�f�j���`����SQL��ҏW�B
            Dim sSQLToDefineCTE As String = _
               "WITH M_SERVICE_MACHINE (MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, ADDRESS, MONITOR_ADDRESS)" _
               & " AS" _
               & " (SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, ADDRESS, MONITOR_ADDRESS" _
                   & " FROM M_MACHINE" _
                   & " WHERE SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                                  & " FROM M_MACHINE" _
                                                  & " WHERE SETTING_START_DATE <= '" & sDeliveryStartDate & "'" _
                                                  & " AND INSERT_DATE <= CONVERT(DATETIME, '" & dllStartTime.ToString("yyyy/MM/dd HH:mm:ss") & "', 120))) "

            '�K�p�摕�u�̐���`���@���擾����SQL��ҏW�B
            'Dim sSQLToSelectApplicableUnits As String = _
            '   "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
            '   & " FROM S_" & oDll.DataPurpose & "_LIST" _
            '   & " WHERE FILE_NAME = '" & oDll.ListFileName & "'"
            Dim sSQLToSelectApplicableUnits As String = _
               "SELECT RAIL_SECTION_CODE + STATION_ORDER_CODE + CAST(CORNER_CODE AS varchar) + '_' + CAST(UNIT_NO AS varchar)" _
               & " FROM S_" & oDll.DataPurpose & "_LIST" _
               & " WHERE FILE_NAME = '" & oDll.ListFileName & "'"
            'NOTE: �v���O�����K�p���X�g�̏ꍇ�́A�L���ȍs�𒊏o����ɂ�����A
            '�K�p���ɂ��ƂÂ��ǉ��̏������������Ă���B�Ȃ��A�u�����N��
            '�ǂ̂悤�ȓ��t�i������j�����������Ƃ݂Ȃ����z��ł���B
            If oDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
                sSQLToSelectApplicableUnits = sSQLToSelectApplicableUnits _
                   & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                        & " OR APPLICABLE_DATE = '19000101'" _
                        & " OR APPLICABLE_DATE = '99999999')"
            End If

            '���ڂ̑��M��ƂȂ鑕�u��IP�A�h���X���擾����SQL��ҏW�B
            'NOTE: MONITOR_ADDRESS�ɂ́A�u�����N������\���͑z�肵�Ă��Ȃ��B
            '���Ƃ��΁A���ۂɓ��Y�R�[�i�[�ɑ��݂��Ȃ��Ď��Ղ̃��R�[�h��
            '�@��\���ɋL�q����^�p�ɂȂ����Ƃ��Ă��A���̃��R�[�h��
            'MONITOR_ADDRESS�ɂ��A���̂ƂȂ�Ď��Ղ�IP�A�h���X��
            '�ݒ肳���z��ł���B
            'Dim sSQLToSelectAddrOfAgents As String = _
            '   "SELECT DISTINCT MONITOR_ADDRESS" _
            '   & " FROM M_SERVICE_MACHINE" _
            '   & " WHERE (RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO) IN (" & sSQLToSelectApplicableUnits & ")" _
            '   & " AND MODEL_CODE = '" & oDll.DataApplicableModel & "'"
            Dim sSQLToSelectAddrOfAgents As String = _
               "SELECT DISTINCT MONITOR_ADDRESS" _
               & " FROM M_SERVICE_MACHINE" _
               & " WHERE MODEL_CODE = '" & oDll.DataApplicableModel & "'" _
               & " AND RAIL_SECTION_CODE + STATION_ORDER_CODE + CAST(CORNER_CODE AS varchar) + '_' + CAST(UNIT_NO AS varchar)" _
                   & " IN (" & sSQLToSelectApplicableUnits & ")"

            '���ڂ̑��M��ƂȂ鑕�u�̐���`���@���擾����SQL��ҏW�B
            'NOTE: sSQLToSelectAddrOfAgents�œ�����S�Ă̊Ď��Ղ܂��͓�����
            'sSQLToSelectAgents�œ�����i���ꂼ��̏o�͌����������ɂȂ�j
            '�z��ł��邪�A���̂��Ƃ̓`�F�b�N���Ȃ��B���̃`�F�b�N�́A
            '�K�p���X�g�ł͂Ȃ��A�@��\���}�X�^�̃`�F�b�N�ɂȂ邽�߁A
            '�@��\���}�X�^�̓o�^���ɍs����ׂ����̂ł���B
            Dim sSQLToSelectAgents As String = _
               "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
               & " FROM M_SERVICE_MACHINE" _
               & " WHERE MODEL_CODE = '" & sClientModel & "'" _
               & " AND ADDRESS IN (" & sSQLToSelectAddrOfAgents & ")"

            '���ڂ̑��M��ƂȂ鑕�u�̐���`���@���擾����B
            Return dbCtl.ExecuteSQLToRead(sSQLToDefineCTE & sSQLToSelectAgents)

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Sub UpdateLastSendVerByUncertainFlag(ByVal oDll As MasProDllInfo, ByVal agentCode As EkCode)
        'TODO: �b��
        Dim sSQL As String = _
           "UPDATE S_" & oDll.DataPurpose & "_DLL_VER" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DATA_VERSION = '0'" _
           & " WHERE MODEL_CODE = '" & sClientModel & "'" _
           & " AND DATA_KIND = '" & oDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
           & " AND RAIL_SECTION_CODE = '" & agentCode.RailSection.ToString("D3") & "'" _
           & " AND STATION_ORDER_CODE = '" & agentCode.StationOrder.ToString("D3") & "'" _
           & " AND CORNER_CODE = " & agentCode.Corner.ToString() _
           & " AND UNIT_NO = " & agentCode.Unit.ToString() _
           & " AND UNCERTAIN_FLG <> '0'"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

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

    Protected Overridable Function SelectLastSendVer(ByVal oDll As MasProDllInfo, ByVal agentCode As EkCode) As DataTable
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            '�w�肳�ꂽ����`���@�̑��u�ɑ΂���w�肳�ꂽ��ʂ̃f�[�^�̑O�񑗐M�o�[�W�������擾����SQL��ҏW�B
            Dim sSQLToSelectLastSendVer As String = _
               "SELECT DATA_VERSION" _
               & " FROM S_" & oDll.DataPurpose & "_DLL_VER" _
               & " WHERE MODEL_CODE = '" & sClientModel & "'" _
               & " AND DATA_KIND = '" & oDll.DataKind & "'" _
               & " AND DATA_SUB_KIND = '" & oDll.DataSubKind & "'" _
               & " AND RAIL_SECTION_CODE = '" & agentCode.RailSection.ToString("D3") & "'" _
               & " AND STATION_ORDER_CODE = '" & agentCode.StationOrder.ToString("D3") & "'" _
               & " AND CORNER_CODE = " & agentCode.Corner.ToString() _
               & " AND UNIT_NO = " & agentCode.Unit.ToString()

            '�w�肳�ꂽ����`���@�̑��u�ɑ΂���w�肳�ꂽ��ʂ̃f�[�^�̑O�񑗐M�o�[�W�������擾
            Return dbCtl.ExecuteSQLToRead(sSQLToSelectLastSendVer)

        Catch ex As DatabaseException
            Throw

        Catch ex As Exception
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Sub InsertScheduledUllFailureToCdt(ByVal oUll As ScheduledUllInfo, ByVal agentCode As EkCode)
        Dim recBaseInfo As New RecDataStructure.BaseInfo(sClientModel, agentCode)

        Dim aCdtKinds As String()
        Dim sDataKind As String = EkScheduledDataFileName.GetKind(oUll.FileName)
        If DbConstants.CdtKindsOfDataKinds.ContainsKey(sDataKind) Then
            aCdtKinds = DbConstants.CdtKindsOfDataKinds(sDataKind)
        Else
            'NOTE: �s���Ȏ�ʂɂ��āu�f�[�^�̓o�^�Ɏ��s���܂����v�ُ̈��
            '�o�^����ꍇ�ƁA�t�H�[���o�b�N�̕��@���قȂ邪�A�P�Ȃ�
            '�t�H�[���o�b�N�ł���ASchedule�̐ݒ�Ɍ�肪�Ȃ�����A
            '���삷�邱�Ƃ��Ȃ����߁A�C�ɂ��Ȃ����Ƃɂ���B
            Log.Error("CollectedDataTypo code for [" & sDataKind & "] is not defined.")
            aCdtKinds = New String(0) {sDataKind}
        End If

        Dim sErrorInfo As String = Lexis.CdtScheduledUllFailed.Gen(sCdtClientModelName, agentCode.Unit.ToString())

        For i As Integer = 0 To aCdtKinds.Length - 1
            CollectedDataTypoRecorder.Record(recBaseInfo, aCdtKinds(i), sErrorInfo)
        Next
    End Sub
#End Region

#Region "�C�x���g�������\�b�h"
    '-------Ver0.1 ������ԕ�Ή� MOD START-----------
    Protected MustOverride Function CreateTelegrapher( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal clientCode As EkCode, _
       ByVal sClientStationName As String, _
       ByVal sClientCornerName As String) As ServerTelegrapher
    '-------Ver0.1 ������ԕ�Ή� MOD END-------------

    Protected Overridable Function IsWaitingForChildMessage(ByVal oClient As Client) As Boolean
        If oClient.MasProDllState = ClientActiveXllState.Running Then Return True
        If oClient.ScheduledUllState = ClientActiveXllState.Running Then Return True
        Return False
    End Function

    Protected Overridable Sub ProcOnManagementReady()
        '�N���C�A���g��o�^����B
        'NOTE: �N�����Ȃ̂ŁA������A�����^�C���ȗ�O�����������ꍇ�́A
        '�v���Z�X�I���Ƃ���B
        Dim serviceUnits As DataRowCollection = SelectUnitsInService(EkServiceDate.GenString()).Rows
        For Each serviceUnit As DataRow In serviceUnits
            Dim code As EkCode
            code.Model = clientModelInProtocol
            code.RailSection = Integer.Parse(serviceUnit.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(serviceUnit.Field(Of String)("STATION_ORDER_CODE"))
            code.Corner = serviceUnit.Field(Of Integer)("CORNER_CODE")
            code.Unit = serviceUnit.Field(Of Integer)("UNIT_NO")
            '-------Ver0.1 ������ԕ�Ή� MOD START-----------
            RegisterClient(code, serviceUnit.Field(Of String)("STATION_NAME"), serviceUnit.Field(Of String)("CORNER_NAME"))
            '-------Ver0.1 ������ԕ�Ή� MOD END-------------
        Next serviceUnit

        '�S�N���C�A���g�̓d������M�X���b�h���J�n����B
        For Each oClient As Client In oClientList
            StartTelegrapher(oClient)
        Next oClient
    End Sub

    Protected Overridable Sub ProcOnTelegrapherAbort(ByVal oClient As Client)
        'NOTE: ���ɒ�~���čċN���҂��̏�ԁiClientState.WaitingForRestart�j��
        'oClient�ɂ��ẮA����ɑ΂��郁�b�Z�[�W���M�����݂��ہA
        '�ēx���̃��\�b�h���Ăяo�����悤�ɂȂ��Ă���B

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

        If oClient.MasProDllState = ClientActiveXllState.Running Then
            Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]�ւ̔z�M�𒆎~���܂����B")

            'DLL��ԃe�[�u���ɂāA���݂̔z�M�Ɋւ���oClient���������@�ւ̔z�M���ʂ��u�ُ�v�ɂ���B
            'NOTE: DB�����ꑕ�u���ɂ��邽�߁ADB�ڑ��ُ퓙�̏�Ԉˑ���O���\�����ʈُ�Ƃ݂Ȃ��B
            '�����ꔭ�������ꍇ�A�v���Z�X���ċN�����A���̍ۂɔz�M���ʂ��u�ُ�v�ɂł���΂悢�B
            TransitDllStatusToAbnormal(oCurMasProDll, oClient.Code)

            oClient.MasProDllState = ClientActiveXllState.None
            oCurMasProDll.RemainingCount -= 1
            If oCurMasProDll.RemainingCount = 0 Then
                Log.Info("�K�p���X�g[" & oCurMasProDll.ListFileName & "]�ɂ��z�M���I�����܂��B")
                oCurMasProDll = Nothing
                oMasProDllQueue.Dequeue()
                DoNextMasProDll()
            Else
                RequestMasProDllToNextClient()
            End If
        End If

        If oClient.ScheduledUllState = ClientActiveXllState.Running Then
            Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]����̎��W�𒆎~���܂����B")

            'oClient���������@����̌��݂̎��W�ɂ��āADB�Ɉُ��o�^����B
            InsertScheduledUllFailureToCdt(oCurScheduledUll, oClient.Code)

            oClient.ScheduledUllState = ClientActiveXllState.None
            oCurScheduledUll.RemainingCount -= 1
            If oCurScheduledUll.RemainingCount = 0 Then
                Log.Info("�f�[�^[" & oCurScheduledUll.FileName & "]�̎��W���I�����܂��B")
                oCurScheduledUll = Nothing
                oScheduledUllQueue.Dequeue()
                DoNextScheduledUll()
            Else
                RequestScheduledUllToNextClient()
            End If
        End If
    End Sub

    Protected Overridable Sub ProcOnTelegrapherRestart(ByVal oClient As Client)
    End Sub

    Protected Overridable Sub ProcOnChildSteerSockReadable(ByVal oClient As Client)
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oClient.ChildSteerSock, TelServerAppBaseConfig.TelegrapherPendingLimitTicks)
        If Not oRcvMsg.HasValue Then
            Log.Fatal("Telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "] seems broken.")
            AbortTelegrapher(oClient)
            Return
        End If

        ProcOnChildMessageReceive(oClient, oRcvMsg)
    End Sub

    Protected Overridable Sub ProcOnChildMessageReceive(ByVal oClient As Client, ByVal oRcvMsg As InternalMessage)
        Select Case oRcvMsg.Kind
            Case ServerAppInternalMessageKind.MasProDllResponse
                Log.Info("MasProDllResponse received.")
                ProcOnMasProDllResponseReceive(oClient, oRcvMsg)
            Case ServerAppInternalMessageKind.ScheduledUllResponse
                Log.Info("ScheduledUllResponse received.")
                ProcOnScheduledUllResponseReceive(oClient, oRcvMsg)
        End Select
    End Sub

    Protected Overridable Sub ProcOnMasProDllResponseReceive(ByVal oClient As Client, ByVal oRcvMsg As InternalMessage)
        Debug.Assert(oClient.MasProDllState = ClientActiveXllState.Running)
        oClient.MasProDllState = ClientActiveXllState.None
        oCurMasProDll.RemainingCount -= 1
        If oCurMasProDll.RemainingCount = 0 Then
            Log.Info("�K�p���X�g[" & oCurMasProDll.ListFileName & "]�ɂ��z�M���I�����܂��B")
            oCurMasProDll = Nothing
            oMasProDllQueue.Dequeue()
            DoNextMasProDll()
        Else
            RequestMasProDllToNextClient()
        End If
    End Sub

    Protected Overridable Sub ProcOnScheduledUllResponseReceive(ByVal oClient As Client, ByVal oRcvMsg As InternalMessage)
        Debug.Assert(oClient.ScheduledUllState = ClientActiveXllState.Running)
        oClient.ScheduledUllState = ClientActiveXllState.None
        oCurScheduledUll.RemainingCount -= 1
        If oCurScheduledUll.RemainingCount = 0 Then
            Log.Info("�f�[�^[" & oCurScheduledUll.FileName & "]�̎��W���I�����܂��B")
            oCurScheduledUll = Nothing
            oScheduledUllQueue.Dequeue()
            DoNextScheduledUll()
        Else
            RequestScheduledUllToNextClient()
        End If
    End Sub

    Protected Overridable Sub ProcOnMessageReceive(ByVal oMessage As Message)
        Select Case oMessage.AppSpecific
            Case ExtMasProDllRequest.FormalKind
                Log.Info("ExtMasProDllRequest received.")
                ProcOnMasProDllRequestReceive(oMessage)
            Case ExtScheduledUllRequest.FormalKind
                Log.Info("ExtScheduledUllRequest received.")
                ProcOnScheduledUllRequestReceive(oMessage)
            Case ExtServiceDateChangeNotice.FormalKind
                Log.Info("ExtServiceDateChangeNotice received.")
                ProcOnServiceDateChangeNoticeReceive(oMessage)
            Case Else
                Log.Error("Unwelcome ExtMessage received.")
        End Select
    End Sub

    Protected Overridable Sub ProcOnMasProDllRequestReceive(ByVal oMessage As Message)
        Dim oMsg As New ExtMasProDllRequest(oMessage)
        Try
            Dim sFileName As String = oMsg.ListFileName
            If Not EkMasProListFileName.IsValid(sFileName) Then
                Log.Error("The message specifies invalid file [" & sFileName & "].")
                Return
            End If
        Catch ex As Exception
            Log.Error("Exception caught on parsing the message.", ex)
            Return
        End Try

        oMasProDllQueue.Enqueue(oMsg)
        If oMasProDllQueue.Count = 1 Then
            'NOTE: ��Ɏ��s���ɂȂ��Ă���z�M�������ꍇ�ł���B
            DoNextMasProDll()
        End If
    End Sub

    Protected Overridable Sub ProcOnScheduledUllRequestReceive(ByVal oMessage As Message)
        Dim oMsg As New ExtScheduledUllRequest(oMessage)
        Try
            Dim sFileName As String = oMsg.FileName
            If Not EkScheduledDataFileName.IsValid(sFileName) Then
                Log.Error("The message specifies invalid file [" & sFileName & "].")
                Return
            End If
        Catch ex As Exception
            Log.Error("Exception caught on parsing the message.", ex)
            Return
        End Try

        oScheduledUllQueue.Enqueue(oMsg)
        If oScheduledUllQueue.Count = 1 Then
            'NOTE: ��Ɏ��s���ɂȂ��Ă�����W�������ꍇ�ł���B
            DoNextScheduledUll()
        End If
    End Sub

    Protected Overridable Sub ProcOnServiceDateChangeNoticeReceive(ByVal oMessage As Message)
        '�@��\���}�X�^����A���݂̉^�p���t�ŉ^�p�����ׂ��S�Ă̍��@����������B
        Dim oServiceUnitTable As DataTable
        Try
            oServiceUnitTable = SelectUnitsInService(EkServiceDate.GenString())
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            '���[�U���C�t���ꏊ�Ɉُ���L�^����B
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtMachineMasterErratumDetected.Gen())
            Return
        End Try

        Dim serviceUnits As EnumerableRowCollection(Of DataRow) = oServiceUnitTable.AsEnumerable()

        '���ɓo�^���Ă��鍆�@�Ɋւ��āA�����̌��ʂɊ܂܂�Ă��Ȃ��ꍇ�́A
        '���Y���@�p�̓d������M�X���b�h�ɏI����v������B
        'NOTE: �����I����������̂��̂�A�ċN���҂��̂��̂��A
        '�I����v�����邱�ƂŁA�I���Ώۍ��@�i�o�^�����҂���ԁj�ɂȂ�B
        For Each oClient As Client In oClientList
            Dim code As EkCode = oClient.Code
            Dim num As Integer = ( _
               From serviceUnit In serviceUnits _
               Where serviceUnit.Field(Of String)("RAIL_SECTION_CODE") = code.RailSection.ToString("D3") And _
                     serviceUnit.Field(Of String)("STATION_ORDER_CODE") = code.StationOrder.ToString("D3") And _
                     serviceUnit.Field(Of Integer)("CORNER_CODE") = code.Corner And _
                     serviceUnit.Field(Of Integer)("UNIT_NO") = code.Unit _
               Select serviceUnit _
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

        '�I���Ώۍ��@�Ɋւ��āADLL��ԃe�[�u���̌��݂̔z�M�̔z�M���ʂ��u�ُ�v�ɂ���B
        'NOTE: Telegrapher���������I�������ꍇ�ATelegrapher���g���s���͂��̂��Ƃł��邪�A
        'Telegrapher���������I�������Ƃ͌���Ȃ����߁A�����ł��s���B
        Dim dllStoppedCount As Integer = 0
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Discarded AndAlso _
               oClient.MasProDllState <> ClientActiveXllState.None Then
                Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]�ւ̔z�M�𒆎~���܂����B")

                '���Y���@�Ɋւ��āADLL��ԃe�[�u���̔z�M���ʂ��u�ُ�v�ɂ���B
                'NOTE: DB�����ꑕ�u���ɂ��邽�߁ADB�ڑ��ُ퓙�̏�Ԉˑ���O���\�����ʈُ�Ƃ݂Ȃ��B
                '�����ꔭ�������ꍇ�A�v���Z�X���ċN�����A���̍ۂɔz�M���ʂ��u�ُ�v�ɂł���΂悢�B
                TransitDllStatusToAbnormal(oCurMasProDll, oClient.Code)

                If oClient.MasProDllState = ClientActiveXllState.Running Then
                    dllStoppedCount += 1
                Else
                    Debug.Assert(oClient.MasProDllState = ClientActiveXllState.Waiting)
                    oCurMasProDll.WaitingClients.Remove(oClient)
                End If

                oClient.MasProDllState = ClientActiveXllState.None
                oCurMasProDll.RemainingCount -= 1
            End If
        Next oClient

        '���݂̎��W�̏I���Ώۍ��@�Ɋւ��āA���W�f�[�^��L�e�[�u���Ɉُ��o�^����B
        'NOTE: Telegrapher���������I�������ꍇ�ATelegrapher���g���s���͂��̂��Ƃł��邪�A
        'Telegrapher���������I�������Ƃ͌���Ȃ����߁A�����ł��s���B
        Dim ullStoppedCount As Integer = 0
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Discarded AndAlso _
               oClient.ScheduledUllState <> ClientActiveXllState.None Then
                Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]����̎��W�𒆎~���܂����B")

                '���Y���@����̎��W�ɂ��āADB�Ɉُ��o�^����B
                InsertScheduledUllFailureToCdt(oCurScheduledUll, oClient.Code)

                If oClient.ScheduledUllState = ClientActiveXllState.Running Then
                    ullStoppedCount += 1
                Else
                    Debug.Assert(oClient.ScheduledUllState = ClientActiveXllState.Waiting)
                    oCurScheduledUll.WaitingClients.Remove(oClient)
                End If

                oClient.ScheduledUllState = ClientActiveXllState.None
                oCurScheduledUll.RemainingCount -= 1
            End If
        Next oClient

        '�o�^��������B
        UnregisterDiscardedClients()

        '�z�M�����s���������Ɍ������V���Ȕz�M���J�n����B
        If oCurMasProDll IsNot Nothing AndAlso oCurMasProDll.RemainingCount = 0 Then
            Log.Info("�K�p���X�g[" & oCurMasProDll.ListFileName & "]�ɂ��z�M���I�����܂��B")
            oCurMasProDll = Nothing
            oMasProDllQueue.Dequeue()
            DoNextMasProDll()
        Else
            While dllStoppedCount > 0
                RequestMasProDllToNextClient()
                dllStoppedCount -= 1
            End While
        End If

        '���W�����s���������Ɍ������V���Ȏ��W���J�n����B
        If  oCurScheduledUll IsNot Nothing AndAlso oCurScheduledUll.RemainingCount = 0 Then
            Log.Info("�f�[�^[" & oCurScheduledUll.FileName & "]�̎��W���I�����܂��B")
            oCurScheduledUll = Nothing
            oScheduledUllQueue.Dequeue()
            DoNextScheduledUll()
        Else
            While ullStoppedCount > 0
                RequestScheduledUllToNextClient()
                dllStoppedCount -= 1
            End While
        End If

        '�����œ����w���@�펯�ʃR�[�h�Ɋւ��āA�o�^����Ă��Ȃ����̂́A�o�^����B
        '�o�^����Ă�����̂́A�w����R�[�i�[�����X�V����Ă��Ȃ����`�F�b�N���A
        '�X�V����Ă�����A�V�����w���ƃR�[�i�[����ʒm����B
        For Each row As DataRow In oServiceUnitTable.Rows
            Dim code As EkCode
            code.Model = clientModelInProtocol
            code.RailSection = Integer.Parse(row.Field(Of String)("RAIL_SECTION_CODE"))
            code.StationOrder = Integer.Parse(row.Field(Of String)("STATION_ORDER_CODE"))
            code.Corner = row.Field(Of Integer)("CORNER_CODE")
            code.Unit = row.Field(Of Integer)("UNIT_NO")
            '-------Ver0.1 ������ԕ�Ή� MOD START-----------
            Dim oClient As Client = FindClient(code)
            Dim sStationName As String = row.Field(Of String)("STATION_NAME")
            Dim sCornerName As String = row.Field(Of String)("CORNER_NAME")
            If oClient Is Nothing Then
                RegisterClient(code, sStationName, sCornerName)
            Else
                If Not oClient.StationName.Equals(sStationName) OrElse _
                   Not oClient.CornerName.Equals(sCornerName) Then
                    'NOTE: SendToTelegrapher��Telegrapher��Abort�����邱�ƂɂȂ����ꍇ��
                    '���X�^�[�g���ɐV�������O��n����悤�A���̎��_��Client�I�u�W�F�N�g��
                    '���e�����������Ă����B
                    oClient.StationName = sStationName
                    oClient.CornerName = sCornerName

                    Dim oExt As New NameChangeNoticeExtendPart()
                    oExt.StationName = sStationName
                    oExt.CornerName = sCornerName

                    Log.Info("Sending NameChange notice to telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
                    SendToTelegrapher(oClient, NameChangeNotice.Gen(oExt))
                End If
            End If
            '-------Ver0.1 ������ԕ�Ή� MOD END-------------
        Next row

        '�o�^�������@�̓d������M�X���b�h���J�n������B
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.Registered Then
                StartTelegrapher(oClient)
            End If
        Next oClient
    End Sub

    'NOTE: MayOverride
    Protected Overridable Sub ProcOnAccept(ByVal oNewSocket As Socket)
    End Sub

    'NOTE: ��Ɏ��s���Ă���z�M�������ꍇ�̂݌Ăяo�����B
    '�������A���ɍs���ׂ��z�M������Ƃ͌���Ȃ����̂Ƃ���B
    Protected Overridable Sub DoNextMasProDll()
        While oMasProDllQueue.Count <> 0
            Dim oMsg As ExtMasProDllRequest = oMasProDllQueue.Peek()
            Dim sListFileName As String = oMsg.ListFileName
            Log.Info("�K�p���X�g[" & sListFileName & "]�ɂ��z�M���J�n���܂��B")

            'NOTE: DB�����ꑕ�u���ɂ��邽�߁ADB�ڑ��ُ퓙�̏�Ԉˑ���O���\�����ʈُ�Ƃ݂Ȃ��B
            '�����ꔭ�������ꍇ�A�v���Z�X���ċN�����A���̍ۂɔz�M���ʂ��u�ُ�v�ɂł���΂悢�B

            Dim oDll As New MasProDllInfo()
            oDll.DataApplicableModel = EkMasProListFileName.GetDataApplicableModel(sListFileName)
            oDll.DataPurpose = EkMasProListFileName.GetDataPurpose(sListFileName)
            oDll.DataKind = EkMasProListFileName.GetDataKind(sListFileName)
            oDll.DataSubKind = EkMasProListFileName.GetDataSubKind(sListFileName)
            oDll.DataVersion = EkMasProListFileName.GetDataVersion(sListFileName)
            oDll.ListVersion = EkMasProListFileName.GetListVersion(sListFileName)
            oDll.DataFileName = SelectDllDataFileName(oDll)
            oDll.ListFileName = sListFileName
            oDll.DataFileHashValue = SelectDllDataFileHashValue(oDll)
            oDll.ListFileHashValue = SelectDllListFileHashValue(oDll)

            '���ڂ̔z�M��ƂȂ鍆�@�̉w���@�펯�ʃR�[�h����������B
            '�܂��A�e���@�Ɋւ���f�[�^�{�̂̍ŏI���M�o�[�W�������擾����B
            Dim agents As DataRowCollection = SelectDllAgentUnits(oDll).Rows
            Dim oList As New List(Of Client)(agents.Count)
            For Each agent As DataRow In agents
                Dim code As EkCode
                code.Model = clientModelInProtocol
                code.RailSection = Integer.Parse(agent.Field(Of String)("RAIL_SECTION_CODE"))
                code.StationOrder = Integer.Parse(agent.Field(Of String)("STATION_ORDER_CODE"))
                code.Corner = agent.Field(Of Integer)("CORNER_CODE")
                code.Unit = agent.Field(Of Integer)("UNIT_NO")

                'NOTE: ����agent�֑��M���s���ہA�ǂ�Ȃ��Ƃ������Ă��i���Ƃ��΁A
                'agent�Ɗ��ɕs�ʂł������Ƃ��Ă��j�A���M���ʂ̊m��ɂ����
                'UNCERTAIN_FLG���N���A��������O�ɁA���������s����i������
                'UNCERTAIN_FLG��DATA_VERSION�ɔ��f������j���̂Ƃ���B

                'OPT: �œK���BDB�ɐڑ������܂܁A���ׂĂ��s���悤�ɁB
                UpdateLastSendVerByUncertainFlag(oDll, code)

                Dim sendSuite As Boolean = False
                If oMsg.ForcingFlag = True Then
                    sendSuite = True
                Else
                    'OPT: ExecuteSQLToReadScalar���g���B
                    Dim lastSendVer As DataRowCollection = SelectLastSendVer(oDll, code).Rows
                    If lastSendVer.Count = 0 OrElse
                       Not lastSendVer(0).Field(Of String)("DATA_VERSION").Equals(oDll.DataVersion) Then
                        sendSuite = True
                    End If
                End If

                Dim oClient As Client = FindClient(code)
                If oClient Is Nothing Then
                    Log.Error("[" & oClient.Code.ToString(EkCodeOupFormat) & "]�͋@��\���ɓo�^����Ă��܂���B")

                    'DLL��ԃe�[�u���ɂāA���Y�̔z�M�Ɋւ��铖�Y�̍��@�ւ̔z�M���ʂ��u�ُ�v�ɂ���B
                    TransitDllStatusToAbnormal(oDll, code)
                Else
                    If sendSuite Then
                        Log.Info("[" & oClient.Code.ToString(EkCodeOupFormat) & "]�ւ͓K�p���X�g�ƃf�[�^�{�̂�z�M���܂��B")
                    Else
                        Log.Info("[" & oClient.Code.ToString(EkCodeOupFormat) & "]�ւ͓K�p���X�g��z�M���܂��B")
                    End If
                    oClient.MasProDllState = ClientActiveXllState.Waiting
                    oClient.SendSuiteOnMasProDll = sendSuite
                    oList.Add(oClient)
                End If
            Next agent

            If oList.Count <> 0 Then
                oDll.RemainingCount = oList.Count
                oDll.WaitingClients = oList
                oCurMasProDll = oDll
                For count As Integer = 0 To TelServerAppBaseConfig.ConcurrentMasProDllMaxCount - 1
                    RequestMasProDllToNextClient()
                Next
                Exit While
            Else
                Log.Info("�K�p���X�g[" & sListFileName & "]�ɂ��z�M���I�����܂��B")
                oMasProDllQueue.Dequeue()
            End If
        End While
    End Sub

    'NOTE: ��Ɏ��s���Ă�����W�������ꍇ�̂݌Ăяo�����B
    '�������A���ɍs���ׂ����W������Ƃ͌���Ȃ����̂Ƃ���B
    Protected Overridable Sub DoNextScheduledUll()
        While oScheduledUllQueue.Count <> 0
            Dim oMsg As ExtScheduledUllRequest = oScheduledUllQueue.Peek()
            Dim sFileName As String = oMsg.FileName
            Log.Info("�f�[�^[" & sFileName & "]�̎��W���J�n���܂��B")

            Dim oUll As New ScheduledUllInfo()
            oUll.FileName = sFileName

            Dim oList As New List(Of Client)(oClientList.Count)
            For Each oClient As Client In oClientList
                oClient.ScheduledUllState = ClientActiveXllState.Waiting
                oList.Add(oClient)
            Next oClient

            If oList.Count <> 0 Then
                oUll.RemainingCount = oList.Count
                oUll.WaitingClients = oList
                oCurScheduledUll = oUll
                For count As Integer = 0 To TelServerAppBaseConfig.ConcurrentScheduledUllMaxCount - 1
                    RequestScheduledUllToNextClient()
                Next
                Exit While
            Else
                Log.Info("�f�[�^[" & sFileName & "]�̎��W���I�����܂��B")
                oScheduledUllQueue.Dequeue()
            End If
        End While
    End Sub

    'NOTE: oCurMasProDll�����s�����iRemainingCount��0�łȂ��j�ꍇ�̂݌Ăяo�����B
    '�������AWaiting��Ԃ�Client���c���Ă���Ƃ͌���Ȃ����̂Ƃ���B
    Protected Overridable Sub RequestMasProDllToNextClient()
        If oCurMasProDll.WaitingClients.Count = 0 Then Return

        Dim oClient As Client = oCurMasProDll.WaitingClients(0)
        Debug.Assert(oClient.MasProDllState = ClientActiveXllState.Waiting)

        'NOTE: oClient����̔\���I���W�����s���̏ꍇ�́A�ʐM�ő҂��ƂɂȂ���
        '���܂����A�{���A���̎��_�œ������b�Z�[�W�𑗐M���錠��������̂�
        '�d������M�X���b�h���Ƃ������ƂɂȂ邽�߁A�ʂ�Client�ւ̔z�M��
        '��ɍs�������悢��������Ȃ��B

        Log.Info("[" & oClient.Code.ToString(EkCodeOupFormat) & "]�ւ̔z�M���s���܂��B")
        oCurMasProDll.WaitingClients.RemoveAt(0)
        oClient.MasProDllState = ClientActiveXllState.Running

        Dim oExt As New MasProDllRequestExtendPart()
        oExt.ListFileName = oCurMasProDll.ListFileName
        oExt.ListFileHashValue = oCurMasProDll.ListFileHashValue
        If oClient.SendSuiteOnMasProDll Then
            oExt.DataFileName = oCurMasProDll.DataFileName
            oExt.DataFileHashValue = oCurMasProDll.DataFileHashValue
        Else
            oExt.DataFileName = Nothing
            oExt.DataFileHashValue = Nothing
        End If

        Log.Info("Sending MasProDll request to telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
        SendToTelegrapher(oClient, MasProDllRequest.Gen(oExt))
    End Sub

    'NOTE: oScheduledUllQueue�����s�����iRemainingCount��0�łȂ��j�ꍇ�̂݌Ăяo�����B
    '�������AWaiting��Ԃ�Client���c���Ă���Ƃ͌���Ȃ����̂Ƃ���B
    Protected Overridable Sub RequestScheduledUllToNextClient()
        If oCurScheduledUll.WaitingClients.Count = 0 Then Return

        Dim oClient As Client = oCurScheduledUll.WaitingClients(0)
        Debug.Assert(oClient.ScheduledUllState = ClientActiveXllState.Waiting)

        'NOTE: oClient�ւ̔\���I�z�M�����s���̏ꍇ�́A�ʐM�ő҂��ƂɂȂ���
        '���܂����A���̎��_�œ������b�Z�[�W�𑗐M���錠��������̂�
        '�d������M�X���b�h���Ƃ������ƂɂȂ��Ă��܂����߁A�ʂ�Client�����
        '���W���ɍs�������悢��������Ȃ��B

        Log.Info("[" & oClient.Code.ToString(EkCodeOupFormat) & "]����̎��W���s���܂��B")
        oCurScheduledUll.WaitingClients.RemoveAt(0)
        oClient.ScheduledUllState = ClientActiveXllState.Running

        Dim oExt As New ScheduledUllRequestExtendPart()
        oExt.FileName = oCurScheduledUll.FileName

        Log.Info("Sending ScheduledUll request to telegrapher [" & oClient.Code.ToString(EkCodeOupFormat) & "]...")
        SendToTelegrapher(oClient, ScheduledUllRequest.Gen(oExt))
    End Sub
#End Region

End Class
