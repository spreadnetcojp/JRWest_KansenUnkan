' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2013/12/09  (NES)金沢  運管と切断された場合の対応
'   0.2      2013/12/14  (NES)金沢  窓処と運管が切断された場合の対応
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '定数値のみ使用
Imports JR.ExOpmg.DataAccess
Imports System.IO
Imports System
Imports System.Text
Imports GrapeCity.Win

''' <summary>
''' 【機器接続状態確認　画面クラス】
''' </summary>
Public Class FrmMntDispConStatus
    Inherits FrmBase

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        LcstSearchCol = {Me.cmbEki, Me.cmbMado}
    End Sub

    ' Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使って変更してください。
    ' コード エディタを使って変更しないでください。
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents wkbMain As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents pnlMado As System.Windows.Forms.Panel
    Friend WithEvents cmbMado As System.Windows.Forms.ComboBox
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents lblRefreshRate As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispConStatus))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.wkbMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.pnlMado = New System.Windows.Forms.Panel()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.lblRefreshRate = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.wkbMain.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlMado.SuspendLayout()
        Me.pnlEki.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.lblRefreshRate)
        Me.pnlBodyBase.Controls.Add(Me.pnlMado)
        Me.pnlBodyBase.Controls.Add(Me.pnlEki)
        Me.pnlBodyBase.Controls.Add(Me.wkbMain)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnKensaku)
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 87)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/12/18(水)  10:10"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'wkbMain
        '
        Me.wkbMain.Controls.Add(Me.shtMain)
        Me.wkbMain.Location = New System.Drawing.Point(13, 67)
        Me.wkbMain.Name = "wkbMain"
        Me.wkbMain.ProcessTabKey = False
        Me.wkbMain.ShowTabs = False
        Me.wkbMain.Size = New System.Drawing.Size(988, 483)
        Me.wkbMain.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wkbMain.TabIndex = 8
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(2, 2)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(967, 462)
        Me.shtMain.TabIndex = 0
        Me.shtMain.TabStop = False
        Me.shtMain.TransformEditor = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(705, 584)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 4
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(873, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 5
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(873, 7)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 3
        Me.btnKensaku.Text = "検　索"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'pnlMado
        '
        Me.pnlMado.Controls.Add(Me.cmbMado)
        Me.pnlMado.Controls.Add(Me.lblMado)
        Me.pnlMado.Location = New System.Drawing.Point(241, 14)
        Me.pnlMado.Name = "pnlMado"
        Me.pnlMado.Size = New System.Drawing.Size(284, 33)
        Me.pnlMado.TabIndex = 2
        '
        'cmbMado
        '
        Me.cmbMado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMado.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMado.ItemHeight = 13
        Me.cmbMado.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbMado.Location = New System.Drawing.Point(67, 6)
        Me.cmbMado.Name = "cmbMado"
        Me.cmbMado.Size = New System.Drawing.Size(162, 21)
        Me.cmbMado.TabIndex = 2
        '
        'lblMado
        '
        Me.lblMado.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMado.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMado.Location = New System.Drawing.Point(3, 6)
        Me.lblMado.Name = "lblMado"
        Me.lblMado.Size = New System.Drawing.Size(64, 21)
        Me.lblMado.TabIndex = 0
        Me.lblMado.Text = "コーナー"
        Me.lblMado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlEki
        '
        Me.pnlEki.Controls.Add(Me.cmbEki)
        Me.pnlEki.Controls.Add(Me.lblEki)
        Me.pnlEki.Location = New System.Drawing.Point(9, 14)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(226, 33)
        Me.pnlEki.TabIndex = 1
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbEki.Location = New System.Drawing.Point(45, 6)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(162, 21)
        Me.cmbEki.TabIndex = 1
        '
        'lblEki
        '
        Me.lblEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblEki.Location = New System.Drawing.Point(4, 6)
        Me.lblEki.Name = "lblEki"
        Me.lblEki.Size = New System.Drawing.Size(39, 21)
        Me.lblEki.TabIndex = 0
        Me.lblEki.Text = "駅名"
        Me.lblEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblRefreshRate
        '
        Me.lblRefreshRate.AutoSize = True
        Me.lblRefreshRate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblRefreshRate.Location = New System.Drawing.Point(13, 596)
        Me.lblRefreshRate.Name = "lblRefreshRate"
        Me.lblRefreshRate.Size = New System.Drawing.Size(200, 16)
        Me.lblRefreshRate.TabIndex = 11
        Me.lblRefreshRate.Text = "現在、Z9分毎に自動更新中"
        '
        'FrmMntDispConStatus
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispConStatus"
        Me.Text = "運用端末 Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlBodyBase.PerformLayout()
        Me.wkbMain.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlMado.ResumeLayout(False)
        Me.pnlEki.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "宣言領域（Private）"

    ''' <summary>
    ''' 初期処理呼出判定
    ''' （True:初期処理呼出済み、False:初期処理未呼出(Form_Load内で初期処理実施)）
    ''' </summary>
    Private LbInitCallFlg As Boolean = False

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean
    ''' <summary>
    ''' 出力用テンプレートファイル名
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "機器接続状態確認.xls"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "機器接続状態確認"

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly FormTitle As String = "機器接続状態確認"

    ''' <summary>
    ''' 駅コードの先頭3桁:「000」
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"

    ''' <summary>
    ''' 一覧表示最大列
    ''' </summary>
    Private LcstMaxColCnt As Integer
    ''' <summary>
    ''' 一覧ヘッダのソート列割り当て
    ''' （一覧ヘッダクリック時に割り当てる対象列を定義。列番号はゼロ相対で"-1"はソート対象外の列）
    ''' 駅名、機種、最終収集日時、電源、監視盤（主）、主（IC）、
    ''' 配信SV（主）、配信SV（IC）、明収/EX統括、明収/EX統括（DL）のヘッダが選択可能（ソート可能）
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, 0, -1, 13, -1, 5, 14, 15, 16, 17, 18, 19, 20, -1}

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した機器接続状態確認に対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}

    ''' <summary>
    ''' 検索条件によって、検索ボタン活性化
    ''' </summary>
    Private LcstSearchCol() As Control

    '適用開始日
    Private sApplyDate As String = Now.ToString("yyyyMMdd")     'デフォルトをシステム日付
    '適用開始日
    Public Property ApplyDate() As String
        Get
            Return sApplyDate
        End Get
        Set(ByVal Value As String)
            sApplyDate = Value
        End Set
    End Property

    '検索SQL取得区分
    Private Enum SlcSQLType
        SlcCount = 0  '件数取得用
        SlcDetail = 1 'データ検索用
    End Enum


    ''' <summary>
    ''' 通常電源OFF
    ''' </summary>
    Private Const LcstPowerOff As String = "×"
    ''' <summary>
    ''' 通常電源ON
    ''' </summary>
    Private Const LcstPowerOn As String = "○"
    ''' <summary>
    ''' ０：正常
    ''' </summary>
    Private Const LcstNormal As String = "0"
    ''' <summary>
    ''' １：異常
    ''' </summary>
    Private Const LcstUnusual As String = "1"
    ''' <summary>
    ''' 通常単体電源ON
    ''' </summary>
    Private Const LcstSinglePowerOn As String = "単"
    ''' <summary>
    ''' 上位以外
    ''' </summary>
    Private Const LcstOther As String = "-"
    ''' <summary>
    ''' 機種:窓
    ''' </summary>
    Private Const LcstY As String = "Y"
    ''' <summary>
    ''' 機種:改
    ''' </summary>
    Private Const LcstG As String = "G"
    ''' <summary>
    ''' 更新時間
    ''' </summary>
    Private LcstTime As Integer
    ''' <summary>
    ''' 更新
    ''' </summary>
    Private LcstRefreshRate As String = " 現在、{0}分毎に自動更新中"
    ''' <summary>
    ''' 開始時間
    ''' </summary>
    Private LcstSystemDate As DateTime
#End Region

#Region "メソッド（Public）"

    ''' <summary>
    ''' [画面初期処理]
    ''' エラー発生時は内部でメッセージを表示します。
    ''' </summary>
    ''' <returns>True:成功,False:失敗</returns>
    Public Function InitFrm() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Try
            Log.Info("Method started.")

            '--画面タイトル
            lblTitle.Text = FormTitle
            'シート初期化
            shtMain.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row                      '行選択モード
            shtMain.MaxRows() = 0                                               '行の初期化
            LcstMaxColCnt = shtMain.MaxColumns()                                '列数を取得
            'シートの表示選択モードを設定する
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtMain.ColumnHeaders.HeaderClick, AddressOf Me.shtMainColumnHeaders_HeadersClick
            AddHandler Me.Timer1.Tick, AddressOf Me.btnKensaku_Click
            btnReturn.Enabled = True        '終了ボタン
            '値初期化
            Dim all As Control() = BaseGetAllControls(pnlBodyBase)
            For Each c As Control In all
                Try
                    If TypeOf c Is RadioButton Then
                        CType(c, RadioButton).Checked = False
                    ElseIf TypeOf c Is ComboBox Then
                        CType(c, ComboBox).DataSource = Nothing
                        If CType(c, ComboBox).Items.Count > 0 Then CType(c, ComboBox).Items.Clear()
                        CType(c, ComboBox).MaxDropDownItems = 20
                    End If
                Catch ex As Exception
                End Try
            Next

            '駅コンボ設定
            BaseCtlEnabled(pnlEki)          '駅コンボ活性化
            LbEventStop = False 'イベント発生ＯＮ
            '各コンボボックスの項目登録
            If LfSetEki() = False Then Exit Try '駅名コンボボックス設定
            cmbEki.SelectedIndex = 0            'デフォルト表示項目
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try 'コーナーコンボボックス設定
            cmbMado.SelectedIndex = 0           'デフォルト表示項目

            LfClrList() '一覧初期化
            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)        '開始異常メッセージ
            End If
            LbEventStop = False 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

