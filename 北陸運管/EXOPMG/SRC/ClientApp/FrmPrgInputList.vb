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
''' �v���O�����K�p���X�g�捞
''' </summary>
''' <remarks>�v���O�����Ǘ����j���[���u�v���O�����K�p���X�g�捞�v�{�^�����N���b�N����ƁA�{��ʂ�\������B
''' �{��ʂɂăv���O�����K�p���X�g�̓Ǎ��݁A�o�^���s���B</remarks>
Public Class FrmPrgInputList
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

    Friend WithEvents lblModelName As System.Windows.Forms.Label
    Friend WithEvents lblKisyu As System.Windows.Forms.Label
    Friend WithEvents lblSaveDT As System.Windows.Forms.Label
    Friend WithEvents lblSave As System.Windows.Forms.Label
    Friend WithEvents lblAfterVer As System.Windows.Forms.Label
    Friend WithEvents lblBeforeVer As System.Windows.Forms.Label
    Friend WithEvents lblTdlName As System.Windows.Forms.Label
    Friend WithEvents lblAppliedArea As System.Windows.Forms.Label
    Friend WithEvents lblAfter As System.Windows.Forms.Label
    Friend WithEvents lblBefore As System.Windows.Forms.Label
    Friend WithEvents lblTdlNa As System.Windows.Forms.Label
    Friend WithEvents lblPrm As System.Windows.Forms.Label
    Friend WithEvents btnOpenFile As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents dlgOpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnSaveData As System.Windows.Forms.Button

    Private Sub InitializeComponent()
        Me.lblModelName = New System.Windows.Forms.Label()
        Me.lblKisyu = New System.Windows.Forms.Label()
        Me.lblSaveDT = New System.Windows.Forms.Label()
        Me.lblSave = New System.Windows.Forms.Label()
        Me.lblAfterVer = New System.Windows.Forms.Label()
        Me.lblBeforeVer = New System.Windows.Forms.Label()
        Me.lblTdlName = New System.Windows.Forms.Label()
        Me.lblAppliedArea = New System.Windows.Forms.Label()
        Me.lblAfter = New System.Windows.Forms.Label()
        Me.lblBefore = New System.Windows.Forms.Label()
        Me.lblTdlNa = New System.Windows.Forms.Label()
        Me.lblPrm = New System.Windows.Forms.Label()
        Me.btnOpenFile = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnSaveData = New System.Windows.Forms.Button()
        Me.dlgOpenFileDialog = New System.Windows.Forms.OpenFileDialog()
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
        Me.pnlBodyBase.Controls.Add(Me.lblKisyu)
        Me.pnlBodyBase.Controls.Add(Me.lblSaveDT)
        Me.pnlBodyBase.Controls.Add(Me.lblSave)
        Me.pnlBodyBase.Controls.Add(Me.lblAfterVer)
        Me.pnlBodyBase.Controls.Add(Me.lblBeforeVer)
        Me.pnlBodyBase.Controls.Add(Me.lblTdlName)
        Me.pnlBodyBase.Controls.Add(Me.lblAppliedArea)
        Me.pnlBodyBase.Controls.Add(Me.lblAfter)
        Me.pnlBodyBase.Controls.Add(Me.lblBefore)
        Me.pnlBodyBase.Controls.Add(Me.lblTdlNa)
        Me.pnlBodyBase.Controls.Add(Me.lblPrm)
        Me.pnlBodyBase.Controls.Add(Me.btnOpenFile)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnSaveData)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/08/02(��)  15:26"
        '
        'lblModelName
        '
        Me.lblModelName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModelName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModelName.Location = New System.Drawing.Point(344, 202)
        Me.lblModelName.Name = "lblModelName"
        Me.lblModelName.Size = New System.Drawing.Size(114, 18)
        Me.lblModelName.TabIndex = 103
        Me.lblModelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblKisyu
        '
        Me.lblKisyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblKisyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKisyu.Location = New System.Drawing.Point(134, 202)
        Me.lblKisyu.Name = "lblKisyu"
        Me.lblKisyu.Size = New System.Drawing.Size(160, 18)
        Me.lblKisyu.TabIndex = 102
        Me.lblKisyu.Text = "�@��"
        '
        'lblSaveDT
        '
        Me.lblSaveDT.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSaveDT.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSaveDT.Location = New System.Drawing.Point(526, 365)
        Me.lblSaveDT.Name = "lblSaveDT"
        Me.lblSaveDT.Size = New System.Drawing.Size(168, 16)
        Me.lblSaveDT.TabIndex = 101
        Me.lblSaveDT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSave
        '
        Me.lblSave.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSave.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSave.Location = New System.Drawing.Point(438, 365)
        Me.lblSave.Name = "lblSave"
        Me.lblSave.Size = New System.Drawing.Size(88, 16)
        Me.lblSave.TabIndex = 100
        Me.lblSave.Text = "�o�^�����F"
        '
        'lblAfterVer
        '
        Me.lblAfterVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfterVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfterVer.Location = New System.Drawing.Point(317, 413)
        Me.lblAfterVer.Name = "lblAfterVer"
        Me.lblAfterVer.Size = New System.Drawing.Size(40, 16)
        Me.lblAfterVer.TabIndex = 99
        Me.lblAfterVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblBeforeVer
        '
        Me.lblBeforeVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBeforeVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBeforeVer.Location = New System.Drawing.Point(317, 365)
        Me.lblBeforeVer.Name = "lblBeforeVer"
        Me.lblBeforeVer.Size = New System.Drawing.Size(40, 16)
        Me.lblBeforeVer.TabIndex = 97
        Me.lblBeforeVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblTdlName
        '
        Me.lblTdlName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTdlName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTdlName.Location = New System.Drawing.Point(344, 300)
        Me.lblTdlName.Name = "lblTdlName"
        Me.lblTdlName.Size = New System.Drawing.Size(266, 18)
        Me.lblTdlName.TabIndex = 95
        Me.lblTdlName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAppliedArea
        '
        Me.lblAppliedArea.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAppliedArea.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAppliedArea.Location = New System.Drawing.Point(344, 253)
        Me.lblAppliedArea.Name = "lblAppliedArea"
        Me.lblAppliedArea.Size = New System.Drawing.Size(190, 18)
        Me.lblAppliedArea.TabIndex = 94
        Me.lblAppliedArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAfter
        '
        Me.lblAfter.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfter.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfter.Location = New System.Drawing.Point(134, 413)
        Me.lblAfter.Name = "lblAfter"
        Me.lblAfter.Size = New System.Drawing.Size(160, 16)
        Me.lblAfter.TabIndex = 93
        Me.lblAfter.Text = "����o�^�o�[�W����"
        '
        'lblBefore
        '
        Me.lblBefore.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBefore.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBefore.Location = New System.Drawing.Point(134, 365)
        Me.lblBefore.Name = "lblBefore"
        Me.lblBefore.Size = New System.Drawing.Size(160, 16)
        Me.lblBefore.TabIndex = 92
        Me.lblBefore.Text = "�O��o�^�o�[�W����"
        '
        'lblTdlNa
        '
        Me.lblTdlNa.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTdlNa.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTdlNa.Location = New System.Drawing.Point(134, 301)
        Me.lblTdlNa.Name = "lblTdlNa"
        Me.lblTdlNa.Size = New System.Drawing.Size(204, 18)
        Me.lblTdlNa.TabIndex = 91
        Me.lblTdlNa.Text = "�v���O�����K�p���X�g����"
        '
        'lblPrm
        '
        Me.lblPrm.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrm.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrm.Location = New System.Drawing.Point(134, 253)
        Me.lblPrm.Name = "lblPrm"
        Me.lblPrm.Size = New System.Drawing.Size(160, 18)
        Me.lblPrm.TabIndex = 90
        Me.lblPrm.Text = "�K�p�G���A����"
        '
        'btnOpenFile
        '
        Me.btnOpenFile.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnOpenFile.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnOpenFile.Location = New System.Drawing.Point(748, 295)
        Me.btnOpenFile.Name = "btnOpenFile"
        Me.btnOpenFile.Size = New System.Drawing.Size(128, 40)
        Me.btnOpenFile.TabIndex = 87
        Me.btnOpenFile.Text = "�ǁ@��"
        Me.btnOpenFile.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(748, 411)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 89
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnSaveData
        '
        Me.btnSaveData.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnSaveData.Enabled = False
        Me.btnSaveData.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSaveData.Location = New System.Drawing.Point(748, 353)
        Me.btnSaveData.Name = "btnSaveData"
        Me.btnSaveData.Size = New System.Drawing.Size(128, 40)
        Me.btnSaveData.TabIndex = 88
        Me.btnSaveData.Text = "�o�@�^"
        Me.btnSaveData.UseVisualStyleBackColor = False
        '
        'dlgOpenFileDialog
        '
        Me.dlgOpenFileDialog.ReadOnlyChecked = True
        '
        'FrmPrgInputList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgInputList"
        Me.Text = "�^�p�[��"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
