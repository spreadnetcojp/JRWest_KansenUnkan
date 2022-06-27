' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/06/01       金沢  北陸対応
'   0.2      2015/04/01       金沢  入力値チェック
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '定数値のみ使用
Imports GrapeCity.Win
Imports System.IO
Imports System.Text
Imports System.Data.SqlClient

''' <summary>
''' 【稼動・保守マスタ設定】
''' </summary>
Public Class FrmSysKadoDataMst
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
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents lblLastDate As System.Windows.Forms.Label
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents shtMain1 As GrapeCity.Win.ElTabelleSheet.Sheet
    Friend WithEvents pnlKisyu As System.Windows.Forms.Panel
    Friend WithEvents cmbKishu As System.Windows.Forms.ComboBox
    Friend WithEvents lblKisyu As System.Windows.Forms.Label
    Friend WithEvents pnlBui As System.Windows.Forms.Panel
    Friend WithEvents grpStandKind As System.Windows.Forms.GroupBox
    Friend WithEvents rdoKado As System.Windows.Forms.RadioButton
    Friend WithEvents rdoManbunhi As System.Windows.Forms.RadioButton
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents wkbMain As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysKadoDataMst))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.rdoManbunhi = New System.Windows.Forms.RadioButton()
        Me.rdoKado = New System.Windows.Forms.RadioButton()
        Me.lblLastDate = New System.Windows.Forms.Label()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.shtMain1 = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.pnlKisyu = New System.Windows.Forms.Panel()
        Me.cmbKishu = New System.Windows.Forms.ComboBox()
        Me.lblKisyu = New System.Windows.Forms.Label()
        Me.grpStandKind = New System.Windows.Forms.GroupBox()
        Me.pnlBui = New System.Windows.Forms.Panel()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.wkbMain = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.pnlBodyBase.SuspendLayout()
        CType(Me.shtMain1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlKisyu.SuspendLayout()
        Me.grpStandKind.SuspendLayout()
        Me.pnlBui.SuspendLayout()
        Me.wkbMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.wkbMain)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.lblLastDate)
        Me.pnlBodyBase.Controls.Add(Me.btnUpdate)
        Me.pnlBodyBase.Controls.Add(Me.pnlKisyu)
        Me.pnlBodyBase.Controls.Add(Me.pnlBui)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/09/08(日)  17:10"
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
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!)
        Me.btnPrint.Location = New System.Drawing.Point(872, 516)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 5
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'rdoManbunhi
        '
        Me.rdoManbunhi.AutoSize = True
        Me.rdoManbunhi.Location = New System.Drawing.Point(138, 23)
        Me.rdoManbunhi.Name = "rdoManbunhi"
        Me.rdoManbunhi.Size = New System.Drawing.Size(109, 17)
        Me.rdoManbunhi.TabIndex = 3
        Me.rdoManbunhi.Text = "万分比データ"
        Me.rdoManbunhi.UseVisualStyleBackColor = True
        '
        'rdoKado
        '
        Me.rdoKado.AutoSize = True
        Me.rdoKado.Checked = True
        Me.rdoKado.Location = New System.Drawing.Point(13, 23)
        Me.rdoKado.Name = "rdoKado"
        Me.rdoKado.Size = New System.Drawing.Size(95, 17)
        Me.rdoKado.TabIndex = 2
        Me.rdoKado.TabStop = True
        Me.rdoKado.Text = "稼動データ"
        Me.rdoKado.UseVisualStyleBackColor = True
        '
        'lblLastDate
        '
        Me.lblLastDate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblLastDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblLastDate.Location = New System.Drawing.Point(37, 12)
        Me.lblLastDate.Name = "lblLastDate"
        Me.lblLastDate.Size = New System.Drawing.Size(347, 18)
        Me.lblLastDate.TabIndex = 120
        Me.lblLastDate.Text = "◆最終登録日時：　2004年07月20日　13:10"
        Me.lblLastDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(873, 450)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(128, 40)
        Me.btnUpdate.TabIndex = 4
        Me.btnUpdate.Text = "登　録"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'shtMain1
        '
        Me.shtMain1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtMain1.Data = CType(resources.GetObject("shtMain1.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtMain1.Location = New System.Drawing.Point(1, 1)
        Me.shtMain1.Name = "shtMain1"
        Me.shtMain1.Size = New System.Drawing.Size(800, 486)
        Me.shtMain1.TabIndex = 114
        Me.shtMain1.TabStop = False
        Me.shtMain1.TransformEditor = False
        '
        'pnlKisyu
        '
        Me.pnlKisyu.Controls.Add(Me.cmbKishu)
        Me.pnlKisyu.Controls.Add(Me.lblKisyu)
        Me.pnlKisyu.Location = New System.Drawing.Point(40, 55)
        Me.pnlKisyu.Name = "pnlKisyu"
        Me.pnlKisyu.Size = New System.Drawing.Size(344, 33)
        Me.pnlKisyu.TabIndex = 118
        '
        'cmbKishu
        '
        Me.cmbKishu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKishu.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKishu.ItemHeight = 12
        Me.cmbKishu.Location = New System.Drawing.Point(45, 7)
        Me.cmbKishu.Name = "cmbKishu"
        Me.cmbKishu.Size = New System.Drawing.Size(199, 20)
        Me.cmbKishu.TabIndex = 1
        '
        'lblKisyu
        '
        Me.lblKisyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblKisyu.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKisyu.Location = New System.Drawing.Point(4, 6)
        Me.lblKisyu.Name = "lblKisyu"
        Me.lblKisyu.Size = New System.Drawing.Size(45, 21)
        Me.lblKisyu.TabIndex = 91
        Me.lblKisyu.Text = "機種"
        Me.lblKisyu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'grpStandKind
        '
        Me.grpStandKind.Controls.Add(Me.rdoKado)
        Me.grpStandKind.Controls.Add(Me.rdoManbunhi)
        Me.grpStandKind.Location = New System.Drawing.Point(14, 11)
        Me.grpStandKind.Name = "grpStandKind"
        Me.grpStandKind.Size = New System.Drawing.Size(285, 56)
        Me.grpStandKind.TabIndex = 2
        Me.grpStandKind.TabStop = False
        Me.grpStandKind.Text = "基準値変更データ"
        '
        'pnlBui
        '
        Me.pnlBui.Controls.Add(Me.grpStandKind)
        Me.pnlBui.Location = New System.Drawing.Point(547, 12)
        Me.pnlBui.Name = "pnlBui"
        Me.pnlBui.Size = New System.Drawing.Size(312, 84)
        Me.pnlBui.TabIndex = 119
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(873, 582)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 6
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'wkbMain
        '
        Me.wkbMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.wkbMain.Controls.Add(Me.shtMain1)
        Me.wkbMain.Location = New System.Drawing.Point(40, 119)
        Me.wkbMain.Name = "wkbMain"
        Me.wkbMain.ProcessTabKey = False
        Me.wkbMain.ShowTabs = False
        Me.wkbMain.Size = New System.Drawing.Size(819, 505)
        Me.wkbMain.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.wkbMain.TabIndex = 121
        '
        'FrmSysKadoDataMst
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmSysKadoDataMst"
        Me.Text = "運用端末 V1.00"
        Me.pnlBodyBase.ResumeLayout(False)
        CType(Me.shtMain1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlKisyu.ResumeLayout(False)
        Me.grpStandKind.ResumeLayout(False)
        Me.grpStandKind.PerformLayout()
        Me.pnlBui.ResumeLayout(False)
        Me.wkbMain.ResumeLayout(False)
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
    ''' 一覧表示最大列
    ''' </summary>
    Private ReadOnly LcstMaxColCnt As Integer = 8

    ''' <summary>
    ''' 固定文字
    ''' </summary>
    Private ReadOnly LcstLstCmtDt As String = "◆最終登録日時："

    ''' <summary>
    ''' コンボ選択Index退避用
    ''' </summary>
    Private Structure SearchCodeInf
        Dim nKisyu As Integer
    End Structure
    Private stSearchInf As SearchCodeInf

    '検索SQL取得区分
    Private Enum SlcSQLType
        SlcCount = 0  '件数取得用
        SlcDetail = 1 'データ検索用
    End Enum

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetWinName As String = "稼動・保守データ設定_窓口処理機"
    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly FormTitle As String = "稼動・保守データ設定"
    ''' <summary>
    ''' 帳票出力のタイトル
    ''' </summary>
    Private ReadOnly LcstXlsSheetTitle As String = "稼動データ設定"
    ''' <summary>
    ''' 列のサイズ
    ''' </summary>
    Private Const LcstColWidth As Integer = 142
    ''' <summary>
    ''' Exception情報
    ''' </summary>
    Private Const LcstColChgName As String = "改基準値"
    ''' <summary>
    ''' Exception情報
    ''' </summary>
    Private Const LcstColName As String = "基準値"

    '機種名称
    Private Const MachinTypeMod As String = "G" '改札機
    Private Const MachinTypeWin As String = "Y" '窓口処理機

    Private LastDate As DateTime = Nothing

#End Region

#Region "メソッド（Public）"

    ''' <summary>
    ''' [画面初期処理]
    ''' エラー発生時は内部でメッセージを表示します。
    ''' </summary>
    ''' <returns>True:成功,False:失敗</returns>
    Public Function InitFrm() As Boolean
        Dim bRtn As Boolean = False
        Dim sErrSub As String = ""
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim sLastTest As String = ""
        Try
            Log.Info("Method started.")

            '画面タイトル
            lblTitle.Text = FormTitle

            'シート初期化
            shtMain1.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
            shtMain1.MaxRows = 0                                                 '行の初期化
            'シートのヘッダ選択イベントのハンドラ追加
            shtMain1.SelectableArea = GrapeCity.Win.ElTabelleSheet.SelectableArea.CellsWithRowHeader

            'Enter キーで次のセルに移動する
            Dim aryKeyActions(0) As GrapeCity.Win.ElTabelleSheet.KeyAction
            aryKeyActions(0) = GrapeCity.Win.ElTabelleSheet.KeyAction.NextCellWithWrap
            shtMain1.ShortCuts.Add(Keys.Enter, aryKeyActions)

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

            'コンボ選択インデックス退避エリア初期化
            With stSearchInf
                .nKisyu = 0
            End With

            '機種コンボ設定
            SetCombox()
            cmbKishu.SelectedIndex = 0
            rdoKado.Checked = True

            '一覧データ設定
            LbEventStop = False
            LfSetList()
            If shtMain1.Rows.Count < 1 Then
                bRtn = False
            Else
                '改札機、稼動データ場合
                shtMain1.Columns(5).Hidden = True
                shtMain1.Columns(6).Hidden = True

                bRtn = True
            End If
            LbEventStop = True
        Catch ex As OPMGException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
            End If
            cmbKishu.Select()
            LbEventStop = False                 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

