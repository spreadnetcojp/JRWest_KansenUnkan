' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
'   0.1      2014/06/12  (NES)中原    北陸対応（対象パターンNo.チェック処理追加）
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO
Imports GrapeCity.Win.ElTabelleSheet.Editors

''' <summary>
''' マスタ配信指示設定
''' </summary>
''' <remarks>マスタ管理メニューより、「配信指示設定」ボタンをクリックすることにより、本画面を表示する。
''' 本画面にて適用日、配信データ、配信先を指定し、配信指示を行う。</remarks>
Public Class FrmMstOrderDelivery
    Inherits FrmBase

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

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
    Friend WithEvents cmbPtnName As System.Windows.Forms.ComboBox
    Friend WithEvents cmbMstName As System.Windows.Forms.ComboBox
    Friend WithEvents lblPtnNa As System.Windows.Forms.Label
    Friend WithEvents lblMst As System.Windows.Forms.Label
    Friend WithEvents grpDeliveryCnd As System.Windows.Forms.GroupBox
    Friend WithEvents chkbForceDlv As System.Windows.Forms.CheckBox
    Friend WithEvents lblUpdateDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmbTglVersion As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblMstName As System.Windows.Forms.Label
    Friend WithEvents lblMStNa2 As System.Windows.Forms.Label
    Friend WithEvents shtTglConfirm As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents lblModelCode As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents lblPtnNo As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblMstVer As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblTglVer As System.Windows.Forms.Label '配信対象リストのDatasource
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnDelivery As System.Windows.Forms.Button
    Friend WithEvents tabOdrDelivery As System.Windows.Forms.TabControl
    Friend WithEvents tabpDeliveryData As System.Windows.Forms.TabPage
    Friend WithEvents tabpTglConfirm As System.Windows.Forms.TabPage
    Friend WithEvents grpDeliveryInf As System.Windows.Forms.GroupBox
    Friend WithEvents grpDeliveryData As System.Windows.Forms.GroupBox
    Friend WithEvents grpDeliveryLst As System.Windows.Forms.GroupBox
    Friend WithEvents wbkData As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents shtOdrDelivery As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents lblModel As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMstOrderDelivery))
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnDelivery = New System.Windows.Forms.Button()
        Me.tabOdrDelivery = New System.Windows.Forms.TabControl()
        Me.tabpDeliveryData = New System.Windows.Forms.TabPage()
        Me.grpDeliveryCnd = New System.Windows.Forms.GroupBox()
        Me.chkbForceDlv = New System.Windows.Forms.CheckBox()
        Me.grpDeliveryInf = New System.Windows.Forms.GroupBox()
        Me.cmbPtnName = New System.Windows.Forms.ComboBox()
        Me.cmbMstName = New System.Windows.Forms.ComboBox()
        Me.lblPtnNa = New System.Windows.Forms.Label()
        Me.lblMst = New System.Windows.Forms.Label()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.grpDeliveryData = New System.Windows.Forms.GroupBox()
        Me.wbkData = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtOdrDelivery = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.tabpTglConfirm = New System.Windows.Forms.TabPage()
        Me.grpDeliveryLst = New System.Windows.Forms.GroupBox()
        Me.shtTglConfirm = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.lblModelCode = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblPtnNo = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblMstVer = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblTglVer = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblMstName = New System.Windows.Forms.Label()
        Me.lblMStNa2 = New System.Windows.Forms.Label()
        Me.lblUpdateDate = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmbTglVersion = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.tabOdrDelivery.SuspendLayout()
        Me.tabpDeliveryData.SuspendLayout()
        Me.grpDeliveryCnd.SuspendLayout()
        Me.grpDeliveryInf.SuspendLayout()
        Me.grpDeliveryData.SuspendLayout()
        Me.wbkData.SuspendLayout()
        CType(Me.shtOdrDelivery, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabpTglConfirm.SuspendLayout()
        Me.grpDeliveryLst.SuspendLayout()
        CType(Me.shtTglConfirm, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.tabOdrDelivery)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnDelivery)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2014/06/12(木)  19:00"
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 4
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnDelivery
        '
        Me.btnDelivery.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDelivery.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelivery.Location = New System.Drawing.Point(872, 520)
        Me.btnDelivery.Name = "btnDelivery"
        Me.btnDelivery.Size = New System.Drawing.Size(128, 40)
        Me.btnDelivery.TabIndex = 3
        Me.btnDelivery.Text = "配　信"
        Me.btnDelivery.UseVisualStyleBackColor = False
        '
        'tabOdrDelivery
        '
        Me.tabOdrDelivery.Controls.Add(Me.tabpDeliveryData)
        Me.tabOdrDelivery.Controls.Add(Me.tabpTglConfirm)
        Me.tabOdrDelivery.Location = New System.Drawing.Point(28, 28)
        Me.tabOdrDelivery.Name = "tabOdrDelivery"
        Me.tabOdrDelivery.SelectedIndex = 0
        Me.tabOdrDelivery.Size = New System.Drawing.Size(812, 596)
        Me.tabOdrDelivery.TabIndex = 0
        '
        'tabpDeliveryData
        '
        Me.tabpDeliveryData.Controls.Add(Me.grpDeliveryCnd)
        Me.tabpDeliveryData.Controls.Add(Me.grpDeliveryInf)
        Me.tabpDeliveryData.Controls.Add(Me.grpDeliveryData)
        Me.tabpDeliveryData.Location = New System.Drawing.Point(4, 23)
        Me.tabpDeliveryData.Name = "tabpDeliveryData"
        Me.tabpDeliveryData.Size = New System.Drawing.Size(804, 569)
        Me.tabpDeliveryData.TabIndex = 0
        Me.tabpDeliveryData.Text = "配信データ"
        '
        'grpDeliveryCnd
        '
        Me.grpDeliveryCnd.Controls.Add(Me.chkbForceDlv)
        Me.grpDeliveryCnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryCnd.Location = New System.Drawing.Point(461, 20)
        Me.grpDeliveryCnd.Name = "grpDeliveryCnd"
        Me.grpDeliveryCnd.Size = New System.Drawing.Size(319, 126)
        Me.grpDeliveryCnd.TabIndex = 13
        Me.grpDeliveryCnd.TabStop = False
        Me.grpDeliveryCnd.Text = "配信条件"
        '
        'chkbForceDlv
        '
        Me.chkbForceDlv.AutoSize = True
        Me.chkbForceDlv.Location = New System.Drawing.Point(24, 25)
        Me.chkbForceDlv.Name = "chkbForceDlv"
        Me.chkbForceDlv.Size = New System.Drawing.Size(266, 17)
        Me.chkbForceDlv.TabIndex = 0
        Me.chkbForceDlv.Text = "マスタ＋マスタ適用リスト強制配信"
        Me.chkbForceDlv.UseVisualStyleBackColor = True
        '
        'grpDeliveryInf
        '
        Me.grpDeliveryInf.Controls.Add(Me.cmbPtnName)
        Me.grpDeliveryInf.Controls.Add(Me.cmbMstName)
        Me.grpDeliveryInf.Controls.Add(Me.lblPtnNa)
        Me.grpDeliveryInf.Controls.Add(Me.lblMst)
        Me.grpDeliveryInf.Controls.Add(Me.cmbModel)
        Me.grpDeliveryInf.Controls.Add(Me.lblModel)
        Me.grpDeliveryInf.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryInf.Location = New System.Drawing.Point(19, 20)
        Me.grpDeliveryInf.Name = "grpDeliveryInf"
        Me.grpDeliveryInf.Size = New System.Drawing.Size(423, 126)
        Me.grpDeliveryInf.TabIndex = 11
        Me.grpDeliveryInf.TabStop = False
        Me.grpDeliveryInf.Text = "配信情報"
        '
        'cmbPtnName
        '
        Me.cmbPtnName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPtnName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbPtnName.Items.AddRange(New Object() {"全角＿＿＿＿＿＿１０"})
        Me.cmbPtnName.Location = New System.Drawing.Point(129, 87)
        Me.cmbPtnName.MaxLength = 20
        Me.cmbPtnName.Name = "cmbPtnName"
        Me.cmbPtnName.Size = New System.Drawing.Size(173, 21)
        Me.cmbPtnName.TabIndex = 52
        '
        'cmbMstName
        '
        Me.cmbMstName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMstName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMstName.Items.AddRange(New Object() {"全角＿＿＿＿＿＿＿＿＿＿＿１５"})
        Me.cmbMstName.Location = New System.Drawing.Point(129, 55)
        Me.cmbMstName.Name = "cmbMstName"
        Me.cmbMstName.Size = New System.Drawing.Size(243, 21)
        Me.cmbMstName.TabIndex = 51
        '
        'lblPtnNa
        '
        Me.lblPtnNa.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnNa.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNa.Location = New System.Drawing.Point(24, 90)
        Me.lblPtnNa.Name = "lblPtnNa"
        Me.lblPtnNa.Size = New System.Drawing.Size(120, 18)
        Me.lblPtnNa.TabIndex = 50
        Me.lblPtnNa.Text = "パターン名称"
        Me.lblPtnNa.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMst
        '
        Me.lblMst.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMst.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMst.Location = New System.Drawing.Point(24, 55)
        Me.lblMst.Name = "lblMst"
        Me.lblMst.Size = New System.Drawing.Size(120, 18)
        Me.lblMst.TabIndex = 49
        Me.lblMst.Text = "マスタ名称"
        Me.lblMst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.Items.AddRange(New Object() {"全角＿＿５", "改札機", "窓口処理機"})
        Me.cmbModel.Location = New System.Drawing.Point(129, 21)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(103, 21)
        Me.cmbModel.TabIndex = 47
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(24, 24)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(120, 18)
        Me.lblModel.TabIndex = 48
        Me.lblModel.Text = "機種"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'grpDeliveryData
        '
        Me.grpDeliveryData.Controls.Add(Me.wbkData)
        Me.grpDeliveryData.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryData.Location = New System.Drawing.Point(19, 161)
        Me.grpDeliveryData.Name = "grpDeliveryData"
        Me.grpDeliveryData.Size = New System.Drawing.Size(761, 369)
        Me.grpDeliveryData.TabIndex = 12
        Me.grpDeliveryData.TabStop = False
        Me.grpDeliveryData.Text = "配信データ"
        '
        'wbkData
        '
        Me.wbkData.Controls.Add(Me.shtOdrDelivery)
        Me.wbkData.Location = New System.Drawing.Point(12, 24)
        Me.wbkData.Name = "wbkData"
        Me.wbkData.ProcessTabKey = False
        Me.wbkData.ShowTabs = False
        Me.wbkData.Size = New System.Drawing.Size(710, 312)
        Me.wbkData.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wbkData.TabIndex = 1
        '
        'shtOdrDelivery
        '
        Me.shtOdrDelivery.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtOdrDelivery.Data = CType(resources.GetObject("shtOdrDelivery.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtOdrDelivery.Location = New System.Drawing.Point(2, 2)
        Me.shtOdrDelivery.Name = "shtOdrDelivery"
        Me.shtOdrDelivery.Size = New System.Drawing.Size(689, 291)
        Me.shtOdrDelivery.TabIndex = 0
        '
        'tabpTglConfirm
        '
        Me.tabpTglConfirm.Controls.Add(Me.grpDeliveryLst)
        Me.tabpTglConfirm.Location = New System.Drawing.Point(4, 23)
        Me.tabpTglConfirm.Name = "tabpTglConfirm"
        Me.tabpTglConfirm.Size = New System.Drawing.Size(804, 569)
        Me.tabpTglConfirm.TabIndex = 1
        Me.tabpTglConfirm.Text = "マスタ適用確認"
        '
        'grpDeliveryLst
        '
        Me.grpDeliveryLst.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grpDeliveryLst.Controls.Add(Me.shtTglConfirm)
        Me.grpDeliveryLst.Controls.Add(Me.lblModelCode)
        Me.grpDeliveryLst.Controls.Add(Me.Label9)
        Me.grpDeliveryLst.Controls.Add(Me.lblPtnNo)
        Me.grpDeliveryLst.Controls.Add(Me.Label7)
        Me.grpDeliveryLst.Controls.Add(Me.lblMstVer)
        Me.grpDeliveryLst.Controls.Add(Me.Label5)
        Me.grpDeliveryLst.Controls.Add(Me.lblTglVer)
        Me.grpDeliveryLst.Controls.Add(Me.Label4)
        Me.grpDeliveryLst.Controls.Add(Me.lblMstName)
        Me.grpDeliveryLst.Controls.Add(Me.lblMStNa2)
        Me.grpDeliveryLst.Controls.Add(Me.lblUpdateDate)
        Me.grpDeliveryLst.Controls.Add(Me.Label2)
        Me.grpDeliveryLst.Controls.Add(Me.cmbTglVersion)
        Me.grpDeliveryLst.Controls.Add(Me.Label1)
        Me.grpDeliveryLst.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryLst.Location = New System.Drawing.Point(28, 22)
        Me.grpDeliveryLst.Name = "grpDeliveryLst"
        Me.grpDeliveryLst.Size = New System.Drawing.Size(752, 534)
        Me.grpDeliveryLst.TabIndex = 12
        Me.grpDeliveryLst.TabStop = False
        '
        'shtTglConfirm
        '
        Me.shtTglConfirm.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtTglConfirm.Data = CType(resources.GetObject("shtTglConfirm.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtTglConfirm.Location = New System.Drawing.Point(62, 158)
        Me.shtTglConfirm.Name = "shtTglConfirm"
        Me.shtTglConfirm.Size = New System.Drawing.Size(353, 310)
        Me.shtTglConfirm.TabIndex = 71
        '
        'lblModelCode
        '
        Me.lblModelCode.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModelCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModelCode.Location = New System.Drawing.Point(614, 103)
        Me.lblModelCode.Name = "lblModelCode"
        Me.lblModelCode.Size = New System.Drawing.Size(21, 18)
        Me.lblModelCode.TabIndex = 70
        Me.lblModelCode.Text = "G"
        Me.lblModelCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(525, 103)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 18)
        Me.Label9.TabIndex = 69
        Me.Label9.Text = "機種コード"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnNo
        '
        Me.lblPtnNo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnNo.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNo.Location = New System.Drawing.Point(291, 103)
        Me.lblPtnNo.Name = "lblPtnNo"
        Me.lblPtnNo.Size = New System.Drawing.Size(27, 18)
        Me.lblPtnNo.TabIndex = 68
        Me.lblPtnNo.Text = "01"
        Me.lblPtnNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(199, 103)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(86, 18)
        Me.Label7.TabIndex = 67
        Me.Label7.Text = "パターンNo"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMstVer
        '
        Me.lblMstVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMstVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMstVer.Location = New System.Drawing.Point(105, 103)
        Me.lblMstVer.Name = "lblMstVer"
        Me.lblMstVer.Size = New System.Drawing.Size(47, 18)
        Me.lblMstVer.TabIndex = 66
        Me.lblMstVer.Text = "001"
        Me.lblMstVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(37, 103)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(62, 18)
        Me.Label5.TabIndex = 65
        Me.Label5.Text = "代表Ver"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTglVer
        '
        Me.lblTglVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTglVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTglVer.Location = New System.Drawing.Point(679, 75)
        Me.lblTglVer.Name = "lblTglVer"
        Me.lblTglVer.Size = New System.Drawing.Size(27, 18)
        Me.lblTglVer.TabIndex = 64
        Me.lblTglVer.Text = "01"
        Me.lblTglVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(525, 78)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(148, 19)
        Me.Label4.TabIndex = 63
        Me.Label4.Text = "マスタ適用リストVer"
        '
        'lblMstName
        '
        Me.lblMstName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMstName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMstName.Location = New System.Drawing.Point(291, 71)
        Me.lblMstName.Name = "lblMstName"
        Me.lblMstName.Size = New System.Drawing.Size(218, 25)
        Me.lblMstName.TabIndex = 62
        Me.lblMstName.Text = "全角＿＿＿＿＿＿＿＿＿＿＿１５"
        Me.lblMstName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMStNa2
        '
        Me.lblMStNa2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMStNa2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMStNa2.Location = New System.Drawing.Point(199, 77)
        Me.lblMStNa2.Name = "lblMStNa2"
        Me.lblMStNa2.Size = New System.Drawing.Size(86, 15)
        Me.lblMStNa2.TabIndex = 61
        Me.lblMStNa2.Text = "マスタ名称"
        '
        'lblUpdateDate
        '
        Me.lblUpdateDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblUpdateDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUpdateDate.Location = New System.Drawing.Point(105, 74)
        Me.lblUpdateDate.Name = "lblUpdateDate"
        Me.lblUpdateDate.Size = New System.Drawing.Size(88, 18)
        Me.lblUpdateDate.TabIndex = 52
        Me.lblUpdateDate.Text = "2013/04/16"
        Me.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(37, 75)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 18)
        Me.Label2.TabIndex = 51
        Me.Label2.Text = "作成日"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbTglVersion
        '
        Me.cmbTglVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTglVersion.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbTglVersion.Items.AddRange(New Object() {"01"})
        Me.cmbTglVersion.Location = New System.Drawing.Point(231, 26)
        Me.cmbTglVersion.Name = "cmbTglVersion"
        Me.cmbTglVersion.Size = New System.Drawing.Size(54, 21)
        Me.cmbTglVersion.TabIndex = 49
        Me.cmbTglVersion.Enabled = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(37, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(198, 18)
        Me.Label1.TabIndex = 50
        Me.Label1.Text = "マスタ適用リストバージョン"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(872, 456)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 2
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'FrmMstOrderDelivery
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMstOrderDelivery"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.tabOdrDelivery.ResumeLayout(False)
        Me.tabpDeliveryData.ResumeLayout(False)
        Me.grpDeliveryCnd.ResumeLayout(False)
        Me.grpDeliveryCnd.PerformLayout()
        Me.grpDeliveryInf.ResumeLayout(False)
        Me.grpDeliveryData.ResumeLayout(False)
        Me.wbkData.ResumeLayout(False)
        CType(Me.shtOdrDelivery, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabpTglConfirm.ResumeLayout(False)
        Me.grpDeliveryLst.ResumeLayout(False)
        CType(Me.shtTglConfirm, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "各種宣言領域"

    '強制配信
    Public Const FORCED_DELIVERY As String = "強制配信："
    '作成日
    Public Const UPDATED_DATE As String = "作成日："
    'マスタ適用リスト
    Public Const MASTER_APPLIED_LIST As String = "マスタ適用リストバージョン："
    '代表バージョン
    Public Const MSTVER As String = "代表バージョン："
    'パターンNo
    Public Const PTNNO As String = "パターンNo："
    '機種コード
    Public Const MODELCODE As String = "機種コード："

    Private LbInitCallFlg As Boolean = False

    '-------Ver0.1　北陸対応　ADD START-----------
    '対象パターンNo.チェック結果フラグ　True：正常 False：異常
    Private bPatternChkResultFlg As Boolean = True
    '対象パターンNo.チェック実施フラグ　True：実施ＯＮ False：実施ＯＦＦ
    Private bPatternChkEventFlg As Boolean = False
    '-------Ver0.1　北陸対応　ADD END-----------

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    ''' 出力用テンプレートファイル名
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "マスタ配信指示設定（配信データ）.xls"

    ''' <summary>
    ''' 出力用テンプレートファイル名
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName2 As String = "マスタ配信指示設定（マスタ適用確認）.xls"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "マスタ配信指示設定（配信データ）"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName2 As String = "マスタ配信指示設定（マスタ適用確認）"

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "マスタ配信指示設定"

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した別集札データに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3, 4, 5}

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した別集札データに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol2() As Integer = {0, 1, 2}

    Private LstListFile_Name As String

