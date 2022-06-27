' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/11/21  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Public Class MyUtility

    Public Shared Function GetNextSeqNumber(ByVal n As UInteger) As UInteger
        If n = UInteger.MaxValue Then
            Return 1UI
        Else
            Return CUInt(n + 1)
        End If
    End Function

    Public Shared Function GetValidByteCount(ByVal field As XlsField, ByVal value As String) As Integer
        Dim byteCount As Integer = field.ElementBits \ 8
        If byteCount * 8 <> field.ElementBits Then Throw New ArgumentException("ElemrntBits of field must be a multiple of 8.")
        Dim workBytes(byteCount - 1) As Byte
        field.CopyValueToBytes(value, workBytes)
        For i As Integer = byteCount - 1 To 0 Step -1
            If workBytes(i) <> 0 Then Return i + 1
        Next i
        Return 0
    End Function

    Public Shared Function GetRightPaddedValue(ByVal field As XlsField, ByVal srcValue As String, ByVal padCode As Byte) As String
        Dim byteCount As Integer = field.ElementBits \ 8
        If byteCount * 8 <> field.ElementBits Then Throw New ArgumentException("ElemrntBits of field must be a multiple of 8.")
        Dim workBytes(byteCount - 1) As Byte
        field.CopyValueToBytes(srcValue, workBytes)
        For i As Integer = byteCount - 1 To 0 Step -1
            If workBytes(i) <> 0 Then Exit For
            workBytes(i) = padCode
        Next i
        Return field.CreateValueFromBytes(workBytes)
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
