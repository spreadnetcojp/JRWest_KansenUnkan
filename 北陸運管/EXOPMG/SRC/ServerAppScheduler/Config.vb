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

    'イベント情報のリスト
    Public Shared ScheduledEvents As Dictionary(Of String, ScheduledEventConfig)

    'プロセス別キーに対するプレフィックス
    Private Const APP_ID As String = "Scheduler"

    'INIファイル内における各設定項目のキー
    Private Const START_TIME_KEY As String = "_StartTime"
    Private Const END_TIME_KEY As String = "_EndTime"
    Private Const CYCLE_KEY As String = "_Cycle"
    Private Const MESSAGE_KEY As String = "_Message"
    Private Const TARGETS_KEY As String = "_Targets"

    '正しいSTART_TIME_KEYのみにマッチする正規表現
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

    ''' <summary>INIファイルから運管サーバのスケジューラプロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID, True)

        ScheduledEvents = New Dictionary(Of String, ScheduledEventConfig)
        Dim aStrings As String()
        Try
            'SCHEDULE_SECTION内の全キーをヌル区切りでバイト列内に取得する。
            Dim aBytes(16384) As Byte
            Dim validLengthOfBytes As Integer = _
               GetPrivateProfileStringToBytes(SCHEDULE_SECTION, Nothing, "[]_", aBytes, aBytes.Length, sIniFilePath)
            If validLengthOfBytes = 0 Then
                'INIファイルや所定セクションは存在し、キーが１つもない場合である。
                Return
            End If

            'バイト列をStringに変換後、各キーを要素とするString配列を作成する。
            Dim sNullSeparatedKeys As String = Encoding.Default.GetString(aBytes, 0, validLengthOfBytes - 1)
            If sNullSeparatedKeys.Equals("[]") Then
                'INIファイルまたは所定セクションが存在しない場合である。
                Throw New OPMGException("The [" & SCHEDULE_SECTION & "] section not found.")
            End If
            Dim aKeys As String() = sNullSeparatedKeys.Split(Chr(0))

            For Each sKey As String In aKeys
                If oValidTimeKeyRegx.IsMatch(sKey) Then
                    'START_TIME_KEYのキー名の一部からタイトルを抽出する。
                    aStrings = sKey.Split("_"c)
                    Dim sTitle As String = aStrings(0)
                    If ScheduledEvents.ContainsKey(sTitle) Then
                        '同一タイトルのSTART_TIME_KEYが存在している場合である。
                        Throw New OPMGException("The [" & SCHEDULE_SECTION & "] section contains duplicate key [" & sKey & "].")
                    End If

                    'リストに登録するイベント情報を作成する。
                    Dim oScheduledEvent As New ScheduledEventConfig()

                    'START_TIME_KEYに紐づく値（起点(時)、起点(分)）を取得し、
                    'イベント情報にセットする。
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

                    'END_TIME_KEYに紐づく値（終了(時)、終了(分)）を取得し、
                    'イベント情報にセットする。
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

                    'StartMinutesInDay <= EndMinutesInDayになるよう、
                    '必要に応じてEndMinutesInDayには補正をかけておく。
                    'NOTE: StartMinutesInDay == EndMinutesInDayは正当な設定
                    'であり、有効時間帯がその１分間だけであることを意味する。
                    If oScheduledEvent.StartMinutesInDay > oScheduledEvent.EndMinutesInDay Then
                        oScheduledEvent.EndMinutesInDay += 24 * 60
                    End If

                    'CYCLE_KEYに紐づく値（周期(分)）を取得し、
                    'イベント情報にセットする。
                    ReadFileElem(SCHEDULE_SECTION, sTitle & CYCLE_KEY)
                    oScheduledEvent.Cycle = Integer.Parse(LastReadValue)
                    If oScheduledEvent.Cycle < 1 OrElse oScheduledEvent.Cycle > 24 * 60 Then
                        Throw New OPMGException("The value must be within the range 1 and 1440. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")")
                    End If

                    'MESSAGE_KEYに紐づく値（メッセージ種別、メッセージ本文）を取得し、
                    'イベント情報にセットする。
                    ReadFileElem(SCHEDULE_SECTION, sTitle & MESSAGE_KEY)
                    Dim sepPos As Integer = LastReadValue.IndexOf(","c)
                    If sepPos = -1 Then
                        oScheduledEvent.MessageKind = Integer.Parse(LastReadValue)
                        oScheduledEvent.MessageBody = ""
                    Else
                        oScheduledEvent.MessageKind = Integer.Parse(LastReadValue.Substring(0, sepPos))
                        oScheduledEvent.MessageBody = LastReadValue.Substring(sepPos + 1)
                    End If

                    'TARGETS_KEYに紐づく値（送信先プロセスの識別子）を取得し、
                    'イベント情報にセットする。
                    ReadFileElem(SCHEDULE_SECTION, sTitle & TARGETS_KEY)
                    aStrings = LastReadValue.Split(","c)
                    oScheduledEvent.TargetApps = New String(aStrings.Length - 1) {}
                    For i As Integer = 0 To aStrings.Length - 1
                        oScheduledEvent.TargetApps(i) = aStrings(i)
                    Next i

                    'イベント情報をリストに登録する。
                    ScheduledEvents.Add(sTitle, oScheduledEvent)
                End If
            Next sKey

            'SCHEDULE_SECTION内に余分な（記述ミスとみなすべき）キーが存在していないかチェックする。
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
