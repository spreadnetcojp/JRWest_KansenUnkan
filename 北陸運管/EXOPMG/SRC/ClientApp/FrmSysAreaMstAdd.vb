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

''' <summary>エリア登録</summary>
''' <remarks>
''' エリア名称を入力し、「登録」ボタンをクリックすることにより、
''' 設定内容を運用管理サーバに登録する。
''' </remarks>
Public Class FrmSysAreaMstAdd
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
    Friend WithEvents lblAreaname As System.Windows.Forms.Label
    Friend WithEvents lblAreano As System.Windows.Forms.Label
    Friend WithEvents txtAreaname As System.Windows.Forms.TextBox
    Friend WithEvents txtAreano As System.Windows.Forms.TextBox
    Friend WithEvents pnlPtnAdd As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.lblAreaname = New System.Windows.Forms.Label()
        Me.lblAreano = New System.Windows.Forms.Label()
        Me.txtAreaname = New System.Windows.Forms.TextBox()
        Me.txtAreano = New System.Windows.Forms.TextBox()
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
        Me.btnStop.TabIndex = 3
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
        Me.btnInsert.TabIndex = 2
        Me.btnInsert.Text = "登  録"
        Me.btnInsert.UseVisualStyleBackColor = False
        '
        'lblAreaname
        '
        Me.lblAreaname.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAreaname.Location = New System.Drawing.Point(53, 261)
        Me.lblAreaname.Name = "lblAreaname"
        Me.lblAreaname.Size = New System.Drawing.Size(110, 21)
        Me.lblAreaname.TabIndex = 4
        Me.lblAreaname.Text = "エリア名称"
        Me.lblAreaname.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAreano
        '
        Me.lblAreano.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblAreano.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAreano.Location = New System.Drawing.Point(53, 121)
        Me.lblAreano.Name = "lblAreano"
        Me.lblAreano.Size = New System.Drawing.Size(110, 21)
        Me.lblAreano.TabIndex = 0
        Me.lblAreano.Text = "エリアNo"
        Me.lblAreano.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtAreaname
        '
        Me.txtAreaname.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtAreaname.Location = New System.Drawing.Point(165, 261)
        Me.txtAreaname.MaxLength = 10
        Me.txtAreaname.Name = "txtAreaname"
        Me.txtAreaname.Size = New System.Drawing.Size(170, 22)
        Me.txtAreaname.TabIndex = 1
        '
        'txtAreano
        '
        Me.txtAreano.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtAreano.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtAreano.Location = New System.Drawing.Point(165, 121)
        Me.txtAreano.MaxLength = 2
        Me.txtAreano.Name = "txtAreano"
        Me.txtAreano.Size = New System.Drawing.Size(30, 22)
        Me.txtAreano.TabIndex = 0
        '
        'pnlPtnAdd
        '
        Me.pnlPtnAdd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlPtnAdd.Controls.Add(Me.lblAreano)
        Me.pnlPtnAdd.Controls.Add(Me.btnStop)
        Me.pnlPtnAdd.Controls.Add(Me.txtAreaname)
        Me.pnlPtnAdd.Controls.Add(Me.btnInsert)
        Me.pnlPtnAdd.Controls.Add(Me.txtAreano)
        Me.pnlPtnAdd.Controls.Add(Me.lblAreaname)
        Me.pnlPtnAdd.Location = New System.Drawing.Point(0, 0)
        Me.pnlPtnAdd.Name = "pnlPtnAdd"
        Me.pnlPtnAdd.Size = New System.Drawing.Size(594, 418)
        Me.pnlPtnAdd.TabIndex = 0
        '
        'FrmSysAreaMstAdd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlPtnAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysAreaMstAdd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "エリア登録"
        Me.pnlPtnAdd.ResumeLayout(False)
        Me.pnlPtnAdd.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "宣言領域（Private）"

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    '''登録ユーザのIDを取得する。
    ''' </summary>
    Private sLoginID As String = ""

    ''' <summary>
    '''機種コードを取得する
    ''' </summary>
    Private sModelCode As String = ""

    Public Property ModelCode() As String
        Get
            Return sModelCode
        End Get
        Set(ByVal value As String)
            sModelCode = value
        End Set
    End Property
