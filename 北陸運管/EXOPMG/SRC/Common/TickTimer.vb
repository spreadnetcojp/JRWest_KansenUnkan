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

Imports System.Environment

Public Class TickTimer
    '�^�C�}�����삵�Ă��Ȃ��ꍇ�̎c�莞�ԁB
    'NOTE: ���p���͑傫�Ȑ����ł��邱�Ƃ�O��ɂ��Ă悢�i�c�莞�ԂƂ̑召��r�j�B
    Public Const InfiniteTicks As Long = Long.MaxValue

    Private isActive As Boolean
    Private numerateTicks As Long
    Private timeoutTick As Long

    'UInt32�Ȓl�i0�`0xFFFFFFFF�j��ԋp����B
    Public Shared Function GetSystemTick() As Long
        Dim tick As Integer = Environment.TickCount
        If tick >= 0 Then
            return CLng(tick)
        Else
            return CLng(tick) + (CLng(UInt32.MaxValue) + 1)
        End If
    End Function

    Public Shared Function GetTickDifference(ByVal evalTick As Long, ByVal baseTick As Long) As Long
        Dim ticks As Long = evalTick - baseTick
        If ticks >= 0 Then
            If ticks <= Integer.MaxValue Then
                Return ticks
            Else
                Return ticks - (CLng(UInt32.MaxValue) + 1)
            End If
        Else
            If ticks >= Integer.MinValue Then
                Return ticks
            Else
                Return ticks + (CLng(UInt32.MaxValue) + 1)
            End If
        End If
    End Function

    Public Sub New(ByVal numerateTicks As Long)
        If numerateTicks < Integer.MinValue \ 4 OrElse numerateTicks > Integer.MaxValue \ 4
            Throw New ArgumentOutOfRangeException()
        End If

        Me.isActive = False
        Me.numerateTicks = numerateTicks
    End Sub

    Public Sub Renew(ByVal numerateTicks As Long)
        If numerateTicks < Integer.MinValue \ 4 OrElse numerateTicks > Integer.MaxValue \ 4
            Throw New ArgumentOutOfRangeException()
        End If

        Me.isActive = False
        Me.numerateTicks = numerateTicks
    End Sub

    Public Sub Start(ByVal systemTick As Long)
        isActive = True
        timeoutTick = systemTick + numerateTicks
        If timeoutTick > Integer.MaxValue Then
            timeoutTick -= CLng(UInt32.MaxValue) + 1
        End If
    End Sub

    Public Sub Terminate()
        isActive = False
    End Sub

    'NOTE: ���Y�^�C�}�����삵�Ă��Ȃ��ꍇ�́AInfiniteTicks��ԋp����d�l�ł���B
    Public Function GetTicksToTimeout(ByVal systemTick As Long) As Long
        If Not isActive Then
            Return InfiniteTicks
        Else
            Return GetTickDifference(timeoutTick, systemTick)
        End If
    End Function

End Class
