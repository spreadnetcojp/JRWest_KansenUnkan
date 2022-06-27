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
''' 【異常データ確認　画面クラス】
''' </summary>
Public Class FrmMntDispFaultData
    Inherits FrmBase

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        LcstSearchCol = {Me.cmbEki, Me.cmbMado, Me.cmbKisyu, Me.cmbGouki, Me.dtpYmdFrom, Me.dtpHmFrom, Me.dtpYmdTo, Me.dtpHmTo}

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
    Friend WithEvents lblKisyu As System.Windows.Forms.Label
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents cmbKisyu As System.Windows.Forms.ComboBox
    Friend WithEvents cmbGouki As System.Windows.Forms.ComboBox
    Friend WithEvents lblGouki As System.Windows.Forms.Label
    Friend WithEvents cmbErrcd As System.Windows.Forms.ComboBox
    Friend WithEvents lblErrcd As System.Windows.Forms.Label
    Friend WithEvents pnlFromTo As System.Windows.Forms.Panel
    Friend WithEvents lblFromDate As System.Windows.Forms.Label
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents lblToDate As System.Windows.Forms.Label
    Friend WithEvents dtpYmdFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpHmTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpHmFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpYmdTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents pnlMado As System.Windows.Forms.Panel
    Friend WithEvents cmbMado As System.Windows.Forms.ComboBox
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents pnlKisyu As System.Windows.Forms.Panel
    Friend WithEvents pnlGouki As System.Windows.Forms.Panel
    Friend WithEvents pnlErrcd As System.Windows.Forms.Panel
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport

    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispFaultData))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.wkbMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.cmbKisyu = New System.Windows.Forms.ComboBox()
        Me.lblKisyu = New System.Windows.Forms.Label()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.cmbGouki = New System.Windows.Forms.ComboBox()
        Me.lblGouki = New System.Windows.Forms.Label()
        Me.cmbErrcd = New System.Windows.Forms.ComboBox()
        Me.lblErrcd = New System.Windows.Forms.Label()
        Me.pnlFromTo = New System.Windows.Forms.Panel()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.lblFromDate = New System.Windows.Forms.Label()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.lblToDate = New System.Windows.Forms.Label()
        Me.dtpYmdFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpHmTo = New System.Windows.Forms.DateTimePicker()
        Me.dtpHmFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpYmdTo = New System.Windows.Forms.DateTimePicker()
        Me.pnlMado = New System.Windows.Forms.Panel()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.pnlKisyu = New System.Windows.Forms.Panel()
        Me.pnlGouki = New System.Windows.Forms.Panel()
        Me.pnlErrcd = New System.Windows.Forms.Panel()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.wkbMain.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlFromTo.SuspendLayout()
        Me.pnlMado.SuspendLayout()
        Me.pnlEki.SuspendLayout()
        Me.pnlKisyu.SuspendLayout()
        Me.pnlGouki.SuspendLayout()
        Me.pnlErrcd.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.pnlErrcd)
        Me.pnlBodyBase.Controls.Add(Me.pnlGouki)
        Me.pnlBodyBase.Controls.Add(Me.pnlKisyu)
        Me.pnlBodyBase.Controls.Add(Me.pnlFromTo)
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
        Me.lblToday.Text = "2013/08/21(水)  12:51"
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
        Me.wkbMain.Location = New System.Drawing.Point(13, 122)
        Me.wkbMain.Name = "wkbMain"
        Me.wkbMain.ProcessTabKey = False
        Me.wkbMain.ShowTabs = False
        Me.wkbMain.Size = New System.Drawing.Size(988, 442)
        Me.wkbMain.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wkbMain.TabIndex = 8
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(2, 2)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(968, 422)
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
        Me.btnPrint.TabIndex = 11
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'cmbKisyu
        '
        Me.cmbKisyu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKisyu.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKisyu.ItemHeight = 13
        Me.cmbKisyu.Items.AddRange(New Object() {"", "ＸＸＸＸＸ", "ＸＸＸＸＸ", "ＸＸＸＸＸ", "ＸＸＸＸＸ", "ＸＸＸＸＸ", "ＸＸＸＸＸ", "ＸＸＸＸＸ", "ＸＸＸＸＸ", "ＸＸＸＸＸ", "ＸＸＸＸＸ"})
        Me.cmbKisyu.Location = New System.Drawing.Point(38, 6)
        Me.cmbKisyu.MaxLength = 3
        Me.cmbKisyu.Name = "cmbKisyu"
        Me.cmbKisyu.Size = New System.Drawing.Size(126, 21)
        Me.cmbKisyu.TabIndex = 3
        '
        'lblKisyu
        '
        Me.lblKisyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblKisyu.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKisyu.Location = New System.Drawing.Point(3, 6)
        Me.lblKisyu.Name = "lblKisyu"
        Me.lblKisyu.Size = New System.Drawing.Size(36, 21)
        Me.lblKisyu.TabIndex = 0
        Me.lblKisyu.Text = "機種"
        Me.lblKisyu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(873, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 12
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(873, 70)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 10
        Me.btnKensaku.Text = "検　索"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'cmbGouki
        '
        Me.cmbGouki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGouki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbGouki.ItemHeight = 13
        Me.cmbGouki.Items.AddRange(New Object() {"", "XX", "XX", "XX", "XX", "XX", "XX", "XX", "XX", "XX", "XX"})
        Me.cmbGouki.Location = New System.Drawing.Point(43, 7)
        Me.cmbGouki.MaxLength = 2
        Me.cmbGouki.Name = "cmbGouki"
        Me.cmbGouki.Size = New System.Drawing.Size(70, 21)
        Me.cmbGouki.TabIndex = 4
        '
        'lblGouki
        '
        Me.lblGouki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblGouki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblGouki.Location = New System.Drawing.Point(4, 7)
        Me.lblGouki.Name = "lblGouki"
        Me.lblGouki.Size = New System.Drawing.Size(39, 21)
        Me.lblGouki.TabIndex = 0
        Me.lblGouki.Text = "号機"
        Me.lblGouki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbErrcd
        '
        Me.cmbErrcd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple
        Me.cmbErrcd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbErrcd.ItemHeight = 13
        Me.cmbErrcd.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbErrcd.Location = New System.Drawing.Point(102, 7)
        Me.cmbErrcd.Name = "cmbErrcd"
        Me.cmbErrcd.Size = New System.Drawing.Size(645, 21)
        Me.cmbErrcd.TabIndex = 9
        '
        'lblErrcd
        '
        Me.lblErrcd.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblErrcd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblErrcd.Location = New System.Drawing.Point(4, 6)
        Me.lblErrcd.Name = "lblErrcd"
        Me.lblErrcd.Size = New System.Drawing.Size(92, 22)
        Me.lblErrcd.TabIndex = 1
        Me.lblErrcd.Text = "エラーコード"
        Me.lblErrcd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlFromTo
        '
        Me.pnlFromTo.Controls.Add(Me.lblTo)
        Me.pnlFromTo.Controls.Add(Me.lblFromDate)
        Me.pnlFromTo.Controls.Add(Me.lblFrom)
        Me.pnlFromTo.Controls.Add(Me.lblToDate)
        Me.pnlFromTo.Controls.Add(Me.dtpYmdFrom)
        Me.pnlFromTo.Controls.Add(Me.dtpHmTo)
        Me.pnlFromTo.Controls.Add(Me.dtpHmFrom)
        Me.pnlFromTo.Controls.Add(Me.dtpYmdTo)
        Me.pnlFromTo.Location = New System.Drawing.Point(9, 45)
        Me.pnlFromTo.Name = "pnlFromTo"
        Me.pnlFromTo.Size = New System.Drawing.Size(628, 31)
        Me.pnlFromTo.TabIndex = 5
        '
        'lblTo
        '
        Me.lblTo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTo.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTo.Location = New System.Drawing.Point(586, 6)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(37, 20)
        Me.lblTo.TabIndex = 7
        Me.lblTo.Text = "まで"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFromDate
        '
        Me.lblFromDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFromDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFromDate.Location = New System.Drawing.Point(4, 6)
        Me.lblFromDate.Name = "lblFromDate"
        Me.lblFromDate.Size = New System.Drawing.Size(64, 20)
        Me.lblFromDate.TabIndex = 0
        Me.lblFromDate.Text = "開始日時"
        Me.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFrom
        '
        Me.lblFrom.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFrom.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFrom.Location = New System.Drawing.Point(274, 6)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(37, 20)
        Me.lblFrom.TabIndex = 3
        Me.lblFrom.Text = "から"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblToDate
        '
        Me.lblToDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblToDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToDate.Location = New System.Drawing.Point(320, 6)
        Me.lblToDate.Name = "lblToDate"
        Me.lblToDate.Size = New System.Drawing.Size(65, 20)
        Me.lblToDate.TabIndex = 4
        Me.lblToDate.Text = "終了日時"
        Me.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dtpYmdFrom
        '
        Me.dtpYmdFrom.Location = New System.Drawing.Point(68, 6)
        Me.dtpYmdFrom.Name = "dtpYmdFrom"
        Me.dtpYmdFrom.Size = New System.Drawing.Size(140, 20)
        Me.dtpYmdFrom.TabIndex = 5
        '
        'dtpHmTo
        '
        Me.dtpHmTo.Location = New System.Drawing.Point(520, 6)
        Me.dtpHmTo.Name = "dtpHmTo"
        Me.dtpHmTo.ShowUpDown = True
        Me.dtpHmTo.Size = New System.Drawing.Size(60, 20)
        Me.dtpHmTo.TabIndex = 8
        '
        'dtpHmFrom
        '
        Me.dtpHmFrom.Checked = False
        Me.dtpHmFrom.Location = New System.Drawing.Point(208, 6)
        Me.dtpHmFrom.Name = "dtpHmFrom"
        Me.dtpHmFrom.ShowUpDown = True
        Me.dtpHmFrom.Size = New System.Drawing.Size(60, 20)
        Me.dtpHmFrom.TabIndex = 6
        '
        'dtpYmdTo
        '
        Me.dtpYmdTo.Location = New System.Drawing.Point(385, 6)
        Me.dtpYmdTo.Name = "dtpYmdTo"
        Me.dtpYmdTo.Size = New System.Drawing.Size(135, 20)
        Me.dtpYmdTo.TabIndex = 7
        '
        'pnlMado
        '
        Me.pnlMado.Controls.Add(Me.cmbMado)
        Me.pnlMado.Controls.Add(Me.lblMado)
        Me.pnlMado.Location = New System.Drawing.Point(222, 6)
        Me.pnlMado.Name = "pnlMado"
        Me.pnlMado.Size = New System.Drawing.Size(233, 33)
        Me.pnlMado.TabIndex = 2
        '
        'cmbMado
        '
        Me.cmbMado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMado.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMado.ItemHeight = 13
        Me.cmbMado.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbMado.Location = New System.Drawing.Point(67, 6)
        Me.cmbMado.MaxLength = 10
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
        Me.pnlEki.Location = New System.Drawing.Point(9, 6)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(210, 33)
        Me.pnlEki.TabIndex = 1
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbEki.Location = New System.Drawing.Point(45, 6)
        Me.cmbEki.MaxLength = 10
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
        'pnlKisyu
        '
        Me.pnlKisyu.Controls.Add(Me.cmbKisyu)
        Me.pnlKisyu.Controls.Add(Me.lblKisyu)
        Me.pnlKisyu.Location = New System.Drawing.Point(457, 6)
        Me.pnlKisyu.Name = "pnlKisyu"
        Me.pnlKisyu.Size = New System.Drawing.Size(168, 33)
        Me.pnlKisyu.TabIndex = 3
        '
        'pnlGouki
        '
        Me.pnlGouki.Controls.Add(Me.cmbGouki)
        Me.pnlGouki.Controls.Add(Me.lblGouki)
        Me.pnlGouki.Location = New System.Drawing.Point(627, 6)
        Me.pnlGouki.Name = "pnlGouki"
        Me.pnlGouki.Size = New System.Drawing.Size(118, 33)
        Me.pnlGouki.TabIndex = 4
        '
        'pnlErrcd
        '
        Me.pnlErrcd.Controls.Add(Me.cmbErrcd)
        Me.pnlErrcd.Controls.Add(Me.lblErrcd)
        Me.pnlErrcd.Location = New System.Drawing.Point(9, 82)
        Me.pnlErrcd.Name = "pnlErrcd"
        Me.pnlErrcd.Size = New System.Drawing.Size(752, 34)
        Me.pnlErrcd.TabIndex = 6
        '
        'FrmMntDispFaultData
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispFaultData"
        Me.Text = "運用端末 Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wkbMain.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlFromTo.ResumeLayout(False)
        Me.pnlMado.ResumeLayout(False)
        Me.pnlEki.ResumeLayout(False)
        Me.pnlKisyu.ResumeLayout(False)
        Me.pnlGouki.ResumeLayout(False)
        Me.pnlErrcd.ResumeLayout(False)
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
    Private ReadOnly LcstXlsTemplateName As String = "異常データ.xls"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "異常データ"

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly FormTitle As String = "異常データ確認"

    ''' <summary>
    ''' 駅コードの先頭3桁:「000」
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"

    ''' <summary>
    ''' 一覧表示最大列
    ''' </summary>
    Private ReadOnly LcstMaxColCnt As Integer = 12

    ''' <summary>
    ''' 一覧ヘッダのソート列割り当て
    ''' （一覧ヘッダクリック時に割り当てる対象列を定義。列番号はゼロ相対で"-1"はソート対象外の列）
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, 0, -1, -1, -1, 5, -1, -1, -1, 9, -1, -1}

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した異常データに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13}

    ''' <summary>
    ''' 検索条件によって、検索ボタン活性化
    ''' </summary>
    Private LcstSearchCol() As Control

    '検索SQL取得区分
    Private Enum SlcSQLType
        SlcCount = 0  '件数取得用
        SlcDetail = 1 'データ検索用
    End Enum

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
            shtMain.TransformEditor = False '一覧の列種類毎のチェックを無効にする
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row
            shtMain.MaxRows() = 0                                               '行の初期化
            shtMain.MaxColumns() = LcstMaxColCnt
            '行ヘッダの設定
            shtMain.RowHeaders.MaxColumns = 1
            shtMain.RowHeaders.GetColumn(0).Width = 34
            '列ヘッダの設定
            shtMain.ColumnHeaders.MaxRows = 1
            shtMain.ColumnHeaders.GetRow(0).Height = 42

            'シートの表示選択モードを設定する
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtMain.ColumnHeaders.HeaderClick, AddressOf Me.shtMainColumnHeaders_HeadersClick

            '--常時活性化項目設定
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

            '各コンボボックスの項目登録()
            If LfSetEki() = False Then Exit Try '駅名コンボボックス設定
            cmbEki.SelectedIndex = 0            'デフォルト表示項目
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try 'コーナーコンボボックス設定
            cmbMado.SelectedIndex = 0           'デフォルト表示項目
            If LfSetKisyu(cmbEki.SelectedValue.ToString, cmbMado.SelectedValue.ToString) = False Then Exit Try '機種コンボボックス設定
            cmbKisyu.SelectedIndex = 0          'デフォルト表示項目
            If LfSetGouki(cmbEki.SelectedValue.ToString, _
                          cmbMado.SelectedValue.ToString, _
                          cmbKisyu.SelectedValue.ToString) = False Then Exit Try '号機コンボボックス設定
            cmbGouki.SelectedIndex = 0          'デフォルト表示項目

            '一覧ソートの初期化
            LfClrList()

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
    Private Sub FrmMntDispAbnormalData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrm() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If
            LbEventStop = True      'イベント発生ＯＦＦ
            LfSetDateFromTo()       'Loadされないと開始時間の00:00が設定されない為、ここで設定しています。
            LbEventStop = False     'イベント発生ＯＮ

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
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
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
        Dim ErrSts, nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim ErrCdWhere, CmbErrCdTxt As String

        LfWaitCursor()
        Try
            LbEventStop = True
            LogOperation(sender, e)    'ボタン押下ログ

            '初期化処理
            LfClrList()

            '運用管理端末のINIファイルから取得可能件数を取得
            Dim nMaxCount As Integer = Config.MaxUpboundDataToGet

            ErrSts = 0

            '入力文字のチェック
            ErrSts = ErrCdCheck(cmbErrcd.Text.ToString)
            If Not ErrSts = 0 Then
                AlertBox.Show(Lexis.TheInputValueIsUnsuitableForFaultDataErrorCode)
                cmbErrcd.Select()
                Exit Sub
            End If

            '入力データ
            CmbErrCdTxt = cmbErrcd.Text.ToString

            '条件文（エラーコード）を作成する
            ErrCdWhere = "" : ErrCdWhere = ErrCdSelect(CmbErrCdTxt)


            '件数取得チェック
            sSql = LfGetSelectString(SlcSQLType.SlcCount, ErrCdWhere)
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
            sSql = LfGetSelectString(SlcSQLType.SlcDetail, ErrCdWhere)
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
            Log.Fatal("Unwelcome Exception caught.", ex)    '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ
            btnReturn.Select()
        Finally
            'DB開放()
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

        Dim ErrFileName As String = ""
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
            LfXlsStart(sPath)
            cmbEki.Select()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////DoubleClick

    ''' <summary>
    ''' リスト行選択時（ダブルクリック）
    ''' </summary>
    Private Sub shtMain_CellDoubleClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs) _
    Handles shtMain.CellDoubleClick
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Dim hFrmMntDispAbnormalDetail As New FrmMntDispFaultDataDetail
        Dim sErr As String = ""
        Dim dt As New DataTable
        Dim sSql As String = ""
        Try
            LbEventStop = True
            With hFrmMntDispAbnormalDetail
                '取得済み情報の設定
                .setContent(shtMain.Item(1, e.Row).Text, shtMain.Item(2, e.Row).Text, shtMain.Item(3, e.Row).Text, _
                           shtMain.Item(4, e.Row).Text, shtMain.Item(5, e.Row).Text, _
                           shtMain.Item(10, e.Row).Text & "(" & shtMain.Item(9, e.Row).Text & ")", _
                           shtMain.Item(12, e.Row).Text, shtMain.Item(13, e.Row).Text)
                '表示
                If .InitFrm Then
                    .ShowDialog()
                End If
            End With
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dt = Nothing
            hFrmMntDispAbnormalDetail.Dispose()
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
            LbEventStop = True      'イベント発生ＯＦＦ

            'コーナーコンボ設定
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then
                If cmbMado.Enabled = True Then BaseCtlDisabled(pnlMado, False)
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
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
            LbEventStop = True      'イベント発生ＯＦＦ

            '機種コンボ設定
            If LfSetKisyu(cmbEki.SelectedValue.ToString, cmbMado.SelectedValue.ToString) = False Then
                If cmbKisyu.Enabled = True Then BaseCtlDisabled(pnlKisyu, False)
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblKisyu.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbKisyu.SelectedIndex = 0               '★イベント発生箇所
            If cmbKisyu.Enabled = False Then BaseCtlEnabled(pnlKisyu)
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
    ''' 「機種」コンボ
    ''' </summary>
    Private Sub cmbKisyu_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbKisyu.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            LbEventStop = True      'イベント発生ＯＮ

            '号機コンボ設定
            If LfSetGouki(cmbEki.SelectedValue.ToString, _
                          cmbMado.SelectedValue.ToString, _
                          cmbKisyu.SelectedValue.ToString) = False Then
                If cmbGouki.Enabled = True Then BaseCtlDisabled(pnlGouki, False)
                If dtpYmdFrom.Enabled = True Then BaseCtlDisabled(pnlFromTo, False)
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblGouki.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbGouki.SelectedIndex = 0               '★イベント発生箇所

            If cmbGouki.Enabled = False Then BaseCtlEnabled(pnlGouki)

            BaseCtlEnabled(pnlFromTo)
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
    ''' 「号機」コンボ
    ''' </summary>
    Private Sub CmbGouki_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmbGouki.SelectedIndexChanged
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

            If intCurrentSortColumn > -1 Then
                '前回ソートされた列ヘッダのイメージを削除する
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '前回ソートされた列の背景色を初期化する
                shtMain.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '前回ソートされた列のセル罫線を消去する
                shtMain.Columns(intCurrentSortColumn).SetBorder(New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), GrapeCity.Win.ElTabelleSheet.Borders.All)
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
        Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
        'マウスカーソルが列ヘッダ上にある場合
        If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
            shtMain.CrossCursor = Cursors.Default
        Else
            'マウスカーソルを既定に戻す
            shtMain.CrossCursor = Nothing
        End If
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
            dt = oMst.SelectTable(True, "G,W,Y")
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
        Dim sXYRange As String
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
            If shtMain.MaxRows > 0 Then
                sXYRange = "1:" & shtMain.MaxRows.ToString
                shtMain.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
            End If
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
        Dim sFrom As String = String.Format("{0} {1}", dtpYmdFrom.Text, dtpHmFrom.Text)
        Dim sTo As String = String.Format("{0} {1}", dtpYmdTo.Text, dtpHmTo.Text)
        For Each control As Control In LcstSearchCol
            If control.Enabled = False Then
                btnKensaku.Enabled = False
                Exit Sub
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
                (cmbKisyu.SelectedIndex < 0) OrElse _
                (cmbGouki.SelectedIndex < 0)) Then
                bEnabled = False
            End If
        End If
        If bEnabled Then
            If btnKensaku.Enabled = False Then btnKensaku.Enabled = True
        Else
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
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
            If Station <> "" AndAlso Station <> ClientDaoConstants.TERMINAL_ALL Then
                dt = oMst.SelectTable(Station, "G,W,Y")
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
    ''' [機種コンボ設定]
    ''' </summary>
    ''' <param name="Station">駅コード</param>
    ''' <param name="Corner">コーナーコード</param>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetKisyu(ByVal Station As String, ByVal Corner As String) As Boolean
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As ModelMaster
        oMst = New ModelMaster
        Try
            oMst.ApplyDate = ApplyDate
            If String.IsNullOrEmpty(Station) Then
                Station = ""
            End If
            If String.IsNullOrEmpty(Corner) Then
                Corner = ""
            End If
            If ((Station <> "" AndAlso Station <> ClientDaoConstants.TERMINAL_ALL) _
            AndAlso (Corner <> "" AndAlso Corner <> ClientDaoConstants.TERMINAL_ALL)) Then
                dt = oMst.SelectTable(Station, Corner, False, True)
            End If
            dt = oMst.SetAll
            bRtn = BaseSetMstDtToCmb(dt, cmbKisyu)
            cmbKisyu.SelectedIndex = -1
            If cmbKisyu.Items.Count <= 0 Then bRtn = False
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
    ''' [号機コンボ設定]
    ''' </summary>
    ''' <param name="Station">駅コード</param>
    ''' <param name="Corner">コーナーコード</param>
    ''' <param name="Kisyu">機種コード</param>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetGouki(ByVal Station As String, ByVal Corner As String, ByVal Kisyu As String) As Boolean
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As UnitMaster
        oMst = New UnitMaster
        Try
            oMst.ApplyDate = ApplyDate
            If String.IsNullOrEmpty(Station) Then
                Station = ""
            End If
            If String.IsNullOrEmpty(Corner) Then
                Corner = ""
            End If
            If String.IsNullOrEmpty(Kisyu) Then
                Kisyu = ""
            End If
            If ((Station <> "" AndAlso Station <> ClientDaoConstants.TERMINAL_ALL) _
            AndAlso (Corner <> "" AndAlso Corner <> ClientDaoConstants.TERMINAL_ALL) _
            AndAlso (Kisyu <> "" AndAlso Kisyu <> ClientDaoConstants.TERMINAL_ALL)) Then
                dt = oMst.SelectTable(Station, Corner, Kisyu)
            End If
            dt = oMst.SetAll()
            bRtn = BaseSetMstDtToCmb(dt, cmbGouki)
            cmbGouki.SelectedIndex = -1
            If cmbGouki.Items.Count <= 0 Then bRtn = False
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
    Private Function LfGetSelectString(ByVal slcSQLType As SlcSQLType, ByVal ErrCdWhere As String) As String
        Dim sSql As String
        Try
            Dim sSqlWhere As StringBuilder = New StringBuilder()
            Dim sFrom As String
            Dim sTo As String
            Dim sBuilder As New StringBuilder
            Dim sEki As String

            sBuilder.AppendLine("")

            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '件数取得項目--------------------------
                    sBuilder.AppendLine("SELECT COUNT(1) FROM V_FAULT_DATA")

                Case slcSQLType.SlcDetail
                    '取得項目--------------------------
                    sBuilder.AppendLine("SELECT STATION_CODE, STATION_NAME, CORNER_NAME, MODEL_NAME, UNIT_NO,")
                    sBuilder.AppendLine(" SUBSTRING(OCCUR_DATE,1,4)+'/'+SUBSTRING(OCCUR_DATE,5,2)+'/'+SUBSTRING(OCCUR_DATE,7,2)+' '+")
                    sBuilder.AppendLine(" SUBSTRING(OCCUR_DATE,9,2)+':'+SUBSTRING(OCCUR_DATE,11,2)+':'+SUBSTRING(OCCUR_DATE,13,2)")
                    sBuilder.AppendLine(" AS YMDHMS, PASSAGE_NAME, ERROR_TYPE, ACT_STEP, ERR_CODE, ERR_ITEM, ERROR_KIND, DTL_INFO, RES_INFO FROM V_FAULT_DATA")
            End Select

            'Where句生成--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine(" where 0=0")

            '駅
            If Not (cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sEki = cmbEki.SelectedValue.ToString
                If sEki.Substring(0, 3).Equals(LcstEkiSentou) Then
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE in {0})", _
                                                       String.Format("(SELECT DISTINCT(RAIL_SECTION_CODE + STATION_ORDER_CODE) AS STATION_CODE " & _
                                                                     " FROM M_MACHINE WHERE BRANCH_OFFICE_CODE = {0}) ", _
                                                                     Utility.SetSglQuot(sEki.Substring(sEki.Length - 3, 3)))))
                Else
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE = {0})", Utility.SetSglQuot(cmbEki.SelectedValue.ToString)))
                End If
            End If
            'コーナー
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (CORNER_CODE = {0})", _
                                          Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If
            '機種
            If Not (cmbKisyu.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (MODEL_CODE = {0})", _
                                          Utility.SetSglQuot(cmbKisyu.SelectedValue.ToString)))
            End If
            '号機
            If Not (cmbGouki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL OrElse cmbGouki.SelectedValue.ToString = "") Then
                sSqlWhere.AppendLine(String.Format(" and (UNIT_NO = {0})", _
                                          Utility.SetSglQuot(cmbGouki.SelectedValue.ToString)))
            End If
            'エラーコード
            If Not cmbErrcd.Text.ToString = "" Then
                'エラーコードの条件文を追加で結合
                sSqlWhere.AppendLine("and " + ErrCdWhere)
            End If

            '開始終了日時
            sFrom = Replace(Replace(Replace(dtpYmdFrom.Text, "年", ""), "月", ""), "日", "") + _
                    Replace(dtpHmFrom.Text, ":", "") + "00"
            sTo = Replace(Replace(Replace(dtpYmdTo.Text, "年", ""), "月", ""), "日", "") + _
                  Replace(dtpHmTo.Text, ":", "") + "59"

            sSqlWhere.AppendLine(String.Format("and (OCCUR_DATE >= {0}) And (OCCUR_DATE <= {1})", _
                                      Utility.SetSglQuot(sFrom), _
                                      Utility.SetSglQuot(sTo)))


            If slcSQLType.Equals(slcSQLType.SlcDetail) Then
                'Order by句生成
                sSqlWhere.AppendLine(" order by STATION_CODE, CORNER_CODE asc ")
            End If

            'Where句結合
            sSql = sBuilder.ToString + sSqlWhere.ToString

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

        Dim i As Integer
        Dim GyoHight As Integer = 21
        Dim objTextEditor As ElTabelleSheet.Editors.TextEditor

        objTextEditor = New ElTabelleSheet.Editors.TextEditor
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try
            If Not (shtMain.DataSource Is Nothing) Then
                shtMain.DataSource = Nothing
                shtMain.MaxRows = 0
            End If
            shtMain.MaxRows = dt.Rows.Count
            shtMain.Rows.SetAllRowsHeight(21)

            '関係ない列を隠す
            shtMain.DataSource = dt
            If LcstMaxColCnt < dt.Columns.Count Then
                For i = LcstMaxColCnt To dt.Columns.Count - 1
                    shtMain.Columns(i).Hidden = True                                'この行をコメントアウトするとSelect結果全ての行が見えます
                Next i
            End If

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
    ''' エラーコードの条件文を作る。
    ''' </summary>
    ''' <param name="InputErrCd">対象文字列</param>
    ''' <returns>エラーコード条件文</returns>
    Private Function ErrCdSelect(ByVal InputErrCd As String) As String

        Dim sInputErrCdDel As String = ""
        Dim strErr() As String
        Dim strRange() As String
        Dim sSqlErrWhere As String
        Dim nCnt As Integer = 0
        Dim nCnt1 As Integer = 0
        Dim nCntBf As Integer = 0
        Dim nCntAf As Integer = 0
        Dim i As Integer

        '「,」を抜いた文字列を取得
        sInputErrCdDel = InputErrCd.Replace(",", "")

        'それぞれの文字数を取得
        nCntBf = InputErrCd.Length
        nCntAf = sInputErrCdDel.Length

        'コンマの数を取得
        nCnt = nCntBf - nCntAf

        '文字列を分割
        strErr = InputErrCd.Split(CChar(","))

        '条件分作成（where分作成）
        sSqlErrWhere = ""
        '「 "(" 」を付ける処理
        sSqlErrWhere = sSqlErrWhere + "("
        For i = 0 To nCnt

            '「-」を抜いた文字列を取得
            sInputErrCdDel = strErr(i).Replace("-", "")

            'それぞれの文字数を取得
            nCntBf = strErr(i).Length
            nCntAf = sInputErrCdDel.Length

            '-の数を取得
            nCnt1 = nCntBf - nCntAf

            '文字列を分割
            strRange = strErr(i).Split(CChar("-"))

            If i > 0 Then
                sSqlErrWhere = sSqlErrWhere + " OR "
            End If

            If nCnt1 > 0 Then
                sSqlErrWhere = sSqlErrWhere + String.Format("(ERR_CODE >= '{0}' And ERR_CODE <= '{1}')", strRange(0), strRange(1))
                If i = nCnt Then
                    sSqlErrWhere = sSqlErrWhere + ")"
                End If
            Else
                '「 ")" 」を付ける処理
                If i = nCnt Then
                    sSqlErrWhere = sSqlErrWhere + String.Format("(ERR_CODE LIKE '{0}')", strErr(i))
                    sSqlErrWhere = sSqlErrWhere + ")"
                Else
                    sSqlErrWhere = sSqlErrWhere + String.Format("(ERR_CODE LIKE '{0}')", strErr(i))
                End If
            End If
        Next
        Return sSqlErrWhere

    End Function

    ''' <summary>
    ''' 入力された文字のチェックを行う。
    ''' </summary>
    ''' <param name="InputErrCd"></param>
    ''' <returns>合否ステータス</returns>
    Private Function ErrCdCheck(ByVal InputErrCd As String) As Integer

        Dim sInputErrCdDel As String = ""
        Dim nCnt As Integer = 0
        Dim nCntBf As Integer = 0
        Dim nCntAf As Integer = 0
        Dim nStrLen As Integer = 0
        Dim nStrPnt As Integer = 1
        Dim strMoji As String
        Dim sjisEnc As Encoding = Encoding.GetEncoding("Shift_JIS")

        Dim strErr() As String
        Dim i As Integer
        Dim j As Integer
        Dim nCnt1 As Integer
        Dim strRange() As String

        '初期化＆設定
        nStrLen = InputErrCd.Length
        nStrPnt = 1
        ErrCdCheck = 0

        Try
            '入力チェック処理
            Do
                '文字列の検知終了
                If nStrPnt > nStrLen Then
                    Exit Do
                End If

                '1文字を取得
                strMoji = ""
                strMoji = Mid(InputErrCd, nStrPnt, 1)

                '全角がある場合に異常発生
                If sjisEnc.GetByteCount(strMoji) = 1 Then
                    '英数字、あるいは「,」、「-」以外の文字がある場合に異常発生
                    If (Not Char.IsLetterOrDigit(strMoji, 0) = True) AndAlso _
                        (Not strMoji = ",") AndAlso _
                        (Not strMoji = "-") Then
                        ErrCdCheck = 2          '指定以外の文字入力異常
                        Exit Function
                    End If
                Else
                    ErrCdCheck = 1              '全角文字入力の異常発生
                    Exit Function
                End If

                'カウントアップ
                nStrPnt = nStrPnt + 1
            Loop

            If nStrLen > 0 Then
                '「,」を抜いた文字列を取得
                sInputErrCdDel = InputErrCd.Replace(",", "")

                'それぞれの文字数を取得
                nCntBf = InputErrCd.Length
                nCntAf = sInputErrCdDel.Length

                'コンマの数を取得
                nCnt = nCntBf - nCntAf

                '文字列を分割
                strErr = InputErrCd.Split(CChar(","))
                For i = 0 To nCnt
                    If strErr(i) = "" Then
                        ErrCdCheck = 1
                        Exit For
                    Else
                        '「-」を抜いた文字列を取得
                        sInputErrCdDel = strErr(i).Replace("-", "")

                        'それぞれの文字数を取得
                        nCntBf = strErr(i).Length
                        nCntAf = sInputErrCdDel.Length

                        '-の数を取得
                        nCnt1 = nCntBf - nCntAf

                        '文字列を分割
                        strRange = strErr(i).Split(CChar("-"))
                        For j = 0 To nCnt1
                            If strRange(j) = "" Then
                                ErrCdCheck = 1
                                Exit For
                            End If
                        Next
                    End If
                Next
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

    End Function

    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 6
        Try
            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                ' 帳票ファイル名称を取得
                .FileName = sPath
                .ExcelMode = True
                ' 帳票の出力処理を開始を宣言
                .Report.Start()
                .Report.File()
                '帳票ファイルシート名称を取得します。
                .Page.Start(LcstXlsSheetName, "1-9999")

                ' 見出し部セルへ見出しデータ出力
                .Cell("B1").Value = LcstXlsSheetName
                .Cell("V1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("V2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim + "　　　" + _
                                    OPMGFormConstants.CORNER_STR + cmbMado.Text.Trim + "　　" + _
                                    OPMGFormConstants.EQUIPMENT_TYPE + cmbKisyu.Text.Trim + "　　" + _
                                    OPMGFormConstants.NUM_EQUIPMENT + cmbGouki.Text.Trim
                .Cell("C4").Value = Lexis.TimeSpan.Gen(
                                                  Replace(Replace(Replace(dtpYmdFrom.Text, "年", "/"), "月", "/"), "日", ""), _
                                                  dtpHmFrom.Text, _
                                                  Replace(Replace(Replace(dtpYmdTo.Text, "年", "/"), "月", "/"), "日", ""), _
                                                  dtpHmTo.Text)

                ' 配信対象のデータ数を取得します
                nRecCnt = shtMain.MaxRows

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        If x = LcstPrntCol.Length - 1 Then
                            .Pos(x + 4, y + nStartRow).Value = shtMain.Item(LcstPrntCol(x), y).Text
                        Else
                            .Pos(x + 1, y + nStartRow).Value = shtMain.Item(LcstPrntCol(x), y).Text
                        End If
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

#End Region

End Class