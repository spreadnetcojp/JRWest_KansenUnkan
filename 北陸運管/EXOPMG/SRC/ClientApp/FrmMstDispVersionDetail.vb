' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess
Imports GrapeCity.Win
Imports System.IO

''' <summary>マスタバージョン詳細表示</summary>
''' <remarks>
''' マスタバージョン詳細表示
''' </remarks>
Public Class FrmMstDispVersionDetail
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
    Friend WithEvents WorkBook1 As GrapeCity.Win.ElTabelleSheet.WorkBook
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents cmbMst As System.Windows.Forms.ComboBox
    Friend WithEvents lblMst As System.Windows.Forms.Label
    Friend WithEvents cmbUnit As System.Windows.Forms.ComboBox
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents cmbCorner As System.Windows.Forms.ComboBox
    Friend WithEvents cmbEki As System.Windows.Forms.ComboBox
    Friend WithEvents lblUnit As System.Windows.Forms.Label
    Friend WithEvents lblModel As System.Windows.Forms.Label
    Friend WithEvents lblMado As System.Windows.Forms.Label
    Friend WithEvents lblEki As System.Windows.Forms.Label
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnKensaku As System.Windows.Forms.Button
    Friend WithEvents lblState As System.Windows.Forms.Label
    Friend WithEvents cmbState As System.Windows.Forms.ComboBox
    Friend WithEvents XlsReport1 As AdvanceSoftware.VBReport7.Xls.XlsReport
    Friend WithEvents shtVerDetail As GrapeCity.Win.ElTabelleSheet.Sheet
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMstDispVersionDetail))
        Me.WorkBook1 = New GrapeCity.Win.ElTabelleSheet.WorkBook()
        Me.shtVerDetail = New GrapeCity.Win.ElTabelleSheet.Sheet()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.cmbMst = New System.Windows.Forms.ComboBox()
        Me.lblMst = New System.Windows.Forms.Label()
        Me.cmbUnit = New System.Windows.Forms.ComboBox()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.cmbCorner = New System.Windows.Forms.ComboBox()
        Me.cmbEki = New System.Windows.Forms.ComboBox()
        Me.lblUnit = New System.Windows.Forms.Label()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.lblMado = New System.Windows.Forms.Label()
        Me.lblEki = New System.Windows.Forms.Label()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnKensaku = New System.Windows.Forms.Button()
        Me.lblState = New System.Windows.Forms.Label()
        Me.cmbState = New System.Windows.Forms.ComboBox()
        Me.XlsReport1 = New AdvanceSoftware.VBReport7.Xls.XlsReport(Me.components)
        Me.pnlBodyBase.SuspendLayout()
        Me.WorkBook1.SuspendLayout()
        CType(Me.shtVerDetail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.cmbState)
        Me.pnlBodyBase.Controls.Add(Me.lblState)
        Me.pnlBodyBase.Controls.Add(Me.WorkBook1)
        Me.pnlBodyBase.Controls.Add(Me.btnPrint)
        Me.pnlBodyBase.Controls.Add(Me.cmbMst)
        Me.pnlBodyBase.Controls.Add(Me.lblMst)
        Me.pnlBodyBase.Controls.Add(Me.cmbUnit)
        Me.pnlBodyBase.Controls.Add(Me.cmbModel)
        Me.pnlBodyBase.Controls.Add(Me.cmbCorner)
        Me.pnlBodyBase.Controls.Add(Me.cmbEki)
        Me.pnlBodyBase.Controls.Add(Me.lblUnit)
        Me.pnlBodyBase.Controls.Add(Me.lblModel)
        Me.pnlBodyBase.Controls.Add(Me.lblMado)
        Me.pnlBodyBase.Controls.Add(Me.lblEki)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnKensaku)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/08/02(金)  16:24"
        '
        'WorkBook1
        '
        Me.WorkBook1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.WorkBook1.Controls.Add(Me.shtVerDetail)
        Me.WorkBook1.Location = New System.Drawing.Point(36, 84)
        Me.WorkBook1.Name = "WorkBook1"
        Me.WorkBook1.ProcessTabKey = False
        Me.WorkBook1.ShowTabs = False
        Me.WorkBook1.Size = New System.Drawing.Size(880, 481)
        Me.WorkBook1.TabFont = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.WorkBook1.TabIndex = 0
        '
        'shtVerDetail
        '
        Me.shtVerDetail.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.shtVerDetail.Data = CType(resources.GetObject("shtVerDetail.Data"), GrapeCity.Win.ElTabelleSheet.SheetData)
        Me.shtVerDetail.Location = New System.Drawing.Point(1, 1)
        Me.shtVerDetail.Name = "shtVerDetail"
        Me.shtVerDetail.Size = New System.Drawing.Size(861, 462)
        Me.shtVerDetail.TabIndex = 0
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(704, 584)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(128, 40)
        Me.btnPrint.TabIndex = 8
        Me.btnPrint.Tag = "8"
        Me.btnPrint.Text = "出　力"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'cmbMst
        '
        Me.cmbMst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMst.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbMst.ItemHeight = 13
        Me.cmbMst.Location = New System.Drawing.Point(265, 48)
        Me.cmbMst.Name = "cmbMst"
        Me.cmbMst.Size = New System.Drawing.Size(220, 21)
        Me.cmbMst.TabIndex = 5
        Me.cmbMst.Tag = "5"
        '
        'lblMst
        '
        Me.lblMst.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMst.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMst.Location = New System.Drawing.Point(184, 50)
        Me.lblMst.Name = "lblMst"
        Me.lblMst.Size = New System.Drawing.Size(80, 18)
        Me.lblMst.TabIndex = 93
        Me.lblMst.Text = "マスタ名称"
        Me.lblMst.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbUnit
        '
        Me.cmbUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbUnit.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbUnit.ItemHeight = 13
        Me.cmbUnit.Items.AddRange(New Object() {""})
        Me.cmbUnit.Location = New System.Drawing.Point(783, 16)
        Me.cmbUnit.Name = "cmbUnit"
        Me.cmbUnit.Size = New System.Drawing.Size(70, 21)
        Me.cmbUnit.TabIndex = 4
        Me.cmbUnit.Tag = "4"
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.ItemHeight = 13
        Me.cmbModel.Location = New System.Drawing.Point(73, 16)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(126, 21)
        Me.cmbModel.TabIndex = 1
        Me.cmbModel.Tag = "1"
        '
        'cmbCorner
        '
        Me.cmbCorner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCorner.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbCorner.ItemHeight = 13
        Me.cmbCorner.Location = New System.Drawing.Point(554, 16)
        Me.cmbCorner.Name = "cmbCorner"
        Me.cmbCorner.Size = New System.Drawing.Size(162, 21)
        Me.cmbCorner.TabIndex = 3
        Me.cmbCorner.Tag = "3"
        '
        'cmbEki
        '
        Me.cmbEki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbEki.ItemHeight = 13
        Me.cmbEki.Location = New System.Drawing.Point(265, 16)
        Me.cmbEki.Name = "cmbEki"
        Me.cmbEki.Size = New System.Drawing.Size(162, 21)
        Me.cmbEki.TabIndex = 2
        Me.cmbEki.Tag = "2"
        '
        'lblUnit
        '
        Me.lblUnit.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblUnit.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUnit.Location = New System.Drawing.Point(745, 18)
        Me.lblUnit.Name = "lblUnit"
        Me.lblUnit.Size = New System.Drawing.Size(44, 18)
        Me.lblUnit.TabIndex = 92
        Me.lblUnit.Text = "号機"
        Me.lblUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(34, 17)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(44, 18)
        Me.lblModel.TabIndex = 91
        Me.lblModel.Text = "機種"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMado
        '
        Me.lblMado.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMado.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMado.Location = New System.Drawing.Point(489, 18)
        Me.lblMado.Name = "lblMado"
        Me.lblMado.Size = New System.Drawing.Size(64, 18)
        Me.lblMado.TabIndex = 90
        Me.lblMado.Text = "コーナー"
        Me.lblMado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblEki
        '
        Me.lblEki.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblEki.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblEki.Location = New System.Drawing.Point(225, 17)
        Me.lblEki.Name = "lblEki"
        Me.lblEki.Size = New System.Drawing.Size(50, 18)
        Me.lblEki.TabIndex = 89
        Me.lblEki.Text = "駅名"
        Me.lblEki.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 9
        Me.btnReturn.Tag = "9"
        Me.btnReturn.Text = "戻　る"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnKensaku
        '
        Me.btnKensaku.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnKensaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKensaku.Location = New System.Drawing.Point(872, 32)
        Me.btnKensaku.Name = "btnKensaku"
        Me.btnKensaku.Size = New System.Drawing.Size(128, 40)
        Me.btnKensaku.TabIndex = 7
        Me.btnKensaku.Tag = "7"
        Me.btnKensaku.Text = "検　索"
        Me.btnKensaku.UseVisualStyleBackColor = False
        '
        'lblState
        '
        Me.lblState.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblState.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblState.Location = New System.Drawing.Point(519, 50)
        Me.lblState.Name = "lblState"
        Me.lblState.Size = New System.Drawing.Size(44, 18)
        Me.lblState.TabIndex = 95
        Me.lblState.Text = "状態"
        Me.lblState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbState
        '
        Me.cmbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbState.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbState.ItemHeight = 13
        Me.cmbState.Location = New System.Drawing.Point(555, 48)
        Me.cmbState.Name = "cmbState"
        Me.cmbState.Size = New System.Drawing.Size(70, 21)
        Me.cmbState.TabIndex = 6
        Me.cmbState.Tag = "6"
        '
        'FrmMstDispVersionDetail
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMstDispVersionDetail"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.WorkBook1.ResumeLayout(False)
        CType(Me.shtVerDetail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "各種宣言領域"

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    ''' 出力用テンプレートファイル名
    ''' </summary>
    Private ReadOnly LcstXlsTemplateName As String = "マスタバージョン情報.xls"

    ''' <summary>
    ''' 出力時用テンプレートシート名
    ''' </summary>
    Private ReadOnly LcstXlsSheetName As String = "マスタバージョン情報"

    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private ReadOnly LcstFormTitle As String = "マスタバージョン詳細表示"

    ''' <summary>
    ''' 帳票出力対象列の割り当て
    ''' （検索した別集札データに対し帳票出力列を定義）
    ''' </summary>
    Private ReadOnly LcstPrntCol() As Integer = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

    ''' <summary>
    ''' 一覧表示最大列
    ''' </summary>
    Private LcstMaxColCnt As Integer

    Private LbInitCallFlg As Boolean = False


    '前の画面から渡された’機種コード’を受け取る
    Private sCmbModel As Integer
    '前の画面から渡された’ 線区コード’を受け取る
    Private sBtnRail As String = ""
    '前の画面から渡された’駅順コード’を受け取る
    Private sBtnStation As String = ""

    Public Property sCmbValue() As Integer
        Get
            Return sCmbModel
        End Get
        Set(ByVal value As Integer)
            sCmbModel = value
        End Set
    End Property

    Public Property sBtnName() As String
        Get
            Return sBtnRail
        End Get
        Set(ByVal value As String)
            sBtnRail = value
        End Set
    End Property

    Public Property sBtnTag() As String
        Get
            Return sBtnStation
        End Get
        Set(ByVal value As String)
            sBtnStation = value
        End Set
    End Property
#End Region

#Region "画面のデータを準備する"
    ''' <summary>画面のデータを準備する</summary>
    ''' <remarks>
    '''データを検索し、画面に表示する
    ''' </remarks>   
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        Dim nEkiIndex As Integer
        LbInitCallFlg = True    '当関数呼出フラグ
        LbEventStop = True      'イベント発生ＯＦＦ

        Try
            Log.Info("Method started.")

            '前の画面から渡された値を受け取るかを判断する
            If String.IsNullOrEmpty(sBtnRail) Or String.IsNullOrEmpty(sBtnStation) Then
                '画面表示処理に失敗しました
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
                Return False
            Else

                '画面タイトル
                lblTitle.Text = LcstFormTitle

                'シート初期化
                shtVerDetail.TransformEditor = False                                     '一覧の列種類毎のチェックを無効にする
                shtVerDetail.ViewMode = ElTabelleSheet.ViewMode.Row                      '行選択モード
                shtVerDetail.MaxRows() = 0                                               '行の初期化
                LcstMaxColCnt = shtVerDetail.MaxColumns()                                '列数を取得
                shtVerDetail.EditType = GrapeCity.Win.ElTabelleSheet.EditType.ReadOnly   'シートを表示モード

                '機種名称を設定する。
                If setCmbModel() = False Then Exit Try
                cmbModel.SelectedIndex = sCmbModel          'デフォルト表示項目

                If setCmbEki(cmbModel.SelectedValue.ToString) = False Then Exit Try
                nEkiIndex = getIndex(CType(cmbEki.DataSource, DataTable), sBtnRail & sBtnStation)
                cmbEki.SelectedIndex = nEkiIndex          'デフォルト表示項目

                If setCmbCorner(cmbModel.SelectedValue.ToString, cmbEki.SelectedValue.ToString) = False Then Exit Try
                cmbCorner.SelectedIndex = 0          'デフォルト表示項目

                If setCmbUnit(cmbModel.SelectedValue.ToString, cmbEki.SelectedValue.ToString, cmbCorner.SelectedValue.ToString) = False Then Exit Try
                cmbUnit.SelectedIndex = 0          'デフォルト表示項目

                If setCmbMst(cmbModel.SelectedValue.ToString) = False Then Exit Try
                cmbMst.SelectedIndex = 0          'デフォルト表示項目

                Call setCmbState()
                Call initElTable()

            End If

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

#Region "フォームロード"

    ''' <summary>フォームロード</summary>
    ''' <remarks>
    '''  画面タイトル、画面背景色（BackColor）を設定し、ELTableを表示する。
    ''' 「駅名」を初期化する
    ''' </remarks>
    Private Sub FrmMstDispVersionDetail_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        LfWaitCursor()
        If LbInitCallFlg = False Then   '初期処理が呼び出されていない場合のみ実施
            If InitFrmData() = False Then   '初期処理
                Me.Close()
                Exit Sub
            End If
        End If

        LfWaitCursor(False)

    End Sub
#End Region



#Region "コンボボックス設定"

    ''' <summary>
    ''' 機種名称コンボボックスを設定する。
    ''' </summary>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理している機種名称の一覧及び「全機種」を設定する。</remarks>
    Private Function setCmbModel() As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New ModelMaster

        Try
            '機種名称コンボボックス用のデータを取得する。
            dt = oMst.SelectTable()
            If dt.Rows.Count = 0 Then
                '機種データ取得失敗
                Return bRtn
            End If
            dt = oMst.SetAll()

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
    ''' 駅名称コンボボックスを設定する。
    ''' </summary>
    ''' <param name="Model">機種コード</param>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理している駅名称の一覧及び「全駅」を設定する。</remarks>
    Private Function setCmbEki(ByVal Model As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New StationMaster
        Dim sModel As String

        Try
            If Model = ClientDaoConstants.TERMINAL_ALL Then
                sModel = "G,Y"
            Else
                sModel = Model
            End If

            '駅名称コンボボックス用のデータを取得する。
            dt = oMst.SelectTable(False, sModel)
            If dt.Rows.Count = 0 Then
                '駅データ取得失敗
                Return bRtn
            End If
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
    ''' コーナー名称コンボボックスを設定する。
    ''' </summary>
    ''' <param name="Model">機種コード</param>
    ''' <param name="Station">駅コード</param>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理しているコーナー名称の一覧及び「全コーナー」を設定する。</remarks>
    Private Function setCmbCorner(ByVal Model As String, ByVal Station As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New CornerMaster
        Dim sModel As String

        Try
            If Station <> ClientDaoConstants.TERMINAL_ALL Then
                If Model = ClientDaoConstants.TERMINAL_ALL Then
                    sModel = "G,Y"
                Else
                    sModel = Model
                End If

                'コーナー名称コンボボックス用のデータを取得する。
                dt = oMst.SelectTable(Station, sModel)
                If dt.Rows.Count = 0 Then
                    'コーナーデータ取得失敗
                    Return bRtn
                End If
            End If
            dt = oMst.SetAll()

            bRtn = BaseSetMstDtToCmb(dt, cmbCorner)
            cmbCorner.SelectedIndex = -1
            If cmbCorner.Items.Count <= 0 Then bRtn = False

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
    ''' 号機名称コンボボックスを設定する。
    ''' </summary>
    ''' <param name="Model">機種コード</param>
    ''' <param name="Station">駅コード</param>
    ''' <param name="Corner">コーナーコード</param>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理している号機名称の一覧及び「全号機」を設定する。</remarks>
    Private Function setCmbUnit(ByVal Model As String, ByVal Station As String, ByVal Corner As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New UnitMaster
        Dim sModel As String

        Try
            If Corner <> ClientDaoConstants.TERMINAL_ALL Then
                If Model = ClientDaoConstants.TERMINAL_ALL Then
                    sModel = "G,Y"
                Else
                    sModel = Model
                End If

                '号機名称コンボボックス用のデータを取得する。
                dt = oMst.SelectTable(Station, Corner, sModel)
                If dt.Rows.Count = 0 Then
                    '号機データ取得失敗
                    Return bRtn
                End If
            End If
            dt = oMst.SetAll()

            bRtn = BaseSetMstDtToCmb(dt, cmbUnit)
            cmbUnit.SelectedIndex = -1
            If cmbUnit.Items.Count <= 0 Then bRtn = False

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
    ''' マスタ名称コンボボックスを設定する。
    ''' </summary>
    ''' <param name="Model">機種コード</param>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理しているマスタ名称の一覧及び「全マスタ」を設定する。</remarks>
    Private Function setCmbMst(ByVal Model As String) As Boolean
        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New MasterMaster
        Dim sModel As String

        Try
            If Model = ClientDaoConstants.TERMINAL_ALL Then
                sModel = "G,Y"
            Else
                sModel = Model
            End If

            'マスタ名称コンボボックス用のデータを取得する。
            dt = oMst.SelectTable(sModel)
            If dt.Rows.Count = 0 Then
                'マスタデータ取得失敗
                Return bRtn
            End If
            dt = oMst.SetAll()

            bRtn = BaseSetMstDtToCmb(dt, cmbMst)
            cmbMst.SelectedIndex = -1
            If cmbMst.Items.Count <= 0 Then bRtn = False

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn

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

    ''' <summary>
    ''' 状態コンボボックスを設定する。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setCmbState()

        Me.cmbState.Items.Clear()

        Me.cmbState.Items.Add("全て")
        Me.cmbState.Items.Add("異常")
        Me.cmbState.Items.Add("正常")

        '「状態」を全てに設定する
        cmbState.SelectedIndex = 1

    End Sub

#End Region

#Region "コンボ選択イベント"

    ''' <summary>機種コンボ選択時</summary>
    ''' <remarks>
    ''' 対応する「機種」コンボボックスに値を代入し、他のコンボボックスのプロパティを設定する
    ''' </remarks>
    Private Sub cmbModel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            '駅名コンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbEki(cmbModel.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblEki.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            'マスタ名コンボ設定
            If setCmbMst(cmbModel.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMst.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbEki.SelectedIndex = 0               '★イベント発生箇所
            cmbMst.SelectedIndex = 0               '★イベント発生箇所

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblEki.Text)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub

    ''' <summary>駅名コンボ選択時</summary>
    ''' <remarks>
    ''' 対応する「コーナー」コンボボックスに値を代入し、他のコンボボックスのプロパティを設定する
    ''' </remarks>
    Private Sub cmbEki_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbEki.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            'コーナーコンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbCorner(cmbModel.SelectedValue.ToString, cmbEki.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbCorner.SelectedIndex = 0               '★イベント発生箇所

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblEki.Text)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub

    ''' <summary>コーナーコンボ選択時</summary>
    ''' <remarks>
    ''' 対応する「コーナー」コンボボックスに値を代入し、他のコンボボックスのプロパティを設定する
    ''' </remarks>
    Private Sub cmbCorner_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCorner.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()
        Try
            '号機コンボ設定
            LbEventStop = True      'イベント発生ＯＦＦ
            If setCmbUnit(cmbModel.SelectedValue.ToString, cmbEki.SelectedValue.ToString, cmbCorner.SelectedValue.ToString) = False Then
                'エラーメッセージ
                AlertBox.Show(Lexis.ComboBoxSetupFailed, lblUnit.Text)
                LbEventStop = False      'イベント発生ＯＮ
                btnReturn.Select()
                Exit Sub
            End If
            LbEventStop = False      'イベント発生ＯＮ
            cmbUnit.SelectedIndex = 0               '★イベント発生箇所

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.ComboBoxSetupFailed, lblMado.Text)
        Finally
            LfWaitCursor(False)
        End Try
    End Sub


    ''' <summary>号機コンボ選択時</summary>
    ''' <remarks>
    ''' 対応する「号機」コンボボックスに値を代入
    ''' </remarks>
    Private Sub cmbUnit_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbUnit.SelectedIndexChanged

        Call initElTable()

    End Sub

    ''' <summary>マスタ名称コンボ選択時</summary>
    ''' <remarks>
    ''' 対応する「マスタ名称」コンボボックスに値を代入
    ''' </remarks>
    Private Sub cmbMst_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbMst.SelectedIndexChanged

        Call initElTable()

    End Sub

    ''' <summary>状態名称コンボ選択時</summary>
    ''' <remarks>
    ''' 対応する「状態名称」コンボボックスに値を代入
    ''' </remarks>
    Private Sub cmbState_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbState.SelectedIndexChanged

        Call initElTable()

    End Sub

#End Region

#Region " 画面表示用SQL作成 "

    ''' <summary>画面表示用SQL作成</summary>
    ''' <returns>SQL文</returns>
    Private Function makeSql() As String

        Dim sSQL As String = ""
        Dim sSubSQL1 As String = ""
        Dim sSubSQL2 As String = ""
        Dim sSubSQL3 As String = ""

        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            sSubSQL1 = " AND (MAC.MODEL_CODE='G' OR MAC.MODEL_CODE='Y')"
            sSubSQL2 = " AND (VER.MODEL_CODE='G' OR VER.MODEL_CODE='Y')"
            sSubSQL3 = " WHERE (MODEL_CODE='G' OR MODEL_CODE='Y')"
        Else
            sSubSQL1 = " AND MAC.MODEL_CODE='" & cmbModel.SelectedValue.ToString & "'"
            sSubSQL2 = " AND VER.MODEL_CODE='" & cmbModel.SelectedValue.ToString & "'"
            sSubSQL3 = " WHERE MODEL_CODE='" & cmbModel.SelectedValue.ToString & "'"
        End If
        If cmbEki.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
            sSubSQL1 = sSubSQL1 & " AND MAC.RAIL_SECTION_CODE+MAC.STATION_ORDER_CODE='" & cmbEki.SelectedValue.ToString & "'"
            sSubSQL2 = sSubSQL2 & " AND VER.RAIL_SECTION_CODE+VER.STATION_ORDER_CODE='" & cmbEki.SelectedValue.ToString & "'"
            sSubSQL3 = sSubSQL3 & " AND RAIL_SECTION_CODE+STATION_ORDER_CODE='" & cmbEki.SelectedValue.ToString & "'"

            If cmbCorner.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
                sSubSQL1 = sSubSQL1 & " AND MAC.CORNER_CODE='" & cmbCorner.SelectedValue.ToString & "'"
                sSubSQL2 = sSubSQL2 & " AND VER.CORNER_CODE='" & cmbCorner.SelectedValue.ToString & "'"
                sSubSQL3 = sSubSQL3 & " AND CORNER_CODE='" & cmbCorner.SelectedValue.ToString & "'"

                If cmbUnit.SelectedValue.ToString <> "" Then
                    sSubSQL1 = sSubSQL1 & " AND MAC.UNIT_NO='" & cmbUnit.SelectedValue.ToString & "'"
                    sSubSQL2 = sSubSQL2 & " AND VER.UNIT_NO='" & cmbUnit.SelectedValue.ToString & "'"
                    sSubSQL3 = sSubSQL3 & " AND UNIT_NO='" & cmbUnit.SelectedValue.ToString & "'"
                End If
            End If
        End If
        If cmbMst.SelectedValue.ToString <> ClientDaoConstants.TERMINAL_ALL Then
            sSubSQL1 = sSubSQL1 & " AND MST.DATA_KIND='" & cmbMst.SelectedValue.ToString & "'"
            sSubSQL2 = sSubSQL2 & " AND VER.DATA_KIND='" & cmbMst.SelectedValue.ToString & "'"
            sSubSQL3 = sSubSQL3 & " AND DATA_KIND='" & cmbMst.SelectedValue.ToString & "'"
        End If

        sSQL = "SELECT STATION_NAME,CORNER_NAME,M.MODEL_NAME,M.UNIT_NO,M.NAME,ISNULL(V1.PATTERN_NAME,'')" _
            & "     AS PATTERN_NAME,ISNULL(V1.DATA_VERSION,'') AS VERSION1,ISNULL(V2.DATA_VERSION,'')" _
            & "     AS VERSION2," _
            & "     CASE" _
            & "         WHEN ISNULL(V1.DATA_VERSION,'') = ISNULL(V2.DATA_VERSION,'') THEN '正常'" _
            & "         ELSE '異常'" _
            & "     END AS STS," _
            & "     ISNULL(CONVERT(CHAR(10),V1.UPDATE_DATE,111)+' '+CONVERT(CHAR(8),V1.UPDATE_DATE,108),'')" _
            & "     AS UPDATE_DATE" _
            & " FROM" _
            & "     (" _
            & "         SELECT STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_NAME," _
            & "             CORNER_CODE,MAC.MODEL_CODE,MODEL_NAME,UNIT_NO,MST.DATA_KIND,MST.NAME" _
            & "         FROM" _
            & "             V_MACHINE_NOW AS MAC," _
            & "             M_MST_NAME AS MST" _
            & "         WHERE" _
            & "             MST.FILE_KBN='DAT' AND MST.USE_FLG='1'" _
            & "         AND MST.MODEL_CODE=MAC.MODEL_CODE" _
            & sSubSQL1 _
            & "     ) AS M" _
            & "     LEFT OUTER JOIN" _
            & "         (" _
            & "             SELECT RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,VER.MODEL_CODE," _
            & "                 UNIT_NO,DATA_KIND,DATA_SUB_KIND," _
            & "                 CASE WHEN PATTERN_NAME IS NULL" _
            & "                      THEN '['+CAST(VER.DATA_SUB_KIND AS varchar)+']'" _
            & "                      ELSE PATTERN_NAME END AS PATTERN_NAME," _
            & "                 DATA_VERSION,VER.UPDATE_DATE," _
            & "                 rank () over (partition by RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                     CORNER_CODE,VER.MODEL_CODE,UNIT_NO,DATA_KIND" _
            & "                     order by VER.UPDATE_DATE desc) AS 'ranking'" _
            & "             FROM" _
            & "                 D_MST_VER_INFO AS VER" _
            & "                 LEFT OUTER JOIN" _
            & "                 M_PATTERN_DATA AS PTN" _
            & "             ON  PTN.MODEL_CODE=VER.MODEL_CODE" _
            & "             AND PTN.MST_KIND=VER.DATA_KIND" _
            & "             AND PTN.PATTERN_NO=VER.DATA_SUB_KIND" _
            & sSubSQL2 _
            & "         ) AS V1" _
            & "     ON  M.RAIL_SECTION_CODE=V1.RAIL_SECTION_CODE" _
            & "     AND M.STATION_ORDER_CODE=V1.STATION_ORDER_CODE" _
            & "     AND M.CORNER_CODE=V1.CORNER_CODE" _
            & "     AND M.MODEL_CODE=V1.MODEL_CODE" _
            & "     AND M.UNIT_NO=V1.UNIT_NO" _
            & "     AND M.DATA_KIND=V1.DATA_KIND" _
            & "     AND V1.ranking='1'" _
            & "     LEFT OUTER JOIN" _
            & "         (" _
            & "             SELECT RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO," _
            & "                 DATA_KIND,DATA_SUB_KIND,DATA_VERSION,UPDATE_DATE," _
            & "                 rank () over (partition by RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                     CORNER_CODE,MODEL_CODE,UNIT_NO,DATA_KIND" _
            & "                     order by UPDATE_DATE desc) AS 'ranking'" _
            & "             FROM" _
            & "                 S_MST_VER_INFO_EXPECTED" _
            & sSubSQL3 _
            & "         ) AS V2" _
            & "     ON  M.RAIL_SECTION_CODE=V2.RAIL_SECTION_CODE" _
            & "     AND M.STATION_ORDER_CODE=V2.STATION_ORDER_CODE" _
            & "     AND M.CORNER_CODE=V2.CORNER_CODE" _
            & "     AND M.MODEL_CODE=V2.MODEL_CODE" _
            & "     AND M.UNIT_NO=V2.UNIT_NO" _
            & "     AND M.DATA_KIND=V2.DATA_KIND" _
            & "     AND V2.ranking='1'"

        If cmbState.SelectedIndex = 2 Then
            sSQL = sSQL & " WHERE ISNULL(V1.DATA_VERSION, '') = ISNULL (V2.DATA_VERSION, '')"
        ElseIf cmbState.SelectedIndex = 1 Then
            sSQL = sSQL & " WHERE ISNULL(V1.DATA_VERSION, '') <> ISNULL (V2.DATA_VERSION, '')"
        End If

        Return sSQL

    End Function

#End Region

#Region " ELTableのクリア "

    ''' <summary>ELTableのクリア</summary>
    ''' <remarks>
    ''' Eltableにあるデータをクリア
    ''' </remarks>
    Private Sub initElTable()

        'Eltableのカレントの最大桁数
        Dim sXYRange As String = ""

        '画面の閃きを防ぐため
        shtVerDetail.Redraw = False

        If shtVerDetail.MaxRows > 0 Then
            'Eltableのカレントの最大桁数を取得する。
            sXYRange = "1:" & shtVerDetail.MaxRows.ToString

            '選択されたエリアのデータをクリアする。
            shtVerDetail.Clear(New ElTabelleSheet.Range(sXYRange), ElTabelleSheet.DataTransferMode.DataOnly)
        End If

        shtVerDetail.MaxRows = 0
        If btnPrint.Enabled = True Then btnPrint.Enabled = False

        '画面の閃きを防ぐため
        shtVerDetail.Redraw = True

    End Sub

#End Region

#Region " Eltableの内容を表示する "

    ''' <summary>Eltableの内容を表示する</summary>
    ''' <remarks>
    ''' Eltableの内容を表示する
    ''' </remarks>
    ''' <param name="dt">検索結果</param>
    Private Sub displayData(ByVal dt As DataTable)
        Dim i As Integer

        '画面の閃きを防ぐ。
        Me.shtVerDetail.Redraw = False

        Try
            'Eltableの最大桁数を設定する。
            Me.shtVerDetail.MaxRows = dt.Rows.Count

            shtVerDetail.Rows.SetAllRowsHeight(21)

            'データのバインド。
            Me.shtVerDetail.DataSource = dt

            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i).Item("VERSION1").ToString <> dt.Rows(i).Item("VERSION2").ToString Then
                    shtVerDetail.Rows(i).BackColor = Color.Red
                End If
            Next

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SheetProcAbnormalEnd)
        Finally
            'Eltableを再表示する。
            Me.shtVerDetail.Redraw = True

        End Try

    End Sub

