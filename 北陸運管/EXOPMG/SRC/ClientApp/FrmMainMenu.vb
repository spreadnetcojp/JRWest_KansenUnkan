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

''' <summary>メインメニュー</summary>
''' <remarks></remarks>
Public Class FrmMainMenu
    Inherits FrmBase

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
    Public WithEvents btnUnyo As System.Windows.Forms.Button
    Public WithEvents btnLogout As System.Windows.Forms.Button
    Public WithEvents btnSystem As System.Windows.Forms.Button
    Public WithEvents btnHosyu As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.btnUnyo = New System.Windows.Forms.Button
        Me.btnLogout = New System.Windows.Forms.Button
        Me.btnSystem = New System.Windows.Forms.Button
        Me.btnHosyu = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2006/08/10(木)  10:10"
        '
        'btnUnyo
        '
        Me.btnUnyo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnUnyo.Font = New System.Drawing.Font("ＭＳ ゴシック", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUnyo.Location = New System.Drawing.Point(300, 140)
        Me.btnUnyo.Name = "btnUnyo"
        Me.btnUnyo.Size = New System.Drawing.Size(416, 86)
        Me.btnUnyo.TabIndex = 3
        Me.btnUnyo.Text = "運用管理業務"
        Me.btnUnyo.UseVisualStyleBackColor = False
        '
        'btnLogout
        '
        Me.btnLogout.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnLogout.Font = New System.Drawing.Font("ＭＳ ゴシック", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnLogout.Location = New System.Drawing.Point(300, 560)
        Me.btnLogout.Name = "btnLogout"
        Me.btnLogout.Size = New System.Drawing.Size(416, 86)
        Me.btnLogout.TabIndex = 6
        Me.btnLogout.Text = "ログアウト"
        Me.btnLogout.UseVisualStyleBackColor = False
        '
        'btnSystem
        '
        Me.btnSystem.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnSystem.Font = New System.Drawing.Font("ＭＳ ゴシック", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSystem.Location = New System.Drawing.Point(300, 420)
        Me.btnSystem.Name = "btnSystem"
        Me.btnSystem.Size = New System.Drawing.Size(416, 86)
        Me.btnSystem.TabIndex = 5
        Me.btnSystem.Text = "システム管理業務"
        Me.btnSystem.UseVisualStyleBackColor = False
        '
        'btnHosyu
        '
        Me.btnHosyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnHosyu.Font = New System.Drawing.Font("ＭＳ ゴシック", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnHosyu.Location = New System.Drawing.Point(300, 280)
        Me.btnHosyu.Name = "btnHosyu"
        Me.btnHosyu.Size = New System.Drawing.Size(416, 86)
        Me.btnHosyu.TabIndex = 4
        Me.btnHosyu.Text = "保守管理業務"
        Me.btnHosyu.UseVisualStyleBackColor = False
        '
        'FrmMainMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Controls.Add(Me.btnUnyo)
        Me.Controls.Add(Me.btnLogout)
        Me.Controls.Add(Me.btnSystem)
        Me.Controls.Add(Me.btnHosyu)
        Me.Name = "FrmMainMenu"
        Me.Controls.SetChildIndex(Me.pnlBodyBase, 0)
        Me.Controls.SetChildIndex(Me.btnHosyu, 0)
        Me.Controls.SetChildIndex(Me.btnSystem, 0)
        Me.Controls.SetChildIndex(Me.btnLogout, 0)
        Me.Controls.SetChildIndex(Me.btnUnyo, 0)
        Me.Controls.SetChildIndex(Me.lblTitle, 0)
        Me.Controls.SetChildIndex(Me.lblToday, 0)
        Me.ResumeLayout(False)

    End Sub

#End Region

    'フォームロード
    Private Sub FrmMainMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '画面タイトル
        lblTitle.Text = "メインメニュー"

        '権限を検査する
        '2：運用管理
        If (FrmBase.Authority = "2") Then

            btnUnyo.Enabled = True

            btnHosyu.Enabled = True

            btnSystem.Enabled = False

            '3：保守管理
        ElseIf (FrmBase.Authority = "3") Then

            btnUnyo.Enabled = False

            btnHosyu.Enabled = True

            btnSystem.Enabled = False
            '1：システム管理
        ElseIf (FrmBase.Authority = "1") Then

            btnUnyo.Enabled = True

            btnHosyu.Enabled = True

            btnSystem.Enabled = True
            '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
        ElseIf (FrmBase.Authority = "4") Then
            Dim UCount As Integer = 0
            Dim SCount As Integer = 0
            Dim HCount As Integer = 0
            For a As Integer = 0 To FrmBase.DetailSet.Count - 1
                If (FrmBase.DetailSet(a).ToString = "1") Then
                    If (a < 10) Then
                        UCount = UCount + 1
                    ElseIf ((a > 9) And (a < 20)) Then
                        HCount = HCount + 1
                    ElseIf (a > 19) Then
                        SCount = SCount + 1
                    End If
                End If
            Next
            If (UCount > 0) Then
                btnUnyo.Enabled = True
            Else
                btnUnyo.Enabled = False
            End If
            If (HCount > 0) Then
                btnHosyu.Enabled = True
            Else
                btnHosyu.Enabled = False
            End If
            If (SCount > 0) Then
                btnSystem.Enabled = True
            Else
                btnSystem.Enabled = False
            End If
        End If
        '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
    End Sub

    '「運用管理業務」ボタンクリック
    Private Sub btnUnyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnyo.Click

        Call waitCursor(True)
        '「運用管理業務」ボタン押下。
        LogOperation(sender, e)

        Dim oFrmOpeMenu As New FrmOpeMenu

        Me.Hide()
        oFrmOpeMenu.ShowDialog()
        oFrmOpeMenu.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「保守管理業務」ボタンクリック
    Private Sub btnHosyu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHosyu.Click

        Call waitCursor(True)
        '「保守管理業務」ボタン押下。
        LogOperation(sender, e)

        Dim oFrmMntMenu As New FrmMntMenu

        Me.Hide()
        oFrmMntMenu.ShowDialog()
        oFrmMntMenu.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「システム管理業務」ボタンクリック
    Private Sub btnSystem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSystem.Click

        Call waitCursor(True)
        '「システム管理業務」ボタン押下。
        LogOperation(sender, e)

        Dim oFrmSysMenu As New FrmSysMenu

        Me.Hide()
        oFrmSysMenu.ShowDialog()
        oFrmSysMenu.Dispose()
        Me.Show()
        Call waitCursor(False)

    End Sub

    '「ログアウト」ボタンクリック
    Private Sub btnLogout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogout.Click

        '「ログアウト」ボタン押下。
        LogOperation(sender, e)

        Me.Close()

    End Sub

End Class
