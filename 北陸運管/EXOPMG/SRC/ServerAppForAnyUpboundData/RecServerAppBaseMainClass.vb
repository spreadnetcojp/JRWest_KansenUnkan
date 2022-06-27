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
Imports JR.ExOpmg.DBCommon

''' <summary>
''' �o�^�v���Z�X���ʂ̃��C����������������N���X�B
''' </summary>
Public Class RecServerAppBaseMainClass
    Inherits ServerAppBaseMainClass

#Region "�����N���X��"
    Protected Delegate Function RecordToDatabaseDelegate(ByVal sFilePath As String) As RecordingResult

    Protected Enum RecordingResult As Integer
        Success
        IOError
        ParseError
    End Enum
#End Region

#Region "�萔��ϐ�"
    '���C���E�B���h�E
    Protected Shared oMainForm As ServerAppForm

    '�������f�[�^���i�[����Ă���f�B���N�g���̃p�X
    Protected Shared sInputDirPath As String

    '�����s���œo�^�ł��Ȃ������f�[�^���i�[����f�B���N�g���̃p�X
    Protected Shared sSuspenseDirPath As String

    '�����ُ�œo�^�ł��Ȃ������f�[�^���i�[����f�B���N�g���̃p�X
    Protected Shared sQuarantineDirPath As String

    '�o�^�ς݃f�[�^���i�[����f�B���N�g���̃p�X
    Protected Shared sTrashDirPath As String

    '�}�Ԃ̍ő�l
    Private Shared maxBranchNumber As Integer

    '�f�[�^�o�^�X���b�h�ւ̏I���v���t���O
    Private Shared quitListener As Integer

    '�f�[�^�o�^���\�b�h�ւ̃f���Q�[�g
    Private Shared oRecordToDatabaseDelegate As RecordToDatabaseDelegate
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �o�^�v���Z�X�̋��ʃ��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �e�o�^�v���Z�X�̃��C����������Ăяo���B
    ''' </remarks>
    Protected Shared Sub RecServerAppBaseMain(ByVal oArgRecordToDatabaseDelegate As RecordToDatabaseDelegate)
        Try
            oRecordToDatabaseDelegate = oArgRecordToDatabaseDelegate

            '���b�Z�[�W���[�v���A�C�h����ԂɂȂ�O�i���A����I�ɂ�����s��
            '�X���b�h���N������O�j�ɁA�����ؖ��t�@�C�����X�V���Ă����B
            Directory.CreateDirectory(RecServerAppBaseConfig.ResidentAppPulseDirPath)
            ServerAppPulser.Pulse()

            oMainForm = New ServerAppForm()

            '�f�[�^�o�^�X���b�h���J�n����B
            Dim oRecorderThread As New Thread(AddressOf RecServerAppBaseMainClass.RecordingLoop)
            Log.Info("Starting the recorder thread...")
            quitListener = 0
            oRecorderThread.Name = "Recorder"
            oRecorderThread.Start()

            '�E�C���h�E�v���V�[�W�������s����B
            'NOTE: ���̃��\�b�h�����O���X���[����邱�Ƃ͂Ȃ��B
            ServerAppBaseMain(oMainForm)

            Try
                '�f�[�^�o�^�X���b�h�ɏI����v������B
                Log.Info("Sending quit request to the recorder thread...")
                Thread.VolatileWrite(quitListener, 1)

                'NOTE: �ȉ��Ńf�[�^�o�^�X���b�h���I�����Ȃ��ꍇ�A
                '�f�[�^�o�^�X���b�h�͐����ؖ����s��Ȃ��͂��ł���A
                '�󋵂ւ̑Ώ��̓v���Z�X�}�l�[�W���ōs����z��ł���B

                '�f�[�^�o�^�X���b�h�̏I����҂B
                Log.Info("Waiting for the recorder thread to quit...")
                oRecorderThread.Join()
                Log.Info("The recorder thread has quit.")
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                oRecorderThread.Abort()
            End Try
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            If oMainForm IsNot Nothing Then
                oMainForm.Dispose()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' �f�[�^�o�^�X���b�h�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �f�[�^�o�^���s���B
    ''' </remarks>
    Private Shared Sub RecordingLoop()
        Dim oMessageQueue As MessageQueue = Nothing
        Try
            Log.Info("The recorder thread started.")

            sInputDirPath = RecServerAppBaseConfig.InputDirPathForApps(RecServerAppBaseConfig.AppIdentifier)
            sSuspenseDirPath = RecServerAppBaseConfig.SuspenseDirPathForApps(RecServerAppBaseConfig.AppIdentifier)
            sQuarantineDirPath = RecServerAppBaseConfig.QuarantineDirPathForApps(RecServerAppBaseConfig.AppIdentifier)
            sTrashDirPath = RecServerAppBaseConfig.TrashDirPathForApps(RecServerAppBaseConfig.AppIdentifier)
            maxBranchNumber = RecServerAppBaseConfig.MaxBranchNumberForApps(RecServerAppBaseConfig.AppIdentifier)

            '�A�N�Z�X����\��̃f�B���N�g���ɂ��āA������΍쐬���Ă����B
            'NOTE: ���N���X���쐬������̂�A�K���T�u�f�B���N�g���̍쐬����
            '�s�����ƂɂȂ���̂ɂ��ẮA�ΏۊO�Ƃ���B
            Directory.CreateDirectory(sInputDirPath)

            Dim oDiagnosisTimer As New TickTimer(RecServerAppBaseConfig.SelfDiagnosisIntervalTicks)
            Dim isInitial As Boolean = True
            Dim fewSpan As New TimeSpan(0, 0, 0, 0, RecServerAppBaseConfig.PollIntervalTicks)
            Dim oFilter As New MessagePropertyFilter()
            oFilter.ClearAll()
            'oFilter.AppSpecific = True
            'oFilter.Body = True

            'NOTE: RecServerAppBaseConfig.MyMqPath�̃��b�Z�[�W�L���[�́A
            '���̎��_�ŕK�����݂��Ă���O��ł���B���݂��Ă��Ȃ���΁A
            '�V�X�e���Ɉُ킪����́A���̃v���Z�X�͋N������ɏI������ׂ�
            '�ł���B
            oMessageQueue = New MessageQueue(RecServerAppBaseConfig.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([String])})

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())
            While Thread.VolatileRead(quitListener) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()
                End If

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
                '�s���邱�Ƃ͂Ȃ����낤���A������TimeSpan.Zero��n���āA
                '�ʂ̕��@��CPU�̉�����Ԃ���邷��悤�ɂ���΁A
                '���b�Z�[�W��M�ɑ΂��锽���������Ȃ�i���b�Z�[�W����
                '���\���ቺ����j�͂��ł��邽�߁A�ȉ��̂Ƃ���A
                'MessageQueue.Receive()�ő҂��Ƃɂ��Ă���B
                Try
                    oMessageQueue.Receive(fewSpan)
                Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                    '�v���Z�X���N�����Ă����x�ł��o�^���������{���Ă���Ȃ�΁A
                    '���b�Z�[�W��M�҂��ɖ߂�B
                    If Not isInitial Then Continue While
                End Try

                isInitial = False
                While Thread.VolatileRead(quitListener) = 0
                    systemTick = TickTimer.GetSystemTick()
                    If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                        oDiagnosisTimer.Start(systemTick)
                        ServerAppPulser.Pulse()
                    End If

                    '�L���[�C���O����Ă��郁�b�Z�[�W��S�ēǂݎ̂Ă�B
                    oMessageQueue.Purge()

                    '�ł��Â��t�@�C�����P����������B
                    '�t�@�C�����Ȃ��ꍇ�́A���b�Z�[�W��M�҂��ɖ߂�B
                    If DispatchEarliestFile() = False Then Exit While
                End While
            End While
            Log.Info("Quit requested by manager.")
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j�́A
            '�v���Z�X�}�l�[�W�����s���̂ŁA�����ł͕s�v�ł���B

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        Finally
            If oMessageQueue IsNot Nothing Then
                oMessageQueue.Close()
            End If
        End Try
    End Sub

    Private Shared Function DispatchEarliestFile() As Boolean
        '�������f�[�^�̊i�[�f�B���N�g�����珊��p�^�[���̖��O������
        '�ŌẪt�@�C������������B
        'NOTE: �������f�[�^�̊i�[�f�B���N�g���́A���̎��_�Łi����
        '�v���Z�X�̌����ŃA�N�Z�X�\�ȏ�ԂŁj�K�����݂��Ă���
        '�Ƃ����O��ł���B���݂��Ă��Ȃ���΁A�V�X�e���Ɉُ킪
        '����́A���̃v���Z�X�͋N������ɏI������ׂ��ł���B
        Dim oEarliestFileInfo As FileInfo = UpboundDataPath.FindEarliest(sInputDirPath)
        If oEarliestFileInfo Is Nothing Then Return False

        '�t�@�C���̓��e���f�[�^�x�[�X�ɔ��f����B
        Log.Info("�t�@�C��[" & oEarliestFileInfo.Name & "]�̓o�^���s���܂�...")
        Dim result As RecordingResult = oRecordToDatabaseDelegate(oEarliestFileInfo.FullName)

        '���f�̌��ʂɂ��A�t�@�C���̐V�p�X�̌��ʕʃf�B���N�g���܂ł����߂�B
        Dim sDestPath As String
        Select Case result
            Case RecordingResult.Success
                sDestPath = sTrashDirPath
            Case RecordingResult.IOError
                sDestPath = sSuspenseDirPath
            Case RecordingResult.ParseError
                sDestPath = sQuarantineDirPath
            Case Else
                Debug.Fail("This case is impermissible.")
                Return True
        End Select

        '�t�@�C�����́u�N�����v���������ƂɁA�t�@�C���̐V�p�X�̓��t�ʃf�B���N�g���܂ł����߂�B
        sDestPath = Path.Combine(sDestPath, UpboundDataPath.GetDateString(oEarliestFileInfo.Name))

        'NOTE: sDestPath�ƏՓ˂��閼�O�̃t�@�C���͑��݂��Ȃ��Ƃ����O��ł���B
        '�܂��f�B���N�g���Ƃ��Ċ��ɑ��݂��Ă���ꍇ�́A�����\�Ƃ����O��ł���B
        If Not Directory.Exists(sDestPath) Then
            '�f�B���N�g�������݂��Ă��Ȃ��ꍇ�ł���B
            '�f�B���N�g�����쐬������ŁA���p�X�̃t�@�C���������̂܂܌������āA
            '�V�p�X������������B
            Directory.CreateDirectory(sDestPath)
            sDestPath = Path.Combine(sDestPath, oEarliestFileInfo.Name)
        Else
            '�f�B���N�g�������݂��Ă���ꍇ�ł���B
            sDestPath = UpboundDataPath.Gen(sDestPath, oEarliestFileInfo.Name)
        End If

        If UpboundDataPath.GetBranchNumber(sDestPath) <= maxBranchNumber Then
            '�t�@�C����V�p�X�Ɉړ�����B
            'NOTE: �t�@�C���͏����\�Ƃ����O��ł���B
            File.Move(oEarliestFileInfo.FullName, sDestPath)
            Log.Info("�t�@�C����[" & sDestPath & "]�Ɉړ����܂����B")
        Else
            '�t�@�C�����폜����B
            'NOTE: �t�@�C���͏����\�Ƃ����O��ł���B
            'NOTE: �����A�m�[�h�̍폜�����̃t�@�C������ƕ��s���čs��꓾��Ƃ��Ă��A
            '�f�B���N�g������݂��Ȃ��Ȃ�΁A�ʐM�n�v���Z�X�͎�M�����t�@�C����
            '���Y�f�B���N�g���̓����G���g���ɖ��Ȃ�Move�\�Ƃ���B
            File.Delete(oEarliestFileInfo.FullName)
            Log.Warn("�t�@�C�����폜���܂����B")
        End If

        Return True
    End Function
#End Region

End Class
