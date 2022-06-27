' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/02/16  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text

Imports JR.ExOpmg.Common

Public Class GateMasDataForm

    Private Shared ReadOnly FooterFieldNames As String() = { _
        "機種名", _
        "データ名", _
        "作成日時", _
        "バージョン", _
        "表示名称", _
        "サム範囲長", _
        "サム値"}

    Private Shared ReadOnly FooterFieldNamesTypes As New Dictionary(Of String, Type) From { _
        {"機種名", GetType(String)}, _
        {"データ名", GetType(String)}, _
        {"作成日時", GetType(String)}, _
        {"バージョン", GetType(String)}, _
        {"表示名称", GetType(String)}, _
        {"サム範囲長", GetType(UInteger)}, _
        {"サム値", GetType(UInteger)}}

    Private Shared ReadOnly FooterFieldNamesCanonicalValues As New Dictionary(Of String, String) From { _
        {"機種名", "WWWWWWWW..."}, _
        {"データ名", "WWWWWWWW..."}, _
        {"作成日時", "9999/99/99 99:99."}, _
        {"バージョン", "バージョン."}, _
        {"表示名称", "○○○○○○○○○○○○○○○○○○○○○○○○○○○○○○○○"}, _
        {"サム範囲長", "9999999999"}, _
        {"サム値", "9999999999"}}

    Private FormKey As String
    Private ManagerForm As MainForm

    Public Sub New(ByVal sMachineId As String, ByVal sDataKind As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal dataAcceptDate As DateTime, ByVal sDataHashValue As String, ByVal oFooter As Byte(), ByVal sFormKey As String, ByVal oManagerForm As MainForm)
        InitializeComponent()

        Me.Text = sDataKind & "マスタ"
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
            oRow("機種名") = footerView.ApplicableSpecificModel
            oRow("データ名") = footerView.PrefixedKind
            oRow("作成日時") = footerView.CreatedTime.ToString("yyyy/MM/dd HH:mm")
            oRow("バージョン") = footerView.Version
            oRow("表示名称") = footerView.DispData.TrimEnd()
            oRow("サム範囲長") = footerView.SumCheckLength
            oRow("サム値") = footerView.SumValue
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
