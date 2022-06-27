' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/04/01  �@�@ ����  �k���Ή�
'                                   (���[�o�͂ŃO���[�vNo�𔻕ʂ����[�𕪂���)
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '�萔�l�̂ݎg�p
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO
Imports System.Text

''' <summary>
''' �y�s����Ԍ��o�f�[�^�m�F�@��ʃN���X�z
''' </summary>
Public Class FrmMntDispFuseiJoshaData
    Inherits FrmBase

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B
        LcstSearchCol = {Me.cmbEki, Me.cmbMado, Me.cmbDirection, Me.dtpYmdFrom, Me.dtpHmFrom, Me.dtpYmdTo, Me.dtpHmTo}

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
        Me.lblToday.Text = "2013/08/31(�y)  17:18"
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
        Me.wkbMain.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(705, 584)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 5
        Me.btnPrint.Text = "�o�@��"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'lblFromTo
        '
        Me.lblFromTo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFromTo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFromTo.Location = New System.Drawing.Point(279, 6)
        Me.lblFromTo.Name = "lblFromTo"
        Me.lblFromTo.Size = New System.Drawing.Size(37, 20)
        Me.lblFromTo.TabIndex = 93
        Me.lblFromTo.Text = "����"
        Me.lblFromTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbDirection
        '
        Me.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDirection.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbDirection.ItemHeight = 13
        Me.cmbDirection.Items.AddRange(New Object() {"", "�w�w�w", "�w�w�w", "�w�w�w", "�w�w�w", "�w�w�w", "�w�w�w", "�w�w�w", "�w�w�w", "�w�w�w", "�w�w�w"})
        Me.cmbDirection.Location = New System.Drawing.Point(67, 7)
        Me.cmbDirection.Name = "cmbDirection"
        Me.cmbDirection.Size = New System.Drawing.Size(80, 21)
        Me.cmbDirection.TabIndex = 2
        '
        'cmbMado
        '
        Me.cmbMado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMado.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMado.ItemHeight = 13
        Me.cmbMado.Items.AddRange(New Object() {"", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w"})
        Me.cmbMado.Location = New System.Drawing.Point(67, 6)
        Me.cmbMado.Name = "cmbMado"
        Me.cmbMado.Size = New System.Drawing.Size(162, 21)
        Me.cmbMado.TabIndex = 1
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Items.AddRange(New Object() {"", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w"})
        Me.cmbEki.Location = New System.Drawing.Point(44, 6)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(162, 21)
        Me.cmbEki.TabIndex = 0
        '
        'lblFrom
        '
        Me.lblFrom.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFrom.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFrom.Location = New System.Drawing.Point(4, 6)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(64, 20)
        Me.lblFrom.TabIndex = 92
        Me.lblFrom.Text = "�J�n����"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDirection
        '
        Me.lblDirection.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblDirection.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDirection.Location = New System.Drawing.Point(3, 7)
        Me.lblDirection.Name = "lblDirection"
        Me.lblDirection.Size = New System.Drawing.Size(64, 21)
        Me.lblDirection.TabIndex = 91
        Me.lblDirection.Text = "�ʘH����"
        Me.lblDirection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMado
        '
        Me.lblMado.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMado.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMado.Location = New System.Drawing.Point(3, 6)
        Me.lblMado.Name = "lblMado"
        Me.lblMado.Size = New System.Drawing.Size(64, 21)
        Me.lblMado.TabIndex = 90
        Me.lblMado.Text = "�R�[�i�["
        Me.lblMado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblEki
        '
        Me.lblEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblEki.Location = New System.Drawing.Point(4, 6)
        Me.lblEki.Name = "lblEki"
        Me.lblEki.Size = New System.Drawing.Size(39, 21)
        Me.lblEki.TabIndex = 89
        Me.lblEki.Text = "�w��"
        Me.lblEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(873, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 6
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(873, 33)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 4
        Me.btnKensaku.Text = "���@��"
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
        Me.lblTo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTo.Location = New System.Drawing.Point(328, 6)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(65, 20)
        Me.lblTo.TabIndex = 93
        Me.lblTo.Text = "�I������"
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
        Me.lblFromTo2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFromTo2.Location = New System.Drawing.Point(606, 6)
        Me.lblFromTo2.Name = "lblFromTo2"
        Me.lblFromTo2.Size = New System.Drawing.Size(37, 20)
        Me.lblFromTo2.TabIndex = 94
        Me.lblFromTo2.Text = "�܂�"
        Me.lblFromTo2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmMntDispFuseiJoshaData
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispFuseiJoshaData"
        Me.Text = "�^�p�[�� V1.00"
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

#Region "�錾�̈�iPrivate�j"

    ''' <summary>
    ''' ���������ďo����
    ''' �iTrue:���������ďo�ς݁AFalse:�����������ďo(Form_Load���ŏ����������{)�j
    ''' </summary>
    Private LbInitCallFlg As Boolean = True

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
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private LcstXlsSheetName As String = "�s����Ԍ��o�f�[�^"


    ''' <summary>
    ''' �w�R�[�h�̐擪3��:�u000�v
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"

    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private LcstMaxColCnt As Integer

    ''' <summary>
    ''' �ꗗ�w�b�_�̃\�[�g�񊄂蓖��
    ''' �i�ꗗ�w�b�_�N���b�N���Ɋ��蓖�Ă�Ώۗ���`�B��ԍ��̓[�����΂�"-1"�̓\�[�g�ΏۊO�̗�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {6, -1, -1, -1, -1, -1, -1, -1, -1}

    ''' <summary>
    ''' ���������ɂ���āA�����{�^��������
    ''' </summary>
    Private LcstSearchCol() As Control

    '�K�p�J�n��
    Private sApplyDate As String = Now.ToString("yyyyMMdd")
    '�K�p�J�n��
    Public Property ApplyDate() As String
        Get
            Return sApplyDate
        End Get
        Set(ByVal Value As String)
            sApplyDate = Value
        End Set
    End Property

    '����SQL�擾�敪
    Private Enum SlcSQLType
        SlcCount = 0  '�����擾�p
        SlcDetail = 1 '�f�[�^�����p
    End Enum

    ''' <summary>
    ''' Title���
    ''' </summary>
    Private Const FormTitle As String = "�s����Ԍ��o�f�[�^�m�F"

    ''' <summary>
    ''' ���[Title���
    ''' </summary>
    Private Const FormTitle2 As String = "�s����Ԍ��o�f�[�^"

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

            '��ʃ^�C�g��
            lblTitle.Text = FormTitle

            '�V�[�g������
            shtMain.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row                      '�s�I�����[�h
            shtMain.MaxRows() = 0                                               '�s�̏�����
            LcstMaxColCnt = shtMain.MaxColumns()                                '�񐔂��擾
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   '�V�[�g��\�����[�h
            '�V�[�g�̃w�b�_�I���C�x���g�̃n���h���ǉ�
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtMain.ColumnHeaders.HeaderClick, AddressOf Me.shtMainColumnHeaders_HeadersClick

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

            '�e�R���{�{�b�N�X�̍��ړo�^()
            If LfSetEki() = False Then Exit Try '�w���R���{�{�b�N�X�ݒ�
            cmbEki.SelectedIndex = 0            '�f�t�H���g�\������
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try '�R�[�i�[�R���{�{�b�N�X�ݒ�
            cmbMado.SelectedIndex = 0           '�f�t�H���g�\������
            If LfSetDirection() = False Then Exit Try '�ʘH�����R���{�{�b�N�X�ݒ�
            cmbDirection.SelectedIndex = 0      '�f�t�H���g�\������

            '�ꗗ�\�[�g�̏�����()
            LfClrList()
            LbEventStop = False '�C�x���g�����n�m
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
    Private Sub FrmMntDispFuseiJoshaData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrm() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If

            '�J�n���t�A�J�n�����A�I�����t�A�I�������̃R���g���[��������
            LbEventStop = True              '�C�x���g�����n�e�e
            LfSetDateFromTo()               'Load����Ȃ��ƊJ�n���Ԃ�00:00���ݒ肳��Ȃ��ׁA�����Őݒ肵�Ă��܂��B
            LbEventStop = False             '�C�x���g�����n�m

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
    Private Sub BtnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
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
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        LfWaitCursor()

        Try
            LbEventStop = True
            LogOperation(sender, e)    '�{�^���������O
            '����������
            LfClrList()

            '�^�p�Ǘ��[����INI�t�@�C������擾�\�������擾
            Dim nMaxCount As Integer = Config.MaxUpboundDataToGet

            '�����擾�`�F�b�N
            sSql = LfGetSelectString(SlcSQLType.SlcCount)
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
            sSql = LfGetSelectString(SlcSQLType.SlcDetail)
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
            Log.Fatal("Unwelcome Exception caught.", ex)        '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W
            btnReturn.Select()
        Finally
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
            '-------Ver0.1�@�k���Ή��@ADD START---------
            sPath = Path.Combine(sPath, Config.FuseiJoshaPrintList(GrpNo).ToString)
            LcstXlsSheetName = Config.FuseiJoshaPrintList(GrpNo).ToString.Substring(0, Config.FuseiJoshaPrintList(GrpNo).ToString.Length - 4)
            '-------Ver0.1�@�k���Ή��@ADD END---------
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
            '�G���[���b�Z�[�W
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////SelectedIndexChanged

    ''' <summary>
    ''' �w�R���{
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            '-------Ver0.1�@�k���Ή��@ADD START---------
            '�O���[�vNo���擾
            Dim station As String = cmbEki.SelectedValue.ToString
            If station <> "" And station <> ClientDaoConstants.TERMINAL_ALL Then
                GrpNo = CInt(station.Substring(0, 1))
            ElseIf station = ClientDaoConstants.TERMINAL_ALL Then
                GrpNo = ClientDaoConstants.TERMINAL_ALL_GrpNo
            End If
            '-------Ver0.1�@�k���Ή��@ADD END---------
            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If LfSetMado(station.Substring(station.Length - 6, 6)) = False Then
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                If cmbMado.Enabled = True Then BaseCtlDisabled(pnlMado, False)
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbMado.SelectedIndex = 0               '���C�x���g�����ӏ�
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
    ''' �R�[�i�[�R���{
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
    ''' �ʘH�����R���{
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

            '�O��I�����ꂽ��w�b�_�̏�����
            If intCurrentSortColumn > -1 Then
                '��w�b�_�̃C���[�W���폜����
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '��̔w�i�F������������
                shtMain.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '��̃Z���r������������
                shtMain.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
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
            '�}�E�X�J�[�\������w�b�_��ɂ���ꍇ
            If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtMain.CrossCursor = Cursors.Default
            Else
                '�}�E�X�J�[�\��������ɖ߂�
                shtMain.CrossCursor = Nothing
            End If
        Catch ex As Exception
        End Try
    End Sub
    ''' <summary>
    ''' �\�[�g
    ''' </summary>
    Private Sub SheetSort(ByRef sheetTarget As GrapeCity.Win.ElTabelleSheet.Sheet, ByVal intKeyColumn As Integer,
                          ByVal sortOrder As GrapeCity.Win.ElTabelleSheet.SortOrder)
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
            '-------Ver0.1�@�k���Ή��@ADD START---------
            dt = oMst.SelectTable(True, "G", True)
            '-------Ver0.1�@�k���Ή��@ADD END---------
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
    ''' [�ʘH�����R���{�ݒ�]
    ''' </summary>
    ''' <returns>True:�����AFalse:���s</returns>
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
    ''' [�����pSELECT������擾]
    ''' </summary>
    ''' <returns>SELECT��</returns>
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
                    '�����擾����--------------------------
                    sBuilder.AppendLine(" SELECT COUNT(1) FROM V_FUSEI_JOSHA_DATA ")
                Case slcSQLType.SlcDetail
                    '�擾����--------------------------
                    sBuilder.AppendLine(" SELECT STATION_NAME, CORNER_NAME,UNIT_NO,PASSAGE_NAME, ")
                    sBuilder.AppendLine(" WRANG_TARGET_NAME,count(*),STATION_CODE,WRANG_TARGET_NO,CORNER_CODE ")
                    sBuilder.AppendLine(" FROM V_FUSEI_JOSHA_DATA ")
            End Select

            'Where�吶��--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine("")
            sSqlWhere.AppendLine(" where 0 = 0 ")
            '-------Ver0.1�@�k���Ή��@MOD START---------
            '�w
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
            '-------Ver0.1�@�k���Ή��@MOD END---------
            '�R�[�i�[
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (CORNER_CODE = {0})", _
                                          Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If
            '�ʘH����
            If Not (cmbDirection.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (PASSAGE_FLG = {0})", _
                                          Utility.SetSglQuot(cmbDirection.SelectedValue.ToString)))
            End If
            '�J�n�I������
            sFrom = Replace(Replace(Replace(dtpYmdFrom.Text, "�N", ""), "��", ""), "��", "") + _
                    Replace(dtpHmFrom.Text, ":", "") + "00"
            sTo = Replace(Replace(Replace(dtpYmdTo.Text, "�N", ""), "��", ""), "��", "") + _
                  Replace(dtpHmTo.Text, ":", "") + "59"
            sSqlWhere.AppendLine(String.Format(" And (PROCESSING_TIME >= {0} And PROCESSING_TIME <= {1})", _
                                      Utility.SetSglQuot(sFrom), _
                                      Utility.SetSglQuot(sTo)))

            If slcSQLType.Equals(slcSQLType.SlcDetail) Then

                'Group by�吶��
                sSqlWhere.AppendLine(" group by STATION_NAME, CORNER_NAME,UNIT_NO,PASSAGE_NAME,WRANG_TARGET_NAME,STATION_CODE,WRANG_TARGET_NO,CORNER_CODE ")
                'Order by�吶��
                sSqlWhere.AppendLine(" order by STATION_CODE, CORNER_CODE asc ")
            End If

            'Where�匋��()
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
    ''' [�ꗗ�ݒ�]
    ''' </summary>
    ''' <param name="dt">�ݒ�Ώۃf�[�^�e�[�u��</param>
    Private Sub LfSetSheetData(ByVal dt As DataTable)
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try

            shtMain.MaxRows = dt.Rows.Count         '���o�������̍s���ꗗ�ɍ쐬
            shtMain.Rows.SetAllRowsHeight(21)       '�s�����𑵂���
            shtMain.DataSource = dt                 '�f�[�^���Z�b�g

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            wkbMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' [�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 8
        Dim Count As Integer = 0

        Try
            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = sPath
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()
                '���[�t�@�C���V�[�g���̂��擾���܂��B
                .Page.Start(LcstXlsSheetName, "1-9999")

                ' ���o�����Z���֌��o���f�[�^�o��
                .Cell("B1").Value = FormTitle2
                '-------Ver0.1�@�k���Ή��@MOD START---------
                .Cell("P1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("P2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                '-------Ver0.1�@�k���Ή��@MOD END---------
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim + "�@�@�@" +
                                    OPMGFormConstants.CORNER_STR + cmbMado.Text.Trim _
                                    + "�@�@" + Lexis.PassageInfo.Gen(cmbDirection.Text.Trim)
                .Cell("C4").Value = Lexis.TimeSpan.Gen( _
                                                  Replace(Replace(Replace(dtpYmdFrom.Text, "�N", "/"), "��", "/"), "��", ""), _
                                                  dtpHmFrom.Text, _
                                                  Replace(Replace(Replace(dtpYmdTo.Text, "�N", "/"), "��", "/"), "��", ""), _
                                                  dtpHmTo.Text)

                ' �z�M�Ώۂ̃f�[�^�����擾���܂�
                nRecCnt = shtMain.MaxRows

                '�f�[�^�W�v
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
                        '-------Ver0.1�@�k���Ή��@MOD START---------
                        Dim strsLineData(13) As String
                        '-------Ver0.1�@�k���Ή��@MOD ENT---------
                        strsLineData(0) = shtMain.Item(0, i).Text '�w��
                        strsLineData(1) = shtMain.Item(1, i).Text  '�R�[�i�[
                        strsLineData(2) = shtMain.Item(2, i).Text  '���@
                        strsLineData(3) = shtMain.Item(3, i).Text  '�ʘH����
                        lstReportData.Add(strsLineData)
                    End If

                    '���}��
                    If shtMain.Item(7, i).Text = "1" Then
                        lstReportData(lstReportData.Count - 1)(4) = shtMain.Item(5, i).Text
                    End If
                    '���ꌔ
                    If shtMain.Item(7, i).Text = "2" Then
                        lstReportData(lstReportData.Count - 1)(5) = shtMain.Item(5, i).Text
                    End If
                    '�񐔌�
                    If shtMain.Item(7, i).Text = "3" Then
                        lstReportData(lstReportData.Count - 1)(6) = shtMain.Item(5, i).Text
                    End If
                    '�����
                    If shtMain.Item(7, i).Text = "4" Then
                        lstReportData(lstReportData.Count - 1)(7) = shtMain.Item(5, i).Text
                    End If
                    '������
                    If shtMain.Item(7, i).Text = "7" Then
                        lstReportData(lstReportData.Count - 1)(8) = shtMain.Item(5, i).Text
                    End If
                    '�Z���o
                    If shtMain.Item(7, i).Text = "8" Then
                        lstReportData(lstReportData.Count - 1)(9) = shtMain.Item(5, i).Text
                    End If
                    '�Z�o��
                    If shtMain.Item(7, i).Text = "9" Then
                        lstReportData(lstReportData.Count - 1)(10) = shtMain.Item(5, i).Text
                    End If
                    '���D
                    If shtMain.Item(7, i).Text = "5" Then
                        lstReportData(lstReportData.Count - 1)(11) = shtMain.Item(5, i).Text
                    End If
                    '�W�D
                    If shtMain.Item(7, i).Text = "6" Then
                        lstReportData(lstReportData.Count - 1)(12) = shtMain.Item(5, i).Text
                    End If
                    '-------Ver0.1�@�k���Ή��@ADD START---------
                    '���Ԓ���
                    If shtMain.Item(7, i).Text = "10" Then
                        lstReportData(lstReportData.Count - 1)(13) = shtMain.Item(5, i).Text
                    End If
                    '-------Ver0.1�@�k���Ή��@ADD END---------
                Next

                For i As Integer = 1 To lstReportData.Count - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                For i As Integer = 0 To lstReportData.Count - 1
                    '-------Ver0.1�@�k���Ή��@MOD START---------
                    For j As Integer = 0 To 13
                        '-------Ver0.1�@�k���Ή��@MOD END---------
                        .Pos(1 + j, i + nStartRow).Value = lstReportData(i)(j)
                    Next
                Next

                ' ����͈͂̐ݒ�i���W�w��F�J�n��C�J�n�s�C�I����C�I���s�j
                ' .Page.Attr.PrintArea(0, 0, 25, nRecCnt + nStartRow - 1)
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
