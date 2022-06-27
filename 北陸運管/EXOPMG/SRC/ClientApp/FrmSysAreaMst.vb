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
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.Text

''' <summary>エリア設定</summary>
''' <remarks>
''' エリア設定画面の検索条件によって、エリア情報を表示する。
''' 一覧データを選択することにより、サブ画面にて修正、削除が可能。
''' </remarks>
Public Class FrmSysAreaMst
    Inherits FrmBase

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
    Friend WithEvents istAreaMst As System.Windows.Forms.ImageList
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents wbkMain As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnInsert As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents cmbModelname As System.Windows.Forms.ComboBox
    Friend WithEvents btnSearch As System.Windows.Forms.Button
    Friend WithEvents pnlSelect As System.Windows.Forms.Panel
    Friend WithEvents lblMach As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysAreaMst))
        Me.wbkMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.cmbModelname = New System.Windows.Forms.ComboBox()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.pnlSelect = New System.Windows.Forms.Panel()
        Me.lblMach = New System.Windows.Forms.Label()
        Me.istAreaMst = New System.Windows.Forms.ImageList(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.wbkMain.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlSelect.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.btnSearch)
        Me.pnlBodyBase.Controls.Add(Me.wbkMain)
        Me.pnlBodyBase.Controls.Add(Me.btnInsert)
        Me.pnlBodyBase.Controls.Add(Me.btnUpdate)
        Me.pnlBodyBase.Controls.Add(Me.btnDelete)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.pnlSelect)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/07/31(水)  11:51"
        '
        'wbkMain
        '
        Me.wbkMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wbkMain.Controls.Add(Me.shtMain)
        Me.wbkMain.Location = New System.Drawing.Point(125, 100)
        Me.wbkMain.Name = "wbkMain"
        Me.wbkMain.ProcessTabKey = False
        Me.wbkMain.ShowTabs = False
        Me.wbkMain.Size = New System.Drawing.Size(580, 505)
        Me.wbkMain.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wbkMain.TabIndex = 1
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(1, 1)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(561, 486)
        Me.shtMain.TabIndex = 99
        Me.shtMain.TabStop = False
        '
        'btnInsert
        '
        Me.btnInsert.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnInsert.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnInsert.Location = New System.Drawing.Point(872, 404)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(128, 40)
        Me.btnInsert.TabIndex = 3
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
        Me.btnUpdate.TabIndex = 4
        Me.btnUpdate.Text = "修  正"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(872, 524)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(128, 40)
        Me.btnDelete.TabIndex = 5
        Me.btnDelete.Text = "削  除"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 6
        Me.btnReturn.Text = "終  了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'cmbModelname
        '
        Me.cmbModelname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModelname.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmbModelname.Location = New System.Drawing.Point(45, 10)
        Me.cmbModelname.MaxLength = 5
        Me.cmbModelname.Name = "cmbModelname"
        Me.cmbModelname.Size = New System.Drawing.Size(252, 21)
        Me.cmbModelname.TabIndex = 1
        '
        'btnSearch
        '
        Me.btnSearch.BackColor = System.Drawing.Color.Silver
        Me.btnSearch.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!)
        Me.btnSearch.Location = New System.Drawing.Point(657, 17)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(128, 40)
        Me.btnSearch.TabIndex = 2
        Me.btnSearch.Text = "検  索"
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'pnlSelect
        '
        Me.pnlSelect.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlSelect.Controls.Add(Me.cmbModelname)
        Me.pnlSelect.Controls.Add(Me.lblMach)
        Me.pnlSelect.Location = New System.Drawing.Point(117, 17)
        Me.pnlSelect.Name = "pnlSelect"
        Me.pnlSelect.Size = New System.Drawing.Size(384, 40)
        Me.pnlSelect.TabIndex = 0
        '
        'lblMach
        '
        Me.lblMach.Location = New System.Drawing.Point(4, 16)
        Me.lblMach.Name = "lblMach"
        Me.lblMach.Size = New System.Drawing.Size(45, 19)
        Me.lblMach.TabIndex = 6
        Me.lblMach.Text = "機種"
        '
        'istAreaMst
        '
        Me.istAreaMst.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.istAreaMst.ImageSize = New System.Drawing.Size(16, 16)
        Me.istAreaMst.TransparentColor = System.Drawing.Color.White
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'FrmSysAreaMst
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysAreaMst"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wbkMain.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlSelect.ResumeLayout(False)
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
    Private ReadOnly LcstFormTitle As String = "エリア設定"

    ''' <summary>
    ''' 一覧表示最大列
    ''' </summary>
    Private LcstMaxColCnt As Integer

    ''' <summary>
    ''' 一覧ヘッダのソート列割り当て
    ''' （一覧ヘッダクリック時に割り当てる対象列を定義。列番号はゼロ相対で"-1"はソート対象外の列）
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {0, 1}

#End Region

#Region "メソッド（Public）"

    ''' <summary>
    ''' エリア設定画面のデータを準備する
    ''' </summary>
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ

        Try
            Log.Info("Method started.")

            '業務タイトル表示エリアに画面タイトルをセット
            lblTitle.Text = LcstFormTitle

            'シート初期化
            shtMain.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row                      '行選択モード
            shtMain.MaxRows() = 0                                               '行の初期化
            LcstMaxColCnt = shtMain.MaxColumns()                                '列数を取得
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード
            'シートのヘッダ選択イベントのハンドラ追加
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

            '各コンボボックスの項目登録
            If LfSetCmbModelname() = False Then Exit Try
            cmbModelname.SelectedIndex = 0

            '一覧ソートの初期化
            LfClrList()

            bRtn = True

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd) '開始異常メッセージ
            End If
            LbEventStop = False 'イベント発生ＯＮ
        End Try
        Return bRtn

    End Function
