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

''' <summary>運用管理メニュー</summary>
''' <remarks></remarks>
Public Class FrmOpeMenu
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
        'Button3
        '
        Me.btnButton3.Name = "Button3"
        '
        'Button4
        '
        Me.btnButton4.Name = "Button4"
        '
        'Button5
        '
        Me.btnButton5.Name = "Button5"
        '
        'Button6
        '
        Me.btnButton6.Name = "Button6"
        '
        'Button1
        '
        Me.btnButton1.Name = "Button1"
        '
        'Button7
        '
        Me.btnButton7.Name = "Button7"
        '
        'Button13
        '
        Me.btnButton13.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(0, Byte), CType(0, Byte))
        Me.btnButton13.Name = "Button13"
        '
        'Button8
        '
        Me.btnButton8.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(0, Byte), CType(0, Byte))
        Me.btnButton8.Name = "Button8"
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(0, Byte), CType(0, Byte))
        Me.btnReturn.Name = "btnReturn"
        '
        'Button9
        '
        Me.btnButton9.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(0, Byte), CType(0, Byte))
        Me.btnButton9.Name = "Button9"
        '
        'Button2
        '
        Me.btnButton2.Name = "Button2"
        '
        'Button10
        '
        Me.btnButton10.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(0, Byte), CType(0, Byte))
        Me.btnButton10.Name = "Button10"
        '
        'Button12
        '
        Me.btnButton12.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(0, Byte), CType(0, Byte))
        Me.btnButton12.Name = "Button12"
        '
        'Button11
        '
        Me.btnButton11.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(0, Byte), CType(0, Byte))
        Me.btnButton11.Name = "Button11"
        '
        'lblTitle
        '
        Me.lblTitle.Name = "lblTitle"
        '
        'lblToday
        '
        Me.lblToday.Name = "lblToday"
        Me.lblToday.Text = "2006/06/27(火)  16:11"
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.Name = "pnlBodyBase"
        '
        'frmUnyoMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "frmUnyoMenu"
        Me.ResumeLayout(False)

    End Sub

#End Region

    'フォームロード
    Private Sub FrmOpeMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '画面タイトル
        lblTitle.Text = "運用管理メニュー"

        'ボタン名称を設定する
        btnButton1.Text = "マスタ管理"
        btnButton2.Text = "プログラム管理"

        'ボタン非表示
        '2013/10/18　権限認証対応で追加
        '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
        If (FrmBase.Authority = "4") Then
            Dim mCount As Integer = 0
            Dim pCount As Integer = 0
            For a As Integer = 0 To 9
                If (FrmBase.DetailSet(a).ToString = "1") Then
                    If (a < 5) Then
                        mCount = mCount + 1
                    ElseIf ((a > 4) And (a < 10)) Then
                        pCount = pCount + 1
                    End If
                End If
               
            Next
            If (mCount > 0) Then
                btnButton1.Enabled = True
            Else
                btnButton1.Enabled = False
            End If
            If (pCount > 0) Then
                btnButton2.Enabled = True
            Else
                btnButton2.Enabled = False
            End If
        End If
        '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
        btnButton3.Visible = False
        btnButton4.Visible = False
        btnButton5.Visible = False
        btnButton6.Visible = False
        btnButton7.Visible = False
        btnButton8.Visible = False
        btnButton9.Visible = False
        btnButton10.Visible = False
        btnButton11.Visible = False
        btnButton12.Visible = False
        btnButton13.Visible = False

        'ボタン名称(戻　る)を設定する
        btnReturn.Text = "戻　る"

    End Sub

    '「マスタ管理」ボタンクリック
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton1.Click

        Call waitCursor(True)
        '「マスタ管理」ボタン押下。
        LogOperation(sender, e)
        Dim oFrmMstMenu As New FrmMstMenu

        Me.Hide()
        oFrmMstMenu.ShowDialog()
        oFrmMstMenu.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「プログラム管理」ボタンクリック
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton2.Click

        Call waitCursor(True)
        '「プログラム管理」ボタン押下。
        LogOperation(sender, e)
        Dim oFrmPrgMenu As New FrmPrgMenu

        Me.Hide()
        oFrmPrgMenu.ShowDialog()
        oFrmPrgMenu.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「終了」ボタンクリック
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '「終了」ボタン押下。
        LogOperation(sender, e)

        Me.Close()

    End Sub

End Class
