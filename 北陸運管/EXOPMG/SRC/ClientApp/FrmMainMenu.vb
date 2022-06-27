' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
'   0.1      2013/11/11  (NES)����  �t�F�[�Y�Q�����Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>���C�����j���[</summary>
''' <remarks></remarks>
Public Class FrmMainMenu
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
    Public WithEvents btnUnyo As System.Windows.Forms.Button
    Public WithEvents btnLogout As System.Windows.Forms.Button
    Public WithEvents btnSystem As System.Windows.Forms.Button
    Public WithEvents btnHosyu As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.btnUnyo = New System.Windows.Forms.Button
        Me.btnLogout = New System.Windows.Forms.Button
        Me.btnSystem = New System.Windows.Forms.Button
        Me.btnHosyu = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2006/08/10(��)  10:10"
        '
        'btnUnyo
        '
        Me.btnUnyo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnUnyo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUnyo.Location = New System.Drawing.Point(300, 140)
        Me.btnUnyo.Name = "btnUnyo"
        Me.btnUnyo.Size = New System.Drawing.Size(416, 86)
        Me.btnUnyo.TabIndex = 3
        Me.btnUnyo.Text = "�^�p�Ǘ��Ɩ�"
        Me.btnUnyo.UseVisualStyleBackColor = False
        '
        'btnLogout
        '
        Me.btnLogout.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnLogout.Font = New System.Drawing.Font("�l�r �S�V�b�N", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnLogout.Location = New System.Drawing.Point(300, 560)
        Me.btnLogout.Name = "btnLogout"
        Me.btnLogout.Size = New System.Drawing.Size(416, 86)
        Me.btnLogout.TabIndex = 6
        Me.btnLogout.Text = "���O�A�E�g"
        Me.btnLogout.UseVisualStyleBackColor = False
        '
        'btnSystem
        '
        Me.btnSystem.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnSystem.Font = New System.Drawing.Font("�l�r �S�V�b�N", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSystem.Location = New System.Drawing.Point(300, 420)
        Me.btnSystem.Name = "btnSystem"
        Me.btnSystem.Size = New System.Drawing.Size(416, 86)
        Me.btnSystem.TabIndex = 5
        Me.btnSystem.Text = "�V�X�e���Ǘ��Ɩ�"
        Me.btnSystem.UseVisualStyleBackColor = False
        '
        'btnHosyu
        '
        Me.btnHosyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnHosyu.Font = New System.Drawing.Font("�l�r �S�V�b�N", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnHosyu.Location = New System.Drawing.Point(300, 280)
        Me.btnHosyu.Name = "btnHosyu"
        Me.btnHosyu.Size = New System.Drawing.Size(416, 86)
        Me.btnHosyu.TabIndex = 4
        Me.btnHosyu.Text = "�ێ�Ǘ��Ɩ�"
        Me.btnHosyu.UseVisualStyleBackColor = False
        '
        'FrmMainMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Controls.Add(Me.btnUnyo)
        Me.Controls.Add(Me.btnLogout)
        Me.Controls.Add(Me.btnSystem)
        Me.Controls.Add(Me.btnHosyu)
        Me.Name = "FrmMainMenu"
        Me.Controls.SetChildIndex(Me.pnlBodyBase, 0)
        Me.Controls.SetChildIndex(Me.btnHosyu, 0)
        Me.Controls.SetChildIndex(Me.btnSystem, 0)
        Me.Controls.SetChildIndex(Me.btnLogout, 0)
        Me.Controls.SetChildIndex(Me.btnUnyo, 0)
        Me.Controls.SetChildIndex(Me.lblTitle, 0)
        Me.Controls.SetChildIndex(Me.lblToday, 0)
        Me.ResumeLayout(False)

    End Sub

#End Region

    '�t�H�[�����[�h
    Private Sub FrmMainMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '��ʃ^�C�g��
        lblTitle.Text = "���C�����j���["

        '��������������
        '2�F�^�p�Ǘ�
        If (FrmBase.Authority = "2") Then

            btnUnyo.Enabled = True

            btnHosyu.Enabled = True

            btnSystem.Enabled = False

            '3�F�ێ�Ǘ�
        ElseIf (FrmBase.Authority = "3") Then

            btnUnyo.Enabled = False

            btnHosyu.Enabled = True

            btnSystem.Enabled = False
            '1�F�V�X�e���Ǘ�
        ElseIf (FrmBase.Authority = "1") Then

            btnUnyo.Enabled = True

            btnHosyu.Enabled = True

            btnSystem.Enabled = True
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
        ElseIf (FrmBase.Authority = "4") Then
            Dim UCount As Integer = 0
            Dim SCount As Integer = 0
            Dim HCount As Integer = 0
            For a As Integer = 0 To FrmBase.DetailSet.Count - 1
                If (FrmBase.DetailSet(a).ToString = "1") Then
                    If (a < 10) Then
                        UCount = UCount + 1
                    ElseIf ((a > 9) And (a < 20)) Then
                        HCount = HCount + 1
                    ElseIf (a > 19) Then
                        SCount = SCount + 1
                    End If
                End If
            Next
            If (UCount > 0) Then
                btnUnyo.Enabled = True
            Else
                btnUnyo.Enabled = False
            End If
            If (HCount > 0) Then
                btnHosyu.Enabled = True
            Else
                btnHosyu.Enabled = False
            End If
            If (SCount > 0) Then
                btnSystem.Enabled = True
            Else
                btnSystem.Enabled = False
            End If
        End If
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
    End Sub

    '�u�^�p�Ǘ��Ɩ��v�{�^���N���b�N
    Private Sub btnUnyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnyo.Click

        Call waitCursor(True)
        '�u�^�p�Ǘ��Ɩ��v�{�^�������B
        LogOperation(sender, e)

        Dim oFrmOpeMenu As New FrmOpeMenu

        Me.Hide()
        oFrmOpeMenu.ShowDialog()
        oFrmOpeMenu.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u�ێ�Ǘ��Ɩ��v�{�^���N���b�N
    Private Sub btnHosyu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHosyu.Click

        Call waitCursor(True)
        '�u�ێ�Ǘ��Ɩ��v�{�^�������B
        LogOperation(sender, e)

        Dim oFrmMntMenu As New FrmMntMenu

        Me.Hide()
        oFrmMntMenu.ShowDialog()
        oFrmMntMenu.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u�V�X�e���Ǘ��Ɩ��v�{�^���N���b�N
    Private Sub btnSystem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSystem.Click

        Call waitCursor(True)
        '�u�V�X�e���Ǘ��Ɩ��v�{�^�������B
        LogOperation(sender, e)

        Dim oFrmSysMenu As New FrmSysMenu

        Me.Hide()
        oFrmSysMenu.ShowDialog()
        oFrmSysMenu.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u���O�A�E�g�v�{�^���N���b�N
    Private Sub btnLogout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogout.Click

        '�u���O�A�E�g�v�{�^�������B
        LogOperation(sender, e)

        Me.Close()

    End Sub

End Class
