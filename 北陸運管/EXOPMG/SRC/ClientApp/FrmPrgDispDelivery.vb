' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
'   0.1      2013/11/11  (NES)����    �t�F�[�Y�Q��M�������h-�h�o�͑Ή�
'   0.2      2014/06/01       ����    �ꗗ�\�[�g�Ή�
' **********************************************************************

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO

''' <summary>
''' �v���O�����z�M�󋵕\��
''' </summary>
''' <remarks>�v���O�����Ǘ����j���[���A�u�z�M�󋵕\���v�{�^�����N���b�N���邱�Ƃɂ��A
''' �{��ʂ�\������B�m�F�������u�@�햼�́v�u�}�X�^���́v�u�p�^�[�����́v�u�o�[�W�����v��I�����A
''' �u�����v���N���b�N���邱�Ƃɂ��A���Y�f�[�^�̕\�����s���B</remarks>
Public Class FrmPrgDispDelivery
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
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents WorkBook1 As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents lblPrograme As System.Windows.Forms.Label
    Friend WithEvents lblArea As System.Windows.Forms.Label
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents cmbVersion As System.Windows.Forms.ComboBox
    Friend WithEvents cmbPrgName As System.Windows.Forms.ComboBox
    Friend WithEvents cmbAppliedArea As System.Windows.Forms.ComboBox
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents lblModel As System.Windows.Forms.Label
    Friend WithEvents shtDspDelivery As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents cmbProgram As System.Windows.Forms.ComboBox
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmPrgDispDelivery))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.WorkBook1 = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtDspDelivery = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.cmbVersion = New System.Windows.Forms.ComboBox()
        Me.cmbPrgName = New System.Windows.Forms.ComboBox()
        Me.cmbAppliedArea = New System.Windows.Forms.ComboBox()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.lblPrograme = New System.Windows.Forms.Label()
        Me.lblArea = New System.Windows.Forms.Label()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.cmbProgram = New System.Windows.Forms.ComboBox()
        Me.pnlBodyBase.SuspendLayout()
        Me.WorkBook1.SuspendLayout()
        CType(Me.shtDspDelivery, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.cmbModel)
        Me.pnlBodyBase.Controls.Add(Me.lblModel)
        Me.pnlBodyBase.Controls.Add(Me.WorkBook1)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.cmbVersion)
        Me.pnlBodyBase.Controls.Add(Me.cmbPrgName)
        Me.pnlBodyBase.Controls.Add(Me.cmbAppliedArea)
        Me.pnlBodyBase.Controls.Add(Me.lblVersion)
        Me.pnlBodyBase.Controls.Add(Me.lblPrograme)
        Me.pnlBodyBase.Controls.Add(Me.lblArea)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnKensaku)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/11/08(��)  16:37"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'WorkBook1
        '
        Me.WorkBook1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.WorkBook1.Controls.Add(Me.shtDspDelivery)
        Me.WorkBook1.Location = New System.Drawing.Point(21, 84)
        Me.WorkBook1.Name = "WorkBook1"
        Me.WorkBook1.ProcessTabKey = False
        Me.WorkBook1.ShowTabs = False
        Me.WorkBook1.Size = New System.Drawing.Size(866, 458)
        Me.WorkBook1.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.WorkBook1.TabIndex = 0
        '
        'shtDspDelivery
        '
        Me.shtDspDelivery.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtDspDelivery.Data = CType(resources.GetObject("shtDspDelivery.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtDspDelivery.Location = New System.Drawing.Point(1, 1)
        Me.shtDspDelivery.Name = "shtDspDelivery"
        Me.shtDspDelivery.Size = New System.Drawing.Size(847, 439)
        Me.shtDspDelivery.TabIndex = 0
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(707, 584)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 6
        Me.btnPrint.Text = "�o�@��"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'cmbVersion
        '
        Me.cmbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbVersion.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbVersion.ItemHeight = 13
        Me.cmbVersion.Items.AddRange(New Object() {""})
        Me.cmbVersion.Location = New System.Drawing.Point(533, 50)
        Me.cmbVersion.Name = "cmbVersion"
        Me.cmbVersion.Size = New System.Drawing.Size(100, 21)
        Me.cmbVersion.TabIndex = 4
        '
        'cmbPrgName
        '
        Me.cmbPrgName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPrgName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbPrgName.ItemHeight = 13
        Me.cmbPrgName.Items.AddRange(New Object() {""})
        Me.cmbPrgName.Location = New System.Drawing.Point(153, 50)
        Me.cmbPrgName.Name = "cmbPrgName"
        Me.cmbPrgName.Size = New System.Drawing.Size(242, 21)
        Me.cmbPrgName.TabIndex = 3
        '
        'cmbAppliedArea
        '
        Me.cmbAppliedArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAppliedArea.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbAppliedArea.ItemHeight = 13
        Me.cmbAppliedArea.Items.AddRange(New Object() {""})
        Me.cmbAppliedArea.Location = New System.Drawing.Point(533, 20)
        Me.cmbAppliedArea.Name = "cmbAppliedArea"
        Me.cmbAppliedArea.Size = New System.Drawing.Size(198, 21)
        Me.cmbAppliedArea.TabIndex = 2
        '
        'lblVersion
        '
        Me.lblVersion.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblVersion.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblVersion.Location = New System.Drawing.Point(424, 50)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(80, 18)
        Me.lblVersion.TabIndex = 91
        Me.lblVersion.Text = "�o�[�W����"
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPrograme
        '
        Me.lblPrograme.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrograme.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrograme.Location = New System.Drawing.Point(45, 50)
        Me.lblPrograme.Name = "lblPrograme"
        Me.lblPrograme.Size = New System.Drawing.Size(107, 18)
        Me.lblPrograme.TabIndex = 90
        Me.lblPrograme.Text = "�v���O��������"
        Me.lblPrograme.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblArea
        '
        Me.lblArea.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblArea.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblArea.Location = New System.Drawing.Point(424, 20)
        Me.lblArea.Name = "lblArea"
        Me.lblArea.Size = New System.Drawing.Size(105, 18)
        Me.lblArea.TabIndex = 89
        Me.lblArea.Text = "�K�p�G���A����"
        Me.lblArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 7
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(872, 20)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 5
        Me.btnKensaku.Text = "���@��"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.ItemHeight = 13
        Me.cmbModel.Items.AddRange(New Object() {""})
        Me.cmbModel.Location = New System.Drawing.Point(153, 20)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(106, 21)
        Me.cmbModel.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(45, 20)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(91, 18)
        Me.lblModel.TabIndex = 96
        Me.lblModel.Text = "�@��"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbProgram
        '
        Me.cmbProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProgram.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbProgram.ItemHeight = 13
        Me.cmbProgram.Items.AddRange(New Object() {""})
        Me.cmbProgram.Location = New System.Drawing.Point(460, 20)
        Me.cmbProgram.Name = "cmbProgram"
        Me.cmbProgram.Size = New System.Drawing.Size(198, 21)
        Me.cmbProgram.TabIndex = 1
        '
        'FrmPrgDispDelivery
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgDispDelivery"
        Me.Text = "�^�p�[�� "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.WorkBook1.ResumeLayout(False)
        CType(Me.shtDspDelivery, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "�e��錾�̈�"

    '�v���O��������
    Public Const APPLIED_AREANAME As String = "�K�p�G���A���́F"


    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    ''' �o�͗p�e���v���[�g�t�@�C����
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "�v���O�����z�M��.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "�v���O�����z�M��"

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "�v���O�����z�M�󋵕\��"

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������ʏW�D�f�[�^�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3, 4, 5, 6, 7, 8}
    '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD START-----------
    ''' <summary>
    ''' �ꗗ�w�b�_�̃\�[�g�񊄂蓖��
    ''' �i�ꗗ�w�b�_�N���b�N���Ɋ��蓖�Ă�Ώۗ���`�B��ԍ��̓[�����΂�"-1"�̓\�[�g�ΏۊO�̗�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, -1, -1, -1, 4, -1, -1, 7, 8}
    '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD END-----------
    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private LcstMaxColCnt As Integer

    Private LbInitCallFlg As Boolean = False

#End Region

#Region "�t�H�[�����[�h"
    '�t�H�[�����[�h
    Private Sub FrmPrgDispDelivery_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LfWaitCursor()
        If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
            If InitFrmData() = False Then   '��������
                Me.Close()
                Exit Sub
            End If
        End If

        LfWaitCursor(False)
    End Sub

    ''' <summary>
    ''' �}�X�^�z�M�󋵕\����ʂ̃f�[�^����������
    ''' </summary>
    ''' <remarks>
    ''' �}�X�^�z�M�󋵕\���ݒ�f�[�^���������A��ʂɕ\������
    ''' </remarks>
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e

        Try
            Log.Info("Method started.")

            '��ʃ^�C�g��
            lblTitle.Text = LcstFormTitle

            '�V�[�g������
            shtDspDelivery.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtDspDelivery.ViewMode = ElTabelleSheet.ViewMode.Row                      '�s�I�����[�h
            shtDspDelivery.MaxRows() = 0                                               '�s�̏�����
            LcstMaxColCnt = shtDspDelivery.MaxColumns()                                '�񐔂��擾
            shtDspDelivery.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   '�V�[�g��\�����[�h
            shtDspDelivery.ColumnHeaders(2, 0).Caption = " "
            shtDspDelivery.ColumnHeaders(6, 0).Caption = " "
            '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD START-----------
            '�V�[�g�̃w�b�_�I���C�x���g�̃n���h���ǉ�
            shtDspDelivery.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtDspDelivery.ColumnHeaders.HeaderClick, AddressOf Me.shtDspDeliveryColumnHeaders_HeadersClick
            '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD END-----------

            '�@�햼�̂�ݒ肷��B
            If setCmbModel() = False Then Exit Try
            cmbModel.SelectedIndex = 0            '�f�t�H���g�\������

            '�G���A���̂�ݒ肷��B
            If setCmbAreaName(Me.cmbModel.SelectedValue.ToString) = False Then Exit Try
            cmbAppliedArea.SelectedIndex = 0      '�f�t�H���g�\������

            '�}�X�^���̂�ݒ肷��B
            If setCmbProgram(Me.cmbModel.SelectedValue.ToString, Me.cmbAppliedArea.SelectedValue.ToString) = False Then Exit Try
            cmbPrgName.SelectedIndex = 0          '�f�t�H���g�\������

            '�{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B
            Call enableBtn()

            bRtn = True

        Catch ex As DatabaseException
            '��ʕ\�������Ɏ��s���܂���
            bRtn = False

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

#Region "�R���{�N���b�N"

    ' �u�@�햼�́v�R���{�N���b�N
    Private Sub cmbModel_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            Select Case cmbModel.SelectedValue.ToString
                Case "G"
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = "�Ď���"
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = "���D�@"
                Case "Y"
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = "���׎��W�^�d�w����"
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = "���������@"
                Case "W"
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = "�Ď���"
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = "�Ď���"
                Case Else
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = " "
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = " "
            End Select

            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbAreaName(cmbModel.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblArea.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbAppliedArea.SelectedIndex = 0               '���C�x���g�����ӏ�
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblModel.Text)
        Finally
            LfWaitCursor(False)
        End Try


    End Sub

    '�u�K�p�G���A���́v�R���{�N���b�N
    Private Sub cmbAppliedArea_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbAppliedArea.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            '�v���O�����R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbProgram(cmbModel.SelectedValue.ToString, cmbAppliedArea.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblPrograme.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbPrgName.SelectedIndex = 0               '���C�x���g�����ӏ�

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblArea.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    '�u�v���O�������́v�R���{�N���b�N
    Private Sub cmbPrgName_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbPrgName.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            '�v���O�����R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbVer(cmbModel.SelectedValue.ToString, cmbAppliedArea.SelectedValue.ToString, cmbPrgName.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblVersion.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbVersion.SelectedIndex = 0               '���C�x���g�����ӏ�

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblPrograme.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    '�u�o�[�W�������́v�R���{�N���b�N
    Private Sub CmbVersion_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbVersion.SelectedIndexChanged
        If LbEventStop Then Exit Sub

        'Eltable�Ɋ��������f�[�^���N���A����B
        clearEltable(shtDspDelivery)

        '�{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B
        Call enableBtn()

        If cmbVersion.SelectedIndex = 0 Then
            Exit Sub
        Else
            '�{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B
            Call enableBtn(True, False)
        End If

    End Sub
#End Region

#Region "�������s��"
    ''' <summary>
    ''' �u�����v�{�^�����N���b�N���邱�Ƃɂ��A���������Ɉ�v����f�[�^����ʂɕ\������B
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�u�v���O�������́v�R���{�N���b�N���A�u�@�햼�́v�u�}�X�^���́v�u�p�^�[�����́v
    ''' �@�@�@�@�@�u�o�[�W�����v�����������Ƃ��āADB����z�M��̉w�̈ꗗ�y�єz�M�󋵂𒊏o����B
    ''' </remarks>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKensaku.Click
        Dim dtEltData As DataTable = Nothing

        LogOperation(sender, e)    '�{�^���������O
        Call Me.waitCursor(True)

        Try
            '�{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B
            Call enableBtn(True, False)

            clearEltable(shtDspDelivery)

            dtEltData = getEltableData(cmbModel.SelectedValue.ToString, cmbPrgName.SelectedValue.ToString.Substring(0, 3), _
                            cmbPrgName.SelectedValue.ToString.Substring(3, 3), cmbAppliedArea.SelectedValue.ToString, cmbVersion.Text)

            If dtEltData.Rows.Count <= 0 Then
                '���������Ɉ�v����f�[�^�͑��݂��܂���B
                AlertBox.Show(Lexis.NoRecordsFound)
                Exit Sub
            End If

            FillData(shtDspDelivery, dtEltData)

            '�{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B
            Call enableBtn(True, True)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            dtEltData = Nothing
            Call Me.waitCursor(False)
        End Try
    End Sub
#End Region

#Region "Eltable�̕\��"

    ''' <summary>
    ''' Eltable�p�̃f�[�^���擾����B
    ''' </summary>
    ''' <returns>Eltable�p�̃f�[�^</returns>
    ''' <remarks>Eltable�p�̃f�[�^���擾����B</remarks>
    Function getEltableData(ByVal sMdlCd As String, ByVal sKbn As String, ByVal sMstKind As String, _
                            ByVal sArea As String, ByVal sVerNo As String) As DataTable

        Dim dtReturn As DataTable
        Dim dbCtl As DatabaseTalker
        Dim sDllMdl As String
        Dim sSql As String

        Select Case sMdlCd
            Case "G"
                sDllMdl = "W"
            Case "Y"
                sDllMdl = "X"
            Case Else
                sDllMdl = "W"
        End Select
        '-------Ver0.1�@�t�F�[�Y�Q�@��M�������́h-�h�o�͑Ή���SQL���C���@MOD START-----------
        '  CASE" _
        '& "                 WHEN DELIVERY_STS = 0 AND DELIVERY_END_TIME IS NOT NULL AND DELIVERY_END_TIME <> '' THEN" _
        '& "                       SUBSTRING(DELIVERY_END_TIME,1,4)+'/'+SUBSTRING(DELIVERY_END_TIME,5,2)+'/'" _
        '& "                 +SUBSTRING(DELIVERY_END_TIME,7,2)+' '+SUBSTRING(DELIVERY_END_TIME,9,2)+':'" _
        '& "                 +SUBSTRING(DELIVERY_END_TIME,11,2)+':'+SUBSTRING(DELIVERY_END_TIME,13,2)" _
        '& "                 ELSE  '-'" _
        '& "             END AS END_TIME," _
        '-------Ver0.1�@�t�F�[�Y�Q�@��M�������́h-�h�o�͑Ή���SQL���C���@MOD END-------------

        sSql = "SELECT" _
        & "     CASE" _
        & "         WHEN LTRIM(Isnull(STR(DL_DATA.UNIT_NO),'')) = '' THEN DLL_DATA.STATION_NAME" _
        & "         ELSE DL_DATA.STATION_NAME" _
        & "     END AS STATION_NAME," _
        & "     CASE" _
        & "         WHEN LTRIM(Isnull(STR(DL_DATA.UNIT_NO),'')) = '' THEN DLL_DATA.CORNER_NAME" _
        & "         ELSE DL_DATA.CORNER_NAME" _
        & "     END AS CORNER_NAME," _
        & "     DLL_DATA.UNIT_NO,DLL_DATA.START_TIME,DLL_DATA.END_TIME,DLL_DATA.STS," _
        & "     LTRIM(Isnull(STR(DL_DATA.UNIT_NO),'')) AS UNIT_NO2," _
        & "     Isnull(DL_DATA.END_TIME,'') AS END_TIME2,Isnull(DL_DATA.STS,'') AS STS2" _
        & " FROM" _
        & "     (" _
        & "         SELECT" _
        & "             MAC.STATION_NAME,MAC.CORNER_NAME,DLL.UNIT_NO,DLL.START_TIME,DLL.END_TIME," _
        & "             DLL.STS,MAC.ADDRESS" _
        & "         FROM" _
        & "             (" _
        & "                 SELECT" _
        & "                     STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_NAME," _
        & "                     CORNER_CODE,MODEL_CODE,UNIT_NO,ADDRESS" _
        & "                 FROM" _
        & "                     V_MACHINE_NOW" _
        & "             ) AS MAC," _
        & "             (" _
        & "                 SELECT" _
        & "                     RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO," _
        & "                     SUBSTRING(DELIVERY_START_TIME,1,4)+'/'+SUBSTRING(DELIVERY_START_TIME,5,2)+'/'" _
        & "                     +SUBSTRING(DELIVERY_START_TIME,7,2)+' '+SUBSTRING(DELIVERY_START_TIME,9,2)+':'" _
        & "                     +SUBSTRING(DELIVERY_START_TIME,11,2)+':'+SUBSTRING(DELIVERY_START_TIME,13,2)" _
        & "                     AS START_TIME," _
        & "                     CASE" _
        & "                         WHEN DELIVERY_END_TIME IS NULL" _
        & "                     OR  DELIVERY_END_TIME = '' THEN ''" _
        & "                     ELSE SUBSTRING(DELIVERY_END_TIME,1,4)+'/'+SUBSTRING(DELIVERY_END_TIME,5,2)+'/'" _
        & "                         +SUBSTRING(DELIVERY_END_TIME,7,2)+' '+SUBSTRING(DELIVERY_END_TIME,9,2)+':'" _
        & "                         +SUBSTRING(DELIVERY_END_TIME,11,2)+':'+SUBSTRING(DELIVERY_END_TIME,13,2)" _
        & "                     END AS END_TIME," _
        & "                     CASE DELIVERY_STS" _
        & "                         WHEN 0 THEN '����'" _
        & "                         WHEN 1 THEN '�ُ�'" _
        & "                         WHEN 2 THEN '�s���ް�'" _
        & "                         WHEN 3 THEN '��ѱ��'" _
        & "                         WHEN 65535 THEN '�z�M��'" _
        & "                     ELSE '['+LTRIM(STR(DELIVERY_STS))+']'" _
        & "                     END AS STS" _
        & "                 FROM" _
        & "                     S_PRG_DLL_STS" _
        & "                 WHERE" _
        & "                     RAIL_SECTION_CODE+STATION_ORDER_CODE<>'000000' AND MODEL_CODE='" & sDllMdl & "'" _
        & "                 AND FILE_KBN='" & sKbn & "' AND DATA_KIND='" & sMstKind & "'" _
        & "                 AND DATA_SUB_KIND='" & sArea & "' AND VERSION='" & sVerNo & "'" _
        & "             ) AS DLL" _
        & "         WHERE" _
        & "             MAC.RAIL_SECTION_CODE=DLL.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE=DLL.STATION_ORDER_CODE" _
        & "         AND MAC.CORNER_CODE=DLL.CORNER_CODE AND MAC.MODEL_CODE=DLL.MODEL_CODE" _
        & "         AND MAC.UNIT_NO=DLL.UNIT_NO" _
        & "     ) AS DLL_DATA" _
        & "     LEFT OUTER JOIN" _
        & "         (" _
        & "             SELECT" _
        & "                 MAC.STATION_NAME,MAC.CORNER_NAME,MAC.MONITOR_ADDRESS,DL2.UNIT_NO,DL2.END_TIME,DL2.STS" _
        & "             FROM" _
        & "                 (" _
        & "                     SELECT" _
        & "                         STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,CORNER_NAME," _
        & "                         MODEL_CODE,UNIT_NO,MONITOR_ADDRESS" _
        & "                     FROM" _
        & "                         V_MACHINE_NOW" _
        & "                 ) AS MAC," _
        & "                 (" _
        & "                     SELECT" _
        & "                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,DL.MODEL_CODE," _
        & "                         UNIT_NO," _
        & "             CASE" _
        & "                 WHEN DELIVERY_STS = 0 AND DELIVERY_END_TIME IS NOT NULL AND DELIVERY_END_TIME <> '' THEN" _
        & "                       SUBSTRING(DELIVERY_END_TIME,1,4)+'/'+SUBSTRING(DELIVERY_END_TIME,5,2)+'/'" _
        & "                 +SUBSTRING(DELIVERY_END_TIME,7,2)+' '+SUBSTRING(DELIVERY_END_TIME,9,2)+':'" _
        & "                 +SUBSTRING(DELIVERY_END_TIME,11,2)+':'+SUBSTRING(DELIVERY_END_TIME,13,2)" _
        & "                 ELSE  '-'" _
        & "             END AS END_TIME," _
        & "                         CASE" _
        & "                             WHEN ST.STS_NAME IS NULL THEN '['+LTRIM(STR(DL.DELIVERY_STS))+']'" _
        & "                             ELSE ST.STS_NAME" _
        & "                         END AS STS" _
        & "                     FROM" _
        & "                         S_PRG_DL_STS AS DL" _
        & "                         LEFT OUTER JOIN" _
        & "                             M_PRG_DL_DELIVERY_STS_NAME AS ST" _
        & "                         ON  ST.STS = DL.DELIVERY_STS" _
        & "                     WHERE" _
        & "                         ST.MODEL_CODE='" & sMdlCd & "' AND ST.FILE_KBN='" & sKbn & "'" _
        & "                     AND DL.MODEL_CODE='" & sMdlCd & "' AND DL.FILE_KBN='" & sKbn & "'" _
        & "                     AND DL.DATA_KIND='" & sMstKind & "' AND DL.VERSION='" & sVerNo & "'" _
        & "                 ) AS DL2" _
        & "             WHERE" _
        & "                 MAC.RAIL_SECTION_CODE=DL2.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE=DL2.STATION_ORDER_CODE" _
        & "             AND MAC.CORNER_CODE=DL2.CORNER_CODE AND MAC.MODEL_CODE=DL2.MODEL_CODE" _
        & "             AND MAC.UNIT_NO = DL2.UNIT_NO" _
        & "         ) AS DL_DATA" _
        & "     ON  DLL_DATA.ADDRESS = DL_DATA.MONITOR_ADDRESS"

        dbCtl = New DatabaseTalker

        Try

            dbCtl.ConnectOpen()
            dtReturn = dbCtl.ExecuteSQLToRead(sSql)

        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dtReturn
    End Function

    Private Sub FillData(ByVal target As GrapeCity.Win.ElTabelleSheet.Sheet, ByVal dtEltData As DataTable)
        target.Redraw = False
        '�T���v���f�[�^����͂���
        With target
            .DataSource = dtEltData
        End With

        target.Rows.SetAllRowsHeight(21)
        btnPrint.Enabled = True

        '��ʂ̑M����h���B
        target.Redraw = True
    End Sub
#End Region

#Region "�I���{�^��"
    '�u�I���v�{�^���N���b�N
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        LogOperation(sender, e)    '�{�^���������O
        Me.Close()
    End Sub
#End Region

#Region "�R���{�N���b�N�l��ݒ肷��"
    '�@�햼�̂�ݒ肷��
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
            If (Model <> "") Then
                dt = oMst.SelectTable(Model)
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbAppliedArea)
            cmbAppliedArea.SelectedIndex = -1
            If cmbAppliedArea.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function


    '�v���O������ݒ肷��
    Private Function setCmbProgram(ByVal Model As String, ByVal Area As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As ProgramMaster
        oMst = New ProgramMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If String.IsNullOrEmpty(Area) Then
                Area = ""
            End If
            If (Model <> "" AndAlso Area <> "") Then
                dt = oMst.SelectTable(Model, True)
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbPrgName)
            cmbPrgName.SelectedIndex = -1
            If cmbPrgName.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function

    Private Function setCmbVer(ByVal Model As String, ByVal Area As String, ByVal Program As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As ProgramVersionMaster
        oMst = New ProgramVersionMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If String.IsNullOrEmpty(Area) Then
                Area = ""
            End If
            If String.IsNullOrEmpty(Program) Then
                Program = ""
            End If
            If (Model <> "" AndAlso Area <> "" AndAlso Program <> "") Then
                dt = oMst.SelectTable(Model, Area, Program.Substring(0, 3), Program.Substring(3, 3))
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbVersion)
            cmbVersion.SelectedIndex = -1
            If cmbVersion.Items.Count <= 0 Then bRtn = False
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

#Region "�{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B"
    ''' <summary>
    ''' �{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B
    ''' </summary>
    ''' <param name="bKensaku">�u�� ���v�{�^��</param>
    ''' <param name="bPrint">�u�o �́v�{�^��</param>
    ''' <remarks></remarks>
    Private Sub enableBtn(Optional ByVal bKensaku As Boolean = False, Optional ByVal bPrint As Boolean = False)
        Me.btnKensaku.Enabled = bKensaku
        Me.btnPrint.Enabled = bPrint
        '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD START-----------
        Me.shtDspDelivery.Enabled = bPrint
        '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD END-----------
    End Sub

#End Region

#Region "ELTable�̏�����"
    ''' <summary>
    ''' ELTable�̏�����
    ''' </summary>
    ''' <param name="target"></param>
    ''' <remarks>Eltable�Ɋ��������f�[�^���N���A����B</remarks>
    Private Sub clearEltable(ByVal target As GrapeCity.Win.ElTabelleSheet.Sheet)

        'Eltable�̃J�����g�̍ő包��
        Dim sXYRange As String = ""

        '��ʂ̑M����h������
        shtDspDelivery.Redraw = False

        If shtDspDelivery.MaxRows > 0 Then
            'Eltable�̃J�����g�̍ő包�����擾����B
            sXYRange = "1:" & shtDspDelivery.MaxRows.ToString

            '�I�����ꂽ�G���A�̃f�[�^���N���A����B
            shtDspDelivery.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
        End If
        '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD START-----------
        Dim i As Integer
        '�\�[�g���̃N���A
        With shtDspDelivery
            For i = 0 To LcstMaxColCnt - 1
                .ColumnHeaders(i).Image = Nothing
                .Columns(i).BackColor = Color.Empty
            Next
        End With
        '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD END-----------
        shtDspDelivery.MaxRows = 0

        btnKensaku.Enabled = False
        btnPrint.Enabled = False

        '��ʂ̑M����h������
        shtDspDelivery.Redraw = True

    End Sub
#End Region
    '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD START-----------
#Region "�ꗗ�\�[�g"
    ''' <summary>
    ''' ElTable
    ''' </summary>
    Private Sub shtDspDeliveryColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)
        Static intCurrentSortColumn As Integer = -1
        Static bolColumn1SortOrder(63) As Boolean

        If LcstSortCol(e.Column) = -1 Then Exit Sub

        Try

            shtDspDelivery.BeginUpdate()

            '�O��I�����ꂽ��w�b�_�̏�����
            If intCurrentSortColumn > -1 Then
                '��w�b�_�̃C���[�W���폜����
                shtDspDelivery.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '��̔w�i�F������������
                shtDspDelivery.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '��̃Z���r������������
                shtDspDelivery.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
            End If

            '�I�����ꂽ��ԍ���ۑ�
            intCurrentSortColumn = e.Column

            '�\�[�g�����̔w�i�F��ݒ肷��
            shtDspDelivery.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
            '�\�[�g�����̃Z���r����ݒ肷��
            shtDspDelivery.Columns(intCurrentSortColumn).SetBorder( _
                New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                GrapeCity.Win.ElTabelleSheet.Borders.All)

            If bolColumn1SortOrder(intCurrentSortColumn) = False Then
                '��w�b�_�̃C���[�W��ݒ肷��
                shtDspDelivery.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(1)
                '�~���Ń\�[�g����
                Call SheetSort(shtDspDelivery, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Descending)
                '��̃\�[�g��Ԃ�ۑ�����
                bolColumn1SortOrder(intCurrentSortColumn) = True
            Else
                '��w�b�_�̃C���[�W��ݒ肷��
                shtDspDelivery.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(0)
                '�����Ń\�[�g����
                Call SheetSort(shtDspDelivery, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Ascending)
                '��̃\�[�g��Ԃ�ۑ�����
                bolColumn1SortOrder(intCurrentSortColumn) = False
            End If

            shtDspDelivery.EndUpdate()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' MouseMove
    ''' </summary>
    Private Sub shtDspDelivery_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
            '�}�E�X�J�[�\������w�b�_��ɂ���ꍇ
            If shtDspDelivery.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtDspDelivery.CrossCursor = Cursors.Default
            Else
                '�}�E�X�J�[�\��������ɖ߂�
                shtDspDelivery.CrossCursor = Nothing
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
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
    '-------Ver0.2�@�ꗗ�\�[�g�Ή��@ADD END-----------

#Region "���[�o��"
    '�u�o�́v�{�^���N���b�N
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
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
            cmbModel.Select()

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


    ''' <summary>
    ''' [�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 8
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
                .Cell("J1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("J2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B4").Value = OPMGFormConstants.EQUIPMENT_TYPE_NAME + cmbModel.Text.Trim + "   " _
                                  + APPLIED_AREANAME + cmbAppliedArea.Text.Trim
                .Cell("B5").Value = OPMGFormConstants.PRO_NAME + cmbPrgName.Text.Trim + "  " _
                                  + OPMGFormConstants.VERSION_STR + cmbVersion.Text.Trim
                .Cell("D7").Value = shtDspDelivery.ColumnHeaders(2, 0).Caption
                .Cell("H7").Value = shtDspDelivery.ColumnHeaders(6, 0).Caption

                ' �z�M�Ώۂ̃f�[�^�����擾���܂�
                nRecCnt = shtDspDelivery.MaxRows

                ' �f�[�^�����̌r���g���쐬
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                '�f�[�^�����̒l�Z�b�g
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtDspDelivery.Item(LcstPrntCol(x), y).Text
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
            Throw New OPMGException(ex)
        End Try
    End Sub
#End Region

End Class
