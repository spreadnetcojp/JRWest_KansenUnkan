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

''' <summary>�v���O�����Ǘ����j���[</summary>
''' <remarks></remarks>

Public Class FrmPrgMenu
    Inherits FrmBaseMenu

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

    'TODO: �ȉ��A�ł���΁ABackColor�v���p�e�B�ł͂Ȃ��A���̂悤��Name�v���p�e�B���Z�b�g����R�[�h�ɂ������B
    ' ���� : �ȉ��̃v���V�[�W���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B  
    ' �R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'btnButton13
        '
        Me.btnButton13.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton12
        '
        Me.btnButton12.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton11
        '
        Me.btnButton11.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton10
        '
        Me.btnButton10.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton9
        '
        Me.btnButton9.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton8
        '
        Me.btnButton8.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton7
        '
        Me.btnButton7.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton6
        '
        Me.btnButton6.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton5
        '
        Me.btnButton5.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton4
        '
        Me.btnButton4.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton3
        '
        Me.btnButton3.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton2
        '
        Me.btnButton2.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton1
        '
        Me.btnButton1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblToday.Text = "2013/04/15(��)  17:09"
        '
        'FrmPrgMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgMenu"
        Me.Text = " "
        Me.ResumeLayout(False)

    End Sub

#End Region

    '�t�H�[�����[�h
    Private Sub FrmPrgMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '��ʃ^�C�g��
        lblTitle.Text = "�v���O�����Ǘ����j���["

        '�{�^�����̂�ݒ肷��
        btnButton1.Text = "�O���}�̎捞"
        btnButton2.Text = "�v���O�����K�p���X�g�捞"

        '�{�^����\��
        btnButton3.Visible = False
        btnButton4.Visible = False
        btnButton5.Visible = False
        btnButton6.Visible = False
        btnButton7.Visible = False

        '�{�^�����̂�ݒ肷��
        btnButton8.Text = "�z�M�w���ݒ�"
        btnButton9.Text = "�z�M�󋵕\��"
        btnButton10.Text = "�o�[�W�����\��"

        '�{�^����\��
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
        If (FrmBase.Authority = "4") Then
            For a As Integer = 5 To 9
                If (FrmBase.DetailSet(a).ToString = "0") Then
                    If (a = 5) Then
                        btnButton1.Enabled = False
                    ElseIf (a = 6) Then
                        btnButton2.Enabled = False
                    ElseIf (a = 7) Then
                        btnButton8.Enabled = False
                    ElseIf (a = 8) Then
                        btnButton9.Enabled = False
                    ElseIf (a = 9) Then
                        btnButton10.Enabled = False
                    End If
                End If
            Next
        End If
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
        btnButton11.Visible = False
        btnButton12.Visible = False
        btnButton13.Visible = False

        '�{�^������(�߁@��)��ݒ肷��
        btnReturn.Text = "�߁@��"

    End Sub

    '�u�O���}�̎捞�v�{�^���N���b�N
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton1.Click

        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim oFrmPrgInputData As New FrmPrgInputData

        Me.Hide()
        oFrmPrgInputData.ShowDialog()
        oFrmPrgInputData.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u�z�M�ݒ�w���v�{�^���N���b�N
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton8.Click

        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim oFrmPrgOrderDelivery As New FrmPrgOrderDelivery

        If oFrmPrgOrderDelivery.InitFrmData = False Then
            oFrmPrgOrderDelivery = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmPrgOrderDelivery.ShowDialog()
        oFrmPrgOrderDelivery.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u�z�M�󋵕\���v�{�^���N���b�N
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton9.Click

        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim oFrmPrgDispDelivery As New FrmPrgDispDelivery

        If oFrmPrgDispDelivery.InitFrmData() = False Then
            oFrmPrgDispDelivery = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmPrgDispDelivery.ShowDialog()
        oFrmPrgDispDelivery.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u�o�[�W�����\���v�{�^���N���b�N
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton10.Click

        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim oFrmPrgDispVersion As New FrmPrgDispVersion

        If oFrmPrgDispVersion.InitFrmData() = False Then
            oFrmPrgDispVersion = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmPrgDispVersion.ShowDialog()
        oFrmPrgDispVersion.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub
    '�u�I���v�{�^���N���b�N
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        LogOperation(sender, e)
        Me.Close()

    End Sub

    '�u�v���O�����K�p���X�g�捞�v�{�^���N���b�N
    Private Sub btnButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton2.Click

        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim oFrmPrgInputList As New FrmPrgInputList

        Me.Hide()
        oFrmPrgInputList.ShowDialog()
        oFrmPrgInputList.Dispose()
        Me.Show()
        Call waitCursor(False)
    End Sub
End Class
