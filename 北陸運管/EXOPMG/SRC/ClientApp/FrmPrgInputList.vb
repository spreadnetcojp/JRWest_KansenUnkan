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
Imports System.IO

''' <summary>
''' プログラム適用リスト取込
''' </summary>
''' <remarks>プログラム管理メニューより「プログラム適用リスト取込」ボタンをクリックすると、本画面を表示する。
''' 本画面にてプログラム適用リストの読込み、登録を行う。</remarks>
Public Class FrmPrgInputList
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

    Friend WithEvents lblModelName As System.Windows.Forms.Label
    Friend WithEvents lblKisyu As System.Windows.Forms.Label
    Friend WithEvents lblSaveDT As System.Windows.Forms.Label
    Friend WithEvents lblSave As System.Windows.Forms.Label
    Friend WithEvents lblAfterVer As System.Windows.Forms.Label
    Friend WithEvents lblBeforeVer As System.Windows.Forms.Label
    Friend WithEvents lblTdlName As System.Windows.Forms.Label
    Friend WithEvents lblAppliedArea As System.Windows.Forms.Label
    Friend WithEvents lblAfter As System.Windows.Forms.Label
    Friend WithEvents lblBefore As System.Windows.Forms.Label
    Friend WithEvents lblTdlNa As System.Windows.Forms.Label
    Friend WithEvents lblPrm As System.Windows.Forms.Label
    Friend WithEvents btnOpenFile As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents dlgOpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnSaveData As System.Windows.Forms.Button

    Private Sub InitializeComponent()
        Me.lblModelName = New System.Windows.Forms.Label()
        Me.lblKisyu = New System.Windows.Forms.Label()
        Me.lblSaveDT = New System.Windows.Forms.Label()
        Me.lblSave = New System.Windows.Forms.Label()
        Me.lblAfterVer = New System.Windows.Forms.Label()
        Me.lblBeforeVer = New System.Windows.Forms.Label()
        Me.lblTdlName = New System.Windows.Forms.Label()
        Me.lblAppliedArea = New System.Windows.Forms.Label()
        Me.lblAfter = New System.Windows.Forms.Label()
        Me.lblBefore = New System.Windows.Forms.Label()
        Me.lblTdlNa = New System.Windows.Forms.Label()
        Me.lblPrm = New System.Windows.Forms.Label()
        Me.btnOpenFile = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnSaveData = New System.Windows.Forms.Button()
        Me.dlgOpenFileDialog = New System.Windows.Forms.OpenFileDialog()
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
        Me.pnlBodyBase.Controls.Add(Me.lblModelName)
        Me.pnlBodyBase.Controls.Add(Me.lblKisyu)
        Me.pnlBodyBase.Controls.Add(Me.lblSaveDT)
        Me.pnlBodyBase.Controls.Add(Me.lblSave)
        Me.pnlBodyBase.Controls.Add(Me.lblAfterVer)
        Me.pnlBodyBase.Controls.Add(Me.lblBeforeVer)
        Me.pnlBodyBase.Controls.Add(Me.lblTdlName)
        Me.pnlBodyBase.Controls.Add(Me.lblAppliedArea)
        Me.pnlBodyBase.Controls.Add(Me.lblAfter)
        Me.pnlBodyBase.Controls.Add(Me.lblBefore)
        Me.pnlBodyBase.Controls.Add(Me.lblTdlNa)
        Me.pnlBodyBase.Controls.Add(Me.lblPrm)
        Me.pnlBodyBase.Controls.Add(Me.btnOpenFile)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnSaveData)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/08/02(金)  15:26"
        '
        'lblModelName
        '
        Me.lblModelName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModelName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModelName.Location = New System.Drawing.Point(344, 202)
        Me.lblModelName.Name = "lblModelName"
        Me.lblModelName.Size = New System.Drawing.Size(114, 18)
        Me.lblModelName.TabIndex = 103
        Me.lblModelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblKisyu
        '
        Me.lblKisyu.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblKisyu.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKisyu.Location = New System.Drawing.Point(134, 202)
        Me.lblKisyu.Name = "lblKisyu"
        Me.lblKisyu.Size = New System.Drawing.Size(160, 18)
        Me.lblKisyu.TabIndex = 102
        Me.lblKisyu.Text = "機種"
        '
        'lblSaveDT
        '
        Me.lblSaveDT.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSaveDT.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSaveDT.Location = New System.Drawing.Point(526, 365)
        Me.lblSaveDT.Name = "lblSaveDT"
        Me.lblSaveDT.Size = New System.Drawing.Size(168, 16)
        Me.lblSaveDT.TabIndex = 101
        Me.lblSaveDT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSave
        '
        Me.lblSave.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSave.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSave.Location = New System.Drawing.Point(438, 365)
        Me.lblSave.Name = "lblSave"
        Me.lblSave.Size = New System.Drawing.Size(88, 16)
        Me.lblSave.TabIndex = 100
        Me.lblSave.Text = "登録日時："
        '
        'lblAfterVer
        '
        Me.lblAfterVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfterVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfterVer.Location = New System.Drawing.Point(317, 413)
        Me.lblAfterVer.Name = "lblAfterVer"
        Me.lblAfterVer.Size = New System.Drawing.Size(40, 16)
        Me.lblAfterVer.TabIndex = 99
        Me.lblAfterVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblBeforeVer
        '
        Me.lblBeforeVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBeforeVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBeforeVer.Location = New System.Drawing.Point(317, 365)
        Me.lblBeforeVer.Name = "lblBeforeVer"
        Me.lblBeforeVer.Size = New System.Drawing.Size(40, 16)
        Me.lblBeforeVer.TabIndex = 97
        Me.lblBeforeVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblTdlName
        '
        Me.lblTdlName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTdlName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTdlName.Location = New System.Drawing.Point(344, 300)
        Me.lblTdlName.Name = "lblTdlName"
        Me.lblTdlName.Size = New System.Drawing.Size(266, 18)
        Me.lblTdlName.TabIndex = 95
        Me.lblTdlName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAppliedArea
        '
        Me.lblAppliedArea.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAppliedArea.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAppliedArea.Location = New System.Drawing.Point(344, 253)
        Me.lblAppliedArea.Name = "lblAppliedArea"
        Me.lblAppliedArea.Size = New System.Drawing.Size(190, 18)
        Me.lblAppliedArea.TabIndex = 94
        Me.lblAppliedArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAfter
        '
        Me.lblAfter.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfter.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfter.Location = New System.Drawing.Point(134, 413)
        Me.lblAfter.Name = "lblAfter"
        Me.lblAfter.Size = New System.Drawing.Size(160, 16)
        Me.lblAfter.TabIndex = 93
        Me.lblAfter.Text = "今回登録バージョン"
        '
        'lblBefore
        '
        Me.lblBefore.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBefore.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBefore.Location = New System.Drawing.Point(134, 365)
        Me.lblBefore.Name = "lblBefore"
        Me.lblBefore.Size = New System.Drawing.Size(160, 16)
        Me.lblBefore.TabIndex = 92
        Me.lblBefore.Text = "前回登録バージョン"
        '
        'lblTdlNa
        '
        Me.lblTdlNa.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTdlNa.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTdlNa.Location = New System.Drawing.Point(134, 301)
        Me.lblTdlNa.Name = "lblTdlNa"
        Me.lblTdlNa.Size = New System.Drawing.Size(204, 18)
        Me.lblTdlNa.TabIndex = 91
        Me.lblTdlNa.Text = "プログラム適用リスト名称"
        '
        'lblPrm
        '
        Me.lblPrm.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPrm.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPrm.Location = New System.Drawing.Point(134, 253)
        Me.lblPrm.Name = "lblPrm"
        Me.lblPrm.Size = New System.Drawing.Size(160, 18)
        Me.lblPrm.TabIndex = 90
        Me.lblPrm.Text = "適用エリア名称"
        '
        'btnOpenFile
        '
        Me.btnOpenFile.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnOpenFile.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnOpenFile.Location = New System.Drawing.Point(748, 295)
        Me.btnOpenFile.Name = "btnOpenFile"
        Me.btnOpenFile.Size = New System.Drawing.Size(128, 40)
        Me.btnOpenFile.TabIndex = 87
        Me.btnOpenFile.Text = "読　込"
        Me.btnOpenFile.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(748, 411)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 89
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnSaveData
        '
        Me.btnSaveData.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnSaveData.Enabled = False
        Me.btnSaveData.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSaveData.Location = New System.Drawing.Point(748, 353)
        Me.btnSaveData.Name = "btnSaveData"
        Me.btnSaveData.Size = New System.Drawing.Size(128, 40)
        Me.btnSaveData.TabIndex = 88
        Me.btnSaveData.Text = "登　録"
        Me.btnSaveData.UseVisualStyleBackColor = False
        '
        'dlgOpenFileDialog
        '
        Me.dlgOpenFileDialog.ReadOnlyChecked = True
        '
        'FrmPrgInputList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgInputList"
        Me.Text = "運用端末"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
#End Region

#Region "宣言領域（Private）"

    Private sPathWithName As String = ""        'フルパスファイル名
    Private sFileName As String = ""            'ファイル名

    Private sMdlName As String = ""             '機種名称
    Private sMdlKind As String = ""             '機種コード
    Private sTdlName As String = ""             'プログラム適用リスト名称
    Private sBeforVer As String = ""            '前回登録バージョン
    Private sUpDate As String = ""              '登録日時
    Private sAreaName As String = ""            '適用エリア名称

    Private sTdlKind As String = ""             'プログラム適用リスト種別
    Private sAreaNo As String = ""              'パターン番号
    Private sAfterVer As String = ""            '今回登録バージョン
    Private sPrgVer As String = ""              'プログラムバージョン

    Private bSaved As Boolean = False           '登録完了

    Private ReadOnly LcstFormTitle As String = "プログラム適用リスト取込"

#End Region

#Region "イベント"

    ''' <summary>
    '''フォームロード 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FrmPrgInputList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Log.Info("Method started.")

        '画面タイトル、画面背景色（BackColor）を設定する
        lblTitle.Text = LcstFormTitle

        'ラベル非可視化
        Call setLbl(False)
        '登録ボタン非活性化
        Me.btnSaveData.Enabled = False

        Log.Info("Method ended.")
    End Sub
    ''' <summary>
    ''' 「読込」ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>「読込」ボタンをクリックすることにより外部媒体からプログラム適用リストを読込み、
    ''' 「機種名称」「適用エリア名称」「マスタ適用リスト名称」
    ''' 「前回登録バージョン」「登録日時」「今回登録バージョン」を表示する。</remarks>
    Private Sub btnOpenFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenFile.Click

        LogOperation(sender, e)    'ボタン押下ログ

        '「ファイルOpenダイアログ」を表示する。
        dlgOpenFileDialog.FileName = ""
        dlgOpenFileDialog.ShowDialog()

        'OpenFileDialogにてファイルを選択しない場合、操作を実施しない。
        If dlgOpenFileDialog.FileName = "" Then
            Exit Sub
        End If

        Call waitCursor(True)

        sPathWithName = dlgOpenFileDialog.FileName
        sFileName = ""            'ファイル名
        sMdlName = ""             '機種名称
        sTdlName = ""             'プログラム適用リスト名称
        sAreaName = ""            'エリア名称
        sAreaNo = ""              'エリア番号

        sBeforVer = ""            '前回登録バージョン
        sUpDate = ""              '登録日時
        sAfterVer = ""            '今回登録バージョン
        sPrgVer = ""              'プログラムバージョン

        sMdlKind = ""             '機種コード
        sTdlKind = ""             'プログラム種別
        bSaved = False            '登録完了

        'ラベル非可視化
        Call setLbl(False)

        '「登録」ボタン：非活性化
        Me.btnSaveData.Enabled = False

        '「ファイル名」を各コードを取得する
        If getDataFromFName(sPathWithName) = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        '各コードから名称を取得する
        If checkKindFromDb() = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        'エリア名称を取得する
        If getAreaFromDb() = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        '前回登録バージョンと登録日時を取得する
        If getDataFromDb() = False Then
            Call waitCursor(False)
            Exit Sub
        End If

        '取得情報を画面にセット
        Call showLable()

        'ラベル活性化
        Call setLbl(True)

        '「登録」ボタン活性化
        Me.btnSaveData.Enabled = True

        Call waitCursor(False)

    End Sub

    ''' <summary>
    '''「登録」ボタンクリック 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>「登録」ボタンをクリックすることにより、
    ''' 外部媒体より読込んだバージョンのプログラムデータを運用管理サーバに登録する。</remarks>
    Private Sub btnSaveData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveData.Click
        Try
            LogOperation(sender, e)    'ボタン押下ログ

            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUllMasProFile) = DialogResult.No Then
                LogOperation(Lexis.NoButtonClicked)     'Noボタン押下ログ
                Exit Sub
            End If

            LogOperation(Lexis.YesButtonClicked)     'Yesボタン押下ログ

            Call waitCursor(True)

            Me.bSaved = False

            If OpClientUtil.Connect() = False Then
                AlertBox.Show(Lexis.ConnectFailed)
                Exit Sub
            End If

            Dim ullResult As MasProUllResult = OpClientUtil.UllMasProFile(sPathWithName)

            OpClientUtil.Disconnect()

            Select Case ullResult
                Case MasProUllResult.Completed
                    Log.Info("MasProUllResponse with MasProUllResult.Completed received.")
                    AlertBox.Show(Lexis.UllMasProFileCompleted)
                Case MasProUllResult.Failed
                    Log.Info("MasProUllResponse with MasProUllResult.Failed received.")
                    AlertBox.Show(Lexis.UllMasProFileFailed)
                    Exit Sub
                Case MasProUllResult.FailedByBusy
                    Log.Info("MasProUllResponse with MasProUllResult.FailedByBusy received.")
                    AlertBox.Show(Lexis.UllMasProFileFailedByBusy)
                    Exit Sub
                Case MasProUllResult.FailedByInvalidContent
                    Log.Info("MasProUllResponse with MasProUllResult.FailedByInvalidContent received.")
                    AlertBox.Show(Lexis.UllMasProFileFailedByInvalidContent)
                    Exit Sub
                Case MasProUllResult.FailedByUnknownLight
                    Log.Info("MasProUllResponse with MasProUllResult.FailedByUnknownLight received.")
                    AlertBox.Show(Lexis.UllMasProFileFailedByUnknownLight)
                    Exit Sub
                Case Else
                    Log.Fatal("The telegrapher seems broken.")
                    AlertBox.Show(Lexis.UnforeseenErrorOccurred)
                    OpClientUtil.RestartBrokenTelegrapher()
                    Exit Sub
            End Select

            Me.bSaved = True

        Catch ex As OPMGException
            Log.Error("MasProUll failed.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.UnforeseenErrorOccurred)

        Finally
            Call waitCursor(False)
        End Try
    End Sub

    ''' <summary>
    ''' 「終了」ボタンクリック
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>「終了」ボタンをクリックすることにより、「プログラム管理メニュー」画面に戻る。</remarks>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        Dim oRet As Windows.Forms.DialogResult

        LogOperation(sender, e)    'ボタン押下ログ

        If Me.bSaved = False And Me.btnSaveData.Enabled = True Then
            'データが登録されていません。\n終了してもよろしいですか？
            oRet = AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyExitWithoutUll)
            If oRet = Windows.Forms.DialogResult.No Then
                LogOperation(Lexis.NoButtonClicked)     'Noボタン押下ログ
                Exit Sub
            End If

            LogOperation(Lexis.YesButtonClicked)     'Yesボタン押下ログ
        End If

        Me.Close()
    End Sub

