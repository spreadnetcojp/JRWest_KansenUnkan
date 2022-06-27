' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/04/01  (NES)河脇  北陸対応：グループ＋支社コード毎駅名表示
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Common

''' <summary>
''' 機器構成マスタより、駅情報を取得し、DataTableに格納する。
''' </summary>
''' <remarks>クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。</remarks>
Public Class StationMaster

    '駅マスタ取得結果格納テーブル
    Private dt As DataTable

    Private Const ALL_STATION As String = "全駅"

    '適用開始日
    Private sApplyDate As String = ""
    '適用開始日
    Public Property ApplyDate() As String
        Get
            Return sApplyDate
        End Get
        Set(ByVal Value As String)
            sApplyDate = Value
        End Set
    End Property

    ''' <summary>プロパティApplyDateに、本日の日付(YYYYMMDD)をセットする</summary>
    Public Sub New()
        ApplyDate = Now.ToString("yyyyMMdd")
    End Sub

    ''' <summary>DBより、指定した引数に一致するデータを取得する。</summary>
    ''' <param name="sFlg">支社追加有無　False：無、True：有</param>
    ''' <param name="sModel">対象機種</param>
    ''' <param name="GroupSortFlg">駅コードのグループNo,支社コード付与有無　False：無、True：有</param>
    ''' <returns>駅マスタ情報</returns>
    Public Function SelectTable(ByVal sFlg As Boolean, ByVal sModel As String, Optional ByVal GroupSortFlg As Boolean = False) As DataTable
        ' --- Ver0.1 グループ＋支社コード毎駅名表示 MOD
        'Public Function SelectTable(ByVal sFlg As Boolean, ByVal sModel As String) As DataTable
        Dim sSQL As String = ""
        Dim sSQLsub As String = ""
        Dim dbCtl As DatabaseTalker
        Dim strModel() As String
        Dim i As Integer

        dbCtl = New DatabaseTalker

        If sModel <> "" Then
            '文字列を分割
            strModel = sModel.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    ' --- Ver0.1 グループ＋支社コード毎駅名表示 MOD START
                    'sSQLsub = " AND (MODEL_CODE='" & strModel(i) & "' "
                    sSQLsub = " (MODEL_CODE='" & strModel(i) & "' "
                    ' --- Ver0.1 グループ＋支社コード毎駅名表示 MOD END
                Else
                    sSQLsub = sSQLsub & " OR MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQLsub = sSQLsub & ")"
        End If

        Try
            ' --- Ver0.1 グループ＋支社コード毎駅名表示 MOD START
            'If sFlg Then
            '    sSQL = " SELECT '000'+OFFICE.BRANCH_OFFICE_CODE AS STATION_CODE," _
            '         & " M_BRANCH_OFFICE.NAME AS STATION_NAME" _
            '         & " FROM (SELECT DISTINCT BRANCH_OFFICE_CODE" _
            '         & " FROM M_MACHINE WHERE SETTING_START_DATE=(SELECT MAX(SETTING_START_DATE)" _
            '         & " FROM M_MACHINE WHERE SETTING_START_DATE <= '" & ApplyDate & "')" _
            '         & sSQLsub & ") AS OFFICE,M_BRANCH_OFFICE" _
            '         & " WHERE OFFICE.BRANCH_OFFICE_CODE=M_BRANCH_OFFICE.CODE" _
            '         & " UNION"
            'End If

            'sSQL = sSQL & " SELECT DISTINCT RAIL_SECTION_CODE+STATION_ORDER_CODE AS STATION_CODE" _
            ' & " ,STATION_NAME" _
            ' & " FROM M_MACHINE WHERE SETTING_START_DATE=(SELECT MAX(SETTING_START_DATE) " _
            ' & " FROM M_MACHINE WHERE SETTING_START_DATE <= '" & ApplyDate & "')" _
            ' & sSQLsub & " ORDER BY STATION_CODE"
            sSQL = sSQL + "SELECT"
            If GroupSortFlg Then
                ' 駅名コードにグループ＋支社コードを付与
                sSQL = sSQL + "   LTRIM(GROUP_NO2+BRANCH_OFFICE_CODE+RAIL_SECTION_CODE+STATION_ORDER_CODE) AS STATION_CODE,"
            Else
                sSQL = sSQL + "   LTRIM(RAIL_SECTION_CODE+STATION_ORDER_CODE) AS STATION_CODE, "
            End If
            sSQL = sSQL + "   STATION_NAME"
            sSQL = sSQL + " FROM("
            ' 支社の追加有無
            If sFlg Then
                sSQL = sSQL + "   SELECT"
                sSQL = sSQL + "     '0' AS GROUP_NO,'000' AS BRANCH_OFFICE_CODE,"
                sSQL = sSQL + "     '000' AS RAIL_SECTION_CODE,"
                sSQL = sSQL + "     V_MACHINE_NOW.BRANCH_OFFICE_CODE AS STATION_ORDER_CODE,"
                sSQL = sSQL + "     M_BRANCH_OFFICE.NAME AS STATION_NAME,"
                sSQL = sSQL + "     CONVERT(varchar,M_BRANCH_OFFICE.GROUP_NO) AS GROUP_NO2"
                sSQL = sSQL + "   FROM"
                sSQL = sSQL + "     V_MACHINE_NOW,M_BRANCH_OFFICE"
                sSQL = sSQL + "   WHERE"
                sSQL = sSQL + "     V_MACHINE_NOW.BRANCH_OFFICE_CODE=M_BRANCH_OFFICE.CODE"
                If sSQLsub <> "" Then
                    sSQL = sSQL + "     AND " + sSQLsub
                End If
                sSQL = sSQL + "   UNION"
            End If
            sSQL = sSQL + "   SELECT"
            sSQL = sSQL + "     CONVERT(varchar,GROUP_NO)AS GROUP_NO,BRANCH_OFFICE_CODE,"
            sSQL = sSQL + "     RAIL_SECTION_CODE,STATION_ORDER_CODE,STATION_NAME,"
            sSQL = sSQL + "     CONVERT(varchar,GROUP_NO)AS GROUP_NO2"
            sSQL = sSQL + "   FROM"
            sSQL = sSQL + "     V_MACHINE_NOW"
            If sSQLsub <> "" Then
                sSQL = sSQL + "   WHERE " + sSQLsub
            End If
            sSQL = sSQL + "   GROUP BY"
            sSQL = sSQL + "     GROUP_NO,BRANCH_OFFICE_CODE,RAIL_SECTION_CODE,"
            sSQL = sSQL + "     STATION_ORDER_CODE,STATION_NAME"
            sSQL = sSQL + " ) AS DAT"
            ' --- Ver0.1 グループ＋支社コード毎駅名表示 MOD END

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
    ''' <returns>駅マスタ情報</returns>
    Public Function SetSpace() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        Try
            For i As Integer = 0 To dt.Columns.Count - 1
                drw.Item(i) = ""
            Next
            dt.Rows.InsertAt(drw, 0)
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            drw = Nothing
        End Try

        Return dt
    End Function

    ''' <summary>DataTableの先頭に、「全駅」を追加する。</summary>
    ''' <returns>駅マスタ情報</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        Try
            drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
            drw.Item(1) = ALL_STATION
            dt.Rows.InsertAt(drw, 0)
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            drw = Nothing
        End Try

        Return dt
    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("NONMONITOR_STATION_CODE")
                dt.Columns.Add("NONMONITOR_STATION_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class