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
Imports JR.ExOpmg.DBCommon   '定数値のみ使用
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO
Imports System.Text
Imports AdvanceSoftware.VBReport7.Xls

''' <summary>パターン設定</summary>
''' <remarks>
''' パターン設定画面の検索条件によって、パターン情報を表示する。
''' 一つの検索レコードを選択し、対応する引数をサブ画面登録、修正、削除に渡す。
''' </remarks>
Public Class FrmSysPatternMst
    Inherits FrmBase

    'フラグ:検索条件は「マスタ名称」を取得しますか、それもと「プログラム名称」を取得しますか
    Private bMstChecked As Boolean = False


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
    Friend WithEvents istPatternMst As System.Windows.Forms.ImageList
    Friend WithEvents btnInsert As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelet As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents cmbMstname As System.Windows.Forms.ComboBox
    Friend WithEvents cmbModelname As System.Windows.Forms.ComboBox
    Friend WithEvents btnSearch As System.Windows.Forms.Button
    Friend WithEvents pnlSelect As System.Windows.Forms.Panel
    Friend WithEvents lblMstName As System.Windows.Forms.Label
    Friend WithEvents lblMach As System.Windows.Forms.Label
    Friend WithEvents wbkIDMst As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents grpSelect As System.Windows.Forms.GroupBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysPatternMst))
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDelet = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.cmbMstname = New System.Windows.Forms.ComboBox()
        Me.cmbModelname = New System.Windows.Forms.ComboBox()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.pnlSelect = New System.Windows.Forms.Panel()
        Me.grpSelect = New System.Windows.Forms.GroupBox()
        Me.lblMach = New System.Windows.Forms.Label()
        Me.lblMstName = New System.Windows.Forms.Label()
        Me.istPatternMst = New System.Windows.Forms.ImageList(Me.components)
        Me.wbkIDMst = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.pnlBodyBase.SuspendLayout()
        Me.pnlSelect.SuspendLayout()
        Me.grpSelect.SuspendLayout()
        Me.wbkIDMst.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.wbkIDMst)
        Me.pnlBodyBase.Controls.Add(Me.btnInsert)
        Me.pnlBodyBase.Controls.Add(Me.btnUpdate)
        Me.pnlBodyBase.Controls.Add(Me.btnDelet)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.pnlSelect)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/08/02(金)  15:27"
        '
        'btnInsert
        '
        Me.btnInsert.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnInsert.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnInsert.Location = New System.Drawing.Point(872, 404)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(128, 40)
        Me.btnInsert.TabIndex = 4
        Me.btnInsert.Text = "登  録"
        Me.btnInsert.UseVisualStyleBackColor = False
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(872, 464)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(128, 40)
        Me.btnUpdate.TabIndex = 5
        Me.btnUpdate.Text = "修  正"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'btnDelet
        '
        Me.btnDelet.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDelet.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelet.Location = New System.Drawing.Point(872, 524)
        Me.btnDelet.Name = "btnDelet"
        Me.btnDelet.Size = New System.Drawing.Size(128, 40)
        Me.btnDelet.TabIndex = 6
        Me.btnDelet.Text = "削  除"
        Me.btnDelet.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 7
        Me.btnReturn.Text = "終  了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'cmbMstname
        '
        Me.cmbMstname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMstname.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmbMstname.Location = New System.Drawing.Point(160, 67)
        Me.cmbMstname.MaxLength = 15
        Me.cmbMstname.Name = "cmbMstname"
        Me.cmbMstname.Size = New System.Drawing.Size(220, 21)
        Me.cmbMstname.TabIndex = 2
        '
        'cmbModelname
        '
        Me.cmbModelname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModelname.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmbModelname.Location = New System.Drawing.Point(160, 19)
        Me.cmbModelname.MaxLength = 5
        Me.cmbModelname.Name = "cmbModelname"
        Me.cmbModelname.Size = New System.Drawing.Size(220, 21)
        Me.cmbModelname.TabIndex = 1
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.Color.Silver
        Me.btnSearch.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!)
        Me.btnSearch.Location = New System.Drawing.Point(570, 22)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(128, 40)
        Me.btnSearch.TabIndex = 3
        Me.btnSearch.Text = "検  索"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'pnlSelect
        '
        Me.pnlSelect.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlSelect.Controls.Add(Me.grpSelect)
        Me.pnlSelect.Location = New System.Drawing.Point(10, 20)
        Me.pnlSelect.Name = "pnlSelect"
        Me.pnlSelect.Size = New System.Drawing.Size(824, 110)
        Me.pnlSelect.TabIndex = 0
        '
        'grpSelect
        '
        Me.grpSelect.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grpSelect.Controls.Add(Me.lblMach)
        Me.grpSelect.Controls.Add(Me.cmbMstname)
        Me.grpSelect.Controls.Add(Me.lblMstName)
        Me.grpSelect.Controls.Add(Me.cmbModelname)
        Me.grpSelect.Controls.Add(Me.btnSearch)
        Me.grpSelect.Location = New System.Drawing.Point(56, 10)
        Me.grpSelect.Name = "grpSelect"
        Me.grpSelect.Size = New System.Drawing.Size(747, 100)
        Me.grpSelect.TabIndex = 0
        Me.grpSelect.TabStop = False
        '
        'lblMach
        '
        Me.lblMach.Location = New System.Drawing.Point(74, 22)
        Me.lblMach.Name = "lblMach"
        Me.lblMach.Size = New System.Drawing.Size(77, 19)
        Me.lblMach.TabIndex = 6
        Me.lblMach.Text = "機種"
        '
        'lblMstName
        '
        Me.lblMstName.Location = New System.Drawing.Point(74, 70)
        Me.lblMstName.Name = "lblMstName"
        Me.lblMstName.Size = New System.Drawing.Size(77, 19)
        Me.lblMstName.TabIndex = 7
        Me.lblMstName.Text = "マスタ名称"
        '
        'istPatternMst
        '
        Me.istPatternMst.ImageStream = CType(resources.GetObject("istPatternMst.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.istPatternMst.TransparentColor = System.Drawing.Color.White
        Me.istPatternMst.Images.SetKeyName(0, "")
        Me.istPatternMst.Images.SetKeyName(1, "")
        '
        'wbkIDMst
        '
        Me.wbkIDMst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wbkIDMst.Controls.Add(Me.shtMain)
        Me.wbkIDMst.Location = New System.Drawing.Point(124, 164)
        Me.wbkIDMst.Name = "wbkIDMst"
        Me.wbkIDMst.ProcessTabKey = False
        Me.wbkIDMst.ShowTabs = False
        Me.wbkIDMst.Size = New System.Drawing.Size(580, 436)
        Me.wbkIDMst.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wbkIDMst.TabIndex = 0
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(1, 1)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(561, 417)
        Me.shtMain.TabIndex = 0
        Me.shtMain.TabStop = False
        '
        'FrmSysPatternMst
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysPatternMst"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlSelect.ResumeLayout(False)
        Me.grpSelect.ResumeLayout(False)
        Me.wbkIDMst.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub


#End Region

#Region "宣言領域（Private）"


    '検索条件を取得する。
    Private sKind As String = ""

    ''' <summary>
    ''' 初期処理呼出判定
    ''' （True:初期処理呼出済み、False:初期処理未呼出(Form_Load内で初期処理実施)）
    ''' </summary>
    Private LbInitCallFlg As Boolean = False

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean = False

    ''' <summary>
    ''' 一覧表示最大列
    ''' </summary>
    Private LcstMaxColCnt As Integer

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "パターン設定"

