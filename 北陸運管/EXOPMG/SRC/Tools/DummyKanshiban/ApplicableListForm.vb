' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/02/16  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Public Class ApplicableListForm

    Private FormKey As String
    Private ManagerForm As MainForm

    Public Sub New(ByVal sMachineId As String, ByVal sDataKind As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal listVersion As Integer, ByVal listAcceptDate As DateTime, ByVal sListHashValue As String, ByVal sListContent As String, ByVal sFormKey As String, ByVal oManagerForm As MainForm)
        InitializeComponent()

        Me.FormKey = sFormKey
        Me.ManagerForm = oManagerForm
        Me.MachineIdTextBox.Text = sMachineId
        Me.DataKindTextBox.Text = sDataKind
        Me.DataSubKindTextBox.Text = dataSubKind.ToString()
        Me.DataVersionTextBox.Text = dataVersion.ToString()
        Me.ListVersionTextBox.Text = listVersion.ToString()
        Me.ListAcceptDateTextBox.Text = listAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff")
        If listAcceptDate = Config.EmptyTime Then
            Me.ListAcceptDateTextBox.Text = Lexis.EmptyTime.Gen()
        ElseIf listAcceptDate = Config.UnknownTime Then
            Me.ListAcceptDateTextBox.Text = Lexis.UnknownTime.Gen()
        Else
            Me.ListAcceptDateTextBox.Text = listAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff")
        End If
        Me.ListHashValueTextBox.Text = sListHashValue
        Me.ListContentTextBox.Text = sListContent

        If sDataKind = "GPG" Then
            Me.Text = "���D�@�v���O�����K�p���X�g"
            Me.DataSubKindLabel.Text = "�G���ANo"
            Me.DataVersionLabel.Text = "��\Ver"
        ElseIf sDataKind = "WPG" Then
            Me.Text = "�Ď��Ճv���O�����K�p���X�g"
            Me.DataSubKindLabel.Text = "�G���ANo"
            Me.DataVersionLabel.Text = "��\Ver"
        Else
            Me.Text = sDataKind & "�}�X�^�K�p���X�g"
            Me.DataSubKindLabel.Text = "�p�^�[��No"
            Me.DataVersionLabel.Text = "�}�X�^Ver"
        End If
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        ManagerForm.MasProListFormDic.Remove(FormKey)
        MyBase.OnFormClosed(e)
    End Sub

End Class
