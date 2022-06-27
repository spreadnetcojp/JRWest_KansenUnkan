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

Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Common

''' <summary>
''' 登録済のマスタ見出し／マスタ適用リスト見出しより、値を取得し、DataTableに格納する。
''' </summary>
''' <remarks>クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。</remarks>
Public Class MasterVersionMaster

    'マスタ／マスタ適用リストのバージョン取得結果格納テーブル
    Private dt As DataTable

    ''' <summary>
    ''' DBより、データを取得する。
    ''' </summary>
    ''' <param name="model">機種</param>
    ''' <param name="master">マスタ</param>
    ''' <param name="kbn">データ種別</param>
    ''' <param name="pattern">パターン</param>
    ''' <returns>マスタ／マスタ適用リストのバージョン取得結果格納テーブル</returns>
    ''' <remarks>DBより、指定した引数に一致するデータを取得する。</remarks>
    Public Function SelectTable(ByVal model As String, ByVal master As String, ByVal kbn As String, ByVal pattern As String) As DataTable
        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker

        'テーブル:マスタ／マスタ適用リストの見出し
        '取得項目:ファイル区分
        '取得項目:バージョン
        sSQL = "SELECT" _
             & "    DISTINCT LST.KBN," _
             & "    CASE" _
             & "        WHEN LST.KBN = 'DAT' THEN LST.DATA_VERSION" _
             & "        ELSE LST.LIST_VERSION" _
             & "    END AS VER" _
             & " FROM" _
             & "    (" _
             & "        SELECT" _
             & "            MODEL_CODE,DATA_KIND,DATA_SUB_KIND,DATA_VERSION" _
             & "        FROM" _
             & "            S_MST_DATA_HEADLINE" _
             & "    ) AS MST," _
             & "    (" _
             & "        SELECT MODEL_CODE,'" & kbn & "' AS KBN,DATA_KIND,DATA_SUB_KIND," _
             & "            DATA_VERSION,LIST_VERSION" _
             & "        FROM" _
             & "            S_MST_LIST_HEADLINE" _
             & "    ) AS LST" _
             & " WHERE" _
             & "     MST.MODEL_CODE = LST.MODEL_CODE AND MST.DATA_KIND = LST.DATA_KIND" _
             & " AND MST.DATA_SUB_KIND = LST.DATA_SUB_KIND AND MST.DATA_VERSION = LST.DATA_VERSION" _
             & " AND LST.MODEL_CODE = '" & model & "' AND LST.DATA_KIND = '" & master & "'" _
             & " AND LST.DATA_SUB_KIND = '" & pattern & "'"

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
    ''' <returns>マスタバージョンマスタ</returns>
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

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("KBN")
                dt.Columns.Add("VERSION")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
