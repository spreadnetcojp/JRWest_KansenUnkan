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

Public Class SelectDateTimeDialog

    Public Property Format As String
        Get
            Return Picker.CustomFormat
        End Get
        Set(ByVal value As String)
             Picker.CustomFormat = value
        End Set
    End Property

    Public Property DateTime As DateTime
        Get
            Return Picker.Value
        End Get
        Set(ByVal value As DateTime)
            Picker.Value = value
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
