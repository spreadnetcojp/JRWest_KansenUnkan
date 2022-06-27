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
            Me.Text = "改札機プログラム適用リスト"
            Me.DataSubKindLabel.Text = "エリアNo"
            Me.DataVersionLabel.Text = "代表Ver"
        ElseIf sDataKind = "WPG" Then
            Me.Text = "監視盤プログラム適用リスト"
            Me.DataSubKindLabel.Text = "エリアNo"
            Me.DataVersionLabel.Text = "代表Ver"
        Else
            Me.Text = sDataKind & "マスタ適用リスト"
            Me.DataSubKindLabel.Text = "パターンNo"
            Me.DataVersionLabel.Text = "マスタVer"
        End If
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        ManagerForm.MasProListFormDic.Remove(FormKey)
        MyBase.OnFormClosed(e)
    End Sub

End Class