#End Region

#Region "イベント"

    ''' <summary>
    ''' フォームロード
    ''' </summary>
    Private Sub FrmMntDispConnectionStatus_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrm() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If
            '検索ボタン活性化
            LfSearchTrue()
            LcstTime = Config.ConStatusDispRefreshRate
            Timer1.Interval = LcstTime * 60000
            Timer1.Enabled = True
            Timer1.Start()
            LcstSystemDate = System.DateTime.Now
            lblRefreshRate.Text = String.Format(LcstRefreshRate, LcstTime)
            cmbEki.Select() '初期フォーカス
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////ボタンクリック

    ''' <summary>
    ''' 終了
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnReturn.Click
        LogOperation(sender, e)   'ボタン押下ログ
        Me.Close()
    End Sub

    ''' <summary>
    ''' 検索
    ''' </summary>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnKensaku.Click
        If LbEventStop Then Exit Sub
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        LfWaitCursor()
        Try
            Timer1.Stop()
            Timer1.Enabled = False
            LbEventStop = True
            LogOperation(sender, e)   'ボタン押下ログ
            '初期化処理
            LfClrList()

            '運用管理端末のINIファイルから取得可能件数を取得
            Dim nMaxCount As Integer = Config.MaxUpboundDataToGet

            '件数取得チェック
            sSql = LfGetSelectString(SlcSQLType.SlcCount)
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case Else
                    '上限チェック
                    If Convert.ToInt64(dt.Rows(0)(0)) > nMaxCount Then
                        AlertBox.Show(Lexis.HugeRecordsFound, nMaxCount.ToString())
                        cmbEki.Select()
                        Exit Sub
                    ElseIf Convert.ToInt64(dt.Rows(0)(0)) = 0 Then
                        AlertBox.Show(Lexis.NoRecordsFound)
                        cmbEki.Select()
                        Exit Sub
                    End If
            End Select

            'クリア
            sSql = ""
            dt = New DataTable

            'データ取得処理
            sSql = LfGetSelectString(SlcSQLType.SlcDetail)
            nRtn = BaseSqlDataTableFill(sSql, dt)
            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '該当なし
                    AlertBox.Show(Lexis.NoRecordsFound)
                    cmbEki.Select()
                    Exit Sub
                Case Is > nMaxCount     '件数＞取得可能件数
                    AlertBox.Show(Lexis.HugeRecordsFound, nMaxCount.ToString())
                    cmbEki.Select()
                    Exit Sub
            End Select
            '取得データを一覧に設定
            LfSetSheetData(dt)
            '一覧、出力ボタン活性化
            If shtMain.Enabled = False Then shtMain.Enabled = True
            If btnPrint.Enabled = False Then btnPrint.Enabled = True
            shtMain.Select()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)       '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred)   '検索失敗メッセージ
            btnReturn.Select()
        Finally
            Dim ND As System.TimeSpan = System.DateTime.Now - LcstSystemDate
            LcstTime = Config.ConStatusDispRefreshRate
            Timer1.Interval += ND.Minutes
            Timer1.Enabled = True
            dt = Nothing
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 出力
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click

        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            LogOperation(sender, e)    'ボタン押下ログ
            Dim sPath As String = Config.LedgerTemplateDirPath

            'テンプレート格納フォルダチェック
            If Directory.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If
            'テンプレートフルパスチェック
            sPath = Path.Combine(sPath, LcstXlsTemplateName)
            If File.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If
            '出力
            LfXlsStart2(sPath)
            cmbEki.Select()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'エラーメッセージ
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////SelectedIndexChanged

    '''<summary>
    ''' 「駅」コンボ
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            'コーナーコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then
                If cmbMado.Enabled = True Then BaseCtlDisabled(pnlMado, False)
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbMado.SelectedIndex = 0               '★イベント発生箇所
            If cmbMado.Enabled = False Then BaseCtlEnabled(pnlMado)
            LfSearchTrue()
        Catch ex As Exception
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub
    '''<summary>
    ''' 「コーナー」コンボ
    ''' </summary>
    Private Sub cmbMado_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbMado.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            LfSearchTrue()
        Catch ex As Exception
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////ValueChanged


    '//////////////////////////////////////////////ElTable関連
    ''' <summary>
    ''' ElTable
    ''' </summary>
    Private Sub shtMainColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)
        Static intCurrentSortColumn As Integer = -1
        Static bolColumn1SortOrder(63) As Boolean

        If LcstSortCol(e.Column) = -1 Then Exit Sub

        Try

            shtMain.BeginUpdate()

            '前回選択された列ヘッダの初期化
            If intCurrentSortColumn > -1 Then
                '列ヘッダのイメージを削除する
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '列の背景色を初期化する
                shtMain.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '列のセル罫線を消去する
                shtMain.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
            End If

            '選択された列番号を保存
            intCurrentSortColumn = e.Column

            'ソートする列の背景色を設定する
            shtMain.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
            'ソートする列のセル罫線を設定する
            shtMain.Columns(intCurrentSortColumn).SetBorder( _
                New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                GrapeCity.Win.ElTabelleSheet.Borders.All)

            If bolColumn1SortOrder(intCurrentSortColumn) = False Then
                '列ヘッダのイメージを設定する
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(1)
                '降順でソートする
                Call SheetSort(shtMain, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Descending)
                '列のソート状態を保存する
                bolColumn1SortOrder(intCurrentSortColumn) = True
            Else
                '列ヘッダのイメージを設定する
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(0)
                '昇順でソートする
                Call SheetSort(shtMain, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Ascending)
                '列のソート状態を保存する
                bolColumn1SortOrder(intCurrentSortColumn) = False
            End If

            shtMain.EndUpdate()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' MouseMove
    ''' </summary>
    Private Sub shtMain_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
            'マウスカーソルが列ヘッダ上にある場合
            If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtMain.CrossCursor = Cursors.Default
            Else
                'マウスカーソルを既定に戻す
                shtMain.CrossCursor = Nothing
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' ソート
    ''' </summary>
    Private Sub SheetSort(ByRef sheetTarget As GrapeCity.Win.ElTabelleSheet.Sheet, ByVal intKeyColumn As Integer, ByVal sortOrder As GrapeCity.Win.ElTabelleSheet.SortOrder)
        Dim objSortItem As New GrapeCity.Win.ElTabelleSheet.SortItem(intKeyColumn, False, sortOrder)
        Dim objSortList(0) As GrapeCity.Win.ElTabelleSheet.SortItem
        '配列にソートオブジェクトを追加する
        objSortList(0) = objSortItem
        'ソートを実行する
        sheetTarget.Sort(objSortList)
    End Sub

