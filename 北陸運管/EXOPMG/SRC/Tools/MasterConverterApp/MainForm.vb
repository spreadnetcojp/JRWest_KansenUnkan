' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/03/01  (NES)河脇  新規作成
'   0.1      2013/11/07  (NES)河脇  フェーズ２対応
'                                   ・IC表示用ﾏｽﾀ　データ部サム値追加対応
'                                   ・列車遅延設定ﾃﾞｰﾀ　予備追加対応
'                                   ・IC土休日ﾏｽﾀ　休日数のインテル形式対応
'                                   ・IC土休日ﾏｽﾀ　BINファイル取込対応
'                                   ・JR東海 IC不正ﾏｽﾀ　BINファイル取込対応
'                                   ・JR東海 IC運用ﾏｽﾀ　BINファイル取込対応
'                                   ・判定ﾊﾟﾗﾒｰﾀ(東海用)追加対応
'   0.2      2014/06/12  (NES)田保  北陸対応
'                                   ・北陸用５種類のマスタ対応
'                                   ・ツールVerのタイトル表示対応
'                                   ・マスタ別パターン番号チェック対応
'                                   ・INPUTファイル名からVer取得対応
'                                   ・INPUTデータの拡張子チェック対応
'                                   ・INPUTデータの変換済チェック対応
'   0.3      2014/10/14  (NES)河脇  H26年度施策
'                                   ・IC表示用ﾏｽﾀ　第２世代適用日のNull対応
'                                   ・FREX定期券ﾏｽﾀ　ID番号BCD→HEX変更対応
'   0.4      2017/06/15  (NES)趙　  ポイントポストペイ対応
'                                   ・昼特区間・時間マスタ追加対応
'                                   ・ポストペイエリアマスタ追加対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.DataAccess
Imports JR.ExOpmg.Common
Imports System.IO

