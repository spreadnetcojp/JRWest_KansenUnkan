' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／保守ツール）
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2014/04/20  (NES)      新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DataAccess

Public Class FrmRestingMachine

    ' イベントが発生した場合に無視するか否か
    ' （True:イベントを無視する、False:イベントをハンドリングする）
    Private dontHandleEvent As Boolean

    '適用開始日
    Private sApplyDate As String = Now.ToString("yyyyMMdd")     'デフォルトをシステム日付とする

    '適用開始日
    Public Property ApplyDate() As String
        Get
            Return sApplyDate
        End Get
        Set(ByVal Value As String)
            sApplyDate = Value
        End Set
    End Property

    ' 検索時線区コード
    Private sSenkuCode As String = ""
    ' 検索時駅順コード
    Private sEkijunCode As String = ""
    ' 検索時コーナーコード
    Private sCornerCode As String = ""
    ' 検索時機種コード
    Private sKisyuCode As String = ""

    ''' <summary>
    ''' 立上処理
    ''' </summary>
    Private Sub FrmRestingMachine_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Try
            Me.Cursor = Cursors.WaitCursor

            'イベント無視開始
            dontHandleEvent = True
            ' 駅コンボ設定
            Me.setCmbEki()
            ' コーナーコンボ設定
            Me.setCmbMado(Me.cmbEki.SelectedValue.ToString)
            ' 機種コンボ設定
            Me.setCmbModel()
            'イベント無視終了
            dontHandleEvent = False

            ' datagridviewをタブキーで次に抜ける
            Me.dgvGokiList.StandardTab = True

            Me.Cursor = Cursors.Default
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(AlertBoxAttr.OK, Lexis.FormProcAbnormalEnd)
            Me.Close()
        End Try

    End Sub

    ''' <summary>
    ''' 駅コンボ変更
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbEki.SelectedIndexChanged
        If dontHandleEvent Then Exit Sub
        Try
            Me.Cursor = Cursors.WaitCursor

            ' コーナーコンボ設定
            Me.setCmbMado(Me.cmbEki.SelectedValue.ToString)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(AlertBoxAttr.OK, Lexis.FormProcAbnormalEnd)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' [駅コンボ設定]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function setCmbEki() As Boolean
        Dim dt As DataTable
        Dim oMst As StationMaster

        oMst = New StationMaster
        oMst.ApplyDate = ApplyDate
        dt = oMst.SelectTable(False, "G,Y")
        dt = oMst.SetAll()
        BaseSetMstDtToCmb(dt, cmbEki)
        cmbEki.SelectedIndex = 0
        If cmbEki.Items.Count <= 0 Then Return False
        Return True
    End Function

    ''' <summary>
    ''' [コーナーコンボ設定]
    ''' </summary>
    ''' <param name="Station">駅コード</param>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function setCmbMado(ByVal Station As String) As Boolean
        Dim dt As DataTable
        Dim oMst As CornerMaster

        oMst = New CornerMaster
        oMst.ApplyDate = ApplyDate
        If String.IsNullOrEmpty(Station) Then
            Station = ""
        End If
        If Station <> "" And Station <> ClientDaoConstants.TERMINAL_ALL Then
            dt = oMst.SelectTable(Station, "G,Y")
        End If
        dt = oMst.SetAll()
        BaseSetMstDtToCmb(dt, Me.cmbCorner)
        Me.cmbCorner.SelectedIndex = 0
        If Me.cmbCorner.Items.Count <= 0 Then Return False
        Return True
    End Function

    ''' <summary>
    ''' 機種名称コンボボックスを設定する。
    ''' </summary>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理している機種名称の一覧及び「空白」を設定する。</remarks>
    Private Function setCmbModel() As Boolean
        Dim dt As DataTable
        Dim oMst As New ModelMaster

        '機種名称コンボボックス用のデータを取得する。
        'dt = oMst.SelectTable(True)
        dt = oMst.SelectTable(False)
        If dt.Rows.Count = 0 Then
            '機種データ取得失敗
            Return False
        End If
        'dt = oMst.SetSpace()
        dt = oMst.SetAll()

        BaseSetMstDtToCmb(dt, Me.cmbModel)
        Me.cmbModel.SelectedIndex = 0
        If Me.cmbModel.Items.Count <= 0 Then Return False

        Return True
    End Function

    ''' <summary>
    ''' 端末マスタクラスより返却されたデータテーブルをコンボボックスのデータソースにバインドし、
    ''' 表示情報と設定情報を設定する。
    ''' </summary>
    ''' <param name="dt">バインド用DataTable(Columuns構成は端末マスタクラスに準拠)</param>
    ''' <param name="cmb">バインド先のComboBox</param>
    Public Shared Sub BaseSetMstDtToCmb(ByVal dt As DataTable, ByRef cmb As ComboBox)

        cmb.DataSource = Nothing
        'コンボボックス初期化
        If cmb.Items.Count > 0 Then
            cmb.Items.Clear()
        End If
        'DataSourceの設定
        cmb.DataSource = dt
        '表示メンバーの設定
        cmb.DisplayMember = dt.Columns(1).ColumnName
        'バリューメンバーの設定
        cmb.ValueMember = dt.Columns(0).ColumnName
    End Sub

    ''' <summary>
    ''' 指定Select文を実行し、DataTableに設定返却する。
    ''' オープン以外の実行エラーはOPMGExceptionを生成しThrowする。
    ''' </summary>
    ''' <param name="sSql">実行するSelect文</param>
    ''' <param name="dt">実行結果を格納するDataTable</param>
    ''' <returns>整数:処理件数,-9:オープン失敗</returns>
    Public Shared Function BaseSqlDataTableFill(ByVal sSql As String, ByRef dt As DataTable) As Integer
        Dim Cn As SqlClient.SqlConnection
        Dim da As SqlClient.SqlDataAdapter

        'オープン
        Try
            Log.Debug("Connecting to DB...")
            Cn = New SqlClient.SqlConnection(Utility.GetDbConnectString)
            Cn.Open()
            da = New SqlClient.SqlDataAdapter(sSql, Cn)
            da.SelectCommand.CommandTimeout = Config.DatabaseReadLimitSeconds
            dt = New System.Data.DataTable()
        Catch ex As Exception
            Log.Error("Unwelcome Exception caught.", ex)
            Return -9
        End Try

        '実行
        Dim nCnt As Integer
        Try
            Log.Debug(sSql & "...")
            da.Fill(dt)
            nCnt = dt.Rows.Count
            Cn.Dispose()
            da.Dispose()
        Catch ex As Exception
            If Not Log.LoggingDebug Then
                Log.Error(sSql & "...")
            End If
            Cn.Dispose()
            da.Dispose()
            Throw New OPMGException(ex)
        End Try

        Log.Debug(nCnt.ToString() & " record(s) read.")
        Return nCnt
    End Function

    ''' <summary>
    ''' メニュー画面に戻る
    ''' </summary>
    Private Sub btnReturn_Click(sender As System.Object, e As System.EventArgs) Handles btnReturn.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' 検索開始
    ''' </summary>
    Private Sub btnSearch_Click(sender As System.Object, e As System.EventArgs) Handles btnSearch.Click
        If dontHandleEvent Then Exit Sub

        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim sSql2 As String = ""
        Dim code1, code2, code3 As String

        Try
            Me.Cursor = Cursors.WaitCursor

            code1 = ""
            code2 = ""
            code3 = ""

            If Not (Me.cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code1 = Me.cmbEki.SelectedValue.ToString
            End If

            If Not (Me.cmbCorner.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code2 = Me.cmbCorner.SelectedValue.ToString
            End If

            If Not (Me.cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code3 = Me.cmbModel.SelectedValue.ToString
            End If

            'データ取得処理
            sSql =
               "select (case when mrm.MODEL_CODE is Null then convert(bit, 'true') else convert(bit, 'false') end) as kado_chk, " & _
               "vmn.BRANCH_OFFICE_CODE as BRANCH_OFFICE_CODE, " & _
               "vmn.RAIL_SECTION_CODE as RAIL_SECTION_CODE, " & _
               "vmn.STATION_ORDER_CODE as STATION_ORDER_CODE, " & _
               "vmn.STATION_NAME as STATION_NAME, " & _
               "vmn.CORNER_CODE as CORNER_CODE, " & _
               "vmn.CORNER_NAME as CORNER_NAME, " & _
               "vmn.MODEL_CODE as MODEL_CODE, " & _
               "vmn.MODEL_NAME as MODEL_NAME, " & _
               "vmn.UNIT_NO as UNIT_NO " & _
               "from V_MACHINE_NOW as vmn " & _
               "left join M_RESTING_MACHINE as mrm " & _
               "on vmn.RAIL_SECTION_CODE = mrm.RAIL_SECTION_CODE " & _
               "and vmn.STATION_ORDER_CODE = mrm.STATION_ORDER_CODE " & _
               "and vmn.CORNER_CODE = mrm.CORNER_CODE " & _
               "and vmn.MODEL_CODE = mrm.MODEL_CODE " & _
               "and vmn.UNIT_NO = mrm.UNIT_NO"

            ' 駅名
            If code1 <> "" Then
                sSql2 = " where vmn.RAIL_SECTION_CODE = '" & code1.Substring(0, 3) & "' and vmn.STATION_ORDER_CODE = '" & code1.Substring(3, 3) & "'"
            End If

            ' コーナー
            If code2 <> "" Then
                If sSql2 = "" Then
                    sSql2 = " where"
                Else
                    sSql2 = sSql2 & " and"
                End If

                sSql2 = sSql2 & " vmn.CORNER_CODE = '" & code2 & "'"
            End If

            ' 機種
            If sSql2 = "" Then
                sSql2 = " where"
            Else
                sSql2 = sSql2 & " and"
            End If

            If code3 <> "" Then
                sSql2 = sSql2 & " vmn.MODEL_CODE = '" & code3 & "'"
            Else
                ' 全機種
                Dim j, p As Integer
                p = Me.cmbModel.SelectedIndex
                For j = 1 To Me.cmbModel.Items.Count - 1
                    Me.cmbModel.SelectedIndex = j
                    If j = 1 Then
                        sSql2 = sSql2 & " vmn.MODEL_CODE in ('" & Me.cmbModel.SelectedValue.ToString() & "'"
                    Else
                        sSql2 = sSql2 & ", '" & Me.cmbModel.SelectedValue.ToString() & "'"
                    End If
                Next
                sSql2 = sSql2 & ")"
                Me.cmbModel.SelectedIndex = p
            End If

            sSql = sSql & sSql2 & " order by vmn.BRANCH_OFFICE_CODE, vmn.RAIL_SECTION_CODE, vmn.STATION_ORDER_CODE, vmn.CORNER_CODE, vmn.MODEL_CODE, vmn.UNIT_NO"

            nRtn = BaseSqlDataTableFill(sSql, dt)

            'Select Case nRtn
            '   Case -9             'ＤＢオープンエラー
            '       Exit Sub
            '   Case 0              '該当なし
            '       Exit Sub
            '   Case Is > nMaxCount     '件数＞取得可能件数
            '       Exit Sub
            'End Select

            Me.dgvGokiList.Columns.Clear()

            Me.dgvGokiList.RowHeadersVisible = False
            Me.dgvGokiList.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvGokiList.AutoGenerateColumns = True

            If nRtn < 0 Then
                AlertBox.Show(AlertBoxAttr.OK, Lexis.DatabaseOpenErrorOccurred)
            ElseIf nRtn > 0 Then
                Me.dgvGokiList.DataSource = dt

                Me.dgvGokiList.AllowUserToAddRows = False

                Dim dummy As New DataGridViewTextBoxColumn()
                dummy.DataPropertyName = "dummy"
                dummy.Name = ""
                dummy.HeaderText = ""
                Me.dgvGokiList.Columns.Add(dummy)

                Me.dgvGokiList.Columns(0).HeaderText = "稼動"
                Me.dgvGokiList.Columns(4).HeaderText = "駅"
                Me.dgvGokiList.Columns(6).HeaderText = "コーナー"
                Me.dgvGokiList.Columns(8).HeaderText = "機種"
                Me.dgvGokiList.Columns(9).HeaderText = "号機"

                Me.dgvGokiList.Columns(0).ReadOnly = False
                Me.dgvGokiList.Columns(4).ReadOnly = True
                Me.dgvGokiList.Columns(6).ReadOnly = True
                Me.dgvGokiList.Columns(8).ReadOnly = True
                Me.dgvGokiList.Columns(9).ReadOnly = True

                Me.dgvGokiList.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvGokiList.Columns(4).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvGokiList.Columns(6).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvGokiList.Columns(8).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvGokiList.Columns(9).SortMode = DataGridViewColumnSortMode.NotSortable

                Me.dgvGokiList.Columns(0).Width = 40
                Me.dgvGokiList.Columns(1).Visible = False
                Me.dgvGokiList.Columns(2).Visible = False
                Me.dgvGokiList.Columns(3).Visible = False
                Me.dgvGokiList.Columns(4).Width = 150
                Me.dgvGokiList.Columns(5).Visible = False
                Me.dgvGokiList.Columns(6).Width = 150
                Me.dgvGokiList.Columns(7).Visible = False
                Me.dgvGokiList.Columns(8).Width = 150
                Me.dgvGokiList.Columns(9).Width = 40
                Me.dgvGokiList.Columns(9).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                Me.dgvGokiList.Columns(10).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If

            ' 検索時の情報を保存
            If code1 <> "" Then
                Me.sSenkuCode = code1.Substring(0, 3)
                Me.sEkijunCode = code1.Substring(3, 3)
            Else
                Me.sSenkuCode = ""
                Me.sEkijunCode = ""
            End If
            Me.sCornerCode = code2
            Me.sKisyuCode = code3

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnEnter_Click(sender As System.Object, e As System.EventArgs) Handles btnEnter.Click
        Dim line_count As Integer
        Dim i As Integer
        Dim code1, code2, code3, code4, code5 As String
        Dim sSql As String = ""
        Dim sSql2 As String = ""
        Dim dbCtl As DatabaseTalker
        Dim completed As Boolean = False

        dbCtl = New DatabaseTalker

        Try
            '「更新してもよろしいですか？」
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUpdate) = DialogResult.No Then
                Exit Sub
            End If

            Me.Cursor = Cursors.WaitCursor

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            line_count = Me.dgvGokiList.RowCount

            If line_count > 0 Then
                sSql = "delete from M_RESTING_MACHINE"

                If Me.sSenkuCode <> "" Then
                    sSql2 = " where RAIL_SECTION_CODE = '" & Me.sSenkuCode & "' and STATION_ORDER_CODE = '" & Me.sEkijunCode & "'"
                End If

                If Me.sCornerCode <> "" Then
                    If sSql2 = "" Then
                        sSql2 = " where"
                    Else
                        sSql2 = sSql2 & " and"
                    End If

                    sSql2 = sSql2 & " CORNER_CODE = '" & Me.sCornerCode & "'"
                End If

                If Me.sKisyuCode <> "" Then
                    If sSql2 = "" Then
                        sSql2 = " where"
                    Else
                        sSql2 = sSql2 & " and"
                    End If

                    sSql2 = sSql2 & " MODEL_CODE = '" & Me.sKisyuCode & "'"
                End If

                sSql = sSql & sSql2

                dbCtl.ExecuteSQLToWrite(sSql)
            End If

            For i = 0 To line_count - 1 Step 1
                If CBool(Me.dgvGokiList(0, i).Value) = False Then
                    code1 = CStr(Me.dgvGokiList(2, i).Value)
                    code2 = CStr(Me.dgvGokiList(3, i).Value)
                    code3 = CStr(Me.dgvGokiList(5, i).Value)
                    code4 = CStr(Me.dgvGokiList(7, i).Value)
                    code5 = CStr(Me.dgvGokiList(9, i).Value)

                    'Me.saveRestingMachine(code1, code2, code3, code4, code5)

                    sSql =
                     "insert into M_RESTING_MACHINE ( " & _
                     "INSERT_DATE, " & _
                     "INSERT_USER_ID, " & _
                     "INSERT_MACHINE_ID, " & _
                     "UPDATE_DATE, " & _
                     "UPDATE_USER_ID, " & _
                     "UPDATE_MACHINE_ID, " & _
                     "RAIL_SECTION_CODE, " & _
                     "STATION_ORDER_CODE, " & _
                     "CORNER_CODE, " & _
                     "MODEL_CODE, " & _
                     "UNIT_NO " & _
                     ") values ( " & _
                     "GETDATE(), " & _
                     "'TOOL', " & _
                     "'00', " & _
                     "GETDATE(), " & _
                     "'TOOL', " & _
                     "'00', " & _
                     "'" & code1 & "', " & _
                     "'" & code2 & "', " & _
                     "'" & code3 & "', " & _
                     "'" & code4 & "', " & _
                     "'" & code5 & "' )"

                    dbCtl.ExecuteSQLToWrite(sSql)
                End If
            Next

            dbCtl.TransactionCommit()
            completed = True

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Log.Fatal("Unwelcome Exception caught.", ex)
            '「更新処理に失敗しました。」
            AlertBox.Show(AlertBoxAttr.OK, Lexis.UpdateFailed)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
            Me.Cursor = Cursors.Default
        End Try

        If completed Then
            '「更新処理が正常に終了しました。」
            AlertBox.Show(AlertBoxAttr.OK, Lexis.UpdateCompleted)
        End If
    End Sub

End Class
