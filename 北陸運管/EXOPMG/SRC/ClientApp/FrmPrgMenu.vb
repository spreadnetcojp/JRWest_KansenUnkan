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

''' <summary>プログラム管理メニュー</summary>
''' <remarks></remarks>

Public Class FrmPrgMenu
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

    'TODO: 以下、できれば、BackColorプロパティではなく、元のようにNameプロパティをセットするコードにしたい。
    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使って変更してください。  
    ' コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'btnButton13
        '
        Me.btnButton13.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton12
        '
        Me.btnButton12.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton11
        '
        Me.btnButton11.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton10
        '
        Me.btnButton10.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton9
        '
        Me.btnButton9.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton8
        '
        Me.btnButton8.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton7
        '
        Me.btnButton7.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton6
        '
        Me.btnButton6.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton5
        '
        Me.btnButton5.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton4
        '
        Me.btnButton4.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton3
        '
        Me.btnButton3.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton2
        '
        Me.btnButton2.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnButton1
        '
        Me.btnButton1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblToday.Text = "2013/04/15(月)  17:09"
        '
        'FrmPrgMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgMenu"
        Me.Text = " "
        Me.ResumeLayout(False)

    End Sub

#End Region

    'フォームロード
    Private Sub FrmPrgMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '画面タイトル
        lblTitle.Text = "プログラム管理メニュー"

        'ボタン名称を設定する
        btnButton1.Text = "外部媒体取込"
        btnButton2.Text = "プログラム適用リスト取込"

        'ボタン非表示
        btnButton3.Visible = False
        btnButton4.Visible = False
        btnButton5.Visible = False
        btnButton6.Visible = False
        btnButton7.Visible = False

        'ボタン名称を設定する
        btnButton8.Text = "配信指示設定"
        btnButton9.Text = "配信状況表示"
        btnButton10.Text = "バージョン表示"

        'ボタン非表示
        '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
        If (FrmBase.Authority = "4") Then
            For a As Integer = 5 To 9
                If (FrmBase.DetailSet(a).ToString = "0") Then
                    If (a = 5) Then
                        btnButton1.Enabled = False
                    ElseIf (a = 6) Then
                        btnButton2.Enabled = False
                    ElseIf (a = 7) Then
                        btnButton8.Enabled = False
                    ElseIf (a = 8) Then
                        btnButton9.Enabled = False
                    ElseIf (a = 9) Then
                        btnButton10.Enabled = False
                    End If
                End If
            Next
        End If
        '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
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

        Dim oFrmPrgInputData As New FrmPrgInputData

        Me.Hide()
        oFrmPrgInputData.ShowDialog()
        oFrmPrgInputData.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「配信設定指示」ボタンクリック
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton8.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmPrgOrderDelivery As New FrmPrgOrderDelivery

        If oFrmPrgOrderDelivery.InitFrmData = False Then
            oFrmPrgOrderDelivery = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmPrgOrderDelivery.ShowDialog()
        oFrmPrgOrderDelivery.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「配信状況表示」ボタンクリック
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton9.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmPrgDispDelivery As New FrmPrgDispDelivery

        If oFrmPrgDispDelivery.InitFrmData() = False Then
            oFrmPrgDispDelivery = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmPrgDispDelivery.ShowDialog()
        oFrmPrgDispDelivery.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「バージョン表示」ボタンクリック
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton10.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmPrgDispVersion As New FrmPrgDispVersion

        If oFrmPrgDispVersion.InitFrmData() = False Then
            oFrmPrgDispVersion = Nothing
            Call waitCursor(False)
            Exit Sub
        End If

        Me.Hide()
        oFrmPrgDispVersion.ShowDialog()
        oFrmPrgDispVersion.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub
    '「終了」ボタンクリック
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        LogOperation(sender, e)
        Me.Close()

    End Sub

    '「プログラム適用リスト取込」ボタンクリック
    Private Sub btnButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton2.Click

        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim oFrmPrgInputList As New FrmPrgInputList

        Me.Hide()
        oFrmPrgInputList.ShowDialog()
        oFrmPrgInputList.Dispose()
        Me.Show()
        Call waitCursor(False)
    End Sub
End Class
