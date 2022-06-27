' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/06/01  (NES)河脇  北陸対応：グループ対応に伴う登録更新チェックの変更
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon   '定数値のみ使用
Imports JR.ExOpmg.ClientApp.FMTStructure
Imports GrapeCity.Win
Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' 【運管設定管理　画面クラス】
''' </summary>
Public Class FrmSysUnKanSetMng
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
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnReader As System.Windows.Forms.Button
    Friend WithEvents pnlEki As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmbEki As System.Windows.Forms.Label
    Friend WithEvents dtpYmdTo As System.Windows.Forms.Label
    Friend WithEvents dtpHmFrom As System.Windows.Forms.Label
    Friend WithEvents dtpYmdFrom As System.Windows.Forms.Label
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents labEki As System.Windows.Forms.Label

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSysUnKanSetMng))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnReader = New System.Windows.Forms.Button()
        Me.pnlEki = New System.Windows.Forms.Panel()
        Me.dtpYmdTo = New System.Windows.Forms.Label()
        Me.dtpHmFrom = New System.Windows.Forms.Label()
        Me.dtpYmdFrom = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmbEki = New System.Windows.Forms.Label()
        Me.labEki = New System.Windows.Forms.Label()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.pnlBodyBase.SuspendLayout()
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
        Me.pnlBodyBase.Controls.Add(Me.pnlEki)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnReader)
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 87)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/04/03(水)  21:13"
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
        Me.btnPrint.Location = New System.Drawing.Point(744, 162)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 2
        Me.btnPrint.Text = "登　録"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(744, 265)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 3
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnReader
        '
        Me.btnReader.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReader.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReader.Location = New System.Drawing.Point(744, 63)
        Me.btnReader.Name = "btnReader"
        Me.btnReader.Size = New System.Drawing.Size(128, 40)
        Me.btnReader.TabIndex = 1
        Me.btnReader.Text = "読　込"
        Me.btnReader.UseVisualStyleBackColor = False
        '
        'pnlEki
        '
        Me.pnlEki.Controls.Add(Me.dtpYmdTo)
        Me.pnlEki.Controls.Add(Me.dtpHmFrom)
        Me.pnlEki.Controls.Add(Me.dtpYmdFrom)
        Me.pnlEki.Controls.Add(Me.Label1)
        Me.pnlEki.Controls.Add(Me.Label3)
        Me.pnlEki.Controls.Add(Me.Label2)
        Me.pnlEki.Controls.Add(Me.cmbEki)
        Me.pnlEki.Controls.Add(Me.labEki)
        Me.pnlEki.Location = New System.Drawing.Point(13, 6)
        Me.pnlEki.Name = "pnlEki"
        Me.pnlEki.Size = New System.Drawing.Size(646, 312)
        Me.pnlEki.TabIndex = 1
        '
        'dtpYmdTo
        '
        Me.dtpYmdTo.Location = New System.Drawing.Point(210, 233)
        Me.dtpYmdTo.Name = "dtpYmdTo"
        Me.dtpYmdTo.Size = New System.Drawing.Size(147, 18)
        Me.dtpYmdTo.TabIndex = 4
        Me.dtpYmdTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dtpHmFrom
        '
        Me.dtpHmFrom.Location = New System.Drawing.Point(479, 155)
        Me.dtpHmFrom.Name = "dtpHmFrom"
        Me.dtpHmFrom.Size = New System.Drawing.Size(145, 18)
        Me.dtpHmFrom.TabIndex = 4
        Me.dtpHmFrom.Text = "YYYY/MM/DD hh:mm:ss"
        Me.dtpHmFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dtpYmdFrom
        '
        Me.dtpYmdFrom.Location = New System.Drawing.Point(210, 156)
        Me.dtpYmdFrom.Name = "dtpYmdFrom"
        Me.dtpYmdFrom.Size = New System.Drawing.Size(150, 18)
        Me.dtpYmdFrom.TabIndex = 4
        Me.dtpYmdFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "labYmdFrom"
        Me.Label1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(60, 155)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(144, 21)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "前回登録バージョン"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(60, 232)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(144, 21)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "今回登録バージョン"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(386, 154)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(85, 21)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "登録日時："
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbEki
        '
        Me.cmbEki.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.cmbEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.Location = New System.Drawing.Point(196, 82)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(149, 21)
        Me.cmbEki.TabIndex = 0
        Me.cmbEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'labEki
        '
        Me.labEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.labEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.labEki.Location = New System.Drawing.Point(60, 82)
        Me.labEki.Name = "labEki"
        Me.labEki.Size = New System.Drawing.Size(85, 21)
        Me.labEki.TabIndex = 0
        Me.labEki.Text = "データ名称"
        Me.labEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'FrmSysUnKanSetMng
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1013, 741)
        Me.Name = "FrmSysUnKanSetMng"
        Me.Text = "運用端末 Ver.1.00"
        Me.pnlBodyBase.ResumeLayout(False)
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
    ''' CSVデータ
    ''' </summary>
    Private infoLst As New List(Of String())

    ''' <summary>
    ''' 定義情報
    ''' </summary>
    ''' <remarks></remarks>
    Private infoObj() As FMTInfo = Nothing

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly FormTitle As String = "運管設定管理"

    ''' <summary>
    ''' データ名称
    ''' </summary>
    Private ReadOnly DataName As String = "機器構成マスタデータ"

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

    ''' <summary>
    ''' 登録機器構成マスタ失敗
    ''' </summary>
    Private Const LcstPrintMachineError As String = "登録処理に失敗しました。設定ファイルの内容を確認してください。"

    ''' <summary>
    ''' 登録マスタバージョン（機器）失敗
    ''' </summary>
    Private Const LcstPrintVersionError As String = "登録処理に失敗しました。"

    ''' <summary>
    ''' 読込失敗
    ''' </summary>
    Private Const LcstReaderError As String = "読込処理に失敗しました。"

    ''' <summary>
    ''' ファイル名エラー
    ''' </summary>
    Private Const LcstCSVFileNameError As String = "読込対象ファイルが不正です。"

    ''' <summary>
    ''' ファイルエラー
    ''' </summary>
    Private Const LcstCSVFileCheckError As String = "読込対象ファイルが存在しません。"

    ''' <summary>
    ''' 項目数チェック
    ''' </summary>
    Private Const LcstItemCountCheck As String = "{0}行目のデータ項目数が不正です。"

    ''' <summary>
    ''' 必須チェック
    ''' </summary>
    Private Const LcstMustCheck As String = "{0}行目のデータ項目「{1}」が必須です。"

    ''' <summary>
    ''' 属性チェック
    ''' </summary>
    Private Const LcstAttributeCheck As String = "{0}行目のデータ項目「{1}」の属性が不正です。"

    ''' <summary>
    ''' 桁数チェック
    ''' </summary>
    Private Const LcstTrussNumber As String = "{0}行目のデータ項目「{1}」の桁数が超過です。"

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

            '--画面タイトル
            lblTitle.Text = FormTitle
            '--常時活性化項目設定
            btnPrint.Enabled = False
            btnReturn.Enabled = True        '終了ボタン
            '値初期化
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

            LfGetInitFrm()

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
                AlertBox.Show(Lexis.FormProcAbnormalEnd)       '開始異常メッセージ
            End If
            LbEventStop = False 'イベント発生ＯＮ
        End Try
        Return bRtn
    End Function

#End Region

#Region "イベント"

    'Private Sub FrmMntDispAbnormalData_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
    'Handles MyBase.Load
    'プロシージャ名を『FrmMntDispAbnormalData_Load → FrmSysUnKanSetMng_Load』に変更
    ''' <summary>
    ''' フォームロード
    ''' </summary>
    Private Sub FrmSysUnKanSetMng_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
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
            cmbEki.Text = DataName  'データ名称
            LbEventStop = False             'イベント発生ＯＮ
            labEki.Select() '初期フォーカス
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
    ''' 登録
    ''' </summary>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnPrint.Click
        If LbEventStop Then Exit Sub
        LbEventStop = True

        Dim dbCtl As DatabaseTalker = New DatabaseTalker()
        Dim dt As DataTable = New DataTable()
        Dim sBuilder As StringBuilder
        Dim wBuilder As StringBuilder
        Dim vBuilder As StringBuilder
        '登録日時
        Dim sCurTime As String
        Dim loginiD As String = Config.MachineName                                      '設定ファイルの端末ＩＤ
        Dim dbError As Boolean = False                  'db異常発生ＯＮ
        Dim i As Integer = 0
        Dim j As Integer = 0
        LfWaitCursor()
        Try
            'ボタン押下ログ
            LogOperation(sender, e)
            dbCtl.ConnectOpen()          'クネクションを取得する。

            dbCtl.TransactionBegin()  'トランザクションを開始する。
            '登録日時の作成
            sCurTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
            For i = 0 To infoLst.Count - 1

                Try
                    '件数取得チェック
                    sBuilder = New StringBuilder
                    sBuilder.AppendLine("select count(1) ")
                    sBuilder.AppendLine(String.Format(" FROM M_MACHINE where [SETTING_START_DATE]={0}", Utility.SetSglQuot(infoLst.Item(i)(0).ToString)))
                    '-------Ver0.1　北陸対応：グループ対応に伴う登録更新チェックの変更　ADD START-----------
                    sBuilder.AppendLine(String.Format(" AND [BRANCH_OFFICE_CODE]={0}", Utility.SetSglQuot(infoLst.Item(i)(2).ToString)))
                    '-------Ver0.1　北陸対応：グループ対応に伴う登録更新チェックの変更　END START-----------
                    sBuilder.AppendLine(String.Format(" AND [RAIL_SECTION_CODE]={0}", Utility.SetSglQuot(infoLst.Item(i)(7).ToString)))
                    sBuilder.AppendLine(String.Format(" AND [STATION_ORDER_CODE] ={0}", Utility.SetSglQuot(infoLst.Item(i)(8).ToString)))
                    sBuilder.AppendLine(String.Format(" AND [CORNER_CODE]={0} ", Utility.SetSglQuot(infoLst.Item(i)(10).ToString)))
                    sBuilder.AppendLine(String.Format(" AND [MODEL_CODE]={0} ", Utility.SetSglQuot(infoLst.Item(i)(12).ToString)))
                    sBuilder.AppendLine(String.Format(" AND [UNIT_NO]={0}", infoLst.Item(i)(13)))
                    'データ取得処理
                    FrmBase.BaseSqlDataTableFill(sBuilder.ToString, dt)
                    sBuilder = New StringBuilder
                    wBuilder = New StringBuilder
                    vBuilder = New StringBuilder
                    wBuilder.AppendLine("Where 0=0")
                    vBuilder.AppendLine("values(")
                    '機器構成マスタの更新
                    If CInt(dt.Rows(0)(0)) > 0 Then
                        sBuilder.AppendLine(String.Format(" update M_MACHINE set UPDATE_DATE={0},", Utility.SetSglQuot(sCurTime)))
                        sBuilder.AppendLine(String.Format(" UPDATE_USER_ID={0},", Utility.SetSglQuot(GlobalVariables.UserId)))
                        sBuilder.AppendLine(String.Format(" UPDATE_MACHINE_ID={0}", Utility.SetSglQuot(loginiD)))
                        For j = 0 To infoObj.Length - 1
                            If infoObj(j).FIELD_NAME = "SETTING_START_DATE" OrElse
                               infoObj(j).FIELD_NAME = "RAIL_SECTION_CODE" OrElse
                               infoObj(j).FIELD_NAME = "STATION_ORDER_CODE" OrElse
                               infoObj(j).FIELD_NAME = "CORNER_CODE" OrElse
                               infoObj(j).FIELD_NAME = "MODEL_CODE" Then
                                wBuilder.AppendLine(String.Format("AND {0}={1}", infoObj(j).FIELD_NAME, _
                                                                  Utility.SetSglQuot(infoLst.Item(i)(j).ToString)))
                            ElseIf infoObj(j).FIELD_NAME = "UNIT_NO" Then
                                sBuilder.AppendLine(String.Format(",{0}={1}", infoObj(j).FIELD_NAME, _
                                                                 infoLst.Item(i)(j)))
                                wBuilder.AppendLine(String.Format("AND {0}={1}", infoObj(j).FIELD_NAME, _
                                                                  infoLst.Item(i)(j)))
                            Else
                                sBuilder.AppendLine(String.Format(",{0}={1}", infoObj(j).FIELD_NAME, _
                                                                  Utility.SetSglQuot(infoLst.Item(i)(j).ToString)))
                            End If
                        Next
                        sBuilder.AppendLine(wBuilder.ToString)
                    Else  '機器構成マスタの登録
                        sBuilder.AppendLine(" insert into M_MACHINE (INSERT_DATE ,INSERT_USER_ID,INSERT_MACHINE_ID")
                        sBuilder.AppendLine(" ,UPDATE_DATE,UPDATE_USER_ID,UPDATE_MACHINE_ID")
                        vBuilder.AppendLine(String.Format("{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot(GlobalVariables.UserId)))
                        vBuilder.AppendLine(String.Format("{0}", Utility.SetSglQuot(loginiD)))
                        vBuilder.AppendLine(String.Format(",{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot(GlobalVariables.UserId)))
                        vBuilder.AppendLine(String.Format("{0}", Utility.SetSglQuot(loginiD)))
                        For j = 0 To infoObj.Length - 1
                            If infoObj(j).FIELD_NAME = "UNIT_NO" OrElse
                               infoObj(j).FIELD_NAME = "Y_AREA_CODE" OrElse
                               infoObj(j).FIELD_NAME = "G_AREA_CODE" OrElse
                               infoObj(j).FIELD_NAME = "W_AREA_CODE" Then
                                vBuilder.AppendLine(String.Format(",{0}", infoLst.Item(i)(j).ToString))
                            Else
                                vBuilder.AppendLine(String.Format(",{0}", Utility.SetSglQuot(infoLst.Item(i)(j).ToString)))
                            End If
                            sBuilder.AppendLine(String.Format(",{0}", infoObj(j).FIELD_NAME))
                        Next
                        vBuilder.Append(")")
                        sBuilder.Append(")")
                        sBuilder.AppendLine(vBuilder.ToString)
                    End If
                    'データ処理
                    dbCtl.ExecuteSQLToWrite(sBuilder.ToString)
                Catch ex As Exception
                    dbError = True
                    infoLst = Nothing
                    Log.Fatal(LcstPrintMachineError)
                    AlertBox.Show(Lexis.MachineMasterInsertFailed)
                    btnPrint.Enabled = False
                    btnReturn.Select()
                    'TODO: トランザクションのロールバックはdbCtl.TransactionRollBack()で行わないと、
                    '例外が漏れ出す。
                    Exit Sub
                End Try
            Next
            'マスタバージョン（機器）「M_MACHINE_DATA_VER」の登録
            If dbError <> True Then
                sBuilder = New StringBuilder
                sBuilder.AppendLine("select count(1) ")
                sBuilder.AppendLine(String.Format(" FROM M_MACHINE_DATA_VER where [VERSION]={0}", Utility.SetSglQuot(dtpYmdTo.Text)))
                'データ取得処理
                FrmBase.BaseSqlDataTableFill(sBuilder.ToString, dt)
                If CInt(dt.Rows(0)(0)) > 0 Then
                    sBuilder = New StringBuilder
                    sBuilder.AppendLine(String.Format("UPDATE M_MACHINE_DATA_VER SET UPDATE_DATE={0},", Utility.SetSglQuot(sCurTime)))
                    sBuilder.AppendLine(String.Format(" UPDATE_USER_ID={0},", Utility.SetSglQuot(GlobalVariables.UserId)))
                    sBuilder.AppendLine(String.Format(" UPDATE_MACHINE_ID={0}", Utility.SetSglQuot(loginiD)))
                    sBuilder.AppendLine(String.Format("WHERE VERSION = {0}", Utility.SetSglQuot(dtpYmdTo.Text)))
                Else
                    sBuilder = New StringBuilder
                    sBuilder.AppendLine("insert into M_MACHINE_DATA_VER(INSERT_DATE,INSERT_USER_ID,INSERT_MACHINE_ID")
                    sBuilder.AppendLine(" ,UPDATE_DATE,UPDATE_USER_ID,UPDATE_MACHINE_ID")
                    sBuilder.AppendLine(", VERSION)")
                    sBuilder.AppendLine(String.Format("values({0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot((GlobalVariables.UserId))))
                    sBuilder.AppendLine(String.Format("{0},", Utility.SetSglQuot(loginiD)))
                    sBuilder.AppendLine(String.Format("{0},{1},", Utility.SetSglQuot(sCurTime), Utility.SetSglQuot((GlobalVariables.UserId))))
                    sBuilder.AppendLine(String.Format("{0},", Utility.SetSglQuot(loginiD)))
                    sBuilder.AppendLine(String.Format("{0})", Utility.SetSglQuot(dtpYmdTo.Text)))
                End If
                '更新、挿入のためのSQLを実行する。
                dbCtl.ExecuteSQLToWrite(sBuilder.ToString)
            End If
            'トランザクションをコミットする
            dbCtl.TransactionCommit()
            AlertBox.Show(Lexis.InsertCompleted)
        Catch ex As DatabaseException
            dbCtl.TransactionRollBack()
            dbError = True
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnReturn.Select()
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            dbError = True
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.MachineMasterInsertFailed2)
            btnReturn.Select()
        Finally
            dbCtl.ConnectClose()
            infoLst = Nothing
            LbEventStop = False
            dbError = False
            btnPrint.Enabled = False
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 読込
    ''' </summary>
    Private Sub btnReader_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReader.Click
        If LbEventStop Then Exit Sub
        LbEventStop = True      'イベント発生ＯＦＦ
        Dim fName As String = "DATA_MachineConfig_XXX.csv"
        Dim filePath As String = ""
        Dim fileNo As String
        Dim oldFPath As String
        Dim newFPath As String
        infoLst = New List(Of String())
        Dim filenumber As Int32
        Dim strRead() As String
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim iFlag As Integer = 0

        LfWaitCursor()
        Try
            'ボタン押下’
            LogOperation(sender, e)
            OpenFileDialog1.Multiselect = False
            'ファイルを選択
            If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                '読込対象ファイル名チェック
                oldFPath = OpenFileDialog1.FileName
                fileNo = oldFPath.Substring(oldFPath.LastIndexOf("_") + 1, 3)
                fName = fName.Replace("XXX", fileNo)
                filePath = oldFPath.Substring(0, oldFPath.LastIndexOf("\") + 1)
                newFPath = Path.Combine(filePath, fName)

                If oldFPath <> newFPath Then
                    Log.Error(LcstCSVFileNameError)
                    AlertBox.Show(Lexis.TheFileNameIsUnsuitableForMachineMaster)
                    btnReturn.Select()
                    Exit Sub
                End If
                ' 読込対象ファイルチェック
                If File.Exists(newFPath) = False Then
                    Log.Error(LcstCSVFileCheckError)
                    AlertBox.Show(Lexis.MachineMasterFileNotFound)
                    btnReturn.Select()
                    Exit Sub
                End If
                dtpYmdTo.Text = fileNo
            Else
                Exit Sub
            End If
            'CSVフォーマット定義情報を取得する
            If GetDefineInfo(Config.MachineMasterFormatFilePath, "FMT_MachineConfig", infoObj) = False Then
                btnReturn.Select()
                Exit Sub
            End If
            'CSVファイルより、データを取得する。
            filenumber = CShort(FreeFile())
            FileOpen(filenumber, filePath + fName, OpenMode.Binary)
            Do While Not EOF(1)

                Dim str As String = ""","""
                strRead = Nothing
                strRead = Split(LineInput(1), str)
                If strRead(0).ToString = "[FMT_MachineConfig]" Then
                    Continue Do
                End If
                i += 1
                If strRead.Length <> infoObj.Length Then
                    Log.Info(String.Format(LcstItemCountCheck, i))
                    iFlag = 1
                    Exit Do
                End If
                strRead(0) = strRead(0).Remove(0, 1)
                strRead(22) = strRead(22).Remove(strRead(22).Length - 1, 1)

                'データチェック
                For j = 0 To infoObj.Length - 1
                    If LfCheck(strRead(j), i, infoObj(j)) = False Then
                        iFlag = 1
                        Exit Do
                    End If
                Next
                infoLst.Add(strRead)
            Loop
            FileClose(1)
            'ボタン活性化
            If infoLst.Count > 0 And iFlag <> 1 Then
                btnPrint.Enabled = True
            Else
                AlertBox.Show(Lexis.MachineMasterFileReadFailed) 'チェックエラーが発生した場合
                btnPrint.Enabled = False
            End If
            LfGetInitFrm()
        Catch ex As Exception
            FileClose(1)
            infoLst = Nothing
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.MachineMasterFileReadFailed) '検索失敗メッセージ
            btnReturn.Select()
        Finally
            btnPrint.Select()
            LbEventStop = False
            LfWaitCursor(False)
        End Try
    End Sub

#End Region
#Region "メソッド（Private）"

    ''' <summary>
    ''' 前回登録
    ''' </summary>
    Private Sub LfGetInitFrm()
        Dim sSql As String = ""
        Dim dtData As New DataTable
        Dim sBuilder As New StringBuilder
        Try
            sBuilder.AppendLine("SELECT top(1) VERSION ,")
            sBuilder.AppendLine("CONVERT(VARCHAR(100), INSERT_DATE, 111)+ ' '+CONVERT(VARCHAR(100), INSERT_DATE, 24) AS INSERT_DATE")
            sBuilder.AppendLine(" FROM M_MACHINE_DATA_VER order by UPDATE_DATE DESC ")
            sSql = sBuilder.ToString()
            BaseSqlDataTableFill(sSql, dtData)
            If dtData.Rows.Count > 0 Then
                If dtData.Rows(0).Item(0).ToString <> Nothing Then
                    dtpYmdFrom.Text = dtData.Rows(0).Item(0).ToString   '前回登録バージョン
                End If
                If dtData.Rows(0).Item(1).ToString <> Nothing Then
                    dtpHmFrom.Text = dtData.Rows(0).Item(1).ToString()
                End If
            Else
                dtpYmdFrom.Text = ""   '前回登録バージョン
                dtpHmFrom.Text = ""    '登録日時
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try
    End Sub

    ''' <summary>
    ''' [検索ボタン活性化]
    ''' </summary>
    Private Sub LfSearchTrue()
        Dim bEnabled As Boolean
        If bEnabled Then
        End If
        If bEnabled Then
            If btnReader.Enabled = False Then btnReader.Enabled = True
        Else
            If btnReader.Enabled = True Then btnReader.Enabled = False
        End If
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
                AlertBox.Show(Lexis.MachineMasterFormatFileNotFound)
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
    ''' データチェック
    ''' </summary>
    ''' <param name="CodeName">フィールド名</param>
    ''' <param name="iRow">桁数</param>
    ''' <param name="AarrayCode">定義情報</param>
    Private Function LfCheck(ByRef CodeName As String, ByVal iRow As Integer, ByVal AarrayCode As FMTInfo) As Boolean
        Dim Encode As Encoding = Encoding.GetEncoding("Shift_JIS")
        If AarrayCode.MUST = True Then      '必須チェック
            If CodeName.Length = 0 Then
                Log.Info(String.Format(LcstMustCheck, iRow, AarrayCode.KOMOKU_NAME))
                Return False
            ElseIf AarrayCode.FIELD_FORMAT = "Integer" Then 'フィールド形式:Integer
                If OPMGUtility.checkNumber(CodeName) = False Then         'フィールド形式チェック
                    Log.Info(String.Format(LcstAttributeCheck, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                ElseIf CDec(CodeName) > Integer.MaxValue Then      'フィールド桁数チェック
                    Log.Info(String.Format(LcstTrussNumber, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                End If
            ElseIf AarrayCode.FIELD_FORMAT = "String" Then  'フィールド形式:String
                If Encode.GetByteCount(CodeName) > AarrayCode.DATA_LEN Then 'フィールド桁数チェック
                    Log.Info(String.Format(LcstTrussNumber, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                End If
            End If
        Else
            If AarrayCode.FIELD_FORMAT = "String" Then        'フィールド形式:String
                If Encode.GetByteCount(CodeName) > AarrayCode.DATA_LEN Then 'フィールド桁数チェック
                    Log.Info(String.Format(LcstTrussNumber, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                End If
            ElseIf AarrayCode.FIELD_FORMAT = "Integer" Then     'フィールド形式:Integer
                If OPMGUtility.checkNumber(CodeName) = False Then             'フィールド形式チェック
                    Log.Info(String.Format(LcstAttributeCheck, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                ElseIf CDec(CodeName) > Integer.MaxValue Then              'フィールド桁数チェック
                    Log.Info(String.Format(LcstTrussNumber, iRow, AarrayCode.KOMOKU_NAME))
                    Return False
                End If
            End If
        End If
        Return True
    End Function

#End Region
End Class
Public Structure FMTStructure
#Region "宣言領域（Public）"
    Public Structure FMTInfo
        Dim KOMOKU_NAME As String                   '項目名称
        Dim IN_TURN As Integer                      '順番
        Dim MUST As Boolean                         '必須
        Dim FIELD_FORMAT As String                  'フィールド形式
        Dim DATA_LEN As Integer                      'データ長
        Dim FIELD_NAME As String                    'フィールド名
    End Structure
#End Region
End Structure