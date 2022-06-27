' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2013/11/11  (NES)����  �t�F�[�Y�Q�����Ή�
'   �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�C���|�[�g���G�N�X�|�[�g�@�\�ǉ�
'   0.2      2014/01/01       ����  �C���|�[�g���́h���h�`�F�b�N�ǉ�
' **********************************************************************
Option Explicit On
Option Strict On
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '�萔�l�̂ݎg�p
'-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD START-----------
Imports JR.ExOpmg.ClientApp.FMTStructure
'-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD END-------------
Imports System.IO
Imports System.Text
Imports GrapeCity.Win
Imports AdvanceSoftware.VBReport7.Xls
''' <summary>�h�c�}�X�^�ݒ�</summary>
''' <remarks>�ŏI�o�^�����ƑS���[�U����\������B</remarks>
Public Class FrmSysIDMst
    Inherits FrmBase

#Region "�������ݒ�"
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD START-----------
    '�����敪
    Protected Const PREMI_SYS As String = "1"
    Protected Const PREMI_ADMIN As String = "2"
    Protected Const PREMI_USUAL As String = "3"
    Protected Const PREMI_SYOSET As String = "4"
    '��������敪
    Protected Const PREMIT_ON As String = "1"
    Protected Const PREMIT_OFF As String = "0"

    '�ُ펖�R�R�[�h
    Protected Const ERRCODE1 As String = "�@ID�R�[�h�G���["
    Protected Const ERRCODE2 As String = "�@�p�X���[�h�G���["
    Protected Const ERRCODE3 As String = "�@�����G���["
    Protected Const ERRCODE4 As String = "�@���͒l�G���["
    Protected Const ERRCODE5 As String = "�@�V�X�e���Ǘ��Ҍ����G���["
    Protected Const ERRFst As String = "�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@"

    '�G���[���b�Z�[�W
    Protected Const MSGCODE1 As String = "�@�@�C���|�[�g�����@"
    Protected Const MSGCODE2 As String = "�@�G�N�X�|�[�g�����@"
    Protected Const MSGCODE3 As String = "�@�@�C���|�[�g���s�@"
    Protected Const MSGCODE4 As String = "�@�G�N�X�|�[�g���s�@"
    Protected Const MSGCODEFst As String = "�ُ�ڍ�"
    Protected Const MSGVer As String = "�@Ver."
    Protected Const MSGVer1 As String = "�@�@�@�@�@"
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD END-------------

#End Region
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
    Friend WithEvents istIDMst As System.Windows.Forms.ImageList
    Friend WithEvents wbkIDMst As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents lblTitleDate As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents btnAddNew As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents shtIDMst As GrapeCity.Win.ElTabelleSheet.Sheet
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD START-----------
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnExport As System.Windows.Forms.Button
    Friend WithEvents btnImport As System.Windows.Forms.Button
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD END-------------
    Friend WithEvents btnReturn As System.Windows.Forms.Button

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysIDMst))
        Me.wbkIDMst = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtIDMst = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.lblTitleDate = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.btnAddNew = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.istIDMst = New System.Windows.Forms.ImageList(Me.components)
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD START-----------
        Me.btnImport = New System.Windows.Forms.Button()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD END-------------
        Me.pnlBodyBase.SuspendLayout()
        Me.wbkIDMst.SuspendLayout()
        CType(Me.shtIDMst, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD START-----------
        Me.pnlBodyBase.Controls.Add(Me.btnExport)
        Me.pnlBodyBase.Controls.Add(Me.btnImport)
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD END-------------
        Me.pnlBodyBase.Controls.Add(Me.wbkIDMst)
        Me.pnlBodyBase.Controls.Add(Me.lblTitleDate)
        Me.pnlBodyBase.Controls.Add(Me.lblDate)
        Me.pnlBodyBase.Controls.Add(Me.btnAddNew)
        Me.pnlBodyBase.Controls.Add(Me.btnUpdate)
        Me.pnlBodyBase.Controls.Add(Me.btnDelete)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/11/01(��)  15:28"
        '
        'wbkIDMst
        '
        Me.wbkIDMst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wbkIDMst.Controls.Add(Me.shtIDMst)
        Me.wbkIDMst.Location = New System.Drawing.Point(124, 84)
        Me.wbkIDMst.Name = "wbkIDMst"
        Me.wbkIDMst.ProcessTabKey = False
        Me.wbkIDMst.ShowTabs = False
        Me.wbkIDMst.Size = New System.Drawing.Size(580, 525)
        Me.wbkIDMst.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wbkIDMst.TabIndex = 5
        '
        'shtIDMst
        '
        Me.shtIDMst.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtIDMst.Data = CType(resources.GetObject("shtIDMst.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtIDMst.Location = New System.Drawing.Point(1, 1)
        Me.shtIDMst.Name = "shtIDMst"
        Me.shtIDMst.Size = New System.Drawing.Size(561, 506)
        Me.shtIDMst.TabIndex = 99
        Me.shtIDMst.TabStop = False
        Me.shtIDMst.TransformEditor = False
        '
        'lblTitleDate
        '
        Me.lblTitleDate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblTitleDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitleDate.Location = New System.Drawing.Point(121, 40)
        Me.lblTitleDate.Name = "lblTitleDate"
        Me.lblTitleDate.Size = New System.Drawing.Size(145, 18)
        Me.lblTitleDate.TabIndex = 5
        Me.lblTitleDate.Text = "���ŏI�o�^�����F"
        Me.lblTitleDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDate
        '
        Me.lblDate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(266, 40)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(180, 18)
        Me.lblDate.TabIndex = 6
        Me.lblDate.Text = "2004�N07��20���@13:10"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnAddNew
        '
        Me.btnAddNew.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnAddNew.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAddNew.Location = New System.Drawing.Point(873, 320)
        Me.btnAddNew.Name = "btnAddNew"
        Me.btnAddNew.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD START-----------
        Me.btnAddNew.TabIndex = 2
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD END-----------
        Me.btnAddNew.Text = "�o  �^"
        Me.btnAddNew.UseVisualStyleBackColor = False
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnUpdate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(873, 386)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD START-----------
        Me.btnUpdate.TabIndex = 3
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD END-----------
        Me.btnUpdate.Text = "�C  ��"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDelete.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(873, 452)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD START-----------
        Me.btnDelete.TabIndex = 4
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD END-----------
        Me.btnDelete.Text = "��  ��"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(873, 518)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD START-----------
        Me.btnPrint.TabIndex = 5
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD END-----------
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
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD START-----------
        Me.btnReturn.TabIndex = 6
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@MOD END-----------
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'istIDMst
        '
        Me.istIDMst.ImageStream = CType(resources.GetObject("istIDMst.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.istIDMst.TransparentColor = System.Drawing.Color.White
        Me.istIDMst.Images.SetKeyName(0, "")
        Me.istIDMst.Images.SetKeyName(1, "")
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD START-----------
        '
        'btnImport
        '
        Me.btnImport.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnImport.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!)
        Me.btnImport.Location = New System.Drawing.Point(872, 183)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(128, 40)
        Me.btnImport.TabIndex = 0
        Me.btnImport.Text = "�C���|�[�g"
        Me.btnImport.UseVisualStyleBackColor = False
        '
        'btnExport
        '
        Me.btnExport.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnExport.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!)
        Me.btnExport.Location = New System.Drawing.Point(872, 251)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(128, 40)
        Me.btnExport.TabIndex = 1
        Me.btnExport.Text = "�G�N�X�|�[�g"
        Me.btnExport.UseVisualStyleBackColor = False
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD END-----------
        '
        'FrmSysIDMst
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysIDMst"
        Me.Text = " "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wbkIDMst.ResumeLayout(False)
        CType(Me.shtIDMst, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region


#Region "�錾�̈�iPrivate�j"

    '�v���p�e�B�ɒl��������ϐ��B
    ''' <summary>
    ''' ID�R�[�h
    ''' </summary>
    Private sUserid As String = ""
    ''''-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD START-----------
    '���O�C����ID
    Private sLoginID As String = ""
    '���O�C�����[�U����
    Private sAuth As String = ""
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD END-----------

    '�����ɑΉ�����萔�B
    ''' <summary>
    ''' ��ʎ�
    ''' </summary>
    Private Const AUTH_USUAL As String = "��ʎ�"

    ''' <summary>
    ''' �^�p�Ǘ���
    ''' </summary>
    Private Const AUTH_ADMIN As String = "�^�p�Ǘ���"

    ''' <summary>
    ''' �V�X�e���Ǘ���
    ''' </summary>
    Private Const AUTH_SYS As String = "�V�X�e���Ǘ���"

    '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD START-----------
    ''' <summary>
    ''' �ڍאݒ�
    ''' </summary>
    Private Const AUTH_DETTAILSET As String = "�ڍאݒ�"
    ''' <summary>
    ''' ��`���
    ''' </summary>
    ''' <remarks></remarks>
    Private infoObj() As FMTInfo = Nothing

    ''' <summary>
    ''' [���O�t�@�C���o�͐�f�B���N�g���w��p���ϐ���]
    ''' </summary>
    Private Const REG_LOG As String = "EXOPMG_LOG_DIR"

    ''' <summary>
    ''' CSV�f�[�^
    ''' </summary>
    Private infoLst As New List(Of String())

    ''' <summary>
    ''' ���O���X�g
    ''' </summary>
    Private LogLst As New ArrayList
    Private MSG As String = ""
    Private Ver00 As String = ""
    Private ErrCount As Integer = 0
    Private SumCount As Integer = 0
    ''' <summary>
    ''' �o�^ID�}�X�^���s
    ''' </summary>
    Private Const LcstIsMstError As String = "�o�^�����Ɏ��s���܂����B�ݒ�t�@�C���̓��e���m�F���Ă��������B"

    ''' <summary>
    ''' �t�@�C�����G���[
    ''' </summary>
    Private Const LcstCSVFileNameError As String = "�Ǎ��Ώۃt�@�C�����s���ł��B"

    ''' <summary>
    ''' �t�@�C���G���[
    ''' </summary>
    Private Const LcstCSVFileCheckError As String = "�Ǎ��Ώۃt�@�C�������݂��܂���B"


    ''' <summary>
    ''' �K�{�`�F�b�N
    ''' </summary>
    Private Const LcstMustCheck As String = "{0}�s�ڂ̃f�[�^���ځu{1}�v���K�{�ł��B"
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή��@ADD END-------------

    'ۯ���ĂɑΉ�����萔�B
    ''' <summary>
    ''' LOCK_STS = 0
    ''' </summary>
    Private Const LOCK_NOMAL As String = ""

    ''' <summary>
    ''' ���b�N��(LOCK_STS = 1)
    ''' </summary>
    Private Const LOCKING As String = "���b�N��"

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
    Private ReadOnly LcstXlsTemplateName As String = "�h�c�}�X�^�ݒ�.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "�h�c�}�X�^�ݒ�"

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "�h�c�}�X�^�ݒ�"

    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private nMaxColCnt As Integer

    ''' <summary>
    ''' �f�[�^���h*�h�ŏo��
    ''' </summary>
    Private Const LcstPwd As String = "'********"

    ''' <summary>
    ''' �ꗗ�w�b�_�̃\�[�g�񊄂蓖��
    ''' �i�ꗗ�w�b�_�N���b�N���Ɋ��蓖�Ă�Ώۗ���`�B��ԍ��̓[�����΂�"-1"�̓\�[�g�ΏۊO�̗�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {0, -1, 2, 3}

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������ʏW�D�f�[�^�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3}
#End Region
#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' �h�c�}�X�^�ݒ��ʂ̃f�[�^����������
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    ''' </summary>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        Dim sSql As String = ""
        Dim nRtn As Integer
        Dim dtMstTable As New DataTable
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e
        Try
            Log.Info("Method started.")

            '--��ʃ^�C�g��
            lblTitle.Text = LcstFormTitle

            '�V�[�g������
            shtIDMst.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtIDMst.ViewMode = ElTabelleSheet.ViewMode.Row
            shtIDMst.MaxRows = 0                                                 '�s�̏�����
            nMaxColCnt = shtIDMst.MaxColumns()                                '�񐔂��擾
            shtIDMst.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   '�V�[�g��\�����[�h
            '�V�[�g�̃w�b�_�I���C�x���g�̃n���h���ǉ�
            shtIDMst.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtIDMst.ColumnHeaders.HeaderClick, AddressOf Me.shtIDMstColumnHeaders_HeadersClick

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

            '�ꗗ�\�[�g�̏�����
            LfClrList()

            'Eltable�̂��ׂẴf�[�^���擾����B
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dtMstTable)

            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnAddNew.Select()
                Case 0
                    Me.btnUpdate.Enabled = False
                    Me.btnDelete.Enabled = False
                    Me.btnPrint.Enabled = False
                    AlertBox.Show(Lexis.NoIdCodeExists)    '�h�c�}�X�^��񂪓o�^����Ă��܂���B
                    bRtn = True
                    Return False
                Case Else
                    Me.btnUpdate.Enabled = True
                    Me.btnDelete.Enabled = True
                    Me.btnPrint.Enabled = True
                    Me.shtIDMst.Enabled = True
            End Select

            '�ŏI�o�^�������擾����B
            If GetDateTable() = -9 AndAlso Not nRtn = -9 Then
                AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            End If

            'Eltable�̓��e��\������B
            Call LfSetSheetData(dtMstTable)

            bRtn = True

        Catch ex As Exception
            '��ʕ\�������Ɏ��s���܂����B
            Log.Fatal("Unwelcome Exception caught.", ex)
            Me.btnAddNew.Select()
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
    ''' ���[�f�B���O�@���C���E�B���h�E
    ''' </summary>
    Private Sub FrmSysIDMst_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrmData() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If

            Me.btnAddNew.Focus()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' �u�o�^�v�{�^������������ƁA�h�c�f�[�^�o�^��ʂ��\�������B
    ''' </summary>
    Private Sub btnAddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddNew.Click
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim nRtn As Integer
        Dim sSql As String = ""
        Try
            LbEventStop = True
            LfWaitCursor()
            '�o�^�{�^�������B
            LogOperation(sender, e)

            Dim oFrmSysIDMstAdd As New FrmSysIDMstAdd

            oFrmSysIDMstAdd.ShowDialog()

            'TODO: Form.New���Ăяo���Ĉȍ~�ɗ�O�����������ꍇ�̂��Ƃ�
            '�l����ƁAFrmMntDispFaultDataDetail��ShowDialog���s���Ƃ��Ɠ��l��
            '���j�ɓ��ꂷ������悢��������Ȃ��B�i�t�ɂ����炪�����̉\��������j
            oFrmSysIDMstAdd.Dispose()

            'shtIDMst�X�V
            Call LfClrList() '�ꗗ�\�[�g�̏�����
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnAddNew.Select()
                Case 0
                    Me.btnUpdate.Enabled = False
                    Me.btnDelete.Enabled = False
                    Me.btnPrint.Enabled = False
                Case Else
                    Me.btnUpdate.Enabled = True
                    Me.btnDelete.Enabled = True
                    Me.btnPrint.Enabled = True
                    Me.shtIDMst.Enabled = True
            End Select

            Call LfSetSheetData(dt) '��ʕ\������
            shtIDMst.Enabled = True

            '�ŏI�o�^�������擾����B
            Select Case GetDateTable()
                Case -9
                    If Not nRtn = -9 Then
                        AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    End If
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'TODO: ���̂悤�ȃP�[�X�ŉ��L���s���ׂ����ۂ��A���j�𓝈ꂵ�Ȃ���΂Ȃ�Ȃ��B
            '���[�_����ShowDialog�̍Œ��ɔ���������O���{���ɂ����ɓ��B����Ȃ�A
            '���̉ӏ����A����������ŁAInitFrm�œ��l�̃��b�Z�[�W�{�b�N�X�\����
            '�s��Ȃ��悤�ɂ�������悢��������Ȃ��B
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
            LbEventStop = False
        End Try
    End Sub

    ''' <summary>
    ''' �u�C���v�{�^������������ƁA�h�c�f�[�^�C����ʂ��\�������B
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer
        Try
            LbEventStop = True
            LfWaitCursor()
            '�C���{�^�������B
            LogOperation(sender, e)

            Dim oFrmSysIDMstUpdate As New FrmSysIDMstUpdate
            'FrmSysIDMstUpdate��ʂ̃v���p�e�B�ɒl��������B
            Dim nRowno As Integer = shtIDMst.ActivePosition.Row

            sUserid = Me.shtIDMst.Item(0, nRowno).Text

            '�o�^���[�U��ID���擾����B
            oFrmSysIDMstUpdate.Userid() = sUserid

            If oFrmSysIDMstUpdate.InitFrmData() = False Then
                oFrmSysIDMstUpdate = Nothing
                Call waitCursor(False)
                Exit Sub
            End If

            oFrmSysIDMstUpdate.ShowDialog()
            oFrmSysIDMstUpdate.Dispose()

            'shtIDMst�X�V
            Call LfClrList() '�ꗗ�\�[�g�̏�����
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnUpdate.Select()
                Case 0
                    Me.btnUpdate.Enabled = False
                    Me.btnDelete.Enabled = False
                    Me.btnPrint.Enabled = False
                Case Else
                    Me.btnUpdate.Enabled = True
                    Me.btnDelete.Enabled = True
                    Me.btnPrint.Enabled = True
                    Me.shtIDMst.Enabled = True
            End Select

            Call LfSetSheetData(dt) '��ʕ\������
            shtIDMst.Enabled = True

            '�ŏI�o�^�������擾����B
            Select Case GetDateTable()
                Case -9
                    If Not nRtn = -9 Then
                        AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    End If
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
            LbEventStop = False
        End Try
    End Sub

    ''' <summary>
    ''' �u�폜�v�{�^������������ƁA�h�c�f�[�^�폜��ʂ��\�������B
    ''' </summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer
        Try
            LbEventStop = True
            LfWaitCursor()
            '�폜�{�^�������B
            LogOperation(sender, e)
            Dim oFrmSysIDMstDelete As New FrmSysIDMstDelete
            'FrmSysIDMstDelete��ʂ̃v���p�e�B�ɒl��������B
            Dim sRowno As Integer = shtIDMst.ActivePosition.Row

            sUserid = Me.shtIDMst.Item(0, sRowno).Text

            oFrmSysIDMstDelete.Userid() = sUserid

            If oFrmSysIDMstDelete.InitFrmData() = False Then
                oFrmSysIDMstDelete = Nothing
                Call waitCursor(False)
                Exit Sub
            End If

            oFrmSysIDMstDelete.ShowDialog()
            oFrmSysIDMstDelete.Dispose()

            'shtIDMst�X�V
            Call LfClrList() '�ꗗ�\�[�g�̏�����
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnDelete.Select()
                Case 0
                    Me.btnUpdate.Enabled = False
                    Me.btnDelete.Enabled = False
                    Me.btnPrint.Enabled = False
                Case Else
                    Me.btnUpdate.Enabled = True
                    Me.btnDelete.Enabled = True
                    Me.btnPrint.Enabled = True
                    Me.shtIDMst.Enabled = True
            End Select

            Call LfSetSheetData(dt) '��ʕ\������
            shtIDMst.Enabled = True

            '�ŏI�o�^�������擾����B
            Select Case GetDateTable()
                Case -9
                    If Not nRtn = -9 Then
                        AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    End If
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' �u�o�́v�{�^�������������
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
            btnAddNew.Select()
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
    ''' �u�I���v�{�^������������ƁA�{��ʂ��I�������B
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '�I���{�^�������B
        LogOperation(sender, e)
        Me.Close()
    End Sub

    ''' <summary>
    ''' ELTable�̃N���b�N����
    ''' </summary>
    Private Sub shtIDMstColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)
        Static intCurrentSortColumn As Integer = -1
        Static bolColumn1SortOrder(63) As Boolean

        If LcstSortCol(e.Column) = -1 Then Exit Sub

        Try

            shtIDMst.BeginUpdate()

            '�O��I�����ꂽ��w�b�_�̏�����
            If intCurrentSortColumn > -1 Then
                '��w�b�_�̃C���[�W���폜����
                shtIDMst.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '��̔w�i�F������������
                shtIDMst.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '��̃Z���r������������
                shtIDMst.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
            End If

            '�I�����ꂽ��ԍ���ۑ�
            intCurrentSortColumn = e.Column

            '�\�[�g�����̔w�i�F��ݒ肷��
            shtIDMst.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
            '�\�[�g�����̃Z���r����ݒ肷��
            shtIDMst.Columns(intCurrentSortColumn).SetBorder( _
                New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                GrapeCity.Win.ElTabelleSheet.Borders.All)

            If bolColumn1SortOrder(intCurrentSortColumn) = False Then
                '��w�b�_�̃C���[�W��ݒ肷��
                shtIDMst.ColumnHeaders(intCurrentSortColumn).Image = istIDMst.Images(1)
                '�~���Ń\�[�g����
                Call SheetSort(shtIDMst, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Descending)
                '��̃\�[�g��Ԃ�ۑ�����
                bolColumn1SortOrder(intCurrentSortColumn) = True
            Else
                '��w�b�_�̃C���[�W��ݒ肷��
                shtIDMst.ColumnHeaders(intCurrentSortColumn).Image = istIDMst.Images(0)
                '�����Ń\�[�g����
                Call SheetSort(shtIDMst, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Ascending)
                '��̃\�[�g��Ԃ�ۑ�����
                bolColumn1SortOrder(intCurrentSortColumn) = False
            End If

            shtIDMst.EndUpdate()
            '�������ڍאݒ�̏ꍇ�A�C���y�э폜�{�^�����񊈐�
            Dim nRowno As Integer = shtIDMst.ActivePosition.Row
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
            sAuth = Me.shtIDMst.Item(2, nRowno).Text
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
            Call AuthCheck(sAuth)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' ELTable�̃}�E�X�̈ړ�����
    ''' </summary>
    Private Sub shtIDMst_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
            '�}�E�X�J�[�\������w�b�_��ɂ���ꍇ
            If shtIDMst.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtIDMst.CrossCursor = Cursors.Default
            Else
                '�}�E�X�J�[�\��������ɖ߂�
                shtIDMst.CrossCursor = Nothing
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' ELTable�̃\�[�g����
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

    ''' <summary>Eltable�̓��e��\������B</summary>
    ''' <param name="dtMstTable">���[�U�f�[�^</param >
    ''' <remarks>
    ''' �h�c�R�[�h,�p�X���[�h,����,ۯ���Ă�\������B
    ''' </remarks>
    Private Sub LfSetSheetData(ByVal dtMstTable As DataTable)

        '��ʂ̑M����h���B
        Me.shtIDMst.Redraw = False
        Me.wbkIDMst.Redraw = False
        Try
            Me.shtIDMst.MaxRows = dtMstTable.Rows.Count     '���o�������̍s���ꗗ�ɍ쐬

            Me.shtIDMst.DataSource = dtMstTable             '�f�[�^���Z�b�g

            shtIDMst.Rows.SetAllRowsHeight(21)              '�s�����𑵂���

            '�������ڍאݒ�̏ꍇ�A�C���y�э폜���{�^�����񊈐�
            Dim nRowno As Integer = shtIDMst.ActivePosition.Row
            sAuth = Me.shtIDMst.Item(2, nRowno).Text
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
            Call AuthCheck(sAuth)
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SheetProcAbnormalEnd)             '�ꗗ�\�������Ɏ��s���܂����B
            btnAddNew.Select()
        Finally
            'Eltable���ĕ\������B
            Me.shtIDMst.Redraw = True
            Me.wbkIDMst.Redraw = True
        End Try

    End Sub

    ''' <summary>
    ''' [�ꗗ�N���A]
    ''' </summary>
    Private Sub LfClrList()
        shtIDMst.Redraw = False
        wbkIDMst.Redraw = False
        Try
            Dim i As Integer
            '�\�[�g���̃N���A
            With shtIDMst
                For i = 0 To nMaxColCnt - 1
                    .ColumnHeaders(i).Image = Nothing
                    .Columns(i).BackColor = Color.Empty
                Next
            End With

            shtIDMst.DataSource = Nothing
            shtIDMst.MaxRows = 0

            If shtIDMst.Enabled = True Then shtIDMst.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            If btnDelete.Enabled = True Then btnDelete.Enabled = False
            If btnUpdate.Enabled = True Then btnUpdate.Enabled = False
        Finally
            wbkIDMst.Redraw = True
            shtIDMst.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' [�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 5
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
                .Cell("B1").Value = lblTitle.Text
                .Cell("H1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("H2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = Microsoft.VisualBasic.Right(lblTitleDate.Text, 7) + lblDate.Text
                .Cell("B5").Value = "ID�R�[�h"
                .Cell("C5").Value = "�p�X���[�h"
                .Cell("D5").Value = "���@��"
                .Cell("E5").Value = "���b�N�A�E�g"
                ' �z�M�Ώۂ̃f�[�^�����擾���܂�
                nRecCnt = shtIDMst.MaxRows

                ' �f�[�^�����̌r���g���쐬
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                '�f�[�^�����̒l�Z�b�g
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        '�f�[�^���h*�h�ŏo��
                        If x = 1 Then
                            .Pos(x + 1, y + nStartRow).Value = LcstPwd
                        Else
                            .Pos(x + 1, y + nStartRow).Value = shtIDMst.Item(LcstPrntCol(x), y).Text
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

    ''' <summary>
    ''' [�����pSELECT������擾]
    ''' </summary>
    ''' <returns>SELECT��</returns>
    Private Function LfGetSelectString() As String

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine(" SELECT USER_ID,PASSWORD, ")
            sBuilder.AppendLine(" CASE  AUTHORITY_LEVEL ")
            sBuilder.AppendLine(String.Format(" WHEN '1' THEN '{0}' ", AUTH_SYS))
            sBuilder.AppendLine(String.Format(" WHEN '2' THEN '{0}'", AUTH_ADMIN))
            sBuilder.AppendLine(String.Format(" WHEN '3' THEN '{0}' ", AUTH_USUAL))
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
            sBuilder.AppendLine(String.Format(" WHEN '4' THEN '{0}' ", AUTH_DETTAILSET))
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
            sBuilder.AppendLine(" ELSE '' END , ")
            sBuilder.AppendLine("CASE LOCK_STS ")
            sBuilder.AppendLine(String.Format(" WHEN '0' THEN '{0}' ", LOCK_NOMAL))
            sBuilder.AppendLine(String.Format(" WHEN '1' THEN '{0}' ", LOCKING))
            sBuilder.AppendLine(" ELSE '' END  ")
            sBuilder.AppendLine("  FROM M_USER  ")
            sBuilder.AppendLine("ORDER BY USER_ID ")
            sSQL = sBuilder.ToString()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

        Return sSQL

    End Function
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
    ''' <summary>
    ''' [CSV�o�͗pSELECT������擾]
    ''' </summary>
    ''' <returns>SELECT��</returns>
    Private Function CsvGetSelectString() As String

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine(" SELECT '/' + USER_ID , ")
            sBuilder.AppendLine(" '/' + PASSWORD,AUTHORITY_LEVEL,LOCK_STS, ")
            '�}�X�^�Ǘ����j���[
            sBuilder.AppendLine(" MST_FUNC1,MST_FUNC2,MST_FUNC3,MST_FUNC4,MST_FUNC5, ")
            '�v���O�����Ǘ����j���[
            sBuilder.AppendLine(" PRG_FUNC1,PRG_FUNC2,PRG_FUNC3,PRG_FUNC4,PRG_FUNC5, ")
            '�ێ�Ǘ����j���[
            sBuilder.AppendLine(" MNT_FUNC1,MNT_FUNC2,MNT_FUNC3,MNT_FUNC4,MNT_FUNC5, ")
            sBuilder.AppendLine(" MNT_FUNC6,MNT_FUNC7,MNT_FUNC8,MNT_FUNC9,MNT_FUNC10, ")
            '�V�X�e���Ǘ����j���[
            sBuilder.AppendLine(" SYS_FUNC1,SYS_FUNC2,SYS_FUNC3,SYS_FUNC4,SYS_FUNC5 ")
            sBuilder.AppendLine("  FROM M_USER  ")
            sBuilder.AppendLine("ORDER BY USER_ID ")
            sSQL = sBuilder.ToString()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

        Return sSQL

    End Function
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
    ''' <summary>
    ''' �ŏI�o�^�������擾����
    ''' </summary>
    ''' <returns>�ŏI�o�^����</returns>
    Private Function GetDateTable() As Integer

        '�ŏI�o�^�������i�[����B
        Dim dtDateTable As New DataTable

        '���֐��̖߂�l
        Dim sLoginDate As String = ""

        Dim sSQL As String = ""

        Dim nRtn As Integer

        Dim dLastDate As DateTime = Nothing

        sSQL = " SELECT MAX(UPDATE_DATE)  FROM M_USER "

        Try
            nRtn = BaseSqlDataTableFill(sSQL, dtDateTable)
            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    Return nRtn
                Case 0
                    Return nRtn
            End Select

            '�ŏI�o�^�������i�[����B
            If dtDateTable IsNot Nothing AndAlso Convert.ToString(dtDateTable.Rows(0)(0)).Trim <> "" Then

                dLastDate = DateTime.Parse(dtDateTable.Rows(0).Item(0).ToString())
                sLoginDate = dLastDate.ToString("yyyy/MM/dd(ddd)  HH:mm")

            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            lblDate.Text = sLoginDate
        End Try

        Return nRtn

    End Function
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
    ''' <summary>
    ''' [�C���|�[�g����]
    ''' </summary>
    Private Sub btnImport_Click(sender As System.Object, e As System.EventArgs) Handles btnImport.Click
        Dim filePath As String = ""
        Dim oldFPath As String
        Dim newFPath As String
        Dim filenumber As Int32
        Dim strRead() As String                                     '�ݒ�t�@�C���̒[���h�c
        Dim j As Integer = 0
        Dim Errflg As Boolean = False
        Dim sSql As String = ""
        Dim nRtn As Integer
        Dim dtMstTable As New DataTable
        Dim Time As DateTime = Now

        Try
            Call waitCursor(True)
            '������
            ErrCount = 0
            SumCount = 0
            LogLst.Clear()
            infoLst.Clear()
            '�{�^�������f
            LogOperation(sender, e)
            OpenFileDialog1.Multiselect = False
            OpenFileDialog1.FileName = ""
            '�t�@�C����I��
            If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                '�Ǎ��Ώۃt�@�C�����`�F�b�N
                oldFPath = OpenFileDialog1.FileName
                filePath = oldFPath.Substring(0, oldFPath.LastIndexOf("\") + 1)
                newFPath = Path.Combine(filePath, Path.GetFileName(oldFPath))
                If (FileCheck(newFPath, oldFPath)) = False Then
                    Exit Sub
                End If
            Else
                Exit Sub
            End If
            'CSV�t�H�[�}�b�g��`�����擾����
            If GetDefineInfo(Config.IdMasterFormatFilePath, "FMT_IDMstConfig", infoObj) = False Then
                btnReturn.Select()
                Exit Sub
            End If
            'CSV�t�@�C�����A�f�[�^���擾����B
            filenumber = CShort(FreeFile())
            '�s���J�E���g
            Dim CLine As Integer = 0
            If System.IO.File.Exists(filePath + Path.GetFileName(OpenFileDialog1.FileName)) Then
                FileOpen(filenumber, filePath + Path.GetFileName(OpenFileDialog1.FileName), OpenMode.Binary, OpenAccess.Read)

                Do While Not EOF(1)
                    CLine += 1
                    strRead = Nothing
                    strRead = Split(LineInput(1), ",")
                    If (strRead(0).Substring(0, 1).ToString <> "#") Then
                        '�f�[�^�`�F�b�N
                        If (DataCheck(strRead, CLine)) = False Then
                            Errflg = True
                        End If
                    End If
                    infoLst.Add(strRead)
                Loop
                FileClose(1)
                '�f�[�^�������̃`�F�b�N
                If (ComCheck()) = False Then
                    Errflg = True
                End If
                If (Errflg = False) Then
                    'DB�X�V
                    If MuserImport() = False Then
                        '���s�����ꍇ�A�������I������
                        Exit Sub
                    End If

                    '�ꗗ�\�[�g�̏�����
                    LfClrList()
                    sSql = LfGetSelectString()
                    nRtn = BaseSqlDataTableFill(sSql, dtMstTable)
                    Call LfSetSheetData(dtMstTable) '��ʕ\������
                    shtIDMst.Enabled = True

                    '�ŏI�o�^�������擾����B
                    Select Case GetDateTable()
                        Case -9
                            If Not nRtn = -9 Then
                                AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                            End If
                    End Select
                Else
                    MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                              MSGCODE3 & ErrCount.ToString().PadLeft(4))
                    '���O�o��
                    LogLst.Insert(0, MSG)
                    If (WriteInExportLog(LogLst)) = False Then
                        AlertBox.Show(Lexis.IdMstImportlog)
                        Exit Sub
                    End If
                    AlertBox.Show(Lexis.IdMstImport)
                End If
            Else
                AlertBox.Show(Lexis.IdMstFileNotFound)
            End If

        Catch ex As IOException
            Log.Error("Exception caught.", ex)
            '�t�@�C���Ǎ����s���b�Z�[�W
            AlertBox.Show(Lexis.IdMstFileReadFailed)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                          MSGCODE3 & ErrCount.ToString().PadLeft(4))
            '���O�o��
            LogLst.Insert(0, MSG)
            If (WriteInExportLog(LogLst)) = False Then
                AlertBox.Show(Lexis.IdMstImportlog)
                Exit Sub
            End If
            AlertBox.Show(Lexis.IdMstImport)
        Finally
            FileClose(filenumber)
            infoObj = Nothing
            Call waitCursor(False)
        End Try
    End Sub
    ''' <summary>
    ''' [�G�N�X�|�[�g����]
    ''' </summary>
    Private Sub btnExport_Click(sender As System.Object, e As System.EventArgs) Handles btnExport.Click
        If LbEventStop Then Exit Sub
        Dim ofd As New SaveFileDialog()
        'CSV�t�@�C���ɏ������ނƂ��Ɏg��Encoding
        Dim enc As System.Text.Encoding = _
       System.Text.Encoding.GetEncoding("Shift_JIS")
        Dim sSql As String = ""
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim FileType, Prompt As String
        Dim Filepath As String
        Dim colCount As Integer = dt.Columns.Count
        Dim i As Integer
        Dim ExHdObj As New ArrayList
        Dim Time As DateTime = Now

        Try
            Call waitCursor(True)
            SumCount = 0
            LogLst.Clear()
            '�w�b�_�[����`
            ExHdObj.Add("#�^�p�Ǘ��V�X�e���@�ڍ׃f�[�^,,,,,,,,,,,,,,,,,,,,,,,,,,,,,")
            ExHdObj.Add("Ver,/0000,,,,,,,,,,,,,,,,,,,,,,,,,,,,")
            ExHdObj.Add("#,,,,�ڍאݒ�,,,,,,,,,,,,,,,,,,,,,,,,,")
            ExHdObj.Add("#,,,,�}�X�^�Ǘ����j���[,,,,,�v���O�����Ǘ����j���[,,,,,�ێ�Ǘ����j���[,,,,,,,,,,�V�X�e���Ǘ����j���[,,,,,")
            ExHdObj.Add("#�h�c�R�[�h,�p�X���[�h,����,���b�N�A�E�g,�O���}�̎捞,�}�X�^�K�p���X�g�捞,�z�M�w���ݒ�,�z�M�󋵕\��,�o�[�W�����\��,�O���}�̎捞,�v���O�����K�p���X�g�捞,�z�M�w���ݒ�,�z�M�󋵕\��,�o�[�W�����\��,�ʏW�D�f�[�^�m�F,�s����Ԍ��o�f�[�^�m�F,���s�˔j���o�f�[�^�m�F,���������o�f�[�^�m�F,�ُ�f�[�^�m�F,�ғ��E�ێ�f�[�^�o��,�@��ڑ���Ԋm�F,�Ď��Րݒ���,���W�f�[�^�m�F,���ԑѕʏ�~�f�[�^�o��,ID�}�X�^�ݒ�,�ғ��E�ێ�f�[�^�ݒ�,�p�^�[���ݒ�,�G���A�ݒ�,�^�ǐݒ�Ǘ�,�R�����g��")

            ofd.FileName = "ID�}�X�^.csv"
            FileType = "CSV ̧�� (*.csv),*.csv"
            Prompt = "�ۑ����I�����Ă�������"
            SumCount = 0
            If ofd.ShowDialog() = DialogResult.OK Then
                Filepath = ofd.FileName
                Dim sw As New System.IO.StreamWriter(Filepath, False, enc)
                sSql = CsvGetSelectString()
                nRtn = BaseSqlDataTableFill(sSql, dt)
                '���R�[�h����������
                '�w�b�_�[��񏑂�����
                For i = 0 To ExHdObj.Count - 1
                    sw.Write(ExHdObj(i).ToString)
                    sw.Write(vbCrLf)
                Next
                '�f�[�^����������
                Dim row As DataRow
                For Each row In dt.Rows
                    For i = 0 To dt.Columns.Count - 1
                        '�t�B�[���h�̎擾
                        Dim field As String = row(i).ToString()
                        '�t�B�[���h����������
                        sw.Write(field)
                        '�J���}����������
                        If dt.Columns.Count - 1 >= i Then
                            sw.Write(","c)
                        End If
                    Next
                    SumCount = SumCount + 1
                    '���s����
                    sw.Write(vbCrLf)
                Next

                '����
                sw.Close()
                MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer1 & MSGCODE2 & SumCount.ToString().PadLeft(4))
                '���O�o�͐��큕�ُ�
                LogLst.Insert(0, MSG)
                If (WriteInExportLog(LogLst)) = False Then
                    AlertBox.Show(Lexis.IdMstImportlog)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer1 & MSGCODE4 & "�@�@")
            '���O�o�͐��큕�ُ�
            LogLst.Insert(0, MSG)
            If (WriteInExportLog(LogLst)) = False Then
                AlertBox.Show(Lexis.IdMstImportlog)
                Exit Sub
            End If
            Log.Fatal("Unwelcome Exception caught.", ex)
            '�G���[���b�Z�[�W
            AlertBox.Show(Lexis.IdMstExport)
        Finally
            Call waitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' ��`���̎擾
    ''' </summary>
    ''' <param name="fileName">INI�t�@�C����</param>
    ''' <param name="sectionName">�Z�N�V������</param>
    ''' <param name="infoObj">�擾�������ʂ�ۑ��p</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>INI�t�@�C�����ɂēd���t�H�[�}�b�g��`�����擾���A�ꎞ�ێ�����</remarks>
    Public Shared Function GetDefineInfo(ByVal fileName As String, _
                                         ByVal sectionName As String, _
                                         ByRef infoObj() As FMTStructure.FMTInfo) As Boolean
        Dim bRtn As Boolean = False

        Dim i As Integer = 0
        Dim strDefInfo As String = ""
        Dim strData() As String
        Try
            'CSV�t�H�[�}�b�g��`���`�F�b�N

            If File.Exists(fileName) = False Then
                AlertBox.Show(Lexis.IdMstFormatFileNotFound)
                Return bRtn
            End If

            For i = 1 To 9999
                strDefInfo = Constant.GetIni(sectionName, Format(i, "0000"), fileName)
                If strDefInfo <> "" Then
                    strData = strDefInfo.Split(CChar(","))

                    ReDim Preserve infoObj(i - 1)
                    '���ږ��́F���{�ꖼ�̂��擾�B�G���[���b�Z�[�W�Ɏg�p�B
                    infoObj(i - 1).KOMOKU_NAME = strData(0)
                    '����
                    infoObj(i - 1).IN_TURN = CInt(strData(1))
                    '�K�{
                    infoObj(i - 1).MUST = CBool(strData(2))

                    '�t�B�[���h�`��: �o�^���̌^
                    infoObj(i - 1).FIELD_FORMAT = strData(3)

                    '�f�[�^���F�o�^�Ώۂc�a��
                    If strData(4) = "" Then
                        infoObj(i - 1).DATA_LEN = 10
                    Else
                        infoObj(i - 1).DATA_LEN = CInt(strData(4))
                    End If


                    '�t�B�[���h��: �o�^�Ώۂc�a�t�B�[���h
                    infoObj(i - 1).FIELD_NAME = strData(5)
                Else
                    Exit For
                End If
            Next
            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        End Try

        Return bRtn

    End Function
    ''' <summary>
    ''' �擪�����`�F�b�N
    ''' </summary>
    ''' <param name="CodeName">�t�B�[���h��</param>
    Private Function FrastChar(ByRef CodeName As String) As Boolean
        If (CodeName.Substring(0, 1) <> "/") Then      '�擪�����`�F�b�N
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' �������`�F�b�N
    ''' </summary>
    ''' <param name="CodeName">�t�B�[���h��</param>
    ''' <param name="iRow">����</param>
    ''' <param name="AarrayCode">��`���</param>
    Private Function ByteCheck(ByRef CodeName As String, ByVal iRow As Integer, ByVal AarrayCode As FMTInfo) As Boolean
        '���[�UID�������`�F�b�N
        If AarrayCode.FIELD_NAME = "USER_ID" Then
            '8���ł͂Ȃ��̏ꍇ
            If (CodeName.Length <> AarrayCode.DATA_LEN) Then
                Log.Info(String.Format(LcstMustCheck, iRow, AarrayCode.KOMOKU_NAME))
                Return False
            End If
            '�p�X���[�h�̕������`�F�b�N
        ElseIf (AarrayCode.FIELD_NAME = "PASSWORD") Then
            If (CodeName.Length < 4) Or (CodeName.Length > AarrayCode.DATA_LEN) Then
                Return False
            End If
        End If
        Return True
    End Function
    ''' <summary>
    ''' �C���|�[�g�iDB�X�V�j
    ''' </summary>
    Private Function MuserImport() As Boolean
        Dim dbCtl As DatabaseTalker = New DatabaseTalker()
        Dim sCurTime As String
        Dim sBuilder As StringBuilder
        Dim vBuilder As StringBuilder
        Dim Time As DateTime = Now
        Dim j As Integer = 0
        Dim Errflg As Boolean = False
        Dim loginiD As String = Config.MachineName
        Dim i As Integer = 0
        Dim dbError As Boolean = False                  'db�ُ픭���n�m
        Try
            'shtIDMst�X�V
            'Call LfClrList() '�ꗗ�\�[�g�̏�����
            dbCtl.ConnectOpen()          '�N�l�N�V�������擾����B

            dbCtl.TransactionBegin()  '�g�����U�N�V�������J�n����B
            '�o�^�����̍쐬
            sCurTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")

            sBuilder = New StringBuilder

            '�r������
            sBuilder.AppendLine("SELECT * FROM M_USER WITH( TABLOCK , XLOCK ) ")
            dbCtl.ExecuteSQLToWrite(sBuilder.ToString)

            '�e�[�u���N���A
            sBuilder.AppendLine("delete FROM M_USER ")
            dbCtl.ExecuteSQLToWrite(sBuilder.ToString)

            For i = 5 To infoLst.Count - 1
                '-------Ver0.2�@"#"�`�F�b�N�Ή��@ADD START-----------
                If infoLst(i)(0).Substring(0, 1).ToString <> "#" Then
                    '-------Ver0.2�@"#"�`�F�b�N�Ή��@ADD END-----------
                    SumCount = SumCount + 1
                    sBuilder = New StringBuilder
                    vBuilder = New StringBuilder

                    vBuilder.AppendLine("values(")

                    'ID�}�X�^�̓o�^
                    sBuilder.AppendLine(" insert into M_USER (INSERT_DATE ,INSERT_USER_ID,INSERT_MACHINE_ID,UPDATE_DATE,UPDATE_USER_ID,UPDATE_MACHINE_ID")

                    vBuilder.AppendLine(String.Format("{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot(GlobalVariables.UserId)))
                    vBuilder.AppendLine(String.Format("{0}", Utility.SetSglQuot(loginiD)))
                    vBuilder.AppendLine(String.Format(",{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot(GlobalVariables.UserId)))
                    vBuilder.AppendLine(String.Format("{0}", Utility.SetSglQuot(loginiD)))
                    For j = 0 To infoObj.Length - 1

                        vBuilder.AppendLine(String.Format(",{0}", Utility.SetSglQuot(infoLst.Item(i)(j).ToString)))
                        sBuilder.AppendLine(String.Format(",{0}", infoObj(j).FIELD_NAME))
                    Next

                    vBuilder.Append(")")
                    sBuilder.Append(")")
                    sBuilder.AppendLine(vBuilder.ToString)

                    '�f�[�^����
                    dbCtl.ExecuteSQLToWrite(sBuilder.ToString)
                    '-------Ver0.2�@"#"�`�F�b�N�Ή��@ADD START-----------
                End If
                '-------Ver0.2�@"#"�`�F�b�N�Ή��@ADD END-----------
            Next
            '���O���X�g�ɏ��������Z�b�g
            If (Errflg = False) Then
                MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                       MSGCODE1 & SumCount.ToString().PadLeft(4))
            Else
                MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                       MSGCODE3 & ErrCount.ToString().PadLeft(4))
                AlertBox.Show(Lexis.IdMstImport)
            End If
            '���O�o�͐��큕�ُ�
            LogLst.Insert(0, MSG)
            If (WriteInExportLog(LogLst)) = False Then
                AlertBox.Show(Lexis.IdMstImportlog)
                Return False
            End If
            '�g�����U�N�V�������R�~�b�g����
            dbCtl.TransactionCommit()
            Return True
        Catch ex As Exception
            dbError = True
            infoLst = Nothing
            Log.Fatal(LcstIsMstError)
            MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                  MSGCODE3 & ErrCount.ToString().PadLeft(4))
            '���O�o��
            LogLst.Insert(0, MSG)
            If (WriteInExportLog(LogLst)) = False Then
                AlertBox.Show(Lexis.IdMstImportlog)
                Return False
            End If
            AlertBox.Show(Lexis.IdMstInsertFailed)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' ���[�U�A�p�X���[�h�`�F�b�N
    ''' </summary>
    ''' <param name="CodeName">�t�B�[���h��</param>
    ''' <param name="CLine">����</param>
    ''' <param name="AarrayCode">��`���</param>
    Private Function UsPsCheck(ByRef CodeName As String, ByVal CLine As Integer, ByVal AarrayCode As FMTInfo) As Boolean
        '���[�UID���󔒂łȂ��ꍇ
        If (CodeName <> "") Then
            '���[�UID�擪�����`�F�b�N
            If FrastChar(CodeName) = True Then
                CodeName = CodeName.Remove(0, 1)
                '�������`�F�b�N
                If (ByteCheck(CodeName, CLine, AarrayCode)) = False Then
                    Return False
                End If
                '�p�����`�F�b�N
                If (OPMGUtility.checkCharacter(CodeName)) = False Then
                    Return False
                End If
            Else
                Return False
            End If
        Else
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' �f�[�^�̐������`�F�b�N
    ''' </summary>
    Private Function ComCheck() As Boolean
        Dim i As Integer = 0
        Dim a As Integer = 0
        Dim Permiflg As Boolean = False
        Dim Errflg As Boolean = False
        Dim Authflg As Boolean = False
        Dim Pcount As Integer = 0
        Dim Ecount As Integer = 0

        '�f�[�^���Ń��[�v���A���O�C�������[�UID�̑��݃`�F�b�N
        For i = 5 To infoLst.Count - 1
            '-------Ver0.2�@"#"�`�F�b�N�Ή��@ADD START-----------
            If infoLst(i)(0).Substring(0, 1).ToString <> "#" Then
                '-------Ver0.2�@"#"�`�F�b�N�Ή��@ADD END-----------
                '���쒆�̃��[�UID�`�F�b�N
                If (GlobalVariables.UserId.ToString = infoLst(i)(0).ToString) Then
                    Authflg = True
                    If (infoLst(i)(2).ToString = PREMI_SYS) Then
                        '���O�o��
                        Permiflg = True
                    Else
                        Permiflg = False
                        Pcount = i + 1
                    End If

                End If
                For a = 5 To infoLst.Count - 1
                    If (i <> a) Then
                        '���[�UID�̏d���`�F�b�N
                        If (infoLst(i)(0).ToString = infoLst(a)(0).ToString) Then
                            Errflg = True
                            Ecount = i + 1
                            '���O�o��
                            Exit For
                        End If
                    End If
                Next
                '-------Ver0.2�@"#"�`�F�b�N�Ή��@ADD START-----------
            End If
            '-------Ver0.2�@"#"�`�F�b�N�Ή��@ADD END-----------
        Next

        'ID�d���`�F�b�N
        If Errflg = True Then
            '���O�o��
            SetMSGSyousai(ERRFst, Ecount, ERRCODE4)
        End If
        '���쒆���[�U�����݂��Ȃ��ꍇ
        If (Authflg = False) Then
            '���O�o��
            SetMSGSyousai(ERRFst, 0, ERRCODE5)
        Else
            '���쒆���[�U���V�X�e���Ǘ��łȂ��ꍇ
            If (Permiflg = False) Then
                '���O�o��
                SetMSGSyousai(ERRFst, Pcount, ERRCODE5)
            End If
        End If

        If ((Errflg = True) Or (Permiflg = False) Or (Authflg = False)) Then
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' �t�@�C���`�F�b�N
    ''' </summary>
    ''' <param name="newFPath">�t�@�C����</param>
    ''' <param name="oldFPath ">�t�@�C����</param>
    Private Function FileCheck(ByRef newFPath As String, ByRef oldFPath As String) As Boolean
        If oldFPath <> newFPath Then
            Log.Error(LcstCSVFileNameError)
            AlertBox.Show(Lexis.TheFileNameIsUnsuitableForIdMst)
            btnReturn.Select()
            Return False
        End If
        ' �Ǎ��Ώۃt�@�C���`�F�b�N
        If File.Exists(newFPath) = False Then
            Log.Error(LcstCSVFileCheckError)
            AlertBox.Show(Lexis.IdMstFileNotFound)
            btnReturn.Select()
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' �f�[�^�`�F�b�N
    ''' </summary>
    ''' <param name="strRead">�ꃌ�R�[�h���̃f�[�^</param>
    ''' <param name="CLine">����</param>
    Private Function DataCheck(ByRef strRead() As String, ByVal CLine As Integer) As Boolean
        Dim j As Integer = 0

        '�w�b�_�[���擾
        If (CLine <= 5) Then
            If strRead(0).ToString = "Ver" Then
                '�o�[�W�����ԍ��擾
                If (strRead(1).ToString <> "") Then
                    '�o�[�W�����̐擪�������h/�h�̏ꍇ
                    If FrastChar(strRead(1)) = True Then
                        Ver00 = strRead(1).Substring(1)
                        '�o�[�W�������S���ȊO�̏ꍇ
                        If Ver00.Length <> 4 Then
                            SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                            Return False
                        End If
                        '�o�[�W�����ԍ��`�F�b�N�ُ�
                        If (OPMGUtility.checkNumber(Ver00)) = False Then
                            SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                            Return False
                        End If
                    Else
                        Ver00 = strRead(1).ToString
                        SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                        Return False
                    End If
                Else
                    SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                    Return False
                End If
            End If
            '�f�[�^��
        Else
            '���ڕʃ`�F�b�N
            For j = 0 To infoObj.Length - 1
                If (j = 0) Then
                    '���[�U�`�F�b�N
                    If (UsPsCheck(strRead(j), CLine, infoObj(j)) = False) Then
                        SetMSGSyousai(ERRFst, CLine, ERRCODE1)
                        Return False
                    End If
                ElseIf (j = 1) Then
                    'ID�`�F�b�N
                    If (UsPsCheck(strRead(j), CLine, infoObj(j)) = False) Then
                        SetMSGSyousai(ERRFst, CLine, ERRCODE2)
                        Return False
                    End If
                ElseIf (j > 2) Then
                    '���O�A�E�g����e���ڂ̑����`�F�b�N
                    If (strRead(j).ToString <> "") Then
                        '�t�B�[���h�`���`�F�b�N
                        If OPMGUtility.checkNumber(strRead(j)) = False Then
                            SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                            Return False
                        Else
                            '���̓`�F�b�N�G���[
                            If ((Integer.Parse(strRead(j)) <> 0) And (Integer.Parse(strRead(j)) <> 1)) Then
                                SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                                Return False
                            End If
                        End If
                    Else
                        SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                        Return False
                    End If
                ElseIf (j = 2) Then
                    '�t�B�[���h�`���`�F�b�N
                    If OPMGUtility.checkNumber(strRead(j)) = False Then
                        SetMSGSyousai(ERRFst, CLine, ERRCODE3)
                        Return False
                    Else
                        '�����`�F�b�N
                        If (infoObj(j).FIELD_NAME = "AUTHORITY_LEVEL") Then
                            If ((strRead(j).ToString <> PREMI_USUAL) And
                                    (strRead(j).ToString <> PREMI_ADMIN) And
                                    (strRead(j).ToString <> PREMI_SYS) And
                                    (strRead(j).ToString <> PREMI_SYOSET)) Then
                                '�����G���[
                                SetMSGSyousai(ERRFst, CLine, ERRCODE3)
                                Return False
                            End If
                        End If
                    End If
                End If
                '���ڐ���29�����̏ꍇ
                If strRead.Length - 1 <> infoObj.Length Then
                    SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                    Return False
                End If
            Next
        End If
        Return True
    End Function
    ''' <summary>
    ''' ���O�o��
    ''' </summary>
    '''<param name="MSG">���b�Z�[�W���X�g</param>
    Private Function WriteInExportLog(ByVal MSG As ArrayList, Optional ByVal ex As Exception = Nothing) As Boolean
        Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
        Dim enc As System.Text.Encoding = _
        System.Text.Encoding.GetEncoding("Shift_JIS")
        Dim i As Integer = 0
        Dim line As String = ""
        '���O�t�@�C���̃p�X�̎w�肪�Ȃ��󔒂̏ꍇ
        If sLogBasePath Is Nothing Then
            AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
        End If

        Try
            ' ���O�t�@�C�����쐬
            Dim logFile As String = sLogBasePath & "\" & Config.MachineKind & Config.MachineName & "_kengen" & ".log"
            If System.IO.File.Exists(logFile) Then
                Dim sw As StreamReader = New StreamReader(logFile, enc)
                Do While Not sw.Peek() = -1
                    line = sw.ReadLine()
                    MSG.Insert(i, line)
                    i += 1
                Loop
                sw.Close()
                sw = Nothing
                Dim sr As New System.IO.StreamWriter(logFile, False, enc)
                Try
                    '���O�����`�F�b�N
                    '10000�s�ȏ�̏ꍇ�A�ŐV��10000�s�̂ݏo��
                    If (MSG.Count - 1 > 10000) Then

                        For i = ((MSG.Count) - 10000) To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    Else
                        '10000�s�ȓ��̏ꍇ�A���ׂďo��
                        For i = 0 To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    End If
                    sr.Close()
                Catch ex2 As Exception
                    Return False
                Finally
                    If sr Is Nothing = False Then sr.Close()
                End Try
            Else
                Dim sr As New System.IO.StreamWriter(logFile, False, enc)
                Try
                    '���O�����`�F�b�N
                    '10000�s�ȏ�̏ꍇ�A�ŐV��10000�s�̂ݏo��
                    If (MSG.Count - 1 > 10000) Then

                        For i = ((MSG.Count - 1) - 10000) To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    Else
                        '10000�s�ȓ��̏ꍇ�A���ׂďo��
                        For i = 0 To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    End If
                    sr.Close()
                Catch ex2 As Exception
                    Return False
                Finally
                    If sr Is Nothing = False Then sr.Close()
                End Try
            End If
        Catch ex2 As Exception
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' �G���[���b�Z�[�W��`
    ''' </summary>
    '''<param name="ERRFst">���b�Z�[�W�ڍא擪�̃X�y�[�X</param>
    '''<param name="CLine">�G���[�s</param>
    '''<param name="ERR">�G���[���b�Z�[�W</param>
    Private Sub SetMSGSyousai(ByVal ERRFst As String, ByVal CLine As Integer, ByVal ERR As String)
            MSG = ERRFst & CLine.ToString().PadLeft(4) & ERR
            LogLst.Add(MSG)
            ErrCount = ErrCount + 1
    End Sub
    ''' <summary>
    ''' ���O�o��
    ''' </summary>
    '''<param name="MSG">���b�Z�[�W���X�g</param>
    Public Shared Sub WriteInExportLog(ByVal MSG As ArrayList, ByVal EnvVarNotFound As AlertBoxAttr, ByVal MachineKind As String, Optional ByVal ex As Exception = Nothing)
        Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
        Dim enc As System.Text.Encoding = _
        System.Text.Encoding.GetEncoding("Shift_JIS")
        Dim i As Integer = 0
        Dim line As String = ""
        '���O�t�@�C���̃p�X�̎w�肪�Ȃ��ꍇ
        If sLogBasePath Is Nothing Then
            AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
        End If

        Try
            ' ���O�t�@�C�����쐬
            Dim logFile As String = sLogBasePath & "\" & MachineKind & "_kengen" & ".log"
            If System.IO.File.Exists(logFile) Then
                Dim sw As StreamReader = New StreamReader(logFile, enc)
                Do While Not sw.Peek() = -1
                    line = sw.ReadLine()
                    MSG.Insert(i, line)
                    i += 1
                Loop
                sw.Close()
                sw = Nothing
                Dim sr As New System.IO.StreamWriter(logFile, False, enc)
                Try
                    '���O�����`�F�b�N
                    '10000�s�ȏ�̏ꍇ�A�ŐV��10000�s�̂ݏo��
                    If (MSG.Count - 1 > 10000) Then

                        For i = ((MSG.Count - 1) - 10000) To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    Else
                        '10000�s�ȓ��̏ꍇ�A���ׂďo��
                        For i = 0 To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    End If
                    sr.Close()
                Catch ex2 As Exception
                Finally
                    If sr Is Nothing = False Then sr.Close()
                End Try
            Else
                Dim sr As New System.IO.StreamWriter(logFile, False, enc)
                Try
                    '���O�����`�F�b�N
                    '10000�s�ȏ�̏ꍇ�A�ŐV��10000�s�̂ݏo��
                    If (MSG.Count - 1 > 10000) Then

                        For i = ((MSG.Count - 1) - 10000) To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    Else
                        '10000�s�ȓ��̏ꍇ�A���ׂďo��
                        For i = 0 To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    End If
                    sr.Close()
                Catch ex2 As Exception
                Finally
                    If sr Is Nothing = False Then sr.Close()
                End Try
            End If
        Catch ex2 As Exception
        End Try
    End Sub
    ''' <summary>
    ''' ���׍s�̌����`�F�b�N
    ''' </summary>
    '''<param name="sAuth">����</param>
    Private Sub AuthCheck(ByVal sAuth As String)
        If sAuth <> "" Then
            '�ڍאݒ�̏ꍇ�A�C���A�폜�{�^���̔񊈐���
            If sAuth = AUTH_DETTAILSET Then
                btnUpdate.Enabled = False
                btnDelete.Enabled = False
            Else
                btnUpdate.Enabled = True
                btnDelete.Enabled = True
            End If
        End If
    End Sub

    Private Sub shtIDMst_EnteredCell(sender As Object, e As System.EventArgs) Handles shtIDMst.EnteredCell
        '�������ڍאݒ�̏ꍇ�A�C���y�э폜���{�^�����񊈐�
        Dim nRowno As Integer = shtIDMst.ActivePosition.Row
        sAuth = Me.shtIDMst.Item(2, nRowno).Text
        Try
            Call AuthCheck(sAuth)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
#End Region
End Class
