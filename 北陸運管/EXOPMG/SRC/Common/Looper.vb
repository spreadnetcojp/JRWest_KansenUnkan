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

Imports System.Net.Sockets
Imports System.Threading

''' <summary>
''' �e�X���b�h����̗v���⑼���u����̓d����҂X���b�h�̊�{�N���X�B
''' </summary>
Public Class Looper

#Region "�萔��ϐ�"
    '�e�X���b�h���b�Z�[�W��M�i�y�щ����ԐM�j�p�\�P�b�g
    Protected oParentMessageSock As Socket

    '���쒆�^�C�}�Ǘ����X�g
    Private oTimerList As LinkedList(Of TickTimer)

    '��M�Ď��\�P�b�g�Ǘ����X�g
    Private oSockList As ArrayList

    '�X���b�h
    Private oThread As Thread
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal sThreadName As String, ByVal oParentMessageSock As Socket)
        Me.oParentMessageSock = oParentMessageSock
        Me.oTimerList = New LinkedList(Of TickTimer)
        Me.oSockList = New ArrayList()
        Me.oThread = New Thread(AddressOf Me.TaskLooper)
        Me.oThread.Name = sThreadName

        'NOTE: �����́ALooper�̊e�C���X�^���X���X���b�h�Ƃ���X���b�h���f����
        '�̗p���Ă���B�����A�v���Z�X���f���Ƃ���ꍇ�́A�����̕ύX���K�v�B
        '�܂��A��������oParentMessageSock���擾���邱�Ƃ͂ł��Ȃ����߁A�����Ŏ���
        '�\�P�b�g���쐬���āA���[�J���z�X�g�̏���́i�����Œʒm���ꂽ�H�j
        '�|�[�g��Connect���邱�ƂɂȂ�͂��ł���B�����āA�e�v���Z�X�݂̂�
        '���̃v���Z�X�����i1��Looper����邲�ƂɕK��Accept�̊����܂ő҂j
        '�悤�Ȑ݌v�ɂ��Ȃ��i���[�U������exe���N�����邱�Ƃ��\�ɂ���j�̂�
        '����΁A�e�v���Z�X���z���́i���[�J���ڑ������j�eLooper����ʂł���
        '�悤�ɂ��邽�߂ɁALooper�́AConnect������A�v���Z�X�������L�q����
        '�J�n�ʒm��e�v���Z�X�ɑ��M����K�v������Ǝv����B

        Me.RegisterSocket(Me.oParentMessageSock)
    End Sub
#End Region

#Region "�e�X���b�h�p���\�b�h"
    '�������o���A�ɂȂ�܂��B
    Public Overridable Sub Start()
        oThread.Start()
    End Sub

    Public Overridable Sub Join()
        oThread.Join()
    End Sub

    Public Overridable Function Join(ByVal millisecondsTimeout As Integer) As Boolean
        Return oThread.Join(millisecondsTimeout)
    End Function

    Public Overridable Sub Abort()
        oThread.Abort()
    End Sub

    Public ReadOnly Property ThreadState() As ThreadState
        Get
            Return oThread.ThreadState
        End Get
    End Property
#End Region

#Region "�T�u�N���X�����p���\�b�h"
    Protected Sub RegisterTimer(ByVal oTimer As TickTimer, ByVal systemTick As Long)
        oTimer.Start(systemTick)
        If Not oTimerList.Contains(oTimer) Then
            oTimerList.AddLast(oTimer)
        End If
    End Sub

    Protected Sub UnregisterTimer(ByVal oTimer As TickTimer)
        oTimer.Terminate()
        oTimerList.Remove(oTimer)
    End Sub

    Protected Sub RegisterSocket(ByVal oSock As Socket)
        If Not oSockList.Contains(oSock) Then
            oSockList.Add(oSock)
        End If
    End Sub

    Protected Sub UnregisterSocket(ByVal oSock As Socket)
        oSockList.Remove(oSock)
    End Sub
#End Region

#Region "�C�x���g�������\�b�h�i���z�j"
    Protected Overridable Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        Return True
    End Function

    Protected Overridable Function ProcOnSockReadable(ByVal oSock As Socket) As Boolean
        Return True
    End Function

    Protected Overridable Sub ProcOnUnhandledException(ByVal ex As Exception)
        If ex.GetType() IsNot GetType(ThreadAbortException) Then
            'NOTE: ���̃��\�b�h���I�[�o���C�h���Ȃ���΁A�ȉ��̂Ƃ���Aex��
            'Catch���Ȃ��ꍇ�Ɠ����ƂȂ�܂��i�v���Z�X�S�̂��I�����܂��j�B
            '����āA���YLooper�̃X���b�h�݂̂��I�����������ꍇ�́A
            '�T�u�N���X�ŃI�[�o���C�h���邱�Ƃ𐄏����܂��B��������ꍇ��
            '�Ó��Ȏ����́A��O�̔��������Ȃ��K�v�ŏ����̂��Ƃ��s���A���̂܂�
            '���\�b�h���I������Ȃǂł��B���̃��\�b�h����߂�΁A���Y
            'Looper�̃X���b�h�݂̂�ThreadState.Stopped�ɑJ�ڂ���͂��ł��B
            '�e�X���b�h�͔C�ӎ����ŊeLooper��ThreadState�v���p�e�B���Ď�����
            '���ɂ��A���̂��Ƃ����m�\�ł��B
            '�Ȃ��A�eLooper�ɂ����鏈���p���ΏۊO�̗�O���������łȂ��A
            '�eLooper�̃t���[�Y�����Ď��������̂ł���΁A�eLooper��
            '������Z�������ŏ���̃v���p�e�B���X�V����悤�ɂ��A�e�X���b�h
            '�́AThreadState�v���p�e�B�ł͂Ȃ��A������Ď�����̂��Ó��ł��B
            Throw ex
        End If
    End Sub
#End Region

#Region "���j����"
    Private Function FindTimeoutTimer(ByVal systemTick As Long) As TickTimer
        'NOTE: minTicks�̏����l�͗��_���0�ɂ���ׂ������A
        'Select�����^�C���A�E�g����^�C�~���O�Ɍ덷������ꍇ��
        '���\���l�����A1ms�����Ƀ^�C���A�E�g����ׂ��^�C�}��
        '�����Ă��^�C���A�E�g�Ɣ��肷��B
        Dim minTicks As Long = 1
        Dim oFoundTimer As TickTimer = Nothing
        For Each oTimer As TickTimer In oTimerList
            Dim ticks As Long = oTimer.GetTicksToTimeout(systemTick)
            If ticks < minTicks Then
                minTicks = ticks
                oFoundTimer = oTimer
            End If
        Next oTimer
        Return oFoundTimer
    End Function

    'NOTE: ���삵�Ă���^�C�}�������ꍇ�́AInfiniteTicks��ԋp����B
    Private Function GetTicksToNextTimeout(ByVal systemTick As Long) As Long
        Dim minTicks As Long = TickTimer.InfiniteTicks
        For Each oTimer As TickTimer In oTimerList
            Dim ticks As Long = oTimer.GetTicksToTimeout(systemTick)
            If ticks < minTicks Then
                minTicks = ticks
            End If
        Next oTimer
        Return minTicks
    End Function

    Private Sub TaskLooper()
        Log.Info("The thread started.")
        Try
            Do
                Dim systemTick As Long = TickTimer.GetSystemTick()

                '�^�C���A�E�g���Ă���^�C�}������΁A
                '�Y�����郁�\�b�h�����s���āA�C�x���g�̌����ɖ߂�B
                Dim oTimer As TickTimer = FindTimeoutTimer(systemTick)
                If oTimer IsNot Nothing Then
                    UnregisterTimer(oTimer)
                    '�Y�����郁�\�b�h�����s����B
                    Dim toBeContinued As Boolean = ProcOnTimeout(oTimer)
                    '���\�b�h���X���b�h���I�����ׂ��Ɣ��f�����ꍇ�́A�X���b�h���I������B
                    If Not toBeContinued Then
                        Return
                    End If
                    '�C�x���g�̌����ɖ߂�B
                    Continue Do
                End If

                '���̃^�C���A�E�g�܂ł̎��Ԃ��擾����B
                '�^�C���A�E�g���Ă���^�C�}���Ȃ��P�[�X�ł��邽�߁A
                '�����œ����鎞�Ԃ͕K��1�ȏ�ł���B
                Dim ticks As Long = GetTicksToNextTimeout(systemTick)
                Debug.Assert(ticks > 0)

                If oSockList.Count <> 0 Then
                    '���Ԃ̒P�ʂ�ϊ�����B
                    'NOTE: �{���́Aticks��TickTimer.InfiniteTicks�̃P�[�X�ł́A
                    'Socket.Select�Ɂu-1�v��n���悤�ɂ��āA�u�������ҋ@�v��
                    '�������B�������A.NET Framework 3.5��Socket.Select�ɂ�
                    '�o�O������A�u-1�v���w�肵���ꍇ�ɑ������A����悤�ł���
                    '���߁A�ł��邾���������ԁiInteger.MaxValue�j���w�肵��
                    '�����t���̑ҋ@�ɂ��Ă����B
                    Dim microSeconds As Integer = Integer.MaxValue
                    If ticks <= Integer.MaxValue \ 1000 Then
                        microSeconds = CInt(ticks * 1000)
                    End If

                    '�\�P�b�g�ǂݏo���Ď����Ď����ʎ擾�p�̃��X�g���쐬����B
                    'OPT: �����蒼�����A�C���X�^���X���t�B�[���h�ɕێ����Ă����A
                    '�����Clear()���Ă���v�f��ǉ�������������I�Ǝv����B
                    Dim oCheckReadList As ArrayList = DirectCast(oSockList.Clone(), ArrayList)

                    '�\�P�b�g���ǂݏo���\�ɂȂ邩���̃^�C���A�E�g����������܂őҋ@����B
                    Socket.Select(oCheckReadList, Nothing, Nothing, microSeconds)

                    '�ǂݏo���\�ɂȂ����\�P�b�g������΁A
                    '�Y�����郁�\�b�h�����s���āA�C�x���g�̌����ɖ߂�B
                    If oCheckReadList.Count > 0 Then
                        Dim oSock As Socket = DirectCast(oCheckReadList(0), Socket)
                        '�Y�����郁�\�b�h�����s����B
                        Dim toBeContinued As Boolean = ProcOnSockReadable(oSock)
                        '���\�b�h���X���b�h���I�����ׂ��Ɣ��f�����ꍇ�́A�X���b�h���I������B
                        If Not toBeContinued Then
                            Return
                        End If
                        '�C�x���g�̌����ɖ߂�B
                        Continue Do
                    End If
                Else
                    If ticks = TickTimer.InfiniteTicks Then
                        Thread.Sleep(Timeout.Infinite)
                    Else
                        Thread.Sleep(CInt(ticks))
                    End If
                End If
            Loop
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            ProcOnUnhandledException(ex)
        End Try
    End Sub
#End Region

End Class
