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

''' <summary>パターン削除</summary>
''' <remarks>
''' パターン名称を変更し、「削除」ボタンをクリックすることにより、
''' 当該データを運用管理サーバより削除する。
''' </remarks>
Public Class FrmSysPatternMstDelete
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
    Friend WithEvents lblPtnNoTitle As System.Windows.Forms.Label
    Friend WithEvents lblPtnNameTitle As System.Windows.Forms.Label
    Friend WithEvents lblPtnNo As System.Windows.Forms.Label
    Friend WithEvents lblPtnName As System.Windows.Forms.Label
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents pnlPtnDelete As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblPtnNoTitle = New System.Windows.Forms.Label()
        Me.lblPtnNameTitle = New System.Windows.Forms.Label()
        Me.lblPtnNo = New System.Windows.Forms.Label()
        Me.lblPtnName = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.pnlPtnDelete = New System.Windows.Forms.Panel()
        Me.pnlPtnDelete.SuspendLayout()
        Me.SuspendLayout()
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
        'lblPtnName
        '
        Me.lblPtnName.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnName.Location = New System.Drawing.Point(165, 261)
        Me.lblPtnName.Name = "lblPtnName"
        Me.lblPtnName.Size = New System.Drawing.Size(180, 21)
        Me.lblPtnName.TabIndex = 4
        Me.lblPtnName.Text = "ＸＸＸＸＸＸＸＸＸＸ"
        Me.lblPtnName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.Silver
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(426, 116)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(90, 32)
        Me.btnDelete.TabIndex = 0
        Me.btnDelete.Text = "削  除"
        Me.btnDelete.UseVisualStyleBackColor = False
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
        'pnlPtnDelete
        '
        Me.pnlPtnDelete.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlPtnDelete.Controls.Add(Me.lblPtnNoTitle)
        Me.pnlPtnDelete.Controls.Add(Me.btnReturn)
        Me.pnlPtnDelete.Controls.Add(Me.lblPtnNameTitle)
        Me.pnlPtnDelete.Controls.Add(Me.btnDelete)
        Me.pnlPtnDelete.Controls.Add(Me.lblPtnNo)
        Me.pnlPtnDelete.Controls.Add(Me.lblPtnName)
        Me.pnlPtnDelete.Location = New System.Drawing.Point(0, 0)
        Me.pnlPtnDelete.Name = "pnlPtnDelete"
        Me.pnlPtnDelete.Size = New System.Drawing.Size(594, 418)
        Me.pnlPtnDelete.TabIndex = 0
        '
        'FrmSysPatternMstDelete
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlPtnDelete)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysPatternMstDelete"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "パターン削除"
        Me.pnlPtnDelete.ResumeLayout(False)
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
    Private ReadOnly FormTitle As String = "パターン設定削除"

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
    Private sModelcode As String = ""

    Public Property Modelcode() As String
        Get
            Return sModelcode
        End Get
        Set(ByVal value As String)
            sModelcode = value
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
    ''' Patternデータ削除画面のデータを準備する
    ''' </summary>
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean
        Dim bRet As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim dtMstTable As DataTable
        Try
            Log.Info("Method started.")

            'データを取得する。
            dtMstTable = GetMstTable()

            If dtMstTable Is Nothing OrElse dtMstTable.Rows.Count = 0 Then
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
    Private Sub FrmSysPatternMstDelete_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrmData() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            '画面背景色（BackColor）を設定する
            pnlPtnDelete.BackColor = Config.BackgroundColor
            lblPtnNameTitle.BackColor = Config.BackgroundColor
            lblPtnNoTitle.BackColor = Config.BackgroundColor
            lblPtnName.BackColor = Config.BackgroundColor
            lblPtnNo.BackColor = Config.BackgroundColor

            'ボタン背景色（BackColor）を設定する
            btnDelete.BackColor = Config.ButtonColor
            btnReturn.BackColor = Config.ButtonColor

            'バターンNoを設定する。
            lblPtnNo.Text = sPatternNo

            'バターンの値を設定する。
            lblPtnName.Text = sPatternName

            Me.btnDelete.Focus()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' 「削除」ボタンを押下すると、DBへ設定されたパターン情報を削除する。
    ''' </summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        '削除ボタンを押下
        If LbEventStop Then Exit Sub
        Dim dtMstTable As DataTable

        Try
            LbEventStop = True
            '削除ボタン押下。
            FrmBase.LogOperation(sender, e, Me.Text)

            '削除確認処理
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyDelete).Equals(System.Windows.Forms.DialogResult.Yes) Then
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

                '削除処理
                Call DeletePattern()
                '削除処理が正常に終了しました。
                FrmBase.LogOperation(Lexis.DeleteCompleted) 'TODO: 少なくとも「操作」ログではない。詳細設計も含め確認。
                AlertBox.Show(Lexis.DeleteCompleted)
                FrmBase.LogOperation(Lexis.OkButtonClicked)
                Me.Close()
            Else
                FrmBase.LogOperation(Lexis.NoButtonClicked)
                FrmBase.LogOperation(Lexis.DeleteFailed) 'TODO: 少なくとも「操作」ログではない。詳細設計も含め確認。
                btnDelete.Select()
            End If
        Catch ex As DatabaseException
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '予期せぬエラーが発生しました。
            AlertBox.Show(Lexis.DeleteFailed)
            btnDelete.Select()
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

#End Region

#Region "メソッド（Private）"

    ''' <summary>
    ''' DBへ設定されたバターン情報を削除する。
    ''' </summary>
    Private Sub DeletePattern()

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Dim dbCtl As DatabaseTalker
        dbCtl = New DatabaseTalker

        Try
            sBuilder.AppendLine(" DELETE FROM M_PATTERN_DATA ")
            sBuilder.AppendLine(" WHERE PATTERN_NO = " & Utility.SetSglQuot(sPatternNo))
            sBuilder.AppendLine(" AND MST_KIND = " & Utility.SetSglQuot(sKind))
            sBuilder.AppendLine(" AND MODEL_CODE = " & Utility.SetSglQuot(sModelcode))
            sSQL = sBuilder.ToString()

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()
            dbCtl.ConnectClose()
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            btnDelete.Select()
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

            sBuilder.AppendLine(" SELECT PATTERN_NAME,  UPDATE_DATE")
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
