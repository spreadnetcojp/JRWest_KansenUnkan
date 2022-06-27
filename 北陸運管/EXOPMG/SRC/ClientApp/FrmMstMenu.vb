' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
'   0.1      2013/11/11  (NES)金沢  フェーズ２権限対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>マスタ管理メニュー</summary>
''' <remarks></remarks>
Public Class FrmMstMenu
    Inherits FrmBaseMenu

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

    End Sub

    ' Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使って変更してください。  
    ' コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'btnReturn
        '
        '
        'btnButton12
        '
        '
        'btnButton11
        '
        '
        'btnButton10
        '
        '
        'btnButton9
        '
        '
        'btnButton8
        '
        '
        'btnButton1
        '
        '
        'lblToday
        '
        Me.lblToday.Text = "2011/07/20(水)  12:57"
        '
        'FrmMstMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMstMenu"
        Me.Text = "運用端末 "
        Me.ResumeLayout(False)

    End Sub

#End Region

    'フォームロード
    Private Sub FrmMstMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '画面タイトル
        lblTitle.Text = "マスタ管理メニュー"

        'ボタン名称
        btnButton1.Text = "外部媒体取込"
        btnButton2.Text = "マスタ適用リスト取込"
        btnButton8.Text = "配信指示設定"
        btnButton9.Text = "配信状況表示"
        btnButton10.Text = "バージョン表示"

        'ボタン非表示
        '-------Ver0.1　フェーズ２権限対応　ADD START-----------
        If (FrmBase.Authority = "4") Then
            For a As Integer = 0 To 4
                If (FrmBase.DetailSet(a).ToString = "0") Then
                    If (a = 0) Then
                        btnButton1.Enabled = False
                    ElseIf (a = 1) Then
                        btnButton2.Enabled = False
                    ElseIf (a = 2) Then
                        btnButton8.Enabled = False
                    ElseIf (a = 3) Then
                        btnButton9.Enabled = False
                    ElseIf (a = 4) Then
                        btnButton10.Enabled = False
                    End If
                End If
            Next
        End If
        '-------Ver0.1　フェーズ２権限対応　ADD END-------------
        btnButton3.Visible = False
        btnButton4.Visible = False
        btnButton5.Visible = False
        btnButton6.Visible = False
        btnButton7.Visible = False
        btnButton11.Visible = False
        btnButton12.Visible = False
        btnButton13.Visible = False

        'ボタン名称(戻　る)を設定する
        btnReturn.Text = "戻　る"

    End Sub

    '「外部媒体取込」ボタンクリック
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton1.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmMstInputData As New FrmMstInputData

        Me.Hide()
        oFrmMstInputData.ShowDialog()
        oFrmMstInputData.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「マスタ適用リスト取込」ボタンクリック
    Private Sub btnButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton2.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmMstInputList As New FrmMstInputList

        Me.Hide()
        oFrmMstInputList.ShowDialog()
        oFrmMstInputList.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「配信指示設定」ボタンクリック
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton8.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmMstOrderDelivery As New FrmMstOrderDelivery

        If oFrmMstOrderDelivery.InitFrmData = False Then
            oFrmMstOrderDelivery = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmMstOrderDelivery.ShowDialog()
        oFrmMstOrderDelivery.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「配信状況表示」ボタンクリック
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton9.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmMstDispDelivery As New FrmMstDispDelivery

        If oFrmMstDispDelivery.InitFrmData = False Then
            oFrmMstDispDelivery = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmMstDispDelivery.ShowDialog()
        oFrmMstDispDelivery.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「バージョン表示」ボタンクリック
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton10.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmMstDispVersion As New FrmMstDispVersion

        If oFrmMstDispVersion.InitFrmData = False Then
            oFrmMstDispVersion = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmMstDispVersion.ShowDialog()
        oFrmMstDispVersion.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「戻　る」ボタンクリック
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()

    End Sub

End Class