#End Region

#Region "�錾�̈�iPrivate�j"

    Private sPathWithName As String = ""        '�t���p�X�t�@�C����
    Private sFileName As String = ""            '�t�@�C����

    Private sMdlName As String = ""             '�@�햼��
    Private sMdlKind As String = ""             '�@��R�[�h
    Private sTdlName As String = ""             '�v���O�����K�p���X�g����
    Private sBeforVer As String = ""            '�O��o�^�o�[�W����
    Private sUpDate As String = ""              '�o�^����
    Private sAreaName As String = ""            '�K�p�G���A����

    Private sTdlKind As String = ""             '�v���O�����K�p���X�g���
    Private sAreaNo As String = ""              '�p�^�[���ԍ�
    Private sAfterVer As String = ""            '����o�^�o�[�W����
    Private sPrgVer As String = ""              '�v���O�����o�[�W����

    Private bSaved As Boolean = False           '�o�^����

    Private ReadOnly LcstFormTitle As String = "�v���O�����K�p���X�g�捞"

#End Region

#Region "�C�x���g"

    ''' <summary>
    '''�t�H�[�����[�h 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FrmPrgInputList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
    ''' <remarks>�u�Ǎ��v�{�^�����N���b�N���邱�Ƃɂ��O���}�̂���v���O�����K�p���X�g��Ǎ��݁A
    ''' �u�@�햼�́v�u�K�p�G���A���́v�u�}�X�^�K�p���X�g���́v
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
        sTdlName = ""             '�v���O�����K�p���X�g����
        sAreaName = ""            '�G���A����
        sAreaNo = ""              '�G���A�ԍ�

        sBeforVer = ""            '�O��o�^�o�[�W����
        sUpDate = ""              '�o�^����
        sAfterVer = ""            '����o�^�o�[�W����
        sPrgVer = ""              '�v���O�����o�[�W����

        sMdlKind = ""             '�@��R�[�h
        sTdlKind = ""             '�v���O�������
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

        '�G���A���̂��擾����
        If getAreaFromDb() = False Then
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
    '''�u�o�^�v�{�^���N���b�N 
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
               EkMasProListFileName.GetListKind(sFileName).Equals("TDL") Then
                Me.sTdlKind = EkMasProListFileName.GetDataKind(sFileName)
                Me.sAreaNo = EkMasProListFileName.GetDataSubKind(sFileName)
                Me.sMdlKind = EkMasProListFileName.GetDataApplicableModel(sFileName)
                Me.sPrgVer = EkMasProListFileName.GetDataVersion(sFileName)
                Me.sAfterVer = EkMasProListFileName.GetListVersion(sFileName)
                bRtn = True
            Else
                '�I�����ꂽ�t�@�C���̓v���O�����K�p���X�g�t�@�C���ł͂���܂���B
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�v���O�����K�p���X�g�t�@�C��")
                bRtn = False
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"�\�����ʃG���[���������܂����B"
            '�I�����ꂽ�t�@�C���̓v���O�����K�p���X�g�t�@�C���ł͂���܂���B
            AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�v���O�����K�p���X�g�t�@�C��")

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
            sSQL = "SELECT TOP 1 UPDATE_DATE, LIST_VERSION FROM S_PRG_LIST_HEADLINE" _
                & " WHERE MODEL_CODE = '" & Me.sMdlKind & "'" _
                & " AND DATA_KIND = '" & Me.sTdlKind & "'" _
                & " AND DATA_SUB_KIND = '" & Me.sAreaNo & "'" _
                & " AND DATA_VERSION = '" & Me.sPrgVer & "'" _
                & " ORDER BY UPDATE_DATE DESC"
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)

            '�O��̓o�^���t�ƃo�[�W�������Z�b�g
            If dtTable.Rows.Count = 1 Then
                Me.sUpDate = Format(Convert.ToDateTime(dtTable.Rows(0).Item("UPDATE_DATE")), "yyyy/MM/dd HH:mm:ss")
                Me.sBeforVer = dtTable.Rows(0).Item("LIST_VERSION").ToString
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
            sSQL = "SELECT PRG.NAME AS PRG_NAME, MDL.MODEL_NAME FROM M_PRG_NAME AS PRG, M_MODEL AS MDL" _
                   & " where PRG.MODEL_CODE = MDL.MODEL_CODE AND PRG.FILE_KBN = 'LST'" _
                   & " AND MDL.MODEL_CODE = '" & Me.sMdlKind & "'" _
                   & " AND PRG.DATA_KIND ='" & Me.sTdlKind & "'"
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)

            If dtTable.Rows.Count > 0 Then
                Me.sTdlName = dtTable.Rows(0).Item("PRG_NAME").ToString
                Me.sMdlName = dtTable.Rows(0).Item("MODEL_NAME").ToString
                bRtn = True
            Else
                '�I�����ꂽ�t�@�C���̓v���O�����K�p���X�g�t�@�C���ł͂���܂���B
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "�v���O�����K�p���X�g�t�@�C��")
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
                   & " AND AREA_NO ='" & Me.sAreaNo & "'"
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
    ''' <remarks>�u�G���A���́v�u�O��o�^�o�[�W�����v�u�o�^�����v�u����o�^�o�[�W�����v��\������B</remarks>
    Private Sub showLable()

        Me.lblModelName.Text = Me.sMdlName

        '�t�@�C�������}�X�^���̂�\��
        Me.lblTdlName.Text = Me.sTdlName

        '�t�@�C�������G���A���̂�\��
        Me.lblAppliedArea.Text = Me.sAreaName

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
        lblTdlName.Visible = bEnableLbl
        lblAppliedArea.Visible = bEnableLbl
        lblBeforeVer.Visible = bEnableLbl
        lblAfterVer.Visible = bEnableLbl
        lblSaveDT.Visible = bEnableLbl

    End Sub

#End Region


End Class
