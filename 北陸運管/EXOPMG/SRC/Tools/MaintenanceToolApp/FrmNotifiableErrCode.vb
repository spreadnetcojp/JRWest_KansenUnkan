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

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DataAccess

Public Class FrmNotifiableErrCode

    Private Shared ReadOnly chkNumbRegx As New Regex("^[0-9]+$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly chkDataRegx As New Regex("^[a-zA-Z0-9]+$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

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

    ' 線区情報
    Private sSenku() As String
    ' 登録線区情報
    Private dtSenku As DataTable

    ''' <summary>
    ''' 立上処理
    ''' </summary>
    Private Sub FrmNotifiableErrCode_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen

        Try
            Me.Cursor = Cursors.WaitCursor

            ' iniファイルより線区情報取得
            Dim i As Integer
            Dim s As String
            Dim sIniFilePath As String = Path.ChangeExtension(Application.ExecutablePath, ".ini")

            i = 0
            Do
                s = Constant.GetIni("Senku", "Senku" & i.ToString(), sIniFilePath)

                If s Is Nothing OrElse s = "" Then Exit Do

                ReDim Preserve Me.sSenku(i)

                Me.sSenku(i) = s

                i = i + 1
            Loop

            ' 駅コンボ設定
            Me.setCmbEki()
            ' 機種コンボ設定
            Me.setCmbModel()

            ' datagridviewをタブキーで次に抜ける
            Me.dgvErrCodeList.StandardTab = True

            Me.Cursor = Cursors.Default
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(AlertBoxAttr.OK, Lexis.FormProcAbnormalEnd)
            Me.Close()
        End Try

    End Sub

    ''' <summary>
    ''' [駅コンボ設定]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function setCmbEki() As Boolean
        Dim dt As DataTable
        Dim oMst As StationMaster = New StationMaster
        Dim cname As String()
        Dim i, j, num As Integer

        num = Me.sSenku.Length

        If num > 0 Then
            Me.dtSenku = New DataTable
            Me.dtSenku.Columns.Add("name", GetType(String))
            Me.dtSenku.Columns.Add("code", GetType(String))
        End If

        ' DB取得
        oMst.ApplyDate = ApplyDate
        dt = oMst.SelectTable(False, "G,W,Y")

        ' iniファイルsenku情報
        For i = num - 1 To 0 Step -1

            ' 線区が存在するか？
            For j = 0 To dt.Rows.Count - 1
                If (dt.Rows(j).Item(0).ToString()).Substring(0, 3) = Me.sSenku(i).Substring(0, 3) Then
                    j = -1
                    Exit For
                ElseIf Me.sSenku(i).Substring(0, 3) = "999" Then
                    j = -2
                    Exit For
                End If
            Next

            ' 存在する線区の情報追加
            If j = -1 OrElse j = -2 Then
                cname = Me.sSenku(i).Split(","c)

                Me.dtSenku.Rows.Add(cname(2), cname(0))

                Dim s As String = "STATION_NAME = '" & cname(2) & "'"

                If dt.Select(s).Length = 0 Then
                    dt = oMst.SetSpace()
                    dt.Rows(0).Item(0) = cname(0) & cname(1)
                    dt.Rows(0).Item(1) = cname(2)
                End If
            End If
        Next

        ' 全て
        dt = oMst.SetAll()
        dt.Rows(0).Item(1) = "全て"

        BaseSetMstDtToCmb(dt, cmbEki)
        cmbEki.SelectedIndex = 0

        If cmbEki.Items.Count <= 0 Then Return False

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
        dt = Me.SelectTable()
        If dt.Rows.Count = 0 Then
            '機種データ取得失敗
            Return False
        End If
        'dt = oMst.SetSpace()
        'dt = oMst.SetAll()
        dt = Me.SetAll(dt)

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
    ''' <param name="cmb">バインド必要のあるComboBox</param>
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
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim sSql2 As String = ""
        Dim code1, code2, code3 As String
        Dim senkus As String

        Try
            Me.Cursor = Cursors.WaitCursor

            code1 = ""
            code2 = ""
            code3 = ""

            senkus = ""

            If Not (Me.cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code1 = Me.cmbEki.SelectedValue.ToString
            End If

            If code1 <> "" Then
                If code1.Substring(3, 3) = "999" Then

                    Dim rows() As DataRow = Me.dtSenku.Select("name = '" & Me.cmbEki.Text.ToString & "'")

                    For Each row As DataRow In rows
                        If senkus = "" Then
                            senkus = "'" & row.Item("code").ToString & "'"
                        Else
                            senkus = senkus & ",'" & row.Item("code").ToString & "'"
                        End If
                    Next

                End If
            End If

            If Not (Me.cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code3 = Me.cmbModel.SelectedValue.ToString
            End If

            'データ取得処理
            sSql =
             "select distinct nec.RAIL_SECTION_CODE, " & _
             "nec.STATION_ORDER_CODE, " & _
             "vmn.STATION_NAME, " & _
             "nec.MODEL_CODE, " & _
             "mdl.MODEL_NAME, " & _
             "nec.ERR_CODE " & _
             "from M_NOTIFIABLE_ERR_CODE as nec " & _
             "left join V_MACHINE_NOW as vmn " & _
             "on nec.RAIL_SECTION_CODE = vmn.RAIL_SECTION_CODE " & _
             "and nec.STATION_ORDER_CODE = vmn.STATION_ORDER_CODE " & _
             "and nec.MODEL_CODE = vmn.MODEL_CODE " & _
             "left join M_MODEL as mdl " & _
             "on nec.MODEL_CODE = mdl.MODEL_CODE"

            If code1 <> "" Then
                '↑駅コンボで「全て」以外のものが選択されている場合である。
                If code1.Substring(3, 3) <> "999" Then
                    '↑駅コンボで具体的な（機器構成マスタから取得してきた）駅が選択されており、
                    'その（機器構成マスタに登録されている）駅順が「999」でない場合である。
                    'NOTE: INIファイルのSenkuセクションに駅順が「999」でないレコードが存在
                    'する可能性はないものとする。
                    sSql2 = " where nec.RAIL_SECTION_CODE = '" & code1.Substring(0, 3) & "' and nec.STATION_ORDER_CODE = '" & code1.Substring(3, 3) & "'"
                Else
                    If senkus = "" Then
                        '↑駅コンボで具体的な（機器構成マスタから取得してきた）駅が選択されており、
                        'その（機器構成マスタに登録されている）駅順が「999」の場合である。
                        sSql2 = " where nec.RAIL_SECTION_CODE = '" & code1.Substring(0, 3) & "' and nec.STATION_ORDER_CODE = '999'"
                        Log.Warn("機器構成マスタに駅順が999のレコードが存在しています。")
                    Else
                        '↑駅コンボで特別な意味の（INIファイルのSenkuセクションから取得してきた）
                        'アイテムが選択されている場合である。
                        'NOTE: INIファイルのSenkuセクションに登録されているものと同名の駅が
                        '機器構成マスタに登録されている可能性はないものとする。
                        sSql2 = " where nec.RAIL_SECTION_CODE in (" & senkus & ") and nec.STATION_ORDER_CODE = '999'"
                    End If
                End If
            End If

            If code3 <> "" Then
                If sSql2 = "" Then
                    sSql2 = " where"
                Else
                    sSql2 = sSql2 & " and"
                End If

                sSql2 = sSql2 & " nec.MODEL_CODE = '" & code3 & "'"
            End If

            sSql = sSql & sSql2 & " order by ERR_CODE, MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE"

            nRtn = BaseSqlDataTableFill(sSql, dt)

            'Select Case nRtn
            '   Case -9             'ＤＢオープンエラー
            '       Exit Sub
            '   Case 0              '該当なし
            '       Exit Sub
            '   Case Is > nMaxCount     '件数＞取得可能件数
            '       Exit Sub
            'End Select

            Me.dgvErrCodeList.Columns.Clear()

            Me.dgvErrCodeList.RowHeadersVisible = False
            Me.dgvErrCodeList.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvErrCodeList.AutoGenerateColumns = True

            If nRtn < 0 Then
                AlertBox.Show(AlertBoxAttr.OK, Lexis.DatabaseOpenErrorOccurred)
            ElseIf nRtn > 0 Then
                Me.dgvErrCodeList.DataSource = dt

                Me.dgvErrCodeList.AllowUserToAddRows = False

                Me.ReplaceUnsolvedNamesInErrCodeList()

                Dim dummy As New DataGridViewTextBoxColumn()
                dummy.DataPropertyName = "dummy"
                dummy.Name = ""
                dummy.HeaderText = ""
                Me.dgvErrCodeList.Columns.Add(dummy)

                Me.dgvErrCodeList.Columns(2).HeaderText = "駅"
                Me.dgvErrCodeList.Columns(4).HeaderText = "機種"
                Me.dgvErrCodeList.Columns(5).HeaderText = "エラーコード"

                Me.dgvErrCodeList.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvErrCodeList.Columns(4).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvErrCodeList.Columns(5).SortMode = DataGridViewColumnSortMode.NotSortable

                Me.dgvErrCodeList.Columns(0).Visible = False
                Me.dgvErrCodeList.Columns(1).Visible = False
                Me.dgvErrCodeList.Columns(2).Width = 150
                Me.dgvErrCodeList.Columns(3).Visible = False
                Me.dgvErrCodeList.Columns(4).Width = 150
                Me.dgvErrCodeList.Columns(5).Width = 150
                Me.dgvErrCodeList.Columns(6).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' 一覧表示内の未解決の駅名を解決する
    ''' </summary>
    Private Sub ReplaceUnsolvedNamesInErrCodeList()
        Dim lineCount As Integer = Me.dgvErrCodeList.Rows.Count
        Dim i As Integer

        For i = 0 To lineCount - 1
            If Me.dgvErrCodeList(2, i).Value Is Nothing OrElse Me.dgvErrCodeList(2, i).Value.ToString() = "" Then
                Dim senk As String = CStr(Me.dgvErrCodeList(0, i).Value)
                Dim ekjn As String = CStr(Me.dgvErrCodeList(1, i).Value)
                Dim name As String = ""
                If ekjn = "999" Then
                    name = Me.GetSenkuName(senk)
                End If
                If name = "" Then
                    name = "[" & senk & ekjn & "]"
                End If
                Me.dgvErrCodeList(2, i).Value = name
            End If
        Next
    End Sub

    ''' <summary>
    ''' 線区コードをiniファイルの線区名称に変換する
    ''' </summary>
    Private Function GetSenkuName(senkuCode As String) As String
        Dim rname As String = ""
        Dim cname As String()
        Dim num As Integer = Me.sSenku.Length
        Dim i As Integer

        For i = 0 To num - 1
            cname = Me.sSenku(i).Split(","c)

            If senkuCode = cname(0) Then
                rname = cname(2)
                Exit For
            End If
        Next

        Return rname
    End Function

    ''' <summary>
    ''' インポート
    ''' </summary>
    Private Sub btnImport_Click(sender As System.Object, e As System.EventArgs) Handles btnImport.Click
        Dim ofd As New OpenFileDialog()

        'ダイアログを表示する
        If ofd.ShowDialog() = DialogResult.OK Then

            Dim fname As String = ofd.FileName

            '「○○ファイルの内容で更新してもよろしいですか？」
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyImport, fname) = DialogResult.No Then
                Exit Sub
            End If

            Dim dbCtl As DatabaseTalker = New DatabaseTalker
            Dim completed As Boolean = False

            Try
                Me.Cursor = Cursors.WaitCursor

                'CSVファイルを読み込み
                Dim csvData As ArrayList = Common.ReadCsv(fname)
                Dim listCount As Integer = csvData.Count
                Dim i As Integer
                Dim data1, data2, data3, data4 As String

                data1 = ""
                data2 = ""
                data3 = ""
                data4 = ""

                Dim sSql As String = ""
                Dim sSql2 As String = ""
                Dim errflg As Integer = 0
                Dim enc As Encoding = Encoding.GetEncoding(932)

                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()

                sSql = "delete from M_NOTIFIABLE_ERR_CODE"

                dbCtl.ExecuteSQLToWrite(sSql)

                For i = 0 To listCount - 1
                    data1 = Common.ReadStringFromCSV(csvData, i, 0)     ' 線区
                    data2 = Common.ReadStringFromCSV(csvData, i, 1)     ' 駅順
                    data3 = Common.ReadStringFromCSV(csvData, i, 2)     ' 機種
                    data4 = Common.ReadStringFromCSV(csvData, i, 3)     ' エラーコード

                    ' 登録データチェック
                    If data1 = "" OrElse data2 = "" OrElse data3 = "" OrElse data4 = "" Then
                        errflg = 1
                        Exit For
                    End If
                    If data1.Length > 3 OrElse _
                       data2.Length > 3 OrElse _
                       data3.Length > 1 OrElse _
                       data4.Length > 6 Then
                        errflg = 2
                        Exit For
                    End If
                    If data1.Length <> enc.GetByteCount(data1) OrElse _
                       data2.Length <> enc.GetByteCount(data2) OrElse _
                       data3.Length <> enc.GetByteCount(data3) OrElse _
                       data4.Length <> enc.GetByteCount(data4) Then
                        errflg = 3
                        Exit For
                    End If
                    If chkNumbRegx.IsMatch(data1) = False OrElse _
                       chkNumbRegx.IsMatch(data2) = False Then
                        errflg = 4
                        Exit For
                    End If
                    If chkDataRegx.IsMatch(data4) = False Then
                        errflg = 5
                        Exit For
                    End If
                    If data3.Equals("G") = False AndAlso _
                       data3.Equals("W") = False AndAlso _
                       data3.Equals("Y") = False Then
                        errflg = 6
                        Exit For
                    End If

                    ' 桁足らず修正
                    data1 = data1.PadLeft(3, "0"c)
                    data2 = data2.PadLeft(3, "0"c)
                    data4 = data4.PadLeft(6, "0"c)

                    sSql =
                      "insert into M_NOTIFIABLE_ERR_CODE ( " & _
                      "INSERT_DATE, " & _
                      "INSERT_USER_ID, " & _
                      "INSERT_MACHINE_ID, " & _
                      "UPDATE_DATE, " & _
                      "UPDATE_USER_ID, " & _
                      "UPDATE_MACHINE_ID, " & _
                      "RAIL_SECTION_CODE, " & _
                      "STATION_ORDER_CODE, " & _
                      "MODEL_CODE, " & _
                      "ERR_CODE, " & _
                      "SNMP_SEVERITY " & _
                      ") values ( " & _
                      "GETDATE(), " & _
                      "'TOOL', " & _
                      "'00', " & _
                      "GETDATE(), " & _
                      "'TOOL', " & _
                      "'00', " & _
                      "'" & data1 & "', " & _
                      "'" & data2 & "', " & _
                      "'" & data3 & "', " & _
                      "'" & data4 & "', " & _
                      "'' )"

                    dbCtl.ExecuteSQLToWrite(sSql)
                Next

                If errflg = 1 Then
                    ' データエラー（未設定）
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr1DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 2 Then
                    ' データエラー（桁オーバー）
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr2DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 3 Then
                    ' データエラー（全角文字有り）
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr3DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 4 Then
                    ' データエラー（線区駅順に数字以外）
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr4DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 5 Then
                    ' データエラー（不正文字）
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr5DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 6 Then
                    ' データエラー（機種コード異常）
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr6DetectedOnImport, data1, data2, data3, data4)
                Else
                    ' 登録終了
                    dbCtl.TransactionCommit()
                    completed = True
                End If

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
                AlertBox.Show(AlertBoxAttr.OK, Lexis.UpdateCompleted)
                Me.btnSearch_Click(sender, e)
            End If

        End If
    End Sub

    ''' <summary>
    ''' エクスポート
    ''' </summary>
    Private Sub btnExport_Click(sender As System.Object, e As System.EventArgs) Handles btnExport.Click
        Dim sfd As New SaveFileDialog()
        sfd.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") & "ErrCode.csv"
        sfd.Filter = "CSVファイル(*.csv)|*.csv;*.CSV|すべてのファイル(*.*)|*.*"

        ' ダイアログを表示する
        If sfd.ShowDialog() = DialogResult.OK Then

            Dim fname As String = sfd.FileName

            '「○○ファイルに保存してもよろしいですか？」
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyExport, fname) = DialogResult.No Then
                Exit Sub
            End If

            Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(932)
            Dim sdata As String
            Dim nRtn As Integer
            Dim dt As New DataTable
            Dim sSql As String = ""
            Dim i As Integer

            Try
                Me.Cursor = Cursors.WaitCursor

                ' 全データ取得処理
                sSql =
                 "select RAIL_SECTION_CODE, STATION_ORDER_CODE, MODEL_CODE, ERR_CODE " & _
                 "from M_NOTIFIABLE_ERR_CODE " & _
                 "order by ERR_CODE"

                nRtn = BaseSqlDataTableFill(sSql, dt)

                If nRtn < 0 Then
                    '「DB接続に失敗しました。」
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DatabaseOpenErrorOccurred)
                Else
                    Using sw As New System.IO.StreamWriter(fname, False, enc)
                        ' ヘッダ
                        sdata = "#線区,駅順,機種,エラーコード"

                        sw.WriteLine(sdata)

                        For i = 0 To nRtn - 1
                            sdata =
                             dt.Rows(i).Item(0).ToString() & "," & _
                             dt.Rows(i).Item(1).ToString() & "," & _
                             dt.Rows(i).Item(2).ToString() & "," & _
                             dt.Rows(i).Item(3).ToString()

                            sw.WriteLine(sdata)
                        Next

                        sw.Flush()
                    End Using

                    '「保存処理が正常に終了しました。」
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.ExportCompleted)
                End If

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                '「ファイルの書き込みに失敗しました。」
                AlertBox.Show(AlertBoxAttr.OK, Lexis.ERR_FILE_WRITE)
            Finally
                Me.Cursor = Cursors.Default
            End Try
        End If
    End Sub

    ''' <summary> DBより、データを取得する。</summary>
    ''' <returns>マスタ取得結果格納テーブル</returns>
    Private Function SelectTable() As DataTable
        Dim dt As DataTable = New DataTable
        Dim dbCtl As New DatabaseTalker
        Dim sSQL As String = ""

        'テーブル:機種マスタ
        sSQL = "SELECT MODEL_CODE,MODEL_NAME  FROM M_MODEL"

        sSQL = sSQL & " WHERE FAULT_RCV_FLAG = '1'"

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

    ''' <summary>DataTableの先頭に、「全機種」を追加する。</summary>
    ''' <returns>機種マスタ情報</returns>
    Private Function SetAll(dt As DataTable) As DataTable
        Dim drw As DataRow

        drw = dt.NewRow()

        Try
            drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
            drw.Item(1) = "全機種"
            dt.Rows.InsertAt(drw, 0)

        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            drw = Nothing
        End Try

        Return dt
    End Function

End Class
