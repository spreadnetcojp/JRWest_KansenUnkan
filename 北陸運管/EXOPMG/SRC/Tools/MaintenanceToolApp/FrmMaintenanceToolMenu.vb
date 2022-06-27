' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2014/04/20  (NES)      新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' 保守管理メニュー
''' </summary>
Public Class FrmMaintenanceToolMenu

    Public Sub New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

    End Sub

    ''' <summary>
    ''' フォームロード
    ''' </summary>
    Private Sub FrmMaintenanceToolMenu_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim bRtn As Boolean = False
        Me.Cursor = Cursors.WaitCursor
        Try
            Log.Info("Method started.")

            'ウィンドウタイトルを設定する
            Me.Text = Config.MachineKind & " Ver" & Config.VerNoSet

            'ボタン名称を設定する
            Me.btnButton1.Text = "休止号機設定"
            Me.btnButton2.Text = "メール送信対象エラーコード設定"
            Me.btnButton3.Text = ""
            Me.btnButton4.Text = ""

            'ボタン非表示
            Me.btnButton1.Visible = True
            Me.btnButton2.Visible = True
            Me.btnButton3.Visible = False
            Me.btnButton4.Visible = False

            'ボタン名称(閉じる)を設定する
            Me.btnReturn.Text = "閉じる"
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
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' 「休止号機設定」
    ''' </summary>
    Private Sub btnButton1_Click(sender As System.Object, e As System.EventArgs) Handles btnButton1.Click
        Me.Cursor = Cursors.WaitCursor
        Dim hFrmRestingMachine As New FrmRestingMachine()
        Me.Cursor = Cursors.Default
        Me.Hide()
        hFrmRestingMachine.ShowDialog()
        hFrmRestingMachine.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「メール送信対象エラーコード設定」
    ''' </summary>
    Private Sub btnButton2_Click(sender As System.Object, e As System.EventArgs) Handles btnButton2.Click
        Me.Cursor = Cursors.WaitCursor
        Dim hFrmNotifiableErrCode As New FrmNotifiableErrCode()
        Me.Cursor = Cursors.Default
        Me.Hide()
        hFrmNotifiableErrCode.ShowDialog()
        hFrmNotifiableErrCode.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「戻る」
    ''' </summary>
    Private Sub btnReturn_Click(sender As System.Object, e As System.EventArgs) Handles btnReturn.Click
        Me.Close()
    End Sub

End Class
