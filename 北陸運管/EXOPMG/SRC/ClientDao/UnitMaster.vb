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

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' 号機マスタより、値を取得し、DataTableに格納する。
''' </summary>
''' <remarks>クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。</remarks>

Public Class UnitMaster
    '号機マスタ取得結果格納テーブル
    Private dt As DataTable

    '全号機
    Private Const ALL_UNIT As String = "全号機"

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

    ''' <summary> DBより、指定した引数に一致するデータを取得する。</summary>
    ''' <remarks>
    '''  DBより、指定した引数に一致するデータを取得する。
    ''' </remarks>
    ''' <param name="station">駅</param>
    ''' <param name="corner">コーナ</param>
    ''' <param name="model">機種</param>
    ''' <returns>号機マスタ取得結果格納テーブル</returns>
    Public Function SelectTable(ByVal station As String, ByVal corner As String, ByVal model As String) As DataTable
        Dim sSQL As String
        Dim strModel() As String
        Dim i As Integer
        Dim dbCtl As DatabaseTalker

        'パラメーターをチェックする
        'If (System.String.IsNullOrEmpty(station) Or station.Length <> 8) Then
        If (System.String.IsNullOrEmpty(station) Or station.Length <> 6) Then
            Log.Error("引数stationが6桁でありません。") '引数不正
            Throw New DatabaseException()
        ElseIf (System.String.IsNullOrEmpty(corner) Or corner.Length > 4) Then
            Log.Error("引数cornerが4桁を超えています。") '引数不正
            Throw New DatabaseException()
        ElseIf (System.String.IsNullOrEmpty(model) Or model.Length > 14) Then
            Log.Error("引数modelが14桁を超えています。") '引数不正
            Throw New DatabaseException()

        End If

        'テーブル:機器構成マスタ
        '取得項目:表示用号機NO
        '取得項目:表示用号機NAME
        sSQL = " SELECT MCHN.UNIT_NO AS INDICATION_NO," _
             & " CONVERT(CHAR(8),MCHN.UNIT_NO) AS INDICATION_NAME" _
             & " FROM M_MACHINE MCHN" _
             & " WHERE[RAIL_SECTION_CODE]+[STATION_ORDER_CODE]='" & station & "'" _
             & " AND MCHN.CORNER_CODE = '" & corner & "'" _
             & " AND MCHN.SETTING_START_DATE = (" _
                 & " SELECT MAX(SETTING_START_DATE)" _
                 & " FROM M_MACHINE " _
                 & " WHERE SETTING_START_DATE <= '" & ApplyDate & "' )"

        If model <> "" Then
            '文字列を分割
            strModel = model.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    sSQL = sSQL & " AND (MCHN.MODEL_CODE='" & strModel(i) & "' "
                Else
                    sSQL = sSQL & " OR MCHN.MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQL = sSQL & ") ORDER BY INDICATION_NO"
        Else
            sSQL = sSQL & " AND MODEL_CODE<>'X' ORDER BY INDICATION_NO"
        End If

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

    ''' <summary>DataTableの先頭に、「全号機」を追加する。</summary>
    ''' <remarks>
    '''  DataTableの先頭に、「全号機」を追加する。
    ''' </remarks>
    ''' <returns>マスタデータマスタ取得結果格納テーブル</returns>
    Public Function SetAll() As DataTable

        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'DataTableのMODELに、「TERMINAL_ALL」追加する。
        drw.Item(0) = DBNull.Value

        'DataTableのMODEL_NAMEに、「全号機」を追加する。
        drw.Item(1) = ALL_UNIT

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    ''' <summary>DataTableの先頭に、空白行を追加する。</summary>
    ''' <returns>号機マスタ情報</returns>
    Public Function SetSpace() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        Try

            'DataTableの先頭に、空白行を追加する。
            For i As Integer = 0 To dt.Columns.Count - 1
                drw.Item(i) = DBNull.Value
            Next

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
                dt.Columns.Add("INDICATION_NO")
                dt.Columns.Add("INDICATION_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
