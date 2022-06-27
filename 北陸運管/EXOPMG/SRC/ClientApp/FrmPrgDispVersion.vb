' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
'   0.1      2014/06/03  (NES)中原    北陸対応（タブ・ボタン位置可変化）
'   0.2      2015/01/13  (NES)金沢    窓処対象外PG非表示対応
' **********************************************************************

Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess

''' <summary>プログラムバージョン表示</summary>
''' <remarks>
''' 運用管理サーバにて保持しているプログラムバージョンと端末機器で保持しているプログラムバージョンを比較し、差異があれば該当の駅を赤色表示する。
'''「駅」ボタンをクリックすることにより各駅に対応するバージョン詳細画面を表示する。
''' </remarks>
Public Class FrmPrgDispVersion
    Inherits FrmBase

#Region "定数の定義"
    'ボタンの高さを定義する
    Private Const BTNH As Integer = 48
    'ボタンの幅を定義する
    Private Const BTNW As Integer = 152
    'ページごとに表示するボタンの数を定義する
    Private Const BTNEKI_CNT As Integer = 50
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents lblModel As System.Windows.Forms.Label
    'タブページの高さを定義する
    Private Const BTNEKI_TABH As Integer = BTNH * 10

    ''' <summary>
    ''' 値変更によるイベント発生を防ぐフラグ
    ''' （True:イベント停止、False:イベント発生ＯＫ）
    ''' </summary>
    Private LbEventStop As Boolean

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
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents tabDspVer As System.Windows.Forms.TabControl
    Friend WithEvents btnGetData As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnGetData = New System.Windows.Forms.Button()
        Me.tabDspVer = New System.Windows.Forms.TabControl()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.pnlBodyBase.SuspendLayout()
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
        Me.pnlBodyBase.Controls.Add(Me.tabDspVer)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnGetData)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/07/31(水)  11:43"
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 4
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnGetData
        '
        Me.btnGetData.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnGetData.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnGetData.Location = New System.Drawing.Point(872, 520)
        Me.btnGetData.Name = "btnGetData"
        Me.btnGetData.Size = New System.Drawing.Size(128, 40)
        Me.btnGetData.TabIndex = 3
        Me.btnGetData.Text = "再表示"
        Me.btnGetData.UseVisualStyleBackColor = False
        '
        'tabDspVer
        '
        Me.tabDspVer.Alignment = System.Windows.Forms.TabAlignment.Bottom
        Me.tabDspVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.tabDspVer.Location = New System.Drawing.Point(48, 52)
        Me.tabDspVer.Name = "tabDspVer"
        Me.tabDspVer.SelectedIndex = 0
        Me.tabDspVer.Size = New System.Drawing.Size(772, 515)
        Me.tabDspVer.TabIndex = 2
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.Location = New System.Drawing.Point(90, 16)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(172, 21)
        Me.cmbModel.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(50, 19)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(45, 18)
        Me.lblModel.TabIndex = 52
        Me.lblModel.Text = "機種"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmPrgDispVersion
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgDispVersion"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "画面のデータを準備する"
    ''' <summary>画面のデータを準備する</summary>
    ''' <remarks>
    '''データを検索し、画面に表示する
    ''' </remarks>
    ''' <returns>データ準備フラグ：成功（True）、失敗（False）</returns>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        LbEventStop = True      'イベント発生ＯＦＦ

        Try
            Log.Info("Method started.")

            '機種名称コンボボックスを設定する。
            If setCmbModel() = False Then Exit Try
            cmbModel.SelectedIndex = 0            'デフォルト表示項目

            '-------Ver0.1　北陸対応　MOD START-----------
            'データ取得＆駅ボタン配置＆画面表示処理
            If reShowSelect() = False Then Exit Try
            '-------Ver0.1　北陸対応　MOD END-----------

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
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If
        End Try
        Return bRtn
    End Function
#End Region

