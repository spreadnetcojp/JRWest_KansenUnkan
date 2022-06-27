' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2014/04/20  (NES)      �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' �ێ�Ǘ����j���[
''' </summary>
Public Class FrmMaintenanceToolMenu

    Public Sub New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

    End Sub

    ''' <summary>
    ''' �t�H�[�����[�h
    ''' </summary>
    Private Sub FrmMaintenanceToolMenu_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim bRtn As Boolean = False
        Me.Cursor = Cursors.WaitCursor
        Try
            Log.Info("Method started.")

            '�E�B���h�E�^�C�g����ݒ肷��
            Me.Text = Config.MachineKind & " Ver" & Config.VerNoSet

            '�{�^�����̂�ݒ肷��
            Me.btnButton1.Text = "�x�~���@�ݒ�"
            Me.btnButton2.Text = "���[�����M�ΏۃG���[�R�[�h�ݒ�"
            Me.btnButton3.Text = ""
            Me.btnButton4.Text = ""

            '�{�^����\��
            Me.btnButton1.Visible = True
            Me.btnButton2.Visible = True
            Me.btnButton3.Visible = False
            Me.btnButton4.Visible = False

            '�{�^������(����)��ݒ肷��
            Me.btnReturn.Text = "����"
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
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' �u�x�~���@�ݒ�v
    ''' </summary>
    Private Sub btnButton1_Click(sender As System.Object, e As System.EventArgs) Handles btnButton1.Click
        Me.Cursor = Cursors.WaitCursor
        Dim hFrmRestingMachine As New FrmRestingMachine()
        Me.Cursor = Cursors.Default
        Me.Hide()
        hFrmRestingMachine.ShowDialog()
        hFrmRestingMachine.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u���[�����M�ΏۃG���[�R�[�h�ݒ�v
    ''' </summary>
    Private Sub btnButton2_Click(sender As System.Object, e As System.EventArgs) Handles btnButton2.Click
        Me.Cursor = Cursors.WaitCursor
        Dim hFrmNotifiableErrCode As New FrmNotifiableErrCode()
        Me.Cursor = Cursors.Default
        Me.Hide()
        hFrmNotifiableErrCode.ShowDialog()
        hFrmNotifiableErrCode.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' �u�߂�v
    ''' </summary>
    Private Sub btnReturn_Click(sender As System.Object, e As System.EventArgs) Handles btnReturn.Click
        Me.Close()
    End Sub

End Class