#End Region

#Region "メソッド（Private）"

    ''' <summary>
    ''' [一覧クリア]
    ''' </summary>
    Private Sub LfClrList()
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try
            Dim i As Integer
            'ソート情報のクリア
            With shtMain
                For i = 0 To LcstMaxColCnt - 1
                    .ColumnHeaders(i).Image = Nothing
                    .Columns(i).BackColor = Color.Empty
                Next
            End With
            shtMain.DataSource = Nothing
            shtMain.MaxRows = 0

            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
        Finally
            wkbMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' [検索ボタン活性化]
    ''' </summary>
    Private Sub LfSearchTrue()
        Dim bEnabled As Boolean = True
        If bEnabled Then
            If ((cmbEki.SelectedIndex < 0) OrElse _
                (cmbMado.SelectedIndex < 0)) Then
                bEnabled = False
            End If
        End If
        If bEnabled Then
            If btnKensaku.Enabled = False Then btnKensaku.Enabled = True
        Else
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
        End If
        '検索ボタン活性化
        Call LfSearchButton()
    End Sub
    ''' <summary>
    ''' 検索ボタン活性化
    ''' </summary>
    Private Sub LfSearchButton()
        Dim bEnabled As Boolean = True
        For Each control As Control In LcstSearchCol
            If control.Enabled = False Then
                bEnabled = False
                Exit For
            End If
        Next
        If bEnabled Then
            btnKensaku.Enabled = True
        Else
            btnKensaku.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' [コーナーコンボ設定]
    ''' </summary>
    ''' <param name="Station">駅コード</param>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetMado(ByVal Station As String) As Boolean
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As CornerMaster
        oMst = New CornerMaster
        Try
            oMst.ApplyDate = ApplyDate
            If String.IsNullOrEmpty(Station) Then
                Station = ""
            End If
            If Station <> "" And Station <> ClientDaoConstants.TERMINAL_ALL Then
                dt = oMst.SelectTable(Station, "G,Y")
            End If
            dt = oMst.SetAll()
            bRtn = BaseSetMstDtToCmb(dt, cmbMado)
            cmbMado.SelectedIndex = -1
            If cmbMado.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
            LbEventStop = False 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [検索用SELECT文字列取得]
    ''' </summary>
    ''' <returns>SELECT文</returns>
    Private Function LfGetSelectString(ByVal slcSQLType As SlcSQLType) As String
        Dim sSql As String = ""
        Try
            Dim sSqlWhere As New StringBuilder
            Dim sBuilder As New StringBuilder
            Dim sEki As String
            sBuilder.AppendLine("")
            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '件数取得--------------------------
                    sBuilder.AppendLine(" SELECT COUNT(1) FROM V_CON_STATUS ")
                    '取得項目--------------------------
                Case slcSQLType.SlcDetail
                    '-----------Ver0.1　運管と切断対応　MOD START--------------------------------------------------------------
                    '---------駅単位検索対応　　START----------------------------
                    sBuilder.AppendLine(" SELECT * FROM ( ")
                    '---------駅単位検索対応　　END------------------------------
                    sBuilder.AppendLine("  SELECT STATION_CODE,STATION_NAME ,CORNER_NAME,MODEL_NAME,UNIT_NO   ")
                    sBuilder.AppendLine("  ,Convert(varchar(10),SYUSYU_DATE,111)+' '+Convert(varchar(8),SYUSYU_DATE,8) as SYUSYU_DATE  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when KAIDENGEN=null then '-'  ")
                    sBuilder.AppendLine("  		else '-' end)   ")
                    sBuilder.AppendLine("  	else '-' end ) As KAIDENGEN  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUDLCONNECT=2 then  ")
                    sBuilder.AppendLine("  		(case when  EXTOKATUCONNECT=2 OR EXTOKATUCONNECT=1 then  ")
                    sBuilder.AppendLine("  			(case when KANSICONNECT=2 then '○'  ")
                    sBuilder.AppendLine("  				when KANSICONNECT=1 then '×'   ")
                    sBuilder.AppendLine("  			 else '-' end)  ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end)	 ")
                    sBuilder.AppendLine("    else '-' end ) As KANSICONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when SHUSECONNECT = null then '-'  ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end ) As SHUSECONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUDLCONNECT=2 then  ")
                    sBuilder.AppendLine("  		(case when EXTOKATUCONNECT=2 OR EXTOKATUCONNECT=1 then  ")
                    sBuilder.AppendLine("  			(case when HAISINSYUCONNECT=2 then '○'  ")
                    sBuilder.AppendLine("  				when HAISINSYUCONNECT=1 then '×'   ")
                    sBuilder.AppendLine("  			else '-' end)  ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end )  ")
                    sBuilder.AppendLine("   else '-' end) As HAISINSYUCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when HAISINICMCONNECT = null then '-'   ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	else '-' end) As HAISINICMCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUCONNECT=2 then '○'  ")
                    sBuilder.AppendLine("  		when EXTOKATUCONNECT=1 then '×'   ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	else '-' end) As EXTOKATUCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUDLCONNECT=2 then '○'  ")
                    sBuilder.AppendLine("      when EXTOKATUDLCONNECT=1 then '×'   ")
                    sBuilder.AppendLine("      else '-' end)  ")
                    sBuilder.AppendLine("     else '-' end ) As EXTOKATUDLCONNECT  ")
                    sBuilder.AppendLine("  ,MODEL_CODE  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case when KAIDENGEN = null then -3  ")
                    sBuilder.AppendLine("  		else -3	 end)   ")
                    sBuilder.AppendLine("  	else -3 end ) As KAIDENGEN1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUDLCONNECT=2 then  ")
                    sBuilder.AppendLine("  		(case when  EXTOKATUCONNECT=2 OR EXTOKATUCONNECT=1 then   ")
                    sBuilder.AppendLine(" 			 (case when KANSICONNECT=2 then -1   ")
                    sBuilder.AppendLine("  					when KANSICONNECT=1 then -4   ")
                    sBuilder.AppendLine("  			 else -3 end ) ")
                    sBuilder.AppendLine("  		 else -3 end ) ")
                    sBuilder.AppendLine("  	 else -3 end ) ")
                    sBuilder.AppendLine("   else -3  end)  As KANSICONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	 (case  when SHUSECONNECT = null then -3   ")
                    sBuilder.AppendLine(" 	  else -3 end) ")
                    sBuilder.AppendLine("    else -3  end) As SHUSECONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case when EXTOKATUDLCONNECT=2 then  ")
                    sBuilder.AppendLine("  		(case when EXTOKATUCONNECT=2 OR EXTOKATUCONNECT=1 then  ")
                    sBuilder.AppendLine(" 			(case when HAISINSYUCONNECT=2 then -1   ")
                    sBuilder.AppendLine("  				when HAISINSYUCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  			 else -3 end) ")
                    sBuilder.AppendLine("  		else -3 end) ")
                    sBuilder.AppendLine("  	else -3 end) ")
                    sBuilder.AppendLine("   else -3  end) As HAISINSYUCONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case  when HAISINICMCONNECT = null then -3   ")
                    sBuilder.AppendLine(" 	else -3 end) ")
                    sBuilder.AppendLine("    else -3 end)  As HAISINICMCONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case when EXTOKATUCONNECT=2 then -1   ")
                    sBuilder.AppendLine("  		when EXTOKATUCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  	else -3  end) ")
                    sBuilder.AppendLine("   else -3  end) As EXTOKATUCONNECT1   ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case when EXTOKATUDLCONNECT=2 then -1   ")
                    sBuilder.AppendLine(" 		when EXTOKATUDLCONNECT=1 then -4   ")
                    sBuilder.AppendLine("      else -3 end) ")
                    sBuilder.AppendLine("    else -3  end)  As EXTOKATUDLCONNECT1 ,CNT,CORNER_CODE  ")
                    sBuilder.AppendLine(" FROM  ")
                    '--------Ver 0.2 修正前　　START-----------------------------------------------------------------
                    'sBuilder.AppendLine("   (select V_CON_STATUS.*,   ")
                    'sBuilder.AppendLine("   	(select COUNT(*)   ")
                    'sBuilder.AppendLine("   	    from S_DIRECT_CON_STATUS ds   ")
                    'sBuilder.AppendLine("  		where(V_CON_STATUS.STATION_CODE = ds.RAIL_SECTION_CODE + ds.STATION_ORDER_CODE)   ")
                    'sBuilder.AppendLine("   		and V_CON_STATUS.CORNER_CODE = ds.CORNER_CODE  and V_CON_STATUS.MODEL_CODE = ds.MODEL_CODE    ")
                    'sBuilder.AppendLine("   		and V_CON_STATUS.UNIT_NO = ds.UNIT_NO ) as CNT  ")
                    '--------Ver 0.2   修正前　　END-----------------------------------------------------------------
                    '--------Ver 0.2   修正後　　START---------------------------------------------------------------
                    sBuilder.AppendLine("   (select V_CON_STATUS.*,   ")
                    sBuilder.AppendLine("   	(select COUNT(*)   ")
                    sBuilder.AppendLine("   	    from  V_MACHINE_NOW  m1,V_MACHINE_NOW m2,S_DIRECT_CON_STATUS ds   ")
                    sBuilder.AppendLine("                      where(V_CON_STATUS.STATION_CODE = m1.RAIL_SECTION_CODE + m1.STATION_ORDER_CODE)   ")
                    sBuilder.AppendLine("   		and V_CON_STATUS.CORNER_CODE = m1.CORNER_CODE  and V_CON_STATUS.MODEL_CODE = m1.MODEL_CODE    ")
                    sBuilder.AppendLine("   		and V_CON_STATUS.UNIT_NO = m1.UNIT_NO    ")
                    sBuilder.AppendLine("   		and m1.MONITOR_ADDRESS = m2.ADDRESS    ")
                    sBuilder.AppendLine("   		and m2.MODEL_CODE = 'X'    ")
                    sBuilder.AppendLine("   		and m2.RAIL_SECTION_CODE = ds.RAIL_SECTION_CODE    ")
                    sBuilder.AppendLine("   		and m2.STATION_ORDER_CODE = ds.STATION_ORDER_CODE    ")
                    sBuilder.AppendLine("   		and m2.MODEL_CODE = ds.MODEL_CODE    ")
                    sBuilder.AppendLine("   		and m2.CORNER_CODE = ds.CORNER_CODE    ")
                    sBuilder.AppendLine("   		and m2.UNIT_NO = ds.UNIT_NO    ")
                    sBuilder.AppendLine("           and ds.PORT_KBN='1' ")
                    sBuilder.AppendLine("   	) as CNT   ")
                    '--------Ver 0.2   修正後　　END--------------------------------------------------------------------
                    sBuilder.AppendLine("   from V_CON_STATUS  where V_CON_STATUS.MODEL_CODE ='Y' ) dt  ")
                    sBuilder.AppendLine("   UNION  ")
                    sBuilder.AppendLine("  SELECT STATION_CODE,STATION_NAME ,CORNER_NAME,MODEL_NAME,UNIT_NO   ")
                    sBuilder.AppendLine("  ,Convert(varchar(10),SYUSYU_DATE,111)+' '+Convert(varchar(8),SYUSYU_DATE,8) as SYUSYU_DATE  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when KAIDENGEN=1 then '○'  ")
                    sBuilder.AppendLine("  		when KAIDENGEN=2 then '×'   ")
                    sBuilder.AppendLine("  		when KAIDENGEN=3 then '単'  ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end) As KAIDENGEN  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when KAIDENGEN=1 OR KAIDENGEN=3 then   ")
                    sBuilder.AppendLine("  		(case when KANSICONNECT=0 then '○'  ")
                    sBuilder.AppendLine("  			when KANSICONNECT=1 then '×'   ")
                    sBuilder.AppendLine("  		 else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end)  ")
                    sBuilder.AppendLine("   else '-' end ) As KANSICONNECT  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0  then  ")
                    sBuilder.AppendLine("  		(case when SHUSECONNECT=0 then '○'  ")
                    sBuilder.AppendLine("  			when SHUSECONNECT=1 then '×'   ")
                    sBuilder.AppendLine("  		 else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end)  ")
                    sBuilder.AppendLine("    else '-' end) As SHUSECONNECT  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3)and (SHUSECONNECT=0 OR SHUSECONNECT=1) and KANSICONNECT=0 then   ")
                    sBuilder.AppendLine(" 		(case when HAISINSYUCONNECT=0 then '○'  ")
                    sBuilder.AppendLine(" 			when HAISINSYUCONNECT=1 then '×'   ")
                    sBuilder.AppendLine(" 		 else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end)  ")
                    sBuilder.AppendLine("    else '-' end) As HAISINSYUCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0 and SHUSECONNECT=0 then  ")
                    sBuilder.AppendLine("  		(case when HAISINICMCONNECT=0 then '○'  ")
                    sBuilder.AppendLine("  			when HAISINICMCONNECT=1 then '×'   ")
                    sBuilder.AppendLine("  		else '-' end )  ")
                    sBuilder.AppendLine("  	else '-' end )  ")
                    sBuilder.AppendLine("   else '-' end) As HAISINICMCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0 and SHUSECONNECT=0 then  ")
                    sBuilder.AppendLine("  		(case when EXTOKATUCONNECT=0 then '○'  ")
                    sBuilder.AppendLine("  			when EXTOKATUCONNECT=1 then '×'   ")
                    sBuilder.AppendLine("  		else '-' end )  ")
                    sBuilder.AppendLine("  	else '-' end)	  ")
                    sBuilder.AppendLine("   else '-' end) As EXTOKATUCONNECT  ")
                    sBuilder.AppendLine("  ,(case when EXTOKATUDLCONNECT = null then '-'  ")
                    sBuilder.AppendLine("       else '-' end) As EXTOKATUDLCONNECT  ")
                    sBuilder.AppendLine("  ,MODEL_CODE  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine(" 	(case when KAIDENGEN=1 then -1  ")
                    sBuilder.AppendLine("  		when KAIDENGEN=2 then -4   ")
                    sBuilder.AppendLine("  		when KAIDENGEN=3 then -2   ")
                    sBuilder.AppendLine("  		else -3 end ) ")
                    sBuilder.AppendLine("  	else -3  end) As KAIDENGEN1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when KAIDENGEN=1 OR KAIDENGEN=3 then   ")
                    sBuilder.AppendLine(" 		(case when KANSICONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when KANSICONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		else -3 end) ")
                    sBuilder.AppendLine("  	else -3 end)  ")
                    sBuilder.AppendLine("   else -3 end) As KANSICONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0  then  ")
                    sBuilder.AppendLine(" 		(case when SHUSECONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when SHUSECONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		else -3  end) ")
                    sBuilder.AppendLine("  	 else -3 end)	 ")
                    sBuilder.AppendLine("   else -3 end) As SHUSECONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3)and (SHUSECONNECT=0 OR SHUSECONNECT=1) and KANSICONNECT=0 then  ")
                    sBuilder.AppendLine(" 		(case when HAISINSYUCONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when HAISINSYUCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		else -3 end) ")
                    sBuilder.AppendLine("  	 else -3 end)   ")
                    sBuilder.AppendLine("   else -3 end) As HAISINSYUCONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0 and SHUSECONNECT=0 then ")
                    sBuilder.AppendLine(" 		(case when HAISINICMCONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when HAISINICMCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		else -3 end) ")
                    sBuilder.AppendLine("  	else -3 end )	  ")
                    sBuilder.AppendLine("   else -3 end) As HAISINICMCONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0 and SHUSECONNECT=0 then  ")
                    sBuilder.AppendLine(" 		(case when EXTOKATUCONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when EXTOKATUCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		 else -3  end ) ")
                    sBuilder.AppendLine("  	 else -3 end)  ")
                    sBuilder.AppendLine("    else -3 end) As EXTOKATUCONNECT1   ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine(" 	(case when EXTOKATUDLCONNECT = null then -3   ")
                    sBuilder.AppendLine("     else -3  end ) ")
                    sBuilder.AppendLine("    else -3 end) As EXTOKATUDLCONNECT1,CT,CORNER_CODE  ")
                    sBuilder.AppendLine("   FROM   ")
                    sBuilder.AppendLine("   (select V_CON_STATUS.*,   ")
                    sBuilder.AppendLine("   	(select COUNT(*)   ")
                    sBuilder.AppendLine("   	    from  V_MACHINE_NOW  m1,V_MACHINE_NOW m2,S_DIRECT_CON_STATUS ds   ")
                    sBuilder.AppendLine("                      where(V_CON_STATUS.STATION_CODE = m1.RAIL_SECTION_CODE + m1.STATION_ORDER_CODE)   ")
                    sBuilder.AppendLine("   		and V_CON_STATUS.CORNER_CODE = m1.CORNER_CODE  and V_CON_STATUS.MODEL_CODE = m1.MODEL_CODE    ")
                    sBuilder.AppendLine("   		and V_CON_STATUS.UNIT_NO = m1.UNIT_NO    ")
                    sBuilder.AppendLine("   		and m1.MONITOR_ADDRESS = m2.ADDRESS    ")
                    sBuilder.AppendLine("   		and m2.MODEL_CODE = 'W'    ")
                    sBuilder.AppendLine("   		and m2.RAIL_SECTION_CODE = ds.RAIL_SECTION_CODE    ")
                    sBuilder.AppendLine("   		and m2.STATION_ORDER_CODE = ds.STATION_ORDER_CODE    ")
                    sBuilder.AppendLine("   		and m2.MODEL_CODE = ds.MODEL_CODE    ")
                    sBuilder.AppendLine("   		and m2.CORNER_CODE = ds.CORNER_CODE    ")
                    sBuilder.AppendLine("   		and m2.UNIT_NO = ds.UNIT_NO    ")
                    sBuilder.AppendLine("           and ds.PORT_KBN='1' ")
                    sBuilder.AppendLine("   	) as CT   ")
                    sBuilder.AppendLine("   from V_CON_STATUS  where V_CON_STATUS.MODEL_CODE ='G' ) ds  ")
                    '---------駅単位検索対応　　START------------------------------
                    sBuilder.AppendLine(" ) as SELECTDATA ")
                    '---------駅単位検索対応　　END--------------------------------
                    '-----------Ver0.1　運管と切断対応　MOD END----------------------------------------------------------------------------
            End Select

            'Where句生成--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine("")
            sSqlWhere.AppendLine(" Where 0 = 0 ")

            '駅名
            If Not (cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sEki = cmbEki.SelectedValue.ToString
                If sEki.Substring(0, 3).Equals(LcstEkiSentou) Then
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE in {0})", _
                                                       String.Format("(SELECT DISTINCT(RAIL_SECTION_CODE + STATION_ORDER_CODE) AS STATION_CODE" _
                                                                     & " FROM M_MACHINE WHERE BRANCH_OFFICE_CODE = {0}) ", _
                                                                     Utility.SetSglQuot(sEki.Substring(sEki.Length - 3, 3)))))
                Else
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE = {0})", Utility.SetSglQuot(cmbEki.SelectedValue.ToString)))
                End If
            End If
            'コーナー名
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format("and (CORNER_CODE={0})", _
                                                   Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If

            If slcSQLType.Equals(slcSQLType.SlcDetail) Then
                sSqlWhere.AppendLine(" ORDER BY KAIDENGEN1 ,KANSICONNECT1 ,SHUSECONNECT1 ,HAISINSYUCONNECT1 ,HAISINICMCONNECT1 ,EXTOKATUCONNECT1,EXTOKATUDLCONNECT1 asc ")
            End If
            'Where句結合
            sBuilder.AppendLine(sSqlWhere.ToString)
            sSql = sBuilder.ToString()

            Debug.Print(sSql)
            Return sSql
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Function


    ''' <summary>
    ''' [一覧設定]
    ''' </summary>
    ''' <param name="dt">設定対象データテーブル</param>
    Private Sub LfSetSheetData(ByVal dt As DataTable)
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try
            If Not (shtMain.DataSource Is Nothing) Then
                shtMain.DataSource = Nothing
                shtMain.MaxRows = 0
            End If
            shtMain.MaxRows = dt.Rows.Count         '抽出件数分の行を一覧に作成
            shtMain.Rows.SetAllRowsHeight(21)       '行高さを揃える
            shtMain.DataSource = dt                 'データをセット
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            dt = Nothing
            wkbMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' [出力処理2]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart2(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 5
        Try

            With XlsReport1
                'ヘッダ編集
                Log.Info("Start printing about [" & sPath & "].")
                ' 帳票ファイル名称を取得
                .FileName = sPath
                ' 帳票の出力処理を開始を宣言
                .Report.Start()
                .Report.File()
                '帳票ファイルシート名称を取得します。
                .Page.Start(LcstXlsSheetName, "1-9999")

                ' 見出し部セルへ見出しデータ出力
                .Cell("B1").Value = lblTitle.Text
                .Cell("M1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("M2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim + "　　　" + OPMGFormConstants.CORNER_STR + cmbMado.Text.Trim
                ' 配信対象のデータ数を取得します
                nRecCnt = shtMain.MaxRows

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtMain.Item(LcstPrntCol(x), y).Text
                    Next
                Next

                '出力処理の終了を宣言
                .Page.End()
                .Report.End()

                ' 帳票のプレビューをモーダルダイアログで起動します。
                PrintViewer.GetDocument(XlsReport1.Document)
                PrintViewer.ShowDialog(Me)
                PrintViewer.Dispose()
                Log.Info("Printing finished.")
            End With
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

    ''' <summary>
    ''' [駅コンボ設定]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetEki() As Boolean
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim bRtn As Boolean = False
        Dim dt As DataTable = Nothing
        Dim oMst As StationMaster
        oMst = New StationMaster
        Try
            oMst.ApplyDate = ApplyDate
            dt = oMst.SelectTable(True, "G,Y")
            dt = oMst.SetAll()
            bRtn = BaseSetMstDtToCmb(dt, cmbEki)
            cmbEki.SelectedIndex = -1
            If cmbEki.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            LfCmbClear(cmbEki)
            LfCmbClear(cmbMado)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
            LbEventStop = False 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [指定コンボ初期化]
    ''' </summary>
    ''' <param name="cmb">対象コンボボックスコントロール</param>
    Private Sub LfCmbClear(ByVal cmb As ComboBox)
        Try
            cmb.DataSource = Nothing
            If cmb.Items.Count > 0 Then cmb.Items.Clear()
        Catch ex As Exception
        End Try
    End Sub

#End Region
End Class