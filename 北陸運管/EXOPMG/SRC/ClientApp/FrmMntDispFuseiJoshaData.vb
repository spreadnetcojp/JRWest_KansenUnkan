' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/04/01  　　 金沢  北陸対応
'                                   (帳票出力でグループNoを判別し帳票を分ける)
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '定数値のみ使用
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO
Imports System.Text

''' <summary>
''' 【不正乗車検出データ確認　画面クラス】
''' </summary>
Public Class FrmMntDispFuseiJoshaData
    Inherits FrmBase

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        LcstSearchCol = {Me.cmbEki, Me.cmbMado, Me.cmbDirection, Me.dtpYmdFrom, Me.dtpHmFrom, Me.dtpYmdTo, Me.dtpHmTo}

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
    Friend WithEvents lblFromTo As System.Windows.Forms.Label
    Friend WithEvents cmbMado As System.Windows.Forms.ComboBox
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents lblDirection As System.Windows.Forms.Label
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents dtpYmdFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpHmFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents cmbDirection As System.Windows.Forms.ComboBox
    Friend WithEvents dtpHmTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpYmdTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents pnlMado As System.Windows.Forms.Panel
    Friend WithEvents pnlDirection As System.Windows.Forms.Panel
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents lblFromTo2 As System.Windows.Forms.Label
    Friend WithEvents pnlFromTo As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispFuseiJoshaData))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.wkbMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.lblFromTo = New System.Windows.Forms.Label()
        Me.cmbDirection = New System.Windows.Forms.ComboBox()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.lblDirection = New System.Windows.Forms.Label()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.dtpYmdFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpHmFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpHmTo = New System.Windows.Forms.DateTimePicker()
        Me.dtpYmdTo = New System.Windows.Forms.DateTimePicker()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.pnlMado = New System.Windows.Forms.Panel()
        Me.pnlDirection = New System.Windows.Forms.Panel()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.pnlFromTo = New System.Windows.Forms.Panel()
        Me.lblFromTo2 = New System.Windows.Forms.Label()
        Me.pnlBodyBase.SuspendLayout()
        Me.wkbMain.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlEki.SuspendLayout()
        Me.pnlMado.SuspendLayout()
        Me.pnlDirection.SuspendLayout()
        Me.pnlFromTo.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.pnlFromTo)
        Me.pnlBodyBase.Controls.Add(Me.pnlDirection)
        Me.pnlBodyBase.Controls.Add(Me.pnlMado)
        Me.pnlBodyBase.Controls.Add(Me.pnlEki)
        Me.pnlBodyBase.Controls.Add(Me.wkbMain)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnKensaku)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/08/31(土)  17:18"
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
        Me.wkbMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wkbMain.Controls.Add(Me.shtMain)
        Me.wkbMain.Location = New System.Drawing.Point(13, 84)
        Me.wkbMain.Name = "wkbMain"
        Me.wkbMain.ProcessTabKey = False
        Me.wkbMain.ShowTabs = False
        Me.wkbMain.Size = New System.Drawing.Size(988, 481)
        Me.wkbMain.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wkbMain.TabIndex = 94
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(1, 1)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(969, 462)
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
        Me.btnPrint.TabIndex = 5
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'lblFromTo
        '
        Me.lblFromTo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFromTo.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFromTo.Location = New System.Drawing.Point(279, 6)
        Me.lblFromTo.Name = "lblFromTo"
        Me.lblFromTo.Size = New System.Drawing.Size(37, 20)
        Me.lblFromTo.TabIndex = 93
        Me.lblFromTo.Text = "から"
        Me.lblFromTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbDirection
        '
        Me.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDirection.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbDirection.ItemHeight = 13
        Me.cmbDirection.Items.AddRange(New Object() {"", "ＸＸＸ", "ＸＸＸ", "ＸＸＸ", "ＸＸＸ", "ＸＸＸ", "ＸＸＸ", "ＸＸＸ", "ＸＸＸ", "ＸＸＸ", "ＸＸＸ"})
        Me.cmbDirection.Location = New System.Drawing.Point(67, 7)
        Me.cmbDirection.Name = "cmbDirection"
        Me.cmbDirection.Size = New System.Drawing.Size(80, 21)
        Me.cmbDirection.TabIndex = 2
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
        Me.cmbMado.TabIndex = 1
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbEki.Location = New System.Drawing.Point(44, 6)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(162, 21)
        Me.cmbEki.TabIndex = 0
        '
        'lblFrom
        '
        Me.lblFrom.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFrom.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFrom.Location = New System.Drawing.Point(4, 6)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(64, 20)
        Me.lblFrom.TabIndex = 92
        Me.lblFrom.Text = "開始日時"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDirection
        '
        Me.lblDirection.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblDirection.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDirection.Location = New System.Drawing.Point(3, 7)
        Me.lblDirection.Name = "lblDirection"
        Me.lblDirection.Size = New System.Drawing.Size(64, 21)
        Me.lblDirection.TabIndex = 91
        Me.lblDirection.Text = "通路方向"
        Me.lblDirection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMado
        '
        Me.lblMado.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMado.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMado.Location = New System.Drawing.Point(3, 6)
        Me.lblMado.Name = "lblMado"
        Me.lblMado.Size = New System.Drawing.Size(64, 21)
        Me.lblMado.TabIndex = 90
        Me.lblMado.Text = "コーナー"
        Me.lblMado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblEki
        '
        Me.lblEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblEki.Location = New System.Drawing.Point(4, 6)
        Me.lblEki.Name = "lblEki"
        Me.lblEki.Size = New System.Drawing.Size(39, 21)
        Me.lblEki.TabIndex = 89
        Me.lblEki.Text = "駅名"
        Me.lblEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(873, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 6
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(873, 33)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 4
        Me.btnKensaku.Text = "検　索"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'dtpYmdFrom
        '
        Me.dtpYmdFrom.Location = New System.Drawing.Point(68, 6)
        Me.dtpYmdFrom.Name = "dtpYmdFrom"
        Me.dtpYmdFrom.Size = New System.Drawing.Size(139, 20)
        Me.dtpYmdFrom.TabIndex = 3
        '
        'dtpHmFrom
        '
        Me.dtpHmFrom.Checked = False
        Me.dtpHmFrom.Location = New System.Drawing.Point(213, 6)
        Me.dtpHmFrom.Name = "dtpHmFrom"
        Me.dtpHmFrom.ShowUpDown = True
        Me.dtpHmFrom.Size = New System.Drawing.Size(60, 20)
        Me.dtpHmFrom.TabIndex = 4
        '
        'dtpHmTo
        '
        Me.dtpHmTo.Location = New System.Drawing.Point(540, 6)
        Me.dtpHmTo.Name = "dtpHmTo"
        Me.dtpHmTo.ShowUpDown = True
        Me.dtpHmTo.Size = New System.Drawing.Size(60, 20)
        Me.dtpHmTo.TabIndex = 6
        '
        'dtpYmdTo
        '
        Me.dtpYmdTo.Location = New System.Drawing.Point(393, 6)
        Me.dtpYmdTo.Name = "dtpYmdTo"
        Me.dtpYmdTo.Size = New System.Drawing.Size(141, 20)
        Me.dtpYmdTo.TabIndex = 5
        '
        'pnlEki
        '
        Me.pnlEki.Controls.Add(Me.cmbEki)
        Me.pnlEki.Controls.Add(Me.lblEki)
        Me.pnlEki.Location = New System.Drawing.Point(9, 8)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(226, 33)
        Me.pnlEki.TabIndex = 0
        '
        'pnlMado
        '
        Me.pnlMado.Controls.Add(Me.cmbMado)
        Me.pnlMado.Controls.Add(Me.lblMado)
        Me.pnlMado.Location = New System.Drawing.Point(241, 8)
        Me.pnlMado.Name = "pnlMado"
        Me.pnlMado.Size = New System.Drawing.Size(251, 33)
        Me.pnlMado.TabIndex = 1
        '
        'pnlDirection
        '
        Me.pnlDirection.Controls.Add(Me.lblDirection)
        Me.pnlDirection.Controls.Add(Me.cmbDirection)
        Me.pnlDirection.Location = New System.Drawing.Point(498, 8)
        Me.pnlDirection.Name = "pnlDirection"
        Me.pnlDirection.Size = New System.Drawing.Size(168, 33)
        Me.pnlDirection.TabIndex = 2
        '
        'lblTo
        '
        Me.lblTo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTo.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTo.Location = New System.Drawing.Point(328, 6)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(65, 20)
        Me.lblTo.TabIndex = 93
        Me.lblTo.Text = "終了日時"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlFromTo
        '
        Me.pnlFromTo.Controls.Add(Me.lblFromTo2)
        Me.pnlFromTo.Controls.Add(Me.lblFrom)
        Me.pnlFromTo.Controls.Add(Me.lblFromTo)
        Me.pnlFromTo.Controls.Add(Me.lblTo)
        Me.pnlFromTo.Controls.Add(Me.dtpYmdFrom)
        Me.pnlFromTo.Controls.Add(Me.dtpHmTo)
        Me.pnlFromTo.Controls.Add(Me.dtpHmFrom)
        Me.pnlFromTo.Controls.Add(Me.dtpYmdTo)
        Me.pnlFromTo.Location = New System.Drawing.Point(9, 47)
        Me.pnlFromTo.Name = "pnlFromTo"
        Me.pnlFromTo.Size = New System.Drawing.Size(656, 31)
        Me.pnlFromTo.TabIndex = 3
        '
        'lblFromTo2
        '
        Me.lblFromTo2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFromTo2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFromTo2.Location = New System.Drawing.Point(606, 6)
        Me.lblFromTo2.Name = "lblFromTo2"
        Me.lblFromTo2.Size = New System.Drawing.Size(37, 20)
        Me.lblFromTo2.TabIndex = 94
        Me.lblFromTo2.Text = "まで"
        Me.lblFromTo2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmMntDispFuseiJoshaData
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispFuseiJoshaData"
        Me.Text = "運用端末 V1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wkbMain.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlEki.ResumeLayout(False)
        Me.pnlMado.ResumeLayout(False)
        Me.pnlDirection.ResumeLayout(False)
        Me.pnlFromTo.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "宣言領域（Private）"

    ''' <summary>
    ''' 初期処理呼出判定
    ''' （True:初期処理呼出済み、False:初期処理未呼出(Form_Load内で初期処理実施)）
    ''' </summary>
    Private LbInitCallFlg As Boolean = True

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean
    '-------Ver0.1　北陸対応　ADD START---------
    'グループ番号
    Private GrpNo As Integer = 0

    '-------Ver0.1　北陸対応　ADD END-----------

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private LcstXlsSheetName As String = "不正乗車検出データ"


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
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {6, -1, -1, -1, -1, -1, -1, -1, -1}

    ''' <summary>
    ''' 検索条件によって、検索ボタン活性化
    ''' </summary>
    Private LcstSearchCol() As Control

    '適用開始日
    Private sApplyDate As String = Now.ToString("yyyyMMdd")
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
    ''' Title情報
    ''' </summary>
    Private Const FormTitle As String = "不正乗車検出データ確認"

    ''' <summary>
    ''' 帳票Title情報
    ''' </summary>
    Private Const FormTitle2 As String = "不正乗車検出データ"

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

            '画面タイトル
            lblTitle.Text = FormTitle

            'シート初期化
            shtMain.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row                      '行選択モード
            shtMain.MaxRows() = 0                                               '行の初期化
            LcstMaxColCnt = shtMain.MaxColumns()                                '列数を取得
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード
            'シートのヘッダ選択イベントのハンドラ追加
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtMain.ColumnHeaders.HeaderClick, AddressOf Me.shtMainColumnHeaders_HeadersClick

            'コントロールの初期化（共通設定）
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

            '各コンボボックスの項目登録()
            If LfSetEki() = False Then Exit Try '駅名コンボボックス設定
            cmbEki.SelectedIndex = 0            'デフォルト表示項目
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try 'コーナーコンボボックス設定
            cmbMado.SelectedIndex = 0           'デフォルト表示項目
            If LfSetDirection() = False Then Exit Try '通路方向コンボボックス設定
            cmbDirection.SelectedIndex = 0      'デフォルト表示項目

            '一覧ソートの初期化()
            LfClrList()
            LbEventStop = False 'イベント発生ＯＮ
            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
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
    Private Sub FrmMntDispFuseiJoshaData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrm() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            '開始日付、開始時刻、終了日付、終了時刻のコントロール初期化
            LbEventStop = True              'イベント発生ＯＦＦ
            LfSetDateFromTo()               'Loadされないと開始時間の00:00が設定されない為、ここで設定しています。
            LbEventStop = False             'イベント発生ＯＮ

            '検索ボタン活性化
            LfSearchTrue()

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
    Private Sub BtnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnReturn.Click
        LogOperation(sender, e)    'ボタン押下ログ
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
            LbEventStop = True
            LogOperation(sender, e)    'ボタン押下ログ
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
            Log.Fatal("Unwelcome Exception caught.", ex)        '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ
            btnReturn.Select()
        Finally
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
            '-------Ver0.1　北陸対応　ADD START---------
            sPath = Path.Combine(sPath, Config.FuseiJoshaPrintList(GrpNo).ToString)
            LcstXlsSheetName = Config.FuseiJoshaPrintList(GrpNo).ToString.Substring(0, Config.FuseiJoshaPrintList(GrpNo).ToString.Length - 4)
            '-------Ver0.1　北陸対応　ADD END---------
            If File.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If
            '出力
            LfXlsStart(sPath)
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

    ''' <summary>
    ''' 駅コンボ
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            '-------Ver0.1　北陸対応　ADD START---------
            'グループNoを取得
            Dim station As String = cmbEki.SelectedValue.ToString
            If station <> "" And station <> ClientDaoConstants.TERMINAL_ALL Then
                GrpNo = CInt(station.Substring(0, 1))
            ElseIf station = ClientDaoConstants.TERMINAL_ALL Then
                GrpNo = ClientDaoConstants.TERMINAL_ALL_GrpNo
            End If
            '-------Ver0.1　北陸対応　ADD END---------
            'コーナーコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If LfSetMado(station.Substring(station.Length - 6, 6)) = False Then
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                If cmbMado.Enabled = True Then BaseCtlDisabled(pnlMado, False)
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbMado.SelectedIndex = 0               '★イベント発生箇所
            If cmbMado.Enabled = False Then BaseCtlEnabled(pnlMado)
            If btnKensaku.Enabled = False Then btnKensaku.Enabled = True
            LfSearchTrue()
        Catch ex As Exception
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' コーナーコンボ
    ''' </summary>
    Private Sub cmbMado_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbMado.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            LfSearchTrue()
        Catch ex As Exception
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 通路方向コンボ
    ''' </summary>
    Private Sub cmbDirection_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmbDirection.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
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

    ''' <summary>
    ''' 開始日時（年月日）,開始日時（時分）,終了日時（年月日）,終了日時（時分）
    ''' </summary>
    Private Sub dtpYmdFrom_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles dtpYmdFrom.ValueChanged, dtpHmFrom.ValueChanged, dtpYmdTo.ValueChanged, dtpHmTo.ValueChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            LfSearchTrue()
        Catch ex As Exception
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////ElTable関連

    Private Sub shtMainColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)
        Static intCurrentSortColumn As Integer = -1
        Static bolColumn1SortOrder(63) As Boolean

        Try
            If LcstSortCol(e.Column) = -1 Then Exit Sub

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

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            shtMain.EndUpdate()
        End Try
    End Sub
    ''' <summary>
    ''' MouseMove
    ''' </summary>
    Private Sub shtMain_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
        Try
            'マウスカーソルが列ヘッダ上にある場合
            If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtMain.CrossCursor = Cursors.Default
            Else
                'マウスカーソルを既定に戻す
                shtMain.CrossCursor = Nothing
            End If
        Catch ex As Exception
        End Try
    End Sub
    ''' <summary>
    ''' ソート
    ''' </summary>
    Private Sub SheetSort(ByRef sheetTarget As GrapeCity.Win.ElTabelleSheet.Sheet, ByVal intKeyColumn As Integer,
                          ByVal sortOrder As GrapeCity.Win.ElTabelleSheet.SortOrder)
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
    ''' [駅コンボ設定]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetEki() As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As StationMaster
        oMst = New StationMaster
        Try
            oMst.ApplyDate = ApplyDate
            '-------Ver0.1　北陸対応　ADD START---------
            dt = oMst.SelectTable(True, "G", True)
            '-------Ver0.1　北陸対応　ADD END---------
            dt = oMst.SetAll()
            bRtn = BaseSetMstDtToCmb(dt, cmbEki)
            cmbEki.SelectedIndex = -1
            If cmbEki.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function
    ''' <summary>
    ''' [コーナーコンボ設定]
    ''' </summary>
    ''' <param name="Station">駅コード</param>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetMado(ByVal Station As String) As Boolean

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
                dt = oMst.SelectTable(Station, "G")
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
        End Try
        Return bRtn
    End Function
    ''' <summary>
    ''' [通路方向コンボ設定]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetDirection() As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As DirectionMaster
        oMst = New DirectionMaster
        Try
            dt = oMst.SelectTable()
            bRtn = BaseSetMstDtToCmb(dt, cmbDirection)
            cmbDirection.SelectedIndex = -1
            If cmbDirection.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [開始終了日時設定]
    ''' </summary>
    Private Sub LfSetDateFromTo()
        Dim dtWork As DateTime = DateAdd(DateInterval.Day, -1, Today)
        Dim dtFrom As New DateTime(dtWork.Year, dtWork.Month, dtWork.Day, 0, 0, 0)
        Dim dtTo As DateTime = Now
        dtpYmdFrom.Format = DateTimePickerFormat.Custom
        dtpYmdFrom.CustomFormat = "yyyy年MM月dd日"
        dtpYmdFrom.Value = dtFrom
        dtpHmFrom.Format = DateTimePickerFormat.Custom
        dtpHmFrom.CustomFormat = "HH:mm"
        dtpHmFrom.Value = dtFrom
        dtpYmdTo.Format = DateTimePickerFormat.Custom
        dtpYmdTo.CustomFormat = "yyyy年MM月dd日"
        dtpYmdTo.Value = dtTo
        dtpHmTo.Format = DateTimePickerFormat.Custom
        dtpHmTo.CustomFormat = "HH:mm"
        dtpHmTo.Value = dtTo
    End Sub

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
        Dim bEnabled As Boolean
        Dim bFlg As Boolean = True
        Dim sFrom As String = String.Format("{0} {1}", dtpYmdFrom.Text, dtpHmFrom.Text)
        Dim sTo As String = String.Format("{0} {1}", dtpYmdTo.Text, dtpHmTo.Text)
        For Each control As Control In LcstSearchCol
            If control.Enabled = False Then
                bFlg = False
                Exit For
            End If
        Next
        If sFrom > sTo Then
            bEnabled = False
        Else
            bEnabled = True
        End If
        If bEnabled Then
            If ((cmbEki.SelectedIndex < 0) OrElse _
                (cmbMado.SelectedIndex < 0) OrElse _
                (cmbDirection.SelectedIndex < 0)) Then
                bEnabled = False
            End If
        End If
        If bFlg And bEnabled Then
            If btnKensaku.Enabled = False Then btnKensaku.Enabled = True
        Else
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
        End If

    End Sub


    ''' <summary>
    ''' [検索用SELECT文字列取得]
    ''' </summary>
    ''' <returns>SELECT文</returns>
    Private Function LfGetSelectString(ByVal slcSQLType As SlcSQLType) As String

        Dim sSql As String = ""
        Try
            Dim sSqlWhere As New StringBuilder
            Dim sFrom As String
            Dim sTo As String
            Dim sBuilder As New StringBuilder
            Dim sEki As String

            sBuilder.AppendLine("")
            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '件数取得項目--------------------------
                    sBuilder.AppendLine(" SELECT COUNT(1) FROM V_FUSEI_JOSHA_DATA ")
                Case slcSQLType.SlcDetail
                    '取得項目--------------------------
                    sBuilder.AppendLine(" SELECT STATION_NAME, CORNER_NAME,UNIT_NO,PASSAGE_NAME, ")
                    sBuilder.AppendLine(" WRANG_TARGET_NAME,count(*),STATION_CODE,WRANG_TARGET_NO,CORNER_CODE ")
                    sBuilder.AppendLine(" FROM V_FUSEI_JOSHA_DATA ")
            End Select

            'Where句生成--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine("")
            sSqlWhere.AppendLine(" where 0 = 0 ")
            '-------Ver0.1　北陸対応　MOD START---------
            '駅
            If Not (cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sEki = cmbEki.SelectedValue.ToString
                If sEki.Substring(1, 3).Equals(LcstEkiSentou) Then
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE in {0})", _
                                                       String.Format("(SELECT DISTINCT(RAIL_SECTION_CODE + STATION_ORDER_CODE) AS STATION_CODE" _
                                                                     & " FROM M_MACHINE WHERE BRANCH_OFFICE_CODE = {0}) ", _
                                                                     Utility.SetSglQuot(sEki.Substring(sEki.Length - 3, 3)))))
                Else
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE = {0})", Utility.SetSglQuot(sEki.Substring(sEki.Length - 6, 6))))
                End If
            End If
            '-------Ver0.1　北陸対応　MOD END---------
            'コーナー
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (CORNER_CODE = {0})", _
                                          Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If
            '通路方向
            If Not (cmbDirection.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (PASSAGE_FLG = {0})", _
                                          Utility.SetSglQuot(cmbDirection.SelectedValue.ToString)))
            End If
            '開始終了日時
            sFrom = Replace(Replace(Replace(dtpYmdFrom.Text, "年", ""), "月", ""), "日", "") + _
                    Replace(dtpHmFrom.Text, ":", "") + "00"
            sTo = Replace(Replace(Replace(dtpYmdTo.Text, "年", ""), "月", ""), "日", "") + _
                  Replace(dtpHmTo.Text, ":", "") + "59"
            sSqlWhere.AppendLine(String.Format(" And (PROCESSING_TIME >= {0} And PROCESSING_TIME <= {1})", _
                                      Utility.SetSglQuot(sFrom), _
                                      Utility.SetSglQuot(sTo)))

            If slcSQLType.Equals(slcSQLType.SlcDetail) Then

                'Group by句生成
                sSqlWhere.AppendLine(" group by STATION_NAME, CORNER_NAME,UNIT_NO,PASSAGE_NAME,WRANG_TARGET_NAME,STATION_CODE,WRANG_TARGET_NO,CORNER_CODE ")
                'Order by句生成
                sSqlWhere.AppendLine(" order by STATION_CODE, CORNER_CODE asc ")
            End If

            'Where句結合()
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

            shtMain.MaxRows = dt.Rows.Count         '抽出件数分の行を一覧に作成
            shtMain.Rows.SetAllRowsHeight(21)       '行高さを揃える
            shtMain.DataSource = dt                 'データをセット

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            wkbMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 8
        Dim Count As Integer = 0

        Try
            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                ' 帳票ファイル名称を取得
                .FileName = sPath
                ' 帳票の出力処理を開始を宣言
                .Report.Start()
                .Report.File()
                '帳票ファイルシート名称を取得します。
                .Page.Start(LcstXlsSheetName, "1-9999")

                ' 見出し部セルへ見出しデータ出力
                .Cell("B1").Value = FormTitle2
                '-------Ver0.1　北陸対応　MOD START---------
                .Cell("P1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("P2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                '-------Ver0.1　北陸対応　MOD END---------
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim + "　　　" +
                                    OPMGFormConstants.CORNER_STR + cmbMado.Text.Trim _
                                    + "　　" + Lexis.PassageInfo.Gen(cmbDirection.Text.Trim)
                .Cell("C4").Value = Lexis.TimeSpan.Gen( _
                                                  Replace(Replace(Replace(dtpYmdFrom.Text, "年", "/"), "月", "/"), "日", ""), _
                                                  dtpHmFrom.Text, _
                                                  Replace(Replace(Replace(dtpYmdTo.Text, "年", "/"), "月", "/"), "日", ""), _
                                                  dtpHmTo.Text)

                ' 配信対象のデータ数を取得します
                nRecCnt = shtMain.MaxRows

                'データ集計
                Dim lstReportData As List(Of String())
                lstReportData = New List(Of String())
                '
                Dim strPreKey As String = ""
                For i As Integer = 0 To shtMain.MaxRows - 1
                    Dim strNowKey As String = shtMain.Item(0, i).Text & "-" _
                                              & shtMain.Item(1, i).Text & "-" _
                                              & shtMain.Item(2, i).Text & "-" _
                                              & shtMain.Item(3, i).Text
                    If strNowKey <> strPreKey Then
                        strPreKey = strNowKey
                        'new Line
                        '-------Ver0.1　北陸対応　MOD START---------
                        Dim strsLineData(13) As String
                        '-------Ver0.1　北陸対応　MOD ENT---------
                        strsLineData(0) = shtMain.Item(0, i).Text '駅名
                        strsLineData(1) = shtMain.Item(1, i).Text  'コーナー
                        strsLineData(2) = shtMain.Item(2, i).Text  '号機
                        strsLineData(3) = shtMain.Item(3, i).Text  '通路方向
                        lstReportData.Add(strsLineData)
                    End If

                    '特急券
                    If shtMain.Item(7, i).Text = "1" Then
                        lstReportData(lstReportData.Count - 1)(4) = shtMain.Item(5, i).Text
                    End If
                    '入場券
                    If shtMain.Item(7, i).Text = "2" Then
                        lstReportData(lstReportData.Count - 1)(5) = shtMain.Item(5, i).Text
                    End If
                    '回数券
                    If shtMain.Item(7, i).Text = "3" Then
                        lstReportData(lstReportData.Count - 1)(6) = shtMain.Item(5, i).Text
                    End If
                    '定期券
                    If shtMain.Item(7, i).Text = "4" Then
                        lstReportData(lstReportData.Count - 1)(7) = shtMain.Item(5, i).Text
                    End If
                    '長時間
                    If shtMain.Item(7, i).Text = "7" Then
                        lstReportData(lstReportData.Count - 1)(8) = shtMain.Item(5, i).Text
                    End If
                    '短入出
                    If shtMain.Item(7, i).Text = "8" Then
                        lstReportData(lstReportData.Count - 1)(9) = shtMain.Item(5, i).Text
                    End If
                    '短出入
                    If shtMain.Item(7, i).Text = "9" Then
                        lstReportData(lstReportData.Count - 1)(10) = shtMain.Item(5, i).Text
                    End If
                    '改札
                    If shtMain.Item(7, i).Text = "5" Then
                        lstReportData(lstReportData.Count - 1)(11) = shtMain.Item(5, i).Text
                    End If
                    '集札
                    If shtMain.Item(7, i).Text = "6" Then
                        lstReportData(lstReportData.Count - 1)(12) = shtMain.Item(5, i).Text
                    End If
                    '-------Ver0.1　北陸対応　ADD START---------
                    '時間超過
                    If shtMain.Item(7, i).Text = "10" Then
                        lstReportData(lstReportData.Count - 1)(13) = shtMain.Item(5, i).Text
                    End If
                    '-------Ver0.1　北陸対応　ADD END---------
                Next

                For i As Integer = 1 To lstReportData.Count - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                For i As Integer = 0 To lstReportData.Count - 1
                    '-------Ver0.1　北陸対応　MOD START---------
                    For j As Integer = 0 To 13
                        '-------Ver0.1　北陸対応　MOD END---------
                        .Pos(1 + j, i + nStartRow).Value = lstReportData(i)(j)
                    Next
                Next

                ' 印刷範囲の設定（座標指定：開始列，開始行，終了列，終了行）
                ' .Page.Attr.PrintArea(0, 0, 25, nRecCnt + nStartRow - 1)
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
#End Region

End Class
