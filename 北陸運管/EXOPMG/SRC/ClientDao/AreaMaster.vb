' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' エリアマスタより、値を取得し、DataTableに格納する。
''' </summary>
''' <remarks>クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。</remarks>
Public Class AreaMaster

    'エリアマスタ取得結果格納テーブル
    Private dt As DataTable

    'set 全パターン
    Private Const AllArea As String = "全適用エリア"

    ''' <summary> DBより、データを取得する。</summary>
    ''' <remarks>
    '''  DBより、指定した引数に一致するデータを取得する。
    ''' </remarks>
    ''' <param name="model">機種</param>
    ''' <returns>エリアマスタ取得結果格納テーブル</returns>
    Public Function SelectTable(ByVal model As String) As DataTable
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker

        sSQL = "SELECT AREA_NO, AREA_NAME FROM M_AREA_DATA" _
             & " WHERE MODEL_CODE='" & model & "'"

        dbCtl = New DatabaseTalker

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dt
    End Function

    ''' <summary>DataTableの先頭に、空白行を追加する。</summary>
    ''' <returns>エリアマスタ取得結果格納テーブル</returns>
    Public Function SetSpace() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'DataTableの先頭に、空白行を追加する。

        For i As Integer = 0 To dt.Columns.Count - 1
            drw.Item(i) = ""
        Next

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    ''' <summary>DataTableの先頭に、「全エリア」を追加する。</summary>
    ''' <returns>エリアマスタ情報</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'English DataRowのAREA_NOに、「TERMINAL_ALL」追加する。
        drw.Item(0) = ClientDaoConstants.TERMINAL_ALL

        'English DataRowのAREA_NAMEに、「TERMINAL_ALL」追加する。
        drw.Item(1) = AllArea

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("AREA_NO")
                dt.Columns.Add("AREA_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
