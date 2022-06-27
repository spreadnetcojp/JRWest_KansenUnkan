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
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions

Imports JR.ExOpmg.Common

Public Class ScheduledEventConfig
    Public StartMinutesInDay As Integer
    Public EndMinutesInDay As Integer
    Public Cycle As Integer
    Public MessageKind As Integer
    Public MessageBody As String
    Public TargetApps As String()

    Public Function Normalize(ByVal time As DateTime) As DateTime
        Dim minutes As Integer = (time.Hour * 60 + time.Minute) - StartMinutesInDay
        If minutes < 0 Then
            minutes += 24 * 60
            minutes = (minutes \ Cycle) * Cycle
            minutes -= 24 * 60
        Else
            minutes = (minutes \ Cycle) * Cycle
        End If
        Dim startTime As New DateTime(time.Year, time.Month, time.Day)
        Return startTime.AddMinutes(StartMinutesInDay + minutes)
    End Function
End Class

Public Class Config
    Inherits ServerAppBaseConfig

    '�C�x���g���̃��X�g
    Public Shared ScheduledEvents As Dictionary(Of String, ScheduledEventConfig)

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const APP_ID As String = "Scheduler"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const START_TIME_KEY As String = "_StartTime"
    Private Const END_TIME_KEY As String = "_EndTime"
    Private Const CYCLE_KEY As String = "_Cycle"
    Private Const MESSAGE_KEY As String = "_Message"
    Private Const TARGETS_KEY As String = "_Targets"

    '������START_TIME_KEY�݂̂Ƀ}�b�`���鐳�K�\��
    Private Shared ReadOnly oValidTimeKeyRegx As New Regex("^[A-Z0-9]+" & START_TIME_KEY & "$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Private Declare Ansi Function GetPrivateProfileStringToBytes Lib "KERNEL32.DLL" _
       Alias "GetPrivateProfileStringA" ( _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpApplicationName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String, _
        <MarshalAs(UnmanagedType.LPArray, ArraySubType:=UnmanagedType.U1)> ByVal lpReturnedString As Byte(), _
        ByVal nSize As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String _
      ) As Integer

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̃X�P�W���[���v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID, True)

        ScheduledEvents = New Dictionary(Of String, ScheduledEventConfig)
        Dim aStrings As String()
        Try
            'SCHEDULE_SECTION���̑S�L�[���k����؂�Ńo�C�g����Ɏ擾����B
            Dim aBytes(16384) As Byte
            Dim validLengthOfBytes As Integer = _
               GetPrivateProfileStringToBytes(SCHEDULE_SECTION, Nothing, "[]_", aBytes, aBytes.Length, sIniFilePath)
            If validLengthOfBytes = 0 Then
                'INI�t�@�C���⏊��Z�N�V�����͑��݂��A�L�[���P���Ȃ��ꍇ�ł���B
                Return
            End If

            '�o�C�g���String�ɕϊ���A�e�L�[��v�f�Ƃ���String�z����쐬����B
            Dim sNullSeparatedKeys As String = Encoding.Default.GetString(aBytes, 0, validLengthOfBytes - 1)
            If sNullSeparatedKeys.Equals("[]") Then
                'INI�t�@�C���܂��͏���Z�N�V���������݂��Ȃ��ꍇ�ł���B
                Throw New OPMGException("The [" & SCHEDULE_SECTION & "] section not found.")
            End If
            Dim aKeys As String() = sNullSeparatedKeys.Split(Chr(0))

            For Each sKey As String In aKeys
                If oValidTimeKeyRegx.IsMatch(sKey) Then
                    'START_TIME_KEY�̃L�[���̈ꕔ����^�C�g���𒊏o����B
                    aStrings = sKey.Split("_"c)
                    Dim sTitle As String = aStrings(0)
                    If ScheduledEvents.ContainsKey(sTitle) Then
                        '����^�C�g����START_TIME_KEY�����݂��Ă���ꍇ�ł���B
                        Throw New OPMGException("The [" & SCHEDULE_SECTION & "] section contains duplicate key [" & sKey & "].")
                    End If

                    '���X�g�ɓo�^����C�x���g�����쐬����B
                    Dim oScheduledEvent As New ScheduledEventConfig()

                    'START_TIME_KEY�ɕR�Â��l�i�N�_(��)�A�N�_(��)�j���擾���A
                    '�C�x���g���ɃZ�b�g����B
                    ReadFileElem(SCHEDULE_SECTION, sKey)
                    aStrings = LastReadValue.Split(":"c)
                    If aStrings.Length <> 2 Then
                        Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
                    End If
                    Dim startHour As Integer = Integer.Parse(aStrings(0))
                    If startHour < 0 OrElse startHour > 23 Then
                        Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
                    End If
                    Dim startMinute As Integer = Integer.Parse(aStrings(1))
                    If startMinute < 0 OrElse startMinute > 59 Then
                        Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
                    End If
                    oScheduledEvent.StartMinutesInDay = startHour * 60 + startMinute

                    'END_TIME_KEY�ɕR�Â��l�i�I��(��)�A�I��(��)�j���擾���A
                    '�C�x���g���ɃZ�b�g����B
                    ReadFileElem(SCHEDULE_SECTION, sTitle & END_TIME_KEY)
                    aStrings = LastReadValue.Split(":"c)
                    If aStrings.Length <> 2 Then
                        Throw New OPMGException("The value must be hour:minute style. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
                    End If
                    Dim endHour As Integer = Integer.Parse(aStrings(0))
                    If endHour < 0 OrElse endHour > 23 Then
                        Throw New OPMGException("The hour must be within the range 0 and 23. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
                    End If
                     Dim endMinute As Integer = Integer.Parse(aStrings(1))
                    If endMinute < 0 OrElse endMinute > 59 Then
                        Throw New OPMGException("The minute must be within the range 0 and 59. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
                    End If
                    oScheduledEvent.EndMinutesInDay = endHour * 60 + endMinute

                    'StartMinutesInDay <= EndMinutesInDay�ɂȂ�悤�A
                    '�K�v�ɉ�����EndMinutesInDay�ɂ͕␳�������Ă����B
                    'NOTE: StartMinutesInDay == EndMinutesInDay�͐����Ȑݒ�
                    '�ł���A�L�����ԑт����̂P���Ԃ����ł��邱�Ƃ��Ӗ�����B
                    If oScheduledEvent.StartMinutesInDay > oScheduledEvent.EndMinutesInDay Then
                        oScheduledEvent.EndMinutesInDay += 24 * 60
                    End If

                    'CYCLE_KEY�ɕR�Â��l�i����(��)�j���擾���A
                    '�C�x���g���ɃZ�b�g����B
                    ReadFileElem(SCHEDULE_SECTION, sTitle & CYCLE_KEY)
                    oScheduledEvent.Cycle = Integer.Parse(LastReadValue)
                    If oScheduledEvent.Cycle < 1 OrElse oScheduledEvent.Cycle > 24 * 60 Then
                        Throw New OPMGException("The value must be within the range 1 and 1440. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
                    End If

                    'MESSAGE_KEY�ɕR�Â��l�i���b�Z�[�W��ʁA���b�Z�[�W�{���j���擾���A
                    '�C�x���g���ɃZ�b�g����B
                    ReadFileElem(SCHEDULE_SECTION, sTitle & MESSAGE_KEY)
                    Dim sepPos As Integer = LastReadValue.IndexOf(","c)
                    If sepPos = -1 Then
                        oScheduledEvent.MessageKind = Integer.Parse(LastReadValue)
                        oScheduledEvent.MessageBody = ""
                    Else
                        oScheduledEvent.MessageKind = Integer.Parse(LastReadValue.Substring(0, sepPos))
                        oScheduledEvent.MessageBody = LastReadValue.Substring(sepPos + 1)
                    End If

                    'TARGETS_KEY�ɕR�Â��l�i���M��v���Z�X�̎��ʎq�j���擾���A
                    '�C�x���g���ɃZ�b�g����B
                    ReadFileElem(SCHEDULE_SECTION, sTitle & TARGETS_KEY)
                    aStrings = LastReadValue.Split(","c)
                    oScheduledEvent.TargetApps = New String(aStrings.Length - 1) {}
                    For i As Integer = 0 To aStrings.Length - 1
                        oScheduledEvent.TargetApps(i) = aStrings(i)
                    Next i

                    '�C�x���g�������X�g�ɓo�^����B
                    ScheduledEvents.Add(sTitle, oScheduledEvent)
                End If
            Next sKey

            'SCHEDULE_SECTION���ɗ]���ȁi�L�q�~�X�Ƃ݂Ȃ��ׂ��j�L�[�����݂��Ă��Ȃ����`�F�b�N����B
            If aKeys.Length <> ScheduledEvents.Count * 5 Then
                Throw New OPMGException("The [" & SCHEDULE_SECTION & "] section contains some invalid keys.")
            End If
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    Public Shared Sub Dispose()
        ServerAppBaseDispose()
    End Sub

End Class