#End Region

#Region " ボタンの処理 "

    ''' <summary>「検索」ボタンの処理 </summary>
    ''' <remarks>
    ''' 「検索」ボタンを押下すると、画面で表示する
    ''' </remarks>
    Private Sub btnKensaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKensaku.Click
        Dim sSQL As String = ""
        Dim Cnt As Integer
        Dim dtData As New DataTable

        LogOperation(sender, e)    'ボタン押下ログ
        Try
            Call waitCursor(True)

            sSQL = makeSql()

            Cnt = BaseSqlDataTableFill(sSQL, dtData)
            Select Case Cnt
                Case -9             'ＤＢオープンエラー
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    btnReturn.Select()
                Case 0              '該当なし
                    AlertBox.Show(Lexis.NoRecordsFound)
                    cmbEki.Select()
                Case Else

                    '「出力」ボタン状態
                    If btnPrint.Enabled = False Then btnPrint.Enabled = True
                    'ELTableのクリア
                    Call initElTable()

                    'Eltableの内容を表示する。
                    Call displayData(dtData)

            End Select

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '検索失敗ログ
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ
            btnReturn.Select()

        Finally
            dtData = Nothing
            Call waitCursor(False)
        End Try
    End Sub

    ''' <summary>「終了」ボタンの処理 </summary>
    ''' <remarks>
    ''' 「終了」ボタンを押下すると、本画面が終了される
    ''' </remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        '終了ボタン押下。
        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()

    End Sub