#End Region

#Region "イベント"

    ''' <summary>
    ''' ローディング　メインウィンドウ
    ''' </summary>
    Private Sub FrmSysAreaMst_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrmData() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            '初期化 マスタ名称のrbtnMstを設定する。
            setBtnStatus(False, False, False, False)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 「検索」ボタンを押下すると、Eltableの内容を表示する。
    ''' </summary>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True

            '検索ボタン押下。
            LogOperation(sender, e)

            '一覧シートの初期化（LfClrList）
            LfClrList()

            'エリア情報取得
            Call SelectArea(True)

            btnInsert.Enabled = True
            shtMain.Select()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred)    '検索失敗メッセージ
            btnSearch.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 「登録」ボタンを押下すると、エリア名称を入力画面が表示される。
    ''' </summary>
    Private Sub btnAddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            LbEventStop = True

            '登録ボタン押下。
            LogOperation(sender, e)

            Dim oFrmSysAreaMstAdd As New FrmSysAreaMstAdd
            oFrmSysAreaMstAdd.ModelCode = cmbModelname.SelectedValue.ToString

            oFrmSysAreaMstAdd.ShowDialog()

            'TODO: Form.Newを呼び出して以降に例外が発生した場合のことを
            '考えると、FrmMntDispFaultDataDetailのShowDialogを行うときと同様の
            '方針に統一する方がよいかもしれない。（逆にこちらが正解の可能性もある）
            oFrmSysAreaMstAdd.Dispose()

            '一覧シートの初期化（LfClrList）
            LfClrList()

            'エリア情報取得
            Call SelectArea(False)

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

    ''' <summary>
    ''' 「修正」ボタンを押下すると、エリア名称を変更画面が表示される 。
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            '修正ボタン押下。
            LogOperation(sender, e)

            Dim oFrmSysAreaMstUpdate As New FrmSysAreaMstUpdate

            'FrmSysAreaMstUpdate画面のプロパティに値を代入する。
            Dim nRowno As Integer = shtMain.ActivePosition.Row

            '機種コードを取得する。
            oFrmSysAreaMstUpdate.ModelCode = cmbModelname.SelectedValue.ToString
            'エリアNoを取得する。
            oFrmSysAreaMstUpdate.AreaNo = Me.shtMain.Item(0, nRowno).Text

            If oFrmSysAreaMstUpdate.InitFrmData() = False Then
                oFrmSysAreaMstUpdate = Nothing
                Exit Sub
            End If

            oFrmSysAreaMstUpdate.ShowDialog()
            oFrmSysAreaMstUpdate.Dispose()

            '一覧シートの初期化（LfClrList）
            LfClrList()

            'エリア情報取得
            Call SelectArea(False)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 「削除」ボタンを押下すると、エリア名称を削除画面が表示される。
    ''' </summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True

            '削除ボタン押下。
            LogOperation(sender, e)

            Dim oFrmSysAreaMstDelete As New FrmSysAreaMstDelete

            'oFrmSysAreaMstDelete画面のプロパティに値を代入する。
            Dim nRowno As Integer = shtMain.ActivePosition.Row

            '機種コードを取得する。
            oFrmSysAreaMstDelete.ModelCode() = cmbModelname.SelectedValue.ToString
            'エリアNoを取得する。
            oFrmSysAreaMstDelete.AreaNo() = Me.shtMain.Item(0, nRowno).Text

            If oFrmSysAreaMstDelete.InitFrmData() = False Then
                oFrmSysAreaMstDelete = Nothing
                LfWaitCursor(False)
                Exit Sub
            End If

            oFrmSysAreaMstDelete.ShowDialog()
            oFrmSysAreaMstDelete.Dispose()

            '一覧シートの初期化（LfClrList）
            LfClrList()

            'エリア情報取得
            Call SelectArea(False)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 「終了」ボタンを押下すると、本画面が終了される。
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '終了ボタン押下。
        LogOperation(sender, e)
        Me.Close()
    End Sub

    ''' <summary>
    ''' 「機種」の選択、検索ボタン活性化、登録ボタン、修正ボタン、削除ボタンの非活性化
    ''' </summary>
    Private Sub cmbModelname_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbModelname.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            LfClrList()         '一覧シートの初期化（LfClrList）
            If shtMain.Enabled = True Then shtMain.Enabled = False

            If cmbModelname.SelectedIndex = 0 Then
                setBtnStatus(False, False, False, False)
            Else
                setBtnStatus(True, False, False, False)      '検索ボタン活性化（LfSearchTrue）
            End If

        Catch ex As Exception
            If btnSearch.Enabled = True Then btnSearch.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMach.Text)
        Finally
            LfWaitCursor(False)
            LbEventStop = False
        End Try
    End Sub

    ''' <summary>
    ''' MouseMove
    ''' </summary>
    Private Sub shtMain_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
            'マウスカーソルが列ヘッダ上にある場合
            If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtMain.CrossCursor = Cursors.Default
            Else
                'マウスカーソルを既定に戻す
                shtMain.CrossCursor = Nothing
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

