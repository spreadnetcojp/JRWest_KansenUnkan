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
Option Explicit On
Option Strict On


Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Common

''' <summary>
''' クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。
''' </summary>
''' <remarks>機器構成マスタより、コーナー情報を取得し、DataTableに格納する。
''' </remarks>
Public Class CornerMaster


    'set 全コーナー
    Private Const AllConnor As String = "全コーナー"

    '区マスタ取得結果格納テーブル
    Private dt As DataTable

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

    ''' <summary> DBより、データを取得する。</summary>
    ''' <param name="station">駅コード（駅区コード＋駅順コード）</param>
    ''' <param name="sModel">機種コード</param>
    ''' <returns>グループマスタ取得結果格納テーブル</returns>
    Public Function SelectTable(ByVal station As String, ByVal sModel As String) As DataTable
        Dim sSQL As String

        Dim dbCtl As New DatabaseTalker

        Dim strModel() As String
        Dim i As Integer

        'パラメーターをチェックする
        If (System.String.IsNullOrEmpty(station)) Then

            'ログ出力
            Log.Error("引数stationが空です。") '引数不正

            '例外を呼出元に戻す
            Throw New DatabaseException()

        ElseIf (station.Length <> 6) Then

            'ログ出力
            Log.Error("引数stationが6桁でありません。") '引数不正

            '例外を呼出元に戻す
            Throw New DatabaseException()

        End If

        'テーブル:機器構成マスタ
        '取得項目:機器構成マスタ．コーナー名称
        '取得項目:機器構成マスタ．コーナーコード

        sSQL = " SELECT DISTINCT CAST(CORNER_CODE AS varchar) AS CORNER_CODE,CORNER_NAME" _
             & " FROM M_MACHINE" _
             & " WHERE SETTING_START_DATE=(SELECT MAX(SETTING_START_DATE)" _
             & " FROM M_MACHINE WHERE SETTING_START_DATE <= '" & ApplyDate & "')" _
             & " AND [RAIL_SECTION_CODE]+[STATION_ORDER_CODE]='" & station & "'"
        If sModel <> "" Then
            '文字列を分割
            strModel = sModel.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    sSQL = sSQL & " AND (MODEL_CODE='" & strModel(i) & "' "
                Else
                    sSQL = sSQL & " OR MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQL = sSQL & ") ORDER BY CORNER_CODE"
        Else
            sSQL = sSQL & " AND MODEL_CODE<>'X' ORDER BY CORNER_CODE"
        End If

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
    ''' <returns>コーナーマスタ取得結果格納テーブル</returns>
    Public Function SetSpace() As DataTable

        Dim drw As DataRow

        Dim i As Integer

        DtNothingToOneColumn()
        drw = dt.NewRow()

        For i = 0 To dt.Columns.Count - 1
            drw.Item(i) = ""
        Next

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    ''' <summary>DataTableの先頭に、「全コーナー」を追加する。</summary>
    ''' <returns>コーナーマスタ情報</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
        drw.Item(1) = AllConnor
        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("CORNER_CODE")
                dt.Columns.Add("EXIT_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()

        MyBase.Finalize()

    End Sub
End Class