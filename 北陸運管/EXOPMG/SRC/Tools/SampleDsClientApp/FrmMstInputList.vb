' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e  �V�K�쐬
'   0.1      2013/05/13  (NES)����  �f�W�N���C�A���g�����A�v����
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports System.IO

''' <summary>
''' �}�X�^�K�p���X�g�o�^
''' </summary>
''' <remarks>�}�X�^�Ǘ����j���[���u�}�X�^�K�p���X�g�o�^�v�{�^�����N���b�N����ƁA�{��ʂ�\������B
''' �{��ʂɂă}�X�^�K�p���X�g�̓Ǎ��݁A�o�^���s���B</remarks>
Public Class FrmMstInputList
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

    Friend WithEvents dlgOpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnSaveData As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnOpenFile As System.Windows.Forms.Button
    Friend WithEvents lblTgl As System.Windows.Forms.Label
    Friend WithEvents lblPtnNa As System.Windows.Forms.Label
    Friend WithEvents lblBefore As System.Windows.Forms.Label
    Friend WithEvents lblAfter As System.Windows.Forms.Label
    Friend WithEvents lblTglName As System.Windows.Forms.Label
    Friend WithEvents lblPtnName As System.Windows.Forms.Label
    Friend WithEvents lblPtnNo As System.Windows.Forms.Label
    Friend WithEvents lblBeforeVer As System.Windows.Forms.Label
    Friend WithEvents lblPtnN As System.Windows.Forms.Label
    Friend WithEvents lblAfterVer As System.Windows.Forms.Label
    Friend WithEvents lblSave As System.Windows.Forms.Label
    Friend WithEvents lblSaveDT As System.Windows.Forms.Label
    Friend WithEvents lblMdl As System.Windows.Forms.Label
    Friend WithEvents lblModelName As System.Windows.Forms.Label


    Private Sub InitializeComponent()
        Me.dlgOpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.lblModelName = New System.Windows.Forms.Label()
        Me.lblMdl = New System.Windows.Forms.Label()
        Me.lblSaveDT = New System.Windows.Forms.Label()
        Me.lblSave = New System.Windows.Forms.Label()
        Me.lblAfterVer = New System.Windows.Forms.Label()
        Me.lblPtnN = New System.Windows.Forms.Label()
        Me.lblBeforeVer = New System.Windows.Forms.Label()
        Me.lblPtnNo = New System.Windows.Forms.Label()
        Me.lblPtnName = New System.Windows.Forms.Label()
        Me.lblTglName = New System.Windows.Forms.Label()
        Me.lblAfter = New System.Windows.Forms.Label()
        Me.lblBefore = New System.Windows.Forms.Label()
        Me.lblPtnNa = New System.Windows.Forms.Label()
        Me.lblTgl = New System.Windows.Forms.Label()
        Me.btnOpenFile = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnSaveData = New System.Windows.Forms.Button()
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
        Me.pnlBodyBase.Controls.Add(Me.lblModelName)
        Me.pnlBodyBase.Controls.Add(Me.lblMdl)
        Me.pnlBodyBase.Controls.Add(Me.lblSaveDT)
        Me.pnlBodyBase.Controls.Add(Me.lblSave)
        Me.pnlBodyBase.Controls.Add(Me.lblAfterVer)
        Me.pnlBodyBase.Controls.Add(Me.lblPtnN)
        Me.pnlBodyBase.Controls.Add(Me.lblBeforeVer)
        Me.pnlBodyBase.Controls.Add(Me.lblPtnNo)
        Me.pnlBodyBase.Controls.Add(Me.lblPtnName)
        Me.pnlBodyBase.Controls.Add(Me.lblTglName)
        Me.pnlBodyBase.Controls.Add(Me.lblAfter)
        Me.pnlBodyBase.Controls.Add(Me.lblBefore)
        Me.pnlBodyBase.Controls.Add(Me.lblPtnNa)
        Me.pnlBodyBase.Controls.Add(Me.lblTgl)
        Me.pnlBodyBase.Controls.Add(Me.btnOpenFile)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnSaveData)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/05/12(��)  21:13"
        '
        'dlgOpenFileDialog
        '
        Me.dlgOpenFileDialog.ReadOnlyChecked = True
        '
        'lblModelName
        '
        Me.lblModelName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModelName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!)
        Me.lblModelName.Location = New System.Drawing.Point(319, 181)
        Me.lblModelName.Name = "lblModelName"
        Me.lblModelName.Size = New System.Drawing.Size(251, 18)
        Me.lblModelName.TabIndex = 86
        Me.lblModelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMdl
        '
        Me.lblMdl.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMdl.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMdl.Location = New System.Drawing.Point(135, 181)
        Me.lblMdl.Name = "lblMdl"
        Me.lblMdl.Size = New System.Drawing.Size(178, 18)
        Me.lblMdl.TabIndex = 85
        Me.lblMdl.Text = "�@�햼��"
        '
        'lblSaveDT
        '
        Me.lblSaveDT.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSaveDT.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSaveDT.Location = New System.Drawing.Point(319, 344)
        Me.lblSaveDT.Name = "lblSaveDT"
        Me.lblSaveDT.Size = New System.Drawing.Size(252, 16)
        Me.lblSaveDT.TabIndex = 84
        Me.lblSaveDT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSave
        '
        Me.lblSave.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSave.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSave.Location = New System.Drawing.Point(135, 344)
        Me.lblSave.Name = "lblSave"
        Me.lblSave.Size = New System.Drawing.Size(160, 16)
        Me.lblSave.TabIndex = 83
        Me.lblSave.Text = "�O��o�^����"
        '
        'lblAfterVer
        '
        Me.lblAfterVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfterVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfterVer.Location = New System.Drawing.Point(319, 438)
        Me.lblAfterVer.Name = "lblAfterVer"
        Me.lblAfterVer.Size = New System.Drawing.Size(251, 16)
        Me.lblAfterVer.TabIndex = 82
        Me.lblAfterVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnN
        '
        Me.lblPtnN.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnN.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnN.Location = New System.Drawing.Point(135, 298)
        Me.lblPtnN.Name = "lblPtnN"
        Me.lblPtnN.Size = New System.Drawing.Size(160, 16)
        Me.lblPtnN.TabIndex = 81
        Me.lblPtnN.Text = "(�p�^�[���ԍ�)"
        '
        'lblBeforeVer
        '
        Me.lblBeforeVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBeforeVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBeforeVer.Location = New System.Drawing.Point(319, 390)
        Me.lblBeforeVer.Name = "lblBeforeVer"
        Me.lblBeforeVer.Size = New System.Drawing.Size(251, 16)
        Me.lblBeforeVer.TabIndex = 80
        Me.lblBeforeVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnNo
        '
        Me.lblPtnNo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnNo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNo.Location = New System.Drawing.Point(319, 298)
        Me.lblPtnNo.Name = "lblPtnNo"
        Me.lblPtnNo.Size = New System.Drawing.Size(168, 16)
        Me.lblPtnNo.TabIndex = 79
        Me.lblPtnNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnName
        '
        Me.lblPtnName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnName.Location = New System.Drawing.Point(319, 280)
        Me.lblPtnName.Name = "lblPtnName"
        Me.lblPtnName.Size = New System.Drawing.Size(252, 18)
        Me.lblPtnName.TabIndex = 78
        Me.lblPtnName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTglName
        '
        Me.lblTglName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTglName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTglName.Location = New System.Drawing.Point(319, 232)
        Me.lblTglName.Name = "lblTglName"
        Me.lblTglName.Size = New System.Drawing.Size(251, 18)
        Me.lblTglName.TabIndex = 77
        Me.lblTglName.Text = "�P�Q�R�S�T�U�V�W�X�O�P�Q�R�S�T"
        Me.lblTglName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAfter
        '
        Me.lblAfter.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfter.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfter.Location = New System.Drawing.Point(135, 438)
        Me.lblAfter.Name = "lblAfter"
        Me.lblAfter.Size = New System.Drawing.Size(160, 16)
        Me.lblAfter.TabIndex = 76
        Me.lblAfter.Text = "����o�^�o�[�W����"
        '
        'lblBefore
        '
        Me.lblBefore.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBefore.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBefore.Location = New System.Drawing.Point(135, 390)
        Me.lblBefore.Name = "lblBefore"
        Me.lblBefore.Size = New System.Drawing.Size(160, 16)
        Me.lblBefore.TabIndex = 75
        Me.lblBefore.Text = "�O��o�^�o�[�W����"
        '
        'lblPtnNa
        '
        Me.lblPtnNa.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnNa.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNa.Location = New System.Drawing.Point(135, 280)
        Me.lblPtnNa.Name = "lblPtnNa"
        Me.lblPtnNa.Size = New System.Drawing.Size(160, 18)
        Me.lblPtnNa.TabIndex = 74
        Me.lblPtnNa.Text = "�p�^�[������"
        '
        'lblTgl
        '
        Me.lblTgl.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTgl.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTgl.Location = New System.Drawing.Point(135, 232)
        Me.lblTgl.Name = "lblTgl"
        Me.lblTgl.Size = New System.Drawing.Size(178, 18)
        Me.lblTgl.TabIndex = 73
        Me.lblTgl.Text = "�}�X�^�K�p���X�g����"
        '
        'btnOpenFile
        '
        Me.btnOpenFile.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnOpenFile.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnOpenFile.Location = New System.Drawing.Point(749, 298)
        Me.btnOpenFile.Name = "btnOpenFile"
        Me.btnOpenFile.Size = New System.Drawing.Size(128, 40)
        Me.btnOpenFile.TabIndex = 70
        Me.btnOpenFile.Text = "�ǁ@��"
        Me.btnOpenFile.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(749, 414)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 72
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnSaveData
        '
        Me.btnSaveData.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnSaveData.Enabled = False
        Me.btnSaveData.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSaveData.Location = New System.Drawing.Point(749, 356)
        Me.btnSaveData.Name = "btnSaveData"
        Me.btnSaveData.Size = New System.Drawing.Size(128, 40)
        Me.btnSaveData.TabIndex = 71
        Me.btnSaveData.Text = "�o�@�^"
        Me.btnSaveData.UseVisualStyleBackColor = False
        '
        'FrmMstInputList
        '
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMstInputList"
        Me.Text = "�^�p�[�� "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
#End Region

#Region "�錾�̈�iPrivate�j"

    Private sPathWithName As String = ""        '�t���p�X�t�@�C����
    Private sFileName As String = ""            '�t�@�C����

    Private sMdlName As String = ""             '�@�햼��
    Private sMdlKind As String = ""             '�@��R�[�h
    Private sTglName As String = ""             '�}�X�^�K�p���X�g����
    Private sBeforVer As String = ""            '�O��o�^�o�[�W����
    Private sUpDate As String = ""              '�o�^����
    Private sPatternName As String = ""         '�p�^�[������
    Private sNewUpDate As String = ""           '�ŏI�o�^����

    Private sTglKind As String = ""             '�}�X�^�K�p���X�g���
    Private sPatternNo As String = ""           '�p�^�[���ԍ�
    Private sAfterVer As String = ""            '����o�^�o�[�W����
    Private sMstVer As String = ""              '�}�X�^�o�[�W����

    Private bSaved As Boolean = False           '�o�^����

    Private ReadOnly LcstFormTitle As String = "�}�X�^�K�p���X�g�o�^"

#End Region

#Region "�C�x���g"

    Private Sub FrmMstInputList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
    ''' <remarks>�u�Ǎ��v�{�^�����N���b�N���邱�Ƃɂ��O���}�̂���}�X�^�K�p���X�g��Ǎ��݁A
    ''' �u�@�햼�́v�u�}�X�^�K�p���X�g���́v�u�p�^�[�����́v�u�i�p�^�[��No�j�v
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
        sMdlName = ""             '�@�햼��
        sTglName = ""             '�}�X�^�K�p���X�g����
        sPatternName = ""         '�p�^�[������
        sPatternNo = ""           '�p�^�[���ԍ�

        sBeforVer = ""            '�O��o�^�o�[�W����
        sUpDate = ""              '�o�^����
        sAfterVer = ""            '����o�^�o�[�W����
        sMstVer = ""              '�}�X�^�o�[�W����

        sMdlKind = ""             '�@��R�[�h
        sTglKind = ""             '�}�X�^���
        sNewUpDate = ""           '�ŏI�o�^����
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

        '�e�R�[�h���疼�̂��擾����
        If checkKindFromDb() = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        '�p�^�[�����̂��擾����
        If getPatternFromDb() = False Then
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
    ''' <remarks>�u�I���v�{�^�����N���b�N���邱�Ƃɂ��A�u�}�X�^�Ǘ����j���[�v��ʂɖ߂�B</remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
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
    '''  <returns>�����iTrue�j�A���s�iFalse�j</returns>
    Private Function getDataFromFName(ByVal sPath As String) As Boolean

        Dim bRtn As Boolean = False

        Try
            '�t�@�C�������uTGL_XXX99_X_999_99.csv�v�^�������`�F�b�N
            Me.sFileName = Path.GetFileName(sPath)
            If EkMasProListFileName.IsValid(sFileName) AndAlso _
               EkMasProListFileName.GetListKind(sFileName).Equals("TGL") Then
                Me.sTglKind = EkMasProListFileName.GetDataKind(sFileName)
                Me.sPatternNo = EkMasProListFileName.GetDataSubKind(sFileName)
                Me.sMdlKind = EkMasProListFileName.GetDataApplicableModel(sFileName)
                Me.sMstVer = EkMasProListFileName.GetDataVersion(sFileName)
                Me.sAfterVer = EkMasProListFileName.GetListVersion(sFileName)
                bRtn = True
            Else
                '�I�����ꂽ�t�@�C���̓}�X�^�K�p���X�g�t�@�C���ł͂���܂���B
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�}�X�^�K�p���X�g�t�@�C��")
                bRtn = False
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"�\�����ʃG���[���������܂����B"
            '�I�����ꂽ�t�@�C���̓}�X�^�K�p���X�g�t�@�C���ł͂���܂���B
            AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�}�X�^�K�p���X�g�t�@�C��")

        End Try

        Return bRtn

    End Function

    ''' <summary>
    ''' �u�ŐV�o�[�W�����v�y�ѓo�^�����擾����
    ''' </summary>
    ''' <remarks>�}�X�^�Ǘ��e�[�u�����������A�ŐV�o�[�W�����y�ѓo�^�����擾����B</remarks>
    Private Function getDataFromDb() As Boolean

        Me.sUpDate = "�����ł̂��ߕ\���ł��܂���"
        Me.sBeforVer = "�����ł̂��ߕ\���ł��܂���"
        Return True

    End Function

    Private Function checkKindFromDb() As Boolean

        Me.sTglName = "�����ł̂��ߕ\���ł��܂���"
        Me.sMdlName = "�����ł̂��ߕ\���ł��܂���"
        Return True

    End Function

    Private Function getPatternFromDb() As Boolean

        Me.sPatternName = "�����ł̂��ߕ\���ł��܂���"
        Return True

    End Function

    ''' <summary>
    ''' �e���x����ݒ肵�A�\������B
    ''' </summary>
    ''' <remarks>�u�}�X�^���́v�u�p�^�[�����́v�u�O��o�^�o�[�W�����v�u�o�^�����v�u����o�^�o�[�W�����v��\������B</remarks>
    Private Sub showLable()

        Me.lblModelName.Text = Me.sMdlName

        '�t�@�C�������}�X�^���̂�\��
        Me.lblTglName.Text = Me.sTglName

        '�t�@�C�������p�^�[�����̂�\��
        Me.lblPtnName.Text = Me.sPatternName

        '�t�@�C�������p�^�[���ԍ���\��
        Me.lblPtnNo.Text = "(" & sPatternNo & ")"

        'DB���������Ǎ��񂾃}�X�^�̑O��o�^�o�[�W������\��
        '�O��o�^�f�[�^�����݂��Ȃ��ꍇ�́A�u�󔒁v��\��
        Me.lblBeforeVer.Text = Me.sBeforVer

        'DB���������Ǎ��񂾃}�X�^�̑O��o�^������\��
        '�O��o�^�f�[�^�����݂��Ȃ��ꍇ�́A�u�󔒁v��\��
        Me.lblSaveDT.Text = Me.sUpDate

        '�t�@�C���̓��e���������ĕ\��
        Me.lblAfterVer.Text = Me.sAfterVer

    End Sub

    ''' <summary>
    ''' ���x�������̐ݒ�B
    ''' </summary>
    ''' <param name="bEnableLbl">�e���x���̉���</param>
    Private Sub setLbl(ByVal bEnableLbl As Boolean)

        lblModelName.Visible = bEnableLbl
        lblTglName.Visible = bEnableLbl
        lblPtnName.Visible = bEnableLbl
        lblPtnNo.Visible = bEnableLbl
        lblBeforeVer.Visible = bEnableLbl
        lblAfterVer.Visible = bEnableLbl
        lblSaveDT.Visible = bEnableLbl

    End Sub

#End Region

End Class
