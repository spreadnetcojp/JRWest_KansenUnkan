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
Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Enum ScenarioElementVerb As Integer
    Connect
    Disconnect
    ActiveOne
    ActiveUll
End Enum

Public Structure ScenarioElement
    Public Timing As String
    Public Verb As ScenarioElementVerb
    Public Obj As String()
    Public Sub New(ByVal t As String, ByVal v As ScenarioElementVerb, ByVal o As String())
        Me.Timing = t
        Me.Verb = v
        Me.Obj = o
    End Sub
End Structure

Public Class ScenarioReader

    'NOTE: sFilePathにファイルがない場合などには、IOExceptionをスローします。
    'NOTE: 書式に異常がある場合などには、IOException以外のExceptionをスローします。
    Public Shared Function Read(ByVal sFilePath As String) As List(Of ScenarioElement)

        Dim oResult As New List(Of ScenarioElement)
        Dim sLine As String
        Dim aColumns As String()
        Using oReader As StreamReader _
           = New StreamReader(sFilePath, Encoding.Default)

            Dim lineNumber As Integer = 1
            sLine = oReader.ReadLine()
            While sLine IsNot Nothing
                If Not sLine.Length = 0 AndAlso _
                   Not sLine.StartsWith("'", StringComparison.Ordinal) Then

                    '読み込んだ行を列に分割する。
                    aColumns = sLine.Split(","c)

                    If aColumns.Length < 2 Then
                        Log.Error("The line #" & lineNumber.ToString() & " of the file contains too few columns.")
                        Throw New FormatException()
                    End If

                    Dim sTiming As String = aColumns(0)
                    If sTiming.StartsWith("+") Then
                        Dim intVar As Integer
                        If Integer.TryParse(sTiming, intVar) = False Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal timing.")
                            Throw New FormatException()
                        End If
                    Else
                        Dim timing As DateTime
                        If DateTime.TryParseExact(sTiming, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, timing) = False Then
                            Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal timing.")
                            Throw New FormatException()
                        End If
                    End If

                    Dim verb As ScenarioElementVerb
                    Dim sVerb As String = aColumns(1)
                    Try
                        verb = DirectCast([Enum].Parse(GetType(ScenarioElementVerb), sVerb), ScenarioElementVerb)
                    Catch ex As Exception
                        Log.Error("The line #" & lineNumber.ToString() & " of the file contains illegal verb.")
                        Throw New FormatException()
                    End Try

                    Select Case verb
                        Case ScenarioElementVerb.Connect, ScenarioElementVerb.Disconnect
                            If aColumns.Length > 2 Then
                                Log.Error("The line #" & lineNumber.ToString() & " of the file contains too many columns.")
                                Throw New FormatException()
                            End If

                            oResult.Add(New ScenarioElement(sTiming, verb, Nothing))

                        Case ScenarioElementVerb.ActiveOne
                            If aColumns.Length < 3 Then
                                Log.Error("The line #" & lineNumber.ToString() & " of the file contains too few columns.")
                                Throw New FormatException()
                            End If

                            If aColumns.Length > 7 Then
                                Log.Error("The line #" & lineNumber.ToString() & " of the file contains too many columns.")
                                Throw New FormatException()
                            End If

                            Dim aObjs As String() = {aColumns(2), "60000", "60000", "0", "0"}
                            For i As Integer = 3 To aColumns.Length - 1
                                If Not aColumns(i).Equals("") Then
                                    Dim intVar As Integer
                                    If Integer.TryParse(aColumns(i), intVar) = False OrElse intVar < 0 Then
                                        Log.Error("The column #" & (i + 1).ToString &  " of the line #" & lineNumber.ToString() & " of the file is illegal.")
                                        Throw New FormatException()
                                    End If
                                    aObjs(i - 2) = aColumns(i)
                                End If
                            Next
                            oResult.Add(New ScenarioElement(sTiming, verb, aObjs))

                        Case ScenarioElementVerb.ActiveUll
                            If aColumns.Length < 4 Then
                                Log.Error("The line #" & lineNumber.ToString() & " of the file contains too few columns.")
                                Throw New FormatException()
                            End If

                            If aColumns.Length > 10 Then
                                Log.Error("The line #" & lineNumber.ToString() & " of the file contains too many columns.")
                                Throw New FormatException()
                            End If

                            Dim byteVar As Byte
                            If Byte.TryParse(aColumns(2), NumberStyles.HexNumber, Nothing, byteVar) = False Then
                                Log.Error("The column #3 of the line #" & lineNumber.ToString() & " of the file is illegal.")
                                Throw New FormatException()
                            End If

                            Dim aObjs As String() = {aColumns(2), aColumns(3), aColumns(4), "0", "60000", "60000", "0", "0"}
                            For i As Integer = 5 To aColumns.Length - 1
                                If Not aColumns(i).Equals("") Then
                                    Dim intVar As Integer
                                    If Integer.TryParse(aColumns(i), intVar) = False OrElse intVar < 0 Then
                                        Log.Error("The column #" & (i + 1).ToString &  " of the line #" & lineNumber.ToString() & " of the file is illegal.")
                                        Throw New FormatException()
                                    End If
                                    aObjs(i - 2) = aColumns(i)
                                End If
                            Next
                            oResult.Add(New ScenarioElement(sTiming, verb, aObjs))
                    End Select
                End If

                sLine = oReader.ReadLine()
                lineNumber += 1
            End While
        End Using

        Return oResult
    End Function

End Class