#End Region

#Region "メソッド（Private）"

    ''' <summary>
    ''' 「ファイル名」 を取得する。
    ''' </summary>
    ''' <remarks>ファイルダイアログを表示し、指定されたファイル名を取得する。
    ''' ファイル名を各コード単位に分割する。</remarks>
    '''  <returns>成功（True）、失敗（False）</returns>
    Private Function getDataFromFName(ByVal sPath As String) As Boolean

        Dim bRtn As Boolean = False

        Try
            'ファイル名が「TGL_XXX99_X_999_99.csv」型式かをチェック
            Me.sFileName = Path.GetFileName(sPath)
            If EkMasProListFileName.IsValid(sFileName) AndAlso _
               EkMasProListFileName.GetListKind(sFileName).Equals("TDL") Then
                Me.sTdlKind = EkMasProListFileName.GetDataKind(sFileName)
                Me.sAreaNo = EkMasProListFileName.GetDataSubKind(sFileName)
                Me.sMdlKind = EkMasProListFileName.GetDataApplicableModel(sFileName)
                Me.sPrgVer = EkMasProListFileName.GetDataVersion(sFileName)
                Me.sAfterVer = EkMasProListFileName.GetListVersion(sFileName)
                bRtn = True
            Else
                '選択されたファイルはプログラム適用リストファイルではありません。
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "プログラム適用リストファイル")
                bRtn = False
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"予期せぬエラーが発生しました。"
            '選択されたファイルはプログラム適用リストファイルではありません。
            AlertBox.Show(Lexis.TheFileTypeIsInvalid, "プログラム適用リストファイル")

        End Try

        Return bRtn

    End Function

    ''' <summary>
    ''' 「最新バージョン」及び登録日を取得する
    ''' </summary>
    ''' <remarks>プログラム管理テーブルを検索し、最新バージョン及び登録日を取得する。</remarks>
    Private Function getDataFromDb() As Boolean

        Dim bRtn As Boolean = False
        Dim sSQL As String = ""
        Dim dbCtl As New DatabaseTalker
        Dim dtTable As New DataTable

        'DBオープン
        Try
            dbCtl.ConnectOpen()
        Catch ex As DatabaseException

        End Try

        'DB接続に失敗しました
        If dbCtl.IsConnect = False Then
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Return bRtn
        End If


        'DB登録されている最新バージョンを取得
        Try
            sSQL = "SELECT TOP 1 UPDATE_DATE, LIST_VERSION FROM S_PRG_LIST_HEADLINE" _
                & " WHERE MODEL_CODE = '" & Me.sMdlKind & "'" _
                & " AND DATA_KIND = '" & Me.sTdlKind & "'" _
                & " AND DATA_SUB_KIND = '" & Me.sAreaNo & "'" _
                & " AND DATA_VERSION = '" & Me.sPrgVer & "'" _
                & " ORDER BY UPDATE_DATE DESC"
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)

            '前回の登録日付とバージョンをセット
            If dtTable.Rows.Count = 1 Then
                Me.sUpDate = Format(Convert.ToDateTime(dtTable.Rows(0).Item("UPDATE_DATE")), "yyyy/MM/dd HH:mm:ss")
                Me.sBeforVer = dtTable.Rows(0).Item("LIST_VERSION").ToString
            End If

            bRtn = True

        Catch ex As Exception
            '接続処理に失敗しました
            AlertBox.Show(Lexis.ConnectFailed)

        Finally
            dbCtl.ConnectClose()

        End Try

        Return bRtn

    End Function

    Private Function checkKindFromDb() As Boolean

        Dim bRtn As Boolean = False
        Dim sSQL As String = ""
        Dim dbCtl As New DatabaseTalker
        Dim dtTable As New DataTable

        'DBオープン
        Try
            dbCtl.ConnectOpen()
        Catch ex As DatabaseException

        End Try

        'DB接続に失敗しました
        If dbCtl.IsConnect = False Then
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Return bRtn
        End If


        'プログラム名称、機種名称の取得。
        Try
            sSQL = "SELECT PRG.NAME AS PRG_NAME, MDL.MODEL_NAME FROM M_PRG_NAME AS PRG, M_MODEL AS MDL" _
                   & " where PRG.MODEL_CODE = MDL.MODEL_CODE AND PRG.FILE_KBN = 'LST'" _
                   & " AND MDL.MODEL_CODE = '" & Me.sMdlKind & "'" _
                   & " AND PRG.DATA_KIND ='" & Me.sTdlKind & "'"
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)

            If dtTable.Rows.Count > 0 Then
                Me.sTdlName = dtTable.Rows(0).Item("PRG_NAME").ToString
                Me.sMdlName = dtTable.Rows(0).Item("MODEL_NAME").ToString
                bRtn = True
            Else
                '選択されたファイルはプログラム適用リストファイルではありません。
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "プログラム適用リストファイル")
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"予期せぬエラーが発生しました。"
            '接続処理に失敗しました
            AlertBox.Show(Lexis.ConnectFailed)

        Finally
            dbCtl.ConnectClose()

        End Try

        Return bRtn

    End Function

    Private Function getAreaFromDb() As Boolean

        Dim bRtn As Boolean = False
        Dim sSQL As String = ""
        Dim dbCtl As New DatabaseTalker
        Dim dtTable As New DataTable

        'DBオープン
        Try
            dbCtl.ConnectOpen()
        Catch ex As DatabaseException

        End Try

        'DB接続に失敗しました
        If dbCtl.IsConnect = False Then
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Return bRtn
        End If


        'エリア名称の取得。
        Try
            sSQL = "SELECT AREA_NAME FROM M_AREA_DATA" _
                   & " WHERE MODEL_CODE = '" & Me.sMdlKind & "'" _
                   & " AND AREA_NO ='" & Me.sAreaNo & "'"
            dtTable = dbCtl.ExecuteSQLToRead(sSQL)

            If dtTable.Rows.Count > 0 Then
                Me.sAreaName = dtTable.Rows(0).Item("AREA_NAME").ToString
                bRtn = True
            Else
                'エリアデータが登録されていません。
                AlertBox.Show(Lexis.TheAreaNoDoesNotExist)
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"予期せぬエラーが発生しました。"
            '接続処理に失敗しました
            AlertBox.Show(Lexis.ConnectFailed)

        Finally
            dbCtl.ConnectClose()

        End Try

        Return bRtn

    End Function

    ''' <summary>
    ''' 各ラベルを設定し、表示する。
    ''' </summary>
    ''' <remarks>「エリア名称」「前回登録バージョン」「登録日時」「今回登録バージョン」を表示する。</remarks>
    Private Sub showLable()

        Me.lblModelName.Text = Me.sMdlName

        'ファイル名よりマスタ名称を表示
        Me.lblTdlName.Text = Me.sTdlName

        'ファイル名よりエリア名称を表示
        Me.lblAppliedArea.Text = Me.sAreaName

        'DBを検索し読込んだマスタの前回登録バージョンを表示
        '前回登録データが存在しない場合は、「空白」を表示
        Me.lblBeforeVer.Text = Me.sBeforVer

        'DBを検索し読込んだマスタの前回登録日時を表示
        '前回登録データが存在しない場合は、「空白」を表示
        Me.lblSaveDT.Text = Me.sUpDate

        'ファイルの内容を検索して表示
        Me.lblAfterVer.Text = Me.sAfterVer

    End Sub

    ''' <summary>
    ''' ラベル可視性の設定。
    ''' </summary>
    ''' <param name="bEnableLbl">各ラベルの可視性</param>
    Private Sub setLbl(ByVal bEnableLbl As Boolean)

        lblModelName.Visible = bEnableLbl
        lblTdlName.Visible = bEnableLbl
        lblAppliedArea.Visible = bEnableLbl
        lblBeforeVer.Visible = bEnableLbl
        lblAfterVer.Visible = bEnableLbl
        lblSaveDT.Visible = bEnableLbl

    End Sub

#End Region


End Class
