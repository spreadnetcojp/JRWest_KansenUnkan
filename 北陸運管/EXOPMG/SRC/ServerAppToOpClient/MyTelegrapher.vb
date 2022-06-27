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

Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Messaging
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �^�ǒ[���Ɠd���̑���M���s���N���X�B
''' </summary>
Public Class MyTelegrapher
    Inherits ServerTelegrapher

#Region "�����N���X��"
    Protected Structure DllIdentifier
        Public DataApplicableModel As String
        Public DataPurpose As String
        Public DataKind As String
        Public DataSubKind As String
        Public DataVersion As String
        Public ListVersion As String
    End Structure
#End Region

#Region "�萔��ϐ�"
    '�e��e�[�u�����ʂ̍��ڂɃZ�b�g����l
    Protected Const UserId As String = "System"
    Protected Const MachineId As String = "Server"

    '�Ď��Ղ���D�@��CAB�Ɋ܂܂��o�[�W�����Ǘ��Ώۖژ^�̃t�@�C����
    Protected Const CatalogNameInCabDir As String = "FILELIST.TXT"

    'DLL��ԃe�[�u����TAS����ۂ�SyncLock�p�I�u�W�F�N�g
    'NOTE: �e�[������̓o�^��z�M�w���̎󂯓���́A���Y�f�[�^�Ɋւ���DLL��Ԃ�
    '�u�z�M���v�ȊO�̏ꍇ�̂݉\�Ƃ��邪�ADLL��ԃe�[�u���̂����郌�R�[�h��
    '�ւ���i���̂悤�ȖړI�́j�`�F�b�N��`�F�b�N�`�Z�b�g�́A���̃I�u�W�F�N�g��
    'SyncLock������Ԃōs���BDLL��ԃe�[�u���̔z�M���ʗ���u�z�M���v�ɕύX����
    '�̂́A���̃v���Z�X�݂̖̂����ł���A�X���b�h���������ۂɁA���̃I�u�W�F�N�g
    '���̂�SyncLock���ꂽ�܂܂ɂȂ�悤�Ȃ��Ƃ��Ȃ��i.NET Framework���ۏ�
    '���Ă���͂��ł���j���߁ADB�̃��R�[�h���b�N�@�\�͗p����܂ł��Ȃ��B
    '�Ȃ��ADLL��Ԃ��u�z�M���v���瑼�̏�ԂɕύX����Ӗ��𕉂����X���b�h��
    '�������ہA�ǂ�����ĐӖ���S�����邩�́A�ʖ��Ƃ��čl���Ȃ���΂Ȃ�Ȃ��B
    '���̃X���b�h����~��A������������p�����Ă��郊�X�i�[�X���b�h���Ӗ���
    '�����p���̂��P�̗��z�`�ł��邪�A����͂Ȃ��Ȃ���ςł���i�Ӗ��𕉂���
    '�����X���b�h�𔻕ʂ��邽�߂̍��ڂ�DLL��ԃe�[�u����ɗp�ӂ��A�d������M
    '�X���b�h���������ۂ́A�������X���b�h���L�[��DLL��ԃe�[�u���̍s���T�[�`
    '���Ȃ���΂Ȃ�Ȃ��j�B����āA���̃X���b�h���g��ProcOnUnhandledException
    '�ŁA���ɖ߂����Ƃɂ���B
    Protected Shared ReadOnly oDllStateTableTasLockObject As New Object()

    '��L���������邽�߁ADLL��ԃe�[�u���̉��炩�̃��R�[�h���u�z�M���v��
    '�ύX����ꍇ�́A���O��curDll��isLockingDllStateRecord�ɂ��̎|��
    '�L�^���Ă�����{����i�W���[�i�����O�����̏����j�B
    '�����̓r����Abort��������ꍇ�̂��Ƃ�z�肵�A���ۂ̎��s������
    '�ς��Ȃ��悤�Ƀ������o���A������ŏ������s���B
    '��̓I�ɂ́A���L�̏����ŏ������s���B
    '(1) �ύX���郌�R�[�h�����ʂ��邽�ߒl��curDll�ɃZ�b�g����B
    '(2) WriteBarrier
    '(3) isLockingDllStateRecord��True���Z�b�g����B
    '(4) WriteBarrier
    '(5) ���ۂɃ��R�[�h���u�z�M���v�ɕύX����B
    'ProcOnUnhandledException��isLockingDllStateRecord��True�̏ꍇ�́A
    '�L�^���Ă�������񂩂烌�R�[�h����肵�A�u�ُ�v�ɕύX����B
    '(3)��������(5)�����܂ł̊ԂɁi�u�z�M���v�ɕύX����O�ɁjAbort
    '������ꂽ�ꍇ�A���ۂɂ��̃X���b�h�͓��Y���R�[�h�̕ύX�����l������
    '�킯�łȂ��ɂ�������炸�AProcOnUnhandledException�Łu�ُ�v��
    '�ύX���Ă��܂����ƂɂȂ�B�������A(3)���������Ă���
    'ProcOnUnhandledException�Łu�ُ�v�ɕύX����܂ł̊ԂɁA����
    '�X���b�h�����Y���R�[�h��ύX���Ă���\���͋ɂ߂ĒႢ��A
    '���������X���b�h��Abort��������Ƃ������Ǝ��́A�����Ă�
    '�Ȃ�Ȃ��ُ펖�Ԃł��邩��A����͋��e����B
    Protected curDll As DllIdentifier
    Protected isLockingDllStateRecord As Boolean = False

    '�ꎞ��Ɨp�f�B���N�g����
    Protected sTempDirPath As String

    '�d������
    Protected oTelegGene As EkTelegramGene

    '���葕�u�̑��u�R�[�h
    'NOTE: ProcOnReqTelegramReceive()���t�b�N���Ď�M�d����ClientCode�Ɣ�r���Ă��悢�B
    Protected clientCode As EkCode

    '�A�N�Z�X��������p�X
    Protected sPermittedPath As String

    '���ɑ��M����REQ�d���̒ʔ�
    Protected reqNumberForNextSnd As Integer

    '���Ɏ�M����REQ�d���̒ʔ�
    'NOTE: ProcOnReqTelegramReceive()���t�b�N���āA��M����REQ�d���̒ʔԂ�
    '�A���������`�F�b�N����Ȃ�p�ӂ���B
    'Protected reqNumberForNextRcv As Integer

    'NOTE: �u�Ӑ}�I�Ȑؒf�v�Ɓu�ُ�ɂ��ؒf�v����ʂ������Ȃ�΁A
    'Protected needConnection As Boolean��p�ӂ��A
    'ProcOnConnectNoticeReceive()��ProcOnDisconnectRequestReceive()���t�b�N����
    '�����ON/OFF����Ƃ悢�BProcOnConnectionDisappear()�ł́A������݂āA
    '�J�ڐ�̉����Ԃ����߂邱�Ƃ��ł���B

    '������
    Private _LineStatus As Integer
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal oTelegImporter As EkTelegramImporter, _
       ByVal oTelegGene As EkTelegramGene, _
       ByVal clientCode As EkCode, _
       ByVal sTempPath As String, _
       ByVal sPermittedPath As String)

        MyBase.New(sThreadName, oParentMessageSock, oTelegImporter)
        Me.sTempDirPath = sTempPath
        Me.oTelegGene = oTelegGene
        Me.clientCode = clientCode
        Me.sPermittedPath = sPermittedPath
        Me.reqNumberForNextSnd = 0
        Me.LineStatus = LineStatus.Initial

        Me.oWatchdogTimer.Renew(Config.WatchdogIntervalTicks)
        Me.telegReadingLimitBaseTicks = Config.TelegReadingLimitBaseTicks
        Me.telegReadingLimitExtraTicksPerMiB = Config.TelegReadingLimitExtraTicksPerMiB
        Me.telegWritingLimitBaseTicks = Config.TelegWritingLimitBaseTicks
        Me.telegWritingLimitExtraTicksPerMiB = Config.TelegWritingLimitExtraTicksPerMiB
        Me.telegLoggingMaxLengthOnRead = Config.TelegLoggingMaxLengthOnRead
        Me.telegLoggingMaxLengthOnWrite = Config.TelegLoggingMaxLengthOnWrite
        Me.enableXllStrongExclusion = Config.EnableXllStrongExclusion
        Me.enableActiveSeqStrongExclusion = Config.EnableActiveSeqStrongExclusion
        Me.enableActiveOneOrdering = Config.EnableActiveOneOrdering
    End Sub
#End Region

#Region "�v���p�e�B"
    '���̃v���p�e�B�́A�e�X���b�h�ɂ����Ă��A����u�ԁi�����ߋ��̏u�ԁj��
    '�����Ԃ����[�U�ɕ\������ړI�ł���΁A�Q�Ɖ\�ł���B
    '���̃X���b�h�����̉ӏ��Œ�~��������ŏ�Ԃ��擾����i���̌�A����
    '�X���b�h�ɑ΂��ĔC�ӂ̑�����s���Ă���A�C�ӂɍĊJ�����邱�Ƃ��ł���j
    '�킯�ł͂Ȃ����߁A�Ăяo������߂������_�ŁA�߂�l�̉����Ԃ��ێ�
    '����Ă���Ƃ͌���Ȃ��B���̂����A���p�x�ŌĂяo���Ă��債�����ׂ�
    '������Ȃ��B�܂��A��x�u�ؒf�v�ɂȂ�����A�e�X���b�h���V�����\�P�b�g��
    '�n���Ȃ����葼�̏�Ԃɕω����Ȃ����Ƃ��l������΁A�e�X���b�h������
    '�X���b�h�𐧌䂷���ł����p�ł���P�[�X�͂���B
    Public Property LineStatus() As LineStatus
        'NOTE: Interlocked�N���X��Read���\�b�h�Ɋւ���msdn�̉����ǂނƁA
        '32�r�b�g�ϐ�����̒l�̓ǂݎ���Interlocked�N���X�̃��\�b�h���g��
        '�܂ł��Ȃ��s���ł���i�S�̂�ǂݎ�邽�߂̃o�X�I�y���[�V�������A
        '���̃R�A�ɂ��o�X�I�y���[�V�����ɕ��f����邱�Ƃ��Ȃ��j���Ƃ�
        '�ۏ؂���Ă���悤�ɂ������A���ۂ�Integer�������Ƃ���Read���\�b�h��
        '�p�ӂ���Ă��Ȃ��B�����ł́A�Ƃ肠����Interlocked.Add�iLOCK: XADD?�j
        '���p���Ă��邪�A��ʓI�ɍl���āAInterlocked�N���X��
        '�uRead�������o���A+�P�Ƃ�32bit���[�h���߁v�Ŏ������ꂽ�i�����I��
        'VolatileRead�����́jRead���\�b�h���p�ӂ����ׂ��ł���A�����A
        '���ꂪ�p�ӂ��ꂽ��A����ɕύX���������悢�B�Ȃ��AVolatileRead��
        '�g�p���Ȃ��̂́AServerTelegrapher�Ō��߂����j�ł���B���j�̏ڍׂ�
        'ServerTelegrapher.LastPulseTick�̃R�����g���Q�ƁB
        Get
            Return DirectCast(Interlocked.Add(_LineStatus, 0), LineStatus)
        End Get

        Protected Set(ByVal status As LineStatus)
            Interlocked.Exchange(_LineStatus, status)
        End Set
    End Property
#End Region

#Region "�C�x���g�������\�b�h"
    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)
        Select Case oRcvTeleg.SubCmdCode
            Case EkSubCmdCode.Get
                Select Case oRcvTeleg.ObjCode
                    Case EkMasProDllInvokeReqTelegram.FormalObjCode
                        Return ProcOnMasProDllInvokeReqTelegramReceive(oRcvTeleg)
                End Select
        End Select
        Return MyBase.ProcOnPassiveOneReqTelegramReceive(oRcvTeleg)
    End Function

    Protected Overridable Function ProcOnMasProDllInvokeReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkMasProDllInvokeReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("MasProDllInvoke REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Dim sListFileName As String = Path.GetFileName(oRcvTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error("MasProDllInvoke REQ with invalid ListFileName received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.NoData, oRcvTeleg)
            Return True
        End If

        sListFileName = EkMasProListFileName.Normalize(sListFileName)
        Log.Info("MasProDllInvoke REQ about [" & sListFileName & "] received.")

        curDll.DataApplicableModel = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        curDll.DataPurpose = EkMasProListFileName.GetDataPurpose(sListFileName)
        curDll.DataKind = EkMasProListFileName.GetDataKind(sListFileName)
        curDll.DataSubKind = EkMasProListFileName.GetDataSubKind(sListFileName)
        curDll.DataVersion = EkMasProListFileName.GetDataVersion(sListFileName)
        curDll.ListVersion = EkMasProListFileName.GetListVersion(sListFileName)
        Thread.MemoryBarrier()

        Dim nakCause As NakCauseCode = EkNakCauseCode.None
        Do
            SyncLock oDllStateTableTasLockObject
                Try
                    '���Y��ʁE���Y�o�[�W�����̃��X�g��f�[�^���o�^����Ă��Ȃ���΁A
                    'NAK�iNO DATA�j��ԐM����B
                    If Not IsCurDllObjectRegistered() Then
                        Log.Error("DLL objects are not registered.")
                        nakCause = EkNakCauseCode.NoData
                        Exit Do
                    End If

                    '���싖�������ߋ����ƂȂ�K�p�����P�ł��܂܂��
                    '�ꍇ�́ANAK�iINVALID CONTENT�j��ԐM����B
                    If curDll.DataPurpose = EkConstants.DataPurposeProgram AndAlso _
                       Not IsCurDllApplyingRunnableProgram(sListFileName) Then
                        Log.Error("The list contains adaptible date that is earlier than runnable date.")
                        nakCause = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    Dim dllStartTime As DateTime = DateTime.Now

                    '�V���Ȕz�M���s�v�Ȃ��NAK�iUNNECESSARY�j��ԐM����B
                    If Not IsCurDllNecessary(sListFileName, dllStartTime) Then
                        Log.Info("There is no client in the list file.")
                        nakCause = EkNakCauseCode.Unnecessary
                        Exit Do
                    End If

                    '�@��\���}�X�^��L���łȂ����@���K�p�ΏۂɂȂ��Ă���
                    '�ꍇ�́ANAK�iINVALID CONTENT�j��ԐM����B
                    'NOTE: �P�̓K�p���X�g���g���܂킵�Ă���Œ��ɁA����
                    '�z�M���ς܂��Ă��鍆�@���@��\������O�����Ƃ��ł���悤�A
                    '�V���Ȕz�M��ɂȂ鍆�@�݂̂��`�F�b�N�Ώۂɂ��Ă���B
                    '�K�p���X�g���̑S�K�p���i9999/99/99���͏����j���r�Ώ�
                    '�ɂ��铮�싖���`�F�b�N�Ƃ̓|���V�[���قȂ邪�A
                    '�@��\���}�X�^�Ɠ��싖���i�v���O�����{�́j�ł́A
                    '�v���O�����K�p���X�g�Ƃ̌��т��̋������قȂ�̂�
                    '���R�ł���A�Ƃ肠�������ł͂Ȃ����̂Ƃ���B
                    If Not IsCurDllConsistentWithMachineMaster(sListFileName, dllStartTime) Then
                        Log.Error("There is invalid client in the list file.")
                        nakCause = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '���Y��ʁE���Y�o�[�W�����̃f�[�^�ɂ��āu�z�M���v��u�o�^���v��
                    '���@������Ȃ�ANAK�iBUSY�j��ԐM����B
                    'NOTE: �u�o�^���v�̍��@������ꍇ�Ƃ́ADLL��ԃe�[�u����
                    '�z�M���ʂ��u�z�M���v�̃_�~�[���@�����݂���ꍇ�̂��Ƃł���B
                    If IsCurDllStatusBusyToStart() Then
                        Log.Info("DLL status is busy.")
                        nakCause = EkNakCauseCode.Busy
                        Exit Do
                    End If

                    '���Y��ʁE���Y�o�[�W�����E���Y���@�̔z�M���ʂ��u�z�M���v�ɕύX���āA
                    '���Y�̒ʐM�v���Z�X�ɔz�M�w�����b�Z�[�W�𑗐M����B
                    StartCurDll(sListFileName, dllStartTime, oRcvTeleg.ForcingFlag)

                Catch ex As DatabaseException
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    nakCause = EkNakCauseCode.TelegramError 'NOTE: ����
                    If isLockingDllStateRecord Then
                        'NOTE: ConnectClose�ŗ�O�����������ꍇ�ł���AAbort
                        '�����Ƃ���ŁA�u�z�M���v����ύX���邱�Ƃ��ł��Ȃ�
                        '�\���͍������A�ł���\��������̂ł���Ă����B
                        'NOTE: �{���Ȃ�A���[�U�ւُ̈�ʒm�Ɋւ��āA
                        '���W�f�[�^��L�e�[�u���ւ̓o�^�����łȂ��A�ԈႢ�Ȃ�
                        '�@�\����i�ʖڂȂ��̂Ɉˑ����Ȃ��j���[���x����
                        '�d�g�݂�p�ӂ���ׂ��ł���B
                        '���Ƃ��΁A���W�f�[�^��L�o�^�@�\���ˑ�����S�X���b�h��
                        '�u�ŏI�I�Ƀ��[�U�Ɉُ��\������P���ȋ@��v�ɑ΂��āA
                        '�����I�ɐ�����ʒm���郋�[����݂���Ȃǂł���B
                        '�Ȃ��ASNMP TRAP�́A���W�f�[�^��L�e�[�u�������[�U��
                        '�\���I�Ƀ|�[�����O����Ԃ𖄂߂�i�ʒm�����������
                        '�����Ɉُ��m�邱�Ƃ��ł��āA������Ɗ������j�Ƃ���
                        '�Ӌ`�͂�����̂́A���̂悤�ȗv���͖������Ȃ��B
                        '�u�\�Ȃ�v�ُ��o�^����i�ʒm����j�Ƃ����_�ŁA
                        '���W�f�[�^��L�o�^�Ƒ哯���ق̋@�\�ł���B
                        Abort()
                    Else
                        Exit Do
                    End If
                End Try
            End SyncLock
        Loop While False

        If nakCause <> EkNakCauseCode.None Then
            SendNakTelegramThenDisconnect(nakCause, oRcvTeleg)
            Return True
        End If

        Dim oReplyTeleg As EkMasProDllInvokeAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending MasProDllInvoke ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    Protected Overrides Function CreateWatchdogReqTelegram() As IReqTelegram
        Return New EkWatchdogReqTelegram(oTelegGene, EkWatchdogReqTelegram.FormalObjCodeInOpClient, Config.WatchdogReplyLimitTicks)
    End Function

    '�w�b�_���̓��e���󓮓IULL��REQ�d���̂��̂ł��邩���肷�郁�\�b�h
    Protected Overrides Function IsPassiveUllReq(ByVal iTeleg As ITelegram) As Boolean
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        Return oTeleg.SubCmdCode = EkSubCmdCode.Get AndAlso _
               oTeleg.ObjCode = EkClientDrivenUllReqTelegram.FormalObjCodeAsOpClientFile
    End Function

    '�n���ꂽ�d���C���X�^���X��K�؂Ȍ^�̃C���X�^���X�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsPassiveUllReq(ByVal iTeleg As ITelegram) As IXllReqTelegram
        Return New EkClientDrivenUllReqTelegram(iTeleg, Config.OpClientFileUllTransferLimitTicks)
    End Function

    '�󓮓IULL�̏����i�\�����ꂽ�t�@�C���̎󂯓���m�F�j���s�����\�b�h
    Protected Overrides Function PrepareToStartPassiveUll(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePathInFtp As String = oXllReqTeleg.FileName
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, sFilePathInFtp)

        'NOTE: �u..\�v���̍����������Ȃ�AsPermittedPath��sFilePath�����K�����������悢��������Ȃ��B
        If Not Utility.IsAncestPath(sPermittedPath, sFilePath) Then
            Log.Error("The telegram specifies illegal path [" & sFilePathInFtp & "].")
            Return EkNakCauseCode.NotPermit 'NOTE: ����
        End If

        Dim sFileName As String = Path.GetFileName(sFilePath)
        If EkMasProListFileName.IsValid(sFileName) Then
            curDll.DataApplicableModel = EkMasProListFileName.GetDataApplicableModel(sFileName)
            curDll.DataPurpose = EkMasProListFileName.GetDataPurpose(sFileName)
            curDll.DataKind = EkMasProListFileName.GetDataKind(sFileName)
            curDll.DataSubKind = EkMasProListFileName.GetDataSubKind(sFileName)
            curDll.DataVersion = EkMasProListFileName.GetDataVersion(sFileName)
            curDll.ListVersion = Nothing
        ElseIf EkMasterDataFileName.IsValid(sFileName) Then
            curDll.DataApplicableModel = EkMasterDataFileName.GetApplicableModel(sFileName)
            curDll.DataPurpose = EkConstants.DataPurposeMaster
            curDll.DataKind = EkMasterDataFileName.GetKind(sFileName)
            curDll.DataSubKind = EkMasterDataFileName.GetSubKind(sFileName)
            curDll.DataVersion = EkMasterDataFileName.GetVersion(sFileName)
            curDll.ListVersion = Nothing
        ElseIf EkProgramDataFileName.IsValid(sFileName) Then
            curDll.DataApplicableModel = EkProgramDataFileName.GetApplicableModel(sFileName)
            curDll.DataPurpose = EkConstants.DataPurposeProgram
            curDll.DataKind = EkProgramDataFileName.GetKind(sFileName)
            curDll.DataSubKind = EkProgramDataFileName.GetSubKind(sFileName)
            curDll.DataVersion = EkProgramDataFileName.GetVersion(sFileName)
            curDll.ListVersion = Nothing
        Else
            Log.Error("The telegram specifies invalid file name [" & sFileName & "].")
            Return EkNakCauseCode.InvalidContent
        End If
        Thread.MemoryBarrier()

        SyncLock oDllStateTableTasLockObject
            Try
                '���Y��ʁE���Y�o�[�W�����̃f�[�^�ɂ��āu�z�M���v��u�o�^���v��
                '���@������Ȃ�ANAK�iBUSY�j��ԐM����B
                'NOTE: �u�o�^���v�̍��@������ꍇ�Ƃ́ADLL��ԃe�[�u����
                '�z�M���ʂ��u�z�M���v�̃_�~�[���@�����݂���ꍇ�̂��Ƃł���B
                If IsCurDllStatusBusyToRegister() Then
                    Log.Info("I am busy to accept a file [" & sFileName & "].")
                    Return EkNakCauseCode.Busy
                End If

                Log.Info("Accepting a file [" & sFileName & "]...")

                '���Y��ʁE���Y�o�[�W�����̃_�~�[���@�̔z�M���ʂ��u�z�M���v�ɕύX�B
                PrepareToRegisterCurDllObject()

            Catch ex As DatabaseException
                Log.Fatal("Unwelcome Exception caught.", ex)
                If isLockingDllStateRecord Then
                    'NOTE: ConnectClose�ŗ�O�����������ꍇ�ł���AAbort
                    '�����Ƃ���ŁA�u�z�M���v����ύX���邱�Ƃ��ł��Ȃ�
                    '�\���͍������A�ł���\��������̂ł���Ă����B
                    'NOTE: �{���Ȃ�A���[�U�ւُ̈�ʒm�Ɋւ��āA
                    '���W�f�[�^��L�e�[�u���ւ̓o�^�����łȂ��A�ԈႢ�Ȃ�
                    '�@�\����i�ʖڂȂ��̂Ɉˑ����Ȃ��j���[���x����
                    '�d�g�݂�p�ӂ���ׂ��ł���B
                    '���Ƃ��΁A���W�f�[�^��L�o�^�@�\���ˑ�����S�X���b�h��
                    '�u�ŏI�I�Ƀ��[�U�Ɉُ��\������P���ȋ@��v�ɑ΂��āA
                    '�����I�ɐ�����ʒm���郋�[����݂���Ȃǂł���B
                    '�Ȃ��ASNMP TRAP�́A���W�f�[�^��L�e�[�u�������[�U��
                    '�\���I�Ƀ|�[�����O����Ԃ𖄂߂�i�ʒm�����������
                    '�����Ɉُ��m�邱�Ƃ��ł��āA������Ɗ������j�Ƃ���
                    '�Ӌ`�͂�����̂́A���̂悤�ȗv���͖������Ȃ��B
                    '�u�\�Ȃ�v�ُ��o�^����i�ʒm����j�Ƃ����_�ŁA
                    '���W�f�[�^��L�o�^�Ƒ哯���ق̋@�\�ł���B
                    Abort()
                Else
                    Return EkNakCauseCode.TelegramError 'NOTE: ����
                End If
            End Try
        End SyncLock

        Return EkNakCauseCode.None
    End Function

    '�󓮓IULL�����������i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e���������Ă��邱�Ƃ��m�F�����j
    '�i�]���I��REQ�d���ɑ΂�ACK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Function ProcOnPassiveUllComplete(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, oXllReqTeleg.FileName)
        Dim sFileName As String = Path.GetFileName(sFilePath)
        Dim violation As NakCauseCode

        '��M�����t�@�C���̓��e��DB�ɓo�^�B
        If EkMasProListFileName.IsValid(sFileName) Then
            sFileName = EkMasProListFileName.Normalize(sFileName)
            violation = RegisterCurDllObjectAsList(sFilePath, sFileName, oXllReqTeleg.FileHashValue)
        ElseIf EkMasterDataFileName.IsValid(sFileName) Then
            sFileName = EkMasterDataFileName.Normalize(sFileName)
            violation = RegisterCurDllObjectAsMasterData(sFilePath, sFileName, oXllReqTeleg.FileHashValue)
        ElseIf EkProgramDataFileName.IsValid(sFileName) Then
            sFileName = EkProgramDataFileName.Normalize(sFileName)
            violation = RegisterCurDllObjectAsProgramData(sFilePath, sFileName, oXllReqTeleg.FileHashValue)
        Else
            Debug.Fail("This case is impermissible.")
            violation = EkNakCauseCode.TelegramError
        End If

        '���e���̖��ɂ��ADB�ɓo�^�ł��Ȃ��ꍇ
        If violation <> EkNakCauseCode.None Then
            '��M�����t�@�C�����폜�B
            File.Delete(sFilePath)

            '���Y��ʁE���Y�o�[�W�����̃_�~�[���@�̔z�M���ʂ��u�|�v�ɕύX�B
            'NOTE: ��O�����������ꍇ�A�u�z�M���v�̂܂܏������p������̂�
            '���������̂ŁA���̂܂܃X���b�h�̏I���Ɏ������ށB
            'NOTE: �{���Ȃ�i�ȉ������j
            FinishToRegisterCurDllObject()

            '�Ăь��ɖ���ʒm���ďI���B
            Return violation
        End If

        '��M�����t�@�C�����}�X�^/�v���O�����̊Ǘ��f�B���N�g���Ɉړ��B
        'NOTE: �{���́ADB�ɃR�~�b�g���s���O�i���[���o�b�N���ł��鎞�_�j��
        '���{����̂����z�ł��邪�A�R�~�b�g���Ă���t�@�C���ړ���
        '�s����܂ł̊Ԃɗ�O�̔�����������\���͒Ⴂ���߁A
        '���ʉ���D�悵�Ă����Ŏ��s����B
        Dim sDstPath As String = Path.Combine(Config.MasProDirPath, sFileName)
        File.Delete(sDstPath)
        File.Move(sFilePath, sDstPath)

        '���Y��ʁE���Y�o�[�W�����̃_�~�[���@�̔z�M���ʂ��u�|�v�ɕύX�B
        'NOTE: ��O�����������ꍇ�A�u�z�M���v�̂܂܏������p������̂�
        '���������̂ŁA���̂܂܃X���b�h�̏I���Ɏ������ށB
        'NOTE: �{���Ȃ�i�ȉ������j
        FinishToRegisterCurDllObject()

        Return EkNakCauseCode.None
    End Function

    '�󓮓IULL�ɂē]�����������o�����i��M�ς݂̃n�b�V���l�Ǝ�M���������t�@�C���̓��e�ɕs���������o�����j
    '�i�]���I��REQ�d���ɑ΂��n�b�V���l�̕s��v������NAK�d����ԐM���邱�ƂɂȂ�j�ꍇ
    Protected Overrides Function ProcOnPassiveUllHashValueError(ByVal iXllReqTeleg As IXllReqTelegram) As NakCauseCode
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, oXllReqTeleg.FileName)

        '��M�����t�@�C��������Ύ̂Ă�B
        File.Delete(sFilePath)

        '���Y��ʁE���Y�o�[�W�����̃_�~�[���@�̔z�M���ʂ��u�|�v�ɕύX�B
        'NOTE: ��O�����������ꍇ�A�u�z�M���v�̂܂܏������p������̂�
        '���������̂ŁA���̂܂܃X���b�h�̏I���Ɏ������ށB
        'NOTE: �{���Ȃ�i�ȉ������j
        FinishToRegisterCurDllObject()

        Return EkNakCauseCode.HashValueError
    End Function

    '�󓮓IULL�ɂăN���C�A���g����]�����s��ʒm���ꂽ�iContinueCode.Abort�̓]���I��REQ�d������M�����j�ꍇ
    Protected Overrides Sub ProcOnPassiveUllAbort(ByVal iXllReqTeleg As IXllReqTelegram)
        Dim oXllReqTeleg As EkClientDrivenUllReqTelegram = DirectCast(iXllReqTeleg, EkClientDrivenUllReqTelegram)
        Dim sFilePath As String = Utility.CombinePathWithVirtualPath(Config.FtpServerRootDirPath, oXllReqTeleg.FileName)

        '��M�����t�@�C��������Ύ̂Ă�B
        File.Delete(sFilePath)

        '���Y��ʁE���Y�o�[�W�����̃_�~�[���@�̔z�M���ʂ��u�|�v�ɕύX�B
        'NOTE: ��O�����������ꍇ�A�u�z�M���v�̂܂܏������p������̂�
        '���������̂ŁA���̂܂܃X���b�h�̏I���Ɏ������ށB
        'NOTE: �{���Ȃ�i�ȉ������j
        FinishToRegisterCurDllObject()
    End Sub

    '�󓮓IULL�ɂē]���I��REQ�d���҂��̃^�C���A�E�g�����������ꍇ
    Protected Overrides Sub ProcOnPassiveUllTimeout(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: ���̏󋵂ł́AFTP�T�[�o��ɂ����M�i��M���j�t�@�C�����폜���Ȃ������悢�B

        '���Y��ʁE���Y�o�[�W�����̃_�~�[���@�̔z�M���ʂ��u�|�v�ɕύX�B
        'NOTE: ��O�����������ꍇ�A�u�z�M���v�̂܂܏������p������̂�
        '���������̂ŁA���̂܂܃X���b�h�̏I���Ɏ������ށB
        'NOTE: �{���Ȃ�i�ȉ������j
        FinishToRegisterCurDllObject()
    End Sub

    '�󓮓IULL�̍Œ���L���[�C���O���ꂽ�󓮓IULL�̊J�n�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnPassiveUllAnonyError(ByVal iXllReqTeleg As IXllReqTelegram)
        'NOTE: ���̏󋵂ł́AFTP�T�[�o��ɂ����M�i��M���j�t�@�C�����폜���Ȃ������悢�B

        If isLockingDllStateRecord Then
            '���Y��ʁE���Y�o�[�W�����̃_�~�[���@�̔z�M���ʂ��u�|�v�ɕύX�B
            'NOTE: ��O�����������ꍇ�A�u�z�M���v�̂܂܏������p������̂�
            '���������̂ŁA���̂܂܃X���b�h�̏I���Ɏ������ށB
            'NOTE: �{���Ȃ�i�ȉ������j
            FinishToRegisterCurDllObject()
        End If
    End Sub

    '�e�X���b�h����R�l�N�V�������󂯎�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionAppear()
        LineStatus = LineStatus.Steady
    End Sub

    '�R�l�N�V������ؒf�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionDisappear()
        LineStatus = LineStatus.Disconnected
    End Sub

    Protected Overrides Sub ProcOnUnhandledException(ByVal unhandledEx As Exception)
        If isLockingDllStateRecord Then
            Try
                '���Y��ʁE���Y�o�[�W�����́u�z�M���v�ɂȂ��Ă���
                '�S�Ẵ��R�[�h���u�ُ�v�ɕύX�B
                TransitCurDllStatusToAbnormal()
            Catch ex As Exception
                'NOTE: ����̐��i�d�l�ł̑Ώ����@�Ƃ��ẮA
                '�^�ǃT�[�o�A�v���̑S�v���Z�X���I������i�����Ȃ��Ƃ�
                'DLL��ԃe�[�u���́u�z�M���v���R�[�h���N�����Ɂu�ُ�v�ɂ���
                '���Ƃ��ł���ʐM�n�v���Z�X���I������j���炢�����Ȃ��B
                'NOTE: �{���Ȃ�i�ȉ������j
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        MyBase.ProcOnUnhandledException(unhandledEx)
    End Sub
#End Region

#Region "�C�x���g���������p���\�b�h"
    Protected Overrides Function SendReqTelegram(ByVal iReqTeleg As IReqTelegram) As Boolean
        Dim oReqTeleg As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        oReqTeleg.ReqNumber = reqNumberForNextSnd
        oReqTeleg.ClientCode = clientCode
        Dim ret As Boolean = MyBase.SendReqTelegram(oReqTeleg)

        If reqNumberForNextSnd >= 999999 Then
            reqNumberForNextSnd = 0
        Else
            reqNumberForNextSnd += 1
        End If

        Return ret
    End Function

    Protected Overrides Function SendReplyTelegram(ByVal iReplyTeleg As ITelegram, ByVal iSourceTeleg As ITelegram) As Boolean
        Dim oReplyTeleg As EkTelegram = DirectCast(iReplyTeleg, EkTelegram)
        Dim oSourceTeleg As EkTelegram = DirectCast(iSourceTeleg, EkTelegram)
        oReplyTeleg.RawReqNumber = oSourceTeleg.RawReqNumber
        oReplyTeleg.RawClientCode = oSourceTeleg.RawClientCode
        Return MyBase.SendReplyTelegram(oReplyTeleg, oSourceTeleg)
    End Function

    'NAK�d���𑗐M����ꍇ���M�����ꍇ�̂��̌�̋��������߂邽�߂̃��\�b�h
    Protected Overrides Function GetRequirement(ByVal oNakTeleg As INakTelegram) As NakRequirement
        'TODO: �f�[�^��ʂȂǂŕ��򂵂Ă����΁A�قƂ�ǂ̃P�[�X��
        '�v���g�R���ᔽ�Ƃ݂Ȃ��āANakRequirement.DisconnectImmediately
        '��ԋp���邱�ƂɂȂ�͂��B
        Select Case oNakTeleg.CauseCode
            '�p���i���g���C�I�[�o�[�j���Ă��ُ�Ƃ݂͂Ȃ��Ȃ�NAK�d��
            Case EkNakCauseCode.NoData, EkNakCauseCode.Unnecessary
                Return NakRequirement.ForgetOnRetryOver

            '�p���i���g���C�I�[�o�[�j������ُ�Ƃ݂Ȃ��ׂ�NAK�d��
            Case EkNakCauseCode.Busy, EkNakCauseCode.NoTime, EkNakCauseCode.InvalidContent, EkNakCauseCode.UnknownLight
                Return NakRequirement.CareOnRetryOver

            '�ʐM�ُ�Ƃ݂Ȃ��ׂ�NAK�d��
            Case EkNakCauseCode.TelegramError, EkNakCauseCode.NotPermit, EkNakCauseCode.HashValueError, EkNakCauseCode.UnknownFatal
                Return NakRequirement.DisconnectImmediately

            'NOTE: �ǂ̂悤�ȃo�C�g���Parse���Ă�CauseCode��None��
            'NAK�d���ɂ͂Ȃ�Ȃ��͂��ł��邽�߁ACauseCode��None�̏ꍇ�A
            '���L�̃P�[�X�Ƃ��ď�������B
            Case Else
                Debug.Fail("This case is impermissible.")
                Return NakRequirement.CareOnRetryOver
        End Select
    End Function

    Protected Overridable Function IsCurDllObjectRegistered() As Boolean
        'NOTE: �e�[�u�����̍ő僌�R�[�h���������Ă��邵�A�C���f�b�N�X��
        '�����̂ŁA�������̂��̂��擾���đΏۃf�[�^�̗L���𔻒f����B

        Dim sSQLToCountList As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_LIST_HEADLINE" _
           & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
           & " AND LIST_VERSION = '" & curDll.ListVersion & "'"

        Dim sSQLToCountData As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
           & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            If CInt(dbCtl.ExecuteSQLToReadScalar(sSQLToCountList)) = 0 Then Return False
            If CInt(dbCtl.ExecuteSQLToReadScalar(sSQLToCountData)) = 0 Then Return False
            Return True
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function IsCurDllApplyingRunnableProgram(ByVal sListFileName As String) As Boolean
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            Dim sSQLToGetRunnableDate As String = _
               "SELECT RUNNABLE_DATE" _
               & " FROM S_" & EkConstants.DataPurposeProgram & "_DATA_HEADLINE" _
               & " WHERE DATA_KIND = '" & curDll.DataKind & "'" _
               & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
               & " AND DATA_VERSION = '" & curDll.DataVersion & "'"
            Dim sRunnableDate As String = CStr(dbCtl.ExecuteSQLToReadScalar(sSQLToGetRunnableDate))

            'NOTE: �e�[�u�����̍ő僌�R�[�h���������Ă��邵�A�C���f�b�N�X��
            '�������A�ʏ펞�̓��삩�炵�ĂP�����q�b�g���Ȃ��iFILE_NAME�̈�v����
            '���R�[�h���ǂ݂̂��S�Ă݂邱�ƂɂȂ�j�̂ŁA�������̂��̂��擾����
            '�Ώۃf�[�^�̗L���𔻒f����B
            Dim sSQL As String = _
               "SELECT COUNT(*)" _
               & " FROM S_" & EkConstants.DataPurposeProgram & "_LIST" _
               & " WHERE FILE_NAME = '" & sListFileName & "'" _
               & " AND APPLICABLE_DATE < '" & sRunnableDate & "'" _
               & " AND APPLICABLE_DATE <> '19000101'" _
               & " AND APPLICABLE_DATE <> ''"
            Return CInt(dbCtl.ExecuteSQLToReadScalar(sSQL)) = 0
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function IsCurDllNecessary(ByVal sListFileName As String, ByVal dllStartTime As DateTime) As Boolean
        Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)

        'NOTE: �e�[�u�����̍ő僌�R�[�h���������Ă��邵�A�C���f�b�N�X��
        '�����̂ŁA�������̂��̂��擾���đΏۃf�[�^�̗L���𔻒f����B
        Dim sSQL As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        'NOTE: �v���O�����K�p���X�g�̏ꍇ�́A�L���ȍs�𒊏o����ɂ�����A
        '�K�p���ɂ��ƂÂ��ǉ��̏������������Ă���B�Ȃ��A�u�����N��
        '�ǂ̂悤�ȓ��t�i������j�����������Ƃ݂Ȃ����z��ł���B
        If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
            sSQL = sSQL _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                   & " OR APPLICABLE_DATE = '19000101'" _
                   & " OR APPLICABLE_DATE = '99999999')"
        End If

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CInt(dbCtl.ExecuteSQLToReadScalar(sSQL)) <> 0
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function IsCurDllConsistentWithMachineMaster(ByVal sListFileName As String, ByVal dllStartTime As DateTime) As Boolean
        Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)

        '�K�p�Ώۋ@����擾���邽�߂�SQL��ҏW�B
        Dim sSQLToSelectDataApplicableUnits As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM S_" & curDll.DataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        'NOTE: �v���O�����K�p���X�g�̏ꍇ�́A�L���ȍs�𒊏o����ɂ�����A
        '�K�p���ɂ��ƂÂ��ǉ��̏������������Ă���B�Ȃ��A�u�����N��
        '�ǂ̂悤�ȓ��t�i������j�����������Ƃ݂Ȃ����z��ł���B
        If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
            sSQLToSelectDataApplicableUnits = sSQLToSelectDataApplicableUnits _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                   & " OR APPLICABLE_DATE = '19000101'" _
                   & " OR APPLICABLE_DATE = '99999999')"
        End If

        '�z�M�w���̎��_�ŗL���ȋ@����擾���邽�߂�SQL��ҏW�B
        Dim sSQLToSelectUnitsInService As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM M_MACHINE" _
           & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
           & " AND ADDRESS <> ''" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & sDeliveryStartDate & "'" _
                                        & " AND INSERT_DATE <= CONVERT(DATETIME, '" & dllStartTime.ToString("yyyy/MM/dd HH:mm:ss") & "', 120))"

        Dim applicableUnits As DataRowCollection
        Dim serviceUnits As EnumerableRowCollection(Of DataRow)
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            applicableUnits = dbCtl.ExecuteSQLToRead(sSQLToSelectDataApplicableUnits).Rows
            serviceUnits = dbCtl.ExecuteSQLToRead(sSQLToSelectUnitsInService).AsEnumerable()
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try

        For Each applicableUnit As DataRow In applicableUnits
            Dim sRailSection As String = applicableUnit.Field(Of String)("RAIL_SECTION_CODE")
            Dim sStationOrder As String = applicableUnit.Field(Of String)("STATION_ORDER_CODE")
            Dim corner As Integer = applicableUnit.Field(Of Integer)("CORNER_CODE")
            Dim unit As Integer = applicableUnit.Field(Of Integer)("UNIT_NO")
            Dim num As Integer = ( _
               From serviceUnit In serviceUnits _
               Where serviceUnit.Field(Of String)("RAIL_SECTION_CODE") = sRailSection And _
                     serviceUnit.Field(Of String)("STATION_ORDER_CODE") = sStationOrder And _
                     serviceUnit.Field(Of Integer)("CORNER_CODE") = corner And _
                     serviceUnit.Field(Of Integer)("UNIT_NO") = unit _
               Select serviceUnit _
            ).Count

            If num = 0 Then
                Log.Info("[" & sRailSection & sStationOrder & "_" & corner.ToString("D4") & "_" & unit.ToString("D2") & "] of [" & curDll.DataApplicableModel & "] is not in service.")
                Return False
            End If
        Next applicableUnit
        Return True
    End Function

    Protected Overridable Function IsCurDllStatusBusyToStart() As Boolean
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        'NOTE: �e�[�u�����̍ő僌�R�[�h���������Ă��邵�A�C���f�b�N�X��
        '�����̂ŁA�������̂��̂��擾���đΏۃf�[�^�̗L���𔻒f����B
        'NOTE: �ꌩ����ƓK�p���X�g�̃��R�[�h�����݂Ĕr����������΂悢
        '�悤�Ɏv���邩������Ȃ����A�o�^�����Ƃ̔r���̂��߂ɁAFILE_KBN��
        'FilePurposeData�̃��R�[�h���݂�K�v������B
        'NOTE: �z�M���́A���X�g�o�[�W�����������K�p���X�g�̔z�M�J�n�͋֎~�ɂ���B
        '�z�M�J�n���A��ʕ\���̓s���ɂ��A�K�p���X�g�Ɋւ��郌�R�[�h�́A
        '�z�M�Ɏg���K�p���X�g�ƃ��X�g�o�[�W���������������̑S�Ă�DLL_STS�e�[�u��
        '����������邪�A�z�M���́A���Y�z�M���̊e���M��Ɋւ��郌�R�[�h��
        'DLL_STS�e�[�u������Q�Ƃ��邽�߂ł���B
        '�t�ɁA�K�p���X�g�̓o�^���́A���Ƃ��K�p���X�g�o�[�W�����������ł����Ă�
        '��\�o�[�W�������قȂ�K�p���X�g�ł���΁A�z�M�J�n���֎~���Ȃ��B
        '�o�^���́A�K�p���X�g�Ɋւ��郌�R�[�h��DLL_STS�e�[�u������Q�Ƃ��Ȃ�����
        '�ł���B�Ȃ��A���L��SQL�ɂ����āA��\�o�[�W�������������K�p���X�g��
        '�o�^���ł��邩�ۂ��́AFILE_KBN��FilePurposeData�̃P�[�X�Ŕ��肵�Ă���
        '���Ƃɒ��Ӂi�o�^���������_�~�[���R�[�h�́A���Ƃ��K�p���X�g�̓o�^����
        '�������̂ł����Ă��AFILE_KBN��FilePurposeData�ł���j�B
        Dim sSQL As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_DLL_STS" _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND ((FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
             & " AND DATA_KIND = '" & curDll.DataKind & "'" _
             & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
             & " AND DATA_VERSION = '" & curDll.DataVersion & "')" _
             & " OR (FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
             & " AND DATA_KIND = '" & curDll.DataKind & "'" _
             & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
             & " AND (DATA_VERSION = '" & curDll.DataVersion & "'" _
               & " OR VERSION = '" & curDll.ListVersion & "')))" _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CInt(dbCtl.ExecuteSQLToReadScalar(sSQL)) <> 0
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Function IsCurDllStatusBusyToRegister() As Boolean
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        'NOTE: �e�[�u�����̍ő僌�R�[�h���������Ă��邵�A�C���f�b�N�X��
        '�����̂ŁA�������̂��̂��擾���đΏۃf�[�^�̗L���𔻒f����B
        Dim sSQL As String = _
           "SELECT COUNT(*)" _
           & " FROM S_" & curDll.DataPurpose & "_DLL_STS" _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            Return CInt(dbCtl.ExecuteSQLToReadScalar(sSQL)) <> 0
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
    End Function

    Protected Overridable Sub StartCurDll(ByVal sListFileName As String, ByVal dllStartTime As DateTime, ByVal forcingFlag As Boolean)
        Dim sDeliveryStartDate As String = EkServiceDate.GenString(dllStartTime)
        Dim sDeliveryStartTime As String = dllStartTime.ToString("yyyyMMddHHmmss")

        '�z�M�w���𑗐M����ׂ����b�Z�[�W�L���[�������߂Ă����B
        Dim oTargetQueue As MessageQueue
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            oTargetQueue = Config.MessageQueueForApps("ToTokatsu")
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            oTargetQueue = Config.MessageQueueForApps("ToKanshiban")
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        '�z�M�w�����쐬���Ă����B
        Dim oDllRequest As New ExtMasProDllRequest(sListFileName, forcingFlag)

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
        Dim sSQLToSelectApplicableUnits As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM S_" & curDll.DataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        Dim sSQLToSelectApplicableUnitsCompoundStyled As String = _
           "SELECT RAIL_SECTION_CODE + STATION_ORDER_CODE + CAST(CORNER_CODE AS varchar) + '_' + CAST(UNIT_NO AS varchar)" _
           & " FROM S_" & curDll.DataPurpose & "_LIST" _
           & " WHERE FILE_NAME = '" & sListFileName & "'"
        'NOTE: �v���O�����K�p���X�g�̏ꍇ�́A�L���ȍs�𒊏o����ɂ�����A
        '�K�p���ɂ��ƂÂ��ǉ��̏������������Ă���B�Ȃ��A�u�����N��
        '�ǂ̂悤�ȓ��t�i������j�����������Ƃ݂Ȃ����z��ł���B
        If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
            sSQLToSelectApplicableUnits = _
               sSQLToSelectApplicableUnits _
               & " AND (APPLICABLE_DATE >= '" & sDeliveryStartDate & "'" _
                    & " OR APPLICABLE_DATE = '19000101'" _
                    & " OR APPLICABLE_DATE = '99999999')"
            sSQLToSelectApplicableUnitsCompoundStyled = _
               sSQLToSelectApplicableUnitsCompoundStyled _
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
        '   & " AND MODEL_CODE = '" & curDll.DataApplicableModel & "'"
        Dim sSQLToSelectAddrOfAgents As String = _
           "SELECT DISTINCT MONITOR_ADDRESS" _
           & " FROM M_SERVICE_MACHINE" _
           & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
           & " AND RAIL_SECTION_CODE + STATION_ORDER_CODE + CAST(CORNER_CODE AS varchar) + '_' + CAST(UNIT_NO AS varchar)" _
               & " IN (" & sSQLToSelectApplicableUnitsCompoundStyled & ")"

        '���ڂ̑��M��ƂȂ鑕�u�̐���`���@���擾����SQL��ҏW�B
        'NOTE: sSQLToSelectAddrOfAgents�œ�����S�Ă̊Ď��Ղ܂��͓�����
        'sSQLToSelectAgents�ł�������i���ꂼ��̏o�͌����������ɂȂ�j
        '�z��ł��邪�A���̂��Ƃ̓`�F�b�N���Ȃ��B���̃`�F�b�N�́A
        '�K�p���X�g�ł͂Ȃ��A�@��\���}�X�^�̃`�F�b�N�ɂȂ邽�߁A
        '�@��\���}�X�^�̓o�^���ɍs����ׂ����̂ł���B
        Dim sSQLToSelectAgents As String = _
           "SELECT RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO" _
           & " FROM M_SERVICE_MACHINE" _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND ADDRESS IN (" & sSQLToSelectAddrOfAgents & ")"

        'DLL��ԃe�[�u���̃��R�[�h���X�V�܂��͒ǉ����邽�߂�SQL�̌㔼��ҏW�B
        Dim sSQLWithoutUsingToUpdateOrInsertDllSts As String = _
             " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
             & " AND Target.FILE_KBN = Source.FILE_KBN" _
             & " AND Target.DATA_KIND = Source.DATA_KIND" _
             & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
             & " AND Target.DATA_VERSION = Source.DATA_VERSION" _
             & " AND Target.VERSION = Source.VERSION" _
             & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
             & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
             & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
             & " AND Target.UNIT_NO = Source.UNIT_NO)" _
           & " WHEN MATCHED THEN" _
            & " UPDATE" _
             & " SET Target.UPDATE_DATE = GETDATE()," _
                 & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                 & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                 & " Target.DELIVERY_STS = Source.DELIVERY_STS," _
                 & " Target.DELIVERY_START_TIME = Source.DELIVERY_START_TIME," _
                 & " Target.DELIVERY_END_TIME = Source.DELIVERY_END_TIME" _
           & " WHEN NOT MATCHED THEN" _
            & " INSERT (INSERT_DATE," _
                    & " INSERT_USER_ID," _
                    & " INSERT_MACHINE_ID," _
                    & " UPDATE_DATE," _
                    & " UPDATE_USER_ID," _
                    & " UPDATE_MACHINE_ID," _
                    & " MODEL_CODE," _
                    & " FILE_KBN," _
                    & " DATA_KIND," _
                    & " DATA_SUB_KIND," _
                    & " DATA_VERSION," _
                    & " VERSION," _
                    & " RAIL_SECTION_CODE," _
                    & " STATION_ORDER_CODE," _
                    & " CORNER_CODE," _
                    & " UNIT_NO," _
                    & " DELIVERY_STS," _
                    & " DELIVERY_START_TIME," _
                    & " DELIVERY_END_TIME)" _
            & " VALUES (GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " Source.MODEL_CODE," _
                    & " Source.FILE_KBN," _
                    & " Source.DATA_KIND," _
                    & " Source.DATA_SUB_KIND," _
                    & " Source.DATA_VERSION," _
                    & " Source.VERSION," _
                    & " Source.RAIL_SECTION_CODE," _
                    & " Source.STATION_ORDER_CODE," _
                    & " Source.CORNER_CODE," _
                    & " Source.UNIT_NO," _
                    & " Source.DELIVERY_STS," _
                    & " Source.DELIVERY_START_TIME," _
                    & " Source.DELIVERY_END_TIME);"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            'NOTE: ����̐݌v�ŁA�z�M�̔r�����G���A�ʂɍs���̂́A�v���O������v���O����
            '�K�p���X�g�̃t�@�C�������G���A�ʂɗp�ӂ���Ă��邽�߂ł���i���郆�[�U��
            '�z�M�w���Ŏw�������t�@�C�����A�ʂ̃��[�U�ɂ���ď㏑������Ă��܂��̂�h��
            '��ŁA�C���^�[���b�N�̍ŏ�����ڎw���Ă���j�B�������A���ۂɓK�p���X�g��
            '���ɋL�ڂ���Ă��鍆�@���A�t�@�C�����̃G���A�ɑ����Ă��邩�́A�^�ǃT�[�o
            '���g�̓`�F�b�N���Ă��Ȃ��B�����A�G���A�ɑ����Ȃ��w���L�ڂ���Ă���΁A
            '���ۂɔz�M�Ŏg���t�@�C���Ɩ��֌W�̂��̂܂Ń��b�N���邱�ƂɂȂ��Ă��܂��A
            '�ܑ̂Ȃ��B�܂��A�g�p����K�p���X�g�ɂ́A�Œ�ł��P���̗L���ȍ��@��
            '�L�ڂ���Ă���Ƃ͂����A���ꂪ�t�@�C�����̃G���A�ɑ����Ă��Ȃ���΁A
            '���̃t�@�C�������b�N�ł��Ȃ��B�����h�����߂ɁAS_PRG_DLL_STS�ɂ�
            'DATA_SUB_KIND���p�ӂ��A�e�s�̃L�[�ɉw�R�[�h���܂܂�Ă���ɂ�������炸�A
            '�e�s�̑�����G���ANo�́ADATA_SUB_KIND�Ŏ��ʂ��邱�Ƃɂ��Ă���B
            '����́A�}�X�^�Ƃ̎������ʉ��Ƃ��������b�g�������炷�B

            'DLL��ԃe�[�u�����獡��̔z�M�Ǝ�ʁE�G���ANo�E�K�p���X�g�o�[�W������
            '�������K�p���X�g�Ɋւ���S���R�[�h���폜����B
            Dim sSQLToDeleteFromDllStsAboutList As String = _
               "DELETE FROM S_" & curDll.DataPurpose & "_DLL_STS" _
               & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
               & " AND FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
               & " AND DATA_KIND = '" & curDll.DataKind & "'" _
               & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
               & " AND VERSION = '" & curDll.ListVersion & "'"
            dbCtl.ExecuteSQLToWrite(sSQLToDeleteFromDllStsAboutList)

            If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
                '�v���O����DL��ԃe�[�u�����獡��̔z�M�Ǝ�ʁE�K�p���X�g�o�[�W�����E
                '�G���ANo���������K�p���X�g�Ɋւ���S���R�[�h���폜����B
                Dim sSQLToDeleteFromPrgDlStsAboutList As String = _
                   "DELETE FROM S_" & curDll.DataPurpose & "_DL_STS" _
                   & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                   & " AND FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
                   & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                   & " AND VERSION = '" & curDll.ListVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteFromPrgDlStsAboutList)
            End If

            '���ڂ̑��M��ƂȂ鑕�u�̐���`���@���擾����B
            Dim agents As DataRowCollection = dbCtl.ExecuteSQLToRead(sSQLToDefineCTE & sSQLToSelectAgents).Rows

            For Each agent As DataRow In agents
                Dim sAgentRailSection As String = agent.Field(Of String)("RAIL_SECTION_CODE")
                Dim sAgentStationOrder As String = agent.Field(Of String)("STATION_ORDER_CODE")
                Dim sAgentCorner As String = agent.Field(Of Integer)("CORNER_CODE").ToString()
                Dim sAgentUnit As String = agent.Field(Of Integer)("UNIT_NO").ToString()

                'NOTE: UNCERTAIN_FLG��True�ȃ��R�[�h��DATA_VERSION��0�ɂ���̂́A
                '���̌�ɒʐM�v���Z�X�ōs�����������ōs�����������I�ł͂���B
                '�������A���������s���́A�^�ǒ[���ŉ�����M�^�C�}���������Ă��邽�߁A
                '���ԓI����Ƃ��Ă͌������B���������A���̃^�C�~���O�ł́A
                '���Y���@�ւ̓��Y��ʂɊւ���z�M���ʃo�[�W�����ɂ��Ď��s����Ă���
                '�\��������A���Y��ʂ̍ŏI���M�o�[�W�����e�[�u����ύX���邱�Ƃ�
                '������̃v���Z�X�̖����Ŗ������Ƃ͖��炩�ł���B�ȏ�̂��Ƃ���A
                '��L���������́A�w���@��Ƃ̒ʐM�v���Z�X���ōs�����Ƃɂ��Ă���B

                'agent�̐���`���@���ݒ肳��Ă��鑕�u�ɑ΂��A�K�p���X�g�����łȂ�
                '�f�[�^�{�̂����t����̂��ۂ������肷��B
                Dim sendSuite As Boolean = False
                If forcingFlag = True Then
                    sendSuite = True
                Else
                    Dim sSQLToGetVer As String = _
                       "SELECT DATA_VERSION, UNCERTAIN_FLG" _
                       & " FROM S_" & curDll.DataPurpose & "_DLL_VER" _
                       & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
                       & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                       & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                       & " AND RAIL_SECTION_CODE = '" & sAgentRailSection & "'" _
                       & " AND STATION_ORDER_CODE = '" & sAgentStationOrder & "'" _
                       & " AND CORNER_CODE = " & sAgentCorner _
                       & " AND UNIT_NO = " & sAgentUnit
                    Dim verRows As DataRowCollection = dbCtl.ExecuteSQLToRead(sSQLToGetVer).Rows
                    If verRows.Count = 0 OrElse _
                       Not verRows(0).Field(Of String)("UNCERTAIN_FLG").Equals("0") OrElse _
                       Not verRows(0).Field(Of String)("DATA_VERSION").Equals(curDll.DataVersion) Then
                        sendSuite = True
                    End If
                End If

                'DLL��ԃe�[�u���ɂēK�p���X�g�̃��R�[�h���X�V�܂��͒ǉ�����B
                'OPT: �Y��������͎̂��O�̍폜�����̑ΏۂɂȂ��Ă���̂ŁA
                '�P�Ȃ�INSERT�ł��悢�B
                Dim sSQLToUpdateOrInsertDllStsAboutList As String = _
                   "MERGE INTO S_" & curDll.DataPurpose & "_DLL_STS AS Target" _
                   & " USING (SELECT '" & sDllAgentModel & "' MODEL_CODE," _
                                 & " '" & EkConstants.FilePurposeList & "' FILE_KBN," _
                                 & " '" & curDll.DataKind & "' DATA_KIND," _
                                 & " '" & curDll.DataSubKind & "' DATA_SUB_KIND," _
                                 & " '" & curDll.DataVersion & "' DATA_VERSION," _
                                 & " '" & curDll.ListVersion & "' VERSION," _
                                 & " '" & sAgentRailSection & "' RAIL_SECTION_CODE," _
                                 & " '" & sAgentStationOrder & "' STATION_ORDER_CODE," _
                                 & " " & sAgentCorner & " CORNER_CODE," _
                                 & " " & sAgentUnit & " UNIT_NO," _
                                 & " " & DbConstants.DllStatusExecuting.ToString() & " DELIVERY_STS," _
                                 & " '" & sDeliveryStartTime & "' DELIVERY_START_TIME," _
                                 & " '' DELIVERY_END_TIME) AS Source" _
                   & sSQLWithoutUsingToUpdateOrInsertDllSts
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDllStsAboutList)

                If sendSuite Then
                    'DLL��ԃe�[�u���ɂăf�[�^�{�̂̃��R�[�h���X�V�܂��͒ǉ�����B
                    Dim sSQLToUpdateOrInsertDllStsAboutData As String = _
                       "MERGE INTO S_" & curDll.DataPurpose & "_DLL_STS AS Target" _
                       & " USING (SELECT '" & sDllAgentModel & "' MODEL_CODE," _
                                     & " '" & EkConstants.FilePurposeData & "' FILE_KBN," _
                                     & " '" & curDll.DataKind & "' DATA_KIND," _
                                     & " '" & curDll.DataSubKind & "' DATA_SUB_KIND," _
                                     & " '" & curDll.DataVersion & "' DATA_VERSION," _
                                     & " '" & curDll.DataVersion & "' VERSION," _
                                     & " '" & sAgentRailSection & "' RAIL_SECTION_CODE," _
                                     & " '" & sAgentStationOrder & "' STATION_ORDER_CODE," _
                                     & " " & sAgentCorner & " CORNER_CODE," _
                                     & " " & sAgentUnit & " UNIT_NO," _
                                     & " " & DbConstants.DllStatusExecuting.ToString() & " DELIVERY_STS," _
                                     & " '" & sDeliveryStartTime & "' DELIVERY_START_TIME," _
                                     & " '' DELIVERY_END_TIME) AS Source" _
                       & sSQLWithoutUsingToUpdateOrInsertDllSts
                    dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDllStsAboutData)
                End If
            Next agent

            If curDll.DataPurpose.Equals(EkConstants.DataPurposeProgram) Then
                '�K�p���X�g�ɋL�ڂ���Ă���S�Ă̗L�����@�ɂ��āA
                'DL��ԃe�[�u���ɓ��Y���R�[�h�������ꍇ�͒ǉ�����B
                '���R�[�h�����ɂ���ꍇ���i�ߋ��ɈႤ�G���A��
                '�o�^���ꂽ���̂ł����Ă������������ŏ������悤��
                '���Ă������߂Ɂj�G���ANo���㏑������B
                'NOTE: �v���O������DL��Ԃɂ����āA�G���ANo��
                '���炽�ɔz�M���n�߂�ۂɍ폜���郌�R�[�h��
                '�I�����邽�߂����̂��̂ł���B�^�ǒ[���̔z�M��
                '��ʂŃG���A���i��ۂ́ADLL��ԃe�[�u���̃G���A��
                '�i�邱�ƂɂȂ��Ă���A�{���I��DL��ԃe�[�u����
                '���R�[�h���i��ʂ́j�G���A�ɂЂ��Â���̂�
                '�w�R�[�h�ł���B����āA�G���ANo����v���Ȃ��Ă��A
                '�i�K�p���X�g�ɋL�q���ꂽ���̂Ɛ���E�w���E
                '�R�[�i�[�E���@����v����΁j���Y���R�[�h�Ƃ݂Ȃ��B
                Dim sSQLToUpdateOrInsertDlStsAboutData As String = _
                   "MERGE INTO S_" & curDll.DataPurpose & "_DL_STS AS Target" _
                   & " USING (" & sSQLToSelectApplicableUnits & ") AS Source" _
                   & " ON (Target.MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                     & " AND Target.FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                     & " AND Target.DATA_KIND = '" & curDll.DataKind & "'" _
                     & " AND Target.VERSION = '" & curDll.DataVersion & "'" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                   & " WHEN NOT MATCHED THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " DATA_SUB_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " '" & curDll.DataApplicableModel & "'," _
                            & " '" & EkConstants.FilePurposeData & "'," _
                            & " '" & curDll.DataKind & "'," _
                            & " '" & curDll.DataSubKind & "'," _
                            & " '" & curDll.DataVersion & "'," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusPreExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDlStsAboutData)

                '�v���O�����̏ꍇ�AData�����łȂ��AList�ɂ��Ăɂ��Ă��A
                'DL��ԃe�[�u���̃��R�[�h��ǉ�����B
                'NOTE: List�̏ꍇ�́ADL��ԃe�[�u���̊֘A���R�[�h��
                '���O�ɍ폜���Ă��邽�߁A��{�I��INSERT�������s����Ȃ��͂��B
                Dim sSQLToUpdateOrInsertDlStsAboutList As String = _
                   "MERGE INTO S_" & curDll.DataPurpose & "_DL_STS AS Target" _
                   & " USING (" & sSQLToSelectApplicableUnits & ") AS Source" _
                   & " ON (Target.MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                     & " AND Target.FILE_KBN = '" & EkConstants.FilePurposeList & "'" _
                     & " AND Target.DATA_KIND = '" & curDll.DataKind & "'" _
                     & " AND Target.VERSION = '" & curDll.ListVersion & "'" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN MATCHED THEN" _
                    & " UPDATE" _
                     & " SET Target.UPDATE_DATE = GETDATE()," _
                         & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                         & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                         & " Target.DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                   & " WHEN NOT MATCHED THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " DATA_SUB_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " '" & curDll.DataApplicableModel & "'," _
                            & " '" & EkConstants.FilePurposeList & "'," _
                            & " '" & curDll.DataKind & "'," _
                            & " '" & curDll.DataSubKind & "'," _
                            & " '" & curDll.ListVersion & "'," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusPreExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDlStsAboutList)
            Else
                '�K�p���X�g�ɋL�ڂ���Ă���S�Ă̗L�����@�ɂ��āA
                'DL��ԃe�[�u���ɓ��Y���R�[�h�������ꍇ�͒ǉ�����B
                'NOTE: �}�X�^��DL��Ԃ́A�p�^�[���ʂɊǗ�����̂ŁA
                '�p�^�[��No�܂ň�v���Ȃ���΁A���Y���R�[�h�Ƃ݂Ȃ��Ȃ��B
                Dim sSQLToUpdateOrInsertDlStsAboutData As String = _
                   "MERGE INTO S_" & curDll.DataPurpose & "_DL_STS AS Target" _
                   & " USING (" & sSQLToSelectApplicableUnits & ") AS Source" _
                   & " ON (Target.MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                     & " AND Target.FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
                     & " AND Target.DATA_KIND = '" & curDll.DataKind & "'" _
                     & " AND Target.DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                     & " AND Target.VERSION = '" & curDll.DataVersion & "'" _
                     & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
                     & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
                     & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
                     & " AND Target.UNIT_NO = Source.UNIT_NO)" _
                   & " WHEN NOT MATCHED THEN" _
                    & " INSERT (INSERT_DATE," _
                            & " INSERT_USER_ID," _
                            & " INSERT_MACHINE_ID," _
                            & " UPDATE_DATE," _
                            & " UPDATE_USER_ID," _
                            & " UPDATE_MACHINE_ID," _
                            & " MODEL_CODE," _
                            & " FILE_KBN," _
                            & " DATA_KIND," _
                            & " DATA_SUB_KIND," _
                            & " VERSION," _
                            & " RAIL_SECTION_CODE," _
                            & " STATION_ORDER_CODE," _
                            & " CORNER_CODE," _
                            & " UNIT_NO," _
                            & " DELIVERY_STS)" _
                    & " VALUES (GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " GETDATE()," _
                            & " '" & UserId & "'," _
                            & " '" & MachineId & "'," _
                            & " '" & curDll.DataApplicableModel & "'," _
                            & " '" & EkConstants.FilePurposeData & "'," _
                            & " '" & curDll.DataKind & "'," _
                            & " '" & curDll.DataSubKind & "'," _
                            & " '" & curDll.DataVersion & "'," _
                            & " Source.RAIL_SECTION_CODE," _
                            & " Source.STATION_ORDER_CODE," _
                            & " Source.CORNER_CODE," _
                            & " Source.UNIT_NO," _
                            & " " & DbConstants.DlStatusPreExecuting.ToString() & ");"
                dbCtl.ExecuteSQLToWrite(sSQLToUpdateOrInsertDlStsAboutData)
            End If

            '���Y�f�[�^�̓��Y���@�ւ̔z�M���ʂ����ۂɁu�z�M���v�ɂ�����A
            '���b�Z�[�W�̑��M���Ŏ��s���邱�Ƃ͍l���ɂ������A
            '�e�X���b�h����Abort���s����P�[�X�����邽�߁A
            '����ɔ����āA�z�M���ʂ��u�z�M���v�ɂ���|���L�^���Ă���
            '�g�����U�N�V����������������B
            isLockingDllStateRecord = True
            Thread.MemoryBarrier()
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            isLockingDllStateRecord = False
            Thread.MemoryBarrier()
            Throw

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            isLockingDllStateRecord = False
            Thread.MemoryBarrier()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try

        oTargetQueue.Send(oDllRequest)

        'NOTE: ���̎��_�ł́A�����������Ƃ���DLL��ԃe�[�u���́u�z�M���v������
        '����i�u�ُ�v�ɕύX����j�̂́A���ɁAoDllRequest�̑��M��v���Z�X��
        '�Ӗ��ɂȂ��Ă�����̂Ƃ���B
        isLockingDllStateRecord = False
        Thread.MemoryBarrier()
    End Sub

    Protected Overridable Sub PrepareToRegisterCurDllObject()
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        Dim sSQL As String = _
           "MERGE INTO S_" & curDll.DataPurpose & "_DLL_STS AS Target" _
           & " USING (SELECT '" & sDllAgentModel & "' MODEL_CODE," _
                         & " '" & EkConstants.FilePurposeData & "' FILE_KBN," _
                         & " '" & curDll.DataKind & "' DATA_KIND," _
                         & " '" & curDll.DataSubKind & "' DATA_SUB_KIND," _
                         & " '" & curDll.DataVersion & "' DATA_VERSION," _
                         & " '" & curDll.DataVersion & "' VERSION," _
                         & " '000' RAIL_SECTION_CODE," _
                         & " '000' STATION_ORDER_CODE," _
                         & " 0 CORNER_CODE," _
                         & " 0 UNIT_NO," _
                         & " " & DbConstants.DllStatusExecuting.ToString() & " DELIVERY_STS," _
                         & " '' DELIVERY_START_TIME," _
                         & " '' DELIVERY_END_TIME) AS Source" _
           & " ON (Target.MODEL_CODE = Source.MODEL_CODE" _
            & " AND Target.FILE_KBN = Source.FILE_KBN" _
            & " AND Target.DATA_KIND = Source.DATA_KIND" _
            & " AND Target.DATA_SUB_KIND = Source.DATA_SUB_KIND" _
            & " AND Target.DATA_VERSION = Source.DATA_VERSION" _
            & " AND Target.VERSION = Source.VERSION" _
            & " AND Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
            & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
            & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
            & " AND Target.UNIT_NO = Source.UNIT_NO)" _
           & " WHEN MATCHED THEN" _
            & " UPDATE" _
             & " SET Target.UPDATE_DATE = GETDATE()," _
                 & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                 & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'," _
                 & " Target.DELIVERY_STS = Source.DELIVERY_STS," _
                 & " Target.DELIVERY_START_TIME = Source.DELIVERY_START_TIME," _
                 & " Target.DELIVERY_END_TIME = Source.DELIVERY_END_TIME" _
           & " WHEN NOT MATCHED THEN" _
            & " INSERT (INSERT_DATE," _
                    & " INSERT_USER_ID," _
                    & " INSERT_MACHINE_ID," _
                    & " UPDATE_DATE," _
                    & " UPDATE_USER_ID," _
                    & " UPDATE_MACHINE_ID," _
                    & " MODEL_CODE," _
                    & " FILE_KBN," _
                    & " DATA_KIND," _
                    & " DATA_SUB_KIND," _
                    & " DATA_VERSION," _
                    & " VERSION," _
                    & " RAIL_SECTION_CODE," _
                    & " STATION_ORDER_CODE," _
                    & " CORNER_CODE," _
                    & " UNIT_NO," _
                    & " DELIVERY_STS," _
                    & " DELIVERY_START_TIME," _
                    & " DELIVERY_END_TIME)" _
            & " VALUES (GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " Source.MODEL_CODE," _
                    & " Source.FILE_KBN," _
                    & " Source.DATA_KIND," _
                    & " Source.DATA_SUB_KIND," _
                    & " Source.DATA_VERSION," _
                    & " Source.VERSION," _
                    & " Source.RAIL_SECTION_CODE," _
                    & " Source.STATION_ORDER_CODE," _
                    & " Source.CORNER_CODE," _
                    & " Source.UNIT_NO," _
                    & " Source.DELIVERY_STS," _
                    & " Source.DELIVERY_START_TIME," _
                    & " Source.DELIVERY_END_TIME);"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            '���Y�f�[�^�̃_�~�[���@�ւ̔z�M���ʂ����ۂɁu�z�M���v�ɂ�����A
            '�o�^���I���āu�|�v�ɖ߂��܂ł̊Ԃɐe�X���b�h����Abort���s����
            '�ꍇ�Ȃǂɔ����āA�z�M���ʂ��u�z�M���v�ɂ���|���L�^���Ă���
            '�g�����U�N�V����������������B
            isLockingDllStateRecord = True
            Thread.MemoryBarrier()
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            isLockingDllStateRecord = False
            Thread.MemoryBarrier()
            Throw

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            isLockingDllStateRecord = False
            Thread.MemoryBarrier()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Protected Overridable Sub FinishToRegisterCurDllObject()
        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        Dim sSQL As String = _
           "DELETE FROM S_" & curDll.DataPurpose & "_DLL_STS" _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND FILE_KBN = '" & EkConstants.FilePurposeData & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
           & " AND VERSION = '" & curDll.DataVersion & "'" _
           & " AND RAIL_SECTION_CODE = '000'" _
           & " AND STATION_ORDER_CODE = '000'" _
           & " AND CORNER_CODE = 0" _
           & " AND UNIT_NO = 0"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()
            'NOTE: �t���[�����[�N��Ńg�����U�N�V������������Ɗ���������΁A
            '�������o���A����������i���L�̃����������������A���̎�O�ɓ��荞��
            '���Ƃ͂Ȃ��j�Ǝv���邽�߁A�����ł�Thread.MemoryBarrier()��
            '�ȗ�����B

            isLockingDllStateRecord = False
            Thread.MemoryBarrier()

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

    Protected Overridable Sub TransitCurDllStatusToAbnormal()
        'NOTE: ���̃��\�b�h�́A�z�M���̓o�^��z�M�J�n�ŗ�O�����������ꍇ��
        '���s����B�z�M��Ԃ��u�z�M���v�ɂ������R�[�h�̂����ŁA�X���b�h
        '�ċN����̓o�^��z�M���s�\�ɂȂ�Ȃ��悤�A���Y���R�[�h�̔z�M���
        '���u�ُ�v�ɕύX���邽�߂̃��\�b�h�ł���B
        '���Y���R�[�h���w�肷���ŁALIST_VERSION�̎w��͕s�v�ł���B
        'MODEL_CODE�`DATA_VERSION���w��ǂ���̃��R�[�h�̒��ŁA
        'DELIVERY_STS��DllStatusExecuting�ɂȂ��Ă���̂́A
        '�r������ɂ��A���Y���R�[�h�݂̂ɐ�������Ă��邩��ł���B
        '�֑��ł��邪�A���l�̗��R�ŁADELIVERY_STS�`DATA_SUB_KIND��
        'LIST_VERSION���w�肷��΁ADATA_VERSION���w�肹���Ƃ��ADELIVERY_STS��
        'DllStatusExecuting�ɂȂ��Ă���K�p���X�g�̃��R�[�h�́A
        '���ݎ��s���̔z�M�̂��߂̃��R�[�h�����ɍi�荞�߂�B

        Dim sDllAgentModel As String
        If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeMadosho) Then
            sDllAgentModel = EkConstants.ModelCodeTokatsu
        Else
            sDllAgentModel = EkConstants.ModelCodeKanshiban
        End If

        Dim sSQL As String = _
           "UPDATE S_" & curDll.DataPurpose & "_DLL_STS" _
           & " SET UPDATE_DATE = GETDATE()," _
               & " UPDATE_USER_ID = '" & UserId & "'," _
               & " UPDATE_MACHINE_ID = '" & MachineId & "'," _
               & " DELIVERY_STS = " & DbConstants.DllStatusAbnormal.ToString() _
           & " WHERE MODEL_CODE = '" & sDllAgentModel & "'" _
           & " AND DATA_KIND = '" & curDll.DataKind & "'" _
           & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
           & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
           & " AND DELIVERY_STS = " & DbConstants.DllStatusExecuting.ToString()

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()
            'NOTE: �t���[�����[�N��Ńg�����U�N�V������������Ɗ���������΁A
            '�������o���A����������i���L�̃����������������A���̎�O�ɓ��荞��
            '���Ƃ͂Ȃ��j�Ǝv���邽�߁A�����ł�Thread.MemoryBarrier()��
            '�ȗ�����B

            isLockingDllStateRecord = False
            Thread.MemoryBarrier()

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

    'NOTE: ���[�U��������s�������̏�ŁA���[�U�Ɍ��ʂ�Ԃ����ƂɂȂ�󋵂Ŏ��s�����
    '�i���[�U�����̏�Ŗ���F�����ďC���\�ł���j���߁A���̃��\�b�h�ɂ��`�F�b�N��
    '�\�������邭�炢�������č\��Ȃ����̂Ƃ���B
    '��L�̂悤�ȏ����Ŏ��s������A�K�p���X�g�͓��͂��ꂽ���̂����̂܂܏o�͂��Ȃ����
    '�Ȃ�Ȃ����Ƃ���A���̃��\�b�h�ɂ́u��M������̂ɂ͊��e�ɁA���M������̂͌����Ɂv
    '�̐��_�͓��Ă͂܂�Ȃ��B
    '�ނ���A�����ȍ~�ōs���鏈���ł́A���������������[�U�����̏�ŔF�����邱�Ƃ�
    '���҂ł��Ȃ����A����̂���@��ōs����́A���̓f�[�^�̏��������肵�Ă���\����
    '�������߁A�����ŃK�[�h���Ȃ���΂Ȃ�Ȃ��B
    '�ȏ�̗��R����A��̒l���_�u���N�H�[�e�[�V�����ň͂܂�Ă���t�@�C���Ȃǂ�
    'CSV�Ƃ��Ă͐������Ă��A�K�p���X�g�̎d�l�Ƃ��Ē�`����Ă��鏑���Ƃ͈قȂ邽�߁A
    '������NG�ɂȂ�悤�ɏ������s���B
    '�K�p���Ɋւ��ẮA�K�p���X�g�̎d�l���Ɂu���t�ł�99999999�ł��u�����N�ł��Ȃ��v�ꍇ��
    '���삪�L�ڂ���Ă���悤�ɂ݂��邪�A����͉w���@�푤���z�肷�ׂ��P�[�X�Ƃ���
    '�L�ڂ���Ă��邾���ł���A�^�ǃT�[�o�����e���ׂ��P�[�X�ł͂Ȃ����̂Ƃ���B
    Protected Overridable Function RegisterCurDllObjectAsList(ByVal sFilePath As String, ByVal sFileName As String, ByVal sFileHashValue As String) As NakCauseCode
        Log.Info("Registering a list file [" & sFilePath & "]...")

        Dim ret As NakCauseCode = EkNakCauseCode.InvalidContent
        Do
            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()

                Dim sLine As String
                Dim aColumns As String()
                Using oReader As StreamReader _
                   = New StreamReader(sFilePath, Encoding.GetEncoding(932))

                    '�w�b�_���̂P�s�ڂ�ǂݍ��ށB
                    sLine = oReader.ReadLine()
                    If sLine Is Nothing Then
                        Log.Error("The file is empty.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '�w�b�_���̂P�s�ڂ��ɕ�������B
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 2 Then
                        Log.Error("The first line of the file contains too many or too few columns.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '�쐬�N�����𒊏o����B
                    Dim sCreatedDate As String = aColumns(0)
                    Dim createdDate As DateTime
                    If DateTime.TryParseExact(sCreatedDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                        Log.Error("The first line of the file contains illegal created date.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '�K�p���X�g�o�[�W�����𒊏o����B
                    Dim sListVersion As String = aColumns(1)
                    If Not EkMasProListFileName.GetListVersion(sFileName).Equals(sListVersion) Then
                        Log.Error("The first line of the file contains illegal list version.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '�w�b�_���̂Q�s�ڂ�ǂݍ��ށB
                    sLine = oReader.ReadLine()
                    If sLine Is Nothing Then
                        Log.Error("The file does not have the second line.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '�w�b�_���̂Q�s�ڂ��ɕ�������B
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 3 Then
                        Log.Error("The second line of the file contains too many or too few columns.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    'NOTE: �S�G���A�Ή��̃v���O������z�M����ۂ��K�p���X�g�̓G���A�ʂɗp�ӂł���
                    '�d�l�ɂ���̂ł���΁ADataPurpose��Program�ł���aColumns(0)��"00"�̃P�[�X��
                    '���e���Ȃ���΂Ȃ�Ȃ��B�܂��A�K�p���X�g�����ɋL�ڂ��ꂽ�iCAB�ɕR�Â��j
                    '�G���A��00�ł��邱�Ƃ�DB�ɋL�����A�z�M�w���ł��̃��R�[�h���w�肳�ꂽ�ۂ́A
                    '�G���ANo��00��CAB��ǂݏo���Ȃ���΂Ȃ�Ȃ��B

                    '�p�^�[��No.�܂��̓G���ANo.�𒊏o����B
                    If Not curDll.DataSubKind.Equals(aColumns(0)) Then
                        Log.Error("The second line of the file contains illegal sub kind.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '�}�X�^�o�[�W�����܂��͑�\�o�[�W�����𒊏o����B
                    'NOTE: �v���O�����K�p���X�g�̎d�l�ɍ��킹�āA��r����
                    '�i�t�@�C��������擾�����o�[�W�����j�̌����𒲐����Ă���B
                    '�v���O�����K�p���X�g�̎d�l�������łȂ��i���D�@�v���O������
                    '�����v���O�����̑�\�o�[�W�������S���ŋL�q����j�Ȃ�΁A
                    '���LIf���̏����́uNot curDll.DataVersion.Equals(aColumns(1))�v
                    '�ɂ���ׂ��ł���B
                    Dim sVerFormat As String = If(curDll.DataPurpose.Equals(EkConstants.DataPurposeMaster), "D3", "D8")
                    If Not Integer.Parse(curDll.DataVersion).ToString(sVerFormat).Equals(aColumns(1)) Then
                        Log.Error("The second line of the file contains illegal version.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '�@��R�[�h�𒊏o����B
                    If Not curDll.DataApplicableModel.Equals(aColumns(2)) Then
                        Log.Error("The second line of the file contains illegal model code.")
                        ret = EkNakCauseCode.InvalidContent
                        Exit Do
                    End If

                    '�K�p���X�g���o���e�[�u���ɓ���̃t�@�C�����ɕR�Â������̃��R�[�h������΍폜����B
                    Dim sSQLToDeleteHeadline As String = _
                       "DELETE FROM S_" & curDll.DataPurpose & "_LIST_HEADLINE" _
                       & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                       & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                       & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                       & " AND DATA_VERSION = '" & curDll.DataVersion & "'" _
                       & " AND LIST_VERSION = '" & sListVersion & "'"
                    dbCtl.ExecuteSQLToWrite(sSQLToDeleteHeadline)

                    '�K�p���X�g���o���e�[�u���ɏ���o�^����B
                    Dim sSQLToInsertHeadline As String = _
                       "INSERT INTO S_" & curDll.DataPurpose & "_LIST_HEADLINE" _
                       & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, LIST_VERSION, FILE_NAME, HASH_VALUE, FILE_CREATE_DATE)" _
                       & " VALUES (GETDATE()," _
                               & " '" & UserId & "'," _
                               & " '" & MachineId & "'," _
                               & " GETDATE()," _
                               & " '" & UserId & "'," _
                               & " '" & MachineId & "'," _
                               & " '" & curDll.DataApplicableModel & "'," _
                               & " '" & curDll.DataKind & "'," _
                               & " '" & curDll.DataSubKind & "'," _
                               & " '" & curDll.DataVersion & "'," _
                               & " '" & sListVersion & "'," _
                               & " '" & sFileName & "'," _
                               & " '" & sFileHashValue & "'," _
                               & " '" & sCreatedDate & "')"
                    dbCtl.ExecuteSQLToWrite(sSQLToInsertHeadline)

                    '�K�p���X�g���e�e�[�u���ɓ���̃t�@�C�����ɕR�Â������̃��R�[�h������΍폜����B
                    Dim sSQLToDelete As String = _
                       "DELETE FROM S_" & curDll.DataPurpose & "_LIST" _
                       & " WHERE FILE_NAME = '" & sFileName & "'"
                    dbCtl.ExecuteSQLToWrite(sSQLToDelete)

                    '�f�[�^������͂���B
                    Dim idealColumnCount As Integer = If(curDll.DataPurpose.Equals(EkConstants.DataPurposeMaster), 3, 4)
                    Dim oAboveLines As New LinkedList(Of String)
                    Dim lineNumber As Integer = 3
                    sLine = oReader.ReadLine()
                    While sLine IsNot Nothing
                        '�ǂݍ��񂾍s���ɕ�������B
                        aColumns = sLine.Split(","c)
                        If aColumns.Length <> idealColumnCount Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains too many or too few columns.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If

                        '�T�C�o�l����w���R�[�h�̏������`�F�b�N����B
                        If aColumns(0).Length <> 6 OrElse _
                           Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal station code.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If

                        '�R�[�i�[�R�[�h�̏������`�F�b�N����B
                        If aColumns(1).Length <> 4 OrElse _
                           Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal corner code.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If

                        '���@�ԍ��̏������`�F�b�N����B
                        If aColumns(2).Length <> 2 OrElse _
                           Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                           aColumns(2).Equals("00") Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal unit number.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If

                        If idealColumnCount = 4 Then
                            '�K�p���̃����O�X���`�F�b�N����B
                            If aColumns(3).Length <> 8 AndAlso aColumns(3).Length <> 0 Then
                                Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal applicable date.")
                                ret = EkNakCauseCode.InvalidContent
                                Exit Do
                            End If

                            '�K�p�����u�����N�łȂ��ꍇ�A�l���`�F�b�N����B
                            If aColumns(3).Length = 8 Then
                               If Not Utility.IsDecimalStringFixed(aColumns(3), 0, 8) Then
                                    Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal applicable date.")
                                    ret = EkNakCauseCode.InvalidContent
                                    Exit Do
                                End If

                                If Not aColumns(3).Equals("99999999") AndAlso _
                                   DateTime.TryParseExact(aColumns(3), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                                    Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal applicable date.")
                                    ret = EkNakCauseCode.InvalidContent
                                    Exit Do
                                End If
                            End If
                        End If

                        Dim sLineKey As String = aColumns(0) & aColumns(1) & aColumns(2)
                        If oAboveLines.Contains(sLineKey) Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file is duplicative.")
                            ret = EkNakCauseCode.InvalidContent
                            Exit Do
                        End If
                        oAboveLines.AddLast(sLineKey)

                        '�s�̏���K�p���X�g���e�e�[�u���ɓo�^����B
                        Dim sSQLToInsert As String
                        If idealColumnCount = 3 Then
                            sSQLToInsert = _
                               "INSERT INTO S_" & curDll.DataPurpose & "_LIST" _
                               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO)" _
                               & " VALUES (GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " '" & sFileName & "'," _
                                       & " '" & aColumns(0).Substring(0, 3) & "'," _
                                       & " '" & aColumns(0).Substring(3, 3) & "'," _
                                       & " " & aColumns(1) & "," _
                                       & " " & aColumns(2) & ")"
                        Else
                            sSQLToInsert = _
                               "INSERT INTO S_" & curDll.DataPurpose & "_LIST" _
                               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO, APPLICABLE_DATE)" _
                               & " VALUES (GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " '" & sFileName & "'," _
                                       & " '" & aColumns(0).Substring(0, 3) & "'," _
                                       & " '" & aColumns(0).Substring(3, 3) & "'," _
                                       & " " & aColumns(1) & "," _
                                       & " " & aColumns(2) & "," _
                                       & " '" & aColumns(3) & "')"
                        End If
                        dbCtl.ExecuteSQLToWrite(sSQLToInsert)

                        sLine = oReader.ReadLine()
                        lineNumber += 1
                    End While
                End Using
                dbCtl.TransactionCommit()
                ret = EkNakCauseCode.None

            Catch ex As DatabaseException
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.TelegramError  'NOTE: ����

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent  'NOTE: ����

            Finally
                If ret <> EkNakCauseCode.None Then
                    dbCtl.TransactionRollBack()
                End If
                dbCtl.ConnectClose()
            End Try
        Loop While False
        Return ret
    End Function

    Protected Overridable Function RegisterCurDllObjectAsMasterData(ByVal sFilePath As String, ByVal sFileName As String, ByVal sFileHashValue As String) As NakCauseCode
        Log.Info("Registering a master data file [" & sFilePath & "]...")

        Dim ret As NakCauseCode = EkNakCauseCode.InvalidContent
        Do
            Dim oFooter As EkMasterDataFileFooter
            Try
                oFooter = New EkMasterDataFileFooter(sFilePath)
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End Try

            Dim sFooterViolation As String = oFooter.GetFormatViolation()
            If sFooterViolation IsNot Nothing Then
                Log.Error("Footer format error detected." & vbCrLf & sFooterViolation)
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            If Not curDll.DataApplicableModel.Equals(oFooter.ApplicableModel) Then
                Log.Error("ApplicableModel values differ in file name and file footer.")
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            If Not curDll.DataKind.Equals(oFooter.Kind) Then
                Log.Error("Kind values differ in file name and file footer.")
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            If Not curDll.DataVersion.Equals(oFooter.Version) Then
                Log.Error("Version values differ in file name and file footer.")
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()

                '�}�X�^�f�[�^���o���e�[�u���ɓ���̃t�@�C�����ɕR�Â������̃��R�[�h������΍폜����B
                Dim sSQLToDeleteHeadline As String = _
                   "DELETE FROM S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
                   & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                   & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                   & " AND DATA_VERSION = '" & curDll.DataVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteHeadline)

                '�}�X�^�f�[�^���o���e�[�u���ɏ���o�^����B
                Dim sSQLToInsertHeadline As String = _
                   "INSERT INTO S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
                   & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, FILE_NAME, HASH_VALUE)" _
                   & " VALUES (GETDATE()," _
                           & " '" & UserId & "'," _
                           & " '" & MachineId & "'," _
                           & " GETDATE()," _
                           & " '" & UserId & "'," _
                           & " '" & MachineId & "'," _
                           & " '" & curDll.DataApplicableModel & "'," _
                           & " '" & curDll.DataKind & "'," _
                           & " '" & curDll.DataSubKind & "'," _
                           & " '" & curDll.DataVersion & "'," _
                           & " '" & sFileName & "'," _
                           & " '" & sFileHashValue & "')"
                dbCtl.ExecuteSQLToWrite(sSQLToInsertHeadline)

                dbCtl.TransactionCommit()
                ret = EkNakCauseCode.None

            Catch ex As DatabaseException
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.TelegramError  'NOTE: ����

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent  'NOTE: ����

            Finally
                If ret <> EkNakCauseCode.None Then
                    dbCtl.TransactionRollBack()
                End If
                dbCtl.ConnectClose()
            End Try
        Loop While False
        Return ret
    End Function

    Protected Overridable Function RegisterCurDllObjectAsProgramData(ByVal sFilePath As String, ByVal sFileName As String, ByVal sFileHashValue As String) As NakCauseCode
        Log.Info("Registering a program data file [" & sFilePath & "]...")

        Dim ret As NakCauseCode = EkNakCauseCode.InvalidContent
        Do
            '�ꎞ��Ɨp�f�B���N�g��������������B
            Log.Info("Initializing directory [" & sTempDirPath & "]...")
            Utility.DeleteTemporalDirectory(sTempDirPath)
            Directory.CreateDirectory(sTempDirPath)

            'CAB��W�J����B
            Using oProcess As New System.Diagnostics.Process()
                oProcess.StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, "TsbCab.exe")
                oProcess.StartInfo.Arguments = "-x """ & sFilePath & """ """ & sTempDirPath & "\"""
                oProcess.StartInfo.UseShellExecute = False
                oProcess.StartInfo.RedirectStandardInput = True
                oProcess.StartInfo.CreateNoWindow = True
                oProcess.Start()
                Dim oStreamWriter As StreamWriter = oProcess.StandardInput
                oStreamWriter.WriteLine("")
                oStreamWriter.Close()
                oProcess.WaitForExit()
            End Using

            '�v���O�����o�[�W�������X�g�̃p�X���擾����B
            Dim sVerListPath As String
            If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeKanshiban) Then
                sVerListPath = Config.KsbProgramVersionListPathInCab
            ElseIf curDll.DataApplicableModel.Equals(EkConstants.ModelCodeGate) Then
                sVerListPath = Config.GateProgramVersionListPathInCab
            Else
                sVerListPath = Config.MadoProgramVersionListPathInCab
            End If
            sVerListPath = Utility.CombinePathWithVirtualPath(sTempDirPath, sVerListPath)

            '�v���O�����o�[�W�������X�g����@�틤�ʕ���ǂݏo���B
            Dim oVerList As EkProgramVersionListHeader
            Try
                oVerList = New EkProgramVersionListHeader(sVerListPath)
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End Try

            '�ǂݏo�����@�틤�ʕ��̏������`�F�b�N����B
            Dim sVerListViolation As String = oVerList.GetFormatViolation()
            If sVerListViolation IsNot Nothing Then
                Log.Error("Format error detected in ProgramVersionList file." & vbCrLf & sVerListViolation)
                ret = EkNakCauseCode.InvalidContent
                Exit Do
            End If

            '�ǂݏo�����@�틤�ʕ����瓮�싖�����擾����B
            Dim runnableDate As DateTime = oVerList.RunnableDate

            '�S�Ẵv���O�����O���[�v�̃x�[�X�p�X�ƁA
            '�e�O���[�v�̃f�B���N�g�����̔z�񂨂�сA
            '�e�O���[�v�̕\�����̔z����擾����B
            'TODO: �Ď���CAB�����D�@CAB�Ɠ������@�ŏ�������ꍇ��
            'Config�ɊĎ��Ղ�ProgramGroup�Ɋւ���t�B�[���h��p�ӂ��A
            '���̎Q�Ƃ������ŉ��L�ϐ��ɃZ�b�g���邱�ƁB
            Dim sGroupBasePath As String = Nothing
            Dim aGroupNames As String() = Nothing
            Dim aGroupTitles As String() = Nothing
            If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeGate) Then
                sGroupBasePath = Config.GateProgramGroupBasePathInCab
                aGroupNames = Config.GateProgramGroupNamesInCab
                aGroupTitles = Config.GateProgramGroupTitles
            End If

            Dim dbCtl As New DatabaseTalker()
            Try
                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()

                '�v���O�����f�[�^���o���e�[�u���ɓ���̃t�@�C�����ɕR�Â������̃��R�[�h������΍폜����B
                Dim sSQLToDeleteHeadline As String = _
                   "DELETE FROM S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
                   & " WHERE MODEL_CODE = '" & curDll.DataApplicableModel & "'" _
                   & " AND DATA_KIND = '" & curDll.DataKind & "'" _
                   & " AND DATA_SUB_KIND = '" & curDll.DataSubKind & "'" _
                   & " AND DATA_VERSION = '" & curDll.DataVersion & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDeleteHeadline)

                '�v���O�����f�[�^���e�e�[�u���ɓ���̃t�@�C�����ɕR�Â������̃��R�[�h������΍폜����B
                'NOTE: ���Ƃ��v���O�����o�[�W�������X�g�̃v���O�����敪���u����DLL�v�ł����Ă��A
                '�S�t�@�C���̏�������������ŁACAB�Ŏ��������݂̂̂�o�^����B
                '�����炭�A���D�@��CAB�ɂ͍�����������ꂽ���̂͑��݂����A�Ď��Ղ�CAB��
                '�t�@�C�����P�ł���̂ɍ���DLL�ƑS��DLL�ɈႢ���Ȃ��A�����ɂ��ẮA
                '���Ƃ������ł����Ă��A�v���O�����o�[�W�������X�g�ɑS�t�@�C���̏��
                '�i�[����Ă��邽�߁A����ł悢���̂ƍl������B
                Dim sSQLToDelete As String = _
                   "DELETE FROM S_" & curDll.DataPurpose & "_DATA" _
                   & " WHERE FILE_NAME = '" & sFileName & "'"
                dbCtl.ExecuteSQLToWrite(sSQLToDelete)

                '�v���O�����f�[�^���o���e�[�u���ɏ���o�^����B
                Dim sSQLToInsertHeadline As String = _
                   "INSERT INTO S_" & curDll.DataPurpose & "_DATA_HEADLINE" _
                   & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION, FILE_NAME, HASH_VALUE, RUNNABLE_DATE)" _
                   & " VALUES (GETDATE()," _
                           & " '" & UserId & "'," _
                           & " '" & MachineId & "'," _
                           & " GETDATE()," _
                           & " '" & UserId & "'," _
                           & " '" & MachineId & "'," _
                           & " '" & curDll.DataApplicableModel & "'," _
                           & " '" & curDll.DataKind & "'," _
                           & " '" & curDll.DataSubKind & "'," _
                           & " '" & curDll.DataVersion & "'," _
                           & " '" & sFileName & "'," _
                           & " '" & sFileHashValue & "'," _
                           & " '" & runnableDate.ToString("yyyyMMdd") & "')"
                dbCtl.ExecuteSQLToWrite(sSQLToInsertHeadline)

                '�v���O�����f�[�^���e�e�[�u���ɏ���o�^����B
                'TODO: �Ď���CAB�����D�@CAB�Ɠ����悤�ɁA����f�B���N�g����FILELIST.TXT��p�ӂ��A
                '��������Q�Ƃ����t�@�C���̃t�b�^�Ƀo�[�W�������i�[����̂ł���΁A
                '�ȉ��̊Ď��Ր�p�̏����͏������A���́uElseIf ...�v��
                '�uIf sGroupBasePath IsNot Nothing�v�ɂ���ׂ��ł���B
                If curDll.DataApplicableModel.Equals(EkConstants.ModelCodeKanshiban) Then
                    '�o�^������𐬌^����B
                    'NOTE: �Ď��Ղ����M����v���O�����o�[�W�������̎d�l��
                    '���킹��B����sElementId�̃t�@�C���������ɂ͒��ӁB
                    Dim sElementId As String = "00\            "
                    Dim sVersion As String = oVerList.EntireVersion.ToString(EkConstants.ProgramDataVersionFormatOfKanshiban)
                    Dim sDispName As String = "�Ď��ՃA�v���P�[�V����"

                    '�v���O�����f�[�^���e�e�[�u���ɓo�^����B
                    Dim sSQLToInsert As String
                    sSQLToInsert = _
                       "INSERT INTO S_" & curDll.DataPurpose & "_DATA" _
                       & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
                       & " VALUES (GETDATE()," _
                               & " '" & UserId & "'," _
                               & " '" & MachineId & "'," _
                               & " GETDATE()," _
                               & " '" & UserId & "'," _
                               & " '" & MachineId & "'," _
                               & " '" & sFileName & "'," _
                               & " '" & sElementId & "'," _
                               & " '" & sVersion & "'," _
                               & " '" & sDispName & "')"
                    dbCtl.ExecuteSQLToWrite(sSQLToInsert)

                ElseIf curDll.DataApplicableModel.Equals(EkConstants.ModelCodeGate) Then
                    Dim sBaseDirPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, sGroupBasePath)

                    'CAB���̏���f�B���N�g�������ɏ�������B
                    For i As Integer = 0 To aGroupNames.Length - 1
                        'NOTE: aGroupNames(i)�̒�����0�̏ꍇ���z�肷��isBaseDirPath��������t�@�C����ǂށj
                        '�d�l�ł��邪�A�����I�ɂ�Path.Combine()�̔z���Ɉς˂邱�Ƃɂ��Ă���B
                        Dim sDirPath As String = Path.Combine(sBaseDirPath, aGroupNames(i))
                        Dim sLine As String

                        '�f�B���N�g�����ɂ��錩�o���t�@�C������͂���B
                        Using oReader As StreamReader _
                           = New StreamReader(Path.Combine(sDirPath, CatalogNameInCabDir), Encoding.GetEncoding(932))

                            '���o���t�@�C���̊e�s����������B
                            Dim lineNumber As Integer = 1
                            sLine = oReader.ReadLine()
                            While sLine IsNot Nothing
                                If Not sLine.StartsWith("/", StringComparison.Ordinal) Then
                                    '���o���t�@�C���̔�R�����g�s����o�[�W�����Ǘ��ΏۂƂȂ�t�@�C���̖��O���擾����B
                                    Dim sElementFileName As String = sLine.Substring(2, 16).TrimEnd(Chr(&H20))
                                    If Not Path.GetFileName(sElementFileName).Equals(sElementFileName, StringComparison.OrdinalIgnoreCase) Then
                                        Log.Error("The line #" & lineNumber.ToString() & " of [" &  Path.Combine(aGroupNames(i), CatalogNameInCabDir) & "] contains illegal file name [" & sElementFileName  & "].")
                                        ret = EkNakCauseCode.InvalidContent
                                        Exit Do
                                    End If

                                    '�t�@�C���̃t�b�^��ǂݏo���B
                                    Dim sElementFilePath As String = Path.Combine(sDirPath, sElementFileName)
                                    Dim oFooter As EkProgramElementFooter
                                    Try
                                        'TODO: �Ď���CAB�������ŏ������邱�ƂɂȂ����ꍇ�́A
                                        'curDll.DataApplicableModel.Equals(EkConstants.ModelCodeKanshiban)
                                        '�̏ꍇ�ɁuEkProgramElementFooterForW�v�C���X�^���X�𐶐�����悤
                                        '�����𕪊򂳂��邱�ƁB
                                        oFooter = New EkProgramElementFooterForG(sElementFilePath)
                                    Catch ex As IOException
                                        Log.Error("Exception caught in parsing [" & Path.Combine(aGroupNames(i), sElementFileName) & "].", ex)
                                        ret = EkNakCauseCode.InvalidContent
                                        Exit Do
                                    End Try

                                    '�ǂݏo�����t�b�^�̏������`�F�b�N����B
                                    Dim sFooterViolation As String = oFooter.GetFormatViolation()
                                    If sFooterViolation IsNot Nothing Then
                                        Log.Error("Footer format error detected in [" & Path.Combine(aGroupNames(i), sElementFileName) & "]." & vbCrLf & sFooterViolation)
                                        ret = EkNakCauseCode.InvalidContent
                                        Exit Do
                                    End If

                                    'NOTE: �����f�B���N�g���Ɋg���q�݂̂��قȂ�t�@�C�����i�[����Ă���ꍇ�A
                                    '����\�����ł����̍s���o�^�����͂��ł��邪�A���������A�����
                                    '���D�@�p�ɂ́A���̂悤��CAB�͗p�ӂ��Ȃ����ƂɂȂ��Ă���B

                                    '�t�b�^�̏��𐬌^����B
                                    Dim sElementId As String = i.ToString("D2") & "\" & sElementFileName.ToUpperInvariant()
                                    Dim sVersion As String = oFooter.Version
                                    Dim sDispName As String = oFooter.DispName
                                    If aGroupTitles(i).Length <> 0 Then
                                        sDispName = aGroupTitles(i) & "\" & Path.GetFileNameWithoutExtension(sElementFileName)
                                    End If

                                    '���^���������v���O�����f�[�^���e�e�[�u���ɓo�^����B
                                    Dim sSQLToInsert As String
                                    sSQLToInsert = _
                                       "INSERT INTO S_" & curDll.DataPurpose & "_DATA" _
                                       & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
                                       & " VALUES (GETDATE()," _
                                               & " '" & UserId & "'," _
                                               & " '" & MachineId & "'," _
                                               & " GETDATE()," _
                                               & " '" & UserId & "'," _
                                               & " '" & MachineId & "'," _
                                               & " '" & sFileName & "'," _
                                               & " '" & sElementId & "'," _
                                               & " '" & sVersion & "'," _
                                               & " '" & sDispName & "')"
                                    dbCtl.ExecuteSQLToWrite(sSQLToInsert)
                                End If

                                sLine = oReader.ReadLine()
                                lineNumber += 1
                            End While
                        End Using
                    Next

                Else
                    'NOTE: �K�p�Ώۋ@�킪�����̏ꍇ�ł���B
                    Dim aElements As EkMadoProgramVersionInfoElement()

                    '�v���O�����o�[�W�������X�g���瑋���̃v���O�����o�[�W��������ǂݏo���B
                    Using oInputStream As New FileStream(sVerListPath, FileMode.Open, FileAccess.Read)
                        aElements = EkMadoProgramVersionInfoReader.GetElementsFromStream(oInputStream)
                    End Using

                    '�ǂݏo�����o�[�W�������̊e���R�[�h����������B
                    For i As Integer = 0 To aElements.Length - 1
                        If aElements(i).IsVersion Then
                            '�\���Ώۃ��R�[�h�̏ꍇ�́A���𐬌^����B
                            Dim sElementId As String = i.ToString("D2")
                            Dim sVersion As String = aElements(i).Value
                            Dim sDispName As String = aElements(i).Name.Replace("�o�[�W����", "")

                            '���^���������v���O�����f�[�^���e�e�[�u���ɓo�^����B
                            Dim sSQLToInsert As String
                            sSQLToInsert = _
                               "INSERT INTO S_" & curDll.DataPurpose & "_DATA" _
                               & " (INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, FILE_NAME, ELEMENT_ID, ELEMENT_VERSION, ELEMENT_NAME)" _
                               & " VALUES (GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " GETDATE()," _
                                       & " '" & UserId & "'," _
                                       & " '" & MachineId & "'," _
                                       & " '" & sFileName & "'," _
                                       & " '" & sElementId & "'," _
                                       & " '" & sVersion & "'," _
                                       & " '" & sDispName & "')"
                            dbCtl.ExecuteSQLToWrite(sSQLToInsert)
                        End If
                    Next
                End If

                dbCtl.TransactionCommit()
                ret = EkNakCauseCode.None

            Catch ex As DatabaseException
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.TelegramError  'NOTE: ����

            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent

            Catch ex As FormatException
                Log.Error("Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                ret = EkNakCauseCode.InvalidContent  'NOTE: ����

            Finally
                If ret <> EkNakCauseCode.None Then
                    dbCtl.TransactionRollBack()
                End If
                dbCtl.ConnectClose()
            End Try
        Loop While False

        '�ꎞ��Ɨp�f�B���N�g�����폜����B
        Log.Info("Sweeping directory [" & sTempDirPath & "]...")
        Utility.DeleteTemporalDirectory(sTempDirPath)

        Return ret
    End Function
#End Region

End Class

''' <summary>
''' �����ԁB
''' </summary>
Public Enum LineStatus As Integer
    Initial
    Steady
    Disconnected
End Enum
