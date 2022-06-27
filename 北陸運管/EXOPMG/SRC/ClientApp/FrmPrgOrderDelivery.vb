' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
'   0.1      2014/06/01  �@�@����  ���X�g�ُ펞�z�M�{�^���\���Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO

''' <summary>
''' �v���O�����z�M�w���ݒ�
''' </summary>
''' <remarks>�v���O�����Ǘ����j���[���u�z�M�w���ݒ�v�{�^�����N���b�N���邱�Ƃɂ��A�{��ʂ�\������B
''' �{��ʂɂĔz�M���A�z�M�f�[�^�A�z�M����w�肵�A�z�M�w�����s���B</remarks>
Public Class FrmPrgOrderDelivery
    Inherits FrmBase

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    ' Form �́A�R���|�[�l���g�ꗗ�Ɍ㏈�������s���邽�߂� dispose ���I�[�o�[���C�h���܂��B
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Private components As System.ComponentModel.IContainer

    ' ���� : �ȉ��̃v���V�[�W���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B  
    ' �R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
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
        Me.lblToday.Text = "2013/08/02(��)  15:43"
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 4
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnDelivery
        '
        Me.btnDelivery.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDelivery.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelivery.Location = New System.Drawing.Point(872, 520)
        Me.btnDelivery.Name = "btnDelivery"
        Me.btnDelivery.Size = New System.Drawing.Size(128, 40)
        Me.btnDelivery.TabIndex = 3
        Me.btnDelivery.Text = "�z�@�M"
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
        Me.tabpDeliveryData.Text = "�z�M�f�[�^"
        '
        'grpDeliveryCnd
        '
        Me.grpDeliveryCnd.Controls.Add(Me.chkbxForceDlv)
        Me.grpDeliveryCnd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryCnd.Location = New System.Drawing.Point(455, 20)
        Me.grpDeliveryCnd.Name = "grpDeliveryCnd"
        Me.grpDeliveryCnd.Size = New System.Drawing.Size(291, 84)
        Me.grpDeliveryCnd.TabIndex = 14
        Me.grpDeliveryCnd.TabStop = False
        Me.grpDeliveryCnd.Text = "�z�M����"
        '
        'chkbxForceDlv
        '
        Me.chkbxForceDlv.AutoSize = True
        Me.chkbxForceDlv.Location = New System.Drawing.Point(24, 25)
        Me.chkbxForceDlv.Name = "chkbxForceDlv"
        Me.chkbxForceDlv.Size = New System.Drawing.Size(286, 17)
        Me.chkbxForceDlv.TabIndex = 0
        Me.chkbxForceDlv.Text = "�v���O�����{��۸��ѓK�pؽċ����z�M"
        Me.chkbxForceDlv.UseVisualStyleBackColor = True
        '
        'grpDeliveryInf
        '
        Me.grpDeliveryInf.Controls.Add(Me.cmbAreaName)
        Me.grpDeliveryInf.Controls.Add(Me.lblApplyArea)
        Me.grpDeliveryInf.Controls.Add(Me.cmbModel)
        Me.grpDeliveryInf.Controls.Add(Me.lblModel)
        Me.grpDeliveryInf.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryInf.Location = New System.Drawing.Point(16, 20)
        Me.grpDeliveryInf.Name = "grpDeliveryInf"
        Me.grpDeliveryInf.Size = New System.Drawing.Size(410, 84)
        Me.grpDeliveryInf.TabIndex = 0
        Me.grpDeliveryInf.TabStop = False
        Me.grpDeliveryInf.Text = "�z�M���"
        '
        'cmbAreaName
        '
        Me.cmbAreaName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAreaName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbAreaName.Location = New System.Drawing.Point(137, 52)
        Me.cmbAreaName.Name = "cmbAreaName"
        Me.cmbAreaName.Size = New System.Drawing.Size(210, 21)
        Me.cmbAreaName.TabIndex = 47
        '
        'lblApplyArea
        '
        Me.lblApplyArea.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblApplyArea.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblApplyArea.Location = New System.Drawing.Point(24, 52)
        Me.lblApplyArea.Name = "lblApplyArea"
        Me.lblApplyArea.Size = New System.Drawing.Size(120, 18)
        Me.lblApplyArea.TabIndex = 46
        Me.lblApplyArea.Text = "�K�p�G���A����"
        Me.lblApplyArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.Location = New System.Drawing.Point(137, 22)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(162, 21)
        Me.cmbModel.TabIndex = 0
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(24, 24)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(120, 18)
        Me.lblModel.TabIndex = 2
        Me.lblModel.Text = "�@��"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'grpDeliveryData
        '
        Me.grpDeliveryData.Controls.Add(Me.shtPrgDelivery)
        Me.grpDeliveryData.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpDeliveryData.Location = New System.Drawing.Point(16, 110)
        Me.grpDeliveryData.Name = "grpDeliveryData"
        Me.grpDeliveryData.Size = New System.Drawing.Size(730, 399)
        Me.grpDeliveryData.TabIndex = 1
        Me.grpDeliveryData.TabStop = False
        Me.grpDeliveryData.Text = "�z�M�f�[�^"
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
        Me.tabpTdlConfirm.Text = "��۸��ѓK�p�m�F"
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
        Me.grpDeliveryLst.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.lblMdlCode.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.Label9.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(530, 101)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 18)
        Me.Label9.TabIndex = 84
        Me.Label9.Text = "�@��R�[�h"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPrgVer
        '
        Me.lblPrgVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrgVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.Label7.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(275, 101)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(67, 18)
        Me.Label7.TabIndex = 82
        Me.Label7.Text = "��\Ver"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAreaName
        '
        Me.lblAreaName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAreaName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAreaName.Location = New System.Drawing.Point(107, 101)
        Me.lblAreaName.Name = "lblAreaName"
        Me.lblAreaName.Size = New System.Drawing.Size(150, 18)
        Me.lblAreaName.TabIndex = 81
        Me.lblAreaName.Text = "�S�p�Q�Q�Q�Q�Q�Q�P�O"
        Me.lblAreaName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label5.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(29, 101)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(78, 18)
        Me.Label5.TabIndex = 80
        Me.Label5.Text = "�G���A����"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTdlVer
        '
        Me.lblTdlVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTdlVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.Label4.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(530, 76)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(183, 19)
        Me.Label4.TabIndex = 78
        Me.Label4.Text = "�v���O�����K�p���X�gVer"
        '
        'lblPrgName
        '
        Me.lblPrgName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrgName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrgName.Location = New System.Drawing.Point(306, 69)
        Me.lblPrgName.Name = "lblPrgName"
        Me.lblPrgName.Size = New System.Drawing.Size(218, 25)
        Me.lblPrgName.TabIndex = 77
        Me.lblPrgName.Text = "�S�p�Q�Q�Q�Q�Q�Q�Q�Q�Q�Q�Q�P�T"
        Me.lblPrgName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPrgNa2
        '
        Me.lblPrgNa2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrgNa2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrgNa2.Location = New System.Drawing.Point(200, 75)
        Me.lblPrgNa2.Name = "lblPrgNa2"
        Me.lblPrgNa2.Size = New System.Drawing.Size(106, 15)
        Me.lblPrgNa2.TabIndex = 76
        Me.lblPrgNa2.Text = "�v���O��������"
        '
        'lblCreateDate
        '
        Me.lblCreateDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblCreateDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.Label2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(29, 73)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 18)
        Me.Label2.TabIndex = 74
        Me.Label2.Text = "�쐬��"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbTdlVersion
        '
        Me.cmbTdlVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTdlVersion.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbTdlVersion.Location = New System.Drawing.Point(253, 22)
        Me.cmbTdlVersion.Name = "cmbTdlVersion"
        Me.cmbTdlVersion.Size = New System.Drawing.Size(50, 21)
        Me.cmbTdlVersion.TabIndex = 72
        Me.cmbTdlVersion.Enabled = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label1.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(29, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(218, 18)
        Me.Label1.TabIndex = 73
        Me.Label1.Text = "�v���O�����K�p���X�g�o�[�W����"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(872, 456)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 2
        Me.btnPrint.Text = "�o�@��"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'FrmPrgOrderDelivery
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgOrderDelivery"
        Me.Text = "�^�p�[�� "
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

#Region "�e��錾�̈�"

    '�G���A��
    Public Const APPLIED_AREA As String = "�K�p�G���A���́F"
    '�����z�M
    Public Const FORCED_DELIVERY As String = "�����z�M�F"
    '�쐬��
    Public Const UPDATED_DATE As String = "�쐬���F"
    '�v���O�����K�p���X�g
    Public Const PRG_APPLIEDLIST_VER As String = "�v���O�����K�p���X�g�o�[�W�����F"
    '�G���A��
    Public Const AREANAME As String = "�G���A���́F"
    '��\�o�[�W����
    Public Const PROVER As String = "��\�o�[�W�����F"
    '�@��R�[�h
    Public Const MODELCODE As String = "�@��R�[�h�F"

    Private LbInitCallFlg As Boolean = False

    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    ''' �o�͗p�e���v���[�g�t�@�C����
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "�v���O�����z�M�w���ݒ�i�z�M�f�[�^�j.xls"

    ''' <summary>
    ''' �o�͗p�e���v���[�g�t�@�C����
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName2 As String = "�v���O�����z�M�w���ݒ�i�v���O�����K�p�m�F�j.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "�v���O�����z�M�w���ݒ�i�z�M�f�[�^�j"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName2 As String = "�v���O�����z�M�w���ݒ�i�v���O�����K�p�m�F�j"

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "�v���O�����z�M�w���ݒ�"

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������z�M�f�[�^�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3, 4, 5}

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������v���O�����K�p���X�g�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol2() As Integer = {0, 1, 2, 3}

    ''' <summary>
    ''' �z�M�Ώۂ̃v���O�����K�p���X�g�t�@�C����
    ''' </summary>
    Private LstListFile_Name As String

#End Region

#Region "�t�H�[�����[�h"
    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmPrgOrderDelivery_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        LfWaitCursor()
        If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
            If InitFrmData() = False Then   '��������
                Me.Close()
                Exit Sub
            End If
        End If
        '�`�F�b�N�{�b�N�X�\���ݒ�
        chkbxForceDlv.Text = "�v���O�����{��۸��ѓK�pؽ�" & vbCrLf & "�����z�M"
        Me.tabOdrDelivery.Focus()
        LfWaitCursor(False)

    End Sub
#End Region

#Region "�v���O�����z�M�w���ݒ��ʂ̃f�[�^����������"
    ''' <summary>
    ''' �v���O�����z�M�w���ݒ��ʂ̃f�[�^����������
    ''' </summary>
    ''' <remarks>
    '''�v���O�����z�M�w���ݒ�f�[�^���������A��ʂɕ\������
    ''' </remarks>   
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean

        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e

        Try
            Log.Info("Method started.")

            shtPrgDelivery.TransformEditor = False                                    '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtTdlApplied.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtTdlApplied.ViewMode = ElTabelleSheet.ViewMode.Row                      '�s�I�����[�h
            shtTdlApplied.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   '�V�[�g��\�����[�h


            '��ʃ^�C�g��
            lblTitle.Text = LcstFormTitle

            '�R���g���[���̏������i���ʐݒ�j
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

            '�e�R���{�{�b�N�X�̍��ړo�^

            '�@�햼�̃R���{�{�b�N�X��ݒ肷��B
            If setCmbModel() = False Then Exit Try
            cmbModel.SelectedIndex = 0            '�f�t�H���g�\������

            '�K�p�G���A���̃R���{�{�b�N�X��ݒ肷��B
            If setCmbAreaName(cmbModel.SelectedValue.ToString) = False Then Exit Try
            cmbAreaName.SelectedIndex = 0            '�f�t�H���g�\������

            'ELTable �̏�����
            Call initElTable(Me.shtPrgDelivery)
            Call initElTable(Me.shtTdlApplied)
            Call ClrTdlList()

            Me.btnPrint.Enabled = False
            Me.btnDelivery.Enabled = False

            bRtn = True

        Catch ex As Exception
            '��ʕ\�������Ɏ��s���܂���
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function
#End Region

#Region "�v���O�����K�p���X�gVer�̕ύX���C�x���g(�ꗗ��)"
    ''' <summary>
    ''' �K�p���X�gVer�w�莞�̃C�x���g����
    ''' </summary>
    ''' <remarks>�o�[�W�����ύX���A�K�p���X�g���e��K�p���X�g�̓��e�ꗗ�֓W�J�B</remarks>
    Private Sub shtOdrDelivery_CellNotify(ByVal sender As Object, _
        ByVal e As GrapeCity.Win.ElTabelleSheet.CellNotifyEventArgs) Handles shtPrgDelivery.CellNotify

        If e.Name <> ElTabelleSheet.CellNotifyEvents.SelectedIndexChanged Then Exit Sub

        Dim cmbEdt As ElTabelleSheet.Editors.SuperiorComboEditor
        Dim nIndex As Integer
        Dim lstCmbItems As New ArrayList()

        LfWaitCursor(True)
        Me.btnDelivery.Enabled = False

        '��ʂ̑M����h������
        Me.shtPrgDelivery.Redraw = False

        Try
            '�I�����ꂽ�R���{�{�b�N�X�̃C���f�b�N�X���擾
            cmbEdt = CType(Me.shtPrgDelivery.Item(1, e.Position.Row).Editor, ElTabelleSheet.Editors.SuperiorComboEditor)
            For i As Integer = 1 To cmbEdt.Items.Count - 1
                If cmbEdt.Items(i).Selected = True Then
                    nIndex = i
                End If
                lstCmbItems.Add(New DictionaryEntry(cmbEdt.Items(i).Content.ToString, cmbEdt.Items(i).Value.ToString))
            Next

            '�o�[�W������I�����ꂽ��
            If nIndex <> 0 Then
                '�I�����ꂽ�o�[�W�����̃v���O�����K�p�t�@�C�������擾
                LstListFile_Name = cmbEdt.Items(nIndex).Value.ToString

                '�v���O�����K�p�m�F�^�u�̃R���{�{�b�N�X�𐶐�
                If nIndex <> 1 Then
                    LbEventStop = True      '�C�x���g�����n�e�e
                End If
                cmbTdlVersion.DisplayMember = "Key"
                cmbTdlVersion.ValueMember = "Value"
                cmbTdlVersion.DataSource = lstCmbItems
                LbEventStop = False      '�C�x���g�����n�e�e
                cmbTdlVersion.SelectedIndex = nIndex - 1

                '�I�����ꂽ�s�ȊO�̃R���{�{�b�N�X���N���A
                For i As Integer = 0 To Me.shtPrgDelivery.MaxRows - 1
                    If i <> e.Position.Row Then
                        Me.shtPrgDelivery.Item(1, i).Text = ""
                    End If
                Next
                '-------Ver0.1�@���X�g�ُ펞�z�M�{�^���\���Ή��@ADD START-----------
                Me.btnDelivery.Enabled = True
                '-------Ver0.1�@���X�g�ُ펞�z�M�{�^���\���Ή��@ADD END-----------
            Else
                '�󔒂�I�����ꂽ��
                LstListFile_Name = ""
                LbEventStop = True      '�C�x���g�����n�e�e
                cmbTdlVersion.DataSource = Nothing
                cmbTdlVersion.Items.Clear()

                Call initElTable(Me.shtTdlApplied)
                '-------Ver0.1�@���X�g�ُ펞�z�M�{�^���\���Ή��@ADD START-----------
                Me.btnDelivery.Enabled = False
                '-------Ver0.1�@���X�g�ُ펞�z�M�{�^���\���Ή��@ADD END-----------
                '���x��������
                Call ClrTdlList()
                LbEventStop = False      '�C�x���g�����n�e�e

            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, cmbModel.Text)

        Finally
            lstCmbItems = Nothing
            '��ʂ̑M����h������
            Me.shtPrgDelivery.Redraw = True
            LfWaitCursor(False)

        End Try

    End Sub
#End Region

#Region "�u�o�́v�{�^���N���b�N"

    ''' <summary>
    ''' �u�o�́v�{�^���N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            LogOperation(sender, e)    '�{�^���������O

            Dim sDirPath As String = Config.LedgerTemplateDirPath
            Dim sFilePath As String = ""

            '�e���v���[�g�i�[�t�H���_�`�F�b�N
            If Directory.Exists(sDirPath) = False Then
                Log.Error("It's not found [" & sDirPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If

            '�e���v���[�g�t���p�X�`�F�b�N
            sFilePath = Path.Combine(sDirPath, LcstXlsTemplateName)
            If File.Exists(sFilePath) = False Then
                Log.Error("It's not found [" & sFilePath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If
            '�o��
            LfXlsStart(sFilePath)
            cmbModel.Select()

            '�e���v���[�g�t���p�X�`�F�b�N
            sFilePath = Path.Combine(sDirPath, LcstXlsTemplateName2)
            If File.Exists(sFilePath) = False Then
                Log.Error("It's not found [" & sFilePath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If
            '�o��
            '�K�p���X�g��Ver���I������Ă���ꍇ�̂݁A�o�͂��s���B
            If cmbTdlVersion.Text <> String.Empty Then
                LfXlsStart2(sFilePath)
                cmbTdlVersion.Select()
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            '�G���[���b�Z�[�W
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub
#End Region

#Region "�u�z�M�v�{�^���N���b�N"
    ''' <summary>
    ''' �u�z�M�v�{�^���N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�z�M��Ƃ��đS�w��I�������ꍇ�Ɣz�M�Ώۃ��X�g�Ƀf�[�^�����݂��Ă���ꍇ�ɁA����������B
    ''' �u�z�M�v�{�^�����N���b�N���邱�Ƃɂ��A�u�z�M�Ώۃ��X�g�v�ɂĎw�肳�ꂽ�z�M�Ώۂ̋@��ɑ΂��Ĕz�M�w�����s���B
    ''' �i�z�M��Ƃ��đS�w���I������Ă���ꍇ�́A�S�w�̋@��ɑ΂��Ĕz�M�w�����s���B�j
    ''' �z�M�w�����I�������ꍇ�́A�w�������̃|�b�v�A�b�v��ʂ�\������B
    ''' </remarks>
    Private Sub btnDoHaishin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelivery.Click
        Try
            LogOperation(sender, e)    '�{�^���������O

            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyInvokeMasProDll) = DialogResult.No Then
                LogOperation(Lexis.NoButtonClicked)     'No�{�^���������O
                Exit Sub
            End If

            LogOperation(Lexis.YesButtonClicked)     'Yes�{�^���������O

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

#Region "�u�I���v�{�^���N���b�N"
    ''' <summary>
    ''' �u�I���v�{�^���N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        LogOperation(sender, e)    '�{�^���������O
        Me.Close()
    End Sub
#End Region

#Region "ELTable�̏�����"
    ''' <summary>
    ''' ELTable�̏�����
    ''' </summary>
    ''' <remarks>Eltable�Ɋ��������f�[�^���N���A����B�f�[�^�ɂ���čēxEltable�̃f�[�^�G���A��ݒ肷��B</remarks>
    Private Sub initElTable(ByVal shtTarget As GrapeCity.Win.ElTabelleSheet.Sheet)

        'Eltable�̃J�����g�̍ő包��
        Dim sXYRange As String = ""

        '��ʂ̑M����h������
        shtTarget.Redraw = False

        If shtTarget.MaxRows > 0 Then
            'Eltable�̃J�����g�̍ő包�����擾����B
            sXYRange = "1:" & shtTarget.MaxRows.ToString

            '�I�����ꂽ�G���A�̃f�[�^���N���A����B
            shtTarget.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
        End If

        shtTarget.MaxRows = 0

        '��ʂ̑M����h������
        shtTarget.Redraw = True

    End Sub
#End Region

#Region "�v���O�����z�M�w���ݒ�i�v���O�����K�p�m�F�j�@���[�o��"
    ''' <summary>
    ''' [�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart2(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 13
        Try
            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = sPath
                .ExcelMode = True
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()
                '���[�t�@�C���V�[�g���̂��擾���܂��B
                .Page.Start(LcstXlsSheetName2, "1-9999")

                ' ���o�����Z���֌��o���f�[�^�o��
                .Cell("B1").Value = LcstXlsSheetName2
                .Cell("P1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("P2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B4").Value = UPDATED_DATE + Me.lblCreateDate.Text.Trim
                .Cell("B5").Value = OPMGFormConstants.PRO_NAME + Me.lblPrgName.Text.Trim
                .Cell("B6").Value = PRG_APPLIEDLIST_VER + Me.lblTdlVer.Text.Trim
                .Cell("B7").Value = AREANAME + Me.lblAreaName.Text.Trim
                .Cell("B8").Value = PROVER + Me.lblPrgVer.Text.Trim
                .Cell("B9").Value = MODELCODE + Me.lblMdlCode.Text.Trim

                ' �z�M�Ώۂ̃f�[�^�����擾���܂�
                nRecCnt = shtTdlApplied.MaxRows

                If nRecCnt = 0 Then
                    .RowClear(nStartRow, 1)
                End If

                ' �f�[�^�����̌r���g���쐬
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                '�f�[�^�����̒l�Z�b�g
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol2.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtTdlApplied.Item(LcstPrntCol2(x), y).Text
                    Next
                Next

                '�o�͏����̏I����錾
                .Page.End()
                .Report.End()

                ' ���[�̃v���r���[�����[�_���_�C�A���O�ŋN�����܂��B
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

#Region "�v���O�����z�M�w���ݒ�i�z�M�f�[�^�j�@���[�o��"
    ''' <summary>
    ''' [�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 13
        Try
            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = sPath
                .ExcelMode = True
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()
                '���[�t�@�C���V�[�g���̂��擾���܂��B
                .Page.Start(LcstXlsSheetName, "1-9999")

                ' ���o�����Z���֌��o���f�[�^�o��
                .Cell("B1").Value = LcstXlsSheetName
                .Cell("G1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("G2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B6").Value = OPMGFormConstants.EQUIPMENT_TYPE_NAME + cmbModel.Text.Trim
                If Me.chkbxForceDlv.Checked = False Then
                    .Cell("F6").Value = FORCED_DELIVERY + "��"
                Else
                    .Cell("F6").Value = FORCED_DELIVERY + "�L"
                End If
                .Cell("B7").Value = APPLIED_AREA + Me.cmbAreaName.Text.Trim

                ' �z�M�Ώۂ̃f�[�^�����擾���܂�
                nRecCnt = shtPrgDelivery.MaxRows

                ' �f�[�^�����̌r���g���쐬
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                '�f�[�^�����̒l�Z�b�g
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtPrgDelivery.Item(LcstPrntCol(x), y).Text
                    Next
                Next

                '�o�͏����̏I����錾
                .Page.End()
                .Report.End()

                ' ���[�̃v���r���[�����[�_���_�C�A���O�ŋN�����܂��B
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

#Region "�R���{�{�b�N�X�̃N���b�N�C�x���g"
    ''' <summary>
    ''' �@��R���{�I���C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbModel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            '�K�p�G���A�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbAreaName(cmbModel.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblApplyArea.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbAreaName.SelectedIndex = 0               '���C�x���g�����ӏ�
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblModel.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' �K�p�G���A�R���{�I���C�x���g
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
        LbEventStop = True      '�C�x���g�����n�m
        cmbTdlVersion.DataSource = Nothing
        cmbTdlVersion.Items.Clear()

        LbEventStop = False      '�C�x���g�����n�m
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
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '�Y���Ȃ�
                    AlertBox.Show(Lexis.NoRecordsFound)
                    cmbModel.Select()
                    Exit Sub
            End Select

            '��ʂ̑M����h���B
            Me.shtPrgDelivery.Redraw = False

            '�f�[�^������ꍇ�A�f�[�^�̍s���ɂ���čēxEltable�̍ő包����ݒ肷��B
            If Me.shtPrgDelivery.MaxRows < nCnt Then
                Me.shtPrgDelivery.MaxRows = nCnt
            End If
            RowCnt = 0
            '���I�Ƀf�[�^��ǉ�����B
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
    ''' �v���O�����K�p���X�g�o�[�W�����R���{�I���C�x���g
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
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '�Y���Ȃ�
                    AlertBox.Show(Lexis.NoRecordsFound)
                    Exit Sub
            End Select

            Call initElTable(Me.shtTdlApplied)
            shtTdlApplied.MaxRows = dtData.Rows.Count         '���o�������̍s���ꗗ�ɍ쐬
            shtTdlApplied.Rows.SetAllRowsHeight(21)       '�s�����𑵂���
            shtTdlApplied.DataSource = dtData                 '�f�[�^���Z�b�g

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
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '�Y���Ȃ�
                    AlertBox.Show(Lexis.NoRecordsFound)
                    Exit Sub
            End Select

            Me.lblCreateDate.Text = dtData2.Rows(nCnt - 1).Item("DATE").ToString
            Me.lblPrgName.Text = Me.shtPrgDelivery.Item(2, 0).Text
            Me.lblTdlVer.Text = EkMasProListFileName.GetListVersion(Me.cmbTdlVersion.SelectedValue.ToString)
            Me.lblAreaName.Text = Me.cmbAreaName.Text
            Me.lblPrgVer.Text = EkMasProListFileName.GetDataVersion(Me.cmbTdlVersion.SelectedValue.ToString)
            Me.lblMdlCode.Text = EkMasProListFileName.GetDataApplicableModel(Me.cmbTdlVersion.SelectedValue.ToString)
            '------Ver0.1�@���X�g�ُ펞�z�M�{�^���\���Ή��@ADD�@START-----------------------
            ''�z�M�f�[�^�Ŏw�肳�ꂽ�K�p���X�g���@��\����A�����Ă���΁u�z�M�v�{�^����������
            'If LstListFile_Name = Me.cmbTdlVersion.SelectedValue.ToString Then
            '    If Me.shtTdlApplied.Item(4, 0).Text = "1" Then
            '        btnDelivery.Enabled = True
            '    End If
            'End If

            '------Ver0.1�@���X�g�ُ펞�z�M�{�^���\���Ή��@ADD�@END---------------------------
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

#Region "�R���{�{�b�N�X��ݒ肷��B"
    ''' <summary>
    ''' �@�햼�̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���@�햼�̂̈ꗗ�y�сu�󔒁v��ݒ肷��B</remarks>
    Private Function setCmbModel() As Boolean

        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New ModelMaster

        Try
            '�@�햼�̃R���{�{�b�N�X�p�̃f�[�^���擾����B
            dt = oMst.SelectTable(True)
            If dt.Rows.Count = 0 Then
                '�@��f�[�^�擾���s
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
    ''' �K�p�G���A���̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <param name="Model">�@��R�[�h</param>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���p�^�[�����̂̈ꗗ�y�сu�󔒁v��ݒ肷��B</remarks>
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
    ''' �v���O�����K�p�m�F���N���A�B
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
