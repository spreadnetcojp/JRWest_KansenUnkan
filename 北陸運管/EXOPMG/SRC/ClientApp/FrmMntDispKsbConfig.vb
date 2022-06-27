' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/06/01  (NES)����  �o�͍��ڊg���Ή�
'   0.2      2017/01/23  (NES)����@�VID���f���T�[�r�X�Ή�
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '�萔�l�̂ݎg�p
Imports JR.ExOpmg.DataAccess
Imports System
Imports System.IO
Imports System.Text
Imports GrapeCity.Win

''' <summary>
''' �y�Ď��Րݒ���@��ʃN���X�z
''' </summary>
Public Class FrmMntDispKsbConfig
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
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispKsbConfig))
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.wkbMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.pnlMado = New System.Windows.Forms.Panel()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblEki = New System.Windows.Forms.Label()
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
        Me.lblToday.Text = "2017/01/23(��)  10:21"
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
        Me.wkbMain.Location = New System.Drawing.Point(13, 66)
        Me.wkbMain.Name = "wkbMain"
        Me.wkbMain.ProcessTabKey = False
        Me.wkbMain.ShowTabs = False
        Me.wkbMain.Size = New System.Drawing.Size(988, 482)
        Me.wkbMain.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wkbMain.TabIndex = 8
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(2, 2)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(968, 462)
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
        Me.btnKensaku.Location = New System.Drawing.Point(873, 7)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 7
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
        Me.cmbMado.Size = New System.Drawing.Size(214, 21)
        Me.cmbMado.TabIndex = 1
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
        Me.cmbEki.Location = New System.Drawing.Point(41, 6)
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
        'FrmMntDispKsbConfig
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispKsbConfig"
        Me.Text = "�^�p�[�� Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
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
    Private LbInitCallFlg As Boolean = True

    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean
    '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD START-----------
    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private LcstXlsSheetName As String
    '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD END-----------
    ''' <summary>
    ''' �w�R�[�h�̐擪3��:�u000�v
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"

    ''' <summary>
    ''' Title���
    ''' </summary>
    Private Const FormTitle As String = "�Ď��Րݒ���"
    '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD START-----------
    '-------Ver0.2�@�VID���f���T�[�r�X�Ή��@ADD START---------
    ''' <summary>
    ''' �ꗗ�w�b�_�̃\�[�g�񊄂蓖��
    ''' �i�ꗗ�w�b�_�N���b�N���Ɋ��蓖�Ă�Ώۗ���`�B��ԍ��̓[�����΂�"-1"�̓\�[�g�ΏۊO�̗�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, -1, -1, -1, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                                                 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                                                 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 34,
                                                 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58,
                                                 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73,
                                                 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88,
                                                 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104,
                                                 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
                                                 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136,
                                                 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151,
                                                 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166,
                                                 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181,
                                                 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196,
                                                 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
                                                 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228,
                                                 229, 230, 231, 232, 233, 234, 235}

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������ʏW�D�f�[�^�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
                                                 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26,
                                                 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
                                                 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
                                                 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62,
                                                 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
                                                 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86,
                                                 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104,
                                                 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
                                                 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136,
                                                 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151,
                                                 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166,
                                                 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181,
                                                 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196,
                                                 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
                                                 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228,
                                                 229, 230, 231, 232, 233, 234, 235}
    '-------Ver0.2�@�VID���f���T�[�r�X�Ή��@ADD END-----------
    '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@�@END---------
    '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD START-----------
    '�O���[�v�ԍ�
    Private GrpNo As Integer = 0
    '���ʃR�[�i�[����t���O
    Private CorFlg As Boolean = False
    '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD �@�@END---------
    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private LcstMaxColCnt As Integer

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

            '�ꗗ�\�[�g�̏�����()
            LfClrList()
            '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD START---------
            LfListSet()
            '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD   END---------
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
    Private Sub FrmMntDispKsbConfig_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
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
            '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD START---------
            dt.Columns.Remove("CORNER_CODE")
            '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD �@END---------
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
            '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD START---------
            If CorFlg = True Then
                sPath = Path.Combine(sPath, Config.KsbConfigPrintDirect)
                LcstXlsSheetName = Config.KsbConfigPrintDirect.Substring(0, Config.KsbConfigPrintDirect.Length - 4)
            Else
                sPath = Path.Combine(sPath, Config.KsbPrintList(GrpNo).ToString)
                LcstXlsSheetName = Config.KsbPrintList(GrpNo).ToString.Substring(0, Config.KsbPrintList(GrpNo).ToString.Length - 4)
            End If

            '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@END---------

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

    '''<summary>
    ''' �u�w�v�R���{
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged

        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD �@START---------
            '���ʃR�[�i�[�t���O��������
            CorFlg = False
            Dim station As String = cmbEki.SelectedValue.ToString
            If (station <> "" And station <> ClientDaoConstants.TERMINAL_ALL) Then
                GrpNo = CInt(station.Substring(0, 1))
            ElseIf station = ClientDaoConstants.TERMINAL_ALL Then
                GrpNo = ClientDaoConstants.TERMINAL_ALL_GrpNo
            End If
            If LfSetMado(station.Substring(station.Length - 6, 6)) = False Then
                '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD �@END---------
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
            '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD START---------
            LfListSet()
            '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD   END---------
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
            LfSearchTrue()
            '-------Ver0.1�@���ʃR�[�i�[�Ή��@ADD START---------
            '�t���O�̃`�F�C���W���f
            Dim ChFlg As Boolean = CorFlg
            '���ʃR�[�i�[�̏ꍇ
            If CheckCorner() = True Then
                '���ʃR�[�i�[�`�F�b�N�ŁA�R�[�i�[�t���O���ύX���ꂽ�ꍇ
                If CorFlg <> ChFlg Then
                    LfListSet()
                End If
            Else
                '�R�[�i�[�t���O���ύX����ĂȂ��ꍇ
                If CorFlg <> ChFlg Then
                    LfListSet()
                End If
            End If
            '-------Ver0.1�@���ʃR�[�i�[�Ή��@ADD   END---------
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
        '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD START---------
        Static bolColumn1SortOrder(LcstMaxColCnt) As Boolean
        '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD START---------
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
    '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD �@START---------
    ''' <summary>
    ''' [�ꗗ����]
    ''' </summary>
    Private Sub LfListSet()
        Dim SetInfo As String = ""
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try
            '���ʃR�[�i�[���ʂ��A��������擾
            If CorFlg = False Then
                SetInfo = Config.KsbConfigOutListCol(GrpNo).ToString
            Else
                SetInfo = Config.KsbConfigOutListColDirect
            End If
            Dim i, a As Integer
            '�ꗗ�\������
            With shtMain
                '�S�Ă̗��\��
                For a = 0 To SetInfo.Length - 1
                    .Columns(a).Hidden = False
                Next
                '�ꗗ�\������
                For i = 0 To SetInfo.Length - 1
                    If SetInfo(i).ToString = "0" Then
                        .Columns(i).Hidden = True
                    End If
                Next
            End With
        Finally
            wkbMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub
    '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD �@  END---------

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
    End Sub

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
                '----------- 0.1  �o�͍��ڊg���Ή� MOD START------------------------
                dt = oMst.SelectTable(Station, "G")
                '----------- 0.1  �o�͍��ڊg���Ή� MOD END------------------------
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
        Dim sEki As String = ""
        Try
            Dim sSqlWhere As New StringBuilder
            Dim sBuilder As New StringBuilder
            '-------Ver0.2�@�VID���f���T�[�r�X�Ή��@MOD START---------
            sBuilder.AppendLine("")
            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '�����擾����--------------------------
                    sBuilder.AppendLine(" SELECT COUNT(1) FROM V_KSB_CONFIG2 ")
                Case slcSQLType.SlcDetail
                    '�擾����--------------------------
                    '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@START---------
                    sBuilder.AppendLine(" SELECT * ")
                    sBuilder.AppendLine(" FROM V_KSB_CONFIG2 ")
                    '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@END---------
                    Dim s As String = sBuilder.ToString
            End Select
            '-------Ver0.2�@�VID���f���T�[�r�X�Ή��@MOD END-----------

            'Where�吶��--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine("")
            sSqlWhere.AppendLine(" where 0 = 0 ")

            '�w
            If Not (cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sEki = cmbEki.SelectedValue.ToString
                '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@START---------
                If sEki.Substring(1, 3).Equals(LcstEkiSentou) Then

                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE in {0})", _
                                                       String.Format("(SELECT DISTINCT(RAIL_SECTION_CODE + STATION_ORDER_CODE) AS STATION_CODE" _
                                                                     & " FROM M_MACHINE WHERE BRANCH_OFFICE_CODE = {0}) ", _
                                                                     Utility.SetSglQuot(sEki.Substring(sEki.Length - 3, 3)))))
                Else
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE = {0})", Utility.SetSglQuot(sEki.Substring(sEki.Length - 6, 6))))
                End If
                '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@END---------
            End If
            '�R�[�i�[
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (CORNER_CODE = {0})", _
                                          Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If

            If slcSQLType.Equals(slcSQLType.SlcDetail) Then
                sSqlWhere.AppendLine(" ORDER BY STATION_CODE,CORNER_CODE,SYUSYU_DATE ASC ")
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
    ''' [�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart(ByVal sPath As String)

        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 7
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
                .Cell("B1").Value = lblTitle.Text
                '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD START---------
                '-------Ver0.2�@�VID���f���T�[�r�X�Ή��@MOD START---------
                .Cell("HZ1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("HZ2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                '-------Ver0.2�@�VID���f���T�[�r�X�Ή��@MOD END-----------
                '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@END---------
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim + "�@�@�@" +
                                    OPMGFormConstants.CORNER_STR + cmbMado.Text.Trim

                ' �z�M�Ώۂ̃f�[�^�����擾���܂�
                nRecCnt = shtMain.MaxRows

                '�r���g���쐬
                For i As Integer = 1 To shtMain.MaxRows - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                '���
                For i As Integer = 0 To shtMain.MaxRows - 1
                    For j As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(1 + j, i + nStartRow).Value = shtMain.Item(j + 1, i).Text
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
        Dim dt As DataTable
        Dim oMst As StationMaster
        oMst = New StationMaster
        Try
            oMst.ApplyDate = ApplyDate
            '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@START---------
            dt = oMst.SelectTable(True, "W", True)
            '-------Ver0.1�@�o�͍��ڊg���Ή��@MOD �@�@END---------
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

    '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD �@START---------
    ''' <summary>
    ''' [���ʃR�[�i�[�`�F�b�N]
    ''' </summary>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function CheckCorner() As Boolean
        Dim station, sStation As String
        Dim corCode As String
        Dim code As String
        Try
            'INI�ݒ�F���ʃR�[�i�[���擾
            code = Config.KsbConfigDirectEkCode.ToString
            'INI�ݒ�F���ʃR�[�i�[�̉w
            station = code.Substring(0, 3) & code.Substring(4, 3)
            'INI�ݒ�F���ʃR�[�i�[
            corCode = code.Substring(code.Length - 2, 2)
            '�I�𒆂̉w
            sStation = cmbEki.SelectedValue.ToString
            '�I�𒆂̃R�[�i�[�`�F�b�N
            If sStation.Substring(sStation.Length - 6, 6) = station And cmbMado.SelectedValue.ToString = corCode Then
                CorFlg = True
            Else
                CorFlg = False
            End If
        Catch ex As Exception
        End Try
        Return CorFlg
    End Function
    '-------Ver0.1�@�o�͍��ڊg���Ή��@ADD �@END---------
#End Region

End Class