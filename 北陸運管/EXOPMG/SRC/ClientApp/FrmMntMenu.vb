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

''' <summary>
''' 保守管理メニュー
''' </summary>
Public Class FrmMntMenu
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
        Me.lblToday.Text = "2013/02/20(水)  19:56"
        '
        'FrmMntMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntMenu"
        Me.Text = "運用端末 "
        Me.ResumeLayout(False)

    End Sub

#End Region
#Region "宣言領域（Private）"
    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean
#End Region
#Region "イベント"

    ''' <summary>
    ''' フォームロード
    ''' </summary>
    Private Sub FrmMntMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim bRtn As Boolean = False
        LbEventStop = True      'イベント発生ＯＦＦ
        LfWaitCursor()
        Try
            Log.Info("Method started.")

            '画面タイトル
            lblTitle.Text = "保守管理メニュー"

            'ボタン名称を設定する
            btnButton1.Text = "別集札データ確認"
            btnButton2.Text = "不正乗車検出データ確認"
            btnButton3.Text = "強行突破検出データ確認"
            btnButton4.Text = "紛失券検出データ確認"
            btnButton5.Text = "異常データ確認"
            btnButton6.Text = "稼動・保守データ出力"
            btnButton8.Text = "機器接続状態確認"
            btnButton9.Text = "監視盤設定情報"
            btnButton10.Text = "収集データ確認"
            '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
            If (FrmBase.Authority = "4") Then
                For a As Integer = 10 To 19
                    If (FrmBase.DetailSet(a).ToString = "0") Then
                        If (a = 10) Then
                            btnButton1.Enabled = False
                        ElseIf (a = 11) Then
                            btnButton2.Enabled = False
                        ElseIf (a = 12) Then
                            btnButton3.Enabled = False
                        ElseIf (a = 13) Then
                            btnButton4.Enabled = False
                        ElseIf (a = 14) Then
                            btnButton5.Enabled = False
                        ElseIf (a = 15) Then
                            btnButton6.Enabled = False
                        ElseIf (a = 16) Then
                            btnButton8.Enabled = False
                        ElseIf (a = 17) Then
                            btnButton9.Enabled = False
                        ElseIf (a = 18) Then
                            btnButton10.Enabled = False
                        ElseIf (a = 19) Then
                            btnButton11.Enabled = False
                        End If
                    End If
                Next
            End If
            '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
            If Config.SelfCompany = EkCompany.JRWest Then btnButton11.Text = "時間帯別乗降データ出力"

            'ボタン非表示
            btnButton7.Visible = False
            If Not (Config.SelfCompany = EkCompany.JRWest) Then btnButton11.Visible = False
            btnButton12.Visible = False
            btnButton13.Visible = False

            'ボタン名称(戻　る)を設定する
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

    '//////////////////////////////////////////////ボタンクリック
    ''' <summary>
    ''' 「別集札データ確認」
    ''' </summary>
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton1.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hfrmMntDispBesshuData As New FrmMntDispBesshuData
        If hfrmMntDispBesshuData.InitFrm = False Then
            LfWaitCursor(False)
            hfrmMntDispBesshuData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hfrmMntDispBesshuData.ShowDialog()
        hfrmMntDispBesshuData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「不正乗車検出データ確認」
    ''' </summary>
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton2.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispFuseiJoshaData As New FrmMntDispFuseiJoshaData
        If hFrmMntDispFuseiJoshaData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispFuseiJoshaData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispFuseiJoshaData.ShowDialog()
        hFrmMntDispFuseiJoshaData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「強行突破検出データ確認」
    ''' </summary>
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton3.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispKyokoToppaData As New FrmMntDispKyokoToppaData
        If hFrmMntDispKyokoToppaData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispKyokoToppaData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispKyokoToppaData.ShowDialog()
        hFrmMntDispKyokoToppaData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「紛失券検出データ確認」
    ''' </summary>
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton4.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispFunshitsuData As New FrmMntDispFunshitsuData
        If hFrmMntDispFunshitsuData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispFunshitsuData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispFunshitsuData.ShowDialog()
        hFrmMntDispFunshitsuData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「異常データ確認」
    ''' </summary>
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton5.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispFaultData As New FrmMntDispFaultData
        If hFrmMntDispFaultData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispFaultData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispFaultData.ShowDialog()
        hFrmMntDispFaultData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「稼動・保守データ出力」
    ''' </summary>
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton6.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispKadoData As New FrmMntDispKadoData
        If hFrmMntDispKadoData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispKadoData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispKadoData.ShowDialog()
        hFrmMntDispKadoData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「機器接続状態確認」
    ''' </summary>
    Private Sub btnButton8_Click(sender As System.Object, e As System.EventArgs) Handles btnButton8.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispConStatus As New FrmMntDispConStatus
        If hFrmMntDispConStatus.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispConStatus.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispConStatus.ShowDialog()
        hFrmMntDispConStatus.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「監視盤設定情報」
    ''' </summary>
    Private Sub btnButton9_Click(sender As System.Object, e As System.EventArgs) Handles btnButton9.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispKsbConfig As New FrmMntDispKsbConfig
        If hFrmMntDispKsbConfig.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispKsbConfig.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispKsbConfig.ShowDialog()
        hFrmMntDispKsbConfig.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「収集データ確認」
    ''' </summary>
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton10.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispCollectedData As New FrmMntDispCollectedData
        If hFrmMntDispCollectedData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispCollectedData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispCollectedData.ShowDialog()
        hFrmMntDispCollectedData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「時間帯別乗降データ出力」
    ''' </summary>
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnButton11.Click
        LfWaitCursor()
        LogOperation(sender, e)
        Dim hFrmMntDispTrafficData As New FrmMntDispTrafficData
        If hFrmMntDispTrafficData.InitFrm = False Then
            LfWaitCursor(False)
            hFrmMntDispTrafficData.Dispose()
            Exit Sub
        End If
        LfWaitCursor(False)
        Me.Hide()
        hFrmMntDispTrafficData.ShowDialog()
        hFrmMntDispTrafficData.Dispose()
        Me.Show()
    End Sub

    ''' <summary>
    ''' 「戻る」
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        LogOperation(sender, e)
        Me.Close()
    End Sub

#End Region

End Class
