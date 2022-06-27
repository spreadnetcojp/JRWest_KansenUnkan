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

Public Class EkServiceDate
    Private Const sDefaultStringFormat As String = "yyyyMMdd"

    Public Shared Function Gen(ByVal realDateTime As DateTime) As DateTime
        Dim serviceDate As DateTime = realDateTime.Date
        If realDateTime.Hour >= 0 AndAlso realDateTime.Hour < 3 Then
            Return serviceDate.AddDays(-1)
        Else
            Return serviceDate
        End If
    End Function

    Public Shared Function Gen() As DateTime
        Return Gen(DateTime.Now)
    End Function

    Public Shared Function GenString(ByVal realDateTime As DateTime, ByVal format As String) As String
        Dim serviceDate As DateTime = Gen(realDateTime)
        Return serviceDate.ToString(format)
    End Function

    Public Shared Function GenString(ByVal format As String) As String
        Dim serviceDate As DateTime = Gen()
        Return serviceDate.ToString(format)
    End Function

    Public Shared Function GenString(ByVal realDateTime As DateTime) As String
        Dim serviceDate As DateTime = Gen(realDateTime)
        Return serviceDate.ToString(sDefaultStringFormat)
    End Function

    Public Shared Function GenString() As String
        Dim serviceDate As DateTime = Gen()
        Return serviceDate.ToString(sDefaultStringFormat)
    End Function
End Class
