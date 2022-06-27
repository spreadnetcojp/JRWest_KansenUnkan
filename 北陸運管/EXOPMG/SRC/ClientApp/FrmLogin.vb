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

Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DBCommon.OPMGUtility
Imports JR.ExOpmg.Common
Imports System.Data.SqlClient
Imports System.IO
Imports System.Deployment.Application

''' <summary> ログイン </summary>
''' <remarks>
''' ＩＤコードの登録データが存在するか、ロックされたか、パスワードが一致するかをチェックする。
''' 連続して三回、間違ったパスワードが入力されると、そのＩＤコードがロックされる。
''' </remarks>
Public Class FrmLogin
    Inherits FrmBase

    Private Const KEYNAME As String = "USER_ID"         'キー名
    Private Const SECTIONNAME As String = "LOGIN"       'セクション名

    Private sAuth As String = ""    '権限
    Private sLstUID As String = ""  '前回登録されたＩＤコードを記録する。
    Private nTimes As Integer = 1   '同じＩＤコードでログイン試行した回数
    Private nLockout As Integer = 3 'ロックアウトするログイン試行回数


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
    Friend WithEvents txtPWD As System.Windows.Forms.TextBox
    Friend WithEvents txtID As System.Windows.Forms.TextBox
    Friend WithEvents lblPWD As System.Windows.Forms.Label
    Friend WithEvents lblID As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnLogin As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtPWD = New System.Windows.Forms.TextBox
        Me.txtID = New System.Windows.Forms.TextBox
        Me.lblPWD = New System.Windows.Forms.Label
        Me.lblID = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnLogin = New System.Windows.Forms.Button
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
        Me.pnlBodyBase.Controls.Add(Me.txtID)
        Me.pnlBodyBase.Controls.Add(Me.btnEnd)
        Me.pnlBodyBase.Controls.Add(Me.lblPWD)
        Me.pnlBodyBase.Controls.Add(Me.lblID)
        Me.pnlBodyBase.Controls.Add(Me.btnLogin)
        Me.pnlBodyBase.Controls.Add(Me.txtPWD)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2006/08/29(火)  10:05"
        '
        'txtPWD
        '
        Me.txtPWD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPWD.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.txtPWD.Location = New System.Drawing.Point(528, 256)
        Me.txtPWD.MaxLength = 8
        Me.txtPWD.Name = "txtPWD"
        Me.txtPWD.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPWD.Size = New System.Drawing.Size(80, 23)
        Me.txtPWD.TabIndex = 1
        '
        'txtID
        '
        Me.txtID.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtID.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.txtID.Location = New System.Drawing.Point(528, 208)
        Me.txtID.MaxLength = 9
        Me.txtID.Name = "txtID"
        Me.txtID.Size = New System.Drawing.Size(80, 23)
        Me.txtID.TabIndex = 0
        '
        'lblPWD
        '
        Me.lblPWD.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblPWD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPWD.Location = New System.Drawing.Point(356, 256)
        Me.lblPWD.Name = "lblPWD"
        Me.lblPWD.Size = New System.Drawing.Size(160, 23)
        Me.lblPWD.TabIndex = 31
        Me.lblPWD.Text = "パスワード"
        Me.lblPWD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblID
        '
        Me.lblID.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblID.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblID.Location = New System.Drawing.Point(356, 208)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(160, 23)
        Me.lblID.TabIndex = 30
        Me.lblID.Text = "ＩＤコード"
        Me.lblID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnEnd
        '
        Me.btnEnd.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(532, 336)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(145, 48)
        Me.btnEnd.TabIndex = 3
        Me.btnEnd.Text = "中　止"
        Me.btnEnd.UseVisualStyleBackColor = False
        '
        'btnLogin
        '
        Me.btnLogin.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnLogin.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnLogin.Location = New System.Drawing.Point(316, 336)
        Me.btnLogin.Name = "btnLogin"
        Me.btnLogin.Size = New System.Drawing.Size(145, 48)
        Me.btnLogin.TabIndex = 2
        Me.btnLogin.Text = "ログイン"
        Me.btnLogin.UseVisualStyleBackColor = False
        '
        'FrmLogin
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmLogin"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlBodyBase.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "フォームロード"

    ''' <summary>フォームロード</summary>
    '''  <remarks>
    ''' フォームロード
    ''' </remarks>
    Private Sub FrmLogin_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Log.Info("Method started.")

        '画面タイトル
        lblTitle.Text = "ログイン"

        '状態保存ファイルから前回成功に登録されたＩＤコードを取得する。
        txtID.Text = getLstUsrID()

        '初期設定ファイルからロックアウト・ログイン試行回数を取得する。
        nLockout = Config.MaxInvalidPasswordAttempts

        Log.Info("Method ended.")
    End Sub
#End Region

#Region "ボタンクリック"

    ''' <summary>「ログイン」ボタン押下</summary>
    '''  <remarks>
    ''' 「ログイン」ボタン押下
    ''' </remarks>
    Private Sub btnLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        Call waitCursor(True)
        LogOperation(sender, e)    'ボタン押下ログ

        Dim sUsrID As String = ""   '入力されたＩＤコード
        Dim sPwd As String = ""     '入力されたパスワード
        Dim sDBPwd As String = ""   'DBから検索したパスワード
        Dim sLockSts As String = ""
        Dim dt As DataTable
        sUsrID = txtID.Text
        sPwd = txtPWD.Text

        Try
            '入力されたＩＤコードを検索
            dt = getData(sUsrID)
            If dt Is Nothing Then
                sLstUID = sUsrID
                nTimes = 1
                Exit Sub
            End If

            '入力されたＩＤコードに対応する登録データがない場合
            If checkUser(dt) = False Then
                sLstUID = sUsrID
                nTimes = 1
                Exit Sub
            End If

            sDBPwd = dt.Rows(0).Item("PASSWORD").ToString
            sAuth = dt.Rows(0).Item("AUTHORITY_LEVEL").ToString
            sLockSts = dt.Rows(0).Item("LOCK_STS").ToString
            '登録データがロックアウトかどうかをチェックする
            If checkLock(sLockSts) = False Then
                sLstUID = sUsrID
                nTimes = 1
                Exit Sub
            End If
            '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
            FrmBase.DetailSet = New ArrayList
            Dim i As Integer = 0
            For i = 4 To dt.Columns.Count - 1
                FrmBase.DetailSet.Add(dt.Rows(0)(i).ToString())
            Next
            '-------Ver0.1　フェーズ２権限対応 ADD　END-------------

            If sPwd = sDBPwd Then 'ログイン成功
                '状態保存ファイルにＩＤコードを格納する
                setUsrID(sUsrID)
                'ユーザ情報を格納する
                GlobalVariables.UserId = sUsrID
                sLstUID = ""
                nTimes = 1
                'メニューに遷移
                openMenu(sUsrID, sAuth)

            Else 'ログイン失敗
                If sLstUID = sUsrID Then
                    nTimes = nTimes + 1
                Else
                    nTimes = 1 '前回のログインした名称と異なり、回数に１を再度設定する。
                End If

                sLstUID = sUsrID
                Log.Info("ＩＤコードとパスワードが一致しません。")
                AlertBox.Show(Lexis.LoginFailedBecauseThePasswordIsIncorrect)
                txtPWD.Text = ""
                txtPWD.Focus()

                '同じＩＤコードでログインをロックアウト判定回数試みると、ロックアウトする
                If nTimes >= nLockout Then
                    lockID(sUsrID)
                End If
            End If
        Catch ex As DatabaseException
            If ex.TargetSite.Name = "getData" Or ex.TargetSite.Name = "lockID" Then
                'DB接続に失敗しました。
                Log.Error("DB接続に失敗しました。")
                AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Else
                'ログイン処理に失敗しました。
                Log.Error("ログイン処理に失敗しました。")
                AlertBox.Show(Lexis.LoginFailed)
            End If

        Catch ex As Exception
            'ログイン処理に失敗しました。
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.LoginFailed)

        Finally
            dt = Nothing
            sUsrID = Nothing
            sPwd = Nothing
            sDBPwd = Nothing
            sLockSts = Nothing
            Call waitCursor(False)
        End Try

    End Sub

    ''' <summary>「中止」ボタンクリック時 </summary>
    '''  <remarks>
    ''' 「中止」ボタンクリック時
    ''' </remarks>
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()
    End Sub

#End Region

#Region "状態保存ファイルから前回ログインに成功したＩＤコードを取得する。"
    ''' <summary>
    ''' 状態保存ファイルから前回ログインに成功したＩＤコードを取得する。
    ''' </summary>
    ''' <returns>前回ログインされたＩＤコード</returns>
    Private Function getLstUsrID() As String

        Dim sLstUsrID As String = ""

        Try
            sLstUsrID = Constant.GetIni(SECTIONNAME, KEYNAME, Config.CookieFilePath)
            If sLstUsrID Is Nothing Then sLstUsrID = ""
        Catch ex As Exception
            Log.Info("SECTIONNAME :" & SECTIONNAME & "KEYNAME :" & KEYNAME & "FILENAME:" & Config.CookieFilePath)
        End Try

        Return sLstUsrID

    End Function
#End Region

#Region "状態保存ファイルに今回ログインに成功したＩＤコードを書き込む"
    ''' <summary>
    ''' 状態保存ファイルに今回ログインに成功したＩＤコードを書き込む。
    ''' </summary>
    ''' <param name="sUsrID">ＩＤコード</param>
    Private Sub setUsrID(ByVal sUsrID As String)

        Dim bFlg As Boolean = False

        Try
            bFlg = Constant.SetIni(SECTIONNAME, KEYNAME, Config.CookieFilePath, sUsrID)
        Catch ex As Exception
            Log.Info("sectionName :" & SECTIONNAME & "keyName :" & KEYNAME & "FILENAME:" & Config.CookieFilePath & "USERID:" & sUsrID)
        End Try
    End Sub

#End Region

#Region "メニュー画面に渡すべきのユーザの権限値を取得する"

    ''' <summary>メニュー画面に渡すべきのユーザの権限値を取得する。</summary>
    ''' <param name="sNowUID">入力されたＩＤコード</param>
    ''' <returns>取得された権限値、パスワード、ロックのフラグ</returns>
    Private Function getData(ByVal sNowUID As String) As DataTable
        Dim sSql As String = ""
        Dim dt As DataTable
        Dim dbCtl As DatabaseTalker

        'テーブル:IDデータ
        '取得された項目:パスワード
        '取得された項目:権限レベル
        '取得された項目:ロックのフラグ
        '-------Ver0.1　フェーズ２権限対応 MOD　START-----------
        sSql = " SELECT USER_ID,PASSWORD,AUTHORITY_LEVEL,LOCK_STS," _
            & " MST_FUNC1,MST_FUNC2,MST_FUNC3,MST_FUNC4,MST_FUNC5," _
            & " PRG_FUNC1,PRG_FUNC2,PRG_FUNC3,PRG_FUNC4,PRG_FUNC5," _
            & " MNT_FUNC1,MNT_FUNC2,MNT_FUNC3,MNT_FUNC4,MNT_FUNC5,MNT_FUNC6,MNT_FUNC7,MNT_FUNC8,MNT_FUNC9,MNT_FUNC10," _
            & " SYS_FUNC1,SYS_FUNC2,SYS_FUNC3,SYS_FUNC4,SYS_FUNC5 " _
            & " FROM M_USER " _
            & " WHERE USER_ID=" & "'" & sNowUID & "'"
        '-------Ver0.1　フェーズ２権限対応 MOD　END-------------
        dbCtl = New DatabaseTalker
        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSql)
        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw ex
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
            sSql = Nothing
        End Try

        Return dt

    End Function
