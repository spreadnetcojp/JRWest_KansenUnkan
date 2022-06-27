' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/04/01  �@�@ ����  �k���Ή�(�ُ�ڍ׍��ڊg��)
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '�萔�l�̂ݎg�p
Imports GrapeCity.Win
Imports System.IO

''' <summary>
''' �y�ُ�f�[�^�ڍׁ@��ʃN���X�z
''' </summary>
Public Class FrmMntDispFaultDataDetail
    Inherits System.Windows.Forms.Form

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()
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
    Public WithEvents pnlBodyBase As System.Windows.Forms.Panel
    Friend WithEvents LblEki As System.Windows.Forms.Label
    Friend WithEvents LblMado As System.Windows.Forms.Label
    Friend WithEvents LblKisyu As System.Windows.Forms.Label
    Friend WithEvents LblGouki As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents LblErrName As System.Windows.Forms.Label
    Friend WithEvents LblErrDetail As System.Windows.Forms.Label
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents LblDateTime As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents LblErrTreatment As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.pnlBodyBase = New System.Windows.Forms.Panel()
        Me.LblErrTreatment = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.LblErrDetail = New System.Windows.Forms.Label()
        Me.LblErrName = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.LblDateTime = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LblGouki = New System.Windows.Forms.Label()
        Me.LblKisyu = New System.Windows.Forms.Label()
        Me.LblMado = New System.Windows.Forms.Label()
        Me.LblEki = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlBodyBase.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlBodyBase.Controls.Add(Me.LblErrTreatment)
        Me.pnlBodyBase.Controls.Add(Me.Label8)
        Me.pnlBodyBase.Controls.Add(Me.Label7)
        Me.pnlBodyBase.Controls.Add(Me.Label6)
        Me.pnlBodyBase.Controls.Add(Me.Label5)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.LblErrDetail)
        Me.pnlBodyBase.Controls.Add(Me.LblErrName)
        Me.pnlBodyBase.Controls.Add(Me.Label4)
        Me.pnlBodyBase.Controls.Add(Me.Label3)
        Me.pnlBodyBase.Controls.Add(Me.LblDateTime)
        Me.pnlBodyBase.Controls.Add(Me.Label2)
        Me.pnlBodyBase.Controls.Add(Me.Label1)
        Me.pnlBodyBase.Controls.Add(Me.LblGouki)
        Me.pnlBodyBase.Controls.Add(Me.LblKisyu)
        Me.pnlBodyBase.Controls.Add(Me.LblMado)
        Me.pnlBodyBase.Controls.Add(Me.LblEki)
        Me.pnlBodyBase.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlBodyBase.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 0)
        Me.pnlBodyBase.Name = "pnlBodyBase"
        Me.pnlBodyBase.Size = New System.Drawing.Size(578, 379)
        Me.pnlBodyBase.TabIndex = 0
        '
        'LblErrTreatment
        '
        Me.LblErrTreatment.Location = New System.Drawing.Point(103, 275)
        Me.LblErrTreatment.Name = "LblErrTreatment"
        Me.LblErrTreatment.Size = New System.Drawing.Size(451, 47)
        Me.LblErrTreatment.TabIndex = 15
        Me.LblErrTreatment.Text = "�|�|�|�|�{�|�|�|�|�P�|�|�|�|�{�|�|�|�|�Q�|�|�|�|�{�|�|�|�|�R" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "�|�|�|�|�{�|�|�|�|�S�|�|�|�|�{�|�|�|�|�T"
        '
        'Label8
        '
        Me.Label8.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(5, 275)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(92, 18)
        Me.Label8.TabIndex = 14
        Me.Label8.Text = "���u���e�F"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(256, 16)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(98, 18)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "�R�[�i�[�F"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(5, 16)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(92, 18)
        Me.Label6.TabIndex = 12
        Me.Label6.Text = "�w�@���F"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(5, 44)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(92, 18)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "�@�@��F"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnPrint
        '
        Me.btnPrint.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("�l�r �S�V�b�N", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(344, 329)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(100, 36)
        Me.btnPrint.TabIndex = 1
        Me.btnPrint.Text = "�o�@��"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(460, 329)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(100, 36)
        Me.btnReturn.TabIndex = 2
        Me.btnReturn.Text = "����"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'LblErrDetail
        '
        Me.LblErrDetail.Location = New System.Drawing.Point(103, 146)
        Me.LblErrDetail.Name = "LblErrDetail"
        Me.LblErrDetail.Size = New System.Drawing.Size(451, 117)
        Me.LblErrDetail.TabIndex = 7
        Me.LblErrDetail.Text = "�|�|�|�|�{�|�|�|�|�P�|�|�|�|�{�|�|�|�|�Q�|�|�|�|�{�|�|�|�|�R" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "�|�|�|�|�{�|�|�|�|�S�|�|�|�|�{�|�|�|�|�T�|�|�|�|�{�|�|�|�|�U" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "�|�|�|�|�{�|�|�|�|�V�|�|�|�|�{�|�|" & _
    "�|�|�W�|�|�|�|�{�|�|�|�|�X" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "�|�|�|�|�{�|�|�|�|�O"
        '
        'LblErrName
        '
        Me.LblErrName.Location = New System.Drawing.Point(103, 115)
        Me.LblErrName.Name = "LblErrName"
        Me.LblErrName.Size = New System.Drawing.Size(388, 18)
        Me.LblErrName.TabIndex = 5
        Me.LblErrName.Text = "�P�Q�R�S�T�U�V�W�X�O�P�Q�R�S�T�U�V�W�X�O(XXXXXX)"
        Me.LblErrName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(5, 146)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(92, 18)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "�ڍד��e�F"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(5, 115)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(92, 18)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "�ُ퍀�ځF"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblDateTime
        '
        Me.LblDateTime.Location = New System.Drawing.Point(103, 87)
        Me.LblDateTime.Name = "LblDateTime"
        Me.LblDateTime.Size = New System.Drawing.Size(156, 18)
        Me.LblDateTime.TabIndex = 4
        Me.LblDateTime.Text = "YYYY/MM/DD hh:mm:ss"
        Me.LblDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(5, 87)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(92, 18)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "���������F"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(256, 44)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(98, 18)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "���@�F"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblGouki
        '
        Me.LblGouki.Location = New System.Drawing.Point(360, 44)
        Me.LblGouki.Name = "LblGouki"
        Me.LblGouki.Size = New System.Drawing.Size(84, 18)
        Me.LblGouki.TabIndex = 3
        Me.LblGouki.Text = "99"
        Me.LblGouki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblKisyu
        '
        Me.LblKisyu.Location = New System.Drawing.Point(103, 44)
        Me.LblKisyu.Name = "LblKisyu"
        Me.LblKisyu.Size = New System.Drawing.Size(125, 18)
        Me.LblKisyu.TabIndex = 2
        Me.LblKisyu.Text = "����������"
        Me.LblKisyu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblMado
        '
        Me.LblMado.Location = New System.Drawing.Point(360, 16)
        Me.LblMado.Name = "LblMado"
        Me.LblMado.Size = New System.Drawing.Size(175, 18)
        Me.LblMado.TabIndex = 1
        Me.LblMado.Text = "��������������������"
        Me.LblMado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblEki
        '
        Me.LblEki.Location = New System.Drawing.Point(103, 16)
        Me.LblEki.Name = "LblEki"
        Me.LblEki.Size = New System.Drawing.Size(125, 18)
        Me.LblEki.TabIndex = 0
        Me.LblEki.Text = "����������"
        Me.LblEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmMntDispFaultDataDetail
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(578, 379)
        Me.Controls.Add(Me.pnlBodyBase)
        Me.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmMntDispFaultDataDetail"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "�ڍ׈ُ�\��"
        Me.pnlBodyBase.ResumeLayout(False)
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
    ''' �o�͗p�e���v���[�g�t�@�C����
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "�ُ�ڍ׃f�[�^.xls"

    ''' <summary>
    ''' �o�͎��p�e���v���[�g�V�[�g��
    ''' </summary>�f�[�^
    Private ReadOnly LcstXlsSheetName As String = "�ُ�ڍ׃f�[�^"

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
        Try
            Log.Info("Method started.")

            '--�펞���������ڐݒ�
            btnPrint.Enabled = True         '�o�̓{�^��
            btnReturn.Enabled = True        '�I���{�^��
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
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [��ʏ�������]
    ''' ��ʍ��ڂ�\�����܂��B
    ''' </summary>
    Public Sub setContent(ByVal eki As String, ByVal mado As String, ByVal kisyu As String, _
                          ByVal gouki As String, ByVal dateTime As String, ByVal errName As String, _
                          ByVal errDetail As String, ByVal errTreatment As String)
        LblEki.Text = eki
        LblMado.Text = mado
        LblKisyu.Text = kisyu
        LblGouki.Text = gouki
        LblDateTime.Text = dateTime
        LblErrName.Text = errName
        LblErrDetail.Text = errDetail
        LblErrTreatment.Text = errTreatment
    End Sub

#End Region

#Region "�C�x���g"

    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    Private Sub FrmMntDispAbnormalDetail_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        FrmBase.LfWaitCursor()
        Try
            '�����������Ăяo����Ă��Ȃ��ꍇ�I��
            If LbInitCallFlg = False Then
                Me.Close()
                Exit Sub
            End If
            '��ʔw�i�F�iBackColor�j��ݒ肷��
            pnlBodyBase.BackColor = Config.BackgroundColor
            '------------------------------
            Label1.BackColor = Config.BackgroundColor
            Label2.BackColor = Config.BackgroundColor
            Label3.BackColor = Config.BackgroundColor
            Label4.BackColor = Config.BackgroundColor
            Label5.BackColor = Config.BackgroundColor
            Label6.BackColor = Config.BackgroundColor
            Label7.BackColor = Config.BackgroundColor
            Label8.BackColor = Config.BackgroundColor
            '------------------------------
            LblEki.BackColor = Config.BackgroundColor
            LblMado.BackColor = Config.BackgroundColor
            LblKisyu.BackColor = Config.BackgroundColor
            LblGouki.BackColor = Config.BackgroundColor
            LblDateTime.BackColor = Config.BackgroundColor
            LblErrName.BackColor = Config.BackgroundColor
            LblErrDetail.BackColor = Config.BackgroundColor
            LblErrTreatment.BackColor = Config.BackgroundColor
            '-----------------------
            '�{�^���w�i�F�iBackColor�j��ݒ肷��
            btnPrint.BackColor = Config.ButtonColor
            btnReturn.BackColor = Config.ButtonColor
            '�����t�H�[�J�X
            btnPrint.Select()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
        End Try
    End Sub

    '//////////////////////////////////////////////�{�^���N���b�N

    ''' <summary>
    ''' ����
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnReturn.Click
        FrmBase.LogOperation(sender, e, Me.Text)    '�{�^���������O
        Me.Close()
    End Sub

    ''' <summary>
    ''' �o��
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click
        Me.Cursor = Cursors.WaitCursor
        Try
            FrmBase.LogOperation(sender, e, Me.Text)    '�{�^���������O

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
            btnReturn.Select()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            '�G���[���b�Z�[�W
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

#End Region

#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    ''' [�o�͏���]
    ''' </summary>
    ''' <param name="sPath">�t�@�C���t���p�X</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Try
            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                .FileName = sPath
                .Report.Start()
                .Report.File()
                .Page.Start(LcstXlsSheetName, "1-9999")
                .Cell("B1").Value = LcstXlsSheetName
                .Cell("AD1").Value = OPMGFormConstants.OUT_TERMINAL + FrmBase.GetLedgerTitle()
                .Cell("AD2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + LblEki.Text.Trim + _
                                    StrConv(Space(2), VbStrConv.Wide)
                .Cell("H3").Value = OPMGFormConstants.CORNER_STR + LblMado.Text.Trim + _
                                    StrConv(Space(2), VbStrConv.Wide)
                .Cell("R3").Value = OPMGFormConstants.EQUIPMENT_TYPE + LblKisyu.Text.Trim + _
                                    StrConv(Space(2), VbStrConv.Wide)
                .Cell("Y3").Value = OPMGFormConstants.NUM_EQUIPMENT + LblGouki.Text.Trim
                .Cell("F5").Value = LblDateTime.Text.Trim
                Dim errName As String = LblErrName.Text.Trim
                '-------Ver0.1�@�k���Ή�(�ُ�ڍ׍��ڊg��)�@MOD START-----------
                .Cell("F7").Value = LblErrName.Text
                .Cell("F9").Value = LblErrDetail.Text
                .Cell("F19").Value = LblErrTreatment.Text
                '-------Ver0.1�@�k���Ή�(�ُ�ڍ׍��ڊg��)�@MOD END-----------
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
