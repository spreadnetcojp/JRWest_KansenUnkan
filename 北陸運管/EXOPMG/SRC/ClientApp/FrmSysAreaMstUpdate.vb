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
Imports System.Text

''' <summary>エリア修正</summary>
''' <remarks>
''' エリア名称を変更し、「修正」ボタンをクリックすることにより、
''' 設定内容を運用管理サーバに登録する。
''' </remarks>
Public Class FrmSysAreaMstUpdate
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
    Friend WithEvents txtAreaname As System.Windows.Forms.TextBox
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents txtAreano As System.Windows.Forms.Label
    Friend WithEvents lblAreaname As System.Windows.Forms.Label
    Friend WithEvents lblAreano As System.Windows.Forms.Label
    Friend WithEvents pnlAreaUpdate As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtAreaname = New System.Windows.Forms.TextBox()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.txtAreano = New System.Windows.Forms.Label()
        Me.lblAreaname = New System.Windows.Forms.Label()
        Me.lblAreano = New System.Windows.Forms.Label()
        Me.pnlAreaUpdate = New System.Windows.Forms.Panel()
        Me.pnlAreaUpdate.SuspendLayout()
        Me.SuspendLayout()
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
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.Color.Silver
        Me.btnUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(426, 116)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(90, 32)
        Me.btnUpdate.TabIndex = 2
        Me.btnUpdate.Text = "修  正"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'txtAreano
        '
        Me.txtAreano.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtAreano.Location = New System.Drawing.Point(165, 121)
        Me.txtAreano.Name = "txtAreano"
        Me.txtAreano.Size = New System.Drawing.Size(50, 21)
        Me.txtAreano.TabIndex = 4
        Me.txtAreano.Text = "XX"
        Me.txtAreano.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAreaname
        '
        Me.lblAreaname.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAreaname.Location = New System.Drawing.Point(53, 261)
        Me.lblAreaname.Name = "lblAreaname"
        Me.lblAreaname.Size = New System.Drawing.Size(110, 21)
        Me.lblAreaname.TabIndex = 0
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
        Me.lblAreano.TabIndex = 3
        Me.lblAreano.Text = "エリアNo"
        Me.lblAreano.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlAreaUpdate
        '
        Me.pnlAreaUpdate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlAreaUpdate.Controls.Add(Me.lblAreaname)
        Me.pnlAreaUpdate.Controls.Add(Me.btnStop)
        Me.pnlAreaUpdate.Controls.Add(Me.txtAreaname)
        Me.pnlAreaUpdate.Controls.Add(Me.btnUpdate)
        Me.pnlAreaUpdate.Controls.Add(Me.lblAreano)
        Me.pnlAreaUpdate.Controls.Add(Me.txtAreano)
        Me.pnlAreaUpdate.Location = New System.Drawing.Point(0, 0)
        Me.pnlAreaUpdate.Name = "pnlAreaUpdate"
        Me.pnlAreaUpdate.Size = New System.Drawing.Size(594, 418)
        Me.pnlAreaUpdate.TabIndex = 0
        '
        'FrmSysAreaMstUpdate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlAreaUpdate)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysAreaMstUpdate"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "エリア修正"
        Me.pnlAreaUpdate.ResumeLayout(False)
        Me.pnlAreaUpdate.PerformLayout()
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

    '登録ユーザのIDを取得する。
    Private sLoginID As String = ""

    'エリアNoを取得する。
    Private sAreaNo As String = ""

    Public Property AreaNo() As String
        Get
            Return sAreaNo
        End Get
        Set(ByVal value As String)
            sAreaNo = value
        End Set
    End Property

    'エリア名称を取得する。
    Private sAreaName As String = ""

    '機器種別を取得する。
    Private sModelCode As String = ""

    Public Property ModelCode() As String
        Get
            Return sModelCode
        End Get
        Set(ByVal value As String)
            sModelCode = value
        End Set
    End Property

    '更新日時
    Private oldDate As String = ""

    '更新日時
    Private newDate As String = ""

#End Region

#Region "メソッド（Public）"

    ''' <summary>エリア修正画面のデータを準備する</summary>
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean
        Dim bRet As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim dt As New DataTable
        Dim nRtn As Integer
        Dim sSql As String = ""

        Try
            Log.Info("Method started.")

            '操作者IDを取得する
            sLoginID = GlobalVariables.UserId

            'データを取得する。
            sSql = LfGetSelectString()
            nRtn = FrmBase.BaseSqlDataTableFill(sSql, dt)
            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    Return bRet
                Case Else
                    If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                        '検索条件に一致するデータは存在しません。
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Return bRet
                    Else
                        sAreaName = dt.Rows(0).Item("AREA_NAME").ToString
                        oldDate = dt.Rows(0).Item("UPDATE_DATE").ToString
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
    Private Sub FrmSysAreaMstUpdate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrmData() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            '画面背景色（BackColor）を設定する
            pnlAreaUpdate.BackColor = Config.BackgroundColor
            lblAreaname.BackColor = Config.BackgroundColor
            lblAreano.BackColor = Config.BackgroundColor
            txtAreano.BackColor = Config.BackgroundColor

            'ボタン背景色（BackColor）を設定する
            btnUpdate.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor

            'エリアNoを表示する。
            Me.txtAreano.Text = sAreaNo
            'エリア名称を表示する。
            Me.txtAreaname.Text = sAreaName

            Me.txtAreaname.ImeMode = Windows.Forms.ImeMode.Hiragana
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' 「修正」ボタンを押下すると、DBへ設定されたエリア情報を修正する。
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer

        Try
            LbEventStop = True
            '修正ボタンを押下
            FrmBase.LogOperation(sender, e, Me.Text)

            If CheckAll() = True Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUpdate).Equals(System.Windows.Forms.DialogResult.Yes) Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked, Me.Text)
                    Call WaitCursor(True)
                    'データを取得する。
                    sSql = LfGetSelectString()
                    nRtn = FrmBase.BaseSqlDataTableFill(sSql, dt)
                    Select Case nRtn
                        Case -9             'ＤＢオープンエラー
                            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                            Exit Sub
                        Case Else
                            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                                '検索条件に一致するデータは存在しません。
                                AlertBox.Show(Lexis.CompetitiveOperationDetected)
                                Exit Sub
                            Else
                                newDate = dt.Rows(0).Item("UPDATE_DATE").ToString
                            End If
                    End Select
                    
                    '排他チェック
                    If Not oldDate.Equals(newDate) Then
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Exit Sub
                    End If

                    '更新処理
                    Call UpdateArea()
                    FrmBase.LogOperation(Lexis.UpdateCompleted, Me.Text) 'TODO: 少なくとも「操作」ログではない。詳細設計も含め確認。   '更新処理が正常に終了しました。
                    AlertBox.Show(Lexis.UpdateCompleted)
                    FrmBase.LogOperation(Lexis.OkButtonClicked, Me.Text)
                    Me.Close()
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked, Me.Text)
                    btnUpdate.Select()
                End If
            End If

        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnUpdate.Select()
            Exit Sub

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '予期せぬエラーが発生しました。
            AlertBox.Show(Lexis.UpdateFailed)
            btnUpdate.Select()
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
        '終了ボタンを押下
        FrmBase.LogOperation(sender, e, Me.Text)
        Me.Close()
    End Sub

    ''' <summary>「エリア名称」の入力値が制限する</summary>
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
    ''' 「修正」ボタンを押下した際にすべてのコントロールの値をチェックする。
    ''' </summary>
    ''' <returns>データ合法フラグ</returns>
    Private Function CheckAll() As Boolean

        '当関数の戻り値
        Dim bRetAll As Boolean = False

        If System.String.IsNullOrEmpty(txtAreaname.Text) Then
            'エリア名称:値がヌルである。
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblAreaname.Text)
            txtAreaname.Focus()
        ElseIf OPMGUtility.CheckString(Me.txtAreaname.Text.ToString, 10, 2, True) = -4 Then
            '入力値が不正です。
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaName)
            txtAreaname.Focus()
        ElseIf Me.txtAreaname.Text.ToString.Trim() = "" Then
            '入力値が不正です。全角スペースのみ入力した場合
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaName)
            txtAreaname.Focus()
        Else
            bRetAll = True
        End If

        Return bRetAll

    End Function

    ''' <summary>
    ''' DBへ設定されたエリア情報を修正する。
    ''' </summary>
    Private Sub UpdateArea()

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder
        Dim dbCtl As DatabaseTalker

        '端末ID
        Dim sClient As String = Config.MachineName
        dbCtl = New DatabaseTalker

        Try
            'エリア名称を取得する。
            sAreaName = txtAreaname.Text

            sBuilder.AppendLine(" UPDATE M_AREA_DATA SET UPDATE_DATE = GETDATE(),")
            sBuilder.AppendLine(" UPDATE_USER_ID = " & Utility.SetSglQuot(sLoginID.ToString) & ",")
            sBuilder.AppendLine(" UPDATE_MACHINE_ID = " & Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine(" AREA_NAME = " & Utility.SetSglQuot(sAreaName))
            sBuilder.AppendLine(" WHERE MODEL_CODE = " & Utility.SetSglQuot(sModelCode))
            sBuilder.AppendLine(" AND AREA_NO = " & Utility.SetSglQuot(sAreaNo))

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

        'データを取得する。
        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine(" SELECT AREA_NAME, UPDATE_DATE")
            sBuilder.AppendLine("  FROM M_AREA_DATA  ")
            sBuilder.AppendLine(" WHERE MODEL_CODE = " & Utility.SetSglQuot(sModelCode))
            sBuilder.AppendLine(" AND AREA_NO = " & Utility.SetSglQuot(sAreaNo))
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
