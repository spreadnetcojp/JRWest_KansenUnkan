' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2013/11/11  (NES)金沢  フェーズ２権限対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>システム管理メニュー</summary>
''' <remarks></remarks>
Public Class FrmSysMenu
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
        'btnButton13
        '
        Me.btnButton13.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnReturn.TabIndex = 7
        '
        'btnButton12
        '
        Me.btnButton12.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton12.TabIndex = 5
        '
        'btnButton11
        '
        Me.btnButton11.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton11.TabIndex = 5
        '
        'btnButton10
        '
        Me.btnButton10.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton10.TabIndex = 4
        '
        'btnButton9
        '
        Me.btnButton9.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton9.Size = New System.Drawing.Size(369, 48)
        Me.btnButton9.TabIndex = 3
        '
        'btnButton8
        '
        Me.btnButton8.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton8.TabIndex = 2
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
        Me.btnButton2.TabIndex = 1
        '
        'btnButton1
        '
        Me.btnButton1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnButton1.TabIndex = 1
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.Black
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblToday.Text = "2013/03/27(水)  09:08"
        '
        'FrmSysMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysMenu"
        Me.Text = " "
        Me.ResumeLayout(False)

    End Sub

#End Region
#Region "宣言領域（Private）"
    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly FormTitle As String = "システム管理メニュー"
    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean
#End Region
    ''' <summary>
    ''' フォームロード
    ''' </summary>
    Private Sub FrmSysMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LfWaitCursor()
        Dim bRtn As Boolean = False
        LbEventStop = True      'イベント発生ＯＦＦ
        Try
            Log.Info("Method started.")

            '画面タイトル
            lblTitle.Text = FormTitle
            'ボタン名称を設定する
            btnButton1.Text = "ＩＤマスタ設定"
            'ボタン非表示
            btnButton2.Visible = False
            btnButton3.Visible = False
            btnButton4.Visible = False
            btnButton5.Visible = False
            btnButton6.Visible = False
            btnButton7.Visible = False
            'ボタン名称を設定する
            btnButton8.Text = "稼動・保守データ設定"
            btnButton9.Text = "パターン設定"
            btnButton10.Text = "エリア設定"
            btnButton11.Text = "運管設定管理"
            'ボタン非表示
            '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
            If (FrmBase.Authority = "4") Then
                For a As Integer = 20 To 24
                    If (FrmBase.DetailSet(a).ToString = "0") Then
                        If (a = 20) Then
                            btnButton1.Enabled = False
                        ElseIf (a = 21) Then
                            btnButton8.Enabled = False
                        ElseIf (a = 22) Then
                            btnButton9.Enabled = False
                        ElseIf (a = 23) Then
                            btnButton10.Enabled = False
                        ElseIf (a = 24) Then
                            btnButton11.Enabled = False
                        End If
                    End If
                Next
            End If
            '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
            btnButton12.Visible = False
            btnButton13.Visible = False
            'ボタン名称を設定する
            btnReturn.Text = "戻　る"
            LbEventStop = False 'イベント発生ＯＮ
            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
                Me.Close()
            End If
            LbEventStop = False 'イベント発生ＯＮ
            LfWaitCursor(False)
        End Try
    End Sub

    '「ＩＤマスタ設定」ボタンクリック時
    Private Sub btnButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton1.Click

        Call waitCursor(True)
        '「ＩＤマスタ設定」ボタン押下。
        LogOperation(sender, e)
        Dim oFrmSysIDMst As New FrmSysIDMst

        If oFrmSysIDMst.InitFrmData() = False Then
            oFrmSysIDMst = Nothing
            Call waitCursor(False)
            Exit Sub
        End If
        Me.Hide()
        '「ＩＤマスタ設定」画面へ遷移する。
        oFrmSysIDMst.ShowDialog()
        oFrmSysIDMst.Dispose()
        Me.Show()
        Call waitCursor(False)
    End Sub

    ''' <summary>
    ''' 「稼動・保守データ設定」
    ''' </summary>
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton8.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrm As New FrmSysKadoDataMst
        If hFrm.InitFrm = False Then
            LfWaitCursor(False)
            hFrm.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        '稼動・保守データ設定画面へ遷移する。
        hFrm.ShowDialog()
        hFrm.Dispose()
        Me.Show()
    End Sub

    '「パターン設定」ボタンクリック時
    Private Sub btnButton9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton9.Click

        Call waitCursor(True)
        '「パターン設定」ボタン押下。
        LogOperation(sender, e)
        Dim oFrmSysPatternMst As New FrmSysPatternMst

        If oFrmSysPatternMst.InitFrm() = False Then
            oFrmSysPatternMst = Nothing
            Call waitCursor(False)
            Exit Sub
        End If
        Me.Hide()
        'パターン設定画面へ遷移する。
        oFrmSysPatternMst.ShowDialog()
        oFrmSysPatternMst.Dispose()
        Me.Show()
        Call waitCursor(False)


    End Sub

    '「エリア設定」ボタンクリック時
    Private Sub btnButton10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton10.Click
        Call waitCursor(True)
        '「エリア設定」ボタン押下。
        LogOperation(sender, e)
        Dim oFrmSysAreaMst As New FrmSysAreaMst

        If oFrmSysAreaMst.InitFrmData() = False Then
            oFrmSysAreaMst = Nothing
            Call waitCursor(False)
            Exit Sub
        End If
        Me.Hide()
        '「エリア設定」画面へ遷移する。
        oFrmSysAreaMst.ShowDialog()
        oFrmSysAreaMst.Dispose()
        Me.Show()
        Call waitCursor(False)
    End Sub

    '「 運管設定管理 」ボタンクリック時
    Private Sub btnButton11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton11.Click
        Call waitCursor(True)
        '「運管設定管理」ボタン押下。
        LogOperation(sender, e)
        Dim oFrmSysUnKanSetMng As New FrmSysUnKanSetMng

        If oFrmSysUnKanSetMng.InitFrm() = False Then
            oFrmSysUnKanSetMng = Nothing
            Call waitCursor(False)
            Exit Sub
        End If
        Me.Hide()
        '「運管設定管理」画面へ遷移する。
        oFrmSysUnKanSetMng.ShowDialog()
        oFrmSysUnKanSetMng.Dispose()
        Me.Show()
        Call waitCursor(False)
    End Sub
    '「戻  る」ボタンクリック時
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '「戻  る」ボタン押下。
        LogOperation(sender, e)
        Me.Close()

    End Sub
End Class
