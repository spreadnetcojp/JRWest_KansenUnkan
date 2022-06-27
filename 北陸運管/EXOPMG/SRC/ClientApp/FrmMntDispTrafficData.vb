' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2015/04/21  (NES)����  ���[�o�͕��@�ύX
'�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�i�w�A�R�[�i���ŃV�[�g�𕪂��ďo�͂���悤�ɕύX�j
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
''' �y���ԑѕʏ�~�f�[�^�o�́@��ʃN���X�z
''' </summary>
Public Class FrmMntDispTrafficData
    Inherits FrmBase

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B
        LcstSearchCol = {Me.cmbEki, Me.cmbMado, Me.dtpYmdFrom, Me.dtpYmdTo}

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
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents pnlFromTo As System.Windows.Forms.Panel
    Friend WithEvents lblFromDate As System.Windows.Forms.Label
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents lblToDate As System.Windows.Forms.Label
    Friend WithEvents dtpYmdFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpYmdTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents pnlMado As System.Windows.Forms.Panel
    Friend WithEvents cmbMado As System.Windows.Forms.ComboBox
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispTrafficData))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.pnlFromTo = New System.Windows.Forms.Panel()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.lblFromDate = New System.Windows.Forms.Label()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.lblToDate = New System.Windows.Forms.Label()
        Me.dtpYmdFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpYmdTo = New System.Windows.Forms.DateTimePicker()
        Me.pnlMado = New System.Windows.Forms.Panel()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.pnlFromTo.SuspendLayout()
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
        Me.pnlBodyBase.Controls.Add(Me.pnlFromTo)
        Me.pnlBodyBase.Controls.Add(Me.pnlMado)
        Me.pnlBodyBase.Controls.Add(Me.pnlEki)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 87)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/07/30(��)  18:54"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(856, 511)
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
        Me.btnReturn.Location = New System.Drawing.Point(856, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 10
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'pnlFromTo
        '
        Me.pnlFromTo.Controls.Add(Me.lblTo)
        Me.pnlFromTo.Controls.Add(Me.lblFromDate)
        Me.pnlFromTo.Controls.Add(Me.lblFrom)
        Me.pnlFromTo.Controls.Add(Me.lblToDate)
        Me.pnlFromTo.Controls.Add(Me.dtpYmdFrom)
        Me.pnlFromTo.Controls.Add(Me.dtpYmdTo)
        Me.pnlFromTo.Location = New System.Drawing.Point(122, 75)
        Me.pnlFromTo.Name = "pnlFromTo"
        Me.pnlFromTo.Size = New System.Drawing.Size(540, 31)
        Me.pnlFromTo.TabIndex = 5
        '
        'lblTo
        '
        Me.lblTo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTo.Location = New System.Drawing.Point(449, 6)
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
        Me.lblFromDate.Size = New System.Drawing.Size(50, 20)
        Me.lblFromDate.TabIndex = 0
        Me.lblFromDate.Text = "�J�n��"
        Me.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFrom
        '
        Me.lblFrom.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFrom.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFrom.Location = New System.Drawing.Point(202, 6)
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
        Me.lblToDate.Location = New System.Drawing.Point(256, 6)
        Me.lblToDate.Name = "lblToDate"
        Me.lblToDate.Size = New System.Drawing.Size(50, 20)
        Me.lblToDate.TabIndex = 4
        Me.lblToDate.Text = "�I����"
        Me.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dtpYmdFrom
        '
        Me.dtpYmdFrom.Location = New System.Drawing.Point(57, 6)
        Me.dtpYmdFrom.Name = "dtpYmdFrom"
        Me.dtpYmdFrom.Size = New System.Drawing.Size(140, 20)
        Me.dtpYmdFrom.TabIndex = 4
        '
        'dtpYmdTo
        '
        Me.dtpYmdTo.Location = New System.Drawing.Point(309, 6)
        Me.dtpYmdTo.Name = "dtpYmdTo"
        Me.dtpYmdTo.Size = New System.Drawing.Size(135, 20)
        Me.dtpYmdTo.TabIndex = 6
        '
        'pnlMado
        '
        Me.pnlMado.Controls.Add(Me.cmbMado)
        Me.pnlMado.Controls.Add(Me.lblMado)
        Me.pnlMado.Location = New System.Drawing.Point(342, 36)
        Me.pnlMado.Name = "pnlMado"
        Me.pnlMado.Size = New System.Drawing.Size(237, 33)
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
        Me.pnlEki.Location = New System.Drawing.Point(122, 36)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(216, 33)
        Me.pnlEki.TabIndex = 1
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.DropDownWidth = 162
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
        'FrmMntDispTrafficData
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispTrafficData"
        Me.Text = "�^�p�[�� Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlFromTo.ResumeLayout(False)
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
    Private ReadOnly LcstXlsTemplateName As String = "���ԑѕʏ�~�f�[�^.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "���ԑѕʏ�~�f�[�^"

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
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly FormTitle As String = "���ԑѕʏ�~�f�[�^�o��"
    ''' <summary>
    ''' �w�R�[�h�̐擪3��:�u000�v
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"


    ''' <summary>
    ''' ���������ɂ���āA�����{�^��������
    ''' </summary>
    Private LcstSearchCol() As Control
#End Region

#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' ��ʏ�������
    ''' �G���[�������͓����Ń��b�Z�[�W��\�����܂��B
    ''' </summary>
    ''' <returns>True:����,False:���s</returns>
    Public Function InitFrm() As Boolean

        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e
        Try
            Log.Info("Method started.")

            '���O�o��
            lblTitle.Text = FormTitle

            '�I���{�^�����������ڐݒ�
            btnReturn.Enabled = True

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

            '�w�R���{�ݒ�
            BaseCtlEnabled(pnlEki)

            If LfSetEki() = False Then Exit Try '�w���R���{�{�b�N�X�ݒ�
            cmbEki.SelectedIndex = 0            '�f�t�H���g�\������
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try '�R�[�i�[�R���{�{�b�N�X�ݒ�
            cmbMado.SelectedIndex = 0           '�f�t�H���g�\������

            '�C�x���g�����n�m
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
            LbEventStop = False                 '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function

#End Region

#Region "�C�x���g"

    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    Private Sub FrmMntDispTrafficData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
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

            cmbEki.Select()     '�����t�H�[�J�X

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' �I��
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnReturn.Click
        LogOperation(sender, e)    '�{�^���������O
        Me.Close()
    End Sub

    ''' <summary>
    ''' ���[�o��
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click

        If LbEventStop Then Exit Sub
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""

        Try
            LfWaitCursor()

            '�f�[�^�擾
            LbEventStop = True
            LogOperation(sender, e)    '�{�^���������O
            '----Ver0.1 MOD START---------------------------
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
                Case Else
                    '���b�Z�[�W�\��
                    If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.ReallyPrinting) = DialogResult.Cancel Then
                        cmbEki.Select()
                        Exit Sub
                    End If
            End Select
            '----Ver0.1 MOD START---------------------------
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

            '�擾�f�[�^�𒠕[�ɐݒ�
            LfXlsStart(sPath, dt)
            '�w���R���{�{�b�N�X�Ƀt�H�[�J�X�Z�b�g
            cmbEki.Select()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            '�G���[���b�Z�[�W
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        Finally
            'DB�J��()
            dt = Nothing
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    '''<summary>
    ''' �u�w�v�R���{
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try

            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then
                If cmbMado.Enabled = True Then cmbMado.Enabled = False
                If btnPrint.Enabled = True Then btnPrint.Enabled = False
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            Else
                If cmbMado.Enabled = False Then
                    cmbMado.Enabled = True
                Else
                    '�o�̓{�^���L����
                    If btnPrint.Enabled = False Then
                        '�����{�^��������
                        Call LfSearchButton()
                    End If
                End If

            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbMado.SelectedIndex = 0               '���C�x���g�����ӏ�
        Catch ex As Exception
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
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
            LfPrintTrue()
        Catch ex As Exception
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' �J�n�����i�N�����j,�J�n�����i�����j,�I�������i�N�����j,�I�������i�����j
    ''' </summary>
    Private Sub dtpYmdFrom_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles dtpYmdFrom.ValueChanged, dtpYmdTo.ValueChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfPrintTrue()
            
        Catch ex As Exception
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

#End Region

#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    ''' [���[�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart(ByVal sPath As String, ByVal dt As DataTable)
        '------Ver0.1�@ADD�@START-------------
        Dim sSheet As String = ""
        Dim SheetStation As String = ""  '�e�V�[�g�̉w��
        Dim SheetCoener As String = ""  '�e�V�[�g�̃R�[�i�[��
        Dim TicketKind(15, 4) As String     '�e����̍��v�z��
        Dim TicketClear As Boolean = False   '���평��������t���O
        Dim TicketCnt As Integer = 0     '���퐔
        Dim PrCnt As Integer = 0     '�o�͍s
        Dim MaxRow As Integer = 18000    '�r���ő�s
        '------Ver0.1�@ADD�@END---------------
        'excel���ŁA�n�s��
        Dim nStartRow As Integer = 6

        '��
        Dim nY As Integer = 0

        '����
        Dim nInStatic As Long = 0
        '�o��
        Dim nOutStatic As Long = 0

        '�w�R�[�h
        Dim sStationCodeOld As String = ""
        Dim sStationCode As String = ""
        'true:��w���Ɠ����Gfalse:�����ł͂Ȃ�
        Dim isSameStation As Boolean = False
        '�R�[�i�[
        Dim sCornerCodOld As String = ""
        Dim sCornerCode As String = ""

        'true:�����Gfalse:�����ł͂Ȃ�
        Dim isCorner As Boolean = False
        '���t
        Dim dOldDate As DateTime = Nothing
        Dim dCurDate As DateTime = Nothing

        'true:�����Gfalse:�����ł͂Ȃ�
        Dim isDate As Boolean = False

        '���ԑт̎n��
        Dim dZonFrom As DateTime = DateTime.Now
        Dim dZonTo As DateTime = DateTime.Now

        Try
            With XlsReport1

                Log.Info("Start printing about [" & sPath & "].")
                ' ���[�t�@�C�����̂��擾
                .FileName = sPath
                .ExcelMode = True
                ' ���[�̏o�͏������J�n��錾
                .Report.Start()
                .Report.File()

                '------Ver0.1�@MOD�@START-------------
                sSheet = ""
                SheetStation = ""
                SheetCoener = ""
                For Rec As Integer = 0 To dt.Rows.Count - 1
                    '���o�f�[�^�ɐ���R�[�h�A�w���R�[�h������Έȉ��̏���
                    If dt.Rows(Rec)(8).ToString() & dt.Rows(Rec)(9).ToString() <> "" Then
                        '�L�[�u���[�N�F����R�[�h�A�w���R�[�h�A�R�[�i�[�R�[�h���ς��Ή��y�[�W
                        If sSheet <> dt.Rows(Rec)(8).ToString() & dt.Rows(Rec)(9).ToString() Then
                            If sSheet <> "" Then
                                '�Ō�s�A�e����̍��v���o�͂���
                                PrCnt = PrCnt + +nStartRow
                                .Pos(2, PrCnt).Attr.HorizontalAlignment = AdvanceSoftware.VBReport7.HorizontalAlignment.Right
                                .Pos(2, PrCnt).Value = "���v"
                                For RecCnt As Integer = 0 To TicketKind.GetLength(0) - 2
                                    If TicketKind(RecCnt, 0).ToString <> "" And TicketKind(RecCnt, 0).ToString <> "0" Then
                                        .Pos(5, PrCnt).Value = TicketKind(RecCnt, 0).ToString
                                        .Pos(6, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 1).ToString)
                                        .Pos(7, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 2).ToString)
                                        .Pos(8, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 3).ToString)
                                        PrCnt = PrCnt + 1
                                    End If
                                Next
                                ''�K�v�Ȃ��r���g���폜
                                For DelRs As Integer = PrCnt To MaxRow
                                    .RowClear(DelRs)
                                Next
                                PrCnt = 0
                                TicketCnt = 0
                                TicketClear = False
                                .Page.End()
                            End If
                            '���v�z�������������
                            For Cola As Integer = 0 To 14
                                For Rowe As Integer = 0 To 3
                                    TicketKind(Cola, Rowe) = "0"
                                Next
                            Next
                            '�V�[�g���ݒ�F�w�{�R�[�i�[
                            sSheet = dt.Rows(Rec)(8).ToString() & dt.Rows(Rec)(9).ToString()
                            '�V�[�g�̉w�A�R�[�i�[���擾
                            SheetCoener = dt.Rows(Rec)(9).ToString()
                            SheetStation = dt.Rows(Rec)(8).ToString()
                            '���[�t�@�C���V�[�g���̂��擾���܂��B
                            .Page.Start(LcstXlsSheetName, "1-9999")
                            .Page.Name = dt.Rows(Rec)(0).ToString() & "�@" & dt.Rows(Rec)(1).ToString()

                            ' ���o�����Z���֌��o���f�[�^�o��
                            '�^�C�g��
                            .Cell("B1").Value = LcstXlsSheetName

                            '�o�͒[��:�g�^�p�Ǘ��[���h + Config.MachineName
                            .Cell("I1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()

                            '�o�͓���
                            .Cell("I2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")

                            '�w���A�R�[�i�[
                            .Cell("B3").Value = OPMGFormConstants.STATION_NAME + dt.Rows(Rec)(0).ToString + "�@�@�@" +
                                OPMGFormConstants.CORNER_STR + dt.Rows(Rec)(1).ToString

                            '��������A�܂œ���
                            .Cell("C4").Value = Lexis.TimeSpan.Gen(Replace(Replace(Replace(dtpYmdFrom.Text, "�N", "/"), "��", "/"), "��", ""), "", _
                                                                Replace(Replace(Replace(dtpYmdTo.Text, "�N", "/"), "��", "/"), "��", ""), "")
                        End If

                        '�w��
                        nY = 0
                        '�w�R�[�h
                        sStationCode = dt.Rows(Rec)("STATION_CODE").ToString()
                        If PrCnt = 0 Then
                            sStationCodeOld = sStationCode
                            .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("STATION_NAME").ToString()
                            isSameStation = False
                        Else
                            '���w���O�w�Ɠ���
                            If sStationCodeOld.Equals(sStationCode) Then
                                .Pos(nY + 1, PrCnt + nStartRow).Value = ""
                                isSameStation = True
                            End If
                        End If

                        '�R�[�i�[
                        nY = nY + 1
                        sCornerCode = dt.Rows(Rec)("CORNER_CODE").ToString()
                        If isSameStation = False Then
                            sCornerCodOld = sCornerCode
                            isCorner = False
                            .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("CORNER_NAME").ToString()
                        Else
                            If sCornerCodOld.Equals(sCornerCode) Then
                                isCorner = True
                                .Pos(nY + 1, PrCnt + nStartRow).Value = ""
                            End If
                        End If

                        '���t
                        nY = nY + 1
                        dCurDate = CDate(dt.Rows(Rec)("DATE"))
                        If isCorner = True Then
                            If dOldDate = dCurDate Then
                                .Pos(nY + 1, PrCnt + nStartRow).Value = ""
                                isDate = True
                            Else
                                dOldDate = dCurDate
                                .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("DATE")
                                isDate = False
                            End If
                        Else
                            dOldDate = dCurDate
                            .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("DATE")
                            isDate = False
                        End If

                        '���ԑ�
                        nY = nY + 1
                        If isDate = False Then

                            '���ԑ�from,���ԑ�to
                            getTimeZone(dt.Rows(Rec)("TIME_ZONE").ToString(), dCurDate, dZonFrom, dZonTo)
                            .Pos(nY + 1, PrCnt + nStartRow).Value = Format(dZonFrom.Hour, "00") & ":" & Format(dZonFrom.Minute, "00")
                            '����N���A�����L��
                            TicketClear = True
                        Else
                            '�S���f�[�^����s�Ɠ���
                            If DateTime.Parse(dCurDate & " " & dt.Rows(Rec)(nY).ToString()) >= dZonFrom AndAlso
                                DateTime.Parse(dCurDate & " " & dt.Rows(Rec)(nY).ToString()) <= dZonTo Then
                                .Pos(nY + 1, PrCnt + nStartRow).Value = ""
                                '����N���A����𖳌�
                                TicketClear = False
                            Else
                                '���ԑ�from,���ԑ�to
                                getTimeZone(dt.Rows(Rec)("TIME_ZONE").ToString(), dCurDate, dZonFrom, dZonTo)
                                .Pos(nY + 1, PrCnt + nStartRow).Value = Format(dZonFrom.Hour, "00") & ":" & Format(dZonFrom.Minute, "00")
                                '����N���A�����L��
                                TicketClear = True
                            End If
                        End If

                        '����
                        nY = nY + 1
                        .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("TICKET_NAME").ToString()
                        '����
                        nY = nY + 1
                        nInStatic = Long.Parse(dt.Rows(Rec)("STATION_IN").ToString)
                        .Pos(nY + 1, PrCnt + nStartRow).Value = nInStatic

                        '�o��
                        nY = nY + 1
                        nOutStatic = Long.Parse(dt.Rows(Rec)("STATION_OUT").ToString)
                        .Pos(nY + 1, PrCnt + nStartRow).Value = nOutStatic

                        '���v  
                        nY = nY + 1
                        .Pos(nY + 1, PrCnt + nStartRow).Value = Long.Parse(dt.Rows(Rec)("STATION_SUM").ToString)
                        '���ԑт��ύX����鎞�A������N���A����
                        If TicketClear = True Then
                            TicketCnt = 0
                        Else
                            TicketCnt = TicketCnt + 1
                        End If
                        '���햼�̂�ݒ�
                        TicketKind(TicketCnt, 0) = dt.Rows(Rec)("TICKET_NAME").ToString
                        '���킲�Ƃ̓��ꌔ���J�E���g
                        TicketKind(TicketCnt, 1) = (Long.Parse(TicketKind(TicketCnt, 1)) + Long.Parse(dt.Rows(Rec)("STATION_IN").ToString)).ToString
                        '���킲�Ƃ̏o�ꌔ���J�E���g
                        TicketKind(TicketCnt, 2) = (Long.Parse(TicketKind(TicketCnt, 2)) + Long.Parse(dt.Rows(Rec)("STATION_OUT").ToString)).ToString
                        '���킲�Ƃ̍��v�����J�E���g
                        TicketKind(TicketCnt, 3) = (Long.Parse(TicketKind(TicketCnt, 3)) + Long.Parse(dt.Rows(Rec)("STATION_SUM").ToString)).ToString
                        PrCnt = PrCnt + 1
                    End If
                    If Rec = dt.Rows.Count - 1 Then
                        '�Ō�s�A�e����̍��v���o�͂���
                        PrCnt = PrCnt + +nStartRow
                        .Pos(2, PrCnt).Attr.HorizontalAlignment = AdvanceSoftware.VBReport7.HorizontalAlignment.Right
                        .Pos(2, PrCnt).Value = "���v"
                        For RecCnt As Integer = 0 To TicketKind.GetLength(0) - 2
                            If TicketKind(RecCnt, 0).ToString <> "" And TicketKind(RecCnt, 0).ToString <> "0" Then
                                .Pos(5, PrCnt).Value = TicketKind(RecCnt, 0).ToString
                                .Pos(6, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 1).ToString)
                                .Pos(7, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 2).ToString)
                                .Pos(8, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 3).ToString)
                                PrCnt = PrCnt + 1
                            End If
                        Next
                        ''�K�v�Ȃ��r���g���폜
                        For DelRx As Integer = PrCnt To MaxRow
                            .RowClear(DelRx)
                        Next
                        PrCnt = 0
                        TicketCnt = 0
                        TicketClear = False
                    End If
                Next
                '------Ver0.1�@MOD�@END---------------

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

    '''<summary>
    ''' ���ԑю擾
    ''' </summary>
    Private Sub getTimeZone(ByVal timezon As String, ByVal dateHourly As Date, ByRef datetimzonfrom As DateTime, ByRef datetimzonto As DateTime)

        Dim strHour As String = ""

        If timezon.IndexOf(":") > 0 Then
            '���ԑђ��@���̎擾
            strHour = timezon.Substring(0, timezon.IndexOf(":"))
        Else
            '���ԑђ��@���̎擾
            strHour = "00"
        End If

        Dim datSmal As DateTime = DateTime.Parse(dateHourly & " " & strHour & ":00")
        Dim datBig As DateTime = DateTime.Parse(dateHourly & " " & strHour & ":30")

        If DateTime.Parse(dateHourly & " " & timezon) >= datSmal AndAlso
            DateTime.Parse(dateHourly & " " & timezon) < datBig Then
            datetimzonfrom = datSmal
            datetimzonto = DateTime.Parse(dateHourly & " " & strHour & ":29")
        Else
            datetimzonfrom = datBig
            datetimzonto = DateTime.Parse(dateHourly & " " & strHour & ":59")
        End If

    End Sub

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
       
        dtpYmdTo.Format = DateTimePickerFormat.Custom
        dtpYmdTo.CustomFormat = "yyyy�NMM��dd��"
        dtpYmdTo.Value = dtTo
        
    End Sub

    ''' <summary>
    ''' [�o�̓{�^��������]
    ''' </summary>
    Private Sub LfPrintTrue()
        Dim bEnabled As Boolean
        Dim sFrom As String = String.Format("{0} {1}", dtpYmdFrom.Text, "00:00")
        Dim sTo As String = String.Format("{0} {1}", dtpYmdTo.Text, "23:59")
        If sFrom > sTo Then
            bEnabled = False
        Else
            bEnabled = True
        End If
        If bEnabled Then
            If ((cmbEki.SelectedIndex < 0) OrElse _
                (cmbMado.SelectedIndex < 0)) Then
                bEnabled = False
            End If
        End If
        If bEnabled Then

            If btnPrint.Enabled = False Then
                '�����{�^��������
                Call LfSearchButton()
            End If

        Else
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
        End If
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
            dt = oMst.SelectTable(True, "G")
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
            If Station <> "" AndAlso Station <> ClientDaoConstants.TERMINAL_ALL Then
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
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [�����pSELECT������擾]
    ''' </summary>
    ''' <returns>SELECT��</returns>
    Private Function LfGetSelectString(ByVal slcSQLType As SlcSQLType) As String

        Dim sSql As StringBuilder = New StringBuilder()
        Try
            Dim sSqlWhere As StringBuilder = New StringBuilder()
            Dim sFrom As String = ""
            Dim sTo As String = ""
            Dim sTabName As String = "V_TRAFFIC_DATA"
            Dim sEki As String

            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '�����擾����--------------------------
                    sSql.AppendLine("SELECT COUNT(1) FROM " + sTabName)
                Case slcSQLType.SlcDetail
                    '�擾����--------------------------
                    sSql.AppendLine("SELECT STATION_NAME,CORNER_NAME,")
                    '----Ver0.1 MOD START----------------------------
                    sSql.AppendLine("DATE,TIME_ZONE,TICKET_NAME,STATION_IN,STATION_OUT,STATION_SUM,STATION_CODE,CORNER_CODE,TICKET_NO ")
                    '----Ver0.1 MOD END------------------------------
                    sSql.AppendLine(" FROM " + sTabName)
            End Select

            'Where�吶��--------------------------
            sSql.AppendLine(" where 0=0 ")
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
            '�R�[�i�[
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then

                sSqlWhere.AppendLine(String.Format(" and (CORNER_CODE = {0})", Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If

            '�J�n�I������
            sFrom = (Replace(Replace(Replace(dtpYmdFrom.Text, "�N", ""), "��", ""), "��", ""))

            sTo = (Replace(Replace(Replace(dtpYmdTo.Text, "�N", ""), "��", ""), "��", ""))

            sSqlWhere.AppendLine(" And")
            sSqlWhere.AppendLine("( (SUBSTRING([DATE],1,4)+SUBSTRING([DATE],6,2)+SUBSTRING([DATE],9,2)) >= ")
            sSqlWhere.AppendLine("'" + sFrom.ToString + "'")
            sSqlWhere.AppendLine(" and (SUBSTRING([DATE],1,4)+SUBSTRING([DATE],6,2)+SUBSTRING([DATE],9,2)) <= ")
            sSqlWhere.AppendLine("'" + sTo.ToString + "'")
            sSqlWhere.AppendLine(")")

            Select Case slcSQLType
                Case slcSQLType.SlcDetail
                    '�擾����--------------------------
                    sSqlWhere.AppendLine(" order by STATION_CODE,CORNER_CODE,[DATE],TIME_ZONE,TICKET_NO")
            End Select

            'Where�匋��
            sSql.AppendLine(sSqlWhere.ToString)

            Return sSql.ToString
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Function

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
            btnPrint.Enabled = True
        Else
            btnPrint.Enabled = False
        End If
    End Sub

#End Region

End Class