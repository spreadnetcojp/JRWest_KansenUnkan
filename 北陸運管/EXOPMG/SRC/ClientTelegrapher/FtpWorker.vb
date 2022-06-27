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
Imports System.Net.Cache
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' �t�@�C���]���̑S�葱���𗠂Łi�������Ɣ񓯊��Ɂj�s���N���X�B
''' </summary>
''' <remarks>
''' �t�@�C���]���̕��@��FTP�ł���B
''' </remarks>
Public Class FtpWorker
    Inherits Looper
    Implements IXllWorker

#Region "�萔��ϐ�"
    '�T�[�o���t�@�C���̃x�[�XURI
    Private oForeignBaseUri As Uri

    '�T�[�o�̎��i���
    Private oCredential As NetworkCredential

    '�ŏ��̗v�����M�J�n����f�[�^�R�l�N�V�������m������܂ł�
    '�����i�t�@�C���̃_�E�����[�h����A�b�v���[�h���j�܂��́A
    '�ŏ��̗v�����M�J�n����Ō�̉�������M��������܂ł̊���
    '�i����R�l�N�V���������ł��ׂĂ��I��郁�\�b�h�̏ꍇ�j����сA
    '�A�b�v���[�h���FTP��Ԏ擾�̊����B
    Private requestLimitTicks As Integer

    '���O�A�E�g�̎��s����
    'NOTE: �e�X���b�h�́A���Ƃ�CancelTransfer()�����s�����Ƃ��Ă�
    '�������b�Z�[�W��M�܂łɂ́A���ꂾ���i+���j�̎��Ԃ��|�蓾��
    '���Ƃ�z�肵�Ȃ���΂Ȃ�Ȃ��B
    '�܂�AClientTelegrapher��activeXllWorkerPendingLimitTicks
    '��passiveXllWorkerPendingLimitTicks�ɂ́A���̐ݒ�l����
    '�\���ɑ傫�Ȓl��ݒ肷��ׂ��ł���B
    '�Ȃ��AQuitRequest���b�Z�[�W���M����A���ꂾ���i+���j�̎��Ԃ́A
    '���̃X���b�h���c�蓾��B
    Private logoutLimitTicks As Integer

    '�]����~���e����
    Private transferStallLimitTicks As Integer

    '�p�b�V�u���[�h���g�p���邩�ۂ�
    Private usePassiveMode As Boolean

    '�e�X���b�h����̗v�������s���邲�ƂɃ��O�A�E�g���邩�ۂ�
    Private logoutEachTime As Boolean

    '�]���f�[�^�̓ǂݍ��݁i�����o���j�p�o�b�t�@
    Private aBuffer As Byte()

    '�]���L�����Z���̒ʒm�҂��I�u�W�F�N�g
    Private oCancelEvent As ManualResetEvent

    '���O�C�����Ă��邩�ۂ�
    Private isLoggedIn As Boolean
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal sForeignBaseUri As String, _
       ByVal oCredential As NetworkCredential, _
       ByVal requestLimitTicks As Integer, _
       ByVal logoutLimitTicks As Integer, _
       ByVal transferStallLimitTicks As Integer, _
       ByVal usePassiveMode As Boolean, _
       ByVal logoutEachTime As Boolean, _
       Optional ByVal bufferLength As Integer = 1024)
        'NOTE: ���̃��\�b�h�͐e�X���b�h�Ŏ��s����邱�ƂɂȂ�B�����āA
        '�����Łi�e�X���b�h�Łj�����������ϐ��́AMyBase.Start���\�b�h�����s����
        '�ȍ~�A�q�X���b�h�ŎQ�Ƃ���邱�ƂɂȂ�B�������AMyBase.Start���\�b�h��
        '�������o���A�ƂȂ邽�߁A�������͒P��������ōς܂��Ė��Ȃ��B

        MyBase.New(sThreadName, oParentMessageSock)
        Me.oForeignBaseUri = New Uri(sForeignBaseUri)
        Me.oCredential = oCredential
        Me.requestLimitTicks = requestLimitTicks
        Me.logoutLimitTicks = logoutLimitTicks
        Me.transferStallLimitTicks = transferStallLimitTicks
        Me.usePassiveMode = usePassiveMode
        Me.logoutEachTime = logoutEachTime
        Me.aBuffer = New Byte(bufferLength - 1) {}
        Me.oCancelEvent = New ManualResetEvent(False)
        Me.isLoggedIn = False
    End Sub