Public Class MainForm

    Private Const KindPrefix As String = "PR_"
    Private Const FileExt As String = "00.bin"

    '券止めマスタ
    Private Const KEN As String = "KEN"
    '列車遅延設定データ
    Private Const DLY As String = "DLY"
    '遅払い設定データ
    Private Const PAY As String = "PAY"
    'ＩＣ表示用マスタ
    Private Const ICD As String = "ICD"
    '紛失券ＩＤデータ
    Private Const LOS As String = "LOS"
    '割引コード設定データ
    Private Const DSC As String = "DSC"
    '祝祭日データ
    Private Const HLD As String = "HLD"
    '在来特急券着駅データ
    Private Const EXP As String = "EXP"
    'ＦＲＥＸ定期券ＩＤデータ
    Private Const FRX As String = "FRX"
    'ＩＣ土休日マスタ
    Private Const ICH As String = "ICH"
    'ＪＲ西エリア　ＩＣ不正マスタ
    Private Const FJW As String = "FJW"
    'ＪＲ西エリア　ＩＣ発行機関マスタ
    Private Const IJW As String = "IJW"
    'ＪＲ東海エリア　ＩＣ不正マスタ
    Private Const FJC As String = "FJC"
    'ＪＲ東海エリア　ＩＣ運用マスタ
    Private Const IJC As String = "IJC"
    'ＪＲ東日本エリア　ＩＣ不正マスタ
    Private Const FJR As String = "FJR"
    '判定パラメータ
    Private Const DSH As String = "DSH"
    '終列車時刻データ
    Private Const LST As String = "LST"
    'ＪＲ東日本エリア　ＩＣ運用マスタ
    Private Const IJE As String = "IJE"
    'サイクル判定救済マスタ
    Private Const CYC As String = "CYC"
    '券通しマスタ
    Private Const STP As String = "STP"
    '商品・割引コードデータ
    Private Const PNO As String = "PNO"
    'フリーコードデータ
    Private Const FRC As String = "FRC"
    '改札機動作マスタ
    Private Const DUS As String = "DUS"
    '駅名データ（JR東海　Suicaエリア）
    Private Const NSI As String = "NSI"
    '駅名データ（JR東海　TOICAエリア）
    Private Const NTO As String = "NTO"
    '駅名データ（JR東海　ICOCAエリア）
    Private Const NIC As String = "NIC"
    '駅名データ（JR西日本　ICOCAエリア）
    Private Const NJW As String = "NJW"

    'Ver0.2 ADD START  北陸対応
    '北陸(JRE)新幹線ＩＣカード運用マスタ
    Private Const IUK As String = "IUK"
    '北陸(JRE)改札機過渡期制限マスタ
    Private Const KSZ As String = "KSZ"
    '北陸(JRE)新幹線改札機スイッチマスタ
    Private Const SWK As String = "SWK"
    '北陸(JRE)ＩＣカード運用マスタ
    Private Const IUZ As String = "IUZ"
    '北陸エリア　新幹線不正パラメータ
    Private Const FSK As String = "FSK"
    'Ver0.2 ADD END    北陸対応

    'Ver0.4 ADD START  ポイントポストペイ対応
    '昼特区間・時間マスタ
    Private Const HIR As String = "HIR"
    'ポストペイエリアマスタ
    Private Const PPA As String = "PPA"
    'Ver0.4 ADD END    ポイントポストペイ対応

    '前回選択したファイル名
    Private OriFileName As String = Nothing

    'Ver0.2 ADD START  北陸対応
    'パターン番号取得
    Private PatternNo As Integer
    'Ver0.2 ADD END    北陸対応

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dt As DataTable
        Dim oMst As ModelMaster
        oMst = New ModelMaster
        Try
            'Ver0.2 ADD START  北陸対応
            Me.Text = Config.MachineKind
            If Config.VerNoSet.Length <> 0 Then
                Me.Text = Me.Text + " Ver " + Config.VerNoSet
            End If
            'Ver0.2 ADD END    北陸対応

            'データを取得する
            dt = oMst.SelectTable()
            '空白行を追加する
            dt = oMst.SetSpace

            'DataSourceの設定
            cmbModel.DataSource = dt
            '表示メンバーの設定
            cmbModel.DisplayMember = dt.Columns(1).ColumnName
            'バリューメンバーの設定
            cmbModel.ValueMember = dt.Columns(0).ColumnName
        Catch ex As Exception
            ControlDisable()
            AlertBox.Show(Lexis.ERR_UNKNOWN)
            Exit Sub
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
    End Sub

    Private Sub cmbModel_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        'Ver0.1 MOD START  フェーズ２対応
        'Dim dt As DataTable
        'Dim oMst As MasterMaster = New MasterMaster
        Dim dt As DataTable = New DataTable()
        'Ver0.1 MOD END

        Try
            Me.Cursor = Cursors.WaitCursor

            'バージョンをクリア
            txtVersion.Text = ""

            'コンボボックス初期化
            If cmbPattern.Items.Count > 0 Then
                'パターンクリア
                cmbPattern.DataSource = Nothing
                cmbPattern.Items.Clear()
            End If

            If cmbModel.SelectedIndex = 0 Then
                'マスタデータ名称をクリア
                cmbMaster.DataSource = Nothing
                cmbMaster.Items.Clear()
                Exit Sub
            End If

            'Ver0.1 MOD START  フェーズ２対応
            'dt = oMst.SelectTableShort(cmbModel.SelectedValue.ToString)
            'dt = oMst.SetSpace
            dt.Columns.Add("CODE")
            dt.Columns.Add("NAME")
            dt.Columns.Add("INDATA")
            dt.Columns.Add("INTYPE")

            dt.Rows.Add(New String() {"", "", "", ""})
            dt.Rows.Add(New String() {"KEN券止めﾏｽﾀ", "券止めマスタ", "0", "BIN"})
            dt.Rows.Add(New String() {"DLY列車遅延設定ﾃﾞｰﾀ", "列車遅延設定データ", "0", "CSV"})
            dt.Rows.Add(New String() {"PAY遅払い設定ﾃﾞｰﾀ", "遅払い設定データ", "0", "CSV"})
            dt.Rows.Add(New String() {"ICDIC表示用ﾏｽﾀ", "ＩＣ表示用マスタ", "0", "CSV"})
            dt.Rows.Add(New String() {"LOS紛失券IDﾃﾞｰﾀ", "紛失券ＩＤデータ", "0", "CSV"})
            dt.Rows.Add(New String() {"DSC割引ｺｰﾄﾞ設定ﾃﾞｰﾀ", "割引コード設定データ", "0", "CSV"})
            dt.Rows.Add(New String() {"HLD祝祭日ﾃﾞｰﾀ", "祝祭日データ", "0", "CSV"})
            dt.Rows.Add(New String() {"EXP在来特急券着駅ﾃﾞｰﾀ", "在来特急券着駅データ", "0", "CSV"})
            dt.Rows.Add(New String() {"FRXFREXIDﾃﾞｰﾀ", "ＦＲＥＸ定期券ＩＤデータ", "0", "CSV"})
            dt.Rows.Add(New String() {"ICHIC土休日ﾏｽﾀ", "ＩＣ土休日マスタ（CSV→BIN）", "0", "CSV"})
            dt.Rows.Add(New String() {"ICHIC土休日ﾏｽﾀ", "ＩＣ土休日マスタ（BIN→BIN）", "1", "BIN"})
            dt.Rows.Add(New String() {"FJWJR西IC不正ﾏｽﾀ", "ＪＲ西エリア　ＩＣ不正マスタ", "0", "CSV"})
            dt.Rows.Add(New String() {"IJWJR西IC発行機関ﾏｽﾀ", "ＪＲ西エリア　ＩＣ発行機関マスタ", "0", "CSV"})
            dt.Rows.Add(New String() {"FJCJR東海IC不正ﾏｽﾀ", "ＪＲ東海エリア　ＩＣ不正マスタ（CSV→BIN）", "0", "CSV"})
            dt.Rows.Add(New String() {"FJCJR東海IC不正ﾏｽﾀ", "ＪＲ東海エリア　ＩＣ不正マスタ（BIN→BIN）", "1", "BIN"})
            dt.Rows.Add(New String() {"IJCJR東海IC運用ﾏｽﾀ", "ＪＲ東海エリア　ＩＣ運用マスタ（CSV→BIN）", "0", "CSV"})
            dt.Rows.Add(New String() {"IJCJR東海IC運用ﾏｽﾀ", "ＪＲ東海エリア　ＩＣ運用マスタ（BIN→BIN）", "1", "BIN"})
            dt.Rows.Add(New String() {"FJRJR東IC不正ﾏｽﾀ", "ＪＲ東日本エリア　ＩＣ不正マスタ", "0", "BIN"})
            dt.Rows.Add(New String() {"DSH判定ﾊﾟﾗﾒｰﾀ", "判定パラメータ", "0", "CSV"})
            dt.Rows.Add(New String() {"DSH判定ﾊﾟﾗﾒｰﾀ", "判定パラメータ（東海用）", "1", "CSV"})
            dt.Rows.Add(New String() {"LST終列車時刻ﾃﾞｰﾀ", "終列車時刻データ", "0", "CSV"})
            dt.Rows.Add(New String() {"IJEJR東IC運用ﾏｽﾀ", "ＪＲ東日本エリア　ＩＣ運用マスタ", "0", "BIN"})
            dt.Rows.Add(New String() {"CYCｻｲｸﾙ判定救済ﾏｽﾀ", "サイクル判定救済マスタ", "0", "BIN"})
            dt.Rows.Add(New String() {"STP券通しﾏｽﾀ", "券通しマスタ", "0", "BIN"})
            dt.Rows.Add(New String() {"PNO商品･割引ｺｰﾄﾞﾃﾞｰﾀ", "商品・割引コードデータ", "0", "BIN"})
            dt.Rows.Add(New String() {"FRCﾌﾘｰｺｰﾄﾞﾃﾞｰﾀ", "フリーコードデータ", "0", "BIN"})
            dt.Rows.Add(New String() {"DUS改札機動作ﾏｽﾀ", "改札機動作マスタ", "0", "CSV"})
            dt.Rows.Add(New String() {"NSI駅名ﾃﾞｰﾀ東海Suica", "駅名データ（JR東海　Suicaエリア）", "0", "CSV"})
            dt.Rows.Add(New String() {"NTO駅名ﾃﾞｰﾀ東海TOICA", "駅名データ（JR東海　TOICAエリア）", "0", "CSV"})
            dt.Rows.Add(New String() {"NIC駅名ﾃﾞｰﾀ東海ICOCA", "駅名データ（JR東海　ICOCAエリア）", "0", "CSV"})
            dt.Rows.Add(New String() {"NJW駅名ﾃﾞｰﾀ西日本ICOCA", "駅名データ（JR西日本　ICOCAエリア）", "0", "CSV"})
            'Ver0.1 MOD END
            'Ver0.2 ADD START  北陸対応
            dt.Rows.Add(New String() {"IUK新幹線ICｶｰﾄﾞ運用ﾏｽﾀ", "北陸(JRE)新幹線ＩＣカード運用マスタ", "1", "BIN"})
            dt.Rows.Add(New String() {"IUZICｶｰﾄﾞ運用ﾏｽﾀ", "北陸(JRE)ＩＣカード運用マスタ", "1", "BIN"})
            dt.Rows.Add(New String() {"KSZ改札機過渡期制限ﾏｽﾀ", "北陸(JRE)改札機過渡期制限マスタ", "1", "BIN"})
            dt.Rows.Add(New String() {"SWK新幹線改札機ｽｲｯﾁﾏｽﾀ", "北陸(JRE)新幹線改札機スイッチマスタ", "1", "BIN"})
            dt.Rows.Add(New String() {"FSK新幹線不正ﾊﾟﾗﾒｰﾀ", "北陸(JRE)新幹線不正パラメータ", "0", "CSV"})
            'Ver0.2 ADD END    北陸対応

            'Ver0.4 ADD START  ポイントポストペイ対応
            dt.Rows.Add(New String() {"HIRJR西昼特区間･時間ﾏｽﾀ", "昼特区間・時間マスタ", "0", "CSV"})
            dt.Rows.Add(New String() {"PPAJR西ﾎﾟｽﾄﾍﾟｲｴﾘｱﾏｽﾀ", "ポストペイエリアマスタ", "0", "CSV"})
            'Ver0.4 ADD END    ポイントポストペイ対応

            'コンボボックス初期化
            If cmbMaster.Items.Count > 0 Then
                cmbMaster.DataSource = Nothing
                cmbMaster.Items.Clear()
            End If
            'DataSourceの設定
            cmbMaster.DataSource = dt
            '表示メンバーの設定
            cmbMaster.DisplayMember = dt.Columns(1).ColumnName
            'バリューメンバーの設定
            cmbMaster.ValueMember = dt.Columns(0).ColumnName
        Catch ex As Exception
            ControlDisable()
            AlertBox.Show(Lexis.ERR_UNKNOWN)
        Finally
            Me.Cursor = Cursors.Default
            'oMst = Nothing
            dt = Nothing
        End Try
    End Sub

    Private Sub cmbMaster_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbMaster.SelectedIndexChanged

        Dim dt As DataTable
        Dim oMst As New PatternMaster
        Dim inType As String

        Try
            Me.Cursor = Cursors.WaitCursor

            'バージョンをクリア
            txtVersion.Text = ""

            'コンボボックス初期化
            If cmbPattern.Items.Count > 0 Then
                'パターンクリア
                cmbPattern.DataSource = Nothing
                cmbPattern.Items.Clear()
            End If

            'マスタデータ名称がない場合、returnする
            If cmbMaster.DataSource Is Nothing Then Return
            Dim master As String = CType(cmbMaster.SelectedItem, DataRowView).Item(0).ToString()
            If master.Length = 0 Then Return

            '先頭３文字を取得
            master = master.Substring(0, 3)

            dt = oMst.SelectTable(cmbModel.SelectedValue.ToString, master)
            dt = oMst.SetSpace

            'DataSourceの設定
            cmbPattern.DataSource = dt
            '表示メンバーの設定
            cmbPattern.DisplayMember = dt.Columns(0).ColumnName
            'バリューメンバーの設定
            cmbPattern.ValueMember = dt.Columns(1).ColumnName
            '入力ファイルタイプ
            inType = CType(cmbMaster.SelectedItem, DataRowView).Item(3).ToString().ToUpper()
            If ChkFileType(inType, Me.txtFileName.Text) = False Then
                cmbMaster.SelectedIndex = 0
                Return
            End If

            'バージョンを取得する
            Try
                'Ver0.2 ADD START  北陸対応
                'ファイル名からバージョンを取得
                txtVersion.Text = GetFileVersion(Me.txtFileName.Text)
                If txtVersion.Text = "" Then
                    'ファイル名からバージョンが取れなかった時はマスタ別の処理
                    txtVersion.Text = GetVersion(master, Me.txtFileName.Text)
                End If
                'Ver0.2 ADD END    北陸対応
            Catch ex As Exception

            End Try
        Catch ex As Exception
            ControlDisable()
            AlertBox.Show(Lexis.ERR_UNKNOWN)
        Finally
            Me.Cursor = Cursors.Default
            oMst = Nothing
            dt = Nothing
        End Try
    End Sub

    Private Sub cmbPattern_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbPattern.SelectedIndexChanged
        'パターンNOをクリア
        txtPattern.Text = ""

        If Not cmbPattern.SelectedItem Is Nothing Then
            txtPattern.Text = CType(cmbPattern.SelectedItem, DataRowView).Item(1).ToString
        End If

        EnableBtnConvert()
    End Sub

    Private Sub btnFileOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFileOpen.Click

        Dim ofd As New OpenFileDialog()

        'ダイアログを表示する
        If ofd.ShowDialog() = DialogResult.OK Then
            'OKボタンがクリックされたとき
            '選択されたファイル名を表示する
            txtFileName.Text = ofd.FileName

            cmbModel.Enabled = True
            cmbMaster.Enabled = True
            cmbPattern.Enabled = True
            txtVersion.Enabled = True

            If Not OriFileName Is Nothing And txtFileName.Text.CompareTo(OriFileName) <> 0 Then
                cmbModel.SelectedIndex = 0
            End If

            OriFileName = txtFileName.Text
        End If
    End Sub


    Private Sub txtFileName_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFileName.LostFocus
        If Not System.IO.File.Exists(txtFileName.Text) Then
            cmbModel.Enabled = False
            cmbMaster.Enabled = False
            cmbPattern.Enabled = False
            btnConvert.Enabled = False
            txtVersion.Enabled = False
        Else
            cmbModel.Enabled = True
            cmbMaster.Enabled = True
            cmbPattern.Enabled = True
            txtVersion.Enabled = True
        End If

        If Not OriFileName Is Nothing And txtFileName.Text.CompareTo(OriFileName) <> 0 Then
            cmbModel.SelectedIndex = 0
        End If

        OriFileName = txtFileName.Text
    End Sub


    Private Sub txtVersion_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtVersion.LostFocus
        EnableBtnConvert()
    End Sub

    Private Sub ControlDisable()
        Me.txtFileName.Enabled = False
        Me.btnFileOpen.Enabled = False
        Me.cmbModel.Enabled = False
        Me.cmbMaster.Enabled = False
        Me.txtVersion.Enabled = False
        Me.cmbPattern.Enabled = False
        Me.btnConvert.Enabled = False
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' 券止めマスタ変換
    ''' </summary>
    Private Function MakePR_KEN() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H59, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        Try
            Dim str As String = Utility.DECtoCHAR(Utility.GetBytesFromBytes(binData, 8, 8))
            fileData.AddRange(Utility.CHARtoBCD(str, 4))
            '日付チェック
            DateTime.Parse(Format(CInt(str), "0000/00/00"))
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, "適用日付")
            Throw
        End Try

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' IC表示用マスタ変換
    ''' </summary>
    Private Function MakePR_ICD() As Byte()

        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'Ver0.1 DEL START　サム値追加対応
        ''パラメータ識別
        'fileData.Add(CType(&H55, Byte))

        ''フォーマット識別番号
        'fileData.Add(CType(&H0, Byte))

        ''適用日付
        'fileData.AddRange(Common.GetApplyDate(Common.ReadStringFromCSV(csvData, 1, 1), "適用日付"))
        'Ver0.1 DEL END

        'データ部を追加
        'マスタバージョン
        fileData.AddRange(Common.GetVersion(Common.ReadStringFromCSV(csvData, 0, 0)))

        '予備
        fileData.AddRange(New Byte() {&H0, &H0, &H0})

        'Ver0.1 ADD START　サム値追加対応
        'データサム値――後で計算する
        fileData.AddRange(New Byte(3) {})
        'Ver0.1 ADD END

        '世代１
        '適用日
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(csvData, 1, 1), "世代１適用日"))

        Dim i As Integer
        For i = 2 To 201
            '事業者　地域番号
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, i, 1), "0", "255", "世代１事業者　地域番号"))

            '事業者　ユーザ番号
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, i, 2), "0", "255", "世代１事業者　ユーザ番号"))

            '事業者IDi文字（半角）
            Dim str As String = Common.ReadStringFromCSV(csvData, i, 3)

            If str.Length = 0 Then
                fileData.AddRange(New Byte() {&H0, &H0})
            Else
                Dim r As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]+$")
                If r.IsMatch(str) = False Or str.Length <> 2 Then
                    AlertBox.Show(Lexis.ERR_COMMON, "世代１事業者IDi文字")
                    Throw New Exception
                End If

                fileData.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes(str))
            End If
        Next

        '世代２
        '適用日
        'Ver0.3 MOD START　第２世代適用日のNull対応
        Dim Date_Str As String = Common.ReadStringFromCSV(csvData, 202, 1)
        If Date_Str.Length = 0 Then
            fileData.AddRange(New Byte() {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0})
        Else
            fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(csvData, 202, 1), "世代２適用日"))
        End If
        'Ver0.3 MOD END  　第２世代適用日のNull対応

        For i = 203 To 402
            '事業者　地域番号
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, i, 1), "0", "255", "世代２事業者　地域番号"))

            '事業者　ユーザ番号
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, i, 2), "0", "255", "世代２事業者　ユーザ番号"))

            '事業者IDi文字（半角）
            Dim str As String = Common.ReadStringFromCSV(csvData, i, 3)

            If str.Length = 0 Then
                fileData.AddRange(New Byte() {&H0, &H0})
            Else
                Dim r As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]+$")
                If r.IsMatch(str) = False Or str.Length <> 2 Then
                    AlertBox.Show(Lexis.ERR_COMMON, "世代２事業者IDi文字")
                    Throw New Exception
                End If

                fileData.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes(str))
            End If
        Next

        'Ver0.1 ADD START　サム値追加対応
        Dim ret As Byte() = CType(fileData.ToArray(GetType(Byte)), Byte())

        Dim sumValue As Long = 0
        'マスタバージョンを除く
        For k As Integer = 1 To ret.Length - 1
            sumValue += ret(k)
            sumValue = sumValue And &HFFFFFFFF
        Next

        'サム値
        Dim sum As Byte() = BitConverter.GetBytes(CUInt(sumValue))
        ret(4) = sum(3)
        ret(5) = sum(2)
        ret(6) = sum(1)
        ret(7) = sum(0)

        fileData.Clear()

        'パラメータ識別
        fileData.Add(CType(&H55, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange(Common.GetApplyDate(Common.ReadStringFromCSV(csvData, 1, 1), "適用日付"))

        fileData.AddRange(ret)
        'Ver0.1 ADD END

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 列車遅延設定データ変換
    ''' </summary>
    Private Function MakePR_DLY() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H41, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim i, row As Integer

        '遅延設定フラブ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "1", "遅延設定フラブ"))

        'Ver0.1 ADD 予備追加対応
        '予備
        fileData.AddRange((New Byte() {&H0, &H0}))

        '上り遅延列車数
        row = row + 1
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "300", "上り遅延列車数"))

        Dim count As Integer = Integer.Parse(Common.ReadStringFromCSV(csvData, row, 0))

        For i = 1 To count
            row = row + 1

            '上り遅延列車番号
            fileData.AddRange(Common.GetBytes3BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "999999", "上り遅延列車番号"))

            '上り予備
            fileData.AddRange(New Byte() {&H0})
        Next

        For i = count + 1 To 300
            row = row + 1

            '上り遅延列車番号
            fileData.AddRange(New Byte() {&H0, &H0, &H0})

            '上り予備
            fileData.AddRange(New Byte() {&H0})
        Next

        '下り遅延列車数
        row = row + 1
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "300", "下り遅延列車数"))

        count = Integer.Parse(Common.ReadStringFromCSV(csvData, row, 0))

        For i = 1 To count
            row = row + 1

            '下り遅延列車番号
            fileData.AddRange(Common.GetBytes3BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "999999", "下り遅延列車番号"))

            '下り予備
            fileData.AddRange(New Byte() {&H0})
        Next

        For i = count + 1 To 300
            row = row + 1

            '下り遅延列車番号
            fileData.AddRange(New Byte() {&H0, &H0, &H0})

            '下り予備
            fileData.AddRange(New Byte() {&H0})
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 遅払い設定データ変換
    ''' </summary>
    Private Function MakePR_PAY() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H42, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim i, row As Integer

        '上り遅払い列車数
        row = row + 1
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "15", "上り遅払い列車数"))

        Dim count As Integer = Integer.Parse(Common.ReadStringFromCSV(csvData, row, 0))

        For i = 1 To count
            row = row + 1

            '上り遅払い列車番号
            fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "999", "上り遅払い列車番号"))

            '上り遅払い設定種別
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "1", "3", "上り遅払い設定種別"))

            '予備
            fileData.AddRange(New Byte() {&H0})
        Next

        For i = count + 1 To 15
            row = row + 1

            '上り遅払い列車番号
            fileData.AddRange(New Byte() {&H0, &H0})

            '上り遅払い設定種別
            fileData.AddRange(New Byte() {&H0})

            '予備
            fileData.AddRange(New Byte() {&H0})
        Next

        '下り遅払い列車数
        row = row + 1
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "15", "下り遅払い列車数"))

        count = Integer.Parse(Common.ReadStringFromCSV(csvData, row, 0))

        For i = 1 To count
            row = row + 1

            '下り遅払い列車番号
            fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "999", "下り遅払い列車番号"))

            '下り遅払い設定種別
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "1", "3", "下り遅払い設定種別"))

            '予備
            fileData.AddRange(New Byte() {&H0})
        Next

        For i = count + 1 To 15
            row = row + 1

            '下り遅払い列車番号
            fileData.AddRange(New Byte() {&H0, &H0})

            '下り遅払い設定種別
            fileData.AddRange(New Byte() {&H0})

            '予備
            fileData.AddRange(New Byte() {&H0})
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 紛失券ＩＤデータ変換
    ''' </summary>
    Private Function MakePR_LOS() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H48, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加

        Dim row As Integer = 1

        '券種数
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "20", "券種数"))
        Dim ticketCount As Integer = Integer.Parse(Common.ReadStringFromCSV(csvData, row, 0))

        Dim i, j As Integer

        For i = 1 To ticketCount

            row = row + 1

            '発駅コード線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "255", "発駅コード線区"))

            '発駅コード駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "発駅コード駅順"))

            '券種３
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "99", "券種３"))

            '予備
            fileData.Add(CType(&H0, Byte))

            'ＩＤ数
            fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "100", "ＩＤ数"))
            Dim idCount As Integer = Integer.Parse(Common.ReadStringFromCSV(csvData, row, 4))

            For j = 1 To idCount
                row = row + 1

                '再発行フラグ
                fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "1", "再発行フラグ"))

                'ＩＤ番号
                fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "99999999", "ＩＤ番号", 4))

                '有効終了日
                fileData.AddRange(Common.GetBCDDate(Common.ReadStringFromCSV(csvData, row, 3), "有効終了日"))

                '予備
                fileData.Add(CType(&H0, Byte))
            Next

            For j = idCount + 1 To 100
                row = row + 1
                fileData.AddRange(New Byte(10 - 1) {})
            Next
        Next

        For i = ticketCount + 1 To 20
            fileData.AddRange(New Byte(1006 - 1) {})
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    ''' <summary>
    ''' 割引コード設定データ変換
    ''' </summary>
    Private Function MakePR_DSC() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H49, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim row As Integer = 1

        'エドモンソン券割引ビット
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "1", "エドモンソン券割引ビット"))

        '定期券割引ビット
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "1", "定期券割引ビット"))

        '定期券通学ビット
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "定期券通学ビット"))

        '大型券割引コード０～255
        Dim i As Integer
        For i = 0 To 255
            row = row + 1

            '表示灯
            Dim disp As Byte = Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "1", "大型券割引コード表示灯")(0)

            '通過判定
            Dim pass As Byte = Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "大型券割引コード通過判定")(0)
            pass = pass << 4

            '通過判定 and 表示灯して、配列に追加
            fileData.Add(disp Or pass)
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function


    ''' <summary>
    ''' 祝祭日データ変換
    ''' </summary>
    Private Function MakePR_HLD() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H4A, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim i, row As Integer

        For i = 1 To 20
            row = row + 1
            '固定祝祭日　月
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "12", "固定祝祭日　月", 1))

            '固定祝祭日　日
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "31", "固定祝祭日　日", 1))
        Next

        For i = 1 To 5
            row = row + 1
            '特別休日　月
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "12", "特別休日　月", 1))

            '特別休日　日
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "31", "特別休日　日", 1))
        Next

        For i = 1 To 30
            row = row + 1
            '変動祝祭日　年
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "9999", "変動祝祭日　年", 2))

            '変動祝祭日　月
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "12", "変動祝祭日　月", 1))

            '変動祝祭日　日
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "31", "変動祝祭日　日", 1))
        Next

        '時差入場ＯＫ　From時刻
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "99", "時差入場ＯＫ　From時刻　時", 1))
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "時差入場ＯＫ　From時刻　分", 1))

        '時差入場ＯＫ　To時刻
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "99", "時差入場ＯＫ　To時刻　時", 1))
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "時差入場ＯＫ　To時刻　分", 1))

        '時差出場ＯＫ　From時刻
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "99", "時差出場ＯＫ　From時刻　時", 1))
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "時差出場ＯＫ　From時刻　分", 1))

        '時差出場ＯＫ　To時刻
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "99", "時差出場ＯＫ　To時刻　時", 1))
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "時差出場ＯＫ　To時刻　分", 1))

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 在来特急券着駅データ変換
    ''' </summary>
    Private Function MakePR_EXP() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H4B, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加

        Dim i, row As Integer
        For i = 1 To 5
            row = row + 1
            '自駅近隣在来特急券着駅線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "自駅近隣在来特急券着駅　線区コード"))

            '自駅近隣在来特急券着駅駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "自駅近隣在来特急券着駅　駅順コード"))
        Next

        For i = 1 To 50
            row = row + 1

            '他駅近隣在来特急券着駅線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "他駅近隣在来特急券着駅　線区コード"))

            '他駅近隣在来特急券着駅駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "他駅近隣在来特急券着駅　駅順コード"))
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function


    ''' <summary>
    ''' ＦＲＥＸ定期券ＩＤデータ変換
    ''' </summary>
    Private Function MakePR_FRX() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H4C, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加

        Dim row As Integer = 1

        'ＩＤ数
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "500", "ＩＤ数"))
        Dim ticketCount As Integer = Integer.Parse(Common.ReadStringFromCSV(csvData, row, 0))

        Dim i As Integer
        'Ver0.3 ADD START　ID番号BCD→HEX変更対応
        Dim r As New System.Text.RegularExpressions.Regex("^[a-fA-F0-9]{14}$")
        Dim str As String
        'Ver0.3 ADD END  　ID番号BCD→HEX変更対応

        For i = 1 To ticketCount
            row = row + 1

            '発行機関（ユーザコード）
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "発行機関（ユーザコード）"))

            '予備
            fileData.Add(CType(&H0, Byte))

            '再発行フラグ
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "1", "再発行フラグ"))

            'ＩＤ番号
            'Ver0.3 MOD START　ID番号BCD→HEX変更対応
            'If Not Common.IsBetweenAnd(str, "0", "99999999999999") Then
            '    AlertBox.Show(Lexis.ERR_COMMON, "ＩＤ番号")
            '    Throw New Exception
            'End If
            str = Common.ReadStringFromCSV(csvData, row, 4)
            If r.IsMatch(str) = False Then
                AlertBox.Show(Lexis.ERR_COMMON, "ＩＤ番号")
                Throw New Exception
            End If
            'Ver0.3 MOD END  　ID番号BCD→HEX変更対応

            fileData.AddRange(Utility.CHARtoBCD(str, 7))

            '終了日
            fileData.AddRange(Common.GetBCDDate(Common.ReadStringFromCSV(csvData, row, 5), "終了日"))

        Next

        For i = ticketCount + 1 To 500
            fileData.AddRange(New Byte(14 - 1) {})
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function


    ''' <summary>
    ''' ＩＣ土休日マスタ変換
    ''' </summary>
    Private Function MakePR_ICH() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H44, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        Dim row As Integer
        'データ部を追加
        '使用可能開始日
        fileData.AddRange(Common.GetApplyDate(Common.ReadStringFromCSV(csvData, row, 2), "使用可能開始日"))

        '予備
        fileData.Add(CType(&H0, Byte))

        '使用可能終了日
        row = row + 1
        fileData.AddRange(Common.GetApplyDate(Common.ReadStringFromCSV(csvData, row, 2), "使用可能終了日"))

        '予備
        fileData.Add(CType(&H0, Byte))

        '休日数
        row = row + 1
        'Ver0.1 MOD START  休日数のインテル形式対応
        'Dim temp As Byte() = Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "50", "休日数")
        'fileData.Add(temp(1))
        'fileData.Add(temp(0))
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "50", "休日数"))
        'Ver0.1 MOD END
        Dim Count As Integer = Integer.Parse(Common.ReadStringFromCSV(csvData, 2, 2))

        '休日データ部
        Dim i As Integer

        For i = 1 To Count
            row = row + 1

            '年月日
            fileData.AddRange(Common.GetApplyDate(Common.ReadStringFromCSV(csvData, row, 1), "休日データ部年月日"))
        Next

        For i = Count + 1 To 50
            fileData.AddRange(New Byte(4 - 1) {})
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    'Ver0.1 ADD START  BINファイル取込対応
    ''' <summary>
    ''' ＩＣ土休日マスタ変換（BIN→BIN）
    ''' </summary>
    Private Function MakePR_ICH2() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H44, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function
    'Ver0.1 ADD END


    ''' <summary>
    ''' ＪＲ西エリア　ＩＣ不正マスタ変換
    ''' </summary>
    Private Function MakePR_FJW() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'マスタバージョン
        fileData.AddRange(Common.GetVersion(Common.ReadStringFromCSV(csvData, 0, 0)))

        'データサム値――後で計算する
        fileData.AddRange(New Byte(3) {})

        '予備
        fileData.Add(CType(&H0, Byte))

        '世代２オフセット――後で計算する
        fileData.AddRange(New Byte(1) {})

        Dim row As Integer = 0

        '==============世代１==================
        Dim temp As Byte()

        '適用日
        row = row + 1
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(csvData, row, 1), "適用日"))

        temp = Common.GetApplyDate(Common.ReadStringFromCSV(csvData, row, 1), "適用日")

        '連続入場判定入／切　定期
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続入場判定入／切　定期"))

        '連続入場判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続入場判定入／切　ＩＣ"))

        '連続出場判定入／切　定期
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続出場判定入／切　定期"))

        '連続出場判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続出場判定入／切　ＩＣ"))

        '連続出場判定（他駅出場）入／切　定期
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続出場判定（他駅出場）入／切　定期"))

        '連続出場判定（他駅出場）入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続出場判定（他駅出場）入／切　ＩＣ"))

        '入場側サイクル判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入場側サイクル判定入／切　ＩＣ"))

        '出場側サイクル判定入／切　近距離券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　近距離券"))

        '出場側サイクル判定入／切　回数券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　回数券"))

        '出場側サイクル判定入／切　定期券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　定期券"))

        '出場側サイクル判定入／切　入場券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　入場券"))

        '出場側サイクル判定入／切　発着固定券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　発着固定券"))

        '出場側サイクル判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　ＩＣ"))

        '入場券入出場時間判定入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入場券入出場時間判定入／切"))

        '入出場時間判定入／切　近距離券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　近距離券"))

        '入出場時間判定入／切　回数券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　回数券"))

        '入出場時間判定入／切　定期券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　定期券"))

        '入出場時間判定入／切　入場券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　入場券"))

        '入出場時間判定入／切　発着固定券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　発着固定券"))

        '入出場時間判定入／切　ＳＦカード
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　ＳＦカード"))

        '入出場時間判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　ＩＣ"))

        '入出場同一駅判定（入場）入／切　定期券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場同一駅判定（入場）入／切　定期券"))

        '入出場同一駅判定（入場）入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場同一駅判定（入場）入／切　ＩＣ"))

        '入出場同一駅判定（出場）入／切　定期券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場同一駅判定（出場）入／切　定期券"))

        '入出場同一駅判定（出場）入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場同一駅判定（出場）入／切　ＩＣ"))

        '期限切れ定期券判定入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "期限切れ定期券判定入／切"))

        '予備
        fileData.AddRange(New Byte(15) {})

        '連続入場阻止時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続入場阻止時間　定期", 2))

        '連続入場阻止時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続入場阻止時間　ＩＣ", 2))

        '連続出場阻止時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続出場阻止時間　定期", 2))

        '連続出場阻止時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続出場阻止時間　ＩＣ", 2))

        '連続出場阻止時間（他駅出場）　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続出場阻止時間（他駅出場）　定期", 2))

        '連続出場阻止時間（他駅出場）　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続出場阻止時間（他駅出場）　ＩＣ", 2))

        '購入→入場共用時間
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "購入→入場共用時間", 2))

        '入出場許容時間　近距離券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　近距離券", 2))

        '入出場許容時間　回数券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　回数券", 2))

        '入出場許容時間　定期券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　定期券", 2))

        '入出場許容時間　入場券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　入場券", 2))

        '入出場許容時間　発着固定券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　発着固定券", 2))

        '入出場許容時間　ＳＦカード
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　ＳＦカード", 2))

        '入出場許容時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　ＩＣ", 2))

        '入出場同一駅（入場）許容下限時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（入場）許容下限時間　定期", 2))

        '入出場同一駅（入場）許容下限時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（入場）許容下限時間　ＩＣ", 2))

        '入出場同一駅（出場）許容下限時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（出場）許容下限時間　定期", 2))

        '入出場同一駅（出場）許容下限時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（出場）許容下限時間　ＩＣ", 2))

        '入出場同一駅（出場）許容上限時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（出場）許容上限時間　定期", 2))

        '入出場同一駅（出場）許容上限時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（出場）許容上限時間　ＩＣ", 2))

        '同一駅減額判定時間
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "同一駅減額判定時間", 2))

        '期限切れ日数
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "30", "9999", "期限切れ日数", 2))

        '予備
        fileData.AddRange(New Byte(29) {})

        '入場側複数枚処理　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入場側複数枚処理　入／切"))

        '出場側３枚処理　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側３枚処理　入／切"))

        '途中下車印字　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "途中下車印字　入／切"))

        '大型券回数券印字　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "大型券回数券印字　入／切"))

        '金曜昼間特割判定　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "金曜昼間特割判定　入／切"))

        '入出場ｻｲｸﾙ磁気　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場ｻｲｸﾙ磁気　入／切"))

        '入出場ｻｲｸﾙＩＣ　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場ｻｲｸﾙＩＣ　入／切"))

        '不正判定磁気（入出場ｻｲｸﾙ除く）　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "不正判定磁気（入出場ｻｲｸﾙ除く）　入／切"))

        '不正判定ＩＣ（入出場ｻｲｸﾙ除く）　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "不正判定ＩＣ（入出場ｻｲｸﾙ除く）　入／切"))

        '簡易改札不正判定　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "簡易改札不正判定　入／切"))

        '折返乗車入場　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "折返乗車入場　可／否"))

        '折返乗車出場　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "折返乗車出場　可／否"))

        '途中乗車可否　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "途中乗車可否　可／否"))

        '途中下車可否　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "途中下車可否　可／否"))

        '料金付乗車券　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "料金付乗車券　可／否"))

        '料金券判定入場　必須／入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "2", "料金券判定入場　必須／入／切"))

        '料金券判定出場　必須／入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "2", "料金券判定出場　必須／入／切"))

        '予備
        fileData.AddRange(New Byte(20) {})

        '世代２オフセット計算
        Dim s As Short = CShort(fileData.Count)
        Dim offBytes As Byte() = BitConverter.GetBytes(s)

        If s Mod 2 = 1 Then fileData.Add(CType(&H0, Byte))

        '==============世代２==================
        '適用日
        row = row + 1
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(csvData, row, 1), "適用日"))

        '連続入場判定入／切　定期
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続入場判定入／切　定期"))

        '連続入場判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続入場判定入／切　ＩＣ"))

        '連続出場判定入／切　定期
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続出場判定入／切　定期"))

        '連続出場判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続出場判定入／切　ＩＣ"))

        '連続出場判定（他駅出場）入／切　定期
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続出場判定（他駅出場）入／切　定期"))

        '連続出場判定（他駅出場）入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "連続出場判定（他駅出場）入／切　ＩＣ"))

        '入場側サイクル判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入場側サイクル判定入／切　ＩＣ"))

        '出場側サイクル判定入／切　近距離券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　近距離券"))

        '出場側サイクル判定入／切　回数券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　回数券"))

        '出場側サイクル判定入／切　定期券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　定期券"))

        '出場側サイクル判定入／切　入場券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　入場券"))

        '出場側サイクル判定入／切　発着固定券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　発着固定券"))

        '出場側サイクル判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側サイクル判定入／切　ＩＣ"))

        '入場券入出場時間判定入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入場券入出場時間判定入／切"))

        '入出場時間判定入／切　近距離券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　近距離券"))

        '入出場時間判定入／切　回数券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　回数券"))

        '入出場時間判定入／切　定期券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　定期券"))

        '入出場時間判定入／切　入場券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　入場券"))

        '入出場時間判定入／切　発着固定券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　発着固定券"))

        '入出場時間判定入／切　ＳＦカード
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　ＳＦカード"))

        '入出場時間判定入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場時間判定入／切　ＩＣ"))

        '入出場同一駅判定（入場）入／切　定期券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場同一駅判定（入場）入／切　定期券"))

        '入出場同一駅判定（入場）入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場同一駅判定（入場）入／切　ＩＣ"))

        '入出場同一駅判定（出場）入／切　定期券
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場同一駅判定（出場）入／切　定期券"))

        '入出場同一駅判定（出場）入／切　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場同一駅判定（出場）入／切　ＩＣ"))

        '期限切れ定期券判定入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "期限切れ定期券判定入／切"))

        '予備
        fileData.AddRange(New Byte(15) {})

        '連続入場阻止時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続入場阻止時間　定期", 2))

        '連続入場阻止時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続入場阻止時間　ＩＣ", 2))

        '連続出場阻止時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続出場阻止時間　定期", 2))

        '連続出場阻止時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続出場阻止時間　ＩＣ", 2))

        '連続出場阻止時間（他駅出場）　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続出場阻止時間（他駅出場）　定期", 2))

        '連続出場阻止時間（他駅出場）　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続出場阻止時間（他駅出場）　ＩＣ", 2))

        '購入→入場共用時間
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "購入→入場共用時間", 2))

        '入出場許容時間　近距離券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　近距離券", 2))

        '入出場許容時間　回数券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　回数券", 2))

        '入出場許容時間　定期券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　定期券", 2))

        '入出場許容時間　入場券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　入場券", 2))

        '入出場許容時間　発着固定券
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　発着固定券", 2))

        '入出場許容時間　ＳＦカード
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　ＳＦカード", 2))

        '入出場許容時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場許容時間　ＩＣ", 2))

        '入出場同一駅（入場）許容下限時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（入場）許容下限時間　定期", 2))

        '入出場同一駅（入場）許容下限時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（入場）許容下限時間　ＩＣ", 2))

        '入出場同一駅（出場）許容下限時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（出場）許容下限時間　定期", 2))

        '入出場同一駅（出場）許容下限時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（出場）許容下限時間　ＩＣ", 2))

        '入出場同一駅（出場）許容上限時間　定期
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（出場）許容上限時間　定期", 2))

        '入出場同一駅（出場）許容上限時間　ＩＣ
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場同一駅（出場）許容上限時間　ＩＣ", 2))

        '同一駅減額判定時間
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "同一駅減額判定時間", 2))

        '期限切れ日数
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "30", "9999", "期限切れ日数", 2))

        '予備
        fileData.AddRange(New Byte(29) {})

        '入場側複数枚処理　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入場側複数枚処理　入／切"))

        '出場側３枚処理　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "出場側３枚処理　入／切"))

        '途中下車印字　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "途中下車印字　入／切"))

        '大型券回数券印字　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "大型券回数券印字　入／切"))

        '金曜昼間特割判定　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "金曜昼間特割判定　入／切"))

        '入出場ｻｲｸﾙ磁気　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場ｻｲｸﾙ磁気　入／切"))

        '入出場ｻｲｸﾙＩＣ　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場ｻｲｸﾙＩＣ　入／切"))

        '不正判定磁気（入出場ｻｲｸﾙ除く）　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "不正判定磁気（入出場ｻｲｸﾙ除く）　入／切"))

        '不正判定ＩＣ（入出場ｻｲｸﾙ除く）　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "不正判定ＩＣ（入出場ｻｲｸﾙ除く）　入／切"))

        '簡易改札不正判定　入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "簡易改札不正判定　入／切"))

        '折返乗車入場　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "折返乗車入場　可／否"))

        '折返乗車出場　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "折返乗車出場　可／否"))

        '途中乗車可否　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "途中乗車可否　可／否"))

        '途中下車可否　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "途中下車可否　可／否"))

        '料金付乗車券　可／否
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "料金付乗車券　可／否"))

        '料金券判定入場　必須／入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "2", "料金券判定入場　必須／入／切"))

        '料金券判定出場　必須／入／切
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "2", "料金券判定出場　必須／入／切"))

        '予備
        fileData.AddRange(New Byte(20) {})

        Dim ret As Byte() = CType(fileData.ToArray(GetType(Byte)), Byte())

        '世代２オフセットを設定
        ret(6) = offBytes(1)
        ret(7) = offBytes(0)

        Dim sumValue As Long = 0
        'マスタバージョンを除く
        For i As Integer = 1 To ret.Length - 1
            sumValue += ret(i)
            sumValue = sumValue And &HFFFFFFFF
        Next

        Dim sum As Byte() = BitConverter.GetBytes(CUInt(sumValue))
        ret(1) = sum(3)
        ret(2) = sum(2)
        ret(3) = sum(1)
        ret(4) = sum(0)

        fileData.Clear()

        'パラメータ識別
        fileData.Add(CType(&H3E, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange(temp)

        fileData.AddRange(ret)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function


    ''' <summary>
    ''' ＪＲ西日本エリア ＩＣ発行機関マスタ変換
    ''' </summary>
    Private Function MakePR_IJW() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'データ部を追加
        Dim i, row As Integer

        Dim temp As Byte()

        '世代１
        '適用日
        row = row + 1
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(csvData, row, 1), "適用日"))
        temp = Common.GetApplyDate(Common.ReadStringFromCSV(csvData, row, 1), "適用日")

        For i = 1 To 100
            row = row + 1

            'エリアコード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "99", "エリアコード", 1))

            '機関コード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "機関コード", 2))

            '共通コード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "共通コード", 1))

            '発行機関名
            fileData.AddRange(Common.GetBytesKikan(Common.ReadStringFromCSV(csvData, row, 4), "発行機関名"))

            '予備
            fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

            '券種１
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "99999999", "券種１", 4))

            '券種２
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 7), "0", "99999999", "券種２", 4))

            '券種３
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 8), "0", "99999999", "券種３", 4))

            '券種４
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 9), "0", "99999999", "券種４", 4))

            '券種５
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 10), "0", "99999999", "券種５", 4))

            '券種６
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 11), "0", "99999999", "券種６", 4))

            '券種７
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 12), "0", "99999999", "券種７", 4))

            '券種８
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 13), "0", "99999999", "券種８", 4))

            '券種９
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 14), "0", "99999999", "券種９", 4))

            '券種１０
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 15), "0", "99999999", "券種１０", 4))

            '券種１１
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 16), "0", "99999999", "券種１１", 4))

            '券種１２
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 17), "0", "99999999", "券種１２", 4))

            '券種１３
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 18), "0", "99999999", "券種１３", 4))

            '券種１４
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 19), "0", "99999999", "券種１４", 4))

            '券種１５
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 20), "0", "99999999", "券種１５", 4))

            '券種１６
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 21), "0", "99999999", "券種１６", 4))

        Next

        '世代２
        '適用日
        row = row + 1
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(csvData, row, 1), "適用日"))

        For i = 1 To 100
            row = row + 1

            'エリアコード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "99", "エリアコード", 1))

            '機関コード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "機関コード", 2))

            '共通コード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "共通コード", 1))

            '発行機関名
            fileData.AddRange(Common.GetBytesKikan(Common.ReadStringFromCSV(csvData, row, 4), "発行機関名"))

            '予備
            fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

            '券種１
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "99999999", "券種１", 4))

            '券種２
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 7), "0", "99999999", "券種２", 4))

            '券種３
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 8), "0", "99999999", "券種３", 4))

            '券種４
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 9), "0", "99999999", "券種４", 4))

            '券種５
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 10), "0", "99999999", "券種５", 4))

            '券種６
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 11), "0", "99999999", "券種６", 4))

            '券種７
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 12), "0", "99999999", "券種７", 4))

            '券種８
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 13), "0", "99999999", "券種８", 4))

            '券種９
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 14), "0", "99999999", "券種９", 4))

            '券種１０
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 15), "0", "99999999", "券種１０", 4))

            '券種１１
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 16), "0", "99999999", "券種１１", 4))

            '券種１２
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 17), "0", "99999999", "券種１２", 4))

            '券種１３
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 18), "0", "99999999", "券種１３", 4))

            '券種１４
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 19), "0", "99999999", "券種１４", 4))

            '券種１５
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 20), "0", "99999999", "券種１５", 4))

            '券種１６
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 21), "0", "99999999", "券種１６", 4))

        Next

        Dim ret As Byte() = CType(fileData.ToArray(GetType(Byte)), Byte())
        Dim sumValue As Long = 0

        For i = 0 To ret.Length - 1
            sumValue += ret(i)
            sumValue = sumValue And &HFFFFFFFF
        Next


        fileData.Clear()

        'パラメータ識別
        fileData.Add(CType(&H43, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange(temp)

        'マスタバージョン
        fileData.AddRange(Common.GetVersion(Common.ReadStringFromCSV(csvData, 0, 0)))

        'データサム値
        Dim sum As Byte() = BitConverter.GetBytes(CUInt(sumValue))
        fileData.Add(sum(3))
        fileData.Add(sum(2))
        fileData.Add(sum(1))
        fileData.Add(sum(0))

        fileData.AddRange(ret)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    ''' <summary>
    '''  ＪＲ東海エリア　ＩＣ不正マスタ変換
    ''' </summary>
    Private Function MakePR_FJC() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H4E, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim row As Integer

        '標準設定
        fileData.Add(CType(&H0, Byte))
        '予備
        fileData.Add(CType(&H0, Byte))
        '駅指定判定
        row = row + 1
        fileData.Add(CType(&H0, Byte))
        '予備
        fileData.Add(CType(&H0, Byte))
        '不正判定（自社ﾗｯﾁ連絡）
        row = row + 1
        fileData.Add(CType(&H0, Byte))
        '不正判定（自社ﾉｰﾗｯﾁ連絡）
        row = row + 1
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '不正判定（他社１ﾗｯﾁ連絡）
        row = row + 1
        fileData.Add(CType(&H0, Byte))
        '不正判定（他社１ﾉｰﾗｯﾁ連絡）
        row = row + 1
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '不正判定（他社２ﾗｯﾁ連絡）
        row = row + 1
        fileData.Add(CType(&H0, Byte))
        '不正判定（他社２ﾉｰﾗｯﾁ連絡）
        row = row + 1
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '自社（他社１／他社２）駅ｺｰﾄﾞ
        row = row + 1
        fileData.AddRange((New Byte() {&H0, &H0}))
        '連絡する電鉄ｺｰﾄﾞ
        row = row + 1
        fileData.Add(CType(&H0, Byte))
        '予備
        row = row + 1
        fileData.Add(CType(&H0, Byte))

        row = row + 1

        '入出場ｻｲｸﾙ異常回数 普通券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "1", "255", "入出場ｻｲｸﾙ異常回数 普通券"))

        '入出場ｻｲｸﾙ異常回数 入場券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "1", "255", "入出場ｻｲｸﾙ異常回数 入場券"))

        '入出場ｻｲｸﾙ異常回数 回数券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "1", "255", "入出場ｻｲｸﾙ異常回数 回数券"))
        fileData.Add(CType(&H0, Byte))

        '入出場ｻｲｸﾙ異常回数 ＩＣカード
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "1", "255", "入出場ｻｲｸﾙ異常回数 ＩＣカード"))
        fileData.Add(CType(&H0, Byte))
        fileData.Add(CType(&H0, Byte))

        '入出場ｻｲｸﾙ異常回数 定期券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "1", "255", "入出場ｻｲｸﾙ異常回数 定期券"))

        row = row + 1

        '入出場時間異常回数 普通券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "1", "255", "入出場時間異常回数 普通券"))

        '入出場時間異常回数 入場券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "1", "255", "入出場時間異常回数 入場券"))

        '入出場時間異常回数 回数券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "1", "255", "入出場時間異常回数 回数券"))
        fileData.Add(CType(&H0, Byte))

        '入出場時間異常回数 ＩＣカード
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "1", "255", "入出場時間異常回数 ＩＣカード"))
        fileData.Add(CType(&H0, Byte))
        fileData.Add(CType(&H0, Byte))

        '入出場時間異常回数 定期券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "1", "255", "入出場時間異常回数 定期券"))

        row = row + 1

        '同一駅入出場異常回数 普通券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "1", "255", "同一駅入出場異常回数 普通券"))

        '同一駅入出場異常回数 入場券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "1", "255", "同一駅入出場異常回数 入場券"))

        '同一駅入出場異常回数 回数券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "1", "255", "同一駅入出場異常回数 回数券"))
        fileData.Add(CType(&H0, Byte))

        '同一駅入出場異常回数 ＩＣカード
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "1", "255", "同一駅入出場異常回数 ＩＣカード"))
        fileData.Add(CType(&H0, Byte))
        fileData.Add(CType(&H0, Byte))

        '同一駅入出場異常回数 定期券
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "1", "255", "同一駅入出場異常回数 定期券"))

        row = row + 1

        '入出場時間異常許容時間　普通券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "入出場時間異常許容時間 普通券", 4))

        '入出場時間異常許容時間　入場券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "9999", "入出場時間異常許容時間 入場券", 4))

        '入出場時間異常許容時間　回数券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "9999", "入出場時間異常許容時間 回数券", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '入出場時間異常許容時間　ＩＣカード
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "9999", "入出場時間異常許容時間 ＩＣカード", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '入出場時間異常許容時間　定期券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "9999", "入出場時間異常許容時間 定期券", 4))

        row = row + 1

        '同一駅入出場時間異常許容時間　普通券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "同一駅入出場時間異常許容時間 普通券", 4))

        '同一駅入出場時間異常許容時間　入場券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "9999", "同一駅入出場時間異常許容時間 入場券", 4))

        '同一駅入出場時間異常許容時間　回数券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "9999", "同一駅入出場時間異常許容時間 回数券", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '同一駅入出場時間異常許容時間　ＩＣカード
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "9999", "同一駅入出場時間異常許容時間 ＩＣカード", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '同一駅入出場時間異常許容時間　定期券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "9999", "同一駅入出場時間異常許容時間 定期券", 4))

        row = row + 1

        '再投入異常時間　普通券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "再投入異常時間 普通券", 4))

        '再投入異常時間　入場券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "9999", "再投入異常時間 入場券", 4))

        '再投入異常時間　回数券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "9999", "再投入異常時間 回数券", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '再投入異常時間　ＩＣカード
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "9999", "再投入異常時間 ＩＣカード", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '再投入異常時間　定期券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "9999", "再投入異常時間 定期券", 4))

        row = row + 1

        '連続精算異常時間　普通券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "連続精算異常時間 普通券", 4))

        '連続精算異常時間　入場券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "9999", "連続精算異常時間 入場券", 4))

        '連続精算異常時間　回数券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "9999", "連続精算異常時間 回数券", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '連続精算異常時間　ＩＣカード
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "9999", "連続精算異常時間 ＩＣカード", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '連続精算異常時間　定期券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "9999", "連続精算異常時間 定期券", 4))

        row = row + 1

        '２ﾗｯﾁ連絡入出場許容時間　普通券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "9999", "２ﾗｯﾁ連絡入出場許容時間 普通券", 4))

        '２ﾗｯﾁ連絡入出場許容時間　入場券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "9999", "２ﾗｯﾁ連絡入出場許容時間 入場券", 4))

        '２ﾗｯﾁ連絡入出場許容時間　回数券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "9999", "２ﾗｯﾁ連絡入出場許容時間 回数券", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '２ﾗｯﾁ連絡入出場許容時間　ＩＣカード
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "9999", "２ﾗｯﾁ連絡入出場許容時間 ＩＣカード", 4))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        '２ﾗｯﾁ連絡入出場許容時間　定期券
        fileData.AddRange(Common.GetDECBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "9999", "２ﾗｯﾁ連絡入出場許容時間 定期券", 4))

        row = row + 1
        '入出場ｻｲｸﾙ異常ＳＷ　普通券
        Dim b1 As Byte = Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "1", "入出場ｻｲｸﾙ異常ＳＷ　普通券")(0)

        '入出場ｻｲｸﾙ異常ＳＷ　入場券
        Dim b2 As Byte = Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "1", "入出場ｻｲｸﾙ異常ＳＷ　入場券")(0)

        '入出場ｻｲｸﾙ異常ＳＷ　回数券
        Dim b3 As Byte = Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "1", "入出場ｻｲｸﾙ異常ＳＷ　回数券")(0)

        '入出場ｻｲｸﾙ異常ＳＷ　定期券
        Dim b8 As Byte = Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "1", "入出場ｻｲｸﾙ異常ＳＷ　定期券")(0)

        Dim b As Byte = b1 Or b2 << 1 Or b3 << 2 Or b8 << 7
        fileData.Add(b)

        '入出場時間異常ＳＷ
        fileData.Add(CType(&H0, Byte))
        '同一駅入出場異常ＳＷ
        fileData.Add(CType(&H0, Byte))
        '予備
        fileData.Add(CType(&H0, Byte))
        '２ﾗｯﾁ連絡許容時間ＳＷ
        fileData.Add(CType(&H0, Byte))
        '予備
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))

        '自動化範囲設定
        '範囲１
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲２
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲３
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲４
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲５
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲６
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲７
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲８
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲９
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '範囲１０
        fileData.AddRange((New Byte() {&H0, &H0, &H0}))
        '予備
        fileData.AddRange((New Byte() {&H0, &H0}))

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    'Ver0.1 ADD START  BINファイル取込対応
    ''' <summary>
    ''' ＪＲ東海エリア　ＩＣ不正マスタ変換（BIN→BIN）
    ''' </summary>
    Private Function MakePR_FJC2() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H4E, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function
    'Ver0.1 ADD END

    ''' <summary>
    ''' ＪＲ東海エリア ＩＣ運用マスタ変換
    ''' </summary>
    Private Function MakePR_IJC() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H4F, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim i, row As Integer

        '世代１
        'マスタバージョン
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "1", "255", "マスタバージョン", 2))

        '有効開始年月日時分
        fileData.AddRange(Common.GetApplyDateTimeBCD(Common.ReadStringFromCSV(csvData, row, 2), "有効開始年月日時分"))

        '予備
        fileData.AddRange((New Byte(7) {}))

        '一次発行事業者
        For i = 1 To 10
            row = row + 1

            '地域コード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "99", "地域コード", 1))

            'ユーザコード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "ユーザコード", 2))

            '予備
            fileData.AddRange((New Byte(4) {}))
        Next

        '活性事業者
        For i = 1 To 50
            row = row + 1

            '地域コード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "99", "地域コード", 1))

            'ユーザコード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "ユーザコード", 2))

            '集計グループ指定
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "集計グループ指定", 1))

            '制御項目
            Try
                fileData.Add(Convert.ToByte(Common.ReadStringFromCSV(csvData, row, 4), 16))
            Catch ex As Exception
                AlertBox.Show(Lexis.ERR_COMMON, "制御項目")
                Throw
            End Try

            'パース上限金額
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "999999", "パース上限金額", 3))

            'カード最大金額
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "999999", "カード最大金額", 3))

            '予備
            fileData.AddRange((New Byte(4) {}))
        Next

        '世代２
        row = row + 1

        'マスタバージョン
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "1", "255", "マスタバージョン", 2))

        '有効開始年月日時分
        fileData.AddRange(Common.GetApplyDateTimeBCD(Common.ReadStringFromCSV(csvData, row, 2), "有効開始年月日時分"))

        '予備
        fileData.AddRange((New Byte(7) {}))

        '一次発行事業者
        For i = 1 To 10
            row = row + 1

            '地域コード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "99", "地域コード", 1))

            'ユーザコード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "ユーザコード", 2))

            '予備
            fileData.AddRange((New Byte(4) {}))
        Next

        '活性事業者
        For i = 1 To 50
            row = row + 1

            '地域コード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "99", "地域コード", 1))

            'ユーザコード
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "ユーザコード", 2))

            '集計グループ指定
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "集計グループ指定", 1))

            '制御項目
            Try
                fileData.Add(Convert.ToByte(Common.ReadStringFromCSV(csvData, row, 4), 16))
            Catch ex As Exception
                AlertBox.Show(Lexis.ERR_COMMON, "制御項目")
                Throw
            End Try

            'パース上限金額
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "999999", "パース上限金額", 3))

            'カード最大金額
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "999999", "カード最大金額", 3))

            '予備
            fileData.AddRange((New Byte(4) {}))
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    'Ver0.1 ADD START  BINファイル取込対応
    ''' <summary>
    ''' ＪＲ東海エリア　ＩＣ運用マスタ変換（BIN→BIN）
    ''' </summary>
    Private Function MakePR_IJC2() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H4F, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function
    'Ver0.1 ADD END

    ''' <summary>
    ''' 判定パラメータ変換
    ''' </summary>
    Private Function MakePR_DSH() As Byte()
        'Ver0.1 ADD　判定ﾊﾟﾗﾒｰﾀ(東海用)追加対応
        Dim flg As String = CType(cmbMaster.SelectedItem, DataRowView).Item(2).ToString

        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H47, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim i, row As Integer

        '予備
        fileData.AddRange((New Byte(15) {}))

        '不正判定対象駅
        For i = 1 To 40
            row = row + 1

            '線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "不正判定対象駅線区"))

            '駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "不正判定対象駅駅順"))

            '判定有無
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "1", "不正判定対象駅判定有無"))

            '予備
            fileData.Add(CType(&H0, Byte))

        Next

        '同一駅入出場長時間禁止時間
        row = row + 1
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "同一駅入出場長時間禁止時間"))

        '複数回利用禁止時間
        row = row + 1
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "複数回利用禁止時間"))

        '同一駅入出場短時間禁止時間
        row = row + 1
        'Ver0.1 MOD STRAT 判定ﾊﾟﾗﾒｰﾀ(東海用)追加対応
        'fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "同一駅入出場短時間禁止時間"))
        If flg = "0" Then
            fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "同一駅入出場短時間禁止時間"))
        Else
            fileData.AddRange((New Byte() {&H0, &H0}))
        End If
        'Ver0.1 MOD END

        '同一駅出入場短時間禁止時間
        row = row + 1
        'Ver0.1 MOD STRAT 判定ﾊﾟﾗﾒｰﾀ(東海用)追加対応
        'fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "同一駅出入場短時間禁止時間"))
        If flg = "0" Then
            fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "同一駅出入場短時間禁止時間"))
        Else
            fileData.AddRange((New Byte() {&H0, &H0}))
        End If
        'Ver0.1 MOD END

        '不正判定対象項目
        For i = 1 To 32
            row = row + 1
            'Ver0.1 MOD STRAT 判定ﾊﾟﾗﾒｰﾀ(東海用)追加対応
            'fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "1", "不正判定対象項目"))
            If flg = "0" Then
                fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "1", "不正判定対象項目"))
            Else
                Select Case i
                    Case 6, 7, 11
                        fileData.Add(CType(&H0, Byte))
                    Case 10, 12, 13, 14, 15, 16, 17, 18
                        fileData.Add(CType(&H1, Byte))
                    Case Else
                        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "1", "不正判定対象項目"))
                End Select
            End If
            'Ver0.1 MOD END
        Next

        '最終利用日付設定
        row = row + 1
        'Ver0.1 MOD STRAT 判定ﾊﾟﾗﾒｰﾀ(東海用)追加対応
        'fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "63", "最終利用日付設定"))
        If flg = "0" Then
            fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "63", "最終利用日付設定"))
        Else
            fileData.AddRange((New Byte() {&H0, &H0}))
        End If
        'Ver0.1 MOD END

        '改札機状態情報送信間隔
        row = row + 1
        'Ver0.1 MOD STRAT 判定ﾊﾟﾗﾒｰﾀ(東海用)追加対応
        'fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "改札機状態情報送信間隔"))
        If flg = "0" Then
            fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "改札機状態情報送信間隔"))
        Else
            fileData.AddRange((New Byte() {&H0, &H1}))
        End If
        'Ver0.1 MOD END

        '最終利用日付設定（在来）
        row = row + 1
        'Ver0.1 MOD STRAT 判定ﾊﾟﾗﾒｰﾀ(東海用)追加対応
        'fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "63", "最終利用日付設定（在来）"))
        If flg = "0" Then
            fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "63", "最終利用日付設定（在来）"))
        Else
            fileData.AddRange((New Byte() {&H0, &H0}))
        End If
        'Ver0.1 MOD END

        '不正判定対象駅
        For i = 41 To 60
            row = row + 1

            '線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "不正判定対象駅線区"))

            '駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "不正判定対象駅駅順"))

            '判定有無
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "1", "不正判定対象駅判定有無"))

            '予備
            fileData.Add(CType(&H0, Byte))
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    ''' <summary>
    '''終列車時刻データ変換
    ''' </summary>
    Private Function MakePR_LST() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H4D, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim i, row As Integer

        '判定フラグ
        row = row + 1
        fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 0), "0", "1", "判定フラグ"))

        '予備
        fileData.Add(CType(&H0, Byte))

        '猶予時間
        Try
            Dim r As New System.Text.RegularExpressions.Regex("^[-]?[0-9]+$")
            If r.IsMatch(Common.ReadStringFromCSV(csvData, row, 2)) = False Then
                Throw New Exception
            End If

            Dim time As Short = Short.Parse(Common.ReadStringFromCSV(csvData, row, 2))
            If time < -10 Or time > 30 Then
                Throw New Exception
            End If

            fileData.AddRange(BitConverter.GetBytes(time))
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, "猶予時間")
            Throw
        End Try

        '終列車時刻
        For i = 1 To 60
            row = row + 1
            Console.WriteLine(i)
            '線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "終列車時刻線区"))

            '駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "終列車時刻駅順"))

            'のぞみ時
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "99", "のぞみ時", 1))

            'のぞみ分
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "99", "のぞみ分", 1))

            'ひかり/こだま時
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "99", "ひかり/こだま時", 1))

            'ひかり/こだま分
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 6), "0", "99", "ひかり/こだま分", 1))

            '予備
            fileData.AddRange((New Byte() {&H0, &H0}))
        Next

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    ''' <summary>
    ''' ＪＲ東日本エリア　ＩＣ運用マスタ変換
    ''' </summary>
    Private Function MakePR_IJE() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H56, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    ''' <summary>
    ''' 改札機動作マスタ変換
    ''' </summary>
    Private Function MakePR_DUS() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H66, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'リセットスイッチ受付有効タイマ
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, 1, 1), "0", "99", "リセットスイッチ受付有効タイマ", 2))

        'リセットスイッチ押下有効タイマ
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, 2, 1), "0", "99", "リセットスイッチ押下有効タイマ", 2))

        '予備
        fileData.AddRange((New Byte(250) {}))

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    ''' <summary>
    ''' ＪＲ東日本エリア　ＩＣ不正マスタ変換
    ''' </summary>
    Private Function MakePR_FJR() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H50, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function


    ''' <summary>
    ''' サイクル判定救済マスタ変換
    ''' </summary>
    Private Function MakePR_CYC() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H64, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 券通しマスタ変換
    ''' </summary>
    Private Function MakePR_STP() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H63, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function


    ''' <summary>
    ''' 商品・割引コードデータ変換
    ''' </summary>
    Private Function MakePR_PNO() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H62, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function


    ''' <summary>
    ''' フリーコードデータ変換
    ''' </summary>
    Private Function MakePR_FRC() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H61, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 駅名データ（ＪＲ東海 Ｓｕｉｃａ）変換
    ''' </summary>
    Private Function MakePR_NSI() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H70, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 駅名データ（ＪＲ東海 TOICA）変換
    ''' </summary>
    Private Function MakePR_NTO() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H71, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function


    ''' <summary>
    ''' 駅名データ（ＪＲ東海 ＩＣＯＣＡ）変換
    ''' </summary>
    Private Function MakePR_NIC() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H72, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 駅名データ（ＪＲ西日本ＩＣＯＣＡ）変換
    ''' </summary>
    Private Function MakePR_NJW() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H73, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    'Ver0.2 ADD START  北陸対応
    ''' <summary>
    ''' 北陸(JRE)新幹線ＩＣカード運用マスタ変換（BIN→BIN）
    ''' </summary>
    Private Function MakePR_IUK() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H86, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 北陸(JRE)改札機過渡期制限マスタ変換（BIN→BIN）
    ''' </summary>
    Private Function MakePR_KSZ() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H85, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 北陸(JRE)新幹線改札機スイッチマスタ変換（BIN→BIN）
    ''' </summary>
    Private Function MakePR_SWK() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H87, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 北陸(JRE)ＩＣカード運用マスタ変換（BIN→BIN）
    ''' </summary>
    Private Function MakePR_IUZ() As Byte()
        'BINファイルを読み込み
        Dim binData As Byte() = Common.ReadBin(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H84, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        fileData.AddRange(binData)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())
    End Function

    ''' <summary>
    ''' 北陸エリア　新幹線不正パラメータ変換（CSV→BIN）
    ''' </summary>
    Private Function MakePR_FSK() As Byte()
        Dim flg As String = CType(cmbMaster.SelectedItem, DataRowView).Item(2).ToString

        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsv(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        'パラメータ識別
        fileData.Add(CType(&H80, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange((New Byte() {&H0, &H0, &H0, &H0}))

        'データ部を追加
        Dim i, row As Integer

        'マスタバージョン
        row = 0

        'パラメータ識別,フォーマット識別番号,適用日付,予備
        row = row + 1

        '不正判定対象駅
        For i = 1 To 70
            row = row + 1

            '線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "不正判定対象駅線区"))

            '駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "不正判定対象駅駅順"))

            '判定有無
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "1", "不正判定対象駅判定有無"))

        Next

        '同一駅入出場時間
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "同一駅入出場時間", 2))

        '同一駅短時間入出場禁止時間
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "同一駅短時間入出場禁止時間", 2))

        '同一駅短時間入出場禁止時間
        row = row + 1
        fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "同一駅短時間入出場禁止時間", 2))

        '予備
        row = row + 1
        fileData.AddRange((New Byte(1) {}))


        '磁気券入出場時間チェック
        For i = 1 To 3
            row = row + 1

            '下限線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "磁気券入出場時間下限線区"))

            '下限駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "磁気券入出場時間下限駅順"))

            '上限線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "255", "磁気券入出場時間上限線区"))

            '上限駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "255", "磁気券入出場時間上限駅順"))

            '許容時間
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "9999", "磁気券入出場時間許容時間", 2))

        Next

        '不正判定対象項目
        For i = 1 To 32
            row = row + 1

            '判定有無
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "1", "不正判定対象項目判定有無"))

        Next

        '改札機状態情報送信間隔
        row = row + 1
        fileData.AddRange(Common.GetBytes2BetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "120", "改札機状態情報送信間隔"))

        '予備
        row = row + 1
        fileData.AddRange((New Byte(15) {}))

        'ＩＣ入出場許容時間
        For i = 1 To 3
            row = row + 1

            '下限線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "ＩＣ入出場許容時間下限線区"))

            '下限駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "ＩＣ入出場許容時間下限駅順"))

            '上限線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "255", "ＩＣ入出場許容時間上限線区"))

            '上限駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 4), "0", "255", "ＩＣ入出場許容時間上限駅順"))

            '許容時間
            fileData.AddRange(Common.GetBCDBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 5), "0", "9999", "ＩＣ入出場許容時間許容時間", 2))

        Next

        '予備
        row = row + 1
        fileData.AddRange((New Byte(17) {}))

        '入場フリー券種
        For i = 1 To 9
            row = row + 1

            '判定有無（0固定で設定する）
            fileData.AddRange(Common.GetBytesBetweenAnd("0", "0", "1", "入場フリー券種判定有無"))

        Next

        '出場フリー券種
        For i = 1 To 9
            row = row + 1

            '判定有無（0固定で設定する）
            fileData.AddRange(Common.GetBytesBetweenAnd("0", "0", "1", "出場フリー券種判定有無"))

        Next

        '入場フリー年月日時
        For i = 1 To 9
            row = row + 1

            '判定有無（0固定で設定する）
            fileData.AddRange(Common.GetBCDBytesBetweenAnd("0", "0", "99999999", "入場フリー年月日時開始", 4))

            '判定有無（0固定で設定する）
            fileData.AddRange(Common.GetBCDBytesBetweenAnd("0", "0", "99999999", "入場フリー年月日時終了", 4))

        Next

        '出場フリー年月日時
        For i = 1 To 9
            row = row + 1

            '判定有無（0固定で設定する）
            fileData.AddRange(Common.GetBCDBytesBetweenAnd("0", "0", "99999999", "出場フリー年月日時開始", 4))

            '判定有無（0固定で設定する）
            fileData.AddRange(Common.GetBCDBytesBetweenAnd("0", "0", "99999999", "出場フリー年月日時終了", 4))

        Next

        '予備
        row = row + 1
        fileData.AddRange((New Byte(185) {}))

        '磁気券回収中止駅別設定対象駅
        For i = 1 To 70
            row = row + 1

            '線区
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 1), "0", "255", "磁気券回収中止駅別設定対象駅線区"))

            '駅順
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 2), "0", "255", "磁気券回収中止駅別設定対象駅駅順"))

            '判定有無
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(csvData, row, 3), "0", "1", "磁気券回収中止駅別設定対象駅判定有無"))

        Next

        '予備
        row = row + 1
        fileData.AddRange((New Byte(11) {}))

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function
    'Ver0.2 ADD END    北陸対応

    'Ver0.4 ADD START  ポイントポストペイ対応
    ''' <summary>
    ''' 昼特区間・時間マスタ変換（CSV→BIN）
    ''' </summary>
    Private Function MakePR_HIR() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsvJRW(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        '適用日
        Dim temp As Byte()

        'CSVフォーマットの補正
        If csvData.Count < 6 Then
            For i As Integer = csvData.Count To 5
                csvData.Add(New ArrayList())
            Next
        End If

        'マスタバージョン
        fileData.AddRange(Common.GetVersion(Common.ReadStringFromCSV(CType(csvData.Item(1), ArrayList), 0, 0)))

        'データサム値――後で計算する
        fileData.AddRange(New Byte(3) {})

        '予備
        fileData.Add(CType(&H0, Byte))

        '世代２オフセット――後で計算する
        fileData.AddRange(New Byte(1) {})

        '==============世代１==================
        '適用日
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(CType(csvData.Item(2), ArrayList), 0, 1), "世代１適用日"))
        temp = Common.GetApplyDate(Common.ReadStringFromCSV(CType(csvData.Item(2), ArrayList), 0, 1), "世代１適用日")

        '#発駅線区,#発駅駅順,#着駅線区,#着駅駅順,#対象時間帯,#プラン識別コード
        For i As Integer = 0 To CType(csvData.Item(3), ArrayList).Count - 1

            '発駅・線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 0), "0", "255", "世代１区間　発駅・線区コード"))

            '発駅・駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 1), "0", "255", "世代１区間　発駅・駅順コード"))

            '着駅・線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 2), "0", "255", "世代１区間　着駅・線区コード"))

            '着駅・駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 3), "0", "255", "世代１区間　着駅・駅順コード"))

            '対象時間帯(FROM～TO)
            Dim r1 As New System.Text.RegularExpressions.Regex("^[0-9]{8}$")
            If r1.IsMatch(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 4)) = False Then
                AlertBox.Show(Lexis.ERR_COMMON, "世代１区間　対象時間帯(FROM～TO)")
                Throw New Exception
            End If
            fileData.AddRange(Utility.CHARtoBCD(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 4), 4))

            'プラン識別コード
            Dim r2 As New System.Text.RegularExpressions.Regex("^[0-9]{6}$")
            If r2.IsMatch(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 5)) = False Then
                AlertBox.Show(Lexis.ERR_COMMON, "世代１区間　プラン識別コード")
                Throw New Exception
            End If
            fileData.AddRange(Utility.CHARtoBCD(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 5), 3))

            '予備
            fileData.Add(CType(&H0, Byte))

        Next

        'エンドコード
        fileData.Add(CType(&HFF, Byte))
        fileData.Add(CType(&HFF, Byte))

        '偶数アドレスになるため
        If fileData.Count Mod 2 = 1 Then fileData.Add(CType(&H0, Byte))

        '世代２オフセット計算
        Dim s As UShort = CUShort(fileData.Count)
        Dim offBytes As Byte() = BitConverter.GetBytes(s)

        '==============世代２==================
        '適用日
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(CType(csvData.Item(4), ArrayList), 0, 1), "世代２適用日"))

        '#発駅線区,#発駅駅順,#着駅線区,#着駅駅順,#対象時間帯,#プラン識別コード
        For i As Integer = 0 To CType(csvData.Item(5), ArrayList).Count - 1

            '発駅・線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 0), "0", "255", "世代２区間　発駅・線区コード"))

            '発駅・駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 1), "0", "255", "世代２区間　発駅・駅順コード"))

            '着駅・線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 2), "0", "255", "世代２区間　着駅・線区コード"))

            '着駅・駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 3), "0", "255", "世代２区間　着駅・駅順コード"))

            '対象時間帯(FROM～TO)
            Dim r1 As New System.Text.RegularExpressions.Regex("^[0-9]{8}$")
            If r1.IsMatch(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 4)) = False Then
                AlertBox.Show(Lexis.ERR_COMMON, "世代２区間　対象時間帯(FROM～TO)")
                Throw New Exception
            End If
            fileData.AddRange(Utility.CHARtoBCD(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 4), 4))

            'プラン識別コード
            Dim r2 As New System.Text.RegularExpressions.Regex("^[0-9]{6}$")
            If r2.IsMatch(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 5)) = False Then
                AlertBox.Show(Lexis.ERR_COMMON, "世代２区間　プラン識別コード")
                Throw New Exception
            End If
            fileData.AddRange(Utility.CHARtoBCD(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 5), 3))

            '予備
            fileData.Add(CType(&H0, Byte))

        Next

        'エンドコード
        fileData.Add(CType(&HFF, Byte))
        fileData.Add(CType(&HFF, Byte))

        Dim ret As Byte() = CType(fileData.ToArray(GetType(Byte)), Byte())

        '世代２オフセットを設定
        ret(6) = offBytes(1)
        ret(7) = offBytes(0)

        Dim sumValue As Long = 0
        'マスタバージョンを除く
        For i As Integer = 1 To ret.Length - 1
            sumValue += ret(i)
            sumValue = sumValue And &HFFFFFFFF
        Next

        Dim sum As Byte() = BitConverter.GetBytes(CUInt(sumValue))
        ret(1) = sum(3)
        ret(2) = sum(2)
        ret(3) = sum(1)
        ret(4) = sum(0)

        fileData.Clear()

        'パラメータ識別
        fileData.Add(CType(&H8A, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange(temp)

        fileData.AddRange(ret)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function

    ''' <summary>
    ''' ポストペイエリアマスタ変換（CSV→BIN）
    ''' </summary>
    Private Function MakePR_PPA() As Byte()
        'CSVファイルを読み込み
        Dim csvData As ArrayList = Common.ReadCsvJRW(Me.txtFileName.Text)

        Dim fileData As ArrayList = New ArrayList()

        '適用日
        Dim temp As Byte()

        'CSVフォーマットの補正
        If csvData.Count < 12 Then
            For i As Integer = csvData.Count To 11
                csvData.Add(New ArrayList())
            Next
        End If

        'マスタバージョン
        fileData.AddRange(Common.GetVersion(Common.ReadStringFromCSV(CType(csvData.Item(1), ArrayList), 0, 0)))

        'データサム値――後で計算する
        fileData.AddRange(New Byte(3) {})

        '予備
        fileData.Add(CType(&H0, Byte))

        '世代２オフセット――後で計算する
        fileData.AddRange(New Byte(1) {})

        '==============世代１==================
        '適用日
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(CType(csvData.Item(2), ArrayList), 0, 1), "世代１適用日"))
        temp = Common.GetApplyDate(Common.ReadStringFromCSV(CType(csvData.Item(2), ArrayList), 0, 1), "世代１適用日")

        '線区テーブル(線区インデックス)
        Dim idxTable As ArrayList = New ArrayList()

        'オフセット計算用
        Dim intVal As Integer = 0

        '#線区インデックス,#駅コード下限,#駅コード上限,#テーブルオフセット
        For i As Integer = 0 To CType(csvData.Item(3), ArrayList).Count - 1

            '線区インデックス
            Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 0), "0", "255", "世代１線区テーブル　線区インデックス")

            '駅コード（下限）
            idxTable.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 1), "0", "255", "世代１線区テーブル　駅コード（下限）"))

            '駅コード（上限）
            idxTable.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 2), "0", "255", "世代１線区テーブル　駅コード（上限）"))

            'ポストペイエリアテーブルオフセット
            If Common.IsBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 3), "0", (CType(csvData.Item(4), ArrayList).Count - 1).ToString) Then
                intVal = fileData.Count + 2 * 256 + 2 + 2 + 2 + CType(csvData.Item(3), ArrayList).Count * 4 + CInt(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), i, 3))
                idxTable.AddRange(Common.INTtoBINwithBigEndian(intVal))
            Else
                AlertBox.Show(Lexis.ERR_COMMON, "世代１線区テーブル　ポストペイエリアテーブルオフセット")
                Throw New Exception
            End If

        Next

        '#線区インデックス０～２５５
        For i As Integer = 0 To 255

            '設定値なければ、０を設定
            intVal = 0

            For j As Integer = 0 To CType(csvData.Item(3), ArrayList).Count - 1

                '線区インデックス
                If CInt(Common.ReadStringFromCSV(CType(csvData.Item(3), ArrayList), j, 0)) = i Then
                    intVal = 1 + 4 + 1 + 2 + 8 + 2 * 256 + 2 + 2 + 2 + j * 4
                    'Exit For
                End If

            Next

            fileData.AddRange(Common.INTtoBINwithBigEndian(intVal))

        Next

        '大近有効特急停車駅テーブルオフセット
        intVal = fileData.Count + 2 + 2 + 2 + CType(csvData.Item(3), ArrayList).Count * 4 + CType(csvData.Item(4), ArrayList).Count
        fileData.AddRange(Common.INTtoBINwithBigEndian(intVal))

        '大近無効特急停車駅テーブルオフセット
        'エンドコード + 大近有効特急停車駅数量 * 4
        intVal = intVal + 2 + CType(csvData.Item(5), ArrayList).Count * 4
        fileData.AddRange(Common.INTtoBINwithBigEndian(intVal))

        '予備
        fileData.AddRange(New Byte(1) {})

        '線区テーブル
        fileData.AddRange(idxTable)
        idxTable.Clear()

        '#テーブルインデックス,#ポストペイエリアデータ,,
        For i As Integer = 0 To CType(csvData.Item(4), ArrayList).Count - 1

            'ポストペイエリアテーブルデータ
            Dim str As String = Common.ReadStringFromCSV(CType(csvData.Item(4), ArrayList), i, 1)

            If str = "0" Or str = "00" Or str = "FF" Then
                fileData.Add(CType("&H" + str, Byte))
            Else
                AlertBox.Show(Lexis.ERR_COMMON, "世代１ポストペイエリアテーブルデータ")
                Throw New Exception
            End If

        Next

        '#大近有効線区,#大近有効駅順,#大近有効系統番号,
        For i As Integer = 0 To CType(csvData.Item(5), ArrayList).Count - 1

            '線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 0), "0", "255", "世代１大近有効特急停車駅　線区コード"))

            '駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 1), "0", "255", "世代１大近有効特急停車駅　駅順コード"))

            '系統番号
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(5), ArrayList), i, 2), "0", "255", "世代１大近有効特急停車駅　系統番号"))

            '予備
            fileData.Add(CType(&H0, Byte))

        Next

        'エンドコード
        fileData.Add(CType(&HFF, Byte))
        fileData.Add(CType(&HFF, Byte))

        '#大近無効線区,#大近無効駅順,#大近無効系統番号,
        For i As Integer = 0 To CType(csvData.Item(6), ArrayList).Count - 1

            '線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(6), ArrayList), i, 0), "0", "255", "世代１大近無効特急停車駅　線区コード"))

            '駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(6), ArrayList), i, 1), "0", "255", "世代１大近無効特急停車駅　駅順コード"))

            '系統番号
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(6), ArrayList), i, 2), "0", "255", "世代１大近無効特急停車駅　系統番号"))

            '予備
            fileData.Add(CType(&H0, Byte))

        Next

        'エンドコード
        fileData.Add(CType(&HFF, Byte))
        fileData.Add(CType(&HFF, Byte))

        '偶数アドレスになるため
        If fileData.Count Mod 2 = 1 Then fileData.Add(CType(&H0, Byte))

        '世代２オフセット計算
        Dim s As UShort = CUShort(fileData.Count)
        Dim offBytes As Byte() = BitConverter.GetBytes(s)

        '==============世代２==================
        '適用日
        fileData.AddRange(Common.GetApplyDateDEC(Common.ReadStringFromCSV(CType(csvData.Item(7), ArrayList), 0, 1), "世代２適用日"))

        '#線区インデックス,#駅コード下限,#駅コード上限,#テーブルオフセット
        For i As Integer = 0 To CType(csvData.Item(8), ArrayList).Count - 1

            '線区インデックス
            Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(8), ArrayList), i, 0), "0", "255", "世代２線区テーブル　線区インデックス")

            '駅コード（下限）
            idxTable.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(8), ArrayList), i, 1), "0", "255", "世代２線区テーブル　駅コード（下限）"))

            '駅コード（上限）
            idxTable.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(8), ArrayList), i, 2), "0", "255", "世代２線区テーブル　駅コード（上限）"))

            'ポストペイエリアテーブルオフセット
            If Common.IsBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(8), ArrayList), i, 3), "0", (CType(csvData.Item(9), ArrayList).Count - 1).ToString) Then
                intVal = fileData.Count + 2 * 256 + 2 + 2 + 2 + CType(csvData.Item(8), ArrayList).Count * 4 + CInt(Common.ReadStringFromCSV(CType(csvData.Item(8), ArrayList), i, 3))
                idxTable.AddRange(Common.INTtoBINwithBigEndian(intVal))
            Else
                AlertBox.Show(Lexis.ERR_COMMON, "世代２線区テーブル　ポストペイエリアテーブルオフセット")
                Throw New Exception
            End If

        Next

        '世帯２適用日までのバイト数
        Dim byteCount As Integer = fileData.Count

        '#線区インデックス０～２５５
        For i As Integer = 0 To 255

            intVal = 0

            For j As Integer = 0 To CType(csvData.Item(8), ArrayList).Count - 1

                '線区インデックス
                If CInt(Common.ReadStringFromCSV(CType(csvData.Item(8), ArrayList), j, 0)) = i Then
                    intVal = byteCount + 2 * 256 + 2 + 2 + 2 + j * 4
                    'Exit For
                End If

            Next

            fileData.AddRange(Common.INTtoBINwithBigEndian(intVal))

        Next

        '大近有効特急停車駅テーブルオフセット
        intVal = fileData.Count + 2 + 2 + 2 + CType(csvData.Item(8), ArrayList).Count * 4 + CType(csvData.Item(9), ArrayList).Count
        fileData.AddRange(Common.INTtoBINwithBigEndian(intVal))

        '大近無効特急停車駅テーブルオフセット
        'エンドコード + 大近有効特急停車駅数量 * 4
        intVal = intVal + 2 + CType(csvData.Item(10), ArrayList).Count * 4
        fileData.AddRange(Common.INTtoBINwithBigEndian(intVal))

        '予備
        fileData.AddRange(New Byte(1) {})

        '線区テーブル
        fileData.AddRange(idxTable)

        '#テーブルインデックス,#ポストペイエリアデータ,,
        For i As Integer = 0 To CType(csvData.Item(9), ArrayList).Count - 1

            'ポストペイエリアテーブルデータ
            Dim str As String = Common.ReadStringFromCSV(CType(csvData.Item(9), ArrayList), i, 1)

            If str = "0" Or str = "00" Or str = "FF" Then
                fileData.Add(CType("&H" + str, Byte))
            Else
                AlertBox.Show(Lexis.ERR_COMMON, "世代２ポストペイエリアテーブルデータ")
                Throw New Exception
            End If

        Next

        '#大近有効線区,#大近有効駅順,#大近有効系統番号,
        For i As Integer = 0 To CType(csvData.Item(10), ArrayList).Count - 1

            '線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(10), ArrayList), i, 0), "0", "255", "世代２大近有効特急停車駅　線区コード"))

            '駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(10), ArrayList), i, 1), "0", "255", "世代２大近有効特急停車駅　駅順コード"))

            '系統番号
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(10), ArrayList), i, 2), "0", "255", "世代２大近有効特急停車駅　系統番号"))

            '予備
            fileData.Add(CType(&H0, Byte))

        Next

        'エンドコード
        fileData.Add(CType(&HFF, Byte))
        fileData.Add(CType(&HFF, Byte))

        '#大近無効線区,#大近無効駅順,#大近無効系統番号,
        For i As Integer = 0 To CType(csvData.Item(11), ArrayList).Count - 1

            '線区コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(11), ArrayList), i, 0), "0", "255", "世代２大近無効特急停車駅　線区コード"))

            '駅順コード
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(11), ArrayList), i, 1), "0", "255", "世代２大近無効特急停車駅　駅順コード"))

            '系統番号
            fileData.AddRange(Common.GetBytesBetweenAnd(Common.ReadStringFromCSV(CType(csvData.Item(11), ArrayList), i, 2), "0", "255", "世代２大近無効特急停車駅　系統番号"))

            '予備
            fileData.Add(CType(&H0, Byte))

        Next

        'エンドコード
        fileData.Add(CType(&HFF, Byte))
        fileData.Add(CType(&HFF, Byte))

        Dim ret As Byte() = CType(fileData.ToArray(GetType(Byte)), Byte())

        '世代２オフセットを設定
        ret(6) = offBytes(1)
        ret(7) = offBytes(0)

        Dim sumValue As Long = 0
        'マスタバージョンを除く
        For i As Integer = 1 To ret.Length - 1
            sumValue += ret(i)
            sumValue = sumValue And &HFFFFFFFF
        Next

        Dim sum As Byte() = BitConverter.GetBytes(CUInt(sumValue))
        ret(1) = sum(3)
        ret(2) = sum(2)
        ret(3) = sum(1)
        ret(4) = sum(0)

        fileData.Clear()

        'パラメータ識別
        fileData.Add(CType(&H89, Byte))

        'フォーマット識別番号
        fileData.Add(CType(&H0, Byte))

        '適用日付
        fileData.AddRange(temp)

        fileData.AddRange(ret)

        Return CType(fileData.ToArray(GetType(Byte)), Byte())

    End Function
    'Ver0.4 ADD END    ポイントポストペイ対応

    Private Sub btnConvert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConvert.Click
        Me.Cursor = Cursors.WaitCursor

        Try
            '確認する
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.Confirm) = DialogResult.No Then
                Exit Sub
            End If

            'バージョン
            If Not Common.IsBetweenAnd(Me.txtVersion.Text, "1", "255") Then
                AlertBox.Show(Lexis.TheInputValueIsUnsuitableForMasterVersion)
                txtVersion.Select()
                Exit Sub
            End If

            Dim data As Byte()

            Dim master As String = CType(cmbMaster.SelectedItem, DataRowView).Item(0).ToString.Substring(0, 3)

            'Ver0.2 ADD START  北陸対応
            'パターン番号取得
            PatternNo = Integer.Parse(CType(cmbPattern.SelectedItem, DataRowView).Item(0).ToString())
            If MastPatternNoChk(master) = False Then
                AlertBox.Show(Lexis.ThePatternNoDoesNotRelated)
                Exit Sub
            End If
            'Ver0.2 ADD END    北陸対応

            If KEN.CompareTo(master) = 0 Then
                data = MakePR_KEN()
            ElseIf DLY.CompareTo(master) = 0 Then
                data = MakePR_DLY()
            ElseIf PAY.CompareTo(master) = 0 Then
                data = MakePR_PAY()
            ElseIf ICD.CompareTo(master) = 0 Then
                data = MakePR_ICD()
            ElseIf LOS.CompareTo(master) = 0 Then
                data = MakePR_LOS()
            ElseIf DSC.CompareTo(master) = 0 Then
                data = MakePR_DSC()
            ElseIf HLD.CompareTo(master) = 0 Then
                data = MakePR_HLD()
            ElseIf EXP.CompareTo(master) = 0 Then
                data = MakePR_EXP()
            ElseIf FRX.CompareTo(master) = 0 Then
                data = MakePR_FRX()
            ElseIf ICH.CompareTo(master) = 0 Then
                'Ver0.1 MOD START  フェーズ２対応
                'data = MakePR_ICH()
                If CType(cmbMaster.SelectedItem, DataRowView).Item(2).ToString = "0" Then
                    data = MakePR_ICH()
                Else
                    data = MakePR_ICH2()
                End If
                'Ver0.1 MOD END
            ElseIf FJW.CompareTo(master) = 0 Then
                data = MakePR_FJW()
            ElseIf IJW.CompareTo(master) = 0 Then
                data = MakePR_IJW()
            ElseIf FJC.CompareTo(master) = 0 Then
                'Ver0.1 MOD START  フェーズ２対応
                'data = MakePR_FJC()
                If CType(cmbMaster.SelectedItem, DataRowView).Item(2).ToString = "0" Then
                    data = MakePR_FJC()
                Else
                    data = MakePR_FJC2()
                End If
                'Ver0.1 MOD END
            ElseIf IJC.CompareTo(master) = 0 Then
                'Ver0.1 MOD START  フェーズ２対応
                'data = MakePR_IJC()
                If CType(cmbMaster.SelectedItem, DataRowView).Item(2).ToString = "0" Then
                    data = MakePR_IJC()
                Else
                    data = MakePR_IJC2()
                End If
                'Ver0.1 MOD END
            ElseIf FJR.CompareTo(master) = 0 Then
                data = MakePR_FJR()
            ElseIf DSH.CompareTo(master) = 0 Then
                data = MakePR_DSH()
            ElseIf LST.CompareTo(master) = 0 Then
                data = MakePR_LST()
            ElseIf IJE.CompareTo(master) = 0 Then
                data = MakePR_IJE()
            ElseIf CYC.CompareTo(master) = 0 Then
                data = MakePR_CYC()
            ElseIf STP.CompareTo(master) = 0 Then
                data = MakePR_STP()
            ElseIf PNO.CompareTo(master) = 0 Then
                data = MakePR_PNO()
            ElseIf FRC.CompareTo(master) = 0 Then
                data = MakePR_FRC()
            ElseIf DUS.CompareTo(master) = 0 Then
                data = MakePR_DUS()
            ElseIf NSI.CompareTo(master) = 0 Then
                data = MakePR_NSI()
            ElseIf NTO.CompareTo(master) = 0 Then
                data = MakePR_NTO()
            ElseIf NIC.CompareTo(master) = 0 Then
                data = MakePR_NIC()
            ElseIf NJW.CompareTo(master) = 0 Then
                data = MakePR_NJW()
                'Ver0.2 ADD START  北陸対応
            ElseIf IUK.CompareTo(master) = 0 Then
                data = MakePR_IUK()
            ElseIf KSZ.CompareTo(master) = 0 Then
                data = MakePR_KSZ()
            ElseIf SWK.CompareTo(master) = 0 Then
                data = MakePR_SWK()
            ElseIf IUZ.CompareTo(master) = 0 Then
                data = MakePR_IUZ()
            ElseIf FSK.CompareTo(master) = 0 Then
                data = MakePR_FSK()
                'Ver0.2 ADD END    北陸対応
                'Ver0.4 ADD START  ポイントポストペイ対応
            ElseIf HIR.CompareTo(master) = 0 Then
                data = MakePR_HIR()
            ElseIf PPA.CompareTo(master) = 0 Then
                data = MakePR_PPA()
                'Ver0.4 ADD END    ポイントポストペイ対応
            Else
                AlertBox.Show(Lexis.ERR_UNKNOWN)
                Exit Sub
            End If

            Try
                'ファイル名を作成
                Dim fileName As String = KindPrefix + CType(cmbMaster.SelectedItem, DataRowView).Item(0).ToString.Substring(0, 3) _
                                          + CType(cmbPattern.SelectedItem, DataRowView).Item(0).ToString _
                                          + "_" + CType(cmbModel.SelectedItem, DataRowView).Item(0).ToString _
                                          + "_" + Me.txtVersion.Text.PadLeft(3, "0"c) + "_" + Now.ToString("yyMMdd") + FileExt

                'SaveFileDialogクラスのインスタンスを作成
                Dim sfd As New SaveFileDialog()

                'ファイル名を指定する
                sfd.FileName = fileName

                'ダイアログを表示する
                If sfd.ShowDialog() = DialogResult.OK Then

                    System.IO.File.WriteAllBytes(sfd.FileName, data)

                    Dim dtNow As DateTime = DateTime.Now

                    Dim oFooter As New EkMasterDataFileFooter(CType(cmbModel.SelectedItem, DataRowView).Item(0).ToString,
                                                               CType(cmbMaster.SelectedItem, DataRowView).Item(0).ToString.Substring(0, 3),
                                                               dtNow,
                                                               Me.txtVersion.Text.PadLeft(3, "0"c),
                                                               CType(cmbMaster.SelectedItem, DataRowView).Item(0).ToString.Substring(3),
                                                               dtNow.ToString("yyyy-MM-dd HH:mm:ss")) '2012-11-19 14:24:59
                    'フッタ追加
                    oFooter.AddInto(sfd.FileName)

                    AlertBox.Show(Lexis.Finished)
                End If
            Catch ex As Exception
                AlertBox.Show(Lexis.ERR_FILE_WRITE)
            End Try
        Catch ex As Exception
            'NOTE: 障害解析のためのログは出力しない...
            'メッセージボックスの内容を伝え聞いて、推測する。

        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Function GetVersion(ByVal master As String, ByVal filename As String) As String

        Dim ar0 As ArrayList = New ArrayList()

        ar0.Add(ICD)
        ar0.Add(FJW)
        ar0.Add(IJW)
        ar0.Add(FSK)    'Ver0.2 MOD 北陸対応
        
        'Ver0.4 ADD START  ポイントポストペイ対応
        ar0.Add(HIR)
        ar0.Add(PPA)
        'Ver0.4 ADD END    ポイントポストペイ対応

        Dim ar1 As ArrayList = New ArrayList()

        'Ver0.1 DEL JR東海 IC運用ﾏｽﾀ　BINファイル取込対応
        'ar1.Add(IJC)
        ar1.Add(NSI)
        ar1.Add(NTO)
        ar1.Add(NIC)
        ar1.Add(NJW)

        If KEN.CompareTo(master) = 0 Or IJE.CompareTo(master) = 0 Or
           IUK.CompareTo(master) = 0 Or IUZ.CompareTo(master) = 0 Then  'Ver0.2 MOD 北陸対応
            Dim binData As Byte() = Common.ReadBin(filename)
            Return binData(0).ToString
            'Ver0.1 ADD STRAT JR東海 IC運用ﾏｽﾀ　BINファイル取込対応
        ElseIf IJC.CompareTo(master) = 0 Then
            If CType(cmbMaster.SelectedItem, DataRowView).Item(2).ToString = "0" Then
                Dim csvData As ArrayList = Common.ReadCsv(filename)
                Return Common.ReadStringFromCSV(csvData, 0, 1)
            Else
                Dim binData As Byte() = Common.ReadBin(filename)
                Return Integer.Parse(Utility.BCDtoCHAR(Utility.GetBytesFromBytes(binData, 0, 2))).ToString
            End If
            'Ver0.1 ADD END
        ElseIf ar0.Contains(master) Then
            Dim csvData As ArrayList = Common.ReadCsv(filename)
            Return Common.ReadStringFromCSV(csvData, 0, 0)
        ElseIf ar1.Contains(master) Then
            Dim csvData As ArrayList = Common.ReadCsv(filename)
            Return Common.ReadStringFromCSV(csvData, 0, 1)
        End If

        Return Nothing
    End Function

    Private Sub EnableBtnConvert()

        If txtVersion.Text.Length > 0 And cmbPattern.SelectedIndex > 0 Then
            btnConvert.Enabled = True
        Else
            btnConvert.Enabled = False
        End If
    End Sub

    'Ver0.2 ADD START  北陸対応
    ''' <summary>
    ''' マスタパターン番号チェック（北陸用かどうかのチェック（定義はINI））
    ''' </summary>
    Private Function MastPatternNoChk(ByVal master As String) As Boolean
        For Each subList As ArrayList In Config.LimitPatterns
            If CType(subList(0), String) = master Then
                If CType(subList(1), Integer) <= PatternNo And PatternNo <= CType(subList(2), Integer) Then
                    Return True
                End If
                Return False
            End If
        Next
        Return True
    End Function

    ''' <summary>
    ''' ファイル名バージョン取得処理（ファイル名にバージョンがあれば取得する）
    ''' </summary>
    Private Function GetFileVersion(ByVal filename As String) As String
        Dim tempFileName As String = ""
        Dim sVersion As String = ""
        Dim iVersion As Integer
        Try
            '拡張子もフォルダ名も無いファイル名だけを取得する
            tempFileName = Path.GetFileNameWithoutExtension(filename)
        Catch
            'ファイル名が取れなければ空文字列を返す
            Return ""
        End Try

        'ファイル名が６文字以上ないとバージョンはない
        If tempFileName.Length >= 6 Then
            'ファイル名の後ろ６文字の頭３文字がVERでないとバージョンはない
            If tempFileName.Substring(tempFileName.Length - 6, 3).ToUpper() = "VER" Then
                'ファイル名の後ろ３文字が数字だとバージョンである
                sVersion = tempFileName.Substring(tempFileName.Length - 3)
                If Integer.TryParse(sVersion, iVersion) Then
                    'ゼロサプレスでバージョンを返す
                    Return iVersion.ToString()
                End If
            End If
        End If

        '空文字列を返す
        Return ""
    End Function

    ''' <summary>
    ''' ファイルタイプチェック処理
    ''' </summary>
    Private Function ChkFileType(ByVal filetype As String, ByVal filename As String) As Boolean
        Dim extension As String
        Try
            '拡張子を取得する
            extension = Path.GetExtension(filename).ToUpper()
        Catch
            '拡張子取得に失敗すると拡張子なしとする
            extension = ""
        End Try

        If filetype = "CSV" Then
            '入力ファイルタイプがＣＳＶならＣＳＶファイル以外ダメ
            If extension <> ".CSV" Then
                'ＣＳＶファイル以外ダメ
                AlertBox.Show(Lexis.FileTypeNG1)
                Return False
            End If
        Else
            '入力ファイルタイプがバイナリなら
            If extension = ".CSV" Then
                'ＣＳＶファイルはダメ
                AlertBox.Show(Lexis.FileTypeNG2)
                Return False
            ElseIf extension = ".BIN" Then
                'フッタチェック
                If ChkBinFileFooter(filename) Then
                    '変換済みBINが指定された
                    AlertBox.Show(Lexis.FileTypeNG3)
                    Return False
                End If
            End If
        End If

        '入力ファイルタイプチェックＯＫ
        Return True
    End Function

    ''' <summary>
    ''' バイナリファイルフッタチェック処理
    ''' </summary>
    Private Function ChkBinFileFooter(ByVal filename As String) As Boolean
        Dim binData As Byte()
        Dim FooterKisyuBin(7) As Byte
        Dim FooterKisyu As String

        Try
            'ファイル読む
            binData = Common.ReadBin(filename)
            'サイズチェック（ヘッダ＋フッタが１０２バイトそれ未満は変化前とみなす）
            If binData.Length < 102 Then
                'フッタなし扱い
                Return False
            End If
            'フッタ機種チェック
            Array.Copy(binData, binData.Length - 96, FooterKisyuBin, 0, 8)
            FooterKisyu = System.Text.Encoding.Default.GetString(FooterKisyuBin)
            If FooterKisyu = "EG7000  " Or FooterKisyu = "EY4100  " Then
                'フッタ付き扱い（再変換されかけている）
                Return True
            End If
        Catch
            'フッタなし扱い
            Return False
        End Try

        'フッタなし扱い
        Return False
    End Function
    'Ver0.2 ADD END    北陸対応

End Class