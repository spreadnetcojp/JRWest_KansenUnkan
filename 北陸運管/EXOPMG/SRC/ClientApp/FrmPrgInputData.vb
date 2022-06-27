' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports System.IO

''' <summary>
''' �O���}�̎捞�i�v���O�����j
''' </summary>
''' <remarks>���j���[�Ǘ����j���[���u�O���}�̎捞�i�v���O�����j�v�{�^�����N���b�N����ƁA�{��ʂ�\������B
''' �{��ʂɂăv���O�����f�[�^�̓Ǎ��݁A�o�^���s���B</remarks>
Public Class FrmPrgInputData
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
    Friend WithEvents dlgOpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents lblSaveDT As System.Windows.Forms.Label
    Friend WithEvents lblSave As System.Windows.Forms.Label
    Friend WithEvents lblAfterVer As System.Windows.Forms.Label
    Friend WithEvents lblBeforeVer As System.Windows.Forms.Label
    Friend WithEvents lblPrgName As System.Windows.Forms.Label
    Friend WithEvents lblAppliedArea As System.Windows.Forms.Label
    Friend WithEvents lblAfter As System.Windows.Forms.Label
    Friend WithEvents lblBefore As System.Windows.Forms.Label
    Friend WithEvents lblPrgNa As System.Windows.Forms.Label
    Friend WithEvents lblPrm As System.Windows.Forms.Label
    Friend WithEvents btnOpenFile As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents lblModelName As System.Windows.Forms.Label
    Friend WithEvents lblKisyu As System.Windows.Forms.Label
    Friend WithEvents btnSaveData As System.Windows.Forms.Button
    Friend WithEvents lblAcceptDate As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.dlgOpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.lblSaveDT = New System.Windows.Forms.Label()
        Me.lblSave = New System.Windows.Forms.Label()
        Me.lblAfterVer = New System.Windows.Forms.Label()
        Me.lblBeforeVer = New System.Windows.Forms.Label()
        Me.lblPrgName = New System.Windows.Forms.Label()
        Me.lblAppliedArea = New System.Windows.Forms.Label()
        Me.lblAfter = New System.Windows.Forms.Label()
        Me.lblBefore = New System.Windows.Forms.Label()
        Me.lblPrgNa = New System.Windows.Forms.Label()
        Me.lblPrm = New System.Windows.Forms.Label()
        Me.btnOpenFile = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnSaveData = New System.Windows.Forms.Button()
        Me.lblModelName = New System.Windows.Forms.Label()
        Me.lblKisyu = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblAcceptDate = New System.Windows.Forms.Label()
        Me.pnlBodyBase.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.lblAcceptDate)
        Me.pnlBodyBase.Controls.Add(Me.Label1)
        Me.pnlBodyBase.Controls.Add(Me.lblModelName)
        Me.pnlBodyBase.Controls.Add(Me.lblKisyu)
        Me.pnlBodyBase.Controls.Add(Me.lblSaveDT)
        Me.pnlBodyBase.Controls.Add(Me.lblSave)
        Me.pnlBodyBase.Controls.Add(Me.lblAfterVer)
        Me.pnlBodyBase.Controls.Add(Me.lblBeforeVer)
        Me.pnlBodyBase.Controls.Add(Me.lblPrgName)
        Me.pnlBodyBase.Controls.Add(Me.lblAppliedArea)
        Me.pnlBodyBase.Controls.Add(Me.lblAfter)
        Me.pnlBodyBase.Controls.Add(Me.lblBefore)
        Me.pnlBodyBase.Controls.Add(Me.lblPrgNa)
        Me.pnlBodyBase.Controls.Add(Me.lblPrm)
        Me.pnlBodyBase.Controls.Add(Me.btnOpenFile)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnSaveData)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/07/31(��)  14:36"
        '
        'dlgOpenFileDialog
        '
        Me.dlgOpenFileDialog.ReadOnlyChecked = True
        '
        'lblSaveDT
        '
        Me.lblSaveDT.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSaveDT.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSaveDT.Location = New System.Drawing.Point(527, 344)
        Me.lblSaveDT.Name = "lblSaveDT"
        Me.lblSaveDT.Size = New System.Drawing.Size(168, 16)
        Me.lblSaveDT.TabIndex = 67
        Me.lblSaveDT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSave
        '
        Me.lblSave.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSave.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSave.Location = New System.Drawing.Point(439, 344)
        Me.lblSave.Name = "lblSave"
        Me.lblSave.Size = New System.Drawing.Size(88, 16)
        Me.lblSave.TabIndex = 66
        Me.lblSave.Text = "�o�^����"
        '
        'lblAfterVer
        '
        Me.lblAfterVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfterVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfterVer.Location = New System.Drawing.Point(319, 392)
        Me.lblAfterVer.Name = "lblAfterVer"
        Me.lblAfterVer.Size = New System.Drawing.Size(91, 16)
        Me.lblAfterVer.TabIndex = 65
        Me.lblAfterVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblBeforeVer
        '
        Me.lblBeforeVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBeforeVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBeforeVer.Location = New System.Drawing.Point(319, 344)
        Me.lblBeforeVer.Name = "lblBeforeVer"
        Me.lblBeforeVer.Size = New System.Drawing.Size(91, 16)
        Me.lblBeforeVer.TabIndex = 63
        Me.lblBeforeVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPrgName
        '
        Me.lblPrgName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrgName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrgName.Location = New System.Drawing.Point(319, 280)
        Me.lblPrgName.Name = "lblPrgName"
        Me.lblPrgName.Size = New System.Drawing.Size(262, 18)
        Me.lblPrgName.TabIndex = 61
        Me.lblPrgName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAppliedArea
        '
        Me.lblAppliedArea.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAppliedArea.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAppliedArea.Location = New System.Drawing.Point(319, 232)
        Me.lblAppliedArea.Name = "lblAppliedArea"
        Me.lblAppliedArea.Size = New System.Drawing.Size(179, 18)
        Me.lblAppliedArea.TabIndex = 60
        Me.lblAppliedArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAfter
        '
        Me.lblAfter.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfter.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfter.Location = New System.Drawing.Point(135, 392)
        Me.lblAfter.Name = "lblAfter"
        Me.lblAfter.Size = New System.Drawing.Size(160, 16)
        Me.lblAfter.TabIndex = 59
        Me.lblAfter.Text = "����o�^�o�[�W����"
        '
        'lblBefore
        '
        Me.lblBefore.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBefore.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBefore.Location = New System.Drawing.Point(135, 344)
        Me.lblBefore.Name = "lblBefore"
        Me.lblBefore.Size = New System.Drawing.Size(160, 16)
        Me.lblBefore.TabIndex = 58
        Me.lblBefore.Text = "�O��o�^�o�[�W����"
        '
        'lblPrgNa
        '
        Me.lblPrgNa.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrgNa.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrgNa.Location = New System.Drawing.Point(135, 280)
        Me.lblPrgNa.Name = "lblPrgNa"
        Me.lblPrgNa.Size = New System.Drawing.Size(160, 18)
        Me.lblPrgNa.TabIndex = 57
        Me.lblPrgNa.Text = "�v���O��������"
        '
        'lblPrm
        '
        Me.lblPrm.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrm.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrm.Location = New System.Drawing.Point(135, 232)
        Me.lblPrm.Name = "lblPrm"
        Me.lblPrm.Size = New System.Drawing.Size(160, 18)
        Me.lblPrm.TabIndex = 56
        Me.lblPrm.Text = "�K�p�G���A����"
        '
        'btnOpenFile
        '
        Me.btnOpenFile.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnOpenFile.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnOpenFile.Location = New System.Drawing.Point(749, 278)
        Me.btnOpenFile.Name = "btnOpenFile"
        Me.btnOpenFile.Size = New System.Drawing.Size(128, 40)
        Me.btnOpenFile.TabIndex = 0
        Me.btnOpenFile.Text = "�ǁ@��"
        Me.btnOpenFile.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(749, 390)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 3
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnSaveData
        '
        Me.btnSaveData.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnSaveData.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSaveData.Location = New System.Drawing.Point(749, 334)
        Me.btnSaveData.Name = "btnSaveData"
        Me.btnSaveData.Size = New System.Drawing.Size(128, 40)
        Me.btnSaveData.TabIndex = 1
        Me.btnSaveData.Text = "�o�@�^"
        Me.btnSaveData.UseVisualStyleBackColor = False
        '
        'lblModelName
        '
        Me.lblModelName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModelName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModelName.Location = New System.Drawing.Point(319, 181)
        Me.lblModelName.Name = "lblModelName"
        Me.lblModelName.Size = New System.Drawing.Size(106, 18)
        Me.lblModelName.TabIndex = 69
        Me.lblModelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblKisyu
        '
        Me.lblKisyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblKisyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKisyu.Location = New System.Drawing.Point(135, 181)
        Me.lblKisyu.Name = "lblKisyu"
        Me.lblKisyu.Size = New System.Drawing.Size(160, 18)
        Me.lblKisyu.TabIndex = 68
        Me.lblKisyu.Text = "�@��"
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label1.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(439, 392)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 16)
        Me.Label1.TabIndex = 70
        Me.Label1.Text = "���싖��"
        '
        'lblAcceptDate
        '
        Me.lblAcceptDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAcceptDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAcceptDate.Location = New System.Drawing.Point(527, 392)
        Me.lblAcceptDate.Name = "lblAcceptDate"
        Me.lblAcceptDate.Size = New System.Drawing.Size(168, 16)
        Me.lblAcceptDate.TabIndex = 71
        Me.lblAcceptDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmPrgInputData
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1018, 736)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "FrmPrgInputData"
        Me.Text = "�^�p�[�� "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "�錾�̈�iPrivate�j"

    Private sPathWithName As String = ""        '�t���p�X�t�@�C����
    Private sFileName As String = ""            '�t�@�C����
    Private sModName As String = ""             '�@�햼
    Private sAreaName As String = ""            '�G���A����
    Private sPrmName As String = ""             '�v���O��������

    Private sBeforVer As String = ""            '�O��o�^�o�[�W����
    Private sAfterVer As String = ""            '����o�^�o�[�W����
    Private sUpDate As String = ""              '�o�^����
    Private sExeDate As String = ""             '���싖��

    Private sMdlKind As String = ""             '�@��R�[�h
    Private sAreaKind As String = ""            '�G���A�R�[�h
    Private sPrmKind As String = ""             '�v���O�������

    Private bSaved As Boolean = False           '�o�^����

    Private ReadOnly LcstFormTitle As String = "�O���}�̎捞�i�v���O�����j"

#End Region

#Region "�C�x���g"

    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FrmPrgInputData_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Log.Info("Method started.")

        '��ʃ^�C�g���A��ʔw�i�F�iBackColor�j��ݒ肷��
        lblTitle.Text = LcstFormTitle

        '���x�������
        Call setLbl(False)
        '�o�^�{�^���񊈐���
        Me.btnSaveData.Enabled = False

        Log.Info("Method ended.")
    End Sub

    ''' <summary>
    ''' �u�Ǎ��v�{�^���N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�u�Ǎ��v�{�^�����N���b�N���邱�Ƃɂ��O���}�̂���v���O�����f�[�^��Ǎ��݁A
    ''' �u�@�햼�́v�u�v���O�������́v�u�K�p�G���A�v
    ''' �u�O��o�^�o�[�W�����v�u�o�^�����v�u����o�^�o�[�W�����v��\������B</remarks>
    Private Sub btnOpenFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenFile.Click

        LogOperation(sender, e)    '�{�^���������O

        '�u�t�@�C��Open�_�C�A���O�v��\������B
        dlgOpenFileDialog.FileName = ""
        dlgOpenFileDialog.ShowDialog()

        'OpenFileDialog�ɂăt�@�C����I�����Ȃ��ꍇ�A��������{���Ȃ��B
        If dlgOpenFileDialog.FileName = "" Then
            Exit Sub
        End If

        Call waitCursor(True)

        sPathWithName = dlgOpenFileDialog.FileName
        sFileName = ""            '�t�@�C����
        sModName = ""             '�@�햼��
        sAreaName = ""            '�G���A����
        sPrmName = ""             '�v���O��������

        sBeforVer = ""            '�O��o�^�o�[�W����
        sUpDate = ""              '�o�^����
        sAfterVer = ""            '����o�^�o�[�W����
        sExeDate = ""             '���싖��

        sMdlKind = ""             '�@��R�[�h
        sAreaKind = ""            '�G���A�R�[�h
        sPrmKind = ""             '�v���O�������
        bSaved = False            '�o�^����

        '���x�������
        Call setLbl(False)

        '�u�o�^�v�{�^���F�񊈐���
        Me.btnSaveData.Enabled = False

        '�u�t�@�C�����v���e�R�[�h���擾����
        If getDataFromFName(sPathWithName) = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        If getExeDate(sPathWithName) = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        '�G���A���̂��擾����
        If getAreaFromDb() = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        '�e�R�[�h���疼�̂��擾����
        If checkKindFromDb() = False Then
            Call waitCursor(False)
            Exit Sub
        End If



        '�O��o�^�o�[�W�����Ɠo�^�������擾����
        If getDataFromDb() = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        '�擾������ʂɃZ�b�g
        Call showLable()

        '���x��������
        Call setLbl(True)

        '�u�o�^�v�{�^��������
        Me.btnSaveData.Enabled = True

        Call waitCursor(False)

    End Sub

    ''' <summary>
    ''' �u�o�^�v�{�^���N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�u�o�^�v�{�^�����N���b�N���邱�Ƃɂ��A
    ''' �O���}�̂��Ǎ��񂾃o�[�W�����̃v���O�����f�[�^���^�p�Ǘ��T�[�o�ɓo�^����B</remarks>
    Private Sub btnSaveData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveData.Click
        Try
            LogOperation(sender, e)    '�{�^���������O

            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUllMasProFile) = DialogResult.No Then
                LogOperation(Lexis.NoButtonClicked)     'No�{�^���������O
                Exit Sub
            End If

            LogOperation(Lexis.YesButtonClicked)     'Yes�{�^���������O

            Call waitCursor(True)

            Me.bSaved = False

            If OpClientUtil.Connect() = False Then
                AlertBox.Show(Lexis.ConnectFailed)
                Exit Sub
            End If

            Dim ullResult As MasProUllResult = OpClientUtil.UllMasProFile(sPathWithName)

            OpClientUtil.Disconnect()

            Select Case ullResult
                Case MasProUllResult.Completed
                    Log.Info("MasProUllResponse with MasProUllResult.Completed received.")
                    AlertBox.Show(Lexis.UllMasProFileCompleted)
                Case MasProUllResult.Failed
                    Log.Info("MasProUllResponse with MasProUllResult.Failed received.")
                    AlertBox.Show(Lexis.UllMasProFileFailed)
                    Exit Sub
                Case MasProUllResult.FailedByBusy
                    Log.Info("MasProUllResponse with MasProUllResult.FailedByBusy received.")
                    AlertBox.Show(Lexis.UllMasProFileFailedByBusy)
                    Exit Sub
                Case MasProUllResult.FailedByInvalidContent
                    Log.Info("MasProUllResponse with MasProUllResult.FailedByInvalidContent received.")
                    AlertBox.Show(Lexis.UllMasProFileFailedByInvalidContent)
                    Exit Sub
                Case MasProUllResult.FailedByUnknownLight
                    Log.Info("MasProUllResponse with MasProUllResult.FailedByUnknownLight received.")
                    AlertBox.Show(Lexis.UllMasProFileFailedByUnknownLight)
                    Exit Sub
                Case Else
                    Log.Fatal("The telegrapher seems broken.")
                    AlertBox.Show(Lexis.UnforeseenErrorOccurred)
                    OpClientUtil.RestartBrokenTelegrapher()
                    Exit Sub
            End Select

            Me.bSaved = True

        Catch ex As OPMGException
            Log.Error("MasProUll failed.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)

        Finally
            Call waitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' �u�I���v�{�^���N���b�N
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�u�I���v�{�^�����N���b�N���邱�Ƃɂ��A�u�v���O�����Ǘ����j���[�v��ʂɖ߂�B</remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles btnReturn.Click

        Dim oRet As Windows.Forms.DialogResult

        LogOperation(sender, e)    '�{�^���������O
        If Me.bSaved = False And Me.btnSaveData.Enabled = True Then
            '�f�[�^���o�^����Ă��܂���B\n�I�����Ă���낵���ł����H
            oRet = AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyExitWithoutUll)

            If oRet = Windows.Forms.DialogResult.No Then
                LogOperation(Lexis.NoButtonClicked)     'No�{�^���������O
                Exit Sub
            End If

            LogOperation(Lexis.YesButtonClicked)     'Yes�{�^���������O
        End If

        Me.Close()

    End Sub

#End Region

#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    ''' �u�t�@�C�����v ���擾����B
    ''' </summary>
    ''' <remarks>�t�@�C���_�C�A���O��\�����A�w�肳�ꂽ�t�@�C�������擾����B
    ''' �t�@�C�������e�R�[�h�P�ʂɕ�������B</remarks>
    ''' <returns>�����iTrue�j�A���s�iFalse�j</returns>
    Private Function getDataFromFName(ByVal sPath As String) As Boolean

        Dim bRtn As Boolean = False

        Try
            '�t�@�C�������u99_XXXXXX_99999999.CAB�v�^�������`�F�b�N
            Me.sFileName = Path.GetFileName(sPath)
            If EkProgramDataFileName.IsValid(sFileName) Then
                Me.sPrmKind = EkProgramDataFileName.GetKind(sFileName)
                Me.sAreaKind = EkProgramDataFileName.GetSubKind(sFileName)
                Me.sMdlKind = EkProgramDataFileName.GetApplicableModel(sFileName)
                Me.sAfterVer = EkProgramDataFileName.GetVersion(sFileName)
                bRtn = True
            Else
                '�I�����ꂽ�t�@�C���̓v���O�����t�@�C���ł͂���܂���B
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�v���O�����t�@�C��")
                bRtn = False
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"�\�����ʃG���[���������܂����B"
            '�I�����ꂽ�t�@�C���̓v���O�����t�@�C���ł͂���܂���B
            AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�v���O�����t�@�C��")

        End Try

        Return bRtn

    End Function

    Private Function getExeDate(ByVal sPath As String) As Boolean

        Dim bRtn As Boolean = False

        Try
            '�ꎞ��Ɨp�f�B���N�g��������������B
            Utility.DeleteTemporalDirectory(Config.TemporaryBaseDirPath)
            Directory.CreateDirectory(Config.TemporaryBaseDirPath)

            'CAB��W�J����B
            Using oProcess As New System.Diagnostics.Process()
                oProcess.StartInfo.FileName = Path.Combine(My.Application.Info.DirectoryPath, "TsbCab.exe")
                oProcess.StartInfo.Arguments = "-x """ & sPath & """ """ & Config.TemporaryBaseDirPath & "\"""
                oProcess.StartInfo.UseShellExecute = False
                oProcess.StartInfo.RedirectStandardInput = True
                oProcess.StartInfo.CreateNoWindow = True
                oProcess.Start()
                Dim oStreamWriter As StreamWriter = oProcess.StandardInput
                oStreamWriter.WriteLine("")
                oStreamWriter.Close()
                oProcess.WaitForExit()
            End Using

            Dim sVerListPath As String = ""
            Select Case Me.sMdlKind
                Case "W"
                    sVerListPath = Config.KsbProgramVersionListPathInCab
                Case "G"
                    sVerListPath = Config.GateProgramVersionListPathInCab
                Case "Y"
                    sVerListPath = Config.MadoProgramVersionListPathInCab
            End Select
            sVerListPath = Utility.CombinePathWithVirtualPath(Config.TemporaryBaseDirPath, sVerListPath)

            '�v���O�����o�[�W�������X�g����@�틤�ʕ���ǂݏo���B
            Dim oVerList As EkProgramVersionListHeader
            Try
                oVerList = New EkProgramVersionListHeader(sVerListPath)
                sExeDate = Format(oVerList.RunnableDate, "yyyy/MM/dd")
                bRtn = True
            Catch ex As IOException
                Log.Error("Exception caught.", ex)
                '�I�����ꂽ�t�@�C���̓v���O�����t�@�C���ł͂���܂���B
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�v���O�����t�@�C��")
                bRtn = False
            End Try

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"�\�����ʃG���[���������܂����B"
            '�I�����ꂽ�t�@�C���̓v���O�����t�@�C���ł͂���܂���B
            AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�v���O�����t�@�C��")

        End Try

        Return bRtn

    End Function

    ''' <summary>
    ''' �u�ŐV�o�[�W�����v�y�ѓo�^�����擾����
    ''' </summary>
    ''' <remarks>�v���O�����Ǘ��e�[�u�����������A�ŐV�o�[�W�����y�ѓo�^�����擾����B</remarks>
    Private Function getDataFromDb() As Boolean

        Dim bRtn As Boolean = False
        Dim sSQL As String = ""
        Dim dbCtl As New DatabaseTalker
        Dim dtTable As New DataTable

        'DB�I�[�v��
        Try
            dbCtl.ConnectOpen()
        Catch ex As DatabaseException

        End Try

        'DB�ڑ��Ɏ��s���܂���
        If dbCtl.IsConnect = False Then
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Return bRtn
        End If


        'DB�o�^����Ă���ŐV�o�[�W�������擾
        Try
            sSQL = "SELECT TOP 1 UPDATE_DATE, DATA_VERSION FROM S_PRG_DATA_HEADLINE" _
                & " WHERE MODEL_CODE = '" & Me.sMdlKind & "'" _
                & " AND DATA_KIND = '" & Me.sPrmKind & "'" _
                & " AND DATA_SUB_KIND = '" & Me.sAreaKind & "'" _
                & " ORDER BY UPDATE_DATE DESC"
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)

            '�O��̓o�^���t�ƃo�[�W�������Z�b�g
            If dtTable.Rows.Count = 1 Then
                Me.sUpDate = Format(Convert.ToDateTime(dtTable.Rows(0).Item("UPDATE_DATE")), "yyyy/MM/dd HH:mm:ss")
                Me.sBeforVer = dtTable.Rows(0).Item("DATA_VERSION").ToString
            End If

            bRtn = True

        Catch ex As Exception
            '�ڑ������Ɏ��s���܂���
            AlertBox.Show(Lexis.ConnectFailed)

        Finally
            dbCtl.ConnectClose()

        End Try

        Return bRtn

    End Function

    Private Function checkKindFromDb() As Boolean

        Dim bRtn As Boolean = False
        Dim sSQL As String = ""
        Dim dbCtl As New DatabaseTalker
        Dim dtTable As New DataTable

        'DB�I�[�v��
        Try
            dbCtl.ConnectOpen()
        Catch ex As DatabaseException

        End Try

        'DB�ڑ��Ɏ��s���܂���
        If dbCtl.IsConnect = False Then
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Return bRtn
        End If


        '�v���O�������́A�@�햼�̂̎擾�B
        Try
            sSQL = "SELECT MO.MODEL_NAME AS MODEL_NAME ,PG.NAME AS PRG_NAME FROM M_PRG_NAME AS PG" _
                   & " ,M_MODEL AS MO WHERE PG.MODEL_CODE=MO.MODEL_CODE AND PG.FILE_KBN='DAT'" _
                   & " AND PG.MODEL_CODE ='" & Me.sMdlKind & "'"
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)

            If dtTable.Rows.Count > 0 Then
                Me.sModName = dtTable.Rows(0).Item("MODEL_NAME").ToString
                Me.sPrmName = dtTable.Rows(0).Item("PRG_NAME").ToString
                bRtn = True
            Else
                '�I�����ꂽ�t�@�C���̓v���O�����t�@�C���ł͂���܂���B
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�v���O�����t�@�C��")
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"�\�����ʃG���[���������܂����B"
            '�ڑ������Ɏ��s���܂���
            AlertBox.Show(Lexis.ConnectFailed)

        Finally
            dbCtl.ConnectClose()

        End Try

        Return bRtn

    End Function

    Private Function getAreaFromDb() As Boolean

        Dim bRtn As Boolean = False
        Dim sSQL As String = ""
        Dim dbCtl As New DatabaseTalker
        Dim dtTable As New DataTable

        'DB�I�[�v��
        Try
            dbCtl.ConnectOpen()
        Catch ex As DatabaseException

        End Try

        'DB�ڑ��Ɏ��s���܂���
        If dbCtl.IsConnect = False Then
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Return bRtn
        End If


        '�G���A���̂̎擾�B
        Try
            sSQL = "SELECT AREA_NAME FROM M_AREA_DATA" _
                   & " WHERE MODEL_CODE = '" & Me.sMdlKind & "'" _
                   & " AND AREA_NO ='" & Me.sAreaKind & "'"
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)

            If dtTable.Rows.Count > 0 Then
                Me.sAreaName = dtTable.Rows(0).Item("AREA_NAME").ToString
                bRtn = True
            Else
                '�G���A�f�[�^���o�^����Ă��܂���B
                AlertBox.Show(Lexis.TheAreaNoDoesNotExist)
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"�\�����ʃG���[���������܂����B"
            '�ڑ������Ɏ��s���܂���
            AlertBox.Show(Lexis.ConnectFailed)

        Finally
            dbCtl.ConnectClose()

        End Try

        Return bRtn

    End Function

    ''' <summary>
    ''' �e���x����ݒ肵�A�\������B
    ''' </summary>
    ''' <remarks>�u�@�햼�́v�u�v���O�������́v�u�G���A���́v
    ''' �u�O��o�^�o�[�W�����v�u�o�^�����v�u����o�^�o�[�W�����v��\������B</remarks>
    Private Sub showLable()

        '�t�@�C�������@�햼�̂�\���B
        Me.lblModelName.Text = Me.sModName

        '�t�@�C�������G���A���̂�\��
        Me.lblAppliedArea.Text = Me.sAreaName

        '�t�@�C�������v���O������\��
        Me.lblPrgName.Text = Me.sPrmName


        'DB���������Ǎ��񂾃v���O�����̑O��o�^�o�[�W������\��()
        '�O��o�^�f�[�^�����݂��Ȃ��ꍇ�́A�u�󔒁v��\��
        Me.lblBeforeVer.Text = Me.sBeforVer

        'DB���������Ǎ��񂾃v���O�����̑O��o�^������\��
        '�O��o�^�f�[�^�����݂��Ȃ��ꍇ�́A�u�󔒁v��\��
        Me.lblSaveDT.Text = Me.sUpDate

        '�t�@�C�������o�[�W������\��
        Me.lblAfterVer.Text = Me.sAfterVer

        Me.lblAcceptDate.Text = Me.sExeDate

    End Sub

    ''' <summary>
    ''' ���x�������̐ݒ�B
    ''' </summary>
    ''' <param name="bEnableLbl">�e���x���̉���</param>
    Private Sub setLbl(ByVal bEnableLbl As Boolean)

        lblModelName.Visible = bEnableLbl
        lblAppliedArea.Visible = bEnableLbl
        lblPrgName.Visible = bEnableLbl
        lblBeforeVer.Visible = bEnableLbl
        lblAfterVer.Visible = bEnableLbl
        lblSaveDT.Visible = bEnableLbl

    End Sub

#End Region

End Class
