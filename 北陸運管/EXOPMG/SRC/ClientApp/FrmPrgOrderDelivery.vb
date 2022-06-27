' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
'   0.1      2014/06/01  　　金沢  リスト異常時配信ボタン表示対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO

''' <summary>
''' プログラム配信指示設定
''' </summary>
''' <remarks>プログラム管理メニューより「配信指示設定」ボタンをクリックすることにより、本画面を表示する。
''' 本画面にて配信情報、配信データ、配信先を指定し、配信指示を行う。</remarks>
Public Class FrmPrgOrderDelivery
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
    Friend WithEvents cmbAreaName As System.Windows.Forms.ComboBox
    Friend WithEvents grpDeliveryCnd As System.Windows.Forms.GroupBox
    Friend WithEvents chkbxForceDlv As System.Windows.Forms.CheckBox
    Friend WithEvents shtTdlApplied As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents lblMdlCode As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents lblPrgVer As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblAreaName As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblTdlVer As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblPrgName As System.Windows.Forms.Label
    Friend WithEvents lblPrgNa2 As System.Windows.Forms.Label
    Friend WithEvents lblCreateDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmbTdlVersion As System.Windows.Forms.ComboBox
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tabOdrDelivery As System.Windows.Forms.TabControl
    Friend WithEvents tabpDeliveryData As System.Windows.Forms.TabPage
    Friend WithEvents tabpTdlConfirm As System.Windows.Forms.TabPage
    Friend WithEvents grpDeliveryInf As System.Windows.Forms.GroupBox
    Friend WithEvents lblApplyArea As System.Windows.Forms.Label
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents lblModel As System.Windows.Forms.Label
    Friend WithEvents grpDeliveryData As System.Windows.Forms.GroupBox
    Friend WithEvents grpDeliveryLst As System.Windows.Forms.GroupBox
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents shtPrgDelivery As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents btnDelivery As System.Windows.Forms.Button
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmPrgOrderDelivery))
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnDelivery = New System.Windows.Forms.Button()
        Me.tabOdrDelivery = New System.Windows.Forms.TabControl()
        Me.tabpDeliveryData = New System.Windows.Forms.TabPage()
        Me.grpDeliveryCnd = New System.Windows.Forms.GroupBox()
        Me.chkbxForceDlv = New System.Windows.Forms.CheckBox()
        Me.grpDeliveryInf = New System.Windows.Forms.GroupBox()
        Me.cmbAreaName = New System.Windows.Forms.ComboBox()
        Me.lblApplyArea = New System.Windows.Forms.Label()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.grpDeliveryData = New System.Windows.Forms.GroupBox()
        Me.shtPrgDelivery = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.tabpTdlConfirm = New System.Windows.Forms.TabPage()
        Me.grpDeliveryLst = New System.Windows.Forms.GroupBox()
        Me.shtTdlApplied = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.lblMdlCode = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblPrgVer = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblAreaName = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblTdlVer = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblPrgName = New System.Windows.Forms.Label()
        Me.lblPrgNa2 = New System.Windows.Forms.Label()
        Me.lblCreateDate = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmbTdlVersion = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.tabOdrDelivery.SuspendLayout()
        Me.tabpDeliveryData.SuspendLayout()
        Me.grpDeliveryCnd.SuspendLayout()
        Me.grpDeliveryInf.SuspendLayout()
        Me.grpDeliveryData.SuspendLayout()
        CType(Me.shtPrgDelivery, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabpTdlConfirm.SuspendLayout()
        Me.grpDeliveryLst.SuspendLayout()
        CType(Me.shtTdlApplied, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.lblToday.Text = "2013/08/02(金)  15:43"
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
        Me.tabOdrDelivery.Controls.Add(Me.tabpTdlConfirm)
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
        Me.grpDeliveryCnd.Controls.Add(Me.chkbxForceDlv)
        Me.grpDeliveryCnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryCnd.Location = New System.Drawing.Point(455, 20)
        Me.grpDeliveryCnd.Name = "grpDeliveryCnd"
        Me.grpDeliveryCnd.Size = New System.Drawing.Size(291, 84)
        Me.grpDeliveryCnd.TabIndex = 14
        Me.grpDeliveryCnd.TabStop = False
        Me.grpDeliveryCnd.Text = "配信条件"
        '
        'chkbxForceDlv
        '
        Me.chkbxForceDlv.AutoSize = True
        Me.chkbxForceDlv.Location = New System.Drawing.Point(24, 25)
        Me.chkbxForceDlv.Name = "chkbxForceDlv"
        Me.chkbxForceDlv.Size = New System.Drawing.Size(286, 17)
        Me.chkbxForceDlv.TabIndex = 0
        Me.chkbxForceDlv.Text = "プログラム＋ﾌﾟﾛｸﾞﾗﾑ適用ﾘｽﾄ強制配信"
        Me.chkbxForceDlv.UseVisualStyleBackColor = True
        '
        'grpDeliveryInf
        '
        Me.grpDeliveryInf.Controls.Add(Me.cmbAreaName)
        Me.grpDeliveryInf.Controls.Add(Me.lblApplyArea)
        Me.grpDeliveryInf.Controls.Add(Me.cmbModel)
        Me.grpDeliveryInf.Controls.Add(Me.lblModel)
        Me.grpDeliveryInf.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryInf.Location = New System.Drawing.Point(16, 20)
        Me.grpDeliveryInf.Name = "grpDeliveryInf"
        Me.grpDeliveryInf.Size = New System.Drawing.Size(410, 84)
        Me.grpDeliveryInf.TabIndex = 0
        Me.grpDeliveryInf.TabStop = False
        Me.grpDeliveryInf.Text = "配信情報"
        '
        'cmbAreaName
        '
        Me.cmbAreaName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAreaName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbAreaName.Location = New System.Drawing.Point(137, 52)
        Me.cmbAreaName.Name = "cmbAreaName"
        Me.cmbAreaName.Size = New System.Drawing.Size(210, 21)
        Me.cmbAreaName.TabIndex = 47
        '
        'lblApplyArea
        '
        Me.lblApplyArea.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblApplyArea.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblApplyArea.Location = New System.Drawing.Point(24, 52)
        Me.lblApplyArea.Name = "lblApplyArea"
        Me.lblApplyArea.Size = New System.Drawing.Size(120, 18)
        Me.lblApplyArea.TabIndex = 46
        Me.lblApplyArea.Text = "適用エリア名称"
        Me.lblApplyArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.Location = New System.Drawing.Point(137, 22)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(162, 21)
        Me.cmbModel.TabIndex = 0
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(24, 24)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(120, 18)
        Me.lblModel.TabIndex = 2
        Me.lblModel.Text = "機種"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'grpDeliveryData
        '
        Me.grpDeliveryData.Controls.Add(Me.shtPrgDelivery)
        Me.grpDeliveryData.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryData.Location = New System.Drawing.Point(16, 110)
        Me.grpDeliveryData.Name = "grpDeliveryData"
        Me.grpDeliveryData.Size = New System.Drawing.Size(730, 399)
        Me.grpDeliveryData.TabIndex = 1
        Me.grpDeliveryData.TabStop = False
        Me.grpDeliveryData.Text = "配信データ"
        '
        'shtPrgDelivery
        '
        Me.shtPrgDelivery.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtPrgDelivery.Data = CType(resources.GetObject("shtPrgDelivery.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtPrgDelivery.Location = New System.Drawing.Point(27, 33)
        Me.shtPrgDelivery.Name = "shtPrgDelivery"
        Me.shtPrgDelivery.Size = New System.Drawing.Size(686, 311)
        Me.shtPrgDelivery.TabIndex = 0
        '
        'tabpTdlConfirm
        '
        Me.tabpTdlConfirm.Controls.Add(Me.grpDeliveryLst)
        Me.tabpTdlConfirm.Location = New System.Drawing.Point(4, 23)
        Me.tabpTdlConfirm.Name = "tabpTdlConfirm"
        Me.tabpTdlConfirm.Size = New System.Drawing.Size(804, 569)
        Me.tabpTdlConfirm.TabIndex = 1
        Me.tabpTdlConfirm.Text = "ﾌﾟﾛｸﾞﾗﾑ適用確認"
        '
        'grpDeliveryLst
        '
        Me.grpDeliveryLst.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grpDeliveryLst.Controls.Add(Me.shtTdlApplied)
        Me.grpDeliveryLst.Controls.Add(Me.lblMdlCode)
        Me.grpDeliveryLst.Controls.Add(Me.Label9)
        Me.grpDeliveryLst.Controls.Add(Me.lblPrgVer)
        Me.grpDeliveryLst.Controls.Add(Me.Label7)
        Me.grpDeliveryLst.Controls.Add(Me.lblAreaName)
        Me.grpDeliveryLst.Controls.Add(Me.Label5)
        Me.grpDeliveryLst.Controls.Add(Me.lblTdlVer)
        Me.grpDeliveryLst.Controls.Add(Me.Label4)
        Me.grpDeliveryLst.Controls.Add(Me.lblPrgName)
        Me.grpDeliveryLst.Controls.Add(Me.lblPrgNa2)
        Me.grpDeliveryLst.Controls.Add(Me.lblCreateDate)
        Me.grpDeliveryLst.Controls.Add(Me.Label2)
        Me.grpDeliveryLst.Controls.Add(Me.cmbTdlVersion)
        Me.grpDeliveryLst.Controls.Add(Me.Label1)
        Me.grpDeliveryLst.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryLst.Location = New System.Drawing.Point(28, 22)
        Me.grpDeliveryLst.Name = "grpDeliveryLst"
        Me.grpDeliveryLst.Size = New System.Drawing.Size(752, 534)
        Me.grpDeliveryLst.TabIndex = 12
        Me.grpDeliveryLst.TabStop = False
        '
        'shtTdlApplied
        '
        Me.shtTdlApplied.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtTdlApplied.Data = CType(resources.GetObject("shtTdlApplied.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtTdlApplied.Location = New System.Drawing.Point(67, 156)
        Me.shtTdlApplied.Name = "shtTdlApplied"
        Me.shtTdlApplied.Size = New System.Drawing.Size(442, 309)
        Me.shtTdlApplied.TabIndex = 86
        '
        'lblMdlCode
        '
        Me.lblMdlCode.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMdlCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMdlCode.Location = New System.Drawing.Point(619, 101)
        Me.lblMdlCode.Name = "lblMdlCode"
        Me.lblMdlCode.Size = New System.Drawing.Size(21, 18)
        Me.lblMdlCode.TabIndex = 85
        Me.lblMdlCode.Text = "X"
        Me.lblMdlCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(530, 101)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 18)
        Me.Label9.TabIndex = 84
        Me.Label9.Text = "機種コード"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPrgVer
        '
        Me.lblPrgVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrgVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrgVer.Location = New System.Drawing.Point(338, 101)
        Me.lblPrgVer.Name = "lblPrgVer"
        Me.lblPrgVer.Size = New System.Drawing.Size(69, 18)
        Me.lblPrgVer.TabIndex = 83
        Me.lblPrgVer.Text = "12345678"
        Me.lblPrgVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(275, 101)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(67, 18)
        Me.Label7.TabIndex = 82
        Me.Label7.Text = "代表Ver"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAreaName
        '
        Me.lblAreaName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAreaName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAreaName.Location = New System.Drawing.Point(107, 101)
        Me.lblAreaName.Name = "lblAreaName"
        Me.lblAreaName.Size = New System.Drawing.Size(150, 18)
        Me.lblAreaName.TabIndex = 81
        Me.lblAreaName.Text = "全角＿＿＿＿＿＿１０"
        Me.lblAreaName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(29, 101)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(78, 18)
        Me.Label5.TabIndex = 80
        Me.Label5.Text = "エリア名称"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTdlVer
        '
        Me.lblTdlVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTdlVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTdlVer.Location = New System.Drawing.Point(707, 73)
        Me.lblTdlVer.Name = "lblTdlVer"
        Me.lblTdlVer.Size = New System.Drawing.Size(27, 18)
        Me.lblTdlVer.TabIndex = 79
        Me.lblTdlVer.Text = "01"
        Me.lblTdlVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(530, 76)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(183, 19)
        Me.Label4.TabIndex = 78
        Me.Label4.Text = "プログラム適用リストVer"
        '
        'lblPrgName
        '
        Me.lblPrgName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrgName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrgName.Location = New System.Drawing.Point(306, 69)
        Me.lblPrgName.Name = "lblPrgName"
        Me.lblPrgName.Size = New System.Drawing.Size(218, 25)
        Me.lblPrgName.TabIndex = 77
        Me.lblPrgName.Text = "全角＿＿＿＿＿＿＿＿＿＿＿１５"
        Me.lblPrgName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPrgNa2
        '
        Me.lblPrgNa2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrgNa2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrgNa2.Location = New System.Drawing.Point(200, 75)
        Me.lblPrgNa2.Name = "lblPrgNa2"
        Me.lblPrgNa2.Size = New System.Drawing.Size(106, 15)
        Me.lblPrgNa2.TabIndex = 76
        Me.lblPrgNa2.Text = "プログラム名称"
        '
        'lblCreateDate
        '
        Me.lblCreateDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblCreateDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblCreateDate.Location = New System.Drawing.Point(107, 72)
        Me.lblCreateDate.Name = "lblCreateDate"
        Me.lblCreateDate.Size = New System.Drawing.Size(78, 18)
        Me.lblCreateDate.TabIndex = 75
        Me.lblCreateDate.Text = "2013/04/16"
        Me.lblCreateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(29, 73)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 18)
        Me.Label2.TabIndex = 74
        Me.Label2.Text = "作成日"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbTdlVersion
        '
        Me.cmbTdlVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTdlVersion.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbTdlVersion.Location = New System.Drawing.Point(253, 22)
        Me.cmbTdlVersion.Name = "cmbTdlVersion"
        Me.cmbTdlVersion.Size = New System.Drawing.Size(50, 21)
        Me.cmbTdlVersion.TabIndex = 72
        Me.cmbTdlVersion.Enabled = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(29, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(218, 18)
        Me.Label1.TabIndex = 73
        Me.Label1.Text = "プログラム適用リストバージョン"
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
        'FrmPrgOrderDelivery
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgOrderDelivery"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.tabOdrDelivery.ResumeLayout(False)
        Me.tabpDeliveryData.ResumeLayout(False)
        Me.grpDeliveryCnd.ResumeLayout(False)
        Me.grpDeliveryCnd.PerformLayout()
        Me.grpDeliveryInf.ResumeLayout(False)
        Me.grpDeliveryData.ResumeLayout(False)
        CType(Me.shtPrgDelivery, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabpTdlConfirm.ResumeLayout(False)
        Me.grpDeliveryLst.ResumeLayout(False)
        CType(Me.shtTdlApplied, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "各種宣言領域"

    'エリア名
    Public Const APPLIED_AREA As String = "適用エリア名称："
    '強制配信
    Public Const FORCED_DELIVERY As String = "強制配信："
    '作成日
    Public Const UPDATED_DATE As String = "作成日："
    'プログラム適用リスト
    Public Const PRG_APPLIEDLIST_VER As String = "プログラム適用リストバージョン："
    'エリア名
    Public Const AREANAME As String = "エリア名称："
    '代表バージョン
    Public Const PROVER As String = "代表バージョン："
    '機種コード
    Public Const MODELCODE As String = "機種コード："

    Private LbInitCallFlg As Boolean = False

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    ''' 出力用テンプレートファイル名
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "プログラム配信指示設定（配信データ）.xls"

    ''' <summary>
    ''' 出力用テンプレートファイル名
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName2 As String = "プログラム配信指示設定（プログラム適用確認）.xls"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "プログラム配信指示設定（配信データ）"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName2 As String = "プログラム配信指示設定（プログラム適用確認）"

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "プログラム配信指示設定"

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した配信データに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3, 4, 5}

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索したプログラム適用リストに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol2() As Integer = {0, 1, 2, 3}

    ''' <summary>
    ''' 配信対象のプログラム適用リストファイル名
    ''' </summary>
    Private LstListFile_Name As String

#End Region

#Region "フォームロード"
    ''' <summary>
    ''' フォームロード
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmPrgOrderDelivery_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        LfWaitCursor()
        If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
            If InitFrmData() = False Then   '初期処理
                Me.Close()
                Exit Sub
            End If
        End If
        'チェックボックス表示設定
        chkbxForceDlv.Text = "プログラム＋ﾌﾟﾛｸﾞﾗﾑ適用ﾘｽﾄ" & vbCrLf & "強制配信"
        Me.tabOdrDelivery.Focus()
        LfWaitCursor(False)

    End Sub
#End Region

#Region "プログラム配信指示設定画面のデータを準備する"
    ''' <summary>
    ''' プログラム配信指示設定画面のデータを準備する
    ''' </summary>
    ''' <remarks>
    '''プログラム配信指示設定データを検索し、画面に表示する
    ''' </remarks>   
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean

        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ

        Try
            Log.Info("Method started.")

            shtPrgDelivery.TransformEditor = False                                    '一覧の列種類毎のチェックを無効にする
            shtTdlApplied.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtTdlApplied.ViewMode = ElTabelleSheet.ViewMode.Row                      '行選択モード
            shtTdlApplied.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード


            '画面タイトル
            lblTitle.Text = LcstFormTitle

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
            If setCmbModel() = False Then Exit Try
            cmbModel.SelectedIndex = 0            'デフォルト表示項目

            '適用エリア名称コンボボックスを設定する。
            If setCmbAreaName(cmbModel.SelectedValue.ToString) = False Then Exit Try
            cmbAreaName.SelectedIndex = 0            'デフォルト表示項目

            'ELTable の初期化
            Call initElTable(Me.shtPrgDelivery)
            Call initElTable(Me.shtTdlApplied)
            Call ClrTdlList()

            Me.btnPrint.Enabled = False
            Me.btnDelivery.Enabled = False

            bRtn = True

        Catch ex As Exception
            '画面表示処理に失敗しました
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

#Region "プログラム適用リストVerの変更時イベント(一覧内)"
    ''' <summary>
    ''' 適用リストVer指定時のイベント処理
    ''' </summary>
    ''' <remarks>バージョン変更時、適用リスト内容を適用リストの内容一覧へ展開。</remarks>
    Private Sub shtOdrDelivery_CellNotify(ByVal sender As Object, _
        ByVal e As GrapeCity.Win.ElTabelleSheet.CellNotifyEventArgs) Handles shtPrgDelivery.CellNotify

        If e.Name <> ElTabelleSheet.CellNotifyEvents.SelectedIndexChanged Then Exit Sub

        Dim cmbEdt As ElTabelleSheet.Editors.SuperiorComboEditor
        Dim nIndex As Integer
        Dim lstCmbItems As New ArrayList()

        LfWaitCursor(True)
        Me.btnDelivery.Enabled = False

        '画面の閃きを防ぐため
        Me.shtPrgDelivery.Redraw = False

        Try
            '選択されたコンボボックスのインデックスを取得
            cmbEdt = CType(Me.shtPrgDelivery.Item(1, e.Position.Row).Editor, ElTabelleSheet.Editors.SuperiorComboEditor)
            For i As Integer = 1 To cmbEdt.Items.Count - 1
                If cmbEdt.Items(i).Selected = True Then
                    nIndex = i
                End If
                lstCmbItems.Add(New DictionaryEntry(cmbEdt.Items(i).Content.ToString, cmbEdt.Items(i).Value.ToString))
            Next

            'バージョンを選択されたら
            If nIndex <> 0 Then
                '選択されたバージョンのプログラム適用ファイル名を取得
                LstListFile_Name = cmbEdt.Items(nIndex).Value.ToString

                'プログラム適用確認タブのコンボボックスを生成
                If nIndex <> 1 Then
                    LbEventStop = True      'イベント発生ＯＦＦ
                End If
                cmbTdlVersion.DisplayMember = "Key"
                cmbTdlVersion.ValueMember = "Value"
                cmbTdlVersion.DataSource = lstCmbItems
                LbEventStop = False      'イベント発生ＯＦＦ
                cmbTdlVersion.SelectedIndex = nIndex - 1

                '選択された行以外のコンボボックスをクリア
                For i As Integer = 0 To Me.shtPrgDelivery.MaxRows - 1
                    If i <> e.Position.Row Then
                        Me.shtPrgDelivery.Item(1, i).Text = ""
                    End If
                Next
                '-------Ver0.1　リスト異常時配信ボタン表示対応　ADD START-----------
                Me.btnDelivery.Enabled = True
                '-------Ver0.1　リスト異常時配信ボタン表示対応　ADD END-----------
            Else
                '空白を選択されたら
                LstListFile_Name = ""
                LbEventStop = True      'イベント発生ＯＦＦ
                cmbTdlVersion.DataSource = Nothing
                cmbTdlVersion.Items.Clear()

                Call initElTable(Me.shtTdlApplied)
                '-------Ver0.1　リスト異常時配信ボタン表示対応　ADD START-----------
                Me.btnDelivery.Enabled = False
                '-------Ver0.1　リスト異常時配信ボタン表示対応　ADD END-----------
                'ラベル活性化
                Call ClrTdlList()
                LbEventStop = False      'イベント発生ＯＦＦ

            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, cmbModel.Text)

        Finally
            lstCmbItems = Nothing
            '画面の閃きを防ぐため
            Me.shtPrgDelivery.Redraw = True
            LfWaitCursor(False)

        End Try

    End Sub
#End Region

#Region "「出力」ボタンクリック"

    ''' <summary>
    ''' 「出力」ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
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
            cmbModel.Select()

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
            If cmbTdlVersion.Text <> String.Empty Then
                LfXlsStart2(sFilePath)
                cmbTdlVersion.Select()
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
    ''' <remarks>配信先として全駅を選択した場合と配信対象リストにデータが存在している場合に、活性化する。
    ''' 「配信」ボタンをクリックすることにより、「配信対象リスト」にて指定された配信対象の機器に対して配信指示を行う。
    ''' （配信先として全駅が選択されている場合は、全駅の機器に対して配信指示を行う。）
    ''' 配信指示が終了した場合は、指示完了のポップアップ画面を表示する。
    ''' </remarks>
    Private Sub btnDoHaishin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelivery.Click
        Try
            LogOperation(sender, e)    'ボタン押下ログ

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
            Dim ullResult As MasProDllInvokeResult = OpClientUtil.InvokeMasProDll(sListFileName, chkbxForceDlv.Checked)

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
    ''' <remarks></remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()
    End Sub
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

#Region "プログラム配信指示設定（プログラム適用確認）　帳票出力"
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
                .Cell("P1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("P2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B4").Value = UPDATED_DATE + Me.lblCreateDate.Text.Trim
                .Cell("B5").Value = OPMGFormConstants.PRO_NAME + Me.lblPrgName.Text.Trim
                .Cell("B6").Value = PRG_APPLIEDLIST_VER + Me.lblTdlVer.Text.Trim
                .Cell("B7").Value = AREANAME + Me.lblAreaName.Text.Trim
                .Cell("B8").Value = PROVER + Me.lblPrgVer.Text.Trim
                .Cell("B9").Value = MODELCODE + Me.lblMdlCode.Text.Trim

                ' 配信対象のデータ数を取得します
                nRecCnt = shtTdlApplied.MaxRows

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
                        .Pos(x + 1, y + nStartRow).Value = shtTdlApplied.Item(LcstPrntCol2(x), y).Text
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

#Region "プログラム配信指示設定（配信データ）　帳票出力"
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
                If Me.chkbxForceDlv.Checked = False Then
                    .Cell("F6").Value = FORCED_DELIVERY + "無"
                Else
                    .Cell("F6").Value = FORCED_DELIVERY + "有"
                End If
                .Cell("B7").Value = APPLIED_AREA + Me.cmbAreaName.Text.Trim

                ' 配信対象のデータ数を取得します
                nRecCnt = shtPrgDelivery.MaxRows

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtPrgDelivery.Item(LcstPrntCol(x), y).Text
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

#Region "コンボボックスのクリックイベント"
    ''' <summary>
    ''' 機種コンボ選択イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbModel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            '適用エリアコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbAreaName(cmbModel.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblApplyArea.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbAreaName.SelectedIndex = 0               '★イベント発生箇所
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblModel.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 適用エリアコンボ選択イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbAreaName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbAreaName.SelectedIndexChanged
        Dim sSql As String = ""
        Dim dtData As New DataTable
        Dim nCnt As Integer
        Dim RowCnt As Integer
        Dim sKey As String = ""
        Dim cmbEdt As ElTabelleSheet.Editors.SuperiorComboEditor = Nothing
        Dim Ar As New ArrayList

        If LbEventStop Then Exit Sub

        LstListFile_Name = ""

        Call initElTable(Me.shtPrgDelivery)
        Call initElTable(Me.shtTdlApplied)
        Call ClrTdlList()
        LbEventStop = True      'イベント発生ＯＮ
        cmbTdlVersion.DataSource = Nothing
        cmbTdlVersion.Items.Clear()

        LbEventStop = False      'イベント発生ＯＮ
        Me.btnPrint.Enabled = False
        Me.btnDelivery.Enabled = False

        If cmbAreaName.SelectedIndex = 0 Then
            Exit Sub
        End If

        LfWaitCursor()

        Try

            sSql = "SELECT" _
                & "     LST.NAME AS L_NAME,LST.LIST_VERSION,LST.FILE_NAME,DAT.NAME AS D_NAME," _
                & "     LST.DATA_VERSION,DAT.UPDATE_DATE," _
                & "     Convert(Varchar(10),Convert(DateTime,DAT.RUNNABLE_DATE),111) AS DATE" _
                & " FROM" _
                & "     (" _
                & "         SELECT" _
                & "             LS.MODEL_CODE,LS.DATA_KIND,NAME,DATA_SUB_KIND,DATA_VERSION," _
                & "             LIST_VERSION,FILE_NAME" _
                & "         FROM" _
                & "             S_PRG_LIST_HEADLINE AS LS," _
                & "             M_PRG_NAME AS MS" _
                & "         WHERE" _
                & "             LS.MODEL_CODE = MS.MODEL_CODE" _
                & "         AND LS.DATA_KIND = MS.DATA_KIND" _
                & "         AND MS.FILE_KBN = 'LST'" _
                & "     ) AS LST," _
                & "     (" _
                & "         SELECT" _
                & "             DT.MODEL_CODE,DT.DATA_KIND,DT.UPDATE_DATE,NAME," _
                & "             DATA_SUB_KIND,DATA_VERSION,RUNNABLE_DATE" _
                & "         FROM" _
                & "             S_PRG_DATA_HEADLINE AS DT," _
                & "             M_PRG_NAME AS M2" _
                & "         WHERE" _
                & "             DT.MODEL_CODE = M2.MODEL_CODE" _
                & "         AND DT.DATA_KIND = M2.DATA_KIND" _
                & "         AND M2.FILE_KBN = 'DAT'" _
                & "     ) AS DAT" _
                & " WHERE" _
                & "     LST.MODEL_CODE = DAT.MODEL_CODE" _
                & " AND LST.DATA_KIND = DAT.DATA_KIND" _
                & " AND LST.DATA_SUB_KIND = DAT.DATA_SUB_KIND" _
                & " AND LST.DATA_VERSION = DAT.DATA_VERSION" _
                & " AND LST.MODEL_CODE = '" & cmbModel.SelectedValue.ToString & "'" _
                & " AND LST.DATA_SUB_KIND = '" & cmbAreaName.SelectedValue.ToString & "'" _
                & " ORDER BY" _
                & "     LST.DATA_VERSION"

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
            Me.shtPrgDelivery.Redraw = False

            'データがある場合、データの行数によって再度Eltableの最大桁数を設定する。
            If Me.shtPrgDelivery.MaxRows < nCnt Then
                Me.shtPrgDelivery.MaxRows = nCnt
            End If
            RowCnt = 0
            '動的にデータを追加する。
            For i As Integer = 0 To nCnt - 1
                If sKey <> dtData.Rows(i).Item("DATA_VERSION").ToString Then
                    If i <> 0 Then
                        'cmbEdt.Editable = False
                        Me.shtPrgDelivery.Item(0, RowCnt).Text = dtData.Rows(i - 1).Item("L_NAME").ToString
                        Me.shtPrgDelivery.Item(1, RowCnt).Editor = cmbEdt
                        Me.shtPrgDelivery.Item(2, RowCnt).Text = dtData.Rows(i - 1).Item("D_NAME").ToString
                        Me.shtPrgDelivery.Item(3, RowCnt).Text = dtData.Rows(i - 1).Item("DATA_VERSION").ToString
                        Me.shtPrgDelivery.Item(4, RowCnt).Text _
                            = Format(Convert.ToDateTime(dtData.Rows(i - 1).Item("UPDATE_DATE")), "yyyy/MM/dd")
                        Me.shtPrgDelivery.Item(5, RowCnt).Text = dtData.Rows(i - 1).Item("DATE").ToString
                        RowCnt = RowCnt + 1
                    End If

                    cmbEdt = New ElTabelleSheet.Editors.SuperiorComboEditor
                    cmbEdt.Editable = False
                    cmbEdt.Items.Add(New GrapeCity.Win.ElTabelleSheet.Editors.ComboItem(0, Nothing, "", "", ""))
                    sKey = dtData.Rows(i).Item("DATA_VERSION").ToString
                End If
                cmbEdt.Items.Add(New GrapeCity.Win.ElTabelleSheet.Editors.ComboItem(0, Nothing, _
                    dtData.Rows(i).Item("LIST_VERSION").ToString, "", dtData.Rows(i).Item("FILE_NAME").ToString))

            Next

            'cmbEdt.Editable = False
            Me.shtPrgDelivery.Item(0, RowCnt).Text = dtData.Rows(nCnt - 1).Item("L_NAME").ToString
            Me.shtPrgDelivery.Item(1, RowCnt).Editor = cmbEdt
            Me.shtPrgDelivery.Item(2, RowCnt).Text = dtData.Rows(nCnt - 1).Item("D_NAME").ToString
            Me.shtPrgDelivery.Item(3, RowCnt).Text = dtData.Rows(nCnt - 1).Item("DATA_VERSION").ToString
            Me.shtPrgDelivery.Item(4, RowCnt).Text _
                = Format(Convert.ToDateTime(dtData.Rows(nCnt - 1).Item("UPDATE_DATE")), "yyyy/MM/dd")
            Me.shtPrgDelivery.Item(5, RowCnt).Text = dtData.Rows(nCnt - 1).Item("DATE").ToString

            Me.shtPrgDelivery.MaxRows = RowCnt + 1
            Me.shtPrgDelivery.Rows.SetAllRowsHeight(21)

            Me.btnPrint.Enabled = True

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblApplyArea.Text)
        Finally
            Me.shtPrgDelivery.Redraw = True
            LfWaitCursor(False)
        End Try


    End Sub

    ''' <summary>
    ''' プログラム適用リストバージョンコンボ選択イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbTdlVersion_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbTdlVersion.SelectedIndexChanged
        Dim sSql As String = ""
        Dim dtData As New DataTable
        Dim dtData2 As New DataTable
        Dim nCnt As Integer

        If LbEventStop Then Exit Sub

        LfWaitCursor()

        Try

            sSql = "SELECT CASE WHEN STA.STATION_NAME IS NULL" _
                & "             THEN '['+LIST.RAIL_SECTION_CODE+LIST.STATION_ORDER_CODE+']'" _
                & "             ELSE STA.STATION_NAME END AS STATION_NAME," _
                & "        CASE WHEN COM.CORNER_NAME IS NULL" _
                & "             THEN '['+CAST(LIST.CORNER_CODE AS varchar)+']'" _
                & "             ELSE COM.CORNER_NAME END AS CORNER_NAME," _
                & "        CASE WHEN MAC.UNIT_NO IS NULL" _
                & "             THEN '['+CAST(LIST.UNIT_NO AS varchar)+']'" _
                & "             ELSE CAST(MAC.UNIT_NO AS varchar) END AS UNIT_NO," _
                & "        CASE WHEN LEN(LIST.APPLICABLE_DATE)=8" _
                & "             THEN SUBSTRING(LIST.APPLICABLE_DATE,1,4)+'/'+SUBSTRING(LIST.APPLICABLE_DATE,5,2)+'/'" _
                & "             +SUBSTRING(LIST.APPLICABLE_DATE,7,2) ELSE LIST.APPLICABLE_DATE END AS DATE," _
                & "        CASE WHEN MAC.UNIT_NO IS NULL" _
                & "             THEN '0' ELSE '1' END AS OK_FLG" _
                & " FROM S_PRG_LIST AS LIST LEFT OUTER JOIN v_station_mast AS STA" _
                & "   ON LIST.RAIL_SECTION_CODE+LIST.STATION_ORDER_CODE = STA.STATION_CODE" _
                & "   LEFT OUTER JOIN v_corner_mast AS COM ON LIST.RAIL_SECTION_CODE+LIST.STATION_ORDER_CODE" _
                & "   = COM.STATION_CODE AND LIST.CORNER_CODE=COM.CORNER_CODE" _
                & "   LEFT OUTER JOIN V_MACHINE_NOW AS MAC ON LIST.RAIL_SECTION_CODE+LIST.STATION_ORDER_CODE" _
                & "   = MAC.RAIL_SECTION_CODE+MAC.STATION_ORDER_CODE AND LIST.CORNER_CODE=MAC.CORNER_CODE" _
                & "   AND LIST.UNIT_NO=MAC.UNIT_NO AND MAC.MODEL_CODE='" & Me.cmbModel.SelectedValue.ToString & "'" _
                & " WHERE LIST.FILE_NAME='" & Me.cmbTdlVersion.SelectedValue.ToString & "'" _
                & " ORDER BY OK_FLG"

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

            Call initElTable(Me.shtTdlApplied)
            shtTdlApplied.MaxRows = dtData.Rows.Count         '抽出件数分の行を一覧に作成
            shtTdlApplied.Rows.SetAllRowsHeight(21)       '行高さを揃える
            shtTdlApplied.DataSource = dtData                 'データをセット

            For i As Integer = 0 To Me.shtTdlApplied.MaxRows - 1
                If Me.shtTdlApplied.Item(4, i).Text = "0" Then
                    shtTdlApplied.Rows(i).BackColor = Color.Yellow
                End If
            Next

            sSql = "SELECT Convert(Varchar(10), Convert(DateTime, FILE_CREATE_DATE), 111)" _
                & "  AS DATE FROM S_PRG_LIST_HEADLINE WHERE FILE_NAME='" _
                & Me.cmbTdlVersion.SelectedValue.ToString & "'"

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

            Me.lblCreateDate.Text = dtData2.Rows(nCnt - 1).Item("DATE").ToString
            Me.lblPrgName.Text = Me.shtPrgDelivery.Item(2, 0).Text
            Me.lblTdlVer.Text = EkMasProListFileName.GetListVersion(Me.cmbTdlVersion.SelectedValue.ToString)
            Me.lblAreaName.Text = Me.cmbAreaName.Text
            Me.lblPrgVer.Text = EkMasProListFileName.GetDataVersion(Me.cmbTdlVersion.SelectedValue.ToString)
            Me.lblMdlCode.Text = EkMasProListFileName.GetDataApplicableModel(Me.cmbTdlVersion.SelectedValue.ToString)
            '------Ver0.1　リスト異常時配信ボタン表示対応　ADD　START-----------------------
            ''配信データで指定された適用リストが機器構成上、合っていれば「配信」ボタンを活性化
            'If LstListFile_Name = Me.cmbTdlVersion.SelectedValue.ToString Then
            '    If Me.shtTdlApplied.Item(4, 0).Text = "1" Then
            '        btnDelivery.Enabled = True
            '    End If
            'End If

            '------Ver0.1　リスト異常時配信ボタン表示対応　ADD　END---------------------------
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
            dt = oMst.SelectTable(True)
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
    ''' 適用エリア名称コンボボックスを設定する。
    ''' </summary>
    ''' <param name="Model">機種コード</param>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理しているパターン名称の一覧及び「空白」を設定する。</remarks>
    Private Function setCmbAreaName(ByVal Model As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As AreaMaster
        oMst = New AreaMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If (Model <> "" AndAlso Model <> ClientDaoConstants.TERMINAL_ALL) Then
                dt = oMst.SelectTable(Model)
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbAreaName)
            cmbAreaName.SelectedIndex = -1
            If cmbAreaName.Items.Count <= 0 Then bRtn = False
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

    ''' <summary>
    ''' プログラム適用確認情報クリア。
    ''' </summary>
    Private Sub ClrTdlList()

        lblCreateDate.Text = ""
        lblPrgName.Text = ""
        lblTdlVer.Text = ""
        lblAreaName.Text = ""
        lblPrgVer.Text = ""
        lblMdlCode.Text = ""

    End Sub

End Class
