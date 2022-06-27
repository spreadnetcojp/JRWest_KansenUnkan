' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/06/01  (NES)�͘e  �k���Ή��F�O���[�v�Ή��ɔ����o�^�X�V�`�F�b�N�̕ύX
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '�萔�l�̂ݎg�p
Imports JR.ExOpmg.ClientApp.FMTStructure
Imports GrapeCity.Win
Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' �y�^�ǐݒ�Ǘ��@��ʃN���X�z
''' </summary>
Public Class FrmSysUnKanSetMng
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
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnReader As System.Windows.Forms.Button
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmbEki As System.Windows.Forms.Label
    Friend WithEvents dtpYmdTo As System.Windows.Forms.Label
    Friend WithEvents dtpHmFrom As System.Windows.Forms.Label
    Friend WithEvents dtpYmdFrom As System.Windows.Forms.Label
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents labEki As System.Windows.Forms.Label

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysUnKanSetMng))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnReader = New System.Windows.Forms.Button()
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.dtpYmdTo = New System.Windows.Forms.Label()
        Me.dtpHmFrom = New System.Windows.Forms.Label()
        Me.dtpYmdFrom = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmbEki = New System.Windows.Forms.Label()
        Me.labEki = New System.Windows.Forms.Label()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.pnlBodyBase.SuspendLayout()
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
        Me.pnlBodyBase.Controls.Add(Me.pnlEki)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnReader)
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 87)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/04/03(��)  21:13"
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
        Me.btnPrint.Location = New System.Drawing.Point(744, 162)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 2
        Me.btnPrint.Text = "�o�@�^"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(744, 265)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 3
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnReader
        '
        Me.btnReader.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReader.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReader.Location = New System.Drawing.Point(744, 63)
        Me.btnReader.Name = "btnReader"
        Me.btnReader.Size = New System.Drawing.Size(128, 40)
        Me.btnReader.TabIndex = 1
        Me.btnReader.Text = "�ǁ@��"
        Me.btnReader.UseVisualStyleBackColor = False
        '
        'pnlEki
        '
        Me.pnlEki.Controls.Add(Me.dtpYmdTo)
        Me.pnlEki.Controls.Add(Me.dtpHmFrom)
        Me.pnlEki.Controls.Add(Me.dtpYmdFrom)
        Me.pnlEki.Controls.Add(Me.Label1)
        Me.pnlEki.Controls.Add(Me.Label3)
        Me.pnlEki.Controls.Add(Me.Label2)
        Me.pnlEki.Controls.Add(Me.cmbEki)
        Me.pnlEki.Controls.Add(Me.labEki)
        Me.pnlEki.Location = New System.Drawing.Point(13, 6)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(646, 312)
        Me.pnlEki.TabIndex = 1
        '
        'dtpYmdTo
        '
        Me.dtpYmdTo.Location = New System.Drawing.Point(210, 233)
        Me.dtpYmdTo.Name = "dtpYmdTo"
        Me.dtpYmdTo.Size = New System.Drawing.Size(147, 18)
        Me.dtpYmdTo.TabIndex = 4
        Me.dtpYmdTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dtpHmFrom
        '
        Me.dtpHmFrom.Location = New System.Drawing.Point(479, 155)
        Me.dtpHmFrom.Name = "dtpHmFrom"
        Me.dtpHmFrom.Size = New System.Drawing.Size(145, 18)
        Me.dtpHmFrom.TabIndex = 4
        Me.dtpHmFrom.Text = "YYYY/MM/DD hh:mm:ss"
        Me.dtpHmFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dtpYmdFrom
        '
        Me.dtpYmdFrom.Location = New System.Drawing.Point(210, 156)
        Me.dtpYmdFrom.Name = "dtpYmdFrom"
        Me.dtpYmdFrom.Size = New System.Drawing.Size(150, 18)
        Me.dtpYmdFrom.TabIndex = 4
        Me.dtpYmdFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "labYmdFrom"
        Me.Label1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label1.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(60, 155)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(144, 21)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "�O��o�^�o�[�W����"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label3.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(60, 232)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(144, 21)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "����o�^�o�[�W����"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(386, 154)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(85, 21)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "�o�^�����F"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbEki
        '
        Me.cmbEki.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.cmbEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.Location = New System.Drawing.Point(196, 82)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(149, 21)
        Me.cmbEki.TabIndex = 0
        Me.cmbEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'labEki
        '
        Me.labEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.labEki.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.labEki.Location = New System.Drawing.Point(60, 82)
        Me.labEki.Name = "labEki"
        Me.labEki.Size = New System.Drawing.Size(85, 21)
        Me.labEki.TabIndex = 0
        Me.labEki.Text = "�f�[�^����"
        Me.labEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'FrmSysUnKanSetMng
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1013, 741)
        Me.Name = "FrmSysUnKanSetMng"
        Me.Text = "�^�p�[�� Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
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
    ''' CSV�f�[�^
    ''' </summary>
    Private infoLst As New List(Of String())

    ''' <summary>
    ''' ��`���
    ''' </summary>
    ''' <remarks></remarks>
    Private infoObj() As FMTInfo = Nothing

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly FormTitle As String = "�^�ǐݒ�Ǘ�"

    ''' <summary>
    ''' �f�[�^����
    ''' </summary>
    Private ReadOnly DataName As String = "�@��\���}�X�^�f�[�^"

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

    ''' <summary>
    ''' �o�^�@��\���}�X�^���s
    ''' </summary>
    Private Const LcstPrintMachineError As String = "�o�^�����Ɏ��s���܂����B�ݒ�t�@�C���̓��e���m�F���Ă��������B"

    ''' <summary>
    ''' �o�^�}�X�^�o�[�W�����i�@��j���s
    ''' </summary>
    Private Const LcstPrintVersionError As String = "�o�^�����Ɏ��s���܂����B"

    ''' <summary>
    ''' �Ǎ����s
    ''' </summary>
    Private Const LcstReaderError As String = "�Ǎ������Ɏ��s���܂����B"

    ''' <summary>
    ''' �t�@�C�����G���[
    ''' </summary>
    Private Const LcstCSVFileNameError As String = "�Ǎ��Ώۃt�@�C�����s���ł��B"

    ''' <summary>
    ''' �t�@�C���G���[
    ''' </summary>
    Private Const LcstCSVFileCheckError As String = "�Ǎ��Ώۃt�@�C�������݂��܂���B"

    ''' <summary>
    ''' ���ڐ��`�F�b�N
    ''' </summary>
    Private Const LcstItemCountCheck As String = "{0}�s�ڂ̃f�[�^���ڐ����s���ł��B"

    ''' <summary>
    ''' �K�{�`�F�b�N
    ''' </summary>
    Private Const LcstMustCheck As String = "{0}�s�ڂ̃f�[�^���ځu{1}�v���K�{�ł��B"

    ''' <summary>
    ''' �����`�F�b�N
    ''' </summary>
    Private Const LcstAttributeCheck As String = "{0}�s�ڂ̃f�[�^���ځu{1}�v�̑������s���ł��B"

    ''' <summary>
    ''' �����`�F�b�N
    ''' </summary>
    Private Const LcstTrussNumber As String = "{0}�s�ڂ̃f�[�^���ځu{1}�v�̌��������߂ł��B"

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
            '--�펞���������ڐݒ�
            btnPrint.Enabled = False
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

            LfGetInitFrm()

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
                AlertBox.Show(Lexis.FormProcAbnormalEnd)       '�J�n�ُ탁�b�Z�[�W
            End If
            LbEventStop = False '�C�x���g�����n�m
        End Try
        Return bRtn
    End Function

#End Region

#Region "�C�x���g"

    'Private Sub FrmMntDispAbnormalData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    'Handles MyBase.Load
    '�v���V�[�W�������wFrmMntDispAbnormalData_Load �� FrmSysUnKanSetMng_Load�x�ɕύX
    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    Private Sub FrmSysUnKanSetMng_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
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
            cmbEki.Text = DataName  '�f�[�^����
            LbEventStop = False             '�C�x���g�����n�m
            labEki.Select() '�����t�H�[�J�X
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
    ''' �o�^
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click
        If LbEventStop Then Exit Sub
        LbEventStop = True

        Dim dbCtl As DatabaseTalker = New DatabaseTalker()
        Dim dt As DataTable = New DataTable()
        Dim sBuilder As StringBuilder
        Dim wBuilder As StringBuilder
        Dim vBuilder As StringBuilder
        '�o�^����
        Dim sCurTime As String
        Dim loginiD As String = Config.MachineName                                      '�ݒ�t�@�C���̒[���h�c
        Dim dbError As Boolean = False                  'db�ُ픭���n�m
        Dim i As Integer = 0
        Dim j As Integer = 0
        LfWaitCursor()
        Try
            '�{�^���������O
            LogOperation(sender, e)
            dbCtl.ConnectOpen()          '�N�l�N�V�������擾����B

            dbCtl.TransactionBegin()  '�g�����U�N�V�������J�n����B
            '�o�^�����̍쐬
            sCurTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
            For i = 0 To infoLst.Count - 1

                Try
                    '�����擾�`�F�b�N
                    sBuilder = New StringBuilder
                    sBuilder.AppendLine("select count(1) ")
                    sBuilder.AppendLine(String.Format(" FROM M_MACHINE where [SETTING_START_DATE]={0}", Utility.SetSglQuot(infoLst.Item(i)(0).ToString)))
                    '-------Ver0.1�@�k���Ή��F�O���[�v�Ή��ɔ����o�^�X�V�`�F�b�N�̕ύX�@ADD START-----------
                    sBuilder.AppendLine(String.Format(" AND [BRANCH_OFFICE_CODE]={0}", Utility.SetSglQuot(infoLst.Item(i)(2).ToString)))
                    '-------Ver0.1�@�k���Ή��F�O���[�v�Ή��ɔ����o�^�X�V�`�F�b�N�̕ύX�@END START-----------
                    sBuilder.AppendLine(String.Format(" AND [RAIL_SECTION_CODE]={0}", Utility.SetSglQuot(infoLst.Item(i)(7).ToString)))
                    sBuilder.AppendLine(String.Format(" AND [STATION_ORDER_CODE] ={0}", Utility.SetSglQuot(infoLst.Item(i)(8).ToString)))
                    sBuilder.AppendLine(String.Format(" AND [CORNER_CODE]={0} ", Utility.SetSglQuot(infoLst.Item(i)(10).ToString)))
                    sBuilder.AppendLine(String.Format(" AND [MODEL_CODE]={0} ", Utility.SetSglQuot(infoLst.Item(i)(12).ToString)))
                    sBuilder.AppendLine(String.Format(" AND [UNIT_NO]={0}", infoLst.Item(i)(13)))
                    '�f�[�^�擾����
                    FrmBase.BaseSqlDataTableFill(sBuilder.ToString, dt)
                    sBuilder = New StringBuilder
                    wBuilder = New StringBuilder
                    vBuilder = New StringBuilder
                    wBuilder.AppendLine("Where 0=0")
                    vBuilder.AppendLine("values(")
                    '�@��\���}�X�^�̍X�V
                    If CInt(dt.Rows(0)(0)) > 0 Then
                        sBuilder.AppendLine(String.Format(" update M_MACHINE set UPDATE_DATE={0},", Utility.SetSglQuot(sCurTime)))
                        sBuilder.AppendLine(String.Format(" UPDATE_USER_ID={0},", Utility.SetSglQuot(GlobalVariables.UserId)))
                        sBuilder.AppendLine(String.Format(" UPDATE_MACHINE_ID={0}", Utility.SetSglQuot(loginiD)))
                        For j = 0 To infoObj.Length - 1
                            If infoObj(j).FIELD_NAME = "SETTING_START_DATE" OrElse
                               infoObj(j).FIELD_NAME = "RAIL_SECTION_CODE" OrElse
                               infoObj(j).FIELD_NAME = "STATION_ORDER_CODE" OrElse
                               infoObj(j).FIELD_NAME = "CORNER_CODE" OrElse
                               infoObj(j).FIELD_NAME = "MODEL_CODE" Then
                                wBuilder.AppendLine(String.Format("AND {0}={1}", infoObj(j).FIELD_NAME, _
                                                                  Utility.SetSglQuot(infoLst.Item(i)(j).ToString)))
                            ElseIf infoObj(j).FIELD_NAME = "UNIT_NO" Then
                                sBuilder.AppendLine(String.Format(",{0}={1}", infoObj(j).FIELD_NAME, _
                                                                 infoLst.Item(i)(j)))
                                wBuilder.AppendLine(String.Format("AND {0}={1}", infoObj(j).FIELD_NAME, _
                                                                  infoLst.Item(i)(j)))
                            Else
                                sBuilder.AppendLine(String.Format(",{0}={1}", infoObj(j).FIELD_NAME, _
                                                                  Utility.SetSglQuot(infoLst.Item(i)(j).ToString)))
                            End If
                        Next
                        sBuilder.AppendLine(wBuilder.ToString)
                    Else  '�@��\���}�X�^�̓o�^
                        sBuilder.AppendLine(" insert into M_MACHINE (INSERT_DATE ,INSERT_USER_ID,INSERT_MACHINE_ID")
                        sBuilder.AppendLine(" ,UPDATE_DATE,UPDATE_USER_ID,UPDATE_MACHINE_ID")
                        vBuilder.AppendLine(String.Format("{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot(GlobalVariables.UserId)))
                        vBuilder.AppendLine(String.Format("{0}", Utility.SetSglQuot(loginiD)))
                        vBuilder.AppendLine(String.Format(",{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot(GlobalVariables.UserId)))
                        vBuilder.AppendLine(String.Format("{0}", Utility.SetSglQuot(loginiD)))
                        For j = 0 To infoObj.Length - 1
                            If infoObj(j).FIELD_NAME = "UNIT_NO" OrElse
                               infoObj(j).FIELD_NAME = "Y_AREA_CODE" OrElse
                               infoObj(j).FIELD_NAME = "G_AREA_CODE" OrElse
                               infoObj(j).FIELD_NAME = "W_AREA_CODE" Then
                                vBuilder.AppendLine(String.Format(",{0}", infoLst.Item(i)(j).ToString))
                            Else
                                vBuilder.AppendLine(String.Format(",{0}", Utility.SetSglQuot(infoLst.Item(i)(j).ToString)))
                            End If
                            sBuilder.AppendLine(String.Format(",{0}", infoObj(j).FIELD_NAME))
                        Next
                        vBuilder.Append(")")
                        sBuilder.Append(")")
                        sBuilder.AppendLine(vBuilder.ToString)
                    End If
                    '�f�[�^����
                    dbCtl.ExecuteSQLToWrite(sBuilder.ToString)
                Catch ex As Exception
                    dbError = True
                    infoLst = Nothing
                    Log.Fatal(LcstPrintMachineError)
                    AlertBox.Show(Lexis.MachineMasterInsertFailed)
                    btnPrint.Enabled = False
                    btnReturn.Select()
                    'TODO: �g�����U�N�V�����̃��[���o�b�N��dbCtl.TransactionRollBack()�ōs��Ȃ��ƁA
                    '��O���R��o���B
                    Exit Sub
                End Try
            Next
            '�}�X�^�o�[�W�����i�@��j�uM_MACHINE_DATA_VER�v�̓o�^
            If dbError <> True Then
                sBuilder = New StringBuilder
                sBuilder.AppendLine("select count(1) ")
                sBuilder.AppendLine(String.Format(" FROM M_MACHINE_DATA_VER where [VERSION]={0}", Utility.SetSglQuot(dtpYmdTo.Text)))
                '�f�[�^�擾����
                FrmBase.BaseSqlDataTableFill(sBuilder.ToString, dt)
                If CInt(dt.Rows(0)(0)) > 0 Then
                    sBuilder = New StringBuilder
                    sBuilder.AppendLine(String.Format("UPDATE M_MACHINE_DATA_VER SET UPDATE_DATE={0},", Utility.SetSglQuot(sCurTime)))
                    sBuilder.AppendLine(String.Format(" UPDATE_USER_ID={0},", Utility.SetSglQuot(GlobalVariables.UserId)))
                    sBuilder.AppendLine(String.Format(" UPDATE_MACHINE_ID={0}", Utility.SetSglQuot(loginiD)))
                    sBuilder.AppendLine(String.Format("WHERE VERSION = {0}", Utility.SetSglQuot(dtpYmdTo.Text)))
                Else
                    sBuilder = New StringBuilder
                    sBuilder.AppendLine("insert into M_MACHINE_DATA_VER(INSERT_DATE,INSERT_USER_ID,INSERT_MACHINE_ID")
                    sBuilder.AppendLine(" ,UPDATE_DATE,UPDATE_USER_ID,UPDATE_MACHINE_ID")
                    sBuilder.AppendLine(", VERSION)")
                    sBuilder.AppendLine(String.Format("values({0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot((GlobalVariables.UserId))))
                    sBuilder.AppendLine(String.Format("{0},", Utility.SetSglQuot(loginiD)))
                    sBuilder.AppendLine(String.Format("{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot((GlobalVariables.UserId))))
                    sBuilder.AppendLine(String.Format("{0},", Utility.SetSglQuot(loginiD)))
                    sBuilder.AppendLine(String.Format("{0})", Utility.SetSglQuot(dtpYmdTo.Text)))
                End If
                '�X�V�A�}���̂��߂�SQL�����s����B
                dbCtl.ExecuteSQLToWrite(sBuilder.ToString)
            End If
            '�g�����U�N�V�������R�~�b�g����
            dbCtl.TransactionCommit()
            AlertBox.Show(Lexis.InsertCompleted)
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            dbError = True
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnReturn.Select()
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            dbError = True
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.MachineMasterInsertFailed2)
            btnReturn.Select()
        Finally
            dbCtl.ConnectClose()
            infoLst = Nothing
            LbEventStop = False
            dbError = False
            btnPrint.Enabled = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' �Ǎ�
    ''' </summary>
    Private Sub btnReader_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReader.Click
        If LbEventStop Then Exit Sub
        LbEventStop = True      '�C�x���g�����n�e�e
        Dim fName As String = "DATA_MachineConfig_XXX.csv"
        Dim filePath As String = ""
        Dim fileNo As String
        Dim oldFPath As String
        Dim newFPath As String
        infoLst = New List(Of String())
        Dim filenumber As Int32
        Dim strRead() As String
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim iFlag As Integer = 0

        LfWaitCursor()
        Try
            '�{�^�������f
            LogOperation(sender, e)
            OpenFileDialog1.Multiselect = False
            '�t�@�C����I��
            If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                '�Ǎ��Ώۃt�@�C�����`�F�b�N
                oldFPath = OpenFileDialog1.FileName
                fileNo = oldFPath.Substring(oldFPath.LastIndexOf("_") + 1, 3)
                fName = fName.Replace("XXX", fileNo)
                filePath = oldFPath.Substring(0, oldFPath.LastIndexOf("\") + 1)
                newFPath = Path.Combine(filePath, fName)

                If oldFPath <> newFPath Then
                    Log.Error(LcstCSVFileNameError)
                    AlertBox.Show(Lexis.TheFileNameIsUnsuitableForMachineMaster)
                    btnReturn.Select()
                    Exit Sub
                End If
                ' �Ǎ��Ώۃt�@�C���`�F�b�N
                If File.Exists(newFPath) = False Then
                    Log.Error(LcstCSVFileCheckError)
                    AlertBox.Show(Lexis.MachineMasterFileNotFound)
                    btnReturn.Select()
                    Exit Sub
                End If
                dtpYmdTo.Text = fileNo
            Else
                Exit Sub
            End If
            'CSV�t�H�[�}�b�g��`�����擾����
            If GetDefineInfo(Config.MachineMasterFormatFilePath, "FMT_MachineConfig", infoObj) = False Then
                btnReturn.Select()
                Exit Sub
            End If
            'CSV�t�@�C�����A�f�[�^���擾����B
            filenumber = CShort(FreeFile())
            FileOpen(filenumber, filePath + fName, OpenMode.Binary)
            Do While Not EOF(1)

                Dim str As String = ""","""
                strRead = Nothing
                strRead = Split(LineInput(1), str)
                If strRead(0).ToString = "[FMT_MachineConfig]" Then
                    Continue Do
                End If
                i += 1
                If strRead.Length <> infoObj.Length Then
                    Log.Info(String.Format(LcstItemCountCheck, i))
                    iFlag = 1
                    Exit Do
                End If
                strRead(0) = strRead(0).Remove(0, 1)
                strRead(22) = strRead(22).Remove(strRead(22).Length - 1, 1)

                '�f�[�^�`�F�b�N
                For j = 0 To infoObj.Length - 1
                    If LfCheck(strRead(j), i, infoObj(j)) = False Then
                        iFlag = 1
                        Exit Do
                    End If
                Next
                infoLst.Add(strRead)
            Loop
            FileClose(1)
            '�{�^��������
            If infoLst.Count > 0 And iFlag <> 1 Then
                btnPrint.Enabled = True
            Else
                AlertBox.Show(Lexis.MachineMasterFileReadFailed) '�`�F�b�N�G���[�����������ꍇ
                btnPrint.Enabled = False
            End If
            LfGetInitFrm()
        Catch ex As Exception
            FileClose(1)
            infoLst = Nothing
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.MachineMasterFileReadFailed) '�������s���b�Z�[�W
            btnReturn.Select()
        Finally
            btnPrint.Select()
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

#End Region
#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    ''' �O��o�^
    ''' </summary>
    Private Sub LfGetInitFrm()
        Dim sSql As String = ""
        Dim dtData As New DataTable
        Dim sBuilder As New StringBuilder
        Try
            sBuilder.AppendLine("SELECT top(1) VERSION ,")
            sBuilder.AppendLine("CONVERT(VARCHAR(100), INSERT_DATE, 111)+ ' '+CONVERT(VARCHAR(100), INSERT_DATE, 24) AS INSERT_DATE")
            sBuilder.AppendLine(" FROM M_MACHINE_DATA_VER order by UPDATE_DATE DESC ")
            sSql = sBuilder.ToString()
            BaseSqlDataTableFill(sSql, dtData)
            If dtData.Rows.Count > 0 Then
                If dtData.Rows(0).Item(0).ToString <> Nothing Then
                    dtpYmdFrom.Text = dtData.Rows(0).Item(0).ToString   '�O��o�^�o�[�W����
                End If
                If dtData.Rows(0).Item(1).ToString <> Nothing Then
                    dtpHmFrom.Text = dtData.Rows(0).Item(1).ToString()
                End If
            Else
                dtpYmdFrom.Text = ""   '�O��o�^�o�[�W����
                dtpHmFrom.Text = ""    '�o�^����
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

    ''' <summary>
    ''' [�����{�^��������]
    ''' </summary>
    Private Sub LfSearchTrue()
        Dim bEnabled As Boolean
        If bEnabled Then
        End If
        If bEnabled Then
            If btnReader.Enabled = False Then btnReader.Enabled = True
        Else
            If btnReader.Enabled = True Then btnReader.Enabled = False
        End If
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
                AlertBox.Show(Lexis.MachineMasterFormatFileNotFound)
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
    ''' �f�[�^�`�F�b�N
    ''' </summary>
    ''' <param name="CodeName">�t�B�[���h��</param>
    ''' <param name="iRow">����</param>
    ''' <param name="AarrayCode">��`���</param>
    Private Function LfCheck(ByRef CodeName As String, ByVal iRow As Integer, ByVal AarrayCode As FMTInfo) As Boolean
        Dim Encode As Encoding = Encoding.GetEncoding("Shift_JIS")
        If AarrayCode.MUST = True Then      '�K�{�`�F�b�N
            If CodeName.Length = 0 Then
                Log.Info(String.Format(LcstMustCheck, iRow, AarrayCode.KOMOKU_NAME))
                Return False
            ElseIf AarrayCode.FIELD_FORMAT = "Integer" Then '�t�B�[���h�`��:Integer
                If OPMGUtility.checkNumber(CodeName) = False Then         '�t�B�[���h�`���`�F�b�N
                    Log.Info(String.Format(LcstAttributeCheck, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                ElseIf CDec(CodeName) > Integer.MaxValue Then      '�t�B�[���h�����`�F�b�N
                    Log.Info(String.Format(LcstTrussNumber, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                End If
            ElseIf AarrayCode.FIELD_FORMAT = "String" Then  '�t�B�[���h�`��:String
                If Encode.GetByteCount(CodeName) > AarrayCode.DATA_LEN Then '�t�B�[���h�����`�F�b�N
                    Log.Info(String.Format(LcstTrussNumber, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                End If
            End If
        Else
            If AarrayCode.FIELD_FORMAT = "String" Then        '�t�B�[���h�`��:String
                If Encode.GetByteCount(CodeName) > AarrayCode.DATA_LEN Then '�t�B�[���h�����`�F�b�N
                    Log.Info(String.Format(LcstTrussNumber, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                End If
            ElseIf AarrayCode.FIELD_FORMAT = "Integer" Then     '�t�B�[���h�`��:Integer
                If OPMGUtility.checkNumber(CodeName) = False Then             '�t�B�[���h�`���`�F�b�N
                    Log.Info(String.Format(LcstAttributeCheck, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                ElseIf CDec(CodeName) > Integer.MaxValue Then              '�t�B�[���h�����`�F�b�N
                    Log.Info(String.Format(LcstTrussNumber, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                End If
            End If
        End If
        Return True
    End Function

#End Region
End Class
Public Structure FMTStructure
#Region "�錾�̈�iPublic�j"
    Public Structure FMTInfo
        Dim KOMOKU_NAME As String                   '���ږ���
        Dim IN_TURN As Integer                      '����
        Dim MUST As Boolean                         '�K�{
        Dim FIELD_FORMAT As String                  '�t�B�[���h�`��
        Dim DATA_LEN As Integer                      '�f�[�^��
        Dim FIELD_NAME As String                    '�t�B�[���h��
    End Structure
#End Region
End Structure