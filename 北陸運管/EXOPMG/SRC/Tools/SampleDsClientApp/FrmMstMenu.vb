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

''' <summary>�}�X�^�Ǘ����j���[</summary>
''' <remarks></remarks>
Public Class FrmMstMenu
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

    ' ���� : �ȉ��̃v���V�[�W���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B  
    ' �R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'btnReturn
        '
        '
        'btnButton12
        '
        '
        'btnButton11
        '
        '
        'btnButton10
        '
        '
        'btnButton9
        '
        '
        'btnButton8
        '
        '
        'btnButton1
        '
        '
        'lblToday
        '
        Me.lblToday.Text = "2011/07/20(��)  12:57"
        '
        'FrmMstMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMstMenu"
        Me.Text = "�^�p�[�� "
        Me.ResumeLayout(False)

    End Sub

#End Region

    '�t�H�[�����[�h
    Private Sub FrmMstMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '��ʃ^�C�g��
        lblTitle.Text = "�}�X�^�z�M�A�v��"

        '�{�^������
        btnButton1.Text = "�}�X�^�o�^"
        btnButton2.Text = "�}�X�^�K�p���X�g�o�^"
        btnButton8.Text = "�z�M�w��"

        '�{�^����\��
        btnButton3.Visible = False
        btnButton4.Visible = False
        btnButton5.Visible = False
        btnButton6.Visible = False
        btnButton7.Visible = False
        btnButton9.Visible = False
        btnButton10.Visible = False
        btnButton11.Visible = False
        btnButton12.Visible = False
        btnButton13.Visible = False

        '�{�^������
        btnReturn.Text = "�I�@��"

    End Sub

    '�u�}�X�^�o�^�v�{�^���N���b�N
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton1.Click

        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim oFrmMstInputData As New FrmMstInputData

        Me.Hide()
        oFrmMstInputData.ShowDialog()
        oFrmMstInputData.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u�}�X�^�K�p���X�g�o�^�v�{�^���N���b�N
    Private Sub btnButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton2.Click

        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim oFrmMstInputList As New FrmMstInputList

        Me.Hide()
        oFrmMstInputList.ShowDialog()
        oFrmMstInputList.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u�z�M�w���v�{�^���N���b�N
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton8.Click

        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim oFrmMstOrderDelivery As New FrmMstOrderDelivery

        Me.Hide()
        oFrmMstOrderDelivery.ShowDialog()
        oFrmMstOrderDelivery.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '�u�I�@���v�{�^���N���b�N
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        LogOperation(sender, e)    '�{�^���������O
        Me.Close()

    End Sub

End Class
