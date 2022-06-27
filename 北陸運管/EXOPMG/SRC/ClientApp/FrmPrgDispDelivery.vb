' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
'   0.1      2013/11/11  (NES)金沢    フェーズ２受信完了日”-”出力対応
'   0.2      2014/06/01       金沢    一覧ソート対応
' **********************************************************************

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO

''' <summary>
''' プログラム配信状況表示
''' </summary>
''' <remarks>プログラム管理メニューより、「配信状況表示」ボタンをクリックすることにより、
''' 本画面を表示する。確認したい「機種名称」「マスタ名称」「パターン名称」「バージョン」を選択し、
''' 「検索」をクリックすることにより、当該データの表示を行う。</remarks>
Public Class FrmPrgDispDelivery
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
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents WorkBook1 As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents lblPrograme As System.Windows.Forms.Label
    Friend WithEvents lblArea As System.Windows.Forms.Label
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents cmbVersion As System.Windows.Forms.ComboBox
    Friend WithEvents cmbPrgName As System.Windows.Forms.ComboBox
    Friend WithEvents cmbAppliedArea As System.Windows.Forms.ComboBox
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents lblModel As System.Windows.Forms.Label
    Friend WithEvents shtDspDelivery As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents cmbProgram As System.Windows.Forms.ComboBox
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmPrgDispDelivery))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.WorkBook1 = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtDspDelivery = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.cmbVersion = New System.Windows.Forms.ComboBox()
        Me.cmbPrgName = New System.Windows.Forms.ComboBox()
        Me.cmbAppliedArea = New System.Windows.Forms.ComboBox()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.lblPrograme = New System.Windows.Forms.Label()
        Me.lblArea = New System.Windows.Forms.Label()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.cmbProgram = New System.Windows.Forms.ComboBox()
        Me.pnlBodyBase.SuspendLayout()
        Me.WorkBook1.SuspendLayout()
        CType(Me.shtDspDelivery, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.cmbModel)
        Me.pnlBodyBase.Controls.Add(Me.lblModel)
        Me.pnlBodyBase.Controls.Add(Me.WorkBook1)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.cmbVersion)
        Me.pnlBodyBase.Controls.Add(Me.cmbPrgName)
        Me.pnlBodyBase.Controls.Add(Me.cmbAppliedArea)
        Me.pnlBodyBase.Controls.Add(Me.lblVersion)
        Me.pnlBodyBase.Controls.Add(Me.lblPrograme)
        Me.pnlBodyBase.Controls.Add(Me.lblArea)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnKensaku)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/11/08(金)  16:37"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'WorkBook1
        '
        Me.WorkBook1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.WorkBook1.Controls.Add(Me.shtDspDelivery)
        Me.WorkBook1.Location = New System.Drawing.Point(21, 84)
        Me.WorkBook1.Name = "WorkBook1"
        Me.WorkBook1.ProcessTabKey = False
        Me.WorkBook1.ShowTabs = False
        Me.WorkBook1.Size = New System.Drawing.Size(866, 458)
        Me.WorkBook1.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.WorkBook1.TabIndex = 0
        '
        'shtDspDelivery
        '
        Me.shtDspDelivery.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtDspDelivery.Data = CType(resources.GetObject("shtDspDelivery.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtDspDelivery.Location = New System.Drawing.Point(1, 1)
        Me.shtDspDelivery.Name = "shtDspDelivery"
        Me.shtDspDelivery.Size = New System.Drawing.Size(847, 439)
        Me.shtDspDelivery.TabIndex = 0
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(707, 584)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 6
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'cmbVersion
        '
        Me.cmbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbVersion.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbVersion.ItemHeight = 13
        Me.cmbVersion.Items.AddRange(New Object() {""})
        Me.cmbVersion.Location = New System.Drawing.Point(533, 50)
        Me.cmbVersion.Name = "cmbVersion"
        Me.cmbVersion.Size = New System.Drawing.Size(100, 21)
        Me.cmbVersion.TabIndex = 4
        '
        'cmbPrgName
        '
        Me.cmbPrgName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPrgName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbPrgName.ItemHeight = 13
        Me.cmbPrgName.Items.AddRange(New Object() {""})
        Me.cmbPrgName.Location = New System.Drawing.Point(153, 50)
        Me.cmbPrgName.Name = "cmbPrgName"
        Me.cmbPrgName.Size = New System.Drawing.Size(242, 21)
        Me.cmbPrgName.TabIndex = 3
        '
        'cmbAppliedArea
        '
        Me.cmbAppliedArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAppliedArea.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbAppliedArea.ItemHeight = 13
        Me.cmbAppliedArea.Items.AddRange(New Object() {""})
        Me.cmbAppliedArea.Location = New System.Drawing.Point(533, 20)
        Me.cmbAppliedArea.Name = "cmbAppliedArea"
        Me.cmbAppliedArea.Size = New System.Drawing.Size(198, 21)
        Me.cmbAppliedArea.TabIndex = 2
        '
        'lblVersion
        '
        Me.lblVersion.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblVersion.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblVersion.Location = New System.Drawing.Point(424, 50)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(80, 18)
        Me.lblVersion.TabIndex = 91
        Me.lblVersion.Text = "バージョン"
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPrograme
        '
        Me.lblPrograme.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrograme.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrograme.Location = New System.Drawing.Point(45, 50)
        Me.lblPrograme.Name = "lblPrograme"
        Me.lblPrograme.Size = New System.Drawing.Size(107, 18)
        Me.lblPrograme.TabIndex = 90
        Me.lblPrograme.Text = "プログラム名称"
        Me.lblPrograme.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblArea
        '
        Me.lblArea.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblArea.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblArea.Location = New System.Drawing.Point(424, 20)
        Me.lblArea.Name = "lblArea"
        Me.lblArea.Size = New System.Drawing.Size(105, 18)
        Me.lblArea.TabIndex = 89
        Me.lblArea.Text = "適用エリア名称"
        Me.lblArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 7
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(872, 20)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 5
        Me.btnKensaku.Text = "検　索"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.ItemHeight = 13
        Me.cmbModel.Items.AddRange(New Object() {""})
        Me.cmbModel.Location = New System.Drawing.Point(153, 20)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(106, 21)
        Me.cmbModel.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(45, 20)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(91, 18)
        Me.lblModel.TabIndex = 96
        Me.lblModel.Text = "機種"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbProgram
        '
        Me.cmbProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProgram.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbProgram.ItemHeight = 13
        Me.cmbProgram.Items.AddRange(New Object() {""})
        Me.cmbProgram.Location = New System.Drawing.Point(460, 20)
        Me.cmbProgram.Name = "cmbProgram"
        Me.cmbProgram.Size = New System.Drawing.Size(198, 21)
        Me.cmbProgram.TabIndex = 1
        '
        'FrmPrgDispDelivery
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgDispDelivery"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.WorkBook1.ResumeLayout(False)
        CType(Me.shtDspDelivery, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "各種宣言領域"

    'プログラム名称
    Public Const APPLIED_AREANAME As String = "適用エリア名称："


    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    ''' 出力用テンプレートファイル名
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "プログラム配信状況.xls"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "プログラム配信状況"

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "プログラム配信状況表示"

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した別集札データに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3, 4, 5, 6, 7, 8}
    '-------Ver0.2　一覧ソート対応　ADD START-----------
    ''' <summary>
    ''' 一覧ヘッダのソート列割り当て
    ''' （一覧ヘッダクリック時に割り当てる対象列を定義。列番号はゼロ相対で"-1"はソート対象外の列）
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, -1, -1, -1, 4, -1, -1, 7, 8}
    '-------Ver0.2　一覧ソート対応　ADD END-----------
    ''' <summary>
    ''' 一覧表示最大列
    ''' </summary>
    Private LcstMaxColCnt As Integer

    Private LbInitCallFlg As Boolean = False

#End Region

#Region "フォームロード"
    'フォームロード
    Private Sub FrmPrgDispDelivery_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LfWaitCursor()
        If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
            If InitFrmData() = False Then   '初期処理
                Me.Close()
                Exit Sub
            End If
        End If

        LfWaitCursor(False)
    End Sub

    ''' <summary>
    ''' マスタ配信状況表示画面のデータを準備する
    ''' </summary>
    ''' <remarks>
    ''' マスタ配信状況表示設定データを検索し、画面に表示する
    ''' </remarks>
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ

        Try
            Log.Info("Method started.")

            '画面タイトル
            lblTitle.Text = LcstFormTitle

            'シート初期化
            shtDspDelivery.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtDspDelivery.ViewMode = ElTabelleSheet.ViewMode.Row                      '行選択モード
            shtDspDelivery.MaxRows() = 0                                               '行の初期化
            LcstMaxColCnt = shtDspDelivery.MaxColumns()                                '列数を取得
            shtDspDelivery.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード
            shtDspDelivery.ColumnHeaders(2, 0).Caption = " "
            shtDspDelivery.ColumnHeaders(6, 0).Caption = " "
            '-------Ver0.2　一覧ソート対応　ADD START-----------
            'シートのヘッダ選択イベントのハンドラ追加
            shtDspDelivery.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtDspDelivery.ColumnHeaders.HeaderClick, AddressOf Me.shtDspDeliveryColumnHeaders_HeadersClick
            '-------Ver0.2　一覧ソート対応　ADD END-----------

            '機種名称を設定する。
            If setCmbModel() = False Then Exit Try
            cmbModel.SelectedIndex = 0            'デフォルト表示項目

            'エリア名称を設定する。
            If setCmbAreaName(Me.cmbModel.SelectedValue.ToString) = False Then Exit Try
            cmbAppliedArea.SelectedIndex = 0      'デフォルト表示項目

            'マスタ名称を設定する。
            If setCmbProgram(Me.cmbModel.SelectedValue.ToString, Me.cmbAppliedArea.SelectedValue.ToString) = False Then Exit Try
            cmbPrgName.SelectedIndex = 0          'デフォルト表示項目

            'ボタン「検 索」、「出 力」の利用可能性を設定する。
            Call enableBtn()

            bRtn = True

        Catch ex As DatabaseException
            '画面表示処理に失敗しました
            bRtn = False

        Catch ex As Exception
            '画面表示処理に失敗しました
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

#Region "コンボクリック"

    ' 「機種名称」コンボクリック
    Private Sub cmbModel_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            Select Case cmbModel.SelectedValue.ToString
                Case "G"
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = "監視盤"
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = "改札機"
                Case "Y"
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = "明細収集／ＥＸ統括"
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = "窓口処理機"
                Case "W"
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = "監視盤"
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = "監視盤"
                Case Else
                    shtDspDelivery.ColumnHeaders(2, 0).Caption = " "
                    shtDspDelivery.ColumnHeaders(6, 0).Caption = " "
            End Select

            'コーナーコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbAreaName(cmbModel.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblArea.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbAppliedArea.SelectedIndex = 0               '★イベント発生箇所
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblModel.Text)
        Finally
            LfWaitCursor(False)
        End Try


    End Sub

    '「適用エリア名称」コンボクリック
    Private Sub cmbAppliedArea_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbAppliedArea.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            'プログラムコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbProgram(cmbModel.SelectedValue.ToString, cmbAppliedArea.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblPrograme.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbPrgName.SelectedIndex = 0               '★イベント発生箇所

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblArea.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    '「プログラム名称」コンボクリック
    Private Sub cmbPrgName_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbPrgName.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            'プログラムコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbVer(cmbModel.SelectedValue.ToString, cmbAppliedArea.SelectedValue.ToString, cmbPrgName.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblVersion.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbVersion.SelectedIndex = 0               '★イベント発生箇所

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblPrograme.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    '「バージョン名称」コンボクリック
    Private Sub CmbVersion_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbVersion.SelectedIndexChanged
        If LbEventStop Then Exit Sub

        'Eltableに既存したデータをクリアする。
        clearEltable(shtDspDelivery)

        'ボタン「検 索」、「出 力」の利用可能性を設定する。
        Call enableBtn()

        If cmbVersion.SelectedIndex = 0 Then
            Exit Sub
        Else
            'ボタン「検 索」、「出 力」の利用可能性を設定する。
            Call enableBtn(True, False)
        End If

    End Sub
#End Region

#Region "検索を行う"
    ''' <summary>
    ''' 「検索」ボタンをクリックすることにより、検索条件に一致するデータを画面に表示する。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>「プログラム名称」コンボクリックし、「機種名称」「マスタ名称」「パターン名称」
    ''' 　　　　　「バージョン」を検索条件として、DBから配信先の駅の一覧及び配信状況を抽出する。
    ''' </remarks>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKensaku.Click
        Dim dtEltData As DataTable = Nothing

        LogOperation(sender, e)    'ボタン押下ログ
        Call Me.waitCursor(True)

        Try
            'ボタン「検 索」、「出 力」の利用可能性を設定する。
            Call enableBtn(True, False)

            clearEltable(shtDspDelivery)

            dtEltData = getEltableData(cmbModel.SelectedValue.ToString, cmbPrgName.SelectedValue.ToString.Substring(0, 3), _
                            cmbPrgName.SelectedValue.ToString.Substring(3, 3), cmbAppliedArea.SelectedValue.ToString, cmbVersion.Text)

            If dtEltData.Rows.Count <= 0 Then
                '検索条件に一致するデータは存在しません。
                AlertBox.Show(Lexis.NoRecordsFound)
                Exit Sub
            End If

            FillData(shtDspDelivery, dtEltData)

            'ボタン「検 索」、「出 力」の利用可能性を設定する。
            Call enableBtn(True, True)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            dtEltData = Nothing
            Call Me.waitCursor(False)
        End Try
    End Sub
#End Region

#Region "Eltableの表示"

    ''' <summary>
    ''' Eltable用のデータを取得する。
    ''' </summary>
    ''' <returns>Eltable用のデータ</returns>
    ''' <remarks>Eltable用のデータを取得する。</remarks>
    Function getEltableData(ByVal sMdlCd As String, ByVal sKbn As String, ByVal sMstKind As String, _
                            ByVal sArea As String, ByVal sVerNo As String) As DataTable

        Dim dtReturn As DataTable
        Dim dbCtl As DatabaseTalker
        Dim sDllMdl As String
        Dim sSql As String

        Select Case sMdlCd
            Case "G"
                sDllMdl = "W"
            Case "Y"
                sDllMdl = "X"
            Case Else
                sDllMdl = "W"
        End Select
        '-------Ver0.1　フェーズ２　受信完了日の”-”出力対応でSQL文修正　MOD START-----------
        '  CASE" _
        '& "                 WHEN DELIVERY_STS = 0 AND DELIVERY_END_TIME IS NOT NULL AND DELIVERY_END_TIME <> '' THEN" _
        '& "                       SUBSTRING(DELIVERY_END_TIME,1,4)+'/'+SUBSTRING(DELIVERY_END_TIME,5,2)+'/'" _
        '& "                 +SUBSTRING(DELIVERY_END_TIME,7,2)+' '+SUBSTRING(DELIVERY_END_TIME,9,2)+':'" _
        '& "                 +SUBSTRING(DELIVERY_END_TIME,11,2)+':'+SUBSTRING(DELIVERY_END_TIME,13,2)" _
        '& "                 ELSE  '-'" _
        '& "             END AS END_TIME," _
        '-------Ver0.1　フェーズ２　受信完了日の”-”出力対応でSQL文修正　MOD END-------------

        sSql = "SELECT" _
        & "     CASE" _
        & "         WHEN LTRIM(Isnull(STR(DL_DATA.UNIT_NO),'')) = '' THEN DLL_DATA.STATION_NAME" _
        & "         ELSE DL_DATA.STATION_NAME" _
        & "     END AS STATION_NAME," _
        & "     CASE" _
        & "         WHEN LTRIM(Isnull(STR(DL_DATA.UNIT_NO),'')) = '' THEN DLL_DATA.CORNER_NAME" _
        & "         ELSE DL_DATA.CORNER_NAME" _
        & "     END AS CORNER_NAME," _
        & "     DLL_DATA.UNIT_NO,DLL_DATA.START_TIME,DLL_DATA.END_TIME,DLL_DATA.STS," _
        & "     LTRIM(Isnull(STR(DL_DATA.UNIT_NO),'')) AS UNIT_NO2," _
        & "     Isnull(DL_DATA.END_TIME,'') AS END_TIME2,Isnull(DL_DATA.STS,'') AS STS2" _
        & " FROM" _
        & "     (" _
        & "         SELECT" _
        & "             MAC.STATION_NAME,MAC.CORNER_NAME,DLL.UNIT_NO,DLL.START_TIME,DLL.END_TIME," _
        & "             DLL.STS,MAC.ADDRESS" _
        & "         FROM" _
        & "             (" _
        & "                 SELECT" _
        & "                     STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_NAME," _
        & "                     CORNER_CODE,MODEL_CODE,UNIT_NO,ADDRESS" _
        & "                 FROM" _
        & "                     V_MACHINE_NOW" _
        & "             ) AS MAC," _
        & "             (" _
        & "                 SELECT" _
        & "                     RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO," _
        & "                     SUBSTRING(DELIVERY_START_TIME,1,4)+'/'+SUBSTRING(DELIVERY_START_TIME,5,2)+'/'" _
        & "                     +SUBSTRING(DELIVERY_START_TIME,7,2)+' '+SUBSTRING(DELIVERY_START_TIME,9,2)+':'" _
        & "                     +SUBSTRING(DELIVERY_START_TIME,11,2)+':'+SUBSTRING(DELIVERY_START_TIME,13,2)" _
        & "                     AS START_TIME," _
        & "                     CASE" _
        & "                         WHEN DELIVERY_END_TIME IS NULL" _
        & "                     OR  DELIVERY_END_TIME = '' THEN ''" _
        & "                     ELSE SUBSTRING(DELIVERY_END_TIME,1,4)+'/'+SUBSTRING(DELIVERY_END_TIME,5,2)+'/'" _
        & "                         +SUBSTRING(DELIVERY_END_TIME,7,2)+' '+SUBSTRING(DELIVERY_END_TIME,9,2)+':'" _
        & "                         +SUBSTRING(DELIVERY_END_TIME,11,2)+':'+SUBSTRING(DELIVERY_END_TIME,13,2)" _
        & "                     END AS END_TIME," _
        & "                     CASE DELIVERY_STS" _
        & "                         WHEN 0 THEN '正常'" _
        & "                         WHEN 1 THEN '異常'" _
        & "                         WHEN 2 THEN '不正ﾃﾞｰﾀ'" _
        & "                         WHEN 3 THEN 'ﾀｲﾑｱｳﾄ'" _
        & "                         WHEN 65535 THEN '配信中'" _
        & "                     ELSE '['+LTRIM(STR(DELIVERY_STS))+']'" _
        & "                     END AS STS" _
        & "                 FROM" _
        & "                     S_PRG_DLL_STS" _
        & "                 WHERE" _
        & "                     RAIL_SECTION_CODE+STATION_ORDER_CODE<>'000000' AND MODEL_CODE='" & sDllMdl & "'" _
        & "                 AND FILE_KBN='" & sKbn & "' AND DATA_KIND='" & sMstKind & "'" _
        & "                 AND DATA_SUB_KIND='" & sArea & "' AND VERSION='" & sVerNo & "'" _
        & "             ) AS DLL" _
        & "         WHERE" _
        & "             MAC.RAIL_SECTION_CODE=DLL.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE=DLL.STATION_ORDER_CODE" _
        & "         AND MAC.CORNER_CODE=DLL.CORNER_CODE AND MAC.MODEL_CODE=DLL.MODEL_CODE" _
        & "         AND MAC.UNIT_NO=DLL.UNIT_NO" _
        & "     ) AS DLL_DATA" _
        & "     LEFT OUTER JOIN" _
        & "         (" _
        & "             SELECT" _
        & "                 MAC.STATION_NAME,MAC.CORNER_NAME,MAC.MONITOR_ADDRESS,DL2.UNIT_NO,DL2.END_TIME,DL2.STS" _
        & "             FROM" _
        & "                 (" _
        & "                     SELECT" _
        & "                         STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,CORNER_NAME," _
        & "                         MODEL_CODE,UNIT_NO,MONITOR_ADDRESS" _
        & "                     FROM" _
        & "                         V_MACHINE_NOW" _
        & "                 ) AS MAC," _
        & "                 (" _
        & "                     SELECT" _
        & "                         RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,DL.MODEL_CODE," _
        & "                         UNIT_NO," _
        & "             CASE" _
        & "                 WHEN DELIVERY_STS = 0 AND DELIVERY_END_TIME IS NOT NULL AND DELIVERY_END_TIME <> '' THEN" _
        & "                       SUBSTRING(DELIVERY_END_TIME,1,4)+'/'+SUBSTRING(DELIVERY_END_TIME,5,2)+'/'" _
        & "                 +SUBSTRING(DELIVERY_END_TIME,7,2)+' '+SUBSTRING(DELIVERY_END_TIME,9,2)+':'" _
        & "                 +SUBSTRING(DELIVERY_END_TIME,11,2)+':'+SUBSTRING(DELIVERY_END_TIME,13,2)" _
        & "                 ELSE  '-'" _
        & "             END AS END_TIME," _
        & "                         CASE" _
        & "                             WHEN ST.STS_NAME IS NULL THEN '['+LTRIM(STR(DL.DELIVERY_STS))+']'" _
        & "                             ELSE ST.STS_NAME" _
        & "                         END AS STS" _
        & "                     FROM" _
        & "                         S_PRG_DL_STS AS DL" _
        & "                         LEFT OUTER JOIN" _
        & "                             M_PRG_DL_DELIVERY_STS_NAME AS ST" _
        & "                         ON  ST.STS = DL.DELIVERY_STS" _
        & "                     WHERE" _
        & "                         ST.MODEL_CODE='" & sMdlCd & "' AND ST.FILE_KBN='" & sKbn & "'" _
        & "                     AND DL.MODEL_CODE='" & sMdlCd & "' AND DL.FILE_KBN='" & sKbn & "'" _
        & "                     AND DL.DATA_KIND='" & sMstKind & "' AND DL.VERSION='" & sVerNo & "'" _
        & "                 ) AS DL2" _
        & "             WHERE" _
        & "                 MAC.RAIL_SECTION_CODE=DL2.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE=DL2.STATION_ORDER_CODE" _
        & "             AND MAC.CORNER_CODE=DL2.CORNER_CODE AND MAC.MODEL_CODE=DL2.MODEL_CODE" _
        & "             AND MAC.UNIT_NO = DL2.UNIT_NO" _
        & "         ) AS DL_DATA" _
        & "     ON  DLL_DATA.ADDRESS = DL_DATA.MONITOR_ADDRESS"

        dbCtl = New DatabaseTalker

        Try

            dbCtl.ConnectOpen()
            dtReturn = dbCtl.ExecuteSQLToRead(sSql)

        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dtReturn
    End Function

    Private Sub FillData(ByVal target As GrapeCity.Win.ElTabelleSheet.Sheet, ByVal dtEltData As DataTable)
        target.Redraw = False
        'サンプルデータを入力する
        With target
            .DataSource = dtEltData
        End With

        target.Rows.SetAllRowsHeight(21)
        btnPrint.Enabled = True

        '画面の閃きを防ぐ。
        target.Redraw = True
    End Sub
#End Region

#Region "終了ボタン"
    '「終了」ボタンクリック
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()
    End Sub
#End Region

#Region "コンボクリック値を設定する"
    '機種名称を設定する
    Private Function setCmbModel() As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New ModelMaster

        Try
            '機種名称コンボボックス用のデータを取得する。
            dt = oMst.SelectTable(True)
            If dt.Rows.Count = 0 Then
                '機種データ取得失敗
                Return bRtn
            End If
            dt = oMst.SetSpace()

            bRtn = BaseSetMstDtToCmb(dt, cmbModel)
            cmbModel.SelectedIndex = -1
            If cmbModel.Items.Count <= 0 Then bRtn = False

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
    ''' 適用エリア名称コンボボックスを設定する。
    ''' </summary>
    ''' <param name="Model">機種コード</param>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理しているパターン名称の一覧及び「空白」を設定する。</remarks>
    Private Function setCmbAreaName(ByVal Model As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As AreaMaster
        oMst = New AreaMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If (Model <> "") Then
                dt = oMst.SelectTable(Model)
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbAppliedArea)
            cmbAppliedArea.SelectedIndex = -1
            If cmbAppliedArea.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function


    'プログラムを設定する
    Private Function setCmbProgram(ByVal Model As String, ByVal Area As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As ProgramMaster
        oMst = New ProgramMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If String.IsNullOrEmpty(Area) Then
                Area = ""
            End If
            If (Model <> "" AndAlso Area <> "") Then
                dt = oMst.SelectTable(Model, True)
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbPrgName)
            cmbPrgName.SelectedIndex = -1
            If cmbPrgName.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn
    End Function

    Private Function setCmbVer(ByVal Model As String, ByVal Area As String, ByVal Program As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As ProgramVersionMaster
        oMst = New ProgramVersionMaster
        Try
            If String.IsNullOrEmpty(Model) Then
                Model = ""
            End If
            If String.IsNullOrEmpty(Area) Then
                Area = ""
            End If
            If String.IsNullOrEmpty(Program) Then
                Program = ""
            End If
            If (Model <> "" AndAlso Area <> "" AndAlso Program <> "") Then
                dt = oMst.SelectTable(Model, Area, Program.Substring(0, 3), Program.Substring(3, 3))
            End If
            dt = oMst.SetSpace()
            bRtn = BaseSetMstDtToCmb(dt, cmbVersion)
            cmbVersion.SelectedIndex = -1
            If cmbVersion.Items.Count <= 0 Then bRtn = False
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

#Region "ボタン「検 索」、「出 力」の利用可能性を設定する。"
    ''' <summary>
    ''' ボタン「検 索」、「出 力」の利用可能性を設定する。
    ''' </summary>
    ''' <param name="bKensaku">「検 索」ボタン</param>
    ''' <param name="bPrint">「出 力」ボタン</param>
    ''' <remarks></remarks>
    Private Sub enableBtn(Optional ByVal bKensaku As Boolean = False, Optional ByVal bPrint As Boolean = False)
        Me.btnKensaku.Enabled = bKensaku
        Me.btnPrint.Enabled = bPrint
        '-------Ver0.2　一覧ソート対応　ADD START-----------
        Me.shtDspDelivery.Enabled = bPrint
        '-------Ver0.2　一覧ソート対応　ADD END-----------
    End Sub

#End Region

#Region "ELTableの初期化"
    ''' <summary>
    ''' ELTableの初期化
    ''' </summary>
    ''' <param name="target"></param>
    ''' <remarks>Eltableに既存したデータをクリアする。</remarks>
    Private Sub clearEltable(ByVal target As GrapeCity.Win.ElTabelleSheet.Sheet)

        'Eltableのカレントの最大桁数
        Dim sXYRange As String = ""

        '画面の閃きを防ぐため
        shtDspDelivery.Redraw = False

        If shtDspDelivery.MaxRows > 0 Then
            'Eltableのカレントの最大桁数を取得する。
            sXYRange = "1:" & shtDspDelivery.MaxRows.ToString

            '選択されたエリアのデータをクリアする。
            shtDspDelivery.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
        End If
        '-------Ver0.2　一覧ソート対応　ADD START-----------
        Dim i As Integer
        'ソート情報のクリア
        With shtDspDelivery
            For i = 0 To LcstMaxColCnt - 1
                .ColumnHeaders(i).Image = Nothing
                .Columns(i).BackColor = Color.Empty
            Next
        End With
        '-------Ver0.2　一覧ソート対応　ADD END-----------
        shtDspDelivery.MaxRows = 0

        btnKensaku.Enabled = False
        btnPrint.Enabled = False

        '画面の閃きを防ぐため
        shtDspDelivery.Redraw = True

    End Sub
#End Region
    '-------Ver0.2　一覧ソート対応　ADD START-----------
#Region "一覧ソート"
    ''' <summary>
    ''' ElTable
    ''' </summary>
    Private Sub shtDspDeliveryColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)
        Static intCurrentSortColumn As Integer = -1
        Static bolColumn1SortOrder(63) As Boolean

        If LcstSortCol(e.Column) = -1 Then Exit Sub

        Try

            shtDspDelivery.BeginUpdate()

            '前回選択された列ヘッダの初期化
            If intCurrentSortColumn > -1 Then
                '列ヘッダのイメージを削除する
                shtDspDelivery.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '列の背景色を初期化する
                shtDspDelivery.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '列のセル罫線を消去する
                shtDspDelivery.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
            End If

            '選択された列番号を保存
            intCurrentSortColumn = e.Column

            'ソートする列の背景色を設定する
            shtDspDelivery.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
            'ソートする列のセル罫線を設定する
            shtDspDelivery.Columns(intCurrentSortColumn).SetBorder( _
                New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                GrapeCity.Win.ElTabelleSheet.Borders.All)

            If bolColumn1SortOrder(intCurrentSortColumn) = False Then
                '列ヘッダのイメージを設定する
                shtDspDelivery.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(1)
                '降順でソートする
                Call SheetSort(shtDspDelivery, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Descending)
                '列のソート状態を保存する
                bolColumn1SortOrder(intCurrentSortColumn) = True
            Else
                '列ヘッダのイメージを設定する
                shtDspDelivery.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(0)
                '昇順でソートする
                Call SheetSort(shtDspDelivery, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Ascending)
                '列のソート状態を保存する
                bolColumn1SortOrder(intCurrentSortColumn) = False
            End If

            shtDspDelivery.EndUpdate()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' MouseMove
    ''' </summary>
    Private Sub shtDspDelivery_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
            'マウスカーソルが列ヘッダ上にある場合
            If shtDspDelivery.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtDspDelivery.CrossCursor = Cursors.Default
            Else
                'マウスカーソルを既定に戻す
                shtDspDelivery.CrossCursor = Nothing
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' ソート
    ''' </summary>
    Private Sub SheetSort(ByRef sheetTarget As GrapeCity.Win.ElTabelleSheet.Sheet, ByVal intKeyColumn As Integer, ByVal sortOrder As GrapeCity.Win.ElTabelleSheet.SortOrder)
        Dim objSortItem As New GrapeCity.Win.ElTabelleSheet.SortItem(intKeyColumn, False, sortOrder)
        Dim objSortList(0) As GrapeCity.Win.ElTabelleSheet.SortItem
        '配列にソートオブジェクトを追加する
        objSortList(0) = objSortItem
        'ソートを実行する
        sheetTarget.Sort(objSortList)
    End Sub
#End Region
    '-------Ver0.2　一覧ソート対応　ADD END-----------

#Region "帳票出力"
    '「出力」ボタンクリック
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            LogOperation(sender, e)    'ボタン押下ログ

            Dim sPath As String = Config.LedgerTemplateDirPath
            'テンプレート格納フォルダチェック
            If Directory.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If

            'テンプレートフルパスチェック
            sPath = Path.Combine(sPath, LcstXlsTemplateName)
            If File.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If

            '出力
            LfXlsStart(sPath)
            cmbModel.Select()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'エラーメッセージ
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub


    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 8
        Try
            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                ' 帳票ファイル名称を取得
                .FileName = sPath
                .ExcelMode = True
                ' 帳票の出力処理を開始を宣言
                .Report.Start()
                .Report.File()
                '帳票ファイルシート名称を取得します。
                .Page.Start(LcstXlsSheetName, "1-9999")

                ' 見出し部セルへ見出しデータ出力
                .Cell("B1").Value = LcstXlsSheetName
                .Cell("J1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("J2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B4").Value = OPMGFormConstants.EQUIPMENT_TYPE_NAME + cmbModel.Text.Trim + "   " _
                                  + APPLIED_AREANAME + cmbAppliedArea.Text.Trim
                .Cell("B5").Value = OPMGFormConstants.PRO_NAME + cmbPrgName.Text.Trim + "  " _
                                  + OPMGFormConstants.VERSION_STR + cmbVersion.Text.Trim
                .Cell("D7").Value = shtDspDelivery.ColumnHeaders(2, 0).Caption
                .Cell("H7").Value = shtDspDelivery.ColumnHeaders(6, 0).Caption

                ' 配信対象のデータ数を取得します
                nRecCnt = shtDspDelivery.MaxRows

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtDspDelivery.Item(LcstPrntCol(x), y).Text
                    Next
                Next

                '出力処理の終了を宣言
                .Page.End()
                .Report.End()

                ' 帳票のプレビューをモーダルダイアログで起動します。
                PrintViewer.GetDocument(XlsReport1.Document)
                PrintViewer.ShowDialog(Me)
                PrintViewer.Dispose()
                Log.Info("Printing finished.")
            End With
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try
    End Sub
#End Region

End Class