#Region "フォームロード"

    ''' <summary>フォームロード</summary>
    Private Sub frmPrgDispVersion_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '画面タイトル
        lblTitle.Text = "プログラムバージョン表示"
        lblTitle.BackColor = Config.BackgroundColor
        lblToday.BackColor = Config.BackgroundColor
        pnlBodyBase.BackColor = Config.BackgroundColor
        lblModel.BackColor = Config.BackgroundColor
        btnGetData.BackColor = Config.ButtonColor
        btnReturn.BackColor = Config.ButtonColor

    End Sub
#End Region

#Region "コンボボックスを設定する。"
    ''' <summary>
    ''' 機種名称コンボボックスを設定する。
    ''' </summary>
    ''' <returns>設定結果：成功（True）、失敗（False）</returns>
    ''' <remarks>管理している機種名称の一覧及び「空白」を設定する。</remarks>
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
#End Region

#Region "データベースと接続して、返す結果集合をdtに渡す"

    ''' <summary>データベースと接続して、返す結果集合をdtに渡す</summary>
    Private Function conSql() As DataTable


        Dim sSql As String = ""
        Dim sModel As String = ""
        Dim dbCtl As DatabaseTalker
        Dim dt As DataTable
        dbCtl = New DatabaseTalker
        dt = New DataTable

        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            sModel = "G','Y','W"
        Else
            sModel = cmbModel.SelectedValue.ToString
        End If
        '-----Ver0.2　窓処対象外PG非表示対応　　ADD　START---------------------------------
        Dim CmbModelSql As String = ""
        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            CmbModelSql = " AND ( PRG.MODEL_CODE='W' OR PRG.MODEL_CODE='G' OR ((PRG.MODEL_CODE='Y') AND((PRG.UPDATE_DATE<>'' AND (VERSION1<>'' OR VERSION2<>'')) OR (PRG.UPDATE_DATE='' AND VERSION3<>''))))"
        ElseIf cmbModel.SelectedValue.ToString = "Y" Then
            CmbModelSql = " AND ((PRG.MODEL_CODE='Y') AND((PRG.UPDATE_DATE<>'' AND (VERSION1<>'' OR VERSION2<>'')) OR (PRG.UPDATE_DATE='' AND VERSION3<>'')))"
        End If
        '-----Ver0.2　窓処対象外PG非表示対応　　ADD　END-----------------------------------
        '-----Ver0.2　窓処対象外PG非表示対応　　MOD　START---------------------------------
        sSql = "SELECT M.STATION_NAME,M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE,MAX(M.STS) AS FLG" _
            & "  FROM" _
            & "      (" _
            & "          SELECT STATION_NAME,MAC.RAIL_SECTION_CODE,MAC.STATION_ORDER_CODE," _
            & "              MAC.MODEL_CODE,MAC.UNIT_NO,ELEMENT_ID," _
            & "              CASE" _
            & "                  WHEN ELEMENT_ID IS NULL THEN '0'" _
            & "                  WHEN VERSION1 = '' THEN '0'" _
            & "                  WHEN (VERSION1 = VERSION3) AND (VERSION2 = '') THEN '0'" _
            & "                  WHEN (VERSION2 = VERSION3) AND (VERSION3 <> '') THEN '1'" _
            & "                  ELSE '2'" _
            & "              END AS STS,UPDATE_DATE" _
            & "          FROM" _
            & "              (" _
            & "                  SELECT STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                      CORNER_CODE,MODEL_CODE,UNIT_NO" _
            & "                  FROM" _
            & "                      V_MACHINE_NOW" _
            & "                  WHERE" _
            & "                      MODEL_CODE IN ('" & sModel & "')" _
            & "              ) AS MAC" _
            & "              LEFT OUTER JOIN" _
            & "                  (" _
            & "                      SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                          CORNER_CODE,UNIT_NO,ELEMENT_ID," _
            & "                          MAX(VERSION1) AS VERSION1," _
            & "                          MAX(VERSION2) AS VERSION2," _
            & "                          MAX(VERSION3) AS VERSION3,MAX(UPDATE_DATE) AS UPDATE_DATE" _
            & "                      FROM" _
            & "                          (" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                                  CORNER_CODE,UNIT_NO,ELEMENT_ID,ELEMENT_VERSION AS VERSION1," _
            & "                                  '' AS VERSION2,'' AS VERSION3, UPDATE_DATE" _
            & "                              FROM" _
            & "                                  D_PRG_VER_INFO_CUR" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                              UNION" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                                  UNIT_NO,ELEMENT_ID,'' AS VERSION1,ELEMENT_VERSION AS VERSION2," _
            & "                                  '' AS VERSION3,UPDATE_DATE" _
            & "                              FROM" _
            & "                                  D_PRG_VER_INFO_NEW" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                              UNION" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                                  UNIT_NO,ELEMENT_ID,'' AS VERSION1,'' AS VERSION2," _
            & "                                  ELEMENT_VERSION AS VERSION3,'' AS UPDATE_DATE" _
            & "                              FROM" _
            & "                                  S_PRG_VER_INFO_EXPECTED" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                          ) AS PR" _
            & "                      GROUP BY" _
            & "                          MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                          UNIT_NO,ELEMENT_ID" _
            & "                  ) AS PRG" _
            & "              ON  MAC.RAIL_SECTION_CODE = PRG.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE = PRG.STATION_ORDER_CODE" _
            & "              AND MAC.CORNER_CODE = PRG.CORNER_CODE AND MAC.MODEL_CODE = PRG.MODEL_CODE" _
            & "              AND MAC.UNIT_NO = PRG.UNIT_NO" & CmbModelSql _
            & "      ) AS M" _
            & "  GROUP BY" _
            & "      M.STATION_NAME,M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE" _
            & "  ORDER BY" _
            & "      M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE"
        '-----Ver0.2　窓処対象外PG非表示対応　　MOD　END----------------------------------
        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSql)
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dt
    End Function

#End Region

#Region "バージョン表示画面を表示する"
    ''' <summary>バージョン表示画面を表示する</summary>
    ''' <remarks>
    ''' データベースからデータを取得し、dt,に渡す。dtのデータによって動的にtabpage,button,を作成する
    ''' </remarks>
    Public Function reShow() As Boolean
        Dim bRtn As Boolean = False

        'Tabpageページ数のループ変数
        Dim i As Integer = 0
        '各Tabpageにボタン数量のループ変数を表示する。
        Dim l As Integer = 0
        '行を単位とし、ボタンのループ変数を追加し、即ちボタンの幅
        Dim j As Integer = 0
        '列を単位とし、ボタンのループ変数を追加し、即ちボタンの高さ
        Dim k As Integer = 0
        '順次にdtにてボタン数量のループ変数をループする
        Dim t As Integer = 0

        Dim tabEki As TabPage

        'ボタンの数量
        Dim nBtnNum As Integer = 0
        'tabpageの数量
        Dim nPage As Integer = 0
        '各Tabpageボタンの数量
        Dim nBtnNumPage As Integer = 0

        'データベースから検出した結果集合を格納する
        Dim dtDispEki As DataTable = New DataTable

        Try
            '駅の名称、バージョンを検索する
            dtDispEki = Me.conSql()

            If dtDispEki.Rows.Count = 0 Then
                '検索条件に一致するデータは存在しない。
                If LbEventStop = False Then
                    AlertBox.Show(Lexis.NoRecordsFound)
                End If
                Return bRtn
            End If

            'ボタンの数量
            nBtnNum = dtDispEki.Rows.Count

            'tabpageの数量
            nPage = CType(Int(nBtnNum / BTNEKI_CNT), Integer)

            If nBtnNum Mod BTNEKI_CNT <> 0 Then
                nPage = nPage + 1
            End If

            'ローディングtabpage
            'タブページを動的に生成し、tabDspVerにロードする。
            For i = 0 To nPage - 1

                tabEki = New TabPage

                'tabEkiのプロパティを設定する
                tabEki.Text = getTabTitle(i, nBtnNum, nPage)

                tabEki.BorderStyle = BorderStyle.Fixed3D
                tabEki.Size = New System.Drawing.Size(764, 523)

                'tabDspVerにロードする
                Me.tabDspVer.Controls.Add(tabEki)

                'iページ目にボタン数を設定する。
                If i <> nPage - 1 Or (i = nPage - 1 And nBtnNum Mod BTNEKI_CNT = 0) Then
                    nBtnNumPage = BTNEKI_CNT - 1
                    '若し、最後のtabpageページである場合
                ElseIf i = nPage - 1 And nBtnNum Mod BTNEKI_CNT <> 0 Then
                    nBtnNumPage = nBtnNum Mod BTNEKI_CNT - 1
                End If

                j = 0
                k = 0

                'ローディングボタン
                For l = 0 To nBtnNumPage

                    Call Me.addBtnEki(tabEki, j, k, t, dtDispEki)

                    k = k + BTNH
                    t = t + 1
                    '改列
                    If (k = BTNEKI_TABH) Then
                        j = j + BTNW
                        k = 0
                    End If
                Next

            Next

            dtDispEki.Dispose()
            bRtn = True
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dtDispEki = Nothing
        End Try
        Return bRtn

    End Function
#End Region

#Region "tabpageのtextプロパティを設定する"

    ''' <summary>tabpageのtextプロパティを設定する</summary>
    ''' <param name="i">Tabpageページ数のループ変数</param>
    ''' <param name="nBtnNum">ボタンの数量</param>
    ''' <param name="nPage">tabpageの数量</param>
    ''' <remarks>
    ''' tabpagesのボタン数の表示範囲を確定する
    '''  </remarks>
    Private Function getTabTitle(ByVal i As Integer, ByVal nBtnNum As Integer, ByVal nPage As Integer) As String
        Dim sStartText As String = ""
        Dim sEndText As String = ""

        'タブ名称
        sStartText = (BTNEKI_CNT * i + 1).ToString

        If (i = nPage - 1) Then
            sEndText = nBtnNum.ToString
        Else
            sEndText = (BTNEKI_CNT * (i + 1)).ToString
        End If

        'tabpageラベルに本ページのボタン数の範囲を表示する。
        Return sStartText & "～" & sEndText

    End Function