#End Region

#Region "「出力」ボタンクリック"

    ''' <summary>「出力」ボタンクリック</summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks> 「出力」ボタンクリック</remarks>   
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
#End Region

#Region "マスタバージョン情報　帳票出力"
    ''' <summary>
    ''' [出力処理]
    ''' </summary>
    ''' <param name="sPath">ファイルフルパス</param>
    Private Sub LfXlsStart(ByVal sPath As String)
        Dim nRecCnt As Integer = 0
        Dim nStartRow As Integer = 6
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
                .Cell("K1").Value = OPMGFormConstants.OUT_TERMINAL + GetLedgerTitle()
                .Cell("K2").Value = OPMGFormConstants.OUT_DATE_TIME + Format(DateTime.Now, "yyyy/MM/dd HH:mm")
                .Cell("B3").Value = OPMGFormConstants.EQUIPMENT_TYPE + cmbModel.Text.Trim + "   " _
                                  + OPMGFormConstants.STATION_NAME + Me.cmbEki.Text.Trim + "  " _
                                  + OPMGFormConstants.CORNER_STR + Me.cmbCorner.Text.Trim + "  " _
                                  + OPMGFormConstants.NUM_EQUIPMENT + Me.cmbUnit.Text.Trim
                .Cell("B4").Value = OPMGFormConstants.MST_NAME + Me.cmbMst.Text.Trim + "  " _
                                  + OPMGFormConstants.STATUS_STR + Me.cmbState.Text.Trim

                ' 配信対象のデータ数を取得します
                nRecCnt = shtVerDetail.MaxRows

                ' データ数分の罫線枠を作成
                For i As Integer = 1 To nRecCnt - 1
                    .RowCopy(nStartRow, nStartRow + i)
                Next

                'データ数分の値セット
                For y As Integer = 0 To nRecCnt - 1
                    For x As Integer = 0 To LcstPrntCol.Length - 1
                        .Pos(x + 1, y + nStartRow).Value = shtVerDetail.Item(LcstPrntCol(x), y).Text
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
#End Region

End Class
