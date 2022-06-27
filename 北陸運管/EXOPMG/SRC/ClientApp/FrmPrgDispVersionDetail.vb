' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
'   0.1      2015/01/13  (NES)����    �����ΏۊOPG��\���Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO

''' <summary>�v���O�����o�[�W�����ڍו\��</summary>
''' <remarks>
''' �v���O�����o�[�W�����ڍו\��
''' </remarks>
Public Class FrmPrgDispVersionDetail
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
    Friend WithEvents istDispApp As System.Windows.Forms.ImageList
    Friend WithEvents WorkBook1 As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents cmbPrg As System.Windows.Forms.ComboBox
    Friend WithEvents lblPrg As System.Windows.Forms.Label
    Friend WithEvents cmbUnit As System.Windows.Forms.ComboBox
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents cmbCorner As System.Windows.Forms.ComboBox
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblUnit As System.Windows.Forms.Label
    Friend WithEvents lblModel As System.Windows.Forms.Label
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents lblState As System.Windows.Forms.Label
    Friend WithEvents cmbState As System.Windows.Forms.ComboBox
    Friend WithEvents shtVerDetail As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmPrgDispVersionDetail))
        Me.istDispApp = New System.Windows.Forms.ImageList(Me.components)
        Me.WorkBook1 = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtVerDetail = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.cmbPrg = New System.Windows.Forms.ComboBox()
        Me.lblPrg = New System.Windows.Forms.Label()
        Me.cmbUnit = New System.Windows.Forms.ComboBox()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.cmbCorner = New System.Windows.Forms.ComboBox()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblUnit = New System.Windows.Forms.Label()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.lblState = New System.Windows.Forms.Label()
        Me.cmbState = New System.Windows.Forms.ComboBox()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.WorkBook1.SuspendLayout()
        CType(Me.shtVerDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.cmbState)
        Me.pnlBodyBase.Controls.Add(Me.lblState)
        Me.pnlBodyBase.Controls.Add(Me.WorkBook1)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.cmbPrg)
        Me.pnlBodyBase.Controls.Add(Me.lblPrg)
        Me.pnlBodyBase.Controls.Add(Me.cmbUnit)
        Me.pnlBodyBase.Controls.Add(Me.cmbModel)
        Me.pnlBodyBase.Controls.Add(Me.cmbCorner)
        Me.pnlBodyBase.Controls.Add(Me.cmbEki)
        Me.pnlBodyBase.Controls.Add(Me.lblUnit)
        Me.pnlBodyBase.Controls.Add(Me.lblModel)
        Me.pnlBodyBase.Controls.Add(Me.lblMado)
        Me.pnlBodyBase.Controls.Add(Me.lblEki)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnKensaku)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/08/02(��)  16:27"
        '
        'istDispApp
        '
        Me.istDispApp.ImageStream = CType(resources.GetObject("istDispApp.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.istDispApp.TransparentColor = System.Drawing.Color.White
        Me.istDispApp.Images.SetKeyName(0, "")
        Me.istDispApp.Images.SetKeyName(1, "")
        '
        'WorkBook1
        '
        Me.WorkBook1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.WorkBook1.Controls.Add(Me.shtVerDetail)
        Me.WorkBook1.Location = New System.Drawing.Point(22, 84)
        Me.WorkBook1.Name = "WorkBook1"
        Me.WorkBook1.ProcessTabKey = False
        Me.WorkBook1.ShowTabs = False
        Me.WorkBook1.Size = New System.Drawing.Size(919, 479)
        Me.WorkBook1.TabFont = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.WorkBook1.TabIndex = 7
        '
        'shtVerDetail
        '
        Me.shtVerDetail.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtVerDetail.Data = CType(resources.GetObject("shtVerDetail.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtVerDetail.Location = New System.Drawing.Point(1, 1)
        Me.shtVerDetail.Name = "shtVerDetail"
        Me.shtVerDetail.Size = New System.Drawing.Size(900, 460)
        Me.shtVerDetail.TabIndex = 7
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(704, 584)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 8
        Me.btnPrint.Text = "�o�@��"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'cmbPrg
        '
        Me.cmbPrg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPrg.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbPrg.ItemHeight = 13
        Me.cmbPrg.Location = New System.Drawing.Point(269, 48)
        Me.cmbPrg.Name = "cmbPrg"
        Me.cmbPrg.Size = New System.Drawing.Size(220, 21)
        Me.cmbPrg.TabIndex = 5
        '
        'lblPrg
        '
        Me.lblPrg.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrg.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrg.Location = New System.Drawing.Point(163, 50)
        Me.lblPrg.Name = "lblPrg"
        Me.lblPrg.Size = New System.Drawing.Size(108, 18)
        Me.lblPrg.TabIndex = 93
        Me.lblPrg.Text = "�v���O��������"
        Me.lblPrg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbUnit
        '
        Me.cmbUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbUnit.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbUnit.ItemHeight = 13
        Me.cmbUnit.Location = New System.Drawing.Point(783, 16)
        Me.cmbUnit.Name = "cmbUnit"
        Me.cmbUnit.Size = New System.Drawing.Size(70, 21)
        Me.cmbUnit.TabIndex = 4
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.ItemHeight = 13
        Me.cmbModel.Location = New System.Drawing.Point(70, 16)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(126, 21)
        Me.cmbModel.TabIndex = 1
        '
        'cmbCorner
        '
        Me.cmbCorner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCorner.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbCorner.ItemHeight = 13
        Me.cmbCorner.Location = New System.Drawing.Point(551, 16)
        Me.cmbCorner.Name = "cmbCorner"
        Me.cmbCorner.Size = New System.Drawing.Size(162, 21)
        Me.cmbCorner.TabIndex = 3
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Location = New System.Drawing.Point(269, 16)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(162, 21)
        Me.cmbEki.TabIndex = 2
        '
        'lblUnit
        '
        Me.lblUnit.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblUnit.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUnit.Location = New System.Drawing.Point(745, 18)
        Me.lblUnit.Name = "lblUnit"
        Me.lblUnit.Size = New System.Drawing.Size(44, 18)
        Me.lblUnit.TabIndex = 92
        Me.lblUnit.Text = "���@"
        Me.lblUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(34, 17)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(44, 18)
        Me.lblModel.TabIndex = 91
        Me.lblModel.Text = "�@��"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMado
        '
        Me.lblMado.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMado.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMado.Location = New System.Drawing.Point(486, 18)
        Me.lblMado.Name = "lblMado"
        Me.lblMado.Size = New System.Drawing.Size(64, 18)
        Me.lblMado.TabIndex = 90
        Me.lblMado.Text = "�R�[�i�["
        Me.lblMado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblEki
        '
        Me.lblEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblEki.Location = New System.Drawing.Point(232, 17)
        Me.lblEki.Name = "lblEki"
        Me.lblEki.Size = New System.Drawing.Size(46, 18)
        Me.lblEki.TabIndex = 89
        Me.lblEki.Text = "�w��"
        Me.lblEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 9
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(872, 32)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 7
        Me.btnKensaku.Text = "���@��"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'lblState
        '
        Me.lblState.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblState.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblState.Location = New System.Drawing.Point(515, 50)
        Me.lblState.Name = "lblState"
        Me.lblState.Size = New System.Drawing.Size(44, 18)
        Me.lblState.TabIndex = 95
        Me.lblState.Text = "���"
        Me.lblState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbState
        '
        Me.cmbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbState.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbState.ItemHeight = 13
        Me.cmbState.Location = New System.Drawing.Point(552, 48)
        Me.cmbState.Name = "cmbState"
        Me.cmbState.Size = New System.Drawing.Size(70, 21)
        Me.cmbState.TabIndex = 6
        '
        'FrmPrgDispVersionDetail
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgDispVersionDetail"
        Me.Text = "�^�p�[�� "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.WorkBook1.ResumeLayout(False)
        CType(Me.shtVerDetail, System.ComponentModel.ISupportInitialize).EndInit()
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
    Private ReadOnly LcstXlsTemplateName As String = "�v���O�����o�[�W�������.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "�v���O�����o�[�W�������"

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "�v���O�����o�[�W�����ڍו\��"

    ''' <summary>
    ''' ���[�o�͑Ώۗ�̊��蓖��
    ''' �i���������ʏW�D�f�[�^�ɑ΂����[�o�͗���`�j
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

    ''' <summary>
    ''' �ꗗ�\���ő��
    ''' </summary>
    Private LcstMaxColCnt As Integer

    Private LbInitCallFlg As Boolean = False

    '�O�̉�ʂ���n���ꂽ�f�@��R�[�h�f���󂯎��
    Private sCmbModel As Integer
    '�O�̉�ʂ���n���ꂽ�f ����R�[�h�f���󂯎��
    Private sBtnRail As String
    '�O�̉�ʂ���n���ꂽ�f �w���R�[�h�f���󂯎��
    Private sBtnStation As String

    Public Property sCmbValue() As Integer
        Get
            Return sCmbModel
        End Get
        Set(ByVal value As Integer)
            sCmbModel = value
        End Set
    End Property

    Public Property sBtnName() As String
        Get
            Return sBtnRail
        End Get
        Set(ByVal value As String)
            sBtnRail = value
        End Set
    End Property

    Public Property sBtnTag() As String
        Get
            Return sBtnStation
        End Get
        Set(ByVal value As String)
            sBtnStation = value
        End Set
    End Property
#End Region

#Region "��ʂ̃f�[�^����������"
    ''' <summary>��ʂ̃f�[�^����������</summary>
    ''' <remarks>
    '''�f�[�^���������A��ʂɕ\������
    ''' </remarks>   
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        Dim nEkiIndex As Integer
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e

        Try
            Log.Info("Method started.")

            '�O�̉�ʂ���n���ꂽ�l���󂯎�邩�𔻒f����
            If String.IsNullOrEmpty(sBtnRail) Or String.IsNullOrEmpty(sBtnStation) Then
                '��ʕ\�������Ɏ��s���܂���
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
                Return False
            Else

                '��ʃ^�C�g��
                lblTitle.Text = LcstFormTitle

                '�V�[�g������
                shtVerDetail.TransformEditor = False                                     '�ꗗ�̗��ޖ��̃`�F�b�N�𖳌��ɂ���
                shtVerDetail.ViewMode = ElTabelleSheet.ViewMode.Row                      '�s�I�����[�h
                shtVerDetail.MaxRows() = 0                                               '�s�̏�����
                LcstMaxColCnt = shtVerDetail.MaxColumns()                                '�񐔂��擾
                shtVerDetail.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   '�V�[�g��\�����[�h

                '�@�햼�̂�ݒ肷��B
                If setCmbModel() = False Then Exit Try
                cmbModel.SelectedIndex = sCmbModel          '�f�t�H���g�\������

                If setCmbEki(cmbModel.SelectedValue.ToString) = False Then Exit Try
                nEkiIndex = getIndex(CType(cmbEki.DataSource, DataTable), sBtnRail & sBtnStation)
                cmbEki.SelectedIndex = nEkiIndex          '�f�t�H���g�\������

                If setCmbCorner(cmbModel.SelectedValue.ToString, cmbEki.SelectedValue.ToString) = False Then Exit Try
                cmbCorner.SelectedIndex = 0          '�f�t�H���g�\������

                If setCmbUnit(cmbModel.SelectedValue.ToString, cmbEki.SelectedValue.ToString, cmbCorner.SelectedValue.ToString) = False Then Exit Try
                cmbUnit.SelectedIndex = 0          '�f�t�H���g�\������

                If setCmbPrg(cmbModel.SelectedValue.ToString) = False Then Exit Try
                cmbPrg.SelectedIndex = 0          '�f�t�H���g�\������

                Call setCmbState()
                Call initElTable()

            End If

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

#Region "�t�H�[�����[�h"


    ''' <summary>�t�H�[�����[�h</summary>
    ''' <remarks>
    '''  ��ʃ^�C�g���A��ʔw�i�F�iBackColor�j��ݒ肵�AELTable��\������B
    ''' �u�w���v������������
    ''' </remarks>
    Private Sub FrmPrgDispVersionDetail_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

#Region "�R���{�{�b�N�X�ݒ�"
    ''' <summary>
    ''' �@�햼�̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���@�햼�̂̈ꗗ�y�сu�S�@��v��ݒ肷��B</remarks>
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
            dt = oMst.SetAll()

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
    ''' �w���̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <param name="Model">�@��R�[�h</param>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���w���̂̈ꗗ�y�сu�S�w�v��ݒ肷��B</remarks>
    Private Function setCmbEki(ByVal Model As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New StationMaster
        Dim sModel As String

        Try
            If Model = ClientDaoConstants.TERMINAL_ALL Then
                sModel = "G,Y,W"
            Else
                sModel = Model
            End If

            '�w���̃R���{�{�b�N�X�p�̃f�[�^���擾����B
            dt = oMst.SelectTable(False, sModel)
            If dt.Rows.Count = 0 Then
                '�w�f�[�^�擾���s
                Return bRtn
            End If
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
    ''' �R�[�i�[���̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <param name="Model">�@��R�[�h</param>
    ''' <param name="Station">�w�R�[�h</param>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���R�[�i�[���̂̈ꗗ�y�сu�S�R�[�i�[�v��ݒ肷��B</remarks>
    Private Function setCmbCorner(ByVal Model As String, ByVal Station As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New CornerMaster
        Dim sModel As String

        Try
            If Station <> ClientDaoConstants.TERMINAL_ALL Then
                If Model = ClientDaoConstants.TERMINAL_ALL Then
                    sModel = "G,Y,W"
                Else
                    sModel = Model
                End If

                '�R�[�i�[���̃R���{�{�b�N�X�p�̃f�[�^���擾����B
                dt = oMst.SelectTable(Station, sModel)
                If dt.Rows.Count = 0 Then
                    '�R�[�i�[�f�[�^�擾���s
                    Return bRtn
                End If
            End If
            dt = oMst.SetAll()

            bRtn = BaseSetMstDtToCmb(dt, cmbCorner)
            cmbCorner.SelectedIndex = -1
            If cmbCorner.Items.Count <= 0 Then bRtn = False

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
    ''' ���@���̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <param name="Model">�@��R�[�h</param>
    ''' <param name="Station">�w�R�[�h</param>
    ''' <param name="Corner">�R�[�i�[�R�[�h</param>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă��鍆�@���̂̈ꗗ�y�сu�S���@�v��ݒ肷��B</remarks>
    Private Function setCmbUnit(ByVal Model As String, ByVal Station As String, ByVal Corner As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New UnitMaster
        Dim sModel As String

        Try
            If Corner <> ClientDaoConstants.TERMINAL_ALL Then
                If Model = ClientDaoConstants.TERMINAL_ALL Then
                    sModel = "G,Y,W"
                Else
                    sModel = Model
                End If

                '���@���̃R���{�{�b�N�X�p�̃f�[�^���擾����B
                dt = oMst.SelectTable(Station, Corner, sModel)
                If dt.Rows.Count = 0 Then
                    '���@�f�[�^�擾���s
                    Return bRtn
                End If
            End If
            dt = oMst.SetAll()

            bRtn = BaseSetMstDtToCmb(dt, cmbUnit)
            cmbUnit.SelectedIndex = -1
            If cmbUnit.Items.Count <= 0 Then bRtn = False

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
    ''' �v���O�������̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <param name="Model">�@��R�[�h</param>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���}�X�^���̂̈ꗗ�y�сu�S�}�X�^�v��ݒ肷��B</remarks>
    Private Function setCmbPrg(ByVal Model As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New ProgramMaster
        Dim sModel As String

        Try
            If Model = ClientDaoConstants.TERMINAL_ALL Then
                sModel = "G,Y,W"
            Else
                sModel = Model
            End If

            '�}�X�^���̃R���{�{�b�N�X�p�̃f�[�^���擾����B
            dt = oMst.SelectTable2(sModel)
            If dt.Rows.Count = 0 Then
                '�}�X�^�f�[�^�擾���s
                Return bRtn
            End If
            dt = oMst.SetAll()

            bRtn = BaseSetMstDtToCmb(dt, cmbPrg)
            cmbPrg.SelectedIndex = -1
            If cmbPrg.Items.Count <= 0 Then bRtn = False

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
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

    ''' <summary>�u��ԁv�R���{�{�b�N�X�����������A�l��������B</summary>
    Private Sub setCmbState()

        Me.cmbState.Items.Clear()

        Me.cmbState.Items.Add("�S��")
        Me.cmbState.Items.Add("�ُ�")
        Me.cmbState.Items.Add("����")
        Me.cmbState.Items.Add("�z�M��")

        '�u��ԁv��S�Ăɐݒ肷��
        cmbState.SelectedIndex = 1

    End Sub

#End Region

#Region "�R���{�I����"

    ''' <summary>�@��R���{�I����</summary>
    ''' <remarks>
    ''' �Ή�����u�@��v�R���{�{�b�N�X�ɒl�������A���̃R���{�{�b�N�X�̃v���p�e�B��ݒ肷��
    ''' </remarks>
    Private Sub cmbModel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            '�w���R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbEki(cmbModel.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblEki.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            '�}�X�^���R���{�ݒ�
            If setCmbPrg(cmbModel.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblPrg.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbEki.SelectedIndex = 0               '���C�x���g�����ӏ�
            cmbPrg.SelectedIndex = 0               '���C�x���g�����ӏ�

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblEki.Text)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>�w���R���{�I����</summary>
    ''' <remarks>
    ''' �Ή�����u�R�[�i�[�v�R���{�{�b�N�X�ɒl�������A���̃R���{�{�b�N�X�̃v���p�e�B��ݒ肷��
    ''' </remarks>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            '�R�[�i�[�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbCorner(cmbModel.SelectedValue.ToString, cmbEki.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbCorner.SelectedIndex = 0               '���C�x���g�����ӏ�

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblEki.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>�R�[�i�[�R���{�I����</summary>
    ''' <remarks>
    ''' �Ή�����u�R�[�i�[�v�R���{�{�b�N�X�ɒl�������A���̃R���{�{�b�N�X�̃v���p�e�B��ݒ肷��
    ''' </remarks>
    Private Sub cmbCorner_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCorner.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            '���@�R���{�ݒ�
            LbEventStop = True      '�C�x���g�����n�e�e
            If setCmbUnit(cmbModel.SelectedValue.ToString, cmbEki.SelectedValue.ToString, cmbCorner.SelectedValue.ToString) = False Then
                '�G���[���b�Z�[�W
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblUnit.Text)
                LbEventStop = False      '�C�x���g�����n�m
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      '�C�x���g�����n�m
            cmbUnit.SelectedIndex = 0               '���C�x���g�����ӏ�

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>���@�R���{�I����</summary>
    ''' <remarks>
    ''' �Ή�����u���@�v�R���{�{�b�N�X�ɒl�������A���̃R���{�{�b�N�X�̃v���p�e�B��ݒ肷��
    ''' </remarks>
    Private Sub cmbUnit_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbUnit.SelectedIndexChanged

        Call initElTable()

    End Sub

    ''' <summary>�v���O�������̃R���{�I����</summary>
    ''' <remarks>
    ''' �Ή�����u�v���O�������́v�R���{�{�b�N�X�ɒl�������A���̃R���{�{�b�N�X�̃v���p�e�B��ݒ肷��
    ''' </remarks>
    Private Sub cmbPrg_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbPrg.SelectedIndexChanged

        Call initElTable()

    End Sub

    ''' <summary>�{�^���u�����v�̗��p�\����ݒ肷��B</summary>
    ''' <remarks>
    ''' �u��ԁv��Emable�l�ɂ���āA�u�����v�{�^���̏�Ԃ𔻒f����
    ''' </remarks>
    Private Sub cmbState_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbState.SelectedIndexChanged

        Call initElTable()

    End Sub

#End Region

#Region " ��ʕ\���pSQL�쐬 "

    ''' <summary>��ʕ\���pSQL�쐬</summary>
    ''' <returns>SQL��</returns>
    Private Function makeSql() As String

        Dim sSQL As String = ""
        Dim sSubSQL As String = ""

        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            If cmbPrg.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
                sSubSQL = " WHERE MODEL_CODE='" & cmbPrg.SelectedValue.ToString & "'"
            Else
                sSubSQL = " WHERE (MODEL_CODE='G' OR MODEL_CODE='Y' OR MODEL_CODE='W')"
            End If
        Else
            sSubSQL = " WHERE MODEL_CODE='" & cmbModel.SelectedValue.ToString & "'"
        End If

        If cmbEki.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
            sSubSQL = sSubSQL & " AND RAIL_SECTION_CODE+STATION_ORDER_CODE='" & cmbEki.SelectedValue.ToString & "'"

            If cmbCorner.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
                sSubSQL = sSubSQL & " AND CORNER_CODE='" & cmbCorner.SelectedValue.ToString & "'"

                If cmbUnit.SelectedValue.ToString <> "" Then
                    sSubSQL = sSubSQL & " AND UNIT_NO='" & cmbUnit.SelectedValue.ToString & "'"
                End If
            End If
        End If

        'TODO: 20130722�b��Ή��������A�܂��݌v�ʂ�ł͂Ȃ��B
        'ELEMENT_NAME�̑I����MIN()�ōs���̂ł͂Ȃ��A���Y�̃��R�[�h��
        'S_PRG_VER_INFO_EXPECTED�ɓo�^����Ă���΂���ELEMENT_NAME�A
        '�����ɓo�^����Ă��Ȃ����D_PRG_VER_INFO_NEW��ELEMENT_NAME�A
        '�����ɂ��o�^����Ă��Ȃ����D_PRG_VER_INFO_CUR��ELEMENT_NAME
        '�Ƃ���ׂ��B
        '-----Ver0.1�@�����ΏۊOPG��\���Ή��@�@MOD�@START---------------------------------
        sSQL = "SELECT" _
            & "     STATION_NAME,CORNER_NAME,MODEL_NAME,MAC.UNIT_NO,ELEMENT_NAME," _
            & "     VERSION1,VERSION2,VERSION3," _
            & "     CASE" _
            & "         WHEN VERSION1 = '' THEN '����'" _
            & "         WHEN (VERSION1 = VERSION3) AND (VERSION2 = '') THEN '����'" _
            & "         WHEN (VERSION2 = VERSION3) AND (VERSION3 <> '') THEN '�z�M��'" _
            & "         ELSE '�ُ�'" _
            & "     END AS STS," _
            & "     UP_DATE" _
            & " FROM" _
            & "     (" _
            & "         SELECT" _
            & "             STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "             CORNER_NAME,CORNER_CODE,MODEL_CODE,MODEL_NAME,UNIT_NO" _
            & "         FROM" _
            & "             V_MACHINE_NOW" _
            & "     ) AS MAC," _
            & "     (" _
            & "         SELECT" _
            & "             MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "             CORNER_CODE,UNIT_NO,ELEMENT_ID,MIN(ELEMENT_NAME) AS ELEMENT_NAME," _
            & "             MAX(VERSION1) AS VERSION1,MAX(VERSION2) AS VERSION2," _
            & "             MAX(VERSION3) AS VERSION3,MAX(UP_DATE) AS UP_DATE" _
            & "         FROM" _
            & "             (" _
            & "                 SELECT" _
            & "                     MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                     CORNER_CODE,UNIT_NO,ELEMENT_ID,ELEMENT_NAME," _
            & "                     ELEMENT_VERSION AS VERSION1,'' AS VERSION2,'' AS VERSION3," _
            & "                     ISNULL(CONVERT(CHAR(10),UPDATE_DATE,111)+' '" _
            & "                     +CONVERT(CHAR(8),UPDATE_DATE,108),'') AS UP_DATE" _
            & "                 FROM" _
            & "                     D_PRG_VER_INFO_CUR" _
            & sSubSQL _
            & "                 UNION" _
            & "                 SELECT" _
            & "                     MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                     CORNER_CODE,UNIT_NO,ELEMENT_ID,ELEMENT_NAME," _
            & "                     '' AS VERSION1,ELEMENT_VERSION AS VERSION2," _
            & "                     '' AS VERSION3," _
            & "                     ISNULL(CONVERT(CHAR(10),UPDATE_DATE,111)+' '" _
            & "                     +CONVERT(CHAR(8),UPDATE_DATE,108),'') AS UP_DATE" _
            & "                 FROM" _
            & "                     D_PRG_VER_INFO_NEW" _
            & sSubSQL _
            & "                 UNION" _
            & "                 SELECT" _
            & "                     MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                     CORNER_CODE,UNIT_NO,ELEMENT_ID,ELEMENT_NAME," _
            & "                     '' AS VERSION1,'' AS VERSION2," _
            & "                     ELEMENT_VERSION AS VERSION3,'' AS UP_DATE" _
            & "                 FROM" _
            & "                     S_PRG_VER_INFO_EXPECTED" _
            & sSubSQL _
            & "             ) AS PR" _
            & "         GROUP BY" _
            & "             MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "             UNIT_NO,ELEMENT_ID" _
            & "     ) AS PRG" _
            & " WHERE" _
            & "     MAC.RAIL_SECTION_CODE=PRG.RAIL_SECTION_CODE" _
            & " AND MAC.STATION_ORDER_CODE=PRG.STATION_ORDER_CODE" _
            & " AND MAC.CORNER_CODE=PRG.CORNER_CODE" _
            & " AND MAC.MODEL_CODE=PRG.MODEL_CODE" _
            & " AND MAC.UNIT_NO=PRG.UNIT_NO"
        '-----Ver0.1�@�����ΏۊOPG��\���Ή��@�@MOD�@END---------------------------------
        '-----Ver0.1�@�����ΏۊOPG��\���Ή��@�@ADD�@START-------------------------------
        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            sSQL = sSQL & " AND ( PRG.MODEL_CODE='W' OR PRG.MODEL_CODE='G' OR ((PRG.MODEL_CODE='Y') AND((PRG.UP_DATE<>'' AND (VERSION1<>'' OR VERSION2<>'')) OR (PRG.UP_DATE='' AND VERSION3<>''))))"
        ElseIf cmbModel.SelectedValue.ToString = "Y" Then
            sSQL = sSQL & " AND ((PRG.MODEL_CODE='Y') AND((PRG.UP_DATE<>'' AND (VERSION1<>'' OR VERSION2<>'')) OR (PRG.UP_DATE='' AND VERSION3<>'')))"
        End If
        '-----Ver0.1�@�����ΏۊOPG��\���Ή��@�@ADD�@END---------------------------------
        If cmbState.SelectedIndex = 3 Then
            sSQL = sSQL & " AND (VERSION2=VERSION3 AND VERSION3<>'' AND VERSION1<>'')"
        ElseIf cmbState.SelectedIndex = 2 Then
            sSQL = sSQL & " AND ((VERSION1=VERSION3 AND VERSION2='') OR VERSION1='')"
        ElseIf cmbState.SelectedIndex = 1 Then
            sSQL = sSQL & " AND NOT (VERSION2=VERSION3 AND VERSION3<>'' AND VERSION1<>'') AND NOT ((VERSION1=VERSION3 AND VERSION2='') OR VERSION1='')"
        End If

        Return sSQL

    End Function

#End Region

#Region " ELTable�̃N���A "

    ''' <summary>ELTable�̃N���A</summary>
    ''' <remarks>
    ''' Eltable�ɂ���f�[�^���N���A
    ''' </remarks>
    Private Sub initElTable()

        'Eltable�̃J�����g�̍ő包��
        Dim sXYRange As String = ""

        '��ʂ̑M����h������
        shtVerDetail.Redraw = False

        If shtVerDetail.MaxRows > 0 Then
            'Eltable�̃J�����g�̍ő包�����擾����B
            sXYRange = "1:" & shtVerDetail.MaxRows.ToString

            '�I�����ꂽ�G���A�̃f�[�^���N���A����B
            shtVerDetail.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
        End If

        shtVerDetail.MaxRows = 0
        If btnPrint.Enabled = True Then btnPrint.Enabled = False

        '��ʂ̑M����h������
        shtVerDetail.Redraw = True

    End Sub

#End Region

#Region " Eltable�̓��e��\������ "

    ''' <summary>Eltable�̓��e��\������</summary>
    ''' <remarks>
    ''' Eltable�̓��e��\������
    ''' </remarks>
    ''' <param name="dt">��������</param>
    Private Sub displayData(ByVal dt As DataTable)
        Dim i As Integer

        '��ʂ̑M����h���B
        Me.shtVerDetail.Redraw = False

        Try
            'Eltable�̍ő包����ݒ肷��B
            Me.shtVerDetail.MaxRows = dt.Rows.Count

            shtVerDetail.Rows.SetAllRowsHeight(21)

            '�f�[�^�̃o�C���h�B
            Me.shtVerDetail.DataSource = dt

            '�s�v�ȏ��͉�ʏ�ŉB��
            For i = 12 To dt.Columns.Count - 1
                shtVerDetail.Columns(i).Hidden = True
            Next

            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i).Item("STS").ToString = "�ُ�" Then
                    shtVerDetail.Rows(i).BackColor = Color.Red
                ElseIf dt.Rows(i).Item("STS").ToString = "�z�M��" Then
                    shtVerDetail.Rows(i).BackColor = Color.Yellow
                End If
            Next

        Catch ex As Exception

            Throw New DatabaseException(ex)

        Finally

            'Eltable���ĕ\������B
            Me.shtVerDetail.Redraw = True

        End Try

    End Sub

#End Region

#Region " �{�^���̏��� "

    ''' <summary>�u�����v�{�^���̏��� </summary>
    ''' <remarks>
    ''' �u�����v�{�^������������ƁA��ʂŕ\������
    ''' </remarks>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKensaku.Click
        Dim sSQL As String = ""
        Dim Cnt As Integer
        Dim dtData As New DataTable

        LogOperation(sender, e)    '�{�^���������O
        Try
            Call waitCursor(True)

            sSQL = makeSql()

            Cnt = BaseSqlDataTableFill(sSQL, dtData)
            Select Case Cnt
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                Case 0              '�Y���Ȃ�
                    AlertBox.Show(Lexis.NoRecordsFound)
                    cmbEki.Select()
                Case Else

                    '�u�o�́v�{�^�����
                    If btnPrint.Enabled = False Then btnPrint.Enabled = True
                    'ELTable�̃N���A
                    Call initElTable()

                    'Eltable�̓��e��\������B
                    Call displayData(dtData)

            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W
            btnReturn.Select()

        Finally
            dtData = Nothing
            Call waitCursor(False)
        End Try

    End Sub

    ''' <summary>�u�I���v�{�^���̏��� </summary>
    ''' <remarks>
    ''' �u�I���v�{�^������������ƁA�{��ʂ��I�������
    ''' </remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        '�I���{�^�������B
        LogOperation(sender, e)    '�{�^���������O
        Me.Close()

    End Sub

#End Region

#Region "�u�o�́v�{�^���N���b�N"

    ''' <summary>
    ''' �u�o�́v�{�^���N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>    ''' 
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
#End Region

#Region "�v���O�����o�[�W�������@���[�o��"
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
                .Cell("K1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("K2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.EQUIPMENT_TYPE_NAME + cmbModel.Text.Trim + "   " _
                                  + OPMGFormConstants.STATION_NAME + Me.cmbEki.Text.Trim + "  " _
                                  + OPMGFormConstants.CORNER_STR + Me.cmbCorner.Text.Trim + "  " _
                                  + OPMGFormConstants.NUM_EQUIPMENT + Me.cmbUnit.Text.Trim
                .Cell("B4").Value = OPMGFormConstants.PRO_NAME + Me.cmbPrg.Text.Trim + "   " _
                                  + OPMGFormConstants.STATUS_STR + Me.cmbState.Text.Trim

                ' �z�M�Ώۂ̃f�[�^�����擾���܂�
                nRecCnt = shtVerDetail.MaxRows

                ' �f�[�^�����̌r���g���쐬
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                '�f�[�^�����̒l�Z�b�g
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtVerDetail.Item(LcstPrntCol(x), y).Text
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
