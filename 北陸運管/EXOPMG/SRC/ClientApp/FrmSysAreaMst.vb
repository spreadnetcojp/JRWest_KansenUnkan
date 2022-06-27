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
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.Text

''' <summary>�G���A�ݒ�</summary>
''' <remarks>
''' �G���A�ݒ��ʂ̌��������ɂ���āA�G���A����\������B
''' �ꗗ�f�[�^��I�����邱�Ƃɂ��A�T�u��ʂɂďC���A�폜���\�B
''' </remarks>
Public Class FrmSysAreaMst
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
    Friend WithEvents istAreaMst As System.Windows.Forms.ImageList
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents wbkMain As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnInsert As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents cmbModelname As System.Windows.Forms.ComboBox
    Friend WithEvents btnSearch As System.Windows.Forms.Button
    Friend WithEvents pnlSelect As System.Windows.Forms.Panel
    Friend WithEvents lblMach As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysAreaMst))
        Me.wbkMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.cmbModelname = New System.Windows.Forms.ComboBox()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.pnlSelect = New System.Windows.Forms.Panel()
        Me.lblMach = New System.Windows.Forms.Label()
        Me.istAreaMst = New System.Windows.Forms.ImageList(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.wbkMain.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlSelect.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.btnSearch)
        Me.pnlBodyBase.Controls.Add(Me.wbkMain)
        Me.pnlBodyBase.Controls.Add(Me.btnInsert)
        Me.pnlBodyBase.Controls.Add(Me.btnUpdate)
        Me.pnlBodyBase.Controls.Add(Me.btnDelete)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.pnlSelect)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/07/31(��)  11:51"
        '
        'wbkMain
        '
        Me.wbkMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wbkMain.Controls.Add(Me.shtMain)
        Me.wbkMain.Location = New System.Drawing.Point(125, 100)
        Me.wbkMain.Name = "wbkMain"
        Me.wbkMain.ProcessTabKey = False
        Me.wbkMain.ShowTabs = False
        Me.wbkMain.Size = New System.Drawing.Size(580, 505)
        Me.wbkMain.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wbkMain.TabIndex = 1
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(1, 1)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(561, 486)
        Me.shtMain.TabIndex = 99
        Me.shtMain.TabStop = False
        '
        'btnInsert
        '
        Me.btnInsert.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnInsert.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnInsert.Location = New System.Drawing.Point(872, 404)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(128, 40)
        Me.btnInsert.TabIndex = 3
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
        Me.btnUpdate.TabIndex = 4
        Me.btnUpdate.Text = "�C  ��"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDelete.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(872, 524)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(128, 40)
        Me.btnDelete.TabIndex = 5
        Me.btnDelete.Text = "��  ��"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 6
        Me.btnReturn.Text = "�I  ��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'cmbModelname
        '
        Me.cmbModelname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModelname.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmbModelname.Location = New System.Drawing.Point(45, 10)
        Me.cmbModelname.MaxLength = 5
        Me.cmbModelname.Name = "cmbModelname"
        Me.cmbModelname.Size = New System.Drawing.Size(252, 21)
        Me.cmbModelname.TabIndex = 1
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.Color.Silver
        Me.btnSearch.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!)
        Me.btnSearch.Location = New System.Drawing.Point(657, 17)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(128, 40)
        Me.btnSearch.TabIndex = 2
        Me.btnSearch.Text = "��  ��"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'pnlSelect
        '
        Me.pnlSelect.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlSelect.Controls.Add(Me.cmbModelname)
        Me.pnlSelect.Controls.Add(Me.lblMach)
        Me.pnlSelect.Location = New System.Drawing.Point(117, 17)
        Me.pnlSelect.Name = "pnlSelect"
        Me.pnlSelect.Size = New System.Drawing.Size(384, 40)
        Me.pnlSelect.TabIndex = 0
        '
        'lblMach
        '
        Me.lblMach.Location = New System.Drawing.Point(4, 16)
        Me.lblMach.Name = "lblMach"
        Me.lblMach.Size = New System.Drawing.Size(45, 19)
        Me.lblMach.TabIndex = 6
        Me.lblMach.Text = "�@��"
        '
        'istAreaMst
        '
        Me.istAreaMst.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.istAreaMst.ImageSize = New System.Drawing.Size(16, 16)
        Me.istAreaMst.TransparentColor = System.Drawing.Color.White
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'FrmSysAreaMst
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysAreaMst"
        Me.Text = "�^�p�[�� "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wbkMain.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlSelect.ResumeLayout(False)
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
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "�G���A�ݒ�"

    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private LcstMaxColCnt As Integer

    ''' <summary>
    ''' �ꗗ�w�b�_�̃\�[�g�񊄂蓖��
    ''' �i�ꗗ�w�b�_�N���b�N���Ɋ��蓖�Ă�Ώۗ���`�B��ԍ��̓[�����΂�"-1"�̓\�[�g�ΏۊO�̗�j
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {0, 1}

#End Region

#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' �G���A�ݒ��ʂ̃f�[�^����������
    ''' </summary>
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e

        Try
            Log.Info("Method started.")

            '�Ɩ��^�C�g���\���G���A�ɉ�ʃ^�C�g�����Z�b�g
            lblTitle.Text = LcstFormTitle

            '�V�[�g������
            shtMain.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row                      '�s�I�����[�h
            shtMain.MaxRows() = 0                                               '�s�̏�����
            LcstMaxColCnt = shtMain.MaxColumns()                                '�񐔂��擾
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   '�V�[�g��\�����[�h
            '�V�[�g�̃w�b�_�I���C�x���g�̃n���h���ǉ�
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

            '�e�R���{�{�b�N�X�̍��ړo�^
            If LfSetCmbModelname() = False Then Exit Try
            cmbModelname.SelectedIndex = 0

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
                AlertBox.Show(Lexis.FormProcAbnormalEnd) '�J�n�ُ탁�b�Z�[�W
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
    Private Sub FrmSysAreaMst_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrmData() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If

            '������ �}�X�^���̂�rbtnMst��ݒ肷��B
            setBtnStatus(False, False, False, False)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' �u�����v�{�^������������ƁAEltable�̓��e��\������B
    ''' </summary>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True

            '�����{�^�������B
            LogOperation(sender, e)

            '�ꗗ�V�[�g�̏������iLfClrList�j
            LfClrList()

            '�G���A���擾
            Call SelectArea(True)

            btnInsert.Enabled = True
            shtMain.Select()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred)    '�������s���b�Z�[�W
            btnSearch.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' �u�o�^�v�{�^������������ƁA�G���A���̂���͉�ʂ��\�������B
    ''' </summary>
    Private Sub btnAddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            LbEventStop = True

            '�o�^�{�^�������B
            LogOperation(sender, e)

            Dim oFrmSysAreaMstAdd As New FrmSysAreaMstAdd
            oFrmSysAreaMstAdd.ModelCode = cmbModelname.SelectedValue.ToString

            oFrmSysAreaMstAdd.ShowDialog()

            'TODO: Form.New���Ăяo���Ĉȍ~�ɗ�O�����������ꍇ�̂��Ƃ�
            '�l����ƁAFrmMntDispFaultDataDetail��ShowDialog���s���Ƃ��Ɠ��l��
            '���j�ɓ��ꂷ������悢��������Ȃ��B�i�t�ɂ����炪�����̉\��������j
            oFrmSysAreaMstAdd.Dispose()

            '�ꗗ�V�[�g�̏������iLfClrList�j
            LfClrList()

            '�G���A���擾
            Call SelectArea(False)

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

    ''' <summary>
    ''' �u�C���v�{�^������������ƁA�G���A���̂�ύX��ʂ��\������� �B
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            '�C���{�^�������B
            LogOperation(sender, e)

            Dim oFrmSysAreaMstUpdate As New FrmSysAreaMstUpdate

            'FrmSysAreaMstUpdate��ʂ̃v���p�e�B�ɒl��������B
            Dim nRowno As Integer = shtMain.ActivePosition.Row

            '�@��R�[�h���擾����B
            oFrmSysAreaMstUpdate.ModelCode = cmbModelname.SelectedValue.ToString
            '�G���ANo���擾����B
            oFrmSysAreaMstUpdate.AreaNo = Me.shtMain.Item(0, nRowno).Text

            If oFrmSysAreaMstUpdate.InitFrmData() = False Then
                oFrmSysAreaMstUpdate = Nothing
                Exit Sub
            End If

            oFrmSysAreaMstUpdate.ShowDialog()
            oFrmSysAreaMstUpdate.Dispose()

            '�ꗗ�V�[�g�̏������iLfClrList�j
            LfClrList()

            '�G���A���擾
            Call SelectArea(False)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' �u�폜�v�{�^������������ƁA�G���A���̂��폜��ʂ��\�������B
    ''' </summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True

            '�폜�{�^�������B
            LogOperation(sender, e)

            Dim oFrmSysAreaMstDelete As New FrmSysAreaMstDelete

            'oFrmSysAreaMstDelete��ʂ̃v���p�e�B�ɒl��������B
            Dim nRowno As Integer = shtMain.ActivePosition.Row

            '�@��R�[�h���擾����B
            oFrmSysAreaMstDelete.ModelCode() = cmbModelname.SelectedValue.ToString
            '�G���ANo���擾����B
            oFrmSysAreaMstDelete.AreaNo() = Me.shtMain.Item(0, nRowno).Text

            If oFrmSysAreaMstDelete.InitFrmData() = False Then
                oFrmSysAreaMstDelete = Nothing
                LfWaitCursor(False)
                Exit Sub
            End If

            oFrmSysAreaMstDelete.ShowDialog()
            oFrmSysAreaMstDelete.Dispose()

            '�ꗗ�V�[�g�̏������iLfClrList�j
            LfClrList()

            '�G���A���擾
            Call SelectArea(False)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
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
    ''' �u�@��v�̑I���A�����{�^���������A�o�^�{�^���A�C���{�^���A�폜�{�^���̔񊈐���
    ''' </summary>
    Private Sub cmbModelname_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbModelname.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            LfClrList()         '�ꗗ�V�[�g�̏������iLfClrList�j
            If shtMain.Enabled = True Then shtMain.Enabled = False

            If cmbModelname.SelectedIndex = 0 Then
                setBtnStatus(False, False, False, False)
            Else
                setBtnStatus(True, False, False, False)      '�����{�^���������iLfSearchTrue�j
            End If

        Catch ex As Exception
            If btnSearch.Enabled = True Then btnSearch.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMach.Text)
        Finally
            LfWaitCursor(False)
            LbEventStop = False
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

#End Region

#Region "���\�b�h�iPrivate�j"

    ''' <summary>Eltable�̓��e��\������</summary>
    ''' <remarks>
    ''' �G���ANo�A�G���A���̂�\������B
    ''' </remarks>
    Private Sub LfSetSheetData(ByVal dtMstTable As DataTable)

        '��ʂ̑M����h���B
        Me.shtMain.Redraw = False
        Me.wbkMain.Redraw = False
        Try
            Me.shtMain.MaxRows = dtMstTable.Rows.Count     '���o�������̍s���ꗗ�ɍ쐬

            Me.shtMain.DataSource = dtMstTable             '�s�����𑵂���

            shtMain.Rows.SetAllRowsHeight(21)              '�f�[�^���Z�b�g

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            btnInsert.Select()
        Finally
            'Eltable���ĕ\������B
            Me.shtMain.Redraw = True
            Me.wbkMain.Redraw = True
        End Try

    End Sub

    ''' <summary>�G���A���̂��擾����B</summary>
    ''' <returns>
    ''' �G���A�f�[�^�G���A�擾���ʊi�[�e�[�u���B
    ''' </returns>
    ''' <remarks>�G���A���擾����</remarks>
    Private Function LfGetSelectString() As String

        Dim sSQL As String = ""

        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine("SELECT AREA_NO , AREA_NAME FROM M_AREA_DATA ")
            sBuilder.AppendLine(String.Format("WHERE MODEL_CODE = {0} ORDER BY AREA_NO", Utility.SetSglQuot(cmbModelname.SelectedValue.ToString)))
            sSQL = sBuilder.ToString()

        Catch ex As Exception
            btnSearch.Select()
            Throw New Exception

        End Try

        Return sSQL
    End Function

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
        Me.btnDelete.Enabled = bBtnDelete
    End Sub

    ''' <summary>
    ''' [�ꗗ�N���A]
    ''' </summary>
    Private Sub LfClrList()
        shtMain.Redraw = False
        wbkMain.Redraw = False
        Try
            shtMain.DataSource = Nothing
            shtMain.MaxRows = 0

            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnDelete.Enabled = True Then btnDelete.Enabled = False
            If btnUpdate.Enabled = True Then btnUpdate.Enabled = False
        Finally
            wbkMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' �G���A�̌�������AEltable�̓��e��\������B
    ''' </summary>
    Private Sub SelectArea(ByVal bArea As Boolean)

        Dim sSql As String = ""
        Dim nRtn As Integer = 1

        'ELTable�ɕ\������Ă���f�[�^���i�[����B
        Dim dtAreaTable As New DataTable

        Try
            'Eltable�̂��ׂẴf�[�^���擾����B
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dtAreaTable)

            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    btnUpdate.Enabled = False
                    btnDelete.Enabled = False
                    btnSearch.Select()
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                Case 0               '�f�[�^���Ȃ��ꍇ
                    btnUpdate.Enabled = False
                    btnDelete.Enabled = False
                    If bArea = True Then
                        '���������Ɉ�v����f�[�^�͑��݂��܂���B
                        AlertBox.Show(Lexis.NoRecordsFound)
                    End If
                Case Else
                    btnUpdate.Enabled = True
                    btnDelete.Enabled = True
                    shtMain.Enabled = True

                    'Eltable�̓��e��\������B
                    Call LfSetSheetData(dtAreaTable)
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

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
            dt = oMst.SelectTable(True)
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

#End Region

End Class
