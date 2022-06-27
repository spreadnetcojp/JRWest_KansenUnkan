' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DBCommon.OPMGUtility
Imports System.Text

''' <summary>DBから削除されたユーザの情報</summary>
''' <remarks>
''' 選択のユーザを削除する。
''' 内容は、非アクティブとする。
''' </remarks>
Public Class FrmSysIDMstDelete
    Inherits System.Windows.Forms.Form

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

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    Friend WithEvents pnlBase As System.Windows.Forms.Panel
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents chkLockout As System.Windows.Forms.CheckBox
    Friend WithEvents grpAuth As System.Windows.Forms.GroupBox
    Friend WithEvents rbtSysmnt As System.Windows.Forms.RadioButton
    Friend WithEvents rbtUsumnt As System.Windows.Forms.RadioButton
    Friend WithEvents rbtAdmin As System.Windows.Forms.RadioButton
    Friend WithEvents lblTitleRepwd As System.Windows.Forms.Label
    Friend WithEvents lblTitlePwd As System.Windows.Forms.Label
    Friend WithEvents lblTitleID As System.Windows.Forms.Label
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents lblID As System.Windows.Forms.Label
    Friend WithEvents lblRePwd As System.Windows.Forms.Label
    Friend WithEvents lblPwd As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.pnlBase = New System.Windows.Forms.Panel()
        Me.lblRePwd = New System.Windows.Forms.Label()
        Me.lblPwd = New System.Windows.Forms.Label()
        Me.lblID = New System.Windows.Forms.Label()
        Me.pnlMain = New System.Windows.Forms.Panel()
        Me.grpAuth = New System.Windows.Forms.GroupBox()
        Me.rbtSysmnt = New System.Windows.Forms.RadioButton()
        Me.rbtUsumnt = New System.Windows.Forms.RadioButton()
        Me.rbtAdmin = New System.Windows.Forms.RadioButton()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.chkLockout = New System.Windows.Forms.CheckBox()
        Me.lblTitleRepwd = New System.Windows.Forms.Label()
        Me.lblTitlePwd = New System.Windows.Forms.Label()
        Me.lblTitleID = New System.Windows.Forms.Label()
        Me.pnlBase.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.grpAuth.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlBase
        '
        Me.pnlBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBase.Controls.Add(Me.lblRePwd)
        Me.pnlBase.Controls.Add(Me.lblPwd)
        Me.pnlBase.Controls.Add(Me.lblID)
        Me.pnlBase.Controls.Add(Me.pnlMain)
        Me.pnlBase.Controls.Add(Me.btnStop)
        Me.pnlBase.Controls.Add(Me.btnDelete)
        Me.pnlBase.Controls.Add(Me.chkLockout)
        Me.pnlBase.Controls.Add(Me.lblTitleRepwd)
        Me.pnlBase.Controls.Add(Me.lblTitlePwd)
        Me.pnlBase.Controls.Add(Me.lblTitleID)
        Me.pnlBase.Location = New System.Drawing.Point(0, 0)
        Me.pnlBase.Name = "pnlBase"
        Me.pnlBase.Size = New System.Drawing.Size(594, 418)
        Me.pnlBase.TabIndex = 0
        '
        'lblRePwd
        '
        Me.lblRePwd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblRePwd.Location = New System.Drawing.Point(161, 119)
        Me.lblRePwd.Name = "lblRePwd"
        Me.lblRePwd.Size = New System.Drawing.Size(110, 18)
        Me.lblRePwd.TabIndex = 0
        '
        'lblPwd
        '
        Me.lblPwd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPwd.Location = New System.Drawing.Point(161, 79)
        Me.lblPwd.Name = "lblPwd"
        Me.lblPwd.Size = New System.Drawing.Size(110, 18)
        Me.lblPwd.TabIndex = 1
        '
        'lblID
        '
        Me.lblID.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblID.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblID.Location = New System.Drawing.Point(161, 39)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(110, 18)
        Me.lblID.TabIndex = 2
        '
        'pnlMain
        '
        Me.pnlMain.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlMain.Controls.Add(Me.grpAuth)
        Me.pnlMain.Location = New System.Drawing.Point(41, 186)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(510, 80)
        Me.pnlMain.TabIndex = 2
        '
        'grpAuth
        '
        Me.grpAuth.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grpAuth.Controls.Add(Me.rbtSysmnt)
        Me.grpAuth.Controls.Add(Me.rbtUsumnt)
        Me.grpAuth.Controls.Add(Me.rbtAdmin)
        Me.grpAuth.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpAuth.Location = New System.Drawing.Point(10, 8)
        Me.grpAuth.Name = "grpAuth"
        Me.grpAuth.Size = New System.Drawing.Size(490, 60)
        Me.grpAuth.TabIndex = 1
        Me.grpAuth.TabStop = False
        Me.grpAuth.Text = "権　限"
        '
        'rbtSysmnt
        '
        Me.rbtSysmnt.AutoSize = True
        Me.rbtSysmnt.Enabled = False
        Me.rbtSysmnt.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbtSysmnt.Location = New System.Drawing.Point(347, 24)
        Me.rbtSysmnt.Name = "rbtSysmnt"
        Me.rbtSysmnt.Size = New System.Drawing.Size(123, 17)
        Me.rbtSysmnt.TabIndex = 4
        Me.rbtSysmnt.TabStop = True
        Me.rbtSysmnt.Text = "システム管理者"
        Me.rbtSysmnt.UseVisualStyleBackColor = True
        '
        'rbtUsumnt
        '
        Me.rbtUsumnt.AutoSize = True
        Me.rbtUsumnt.Enabled = False
        Me.rbtUsumnt.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbtUsumnt.Location = New System.Drawing.Point(171, 24)
        Me.rbtUsumnt.Name = "rbtUsumnt"
        Me.rbtUsumnt.Size = New System.Drawing.Size(95, 17)
        Me.rbtUsumnt.TabIndex = 3
        Me.rbtUsumnt.TabStop = True
        Me.rbtUsumnt.Text = "運用管理者"
        Me.rbtUsumnt.UseVisualStyleBackColor = True
        '
        'rbtAdmin
        '
        Me.rbtAdmin.AutoSize = True
        Me.rbtAdmin.Enabled = False
        Me.rbtAdmin.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbtAdmin.Location = New System.Drawing.Point(26, 24)
        Me.rbtAdmin.Name = "rbtAdmin"
        Me.rbtAdmin.Size = New System.Drawing.Size(67, 17)
        Me.rbtAdmin.TabIndex = 2
        Me.rbtAdmin.TabStop = True
        Me.rbtAdmin.Text = "一般者"
        Me.rbtAdmin.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.BackColor = System.Drawing.Color.Silver
        Me.btnStop.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnStop.Location = New System.Drawing.Point(459, 356)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(90, 32)
        Me.btnStop.TabIndex = 7
        Me.btnStop.Text = "終　了"
        Me.btnStop.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.Silver
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(320, 356)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(90, 32)
        Me.btnDelete.TabIndex = 6
        Me.btnDelete.Text = "削  除"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'chkLockout
        '
        Me.chkLockout.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.chkLockout.Enabled = False
        Me.chkLockout.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chkLockout.Location = New System.Drawing.Point(52, 289)
        Me.chkLockout.Name = "chkLockout"
        Me.chkLockout.Size = New System.Drawing.Size(110, 23)
        Me.chkLockout.TabIndex = 5
        Me.chkLockout.Text = "ロックアウト"
        Me.chkLockout.UseVisualStyleBackColor = False
        '
        'lblTitleRepwd
        '
        Me.lblTitleRepwd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblTitleRepwd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitleRepwd.Location = New System.Drawing.Point(46, 119)
        Me.lblTitleRepwd.Name = "lblTitleRepwd"
        Me.lblTitleRepwd.Size = New System.Drawing.Size(110, 18)
        Me.lblTitleRepwd.TabIndex = 4
        Me.lblTitleRepwd.Text = "パスワード確認"
        Me.lblTitleRepwd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTitlePwd
        '
        Me.lblTitlePwd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblTitlePwd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitlePwd.Location = New System.Drawing.Point(46, 79)
        Me.lblTitlePwd.Name = "lblTitlePwd"
        Me.lblTitlePwd.Size = New System.Drawing.Size(110, 18)
        Me.lblTitlePwd.TabIndex = 5
        Me.lblTitlePwd.Text = "パスワード"
        Me.lblTitlePwd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTitleID
        '
        Me.lblTitleID.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblTitleID.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitleID.Location = New System.Drawing.Point(46, 39)
        Me.lblTitleID.Name = "lblTitleID"
        Me.lblTitleID.Size = New System.Drawing.Size(110, 18)
        Me.lblTitleID.TabIndex = 6
        Me.lblTitleID.Text = "ＩＤコード"
        Me.lblTitleID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmSysIDMstDelete
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlBase)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysIDMstDelete"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "IDマスタ削除"
        Me.pnlBase.ResumeLayout(False)
        Me.pnlMain.ResumeLayout(False)
        Me.grpAuth.ResumeLayout(False)
        Me.grpAuth.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "宣言領域（Private）"

    ''' <summary>
    ''' 初期処理呼出判定
    ''' （True:初期処理呼出済み、False:初期処理未呼出(Form_Load内で初期処理実施)）
    ''' </summary>
    Private LbInitCallFlg As Boolean = False

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean

    'DBへ入力された権限コード。
    Private Const DB_AUTH_SYS As String = "1"
    Private Const DB_AUTH_ADMIN As String = "2"
    Private Const DB_AUTH_USUAL As String = "3"

    'DBへ入力されたロックフラグ。
    Private Const DB_LOCKING As String = "1"

    'FrmSysIDMst画面のID値を取得する。
    Private sUserid As String = ""

    Public Property Userid() As String
        Get
            Return sUserid
        End Get
        Set(ByVal value As String)
            sUserid = value
        End Set
    End Property

    'FrmSysIDMs画面のパスワード値を取得する。
    Private sPwd As String = ""

    'FrmSysIDMst画面の権限値を取得する。
    Private sAuthority As String = ""

    'FrmSysIDMst画面のロックフラグを取得する。
    Private sLock As String = ""

    '更新日時
    Private oldDate As String = ""

    '更新日時
    Private newDate As String = ""

#End Region

#Region "メソッド（Public）"

    ''' <summary>ＩＤデータ削除画面のデータを準備する</summary>
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean
        Dim bRet As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim sPassword As String = ""
        Dim dtMstTable As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer

        Try
            Log.Info("Method started.")
            'データを取得する。
            sSql = LfGetSelectString()
            nRtn = FrmBase.BaseSqlDataTableFill(sSql, dtMstTable)
            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    Return bRet
                Case Else
                    If dtMstTable Is Nothing OrElse dtMstTable.Rows.Count = 0 Then
                        '検索条件に一致するデータは存在しません。
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Return bRet
                    Else
                        sPwd = dtMstTable.Rows(0).Item("PASSWORD").ToString
                        For i As Integer = 0 To sPwd.Length - 1
                            sPassword = sPassword + "*"
                        Next
                        sPwd = sPassword
                        sAuthority = dtMstTable.Rows(0).Item("AUTHORITY_LEVEL").ToString
                        sLock = dtMstTable.Rows(0).Item("LOCK_STS").ToString
                        oldDate = dtMstTable.Rows(0).Item("UPDATE_DATE").ToString
                    End If
            End Select

            bRet = True

        Catch ex As Exception
            '画面表示処理に失敗しました。
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
            bRet = False
        Finally
            If bRet Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
            End If
            LbEventStop = False 'イベント発生ＯＮ
        End Try

        Return bRet

    End Function

