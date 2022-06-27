' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/06/10  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class SelectRecordDialog
    Private dispOrigin As Decimal

    Public Sub New(Optional ByVal zeroRelativeDisplay As Boolean = False)
        InitializeComponent()

        If zeroRelativeDisplay Then
            dispOrigin = Decimal.Zero
        Else
            dispOrigin = Decimal.One
        End If
        IndexUpDown.Minimum = dispOrigin
    End Sub

    Public Property MaxIndex As Decimal
        Get
            Return IndexUpDown.Maximum - dispOrigin
        End Get
        Set(ByVal value As Decimal)
            IndexUpDown.Maximum = value + dispOrigin
        End Set
    End Property

    Public Property Index As Decimal
        Get
            Return IndexUpDown.Value - dispOrigin
        End Get
        Set(ByVal value As Decimal)
            IndexUpDown.Value = value + dispOrigin
        End Set
    End Property

    Public Property Description As String
        Get
            Return DescriptionLabel.Text
        End Get
        Set(ByVal value As String)
            DescriptionLabel.Text = value
        End Set
    End Property

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

End Class