#End Region

#Region "イベント"

    ''' <summary>
    ''' フォームロード
    ''' </summary>
    Private Sub FrmSysKadoDataMst_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
        LfWaitCursor()
        Try
            If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
                If InitFrm() = False Then   '初期処理
                    Me.Close()
                    Exit Sub
                End If
            End If
            LbEventStop = False     'イベント発生ＯＮ
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
    ''' 登録
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        LbEventStop = True      'イベント発生ＯＦＦ
        LfWaitCursor()

        Dim i As Integer
        Dim dt As DataTable = Nothing
        Dim dLastDate As DateTime = Nothing
        Dim dbCtl As DatabaseTalker
        dbCtl = New DatabaseTalker

        Try
            LogOperation(sender, e)    'ボタン押下ログ

            'メッセージ表示
            If AlertBox.Show(AlertBoxAttr.OKCancel, Lexis.ReallyInsert) = DialogResult.Cancel Then
                Exit Sub
            End If

            '排他チェック
            LfGetLstCmtDt(dLastDate)
            If Not dLastDate = LastDate Then
                AlertBox.Show(Lexis.CompetitiveOperationDetected)
                Exit Sub
            End If

            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            '登録確認
            For i = 0 To shtMain1.MaxRows - 1
                If shtMain1.Item(shtMain1.MaxColumns - 1, i).Text = "1" Then
                    dbCtl.ExecuteSQLToWrite(LfUpdateSQL(i))
                End If
            Next i
            dbCtl.TransactionCommit()

            '最終登録日時設定
            LfGetLstCmtDtText()

            '更新成功場合
            Log.Info("commit successed.")
            AlertBox.Show(Lexis.InsertCompleted) '登録成功
            '登録ボタン、使用不可
            If btnUpdate.Enabled = True Then btnUpdate.Enabled = False
            '出力ボタン使用可能
            If btnPrint.Enabled = False Then btnPrint.Enabled = True
        Catch ex As OPMGException
            Log.Fatal("Unwelcome Exception caught.", ex)
            btnUpdate.Enabled = True
            btnPrint.Enabled = False
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred) 'DB接続失敗メッセージ
            btnReturn.Select()
        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            dbCtl.TransactionRollBack()
            btnUpdate.Enabled = True
            btnPrint.Enabled = False
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred) 'DB接続失敗メッセージ
            btnReturn.Select()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            dbCtl.TransactionRollBack()
            btnUpdate.Enabled = True
            btnPrint.Enabled = False
            AlertBox.Show(Lexis.InsertFailed) '登録失敗メッセージ
            btnReturn.Select()
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
            LfWaitCursor(False)
            LbEventStop = False 'イベント発生ＯＮ
        End Try

    End Sub

    '''<summary>
    ''' 「機種」コンボ
    ''' </summary>
    Private Sub cmbKisyu_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles cmbKishu.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        If stSearchInf.nKisyu = CType(sender, ComboBox).SelectedIndex Then Exit Sub
        LbEventStop = True      'イベント発生ＯＦＦ
        LfWaitCursor()
        Try
            LogOperation(sender, e)    'ボタン押下ログ

            '基準値変更データの変更
            If cmbKishu.SelectedValue.ToString.Substring(1, 1) = MachinTypeMod Then
                rdoManbunhi.Enabled = True
            ElseIf cmbKishu.SelectedValue.ToString.Substring(1, 1) = MachinTypeWin Then '窓口処理機
                rdoKado.Checked = True
                rdoManbunhi.Enabled = False
            End If

            '一覧表の列の設定
            '-------Ver0.1　北陸対応　MOD START-----------
            If cmbKishu.SelectedValue.ToString.Substring(1, 1) = MachinTypeMod Then '改札機
                '-------Ver0.1　北陸対応　MOD END-----------
                shtMain1.Columns(4).Hidden = False

                shtMain1.ColumnHeaders(3).Value = LcstColChgName
                shtMain1.Columns(3).Width = LcstColWidth
                shtMain1.Columns(4).Width = LcstColWidth
                If rdoKado.Checked = True Then
                    shtMain1.Columns(3).Hidden = False
                    shtMain1.Columns(4).Hidden = False
                    shtMain1.Columns(5).Hidden = True
                    shtMain1.Columns(6).Hidden = True
                Else
                    shtMain1.Columns(3).Hidden = True
                    shtMain1.Columns(4).Hidden = True
                    shtMain1.Columns(5).Hidden = False
                    shtMain1.Columns(6).Hidden = False
                End If
                '-------Ver0.1　北陸対応　MOD START-----------
            ElseIf cmbKishu.SelectedValue.ToString.Substring(1, 1) = MachinTypeWin Then '窓口処理機
                '-------Ver0.1　北陸対応　MOD END-----------
                shtMain1.Columns(4).Hidden = True

                shtMain1.ColumnHeaders(3).Caption = LcstColName
                shtMain1.Columns(3).Width = LcstColWidth * 2
                If rdoKado.Checked = True Then
                    shtMain1.Columns(3).Hidden = False
                    shtMain1.Columns(5).Hidden = True
                    shtMain1.Columns(6).Hidden = True
                Else
                    shtMain1.Columns(3).Hidden = True
                    shtMain1.Columns(5).Hidden = False
                    shtMain1.Columns(6).Hidden = False
                End If
            End If

            '一覧データ設定
            LbEventStop = False
            LfSetList()
            LbEventStop = True

        Catch EX As OPMGException
            Log.Fatal("Unwelcome Exception caught.", EX)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred) 'DBエラー
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            stSearchInf.nKisyu = CType(sender, ComboBox).SelectedIndex
            LfWaitCursor(False)
            LbEventStop = False 'イベント発生ＯＮ
        End Try
    End Sub

    ''' <summary>
    ''' 基準値変更データ選択変更
    ''' </summary>
    Private Sub rdoKado_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoKado.CheckedChanged
        If LbInitCallFlg = False Then Exit Sub
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        LbEventStop = True
        Try
            LogOperation(sender, e)    'ボタン押下ログ

            LbEventStop = False
            LfSetList() '一覧
            LbEventStop = True

            '基準値:稼動データ場合
            If (rdoKado.Checked = True) Then
                shtMain1.Columns(6).Hidden = True
                shtMain1.Columns(5).Hidden = True
                shtMain1.Columns(4).Hidden = False
                shtMain1.Columns(3).Hidden = False
            Else '万分比
                shtMain1.Columns(3).Hidden = True
                shtMain1.Columns(4).Hidden = True
                shtMain1.Columns(6).Hidden = False
                shtMain1.Columns(5).Hidden = False
            End If

        Catch EX As OPMGException
            Log.Fatal("Unwelcome Exception caught.", EX)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred) 'DBエラー
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 「値変更確定後」
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub shtMain_ValueChanged(ByVal sender As Object, ByVal e As GrapeCity.Win.ElTabelleSheet.ValueChangedEventArgs) Handles shtMain1.ValueChanged
        If LbEventStop Then Exit Sub
        LbEventStop = True      'イベント発生ＯＦＦ
        LfWaitCursor()
        Try
            '変更フラグＯＮ
            shtMain1.Item(shtMain1.Columns.Count - 1, e.Position.Row).Text = "1"

            '-----Ver0.2  入力値チェック　ADD  START -------------------------
            If shtMain1.Item(e.Position.Column, e.Position.Row).Text.ToString = "0" _
                Or shtMain1.Item(e.Position.Column, e.Position.Row).Text.ToString = "" Then
                shtMain1.Item(e.Position.Column, e.Position.Row).Text = "0"
            End If
            '-----Ver0.2  入力値チェック　ADD  END -------------------------

            '登録ボタン使用可能
            If btnUpdate.Enabled = False Then btnUpdate.Enabled = True
            '出力ボタン使用可能
            If btnPrint.Enabled = True Then btnPrint.Enabled = False
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            LfWaitCursor(False)
            LbEventStop = False 'イベント発生ＯＮ
        End Try
    End Sub

    ''' <summary>
    ''' 出力
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            LbEventStop = True
            LogOperation(sender, e)     'ボタン押下ログ

            Dim sPath As String = Config.LedgerTemplateDirPath
            'テンプレート格納フォルダチェック
            If Directory.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If

            'テンプレートフルパスチェック
            '改札機
            '-------Ver0.1　北陸対応　MOD START-----------
            Dim kCode As Integer = cmbKishu.SelectedIndex
            sPath = Path.Combine(sPath, Config.KadoPrintSetList(kCode).ToString)
            '-------Ver0.1　北陸対応　MOD START-----------
            If File.Exists(sPath) = False Then
                Log.Error("It's not found [" & sPath & "].")
                AlertBox.Show(Lexis.LedgerTemplateNotFound)
                btnReturn.Select()
                Exit Sub
            End If

            '出力
            LfXlsStart(sPath)
            cmbKishu.Select()
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

