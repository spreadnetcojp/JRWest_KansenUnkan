' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2013/11/11  (NES)金沢  フェーズ２権限対応
'   　　　　　　　　　　　　　　　　インポート＆エクスポート機能追加
'   0.2      2014/01/01       金沢  インポート時の”＃”チェック追加
' **********************************************************************
Option Explicit On
Option Strict On
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '定数値のみ使用
'-------Ver0.1　フェーズ２権限対応　ADD START-----------
Imports JR.ExOpmg.ClientApp.FMTStructure
'-------Ver0.1　フェーズ２権限対応　ADD END-------------
Imports System.IO
Imports System.Text
Imports GrapeCity.Win
Imports AdvanceSoftware.VBReport7.Xls
''' <summary>ＩＤマスタ設定</summary>
''' <remarks>最終登録日時と全ユーザ情報を表示する。</remarks>
Public Class FrmSysIDMst
    Inherits FrmBase

#Region "権限情報設定"
    '-------Ver0.1　フェーズ２権限対応　ADD START-----------
    '権限区分
    Protected Const PREMI_SYS As String = "1"
    Protected Const PREMI_ADMIN As String = "2"
    Protected Const PREMI_USUAL As String = "3"
    Protected Const PREMI_SYOSET As String = "4"
    '権限操作区分
    Protected Const PREMIT_ON As String = "1"
    Protected Const PREMIT_OFF As String = "0"

    '異常事由コード
    Protected Const ERRCODE1 As String = "　IDコードエラー"
    Protected Const ERRCODE2 As String = "　パスワードエラー"
    Protected Const ERRCODE3 As String = "　権限エラー"
    Protected Const ERRCODE4 As String = "　入力値エラー"
    Protected Const ERRCODE5 As String = "　システム管理者権限エラー"
    Protected Const ERRFst As String = "　　　　　　　　　　　　　　　"

    'エラーメッセージ
    Protected Const MSGCODE1 As String = "　　インポート成功　"
    Protected Const MSGCODE2 As String = "　エクスポート成功　"
    Protected Const MSGCODE3 As String = "　　インポート失敗　"
    Protected Const MSGCODE4 As String = "　エクスポート失敗　"
    Protected Const MSGCODEFst As String = "異常詳細"
    Protected Const MSGVer As String = "　Ver."
    Protected Const MSGVer1 As String = "　　　　　"
    '-------Ver0.1　フェーズ２権限対応　ADD END-------------

