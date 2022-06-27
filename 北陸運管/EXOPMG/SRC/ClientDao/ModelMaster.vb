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
''' 機種マスタより、値を取得し、DataTableに格納する。
''' </summary>
''' <remarks>クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。</remarks>
Public Class ModelMaster

    '全機種
    Private Const ALL_MODE As String = "全機種"

    '機種マスタ取得結果格納テーブル
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

#Region "DBより、データを取得する。"
    ''' <summary> DBより、データを取得する。</summary>
    ''' <param name="bGetSend">true:PRG送信対象取得、false:マスタ送信対象</param>
    ''' <returns>マスタ取得結果格納テーブル</returns>
    Public Function SelectTable(Optional ByVal bGetSend As Boolean = False) As DataTable

        '本メソッドの実行により、DataTable dtは初期化される。
        dt = New DataTable

        Dim dbCtl As New DatabaseTalker

        Dim sSQL As String = ""

        'テーブル:機種マスタ
        '取得項目:機種マスタ．機種コード
        '取得項目:機種マスタ．機種名
        sSQL = "SELECT MODEL_CODE,MODEL_NAME  FROM M_MODEL"

        If bGetSend Then
            sSQL = sSQL & " WHERE PRG_SND_FLAG = '1'"
        Else
            sSQL = sSQL & " WHERE MST_SND_FLAG = '1'"
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
#End Region

#Region "DBより、データを取得する。"
    ''' <summary> DBより、データを取得する。</summary>
    ''' <param name="sStation">駅コード（駅区コード＋駅順コード）</param>
    ''' <param name="sCorner">コーナーコード</param>
    ''' <param name="bKadoReceive">true:稼動データ受信対象取得</param>
    ''' <param name="bFaultReceive">true:異常データ受信対象取得</param>
    ''' <returns>グループマスタ取得結果格納テーブル</returns>
    Public Function SelectTable(ByVal sStation As String, ByVal sCorner As String, _
                                ByVal bKadoReceive As Boolean, Optional ByVal bFaultReceive As Boolean = False) As DataTable

        'パラメーターをチェックする
        If String.IsNullOrEmpty(sStation) Then
            Log.Error("引数sStationが空です。") '引数不正
            Throw New DatabaseException()
        End If

        If sStation.Length <> 6 Then
            Log.Error("引数sStationが6桁でありません。") '引数不正
            Throw New DatabaseException()
        End If

        If String.IsNullOrEmpty(sCorner) Then
            Log.Error("引数sCornerが空です。") '引数不正
            Throw New DatabaseException()
        End If

        '本メソッドの実行により、DataTable dtは初期化される。
        dt = New DataTable

        Dim dbCtl As DatabaseTalker

        Dim sSQL As String = ""

        dbCtl = New DatabaseTalker

        Try
            'テーブル:機種マスタ
            'テーブル:機器構成マスタ
            '取得項目:機種マスタ．機種コード
            '取得項目:機種マスタ．機種名
            sSQL = "SELECT DISTINCT MOD.MODEL_CODE AS MODEL, MOD.MODEL_NAME" _
                & " FROM M_MACHINE MAC,M_MODEL MOD" _
                & " WHERE MAC.MODEL_CODE = MOD.MODEL_CODE" _
                & " AND MAC.RAIL_SECTION_CODE+MAC.STATION_ORDER_CODE='" & sStation & "'" _
                & " AND MAC.CORNER_CODE = '" & sCorner & "'" _
                & " AND MAC.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                & " FROM M_MACHINE WHERE SETTING_START_DATE <= '" & ApplyDate & "')"

            If bFaultReceive Then
                sSQL = sSQL & " AND MOD.FAULT_RCV_FLAG = '1'"
            Else
                If bKadoReceive Then
                    sSQL = sSQL & " AND MOD.KADO_RCV_FLAG = '1'"
                End If
            End If

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
#End Region

#Region "DataTableの先頭に、空白行を追加する。"
    ''' <summary>DataTableの先頭に、空白行を追加する。</summary>
    ''' <returns>機種マスタ情報</returns>
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
#End Region

#Region "DataTableの先頭に、「全機種」を追加する。"
    ''' <summary>DataTableの先頭に、「全機種」を追加する。</summary>
    ''' <returns>機種マスタ情報</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        Try
            drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
            drw.Item(1) = ALL_MODE
            dt.Rows.InsertAt(drw, 0)

        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            drw = Nothing
        End Try

        Return dt
    End Function
#End Region

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("MODEL")
                dt.Columns.Add("MODEL_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

#Region "ディスコンストラクタする"
    ''' <summary>
    ''' ディスコンストラクタする
    ''' </summary>
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
#End Region

End Class
