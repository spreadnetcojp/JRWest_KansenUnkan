' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/06/01  (NES)金沢  出力項目拡張対応
'   0.2      2017/01/23  (NES)平野　新IDモデルサービス対応
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
''' 【監視盤設定情報　画面クラス】
''' </summary>
Public Class FrmMntDispKsbConfig
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
    Friend WithEvents wkbMain As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents pnlMado As System.Windows.Forms.Panel
    Friend WithEvents cmbMado As System.Windows.Forms.ComboBox
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents shtMain As GrapeCity.Win.ElTabelleSheet.Sheet
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMntDispKsbConfig))
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.wkbMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtMain = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.pnlMado = New System.Windows.Forms.Panel()
        Me.cmbMado = New System.Windows.Forms.ComboBox()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.pnlBodyBase.SuspendLayout()
        Me.wkbMain.SuspendLayout()
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.pnlBodyBase.Controls.Add(Me.pnlMado)
        Me.pnlBodyBase.Controls.Add(Me.pnlEki)
        Me.pnlBodyBase.Controls.Add(Me.wkbMain)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnKensaku)
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 87)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2017/01/23(月)  10:21"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.White
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        '
        'wkbMain
        '
        Me.wkbMain.Controls.Add(Me.shtMain)
        Me.wkbMain.Location = New System.Drawing.Point(13, 66)
        Me.wkbMain.Name = "wkbMain"
        Me.wkbMain.ProcessTabKey = False
        Me.wkbMain.ShowTabs = False
        Me.wkbMain.Size = New System.Drawing.Size(988, 482)
        Me.wkbMain.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wkbMain.TabIndex = 8
        '
        'shtMain
        '
        Me.shtMain.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain.Data = CType(resources.GetObject("shtMain.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain.Location = New System.Drawing.Point(2, 2)
        Me.shtMain.Name = "shtMain"
        Me.shtMain.Size = New System.Drawing.Size(968, 462)
        Me.shtMain.TabIndex = 0
        Me.shtMain.TabStop = False
        Me.shtMain.TransformEditor = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(705, 584)
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
        Me.btnReturn.Location = New System.Drawing.Point(873, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 10
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(873, 7)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 7
        Me.btnKensaku.Text = "検　索"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'pnlMado
        '
        Me.pnlMado.Controls.Add(Me.cmbMado)
        Me.pnlMado.Controls.Add(Me.lblMado)
        Me.pnlMado.Location = New System.Drawing.Point(241, 14)
        Me.pnlMado.Name = "pnlMado"
        Me.pnlMado.Size = New System.Drawing.Size(284, 33)
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
        Me.cmbMado.Size = New System.Drawing.Size(214, 21)
        Me.cmbMado.TabIndex = 1
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
        Me.pnlEki.Location = New System.Drawing.Point(9, 14)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(226, 33)
        Me.pnlEki.TabIndex = 1
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Items.AddRange(New Object() {"", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ", "ＸＸＸＸＸＸＸＸ"})
        Me.cmbEki.Location = New System.Drawing.Point(41, 6)
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
        'FrmMntDispKsbConfig
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMntDispKsbConfig"
        Me.Text = "運用端末 Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.wkbMain.ResumeLayout(False)
        CType(Me.shtMain, System.ComponentModel.ISupportInitialize).EndInit()
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
    Private LbInitCallFlg As Boolean = True

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean
    '-------Ver0.1　出力項目拡張対応　MOD START-----------
    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private LcstXlsSheetName As String
    '-------Ver0.1　出力項目拡張対応　MOD END-----------
    ''' <summary>
    ''' 駅コードの先頭3桁:「000」
    ''' </summary>
    Private ReadOnly LcstEkiSentou As String = "000"

    ''' <summary>
    ''' Title情報
    ''' </summary>
    Private Const FormTitle As String = "監視盤設定情報"
    '-------Ver0.1　出力項目拡張対応　MOD START-----------
    '-------Ver0.2　新IDモデルサービス対応　ADD START---------
    ''' <summary>
    ''' 一覧ヘッダのソート列割り当て
    ''' （一覧ヘッダクリック時に割り当てる対象列を定義。列番号はゼロ相対で"-1"はソート対象外の列）
    ''' </summary>
    Private ReadOnly LcstSortCol() As Integer = {-1, -1, -1, -1, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                                                 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                                                 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 34,
                                                 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58,
                                                 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73,
                                                 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88,
                                                 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104,
                                                 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
                                                 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136,
                                                 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151,
                                                 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166,
                                                 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181,
                                                 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196,
                                                 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
                                                 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228,
                                                 229, 230, 231, 232, 233, 234, 235}

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した別集札データに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
                                                 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26,
                                                 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
                                                 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
                                                 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62,
                                                 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
                                                 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86,
                                                 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104,
                                                 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
                                                 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136,
                                                 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151,
                                                 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166,
                                                 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181,
                                                 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196,
                                                 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
                                                 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228,
                                                 229, 230, 231, 232, 233, 234, 235}
    '-------Ver0.2　新IDモデルサービス対応　ADD END-----------
    '-------Ver0.1　出力項目拡張対応　MOD 　　END---------
    '-------Ver0.1　出力項目拡張対応　ADD START-----------
    'グループ番号
    Private GrpNo As Integer = 0
    '特別コーナー判定フラグ
    Private CorFlg As Boolean = False
    '-------Ver0.1　出力項目拡張対応　ADD 　　END---------
    ''' <summary>
    ''' 一覧表示最大列
    ''' </summary>
    Private LcstMaxColCnt As Integer

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

#End Region

#Region "メソッド（Public）"

    ''' <summary>
    ''' [画面初期処理]
    ''' エラー発生時は内部でメッセージを表示します。
    ''' </summary>
    ''' <returns>True:成功,False:失敗</returns>
    Public Function InitFrm() As Boolean
        Dim bRtn As Boolean = False
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Try
            Log.Info("Method started.")

            '画面タイトル
            lblTitle.Text = FormTitle

            'シート初期化
            shtMain.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtMain.ViewMode = ElTabelleSheet.ViewMode.Row                      '行選択モード
            shtMain.MaxRows() = 0                                               '行の初期化
            LcstMaxColCnt = shtMain.MaxColumns()                                '列数を取得
            shtMain.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード
            'シートのヘッダ選択イベントのハンドラ追加
            shtMain.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader
            AddHandler Me.shtMain.ColumnHeaders.HeaderClick, AddressOf Me.shtMainColumnHeaders_HeadersClick

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

            '各コンボボックスの項目登録()
            If LfSetEki() = False Then Exit Try '駅名コンボボックス設定
            cmbEki.SelectedIndex = 0            'デフォルト表示項目
            If LfSetMado(cmbEki.SelectedValue.ToString) = False Then Exit Try 'コーナーコンボボックス設定
            cmbMado.SelectedIndex = 0           'デフォルト表示項目

            '一覧ソートの初期化()
            LfClrList()
            '-------Ver0.1　出力項目拡張対応　ADD START---------
            LfListSet()
            '-------Ver0.1　出力項目拡張対応　ADD   END---------
            LbEventStop = False 'イベント発生ＯＮ
            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)        '開始異常メッセージ
            End If
            LbEventStop = False 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

#End Region

#Region "イベント"

    ''' <summary>
    ''' フォームロード
    ''' </summary>
    Private Sub FrmMntDispKsbConfig_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrm() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If
            '検索ボタン活性化
            LfSearchTrue()
            cmbEki.Select() '初期フォーカス
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////ボタンクリック

    ''' <summary>
    ''' 終了
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnReturn.Click
        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()
    End Sub

    ''' <summary>
    ''' 検索
    ''' </summary>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnKensaku.Click

        If LbEventStop Then Exit Sub
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        LfWaitCursor()

        Try
            LbEventStop = True
            LogOperation(sender, e)    'ボタン押下ログ
            '初期化処理
            LfClrList()

            '運用管理端末のINIファイルから取得可能件数を取得
            Dim nMaxCount As Integer = Config.MaxUpboundDataToGet

            '件数取得チェック
            sSql = LfGetSelectString(SlcSQLType.SlcCount)
            nRtn = BaseSqlDataTableFill(sSql, dt)

            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                    Exit Sub
                Case Else
                    '上限チェック
                    If Convert.ToInt64(dt.Rows(0)(0)) > nMaxCount Then
                        AlertBox.Show(Lexis.HugeRecordsFound, nMaxCount.ToString())
                        cmbEki.Select()
                        Exit Sub
                    ElseIf Convert.ToInt64(dt.Rows(0)(0)) = 0 Then
                        AlertBox.Show(Lexis.NoRecordsFound)
                        cmbEki.Select()
                        Exit Sub
                    End If
            End Select

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
                Case Is > nMaxCount     '件数＞取得可能件数
                    AlertBox.Show(Lexis.HugeRecordsFound, nMaxCount.ToString())
                    cmbEki.Select()
                    Exit Sub
            End Select
            '-------Ver0.1　出力項目拡張対応　ADD START---------
            dt.Columns.Remove("CORNER_CODE")
            '-------Ver0.1　出力項目拡張対応　ADD 　END---------
            '取得データを一覧に設定
            LfSetSheetData(dt)
            '一覧、出力ボタン活性化
            If shtMain.Enabled = False Then shtMain.Enabled = True
            If btnPrint.Enabled = False Then btnPrint.Enabled = True
            shtMain.Select()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ
            btnReturn.Select()
        Finally
            'DB開放()
            dt = Nothing
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 出力
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click

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
            '-------Ver0.1　出力項目拡張対応　MOD START---------
            If CorFlg = True Then
                sPath = Path.Combine(sPath, Config.KsbConfigPrintDirect)
                LcstXlsSheetName = Config.KsbConfigPrintDirect.Substring(0, Config.KsbConfigPrintDirect.Length - 4)
            Else
                sPath = Path.Combine(sPath, Config.KsbPrintList(GrpNo).ToString)
                LcstXlsSheetName = Config.KsbPrintList(GrpNo).ToString.Substring(0, Config.KsbPrintList(GrpNo).ToString.Length - 4)
            End If

            '-------Ver0.1　出力項目拡張対応　MOD 　END---------

            If File.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If

            '出力
            LfXlsStart(sPath)
            cmbEki.Select()
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

    '//////////////////////////////////////////////SelectedIndexChanged

    '''<summary>
    ''' 「駅」コンボ
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbEki.SelectedIndexChanged

        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LfClrList()
            'コーナーコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            '-------Ver0.1　出力項目拡張対応　ADD 　START---------
            '特別コーナーフラグを初期化
            CorFlg = False
            Dim station As String = cmbEki.SelectedValue.ToString
            If (station <> "" And station <> ClientDaoConstants.TERMINAL_ALL) Then
                GrpNo = CInt(station.Substring(0, 1))
            ElseIf station = ClientDaoConstants.TERMINAL_ALL Then
                GrpNo = ClientDaoConstants.TERMINAL_ALL_GrpNo
            End If
            If LfSetMado(station.Substring(station.Length - 6, 6)) = False Then
                '-------Ver0.1　出力項目拡張対応　ADD 　END---------
                If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
                If cmbMado.Enabled = True Then BaseCtlDisabled(pnlMado, False)
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbMado.SelectedIndex = 0               '★イベント発生箇所
            If cmbMado.Enabled = False Then BaseCtlEnabled(pnlMado)
            If btnKensaku.Enabled = False Then btnKensaku.Enabled = True
            LfSearchTrue()
            '-------Ver0.1　出力項目拡張対応　ADD START---------
            LfListSet()
            '-------Ver0.1　出力項目拡張対応　ADD   END---------
        Catch ex As Exception
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
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
            LfClrList()
            LfSearchTrue()
            '-------Ver0.1　特別コーナー対応　ADD START---------
            'フラグのチェインジ判断
            Dim ChFlg As Boolean = CorFlg
            '特別コーナーの場合
            If CheckCorner() = True Then
                '特別コーナーチェックで、コーナーフラグが変更された場合
                If CorFlg <> ChFlg Then
                    LfListSet()
                End If
            Else
                'コーナーフラグも変更されてない場合
                If CorFlg <> ChFlg Then
                    LfListSet()
                End If
            End If
            '-------Ver0.1　特別コーナー対応　ADD   END---------
        Catch ex As Exception
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    '//////////////////////////////////////////////ElTable関連

    Private Sub shtMainColumnHeaders_HeadersClick(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ClickEventArgs)

        Static intCurrentSortColumn As Integer = -1
        '-------Ver0.1　出力項目拡張対応　MOD START---------
        Static bolColumn1SortOrder(LcstMaxColCnt) As Boolean
        '-------Ver0.1　出力項目拡張対応　MOD START---------
        Try
            If LcstSortCol(e.Column) = -1 Then Exit Sub
            shtMain.BeginUpdate()

            '前回選択された列ヘッダの初期化
            If intCurrentSortColumn > -1 Then
                '列ヘッダのイメージを削除する
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = Nothing
                '列の背景色を初期化する
                shtMain.Columns(intCurrentSortColumn).BackColor = Color.Empty
                '列のセル罫線を消去する
                shtMain.Columns(intCurrentSortColumn).SetBorder( _
                    New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.Empty, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.None), _
                    GrapeCity.Win.ElTabelleSheet.Borders.All)
            End If

            '選択された列番号を保存
            intCurrentSortColumn = e.Column

            'ソートする列の背景色を設定する
            shtMain.Columns(intCurrentSortColumn).BackColor = Color.WhiteSmoke
            'ソートする列のセル罫線を設定する
            shtMain.Columns(intCurrentSortColumn).SetBorder( _
                New GrapeCity.Win.ElTabelleSheet.BorderLine(Color.LightGray, GrapeCity.Win.ElTabelleSheet.BorderLineStyle.Thin), _
                GrapeCity.Win.ElTabelleSheet.Borders.All)

            If bolColumn1SortOrder(intCurrentSortColumn) = False Then
                '列ヘッダのイメージを設定する
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(1)
                '降順でソートする
                Call SheetSort(shtMain, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Descending)
                '列のソート状態を保存する
                bolColumn1SortOrder(intCurrentSortColumn) = True
            Else
                '列ヘッダのイメージを設定する
                shtMain.ColumnHeaders(intCurrentSortColumn).Image = ImageList1.Images(0)
                '昇順でソートする
                Call SheetSort(shtMain, LcstSortCol(e.Column), GrapeCity.Win.ElTabelleSheet.SortOrder.Ascending)
                '列のソート状態を保存する
                bolColumn1SortOrder(intCurrentSortColumn) = False
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            shtMain.EndUpdate()
        End Try
    End Sub
    ''' <summary>
    ''' MouseMove
    ''' </summary>
    Private Sub shtMain_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim objRange As New GrapeCity.Win.ElTabelleSheet.Range
        'マウスカーソルが列ヘッダ上にある場合
        If shtMain.HitTest(New Point(e.X, e.Y), objRange) = GrapeCity.Win.ElTabelleSheet.SheetArea.ColumnHeader Then
            shtMain.CrossCursor = Cursors.Default
        Else
            'マウスカーソルを既定に戻す
            shtMain.CrossCursor = Nothing
        End If
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

#Region "メソッド（Private）"

    ''' <summary>
    ''' [一覧クリア]
    ''' </summary>
    Private Sub LfClrList()
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try
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
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
        Finally
            wkbMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub
    '-------Ver0.1　出力項目拡張対応　ADD 　START---------
    ''' <summary>
    ''' [一覧制御]
    ''' </summary>
    Private Sub LfListSet()
        Dim SetInfo As String = ""
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try
            '特別コーナー判別し、制御情報を取得
            If CorFlg = False Then
                SetInfo = Config.KsbConfigOutListCol(GrpNo).ToString
            Else
                SetInfo = Config.KsbConfigOutListColDirect
            End If
            Dim i, a As Integer
            '一覧表示制御
            With shtMain
                '全ての列を表示
                For a = 0 To SetInfo.Length - 1
                    .Columns(a).Hidden = False
                Next
                '一覧表示制御
                For i = 0 To SetInfo.Length - 1
                    If SetInfo(i).ToString = "0" Then
                        .Columns(i).Hidden = True
                    End If
                Next
            End With
        Finally
            wkbMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub
    '-------Ver0.1　出力項目拡張対応　ADD 　  END---------

    ''' <summary>
    ''' [検索ボタン活性化]
    ''' </summary>
    Private Sub LfSearchTrue()
        Dim bEnabled As Boolean = True
        If bEnabled Then
            If ((cmbEki.SelectedIndex < 0) OrElse _
                (cmbMado.SelectedIndex < 0)) Then
                bEnabled = False
            End If
        End If
        If bEnabled Then
            If btnKensaku.Enabled = False Then btnKensaku.Enabled = True
        Else
            If btnKensaku.Enabled = True Then btnKensaku.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' [コーナーコンボ設定]
    ''' </summary>
    ''' <param name="Station">駅コード</param>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetMado(ByVal Station As String) As Boolean

        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As CornerMaster
        oMst = New CornerMaster
        Try
            oMst.ApplyDate = ApplyDate
            If String.IsNullOrEmpty(Station) Then
                Station = ""
            End If
            If Station <> "" And Station <> ClientDaoConstants.TERMINAL_ALL Then
                '----------- 0.1  出力項目拡張対応 MOD START------------------------
                dt = oMst.SelectTable(Station, "G")
                '----------- 0.1  出力項目拡張対応 MOD END------------------------
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

        Dim sSql As String = ""
        Dim sEki As String = ""
        Try
            Dim sSqlWhere As New StringBuilder
            Dim sBuilder As New StringBuilder
            '-------Ver0.2　新IDモデルサービス対応　MOD START---------
            sBuilder.AppendLine("")
            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '件数取得項目--------------------------
                    sBuilder.AppendLine(" SELECT COUNT(1) FROM V_KSB_CONFIG2 ")
                Case slcSQLType.SlcDetail
                    '取得項目--------------------------
                    '-------Ver0.1　出力項目拡張対応　MOD 　START---------
                    sBuilder.AppendLine(" SELECT * ")
                    sBuilder.AppendLine(" FROM V_KSB_CONFIG2 ")
                    '-------Ver0.1　出力項目拡張対応　MOD 　END---------
                    Dim s As String = sBuilder.ToString
            End Select
            '-------Ver0.2　新IDモデルサービス対応　MOD END-----------

            'Where句生成--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine("")
            sSqlWhere.AppendLine(" where 0 = 0 ")

            '駅
            If Not (cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sEki = cmbEki.SelectedValue.ToString
                '-------Ver0.1　出力項目拡張対応　MOD 　START---------
                If sEki.Substring(1, 3).Equals(LcstEkiSentou) Then

                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE in {0})", _
                                                       String.Format("(SELECT DISTINCT(RAIL_SECTION_CODE + STATION_ORDER_CODE) AS STATION_CODE" _
                                                                     & " FROM M_MACHINE WHERE BRANCH_OFFICE_CODE = {0}) ", _
                                                                     Utility.SetSglQuot(sEki.Substring(sEki.Length - 3, 3)))))
                Else
                    sSqlWhere.AppendLine(String.Format(" And (STATION_CODE = {0})", Utility.SetSglQuot(sEki.Substring(sEki.Length - 6, 6))))
                End If
                '-------Ver0.1　出力項目拡張対応　MOD 　END---------
            End If
            'コーナー
            If Not (cmbMado.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                sSqlWhere.AppendLine(String.Format(" and (CORNER_CODE = {0})", _
                                          Utility.SetSglQuot(cmbMado.SelectedValue.ToString)))
            End If

            If slcSQLType.Equals(slcSQLType.SlcDetail) Then
                sSqlWhere.AppendLine(" ORDER BY STATION_CODE,CORNER_CODE,SYUSYU_DATE ASC ")
            End If
            'Where句結合()
            sBuilder.AppendLine(sSqlWhere.ToString)

            sSql = sBuilder.ToString()

            Debug.Print(sSql)
            Return sSql
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

    End Function

    ''' <summary>
    ''' [一覧設定]
    ''' </summary>
    ''' <param name="dt">設定対象データテーブル</param>
    Private Sub LfSetSheetData(ByVal dt As DataTable)
        shtMain.Redraw = False
        wkbMain.Redraw = False
        Try
            If Not (shtMain.DataSource Is Nothing) Then
                shtMain.DataSource = Nothing
                shtMain.MaxRows = 0
            End If
            shtMain.MaxRows = dt.Rows.Count         '抽出件数分の行を一覧に作成
            shtMain.Rows.SetAllRowsHeight(21)       '行高さを揃える
            shtMain.DataSource = dt                 'データをセット
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            dt = Nothing
            wkbMain.Redraw = True
            shtMain.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String)

        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 7
        Dim Count As Integer = 0

        Try
            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                ' 帳票ファイル名称を取得
                .FileName = sPath
                ' 帳票の出力処理を開始を宣言
                .Report.Start()
                .Report.File()
                '帳票ファイルシート名称を取得します。
                .Page.Start(LcstXlsSheetName, "1-9999")

                ' 見出し部セルへ見出しデータ出力
                .Cell("B1").Value = lblTitle.Text
                '-------Ver0.1　出力項目拡張対応　MOD START---------
                '-------Ver0.2　新IDモデルサービス対応　MOD START---------
                .Cell("HZ1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("HZ2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                '-------Ver0.2　新IDモデルサービス対応　MOD END-----------
                '-------Ver0.1　出力項目拡張対応　MOD 　END---------
                .Cell("B3").Value = OPMGFormConstants.STATION_NAME + cmbEki.Text.Trim + "　　　" +
                                    OPMGFormConstants.CORNER_STR + cmbMado.Text.Trim

                ' 配信対象のデータ数を取得します
                nRecCnt = shtMain.MaxRows

                '罫線枠を作成
                For i As Integer = 1 To shtMain.MaxRows - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                '印刷
                For i As Integer = 0 To shtMain.MaxRows - 1
                    For j As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(1 + j, i + nStartRow).Value = shtMain.Item(j + 1, i).Text
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
    ''' [駅コンボ設定]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function LfSetEki() As Boolean
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As StationMaster
        oMst = New StationMaster
        Try
            oMst.ApplyDate = ApplyDate
            '-------Ver0.1　出力項目拡張対応　MOD 　START---------
            dt = oMst.SelectTable(True, "W", True)
            '-------Ver0.1　出力項目拡張対応　MOD 　　END---------
            dt = oMst.SetAll()
            bRtn = BaseSetMstDtToCmb(dt, cmbEki)
            cmbEki.SelectedIndex = -1
            If cmbEki.Items.Count <= 0 Then bRtn = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            LfCmbClear(cmbEki)
            LfCmbClear(cmbMado)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
            LbEventStop = False 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

    ''' <summary>
    ''' [指定コンボ初期化]
    ''' </summary>
    ''' <param name="cmb">対象コンボボックスコントロール</param>
    Private Sub LfCmbClear(ByVal cmb As ComboBox)
        Try
            cmb.DataSource = Nothing
            If cmb.Items.Count > 0 Then cmb.Items.Clear()
        Catch ex As Exception
        End Try
    End Sub

    '-------Ver0.1　出力項目拡張対応　ADD 　START---------
    ''' <summary>
    ''' [特別コーナーチェック]
    ''' </summary>
    ''' <returns>True:成功、False:失敗</returns>
    Private Function CheckCorner() As Boolean
        Dim station, sStation As String
        Dim corCode As String
        Dim code As String
        Try
            'INI設定：特別コーナー情報取得
            code = Config.KsbConfigDirectEkCode.ToString
            'INI設定：特別コーナーの駅
            station = code.Substring(0, 3) & code.Substring(4, 3)
            'INI設定：特別コーナー
            corCode = code.Substring(code.Length - 2, 2)
            '選択中の駅
            sStation = cmbEki.SelectedValue.ToString
            '選択中のコーナーチェック
            If sStation.Substring(sStation.Length - 6, 6) = station And cmbMado.SelectedValue.ToString = corCode Then
                CorFlg = True
            Else
                CorFlg = False
            End If
        Catch ex As Exception
        End Try
        Return CorFlg
    End Function
    '-------Ver0.1　出力項目拡張対応　ADD 　END---------
#End Region

End Class