#End Region

#Region "メソッド（Private）"

    ''' <summary>Eltableの内容を表示する</summary>
    ''' <remarks>
    ''' エリアNo、エリア名称を表示する。
    ''' </remarks>
    Private Sub LfSetSheetData(ByVal dtMstTable As DataTable)

        '画面の閃きを防ぐ。
        Me.shtMain.Redraw = False
        Me.wbkMain.Redraw = False
        Try
            Me.shtMain.MaxRows = dtMstTable.Rows.Count     '抽出件数分の行を一覧に作成

            Me.shtMain.DataSource = dtMstTable             '行高さを揃える

            shtMain.Rows.SetAllRowsHeight(21)              'データをセット

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            btnInsert.Select()
        Finally
            'Eltableを再表示する。
            Me.shtMain.Redraw = True
            Me.wbkMain.Redraw = True
        End Try

    End Sub

    ''' <summary>エリア名称を取得する。</summary>
    ''' <returns>
    ''' エリアデータエリア取得結果格納テーブル。
    ''' </returns>
    ''' <remarks>エリアを取得する</remarks>
    Private Function LfGetSelectString() As String

        Dim sSQL As String = ""

        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine("SELECT AREA_NO , AREA_NAME FROM M_AREA_DATA ")
            sBuilder.AppendLine(String.Format("WHERE MODEL_CODE = {0} ORDER BY AREA_NO", Utility.SetSglQuot(cmbModelname.SelectedValue.ToString)))
            sSQL = sBuilder.ToString()

        Catch ex As Exception
            btnSearch.Select()
            Throw New Exception

        End Try

        Return sSQL
    End Function

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
        Me.btnDelete.Enabled = bBtnDelete
    End Sub

    ''' <summary>
    ''' [一覧クリア]
    ''' </summary>
    Private Sub LfClrList()
        shtMain.Redraw = False
        wbkMain.Redraw = False
        Try
            shtMain.DataSource = Nothing
            shtMain.MaxRows = 0

            If shtMain.Enabled = True Then shtMain.Enabled = False
            If btnDelete.Enabled = True Then btnDelete.Enabled = False
            If btnUpdate.Enabled = True Then btnUpdate.Enabled = False
        Finally
            wbkMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' エリアの検索する、Eltableの内容を表示する。
    ''' </summary>
    Private Sub SelectArea(ByVal bArea As Boolean)

        Dim sSql As String = ""
        Dim nRtn As Integer = 1

        'ELTableに表示されているデータを格納する。
        Dim dtAreaTable As New DataTable

        Try
            'Eltableのすべてのデータを取得する。
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dtAreaTable)

            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    btnUpdate.Enabled = False
                    btnDelete.Enabled = False
                    btnSearch.Select()
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                Case 0               'データがない場合
                    btnUpdate.Enabled = False
                    btnDelete.Enabled = False
                    If bArea = True Then
                        '検索条件に一致するデータは存在しません。
                        AlertBox.Show(Lexis.NoRecordsFound)
                    End If
                Case Else
                    btnUpdate.Enabled = True
                    btnDelete.Enabled = True
                    shtMain.Enabled = True

                    'Eltableの内容を表示する。
                    Call LfSetSheetData(dtAreaTable)
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

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
            dt = oMst.SelectTable(True)
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

#End Region

End Class
