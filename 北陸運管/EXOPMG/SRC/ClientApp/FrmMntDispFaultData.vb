' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '�萔�l�̂ݎg�p
Imports JR.ExOpmg.DataAccess
Imports System.IO
Imports System
Imports System.Text
Imports GrapeCity.Win

''' <summary>
''' �y�ُ�f�[�^�m�F�@��ʃN���X�z
''' </summary>
Public Class FrmMntDispFaultData
    Inherits FrmBase

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B
        LcstSearchCol = {Me.cmbEki, Me.cmbMado, Me.cmbKisyu, Me.cmbGouki, Me.dtpYmdFrom, Me.dtpHmFrom, Me.dtpYmdTo, Me.dtpHmTo}

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
        Me.lblToday.Text = "2013/08/21(��)  12:51"
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
        Me.wkbMain.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(705, 584)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 11
        Me.btnPrint.Text = "�o�@��"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'cmbKisyu
        '
        Me.cmbKisyu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKisyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKisyu.ItemHeight = 13
        Me.cmbKisyu.Items.AddRange(New Object() {"", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w"})
        Me.cmbKisyu.Location = New System.Drawing.Point(38, 6)
        Me.cmbKisyu.MaxLength = 3
        Me.cmbKisyu.Name = "cmbKisyu"
        Me.cmbKisyu.Size = New System.Drawing.Size(126, 21)
        Me.cmbKisyu.TabIndex = 3
        '
        'lblKisyu
        '
        Me.lblKisyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblKisyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKisyu.Location = New System.Drawing.Point(3, 6)
        Me.lblKisyu.Name = "lblKisyu"
        Me.lblKisyu.Size = New System.Drawing.Size(36, 21)
        Me.lblKisyu.TabIndex = 0
        Me.lblKisyu.Text = "�@��"
        Me.lblKisyu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(873, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 12
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(873, 70)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 10
        Me.btnKensaku.Text = "���@��"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'cmbGouki
        '
        Me.cmbGouki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGouki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.lblGouki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblGouki.Location = New System.Drawing.Point(4, 7)
        Me.lblGouki.Name = "lblGouki"
        Me.lblGouki.Size = New System.Drawing.Size(39, 21)
        Me.lblGouki.TabIndex = 0
        Me.lblGouki.Text = "���@"
        Me.lblGouki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbErrcd
        '
        Me.cmbErrcd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple
        Me.cmbErrcd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbErrcd.ItemHeight = 13
        Me.cmbErrcd.Items.AddRange(New Object() {"", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w"})
        Me.cmbErrcd.Location = New System.Drawing.Point(102, 7)
        Me.cmbErrcd.Name = "cmbErrcd"
        Me.cmbErrcd.Size = New System.Drawing.Size(645, 21)
        Me.cmbErrcd.TabIndex = 9
        '
        'lblErrcd
        '
        Me.lblErrcd.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblErrcd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblErrcd.Location = New System.Drawing.Point(4, 6)
        Me.lblErrcd.Name = "lblErrcd"
        Me.lblErrcd.Size = New System.Drawing.Size(92, 22)
        Me.lblErrcd.TabIndex = 1
        Me.lblErrcd.Text = "�G���[�R�[�h"
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
        Me.lblTo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTo.Location = New System.Drawing.Point(586, 6)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(37, 20)
        Me.lblTo.TabIndex = 7
        Me.lblTo.Text = "�܂�"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFromDate
        '
        Me.lblFromDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFromDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFromDate.Location = New System.Drawing.Point(4, 6)
        Me.lblFromDate.Name = "lblFromDate"
        Me.lblFromDate.Size = New System.Drawing.Size(64, 20)
        Me.lblFromDate.TabIndex = 0
        Me.lblFromDate.Text = "�J�n����"
        Me.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFrom
        '
        Me.lblFrom.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFrom.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFrom.Location = New System.Drawing.Point(274, 6)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(37, 20)
        Me.lblFrom.TabIndex = 3
        Me.lblFrom.Text = "����"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblToDate
        '
        Me.lblToDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblToDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToDate.Location = New System.Drawing.Point(320, 6)
        Me.lblToDate.Name = "lblToDate"
        Me.lblToDate.Size = New System.Drawing.Size(65, 20)
        Me.lblToDate.TabIndex = 4
        Me.lblToDate.Text = "�I������"
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
        Me.cmbMado.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMado.ItemHeight = 13
        Me.cmbMado.Items.AddRange(New Object() {"", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w"})
        Me.cmbMado.Location = New System.Drawing.Point(67, 6)
        Me.cmbMado.MaxLength = 10
        Me.cmbMado.Name = "cmbMado"
        Me.cmbMado.Size = New System.Drawing.Size(162, 21)
        Me.cmbMado.TabIndex = 2
        '
        'lblMado
        '
        Me.lblMado.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMado.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMado.Location = New System.Drawing.Point(3, 6)
        Me.lblMado.Name = "lblMado"
        Me.lblMado.Size = New System.Drawing.Size(64, 21)
        Me.lblMado.TabIndex = 0
        Me.lblMado.Text = "�R�[�i�["
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
        Me.cmbEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Items.AddRange(New Object() {"", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w"})
        Me.cmbEki.Location = New System.Drawing.Point(45, 6)
        Me.cmbEki.MaxLength = 10
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(162, 21)
        Me.cmbEki.TabIndex = 1
        '
        'lblEki
        '
        Me.lblEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblEki.Location = New System.Drawing.Point(4, 6)
        Me.lblEki.Name = "lblEki"
        Me.lblEki.Size = New System.Drawing.Size(39, 21)
        Me.lblEki.TabIndex = 0
        Me.lblEki.Text = "�w��"
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
        Me.Text = "�^�p�[�� Ver.1.00"
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

#Region "�錾�̈�iPrivate�j"

    ''' <summary>
    ''' ���������ďo����
    ''' �iTrue:���������ďo�ς݁AFalse:�����������ďo(Form_Load���ŏ����������{)�j
    ''' </summary>
    Private LbInitCallFlg As Boolean = False

    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean
    ''' <summary>
    ''' �o�͗p�e���v���[�g�t�@�C����
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "�ُ�f�[�^.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "�ُ�f�[�^"

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly FormTitle As String = "�ُ�f�[�^�m�F"

    ''' <summary>
    ''' �w�R�[�h�̐擪3��:�u000�v
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"

    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private ReadOnly LcstMaxColCnt As Integer = 12

    ''' <summary>
    ''' �ꗗ�w�b�_�̃\�[�g�񊄂蓖��
    ''' �i�ꗗ�w�b�_�N���b�N���Ɋ��蓖�Ă�Ώۗ���`�B��ԍ��̓[�����΂�"-1"�̓\�[�g�ΏۊO�̗�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, 0, -1, -1, -1, 5, -1, -1, -1, 9, -1, -1}

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������ُ�f�[�^�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13}

    ''' <summary>
    ''' ���������ɂ���āA�����{�^��������
    ''' </summary>
    Private LcstSearchCol() As Control

    '����SQL�擾�敪
    Private Enum SlcSQLType
        SlcCount = 0  '�����擾�p
        SlcDetail = 1 '�f�[�^�����p
    End Enum

    '�K�p�J�n��
    Private sApplyDate As String = Now.ToString("yyyyMMdd")     '�f�t�H���g���V�X�e�����t
    '�K�p�J�n��
    Public Property ApplyDate() As String
        Get
            Return sApplyDate
        End Get
        Set(ByVal Value As String)
            sApplyDate = Value
        End Set
    End Property

#End Region

#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' [��ʏ�������]
    ''' �G���[�������͓����Ń��b�Z�[�W��\�����܂��B
    ''' </summary>
    ''' <returns>True:����,False:���s</returns>
    Public Function InitFrm() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e
        Try
            Log.Info("Method started.")

            '--��ʃ^�C�g��
            lblTitle.Text = FormTitle

            '�V�[�g������
            shtMain.TransformEditor = False '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row
            shtMain.MaxRows() = 0                                               '�s�̏�����
            shtMain.MaxColumns() = LcstMaxColCnt
            '�s�w�b�_�̐ݒ�
            shtMain.RowHeaders.MaxColumns = 1
            shtMain.RowHeaders.GetColumn(0).Width = 34
            '��w�b�_�̐ݒ�
            shtMain.ColumnHeaders.MaxRows = 1
            shtMain.ColumnHeaders.GetRow(0).Height = 42

            '�V�[�g�̕\���I�����[�h��ݒ肷��
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtMain.ColumnHeaders.HeaderClick, AddressOf Me.shtMainColumnHeaders_HeadersClick

            '--�펞���������ڐݒ�
            btnReturn.Enabled = True        '�I���{�^��
            '�l������
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

            '�e�R���{�{�b�N�X�̍��ړo�^()
            If LfSetEki() = False Then Exit Try '�w���R���{�{�b�N�X�ݒ�
            cmbEki.SelectedIndex = 0            '�f�t�H���g�\������
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try '�R�[�i�[�R���{�{�b�N�X�ݒ�
            cmbMado.SelectedIndex = 0           '�f�t�H���g�\������
            If LfSetKisyu(cmbEki.SelectedValue.ToString, cmbMado.SelectedValue.ToString) = False Then Exit Try '�@��R���{�{�b�N�X�ݒ�
            cmbKisyu.SelectedIndex = 0          '�f�t�H���g�\������
            If LfSetGouki(cmbEki.SelectedValue.ToString, _
                          cmbMado.SelectedValue.ToString, _
                          cmbKisyu.SelectedValue.ToString) = False Then Exit Try '���@�R���{�{�b�N�X�ݒ�
            cmbGouki.SelectedIndex = 0          '�f�t�H���g�\������

            '�ꗗ�\�[�g�̏�����
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
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function

#End Region

#Region "�C�x���g"

    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    Private Sub FrmMntDispAbnormalData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrm() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If
            LbEventStop = True      '�C�x���g�����n�e�e
            LfSetDateFromTo()       'Load����Ȃ��ƊJ�n���Ԃ�00:00���ݒ肳��Ȃ��ׁA�����Őݒ肵�Ă��܂��B
            LbEventStop = False     '�C�x���g�����n�m

            '�����{�^��������
            LfSearchTrue()

            cmbEki.Select() '�����t�H�[�J�X
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////�{�^���N���b�N

    ''' <summary>
    ''' �I��
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnReturn.Click
        LogOperation(sender, e)    '�{�^���������O
        Me.Close()
    End Sub

    ''' <summary>
    ''' ����
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
            LogOperation(sender, e)    '�{�^���������O

            '����������
            LfClrList()

            '�^�p�Ǘ��[����INI�t�@�C������擾�\�������擾
            Dim nMaxCount As Integer = Config.MaxUpboundDataToGet

            ErrSts = 0

            '���͕����̃`�F�b�N
            ErrSts = ErrCdCheck(cmbErrcd.Text.ToString)
            If Not ErrSts = 0 Then
                AlertBox.Show(Lexis.TheInputValueIsUnsuitableForFaultDataErrorCode)
                cmbErrcd.Select()
                Exit Sub
            End If

            '���̓f�[�^
            CmbErrCdTxt = cmbErrcd.Text.ToString

            '�������i�G���[�R�[�h�j���쐬����
            ErrCdWhere = "" : ErrCdWhere = ErrCdSelect(CmbErrCdTxt)


            '�����擾�`�F�b�N
            sSql = LfGetSelectString(SlcSQLType.SlcCount, ErrCdWhere)
            nRtn = BaseSqlDataTableFill(sSql, dt)
            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case Else
                    '����`�F�b�N
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

            '�N���A
            sSql = ""
            dt = New DataTable

            '�f�[�^�擾����
            sSql = LfGetSelectString(SlcSQLType.SlcDetail, ErrCdWhere)
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '�Y���Ȃ�
                    AlertBox.Show(Lexis.NoRecordsFound)
                    cmbEki.Select()
                    Exit Sub
                Case Is > nMaxCount     '�������擾�\����
                    AlertBox.Show(Lexis.HugeRecordsFound, nMaxCount.ToString())
                    cmbEki.Select()
                    Exit Sub
            End Select

            '�擾�f�[�^���ꗗ�ɐݒ�
            LfSetSheetData(dt)
            '�ꗗ�A�o�̓{�^��������
            If shtMain.Enabled = False Then shtMain.Enabled = True
            If btnPrint.Enabled = False Then btnPrint.Enabled = True
            shtMain.Select()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)    '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W
            btnReturn.Select()
        Finally
            'DB�J��()
            dt = Nothing
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' �o��
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click

        Dim ErrFileName As String = ""
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            LbEventStop = True
            LogOperation(sender, e)    '�{�^���������O

            Dim sPath As String = Config.LedgerTemplateDirPath

            '�e���v���[�g�i�[�t�H���_�`�F�b�N
            If Directory.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If
            '�e���v���[�g�t���p�X�`�F�b�N
            sPath = Path.Combine(sPath, LcstXlsTemplateName)
            If File.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If

            '�o��
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
    ''' ���X�g�s�I�����i�_�u���N���b�N�j
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
                '�擾�ςݏ��̐ݒ�
                .setContent(shtMain.Item(1, e.Row).Text, shtMain.Item(2, e.Row).Text, shtMain.Item(3, e.Row).Text, _
                           shtMain.Item(4, e.Row).Text, shtMain.Item(5, e.Row).Text, _
                           shtMain.Item(10, e.Row).Text & "(" & shtMain.Item(9, e.Row).Text & ")", _
                           shtMain.Item(12, e.Row).Text, shtMain.Item(13, e.Row).Text)
                '�\��
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
    ''' �u�w�v�R���{
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            LbEventStop = True      '�C�x���g�����n�e�e

            '�R�[�i�[�R���{�ݒ�
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then
                If cmbMado.Enabled = True Then BaseCtlDisabled(pnlMado, False)
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbMado.SelectedIndex = 0               '���C�x���g�����ӏ�
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
    ''' �u�R�[�i�[�v�R���{
    ''' </summary>
    Private Sub cmbMado_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbMado.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            LbEventStop = True      '�C�x���g�����n�e�e

            '�@��R���{�ݒ�
            If LfSetKisyu(cmbEki.SelectedValue.ToString, cmbMado.SelectedValue.ToString) = False Then
                If cmbKisyu.Enabled = True Then BaseCtlDisabled(pnlKisyu, False)
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblKisyu.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbKisyu.SelectedIndex = 0               '���C�x���g�����ӏ�
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
    ''' �u�@��v�R���{
    ''' </summary>
    Private Sub cmbKisyu_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbKisyu.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            LbEventStop = True      '�C�x���g�����n�m

            '���@�R���{�ݒ�
            If LfSetGouki(cmbEki.SelectedValue.ToString, _
                          cmbMado.SelectedValue.ToString, _
                          cmbKisyu.SelectedValue.ToString) = False Then
                If cmbGouki.Enabled = True Then BaseCtlDisabled(pnlGouki, False)
                If dtpYmdFrom.Enabled = True Then BaseCtlDisabled(pnlFromTo, False)
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblGouki.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbGouki.SelectedIndex = 0               '���C�x���g�����ӏ�

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
    ''' �u���@�v�R���{
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
    ''' �J�n�����i�N�����j,�J�n�����i�����j,�I�������i�N�����j,�I�������i�����j
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

    '//////////////////////////////////////////////ElTable�֘A

    Private Sub shtMainColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)
        Static intCurrentSortColumn As Integer = -1
        Static bolColumn1SortOrder(63) As Boolean

        Try
            If LcstSortCol(e.Column) = -1 Then Exit Sub

            shtMain.BeginUpdate()

            If intCurrentSortColumn > -1 Then
                '�O��\�[�g���ꂽ��w�b�_�̃C���[�W���폜����
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '�O��\�[�g���ꂽ��̔w�i�F������������
                shtMain.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '�O��\�[�g���ꂽ��̃Z���r������������
                shtMain.Columns(intCurrentSortColumn).SetBorder(New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), GrapeCity.Win.ElTabelleSheet.Borders.All)
            End If

            '�I�����ꂽ��ԍ���ۑ�
            intCurrentSortColumn = e.Column

            '�\�[�g�����̔w�i�F��ݒ肷��
            shtMain.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
            '�\�[�g�����̃Z���r����ݒ肷��
            shtMain.Columns(intCurrentSortColumn).SetBorder( _
                New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                GrapeCity.Win.ElTabelleSheet.Borders.All)

            If bolColumn1SortOrder(intCurrentSortColumn) = False Then
                '��w�b�_�̃C���[�W��ݒ肷��
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(1)
                '�~���Ń\�[�g����
                Call SheetSort(shtMain, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Descending)
                '��̃\�[�g��Ԃ�ۑ�����
                bolColumn1SortOrder(intCurrentSortColumn) = True
            Else
                '��w�b�_�̃C���[�W��ݒ肷��
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(0)
                '�����Ń\�[�g����
                Call SheetSort(shtMain, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Ascending)
                '��̃\�[�g��Ԃ�ۑ�����
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
        '�}�E�X�J�[�\������w�b�_��ɂ���ꍇ
        If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
            shtMain.CrossCursor = Cursors.Default
        Else
            '�}�E�X�J�[�\��������ɖ߂�
            shtMain.CrossCursor = Nothing
        End If
    End Sub

    ''' <summary>
    ''' �\�[�g
    ''' </summary>
    Private Sub SheetSort(ByRef sheetTarget As GrapeCity.Win.ElTabelleSheet.Sheet, ByVal intKeyColumn As Integer, ByVal sortOrder As GrapeCity.Win.ElTabelleSheet.SortOrder)
        Dim objSortItem As New GrapeCity.Win.ElTabelleSheet.SortItem(intKeyColumn, False, sortOrder)
        Dim objSortList(0) As GrapeCity.Win.ElTabelleSheet.SortItem
        '�z��Ƀ\�[�g�I�u�W�F�N�g��ǉ�����
        objSortList(0) = objSortItem
        '�\�[�g�����s����
        sheetTarget.Sort(objSortList)
    End Sub

#End Region

#Region "���\�b�h�iPrivate�j"
    ''' <summary>
    ''' [�w�R���{�ݒ�]
    ''' </summary>
    ''' <returns>True:�����AFalse:���s</returns>
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
    ''' [�J�n�I�������ݒ�]
    ''' </summary>
    Private Sub LfSetDateFromTo()
        Dim dtWork As DateTime = DateAdd(DateInterval.Day, -1, Today)
        Dim dtFrom As New DateTime(dtWork.Year, dtWork.Month, dtWork.Day, 0, 0, 0)
        Dim dtTo As DateTime = Now
        dtpYmdFrom.Format = DateTimePickerFormat.Custom
        dtpYmdFrom.CustomFormat = "yyyy�NMM��dd��"
        dtpYmdFrom.Value = dtFrom
        dtpHmFrom.Format = DateTimePickerFormat.Custom
        dtpHmFrom.CustomFormat = "HH:mm"
        dtpHmFrom.Value = dtFrom
        dtpYmdTo.Format = DateTimePickerFormat.Custom
        dtpYmdTo.CustomFormat = "yyyy�NMM��dd��"
        dtpYmdTo.Value = dtTo
        dtpHmTo.Format = DateTimePickerFormat.Custom
        dtpHmTo.CustomFormat = "HH:mm"
        dtpHmTo.Value = dtTo
    End Sub

    ''' <summary>
    ''' [�ꗗ�N���A]
    ''' </summary>
    Private Sub LfClrList()
        Dim sXYRange As String
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try
            Dim i As Integer
            '�\�[�g���̃N���A
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
    ''' [�����{�^��������]
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
    ''' [�R�[�i�[�R���{�ݒ�]
    ''' </summary>
    ''' <param name="Station">�w�R�[�h</param>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function LfSetMado(ByVal Station As String) As Boolean
        LbEventStop = True      '�C�x���g�����n�e�e
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
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function
    ''' <summary>
    ''' [�@��R���{�ݒ�]
    ''' </summary>
    ''' <param name="Station">�w�R�[�h</param>
    ''' <param name="Corner">�R�[�i�[�R�[�h</param>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function LfSetKisyu(ByVal Station As String, ByVal Corner As String) As Boolean
        LbEventStop = True      '�C�x���g�����n�e�e
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
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function
    ''' <summary>
    ''' [���@�R���{�ݒ�]
    ''' </summary>
    ''' <param name="Station">�w�R�[�h</param>
    ''' <param name="Corner">�R�[�i�[�R�[�h</param>
    ''' <param name="Kisyu">�@��R�[�h</param>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function LfSetGouki(ByVal Station As String, ByVal Corner As String, ByVal Kisyu As String) As Boolean
        LbEventStop = True      '�C�x���g�����n�e�e
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
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [�����pSELECT������擾]
    ''' </summary>
    ''' <returns>SELECT��</returns>
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
                    '�����擾����--------------------------
                    sBuilder.AppendLine("SELECT COUNT(1) FROM V_FAULT_DATA")

                Case slcSQLType.SlcDetail
                    '�擾����--------------------------
                    sBuilder.AppendLine("SELECT STATION_CODE, STATION_NAME, CORNER_NAME, MODEL_NAME, UNIT_NO,")
                    sBuilder.AppendLine(" SUBSTRING(OCCUR_DATE,1,4)+'/'+SUBSTRING(OCCUR_DATE,5,2)+'/'+SUBSTRING(OCCUR_DATE,7,2)+' '+")
                    sBuilder.AppendLine(" SUBSTRING(OCCUR_DATE,9,2)+':'+SUBSTRING(OCCUR_DATE,11,2)+':'+SUBSTRING(OCCUR_DATE,13,2)")
                    sBuilder.AppendLine(" AS YMDHMS, PASSAGE_NAME, ERROR_TYPE, ACT_STEP, ERR_CODE, ERR_ITEM, ERROR_KIND, DTL_INFO, RES_INFO FROM V_FAULT_DATA")
            End Select

            'Where�吶��--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine(" where 0=0")

            '�w
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
            '�R�[�i�[
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (CORNER_CODE = {0})", _
                                          Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If
            '�@��
            If Not (cmbKisyu.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (MODEL_CODE = {0})", _
                                          Utility.SetSglQuot(cmbKisyu.SelectedValue.ToString)))
            End If
            '���@
            If Not (cmbGouki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL OrElse cmbGouki.SelectedValue.ToString = "") Then
                sSqlWhere.AppendLine(String.Format(" and (UNIT_NO = {0})", _
                                          Utility.SetSglQuot(cmbGouki.SelectedValue.ToString)))
            End If
            '�G���[�R�[�h
            If Not cmbErrcd.Text.ToString = "" Then
                '�G���[�R�[�h�̏�������ǉ��Ō���
                sSqlWhere.AppendLine("and " + ErrCdWhere)
            End If

            '�J�n�I������
            sFrom = Replace(Replace(Replace(dtpYmdFrom.Text, "�N", ""), "��", ""), "��", "") + _
                    Replace(dtpHmFrom.Text, ":", "") + "00"
            sTo = Replace(Replace(Replace(dtpYmdTo.Text, "�N", ""), "��", ""), "��", "") + _
                  Replace(dtpHmTo.Text, ":", "") + "59"

            sSqlWhere.AppendLine(String.Format("and (OCCUR_DATE >= {0}) And (OCCUR_DATE <= {1})", _
                                      Utility.SetSglQuot(sFrom), _
                                      Utility.SetSglQuot(sTo)))


            If slcSQLType.Equals(slcSQLType.SlcDetail) Then
                'Order by�吶��
                sSqlWhere.AppendLine(" order by STATION_CODE, CORNER_CODE asc ")
            End If

            'Where�匋��
            sSql = sBuilder.ToString + sSqlWhere.ToString

            Return sSql
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Function

    ''' <summary>
    ''' [�ꗗ�ݒ�]
    ''' </summary>
    ''' <param name="dt">�ݒ�Ώۃf�[�^�e�[�u��</param>
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

            '�֌W�Ȃ�����B��
            shtMain.DataSource = dt
            If LcstMaxColCnt < dt.Columns.Count Then
                For i = LcstMaxColCnt To dt.Columns.Count - 1
                    shtMain.Columns(i).Hidden = True                                '���̍s���R�����g�A�E�g�����Select���ʑS�Ă̍s�������܂�
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
    ''' �G���[�R�[�h�̏����������B
    ''' </summary>
    ''' <param name="InputErrCd">�Ώە�����</param>
    ''' <returns>�G���[�R�[�h������</returns>
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

        '�u,�v�𔲂�����������擾
        sInputErrCdDel = InputErrCd.Replace(",", "")

        '���ꂼ��̕��������擾
        nCntBf = InputErrCd.Length
        nCntAf = sInputErrCdDel.Length

        '�R���}�̐����擾
        nCnt = nCntBf - nCntAf

        '������𕪊�
        strErr = InputErrCd.Split(CChar(","))

        '�������쐬�iwhere���쐬�j
        sSqlErrWhere = ""
        '�u "(" �v��t���鏈��
        sSqlErrWhere = sSqlErrWhere + "("
        For i = 0 To nCnt

            '�u-�v�𔲂�����������擾
            sInputErrCdDel = strErr(i).Replace("-", "")

            '���ꂼ��̕��������擾
            nCntBf = strErr(i).Length
            nCntAf = sInputErrCdDel.Length

            '-�̐����擾
            nCnt1 = nCntBf - nCntAf

            '������𕪊�
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
                '�u ")" �v��t���鏈��
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
    ''' ���͂��ꂽ�����̃`�F�b�N���s���B
    ''' </summary>
    ''' <param name="InputErrCd"></param>
    ''' <returns>���ۃX�e�[�^�X</returns>
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

        '���������ݒ�
        nStrLen = InputErrCd.Length
        nStrPnt = 1
        ErrCdCheck = 0

        Try
            '���̓`�F�b�N����
            Do
                '������̌��m�I��
                If nStrPnt > nStrLen Then
                    Exit Do
                End If

                '1�������擾
                strMoji = ""
                strMoji = Mid(InputErrCd, nStrPnt, 1)

                '�S�p������ꍇ�Ɉُ픭��
                If sjisEnc.GetByteCount(strMoji) = 1 Then
                    '�p�����A���邢�́u,�v�A�u-�v�ȊO�̕���������ꍇ�Ɉُ픭��
                    If (Not Char.IsLetterOrDigit(strMoji, 0) = True) AndAlso _
                        (Not strMoji = ",") AndAlso _
                        (Not strMoji = "-") Then
                        ErrCdCheck = 2          '�w��ȊO�̕������ُ͈�
                        Exit Function
                    End If
                Else
                    ErrCdCheck = 1              '�S�p�������ُ͂̈픭��
                    Exit Function
                End If

                '�J�E���g�A�b�v
                nStrPnt = nStrPnt + 1
            Loop

            If nStrLen > 0 Then
                '�u,�v�𔲂�����������擾
                sInputErrCdDel = InputErrCd.Replace(",", "")

                '���ꂼ��̕��������擾
                nCntBf = InputErrCd.Length
                nCntAf = sInputErrCdDel.Length

                '�R���}�̐����擾
                nCnt = nCntBf - nCntAf

                '������𕪊�
                strErr = InputErrCd.Split(CChar(","))
                For i = 0 To nCnt
                    If strErr(i) = "" Then
                        ErrCdCheck = 1
                        Exit For
                    Else
                        '�u-�v�𔲂�����������擾
                        sInputErrCdDel = strErr(i).Replace("-", "")

                        '���ꂼ��̕��������擾
                        nCntBf = strErr(i).Length
                        nCntAf = sInputErrCdDel.Length

                        '-�̐����擾
                        nCnt1 = nCntBf - nCntAf

                        '������𕪊�
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
    ''' [�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 6
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
                .Cell("V1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("V2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim + "�@�@�@" + _
                                    OPMGFormConstants.CORNER_STR + cmbMado.Text.Trim + "�@�@" + _
                                    OPMGFormConstants.EQUIPMENT_TYPE + cmbKisyu.Text.Trim + "�@�@" + _
                                    OPMGFormConstants.NUM_EQUIPMENT + cmbGouki.Text.Trim
                .Cell("C4").Value = Lexis.TimeSpan.Gen(
                                                  Replace(Replace(Replace(dtpYmdFrom.Text, "�N", "/"), "��", "/"), "��", ""), _
                                                  dtpHmFrom.Text, _
                                                  Replace(Replace(Replace(dtpYmdTo.Text, "�N", "/"), "��", "/"), "��", ""), _
                                                  dtpHmTo.Text)

                ' �z�M�Ώۂ̃f�[�^�����擾���܂�
                nRecCnt = shtMain.MaxRows

                ' �f�[�^�����̌r���g���쐬
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                '�f�[�^�����̒l�Z�b�g
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        If x = LcstPrntCol.Length - 1 Then
                            .Pos(x + 4, y + nStartRow).Value = shtMain.Item(LcstPrntCol(x), y).Text
                        Else
                            .Pos(x + 1, y + nStartRow).Value = shtMain.Item(LcstPrntCol(x), y).Text
                        End If
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

End Class