#End Region

#Region "メソッド（Private）"

    ''' <summary>
    ''' [一覧クリア]
    ''' </summary>
    Private Sub LfClrList()
        Dim sXYRange As String
        shtMain1.Redraw = False
        Try
            Dim i As Integer
            'ソート情報のクリア
            With shtMain1
                For i = 0 To LcstMaxColCnt - 1
                    .ColumnHeaders(i).Image = Nothing
                    .Columns(i).BackColor = Color.Empty
                Next
            End With
            shtMain1.DataSource = Nothing
            If shtMain1.MaxRows > 0 Then
                sXYRange = "1:" & shtMain1.MaxRows.ToString
                shtMain1.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
            End If
            shtMain1.MaxRows = 0
        Finally
            shtMain1.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' [一覧データ設定]
    ''' </summary>
    Private Sub LfGetList(ByRef dt As DataTable)
        If LbEventStop Then Exit Sub
        Dim nRtn As Integer
        dt = New DataTable

        Dim sSql As String = ""
        Try
            LbEventStop = True

            'データ取得処理
            sSql = LfGetSelectString(SlcSQLType.SlcDetail)
            nRtn = BaseSqlDataTableFill(sSql, dt)
            Select Case nRtn
                Case -9             'ＤＢオープンエラー
                    Throw New OPMGException()
            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException()
        Finally
            LbEventStop = False
        End Try
    End Sub

    ''' <summary>
    ''' [一覧データ設定]
    ''' </summary>
    Private Sub LfSetList()
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim nRet As Boolean = False

        Try
            '一覧クリア
            LfClrList()

            LbEventStop = False
            LfGetList(dt)
            LbEventStop = True

            '取得データを一覧に設定
            LfSetSheetData(dt)

            '一覧、出力ボタン活性化
            If dt.Rows.Count > 0 Then
                nRet = True
            Else
                AlertBox.Show(Lexis.NoRecordsFound)
                cmbKishu.Select()
            End If

            '最終登録日時設定
            LfGetLstCmtDtText()

        Catch ex As OPMGException
            Log.Fatal("Unwelcome Exception caught.", ex)
            btnReturn.Select()
            Throw New OPMGException()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            btnReturn.Select()
            Throw New Exception()
        Finally

            '一覧、出力ボタン活性化
            If nRet Then
                btnUpdate.Enabled = False
                btnPrint.Enabled = True
            Else
                btnUpdate.Enabled = False
                btnPrint.Enabled = False
            End If

            dt = Nothing
            LbEventStop = False
        End Try
    End Sub

    ''' <summary>
    ''' [最終登録日時設定]
    ''' </summary>
    Private Sub LfGetLstCmtDt(ByRef lastD As DateTime)

        Dim sRtn As String = LcstLstCmtDt
        Dim da As DataTable = Nothing
        Dim sSql As String = "SELECT MAX(LAST_DATE) AS RTN FROM M_KADOHOSYU_SET"

        Try
            Dim nRtn As Integer = BaseSqlDataTableFill(sSql, da)
            If nRtn = -9 Then
                Throw New OPMGException
            End If
            If nRtn > 0 Then
                If Not da.Rows(0).Item("RTN").ToString = "" Then
                    lastD = CType(da.Rows(0).Item("RTN"), DateTime)
                End If
            End If

        Finally
            da = Nothing
        End Try

    End Sub

    ''' <summary>
    ''' [最終登録日時設定]
    ''' </summary>
    Private Sub LfGetLstCmtDtText()

        LfGetLstCmtDt(LastDate)
        Dim sRtn As String = LcstLstCmtDt
        lblLastDate.Text = sRtn + Format(LastDate, "yyyy年MM月dd日 HH:mm")

    End Sub

    ''' <summary>
    ''' [検索用SELECT文字列取得]
    ''' </summary>
    ''' <returns>SELECT文</returns>
    Private Function LfGetSelectString(ByVal slcSQLType As SlcSQLType) As String


        Dim sSql As String = ""
        Try
            Dim sSqlWhere As New StringBuilder
            Dim sBuilder As New StringBuilder

            sBuilder.AppendLine("")
            Select Case slcSQLType
                Case slcSQLType.SlcCount
                    '件数取得--------------------------
                    sBuilder.AppendLine(" SELECT COUNT(1) FROM M_KADOHOSYU_SET ")
                Case slcSQLType.SlcDetail
                    '取得項目-------------------------
                    If (rdoKado.Checked = True) Then
                        sBuilder.AppendLine(" SELECT MODEL_CODE")
                        sBuilder.AppendLine(",DATA_SYUBETU")
                        sBuilder.AppendLine(",KOMOKU_NAME ")
                        sBuilder.AppendLine(",KAISATUKIJUN ")
                        sBuilder.AppendLine(",SYUSATUKIJUN ")
                        sBuilder.AppendLine(",'0','0' ")
                        sBuilder.AppendLine(",'0'  FROM M_KADOHOSYU_SET") ' WITH(HOLDLOCK) ")
                    Else '万分比データ
                        sBuilder.AppendLine(" SELECT MODEL_CODE")
                        sBuilder.AppendLine(",DATA_SYUBETU")
                        sBuilder.AppendLine(",KOMOKU_NAME ")
                        sBuilder.AppendLine(",'0','0' ")
                        sBuilder.AppendLine(",KAISATUKIJUN ")
                        sBuilder.AppendLine(",SYUSATUKIJUN ")
                        sBuilder.AppendLine(",'0'  FROM M_KADOHOSYU_SET") ' WITH(HOLDLOCK)")
                    End If

            End Select

            'Where句生成--------------------------
            sSqlWhere = New StringBuilder
            sSqlWhere.AppendLine(" Where 0 = 0 ")

            '機種
            '-------Ver0.1　北陸対応　MOD START-----------
            sSqlWhere.AppendLine(" And (MODEL_CODE = '" & cmbKishu.SelectedValue.ToString.Substring(1, 1) & "')")
            sSqlWhere.AppendLine(" And (GROUP_NO = '" & cmbKishu.SelectedValue.ToString.Substring(0, 1) & "')")
            '-------Ver0.1　北陸対応　MOD END-----------
            '基準値:稼動データ場合
            If (rdoKado.Checked = True) Then
                sSqlWhere.AppendLine(" And (DATA_SYUBETU = 0 OR DATA_SYUBETU = 3)")
            Else '万分比データ
                sSqlWhere.AppendLine(" And (DATA_SYUBETU = 2)")
            End If

            'Where句結合
            sBuilder.AppendLine(sSqlWhere.ToString)
            sBuilder.AppendLine(" Order by KOMOKU_NO")
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
        Dim i As Integer
        shtMain1.Redraw = False
        Try
            If Not (shtMain1.DataSource Is Nothing) Then
                shtMain1.DataSource = Nothing
                shtMain1.MaxRows = 0
            End If

            shtMain1.MaxRows = dt.Rows.Count
            shtMain1.Rows.SetAllRowsHeight(21)
            shtMain1.DataSource = dt
            If LcstMaxColCnt < dt.Columns.Count Then
                For i = LcstMaxColCnt To dt.Columns.Count - 1
                    shtMain1.Columns(i).Hidden = True 'この行をコメントアウトするとSelect結果全ての行が見えます
                Next i
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            shtMain1.Redraw = True
        End Try
    End Sub

    ''' <summary>
    ''' 更新処理のSQL文
    ''' </summary>
    ''' <param name="nRow">一覧対象行</param>
    Private Function LfUpdateSQL(ByVal nRow As Integer) As String
        Dim sSql As StringBuilder = New StringBuilder()
        Dim sSqlWhere As StringBuilder = New StringBuilder()

        Try
            sSql.Append("Update M_KADOHOSYU_SET Set")
            sSql.Append(" UPDATE_DATE = getdate()")
            sSql.Append(",LAST_DATE = getdate()")
            '基準値:稼動データ場合
            If (rdoKado.Checked = True) Then
                sSql.Append(",KAISATUKIJUN = " + shtMain1.Item(3, nRow).Text)
                sSql.Append(",SYUSATUKIJUN = " + shtMain1.Item(4, nRow).Text)
            Else '万分比データ
                sSql.Append(",KAISATUKIJUN = " + shtMain1.Item(5, nRow).Text)
                sSql.Append(",SYUSATUKIJUN = " + shtMain1.Item(6, nRow).Text)
            End If

            '条件式作成
            sSqlWhere.Append(" where MODEL_CODE =" + Utility.SetSglQuot(shtMain1.Item(0, nRow).Text))
            sSqlWhere.Append(" And DATA_SYUBETU = " + shtMain1.Item(1, nRow).Text)
            '-------Ver0.1　北陸対応　ADD START-----------
            sSqlWhere.Append(" And GROUP_NO = " + cmbKishu.SelectedValue.ToString.Substring(0, 1))
            '-------Ver0.1　北陸対応　ADD START-----------
            sSqlWhere.Append(" And KOMOKU_NAME = " + Utility.SetSglQuot(shtMain1.Item(2, nRow).Text))

            '条件文付加
            sSql.AppendLine(sSqlWhere.ToString)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException
            Return ""
        End Try
        Return sSql.ToString
    End Function

    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 5
        Dim cXlsSheetName As String = ""
        Try

            Dim LcstPrntCol() As Integer = Nothing

            '基準値:稼動データ場合
            '-------Ver0.1　北陸対応　MOD START-----------
            If cmbKishu.SelectedValue.ToString.Substring(1, 1) = MachinTypeMod Then
                cXlsSheetName = Config.KadoPrintSetList(cmbKishu.SelectedIndex).ToString.Substring(0, _
                                Config.KadoPrintSetList(cmbKishu.SelectedIndex).ToString.Length - 4)
                '-------Ver0.1　北陸対応　MOD END-----------
                If (rdoKado.Checked = True) Then
                    LcstPrntCol = {2, 3, 4}
                ElseIf rdoManbunhi.Checked = True Then  '万分比データ
                    LcstPrntCol = {2, 5, 6}
                End If
                '-------Ver0.1　北陸対応　MOD START-----------
            ElseIf cmbKishu.SelectedValue.ToString.Substring(1, 1) = MachinTypeWin Then '窓口処理機
                '-------Ver0.1　北陸対応　MOD END-----------
                cXlsSheetName = LcstXlsSheetWinName
                If (rdoKado.Checked = True) Then
                    LcstPrntCol = {2, 3}
                End If
            End If


            With XlsReport1
                Log.Info("Start printing about [" & sPath & "].")
                ' 帳票ファイル名称を取得
                .FileName = sPath
                .ExcelMode = True
                ' 帳票の出力処理を開始を宣言
                .Report.Start()
                .Report.File()
                '帳票ファイルシート名称を取得します。
                .Page.Start(cXlsSheetName, "1-9999")

                ' 見出し部セルへ見出しデータ出力
                .Cell("B1").Value = LcstXlsSheetTitle
                '-------Ver0.1　北陸対応　MOD START-----------
                If cmbKishu.SelectedValue.ToString.Substring(1, 1) = MachinTypeMod Then
                    .Cell("E1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                    .Cell("E2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                ElseIf cmbKishu.SelectedValue.ToString.Substring(1, 1) = MachinTypeWin Then
                    .Cell("D1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                    .Cell("D2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                End If
                '-------Ver0.1　北陸対応　MOD END-----------

                .Cell("B3").Value = Replace(Replace(Replace(lblLastDate.Text, "年", "/"), "月", "/"), "日 ", " ")

                ' 配信対象のデータ数を取得します
                nRecCnt = shtMain1.MaxRows

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    .Pos(1, y + nStartRow).Value = shtMain1.Item(LcstPrntCol(0), y).Text
                    For x As Integer = 1 To LcstPrntCol.Length - 1
                        If rdoKado.Checked = True Then
                            .Pos(x + 1, y + nStartRow).Attr.Format = "##,##0"
                            .Pos(x + 1, y + nStartRow).Value = Double.Parse(shtMain1.Item(LcstPrntCol(x), y).Text)
                        ElseIf rdoManbunhi.Checked = True Then
                            .Pos(x + 1, y + nStartRow).Attr.Format = "###,##0.000"
                            .Pos(x + 1, y + nStartRow).Value = Double.Parse(shtMain1.Item(LcstPrntCol(x), y).Text)
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
    ''' 機種名称の内容の設定
    ''' </summary>
    Private Sub SetCombox()
        Dim drw As DataRow
        Try
            Dim dt As DataTable = New DataTable()
            dt.Columns.Add("CODE")
            dt.Columns.Add("NAME")
            '-------Ver0.1　北陸対応　ADD START-----------
            Dim i As Integer
            For i = 0 To Config.SysKadoDataModelCode.Count - 1
                drw = dt.NewRow()
                drw.Item(0) = Config.SysKadoDataModelCode(i).ToString.Substring(0, 1) _
                 & Config.SysKadoDataModelCode(i).ToString.Substring(2, 1) _
              : drw.Item(1) = Config.SysKadoDataModelCode(i).ToString.Substring(4)
                dt.Rows.InsertAt(drw, i)
            Next
            '-------Ver0.1　北陸対応　ADD START-----------
            cmbKishu.DataSource = dt
            '表示メンバーの設定
            cmbKishu.DisplayMember = dt.Columns(1).ColumnName
            'バリューメンバーの設定
            cmbKishu.ValueMember = dt.Columns(0).ColumnName

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            drw = Nothing
        End Try
    End Sub

#End Region

End Class