#End Region

#Region "�e�X���b�h�p���\�b�h"
    Private ReadOnly Property __ThreadState() As ThreadState Implements IXllWorker.ThreadState
        Get
            Return ThreadState
        End Get
    End Property

    Public Overrides Sub Start() Implements IXllWorker.Start
        MyBase.Start()
    End Sub

    Public Overrides Sub Abort() Implements IXllWorker.Abort
        MyBase.Abort()
    End Sub

    Public Sub PrepareTransfer() Implements IXllWorker.PrepareTransfer
        oCancelEvent.Reset()
    End Sub

    Public Sub CancelTransfer() Implements IXllWorker.CancelTransfer
        oCancelEvent.Set()
    End Sub
#End Region

#Region "���\�b�h"
    Protected Overrides Function ProcOnSockReadable(ByVal oSock As Socket) As Boolean
        Debug.Assert(oSock Is oParentMessageSock)
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oSock)
        Select Case oRcvMsg.Kind
            Case InternalMessageKind.DownloadRequest
                Return ProcOnDownloadRequestReceive(oRcvMsg)

            Case InternalMessageKind.UploadRequest
                Return ProcOnUploadRequestReceive(oRcvMsg)

            Case InternalMessageKind.QuitRequest
                Return ProcOnQuitRequestReceive(oRcvMsg)

            Case Else
                Debug.Fail("This case is impermissible.")
                Return True
        End Select
    End Function

    'TODO: Stream��BeginWrite()�͒�~����\���͂Ȃ����H
 
    Protected Overridable Function ProcOnDownloadRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        'NOTE: �ȉ���For�̓����ł�WebException�ȊO�̗�O����������\���͂���B
        '���Ƃ��΁AFileStream�̃R���X�g���N�^�ɓn��������ɕs��������ꍇ�Ȃǂł���B
        '�������A����炪�������Ȃ��悤�ɂ���̂́AIXllWorker�̌Ăь��̐Ӗ��Ƃ���B
        '�����āA�����������s����ӏ��ŗ�O������������A���Ȃ��Ƃ����̃X���b�h��
        '��Ԃ����S�ȏ�Ԃ܂Ŗ߂����Ƃ͔񌻎��I�ł���B����āA���̂悤�ȗ�O��
        '�����ŕߑ����āA���O�A�E�g��A�b�v���[�h�i���~�j�������M���s���̂ł͂Ȃ��A
        '���̂悤�ȗ�O��ProcOnUnhandledException�ŏ������A�ΐe�X���b�h�ʐM�p�\�P�b�g
        '���N���[�Y���邱�ƂŁA�e�X���b�h�ɒʐM�X���b�h�̔j�������o�����邱�Ƃɂ���B
        Dim isOK As Boolean = True
        Dim oExt As DownloadRequestExtendPart = DownloadRequest.Parse(oRcvMsg).ExtendPart
        Dim lastIndex As Integer = oExt.TransferList.Count - 1
        For i As Integer = 0 To lastIndex
            Dim oForeignUri As New Uri(oForeignBaseUri, oExt.TransferList(i))
            Dim sLocalPath As String = Utility.CombinePathWithVirtualPath(oExt.TransferListBase, oExt.TransferList(i))
            Directory.CreateDirectory(Path.GetDirectoryName(sLocalPath))

            Dim oFtpReq As FtpWebRequest = DirectCast(WebRequest.Create(oForeignUri), FtpWebRequest)
            oFtpReq.Credentials = oCredential
            oFtpReq.Method = WebRequestMethods.Ftp.DownloadFile
            oFtpReq.UseBinary = True
            oFtpReq.UsePassive = usePassiveMode
            oFtpReq.CachePolicy = New RequestCachePolicy(RequestCacheLevel.BypassCache)
            oFtpReq.Proxy = Nothing
            oFtpReq.Timeout = Timeout.Infinite
            oFtpReq.ReadWriteTimeout = Timeout.Infinite
            If logoutEachTime Then
                oFtpReq.KeepAlive = False
            Else
                oFtpReq.KeepAlive = If(i = lastIndex, False, True)
            End If

            'NOTE: oFtpReq.EndGetResponse()�ɂ����郊�\�[�X�m�ۂ̌�A����
            '�߂�l�ƂȂ�ׂ����t�@�����X��oFtpRes�ɃZ�b�g����܂ł̊Ԃ�
            '��O����������΁A�m�ۂ��ꂽ���\�[�X��������邱�Ƃ͂ł��Ȃ�
            '���A���L�̑O�񂪂��邽�߁A���Ȃ��B
            '�܂��AEndGetResponse���g�́A���\�[�X���m�ۂ�����A�O���v����
            '�ُ�i�ʐM�ُ퓙�j��������O���X���[����ꍇ�́A�K�����\�[�X��
            '������s���͂��ł���iEndGetResponse�̐Ӗ��ł���j�B
            '���ɁAEndGetResponse()����߂�����A�߂�l��oFtpRes�ɃZ�b�g
            '��������܂ł̊Ԃ��A���̃X���b�h���g�̏����ł́A���Ȃ��Ƃ�
            '�o�O�ȊO�̗v���ŗ�O���X���[����邱�Ƃ͖����͂��ł���B
            '�Ō�ɁA���̃X���b�h�����̃X���b�h�ɑ΂���Abort()�����s����
            '���ƂŁAoFtpReq.EndGetResponse()�ɂ����郊�\�[�X�m�ۂ̌�A����
            '�߂�l�ƂȂ�ׂ����t�@�����X��oFtpRes�ɃZ�b�g�����܂ł̊ԂɁA
            'ThreadAbortException����������P�[�X�ɂ��ẮA���ɂ�����
            '�Ƃ��Ă��A���\�[�X�̉�����s����K�v�͂Ȃ��B�v���Z�X�̑�����
            '�O��Ƃ���Thread.Abort()�̗��p���̂��A������ׂ����Ƃł���
            '���̂悤�ȗ��p���@������Ȃ�A������C������ׂ��ł���B
            '���Ȃ��Ƃ��AThread.Abort()�́A�����v���ُ̈�i�o�O�j�ɑΉ�����
            '���߂̂��̂ł���A�O���v���Ő����鏈���̃L�����Z������������
            '���߂Ɏg�p���Ă͂Ȃ�Ȃ��B
            Dim oFtpRes As FtpWebResponse = Nothing
            Dim oResStream As Stream = Nothing
            Dim oFileStream As FileStream = Nothing
            Try
                Log.Info("Requesting " & oForeignUri.AbsoluteUri & " to get...")

                isLoggedIn = True
                Dim oBegResult As IAsyncResult = oFtpReq.BeginGetResponse(Nothing, Nothing)
                Dim oBegResultAsyncWaitHandle As WaitHandle = oBegResult.AsyncWaitHandle
                Try
                    Dim aWaitHandles() As WaitHandle _
                       = {oBegResultAsyncWaitHandle, oCancelEvent}

                    Dim index As Integer = WaitHandle.WaitAny(aWaitHandles, requestLimitTicks)
                    If index = WaitHandle.WaitTimeout Then
                        Log.Error("Request limit time comes.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If
                    If index = 1 Then
                        Log.Info("Canceled by manager.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If

                    'NOTE: FtpWebRequest�C���X�^���X�iFtpWebRequest�ւ�
                    'DirectCast�����������C���X�^���X�j����GetResponse()��
                    '�擾�����C���X�^���X�͕K��FtpWebResponse�ł��邽�߁A
                    '���L��DirectCast�ŗ�O����������\���͑z�肵�Ȃ��B
                    oFtpRes = DirectCast(oFtpReq.EndGetResponse(oBegResult), FtpWebResponse)
                    Log.Info("Request succeeded.")
                    Log.Info("ftp status: " & oFtpRes.StatusDescription)
                Finally
                    oBegResultAsyncWaitHandle.Close()
                End Try

                Log.Info("Transferring the file...")
                oResStream = oFtpRes.GetResponseStream()
                oFileStream = New FileStream(sLocalPath, FileMode.Create, FileAccess.Write)
                Do
                    Dim oBegReadResult As IAsyncResult _
                       = oResStream.BeginRead(aBuffer, 0, aBuffer.Length, Nothing, Nothing)
                    Dim oBegReadResultAsyncWaitHandle As WaitHandle _
                       = oBegReadResult.AsyncWaitHandle
                    Try
                        Dim aReadWaitHandles() As WaitHandle _
                           = {oBegReadResultAsyncWaitHandle, oCancelEvent}

                        Dim readableIndex As Integer = WaitHandle.WaitAny(aReadWaitHandles, transferStallLimitTicks)
                        If readableIndex = WaitHandle.WaitTimeout Then
                            Log.Error("Transfer stall limit time comes.")
                            oFtpReq.Abort()
                            isOK = False
                            Exit For
                        End If
                        If readableIndex = 1 Then
                            Log.Info("Canceled by manager.")
                            oFtpReq.Abort()
                            isOK = False
                            Exit For
                        End If

                        Dim readSize As Integer = oResStream.EndRead(oBegReadResult)
                        If readSize = 0 Then Exit Do

                        oFileStream.Write(aBuffer, 0, readSize)
                    Finally
                        oBegReadResultAsyncWaitHandle.Close()
                    End Try
                Loop

                Log.Info("Transfer finished.")
                Log.Info("ftp status: " & oFtpRes.StatusDescription)

            Catch ex As WebException
                Log.Error("WebException caught.", ex)
                oFtpReq.Abort()
                isOK = False

                If ex.Response IsNot Nothing Then
                    Dim oExFtpRes As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
                    Log.Error("ftp status: " & oExFtpRes.StatusDescription)
                End If

                '���O�C�������{���Ă��Ȃ����Ƃ��m���ƌ�����P�[�X�ł́A
                'Logout���\�b�h�̎��s�͖��Ӗ��Ȃ̂ŏȗ�����i�t���[��
                '���[�N���Z�b�V�������p�����Ă��Ȃ��󋵂ɂ�����Logout
                '���\�b�h�����s����ƁA���ʂɃ��O�C�����Ă���PWD�����{
                '���A���̃Z�b�V�������烍�O�A�E�g���邱�ƂɂȂ�j�B
                'OPT: �m���ƌ�����P�[�X�͑��ɂ����邩������Ȃ��B
                If i = 0 AndAlso _
                   oFtpRes Is Nothing AndAlso _
                   ex.Status = WebExceptionStatus.ConnectFailure Then
                    isLoggedIn = False
                End If

                Exit For
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                oFtpReq.Abort()
                isOK = False

                If oFtpRes IsNot Nothing Then
                    Log.Error("ftp status: " & oFtpRes.StatusDescription)
                End If

                Exit For
            Finally
                If oFileStream IsNot Nothing Then
                    oFileStream.Close()
                End If

                If oResStream IsNot Nothing Then
                    Try
                        oResStream.Close()
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        isOK = False
                    End Try
                End If

                If oFtpRes IsNot Nothing Then
                    'NOTE: oFtpRes.Close()�����s�����ꍇ�́A�ȉ���
                    'isLoggedIn��ύX�����ɁAExit For����B
                    Try
                        oFtpRes.Close()

                        'NOTE: FTP�̊J�n����]���̒��ŗ�O�����������ꍇ�ł��A
                        'oFtpRes.Close()����������΁A�����͎��s����邪�A
                        '���̂悤�Ȉُ�n�ł��ANot KeepAlive��oFtpReq���瓾��
                        'oFtpRes�ɂ��āAoFtpRes.Close()����������΁A
                        '�K�����O�A�E�g��ԂɂȂ�i���ɃZ�b�V�������ُ�I��
                        '���Ă��邩�A�����łȂ����oFtpRes.Close()�Ő����
                        '�I������j�Ƃ����z��ł���B
                        '�����A�����ŉ��L�̏�������������ꍇ�͊m����
                        '���O�A�E�g��Ԃ̂͂��ł���ALogout���\�b�h��
                        '���s�͖��Ӗ��Ȃ̂ŏȗ�����i�t���[�����[�N��
                        '�Z�b�V�������p�����Ă��Ȃ��󋵂ɂ�����Logout
                        '���\�b�h�����s����ƁA���ʂɃ��O�C�����Ă���
                        'PWD�����{���A���̃Z�b�V�������烍�O�A�E�g����
                        '���ƂɂȂ�j�B
                        If Not oFtpReq.KeepAlive Then
                            isLoggedIn = False
                        End If
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        isOK = False
                    End Try
                End If
            End Try
            If Not isOK Then Exit For
        Next

        If logoutEachTime AndAlso isLoggedIn Then
            Logout()
        End If

        If isOk Then
            DownloadResponse.Gen(DownloadResult.Finished).WriteToSocket(oParentMessageSock)
        Else
            DownloadResponse.Gen(DownloadResult.Aborted).WriteToSocket(oParentMessageSock)
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnUploadRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        'NOTE: �ȉ���For�̓����ł�WebException�ȊO�̗�O����������\���͂���B
        '���Ƃ��΁AFileStream�̃R���X�g���N�^�ɓn��������ɕs��������ꍇ�Ȃǂł���B
        '�������A����炪�������Ȃ��悤�ɂ���̂́AIXllWorker�̌Ăь��̐Ӗ��Ƃ���B
        '�����āA�����������s����ӏ��ŗ�O������������A���Ȃ��Ƃ����̃X���b�h��
        '��Ԃ����S�ȏ�Ԃ܂Ŗ߂����Ƃ͔񌻎��I�ł���B����āA���̂悤�ȗ�O��
        '�����ŕߑ����āA���O�A�E�g��A�b�v���[�h�i���~�j�������M���s���̂ł͂Ȃ��A
        '���̂悤�ȗ�O��ProcOnUnhandledException�ŏ������A�ΐe�X���b�h�ʐM�p�\�P�b�g
        '���N���[�Y���邱�ƂŁA�e�X���b�h�ɒʐM�X���b�h�̔j�������o�����邱�Ƃɂ���B
        '�Ȃ��A�^�ǒ[���v���Z�X�̏ꍇ�A���̂悤�ȂƂ��A�e�X���b�h�́A�ʐM�X���b�h��
        '�ċN�������{���邱�ƂȂ�B�{���Ȃ�A�v���Z�X�S�̂��ċN���������Ƃ���ł��邪�A
        '���̔��f�́A���b�Z�[�W�{�b�N�X�̕\�����݂����[�U�Ɉς˂�B
        Dim isOK As Boolean = True
        Dim oExt As UploadRequestExtendPart = UploadRequest.Parse(oRcvMsg).ExtendPart
        Dim lastIndex As Integer = oExt.TransferList.Count - 1
        For i As Integer = 0 To lastIndex
            Dim oForeignUri As New Uri(oForeignBaseUri, oExt.TransferList(i))
            Dim sLocalPath As String = Utility.CombinePathWithVirtualPath(oExt.TransferListBase, oExt.TransferList(i))
            Dim oFtpReq As FtpWebRequest = DirectCast(WebRequest.Create(oForeignUri), FtpWebRequest)
            oFtpReq.Credentials = oCredential
            oFtpReq.Method = WebRequestMethods.Ftp.UploadFile
            oFtpReq.UseBinary = True
            oFtpReq.UsePassive = usePassiveMode
            oFtpReq.CachePolicy = New RequestCachePolicy(RequestCacheLevel.BypassCache)
            oFtpReq.Proxy = Nothing
            oFtpReq.Timeout = Timeout.Infinite
            oFtpReq.ReadWriteTimeout = Timeout.Infinite
            If logoutEachTime Then
                oFtpReq.KeepAlive = False
            Else
                oFtpReq.KeepAlive = If(i = lastIndex, False, True)
            End If

            'NOTE: oReqStream�́AWebException���X���[���ꂽ�ۂɁA
            '�ǂ̏����ŃX���[���ꂽ�̂��𔻒f���邾���̂��߂ɁA
            '���̃��x���Ő錾���Ă���B
            Dim oReqStream As Stream = Nothing
            Dim oFileStream As FileStream = Nothing
            Try
                oFileStream = New FileStream(sLocalPath, FileMode.Open, FileAccess.Read)

                Log.Info("Requesting " & oForeignUri.AbsoluteUri & " to put...")

                isLoggedIn = True
                Dim oBegResult As IAsyncResult = oFtpReq.BeginGetRequestStream(Nothing, Nothing)
                Dim oBegResultAsyncWaitHandle As WaitHandle = oBegResult.AsyncWaitHandle
                Try
                    Dim aWaitHandles() As WaitHandle _
                       = {oBegResultAsyncWaitHandle, oCancelEvent}

                    Dim index As Integer = WaitHandle.WaitAny(aWaitHandles, requestLimitTicks)
                    If index = WaitHandle.WaitTimeout Then
                        Log.Error("Request limit time comes.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If
                    If index = 1 Then
                        Log.Info("Canceled by manager.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If

                    oReqStream = oFtpReq.EndGetRequestStream(oBegResult)
                    Log.Info("Request succeeded.")
                Finally
                    oBegResultAsyncWaitHandle.Close()
                End Try

                Log.Info("Transferring the file...")
                Do
                    Dim readSize As Integer = oFileStream.Read(aBuffer, 0, aBuffer.Length)
                    If readSize = 0 Then Exit Do

                    Dim oBegWriteResult As IAsyncResult _
                       = oReqStream.BeginWrite(aBuffer, 0, readSize, Nothing, Nothing)
                    Dim oBegWriteResultAsyncWaitHandle As WaitHandle _
                       = oBegWriteResult.AsyncWaitHandle
                    Try
                        Dim aWriteWaitHandles() As WaitHandle _
                           = {oBegWriteResultAsyncWaitHandle, oCancelEvent}

                        Dim writableIndex As Integer = WaitHandle.WaitAny(aWriteWaitHandles, transferStallLimitTicks)
                        If writableIndex = WaitHandle.WaitTimeout Then
                            Log.Error("Transfer stall limit time comes.")
                            oFtpReq.Abort()
                            isOK = False
                            Exit For
                        End If
                        If writableIndex = 1 Then
                            Log.Info("Canceled by manager.")
                            'NOTE: �Ō�܂ŏ������܂Ȃ��ł悢�Ȃ�A���L��Abort()��
                            'Stream���̂�Close()�ɂ��AoReqStream.EndWrite(oBegWriteResult)��
                            '�������鏈���͕s�v�ɂȂ�Ƃ����z��ł���B
                            oFtpReq.Abort()
                            isOK = False
                            Exit For
                        End If

                        oReqStream.EndWrite(oBegWriteResult)
                    Finally
                        oBegWriteResultAsyncWaitHandle.Close()
                    End Try
                Loop
                Log.Info("Transfer finished.")
            Catch ex As WebException
                Log.Error("WebException caught.", ex)
                oFtpReq.Abort()
                isOK = False

                If ex.Response IsNot Nothing Then
                    Dim oExFtpRes As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
                    Log.Error("ftp status: " & oExFtpRes.StatusDescription)
                End If

                '���O�C�������{���Ă��Ȃ����Ƃ��m���ƌ�����P�[�X�ł́A
                'Logout���\�b�h�̎��s�͖��Ӗ��Ȃ̂ŏȗ�����i�t���[��
                '���[�N���Z�b�V�������p�����Ă��Ȃ��󋵂ɂ�����Logout
                '���\�b�h�����s����ƁA���ʂɃ��O�C�����Ă���PWD�����{
                '���A���̃Z�b�V�������烍�O�A�E�g���邱�ƂɂȂ�j�B
                'OPT: �m���ƌ�����P�[�X�͑��ɂ����邩������Ȃ��B
                If i = 0 AndAlso _
                   oReqStream Is Nothing AndAlso _
                   ex.Status = WebExceptionStatus.ConnectFailure Then
                    isLoggedIn = False
                End If

                Exit For
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                oFtpReq.Abort()
                isOK = False
                Exit For
            Finally
                If oFileStream IsNot Nothing Then
                    oFileStream.Close()
                End If

                If oReqStream IsNot Nothing Then
                    Try
                        oReqStream.Close()
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        isOK = False
                    End Try
                End If
            End Try
            If Not isOK Then Exit For

            'OPT: �Ō�̃t�@�C���ɂ��ẮA�ȉ���Not KeepAlive��
            'FtpWebRequest����FtpWebResponse���쐬���A��������
            '���ƂŁA���O�A�E�g���邱�Ƃ��Ӑ}���Ă��邪�A
            '���Ȃ��Ƃ��Ō�̃t�@�C���ȊO�́A�ȉ����s���K�v��
            '�Ȃ��i�p�t�H�[�}���X���l������Ȃ�s���ׂ��łȂ��j
            '����������B
            Dim oFtpRes As FtpWebResponse = Nothing
            Try
                Log.Info("Requesting ftp status...")
                Dim oBegResult As IAsyncResult = oFtpReq.BeginGetResponse(Nothing, Nothing)
                Dim oBegResultAsyncWaitHandle As WaitHandle = oBegResult.AsyncWaitHandle
                Try
                    Dim aWaitHandles() As WaitHandle _
                       = {oBegResultAsyncWaitHandle, oCancelEvent}

                    'NOTE: �Ō�̑ҋ@����Ȃ̂ŁA�킴�킴requestLimitTicks���w�肷��
                    '���ƂɃ����b�g�͂Ȃ��悤�Ɏv���邪�A�e�X���b�h���^�C�}��������
                    '����Ă���i����Cancel���s���j�Ƃ͌���Ȃ����߁A
                    'requestLimitTicks���w�肷�邱�Ƃɂ���B
                    Dim index As Integer = WaitHandle.WaitAny(aWaitHandles, requestLimitTicks)
                    If index = WaitHandle.WaitTimeout Then
                        Log.Error("Request limit time comes.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If
                    If index = 1 Then
                        Log.Info("Canceled by manager.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If

                    'NOTE: FtpWebRequest�C���X�^���X�iFtpWebRequest�ւ�
                    'DirectCast�����������C���X�^���X�j����GetResponse()��
                    '�擾�����C���X�^���X�͕K��FtpWebResponse�ł��邽�߁A
                    '���L��DirectCast�ŗ�O����������\���͑z�肵�Ȃ��B
                    oFtpRes = DirectCast(oFtpReq.EndGetResponse(oBegResult), FtpWebResponse)
                    Log.Info("Request succeeded.")
                    Log.Info("ftp status: " & oFtpRes.StatusDescription)
                Finally
                    oBegResultAsyncWaitHandle.Close()
                End Try
            Catch ex As WebException
                Log.Error("WebException caught.", ex)
                oFtpReq.Abort()
                isOK = False

                If ex.Response IsNot Nothing Then
                    Dim oExFtpRes As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
                    Log.Error("ftp status: " & oExFtpRes.StatusDescription)
                End If

                Exit For
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                oFtpReq.Abort()
                isOK = False

                'NOTE: ���̏󋵂ł�oFtpRes��Nothing�ł���Ǝv���邵�A
                '������Nothing�łȂ��Ƃ��Ă��AClose�ς݂ł���Ǝv����
                '���߁AoFtpRes.StatusDescription�̃��O�o�͂́A
                '��ɍs��Ȃ��B

                Exit For
            Finally
                If oFtpRes IsNot Nothing Then
                    'NOTE: oFtpRes.Close()�����s�����ꍇ�́A�ȉ���
                    'isLoggedIn��ύX�����ɁAExit For����B
                    Try
                        oFtpRes.Close()

                        'NOTE: FTP�̊J�n����]���̒��ŗ�O�����������ꍇ�ł��A
                        'oFtpRes.Close()����������΁A�����͎��s����邪�A
                        '���̂悤�Ȉُ�n�ł��ANot KeepAlive��oFtpReq���瓾��
                        'oFtpRes�ɂ��āAoFtpRes.Close()����������΁A
                        '�K�����O�A�E�g��ԂɂȂ�i���ɃZ�b�V�������ُ�I��
                        '���Ă��邩�A�����łȂ����oFtpRes.Close()�Ő����
                        '�I������j�Ƃ����z��ł���B
                        '�����A�����ŉ��L�̏�������������ꍇ�͊m����
                        '���O�A�E�g��Ԃ̂͂��ł���ALogout���\�b�h��
                        '���s�͖��Ӗ��Ȃ̂ŏȗ�����i�t���[�����[�N��
                        '�Z�b�V�������p�����Ă��Ȃ��󋵂ɂ�����Logout
                        '���\�b�h�����s����ƁA���ʂɃ��O�C�����Ă���
                        'PWD�����{���A���̃Z�b�V�������烍�O�A�E�g����
                        '���ƂɂȂ�j�B
                        If Not oFtpReq.KeepAlive Then
                            isLoggedIn = False
                        End If
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                        isOK = False
                    End Try
                End If
            End Try
            If Not isOK Then Exit For
        Next

        If logoutEachTime AndAlso isLoggedIn Then
            Logout()
        End If

        If isOk Then
            UploadResponse.Gen(UploadResult.Finished).WriteToSocket(oParentMessageSock)
        Else
            UploadResponse.Gen(UploadResult.Aborted).WriteToSocket(oParentMessageSock)
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnQuitRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Quit requested by manager.")

        If isLoggedIn Then
            Logout()
        End If

        If oParentMessageSock IsNot Nothing Then
            UnregisterSocket(oParentMessageSock)
            oParentMessageSock.Close()
            oParentMessageSock = Nothing
        End If

        If oCancelEvent IsNot Nothing Then
            oCancelEvent.Close()
        End If

        Return False
    End Function

    Protected Overrides Sub ProcOnUnhandledException(ByVal ex As Exception)
        'NOTE: �e�X���b�h�Ȃǂ���Abort���Ă΂ꂽ�ꍇ�����������s�����
        '���߁A�����ł̓���͗\���ł��Ȃ��ƍl����ׂ��ł���B
        '����āA�T�[�o���̃��\�[�X���l����ƁALogout()���s���Ă�������
        '�Ƃ���ł��邪�A����͒��߂�B
        '�������ɃT�[�o���̂��Ƃ͐S�z�ɂȂ邪�A���̃X���b�h���g�̗�O��
        '�����ɓ��B���邱�Ƃ́A�ǂ����̎����ɖ�肪�Ȃ�����A���蓾�Ȃ�
        '�͂��ł��邵�A�T�[�o�����Z���^�C�}��FTP�̃Z�b�V�������������
        '�ݒ�ɂȂ��Ă���͂��ł���A�����炭���ɂ͂Ȃ�Ȃ��B
        '���ꂪ���ɂȂ�O�ɁA�[�����ŗ\�����ʗ�O���������邱�Ǝ��̂�
        '���ɂȂ�A�C�������͂��ł���B

        'NOTE: ����́AIXllWorker�����N���X�̐Ӗ��Ƃ���B
        '�e�X���b�h�́A���̃N���[�Y�ɂ���āA�ُ�����m����B
        If oParentMessageSock IsNot Nothing Then
            UnregisterSocket(oParentMessageSock)
            oParentMessageSock.Close()
            oParentMessageSock = Nothing
        End If

        If oCancelEvent IsNot Nothing Then
            oCancelEvent.Close()
        End If
    End Sub

    Private Sub Logout()
        Dim oFtpReq As FtpWebRequest = DirectCast(WebRequest.Create(oForeignBaseUri), FtpWebRequest)
        oFtpReq.Credentials = oCredential
        oFtpReq.Method = WebRequestMethods.Ftp.PrintWorkingDirectory
        oFtpReq.KeepAlive = False
        oFtpReq.Timeout = logoutLimitTicks
        oFtpReq.Proxy = Nothing
        Try
            Dim oRes As WebResponse = oFtpReq.GetResponse()
            oRes.Close()
        Catch ex As WebException
            'NOTE: ���̃��\�b�h���͎̂��s���Ă��邪�A
            '���������s�����Ƃ������Ƃ́A�����������O�C�������s���Ă��邩�A
            '���O�C��������������ɉ�����؂ꂽ�P�[�X�Ǝv����̂ŁA
            '���̃��\�b�h���������Ȃ��Ă��A���͂Ȃ��Ǝv����B
            Log.Error("WebException caught.", ex)
            oFtpReq.Abort()

            If ex.Response IsNot Nothing Then
                Dim oExFtpRes As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
                Log.Error("ftp status: " & oExFtpRes.StatusDescription)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            oFtpReq.Abort()
        End Try

        isLoggedIn = False
    End Sub

#End Region

End Class
