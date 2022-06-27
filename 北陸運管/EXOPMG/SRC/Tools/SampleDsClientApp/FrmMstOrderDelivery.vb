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
Imports System.IO

''' <summary>
''' 配信指示
''' </summary>
''' <remarks>マスタ管理メニューより「配信指示」ボタンをクリックすると、本画面を表示する。
''' 本画面にてマスタ適用リストの読込み、登録を行う。</remarks>
Public Class FrmMstOrderDelivery
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

    Friend WithEvents btnDllInvoke As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents txtListFileName As System.Windows.Forms.TextBox
    Friend WithEvents chkForcing As System.Windows.Forms.CheckBox
    Friend WithEvents lblTgl As System.Windows.Forms.Label


    Private Sub InitializeComponent()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnDllInvoke = New System.Windows.Forms.Button()
        Me.lblTgl = New System.Windows.Forms.Label()
        Me.txtListFileName = New System.Windows.Forms.TextBox()
        Me.chkForcing = New System.Windows.Forms.CheckBox()
        Me.pnlBodyBase.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.chkForcing)
        Me.pnlBodyBase.Controls.Add(Me.txtListFileName)
        Me.pnlBodyBase.Controls.Add(Me.lblTgl)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnDllInvoke)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/05/13(月)  18:26"
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(749, 414)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 72
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnDllInvoke
        '
        Me.btnDllInvoke.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDllInvoke.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDllInvoke.Location = New System.Drawing.Point(749, 356)
        Me.btnDllInvoke.Name = "btnDllInvoke"
        Me.btnDllInvoke.Size = New System.Drawing.Size(128, 40)
        Me.btnDllInvoke.TabIndex = 71
        Me.btnDllInvoke.Text = "Go!"
        Me.btnDllInvoke.UseVisualStyleBackColor = False
        '
        'lblTgl
        '
        Me.lblTgl.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTgl.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTgl.Location = New System.Drawing.Point(87, 220)
        Me.lblTgl.Name = "lblTgl"
        Me.lblTgl.Size = New System.Drawing.Size(255, 23)
        Me.lblTgl.TabIndex = 73
        Me.lblTgl.Text = "マスタ適用リストファイル名称"
        Me.lblTgl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtListFileName
        '
        Me.txtListFileName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtListFileName.Location = New System.Drawing.Point(348, 220)
        Me.txtListFileName.Name = "txtListFileName"
        Me.txtListFileName.Size = New System.Drawing.Size(514, 23)
        Me.txtListFileName.TabIndex = 74
        '
        'chkForcing
        '
        Me.chkForcing.AutoSize = True
        Me.chkForcing.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chkForcing.Location = New System.Drawing.Point(348, 271)
        Me.chkForcing.Name = "chkForcing"
        Me.chkForcing.Size = New System.Drawing.Size(283, 20)
        Me.chkForcing.TabIndex = 75
        Me.chkForcing.Text = "マスタ適用リスト＋マスタ強制配信"
        Me.chkForcing.UseVisualStyleBackColor = True
        '
        'FrmMstOrderDelivery
        '
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMstOrderDelivery"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlBodyBase.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
#End Region

#Region "宣言領域（Private）"

    Private ReadOnly LcstFormTitle As String = "配信指示"

#End Region

#Region "イベント"

    Private Sub FrmMstOrderDelivery_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Log.Info("Method started.")

        '画面タイトル、画面背景色（BackColor）を設定する
        lblTitle.Text = LcstFormTitle

        Log.Info("Method ended.")
    End Sub


    Private Sub btnDllInvoke_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDllInvoke.Click
        Try
            LogOperation(sender, e)    'ボタン押下ログ

            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyInvokeMasProDll) = DialogResult.No Then
                LogOperation(Lexis.NoButtonClicked)     'Noボタン押下ログ
                Exit Sub
            End If

            LogOperation(Lexis.YesButtonClicked)     'Yesボタン押下ログ

            Call waitCursor(True)

            If OpClientUtil.Connect() = False Then
                AlertBox.Show(Lexis.ConnectFailed)
                Exit Sub
            End If

            Dim sListFileName As String = txtListFileName.Text
            Dim ullResult As MasProDllInvokeResult = OpClientUtil.InvokeMasProDll(sListFileName, chkForcing.Checked)

            OpClientUtil.Disconnect()

            Select Case ullResult
                Case MasProDllInvokeResult.Completed
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.Completed received.")
                    AlertBox.Show(Lexis.InvokeMasProDllCompleted)
                Case MasProDllInvokeResult.Failed
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.Failed received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailed)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByBusy
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByBusy received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByBusy)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByNoData
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByNoData received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByNoData)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByUnnecessary
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByUnnecessary received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByUnnecessary)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByInvalidContent
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByInvalidContent received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByInvalidContent)
                    Exit Sub
                Case MasProDllInvokeResult.FailedByUnknownLight
                    Log.Info("MasProUllResponse with MasProDllInvokeResult.FailedByUnknownLight received.")
                    AlertBox.Show(Lexis.InvokeMasProDllFailedByUnknownLight)
                    Exit Sub
                Case Else
                    Log.Fatal("The telegrapher seems broken.")
                    AlertBox.Show(Lexis.UnforeseenErrorOccurred)
                    OpClientUtil.RestartBrokenTelegrapher()
                    Exit Sub
            End Select

        Catch ex As OPMGException
            Log.Error("MasProUll failed.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)

        Finally
            Call waitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 「終了」ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>「終了」ボタンをクリックすることにより、「マスタ管理メニュー」画面に戻る。</remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        LogOperation(sender, e)    'ボタン押下ログ

        Me.Close()
    End Sub

#End Region

End Class
