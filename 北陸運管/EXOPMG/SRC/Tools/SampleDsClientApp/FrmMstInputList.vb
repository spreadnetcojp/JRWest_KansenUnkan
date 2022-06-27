' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇  新規作成
'   0.1      2013/05/13  (NES)小林  デ集クライアント試供アプリ化
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports System.IO

''' <summary>
''' マスタ適用リスト登録
''' </summary>
''' <remarks>マスタ管理メニューより「マスタ適用リスト登録」ボタンをクリックすると、本画面を表示する。
''' 本画面にてマスタ適用リストの読込み、登録を行う。</remarks>
Public Class FrmMstInputList
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

    Friend WithEvents dlgOpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnSaveData As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnOpenFile As System.Windows.Forms.Button
    Friend WithEvents lblTgl As System.Windows.Forms.Label
    Friend WithEvents lblPtnNa As System.Windows.Forms.Label
    Friend WithEvents lblBefore As System.Windows.Forms.Label
    Friend WithEvents lblAfter As System.Windows.Forms.Label
    Friend WithEvents lblTglName As System.Windows.Forms.Label
    Friend WithEvents lblPtnName As System.Windows.Forms.Label
    Friend WithEvents lblPtnNo As System.Windows.Forms.Label
    Friend WithEvents lblBeforeVer As System.Windows.Forms.Label
    Friend WithEvents lblPtnN As System.Windows.Forms.Label
    Friend WithEvents lblAfterVer As System.Windows.Forms.Label
    Friend WithEvents lblSave As System.Windows.Forms.Label
    Friend WithEvents lblSaveDT As System.Windows.Forms.Label
    Friend WithEvents lblMdl As System.Windows.Forms.Label
    Friend WithEvents lblModelName As System.Windows.Forms.Label


    Private Sub InitializeComponent()
        Me.dlgOpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.lblModelName = New System.Windows.Forms.Label()
        Me.lblMdl = New System.Windows.Forms.Label()
        Me.lblSaveDT = New System.Windows.Forms.Label()
        Me.lblSave = New System.Windows.Forms.Label()
        Me.lblAfterVer = New System.Windows.Forms.Label()
        Me.lblPtnN = New System.Windows.Forms.Label()
        Me.lblBeforeVer = New System.Windows.Forms.Label()
        Me.lblPtnNo = New System.Windows.Forms.Label()
        Me.lblPtnName = New System.Windows.Forms.Label()
        Me.lblTglName = New System.Windows.Forms.Label()
        Me.lblAfter = New System.Windows.Forms.Label()
        Me.lblBefore = New System.Windows.Forms.Label()
        Me.lblPtnNa = New System.Windows.Forms.Label()
        Me.lblTgl = New System.Windows.Forms.Label()
        Me.btnOpenFile = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnSaveData = New System.Windows.Forms.Button()
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
        Me.pnlBodyBase.Controls.Add(Me.lblMdl)
        Me.pnlBodyBase.Controls.Add(Me.lblSaveDT)
        Me.pnlBodyBase.Controls.Add(Me.lblSave)
        Me.pnlBodyBase.Controls.Add(Me.lblAfterVer)
        Me.pnlBodyBase.Controls.Add(Me.lblPtnN)
        Me.pnlBodyBase.Controls.Add(Me.lblBeforeVer)
        Me.pnlBodyBase.Controls.Add(Me.lblPtnNo)
        Me.pnlBodyBase.Controls.Add(Me.lblPtnName)
        Me.pnlBodyBase.Controls.Add(Me.lblTglName)
        Me.pnlBodyBase.Controls.Add(Me.lblAfter)
        Me.pnlBodyBase.Controls.Add(Me.lblBefore)
        Me.pnlBodyBase.Controls.Add(Me.lblPtnNa)
        Me.pnlBodyBase.Controls.Add(Me.lblTgl)
        Me.pnlBodyBase.Controls.Add(Me.btnOpenFile)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnSaveData)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/05/12(日)  21:13"
        '
        'dlgOpenFileDialog
        '
        Me.dlgOpenFileDialog.ReadOnlyChecked = True
        '
        'lblModelName
        '
        Me.lblModelName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModelName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.lblModelName.Location = New System.Drawing.Point(319, 181)
        Me.lblModelName.Name = "lblModelName"
        Me.lblModelName.Size = New System.Drawing.Size(251, 18)
        Me.lblModelName.TabIndex = 86
        Me.lblModelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMdl
        '
        Me.lblMdl.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblMdl.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMdl.Location = New System.Drawing.Point(135, 181)
        Me.lblMdl.Name = "lblMdl"
        Me.lblMdl.Size = New System.Drawing.Size(178, 18)
        Me.lblMdl.TabIndex = 85
        Me.lblMdl.Text = "機種名称"
        '
        'lblSaveDT
        '
        Me.lblSaveDT.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSaveDT.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSaveDT.Location = New System.Drawing.Point(319, 344)
        Me.lblSaveDT.Name = "lblSaveDT"
        Me.lblSaveDT.Size = New System.Drawing.Size(252, 16)
        Me.lblSaveDT.TabIndex = 84
        Me.lblSaveDT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSave
        '
        Me.lblSave.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblSave.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSave.Location = New System.Drawing.Point(135, 344)
        Me.lblSave.Name = "lblSave"
        Me.lblSave.Size = New System.Drawing.Size(160, 16)
        Me.lblSave.TabIndex = 83
        Me.lblSave.Text = "前回登録日時"
        '
        'lblAfterVer
        '
        Me.lblAfterVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfterVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfterVer.Location = New System.Drawing.Point(319, 438)
        Me.lblAfterVer.Name = "lblAfterVer"
        Me.lblAfterVer.Size = New System.Drawing.Size(251, 16)
        Me.lblAfterVer.TabIndex = 82
        Me.lblAfterVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnN
        '
        Me.lblPtnN.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnN.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnN.Location = New System.Drawing.Point(135, 298)
        Me.lblPtnN.Name = "lblPtnN"
        Me.lblPtnN.Size = New System.Drawing.Size(160, 16)
        Me.lblPtnN.TabIndex = 81
        Me.lblPtnN.Text = "(パターン番号)"
        '
        'lblBeforeVer
        '
        Me.lblBeforeVer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBeforeVer.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBeforeVer.Location = New System.Drawing.Point(319, 390)
        Me.lblBeforeVer.Name = "lblBeforeVer"
        Me.lblBeforeVer.Size = New System.Drawing.Size(251, 16)
        Me.lblBeforeVer.TabIndex = 80
        Me.lblBeforeVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnNo
        '
        Me.lblPtnNo.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnNo.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNo.Location = New System.Drawing.Point(319, 298)
        Me.lblPtnNo.Name = "lblPtnNo"
        Me.lblPtnNo.Size = New System.Drawing.Size(168, 16)
        Me.lblPtnNo.TabIndex = 79
        Me.lblPtnNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnName
        '
        Me.lblPtnName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnName.Location = New System.Drawing.Point(319, 280)
        Me.lblPtnName.Name = "lblPtnName"
        Me.lblPtnName.Size = New System.Drawing.Size(252, 18)
        Me.lblPtnName.TabIndex = 78
        Me.lblPtnName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTglName
        '
        Me.lblTglName.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTglName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTglName.Location = New System.Drawing.Point(319, 232)
        Me.lblTglName.Name = "lblTglName"
        Me.lblTglName.Size = New System.Drawing.Size(251, 18)
        Me.lblTglName.TabIndex = 77
        Me.lblTglName.Text = "１２３４５６７８９０１２３４５"
        Me.lblTglName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAfter
        '
        Me.lblAfter.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblAfter.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAfter.Location = New System.Drawing.Point(135, 438)
        Me.lblAfter.Name = "lblAfter"
        Me.lblAfter.Size = New System.Drawing.Size(160, 16)
        Me.lblAfter.TabIndex = 76
        Me.lblAfter.Text = "今回登録バージョン"
        '
        'lblBefore
        '
        Me.lblBefore.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblBefore.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblBefore.Location = New System.Drawing.Point(135, 390)
        Me.lblBefore.Name = "lblBefore"
        Me.lblBefore.Size = New System.Drawing.Size(160, 16)
        Me.lblBefore.TabIndex = 75
        Me.lblBefore.Text = "前回登録バージョン"
        '
        'lblPtnNa
        '
        Me.lblPtnNa.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblPtnNa.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNa.Location = New System.Drawing.Point(135, 280)
        Me.lblPtnNa.Name = "lblPtnNa"
        Me.lblPtnNa.Size = New System.Drawing.Size(160, 18)
        Me.lblPtnNa.TabIndex = 74
        Me.lblPtnNa.Text = "パターン名称"
        '
        'lblTgl
        '
        Me.lblTgl.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTgl.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTgl.Location = New System.Drawing.Point(135, 232)
        Me.lblTgl.Name = "lblTgl"
        Me.lblTgl.Size = New System.Drawing.Size(178, 18)
        Me.lblTgl.TabIndex = 73
        Me.lblTgl.Text = "マスタ適用リスト名称"
        '
        'btnOpenFile
        '
        Me.btnOpenFile.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnOpenFile.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnOpenFile.Location = New System.Drawing.Point(749, 298)
        Me.btnOpenFile.Name = "btnOpenFile"
        Me.btnOpenFile.Size = New System.Drawing.Size(128, 40)
        Me.btnOpenFile.TabIndex = 70
        Me.btnOpenFile.Text = "読　込"
        Me.btnOpenFile.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(749, 414)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 72
        Me.btnReturn.Text = "終　了"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnSaveData
        '
        Me.btnSaveData.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnSaveData.Enabled = False
        Me.btnSaveData.Font = New System.Drawing.Font("ＭＳ ゴシック", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSaveData.Location = New System.Drawing.Point(749, 356)
        Me.btnSaveData.Name = "btnSaveData"
        Me.btnSaveData.Size = New System.Drawing.Size(128, 40)
        Me.btnSaveData.TabIndex = 71
        Me.btnSaveData.Text = "登　録"
        Me.btnSaveData.UseVisualStyleBackColor = False
        '
        'FrmMstInputList
        '
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmMstInputList"
        Me.Text = "運用端末 "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
#End Region

#Region "宣言領域（Private）"

    Private sPathWithName As String = ""        'フルパスファイル名
    Private sFileName As String = ""            'ファイル名

    Private sMdlName As String = ""             '機種名称
    Private sMdlKind As String = ""             '機種コード
    Private sTglName As String = ""             'マスタ適用リスト名称
    Private sBeforVer As String = ""            '前回登録バージョン
    Private sUpDate As String = ""              '登録日時
    Private sPatternName As String = ""         'パターン名称
    Private sNewUpDate As String = ""           '最終登録日時

    Private sTglKind As String = ""             'マスタ適用リスト種別
    Private sPatternNo As String = ""           'パターン番号
    Private sAfterVer As String = ""            '今回登録バージョン
    Private sMstVer As String = ""              'マスタバージョン

    Private bSaved As Boolean = False           '登録完了

    Private ReadOnly LcstFormTitle As String = "マスタ適用リスト登録"

#End Region

#Region "イベント"

    Private Sub FrmMstInputList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
    ''' <remarks>「読込」ボタンをクリックすることにより外部媒体からマスタ適用リストを読込み、
    ''' 「機種名称」「マスタ適用リスト名称」「パターン名称」「（パターンNo）」
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
        sTglName = ""             'マスタ適用リスト名称
        sPatternName = ""         'パターン名称
        sPatternNo = ""           'パターン番号

        sBeforVer = ""            '前回登録バージョン
        sUpDate = ""              '登録日時
        sAfterVer = ""            '今回登録バージョン
        sMstVer = ""              'マスタバージョン

        sMdlKind = ""             '機種コード
        sTglKind = ""             'マスタ種別
        sNewUpDate = ""           '最終登録日時
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

        'パターン名称を取得する
        If getPatternFromDb() = False Then
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
    ''' <remarks>「終了」ボタンをクリックすることにより、「マスタ管理メニュー」画面に戻る。</remarks>
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
               EkMasProListFileName.GetListKind(sFileName).Equals("TGL") Then
                Me.sTglKind = EkMasProListFileName.GetDataKind(sFileName)
                Me.sPatternNo = EkMasProListFileName.GetDataSubKind(sFileName)
                Me.sMdlKind = EkMasProListFileName.GetDataApplicableModel(sFileName)
                Me.sMstVer = EkMasProListFileName.GetDataVersion(sFileName)
                Me.sAfterVer = EkMasProListFileName.GetListVersion(sFileName)
                bRtn = True
            Else
                '選択されたファイルはマスタ適用リストファイルではありません。
                AlertBox.Show(Lexis.TheFileTypeIsInvalid, "マスタ適用リストファイル")
                bRtn = False
            End If

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '"予期せぬエラーが発生しました。"
            '選択されたファイルはマスタ適用リストファイルではありません。
            AlertBox.Show(Lexis.TheFileTypeIsInvalid, "マスタ適用リストファイル")

        End Try

        Return bRtn

    End Function

    ''' <summary>
    ''' 「最新バージョン」及び登録日を取得する
    ''' </summary>
    ''' <remarks>マスタ管理テーブルを検索し、最新バージョン及び登録日を取得する。</remarks>
    Private Function getDataFromDb() As Boolean

        Me.sUpDate = "試供版のため表示できません"
        Me.sBeforVer = "試供版のため表示できません"
        Return True

    End Function

    Private Function checkKindFromDb() As Boolean

        Me.sTglName = "試供版のため表示できません"
        Me.sMdlName = "試供版のため表示できません"
        Return True

    End Function

    Private Function getPatternFromDb() As Boolean

        Me.sPatternName = "試供版のため表示できません"
        Return True

    End Function

    ''' <summary>
    ''' 各ラベルを設定し、表示する。
    ''' </summary>
    ''' <remarks>「マスタ名称」「パターン名称」「前回登録バージョン」「登録日時」「今回登録バージョン」を表示する。</remarks>
    Private Sub showLable()

        Me.lblModelName.Text = Me.sMdlName

        'ファイル名よりマスタ名称を表示
        Me.lblTglName.Text = Me.sTglName

        'ファイル名よりパターン名称を表示
        Me.lblPtnName.Text = Me.sPatternName

        'ファイル名よりパターン番号を表示
        Me.lblPtnNo.Text = "(" & sPatternNo & ")"

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
        lblTglName.Visible = bEnableLbl
        lblPtnName.Visible = bEnableLbl
        lblPtnNo.Visible = bEnableLbl
        lblBeforeVer.Visible = bEnableLbl
        lblAfterVer.Visible = bEnableLbl
        lblSaveDT.Visible = bEnableLbl

    End Sub

#End Region

End Class
