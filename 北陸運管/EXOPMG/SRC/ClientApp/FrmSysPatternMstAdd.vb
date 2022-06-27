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
Imports System.Data.SqlClient
Imports System.Text

''' <summary>パターン登録</summary>
''' <remarks>
''' パターン名称を入力し、「登録」ボタンをクリックすることにより、
''' 設定内容を運用管理サーバに登録する。
''' </remarks>
Public Class FrmSysPatternMstAdd
    Inherits System.Windows.Forms.Form

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

    End Sub

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnInsert As System.Windows.Forms.Button
    Friend WithEvents lblPtnNameTitle As System.Windows.Forms.Label
    Friend WithEvents lblPtnNoTitle As System.Windows.Forms.Label
    Friend WithEvents txtPatternname As System.Windows.Forms.TextBox
    Friend WithEvents txtPatternno As System.Windows.Forms.TextBox
    Friend WithEvents pnlPtnAdd As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.lblPtnNameTitle = New System.Windows.Forms.Label()
        Me.lblPtnNoTitle = New System.Windows.Forms.Label()
        Me.txtPatternname = New System.Windows.Forms.TextBox()
        Me.txtPatternno = New System.Windows.Forms.TextBox()
        Me.pnlPtnAdd = New System.Windows.Forms.Panel()
        Me.pnlPtnAdd.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnStop
        '
        Me.btnStop.BackColor = System.Drawing.Color.Silver
        Me.btnStop.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnStop.Location = New System.Drawing.Point(426, 255)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(90, 32)
        Me.btnStop.TabIndex = 4
        Me.btnStop.Text = "終　了"
        Me.btnStop.UseVisualStyleBackColor = False
        '
        'btnInsert
        '
        Me.btnInsert.BackColor = System.Drawing.Color.Silver
        Me.btnInsert.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnInsert.Location = New System.Drawing.Point(426, 116)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(90, 32)
        Me.btnInsert.TabIndex = 3
        Me.btnInsert.Text = "登  録"
        Me.btnInsert.UseVisualStyleBackColor = False
        '
        'lblPtnNameTitle
        '
        Me.lblPtnNameTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNameTitle.Location = New System.Drawing.Point(53, 261)
        Me.lblPtnNameTitle.Name = "lblPtnNameTitle"
        Me.lblPtnNameTitle.Size = New System.Drawing.Size(110, 21)
        Me.lblPtnNameTitle.TabIndex = 4
        Me.lblPtnNameTitle.Text = "パターン名称"
        Me.lblPtnNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnNoTitle
        '
        Me.lblPtnNoTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblPtnNoTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNoTitle.Location = New System.Drawing.Point(53, 121)
        Me.lblPtnNoTitle.Name = "lblPtnNoTitle"
        Me.lblPtnNoTitle.Size = New System.Drawing.Size(110, 21)
        Me.lblPtnNoTitle.TabIndex = 0
        Me.lblPtnNoTitle.Text = "パターンNo"
        Me.lblPtnNoTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtPatternname
        '
        Me.txtPatternname.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPatternname.Location = New System.Drawing.Point(165, 261)
        Me.txtPatternname.MaxLength = 10
        Me.txtPatternname.Name = "txtPatternname"
        Me.txtPatternname.Size = New System.Drawing.Size(170, 22)
        Me.txtPatternname.TabIndex = 2
        '
        'txtPatternno
        '
        Me.txtPatternno.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPatternno.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtPatternno.Location = New System.Drawing.Point(165, 121)
        Me.txtPatternno.MaxLength = 2
        Me.txtPatternno.Name = "txtPatternno"
        Me.txtPatternno.Size = New System.Drawing.Size(30, 22)
        Me.txtPatternno.TabIndex = 1
        '
        'pnlPtnAdd
        '
        Me.pnlPtnAdd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlPtnAdd.Controls.Add(Me.lblPtnNoTitle)
        Me.pnlPtnAdd.Controls.Add(Me.btnStop)
        Me.pnlPtnAdd.Controls.Add(Me.txtPatternname)
        Me.pnlPtnAdd.Controls.Add(Me.btnInsert)
        Me.pnlPtnAdd.Controls.Add(Me.txtPatternno)
        Me.pnlPtnAdd.Controls.Add(Me.lblPtnNameTitle)
        Me.pnlPtnAdd.Location = New System.Drawing.Point(0, 0)
        Me.pnlPtnAdd.Name = "pnlPtnAdd"
        Me.pnlPtnAdd.Size = New System.Drawing.Size(594, 418)
        Me.pnlPtnAdd.TabIndex = 0
        '
        'FrmSysPatternMstAdd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlPtnAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysPatternMstAdd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "パターン登録"
        Me.pnlPtnAdd.ResumeLayout(False)
        Me.pnlPtnAdd.PerformLayout()
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

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly FormTitle As String = "パターン設定登録"

    '登録ユーザのIDを取得する。
    Private sLoginID As String = ""
#End Region

#Region "宣言領域（Public）"
    Public Property LoginID() As String
        Get
            Return sLoginID
        End Get
        Set(ByVal value As String)
            sLoginID = value
        End Set
    End Property

    'マスタ種別を取得する。
    Private sKind As String = ""

    Public Property Kind() As String
        Get
            Return sKind
        End Get
        Set(ByVal value As String)
            sKind = value
        End Set
    End Property

    '検索条件のフラグを取得する。
    Private bMstChecked As Boolean = False

    Public Property CheckFlag() As Boolean
        Get
            Return bMstChecked
        End Get
        Set(ByVal value As Boolean)
            bMstChecked = value
        End Set
    End Property

    '機種コードを取得する
    Private sModelCode As String = ""

    Public Property ModelCode() As String
        Get
            Return sModelCode
        End Get
        Set(ByVal value As String)
            sModelCode = value
        End Set
    End Property

    '機種タイプを取得する
    Private sMachType As String = ""

    Public Property MachType() As String
        Get
            Return sMachType
        End Get
        Set(ByVal value As String)
            sMachType = value
        End Set
    End Property
#End Region

#Region "メソッド（Public）"

    ''' <summary>
    ''' ローディング　メインウィンドウ
    ''' </summary>
    Private Sub FrmSysPatternMstAdd_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim bRtn As Boolean = False
        LbEventStop = True

        Try
            Log.Info("Method started.")

            '画面背景色（BackColor）を設定する
            pnlPtnAdd.BackColor = Config.BackgroundColor
            lblPtnNameTitle.BackColor = Config.BackgroundColor
            lblPtnNoTitle.BackColor = Config.BackgroundColor

            'ボタン背景色（BackColor）を設定する
            btnInsert.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor
            Me.txtPatternname.ImeMode = Windows.Forms.ImeMode.Hiragana

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method proc ended.")
            Else
                Log.Error("Method proc abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If

            LbEventStop = False 'イベント発生ＯＮ
        End Try
    End Sub
#End Region

#Region "イベント"
    ''' <summary>
    ''' 「登録」ボタンを押下すると、DBへ新しいパターンが登録される
    ''' </summary>
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click
        Try
            '登録ボタン押下。
            FrmBase.LogOperation(sender, e, Me.Text)
            If CheckAll() Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyInsert).Equals(System.Windows.Forms.DialogResult.Yes) Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked, Text)     'Yesボタン押下
                    Call waitCursor(True)

                    If addPattern() > 0 Then
                        '登録処理が正常に終了しました。
                        Log.Info("Insert finished.")
                        If AlertBox.Show(AlertBoxAttr.OK, Lexis.InsertCompleted).Equals(System.Windows.Forms.DialogResult.OK) Then
                            FrmBase.LogOperation(Lexis.OkButtonClicked, Text)
                            Me.Close()
                        End If
                    End If
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked, Text)
                    btnInsert.Select()
                End If
            End If

        Catch ex As Exception

            Log.Fatal("Unwelcome Exception caught.", ex)  '予期せぬエラーが発生しました。
            AlertBox.Show(Lexis.InsertFailed)
            btnInsert.Select()
            Exit Sub
        Finally
            Call waitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 「終了」ボタンを押下すると、本画面が終了される。 
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        '終了ボタン押下。
        FrmBase.LogOperation(sender, e, Me.Text)
        Me.Close()
    End Sub

    ''' <summary>「パターンNo」の入力値が制限する</summary>
    Private Sub txtPtnNo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPatternno.KeyPress
        Select Case e.KeyChar
            Case "0".ToCharArray To "9".ToCharArray
            Case Chr(8)
            Case Else
                e.Handled = True
        End Select
    End Sub

    ''' <summary>「パターン名称」の入力値が制限する</summary>
    Private Sub txtPtnName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPatternname.KeyPress

        Dim Encode As Encoding
        Encode = Encoding.GetEncoding("Shift_JIS")

        If e.KeyChar.ToString.Length = Encode.GetByteCount(e.KeyChar.ToString) / 2 Then
            e.Handled = False
        ElseIf e.KeyChar = Chr(8) Then
            e.Handled = False
        Else
            e.Handled = True
        End If

    End Sub
#End Region

#Region "メソッド（Private）"
    ''' <summary>
    ''' 登録」ボタンを押下した際にすべてのコントロールの値をチェックする。
    ''' </summary>
    ''' <remarks>データ合法フラグ</remarks>
    Private Function CheckAll() As Boolean
        '当関数の戻り値
        Dim bFlag As Boolean = False
        If System.String.IsNullOrEmpty(Me.txtPatternno.Text) Then
            '入力値が不正です。パターンNoの値がヌルである。
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblPtnNoTitle.Text)
            Me.txtPatternno.Focus()
        ElseIf Me.txtPatternno.Text.Length <> 2 Then
            '入力値が不正です。パターンNoの長さが2文字でない。
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternNo)
            Me.txtPatternno.Focus()
        ElseIf CheckIsExist(Me.txtPatternno.Text) Then
            '入力値が不正です。パターンNoXXは既に登録されています。
            AlertBox.Show(Lexis.ThePatternNoAlreadyExists, Me.txtPatternno.Text)
            Me.txtPatternno.Focus()
        ElseIf System.String.IsNullOrEmpty(Me.txtPatternname.Text) Then
            '入力値が不正です。パターン名称の値がヌルである。
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblPtnNameTitle.Text)
            Me.txtPatternname.Focus()
        ElseIf OPMGUtility.CheckString(Me.txtPatternname.Text.ToString, 10, 2, True) = -4 Then
            '入力値が不正です。
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternName)
            Me.txtPatternname.Focus()
        ElseIf Me.txtPatternname.Text.ToString.Trim() = "" Then
            '入力値が不正です。
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternName)
            Me.txtPatternname.Focus()
        ElseIf CheckMachKennsu(sModelCode) = True Then
            '件数を超えています
            AlertBox.Show(Lexis.PatternNoIsFull)
            Me.txtPatternno.Focus()
        Else
            bFlag = True
        End If

        Return bFlag
    End Function
    ''' <summary>
    ''' 機種単位最大件数チェック
    ''' </summary>
    ''' <returns>true:件数を超えています。false:件数を超えしない。</returns>
    Private Function CheckMachKennsu(ByVal sModelCode As String) As Boolean
        Dim Flag As Boolean = False
        Dim sBuilder As New StringBuilder
        Dim dbCtl As DatabaseTalker = New DatabaseTalker
        Dim Kennsu As Integer

        Try
            sBuilder.AppendLine("SELECT COUNT(1) FROM M_PATTERN_DATA WHERE MODEL_CODE= " + Utility.SetSglQuot(sModelCode))
            dbCtl.ConnectOpen()
            Kennsu = CInt(dbCtl.ExecuteSQLToReadScalar(sBuilder.ToString))

            If Kennsu > 100 Then
                Flag = True
            Else
                Flag = False
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)  '予期せぬエラーが発生しました。
            Flag = False
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
        Return Flag
    End Function

    ''' <summary>
    ''' パターンNoの重複チェック
    ''' </summary>
    ''' <returns>true:パターンNoが重複です。false:パターンNoがの重複しない。</returns>
    Private Function CheckIsExist(ByVal PatternNo As String) As Boolean
        Dim Flag As Boolean = False
        Dim sBuilder As New StringBuilder
        Dim dtMstTable As DataTable = New DataTable
        Dim dbCtl As DatabaseTalker = New DatabaseTalker
        Dim iNum As Integer
        Try
            sBuilder.AppendLine(String.Format("SELECT COUNT(1) FROM M_PATTERN_DATA WHERE PATTERN_NO = {0} AND MODEL_CODE={1} AND MST_KIND={2}", _
                                                     Utility.SetSglQuot(txtPatternno.Text), Utility.SetSglQuot(sModelCode), Utility.SetSglQuot(sKind)))

            dbCtl.ConnectOpen()
            iNum = CInt(dbCtl.ExecuteSQLToReadScalar(sBuilder.ToString))
            If iNum = 1 Then
                Flag = True
            Else
                Flag = False
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)  '予期せぬエラーが発生しました。
            Flag = False
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
        Return Flag
    End Function

    ''' <summary>
    ''' DBへ設定されたパターン情報をインサートする。
    ''' </summary>
    Private Function addPattern() As Integer

        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker
        Dim iRetrun As Integer
        'パターンNo、パターン名称を取得する。
        Dim sPatternNo As String = txtPatternno.Text
        Dim sPatternName As String = txtPatternname.Text
        Dim sClient As String

        dbCtl = New DatabaseTalker
        Try
            '操作者IDを取得する。
            sLoginID = GlobalVariables.UserId
            sClient = Config.MachineName

            sSQL = " INSERT INTO M_PATTERN_DATA(" _
                     & " INSERT_DATE," _
                     & " INSERT_USER_ID," _
                     & " INSERT_MACHINE_ID," _
                     & " UPDATE_DATE," _
                     & " UPDATE_USER_ID, " _
                     & " UPDATE_MACHINE_ID, " _
                     & " MODEL_CODE," _
                     & " MST_KIND," _
                     & " PATTERN_NO," _
                     & " PATTERN_NAME)" _
                     & " VALUES(GETDATE()," _
                     & Utility.SetSglQuot(sLoginID) & "," _
                     & Utility.SetSglQuot(sClient) & "," _
                     & "GETDATE()," _
                     & Utility.SetSglQuot(sLoginID) & "," _
                     & Utility.SetSglQuot(sClient) & "," _
                     & Utility.SetSglQuot(sModelCode) & "," _
                     & Utility.SetSglQuot(sKind) & "," _
                     & Utility.SetSglQuot(sPatternNo) & "," _
                     & Utility.SetSglQuot(sPatternName) & ")"

            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            iRetrun = dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException

            '既に登録されています。
            If TypeOf ex.InnerException Is SqlException Then
                If (CType(ex.InnerException, SqlException).Number = 2627) Then
                    Call waitCursor(False)
                    Me.txtPatternno.Focus()

                End If
            End If
            dbCtl.TransactionRollBack()
            'DB接続に失敗
            Log.Fatal("Unwelcome Exception caught.", ex)   '登録処理に失敗しました。
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnInsert.Select()
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try
        Return iRetrun
    End Function
#End Region

#Region "カーソル待ち"

    ''' <summary>
    ''' カーソル待ち
    ''' </summary>
    ''' <param name="bWait">true:待ち開始　false:待ち終了</param>
    ''' <remarks>カーソルが砂時計になる</remarks>
    Private Sub waitCursor(Optional ByVal bWait As Boolean = True)

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
