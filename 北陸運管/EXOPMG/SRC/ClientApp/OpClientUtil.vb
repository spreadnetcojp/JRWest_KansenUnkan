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

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

Public Class OpClientUtil
    Private Shared oTelegramGene As EkTelegramGene
    Private Shared oChildSteerSock As Socket
    Private Shared oTelegrapher As OpClientTelegrapher
    Private Shared sPermittedPathInFtp As String

    Public Shared Sub StartTelegrapher()
        Dim sFtpBase As String = Utility.CombinePathWithVirtualPath(Config.FtpWorkingDirPath, Config.PermittedPathInFtp)
        Log.Info("Sweeping directory [" & sFtpBase & "]...")
        Utility.DeleteTemporalDirectory(sFtpBase)

        oTelegramGene = New EkTelegramGeneForNativeModels(Config.FtpWorkingDirPath)
        Dim oMessageSockForTelegrapher As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oChildSteerSock, oMessageSockForTelegrapher)
        oTelegrapher = New OpClientTelegrapher("Telegrapher", oMessageSockForTelegrapher, oTelegramGene)

        Log.Info("Starting the telegrapher...")
        oTelegrapher.Start()

        sPermittedPathInFtp = Nothing
    End Sub

    Public Shared Sub QuitTelegrapher()
        If oTelegrapher IsNot Nothing Then
            Log.Info("Sending quit request to the telegrapher...")
            If QuitRequest.Gen().WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
                Log.Fatal("The telegrapher seems broken.")
            End If

            Log.Info("Waiting for the telegrapher to quit...")
            If oTelegrapher.Join(Config.TelegrapherPendingLimitTicks) = False Then
                Log.Fatal("The telegrapher seems broken.")
                oTelegrapher.Abort()
            End If
        End If

        If oChildSteerSock IsNot Nothing Then
            oChildSteerSock.Close()
        End If

        oTelegramGene = Nothing
        oChildSteerSock = Nothing
        oTelegrapher = Nothing
        sPermittedPathInFtp = Nothing
    End Sub

    ''' <summary>
    ''' [��ꂽTelegrapher�Ɋւ��郊�\�[�X�̉���ƃ��X�^�[�g]
    ''' </summary>
    Public Shared Sub RestartBrokenTelegrapher()
        '�q�X���b�h��Abort��������
        Const TelegrapherAbortLimitTicks As Integer = 5000

        Log.Info("Renewing the telegrapher...")
        oChildSteerSock.Close()

        If oTelegrapher.ThreadState <> System.Threading.ThreadState.Stopped Then
            oTelegrapher.Abort()

            'NOTE: Abort()�̌��ʁATelegrapher�͗�O���L���b�`���ă��O��
            '�o�͂���\��������B�܂��A�����炪Abort()����߂��Ă������_�ŁA
            '���ɗ�O�������J�n����Ă��邱�Ƃ͍Œ���ۏ؂���Ă��Ăق������A
            'msdn���݂��������Ƃ��܂����s���ł��邽�߁A�X���b�h���I����Ԃ�
            '�Ȃ�Ȃ�����́A�ʐM����Ɋւ��邻�̑��̃O���[�o���ȏ����܂��X�V
            '����\��������ƍl����ׂ��ł���B����āA�ł������I����҂���
            '����A�V����Telegrapher���X�^�[�g������B
            If oTelegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                Log.Warn("The telegrapher may refuse to abort.")
            End If
        End If

        sPermittedPathInFtp = Nothing

        'NOTE: �A�v���P�[�V�������ċN�����邱�Ƃŏ����ł��邵�A
        '��Q��͂̃q���g�ɂȂ�\��������̂ŁAFTP��
        '�ꎞ��Ɨp�f�B���N�g���͂��̂܂܂ɂ��Ă����B

        'oChildSteerSock�Ɋւ��āA���݂̎Q�Ɛ��؂藣���A�V����LocalConnection�̈�[���Q�Ƃ�����B
        'oTelegrapher�Ɋւ��āA���݂̎Q�Ɛ��؂藣���A�V����Telegrapher���Q�Ƃ�����B
        Dim oMessageSockForTelegrapher As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oChildSteerSock, oMessageSockForTelegrapher)
        oTelegrapher = New OpClientTelegrapher("Telegrapher", oMessageSockForTelegrapher, oTelegramGene)

        Log.Info("Starting the telegrapher...")
        oTelegrapher.Start()
    End Sub

    'NOTE: �݌v�i�K�ŃR�l�N�V�����L�[�v��z�肵�Ă������߁A�ȉ��̎����͔����ł���B
    '�����A���̂܂܃R�l�N�V�����L�[�v�ɂ��Ȃ��̂ł���΁A������ƕ��G�ɂȂ邪�A
    '�R�l�N�g�����҂���A�z�M���ʑ҂��A�ؒf�҂��̊Ԃ́A���̂��߂̏�Ԃ�
    '�Ǘ�������ŁA���b�Z�[�W���[�v���p����������悢�B
    '�Ɩ��I�ȏ����͊��S�ɂł��Ȃ�����ɂ��Ă��A�E�B���h�E�̈ړ��Ȃ�
    '�͂ł��������悢�̂ŁA�ň��ATelegrapher�Ƃ̈�A�̂��Ƃ��
    '�v�[���X���b�h�ɔC���āA��ʂ��甲����Ƃ��Ȃǂ݂̂ɁA���̏I����
    '�҂Ă΂悢�B

    Public Shared Function Connect() As Boolean
        Dim sServerName As String = Config.ServerIpAddr & "." & Config.IpPortForTelegConnection.ToString()
        Log.Info("Outgoing to [" & sServerName & "]...")

        Dim oTelegSock As Socket
        Try
            oTelegSock = SockUtil.Connect(Config.ServerIpAddr, Config.IpPortForTelegConnection)
        Catch ex As OPMGException
            Log.Error("Exception caught.", ex)
            Return False
        End Try

        Dim oLocalEndPoint As IPEndPoint = DirectCast(oTelegSock.LocalEndPoint, IPEndPoint)
        Dim sClientName As String = oLocalEndPoint.Address.ToString() & "." & oLocalEndPoint.Port.ToString()
        Log.Info("Connection established by [" & sClientName & "] to [" & sServerName & "].")

        sPermittedPathInFtp = Path.Combine(Config.PermittedPathInFtp, sClientName)

        'FTP�Ŏg���ꎞ��Ɨp�f�B���N�g��������������B
        Dim sFtpBase As String = Utility.CombinePathWithVirtualPath(Config.FtpWorkingDirPath, sPermittedPathInFtp)
        Log.Info("Initializing directory [" & sFtpBase & "]...")
        Utility.DeleteTemporalDirectory(sFtpBase)
        Directory.CreateDirectory(sFtpBase)

        Log.Info("Sending new socket to the telegrapher...")
        oTelegrapher.LineStatus = LineStatus.Connected
        If ConnectNotice.Gen(oTelegSock).WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Return True
    End Function

    Public Shared Sub Disconnect()
        Log.Info("Sending disconnect request to the telegrapher...")
        If DisconnectRequest.Gen().WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Dim oTimer As New TickTimer(Config.TelegrapherPendingLimitTicks)
        oTimer.Start(TickTimer.GetSystemTick())
        While oTelegrapher.LineStatus <> LineStatus.Disconnected
            If oTelegrapher.ThreadState = System.Threading.ThreadState.Stopped Then
                Log.Fatal("The telegrapher seems broken.")
                RestartBrokenTelegrapher()
                Throw New OPMGException()
            End If
            If oTimer.GetTicksToTimeout(TickTimer.GetSystemTick()) <= 0 Then
                Log.Fatal("The telegrapher seems broken.")
                RestartBrokenTelegrapher()
                Throw New OPMGException()
            End If
            System.Threading.Thread.Sleep(100)
        End While

        'FTP�Ŏg�����ꎞ��Ɨp�f�B���N�g����Еt����B
        Dim sFtpBase As String = Utility.CombinePathWithVirtualPath(Config.FtpWorkingDirPath, sPermittedPathInFtp)
        sPermittedPathInFtp = Nothing
        Log.Info("Sweeping directory [" & sFtpBase & "]...")
        Utility.DeleteTemporalDirectory(sFtpBase)
    End Sub

    Public Shared Function UllMasProFile(ByVal sFilePath As String) As MasProUllResult
        Dim sFileNameInFtp As String _
           = Path.Combine(sPermittedPathInFtp, Path.GetFileName(sFilePath))

        Dim sDestPath As String _
           = Utility.CombinePathWithVirtualPath(Config.FtpWorkingDirPath, sFileNameInFtp)

        File.Copy(sFilePath, sDestPath, True)

        Log.Info("Sending MasProUllRequest to the telegrapher...")
        If MasProUllRequest.Gen(sFileNameInFtp).WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Log.Info("Waiting for MasProUllResponse from the telegrapher...")
        'NOTE: �ʐM�X���b�h�ňُ킪���������ꍇ�A�ʐM�X���b�h��oChildSteerSock�̑Β[��K���N���[�Y����z��B
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oChildSteerSock, Config.TelegrapherUllLimitTicks)
        If (Not oRcvMsg.HasValue) OrElse (oRcvMsg.Kind <> ClientAppInternalMessageKind.MasProUllResponse) Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Return MasProUllResponse.Parse(oRcvMsg).Result
    End Function

    Public Shared Function InvokeMasProDll(ByVal sListFileName As String, ByVal forcingFlag As Boolean) As MasProDllInvokeResult
        Dim oExt As New MasProDllInvokeRequestExtendPart()
        oExt.ListFileName = sListFileName
        oExt.ForcingFlag = forcingFlag
        Log.Info("Sending MasProDllInvokeRequest to the telegrapher...")
        If MasProDllInvokeRequest.Gen(oExt).WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Log.Info("Waiting for MasProDllInvokeResponse from the telegrapher...")
        'NOTE: �ʐM�X���b�h�ňُ킪���������ꍇ�A�ʐM�X���b�h��oChildSteerSock�̑Β[��K���N���[�Y����z��B
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oChildSteerSock, Config.TelegrapherDllInvokeLimitTicks)
        If (Not oRcvMsg.HasValue) OrElse (oRcvMsg.Kind <> ClientAppInternalMessageKind.MasProDllInvokeResponse) Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Return MasProDllInvokeResponse.Parse(oRcvMsg).Result
    End Function
End Class