#End Region

#Region "当該ユーザをロックする"

    ''' <summary> 当該ユーザをロックする。</summary>
    ''' <param name="sNowUID">入力されたＩＤコード</param>
    Private Sub lockID(ByVal sNowUID As String)
        Dim sSql As String = ""
        Dim dbCtl As DatabaseTalker

        sSql = "UPDATE M_USER SET LOCK_STS='1' WHERE USER_ID=" & "'" & sNowUID & "'"
        dbCtl = New DatabaseTalker
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSql)
            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            dbCtl.TransactionRollBack()
            Throw ex
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
            sSql = Nothing
        End Try
    End Sub

#End Region

#Region "メニュー画面に遷移"

    ''' <summary>メニュー画面に遷移</summary>
    ''' <param name="sUsrID">ＩＤコード</param>
    ''' <param name="sAuth">権限</param>
    Private Sub openMenu(ByVal sUsrID As String, ByVal sAuth As String)
        '運用管理メニュー画面に値を引き渡す。
        FrmBase.Authority = sAuth
        'メニュー画面を表示する
        Dim hFrmMainMenu As New FrmMainMenu
        Me.Hide()
        hFrmMainMenu.ShowDialog()
        hFrmMainMenu.Dispose()
        GlobalVariables.UserId = ""
        txtPWD.Text = ""
        Me.Show()
        txtPWD.Focus()

    End Sub
#End Region

#Region "チェック"
    ''' <summary>ロックアウトチェックする。</summary>
    ''' <param name="sLockSts">ロックアウト状態</param>
    ''' <returns>ロックアウト場合、falseを返す。ではなければ、trueを返す。</returns>
    Private Function checkLock(ByVal sLockSts As String) As Boolean
        Dim bRet As Boolean = True

        'ユーザをロックかどうかをチェックする
        If sLockSts = "1" Then
            Log.Info("ＩＤコードがロックアウトされています。")
            AlertBox.Show(Lexis.LoginFailedBecauseTheIdCodeHasBeenLockedOut)

            txtPWD.Text = ""
            txtID.Focus()
            bRet = False
        End If
        Return bRet

    End Function

    ''' <summary>ＩＤコードをチェックする。</summary>
    ''' <param name="dt">検索したＩＤコード</param>
    ''' <returns>ＩＤコードがないの場合、falseを返す。ではなければ、trueを返す。</returns>
    Private Function checkUser(ByVal dt As DataTable) As Boolean
        Dim nCount As Integer = 0
        Dim bRet As Boolean = True
        nCount = dt.Rows.Count

        If nCount = 0 Then
            Log.Info("ログインされたＩＤコードは登録されていません。")
            AlertBox.Show(Lexis.LoginFailedBecauseTheIdCodeIsIncorrect)
            txtPWD.Text = ""
            txtID.Focus()
            bRet = False
        End If
        Return bRet

    End Function
#End Region

End Class