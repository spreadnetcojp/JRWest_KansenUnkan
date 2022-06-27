' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2015/04/21  (NES)金沢  帳票出力方法変更
'　　　　　　　　　　　　　　　　　（駅、コーナ毎でシートを分けて出力するように変更）
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '定数値のみ使用
Imports JR.ExOpmg.DataAccess
Imports System
Imports System.IO
Imports System.Text
Imports GrapeCity.Win

''' <summary>
''' 【時間帯別乗降データ出力　画面クラス】
''' </summary>
Public Class FrmMntDispTrafficData
    Inherits FrmBase

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        LcstSearchCol = {Me.cmbEki, Me.cmbMado, Me.dtpYmdFrom, Me.dtpYmdTo}

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
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents pnlFromTo As System.Windows.Forms.Panel
    Friend WithEvents lblFromDate As System.Windows.Forms.Label
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents lblToDate As System.Windows.Forms.Label
    Friend WithEvents dtpYmdFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpYmdTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents pnlMado As System.Windows.Forms.Panel
    Friend WithEvents cmbMado As System.Windows.Forms.ComboBox
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispTrafficData))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.pnlFromTo = New System.Windows.Forms.Panel()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.lblFromDate = New System.Windows.Forms.Label()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.lblToDate = New System.Windows.Forms.Label()
        Me.dtpYmdFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpYmdTo = New System.Windows.Forms.DateTimePicker()
        Me.pnlMado = New System.Windows.Forms.Panel()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.pnlFromTo.SuspendLayout()
        Me.pnlMado.SuspendLayout()
        Me.pnlEki.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.pnlFromTo)
        Me.pnlBodyBase.Controls.Add(Me.pnlMado)
        Me.pnlBodyBase.Controls.Add(Me.pnlEki)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 87)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/07/30(火)  18:54"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(856, 511)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 9
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(856, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 10
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'pnlFromTo
        '
        Me.pnlFromTo.Controls.Add(Me.lblTo)
        Me.pnlFromTo.Controls.Add(Me.lblFromDate)
        Me.pnlFromTo.Controls.Add(Me.lblFrom)
        Me.pnlFromTo.Controls.Add(Me.lblToDate)
        Me.pnlFromTo.Controls.Add(Me.dtpYmdFrom)
        Me.pnlFromTo.Controls.Add(Me.dtpYmdTo)
        Me.pnlFromTo.Location = New System.Drawing.Point(122, 75)
        Me.pnlFromTo.Name = "pnlFromTo"
        Me.pnlFromTo.Size = New System.Drawing.Size(540, 31)
        Me.pnlFromTo.TabIndex = 5
        '
        'lblTo
        '
        Me.lblTo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTo.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTo.Location = New System.Drawing.Point(449, 6)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(37, 20)
        Me.lblTo.TabIndex = 7
        Me.lblTo.Text = "まで"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFromDate
        '
        Me.lblFromDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFromDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFromDate.Location = New System.Drawing.Point(4, 6)
        Me.lblFromDate.Name = "lblFromDate"
        Me.lblFromDate.Size = New System.Drawing.Size(50, 20)
        Me.lblFromDate.TabIndex = 0
        Me.lblFromDate.Text = "開始日"
        Me.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFrom
        '
        Me.lblFrom.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblFrom.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFrom.Location = New System.Drawing.Point(202, 6)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(37, 20)
        Me.lblFrom.TabIndex = 3
        Me.lblFrom.Text = "から"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblToDate
        '
        Me.lblToDate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblToDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToDate.Location = New System.Drawing.Point(256, 6)
        Me.lblToDate.Name = "lblToDate"
        Me.lblToDate.Size = New System.Drawing.Size(50, 20)
        Me.lblToDate.TabIndex = 4
        Me.lblToDate.Text = "終了日"
        Me.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dtpYmdFrom
        '
        Me.dtpYmdFrom.Location = New System.Drawing.Point(57, 6)
        Me.dtpYmdFrom.Name = "dtpYmdFrom"
        Me.dtpYmdFrom.Size = New System.Drawing.Size(140, 20)
        Me.dtpYmdFrom.TabIndex = 4
        '
        'dtpYmdTo
        '
        Me.dtpYmdTo.Location = New System.Drawing.Point(309, 6)
        Me.dtpYmdTo.Name = "dtpYmdTo"
        Me.dtpYmdTo.Size = New System.Drawing.Size(135, 20)
        Me.dtpYmdTo.TabIndex = 6
        '
        'pnlMado
        '
        Me.pnlMado.Controls.Add(Me.cmbMado)
        Me.pnlMado.Controls.Add(Me.lblMado)
        Me.pnlMado.Location = New System.Drawing.Point(342, 36)
        Me.pnlMado.Name = "pnlMado"
        Me.pnlMado.Size = New System.Drawing.Size(237, 33)
        Me.pnlMado.TabIndex = 2
        '
        'cmbMado
        '
        Me.cmbMado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMado.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMado.ItemHeight = 13
        Me.cmbMado.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbMado.Location = New System.Drawing.Point(67, 6)
        Me.cmbMado.Name = "cmbMado"
        Me.cmbMado.Size = New System.Drawing.Size(162, 21)
        Me.cmbMado.TabIndex = 2
        '
        'lblMado
        '
        Me.lblMado.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMado.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMado.Location = New System.Drawing.Point(3, 6)
        Me.lblMado.Name = "lblMado"
        Me.lblMado.Size = New System.Drawing.Size(64, 21)
        Me.lblMado.TabIndex = 0
        Me.lblMado.Text = "コーナー"
        Me.lblMado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlEki
        '
        Me.pnlEki.Controls.Add(Me.cmbEki)
        Me.pnlEki.Controls.Add(Me.lblEki)
        Me.pnlEki.Location = New System.Drawing.Point(122, 36)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(216, 33)
        Me.pnlEki.TabIndex = 1
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.DropDownWidth = 162
        Me.cmbEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbEki.Location = New System.Drawing.Point(45, 6)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(162, 21)
        Me.cmbEki.TabIndex = 1
        '
        'lblEki
        '
        Me.lblEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblEki.Location = New System.Drawing.Point(4, 6)
        Me.lblEki.Name = "lblEki"
        Me.lblEki.Size = New System.Drawing.Size(39, 21)
        Me.lblEki.TabIndex = 0
        Me.lblEki.Text = "駅名"
        Me.lblEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmMntDispTrafficData
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispTrafficData"
        Me.Text = "運用端末 Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlFromTo.ResumeLayout(False)
        Me.pnlMado.ResumeLayout(False)
        Me.pnlEki.ResumeLayout(False)
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
    ''' 出力用テンプレートファイル名
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "時間帯別乗降データ.xls"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "時間帯別乗降データ"

    '適用開始日
    Private sApplyDate As String = Now.ToString("yyyyMMdd")     'デフォルトをシステム日付
    '適用開始日
    Public Property ApplyDate() As String
        Get
            Return sApplyDate
        End Get
        Set(ByVal Value As String)
            sApplyDate = Value
        End Set
    End Property

    '検索SQL取得区分
    Private Enum SlcSQLType
        SlcCount = 0  '件数取得用
        SlcDetail = 1 'データ検索用
    End Enum

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly FormTitle As String = "時間帯別乗降データ出力"
    ''' <summary>
    ''' 駅コードの先頭3桁:「000」
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"


    ''' <summary>
    ''' 検索条件によって、検索ボタン活性化
    ''' </summary>
    Private LcstSearchCol() As Control
#End Region

#Region "メソッド（Public）"

    ''' <summary>
    ''' 画面初期処理
    ''' エラー発生時は内部でメッセージを表示します。
    ''' </summary>
    ''' <returns>True:成功,False:失敗</returns>
    Public Function InitFrm() As Boolean

        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Try
            Log.Info("Method started.")

            'ログ出力
            lblTitle.Text = FormTitle

            '終了ボタン活性化項目設定
            btnReturn.Enabled = True

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

            '駅コンボ設定
            BaseCtlEnabled(pnlEki)

            If LfSetEki() = False Then Exit Try '駅名コンボボックス設定
            cmbEki.SelectedIndex = 0            'デフォルト表示項目
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try 'コーナーコンボボックス設定
            cmbMado.SelectedIndex = 0           'デフォルト表示項目

            'イベント発生ＯＮ
            bRtn = True

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If
            LbEventStop = False                 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