#End Region

#Region "フォームロード"
    'Private Sub FrmMstOrderDelivery_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
    '    Me.tabOdrDelivery.Focus()
    'End Sub

    ''' <summary>
    ''' フォームロード
    ''' </summary>
    ''' <remarks>画面タイトル、画面背景色（BackColor）を設定し、ELTableを表示する。</remarks>
    Private Sub FrmMstOrderDelivery_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        LfWaitCursor()
        If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
            If InitFrmData() = False Then   '初期処理
                Me.Close()
                Exit Sub
            End If
        End If
        'チェックボックス表示設定
        chkbForceDlv.Text = "マスタ＋マスタ適用リスト" & vbCrLf & "強制配信"
        Me.tabOdrDelivery.Focus()
        LfWaitCursor(False)

    End Sub
#End Region

#Region "マスタ配信指示設定画面のデータを準備する"
    ''' <summary>
    ''' マスタ配信指示設定画面のデータを準備する
    ''' </summary>
    ''' <remarks>
    '''マスタ配信指示設定データを検索し、画面に表示する
    ''' </remarks>   
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean

        Dim bSetDefaultVer As Boolean = False
        Dim bGetEltableData As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ

        Try
            Log.Info("Method started.")

            '画面タイトル
            lblTitle.Text = LcstFormTitle

            shtOdrDelivery.TransformEditor = False                                    '一覧の列種類毎のチェックを無効にする
            shtTglConfirm.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtTglConfirm.ViewMode = ElTabelleSheet.ViewMode.Row                      '行選択モード
            shtTglConfirm.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード

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

            '各コンボボックスの項目登録

            '機種名称コンボボックスを設定する。
            If setCmbModel() = False Then
                Return False
            End If
            cmbModel.SelectedIndex = 0            'デフォルト表示項目

            'マスタ名称コンボボックスを設定する。
            If setCmbMstName(cmbModel.SelectedValue.ToString) = False Then
                Return False
            End If
            cmbMstName.SelectedIndex = 0            'デフォルト表示項目

            'パターン名称コンボボックスを設定する。
            If setCmbPtnName(cmbModel.SelectedValue.ToString, cmbMstName.SelectedValue.ToString) = False Then
                Return False
            End If
            cmbPtnName.SelectedIndex = 0            'デフォルト表示項目

            'ELTable の初期化
            Call initElTable(Me.shtOdrDelivery)
            Call initElTable(Me.shtTglConfirm)
            Call ClrTglList()

            Me.btnPrint.Enabled = False
            Me.btnDelivery.Enabled = False

            Log.Info("Method ended.")
            Return True

        Catch ex As Exception
            '画面表示処理に失敗しました
            Log.Fatal("Unwelcome Exception caught.", ex)
            Log.Error("Method abended.")
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
            Return False
        Finally
            LbEventStop = False 'イベント発生ＯＮ
        End Try

    End Function
