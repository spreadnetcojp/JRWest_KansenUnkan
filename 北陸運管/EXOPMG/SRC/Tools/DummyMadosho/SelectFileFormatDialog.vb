' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/12/08  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class SelectFileFormatDialog

    Public Property Format As Integer
        Get
            Return If(RadioButton1.Checked, 0, 1)
        End Get
        Set(ByVal value As Integer)
            If value = 0 Then
                RadioButton1.Checked = True
            Else
                RadioButton2.Checked = True
            End If
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

    Public Property Format0Text As String
        Get
            Return RadioButton1.Text
        End Get
        Set(ByVal value As String)
            RadioButton1.Text = value
        End Set
    End Property

    Public Property Format1Text As String
        Get
            Return RadioButton2.Text
        End Get
        Set(ByVal value As String)
            RadioButton2.Text = value
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