#End Region

#Region "イベント"

    ''' <summary>
    ''' フォームロード
    ''' </summary>
    Private Sub FrmMntDispTrafficData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrm() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            LbEventStop = True      'イベント発生ＯＦＦ
            LfSetDateFromTo()       'Loadされないと開始時間の00:00が設定されない為、ここで設定しています。
            LbEventStop = False     'イベント発生ＯＮ

            cmbEki.Select()     '初期フォーカス

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 終了
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnReturn.Click
        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()
    End Sub

    ''' <summary>
    ''' 帳票出力
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click

        If LbEventStop Then Exit Sub
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""

        Try
            LfWaitCursor()

            'データ取得
            LbEventStop = True
            LogOperation(sender, e)    'ボタン押下ログ
            '----Ver0.1 MOD START---------------------------
            'クリア
            sSql = ""
            dt = New DataTable
            'データ取得処理           
            sSql = LfGetSelectString(SlcSQLType.SlcDetail)
            nRtn = BaseSqlDataTableFill(sSql, dt)
            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case 0              '該当なし
                    AlertBox.Show(Lexis.NoRecordsFound)
                    cmbEki.Select()
                    Exit Sub
                Case Else
                    'メッセージ表示
                    If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.ReallyPrinting) = DialogResult.Cancel Then
                        cmbEki.Select()
                        Exit Sub
                    End If
            End Select
            '----Ver0.1 MOD START---------------------------
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

            '取得データを帳票に設定
            LfXlsStart(sPath, dt)
            '駅名コンボボックスにフォーカスセット
            cmbEki.Select()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'エラーメッセージ
            AlertBox.Show(Lexis.PrintingErrorOccurred)
            btnReturn.Select()
        Finally
            'DB開放()
            dt = Nothing
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    '''<summary>
    ''' 「駅」コンボ
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try

            'コーナーコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then
                If cmbMado.Enabled = True Then cmbMado.Enabled = False
                If btnPrint.Enabled = True Then btnPrint.Enabled = False
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            Else
                If cmbMado.Enabled = False Then
                    cmbMado.Enabled = True
                Else
                    '出力ボタン有効化
                    If btnPrint.Enabled = False Then
                        '検索ボタン活性化
                        Call LfSearchButton()
                    End If
                End If

            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbMado.SelectedIndex = 0               '★イベント発生箇所
        Catch ex As Exception
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    '''<summary>
    ''' 「コーナー」コンボ
    ''' </summary>
    Private Sub cmbMado_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbMado.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfPrintTrue()
        Catch ex As Exception
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 開始日時（年月日）,開始日時（時分）,終了日時（年月日）,終了日時（時分）
    ''' </summary>
    Private Sub dtpYmdFrom_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles dtpYmdFrom.ValueChanged, dtpYmdTo.ValueChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfPrintTrue()
            
        Catch ex As Exception
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

#End Region

#Region "メソッド（Private）"

    ''' <summary>
    ''' [帳票出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String, ByVal dt As DataTable)
        '------Ver0.1　ADD　START-------------
        Dim sSheet As String = ""
        Dim SheetStation As String = ""  '各シートの駅名
        Dim SheetCoener As String = ""  '各シートのコーナー名
        Dim TicketKind(15, 4) As String     '各券種の合計配列
        Dim TicketClear As Boolean = False   '券種初期化判定フラグ
        Dim TicketCnt As Integer = 0     '券種数
        Dim PrCnt As Integer = 0     '出力行
        Dim MaxRow As Integer = 18000    '罫線最大行
        '------Ver0.1　ADD　END---------------
        'excel中で、始行号
        Dim nStartRow As Integer = 6

        '列号
        Dim nY As Integer = 0

        '入場
        Dim nInStatic As Long = 0
        '出場
        Dim nOutStatic As Long = 0

        '駅コード
        Dim sStationCodeOld As String = ""
        Dim sStationCode As String = ""
        'true:上駅名と同じ；false:同じではない
        Dim isSameStation As Boolean = False
        'コーナー
        Dim sCornerCodOld As String = ""
        Dim sCornerCode As String = ""

        'true:同じ；false:同じではない
        Dim isCorner As Boolean = False
        '日付
        Dim dOldDate As DateTime = Nothing
        Dim dCurDate As DateTime = Nothing

        'true:同じ；false:同じではない
        Dim isDate As Boolean = False

        '時間帯の始め
        Dim dZonFrom As DateTime = DateTime.Now
        Dim dZonTo As DateTime = DateTime.Now

        Try
            With XlsReport1

                Log.Info("Start printing about [" & sPath & "].")
                ' 帳票ファイル名称を取得
                .FileName = sPath
                .ExcelMode = True
                ' 帳票の出力処理を開始を宣言
                .Report.Start()
                .Report.File()

                '------Ver0.1　MOD　START-------------
                sSheet = ""
                SheetStation = ""
                SheetCoener = ""
                For Rec As Integer = 0 To dt.Rows.Count - 1
                    '抽出データに線区コード、駅順コードがあれば以下の処理
                    If dt.Rows(Rec)(8).ToString() & dt.Rows(Rec)(9).ToString() <> "" Then
                        'キーブレーク：線区コード、駅順コード、コーナーコードが変われば改ページ
                        If sSheet <> dt.Rows(Rec)(8).ToString() & dt.Rows(Rec)(9).ToString() Then
                            If sSheet <> "" Then
                                '最後行、各券種の合計を出力する
                                PrCnt = PrCnt + +nStartRow
                                .Pos(2, PrCnt).Attr.HorizontalAlignment = AdvanceSoftware.VBReport7.HorizontalAlignment.Right
                                .Pos(2, PrCnt).Value = "合計"
                                For RecCnt As Integer = 0 To TicketKind.GetLength(0) - 2
                                    If TicketKind(RecCnt, 0).ToString <> "" And TicketKind(RecCnt, 0).ToString <> "0" Then
                                        .Pos(5, PrCnt).Value = TicketKind(RecCnt, 0).ToString
                                        .Pos(6, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 1).ToString)
                                        .Pos(7, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 2).ToString)
                                        .Pos(8, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 3).ToString)
                                        PrCnt = PrCnt + 1
                                    End If
                                Next
                                ''必要ない罫線枠を削除
                                For DelRs As Integer = PrCnt To MaxRow
                                    .RowClear(DelRs)
                                Next
                                PrCnt = 0
                                TicketCnt = 0
                                TicketClear = False
                                .Page.End()
                            End If
                            '合計配列を初期化する
                            For Cola As Integer = 0 To 14
                                For Rowe As Integer = 0 To 3
                                    TicketKind(Cola, Rowe) = "0"
                                Next
                            Next
                            'シート名設定：駅＋コーナー
                            sSheet = dt.Rows(Rec)(8).ToString() & dt.Rows(Rec)(9).ToString()
                            'シートの駅、コーナーを取得
                            SheetCoener = dt.Rows(Rec)(9).ToString()
                            SheetStation = dt.Rows(Rec)(8).ToString()
                            '帳票ファイルシート名称を取得します。
                            .Page.Start(LcstXlsSheetName, "1-9999")
                            .Page.Name = dt.Rows(Rec)(0).ToString() & "　" & dt.Rows(Rec)(1).ToString()

                            ' 見出し部セルへ見出しデータ出力
                            'タイトル
                            .Cell("B1").Value = LcstXlsSheetName

                            '出力端末:“運用管理端末” + Config.MachineName
                            .Cell("I1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()

                            '出力日時
                            .Cell("I2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")

                            '駅名、コーナー
                            .Cell("B3").Value = OPMGFormConstants.STATION_NAME + dt.Rows(Rec)(0).ToString + "　　　" +
                                OPMGFormConstants.CORNER_STR + dt.Rows(Rec)(1).ToString

                            'から日時、まで日時
                            .Cell("C4").Value = Lexis.TimeSpan.Gen(Replace(Replace(Replace(dtpYmdFrom.Text, "年", "/"), "月", "/"), "日", ""), "", _
                                                                Replace(Replace(Replace(dtpYmdTo.Text, "年", "/"), "月", "/"), "日", ""), "")
                        End If

                        '駅名
                        nY = 0
                        '駅コード
                        sStationCode = dt.Rows(Rec)("STATION_CODE").ToString()
                        If PrCnt = 0 Then
                            sStationCodeOld = sStationCode
                            .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("STATION_NAME").ToString()
                            isSameStation = False
                        Else
                            '当駅が前駅と同じ
                            If sStationCodeOld.Equals(sStationCode) Then
                                .Pos(nY + 1, PrCnt + nStartRow).Value = ""
                                isSameStation = True
                            End If
                        End If

                        'コーナー
                        nY = nY + 1
                        sCornerCode = dt.Rows(Rec)("CORNER_CODE").ToString()
                        If isSameStation = False Then
                            sCornerCodOld = sCornerCode
                            isCorner = False
                            .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("CORNER_NAME").ToString()
                        Else
                            If sCornerCodOld.Equals(sCornerCode) Then
                                isCorner = True
                                .Pos(nY + 1, PrCnt + nStartRow).Value = ""
                            End If
                        End If

                        '日付
                        nY = nY + 1
                        dCurDate = CDate(dt.Rows(Rec)("DATE"))
                        If isCorner = True Then
                            If dOldDate = dCurDate Then
                                .Pos(nY + 1, PrCnt + nStartRow).Value = ""
                                isDate = True
                            Else
                                dOldDate = dCurDate
                                .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("DATE")
                                isDate = False
                            End If
                        Else
                            dOldDate = dCurDate
                            .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("DATE")
                            isDate = False
                        End If

                        '時間帯
                        nY = nY + 1
                        If isDate = False Then

                            '時間帯from,時間帯to
                            getTimeZone(dt.Rows(Rec)("TIME_ZONE").ToString(), dCurDate, dZonFrom, dZonTo)
                            .Pos(nY + 1, PrCnt + nStartRow).Value = Format(dZonFrom.Hour, "00") & ":" & Format(dZonFrom.Minute, "00")
                            '券種クリア判定を有効
                            TicketClear = True
                        Else
                            '全部データが上行と同じ
                            If DateTime.Parse(dCurDate & " " & dt.Rows(Rec)(nY).ToString()) >= dZonFrom AndAlso
                                DateTime.Parse(dCurDate & " " & dt.Rows(Rec)(nY).ToString()) <= dZonTo Then
                                .Pos(nY + 1, PrCnt + nStartRow).Value = ""
                                '券種クリア判定を無効
                                TicketClear = False
                            Else
                                '時間帯from,時間帯to
                                getTimeZone(dt.Rows(Rec)("TIME_ZONE").ToString(), dCurDate, dZonFrom, dZonTo)
                                .Pos(nY + 1, PrCnt + nStartRow).Value = Format(dZonFrom.Hour, "00") & ":" & Format(dZonFrom.Minute, "00")
                                '券種クリア判定を有効
                                TicketClear = True
                            End If
                        End If

                        '券種
                        nY = nY + 1
                        .Pos(nY + 1, PrCnt + nStartRow).Value = dt.Rows(Rec)("TICKET_NAME").ToString()
                        '入場
                        nY = nY + 1
                        nInStatic = Long.Parse(dt.Rows(Rec)("STATION_IN").ToString)
                        .Pos(nY + 1, PrCnt + nStartRow).Value = nInStatic

                        '出場
                        nY = nY + 1
                        nOutStatic = Long.Parse(dt.Rows(Rec)("STATION_OUT").ToString)
                        .Pos(nY + 1, PrCnt + nStartRow).Value = nOutStatic

                        '合計  
                        nY = nY + 1
                        .Pos(nY + 1, PrCnt + nStartRow).Value = Long.Parse(dt.Rows(Rec)("STATION_SUM").ToString)
                        '時間帯が変更される時、券種もクリアする
                        If TicketClear = True Then
                            TicketCnt = 0
                        Else
                            TicketCnt = TicketCnt + 1
                        End If
                        '券種名称を設定
                        TicketKind(TicketCnt, 0) = dt.Rows(Rec)("TICKET_NAME").ToString
                        '券種ごとの入場券をカウント
                        TicketKind(TicketCnt, 1) = (Long.Parse(TicketKind(TicketCnt, 1)) + Long.Parse(dt.Rows(Rec)("STATION_IN").ToString)).ToString
                        '券種ごとの出場券をカウント
                        TicketKind(TicketCnt, 2) = (Long.Parse(TicketKind(TicketCnt, 2)) + Long.Parse(dt.Rows(Rec)("STATION_OUT").ToString)).ToString
                        '券種ごとの合計券をカウント
                        TicketKind(TicketCnt, 3) = (Long.Parse(TicketKind(TicketCnt, 3)) + Long.Parse(dt.Rows(Rec)("STATION_SUM").ToString)).ToString
                        PrCnt = PrCnt + 1
                    End If
                    If Rec = dt.Rows.Count - 1 Then
                        '最後行、各券種の合計を出力する
                        PrCnt = PrCnt + +nStartRow
                        .Pos(2, PrCnt).Attr.HorizontalAlignment = AdvanceSoftware.VBReport7.HorizontalAlignment.Right
                        .Pos(2, PrCnt).Value = "合計"
                        For RecCnt As Integer = 0 To TicketKind.GetLength(0) - 2
                            If TicketKind(RecCnt, 0).ToString <> "" And TicketKind(RecCnt, 0).ToString <> "0" Then
                                .Pos(5, PrCnt).Value = TicketKind(RecCnt, 0).ToString
                                .Pos(6, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 1).ToString)
                                .Pos(7, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 2).ToString)
                                .Pos(8, PrCnt).Value = Long.Parse(TicketKind(RecCnt, 3).ToString)
                                PrCnt = PrCnt + 1
                            End If
                        Next
                        ''必要ない罫線枠を削除
                        For DelRx As Integer = PrCnt To MaxRow
                            .RowClear(DelRx)
                        Next
                        PrCnt = 0
                        TicketCnt = 0
                        TicketClear = False
                    End If
                Next
                '------Ver0.1　MOD　END---------------

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
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

    '''<summary>
    ''' 時間帯取得
    ''' </summary>
    Private Sub getTimeZone(ByVal timezon As String, ByVal dateHourly As Date, ByRef datetimzonfrom As DateTime, ByRef datetimzonto As DateTime)

        Dim strHour As String = ""

        If timezon.IndexOf(":") > 0 Then
            '時間帯中　時の取得
            strHour = timezon.Substring(0, timezon.IndexOf(":"))
        Else
            '時間帯中　時の取得
            strHour = "00"
        End If

        Dim datSmal As DateTime = DateTime.Parse(dateHourly & " " & strHour & ":00")
        Dim datBig As DateTime = DateTime.Parse(dateHourly & " " & strHour & ":30")

        If DateTime.Parse(dateHourly & " " & timezon) >= datSmal AndAlso
            DateTime.Parse(dateHourly & " " & timezon) < datBig Then
            datetimzonfrom = datSmal
            datetimzonto = DateTime.Parse(dateHourly & " " & strHour & ":29")
        Else
            datetimzonfrom = datBig
            datetimzonto = DateTime.Parse(dateHourly & " " & strHour & ":59")
        End If

    End Sub

    ''' <summary>
    ''' [開始終了日時設定]
    ''' </summary>
    Private Sub LfSetDateFromTo()
        Dim dtWork As DateTime = DateAdd(DateInterval.Day, -1, Today)
        Dim dtFrom As New DateTime(dtWork.Year, dtWork.Month, dtWork.Day, 0, 0, 0)
        Dim dtTo As DateTime = Now
        dtpYmdFrom.Format = DateTimePickerFormat.Custom
        dtpYmdFrom.CustomFormat = "yyyy年MM月dd日"
        dtpYmdFrom.Value = dtFrom
       
        dtpYmdTo.Format = DateTimePickerFormat.Custom
        dtpYmdTo.CustomFormat = "yyyy年MM月dd日"
        dtpYmdTo.Value = dtTo
        
    End Sub

    ''' <summary>
    ''' [出力ボタン活性化]
    ''' </summary>
    Private Sub LfPrintTrue()
        Dim bEnabled As Boolean
        Dim sFrom As String = String.Format("{0} {1}", dtpYmdFrom.Text, "00:00")
        Dim sTo As String = String.Format("{0} {1}", dtpYmdTo.Text, "23:59")
        If sFrom > sTo Then
            bEnabled = False
        Else
            bEnabled = True
        End If
        If bEnabled Then
            If ((cmbEki.SelectedIndex < 0) OrElse _
                (cmbMado.SelectedIndex < 0)) Then
                bEnabled = False
            End If
        End If
        If bEnabled Then

            If btnPrint.Enabled = False Then
                '検索ボタン活性化
                Call LfSearchButton()
            End If

        Else
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' [駅コンボ設定]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetEki() As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As StationMaster
        oMst = New StationMaster
        Try
            oMst.ApplyDate = ApplyDate
            dt = oMst.SelectTable(True, "G")
            dt = oMst.SetAll()
            bRtn = BaseSetMstDtToCmb(dt, cmbEki)
            cmbEki.SelectedIndex = -1
            If cmbEki.Items.Count <= 0 Then bRtn = False
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
    ''' [コーナーコンボ設定]
    ''' </summary>
    ''' <param name="Station">駅コード</param>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetMado(ByVal Station As String) As Boolean
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As CornerMaster
        oMst = New CornerMaster
        Try
            oMst.ApplyDate = ApplyDate
            If String.IsNullOrEmpty(Station) Then
                Station = ""
            End If
            If Station <> "" AndAlso Station <> ClientDaoConstants.TERMINAL_ALL Then
                dt = oMst.SelectTable(Station, "G")
            End If
            dt = oMst.SetAll()
            bRtn = BaseSetMstDtToCmb(dt, cmbMado)
            cmbMado.SelectedIndex = -1
            If cmbMado.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
            LbEventStop = False 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [検索用SELECT文字列取得]
    ''' </summary>
    ''' <returns>SELECT文</returns>
    Private Function LfGetSelectString(ByVal slcSQLType As SlcSQLType) As String

        Dim sSql As StringBuilder = New StringBuilder()
        Try
            Dim sSqlWhere As StringBuilder = New StringBuilder()
            Dim sFrom As String = ""
            Dim sTo As String = ""
            Dim sTabName As String = "V_TRAFFIC_DATA"
            Dim sEki As String

            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '件数取得項目--------------------------
                    sSql.AppendLine("SELECT COUNT(1) FROM " + sTabName)
                Case slcSQLType.SlcDetail
                    '取得項目--------------------------
                    sSql.AppendLine("SELECT STATION_NAME,CORNER_NAME,")
                    '----Ver0.1 MOD START----------------------------
                    sSql.AppendLine("DATE,TIME_ZONE,TICKET_NAME,STATION_IN,STATION_OUT,STATION_SUM,STATION_CODE,CORNER_CODE,TICKET_NO ")
                    '----Ver0.1 MOD END------------------------------
                    sSql.AppendLine(" FROM " + sTabName)
            End Select

            'Where句生成--------------------------
            sSql.AppendLine(" where 0=0 ")
            '駅
            If Not (cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sEki = cmbEki.SelectedValue.ToString
                If sEki.Substring(0, 3).Equals(LcstEkiSentou) Then
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE in {0})", _
                                                       String.Format("(SELECT DISTINCT(RAIL_SECTION_CODE + STATION_ORDER_CODE) AS STATION_CODE " & _
                                                                     " FROM M_MACHINE WHERE BRANCH_OFFICE_CODE = {0}) ", _
                                                                     Utility.SetSglQuot(sEki.Substring(sEki.Length - 3, 3)))))
                Else
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE = {0})", Utility.SetSglQuot(cmbEki.SelectedValue.ToString)))
                End If
            End If
            'コーナー
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then

                sSqlWhere.AppendLine(String.Format(" and (CORNER_CODE = {0})", Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If

            '開始終了日時
            sFrom = (Replace(Replace(Replace(dtpYmdFrom.Text, "年", ""), "月", ""), "日", ""))

            sTo = (Replace(Replace(Replace(dtpYmdTo.Text, "年", ""), "月", ""), "日", ""))

            sSqlWhere.AppendLine(" And")
            sSqlWhere.AppendLine("( (SUBSTRING([DATE],1,4)+SUBSTRING([DATE],6,2)+SUBSTRING([DATE],9,2)) >= ")
            sSqlWhere.AppendLine("'" + sFrom.ToString + "'")
            sSqlWhere.AppendLine(" and (SUBSTRING([DATE],1,4)+SUBSTRING([DATE],6,2)+SUBSTRING([DATE],9,2)) <= ")
            sSqlWhere.AppendLine("'" + sTo.ToString + "'")
            sSqlWhere.AppendLine(")")

            Select Case slcSQLType
                Case slcSQLType.SlcDetail
                    '取得項目--------------------------
                    sSqlWhere.AppendLine(" order by STATION_CODE,CORNER_CODE,[DATE],TIME_ZONE,TICKET_NO")
            End Select

            'Where句結合
            sSql.AppendLine(sSqlWhere.ToString)

            Return sSql.ToString
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Function

    ''' <summary>
    ''' 検索ボタン活性化
    ''' </summary>
    Private Sub LfSearchButton()
        Dim bEnabled As Boolean = True
        For Each control As Control In LcstSearchCol
            If control.Enabled = False Then
                bEnabled = False
                Exit For
            End If
        Next
        If bEnabled Then
            btnPrint.Enabled = True
        Else
            btnPrint.Enabled = False
        End If
    End Sub

#End Region

End Class