#End Region

#Region " メソッド（Public）"

    ''' <summary>パターン設定画面のデータを準備する</summary>
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrm() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True       'イベント発生ＯＦＦ
        Try
            Log.Info("Method started.")

            '業務タイトル表示エリアに画面タイトルをセット
            lblTitle.Text = LcstFormTitle

            'シート初期化
            shtMain.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row
            LcstMaxColCnt = shtMain.MaxColumns()                                '列数を取得
            'シートの表示選択モードを設定する
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader

            'コントロールの初期化（共通設定）
            Dim all As Control() = BaseGetAllControls(pnlBodyBase)
            For Each c As Control In all
                Try
                    If TypeOf c Is RadioButton Then
                        CType(c, RadioButton).Checked = False
                    ElseIf TypeOf c Is ComboBox Then
                        CType(c, ComboBox).DataSource = Nothing
                        If CType(c, ComboBox).Items.Count > 0 Then CType(c, ComboBox).Items.Clear()
                        CType(c, ComboBox).MaxDropDownItems = 20
                    End If
                Catch ex As Exception
                End Try
            Next

            '検索した機種データを格納する。
            If LfSetCmbModelname() = False Then Exit Try '駅名コンボボックス設定
            cmbModelname.SelectedIndex = 0            'デフォルト表示項目
            If cmbModelname.SelectedValue.ToString <> "" Then
                If LfSetCmbMstName(cmbModelname.SelectedValue.ToString) = False Then Exit Try 'コーナーコンボボックス設定
                cmbMstname.SelectedIndex = 0           'デフォルト表示項目
            Else
                cmbMstname.Enabled = False
            End If

            'コンボボックスの状態の設定
            setComboStatus(True, False)
            '一覧ソートの初期化
            LfClrList()
            LbEventStop = False         'イベント発生ＯＮ
            bRtn = True

        Catch ex As Exception
            '画面表示処理に失敗しました。
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If
            LbEventStop = False 'イベント発生ＯＮ
        End Try

        Return bRtn

    End Function

#End Region

#Region " イベント "

    ''' <summary>
    ''' ローディング　メインウィンドウ
    ''' </summary>
    Private Sub FrmSysPatternMst_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrm() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            '初期化 ボタンの非活性化
            setBtnStatus(False, False, False, False)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub


    ''' <summary>「検索」ボタンを押下すると、Eltableの内容を表示する。</summary>
    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If LbEventStop Then Exit Sub
        Try
            LbEventStop = True
            LfWaitCursor()

            '検索ボタン押下。
            LogOperation(sender, e)
            '一覧ソートの初期化
            LfClrList()
            shtMain.Enabled = True
            btnInsert.Enabled = True
            'パターンの検索する
            Call selectPattern(True)
        Catch ex As Exception
            '検索処理に失敗しました。
            Log.Fatal("Unwelcome Exception caught.", ex)        '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ
            btnSearch.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>「登録」ボタンを押下すると、パターン名称を入力画面が表示される。</summary>
    Private Sub btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click
        If LbEventStop Then Exit Sub
        Try
            LbEventStop = True
            LfWaitCursor()
            '登録ボタン押下。
            LogOperation(sender, e)

            Dim oFrmSysPatternMstAdd As New FrmSysPatternMstAdd
            '操作者IDを取得する。
            oFrmSysPatternMstAdd.LoginID() = GlobalVariables.UserId
            'マスタ種別を取得する。
            oFrmSysPatternMstAdd.Kind() = Me.cmbMstname.SelectedValue.ToString
            '検索条件のフラグを取得する。
            oFrmSysPatternMstAdd.CheckFlag() = bMstChecked

            If Not cmbModelname.DataSource Is Nothing Then
                '機種コードを取得する
                oFrmSysPatternMstAdd.ModelCode() = Me.cmbModelname.SelectedValue.ToString()
            End If

            'パターン登録画面表示処理開始
            oFrmSysPatternMstAdd.ShowDialog()

            'TODO: Form.Newを呼び出して以降に例外が発生した場合のことを
            '考えると、FrmMntDispFaultDataDetailのShowDialogを行うときと同様の
            '方針に統一する方がよいかもしれない。（逆にこちらが正解の可能性もある）
            oFrmSysPatternMstAdd.Dispose()

            '一覧ソートの初期化
            LfClrList()
            'パターンの検索する
            selectPattern(False)
            shtMain.Enabled = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'TODO: このようなケースで下記を行うべきか否か、方針を統一しなければならない。
            'モーダルなShowDialogの最中に発生した例外が本当にここに到達するなら、
            '他の箇所も、こうした上で、InitFrmで同様のメッセージボックス表示を
            '行わないようにする方がよいかもしれない。
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>「修正」ボタンを押下すると、パターン名称を変更画面が表示される。</summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Try
            LbEventStop = True
            LfWaitCursor()
            '修正ボタン押下。
            LogOperation(sender, e)

            Dim oFrmSysPatternMstUpdate As New FrmSysPatternMstUpdate

            'FrmSysIDMstUpdate画面のプロパティに値を代入する。
            Dim nRowno As Integer = shtMain.ActivePosition.Row

            '操作者IDを取得する。
            oFrmSysPatternMstUpdate.LoginID() = GlobalVariables.UserId
            'パターンNoを取得する。
            oFrmSysPatternMstUpdate.PatternNo() = Me.shtMain.Item(0, nRowno).Text
            'パターン名称を取得する。
            oFrmSysPatternMstUpdate.PatternName() = Me.shtMain.Item(1, nRowno).Text
            'マスタ種別を取得する。
            oFrmSysPatternMstUpdate.Kind() = Me.cmbMstname.SelectedValue.ToString
            '検索条件のフラグを取得する。
            oFrmSysPatternMstUpdate.CheckFlag() = bMstChecked

            If Not cmbModelname.DataSource Is Nothing Then
                '機種コードを取得する
                oFrmSysPatternMstUpdate.Modelcode() = Me.cmbModelname.SelectedValue.ToString()
            End If

            If oFrmSysPatternMstUpdate.InitFrmData() = False Then
                oFrmSysPatternMstUpdate = Nothing
                Call waitCursor(False)
                Exit Sub
            End If
            'パターン修正画面表示処理開始
            oFrmSysPatternMstUpdate.ShowDialog()
            oFrmSysPatternMstUpdate.Dispose()
            '一覧ソートの初期化
            LfClrList()
            'パターンの検索する
            Call selectPattern(False)
            shtMain.Enabled = True
        Catch ex As Exception
            '画面表示処理に失敗しました。
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>「削除」ボタンを押下すると、パターン名称を削除画面が表示される。</summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelet.Click
        If LbEventStop Then Exit Sub
        Try
            LbEventStop = True
            LfWaitCursor()
            '削除ボタン押下。
            LogOperation(sender, e)

            Dim oFrmSysPatternMstDelete As New FrmSysPatternMstDelete

            'oFrmSysPatternMstDelete画面のプロパティに値を代入する。
            Dim nRowno As Integer = shtMain.ActivePosition.Row

            'パターンNoを取得する。
            oFrmSysPatternMstDelete.PatternNo() = Me.shtMain.Item(0, nRowno).Text
            'パターン名称を取得する。
            oFrmSysPatternMstDelete.PatternName() = Me.shtMain.Item(1, nRowno).Text
            'マスタ種別を取得する。
            oFrmSysPatternMstDelete.Kind() = Me.cmbMstname.SelectedValue.ToString
            '検索条件のフラグを取得する。
            oFrmSysPatternMstDelete.CheckFlag() = bMstChecked

            If Not cmbModelname.DataSource Is Nothing Then
                '機種コードを取得する
                oFrmSysPatternMstDelete.Modelcode() = Me.cmbModelname.SelectedValue.ToString()
            End If

            If oFrmSysPatternMstDelete.InitFrmData() = False Then
                oFrmSysPatternMstDelete = Nothing
                Call waitCursor(False)
                Exit Sub
            End If
            'パターン削除画面表示処理開始
            oFrmSysPatternMstDelete.ShowDialog()
            oFrmSysPatternMstDelete.Dispose()
            '一覧ソートの初期化
            LfClrList()
            'パターンの検索する
            Call selectPattern(False)
            shtMain.Enabled = True
        Catch ex As Exception
            '画面表示処理に失敗しました。
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>「終了」ボタンを押下すると、本画面が終了される。</summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '終了ボタン押下。
        LogOperation(sender, e)
        Me.Close()
    End Sub

#End Region

#Region " ELTableのクリア "

    ''' <summary>ELTableのクリア</summary>
    ''' <remarks>
    ''' Eltableにあるデータをクリア
    ''' </remarks>
    Private Sub initElTable()
        'Eltableの現在の最大桁数
        Dim sXYRange As String
        Dim i As Integer

        '画面の閃きを防ぐ。
        Me.shtMain.Redraw = False

        Try
            For i = 0 To shtMain.Columns.Count - 1
                '列ヘッダのイメージをクリアする
                shtMain.ColumnHeaders(i).Image = Nothing
                If shtMain.Rows.Count > 0 Then
                    '前回ソートされた列の背景色を初期化する
                    shtMain.Columns(i).BackColor = Color.Empty
                    '前回ソートされた列のセル罫線を消去する
                    shtMain.Columns(i).SetBorder(New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), GrapeCity.Win.ElTabelleSheet.Borders.All)
                End If
            Next

            If Me.shtMain.MaxRows > 0 Then
                'Eltableの現在の最大桁数を取得する。
                sXYRange = "1:" & Me.shtMain.MaxRows.ToString

                '選択されたエリアのデータをクリアする。
                Me.shtMain.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
            End If

            'Eltableの最大桁数を設定する。
            Me.shtMain.MaxRows = 0

        Catch ex As Exception

            '画面表示処理に失敗しました
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New DatabaseException(ex)

        Finally

            'Eltableを更新する。
            Me.shtMain.Redraw = True

        End Try

    End Sub

#End Region

#Region " コンボボックス内容を取得 "


    ''' <summary>
    ''' [機種名称コンボ設定]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetCmbModelname() As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As ModelMaster
        oMst = New ModelMaster
        Try
            dt = oMst.SelectTable(False)
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbModelname)
            cmbModelname.SelectedIndex = -1
            If cmbModelname.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function

    ''' <summary>マスタ名称を取得する</summary>
    ''' <returns>マスタデータマスタ取得結果格納テーブル。</returns>
    ''' <remarks>
    ''' マスタ名称を取得する、DataTableの先頭に、空白行を追加する。
    ''' </remarks>
    Private Function LfSetCmbMstName(ByVal sModel As String) As Boolean
        'マスタ名称を格納する。
        Dim bRtn As Boolean = False
        Dim dt As New DataTable
        Dim oMst As MasterMaster
        oMst = New MasterMaster
        Try
            If sModel <> "" Then
                dt = oMst.SelectTable(sModel)
                dt = oMst.SetSpace()
                bRtn = BaseSetMstDtToCmb(dt, cmbMstname)
                cmbMstname.SelectedIndex = -1
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn

    End Function

    ''' <summary>
    ''' [検索用SELECT文字列取得]
    ''' </summary>
    ''' <returns>SELECT文</returns>
    Private Function LfGetSelectString() As String

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder
        '機種データを取得する。
        Dim sModel As String = Me.cmbModelname.SelectedValue.ToString
        Dim sMstKind As String = Me.cmbMstname.SelectedValue.ToString
        Try
            'テーブル:パターン名称マスタ
            '取得項目:パターンNO
            '取得項目:パターン名称
            sBuilder.AppendLine(" SELECT PATTERN_NO,PATTERN_NAME ")
            sBuilder.AppendLine(" FROM M_PATTERN_DATA ")
            sBuilder.AppendLine(" WHERE MODEL_CODE = " + Utility.SetSglQuot(sModel))
            sBuilder.AppendLine(" AND MST_KIND = " + Utility.SetSglQuot(sMstKind))
            sBuilder.AppendLine(" ORDER BY PATTERN_NO ")
            sSQL = sBuilder.ToString()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

        Return sSQL

    End Function

    ''' <summary>DataTableからインデックス値の取得</summary>
    ''' <param name="dtSelect"> 検索するデータテーブル</param>
    ''' <param name="sSelectValue">検索する内容</param>
    ''' <returns>datatableから前の画面から渡された値はdtにあるインデックスを検出する</returns>
    Private Function getIndex(ByVal dtSelect As DataTable, ByVal sSelectValue As String) As Integer

        'インデックスの値
        Dim nIndex As Integer = 0
        Dim i As Integer = 0

        For i = 0 To dtSelect.Rows.Count - 1
            If dtSelect.Rows(i).Item(0).ToString = sSelectValue Then
                nIndex = i
                Exit For
            End If
        Next

        'インデックスの値
        Return nIndex

    End Function

    ''' <summary>DataTableの先頭に、空白行を追加する。</summary>
    ''' <remarks>
    '''  DataTableの先頭に、空白行を追加する。
    ''' </remarks>
    ''' <returns>マスタデータマスタ取得結果格納テーブル</returns>
    Public Function SetSpace(ByVal dt As DataTable) As DataTable
        Dim drw As DataRow

        drw = dt.NewRow()

        'DataTableの先頭に、空白行を追加する。
        For i As Integer = 0 To dt.Columns.Count - 1
            drw.Item(i) = ""
        Next

        dt.Rows.InsertAt(drw, 0)

        Return dt
    End Function

#End Region

#Region "メソッド"

    ''' <summary>パターンの検索する、Eltableの内容を表示する。</summary>
    Private Sub selectPattern(ByVal bPattern As Boolean)

        'ELTableに表示されているデータを格納する。
        Dim dtPatternTable As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer
        Try
            'Eltableのすべてのデータを取得する。
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dtPatternTable)

            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    btnUpdate.Enabled = False
                    btnDelet.Enabled = False
                    btnSearch.Select()
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                Case 0
                    btnUpdate.Enabled = False
                    btnDelet.Enabled = False
                    If bPattern = True Then
                        '検索条件に一致するデータは存在しません。
                        AlertBox.Show(Lexis.NoRecordsFound)
                    End If
                Case Else
                    btnInsert.Enabled = True
                    btnUpdate.Enabled = True
                    btnDelet.Enabled = True
                    shtMain.Enabled = True
                    'Eltableの内容を表示する。
                    Call LfSetSheetData(dtPatternTable)
            End Select

        Catch ex As OPMGException
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New Exception
        End Try
    End Sub

    ''' <summary>
    ''' [一覧クリア]
    ''' </summary>
    Private Sub LfClrList()
        shtMain.Redraw = False
        wbkIDMst.Redraw = False
        Try
            'NOTE: この中で例外が発生した場合のログ出力と
            'メッセージボックス表示は、このメソッドの
            '呼び元で行う。ただし、InitFrm メソッドでは
            'メッセージボックスの表示は行わない（他の
            '画面に合わせる）。

            Dim i As Integer
            'ソート情報のクリア
            With shtMain
                For i = 0 To LcstMaxColCnt - 1
                    .ColumnHeaders(i).Image = Nothing
                    .Columns(i).BackColor = Color.Empty
                Next
            End With

            shtMain.DataSource = Nothing
            shtMain.MaxRows = 0

            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnDelet.Enabled = True Then btnDelet.Enabled = False
            If btnUpdate.Enabled = True Then btnUpdate.Enabled = False
        Finally
            shtMain.Redraw = True
            wbkIDMst.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' ELTableのマウスの移動事件
    ''' </summary>
    Private Sub shtPatternMst_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
        'マウスカーソルが列ヘッダ上にある場合
        If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
            shtMain.CrossCursor = Cursors.Default
        Else
            'マウスカーソルを既定に戻す
            shtMain.CrossCursor = Nothing
        End If
    End Sub

    ''' <summary>コンボボックスに対してデータバインドを行う</summary>
    ''' <param name="dt">バインド用のDataTable</param>
    ''' <param name="cmb">バインド必要のあるComboBox</param>
    ''' <remarks>
    ''' 表示メンバー、バリューメンバーとDataSourceを設定する。
    ''' </remarks>
    Private Sub setComboxValue(ByVal dt As DataTable, ByRef cmb As ComboBox)

        'comboxに対してデータバインドを行うに失敗しました。
        If cmb Is Nothing Then
            '画面表示処理に失敗しました。
            FrmBase.LogOperation(Lexis.FormProcAbnormalEnd) 'TODO: 少なくとも「操作」ログではない。詳細設計も含め確認。
            Throw New OPMGException()
        End If

        Try
            With cmb
                'DataSourceの設定
                .DataSource = dt
                '表示メンバーの設定
                .DisplayMember = dt.Columns(1).ColumnName
                'バリューメンバーの設定
                .ValueMember = dt.Columns(0).ColumnName
            End With

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '予期せぬエラーが発生しました。
            Throw New OPMGException(ex)
        End Try

    End Sub

    ''' <summary>
    ''' コンボボックスの状態の設定
    ''' </summary>
    ''' <param name="bCmbModelname">「マスタ」コンボボックスの活性化のフラグ</param>
    ''' <param name="bCmbMstname ">「プログラム」コンボボックスの活性化のフラグ</param>
    ''' <remarks></remarks>
    Private Sub setComboStatus(ByVal bCmbModelname As Boolean, _
                               ByVal bCmbMstname As Boolean)

        Me.cmbModelname.Enabled = bCmbModelname
        If (bCmbModelname = False) Then
            If Me.cmbModelname.SelectedIndex > 0 Then
                Me.cmbModelname.SelectedIndex = 0
            End If
        End If

        Me.cmbMstname.Enabled = bCmbMstname
        If (bCmbMstname = False) Then
            If Me.cmbMstname.SelectedIndex > 0 Then
                Me.cmbMstname.SelectedIndex = 0
            End If
        End If

    End Sub

    ''' <summary>ボタンの状態の設定</summary>
    ''' <param name="bBtnSelect">「検索」ボタンの活性化のフラグ</param>
    ''' <param name="bBtnAddNew">「登録」ボタンの活性化のフラグ</param>
    ''' <param name="bBtnUpdate">「修正」ボタンの活性化のフラグ</param>
    ''' <param name="bBtnDelete">「削除」ボタンの活性化のフラグ</param>
    Private Sub setBtnStatus(ByVal bBtnSelect As Boolean, ByVal bBtnAddNew As Boolean, _
                             ByVal bBtnUpdate As Boolean, ByVal bBtnDelete As Boolean)
        Me.btnSearch.Enabled = bBtnSelect
        Me.btnInsert.Enabled = bBtnAddNew
        Me.btnUpdate.Enabled = bBtnUpdate
        Me.btnDelet.Enabled = bBtnDelete
    End Sub

    ''' <summary>Eltableの内容を表示する</summary>
    ''' <remarks>
    ''' エリアNo、エリア名称を表示する。
    ''' </remarks>
    Private Sub LfSetSheetData(ByVal dtPatternTable As DataTable)

        '画面の閃きを防ぐ。
        Me.shtMain.Redraw = False
        Me.wbkIDMst.Redraw = False
        Try
            Me.shtMain.MaxRows = dtPatternTable.Rows.Count     '抽出件数分の行を一覧に作成

            Me.shtMain.DataSource = dtPatternTable             '行高さを揃える

            shtMain.Rows.SetAllRowsHeight(21)              'データをセット

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            btnInsert.Select()
        Finally
            'Eltableを再表示する。
            Me.shtMain.Redraw = True
            Me.wbkIDMst.Redraw = True
        End Try

    End Sub

#End Region

#Region " コンボボックスのイベント "


    ''' <summary>
    ''' 「機種」の選択によって、「プログラム名称」を取得する。
    ''' </summary>

    Private Sub cmbModelname_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbModelname.SelectedIndexChanged
        If LbEventStop = True Then Exit Sub
        LfWaitCursor()
        Dim nCmbIndex As Integer = 0
        Try
            LbEventStop = True
            'ELTableの初期化。
            Call initElTable()

            If (Me.cmbModelname.SelectedIndex < 0) Then
                Exit Sub
            End If

            If cmbModelname.SelectedIndex = 0 Then
                setComboStatus(True, False)
                setBtnStatus(False, False, False, False)
                Exit Sub
            ElseIf cmbModelname.SelectedIndex > 0 Then
                setComboStatus(True, True)
                setBtnStatus(False, False, False, False)
            End If

            '検索したマスタデータを格納する。
            If cmbModelname.SelectedValue.ToString <> "" Then
                If LfSetCmbMstName(cmbModelname.SelectedValue.ToString) = False Then
                    If btnSearch.Enabled = True Then btnSearch.Enabled = False
                    If cmbMstname.Enabled = True Then cmbMstname.Enabled = False
                    'エラーメッセージ
                    AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMstName.Text)
                    LbEventStop = False      'イベント発生ＯＮ
                    btnReturn.Select()
                    Exit Sub
                End If
            Else
                cmbMstname.SelectedIndex = 0
                cmbMstname.Enabled = False
            End If

            cmbMstname.SelectedIndex = 0               '★イベント発生箇所
            If cmbMstname.Enabled = False Then cmbMstname.Enabled = True
            LbEventStop = False      'イベント発生ＯＮ
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '予期せぬエラーが発生しました。
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMstName.Text)
            If btnSearch.Enabled = True Then btnSearch.Enabled = False
            cmbMstname.SelectedIndex = 0
            cmbMstname.Enabled = False
            LbEventStop = False      'イベント発生ＯＮ
            btnReturn.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 「マスタ」の選択時、ELTableのクリアする。
    ''' </summary>
    Private Sub cmbMstname_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbMstname.SelectedIndexChanged
        If LbEventStop = True Then Exit Sub
        LfWaitCursor()
        Dim nCmbIndex As Integer = 0
        Try
            LbEventStop = True
            Call initElTable()

            If (Me.cmbMstname.SelectedIndex < 0) Then
                Exit Sub
            End If

            ' 一番目項目「スベース」を選択する時の処理
            nCmbIndex = Me.cmbMstname.SelectedIndex

            If nCmbIndex = 0 Then
                setBtnStatus(False, False, False, False)
            Else
                setBtnStatus(True, False, False, False)
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '予期せぬエラーが発生しました。 '予期せぬエラーが発生しました。
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

#End Region

End Class