#End Region

#Region "eltableの値がチェンジ時、トリガーされる。"
    ''' <summary>
    ''' Eltableの値がチェンジ時、トリガーされる。
    ''' </summary>
    ''' <remarks>バージョンの値がチェンジ時、トリガーされる。</remarks>
    Private Sub shtOdrDelivery_CellNotify(ByVal sender As Object, _
                ByVal e As GrapeCity.Win.ElTabelleSheet.CellNotifyEventArgs) Handles shtOdrDelivery.CellNotify

        If e.Name <> ElTabelleSheet.CellNotifyEvents.SelectedIndexChanged Then Exit Sub

        Dim cmbEdt As ElTabelleSheet.Editors.SuperiorComboEditor
        Dim nIndex As Integer
        Dim bSetCombox As Boolean = False
        Dim lstCmbItems As New ArrayList()

        '画面の閃きを防ぐため
        Me.shtOdrDelivery.Redraw = False

        '選択されたコンボボックスのインデックスを取得
        cmbEdt = CType(Me.shtOdrDelivery.Item(1, e.Position.Row).Editor, ElTabelleSheet.Editors.SuperiorComboEditor)
        For i As Integer = 1 To cmbEdt.Items.Count - 1
            If cmbEdt.Items(i).Selected = True Then
                nIndex = i
            End If
            lstCmbItems.Add(New DictionaryEntry(cmbEdt.Items(i).Content.ToString, cmbEdt.Items(i).Value.ToString))
        Next

        'バージョンを選択されたら
        If nIndex <> 0 Then
            '選択されたバージョンのマスタ適用ファイル名を取得
            LstListFile_Name = cmbEdt.Items(nIndex).Value.ToString

            'マスタ適用確認タブのコンボボックスを生成
            If nIndex <> 1 Then
                LbEventStop = True      'イベント発生ＯＦＦ
                '※「nIndex =  1」の場合は「cmbTglVersion.DataSource = lstCmbItems」でイベント発生し、「cmbTglVersion.SelectedIndex = nIndex - 1」でイベント発生せず、
                '※「nIndex <> 1」の場合は「cmbTglVersion.DataSource = lstCmbItems」でイベント発生し、「cmbTglVersion.SelectedIndex = nIndex - 1」でイベント発生する。
                '※「nIndex <> 1」の場合に２度イベント発生させない為の処理。
            End If
            '-------Ver0.1　北陸対応　ADD START-----------
            '対象パターンNo.チェック結果を初期化
            bPatternChkResultFlg = True
            '対象パターンNo.チェック実施ＯＮ
            bPatternChkEventFlg = True
            '-------Ver0.1　北陸対応　ADD END-----------
            cmbTglVersion.DisplayMember = "Key"
            cmbTglVersion.ValueMember = "Value"
            cmbTglVersion.DataSource = lstCmbItems
            LbEventStop = False      'イベント発生ＯＦＦ
            cmbTglVersion.SelectedIndex = nIndex - 1
            '-------Ver0.1　北陸対応　ADD START-----------
            '対象パターンNo.チェック実施ＯＦＦ
            bPatternChkEventFlg = False
            '-------Ver0.1　北陸対応　ADD END-----------

            '選択された行以外のコンボボックスをクリア
            For i As Integer = 0 To Me.shtOdrDelivery.MaxRows - 1
                If i <> e.Position.Row Then
                    Me.shtOdrDelivery.Item(1, i).Text = ""
                End If
            Next

            Me.btnDelivery.Enabled = True

        Else
            '空白を選択されたら
            LstListFile_Name = ""
            LbEventStop = True      'イベント発生ＯＦＦ
            cmbTglVersion.DataSource = Nothing
            Call initElTable(Me.shtTglConfirm)
            'ラベル活性化
            Call ClrTglList()
            LbEventStop = False      'イベント発生ＯＦＦ

            Me.btnDelivery.Enabled = False

        End If

        lstCmbItems = Nothing
        '画面の閃きを防ぐため
        Me.shtOdrDelivery.Redraw = True

    End Sub

