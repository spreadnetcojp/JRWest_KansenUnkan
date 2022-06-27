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
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �Ή^�ǒ[���ʐM�v���Z�X�̃��C�����������Telegrapher�Ǘ���������������N���X�B
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "�����N���X"
    Private Enum ClientState
        Registered
        QuitRequested
        Discarded
    End Enum

    Private Class Client
        Public State As ClientState
        Public Name As String
        Public Telegrapher As MyTelegrapher
        Public ChildSteerSock As Socket
    End Class
#End Region

#Region "�萔��ϐ�"
    '�d������M�X���b�h��Abort��������
    Private Const TelegrapherAbortLimitTicks As Integer = 5000

    '�d������
    Private Shared oTelegGene As EkTelegramGene

    '�d����荞�݊�
    Private Shared oTelegImporter As EkTelegramImporter

    '�{�v���Z�X��FTP�Ō��J����f�B���N�g���̃��[�J���p�X
    Private Shared sFtpBase As String

    '�N���C�A���g�̃��X�g
    Private Shared oClientList As LinkedList(Of Client)

    '���C���E�B���h�E
    Private Shared oMainForm As ServerAppForm

    '�ʐM�Ǘ��X���b�h�ւ̏I���v���t���O
    Private Shared quitListener As Integer
#End Region

    ''' <summary>
    ''' �Ή^�ǒ[���ʐM�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �Ή^�ǒ[���ʐM�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppToOpClient")
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

                Log.Init(sLogBasePath, "ToOpClient")
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

                LocalConnectionProvider.Init()

                oTelegGene = New EkTelegramGeneForNativeModels(Config.FtpServerRootDirPath)
                oTelegImporter = New EkTelegramImporter(oTelegGene)
                oClientList = New LinkedList(Of Client)

                '���b�Z�[�W���[�v���A�C�h����ԂɂȂ�O�i���A����I�ɂ�����s��
                '�X���b�h���N������O�j�ɁA�����ؖ��t�@�C�����X�V���Ă����B
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '�ʐM�Ǘ��X���b�h���J�n����B
                Dim oListenerThread As New Thread(AddressOf MainClass.ListeningLoop)
                Log.Info("Starting the listener thread...")
                quitListener = 0
                oListenerThread.Name = "Listener"
                oListenerThread.Start()

                '�E�C���h�E�v���V�[�W�������s����B
                'NOTE: ���̃��\�b�h�����O���X���[����邱�Ƃ͂Ȃ��B
                ServerAppBaseMain(oMainForm)

                Try
                    '�ʐM�Ǘ��X���b�h�ɏI����v������B
                    Log.Info("Sending quit request to the listener thread...")
                    Thread.VolatileWrite(quitListener, 1)

                    'NOTE: �ȉ��ŒʐM�Ǘ��X���b�h���I�����Ȃ��ꍇ�A
                    '�ʐM�Ǘ��X���b�h�͐����ؖ����s��Ȃ��͂��ł���A
                    '�󋵂ւ̑Ώ��̓v���Z�X�}�l�[�W���ōs����z��ł���B

                    '�ʐM�Ǘ��X���b�h�̏I����҂B
                    Log.Info("Waiting for the listener thread to quit...")
                    oListenerThread.Join()
                    Log.Info("The listener thread has quit.")
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oListenerThread.Abort()
                End Try
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                If oMainForm IsNot Nothing Then
                    oMainForm.Dispose()
                End If
                LocalConnectionProvider.Dispose()
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
    ''' �ʐM�Ǘ��X���b�h�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' ���X�j���O�\�P�b�g�̐��䂨���Telegrapher�̊Ǘ����s���B
    ''' </remarks>
    Private Shared Sub ListeningLoop()
        Dim oListenerSock As Socket = Nothing  '���X�j���O�\�P�b�g

        Try
            Log.Info("The listener thread started.")
            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            Dim oCheckReadList As New ArrayList()

            '�e�d������M�X���b�h�̈ꎞ��Ɨp�f�B���N�g����e�f�B���N�g�����Ƃ܂Ƃ߂č폜����B
            Log.Info("Sweeping directory [" & Config.TemporaryBaseDirPath & "]...")
            Utility.DeleteTemporalDirectory(Config.TemporaryBaseDirPath)

            '�e�d������M�X���b�h��FTP�T�C�g�p�f�B���N�g���₻�̓��e�����폜����B
            'NOTE: ���̃f�B���N�g���́A���̃v���Z�X�����łȂ��AFTP�T�[�o���Q�ƁE���삵����B
            '���ɑ��݂��Ă�����̂̍폜�Ɏ��s����ꍇ�́A�]���̏I����F�����Ă��Ȃ�FTP�T�[�o��
            '�������݂ň����Ă���P�[�X�ƍl�����邪�A�Y������T�u�f�B���N�g����t�@�C���݂̂�
            '�c���ď��������s����B�Ȃ��A���������s�����ɂ��̃v���Z�X���ُ�I��������Ƃ��Ă��A
            '�v���Z�X�}�l�[�W�������̃v���Z�X���N�����鎟�̋@���FTP�T�[�o���K�[�h��������
            '����΁A�������琳�퓮�삪�n�܂邽�߁A���͂Ȃ��͂��ł���B�A�v���ċN����
            '���̋@��𓦂����ɁA�S�Ă̈ꎞ�t�@�C�����폜����Ƃ����Ӗ��ł́A���̕������z�I��
            '���邩������Ȃ����A�Ƃ肠�����p����D�悵�āA���̂悤�ɂ��Ă���B
            sFtpBase = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, Config.PermittedPathInFtp)
            If Directory.Exists(sFtpBase) Then
                Log.Info("Cleaning up directory [" & sFtpBase & "]...")
                Utility.CleanUpDirectory(sFtpBase)
            End If

            '�}�X�^/�v���O�����̊Ǘ��f�B���N�g�����Ȃ���΁A�쐬���Ă����B
            Directory.CreateDirectory(Config.MasProDirPath)

            '���b�X�����J�n����B
            Log.Info("Start listening for [" & Config.IpAddrForTelegConnection.ToString() & ":" & Config.IpPortForTelegConnection.ToString() & "].")
            oListenerSock = SockUtil.StartListener(Config.IpAddrForTelegConnection, Config.IpPortForTelegConnection)

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Thread.VolatileRead(quitListener) = 0
                Dim oSocket As Socket = Nothing

                '�\�P�b�g�ǂݏo���Ď����Ď����ʎ擾�p�̃��X�g���쐬����B
                oCheckReadList.Clear()
                oCheckReadList.Add(oListenerSock)

                '���X�j���O�\�P�b�g���ǂݏo���\�ɂȂ�܂ŏ��莞�ԑҋ@����B
                Socket.Select(oCheckReadList, Nothing, Nothing, Config.PollIntervalTicks * 1000)

                '���X�j���O�\�P�b�g���ǂݏo���\�ɂȂ����ꍇ�́A����M�p�\�P�b�g�����o���B
                If oCheckReadList.Count > 0 Then
                    Try
                        oSocket = SockUtil.Accept(oListenerSock)
                    Catch ex As OPMGException
                        'NOTE: ���ۂ̂Ƃ���͂Ƃ������A���X�j���O�\�P�b�g���ǂݏo���\
                        '�ɂȂ�������Ƃ����āA���������Accept()����������Ƃ͌���Ȃ�
                        '�ilinux�̃\�P�b�g�̂悤�ɁAAccept()���Ăяo���܂ł̊Ԃɔ���
                        '�����R�l�N�V�����ُ̈킪�AAccept()�Œʒm�����\��������j
                        '���̂Ƃ݂Ȃ��B
                        Log.Error("Exception caught.", ex)
                    End Try
                End If

                '����M�p�\�P�b�g�����o�����ꍇ�A�d������M�X���b�h���쐬���ēn���B
                If oSocket IsNot Nothing Then
                    RegisterClient(oSocket)
                End If

                '�O��`�F�b�N���珊�莞�Ԍo�߂��Ă���ꍇ�́A�S�Ă�
                '�d������M�X���b�h�ɂ��āA�ُ�I���܂��̓t���[�Y
                '���Ă��Ȃ����A���邢�͏I���v���҂��i�ؒf�ς݁j��
                '�Ȃ��Ă��Ȃ������`�F�b�N����B
                'NOTE: �V�����R�l�N�V���������ꂽ�ioSocket IsNot Nothing
                '�ł���j�ꍇ�́A����[���̉ߋ��̃R�l�N�V�������ؒf�ς݂�
                '�Ȃ��Ă���\�����������߁A���莞�Ԃ��o�߂��Ă��Ȃ��Ă��A
                '�`�F�b�N���s�����Ƃɂ��Ă���B
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oSocket IsNot Nothing OrElse _
                   oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()

                    'Log.Info("Checking pulse of all telegraphers...")
                    For Each oClient As Client In oClientList
                        If oClient.Telegrapher.ThreadState = ThreadState.Stopped Then
                            '�\�����ʗ�O�Ȃǂňُ�I�����Ă���ꍇ�ł���B
                            Log.Fatal("Sweeping telegrapher [" & oClient.Name & "] because it stopped...")
                            SweepBrokenTelegrapher(oClient)
                        ElseIf TickTimer.GetTickDifference(systemTick, oClient.Telegrapher.LastPulseTick) > Config.TelegrapherPendingLimitTicks Then
                            '�t���[�Y���Ă���ꍇ�ł���B
                            Log.Fatal("Sweeping telegrapher [" & oClient.Name & "] because it seems broken...")
                            SweepBrokenTelegrapher(oClient)
                        ElseIf oClient.Telegrapher.LineStatus = LineStatus.Disconnected Then
                            'Telegrapher���[���Ƃ̃R�l�N�V������ؒf���āA
                            '�e�X���b�h����̏I���v����҂��Ă���ꍇ�ł���B

                            'NOTE: Telegrapher�̐������I���V�[�P���X�́A
                            '����ł���BTelegrapher������ɏI������d�l��
                            '���蓾�Ȃ��BTelegrapher������n�ŏ���ɏI������
                            '�ƂȂ�ƁA�v���Z�X�I������Telegrapher�ɏI���v����
                            '���M����ہA����n�ł���ɂ��ւ�炸�A�u���b�N
                            '����邱�Ƃ�ʏ�̃P�[�X�Ƃ��đz�肵�Ȃ����
                            '�Ȃ�Ȃ��Ȃ邽�߂ł���B

                            '�d������M�X���b�h�ɏI����v������B
                            QuitTelegrapher(oClient)
                        End If
                    Next oClient

                    '�I����v�������d������M�X���b�h�̏I����҂B
                    WaitForTelegraphersToQuit()

                    '�s�v�ɂȂ����N���C�A���g��o�^��������B
                    UnregisterDiscardedClients()
                End If
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j�́A
            '�v���Z�X�}�l�[�W�����s���̂ŁA�����ł͕s�v�ł���B

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        Finally
            If oClientList IsNot Nothing
                '�c���Ă���N���C�A���g�̓d������M�X���b�h�ɏI����v������B
                'NOTE: �����ł́A�Γd������M�X���b�h�ʐM�p�\�P�b�g��
                '�d������M�X���b�h���쐬������A�d������M�X���b�h��
                '�X�^�[�g������O�ɗ�O�����������ꍇ��A
                '�X�^�[�g��̓d������M�X���b�h��Abort���Ă���ꍇ�Ȃ�
                '���l�������������s���Ă���B
                For Each oClient As Client In oClientList
                    Dim clientThreadState As ThreadState = oClient.Telegrapher.ThreadState
                    If oClient.ChildSteerSock IsNot Nothing AndAlso _
                       oClient.State = ClientState.Registered AndAlso _
                       oClient.Telegrapher.ThreadState <> ThreadState.Unstarted Then
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
                Log.Info("End listening for [" & Config.IpAddrForTelegConnection.ToString() & ":" & Config.IpPortForTelegConnection.ToString() & "].")
            End If
        End Try
    End Sub

    Private Shared Function FindClient(ByVal sName As String) As Client
        For Each oClient As Client In oClientList
            If oClient.Name = sName Then Return oClient
        Next oClient
        Return Nothing
    End Function

    Private Shared Sub RegisterClient(ByVal oSocket As Socket)
        Dim oClient As New Client()
        Dim oRemoteEndPoint As IPEndPoint = DirectCast(oSocket.RemoteEndPoint, IPEndPoint)
        oClient.Name = oRemoteEndPoint.Address.ToString() & "." & oRemoteEndPoint.Port.ToString()
        Log.Info("Incoming from [" & oClient.Name & "].")

        Dim oRcvTeleg As EkDodgyTelegram _
           = oTelegImporter.GetTelegramFromSocket(oSocket, Config.TelegReadingLimitBaseTicks, Config.TelegReadingLimitExtraTicksPerMiB, Config.TelegLoggingMaxLengthOnRead)
        If oRcvTeleg Is Nothing Then
            Log.Info("Closing the connection...")
            oSocket.Close()
            Return
        End If

        Dim headerViolation As NakCauseCode = oRcvTeleg.GetHeaderFormatViolation()
        If headerViolation <> EkNakCauseCode.None Then
            Log.Error("Telegram with invalid HeadPart received.")
            SendNakTelegramThenDisconnect(headerViolation, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.CmdCode <> EkCmdCode.Req Then
            Log.Error("Telegram with invalid CmdCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.SubCmdCode <> EkSubCmdCode.Get Then
            Log.Error("Telegram with invalid SubCmdCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.ObjCode <> EkComStartReqTelegram.FormalObjCodeInOpClient Then
            Log.Error("Telegram with invalid ObjCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        Dim oRcvComStartReqTeleg As New EkComStartReqTelegram(oRcvTeleg)
        Dim bodyViolation As NakCauseCode = oRcvComStartReqTeleg.GetBodyFormatViolation()
        If bodyViolation <> EkNakCauseCode.None Then
            Log.Error("ComStart REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(bodyViolation, oRcvComStartReqTeleg, oSocket)
            Return
        End If

        Dim clientCode As EkCode = oRcvComStartReqTeleg.ClientCode
        'NOTE: clientCode���K��͈͓����`�F�b�N��������悢��������Ȃ��B

        Log.Info("ComStart REQ received.")

        Dim oldClient As Client = FindClient(oClient.Name)
        If oldClient IsNot Nothing Then
            Log.Warn("Telegrapher [" & oClient.Name & "] is running...")
            QuitTelegrapher(oldClient)
            WaitForTelegraphersToQuit()
            UnregisterDiscardedClients()
        End If

        '���Y�N���C�A���g�����d������M�X���b�h�̈ꎞ��Ɨp�f�B���N�g���̃p�X�𐶐��B
        Dim sClientTempBase As String = Path.Combine(Config.TemporaryBaseDirPath , oClient.Name)

        '���Y�N���C�A���g������FTP�T�C�g�p�f�B���N�g���̃p�X�𐶐��B
        Dim sClientFtpBase As String = Path.Combine(sFtpBase, oClient.Name)

        '������FTP�T�C�g�p�f�B���N�g�������݂��Ă���ꍇ�͍폜����B
        Dim isDirLocked As Boolean = False
        Log.Info("Initializing directory [" & sClientFtpBase & "]...")
        Try
            Directory.Delete(sClientFtpBase, True)
        Catch ex As DirectoryNotFoundException

        Catch ex As IOException
            Log.Error("Exception caught.", ex)
            isDirLocked = True
        Catch ex As UnauthorizedAccessException
            Log.Error("Exception caught.", ex)
            isDirLocked = True
        End Try

        '������FTP�T�C�g�p�f�B���N�g�����폜�ł��Ȃ������i�܂�FTP�T�[�o�������Ă���j
        '�ꍇ�́ANAK�i�r�W�[�j�d����ԐM����B
        If isDirLocked Then
            SendNakTelegramThenDisconnect(EkNakCauseCode.Busy, oRcvComStartReqTeleg, oSocket)
            Return
        End If

        '���Y�N���C�A���g������FTP�T�C�g�p�f�B���N�g�����쐬����B
        Directory.CreateDirectory(sClientFtpBase)

        'ACK�d����ԐM����B
        Dim oReplyTeleg As EkComStartAckTelegram = oRcvComStartReqTeleg.CreateAckTelegram()
        Log.Info("Sending ComStart ACK...")
        If SendReplyTelegram(oSocket, oReplyTeleg, oRcvComStartReqTeleg) = False Then
            Log.Info("Closing the connection...")
            oSocket.Close()
            Return
        End If

        oClient.ChildSteerSock = Nothing
        Dim oChildSock As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oClient.ChildSteerSock, oChildSock)
        oClient.Telegrapher = New MyTelegrapher( _
           oClient.Name, _
           oChildSock, _
           oTelegImporter, _
           oTelegGene, _
           clientCode, _
           sClientTempBase, _
           sClientFtpBase)

        oClientList.AddLast(oClient)
        oClient.State = ClientState.Registered

        Log.Info("Starting telegrapher [" & oClient.Name & "]...")
        oClient.Telegrapher.Start()

        Log.Info("Sending new socket to telegrapher [" & oClient.Name & "]...")
        If ConnectNotice.Gen(oSocket).WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("Sweeping telegrapher [" & oClient.Name & "] because it seems broken...")
            SweepBrokenTelegrapher(oClient)
        End If
    End Sub

    Private Shared Sub SendNakTelegramThenDisconnect(ByVal cause As NakCauseCode, ByVal oSourceTeleg As EkTelegram, ByVal oSocket As Socket)
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

    Private Shared Function SendReplyTelegram(ByVal oSocket As Socket, ByVal oReplyTeleg As EkTelegram, ByVal oSourceTeleg As EkTelegram) As Boolean
        oReplyTeleg.RawReqNumber = oSourceTeleg.RawReqNumber
        oReplyTeleg.RawClientCode = oSourceTeleg.RawClientCode
        Return oReplyTeleg.WriteToSocket(oSocket, Config.TelegWritingLimitBaseTicks, Config.TelegWritingLimitExtraTicksPerMiB, Config.TelegLoggingMaxLengthOnWrite)
    End Function

    Private Shared Sub QuitTelegrapher(ByVal oClient As Client)
        Log.Info("Sending quit request to telegrapher [" & oClient.Name & "]...")
        If QuitRequest.Gen().WriteToSocket(oClient.ChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("Sweeping the telegrapher because it seems broken...")
            SweepBrokenTelegrapher(oClient)
        Else
            oClient.State = ClientState.QuitRequested
        End If
    End Sub

    Private Shared Sub WaitForTelegraphersToQuit()
        'Log.Info("Waiting for telegraphers to quit...")
        Dim oJoinLimitTimer As New TickTimer(Config.TelegrapherPendingLimitTicks)
        oJoinLimitTimer.Start(TickTimer.GetSystemTick())
        For Each oClient As Client In oClientList
            If oClient.State = ClientState.QuitRequested Then
                Dim ticks As Long = oJoinLimitTimer.GetTicksToTimeout(TickTimer.GetSystemTick())
                If ticks < 0 Then ticks = 0

                If oClient.Telegrapher.Join(CInt(ticks)) = False Then
                    Log.Fatal("Sweeping telegrapher [" & oClient.Name & "] because it seems broken...")
                    SweepBrokenTelegrapher(oClient)
                Else
                    Log.Info("Telegrapher [" & oClient.Name & "] has quit.")
                    oClient.ChildSteerSock.Close()
                    oClient.State = ClientState.Discarded
                End If
            End If
        Next oClient
    End Sub

    Private Shared Sub SweepBrokenTelegrapher(ByVal oClient As Client)
        oClient.ChildSteerSock.Close()
        If oClient.Telegrapher.ThreadState <> ThreadState.Stopped Then
            oClient.Telegrapher.Abort()
            If oClient.Telegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                Log.Warn("The telegrapher may refuse to abort.")
            End If
        End If
        oClient.State = ClientState.Discarded
    End Sub

    Private Shared Sub UnregisterDiscardedClients()
        Dim oNode As LinkedListNode(Of Client) = oClientList.First
        While oNode IsNot Nothing
            Dim oClient As Client = oNode.Value
            If oClient.State = ClientState.Discarded Then
                Dim oDiscardedNode As LinkedListNode(Of Client) = oNode
                oNode = oNode.Next
                oClientList.Remove(oDiscardedNode)
                Log.Info("Telegrapher [" & oClient.Name & "] unregistered.")

                'NOTE: FTP�T�[�o�́AServerTelegrpher�Ɩ��֌W�ɓ����Ă���B���Ƃ��΁A
                'ServerTelegrpher���]���I���d���̎�M�Ń^�C���A�E�g�����ꍇ�A
                '�N���C�A���g�����ʐM�菇�Ɉᔽ���Ă��Ȃ��Ă��A���̂Ƃ��ɍs���Ă���
                '�t�@�C���]���́A�܂��p�����Ă���\��������BServerTelegrapher��
                '�d���̃R�l�N�V������ؒf���A�����ClientTelegrapher�����m����
                'FTP�𒆎~���邱�Ƃ͂��邩������Ȃ����A�N���C�A���g���̍�莟���
                '���邵�A���ɒ��~����Ƃ��Ă��A�����m���Ē��~���邩�͑S���킩��Ȃ��B
                '�ȏ�̂Ƃ���ł��邩��A������FTP�T�[�o�̓��Y�N���C�A���g�p
                '�f�B���N�g�����폜���邱�Ƃ͂��Ȃ��B
                '��{�I�ɁA�^�ǒ[���̋N���́A���[�U�̑���ɉ����čs���邱�Ƃ�
                '���邩��A�����f�B���N�g���̐����m��Ă���A�v���Z�X�̍ċN������
                '�폜����Ώ\��...�Ƃ����v�z�ł���B
            Else
                oNode = oNode.Next
            End If
        End While
    End Sub

End Class
