' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/04/15  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text

Public Class MyUtility

    'NOTE: 未使用
    Public Shared Function GetStringFromByteFailSafe(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer, ByVal oEncoding As Encoding) As String
        Try
            Return oEncoding.GetString(src, pos, len)
        Catch ex As Exception
            Return "[" & BitConverter.ToString(src, pos, len) & "]"
        End Try
    End Function

    Public Shared Function GetTextWidth(ByVal s As String, ByVal fnt As Font) As Integer
        Dim canvas As New Bitmap(10, 10)
        Dim g As Graphics = Graphics.FromImage(canvas)
        Dim sf As New StringFormat()
        g.DrawString(s, fnt, Brushes.Black, 0, 0, sf)
        Dim stringSize As SizeF = g.MeasureString(s, fnt, 1000, sf)
        sf.Dispose()
        g.Dispose()
        Return CInt(Math.Ceiling(stringSize.Width))
    End Function

    'NOTE: 未使用
    Public Shared Function GetFocusedControl(ByVal parentControl As Control) As Control
        Dim c As Control
        For Each c In parentControl.Controls
            If c.Focused Then
                Return c
            End If
            If c.ContainsFocus Then
                Dim fc As Control = GetFocusedControl(c)
                If Not (fc Is Nothing) Then
                    Return fc
                End If
            End If
        Next
        Return Nothing
    End Function

End Class
