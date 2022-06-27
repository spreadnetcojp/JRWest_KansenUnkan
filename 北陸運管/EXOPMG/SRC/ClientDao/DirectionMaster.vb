' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Common

''' <summary>
''' クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。
''' 通路方向の情報をDataTableに格納する。
''' </summary>
Public Class DirectionMaster

    ''' <summary>
    ''' 通路方向情報(内部固定)取得結果格納テーブル
    ''' </summary>
    Private dt As DataTable

    ''' <summary>
    ''' 通路方向:改札
    ''' </summary>
    Private ReadOnly LcstKaisatu As String = "改札"

    ''' <summary>
    ''' 通路方向:集札
    ''' </summary>
    Private ReadOnly LcstSyusatu As String = "集札"

    ''' <summary>設定データを返却する</summary>
    ''' <returns>通路方向情報</returns>
    Public Function SelectTable() As DataTable

        Dim drw As DataRow
        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker

        dbCtl = New DatabaseTalker

        sSQL = " SELECT FLG" _
             & " , '" & LcstKaisatu & "' name " _
             & "  FROM M_PASSAGE" _
             & " WHERE NAME like '%" & LcstKaisatu & "%'" _
             & " UNION " _
             & " SELECT FLG" _
             & " , '" & LcstSyusatu & "' name " _
             & "  FROM M_PASSAGE" _
             & " WHERE NAME like '%" & LcstSyusatu & "%'" _
             & " ORDER BY FLG"

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSQL)

            drw = dt.NewRow()
            drw.Item(0) = ClientDaoConstants.TERMINAL_ALL : drw.Item(1) = "両方向"
            dt.Rows.InsertAt(drw, 0)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dt
    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("CODE")
                dt.Columns.Add("NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