#End Region

#Region "駅ボタンを追加"

    '''<summary> 駅ボタンを追加 </summary>
    ''' <param name="tab">ローディングするボタンのタブページ</param>
    ''' <param name="j">行を単位とし、ボタンのループ変数を追加し、即ちボタンの幅</param>
    ''' <param name="k">列を単位とし、ボタンのループ変数を追加し、即ちボタンの高さ</param>
    ''' <param name="t">順次にdtにてボタン数量のループ変数をループする</param>
    ''' <param name="dt">データベースから検出した結果集合を格納する</param>
    '''<remarks>
    ''' ボタンを新規作成する。ボタンのプロパティを設定し、tabpageに追加する。
    ''' </remarks>
    Private Sub addBtnEki(ByVal tab As TabPage, ByVal j As Integer, ByVal k As Integer, ByVal t As Integer, ByVal dt As DataTable)

        Dim btnEki As Button

        btnEki = New Button
        btnEki.Size = New Size(BTNW, BTNH)
        btnEki.Text = dt.Rows(t).Item("STATION_NAME").ToString
        btnEki.Name = dt.Rows(t).Item("RAIL_SECTION_CODE").ToString & dt.Rows(t).Item("STATION_ORDER_CODE").ToString
        btnEki.Tag = dt.Rows(t).Item("STATION_ORDER_CODE").ToString
        btnEki.Location = New Point(j, k)
        btnEki.FlatStyle = FlatStyle.Standard

        'ボタン背景色（BackColor）を設定する
        If (CType(dt.Rows(t).Item("FLG"), Integer) = 0) Then
            btnEki.BackColor = Color.White
        ElseIf (CType(dt.Rows(t).Item("FLG"), Integer) = 1) Then
            btnEki.BackColor = Color.Yellow
        Else
            btnEki.BackColor = Color.Red
        End If

        AddHandler btnEki.Click, AddressOf detail
        tab.Controls.Add(btnEki)

    End Sub
#End Region

#Region "「駅状態」ボタンクリック"
    ''' <summary>「駅状態」ボタンクリック</summary>
    ''' <remarks>
    ''' 駅ボタンが押下された場合の処理を行う,各駅に対応するバージョン詳細画面を表示する。
    ''' </remarks>
    Private Sub detail(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try

            Call waitCursor(True)
            '駅ボタン押下
            LogOperation(sender, e)    'ボタン押下ログ

            Dim oFrmPrgDispVersionDetail As New FrmPrgDispVersionDetail

            oFrmPrgDispVersionDetail.sCmbValue = cmbModel.SelectedIndex
            oFrmPrgDispVersionDetail.sBtnName = CType(sender, Button).Name.Substring(0, 3)
            oFrmPrgDispVersionDetail.sBtnTag = CType(sender, Button).Tag.ToString

            If oFrmPrgDispVersionDetail.InitFrmData() = False Then
                oFrmPrgDispVersionDetail = Nothing
                Exit Sub
            End If

            Me.Hide()
            oFrmPrgDispVersionDetail.ShowDialog()
            oFrmPrgDispVersionDetail.Dispose()
            Me.Show()

        Finally

            Call waitCursor(False)

        End Try
    End Sub
#End Region

#Region "「再表示」ボタンクリック"
    ''' <summary>「再表示」ボタンクリック</summary>
    ''' <remarks>
    ''' 「再表示」ボタンをクリックすることにより、各駅のバージョン情報を再取得し表示する。
    ''' </remarks>
    Private Sub btnGetData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetData.Click
        '再表示ボタン押下
        LogOperation(sender, e)    'ボタン押下ログ

        Try

            Call waitCursor(True)
            '再度のローディングを防ぐために'tabcontrol1をクリアする。
            Me.tabDspVer.TabPages.Clear()

            '-------Ver0.1　北陸対応　MOD START-----------
            'データ取得＆駅ボタン配置＆画面表示処理
            If reShowSelect() = False Then Exit Try
            '-------Ver0.1　北陸対応　MOD END-----------

        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '検索失敗メッセージ

        Finally

            Call waitCursor(False)

        End Try

    End Sub
#End Region

#Region "「終了」ボタンクリック"
    ''' <summary>「終了」ボタンクリック</summary>
    ''' <remarks >
    ''' 当画面を終了し、「プログラム管理メニュー」画面を表示する
    ''' </remarks >
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        '終了ボタン押下
        LogOperation(sender, e)    'ボタン押下ログ
        Me.Close()

    End Sub
#End Region

    ''' <summary>
    ''' 機種コンボ選択イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbModel_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            Me.tabDspVer.TabPages.Clear()

            '-------Ver0.1　北陸対応　MOD START-----------
            'データ取得＆駅ボタン配置＆画面表示処理
            If reShowSelect() = False Then Exit Try
            '-------Ver0.1　北陸対応　MOD END-----------

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub
    '-------Ver0.1　北陸対応　ADD START-----------
#Region "タブ名取得"
    Private Function getTab_Name() As DataTable
        Dim sSql As String = ""
        Dim dbCtl As DatabaseTalker
        Dim dt As DataTable
        dbCtl = New DatabaseTalker
        dt = New DataTable

        sSql = " SELECT DISTINCT TAB_ORDER,TAB_NAME FROM M_TAB_BTN WHERE TAB_NAME <> '' ORDER BY TAB_ORDER,TAB_NAME "

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSql)
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dt

    End Function
#End Region
#Region "駅ボタンを可変に表示"
    'OPT 未使用
    Private Function reshow2(ByVal dtTab As DataRow, ByVal dtBtn_Idx As DataTable) As Boolean
        Dim bRtn As Boolean = False
        'Tabpageページ数のループ変数
        Dim i As Integer = 0
        '各Tabpageにボタン数量のループ変数を表示する。
        Dim l As Integer = 0
        '行位置
        Dim j As Integer = 0
        '列位置
        Dim k As Integer = 0
        '順次にdtにてボタン数量のループ変数をループする
        Dim t As Integer = 0

        Dim tabEki As TabPage

        'ボタンの数量
        Dim nBtnNum As Integer = 0
        '各Tabpageボタンの数量
        Dim nBtnNumPage As Integer = 0

        'データベースから検出した結果集合を格納する
        Try
            'ボタンの数量
            nBtnNum = dtBtn_Idx.Rows.Count

            tabEki = New TabPage

            'tabEkiのプロパティを設定する
            tabEki.Text = dtTab.Item("TAB_NAME").ToString

            tabEki.BorderStyle = BorderStyle.Fixed3D
            tabEki.Size = New System.Drawing.Size(764, 523)

            'tabDspVerにロードする
            Me.tabDspVer.Controls.Add(tabEki)

            'ボタンを設定する。
            For l = 0 To nBtnNum - 1
                j = getRowPosition(CType(dtBtn_Idx.Rows(l).Item("ROW_ID"), Integer))
                k = getColumnPosition(CType(dtBtn_Idx.Rows(l).Item("COLUMN_ID"), Integer))
                Call Me.addBtnEki(tabEki, k, j, l, dtBtn_Idx)
            Next

            bRtn = True

        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        End Try

        Return bRtn

    End Function
#End Region

#Region "駅、ボタン配置情報を取得する"
    Private Function consql2(ByVal TabOrder As Integer, ByVal TabPage As String) As DataTable
        Dim sSql As String = ""
        Dim sModel As String = ""
        Dim dbCtl As DatabaseTalker
        Dim dt As DataTable
        dbCtl = New DatabaseTalker
        dt = New DataTable

        '「全機種」選択であればG：改札機、Y：窓口処理機、W：監視盤を機種に設定
        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            sModel = "G','Y','W"
        Else
            sModel = cmbModel.SelectedValue.ToString
        End If

        sSql = "SELECT M.STATION_NAME,M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE,MAX(M.STS) AS FLG,TAB_NAME,ROW_ID,COLUMN_ID" _
            & "  FROM" _
            & "      (" _
            & "          SELECT STATION_NAME,MAC.RAIL_SECTION_CODE,MAC.STATION_ORDER_CODE," _
            & "              MAC.MODEL_CODE,MAC.UNIT_NO,ELEMENT_ID," _
            & "              CASE" _
            & "                  WHEN ELEMENT_ID IS NULL THEN '0'" _
            & "                  WHEN VERSION1 = '' THEN '0'" _
            & "                  WHEN (VERSION1 = VERSION3) AND (VERSION2 = '') THEN '0'" _
            & "                  WHEN (VERSION2 = VERSION3) AND (VERSION3 <> '') THEN '1'" _
            & "                  ELSE '2'" _
            & "              END AS STS," _
            & "              TRC.TAB_NAME,TRC.ROW_ID,TRC.COLUMN_ID" _
            & "          FROM" _
            & "              (" _
            & "                  SELECT STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                      CORNER_CODE,MODEL_CODE,UNIT_NO" _
            & "                  FROM" _
            & "                      V_MACHINE_NOW" _
            & "                  WHERE" _
            & "                      MODEL_CODE IN ('" & sModel & "')" _
            & "              ) AS MAC" _
            & "              LEFT OUTER JOIN" _
            & "                  (" _
            & "                      SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                          CORNER_CODE,UNIT_NO,ELEMENT_ID," _
            & "                          MAX(VERSION1) AS VERSION1," _
            & "                          MAX(VERSION2) AS VERSION2," _
            & "                          MAX(VERSION3) AS VERSION3" _
            & "                      FROM" _
            & "                          (" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                                  CORNER_CODE,UNIT_NO,ELEMENT_ID,ELEMENT_VERSION AS VERSION1," _
            & "                                  '' AS VERSION2,'' AS VERSION3" _
            & "                              FROM" _
            & "                                  D_PRG_VER_INFO_CUR" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                              UNION" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                                  UNIT_NO,ELEMENT_ID,'' AS VERSION1,ELEMENT_VERSION AS VERSION2," _
            & "                                  '' AS VERSION3" _
            & "                              FROM" _
            & "                                  D_PRG_VER_INFO_NEW" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                              UNION" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                                  UNIT_NO,ELEMENT_ID,'' AS VERSION1,'' AS VERSION2," _
            & "                                  ELEMENT_VERSION AS VERSION3" _
            & "                              FROM" _
            & "                                  S_PRG_VER_INFO_EXPECTED" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                          ) AS PR" _
            & "                      GROUP BY" _
            & "                          MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                          UNIT_NO,ELEMENT_ID" _
            & "                  ) AS PRG" _
            & "              ON  MAC.RAIL_SECTION_CODE = PRG.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE = PRG.STATION_ORDER_CODE" _
            & "              AND MAC.CORNER_CODE = PRG.CORNER_CODE AND MAC.MODEL_CODE = PRG.MODEL_CODE" _
            & "              AND MAC.UNIT_NO = PRG.UNIT_NO" _
            & "              LEFT OUTER JOIN" _
            & "                  (" _
            & "                      SELECT TAB_ORDER,TAB_NAME,ROW_ID,COLUMN_ID,RAIL_SECTION_CODE,STATION_ORDER_CODE" _
            & "                      FROM M_TAB_BTN" _
            & "                      WHERE " _
            & "                          RAIL_SECTION_CODE <> ''" _
            & "                      AND STATION_ORDER_CODE <> ''" _
            & "                  ) AS TRC" _
            & "              ON  MAC.RAIL_SECTION_CODE = TRC.RAIL_SECTION_CODE" _
            & "              AND MAC.STATION_ORDER_CODE = TRC.STATION_ORDER_CODE" _
            & "          WHERE" _
            & "              TRC.RAIL_SECTION_CODE <> ''" _
            & "          AND TRC.STATION_ORDER_CODE <> ''" _
            & "          AND TRC.TAB_ORDER = '" & TabOrder & "'" _
            & "          AND TRC.TAB_NAME = '" & TabPage & "'" _
            & "      ) AS M" _
            & "  GROUP BY" _
            & "      M.STATION_NAME,M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE,M.TAB_NAME,M.ROW_ID,M.COLUMN_ID" _
            & "  ORDER BY" _
            & "      M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE"

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSql)
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try
        Return dt

    End Function

#End Region
#Region "行位置算出"
    Private Function getRowPosition(ByVal j As Integer) As Integer
        getRowPosition = (j - 1) * BTNH
    End Function
#End Region

#Region "列位置算出"
    Private Function getColumnPosition(ByVal k As Integer) As Integer
        getColumnPosition = (k - 1) * BTNW
    End Function
#End Region

#Region "駅ボタン配置位置が自動or可変を選択し、バージョン表示画面を表示する"
    ''' <summary>駅ボタン配置位置を選択しバージョン表示画面を表示する</summary>
    ''' <remarks>
    ''' 自動配置ならreShow()を呼び出し
    ''' 可変配置ならconsql2()、reShow2()を呼び出し
    ''' </remarks>
    Public Function reShowSelect() As Boolean
        Dim bRtn As Boolean = False
        Dim dtTab As DataTable
        Dim dtBtn_Idx As DataTable
        Dim i As Integer
        Dim initflg As Boolean = False

        Try
            'タブボタンマスタに登録があれば登録内容に従って駅ボタンを配置する
            dtTab = getTab_Name()
            If dtTab.Rows.Count > 0 Then
                For i = 0 To dtTab.Rows.Count - 1
                    'タブ内の駅のバージョン情報、配置位置情報を取得する
                    dtBtn_Idx = consql2(Integer.Parse(dtTab.Rows(i).Item("TAB_ORDER").ToString), dtTab.Rows(i).Item("TAB_NAME").ToString)
                    If dtBtn_Idx.Rows.Count > 0 Then
                        '駅ボタン配置
                        If reShow2(dtTab.Rows(i), dtBtn_Idx) = False Then Exit Try
                        initflg = True
                    End If
                Next
                '配置位置が１つも決まらなければ自動で駅ボタンを配置する
                If initflg = False Then
                    If reShow() = False Then Exit Try
                End If
            Else
                'タブボタンマスタに登録がなければ自動で駅ボタンを配置する
                If reShow() = False Then Exit Try
            End If

            bRtn = True

        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        End Try
        Return bRtn

    End Function
#End Region
    '-------Ver0.1　北陸対応　ADD END-----------
End Class
