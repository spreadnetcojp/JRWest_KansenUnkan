' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2013/12/09  (NES)����  �^�ǂƐؒf���ꂽ�ꍇ�̑Ή�
'   0.2      2013/12/14  (NES)����  �����Ɖ^�ǂ��ؒf���ꂽ�ꍇ�̑Ή�
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
''' �y�@��ڑ���Ԋm�F�@��ʃN���X�z
''' </summary>
Public Class FrmMntDispConStatus
    Inherits FrmBase

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B
        LcstSearchCol = {Me.cmbEki, Me.cmbMado}
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
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents pnlMado As System.Windows.Forms.Panel
    Friend WithEvents cmbMado As System.Windows.Forms.ComboBox
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents lblRefreshRate As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispConStatus))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.wkbMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.pnlMado = New System.Windows.Forms.Panel()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.lblRefreshRate = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.wkbMain.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlMado.SuspendLayout()
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
        Me.pnlBodyBase.Controls.Add(Me.lblRefreshRate)
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
        Me.lblToday.Text = "2013/12/18(��)  10:10"
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
        Me.wkbMain.Location = New System.Drawing.Point(13, 67)
        Me.wkbMain.Name = "wkbMain"
        Me.wkbMain.ProcessTabKey = False
        Me.wkbMain.ShowTabs = False
        Me.wkbMain.Size = New System.Drawing.Size(988, 483)
        Me.wkbMain.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wkbMain.TabIndex = 8
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(2, 2)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(967, 462)
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
        Me.btnPrint.TabIndex = 4
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
        Me.btnReturn.TabIndex = 5
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(873, 7)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 3
        Me.btnKensaku.Text = "���@��"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'pnlMado
        '
        Me.pnlMado.Controls.Add(Me.cmbMado)
        Me.pnlMado.Controls.Add(Me.lblMado)
        Me.pnlMado.Location = New System.Drawing.Point(241, 14)
        Me.pnlMado.Name = "pnlMado"
        Me.pnlMado.Size = New System.Drawing.Size(284, 33)
        Me.pnlMado.TabIndex = 2
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
        Me.pnlEki.Location = New System.Drawing.Point(9, 14)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(226, 33)
        Me.pnlEki.TabIndex = 1
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Items.AddRange(New Object() {"", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w", "�w�w�w�w�w�w�w�w"})
        Me.cmbEki.Location = New System.Drawing.Point(45, 6)
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
        'lblRefreshRate
        '
        Me.lblRefreshRate.AutoSize = True
        Me.lblRefreshRate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblRefreshRate.Location = New System.Drawing.Point(13, 596)
        Me.lblRefreshRate.Name = "lblRefreshRate"
        Me.lblRefreshRate.Size = New System.Drawing.Size(200, 16)
        Me.lblRefreshRate.TabIndex = 11
        Me.lblRefreshRate.Text = "���݁AZ9�����Ɏ����X�V��"
        '
        'FrmMntDispConStatus
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispConStatus"
        Me.Text = "�^�p�[�� Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlBodyBase.PerformLayout()
        Me.wkbMain.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlMado.ResumeLayout(False)
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
    Private ReadOnly LcstXlsTemplateName As String = "�@��ڑ���Ԋm�F.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "�@��ڑ���Ԋm�F"

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly FormTitle As String = "�@��ڑ���Ԋm�F"

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
    ''' �w���A�@��A�ŏI���W�����A�d���A�Ď��Ձi��j�A��iIC�j�A
    ''' �z�MSV�i��j�A�z�MSV�iIC�j�A����/EX�����A����/EX�����iDL�j�̃w�b�_���I���\�i�\�[�g�\�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, 0, -1, 13, -1, 5, 14, 15, 16, 17, 18, 19, 20, -1}

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������@��ڑ���Ԋm�F�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}

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


    ''' <summary>
    ''' �ʏ�d��OFF
    ''' </summary>
    Private Const LcstPowerOff As String = "�~"
    ''' <summary>
    ''' �ʏ�d��ON
    ''' </summary>
    Private Const LcstPowerOn As String = "��"
    ''' <summary>
    ''' �O�F����
    ''' </summary>
    Private Const LcstNormal As String = "0"
    ''' <summary>
    ''' �P�F�ُ�
    ''' </summary>
    Private Const LcstUnusual As String = "1"
    ''' <summary>
    ''' �ʏ�P�̓d��ON
    ''' </summary>
    Private Const LcstSinglePowerOn As String = "�P"
    ''' <summary>
    ''' ��ʈȊO
    ''' </summary>
    Private Const LcstOther As String = "-"
    ''' <summary>
    ''' �@��:��
    ''' </summary>
    Private Const LcstY As String = "Y"
    ''' <summary>
    ''' �@��:��
    ''' </summary>
    Private Const LcstG As String = "G"
    ''' <summary>
    ''' �X�V����
    ''' </summary>
    Private LcstTime As Integer
    ''' <summary>
    ''' �X�V
    ''' </summary>
    Private LcstRefreshRate As String = " ���݁A{0}�����Ɏ����X�V��"
    ''' <summary>
    ''' �J�n����
    ''' </summary>
    Private LcstSystemDate As DateTime
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
            shtMain.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row                      '�s�I�����[�h
            shtMain.MaxRows() = 0                                               '�s�̏�����
            LcstMaxColCnt = shtMain.MaxColumns()                                '�񐔂��擾
            '�V�[�g�̕\���I�����[�h��ݒ肷��
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   '�V�[�g��\�����[�h
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtMain.ColumnHeaders.HeaderClick, AddressOf Me.shtMainColumnHeaders_HeadersClick
            AddHandler Me.Timer1.Tick, AddressOf Me.btnKensaku_Click
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

            '�w�R���{�ݒ�
            BaseCtlEnabled(pnlEki)          '�w�R���{������
            LbEventStop = False '�C�x���g�����n�m
            '�e�R���{�{�b�N�X�̍��ړo�^
            If LfSetEki() = False Then Exit Try '�w���R���{�{�b�N�X�ݒ�
            cmbEki.SelectedIndex = 0            '�f�t�H���g�\������
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try '�R�[�i�[�R���{�{�b�N�X�ݒ�
            cmbMado.SelectedIndex = 0           '�f�t�H���g�\������

            LfClrList() '�ꗗ������
            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)        '�J�n�ُ탁�b�Z�[�W
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
    Private Sub FrmMntDispConnectionStatus_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrm() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If
            '�����{�^��������
            LfSearchTrue()
            LcstTime = Config.ConStatusDispRefreshRate
            Timer1.Interval = LcstTime * 60000
            Timer1.Enabled = True
            Timer1.Start()
            LcstSystemDate = System.DateTime.Now
            lblRefreshRate.Text = String.Format(LcstRefreshRate, LcstTime)
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
        LogOperation(sender, e)   '�{�^���������O
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
            Timer1.Stop()
            Timer1.Enabled = False
            LbEventStop = True
            LogOperation(sender, e)   '�{�^���������O
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
            Log.Fatal("Unwelcome Exception caught.", ex)       '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred)   '�������s���b�Z�[�W
            btnReturn.Select()
        Finally
            Dim ND As System.TimeSpan = System.DateTime.Now - LcstSystemDate
            LcstTime = Config.ConStatusDispRefreshRate
            Timer1.Interval += ND.Minutes
            Timer1.Enabled = True
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
            LfXlsStart2(sPath)
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

    '''<summary>
    ''' �u�w�v�R���{
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then
                If cmbMado.Enabled = True Then BaseCtlDisabled(pnlMado, False)
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                '�G���[���b�Z�[�W
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
            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
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


    '//////////////////////////////////////////////ElTable�֘A
    ''' <summary>
    ''' ElTable
    ''' </summary>
    Private Sub shtMainColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)
        Static intCurrentSortColumn As Integer = -1
        Static bolColumn1SortOrder(63) As Boolean

        If LcstSortCol(e.Column) = -1 Then Exit Sub

        Try

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

            shtMain.EndUpdate()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' MouseMove
    ''' </summary>
    Private Sub shtMain_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
            '�}�E�X�J�[�\������w�b�_��ɂ���ꍇ
            If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtMain.CrossCursor = Cursors.Default
            Else
                '�}�E�X�J�[�\��������ɖ߂�
                shtMain.CrossCursor = Nothing
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

#Region "���\�b�h�iPrivate�j"

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
        Dim bEnabled As Boolean = True
        If bEnabled Then
            If ((cmbEki.SelectedIndex < 0) OrElse _
                (cmbMado.SelectedIndex < 0)) Then
                bEnabled = False
            End If
        End If
        If bEnabled Then
            If btnKensaku.Enabled = False Then btnKensaku.Enabled = True
        Else
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
        End If
        '�����{�^��������
        Call LfSearchButton()
    End Sub
    ''' <summary>
    ''' �����{�^��������
    ''' </summary>
    Private Sub LfSearchButton()
        Dim bEnabled As Boolean = True
        For Each control As Control In LcstSearchCol
            If control.Enabled = False Then
                bEnabled = False
                Exit For
            End If
        Next
        If bEnabled Then
            btnKensaku.Enabled = True
        Else
            btnKensaku.Enabled = False
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
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [�����pSELECT������擾]
    ''' </summary>
    ''' <returns>SELECT��</returns>
    Private Function LfGetSelectString(ByVal slcSQLType As SlcSQLType) As String
        Dim sSql As String = ""
        Try
            Dim sSqlWhere As New StringBuilder
            Dim sBuilder As New StringBuilder
            Dim sEki As String
            sBuilder.AppendLine("")
            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '�����擾--------------------------
                    sBuilder.AppendLine(" SELECT COUNT(1) FROM V_CON_STATUS ")
                    '�擾����--------------------------
                Case slcSQLType.SlcDetail
                    '-----------Ver0.1�@�^�ǂƐؒf�Ή��@MOD START--------------------------------------------------------------
                    '---------�w�P�ʌ����Ή��@�@START----------------------------
                    sBuilder.AppendLine(" SELECT * FROM ( ")
                    '---------�w�P�ʌ����Ή��@�@END------------------------------
                    sBuilder.AppendLine("  SELECT STATION_CODE,STATION_NAME ,CORNER_NAME,MODEL_NAME,UNIT_NO   ")
                    sBuilder.AppendLine("  ,Convert(varchar(10),SYUSYU_DATE,111)+' '+Convert(varchar(8),SYUSYU_DATE,8) as SYUSYU_DATE  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when KAIDENGEN=null then '-'  ")
                    sBuilder.AppendLine("  		else '-' end)   ")
                    sBuilder.AppendLine("  	else '-' end ) As KAIDENGEN  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUDLCONNECT=2 then  ")
                    sBuilder.AppendLine("  		(case when  EXTOKATUCONNECT=2 OR EXTOKATUCONNECT=1 then  ")
                    sBuilder.AppendLine("  			(case when KANSICONNECT=2 then '��'  ")
                    sBuilder.AppendLine("  				when KANSICONNECT=1 then '�~'   ")
                    sBuilder.AppendLine("  			 else '-' end)  ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end)	 ")
                    sBuilder.AppendLine("    else '-' end ) As KANSICONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when SHUSECONNECT = null then '-'  ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end ) As SHUSECONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUDLCONNECT=2 then  ")
                    sBuilder.AppendLine("  		(case when EXTOKATUCONNECT=2 OR EXTOKATUCONNECT=1 then  ")
                    sBuilder.AppendLine("  			(case when HAISINSYUCONNECT=2 then '��'  ")
                    sBuilder.AppendLine("  				when HAISINSYUCONNECT=1 then '�~'   ")
                    sBuilder.AppendLine("  			else '-' end)  ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end )  ")
                    sBuilder.AppendLine("   else '-' end) As HAISINSYUCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when HAISINICMCONNECT = null then '-'   ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	else '-' end) As HAISINICMCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUCONNECT=2 then '��'  ")
                    sBuilder.AppendLine("  		when EXTOKATUCONNECT=1 then '�~'   ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	else '-' end) As EXTOKATUCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUDLCONNECT=2 then '��'  ")
                    sBuilder.AppendLine("      when EXTOKATUDLCONNECT=1 then '�~'   ")
                    sBuilder.AppendLine("      else '-' end)  ")
                    sBuilder.AppendLine("     else '-' end ) As EXTOKATUDLCONNECT  ")
                    sBuilder.AppendLine("  ,MODEL_CODE  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case when KAIDENGEN = null then -3  ")
                    sBuilder.AppendLine("  		else -3	 end)   ")
                    sBuilder.AppendLine("  	else -3 end ) As KAIDENGEN1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine("  	(case when EXTOKATUDLCONNECT=2 then  ")
                    sBuilder.AppendLine("  		(case when  EXTOKATUCONNECT=2 OR EXTOKATUCONNECT=1 then   ")
                    sBuilder.AppendLine(" 			 (case when KANSICONNECT=2 then -1   ")
                    sBuilder.AppendLine("  					when KANSICONNECT=1 then -4   ")
                    sBuilder.AppendLine("  			 else -3 end ) ")
                    sBuilder.AppendLine("  		 else -3 end ) ")
                    sBuilder.AppendLine("  	 else -3 end ) ")
                    sBuilder.AppendLine("   else -3  end)  As KANSICONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	 (case  when SHUSECONNECT = null then -3   ")
                    sBuilder.AppendLine(" 	  else -3 end) ")
                    sBuilder.AppendLine("    else -3  end) As SHUSECONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case when EXTOKATUDLCONNECT=2 then  ")
                    sBuilder.AppendLine("  		(case when EXTOKATUCONNECT=2 OR EXTOKATUCONNECT=1 then  ")
                    sBuilder.AppendLine(" 			(case when HAISINSYUCONNECT=2 then -1   ")
                    sBuilder.AppendLine("  				when HAISINSYUCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  			 else -3 end) ")
                    sBuilder.AppendLine("  		else -3 end) ")
                    sBuilder.AppendLine("  	else -3 end) ")
                    sBuilder.AppendLine("   else -3  end) As HAISINSYUCONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case  when HAISINICMCONNECT = null then -3   ")
                    sBuilder.AppendLine(" 	else -3 end) ")
                    sBuilder.AppendLine("    else -3 end)  As HAISINICMCONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case when EXTOKATUCONNECT=2 then -1   ")
                    sBuilder.AppendLine("  		when EXTOKATUCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  	else -3  end) ")
                    sBuilder.AppendLine("   else -3  end) As EXTOKATUCONNECT1   ")
                    sBuilder.AppendLine("  ,(case when CNT>0 then  ")
                    sBuilder.AppendLine(" 	(case when EXTOKATUDLCONNECT=2 then -1   ")
                    sBuilder.AppendLine(" 		when EXTOKATUDLCONNECT=1 then -4   ")
                    sBuilder.AppendLine("      else -3 end) ")
                    sBuilder.AppendLine("    else -3  end)  As EXTOKATUDLCONNECT1 ,CNT,CORNER_CODE  ")
                    sBuilder.AppendLine(" FROM  ")
                    '--------Ver 0.2 �C���O�@�@START-----------------------------------------------------------------
                    'sBuilder.AppendLine("   (select V_CON_STATUS.*,   ")
                    'sBuilder.AppendLine("   	(select COUNT(*)   ")
                    'sBuilder.AppendLine("   	    from S_DIRECT_CON_STATUS ds   ")
                    'sBuilder.AppendLine("  		where(V_CON_STATUS.STATION_CODE = ds.RAIL_SECTION_CODE + ds.STATION_ORDER_CODE)   ")
                    'sBuilder.AppendLine("   		and V_CON_STATUS.CORNER_CODE = ds.CORNER_CODE  and V_CON_STATUS.MODEL_CODE = ds.MODEL_CODE    ")
                    'sBuilder.AppendLine("   		and V_CON_STATUS.UNIT_NO = ds.UNIT_NO ) as CNT  ")
                    '--------Ver 0.2   �C���O�@�@END-----------------------------------------------------------------
                    '--------Ver 0.2   �C����@�@START---------------------------------------------------------------
                    sBuilder.AppendLine("   (select V_CON_STATUS.*,   ")
                    sBuilder.AppendLine("   	(select COUNT(*)   ")
                    sBuilder.AppendLine("   	    from  V_MACHINE_NOW  m1,V_MACHINE_NOW m2,S_DIRECT_CON_STATUS ds   ")
                    sBuilder.AppendLine("                      where(V_CON_STATUS.STATION_CODE = m1.RAIL_SECTION_CODE + m1.STATION_ORDER_CODE)   ")
                    sBuilder.AppendLine("   		and V_CON_STATUS.CORNER_CODE = m1.CORNER_CODE  and V_CON_STATUS.MODEL_CODE = m1.MODEL_CODE    ")
                    sBuilder.AppendLine("   		and V_CON_STATUS.UNIT_NO = m1.UNIT_NO    ")
                    sBuilder.AppendLine("   		and m1.MONITOR_ADDRESS = m2.ADDRESS    ")
                    sBuilder.AppendLine("   		and m2.MODEL_CODE = 'X'    ")
                    sBuilder.AppendLine("   		and m2.RAIL_SECTION_CODE = ds.RAIL_SECTION_CODE    ")
                    sBuilder.AppendLine("   		and m2.STATION_ORDER_CODE = ds.STATION_ORDER_CODE    ")
                    sBuilder.AppendLine("   		and m2.MODEL_CODE = ds.MODEL_CODE    ")
                    sBuilder.AppendLine("   		and m2.CORNER_CODE = ds.CORNER_CODE    ")
                    sBuilder.AppendLine("   		and m2.UNIT_NO = ds.UNIT_NO    ")
                    sBuilder.AppendLine("           and ds.PORT_KBN='1' ")
                    sBuilder.AppendLine("   	) as CNT   ")
                    '--------Ver 0.2   �C����@�@END--------------------------------------------------------------------
                    sBuilder.AppendLine("   from V_CON_STATUS  where V_CON_STATUS.MODEL_CODE ='Y' ) dt  ")
                    sBuilder.AppendLine("   UNION  ")
                    sBuilder.AppendLine("  SELECT STATION_CODE,STATION_NAME ,CORNER_NAME,MODEL_NAME,UNIT_NO   ")
                    sBuilder.AppendLine("  ,Convert(varchar(10),SYUSYU_DATE,111)+' '+Convert(varchar(8),SYUSYU_DATE,8) as SYUSYU_DATE  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when KAIDENGEN=1 then '��'  ")
                    sBuilder.AppendLine("  		when KAIDENGEN=2 then '�~'   ")
                    sBuilder.AppendLine("  		when KAIDENGEN=3 then '�P'  ")
                    sBuilder.AppendLine("  		else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end) As KAIDENGEN  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when KAIDENGEN=1 OR KAIDENGEN=3 then   ")
                    sBuilder.AppendLine("  		(case when KANSICONNECT=0 then '��'  ")
                    sBuilder.AppendLine("  			when KANSICONNECT=1 then '�~'   ")
                    sBuilder.AppendLine("  		 else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end)  ")
                    sBuilder.AppendLine("   else '-' end ) As KANSICONNECT  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0  then  ")
                    sBuilder.AppendLine("  		(case when SHUSECONNECT=0 then '��'  ")
                    sBuilder.AppendLine("  			when SHUSECONNECT=1 then '�~'   ")
                    sBuilder.AppendLine("  		 else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end)  ")
                    sBuilder.AppendLine("    else '-' end) As SHUSECONNECT  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3)and (SHUSECONNECT=0 OR SHUSECONNECT=1) and KANSICONNECT=0 then   ")
                    sBuilder.AppendLine(" 		(case when HAISINSYUCONNECT=0 then '��'  ")
                    sBuilder.AppendLine(" 			when HAISINSYUCONNECT=1 then '�~'   ")
                    sBuilder.AppendLine(" 		 else '-' end)  ")
                    sBuilder.AppendLine("  	 else '-' end)  ")
                    sBuilder.AppendLine("    else '-' end) As HAISINSYUCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0 and SHUSECONNECT=0 then  ")
                    sBuilder.AppendLine("  		(case when HAISINICMCONNECT=0 then '��'  ")
                    sBuilder.AppendLine("  			when HAISINICMCONNECT=1 then '�~'   ")
                    sBuilder.AppendLine("  		else '-' end )  ")
                    sBuilder.AppendLine("  	else '-' end )  ")
                    sBuilder.AppendLine("   else '-' end) As HAISINICMCONNECT  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0 and SHUSECONNECT=0 then  ")
                    sBuilder.AppendLine("  		(case when EXTOKATUCONNECT=0 then '��'  ")
                    sBuilder.AppendLine("  			when EXTOKATUCONNECT=1 then '�~'   ")
                    sBuilder.AppendLine("  		else '-' end )  ")
                    sBuilder.AppendLine("  	else '-' end)	  ")
                    sBuilder.AppendLine("   else '-' end) As EXTOKATUCONNECT  ")
                    sBuilder.AppendLine("  ,(case when EXTOKATUDLCONNECT = null then '-'  ")
                    sBuilder.AppendLine("       else '-' end) As EXTOKATUDLCONNECT  ")
                    sBuilder.AppendLine("  ,MODEL_CODE  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine(" 	(case when KAIDENGEN=1 then -1  ")
                    sBuilder.AppendLine("  		when KAIDENGEN=2 then -4   ")
                    sBuilder.AppendLine("  		when KAIDENGEN=3 then -2   ")
                    sBuilder.AppendLine("  		else -3 end ) ")
                    sBuilder.AppendLine("  	else -3  end) As KAIDENGEN1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when KAIDENGEN=1 OR KAIDENGEN=3 then   ")
                    sBuilder.AppendLine(" 		(case when KANSICONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when KANSICONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		else -3 end) ")
                    sBuilder.AppendLine("  	else -3 end)  ")
                    sBuilder.AppendLine("   else -3 end) As KANSICONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0  then  ")
                    sBuilder.AppendLine(" 		(case when SHUSECONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when SHUSECONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		else -3  end) ")
                    sBuilder.AppendLine("  	 else -3 end)	 ")
                    sBuilder.AppendLine("   else -3 end) As SHUSECONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3)and (SHUSECONNECT=0 OR SHUSECONNECT=1) and KANSICONNECT=0 then  ")
                    sBuilder.AppendLine(" 		(case when HAISINSYUCONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when HAISINSYUCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		else -3 end) ")
                    sBuilder.AppendLine("  	 else -3 end)   ")
                    sBuilder.AppendLine("   else -3 end) As HAISINSYUCONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0 and SHUSECONNECT=0 then ")
                    sBuilder.AppendLine(" 		(case when HAISINICMCONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when HAISINICMCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		else -3 end) ")
                    sBuilder.AppendLine("  	else -3 end )	  ")
                    sBuilder.AppendLine("   else -3 end) As HAISINICMCONNECT1  ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine("  	(case when (KAIDENGEN=1 OR KAIDENGEN=3) and KANSICONNECT=0 and SHUSECONNECT=0 then  ")
                    sBuilder.AppendLine(" 		(case when EXTOKATUCONNECT=0 then -1   ")
                    sBuilder.AppendLine("  			when EXTOKATUCONNECT=1 then -4   ")
                    sBuilder.AppendLine("  		 else -3  end ) ")
                    sBuilder.AppendLine("  	 else -3 end)  ")
                    sBuilder.AppendLine("    else -3 end) As EXTOKATUCONNECT1   ")
                    sBuilder.AppendLine("  ,(case when CT>0 then  ")
                    sBuilder.AppendLine(" 	(case when EXTOKATUDLCONNECT = null then -3   ")
                    sBuilder.AppendLine("     else -3  end ) ")
                    sBuilder.AppendLine("    else -3 end) As EXTOKATUDLCONNECT1,CT,CORNER_CODE  ")
                    sBuilder.AppendLine("   FROM   ")
                    sBuilder.AppendLine("   (select V_CON_STATUS.*,   ")
                    sBuilder.AppendLine("   	(select COUNT(*)   ")
                    sBuilder.AppendLine("   	    from  V_MACHINE_NOW  m1,V_MACHINE_NOW m2,S_DIRECT_CON_STATUS ds   ")
                    sBuilder.AppendLine("                      where(V_CON_STATUS.STATION_CODE = m1.RAIL_SECTION_CODE + m1.STATION_ORDER_CODE)   ")
                    sBuilder.AppendLine("   		and V_CON_STATUS.CORNER_CODE = m1.CORNER_CODE  and V_CON_STATUS.MODEL_CODE = m1.MODEL_CODE    ")
                    sBuilder.AppendLine("   		and V_CON_STATUS.UNIT_NO = m1.UNIT_NO    ")
                    sBuilder.AppendLine("   		and m1.MONITOR_ADDRESS = m2.ADDRESS    ")
                    sBuilder.AppendLine("   		and m2.MODEL_CODE = 'W'    ")
                    sBuilder.AppendLine("   		and m2.RAIL_SECTION_CODE = ds.RAIL_SECTION_CODE    ")
                    sBuilder.AppendLine("   		and m2.STATION_ORDER_CODE = ds.STATION_ORDER_CODE    ")
                    sBuilder.AppendLine("   		and m2.MODEL_CODE = ds.MODEL_CODE    ")
                    sBuilder.AppendLine("   		and m2.CORNER_CODE = ds.CORNER_CODE    ")
                    sBuilder.AppendLine("   		and m2.UNIT_NO = ds.UNIT_NO    ")
                    sBuilder.AppendLine("           and ds.PORT_KBN='1' ")
                    sBuilder.AppendLine("   	) as CT   ")
                    sBuilder.AppendLine("   from V_CON_STATUS  where V_CON_STATUS.MODEL_CODE ='G' ) ds  ")
                    '---------�w�P�ʌ����Ή��@�@START------------------------------
                    sBuilder.AppendLine(" ) as SELECTDATA ")
                    '---------�w�P�ʌ����Ή��@�@END--------------------------------
                    '-----------Ver0.1�@�^�ǂƐؒf�Ή��@MOD END----------------------------------------------------------------------------
            End Select

            'Where�吶��--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine("")
            sSqlWhere.AppendLine(" Where 0 = 0 ")

            '�w��
            If Not (cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sEki = cmbEki.SelectedValue.ToString
                If sEki.Substring(0, 3).Equals(LcstEkiSentou) Then
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE in {0})", _
                                                       String.Format("(SELECT DISTINCT(RAIL_SECTION_CODE + STATION_ORDER_CODE) AS STATION_CODE" _
                                                                     & " FROM M_MACHINE WHERE BRANCH_OFFICE_CODE = {0}) ", _
                                                                     Utility.SetSglQuot(sEki.Substring(sEki.Length - 3, 3)))))
                Else
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE = {0})", Utility.SetSglQuot(cmbEki.SelectedValue.ToString)))
                End If
            End If
            '�R�[�i�[��
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format("and (CORNER_CODE={0})", _
                                                   Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If

            If slcSQLType.Equals(slcSQLType.SlcDetail) Then
                sSqlWhere.AppendLine(" ORDER BY KAIDENGEN1 ,KANSICONNECT1 ,SHUSECONNECT1 ,HAISINSYUCONNECT1 ,HAISINICMCONNECT1 ,EXTOKATUCONNECT1,EXTOKATUDLCONNECT1 asc ")
            End If
            'Where�匋��
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
            If Not (shtMain.DataSource Is Nothing) Then
                shtMain.DataSource = Nothing
                shtMain.MaxRows = 0
            End If
            shtMain.MaxRows = dt.Rows.Count         '���o�������̍s���ꗗ�ɍ쐬
            shtMain.Rows.SetAllRowsHeight(21)       '�s�����𑵂���
            shtMain.DataSource = dt                 '�f�[�^���Z�b�g
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
    ''' [�o�͏���2]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart2(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 5
        Try

            With XlsReport1
                '�w�b�_�ҏW
                Log.Info("Start printing about [" & sPath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = sPath
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()
                '���[�t�@�C���V�[�g���̂��擾���܂��B
                .Page.Start(LcstXlsSheetName, "1-9999")

                ' ���o�����Z���֌��o���f�[�^�o��
                .Cell("B1").Value = lblTitle.Text
                .Cell("M1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("M2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim + "�@�@�@" + OPMGFormConstants.CORNER_STR + cmbMado.Text.Trim
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
        LbEventStop = True      '�C�x���g�����n�e�e
        Dim bRtn As Boolean = False
        Dim dt As DataTable = Nothing
        Dim oMst As StationMaster
        oMst = New StationMaster
        Try
            oMst.ApplyDate = ApplyDate
            dt = oMst.SelectTable(True, "G,Y")
            dt = oMst.SetAll()
            bRtn = BaseSetMstDtToCmb(dt, cmbEki)
            cmbEki.SelectedIndex = -1
            If cmbEki.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            LfCmbClear(cmbEki)
            LfCmbClear(cmbMado)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [�w��R���{������]
    ''' </summary>
    ''' <param name="cmb">�ΏۃR���{�{�b�N�X�R���g���[��</param>
    Private Sub LfCmbClear(ByVal cmb As ComboBox)
        Try
            cmb.DataSource = Nothing
            If cmb.Items.Count > 0 Then cmb.Items.Clear()
        Catch ex As Exception
        End Try
    End Sub

#End Region
End Class