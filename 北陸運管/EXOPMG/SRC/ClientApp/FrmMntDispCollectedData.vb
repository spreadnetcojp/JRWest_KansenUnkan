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
Imports AdvanceSoftware.VBReport7.Xls

''' <summary>
''' �y���W�f�[�^�m�F�@��ʃN���X�z
''' </summary>
Public Class FrmMntDispCollectedData
    Inherits FrmBase

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B
        LcstSearchCol = {Me.cmbEki, Me.cmbDataKind, Me.dtpYmdFrom, Me.dtpHmFrom, Me.dtpYmdTo, Me.dtpHmTo}
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
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents pnlFromTo As System.Windows.Forms.Panel
    Friend WithEvents lblFromDate As System.Windows.Forms.Label
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents lblToDate As System.Windows.Forms.Label
    Friend WithEvents dtpYmdFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpHmTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpHmFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpYmdTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents pnlDataKind As System.Windows.Forms.Panel
    Friend WithEvents cmbDataKind As System.Windows.Forms.ComboBox
    Friend WithEvents lblDataKind As System.Windows.Forms.Label
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents lblTo As System.Windows.Forms.Label

    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispCollectedData))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.wkbMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.pnlFromTo = New System.Windows.Forms.Panel()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.lblFromDate = New System.Windows.Forms.Label()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.lblToDate = New System.Windows.Forms.Label()
        Me.dtpYmdFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpHmTo = New System.Windows.Forms.DateTimePicker()
        Me.dtpHmFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpYmdTo = New System.Windows.Forms.DateTimePicker()
        Me.pnlDataKind = New System.Windows.Forms.Panel()
        Me.cmbDataKind = New System.Windows.Forms.ComboBox()
        Me.lblDataKind = New System.Windows.Forms.Label()
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.wkbMain.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlFromTo.SuspendLayout()
        Me.pnlDataKind.SuspendLayout()
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
        Me.pnlBodyBase.Controls.Add(Me.pnlFromTo)
        Me.pnlBodyBase.Controls.Add(Me.pnlDataKind)
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
        Me.lblToday.Text = "2013/08/21(��)  12:50"
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
        Me.wkbMain.Location = New System.Drawing.Point(13, 84)
        Me.wkbMain.Name = "wkbMain"
        Me.wkbMain.ProcessTabKey = False
        Me.wkbMain.ShowTabs = False
        Me.wkbMain.Size = New System.Drawing.Size(988, 482)
        Me.wkbMain.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wkbMain.TabIndex = 94
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(2, 2)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(968, 462)
        Me.shtMain.TabIndex = 99
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
        Me.btnPrint.TabIndex = 9
        Me.btnPrint.Text = "�o�@��"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(873, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 10
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
        Me.btnKensaku.TabIndex = 7
        Me.btnKensaku.Text = "���@��"
        Me.btnKensaku.UseVisualStyleBackColor = False
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
        Me.pnlFromTo.Location = New System.Drawing.Point(9, 47)
        Me.pnlFromTo.Name = "pnlFromTo"
        Me.pnlFromTo.Size = New System.Drawing.Size(634, 31)
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
        Me.dtpYmdFrom.TabIndex = 1
        '
        'dtpHmTo
        '
        Me.dtpHmTo.Location = New System.Drawing.Point(520, 6)
        Me.dtpHmTo.Name = "dtpHmTo"
        Me.dtpHmTo.ShowUpDown = True
        Me.dtpHmTo.Size = New System.Drawing.Size(60, 20)
        Me.dtpHmTo.TabIndex = 6
        '
        'dtpHmFrom
        '
        Me.dtpHmFrom.Checked = False
        Me.dtpHmFrom.Location = New System.Drawing.Point(208, 6)
        Me.dtpHmFrom.Name = "dtpHmFrom"
        Me.dtpHmFrom.ShowUpDown = True
        Me.dtpHmFrom.Size = New System.Drawing.Size(60, 20)
        Me.dtpHmFrom.TabIndex = 2
        '
        'dtpYmdTo
        '
        Me.dtpYmdTo.Location = New System.Drawing.Point(385, 6)
        Me.dtpYmdTo.Name = "dtpYmdTo"
        Me.dtpYmdTo.Size = New System.Drawing.Size(135, 20)
        Me.dtpYmdTo.TabIndex = 5
        '
        'pnlDataKind
        '
        Me.pnlDataKind.Controls.Add(Me.cmbDataKind)
        Me.pnlDataKind.Controls.Add(Me.lblDataKind)
        Me.pnlDataKind.Location = New System.Drawing.Point(244, 8)
        Me.pnlDataKind.Name = "pnlDataKind"
        Me.pnlDataKind.Size = New System.Drawing.Size(466, 33)
        Me.pnlDataKind.TabIndex = 2
        '
        'cmbDataKind
        '
        Me.cmbDataKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDataKind.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbDataKind.ItemHeight = 13
        Me.cmbDataKind.Items.AddRange(New Object() {"�S�f�[�^���", "�ʏW�D�f�[�^", "�s����Ԍ��o�f�[�^", "���s�˔j���o�f�[�^", "���������o�f�[�^", "�ُ�f�[�^�i�Ď��Ձj", "�ُ�f�[�^�i���D�@�j", "�ُ�f�[�^�i���������@�j", "�ғ��E�ێ�f�[�^�i���D�@�j", "�ғ��E�ێ�f�[�^�i���������@�j", "���ԑѕʏ�~�f�[�^", "�T�[�o���ُ�"})
        Me.cmbDataKind.Location = New System.Drawing.Point(91, 6)
        Me.cmbDataKind.MaxLength = 20
        Me.cmbDataKind.Name = "cmbDataKind"
        Me.cmbDataKind.Size = New System.Drawing.Size(355, 21)
        Me.cmbDataKind.TabIndex = 1
        '
        'lblDataKind
        '
        Me.lblDataKind.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblDataKind.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDataKind.Location = New System.Drawing.Point(8, 6)
        Me.lblDataKind.Name = "lblDataKind"
        Me.lblDataKind.Size = New System.Drawing.Size(83, 21)
        Me.lblDataKind.TabIndex = 0
        Me.lblDataKind.Text = "�f�[�^���"
        Me.lblDataKind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlEki
        '
        Me.pnlEki.Controls.Add(Me.cmbEki)
        Me.pnlEki.Controls.Add(Me.lblEki)
        Me.pnlEki.Location = New System.Drawing.Point(9, 8)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(227, 33)
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
        'FrmMntDispCollectedData
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispCollectedData"
        Me.Text = "�^�p�[�� Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wkbMain.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlFromTo.ResumeLayout(False)
        Me.pnlDataKind.ResumeLayout(False)
        Me.pnlEki.ResumeLayout(False)
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
    Private ReadOnly LcstXlsTemplateName As String = "���W�f�[�^.xls"

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i�����������W�f�[�^�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {1, 2, 3, 4, 5}

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "���W�f�[�^"

    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private LcstMaxColCnt As Integer

    ''' <summary>
    ''' �w�R�[�h�̐擪3��:�u000�v
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"

    ''' <summary>
    ''' �ꗗ�w�b�_�̃\�[�g�񊄂蓖��
    ''' �i�ꗗ�w�b�_�N���b�N���Ɋ��蓖�Ă�Ώۗ���`�B��ԍ��̓[�����΂�"-1"�̓\�[�g�ΏۊO�̗�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, 0, -1, -1, 4, -1}

    ''' <summary>
    ''' ���������ɂ���āA�����{�^��������
    ''' </summary>
    Private LcstSearchCol() As Control

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

    '����SQL�擾�敪
    Private Enum SlcSQLType
        SlcCount = 0  '�����擾�p
        SlcDetail = 1 '�f�[�^�����p
    End Enum

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
            lblTitle.Text = "���W�f�[�^�m�F"
            '�V�[�g������
            shtMain.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row
            shtMain.MaxRows = 0                                                 '�s�̏�����
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
            '�e�R���{�{�b�N�X�̍��ړo�^
            If LfSetEki() = False Then Exit Try '�w���R���{�{�b�N�X�ݒ�
            cmbEki.SelectedIndex = 0            '�f�t�H���g�\������
            LfSetDataKind()                     '�f�[�^��ʃR���{�{�b�N�X�ݒ�
            cmbDataKind.SelectedIndex = 0       '�f�t�H���g�\������

            LfClrList()                         '�ꗗ������
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
            LbEventStop = False                 '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function

#End Region

#Region "�C�x���g"

    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    Private Sub FrmMntDispCollectedData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then       '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrm() = False Then       '��������
                    Me.Close()
                    Exit Sub
                End If
            End If
            LbEventStop = True                  '�C�x���g�����n�e�e
            LfSetDateFromTo()                   'Load����Ȃ��ƊJ�n���Ԃ�00:00���ݒ肳��Ȃ��ׁA�����Őݒ肵�Ă��܂��B
            LbEventStop = False                 '�C�x���g�����n�m
            '�����{�^��������
            LfSearchTrue()
            cmbEki.Select()                     '�����t�H�[�J�X
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
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        LfWaitCursor()
        Try
            LbEventStop = True
            LogOperation(sender, e)    '�{�^���������O
            '����������
            LfClrList()

            '�f�[�^�擾����
            sSql = LfGetSelectString(SlcSQLType.SlcCount)
            nRtn = BaseSqlDataTableFill(sSql, dt)

            '�^�p�Ǘ��[����INI�t�@�C������擾�\�������擾
            Dim nMaxCount As Integer = Config.MaxUpboundDataToGet

            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case Else              '�Y���Ȃ�
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

    '//////////////////////////////////////////////SelectedIndexChanged

    '''<summary>
    ''' �u�w�v�R���{
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()         '�ꗗ�V�[�g�̏������iLfClrList�j
            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            LfSearchTrue()      '�����{�^���������iLfSearchTrue�j
        Catch ex As Exception
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub
    '''<summary>
    ''' �u�f�[�^��ʁv�R���{
    ''' </summary>
    Private Sub cmbDataKind_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbDataKind.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()         '�ꗗ�V�[�g�̏������iLfClrList�j
            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            LfSearchTrue()      '�����{�^���������iLfSearchTrue�j
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
            LfClrList()         '�ꗗ�V�[�g�̏������iLfClrList�j
            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            LfSearchTrue()      '�����{�^���������iLfSearchTrue�j
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
        If LcstSortCol(e.Column) = -1 Then Exit Sub
        Try
            shtMain.BeginUpdate()
            If intCurrentSortColumn > -1 Then
                '�O��\�[�g���ꂽ��w�b�_�̃C���[�W���폜����
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '�O��\�[�g���ꂽ��̔w�i�F������������
                shtMain.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '�O��\�[�g���ꂽ��̃Z���r������������
                shtMain.Columns(intCurrentSortColumn).SetBorder(New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, _
                                                        GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), GrapeCity.Win.ElTabelleSheet.Borders.All)
            End If
            intCurrentSortColumn = e.Column

            If bolColumn1SortOrder(intCurrentSortColumn) = False Then
                '�\�[�g�����̔w�i�F��ݒ肷��
                shtMain.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
                '�\�[�g�����̃Z���r����ݒ肷��
                shtMain.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
                '��w�b�_�̃C���[�W��ݒ肷��
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(1)
                '�~���Ń\�[�g����
                Call SheetSort(shtMain, e.Column, GrapeCity.Win.ElTabelleSheet.SortOrder.Descending)
                '��̃\�[�g��Ԃ�ۑ�����
                bolColumn1SortOrder(intCurrentSortColumn) = True
            Else
                '�\�[�g�����̔w�i�F��ݒ肷��
                shtMain.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
                '�\�[�g�����̃Z���r����ݒ肷��
                shtMain.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
                '��w�b�_�̃C���[�W��ݒ肷��
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(0)
                '�����Ń\�[�g����
                Call SheetSort(shtMain, e.Column, GrapeCity.Win.ElTabelleSheet.SortOrder.Ascending)
                '��̃\�[�g��Ԃ�ۑ�����
                bolColumn1SortOrder(intCurrentSortColumn) = False
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw ex  'NOTE: ����ɂ��A�v���I���ƂȂ邪�A���^�p�ł��������s����邱�Ƃ͖����z�肩�B
        End Try
        shtMain.EndUpdate()
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
    Private Sub SheetSort(ByRef sheetTarget As GrapeCity.Win.ElTabelleSheet.Sheet, ByVal intKeyColumn As Integer, _
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
                (cmbDataKind.SelectedIndex < 0)) Then
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
    ''' [�f�[�^��ʃR���{�ݒ�]
    ''' </summary>
    Private Function LfSetDataKind() As Boolean
        'TODO: �Ď��Րݒ����@��ڑ���Ԃ̎��W���s�͂Ƃ������A
        'FREX�����ID���o�f�[�^�̎��W���s�͏o���Ȃ��Ă悢�̂��H
        cmbDataKind.Items.AddRange({DbConstants.CdtKindAll, DbConstants.CdtKindBesshuData, _
                                    DbConstants.CdtKindFuseiJoshaData, DbConstants.CdtKindKyokoToppaData, _
                                    DbConstants.CdtKindFunshitsuData, DbConstants.CdtKindFaultData, _
                                    DbConstants.CdtKindKadoData, DbConstants.CdtKindTrafficData, _
                                    DbConstants.CdtKindServerError})
        Return True
    End Function

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
                    sBuilder.AppendLine("SELECT COUNT(1) FROM V_COLLECTED_DATA_TYPO")
                Case slcSQLType.SlcDetail
                    '�擾����--------------------------
                    sBuilder.AppendLine("SELECT STATION_CODE, STATION_NAME, CORNER_NAME,DATA_KIND,")
                    sBuilder.AppendLine(" SUBSTRING(PROCESSING_TIME,1,4)+'/'+SUBSTRING(PROCESSING_TIME,5,2)+'/'+SUBSTRING(PROCESSING_TIME,7,2)+' '+")
                    sBuilder.AppendLine(" SUBSTRING(PROCESSING_TIME,9,2)+':'+SUBSTRING(PROCESSING_TIME,11,2)+':'+SUBSTRING(PROCESSING_TIME,13,2) AS YMDMS,")
                    sBuilder.AppendLine("ERROR_INFO FROM V_COLLECTED_DATA_TYPO")
            End Select
            sSqlWhere.AppendLine("where 0 = 0")

            'Where�吶��--------------------------
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
            '�f�[�^���
            If Not (cmbDataKind.SelectedIndex = 0) Then
                Dim dataKind As String = cmbDataKind.SelectedItem.ToString
                sSqlWhere.AppendLine(String.Format(" AND (DATA_KIND = {0})", Utility.SetSglQuot(dataKind)))
            End If
            '�J�n�I������
            sFrom = Replace(Replace(Replace(dtpYmdFrom.Text, "�N", ""), "��", ""), "��", "") + _
                   Replace(dtpHmFrom.Text, ":", "") + "00"
            sTo = Replace(Replace(Replace(dtpYmdTo.Text, "�N", ""), "��", ""), "��", "") + _
                  Replace(dtpHmTo.Text, ":", "") + "59"
            sSqlWhere.AppendLine(String.Format(" And (PROCESSING_TIME >= {0}) And (PROCESSING_TIME <= {1})", _
                                      Utility.SetSglQuot(sFrom), _
                                      Utility.SetSglQuot(sTo)))

            If slcSQLType.Equals(slcSQLType.SlcDetail) Then
                sSqlWhere.AppendLine(" ORDER BY STATION_CODE,CORNER_CODE ASC ")
            End If
            'Where�匋��
            sSql = sBuilder.ToString() + sSqlWhere.ToString()

            Debug.Print(sSql)
            Return sSql.ToString
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
                .Cell("F1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("F2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim
                .Cell("D3").Value = lblDataKind.Text.Trim + "�F" + cmbDataKind.Text.Trim
                .Cell("C4").Value = Lexis.TimeSpan.Gen( _
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
                        .Pos(x + 1, y + nStartRow).Value = shtMain.Item(LcstPrntCol(x), y).Text
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

#End Region

    Private Sub shtMain_ValueChanged(sender As System.Object, e As GrapeCity.Win.ElTabelleSheet.ValueChangedEventArgs) Handles shtMain.ValueChanged

    End Sub
End Class