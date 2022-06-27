' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Environment

Public Class TickTimer
    'タイマが動作していない場合の残り時間。
    'NOTE: 利用側は大きな正数であることを前提にしてよい（残り時間との大小比較可）。
    Public Const InfiniteTicks As Long = Long.MaxValue

    Private isActive As Boolean
    Private numerateTicks As Long
    Private timeoutTick As Long

    'UInt32な値（0～0xFFFFFFFF）を返却する。
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

    'NOTE: 当該タイマが動作していない場合は、InfiniteTicksを返却する仕様である。
    Public Function GetTicksToTimeout(ByVal systemTick As Long) As Long
        If Not isActive Then
            Return InfiniteTicks
        Else
            Return GetTickDifference(timeoutTick, systemTick)
        End If
    End Function

End Class