#End Region

#Region "「出力」ボタンクリック"
    ''' <summary>
    ''' 「出力」ボタンクリック
    ''' </summary>
    ''' <param name="sender">System.Object</param>
    ''' <param name="e">System.EventArgs</param>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            LogOperation(sender, e)    'ボタン押下ログ

            Dim sDirPath As String = Config.LedgerTemplateDirPath
            Dim sFilePath As String = ""

            'テンプレート格納フォルダチェック
            If Directory.Exists(sDirPath) = False Then
                Log.Error("It's not found [" & sDirPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If

            'テンプレートフルパスチェック
            sFilePath = Path.Combine(sDirPath, LcstXlsTemplateName)
            If File.Exists(sFilePath) = False Then
                Log.Error("It's not found [" & sFilePath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If
            '出力
            LfXlsStart(sFilePath)
            'テンプレートフルパスチェック
            sFilePath = Path.Combine(sDirPath, LcstXlsTemplateName2)
            If File.Exists(sFilePath) = False Then
                Log.Error("It's not found [" & sFilePath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If
            '出力
            '適用リストのVerが選択されている場合のみ、出力を行う。
            If cmbTglVersion.Text <> String.Empty Then
                LfXlsStart2(sFilePath)
            End If

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
#End Region

#Region "「配信」ボタンクリック"
    ''' <summary>
    ''' 「配信」ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>該当のマスタ適用リストで配信指示を行う。
    ''' 配信指示が終了した場合は、指示完了のポップアップ画面を表示する。</remarks>
    Private Sub btnDelivery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelivery.Click
        Try
            LogOperation(sender, e)    'ボタン押下ログ

            '-------Ver0.1　北陸対応　ADD START-----------
            '対象パターンNo.チェックで異常であれば配信しない。
            If bPatternChkResultFlg = False Then
                '適用リストに対象外の駅が含まれています。
                AlertBox.Show(Lexis.ApplicationListExcludedStationIncluded)
                Exit Sub
            End If
            '-------Ver0.1　北陸対応　ADD END-----------

            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyInvokeMasProDll) = DialogResult.No Then
                LogOperation(Lexis.NoButtonClicked)     'Noボタン押下ログ
                Exit Sub
            End If

            LogOperation(Lexis.YesButtonClicked)     'Yesボタン押下ログ

            Call waitCursor(True)

            If OpClientUtil.Connect() = False Then
                AlertBox.Show(Lexis.ConnectFailed)
                Exit Sub
            End If

            Dim sListFileName As String = LstListFile_Name
            Dim ullResult As MasProDllInvokeResult = OpClientUtil.InvokeMasProDll(sListFileName, chkbForceDlv.Checked)

            OpClientUtil.Disconnect()

            Select Case ullResult
                Case MasProDllInvokeResult.Completed
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.Completed received.")
                    AlertBox.Show(Lexis.InvokeMasProDllCompleted)
                Case MasProDllInvokeResult.Failed
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.Failed received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailed)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByBusy
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByBusy received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByBusy)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByNoData
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByNoData received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByNoData)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByUnnecessary
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByUnnecessary received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByUnnecessary)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByInvalidContent
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByInvalidContent received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByInvalidContent)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByUnknownLight
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByUnknownLight received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByUnknownLight)
                    Exit Sub
                Case Else
                    Log.Fatal("The telegrapher seems broken.")
                    AlertBox.Show(Lexis.UnforeseenErrorOccurred)
                    OpClientUtil.RestartBrokenTelegrapher()
                    Exit Sub
            End Select

        Catch ex As OPMGException
            Log.Error("MasProUll failed.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)

        Finally
            Call waitCursor(False)
        End Try
    End Sub
#End Region

#Region "「終了」ボタンクリック"
    ''' <summary>
    ''' 「終了」ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>「終了」ボタンをクリックすることにより、「マスタ管理メニュー」画面に戻る。</remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()
    End Sub
#End Region

#Region "コンボボックスを設定する。"
    ''' <summary>
    ''' 機種名称コンボボックスを設定する。
    ''' </summary>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理している機種名称の一覧及び「空白」を設定する。</remarks>
    Private Function setCmbModel() As Boolean

        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New ModelMaster

        Try
            '機種名称コンボボックス用のデータを取得する。
            dt = oMst.SelectTable()
            If dt.Rows.Count = 0 Then
                '機種データ取得失敗
                Return bRtn
            End If
            dt = oMst.SetSpace()

            bRtn = BaseSetMstDtToCmb(dt, cmbModel)
            cmbModel.SelectedIndex = -1
            If cmbModel.Items.Count <= 0 Then bRtn = False

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
    ''' マスタ名称コンボボックスを設定する。
    ''' </summary>
    ''' <param name="Model"> 機種コード</param>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理しているマスタ名称の一覧及び「空白」を設定する。</remarks>
    Private Function setCmbMstName(ByVal Model As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As MasterMaster
        oMst = New MasterMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If Model <> "" And Model <> ClientDaoConstants.TERMINAL_ALL Then
                dt = oMst.SelectTable(Model)
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbMstName)
            cmbMstName.SelectedIndex = -1
            If cmbMstName.Items.Count <= 0 Then bRtn = False
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
    ''' パターン名称コンボボックスを設定する。
    ''' </summary>
    ''' <param name="Model">機種コード</param>
    ''' <param name="Master">マスタ区分</param>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理しているパターン名称の一覧及び「空白」を設定する。</remarks>
    Private Function setCmbPtnName(ByVal Model As String, ByVal Master As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As PatternMaster
        oMst = New PatternMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If String.IsNullOrEmpty(Master) Then
                Master = ""
            End If
            If ((Model <> "" AndAlso Model <> ClientDaoConstants.TERMINAL_ALL) _
            AndAlso (Master <> "" AndAlso Master <> ClientDaoConstants.TERMINAL_ALL)) Then
                dt = oMst.SelectTable(Model, Master)
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbPtnName)
            cmbPtnName.SelectedIndex = -1
            If cmbPtnName.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function
#End Region

#Region "ELTableの初期化"
    ''' <summary>
    ''' ELTableの初期化
    ''' </summary>
    ''' <remarks>Eltableに既存したデータをクリアする。データによって再度Eltableのデータエリアを設定する。</remarks>
    Private Sub initElTable(ByVal shtTarget As GrapeCity.Win.ElTabelleSheet.Sheet)

        'Eltableのカレントの最大桁数
        Dim sXYRange As String = ""

        '画面の閃きを防ぐため
        shtTarget.Redraw = False

        If shtTarget.MaxRows > 0 Then
            'Eltableのカレントの最大桁数を取得する。
            sXYRange = "1:" & shtTarget.MaxRows.ToString

            '選択されたエリアのデータをクリアする。
            shtTarget.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
        End If

        shtTarget.MaxRows = 0

        '画面の閃きを防ぐため
        shtTarget.Redraw = True

    End Sub
#End Region

#Region "マスタ配信指示設定（配信データ） 帳票出力"

    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 13
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
                .Cell("G1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("G2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B6").Value = OPMGFormConstants.EQUIPMENT_TYPE_NAME + cmbModel.Text.Trim

                If Me.chkbForceDlv.Checked = False Then
                    .Cell("E6").Value = FORCED_DELIVERY + "無"
                Else
                    .Cell("E6").Value = FORCED_DELIVERY + "有"
                End If
                .Cell("B7").Value = OPMGFormConstants.MST_NAME + Me.cmbMstName.Text.Trim
                .Cell("B8").Value = OPMGFormConstants.PATTERN_NAME + Me.cmbPtnName.Text.Trim

                ' 配信対象のデータ数を取得します
                nRecCnt = shtOdrDelivery.MaxRows

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtOdrDelivery.Item(LcstPrntCol(x), y).Text
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

#Region "マスタ配信指示設定（マスタ適用確認）　帳票出力"
    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart2(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 13
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
                .Page.Start(LcstXlsSheetName2, "1-9999")

                ' 見出し部セルへ見出しデータ出力
                .Cell("B1").Value = LcstXlsSheetName2
                .Cell("S1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("S2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B4").Value = UPDATED_DATE + Me.lblUpdateDate.Text.Trim
                .Cell("B5").Value = OPMGFormConstants.MST_NAME + Me.lblMstName.Text.Trim
                .Cell("B6").Value = MASTER_APPLIED_LIST + Me.lblTglVer.Text.Trim
                .Cell("B7").Value = MSTVER + Me.lblMstVer.Text.Trim
                .Cell("B8").Value = PTNNO + Me.lblPtnNo.Text.Trim
                .Cell("B9").Value = MODELCODE + Me.lblModelCode.Text.Trim

                ' 配信対象のデータ数を取得します
                nRecCnt = shtTglConfirm.MaxRows

                If nRecCnt = 0 Then
                    .RowClear(nStartRow, 1)
                End If

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol2.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtTglConfirm.Item(LcstPrntCol2(x), y).Text
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

    ''' <summary>
    ''' 機種コンボ選択イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbModel_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbModel.SelectedIndexChanged

        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            'コーナーコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbMstName(cmbModel.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMst.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbMstName.SelectedIndex = 0               '★イベント発生箇所
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblModel.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' マスタコンボ選択イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbMstName_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbMstName.SelectedIndexChanged

        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            'コーナーコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbPtnName(cmbModel.SelectedValue.ToString, cmbMstName.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblPtnNa.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbPtnName.SelectedIndex = 0               '★イベント発生箇所

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMst.Text)
        Finally
            LfWaitCursor(False)
        End Try


    End Sub

    ''' <summary>
    ''' パターンコンボ選択イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbPtnName_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbPtnName.SelectedIndexChanged
        Dim sSql As String = ""
        Dim dtData As New DataTable
        Dim nCnt As Integer
        Dim RowCnt As Integer
        Dim sKey As String = ""
        Dim cmbEdt As ElTabelleSheet.Editors.SuperiorComboEditor = Nothing
        Dim Ar As New ArrayList

        If LbEventStop Then Exit Sub

        Call initElTable(Me.shtOdrDelivery)
        Call initElTable(Me.shtTglConfirm)
        Call ClrTglList()
        LbEventStop = True      'イベント発生ＯＮ
        cmbTglVersion.DataSource = Nothing
        LbEventStop = False      'イベント発生ＯＮ
        Me.btnPrint.Enabled = False
        Me.btnDelivery.Enabled = False

        If cmbPtnName.SelectedIndex = 0 Then
            Exit Sub
        End If

        LfWaitCursor()

        Try

            sSql = "SELECT LST.NAME,LST.LIST_VERSION,LST.FILE_NAME,LST.DATA_VERSION,DAT.UPDATE_DATE FROM" _
                & " (SELECT LS.MODEL_CODE, LS.DATA_KIND, LS.LIST_VERSION, LS.DATA_SUB_KIND, LS.DATA_VERSION," _
                & "   LS.FILE_NAME, MS.NAME FROM S_MST_LIST_HEADLINE AS LS, M_MST_NAME AS MS" _
                & "   WHERE LS.MODEL_CODE=MS.MODEL_CODE AND LS.DATA_KIND=MS.DATA_KIND AND MS.FILE_KBN='LST') AS LST," _
                & " (SELECT UPDATE_DATE, MODEL_CODE, DATA_KIND, DATA_SUB_KIND, DATA_VERSION FROM" _
                & "   S_MST_DATA_HEADLINE) AS DAT WHERE LST.MODEL_CODE=DAT.MODEL_CODE AND LST.DATA_KIND=DAT.DATA_KIND" _
                & "   AND LST.DATA_SUB_KIND=DAT.DATA_SUB_KIND AND LST.DATA_VERSION=DAT.DATA_VERSION" _
                & "   AND LST.MODEL_CODE='" & cmbModel.SelectedValue.ToString & "'" _
                & " AND LST.DATA_KIND='" & cmbMstName.SelectedValue.ToString & "'" _
                & " AND LST.DATA_SUB_KIND='" & cmbPtnName.SelectedValue.ToString & "'" _
                & " ORDER BY LST.DATA_VERSION"

            nCnt = BaseSqlDataTableFill(sSql, dtData)
            Select Case nCnt
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '該当なし
                    AlertBox.Show(Lexis.NoRecordsFound)
                    cmbModel.Select()
                    Exit Sub
            End Select

            '画面の閃きを防ぐ。
            Me.shtOdrDelivery.Redraw = False

            'データがある場合、データの行数によって再度Eltableの最大桁数を設定する。
            If Me.shtOdrDelivery.MaxRows < nCnt Then
                Me.shtOdrDelivery.MaxRows = nCnt
            End If
            RowCnt = 0
            '動的にデータを追加する。
            For i As Integer = 0 To nCnt - 1
                If sKey <> dtData.Rows(i).Item("DATA_VERSION").ToString Then
                    If i <> 0 Then
                        cmbEdt.Editable = False
                        Me.shtOdrDelivery.Item(0, RowCnt).Text = dtData.Rows(i - 1).Item("NAME").ToString
                        Me.shtOdrDelivery.Item(1, RowCnt).Editor = cmbEdt
                        Me.shtOdrDelivery.Item(2, RowCnt).Text = Me.cmbMstName.Text
                        Me.shtOdrDelivery.Item(3, RowCnt).Text = Me.cmbPtnName.SelectedValue.ToString
                        Me.shtOdrDelivery.Item(4, RowCnt).Text = dtData.Rows(i - 1).Item("DATA_VERSION").ToString
                        Me.shtOdrDelivery.Item(5, RowCnt).Text = Format(Convert.ToDateTime(dtData.Rows(i - 1).Item("UPDATE_DATE")), "yyyy/MM/dd")
                        RowCnt = RowCnt + 1
                    End If

                    cmbEdt = New ElTabelleSheet.Editors.SuperiorComboEditor
                    cmbEdt.Items.Add(New GrapeCity.Win.ElTabelleSheet.Editors.ComboItem(0, Nothing, "", "", ""))
                    sKey = dtData.Rows(i).Item("DATA_VERSION").ToString
                End If
                cmbEdt.Items.Add(New GrapeCity.Win.ElTabelleSheet.Editors.ComboItem(0, Nothing, dtData.Rows(i).Item("LIST_VERSION").ToString, "", dtData.Rows(i).Item("FILE_NAME").ToString))

            Next

            cmbEdt.Editable = False
            Me.shtOdrDelivery.Item(0, RowCnt).Text = dtData.Rows(nCnt - 1).Item("NAME").ToString
            Me.shtOdrDelivery.Item(1, RowCnt).Editor = cmbEdt
            Me.shtOdrDelivery.Item(2, RowCnt).Text = Me.cmbMstName.Text
            Me.shtOdrDelivery.Item(3, RowCnt).Text = Me.cmbPtnName.SelectedValue.ToString
            Me.shtOdrDelivery.Item(4, RowCnt).Text = dtData.Rows(nCnt - 1).Item("DATA_VERSION").ToString
            Me.shtOdrDelivery.Item(5, RowCnt).Text = Format(Convert.ToDateTime(dtData.Rows(nCnt - 1).Item("UPDATE_DATE")), "yyyy/MM/dd")

            Me.shtOdrDelivery.MaxRows = RowCnt + 1
            Me.shtOdrDelivery.Rows.SetAllRowsHeight(21)

            Me.btnPrint.Enabled = True

        Catch ex As Exception

        Finally
            Me.shtOdrDelivery.Redraw = True
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' マスタ適用リストバージョンコンボ選択イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbTglVersion_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbTglVersion.SelectedIndexChanged
        Dim sSql As String = ""
        Dim dtData As New DataTable
        Dim dtData2 As New DataTable
        Dim wkstr As String
        Dim nCnt As Integer

        If LbEventStop Then Exit Sub

        LfWaitCursor()

        Try
            '-------Ver0.1　北陸対応　MOD START-----------
            sSql = "SELECT CASE WHEN STA.STATION_NAME IS NULL" _
                & "             THEN '['+LIST.RAIL_SECTION_CODE+LIST.STATION_ORDER_CODE+']'" _
                & "             ELSE STA.STATION_NAME END AS STATION_NAME," _
                & "        CASE WHEN COM.CORNER_NAME IS NULL" _
                & "             THEN '['+CAST(LIST.CORNER_CODE AS varchar)+']'" _
                & "             ELSE COM.CORNER_NAME END AS CORNER_NAME," _
                & "        CASE WHEN MAC.UNIT_NO IS NULL" _
                & "             THEN '['+CAST(LIST.UNIT_NO AS varchar)+']'" _
                & "             ELSE CAST(MAC.UNIT_NO AS varchar) END AS UNIT_NO," _
                & "        CASE WHEN MAC.UNIT_NO IS NULL" _
                & "             THEN '0' ELSE '1' END AS OK_FLG," _
                & "        MAC.GROUP_NO" _
                & " FROM S_MST_LIST AS LIST LEFT OUTER JOIN v_station_mast AS STA" _
                & "   ON LIST.RAIL_SECTION_CODE+LIST.STATION_ORDER_CODE = STA.STATION_CODE" _
                & "   LEFT OUTER JOIN v_corner_mast AS COM ON LIST.RAIL_SECTION_CODE+LIST.STATION_ORDER_CODE" _
                & "   = COM.STATION_CODE AND LIST.CORNER_CODE=COM.CORNER_CODE" _
                & "   LEFT OUTER JOIN V_MACHINE_NOW AS MAC ON LIST.RAIL_SECTION_CODE+LIST.STATION_ORDER_CODE" _
                & "   = MAC.RAIL_SECTION_CODE+MAC.STATION_ORDER_CODE AND LIST.CORNER_CODE=MAC.CORNER_CODE" _
                & "   AND LIST.UNIT_NO=MAC.UNIT_NO AND MAC.MODEL_CODE='" & Me.cmbModel.SelectedValue.ToString & "'" _
                & " WHERE LIST.FILE_NAME='" & Me.cmbTglVersion.SelectedValue.ToString & "'" _
                & " ORDER BY OK_FLG"
            '-------Ver0.1　北陸対応　MOD END-----------

            nCnt = BaseSqlDataTableFill(sSql, dtData)
            Select Case nCnt
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '該当なし
                    AlertBox.Show(Lexis.NoRecordsFound)
                    Exit Sub
            End Select

            Call initElTable(Me.shtTglConfirm)
            shtTglConfirm.MaxRows = dtData.Rows.Count         '抽出件数分の行を一覧に作成
            shtTglConfirm.Rows.SetAllRowsHeight(21)       '行高さを揃える
            shtTglConfirm.DataSource = dtData                 'データをセット

            For i As Integer = 0 To Me.shtTglConfirm.MaxRows - 1
                If Me.shtTglConfirm.Item(3, i).Text = "0" Then
                    shtTglConfirm.Rows(i).BackColor = Color.Yellow
                End If
                '-------Ver0.1　北陸対応　ADD START-----------
                '対象パターンNo.チェックで異常が無く、チェック実施ＯＮ状態である
                If bPatternChkResultFlg = True And bPatternChkEventFlg = True Then
                    '※１つ異常があればその後はチェックしない。
                    '対象パターンNo.チェック
                    bPatternChkResultFlg = checkPatternNo(shtTglConfirm.Item(4, i).Text)
                    '※対象パターンNo.チェック異常となってもここでは警告メッセージを出さない。（配信ボタン押下時に出す。）
                End If
                '-------Ver0.1　北陸対応　ADD END-----------
            Next

            sSql = "SELECT FILE_CREATE_DATE FROM S_MST_LIST_HEADLINE" _
                & " WHERE FILE_NAME='" & Me.cmbTglVersion.SelectedValue.ToString & "'"

            nCnt = BaseSqlDataTableFill(sSql, dtData2)
            Select Case nCnt
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '該当なし
                    AlertBox.Show(Lexis.NoRecordsFound)
                    Exit Sub
            End Select

            wkstr = dtData2.Rows(nCnt - 1).Item("FILE_CREATE_DATE").ToString
            Me.lblUpdateDate.Text = wkstr.Substring(0, 4) & "/" & wkstr.Substring(4, 2) & "/" & wkstr.Substring(6, 2)
            Me.lblMstName.Text = Me.cmbMstName.Text
            Me.lblTglVer.Text = EkMasProListFileName.GetListVersion(Me.cmbTglVersion.SelectedValue.ToString)
            Me.lblMstVer.Text = EkMasProListFileName.GetDataVersion(Me.cmbTglVersion.SelectedValue.ToString)
            Me.lblPtnNo.Text = EkMasProListFileName.GetDataSubKind(Me.cmbTglVersion.SelectedValue.ToString)
            Me.lblModelCode.Text = EkMasProListFileName.GetDataApplicableModel(Me.cmbTglVersion.SelectedValue.ToString)

        Catch ex As OPMGException
            Log.Error("DataBaseGet failed.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' マスタ適用確認情報クリア。
    ''' </summary>
    Private Sub ClrTglList()

        lblUpdateDate.Text = ""
        lblMstName.Text = ""
        lblTglVer.Text = ""
        lblMstVer.Text = ""
        lblPtnNo.Text = ""
        lblModelCode.Text = ""

    End Sub
    '-------Ver0.1　北陸対応　ADD START-----------
    ''' <summary>
    ''' 対象パターンNo.チェック処理
    ''' </summary>
    ''' <param name="sGroupNo"></param>
    ''' <remarks>選択したパターンNo.が、適用リスト内の駅のグループ番号のパターン範囲内かチェックする。
    ''' チェック内容文字列（カンマ区切り）"グループ番号,パターン下限,パターン上限"</remarks>
    ''' <returns>正常（True）、異常（False）</returns>
    Private Function checkPatternNo(ByVal sGroupNo As String) As Boolean
        Dim bRtn As Boolean = False
        Dim i As Integer
        Dim sArrCheckInfo() As String

        Try
            'グループ番号が取得出来ていなければ正常終了
            If String.IsNullOrEmpty(sGroupNo) Then
                bRtn = True
                Exit Try
            End If
            '範囲の登録がINIファイルに無ければ正常終了
            If Config.MstLimitEkiCode(0) Is Nothing Then
                bRtn = True
                Exit Try
            End If

            'INIファイルの登録数分チェックする
            For i = 0 To Config.MstLimitEkiCode.Count - 1
                'チェック内容の文字列を分割し取り出す。
                sArrCheckInfo = Nothing
                sArrCheckInfo = Split(Config.MstLimitEkiCode(i).ToString, ",")

                'グループ番号をチェック
                If CInt(sArrCheckInfo(0)) = CInt(sGroupNo) Then
                    '選択したパターンNo.が、適用リスト内の駅のグループ番号のパターン範囲内かチェック
                    If CInt(sArrCheckInfo(1)) <= CInt(cmbPtnName.SelectedValue) And
                       CInt(cmbPtnName.SelectedValue) <= CInt(sArrCheckInfo(2)) Then
                        '範囲内：正常終了
                        bRtn = True
                        Exit Try
                    Else
                        '範囲外：異常終了
                        '※ここでは警告メッセージを出さない。（配信ボタン押下時に出す。）
                        Exit Try
                    End If
                End If
            Next
            'チェック対象外は正常終了
            bRtn = True

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"予期せぬエラーが発生しました。"
            '※ここでは警告メッセージを出さない。（配信ボタン押下時に出す。）
        End Try

        Return bRtn
    End Function
    '-------Ver0.1　北陸対応　ADD END-----------
End Class
