' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
'   0.1      2014/04/01  �@�@ ����  �@�ꗗ�\�[�g�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports System.IO
Imports GrapeCity.Win

''' <summary>
''' �}�X�^�z�M�󋵕\��
''' </summary>
''' <remarks>�}�X�^�Ǘ����j���[���A�u�z�M�󋵕\���v�{�^�����N���b�N���邱�Ƃɂ��A
''' �{��ʂ�\������B�m�F�������u�}�X�^���́v�u�p�^�[�����́v�u�o�[�W�����v��I�����A
''' �u�����v���N���b�N���邱�Ƃɂ��A���Y�f�[�^�̕\�����s���B</remarks>
Public Class FrmMstDispDelivery
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
    ' Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Friend WithEvents wbkWorkBook As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents lblPattern As System.Windows.Forms.Label
    Friend WithEvents lblMstName As System.Windows.Forms.Label
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents cmbVersion As System.Windows.Forms.ComboBox
    Friend WithEvents cmbPattern As System.Windows.Forms.ComboBox
    Friend WithEvents cmbMaster As System.Windows.Forms.ComboBox
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents lblModel As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents shtDspDelivery As GrapeCity.Win.ElTabelleSheet.Sheet
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMstDispDelivery))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.wbkWorkBook = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtDspDelivery = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.cmbVersion = New System.Windows.Forms.ComboBox()
        Me.cmbPattern = New System.Windows.Forms.ComboBox()
        Me.cmbMaster = New System.Windows.Forms.ComboBox()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.lblPattern = New System.Windows.Forms.Label()
        Me.lblMstName = New System.Windows.Forms.Label()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.wbkWorkBook.SuspendLayout()
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
        Me.pnlBodyBase.Controls.Add(Me.wbkWorkBook)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.cmbVersion)
        Me.pnlBodyBase.Controls.Add(Me.cmbPattern)
        Me.pnlBodyBase.Controls.Add(Me.cmbMaster)
        Me.pnlBodyBase.Controls.Add(Me.lblVersion)
        Me.pnlBodyBase.Controls.Add(Me.lblPattern)
        Me.pnlBodyBase.Controls.Add(Me.lblMstName)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnKensaku)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2014/05/30(��)  17:52"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'wbkWorkBook
        '
        Me.wbkWorkBook.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wbkWorkBook.Controls.Add(Me.shtDspDelivery)
        Me.wbkWorkBook.Location = New System.Drawing.Point(21, 84)
        Me.wbkWorkBook.Name = "wbkWorkBook"
        Me.wbkWorkBook.ProcessTabKey = False
        Me.wbkWorkBook.ShowTabs = False
        Me.wbkWorkBook.Size = New System.Drawing.Size(866, 476)
        Me.wbkWorkBook.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wbkWorkBook.TabIndex = 0
        '
        'shtDspDelivery
        '
        Me.shtDspDelivery.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtDspDelivery.Data = CType(resources.GetObject("shtDspDelivery.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtDspDelivery.Location = New System.Drawing.Point(1, 1)
        Me.shtDspDelivery.Name = "shtDspDelivery"
        Me.shtDspDelivery.Size = New System.Drawing.Size(847, 457)
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
        Me.cmbVersion.Location = New System.Drawing.Point(460, 50)
        Me.cmbVersion.Name = "cmbVersion"
        Me.cmbVersion.Size = New System.Drawing.Size(56, 21)
        Me.cmbVersion.TabIndex = 4
        '
        'cmbPattern
        '
        Me.cmbPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPattern.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbPattern.ItemHeight = 13
        Me.cmbPattern.Location = New System.Drawing.Point(153, 50)
        Me.cmbPattern.Name = "cmbPattern"
        Me.cmbPattern.Size = New System.Drawing.Size(170, 21)
        Me.cmbPattern.TabIndex = 3
        '
        'cmbMaster
        '
        Me.cmbMaster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMaster.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMaster.ItemHeight = 13
        Me.cmbMaster.Location = New System.Drawing.Point(460, 20)
        Me.cmbMaster.Name = "cmbMaster"
        Me.cmbMaster.Size = New System.Drawing.Size(243, 21)
        Me.cmbMaster.TabIndex = 2
        '
        'lblVersion
        '
        Me.lblVersion.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblVersion.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblVersion.Location = New System.Drawing.Point(366, 50)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(80, 18)
        Me.lblVersion.TabIndex = 91
        Me.lblVersion.Text = "�o�[�W����"
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPattern
        '
        Me.lblPattern.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPattern.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPattern.Location = New System.Drawing.Point(46, 50)
        Me.lblPattern.Name = "lblPattern"
        Me.lblPattern.Size = New System.Drawing.Size(92, 18)
        Me.lblPattern.TabIndex = 90
        Me.lblPattern.Text = "�p�^�[������"
        Me.lblPattern.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMstName
        '
        Me.lblMstName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMstName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMstName.Location = New System.Drawing.Point(366, 20)
        Me.lblMstName.Name = "lblMstName"
        Me.lblMstName.Size = New System.Drawing.Size(80, 18)
        Me.lblMstName.TabIndex = 89
        Me.lblMstName.Text = "�}�X�^����"
        Me.lblMstName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        Me.cmbModel.Size = New System.Drawing.Size(170, 21)
        Me.cmbModel.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(45, 20)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(91, 18)
        Me.lblModel.TabIndex = 98
        Me.lblModel.Text = "�@��"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmMstDispDelivery
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMstDispDelivery"
        Me.Text = "�^�p�[�� "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wbkWorkBook.ResumeLayout(False)
        CType(Me.shtDspDelivery, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "�e��錾�̈�"

    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    ''' �o�͗p�e���v���[�g�t�@�C����
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "�}�X�^�z�M��.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "�}�X�^�z�M��"

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "�}�X�^�z�M�󋵕\��"

    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private LcstMaxColCnt As Integer

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������ʏW�D�f�[�^�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3, 4, 5, 6, 7, 8}
    '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD START-----------
    ''' <summary>
    ''' �ꗗ�w�b�_�̃\�[�g�񊄂蓖��
    ''' �i�ꗗ�w�b�_�N���b�N���Ɋ��蓖�Ă�Ώۗ���`�B��ԍ��̓[�����΂�"-1"�̓\�[�g�ΏۊO�̗�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, -1, -1, -1, 4, -1, -1, 7, 8}
    '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD END-----------

    Private LbInitCallFlg As Boolean = False

#End Region

#Region "�t�H�[�����[�h"
    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmMstDispDelivery_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        LfWaitCursor()
        If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
            If InitFrmData() = False Then   '��������
                Me.Close()
                Exit Sub
            End If
        End If

        LfWaitCursor(False)

    End Sub
#End Region

#Region "�}�X�^�z�M�󋵕\����ʂ̃f�[�^����������"
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
            '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD START-----------
            '�V�[�g�̃w�b�_�I���C�x���g�̃n���h���ǉ�
            shtDspDelivery.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtDspDelivery.ColumnHeaders.HeaderClick, AddressOf Me.shtDspDeliveryColumnHeaders_HeadersClick
            '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD END-----------
            '�@�햼�̂�ݒ肷��B
            If setCmbModel() = False Then Exit Try
            cmbModel.SelectedIndex = 0            '�f�t�H���g�\������

            '�}�X�^���̂�ݒ肷��B
            If setCmbMst(Me.cmbModel.SelectedValue.ToString) = False Then Exit Try
            cmbMaster.SelectedIndex = 0            '�f�t�H���g�\������

            '�p�^�[�����̂�ݒ肷��B
            If setcmbPattern(cmbModel.SelectedValue.ToString, cmbMaster.SelectedValue.ToString) = False Then Exit Try
            cmbPattern.SelectedIndex = 0            '�f�t�H���g�\������

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


    ''' <summary>
    ''' �u�@�햼�́v�R���{�N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�I�����ꂽ�@��ɂ��āA�@��}�X�^�N���X���Y������}�X�^�̈ꗗ���擾����B</remarks>
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
                Case Else
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = " "
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = " "
            End Select

            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbMst(cmbModel.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMstName.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbMaster.SelectedIndex = 0               '���C�x���g�����ӏ�
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblModel.Text)
        Finally
            LfWaitCursor(False)
        End Try


    End Sub

#Region "�u�}�X�^���́v�R���{�N���b�N"
    ''' <summary>
    ''' �u�}�X�^���́v�R���{�N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�I�����ꂽ�}�X�^�ɂ��āA�}�X�^�p�^�[���}�X�^�N���X���Y������p�^�[���̈ꗗ���擾����B</remarks>
    Private Sub cmbMaster_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles cmbMaster.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setcmbPattern(cmbModel.SelectedValue.ToString, cmbMaster.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblPattern.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbPattern.SelectedIndex = 0               '���C�x���g�����ӏ�

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMstName.Text)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub
#End Region

#Region "�u�p�^�[�����́v�R���{�N���b�N"
    ''' <summary>
    ''' �u�p�^�[�����́v�R���{�N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�I�����ꂽ�}�X�^�A�p�^�[���ɂ��āA�}�X�^�o�[�W�����}�X�^�N���X���Y������o�[�W�����̈ꗗ���擾����B</remarks>
    Private Sub cmbPattern_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles cmbPattern.SelectedIndexChanged

        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbVer(cmbModel.SelectedValue.ToString, cmbMaster.SelectedValue.ToString, cmbPattern.SelectedValue.ToString) = False Then
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
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblPattern.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub
#End Region

#Region "�u�o�[�W�����v�R���{�N���b�N"
    ''' <summary>
    ''' �u�o�[�W�����v�R���{�N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�p�^�[�����̂�I�����邱�Ƃɂ�芈��������B
    ''' �I�����ꂽ�}�X�^�A�p�^�[���ɑΉ�����o�[�W�����̈ꗗ�y�сu�󔒁v��ݒ肷��B
    ''' </remarks>
    Private Sub cmbVersion_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles cmbVersion.SelectedIndexChanged
        If LbEventStop Then Exit Sub

        'Eltable�Ɋ��������f�[�^���N���A����B
        Call clearEltable(shtDspDelivery)

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

#Region "�u�����v�{�^���N���b�N"
    ''' <summary>
    ''' �u�����v�{�^���N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�u�����v�{�^�����N���b�N���邱�Ƃɂ��A���������Ɉ�v����f�[�^����ʂɕ\������B</remarks>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles btnKensaku.Click

        LogOperation(sender, e)    '�{�^���������O
        Try
            LfWaitCursor()
            Call showEltable()

        Catch ex As DatabaseException

            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)

        Finally

            Call Me.waitCursor(False)

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

#Region "�R���{�N���b�N�l��ݒ肷��"
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
            dt = oMst.SelectTable()
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
    ''' �}�X�^���̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <param name="Model"> �@��R�[�h</param>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���}�X�^���̂̈ꗗ�y�сu�󔒁v��ݒ肷��B</remarks>
    Private Function setCmbMst(ByVal Model As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New MasterMaster

        Try
            '�}�X�^���̃R���{�{�b�N�X�p�̃f�[�^���擾����B
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If Model <> "" Then
                dt = oMst.SelectTable2(Model)
            End If
            dt = oMst.SetSpace()

            bRtn = BaseSetMstDtToCmb(dt, cmbMaster)
            cmbMaster.SelectedIndex = -1
            If cmbMaster.Items.Count <= 0 Then bRtn = False

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
    ''' �p�^�[�����̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <param name="Model">�@��R�[�h</param>
    ''' <param name="Master">�}�X�^�敪</param>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���p�^�[�����̂̈ꗗ�y�сu�󔒁v��ݒ肷��B</remarks>
    Private Function setcmbPattern(ByVal Model As String, ByVal Master As String) As Boolean
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
            If (Model <> "" AndAlso Master <> "") Then
                dt = oMst.SelectTable(Model, Master.Substring(3, 3))
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbPattern)
            cmbPattern.SelectedIndex = -1
            If cmbPattern.Items.Count <= 0 Then bRtn = False
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
    ''' �o�[�W������ݒ肷��B
    ''' </summary>
    ''' <param name="Model">�@��</param>
    ''' <param name="Master">�}�X�^</param>
    ''' <param name="Pattern">�p�^�[��</param>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���o�[�W�����̈ꗗ�y�сu�󔒁v��ݒ肷��B</remarks>
    Private Function setCmbVer(ByVal Model As String, ByVal Master As String, ByVal Pattern As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As MasterVersionMaster
        oMst = New MasterVersionMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If String.IsNullOrEmpty(Master) Then
                Master = ""
            End If
            If String.IsNullOrEmpty(Pattern) Then
                Pattern = ""
            End If
            If (Model <> "" AndAlso Master <> "" AndAlso Pattern <> "") Then
                dt = oMst.SelectTable(Model, Master.Substring(3, 3), Master.Substring(0, 3), Pattern)
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
        '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD START-----------
        Me.shtDspDelivery.Enabled = bPrint
        '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD END-----------
    End Sub

#End Region

#Region "Eltable��\������"
    ''' <summary>
    ''' Eltable��\������
    ''' </summary>
    ''' <remarks>ELTable�̏������AEltable�p�̃f�[�^���擾����BEltable��\������B</remarks>
    Private Sub showEltable()
        Dim dtShow As DataTable

        Try

            '�{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B
            Call enableBtn(True, False)

            'ELTable�̏�����
            Call clearEltable(Me.shtDspDelivery)

            'Eltable�p�̃f�[�^���擾����B
            dtShow = Me.getEltableData(cmbModel.SelectedValue.ToString, cmbMaster.SelectedValue.ToString.Substring(0, 3), _
                                       cmbMaster.SelectedValue.ToString.Substring(3, 3), cmbPattern.SelectedValue.ToString, cmbVersion.Text)

            If dtShow.Rows.Count = 0 Then
                AlertBox.Show(Lexis.NoRecordsFound)
                Exit Sub
            End If

            'Eltable��\������B
            Call Me.fillData(Me.shtDspDelivery, dtShow)

            '�{�^���u�� ���v�A�u�o �́v�̗��p�\����ݒ肷��B
            Call enableBtn(True, True)

        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dtShow = Nothing
        End Try

    End Sub
#End Region

#Region "ELTable�̏�����"
    ''' <summary>
    ''' ELTable�̏�����
    ''' </summary>
    ''' <param name="shtTarget"></param>
    ''' <remarks>Eltable�Ɋ��������f�[�^���N���A����B</remarks>
    Private Sub clearEltable(ByVal shtTarget As GrapeCity.Win.ElTabelleSheet.Sheet)

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

        '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD START-----------
        Dim i As Integer
        '�\�[�g���̃N���A
        With shtDspDelivery
            For i = 0 To LcstMaxColCnt - 1
                .ColumnHeaders(i).Image = Nothing
                .Columns(i).BackColor = Color.Empty
            Next
        End With
        '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD END-----------

        shtTarget.MaxRows = 0

        '��ʂ̑M����h������
        shtTarget.Redraw = True

    End Sub
#End Region

#Region "Eltable�p�̃f�[�^���擾����B"
    ''' <summary>
    ''' Eltable�p�̃f�[�^���擾����B
    ''' </summary>
    ''' <returns>Eltable�p�̃f�[�^</returns>
    ''' <remarks>Eltable�p�̃f�[�^���擾����B</remarks>
    Function getEltableData(ByVal sMdlCd As String, ByVal sKbn As String, ByVal sMstKind As String, _
                            ByVal sPtnNo As String, ByVal sVerNo As String) As DataTable

        Dim dtReturn As DataTable
        Dim dbCtl As DatabaseTalker
        Dim sDllMdl As String
        Dim sSql As String

        If sMdlCd = "G" Then
            sDllMdl = "W"
        Else
            sDllMdl = "X"
        End If
        '-------Ver0.1�@�t�F�[�Y�Q�@��M�������́h-�h�o�͑Ή���SQL���C���@MOD START-----------
        '  CASE" _
        '& "                 WHEN DELIVERY_STS = 0 AND DELIVERY_END_TIME IS NOT NULL AND DELIVERY_END_TIME <> '' THEN" _
        '& "                       SUBSTRING(DELIVERY_END_TIME,1,4)+'/'+SUBSTRING(DELIVERY_END_TIME,5,2)+'/'" _
        '& "                 +SUBSTRING(DELIVERY_END_TIME,7,2)+' '+SUBSTRING(DELIVERY_END_TIME,9,2)+':'" _
        '& "                 +SUBSTRING(DELIVERY_END_TIME,11,2)+':'+SUBSTRING(DELIVERY_END_TIME,13,2)" _
        '& "                 ELSE  '-'" _
        '& "             END AS END_TIME," _
        '-------Ver0.1�@�t�F�[�Y�Q�@��M�������́h-�h�o�͑Ή���SQL���C���@MOD END-------------

        If sKbn = "LST" Then
            '�K�p���X�g�w��
            'DLL�z�M���ʂ̂�
            sSql = "SELECT" _
                & "     MAC.STATION_NAME,MAC.CORNER_NAME,DLL.UNIT_NO,DLL.START_TIME,DLL.END_TIME," _
                & "     DLL.STS,'-','-','-'" _
                & " FROM" _
                & "     (" _
                & "         SELECT" _
                & "             STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_NAME," _
                & "             CORNER_CODE,MODEL_CODE,UNIT_NO,ADDRESS" _
                & "         FROM" _
                & "             V_MACHINE_NOW" _
                & "     ) AS MAC," _
                & "     (" _
                & "         SELECT" _
                & "             RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE," _
                & "             UNIT_NO," _
                & "             SUBSTRING(DELIVERY_START_TIME,1,4)+'/'+SUBSTRING(DELIVERY_START_TIME,5,2)+'/'" _
                & "            +SUBSTRING(DELIVERY_START_TIME,7,2)+' '+SUBSTRING(DELIVERY_START_TIME,9,2)+':'" _
                & "            +SUBSTRING(DELIVERY_START_TIME,11,2)+':'+SUBSTRING(DELIVERY_START_TIME,13,2)" _
                & "             AS START_TIME," _
                & "                     CASE" _
                & "                         WHEN DELIVERY_END_TIME IS NULL" _
                & "                     OR  DELIVERY_END_TIME = '' THEN ''" _
                & "                     ELSE SUBSTRING(DELIVERY_END_TIME,1,4)+'/'+SUBSTRING(DELIVERY_END_TIME,5,2)+'/'" _
                & "                         +SUBSTRING(DELIVERY_END_TIME,7,2)+' '+SUBSTRING(DELIVERY_END_TIME,9,2)+':'" _
                & "                         +SUBSTRING(DELIVERY_END_TIME,11,2)+':'+SUBSTRING(DELIVERY_END_TIME,13,2)" _
                & "                     END AS END_TIME," _
                & "             CASE DELIVERY_STS" _
                & "                 WHEN 0 THEN '����'" _
                & "                 WHEN 1 THEN '�ُ�'" _
                & "                 WHEN 2 THEN '�s���ް�'" _
                & "                 WHEN 3 THEN '��ѱ��'" _
                & "                 WHEN 65535 THEN '�z�M��'" _
                & "                 ELSE '['+LTRIM(STR (DELIVERY_STS))+']'" _
                & "             END AS STS" _
                & "         FROM" _
                & "             S_MST_DLL_STS" _
                & "         WHERE" _
                & "             RAIL_SECTION_CODE+STATION_ORDER_CODE<>'000000' AND MODEL_CODE='" & sDllMdl & "'" _
                & "         AND FILE_KBN='" & sKbn & "' AND DATA_KIND='" & sMstKind & "'" _
                & "         AND DATA_SUB_KIND='" & sPtnNo & "' AND VERSION='" & sVerNo & "'" _
                & "     ) AS DLL" _
                & " WHERE" _
                & "     MAC.RAIL_SECTION_CODE=DLL.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE=DLL.STATION_ORDER_CODE" _
                & " AND MAC.CORNER_CODE=DLL.CORNER_CODE AND MAC.MODEL_CODE=DLL.MODEL_CODE" _
                & " AND MAC.UNIT_NO=DLL.UNIT_NO"
        Else
            '�}�X�^�w��
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
                & "                     S_MST_DLL_STS" _
                & "                 WHERE" _
                & "                     RAIL_SECTION_CODE+STATION_ORDER_CODE<>'000000' AND MODEL_CODE='" & sDllMdl & "'" _
                & "                 AND FILE_KBN='" & sKbn & "' AND DATA_KIND='" & sMstKind & "'" _
                & "                 AND DATA_SUB_KIND='" & sPtnNo & "' AND VERSION='" & sVerNo & "'" _
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
                & "                         STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_NAME,CORNER_CODE," _
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
                & "                         S_MST_DL_STS AS DL" _
                & "                         LEFT OUTER JOIN" _
                & "                             M_MST_DL_DELIVERY_STS_NAME AS ST" _
                & "                         ON  ST.STS = DL.DELIVERY_STS" _
                & "                     WHERE" _
                & "                         ST.MODEL_CODE='" & sMdlCd & "' AND ST.FILE_KBN='" & sKbn & "'" _
                & "                     AND DL.MODEL_CODE='" & sMdlCd & "' AND DL.FILE_KBN='" & sKbn & "'" _
                & "                     AND DL.DATA_KIND='" & sMstKind & "' AND DL.DATA_SUB_KIND='" & sPtnNo & "'" _
                & "                     AND DL.VERSION='" & sVerNo & "'" _
                & "                 ) AS DL2" _
                & "             WHERE" _
                & "                 MAC.RAIL_SECTION_CODE=DL2.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE=DL2.STATION_ORDER_CODE" _
                & "             AND MAC.CORNER_CODE=DL2.CORNER_CODE AND MAC.MODEL_CODE=DL2.MODEL_CODE" _
                & "             AND MAC.UNIT_NO = DL2.UNIT_NO" _
                & "         ) AS DL_DATA" _
                & "     ON  DLL_DATA.ADDRESS = DL_DATA.MONITOR_ADDRESS"
        End If

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
#End Region

#Region "Eltable��ݒ肷��B"
    ''' <summary>
    ''' Eltable��ݒ肷��B
    ''' </summary>
    ''' <param name="target"></param>
    ''' <remarks>Eltable��ݒ肷��B</remarks>
    Private Sub fillData(ByRef target As GrapeCity.Win.ElTabelleSheet.Sheet, ByRef dtShow As DataTable)

        '��ʂ̑M����h���B
        target.Redraw = False

        '�T���v���f�[�^����͂���
        With target
            .DataSource = dtShow
        End With

        target.Rows.SetAllRowsHeight(21)

        '��ʂ̑M����h���B
        target.Redraw = True

    End Sub
#End Region
    '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD START-----------
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
    '-------Ver0.1�@�ꗗ�\�[�g�Ή��@ADD END-----------
#Region "���[�o��"
    ''' <summary>
    ''' �u�o�́v�{�^���N���b�N
    ''' </summary>
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
                .Cell("J1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("J2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.EQUIPMENT_TYPE_NAME + cmbModel.Text.Trim + "   " _
                                  + OPMGFormConstants.MST_NAME + cmbMaster.Text.Trim + "  " _
                                  + OPMGFormConstants.PATTERN_NAME + cmbPattern.Text.Trim + "  " _
                                  + OPMGFormConstants.VERSION_STR + cmbVersion.Text.Trim
                .Cell("D5").Value = shtDspDelivery.ColumnHeaders(2, 0).Caption
                .Cell("H5").Value = shtDspDelivery.ColumnHeaders(6, 0).Caption

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
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub
#End Region

End Class
