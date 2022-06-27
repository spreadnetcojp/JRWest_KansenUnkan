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

Imports System.Text

Imports JR.ExOpmg.Common

Public Class GateMasDataForm

    Private Shared ReadOnly FooterFieldNames As String() = { _
        "�@�햼", _
        "�f�[�^��", _
        "�쐬����", _
        "�o�[�W����", _
        "�\������", _
        "�T���͈͒�", _
        "�T���l"}

    Private Shared ReadOnly FooterFieldNamesTypes As New Dictionary(Of String, Type) From { _
        {"�@�햼", GetType(String)}, _
        {"�f�[�^��", GetType(String)}, _
        {"�쐬����", GetType(String)}, _
        {"�o�[�W����", GetType(String)}, _
        {"�\������", GetType(String)}, _
        {"�T���͈͒�", GetType(UInteger)}, _
        {"�T���l", GetType(UInteger)}}

    Private Shared ReadOnly FooterFieldNamesCanonicalValues As New Dictionary(Of String, String) From { _
        {"�@�햼", "WWWWWWWW..."}, _
        {"�f�[�^��", "WWWWWWWW..."}, _
        {"�쐬����", "9999/99/99 99:99."}, _
        {"�o�[�W����", "�o�[�W����."}, _
        {"�\������", "����������������������������������������������������������������"}, _
        {"�T���͈͒�", "9999999999"}, _
        {"�T���l", "9999999999"}}

    Private FormKey As String
    Private ManagerForm As MainForm

    Public Sub New(ByVal sMachineId As String, ByVal sDataKind As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal dataAcceptDate As DateTime, ByVal sDataHashValue As String, ByVal oFooter As Byte(), ByVal sFormKey As String, ByVal oManagerForm As MainForm)
        InitializeComponent()

        Me.Text = sDataKind & "�}�X�^"
        Me.FormKey = sFormKey
        Me.ManagerForm = oManagerForm
        Me.MachineIdTextBox.Text = sMachineId
        Me.DataKindTextBox.Text = sDataKind
        Me.DataSubKindTextBox.Text = dataSubKind.ToString()
        Me.DataVersionTextBox.Text = dataVersion.ToString()
        If dataAcceptDate = Config.EmptyTime Then
            Me.DataAcceptDateTextBox.Text = Lexis.EmptyTime.Gen()
        ElseIf dataAcceptDate = Config.UnknownTime Then
            Me.DataAcceptDateTextBox.Text = Lexis.UnknownTime.Gen()
        Else
            Me.DataAcceptDateTextBox.Text = dataAcceptDate.ToString("yyyy/MM/dd HH:mm:ss.fff")
        End If
        Me.DataHashValueTextBox.Text = sDataHashValue

        With Nothing
            Dim oTable As New DataTable()
            For i As Integer = 0 To FooterFieldNames.Length - 1
                oTable.Columns.Add(FooterFieldNames(i), FooterFieldNamesTypes(FooterFieldNames(i)))
            Next i

            Dim oRow As DataRow = oTable.NewRow()
            Dim footerView As New ExMasterDataFooter(oFooter)
            oRow("�@�햼") = footerView.ApplicableSpecificModel
            oRow("�f�[�^��") = footerView.PrefixedKind
            oRow("�쐬����") = footerView.CreatedTime.ToString("yyyy/MM/dd HH:mm")
            oRow("�o�[�W����") = footerView.Version
            oRow("�\������") = footerView.DispData.TrimEnd()
            oRow("�T���͈͒�") = footerView.SumCheckLength
            oRow("�T���l") = footerView.SumValue
            oTable.Rows.Add(oRow)

            FooterDataGridView.AutoGenerateColumns = True
            FooterDataGridView.DataSource = oTable
            FooterDataGridView.AutoGenerateColumns = False
            For i As Integer = 0 To FooterFieldNames.Length - 1
                FooterDataGridView.Columns(i).Width = MyUtility.GetTextWidth(FooterFieldNamesCanonicalValues(FooterFieldNames(i)), FooterDataGridView.Columns(i).InheritedStyle.Font)
                If FooterFieldNamesTypes(FooterFieldNames(i)) IsNot GetType(String) Then
                    FooterDataGridView.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Next i
        End With
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        ManagerForm.MasProDataFormDic.Remove(FormKey)
        MyBase.OnFormClosed(e)
    End Sub

End Class