#End Region

#Region "イベント"

    ''' <summary>
    ''' ローディング　メインウィンドウ
    ''' </summary>
    Private Sub FrmSysAreaMstAdd_load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim bRtn As Boolean = False
        LbEventStop = True      'イベント発生ＯＦＦ

        Try
            Log.Info("Method started.")

            '画面背景色（BackColor）を設定する
            pnlPtnAdd.BackColor = Config.BackgroundColor
            lblAreaname.BackColor = Config.BackgroundColor
            lblAreano.BackColor = Config.BackgroundColor

            'ボタン背景色（BackColor）を設定する
            btnInsert.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor
            Me.txtAreaname.ImeMode = Windows.Forms.ImeMode.Hiragana

            '操作者IDを取得する
            sLoginID = GlobalVariables.UserId

            Me.txtAreano.Focus()

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("The form proc ended.")
            Else
                Log.Error("The form proc abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If

            LbEventStop = False 'イベント発生ＯＮ
        End Try

    End Sub
    ''' <summary>
    ''' 「登録」ボタンを押下すると、DBへ新しいエリアが登録される
    ''' </summary>
    Private Sub btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click

        Try
            '登録ボタン押下。
            FrmBase.LogOperation(sender, e, Text)
            If CheckAll() Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyInsert).Equals(Windows.Forms.DialogResult.Yes) Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked, Text)                      'はいボタン押下
                    Call WaitCursor(True)
                    Call AddArea()
                    FrmBase.LogOperation(Lexis.InsertCompleted, Text) 'TODO: 少なくとも「操作」ログではない。詳細設計も含め確認。 '登録処理が正常に終了しました。
                    AlertBox.Show(Lexis.InsertCompleted)
                    FrmBase.LogOperation(Lexis.OkButtonClicked, Text)                       'OKボタン押下
                    Me.Close()
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked, Text)                       'いいえボタン押下
                    btnInsert.Select()
                End If
            End If

        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnInsert.Select()
            Exit Sub

        Catch ex As Exception

            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.InsertFailed)      '登録が失敗
            btnInsert.Select()
            Exit Sub
        Finally

            Call WaitCursor(False)

        End Try

    End Sub

    ''' <summary>
    ''' 「終了」ボタンを押下すると、本画面が終了される。 
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        '終了ボタン押下。
        FrmBase.LogOperation(sender, e, Text)
        Me.Close()
    End Sub

    ''' <summary>
    ''' 「エリアNo」の入力値が制限する
    ''' </summary>
    Private Sub txtAreaNo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtAreano.KeyPress
        Select Case e.KeyChar
            Case "0".ToCharArray To "9".ToCharArray
            Case Chr(8)
            Case Else
                e.Handled = True
        End Select
    End Sub

    ''' <summary>
    ''' 「エリア名称」の入力値が制限する
    ''' </summary>
    Private Sub txtAreaName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtAreaname.KeyPress

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
    ''' 「登録」ボタンを押下した際にすべてのコントロールの値をチェックする。
    ''' </summary>
    ''' <remarks>データ合法フラグ</remarks>
    Private Function CheckAll() As Boolean
        '当関数の戻り値
        Dim bRetAll As Boolean = False

        If System.String.IsNullOrEmpty(Me.txtAreano.Text) Then
            '入力値が不正です。エリアNoの値がヌルである。
            AlertBox.Show(Lexis.InputParameterIsIncomplete, Me.lblAreano.Text)
            Me.txtAreano.Focus()
        ElseIf Me.txtAreano.Text.Length <> 2 Then
            '入力値が不正です。エリアNoの長さが2文字でない。
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaNo)
            Me.txtAreano.Focus()
        ElseIf CheckIsExist(Me.txtAreano.Text) Then
            '入力値が不正です。エリアNoXXは既に登録されています。
            AlertBox.Show(Lexis.TheAreaNoAlreadyExists, Me.txtAreano.Text)
            Me.txtAreano.Focus()
        ElseIf System.String.IsNullOrEmpty(Me.txtAreaname.Text) Then
            '入力値が不正です。エリア名称の値がヌルである。
            AlertBox.Show(Lexis.InputParameterIsIncomplete, Me.lblAreaname.Text)
            Me.txtAreaname.Focus()
        ElseIf OPMGUtility.CheckString(Me.txtAreaname.Text.ToString, 10, 2, True) = -4 Then
            '入力値が不正です。
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaName)
            Me.txtAreaname.Focus()
        ElseIf Me.txtAreaname.Text.ToString.Trim() = "" Then
            '入力値が不正です。
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaName)
            Me.txtAreaname.Focus()
        ElseIf CheckAreaCount() Then
            '機種単位で登録できるエリア件数を超えています。
            AlertBox.Show(Lexis.AreaNoIsFull)
            Me.txtAreano.Focus()
        Else
            bRetAll = True
        End If

        Return bRetAll
    End Function

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

    ''' <summary>
    ''' DBへ設定されたエリア情報をインサートする。
    ''' </summary>
    Private Sub AddArea()

        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker
        Dim sBuilder As New StringBuilder

        'エリアNo、エリア名称を取得する。
        Dim sAreanNo As String = txtAreano.Text
        Dim sAreaName As String = txtAreaname.Text

        '端末ID
        Dim sClient As String = Config.MachineName
        dbCtl = New DatabaseTalker

        Try
            sBuilder.AppendLine(" INSERT INTO M_AREA_DATA (")
            sBuilder.AppendLine(" INSERT_DATE,")
            sBuilder.AppendLine(" INSERT_USER_ID,")
            sBuilder.AppendLine(" INSERT_MACHINE_ID,")
            sBuilder.AppendLine(" UPDATE_DATE,")
            sBuilder.AppendLine(" UPDATE_USER_ID,")
            sBuilder.AppendLine(" UPDATE_MACHINE_ID,")
            sBuilder.AppendLine(" MODEL_CODE,")
            sBuilder.AppendLine(" AREA_NO,")
            sBuilder.AppendLine(" AREA_NAME)")
            sBuilder.AppendLine(" VALUES(GETDATE(),")
            sBuilder.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine("GETDATE(),")
            sBuilder.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sModelCode) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sAreanNo) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sAreaName) & ")")
            sSQL = sBuilder.ToString

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException
            btnInsert.Select()
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

    End Sub

    ''' <summary>
    ''' エリアNoの重複チェック
    ''' </summary>
    ''' <returns>true:エリアNoが重複です。false:エリアNがの重複しない。</returns>
    Private Function CheckIsExist(ByVal sAreaNo As String) As Boolean
        Dim Flag As Boolean = False
        Dim sSQL As String = ""
        Dim nRtn As Integer
        Dim dtMstTable As New DataTable
        Try
            sSQL = String.Format("SELECT COUNT(1) FROM M_AREA_DATA WHERE AREA_NO = {0} AND MODEL_CODE = {1}", _
                                 Utility.SetSglQuot(txtAreano.Text), Utility.SetSglQuot(sModelCode))

            nRtn = FrmBase.BaseSqlDataTableFill(sSQL, dtMstTable)

            If nRtn = -9 Then
                Throw New OPMGException()
            End If

            If Convert.ToInt64(dtMstTable.Rows(0)(0)) = 1 Then
                Flag = True
            Else
                Flag = False
            End If

        Catch ex As OPMGException
            Throw New OPMGException(ex)
        End Try
        Return Flag
    End Function

    ''' <summary>
    ''' 登録できる最大エリア数チェック
    ''' </summary>
    ''' <returns>true:エリア件数を超えています。false:エリア件数を超えない。</returns>
    Private Function CheckAreaCount() As Boolean
        Dim Flag As Boolean = False
        Dim sSQL As String = ""
        Dim nRtn As Integer
        Dim dtMstTable As New DataTable
        Try
            sSQL = String.Format("SELECT COUNT(1) FROM M_AREA_DATA WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModelCode))
            nRtn = FrmBase.BaseSqlDataTableFill(sSQL, dtMstTable)

            If nRtn = -9 Then
                Throw New OPMGException()
            End If

            If Convert.ToInt64(dtMstTable.Rows(0)(0)) >= 10 Then
                Flag = True
            End If

        Catch ex As OPMGException
            Throw New OPMGException(ex)
        End Try
        Return Flag
    End Function

#End Region

End Class
