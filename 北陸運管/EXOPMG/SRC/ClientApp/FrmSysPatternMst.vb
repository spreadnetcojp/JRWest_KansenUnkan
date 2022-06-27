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
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '�萔�l�̂ݎg�p
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO
Imports System.Text
Imports AdvanceSoftware.VBReport7.Xls

''' <summary>�p�^�[���ݒ�</summary>
''' <remarks>
''' �p�^�[���ݒ��ʂ̌��������ɂ���āA�p�^�[������\������B
''' ��̌������R�[�h��I�����A�Ή�����������T�u��ʓo�^�A�C���A�폜�ɓn���B
''' </remarks>
Public Class FrmSysPatternMst
    Inherits FrmBase

    '�t���O:���������́u�}�X�^���́v���擾���܂����A������Ɓu�v���O�������́v���擾���܂���
    Private bMstChecked As Boolean = False


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
    Friend WithEvents istPatternMst As System.Windows.Forms.ImageList
    Friend WithEvents btnInsert As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelet As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents cmbMstname As System.Windows.Forms.ComboBox
    Friend WithEvents cmbModelname As System.Windows.Forms.ComboBox
    Friend WithEvents btnSearch As System.Windows.Forms.Button
    Friend WithEvents pnlSelect As System.Windows.Forms.Panel
    Friend WithEvents lblMstName As System.Windows.Forms.Label
    Friend WithEvents lblMach As System.Windows.Forms.Label
    Friend WithEvents wbkIDMst As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents grpSelect As System.Windows.Forms.GroupBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysPatternMst))
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDelet = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.cmbMstname = New System.Windows.Forms.ComboBox()
        Me.cmbModelname = New System.Windows.Forms.ComboBox()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.pnlSelect = New System.Windows.Forms.Panel()
        Me.grpSelect = New System.Windows.Forms.GroupBox()
        Me.lblMach = New System.Windows.Forms.Label()
        Me.lblMstName = New System.Windows.Forms.Label()
        Me.istPatternMst = New System.Windows.Forms.ImageList(Me.components)
        Me.wbkIDMst = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.pnlBodyBase.SuspendLayout()
        Me.pnlSelect.SuspendLayout()
        Me.grpSelect.SuspendLayout()
        Me.wbkIDMst.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.wbkIDMst)
        Me.pnlBodyBase.Controls.Add(Me.btnInsert)
        Me.pnlBodyBase.Controls.Add(Me.btnUpdate)
        Me.pnlBodyBase.Controls.Add(Me.btnDelet)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.pnlSelect)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/08/02(��)  15:27"
        '
        'btnInsert
        '
        Me.btnInsert.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnInsert.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnInsert.Location = New System.Drawing.Point(872, 404)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(128, 40)
        Me.btnInsert.TabIndex = 4
        Me.btnInsert.Text = "�o  �^"
        Me.btnInsert.UseVisualStyleBackColor = False
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnUpdate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(872, 464)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(128, 40)
        Me.btnUpdate.TabIndex = 5
        Me.btnUpdate.Text = "�C  ��"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'btnDelet
        '
        Me.btnDelet.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDelet.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelet.Location = New System.Drawing.Point(872, 524)
        Me.btnDelet.Name = "btnDelet"
        Me.btnDelet.Size = New System.Drawing.Size(128, 40)
        Me.btnDelet.TabIndex = 6
        Me.btnDelet.Text = "��  ��"
        Me.btnDelet.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 7
        Me.btnReturn.Text = "�I  ��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'cmbMstname
        '
        Me.cmbMstname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMstname.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmbMstname.Location = New System.Drawing.Point(160, 67)
        Me.cmbMstname.MaxLength = 15
        Me.cmbMstname.Name = "cmbMstname"
        Me.cmbMstname.Size = New System.Drawing.Size(220, 21)
        Me.cmbMstname.TabIndex = 2
        '
        'cmbModelname
        '
        Me.cmbModelname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModelname.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmbModelname.Location = New System.Drawing.Point(160, 19)
        Me.cmbModelname.MaxLength = 5
        Me.cmbModelname.Name = "cmbModelname"
        Me.cmbModelname.Size = New System.Drawing.Size(220, 21)
        Me.cmbModelname.TabIndex = 1
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.Color.Silver
        Me.btnSearch.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!)
        Me.btnSearch.Location = New System.Drawing.Point(570, 22)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(128, 40)
        Me.btnSearch.TabIndex = 3
        Me.btnSearch.Text = "��  ��"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'pnlSelect
        '
        Me.pnlSelect.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlSelect.Controls.Add(Me.grpSelect)
        Me.pnlSelect.Location = New System.Drawing.Point(10, 20)
        Me.pnlSelect.Name = "pnlSelect"
        Me.pnlSelect.Size = New System.Drawing.Size(824, 110)
        Me.pnlSelect.TabIndex = 0
        '
        'grpSelect
        '
        Me.grpSelect.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grpSelect.Controls.Add(Me.lblMach)
        Me.grpSelect.Controls.Add(Me.cmbMstname)
        Me.grpSelect.Controls.Add(Me.lblMstName)
        Me.grpSelect.Controls.Add(Me.cmbModelname)
        Me.grpSelect.Controls.Add(Me.btnSearch)
        Me.grpSelect.Location = New System.Drawing.Point(56, 10)
        Me.grpSelect.Name = "grpSelect"
        Me.grpSelect.Size = New System.Drawing.Size(747, 100)
        Me.grpSelect.TabIndex = 0
        Me.grpSelect.TabStop = False
        '
        'lblMach
        '
        Me.lblMach.Location = New System.Drawing.Point(74, 22)
        Me.lblMach.Name = "lblMach"
        Me.lblMach.Size = New System.Drawing.Size(77, 19)
        Me.lblMach.TabIndex = 6
        Me.lblMach.Text = "�@��"
        '
        'lblMstName
        '
        Me.lblMstName.Location = New System.Drawing.Point(74, 70)
        Me.lblMstName.Name = "lblMstName"
        Me.lblMstName.Size = New System.Drawing.Size(77, 19)
        Me.lblMstName.TabIndex = 7
        Me.lblMstName.Text = "�}�X�^����"
        '
        'istPatternMst
        '
        Me.istPatternMst.ImageStream = CType(resources.GetObject("istPatternMst.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.istPatternMst.TransparentColor = System.Drawing.Color.White
        Me.istPatternMst.Images.SetKeyName(0, "")
        Me.istPatternMst.Images.SetKeyName(1, "")
        '
        'wbkIDMst
        '
        Me.wbkIDMst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wbkIDMst.Controls.Add(Me.shtMain)
        Me.wbkIDMst.Location = New System.Drawing.Point(124, 164)
        Me.wbkIDMst.Name = "wbkIDMst"
        Me.wbkIDMst.ProcessTabKey = False
        Me.wbkIDMst.ShowTabs = False
        Me.wbkIDMst.Size = New System.Drawing.Size(580, 436)
        Me.wbkIDMst.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wbkIDMst.TabIndex = 0
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(1, 1)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(561, 417)
        Me.shtMain.TabIndex = 0
        Me.shtMain.TabStop = False
        '
        'FrmSysPatternMst
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysPatternMst"
        Me.Text = "�^�p�[�� "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlSelect.ResumeLayout(False)
        Me.grpSelect.ResumeLayout(False)
        Me.wbkIDMst.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub


#End Region

#Region "�錾�̈�iPrivate�j"


    '�����������擾����B
    Private sKind As String = ""

    ''' <summary>
    ''' ���������ďo����
    ''' �iTrue:���������ďo�ς݁AFalse:�����������ďo(Form_Load���ŏ����������{)�j
    ''' </summary>
    Private LbInitCallFlg As Boolean = False

    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean = False

    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private LcstMaxColCnt As Integer

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "�p�^�[���ݒ�"

#End Region

#Region " ���\�b�h�iPublic�j"

    ''' <summary>�p�^�[���ݒ��ʂ̃f�[�^����������</summary>
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrm() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True       '�C�x���g�����n�e�e
        Try
            Log.Info("Method started.")

            '�Ɩ��^�C�g���\���G���A�ɉ�ʃ^�C�g�����Z�b�g
            lblTitle.Text = LcstFormTitle

            '�V�[�g������
            shtMain.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row
            LcstMaxColCnt = shtMain.MaxColumns()                                '�񐔂��擾
            '�V�[�g�̕\���I�����[�h��ݒ肷��
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader

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

            '���������@��f�[�^���i�[����B
            If LfSetCmbModelname() = False Then Exit Try '�w���R���{�{�b�N�X�ݒ�
            cmbModelname.SelectedIndex = 0            '�f�t�H���g�\������
            If cmbModelname.SelectedValue.ToString <> "" Then
                If LfSetCmbMstName(cmbModelname.SelectedValue.ToString) = False Then Exit Try '�R�[�i�[�R���{�{�b�N�X�ݒ�
                cmbMstname.SelectedIndex = 0           '�f�t�H���g�\������
            Else
                cmbMstname.Enabled = False
            End If

            '�R���{�{�b�N�X�̏�Ԃ̐ݒ�
            setComboStatus(True, False)
            '�ꗗ�\�[�g�̏�����
            LfClrList()
            LbEventStop = False         '�C�x���g�����n�m
            bRtn = True

        Catch ex As Exception
            '��ʕ\�������Ɏ��s���܂����B
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

#Region " �C�x���g "

    ''' <summary>
    ''' ���[�f�B���O�@���C���E�B���h�E
    ''' </summary>
    Private Sub FrmSysPatternMst_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrm() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If

            '������ �{�^���̔񊈐���
            setBtnStatus(False, False, False, False)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub


    ''' <summary>�u�����v�{�^������������ƁAEltable�̓��e��\������B</summary>
    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If LbEventStop Then Exit Sub
        Try
            LbEventStop = True
            LfWaitCursor()

            '�����{�^�������B
            LogOperation(sender, e)
            '�ꗗ�\�[�g�̏�����
            LfClrList()
            shtMain.Enabled = True
            btnInsert.Enabled = True
            '�p�^�[���̌�������
            Call selectPattern(True)
        Catch ex As Exception
            '���������Ɏ��s���܂����B
            Log.Fatal("Unwelcome Exception caught.", ex)        '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W
            btnSearch.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>�u�o�^�v�{�^������������ƁA�p�^�[�����̂���͉�ʂ��\�������B</summary>
    Private Sub btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click
        If LbEventStop Then Exit Sub
        Try
            LbEventStop = True
            LfWaitCursor()
            '�o�^�{�^�������B
            LogOperation(sender, e)

            Dim oFrmSysPatternMstAdd As New FrmSysPatternMstAdd
            '�����ID���擾����B
            oFrmSysPatternMstAdd.LoginID() = GlobalVariables.UserId
            '�}�X�^��ʂ��擾����B
            oFrmSysPatternMstAdd.Kind() = Me.cmbMstname.SelectedValue.ToString
            '���������̃t���O���擾����B
            oFrmSysPatternMstAdd.CheckFlag() = bMstChecked

            If Not cmbModelname.DataSource Is Nothing Then
                '�@��R�[�h���擾����
                oFrmSysPatternMstAdd.ModelCode() = Me.cmbModelname.SelectedValue.ToString()
            End If

            '�p�^�[���o�^��ʕ\�������J�n
            oFrmSysPatternMstAdd.ShowDialog()

            'TODO: Form.New���Ăяo���Ĉȍ~�ɗ�O�����������ꍇ�̂��Ƃ�
            '�l����ƁAFrmMntDispFaultDataDetail��ShowDialog���s���Ƃ��Ɠ��l��
            '���j�ɓ��ꂷ������悢��������Ȃ��B�i�t�ɂ����炪�����̉\��������j
            oFrmSysPatternMstAdd.Dispose()

            '�ꗗ�\�[�g�̏�����
            LfClrList()
            '�p�^�[���̌�������
            selectPattern(False)
            shtMain.Enabled = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'TODO: ���̂悤�ȃP�[�X�ŉ��L���s���ׂ����ۂ��A���j�𓝈ꂵ�Ȃ���΂Ȃ�Ȃ��B
            '���[�_����ShowDialog�̍Œ��ɔ���������O���{���ɂ����ɓ��B����Ȃ�A
            '���̉ӏ����A����������ŁAInitFrm�œ��l�̃��b�Z�[�W�{�b�N�X�\����
            '�s��Ȃ��悤�ɂ�������悢��������Ȃ��B
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>�u�C���v�{�^������������ƁA�p�^�[�����̂�ύX��ʂ��\�������B</summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Try
            LbEventStop = True
            LfWaitCursor()
            '�C���{�^�������B
            LogOperation(sender, e)

            Dim oFrmSysPatternMstUpdate As New FrmSysPatternMstUpdate

            'FrmSysIDMstUpdate��ʂ̃v���p�e�B�ɒl��������B
            Dim nRowno As Integer = shtMain.ActivePosition.Row

            '�����ID���擾����B
            oFrmSysPatternMstUpdate.LoginID() = GlobalVariables.UserId
            '�p�^�[��No���擾����B
            oFrmSysPatternMstUpdate.PatternNo() = Me.shtMain.Item(0, nRowno).Text
            '�p�^�[�����̂��擾����B
            oFrmSysPatternMstUpdate.PatternName() = Me.shtMain.Item(1, nRowno).Text
            '�}�X�^��ʂ��擾����B
            oFrmSysPatternMstUpdate.Kind() = Me.cmbMstname.SelectedValue.ToString
            '���������̃t���O���擾����B
            oFrmSysPatternMstUpdate.CheckFlag() = bMstChecked

            If Not cmbModelname.DataSource Is Nothing Then
                '�@��R�[�h���擾����
                oFrmSysPatternMstUpdate.Modelcode() = Me.cmbModelname.SelectedValue.ToString()
            End If

            If oFrmSysPatternMstUpdate.InitFrmData() = False Then
                oFrmSysPatternMstUpdate = Nothing
                Call waitCursor(False)
                Exit Sub
            End If
            '�p�^�[���C����ʕ\�������J�n
            oFrmSysPatternMstUpdate.ShowDialog()
            oFrmSysPatternMstUpdate.Dispose()
            '�ꗗ�\�[�g�̏�����
            LfClrList()
            '�p�^�[���̌�������
            Call selectPattern(False)
            shtMain.Enabled = True
        Catch ex As Exception
            '��ʕ\�������Ɏ��s���܂����B
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>�u�폜�v�{�^������������ƁA�p�^�[�����̂��폜��ʂ��\�������B</summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelet.Click
        If LbEventStop Then Exit Sub
        Try
            LbEventStop = True
            LfWaitCursor()
            '�폜�{�^�������B
            LogOperation(sender, e)

            Dim oFrmSysPatternMstDelete As New FrmSysPatternMstDelete

            'oFrmSysPatternMstDelete��ʂ̃v���p�e�B�ɒl��������B
            Dim nRowno As Integer = shtMain.ActivePosition.Row

            '�p�^�[��No���擾����B
            oFrmSysPatternMstDelete.PatternNo() = Me.shtMain.Item(0, nRowno).Text
            '�p�^�[�����̂��擾����B
            oFrmSysPatternMstDelete.PatternName() = Me.shtMain.Item(1, nRowno).Text
            '�}�X�^��ʂ��擾����B
            oFrmSysPatternMstDelete.Kind() = Me.cmbMstname.SelectedValue.ToString
            '���������̃t���O���擾����B
            oFrmSysPatternMstDelete.CheckFlag() = bMstChecked

            If Not cmbModelname.DataSource Is Nothing Then
                '�@��R�[�h���擾����
                oFrmSysPatternMstDelete.Modelcode() = Me.cmbModelname.SelectedValue.ToString()
            End If

            If oFrmSysPatternMstDelete.InitFrmData() = False Then
                oFrmSysPatternMstDelete = Nothing
                Call waitCursor(False)
                Exit Sub
            End If
            '�p�^�[���폜��ʕ\�������J�n
            oFrmSysPatternMstDelete.ShowDialog()
            oFrmSysPatternMstDelete.Dispose()
            '�ꗗ�\�[�g�̏�����
            LfClrList()
            '�p�^�[���̌�������
            Call selectPattern(False)
            shtMain.Enabled = True
        Catch ex As Exception
            '��ʕ\�������Ɏ��s���܂����B
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>�u�I���v�{�^������������ƁA�{��ʂ��I�������B</summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '�I���{�^�������B
        LogOperation(sender, e)
        Me.Close()
    End Sub

#End Region

#Region " ELTable�̃N���A "

    ''' <summary>ELTable�̃N���A</summary>
    ''' <remarks>
    ''' Eltable�ɂ���f�[�^���N���A
    ''' </remarks>
    Private Sub initElTable()
        'Eltable�̌��݂̍ő包��
        Dim sXYRange As String
        Dim i As Integer

        '��ʂ̑M����h���B
        Me.shtMain.Redraw = False

        Try
            For i = 0 To shtMain.Columns.Count - 1
                '��w�b�_�̃C���[�W���N���A����
                shtMain.ColumnHeaders(i).Image = Nothing
                If shtMain.Rows.Count > 0 Then
                    '�O��\�[�g���ꂽ��̔w�i�F������������
                    shtMain.Columns(i).BackColor = Color.Empty
                    '�O��\�[�g���ꂽ��̃Z���r������������
                    shtMain.Columns(i).SetBorder(New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), GrapeCity.Win.ElTabelleSheet.Borders.All)
                End If
            Next

            If Me.shtMain.MaxRows > 0 Then
                'Eltable�̌��݂̍ő包�����擾����B
                sXYRange = "1:" & Me.shtMain.MaxRows.ToString

                '�I�����ꂽ�G���A�̃f�[�^���N���A����B
                Me.shtMain.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
            End If

            'Eltable�̍ő包����ݒ肷��B
            Me.shtMain.MaxRows = 0

        Catch ex As Exception

            '��ʕ\�������Ɏ��s���܂���
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New DatabaseException(ex)

        Finally

            'Eltable���X�V����B
            Me.shtMain.Redraw = True

        End Try

    End Sub

#End Region

#Region " �R���{�{�b�N�X���e���擾 "


    ''' <summary>
    ''' [�@�햼�̃R���{�ݒ�]
    ''' </summary>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function LfSetCmbModelname() As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As ModelMaster
        oMst = New ModelMaster
        Try
            dt = oMst.SelectTable(False)
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbModelname)
            cmbModelname.SelectedIndex = -1
            If cmbModelname.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function

    ''' <summary>�}�X�^���̂��擾����</summary>
    ''' <returns>�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u���B</returns>
    ''' <remarks>
    ''' �}�X�^���̂��擾����ADataTable�̐擪�ɁA�󔒍s��ǉ�����B
    ''' </remarks>
    Private Function LfSetCmbMstName(ByVal sModel As String) As Boolean
        '�}�X�^���̂��i�[����B
        Dim bRtn As Boolean = False
        Dim dt As New DataTable
        Dim oMst As MasterMaster
        oMst = New MasterMaster
        Try
            If sModel <> "" Then
                dt = oMst.SelectTable(sModel)
                dt = oMst.SetSpace()
                bRtn = BaseSetMstDtToCmb(dt, cmbMstname)
                cmbMstname.SelectedIndex = -1
            End If

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
    ''' [�����pSELECT������擾]
    ''' </summary>
    ''' <returns>SELECT��</returns>
    Private Function LfGetSelectString() As String

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder
        '�@��f�[�^���擾����B
        Dim sModel As String = Me.cmbModelname.SelectedValue.ToString
        Dim sMstKind As String = Me.cmbMstname.SelectedValue.ToString
        Try
            '�e�[�u��:�p�^�[�����̃}�X�^
            '�擾����:�p�^�[��NO
            '�擾����:�p�^�[������
            sBuilder.AppendLine(" SELECT PATTERN_NO,PATTERN_NAME ")
            sBuilder.AppendLine(" FROM M_PATTERN_DATA ")
            sBuilder.AppendLine(" WHERE MODEL_CODE = " + Utility.SetSglQuot(sModel))
            sBuilder.AppendLine(" AND MST_KIND = " + Utility.SetSglQuot(sMstKind))
            sBuilder.AppendLine(" ORDER BY PATTERN_NO ")
            sSQL = sBuilder.ToString()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

        Return sSQL

    End Function

    ''' <summary>DataTable����C���f�b�N�X�l�̎擾</summary>
    ''' <param name="dtSelect"> ��������f�[�^�e�[�u��</param>
    ''' <param name="sSelectValue">����������e</param>
    ''' <returns>datatable����O�̉�ʂ���n���ꂽ�l��dt�ɂ���C���f�b�N�X�����o����</returns>
    Private Function getIndex(ByVal dtSelect As DataTable, ByVal sSelectValue As String) As Integer

        '�C���f�b�N�X�̒l
        Dim nIndex As Integer = 0
        Dim i As Integer = 0

        For i = 0 To dtSelect.Rows.Count - 1
            If dtSelect.Rows(i).Item(0).ToString = sSelectValue Then
                nIndex = i
                Exit For
            End If
        Next

        '�C���f�b�N�X�̒l
        Return nIndex

    End Function

    ''' <summary>DataTable�̐擪�ɁA�󔒍s��ǉ�����B</summary>
    ''' <remarks>
    '''  DataTable�̐擪�ɁA�󔒍s��ǉ�����B
    ''' </remarks>
    ''' <returns>�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SetSpace(ByVal dt As DataTable) As DataTable
        Dim drw As DataRow

        drw = dt.NewRow()

        'DataTable�̐擪�ɁA�󔒍s��ǉ�����B
        For i As Integer = 0 To dt.Columns.Count - 1
            drw.Item(i) = ""
        Next

        dt.Rows.InsertAt(drw, 0)

        Return dt
    End Function

#End Region

#Region "���\�b�h"

    ''' <summary>�p�^�[���̌�������AEltable�̓��e��\������B</summary>
    Private Sub selectPattern(ByVal bPattern As Boolean)

        'ELTable�ɕ\������Ă���f�[�^���i�[����B
        Dim dtPatternTable As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer
        Try
            'Eltable�̂��ׂẴf�[�^���擾����B
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dtPatternTable)

            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    btnUpdate.Enabled = False
                    btnDelet.Enabled = False
                    btnSearch.Select()
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                Case 0
                    btnUpdate.Enabled = False
                    btnDelet.Enabled = False
                    If bPattern = True Then
                        '���������Ɉ�v����f�[�^�͑��݂��܂���B
                        AlertBox.Show(Lexis.NoRecordsFound)
                    End If
                Case Else
                    btnInsert.Enabled = True
                    btnUpdate.Enabled = True
                    btnDelet.Enabled = True
                    shtMain.Enabled = True
                    'Eltable�̓��e��\������B
                    Call LfSetSheetData(dtPatternTable)
            End Select

        Catch ex As OPMGException
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New Exception
        End Try
    End Sub

    ''' <summary>
    ''' [�ꗗ�N���A]
    ''' </summary>
    Private Sub LfClrList()
        shtMain.Redraw = False
        wbkIDMst.Redraw = False
        Try
            'NOTE: ���̒��ŗ�O�����������ꍇ�̃��O�o�͂�
            '���b�Z�[�W�{�b�N�X�\���́A���̃��\�b�h��
            '�Ăь��ōs���B�������AInitFrm ���\�b�h�ł�
            '���b�Z�[�W�{�b�N�X�̕\���͍s��Ȃ��i����
            '��ʂɍ��킹��j�B

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
            If btnDelet.Enabled = True Then btnDelet.Enabled = False
            If btnUpdate.Enabled = True Then btnUpdate.Enabled = False
        Finally
            shtMain.Redraw = True
            wbkIDMst.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' ELTable�̃}�E�X�̈ړ�����
    ''' </summary>
    Private Sub shtPatternMst_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
        '�}�E�X�J�[�\������w�b�_��ɂ���ꍇ
        If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
            shtMain.CrossCursor = Cursors.Default
        Else
            '�}�E�X�J�[�\��������ɖ߂�
            shtMain.CrossCursor = Nothing
        End If
    End Sub

    ''' <summary>�R���{�{�b�N�X�ɑ΂��ăf�[�^�o�C���h���s��</summary>
    ''' <param name="dt">�o�C���h�p��DataTable</param>
    ''' <param name="cmb">�o�C���h�K�v�̂���ComboBox</param>
    ''' <remarks>
    ''' �\�������o�[�A�o�����[�����o�[��DataSource��ݒ肷��B
    ''' </remarks>
    Private Sub setComboxValue(ByVal dt As DataTable, ByRef cmb As ComboBox)

        'combox�ɑ΂��ăf�[�^�o�C���h���s���Ɏ��s���܂����B
        If cmb Is Nothing Then
            '��ʕ\�������Ɏ��s���܂����B
            FrmBase.LogOperation(Lexis.FormProcAbnormalEnd) 'TODO: ���Ȃ��Ƃ��u����v���O�ł͂Ȃ��B�ڍא݌v���܂ߊm�F�B
            Throw New OPMGException()
        End If

        Try
            With cmb
                'DataSource�̐ݒ�
                .DataSource = dt
                '�\�������o�[�̐ݒ�
                .DisplayMember = dt.Columns(1).ColumnName
                '�o�����[�����o�[�̐ݒ�
                .ValueMember = dt.Columns(0).ColumnName
            End With

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '�\�����ʃG���[���������܂����B
            Throw New OPMGException(ex)
        End Try

    End Sub

    ''' <summary>
    ''' �R���{�{�b�N�X�̏�Ԃ̐ݒ�
    ''' </summary>
    ''' <param name="bCmbModelname">�u�}�X�^�v�R���{�{�b�N�X�̊������̃t���O</param>
    ''' <param name="bCmbMstname ">�u�v���O�����v�R���{�{�b�N�X�̊������̃t���O</param>
    ''' <remarks></remarks>
    Private Sub setComboStatus(ByVal bCmbModelname As Boolean, _
                               ByVal bCmbMstname As Boolean)

        Me.cmbModelname.Enabled = bCmbModelname
        If (bCmbModelname = False) Then
            If Me.cmbModelname.SelectedIndex > 0 Then
                Me.cmbModelname.SelectedIndex = 0
            End If
        End If

        Me.cmbMstname.Enabled = bCmbMstname
        If (bCmbMstname = False) Then
            If Me.cmbMstname.SelectedIndex > 0 Then
                Me.cmbMstname.SelectedIndex = 0
            End If
        End If

    End Sub

    ''' <summary>�{�^���̏�Ԃ̐ݒ�</summary>
    ''' <param name="bBtnSelect">�u�����v�{�^���̊������̃t���O</param>
    ''' <param name="bBtnAddNew">�u�o�^�v�{�^���̊������̃t���O</param>
    ''' <param name="bBtnUpdate">�u�C���v�{�^���̊������̃t���O</param>
    ''' <param name="bBtnDelete">�u�폜�v�{�^���̊������̃t���O</param>
    Private Sub setBtnStatus(ByVal bBtnSelect As Boolean, ByVal bBtnAddNew As Boolean, _
                             ByVal bBtnUpdate As Boolean, ByVal bBtnDelete As Boolean)
        Me.btnSearch.Enabled = bBtnSelect
        Me.btnInsert.Enabled = bBtnAddNew
        Me.btnUpdate.Enabled = bBtnUpdate
        Me.btnDelet.Enabled = bBtnDelete
    End Sub

    ''' <summary>Eltable�̓��e��\������</summary>
    ''' <remarks>
    ''' �G���ANo�A�G���A���̂�\������B
    ''' </remarks>
    Private Sub LfSetSheetData(ByVal dtPatternTable As DataTable)

        '��ʂ̑M����h���B
        Me.shtMain.Redraw = False
        Me.wbkIDMst.Redraw = False
        Try
            Me.shtMain.MaxRows = dtPatternTable.Rows.Count     '���o�������̍s���ꗗ�ɍ쐬

            Me.shtMain.DataSource = dtPatternTable             '�s�����𑵂���

            shtMain.Rows.SetAllRowsHeight(21)              '�f�[�^���Z�b�g

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            btnInsert.Select()
        Finally
            'Eltable���ĕ\������B
            Me.shtMain.Redraw = True
            Me.wbkIDMst.Redraw = True
        End Try

    End Sub

#End Region

#Region " �R���{�{�b�N�X�̃C�x���g "


    ''' <summary>
    ''' �u�@��v�̑I���ɂ���āA�u�v���O�������́v���擾����B
    ''' </summary>

    Private Sub cmbModelname_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbModelname.SelectedIndexChanged
        If LbEventStop = True Then Exit Sub
        LfWaitCursor()
        Dim nCmbIndex As Integer = 0
        Try
            LbEventStop = True
            'ELTable�̏������B
            Call initElTable()

            If (Me.cmbModelname.SelectedIndex < 0) Then
                Exit Sub
            End If

            If cmbModelname.SelectedIndex = 0 Then
                setComboStatus(True, False)
                setBtnStatus(False, False, False, False)
                Exit Sub
            ElseIf cmbModelname.SelectedIndex > 0 Then
                setComboStatus(True, True)
                setBtnStatus(False, False, False, False)
            End If

            '���������}�X�^�f�[�^���i�[����B
            If cmbModelname.SelectedValue.ToString <> "" Then
                If LfSetCmbMstName(cmbModelname.SelectedValue.ToString) = False Then
                    If btnSearch.Enabled = True Then btnSearch.Enabled = False
                    If cmbMstname.Enabled = True Then cmbMstname.Enabled = False
                    '�G���[���b�Z�[�W
                    AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMstName.Text)
                    LbEventStop = False      '�C�x���g�����n�m
                    btnReturn.Select()
                    Exit Sub
                End If
            Else
                cmbMstname.SelectedIndex = 0
                cmbMstname.Enabled = False
            End If

            cmbMstname.SelectedIndex = 0               '���C�x���g�����ӏ�
            If cmbMstname.Enabled = False Then cmbMstname.Enabled = True
            LbEventStop = False      '�C�x���g�����n�m
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '�\�����ʃG���[���������܂����B
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMstName.Text)
            If btnSearch.Enabled = True Then btnSearch.Enabled = False
            cmbMstname.SelectedIndex = 0
            cmbMstname.Enabled = False
            LbEventStop = False      '�C�x���g�����n�m
            btnReturn.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' �u�}�X�^�v�̑I�����AELTable�̃N���A����B
    ''' </summary>
    Private Sub cmbMstname_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbMstname.SelectedIndexChanged
        If LbEventStop = True Then Exit Sub
        LfWaitCursor()
        Dim nCmbIndex As Integer = 0
        Try
            LbEventStop = True
            Call initElTable()

            If (Me.cmbMstname.SelectedIndex < 0) Then
                Exit Sub
            End If

            ' ��Ԗڍ��ځu�X�x�[�X�v��I�����鎞�̏���
            nCmbIndex = Me.cmbMstname.SelectedIndex

            If nCmbIndex = 0 Then
                setBtnStatus(False, False, False, False)
            Else
                setBtnStatus(True, False, False, False)
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '�\�����ʃG���[���������܂����B '�\�����ʃG���[���������܂����B
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

#End Region

End Class