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

''' <summary>
''' �ێ�Ǘ����j���[
''' </summary>
Public Class FrmMntMenu
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
        Me.lblToday.Text = "2013/02/20(��)  19:56"
        '
        'FrmMntMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntMenu"
        Me.Text = "�^�p�[�� "
        Me.ResumeLayout(False)

    End Sub

#End Region
#Region "�錾�̈�iPrivate�j"
    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean
#End Region
#Region "�C�x���g"

    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    Private Sub FrmMntMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim bRtn As Boolean = False
        LbEventStop = True      '�C�x���g�����n�e�e
        LfWaitCursor()
        Try
            Log.Info("Method started.")

            '��ʃ^�C�g��
            lblTitle.Text = "�ێ�Ǘ����j���["

            '�{�^�����̂�ݒ肷��
            btnButton1.Text = "�ʏW�D�f�[�^�m�F"
            btnButton2.Text = "�s����Ԍ��o�f�[�^�m�F"
            btnButton3.Text = "���s�˔j���o�f�[�^�m�F"
            btnButton4.Text = "���������o�f�[�^�m�F"
            btnButton5.Text = "�ُ�f�[�^�m�F"
            btnButton6.Text = "�ғ��E�ێ�f�[�^�o��"
            btnButton8.Text = "�@��ڑ���Ԋm�F"
            btnButton9.Text = "�Ď��Րݒ���"
            btnButton10.Text = "���W�f�[�^�m�F"
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
            If (FrmBase.Authority = "4") Then
                For a As Integer = 10 To 19
                    If (FrmBase.DetailSet(a).ToString = "0") Then
                        If (a = 10) Then
                            btnButton1.Enabled = False
                        ElseIf (a = 11) Then
                            btnButton2.Enabled = False
                        ElseIf (a = 12) Then
                            btnButton3.Enabled = False
                        ElseIf (a = 13) Then
                            btnButton4.Enabled = False
                        ElseIf (a = 14) Then
                            btnButton5.Enabled = False
                        ElseIf (a = 15) Then
                            btnButton6.Enabled = False
                        ElseIf (a = 16) Then
                            btnButton8.Enabled = False
                        ElseIf (a = 17) Then
                            btnButton9.Enabled = False
                        ElseIf (a = 18) Then
                            btnButton10.Enabled = False
                        ElseIf (a = 19) Then
                            btnButton11.Enabled = False
                        End If
                    End If
                Next
            End If
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------
            If Config.SelfCompany = EkCompany.JRWest Then btnButton11.Text = "���ԑѕʏ�~�f�[�^�o��"

            '�{�^����\��
            btnButton7.Visible = False
            If Not (Config.SelfCompany = EkCompany.JRWest) Then btnButton11.Visible = False
            btnButton12.Visible = False
            btnButton13.Visible = False

            '�{�^������(�߁@��)��ݒ肷��
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

    '//////////////////////////////////////////////�{�^���N���b�N
    ''' <summary>
    ''' �u�ʏW�D�f�[�^�m�F�v
    ''' </summary>
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton1.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hfrmMntDispBesshuData As New FrmMntDispBesshuData
        If hfrmMntDispBesshuData.InitFrm = False Then
            LfWaitCursor(False)
            hfrmMntDispBesshuData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hfrmMntDispBesshuData.ShowDialog()
        hfrmMntDispBesshuData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u�s����Ԍ��o�f�[�^�m�F�v
    ''' </summary>
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton2.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispFuseiJoshaData As New FrmMntDispFuseiJoshaData
        If hFrmMntDispFuseiJoshaData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispFuseiJoshaData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispFuseiJoshaData.ShowDialog()
        hFrmMntDispFuseiJoshaData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u���s�˔j���o�f�[�^�m�F�v
    ''' </summary>
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton3.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispKyokoToppaData As New FrmMntDispKyokoToppaData
        If hFrmMntDispKyokoToppaData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispKyokoToppaData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispKyokoToppaData.ShowDialog()
        hFrmMntDispKyokoToppaData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u���������o�f�[�^�m�F�v
    ''' </summary>
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton4.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispFunshitsuData As New FrmMntDispFunshitsuData
        If hFrmMntDispFunshitsuData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispFunshitsuData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispFunshitsuData.ShowDialog()
        hFrmMntDispFunshitsuData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u�ُ�f�[�^�m�F�v
    ''' </summary>
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton5.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispFaultData As New FrmMntDispFaultData
        If hFrmMntDispFaultData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispFaultData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispFaultData.ShowDialog()
        hFrmMntDispFaultData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u�ғ��E�ێ�f�[�^�o�́v
    ''' </summary>
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton6.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispKadoData As New FrmMntDispKadoData
        If hFrmMntDispKadoData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispKadoData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispKadoData.ShowDialog()
        hFrmMntDispKadoData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u�@��ڑ���Ԋm�F�v
    ''' </summary>
    Private Sub btnButton8_Click(sender As System.Object, e As System.EventArgs) Handles btnButton8.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispConStatus As New FrmMntDispConStatus
        If hFrmMntDispConStatus.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispConStatus.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispConStatus.ShowDialog()
        hFrmMntDispConStatus.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u�Ď��Րݒ���v
    ''' </summary>
    Private Sub btnButton9_Click(sender As System.Object, e As System.EventArgs) Handles btnButton9.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispKsbConfig As New FrmMntDispKsbConfig
        If hFrmMntDispKsbConfig.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispKsbConfig.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispKsbConfig.ShowDialog()
        hFrmMntDispKsbConfig.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u���W�f�[�^�m�F�v
    ''' </summary>
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton10.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispCollectedData As New FrmMntDispCollectedData
        If hFrmMntDispCollectedData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispCollectedData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispCollectedData.ShowDialog()
        hFrmMntDispCollectedData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u���ԑѕʏ�~�f�[�^�o�́v
    ''' </summary>
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton11.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispTrafficData As New FrmMntDispTrafficData
        If hFrmMntDispTrafficData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispTrafficData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispTrafficData.ShowDialog()
        hFrmMntDispTrafficData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u�߂�v
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        LogOperation(sender, e)
        Me.Close()
    End Sub

#End Region

End Class