#End Region
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
    Friend WithEvents istIDMst As System.Windows.Forms.ImageList
    Friend WithEvents wbkIDMst As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents lblTitleDate As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents btnAddNew As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents shtIDMst As GrapeCity.Win.ElTabelleSheet.Sheet
    '-------Ver0.1　フェーズ２権限対応　ADD START-----------
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnExport As System.Windows.Forms.Button
    Friend WithEvents btnImport As System.Windows.Forms.Button
    '-------Ver0.1　フェーズ２権限対応　ADD END-------------
    Friend WithEvents btnReturn As System.Windows.Forms.Button

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysIDMst))
        Me.wbkIDMst = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtIDMst = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.lblTitleDate = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.btnAddNew = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.istIDMst = New System.Windows.Forms.ImageList(Me.components)
        '-------Ver0.1　フェーズ２権限対応　ADD START-----------
        Me.btnImport = New System.Windows.Forms.Button()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        '-------Ver0.1　フェーズ２権限対応　ADD END-------------
        Me.pnlBodyBase.SuspendLayout()
        Me.wbkIDMst.SuspendLayout()
        CType(Me.shtIDMst, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '-------Ver0.1　フェーズ２権限対応　ADD START-----------
        Me.pnlBodyBase.Controls.Add(Me.btnExport)
        Me.pnlBodyBase.Controls.Add(Me.btnImport)
        '-------Ver0.1　フェーズ２権限対応　ADD END-------------
        Me.pnlBodyBase.Controls.Add(Me.wbkIDMst)
        Me.pnlBodyBase.Controls.Add(Me.lblTitleDate)
        Me.pnlBodyBase.Controls.Add(Me.lblDate)
        Me.pnlBodyBase.Controls.Add(Me.btnAddNew)
        Me.pnlBodyBase.Controls.Add(Me.btnUpdate)
        Me.pnlBodyBase.Controls.Add(Me.btnDelete)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/11/01(金)  15:28"
        '
        'wbkIDMst
        '
        Me.wbkIDMst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wbkIDMst.Controls.Add(Me.shtIDMst)
        Me.wbkIDMst.Location = New System.Drawing.Point(124, 84)
        Me.wbkIDMst.Name = "wbkIDMst"
        Me.wbkIDMst.ProcessTabKey = False
        Me.wbkIDMst.ShowTabs = False
        Me.wbkIDMst.Size = New System.Drawing.Size(580, 525)
        Me.wbkIDMst.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wbkIDMst.TabIndex = 5
        '
        'shtIDMst
        '
        Me.shtIDMst.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtIDMst.Data = CType(resources.GetObject("shtIDMst.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtIDMst.Location = New System.Drawing.Point(1, 1)
        Me.shtIDMst.Name = "shtIDMst"
        Me.shtIDMst.Size = New System.Drawing.Size(561, 506)
        Me.shtIDMst.TabIndex = 99
        Me.shtIDMst.TabStop = False
        Me.shtIDMst.TransformEditor = False
        '
        'lblTitleDate
        '
        Me.lblTitleDate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblTitleDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitleDate.Location = New System.Drawing.Point(121, 40)
        Me.lblTitleDate.Name = "lblTitleDate"
        Me.lblTitleDate.Size = New System.Drawing.Size(145, 18)
        Me.lblTitleDate.TabIndex = 5
        Me.lblTitleDate.Text = "◆最終登録日時："
        Me.lblTitleDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDate
        '
        Me.lblDate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(266, 40)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(180, 18)
        Me.lblDate.TabIndex = 6
        Me.lblDate.Text = "2004年07月20日　13:10"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnAddNew
        '
        Me.btnAddNew.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnAddNew.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAddNew.Location = New System.Drawing.Point(873, 320)
        Me.btnAddNew.Name = "btnAddNew"
        Me.btnAddNew.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1　フェーズ２権限対応　MOD START-----------
        Me.btnAddNew.TabIndex = 2
        '-------Ver0.1　フェーズ２権限対応　MOD END-----------
        Me.btnAddNew.Text = "登  録"
        Me.btnAddNew.UseVisualStyleBackColor = False
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(873, 386)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1　フェーズ２権限対応　MOD START-----------
        Me.btnUpdate.TabIndex = 3
        '-------Ver0.1　フェーズ２権限対応　MOD END-----------
        Me.btnUpdate.Text = "修  正"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(873, 452)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1　フェーズ２権限対応　MOD START-----------
        Me.btnDelete.TabIndex = 4
        '-------Ver0.1　フェーズ２権限対応　MOD END-----------
        Me.btnDelete.Text = "削  除"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(873, 518)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1　フェーズ２権限対応　MOD START-----------
        Me.btnPrint.TabIndex = 5
        '-------Ver0.1　フェーズ２権限対応　MOD END-----------
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(873, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        '-------Ver0.1　フェーズ２権限対応　MOD START-----------
        Me.btnReturn.TabIndex = 6
        '-------Ver0.1　フェーズ２権限対応　MOD END-----------
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'istIDMst
        '
        Me.istIDMst.ImageStream = CType(resources.GetObject("istIDMst.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.istIDMst.TransparentColor = System.Drawing.Color.White
        Me.istIDMst.Images.SetKeyName(0, "")
        Me.istIDMst.Images.SetKeyName(1, "")
        '-------Ver0.1　フェーズ２権限対応　ADD START-----------
        '
        'btnImport
        '
        Me.btnImport.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnImport.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.btnImport.Location = New System.Drawing.Point(872, 183)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(128, 40)
        Me.btnImport.TabIndex = 0
        Me.btnImport.Text = "インポート"
        Me.btnImport.UseVisualStyleBackColor = False
        '
        'btnExport
        '
        Me.btnExport.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnExport.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.btnExport.Location = New System.Drawing.Point(872, 251)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(128, 40)
        Me.btnExport.TabIndex = 1
        Me.btnExport.Text = "エクスポート"
        Me.btnExport.UseVisualStyleBackColor = False
        '-------Ver0.1　フェーズ２権限対応　ADD END-----------
        '
        'FrmSysIDMst
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysIDMst"
        Me.Text = " "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wbkIDMst.ResumeLayout(False)
        CType(Me.shtIDMst, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region


#Region "宣言領域（Private）"

    'プロパティに値を代入する変数。
    ''' <summary>
    ''' IDコード
    ''' </summary>
    Private sUserid As String = ""
    ''''-------Ver0.1　フェーズ２権限対応　ADD START-----------
    'ログインのID
    Private sLoginID As String = ""
    'ログインユーザ権限
    Private sAuth As String = ""
    '-------Ver0.1　フェーズ２権限対応　ADD END-----------

    '権限に対応する定数。
    ''' <summary>
    ''' 一般者
    ''' </summary>
    Private Const AUTH_USUAL As String = "一般者"

    ''' <summary>
    ''' 運用管理者
    ''' </summary>
    Private Const AUTH_ADMIN As String = "運用管理者"

    ''' <summary>
    ''' システム管理者
    ''' </summary>
    Private Const AUTH_SYS As String = "システム管理者"

    '-------Ver0.1　フェーズ２権限対応　ADD START-----------
    ''' <summary>
    ''' 詳細設定
    ''' </summary>
    Private Const AUTH_DETTAILSET As String = "詳細設定"
    ''' <summary>
    ''' 定義情報
    ''' </summary>
    ''' <remarks></remarks>
    Private infoObj() As FMTInfo = Nothing

    ''' <summary>
    ''' [ログファイル出力先ディレクトリ指定用環境変数名]
    ''' </summary>
    Private Const REG_LOG As String = "EXOPMG_LOG_DIR"

    ''' <summary>
    ''' CSVデータ
    ''' </summary>
    Private infoLst As New List(Of String())

    ''' <summary>
    ''' ログリスト
    ''' </summary>
    Private LogLst As New ArrayList
    Private MSG As String = ""
    Private Ver00 As String = ""
    Private ErrCount As Integer = 0
    Private SumCount As Integer = 0
    ''' <summary>
    ''' 登録IDマスタ失敗
    ''' </summary>
    Private Const LcstIsMstError As String = "登録処理に失敗しました。設定ファイルの内容を確認してください。"

    ''' <summary>
    ''' ファイル名エラー
    ''' </summary>
    Private Const LcstCSVFileNameError As String = "読込対象ファイルが不正です。"

    ''' <summary>
    ''' ファイルエラー
    ''' </summary>
    Private Const LcstCSVFileCheckError As String = "読込対象ファイルが存在しません。"


    ''' <summary>
    ''' 必須チェック
    ''' </summary>
    Private Const LcstMustCheck As String = "{0}行目のデータ項目「{1}」が必須です。"
    '-------Ver0.1　フェーズ２権限対応　ADD END-------------

    'ﾛｯｸｱｳﾄに対応する定数。
    ''' <summary>
    ''' LOCK_STS = 0
    ''' </summary>
    Private Const LOCK_NOMAL As String = ""

    ''' <summary>
    ''' ロック中(LOCK_STS = 1)
    ''' </summary>
    Private Const LOCKING As String = "ロック中"

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
    Private ReadOnly LcstXlsTemplateName As String = "ＩＤマスタ設定.xls"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "ＩＤマスタ設定"

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "ＩＤマスタ設定"

    ''' <summary>
    ''' 一覧表示最大列
    ''' </summary>
    Private nMaxColCnt As Integer

    ''' <summary>
    ''' データを”*”で出力
    ''' </summary>
    Private Const LcstPwd As String = "'********"

    ''' <summary>
    ''' 一覧ヘッダのソート列割り当て
    ''' （一覧ヘッダクリック時に割り当てる対象列を定義。列番号はゼロ相対で"-1"はソート対象外の列）
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {0, -1, 2, 3}

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した別集札データに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3}
#End Region
#Region "メソッド（Public）"

    ''' <summary>
    ''' ＩＤマスタ設定画面のデータを準備する
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    ''' </summary>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        Dim sSql As String = ""
        Dim nRtn As Integer
        Dim dtMstTable As New DataTable
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Try
            Log.Info("Method started.")

            '--画面タイトル
            lblTitle.Text = LcstFormTitle

            'シート初期化
            shtIDMst.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtIDMst.ViewMode = ElTabelleSheet.ViewMode.Row
            shtIDMst.MaxRows = 0                                                 '行の初期化
            nMaxColCnt = shtIDMst.MaxColumns()                                '列数を取得
            shtIDMst.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード
            'シートのヘッダ選択イベントのハンドラ追加
            shtIDMst.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtIDMst.ColumnHeaders.HeaderClick, AddressOf Me.shtIDMstColumnHeaders_HeadersClick

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

            '一覧ソートの初期化
            LfClrList()

            'Eltableのすべてのデータを取得する。
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dtMstTable)

            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnAddNew.Select()
                Case 0
                    Me.btnUpdate.Enabled = False
                    Me.btnDelete.Enabled = False
                    Me.btnPrint.Enabled = False
                    AlertBox.Show(Lexis.NoIdCodeExists)    'ＩＤマスタ情報が登録されていません。
                    bRtn = True
                    Return False
                Case Else
                    Me.btnUpdate.Enabled = True
                    Me.btnDelete.Enabled = True
                    Me.btnPrint.Enabled = True
                    Me.shtIDMst.Enabled = True
            End Select

            '最終登録日時を取得する。
            If GetDateTable() = -9 AndAlso Not nRtn = -9 Then
                AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            End If

            'Eltableの内容を表示する。
            Call LfSetSheetData(dtMstTable)

            bRtn = True

        Catch ex As Exception
            '画面表示処理に失敗しました。
            Log.Fatal("Unwelcome Exception caught.", ex)
            Me.btnAddNew.Select()
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

#Region "イベント"

    ''' <summary>
    ''' ローディング　メインウィンドウ
    ''' </summary>
    Private Sub FrmSysIDMst_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrmData() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If

            Me.btnAddNew.Focus()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 「登録」ボタンを押下すると、ＩＤデータ登録画面が表示される。
    ''' </summary>
    Private Sub btnAddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddNew.Click
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim nRtn As Integer
        Dim sSql As String = ""
        Try
            LbEventStop = True
            LfWaitCursor()
            '登録ボタン押下。
            LogOperation(sender, e)

            Dim oFrmSysIDMstAdd As New FrmSysIDMstAdd

            oFrmSysIDMstAdd.ShowDialog()

            'TODO: Form.Newを呼び出して以降に例外が発生した場合のことを
            '考えると、FrmMntDispFaultDataDetailのShowDialogを行うときと同様の
            '方針に統一する方がよいかもしれない。（逆にこちらが正解の可能性もある）
            oFrmSysIDMstAdd.Dispose()

            'shtIDMst更新
            Call LfClrList() '一覧ソートの初期化
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnAddNew.Select()
                Case 0
                    Me.btnUpdate.Enabled = False
                    Me.btnDelete.Enabled = False
                    Me.btnPrint.Enabled = False
                Case Else
                    Me.btnUpdate.Enabled = True
                    Me.btnDelete.Enabled = True
                    Me.btnPrint.Enabled = True
                    Me.shtIDMst.Enabled = True
            End Select

            Call LfSetSheetData(dt) '画面表示処理
            shtIDMst.Enabled = True

            '最終登録日時を取得する。
            Select Case GetDateTable()
                Case -9
                    If Not nRtn = -9 Then
                        AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    End If
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'TODO: このようなケースで下記を行うべきか否か、方針を統一しなければならない。
            'モーダルなShowDialogの最中に発生した例外が本当にここに到達するなら、
            '他の箇所も、こうした上で、InitFrmで同様のメッセージボックス表示を
            '行わないようにする方がよいかもしれない。
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
            LbEventStop = False
        End Try
    End Sub

    ''' <summary>
    ''' 「修正」ボタンを押下すると、ＩＤデータ修正画面が表示される。
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer
        Try
            LbEventStop = True
            LfWaitCursor()
            '修正ボタン押下。
            LogOperation(sender, e)

            Dim oFrmSysIDMstUpdate As New FrmSysIDMstUpdate
            'FrmSysIDMstUpdate画面のプロパティに値を代入する。
            Dim nRowno As Integer = shtIDMst.ActivePosition.Row

            sUserid = Me.shtIDMst.Item(0, nRowno).Text

            '登録ユーザのIDを取得する。
            oFrmSysIDMstUpdate.Userid() = sUserid

            If oFrmSysIDMstUpdate.InitFrmData() = False Then
                oFrmSysIDMstUpdate = Nothing
                Call waitCursor(False)
                Exit Sub
            End If

            oFrmSysIDMstUpdate.ShowDialog()
            oFrmSysIDMstUpdate.Dispose()

            'shtIDMst更新
            Call LfClrList() '一覧ソートの初期化
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnUpdate.Select()
                Case 0
                    Me.btnUpdate.Enabled = False
                    Me.btnDelete.Enabled = False
                    Me.btnPrint.Enabled = False
                Case Else
                    Me.btnUpdate.Enabled = True
                    Me.btnDelete.Enabled = True
                    Me.btnPrint.Enabled = True
                    Me.shtIDMst.Enabled = True
            End Select

            Call LfSetSheetData(dt) '画面表示処理
            shtIDMst.Enabled = True

            '最終登録日時を取得する。
            Select Case GetDateTable()
                Case -9
                    If Not nRtn = -9 Then
                        AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    End If
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
            LbEventStop = False
        End Try
    End Sub

    ''' <summary>
    ''' 「削除」ボタンを押下すると、ＩＤデータ削除画面が表示される。
    ''' </summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer
        Try
            LbEventStop = True
            LfWaitCursor()
            '削除ボタン押下。
            LogOperation(sender, e)
            Dim oFrmSysIDMstDelete As New FrmSysIDMstDelete
            'FrmSysIDMstDelete画面のプロパティに値を代入する。
            Dim sRowno As Integer = shtIDMst.ActivePosition.Row

            sUserid = Me.shtIDMst.Item(0, sRowno).Text

            oFrmSysIDMstDelete.Userid() = sUserid

            If oFrmSysIDMstDelete.InitFrmData() = False Then
                oFrmSysIDMstDelete = Nothing
                Call waitCursor(False)
                Exit Sub
            End If

            oFrmSysIDMstDelete.ShowDialog()
            oFrmSysIDMstDelete.Dispose()

            'shtIDMst更新
            Call LfClrList() '一覧ソートの初期化
            sSql = LfGetSelectString()
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnDelete.Select()
                Case 0
                    Me.btnUpdate.Enabled = False
                    Me.btnDelete.Enabled = False
                    Me.btnPrint.Enabled = False
                Case Else
                    Me.btnUpdate.Enabled = True
                    Me.btnDelete.Enabled = True
                    Me.btnPrint.Enabled = True
                    Me.shtIDMst.Enabled = True
            End Select

            Call LfSetSheetData(dt) '画面表示処理
            shtIDMst.Enabled = True

            '最終登録日時を取得する。
            Select Case GetDateTable()
                Case -9
                    If Not nRtn = -9 Then
                        AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    End If
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' 「出力」ボタンを押下すると
    ''' </summary>
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
            btnAddNew.Select()
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
    ''' 「終了」ボタンを押下すると、本画面が終了される。
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '終了ボタン押下。
        LogOperation(sender, e)
        Me.Close()
    End Sub

    ''' <summary>
    ''' ELTableのクリック事件
    ''' </summary>
    Private Sub shtIDMstColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)
        Static intCurrentSortColumn As Integer = -1
        Static bolColumn1SortOrder(63) As Boolean

        If LcstSortCol(e.Column) = -1 Then Exit Sub

        Try

            shtIDMst.BeginUpdate()

            '前回選択された列ヘッダの初期化
            If intCurrentSortColumn > -1 Then
                '列ヘッダのイメージを削除する
                shtIDMst.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '列の背景色を初期化する
                shtIDMst.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '列のセル罫線を消去する
                shtIDMst.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
            End If

            '選択された列番号を保存
            intCurrentSortColumn = e.Column

            'ソートする列の背景色を設定する
            shtIDMst.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
            'ソートする列のセル罫線を設定する
            shtIDMst.Columns(intCurrentSortColumn).SetBorder( _
                New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                GrapeCity.Win.ElTabelleSheet.Borders.All)

            If bolColumn1SortOrder(intCurrentSortColumn) = False Then
                '列ヘッダのイメージを設定する
                shtIDMst.ColumnHeaders(intCurrentSortColumn).Image = istIDMst.Images(1)
                '降順でソートする
                Call SheetSort(shtIDMst, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Descending)
                '列のソート状態を保存する
                bolColumn1SortOrder(intCurrentSortColumn) = True
            Else
                '列ヘッダのイメージを設定する
                shtIDMst.ColumnHeaders(intCurrentSortColumn).Image = istIDMst.Images(0)
                '昇順でソートする
                Call SheetSort(shtIDMst, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Ascending)
                '列のソート状態を保存する
                bolColumn1SortOrder(intCurrentSortColumn) = False
            End If

            shtIDMst.EndUpdate()
            '権限が詳細設定の場合、修正及び削除ボタンが非活性
            Dim nRowno As Integer = shtIDMst.ActivePosition.Row
            '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
            sAuth = Me.shtIDMst.Item(2, nRowno).Text
            '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
            Call AuthCheck(sAuth)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' ELTableのマウスの移動事件
    ''' </summary>
    Private Sub shtIDMst_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Try
            Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
            'マウスカーソルが列ヘッダ上にある場合
            If shtIDMst.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
                shtIDMst.CrossCursor = Cursors.Default
            Else
                'マウスカーソルを既定に戻す
                shtIDMst.CrossCursor = Nothing
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' ELTableのソート事件
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

#Region "メソッド（Private）"

    ''' <summary>Eltableの内容を表示する。</summary>
    ''' <param name="dtMstTable">ユーザデータ</param >
    ''' <remarks>
    ''' ＩＤコード,パスワード,権限,ﾛｯｸｱｳﾄを表示する。
    ''' </remarks>
    Private Sub LfSetSheetData(ByVal dtMstTable As DataTable)

        '画面の閃きを防ぐ。
        Me.shtIDMst.Redraw = False
        Me.wbkIDMst.Redraw = False
        Try
            Me.shtIDMst.MaxRows = dtMstTable.Rows.Count     '抽出件数分の行を一覧に作成

            Me.shtIDMst.DataSource = dtMstTable             'データをセット

            shtIDMst.Rows.SetAllRowsHeight(21)              '行高さを揃える

            '権限が詳細設定の場合、修正及び削除うボタンが非活性
            Dim nRowno As Integer = shtIDMst.ActivePosition.Row
            sAuth = Me.shtIDMst.Item(2, nRowno).Text
            '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
            Call AuthCheck(sAuth)
            '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SheetProcAbnormalEnd)             '一覧表示処理に失敗しました。
            btnAddNew.Select()
        Finally
            'Eltableを再表示する。
            Me.shtIDMst.Redraw = True
            Me.wbkIDMst.Redraw = True
        End Try

    End Sub

    ''' <summary>
    ''' [一覧クリア]
    ''' </summary>
    Private Sub LfClrList()
        shtIDMst.Redraw = False
        wbkIDMst.Redraw = False
        Try
            Dim i As Integer
            'ソート情報のクリア
            With shtIDMst
                For i = 0 To nMaxColCnt - 1
                    .ColumnHeaders(i).Image = Nothing
                    .Columns(i).BackColor = Color.Empty
                Next
            End With

            shtIDMst.DataSource = Nothing
            shtIDMst.MaxRows = 0

            If shtIDMst.Enabled = True Then shtIDMst.Enabled = False
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
            If btnDelete.Enabled = True Then btnDelete.Enabled = False
            If btnUpdate.Enabled = True Then btnUpdate.Enabled = False
        Finally
            wbkIDMst.Redraw = True
            shtIDMst.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 5
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
                .Cell("B1").Value = lblTitle.Text
                .Cell("H1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("H2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = Microsoft.VisualBasic.Right(lblTitleDate.Text, 7) + lblDate.Text
                .Cell("B5").Value = "IDコード"
                .Cell("C5").Value = "パスワード"
                .Cell("D5").Value = "権　限"
                .Cell("E5").Value = "ロックアウト"
                ' 配信対象のデータ数を取得します
                nRecCnt = shtIDMst.MaxRows

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        'データを”*”で出力
                        If x = 1 Then
                            .Pos(x + 1, y + nStartRow).Value = LcstPwd
                        Else
                            .Pos(x + 1, y + nStartRow).Value = shtIDMst.Item(LcstPrntCol(x), y).Text
                        End If
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
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

    ''' <summary>
    ''' [検索用SELECT文字列取得]
    ''' </summary>
    ''' <returns>SELECT文</returns>
    Private Function LfGetSelectString() As String

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine(" SELECT USER_ID,PASSWORD, ")
            sBuilder.AppendLine(" CASE  AUTHORITY_LEVEL ")
            sBuilder.AppendLine(String.Format(" WHEN '1' THEN '{0}' ", AUTH_SYS))
            sBuilder.AppendLine(String.Format(" WHEN '2' THEN '{0}'", AUTH_ADMIN))
            sBuilder.AppendLine(String.Format(" WHEN '3' THEN '{0}' ", AUTH_USUAL))
            '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
            sBuilder.AppendLine(String.Format(" WHEN '4' THEN '{0}' ", AUTH_DETTAILSET))
            '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
            sBuilder.AppendLine(" ELSE '' END , ")
            sBuilder.AppendLine("CASE LOCK_STS ")
            sBuilder.AppendLine(String.Format(" WHEN '0' THEN '{0}' ", LOCK_NOMAL))
            sBuilder.AppendLine(String.Format(" WHEN '1' THEN '{0}' ", LOCKING))
            sBuilder.AppendLine(" ELSE '' END  ")
            sBuilder.AppendLine("  FROM M_USER  ")
            sBuilder.AppendLine("ORDER BY USER_ID ")
            sSQL = sBuilder.ToString()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

        Return sSQL

    End Function
    '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
    ''' <summary>
    ''' [CSV出力用SELECT文字列取得]
    ''' </summary>
    ''' <returns>SELECT文</returns>
    Private Function CsvGetSelectString() As String

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine(" SELECT '/' + USER_ID , ")
            sBuilder.AppendLine(" '/' + PASSWORD,AUTHORITY_LEVEL,LOCK_STS, ")
            'マスタ管理メニュー
            sBuilder.AppendLine(" MST_FUNC1,MST_FUNC2,MST_FUNC3,MST_FUNC4,MST_FUNC5, ")
            'プログラム管理メニュー
            sBuilder.AppendLine(" PRG_FUNC1,PRG_FUNC2,PRG_FUNC3,PRG_FUNC4,PRG_FUNC5, ")
            '保守管理メニュー
            sBuilder.AppendLine(" MNT_FUNC1,MNT_FUNC2,MNT_FUNC3,MNT_FUNC4,MNT_FUNC5, ")
            sBuilder.AppendLine(" MNT_FUNC6,MNT_FUNC7,MNT_FUNC8,MNT_FUNC9,MNT_FUNC10, ")
            'システム管理メニュー
            sBuilder.AppendLine(" SYS_FUNC1,SYS_FUNC2,SYS_FUNC3,SYS_FUNC4,SYS_FUNC5 ")
            sBuilder.AppendLine("  FROM M_USER  ")
            sBuilder.AppendLine("ORDER BY USER_ID ")
            sSQL = sBuilder.ToString()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

        Return sSQL

    End Function
    '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
    ''' <summary>
    ''' 最終登録日時を取得する
    ''' </summary>
    ''' <returns>最終登録日時</returns>
    Private Function GetDateTable() As Integer

        '最終登録日時を格納する。
        Dim dtDateTable As New DataTable

        '当関数の戻り値
        Dim sLoginDate As String = ""

        Dim sSQL As String = ""

        Dim nRtn As Integer

        Dim dLastDate As DateTime = Nothing

        sSQL = " SELECT MAX(UPDATE_DATE)  FROM M_USER "

        Try
            nRtn = BaseSqlDataTableFill(sSQL, dtDateTable)
            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    Return nRtn
                Case 0
                    Return nRtn
            End Select

            '最終登録日時を格納する。
            If dtDateTable IsNot Nothing AndAlso Convert.ToString(dtDateTable.Rows(0)(0)).Trim <> "" Then

                dLastDate = DateTime.Parse(dtDateTable.Rows(0).Item(0).ToString())
                sLoginDate = dLastDate.ToString("yyyy/MM/dd(ddd)  HH:mm")

            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            lblDate.Text = sLoginDate
        End Try

        Return nRtn

    End Function
    '-------Ver0.1　フェーズ２権限対応 ADD　START-----------
    ''' <summary>
    ''' [インポート処理]
    ''' </summary>
    Private Sub btnImport_Click(sender As System.Object, e As System.EventArgs) Handles btnImport.Click
        Dim filePath As String = ""
        Dim oldFPath As String
        Dim newFPath As String
        Dim filenumber As Int32
        Dim strRead() As String                                     '設定ファイルの端末ＩＤ
        Dim j As Integer = 0
        Dim Errflg As Boolean = False
        Dim sSql As String = ""
        Dim nRtn As Integer
        Dim dtMstTable As New DataTable
        Dim Time As DateTime = Now

        Try
            Call waitCursor(True)
            '初期化
            ErrCount = 0
            SumCount = 0
            LogLst.Clear()
            infoLst.Clear()
            'ボタン押下’
            LogOperation(sender, e)
            OpenFileDialog1.Multiselect = False
            OpenFileDialog1.FileName = ""
            'ファイルを選択
            If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                '読込対象ファイル名チェック
                oldFPath = OpenFileDialog1.FileName
                filePath = oldFPath.Substring(0, oldFPath.LastIndexOf("\") + 1)
                newFPath = Path.Combine(filePath, Path.GetFileName(oldFPath))
                If (FileCheck(newFPath, oldFPath)) = False Then
                    Exit Sub
                End If
            Else
                Exit Sub
            End If
            'CSVフォーマット定義情報を取得する
            If GetDefineInfo(Config.IdMasterFormatFilePath, "FMT_IDMstConfig", infoObj) = False Then
                btnReturn.Select()
                Exit Sub
            End If
            'CSVファイルより、データを取得する。
            filenumber = CShort(FreeFile())
            '行数カウント
            Dim CLine As Integer = 0
            If System.IO.File.Exists(filePath + Path.GetFileName(OpenFileDialog1.FileName)) Then
                FileOpen(filenumber, filePath + Path.GetFileName(OpenFileDialog1.FileName), OpenMode.Binary, OpenAccess.Read)

                Do While Not EOF(1)
                    CLine += 1
                    strRead = Nothing
                    strRead = Split(LineInput(1), ",")
                    If (strRead(0).Substring(0, 1).ToString <> "#") Then
                        'データチェック
                        If (DataCheck(strRead, CLine)) = False Then
                            Errflg = True
                        End If
                    End If
                    infoLst.Add(strRead)
                Loop
                FileClose(1)
                'データ整合性のチェック
                If (ComCheck()) = False Then
                    Errflg = True
                End If
                If (Errflg = False) Then
                    'DB更新
                    If MuserImport() = False Then
                        '失敗した場合、処理を終了する
                        Exit Sub
                    End If

                    '一覧ソートの初期化
                    LfClrList()
                    sSql = LfGetSelectString()
                    nRtn = BaseSqlDataTableFill(sSql, dtMstTable)
                    Call LfSetSheetData(dtMstTable) '画面表示処理
                    shtIDMst.Enabled = True

                    '最終登録日時を取得する。
                    Select Case GetDateTable()
                        Case -9
                            If Not nRtn = -9 Then
                                AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                            End If
                    End Select
                Else
                    MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                              MSGCODE3 & ErrCount.ToString().PadLeft(4))
                    'ログ出力
                    LogLst.Insert(0, MSG)
                    If (WriteInExportLog(LogLst)) = False Then
                        AlertBox.Show(Lexis.IdMstImportlog)
                        Exit Sub
                    End If
                    AlertBox.Show(Lexis.IdMstImport)
                End If
            Else
                AlertBox.Show(Lexis.IdMstFileNotFound)
            End If

        Catch ex As IOException
            Log.Error("Exception caught.", ex)
            'ファイル読込失敗メッセージ
            AlertBox.Show(Lexis.IdMstFileReadFailed)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                          MSGCODE3 & ErrCount.ToString().PadLeft(4))
            'ログ出力
            LogLst.Insert(0, MSG)
            If (WriteInExportLog(LogLst)) = False Then
                AlertBox.Show(Lexis.IdMstImportlog)
                Exit Sub
            End If
            AlertBox.Show(Lexis.IdMstImport)
        Finally
            FileClose(filenumber)
            infoObj = Nothing
            Call waitCursor(False)
        End Try
    End Sub
    ''' <summary>
    ''' [エクスポート処理]
    ''' </summary>
    Private Sub btnExport_Click(sender As System.Object, e As System.EventArgs) Handles btnExport.Click
        If LbEventStop Then Exit Sub
        Dim ofd As New SaveFileDialog()
        'CSVファイルに書き込むときに使うEncoding
        Dim enc As System.Text.Encoding = _
       System.Text.Encoding.GetEncoding("Shift_JIS")
        Dim sSql As String = ""
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim FileType, Prompt As String
        Dim Filepath As String
        Dim colCount As Integer = dt.Columns.Count
        Dim i As Integer
        Dim ExHdObj As New ArrayList
        Dim Time As DateTime = Now

        Try
            Call waitCursor(True)
            SumCount = 0
            LogLst.Clear()
            'ヘッダー部定義
            ExHdObj.Add("#運用管理システム　詳細データ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,")
            ExHdObj.Add("Ver,/0000,,,,,,,,,,,,,,,,,,,,,,,,,,,,")
            ExHdObj.Add("#,,,,詳細設定,,,,,,,,,,,,,,,,,,,,,,,,,")
            ExHdObj.Add("#,,,,マスタ管理メニュー,,,,,プログラム管理メニュー,,,,,保守管理メニュー,,,,,,,,,,システム管理メニュー,,,,,")
            ExHdObj.Add("#ＩＤコード,パスワード,権限,ロックアウト,外部媒体取込,マスタ適用リスト取込,配信指示設定,配信状況表示,バージョン表示,外部媒体取込,プログラム適用リスト取込,配信指示設定,配信状況表示,バージョン表示,別集札データ確認,不正乗車検出データ確認,強行突破検出データ確認,紛失券検出データ確認,異常データ確認,稼働・保守データ出力,機器接続状態確認,監視盤設定情報,収集データ確認,時間帯別乗降データ出力,IDマスタ設定,稼働・保守データ設定,パターン設定,エリア設定,運管設定管理,コメント欄")

            ofd.FileName = "IDマスタ.csv"
            FileType = "CSV ﾌｧｲﾙ (*.csv),*.csv"
            Prompt = "保存先を選択してください"
            SumCount = 0
            If ofd.ShowDialog() = DialogResult.OK Then
                Filepath = ofd.FileName
                Dim sw As New System.IO.StreamWriter(Filepath, False, enc)
                sSql = CsvGetSelectString()
                nRtn = BaseSqlDataTableFill(sSql, dt)
                'レコードを書き込む
                'ヘッダー情報書き込む
                For i = 0 To ExHdObj.Count - 1
                    sw.Write(ExHdObj(i).ToString)
                    sw.Write(vbCrLf)
                Next
                'データ部書き込む
                Dim row As DataRow
                For Each row In dt.Rows
                    For i = 0 To dt.Columns.Count - 1
                        'フィールドの取得
                        Dim field As String = row(i).ToString()
                        'フィールドを書き込む
                        sw.Write(field)
                        'カンマを書き込む
                        If dt.Columns.Count - 1 >= i Then
                            sw.Write(","c)
                        End If
                    Next
                    SumCount = SumCount + 1
                    '改行する
                    sw.Write(vbCrLf)
                Next

                '閉じる
                sw.Close()
                MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer1 & MSGCODE2 & SumCount.ToString().PadLeft(4))
                'ログ出力正常＆異常
                LogLst.Insert(0, MSG)
                If (WriteInExportLog(LogLst)) = False Then
                    AlertBox.Show(Lexis.IdMstImportlog)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer1 & MSGCODE4 & "　　")
            'ログ出力正常＆異常
            LogLst.Insert(0, MSG)
            If (WriteInExportLog(LogLst)) = False Then
                AlertBox.Show(Lexis.IdMstImportlog)
                Exit Sub
            End If
            Log.Fatal("Unwelcome Exception caught.", ex)
            'エラーメッセージ
            AlertBox.Show(Lexis.IdMstExport)
        Finally
            Call waitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 定義情報の取得
    ''' </summary>
    ''' <param name="fileName">INIファイル名</param>
    ''' <param name="sectionName">セクション名</param>
    ''' <param name="infoObj">取得した結果を保存用</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>INIファイル名にて電文フォーマット定義情報を取得し、一時保持する</remarks>
    Public Shared Function GetDefineInfo(ByVal fileName As String, _
                                         ByVal sectionName As String, _
                                         ByRef infoObj() As FMTStructure.FMTInfo) As Boolean
        Dim bRtn As Boolean = False

        Dim i As Integer = 0
        Dim strDefInfo As String = ""
        Dim strData() As String
        Try
            'CSVフォーマット定義情報チェック

            If File.Exists(fileName) = False Then
                AlertBox.Show(Lexis.IdMstFormatFileNotFound)
                Return bRtn
            End If

            For i = 1 To 9999
                strDefInfo = Constant.GetIni(sectionName, Format(i, "0000"), fileName)
                If strDefInfo <> "" Then
                    strData = strDefInfo.Split(CChar(","))

                    ReDim Preserve infoObj(i - 1)
                    '項目名称：日本語名称を取得。エラーメッセージに使用。
                    infoObj(i - 1).KOMOKU_NAME = strData(0)
                    '順番
                    infoObj(i - 1).IN_TURN = CInt(strData(1))
                    '必須
                    infoObj(i - 1).MUST = CBool(strData(2))

                    'フィールド形式: 登録時の型
                    infoObj(i - 1).FIELD_FORMAT = strData(3)

                    'データ長：登録対象ＤＢ長
                    If strData(4) = "" Then
                        infoObj(i - 1).DATA_LEN = 10
                    Else
                        infoObj(i - 1).DATA_LEN = CInt(strData(4))
                    End If


                    'フィールド名: 登録対象ＤＢフィールド
                    infoObj(i - 1).FIELD_NAME = strData(5)
                Else
                    Exit For
                End If
            Next
            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        End Try

        Return bRtn

    End Function
    ''' <summary>
    ''' 先頭文字チェック
    ''' </summary>
    ''' <param name="CodeName">フィールド名</param>
    Private Function FrastChar(ByRef CodeName As String) As Boolean
        If (CodeName.Substring(0, 1) <> "/") Then      '先頭文字チェック
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' 文字数チェック
    ''' </summary>
    ''' <param name="CodeName">フィールド名</param>
    ''' <param name="iRow">桁数</param>
    ''' <param name="AarrayCode">定義情報</param>
    Private Function ByteCheck(ByRef CodeName As String, ByVal iRow As Integer, ByVal AarrayCode As FMTInfo) As Boolean
        'ユーザID文字数チェック
        If AarrayCode.FIELD_NAME = "USER_ID" Then
            '8桁ではないの場合
            If (CodeName.Length <> AarrayCode.DATA_LEN) Then
                Log.Info(String.Format(LcstMustCheck, iRow, AarrayCode.KOMOKU_NAME))
                Return False
            End If
            'パスワードの文字数チェック
        ElseIf (AarrayCode.FIELD_NAME = "PASSWORD") Then
            If (CodeName.Length < 4) Or (CodeName.Length > AarrayCode.DATA_LEN) Then
                Return False
            End If
        End If
        Return True
    End Function
    ''' <summary>
    ''' インポート（DB更新）
    ''' </summary>
    Private Function MuserImport() As Boolean
        Dim dbCtl As DatabaseTalker = New DatabaseTalker()
        Dim sCurTime As String
        Dim sBuilder As StringBuilder
        Dim vBuilder As StringBuilder
        Dim Time As DateTime = Now
        Dim j As Integer = 0
        Dim Errflg As Boolean = False
        Dim loginiD As String = Config.MachineName
        Dim i As Integer = 0
        Dim dbError As Boolean = False                  'db異常発生ＯＮ
        Try
            'shtIDMst更新
            'Call LfClrList() '一覧ソートの初期化
            dbCtl.ConnectOpen()          'クネクションを取得する。

            dbCtl.TransactionBegin()  'トランザクションを開始する。
            '登録日時の作成
            sCurTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")

            sBuilder = New StringBuilder

            '排他制御
            sBuilder.AppendLine("SELECT * FROM M_USER WITH( TABLOCK , XLOCK ) ")
            dbCtl.ExecuteSQLToWrite(sBuilder.ToString)

            'テーブルクリア
            sBuilder.AppendLine("delete FROM M_USER ")
            dbCtl.ExecuteSQLToWrite(sBuilder.ToString)

            For i = 5 To infoLst.Count - 1
                '-------Ver0.2　"#"チェック対応　ADD START-----------
                If infoLst(i)(0).Substring(0, 1).ToString <> "#" Then
                    '-------Ver0.2　"#"チェック対応　ADD END-----------
                    SumCount = SumCount + 1
                    sBuilder = New StringBuilder
                    vBuilder = New StringBuilder

                    vBuilder.AppendLine("values(")

                    'IDマスタの登録
                    sBuilder.AppendLine(" insert into M_USER (INSERT_DATE ,INSERT_USER_ID,INSERT_MACHINE_ID,UPDATE_DATE,UPDATE_USER_ID,UPDATE_MACHINE_ID")

                    vBuilder.AppendLine(String.Format("{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot(GlobalVariables.UserId)))
                    vBuilder.AppendLine(String.Format("{0}", Utility.SetSglQuot(loginiD)))
                    vBuilder.AppendLine(String.Format(",{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot(GlobalVariables.UserId)))
                    vBuilder.AppendLine(String.Format("{0}", Utility.SetSglQuot(loginiD)))
                    For j = 0 To infoObj.Length - 1

                        vBuilder.AppendLine(String.Format(",{0}", Utility.SetSglQuot(infoLst.Item(i)(j).ToString)))
                        sBuilder.AppendLine(String.Format(",{0}", infoObj(j).FIELD_NAME))
                    Next

                    vBuilder.Append(")")
                    sBuilder.Append(")")
                    sBuilder.AppendLine(vBuilder.ToString)

                    'データ処理
                    dbCtl.ExecuteSQLToWrite(sBuilder.ToString)
                    '-------Ver0.2　"#"チェック対応　ADD START-----------
                End If
                '-------Ver0.2　"#"チェック対応　ADD END-----------
            Next
            'ログリストに処理情報をセット
            If (Errflg = False) Then
                MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                       MSGCODE1 & SumCount.ToString().PadLeft(4))
            Else
                MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                       MSGCODE3 & ErrCount.ToString().PadLeft(4))
                AlertBox.Show(Lexis.IdMstImport)
            End If
            'ログ出力正常＆異常
            LogLst.Insert(0, MSG)
            If (WriteInExportLog(LogLst)) = False Then
                AlertBox.Show(Lexis.IdMstImportlog)
                Return False
            End If
            'トランザクションをコミットする
            dbCtl.TransactionCommit()
            Return True
        Catch ex As Exception
            dbError = True
            infoLst = Nothing
            Log.Fatal(LcstIsMstError)
            MSG = (Time.ToString("yyyy/MM/dd HH:mm:ss") & MSGVer & Ver00.ToString().PadLeft(4) & _
                  MSGCODE3 & ErrCount.ToString().PadLeft(4))
            'ログ出力
            LogLst.Insert(0, MSG)
            If (WriteInExportLog(LogLst)) = False Then
                AlertBox.Show(Lexis.IdMstImportlog)
                Return False
            End If
            AlertBox.Show(Lexis.IdMstInsertFailed)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' ユーザ、パスワードチェック
    ''' </summary>
    ''' <param name="CodeName">フィールド名</param>
    ''' <param name="CLine">桁数</param>
    ''' <param name="AarrayCode">定義情報</param>
    Private Function UsPsCheck(ByRef CodeName As String, ByVal CLine As Integer, ByVal AarrayCode As FMTInfo) As Boolean
        'ユーザIDが空白でない場合
        If (CodeName <> "") Then
            'ユーザID先頭文字チェック
            If FrastChar(CodeName) = True Then
                CodeName = CodeName.Remove(0, 1)
                '文字数チェック
                If (ByteCheck(CodeName, CLine, AarrayCode)) = False Then
                    Return False
                End If
                '英数字チェック
                If (OPMGUtility.checkCharacter(CodeName)) = False Then
                    Return False
                End If
            Else
                Return False
            End If
        Else
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' データの整合性チェック
    ''' </summary>
    Private Function ComCheck() As Boolean
        Dim i As Integer = 0
        Dim a As Integer = 0
        Dim Permiflg As Boolean = False
        Dim Errflg As Boolean = False
        Dim Authflg As Boolean = False
        Dim Pcount As Integer = 0
        Dim Ecount As Integer = 0

        'データ部でループし、ログイン中ユーザIDの存在チェック
        For i = 5 To infoLst.Count - 1
            '-------Ver0.2　"#"チェック対応　ADD START-----------
            If infoLst(i)(0).Substring(0, 1).ToString <> "#" Then
                '-------Ver0.2　"#"チェック対応　ADD END-----------
                '操作中のユーザIDチェック
                If (GlobalVariables.UserId.ToString = infoLst(i)(0).ToString) Then
                    Authflg = True
                    If (infoLst(i)(2).ToString = PREMI_SYS) Then
                        'ログ出力
                        Permiflg = True
                    Else
                        Permiflg = False
                        Pcount = i + 1
                    End If

                End If
                For a = 5 To infoLst.Count - 1
                    If (i <> a) Then
                        'ユーザIDの重複チェック
                        If (infoLst(i)(0).ToString = infoLst(a)(0).ToString) Then
                            Errflg = True
                            Ecount = i + 1
                            'ログ出力
                            Exit For
                        End If
                    End If
                Next
                '-------Ver0.2　"#"チェック対応　ADD START-----------
            End If
            '-------Ver0.2　"#"チェック対応　ADD END-----------
        Next

        'ID重複チェック
        If Errflg = True Then
            'ログ出力
            SetMSGSyousai(ERRFst, Ecount, ERRCODE4)
        End If
        '操作中ユーザが存在しない場合
        If (Authflg = False) Then
            'ログ出力
            SetMSGSyousai(ERRFst, 0, ERRCODE5)
        Else
            '操作中ユーザがシステム管理でない場合
            If (Permiflg = False) Then
                'ログ出力
                SetMSGSyousai(ERRFst, Pcount, ERRCODE5)
            End If
        End If

        If ((Errflg = True) Or (Permiflg = False) Or (Authflg = False)) Then
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' ファイルチェック
    ''' </summary>
    ''' <param name="newFPath">ファイル名</param>
    ''' <param name="oldFPath ">ファイル名</param>
    Private Function FileCheck(ByRef newFPath As String, ByRef oldFPath As String) As Boolean
        If oldFPath <> newFPath Then
            Log.Error(LcstCSVFileNameError)
            AlertBox.Show(Lexis.TheFileNameIsUnsuitableForIdMst)
            btnReturn.Select()
            Return False
        End If
        ' 読込対象ファイルチェック
        If File.Exists(newFPath) = False Then
            Log.Error(LcstCSVFileCheckError)
            AlertBox.Show(Lexis.IdMstFileNotFound)
            btnReturn.Select()
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' データチェック
    ''' </summary>
    ''' <param name="strRead">一レコード分のデータ</param>
    ''' <param name="CLine">桁数</param>
    Private Function DataCheck(ByRef strRead() As String, ByVal CLine As Integer) As Boolean
        Dim j As Integer = 0

        'ヘッダー情報取得
        If (CLine <= 5) Then
            If strRead(0).ToString = "Ver" Then
                'バージョン番号取得
                If (strRead(1).ToString <> "") Then
                    'バージョンの先頭文字が”/”の場合
                    If FrastChar(strRead(1)) = True Then
                        Ver00 = strRead(1).Substring(1)
                        'バージョンが４桁以外の場合
                        If Ver00.Length <> 4 Then
                            SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                            Return False
                        End If
                        'バージョン番号チェック異常
                        If (OPMGUtility.checkNumber(Ver00)) = False Then
                            SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                            Return False
                        End If
                    Else
                        Ver00 = strRead(1).ToString
                        SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                        Return False
                    End If
                Else
                    SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                    Return False
                End If
            End If
            'データ部
        Else
            '項目別チェック
            For j = 0 To infoObj.Length - 1
                If (j = 0) Then
                    'ユーザチェック
                    If (UsPsCheck(strRead(j), CLine, infoObj(j)) = False) Then
                        SetMSGSyousai(ERRFst, CLine, ERRCODE1)
                        Return False
                    End If
                ElseIf (j = 1) Then
                    'IDチェック
                    If (UsPsCheck(strRead(j), CLine, infoObj(j)) = False) Then
                        SetMSGSyousai(ERRFst, CLine, ERRCODE2)
                        Return False
                    End If
                ElseIf (j > 2) Then
                    'ログアウトから各項目の属性チェック
                    If (strRead(j).ToString <> "") Then
                        'フィールド形式チェック
                        If OPMGUtility.checkNumber(strRead(j)) = False Then
                            SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                            Return False
                        Else
                            '入力チェックエラー
                            If ((Integer.Parse(strRead(j)) <> 0) And (Integer.Parse(strRead(j)) <> 1)) Then
                                SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                                Return False
                            End If
                        End If
                    Else
                        SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                        Return False
                    End If
                ElseIf (j = 2) Then
                    'フィールド形式チェック
                    If OPMGUtility.checkNumber(strRead(j)) = False Then
                        SetMSGSyousai(ERRFst, CLine, ERRCODE3)
                        Return False
                    Else
                        '権限チェック
                        If (infoObj(j).FIELD_NAME = "AUTHORITY_LEVEL") Then
                            If ((strRead(j).ToString <> PREMI_USUAL) And
                                    (strRead(j).ToString <> PREMI_ADMIN) And
                                    (strRead(j).ToString <> PREMI_SYS) And
                                    (strRead(j).ToString <> PREMI_SYOSET)) Then
                                '権限エラー
                                SetMSGSyousai(ERRFst, CLine, ERRCODE3)
                                Return False
                            End If
                        End If
                    End If
                End If
                '項目数が29未満の場合
                If strRead.Length - 1 <> infoObj.Length Then
                    SetMSGSyousai(ERRFst, CLine, ERRCODE4)
                    Return False
                End If
            Next
        End If
        Return True
    End Function
    ''' <summary>
    ''' ログ出力
    ''' </summary>
    '''<param name="MSG">メッセージリスト</param>
    Private Function WriteInExportLog(ByVal MSG As ArrayList, Optional ByVal ex As Exception = Nothing) As Boolean
        Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
        Dim enc As System.Text.Encoding = _
        System.Text.Encoding.GetEncoding("Shift_JIS")
        Dim i As Integer = 0
        Dim line As String = ""
        'ログファイルのパスの指定がない空白の場合
        If sLogBasePath Is Nothing Then
            AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
        End If

        Try
            ' ログファイル名作成
            Dim logFile As String = sLogBasePath & "\" & Config.MachineKind & Config.MachineName & "_kengen" & ".log"
            If System.IO.File.Exists(logFile) Then
                Dim sw As StreamReader = New StreamReader(logFile, enc)
                Do While Not sw.Peek() = -1
                    line = sw.ReadLine()
                    MSG.Insert(i, line)
                    i += 1
                Loop
                sw.Close()
                sw = Nothing
                Dim sr As New System.IO.StreamWriter(logFile, False, enc)
                Try
                    'ログ件数チェック
                    '10000行以上の場合、最新の10000行のみ出力
                    If (MSG.Count - 1 > 10000) Then

                        For i = ((MSG.Count) - 10000) To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    Else
                        '10000行以内の場合、すべて出力
                        For i = 0 To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    End If
                    sr.Close()
                Catch ex2 As Exception
                    Return False
                Finally
                    If sr Is Nothing = False Then sr.Close()
                End Try
            Else
                Dim sr As New System.IO.StreamWriter(logFile, False, enc)
                Try
                    'ログ件数チェック
                    '10000行以上の場合、最新の10000行のみ出力
                    If (MSG.Count - 1 > 10000) Then

                        For i = ((MSG.Count - 1) - 10000) To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    Else
                        '10000行以内の場合、すべて出力
                        For i = 0 To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    End If
                    sr.Close()
                Catch ex2 As Exception
                    Return False
                Finally
                    If sr Is Nothing = False Then sr.Close()
                End Try
            End If
        Catch ex2 As Exception
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' エラーメッセージ定義
    ''' </summary>
    '''<param name="ERRFst">メッセージ詳細先頭のスペース</param>
    '''<param name="CLine">エラー行</param>
    '''<param name="ERR">エラーメッセージ</param>
    Private Sub SetMSGSyousai(ByVal ERRFst As String, ByVal CLine As Integer, ByVal ERR As String)
            MSG = ERRFst & CLine.ToString().PadLeft(4) & ERR
            LogLst.Add(MSG)
            ErrCount = ErrCount + 1
    End Sub
    ''' <summary>
    ''' ログ出力
    ''' </summary>
    '''<param name="MSG">メッセージリスト</param>
    Public Shared Sub WriteInExportLog(ByVal MSG As ArrayList, ByVal EnvVarNotFound As AlertBoxAttr, ByVal MachineKind As String, Optional ByVal ex As Exception = Nothing)
        Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
        Dim enc As System.Text.Encoding = _
        System.Text.Encoding.GetEncoding("Shift_JIS")
        Dim i As Integer = 0
        Dim line As String = ""
        'ログファイルのパスの指定がない場合
        If sLogBasePath Is Nothing Then
            AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
        End If

        Try
            ' ログファイル名作成
            Dim logFile As String = sLogBasePath & "\" & MachineKind & "_kengen" & ".log"
            If System.IO.File.Exists(logFile) Then
                Dim sw As StreamReader = New StreamReader(logFile, enc)
                Do While Not sw.Peek() = -1
                    line = sw.ReadLine()
                    MSG.Insert(i, line)
                    i += 1
                Loop
                sw.Close()
                sw = Nothing
                Dim sr As New System.IO.StreamWriter(logFile, False, enc)
                Try
                    'ログ件数チェック
                    '10000行以上の場合、最新の10000行のみ出力
                    If (MSG.Count - 1 > 10000) Then

                        For i = ((MSG.Count - 1) - 10000) To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    Else
                        '10000行以内の場合、すべて出力
                        For i = 0 To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    End If
                    sr.Close()
                Catch ex2 As Exception
                Finally
                    If sr Is Nothing = False Then sr.Close()
                End Try
            Else
                Dim sr As New System.IO.StreamWriter(logFile, False, enc)
                Try
                    'ログ件数チェック
                    '10000行以上の場合、最新の10000行のみ出力
                    If (MSG.Count - 1 > 10000) Then

                        For i = ((MSG.Count - 1) - 10000) To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    Else
                        '10000行以内の場合、すべて出力
                        For i = 0 To MSG.Count - 1
                            sr.WriteLine(MSG(i).ToString)
                        Next
                    End If
                    sr.Close()
                Catch ex2 As Exception
                Finally
                    If sr Is Nothing = False Then sr.Close()
                End Try
            End If
        Catch ex2 As Exception
        End Try
    End Sub
    ''' <summary>
    ''' 明細行の権限チェック
    ''' </summary>
    '''<param name="sAuth">権限</param>
    Private Sub AuthCheck(ByVal sAuth As String)
        If sAuth <> "" Then
            '詳細設定の場合、修正、削除ボタンの非活性化
            If sAuth = AUTH_DETTAILSET Then
                btnUpdate.Enabled = False
                btnDelete.Enabled = False
            Else
                btnUpdate.Enabled = True
                btnDelete.Enabled = True
            End If
        End If
    End Sub

    Private Sub shtIDMst_EnteredCell(sender As Object, e As System.EventArgs) Handles shtIDMst.EnteredCell
        '権限が詳細設定の場合、修正及び削除うボタンが非活性
        Dim nRowno As Integer = shtIDMst.ActivePosition.Row
        sAuth = Me.shtIDMst.Item(2, nRowno).Text
        Try
            Call AuthCheck(sAuth)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub
    '-------Ver0.1　フェーズ２権限対応 ADD　END-------------
#End Region
End Class