#End Region

#Region "イベント"

    ''' <summary>
    ''' ローディング　メインウィンドウ  
    ''' </summary>
    Private Sub FrmSysIDMstDelete_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrmData() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            '画面背景色（BackColor）を設定する
            pnlBase.BackColor = Config.BackgroundColor
            pnlMain.BackColor = Config.BackgroundColor
            grpAuth.BackColor = Config.BackgroundColor
            rbtAdmin.BackColor = Config.BackgroundColor
            rbtUsumnt.BackColor = Config.BackgroundColor
            rbtSysmnt.BackColor = Config.BackgroundColor
            chkLockout.BackColor = Config.BackgroundColor
            lblID.BackColor = Config.BackgroundColor
            lblPwd.BackColor = Config.BackgroundColor
            lblRePwd.BackColor = Config.BackgroundColor
            lblTitleID.BackColor = Config.BackgroundColor
            lblTitlePwd.BackColor = Config.BackgroundColor
            lblTitleRepwd.BackColor = Config.BackgroundColor

            'ボタン背景色（BackColor）を設定する
            btnDelete.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor

            'ユーザを設定する。
            lblID.Text = sUserid

            'パスワードを設定する
            lblPwd.Text = sPwd

            'パスワード確認を設定する。
            lblRePwd.Text = sPwd

            '権限を設定する。
            If sAuthority = DB_AUTH_SYS Then
                rbtSysmnt.Checked = True
            ElseIf sAuthority = DB_AUTH_ADMIN Then
                rbtUsumnt.Checked = True
            ElseIf sAuthority = DB_AUTH_USUAL Then
                rbtAdmin.Checked = True
            Else
                rbtSysmnt.Checked = False
                rbtUsumnt.Checked = False
                rbtAdmin.Checked = False
            End If

            'ロックフラグを設定する。
            If sLock = DB_LOCKING Then
                chkLockout.Checked = True
            Else
                chkLockout.Checked = False
            End If

            Me.btnDelete.Focus()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' 「削除」ボタンを押下すると、データベースにて現在選択されたユーザを削除する。
    ''' </summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If LbEventStop Then Exit Sub
        Dim dtMstTable As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer

        Try
            LbEventStop = True
            '削除ボタン押下。
            FrmBase.LogOperation(sender, e, Me.Text)

            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyDeleteTheIdCode, sUserid).Equals(System.Windows.Forms.DialogResult.Yes) Then
                FrmBase.LogOperation(Lexis.YesButtonClicked, Me.Text)
                Call WaitCursor(True)
                'データを取得する。
                sSql = LfGetSelectString()
                nRtn = FrmBase.BaseSqlDataTableFill(sSql, dtMstTable)
                Select Case nRtn
                    Case -9             'ＤＢオープンエラー
                        AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                        Exit Sub
                    Case Else
                        If dtMstTable Is Nothing OrElse dtMstTable.Rows.Count = 0 Then
                            '検索条件に一致するデータは存在しません。
                            AlertBox.Show(Lexis.CompetitiveOperationDetected)
                            Exit Sub
                        Else
                            newDate = dtMstTable.Rows(0).Item("UPDATE_DATE").ToString
                        End If
                End Select

                '排他チェック
                If Not oldDate.Equals(newDate) Then
                    AlertBox.Show(Lexis.CompetitiveOperationDetected)
                    Exit Sub
                End If

                '削除処理
                Call DeleteID()
                FrmBase.LogOperation(Lexis.DeleteCompleted, Me.Text) 'TODO: 少なくとも「操作」ログではない。詳細設計も含め確認。   '削除処理が正常に終了しました。
                AlertBox.Show(Lexis.DeleteCompleted)
                FrmBase.LogOperation(Lexis.OkButtonClicked, Me.Text)
                Me.Close()
            Else
                FrmBase.LogOperation(Lexis.NoButtonClicked, Me.Text)
                btnDelete.Select()
            End If
        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnDelete.Select()
            Exit Sub
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '予期せぬエラーが発生しました。
            AlertBox.Show(Lexis.DeleteFailed)
            btnDelete.Select()
            Exit Sub
        Finally
            LbEventStop = False
            Call WaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 「終了」ボタンを押下すると、本画面が終了される。
    ''' </summary>
    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        '終了ボタン押下。
        FrmBase.LogOperation(sender, e, Me.Text)
        Me.Close()
    End Sub

#End Region

#Region "メソッド（Private）"

    ''' <summary>
    ''' データベースにて現在選択されたユーザを削除する。
    ''' </summary>
    Private Sub DeleteID()

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Dim dbCtl As DatabaseTalker
        dbCtl = New DatabaseTalker

        Try
            sBuilder.AppendLine(" DELETE FROM M_USER ")
            sBuilder.AppendLine(" WHERE USER_ID = " & Utility.SetSglQuot(sUserid))
            sSQL = sBuilder.ToString()

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()

        Catch ex As Exception

            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

    End Sub

    ''' <summary>
    ''' データを取得する。
    ''' </summary>
    Private Function LfGetSelectString() As String
        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Try

            sBuilder.AppendLine(" SELECT PASSWORD, AUTHORITY_LEVEL, LOCK_STS, UPDATE_DATE")
            sBuilder.AppendLine("  FROM M_USER  ")
            sBuilder.AppendLine("  WHERE USER_ID = " & Utility.SetSglQuot(sUserid))
            sSQL = sBuilder.ToString()

            Return sSQL
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try

    End Function

#End Region

#Region "カーソル待ち"

    ''' <summary>
    ''' カーソル待ち
    ''' </summary>
    ''' <param name="bWait">true:待ち開始　false:待ち終了</param>
    ''' <remarks>カーソルが砂時計になる</remarks>
    Private Sub WaitCursor(Optional ByVal bWait As Boolean = True)

        If bWait = True Then
            Me.Cursor = Cursors.WaitCursor
            Me.Enabled = False
        Else
            Me.Cursor = Cursors.Default
            Me.Enabled = True
        End If

    End Sub

#End Region

End Class