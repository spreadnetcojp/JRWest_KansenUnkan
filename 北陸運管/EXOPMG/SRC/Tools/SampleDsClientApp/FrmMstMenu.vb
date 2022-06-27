' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇  新規作成
'   0.1      2013/05/13  (NES)小林  デ集クライアント試供アプリ化
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

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
        lblTitle.Text = "マスタ配信アプリ"

        'ボタン名称
        btnButton1.Text = "マスタ登録"
        btnButton2.Text = "マスタ適用リスト登録"
        btnButton8.Text = "配信指示"

        'ボタン非表示
        btnButton3.Visible = False
        btnButton4.Visible = False
        btnButton5.Visible = False
        btnButton6.Visible = False
        btnButton7.Visible = False
        btnButton9.Visible = False
        btnButton10.Visible = False
        btnButton11.Visible = False
        btnButton12.Visible = False
        btnButton13.Visible = False

        'ボタン名称
        btnReturn.Text = "終　了"

    End Sub

    '「マスタ登録」ボタンクリック
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

    '「マスタ適用リスト登録」ボタンクリック
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

    '「配信指示」ボタンクリック
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton8.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmMstOrderDelivery As New FrmMstOrderDelivery

        Me.Hide()
        oFrmMstOrderDelivery.ShowDialog()
        oFrmMstOrderDelivery.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「終　了」ボタンクリック
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()

    End Sub

End Class
