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

''' <summary>パターン修正</summary>
''' <remarks>
''' パターン名称を変更し、「修正」ボタンをクリックすることにより、
''' 設定内容を運用管理サーバに登録する。
''' </remarks>
Public Class FrmSysPatternMstUpdate
    Inherits System.Windows.Forms.Form

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

    End Sub

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    Friend WithEvents txtPtnName As System.Windows.Forms.TextBox
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents lblPtnNo As System.Windows.Forms.Label
    Friend WithEvents lblPtnNameTitle As System.Windows.Forms.Label
    Friend WithEvents lblPtnNoTitle As System.Windows.Forms.Label
    Friend WithEvents pnlPtnUpdate As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtPtnName = New System.Windows.Forms.TextBox()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.lblPtnNo = New System.Windows.Forms.Label()
        Me.lblPtnNameTitle = New System.Windows.Forms.Label()
        Me.lblPtnNoTitle = New System.Windows.Forms.Label()
        Me.pnlPtnUpdate = New System.Windows.Forms.Panel()
        Me.pnlPtnUpdate.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtPtnName
        '
        Me.txtPtnName.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPtnName.Location = New System.Drawing.Point(165, 261)
        Me.txtPtnName.MaxLength = 10
        Me.txtPtnName.Name = "txtPtnName"
        Me.txtPtnName.Size = New System.Drawing.Size(170, 22)
        Me.txtPtnName.TabIndex = 0
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.Silver
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(426, 255)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(90, 32)
        Me.btnReturn.TabIndex = 1
        Me.btnReturn.Text = "終  了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.Color.Silver
        Me.btnUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(426, 116)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(90, 32)
        Me.btnUpdate.TabIndex = 0
        Me.btnUpdate.Text = "修  正"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'lblPtnNo
        '
        Me.lblPtnNo.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNo.Location = New System.Drawing.Point(165, 121)
        Me.lblPtnNo.Name = "lblPtnNo"
        Me.lblPtnNo.Size = New System.Drawing.Size(50, 21)
        Me.lblPtnNo.TabIndex = 3
        Me.lblPtnNo.Text = "XX"
        Me.lblPtnNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnNameTitle
        '
        Me.lblPtnNameTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNameTitle.Location = New System.Drawing.Point(53, 261)
        Me.lblPtnNameTitle.Name = "lblPtnNameTitle"
        Me.lblPtnNameTitle.Size = New System.Drawing.Size(110, 21)
        Me.lblPtnNameTitle.TabIndex = 2
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
        'pnlPtnUpdate
        '
        Me.pnlPtnUpdate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlPtnUpdate.Controls.Add(Me.lblPtnNameTitle)
        Me.pnlPtnUpdate.Controls.Add(Me.btnReturn)
        Me.pnlPtnUpdate.Controls.Add(Me.txtPtnName)
        Me.pnlPtnUpdate.Controls.Add(Me.btnUpdate)
        Me.pnlPtnUpdate.Controls.Add(Me.lblPtnNoTitle)
        Me.pnlPtnUpdate.Controls.Add(Me.lblPtnNo)
        Me.pnlPtnUpdate.Location = New System.Drawing.Point(0, 0)
        Me.pnlPtnUpdate.Name = "pnlPtnUpdate"
        Me.pnlPtnUpdate.Size = New System.Drawing.Size(594, 418)
        Me.pnlPtnUpdate.TabIndex = 0
        '
        'FrmSysPatternMstUpdate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlPtnUpdate)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysPatternMstUpdate"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "パターン修正"
        Me.pnlPtnUpdate.ResumeLayout(False)
        Me.pnlPtnUpdate.PerformLayout()
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
    Private ReadOnly FormTitle As String = "パターン設定修正"

    '修正ユーザのIDを取得する。
    Private sLoginID As String = ""
    Public Property LoginID() As String
        Get
            Return sLoginID
        End Get
        Set(ByVal value As String)
            sLoginID = value
        End Set
    End Property

    'バターンNoを取得する。
    Private sPatternNo As String = ""

    Public Property PatternNo() As String
        Get
            Return sPatternNo
        End Get
        Set(ByVal value As String)
            sPatternNo = value
        End Set
    End Property

    'バターンNameを取得する。
    Private sPatternName As String = ""

    Public Property PatternName() As String
        Get
            Return sPatternName
        End Get
        Set(ByVal value As String)
            sPatternName = value
        End Set
    End Property

    '機種codeを取得する。
    Private sModelcode As String = ""

    Public Property Modelcode() As String
        Get
            Return sModelcode
        End Get
        Set(ByVal value As String)
            sModelcode = value
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

    'バターン値を取得する。
    Private sPattern As String = ""

    '更新日時
    Private oldDate As String = ""

    '更新日時
    Private newDate As String = ""

#End Region

#Region "メソッド（Public）"

    ''' <summary>
    ''' Patternデータ修正画面のデータを準備する
    ''' </summary>
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean
        Dim bRet As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim dtMstTable As DataTable
        Try
            Log.Info("Method started.")

            '操作者IDを取得する。
            sLoginID = GlobalVariables.UserId

            'データを取得する。
            dtMstTable = GetMstTable()

            If dtMstTable Is Nothing Or dtMstTable.Rows.Count = 0 Then
                '検索条件に一致するデータは存在しません。
                AlertBox.Show(Lexis.CompetitiveOperationDetected)
                Return bRet
                Exit Function
            Else
                sPatternName = dtMstTable.Rows(0).Item("PATTERN_NAME").ToString
                oldDate = dtMstTable.Rows(0).Item("UPDATE_DATE").ToString
            End If

            bRet = True

        Catch ex As Exception

            '画面表示処理に失敗しました。
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRet = False

        Finally
            If bRet Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd) '開始異常メッセージ
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
    Private Sub FrmSysPatternMstUpdate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrmData() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            '画面背景色（BackColor）を設定する
            pnlPtnUpdate.BackColor = Config.BackgroundColor
            lblPtnNameTitle.BackColor = Config.BackgroundColor
            lblPtnNoTitle.BackColor = Config.BackgroundColor
            lblPtnNo.BackColor = Config.BackgroundColor

            'ボタン背景色（BackColor）を設定する
            btnUpdate.BackColor = Config.ButtonColor
            btnReturn.BackColor = Config.ButtonColor
            Me.txtPtnName.ImeMode = Windows.Forms.ImeMode.Hiragana

            'バターンNoを設定する。
            lblPtnNo.Text = sPatternNo

            'バターンの値を設定する。
            txtPtnName.Text = sPatternName

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' 「修正」ボタンを押下すると、DBへ設定されたパターン情報を修正する。
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Dim dtMstTable As DataTable

        Try
            LbEventStop = True
            '修正ボタン押下。
            FrmBase.LogOperation(sender, e, Me.Text)

            If CheckAll() = True Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUpdate) = DialogResult.Yes Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked)
                    Call waitCursor(True)
                    'データを取得する。
                    dtMstTable = GetMstTable()

                    If dtMstTable Is Nothing OrElse dtMstTable.Rows.Count = 0 Then
                        '検索条件に一致するデータは存在しません。
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Exit Sub
                    Else
                        newDate = dtMstTable.Rows(0).Item("UPDATE_DATE").ToString
                    End If

                    '排他チェック
                    If Not oldDate.Equals(newDate) Then
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Exit Sub
                    End If

                    '更新処理
                    Call UpdatePattern()
                    FrmBase.LogOperation(Lexis.UpdateCompleted) 'TODO: 少なくとも「操作」ログではない。詳細設計も含め確認。   '更新処理が正常に終了しました。
                    AlertBox.Show(Lexis.UpdateCompleted)
                    FrmBase.LogOperation(Lexis.OkButtonClicked)
                    Me.Close()
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked)
                    FrmBase.LogOperation(Lexis.UpdateFailed) 'TODO: 少なくとも「操作」ログではない。詳細設計も含め確認。
                    btnUpdate.Select()
                End If
            End If
        Catch ex As DatabaseException
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)  '予期せぬエラーが発生しました。
            AlertBox.Show(Lexis.UpdateFailed)
            btnUpdate.Select()
            Exit Sub
        Finally
            LbEventStop = False
            Call waitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 「終了」ボタンを押下すると、本画面が終了される。 
    ''' </summary>
    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '終了ボタン押下。
        FrmBase.LogOperation(sender, e, Me.Text)
        Me.Close()
    End Sub

    ''' <summary>
    ''' バターンの入力値が制限する
    ''' </summary>
    Private Sub txtPattern_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPtnName.KeyPress

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

        If System.String.IsNullOrEmpty(txtPtnName.Text) Then
            '入力値が不正です。バターンの値はヌルである。
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblPtnNameTitle.Text)
            txtPtnName.Focus()
        ElseIf OPMGUtility.CheckString(Me.txtPtnName.Text.ToString, 10, 2, True) = -4 Then
            '入力値が不正です。
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternName)
            txtPtnName.Focus()
        ElseIf Me.txtPtnName.Text.ToString.Trim() = "" Then
            '入力値が不正です。全角スペースのみ入力した場合
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternName)
            txtPtnName.Focus()
        Else
            bRetAll = True
        End If
        Return bRetAll

    End Function

    ''' <summary>
    ''' DBへ設定されたバターン情報を更新する。
    ''' </summary>
    Private Sub UpdatePattern()

        Dim sSQL As String = ""

        Dim sBuilder As New StringBuilder

        Dim dbCtl As New DatabaseTalker
        dbCtl = New DatabaseTalker

        Try
            'バターンの値を取得する。
            sPattern = txtPtnName.Text

            '端末ID
            Dim sClient As String = Config.MachineName
            sBuilder.AppendLine(" UPDATE M_PATTERN_DATA SET UPDATE_DATE = GETDATE(),")
            sBuilder.AppendLine(" UPDATE_USER_ID = " & Utility.SetSglQuot(sLoginID.ToString) & ",")
            sBuilder.AppendLine(" UPDATE_MACHINE_ID = " & Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine(" PATTERN_NAME = " & Utility.SetSglQuot(sPattern))
            sBuilder.AppendLine(" WHERE PATTERN_NO = " & Utility.SetSglQuot(sPatternNo))
            sBuilder.AppendLine(" AND MST_KIND = " & Utility.SetSglQuot(sKind))
            sBuilder.AppendLine(" AND MODEL_CODE = " & Utility.SetSglQuot(sModelcode))

            sSQL = sBuilder.ToString()

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            btnUpdate.Select()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

    End Sub

    ''' <summary>
    ''' データを取得する。
    ''' </summary>
    Private Function GetMstTable() As DataTable

        'データを取得する。
        Dim dtMstTable As New DataTable
        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder
        Dim nRtn As Integer

        Try
            sBuilder.AppendLine(" SELECT PATTERN_NO, PATTERN_NAME, UPDATE_DATE ")
            sBuilder.AppendLine(" FROM M_PATTERN_DATA ")
            sBuilder.AppendLine(" WHERE PATTERN_NO = " & Utility.SetSglQuot(sPatternNo))
            sBuilder.AppendLine(" AND MST_KIND = " & Utility.SetSglQuot(sKind))
            sBuilder.AppendLine(" AND MODEL_CODE = " & Utility.SetSglQuot(sModelcode))
            sSQL = sBuilder.ToString()

            nRtn = FrmBase.BaseSqlDataTableFill(sSQL, dtMstTable)

            If nRtn = -9 Then
                Throw New OPMGException()
            End If
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try

        Return dtMstTable

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
