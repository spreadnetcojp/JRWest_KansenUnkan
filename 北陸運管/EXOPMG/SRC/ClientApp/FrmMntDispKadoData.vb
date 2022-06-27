' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2014/06/01       ����  �k���Ή�
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '�萔�l�̂ݎg�p
Imports JR.ExOpmg.DataAccess
Imports System.IO
Imports System.Text

Public Class FrmMntDispKadoData
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
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents cmbKisyu As System.Windows.Forms.ComboBox
    Friend WithEvents cmbMado As System.Windows.Forms.ComboBox
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblKisyu As System.Windows.Forms.Label
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents grpFrom As System.Windows.Forms.GroupBox
    Friend WithEvents dtpYmdTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents grp01 As System.Windows.Forms.GroupBox
    Friend WithEvents optKado As System.Windows.Forms.RadioButton
    Friend WithEvents optMente As System.Windows.Forms.RadioButton
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents cmbKensyuFrom As System.Windows.Forms.ComboBox
    Friend WithEvents dtpYmdFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents grpTo As System.Windows.Forms.GroupBox
    Friend WithEvents cmbKensyuTo As System.Windows.Forms.ComboBox
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents gtp05 As System.Windows.Forms.GroupBox
    Friend WithEvents chkLastData As System.Windows.Forms.CheckBox
    Friend WithEvents optFromKensyu As System.Windows.Forms.RadioButton
    Friend WithEvents optFromDate As System.Windows.Forms.RadioButton
    Friend WithEvents optToKensyu As System.Windows.Forms.RadioButton
    Friend WithEvents optToDate As System.Windows.Forms.RadioButton
    Friend WithEvents chkFromLastClear As System.Windows.Forms.CheckBox
    Friend WithEvents chkToYesterday As System.Windows.Forms.CheckBox
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.cmbKisyu = New System.Windows.Forms.ComboBox()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblKisyu = New System.Windows.Forms.Label()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.grpFrom = New System.Windows.Forms.GroupBox()
        Me.chkFromLastClear = New System.Windows.Forms.CheckBox()
        Me.optFromKensyu = New System.Windows.Forms.RadioButton()
        Me.optFromDate = New System.Windows.Forms.RadioButton()
        Me.cmbKensyuFrom = New System.Windows.Forms.ComboBox()
        Me.dtpYmdFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpYmdTo = New System.Windows.Forms.DateTimePicker()
        Me.grp01 = New System.Windows.Forms.GroupBox()
        Me.optKado = New System.Windows.Forms.RadioButton()
        Me.optMente = New System.Windows.Forms.RadioButton()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.grpTo = New System.Windows.Forms.GroupBox()
        Me.chkToYesterday = New System.Windows.Forms.CheckBox()
        Me.optToKensyu = New System.Windows.Forms.RadioButton()
        Me.optToDate = New System.Windows.Forms.RadioButton()
        Me.cmbKensyuTo = New System.Windows.Forms.ComboBox()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.gtp05 = New System.Windows.Forms.GroupBox()
        Me.chkLastData = New System.Windows.Forms.CheckBox()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.grpFrom.SuspendLayout()
        Me.grp01.SuspendLayout()
        Me.grpTo.SuspendLayout()
        Me.gtp05.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.gtp05)
        Me.pnlBodyBase.Controls.Add(Me.lblTo)
        Me.pnlBodyBase.Controls.Add(Me.grpTo)
        Me.pnlBodyBase.Controls.Add(Me.lblFrom)
        Me.pnlBodyBase.Controls.Add(Me.cmbKisyu)
        Me.pnlBodyBase.Controls.Add(Me.lblKisyu)
        Me.pnlBodyBase.Controls.Add(Me.grpFrom)
        Me.pnlBodyBase.Controls.Add(Me.cmbMado)
        Me.pnlBodyBase.Controls.Add(Me.cmbEki)
        Me.pnlBodyBase.Controls.Add(Me.lblMado)
        Me.pnlBodyBase.Controls.Add(Me.grp01)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.lblEki)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/08/29(��)  17:41"
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(856, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 22
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(856, 520)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 21
        Me.btnPrint.Text = "�o�@��"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'cmbKisyu
        '
        Me.cmbKisyu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKisyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKisyu.Items.AddRange(New Object() {"", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w", "�w�w�w�w�w"})
        Me.cmbKisyu.Location = New System.Drawing.Point(646, 36)
        Me.cmbKisyu.Name = "cmbKisyu"
        Me.cmbKisyu.Size = New System.Drawing.Size(126, 21)
        Me.cmbKisyu.TabIndex = 3
        '
        'cmbMado
        '
        Me.cmbMado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMado.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMado.Items.AddRange(New Object() {"", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w"})
        Me.cmbMado.Location = New System.Drawing.Point(340, 36)
        Me.cmbMado.Name = "cmbMado"
        Me.cmbMado.Size = New System.Drawing.Size(214, 21)
        Me.cmbMado.TabIndex = 2
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.Items.AddRange(New Object() {"", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w"})
        Me.cmbEki.Location = New System.Drawing.Point(104, 36)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(126, 21)
        Me.cmbEki.TabIndex = 1
        '
        'lblKisyu
        '
        Me.lblKisyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblKisyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKisyu.Location = New System.Drawing.Point(601, 36)
        Me.lblKisyu.Name = "lblKisyu"
        Me.lblKisyu.Size = New System.Drawing.Size(39, 18)
        Me.lblKisyu.TabIndex = 6
        Me.lblKisyu.Text = "�@��"
        Me.lblKisyu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMado
        '
        Me.lblMado.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMado.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMado.Location = New System.Drawing.Point(270, 36)
        Me.lblMado.Name = "lblMado"
        Me.lblMado.Size = New System.Drawing.Size(64, 18)
        Me.lblMado.TabIndex = 4
        Me.lblMado.Text = "�R�[�i�["
        Me.lblMado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblEki
        '
        Me.lblEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblEki.Location = New System.Drawing.Point(59, 36)
        Me.lblEki.Name = "lblEki"
        Me.lblEki.Size = New System.Drawing.Size(39, 18)
        Me.lblEki.TabIndex = 2
        Me.lblEki.Text = "�w��"
        Me.lblEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'grpFrom
        '
        Me.grpFrom.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grpFrom.Controls.Add(Me.chkFromLastClear)
        Me.grpFrom.Controls.Add(Me.optFromKensyu)
        Me.grpFrom.Controls.Add(Me.optFromDate)
        Me.grpFrom.Controls.Add(Me.cmbKensyuFrom)
        Me.grpFrom.Controls.Add(Me.dtpYmdFrom)
        Me.grpFrom.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpFrom.Location = New System.Drawing.Point(43, 206)
        Me.grpFrom.Name = "grpFrom"
        Me.grpFrom.Size = New System.Drawing.Size(291, 115)
        Me.grpFrom.TabIndex = 7
        Me.grpFrom.TabStop = False
        Me.grpFrom.Text = "�J�n����"
        '
        'chkFromLastClear
        '
        Me.chkFromLastClear.AutoSize = True
        Me.chkFromLastClear.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chkFromLastClear.Location = New System.Drawing.Point(42, 83)
        Me.chkFromLastClear.Name = "chkFromLastClear"
        Me.chkFromLastClear.Size = New System.Drawing.Size(208, 17)
        Me.chkFromLastClear.TabIndex = 12
        Me.chkFromLastClear.Text = "�O��N���A���i�ێ�f�[�^�j"
        Me.chkFromLastClear.UseVisualStyleBackColor = True
        '
        'optFromKensyu
        '
        Me.optFromKensyu.AutoSize = True
        Me.optFromKensyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.optFromKensyu.Location = New System.Drawing.Point(42, 53)
        Me.optFromKensyu.Name = "optFromKensyu"
        Me.optFromKensyu.Size = New System.Drawing.Size(53, 17)
        Me.optFromKensyu.TabIndex = 10
        Me.optFromKensyu.TabStop = True
        Me.optFromKensyu.Text = "���C"
        Me.optFromKensyu.UseVisualStyleBackColor = True
        '
        'optFromDate
        '
        Me.optFromDate.AutoSize = True
        Me.optFromDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.optFromDate.Location = New System.Drawing.Point(42, 24)
        Me.optFromDate.Name = "optFromDate"
        Me.optFromDate.Size = New System.Drawing.Size(53, 17)
        Me.optFromDate.TabIndex = 8
        Me.optFromDate.TabStop = True
        Me.optFromDate.Text = "���t"
        Me.optFromDate.UseVisualStyleBackColor = True
        '
        'cmbKensyuFrom
        '
        Me.cmbKensyuFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKensyuFrom.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKensyuFrom.Items.AddRange(New Object() {"", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX"})
        Me.cmbKensyuFrom.Location = New System.Drawing.Point(100, 52)
        Me.cmbKensyuFrom.Name = "cmbKensyuFrom"
        Me.cmbKensyuFrom.Size = New System.Drawing.Size(136, 21)
        Me.cmbKensyuFrom.TabIndex = 11
        '
        'dtpYmdFrom
        '
        Me.dtpYmdFrom.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.dtpYmdFrom.Location = New System.Drawing.Point(100, 22)
        Me.dtpYmdFrom.Name = "dtpYmdFrom"
        Me.dtpYmdFrom.Size = New System.Drawing.Size(136, 20)
        Me.dtpYmdFrom.TabIndex = 9
        '
        'dtpYmdTo
        '
        Me.dtpYmdTo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.dtpYmdTo.Location = New System.Drawing.Point(101, 21)
        Me.dtpYmdTo.Name = "dtpYmdTo"
        Me.dtpYmdTo.Size = New System.Drawing.Size(136, 20)
        Me.dtpYmdTo.TabIndex = 15
        '
        'grp01
        '
        Me.grp01.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grp01.Controls.Add(Me.optKado)
        Me.grp01.Controls.Add(Me.optMente)
        Me.grp01.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grp01.Location = New System.Drawing.Point(43, 94)
        Me.grp01.Name = "grp01"
        Me.grp01.Size = New System.Drawing.Size(350, 72)
        Me.grp01.TabIndex = 4
        Me.grp01.TabStop = False
        Me.grp01.Text = "�o�̓f�[�^"
        '
        'optKado
        '
        Me.optKado.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.optKado.Location = New System.Drawing.Point(42, 33)
        Me.optKado.Name = "optKado"
        Me.optKado.Size = New System.Drawing.Size(104, 18)
        Me.optKado.TabIndex = 5
        Me.optKado.TabStop = True
        Me.optKado.Text = "�ғ��f�[�^"
        '
        'optMente
        '
        Me.optMente.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.optMente.Location = New System.Drawing.Point(196, 33)
        Me.optMente.Name = "optMente"
        Me.optMente.Size = New System.Drawing.Size(104, 18)
        Me.optMente.TabIndex = 6
        Me.optMente.TabStop = True
        Me.optMente.Text = "�ێ�f�[�^"
        '
        'lblFrom
        '
        Me.lblFrom.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFrom.Location = New System.Drawing.Point(340, 255)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(38, 18)
        Me.lblFrom.TabIndex = 1
        Me.lblFrom.Text = "����"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpTo
        '
        Me.grpTo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grpTo.Controls.Add(Me.chkToYesterday)
        Me.grpTo.Controls.Add(Me.optToKensyu)
        Me.grpTo.Controls.Add(Me.optToDate)
        Me.grpTo.Controls.Add(Me.cmbKensyuTo)
        Me.grpTo.Controls.Add(Me.dtpYmdTo)
        Me.grpTo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpTo.Location = New System.Drawing.Point(471, 206)
        Me.grpTo.Name = "grpTo"
        Me.grpTo.Size = New System.Drawing.Size(291, 115)
        Me.grpTo.TabIndex = 13
        Me.grpTo.TabStop = False
        Me.grpTo.Text = "�I������"
        '
        'chkToYesterday
        '
        Me.chkToYesterday.AutoSize = True
        Me.chkToYesterday.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chkToYesterday.Location = New System.Drawing.Point(42, 83)
        Me.chkToYesterday.Name = "chkToYesterday"
        Me.chkToYesterday.Size = New System.Drawing.Size(54, 17)
        Me.chkToYesterday.TabIndex = 18
        Me.chkToYesterday.Text = "�O��"
        Me.chkToYesterday.UseVisualStyleBackColor = True
        '
        'optToKensyu
        '
        Me.optToKensyu.AutoSize = True
        Me.optToKensyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.optToKensyu.Location = New System.Drawing.Point(42, 53)
        Me.optToKensyu.Name = "optToKensyu"
        Me.optToKensyu.Size = New System.Drawing.Size(53, 17)
        Me.optToKensyu.TabIndex = 16
        Me.optToKensyu.TabStop = True
        Me.optToKensyu.Text = "���C"
        Me.optToKensyu.UseVisualStyleBackColor = True
        '
        'optToDate
        '
        Me.optToDate.AutoSize = True
        Me.optToDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.optToDate.Location = New System.Drawing.Point(42, 24)
        Me.optToDate.Name = "optToDate"
        Me.optToDate.Size = New System.Drawing.Size(53, 17)
        Me.optToDate.TabIndex = 14
        Me.optToDate.TabStop = True
        Me.optToDate.Text = "���t"
        Me.optToDate.UseVisualStyleBackColor = True
        '
        'cmbKensyuTo
        '
        Me.cmbKensyuTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKensyuTo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKensyuTo.Items.AddRange(New Object() {"", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX"})
        Me.cmbKensyuTo.Location = New System.Drawing.Point(101, 52)
        Me.cmbKensyuTo.Name = "cmbKensyuTo"
        Me.cmbKensyuTo.Size = New System.Drawing.Size(136, 21)
        Me.cmbKensyuTo.TabIndex = 17
        '
        'lblTo
        '
        Me.lblTo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTo.Location = New System.Drawing.Point(768, 258)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(38, 18)
        Me.lblTo.TabIndex = 9
        Me.lblTo.Text = "�܂�"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'gtp05
        '
        Me.gtp05.BackColor = System.Drawing.SystemColors.ControlLight
        Me.gtp05.Controls.Add(Me.chkLastData)
        Me.gtp05.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.gtp05.Location = New System.Drawing.Point(43, 361)
        Me.gtp05.Name = "gtp05"
        Me.gtp05.Size = New System.Drawing.Size(291, 69)
        Me.gtp05.TabIndex = 19
        Me.gtp05.TabStop = False
        Me.gtp05.Text = "�o�͏���"
        '
        'chkLastData
        '
        Me.chkLastData.AutoSize = True
        Me.chkLastData.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chkLastData.Location = New System.Drawing.Point(42, 32)
        Me.chkLastData.Name = "chkLastData"
        Me.chkLastData.Size = New System.Drawing.Size(194, 17)
        Me.chkLastData.TabIndex = 20
        Me.chkLastData.Text = "�ŐV�f�[�^�i�ғ��f�[�^�j"
        Me.chkLastData.UseVisualStyleBackColor = True
        '
        'FrmMntDispKadoData
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispKadoData"
        Me.Text = "�^�p�[�� Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.grpFrom.ResumeLayout(False)
        Me.grpFrom.PerformLayout()
        Me.grp01.ResumeLayout(False)
        Me.grpTo.ResumeLayout(False)
        Me.grpTo.PerformLayout()
        Me.gtp05.ResumeLayout(False)
        Me.gtp05.PerformLayout()
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

    '-------Ver0.1�@�k���Ή��@ADD START---------
    '�O���[�v�ԍ�
    Private GrpNo As Integer = 0

    '-------Ver0.1�@�k���Ή��@ADD END-----------
    ''' <summary>
    ''' �o�͗p�e���v���[�g�t�@�C����
    ''' </summary>
    Private ReadOnly LcstXlsTemplateNameKadoY As String = "�ғ��f�[�^�i���������@�j.xls"
    Private LsXlsTemplatePath As String = ""

    ''' <summary>
    ''' ���[�o�͋@��
    ''' �i0�F���ݒ�A1�F���D�@�A2�F���������@�j
    ''' </summary>
    Private LiOutPutSTS As Integer

    Dim LsFromKensyu As String = ""
    Dim LsToKensyu As String = ""
    Dim LsFromDate As String = ""
    Dim LsToDate As String = ""
    Dim LbLastClear As Boolean = False


    Private LsKaiSQL As String = ""
    Private LsMadoSQL As String = ""

    Private LsBaseSQLWhere As String = ""
    Private LsMacSQLWhere As String = ""

    Private LdtTarget As DataTable

    Private LdtKoumoku As DataTable

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
            lblTitle.Text = "�ғ��E�ێ�f�[�^�o��"
            '--�e�p�l�����̑S���ڂ�������
            BaseCtlEnabled(pnlBodyBase)     '�S����

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

            '�e�R���{�{�b�N�X�̍��ړo�^
            If LfSetEki() = False Then Exit Try '�w���R���{�{�b�N�X�ݒ�
            cmbEki.SelectedIndex = 0            '�f�t�H���g�\������
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            Dim station As String = cmbEki.SelectedValue.ToString
            If LfSetMado(station.Substring(station.Length - 6, 6)) = False Then Exit Try '�R�[�i�[�R���{�{�b�N�X�ݒ�
            cmbMado.SelectedIndex = 0           '�f�t�H���g�\������
            If LfSetKisyu(station.Substring(station.Length - 6, 6), cmbMado.SelectedValue.ToString) = False Then Exit Try '�@��R���{�{�b�N�X�ݒ�
            '-------Ver0.1�@�k���Ή��@MOD END-----------
            cmbKisyu.SelectedIndex = 0          '�f�t�H���g�\������
            LfSetKensyuFrom()
            cmbKensyuFrom.SelectedIndex = 0          '�f�t�H���g�\������
            LfSetKensyuTo()
            cmbKensyuTo.SelectedIndex = 0          '�f�t�H���g�\������


            '������Ԑݒ�
            optKado.Checked = True '���C�x���g�����ӏ�
            optFromDate.Checked = True
            optToDate.Checked = True
            chkFromLastClear.Enabled = False
            chkFromLastClear.Checked = False

            bRtn = True

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
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
    Private Sub FrmMntDispKadoData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles Me.Load
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
            optKado.Select()        '�����t�H�[�J�X
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
        LogOperation(sender, e)
        Me.Close()
    End Sub

    ''' <summary>
    ''' �o��
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click
        Dim sSQL As String = ""
        Dim Cnt As Integer
        Dim dtData As New DataTable
        If LbEventStop Then Exit Sub

        LogOperation(sender, e)
        LfWaitCursor(True)

        LbEventStop = True
        If LfCheckInput() = False Then
            LbEventStop = False
            LfWaitCursor(False)
            Exit Sub
        End If
        LiOutPutSTS = 0
        sSQL = LfGetSelectString("")
        Try

            Cnt = BaseSqlDataTableFill(sSQL, dtData)
            Select Case Cnt
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                Case 0              '�Y���Ȃ�
                    AlertBox.Show(Lexis.NoRecordsFound)
                    cmbEki.Select()
                Case Else

                    If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyPrinting).Equals( _
                        System.Windows.Forms.DialogResult.Yes) Then
                        LfPrint(dtData)
                    End If
            End Select

            LbEventStop = False
            LfWaitCursor(False)

            Select Case LiOutPutSTS
                Case 5              '�N���A������
                    AlertBox.Show(Lexis.PrintEndItClearDate)
                Case 4              '�ڐ݂���
                    AlertBox.Show(Lexis.PrintEndItMachineChange)
                Case 3              '�J�n�`�I���̓��t���t�]
                    AlertBox.Show(Lexis.PrintEndItDateReverse)
            End Select
        Catch ex As Exception
            LbEventStop = False
            LfWaitCursor(False)
            Log.Error("Select data failed.", ex)    '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W
            btnReturn.Select()
        End Try
    End Sub

    ''' <summary>
    ''' [�ꗗ�ݒ�]
    ''' </summary>
    ''' <param name="dt">�ݒ�Ώۃf�[�^�e�[�u��</param>
    Private Sub LfPrint(ByVal dt As DataTable)
        Dim Flg_G As Boolean = False
        Dim Flg_Y As Boolean = False
        Dim Cnt_X As Integer
        '-------Ver0.1�@�k���Ή��@ADD START-----------
        Dim e As Integer = 0
        Dim sLen As Integer = 0
        LsXlsTemplatePath = ""
        '�f�[�^���X�g�t���O
        Dim Flg_G_List As New ArrayList
        Dim Flg_Y_List As New ArrayList
        '�f�[�^���X�g�����ݒ�
        For e = 0 To Config.KadoPrintListK.Count - 1
            Flg_G_List.Add(False)
            Flg_Y_List.Add(False)
        Next
        '-------Ver0.1�@�k���Ή��@ADD   END-----------
        Try
            Dim sPath As String = Config.LedgerTemplateDirPath
            '�e���v���[�g�i�[�t�H���_�`�F�b�N
            If Directory.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If


            '�e���v���[�g�t���p�X�`�F�b�N
            '�@�@���D�@�A���������@�����o���ʂɂ��邩���`�F�b�N
            If cmbKisyu.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
                '���o���ʂ́u�@��v������
                If chkLastData.Checked Then
                    Cnt_X = 3
                Else
                    Cnt_X = 3
                End If
                For i As Integer = 0 To dt.Rows.Count - 1
                    '-------Ver0.1�@�k���Ή��@ADD START-----------
                    '�f�[�^�����݂���ꍇ
                    If dt.Rows(i)(Cnt_X).ToString = "G" Then
                        Flg_G = True
                        '�f�[�^���X�g�t���O�ɁhTRUE�h
                        If chkLastData.Checked Then
                            Flg_G_List(CInt(dt.Rows(i)(6).ToString)) = True
                        Else
                            Flg_G_List(CInt(dt.Rows(i)(15).ToString)) = True
                        End If
                    Else
                        Flg_Y = True
                        '�f�[�^���X�g�t���O�ɁhTRUE�h
                        If chkLastData.Checked Then
                            Flg_Y_List(CInt(dt.Rows(i)(6).ToString)) = True
                        Else
                            Flg_Y_List(CInt(dt.Rows(i)(15).ToString)) = True
                        End If
                    End If
                    '-------Ver0.1�@�k���Ή��@ADD END-----------
                Next
            Else
                '-------Ver0.1�@�k���Ή��@ADD START-----------
                If cmbKisyu.SelectedValue.ToString = "G" Then
                    Flg_G = True
                    '�I�������w�̃O���[�v�ԍ��ɊY������f�[�^���X�g�t���O��ݒ�
                    Flg_G_List(CInt(cmbEki.SelectedValue.ToString.Substring(0, 1))) = True
                Else
                    Flg_Y = True
                    '�I�������w�̃O���[�v�ԍ��ɊY������f�[�^���X�g�t���O��ݒ�
                    Flg_Y_List(CInt(cmbEki.SelectedValue.ToString.Substring(0, 1))) = True

                End If
                '-------Ver0.1�@�k���Ή��@ADD END-----------
            End If
            If optKado.Checked Then
                '�ғ��f�[�^�o��
                If Flg_G Then
                    '-------Ver0.1�@�k���Ή��@MOD START-----------
                    '�O���[�v�ԍ����L�[�ɒ��[���o�͂���
                    For e = 0 To Flg_G_List.Count - 1
                        '�f�[�^�����݂���ꍇ�̂ݏo��
                        If CBool(Flg_G_List(e)) Then
                            LsXlsTemplatePath = Path.Combine(sPath, Config.KadoPrintListK(e).ToString)
                            GrpNo = e
                            '���[�t�H�[�}�b�g�����݂��Ȃ��ꍇ
                            If File.Exists(LsXlsTemplatePath) = False Then
                                Log.Error("It's not found [" & LsXlsTemplatePath & "].")
                                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                                btnReturn.Select()
                                Exit Sub
                            End If
                            LfXlsDBGet("G")
                            If chkLastData.Checked Then
                                '�o��
                                LfXlsStart_KadoNewG()
                            Else
                                LfXlsStart_KadoHosyuG()
                            End If
                        End If
                    Next e
                    'End If
                    '-------Ver0.1�@�k���Ή��@ADD END-----------
                End If
                If Flg_Y Then
                    '-------Ver0.1�@�k���Ή��@MOD START-----------
                    '�O���[�v�ԍ����L�[�ɒ��[���o�͂���
                    For e = 0 To Flg_Y_List.Count - 1
                        '�f�[�^�����݂���ꍇ�̂ݏo��
                        If CBool(Flg_Y_List(e)) Then
                            LsXlsTemplatePath = Path.Combine(sPath, LcstXlsTemplateNameKadoY)
                            GrpNo = e
                            If File.Exists(LsXlsTemplatePath) = False Then
                                Log.Error("It's not found [" & LsXlsTemplatePath & "].")
                                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                                btnReturn.Select()
                                Exit Sub
                            End If
                            LfXlsDBGet("Y")
                            If chkLastData.Checked Then
                                '�o��
                                LfXlsStart_KadoNewY()
                            Else
                                LfXlsStart_KadoY()
                            End If
                        End If
                    Next e
                End If
            Else
                '�ێ�f�[�^�o��
                If Flg_G Then
                    '-------Ver0.1�@�k���Ή��@MOD START-----------
                    '�O���[�v�ԍ����L�[�ɒ��[���o�͂���
                    For e = 0 To Flg_G_List.Count - 1
                        '�f�[�^�����݂���ꍇ�̂ݏo��
                        If CBool(Flg_G_List(e)) Then
                            '�w��x�Ђ�I�����ꂽ�ꍇ�A�O���[�v�ԍ����L�[�ɒ��[���o�͂���
                            LsXlsTemplatePath = Path.Combine(sPath, Config.KadoPrintListH(e).ToString)
                            GrpNo = e
                            If File.Exists(LsXlsTemplatePath) = False Then
                                Log.Error("It's not found [" & LsXlsTemplatePath & "].")
                                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                                btnReturn.Select()
                                Exit Sub
                            End If
                            LfXlsDBGet("G")
                            LfXlsStart_KadoHosyuG()
                        End If
                    Next e
                    '-------Ver0.1�@�k���Ή��@ADD END-----------
                End If
            End If
            cmbEki.Select()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            '�G���[���b�Z�[�W
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        End Try
    End Sub

    ''' <summary>
    ''' [�o�͏���]
    ''' </summary>
    Public Function LfXlsDBGet(ByVal sModel As String) As Boolean
        Dim bRtn As Boolean = False
        Dim sBuilder As New StringBuilder

        Dim sSQL As String = ""
        Dim sTragetTABLE As String = ""
        Dim sSubSQL As String = ""
        Dim sSQL_Kai As String = ""
        Dim sSQL_Syu As String = ""
        Dim sSQL_Gou As String = ""
        Dim dbCtl As New DatabaseTalker
        Dim dtTable As New DataTable

        'DB�I�[�v��
        Try
            dbCtl.ConnectOpen()
        Catch ex As DatabaseException

        End Try

        'DB�ڑ��Ɏ��s���܂���
        If dbCtl.IsConnect = False Then
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Return bRtn
        End If

        '�ғ�/�ێ烌�R�[�h�̏o�͑Ώۗ񖼒��o
        sBuilder.Length = 0
        sBuilder.AppendLine("SELECT")
        sBuilder.AppendLine("    KAI_FLD,SYU_FLD,GOU_FLD")
        sBuilder.AppendLine(" FROM")
        sBuilder.AppendLine("    M_KADOHOSYU_FIELD")
        sBuilder.AppendLine(" WHERE")
        sBuilder.AppendLine(String.Format(" MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
        '-------Ver0.1�@�k���Ή��@ADD START-----------
        sBuilder.AppendLine(" AND GROUP_NO = " & GrpNo)
        '-------Ver0.1�@�k���Ή��@ADD END-----------
        If optKado.Checked Then
            sBuilder.AppendLine(" AND (DATA_SYUBETU='0' OR DATA_SYUBETU='3')")
        Else
            sBuilder.AppendLine(" AND (DATA_SYUBETU='1' OR DATA_SYUBETU='2')")
        End If
        sBuilder.AppendLine(" ORDER BY")
        sBuilder.AppendLine("    DATA_SYUBETU,KOMOKU_NO")
        sSQL = sBuilder.ToString()
        Try
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As Exception
            '�ڑ������Ɏ��s���܂���
            AlertBox.Show(Lexis.ConnectFailed)
            dbCtl.ConnectClose()
            dtTable = Nothing
            dbCtl = Nothing
            Return bRtn
        End Try

        '���D�A�W�D�A���v���̍��ږ��̂���r�p�k���𐶐�
        For i As Integer = 0 To dtTable.Rows.Count - 1
            If dtTable.Rows(i)(0).ToString <> "" Then
                sSQL_Kai = sSQL_Kai & "," & dtTable.Rows(i)(0).ToString & " AS No" & i
            Else
                sSQL_Kai = sSQL_Kai & ",Null" & " AS No" & i
            End If
            If sModel <> "Y" Then
                If dtTable.Rows(i)(1).ToString <> "" Then
                    sSQL_Syu = sSQL_Syu & "," & dtTable.Rows(i)(1).ToString & " AS No" & i
                Else
                    sSQL_Syu = sSQL_Syu & ",Null" & " AS No" & i
                End If
                If dtTable.Rows(i)(2).ToString <> "" Then
                    sSQL_Gou = sSQL_Gou & "," & dtTable.Rows(i)(2).ToString & " AS No" & i
                Else
                    sSQL_Gou = sSQL_Gou & ",Null" & " AS No" & i
                End If

            End If
        Next

        sBuilder.Length = 0
        If chkLastData.Checked Then
            '���D�@�̉ғ��i�ŐV�f�[�^�j�p�̒��[�o�͍��ڂ𒊏o
            sBuilder.AppendLine("SELECT")
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            sBuilder.AppendLine("     MC.*,DT.*")
            '-------Ver0.1�@�k���Ή��@MOD END-----------
            sBuilder.AppendLine(" FROM")
            sBuilder.AppendLine("     (")
            sBuilder.AppendLine("         SELECT")
            sBuilder.AppendLine("             STATION_NAME,CORNER_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,")
            sBuilder.AppendLine("             CORNER_CODE,MODEL_CODE,UNIT_NO")
            sBuilder.AppendLine("         FROM")
            sBuilder.AppendLine("             V_MACHINE_NOW")
            sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            '-------Ver0.1�@�k���Ή��@ADD START-----------
            sBuilder.AppendLine(" AND GROUP_NO = " & GrpNo)
            '-------Ver0.1�@�k���Ή��@ADD END-----------
            If LsMacSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsMacSQLWhere)
            End If
            sBuilder.AppendLine("     ) AS MC")
            sBuilder.AppendLine("     LEFT OUTER JOIN")
            sBuilder.AppendLine("         (")
            sBuilder.AppendLine("             SELECT")
            sBuilder.AppendLine("                 DA.*")
            sBuilder.AppendLine("             FROM")
            sBuilder.AppendLine("                 (")
            sBuilder.AppendLine("                     SELECT")
            sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,")
            sBuilder.AppendLine("                         MAX(PROCESSING_TIME) AS PROCESSING_TIME")
            sBuilder.AppendLine("                     FROM")
            sBuilder.AppendLine("                         D_KADO_DATA")
            sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            If LsBaseSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
            End If
            sBuilder.AppendLine("                     GROUP BY")
            sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO")
            sBuilder.AppendLine("                 ) AS LT,")
            sBuilder.AppendLine("                 (")
            sBuilder.AppendLine("                     SELECT")
            sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'0' AS KBN,PROCESSING_TIME,")
            sBuilder.AppendLine("                         KAI_SERIAL_NO AS SERIAL_NO")
            sBuilder.AppendLine(sSQL_Kai)
            sBuilder.AppendLine("                     FROM")
            sBuilder.AppendLine("                         D_KADO_DATA")
            sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            If LsBaseSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
            End If
            If sModel <> "Y" Then
                sBuilder.AppendLine("                     UNION")
                sBuilder.AppendLine("                     SELECT")
                sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'1' AS KBN,PROCESSING_TIME,")
                sBuilder.AppendLine("                         SYU_SERIAL_NO AS SERIAL_NO")
                sBuilder.AppendLine(sSQL_Syu)
                sBuilder.AppendLine("                     FROM")
                sBuilder.AppendLine("                         D_KADO_DATA")
                sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
                sBuilder.AppendLine("                     UNION")
                sBuilder.AppendLine("                     SELECT")
                sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'2' AS KBN,PROCESSING_TIME,")
                sBuilder.AppendLine("                         '' AS SERIAL_NO")
                sBuilder.AppendLine(sSQL_Gou)
                sBuilder.AppendLine("                     FROM")
                sBuilder.AppendLine("                         D_KADO_DATA")
                sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
            End If
            sBuilder.AppendLine("                 ) AS DA")
            sBuilder.AppendLine("             WHERE")
            sBuilder.AppendLine("                 LT.RAIL_SECTION_CODE = DA.RAIL_SECTION_CODE")
            sBuilder.AppendLine("             AND LT.STATION_ORDER_CODE = DA.STATION_ORDER_CODE")
            sBuilder.AppendLine("             AND LT.CORNER_CODE = DA.CORNER_CODE")
            sBuilder.AppendLine("             AND LT.MODEL_CODE = DA.MODEL_CODE")
            sBuilder.AppendLine("             AND LT.UNIT_NO = DA.UNIT_NO")
            sBuilder.AppendLine("             AND LT.PROCESSING_TIME = DA.PROCESSING_TIME")
            sBuilder.AppendLine("         ) AS DT")
            sBuilder.AppendLine("     ON  MC.RAIL_SECTION_CODE = DT.RAIL_SECTION_CODE")
            sBuilder.AppendLine("     AND MC.STATION_ORDER_CODE = DT.STATION_ORDER_CODE")
            sBuilder.AppendLine("     AND MC.CORNER_CODE = DT.CORNER_CODE")
            sBuilder.AppendLine("     AND MC.MODEL_CODE = DT.MODEL_CODE")
            sBuilder.AppendLine("     AND MC.UNIT_NO = DT.UNIT_NO")
            '-------Ver0.1�@�k���Ή��@ADD START-----------
            sBuilder.AppendLine(" ORDER BY")
            sBuilder.AppendLine("     MC.RAIL_SECTION_CODE,MC.STATION_ORDER_CODE,MC.CORNER_CODE,")
            sBuilder.AppendLine("     MC.MODEL_CODE,MC.UNIT_NO,DT.KBN")
            '-------Ver0.1�@�k���Ή��@ADD END-----------
        Else

            If optKado.Checked Then
                sTragetTABLE = "D_KADO_DATA"
            Else
                sTragetTABLE = "D_HOSYU_DATA"
            End If

            sBuilder.AppendLine("SELECT")
            sBuilder.AppendLine("     MC.*,DT.*")
            sBuilder.AppendLine(" FROM")
            sBuilder.AppendLine("     (")
            sBuilder.AppendLine("         SELECT")
            sBuilder.AppendLine("             STATION_NAME,CORNER_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,")
            sBuilder.AppendLine("             CORNER_CODE,MODEL_CODE,UNIT_NO")
            sBuilder.AppendLine("         FROM")
            sBuilder.AppendLine("             V_MACHINE_NOW")
            sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            '-------Ver0.1�@�k���Ή��@ADD START-----------
            sBuilder.AppendLine(" AND GROUP_NO = " & GrpNo)
            '-------Ver0.1�@�k���Ή��@ADD END-----------
            If LsMacSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsMacSQLWhere)
            End If
            sBuilder.AppendLine("     ) AS MC")
            sBuilder.AppendLine("     LEFT OUTER JOIN")
            sBuilder.AppendLine("         (")
            sBuilder.AppendLine("             SELECT")
            sBuilder.AppendLine("                 LT.STS,")
            sBuilder.AppendLine("                 CASE")
            sBuilder.AppendLine("                     WHEN DA.RANGE = '0' THEN LT.S_KENSYUU")
            sBuilder.AppendLine("                     WHEN DA.RANGE = '1' THEN LT.E_KENSYUU")
            sBuilder.AppendLine("                 END AS KENSYUU,")
            sBuilder.AppendLine("                 DA.*")
            sBuilder.AppendLine("             FROM")
            sBuilder.AppendLine("                 (")

            sSQL = LfGetSelectString(sModel)
            sBuilder.AppendLine(sSQL)

            sBuilder.AppendLine("                 ) AS LT,")
            sBuilder.AppendLine("                 (")
            '�@�@���D�̊J�n���R�[�h
            sBuilder.AppendLine("                     SELECT")
            sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'0' AS KBN,'1' AS KBN1,'0' AS RANGE,")
            sBuilder.AppendLine("                         PROCESSING_TIME,KAI_SERIAL_NO AS SERIAL_NO")
            '-------Ver0.1�@�k���Ή��@ADD START-----------
            sBuilder.AppendLine("                        , COLLECT_START_TIME AS COLLECT_TIME")
            sBuilder.AppendLine("                        , KAI_INSPECT_TIME AS INSPECT_TIME")
            '-------Ver0.1�@�k���Ή��@ADD END-----------
            sBuilder.AppendLine(sSQL_Kai)
            sBuilder.AppendLine("                     FROM ")
            sBuilder.AppendLine(sTragetTABLE)
            sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            If LsBaseSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
            End If
            sBuilder.AppendLine("                     UNION")
            '�@�@���D�̏I�����R�[�h
            sBuilder.AppendLine("                     SELECT")
            sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'0' AS KBN,'1' AS KBN1,'1' AS RANGE,")
            sBuilder.AppendLine("                         PROCESSING_TIME,KAI_SERIAL_NO AS SERIAL_NO")
            '-------Ver0.1�@�k���Ή��@ADD START-----------
            sBuilder.AppendLine("                        , COLLECT_START_TIME AS COLLECT_TIME")
            sBuilder.AppendLine("                        , KAI_INSPECT_TIME AS INSPECT_TIME")
            '-------Ver0.1�@�k���Ή��@ADD END-----------
            sBuilder.AppendLine(sSQL_Kai)
            sBuilder.AppendLine("                     FROM ")
            sBuilder.AppendLine(sTragetTABLE)
            sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            If LsBaseSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
            End If
            If sModel <> "Y" Then
                sBuilder.AppendLine("                     UNION")
                '�@�@�W�D�̊J�n���R�[�h
                sBuilder.AppendLine("                     SELECT")
                sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'1' AS KBN,'2' AS KBN1,'0' AS RANGE,")
                sBuilder.AppendLine("                         PROCESSING_TIME,SYU_SERIAL_NO AS SERIAL_NO")
                '-------Ver0.1�@�k���Ή��@ADD START-----------
                sBuilder.AppendLine("                        , COLLECT_START_TIME AS COLLECT_TIME")
                sBuilder.AppendLine("                        , SYU_INSPECT_TIME AS INSPECT_TIME")
                '-------Ver0.1�@�k���Ή��@ADD END-----------
                sBuilder.AppendLine(sSQL_Syu)
                sBuilder.AppendLine("                     FROM ")
                sBuilder.AppendLine(sTragetTABLE)
                sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
                sBuilder.AppendLine("                     UNION")
                '�@�@�W�D�̏I�����R�[�h
                sBuilder.AppendLine("                     SELECT")
                sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'1' AS KBN,'2' AS KBN1,'1' AS RANGE,")
                sBuilder.AppendLine("                         PROCESSING_TIME,SYU_SERIAL_NO AS SERIAL_NO")
                '-------Ver0.1�@�k���Ή��@ADD START-----------
                sBuilder.AppendLine("                        , COLLECT_START_TIME AS COLLECT_TIME")
                sBuilder.AppendLine("                        , SYU_INSPECT_TIME AS INSPECT_TIME")
                '-------Ver0.1�@�k���Ή��@ADD END-----------
                sBuilder.AppendLine(sSQL_Syu)
                sBuilder.AppendLine("                     FROM ")
                sBuilder.AppendLine(sTragetTABLE)
                sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
                sBuilder.AppendLine("                     UNION")
                '�@�@���v�i���D�p�j�̊J�n���R�[�h
                sBuilder.AppendLine("                     SELECT")
                sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'2' AS KBN,'1' AS KBN1,'0' AS RANGE,")
                sBuilder.AppendLine("                         PROCESSING_TIME,KAI_SERIAL_NO AS SERIAL_NO")
                '-------Ver0.1�@�k���Ή��@ADD START-----------
                sBuilder.AppendLine("                        , COLLECT_START_TIME AS COLLECT_TIME")
                sBuilder.AppendLine("                        , KAI_INSPECT_TIME AS INSPECT_TIME")
                '-------Ver0.1�@�k���Ή��@ADD END-----------
                sBuilder.AppendLine(sSQL_Gou)
                sBuilder.AppendLine("                     FROM ")
                sBuilder.AppendLine(sTragetTABLE)
                sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
                sBuilder.AppendLine("                     UNION")
                '�@�@���v�i���D�p�j�̏I�����R�[�h
                sBuilder.AppendLine("                     SELECT")
                sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'2' AS KBN,'1' AS KBN1,'1' AS RANGE,")
                sBuilder.AppendLine("                         PROCESSING_TIME,KAI_SERIAL_NO AS SERIAL_NO")
                '-------Ver0.1�@�k���Ή��@ADD START-----------
                sBuilder.AppendLine("                        , COLLECT_START_TIME AS COLLECT_TIME")
                sBuilder.AppendLine("                        , KAI_INSPECT_TIME AS INSPECT_TIME")
                '-------Ver0.1�@�k���Ή��@ADD END-----------
                sBuilder.AppendLine(sSQL_Gou)
                sBuilder.AppendLine("                     FROM ")
                sBuilder.AppendLine(sTragetTABLE)
                sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
                sBuilder.AppendLine("                     UNION")
                '�@�@���v�i�W�D�p�j�̊J�n���R�[�h
                sBuilder.AppendLine("                     SELECT")
                sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'2' AS KBN,'2' AS KBN1,'0' AS RANGE,")
                sBuilder.AppendLine("                         PROCESSING_TIME,SYU_SERIAL_NO AS SERIAL_NO")
                '-------Ver0.1�@�k���Ή��@ADD START-----------
                sBuilder.AppendLine("                        , COLLECT_START_TIME AS COLLECT_TIME")
                sBuilder.AppendLine("                        , SYU_INSPECT_TIME AS INSPECT_TIME")
                '-------Ver0.1�@�k���Ή��@ADD END-----------
                sBuilder.AppendLine(sSQL_Gou)
                sBuilder.AppendLine("                     FROM ")
                sBuilder.AppendLine(sTragetTABLE)
                sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
                sBuilder.AppendLine("                     UNION")
                '�@�@���v�i�W�D�p�j�̏I�����R�[�h
                sBuilder.AppendLine("                     SELECT")
                sBuilder.AppendLine("                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                         MODEL_CODE,UNIT_NO,'2' AS KBN,'2' AS KBN1,'1' AS RANGE,")
                sBuilder.AppendLine("                         PROCESSING_TIME,SYU_SERIAL_NO AS SERIAL_NO")
                '-------Ver0.1�@�k���Ή��@ADD START-----------
                sBuilder.AppendLine("                        , COLLECT_START_TIME AS COLLECT_TIME")
                sBuilder.AppendLine("                        , SYU_INSPECT_TIME AS INSPECT_TIME")
                '-------Ver0.1�@�k���Ή��@ADD END-----------
                sBuilder.AppendLine(sSQL_Gou)
                sBuilder.AppendLine("                     FROM ")
                sBuilder.AppendLine(sTragetTABLE)
                sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            End If
            If LsBaseSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
            End If
            sBuilder.AppendLine("                 ) AS DA")
            sBuilder.AppendLine("             WHERE")
            sBuilder.AppendLine("                 LT.RAIL_SECTION_CODE = DA.RAIL_SECTION_CODE")
            sBuilder.AppendLine("             AND LT.STATION_ORDER_CODE = DA.STATION_ORDER_CODE")
            sBuilder.AppendLine("             AND LT.CORNER_CODE = DA.CORNER_CODE")
            sBuilder.AppendLine("             AND LT.MODEL_CODE = DA.MODEL_CODE")
            sBuilder.AppendLine("             AND LT.UNIT_NO = DA.UNIT_NO")
            sBuilder.AppendLine("             AND LT.KBN = DA.KBN1")
            sBuilder.AppendLine("             AND LT.SERIAL_NO = DA.SERIAL_NO")
            sBuilder.AppendLine("             AND ((DA.RANGE = '0'")
            sBuilder.AppendLine("                     AND LT.S_PROCESSING_TIME = DA.PROCESSING_TIME)")
            sBuilder.AppendLine("              OR  (DA.RANGE = '1'")
            sBuilder.AppendLine("                     AND LT.E_PROCESSING_TIME = DA.PROCESSING_TIME))")
            sBuilder.AppendLine("         ) AS DT")
            sBuilder.AppendLine("     ON  MC.RAIL_SECTION_CODE = DT.RAIL_SECTION_CODE")
            sBuilder.AppendLine("     AND MC.STATION_ORDER_CODE = DT.STATION_ORDER_CODE")
            sBuilder.AppendLine("     AND MC.CORNER_CODE = DT.CORNER_CODE")
            sBuilder.AppendLine("     AND MC.MODEL_CODE = DT.MODEL_CODE")
            sBuilder.AppendLine("     AND MC.UNIT_NO = DT.UNIT_NO")
            sBuilder.AppendLine(" ORDER BY")
            sBuilder.AppendLine("     MC.RAIL_SECTION_CODE,MC.STATION_ORDER_CODE,MC.CORNER_CODE,")
            sBuilder.AppendLine("     MC.MODEL_CODE,MC.UNIT_NO,DT.KBN,DT.KBN1,DT.RANGE")
        End If
        sSQL = sBuilder.ToString()

        Try
            LdtTarget = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As Exception
            '�ڑ������Ɏ��s���܂���
            AlertBox.Show(Lexis.ConnectFailed)
            dbCtl.ConnectClose()
            LdtTarget = Nothing
            dbCtl = Nothing
            Return bRtn
        End Try

        '�ғ�/�ێ�̏o�͍��ږ��A��l�𒊏o
        sBuilder.Length = 0
        sBuilder.AppendLine("SELECT")
        sBuilder.AppendLine("    KOMOKU_NAME,KAISATUKIJUN,SYUSATUKIJUN,DATA_SYUBETU")
        sBuilder.AppendLine(" FROM")
        sBuilder.AppendLine("    M_KADOHOSYU_SET")
        sBuilder.AppendLine(" WHERE")
        sBuilder.AppendLine(String.Format(" MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
        '-------Ver0.1�@�k���Ή��@ADD START-----------
        sBuilder.AppendLine(" AND GROUP_NO = " & GrpNo)
        '-------Ver0.1�@�k���Ή��@ADD END-----------
        If optKado.Checked Then
            sBuilder.AppendLine(" AND (DATA_SYUBETU='0' OR DATA_SYUBETU='3')")
        Else
            sBuilder.AppendLine(" AND (DATA_SYUBETU='1' OR DATA_SYUBETU='2')")
        End If
        sBuilder.AppendLine(" ORDER BY")
        sBuilder.AppendLine("    DATA_SYUBETU,KOMOKU_NO")
        sSQL = sBuilder.ToString()
        Try
            LdtKoumoku = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As Exception
            '�ڑ������Ɏ��s���܂���
            AlertBox.Show(Lexis.ConnectFailed)
            dbCtl.ConnectClose()
            LdtKoumoku = Nothing
            dbCtl = Nothing
            Return bRtn
        End Try

        dbCtl.ConnectClose()
        Return True

    End Function

    '//////////////////////////////////////////////CheckedChanged
    '''<summary>
    ''' �u�ێ�f�[�^�^�ғ��f�[�^�v���W�I�{�^��
    ''' </summary>
    Private Sub optMente_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles optMente.CheckedChanged, optKado.CheckedChanged
        If CType(sender, RadioButton).Name = "optKado" Then    '�ғ��f�[�^�n�m
            '�O��N���A���i�ێ�f�[�^�j�̃`�F�b�N�{�b�N�X�𖳌�������B
            chkFromLastClear.Enabled = False
            chkFromLastClear.Checked = False
            '�J�n�����̓��t��L��������B
            optFromDate.Enabled = True
            dtpYmdFrom.Enabled = True
            '�J�n�����̌��C��L��������B
            optFromKensyu.Enabled = True
            cmbKensyuFrom.Enabled = True
            '�J�n�����S�̂�L�����܂��͖���������B
            If chkLastData.Checked Then
                grpFrom.Enabled = False
                grpTo.Enabled = False
            Else
                grpFrom.Enabled = True
                grpTo.Enabled = True
            End If
            '�ŐV�f�[�^�i�ғ��f�[�^�j�̃`�F�b�N�{�b�N�X��L��������B
            chkLastData.Enabled = True
        Else
            '�O��N���A���i�ێ�f�[�^�j�̃`�F�b�N�{�b�N�X��L��������B
            chkFromLastClear.Enabled = True
            If chkFromLastClear.Checked Then
                '�J�n�����̓��t�𖳌�������B
                optFromDate.Enabled = False
                dtpYmdFrom.Enabled = False
                '�J�n�����̌��C�𖳌�������B
                optFromKensyu.Enabled = False
                cmbKensyuFrom.Enabled = False
            End If
            '�J�n�����S�̂�L��������B
            grpFrom.Enabled = True
            grpTo.Enabled = True
            '�ŐV�f�[�^�i�ғ��f�[�^�j�̃`�F�b�N�{�b�N�X�𖳌�������B
            '-------Ver0.1�@�k���Ή��@ADD START-----------
            chkLastData.Checked = False
            chkLastData.Enabled = False
            '-------Ver0.1�@�k���Ή��@ADD END-----------
        End If
    End Sub

    '''<summary>
    ''' �u�O��N���A���i�ێ�f�[�^�j�v�`�F�b�N�{�b�N�X
    ''' </summary>
    Private Sub chkFromLastClear_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkFromLastClear.CheckedChanged
        If chkFromLastClear.Checked Then
            '�J�n�����̓��t�𖳌�������B
            optFromDate.Enabled = False
            dtpYmdFrom.Enabled = False
            '�J�n�����̌��C�𖳌�������B
            optFromKensyu.Enabled = False
            cmbKensyuFrom.Enabled = False
        Else
            '�J�n�����̓��t��L��������B
            optFromDate.Enabled = True
            dtpYmdFrom.Enabled = True
            '�J�n�����̌��C��L��������B
            optFromKensyu.Enabled = True
            cmbKensyuFrom.Enabled = True
        End If
    End Sub

    '''<summary>
    ''' �u�O���v�`�F�b�N�{�b�N�X
    ''' </summary>
    Private Sub chkToYesterday_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkToYesterday.CheckedChanged
        If chkToYesterday.Checked Then
            '�I�������̓��t�𖳌�������B
            optToDate.Enabled = False
            dtpYmdTo.Enabled = False
            '�I�������̌��C�𖳌�������B
            optToKensyu.Enabled = False
            cmbKensyuTo.Enabled = False
        Else
            '�I�������̓��t��L��������B
            optToDate.Enabled = True
            dtpYmdTo.Enabled = True
            '�I�������̌��C��L��������B
            optToKensyu.Enabled = True
            cmbKensyuTo.Enabled = True
        End If
    End Sub

    '''<summary>
    ''' �u�ŐV�f�[�^�i�ғ��f�[�^�j�v�`�F�b�N�{�b�N�X
    ''' </summary>
    Private Sub chkLastData_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkLastData.CheckedChanged
        '�J�n�����S�̂�L�����܂��͖���������B
        If chkLastData.Checked Then
            grpFrom.Enabled = False
            grpTo.Enabled = False
        Else
            grpFrom.Enabled = True
            grpTo.Enabled = True
        End If
    End Sub

    '//////////////////////////////////////////////SelectedIndexChanged

    '''<summary>
    ''' �u�w�v�R���{
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        GrpNo = 0
        Try
            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            '�O���[�vNo���擾
            Dim station As String = cmbEki.SelectedValue.ToString
            If station <> "" And station <> ClientDaoConstants.TERMINAL_ALL Then
                GrpNo = CInt(station.Substring(0, 1))
            End If
            '-------Ver0.1�@�k���Ή��@MOD END---------
            '-------Ver0.1�@�k���Ή��@ADD START-----------
            If LfSetMado(station.Substring(station.Length - 6, 6)) = False Then
                '-------Ver0.1�@�k���Ή��@ADD END-----------
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbMado.SelectedIndex = 0               '���C�x���g�����ӏ�

        Catch ex As Exception
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
            '�R�[�i�[�R���{�ݒ�
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            Dim station As String = cmbEki.SelectedValue.ToString
            LbEventStop = True      '�C�x���g�����n�e�e
            If LfSetKisyu(station.Substring(station.Length - 6, 6), cmbMado.SelectedValue.ToString) = False Then
                '-------Ver0.1�@�k���Ή��@MOD END-----------
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblKisyu.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If

            LbEventStop = False      '�C�x���g�����n�m
            cmbKisyu.SelectedIndex = 0               '���C�x���g�����ӏ�

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    '''<summary>
    ''' �u�@��v�R���{
    ''' </summary>
    Private Sub cmbKisyu_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbKisyu.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        Dim rowView As DataRowView
        Dim ChkFlg As Boolean = True

        If cmbKisyu.Items.Count = 2 Then
            '�S�@��{���������@�̏ꍇ��False
            rowView = CType(cmbKisyu.Items(1), DataRowView)
            If rowView.Row(0).ToString = "Y" Then
                ChkFlg = False
            End If
        Else
            '���������@�̏ꍇ��False
            If cmbKisyu.SelectedValue.ToString = "Y" Then
                ChkFlg = False
            End If
        End If

        '���������@�̏ꍇ�A�u�ێ�f�[�^�v��I��s��
        If ChkFlg Then
            optKado.Select()
            optMente.Enabled = True
        Else
            optKado.Select()
            optMente.Enabled = False
        End If
    End Sub

#End Region

#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    ''' [�J�n�I�����ݒ�]
    ''' </summary>
    Private Sub LfSetDateFromTo()
        Dim dtWork As DateTime = DateAdd(DateInterval.Day, -1, Today)
        Dim dtFrom As New DateTime(dtWork.Year, dtWork.Month, dtWork.Day, 0, 0, 0)
        Dim dtTo As DateTime = Now
        dtpYmdFrom.Format = DateTimePickerFormat.Custom
        dtpYmdFrom.CustomFormat = "yyyy�NMM��dd��"
        dtpYmdFrom.Value = dtFrom
        dtpYmdTo.Format = DateTimePickerFormat.Custom
        dtpYmdTo.CustomFormat = "yyyy�NMM��dd��"
        dtpYmdTo.Value = dtTo
    End Sub

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
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            dt = oMst.SelectTable(True, "G,Y", True)
            '-------Ver0.1�@�k���Ή��@MOD END-----------
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
    ''' [�R�[�i�[�R���{�ݒ�]
    ''' </summary>
    ''' <param name="Station">�w�R�[�h</param>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function LfSetMado(ByVal Station As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As CornerMaster
        oMst = New CornerMaster
        Try
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
            If String.IsNullOrEmpty(Station) Then
                Station = ""
            End If
            If String.IsNullOrEmpty(Corner) Then
                Corner = ""
            End If
            If ((Station <> "" AndAlso Station <> ClientDaoConstants.TERMINAL_ALL) _
            AndAlso (Corner <> "" AndAlso Corner <> ClientDaoConstants.TERMINAL_ALL)) Then
                dt = oMst.SelectTable(Station, Corner, True)
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
        End Try
        Return bRtn
    End Function

    Private Sub LfSetKensyuFrom()
        Dim drw As DataRow
        Dim dt As DataTable = New DataTable()
        Dim array() As String = New String() {"�P��O", "�Q��O", "�R��O", "�S��O", "�T��O", "�U��O", "�V��O", _
                                "�W��O", "�X��O", "�P�O��O", "�P�P��O", "�P�Q��O", "�P�R��O", "�P�S��O", "�P�T��O"}

        dt.Columns.Add("CODE")
        dt.Columns.Add("NAME")
        For i As Integer = 0 To array.Count - 1
            drw = dt.NewRow()
            drw.ItemArray = New Object() {i + 2, array(i)}
            dt.Rows.Add(drw)
        Next
        cmbKensyuFrom.DataSource = dt
        '�\�������o�[�̐ݒ�
        cmbKensyuFrom.DisplayMember = dt.Columns(1).ColumnName
        '�o�����[�����o�[�̐ݒ�
        cmbKensyuFrom.ValueMember = dt.Columns(0).ColumnName
        drw = Nothing

    End Sub

    Private Sub LfSetKensyuTo()
        Dim drw As DataRow
        Dim dt As DataTable = New DataTable()
        Dim array() As String = New String() {"�ŐV���C", "�P��O", "�Q��O", "�R��O", "�S��O", "�T��O", "�U��O", _
                                "�V��O", "�W��O", "�X��O", "�P�O��O", "�P�P��O", "�P�Q��O", "�P�R��O", "�P�S��O"}

        dt.Columns.Add("CODE")
        dt.Columns.Add("NAME")
        For i As Integer = 0 To array.Count - 1
            drw = dt.NewRow()
            drw.ItemArray = New Object() {i + 1, array(i)}
            dt.Rows.Add(drw)
        Next
        cmbKensyuTo.DataSource = dt
        '�\�������o�[�̐ݒ�
        cmbKensyuTo.DisplayMember = dt.Columns(1).ColumnName
        '�o�����[�����o�[�̐ݒ�
        cmbKensyuTo.ValueMember = dt.Columns(0).ColumnName
        drw = Nothing

    End Sub


    ''' <summary>
    ''' [���������`�F�b�N]
    ''' </summary>
    ''' <returns>"":OK,""�ȊO:���͕s������i�G���[�̐擪�̃��b�Z�[�W�t������ԋp�j</returns>
    Private Function LfCheckInput() As Boolean
        Dim sRtn As Boolean = True

        LsFromKensyu = ""
        LsToKensyu = ""
        LsFromDate = ""
        LsToDate = ""
        LbLastClear = False

        '�J�n����
        If optFromDate.Enabled = True And optFromDate.Checked Then
            LsFromDate = Replace(Replace(Replace(dtpYmdFrom.Text, "�N", ""), "��", ""), "��", "")
        End If
        If optFromKensyu.Enabled = True And optFromKensyu.Checked Then
            LsFromKensyu = cmbKensyuFrom.SelectedValue.ToString
        End If
        If chkFromLastClear.Enabled = True And chkFromLastClear.Checked Then
            LbLastClear = True
        End If

        '�I������
        If optToDate.Enabled = True And optToDate.Checked Then
            LsToDate = Replace(Replace(Replace(dtpYmdTo.Text, "�N", "/"), "��", "/"), "��", "")
            LsToDate = DateAdd(DateInterval.Day, 1, Date.Parse(LsToDate)).ToString("yyyyMMdd")
        End If
        If optToKensyu.Enabled = True And optToKensyu.Checked Then
            LsToKensyu = cmbKensyuTo.SelectedValue.ToString
        End If
        If chkToYesterday.Enabled = True And chkToYesterday.Checked Then
            LsToDate = Now.ToString("yyyyMMdd")
        End If

        '���C�����`�F�b�N
        If LsFromKensyu <> "" And LsToKensyu <> "" Then
            If LsFromKensyu < LsToKensyu Then
                AlertBox.Show(Lexis.KensyuRangeIsInvalid)
                sRtn = False
                '-------Ver0.1�@�k���Ή��@ADD START-----------
            ElseIf LsFromKensyu = LsToKensyu Then
                AlertBox.Show(Lexis.KensyuRangeIsInvalid)
                sRtn = False
                '-------Ver0.1�@�k���Ή��@ADD START-----------
            End If
        End If

        '���t�����`�F�b�N
        If LsFromDate <> "" And LsToDate <> "" Then
            If LsFromDate > LsToDate Then
                AlertBox.Show(Lexis.DateRangeIsInvalid)
                sRtn = False
            End If
        End If

        Return sRtn
    End Function

    ''' <summary>
    ''' [�����pSELECT������擾]
    ''' </summary>
    ''' <returns>SELECT��</returns>
    Private Function LfGetSelectString(ByVal sModel As String) As String

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder
        Dim sTragetTABLE As String = ""

        LsBaseSQLWhere = ""
        LsMacSQLWhere = ""
        If cmbEki.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            Dim station As String = cmbEki.SelectedValue.ToString
            If cmbEki.SelectedValue.ToString.Substring(1, 3) = "000" Then
                LsMacSQLWhere = "BRANCH_OFFICE_CODE='" & station.Substring(station.Length - 3, 3) & "'"
            Else
                LsBaseSQLWhere = " RAIL_SECTION_CODE='" & station.Substring(station.Length - 6, 3) & "'"
                LsBaseSQLWhere = LsBaseSQLWhere & " AND STATION_ORDER_CODE='" & station.Substring(station.Length - 3, 3) & "'"
                If cmbMado.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
                    LsBaseSQLWhere = LsBaseSQLWhere & " AND CORNER_CODE='" & cmbMado.SelectedValue.ToString & "'"
                    If cmbKisyu.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
                        LsBaseSQLWhere = LsBaseSQLWhere & " AND MODEL_CODE='" & cmbKisyu.SelectedValue.ToString & "'"
                    End If
                End If
                '-------Ver0.1�@�k���Ή��@MOD END-----------
                LsMacSQLWhere = LsBaseSQLWhere
            End If
        End If

        sBuilder.Length = 0

        If optKado.Checked And chkLastData.Checked Then
            '�ғ��f�[�^�̍ŐV���R�[�h�擾

            '����A�w���A�R�[�i�A�@��A���@���ɍŐV�̎��W�������擾
            '�Y�����R�[�h�̗L�����`�F�b�N����r�p�k
            sBuilder.AppendLine("SELECT")
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            sBuilder.AppendLine("     LST.*,mac.GROUP_NO")
            sBuilder.AppendLine(" FROM")
            sBuilder.AppendLine("     (")
            sBuilder.AppendLine("         SELECT")
            sBuilder.AppendLine("             RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,")
            sBuilder.AppendLine("             UNIT_NO,GROUP_NO")
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            sBuilder.AppendLine("         FROM")
            sBuilder.AppendLine("             V_MACHINE_NOW")
            If LsMacSQLWhere <> "" Then
                sBuilder.AppendLine(" WHERE " & LsMacSQLWhere)
            End If
            sBuilder.AppendLine("     ) AS MAC,")
            sBuilder.AppendLine("     (")
            sBuilder.AppendLine("         SELECT")
            sBuilder.AppendLine("             RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,")
            sBuilder.AppendLine("             UNIT_NO,MAX(PROCESSING_TIME) AS PROCESSING_TIME")
            sBuilder.AppendLine("         FROM")
            sBuilder.AppendLine("             D_KADO_DATA")
            If LsBaseSQLWhere <> "" Then
                sBuilder.AppendLine(" WHERE " & LsBaseSQLWhere)
            End If
            sBuilder.AppendLine("         GROUP BY")
            sBuilder.AppendLine("             RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,")
            sBuilder.AppendLine("             UNIT_NO")
            sBuilder.AppendLine("     ) AS LST")
            sBuilder.AppendLine(" WHERE")
            sBuilder.AppendLine("     MAC.RAIL_SECTION_CODE = LST.RAIL_SECTION_CODE")
            sBuilder.AppendLine(" AND MAC.STATION_ORDER_CODE = LST.STATION_ORDER_CODE")
            sBuilder.AppendLine(" AND MAC.CORNER_CODE = LST.CORNER_CODE")
            sBuilder.AppendLine(" AND MAC.MODEL_CODE = LST.MODEL_CODE")
            sBuilder.AppendLine(" AND MAC.UNIT_NO = LST.UNIT_NO")
        Else
            If optKado.Checked Then
                sTragetTABLE = "D_KADO_DATA"
            Else
                sTragetTABLE = "D_HOSYU_DATA"
            End If

            '�Ώۂ̉ғ����R�[�h�������o�r�p�k
            sBuilder.AppendLine(" SELECT")
            sBuilder.AppendLine("    E.RAIL_SECTION_CODE,E.STATION_ORDER_CODE,E.CORNER_CODE,E.MODEL_CODE,")
            sBuilder.AppendLine("    E.UNIT_NO,E.KBN,E.SERIAL_NO,S.INSPECT_TIME AS S_INSPECT_TIME,")
            sBuilder.AppendLine("    E.INSPECT_TIME AS E_INSPECT_TIME,")
            '    ������No���قȂ�ꍇ�i�ڐݎ��j���P
            '    �W�v�J�n�������قȂ�ꍇ�i�N���A���j���Q
            '-------Ver0.1�@�k���Ή��@ADD START----------
            If optMente.Checked Then
                '�@�@�@�@�Q�F�i�N���A���j�́A�ێ�f�[�^�̂Ƃ��̂�
                sBuilder.AppendLine("    CASE ")
                sBuilder.AppendLine("    WHEN (S.SERIAL_NO <> E.SERIAL_NO AND E.KBN = '1' ) THEN  ")
                sBuilder.AppendLine("     ( ")
                sBuilder.AppendLine("      CASE  WHEN ( ")
                sBuilder.AppendLine("             SELECT  MIN (PROCESSING_TIME) ")
                sBuilder.AppendLine("             FROM D_HOSYU_DATA ")
                sBuilder.AppendLine("             WHERE KAI_SERIAL_NO = E.SERIAL_NO ")
                sBuilder.AppendLine("               AND RAIL_SECTION_CODE = E.RAIL_SECTION_CODE")
                sBuilder.AppendLine("               AND STATION_ORDER_CODE = E.STATION_ORDER_CODE ")
                sBuilder.AppendLine("               AND CORNER_CODE = E.CORNER_CODE  ")
                sBuilder.AppendLine("              AND MODEL_CODE = E.MODEL_CODE  ")
                sBuilder.AppendLine("              AND UNIT_NO = E.UNIT_NO  ")
                sBuilder.AppendLine("              ) >= (  ")
                sBuilder.AppendLine("               SELECT MIN (PROCESSING_TIME) ")
                sBuilder.AppendLine("               FROM  D_HOSYU_DATA  ")
                sBuilder.AppendLine("               WHERE KAI_SERIAL_NO = E.SERIAL_NO  ")
                sBuilder.AppendLine("               AND COLLECT_START_TIME = E.COLLECT_START_TIME ")
                sBuilder.AppendLine("               AND RAIL_SECTION_CODE = E.RAIL_SECTION_CODE ")
                sBuilder.AppendLine("               AND STATION_ORDER_CODE = E.STATION_ORDER_CODE ")
                sBuilder.AppendLine("               AND CORNER_CODE = E.CORNER_CODE  ")
                sBuilder.AppendLine("               AND MODEL_CODE = E.MODEL_CODE ")
                sBuilder.AppendLine("               AND UNIT_NO = E.UNIT_NO ")
                sBuilder.AppendLine("               ) THEN '1'  ")
                sBuilder.AppendLine("              ELSE '2' END ) ")
                sBuilder.AppendLine("    WHEN (S.SERIAL_NO <> E.SERIAL_NO AND E.KBN = '2' ) THEN  ")
                sBuilder.AppendLine("     ( ")
                sBuilder.AppendLine("      CASE  WHEN ( ")
                sBuilder.AppendLine("             SELECT  MIN (PROCESSING_TIME) ")
                sBuilder.AppendLine("             FROM D_HOSYU_DATA ")
                sBuilder.AppendLine("             WHERE SYU_SERIAL_NO = E.SERIAL_NO ")
                sBuilder.AppendLine("               AND RAIL_SECTION_CODE = E.RAIL_SECTION_CODE")
                sBuilder.AppendLine("               AND STATION_ORDER_CODE = E.STATION_ORDER_CODE ")
                sBuilder.AppendLine("               AND CORNER_CODE = E.CORNER_CODE  ")
                sBuilder.AppendLine("              AND MODEL_CODE = E.MODEL_CODE  ")
                sBuilder.AppendLine("              AND UNIT_NO = E.UNIT_NO  ")
                sBuilder.AppendLine("              ) >= (  ")
                sBuilder.AppendLine("               SELECT MIN (PROCESSING_TIME) ")
                sBuilder.AppendLine("               FROM  D_HOSYU_DATA  ")
                sBuilder.AppendLine("               WHERE SYU_SERIAL_NO = E.SERIAL_NO  ")
                sBuilder.AppendLine("               AND COLLECT_START_TIME = E.COLLECT_START_TIME ")
                sBuilder.AppendLine("               AND RAIL_SECTION_CODE = E.RAIL_SECTION_CODE ")
                sBuilder.AppendLine("               AND STATION_ORDER_CODE = E.STATION_ORDER_CODE ")
                sBuilder.AppendLine("               AND CORNER_CODE = E.CORNER_CODE  ")
                sBuilder.AppendLine("               AND MODEL_CODE = E.MODEL_CODE ")
                sBuilder.AppendLine("               AND UNIT_NO = E.UNIT_NO ")
                sBuilder.AppendLine("               ) THEN '1'  ")
                sBuilder.AppendLine("              ELSE '2' END ) ")
                sBuilder.AppendLine("              WHEN S.COLLECT_START_TIME <> E.COLLECT_START_TIME THEN '2'  ")
                sBuilder.AppendLine("              ELSE '0'  ")
                sBuilder.AppendLine("              END AS STS,  ")
            Else
                sBuilder.AppendLine("    CASE")
                sBuilder.AppendLine("        WHEN S.SERIAL_NO <> E.SERIAL_NO THEN '1'")
                sBuilder.AppendLine("        ELSE '0'")
                sBuilder.AppendLine("    END AS STS,")
            End If
            '-------Ver0.1�@�k���Ή��@ADD END----------
            '�@�@���������F�ڐݎ��́A�ڐ݌㒼�߂̏����������擾
            '  �@�@�@�@�@  �N���A���́A�N���A�㒼�߂̏����������擾
            sBuilder.AppendLine("    CASE")
            sBuilder.AppendLine("        WHEN (S.SERIAL_NO <> E.SERIAL_NO AND E.KBN = '1') THEN")
            sBuilder.AppendLine("        (")
            sBuilder.AppendLine("            SELECT")
            sBuilder.AppendLine("                MIN (PROCESSING_TIME)")
            sBuilder.AppendLine("            FROM")
            sBuilder.AppendLine(" " & sTragetTABLE)
            sBuilder.AppendLine("            WHERE")
            sBuilder.AppendLine("                KAI_SERIAL_NO = E.SERIAL_NO")
            '-------Ver0.1�@�k���Ή��@ADD START----------
            If optMente.Checked Then
                sBuilder.AppendLine("                AND COLLECT_START_TIME = E.COLLECT_START_TIME ")
            End If
            '-------Ver0.1�@�k���Ή��@ADD END----------
            sBuilder.AppendLine("            AND RAIL_SECTION_CODE = E.RAIL_SECTION_CODE")
            sBuilder.AppendLine("            AND STATION_ORDER_CODE = E.STATION_ORDER_CODE")
            sBuilder.AppendLine("            AND CORNER_CODE = E.CORNER_CODE AND MODEL_CODE = E.MODEL_CODE")
            sBuilder.AppendLine("            AND UNIT_NO = E.UNIT_NO")
            sBuilder.AppendLine("        )")
            sBuilder.AppendLine("        WHEN (S.SERIAL_NO <> E.SERIAL_NO AND E.KBN = '2') THEN")
            sBuilder.AppendLine("        (")
            sBuilder.AppendLine("            SELECT")
            sBuilder.AppendLine("                MIN (PROCESSING_TIME)")
            sBuilder.AppendLine("            FROM")
            sBuilder.AppendLine(" " & sTragetTABLE)
            sBuilder.AppendLine("            WHERE")
            sBuilder.AppendLine("                SYU_SERIAL_NO = E.SERIAL_NO")
            '-------Ver0.1�@�k���Ή��@ADD START----------
            If optMente.Checked Then
                sBuilder.AppendLine("                AND COLLECT_START_TIME = E.COLLECT_START_TIME ")
            End If
            '-------Ver0.1�@�k���Ή��@ADD END----------
            sBuilder.AppendLine("            AND RAIL_SECTION_CODE = E.RAIL_SECTION_CODE")
            sBuilder.AppendLine("            AND STATION_ORDER_CODE = E.STATION_ORDER_CODE")
            sBuilder.AppendLine("            AND CORNER_CODE = E.CORNER_CODE AND MODEL_CODE = E.MODEL_CODE")
            sBuilder.AppendLine("            AND UNIT_NO = E.UNIT_NO")
            sBuilder.AppendLine("        )")
            '�@�@�@�@�Q�F�i�N���A���j�́A�ێ�f�[�^�̂Ƃ��̂�
            If optMente.Checked Then
                '-------Ver0.1�@�k���Ή��@MOD START-----------
                If chkFromLastClear.Checked = False Then
                    sBuilder.AppendLine("        WHEN S.COLLECT_START_TIME <> E.COLLECT_START_TIME THEN")
                    sBuilder.AppendLine("        (")
                    sBuilder.AppendLine("        CASE ")
                    sBuilder.AppendLine("        WHEN (S.PROCESSING_TIME <= E.PROCESSING_TIME) THEN ")
                    sBuilder.AppendLine("        ( ")
                    sBuilder.AppendLine("            SELECT")
                    sBuilder.AppendLine("                MIN (PROCESSING_TIME)")
                    sBuilder.AppendLine("            FROM")
                    sBuilder.AppendLine(" " & sTragetTABLE)
                    sBuilder.AppendLine("            WHERE")
                    sBuilder.AppendLine("                COLLECT_START_TIME = E.COLLECT_START_TIME")
                    sBuilder.AppendLine("            AND RAIL_SECTION_CODE = E.RAIL_SECTION_CODE")
                    sBuilder.AppendLine("            AND STATION_ORDER_CODE = E.STATION_ORDER_CODE")
                    sBuilder.AppendLine("            AND CORNER_CODE = E.CORNER_CODE AND MODEL_CODE = E.MODEL_CODE")
                    sBuilder.AppendLine("            AND UNIT_NO = E.UNIT_NO")
                    sBuilder.AppendLine("        )ELSE S.PROCESSING_TIME ")
                    sBuilder.AppendLine("        END")
                    sBuilder.AppendLine("        )")
                End If
                '-------Ver0.1�@�k���Ή��@MOD END-----------
            End If
            sBuilder.AppendLine("        ELSE S.PROCESSING_TIME")
            sBuilder.AppendLine("    END AS S_PROCESSING_TIME,")
            sBuilder.AppendLine("    E.PROCESSING_TIME AS E_PROCESSING_TIME,")
            sBuilder.AppendLine("    E.COLLECT_START_TIME AS E_COLLECT_START_TIME,")
            sBuilder.AppendLine("    S.KENSYUU AS S_KENSYUU,E.KENSYUU AS E_KENSYUU,E.GROUP_NO ")
            sBuilder.AppendLine(" FROM")
            sBuilder.AppendLine("    (")
            '�@�@�J�n�����ɍ��v���郌�R�[�h�̒��o
            '�@�@�@�@�@��\���ɑ��݂���ғ����R�[�h�����D���A�W�D���̌X�ɒ��o
            '�@�@�@�@�@K_RANKING�F���C�i�荞�ݗp�����L���O����P�ʂ����o�ΏۂƂȂ�
            '�@�@�@�@�@P_RANKING�F���������i�荞�ݗp�����L���O����P�ʂ����o�ΏۂƂȂ�
            sBuilder.AppendLine("        SELECT")
            sBuilder.AppendLine("            S0.RAIL_SECTION_CODE,S0.STATION_ORDER_CODE,S0.CORNER_CODE,")
            sBuilder.AppendLine("            S0.MODEL_CODE,S0.UNIT_NO,KBN,SERIAL_NO,INSPECT_TIME,")
            sBuilder.AppendLine("            PROCESSING_TIME,COLLECT_START_TIME,KENSYUU,")
            sBuilder.AppendLine("            DENSE_RANK() over(partition by S0.RAIL_SECTION_CODE,")
            sBuilder.AppendLine("                S0.STATION_ORDER_CODE,S0.CORNER_CODE,S0.MODEL_CODE,")
            sBuilder.AppendLine("                S0.UNIT_NO,KBN")
            sBuilder.AppendLine("                order by KENSYUU DESC) AS K_RANKING,")
            sBuilder.AppendLine("            RANK() over(partition by S0.RAIL_SECTION_CODE,")
            sBuilder.AppendLine("                S0.STATION_ORDER_CODE,S0.CORNER_CODE,S0.MODEL_CODE,")
            sBuilder.AppendLine("                S0.UNIT_NO,KBN,KENSYUU")
            sBuilder.AppendLine("                order by PROCESSING_TIME) AS P_RANKING,MA.GROUP_NO")
            sBuilder.AppendLine("        FROM")
            sBuilder.AppendLine("            (")
            '�@�@�@�@���D���̃��R�[�h���o�FKENSYUU �͂���O���C
            sBuilder.AppendLine("                SELECT DISTINCT")
            sBuilder.AppendLine("                    RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                    MODEL_CODE,UNIT_NO,'1' AS KBN,KAI_SERIAL_NO AS SERIAL_NO,")
            sBuilder.AppendLine("                    KAI_INSPECT_TIME AS INSPECT_TIME,PROCESSING_TIME,")
            sBuilder.AppendLine("                    COLLECT_START_TIME,")
            sBuilder.AppendLine("                    DENSE_RANK() over(partition by RAIL_SECTION_CODE,")
            sBuilder.AppendLine("                        STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO")
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            If optMente.Checked And chkFromLastClear.Checked Then
                sBuilder.AppendLine("                        order by COLLECT_START_TIME ) AS KENSYUU")
            Else
                sBuilder.AppendLine("                        order by COLLECT_START_TIME DESC, KAI_INSPECT_TIME DESC) AS KENSYUU")
            End If
            '-------Ver0.1�@�k���Ή��@MOD END-----------
            sBuilder.AppendLine("                FROM")
            sBuilder.AppendLine(" " & sTragetTABLE)
            sBuilder.AppendLine("                WHERE")
            sBuilder.AppendLine("                    KAI_SERIAL_NO <> '0'")
            If LsFromDate <> "" Then
                sBuilder.AppendLine(String.Format(" AND PROCESSING_TIME >= {0}", Utility.SetSglQuot(LsFromDate & "000000")))
            End If
            If LsBaseSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
            End If
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            '�O��N���A�����I�����ꂽ�ꍇ
            If optMente.Checked And chkFromLastClear.Checked Then
                sBuilder.AppendLine("                    AND COLLECT_START_TIME <> '00000000000000'")
            End If
            '���C�񐔂��I�����ꂽ�ꍇ
            If LsFromKensyu <> "" Then
                sBuilder.AppendLine("                    AND KAI_INSPECT_TIME <> '00000000000000'")
            End If
            '-------Ver0.1�@�k���Ή��@MOD END-----------
            If sModel <> "" Then
                sBuilder.AppendLine(String.Format(" AND MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            End If
            If sModel <> "Y" Then
                sBuilder.AppendLine("                UNION")
                '�@�@�@�@�W�D���̃��R�[�h���o�FKENSYUU �͂���O���C
                sBuilder.AppendLine("                SELECT DISTINCT")
                sBuilder.AppendLine("                    RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                    MODEL_CODE,UNIT_NO,'2' AS KBN,SYU_SERIAL_NO AS SERIAL_NO,")
                sBuilder.AppendLine("                    SYU_INSPECT_TIME AS INSPECT_TIME,PROCESSING_TIME,")
                sBuilder.AppendLine("                    COLLECT_START_TIME,")
                sBuilder.AppendLine("                    DENSE_RANK() over(partition by RAIL_SECTION_CODE,")
                sBuilder.AppendLine("                        STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO")
                '-------Ver0.1�@�k���Ή��@MOD START-----------
                If optMente.Checked And chkFromLastClear.Checked Then
                    sBuilder.AppendLine("                        order by COLLECT_START_TIME) AS KENSYUU")
                Else
                    sBuilder.AppendLine("                        order by COLLECT_START_TIME DESC, SYU_INSPECT_TIME DESC) AS KENSYUU")
                End If
                sBuilder.AppendLine("                FROM")
                sBuilder.AppendLine(" " & sTragetTABLE)
                sBuilder.AppendLine("                WHERE")
                sBuilder.AppendLine("                    SYU_SERIAL_NO <> '0'")
                If LsFromDate <> "" Then
                    sBuilder.AppendLine(String.Format(" AND PROCESSING_TIME >= {0}", Utility.SetSglQuot(LsFromDate & "000000")))
                End If
                '-------Ver0.1�@�k���Ή��@MOD START-----------
                If optMente.Checked And chkFromLastClear.Checked Then
                    sBuilder.AppendLine("                    AND COLLECT_START_TIME <> '00000000000000'")
                End If
                If LsFromKensyu <> "" Then
                    sBuilder.AppendLine("                    AND SYU_INSPECT_TIME <> '00000000000000'")
                End If
                '-------Ver0.1�@�k���Ή��@MOD END-----------
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
                If sModel <> "" Then
                    sBuilder.AppendLine(String.Format(" AND MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                End If
            End If
            sBuilder.AppendLine("            ) AS S0,")
            sBuilder.AppendLine("            (")
            sBuilder.AppendLine("                SELECT")
            sBuilder.AppendLine("                    RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                    MODEL_CODE,UNIT_NO,GROUP_NO")
            sBuilder.AppendLine("                FROM")
            sBuilder.AppendLine("                    V_MACHINE_NOW ")
            If LsMacSQLWhere <> "" Then
                sBuilder.AppendLine(" WHERE " & LsMacSQLWhere)
                If sModel <> "" Then
                    sBuilder.AppendLine(String.Format(" AND MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                End If
            Else
                If sModel <> "" Then
                    sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                End If
            End If
            sBuilder.AppendLine("            ) AS MA")
            sBuilder.AppendLine("        WHERE")
            sBuilder.AppendLine("            MA.RAIL_SECTION_CODE = S0.RAIL_SECTION_CODE")
            sBuilder.AppendLine("        AND MA.STATION_ORDER_CODE = S0.STATION_ORDER_CODE")
            sBuilder.AppendLine("        AND MA.CORNER_CODE = S0.CORNER_CODE")
            sBuilder.AppendLine("        AND MA.MODEL_CODE = S0.MODEL_CODE AND MA.UNIT_NO = S0.UNIT_NO")
            If LsFromKensyu <> "" Then
                sBuilder.AppendLine(String.Format(" AND KENSYUU <= {0}", Utility.SetSglQuot(LsFromKensyu)))
            End If
            sBuilder.AppendLine("    ) AS S,")
            sBuilder.AppendLine("    (")
            '�@�@�I�������ɍ��v���郌�R�[�h�̒��o
            '�@�@�@�@�@��\���ɑ��݂���ғ����R�[�h�����D���A�W�D���̌X�ɒ��o
            '�@�@�@�@�@K_RANKING�F���C�i�荞�ݗp�����L���O����P�ʂ����o�ΏۂƂȂ�
            '�@�@�@�@�@P_RANKING�F���������i�荞�ݗp�����L���O����P�ʂ����o�ΏۂƂȂ�
            sBuilder.AppendLine("        SELECT")
            sBuilder.AppendLine("            E0.RAIL_SECTION_CODE,E0.STATION_ORDER_CODE,E0.CORNER_CODE,")
            sBuilder.AppendLine("            E0.MODEL_CODE,E0.UNIT_NO,KBN,SERIAL_NO,INSPECT_TIME,")
            sBuilder.AppendLine("            PROCESSING_TIME,COLLECT_START_TIME,KENSYUU,")
            sBuilder.AppendLine("            DENSE_RANK() over(partition by E0.RAIL_SECTION_CODE,")
            sBuilder.AppendLine("                E0.STATION_ORDER_CODE,E0.CORNER_CODE,E0.MODEL_CODE,")
            sBuilder.AppendLine("                E0.UNIT_NO,KBN")
            sBuilder.AppendLine("                order by KENSYUU) AS K_RANKING,")
            sBuilder.AppendLine("            RANK() over(partition by E0.RAIL_SECTION_CODE,")
            sBuilder.AppendLine("                E0.STATION_ORDER_CODE,E0.CORNER_CODE,E0.MODEL_CODE,")
            sBuilder.AppendLine("                E0.UNIT_NO,KBN,KENSYUU")
            sBuilder.AppendLine("                order by PROCESSING_TIME DESC) AS P_RANKING,MA.GROUP_NO")
            sBuilder.AppendLine("        FROM")
            sBuilder.AppendLine("            (")
            '�@�@�@�@���D���̃��R�[�h���o�FKENSYUU �͂���O���C
            sBuilder.AppendLine("                SELECT DISTINCT")
            sBuilder.AppendLine("                    RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                    MODEL_CODE,UNIT_NO,'1' AS KBN,KAI_SERIAL_NO AS SERIAL_NO,")
            sBuilder.AppendLine("                    KAI_INSPECT_TIME AS INSPECT_TIME,PROCESSING_TIME,")
            sBuilder.AppendLine("                    COLLECT_START_TIME,")
            sBuilder.AppendLine("                    DENSE_RANK() over(partition by RAIL_SECTION_CODE,")
            sBuilder.AppendLine("                        STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO")
            sBuilder.AppendLine("                        order by COLLECT_START_TIME DESC, KAI_INSPECT_TIME DESC) AS KENSYUU")
            sBuilder.AppendLine("                FROM")
            sBuilder.AppendLine(" " & sTragetTABLE)
            sBuilder.AppendLine("                WHERE")
            sBuilder.AppendLine("                    KAI_SERIAL_NO <> '0'")
            If LsToDate <> "" Then
                sBuilder.AppendLine(String.Format(" AND PROCESSING_TIME <= {0}", Utility.SetSglQuot(LsToDate & "235959")))
            End If
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            If LsToKensyu <> "" Then
                sBuilder.AppendLine(String.Format(" AND KAI_INSPECT_TIME <> '00000000000000'"))
            End If
            '-------Ver0.1�@�k���Ή��@MOD END-----------
            If LsBaseSQLWhere <> "" Then
                sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
            End If
            If sModel <> "" Then
                sBuilder.AppendLine(String.Format(" AND MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
            End If
            If sModel <> "Y" Then
                sBuilder.AppendLine("                UNION")
                '�@�@�@�@�W�D���̃��R�[�h���o�FKENSYUU �͂���O���C
                sBuilder.AppendLine("                SELECT DISTINCT")
                sBuilder.AppendLine("                    RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
                sBuilder.AppendLine("                    MODEL_CODE,UNIT_NO,'2' AS KBN,SYU_SERIAL_NO AS SERIAL_NO,")
                sBuilder.AppendLine("                    SYU_INSPECT_TIME AS INSPECT_TIME,PROCESSING_TIME,")
                sBuilder.AppendLine("                    COLLECT_START_TIME,")
                sBuilder.AppendLine("                    DENSE_RANK() over(partition by RAIL_SECTION_CODE,")
                sBuilder.AppendLine("                        STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO")
                sBuilder.AppendLine("                        order by COLLECT_START_TIME DESC, SYU_INSPECT_TIME DESC) AS KENSYUU")
                sBuilder.AppendLine("                FROM")
                sBuilder.AppendLine(" " & sTragetTABLE)
                sBuilder.AppendLine("                WHERE")
                sBuilder.AppendLine("                    SYU_SERIAL_NO <> '0'")
                If LsToDate <> "" Then
                    sBuilder.AppendLine(String.Format(" AND PROCESSING_TIME <= {0}", Utility.SetSglQuot(LsToDate & "235959")))
                End If
                '-------Ver0.1�@�k���Ή��@MOD START-----------
                If LsToKensyu <> "" Then
                    sBuilder.AppendLine(String.Format(" AND SYU_INSPECT_TIME <> '00000000000000'"))
                End If
                '-------Ver0.1�@�k���Ή��@MOD END-----------
                If LsBaseSQLWhere <> "" Then
                    sBuilder.AppendLine(" AND " & LsBaseSQLWhere)
                End If
                If sModel <> "" Then
                    sBuilder.AppendLine(String.Format(" AND MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                End If
            End If
            sBuilder.AppendLine("            ) AS E0,")
            sBuilder.AppendLine("            (")
            sBuilder.AppendLine("                SELECT")
            sBuilder.AppendLine("                    RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,")
            sBuilder.AppendLine("                    MODEL_CODE,UNIT_NO,GROUP_NO")
            sBuilder.AppendLine("                FROM")
            sBuilder.AppendLine("                    V_MACHINE_NOW ")
            If LsMacSQLWhere <> "" Then
                sBuilder.AppendLine(" WHERE " & LsMacSQLWhere)
                If sModel <> "" Then
                    sBuilder.AppendLine(String.Format(" AND MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                End If
            Else
                If sModel <> "" Then
                    sBuilder.AppendLine(String.Format(" WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModel)))
                End If
            End If
            sBuilder.AppendLine("            ) AS MA")
            sBuilder.AppendLine("        WHERE")
            sBuilder.AppendLine("            MA.RAIL_SECTION_CODE = E0.RAIL_SECTION_CODE")
            sBuilder.AppendLine("        AND MA.STATION_ORDER_CODE = E0.STATION_ORDER_CODE")
            sBuilder.AppendLine("        AND MA.CORNER_CODE = E0.CORNER_CODE")
            sBuilder.AppendLine("        AND MA.MODEL_CODE = E0.MODEL_CODE AND MA.UNIT_NO = E0.UNIT_NO")
            If LsToKensyu <> "" Then
                sBuilder.AppendLine(String.Format(" AND KENSYUU >= {0}", Utility.SetSglQuot(LsToKensyu)))
            End If
            sBuilder.AppendLine("    ) AS E")
            sBuilder.AppendLine(" WHERE")
            sBuilder.AppendLine("    S.RAIL_SECTION_CODE = E.RAIL_SECTION_CODE")
            sBuilder.AppendLine(" AND S.STATION_ORDER_CODE = E.STATION_ORDER_CODE")
            sBuilder.AppendLine(" AND S.CORNER_CODE = E.CORNER_CODE AND S.MODEL_CODE = E.MODEL_CODE")
            sBuilder.AppendLine(" AND S.UNIT_NO = E.UNIT_NO AND S.KBN = E.KBN AND S.K_RANKING = '1'")
            sBuilder.AppendLine(" AND E.K_RANKING = '1' AND S.P_RANKING = '1' AND E.P_RANKING = '1'")
        End If
        sSQL = sBuilder.ToString()

        Return sSQL

    End Function

    ''' <summary>
    ''' [�o�͏���]
    ''' </summary>
    Private Sub LfXlsStart_KadoHosyuG()
        Dim sTemplateSheet As String = ""
        Dim sFrom As String = ""
        Dim sTo As String = ""
        Dim sSheet As String = ""
        Dim Kai_Array As Double()
        Dim Syu_Array As Double()
        Kai_Array = New Double(LdtTarget.Columns.Count) {}
        Syu_Array = New Double(LdtTarget.Columns.Count) {}

        Dim nCnt As Integer = 0

        Dim Flg_Kai As Boolean
        Dim Flg_Syu As Boolean
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 11
        '-------Ver0.1�@�k���Ή��@ADD START-----------
        Dim Print_Kai_S As Boolean = True
        Dim Print_Kai_E As Boolean = True
        Dim Print_Syu_S As Boolean = True
        Dim Print_Syu_E As Boolean = True
        Dim Print_Chk_F As Boolean = False
        '-------Ver0.1�@�k���Ή��@ADD END-----------
        Try
            '-------Ver0.1�@�k���Ή��@MOD START-----------
            If optKado.Checked Then
                sTemplateSheet = Config.KadoPrintListK(GrpNo).ToString.Substring(0, Config.KadoPrintListK(GrpNo).ToString.Length - 4)
            Else
                sTemplateSheet = Config.KadoPrintListH(GrpNo).ToString.Substring(0, Config.KadoPrintListH(GrpNo).ToString.Length - 4)
            End If
            '-------Ver0.1�@�k���Ή��@MOD END-----------
            With XlsReport1
                Log.Info("Start printing about [" & LsXlsTemplatePath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = LsXlsTemplatePath
                .ExcelMode = True
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()

                sSheet = ""
                For Rec As Integer = 0 To LdtTarget.Rows.Count - 1
                    '-------Ver0.1�@�k���Ή��@ADD START-----------
                    '�o�͑Ώ۔���t���O������������
                    Print_Kai_E = True
                    Print_Kai_S = True
                    Print_Syu_S = True
                    Print_Syu_E = True
                    Print_Chk_F = False
                    '-------Ver0.1�@�k���Ή��@ADD END-----------
                    '���o�f�[�^�ɐ���R�[�h�A�w���R�[�h�A�R�[�i�[�R�[�h������Έȉ��̏���
                    If LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() & LdtTarget.Rows(Rec)(4).ToString() <> "" Then
                        '�L�[�u���[�N�F����R�[�h�A�w���R�[�h�A�R�[�i�[�R�[�h���ς��Ή��y�[�W
                        If sSheet <> LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() & LdtTarget.Rows(Rec)(4).ToString() Then
                            If sSheet <> "" Then
                                .Page.End()
                            End If
                            sSheet = LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() & LdtTarget.Rows(Rec)(4).ToString()

                            '���[�t�@�C���V�[�g���̂��擾���܂��B
                            .Page.Start(sTemplateSheet, "1-9999")
                            .Pos(3, 4, 5, nStartRow + LdtKoumoku.Rows.Count - 1).Copy()
                            .Page.Name = LdtTarget.Rows(Rec)(0).ToString() & "�@" & LdtTarget.Rows(Rec)(1).ToString()

                            ' ���o�����Z���֌��o���f�[�^�o��
                            .Cell("O1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                            .Cell("O2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                            .Cell("A3").Value = OPMGFormConstants.STATION_NAME + LdtTarget.Rows(Rec)(0).ToString() + "�@�@�@" _
                            + OPMGFormConstants.CORNER_STR + LdtTarget.Rows(Rec)(1).ToString() _
                            + "�@�@" + OPMGFormConstants.EQUIPMENT_TYPE + "���D�@"

                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                            If chkFromLastClear.Checked Then
                                sFrom = "�O��N���A��"
                            Else
                                If optFromDate.Checked Then
                                    sFrom = Replace(Replace(Replace(dtpYmdFrom.Text, "�N", "/"), "��", "/"), "��", "")
                                ElseIf optFromKensyu.Checked Then
                                    sFrom = cmbKensyuFrom.Text.Trim
                                    .Cell("A6").Value = "���C���i" & sFrom & "�j"
                                End If
                            End If
                            If chkToYesterday.Checked Then
                                If LsToDate <> "" Then
                                    sTo = (DateTime.ParseExact(LsToDate, "yyyyMMdd", Nothing).AddDays(-1)).ToString("yyyy/MM/dd") & "�i�O���j"
                                Else
                                    sTo = DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd") & "�i�O���j"
                                End If
                            Else
                                If optToDate.Checked Then
                                    sTo = Replace(Replace(Replace(dtpYmdTo.Text, "�N", "/"), "��", "/"), "��", "")
                                ElseIf optToKensyu.Checked Then
                                    sTo = cmbKensyuTo.Text.Trim
                                    .Cell("A7").Value = "���C���i" & sTo & "�j"

                                End If
                            End If
                            '-------Ver0.1�@�k���Ή��@MOD�@END-----------
                            .Cell("A4").Value = "      " & Lexis.TimeSpan.Gen(sFrom, "", sTo, "")

                            ' ���ږ��́A��l���Z�b�g
                            For i As Integer = 0 To LdtKoumoku.Rows.Count - 1
                                .Pos(0, i + nStartRow).Value = LdtKoumoku.Rows(i)(0).ToString()
                                '-------Ver0.1�@�k���Ή��@MOD START-----------
                                If Double.Parse(LdtKoumoku.Rows(i)(3).ToString) <> 1 Then
                                    .Pos(1, i + nStartRow).Value = Double.Parse(LdtKoumoku.Rows(i)(1).ToString)
                                    .Pos(2, i + nStartRow).Value = Double.Parse(LdtKoumoku.Rows(i)(2).ToString)
                                End If
                                '-------Ver0.1�@�k���Ή��@MOD   END-----------
                            Next

                            nCnt = 0
                        End If

                        Flg_Kai = False
                        Flg_Syu = False
                        Array.Clear(Kai_Array, 0, Kai_Array.Length)
                        Array.Clear(Syu_Array, 0, Syu_Array.Length)

                        .Pos(nCnt + 3, 4).Paste()
                        .Pos(nCnt + 3, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "��"
                        .Pos(nCnt + 3 + 1, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "�W"
                        .Pos(nCnt + 3 + 2, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "��"
                        If LdtTarget.Rows(Rec)(9).ToString() = "" Then
                            '���@�P�ʂŉғ��f�[�^�����������Ƃ�
                            nCnt = nCnt + 3
                        Else
                            '-------Ver0.1�@�k���Ή��@ADD START---------
                            If LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) > LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                Print_Chk_F = True
                            End If
                            '-------Ver0.1�@�k���Ή��@ADD END-----------
                            '���D�����R�[�h�ҏW
                            If (LdtTarget.Rows(Rec)(14).ToString() = "0") And (LdtTarget.Rows(Rec)(15).ToString() = "1") Then
                                '������No
                                .Pos(nCnt + 3, 4).Value = LdtTarget.Rows(Rec)(18).ToString()
                                '�J�n���t�Z�b�g
                                If chkFromLastClear.Checked Then
                                    If LdtTarget.Rows(Rec)(7).ToString = "1" Then
                                        '�N���A������i�ڐ݁j
                                        .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8)), "0000/00/00")
                                    Else
                                        '�N���A�����`�F�b�N���ꂽ�ꍇ
                                        .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(19).ToString.Substring(0, 8)), "0000/00/00")
                                    End If
                                Else
                                    If LdtTarget.Rows(Rec)(7).ToString = "0" Then
                                        If LsFromDate <> "" Then
                                            '�o�͓��i�J�n���j
                                            If LsFromDate.Substring(0, 8) <> LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) Then
                                                .Pos(nCnt + 3, 8).Value = Format(CInt(LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8)), "0000/00/00")
                                            End If
                                        Else
                                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                                            '���C�񐔎w�肵���ꍇ�A
                                            If LsFromKensyu <> "" Then
                                                '�����I�����i����O�j
                                                '�w�肵���񐔂ɊY������f�[�^���Ȃ��ꍇ
                                                If LsFromKensyu <> LdtTarget.Rows(Rec)(8).ToString() Then
                                                    '���D���o�͊J�n�f�[�^���o�͑ΏۊO�ƃZ�b�g
                                                    Print_Kai_S = False
                                                Else
                                                    '�w�肵���񐔂����݂���ꍇ�A�_���������C���i�J�n�j�ɃZ�b�g
                                                    .Pos(nCnt + 3, 5).Value = Format(CInt(LdtTarget.Rows(Rec)(20).ToString.Substring(0, 8)), "0000/00/00")
                                                End If
                                            End If
                                            '-------Ver0.1�@�k���Ή��@MOD END-----------
                                        End If
                                    ElseIf LdtTarget.Rows(Rec)(7).ToString = "1" Then
                                        '�N���A������i�ڐ݁j
                                        .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8)), "0000/00/00")
                                        If LiOutPutSTS < 4 Then
                                            LiOutPutSTS = 4
                                        End If
                                    ElseIf LdtTarget.Rows(Rec)(7).ToString = "2" Or chkFromLastClear.Checked Then
                                        '�N���A������
                                        .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(19).ToString.Substring(0, 8)), "0000/00/00")
                                        If LiOutPutSTS < 5 Then
                                            LiOutPutSTS = 5
                                        End If
                                    End If
                                End If
                                '�I�����t�Z�b�g
                                If LsToDate <> "" Then
                                    '�o�͓��i�I�����j
                                    If LsToDate.Substring(0, 8) <> LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                        '�J�n�I���������t�]�ł͂Ȃ��ꍇ
                                        If Print_Chk_F = False Then
                                            .Pos(nCnt + 3, 9).Value = Format(CInt(LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8)), "0000/00/00")
                                        End If
                                    End If
                                Else
                                    '-------Ver0.1�@�k���Ή��@MOD START-----------
                                    '���C�񐔂��w�肵���ꍇ
                                    If LsToKensyu <> "" Then
                                        '�����I�����i����O�j
                                        '�w�肵���񐔂ɊY������f�[�^���Ȃ��ꍇ
                                        If LsToKensyu <> LdtTarget.Rows(Rec + 1)(8).ToString() Then
                                            '���D���o�͏I���f�[�^���o�͑ΏۊO�ƃZ�b�g
                                            Print_Kai_E = False
                                        Else
                                            '�J�n�I���������t�]�ł͂Ȃ��ꍇ
                                            If Print_Chk_F = False Then
                                                '�_���������C���i�I���j�ɃZ�b�g
                                                .Pos(nCnt + 3, 6).Value = Format(CInt(LdtTarget.Rows(Rec + 1)(20).ToString.Substring(0, 8)), "0000/00/00")
                                            End If
                                        End If
                                    End If
                                    '-------Ver0.1�@�k���Ή��@MOD END-----------
                                End If
                                '-------Ver0.1�@�k���Ή��@MOD START-----------
                                If Print_Kai_S = True And Print_Kai_E = True Then
                                    If LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) = LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                        '�J�n�`�I����������i�w�i�ԐF�\���j
                                        .Pos(nCnt + 3, nStartRow, nCnt + 3, nStartRow + LdtTarget.Columns.Count - 22).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Red
                                        If LiOutPutSTS < 2 Then
                                            LiOutPutSTS = 2
                                        End If
                                        Rec = Rec + 2
                                    ElseIf LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) > LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                        '�J�n�`�I���̓��t���t�]
                                        If LiOutPutSTS < 3 Then
                                            LiOutPutSTS = 3
                                        End If
                                        Rec = Rec + 2
                                    Else
                                        '�ғ��f�[�^�Z�o���o��
                                        '-------Ver0.1�@�k���Ή��@MOD START-----------
                                        For x As Integer = 21 To LdtTarget.Columns.Count - 1
                                            If LdtTarget.Rows(Rec)(x).ToString <> "" Then
                                                '-------Ver0.1�@�k���Ή��@MOD START-----------
                                                '�Z���T�[���x���̏ꍇ�͍����v�Z���Ȃ�
                                                If Double.Parse(LdtKoumoku.Rows(x - 21)(3).ToString) = 3 Then
                                                    Kai_Array(x - 21) = Double.Parse(LdtTarget.Rows(Rec + 1)(x).ToString)
                                                    .Pos(nCnt + 3, nStartRow + x - 21).Value = Kai_Array(x - 21)
                                                Else
                                                    '�Z���T�[���x���ȊO�̏ꍇ�͍����v�Z�����l���o�͂���
                                                    Kai_Array(x - 21) = Double.Parse(LdtTarget.Rows(Rec + 1)(x).ToString) - Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                                    .Pos(nCnt + 3, nStartRow + x - 21).Value = Kai_Array(x - 21)

                                                End If
                                                If Double.Parse(LdtKoumoku.Rows(x - 21)(3).ToString) <> 1 Then
                                                    If Double.Parse(LdtKoumoku.Rows(x - 21)(1).ToString) > 0 Then
                                                        If Double.Parse(LdtKoumoku.Rows(x - 21)(3).ToString) = 0 Then
                                                            '��l���傫����Δw�i�F���D�F
                                                            If Double.Parse(LdtKoumoku.Rows(x - 21)(1).ToString) < Kai_Array(x - 21) Then
                                                                .Pos(nCnt + 3, nStartRow + x - 21).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                            End If
                                                        Else

                                                            '�Z���T�[���x���͊�l��菬������Δw�i�F���D�F
                                                            If Double.Parse(LdtKoumoku.Rows(x - 21)(1).ToString) > Kai_Array(x - 21) Then
                                                                .Pos(nCnt + 3, nStartRow + x - 21).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                                '-------Ver0.1�@�k���Ή��@MOD END-----------
                                            End If
                                        Next
                                        Rec = Rec + 2
                                        Flg_Kai = True
                                    End If
                                Else
                                    Rec = Rec + 2
                                End If
                                '-------Ver0.1�@�k���Ή��@MOD END-----------
                            End If
                            nCnt = nCnt + 1
                            '-------Ver0.1�@�k���Ή��@ADD START---------
                            '�o�͑Ώۃ`�F�b�N�t���O��������
                            Print_Chk_F = False
                            '�������t���t�]�`�F�b�N
                            If LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) > LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                Print_Chk_F = True
                            End If
                            '-------Ver0.1�@�k���Ή��@ADD END-----------

                            '�W�D�����R�[�h�ҏW
                            If (LdtTarget.Rows(Rec)(14).ToString() = "1") And (LdtTarget.Rows(Rec)(15).ToString() = "2") Then
                                '������No
                                .Pos(nCnt + 3, 4).Value = LdtTarget.Rows(Rec)(18).ToString()
                                '�J�n���t�Z�b�g
                                If chkFromLastClear.Checked Then
                                    If LdtTarget.Rows(Rec)(7).ToString = "1" Then
                                        '�N���A������i�ڐ݁j
                                        .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8)), "0000/00/00")
                                    Else
                                        '�N���A�����`�F�b�N���ꂽ�ꍇ
                                        .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(19).ToString.Substring(0, 8)), "0000/00/00")
                                    End If
                                Else
                                    If LdtTarget.Rows(Rec)(7).ToString = "0" Then
                                        If LsFromDate <> "" Then
                                            '�o�͓��i�J�n���j
                                            If LsFromDate.Substring(0, 8) <> LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) Then
                                                .Pos(nCnt + 3, 8).Value = Format(CInt(LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8)), "0000/00/00")
                                            End If
                                        Else
                                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                                            '���C�񐔂��w�肵���ꍇ
                                            If LsFromKensyu <> "" Then
                                                '�����J�n���i����O�j
                                                '�w�肵�����C�񐔂����݂��Ȃ��ꍇ
                                                If LsFromKensyu <> LdtTarget.Rows(Rec)(8).ToString() Then
                                                    '�W�D���o�͊J�n�f�[�^���o�͑ΏۊO�ƃZ�b�g
                                                    Print_Syu_S = False
                                                Else
                                                    '���C���i�J�n�j�ɓ_�������Z�b�g
                                                    .Pos(nCnt + 3, 5).Value = Format(CInt(LdtTarget.Rows(Rec)(20).ToString.Substring(0, 8)), "0000/00/00")
                                                End If
                                            End If
                                            '-------Ver0.1�@�k���Ή��@MOD END-----------
                                        End If
                                    ElseIf LdtTarget.Rows(Rec)(7).ToString = "1" Then
                                        '�N���A������i�ڐ݁j
                                        .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8)), "0000/00/00")
                                        If LiOutPutSTS < 4 Then
                                            LiOutPutSTS = 4
                                        End If
                                    ElseIf LdtTarget.Rows(Rec)(7).ToString = "2" Then
                                        '�N���A������
                                        .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(19).ToString.Substring(0, 8)), "0000/00/00")
                                        If LiOutPutSTS < 5 Then
                                            LiOutPutSTS = 5
                                        End If
                                    End If
                                End If
                                    '�I�����t�Z�b�g
                                    If LsToDate <> "" Then
                                        '�o�͓��i�I�����j
                                        If LsToDate.Substring(0, 8) <> LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                            '�J�n�I���������t�]�ł͂Ȃ��ꍇ
                                            If Print_Chk_F = False Then
                                                .Pos(nCnt + 3, 9).Value = Format(CInt(LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8)), "0000/00/00")
                                            End If
                                        End If
                                    Else
                                    '-------Ver0.1�@�k���Ή��@MOD START-----------
                                        '���C�񐔂��w�肵���ꍇ
                                        If LsToKensyu <> "" Then
                                            '�����I�����i����O�j
                                            '�w�肵�����C�񐔂����݂��Ȃ��ꍇ
                                            If LsToKensyu <> LdtTarget.Rows(Rec + 1)(8).ToString() Then
                                                '�W�D���o�͏I���f�[�^���o�͑ΏۊO�ƃZ�b�g
                                                Print_Syu_E = False
                                            Else
                                                '�J�n�I���������t�]�ł͂Ȃ��ꍇ
                                                If Print_Chk_F = False Then
                                                    '���C���i�I���j�ɓ_�������Z�b�g
                                                    .Pos(nCnt + 3, 6).Value = Format(CInt(LdtTarget.Rows(Rec + 1)(20).ToString.Substring(0, 8)), "0000/00/00")
                                                End If
                                            End If
                                        '-------Ver0.1�@�k���Ή��@MOD END-----------
                                        End If
                                    End If
                                '-------Ver0.1�@�k���Ή��@MOD START-----------
                                    If Print_Syu_S = True And Print_Syu_E = True Then
                                        If LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) = LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                            '�w�i�ԐF�\��
                                            .Pos(nCnt + 3, nStartRow, nCnt + 3, nStartRow + LdtTarget.Columns.Count - 22).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Red
                                            If LiOutPutSTS < 2 Then
                                                LiOutPutSTS = 2
                                            End If
                                            Rec = Rec + 2
                                        ElseIf LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) > LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                            '�J�n�`�I���̓��t���t�]
                                            If LiOutPutSTS < 3 Then
                                                LiOutPutSTS = 3
                                            End If
                                            Rec = Rec + 2
                                        Else
                                            '�ғ��f�[�^�Z�o���o��
                                            For x As Integer = 21 To LdtTarget.Columns.Count - 1
                                                If LdtTarget.Rows(Rec)(x).ToString <> "" Then
                                                    '-------Ver0.1�@�k���Ή��@MOD START-----------
                                                    '�Z���T�[���x���̏ꍇ�͍����v�Z���Ȃ�
                                                    If Double.Parse(LdtKoumoku.Rows(x - 21)(3).ToString) = 3 Then
                                                        Syu_Array(x - 21) = Double.Parse(LdtTarget.Rows(Rec + 1)(x).ToString)
                                                        .Pos(nCnt + 3, nStartRow + x - 21).Value = Syu_Array(x - 21)
                                                    Else
                                                        '�Z���T�[���x���ȊO�̏ꍇ�͍����v�Z�����l���o�͂���
                                                        Syu_Array(x - 21) = Double.Parse(LdtTarget.Rows(Rec + 1)(x).ToString) - Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                                        .Pos(nCnt + 3, nStartRow + x - 21).Value = Syu_Array(x - 21)
                                                    End If
                                                    If Double.Parse(LdtKoumoku.Rows(x - 21)(3).ToString) <> 1 Then
                                                        If Double.Parse(LdtKoumoku.Rows(x - 21)(2).ToString) > 0 Then
                                                            If Double.Parse(LdtKoumoku.Rows(x - 21)(3).ToString) = 0 Then
                                                                '��l���傫����Δw�i�F���D�F
                                                                If Double.Parse(LdtKoumoku.Rows(x - 21)(2).ToString) < Syu_Array(x - 21) Then
                                                                    .Pos(nCnt + 3, nStartRow + x - 21).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                                End If
                                                            Else
                                                                '�Z���T�[���x���͊�l��菬������Δw�i�F���D�F
                                                                If Double.Parse(LdtKoumoku.Rows(x - 21)(2).ToString) > Syu_Array(x - 21) Then
                                                                    .Pos(nCnt + 3, nStartRow + x - 21).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                    '-------Ver0.1�@�k���Ή��@MOD END-----------
                                                End If
                                            Next
                                            Rec = Rec + 2
                                            Flg_Syu = True
                                        End If
                                    Else
                                        Rec = Rec + 2
                                    End If
                                '-------Ver0.1�@�k���Ή��@MOD END-----------
                                End If
                            nCnt = nCnt + 1

                            '���v���R�[�h�ҏW
                            If Flg_Kai Or Flg_Syu Then
                                For x As Integer = 21 To LdtTarget.Columns.Count - 1
                                    If LdtKoumoku.Rows(x - 21)(3).ToString = "0" Or LdtKoumoku.Rows(x - 21)(3).ToString = "1" Then
                                        If LdtTarget.Rows(Rec)(x).ToString <> "" Then
                                            .Pos(nCnt + 3, nStartRow + x - 21).Value = Double.Parse(LdtTarget.Rows(Rec + 1)(x).ToString) - Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                            .Pos(nCnt + 1, nStartRow + x - 21).Value = Double.Parse(LdtTarget.Rows(Rec + 1)(x).ToString) - Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                        Else
                                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                                            If Print_Kai_S = True And Print_Kai_E = True And Print_Syu_S = True And Print_Syu_E = True Then
                                                .Pos(nCnt + 3, nStartRow + x - 21).Value = Kai_Array(x - 21) + Syu_Array(x - 21)
                                            ElseIf Print_Kai_S = False Or Print_Kai_E = False Then
                                                .Pos(nCnt + 3, nStartRow + x - 21).Value = Syu_Array(x - 21)
                                            ElseIf Print_Syu_S = False Or Print_Syu_E = False Then
                                                .Pos(nCnt + 3, nStartRow + x - 21).Value = Kai_Array(x - 21)
                                            End If
                                            '-------Ver0.1�@�k���Ή��@MOD END-----------
                                        End If
                                    End If
                                Next
                            End If
                            If CStr(.Pos(nCnt + 1, 4).Value) <> "" And CStr(.Pos(nCnt + 2, 4).Value) <> "" Then
                                Rec = Rec + 3
                            Else
                                Rec = Rec + 1
                            End If
                            nCnt = nCnt + 1
                        End If
                    End If
                Next

                .Page.End()
                .Report.End()

                ' ���[�̃v���r���[�����[�_���_�C�A���O�ŋN�����܂��B
                PrintViewer.GetDocument(XlsReport1.Document, sTemplateSheet)
                PrintViewer.ShowDialog(Me)
                PrintViewer.Dispose()
                Log.Info("Printing finished.")
            End With
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

    Private Sub LfXlsStart_KadoY()
        Dim sTemplateSheet As String = ""
        Dim sFrom As String = ""
        Dim sTo As String = ""
        Dim sSheet As String = ""
        Dim lArray As Double()
        lArray = New Double(LdtTarget.Columns.Count) {}

        Dim nCnt As Integer = 0

        Dim Flg_Kai As Boolean
        Dim Flg_Syu As Boolean
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 12
        '-------Ver0.1�@�k���Ή��@ADD START-----------
        Dim Print_Kai_S As Boolean = True
        Dim Print_Kai_E As Boolean = True
        Dim Print_Chk_F As Boolean = False
        '-------Ver0.1�@�k���Ή��@ADD END-----------
        Try
            sTemplateSheet = "�ғ��f�[�^"

            With XlsReport1
                Log.Info("Start printing about [" & LsXlsTemplatePath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = LsXlsTemplatePath
                .ExcelMode = True
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()

                sSheet = ""
                For Rec As Integer = 0 To LdtTarget.Rows.Count - 1
                    '-------Ver0.1�@�k���Ή��@ADD START-----------
                    '�o�͑Ώ۔���t���O������������
                    Print_Kai_E = True
                    Print_Kai_S = True
                    Print_Chk_F = False
                    '-------Ver0.1�@�k���Ή��@ADD END-----------
                    '���o�f�[�^�ɐ���R�[�h�A�w���R�[�h������Έȉ��̏���
                    If LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() <> "" Then
                        '�L�[�u���[�N�F����R�[�h�A�w���R�[�h�A�R�[�i�[�R�[�h���ς��Ή��y�[�W
                        If sSheet <> LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() Then
                            If sSheet <> "" Then
                                .Page.End()
                            End If
                            sSheet = LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString()

                            '���[�t�@�C���V�[�g���̂��擾���܂��B
                            .Page.Start(sTemplateSheet, "1-9999")
                            '.Cell("C5:C237").Copy()
                            .Pos(2, 4, 2, nStartRow + LdtKoumoku.Rows.Count - 1).Copy()
                            .Page.Name = LdtTarget.Rows(Rec)(0).ToString()

                            ' ���o�����Z���֌��o���f�[�^�o��
                            .Cell("O1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                            .Cell("O2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                            .Cell("A3").Value = OPMGFormConstants.STATION_NAME + LdtTarget.Rows(Rec)(0).ToString() + "�@�@�@" _
                            + OPMGFormConstants.EQUIPMENT_TYPE + "���������@"
                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                            '�O���N���A����I�������ꍇ�A�J�n�����Ɂh�O���N���A���h���o�͂���
                            If chkFromLastClear.Checked Then
                                sFrom = "�O��N���A��"
                            Else
                                If optFromDate.Checked Then
                                    sFrom = Replace(Replace(Replace(dtpYmdFrom.Text, "�N", "/"), "��", "/"), "��", "")
                                ElseIf optFromKensyu.Checked Then
                                    sFrom = cmbKensyuFrom.Text.Trim
                                    .Cell("A7").Value = "���C���i" & sFrom & "�j"
                                End If
                            End If
                            '�O����I�������ꍇ�A�I�������Ɂh�V�X�e�����t�̑O���{�i�O���j�h���o�͂���
                            If chkToYesterday.Checked Then
                                If LsToDate <> "" Then
                                    sTo = (DateTime.ParseExact(LsToDate, "yyyyMMdd", Nothing).AddDays(-1)).ToString("yyyy/MM/dd") & "�i�O���j"
                                Else
                                    sTo = DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd") & "�i�O���j"
                                End If
                            Else
                                If optToDate.Checked Then
                                    sTo = Replace(Replace(Replace(dtpYmdTo.Text, "�N", "/"), "��", "/"), "��", "")
                                ElseIf optToKensyu.Checked Then
                                    sTo = cmbKensyuTo.Text.Trim
                                    .Cell("A8").Value = "���C���i" & sTo & "�j"
                                Else
                                    sTo = "�O��"
                                End If
                            End If
                            '-------Ver0.1�@�k���Ή��@MOD �@END-----------
                            .Cell("A4").Value = "      " & Lexis.TimeSpan.Gen(sFrom, "", sTo, "")

                            ' ���ږ��́A��l���Z�b�g
                            For i As Integer = 0 To LdtKoumoku.Rows.Count - 1
                                .Pos(0, i + nStartRow).Value = LdtKoumoku.Rows(i)(0).ToString()
                                .Pos(1, i + nStartRow).Value = Double.Parse(LdtKoumoku.Rows(i)(1).ToString)
                            Next

                            nCnt = 0
                        End If

                        Flg_Kai = False
                        Flg_Syu = False
                        Array.Clear(lArray, 0, lArray.Length)

                        .Pos(nCnt + 2, 4).Paste()
                        .Pos(nCnt + 2, 11).Value = LdtTarget.Rows(Rec)(6).ToString()
                        '�R�[�i�[
                        .Pos(nCnt + 2, 4).Value = LdtTarget.Rows(Rec)(1).ToString()
                        If LdtTarget.Rows(Rec)(9).ToString() = "" Then
                            '���@�P�ʂŉғ��f�[�^�����������Ƃ�
                            nCnt = nCnt + 1
                        Else
                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                            '�������t�]�`�F�b�N
                            If LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) > LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                Print_Chk_F = True
                            End If
                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                            '���D�����R�[�h�ҏW
                            If (LdtTarget.Rows(Rec)(14).ToString() = "0") And (LdtTarget.Rows(Rec)(15).ToString() = "1") Then
                                '������No
                                .Pos(nCnt + 2, 5).Value = LdtTarget.Rows(Rec)(18).ToString()
                                '�J�n���t�Z�b�g
                                If LdtTarget.Rows(Rec)(7).ToString = "0" Then
                                    If LsFromDate <> "" Then
                                        '�o�͓��i�J�n���j
                                        If LsFromDate.Substring(0, 8) <> LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) Then
                                            .Pos(nCnt + 2, 9).Value = Format(CInt(LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8)), "0000/00/00")
                                        End If
                                    Else
                                        '-------Ver0.1�@�k���Ή��@MOD START-----------
                                        If LsFromKensyu <> "" Then
                                            '�����J�n���i����O�j
                                            '�w�肵���񐔂����݂��Ȃ��ꍇ
                                            If LsFromKensyu <> LdtTarget.Rows(Rec)(8).ToString() Then
                                                '�o�͊J�n�f�[�^���o�͑ΏۊO�Ƃ���
                                                Print_Kai_S = False
                                            Else
                                                '���C���i�J�n�j�ɓ_�������Z�b�g
                                                .Pos(nCnt + 2, 6).Value = Format(CInt(LdtTarget.Rows(Rec)(20).ToString.Substring(0, 8)), "0000/00/00")
                                            End If
                                        End If
                                        '-------Ver0.1�@�k���Ή��@MOD END-----------
                                    End If
                                ElseIf LdtTarget.Rows(Rec)(7).ToString = "1" Then
                                    '�N���A������i�ڐ݁j
                                    .Pos(nCnt + 2, 8).Value = Format(CInt(LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8)), "0000/00/00")
                                ElseIf LdtTarget.Rows(Rec)(7).ToString = "2" Then
                                    '�N���A������
                                    .Pos(nCnt + 2, 8).Value = Format(CInt(LdtTarget.Rows(Rec)(19).ToString.Substring(0, 8)), "0000/00/00")
                                End If
                                '�I�����t�Z�b�g
                                If LsToDate <> "" Then
                                    '�o�͓��i�I�����j
                                    If LsToDate.Substring(0, 8) <> LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                        If Print_Chk_F = False Then
                                            .Pos(nCnt + 2, 10).Value = Format(CInt(LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8)), "0000/00/00")
                                        End If
                                    End If
                                Else
                                    '-------Ver0.1�@�k���Ή��@MOD START-----------
                                    If LsToKensyu <> "" Then
                                        '�����I�����i����O�j
                                        If LsToKensyu <> LdtTarget.Rows(Rec + 1)(8).ToString() Then
                                            '�o�͊J�n�f�[�^���o�͑ΏۊO�Ƃ���
                                            Print_Kai_E = False
                                        Else
                                            '���C���i�I���j�ɓ_�������Z�b�g
                                            If Print_Chk_F = False Then
                                                .Pos(nCnt + 2, 7).Value = Format(CInt(LdtTarget.Rows(Rec + 1)(20).ToString.Substring(0, 8)), "0000/00/00")
                                            End If
                                        End If
                                    End If
                                    '-------Ver0.1�@�k���Ή��@MOD END-----------
                                End If
                                If Print_Kai_S = True And Print_Kai_E = True Then
                                    If LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) = LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                        '�w�i�ԐF�\��
                                        .Pos(nCnt + 2, nStartRow, nCnt + 2, nStartRow + LdtTarget.Columns.Count - 22).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Red
                                        Rec = Rec + 1
                                    ElseIf LdtTarget.Rows(Rec)(17).ToString.Substring(0, 8) > LdtTarget.Rows(Rec + 1)(17).ToString.Substring(0, 8) Then
                                        '�J�n�`�I���̓��t���t�]
                                        Rec = Rec + 1
                                    Else
                                        '�ғ��f�[�^�Z�o���o��
                                        For x As Integer = 21 To LdtTarget.Columns.Count - 1
                                            If LdtTarget.Rows(Rec)(x).ToString <> "" Then
                                                lArray(x - 21) = Double.Parse(LdtTarget.Rows(Rec + 1)(x).ToString) - Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                                .Pos(nCnt + 2, nStartRow + x - 21).Value = lArray(x - 21)
                                                If Double.Parse(LdtKoumoku.Rows(x - 21)(1).ToString) > 0 Then
                                                    If Double.Parse(LdtKoumoku.Rows(x - 21)(1).ToString) < lArray(x - 21) Then
                                                        .Pos(nCnt + 2, nStartRow + x - 21).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                    End If
                                                End If
                                            End If
                                        Next
                                        Rec = Rec + 1
                                        Flg_Kai = True
                                    End If
                                Else
                                    Rec = Rec + 1
                                End If
                            End If
                            nCnt = nCnt + 1
                        End If
                    End If
                Next

                .Page.End()
                .Report.End()

                ' ���[�̃v���r���[�����[�_���_�C�A���O�ŋN�����܂��B
                PrintViewer.GetDocument(XlsReport1.Document, sTemplateSheet)
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
    ''' [�o�͏���]
    ''' </summary>
    Private Sub LfXlsStart_KadoNewG()
        Dim sFrom As String = ""
        Dim sTo As String = ""
        Dim sSheet As String = ""

        Dim nCnt As Integer = 0
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 11
        Try
            With XlsReport1
                Log.Info("Start printing about [" & LsXlsTemplatePath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = LsXlsTemplatePath
                .ExcelMode = True
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()
                '-------Ver0.1�@�k���Ή��@ADD START-----------
                Dim sTemplateSheet As String = ""
                If optKado.Checked Then
                    sTemplateSheet = Config.KadoPrintListK(GrpNo).ToString.Substring(0, Config.KadoPrintListK(GrpNo).ToString.Length - 4)
                Else
                    sTemplateSheet = Config.KadoPrintListH(GrpNo).ToString.Substring(0, Config.KadoPrintListH(GrpNo).ToString.Length - 4)
                End If
                '-------Ver0.1�@�k���Ή��@ADD   END-----------
                sSheet = ""
                For Rec As Integer = 0 To LdtTarget.Rows.Count - 1
                    '���o�f�[�^�ɐ���R�[�h�A�w���R�[�h�A�R�[�i�[�R�[�h������Έȉ��̏���
                    If LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() & LdtTarget.Rows(Rec)(4).ToString() <> "" Then
                        '�L�[�u���[�N�F����R�[�h�A�w���R�[�h�A�R�[�i�[�R�[�h���ς��Ή��y�[�W
                        If sSheet <> LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() & LdtTarget.Rows(Rec)(4).ToString() Then
                            If sSheet <> "" Then
                                .Page.End()
                            End If
                            sSheet = LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() & LdtTarget.Rows(Rec)(4).ToString()
                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                            '���[�t�@�C���V�[�g���̂��擾���܂��B
                            .Page.Start(sTemplateSheet, "1-9999")
                            '-------Ver0.1�@�k���Ή��@MOD   END-----------
                            '.Cell("D5:F129").Copy()
                            .Pos(3, 4, 5, nStartRow + LdtKoumoku.Rows.Count - 1).Copy()
                            .Page.Name = LdtTarget.Rows(Rec)(0).ToString() & "�@" & LdtTarget.Rows(Rec)(1).ToString()

                            ' ���o�����Z���֌��o���f�[�^�o��
                            .Cell("O1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                            .Cell("O2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                            .Cell("A3").Value = OPMGFormConstants.STATION_NAME + LdtTarget.Rows(Rec)(0).ToString() + "�@�@�@" _
                            + OPMGFormConstants.CORNER_STR + LdtTarget.Rows(Rec)(1).ToString() _
                            + "�@�@" + OPMGFormConstants.EQUIPMENT_TYPE + "���D�@"
                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                            '�ŐV�f�[�^��I�������ꍇ
                            If chkLastData.Checked Then
                                sFrom = "�ŐV�f�[�^"
                                sTo = DateTime.Today.ToString("yyyy/MM/dd")
                            End If
                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                            .Cell("A4").Value = "      " & Lexis.TimeSpan.Gen(sFrom, "", sTo, "")

                            ' ���ږ��́A��l���Z�b�g
                            For i As Integer = 0 To LdtKoumoku.Rows.Count - 1
                                .Pos(0, i + nStartRow).Value = LdtKoumoku.Rows(i)(0).ToString()
                                .Pos(1, i + nStartRow).Value = Double.Parse(LdtKoumoku.Rows(i)(1).ToString)
                                .Pos(2, i + nStartRow).Value = Double.Parse(LdtKoumoku.Rows(i)(2).ToString)
                            Next

                            nCnt = 0
                        End If


                        '-------Ver0.1�@�k���Ή��@MOD START-----------
                        '���D�A�W�D�A���v���R�[�h�̕ҏW
                        If LdtTarget.Rows(Rec)(12).ToString() <> "" Then
                            Select Case LdtTarget.Rows(Rec)(12).ToString()
                                Case "0"
                                    .Pos(nCnt + 3, 4).Paste()
                                    .Pos(nCnt + 3, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "��"
                                    If (LdtTarget.Rows(Rec)(14).ToString() <> "0") And (LdtTarget.Rows(Rec)(14).ToString() <> "") Then
                                        .Pos(nCnt + 3, 4).Value = LdtTarget.Rows(Rec)(14).ToString()
                                        For x As Integer = 15 To LdtTarget.Columns.Count - 1
                                            If LdtTarget.Rows(Rec)(x).ToString <> "" Then
                                                .Pos(nCnt + 3, nStartRow + x - 15).Value = Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                                If Double.Parse(LdtKoumoku.Rows(x - 15)(1).ToString) > 0 Then
                                                    '-------Ver0.1�@�k���Ή��@ADD START-----------
                                                    If Double.Parse(LdtKoumoku.Rows(x - 15)(3).ToString) = 0 Then
                                                        '��l���傫������Δw�i�F���D�F
                                                        If Double.Parse(LdtKoumoku.Rows(x - 15)(1).ToString) < Double.Parse(LdtTarget.Rows(Rec)(x).ToString) Then
                                                            .Pos(nCnt + 3, nStartRow + x - 15).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                        End If
                                                    Else
                                                        '�Z���T�[���x���͊�l��菬����Δw�i�F���D�F
                                                        If Double.Parse(LdtKoumoku.Rows(x - 15)(1).ToString) > Double.Parse(LdtTarget.Rows(Rec)(x).ToString) Then
                                                            .Pos(nCnt + 3, nStartRow + x - 15).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Next
                                    End If
                                Case "1"
                                    .Pos(nCnt + 3, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "�W"
                                    If (LdtTarget.Rows(Rec)(14).ToString() <> "0") And (LdtTarget.Rows(Rec)(14).ToString() <> "") Then
                                        .Pos(nCnt + 3, 4).Value = LdtTarget.Rows(Rec)(14).ToString()
                                        For x As Integer = 15 To LdtTarget.Columns.Count - 1
                                            If LdtTarget.Rows(Rec)(x).ToString <> "" Then
                                                .Pos(nCnt + 3, nStartRow + x - 15).Value = Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                                If Double.Parse(LdtKoumoku.Rows(x - 15)(2).ToString) > 0 Then
                                                    '-------Ver0.1�@�k���Ή��@ADD START-----------
                                                    If Double.Parse(LdtKoumoku.Rows(x - 15)(3).ToString) = 0 Then
                                                        If Double.Parse(LdtKoumoku.Rows(x - 15)(2).ToString) < Double.Parse(LdtTarget.Rows(Rec)(x).ToString) Then
                                                            .Pos(nCnt + 3, nStartRow + x - 15).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                        End If
                                                    Else
                                                        '�Z���T�[���x���͊�l��菬����Δw�i�F���D�F
                                                        If Double.Parse(LdtKoumoku.Rows(x - 15)(2).ToString) > Double.Parse(LdtTarget.Rows(Rec)(x).ToString) Then
                                                            .Pos(nCnt + 3, nStartRow + x - 15).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Next
                                    End If
                                Case Else
                                    .Pos(nCnt + 3, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "��"
                                    .Pos(nCnt + 3, 7).Value = Format(CInt(LdtTarget.Rows(Rec)(13).ToString.Substring(0, 8)), "0000/00/00")
                                    For x As Integer = 15 To LdtTarget.Columns.Count - 1
                                        If LdtKoumoku.Rows(x - 15)(3).ToString = "0" Then
                                            If LdtTarget.Rows(Rec)(x).ToString <> "" Then
                                                .Pos(nCnt + 3, nStartRow + x - 15).Value = Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                                .Pos(nCnt + 1, nStartRow + x - 15).Value = Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                            Else
                                                .Pos(nCnt + 3, nStartRow + x - 15).Value = Double.Parse(LdtTarget.Rows(Rec - 2)(x).ToString) + Double.Parse(LdtTarget.Rows(Rec - 1)(x).ToString)
                                            End If
                                        End If
                                    Next
                            End Select
                            nCnt = nCnt + 1
                        Else
                            .Pos(nCnt + 3, 4).Paste()
                            .Pos(nCnt + 3, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "��"
                            .Pos(nCnt + 3 + 1, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "�W"
                            .Pos(nCnt + 3 + 2, 10).Value = LdtTarget.Rows(Rec)(6).ToString() & "��"
                            nCnt = nCnt + 3
                        End If
                    End If
                    '-------Ver0.1�@�k���Ή��@MOD END-----------
                Next

                .Page.End()
                .Report.End()
                '-------Ver0.1�@�k���Ή��@MOD START-----------
                ' ���[�̃v���r���[�����[�_���_�C�A���O�ŋN�����܂��B
                PrintViewer.GetDocument(XlsReport1.Document, sTemplateSheet)
                '-------Ver0.1�@�k���Ή��@MOD   END-----------
                PrintViewer.ShowDialog(Me)
                PrintViewer.Dispose()
                Log.Info("Printing finished.")
            End With
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

    Private Sub LfXlsStart_KadoNewY()
        Dim sFrom As String = ""
        Dim sTo As String = ""
        Dim sSheet As String = ""

        Dim nCnt As Integer = 0
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 12
        Try
            With XlsReport1
                Log.Info("Start printing about [" & LsXlsTemplatePath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = LsXlsTemplatePath
                .ExcelMode = True
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()

                sSheet = ""
                For Rec As Integer = 0 To LdtTarget.Rows.Count - 1
                    '���o�f�[�^�ɐ���R�[�h�A�w���R�[�h������Έȉ��̏���
                    If LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() <> "" Then
                        '�L�[�u���[�N�F����R�[�h�A�w���R�[�h�A�R�[�i�[�R�[�h���ς��Ή��y�[�W
                        If sSheet <> LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString() Then
                            If sSheet <> "" Then
                                .Page.End()
                            End If
                            sSheet = LdtTarget.Rows(Rec)(2).ToString() & LdtTarget.Rows(Rec)(3).ToString()

                            '���[�t�@�C���V�[�g���̂��擾���܂��B
                            .Page.Start("�ғ��f�[�^", "1-9999")
                            '.Cell("D5:F129").Copy()
                            .Pos(2, 4, 2, nStartRow + LdtKoumoku.Rows.Count - 1).Copy()
                            .Page.Name = LdtTarget.Rows(Rec)(0).ToString()

                            ' ���o�����Z���֌��o���f�[�^�o��
                            .Cell("O1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                            .Cell("O2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                            .Cell("A3").Value = OPMGFormConstants.STATION_NAME + LdtTarget.Rows(Rec)(0).ToString() + "�@�@�@" _
                            + OPMGFormConstants.EQUIPMENT_TYPE + "���������@"
                            '-------Ver0.1�@�k���Ή��@MOD START-----------
                            '�ŐV�f�[�^��I�������ꍇ
                            If chkLastData.Checked Then
                                sFrom = "�ŐV�f�[�^"
                                sTo = DateTime.Today.ToString("yyyy/MM/dd")
                            End If
                            '-------Ver0.1�@�k���Ή��@MOD END-----------
                            .Cell("A4").Value = "      " & Lexis.TimeSpan.Gen(sFrom, "", sTo, "")

                            ' ���ږ��́A��l���Z�b�g
                            For i As Integer = 0 To LdtKoumoku.Rows.Count - 1
                                .Pos(0, i + nStartRow).Value = LdtKoumoku.Rows(i)(0).ToString()
                                .Pos(1, i + nStartRow).Value = Double.Parse(LdtKoumoku.Rows(i)(1).ToString)
                            Next

                            nCnt = 0
                        End If
                        '-------Ver0.1�@�k���Ή��@MOD START-----------
                        .Pos(nCnt + 2, 4).Paste()
                        .Pos(nCnt + 2, 11).Value = LdtTarget.Rows(Rec)(6).ToString()
                        '�R�[�i�[
                        .Pos(nCnt + 2, 4).Value = LdtTarget.Rows(Rec)(1).ToString()
                        '���D�A�W�D�A���v���R�[�h�̕ҏW
                        If LdtTarget.Rows(Rec)(12).ToString() <> "" Then
                            Select Case LdtTarget.Rows(Rec)(12).ToString()
                                Case "0"
                                    If (LdtTarget.Rows(Rec)(14).ToString() <> "0") And (LdtTarget.Rows(Rec)(14).ToString() <> "") Then
                                        '������No
                                        .Pos(nCnt + 2, 5).Value = LdtTarget.Rows(Rec)(14).ToString()
                                        .Pos(nCnt + 2, 8).Value = Format(CInt(LdtTarget.Rows(Rec)(13).ToString.Substring(0, 8)), "0000/00/00")
                                        For x As Integer = 15 To LdtTarget.Columns.Count - 1
                                            If LdtTarget.Rows(Rec)(x).ToString <> "" Then
                                                .Pos(nCnt + 2, nStartRow + x - 15).Value = Double.Parse(LdtTarget.Rows(Rec)(x).ToString)
                                                If Double.Parse(LdtKoumoku.Rows(x - 15)(1).ToString) > 0 Then
                                                    If Double.Parse(LdtKoumoku.Rows(x - 15)(1).ToString) < Double.Parse(LdtTarget.Rows(Rec)(x).ToString) Then
                                                        .Pos(nCnt + 2, nStartRow + x - 15).Attr.BackColor = AdvanceSoftware.VBReport7.xlColor.Gray25
                                                    End If
                                                End If
                                            End If
                                        Next
                                    End If
                            End Select
                        End If
                    End If
                    nCnt = nCnt + 1
                Next
                '-------Ver0.1�@�k���Ή��@MOD END-----------
                .Page.End()
                .Report.End()

                ' ���[�̃v���r���[�����[�_���_�C�A���O�ŋN�����܂��B
                PrintViewer.GetDocument(XlsReport1.Document, "�ғ��f�[�^")
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
