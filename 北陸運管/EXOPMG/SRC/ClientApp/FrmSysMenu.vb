' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2013/11/11  (NES)����  �t�F�[�Y�Q�����Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>�V�X�e���Ǘ����j���[</summary>
''' <remarks></remarks>
Public Class FrmSysMenu
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
        'btnButton13
        '
        Me.btnButton13.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnReturn.TabIndex = 7
        '
        'btnButton12
        '
        Me.btnButton12.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton12.TabIndex = 5
        '
        'btnButton11
        '
        Me.btnButton11.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton11.TabIndex = 5
        '
        'btnButton10
        '
        Me.btnButton10.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton10.TabIndex = 4
        '
        'btnButton9
        '
        Me.btnButton9.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton9.Size = New System.Drawing.Size(369, 48)
        Me.btnButton9.TabIndex = 3
        '
        'btnButton8
        '
        Me.btnButton8.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton8.TabIndex = 2
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
        Me.btnButton2.TabIndex = 1
        '
        'btnButton1
        '
        Me.btnButton1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton1.TabIndex = 1
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.Black
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblToday.Text = "2013/03/27(��)  09:08"
        '
        'FrmSysMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysMenu"
        Me.Text = " "
        Me.ResumeLayout(False)

    End Sub

#End Region
#Region "�錾�̈�iPrivate�j"
    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly FormTitle As String = "�V�X�e���Ǘ����j���["
    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean
#End Region
    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    Private Sub FrmSysMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LfWaitCursor()
        Dim bRtn As Boolean = False
        LbEventStop = True      '�C�x���g�����n�e�e
        Try
            Log.Info("Method started.")

            '��ʃ^�C�g��
            lblTitle.Text = FormTitle
            '�{�^�����̂�ݒ肷��
            btnButton1.Text = "�h�c�}�X�^�ݒ�"
            '�{�^����\��
            btnButton2.Visible = False
            btnButton3.Visible = False
            btnButton4.Visible = False
            btnButton5.Visible = False
            btnButton6.Visible = False
            btnButton7.Visible = False
            '�{�^�����̂�ݒ肷��
            btnButton8.Text = "�ғ��E�ێ�f�[�^�ݒ�"
            btnButton9.Text = "�p�^�[���ݒ�"
            btnButton10.Text = "�G���A�ݒ�"
            btnButton11.Text = "�^�ǐݒ�Ǘ�"
            '�{�^����\��
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
            If (FrmBase.Authority = "4") Then
                For a As Integer = 20 To 24
                    If (FrmBase.DetailSet(a).ToString = "0") Then
                        If (a = 20) Then
                            btnButton1.Enabled = False
                        ElseIf (a = 21) Then
                            btnButton8.Enabled = False
                        ElseIf (a = 22) Then
                            btnButton9.Enabled = False
                        ElseIf (a = 23) Then
                            btnButton10.Enabled = False
                        ElseIf (a = 24) Then
                            btnButton11.Enabled = False
                        End If
                    End If
                Next
            End If
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
            btnButton12.Visible = False
            btnButton13.Visible = False
            '�{�^�����̂�ݒ肷��
            btnReturn.Text = "�߁@��"
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
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
                Me.Close()
            End If
            LbEventStop = False '�C�x���g�����n�m
            LfWaitCursor(False)
        End Try
    End Sub

    '�u�h�c�}�X�^�ݒ�v�{�^���N���b�N��
    Private Sub btnButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton1.Click

        Call waitCursor(True)
        '�u�h�c�}�X�^�ݒ�v�{�^�������B
        LogOperation(sender, e)
        Dim oFrmSysIDMst As New FrmSysIDMst

        If oFrmSysIDMst.InitFrmData() = False Then
            oFrmSysIDMst = Nothing
            Call waitCursor(False)
            Exit Sub
        End If
        Me.Hide()
        '�u�h�c�}�X�^�ݒ�v��ʂ֑J�ڂ���B
        oFrmSysIDMst.ShowDialog()
        oFrmSysIDMst.Dispose()
        Me.Show()
        Call waitCursor(False)
    End Sub

    ''' <summary>
    ''' �u�ғ��E�ێ�f�[�^�ݒ�v
    ''' </summary>
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton8.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrm As New FrmSysKadoDataMst
        If hFrm.InitFrm = False Then
            LfWaitCursor(False)
            hFrm.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        '�ғ��E�ێ�f�[�^�ݒ��ʂ֑J�ڂ���B
        hFrm.ShowDialog()
        hFrm.Dispose()
        Me.Show()
    End Sub

    '�u�p�^�[���ݒ�v�{�^���N���b�N��
    Private Sub btnButton9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton9.Click

        Call waitCursor(True)
        '�u�p�^�[���ݒ�v�{�^�������B
        LogOperation(sender, e)
        Dim oFrmSysPatternMst As New FrmSysPatternMst

        If oFrmSysPatternMst.InitFrm() = False Then
            oFrmSysPatternMst = Nothing
            Call waitCursor(False)
            Exit Sub
        End If
        Me.Hide()
        '�p�^�[���ݒ��ʂ֑J�ڂ���B
        oFrmSysPatternMst.ShowDialog()
        oFrmSysPatternMst.Dispose()
        Me.Show()
        Call waitCursor(False)


    End Sub

    '�u�G���A�ݒ�v�{�^���N���b�N��
    Private Sub btnButton10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton10.Click
        Call waitCursor(True)
        '�u�G���A�ݒ�v�{�^�������B
        LogOperation(sender, e)
        Dim oFrmSysAreaMst As New FrmSysAreaMst

        If oFrmSysAreaMst.InitFrmData() = False Then
            oFrmSysAreaMst = Nothing
            Call waitCursor(False)
            Exit Sub
        End If
        Me.Hide()
        '�u�G���A�ݒ�v��ʂ֑J�ڂ���B
        oFrmSysAreaMst.ShowDialog()
        oFrmSysAreaMst.Dispose()
        Me.Show()
        Call waitCursor(False)
    End Sub

    '�u �^�ǐݒ�Ǘ� �v�{�^���N���b�N��
    Private Sub btnButton11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton11.Click
        Call waitCursor(True)
        '�u�^�ǐݒ�Ǘ��v�{�^�������B
        LogOperation(sender, e)
        Dim oFrmSysUnKanSetMng As New FrmSysUnKanSetMng

        If oFrmSysUnKanSetMng.InitFrm() = False Then
            oFrmSysUnKanSetMng = Nothing
            Call waitCursor(False)
            Exit Sub
        End If
        Me.Hide()
        '�u�^�ǐݒ�Ǘ��v��ʂ֑J�ڂ���B
        oFrmSysUnKanSetMng.ShowDialog()
        oFrmSysUnKanSetMng.Dispose()
        Me.Show()
        Call waitCursor(False)
    End Sub
    '�u��  ��v�{�^���N���b�N��
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '�u��  ��v�{�^�������B
        LogOperation(sender, e)
        Me.Close()

    End Sub
End Class
